using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES.Infrastructure.Persistence.Entities;

// ============================================================================
// Phase 5 - Analytics, NPI, and Audit Enhancement Entities
// V4.8.0 - Analytics and NPI Module
// V4.9.0 - Audit Trail Enhancement
// ============================================================================

// ============================================================================
// KPI Dashboard Snapshot
// ============================================================================

/// <summary>
/// KPI看板快照表 - KPI dashboard snapshot records for tracking metrics over time
/// </summary>
[Table("kpi_dashboard_snapshot")]
public class KpiDashboardSnapshot
{
    [Key]
    [Column("snapshot_id")]
    [MaxLength(50)]
    public string SnapshotId { get; set; } = string.Empty;

    [Column("snapshot_date")]
    public DateTime SnapshotDate { get; set; }

    [Required]
    [Column("metric_code")]
    [MaxLength(50)]
    public string MetricCode { get; set; } = string.Empty; // YieldRate/OTD/OEE/DPPM/OutputRate/CustomerComplaint/WIPCount

    [Required]
    [Column("metric_name")]
    [MaxLength(200)]
    public string MetricName { get; set; } = string.Empty;

    [Column("metric_value")]
    public decimal? MetricValue { get; set; }

    [Column("target_value")]
    public decimal? TargetValue { get; set; }

    [Column("unit")]
    [MaxLength(20)]
    public string? Unit { get; set; } // %/ppm/pcs

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "Normal"; // Normal/Warning/Critical

    [Column("trend")]
    [MaxLength(20)]
    public string? Trend { get; set; } // Up/Down/Stable

    [Column("period_type")]
    [MaxLength(20)]
    public string PeriodType { get; set; } = "Daily"; // Hourly/Daily/Weekly/Monthly

    [Column("detail_data")]
    public string? DetailData { get; set; } // JSON

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================================
// Cost Record
// ============================================================================

/// <summary>
/// 成本记录表 - Cost records for work orders
/// </summary>
[Table("cost_record")]
public class CostRecord
{
    [Key]
    [Column("cost_id")]
    [MaxLength(50)]
    public string CostId { get; set; } = string.Empty;

    [Required]
    [Column("work_order_id")]
    [MaxLength(50)]
    public string WorkOrderId { get; set; } = string.Empty;

    [Required]
    [Column("product_id")]
    [MaxLength(50)]
    public string ProductId { get; set; } = string.Empty;

    [Required]
    [Column("cost_date")]
    public DateOnly CostDate { get; set; }

    [Required]
    [Column("cost_type")]
    [MaxLength(30)]
    public string CostType { get; set; } = string.Empty; // DirectMaterial/DirectLabor/ManufacturingOverhead

    [Required]
    [Column("amount")]
    public decimal Amount { get; set; }

    [Column("currency")]
    [MaxLength(10)]
    public string Currency { get; set; } = "CNY";

    [Column("detail_json")]
    public string? DetailJson { get; set; } // JSON

    [Column("created_by")]
    [MaxLength(50)]
    public string? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================================
// Yield Statistics
// ============================================================================

/// <summary>
/// 良率统计表 - Yield statistics per lot/step
/// </summary>
[Table("yield_statistics")]
public class YieldStatistics
{
    [Key]
    [Column("stat_id")]
    [MaxLength(50)]
    public string StatId { get; set; } = string.Empty;

    [Required]
    [Column("lot_id")]
    [MaxLength(50)]
    public string LotId { get; set; } = string.Empty;

    [Column("step_code")]
    [MaxLength(50)]
    public string? StepCode { get; set; }

    [Column("step_name")]
    [MaxLength(100)]
    public string? StepName { get; set; }

    [Required]
    [Column("input_qty")]
    public int InputQty { get; set; }

    [Required]
    [Column("output_qty")]
    public int OutputQty { get; set; }

    [Column("yield_rate")]
    public decimal? YieldRate { get; set; }

    [Column("scrap_qty")]
    public int ScrapQty { get; set; }

    [Column("rework_qty")]
    public int ReworkQty { get; set; }

    [Column("defect_json")]
    public string? DefectJson { get; set; } // JSON

    [Required]
    [Column("stat_date")]
    public DateOnly StatDate { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================================
// NPI Stage
// ============================================================================

/// <summary>
/// NPI阶段表 - NPI project stages
/// </summary>
[Table("npi_stage")]
public class NpiStage
{
    [Key]
    [Column("stage_id")]
    [MaxLength(50)]
    public string StageId { get; set; } = string.Empty;

    [Required]
    [Column("project_id")]
    [MaxLength(50)]
    public string ProjectId { get; set; } = string.Empty;

    [Required]
    [Column("stage_name")]
    [MaxLength(100)]
    public string StageName { get; set; } = string.Empty;

    [Column("stage_order")]
    public int? StageOrder { get; set; }

    [Column("status")]
    [MaxLength(30)]
    public string Status { get; set; } = "Pending"; // Pending/InProgress/Completed/Skipped

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [Column("result")]
    public string? Result { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================================
// Reliability Test Plan
// ============================================================================

/// <summary>
/// 可靠性测试计划表 - Reliability test plans (HTOL/THB/TC/uHAST/ESD/LatchUp)
/// </summary>
[Table("reliability_test_plan")]
public class ReliabilityTestPlan
{
    [Key]
    [Column("plan_id")]
    [MaxLength(50)]
    public string PlanId { get; set; } = string.Empty;

    [Required]
    [Column("plan_name")]
    [MaxLength(200)]
    public string PlanName { get; set; } = string.Empty;

    [Required]
    [Column("test_type")]
    [MaxLength(30)]
    public string TestType { get; set; } = string.Empty; // HTOL/THB/TC/uHAST/ESD/LatchUp

    [Required]
    [Column("product_id")]
    [MaxLength(50)]
    public string ProductId { get; set; } = string.Empty;

    [Column("lot_id")]
    [MaxLength(50)]
    public string? LotId { get; set; }

    [Column("sample_size")]
    public int? SampleSize { get; set; }

    [Column("test_duration")]
    public int? TestDuration { get; set; } // Hours

    [Column("test_conditions")]
    public string? TestConditions { get; set; }

    [Column("start_date")]
    public DateTime? StartDate { get; set; }

    [Column("end_date")]
    public DateTime? EndDate { get; set; }

    [Column("status")]
    [MaxLength(30)]
    public string Status { get; set; } = "Planned"; // Planned/InProgress/Completed/Failed

    [Column("result_summary")]
    public string? ResultSummary { get; set; }

    [Column("fa_triggered")]
    public bool FaTriggered { get; set; }

    [Column("created_by")]
    [MaxLength(50)]
    public string? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================================
// Report Schedule
// ============================================================================

/// <summary>
/// 报表计划表 - Automated report scheduling
/// </summary>
[Table("report_schedule")]
public class ReportSchedule
{
    [Key]
    [Column("schedule_id")]
    [MaxLength(50)]
    public string ScheduleId { get; set; } = string.Empty;

    [Required]
    [Column("report_name")]
    [MaxLength(200)]
    public string ReportName { get; set; } = string.Empty;

    [Required]
    [Column("report_type")]
    [MaxLength(30)]
    public string ReportType { get; set; } = string.Empty; // Daily/Weekly/Monthly/Custom

    [Column("schedule_cron")]
    [MaxLength(100)]
    public string? ScheduleCron { get; set; }

    [Column("recipients")]
    public string? Recipients { get; set; } // JSON array of emails/userIds

    [Column("format")]
    [MaxLength(20)]
    public string Format { get; set; } = "PDF"; // PDF/Excel/HTML

    [Column("enabled")]
    public bool Enabled { get; set; } = true;

    [Column("last_run_date")]
    public DateTime? LastRunDate { get; set; }

    [Column("next_run_date")]
    public DateTime? NextRunDate { get; set; }

    [Column("created_by")]
    [MaxLength(50)]
    public string? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================================
// System Config
// ============================================================================

/// <summary>
/// 系统参数配置表 - System configuration key-value store
/// </summary>
[Table("system_config")]
public class SystemConfig
{
    [Key]
    [Column("config_id")]
    [MaxLength(50)]
    public string ConfigId { get; set; } = string.Empty;

    [Required]
    [Column("config_key")]
    [MaxLength(100)]
    public string ConfigKey { get; set; } = string.Empty;

    [Column("config_value")]
    public string? ConfigValue { get; set; }

    [Column("config_type")]
    [MaxLength(30)]
    public string? ConfigType { get; set; } // String/Number/Boolean/JSON

    [Column("category")]
    [MaxLength(50)]
    public string? Category { get; set; } // General/Quality/Production/Alert/Workflow

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("is_public")]
    public bool IsPublic { get; set; } = true;

    [Column("updated_by")]
    [MaxLength(50)]
    public string? UpdatedBy { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================================
// Alert Rule
// ============================================================================

/// <summary>
/// 预警规则表 - Alert and notification rules
/// </summary>
[Table("alert_rule")]
public class AlertRule
{
    [Key]
    [Column("rule_id")]
    [MaxLength(50)]
    public string RuleId { get; set; } = string.Empty;

    [Required]
    [Column("rule_name")]
    [MaxLength(200)]
    public string RuleName { get; set; } = string.Empty;

    [Required]
    [Column("rule_type")]
    [MaxLength(30)]
    public string RuleType { get; set; } = string.Empty; // Yield/Equipment/Material/Quality/Schedule

    [Column("condition_expression")]
    public string? ConditionExpression { get; set; }

    [Column("threshold_value")]
    public decimal? ThresholdValue { get; set; }

    [Column("severity")]
    [MaxLength(20)]
    public string Severity { get; set; } = "Warning"; // Info/Warning/Critical

    [Column("notification_channels")]
    public string? NotificationChannels { get; set; } // JSON array: Email/SMS/System

    [Column("enabled")]
    public bool Enabled { get; set; } = true;

    [Column("created_by")]
    [MaxLength(50)]
    public string? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================================
// Data Correction Record (V4.9.0)
// ============================================================================

/// <summary>
/// 数据修正记录表 - Audit trail for manual data corrections
/// </summary>
[Table("data_correction_record")]
public class DataCorrectionRecord
{
    [Key]
    [Column("correction_id")]
    [MaxLength(50)]
    public string CorrectionId { get; set; } = string.Empty;

    [Required]
    [Column("table_name")]
    [MaxLength(100)]
    public string TableName { get; set; } = string.Empty;

    [Required]
    [Column("record_id")]
    [MaxLength(100)]
    public string RecordId { get; set; } = string.Empty;

    [Required]
    [Column("field_name")]
    [MaxLength(100)]
    public string FieldName { get; set; } = string.Empty;

    [Column("old_value")]
    public string? OldValue { get; set; }

    [Column("new_value")]
    public string? NewValue { get; set; }

    [Required]
    [Column("reason")]
    public string Reason { get; set; } = string.Empty;

    [Column("approved_by")]
    [MaxLength(50)]
    public string? ApprovedBy { get; set; }

    [Required]
    [Column("corrected_by")]
    [MaxLength(50)]
    public string CorrectedBy { get; set; } = string.Empty;

    [Column("corrected_at")]
    public DateTime CorrectedAt { get; set; } = DateTime.UtcNow;
}
