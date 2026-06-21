using BCrypt.Net;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Shell.Services;

/// <summary>
/// 认证系统种子数据初始化。
/// 按半导体封测工厂组织架构创建部门、角色、权限、演示用户。
/// </summary>
public class AuthDataSeeder
{
    private readonly MesDbContext _context;

    public AuthDataSeeder(MesDbContext context) => _context = context;

    public async Task EnsureSeededAsync()
    {
        await EnsureTablesCreatedAsync();

        if (!await _context.SysUsers.AnyAsync())
        {
            await SeedDepartmentsAsync();
            await SeedRolesAsync();
            await SeedPermissionsAsync();
            await SeedUsersAsync();
            await _context.SaveChangesAsync();
            Console.WriteLine("[AuthSeeder] 认证种子数据初始化完成");
        }
        else
        {
            // 用户已存在，确保角色和权限也已种子（幂等）
            await EnsureRolesSeededAsync();
            await EnsurePermissionsSeededAsync();
            await _context.SaveChangesAsync();
            Console.WriteLine("[AuthSeeder] 角色和权限确保完成");
        }

        await SeedMasterDataAsync();
        await _context.SaveChangesAsync();
    }

    private async Task EnsureRolesSeededAsync()
    {
        var existingRoleIds = await _context.SysRoles.Select(r => r.RoleId).ToListAsync();
        var roles = new SysRole[]
        {
            new() { RoleId = "ROLE-ADMIN",      RoleName = "系统管理员",     Level = 1, Description = "系统管理、用户管理、全部权限" },
            new() { RoleId = "ROLE-MGR",        RoleName = "生产主管",       Level = 2, Description = "生产全面管理、批次处理、排程" },
            new() { RoleId = "ROLE-SHIFT-LEAD", RoleName = "领班/组长",      Level = 3, Description = "当班生产管理、进出站操作、Hold/Release" },
            new() { RoleId = "ROLE-OPERATOR",   RoleName = "操作员",         Level = 4, Description = "进出站操作、批次查询" },
            new() { RoleId = "ROLE-ENGINEER",   RoleName = "工艺工程师",     Level = 3, Description = "Recipe管理、SPC/FDC、良率分析" },
            new() { RoleId = "ROLE-QA",         RoleName = "品质工程师",     Level = 3, Description = "检验、SPC监控、批次Hold" },
            new() { RoleId = "ROLE-PLANNER",    RoleName = "计划员",         Level = 3, Description = "工单创建、排程调度" },
            new() { RoleId = "ROLE-MAINT",      RoleName = "设备维护",       Level = 3, Description = "设备监控、PM、告警处理" },
        };

        foreach (var role in roles)
        {
            if (!existingRoleIds.Contains(role.RoleId))
            {
                await _context.SysRoles.AddAsync(role);
                Console.WriteLine($"[AuthSeeder] 补充角色: {role.RoleId}");
            }
        }
    }

    private async Task EnsurePermissionsSeededAsync()
    {
        var existingPermCodes = await _context.SysUserPermissions
            .Where(p => p.UserId.StartsWith("ROLE-"))
            .Select(p => $"{p.UserId}|{p.PermissionCode}")
            .ToListAsync();

        var perms = new (string roleId, string code)[]
        {
            ("ROLE-ADMIN", "*:*"),
            ("ROLE-MGR", "Production:*"),
            ("ROLE-MGR", "Schedule:*"),
            ("ROLE-MGR", "Yield:*"),
            ("ROLE-MGR", "Trace:*"),
            ("ROLE-MGR", "Order:*"),
            ("ROLE-MGR", "MasterData:*"),
            ("ROLE-SHIFT-LEAD", "Production:WorkOrderListView"),
            ("ROLE-SHIFT-LEAD", "Production:LotListView"),
            ("ROLE-SHIFT-LEAD", "Production:TrackInView"),
            ("ROLE-SHIFT-LEAD", "Production:WipOverviewView"),
            ("ROLE-SHIFT-LEAD", "Production:LotHoldView"),
            ("ROLE-SHIFT-LEAD", "Production:LotSplitMergeView"),
            ("ROLE-SHIFT-LEAD", "Production:CustomerProgressView"),
            ("ROLE-SHIFT-LEAD", "Production:ProductionReportView"),
            ("ROLE-SHIFT-LEAD", "Production:GradeSortView"),
            ("ROLE-SHIFT-LEAD", "Production:DispatchView"),
            ("ROLE-OPERATOR", "Production:TrackInView"),
            ("ROLE-OPERATOR", "Production:LotListView"),
            ("ROLE-OPERATOR", "Production:WipOverviewView"),
            ("ROLE-ENGINEER", "Recipe:*"),
            ("ROLE-ENGINEER", "Quality:*"),
            ("ROLE-ENGINEER", "Yield:*"),
            ("ROLE-ENGINEER", "Production:WipOverviewView"),
            ("ROLE-ENGINEER", "Production:MasterDataView"),
            ("ROLE-ENGINEER", "Production:ProductionReportView"),
            ("ROLE-ENGINEER", "Production:YieldReportView"),
            ("ROLE-ENGINEER", "Production:ProductManagementView"),
            ("ROLE-ENGINEER", "Production:RouteManagementView"),
            ("ROLE-ENGINEER", "Production:RecipeManagementView"),
            ("ROLE-ENGINEER", "Production:EquipmentManagementView"),
            ("ROLE-ENGINEER", "Production:CustomerManagementView"),
            ("ROLE-ENGINEER", "Production:ReasonCodeManagementView"),
            ("ROLE-ENGINEER", "Production:DefectCodeManagementView"),
            ("ROLE-ENGINEER", "Production:CarrierManagementView"),
            ("ROLE-ENGINEER", "Production:MaterialManagementView"),
            ("ROLE-ENGINEER", "Production:YieldRuleManagementView"),
            ("ROLE-ENGINEER", "Production:ScrapRuleManagementView"),
            ("ROLE-ENGINEER", "Production:AlarmRuleManagementView"),
            ("ROLE-ENGINEER", "ReportCenter:ProductionReportView"),
            ("ROLE-ENGINEER", "ReportCenter:YieldReportView"),
            ("ROLE-ENGINEER", "ReportCenter:QualityReportView"),
            ("ROLE-ENGINEER", "ReportCenter:EquipmentReportView"),
            ("ROLE-ENGINEER", "ReportCenter:LotGenealogyReportView"),
            ("ROLE-ENGINEER", "ReportCenter:DashboardView"),
            ("ROLE-QA", "Quality:*"),
            ("ROLE-QA", "Production:LotHoldView"),
            ("ROLE-QA", "Production:LotListView"),
            ("ROLE-QA", "Production:WipOverviewView"),
            ("ROLE-QA", "ReportCenter:QualityReportView"),
            ("ROLE-QA", "ReportCenter:ProductionReportView"),
            ("ROLE-QA", "ReportCenter:LotGenealogyReportView"),
            ("ROLE-PLANNER", "Production:WorkOrderListView"),
            ("ROLE-PLANNER", "Production:LotListView"),
            ("ROLE-PLANNER", "Production:WipOverviewView"),
            ("ROLE-PLANNER", "Production:CustomerProgressView"),
            ("ROLE-PLANNER", "ReportCenter:ProductionReportView"),
            ("ROLE-PLANNER", "Schedule:*"),
            ("ROLE-PLANNER", "Order:*"),
            ("ROLE-MAINT", "Equipment:*"),
            ("ROLE-MAINT", "Alarm:*"),
            ("ROLE-MAINT", "ReportCenter:EquipmentReportView"),
            ("ROLE-MAINT", "ReportCenter:ProductionReportView"),
            ("ROLE-MGR", "SystemSettings:*"),
            ("ROLE-ENGINEER", "SystemSettings:UserPermissionView"),
            ("ROLE-ENGINEER", "SystemSettings:OperationLogView"),
            ("ROLE-ENGINEER", "SystemSettings:SystemParamView"),
        };

        int added = 0;
        foreach (var (roleId, code) in perms)
        {
            var key = $"{roleId}|{code}";
            if (!existingPermCodes.Contains(key))
            {
                await _context.SysUserPermissions.AddAsync(new SysUserPermission
                {
                    UserId = roleId,
                    PermissionCode = code,
                });
                added++;
            }
        }

        if (added > 0)
            Console.WriteLine($"[AuthSeeder] 补充了 {added} 条权限记录");
    }

    private async Task EnsureTablesCreatedAsync()
    {
        var sqlStatements = new[]
        {
            // master_product - create
            @"CREATE TABLE IF NOT EXISTS master_product (
                product_id VARCHAR(50) NOT NULL,
                product_name VARCHAR(100) NOT NULL,
                package_type VARCHAR(50) NOT NULL,
                process_stage VARCHAR(20) DEFAULT 'Assemble',
                default_route_id VARCHAR(100),
                unit_qty INT DEFAULT 1,
                customer_id VARCHAR(50),
                customer_name VARCHAR(100),
                customer_pn VARCHAR(100),
                internal_pn VARCHAR(100),
                status VARCHAR(20) DEFAULT 'Active',
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (product_id)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_route - create
            @"CREATE TABLE IF NOT EXISTS master_route (
                route_id VARCHAR(100) NOT NULL,
                route_name VARCHAR(100) NOT NULL,
                route_version VARCHAR(20) NOT NULL,
                product_id VARCHAR(50) NOT NULL,
                package_type VARCHAR(50),
                is_active TINYINT(1) DEFAULT 1,
                is_approved TINYINT(1) DEFAULT 0,
                approved_by VARCHAR(50),
                approved_at DATETIME,
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (route_id)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_route_step - create
            @"CREATE TABLE IF NOT EXISTS master_route_step (
                route_id VARCHAR(100) NOT NULL,
                step_seq INT NOT NULL,
                step_code VARCHAR(50) NOT NULL,
                step_name VARCHAR(100),
                equipment_group VARCHAR(50),
                is_rework TINYINT(1) DEFAULT 0,
                PRIMARY KEY (route_id, step_seq)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_equipment - create
            @"CREATE TABLE IF NOT EXISTS master_equipment (
                equipment_id VARCHAR(50) NOT NULL,
                equipment_name VARCHAR(100) NOT NULL,
                equipment_group VARCHAR(50) NOT NULL,
                equipment_type VARCHAR(50) NOT NULL,
                process_stage VARCHAR(20) DEFAULT 'Assemble',
                vendor VARCHAR(100),
                model VARCHAR(100),
                serial_number VARCHAR(100),
                capability JSON,
                status VARCHAR(20) DEFAULT 'Available',
                current_lot_id VARCHAR(50),
                current_recipe VARCHAR(100),
                location VARCHAR(100),
                responsible_person VARCHAR(50),
                last_maintenance_date DATETIME,
                maintenance_interval_hours INT DEFAULT 500,
                running_hours INT DEFAULT 0,
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (equipment_id)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_carrier - create
            @"CREATE TABLE IF NOT EXISTS master_carrier (
                carrier_id VARCHAR(50) NOT NULL,
                carrier_type VARCHAR(50) NOT NULL,
                status VARCHAR(20) DEFAULT 'Available',
                current_lot_id VARCHAR(50),
                capacity INT DEFAULT 0,
                use_count INT DEFAULT 0,
                max_use_count INT DEFAULT 0,
                last_clean_date DATETIME,
                clean_interval_uses INT DEFAULT 10,
                location VARCHAR(100),
                applicable_process JSON,
                applicable_package JSON,
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (carrier_id)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_recipe - create
            @"CREATE TABLE IF NOT EXISTS master_recipe (
                recipe_id VARCHAR(100) NOT NULL,
                recipe_name VARCHAR(100) NOT NULL,
                equipment_group VARCHAR(50) NOT NULL,
                step_code VARCHAR(50) NOT NULL,
                version VARCHAR(20) NOT NULL,
                is_active TINYINT(1) DEFAULT 1,
                parameters JSON,
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (recipe_id)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_yield_rule - create
            @"CREATE TABLE IF NOT EXISTS master_yield_rule (
                rule_id VARCHAR(50) NOT NULL,
                route_id VARCHAR(100),
                step_code VARCHAR(50),
                yield_threshold DECIMAL(5,2) NOT NULL,
                action_type VARCHAR(20) NOT NULL DEFAULT 'Warning',
                notify_role VARCHAR(50),
                is_active TINYINT(1) DEFAULT 1,
                PRIMARY KEY (rule_id)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_alarm_rule - create
            @"CREATE TABLE IF NOT EXISTS master_alarm_rule (
                rule_id VARCHAR(50) NOT NULL,
                alarm_type VARCHAR(50) NOT NULL,
                severity VARCHAR(20) NOT NULL DEFAULT 'Warning',
                threshold_yield DECIMAL(5,2),
                threshold_qty INT,
                threshold_minutes INT,
                is_enabled TINYINT(1) DEFAULT 1,
                notify_role VARCHAR(50),
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (rule_id)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_scrap_rule - create
            @"CREATE TABLE IF NOT EXISTS master_scrap_rule (
                rule_id VARCHAR(50) NOT NULL,
                route_id VARCHAR(100),
                step_code VARCHAR(50),
                threshold_percent DECIMAL(5,2) NOT NULL,
                requires_approval TINYINT(1) DEFAULT 1,
                approval_level VARCHAR(50),
                is_active TINYINT(1) DEFAULT 1,
                PRIMARY KEY (rule_id)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_material - create
            @"CREATE TABLE IF NOT EXISTS master_material (
                material_id VARCHAR(50) NOT NULL,
                material_name VARCHAR(100) NOT NULL,
                material_type VARCHAR(50) NOT NULL,
                specification VARCHAR(255),
                unit VARCHAR(20) NOT NULL DEFAULT 'pcs',
                supplier VARCHAR(100),
                min_stock INT DEFAULT 0,
                current_stock INT DEFAULT 0,
                is_active TINYINT(1) DEFAULT 1,
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (material_id)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_customer - create
            @"CREATE TABLE IF NOT EXISTS master_customer (
                customer_id VARCHAR(50) NOT NULL,
                customer_name VARCHAR(100) NOT NULL,
                customer_code VARCHAR(50) NOT NULL,
                contact_person VARCHAR(100),
                contact_phone VARCHAR(50),
                email VARCHAR(100),
                address VARCHAR(255),
                customer_pn_prefix VARCHAR(20),
                quality_level VARCHAR(20) NOT NULL DEFAULT 'Industrial',
                special_requirements JSON,
                default_packing_spec VARCHAR(100),
                default_oqc_spec VARCHAR(100),
                status VARCHAR(20) DEFAULT 'Active',
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (customer_id),
                UNIQUE KEY uk_customer_code (customer_code)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_reason_code - create
            @"CREATE TABLE IF NOT EXISTS master_reason_code (
                reason_code_id VARCHAR(50) NOT NULL,
                category VARCHAR(50) NOT NULL,
                sub_category VARCHAR(50),
                reason_text VARCHAR(255) NOT NULL,
                applicable_to VARCHAR(50) NOT NULL,
                is_enabled TINYINT(1) DEFAULT 1,
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (reason_code_id),
                INDEX idx_category (category)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // master_defect_code - create
            @"CREATE TABLE IF NOT EXISTS master_defect_code (
                defect_code_id VARCHAR(50) NOT NULL,
                defect_category VARCHAR(50) NOT NULL,
                defect_text VARCHAR(255) NOT NULL,
                severity VARCHAR(20) DEFAULT 'Major',
                is_enabled TINYINT(1) DEFAULT 1,
                created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
                PRIMARY KEY (defect_code_id),
                INDEX idx_category (defect_category)
            ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;",

            // ALTER TABLE - add missing columns to existing tables
            @"ALTER TABLE master_product ADD COLUMN process_stage VARCHAR(20) DEFAULT 'Assemble' AFTER package_type;",
            @"ALTER TABLE master_product ADD COLUMN default_route_id VARCHAR(100) AFTER process_stage;",
            @"ALTER TABLE master_product ADD COLUMN unit_qty INT DEFAULT 1 AFTER default_route_id;",

            @"ALTER TABLE master_equipment ADD COLUMN process_stage VARCHAR(20) DEFAULT 'Assemble' AFTER equipment_type;",
            @"ALTER TABLE master_equipment ADD COLUMN vendor VARCHAR(100) AFTER process_stage;",
            @"ALTER TABLE master_equipment ADD COLUMN model VARCHAR(100) AFTER vendor;",
            @"ALTER TABLE master_equipment ADD COLUMN serial_number VARCHAR(100) AFTER model;",
            @"ALTER TABLE master_equipment ADD COLUMN capability JSON AFTER serial_number;",

            @"ALTER TABLE master_carrier ADD COLUMN applicable_process JSON AFTER location;",
            @"ALTER TABLE master_carrier ADD COLUMN applicable_package JSON AFTER applicable_process;",

            // Add missing created_at columns
            @"ALTER TABLE master_route_step ADD COLUMN created_at DATETIME DEFAULT CURRENT_TIMESTAMP AFTER is_rework;",
            @"ALTER TABLE master_yield_rule ADD COLUMN created_at DATETIME DEFAULT CURRENT_TIMESTAMP AFTER is_active;",
            @"ALTER TABLE master_scrap_rule ADD COLUMN created_at DATETIME DEFAULT CURRENT_TIMESTAMP AFTER is_active;",

            // Add missing columns to master_recipe
            @"ALTER TABLE master_recipe ADD COLUMN product_id VARCHAR(50) AFTER equipment_group;",
            @"ALTER TABLE master_recipe ADD COLUMN approved_by VARCHAR(50) AFTER parameters;",
            @"ALTER TABLE master_recipe ADD COLUMN approved_at DATETIME AFTER approved_by;",

            // Add missing id column to master_route_step
            @"ALTER TABLE master_route_step ADD COLUMN id BIGINT AUTO_INCREMENT PRIMARY KEY FIRST;",
        };

        foreach (var sql in sqlStatements)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(sql);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                var isExpected = msg.Contains("already exists")
                    || msg.Contains("Duplicate column")
                    || msg.Contains("Duplicate key")
                    || msg.Contains("Duplicate entry")
                    || msg.Contains("Table") && msg.Contains("already exists")
                    || (ex.InnerException != null && (ex.InnerException.Message.Contains("Duplicate column") || ex.InnerException.Message.Contains("already exists")));

                if (!isExpected)
                {
                    Console.WriteLine($"[AuthSeeder] Table creation warning: {msg}");
                }
            }
        }

        Console.WriteLine("[AuthSeeder] 数据库表结构初始化完成");
    }

    private async Task SeedDepartmentsAsync()
    {
        var depts = new SysDepartment[]
        {
            new() { DeptId = "DEPT-PROD",  DeptName = "生产部",     ParentId = null, ManagerId = null, Status = "Active" },
            new() { DeptId = "DEPT-ENG",   DeptName = "工程部",     ParentId = null, ManagerId = null, Status = "Active" },
            new() { DeptId = "DEPT-QA",    DeptName = "品质部",     ParentId = null, ManagerId = null, Status = "Active" },
            new() { DeptId = "DEPT-LOG",   DeptName = "生管/物控",  ParentId = null, ManagerId = null, Status = "Active" },
            new() { DeptId = "DEPT-EQ",    DeptName = "设备部",     ParentId = null, ManagerId = null, Status = "Active" },
            new() { DeptId = "DEPT-FAC",   DeptName = "厂务/EHS",   ParentId = null, ManagerId = null, Status = "Active" },
            new() { DeptId = "DEPT-IT",    DeptName = "资讯部",     ParentId = null, ManagerId = null, Status = "Active" },
            new() { DeptId = "DEPT-ADMIN", DeptName = "管理层",     ParentId = null, ManagerId = null, Status = "Active" },
            new() { DeptId = "DEPT-WH",    DeptName = "仓储部",     ParentId = null, ManagerId = null, Status = "Active" },
        };

        await _context.SysDepartments.AddRangeAsync(depts);
    }

    private async Task SeedRolesAsync()
    {
        var roles = new SysRole[]
        {
            new() { RoleId = "ROLE-ADMIN",      RoleName = "系统管理员",     Level = 1, Description = "系统管理、用户管理、全部权限" },
            new() { RoleId = "ROLE-MGR",        RoleName = "生产主管",       Level = 2, Description = "生产全面管理、批次处理、排程" },
            new() { RoleId = "ROLE-SHIFT-LEAD", RoleName = "领班/组长",      Level = 3, Description = "当班生产管理、进出站操作、Hold/Release" },
            new() { RoleId = "ROLE-OPERATOR",   RoleName = "操作员",         Level = 4, Description = "进出站操作、批次查询" },
            new() { RoleId = "ROLE-ENGINEER",   RoleName = "工艺工程师",     Level = 3, Description = "Recipe管理、SPC/FDC、良率分析" },
            new() { RoleId = "ROLE-QA",         RoleName = "品质工程师",     Level = 3, Description = "检验、SPC监控、批次Hold" },
            new() { RoleId = "ROLE-PLANNER",    RoleName = "计划员",         Level = 3, Description = "工单创建、排程调度" },
            new() { RoleId = "ROLE-MAINT",      RoleName = "设备维护",       Level = 3, Description = "设备监控、PM、告警处理" },
        };

        await _context.SysRoles.AddRangeAsync(roles);
    }

    private async Task SeedPermissionsAsync()
    {
        var perms = new (string roleId, string code)[]
        {
            // Admin: 全部权限
            ("ROLE-ADMIN", "*:*"),

            // 生产主管: 生产+排程+良率+追溯+Order模块
            ("ROLE-MGR", "Production:*"),
            ("ROLE-MGR", "Schedule:*"),
            ("ROLE-MGR", "Yield:*"),
            ("ROLE-MGR", "Trace:*"),
            ("ROLE-MGR", "Order:*"),
            ("ROLE-MGR", "MasterData:*"),

            // 领班/组长: 核心生产操作
            ("ROLE-SHIFT-LEAD", "Production:WorkOrderListView"),
            ("ROLE-SHIFT-LEAD", "Production:LotListView"),
            ("ROLE-SHIFT-LEAD", "Production:TrackInView"),
            ("ROLE-SHIFT-LEAD", "Production:WipOverviewView"),
            ("ROLE-SHIFT-LEAD", "Production:LotHoldView"),
            ("ROLE-SHIFT-LEAD", "Production:LotSplitMergeView"),
            ("ROLE-SHIFT-LEAD", "Production:CustomerProgressView"),
            ("ROLE-SHIFT-LEAD", "Production:ProductionReportView"),
            ("ROLE-SHIFT-LEAD", "Production:GradeSortView"),
            ("ROLE-SHIFT-LEAD", "Production:DispatchView"),

            // 操作员: 仅进出站和查看
            ("ROLE-OPERATOR", "Production:TrackInView"),
            ("ROLE-OPERATOR", "Production:LotListView"),
            ("ROLE-OPERATOR", "Production:WipOverviewView"),

            // 工艺工程师: Recipe+质量+良率+主数据
            ("ROLE-ENGINEER", "Recipe:*"),
            ("ROLE-ENGINEER", "Quality:*"),
            ("ROLE-ENGINEER", "Yield:*"),
            ("ROLE-ENGINEER", "Production:WipOverviewView"),
            ("ROLE-ENGINEER", "Production:MasterDataView"),
            ("ROLE-ENGINEER", "Production:ProductionReportView"),
            ("ROLE-ENGINEER", "Production:YieldReportView"),
            ("ROLE-ENGINEER", "Production:ProductManagementView"),
            ("ROLE-ENGINEER", "Production:RouteManagementView"),
            ("ROLE-ENGINEER", "Production:RecipeManagementView"),
            ("ROLE-ENGINEER", "Production:EquipmentManagementView"),
            ("ROLE-ENGINEER", "Production:CustomerManagementView"),
            ("ROLE-ENGINEER", "Production:ReasonCodeManagementView"),
            ("ROLE-ENGINEER", "Production:DefectCodeManagementView"),
            ("ROLE-ENGINEER", "Production:CarrierManagementView"),
            ("ROLE-ENGINEER", "Production:MaterialManagementView"),
            ("ROLE-ENGINEER", "Production:YieldRuleManagementView"),
            ("ROLE-ENGINEER", "Production:ScrapRuleManagementView"),
            ("ROLE-ENGINEER", "Production:AlarmRuleManagementView"),

            // 品质工程师: 质量+Hold
            ("ROLE-QA", "Quality:*"),
            ("ROLE-QA", "Production:LotHoldView"),
            ("ROLE-QA", "Production:LotListView"),
            ("ROLE-QA", "Production:WipOverviewView"),

            // 计划员: 工单+批次+排程+Order模块
            ("ROLE-PLANNER", "Production:WorkOrderListView"),
            ("ROLE-PLANNER", "Production:LotListView"),
            ("ROLE-PLANNER", "Production:WipOverviewView"),
            ("ROLE-PLANNER", "Production:CustomerProgressView"),
            ("ROLE-PLANNER", "Schedule:*"),
            ("ROLE-PLANNER", "Order:*"),

            // 设备维护: 设备+告警
            ("ROLE-MAINT", "Equipment:*"),
            ("ROLE-MAINT", "Alarm:*"),
        };

        foreach (var (roleId, code) in perms)
        {
            await _context.SysUserPermissions.AddAsync(new SysUserPermission
            {
                UserId = roleId,
                PermissionCode = code,
            });
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new (string empId, string name, string deptId, string roleId, string password)[]
        {
            ("10001", "系统管理员", "DEPT-IT",    "ROLE-ADMIN",      "admin123"),
            ("10002", "王厂长",     "DEPT-ADMIN", "ROLE-MGR",        "123456"),
            ("10003", "张三",       "DEPT-PROD",  "ROLE-OPERATOR",   "123456"),
            ("10004", "李四",       "DEPT-PROD",  "ROLE-OPERATOR",   "123456"),
            ("10005", "赵组长",     "DEPT-PROD",  "ROLE-SHIFT-LEAD", "123456"),
            ("10006", "陈工程师",   "DEPT-ENG",   "ROLE-ENGINEER",   "123456"),
            ("10007", "孙品质",     "DEPT-QA",    "ROLE-QA",         "123456"),
            ("10008", "周计划",     "DEPT-LOG",   "ROLE-PLANNER",    "123456"),
            ("10009", "吴维护",     "DEPT-EQ",    "ROLE-MAINT",      "123456"),
        };

        foreach (var (empId, name, deptId, roleId, password) in users)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            await _context.SysUsers.AddAsync(new SysUser
            {
                UserId = empId,
                UserName = name,
                PasswordHash = hash,
                RoleId = roleId,
                DeptId = deptId,
                IsActive = true,
            });
        }
    }

    private async Task SeedMasterDataAsync()
    {
        // 客户数据
        if (!await _context.MasterCustomers.AnyAsync())
        {
            var customers = new MasterCustomer[]
            {
                new() { CustomerId = "CUST-AUTO", CustomerName = "某汽车电子", CustomerCode = "CUST-AUTO", ContactPerson = "张总", ContactPhone = "13800001111", Email = "auto@example.com", QualityLevel = "Automotive", CustomerPnPrefix = "AE-", DefaultPackingSpec = "PKG-AUTO-STD", DefaultOqcSpec = "OQC-AUTO-STD", Status = "Active" },
                new() { CustomerId = "CUST-IND", CustomerName = "某工业客户", CustomerCode = "CUST-IND", ContactPerson = "李经理", ContactPhone = "13800002222", Email = "industrial@example.com", QualityLevel = "Industrial", CustomerPnPrefix = "IND-", DefaultPackingSpec = "PKG-IND-STD", DefaultOqcSpec = "OQC-IND-STD", Status = "Active" },
                new() { CustomerId = "CUST-CON", CustomerName = "某消费电子", CustomerCode = "CUST-CON", ContactPerson = "王采购", ContactPhone = "13800003333", Email = "consumer@example.com", QualityLevel = "Consumer", CustomerPnPrefix = "CE-", DefaultPackingSpec = "PKG-CON-STD", DefaultOqcSpec = "OQC-CON-STD", Status = "Active" },
            };
            await _context.MasterCustomers.AddRangeAsync(customers);
            Console.WriteLine("[AuthSeeder] 客户数据初始化完成");
        }

        // 产品数据
        if (!await _context.MasterProducts.AnyAsync())
        {
            var products = new MasterProduct[]
            {
                new() { ProductId = "PROD-QFN88", ProductName = "QFN88 标准封装", PackageType = "QFN", ProcessStage = "Assemble", CustomerId = "CUST-AUTO", CustomerName = "某汽车电子", CustomerPn = "AE-QFN88-A", InternalPn = "INT-QFN88", Status = "Active", UnitQty = 1 },
                new() { ProductId = "PROD-SOP8", ProductName = "SOP8 标准封装", PackageType = "SOP", ProcessStage = "Assemble", CustomerId = "CUST-IND", CustomerName = "某工业客户", CustomerPn = "IND-SOP8-B", InternalPn = "INT-SOP8", Status = "Active", UnitQty = 1 },
                new() { ProductId = "PROD-BGA256", ProductName = "BGA256 标准封装", PackageType = "BGA", ProcessStage = "Both", CustomerId = "CUST-CON", CustomerName = "某消费电子", CustomerPn = "CE-BGA256-C", InternalPn = "INT-BGA256", Status = "Active", UnitQty = 1 },
                new() { ProductId = "PROD-QFP64", ProductName = "QFP64 标准封装", PackageType = "QFP", ProcessStage = "Assemble", CustomerId = "CUST-AUTO", CustomerName = "某汽车电子", CustomerPn = "AE-QFP64-A", InternalPn = "INT-QFP64", Status = "Active", UnitQty = 1 },
                new() { ProductId = "PROD-DFN10", ProductName = "DFN10 标准封装", PackageType = "DFN", ProcessStage = "Both", CustomerId = "CUST-CON", CustomerName = "某消费电子", CustomerPn = "CE-DFN10-C", InternalPn = "INT-DFN10", Status = "Active", UnitQty = 1 },
            };
            await _context.MasterProducts.AddRangeAsync(products);
            Console.WriteLine("[AuthSeeder] 产品数据初始化完成");
        }

        // 工艺路线
        if (!await _context.MasterRoutes.AnyAsync())
        {
            var routes = new MasterRoute[]
            {
                new() { RouteId = "QFN-STD:1.0", RouteName = "QFN标准路线", RouteVersion = "1.0", ProductId = "PROD-QFN88", PackageType = "QFN", IsActive = true, IsApproved = true, ApprovedBy = "10006", ApprovedAt = DateTime.UtcNow },
                new() { RouteId = "SOP-STD:1.0", RouteName = "SOP标准路线", RouteVersion = "1.0", ProductId = "PROD-SOP8", PackageType = "SOP", IsActive = true, IsApproved = true, ApprovedBy = "10006", ApprovedAt = DateTime.UtcNow },
                new() { RouteId = "BGA-STD:1.0", RouteName = "BGA标准路线", RouteVersion = "1.0", ProductId = "PROD-BGA256", PackageType = "BGA", IsActive = true, IsApproved = true, ApprovedBy = "10006", ApprovedAt = DateTime.UtcNow },
                new() { RouteId = "QFP-STD:1.0", RouteName = "QFP标准路线", RouteVersion = "1.0", ProductId = "PROD-QFP64", PackageType = "QFP", IsActive = true, IsApproved = true, ApprovedBy = "10006", ApprovedAt = DateTime.UtcNow },
                new() { RouteId = "DFN-STD:1.0", RouteName = "DFN标准路线", RouteVersion = "1.0", ProductId = "PROD-DFN10", PackageType = "DFN", IsActive = true, IsApproved = true, ApprovedBy = "10006", ApprovedAt = DateTime.UtcNow },
            };
            await _context.MasterRoutes.AddRangeAsync(routes);
            Console.WriteLine("[AuthSeeder] 工艺路线初始化完成");
        }

        // 工艺路线步骤
        if (!await _context.MasterRouteSteps.AnyAsync())
        {
            var routeSteps = new MasterRouteStep[]
            {
                new() { RouteId = "QFN-STD:1.0", StepSeq = 1, StepCode = "SAW", StepName = "晶圆切割", EquipmentGroup = "SAW", IsRework = false },
                new() { RouteId = "QFN-STD:1.0", StepSeq = 2, StepCode = "DA", StepName = "晶片贴装", EquipmentGroup = "DA", IsRework = false },
                new() { RouteId = "QFN-STD:1.0", StepSeq = 3, StepCode = "WB", StepName = "引线键合", EquipmentGroup = "WB", IsRework = false },
                new() { RouteId = "QFN-STD:1.0", StepSeq = 4, StepCode = "MOLD", StepName = "塑封成型", EquipmentGroup = "MOLD", IsRework = false },
                new() { RouteId = "QFN-STD:1.0", StepSeq = 5, StepCode = "PMC", StepName = "后固化", EquipmentGroup = "CURE", IsRework = false },
                new() { RouteId = "QFN-STD:1.0", StepSeq = 6, StepCode = "LASER", StepName = "激光打标", EquipmentGroup = "LASER", IsRework = false },
                new() { RouteId = "QFN-STD:1.0", StepSeq = 7, StepCode = "SING", StepName = "成型切筋", EquipmentGroup = "SING", IsRework = false },
                new() { RouteId = "QFN-STD:1.0", StepSeq = 8, StepCode = "FT", StepName = "成品测试", EquipmentGroup = "TEST", IsRework = false },
                new() { RouteId = "QFN-STD:1.0", StepSeq = 9, StepCode = "OQC", StepName = "出货检验", EquipmentGroup = "OQC", IsRework = false },
                new() { RouteId = "QFN-STD:1.0", StepSeq = 10, StepCode = "PACK", StepName = "包装入库", EquipmentGroup = "PACK", IsRework = false },
                new() { RouteId = "SOP-STD:1.0", StepSeq = 1, StepCode = "DA", StepName = "晶片贴装", EquipmentGroup = "DA", IsRework = false },
                new() { RouteId = "SOP-STD:1.0", StepSeq = 2, StepCode = "WB", StepName = "引线键合", EquipmentGroup = "WB", IsRework = false },
                new() { RouteId = "SOP-STD:1.0", StepSeq = 3, StepCode = "MOLD", StepName = "塑封成型", EquipmentGroup = "MOLD", IsRework = false },
                new() { RouteId = "SOP-STD:1.0", StepSeq = 4, StepCode = "PMC", StepName = "后固化", EquipmentGroup = "CURE", IsRework = false },
                new() { RouteId = "SOP-STD:1.0", StepSeq = 5, StepCode = "LASER", StepName = "激光打标", EquipmentGroup = "LASER", IsRework = false },
                new() { RouteId = "SOP-STD:1.0", StepSeq = 6, StepCode = "SING", StepName = "成型切筋", EquipmentGroup = "SING", IsRework = false },
                new() { RouteId = "SOP-STD:1.0", StepSeq = 7, StepCode = "FT", StepName = "成品测试", EquipmentGroup = "TEST", IsRework = false },
                new() { RouteId = "SOP-STD:1.0", StepSeq = 8, StepCode = "OQC", StepName = "出货检验", EquipmentGroup = "OQC", IsRework = false },
                new() { RouteId = "BGA-STD:1.0", StepSeq = 1, StepCode = "DA", StepName = "晶片贴装", EquipmentGroup = "DA", IsRework = false },
                new() { RouteId = "BGA-STD:1.0", StepSeq = 2, StepCode = "WB", StepName = "引线键合", EquipmentGroup = "WB", IsRework = false },
                new() { RouteId = "BGA-STD:1.0", StepSeq = 3, StepCode = "MOLD", StepName = "塑封成型", EquipmentGroup = "MOLD", IsRework = false },
                new() { RouteId = "BGA-STD:1.0", StepSeq = 4, StepCode = "PMC", StepName = "后固化", EquipmentGroup = "CURE", IsRework = false },
                new() { RouteId = "BGA-STD:1.0", StepSeq = 5, StepCode = "LASER", StepName = "激光打标", EquipmentGroup = "LASER", IsRework = false },
                new() { RouteId = "BGA-STD:1.0", StepSeq = 6, StepCode = "FT", StepName = "成品测试", EquipmentGroup = "TEST", IsRework = false },
                new() { RouteId = "BGA-STD:1.0", StepSeq = 7, StepCode = "OQC", StepName = "出货检验", EquipmentGroup = "OQC", IsRework = false },
            };
            await _context.MasterRouteSteps.AddRangeAsync(routeSteps);
            Console.WriteLine("[AuthSeeder] 工艺路线步骤初始化完成");
        }

        // 设备数据
        if (!await _context.MasterEquipments.AnyAsync())
        {
            var equipments = new MasterEquipment[]
            {
                new() { EquipmentId = "EQ-SAW-001", EquipmentName = "晶圆切割机#1", EquipmentGroup = "SAW", EquipmentType = "Disco DAD321", ProcessStage = "Assemble", Vendor = "Disco", Model = "DAD321", SerialNumber = "SAW-2023-001", Status = "Available", Location = "Line-A1", ResponsiblePerson = "10009", MaintenanceIntervalHours = 500, RunningHours = 1200 },
                new() { EquipmentId = "EQ-SAW-002", EquipmentName = "晶圆切割机#2", EquipmentGroup = "SAW", EquipmentType = "Disco DAD321", ProcessStage = "Assemble", Vendor = "Disco", Model = "DAD321", SerialNumber = "SAW-2023-002", Status = "Available", Location = "Line-A2", ResponsiblePerson = "10009", MaintenanceIntervalHours = 500, RunningHours = 800 },
                new() { EquipmentId = "EQ-DA-001", EquipmentName = "晶片贴装机#1", EquipmentGroup = "DA", EquipmentType = "ASM AD830", ProcessStage = "Assemble", Vendor = "ASM Pacific", Model = "AD830", SerialNumber = "DA-2023-001", Status = "Available", Location = "Line-A1", ResponsiblePerson = "10009", MaintenanceIntervalHours = 500, RunningHours = 2400 },
                new() { EquipmentId = "EQ-DA-002", EquipmentName = "晶片贴装机#2", EquipmentGroup = "DA", EquipmentType = "ASM AD830", ProcessStage = "Assemble", Vendor = "ASM Pacific", Model = "AD830", SerialNumber = "DA-2023-002", Status = "Available", Location = "Line-A2", ResponsiblePerson = "10009", MaintenanceIntervalHours = 500, RunningHours = 1800 },
                new() { EquipmentId = "EQ-WB-001", EquipmentName = "引线键合机#1", EquipmentGroup = "WB", EquipmentType = "Kulicke Soffa 4524", ProcessStage = "Assemble", Vendor = "K&S", Model = "4524", SerialNumber = "WB-2023-001", Status = "Available", Location = "Line-A1", ResponsiblePerson = "10009", MaintenanceIntervalHours = 250, RunningHours = 3600 },
                new() { EquipmentId = "EQ-WB-002", EquipmentName = "引线键合机#2", EquipmentGroup = "WB", EquipmentType = "Kulicke Soffa 4524", ProcessStage = "Assemble", Vendor = "K&S", Model = "4524", SerialNumber = "WB-2023-002", Status = "Available", Location = "Line-A2", ResponsiblePerson = "10009", MaintenanceIntervalHours = 250, RunningHours = 2800 },
                new() { EquipmentId = "EQ-MOLD-001", EquipmentName = "塑封机#1", EquipmentGroup = "MOLD", EquipmentType = "Towa YPM120", ProcessStage = "Assemble", Vendor = "Towa", Model = "YPM120", SerialNumber = "MOLD-2023-001", Status = "Available", Location = "Line-A1", ResponsiblePerson = "10009", MaintenanceIntervalHours = 500, RunningHours = 4200 },
                new() { EquipmentId = "EQ-CURE-001", EquipmentName = "固化炉#1", EquipmentGroup = "CURE", EquipmentType = "Standard Oven", ProcessStage = "Assemble", Vendor = "Yamato", Model = "DX402", SerialNumber = "CURE-2023-001", Status = "Available", Location = "Line-A", ResponsiblePerson = "10009", MaintenanceIntervalHours = 1000, RunningHours = 5000 },
                new() { EquipmentId = "EQ-LASER-001", EquipmentName = "激光打标机#1", EquipmentGroup = "LASER", EquipmentType = "Han's Laser", ProcessStage = "Assemble", Vendor = "Han's Laser", Model = "HLM-808", SerialNumber = "LASER-2023-001", Status = "Available", Location = "Line-A1", ResponsiblePerson = "10009", MaintenanceIntervalHours = 500, RunningHours = 1500 },
                new() { EquipmentId = "EQ-SING-001", EquipmentName = "成型切筋机#1", EquipmentGroup = "SING", EquipmentType = "ASM AF5", ProcessStage = "Assemble", Vendor = "ASM Pacific", Model = "AF5", SerialNumber = "SING-2023-001", Status = "Available", Location = "Line-A1", ResponsiblePerson = "10009", MaintenanceIntervalHours = 500, RunningHours = 2200 },
                new() { EquipmentId = "EQ-TEST-001", EquipmentName = "成品测试机#1", EquipmentGroup = "TEST", EquipmentType = "Advantest T5581", ProcessStage = "Test", Vendor = "Advantest", Model = "T5581", SerialNumber = "TEST-2023-001", Status = "Available", Location = "Test-A1", ResponsiblePerson = "10009", MaintenanceIntervalHours = 1000, RunningHours = 6000 },
                new() { EquipmentId = "EQ-TEST-002", EquipmentName = "成品测试机#2", EquipmentGroup = "TEST", EquipmentType = "Advantest T5581", ProcessStage = "Test", Vendor = "Advantest", Model = "T5581", SerialNumber = "TEST-2023-002", Status = "Available", Location = "Test-A2", ResponsiblePerson = "10009", MaintenanceIntervalHours = 1000, RunningHours = 3800 },
                new() { EquipmentId = "EQ-OQC-001", EquipmentName = "出货检验台#1", EquipmentGroup = "OQC", EquipmentType = "Manual Inspection", ProcessStage = "Test", Vendor = "N/A", Model = "N/A", SerialNumber = "OQC-2023-001", Status = "Available", Location = "OQC-1", ResponsiblePerson = "10007", MaintenanceIntervalHours = 2000, RunningHours = 500 },
                new() { EquipmentId = "EQ-PACK-001", EquipmentName = "自动包装机#1", EquipmentGroup = "PACK", EquipmentType = "Auto Taping", ProcessStage = "Test", Vendor = "Taiyo", Model = "AT-200", SerialNumber = "PACK-2023-001", Status = "Available", Location = "Pack-1", ResponsiblePerson = "10009", MaintenanceIntervalHours = 1000, RunningHours = 800 },
            };
            await _context.MasterEquipments.AddRangeAsync(equipments);
            Console.WriteLine("[AuthSeeder] 设备数据初始化完成");
        }

        // 载具数据
        if (!await _context.MasterCarriers.AnyAsync())
        {
            var carriers = new MasterCarrier[]
            {
                new() { CarrierId = "CARRIER-TRAY-001", CarrierType = "Tray", Status = "Available", Capacity = 120, UseCount = 45, MaxUseCount = 500, CleanIntervalUses = 10, LastCleanDate = DateTime.UtcNow.AddDays(-2), Location = "Warehouse-A" },
                new() { CarrierId = "CARRIER-TRAY-002", CarrierType = "Tray", Status = "Available", Capacity = 120, UseCount = 120, MaxUseCount = 500, CleanIntervalUses = 10, LastCleanDate = DateTime.UtcNow.AddDays(-1), Location = "Warehouse-A" },
                new() { CarrierId = "CARRIER-TRAY-003", CarrierType = "Tray", Status = "Available", Capacity = 120, UseCount = 200, MaxUseCount = 500, CleanIntervalUses = 10, LastCleanDate = DateTime.UtcNow.AddDays(-3), Location = "Warehouse-B" },
                new() { CarrierId = "CARRIER-STRIP-001", CarrierType = "Strip", Status = "Available", Capacity = 1, UseCount = 80, MaxUseCount = 1000, CleanIntervalUses = 5, LastCleanDate = DateTime.UtcNow.AddDays(-1), Location = "Line-A1" },
                new() { CarrierId = "CARRIER-STRIP-002", CarrierType = "Strip", Status = "Available", Capacity = 1, UseCount = 150, MaxUseCount = 1000, CleanIntervalUses = 5, LastCleanDate = DateTime.UtcNow.AddDays(-4), Location = "Line-A2" },
                new() { CarrierId = "CARRIER-MAG-001", CarrierType = "Magazine", Status = "Available", Capacity = 50, UseCount = 30, MaxUseCount = 300, CleanIntervalUses = 20, LastCleanDate = DateTime.UtcNow.AddDays(-5), Location = "Warehouse-A" },
                new() { CarrierId = "CARRIER-TUBE-001", CarrierType = "Tube", Status = "Available", Capacity = 25, UseCount = 60, MaxUseCount = 400, CleanIntervalUses = 15, LastCleanDate = DateTime.UtcNow.AddDays(-2), Location = "Line-A1" },
                new() { CarrierId = "CARRIER-WF-001", CarrierType = "WaferFrame", Status = "Available", Capacity = 1, UseCount = 10, MaxUseCount = 50, CleanIntervalUses = 3, LastCleanDate = DateTime.UtcNow.AddDays(-1), Location = "Line-A1" },
            };
            await _context.MasterCarriers.AddRangeAsync(carriers);
            Console.WriteLine("[AuthSeeder] 载具数据初始化完成");
        }

        // Recipe数据
        if (!await _context.MasterRecipes.AnyAsync())
        {
            var recipes = new MasterRecipe[]
            {
                new() { RecipeId = "REC-SAW-QFN-V1", RecipeName = "QFN切割参数", EquipmentGroup = "SAW", StepCode = "SAW", Version = "V1", IsActive = true, Parameters = "{\"cutSpeed\":15,\"cutDepth\":0.3,\"spindleSpeed\":30000}" },
                new() { RecipeId = "REC-DA-QFN-V1", RecipeName = "QFN贴装参数", EquipmentGroup = "DA", StepCode = "DA", Version = "V1", IsActive = true, Parameters = "{\"bondForce\":5,\"bondTemp\":250,\"pickDelay\":50}" },
                new() { RecipeId = "REC-WB-QFN-V1", RecipeName = "QFN引线键合参数", EquipmentGroup = "WB", StepCode = "WB", Version = "V1", IsActive = true, Parameters = "{\"bondForce\":150,\"bondTime\":50,\"ultrasonicPower\":80,\"wireDiameter\":0.8,\"wireMaterial\":\"Au\"}" },
                new() { RecipeId = "REC-MOLD-QFN-V1", RecipeName = "QFN塑封参数", EquipmentGroup = "MOLD", StepCode = "MOLD", Version = "V1", IsActive = true, Parameters = "{\"moldTemp\":175,\"moldPressure\":8,\"cureTime\":120}" },
                new() { RecipeId = "REC-PMC-QFN-V1", RecipeName = "QFN固化参数", EquipmentGroup = "CURE", StepCode = "PMC", Version = "V1", IsActive = true, Parameters = "{\"cureTemp\":175,\"cureTime\":7200}" },
                new() { RecipeId = "REC-LASER-QFN-V1", RecipeName = "QFN打标参数", EquipmentGroup = "LASER", StepCode = "LASER", Version = "V1", IsActive = true, Parameters = "{\"laserPower\":80,\"markSpeed\":500,\"frequency\":20}" },
                new() { RecipeId = "REC-SING-QFN-V1", RecipeName = "QFN切筋参数", EquipmentGroup = "SING", StepCode = "SING", Version = "V1", IsActive = true, Parameters = "{\"cutSpeed\":20,\"cutPressure\":5}" },
                new() { RecipeId = "REC-FT-QFN-V1", RecipeName = "QFN成品测试参数", EquipmentGroup = "TEST", StepCode = "FT", Version = "V1", IsActive = true, Parameters = "{\"testVoltage\":3.3,\"testCurrent\":0.01,\"testFrequency\":1000}" },
                new() { RecipeId = "REC-OQC-QFN-V1", RecipeName = "QFN出货检验标准", EquipmentGroup = "OQC", StepCode = "OQC", Version = "V1", IsActive = true, Parameters = "{\"visualAQL\":0.65,\"dimAQL\":1.0,\"sampleSize\":\"AQL-II\"}" },
                new() { RecipeId = "REC-PACK-QFN-V1", RecipeName = "QFN包装参数", EquipmentGroup = "PACK", StepCode = "PACK", Version = "V1", IsActive = true, Parameters = "{\"tapeType\":\"Embossed\",\"reelSize\":13,\"sealTemp\":150}" },
            };
            await _context.MasterRecipes.AddRangeAsync(recipes);
            Console.WriteLine("[AuthSeeder] Recipe数据初始化完成");
        }

        // 报警规则
        if (!await _context.MasterAlarmRules.AnyAsync())
        {
            var alarmRules = new MasterAlarmRule[]
            {
                new() { RuleId = "ALARM-YIELD-001", AlarmType = "Yield", Severity = "Error", ThresholdYield = 90.0m, IsEnabled = true, NotifyRole = "QA,Eng,Mgr" },
                new() { RuleId = "ALARM-EQ-001", AlarmType = "Equipment", Severity = "Critical", IsEnabled = true, NotifyRole = "Maint,Mgr" },
                new() { RuleId = "ALARM-DELAY-001", AlarmType = "Delay", Severity = "Warning", ThresholdMinutes = 30, IsEnabled = true, NotifyRole = "ShiftLead,Eng" },
                new() { RuleId = "ALARM-QUALITY-001", AlarmType = "Quality", Severity = "Error", ThresholdQty = 5, IsEnabled = true, NotifyRole = "QA,Mgr" },
                new() { RuleId = "ALARM-INV-001", AlarmType = "Inventory", Severity = "Warning", IsEnabled = true, NotifyRole = "Planner,Mgr" },
            };
            await _context.MasterAlarmRules.AddRangeAsync(alarmRules);
            Console.WriteLine("[AuthSeeder] 报警规则初始化完成");
        }

        // 良率规则
        if (!await _context.MasterYieldRules.AnyAsync())
        {
            var yieldRules = new MasterYieldRule[]
            {
                new() { RuleId = "YIELD-QFN-001", RouteId = "QFN-STD:1.0", StepCode = "WB", YieldThreshold = 97.0m, ActionType = "AutoHold", NotifyRole = "QA", IsActive = true },
                new() { RuleId = "YIELD-QFN-002", RouteId = "QFN-STD:1.0", StepCode = "FT", YieldThreshold = 98.0m, ActionType = "AutoHold", NotifyRole = "QA", IsActive = true },
                new() { RuleId = "YIELD-SOP-001", RouteId = "SOP-STD:1.0", StepCode = "WB", YieldThreshold = 97.0m, ActionType = "AutoHold", NotifyRole = "QA", IsActive = true },
                new() { RuleId = "YIELD-SOP-002", RouteId = "SOP-STD:1.0", StepCode = "FT", YieldThreshold = 98.0m, ActionType = "AutoHold", NotifyRole = "QA", IsActive = true },
                new() { RuleId = "YIELD-BGA-001", RouteId = "BGA-STD:1.0", StepCode = "WB", YieldThreshold = 96.5m, ActionType = "AutoHold", NotifyRole = "QA", IsActive = true },
                new() { RuleId = "YIELD-BGA-002", RouteId = "BGA-STD:1.0", StepCode = "FT", YieldThreshold = 97.5m, ActionType = "AutoHold", NotifyRole = "QA", IsActive = true },
            };
            await _context.MasterYieldRules.AddRangeAsync(yieldRules);
            Console.WriteLine("[AuthSeeder] 良率规则初始化完成");
        }

        // 报废规则
        if (!await _context.MasterScrapRules.AnyAsync())
        {
            var scrapRules = new MasterScrapRule[]
            {
                new() { RuleId = "SCRAP-QFN-001", RouteId = "QFN-STD:1.0", StepCode = "WB", ThresholdPercent = 3.0m, RequiresApproval = true, ApprovalLevel = "Engineer", IsActive = true },
                new() { RuleId = "SCRAP-QFN-002", RouteId = "QFN-STD:1.0", StepCode = "FT", ThresholdPercent = 5.0m, RequiresApproval = true, ApprovalLevel = "Manager", IsActive = true },
                new() { RuleId = "SCRAP-SOP-001", RouteId = "SOP-STD:1.0", StepCode = "WB", ThresholdPercent = 3.0m, RequiresApproval = true, ApprovalLevel = "Engineer", IsActive = true },
                new() { RuleId = "SCRAP-SOP-002", RouteId = "SOP-STD:1.0", StepCode = "FT", ThresholdPercent = 5.0m, RequiresApproval = true, ApprovalLevel = "Manager", IsActive = true },
                new() { RuleId = "SCRAP-BGA-001", RouteId = "BGA-STD:1.0", StepCode = "WB", ThresholdPercent = 2.0m, RequiresApproval = true, ApprovalLevel = "Engineer", IsActive = true },
                new() { RuleId = "SCRAP-BGA-002", RouteId = "BGA-STD:1.0", StepCode = "FT", ThresholdPercent = 4.0m, RequiresApproval = true, ApprovalLevel = "Manager", IsActive = true },
            };
            await _context.MasterScrapRules.AddRangeAsync(scrapRules);
            Console.WriteLine("[AuthSeeder] 报废规则初始化完成");
        }

        // 物料数据
        if (!await _context.MasterMaterials.AnyAsync())
        {
            var materials = new MasterMaterial[]
            {
                new() { MaterialId = "MAT-LF-QFN88", MaterialName = "QFN88 引线框架", MaterialType = "LeadFrame", Specification = "48x48x0.25mm Cu Alloy C194", Unit = "strip", Supplier = "三井高科技", MinStock = 500, CurrentStock = 2500, IsActive = true },
                new() { MaterialId = "MAT-LF-SOP8", MaterialName = "SOP8 引线框架", MaterialType = "LeadFrame", Specification = "20x10x0.25mm Cu Alloy C194", Unit = "strip", Supplier = "三井高科技", MinStock = 1000, CurrentStock = 5000, IsActive = true },
                new() { MaterialId = "MAT-LF-QFP64", MaterialName = "QFP64 引线框架", MaterialType = "LeadFrame", Specification = "32x32x0.25mm Cu Alloy C194", Unit = "strip", Supplier = "日东电工", MinStock = 300, CurrentStock = 1500, IsActive = true },
                new() { MaterialId = "MAT-SUB-BGA256", MaterialName = "BGA256 基板", MaterialType = "Substrate", Specification = "40x40mm 4-Layer FR4", Unit = "panel", Supplier = "新光电气", MinStock = 200, CurrentStock = 800, IsActive = true },
                new() { MaterialId = "MAT-SUB-BGA512", MaterialName = "BGA512 基板", MaterialType = "Substrate", Specification = "50x50mm 6-Layer BT", Unit = "panel", Supplier = "新光电气", MinStock = 100, CurrentStock = 400, IsActive = true },
                new() { MaterialId = "MAT-EMC-STD", MaterialName = "标准型环氧塑封料", MaterialType = "EMC", Specification = "EMC-GP500 Black", Unit = "kg", Supplier = "住友电木", MinStock = 50, CurrentStock = 200, IsActive = true },
                new() { MaterialId = "MAT-EMC-LOW", MaterialName = "低应力塑封料", MaterialType = "EMC", Specification = "EMC-LS300 Green", Unit = "kg", Supplier = "住友电木", MinStock = 30, CurrentStock = 100, IsActive = true },
                new() { MaterialId = "MAT-WIRE-AU-08", MaterialName = "金线 0.8mil", MaterialType = "Wire", Specification = "99.99% Au 0.8mil", Unit = "reel", Supplier = "贺利氏", MinStock = 20, CurrentStock = 100, IsActive = true },
                new() { MaterialId = "MAT-WIRE-AU-10", MaterialName = "金线 1.0mil", MaterialType = "Wire", Specification = "99.99% Au 1.0mil", Unit = "reel", Supplier = "贺利氏", MinStock = 20, CurrentStock = 80, IsActive = true },
                new() { MaterialId = "MAT-WIRE-CU-08", MaterialName = "铜线 0.8mil", MaterialType = "Wire", Specification = "Cu 0.8mil Pd Coated", Unit = "reel", Supplier = "田中贵金属", MinStock = 15, CurrentStock = 60, IsActive = true },
                new() { MaterialId = "MAT-BLADE-1", MaterialName = "晶圆切割刀片", MaterialType = "Blade", Specification = "Dicing Blade 0.3mm Ni Bond", Unit = "pcs", Supplier = "Disco", MinStock = 10, CurrentStock = 50, IsActive = true },
                new() { MaterialId = "MAT-BLADE-2", MaterialName = "塑封切割刀片", MaterialType = "Blade", Specification = "Dicing Blade 0.5mm Resin", Unit = "pcs", Supplier = "Disco", MinStock = 10, CurrentStock = 40, IsActive = true },
                new() { MaterialId = "MAT-DA-AG", MaterialName = "导电银浆", MaterialType = "DieAttach", Specification = "Ag Epoxy DA-3000", Unit = "roll", Supplier = "汉高", MinStock = 5, CurrentStock = 30, IsActive = true },
                new() { MaterialId = "MAT-DA-DAF", MaterialName = "Die Attach Film", MaterialType = "DieAttach", Specification = "DAF-500 25μm", Unit = "roll", Supplier = "日东电工", MinStock = 5, CurrentStock = 25, IsActive = true },
                new() { MaterialId = "MAT-TAPE-8MM", MaterialName = "8mm 载带", MaterialType = "Packaging", Specification = "8mm Embossed Tape PS", Unit = "reel", Supplier = "三井高科技", MinStock = 50, CurrentStock = 300, IsActive = true },
                new() { MaterialId = "MAT-TAPE-12MM", MaterialName = "12mm 载带", MaterialType = "Packaging", Specification = "12mm Embossed Tape PS", Unit = "reel", Supplier = "三井高科技", MinStock = 30, CurrentStock = 200, IsActive = true },
                new() { MaterialId = "MAT-TAPE-16MM", MaterialName = "16mm 载带", MaterialType = "Packaging", Specification = "16mm Embossed Tape PS", Unit = "reel", Supplier = "三井高科技", MinStock = 20, CurrentStock = 150, IsActive = true },
                new() { MaterialId = "MAT-REEL-7", MaterialName = "7英寸卷盘", MaterialType = "Packaging", Specification = "7 inch Reel 4mm Paper", Unit = "pcs", Supplier = "通用", MinStock = 100, CurrentStock = 500, IsActive = true },
                new() { MaterialId = "MAT-REEL-13", MaterialName = "13英寸卷盘", MaterialType = "Packaging", Specification = "13 inch Reel 8mm PS", Unit = "pcs", Supplier = "通用", MinStock = 50, CurrentStock = 300, IsActive = true },
                new() { MaterialId = "MAT-DESICCANT", MaterialName = "干燥剂", MaterialType = "Packaging", Specification = "50g Silica Gel Pack", Unit = "pcs", Supplier = "通用", MinStock = 500, CurrentStock = 3000, IsActive = true },
                new() { MaterialId = "MAT-HUMIDITY-CARD", MaterialName = "湿度指示卡", MaterialType = "Packaging", Specification = "3-Spot 10%/60%/90%", Unit = "pcs", Supplier = "通用", MinStock = 200, CurrentStock = 1000, IsActive = true },
            };
            await _context.MasterMaterials.AddRangeAsync(materials);
            Console.WriteLine("[AuthSeeder] 物料数据初始化完成");
        }

        // 原因代码
        if (!await _context.MasterReasonCodes.AnyAsync())
        {
            var reasonCodes = new MasterReasonCode[]
            {
                new() { ReasonCodeId = "RSN-MAT-001", Category = "Material", SubCategory = "来料不良", ReasonText = "原材料来料不良，供应商责任", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-MAT-002", Category = "Material", SubCategory = "材料混批", ReasonText = "不同批次材料混用", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-MAT-003", Category = "Material", SubCategory = "材料过期", ReasonText = "材料超过有效期", ApplicableTo = "Scrap", IsEnabled = true },
                new() { ReasonCodeId = "RSN-MAT-004", Category = "Material", SubCategory = "材料短缺", ReasonText = "生产中发现材料不足", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-MAT-005", Category = "Material", SubCategory = "材料用错", ReasonText = "使用了错误的材料型号", ApplicableTo = "Rework", IsEnabled = true },
                new() { ReasonCodeId = "RSN-EQ-001", Category = "Equipment", SubCategory = "设备故障", ReasonText = "设备突发故障，需维修", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-EQ-002", Category = "Equipment", SubCategory = "保养中", ReasonText = "设备定期保养", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-EQ-003", Category = "Equipment", SubCategory = "参数异常", ReasonText = "设备运行参数超出标准范围", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-EQ-004", Category = "Equipment", SubCategory = "耗材耗尽", ReasonText = "设备耗材耗尽需要更换", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-EQ-005", Category = "Equipment", SubCategory = "设备损坏产品", ReasonText = "设备异常导致产品损坏", ApplicableTo = "Scrap", IsEnabled = true },
                new() { ReasonCodeId = "RSN-PRC-001", Category = "Process", SubCategory = "工艺变更", ReasonText = "客户/工程工艺变更通知", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-PRC-002", Category = "Process", SubCategory = "参数错误", ReasonText = "使用了错误的工艺参数", ApplicableTo = "Rework", IsEnabled = true },
                new() { ReasonCodeId = "RSN-PRC-003", Category = "Process", SubCategory = "超时滞留", ReasonText = "批次在某工序超时滞留", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-PRC-004", Category = "Process", SubCategory = "漏工序", ReasonText = "批次漏做了某个工序", ApplicableTo = "Rework", IsEnabled = true },
                new() { ReasonCodeId = "RSN-PRC-005", Category = "Process", SubCategory = "工艺违规", ReasonText = "违反工艺操作规范", ApplicableTo = "Scrap", IsEnabled = true },
                new() { ReasonCodeId = "RSN-QA-001", Category = "Quality", SubCategory = "良率超标", ReasonText = "工序良率低于控制下限", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-QA-002", Category = "Quality", SubCategory = "检验不合格", ReasonText = "品质检验判定不合格", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-QA-003", Category = "Quality", SubCategory = "客户投诉", ReasonText = "客户品质投诉，需追溯调查", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-QA-004", Category = "Quality", SubCategory = "SPC异常", ReasonText = "SPC监控发现异常趋势", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-QA-005", Category = "Quality", SubCategory = "外观缺陷", ReasonText = "产品外观存在不可接受缺陷", ApplicableTo = "Scrap", IsEnabled = true },
                new() { ReasonCodeId = "RSN-QA-006", Category = "Quality", SubCategory = "电性不良", ReasonText = "产品电性参数不达标", ApplicableTo = "Scrap", IsEnabled = true },
                new() { ReasonCodeId = "RSN-PER-001", Category = "Personnel", SubCategory = "操作失误", ReasonText = "人员操作失误", ApplicableTo = "Rework", IsEnabled = true },
                new() { ReasonCodeId = "RSN-PER-002", Category = "Personnel", SubCategory = "未培训上岗", ReasonText = "操作人员未完成培训", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-OTH-001", Category = "Other", SubCategory = "工程实验", ReasonText = "工程实验需要", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-OTH-002", Category = "Other", SubCategory = "客户特殊要求", ReasonText = "客户特殊要求暂停", ApplicableTo = "Hold", IsEnabled = true },
                new() { ReasonCodeId = "RSN-OTH-003", Category = "Other", SubCategory = "其他", ReasonText = "其他原因", ApplicableTo = "Hold", IsEnabled = true },
            };
            await _context.MasterReasonCodes.AddRangeAsync(reasonCodes);
            Console.WriteLine("[AuthSeeder] 原因代码初始化完成");
        }

        // 缺陷代码
        if (!await _context.MasterDefectCodes.AnyAsync())
        {
            var defectCodes = new MasterDefectCode[]
            {
                new() { DefectCodeId = "DEF-COS-001", DefectCategory = "Cosmetic", DefectText = "表面划伤", Severity = "Major", IsEnabled = true },
                new() { DefectCodeId = "DEF-COS-002", DefectCategory = "Cosmetic", DefectText = "脏污/异物", Severity = "Minor", IsEnabled = true },
                new() { DefectCodeId = "DEF-COS-003", DefectCategory = "Cosmetic", DefectText = "颜色不均", Severity = "Minor", IsEnabled = true },
                new() { DefectCodeId = "DEF-COS-004", DefectCategory = "Cosmetic", DefectText = "标记模糊", Severity = "Major", IsEnabled = true },
                new() { DefectCodeId = "DEF-COS-005", DefectCategory = "Cosmetic", DefectText = "引脚变形", Severity = "Major", IsEnabled = true },
                new() { DefectCodeId = "DEF-COS-006", DefectCategory = "Cosmetic", DefectText = "裂纹/破损", Severity = "Critical", IsEnabled = true },
                new() { DefectCodeId = "DEF-COS-007", DefectCategory = "Cosmetic", DefectText = "气泡", Severity = "Minor", IsEnabled = true },
                new() { DefectCodeId = "DEF-COS-008", DefectCategory = "Cosmetic", DefectText = "缺胶/填充不足", Severity = "Major", IsEnabled = true },
                new() { DefectCodeId = "DEF-DIM-001", DefectCategory = "Dimensional", DefectText = "外形尺寸超差", Severity = "Major", IsEnabled = true },
                new() { DefectCodeId = "DEF-DIM-002", DefectCategory = "Dimensional", DefectText = "引脚间距超差", Severity = "Critical", IsEnabled = true },
                new() { DefectCodeId = "DEF-DIM-003", DefectCategory = "Dimensional", DefectText = "厚度超差", Severity = "Major", IsEnabled = true },
                new() { DefectCodeId = "DEF-DIM-004", DefectCategory = "Dimensional", DefectText = "共面度超差", Severity = "Critical", IsEnabled = true },
                new() { DefectCodeId = "DEF-DIM-005", DefectCategory = "Dimensional", DefectText = "切割偏移", Severity = "Major", IsEnabled = true },
                new() { DefectCodeId = "DEF-ELE-001", DefectCategory = "Electrical", DefectText = "开路", Severity = "Critical", IsEnabled = true },
                new() { DefectCodeId = "DEF-ELE-002", DefectCategory = "Electrical", DefectText = "短路", Severity = "Critical", IsEnabled = true },
                new() { DefectCodeId = "DEF-ELE-003", DefectCategory = "Electrical", DefectText = "漏电超标", Severity = "Critical", IsEnabled = true },
                new() { DefectCodeId = "DEF-ELE-004", DefectCategory = "Electrical", DefectText = "参数漂移", Severity = "Major", IsEnabled = true },
                new() { DefectCodeId = "DEF-ELE-005", DefectCategory = "Electrical", DefectText = "耐压不良", Severity = "Critical", IsEnabled = true },
                new() { DefectCodeId = "DEF-ELE-006", DefectCategory = "Electrical", DefectText = "接触电阻超标", Severity = "Major", IsEnabled = true },
                new() { DefectCodeId = "DEF-ELE-007", DefectCategory = "Electrical", DefectText = "功能失效", Severity = "Critical", IsEnabled = true },
                new() { DefectCodeId = "DEF-FUN-001", DefectCategory = "Functional", DefectText = "信号传输异常", Severity = "Critical", IsEnabled = true },
                new() { DefectCodeId = "DEF-FUN-002", DefectCategory = "Functional", DefectText = "频率响应异常", Severity = "Major", IsEnabled = true },
                new() { DefectCodeId = "DEF-FUN-003", DefectCategory = "Functional", DefectText = "逻辑功能失效", Severity = "Critical", IsEnabled = true },
                new() { DefectCodeId = "DEF-FUN-004", DefectCategory = "Functional", DefectText = "温度特性不良", Severity = "Major", IsEnabled = true },
                new() { DefectCodeId = "DEF-FUN-005", DefectCategory = "Functional", DefectText = "ESD损坏", Severity = "Critical", IsEnabled = true },
            };
            await _context.MasterDefectCodes.AddRangeAsync(defectCodes);
            Console.WriteLine("[AuthSeeder] 缺陷代码初始化完成");
        }

        Console.WriteLine("[AuthSeeder] 主数据种子数据初始化完成");
    }
}
