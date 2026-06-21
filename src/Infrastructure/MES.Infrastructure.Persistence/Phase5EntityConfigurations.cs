using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Infrastructure.Persistence;

public static class Phase5EntityConfigurations
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        // KpiDashboardSnapshot
        modelBuilder.Entity<KpiDashboardSnapshot>(e =>
        {
            e.ToTable("kpi_dashboard_snapshot");
            e.HasKey(x => x.SnapshotId);
            e.Property(x => x.SnapshotId).HasColumnName("snapshot_id").HasMaxLength(50);
            e.Property(x => x.SnapshotDate).HasColumnName("snapshot_date");
            e.Property(x => x.MetricCode).HasColumnName("metric_code").HasMaxLength(50);
            e.Property(x => x.MetricName).HasColumnName("metric_name").HasMaxLength(200);
            e.Property(x => x.MetricValue).HasColumnName("metric_value").HasPrecision(12, 4);
            e.Property(x => x.TargetValue).HasColumnName("target_value").HasPrecision(12, 4);
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Trend).HasColumnName("trend").HasMaxLength(20);
            e.Property(x => x.PeriodType).HasColumnName("period_type").HasMaxLength(20);
            e.Property(x => x.DetailData).HasColumnName("detail_data").HasColumnType("json");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => new { x.SnapshotDate, x.MetricCode }).HasDatabaseName("idx_kds_date_metric");
            e.HasIndex(x => x.PeriodType).HasDatabaseName("idx_kds_period");
        });

        // CostRecord
        modelBuilder.Entity<CostRecord>(e =>
        {
            e.ToTable("cost_record");
            e.HasKey(x => x.CostId);
            e.Property(x => x.CostId).HasColumnName("cost_id").HasMaxLength(50);
            e.Property(x => x.WorkOrderId).HasColumnName("work_order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.CostDate).HasColumnName("cost_date");
            e.Property(x => x.CostType).HasColumnName("cost_type").HasMaxLength(30);
            e.Property(x => x.Amount).HasColumnName("amount").HasPrecision(12, 2);
            e.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(10);
            e.Property(x => x.DetailJson).HasColumnName("detail_json").HasColumnType("json");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.WorkOrderId).HasDatabaseName("idx_cr_work_order");
            e.HasIndex(x => x.CostDate).HasDatabaseName("idx_cr_date");
        });

        // YieldStatistics
        modelBuilder.Entity<YieldStatistics>(e =>
        {
            e.ToTable("yield_statistics");
            e.HasKey(x => x.StatId);
            e.Property(x => x.StatId).HasColumnName("stat_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepName).HasColumnName("step_name").HasMaxLength(100);
            e.Property(x => x.InputQty).HasColumnName("input_qty");
            e.Property(x => x.OutputQty).HasColumnName("output_qty");
            e.Property(x => x.YieldRate).HasColumnName("yield_rate").HasPrecision(5, 2);
            e.Property(x => x.ScrapQty).HasColumnName("scrap_qty");
            e.Property(x => x.ReworkQty).HasColumnName("rework_qty");
            e.Property(x => x.DefectJson).HasColumnName("defect_json").HasColumnType("json");
            e.Property(x => x.StatDate).HasColumnName("stat_date");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_ys_lot");
            e.HasIndex(x => x.StatDate).HasDatabaseName("idx_ys_date");
        });

        // NpiStage
        modelBuilder.Entity<NpiStage>(e =>
        {
            e.ToTable("npi_stage");
            e.HasKey(x => x.StageId);
            e.Property(x => x.StageId).HasColumnName("stage_id").HasMaxLength(50);
            e.Property(x => x.ProjectId).HasColumnName("project_id").HasMaxLength(50);
            e.Property(x => x.StageName).HasColumnName("stage_name").HasMaxLength(100);
            e.Property(x => x.StageOrder).HasColumnName("stage_order");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.StartDate).HasColumnName("start_date");
            e.Property(x => x.EndDate).HasColumnName("end_date");
            e.Property(x => x.Result).HasColumnName("result").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.ProjectId).HasDatabaseName("idx_ns_project");
        });

        // ReliabilityTestPlan
        modelBuilder.Entity<ReliabilityTestPlan>(e =>
        {
            e.ToTable("reliability_test_plan");
            e.HasKey(x => x.PlanId);
            e.Property(x => x.PlanId).HasColumnName("plan_id").HasMaxLength(50);
            e.Property(x => x.PlanName).HasColumnName("plan_name").HasMaxLength(200);
            e.Property(x => x.TestType).HasColumnName("test_type").HasMaxLength(30);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.SampleSize).HasColumnName("sample_size");
            e.Property(x => x.TestDuration).HasColumnName("test_duration");
            e.Property(x => x.TestConditions).HasColumnName("test_conditions").HasColumnType("text");
            e.Property(x => x.StartDate).HasColumnName("start_date");
            e.Property(x => x.EndDate).HasColumnName("end_date");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.ResultSummary).HasColumnName("result_summary").HasColumnType("text");
            e.Property(x => x.FaTriggered).HasColumnName("fa_triggered");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.TestType).HasDatabaseName("idx_rtp_type");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_rtp_status");
        });

        // ReportSchedule
        modelBuilder.Entity<ReportSchedule>(e =>
        {
            e.ToTable("report_schedule");
            e.HasKey(x => x.ScheduleId);
            e.Property(x => x.ScheduleId).HasColumnName("schedule_id").HasMaxLength(50);
            e.Property(x => x.ReportName).HasColumnName("report_name").HasMaxLength(200);
            e.Property(x => x.ReportType).HasColumnName("report_type").HasMaxLength(30);
            e.Property(x => x.ScheduleCron).HasColumnName("schedule_cron").HasMaxLength(100);
            e.Property(x => x.Recipients).HasColumnName("recipients").HasColumnType("json");
            e.Property(x => x.Format).HasColumnName("format").HasMaxLength(20);
            e.Property(x => x.Enabled).HasColumnName("enabled");
            e.Property(x => x.LastRunDate).HasColumnName("last_run_date");
            e.Property(x => x.NextRunDate).HasColumnName("next_run_date");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.ReportType).HasDatabaseName("idx_rs_type");
            e.HasIndex(x => x.Enabled).HasDatabaseName("idx_rs_enabled");
        });

        // SystemConfig
        modelBuilder.Entity<SystemConfig>(e =>
        {
            e.ToTable("system_config");
            e.HasKey(x => x.ConfigId);
            e.Property(x => x.ConfigId).HasColumnName("config_id").HasMaxLength(50);
            e.Property(x => x.ConfigKey).HasColumnName("config_key").HasMaxLength(100);
            e.Property(x => x.ConfigValue).HasColumnName("config_value").HasColumnType("text");
            e.Property(x => x.ConfigType).HasColumnName("config_type").HasMaxLength(30);
            e.Property(x => x.Category).HasColumnName("category").HasMaxLength(50);
            e.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
            e.Property(x => x.IsPublic).HasColumnName("is_public");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.ConfigKey).IsUnique().HasDatabaseName("idx_sc_key");
            e.HasIndex(x => x.Category).HasDatabaseName("idx_sc_category");
        });

        // AlertRule
        modelBuilder.Entity<AlertRule>(e =>
        {
            e.ToTable("alert_rule");
            e.HasKey(x => x.RuleId);
            e.Property(x => x.RuleId).HasColumnName("rule_id").HasMaxLength(50);
            e.Property(x => x.RuleName).HasColumnName("rule_name").HasMaxLength(200);
            e.Property(x => x.RuleType).HasColumnName("rule_type").HasMaxLength(30);
            e.Property(x => x.ConditionExpression).HasColumnName("condition_expression").HasColumnType("text");
            e.Property(x => x.ThresholdValue).HasColumnName("threshold_value").HasPrecision(10, 2);
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.NotificationChannels).HasColumnName("notification_channels").HasColumnType("json");
            e.Property(x => x.Enabled).HasColumnName("enabled");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.RuleType).HasDatabaseName("idx_ar_type");
        });

        // DataCorrectionRecord
        modelBuilder.Entity<DataCorrectionRecord>(e =>
        {
            e.ToTable("data_correction_record");
            e.HasKey(x => x.CorrectionId);
            e.Property(x => x.CorrectionId).HasColumnName("correction_id").HasMaxLength(50);
            e.Property(x => x.TableName).HasColumnName("table_name").HasMaxLength(100);
            e.Property(x => x.RecordId).HasColumnName("record_id").HasMaxLength(100);
            e.Property(x => x.FieldName).HasColumnName("field_name").HasMaxLength(100);
            e.Property(x => x.OldValue).HasColumnName("old_value").HasColumnType("text");
            e.Property(x => x.NewValue).HasColumnName("new_value").HasColumnType("text");
            e.Property(x => x.Reason).HasColumnName("reason").HasColumnType("text");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.CorrectedBy).HasColumnName("corrected_by").HasMaxLength(50);
            e.Property(x => x.CorrectedAt).HasColumnName("corrected_at");
            e.HasIndex(x => new { x.TableName, x.RecordId }).HasDatabaseName("idx_dcr_table_record");
        });
    }
}
