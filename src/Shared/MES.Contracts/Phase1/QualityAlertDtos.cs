using MES.Contracts.Common;

namespace MES.Contracts.Phase1;

// ============================================================================
// 质量预警/召回 DTO
// ============================================================================

/// <summary>
/// 创建质量预警请求
/// </summary>
public class CreateQualityAlertRequest
{
    /// <summary>预警类型: MaterialDefect/ProcessDefect/EquipmentIssue/CustomerComplaint</summary>
    public string AlertType { get; set; } = string.Empty;

    /// <summary>严重程度: Critical/High/Medium</summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>标题</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>描述</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>根本原因</summary>
    public string? RootCause { get; set; }

    /// <summary>来源物料ID</summary>
    public string? SourceMaterialId { get; set; }

    /// <summary>来源批次ID</summary>
    public string? SourceBatchId { get; set; }

    /// <summary>来源工单ID</summary>
    public string? SourceWorkOrderId { get; set; }

    /// <summary>来源Lot ID</summary>
    public string? SourceLotId { get; set; }

    /// <summary>发生日期</summary>
    public DateTime? OccurrenceDate { get; set; }

    /// <summary>发布人工号</summary>
    public string IssuedBy { get; set; } = string.Empty;

    /// <summary>发布人姓名</summary>
    public string IssuedByName { get; set; } = string.Empty;

    /// <summary>通知部门列表</summary>
    public string[]? NotifyDepartments { get; set; }

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 质量预警响应
/// </summary>
public class QualityAlertResponse
{
    /// <summary>预警ID</summary>
    public string AlertId { get; set; } = string.Empty;

    /// <summary>预警类型: MaterialDefect/ProcessDefect/EquipmentIssue/CustomerComplaint</summary>
    public string AlertType { get; set; } = string.Empty;

    /// <summary>严重程度: Critical/High/Medium</summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>标题</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>状态: Active/Investigating/Resolved/Closed</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>冻结批次数量</summary>
    public int FrozenLotsCount { get; set; }

    /// <summary>受影响批次数量</summary>
    public int AffectedLotsCount { get; set; }

    /// <summary>发布时间</summary>
    public DateTime IssuedAt { get; set; }

    /// <summary>关闭时间</summary>
    public DateTime? ClosedAt { get; set; }
}

/// <summary>
/// 受影响批次信息
/// </summary>
public class AffectedLotInfo
{
    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>工单ID</summary>
    public string WorkOrderId { get; set; } = string.Empty;

    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>产品名称</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>当前状态</summary>
    public string CurrentStatus { get; set; } = string.Empty;

    /// <summary>当前工序</summary>
    public string CurrentStep { get; set; } = string.Empty;

    /// <summary>受影响数量</summary>
    public int AffectedQty { get; set; }

    /// <summary>位置</summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>追溯关系</summary>
    public string TraceRelation { get; set; } = string.Empty;
}

/// <summary>
/// 召回通知响应
/// </summary>
public class RecallNoticeResponse
{
    /// <summary>召回ID</summary>
    public string RecallId { get; set; } = string.Empty;

    /// <summary>预警ID</summary>
    public string AlertId { get; set; } = string.Empty;

    /// <summary>生成时间</summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>召回项目列表</summary>
    public List<RecallItem> Items { get; set; } = [];

    /// <summary>总受影响数量</summary>
    public int TotalAffectedQty { get; set; }

    /// <summary>总召回数量</summary>
    public int TotalRecalledQty { get; set; }
}

/// <summary>
/// 召回项目
/// </summary>
public class RecallItem
{
    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>客户ID</summary>
    public string? CustomerId { get; set; }

    /// <summary>客户名称</summary>
    public string? CustomerName { get; set; }

    /// <summary>受影响数量</summary>
    public int AffectedQty { get; set; }

    /// <summary>已召回数量</summary>
    public int RecalledQty { get; set; }

    /// <summary>当前位置: WIP/Warehouse/InTransit/CustomerSite</summary>
    public string CurrentLocation { get; set; } = string.Empty;

    /// <summary>召回状态</summary>
    public string RecallStatus { get; set; } = string.Empty;
}

/// <summary>
/// 关闭质量预警请求
/// </summary>
public class CloseQualityAlertRequest
{
    /// <summary>预警ID</summary>
    public string AlertId { get; set; } = string.Empty;

    /// <summary>关闭原因</summary>
    public string? CloseReason { get; set; }

    /// <summary>关闭人工号</summary>
    public string ClosedBy { get; set; } = string.Empty;

    /// <summary>关闭人姓名</summary>
    public string ClosedByName { get; set; } = string.Empty;
}

/// <summary>
/// 质量预警查询
/// </summary>
public class QualityAlertQuery : PagedQuery
{
    /// <summary>预警类型: MaterialDefect/ProcessDefect/EquipmentIssue/CustomerComplaint</summary>
    public string? AlertType { get; set; }

    /// <summary>严重程度: Critical/High/Medium</summary>
    public string? Severity { get; set; }

    /// <summary>状态: Active/Investigating/Resolved/Closed</summary>
    public string? Status { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}
