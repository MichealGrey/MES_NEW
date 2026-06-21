namespace MES.Contracts.Quality;

// ============================================================================
// 质量检验 DTO
// ============================================================================

/// <summary>
/// 检验创建请求
/// </summary>
public class InspectionCreateRequest
{
    /// <summary>批次号</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>工序代码</summary>
    public string StepCode { get; set; } = string.Empty;

    /// <summary>检验类型: IQC/IPQC/OQC</summary>
    public string InspectionType { get; set; } = string.Empty;

    /// <summary>检验员工号</summary>
    public string InspectorId { get; set; } = string.Empty;

    /// <summary>检验项目列表</summary>
    public List<InspectionItemDto> Items { get; set; } = [];

    /// <summary>备注</summary>
    public string? Remarks { get; set; }

    /// <summary>不合格时是否自动创建NCR</summary>
    public bool AutoCreateNcr { get; set; }
}

/// <summary>
/// 检验项目
/// </summary>
public class InspectionItemDto
{
    /// <summary>项目代码</summary>
    public string ItemCode { get; set; } = string.Empty;

    /// <summary>项目名称</summary>
    public string ItemName { get; set; } = string.Empty;

    /// <summary>规格要求</summary>
    public string? Specification { get; set; }

    /// <summary>实测值</summary>
    public decimal? MeasuredValue { get; set; }

    /// <summary>判定结果: Pass/Fail</summary>
    public string Result { get; set; } = "Pass";

    /// <summary>单位</summary>
    public string? Unit { get; set; }
}

/// <summary>
/// 检验响应
/// </summary>
public class InspectionResponse
{
    public string InspectionId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Inspector { get; set; }
    public DateTime InspectionTime { get; set; }
    public string Result { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public List<InspectionItemDto> Items { get; set; } = [];
    public string? NcrId { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 检验列表响应（分页）
/// </summary>
public class InspectionListResponse
{
    public List<InspectionSummaryDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// 检验摘要（列表用）
/// </summary>
public class InspectionSummaryDto
{
    public string InspectionId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Inspector { get; set; }
    public DateTime InspectionTime { get; set; }
    public string Result { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
}

// ============================================================================
// 不合格品报告（NCR）DTO
// ============================================================================

/// <summary>
/// NCR创建请求
/// </summary>
public class NcrCreateRequest
{
    /// <summary>批次号</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>工序代码</summary>
    public string StepCode { get; set; } = string.Empty;

    /// <summary>缺陷类型</summary>
    public string DefectType { get; set; } = string.Empty;

    /// <summary>缺陷描述</summary>
    public string? DefectDescription { get; set; }

    /// <summary>不合格数量</summary>
    public int Quantity { get; set; }

    /// <summary>严重程度: Critical/Major/Minor</summary>
    public string Severity { get; set; } = "Minor";

    /// <summary>发现人工号</summary>
    public string DiscovererId { get; set; } = string.Empty;
}

/// <summary>
/// NCR响应
/// </summary>
public class NcrResponse
{
    public string NcrId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string DefectType { get; set; } = string.Empty;
    public string? DefectDescription { get; set; }
    public int Quantity { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Disposition { get; set; }
    public string? Discoverer { get; set; }
    public DateTime DiscoveredTime { get; set; }
    public string? Reviewer { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// NCR列表响应（分页）
/// </summary>
public class NcrListResponse
{
    public List<NcrResponse> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// NCR状态更新请求
/// </summary>
public class NcrStatusUpdateRequest
{
    /// <summary>新状态: UnderReview/Dispositioned/Closed</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>处置方式: Rework/Scrap/Return/UseAsIs</summary>
    public string? Disposition { get; set; }

    /// <summary>关闭备注</summary>
    public string? ClosureComment { get; set; }
}

// ============================================================================
// 质量统计 DTO
// ============================================================================

/// <summary>
/// 质量统计结果
/// </summary>
public class QualityStatsResponse
{
    /// <summary>总检验次数</summary>
    public int TotalInspections { get; set; }

    /// <summary>合格次数</summary>
    public int PassedInspections { get; set; }

    /// <summary>不合格次数</summary>
    public int FailedInspections { get; set; }

    /// <summary>条件合格次数</summary>
    public int ConditionalInspections { get; set; }

    /// <summary>合格率</summary>
    public decimal PassRate { get; set; }

    /// <summary>按检验类型统计</summary>
    public List<TypeStatItem> StatsByType { get; set; } = [];

    /// <summary>按缺陷类型统计</summary>
    public List<DefectStatItem> StatsByDefectType { get; set; } = [];

    /// <summary>NCR按状态统计</summary>
    public List<StatusStatItem> NcrStatsByStatus { get; set; } = [];

    /// <summary>近期趋势（按日）</summary>
    public List<TrendItem> DailyTrend { get; set; } = [];
}

/// <summary>
/// 类型统计项
/// </summary>
public class TypeStatItem
{
    public string InspectionType { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Passed { get; set; }
    public int Failed { get; set; }
    public decimal PassRate { get; set; }
}

/// <summary>
/// 缺陷类型统计项
/// </summary>
public class DefectStatItem
{
    public string DefectType { get; set; } = string.Empty;
    public int Count { get; set; }
    public int TotalQuantity { get; set; }
}

/// <summary>
/// 状态统计项
/// </summary>
public class StatusStatItem
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// 趋势项
/// </summary>
public class TrendItem
{
    public DateTime Date { get; set; }
    public int Inspections { get; set; }
    public int Passed { get; set; }
    public int Failed { get; set; }
    public decimal PassRate { get; set; }
    public int NcrCount { get; set; }
}
