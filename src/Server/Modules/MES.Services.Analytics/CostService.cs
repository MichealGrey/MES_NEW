using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Analytics;

public class CostService : ICostService
{
    private readonly MesDbContext _context;
    private readonly ILogger<CostService> _logger;

    public CostService(MesDbContext context, ILogger<CostService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CostRecordResponse> RecordCostAsync(CostCalculationRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var costId = $"COST-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
        var today = DateOnly.FromDateTime(now);

        var record = new CostRecord
        {
            CostId = costId,
            WorkOrderId = request.WorkOrderId,
            ProductId = request.ProductId,
            CostDate = today,
            CostType = "DirectMaterial",
            Amount = request.DirectMaterialCost,
            Currency = "CNY",
            CreatedBy = operatorId,
            CreatedAt = now
        };

        _context.CostRecords.Add(record);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Recorded cost {CostId} for WO {WorkOrderId}", costId, request.WorkOrderId);

        return MapToResponse(record);
    }

    public async Task<CostAnalysisResponse> GetProductAnalysisAsync(string productId, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var query = _context.CostRecords.Where(x => x.ProductId == productId);

        if (startDate.HasValue)
            query = query.Where(x => x.CostDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(x => x.CostDate <= endDate.Value);

        var records = await query.ToListAsync();

        var directMaterial = records.Where(x => x.CostType == "DirectMaterial").Sum(x => x.Amount);
        var directLabor = records.Where(x => x.CostType == "DirectLabor").Sum(x => x.Amount);
        var overhead = records.Where(x => x.CostType == "ManufacturingOverhead").Sum(x => x.Amount);
        var total = directMaterial + directLabor + overhead;

        var workOrders = records.Select(x => x.WorkOrderId).Distinct().ToList();
        int completedQty = 0; // Would normally query from work orders
        var unitCost = completedQty > 0 ? total / completedQty : 0;

        return new CostAnalysisResponse
        {
            WorkOrderId = workOrders.FirstOrDefault() ?? string.Empty,
            ProductId = productId,
            DirectMaterialCost = directMaterial,
            DirectLaborCost = directLabor,
            ManufacturingOverhead = overhead,
            TotalCost = total,
            CompletedQty = completedQty,
            UnitCost = unitCost,
            CostRecords = records.Select(MapToResponse).ToList()
        };
    }

    public async Task<CostAnalysisResponse> GetWorkOrderAnalysisAsync(string workOrderId)
    {
        var records = await _context.CostRecords
            .Where(x => x.WorkOrderId == workOrderId)
            .ToListAsync();

        if (records.Count == 0)
            throw new KeyNotFoundException($"No cost records found for work order '{workOrderId}'");

        var directMaterial = records.Where(x => x.CostType == "DirectMaterial").Sum(x => x.Amount);
        var directLabor = records.Where(x => x.CostType == "DirectLabor").Sum(x => x.Amount);
        var overhead = records.Where(x => x.CostType == "ManufacturingOverhead").Sum(x => x.Amount);
        var total = directMaterial + directLabor + overhead;

        return new CostAnalysisResponse
        {
            WorkOrderId = workOrderId,
            ProductId = records.First().ProductId,
            DirectMaterialCost = directMaterial,
            DirectLaborCost = directLabor,
            ManufacturingOverhead = overhead,
            TotalCost = total,
            CompletedQty = 0,
            UnitCost = 0,
            CostRecords = records.Select(MapToResponse).ToList()
        };
    }

    public async Task<List<CostRecordResponse>> GetVarianceAnalysisAsync(string workOrderId)
    {
        var records = await _context.CostRecords
            .Where(x => x.WorkOrderId == workOrderId)
            .OrderBy(x => x.CostDate)
            .ToListAsync();

        return records.Select(MapToResponse).ToList();
    }

    public async Task<List<CostRecordResponse>> CalculateCostsAsync(CostCalculationRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);
        var results = new List<CostRecordResponse>();

        var costTypes = new[]
        {
            ("DirectMaterial", request.DirectMaterialCost),
            ("DirectLabor", request.DirectLaborCost),
            ("ManufacturingOverhead", request.ManufacturingOverhead)
        };

        foreach (var (costType, amount) in costTypes)
        {
            var costId = $"COST-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
            var record = new CostRecord
            {
                CostId = costId,
                WorkOrderId = request.WorkOrderId,
                ProductId = request.ProductId,
                CostDate = today,
                CostType = costType,
                Amount = amount,
                Currency = "CNY",
                CreatedBy = operatorId,
                CreatedAt = now
            };

            _context.CostRecords.Add(record);
            results.Add(MapToResponse(record));
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Calculated costs for WO {WorkOrderId}", request.WorkOrderId);

        return results;
    }

    private static CostRecordResponse MapToResponse(CostRecord entity) => new()
    {
        CostId = entity.CostId,
        WorkOrderId = entity.WorkOrderId,
        ProductId = entity.ProductId,
        CostDate = entity.CostDate,
        CostType = entity.CostType,
        Amount = entity.Amount,
        Currency = entity.Currency,
        DetailJson = entity.DetailJson,
        CreatedBy = entity.CreatedBy,
        CreatedAt = entity.CreatedAt
    };
}
