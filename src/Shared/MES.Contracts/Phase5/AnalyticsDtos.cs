namespace MES.Contracts.Phase5;

// ============================================================================
// KPI Dashboard DTOs
// ============================================================================

public class KpiMetricResponse
{
    public string SnapshotId { get; set; } = string.Empty;
    public DateTime SnapshotDate { get; set; }
    public string MetricCode { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public decimal? MetricValue { get; set; }
    public decimal? TargetValue { get; set; }
    public string? Unit { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Trend { get; set; }
    public string PeriodType { get; set; } = string.Empty;
    public string? DetailData { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class KpiDashboardResponse
{
    public List<KpiMetricResponse> Metrics { get; set; } = [];
    public DateTime? LastUpdated { get; set; }
    public string OverallStatus { get; set; } = string.Empty;
}

public class KpiMetricQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? MetricCode { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? PeriodType { get; set; }
}

public class KpiCaptureRequest
{
    public string MetricCode { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public decimal MetricValue { get; set; }
    public decimal? TargetValue { get; set; }
    public string? Unit { get; set; }
    public string? Trend { get; set; }
    public string PeriodType { get; set; } = "Daily";
    public string? DetailData { get; set; }
}

// ============================================================================
// Cost DTOs
// ============================================================================

public class CostRecordResponse
{
    public string CostId { get; set; } = string.Empty;
    public string WorkOrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public DateOnly CostDate { get; set; }
    public string CostType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? DetailJson { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CostAnalysisResponse
{
    public string WorkOrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public decimal DirectMaterialCost { get; set; }
    public decimal DirectLaborCost { get; set; }
    public decimal ManufacturingOverhead { get; set; }
    public decimal TotalCost { get; set; }
    public int CompletedQty { get; set; }
    public decimal UnitCost { get; set; }
    public List<CostRecordResponse> CostRecords { get; set; } = [];
}

public class CostQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? WorkOrderId { get; set; }
    public string? ProductId { get; set; }
    public string? CostType { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class CostCalculationRequest
{
    public string WorkOrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public decimal DirectMaterialCost { get; set; }
    public decimal DirectLaborCost { get; set; }
    public decimal ManufacturingOverhead { get; set; }
    public int CompletedQty { get; set; }
}

// ============================================================================
// Yield DTOs
// ============================================================================

public class YieldStatisticsResponse
{
    public string StatId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string? StepCode { get; set; }
    public string? StepName { get; set; }
    public int InputQty { get; set; }
    public int OutputQty { get; set; }
    public decimal? YieldRate { get; set; }
    public int ScrapQty { get; set; }
    public int ReworkQty { get; set; }
    public string? DefectJson { get; set; }
    public DateOnly StatDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class YieldTrendResponse
{
    public string LotId { get; set; } = string.Empty;
    public List<YieldPoint> Points { get; set; } = [];
}

public class YieldPoint
{
    public string? StepCode { get; set; }
    public string? StepName { get; set; }
    public decimal YieldRate { get; set; }
    public int InputQty { get; set; }
    public int OutputQty { get; set; }
    public DateOnly StatDate { get; set; }
}

public class YieldParetoResponse
{
    public List<ParetoItem> Items { get; set; } = [];
}

public class ParetoItem
{
    public string DefectType { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
    public decimal CumulativePercentage { get; set; }
}

public class YieldQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? LotId { get; set; }
    public string? StepCode { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class DppmResponse
{
    public string ProductId { get; set; } = string.Empty;
    public int TotalShipped { get; set; }
    public int DefectCount { get; set; }
    public decimal Dppm { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
}

// ============================================================================
// NPI DTOs
// ============================================================================

public class NpiProjectResponse
{
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string CurrentStage { get; set; } = string.Empty;
    public string? ProjectManager { get; set; }
    public DateOnly? TargetDate { get; set; }
    public DateOnly? ActualDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class NpiStageResponse
{
    public string StageId { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string StageName { get; set; } = string.Empty;
    public int? StageOrder { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Result { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateNpiProjectRequest
{
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string? ProductId { get; set; }
    public string? ProjectManager { get; set; }
    public DateOnly? TargetDate { get; set; }
    public string? Description { get; set; }
}

public class NpiProjectQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Keyword { get; set; }
    public string? Status { get; set; }
    public string? CurrentStage { get; set; }
}

public class TrialRunRequest
{
    public string LotId { get; set; } = string.Empty;
    public int SampleSize { get; set; }
    public string? Remark { get; set; }
}

public class NpiReviewRequest
{
    public string StageId { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string? Comment { get; set; }
}

// ============================================================================
// Reliability Test DTOs
// ============================================================================

public class ReliabilityTestPlanResponse
{
    public string PlanId { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public int? SampleSize { get; set; }
    public int? TestDuration { get; set; }
    public string? TestConditions { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ResultSummary { get; set; }
    public bool FaTriggered { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReliabilityTestPlanRequest
{
    public string PlanName { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public int SampleSize { get; set; }
    public int TestDuration { get; set; }
    public string? TestConditions { get; set; }
}

public class ReliabilityTestQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? TestType { get; set; }
    public string? ProductId { get; set; }
    public string? Status { get; set; }
}

// ============================================================================
// Report DTOs
// ============================================================================

public class ReportScheduleResponse
{
    public string ScheduleId { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string? ScheduleCron { get; set; }
    public string? Recipients { get; set; }
    public string Format { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public DateTime? LastRunDate { get; set; }
    public DateTime? NextRunDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReportScheduleRequest
{
    public string ReportName { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string? ScheduleCron { get; set; }
    public string? Recipients { get; set; }
    public string Format { get; set; } = "PDF";
    public bool Enabled { get; set; } = true;
}

public class ReportScheduleQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportType { get; set; }
    public bool? Enabled { get; set; }
}

// ============================================================================
// System Config DTOs
// ============================================================================

public class SystemConfigResponse
{
    public string ConfigId { get; set; } = string.Empty;
    public string ConfigKey { get; set; } = string.Empty;
    public string? ConfigValue { get; set; }
    public string? ConfigType { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateSystemConfigRequest
{
    public string ConfigKey { get; set; } = string.Empty;
    public string? ConfigValue { get; set; }
    public string? ConfigType { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}

public class SystemConfigQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Category { get; set; }
    public string? Keyword { get; set; }
}

// ============================================================================
// Alert Rule DTOs
// ============================================================================

public class AlertRuleResponse
{
    public string RuleId { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string RuleType { get; set; } = string.Empty;
    public string? ConditionExpression { get; set; }
    public decimal? ThresholdValue { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string? NotificationChannels { get; set; }
    public bool Enabled { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateAlertRuleRequest
{
    public string RuleName { get; set; } = string.Empty;
    public string RuleType { get; set; } = string.Empty;
    public string? ConditionExpression { get; set; }
    public decimal? ThresholdValue { get; set; }
    public string Severity { get; set; } = "Warning";
    public string? NotificationChannels { get; set; }
    public bool Enabled { get; set; } = true;
}

public class AlertRuleQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? RuleType { get; set; }
    public bool? Enabled { get; set; }
}

// ============================================================================
// Data Correction DTOs
// ============================================================================

public class DataCorrectionResponse
{
    public string CorrectionId { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ApprovedBy { get; set; }
    public string CorrectedBy { get; set; } = string.Empty;
    public DateTime CorrectedAt { get; set; }
}

public class CreateDataCorrectionRequest
{
    public string TableName { get; set; } = string.Empty;
    public string RecordId { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ApprovedBy { get; set; }
}

public class DataCorrectionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? TableName { get; set; }
    public string? RecordId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
