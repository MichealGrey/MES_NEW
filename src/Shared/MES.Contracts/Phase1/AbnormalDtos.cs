using MES.Contracts.Common;

namespace MES.Contracts.Phase1;

// ============================================================================
// 异常/停线 DTO
// ============================================================================

/// <summary>
/// 报告异常请求
/// </summary>
public class ReportAbnormalRequest
{
    /// <summary>异常类型: Equipment/Quality/Material/Process/Safety</summary>
    public string AbnormalType { get; set; } = string.Empty;

    /// <summary>严重程度: Critical/Major/Minor</summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>工单ID</summary>
    public string? WorkOrderId { get; set; }

    /// <summary>批次ID</summary>
    public string? LotId { get; set; }

    /// <summary>工序代码</summary>
    public string? StepCode { get; set; }

    /// <summary>设备ID</summary>
    public string? EquipmentId { get; set; }

    /// <summary>异常描述</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>报告人工号</summary>
    public string ReportedBy { get; set; } = string.Empty;

    /// <summary>报告人姓名</summary>
    public string ReportedByName { get; set; } = string.Empty;

    /// <summary>照片URL列表</summary>
    public string[]? PhotoUrls { get; set; }

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 异常记录响应
/// </summary>
public class AbnormalRecordResponse
{
    /// <summary>异常ID</summary>
    public string AbnormalId { get; set; } = string.Empty;

    /// <summary>异常类型: Equipment/Quality/Material/Process/Safety</summary>
    public string AbnormalType { get; set; } = string.Empty;

    /// <summary>严重程度: Critical/Major/Minor</summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>异常描述</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>是否停线</summary>
    public bool IsLineStopped { get; set; }

    /// <summary>报告时间</summary>
    public DateTime ReportedAt { get; set; }

    /// <summary>处理时间</summary>
    public DateTime? HandledAt { get; set; }

    /// <summary>关闭时间</summary>
    public DateTime? ClosedAt { get; set; }
}

/// <summary>
/// 停线请求
/// </summary>
public class LineStopRequest
{
    /// <summary>异常ID</summary>
    public string AbnormalId { get; set; } = string.Empty;

    /// <summary>停线范围: Line/Equipment/WorkOrder/Lot</summary>
    public string StopScope { get; set; } = string.Empty;

    /// <summary>停线目标ID</summary>
    public string StopTargetId { get; set; } = string.Empty;

    /// <summary>停线原因</summary>
    public string StopReason { get; set; } = string.Empty;

    /// <summary>发令人</summary>
    public string IssuedBy { get; set; } = string.Empty;
}

/// <summary>
/// 停线响应
/// </summary>
public class LineStopResponse
{
    /// <summary>停线ID</summary>
    public string StopId { get; set; } = string.Empty;

    /// <summary>异常ID</summary>
    public string AbnormalId { get; set; } = string.Empty;

    /// <summary>停线范围: Line/Equipment/WorkOrder/Lot</summary>
    public string StopScope { get; set; } = string.Empty;

    /// <summary>停线目标ID</summary>
    public string StopTargetId { get; set; } = string.Empty;

    /// <summary>状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>发出时间</summary>
    public DateTime IssuedAt { get; set; }

    /// <summary>恢复时间</summary>
    public DateTime? ResumeAt { get; set; }
}

/// <summary>
/// 处理异常请求
/// </summary>
public class HandleAbnormalRequest
{
    /// <summary>异常ID</summary>
    public string AbnormalId { get; set; } = string.Empty;

    /// <summary>根本原因</summary>
    public string? RootCause { get; set; }

    /// <summary>纠正措施</summary>
    public string? CorrectiveAction { get; set; }

    /// <summary>预防措施</summary>
    public string? PreventiveAction { get; set; }

    /// <summary>处理人</summary>
    public string HandledBy { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 验证异常请求
/// </summary>
public class VerifyAbnormalRequest
{
    /// <summary>异常ID</summary>
    public string AbnormalId { get; set; } = string.Empty;

    /// <summary>验证结果: Pass/Fail</summary>
    public string VerifyResult { get; set; } = string.Empty;

    /// <summary>验证人</summary>
    public string VerifiedBy { get; set; } = string.Empty;

    /// <summary>说明</summary>
    public string? Comment { get; set; }
}

/// <summary>
/// 异常查询
/// </summary>
public class AbnormalQuery : PagedQuery
{
    /// <summary>异常类型: Equipment/Quality/Material/Process/Safety</summary>
    public string? AbnormalType { get; set; }

    /// <summary>严重程度: Critical/Major/Minor</summary>
    public string? Severity { get; set; }

    /// <summary>状态</summary>
    public string? Status { get; set; }

    /// <summary>工单ID</summary>
    public string? WorkOrderId { get; set; }

    /// <summary>设备ID</summary>
    public string? EquipmentId { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}

/// <summary>
/// 异常统计响应
/// </summary>
public class AbnormalStatisticsResponse
{
    /// <summary>总数量</summary>
    public int TotalCount { get; set; }

    /// <summary>按类型统计</summary>
    public Dictionary<string, int> ByType { get; set; } = [];

    /// <summary>按严重程度统计</summary>
    public Dictionary<string, int> BySeverity { get; set; } = [];

    /// <summary>平均处理时间(分钟)</summary>
    public decimal AvgHandleTime { get; set; }

    /// <summary>未关闭数量</summary>
    public int OpenCount { get; set; }
}
