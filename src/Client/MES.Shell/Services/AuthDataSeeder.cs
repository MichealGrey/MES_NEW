using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Shell.Helpers;
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
        if (await _context.SysUsers.AnyAsync())
            return;

        await SeedDepartmentsAsync();
        await SeedRolesAsync();
        await SeedPermissionsAsync();
        await SeedUsersAsync();

        await _context.SaveChangesAsync();
        Console.WriteLine("[AuthSeeder] 认证种子数据初始化完成");
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

            // 生产主管: 生产+排程+良率+追溯
            ("ROLE-MGR", "Production:*"),
            ("ROLE-MGR", "Schedule:*"),
            ("ROLE-MGR", "Yield:*"),
            ("ROLE-MGR", "Trace:*"),

            // 领班/组长: 核心生产操作
            ("ROLE-SHIFT-LEAD", "Production:WorkOrderListView"),
            ("ROLE-SHIFT-LEAD", "Production:LotListView"),
            ("ROLE-SHIFT-LEAD", "Production:TrackInView"),
            ("ROLE-SHIFT-LEAD", "Production:WipOverviewView"),
            ("ROLE-SHIFT-LEAD", "Production:LotHoldView"),
            ("ROLE-SHIFT-LEAD", "Production:LotSplitMergeView"),
            ("ROLE-SHIFT-LEAD", "Production:CustomerProgressView"),
            ("ROLE-SHIFT-LEAD", "Production:ProductionReportView"),

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

            // 品质工程师: 质量+Hold
            ("ROLE-QA", "Quality:*"),
            ("ROLE-QA", "Production:LotHoldView"),
            ("ROLE-QA", "Production:LotListView"),
            ("ROLE-QA", "Production:WipOverviewView"),

            // 计划员: 工单+批次+排程
            ("ROLE-PLANNER", "Production:WorkOrderListView"),
            ("ROLE-PLANNER", "Production:LotListView"),
            ("ROLE-PLANNER", "Production:WipOverviewView"),
            ("ROLE-PLANNER", "Production:CustomerProgressView"),
            ("ROLE-PLANNER", "Schedule:*"),

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
            var hash = PasswordHelper.HashPassword(password);

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
}
