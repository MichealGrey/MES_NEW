namespace MES.Infrastructure.Persistence.Entities;

/// <summary>
/// D1 - 8D团队成员明细
/// </summary>
public class Complaint8DTeamMember
{
    public string MemberId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // 组长/质量/工程/生产/采购等
    public string? ContactInfo { get; set; }
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
    public string? Remark { get; set; }
}

/// <summary>
/// D3 - 围堵措施明细
/// </summary>
public class Complaint8DContainment
{
    public string ContainmentId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string ActionDescription { get; set; } = string.Empty;
    public string AffectedLot { get; set; } = string.Empty;
    public int AffectedQty { get; set; }
    public int ContainedQty { get; set; }
    public string Disposition { get; set; } = string.Empty; // 退货/返工/报废/挑选/让步接收
    public string Result { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending/InProgress/Completed
    public string? Remark { get; set; }
}

/// <summary>
/// D4 - 原因分析明细
/// </summary>
public class Complaint8DRootCause
{
    public string CauseId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string CauseType { get; set; } = string.Empty; // Occurrence(发生)/Escape(流出)
    public string AnalysisMethod { get; set; } = string.Empty; // 鱼骨图/5Why/FMEA/FTA等
    public string CauseDescription { get; set; } = string.Empty;
    public string? Why1 { get; set; }
    public string? Why2 { get; set; }
    public string? Why3 { get; set; }
    public string? Why4 { get; set; }
    public string? Why5 { get; set; }
    public string RootCauseConclusion { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime AnalysisDate { get; set; } = DateTime.UtcNow;
    public string? Remark { get; set; }
}

/// <summary>
/// D5/D6 - 纠正措施明细
/// </summary>
public class Complaint8DAction
{
    public string ActionId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty; // Corrective(纠正)/Preventive(预防)
    public string ActionDescription { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending/InProgress/Completed/Verified
    public string VerificationMethod { get; set; } = string.Empty;
    public string? VerificationResult { get; set; }
    public DateTime? VerificationDate { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// D7 - 文件更新记录
/// </summary>
public class Complaint8DDocUpdate
{
    public string DocId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string DocType { get; set; } = string.Empty; // SOP/SIP/FMEA/ControlPlan/WorkInstruction等
    public string DocName { get; set; } = string.Empty;
    public string DocNo { get; set; } = string.Empty;
    public string UpdateDescription { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending/Completed
    public string? Remark { get; set; }
}

/// <summary>
/// 附件管理
/// </summary>
public class Complaint8DAttachment
{
    public string AttachmentId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty; // image/pdf/excel/word/other
    public long FileSize { get; set; }
    public string UploadStage { get; set; } = string.Empty; // D0/D1/D2/D3/D4/D5/D6/D7/D8
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string? Remark { get; set; }
}
