using MES.Infrastructure.Persistence.Entities;

namespace MES.Infrastructure.Persistence.Repositories;

public interface IRouteRepository : IRepository<MasterRoute>
{
    Task<MasterRoute?> GetRouteWithStepsAsync(string routeId, string? version = null);
    Task<List<MasterRoute>> GetActiveRoutesByProductAsync(string productId);
    Task<List<MasterRouteStep>> GetRouteStepsAsync(string routeId);
}

public interface ILotRepository : IRepository<ProdLot>
{
    Task<ProdLot?> GetLotWithStepsAsync(string lotId);
    Task<List<ProdLotStep>> GetLotStepsAsync(string lotId);
    Task<List<ProdLot>> GetLotsByOrderIdAsync(string orderId);
    Task<List<ProdLot>> GetLotsByStatusAsync(string status);
    Task<ProdLotStep?> GetLotStepAsync(string lotId, string stepCode);
    Task<List<ProdLotStep>> GetLotStepsByStatusAsync(string lotId, string status);
}

public interface IWorkOrderRepository : IRepository<ProdWorkOrder>
{
    Task<List<ProdWorkOrder>> GetOrdersByStatusAsync(string status);
    Task<List<ProdWorkOrder>> GetOrdersByProductAsync(string productId);
}

public interface IEquipmentRepository : IRepository<MasterEquipment>
{
    Task<List<MasterEquipment>> GetEquipmentsByGroupAsync(string group);
    Task<List<MasterEquipment>> GetAvailableEquipmentsAsync(string? group = null);
}

public interface ICarrierRepository : IRepository<MasterCarrier>
{
    Task<List<MasterCarrier>> GetCarriersByTypeAsync(string type);
    Task<List<MasterCarrier>> GetAvailableCarriersAsync(string? type = null);
}

public interface IRecipeRepository : IRepository<MasterRecipe>
{
    Task<List<MasterRecipe>> GetRecipesByStepAsync(string stepCode);
    Task<List<MasterRecipe>> GetActiveRecipesByEquipmentGroupAsync(string group);
}

public interface IYieldRuleRepository : IRepository<MasterYieldRule>
{
    Task<List<MasterYieldRule>> GetRulesByRouteAsync(string routeId);
    Task<List<MasterYieldRule>> GetRulesByStepAsync(string routeId, string stepCode);
}

public interface IAlarmRuleRepository : IRepository<MasterAlarmRule>
{
    Task<List<MasterAlarmRule>> GetEnabledRulesAsync();
}

public interface IScrapRuleRepository : IRepository<MasterScrapRule>
{
    Task<List<MasterScrapRule>> GetActiveRulesAsync();
}

public interface IOperationHistoryRepository : IRepository<ProdOperationHistory>
{
    Task<List<ProdOperationHistory>> GetByLotIdAsync(string lotId);
    Task<List<ProdOperationHistory>> GetByOrderIdAsync(string orderId);
}

public interface IScrapRecordRepository : IRepository<ProdScrapRecord>
{
    Task<List<ProdScrapRecord>> GetByLotIdAsync(string lotId);
}

public interface IReworkRecordRepository : IRepository<ProdReworkRecord>
{
    Task<List<ProdReworkRecord>> GetByLotIdAsync(string lotId);
}

public interface ILotSplitRepository : IRepository<ProdLotSplit>
{
    Task<List<ProdLotSplit>> GetByMotherLotIdAsync(string motherLotId);
    Task<List<ProdLotSplit>> GetByChildLotIdAsync(string childLotId);
}

public interface ILotMergeRepository : IRepository<ProdLotMerge>
{
    Task<List<ProdLotMerge>> GetByTargetLotIdAsync(string targetLotId);
    Task<List<ProdLotMerge>> GetBySourceLotIdAsync(string sourceLotId);
}

public interface ICarrierBindingRepository : IRepository<ProdCarrierBinding>
{
    Task<List<ProdCarrierBinding>> GetByLotIdAsync(string lotId);
    Task<List<ProdCarrierBinding>> GetByCarrierIdAsync(string carrierId);
}

public interface ISignatureRepository : IRepository<ProdSignature>
{
    Task<List<ProdSignature>> GetByEntityAsync(string entityType, string entityId);
}

public interface IAuditTrailRepository : IRepository<ProdAuditTrail>
{
    Task<List<ProdAuditTrail>> GetByEntityAsync(string entityType, string entityId);
    Task<List<ProdAuditTrail>> GetRecentAsync(int count = 50);
}

public interface IAlarmRepository : IRepository<ProdAlarm>
{
    Task<List<ProdAlarm>> GetUnacknowledgedAsync();
    Task<List<ProdAlarm>> GetByLotIdAsync(string lotId);
}

public interface IDispatchTaskRepository : IRepository<ProdDispatchTask>
{
    Task<List<ProdDispatchTask>> GetPendingTasksAsync();
    Task<List<ProdDispatchTask>> GetTasksByLotIdAsync(string lotId);
}

public interface IQuantityTransactionRepository : IRepository<QuantityTransaction>
{
    Task<List<QuantityTransaction>> GetByLotIdAsync(string lotId);
    Task<List<QuantityTransaction>> GetByLotAndStepAsync(string lotId, string stepCode);
}

public interface IProductRepository : IRepository<MasterProduct>
{
    Task<List<MasterProduct>> GetActiveProductsAsync();
}

public interface IUserRepository : IRepository<SysUser>
{
    Task<SysUser?> GetUserWithRoleAsync(string userId);
    Task<List<SysUser>> GetUsersByDeptAsync(string deptId);
    Task<List<SysUser>> GetActiveUsersAsync();
}

public interface IRoleRepository : IRepository<SysRole>
{
    Task<SysRole?> GetRoleWithPermissionsAsync(string roleId);
}

public interface IDepartmentRepository : IRepository<SysDepartment>
{
    Task<List<SysDepartment>> GetDepartmentTreeAsync();
}

public interface ISignatureLevelRepository : IRepository<SysSignatureLevel>
{
    Task<SysSignatureLevel?> GetLevelAsync(string levelCode);
}

public interface IPermissionConfirmRepository : IRepository<SysPermissionConfirm>
{
    Task<List<SysPermissionConfirm>> GetByEmployeeAsync(string employeeId, int days = 7);
    Task<List<SysPermissionConfirm>> GetByOperationAsync(string operationType, DateTime? from = null, DateTime? to = null);
}
