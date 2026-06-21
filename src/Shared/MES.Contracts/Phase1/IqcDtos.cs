using MES.Contracts.Common;

namespace MES.Contracts.Phase1;

// ============================================================================
// IQC 来料检验 DTO
// ============================================================================

/// <summary>
/// 创建IQC检验任务请求
/// </summary>
public class CreateIqcTaskRequest
{
    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>物料名称</summary>
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>供应商ID</summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>供应商名称</summary>
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>供应商批次号</summary>
    public string SupplierBatchNo { get; set; } = string.Empty;

    /// <summary>数量</summary>
    public int Quantity { get; set; }

    /// <summary>单位</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>采购订单号</summary>
    public string PurchaseOrderNo { get; set; } = string.Empty;

    /// <summary>生产日期</summary>
    public DateTime? ManufacturingDate { get; set; }

    /// <summary>有效期</summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>MSL等级</summary>
    public string? MslLevel { get; set; }

    /// <summary>MSL地板寿命(小时)</summary>
    public decimal? MslFloorLifeHours { get; set; }

    /// <summary>COA参考号</summary>
    public string? CoaReference { get; set; }

    /// <summary>收货人</summary>
    public string ReceivedBy { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// IQC任务响应
/// </summary>
public class IqcTaskResponse
{
    /// <summary>任务ID</summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string BatchId { get; set; } = string.Empty;

    /// <summary>物料ID</summary>
    public string MaterialId { get; set; } = string.Empty;

    /// <summary>物料名称</summary>
    public string MaterialName { get; set; } = string.Empty;

    /// <summary>供应商ID</summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>供应商名称</summary>
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>供应商批次号</summary>
    public string SupplierBatchNo { get; set; } = string.Empty;

    /// <summary>数量</summary>
    public int Quantity { get; set; }

    /// <summary>任务状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>检验状态</summary>
    public string InspectionStatus { get; set; } = string.Empty;

    /// <summary>判定结果: Pass/Fail/ConditionalPass</summary>
    public string? JudgmentResult { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// IQC任务详情响应(含检验项目列表)
/// </summary>
public class IqcTaskDetailResponse : IqcTaskResponse
{
    /// <summary>检验项目列表</summary>
    public List<InspectionItemInput> Items { get; set; } = [];
}

/// <summary>
/// 执行检验请求
/// </summary>
public class ExecuteInspectionRequest
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
/// 检验项目输入
/// </summary>
public class InspectionItemInput
{
    /// <summary>项目代码</summary>
    public string ItemCode { get; set; } = string.Empty;

    /// <summary>项目名称</summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>标准值</summary>
    public string? StandardValue { get; set; }

    /// <summary>实测值</summary>
    public string? ActualValue { get; set; }

    /// <summary>下限</summary>
    public decimal? LowerLimit { get; set; }

    /// <summary>上限</summary>
    public decimal? UpperLimit { get; set; }

    /// <summary>结果: Pass/Fail</summary>
    public string Result { get; set; } = string.Empty;

    /// <summary>单位</summary>
    public string? Unit { get; set; }

    /// <summary>测量设备</summary>
    public string? MeasuringEquipment { get; set; }
}

/// <summary>
/// 判定请求
/// </summary>
public class JudgeRequest
{
    /// <summary>判定结果: Pass/Fail/ConditionalPass</summary>
    public string Judgment { get; set; } = string.Empty;

    /// <summary>处置方式: Accept/Reject/Return/Concession</summary>
    public string Disposition { get; set; } = string.Empty;

    /// <summary>判定人</summary>
    public string JudgeBy { get; set; } = string.Empty;

    /// <summary>判定说明</summary>
    public string? JudgeComment { get; set; }
}

/// <summary>
/// IQC统计响应
/// </summary>
public class IqcStatisticsResponse
{
    /// <summary>总批次</summary>
    public int TotalBatches { get; set; }

    /// <summary>合格批次</summary>
    public int PassedBatches { get; set; }

    /// <summary>不合格批次</summary>
    public int FailedBatches { get; set; }

    /// <summary>合格率</summary>
    public decimal PassRate { get; set; }

    /// <summary>供应商统计</summary>
    public List<SupplierStatItem> SupplierStats { get; set; } = [];
}

/// <summary>
/// 供应商统计项
/// </summary>
public class SupplierStatItem
{
    /// <summary>供应商ID</summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>供应商名称</summary>
    public string SupplierName { get; set; } = string.Empty;

    /// <summary>总批次</summary>
    public int TotalBatches { get; set; }

    /// <summary>合格批次</summary>
    public int PassedBatches { get; set; }

    /// <summary>合格率</summary>
    public decimal PassRate { get; set; }
}

/// <summary>
/// IQC任务查询
/// </summary>
public class IqcTaskQuery : PagedQuery
{
    /// <summary>任务状态</summary>
    public string? Status { get; set; }

    /// <summary>物料ID</summary>
    public string? MaterialId { get; set; }

    /// <summary>供应商ID</summary>
    public string? SupplierId { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}

/// <summary>
/// IQC统计查询
/// </summary>
public class IqcStatisticsQuery
{
    /// <summary>供应商ID</summary>
    public string? SupplierId { get; set; }

    /// <summary>物料ID</summary>
    public string? MaterialId { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}

/// <summary>
/// 检验结果响应
/// </summary>
public class InspectionResultResponse
{
    /// <summary>任务ID</summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>检验结果记录ID列表</summary>
    public List<string> ResultIds { get; set; } = [];

    /// <summary>检验是否完成</summary>
    public bool IsComplete { get; set; }

    /// <summary>检验项目数</summary>
    public int ItemCount { get; set; }

    /// <summary>合格项目数</summary>
    public int PassCount { get; set; }

    /// <summary>不合格项目数</summary>
    public int FailCount { get; set; }
}
