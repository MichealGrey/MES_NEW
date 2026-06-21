using MES.Contracts.Common;

namespace MES.Contracts.Phase1;

// ============================================================================
// 不合格品/MRB DTO
// ============================================================================

/// <summary>
/// 创建不合格品记录请求
/// </summary>
public class CreateNonconformingRequest
{
    /// <summary>来源: IQC/FQC/OQC/Process/Audit</summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>来源引用</summary>
    public string SourceReference { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>物料批次ID</summary>
    public string? BatchId { get; set; }

    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>缺陷代码</summary>
    public string DefectCode { get; set; } = string.Empty;

    /// <summary>缺陷描述</summary>
    public string DefectDescription { get; set; } = string.Empty;

    /// <summary>缺陷类别: Critical/Major/Minor</summary>
    public string DefectCategory { get; set; } = string.Empty;

    /// <summary>受影响数量</summary>
    public int AffectedQty { get; set; }

    /// <summary>报告人工号</summary>
    public string ReportedBy { get; set; } = string.Empty;

    /// <summary>报告人姓名</summary>
    public string ReportedByName { get; set; } = string.Empty;

    /// <summary>备注</summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 不合格品记录响应
/// </summary>
public class NonconformingRecordResponse
{
    /// <summary>不合格品ID</summary>
    public string NcrId { get; set; } = string.Empty;

    /// <summary>来源: IQC/FQC/OQC/Process/Audit</summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>缺陷代码</summary>
    public string DefectCode { get; set; } = string.Empty;

    /// <summary>缺陷类别: Critical/Major/Minor</summary>
    public string DefectCategory { get; set; } = string.Empty;

    /// <summary>受影响数量</summary>
    public int AffectedQty { get; set; }

    /// <summary>隔离状态</summary>
    public string IsolationStatus { get; set; } = string.Empty;

    /// <summary>MRB状态</summary>
    public string MrbStatus { get; set; } = string.Empty;

    /// <summary>处置方式</summary>
    public string? Disposition { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// MRB评审响应
/// </summary>
public class MrbReviewResponse
{
    /// <summary>MRB评审ID</summary>
    public string MrbId { get; set; } = string.Empty;

    /// <summary>不合格品ID</summary>
    public string NcrId { get; set; } = string.Empty;

    /// <summary>批次ID</summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>产品ID</summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>受影响数量</summary>
    public int AffectedQty { get; set; }

    /// <summary>评审状态</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>质量投票</summary>
    public string? QualityVote { get; set; }

    /// <summary>工艺投票</summary>
    public string? ProcessVote { get; set; }

    /// <summary>工程投票</summary>
    public string? EngineeringVote { get; set; }

    /// <summary>最终处置方式</summary>
    public string? FinalDisposition { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// MRB投票请求
/// </summary>
public class MrbVoteRequest
{
    /// <summary>投票人工号</summary>
    public string VoterId { get; set; } = string.Empty;

    /// <summary>投票人角色: Quality/Process/Engineering</summary>
    public string VoterRole { get; set; } = string.Empty;

    /// <summary>投票: Approve/Reject/ConditionalApprove</summary>
    public string Vote { get; set; } = string.Empty;

    /// <summary>处置建议</summary>
    public string? DispositionRecommendation { get; set; }

    /// <summary>说明</summary>
    public string? Comment { get; set; }
}

/// <summary>
/// 处置请求
/// </summary>
public class DispositionRequest
{
    /// <summary>处置方式: Rework/Scrap/Concession/Return/UseAsIs</summary>
    public string Disposition { get; set; } = string.Empty;

    /// <summary>处置详情</summary>
    public string DispositionDetail { get; set; } = string.Empty;

    /// <summary>审批人</summary>
    public string ApprovedBy { get; set; } = string.Empty;

    /// <summary>审批说明</summary>
    public string? ApprovalComment { get; set; }
}

/// <summary>
/// 不合格品查询
/// </summary>
public class NonconformingQuery : PagedQuery
{
    /// <summary>来源: IQC/FQC/OQC/Process/Audit</summary>
    public string? Source { get; set; }

    /// <summary>状态</summary>
    public string? Status { get; set; }

    /// <summary>批次ID</summary>
    public string? LotId { get; set; }

    /// <summary>缺陷类别: Critical/Major/Minor</summary>
    public string? DefectCategory { get; set; }

    /// <summary>起始日期</summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>截止日期</summary>
    public DateTime? DateTo { get; set; }
}
