using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Analytics;

public class ReportService : IReportService
{
    private readonly MesDbContext _context;
    private readonly ILogger<ReportService> _logger;

    public ReportService(MesDbContext context, ILogger<ReportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ReportScheduleResponse> CreateScheduleAsync(CreateReportScheduleRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var scheduleId = $"RPT-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var schedule = new ReportSchedule
        {
            ScheduleId = scheduleId,
            ReportName = request.ReportName,
            ReportType = request.ReportType,
            ScheduleCron = request.ScheduleCron,
            Recipients = request.Recipients,
            Format = request.Format,
            Enabled = request.Enabled,
            CreatedBy = operatorId,
            CreatedAt = now
        };

        _context.ReportSchedules.Add(schedule);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created report schedule {ScheduleId}: {ReportName}", scheduleId, request.ReportName);

        return MapToResponse(schedule);
    }

    public async Task<PagedResult<ReportScheduleResponse>> QuerySchedulesAsync(ReportScheduleQuery query)
    {
        var iqQuery = _context.ReportSchedules.AsQueryable();

        if (!string.IsNullOrEmpty(query.ReportType))
            iqQuery = iqQuery.Where(x => x.ReportType == query.ReportType);
        if (query.Enabled.HasValue)
            iqQuery = iqQuery.Where(x => x.Enabled == query.Enabled.Value);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => MapToResponse(x))
            .ToListAsync();

        return new PagedResult<ReportScheduleResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<bool> GenerateReportAsync(string scheduleId)
    {
        var schedule = await _context.ReportSchedules
            .FirstOrDefaultAsync(x => x.ScheduleId == scheduleId);

        if (schedule == null)
            throw new KeyNotFoundException($"Report schedule '{scheduleId}' not found");

        schedule.LastRunDate = DateTime.UtcNow;
        schedule.NextRunDate = DateTime.UtcNow.AddDays(1); // Simplified next run calculation

        await _context.SaveChangesAsync();

        _logger.LogInformation("Generated report for schedule {ScheduleId}", scheduleId);

        return true;
    }

    public async Task<byte[]> DownloadReportAsync(string scheduleId)
    {
        var schedule = await _context.ReportSchedules
            .FirstOrDefaultAsync(x => x.ScheduleId == scheduleId);

        if (schedule == null)
            throw new KeyNotFoundException($"Report schedule '{scheduleId}' not found");

        // Generate a placeholder report content
        var reportContent = $"Report: {schedule.ReportName}\nType: {schedule.ReportType}\nGenerated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
        return System.Text.Encoding.UTF8.GetBytes(reportContent);
    }

    private static ReportScheduleResponse MapToResponse(ReportSchedule entity) => new()
    {
        ScheduleId = entity.ScheduleId,
        ReportName = entity.ReportName,
        ReportType = entity.ReportType,
        ScheduleCron = entity.ScheduleCron,
        Recipients = entity.Recipients,
        Format = entity.Format,
        Enabled = entity.Enabled,
        LastRunDate = entity.LastRunDate,
        NextRunDate = entity.NextRunDate,
        CreatedBy = entity.CreatedBy,
        CreatedAt = entity.CreatedAt
    };
}
