using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Analytics;

public class KpiService : IKpiService
{
    private readonly MesDbContext _context;
    private readonly ILogger<KpiService> _logger;

    public KpiService(MesDbContext context, ILogger<KpiService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<KpiDashboardResponse> GetDashboardAsync(string? periodType = null)
    {
        var query = _context.KpiDashboardSnapshots.AsQueryable();
        if (!string.IsNullOrEmpty(periodType))
            query = query.Where(x => x.PeriodType == periodType);

        var latest = await query
            .OrderByDescending(x => x.SnapshotDate)
            .ToListAsync();

        var metrics = latest.GroupBy(x => x.MetricCode)
            .Select(g => g.First())
            .Select(MapToResponse)
            .ToList();

        var overallStatus = metrics.Any(x => x.Status == "Critical") ? "Critical"
            : metrics.Any(x => x.Status == "Warning") ? "Warning"
            : "Normal";

        return new KpiDashboardResponse
        {
            Metrics = metrics,
            LastUpdated = metrics.Max(x => x.CreatedAt),
            OverallStatus = overallStatus
        };
    }

    public async Task<KpiMetricResponse> GetMetricDetailAsync(string metricCode, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.KpiDashboardSnapshots
            .Where(x => x.MetricCode == metricCode);

        if (startDate.HasValue)
            query = query.Where(x => x.SnapshotDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(x => x.SnapshotDate <= endDate.Value);

        var latest = await query
            .OrderByDescending(x => x.SnapshotDate)
            .FirstOrDefaultAsync();

        if (latest == null)
            throw new KeyNotFoundException($"KPI metric '{metricCode}' not found");

        return MapToResponse(latest);
    }

    public async Task<KpiMetricResponse> CaptureMetricAsync(KpiCaptureRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var snapshotId = $"KPI-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var status = request.TargetValue.HasValue
            ? (request.MetricValue >= request.TargetValue.Value ? "Normal" : "Warning")
            : "Normal";

        var snapshot = new KpiDashboardSnapshot
        {
            SnapshotId = snapshotId,
            SnapshotDate = now,
            MetricCode = request.MetricCode,
            MetricName = request.MetricName,
            MetricValue = request.MetricValue,
            TargetValue = request.TargetValue,
            Unit = request.Unit,
            Status = status,
            Trend = request.Trend,
            PeriodType = request.PeriodType,
            DetailData = request.DetailData,
            CreatedAt = now
        };

        _context.KpiDashboardSnapshots.Add(snapshot);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Captured KPI metric {MetricCode} = {Value}", request.MetricCode, request.MetricValue);

        return MapToResponse(snapshot);
    }

    private static KpiMetricResponse MapToResponse(KpiDashboardSnapshot entity) => new()
    {
        SnapshotId = entity.SnapshotId,
        SnapshotDate = entity.SnapshotDate,
        MetricCode = entity.MetricCode,
        MetricName = entity.MetricName,
        MetricValue = entity.MetricValue,
        TargetValue = entity.TargetValue,
        Unit = entity.Unit,
        Status = entity.Status,
        Trend = entity.Trend,
        PeriodType = entity.PeriodType,
        DetailData = entity.DetailData,
        CreatedAt = entity.CreatedAt
    };
}
