using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Infrastructure.Persistence;

/// <summary>
/// EF Core entity configurations for all Phase 3 entities.
/// Call Phase3EntityConfigurations.Apply(modelBuilder) from MesDbContext.OnModelCreating.
/// </summary>
public static class Phase3EntityConfigurations
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        ConfigureProcessParams(modelBuilder);
        ConfigureCuringCurve(modelBuilder);
        ConfigureBinControl(modelBuilder);
        ConfigureWireMaterial(modelBuilder);
        ConfigureTooling(modelBuilder);
        ConfigureOperatorQualification(modelBuilder);
        // BondPullTestRecord already configured in Phase 1 (ProductionAbnormalEntities)
    }

    private static void ConfigureProcessParams(ModelBuilder modelBuilder)
    {
        // === ProcessParameterSet ===
        modelBuilder.Entity<ProcessParameterSet>(e =>
        {
            e.ToTable("process_parameter_set");
            e.HasKey(x => x.ParameterSetId);
            e.Property(x => x.ParameterSetId).HasColumnName("parameter_set_id").HasMaxLength(50);
            e.Property(x => x.ProcessCode).HasColumnName("process_code").HasMaxLength(50);
            e.Property(x => x.ProcessName).HasColumnName("process_name").HasMaxLength(100);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(200);
            e.Property(x => x.EquipmentType).HasColumnName("equipment_type").HasMaxLength(50);
            e.Property(x => x.Version).HasColumnName("version").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.EffectiveDate).HasColumnName("effective_date");
            e.Property(x => x.ExpiryDate).HasColumnName("expiry_date");
            e.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            e.Property(x => x.ItemCount).HasColumnName("item_count");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.Deleted).HasColumnName("deleted");
            e.HasIndex(x => new { x.ProcessCode, x.ProductId }).HasDatabaseName("idx_pps_process_product");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_pps_status");
            e.HasIndex(x => x.EquipmentType).HasDatabaseName("idx_pps_equip_type");
        });

        // === ProcessParameterItem ===
        modelBuilder.Entity<ProcessParameterItem>(e =>
        {
            e.ToTable("process_parameter_item");
            e.HasKey(x => x.ItemId);
            e.Property(x => x.ItemId).HasColumnName("item_id").ValueGeneratedOnAdd();
            e.Property(x => x.ParameterSetId).HasColumnName("parameter_set_id").HasMaxLength(50);
            e.Property(x => x.ParameterCode).HasColumnName("parameter_code").HasMaxLength(50);
            e.Property(x => x.ParameterName).HasColumnName("parameter_name").HasMaxLength(100);
            e.Property(x => x.ParameterType).HasColumnName("parameter_type").HasMaxLength(20);
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.StandardValue).HasColumnName("standard_value").HasPrecision(10, 4);
            e.Property(x => x.LowerLimit).HasColumnName("lower_limit").HasPrecision(10, 4);
            e.Property(x => x.UpperLimit).HasColumnName("upper_limit").HasPrecision(10, 4);
            e.Property(x => x.TargetValue).HasColumnName("target_value").HasPrecision(10, 4);
            e.Property(x => x.WarningLowerLimit).HasColumnName("warning_lower_limit").HasPrecision(10, 4);
            e.Property(x => x.WarningUpperLimit).HasColumnName("warning_upper_limit").HasPrecision(10, 4);
            e.Property(x => x.IsRequired).HasColumnName("is_required");
            e.Property(x => x.IsAutoCollect).HasColumnName("is_auto_collect");
            e.Property(x => x.DefaultValue).HasColumnName("default_value").HasMaxLength(100);
            e.Property(x => x.ValidationRule).HasColumnName("validation_rule").HasColumnType("text");
            e.Property(x => x.SortOrder).HasColumnName("sort_order");
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.ParameterSetId).HasDatabaseName("idx_ppi_set");
            e.HasIndex(x => x.ParameterCode).HasDatabaseName("idx_ppi_param_code");
        });

        // === ProcessParameterOverrideLog ===
        modelBuilder.Entity<ProcessParameterOverrideLog>(e =>
        {
            e.ToTable("process_parameter_override_log");
            e.HasKey(x => x.LogId);
            e.Property(x => x.LogId).HasColumnName("log_id").ValueGeneratedOnAdd();
            e.Property(x => x.ParameterSetId).HasColumnName("parameter_set_id").HasMaxLength(50);
            e.Property(x => x.ItemId).HasColumnName("item_id");
            e.Property(x => x.ParameterCode).HasColumnName("parameter_code").HasMaxLength(50);
            e.Property(x => x.ParameterName).HasColumnName("parameter_name").HasMaxLength(100);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.OriginalValue).HasColumnName("original_value").HasPrecision(10, 4);
            e.Property(x => x.NewValue).HasColumnName("new_value").HasPrecision(10, 4);
            e.Property(x => x.OriginalLowerLimit).HasColumnName("original_lower_limit").HasMaxLength(50);
            e.Property(x => x.OriginalUpperLimit).HasColumnName("original_upper_limit").HasMaxLength(50);
            e.Property(x => x.NewLowerLimit).HasColumnName("new_lower_limit").HasMaxLength(50);
            e.Property(x => x.NewUpperLimit).HasColumnName("new_upper_limit").HasMaxLength(50);
            e.Property(x => x.OverrideType).HasColumnName("override_type").HasMaxLength(20);
            e.Property(x => x.Reason).HasColumnName("reason").HasColumnType("text");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.ApprovedAt).HasColumnName("approved_at");
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.OverrideTime).HasColumnName("override_time");
            e.HasIndex(x => x.ParameterSetId).HasDatabaseName("idx_ppol_set");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_ppol_lot");
            e.HasIndex(x => x.OverrideTime).HasDatabaseName("idx_ppol_time");
        });
    }

    private static void ConfigureCuringCurve(ModelBuilder modelBuilder)
    {
        // === CuringTemperatureCurve ===
        modelBuilder.Entity<CuringTemperatureCurve>(e =>
        {
            e.ToTable("curing_temperature_curve");
            e.HasKey(x => x.CurveId);
            e.Property(x => x.CurveId).HasColumnName("curve_id").HasMaxLength(50);
            e.Property(x => x.CurveName).HasColumnName("curve_name").HasMaxLength(200);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(200);
            e.Property(x => x.GlueType).HasColumnName("glue_type").HasMaxLength(50);
            e.Property(x => x.EquipmentType).HasColumnName("equipment_type").HasMaxLength(50);
            e.Property(x => x.Version).HasColumnName("version").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.TotalZones).HasColumnName("total_zones");
            e.Property(x => x.PreheatTemp).HasColumnName("preheat_temp").HasPrecision(8, 2);
            e.Property(x => x.PreheatDuration).HasColumnName("preheat_duration").HasPrecision(8, 2);
            e.Property(x => x.CuringTemp).HasColumnName("curing_temp").HasPrecision(8, 2);
            e.Property(x => x.CuringDuration).HasColumnName("curing_duration").HasPrecision(8, 2);
            e.Property(x => x.CoolingTemp).HasColumnName("cooling_temp").HasPrecision(8, 2);
            e.Property(x => x.CoolingDuration).HasColumnName("cooling_duration").HasPrecision(8, 2);
            e.Property(x => x.RampUpRate).HasColumnName("ramp_up_rate").HasPrecision(8, 2);
            e.Property(x => x.RampDownRate).HasColumnName("ramp_down_rate").HasPrecision(8, 2);
            e.Property(x => x.ZoneTemperatures).HasColumnName("zone_temperatures").HasColumnType("json");
            e.Property(x => x.ProfileData).HasColumnName("profile_data").HasColumnType("json");
            e.Property(x => x.EffectiveDate).HasColumnName("effective_date");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.Deleted).HasColumnName("deleted");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_ctc_status");
            e.HasIndex(x => x.ProductId).HasDatabaseName("idx_ctc_product");
            e.HasIndex(x => x.EquipmentType).HasDatabaseName("idx_ctc_equip_type");
        });
    }

    private static void ConfigureBinControl(ModelBuilder modelBuilder)
    {
        // === BinDefinition ===
        modelBuilder.Entity<BinDefinition>(e =>
        {
            e.ToTable("bin_definition");
            e.HasKey(x => x.BinId);
            e.Property(x => x.BinId).HasColumnName("bin_id").HasMaxLength(50);
            e.Property(x => x.BinCode).HasColumnName("bin_code").HasMaxLength(20);
            e.Property(x => x.BinName).HasColumnName("bin_name").HasMaxLength(100);
            e.Property(x => x.BinCategory).HasColumnName("bin_category").HasMaxLength(20);
            e.Property(x => x.BinNo).HasColumnName("bin_no");
            e.Property(x => x.Description).HasColumnName("description").HasMaxLength(255);
            e.Property(x => x.Color).HasColumnName("color").HasMaxLength(20);
            e.Property(x => x.IsDefault).HasColumnName("is_default");
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProcessCode).HasColumnName("process_code").HasMaxLength(50);
            e.Property(x => x.TestType).HasColumnName("test_type").HasMaxLength(20);
            e.Property(x => x.SortOrder).HasColumnName("sort_order");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.Deleted).HasColumnName("deleted");
            e.HasIndex(x => x.BinCode).HasDatabaseName("idx_bin_code");
            e.HasIndex(x => x.BinCategory).HasDatabaseName("idx_bin_category");
            e.HasIndex(x => x.ProcessCode).HasDatabaseName("idx_bin_process");
        });

        // === BinSortRecord ===
        modelBuilder.Entity<BinSortRecord>(e =>
        {
            e.ToTable("bin_sort_record");
            e.HasKey(x => x.RecordId);
            e.Property(x => x.RecordId).HasColumnName("record_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.TestProgram).HasColumnName("test_program").HasMaxLength(100);
            e.Property(x => x.BinCode).HasColumnName("bin_code").HasMaxLength(20);
            e.Property(x => x.BinName).HasColumnName("bin_name").HasMaxLength(100);
            e.Property(x => x.BinCategory).HasColumnName("bin_category").HasMaxLength(20);
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.YieldContribution).HasColumnName("yield_contribution").HasPrecision(5, 2);
            e.Property(x => x.TestResult).HasColumnName("test_result").HasMaxLength(255);
            e.Property(x => x.BinDescription).HasColumnName("bin_description").HasMaxLength(255);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.SortTime).HasColumnName("sort_time");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_bsr_lot");
            e.HasIndex(x => x.StepCode).HasDatabaseName("idx_bsr_step");
            e.HasIndex(x => x.BinCode).HasDatabaseName("idx_bsr_bin");
            e.HasIndex(x => x.SortTime).HasDatabaseName("idx_bsr_time");
        });

        // === BinStatistics ===
        modelBuilder.Entity<BinStatistics>(e =>
        {
            e.ToTable("bin_statistics");
            e.HasKey(x => x.StatId);
            e.Property(x => x.StatId).HasColumnName("stat_id").ValueGeneratedOnAdd();
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.BinCode).HasColumnName("bin_code").HasMaxLength(20);
            e.Property(x => x.BinName).HasColumnName("bin_name").HasMaxLength(100);
            e.Property(x => x.BinCategory).HasColumnName("bin_category").HasMaxLength(20);
            e.Property(x => x.TotalQty).HasColumnName("total_qty");
            e.Property(x => x.Percentage).HasColumnName("percentage").HasPrecision(5, 2);
            e.Property(x => x.InputQty).HasColumnName("input_qty");
            e.Property(x => x.CumulativeYield).HasColumnName("cumulative_yield").HasPrecision(5, 2);
            e.Property(x => x.StatPeriod).HasColumnName("stat_period");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_bs_lot");
            e.HasIndex(x => x.BinCode).HasDatabaseName("idx_bs_bin");
            e.HasIndex(x => x.StatPeriod).HasDatabaseName("idx_bs_period");
        });
    }

    private static void ConfigureWireMaterial(ModelBuilder modelBuilder)
    {
        // === WireMaterialSwitchRecord ===
        modelBuilder.Entity<WireMaterialSwitchRecord>(e =>
        {
            e.ToTable("wire_material_switch_record");
            e.HasKey(x => x.SwitchId);
            e.Property(x => x.SwitchId).HasColumnName("switch_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.OldWireMaterialId).HasColumnName("old_wire_material_id").HasMaxLength(50);
            e.Property(x => x.OldWireMaterialName).HasColumnName("old_wire_material_name").HasMaxLength(100);
            e.Property(x => x.OldWireLotNo).HasColumnName("old_wire_lot_no").HasMaxLength(50);
            e.Property(x => x.OldWireDiameter).HasColumnName("old_wire_diameter").HasPrecision(8, 4);
            e.Property(x => x.NewWireMaterialId).HasColumnName("new_wire_material_id").HasMaxLength(50);
            e.Property(x => x.NewWireMaterialName).HasColumnName("new_wire_material_name").HasMaxLength(100);
            e.Property(x => x.NewWireLotNo).HasColumnName("new_wire_lot_no").HasMaxLength(50);
            e.Property(x => x.NewWireDiameter).HasColumnName("new_wire_diameter").HasPrecision(8, 4);
            e.Property(x => x.WireSupplier).HasColumnName("wire_supplier").HasMaxLength(100);
            e.Property(x => x.SwitchReason).HasColumnName("switch_reason").HasColumnType("text");
            e.Property(x => x.VerificationResult).HasColumnName("verification_result").HasColumnType("text");
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.SwitchTime).HasColumnName("switch_time");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_wmsr_lot");
            e.HasIndex(x => x.StepCode).HasDatabaseName("idx_wmsr_step");
            e.HasIndex(x => x.SwitchTime).HasDatabaseName("idx_wmsr_time");
        });

        // === WireConsumption ===
        modelBuilder.Entity<WireConsumption>(e =>
        {
            e.ToTable("wire_consumption");
            e.HasKey(x => x.ConsumptionId);
            e.Property(x => x.ConsumptionId).HasColumnName("consumption_id").ValueGeneratedOnAdd();
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.WireMaterialId).HasColumnName("wire_material_id").HasMaxLength(50);
            e.Property(x => x.WireMaterialName).HasColumnName("wire_material_name").HasMaxLength(100);
            e.Property(x => x.WireLotNo).HasColumnName("wire_lot_no").HasMaxLength(50);
            e.Property(x => x.WireDiameter).HasColumnName("wire_diameter").HasPrecision(8, 4);
            e.Property(x => x.ConsumedLength).HasColumnName("consumed_length").HasPrecision(12, 4);
            e.Property(x => x.LengthUnit).HasColumnName("length_unit").HasMaxLength(10);
            e.Property(x => x.BondCount).HasColumnName("bond_count");
            e.Property(x => x.ProductQty).HasColumnName("product_qty");
            e.Property(x => x.AvgLengthPerBond).HasColumnName("avg_length_per_bond").HasPrecision(10, 4);
            e.Property(x => x.TheoreticalLength).HasColumnName("theoretical_length").HasPrecision(12, 4);
            e.Property(x => x.LossRate).HasColumnName("loss_rate").HasPrecision(5, 2);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.ConsumptionTime).HasColumnName("consumption_time");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_wc_lot");
            e.HasIndex(x => x.WireMaterialId).HasDatabaseName("idx_wc_material");
            e.HasIndex(x => x.ConsumptionTime).HasDatabaseName("idx_wc_time");
        });
    }

    private static void ConfigureTooling(ModelBuilder modelBuilder)
    {
        // === ToolingRegistry ===
        modelBuilder.Entity<ToolingRegistry>(e =>
        {
            e.ToTable("tooling_registry");
            e.HasKey(x => x.ToolingId);
            e.Property(x => x.ToolingId).HasColumnName("tooling_id").HasMaxLength(50);
            e.Property(x => x.ToolingCode).HasColumnName("tooling_code").HasMaxLength(50);
            e.Property(x => x.ToolingName).HasColumnName("tooling_name").HasMaxLength(200);
            e.Property(x => x.ToolingType).HasColumnName("tooling_type").HasMaxLength(50);
            e.Property(x => x.Specification).HasColumnName("specification").HasMaxLength(200);
            e.Property(x => x.Manufacturer).HasColumnName("manufacturer").HasMaxLength(100);
            e.Property(x => x.Model).HasColumnName("model").HasMaxLength(100);
            e.Property(x => x.Supplier).HasColumnName("supplier").HasMaxLength(100);
            e.Property(x => x.PurchaseDate).HasColumnName("purchase_date");
            e.Property(x => x.InstallDate).HasColumnName("install_date");
            e.Property(x => x.ExpectedLifespan).HasColumnName("expected_lifespan");
            e.Property(x => x.LifespanUnit).HasColumnName("lifespan_unit").HasMaxLength(20);
            e.Property(x => x.CurrentUsage).HasColumnName("current_usage");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.Location).HasColumnName("location").HasMaxLength(100);
            e.Property(x => x.AssociatedEquipment).HasColumnName("associated_equipment").HasMaxLength(50);
            e.Property(x => x.AssociatedProcess).HasColumnName("associated_process").HasMaxLength(50);
            e.Property(x => x.LastMaintenanceDate).HasColumnName("last_maintenance_date").HasMaxLength(20);
            e.Property(x => x.NextMaintenanceDate).HasColumnName("next_maintenance_date").HasMaxLength(20);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.Deleted).HasColumnName("deleted");
            e.HasIndex(x => x.ToolingType).HasDatabaseName("idx_tr_type");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_tr_status");
            e.HasIndex(x => x.AssociatedEquipment).HasDatabaseName("idx_tr_equipment");
        });

        // === ToolingUsageLog ===
        modelBuilder.Entity<ToolingUsageLog>(e =>
        {
            e.ToTable("tooling_usage_log");
            e.HasKey(x => x.LogId);
            e.Property(x => x.LogId).HasColumnName("log_id").ValueGeneratedOnAdd();
            e.Property(x => x.ToolingId).HasColumnName("tooling_id").HasMaxLength(50);
            e.Property(x => x.ToolingCode).HasColumnName("tooling_code").HasMaxLength(50);
            e.Property(x => x.ToolingName).HasColumnName("tooling_name").HasMaxLength(200);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.StartTime).HasColumnName("start_time");
            e.Property(x => x.EndTime).HasColumnName("end_time");
            e.Property(x => x.UsageDuration).HasColumnName("usage_duration").HasPrecision(10, 2);
            e.Property(x => x.UsageDurationUnit).HasColumnName("usage_duration_unit").HasMaxLength(20);
            e.Property(x => x.UsageCount).HasColumnName("usage_count");
            e.Property(x => x.UsageStatus).HasColumnName("usage_status").HasMaxLength(20);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.ToolingId).HasDatabaseName("idx_tul_tooling");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_tul_lot");
            e.HasIndex(x => x.StartTime).HasDatabaseName("idx_tul_time");
        });

        // === ToolingReplacementRecord ===
        modelBuilder.Entity<ToolingReplacementRecord>(e =>
        {
            e.ToTable("tooling_replacement_record");
            e.HasKey(x => x.ReplacementId);
            e.Property(x => x.ReplacementId).HasColumnName("replacement_id").HasMaxLength(50);
            e.Property(x => x.OldToolingId).HasColumnName("old_tooling_id").HasMaxLength(50);
            e.Property(x => x.OldToolingCode).HasColumnName("old_tooling_code").HasMaxLength(50);
            e.Property(x => x.OldToolingName).HasColumnName("old_tooling_name").HasMaxLength(200);
            e.Property(x => x.NewToolingId).HasColumnName("new_tooling_id").HasMaxLength(50);
            e.Property(x => x.NewToolingCode).HasColumnName("new_tooling_code").HasMaxLength(50);
            e.Property(x => x.NewToolingName).HasColumnName("new_tooling_name").HasMaxLength(200);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.ReplacementReason).HasColumnName("replacement_reason").HasMaxLength(50);
            e.Property(x => x.ReasonDetail).HasColumnName("reason_detail").HasColumnType("text");
            e.Property(x => x.OldToolingUsage).HasColumnName("old_tooling_usage");
            e.Property(x => x.ExpectedLifespan).HasColumnName("expected_lifespan");
            e.Property(x => x.UsagePercentage).HasColumnName("usage_percentage").HasPrecision(5, 2);
            e.Property(x => x.VerificationResult).HasColumnName("verification_result").HasColumnType("text");
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.ReplacementTime).HasColumnName("replacement_time");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.OldToolingId).HasDatabaseName("idx_trr_old_tooling");
            e.HasIndex(x => x.EquipmentId).HasDatabaseName("idx_trr_equipment");
            e.HasIndex(x => x.ReplacementTime).HasDatabaseName("idx_trr_time");
        });
    }

    private static void ConfigureOperatorQualification(ModelBuilder modelBuilder)
    {
        // === OperatorQualification ===
        modelBuilder.Entity<OperatorQualification>(e =>
        {
            e.ToTable("operator_qualification");
            e.HasKey(x => x.QualificationId);
            e.Property(x => x.QualificationId).HasColumnName("qualification_id").HasMaxLength(50);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.Department).HasColumnName("department").HasMaxLength(100);
            e.Property(x => x.Position).HasColumnName("position").HasMaxLength(100);
            e.Property(x => x.ProcessCode).HasColumnName("process_code").HasMaxLength(50);
            e.Property(x => x.ProcessName).HasColumnName("process_name").HasMaxLength(100);
            e.Property(x => x.QualificationLevel).HasColumnName("qualification_level").HasMaxLength(20);
            e.Property(x => x.CertificationType).HasColumnName("certification_type").HasMaxLength(50);
            e.Property(x => x.IssueDate).HasColumnName("issue_date");
            e.Property(x => x.ExpiryDate).HasColumnName("expiry_date");
            e.Property(x => x.IssuedBy).HasColumnName("issued_by").HasMaxLength(50);
            e.Property(x => x.CertificationNo).HasColumnName("certification_no").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.Deleted).HasColumnName("deleted");
            e.HasIndex(x => x.OperatorId).HasDatabaseName("idx_oq_operator");
            e.HasIndex(x => x.ProcessCode).HasDatabaseName("idx_oq_process");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_oq_status");
            e.HasIndex(x => x.ExpiryDate).HasDatabaseName("idx_oq_expiry");
        });

        // === QualificationCheckLog ===
        modelBuilder.Entity<QualificationCheckLog>(e =>
        {
            e.ToTable("qualification_check_log");
            e.HasKey(x => x.LogId);
            e.Property(x => x.LogId).HasColumnName("log_id").ValueGeneratedOnAdd();
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.QualificationId).HasColumnName("qualification_id").HasMaxLength(50);
            e.Property(x => x.ProcessCode).HasColumnName("process_code").HasMaxLength(50);
            e.Property(x => x.ProcessName).HasColumnName("process_name").HasMaxLength(100);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.CheckType).HasColumnName("check_type").HasMaxLength(20);
            e.Property(x => x.CheckResult).HasColumnName("check_result").HasMaxLength(20);
            e.Property(x => x.QualificationLevel).HasColumnName("qualification_level").HasMaxLength(20);
            e.Property(x => x.QualificationStatus).HasColumnName("qualification_status").HasMaxLength(20);
            e.Property(x => x.QualificationExpiryDate).HasColumnName("qualification_expiry_date");
            e.Property(x => x.IsQualified).HasColumnName("is_qualified");
            e.Property(x => x.FailReason).HasColumnName("fail_reason").HasColumnType("text");
            e.Property(x => x.Action).HasColumnName("action").HasMaxLength(20);
            e.Property(x => x.CheckTime).HasColumnName("check_time");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.OperatorId).HasDatabaseName("idx_qcl_operator");
            e.HasIndex(x => x.ProcessCode).HasDatabaseName("idx_qcl_process");
            e.HasIndex(x => x.CheckTime).HasDatabaseName("idx_qcl_time");
        });
    }

}
