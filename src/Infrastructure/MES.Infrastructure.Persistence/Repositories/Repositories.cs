using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Infrastructure.Persistence.Repositories;

public class RouteRepository : Repository<MasterRoute>, IRouteRepository
{
    public RouteRepository(MesDbContext context) : base(context) { }

    public async Task<MasterRoute?> GetRouteWithStepsAsync(string routeId, string? version = null)
    {
        var query = _context.MasterRoutes
            .Include(r => r.Steps)
            .Where(r => r.RouteId == routeId);

        if (!string.IsNullOrEmpty(version))
            query = query.Where(r => r.RouteVersion == version);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<MasterRoute>> GetActiveRoutesByProductAsync(string productId) =>
        await _context.MasterRoutes
            .Include(r => r.Steps)
            .Where(r => r.ProductId == productId && r.IsActive)
            .ToListAsync();

    public async Task<List<MasterRouteStep>> GetRouteStepsAsync(string routeId) =>
        await _context.MasterRouteSteps
            .Where(s => s.RouteId == routeId)
            .OrderBy(s => s.StepSeq)
            .ToListAsync();
}

public class LotRepository : Repository<ProdLot>, ILotRepository
{
    public LotRepository(MesDbContext context) : base(context) { }

    public async Task<ProdLot?> GetLotWithStepsAsync(string lotId) =>
        await _context.ProdLots
            .FirstOrDefaultAsync(l => l.LotId == lotId);

    public async Task<List<ProdLotStep>> GetLotStepsAsync(string lotId) =>
        await _context.ProdLotSteps
            .Where(s => s.LotId == lotId)
            .OrderBy(s => s.StepSeq)
            .ToListAsync();

    public async Task<List<ProdLot>> GetLotsByOrderIdAsync(string orderId) =>
        await _context.ProdLots
            .Where(l => l.OrderId == orderId)
            .ToListAsync();

    public async Task<List<ProdLot>> GetLotsByStatusAsync(string status) =>
        await _context.ProdLots
            .Where(l => l.Status == status)
            .ToListAsync();

    public async Task<ProdLotStep?> GetLotStepAsync(string lotId, string stepCode) =>
        await _context.ProdLotSteps
            .FirstOrDefaultAsync(s => s.LotId == lotId && s.StepCode == stepCode);

    public async Task<List<ProdLotStep>> GetLotStepsByStatusAsync(string lotId, string status) =>
        await _context.ProdLotSteps
            .Where(s => s.LotId == lotId && s.Status == status)
            .OrderBy(s => s.StepSeq)
            .ToListAsync();
}

public class WorkOrderRepository : Repository<ProdWorkOrder>, IWorkOrderRepository
{
    public WorkOrderRepository(MesDbContext context) : base(context) { }

    public async Task<List<ProdWorkOrder>> GetOrdersByStatusAsync(string status) =>
        await _context.ProdWorkOrders
            .Where(o => o.Status == status)
            .ToListAsync();

    public async Task<List<ProdWorkOrder>> GetOrdersByProductAsync(string productId) =>
        await _context.ProdWorkOrders
            .Where(o => o.ProductId == productId)
            .ToListAsync();
}

public class EquipmentRepository : Repository<MasterEquipment>, IEquipmentRepository
{
    public EquipmentRepository(MesDbContext context) : base(context) { }

    public async Task<List<MasterEquipment>> GetEquipmentsByGroupAsync(string group) =>
        await _context.MasterEquipments
            .Where(e => e.EquipmentGroup == group)
            .ToListAsync();

    public async Task<List<MasterEquipment>> GetAvailableEquipmentsAsync(string? group = null)
    {
        var query = _context.MasterEquipments
            .Where(e => e.Status == "Available");

        if (!string.IsNullOrEmpty(group))
            query = query.Where(e => e.EquipmentGroup == group);

        return await query.ToListAsync();
    }
}

public class CarrierRepository : Repository<MasterCarrier>, ICarrierRepository
{
    public CarrierRepository(MesDbContext context) : base(context) { }

    public async Task<List<MasterCarrier>> GetCarriersByTypeAsync(string type) =>
        await _context.MasterCarriers
            .Where(c => c.CarrierType == type)
            .ToListAsync();

    public async Task<List<MasterCarrier>> GetAvailableCarriersAsync(string? type = null)
    {
        var query = _context.MasterCarriers
            .Where(c => c.Status == "Available");

        if (!string.IsNullOrEmpty(type))
            query = query.Where(c => c.CarrierType == type);

        return await query.ToListAsync();
    }
}

public class RecipeRepository : Repository<MasterRecipe>, IRecipeRepository
{
    public RecipeRepository(MesDbContext context) : base(context) { }

    public async Task<List<MasterRecipe>> GetRecipesByStepAsync(string stepCode) =>
        await _context.MasterRecipes
            .Where(r => r.StepCode == stepCode)
            .ToListAsync();

    public async Task<List<MasterRecipe>> GetActiveRecipesByEquipmentGroupAsync(string group) =>
        await _context.MasterRecipes
            .Where(r => r.EquipmentGroup == group && r.IsActive)
            .ToListAsync();
}

public class YieldRuleRepository : Repository<MasterYieldRule>, IYieldRuleRepository
{
    public YieldRuleRepository(MesDbContext context) : base(context) { }

    public async Task<List<MasterYieldRule>> GetRulesByRouteAsync(string routeId) =>
        await _context.MasterYieldRules
            .Where(r => r.RouteId == routeId && r.IsActive)
            .ToListAsync();

    public async Task<List<MasterYieldRule>> GetRulesByStepAsync(string routeId, string stepCode) =>
        await _context.MasterYieldRules
            .Where(r => r.RouteId == routeId && r.StepCode == stepCode && r.IsActive)
            .ToListAsync();
}

public class AlarmRuleRepository : Repository<MasterAlarmRule>, IAlarmRuleRepository
{
    public AlarmRuleRepository(MesDbContext context) : base(context) { }

    public async Task<List<MasterAlarmRule>> GetEnabledRulesAsync() =>
        await _context.MasterAlarmRules
            .Where(r => r.IsEnabled)
            .ToListAsync();
}

public class ScrapRuleRepository : Repository<MasterScrapRule>, IScrapRuleRepository
{
    public ScrapRuleRepository(MesDbContext context) : base(context) { }

    public async Task<List<MasterScrapRule>> GetActiveRulesAsync() =>
        await _context.MasterScrapRules
            .Where(r => r.IsActive)
            .ToListAsync();
}

public class OperationHistoryRepository : Repository<ProdOperationHistory>, IOperationHistoryRepository
{
    public OperationHistoryRepository(MesDbContext context) : base(context) { }

    public async Task<List<ProdOperationHistory>> GetByLotIdAsync(string lotId) =>
        await _context.ProdOperationHistories
            .Where(h => h.LotId == lotId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();

    public async Task<List<ProdOperationHistory>> GetByOrderIdAsync(string orderId) =>
        await _context.ProdOperationHistories
            .Where(h => h.OrderId == orderId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
}

public class ScrapRecordRepository : Repository<ProdScrapRecord>, IScrapRecordRepository
{
    public ScrapRecordRepository(MesDbContext context) : base(context) { }

    public async Task<List<ProdScrapRecord>> GetByLotIdAsync(string lotId) =>
        await _context.ProdScrapRecords
            .Where(r => r.LotId == lotId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
}

public class ReworkRecordRepository : Repository<ProdReworkRecord>, IReworkRecordRepository
{
    public ReworkRecordRepository(MesDbContext context) : base(context) { }

    public async Task<List<ProdReworkRecord>> GetByLotIdAsync(string lotId) =>
        await _context.ProdReworkRecords
            .Where(r => r.LotId == lotId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
}

public class LotSplitRepository : Repository<ProdLotSplit>, ILotSplitRepository
{
    public LotSplitRepository(MesDbContext context) : base(context) { }

    public async Task<List<ProdLotSplit>> GetByMotherLotIdAsync(string motherLotId) =>
        await _context.ProdLotSplits
            .Where(s => s.MotherLotId == motherLotId)
            .ToListAsync();

    public async Task<List<ProdLotSplit>> GetByChildLotIdAsync(string childLotId) =>
        await _context.ProdLotSplits
            .Where(s => s.ChildLotId == childLotId)
            .ToListAsync();
}

public class LotMergeRepository : Repository<ProdLotMerge>, ILotMergeRepository
{
    public LotMergeRepository(MesDbContext context) : base(context) { }

    public async Task<List<ProdLotMerge>> GetByTargetLotIdAsync(string targetLotId) =>
        await _context.ProdLotMerges
            .Where(m => m.TargetLotId == targetLotId)
            .ToListAsync();

    public async Task<List<ProdLotMerge>> GetBySourceLotIdAsync(string sourceLotId) =>
        await _context.ProdLotMerges
            .Where(m => m.SourceLotId == sourceLotId)
            .ToListAsync();
}

public class CarrierBindingRepository : Repository<ProdCarrierBinding>, ICarrierBindingRepository
{
    public CarrierBindingRepository(MesDbContext context) : base(context) { }

    public async Task<List<ProdCarrierBinding>> GetByLotIdAsync(string lotId) =>
        await _context.ProdCarrierBindings
            .Where(b => b.LotId == lotId)
            .ToListAsync();

    public async Task<List<ProdCarrierBinding>> GetByCarrierIdAsync(string carrierId) =>
        await _context.ProdCarrierBindings
            .Where(b => b.CarrierId == carrierId)
            .ToListAsync();
}

public class SignatureRepository : Repository<ProdSignature>, ISignatureRepository
{
    public SignatureRepository(MesDbContext context) : base(context) { }

    public async Task<List<ProdSignature>> GetByEntityAsync(string entityType, string entityId) =>
        await _context.ProdSignatures
            .Where(s => s.EntityType == entityType && s.EntityId == entityId)
            .OrderByDescending(s => s.SignTime)
            .ToListAsync();
}

public class AuditTrailRepository : Repository<ProdAuditTrail>, IAuditTrailRepository
{
    public AuditTrailRepository(MesDbContext context) : base(context) { }

    public async Task<List<ProdAuditTrail>> GetByEntityAsync(string entityType, string entityId) =>
        await _context.ProdAuditTrails
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();

    public async Task<List<ProdAuditTrail>> GetRecentAsync(int count = 50) =>
        await _context.ProdAuditTrails
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();
}

public class AlarmRepository : Repository<ProdAlarm>, IAlarmRepository
{
    public AlarmRepository(MesDbContext context) : base(context) { }

    public async Task<List<ProdAlarm>> GetUnacknowledgedAsync() =>
        await _context.ProdAlarms
            .Where(a => a.Status == "Active")
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

    public async Task<List<ProdAlarm>> GetByLotIdAsync(string lotId) =>
        await _context.ProdAlarms
            .Where(a => a.LotId == lotId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
}

public class DispatchTaskRepository : Repository<ProdDispatchTask>, IDispatchTaskRepository
{
    public DispatchTaskRepository(MesDbContext context) : base(context) { }

    public async Task<List<ProdDispatchTask>> GetPendingTasksAsync() =>
        await _context.ProdDispatchTasks
            .Where(t => t.Status == "Pending")
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();

    public async Task<List<ProdDispatchTask>> GetTasksByLotIdAsync(string lotId) =>
        await _context.ProdDispatchTasks
            .Where(t => t.LotId == lotId)
            .ToListAsync();
}

public class QuantityTransactionRepository : Repository<QuantityTransaction>, IQuantityTransactionRepository
{
    public QuantityTransactionRepository(MesDbContext context) : base(context) { }

    public async Task<List<QuantityTransaction>> GetByLotIdAsync(string lotId) =>
        await _context.QuantityTransactions
            .Where(t => t.LotId == lotId)
            .OrderBy(t => t.StepSeq)
            .ToListAsync();

    public async Task<List<QuantityTransaction>> GetByLotAndStepAsync(string lotId, string stepCode) =>
        await _context.QuantityTransactions
            .Where(t => t.LotId == lotId && t.StepCode == stepCode)
            .ToListAsync();
}

public class ProductRepository : Repository<MasterProduct>, IProductRepository
{
    public ProductRepository(MesDbContext context) : base(context) { }

    public async Task<List<MasterProduct>> GetActiveProductsAsync() =>
        await _context.MasterProducts
            .Where(p => p.Status == "Active")
            .ToListAsync();
}

public class UserRepository : Repository<SysUser>, IUserRepository
{
    public UserRepository(MesDbContext context) : base(context) { }

    public async Task<SysUser?> GetUserWithRoleAsync(string userId) =>
        await _context.SysUsers
            .FirstOrDefaultAsync(u => u.UserId == userId);

    public async Task<List<SysUser>> GetUsersByDeptAsync(string deptId) =>
        await _context.SysUsers
            .Where(u => u.DeptId == deptId && u.IsActive)
            .ToListAsync();

    public async Task<List<SysUser>> GetActiveUsersAsync() =>
        await _context.SysUsers
            .Where(u => u.IsActive)
            .ToListAsync();
}

public class RoleRepository : Repository<SysRole>, IRoleRepository
{
    public RoleRepository(MesDbContext context) : base(context) { }

    public async Task<SysRole?> GetRoleWithPermissionsAsync(string roleId) =>
        await _context.SysRoles
            .FirstOrDefaultAsync(r => r.RoleId == roleId);
}

public class DepartmentRepository : Repository<SysDepartment>, IDepartmentRepository
{
    public DepartmentRepository(MesDbContext context) : base(context) { }

    public async Task<List<SysDepartment>> GetDepartmentTreeAsync() =>
        await _context.SysDepartments
            .OrderBy(d => d.ParentId)
            .ThenBy(d => d.DeptName)
            .ToListAsync();
}

public class SignatureLevelRepository : Repository<SysSignatureLevel>, ISignatureLevelRepository
{
    public SignatureLevelRepository(MesDbContext context) : base(context) { }

    public async Task<SysSignatureLevel?> GetLevelAsync(string levelCode) =>
        await _context.SysSignatureLevels
            .FirstOrDefaultAsync(l => l.LevelCode == levelCode);
}

public class PermissionConfirmRepository : Repository<SysPermissionConfirm>, IPermissionConfirmRepository
{
    public PermissionConfirmRepository(MesDbContext context) : base(context) { }

    public async Task<List<SysPermissionConfirm>> GetByEmployeeAsync(string employeeId, int days = 7)
    {
        var cutoff = DateTime.UtcNow.AddDays(-days);
        return await _context.SysPermissionConfirms
            .Where(c => c.EmployeeId == employeeId && c.ConfirmAt >= cutoff)
            .OrderByDescending(c => c.ConfirmAt)
            .ToListAsync();
    }

    public async Task<List<SysPermissionConfirm>> GetByOperationAsync(string operationType, DateTime? from = null, DateTime? to = null)
    {
        var query = _context.SysPermissionConfirms
            .Where(c => c.OperationType == operationType);

        if (from.HasValue)
            query = query.Where(c => c.ConfirmAt >= from.Value);
        if (to.HasValue)
            query = query.Where(c => c.ConfirmAt <= to.Value);

        return await query
            .OrderByDescending(c => c.ConfirmAt)
            .ToListAsync();
    }
}
