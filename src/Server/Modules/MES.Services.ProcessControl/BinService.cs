using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.ProcessControl;

public class BinService : IBinService
{
    private readonly MesDbContext _context;
    private readonly ILogger<BinService> _logger;

    public BinService(MesDbContext context, ILogger<BinService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BinDefinitionResponse> CreateBinDefinitionAsync(CreateBinDefinitionRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var binId = $"BIN-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var binDefinition = new BinDefinition
        {
            BinId = binId,
            BinCode = request.BinCode,
            BinName = request.BinName,
            BinCategory = request.BinCategory,
            BinNo = request.BinNo,
            Description = request.Description,
            Color = request.Color,
            IsDefault = request.IsDefault,
            IsActive = request.IsActive,
            ProductId = request.ProductId,
            ProcessCode = request.ProcessCode,
            TestType = request.TestType,
            SortOrder = request.SortOrder,
            CreatedBy = operatorId,
            CreatedAt = now,
            UpdatedAt = now,
            Deleted = false
        };

        _context.BinDefinitions.Add(binDefinition);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created BinDefinition {BinId} with code {BinCode}", binId, request.BinCode);

        return MapToResponse(binDefinition);
    }

    public async Task<BinDefinitionResponse> GetBinDefinitionAsync(string binId)
    {
        var binDefinition = await _context.BinDefinitions
            .FirstOrDefaultAsync(b => b.BinId == binId && !b.Deleted);

        if (binDefinition == null)
            throw new KeyNotFoundException($"Bin definition {binId} not found");

        return MapToResponse(binDefinition);
    }

    public async Task<PagedResult<BinDefinitionResponse>> QueryBinDefinitionsAsync(BinQuery query)
    {
        var iqQuery = _context.BinDefinitions.Where(b => !b.Deleted).AsQueryable();

        if (!string.IsNullOrEmpty(query.BinCategory))
            iqQuery = iqQuery.Where(b => b.BinCategory == query.BinCategory);
        if (!string.IsNullOrEmpty(query.ProcessCode))
            iqQuery = iqQuery.Where(b => b.ProcessCode == query.ProcessCode);
        if (!string.IsNullOrEmpty(query.TestType))
            iqQuery = iqQuery.Where(b => b.TestType == query.TestType);
        if (query.IsActive.HasValue)
            iqQuery = iqQuery.Where(b => b.IsActive == query.IsActive.Value);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderBy(b => b.SortOrder)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(b => MapToResponse(b))
            .ToListAsync();

        return new PagedResult<BinDefinitionResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<List<BinStatisticsResponse>> GetBinSummaryAsync(BinSummaryQuery query)
    {
        var iqQuery = _context.BinStatistics.AsQueryable();

        if (!string.IsNullOrEmpty(query.LotId))
            iqQuery = iqQuery.Where(s => s.LotId == query.LotId);
        if (!string.IsNullOrEmpty(query.StepCode))
            iqQuery = iqQuery.Where(s => s.StepCode == query.StepCode);
        if (query.StartDate.HasValue)
            iqQuery = iqQuery.Where(s => s.StatPeriod >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            iqQuery = iqQuery.Where(s => s.StatPeriod <= query.EndDate.Value);

        return await iqQuery
            .OrderByDescending(s => s.StatPeriod)
            .Select(s => new BinStatisticsResponse
            {
                StatId = s.StatId,
                LotId = s.LotId,
                StepCode = s.StepCode,
                StepSeq = s.StepSeq,
                BinCode = s.BinCode,
                BinName = s.BinName,
                BinCategory = s.BinCategory,
                TotalQty = s.TotalQty,
                Percentage = s.Percentage,
                InputQty = s.InputQty,
                CumulativeYield = s.CumulativeYield,
                StatPeriod = s.StatPeriod
            })
            .ToListAsync();
    }

    private static BinDefinitionResponse MapToResponse(BinDefinition entity) => new()
    {
        BinId = entity.BinId,
        BinCode = entity.BinCode,
        BinName = entity.BinName,
        BinCategory = entity.BinCategory,
        BinNo = entity.BinNo,
        Description = entity.Description,
        Color = entity.Color,
        IsDefault = entity.IsDefault,
        IsActive = entity.IsActive,
        ProductId = entity.ProductId,
        ProcessCode = entity.ProcessCode,
        TestType = entity.TestType,
        SortOrder = entity.SortOrder,
        CreatedAt = entity.CreatedAt
    };
}
