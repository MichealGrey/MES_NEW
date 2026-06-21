using MES.Contracts.Common;
using MES.Contracts.Quality;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Quality;

public interface IComplaint8DService
{
    // ===== Main CRUD =====
    Task<PagedResult<Complaint8DDto>> GetPagedAsync(int pageIndex, int pageSize, string? status = null, string? eightDStatus = null, string? customerId = null, string? severity = null);
    Task<Complaint8DDto?> GetByIdAsync(string complaintId);
    Task<Complaint8DDto> CreateAsync(CreateComplaint8DRequest request, string createdBy);
    Task<bool> UpdateAsync(UpdateComplaint8DRequest request, string updatedBy);
    Task<bool> CloseAsync(string complaintId, string closedBy, string? closureComment = null);

    // ===== Workflow =====
    Task<bool> AdvanceStepAsync(string complaintId, string targetStep, string? comment = null);
    Task<bool> ApproveAsync(string complaintId, bool approved, string? comment, string approver);

    // ===== Statistics =====
    Task<ComplaintStatisticsDto> GetStatisticsAsync();

    // ===== Sub-table: Team Members =====
    Task AddTeamMemberAsync(string complaintId, Complaint8DTeamMemberDto member);
    Task<bool> DeleteTeamMemberAsync(string memberId);

    // ===== Sub-table: Containments =====
    Task AddContainmentAsync(string complaintId, Complaint8DContainmentDto dto);
    Task<bool> UpdateContainmentAsync(string containmentId, Complaint8DContainmentDto dto);
    Task<bool> DeleteContainmentAsync(string containmentId);

    // ===== Sub-table: Root Causes =====
    Task AddRootCauseAsync(string complaintId, Complaint8DRootCauseDto dto);
    Task<bool> UpdateRootCauseAsync(string causeId, Complaint8DRootCauseDto dto);
    Task<bool> DeleteRootCauseAsync(string causeId);

    // ===== Sub-table: Actions =====
    Task AddActionAsync(string complaintId, Complaint8DActionDto dto);
    Task<bool> UpdateActionAsync(string actionId, Complaint8DActionDto dto);
    Task<bool> DeleteActionAsync(string actionId);

    // ===== Sub-table: Doc Updates =====
    Task AddDocUpdateAsync(string complaintId, Complaint8DDocUpdateDto dto);
    Task<bool> UpdateDocUpdateAsync(string docId, Complaint8DDocUpdateDto dto);
    Task<bool> DeleteDocUpdateAsync(string docId);
}

public class Complaint8DService : IComplaint8DService
{
    private readonly MesDbContext _context;

    public Complaint8DService(MesDbContext context)
    {
        _context = context;
    }

    // ============================================================================
    // Main CRUD
    // ============================================================================

    public async Task<PagedResult<Complaint8DDto>> GetPagedAsync(int pageIndex, int pageSize, string? status = null, string? eightDStatus = null, string? customerId = null, string? severity = null)
    {
        var query = _context.Complaint8Ds.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);

        if (!string.IsNullOrEmpty(eightDStatus))
            query = query.Where(x => x.EightDStatus == eightDStatus);

        if (!string.IsNullOrEmpty(customerId))
            query = query.Where(x => x.CustomerId == customerId);

        if (!string.IsNullOrEmpty(severity))
            query = query.Where(x => x.Severity == severity);

        query = query.OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = items.Select(MapToDto).ToList();
        return new PagedResult<Complaint8DDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<Complaint8DDto?> GetByIdAsync(string complaintId)
    {
        var complaint = await _context.Complaint8Ds.FindAsync(complaintId);
        if (complaint == null) return null;

        var dto = MapToDto(complaint);
        // Load sub-table data
        dto.TeamMembers = await _context.Set<Complaint8DTeamMember>()
            .Where(x => x.ComplaintId == complaintId)
            .Select(x => new Complaint8DTeamMemberDto
            {
                MemberId = x.MemberId,
                ComplaintId = x.ComplaintId,
                MemberName = x.MemberName,
                Department = x.Department,
                Role = x.Role,
                ContactInfo = x.ContactInfo,
                JoinDate = x.JoinDate,
                Remark = x.Remark
            })
            .ToListAsync();

        dto.Containments = await _context.Set<Complaint8DContainment>()
            .Where(x => x.ComplaintId == complaintId)
            .Select(x => new Complaint8DContainmentDto
            {
                ContainmentId = x.ContainmentId,
                ComplaintId = x.ComplaintId,
                ActionDescription = x.ActionDescription,
                AffectedLot = x.AffectedLot,
                AffectedQty = x.AffectedQty,
                ContainedQty = x.ContainedQty,
                Disposition = x.Disposition,
                Result = x.Result,
                ResponsiblePerson = x.ResponsiblePerson,
                PlanDate = x.PlanDate,
                ActualDate = x.ActualDate,
                Status = x.Status,
                Remark = x.Remark
            })
            .ToListAsync();

        dto.RootCauses = await _context.Set<Complaint8DRootCause>()
            .Where(x => x.ComplaintId == complaintId)
            .Select(x => new Complaint8DRootCauseDto
            {
                CauseId = x.CauseId,
                ComplaintId = x.ComplaintId,
                CauseType = x.CauseType,
                AnalysisMethod = x.AnalysisMethod,
                CauseDescription = x.CauseDescription,
                Why1 = x.Why1,
                Why2 = x.Why2,
                Why3 = x.Why3,
                Why4 = x.Why4,
                Why5 = x.Why5,
                RootCauseConclusion = x.RootCauseConclusion,
                ResponsiblePerson = x.ResponsiblePerson,
                AnalysisDate = x.AnalysisDate,
                Remark = x.Remark
            })
            .ToListAsync();

        dto.Actions = await _context.Set<Complaint8DAction>()
            .Where(x => x.ComplaintId == complaintId)
            .Select(x => new Complaint8DActionDto
            {
                ActionId = x.ActionId,
                ComplaintId = x.ComplaintId,
                ActionType = x.ActionType,
                ActionDescription = x.ActionDescription,
                ResponsiblePerson = x.ResponsiblePerson,
                PlanDate = x.PlanDate,
                ActualDate = x.ActualDate,
                Status = x.Status,
                VerificationMethod = x.VerificationMethod,
                VerificationResult = x.VerificationResult,
                VerificationDate = x.VerificationDate,
                Remark = x.Remark
            })
            .ToListAsync();

        dto.DocUpdates = await _context.Set<Complaint8DDocUpdate>()
            .Where(x => x.ComplaintId == complaintId)
            .Select(x => new Complaint8DDocUpdateDto
            {
                DocId = x.DocId,
                ComplaintId = x.ComplaintId,
                DocType = x.DocType,
                DocName = x.DocName,
                DocNo = x.DocNo,
                UpdateDescription = x.UpdateDescription,
                ResponsiblePerson = x.ResponsiblePerson,
                PlanDate = x.PlanDate,
                ActualDate = x.ActualDate,
                Status = x.Status,
                Remark = x.Remark
            })
            .ToListAsync();

        dto.AttachmentsList = await _context.Set<Complaint8DAttachment>()
            .Where(x => x.ComplaintId == complaintId)
            .Select(x => new Complaint8DAttachmentDto
            {
                AttachmentId = x.AttachmentId,
                ComplaintId = x.ComplaintId,
                FileName = x.FileName,
                FilePath = x.FilePath,
                FileType = x.FileType,
                FileSize = x.FileSize,
                UploadStage = x.UploadStage,
                UploadedBy = x.UploadedBy,
                UploadedAt = x.UploadedAt,
                Remark = x.Remark
            })
            .ToListAsync();

        return dto;
    }

    public async Task<Complaint8DDto> CreateAsync(CreateComplaint8DRequest request, string createdBy)
    {
        var customer = await _context.MasterCustomers.FindAsync(request.CustomerId);

        var complaint = new Complaint8D
        {
            ComplaintId = GenerateComplaintId(),
            CustomerId = request.CustomerId,
            CustomerName = customer?.CustomerName,
            OrderNo = request.OrderNo,
            CustomerPONO = request.CustomerPONO,
            LotId = request.LotId,
            ProductId = request.ProductId,
            DefectType = request.DefectType,
            Severity = request.Severity,
            Priority = request.Priority ?? "Normal",
            Status = "Open",
            EightDStatus = "D0",
            D0Assessment = null,
            D0Date = DateTime.UtcNow,
            D2ProblemDescription = request.D2ProblemDescription,
            AffectedQty = request.AffectedQty,
            ReturnQty = request.ReturnQty,
            SampleQty = request.SampleQty,
            DueDate = request.DueDate,
            ApprovalStatus = "Pending",
            Remark = request.Remark,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            OverdueDays = 0
        };

        await _context.Complaint8Ds.AddAsync(complaint);
        await _context.SaveChangesAsync();

        return MapToDto(complaint);
    }

    public async Task<bool> UpdateAsync(UpdateComplaint8DRequest request, string updatedBy)
    {
        var complaint = await _context.Complaint8Ds.FindAsync(request.ComplaintId);
        if (complaint == null) return false;

        // D1
        complaint.D1TeamMembers = request.D1TeamMembers;

        // D2
        complaint.D2ProblemDescription = request.D2ProblemDescription;
        complaint.D2What = request.D2What;
        complaint.D2Who = request.D2Who;
        complaint.D2Where = request.D2Where;
        complaint.D2When = request.D2When;
        complaint.D2Why = request.D2Why;
        complaint.D2How = request.D2How;
        complaint.D2HowMany = request.D2HowMany;
        complaint.D2DefectLocation = request.D2DefectLocation;
        complaint.D2OccurrenceDate = request.D2OccurrenceDate;
        complaint.D2DiscoveryDate = request.D2DiscoveryDate;
        complaint.D2DiscoveryMethod = request.D2DiscoveryMethod;

        // D3
        complaint.D3ContainmentAction = request.D3ContainmentAction;
        complaint.D3ContainmentResult = request.D3ContainmentResult;
        complaint.D3ContainmentDate = request.D3ContainmentDate;

        // D4
        complaint.D4RootCause = request.D4RootCause;
        complaint.D4AnalysisMethod = request.D4AnalysisMethod;
        complaint.D4OccurrenceCause = request.D4OccurrenceCause;
        complaint.D4EscapeCause = request.D4EscapeCause;

        // D5
        complaint.D5PermanentAction = request.D5PermanentAction;
        complaint.D5ActionValidation = request.D5ActionValidation;
        complaint.D5ValidationDate = request.D5ValidationDate;

        // D6
        complaint.D6Implementation = request.D6Implementation;
        complaint.D6VerificationResult = request.D6VerificationResult;
        complaint.D6ImplementDate = request.D6ImplementDate;

        // D7
        complaint.D7Prevention = request.D7Prevention;
        complaint.D7DocUpdateList = request.D7DocUpdateList;
        complaint.D7Standardization = request.D7Standardization;
        complaint.D7HorizontalExpand = request.D7HorizontalExpand;

        // D8
        complaint.D8ClosureComment = request.D8ClosureComment;
        complaint.D8TeamRecognition = request.D8TeamRecognition;
        complaint.D8EffectivenessConfirm = request.D8EffectivenessConfirm;

        complaint.Status = request.Status;
        complaint.EightDStatus = request.EightDStatus;
        complaint.Remark = request.Remark;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CloseAsync(string complaintId, string closedBy, string? closureComment = null)
    {
        var complaint = await _context.Complaint8Ds.FindAsync(complaintId);
        if (complaint == null) return false;

        complaint.Status = "Closed";
        complaint.EightDStatus = "D8";
        complaint.D8Date = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(closureComment))
        {
            complaint.D8ClosureComment = closureComment;
        }
        complaint.ClosedBy = closedBy;
        complaint.ClosedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Workflow
    // ============================================================================

    public async Task<bool> AdvanceStepAsync(string complaintId, string targetStep, string? comment = null)
    {
        var complaint = await _context.Complaint8Ds.FindAsync(complaintId);
        if (complaint == null) return false;

        var validSteps = new[] { "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8" };
        if (!validSteps.Contains(targetStep))
            throw new ArgumentException($"Invalid 8D step: {targetStep}. Valid steps: D0-D8");

        var currentIndex = Array.IndexOf(validSteps, complaint.EightDStatus);
        var targetIndex = Array.IndexOf(validSteps, targetStep);

        // Can only advance to the next step
        if (targetIndex != currentIndex + 1)
            throw new InvalidOperationException($"Cannot advance from {complaint.EightDStatus} to {targetStep}. Must advance one step at a time.");

        var now = DateTime.UtcNow;

        // Set the corresponding date field based on target step
        switch (targetStep)
        {
            case "D0": complaint.D0Date = now; break;
            case "D1": complaint.D1Date = now; break;
            case "D2": complaint.D2Date = now; break;
            case "D3": complaint.D3Date = now; break;
            case "D4": complaint.D4Date = now; break;
            case "D5": complaint.D5Date = now; break;
            case "D6": complaint.D6Date = now; break;
            case "D7": complaint.D7Date = now; break;
            case "D8": complaint.D8Date = now; break;
        }

        complaint.EightDStatus = targetStep;

        // Auto-update overall status
        if (targetStep == "D1" || targetStep == "D2")
        {
            complaint.Status = "InProgress";
        }
        else if (targetStep == "D8")
        {
            complaint.Status = "Closed";
            complaint.ClosedAt = now;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApproveAsync(string complaintId, bool approved, string? comment, string approver)
    {
        var complaint = await _context.Complaint8Ds.FindAsync(complaintId);
        if (complaint == null) return false;

        complaint.ApprovalStatus = approved ? "Approved" : "Rejected";
        complaint.Approver = approver;
        complaint.ApproveDate = DateTime.UtcNow;
        complaint.ApprovalComment = comment;

        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Statistics
    // ============================================================================

    public async Task<ComplaintStatisticsDto> GetStatisticsAsync()
    {
        var totalCount = await _context.Complaint8Ds.CountAsync();
        if (totalCount == 0)
        {
            return new ComplaintStatisticsDto();
        }

        var openCount = await _context.Complaint8Ds.CountAsync(x => x.Status == "Open");
        var inProgressCount = await _context.Complaint8Ds.CountAsync(x => x.Status == "InProgress");
        var closedCount = await _context.Complaint8Ds.CountAsync(x => x.Status == "Closed");

        // Overdue: DueDate is in the past and status is not Closed
        var overdueCount = await _context.Complaint8Ds.CountAsync(x =>
            x.DueDate != null && x.DueDate < DateTime.UtcNow && x.Status != "Closed");

        var closeRate = totalCount > 0 ? (double)closedCount / totalCount * 100 : 0;

        // Average days open for non-closed complaints
        var avgOpenDays = await _context.Complaint8Ds
            .Where(x => x.Status != "Closed")
            .Select(x => (int)EF.Functions.DateDiffDay(x.CreatedAt, DateTime.UtcNow))
            .DefaultIfEmpty(0)
            .AverageAsync();

        var highSeverityCount = await _context.Complaint8Ds
            .CountAsync(x => x.Severity == "High" || x.Severity == "Critical");

        // Top defect type
        var defectTypeDistribution = await _context.Complaint8Ds
            .Where(x => !string.IsNullOrEmpty(x.DefectType))
            .GroupBy(x => x.DefectType!)
            .Select(g => new { DefectType = g.Key, Count = g.Count() })
            .ToListAsync();

        var topDefectType = defectTypeDistribution.OrderByDescending(x => x.Count).FirstOrDefault()?.DefectType;

        // Status distribution
        var statusDistribution = await _context.Complaint8Ds
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        // 8D step distribution
        var eightDStepDistribution = await _context.Complaint8Ds
            .GroupBy(x => x.EightDStatus)
            .Select(g => new { Step = g.Key, Count = g.Count() })
            .ToListAsync();

        return new ComplaintStatisticsDto
        {
            TotalCount = totalCount,
            OpenCount = openCount,
            InProgressCount = inProgressCount,
            ClosedCount = closedCount,
            OverdueCount = overdueCount,
            CloseRate = closeRate,
            AvgOpenDays = avgOpenDays,
            HighSeverityCount = highSeverityCount,
            TopDefectType = topDefectType,
            DefectTypeDistribution = defectTypeDistribution.ToDictionary(x => x.DefectType, x => x.Count),
            StatusDistribution = statusDistribution.ToDictionary(x => x.Status, x => x.Count),
            EightDStepDistribution = eightDStepDistribution.ToDictionary(x => x.Step, x => x.Count)
        };
    }

    // ============================================================================
    // Sub-table: Team Members
    // ============================================================================

    public async Task AddTeamMemberAsync(string complaintId, Complaint8DTeamMemberDto dto)
    {
        var member = new Complaint8DTeamMember
        {
            MemberId = GenerateId("TM"),
            ComplaintId = complaintId,
            MemberName = dto.MemberName,
            Department = dto.Department,
            Role = dto.Role,
            ContactInfo = dto.ContactInfo,
            JoinDate = dto.JoinDate,
            Remark = dto.Remark
        };

        await _context.Set<Complaint8DTeamMember>().AddAsync(member);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteTeamMemberAsync(string memberId)
    {
        var member = await _context.Set<Complaint8DTeamMember>().FindAsync(memberId);
        if (member == null) return false;

        _context.Set<Complaint8DTeamMember>().Remove(member);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Sub-table: Containments
    // ============================================================================

    public async Task AddContainmentAsync(string complaintId, Complaint8DContainmentDto dto)
    {
        var entity = new Complaint8DContainment
        {
            ContainmentId = GenerateId("CT"),
            ComplaintId = complaintId,
            ActionDescription = dto.ActionDescription,
            AffectedLot = dto.AffectedLot,
            AffectedQty = dto.AffectedQty,
            ContainedQty = dto.ContainedQty,
            Disposition = dto.Disposition,
            Result = dto.Result,
            ResponsiblePerson = dto.ResponsiblePerson,
            PlanDate = dto.PlanDate,
            ActualDate = dto.ActualDate,
            Status = dto.Status,
            Remark = dto.Remark
        };

        await _context.Set<Complaint8DContainment>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateContainmentAsync(string containmentId, Complaint8DContainmentDto dto)
    {
        var entity = await _context.Set<Complaint8DContainment>().FindAsync(containmentId);
        if (entity == null) return false;

        entity.ActionDescription = dto.ActionDescription;
        entity.AffectedLot = dto.AffectedLot;
        entity.AffectedQty = dto.AffectedQty;
        entity.ContainedQty = dto.ContainedQty;
        entity.Disposition = dto.Disposition;
        entity.Result = dto.Result;
        entity.ResponsiblePerson = dto.ResponsiblePerson;
        entity.PlanDate = dto.PlanDate;
        entity.ActualDate = dto.ActualDate;
        entity.Status = dto.Status;
        entity.Remark = dto.Remark;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteContainmentAsync(string containmentId)
    {
        var entity = await _context.Set<Complaint8DContainment>().FindAsync(containmentId);
        if (entity == null) return false;

        _context.Set<Complaint8DContainment>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Sub-table: Root Causes
    // ============================================================================

    public async Task AddRootCauseAsync(string complaintId, Complaint8DRootCauseDto dto)
    {
        var entity = new Complaint8DRootCause
        {
            CauseId = GenerateId("RC"),
            ComplaintId = complaintId,
            CauseType = dto.CauseType,
            AnalysisMethod = dto.AnalysisMethod,
            CauseDescription = dto.CauseDescription,
            Why1 = dto.Why1,
            Why2 = dto.Why2,
            Why3 = dto.Why3,
            Why4 = dto.Why4,
            Why5 = dto.Why5,
            RootCauseConclusion = dto.RootCauseConclusion,
            ResponsiblePerson = dto.ResponsiblePerson,
            AnalysisDate = dto.AnalysisDate,
            Remark = dto.Remark
        };

        await _context.Set<Complaint8DRootCause>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateRootCauseAsync(string causeId, Complaint8DRootCauseDto dto)
    {
        var entity = await _context.Set<Complaint8DRootCause>().FindAsync(causeId);
        if (entity == null) return false;

        entity.CauseType = dto.CauseType;
        entity.AnalysisMethod = dto.AnalysisMethod;
        entity.CauseDescription = dto.CauseDescription;
        entity.Why1 = dto.Why1;
        entity.Why2 = dto.Why2;
        entity.Why3 = dto.Why3;
        entity.Why4 = dto.Why4;
        entity.Why5 = dto.Why5;
        entity.RootCauseConclusion = dto.RootCauseConclusion;
        entity.ResponsiblePerson = dto.ResponsiblePerson;
        entity.AnalysisDate = dto.AnalysisDate;
        entity.Remark = dto.Remark;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteRootCauseAsync(string causeId)
    {
        var entity = await _context.Set<Complaint8DRootCause>().FindAsync(causeId);
        if (entity == null) return false;

        _context.Set<Complaint8DRootCause>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Sub-table: Actions
    // ============================================================================

    public async Task AddActionAsync(string complaintId, Complaint8DActionDto dto)
    {
        var entity = new Complaint8DAction
        {
            ActionId = GenerateId("AC"),
            ComplaintId = complaintId,
            ActionType = dto.ActionType,
            ActionDescription = dto.ActionDescription,
            ResponsiblePerson = dto.ResponsiblePerson,
            PlanDate = dto.PlanDate,
            ActualDate = dto.ActualDate,
            Status = dto.Status,
            VerificationMethod = dto.VerificationMethod,
            VerificationResult = dto.VerificationResult,
            VerificationDate = dto.VerificationDate,
            Remark = dto.Remark
        };

        await _context.Set<Complaint8DAction>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateActionAsync(string actionId, Complaint8DActionDto dto)
    {
        var entity = await _context.Set<Complaint8DAction>().FindAsync(actionId);
        if (entity == null) return false;

        entity.ActionType = dto.ActionType;
        entity.ActionDescription = dto.ActionDescription;
        entity.ResponsiblePerson = dto.ResponsiblePerson;
        entity.PlanDate = dto.PlanDate;
        entity.ActualDate = dto.ActualDate;
        entity.Status = dto.Status;
        entity.VerificationMethod = dto.VerificationMethod;
        entity.VerificationResult = dto.VerificationResult;
        entity.VerificationDate = dto.VerificationDate;
        entity.Remark = dto.Remark;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteActionAsync(string actionId)
    {
        var entity = await _context.Set<Complaint8DAction>().FindAsync(actionId);
        if (entity == null) return false;

        _context.Set<Complaint8DAction>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Sub-table: Doc Updates
    // ============================================================================

    public async Task AddDocUpdateAsync(string complaintId, Complaint8DDocUpdateDto dto)
    {
        var entity = new Complaint8DDocUpdate
        {
            DocId = GenerateId("DU"),
            ComplaintId = complaintId,
            DocType = dto.DocType,
            DocName = dto.DocName,
            DocNo = dto.DocNo,
            UpdateDescription = dto.UpdateDescription,
            ResponsiblePerson = dto.ResponsiblePerson,
            PlanDate = dto.PlanDate,
            ActualDate = dto.ActualDate,
            Status = dto.Status,
            Remark = dto.Remark
        };

        await _context.Set<Complaint8DDocUpdate>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateDocUpdateAsync(string docId, Complaint8DDocUpdateDto dto)
    {
        var entity = await _context.Set<Complaint8DDocUpdate>().FindAsync(docId);
        if (entity == null) return false;

        entity.DocType = dto.DocType;
        entity.DocName = dto.DocName;
        entity.DocNo = dto.DocNo;
        entity.UpdateDescription = dto.UpdateDescription;
        entity.ResponsiblePerson = dto.ResponsiblePerson;
        entity.PlanDate = dto.PlanDate;
        entity.ActualDate = dto.ActualDate;
        entity.Status = dto.Status;
        entity.Remark = dto.Remark;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDocUpdateAsync(string docId)
    {
        var entity = await _context.Set<Complaint8DDocUpdate>().FindAsync(docId);
        if (entity == null) return false;

        _context.Set<Complaint8DDocUpdate>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    // ============================================================================
    // Helper Methods
    // ============================================================================

    private static Complaint8DDto MapToDto(Complaint8D c)
    {
        var dto = new Complaint8DDto
        {
            ComplaintId = c.ComplaintId,
            CustomerId = c.CustomerId,
            CustomerName = c.CustomerName,
            OrderNo = c.OrderNo,
            CustomerPONO = c.CustomerPONO,
            LotId = c.LotId,
            ProductId = c.ProductId,
            DefectType = c.DefectType,
            Severity = c.Severity,
            Priority = c.Priority,
            Status = c.Status,
            EightDStatus = c.EightDStatus,
            AffectedQty = c.AffectedQty,
            ReturnQty = c.ReturnQty,
            SampleQty = c.SampleQty,

            D0Assessment = c.D0Assessment,
            D0AssessmentComment = c.D0AssessmentComment,
            D0Date = c.D0Date,

            D1TeamMembers = c.D1TeamMembers,
            D1Date = c.D1Date,

            D2ProblemDescription = c.D2ProblemDescription,
            D2What = c.D2What,
            D2Who = c.D2Who,
            D2Where = c.D2Where,
            D2When = c.D2When,
            D2Why = c.D2Why,
            D2How = c.D2How,
            D2HowMany = c.D2HowMany,
            D2DefectLocation = c.D2DefectLocation,
            D2OccurrenceDate = c.D2OccurrenceDate,
            D2DiscoveryDate = c.D2DiscoveryDate,
            D2DiscoveryMethod = c.D2DiscoveryMethod,
            D2Date = c.D2Date,

            D3ContainmentAction = c.D3ContainmentAction,
            D3ContainmentResult = c.D3ContainmentResult,
            D3ContainmentDate = c.D3ContainmentDate,
            D3Date = c.D3Date,

            D4RootCause = c.D4RootCause,
            D4AnalysisMethod = c.D4AnalysisMethod,
            D4OccurrenceCause = c.D4OccurrenceCause,
            D4EscapeCause = c.D4EscapeCause,
            D4Date = c.D4Date,

            D5PermanentAction = c.D5PermanentAction,
            D5ActionValidation = c.D5ActionValidation,
            D5ValidationDate = c.D5ValidationDate,
            D5Date = c.D5Date,

            D6Implementation = c.D6Implementation,
            D6VerificationResult = c.D6VerificationResult,
            D6ImplementDate = c.D6ImplementDate,
            D6Date = c.D6Date,

            D7Prevention = c.D7Prevention,
            D7DocUpdateList = c.D7DocUpdateList,
            D7Standardization = c.D7Standardization,
            D7HorizontalExpand = c.D7HorizontalExpand,
            D7Date = c.D7Date,

            D8ClosureComment = c.D8ClosureComment,
            D8TeamRecognition = c.D8TeamRecognition,
            D8EffectivenessConfirm = c.D8EffectivenessConfirm,
            D8Date = c.D8Date,

            ApprovalStatus = c.ApprovalStatus,
            Approver = c.Approver,
            ApproveDate = c.ApproveDate,
            ApprovalComment = c.ApprovalComment,

            CreatedBy = c.CreatedBy,
            CreatedAt = c.CreatedAt,
            ClosedAt = c.ClosedAt,
            ClosedBy = c.ClosedBy,
            DueDate = c.DueDate,
            OverdueDays = CalculateOverdueDays(c.DueDate),
            Attachments = c.Attachments,
            Remark = c.Remark
        };

        return dto;
    }

    private static int CalculateOverdueDays(DateTime? dueDate)
    {
        if (dueDate == null) return 0;
        var days = (int)(DateTime.UtcNow - dueDate.Value).TotalDays;
        return days > 0 ? days : 0;
    }

    private static string GenerateComplaintId()
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 999).ToString("D3");
        return $"8D-{now:yyyyMMdd}-{seq}";
    }

    private static string GenerateId(string prefix)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var random = new Random().Next(1000, 9999).ToString();
        return $"{prefix}-{timestamp.Substring(timestamp.Length - 8)}{random}";
    }
}
