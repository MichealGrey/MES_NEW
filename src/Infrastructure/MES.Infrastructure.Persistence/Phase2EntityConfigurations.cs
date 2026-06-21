using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Infrastructure.Persistence;

/// <summary>
/// EF Core entity configurations for all Phase 2 entities.
/// Call Phase2EntityConfigurations.Apply(modelBuilder) from MesDbContext.OnModelCreating.
/// </summary>
public static class Phase2EntityConfigurations
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        ConfigureOrderReview(modelBuilder);
        ConfigureProductionPlan(modelBuilder);
        ConfigureBomMrp(modelBuilder);
        ConfigureOrderProgress(modelBuilder);
        ConfigureRushOrder(modelBuilder);
    }

    private static void ConfigureOrderReview(ModelBuilder modelBuilder)
    {
        // === SalesOrder ===
        modelBuilder.Entity<SalesOrder>(e =>
        {
            e.ToTable("sales_order");
            e.HasKey(x => x.OrderId);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(200);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(200);
            e.Property(x => x.ProductSpec).HasColumnName("product_spec").HasMaxLength(500);
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.UnitPrice).HasColumnName("unit_price").HasPrecision(12, 4);
            e.Property(x => x.TotalAmount).HasColumnName("total_amount").HasPrecision(15, 2);
            e.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(10);
            e.Property(x => x.OrderDate).HasColumnName("order_date");
            e.Property(x => x.DeliveryDate).HasColumnName("delivery_date");
            e.Property(x => x.Priority).HasColumnName("priority").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.ReviewStatus).HasColumnName("review_status").HasMaxLength(30);
            e.Property(x => x.ReviewResult).HasColumnName("review_result").HasColumnType("text");
            e.Property(x => x.PackageType).HasColumnName("package_type").HasMaxLength(100);
            e.Property(x => x.LeadFrameType).HasColumnName("lead_frame_type").HasMaxLength(100);
            e.Property(x => x.WireType).HasColumnName("wire_type").HasMaxLength(50);
            e.Property(x => x.SpecialRequirements).HasColumnName("special_requirements").HasColumnType("text");
            e.Property(x => x.QualityLevel).HasColumnName("quality_level").HasMaxLength(50);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.Deleted).HasColumnName("deleted");
            e.HasIndex(x => x.CustomerId).HasDatabaseName("idx_so_customer");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_so_status");
            e.HasIndex(x => x.ReviewStatus).HasDatabaseName("idx_so_review_status");
            e.HasIndex(x => x.DeliveryDate).HasDatabaseName("idx_so_delivery");
            e.HasIndex(x => x.Priority).HasDatabaseName("idx_so_priority");
            e.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_so_created");
        });

        // === OrderReview ===
        modelBuilder.Entity<OrderReview>(e =>
        {
            e.ToTable("order_review");
            e.HasKey(x => x.ReviewId);
            e.Property(x => x.ReviewId).HasColumnName("review_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ReviewType).HasColumnName("review_type").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.StartTime).HasColumnName("start_time");
            e.Property(x => x.EndTime).HasColumnName("end_time");
            e.Property(x => x.Deadline).HasColumnName("deadline");
            e.Property(x => x.InitiatedBy).HasColumnName("initiated_by").HasMaxLength(50);
            e.Property(x => x.Conclusion).HasColumnName("conclusion").HasColumnType("text");
            e.Property(x => x.Conditions).HasColumnName("conditions").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_or_order");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_or_status");
            e.HasIndex(x => x.Deadline).HasDatabaseName("idx_or_deadline");
        });

        // === OrderReviewItem ===
        modelBuilder.Entity<OrderReviewItem>(e =>
        {
            e.ToTable("order_review_item");
            e.HasKey(x => x.ItemId);
            e.Property(x => x.ItemId).HasColumnName("item_id").ValueGeneratedOnAdd();
            e.Property(x => x.ReviewId).HasColumnName("review_id").HasMaxLength(50);
            e.Property(x => x.ReviewerRole).HasColumnName("reviewer_role").HasMaxLength(50);
            e.Property(x => x.ReviewerName).HasColumnName("reviewer_name").HasMaxLength(100);
            e.Property(x => x.Vote).HasColumnName("vote").HasMaxLength(20);
            e.Property(x => x.Comments).HasColumnName("comments").HasColumnType("text");
            e.Property(x => x.Conditions).HasColumnName("conditions").HasColumnType("text");
            e.Property(x => x.ReviewTime).HasColumnName("review_time");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.ReviewId).HasDatabaseName("idx_ori_review");
            e.HasIndex(x => x.ReviewerRole).HasDatabaseName("idx_ori_role");
            e.HasIndex(x => x.Vote).HasDatabaseName("idx_ori_vote");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_ori_status");
        });

        // === OrderVersion ===
        modelBuilder.Entity<OrderVersion>(e =>
        {
            e.ToTable("order_version");
            e.HasKey(x => x.VersionId);
            e.Property(x => x.VersionId).HasColumnName("version_id").ValueGeneratedOnAdd();
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.VersionNo).HasColumnName("version_no");
            e.Property(x => x.ChangeType).HasColumnName("change_type").HasMaxLength(50);
            e.Property(x => x.ChangeReason).HasColumnName("change_reason").HasColumnType("text");
            e.Property(x => x.ChangeDescription).HasColumnName("change_description").HasColumnType("text");
            e.Property(x => x.OldData).HasColumnName("old_data").HasColumnType("json");
            e.Property(x => x.NewData).HasColumnName("new_data").HasColumnType("json");
            e.Property(x => x.ChangedBy).HasColumnName("changed_by").HasMaxLength(50);
            e.Property(x => x.ChangedAt).HasColumnName("changed_at");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_ov_order");
            e.HasIndex(x => new { x.OrderId, x.VersionNo }).IsUnique().HasDatabaseName("uk_ov_order_version");
        });
    }

    private static void ConfigureProductionPlan(ModelBuilder modelBuilder)
    {
        // === MasterProductionPlan ===
        modelBuilder.Entity<MasterProductionPlan>(e =>
        {
            e.ToTable("master_production_plan");
            e.HasKey(x => x.PlanId);
            e.Property(x => x.PlanId).HasColumnName("plan_id").HasMaxLength(50);
            e.Property(x => x.PlanName).HasColumnName("plan_name").HasMaxLength(200);
            e.Property(x => x.PlanType).HasColumnName("plan_type").HasMaxLength(50);
            e.Property(x => x.PlanPeriodStart).HasColumnName("plan_period_start");
            e.Property(x => x.PlanPeriodEnd).HasColumnName("plan_period_end");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.TotalDemandQty).HasColumnName("total_demand_qty");
            e.Property(x => x.TotalCapacity).HasColumnName("total_capacity");
            e.Property(x => x.CapacityUtilization).HasColumnName("capacity_utilization").HasPrecision(5, 2);
            e.Property(x => x.BottleneckIdentified).HasColumnName("bottleneck_identified");
            e.Property(x => x.BottleneckDescription).HasColumnName("bottleneck_description").HasColumnType("text");
            e.Property(x => x.Planner).HasColumnName("planner").HasMaxLength(100);
            e.Property(x => x.PlanData).HasColumnName("plan_data").HasColumnType("json");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.PublishedBy).HasColumnName("published_by").HasMaxLength(50);
            e.Property(x => x.PublishedAt).HasColumnName("published_at");
            e.HasIndex(x => x.PlanPeriodStart).HasDatabaseName("idx_mpp_period_start");
            e.HasIndex(x => x.PlanPeriodEnd).HasDatabaseName("idx_mpp_period_end");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_mpp_status");
            e.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_mpp_created");
        });

        // === CapacityLoad ===
        modelBuilder.Entity<CapacityLoad>(e =>
        {
            e.ToTable("capacity_load");
            e.HasKey(x => x.LoadId);
            e.Property(x => x.LoadId).HasColumnName("load_id").ValueGeneratedOnAdd();
            e.Property(x => x.PlanId).HasColumnName("plan_id").HasMaxLength(50);
            e.Property(x => x.ProcessCode).HasColumnName("process_code").HasMaxLength(50);
            e.Property(x => x.ProcessName).HasColumnName("process_name").HasMaxLength(100);
            e.Property(x => x.EquipmentGroup).HasColumnName("equipment_group").HasMaxLength(100);
            e.Property(x => x.Uph).HasColumnName("uph").HasPrecision(10, 2);
            e.Property(x => x.AvailableHours).HasColumnName("available_hours").HasPrecision(10, 2);
            e.Property(x => x.RequiredHours).HasColumnName("required_hours").HasPrecision(10, 2);
            e.Property(x => x.LoadRate).HasColumnName("load_rate").HasPrecision(5, 2);
            e.Property(x => x.IsBottleneck).HasColumnName("is_bottleneck");
            e.Property(x => x.AvailableQty).HasColumnName("available_qty");
            e.Property(x => x.RequiredQty).HasColumnName("required_qty");
            e.Property(x => x.ShortageQty).HasColumnName("shortage_qty");
            e.Property(x => x.ShiftPlan).HasColumnName("shift_plan").HasMaxLength(50);
            e.Property(x => x.CalculatedAt).HasColumnName("calculated_at");
            e.HasIndex(x => x.PlanId).HasDatabaseName("idx_cl_plan");
            e.HasIndex(x => x.ProcessCode).HasDatabaseName("idx_cl_process");
            e.HasIndex(x => x.IsBottleneck).HasDatabaseName("idx_cl_bottleneck");
            e.HasIndex(x => x.LoadRate).HasDatabaseName("idx_cl_load_rate");
        });

        // === CapacitySimulation ===
        modelBuilder.Entity<CapacitySimulation>(e =>
        {
            e.ToTable("capacity_simulation");
            e.HasKey(x => x.SimulationId);
            e.Property(x => x.SimulationId).HasColumnName("simulation_id").HasMaxLength(50);
            e.Property(x => x.SimulationName).HasColumnName("simulation_name").HasMaxLength(200);
            e.Property(x => x.BasePlanId).HasColumnName("base_plan_id").HasMaxLength(50);
            e.Property(x => x.ScenarioDescription).HasColumnName("scenario_description").HasColumnType("text");
            e.Property(x => x.ScenarioParams).HasColumnName("scenario_params").HasColumnType("json");
            e.Property(x => x.TotalDemandQty).HasColumnName("total_demand_qty");
            e.Property(x => x.TotalCapacity).HasColumnName("total_capacity");
            e.Property(x => x.CapacityUtilization).HasColumnName("capacity_utilization").HasPrecision(5, 2);
            e.Property(x => x.BottleneckCount).HasColumnName("bottleneck_count");
            e.Property(x => x.ResultSummary).HasColumnName("result_summary").HasColumnType("text");
            e.Property(x => x.ResultData).HasColumnName("result_data").HasColumnType("json");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.BasePlanId).HasDatabaseName("idx_cs_base_plan");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_cs_status");
            e.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_cs_created");
        });
    }

    private static void ConfigureBomMrp(ModelBuilder modelBuilder)
    {
        // === Bom ===
        modelBuilder.Entity<Bom>(e =>
        {
            e.ToTable("bom");
            e.HasKey(x => x.BomId);
            e.Property(x => x.BomId).HasColumnName("bom_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(200);
            e.Property(x => x.BomVersion).HasColumnName("bom_version").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.EffectiveDate).HasColumnName("effective_date");
            e.Property(x => x.ExpiryDate).HasColumnName("expiry_date");
            e.Property(x => x.TotalItems).HasColumnName("total_items");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.ProductId).HasDatabaseName("idx_bom_product");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_bom_status");
            e.HasIndex(x => new { x.ProductId, x.BomVersion }).IsUnique().HasDatabaseName("uk_bom_product_version");
        });

        // === BomItem ===
        modelBuilder.Entity<BomItem>(e =>
        {
            e.ToTable("bom_item");
            e.HasKey(x => x.ItemId);
            e.Property(x => x.ItemId).HasColumnName("item_id").ValueGeneratedOnAdd();
            e.Property(x => x.BomId).HasColumnName("bom_id").HasMaxLength(50);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(200);
            e.Property(x => x.MaterialSpec).HasColumnName("material_spec").HasMaxLength(500);
            e.Property(x => x.QuantityPerUnit).HasColumnName("quantity_per_unit").HasPrecision(12, 4);
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.LossRate).HasColumnName("loss_rate").HasPrecision(5, 2);
            e.Property(x => x.SubstituteMaterials).HasColumnName("substitute_materials").HasColumnType("json");
            e.Property(x => x.SortOrder).HasColumnName("sort_order");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.BomId).HasDatabaseName("idx_bi_bom");
            e.HasIndex(x => x.MaterialId).HasDatabaseName("idx_bi_material");
        });

        // === MrpCalculation ===
        modelBuilder.Entity<MrpCalculation>(e =>
        {
            e.ToTable("mrp_calculation");
            e.HasKey(x => x.CalculationId);
            e.Property(x => x.CalculationId).HasColumnName("calculation_id").HasMaxLength(50);
            e.Property(x => x.PlanId).HasColumnName("plan_id").HasMaxLength(50);
            e.Property(x => x.WorkOrderId).HasColumnName("work_order_id").HasMaxLength(50);
            e.Property(x => x.CalculationType).HasColumnName("calculation_type").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.TotalDemandItems).HasColumnName("total_demand_items");
            e.Property(x => x.ShortageItems).HasColumnName("shortage_items");
            e.Property(x => x.SufficientItems).HasColumnName("sufficient_items");
            e.Property(x => x.CalculationParams).HasColumnName("calculation_params").HasColumnType("json");
            e.Property(x => x.ResultSummary).HasColumnName("result_summary").HasColumnType("text");
            e.Property(x => x.ResultData).HasColumnName("result_data").HasColumnType("json");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.PlanId).HasDatabaseName("idx_mrp_plan");
            e.HasIndex(x => x.WorkOrderId).HasDatabaseName("idx_mrp_work_order");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_mrp_status");
            e.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_mrp_created");
        });

        // === MrpShortageWarning ===
        modelBuilder.Entity<MrpShortageWarning>(e =>
        {
            e.ToTable("mrp_shortage_warning");
            e.HasKey(x => x.WarningId);
            e.Property(x => x.WarningId).HasColumnName("warning_id").ValueGeneratedOnAdd();
            e.Property(x => x.CalculationId).HasColumnName("calculation_id").HasMaxLength(50);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(200);
            e.Property(x => x.RequiredQty).HasColumnName("required_qty").HasPrecision(15, 4);
            e.Property(x => x.AvailableQty).HasColumnName("available_qty").HasPrecision(15, 4);
            e.Property(x => x.ShortageQty).HasColumnName("shortage_qty").HasPrecision(15, 4);
            e.Property(x => x.ExpectedArrival).HasColumnName("expected_arrival");
            e.Property(x => x.PurchaseOrderNo).HasColumnName("purchase_order_no").HasMaxLength(50);
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.ResolutionNote).HasColumnName("resolution_note").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.CalculationId).HasDatabaseName("idx_msw_calculation");
            e.HasIndex(x => x.MaterialId).HasDatabaseName("idx_msw_material");
            e.HasIndex(x => x.Severity).HasDatabaseName("idx_msw_severity");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_msw_status");
        });
    }

    private static void ConfigureOrderProgress(ModelBuilder modelBuilder)
    {
        // === OrderProgressSnapshot ===
        modelBuilder.Entity<OrderProgressSnapshot>(e =>
        {
            e.ToTable("order_progress_snapshot");
            e.HasKey(x => x.SnapshotId);
            e.Property(x => x.SnapshotId).HasColumnName("snapshot_id").ValueGeneratedOnAdd();
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.SnapshotTime).HasColumnName("snapshot_time");
            e.Property(x => x.TotalQuantity).HasColumnName("total_quantity");
            e.Property(x => x.CompletedQuantity).HasColumnName("completed_quantity");
            e.Property(x => x.InProgressQuantity).HasColumnName("in_progress_quantity");
            e.Property(x => x.PendingQuantity).HasColumnName("pending_quantity");
            e.Property(x => x.DefectiveQuantity).HasColumnName("defective_quantity");
            e.Property(x => x.YieldRate).HasColumnName("yield_rate").HasPrecision(5, 2);
            e.Property(x => x.ProgressPercentage).HasColumnName("progress_percentage").HasPrecision(5, 2);
            e.Property(x => x.CurrentStage).HasColumnName("current_stage").HasMaxLength(100);
            e.Property(x => x.EstimatedCompletion).HasColumnName("estimated_completion");
            e.Property(x => x.IsDelayed).HasColumnName("is_delayed");
            e.Property(x => x.DelayDays).HasColumnName("delay_days");
            e.Property(x => x.WorkOrderCount).HasColumnName("work_order_count");
            e.Property(x => x.CompletedWorkOrderCount).HasColumnName("completed_work_order_count");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_ops_order");
            e.HasIndex(x => x.SnapshotTime).HasDatabaseName("idx_ops_time");
            e.HasIndex(x => x.IsDelayed).HasDatabaseName("idx_ops_delayed");
        });

        // === OtdStatistics ===
        modelBuilder.Entity<OtdStatistics>(e =>
        {
            e.ToTable("otd_statistics");
            e.HasKey(x => x.StatId);
            e.Property(x => x.StatId).HasColumnName("stat_id").ValueGeneratedOnAdd();
            e.Property(x => x.StatPeriod).HasColumnName("stat_period").HasMaxLength(20);
            e.Property(x => x.TotalOrders).HasColumnName("total_orders");
            e.Property(x => x.OnTimeOrders).HasColumnName("on_time_orders");
            e.Property(x => x.LateOrders).HasColumnName("late_orders");
            e.Property(x => x.OtdRate).HasColumnName("otd_rate").HasPrecision(5, 2);
            e.Property(x => x.AvgDelayDays).HasColumnName("avg_delay_days").HasPrecision(5, 2);
            e.Property(x => x.MaxDelayDays).HasColumnName("max_delay_days");
            e.Property(x => x.TotalQuantity).HasColumnName("total_quantity");
            e.Property(x => x.OnTimeQuantity).HasColumnName("on_time_quantity");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.StatPeriod).IsUnique().HasDatabaseName("uk_os_stat_period");
        });

        // === DelayReasonRecord ===
        modelBuilder.Entity<DelayReasonRecord>(e =>
        {
            e.ToTable("delay_reason_record");
            e.HasKey(x => x.RecordId);
            e.Property(x => x.RecordId).HasColumnName("record_id").ValueGeneratedOnAdd();
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.WorkOrderId).HasColumnName("work_order_id").HasMaxLength(50);
            e.Property(x => x.DelayReasonCategory).HasColumnName("delay_reason_category").HasMaxLength(50);
            e.Property(x => x.DelayReasonDetail).HasColumnName("delay_reason_detail").HasColumnType("text");
            e.Property(x => x.DelayDays).HasColumnName("delay_days");
            e.Property(x => x.ImpactQuantity).HasColumnName("impact_quantity");
            e.Property(x => x.ResponsibleDept).HasColumnName("responsible_dept").HasMaxLength(100);
            e.Property(x => x.CorrectiveAction).HasColumnName("corrective_action").HasColumnType("text");
            e.Property(x => x.PreventiveAction).HasColumnName("preventive_action").HasColumnType("text");
            e.Property(x => x.ReportedBy).HasColumnName("reported_by").HasMaxLength(50);
            e.Property(x => x.ReportedAt).HasColumnName("reported_at");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_drr_order");
            e.HasIndex(x => x.DelayReasonCategory).HasDatabaseName("idx_drr_category");
            e.HasIndex(x => x.ReportedAt).HasDatabaseName("idx_drr_reported");
        });
    }

    private static void ConfigureRushOrder(ModelBuilder modelBuilder)
    {
        // === RushOrderRequest ===
        modelBuilder.Entity<RushOrderRequest>(e =>
        {
            e.ToTable("rush_order_request");
            e.HasKey(x => x.RequestId);
            e.Property(x => x.RequestId).HasColumnName("request_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(200);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(200);
            e.Property(x => x.RushQuantity).HasColumnName("rush_quantity");
            e.Property(x => x.RequiredDate).HasColumnName("required_date");
            e.Property(x => x.RushReason).HasColumnName("rush_reason").HasColumnType("text");
            e.Property(x => x.PriorityLevel).HasColumnName("priority_level").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.ImpactAnalysisDone).HasColumnName("impact_analysis_done");
            e.Property(x => x.AnalysisSummary).HasColumnName("analysis_summary").HasColumnType("text");
            e.Property(x => x.ApprovalResult).HasColumnName("approval_result").HasMaxLength(20);
            e.Property(x => x.ApprovalBy).HasColumnName("approval_by").HasMaxLength(50);
            e.Property(x => x.ApprovalAt).HasColumnName("approval_at");
            e.Property(x => x.ApprovalComments).HasColumnName("approval_comments").HasColumnType("text");
            e.Property(x => x.ExecutedBy).HasColumnName("executed_by").HasMaxLength(50);
            e.Property(x => x.ExecutedAt).HasColumnName("executed_at");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_ror_order");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_ror_status");
            e.HasIndex(x => x.RequiredDate).HasDatabaseName("idx_ror_required");
            e.HasIndex(x => x.CreatedAt).HasDatabaseName("idx_ror_created");
        });

        // === RushOrderImpact ===
        modelBuilder.Entity<RushOrderImpact>(e =>
        {
            e.ToTable("rush_order_impact");
            e.HasKey(x => x.ImpactId);
            e.Property(x => x.ImpactId).HasColumnName("impact_id").ValueGeneratedOnAdd();
            e.Property(x => x.RequestId).HasColumnName("request_id").HasMaxLength(50);
            e.Property(x => x.AffectedOrderId).HasColumnName("affected_order_id").HasMaxLength(50);
            e.Property(x => x.AffectedWorkOrderId).HasColumnName("affected_work_order_id").HasMaxLength(50);
            e.Property(x => x.ImpactType).HasColumnName("impact_type").HasMaxLength(50);
            e.Property(x => x.ImpactDescription).HasColumnName("impact_description").HasColumnType("text");
            e.Property(x => x.OriginalDeliveryDate).HasColumnName("original_delivery_date");
            e.Property(x => x.NewEstimatedDelivery).HasColumnName("new_estimated_delivery");
            e.Property(x => x.DelayDays).HasColumnName("delay_days");
            e.Property(x => x.MaterialShortageItems).HasColumnName("material_shortage_items").HasColumnType("json");
            e.Property(x => x.CapacityConflictDetails).HasColumnName("capacity_conflict_details").HasColumnType("json");
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.MitigationPlan).HasColumnName("mitigation_plan").HasColumnType("text");
            e.Property(x => x.AnalyzedAt).HasColumnName("analyzed_at");
            e.HasIndex(x => x.RequestId).HasDatabaseName("idx_roi_request");
            e.HasIndex(x => x.AffectedOrderId).HasDatabaseName("idx_roi_order");
            e.HasIndex(x => x.Severity).HasDatabaseName("idx_roi_severity");
        });
    }
}
