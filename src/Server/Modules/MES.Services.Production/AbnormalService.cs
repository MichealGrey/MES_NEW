using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Production;

public class AbnormalService : IAbnormalService
{
    private readonly MesDbContext _context;

    public AbnormalService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<AbnormalRecordResponse> ReportAsync(ReportAbnormalRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AbnormalType))
            throw new ApplicationException("异常类型不能为空");
        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ApplicationException("异常描述不能为空");

        var abnormalId = GenerateId("ABN");
        var now = DateTime.UtcNow;

        var record = new AbnormalRecord
        {
            AbnormalId = abnormalId,
            LotId = request.LotId,
            OrderId = request.WorkOrderId,
            StepCode = request.StepCode,
            EquipmentId = request.EquipmentId,
            AbnormalType = request.AbnormalType,
            AbnormalCategory = request.AbnormalType,
            Severity = request.Severity,
            Description = request.Description,
            Status = "Open",
            ReporterId = request.ReportedBy,
            ReporterName = request.ReportedByName,
            ReportTime = now,
            Remark = request.Remark,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<AbnormalRecord>().AddAsync(record);
        await _context.SaveChangesAsync();

        return MapToResponse(record);
    }

    public async Task<PagedResult<AbnormalRecordResponse>> GetRecordsAsync(AbnormalQuery query)
    {
        var iq = _context.Set<AbnormalRecord>().AsQueryable();

        if (!string.IsNullOrEmpty(query.AbnormalType))
            iq = iq.Where(r => r.AbnormalType == query.AbnormalType);
        if (!string.IsNullOrEmpty(query.Severity))
            iq = iq.Where(r => r.Severity == query.Severity);
        if (!string.IsNullOrEmpty(query.Status))
            iq = iq.Where(r => r.Status == query.Status);
        if (!string.IsNullOrEmpty(query.WorkOrderId))
            iq = iq.Where(r => r.OrderId == query.WorkOrderId);
        if (!string.IsNullOrEmpty(query.EquipmentId))
            iq = iq.Where(r => r.EquipmentId == query.EquipmentId);
        if (query.DateFrom.HasValue)
            iq = iq.Where(r => r.ReportTime >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(r => r.ReportTime <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var items = await iq
            .OrderByDescending(r => r.ReportTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new AbnormalRecordResponse
            {
                AbnormalId = r.AbnormalId,
                AbnormalType = r.AbnormalType,
                Severity = r.Severity,
                Description = r.Description,
                Status = r.Status,
                IsLineStopped = false,
                ReportedAt = r.ReportTime,
                HandledAt = r.CorrectiveAction != null ? r.UpdatedAt : null,
                ClosedAt = r.ClosedTime
            })
            .ToListAsync();

        return new PagedResult<AbnormalRecordResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<AbnormalRecordResponse> GetDetailAsync(string abnormalId)
    {
        var record = await _context.Set<AbnormalRecord>().FindAsync(abnormalId);
        if (record == null)
            throw new ApplicationException($"异常记录 {abnormalId} 不存在");

        return MapToResponse(record);
    }

    public async Task<LineStopResponse> StopLineAsync(LineStopRequest request)
    {
        var stopId = GenerateId("LS");
        var now = DateTime.UtcNow;

        var stopRecord = new LineStopRecord
        {
            StopId = stopId,
            LotId = request.AbnormalId.StartsWith("ABN") ? null : request.StopTargetId,
            LineId = request.StopScope == "Line" ? request.StopTargetId : null,
            EquipmentId = request.StopScope == "Equipment" ? request.StopTargetId : null,
            StopType = request.StopScope,
            StopReason = request.StopReason,
            Severity = "Critical",
            StopTime = now,
            Status = "Stopped",
            ReportedBy = request.IssuedBy,
            ReportedByName = request.IssuedBy,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<LineStopRecord>().AddAsync(stopRecord);

        // If lot-level stop, update prod_lot status
        if (request.StopScope == "Lot" && !string.IsNullOrEmpty(request.StopTargetId))
        {
            var lot = await _context.ProdLots.FindAsync(request.StopTargetId);
            if (lot != null)
            {
                lot.Status = "Hold";
                lot.HoldReason = request.StopReason;
                lot.HoldTime = now;
                lot.HoldOperator = request.IssuedBy;
                lot.UpdatedAt = now;
            }
        }

        await _context.SaveChangesAsync();

        return new LineStopResponse
        {
            StopId = stopId,
            AbnormalId = request.AbnormalId,
            StopScope = request.StopScope,
            StopTargetId = request.StopTargetId,
            Status = "Stopped",
            IssuedAt = now
        };
    }

    public async Task<LineStopResponse> ResumeLineAsync(string stopId, string resumeBy, string comment)
    {
        var stop = await _context.Set<LineStopRecord>().FindAsync(stopId);
        if (stop == null)
            throw new ApplicationException($"停线记录 {stopId} 不存在");

        var now = DateTime.UtcNow;
        stop.ResumeTime = now;
        stop.Status = "Resumed";
        stop.Action = comment;
        stop.ClosedBy = resumeBy;
        stop.UpdatedAt = now;
        stop.StopDurationMinutes = (int)(now - stop.StopTime).TotalMinutes;

        await _context.SaveChangesAsync();

        return new LineStopResponse
        {
            StopId = stop.StopId,
            AbnormalId = string.Empty,
            StopScope = stop.StopType,
            StopTargetId = stop.LineId ?? stop.EquipmentId ?? string.Empty,
            Status = "Resumed",
            IssuedAt = stop.StopTime,
            ResumeAt = stop.ResumeTime
        };
    }

    public async Task<bool> HandleAsync(HandleAbnormalRequest request)
    {
        var record = await _context.Set<AbnormalRecord>().FindAsync(request.AbnormalId);
        if (record == null)
            throw new ApplicationException($"异常记录 {request.AbnormalId} 不存在");

        var now = DateTime.UtcNow;
        record.RootCause = request.RootCause;
        record.CorrectiveAction = request.CorrectiveAction;
        record.PreventiveAction = request.PreventiveAction;
        record.ResponsiblePerson = request.HandledBy;
        record.Status = "Handled";
        record.UpdatedAt = now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> VerifyAsync(VerifyAbnormalRequest request)
    {
        var record = await _context.Set<AbnormalRecord>().FindAsync(request.AbnormalId);
        if (record == null)
            throw new ApplicationException($"异常记录 {request.AbnormalId} 不存在");

        var now = DateTime.UtcNow;

        if (request.VerifyResult == "Pass")
        {
            record.Status = "Closed";
            record.ClosedBy = request.VerifiedBy;
            record.ClosedTime = now;
        }
        else
        {
            record.Status = "Reopen";
        }

        record.UpdatedAt = now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AbnormalStatisticsResponse> GetStatisticsAsync(AbnormalQuery query)
    {
        var iq = _context.Set<AbnormalRecord>().AsQueryable();

        if (!string.IsNullOrEmpty(query.AbnormalType))
            iq = iq.Where(r => r.AbnormalType == query.AbnormalType);
        if (!string.IsNullOrEmpty(query.Severity))
            iq = iq.Where(r => r.Severity == query.Severity);
        if (query.DateFrom.HasValue)
            iq = iq.Where(r => r.ReportTime >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(r => r.ReportTime <= query.DateTo.Value);

        var records = await iq.ToListAsync();

        var totalCount = records.Count;
        var byType = records.GroupBy(r => r.AbnormalType).ToDictionary(g => g.Key, g => g.Count());
        var bySeverity = records.GroupBy(r => r.Severity).ToDictionary(g => g.Key, g => g.Count());
        var openCount = records.Count(r => r.Status != "Closed");

        // Calculate average handle time
        var handledRecords = records.Where(r => r.CorrectiveAction != null).ToList();
        var avgHandleTime = handledRecords.Count > 0
            ? Math.Round((decimal)handledRecords.Average(r => (r.UpdatedAt - r.ReportTime).TotalMinutes), 2)
            : 0m;

        return new AbnormalStatisticsResponse
        {
            TotalCount = totalCount,
            ByType = byType,
            BySeverity = bySeverity,
            AvgHandleTime = avgHandleTime,
            OpenCount = openCount
        };
    }

    private static AbnormalRecordResponse MapToResponse(AbnormalRecord record)
    {
        return new AbnormalRecordResponse
        {
            AbnormalId = record.AbnormalId,
            AbnormalType = record.AbnormalType,
            Severity = record.Severity,
            Description = record.Description,
            Status = record.Status,
            IsLineStopped = false,
            ReportedAt = record.ReportTime,
            HandledAt = record.CorrectiveAction != null ? record.UpdatedAt : null,
            ClosedAt = record.ClosedTime
        };
    }

    private static string GenerateId(string prefix)
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 9999).ToString("D4");
        return $"{prefix}-{now:yyyyMMdd}-{seq}";
    }
}
