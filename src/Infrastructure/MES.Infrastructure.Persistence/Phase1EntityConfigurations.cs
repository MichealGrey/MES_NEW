using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Infrastructure.Persistence;

/// <summary>
/// EF Core entity configurations for all Phase 1 entities.
/// Call Phase1EntityConfigurations.Apply(modelBuilder) from MesDbContext.OnModelCreating.
/// </summary>
public static class Phase1EntityConfigurations
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        ConfigureQuality(modelBuilder);
        ConfigureWarehouse(modelBuilder);
        ConfigureAbnormalEquipmentAlert(modelBuilder);
    }

    private static void ConfigureQuality(ModelBuilder modelBuilder)
    {
        // === IqcIncomingBatch ===
        modelBuilder.Entity<IqcIncomingBatch>(e =>
        {
            e.ToTable("iqc_incoming_batch");
            e.HasKey(x => x.BatchId);
            e.Property(x => x.BatchId).HasColumnName("batch_id").HasMaxLength(50);
            e.Property(x => x.PoNumber).HasColumnName("po_number").HasMaxLength(50);
            e.Property(x => x.SupplierId).HasColumnName("supplier_id").HasMaxLength(50);
            e.Property(x => x.SupplierName).HasColumnName("supplier_name").HasMaxLength(100);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(100);
            e.Property(x => x.MaterialSpec).HasColumnName("material_spec").HasMaxLength(255);
            e.Property(x => x.ReceivedQty).HasColumnName("received_qty");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.LotNumber).HasColumnName("lot_number").HasMaxLength(50);
            e.Property(x => x.ReceivedDate).HasColumnName("received_date");
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.InspectorId).HasColumnName("inspector_id").HasMaxLength(50);
            e.Property(x => x.InspectorName).HasColumnName("inspector_name").HasMaxLength(100);
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.PoNumber).HasDatabaseName("idx_iib_po");
            e.HasIndex(x => x.SupplierId).HasDatabaseName("idx_iib_supplier");
            e.HasIndex(x => x.MaterialId).HasDatabaseName("idx_iib_material");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_iib_status");
            e.HasIndex(x => x.ReceivedDate).HasDatabaseName("idx_iib_received");
        });

        // === IqcInspectionTask ===
        modelBuilder.Entity<IqcInspectionTask>(e =>
        {
            e.ToTable("iqc_inspection_task");
            e.HasKey(x => x.TaskId);
            e.Property(x => x.TaskId).HasColumnName("task_id").HasMaxLength(50);
            e.Property(x => x.BatchId).HasColumnName("batch_id").HasMaxLength(50);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.SupplierId).HasColumnName("supplier_id").HasMaxLength(50);
            e.Property(x => x.InspectionType).HasColumnName("inspection_type").HasMaxLength(50);
            e.Property(x => x.InspectionQty).HasColumnName("inspection_qty");
            e.Property(x => x.SampleSize).HasColumnName("sample_size");
            e.Property(x => x.SamplingPlan).HasColumnName("sampling_plan").HasMaxLength(50);
            e.Property(x => x.AqlLevel).HasColumnName("aql_level").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.InspectorId).HasColumnName("inspector_id").HasMaxLength(50);
            e.Property(x => x.InspectorName).HasColumnName("inspector_name").HasMaxLength(100);
            e.Property(x => x.AssignedTime).HasColumnName("assigned_time");
            e.Property(x => x.CompletedTime).HasColumnName("completed_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.BatchId).HasDatabaseName("idx_iit_batch");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_iit_status");
            e.HasIndex(x => x.InspectorId).HasDatabaseName("idx_iit_inspector");
        });

        // === IqcInspectionResult ===
        modelBuilder.Entity<IqcInspectionResult>(e =>
        {
            e.ToTable("iqc_inspection_result");
            e.HasKey(x => x.ResultId);
            e.Property(x => x.ResultId).HasColumnName("result_id").HasMaxLength(50);
            e.Property(x => x.TaskId).HasColumnName("task_id").HasMaxLength(50);
            e.Property(x => x.BatchId).HasColumnName("batch_id").HasMaxLength(50);
            e.Property(x => x.StandardId).HasColumnName("standard_id").HasMaxLength(50);
            e.Property(x => x.InspectionItem).HasColumnName("inspection_item").HasMaxLength(100);
            e.Property(x => x.InspectionMethod).HasColumnName("inspection_method").HasMaxLength(255);
            e.Property(x => x.StandardValue).HasColumnName("standard_value").HasMaxLength(100);
            e.Property(x => x.UpperLimit).HasColumnName("upper_limit").HasMaxLength(50);
            e.Property(x => x.LowerLimit).HasColumnName("lower_limit").HasMaxLength(50);
            e.Property(x => x.ActualValue).HasColumnName("actual_value").HasMaxLength(100);
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.Judgment).HasColumnName("judgment").HasMaxLength(20);
            e.Property(x => x.DefectCode).HasColumnName("defect_code").HasMaxLength(50);
            e.Property(x => x.DefectDescription).HasColumnName("defect_description").HasMaxLength(500);
            e.Property(x => x.InspectorId).HasColumnName("inspector_id").HasMaxLength(50);
            e.Property(x => x.InspectorName).HasColumnName("inspector_name").HasMaxLength(100);
            e.Property(x => x.InspectionTime).HasColumnName("inspection_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.TaskId).HasDatabaseName("idx_iir_task");
            e.HasIndex(x => x.BatchId).HasDatabaseName("idx_iir_batch");
            e.HasIndex(x => x.StandardId).HasDatabaseName("idx_iir_standard");
            e.HasIndex(x => x.InspectionTime).HasDatabaseName("idx_iir_time");
        });

        // === IqcInspectionStandard ===
        modelBuilder.Entity<IqcInspectionStandard>(e =>
        {
            e.ToTable("iqc_inspection_standard");
            e.HasKey(x => x.StandardId);
            e.Property(x => x.StandardId).HasColumnName("standard_id").HasMaxLength(50);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(100);
            e.Property(x => x.InspectionItem).HasColumnName("inspection_item").HasMaxLength(100);
            e.Property(x => x.InspectionMethod).HasColumnName("inspection_method").HasMaxLength(255);
            e.Property(x => x.StandardValue).HasColumnName("standard_value").HasMaxLength(100);
            e.Property(x => x.UpperLimit).HasColumnName("upper_limit").HasMaxLength(50);
            e.Property(x => x.LowerLimit).HasColumnName("lower_limit").HasMaxLength(50);
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.SamplingPlan).HasColumnName("sampling_plan").HasMaxLength(50);
            e.Property(x => x.AqlLevel).HasColumnName("aql_level").HasMaxLength(20);
            e.Property(x => x.SamplingQty).HasColumnName("sampling_qty");
            e.Property(x => x.InspectionType).HasColumnName("inspection_type").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Version).HasColumnName("version").HasMaxLength(20);
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.MaterialId).HasDatabaseName("idx_iis_material");
            e.HasIndex(x => x.InspectionType).HasDatabaseName("idx_iis_type");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_iis_status");
        });

        // === IqcSupplierQualityStat ===
        modelBuilder.Entity<IqcSupplierQualityStat>(e =>
        {
            e.ToTable("iqc_supplier_quality_stat");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.SupplierId).HasColumnName("supplier_id").HasMaxLength(50);
            e.Property(x => x.SupplierName).HasColumnName("supplier_name").HasMaxLength(100);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.TotalBatches).HasColumnName("total_batches");
            e.Property(x => x.PassBatches).HasColumnName("pass_batches");
            e.Property(x => x.FailBatches).HasColumnName("fail_batches");
            e.Property(x => x.ReturnBatches).HasColumnName("return_batches");
            e.Property(x => x.PassRate).HasColumnName("pass_rate").HasPrecision(5, 2);
            e.Property(x => x.TotalDefects).HasColumnName("total_defects");
            e.Property(x => x.TopDefectType).HasColumnName("top_defect_type").HasMaxLength(50);
            e.Property(x => x.EvaluationPeriod).HasColumnName("evaluation_period").HasMaxLength(20);
            e.Property(x => x.QualityGrade).HasColumnName("quality_grade").HasMaxLength(20);
            e.Property(x => x.StatDate).HasColumnName("stat_date");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.SupplierId).HasDatabaseName("idx_isqs_supplier");
            e.HasIndex(x => x.StatDate).HasDatabaseName("idx_isqs_date");
        });

        // === FqcInspectionRecord ===
        modelBuilder.Entity<FqcInspectionRecord>(e =>
        {
            e.ToTable("fqc_inspection_record");
            e.HasKey(x => x.RecordId);
            e.Property(x => x.RecordId).HasColumnName("record_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.InspectionQty).HasColumnName("inspection_qty");
            e.Property(x => x.PassQty).HasColumnName("pass_qty");
            e.Property(x => x.FailQty).HasColumnName("fail_qty");
            e.Property(x => x.Judgment).HasColumnName("judgment").HasMaxLength(20);
            e.Property(x => x.InspectorId).HasColumnName("inspector_id").HasMaxLength(50);
            e.Property(x => x.InspectorName).HasColumnName("inspector_name").HasMaxLength(100);
            e.Property(x => x.InspectionTime).HasColumnName("inspection_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_fir_lot");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_fir_order");
            e.HasIndex(x => x.InspectionTime).HasDatabaseName("idx_fir_time");
        });

        // === OqcInspectionRecord ===
        modelBuilder.Entity<OqcInspectionRecord>(e =>
        {
            e.ToTable("oqc_inspection_record");
            e.HasKey(x => x.RecordId);
            e.Property(x => x.RecordId).HasColumnName("record_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(100);
            e.Property(x => x.InspectionQty).HasColumnName("inspection_qty");
            e.Property(x => x.PassQty).HasColumnName("pass_qty");
            e.Property(x => x.FailQty).HasColumnName("fail_qty");
            e.Property(x => x.Judgment).HasColumnName("judgment").HasMaxLength(20);
            e.Property(x => x.InspectorId).HasColumnName("inspector_id").HasMaxLength(50);
            e.Property(x => x.InspectorName).HasColumnName("inspector_name").HasMaxLength(100);
            e.Property(x => x.InspectionTime).HasColumnName("inspection_time");
            e.Property(x => x.PackagingCheck).HasColumnName("packaging_check").HasMaxLength(20);
            e.Property(x => x.LabelCheck).HasColumnName("label_check").HasMaxLength(20);
            e.Property(x => x.MslCheck).HasColumnName("msl_check").HasMaxLength(20);
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_oir_lot");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_oir_order");
            e.HasIndex(x => x.CustomerId).HasDatabaseName("idx_oir_customer");
            e.HasIndex(x => x.InspectionTime).HasDatabaseName("idx_oir_time");
        });

        // === ShipmentMslCheck ===
        modelBuilder.Entity<ShipmentMslCheck>(e =>
        {
            e.ToTable("shipment_msl_check");
            e.HasKey(x => x.CheckId);
            e.Property(x => x.CheckId).HasColumnName("check_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.MslLevel).HasColumnName("msl_level").HasMaxLength(20);
            e.Property(x => x.BakeStartTime).HasColumnName("bake_start_time");
            e.Property(x => x.BakeEndTime).HasColumnName("bake_end_time");
            e.Property(x => x.BakeDurationHours).HasColumnName("bake_duration_hours");
            e.Property(x => x.ExposureStartTime).HasColumnName("exposure_start_time");
            e.Property(x => x.ExposureDurationHours).HasColumnName("exposure_duration_hours");
            e.Property(x => x.FloorLifeRemaining).HasColumnName("floor_life_remaining");
            e.Property(x => x.HumidityCardResult).HasColumnName("humidity_card_result").HasMaxLength(50);
            e.Property(x => x.PackagingCondition).HasColumnName("packaging_condition").HasMaxLength(100);
            e.Property(x => x.Judgment).HasColumnName("judgment").HasMaxLength(20);
            e.Property(x => x.CheckerId).HasColumnName("checker_id").HasMaxLength(50);
            e.Property(x => x.CheckerName).HasColumnName("checker_name").HasMaxLength(100);
            e.Property(x => x.CheckTime).HasColumnName("check_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_smc_lot");
            e.HasIndex(x => x.CheckTime).HasDatabaseName("idx_smc_time");
        });

        // === NonconformingRecord ===
        modelBuilder.Entity<NonconformingRecord>(e =>
        {
            e.ToTable("nonconforming_record");
            e.HasKey(x => x.NcrId);
            e.Property(x => x.NcrId).HasColumnName("ncr_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.SourceType).HasColumnName("source_type").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.DefectQty).HasColumnName("defect_qty");
            e.Property(x => x.DefectCode).HasColumnName("defect_code").HasMaxLength(50);
            e.Property(x => x.DefectDescription).HasColumnName("defect_description").HasColumnType("text");
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.Disposition).HasColumnName("disposition").HasMaxLength(50);
            e.Property(x => x.MrbReference).HasColumnName("mrb_reference").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.ReporterId).HasColumnName("reporter_id").HasMaxLength(50);
            e.Property(x => x.ReporterName).HasColumnName("reporter_name").HasMaxLength(100);
            e.Property(x => x.ReportTime).HasColumnName("report_time");
            e.Property(x => x.ResponsibleDept).HasColumnName("responsible_dept").HasMaxLength(50);
            e.Property(x => x.RootCause).HasColumnName("root_cause").HasColumnType("text");
            e.Property(x => x.CorrectiveAction).HasColumnName("corrective_action").HasColumnType("text");
            e.Property(x => x.PreventiveAction).HasColumnName("preventive_action").HasColumnType("text");
            e.Property(x => x.ClosedBy).HasColumnName("closed_by").HasMaxLength(50);
            e.Property(x => x.ClosedTime).HasColumnName("closed_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_nr_lot");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_nr_order");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_nr_status");
            e.HasIndex(x => x.Severity).HasDatabaseName("idx_nr_severity");
            e.HasIndex(x => x.ReportTime).HasDatabaseName("idx_nr_report_time");
        });

        // === MrbReview ===
        modelBuilder.Entity<MrbReview>(e =>
        {
            e.ToTable("mrb_review");
            e.HasKey(x => x.MrbId);
            e.Property(x => x.MrbId).HasColumnName("mrb_id").HasMaxLength(50);
            e.Property(x => x.NcrId).HasColumnName("ncr_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.AffectedQty).HasColumnName("affected_qty");
            e.Property(x => x.Disposition).HasColumnName("disposition").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.ReviewType).HasColumnName("review_type").HasMaxLength(50);
            e.Property(x => x.ReviewTime).HasColumnName("review_time");
            e.Property(x => x.ReviewerIds).HasColumnName("reviewer_ids").HasMaxLength(500);
            e.Property(x => x.ReviewerNames).HasColumnName("reviewer_names").HasMaxLength(500);
            e.Property(x => x.ReviewConclusion).HasColumnName("review_conclusion").HasColumnType("text");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.ApprovedTime).HasColumnName("approved_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.NcrId).HasDatabaseName("idx_mr_ncr");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_mr_lot");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_mr_status");
            e.HasIndex(x => x.ReviewTime).HasDatabaseName("idx_mr_review_time");
        });

        // === MrbReviewItem ===
        modelBuilder.Entity<MrbReviewItem>(e =>
        {
            e.ToTable("mrb_review_item");
            e.HasKey(x => x.ItemId);
            e.Property(x => x.ItemId).HasColumnName("item_id").HasMaxLength(50);
            e.Property(x => x.MrbId).HasColumnName("mrb_id").HasMaxLength(50);
            e.Property(x => x.NcrId).HasColumnName("ncr_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.AffectedQty).HasColumnName("affected_qty");
            e.Property(x => x.Disposition).HasColumnName("disposition").HasMaxLength(50);
            e.Property(x => x.DispositionDetail).HasColumnName("disposition_detail").HasColumnType("text");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.MrbId).HasDatabaseName("idx_mri_mrb");
            e.HasIndex(x => x.NcrId).HasDatabaseName("idx_mri_ncr");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_mri_lot");
        });

        // === DispositionRecord ===
        modelBuilder.Entity<DispositionRecord>(e =>
        {
            e.ToTable("disposition_record");
            e.HasKey(x => x.DispositionId);
            e.Property(x => x.DispositionId).HasColumnName("disposition_id").HasMaxLength(50);
            e.Property(x => x.MrbId).HasColumnName("mrb_id").HasMaxLength(50);
            e.Property(x => x.NcrId).HasColumnName("ncr_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.DispositionType).HasColumnName("disposition_type").HasMaxLength(50);
            e.Property(x => x.DispositionQty).HasColumnName("disposition_qty");
            e.Property(x => x.ExecutionDetail).HasColumnName("execution_detail").HasColumnType("text");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.ExecutorId).HasColumnName("executor_id").HasMaxLength(50);
            e.Property(x => x.ExecutorName).HasColumnName("executor_name").HasMaxLength(100);
            e.Property(x => x.ExecutionTime).HasColumnName("execution_time");
            e.Property(x => x.VerifierId).HasColumnName("verifier_id").HasMaxLength(50);
            e.Property(x => x.VerifierName).HasColumnName("verifier_name").HasMaxLength(100);
            e.Property(x => x.VerificationTime).HasColumnName("verification_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.MrbId).HasDatabaseName("idx_dr_mrb");
            e.HasIndex(x => x.NcrId).HasDatabaseName("idx_dr_ncr");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_dr_lot");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_dr_status");
        });
    }

    private static void ConfigureWarehouse(ModelBuilder modelBuilder)
    {
        // === WarehouseReceipt ===
        modelBuilder.Entity<WarehouseReceipt>(e =>
        {
            e.ToTable("warehouse_receipt");
            e.HasKey(x => x.ReceiptId);
            e.Property(x => x.ReceiptId).HasColumnName("receipt_id").HasMaxLength(50);
            e.Property(x => x.PoNumber).HasColumnName("po_number").HasMaxLength(50);
            e.Property(x => x.SupplierId).HasColumnName("supplier_id").HasMaxLength(50);
            e.Property(x => x.SupplierName).HasColumnName("supplier_name").HasMaxLength(100);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(100);
            e.Property(x => x.MaterialSpec).HasColumnName("material_spec").HasMaxLength(255);
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.LotNumber).HasColumnName("lot_number").HasMaxLength(50);
            e.Property(x => x.ReceivedDate).HasColumnName("received_date");
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.LocationId).HasColumnName("location_id").HasMaxLength(50);
            e.Property(x => x.ReceiptType).HasColumnName("receipt_type").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.IqcBatchId).HasColumnName("iqc_batch_id").HasMaxLength(50);
            e.Property(x => x.IqcStatus).HasColumnName("iqc_status").HasMaxLength(20);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.PoNumber).HasDatabaseName("idx_wr_po");
            e.HasIndex(x => x.SupplierId).HasDatabaseName("idx_wr_supplier");
            e.HasIndex(x => x.MaterialId).HasDatabaseName("idx_wr_material");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_wr_status");
            e.HasIndex(x => x.ReceivedDate).HasDatabaseName("idx_wr_received");
        });

        // === WarehouseInventory ===
        modelBuilder.Entity<WarehouseInventory>(e =>
        {
            e.ToTable("warehouse_inventory");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.LocationId).HasColumnName("location_id").HasMaxLength(50);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(100);
            e.Property(x => x.MaterialSpec).HasColumnName("material_spec").HasMaxLength(255);
            e.Property(x => x.LotNumber).HasColumnName("lot_number").HasMaxLength(50);
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.ReservedQty).HasColumnName("reserved_qty");
            e.Property(x => x.AvailableQty).HasColumnName("available_qty");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.BatchId).HasColumnName("batch_id").HasMaxLength(50);
            e.Property(x => x.SupplierId).HasColumnName("supplier_id").HasMaxLength(50);
            e.Property(x => x.ReceivedDate).HasColumnName("received_date");
            e.Property(x => x.ExpiryDate).HasColumnName("expiry_date");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.WarehouseId).HasDatabaseName("idx_wi_warehouse");
            e.HasIndex(x => x.MaterialId).HasDatabaseName("idx_wi_material");
            e.HasIndex(x => x.LotNumber).HasDatabaseName("idx_wi_lot");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_wi_status");
            e.HasIndex(x => x.ExpiryDate).HasDatabaseName("idx_wi_expiry");
        });

        // === WarehouseLocation ===
        modelBuilder.Entity<WarehouseLocation>(e =>
        {
            e.ToTable("warehouse_location");
            e.HasKey(x => x.LocationId);
            e.Property(x => x.LocationId).HasColumnName("location_id").HasMaxLength(50);
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.WarehouseName).HasColumnName("warehouse_name").HasMaxLength(100);
            e.Property(x => x.Zone).HasColumnName("zone").HasMaxLength(50);
            e.Property(x => x.Aisle).HasColumnName("aisle").HasMaxLength(50);
            e.Property(x => x.Rack).HasColumnName("rack").HasMaxLength(50);
            e.Property(x => x.Level).HasColumnName("level").HasMaxLength(50);
            e.Property(x => x.Bin).HasColumnName("bin").HasMaxLength(50);
            e.Property(x => x.FullPath).HasColumnName("full_path").HasMaxLength(255);
            e.Property(x => x.LocationType).HasColumnName("location_type").HasMaxLength(50);
            e.Property(x => x.MaxCapacity).HasColumnName("max_capacity");
            e.Property(x => x.CurrentCapacity).HasColumnName("current_capacity");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Temperature).HasColumnName("temperature").HasMaxLength(50);
            e.Property(x => x.Humidity).HasColumnName("humidity").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.WarehouseId).HasDatabaseName("idx_wl_warehouse");
            e.HasIndex(x => x.FullPath).IsUnique().HasDatabaseName("uk_wl_full_path");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_wl_status");
        });

        // === MaterialShelfLife ===
        modelBuilder.Entity<MaterialShelfLife>(e =>
        {
            e.ToTable("material_shelf_life");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(100);
            e.Property(x => x.ShelfLifeDays).HasColumnName("shelf_life_days");
            e.Property(x => x.OpenShelfLifeHours).HasColumnName("open_shelf_life_hours");
            e.Property(x => x.StorageCondition).HasColumnName("storage_condition").HasMaxLength(255);
            e.Property(x => x.ManufactureDate).HasColumnName("manufacture_date");
            e.Property(x => x.ExpiryDate).HasColumnName("expiry_date");
            e.Property(x => x.LotNumber).HasColumnName("lot_number").HasMaxLength(50);
            e.Property(x => x.SupplierId).HasColumnName("supplier_id").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.MaterialId).HasDatabaseName("idx_msl_material");
            e.HasIndex(x => x.ExpiryDate).HasDatabaseName("idx_msl_expiry");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_msl_status");
        });

        // === WarehouseIssueOrder ===
        modelBuilder.Entity<WarehouseIssueOrder>(e =>
        {
            e.ToTable("warehouse_issue_order");
            e.HasKey(x => x.IssueOrderId);
            e.Property(x => x.IssueOrderId).HasColumnName("issue_order_id").HasMaxLength(50);
            e.Property(x => x.WorkOrderId).HasColumnName("work_order_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.IssueType).HasColumnName("issue_type").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Destination).HasColumnName("destination").HasMaxLength(100);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.IssueTime).HasColumnName("issue_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.WorkOrderId).HasDatabaseName("idx_wio_work_order");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_wio_status");
            e.HasIndex(x => x.IssueTime).HasDatabaseName("idx_wio_issue_time");
        });

        // === WarehouseIssueItem ===
        modelBuilder.Entity<WarehouseIssueItem>(e =>
        {
            e.ToTable("warehouse_issue_item");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.IssueOrderId).HasColumnName("issue_order_id").HasMaxLength(50);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(100);
            e.Property(x => x.LotNumber).HasColumnName("lot_number").HasMaxLength(50);
            e.Property(x => x.RequestQty).HasColumnName("request_qty");
            e.Property(x => x.ActualQty).HasColumnName("actual_qty");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.LocationId).HasColumnName("location_id").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.IssueOrderId).HasDatabaseName("idx_wii_order");
            e.HasIndex(x => x.MaterialId).HasDatabaseName("idx_wii_material");
        });

        // === WarehouseReturnOrder ===
        modelBuilder.Entity<WarehouseReturnOrder>(e =>
        {
            e.ToTable("warehouse_return_order");
            e.HasKey(x => x.ReturnOrderId);
            e.Property(x => x.ReturnOrderId).HasColumnName("return_order_id").HasMaxLength(50);
            e.Property(x => x.WorkOrderId).HasColumnName("work_order_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.ReturnType).HasColumnName("return_type").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(255);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.ReturnTime).HasColumnName("return_time");
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.WorkOrderId).HasDatabaseName("idx_wro_work_order");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_wro_status");
            e.HasIndex(x => x.ReturnTime).HasDatabaseName("idx_wro_return_time");
        });

        // === WarehouseReturnItem ===
        modelBuilder.Entity<WarehouseReturnItem>(e =>
        {
            e.ToTable("warehouse_return_item");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ReturnOrderId).HasColumnName("return_order_id").HasMaxLength(50);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(100);
            e.Property(x => x.LotNumber).HasColumnName("lot_number").HasMaxLength(50);
            e.Property(x => x.ReturnQty).HasColumnName("return_qty");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.LocationId).HasColumnName("location_id").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.ReturnOrderId).HasDatabaseName("idx_wri_order");
            e.HasIndex(x => x.MaterialId).HasDatabaseName("idx_wri_material");
        });

        // === FinishedGoodsReceipt ===
        modelBuilder.Entity<FinishedGoodsReceipt>(e =>
        {
            e.ToTable("finished_goods_receipt");
            e.HasKey(x => x.ReceiptId);
            e.Property(x => x.ReceiptId).HasColumnName("receipt_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.LocationId).HasColumnName("location_id").HasMaxLength(50);
            e.Property(x => x.ReceiptTime).HasColumnName("receipt_time");
            e.Property(x => x.FqcRecordId).HasColumnName("fqc_record_id").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_fgr_lot");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_fgr_order");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_fgr_status");
            e.HasIndex(x => x.ReceiptTime).HasDatabaseName("idx_fgr_receipt");
        });

        // === FinishedGoodsInventory ===
        modelBuilder.Entity<FinishedGoodsInventory>(e =>
        {
            e.ToTable("finished_goods_inventory");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.LocationId).HasColumnName("location_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(100);
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.ReservedQty).HasColumnName("reserved_qty");
            e.Property(x => x.AvailableQty).HasColumnName("available_qty");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.ProducedDate).HasColumnName("produced_date");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.WarehouseId).HasDatabaseName("idx_fgi_warehouse");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_fgi_lot");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_fgi_order");
            e.HasIndex(x => x.ProductId).HasDatabaseName("idx_fgi_product");
            e.HasIndex(x => x.CustomerId).HasDatabaseName("idx_fgi_customer");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_fgi_status");
        });

        // === FinishedGoodsShipment ===
        modelBuilder.Entity<FinishedGoodsShipment>(e =>
        {
            e.ToTable("finished_goods_shipment");
            e.HasKey(x => x.ShipmentId);
            e.Property(x => x.ShipmentId).HasColumnName("shipment_id").HasMaxLength(50);
            e.Property(x => x.SalesOrderId).HasColumnName("sales_order_id").HasMaxLength(50);
            e.Property(x => x.DeliveryNoteId).HasColumnName("delivery_note_id").HasMaxLength(50);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(100);
            e.Property(x => x.ShipmentType).HasColumnName("shipment_type").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.ShipmentTime).HasColumnName("shipment_time");
            e.Property(x => x.Carrier).HasColumnName("carrier").HasMaxLength(100);
            e.Property(x => x.TrackingNumber).HasColumnName("tracking_number").HasMaxLength(100);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.SalesOrderId).HasDatabaseName("idx_fgs_sales_order");
            e.HasIndex(x => x.CustomerId).HasDatabaseName("idx_fgs_customer");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_fgs_status");
            e.HasIndex(x => x.ShipmentTime).HasDatabaseName("idx_fgs_shipment");
        });

        // === FinishedGoodsShipmentItem ===
        modelBuilder.Entity<FinishedGoodsShipmentItem>(e =>
        {
            e.ToTable("finished_goods_shipment_item");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ShipmentId).HasColumnName("shipment_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.LocationId).HasColumnName("location_id").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.ShipmentId).HasDatabaseName("idx_fgsi_shipment");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_fgsi_lot");
            e.HasIndex(x => x.ProductId).HasDatabaseName("idx_fgsi_product");
        });
    }

    private static void ConfigureAbnormalEquipmentAlert(ModelBuilder modelBuilder)
    {
        // === AbnormalRecord ===
        modelBuilder.Entity<AbnormalRecord>(e =>
        {
            e.ToTable("abnormal_record");
            e.HasKey(x => x.AbnormalId);
            e.Property(x => x.AbnormalId).HasColumnName("abnormal_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.AbnormalType).HasColumnName("abnormal_type").HasMaxLength(50);
            e.Property(x => x.AbnormalCategory).HasColumnName("abnormal_category").HasMaxLength(50);
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.ReporterId).HasColumnName("reporter_id").HasMaxLength(50);
            e.Property(x => x.ReporterName).HasColumnName("reporter_name").HasMaxLength(100);
            e.Property(x => x.ReportTime).HasColumnName("report_time");
            e.Property(x => x.ResponsibleDept).HasColumnName("responsible_dept").HasMaxLength(50);
            e.Property(x => x.ResponsiblePerson).HasColumnName("responsible_person").HasMaxLength(100);
            e.Property(x => x.RootCause).HasColumnName("root_cause").HasColumnType("text");
            e.Property(x => x.CorrectiveAction).HasColumnName("corrective_action").HasColumnType("text");
            e.Property(x => x.PreventiveAction).HasColumnName("preventive_action").HasColumnType("text");
            e.Property(x => x.ClosedBy).HasColumnName("closed_by").HasMaxLength(50);
            e.Property(x => x.ClosedTime).HasColumnName("closed_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_ar_lot");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_ar_order");
            e.HasIndex(x => x.AbnormalType).HasDatabaseName("idx_ar_type");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_ar_status");
            e.HasIndex(x => x.Severity).HasDatabaseName("idx_ar_severity");
            e.HasIndex(x => x.ReportTime).HasDatabaseName("idx_ar_report_time");
        });

        // === LineStopRecord ===
        modelBuilder.Entity<LineStopRecord>(e =>
        {
            e.ToTable("line_stop_record");
            e.HasKey(x => x.StopId);
            e.Property(x => x.StopId).HasColumnName("stop_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.LineId).HasColumnName("line_id").HasMaxLength(50);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.StopType).HasColumnName("stop_type").HasMaxLength(50);
            e.Property(x => x.StopReason).HasColumnName("stop_reason").HasColumnType("text");
            e.Property(x => x.ReasonCode).HasColumnName("reason_code").HasMaxLength(50);
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.StopTime).HasColumnName("stop_time");
            e.Property(x => x.ResumeTime).HasColumnName("resume_time");
            e.Property(x => x.StopDurationMinutes).HasColumnName("stop_duration_minutes");
            e.Property(x => x.ReportedBy).HasColumnName("reported_by").HasMaxLength(50);
            e.Property(x => x.ReportedByName).HasColumnName("reported_by_name").HasMaxLength(100);
            e.Property(x => x.ResponsibleDept).HasColumnName("responsible_dept").HasMaxLength(50);
            e.Property(x => x.Action).HasColumnName("action").HasColumnType("text");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.ClosedBy).HasColumnName("closed_by").HasMaxLength(50);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.LineId).HasDatabaseName("idx_lsr_line");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_lsr_status");
            e.HasIndex(x => x.StopTime).HasDatabaseName("idx_lsr_stop_time");
        });

        // === EquipmentFaultRecord ===
        modelBuilder.Entity<EquipmentFaultRecord>(e =>
        {
            e.ToTable("equipment_fault_record");
            e.HasKey(x => x.FaultId);
            e.Property(x => x.FaultId).HasColumnName("fault_id").HasMaxLength(50);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.EquipmentName).HasColumnName("equipment_name").HasMaxLength(100);
            e.Property(x => x.FaultType).HasColumnName("fault_type").HasMaxLength(50);
            e.Property(x => x.FaultDescription).HasColumnName("fault_description").HasColumnType("text");
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.FaultTime).HasColumnName("fault_time");
            e.Property(x => x.RepairStartTime).HasColumnName("repair_start_time");
            e.Property(x => x.RepairEndTime).HasColumnName("repair_end_time");
            e.Property(x => x.RepairDurationMinutes).HasColumnName("repair_duration_minutes");
            e.Property(x => x.ReportedBy).HasColumnName("reported_by").HasMaxLength(50);
            e.Property(x => x.ReportedByName).HasColumnName("reported_by_name").HasMaxLength(100);
            e.Property(x => x.RepairBy).HasColumnName("repair_by").HasMaxLength(50);
            e.Property(x => x.RepairByName).HasColumnName("repair_by_name").HasMaxLength(100);
            e.Property(x => x.RootCause).HasColumnName("root_cause").HasColumnType("text");
            e.Property(x => x.RepairAction).HasColumnName("repair_action").HasColumnType("text");
            e.Property(x => x.ReplacedParts).HasColumnName("replaced_parts").HasMaxLength(500);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.EquipmentId).HasDatabaseName("idx_efr_equipment");
            e.HasIndex(x => x.FaultType).HasDatabaseName("idx_efr_type");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_efr_status");
            e.HasIndex(x => x.FaultTime).HasDatabaseName("idx_efr_fault_time");
        });

        // === EquipmentRepairSparePart (auto_increment BIGINT PK) ===
        modelBuilder.Entity<EquipmentRepairSparePart>(e =>
        {
            e.ToTable("equipment_repair_spare_part");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.FaultId).HasColumnName("fault_id").HasMaxLength(50);
            e.Property(x => x.SparePartId).HasColumnName("spare_part_id").HasMaxLength(50);
            e.Property(x => x.SparePartName).HasColumnName("spare_part_name").HasMaxLength(100);
            e.Property(x => x.PartNumber).HasColumnName("part_number").HasMaxLength(50);
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.FaultId).HasDatabaseName("idx_ersp_fault");
            e.HasIndex(x => x.SparePartId).HasDatabaseName("idx_ersp_part");
        });

        // === EquipmentPmPlan ===
        modelBuilder.Entity<EquipmentPmPlan>(e =>
        {
            e.ToTable("equipment_pm_plan");
            e.HasKey(x => x.PmPlanId);
            e.Property(x => x.PmPlanId).HasColumnName("pm_plan_id").HasMaxLength(50);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.EquipmentName).HasColumnName("equipment_name").HasMaxLength(100);
            e.Property(x => x.PmType).HasColumnName("pm_type").HasMaxLength(50);
            e.Property(x => x.PmName).HasColumnName("pm_name").HasMaxLength(100);
            e.Property(x => x.PmDescription).HasColumnName("pm_description").HasColumnType("text");
            e.Property(x => x.CycleDays).HasColumnName("cycle_days");
            e.Property(x => x.CycleType).HasColumnName("cycle_type").HasMaxLength(20);
            e.Property(x => x.NextDueDate).HasColumnName("next_due_date");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.EquipmentId).HasDatabaseName("idx_epp_equipment");
            e.HasIndex(x => x.PmType).HasDatabaseName("idx_epp_type");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_epp_status");
            e.HasIndex(x => x.NextDueDate).HasDatabaseName("idx_epp_next_due");
        });

        // === EquipmentPmExecution ===
        modelBuilder.Entity<EquipmentPmExecution>(e =>
        {
            e.ToTable("equipment_pm_execution");
            e.HasKey(x => x.PmExecutionId);
            e.Property(x => x.PmExecutionId).HasColumnName("pm_execution_id").HasMaxLength(50);
            e.Property(x => x.PmPlanId).HasColumnName("pm_plan_id").HasMaxLength(50);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.EquipmentName).HasColumnName("equipment_name").HasMaxLength(100);
            e.Property(x => x.PmType).HasColumnName("pm_type").HasMaxLength(50);
            e.Property(x => x.PmContent).HasColumnName("pm_content").HasColumnType("text");
            e.Property(x => x.ScheduledDate).HasColumnName("scheduled_date");
            e.Property(x => x.ActualStartTime).HasColumnName("actual_start_time");
            e.Property(x => x.ActualEndTime).HasColumnName("actual_end_time");
            e.Property(x => x.ActualDurationMinutes).HasColumnName("actual_duration_minutes");
            e.Property(x => x.ExecutedBy).HasColumnName("executed_by").HasMaxLength(50);
            e.Property(x => x.ExecutedByName).HasColumnName("executed_by_name").HasMaxLength(100);
            e.Property(x => x.Result).HasColumnName("result").HasMaxLength(20);
            e.Property(x => x.IssuesFound).HasColumnName("issues_found").HasColumnType("text");
            e.Property(x => x.FollowUpAction).HasColumnName("follow_up_action").HasColumnType("text");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.PmPlanId).HasDatabaseName("idx_epe_plan");
            e.HasIndex(x => x.EquipmentId).HasDatabaseName("idx_epe_equipment");
            e.HasIndex(x => x.ScheduledDate).HasDatabaseName("idx_epe_scheduled");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_epe_status");
        });

        // === FirstArticleInspection ===
        modelBuilder.Entity<FirstArticleInspection>(e =>
        {
            e.ToTable("first_article_inspection");
            e.HasKey(x => x.FaId);
            e.Property(x => x.FaId).HasColumnName("fa_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepName).HasColumnName("step_name").HasMaxLength(100);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.InspectionQty).HasColumnName("inspection_qty");
            e.Property(x => x.Judgment).HasColumnName("judgment").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.InspectorId).HasColumnName("inspector_id").HasMaxLength(50);
            e.Property(x => x.InspectorName).HasColumnName("inspector_name").HasMaxLength(100);
            e.Property(x => x.InspectionTime).HasColumnName("inspection_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_fai_lot");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_fai_order");
            e.HasIndex(x => x.StepCode).HasDatabaseName("idx_fai_step");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_fai_status");
            e.HasIndex(x => x.InspectionTime).HasDatabaseName("idx_fai_inspection_time");
        });

        // === FirstArticleInspectionItem (auto_increment BIGINT PK) ===
        modelBuilder.Entity<FirstArticleInspectionItem>(e =>
        {
            e.ToTable("first_article_inspection_item");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.FaId).HasColumnName("fa_id").HasMaxLength(50);
            e.Property(x => x.InspectionItem).HasColumnName("inspection_item").HasMaxLength(100);
            e.Property(x => x.InspectionMethod).HasColumnName("inspection_method").HasMaxLength(255);
            e.Property(x => x.StandardValue).HasColumnName("standard_value").HasMaxLength(100);
            e.Property(x => x.UpperLimit).HasColumnName("upper_limit").HasMaxLength(50);
            e.Property(x => x.LowerLimit).HasColumnName("lower_limit").HasMaxLength(50);
            e.Property(x => x.ActualValue).HasColumnName("actual_value").HasMaxLength(100);
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.Judgment).HasColumnName("judgment").HasMaxLength(20);
            e.Property(x => x.DefectDescription).HasColumnName("defect_description").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.FaId).HasDatabaseName("idx_faitem_fa");
        });

        // === FirstArticleSignature ===
        modelBuilder.Entity<FirstArticleSignature>(e =>
        {
            e.ToTable("first_article_signature");
            e.HasKey(x => x.SignatureId);
            e.Property(x => x.SignatureId).HasColumnName("signature_id").HasMaxLength(50);
            e.Property(x => x.FaId).HasColumnName("fa_id").HasMaxLength(50);
            e.Property(x => x.SignerId).HasColumnName("signer_id").HasMaxLength(50);
            e.Property(x => x.SignerName).HasColumnName("signer_name").HasMaxLength(100);
            e.Property(x => x.SignerRole).HasColumnName("signer_role").HasMaxLength(50);
            e.Property(x => x.SignatureType).HasColumnName("signature_type").HasMaxLength(50);
            e.Property(x => x.SignatureData).HasColumnName("signature_data").HasColumnType("json");
            e.Property(x => x.SignTime).HasColumnName("sign_time");
            e.Property(x => x.Comment).HasColumnName("comment").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.FaId).HasDatabaseName("idx_fas_fa");
            e.HasIndex(x => x.SignerId).HasDatabaseName("idx_fas_signer");
            e.HasIndex(x => x.SignTime).HasDatabaseName("idx_fas_sign_time");
        });

        // === BondPullTestRecord ===
        modelBuilder.Entity<BondPullTestRecord>(e =>
        {
            e.ToTable("bond_pull_test_record");
            e.HasKey(x => x.TestId);
            e.Property(x => x.TestId).HasColumnName("test_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.BondWireType).HasColumnName("bond_wire_type").HasMaxLength(50);
            e.Property(x => x.SampleSize).HasColumnName("sample_size");
            e.Property(x => x.StandardValue).HasColumnName("standard_value").HasMaxLength(50);
            e.Property(x => x.UpperLimit).HasColumnName("upper_limit").HasMaxLength(50);
            e.Property(x => x.LowerLimit).HasColumnName("lower_limit").HasMaxLength(50);
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.AvgValue).HasColumnName("avg_value").HasPrecision(10, 4);
            e.Property(x => x.MinValue).HasColumnName("min_value").HasPrecision(10, 4);
            e.Property(x => x.MaxValue).HasColumnName("max_value").HasPrecision(10, 4);
            e.Property(x => x.Judgment).HasColumnName("judgment").HasMaxLength(20);
            e.Property(x => x.TesterId).HasColumnName("tester_id").HasMaxLength(50);
            e.Property(x => x.TesterName).HasColumnName("tester_name").HasMaxLength(100);
            e.Property(x => x.TestTime).HasColumnName("test_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_bptr_lot");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_bptr_order");
            e.HasIndex(x => x.TestTime).HasDatabaseName("idx_bptr_test_time");
        });

        // === QualityAlert ===
        modelBuilder.Entity<QualityAlert>(e =>
        {
            e.ToTable("quality_alert");
            e.HasKey(x => x.AlertId);
            e.Property(x => x.AlertId).HasColumnName("alert_id").HasMaxLength(50);
            e.Property(x => x.AlertType).HasColumnName("alert_type").HasMaxLength(50);
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.Title).HasColumnName("title").HasMaxLength(200);
            e.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.SupplierId).HasColumnName("supplier_id").HasMaxLength(50);
            e.Property(x => x.NcrId).HasColumnName("ncr_id").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.IssuedBy).HasColumnName("issued_by").HasMaxLength(50);
            e.Property(x => x.IssuedByName).HasColumnName("issued_by_name").HasMaxLength(100);
            e.Property(x => x.IssuedTime).HasColumnName("issued_time");
            e.Property(x => x.ClosedBy).HasColumnName("closed_by").HasMaxLength(50);
            e.Property(x => x.ClosedTime).HasColumnName("closed_time");
            e.Property(x => x.CorrectiveAction).HasColumnName("corrective_action").HasColumnType("text");
            e.Property(x => x.PreventiveAction).HasColumnName("preventive_action").HasColumnType("text");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.AlertType).HasDatabaseName("idx_qa_type");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_qa_status");
            e.HasIndex(x => x.Severity).HasDatabaseName("idx_qa_severity");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_qa_lot");
            e.HasIndex(x => x.NcrId).HasDatabaseName("idx_qa_ncr");
        });

        // === QualityAlertAffectedLot (auto_increment BIGINT PK) ===
        modelBuilder.Entity<QualityAlertAffectedLot>(e =>
        {
            e.ToTable("quality_alert_affected_lot");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.AlertId).HasColumnName("alert_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.AffectedQty).HasColumnName("affected_qty");
            e.Property(x => x.Disposition).HasColumnName("disposition").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.AlertId).HasDatabaseName("idx_qaal_alert");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_qaal_lot");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_qaal_status");
        });

        // === RecallNotice ===
        modelBuilder.Entity<RecallNotice>(e =>
        {
            e.ToTable("recall_notice");
            e.HasKey(x => x.RecallId);
            e.Property(x => x.RecallId).HasColumnName("recall_id").HasMaxLength(50);
            e.Property(x => x.NoticeNumber).HasColumnName("notice_number").HasMaxLength(50);
            e.Property(x => x.RecallType).HasColumnName("recall_type").HasMaxLength(50);
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.Reason).HasColumnName("reason").HasColumnType("text");
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(100);
            e.Property(x => x.RecallStartDate).HasColumnName("recall_start_date");
            e.Property(x => x.RecallEndDate).HasColumnName("recall_end_date");
            e.Property(x => x.TotalAffectedQty).HasColumnName("total_affected_qty");
            e.Property(x => x.RecalledQty).HasColumnName("recalled_qty");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.IssuedBy).HasColumnName("issued_by").HasMaxLength(50);
            e.Property(x => x.IssuedByName).HasColumnName("issued_by_name").HasMaxLength(100);
            e.Property(x => x.IssuedTime).HasColumnName("issued_time");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.ApprovedTime).HasColumnName("approved_time");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.NoticeNumber).IsUnique().HasDatabaseName("uk_rn_notice_number");
            e.HasIndex(x => x.RecallType).HasDatabaseName("idx_rn_type");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_rn_status");
            e.HasIndex(x => x.CustomerId).HasDatabaseName("idx_rn_customer");
        });

        // === RecallNoticeItem (auto_increment BIGINT PK) ===
        modelBuilder.Entity<RecallNoticeItem>(e =>
        {
            e.ToTable("recall_notice_item");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            e.Property(x => x.RecallId).HasColumnName("recall_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.AffectedQty).HasColumnName("affected_qty");
            e.Property(x => x.RecalledQty).HasColumnName("recalled_qty");
            e.Property(x => x.WarehouseId).HasColumnName("warehouse_id").HasMaxLength(50);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.ShipmentId).HasColumnName("shipment_id").HasMaxLength(50);
            e.Property(x => x.Disposition).HasColumnName("disposition").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.RecallId).HasDatabaseName("idx_rni_recall");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_rni_lot");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_rni_status");
        });
    }
}
