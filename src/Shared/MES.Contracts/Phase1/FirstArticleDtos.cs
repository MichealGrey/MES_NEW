using MES.Contracts.Common;

namespace MES.Contracts.Phase1;

// ============================================================================
// 首件检验 DTO
// ============================================================================

/// <summary>
/// 首件检验响应
/// </summary>
public class FirstArticleResponse
{
    /// <summary>首件ID</summary>
    public string FaId { get; set; } = string.Empty;

    /// <summary>工单ID</summary>
    public string WorkOrderId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>产品名称</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>工序代码</summary>
    public string StepCode { get; set; } = string.Empty;

    /// <summary>工序名称</summary>
    public string StepName { get; set; } = string.Empty;

    /// <summary>设备ID</summary>
    public string EquipmentId { get; set; } = string.Empty;

    /// <summary>触发原因: LineChange/ShiftStart/ProcessChange/Maintenance/RecipeChange</summary>
    public string TriggerReason { get; set; } = string.Empty;

    /// <summary>状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>技术员ID</summary>
    public string? TechnicianId { get; set; }

    /// <summary>技术员确认时间</summary>
    public DateTime? TechnicianConfirmedAt { get; set; }

    /// <summary>IPQC检验员ID</summary>
    public string? IpqcId { get; set; }

    /// <summary>IPQC确认时间</summary>
    public DateTime? IpqcConfirmedAt { get; set; }

    /// <summary>检验结果</summary>
    public string? Result { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>检验项目列表(仅详情返回)</summary>
    public List<FirstArticleItemInput>? Items { get; set; }

    /// <summary>签名列表(仅详情返回)</summary>
    public object? Signatures { get; set; }
}

/// <summary>
/// 执行首件检验请求
/// </summary>
public class ExecuteFirstArticleRequest
{
    /// <summary>首件ID</summary>
    public string FaId { get; set; } = string.Empty;

    /// <summary>检验项目列表</summary>
    public List<FirstArticleItemInput> Items { get; set; } = [];

    /// <summary>技术员ID</summary>
    public string TechnicianId { get; set; } = string.Empty;

    /// <summary>技术员姓名</summary>
    public string TechnicianName { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 确认首件请求
/// </summary>
public class ConfirmFirstArticleRequest
{
    /// <summary>首件ID</summary>
    public string FaId { get; set; } = string.Empty;

    /// <summary>确认人ID</summary>
    public string ConfirmerId { get; set; } = string.Empty;

    /// <summary>确认人角色: Technician/IPQC</summary>
    public string ConfirmerRole { get; set; } = string.Empty;

    /// <summary>确认: Approve/Reject</summary>
    public string Confirmation { get; set; } = string.Empty;

    /// <summary>说明</summary>
    public string? Comment { get; set; }
}

/// <summary>
/// 驳回首件请求
/// </summary>
public class RejectFirstArticleRequest
{
    /// <summary>首件ID</summary>
    public string FaId { get; set; } = string.Empty;

    /// <summary>驳回原因</summary>
    public string RejectionReason { get; set; } = string.Empty;

    /// <summary>驳回人</summary>
    public string RejectedBy { get; set; } = string.Empty;
}

/// <summary>
/// 拉力测试请求
/// </summary>
public class BondPullTestRequest
{
    /// <summary>首件ID</summary>
    public string? FaId { get; set; }

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>工单ID</summary>
    public string? WorkOrderId { get; set; }

    /// <summary>样品编号</summary>
    public int SampleNo { get; set; }

    /// <summary>拉力值(克)</summary>
    public decimal PullForceGrams { get; set; }

    /// <summary>下限值(克)</summary>
    public decimal? LowerLimitGrams { get; set; }

    /// <summary>上限值(克)</summary>
    public decimal? UpperLimitGrams { get; set; }

    /// <summary>失效模式</summary>
    public string? FailureMode { get; set; }

    /// <summary>测试人</summary>
    public string TestedBy { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 首件检验查询
/// </summary>
public class FirstArticleQuery : PagedQuery
{
    /// <summary>工单ID</summary>
    public string? WorkOrderId { get; set; }

    /// <summary>状态</summary>
    public string? Status { get; set; }

    /// <summary>触发原因: LineChange/ShiftStart/ProcessChange/Maintenance/RecipeChange</summary>
    public string? TriggerReason { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}

/// <summary>
/// 创建首件检验请求
/// </summary>
public class CreateFirstArticleRequest
{
    /// <summary>工单ID</summary>
    public string WorkOrderId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>产品名称</summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>工序代码</summary>
    public string StepCode { get; set; } = string.Empty;

    /// <summary>工序名称</summary>
    public string? StepName { get; set; }

    /// <summary>设备ID</summary>
    public string? EquipmentId { get; set; }

    /// <summary>触发原因: LineChange/ShiftStart/ProcessChange/Maintenance/RecipeChange</summary>
    public string TriggerReason { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 拉力测试响应
/// </summary>
public class BondPullTestResponse
{
    /// <summary>测试ID</summary>
    public string TestId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>产品名称</summary>
    public string? ProductName { get; set; }

    /// <summary>样品编号</summary>
    public int SampleNo { get; set; }

    /// <summary>拉力值(克)</summary>
    public decimal PullForceGrams { get; set; }

    /// <summary>下限值(克)</summary>
    public decimal? LowerLimitGrams { get; set; }

    /// <summary>上限值(克)</summary>
    public decimal? UpperLimitGrams { get; set; }

    /// <summary>结果: Pass/Fail</summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>失效模式</summary>
    public string? FailureMode { get; set; }

    /// <summary>测试时间</summary>
    public DateTime TestedAt { get; set; }
}

/// <summary>
/// 首件检验项目输入
/// </summary>
public class FirstArticleItemInput
{
    /// <summary>项目代码</summary>
    public string ItemCode { get; set; } = string.Empty;

    /// <summary>项目名称</summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>项目类型</summary>
    public string ItemTyp { get; set; } = string.Empty;

    /// <summary>标准值</summary>
    public string? StandardValue { get; set; }

    /// <summary>下限</summary>
    public decimal? LowerLimit { get; set; }

    /// <summary>上限</summary>
    public decimal? UpperLimit { get; set; }

    /// <summary>实测值</summary>
    public string? ActualValue { get; set; }

    /// <summary>单位</summary>
    public string? Unit { get; set; }

    /// <summary>结果</summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}
