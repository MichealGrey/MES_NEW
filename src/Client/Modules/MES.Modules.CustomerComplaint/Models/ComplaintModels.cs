using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.CustomerComplaint.Models;

/// <summary>
/// 客诉主表信息模型
/// </summary>
public class ComplaintInfo
{
    public string ComplaintId { get; set; } = string.Empty;
    public string ComplaintNo { get; set; } = string.Empty;

    // 基本信息
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;

    // 订单相关
    public string OrderNo { get; set; } = string.Empty;
    public string CustomerPONO { get; set; } = string.Empty;

    // 缺陷相关
    public string DefectType { get; set; } = string.Empty;
    public string DefectDescription { get; set; } = string.Empty;

    // 数量相关
    public int AffectedQty { get; set; }
    public int ReturnQty { get; set; }
    public int SampleQty { get; set; }

    // 日期相关
    public DateTime ReportDate { get; set; } = DateTime.Now;
    public DateTime? RequiredDate { get; set; }
    public DateTime? ActualCloseDate { get; set; }

    // 状态相关
    public string Severity { get; set; } = "Medium";
    public string Status { get; set; } = "Open";
    public string EightDStatus { get; set; } = "D0";
    public string Priority { get; set; } = "Normal";
    public string ApprovalStatus { get; set; } = "Draft";

    // 责任人
    public string AssignedTo { get; set; } = string.Empty;

    // 8D 各阶段内容 (D0-D8)
    public string D0AssessmentComment { get; set; } = string.Empty;
    public bool D0AssessmentNeeded { get; set; }
    public string D1TeamDescription { get; set; } = string.Empty;
    public string D2ProblemDescription { get; set; } = string.Empty;
    public string D2WhatDescription { get; set; } = string.Empty;
    public string D2WhoDescription { get; set; } = string.Empty;
    public string D2WhereDescription { get; set; } = string.Empty;
    public string D2WhenDescription { get; set; } = string.Empty;
    public string D2WhyDescription { get; set; } = string.Empty;
    public string D2HowDescription { get; set; } = string.Empty;
    public string D2HowManyDescription { get; set; } = string.Empty;
    public string D2DefectLocation { get; set; } = string.Empty;
    public DateTime? D2DiscoveryDate { get; set; }
    public DateTime? D2ReportDate { get; set; }
    public string D3ContainmentResult { get; set; } = string.Empty;
    public string D4RootCause { get; set; } = string.Empty;
    public string D5CorrectiveAction { get; set; } = string.Empty;
    public string D6ImplementationResult { get; set; } = string.Empty;
    public string D7PreventionDescription { get; set; } = string.Empty;
    public string D7HorizontalExpansion { get; set; } = string.Empty;
    public string D8ClosureComment { get; set; } = string.Empty;
    public string D8TeamRecognition { get; set; } = string.Empty;
    public bool D8EffectivenessConfirmed { get; set; }

    // 兼容旧字段
    public string RootCause { get; set; } = string.Empty;
    public string CorrectiveAction { get; set; } = string.Empty;
    public string PreventiveAction { get; set; } = string.Empty;

    // 审批相关
    public int OverdueDays
    {
        get
        {
            if (RequiredDate.HasValue)
            {
                var diff = (DateTime.Now - RequiredDate.Value).TotalDays;
                return diff > 0 ? (int)diff : 0;
            }
            return 0;
        }
    }

    public bool IsClosed => Status == "Closed";
    public int OpenDays => IsClosed
        ? (int)((ActualCloseDate ?? DateTime.Now) - ReportDate).TotalDays
        : (int)(DateTime.Now - ReportDate).TotalDays;

    public string? Attachments { get; set; }
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 8D步骤定义
/// </summary>
public class EightDStep
{
    public string Step { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string? CompletedBy { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending / InProgress / Completed
}

/// <summary>
/// 8D小组成员
/// </summary>
public class Complaint8DTeamMember
{
    public string MemberId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string ContactInfo { get; set; } = string.Empty;
    public DateTime? JoinDate { get; set; }
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 8D围堵措施
/// </summary>
public class Complaint8DContainment
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
    public DateTime? PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 8D根因分析
/// </summary>
public class Complaint8DRootCause
{
    public string CauseId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string CauseType { get; set; } = string.Empty; // 产生原因 / 流出原因
    public string AnalysisMethod { get; set; } = string.Empty; // 5Why / 鱼骨图 / FMEA / 故障树
    public string CauseDescription { get; set; } = string.Empty;
    public string Why1 { get; set; } = string.Empty;
    public string Why2 { get; set; } = string.Empty;
    public string Why3 { get; set; } = string.Empty;
    public string Why4 { get; set; } = string.Empty;
    public string Why5 { get; set; } = string.Empty;
    public string RootCauseConclusion { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime? AnalysisDate { get; set; }
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 8D纠正/预防措施
/// </summary>
public class Complaint8DAction
{
    public string ActionId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty; // 纠正 / 预防 / 纠正措施 / 预防措施
    public string ActionDescription { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime? PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string VerificationMethod { get; set; } = string.Empty;
    public string VerificationResult { get; set; } = string.Empty;
    public DateTime? VerificationDate { get; set; }
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 8D文件更新记录
/// </summary>
public class Complaint8DDocUpdate
{
    public string DocId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string DocType { get; set; } = string.Empty; // SOP / SIP / FMEA / ControlPlan / BOM
    public string DocName { get; set; } = string.Empty;
    public string DocNo { get; set; } = string.Empty;
    public string UpdateDescription { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime? PlanDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 8D附件
/// </summary>
public class Complaint8DAttachment
{
    public string AttachmentId { get; set; } = string.Empty;
    public string ComplaintId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string UploadStage { get; set; } = string.Empty; // D0-D8
    public string UploadedBy { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.Now;
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 客诉统计模型
/// </summary>
public class ComplaintStatistics
{
    public int TotalCount { get; set; }
    public int OpenCount { get; set; }
    public int InProgressCount { get; set; }
    public int ClosedCount { get; set; }
    public int OverdueCount { get; set; }
    public double CloseRate { get; set; }
    public double AvgOpenDays { get; set; }
    public int HighSeverityCount { get; set; }
    public string TopDefectType { get; set; } = string.Empty;
    public Dictionary<string, int> DefectTypeDistribution { get; set; } = new();
    public Dictionary<string, int> StatusDistribution { get; set; } = new();
    public Dictionary<string, int> EightDStepDistribution { get; set; } = new();
}

/// <summary>
/// 客户质量报告
/// </summary>
public class CustomerQualityReport
{
    public string ReportId { get; set; } = string.Empty;
    public string ComplaintNo { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; } = DateTime.Now;
    public string ReportType { get; set; } = "8D";
    public string Summary { get; set; } = string.Empty;
    public string Findings { get; set; } = string.Empty;
    public string CorrectiveActions { get; set; } = string.Empty;
    public string PreventiveActions { get; set; } = string.Empty;
    public string Conclusion { get; set; } = string.Empty;
}

/// <summary>
/// 缺陷类型分布项
/// </summary>
public class DistributionItem : BindableBase
{
    private string _label = string.Empty;
    private int _count;
    private double _percentage;

    public string Label { get => _label; set => SetProperty(ref _label, value); }
    public int Count { get => _count; set => SetProperty(ref _count, value); }
    public double Percentage { get => _percentage; set => SetProperty(ref _percentage, value); }
}

/// <summary>
/// 缺陷类型统计项
/// </summary>
public class DefectTypeStatItem
{
    public string DefectType { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public string AffectedCustomers { get; set; } = string.Empty;
    public string AffectedProducts { get; set; } = string.Empty;
}
