using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Infrastructure.Persistence;

public class MesDbContext : DbContext
{
    public MesDbContext(DbContextOptions<MesDbContext> options) : base(options) { }

    // System
    public DbSet<SysDepartment> SysDepartments => Set<SysDepartment>();
    public DbSet<SysRole> SysRoles => Set<SysRole>();
    public DbSet<SysUser> SysUsers => Set<SysUser>();
    public DbSet<SysUserPermission> SysUserPermissions => Set<SysUserPermission>();
    public DbSet<SysSignatureLevel> SysSignatureLevels => Set<SysSignatureLevel>();

    // Master Data
    public DbSet<MasterProduct> MasterProducts => Set<MasterProduct>();
    public DbSet<MasterRoute> MasterRoutes => Set<MasterRoute>();
    public DbSet<MasterRouteStep> MasterRouteSteps => Set<MasterRouteStep>();
    public DbSet<MasterEquipment> MasterEquipments => Set<MasterEquipment>();
    public DbSet<MasterEquipmentRoute> MasterEquipmentRoutes => Set<MasterEquipmentRoute>();
    public DbSet<MasterCarrier> MasterCarriers => Set<MasterCarrier>();
    public DbSet<MasterRecipe> MasterRecipes => Set<MasterRecipe>();
    public DbSet<MasterYieldRule> MasterYieldRules => Set<MasterYieldRule>();
    public DbSet<MasterAlarmRule> MasterAlarmRules => Set<MasterAlarmRule>();
    public DbSet<MasterScrapRule> MasterScrapRules => Set<MasterScrapRule>();

    // Production
    public DbSet<ProdWorkOrder> ProdWorkOrders => Set<ProdWorkOrder>();
    public DbSet<ProdLot> ProdLots => Set<ProdLot>();
    public DbSet<ProdLotStep> ProdLotSteps => Set<ProdLotStep>();
    public DbSet<ProdOperationHistory> ProdOperationHistories => Set<ProdOperationHistory>();
    public DbSet<ProdAuditTrail> ProdAuditTrails => Set<ProdAuditTrail>();
    public DbSet<ProdHoldRecord> ProdHoldRecords => Set<ProdHoldRecord>();
    public DbSet<ProdScrapRecord> ProdScrapRecords => Set<ProdScrapRecord>();
    public DbSet<ProdReworkRecord> ProdReworkRecords => Set<ProdReworkRecord>();
    public DbSet<ProdLotSplit> ProdLotSplits => Set<ProdLotSplit>();
    public DbSet<ProdLotMerge> ProdLotMerges => Set<ProdLotMerge>();
    public DbSet<ProdAlarm> ProdAlarms => Set<ProdAlarm>();
    public DbSet<ProdCarrierBinding> ProdCarrierBindings => Set<ProdCarrierBinding>();
    public DbSet<ProdSignature> ProdSignatures => Set<ProdSignature>();
    public DbSet<ProdDispatchTask> ProdDispatchTasks => Set<ProdDispatchTask>();
    public DbSet<QuantityTransaction> QuantityTransactions => Set<QuantityTransaction>();
    public DbSet<ProdGenealogy> ProdGenealogies => Set<ProdGenealogy>();
    public DbSet<ProdLotArchive> ProdLotArchives => Set<ProdLotArchive>();

    // External System
    public DbSet<ExtSystemEvent> ExtSystemEvents => Set<ExtSystemEvent>();
    public DbSet<ExtSystemConfig> ExtSystemConfigs => Set<ExtSystemConfig>();
    public DbSet<CustomerRequirement> CustomerRequirements => Set<CustomerRequirement>();
    public DbSet<MasterMaterial> MasterMaterials => Set<MasterMaterial>();
    public DbSet<MaterialRequirement> MaterialRequirements => Set<MaterialRequirement>();
    public DbSet<MaterialConsumeEntity> MaterialConsumes => Set<MaterialConsumeEntity>();
    public DbSet<QualityGateEntity> QualityGateInstances => Set<QualityGateEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // System
        modelBuilder.Entity<SysDepartment>(e =>
        {
            e.ToTable("sys_department");
            e.HasKey(x => x.DeptId);
            e.Property(x => x.DeptId).HasColumnName("dept_id").HasMaxLength(50);
            e.Property(x => x.DeptName).HasColumnName("dept_name").HasMaxLength(100);
            e.Property(x => x.ParentId).HasColumnName("parent_id").HasMaxLength(50);
            e.Property(x => x.ManagerId).HasColumnName("manager_id").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.ParentId);
        });

        modelBuilder.Entity<SysRole>(e =>
        {
            e.ToTable("sys_role");
            e.HasKey(x => x.RoleId);
            e.Property(x => x.RoleId).HasColumnName("role_id").HasMaxLength(50);
            e.Property(x => x.RoleName).HasColumnName("role_name").HasMaxLength(100);
            e.Property(x => x.Description).HasColumnName("description").HasMaxLength(255);
            e.Property(x => x.Level).HasColumnName("level");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<SysUser>(e =>
        {
            e.ToTable("sys_user");
            e.HasKey(x => x.UserId);
            e.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(50);
            e.Property(x => x.UserName).HasColumnName("user_name").HasMaxLength(100);
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
            e.Property(x => x.RoleId).HasColumnName("role_id").HasMaxLength(50);
            e.Property(x => x.DeptId).HasColumnName("dept_id").HasMaxLength(50);
            e.Property(x => x.Shift).HasColumnName("shift").HasMaxLength(20);
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.RoleId);
            e.HasIndex(x => x.DeptId);
        });

        modelBuilder.Entity<SysUserPermission>(e =>
        {
            e.ToTable("sys_user_permission");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(50);
            e.Property(x => x.PermissionCode).HasColumnName("permission_code").HasMaxLength(100);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => new { x.UserId, x.PermissionCode }).IsUnique().HasDatabaseName("uk_user_permission");
            e.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<SysSignatureLevel>(e =>
        {
            e.ToTable("sys_signature_level");
            e.HasKey(x => x.LevelCode);
            e.Property(x => x.LevelCode).HasColumnName("level_code").HasMaxLength(20);
            e.Property(x => x.LevelName).HasColumnName("level_name").HasMaxLength(50);
            e.Property(x => x.LevelOrder).HasColumnName("level_order");
            e.Property(x => x.Description).HasColumnName("description").HasMaxLength(255);
        });

        // Master Data
        modelBuilder.Entity<MasterProduct>(e =>
        {
            e.ToTable("master_product");
            e.HasKey(x => x.ProductId);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.DieName).HasColumnName("die_name").HasMaxLength(100);
            e.Property(x => x.PackageType).HasColumnName("package_type").HasMaxLength(50);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(100);
            e.Property(x => x.CustomerPn).HasColumnName("customer_pn").HasMaxLength(100);
            e.Property(x => x.InternalPn).HasColumnName("internal_pn").HasMaxLength(100);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.CustomerId);
        });

        modelBuilder.Entity<MasterRoute>(e =>
        {
            e.ToTable("master_route");
            e.HasKey(x => x.RouteId);
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.Property(x => x.RouteName).HasColumnName("route_name").HasMaxLength(100);
            e.Property(x => x.RouteVersion).HasColumnName("route_version").HasMaxLength(20);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.PackageType).HasColumnName("package_type").HasMaxLength(50);
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.IsApproved).HasColumnName("is_approved");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.ApprovedAt).HasColumnName("approved_at");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.ProductId);
            e.HasIndex(x => x.IsActive);
            e.HasMany(x => x.Steps).WithOne().HasForeignKey(x => x.RouteId).HasPrincipalKey(x => x.RouteId);
        });

        modelBuilder.Entity<MasterRouteStep>(e =>
        {
            e.ToTable("master_route_step");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepName).HasColumnName("step_name").HasMaxLength(100);
            e.Property(x => x.EquipmentGroup).HasColumnName("equipment_group").HasMaxLength(50);
            e.Property(x => x.IsRework).HasColumnName("is_rework");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => new { x.RouteId, x.StepSeq }).IsUnique().HasDatabaseName("uk_route_step");
            e.HasIndex(x => x.RouteId);
            e.HasIndex(x => x.StepCode);
        });

        modelBuilder.Entity<MasterEquipment>(e =>
        {
            e.ToTable("master_equipment");
            e.HasKey(x => x.EquipmentId);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.EquipmentName).HasColumnName("equipment_name").HasMaxLength(100);
            e.Property(x => x.EquipmentGroup).HasColumnName("equipment_group").HasMaxLength(50);
            e.Property(x => x.EquipmentType).HasColumnName("equipment_type").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CurrentLotId).HasColumnName("current_lot_id").HasMaxLength(50);
            e.Property(x => x.CurrentRecipe).HasColumnName("current_recipe").HasMaxLength(100);
            e.Property(x => x.Location).HasColumnName("location").HasMaxLength(100);
            e.Property(x => x.ResponsiblePerson).HasColumnName("responsible_person").HasMaxLength(50);
            e.Property(x => x.LastMaintenanceDate).HasColumnName("last_maintenance_date");
            e.Property(x => x.MaintenanceIntervalHours).HasColumnName("maintenance_interval_hours");
            e.Property(x => x.RunningHours).HasColumnName("running_hours");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.EquipmentGroup);
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<MasterEquipmentRoute>(e =>
        {
            e.ToTable("master_equipment_route");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.HasIndex(x => new { x.EquipmentId, x.RouteId }).IsUnique().HasDatabaseName("uk_equip_route");
            e.HasIndex(x => x.EquipmentId);
        });

        modelBuilder.Entity<MasterCarrier>(e =>
        {
            e.ToTable("master_carrier");
            e.HasKey(x => x.CarrierId);
            e.Property(x => x.CarrierId).HasColumnName("carrier_id").HasMaxLength(50);
            e.Property(x => x.CarrierType).HasColumnName("carrier_type").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CurrentLotId).HasColumnName("current_lot_id").HasMaxLength(50);
            e.Property(x => x.Capacity).HasColumnName("capacity");
            e.Property(x => x.UseCount).HasColumnName("use_count");
            e.Property(x => x.MaxUseCount).HasColumnName("max_use_count");
            e.Property(x => x.LastCleanDate).HasColumnName("last_clean_date");
            e.Property(x => x.CleanIntervalUses).HasColumnName("clean_interval_uses");
            e.Property(x => x.Location).HasColumnName("location").HasMaxLength(100);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.CarrierType);
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<MasterRecipe>(e =>
        {
            e.ToTable("master_recipe");
            e.HasKey(x => x.RecipeId);
            e.Property(x => x.RecipeId).HasColumnName("recipe_id").HasMaxLength(100);
            e.Property(x => x.RecipeName).HasColumnName("recipe_name").HasMaxLength(100);
            e.Property(x => x.EquipmentGroup).HasColumnName("equipment_group").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.Version).HasColumnName("version").HasMaxLength(20);
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.Parameters).HasColumnName("parameters").HasColumnType("json");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.ApprovedAt).HasColumnName("approved_at");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.EquipmentGroup);
            e.HasIndex(x => x.ProductId);
            e.HasIndex(x => x.StepCode);
        });

        modelBuilder.Entity<MasterYieldRule>(e =>
        {
            e.ToTable("master_yield_rule");
            e.HasKey(x => x.RuleId);
            e.Property(x => x.RuleId).HasColumnName("rule_id").HasMaxLength(50);
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.YieldThreshold).HasColumnName("yield_threshold").HasPrecision(5, 2);
            e.Property(x => x.ActionType).HasColumnName("action_type").HasMaxLength(50);
            e.Property(x => x.NotifyRole).HasColumnName("notify_role").HasMaxLength(50);
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.RouteId);
            e.HasIndex(x => x.StepCode);
        });

        modelBuilder.Entity<MasterAlarmRule>(e =>
        {
            e.ToTable("master_alarm_rule");
            e.HasKey(x => x.RuleId);
            e.Property(x => x.RuleId).HasColumnName("rule_id").HasMaxLength(50);
            e.Property(x => x.AlarmType).HasColumnName("alarm_type").HasMaxLength(50);
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.ThresholdYield).HasColumnName("threshold_yield").HasPrecision(5, 2);
            e.Property(x => x.ThresholdQty).HasColumnName("threshold_qty");
            e.Property(x => x.ThresholdMinutes).HasColumnName("threshold_minutes");
            e.Property(x => x.IsEnabled).HasColumnName("is_enabled");
            e.Property(x => x.NotifyRole).HasColumnName("notify_role").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.AlarmType);
        });

        modelBuilder.Entity<MasterScrapRule>(e =>
        {
            e.ToTable("master_scrap_rule");
            e.HasKey(x => x.RuleId);
            e.Property(x => x.RuleId).HasColumnName("rule_id").HasMaxLength(50);
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.ThresholdPercent).HasColumnName("threshold_percent").HasPrecision(5, 2);
            e.Property(x => x.RequiresApproval).HasColumnName("requires_approval");
            e.Property(x => x.ApprovalLevel).HasColumnName("approval_level").HasMaxLength(20);
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        // Production
        modelBuilder.Entity<ProdWorkOrder>(e =>
        {
            e.ToTable("prod_work_order");
            e.HasKey(x => x.OrderId);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ParentOrderId).HasColumnName("parent_order_id").HasMaxLength(50);
            e.Property(x => x.WoType).HasColumnName("wo_type").HasMaxLength(20);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.Property(x => x.RouteName).HasColumnName("route_name").HasMaxLength(100);
            e.Property(x => x.DieName).HasColumnName("die_name").HasMaxLength(100);
            e.Property(x => x.PackageType).HasColumnName("package_type").HasMaxLength(50);
            e.Property(x => x.PlannedQty).HasColumnName("planned_qty");
            e.Property(x => x.CompletedQty).HasColumnName("completed_qty");
            e.Property(x => x.WaferQty).HasColumnName("wafer_qty");
            e.Property(x => x.UnitQty).HasColumnName("unit_qty");
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(100);
            e.Property(x => x.CustomerPn).HasColumnName("customer_pn").HasMaxLength(100);
            e.Property(x => x.InternalPn).HasColumnName("internal_pn").HasMaxLength(100);
            e.Property(x => x.Priority).HasColumnName("priority").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Creator).HasColumnName("creator").HasMaxLength(50);
            e.Property(x => x.PlannedStartDate).HasColumnName("planned_start_date");
            e.Property(x => x.PlannedEndDate).HasColumnName("planned_end_date");
            e.Property(x => x.ActualStartDate).HasColumnName("actual_start_date");
            e.Property(x => x.ActualEndDate).HasColumnName("actual_end_date");
            e.Property(x => x.TargetCpYield).HasColumnName("target_cp_yield").HasPrecision(5, 2);
            e.Property(x => x.TargetFtYield).HasColumnName("target_ft_yield").HasPrecision(5, 2);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.ProductId);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.Priority);
            e.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<ProdLot>(e =>
        {
            e.ToTable("prod_lot");
            e.HasKey(x => x.LotId);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.DieName).HasColumnName("die_name").HasMaxLength(100);
            e.Property(x => x.PackageType).HasColumnName("package_type").HasMaxLength(50);
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.Property(x => x.RouteVersion).HasColumnName("route_version").HasMaxLength(20);
            e.Property(x => x.CurrentStepSeq).HasColumnName("current_step_seq");
            e.Property(x => x.CurrentStepCode).HasColumnName("current_step_code").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.ProcessStage).HasColumnName("process_stage").HasMaxLength(20);
            e.Property(x => x.UnitCount).HasColumnName("unit_count");
            e.Property(x => x.StripCount).HasColumnName("strip_count");
            e.Property(x => x.Priority).HasColumnName("priority").HasMaxLength(20);
            e.Property(x => x.CarrierType).HasColumnName("carrier_type").HasMaxLength(50);
            e.Property(x => x.CarrierId).HasColumnName("carrier_id").HasMaxLength(50);
            e.Property(x => x.WaferLotId).HasColumnName("wafer_lot_id").HasMaxLength(50);
            e.Property(x => x.OriginalQty).HasColumnName("original_qty");
            e.Property(x => x.TotalPassQty).HasColumnName("total_pass_qty");
            e.Property(x => x.TotalScrapQty).HasColumnName("total_scrap_qty");
            e.Property(x => x.TotalReworkQty).HasColumnName("total_rework_qty");
            e.Property(x => x.TotalHoldQty).HasColumnName("total_hold_qty");
            e.Property(x => x.IsPartialLot).HasColumnName("is_partial_lot");
            e.Property(x => x.MotherLotId).HasColumnName("mother_lot_id").HasMaxLength(50);
            e.Property(x => x.SplitReason).HasColumnName("split_reason").HasMaxLength(255);
            e.Property(x => x.SplitTime).HasColumnName("split_time");
            e.Property(x => x.SplitQty).HasColumnName("split_qty");
            e.Property(x => x.IsReworkLot).HasColumnName("is_rework_lot");
            e.Property(x => x.OriginalRouteId).HasColumnName("original_route_id").HasMaxLength(100);
            e.Property(x => x.ReworkRouteId).HasColumnName("rework_route_id").HasMaxLength(100);
            e.Property(x => x.ReworkCount).HasColumnName("rework_count");
            e.Property(x => x.ReworkReason).HasColumnName("rework_reason").HasMaxLength(255);
            e.Property(x => x.IsUnderMrb).HasColumnName("is_under_mrb");
            e.Property(x => x.MrbReference).HasColumnName("mrb_reference").HasMaxLength(50);
            e.Property(x => x.MrbDisposition).HasColumnName("mrb_disposition").HasMaxLength(50);
            e.Property(x => x.Grade).HasColumnName("grade").HasMaxLength(20);
            e.Property(x => x.OriginalLotId).HasColumnName("original_lot_id").HasMaxLength(50);
            e.Property(x => x.BinResult).HasColumnName("bin_result").HasMaxLength(50);
            e.Property(x => x.TestResult).HasColumnName("test_result").HasMaxLength(50);
            e.Property(x => x.QtyPass).HasColumnName("qty_pass");
            e.Property(x => x.QtyFail).HasColumnName("qty_fail");
            e.Property(x => x.HoldCategory).HasColumnName("hold_category").HasMaxLength(50);
            e.Property(x => x.HoldReason).HasColumnName("hold_reason").HasMaxLength(255);
            e.Property(x => x.HoldTime).HasColumnName("hold_time");
            e.Property(x => x.HoldOperator).HasColumnName("hold_operator").HasMaxLength(50);
            e.Property(x => x.ReleaseCondition).HasColumnName("release_condition").HasMaxLength(255);
            e.Property(x => x.IsArchived).HasColumnName("is_archived");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.OrderId);
            e.HasIndex(x => x.ProductId);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.CurrentStepCode);
            e.HasIndex(x => x.Priority);
            e.HasIndex(x => x.MotherLotId);
            e.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<ProdLotArchive>(e =>
        {
            e.ToTable("prod_lot_archive");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(200);
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.Property(x => x.ProcessStage).HasColumnName("process_stage").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.OriginalQty).HasColumnName("original_qty");
            e.Property(x => x.TotalPassQty).HasColumnName("total_pass_qty");
            e.Property(x => x.TotalScrapQty).HasColumnName("total_scrap_qty");
            e.Property(x => x.TotalReworkQty).HasColumnName("total_rework_qty");
            e.Property(x => x.TotalHoldQty).HasColumnName("total_hold_qty");
            e.Property(x => x.FinalYield).HasColumnName("final_yield").HasPrecision(5, 2);
            e.Property(x => x.Grade).HasColumnName("grade").HasMaxLength(10);
            e.Property(x => x.CompletedAt).HasColumnName("completed_at");
            e.Property(x => x.ArchivedAt).HasColumnName("archived_at");
            e.Property(x => x.ArchivedBy).HasColumnName("archived_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_archive_lot_id");
            e.HasIndex(x => x.OrderId).HasDatabaseName("idx_archive_order_id");
            e.HasIndex(x => x.CompletedAt).HasDatabaseName("idx_archive_completed_at");
            e.HasIndex(x => x.ProcessStage).HasDatabaseName("idx_archive_stage");
        });

        modelBuilder.Entity<ProdLotStep>(e =>
        {
            e.ToTable("prod_lot_step");
            e.HasKey(x => x.RecordId);
            e.Property(x => x.RecordId).HasColumnName("record_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.Property(x => x.RouteVersion).HasColumnName("route_version").HasMaxLength(20);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepName).HasColumnName("step_name").HasMaxLength(100);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.TrackInEquipment).HasColumnName("track_in_equipment").HasMaxLength(50);
            e.Property(x => x.TrackInCarrier).HasColumnName("track_in_carrier").HasMaxLength(50);
            e.Property(x => x.TrackInRecipe).HasColumnName("track_in_recipe").HasMaxLength(100);
            e.Property(x => x.TrackInTime).HasColumnName("track_in_time");
            e.Property(x => x.TrackInOperator).HasColumnName("track_in_operator").HasMaxLength(50);
            e.Property(x => x.TrackOutTime).HasColumnName("track_out_time");
            e.Property(x => x.TrackOutOperator).HasColumnName("track_out_operator").HasMaxLength(50);
            e.Property(x => x.InputQty).HasColumnName("input_qty");
            e.Property(x => x.PassQty).HasColumnName("pass_qty");
            e.Property(x => x.FailQty).HasColumnName("fail_qty");
            e.Property(x => x.ScrapQty).HasColumnName("scrap_qty");
            e.Property(x => x.ReworkQty).HasColumnName("rework_qty");
            e.Property(x => x.HoldQty).HasColumnName("hold_qty");
            e.Property(x => x.PendingQty).HasColumnName("pending_qty");
            e.Property(x => x.RecipeId).HasColumnName("recipe_id").HasMaxLength(100);
            e.Property(x => x.TestProgram).HasColumnName("test_program").HasMaxLength(100);
            e.Property(x => x.BinSummary).HasColumnName("bin_summary").HasMaxLength(255);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.StepCode);
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<ProdOperationHistory>(e =>
        {
            e.ToTable("prod_operation_history");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.OperationId).HasColumnName("operation_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.OperationType).HasColumnName("operation_type").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.CarrierId).HasColumnName("carrier_id").HasMaxLength(50);
            e.Property(x => x.RecipeId).HasColumnName("recipe_id").HasMaxLength(100);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.InputQty).HasColumnName("input_qty");
            e.Property(x => x.OutputQty).HasColumnName("output_qty");
            e.Property(x => x.ScrapQty).HasColumnName("scrap_qty");
            e.Property(x => x.Detail).HasColumnName("detail").HasColumnType("json");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.OrderId);
            e.HasIndex(x => x.OperationType);
            e.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<ProdAuditTrail>(e =>
        {
            e.ToTable("prod_audit_trail");
            e.HasKey(x => x.AuditId);
            e.Property(x => x.AuditId).HasColumnName("audit_id").HasMaxLength(50);
            e.Property(x => x.EntityType).HasColumnName("entity_type").HasMaxLength(50);
            e.Property(x => x.EntityId).HasColumnName("entity_id").HasMaxLength(50);
            e.Property(x => x.Action).HasColumnName("action").HasMaxLength(50);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.Timestamp).HasColumnName("timestamp");
            e.Property(x => x.BeforeState).HasColumnName("before_state").HasColumnType("json");
            e.Property(x => x.AfterState).HasColumnName("after_state").HasColumnType("json");
            e.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(255);
            e.Property(x => x.Detail).HasColumnName("detail").HasColumnType("text");
            e.Property(x => x.SignatureLevel).HasColumnName("signature_level");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.HasIndex(x => new { x.EntityType, x.EntityId }).HasDatabaseName("idx_entity");
            e.HasIndex(x => x.Action);
            e.HasIndex(x => x.Timestamp);
        });

        modelBuilder.Entity<ProdHoldRecord>(e =>
        {
            e.ToTable("prod_hold_record");
            e.HasKey(x => x.HoldId);
            e.Property(x => x.HoldId).HasColumnName("hold_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.HoldType).HasColumnName("hold_type").HasMaxLength(50);
            e.Property(x => x.HoldReasonCode).HasColumnName("hold_reason_code").HasMaxLength(50);
            e.Property(x => x.HoldReason).HasColumnName("hold_reason").HasMaxLength(255);
            e.Property(x => x.HoldQty).HasColumnName("hold_qty");
            e.Property(x => x.ResponsibleDept).HasColumnName("responsible_dept").HasMaxLength(50);
            e.Property(x => x.Owner).HasColumnName("owner").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.HoldBy).HasColumnName("hold_by").HasMaxLength(50);
            e.Property(x => x.HoldTime).HasColumnName("hold_time");
            e.Property(x => x.RootCause).HasColumnName("root_cause").HasColumnType("text");
            e.Property(x => x.CorrectiveAction).HasColumnName("corrective_action").HasColumnType("text");
            e.Property(x => x.Disposition).HasColumnName("disposition").HasMaxLength(255);
            e.Property(x => x.ReleaseBy).HasColumnName("release_by").HasMaxLength(50);
            e.Property(x => x.ReleaseTime).HasColumnName("release_time");
            e.Property(x => x.ReleaseComment).HasColumnName("release_comment").HasColumnType("text");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.HoldType);
            e.HasIndex(x => x.HoldTime);
        });

        modelBuilder.Entity<ProdScrapRecord>(e =>
        {
            e.ToTable("prod_scrap_record");
            e.HasKey(x => x.ScrapId);
            e.Property(x => x.ScrapId).HasColumnName("scrap_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.ScrapQty).HasColumnName("scrap_qty");
            e.Property(x => x.ScrapReason).HasColumnName("scrap_reason").HasMaxLength(255);
            e.Property(x => x.ScrapReasonCode).HasColumnName("scrap_reason_code").HasMaxLength(50);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.ScrapTime).HasColumnName("scrap_time");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.SignatureId).HasColumnName("signature_id").HasMaxLength(50);
            e.Property(x => x.RequiresApproval).HasColumnName("requires_approval");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.StepCode);
            e.HasIndex(x => x.ScrapTime);
        });

        modelBuilder.Entity<ProdReworkRecord>(e =>
        {
            e.ToTable("prod_rework_record");
            e.HasKey(x => x.ReworkId);
            e.Property(x => x.ReworkId).HasColumnName("rework_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OriginalRouteId).HasColumnName("original_route_id").HasMaxLength(100);
            e.Property(x => x.ReworkRouteId).HasColumnName("rework_route_id").HasMaxLength(100);
            e.Property(x => x.FromStepCode).HasColumnName("from_step_code").HasMaxLength(50);
            e.Property(x => x.TargetStepCode).HasColumnName("target_step_code").HasMaxLength(50);
            e.Property(x => x.ReworkQty).HasColumnName("rework_qty");
            e.Property(x => x.ReworkReason).HasColumnName("rework_reason").HasMaxLength(255);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.ReworkCount).HasColumnName("rework_count");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.CompletedAt).HasColumnName("completed_at");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.SignatureId).HasColumnName("signature_id").HasMaxLength(50);
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.ReworkRouteId);
            e.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<ProdLotSplit>(e =>
        {
            e.ToTable("prod_lot_split");
            e.HasKey(x => x.SplitId);
            e.Property(x => x.SplitId).HasColumnName("split_id").HasMaxLength(50);
            e.Property(x => x.MotherLotId).HasColumnName("mother_lot_id").HasMaxLength(50);
            e.Property(x => x.ChildLotId).HasColumnName("child_lot_id").HasMaxLength(50);
            e.Property(x => x.SplitQty).HasColumnName("split_qty");
            e.Property(x => x.SplitReason).HasColumnName("split_reason").HasMaxLength(255);
            e.Property(x => x.SplitType).HasColumnName("split_type").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.SplitTime).HasColumnName("split_time");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.SignatureId).HasColumnName("signature_id").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.MotherLotId);
            e.HasIndex(x => x.ChildLotId);
            e.HasIndex(x => x.SplitTime);
        });

        modelBuilder.Entity<ProdLotMerge>(e =>
        {
            e.ToTable("prod_lot_merge");
            e.HasKey(x => x.MergeId);
            e.Property(x => x.MergeId).HasColumnName("merge_id").HasMaxLength(50);
            e.Property(x => x.TargetLotId).HasColumnName("target_lot_id").HasMaxLength(50);
            e.Property(x => x.SourceLotId).HasColumnName("source_lot_id").HasMaxLength(50);
            e.Property(x => x.MergeQty).HasColumnName("merge_qty");
            e.Property(x => x.MergeReason).HasColumnName("merge_reason").HasMaxLength(255);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.MergeTime).HasColumnName("merge_time");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.SignatureId).HasColumnName("signature_id").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.TargetLotId);
            e.HasIndex(x => x.SourceLotId);
            e.HasIndex(x => x.MergeTime);
        });

        modelBuilder.Entity<ProdAlarm>(e =>
        {
            e.ToTable("alarm_record");
            e.HasKey(x => x.AlarmId);
            e.Property(x => x.AlarmId).HasColumnName("alarm_id").HasMaxLength(50);
            e.Property(x => x.RuleId).HasColumnName("rule_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.AlarmType).HasColumnName("alarm_type").HasMaxLength(50);
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.Message).HasColumnName("message").HasColumnType("text");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.AcknowledgedBy).HasColumnName("acknowledged_by").HasMaxLength(50);
            e.Property(x => x.AcknowledgedAt).HasColumnName("acknowledged_at");
            e.Property(x => x.ResolvedBy).HasColumnName("resolved_by").HasMaxLength(50);
            e.Property(x => x.ResolvedAt).HasColumnName("resolved_at");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.RuleId);
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<ProdCarrierBinding>(e =>
        {
            e.ToTable("prod_carrier_binding");
            e.HasKey(x => x.BindingId);
            e.Property(x => x.BindingId).HasColumnName("binding_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.CarrierId).HasColumnName("carrier_id").HasMaxLength(50);
            e.Property(x => x.CarrierType).HasColumnName("carrier_type").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.BindTime).HasColumnName("bind_time");
            e.Property(x => x.UnbindTime).HasColumnName("unbind_time");
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.FromCarrierId).HasColumnName("from_carrier_id").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.CarrierId);
            e.HasIndex(x => x.BindTime);
        });

        modelBuilder.Entity<ProdSignature>(e =>
        {
            e.ToTable("prod_signature");
            e.HasKey(x => x.SignatureId);
            e.Property(x => x.SignatureId).HasColumnName("signature_id").HasMaxLength(50);
            e.Property(x => x.EntityType).HasColumnName("entity_type").HasMaxLength(50);
            e.Property(x => x.EntityId).HasColumnName("entity_id").HasMaxLength(50);
            e.Property(x => x.Level).HasColumnName("level").HasMaxLength(20);
            e.Property(x => x.SignerId).HasColumnName("signer_id").HasMaxLength(50);
            e.Property(x => x.SignerName).HasColumnName("signer_name").HasMaxLength(100);
            e.Property(x => x.SignerRole).HasColumnName("signer_role").HasMaxLength(50);
            e.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(255);
            e.Property(x => x.Comment).HasColumnName("comment").HasMaxLength(255);
            e.Property(x => x.SignTime).HasColumnName("sign_time");
            e.HasIndex(x => new { x.EntityType, x.EntityId }).HasDatabaseName("idx_entity");
            e.HasIndex(x => x.SignerId);
        });

        modelBuilder.Entity<ProdDispatchTask>(e =>
        {
            e.ToTable("prod_dispatch_task");
            e.HasKey(x => x.TaskId);
            e.Property(x => x.TaskId).HasColumnName("task_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepName).HasColumnName("step_name").HasMaxLength(100);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.RecipeId).HasColumnName("recipe_id").HasMaxLength(100);
            e.Property(x => x.Qty).HasColumnName("qty");
            e.Property(x => x.Priority).HasColumnName("priority").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.AssignedOperator).HasColumnName("assigned_operator").HasMaxLength(50);
            e.Property(x => x.AssignedAt).HasColumnName("assigned_at");
            e.Property(x => x.StartedAt).HasColumnName("started_at");
            e.Property(x => x.CompletedAt).HasColumnName("completed_at");
            e.Property(x => x.DueHours).HasColumnName("due_hours").HasPrecision(6, 2);
            e.Property(x => x.RemainingHours).HasColumnName("remaining_hours").HasPrecision(6, 2);
            e.Property(x => x.IsOverdue).HasColumnName("is_overdue");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.OrderId);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.Priority);
            e.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<QuantityTransaction>(e =>
        {
            e.ToTable("quantity_transaction");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.RouteId).HasColumnName("route_id").HasMaxLength(100);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepName).HasColumnName("step_name").HasMaxLength(100);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.InputQty).HasColumnName("input_qty");
            e.Property(x => x.PassQty).HasColumnName("pass_qty");
            e.Property(x => x.FailQty).HasColumnName("fail_qty");
            e.Property(x => x.ScrapQty).HasColumnName("scrap_qty");
            e.Property(x => x.ReworkQty).HasColumnName("rework_qty");
            e.Property(x => x.HoldQty).HasColumnName("hold_qty");
            e.Property(x => x.PendingQty).HasColumnName("pending_qty");
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.Timestamp).HasColumnName("timestamp");
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.StepCode);
            e.HasIndex(x => x.Timestamp);
        });

        modelBuilder.Entity<ProdGenealogy>(e =>
        {
            e.ToTable("prod_genealogy");
            e.HasKey(x => x.GenealogyId);
            e.Property(x => x.GenealogyId).HasColumnName("genealogy_id").HasMaxLength(50);
            e.Property(x => x.ParentLotId).HasColumnName("parent_lot_id").HasMaxLength(50);
            e.Property(x => x.ChildLotId).HasColumnName("child_lot_id").HasMaxLength(50);
            e.Property(x => x.RelationType).HasColumnName("relation_type").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.Qty).HasColumnName("qty");
            e.Property(x => x.Grade).HasColumnName("grade").HasMaxLength(20);
            e.Property(x => x.WaferLotId).HasColumnName("wafer_lot_id").HasMaxLength(50);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.ReasonCode).HasColumnName("reason_code").HasMaxLength(50);
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(255);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.ParentLotId);
            e.HasIndex(x => x.ChildLotId);
            e.HasIndex(x => x.RelationType);
        });

        // External System
        modelBuilder.Entity<ExtSystemEvent>(e =>
        {
            e.ToTable("ext_system_event");
            e.HasKey(x => x.EventId);
            e.Property(x => x.EventId).HasColumnName("event_id").HasMaxLength(50);
            e.Property(x => x.EventType).HasColumnName("event_type").HasMaxLength(50);
            e.Property(x => x.SourceSystem).HasColumnName("source_system").HasMaxLength(50);
            e.Property(x => x.TargetSystem).HasColumnName("target_system").HasMaxLength(50);
            e.Property(x => x.Payload).HasColumnName("payload").HasColumnType("json");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.SentAt).HasColumnName("sent_at");
            e.Property(x => x.ErrorMessage).HasColumnName("error_message").HasColumnType("text");
            e.Property(x => x.RetryCount).HasColumnName("retry_count");
            e.HasIndex(x => x.TargetSystem);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<ExtSystemConfig>(e =>
        {
            e.ToTable("ext_system_config");
            e.HasKey(x => x.SystemId);
            e.Property(x => x.SystemId).HasColumnName("system_id").HasMaxLength(50);
            e.Property(x => x.SystemName).HasColumnName("system_name").HasMaxLength(100);
            e.Property(x => x.SystemType).HasColumnName("system_type").HasMaxLength(50);
            e.Property(x => x.Endpoint).HasColumnName("endpoint").HasMaxLength(255);
            e.Property(x => x.AuthType).HasColumnName("auth_type").HasMaxLength(20);
            e.Property(x => x.AuthCredential).HasColumnName("auth_credential").HasColumnType("text");
            e.Property(x => x.IsEnabled).HasColumnName("is_enabled");
            e.Property(x => x.TimeoutSeconds).HasColumnName("timeout_seconds");
            e.Property(x => x.MaxRetries).HasColumnName("max_retries");
            e.Property(x => x.SubscribedEvents).HasColumnName("subscribed_events").HasColumnType("json");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.SystemType);
        });

        modelBuilder.Entity<CustomerRequirement>(e =>
        {
            e.ToTable("customer_requirement");
            e.HasKey(x => x.RequirementId);
            e.Property(x => x.RequirementId).HasColumnName("requirement_id").HasMaxLength(50);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(100);
            e.Property(x => x.OrderId).HasColumnName("order_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.RequirementType).HasColumnName("requirement_type").HasMaxLength(50);
            e.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            e.Property(x => x.Priority).HasColumnName("priority").HasMaxLength(20);
            e.Property(x => x.IsMandatory).HasColumnName("is_mandatory");
            e.Property(x => x.VerificationMethod).HasColumnName("verification_method").HasMaxLength(100);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.ApprovedAt).HasColumnName("approved_at");
            e.HasIndex(x => x.CustomerId);
        });

        modelBuilder.Entity<MasterMaterial>(e =>
        {
            e.ToTable("master_material");
            e.HasKey(x => x.MaterialId);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(100);
            e.Property(x => x.MaterialType).HasColumnName("material_type").HasMaxLength(50);
            e.Property(x => x.Specification).HasColumnName("specification").HasMaxLength(255);
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.Supplier).HasColumnName("supplier").HasMaxLength(100);
            e.Property(x => x.MinStock).HasColumnName("min_stock");
            e.Property(x => x.CurrentStock).HasColumnName("current_stock");
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.MaterialType);
        });

        modelBuilder.Entity<MaterialRequirement>(e =>
        {
            e.ToTable("material_requirement");
            e.HasKey(x => x.RequirementId);
            e.Property(x => x.RequirementId).HasColumnName("requirement_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.RequiredQty).HasColumnName("required_qty").HasColumnType("decimal(10,2)");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.IsMandatory).HasColumnName("is_mandatory");
            e.HasIndex(x => x.StepCode);
        });

        modelBuilder.Entity<MaterialConsumeEntity>(e =>
        {
            e.ToTable("material_consume");
            e.HasKey(x => x.ConsumeId);
            e.Property(x => x.ConsumeId).HasColumnName("consume_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.MaterialId).HasColumnName("material_id").HasMaxLength(50);
            e.Property(x => x.MaterialName).HasColumnName("material_name").HasMaxLength(100);
            e.Property(x => x.ConsumedQty).HasColumnName("consumed_qty").HasColumnType("decimal(10,2)");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.BatchNo).HasColumnName("batch_no").HasMaxLength(50);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.ConsumedAt).HasColumnName("consumed_at");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.ConsumedAt);
        });

        modelBuilder.Entity<QualityGateEntity>(e =>
        {
            e.ToTable("quality_gate_instance");
            e.HasKey(x => x.GateId);
            e.Property(x => x.GateId).HasColumnName("gate_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.GateType).HasColumnName("gate_type").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CheckedBy).HasColumnName("checked_by").HasMaxLength(50);
            e.Property(x => x.CheckedByName).HasColumnName("checked_by_name").HasMaxLength(100);
            e.Property(x => x.CheckedAt).HasColumnName("checked_at");
            e.Property(x => x.Comment).HasColumnName("comment").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.ExpireAt).HasColumnName("expire_at");
            e.HasIndex(x => new { x.LotId, x.StepCode });
            e.HasIndex(x => x.Status);
        });
    }
}
