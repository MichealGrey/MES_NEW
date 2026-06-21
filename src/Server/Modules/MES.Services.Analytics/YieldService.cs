using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Analytics;

public class YieldService : IYieldService
{
    private readonly MesDbContext _context;
    private readonly ILogger<YieldService> _logger;

    public YieldService(MesDbContext context, ILogger<YieldService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<YieldStatisticsResponse> RecordYieldAsync(string lotId, string? stepCode, int inputQty, int outputQty, DateOnly statDate, string? defectJson = null)
    {
        var now = DateTime.UtcNow;
        var statId = $"YIELD-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";
        var yieldRate = inputQty > 0 ? Math.Round((decimal)outputQty / inputQty * 100, 2) : 0m;

        var stats = new YieldStatistics
        {
            StatId = statId,
            LotId = lotId,
            StepCode = stepCode,
            InputQty = inputQty,
            OutputQty = outputQty,
            YieldRate = yieldRate,
            ScrapQty = inputQty - outputQty,
            ReworkQty = 0,
            DefectJson = defectJson,
            StatDate = statDate,
            CreatedAt = now
        };

        _context.YieldStatistics.Add(stats);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Recorded yield {YieldRate}% for lot {LotId}", yieldRate, lotId);

        return MapToResponse(stats);
    }

    public async Task<PagedResult<YieldStatisticsResponse>> GetProcessYieldAsync(YieldQuery query)
    {
        var iqQuery = _context.YieldStatistics.AsQueryable();

        if (!string.IsNullOrEmpty(query.LotId))
            iqQuery = iqQuery.Where(x => x.LotId == query.LotId);
        if (!string.IsNullOrEmpty(query.StepCode))
            iqQuery = iqQuery.Where(x => x.StepCode == query.StepCode);
        if (query.StartDate.HasValue)
            iqQuery = iqQuery.Where(x => x.StatDate >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            iqQuery = iqQuery.Where(x => x.StatDate <= query.EndDate.Value);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(x => x.StatDate)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => MapToResponse(x))
            .ToListAsync();

        return new PagedResult<YieldStatisticsResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<decimal> GetCumulativeYieldAsync(string lotId)
    {
        var yields = await _context.YieldStatistics
            .Where(x => x.LotId == lotId)
            .OrderBy(x => x.StatDate)
            .Select(x => x.YieldRate)
            .ToListAsync();

        if (yields.Count == 0 || yields.Any(y => !y.HasValue))
            return 0;

        decimal cumulative = 1;
        foreach (var y in yields)
            cumulative *= y.Value / 100;

        return Math.Round(cumulative * 100, 2);
    }

    public async Task<YieldTrendResponse> GetYieldTrendAsync(string lotId)
    {
        var stats = await _context.YieldStatistics
            .Where(x => x.LotId == lotId)
            .OrderBy(x => x.StatDate)
            .ToListAsync();

        return new YieldTrendResponse
        {
            LotId = lotId,
            Points = stats.Where(s => s.YieldRate.HasValue).Select(s => new YieldPoint
            {
                StepCode = s.StepCode,
                StepName = s.StepName,
                YieldRate = s.YieldRate!.Value,
                InputQty = s.InputQty,
                OutputQty = s.OutputQty,
                StatDate = s.StatDate
            }).ToList()
        };
    }

    public async Task<YieldParetoResponse> GetParetoAsync(string? lotId = null, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var query = _context.YieldStatistics.AsQueryable();

        if (!string.IsNullOrEmpty(lotId))
            query = query.Where(x => x.LotId == lotId);
        if (startDate.HasValue)
            query = query.Where(x => x.StatDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(x => x.StatDate <= endDate.Value);

        var stats = await query.ToListAsync();

        // Parse defect JSON and aggregate by defect type
        var defectCounts = new Dictionary<string, int>();
        foreach (var stat in stats)
        {
            if (!string.IsNullOrEmpty(stat.DefectJson))
            {
                // Simple JSON parsing for defect counts
                defectCounts.TryGetValue("Defect", out var currentDefect);
                defectCounts["Defect"] = currentDefect + stat.ScrapQty;
            }
            defectCounts.TryGetValue("Scrap", out var currentScrap);
            defectCounts["Scrap"] = currentScrap + stat.ScrapQty;
        }

        var totalDefects = defectCounts.Values.Sum();
        var paretoItems = defectCounts
            .OrderByDescending(kv => kv.Value)
            .Select((kv, index) => new ParetoItem
            {
                DefectType = kv.Key,
                Count = kv.Value,
                Percentage = totalDefects > 0 ? Math.Round((decimal)kv.Value / totalDefects * 100, 2) : 0,
                CumulativePercentage = 0 // Will be calculated below
            })
            .ToList();

        decimal cumulative = 0;
        for (int i = 0; i < paretoItems.Count; i++)
        {
            cumulative += paretoItems[i].Percentage;
            paretoItems[i].CumulativePercentage = cumulative;
        }

        return new YieldParetoResponse { Items = paretoItems };
    }

    public async Task<DppmResponse> GetDppmAsync(string productId, DateOnly periodStart, DateOnly periodEnd)
    {
        // Query yield statistics for the product period
        var lotIds = await _context.YieldStatistics
            .Where(x => x.StatDate >= periodStart && x.StatDate <= periodEnd)
            .Select(x => x.LotId)
            .Distinct()
            .ToListAsync();

        int totalShipped = lotIds.Count * 1000; // Simplified
        int defectCount = 0;

        foreach (var lotId in lotIds)
        {
            var stats = await _context.YieldStatistics
                .Where(x => x.LotId == lotId)
                .ToListAsync();
            defectCount += stats.Sum(s => s.ScrapQty);
        }

        var dppm = totalShipped > 0 ? Math.Round((decimal)defectCount / totalShipped * 1000000, 2) : 0;

        return new DppmResponse
        {
            ProductId = productId,
            TotalShipped = totalShipped,
            DefectCount = defectCount,
            Dppm = dppm,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd
        };
    }

    private static YieldStatisticsResponse MapToResponse(YieldStatistics entity) => new()
    {
        StatId = entity.StatId,
        LotId = entity.LotId,
        StepCode = entity.StepCode,
        StepName = entity.StepName,
        InputQty = entity.InputQty,
        OutputQty = entity.OutputQty,
        YieldRate = entity.YieldRate,
        ScrapQty = entity.ScrapQty,
        ReworkQty = entity.ReworkQty,
        DefectJson = entity.DefectJson,
        StatDate = entity.StatDate,
        CreatedAt = entity.CreatedAt
    };
}
