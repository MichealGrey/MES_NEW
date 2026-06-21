using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.ProcessControl;

public class QualificationService : IQualificationService
{
    private readonly MesDbContext _context;
    private readonly ILogger<QualificationService> _logger;

    public QualificationService(MesDbContext context, ILogger<QualificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OperatorQualificationResponse> CreateQualificationAsync(CreateOperatorQualificationRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var qualificationId = $"QUAL-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        // Determine initial status based on expiry date
        var status = "Active";
        if (request.ExpiryDate.HasValue && request.ExpiryDate.Value < now)
        {
            status = "Expired";
        }

        var qualification = new OperatorQualification
        {
            QualificationId = qualificationId,
            OperatorId = request.OperatorId,
            OperatorName = request.OperatorName,
            Department = request.Department,
            Position = request.Position,
            ProcessCode = request.ProcessCode,
            ProcessName = request.ProcessName,
            QualificationLevel = request.QualificationLevel,
            CertificationType = request.CertificationType,
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate,
            Status = status,
            IssuedBy = request.IssuedBy,
            CertificationNo = request.CertificationNo,
            Remark = request.Remark,
            CreatedBy = operatorId,
            CreatedAt = now,
            UpdatedAt = now,
            Deleted = false
        };

        _context.OperatorQualifications.Add(qualification);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created qualification {QualificationId} for operator {OperatorId} process {ProcessCode}",
            qualificationId, request.OperatorId, request.ProcessCode);

        return MapToResponse(qualification);
    }

    public async Task<OperatorQualificationResponse> GetQualificationAsync(string qualificationId)
    {
        var qualification = await _context.OperatorQualifications
            .FirstOrDefaultAsync(q => q.QualificationId == qualificationId && !q.Deleted);

        if (qualification == null)
            throw new KeyNotFoundException($"Qualification {qualificationId} not found");

        return MapToResponse(qualification);
    }

    public async Task<PagedResult<OperatorQualificationResponse>> QueryQualificationsAsync(OperatorQualificationQuery query)
    {
        var iqQuery = _context.OperatorQualifications.Where(q => !q.Deleted).AsQueryable();

        if (!string.IsNullOrEmpty(query.OperatorId))
            iqQuery = iqQuery.Where(q => q.OperatorId == query.OperatorId);
        if (!string.IsNullOrEmpty(query.ProcessCode))
            iqQuery = iqQuery.Where(q => q.ProcessCode == query.ProcessCode);
        if (!string.IsNullOrEmpty(query.Status))
            iqQuery = iqQuery.Where(q => q.Status == query.Status);
        if (!string.IsNullOrEmpty(query.QualificationLevel))
            iqQuery = iqQuery.Where(q => q.QualificationLevel == query.QualificationLevel);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(q => q.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(q => MapToResponse(q))
            .ToListAsync();

        return new PagedResult<OperatorQualificationResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<QualificationCheckLogResponse> CheckQualificationAsync(OperatorQualificationCheckRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;

        // Find active, non-expired qualification for the operator and process
        var qualification = await _context.OperatorQualifications
            .FirstOrDefaultAsync(q =>
                q.OperatorId == request.OperatorId &&
                q.ProcessCode == request.ProcessCode &&
                q.Status == "Active" &&
                !q.Deleted &&
                (!q.ExpiryDate.HasValue || q.ExpiryDate.Value >= now));

        var isQualified = qualification != null;
        var failReason = string.Empty;
        var action = string.Empty;
        var qualificationLevel = string.Empty;

        if (qualification == null)
        {
            // Check if qualification exists but expired
            var expiredQualification = await _context.OperatorQualifications
                .FirstOrDefaultAsync(q =>
                    q.OperatorId == request.OperatorId &&
                    q.ProcessCode == request.ProcessCode &&
                    !q.Deleted &&
                    q.ExpiryDate.HasValue &&
                    q.ExpiryDate.Value < now);

            if (expiredQualification != null)
            {
                failReason = $"资质已过期 (过期日期: {expiredQualification.ExpiryDate.Value:yyyy-MM-dd})";
                action = "Reject";
                qualificationLevel = expiredQualification.QualificationLevel;
                _logger.LogWarning("Operator {OperatorId} qualification expired for process {ProcessCode}",
                    request.OperatorId, request.ProcessCode);
            }
            else
            {
                failReason = $"未找到操作员 {request.OperatorId} 对工序 {request.ProcessCode} 的资质认证";
                action = "Reject";
                _logger.LogWarning("No qualification found for operator {OperatorId} on process {ProcessCode}",
                    request.OperatorId, request.ProcessCode);
            }
        }
        else
        {
            qualificationLevel = qualification.QualificationLevel;
            action = "Allow";
            _logger.LogInformation("Operator {OperatorId} passed qualification check for process {ProcessCode}",
                request.OperatorId, request.ProcessCode);
        }

        var checkLog = new QualificationCheckLog
        {
            OperatorId = request.OperatorId,
            OperatorName = operatorId,
            ProcessCode = request.ProcessCode,
            ProcessName = qualification?.ProcessName ?? string.Empty,
            QualificationId = qualification?.QualificationId,
            LotId = request.LotId,
            EquipmentId = request.EquipmentId,
            StepCode = request.StepCode,
            StepSeq = request.StepSeq,
            CheckType = request.CheckType,
            CheckResult = isQualified ? "Pass" : "Fail",
            QualificationLevel = qualificationLevel,
            QualificationStatus = qualification?.Status,
            QualificationExpiryDate = qualification?.ExpiryDate,
            IsQualified = isQualified,
            FailReason = failReason,
            Action = action,
            CheckTime = now,
            CreatedAt = now
        };

        _context.QualificationCheckLogs.Add(checkLog);
        await _context.SaveChangesAsync();

        return MapCheckLogToResponse(checkLog);
    }

    public async Task<PagedResult<QualificationCheckLogResponse>> GetCheckLogsAsync(string operatorIdFilter, int pageIndex, int pageSize)
    {
        var iqQuery = _context.QualificationCheckLogs.AsQueryable();

        if (!string.IsNullOrEmpty(operatorIdFilter))
            iqQuery = iqQuery.Where(l => l.OperatorId == operatorIdFilter);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(l => l.CheckTime)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(l => MapCheckLogToResponse(l))
            .ToListAsync();

        return new PagedResult<QualificationCheckLogResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    // ==================== 映射方法 ====================

    private static OperatorQualificationResponse MapToResponse(OperatorQualification entity) => new()
    {
        QualificationId = entity.QualificationId,
        OperatorId = entity.OperatorId,
        OperatorName = entity.OperatorName,
        Department = entity.Department,
        Position = entity.Position,
        ProcessCode = entity.ProcessCode,
        ProcessName = entity.ProcessName,
        QualificationLevel = entity.QualificationLevel,
        CertificationType = entity.CertificationType,
        IssueDate = entity.IssueDate,
        ExpiryDate = entity.ExpiryDate,
        Status = entity.Status,
        CertificationNo = entity.CertificationNo,
        CreatedAt = entity.CreatedAt
    };

    private static QualificationCheckLogResponse MapCheckLogToResponse(QualificationCheckLog entity) => new()
    {
        LogId = entity.LogId,
        OperatorId = entity.OperatorId,
        OperatorName = entity.OperatorName,
        ProcessCode = entity.ProcessCode,
        ProcessName = entity.ProcessName,
        LotId = entity.LotId,
        EquipmentId = entity.EquipmentId,
        StepCode = entity.StepCode,
        CheckType = entity.CheckType,
        CheckResult = entity.CheckResult,
        QualificationLevel = entity.QualificationLevel,
        IsQualified = entity.IsQualified,
        FailReason = entity.FailReason,
        Action = entity.Action,
        CheckTime = entity.CheckTime
    };
}
