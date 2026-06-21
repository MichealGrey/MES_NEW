using MES.Contracts.Common;

namespace MES.Contracts.Phase1;

// ============================================================================
// 设备故障/维护 DTO
// ============================================================================

/// <summary>
/// 报告设备故障请求
/// </summary>
public class ReportEquipmentFaultRequest
{
    /// <summary>设备ID</summary>
    public string EquipmentId { get; set; } = string.Empty;

    /// <summary>故障类型: Mechanical/Electrical/Software/Other</summary>
    public string FaultType { get; set; } = string.Empty;

    /// <summary>严重程度: Critical/Major/Minor</summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>故障描述</summary>
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
/// 设备故障响应
/// </summary>
public class EquipmentFaultResponse
{
    /// <summary>故障ID</summary>
    public string FaultId { get; set; } = string.Empty;

    /// <summary>设备ID</summary>
    public string EquipmentId { get; set; } = string.Empty;

    /// <summary>设备名称</summary>
    public string EquipmentName { get; set; } = string.Empty;

    /// <summary>故障类型: Mechanical/Electrical/Software/Other</summary>
    public string FaultType { get; set; } = string.Empty;

    /// <summary>严重程度: Critical/Major/Minor</summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>状态: Reported/Dispatched/InRepair/Completed/Verified</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>报告时间</summary>
    public DateTime ReportedAt { get; set; }

    /// <summary>完成时间</summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>维修时长(分钟)</summary>
    public int? RepairDurationMinutes { get; set; }
}

/// <summary>
/// 派修请求
/// </summary>
public class DispatchFaultRequest
{
    /// <summary>故障ID</summary>
    public string FaultId { get; set; } = string.Empty;

    /// <summary>指派人ID</summary>
    public string AssigneeId { get; set; } = string.Empty;

    /// <summary>指派人姓名</summary>
    public string AssigneeName { get; set; } = string.Empty;

    /// <summary>优先级</summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 完成维修请求
/// </summary>
public class CompleteRepairRequest
{
    /// <summary>故障ID</summary>
    public string FaultId { get; set; } = string.Empty;

    /// <summary>根本原因</summary>
    public string? RootCause { get; set; }

    /// <summary>维修操作</summary>
    public string? RepairAction { get; set; }

    /// <summary>使用的备件列表</summary>
    public string[]? SparePartsUsed { get; set; }

    /// <summary>完成人</summary>
    public string CompletedBy { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// PM计划响应
/// </summary>
public class PmPlanResponse
{
    /// <summary>计划ID</summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>设备ID</summary>
    public string EquipmentId { get; set; } = string.Empty;

    /// <summary>设备名称</summary>
    public string EquipmentName { get; set; } = string.Empty;

    /// <summary>PM类型: Daily/Weekly/Monthly/Quarterly/Annual</summary>
    public string PmType { get; set; } = string.Empty;

    /// <summary>描述</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>计划日期</summary>
    public DateTime PlannedDate { get; set; }

    /// <summary>实际日期</summary>
    public DateTime? ActualDate { get; set; }

    /// <summary>状态: Planned/InProgress/Completed/Overdue/Skipped</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>执行人</summary>
    public string? ExecutedBy { get; set; }

    /// <summary>结果</summary>
    public string? Result { get; set; }
}

/// <summary>
/// 创建PM计划请求
/// </summary>
public class CreatePmPlanRequest
{
    /// <summary>设备ID</summary>
    public string EquipmentId { get; set; } = string.Empty;

    /// <summary>PM类型: Daily/Weekly/Monthly/Quarterly/Annual</summary>
    public string PmType { get; set; } = string.Empty;

    /// <summary>描述</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>计划日期</summary>
    public DateTime PlannedDate { get; set; }

    /// <summary>检查项目列表</summary>
    public List<string>? CheckItems { get; set; }
}

/// <summary>
/// 执行PM请求
/// </summary>
public class ExecutePmRequest
{
    /// <summary>计划ID</summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>执行人</summary>
    public string ExecutedBy { get; set; } = string.Empty;

    /// <summary>结果: Pass/Fail/Partial</summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>发现项</summary>
    public string? Findings { get; set; }

    /// <summary>检查结果列表</summary>
    public List<PmCheckResult> CheckResults { get; set; } = [];
}

/// <summary>
/// PM检查结果
/// </summary>
public class PmCheckResult
{
    /// <summary>项目代码</summary>
    public string ItemCode { get; set; } = string.Empty;

    /// <summary>项目名称</summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>标准</summary>
    public string? Standard { get; set; }

    /// <summary>实际值</summary>
    public string? ActualValue { get; set; }

    /// <summary>结果: Pass/Fail/NA</summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// MTBF/MTTR响应
/// </summary>
public class MtbfMttrResponse
{
    /// <summary>设备ID</summary>
    public string EquipmentId { get; set; } = string.Empty;

    /// <summary>设备名称</summary>
    public string EquipmentName { get; set; } = string.Empty;

    /// <summary>MTBF(小时)</summary>
    public decimal MtbfHours { get; set; }

    /// <summary>MTTR(小时)</summary>
    public decimal MttrHours { get; set; }

    /// <summary>总故障次数</summary>
    public int TotalFaults { get; set; }

    /// <summary>可用率(%)</summary>
    public decimal AvailabilityPercent { get; set; }

    /// <summary>统计周期: YYYY-MM</summary>
    public string Period { get; set; } = string.Empty;
}

/// <summary>
/// 设备故障查询
/// </summary>
public class EquipmentFaultQuery : PagedQuery
{
    /// <summary>设备ID</summary>
    public string? EquipmentId { get; set; }

    /// <summary>严重程度: Critical/Major/Minor</summary>
    public string? Severity { get; set; }

    /// <summary>状态: Reported/Dispatched/InRepair/Completed/Verified</summary>
    public string? Status { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}

/// <summary>
/// PM计划查询
/// </summary>
public class PmPlanQuery : PagedQuery
{
    /// <summary>设备ID</summary>
    public string? EquipmentId { get; set; }

    /// <summary>PM类型: Daily/Weekly/Monthly/Quarterly/Annual</summary>
    public string? PmType { get; set; }

    /// <summary>状态: Planned/InProgress/Completed/Overdue/Skipped</summary>
    public string? Status { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}
