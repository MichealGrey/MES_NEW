namespace MES.Contracts.Quality;

public class Complaint8DDto
{
    public string ComplaintId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? OrderNo { get; set; }
    public string? CustomerPONO { get; set; }
    public string? LotId { get; set; }
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? DefectType { get; set; }
    public string? Severity { get; set; }
    public string? Priority { get; set; } = "Normal";
    public string Status { get; set; } = "Open";
    public string EightDStatus { get; set; } = "D0";
    public int AffectedQty { get; set; }
    public int ReturnQty { get; set; }
    public int SampleQty { get; set; }

    // D0
    public bool? D0Assessment { get; set; }
    public string? D0AssessmentComment { get; set; }
    public DateTime? D0Date { get; set; }

    // D1
    public string? D1TeamMembers { get; set; }
    public DateTime? D1Date { get; set; }

    // D2
    public string? D2ProblemDescription { get; set; }
    public string? D2What { get; set; }
    public string? D2Who { get; set; }
    public string? D2Where { get; set; }
    public string? D2When { get; set; }
    public string? D2Why { get; set; }
    public string? D2How { get; set; }
    public string? D2HowMany { get; set; }
    public string? D2DefectLocation { get; set; }
    public DateTime? D2OccurrenceDate { get; set; }
    public DateTime? D2DiscoveryDate { get; set; }
    public string? D2DiscoveryMethod { get; set; }
    public DateTime? D2Date { get; set; }

    // D3
    public string? D3ContainmentAction { get; set; }
    public string? D3ContainmentResult { get; set; }
    public DateTime? D3ContainmentDate { get; set; }
    public DateTime? D3Date { get; set; }

    // D4
    public string? D4RootCause { get; set; }
    public string? D4AnalysisMethod { get; set; }
    public string? D4OccurrenceCause { get; set; }
    public string? D4EscapeCause { get; set; }
    public DateTime? D4Date { get; set; }

    // D5
    public string? D5PermanentAction { get; set; }
    public string? D5ActionValidation { get; set; }
    public DateTime? D5ValidationDate { get; set; }
    public DateTime? D5Date { get; set; }

    // D6
    public string? D6Implementation { get; set; }
    public string? D6VerificationResult { get; set; }
    public DateTime? D6ImplementDate { get; set; }
    public DateTime? D6Date { get; set; }

    // D7
    public string? D7Prevention { get; set; }
    public string? D7DocUpdateList { get; set; }
    public string? D7Standardization { get; set; }
    public string? D7HorizontalExpand { get; set; }
    public DateTime? D7Date { get; set; }

    // D8
    public string? D8ClosureComment { get; set; }
    public string? D8TeamRecognition { get; set; }
    public string? D8EffectivenessConfirm { get; set; }
    public DateTime? D8Date { get; set; }

    // 审批
    public string? ApprovalStatus { get; set; }
    public string? Approver { get; set; }
    public DateTime? ApproveDate { get; set; }
    public string? ApprovalComment { get; set; }

    // 通用
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? ClosedBy { get; set; }
    public DateTime? DueDate { get; set; }
    public int OverdueDays { get; set; }
    public string? Attachments { get; set; }
    public string? Remark { get; set; }

    // 子表集合
    public List<Complaint8DTeamMemberDto> TeamMembers { get; set; } = [];
    public List<Complaint8DContainmentDto> Containments { get; set; } = [];
    public List<Complaint8DRootCauseDto> RootCauses { get; set; } = [];
    public List<Complaint8DActionDto> Actions { get; set; } = [];
    public List<Complaint8DDocUpdateDto> DocUpdates { get; set; } = [];
    public List<Complaint8DAttachmentDto> AttachmentsList { get; set; } = [];
}

// ===== 子表 DTO =====

public class Complaint8DTeamMemberDto
{
    public string MemberId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }
    public DateTime JoinDate { get; set; }
    public string? Remark { get; set; }
}

public class Complaint8DContainmentDto
{
    public string ContainmentId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string ActionDescription { get; set; } = string.Empty;
    public string AffectedLot { get; set; } = string.Empty;
    public int AffectedQty { get; set; }
    public int ContainedQty { get; set; }
    public string Disposition { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Remark { get; set; }
}

public class Complaint8DRootCauseDto
{
    public string CauseId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string CauseType { get; set; } = string.Empty;
    public string AnalysisMethod { get; set; } = string.Empty;
    public string CauseDescription { get; set; } = string.Empty;
    public string? Why1 { get; set; }
    public string? Why2 { get; set; }
    public string? Why3 { get; set; }
    public string? Why4 { get; set; }
    public string? Why5 { get; set; }
    public string RootCauseConclusion { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime AnalysisDate { get; set; }
    public string? Remark { get; set; }
}

public class Complaint8DActionDto
{
    public string ActionId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string ActionDescription { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string VerificationMethod { get; set; } = string.Empty;
    public string? VerificationResult { get; set; }
    public DateTime? VerificationDate { get; set; }
    public string? Remark { get; set; }
}

public class Complaint8DDocUpdateDto
{
    public string DocId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string DocType { get; set; } = string.Empty;
    public string DocName { get; set; } = string.Empty;
    public string DocNo { get; set; } = string.Empty;
    public string UpdateDescription { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Remark { get; set; }
}

public class Complaint8DAttachmentDto
{
    public string AttachmentId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string UploadStage { get; set; } = string.Empty;
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string? Remark { get; set; }
}

// ===== Request =====

public class CreateComplaint8DRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string? OrderNo { get; set; }
    public string? CustomerPONO { get; set; }
    public string? LotId { get; set; }
    public string? ProductId { get; set; }
    public string? DefectType { get; set; }
    public string? Severity { get; set; }
    public string? Priority { get; set; } = "Normal";
    public int AffectedQty { get; set; }
    public int ReturnQty { get; set; }
    public int SampleQty { get; set; }
    public string? D2ProblemDescription { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Remark { get; set; }
}

public class UpdateComplaint8DRequest
{
    public string ComplaintId { get; set; } = string.Empty;

    // D1
    public string? D1TeamMembers { get; set; }

    // D2
    public string? D2ProblemDescription { get; set; }
    public string? D2What { get; set; }
    public string? D2Who { get; set; }
    public string? D2Where { get; set; }
    public string? D2When { get; set; }
    public string? D2Why { get; set; }
    public string? D2How { get; set; }
    public string? D2HowMany { get; set; }
    public string? D2DefectLocation { get; set; }
    public DateTime? D2OccurrenceDate { get; set; }
    public DateTime? D2DiscoveryDate { get; set; }
    public string? D2DiscoveryMethod { get; set; }

    // D3
    public string? D3ContainmentAction { get; set; }
    public string? D3ContainmentResult { get; set; }
    public DateTime? D3ContainmentDate { get; set; }

    // D4
    public string? D4RootCause { get; set; }
    public string? D4AnalysisMethod { get; set; }
    public string? D4OccurrenceCause { get; set; }
    public string? D4EscapeCause { get; set; }

    // D5
    public string? D5PermanentAction { get; set; }
    public string? D5ActionValidation { get; set; }
    public DateTime? D5ValidationDate { get; set; }

    // D6
    public string? D6Implementation { get; set; }
    public string? D6VerificationResult { get; set; }
    public DateTime? D6ImplementDate { get; set; }

    // D7
    public string? D7Prevention { get; set; }
    public string? D7DocUpdateList { get; set; }
    public string? D7Standardization { get; set; }
    public string? D7HorizontalExpand { get; set; }

    // D8
    public string? D8ClosureComment { get; set; }
    public string? D8TeamRecognition { get; set; }
    public string? D8EffectivenessConfirm { get; set; }

    // 通用
    public string Status { get; set; } = "Open";
    public string EightDStatus { get; set; } = "D0";
    public string? Remark { get; set; }
}

public class Complaint8DAdvancementRequest
{
    public string ComplaintId { get; set; } = string.Empty;
    public string TargetStep { get; set; } = string.Empty; // D1/D2/D3/D4/D5/D6/D7/D8
    public string? Comment { get; set; }
}

public class Complaint8DApprovalRequest
{
    public string ComplaintId { get; set; } = string.Empty;
    public bool Approved { get; set; }
    public string? Comment { get; set; }
}

// ===== 统计 DTO =====

public class ComplaintStatisticsDto
{
    public int TotalCount { get; set; }
    public int OpenCount { get; set; }
    public int InProgressCount { get; set; }
    public int ClosedCount { get; set; }
    public int OverdueCount { get; set; }
    public double CloseRate { get; set; }
    public double AvgOpenDays { get; set; }
    public int HighSeverityCount { get; set; }
    public string? TopDefectType { get; set; }
    public Dictionary<string, int> DefectTypeDistribution { get; set; } = [];
    public Dictionary<string, int> StatusDistribution { get; set; } = [];
    public Dictionary<string, int> EightDStepDistribution { get; set; } = [];
}
