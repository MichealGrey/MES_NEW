using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Infrastructure.Persistence;

public partial class MesDbContext : DbContext
{
    public MesDbContext(DbContextOptions<MesDbContext> options) : base(options) { }

    // System
    public DbSet<SysDepartment> SysDepartments => Set<SysDepartment>();
    public DbSet<SysRole> SysRoles => Set<SysRole>();
    public DbSet<SysUser> SysUsers => Set<SysUser>();
    public DbSet<SysUserPermission> SysUserPermissions => Set<SysUserPermission>();
    public DbSet<SysPermissionConfirm> SysPermissionConfirms => Set<SysPermissionConfirm>();
    public DbSet<SysSignatureLevel> SysSignatureLevels => Set<SysSignatureLevel>();
    public DbSet<SysMenu> SysMenus => Set<SysMenu>();
    public DbSet<SysUserRole> SysUserRoles => Set<SysUserRole>();
    public DbSet<SysRoleMenu> SysRoleMenus => Set<SysRoleMenu>();
    public DbSet<SysRolePermission> SysRolePermissions => Set<SysRolePermission>();
    public DbSet<SysLoginLog> SysLoginLogs => Set<SysLoginLog>();

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
    public DbSet<MasterCustomer> MasterCustomers => Set<MasterCustomer>();
    public DbSet<MasterReasonCode> MasterReasonCodes => Set<MasterReasonCode>();
    public DbSet<MasterDefectCode> MasterDefectCodes => Set<MasterDefectCode>();

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

    // MFGID Traceability
    public DbSet<MfgUnit> MfgUnits => Set<MfgUnit>();
    public DbSet<MfgOperationHistory> MfgOperationHistories => Set<MfgOperationHistory>();
    public DbSet<MfgPackTrace> MfgPackTraces => Set<MfgPackTrace>();
    public DbSet<LotTraceChain> LotTraceChains => Set<LotTraceChain>();
    public DbSet<AutoHoldRule> AutoHoldRules => Set<AutoHoldRule>();

    // Assemble / Test Data
    public DbSet<ProdAssembleData> ProdAssembleData => Set<ProdAssembleData>();
    public DbSet<ProdTestData> ProdTestData => Set<ProdTestData>();

    // Quality & Engineering (V3.0.0)
    public DbSet<Complaint8D> Complaint8Ds => Set<Complaint8D>();
    public DbSet<EcnRequest> EcnRequests => Set<EcnRequest>();
    public DbSet<EcnItem> EcnItems => Set<EcnItem>();
    public DbSet<EcnImpactItem> EcnImpactItems => Set<EcnImpactItem>();
    public DbSet<EcnApprover> EcnApprovers => Set<EcnApprover>();
    public DbSet<EcnNotifyDept> EcnNotifyDepts => Set<EcnNotifyDept>();
    public DbSet<EcnImplement> EcnImplements => Set<EcnImplement>();
    public DbSet<NpiProject> NpiProjects => Set<NpiProject>();
    public DbSet<SpcMeasurement> SpcMeasurements => Set<SpcMeasurement>();
    public DbSet<ShiftSchedule> ShiftSchedules => Set<ShiftSchedule>();

    // Quality Inspection (Phase 3.4)
    public DbSet<QualityInspection> QualityInspections => Set<QualityInspection>();
    public DbSet<QualityInspectionItem> QualityInspectionItems => Set<QualityInspectionItem>();
    public DbSet<NonConformanceReport> NonConformanceReports => Set<NonConformanceReport>();

    // Equipment Management (Phase 3.5)
    public DbSet<EquipmentMaintenance> EquipmentMaintenances => Set<EquipmentMaintenance>();
    public DbSet<EquipmentFailure> EquipmentFailures => Set<EquipmentFailure>();

    // Phase 3: Process Control Deepening
    // Process Parameter Control
    public DbSet<ProcessParameterSet> ProcessParameterSets => Set<ProcessParameterSet>();
    public DbSet<ProcessParameterItem> ProcessParameterItems => Set<ProcessParameterItem>();
    public DbSet<ProcessParameterOverrideLog> ProcessParameterOverrideLogs => Set<ProcessParameterOverrideLog>();
    public DbSet<CuringTemperatureCurve> CuringTemperatureCurves => Set<CuringTemperatureCurve>();

    // Bin Control
    public DbSet<BinDefinition> BinDefinitions => Set<BinDefinition>();
    public DbSet<BinSortRecord> BinSortRecords => Set<BinSortRecord>();
    public DbSet<BinStatistics> BinStatistics => Set<BinStatistics>();

    // Wire Material Control
    public DbSet<WireMaterialSwitchRecord> WireMaterialSwitchRecords => Set<WireMaterialSwitchRecord>();
    public DbSet<WireConsumption> WireConsumptions => Set<WireConsumption>();

    // Tooling Management
    public DbSet<ToolingRegistry> ToolingRegistries => Set<ToolingRegistry>();
    public DbSet<ToolingUsageLog> ToolingUsageLogs => Set<ToolingUsageLog>();
    public DbSet<ToolingReplacementRecord> ToolingReplacementRecords => Set<ToolingReplacementRecord>();

    // Operator Qualification
    public DbSet<OperatorQualification> OperatorQualifications => Set<OperatorQualification>();
    public DbSet<QualificationCheckLog> QualificationCheckLogs => Set<QualificationCheckLog>();

    // Bond Pull Test (already defined in Phase 1)
    // BondPullTestRecord is defined in ProductionAbnormalEntities.cs and MesDbContext.Phase1.cs

    // Phase 5: Analytics, NPI, and Audit
    public DbSet<KpiDashboardSnapshot> KpiDashboardSnapshots => Set<KpiDashboardSnapshot>();
    public DbSet<CostRecord> CostRecords => Set<CostRecord>();
    public DbSet<YieldStatistics> YieldStatistics => Set<YieldStatistics>();
    // NpiProjects already defined in Phase 3 (NpiProject entity)
    public DbSet<NpiStage> NpiStages => Set<NpiStage>();
    public DbSet<ReliabilityTestPlan> ReliabilityTestPlans => Set<ReliabilityTestPlan>();
    public DbSet<ReportSchedule> ReportSchedules => Set<ReportSchedule>();
    public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();
    public DbSet<AlertRule> AlertRules => Set<AlertRule>();
    public DbSet<DataCorrectionRecord> DataCorrectionRecords => Set<DataCorrectionRecord>();

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

        modelBuilder.Entity<SysPermissionConfirm>(e =>
        {
            e.ToTable("sys_permission_confirm");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.OperationType).HasColumnName("operation_type").HasMaxLength(50);
            e.Property(x => x.EmployeeId).HasColumnName("employee_id").HasMaxLength(50);
            e.Property(x => x.EmployeeName).HasColumnName("employee_name").HasMaxLength(100);
            e.Property(x => x.RequiredLevel).HasColumnName("required_level").HasMaxLength(20);
            e.Property(x => x.Success).HasColumnName("success");
            e.Property(x => x.Reason).HasColumnName("reason").HasMaxLength(255);
            e.Property(x => x.ConfirmAt).HasColumnName("confirm_at");
            e.HasIndex(x => x.EmployeeId);
            e.HasIndex(x => x.ConfirmAt);
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
            e.Property(x => x.ProcessStage).HasColumnName("process_stage").HasMaxLength(20);
            e.Property(x => x.DefaultRouteId).HasColumnName("default_route_id").HasMaxLength(100);
            e.Property(x => x.UnitQty).HasColumnName("unit_qty");
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
            e.Property(x => x.ProcessStage).HasColumnName("process_stage").HasMaxLength(20);
            e.Property(x => x.Vendor).HasColumnName("vendor").HasMaxLength(100);
            e.Property(x => x.Model).HasColumnName("model").HasMaxLength(100);
            e.Property(x => x.SerialNumber).HasColumnName("serial_number").HasMaxLength(100);
            e.Property(x => x.Capability).HasColumnName("capability").HasColumnType("json");
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
            e.Property(x => x.ApplicableProcess).HasColumnName("applicable_process").HasColumnType("json");
            e.Property(x => x.ApplicablePackage).HasColumnName("applicable_package").HasColumnType("json");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.CarrierType);
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<MasterCustomer>(e =>
        {
            e.ToTable("master_customer");
            e.HasKey(x => x.CustomerId);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(100);
            e.Property(x => x.CustomerCode).HasColumnName("customer_code").HasMaxLength(50);
            e.Property(x => x.ContactPerson).HasColumnName("contact_person").HasMaxLength(100);
            e.Property(x => x.ContactPhone).HasColumnName("contact_phone").HasMaxLength(50);
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(100);
            e.Property(x => x.Address).HasColumnName("address").HasMaxLength(255);
            e.Property(x => x.CustomerPnPrefix).HasColumnName("customer_pn_prefix").HasMaxLength(20);
            e.Property(x => x.QualityLevel).HasColumnName("quality_level").HasMaxLength(20);
            e.Property(x => x.SpecialRequirements).HasColumnName("special_requirements").HasColumnType("json");
            e.Property(x => x.DefaultPackingSpec).HasColumnName("default_packing_spec").HasMaxLength(100);
            e.Property(x => x.DefaultOqcSpec).HasColumnName("default_oqc_spec").HasMaxLength(100);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.CustomerCode).IsUnique().HasDatabaseName("uk_customer_code");
        });

        modelBuilder.Entity<MasterReasonCode>(e =>
        {
            e.ToTable("master_reason_code");
            e.HasKey(x => x.ReasonCodeId);
            e.Property(x => x.ReasonCodeId).HasColumnName("reason_code_id").HasMaxLength(50);
            e.Property(x => x.Category).HasColumnName("category").HasMaxLength(50);
            e.Property(x => x.SubCategory).HasColumnName("sub_category").HasMaxLength(50);
            e.Property(x => x.ReasonText).HasColumnName("reason_text").HasMaxLength(255);
            e.Property(x => x.ApplicableTo).HasColumnName("applicable_to").HasMaxLength(50);
            e.Property(x => x.IsEnabled).HasColumnName("is_enabled");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.Category);
        });

        modelBuilder.Entity<MasterDefectCode>(e =>
        {
            e.ToTable("master_defect_code");
            e.HasKey(x => x.DefectCodeId);
            e.Property(x => x.DefectCodeId).HasColumnName("defect_code_id").HasMaxLength(50);
            e.Property(x => x.DefectCategory).HasColumnName("defect_category").HasMaxLength(50);
            e.Property(x => x.DefectText).HasColumnName("defect_text").HasMaxLength(255);
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.IsEnabled).HasColumnName("is_enabled");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.DefectCategory);
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

        // MFGID Traceability
        modelBuilder.Entity<MfgUnit>(e =>
        {
            e.ToTable("mfg_unit");
            e.HasKey(x => x.MfgId);
            e.Property(x => x.MfgId).HasColumnName("mfg_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.RootWaferLotId).HasColumnName("root_wafer_lot_id").HasMaxLength(50);
            e.Property(x => x.WaferId).HasColumnName("wafer_id").HasMaxLength(50);
            e.Property(x => x.DieX).HasColumnName("die_x");
            e.Property(x => x.DieY).HasColumnName("die_y");
            e.Property(x => x.SerialNumber).HasColumnName("serial_number").HasMaxLength(50);
            e.Property(x => x.ReelId).HasColumnName("reel_id").HasMaxLength(50);
            e.Property(x => x.ReelCapacity).HasColumnName("reel_capacity");
            e.Property(x => x.ActualQty).HasColumnName("actual_qty");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Grade).HasColumnName("grade").HasMaxLength(10);
            e.Property(x => x.BinResult).HasColumnName("bin_result").HasMaxLength(20);
            e.Property(x => x.CurrentStep).HasColumnName("current_step").HasMaxLength(50);
            e.Property(x => x.ReworkCount).HasColumnName("rework_count");
            e.Property(x => x.PackTime).HasColumnName("pack_time");
            e.Property(x => x.PackedBy).HasColumnName("packed_by").HasMaxLength(50);
            e.Property(x => x.BoxId).HasColumnName("box_id").HasMaxLength(50);
            e.Property(x => x.PalletId).HasColumnName("pallet_id").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.CompletedAt).HasColumnName("completed_at");
            e.Property(x => x.ShippedAt).HasColumnName("shipped_at");
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.RootWaferLotId);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.Grade);
            e.HasIndex(x => x.ReelId);
        });

        modelBuilder.Entity<MfgOperationHistory>(e =>
        {
            e.ToTable("mfg_operation_history");
            e.HasKey(x => x.HistoryId);
            e.Property(x => x.HistoryId).HasColumnName("history_id").HasMaxLength(50);
            e.Property(x => x.MfgId).HasColumnName("mfg_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.Operation).HasColumnName("operation").HasMaxLength(20);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.Result).HasColumnName("result").HasMaxLength(20);
            e.Property(x => x.TestData).HasColumnName("test_data").HasColumnType("json");
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.MfgId);
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.StepCode);
        });

        modelBuilder.Entity<MfgPackTrace>(e =>
        {
            e.ToTable("mfg_pack_trace");
            e.HasKey(x => x.PackTraceId);
            e.Property(x => x.PackTraceId).HasColumnName("pack_trace_id").HasMaxLength(50);
            e.Property(x => x.MfgId).HasColumnName("mfg_id").HasMaxLength(50);
            e.Property(x => x.PackLevel).HasColumnName("pack_level").HasMaxLength(20);
            e.Property(x => x.PackId).HasColumnName("pack_id").HasMaxLength(50);
            e.Property(x => x.ParentPackId).HasColumnName("parent_pack_id").HasMaxLength(50);
            e.Property(x => x.PackQty).HasColumnName("pack_qty");
            e.Property(x => x.PackedAt).HasColumnName("packed_at");
            e.Property(x => x.PackedBy).HasColumnName("packed_by").HasMaxLength(50);
            e.HasIndex(x => x.MfgId);
            e.HasIndex(x => x.PackId);
        });

        modelBuilder.Entity<LotTraceChain>(e =>
        {
            e.ToTable("lot_trace_chain");
            e.HasKey(x => x.ChainId);
            e.Property(x => x.ChainId).HasColumnName("chain_id").HasMaxLength(50);
            e.Property(x => x.ChildLotId).HasColumnName("child_lot_id").HasMaxLength(50);
            e.Property(x => x.ParentLotId).HasColumnName("parent_lot_id").HasMaxLength(50);
            e.Property(x => x.RelationType).HasColumnName("relation_type").HasMaxLength(20);
            e.Property(x => x.SplitQty).HasColumnName("split_qty");
            e.Property(x => x.MergeSourceLotIds).HasColumnName("merge_source_lot_ids").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.HasIndex(x => x.ChildLotId);
            e.HasIndex(x => x.ParentLotId);
        });

        modelBuilder.Entity<AutoHoldRule>(e =>
        {
            e.ToTable("auto_hold_rule");
            e.HasKey(x => x.RuleId);
            e.Property(x => x.RuleId).HasColumnName("rule_id").HasMaxLength(50);
            e.Property(x => x.RuleName).HasColumnName("rule_name").HasMaxLength(100);
            e.Property(x => x.RuleType).HasColumnName("rule_type").HasMaxLength(20);
            e.Property(x => x.HoldScope).HasColumnName("hold_scope").HasMaxLength(20);
            e.Property(x => x.TriggerCondition).HasColumnName("trigger_condition").HasColumnType("json");
            e.Property(x => x.HoldReasonCode).HasColumnName("hold_reason_code").HasMaxLength(50);
            e.Property(x => x.HoldReason).HasColumnName("hold_reason").HasMaxLength(500);
            e.Property(x => x.AutoRelease).HasColumnName("auto_release");
            e.Property(x => x.AutoReleaseCondition).HasColumnName("auto_release_condition").HasColumnType("json");
            e.Property(x => x.IsActive).HasColumnName("is_active");
            e.Property(x => x.Priority).HasColumnName("priority");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.RuleType);
            e.HasIndex(x => x.IsActive);
        });

        // Assemble / Test Data
        modelBuilder.Entity<ProdAssembleData>(e =>
        {
            e.ToTable("prod_assemble_data");
            e.HasKey(x => x.AssembleId);
            e.Property(x => x.AssembleId).HasColumnName("assemble_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.MfgId).HasColumnName("mfg_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.RecipeId).HasColumnName("recipe_id").HasMaxLength(100);
            e.Property(x => x.CarrierId).HasColumnName("carrier_id").HasMaxLength(50);
            e.Property(x => x.InputQty).HasColumnName("input_qty");
            e.Property(x => x.OutputQty).HasColumnName("output_qty");
            e.Property(x => x.ScrapQty).HasColumnName("scrap_qty");
            e.Property(x => x.ReworkQty).HasColumnName("rework_qty");
            e.Property(x => x.WireBondCount).HasColumnName("wire_bond_count");
            e.Property(x => x.BondForce).HasColumnName("bond_force").HasPrecision(5, 2);
            e.Property(x => x.BondTemp).HasColumnName("bond_temp").HasPrecision(5, 2);
            e.Property(x => x.BondTime).HasColumnName("bond_time");
            e.Property(x => x.UltrasonicPower).HasColumnName("ultrasonic_power").HasPrecision(5, 2);
            e.Property(x => x.MoldPressure).HasColumnName("mold_pressure").HasPrecision(5, 2);
            e.Property(x => x.MoldTemp).HasColumnName("mold_temp").HasPrecision(5, 2);
            e.Property(x => x.MoldTime).HasColumnName("mold_time");
            e.Property(x => x.TransferPressure).HasColumnName("transfer_pressure").HasPrecision(5, 2);
            e.Property(x => x.CureTime).HasColumnName("cure_time");
            e.Property(x => x.CureTemp).HasColumnName("cure_temp").HasPrecision(5, 2);
            e.Property(x => x.DieAttachForce).HasColumnName("die_attach_force").HasPrecision(5, 2);
            e.Property(x => x.DieAttachTemp).HasColumnName("die_attach_temp").HasPrecision(5, 2);
            e.Property(x => x.DieAttachOffsetX).HasColumnName("die_attach_offset_x").HasPrecision(5, 2);
            e.Property(x => x.DieAttachOffsetY).HasColumnName("die_attach_offset_y").HasPrecision(5, 2);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.StartTime).HasColumnName("start_time");
            e.Property(x => x.EndTime).HasColumnName("end_time");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Detail).HasColumnName("detail").HasColumnType("json");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.MfgId);
            e.HasIndex(x => x.StepCode);
            e.HasIndex(x => x.EquipmentId);
            e.HasIndex(x => x.StartTime);
        });

        modelBuilder.Entity<ProdTestData>(e =>
        {
            e.ToTable("prod_test_data");
            e.HasKey(x => x.TestId);
            e.Property(x => x.TestId).HasColumnName("test_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.MfgId).HasColumnName("mfg_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.StepSeq).HasColumnName("step_seq");
            e.Property(x => x.TestProgram).HasColumnName("test_program").HasMaxLength(100);
            e.Property(x => x.TestVersion).HasColumnName("test_version").HasMaxLength(20);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.HandlerId).HasColumnName("handler_id").HasMaxLength(50);
            e.Property(x => x.InputQty).HasColumnName("input_qty");
            e.Property(x => x.PassQty).HasColumnName("pass_qty");
            e.Property(x => x.FailQty).HasColumnName("fail_qty");
            e.Property(x => x.ScrapQty).HasColumnName("scrap_qty");
            e.Property(x => x.RetestQty).HasColumnName("retest_qty");
            e.Property(x => x.TestTemp).HasColumnName("test_temp").HasPrecision(5, 2);
            e.Property(x => x.TestVoltage).HasColumnName("test_voltage").HasPrecision(5, 2);
            e.Property(x => x.TestCurrent).HasColumnName("test_current").HasPrecision(5, 2);
            e.Property(x => x.TestFrequency).HasColumnName("test_frequency").HasPrecision(10, 2);
            e.Property(x => x.BinSummary).HasColumnName("bin_summary").HasMaxLength(255);
            e.Property(x => x.Bin1Qty).HasColumnName("bin1_qty");
            e.Property(x => x.Bin2Qty).HasColumnName("bin2_qty");
            e.Property(x => x.Bin3Qty).HasColumnName("bin3_qty");
            e.Property(x => x.Bin4Qty).HasColumnName("bin4_qty");
            e.Property(x => x.Bin5Qty).HasColumnName("bin5_qty");
            e.Property(x => x.Bin6Qty).HasColumnName("bin6_qty");
            e.Property(x => x.Bin7Qty).HasColumnName("bin7_qty");
            e.Property(x => x.Bin8Qty).HasColumnName("bin8_qty");
            e.Property(x => x.YieldPercent).HasColumnName("yield_percent").HasPrecision(5, 2);
            e.Property(x => x.FirstPassYield).HasColumnName("first_pass_yield").HasPrecision(5, 2);
            e.Property(x => x.FinalYield).HasColumnName("final_yield").HasPrecision(5, 2);
            e.Property(x => x.TestResult).HasColumnName("test_result").HasMaxLength(50);
            e.Property(x => x.ParametricData).HasColumnName("parametric_data").HasColumnType("json");
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
            e.Property(x => x.StartTime).HasColumnName("start_time");
            e.Property(x => x.EndTime).HasColumnName("end_time");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Detail).HasColumnName("detail").HasColumnType("json");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.LotId);
            e.HasIndex(x => x.MfgId);
            e.HasIndex(x => x.StepCode);
            e.HasIndex(x => x.EquipmentId);
            e.HasIndex(x => x.TestProgram);
            e.HasIndex(x => x.StartTime);
        });

        // V3.0.0 - Permission System
        modelBuilder.Entity<SysMenu>(e =>
        {
            e.ToTable("sys_menu");
            e.HasKey(x => x.MenuId);
            e.Property(x => x.MenuId).HasColumnName("menu_id").HasMaxLength(50);
            e.Property(x => x.MenuName).HasColumnName("menu_name").HasMaxLength(100);
            e.Property(x => x.ParentId).HasColumnName("parent_id").HasMaxLength(50);
            e.Property(x => x.Icon).HasColumnName("icon").HasMaxLength(50);
            e.Property(x => x.ViewName).HasColumnName("view_name").HasMaxLength(100);
            e.Property(x => x.ModuleKey).HasColumnName("module_key").HasMaxLength(50);
            e.Property(x => x.PermissionCode).HasColumnName("permission_code").HasMaxLength(100);
            e.Property(x => x.SortOrder).HasColumnName("sort_order");
            e.Property(x => x.IsVisible).HasColumnName("is_visible");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.ParentId);
        });

        modelBuilder.Entity<SysUserRole>(e =>
        {
            e.ToTable("sys_user_role");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(50);
            e.Property(x => x.RoleId).HasColumnName("role_id").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique().HasDatabaseName("uk_user_role");
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.RoleId);
        });

        modelBuilder.Entity<SysRoleMenu>(e =>
        {
            e.ToTable("sys_role_menu");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.RoleId).HasColumnName("role_id").HasMaxLength(50);
            e.Property(x => x.MenuId).HasColumnName("menu_id").HasMaxLength(50);
            e.HasIndex(x => new { x.RoleId, x.MenuId }).IsUnique().HasDatabaseName("uk_role_menu");
            e.HasIndex(x => x.RoleId);
        });

        modelBuilder.Entity<SysRolePermission>(e =>
        {
            e.ToTable("sys_role_permission");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.RoleId).HasColumnName("role_id").HasMaxLength(50);
            e.Property(x => x.PermissionCode).HasColumnName("permission_code").HasMaxLength(100);
            e.HasIndex(x => new { x.RoleId, x.PermissionCode }).IsUnique().HasDatabaseName("uk_role_perm");
            e.HasIndex(x => x.RoleId);
        });

        modelBuilder.Entity<SysLoginLog>(e =>
        {
            e.ToTable("sys_login_log");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(50);
            e.Property(x => x.EmployeeId).HasColumnName("employee_id").HasMaxLength(50);
            e.Property(x => x.LoginTime).HasColumnName("login_time");
            e.Property(x => x.IpAddress).HasColumnName("ip_address").HasMaxLength(50);
            e.Property(x => x.Result).HasColumnName("result").HasMaxLength(20);
            e.Property(x => x.ErrorMessage).HasColumnName("error_message").HasColumnType("text");
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.LoginTime);
        });

        // V3.1.0 - Complaint 8D Enhancement
        modelBuilder.Entity<Complaint8D>(e =>
        {
            e.ToTable("complaint_8d");
            e.HasKey(x => x.ComplaintId);
            e.Property(x => x.ComplaintId).HasColumnName("complaint_id").HasMaxLength(50);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.CustomerName).HasColumnName("customer_name").HasMaxLength(100);
            e.Property(x => x.OrderNo).HasColumnName("order_no").HasMaxLength(50);
            e.Property(x => x.CustomerPONO).HasColumnName("customer_po_no").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.ProductName).HasColumnName("product_name").HasMaxLength(100);
            e.Property(x => x.DefectType).HasColumnName("defect_type").HasMaxLength(50);
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.Priority).HasColumnName("priority").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.EightDStatus).HasColumnName("eight_d_status").HasMaxLength(10);
            e.Property(x => x.AffectedQty).HasColumnName("affected_qty");
            e.Property(x => x.ReturnQty).HasColumnName("return_qty");
            e.Property(x => x.SampleQty).HasColumnName("sample_qty");

            // D0
            e.Property(x => x.D0Assessment).HasColumnName("d0_assessment");
            e.Property(x => x.D0AssessmentComment).HasColumnName("d0_assessment_comment").HasColumnType("text");
            e.Property(x => x.D0Date).HasColumnName("d0_date");

            // D1
            e.Property(x => x.D1TeamMembers).HasColumnName("d1_team_members").HasColumnType("json");
            e.Property(x => x.D1Date).HasColumnName("d1_date");

            // D2
            e.Property(x => x.D2ProblemDescription).HasColumnName("d2_problem_description").HasColumnType("text");
            e.Property(x => x.D2What).HasColumnName("d2_what").HasColumnType("text");
            e.Property(x => x.D2Who).HasColumnName("d2_who").HasMaxLength(200);
            e.Property(x => x.D2Where).HasColumnName("d2_where").HasMaxLength(200);
            e.Property(x => x.D2When).HasColumnName("d2_when").HasMaxLength(200);
            e.Property(x => x.D2Why).HasColumnName("d2_why").HasColumnType("text");
            e.Property(x => x.D2How).HasColumnName("d2_how").HasColumnType("text");
            e.Property(x => x.D2HowMany).HasColumnName("d2_how_many").HasMaxLength(200);
            e.Property(x => x.D2DefectLocation).HasColumnName("d2_defect_location").HasMaxLength(200);
            e.Property(x => x.D2OccurrenceDate).HasColumnName("d2_occurrence_date");
            e.Property(x => x.D2DiscoveryDate).HasColumnName("d2_discovery_date");
            e.Property(x => x.D2DiscoveryMethod).HasColumnName("d2_discovery_method").HasMaxLength(100);
            e.Property(x => x.D2Date).HasColumnName("d2_date");

            // D3
            e.Property(x => x.D3ContainmentAction).HasColumnName("d3_containment_action").HasColumnType("text");
            e.Property(x => x.D3ContainmentResult).HasColumnName("d3_containment_result").HasColumnType("text");
            e.Property(x => x.D3ContainmentDate).HasColumnName("d3_containment_date");
            e.Property(x => x.D3Date).HasColumnName("d3_date");

            // D4
            e.Property(x => x.D4RootCause).HasColumnName("d4_root_cause").HasColumnType("text");
            e.Property(x => x.D4AnalysisMethod).HasColumnName("d4_analysis_method").HasMaxLength(100);
            e.Property(x => x.D4OccurrenceCause).HasColumnName("d4_occurrence_cause").HasColumnType("text");
            e.Property(x => x.D4EscapeCause).HasColumnName("d4_escape_cause").HasColumnType("text");
            e.Property(x => x.D4Date).HasColumnName("d4_date");

            // D5
            e.Property(x => x.D5PermanentAction).HasColumnName("d5_permanent_action").HasColumnType("text");
            e.Property(x => x.D5ActionValidation).HasColumnName("d5_action_validation").HasColumnType("text");
            e.Property(x => x.D5ValidationDate).HasColumnName("d5_validation_date");
            e.Property(x => x.D5Date).HasColumnName("d5_date");

            // D6
            e.Property(x => x.D6Implementation).HasColumnName("d6_implementation").HasColumnType("text");
            e.Property(x => x.D6VerificationResult).HasColumnName("d6_verification_result").HasColumnType("text");
            e.Property(x => x.D6ImplementDate).HasColumnName("d6_implement_date");
            e.Property(x => x.D6Date).HasColumnName("d6_date");

            // D7
            e.Property(x => x.D7Prevention).HasColumnName("d7_prevention").HasColumnType("text");
            e.Property(x => x.D7DocUpdateList).HasColumnName("d7_doc_update_list").HasColumnType("json");
            e.Property(x => x.D7Standardization).HasColumnName("d7_standardization").HasColumnType("text");
            e.Property(x => x.D7HorizontalExpand).HasColumnName("d7_horizontal_expand").HasColumnType("text");
            e.Property(x => x.D7Date).HasColumnName("d7_date");

            // D8
            e.Property(x => x.D8ClosureComment).HasColumnName("d8_closure_comment").HasColumnType("text");
            e.Property(x => x.D8TeamRecognition).HasColumnName("d8_team_recognition").HasColumnType("text");
            e.Property(x => x.D8EffectivenessConfirm).HasColumnName("d8_effectiveness_confirm").HasColumnType("text");
            e.Property(x => x.D8Date).HasColumnName("d8_date");

            // 审批
            e.Property(x => x.ApprovalStatus).HasColumnName("approval_status").HasMaxLength(20);
            e.Property(x => x.Approver).HasColumnName("approver").HasMaxLength(50);
            e.Property(x => x.ApproveDate).HasColumnName("approve_date");
            e.Property(x => x.ApprovalComment).HasColumnName("approval_comment").HasColumnType("text");

            // 通用
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.ClosedAt).HasColumnName("closed_at");
            e.Property(x => x.ClosedBy).HasColumnName("closed_by").HasMaxLength(50);
            e.Property(x => x.DueDate).HasColumnName("due_date");
            e.Property(x => x.OverdueDays).HasColumnName("overdue_days");
            e.Property(x => x.Attachments).HasColumnName("attachments").HasColumnType("json");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");

            e.HasIndex(x => x.CustomerId);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.EightDStatus);
            e.HasIndex(x => x.CreatedAt);
            e.HasIndex(x => x.DueDate);
        });

        // Sub-entities for Complaint 8D
        modelBuilder.Entity<Complaint8DTeamMember>(e =>
        {
            e.ToTable("complaint_8d_team_member");
            e.HasKey(x => x.MemberId);
            e.Property(x => x.MemberId).HasColumnName("member_id").HasMaxLength(50);
            e.Property(x => x.ComplaintId).HasColumnName("complaint_id").HasMaxLength(50);
            e.Property(x => x.MemberName).HasColumnName("member_name").HasMaxLength(50);
            e.Property(x => x.Department).HasColumnName("department").HasMaxLength(50);
            e.Property(x => x.Role).HasColumnName("role").HasMaxLength(50);
            e.Property(x => x.ContactInfo).HasColumnName("contact_info").HasMaxLength(100);
            e.Property(x => x.JoinDate).HasColumnName("join_date");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.HasIndex(x => x.ComplaintId);
        });

        modelBuilder.Entity<Complaint8DContainment>(e =>
        {
            e.ToTable("complaint_8d_containment");
            e.HasKey(x => x.ContainmentId);
            e.Property(x => x.ContainmentId).HasColumnName("containment_id").HasMaxLength(50);
            e.Property(x => x.ComplaintId).HasColumnName("complaint_id").HasMaxLength(50);
            e.Property(x => x.ActionDescription).HasColumnName("action_description").HasColumnType("text");
            e.Property(x => x.AffectedLot).HasColumnName("affected_lot").HasMaxLength(50);
            e.Property(x => x.AffectedQty).HasColumnName("affected_qty");
            e.Property(x => x.ContainedQty).HasColumnName("contained_qty");
            e.Property(x => x.Disposition).HasColumnName("disposition").HasMaxLength(50);
            e.Property(x => x.Result).HasColumnName("result").HasColumnType("text");
            e.Property(x => x.ResponsiblePerson).HasColumnName("responsible_person").HasMaxLength(50);
            e.Property(x => x.PlanDate).HasColumnName("plan_date");
            e.Property(x => x.ActualDate).HasColumnName("actual_date");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.HasIndex(x => x.ComplaintId);
        });

        modelBuilder.Entity<Complaint8DRootCause>(e =>
        {
            e.ToTable("complaint_8d_root_cause");
            e.HasKey(x => x.CauseId);
            e.Property(x => x.CauseId).HasColumnName("cause_id").HasMaxLength(50);
            e.Property(x => x.ComplaintId).HasColumnName("complaint_id").HasMaxLength(50);
            e.Property(x => x.CauseType).HasColumnName("cause_type").HasMaxLength(20);
            e.Property(x => x.AnalysisMethod).HasColumnName("analysis_method").HasMaxLength(50);
            e.Property(x => x.CauseDescription).HasColumnName("cause_description").HasColumnType("text");
            e.Property(x => x.Why1).HasColumnName("why_1").HasColumnType("text");
            e.Property(x => x.Why2).HasColumnName("why_2").HasColumnType("text");
            e.Property(x => x.Why3).HasColumnName("why_3").HasColumnType("text");
            e.Property(x => x.Why4).HasColumnName("why_4").HasColumnType("text");
            e.Property(x => x.Why5).HasColumnName("why_5").HasColumnType("text");
            e.Property(x => x.RootCauseConclusion).HasColumnName("root_cause_conclusion").HasColumnType("text");
            e.Property(x => x.ResponsiblePerson).HasColumnName("responsible_person").HasMaxLength(50);
            e.Property(x => x.AnalysisDate).HasColumnName("analysis_date");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.HasIndex(x => x.ComplaintId);
        });

        modelBuilder.Entity<Complaint8DAction>(e =>
        {
            e.ToTable("complaint_8d_action");
            e.HasKey(x => x.ActionId);
            e.Property(x => x.ActionId).HasColumnName("action_id").HasMaxLength(50);
            e.Property(x => x.ComplaintId).HasColumnName("complaint_id").HasMaxLength(50);
            e.Property(x => x.ActionType).HasColumnName("action_type").HasMaxLength(20);
            e.Property(x => x.ActionDescription).HasColumnName("action_description").HasColumnType("text");
            e.Property(x => x.ResponsiblePerson).HasColumnName("responsible_person").HasMaxLength(50);
            e.Property(x => x.PlanDate).HasColumnName("plan_date");
            e.Property(x => x.ActualDate).HasColumnName("actual_date");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.VerificationMethod).HasColumnName("verification_method").HasMaxLength(200);
            e.Property(x => x.VerificationResult).HasColumnName("verification_result").HasColumnType("text");
            e.Property(x => x.VerificationDate).HasColumnName("verification_date");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.HasIndex(x => x.ComplaintId);
        });

        modelBuilder.Entity<Complaint8DDocUpdate>(e =>
        {
            e.ToTable("complaint_8d_doc_update");
            e.HasKey(x => x.DocId);
            e.Property(x => x.DocId).HasColumnName("doc_id").HasMaxLength(50);
            e.Property(x => x.ComplaintId).HasColumnName("complaint_id").HasMaxLength(50);
            e.Property(x => x.DocType).HasColumnName("doc_type").HasMaxLength(50);
            e.Property(x => x.DocName).HasColumnName("doc_name").HasMaxLength(200);
            e.Property(x => x.DocNo).HasColumnName("doc_no").HasMaxLength(50);
            e.Property(x => x.UpdateDescription).HasColumnName("update_description").HasColumnType("text");
            e.Property(x => x.ResponsiblePerson).HasColumnName("responsible_person").HasMaxLength(50);
            e.Property(x => x.PlanDate).HasColumnName("plan_date");
            e.Property(x => x.ActualDate).HasColumnName("actual_date");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.HasIndex(x => x.ComplaintId);
        });

        modelBuilder.Entity<Complaint8DAttachment>(e =>
        {
            e.ToTable("complaint_8d_attachment");
            e.HasKey(x => x.AttachmentId);
            e.Property(x => x.AttachmentId).HasColumnName("attachment_id").HasMaxLength(50);
            e.Property(x => x.ComplaintId).HasColumnName("complaint_id").HasMaxLength(50);
            e.Property(x => x.FileName).HasColumnName("file_name").HasMaxLength(200);
            e.Property(x => x.FilePath).HasColumnName("file_path").HasMaxLength(500);
            e.Property(x => x.FileType).HasColumnName("file_type").HasMaxLength(20);
            e.Property(x => x.FileSize).HasColumnName("file_size");
            e.Property(x => x.UploadStage).HasColumnName("upload_stage").HasMaxLength(10);
            e.Property(x => x.UploadedBy).HasColumnName("uploaded_by").HasMaxLength(50);
            e.Property(x => x.UploadedAt).HasColumnName("uploaded_at");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.HasIndex(x => x.ComplaintId);
            e.HasIndex(x => x.UploadStage);
        });

        modelBuilder.Entity<EcnRequest>(e =>
        {
            e.ToTable("ecn_request");
            e.HasKey(x => x.EcnId);
            e.Property(x => x.EcnId).HasColumnName("ecn_id").HasMaxLength(50);
            e.Property(x => x.EcnNo).HasColumnName("ecn_no").HasMaxLength(50);
            e.Property(x => x.EcnTitle).HasColumnName("ecn_title").HasMaxLength(200);
            e.Property(x => x.EcnType).HasColumnName("ecn_type").HasMaxLength(50);
            e.Property(x => x.ChangeCategory).HasColumnName("change_category").HasMaxLength(50);
            e.Property(x => x.Reason).HasColumnName("reason").HasColumnType("text");
            e.Property(x => x.ChangeDescription).HasColumnName("change_description").HasColumnType("text");
            e.Property(x => x.ChangeContent).HasColumnName("change_content").HasColumnType("text");
            e.Property(x => x.OldValue).HasColumnName("old_value").HasMaxLength(500);
            e.Property(x => x.NewValue).HasColumnName("new_value").HasMaxLength(500);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.AffectedRoutes).HasColumnName("affected_routes").HasColumnType("json");
            e.Property(x => x.AffectedProducts).HasColumnName("affected_products").HasColumnType("json");
            e.Property(x => x.ImpactAssessment).HasColumnName("impact_assessment").HasColumnType("text");
            e.Property(x => x.Urgency).HasColumnName("urgency").HasMaxLength(20);
            e.Property(x => x.RiskLevel).HasColumnName("risk_level").HasMaxLength(20);
            e.Property(x => x.ReviewComments).HasColumnName("review_comments").HasColumnType("text");
            e.Property(x => x.RejectReason).HasColumnName("reject_reason").HasColumnType("text");
            e.Property(x => x.VerifyResult).HasColumnName("verify_result").HasColumnType("text");
            e.Property(x => x.RequestedBy).HasColumnName("requested_by").HasMaxLength(50);
            e.Property(x => x.RequestedAt).HasColumnName("requested_at");
            e.Property(x => x.ApprovedBy).HasColumnName("approved_by").HasMaxLength(50);
            e.Property(x => x.ApprovedAt).HasColumnName("approved_at");
            e.Property(x => x.EffectiveDate).HasColumnName("effective_date");
            e.Property(x => x.PlannedDate).HasColumnName("planned_date");
            e.Property(x => x.ActualDate).HasColumnName("actual_date");
            e.Property(x => x.IsComplete).HasColumnName("is_complete").HasDefaultValue(false);
            e.Property(x => x.CloseDate).HasColumnName("close_date");
            e.Property(x => x.DaysElapsed).HasColumnName("days_elapsed");
            e.Property(x => x.OAFlowId).HasColumnName("oa_flow_id").HasMaxLength(50);
            e.Property(x => x.OANo).HasColumnName("oa_no").HasMaxLength(50);
            e.Property(x => x.IsUrgent).HasColumnName("is_urgent").HasDefaultValue(false);
            e.Property(x => x.CostEstimate).HasColumnName("cost_estimate").HasColumnType("decimal(15,2)");
            e.Property(x => x.Remark).HasColumnName("remark").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.EcnNo);
            e.HasIndex(x => x.RequestedAt);
            e.HasIndex(x => x.EcnType);
        });

        modelBuilder.Entity<EcnItem>(e =>
        {
            e.ToTable("ecn_item");
            e.HasKey(x => x.ItemId);
            e.Property(x => x.ItemId).HasColumnName("item_id").HasMaxLength(50);
            e.Property(x => x.EcnId).HasColumnName("ecn_id").HasMaxLength(50);
            e.Property(x => x.ItemType).HasColumnName("item_type").HasMaxLength(50);
            e.Property(x => x.ItemCode).HasColumnName("item_code").HasMaxLength(100);
            e.Property(x => x.ItemName).HasColumnName("item_name").HasMaxLength(200);
            e.Property(x => x.OldValue).HasColumnName("old_value").HasColumnType("text");
            e.Property(x => x.NewValue).HasColumnName("new_value").HasColumnType("text");
            e.Property(x => x.ChangeReason).HasColumnName("change_reason").HasColumnType("text");
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.SortOrder).HasColumnName("sort_order").HasDefaultValue(0);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.HasIndex(x => x.EcnId);
            e.HasIndex(x => x.ItemCode);
        });

        modelBuilder.Entity<EcnImpactItem>(e =>
        {
            e.ToTable("ecn_impact_item");
            e.HasKey(x => x.ImpactId);
            e.Property(x => x.ImpactId).HasColumnName("impact_id").HasMaxLength(50);
            e.Property(x => x.EcnId).HasColumnName("ecn_id").HasMaxLength(50);
            e.Property(x => x.ImpactType).HasColumnName("impact_type").HasMaxLength(50);
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            e.Property(x => x.ImpactAnalysis).HasColumnName("impact_analysis").HasColumnType("text");
            e.Property(x => x.Action).HasColumnName("action").HasColumnType("text");
            e.Property(x => x.Responsible).HasColumnName("responsible").HasMaxLength(50);
            e.Property(x => x.DueDate).HasColumnName("due_date");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.HasIndex(x => x.EcnId);
            e.HasIndex(x => x.ImpactType);
            e.HasIndex(x => x.Severity);
        });

        modelBuilder.Entity<EcnApprover>(e =>
        {
            e.ToTable("ecn_approver");
            e.HasKey(x => x.ApproverId);
            e.Property(x => x.ApproverId).HasColumnName("approver_id").HasMaxLength(50);
            e.Property(x => x.EcnId).HasColumnName("ecn_id").HasMaxLength(50);
            e.Property(x => x.ApproverName).HasColumnName("approver_name").HasMaxLength(50);
            e.Property(x => x.Role).HasColumnName("role").HasMaxLength(50);
            e.Property(x => x.ApprovalOrder).HasColumnName("approval_order").HasDefaultValue(1);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Result).HasColumnName("result").HasMaxLength(50);
            e.Property(x => x.Comments).HasColumnName("comments").HasColumnType("text");
            e.Property(x => x.ApprovedAt).HasColumnName("approved_at");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.EcnId);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.ApprovalOrder);
        });

        modelBuilder.Entity<EcnNotifyDept>(e =>
        {
            e.ToTable("ecn_notify_dept");
            e.HasKey(x => x.NotifyId);
            e.Property(x => x.NotifyId).HasColumnName("notify_id").HasMaxLength(50);
            e.Property(x => x.EcnId).HasColumnName("ecn_id").HasMaxLength(50);
            e.Property(x => x.DeptId).HasColumnName("dept_id").HasMaxLength(50);
            e.Property(x => x.DeptName).HasColumnName("dept_name").HasMaxLength(100);
            e.Property(x => x.Confirmed).HasColumnName("confirmed").HasDefaultValue(false);
            e.Property(x => x.NotifiedAt).HasColumnName("notified_at");
            e.Property(x => x.ConfirmedBy).HasColumnName("confirmed_by").HasMaxLength(50);
            e.Property(x => x.ConfirmedAt).HasColumnName("confirmed_at");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.EcnId);
            e.HasIndex(x => x.DeptId);
        });

        modelBuilder.Entity<EcnImplement>(e =>
        {
            e.ToTable("ecn_implement");
            e.HasKey(x => x.ImplementId);
            e.Property(x => x.ImplementId).HasColumnName("implement_id").HasMaxLength(50);
            e.Property(x => x.EcnId).HasColumnName("ecn_id").HasMaxLength(50);
            e.Property(x => x.TaskName).HasColumnName("task_name").HasMaxLength(200);
            e.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            e.Property(x => x.Responsible).HasColumnName("responsible").HasMaxLength(50);
            e.Property(x => x.PlanDate).HasColumnName("plan_date");
            e.Property(x => x.ActualDate).HasColumnName("actual_date");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Result).HasColumnName("result").HasMaxLength(500);
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.UpdatedBy).HasColumnName("updated_by").HasMaxLength(50);
            e.HasIndex(x => x.EcnId);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.Responsible);
        });

        modelBuilder.Entity<NpiProject>(e =>
        {
            e.ToTable("npi_project");
            e.HasKey(x => x.ProjectId);
            e.Property(x => x.ProjectId).HasColumnName("project_id").HasMaxLength(50);
            e.Property(x => x.ProjectName).HasColumnName("project_name").HasMaxLength(100);
            e.Property(x => x.CustomerId).HasColumnName("customer_id").HasMaxLength(50);
            e.Property(x => x.ProductId).HasColumnName("product_id").HasMaxLength(50);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20);
            e.Property(x => x.Phase).HasColumnName("phase").HasMaxLength(20);
            e.Property(x => x.StartDate).HasColumnName("start_date");
            e.Property(x => x.TargetCompletion).HasColumnName("target_completion");
            e.Property(x => x.ActualCompletion).HasColumnName("actual_completion");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.CustomerId);
            e.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<SpcMeasurement>(e =>
        {
            e.ToTable("spc_measurement");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.ParameterName).HasColumnName("parameter_name").HasMaxLength(50);
            e.Property(x => x.MeasuredValue).HasColumnName("measured_value").HasPrecision(10, 4);
            e.Property(x => x.Usl).HasColumnName("usl").HasPrecision(10, 4);
            e.Property(x => x.Lsl).HasColumnName("lsl").HasPrecision(10, 4);
            e.Property(x => x.TargetValue).HasColumnName("target_value").HasPrecision(10, 4);
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.OperatorId).HasColumnName("operator_id").HasMaxLength(50);
            e.Property(x => x.MeasuredAt).HasColumnName("measured_at");
            e.Property(x => x.IsOutOfControl).HasColumnName("is_out_of_control");
            e.HasIndex(x => new { x.StepCode, x.ParameterName }).HasDatabaseName("idx_step_param");
            e.HasIndex(x => x.MeasuredAt);
            e.HasIndex(x => x.LotId);
        });

        modelBuilder.Entity<ShiftSchedule>(e =>
        {
            e.ToTable("shift_schedule");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.EmployeeId).HasColumnName("employee_id").HasMaxLength(50);
            e.Property(x => x.ShiftDate).HasColumnName("shift_date");
            e.Property(x => x.ShiftType).HasColumnName("shift_type").HasMaxLength(20);
            e.Property(x => x.DepartmentId).HasColumnName("department_id").HasMaxLength(50);
            e.Property(x => x.Workshop).HasColumnName("workshop").HasMaxLength(50);
            e.HasIndex(x => new { x.EmployeeId, x.ShiftDate }).HasDatabaseName("idx_employee_date");
            e.HasIndex(x => x.ShiftDate);
        });

        // Equipment Management (Phase 3.5)
        modelBuilder.Entity<EquipmentMaintenance>(e =>
        {
            e.ToTable("equipment_maintenance");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.MaintenanceType).HasColumnName("maintenance_type").HasMaxLength(30);
            e.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.TechnicianId).HasColumnName("technician_id").HasMaxLength(50);
            e.Property(x => x.TechnicianName).HasColumnName("technician_name").HasMaxLength(50);
            e.Property(x => x.ScheduledDate).HasColumnName("scheduled_date");
            e.Property(x => x.StartedAt).HasColumnName("started_at");
            e.Property(x => x.CompletedDate).HasColumnName("completed_date");
            e.Property(x => x.EstimatedHours).HasColumnName("estimated_hours");
            e.Property(x => x.ActualHours).HasColumnName("actual_hours").HasPrecision(5, 2);
            e.Property(x => x.PartsReplaced).HasColumnName("parts_replaced").HasMaxLength(500);
            e.Property(x => x.Notes).HasColumnName("notes").HasColumnType("text");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.EquipmentId).HasDatabaseName("idx_maint_equipment");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_maint_status");
            e.HasIndex(x => x.MaintenanceType).HasDatabaseName("idx_maint_type");
            e.HasIndex(x => x.ScheduledDate).HasDatabaseName("idx_maint_scheduled");
        });

        modelBuilder.Entity<EquipmentFailure>(e =>
        {
            e.ToTable("equipment_failure");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.EquipmentId).HasColumnName("equipment_id").HasMaxLength(50);
            e.Property(x => x.FailureType).HasColumnName("failure_type").HasMaxLength(50);
            e.Property(x => x.Description).HasColumnName("description").HasColumnType("text");
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.ReportedBy).HasColumnName("reported_by").HasMaxLength(50);
            e.Property(x => x.ReportedByName).HasColumnName("reported_by_name").HasMaxLength(50);
            e.Property(x => x.ReportedAt).HasColumnName("reported_at");
            e.Property(x => x.ResolvedAt).HasColumnName("resolved_at");
            e.Property(x => x.ResolvedBy).HasColumnName("resolved_by").HasMaxLength(50);
            e.Property(x => x.DowntimeMinutes).HasColumnName("downtime_minutes");
            e.Property(x => x.RootCause).HasColumnName("root_cause").HasColumnType("text");
            e.Property(x => x.Resolution).HasColumnName("resolution").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.EquipmentId).HasDatabaseName("idx_fail_equipment");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_fail_status");
            e.HasIndex(x => x.Severity).HasDatabaseName("idx_fail_severity");
            e.HasIndex(x => x.ReportedAt).HasDatabaseName("idx_fail_reported");
        });

        // Quality Inspection (Phase 3.4)
        modelBuilder.Entity<QualityInspection>(e =>
        {
            e.ToTable("quality_inspection");
            e.HasKey(x => x.InspectionId);
            e.Property(x => x.InspectionId).HasColumnName("inspection_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.InspectionType).HasColumnName("inspection_type").HasMaxLength(50);
            e.Property(x => x.Result).HasColumnName("result").HasMaxLength(50);
            e.Property(x => x.InspectorId).HasColumnName("inspector_id").HasMaxLength(50);
            e.Property(x => x.InspectorName).HasColumnName("inspector_name").HasMaxLength(50);
            e.Property(x => x.InspectionTime).HasColumnName("inspection_time");
            e.Property(x => x.Detail).HasColumnName("detail");
            e.Property(x => x.Remark).HasColumnName("remark");
            e.Property(x => x.CreatedBy).HasColumnName("created_by").HasMaxLength(50);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_qi_lot");
            e.HasIndex(x => x.InspectionTime).HasDatabaseName("idx_qi_time");
        });

        modelBuilder.Entity<QualityInspectionItem>(e =>
        {
            e.ToTable("quality_inspection_item");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.InspectionId).HasColumnName("inspection_id").HasMaxLength(50);
            e.Property(x => x.ItemCode).HasColumnName("item_code").HasMaxLength(50);
            e.Property(x => x.ItemName).HasColumnName("item_name").HasMaxLength(100);
            e.Property(x => x.Specification).HasColumnName("specification").HasMaxLength(100);
            e.Property(x => x.Usl).HasColumnName("usl");
            e.Property(x => x.Lsl).HasColumnName("lsl");
            e.Property(x => x.TargetValue).HasColumnName("target_value");
            e.Property(x => x.MeasuredValue).HasColumnName("measured_value");
            e.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(20);
            e.Property(x => x.Result).HasColumnName("result").HasMaxLength(20);
            e.Property(x => x.Remark).HasColumnName("remark").HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.HasIndex(x => x.InspectionId).HasDatabaseName("idx_qii_inspection");
            e.HasIndex(x => x.ItemCode).HasDatabaseName("idx_qii_item");
        });

        // Non-Conformance Report (Phase 3.4)
        modelBuilder.Entity<NonConformanceReport>(e =>
        {
            e.ToTable("non_conformance_report");
            e.HasKey(x => x.NcrId);
            e.Property(x => x.NcrId).HasColumnName("ncr_id").HasMaxLength(50);
            e.Property(x => x.LotId).HasColumnName("lot_id").HasMaxLength(50);
            e.Property(x => x.StepCode).HasColumnName("step_code").HasMaxLength(50);
            e.Property(x => x.DefectType).HasColumnName("defect_type").HasMaxLength(50);
            e.Property(x => x.DefectDescription).HasColumnName("defect_description").HasColumnType("text");
            e.Property(x => x.Quantity).HasColumnName("quantity");
            e.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20);
            e.Property(x => x.Status).HasColumnName("status").HasMaxLength(30);
            e.Property(x => x.Disposition).HasColumnName("disposition").HasMaxLength(50);
            e.Property(x => x.DispositionDetail).HasColumnName("disposition_detail").HasColumnType("text");
            e.Property(x => x.DiscoveredBy).HasColumnName("discovered_by").HasMaxLength(50);
            e.Property(x => x.DiscovererId).HasColumnName("discoverer_id").HasMaxLength(50);
            e.Property(x => x.DiscovererName).HasColumnName("discoverer_name").HasMaxLength(50);
            e.Property(x => x.DiscoveredTime).HasColumnName("discovered_time");
            e.Property(x => x.DiscoveredAt).HasColumnName("discovered_at");
            e.Property(x => x.ReviewerId).HasColumnName("reviewer_id").HasMaxLength(50);
            e.Property(x => x.ReviewerName).HasColumnName("reviewer_name").HasMaxLength(50);
            e.Property(x => x.ReviewedAt).HasColumnName("reviewed_at");
            e.Property(x => x.ResolvedBy).HasColumnName("resolved_by").HasMaxLength(50);
            e.Property(x => x.ResolvedAt).HasColumnName("resolved_at");
            e.Property(x => x.CloserId).HasColumnName("closer_id").HasMaxLength(50);
            e.Property(x => x.ClosedAt).HasColumnName("closed_at");
            e.Property(x => x.ClosureComment).HasColumnName("closure_comment").HasColumnType("text");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.LotId).HasDatabaseName("idx_ncr_lot");
            e.HasIndex(x => x.Status).HasDatabaseName("idx_ncr_status");
            e.HasIndex(x => x.Severity).HasDatabaseName("idx_ncr_severity");
            e.HasIndex(x => x.DiscoveredAt).HasDatabaseName("idx_ncr_discovered");
        });

        // Phase 1 entities (Quality, Warehouse, Abnormal/Equipment/FirstArticle/Alert)
        Phase1EntityConfigurations.Apply(modelBuilder);

        // Phase 2 entities (Order, Planning, BOM/MRP, Progress, Rush Order)
        Phase2EntityConfigurations.Apply(modelBuilder);

        // Phase 3 entities (Process Control, Bin, Tooling, Wire, Qualification, Bond Pull)
        Phase3EntityConfigurations.Apply(modelBuilder);

        // Phase 5 entities (Analytics, NPI, Audit)
        Phase5EntityConfigurations.Apply(modelBuilder);
    }
}
