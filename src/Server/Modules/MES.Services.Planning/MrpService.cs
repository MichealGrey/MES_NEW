using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase2;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Planning;

public class MrpService : IMrpService
{
    private readonly MesDbContext _context;
    private readonly ILogger<MrpService> _logger;

    public MrpService(MesDbContext context, ILogger<MrpService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BomResponse> CreateBomAsync(CreateBomRequest request, string operatorId)
    {
        var bomId = $"BOM-{request.ProductId}-{request.BomVersion}";

        var existing = await _context.Boms.FirstOrDefaultAsync(b => b.BomId == bomId);
        if (existing != null)
            throw new InvalidOperationException($"BOM {bomId} already exists");

        var now = DateTime.UtcNow;
        var bom = new Bom
        {
            BomId = bomId,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            BomVersion = request.BomVersion,
            EffectiveDate = request.EffectiveDate,
            ExpiryDate = request.ExpiryDate,
            TotalItems = request.Items.Count,
            Status = "Draft",
            CreatedBy = operatorId
        };

        _context.Boms.Add(bom);

        foreach (var item in request.Items)
        {
            var bomItem = new BomItem
            {
                BomId = bomId,
                MaterialId = item.MaterialId,
                MaterialName = item.MaterialName,
                MaterialSpec = item.MaterialSpec,
                QuantityPerUnit = item.QuantityPerUnit,
                Unit = item.Unit,
                LossRate = item.LossRate,
                SortOrder = item.SortOrder,
                Remark = item.Remark
            };
            _context.BomItems.Add(bomItem);
        }

        await _context.SaveChangesAsync();

        return await GetBomResponseAsync(bomId);
    }

    public async Task<BomResponse> GetBomAsync(string bomId)
    {
        return await GetBomResponseAsync(bomId);
    }

    public async Task<List<BomResponse>> GetBomsByProductAsync(string productId)
    {
        var boms = await _context.Boms
            .Where(b => b.ProductId == productId && b.Status == "Active")
            .OrderByDescending(b => b.BomVersion)
            .ToListAsync();

        var result = new List<BomResponse>();
        foreach (var bom in boms)
        {
            result.Add(await GetBomResponseAsync(bom.BomId));
        }
        return result;
    }

    public async Task<MrpCalculationResponse> CalculateMrpAsync(MrpCalculationRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var calculationId = $"MRP-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        // Mock MRP calculation - in production this would calculate actual material requirements
        var calculation = new MrpCalculation
        {
            CalculationId = calculationId,
            PlanId = request.PlanId,
            WorkOrderId = request.WorkOrderId,
            CalculationType = request.CalculationType,
            Status = "Completed",
            TotalDemandItems = 15,
            ShortageItems = 3,
            SufficientItems = 12,
            CalculationParams = request.CalculationParams,
            ResultSummary = "MRP计算完成，发现3项缺料",
            ResultData = "{}",
            CreatedBy = operatorId
        };

        _context.MrpCalculations.Add(calculation);

        // Create mock shortage warnings
        var mockShortages = new[]
        {
            new { MaterialId = "MAT-001", MaterialName = "引线框架", Required = 5000m, Available = 3000m, Severity = "High" },
            new { MaterialId = "MAT-002", MaterialName = "金线", Required = 1000m, Available = 800m, Severity = "Medium" },
            new { MaterialId = "MAT-003", MaterialName = "塑封料", Required = 2000m, Available = 1500m, Severity = "Low" }
        };

        foreach (var s in mockShortages)
        {
            var warning = new MrpShortageWarning
            {
                CalculationId = calculationId,
                MaterialId = s.MaterialId,
                MaterialName = s.MaterialName,
                RequiredQty = s.Required,
                AvailableQty = s.Available,
                ShortageQty = s.Required - s.Available,
                Severity = s.Severity,
                Status = "Open"
            };
            _context.MrpShortageWarnings.Add(warning);
        }

        await _context.SaveChangesAsync();

        return new MrpCalculationResponse
        {
            CalculationId = calculation.CalculationId,
            PlanId = calculation.PlanId,
            WorkOrderId = calculation.WorkOrderId,
            CalculationType = calculation.CalculationType,
            Status = calculation.Status,
            TotalDemandItems = calculation.TotalDemandItems,
            ShortageItems = calculation.ShortageItems,
            SufficientItems = calculation.SufficientItems,
            ResultSummary = calculation.ResultSummary,
            CreatedBy = calculation.CreatedBy,
            CreatedAt = calculation.CreatedAt
        };
    }

    public async Task<PagedResult<MrpShortageWarningResponse>> GetShortageWarningsAsync(MrpQuery query)
    {
        var iqQuery = _context.MrpShortageWarnings.AsQueryable();

        if (!string.IsNullOrEmpty(query.CalculationId))
            iqQuery = iqQuery.Where(w => w.CalculationId == query.CalculationId);
        if (!string.IsNullOrEmpty(query.Severity))
            iqQuery = iqQuery.Where(w => w.Severity == query.Severity);
        if (!string.IsNullOrEmpty(query.Status))
            iqQuery = iqQuery.Where(w => w.Status == query.Status);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(w => w.WarningId)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(w => new MrpShortageWarningResponse
            {
                WarningId = w.WarningId,
                CalculationId = w.CalculationId,
                MaterialId = w.MaterialId,
                MaterialName = w.MaterialName,
                RequiredQty = w.RequiredQty,
                AvailableQty = w.AvailableQty,
                ShortageQty = w.ShortageQty,
                ExpectedArrival = w.ExpectedArrival,
                PurchaseOrderNo = w.PurchaseOrderNo,
                Severity = w.Severity,
                Status = w.Status,
                CreatedAt = w.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<MrpShortageWarningResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<MrpCalculationResponse> GetMrpCalculationAsync(string calculationId)
    {
        var calculation = await _context.MrpCalculations
            .FirstOrDefaultAsync(c => c.CalculationId == calculationId);

        if (calculation == null)
            throw new KeyNotFoundException($"MRP calculation {calculationId} not found");

        return new MrpCalculationResponse
        {
            CalculationId = calculation.CalculationId,
            PlanId = calculation.PlanId,
            WorkOrderId = calculation.WorkOrderId,
            CalculationType = calculation.CalculationType,
            Status = calculation.Status,
            TotalDemandItems = calculation.TotalDemandItems,
            ShortageItems = calculation.ShortageItems,
            SufficientItems = calculation.SufficientItems,
            ResultSummary = calculation.ResultSummary,
            CreatedBy = calculation.CreatedBy,
            CreatedAt = calculation.CreatedAt
        };
    }

    private async Task<BomResponse> GetBomResponseAsync(string bomId)
    {
        var bom = await _context.Boms.FirstOrDefaultAsync(b => b.BomId == bomId);
        if (bom == null)
            throw new KeyNotFoundException($"BOM {bomId} not found");

        var items = await _context.BomItems
            .Where(i => i.BomId == bomId)
            .OrderBy(i => i.SortOrder)
            .Select(i => new BomItemResponse
            {
                ItemId = i.ItemId,
                BomId = i.BomId,
                MaterialId = i.MaterialId,
                MaterialName = i.MaterialName,
                MaterialSpec = i.MaterialSpec,
                QuantityPerUnit = i.QuantityPerUnit,
                Unit = i.Unit,
                LossRate = i.LossRate,
                SortOrder = i.SortOrder,
                Remark = i.Remark
            })
            .ToListAsync();

        return new BomResponse
        {
            BomId = bom.BomId,
            ProductId = bom.ProductId,
            ProductName = bom.ProductName,
            BomVersion = bom.BomVersion,
            Status = bom.Status,
            EffectiveDate = bom.EffectiveDate,
            ExpiryDate = bom.ExpiryDate,
            TotalItems = bom.TotalItems,
            CreatedBy = bom.CreatedBy,
            CreatedAt = bom.CreatedAt,
            Items = items
        };
    }
}
