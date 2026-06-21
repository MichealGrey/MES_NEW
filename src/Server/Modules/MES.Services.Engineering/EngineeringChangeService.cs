using MES.Contracts.Common;
using MES.Contracts.Engineering;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Engineering;

public class EngineeringChangeService : IEngineeringChangeService
{
    private readonly MesDbContext _context;

    public EngineeringChangeService(MesDbContext context)
    {
        _context = context;
    }

    // ============================================================================
    // Main CRUD
    // ============================================================================

    public async Task<PagedResult<EcnRequestDto>> GetPagedAsync(int pageIndex, int pageSize,
        string? status, string? ecnType, string? changeCategory, string? urgency,
        string? riskLevel, string? requestedBy, string? search)
    {
        var query = _context.EcnRequests.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);

        if (!string.IsNullOrEmpty(ecnType))
            query = query.Where(x => x.EcnType == ecnType);

        if (!string.IsNullOrEmpty(changeCategory))
            query = query.Where(x => x.ChangeCategory == changeCategory);

        if (!string.IsNullOrEmpty(urgency))
            query = query.Where(x => x.Urgency == urgency);

        if (!string.IsNullOrEmpty(riskLevel))
            query = query.Where(x => x.RiskLevel == riskLevel);

        if (!string.IsNullOrEmpty(requestedBy))
            query = query.Where(x => x.RequestedBy == requestedBy);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(x =>
                x.EcnNo.Contains(search) ||
                x.EcnTitle != null && x.EcnTitle.Contains(search) ||
                x.Reason != null && x.Reason.Contains(search));

        query = query.OrderByDescending(x => x.RequestedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = items.Select(MapToDto).ToList();
        return new PagedResult<EcnRequestDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<EcnRequestDto?> GetByIdAsync(string ecnId)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return null;

        var dto = MapToDto(ecn);

        // Load sub-table data
        dto.Items = await _context.EcnItems
            .Where(x => x.EcnId == ecnId)
            .OrderBy(x => x.SortOrder)
            .Select(x => new EcnItemDto
            {
                ItemId = x.ItemId,
                EcnId = x.EcnId,
                ItemType = x.ItemType,
                ItemCode = x.ItemCode,
                ItemName = x.ItemName,
                OldValue = x.OldValue,
                NewValue = x.NewValue,
                ChangeReason = x.ChangeReason,
                Remark = x.Remark,
                SortOrder = x.SortOrder,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy
            })
            .ToListAsync();

        dto.Impacts = await _context.EcnImpactItems
            .Where(x => x.EcnId == ecnId)
            .Select(x => new EcnImpactItemDto
            {
                ImpactId = x.ImpactId,
                EcnId = x.EcnId,
                ImpactType = x.ImpactType,
                Severity = x.Severity,
                Description = x.Description,
                ImpactAnalysis = x.ImpactAnalysis,
                Action = x.Action,
                Responsible = x.Responsible,
                DueDate = x.DueDate,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy
            })
            .ToListAsync();

        dto.Approvers = await _context.EcnApprovers
            .Where(x => x.EcnId == ecnId)
            .OrderBy(x => x.ApprovalOrder)
            .Select(x => new EcnApproverDto
            {
                ApproverId = x.ApproverId,
                EcnId = x.EcnId,
                ApproverName = x.ApproverName,
                Role = x.Role,
                ApprovalOrder = x.ApprovalOrder,
                Status = x.Status,
                Result = x.Result,
                Comments = x.Comments,
                ApprovedAt = x.ApprovedAt,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        dto.NotifyDepts = await _context.EcnNotifyDepts
            .Where(x => x.EcnId == ecnId)
            .Select(x => new EcnNotifyDeptDto
            {
                NotifyId = x.NotifyId,
                EcnId = x.EcnId,
                DeptId = x.DeptId,
                DeptName = x.DeptName,
                Confirmed = x.Confirmed,
                NotifiedAt = x.NotifiedAt,
                ConfirmedBy = x.ConfirmedBy,
                ConfirmedAt = x.ConfirmedAt,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        dto.Implements = await _context.EcnImplements
            .Where(x => x.EcnId == ecnId)
            .Select(x => new EcnImplementDto
            {
                ImplementId = x.ImplementId,
                EcnId = x.EcnId,
                TaskName = x.TaskName,
                Description = x.Description,
                Responsible = x.Responsible,
                PlanDate = x.PlanDate,
                ActualDate = x.ActualDate,
                Status = x.Status,
                Result = x.Result,
                Remark = x.Remark,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                UpdatedAt = x.UpdatedAt,
                UpdatedBy = x.UpdatedBy
            })
            .ToListAsync();

        return dto;
    }

    public async Task<EcnRequestDto> CreateAsync(CreateEcnRequest request, string createdBy)
    {
        var now = DateTime.UtcNow;
        var ecnNo = await GenerateEcnNoAsync(now);

        var ecn = new EcnRequest
        {
            EcnId = IdHelper.GenerateId(),
            EcnNo = ecnNo,
            EcnTitle = request.EcnTitle,
            EcnType = request.EcnType,
            ChangeCategory = request.ChangeCategory,
            Reason = request.Reason,
            ChangeDescription = request.ChangeDescription,
            ChangeContent = request.ChangeContent,
            OldValue = request.OldValue,
            NewValue = request.NewValue,
            Status = "Draft",
            AffectedRoutes = request.AffectedRoutes,
            AffectedProducts = request.AffectedProducts,
            ImpactAssessment = request.ImpactAssessment,
            Urgency = request.Urgency,
            RiskLevel = request.RiskLevel,
            PlannedDate = request.PlannedDate,
            IsUrgent = request.IsUrgent,
            CostEstimate = request.CostEstimate,
            Remark = request.Remark,
            RequestedBy = createdBy,
            RequestedAt = now,
            CreatedAt = now,
            CreatedBy = createdBy,
            DaysElapsed = 0
        };

        await _context.EcnRequests.AddAsync(ecn);
        await _context.SaveChangesAsync();

        return MapToDto(ecn);
    }

    public async Task<bool> UpdateAsync(UpdateEcnRequest request, string updatedBy)
    {
        var ecn = await _context.EcnRequests.FindAsync(request.EcnId);
        if (ecn == null) return false;

        // Only allow updates in Draft status
        if (ecn.Status != "Draft")
            throw new InvalidOperationException($"Cannot update ECN in {ecn.Status} status. Only Draft ECNs can be modified.");

        ecn.EcnTitle = request.EcnTitle ?? ecn.EcnTitle;
        ecn.Reason = request.Reason ?? ecn.Reason;
        ecn.ChangeDescription = request.ChangeDescription ?? ecn.ChangeDescription;
        ecn.ImpactAssessment = request.ImpactAssessment ?? ecn.ImpactAssessment;
        ecn.PlannedDate = request.PlannedDate ?? ecn.PlannedDate;
        ecn.Remark = request.Remark ?? ecn.Remark;
        ecn.UpdatedAt = DateTime.UtcNow;
        ecn.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string ecnId)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return false;

        if (ecn.Status != "Draft")
            throw new InvalidOperationException($"Cannot delete ECN in {ecn.Status} status. Only Draft ECNs can be deleted.");

        // Delete sub-table records
        var items = await _context.EcnItems.Where(x => x.EcnId == ecnId).ToListAsync();
        if (items.Count > 0) _context.EcnItems.RemoveRange(items);

        var impacts = await _context.EcnImpactItems.Where(x => x.EcnId == ecnId).ToListAsync();
        if (impacts.Count > 0) _context.EcnImpactItems.RemoveRange(impacts);

        var approvers = await _context.EcnApprovers.Where(x => x.EcnId == ecnId).ToListAsync();
        if (approvers.Count > 0) _context.EcnApprovers.RemoveRange(approvers);

        var notifyDepts = await _context.EcnNotifyDepts.Where(x => x.EcnId == ecnId).ToListAsync();
        if (notifyDepts.Count > 0) _context.EcnNotifyDepts.RemoveRange(notifyDepts);

        var implements = await _context.EcnImplements.Where(x => x.EcnId == ecnId).ToListAsync();
        if (implements.Count > 0) _context.EcnImplements.RemoveRange(implements);

        _context.EcnRequests.Remove(ecn);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Workflow
    // ============================================================================

    public async Task<bool> SubmitAsync(string ecnId)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return false;

        if (ecn.Status != "Draft")
            throw new InvalidOperationException($"Cannot submit ECN in {ecn.Status} status. Only Draft ECNs can be submitted.");

        ecn.Status = "Review";
        ecn.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AdvanceAsync(string ecnId, string targetStatus, string? comments)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return false;

        var validStatuses = new[] { "Draft", "Review", "Approval", "Implement", "Verify", "Close", "Rejected" };
        if (!validStatuses.Contains(targetStatus))
            throw new ArgumentException($"Invalid status: {targetStatus}. Valid statuses: {string.Join(", ", validStatuses)}");

        var currentIndex = Array.IndexOf(validStatuses, ecn.Status);
        var targetIndex = Array.IndexOf(validStatuses, targetStatus);

        // Must advance to the next sequential status
        if (targetIndex <= currentIndex)
            throw new InvalidOperationException($"Cannot advance from {ecn.Status} to {targetStatus}. Must advance to a later status.");

        if (targetIndex > currentIndex + 1)
            throw new InvalidOperationException($"Cannot skip statuses. Current: {ecn.Status}, Target: {targetStatus}. Advance one step at a time.");

        var now = DateTime.UtcNow;

        // Apply status-specific field updates
        switch (targetStatus)
        {
            case "Review":
                ecn.Status = "Review";
                ecn.ReviewComments = comments;
                break;
            case "Approval":
                ecn.Status = "Approval";
                break;
            case "Implement":
                ecn.Status = "Implement";
                ecn.ApprovedAt = now;
                break;
            case "Verify":
                ecn.Status = "Verify";
                ecn.VerifyResult = comments;
                break;
            case "Close":
                ecn.Status = "Close";
                ecn.IsComplete = true;
                ecn.CloseDate = now;
                break;
        }

        ecn.DaysElapsed = (int)(now - ecn.RequestedAt).TotalDays;
        ecn.UpdatedAt = now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApproveAsync(string ecnId, bool approved, string? comments, string? rejectReason, string approver)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return false;

        if (ecn.Status != "Approval")
            throw new InvalidOperationException($"Cannot approve ECN in {ecn.Status} status. Only ECNs in Approval status can be approved.");

        var now = DateTime.UtcNow;

        if (approved)
        {
            ecn.Status = "Implement";
            ecn.ApprovedBy = approver;
            ecn.ApprovedAt = now;
            ecn.ReviewComments = comments;
            ecn.EffectiveDate = ecn.EffectiveDate ?? now;
        }
        else
        {
            ecn.Status = "Rejected";
            ecn.RejectReason = rejectReason ?? comments;
            ecn.ReviewComments = comments;
            ecn.ApprovedBy = approver;
            ecn.ApprovedAt = now;
        }

        ecn.DaysElapsed = (int)(now - ecn.RequestedAt).TotalDays;
        ecn.UpdatedAt = now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CloseAsync(string ecnId, string closedBy, string? comment)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return false;

        var closeableStatuses = new[] { "Verify", "Implement", "Approval" };
        if (!closeableStatuses.Contains(ecn.Status))
            throw new InvalidOperationException($"Cannot close ECN in {ecn.Status} status. ECN must be in Verify, Implement, or Approval status.");

        var now = DateTime.UtcNow;

        ecn.Status = "Close";
        ecn.IsComplete = true;
        ecn.CloseDate = now;
        ecn.VerifyResult = comment ?? ecn.VerifyResult;
        ecn.DaysElapsed = (int)(now - ecn.RequestedAt).TotalDays;
        ecn.UpdatedAt = now;
        ecn.UpdatedBy = closedBy;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> NotifyAllDeptsAsync(string ecnId)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return false;

        var notifyDepts = await _context.EcnNotifyDepts
            .Where(x => x.EcnId == ecnId && !x.Confirmed)
            .ToListAsync();

        var now = DateTime.UtcNow;

        foreach (var dept in notifyDepts)
        {
            dept.NotifiedAt = now;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Queries
    // ============================================================================

    public async Task<List<EcnRequestDto>> GetPendingApprovalAsync()
    {
        var items = await _context.EcnRequests
            .Where(x => x.Status == "Approval")
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync();

        return items.Select(MapToDto).ToList();
    }

    public async Task<List<EcnRequestDto>> GetImplementingAsync()
    {
        var items = await _context.EcnRequests
            .Where(x => x.Status == "Implement")
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync();

        return items.Select(MapToDto).ToList();
    }

    public async Task<List<EcnRequestDto>> GetOverdueAsync()
    {
        var now = DateTime.UtcNow;
        var items = await _context.EcnRequests
            .Where(x => x.PlannedDate != null && x.PlannedDate < now &&
                        x.Status != "Close" && x.Status != "Rejected")
            .OrderByDescending(x => x.PlannedDate)
            .ToListAsync();

        return items.Select(MapToDto).ToList();
    }

    public async Task<EcnStatisticsDto> GetStatisticsAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.EcnRequests.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(x => x.RequestedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(x => x.RequestedAt <= endDate.Value);

        var totalCount = await query.CountAsync();
        if (totalCount == 0)
            return new EcnStatisticsDto();

        var draftCount = await query.CountAsync(x => x.Status == "Draft");
        var reviewCount = await query.CountAsync(x => x.Status == "Review");
        var approvalCount = await query.CountAsync(x => x.Status == "Approval");
        var implementCount = await query.CountAsync(x => x.Status == "Implement");
        var verifyCount = await query.CountAsync(x => x.Status == "Verify");
        var closedCount = await query.CountAsync(x => x.Status == "Close");
        var rejectedCount = await query.CountAsync(x => x.Status == "Rejected");
        var urgentCount = await query.CountAsync(x => x.IsUrgent);

        var closeRate = totalCount > 0 ? (double)closedCount / totalCount * 100 : 0;

        // Average days to close for closed ECNs
        var avgDaysToClose = await query
            .Where(x => x.Status == "Close" && x.CloseDate != null)
            .Select(x => (int?)EF.Functions.DateDiffDay(x.RequestedAt, x.CloseDate))
            .DefaultIfEmpty(null)
            .AverageAsync();

        // Type distribution
        var typeDistribution = await query
            .Where(x => !string.IsNullOrEmpty(x.EcnType))
            .GroupBy(x => x.EcnType!)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync();

        // Severity distribution (based on RiskLevel)
        var severityDistribution = await query
            .Where(x => !string.IsNullOrEmpty(x.RiskLevel))
            .GroupBy(x => x.RiskLevel!)
            .Select(g => new { Severity = g.Key, Count = g.Count() })
            .ToListAsync();

        // Status distribution
        var statusDistribution = await query
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        return new EcnStatisticsDto
        {
            TotalCount = totalCount,
            DraftCount = draftCount,
            ReviewCount = reviewCount,
            ApprovalCount = approvalCount,
            ImplementCount = implementCount,
            VerifyCount = verifyCount,
            ClosedCount = closedCount,
            RejectedCount = rejectedCount,
            UrgentCount = urgentCount,
            CloseRate = closeRate,
            AvgDaysToClose = avgDaysToClose,
            TypeDistribution = typeDistribution.ToDictionary(x => x.Type, x => x.Count),
            SeverityDistribution = severityDistribution.ToDictionary(x => x.Severity, x => x.Count),
            StatusDistribution = statusDistribution.ToDictionary(x => x.Status, x => x.Count)
        };
    }

    // ============================================================================
    // Sub-table: EcnItem
    // ============================================================================

    public async Task<List<EcnItemDto>> GetItemsAsync(string ecnId)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return [];

        var items = await _context.EcnItems
            .Where(x => x.EcnId == ecnId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        return items.Select(MapItemToDto).ToList();
    }

    public async Task<EcnItemDto> CreateItemAsync(CreateEcnItemRequest request, string createdBy)
    {
        var item = new EcnItem
        {
            ItemId = IdHelper.GenerateId(),
            EcnId = request.EcnId,
            ItemType = request.ItemType,
            ItemCode = request.ItemCode,
            ItemName = request.ItemName,
            OldValue = request.OldValue,
            NewValue = request.NewValue,
            ChangeReason = request.ChangeReason,
            Remark = request.Remark,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _context.EcnItems.AddAsync(item);
        await _context.SaveChangesAsync();

        return MapItemToDto(item);
    }

    public async Task<bool> UpdateItemAsync(string itemId, CreateEcnItemRequest request, string updatedBy)
    {
        var item = await _context.EcnItems.FindAsync(itemId);
        if (item == null) return false;

        item.ItemType = request.ItemType;
        item.ItemCode = request.ItemCode;
        item.ItemName = request.ItemName;
        item.OldValue = request.OldValue ?? item.OldValue;
        item.NewValue = request.NewValue ?? item.NewValue;
        item.ChangeReason = request.ChangeReason ?? item.ChangeReason;
        item.Remark = request.Remark ?? item.Remark;
        item.SortOrder = request.SortOrder;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = updatedBy;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteItemAsync(string itemId)
    {
        var item = await _context.EcnItems.FindAsync(itemId);
        if (item == null) return false;

        _context.EcnItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Sub-table: EcnImpactItem
    // ============================================================================

    public async Task<List<EcnImpactItemDto>> GetImpactsAsync(string ecnId)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return [];

        var items = await _context.EcnImpactItems
            .Where(x => x.EcnId == ecnId)
            .ToListAsync();

        return items.Select(MapImpactToDto).ToList();
    }

    public async Task<EcnImpactItemDto> CreateImpactAsync(CreateEcnImpactItemRequest request, string createdBy)
    {
        var item = new EcnImpactItem
        {
            ImpactId = IdHelper.GenerateId(),
            EcnId = request.EcnId,
            ImpactType = request.ImpactType,
            Severity = request.Severity,
            Description = request.Description,
            ImpactAnalysis = request.ImpactAnalysis,
            Action = request.Action,
            Responsible = request.Responsible,
            DueDate = request.DueDate,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _context.EcnImpactItems.AddAsync(item);
        await _context.SaveChangesAsync();

        return MapImpactToDto(item);
    }

    public async Task<bool> DeleteImpactAsync(string impactId)
    {
        var item = await _context.EcnImpactItems.FindAsync(impactId);
        if (item == null) return false;

        _context.EcnImpactItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Sub-table: EcnApprover
    // ============================================================================

    public async Task<List<EcnApproverDto>> GetApproversAsync(string ecnId)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return [];

        var items = await _context.EcnApprovers
            .Where(x => x.EcnId == ecnId)
            .OrderBy(x => x.ApprovalOrder)
            .ToListAsync();

        return items.Select(MapApproverToDto).ToList();
    }

    public async Task<EcnApproverDto> CreateApproverAsync(CreateEcnApproverRequest request)
    {
        var item = new EcnApprover
        {
            ApproverId = IdHelper.GenerateId(),
            EcnId = request.EcnId,
            ApproverName = request.ApproverName,
            Role = request.Role,
            ApprovalOrder = request.ApprovalOrder,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _context.EcnApprovers.AddAsync(item);
        await _context.SaveChangesAsync();

        return MapApproverToDto(item);
    }

    public async Task<bool> UpdateApproverAsync(UpdateEcnApproverRequest request)
    {
        var item = await _context.EcnApprovers.FindAsync(request.ApproverId);
        if (item == null) return false;

        item.Status = request.Status;
        item.Result = request.Result ?? item.Result;
        item.Comments = request.Comments ?? item.Comments;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteApproverAsync(string approverId)
    {
        var item = await _context.EcnApprovers.FindAsync(approverId);
        if (item == null) return false;

        _context.EcnApprovers.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Sub-table: EcnNotifyDept
    // ============================================================================

    public async Task<List<EcnNotifyDeptDto>> GetNotifyDeptsAsync(string ecnId)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return [];

        var items = await _context.EcnNotifyDepts
            .Where(x => x.EcnId == ecnId)
            .ToListAsync();

        return items.Select(MapNotifyDeptToDto).ToList();
    }

    public async Task<EcnNotifyDeptDto> CreateNotifyDeptAsync(CreateEcnNotifyDeptRequest request)
    {
        var item = new EcnNotifyDept
        {
            NotifyId = IdHelper.GenerateId(),
            EcnId = request.EcnId,
            DeptId = request.DeptId,
            DeptName = request.DeptName,
            Confirmed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _context.EcnNotifyDepts.AddAsync(item);
        await _context.SaveChangesAsync();

        return MapNotifyDeptToDto(item);
    }

    public async Task<bool> ConfirmNotifyAsync(string notifyId, string confirmedBy)
    {
        var item = await _context.EcnNotifyDepts.FindAsync(notifyId);
        if (item == null) return false;

        item.Confirmed = true;
        item.ConfirmedBy = confirmedBy;
        item.ConfirmedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteNotifyDeptAsync(string notifyId)
    {
        var item = await _context.EcnNotifyDepts.FindAsync(notifyId);
        if (item == null) return false;

        _context.EcnNotifyDepts.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Sub-table: EcnImplement
    // ============================================================================

    public async Task<List<EcnImplementDto>> GetImplementsAsync(string ecnId)
    {
        var ecn = await _context.EcnRequests.FindAsync(ecnId);
        if (ecn == null) return [];

        var items = await _context.EcnImplements
            .Where(x => x.EcnId == ecnId)
            .ToListAsync();

        return items.Select(MapImplementToDto).ToList();
    }

    public async Task<EcnImplementDto> CreateImplementAsync(CreateEcnImplementRequest request, string createdBy)
    {
        var item = new EcnImplement
        {
            ImplementId = IdHelper.GenerateId(),
            EcnId = request.EcnId,
            TaskName = request.TaskName,
            Description = request.Description,
            Responsible = request.Responsible,
            PlanDate = request.PlanDate,
            Status = "Pending",
            Remark = request.Remark,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        await _context.EcnImplements.AddAsync(item);
        await _context.SaveChangesAsync();

        return MapImplementToDto(item);
    }

    public async Task<bool> UpdateImplementAsync(UpdateEcnImplementRequest request)
    {
        var item = await _context.EcnImplements.FindAsync(request.ImplementId);
        if (item == null) return false;

        item.Status = request.Status;
        item.ActualDate = request.ActualDate ?? item.ActualDate;
        item.Result = request.Result ?? item.Result;
        item.Remark = request.Remark ?? item.Remark;
        item.UpdatedAt = DateTime.UtcNow;
        item.UpdatedBy = item.Responsible;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteImplementAsync(string implementId)
    {
        var item = await _context.EcnImplements.FindAsync(implementId);
        if (item == null) return false;

        _context.EcnImplements.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Helper Methods
    // ============================================================================

    private static EcnRequestDto MapToDto(EcnRequest e)
    {
        return new EcnRequestDto
        {
            EcnId = e.EcnId,
            EcnNo = e.EcnNo,
            EcnTitle = e.EcnTitle,
            EcnType = e.EcnType,
            ChangeCategory = e.ChangeCategory,
            Reason = e.Reason,
            ChangeDescription = e.ChangeDescription,
            ChangeContent = e.ChangeContent,
            OldValue = e.OldValue,
            NewValue = e.NewValue,
            Status = e.Status,
            AffectedRoutes = e.AffectedRoutes,
            AffectedProducts = e.AffectedProducts,
            ImpactAssessment = e.ImpactAssessment,
            Urgency = e.Urgency,
            RiskLevel = e.RiskLevel,
            ReviewComments = e.ReviewComments,
            RejectReason = e.RejectReason,
            VerifyResult = e.VerifyResult,
            RequestedBy = e.RequestedBy,
            RequestedAt = e.RequestedAt,
            ApprovedBy = e.ApprovedBy,
            ApprovedAt = e.ApprovedAt,
            EffectiveDate = e.EffectiveDate,
            PlannedDate = e.PlannedDate,
            ActualDate = e.ActualDate,
            IsComplete = e.IsComplete,
            CloseDate = e.CloseDate,
            DaysElapsed = e.DaysElapsed,
            OAFlowId = e.OAFlowId,
            OANo = e.OANo,
            IsUrgent = e.IsUrgent,
            CostEstimate = e.CostEstimate,
            Remark = e.Remark,
            CreatedAt = e.CreatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedAt = e.UpdatedAt,
            UpdatedBy = e.UpdatedBy,
            Items = [],
            Impacts = [],
            Approvers = [],
            NotifyDepts = [],
            Implements = []
        };
    }

    private static EcnItemDto MapItemToDto(EcnItem e)
    {
        return new EcnItemDto
        {
            ItemId = e.ItemId,
            EcnId = e.EcnId,
            ItemType = e.ItemType,
            ItemCode = e.ItemCode,
            ItemName = e.ItemName,
            OldValue = e.OldValue,
            NewValue = e.NewValue,
            ChangeReason = e.ChangeReason,
            Remark = e.Remark,
            SortOrder = e.SortOrder,
            CreatedAt = e.CreatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedAt = e.UpdatedAt,
            UpdatedBy = e.UpdatedBy
        };
    }

    private static EcnImpactItemDto MapImpactToDto(EcnImpactItem e)
    {
        return new EcnImpactItemDto
        {
            ImpactId = e.ImpactId,
            EcnId = e.EcnId,
            ImpactType = e.ImpactType,
            Severity = e.Severity,
            Description = e.Description,
            ImpactAnalysis = e.ImpactAnalysis,
            Action = e.Action,
            Responsible = e.Responsible,
            DueDate = e.DueDate,
            CreatedAt = e.CreatedAt,
            CreatedBy = e.CreatedBy
        };
    }

    private static EcnApproverDto MapApproverToDto(EcnApprover e)
    {
        return new EcnApproverDto
        {
            ApproverId = e.ApproverId,
            EcnId = e.EcnId,
            ApproverName = e.ApproverName,
            Role = e.Role,
            ApprovalOrder = e.ApprovalOrder,
            Status = e.Status,
            Result = e.Result,
            Comments = e.Comments,
            ApprovedAt = e.ApprovedAt,
            CreatedAt = e.CreatedAt
        };
    }

    private static EcnNotifyDeptDto MapNotifyDeptToDto(EcnNotifyDept e)
    {
        return new EcnNotifyDeptDto
        {
            NotifyId = e.NotifyId,
            EcnId = e.EcnId,
            DeptId = e.DeptId,
            DeptName = e.DeptName,
            Confirmed = e.Confirmed,
            NotifiedAt = e.NotifiedAt,
            ConfirmedBy = e.ConfirmedBy,
            ConfirmedAt = e.ConfirmedAt,
            CreatedAt = e.CreatedAt
        };
    }

    private static EcnImplementDto MapImplementToDto(EcnImplement e)
    {
        return new EcnImplementDto
        {
            ImplementId = e.ImplementId,
            EcnId = e.EcnId,
            TaskName = e.TaskName,
            Description = e.Description,
            Responsible = e.Responsible,
            PlanDate = e.PlanDate,
            ActualDate = e.ActualDate,
            Status = e.Status,
            Result = e.Result,
            Remark = e.Remark,
            CreatedAt = e.CreatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedAt = e.UpdatedAt,
            UpdatedBy = e.UpdatedBy
        };
    }

    private async Task<string> GenerateEcnNoAsync(DateTime now)
    {
        var year = now.Year;
        var prefix = $"ECN-{year}-";

        var lastEcn = await _context.EcnRequests
            .Where(x => x.EcnNo.StartsWith(prefix))
            .OrderByDescending(x => x.EcnNo)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastEcn != null)
        {
            var lastSeqStr = lastEcn.EcnNo.Substring(prefix.Length);
            if (int.TryParse(lastSeqStr, out int lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D3}";
    }

    private static void UpdateDaysElapsed(EcnRequest ecn)
    {
        ecn.DaysElapsed = (int)(DateTime.UtcNow - ecn.RequestedAt).TotalDays;
    }
}

/// <summary>
/// Helper class for generating unique IDs.
/// Uses timestamp-based approach for uniqueness.
/// </summary>
internal static class IdHelper
{
    private static readonly object _lock = new object();
    private static int _counter;

    public static string GenerateId()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        int counter;
        lock (_lock)
        {
            counter = ++_counter % 10000;
        }
        return $"ECN{timestamp.Substring(timestamp.Length - 8)}{counter:D4}";
    }
}
