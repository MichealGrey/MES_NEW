using MES.Contracts.Common;

namespace MES.Contracts.Phase1;

// ============================================================================
// FQC/OQC 检验 DTO
// ============================================================================

/// <summary>
/// FQC任务响应
/// </summary>
public class FqcTaskResponse
{
    /// <summary>任务ID</summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>工单ID</summary>
    public string WorkOrderId { get; set; } = string.Empty;

    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>产品名称</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>数量</summary>
    public int Quantity { get; set; }

    /// <summary>任务状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>判定结果: Pass/Fail/ConditionalPass</summary>
    public string? JudgmentResult { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>完成时间</summary>
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// 执行FQC检验请求
/// </summary>
public class ExecuteFqcRequest
{
    /// <summary>检验项目列表</summary>
    public List<InspectionItemInput> Items { get; set; } = [];

    /// <summary>检验员工号</summary>
    public string InspectorId { get; set; } = string.Empty;

    /// <summary>检验员姓名</summary>
    public string InspectorName { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// OQC任务响应
/// </summary>
public class OqcTaskResponse
{
    /// <summary>任务ID</summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>出货单ID</summary>
    public string? ShipmentId { get; set; }

    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>数量</summary>
    public int Quantity { get; set; }

    /// <summary>任务状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>判定结果: Pass/Fail/ConditionalPass</summary>
    public string? JudgmentResult { get; set; }

    /// <summary>包装检查</summary>
    public string? PackagingCheck { get; set; }

    /// <summary>标签检查</summary>
    public string? LabelCheck { get; set; }

    /// <summary>文档检查</summary>
    public string? DocumentationCheck { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 执行OQC检验请求
/// </summary>
public class ExecuteOqcRequest
{
    /// <summary>检验项目列表</summary>
    public List<InspectionItemInput> Items { get; set; } = [];

    /// <summary>包装检查</summary>
    public string PackagingCheck { get; set; } = string.Empty;

    /// <summary>标签检查</summary>
    public string LabelCheck { get; set; } = string.Empty;

    /// <summary>文档检查</summary>
    public string DocumentationCheck { get; set; } = string.Empty;

    /// <summary>检验员工号</summary>
    public string InspectorId { get; set; } = string.Empty;

    /// <summary>检验员姓名</summary>
    public string InspectorName { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// OQC MSL检查请求
/// </summary>
public class OqcMslCheckRequest
{
    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>物料批次ID</summary>
    public string BatchId { get; set; } = string.Empty;

    /// <summary>计划出货日期</summary>
    public DateTime PlannedShipDate { get; set; }
}

/// <summary>
/// MSL检查结果
/// </summary>
public class MslCheckResult
{
    /// <summary>是否通过</summary>
    public bool Passed { get; set; }

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>MSL等级</summary>
    public string? MslLevel { get; set; }

    /// <summary>MSL暴露开始时间</summary>
    public DateTime? MslExposureStart { get; set; }

    /// <summary>MSL到期时间</summary>
    public DateTime? MslExpiry { get; set; }

    /// <summary>剩余地板寿命(小时)</summary>
    public decimal RemainingFloorLifeHours { get; set; }

    /// <summary>结果: Pass/Fail/Expired</summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>失败原因</summary>
    public string? FailureReason { get; set; }
}

/// <summary>
/// FQC任务查询
/// </summary>
public class FqcTaskQuery : PagedQuery
{
    /// <summary>工单ID</summary>
    public string? WorkOrderId { get; set; }

    /// <summary>产品ID</summary>
    public string? ProductId { get; set; }

    /// <summary>任务状态</summary>
    public string? Status { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}

/// <summary>
/// OQC任务查询
/// </summary>
public class OqcTaskQuery : PagedQuery
{
    /// <summary>产品ID</summary>
    public string? ProductId { get; set; }

    /// <summary>任务状态</summary>
    public string? Status { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}
