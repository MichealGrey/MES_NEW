using MES.Modules.Schedule.Models;

namespace MES.Modules.Schedule.Services;

public interface IScheduleService
{
    Task<List<DispatchQueueItem>> GetDispatchQueueAsync();
    Task<List<DispatchRuleItem>> GetDispatchRulesAsync();
    Task<List<CapacityItem>> GetCapacityAsync();
    Task<List<WorkOrderScheduleItem>> GetWorkOrderSchedulesAsync();
    Task<List<MrpItem>> GetMrpDataAsync();
    Task<List<DeliveryRecord>> GetDeliveryRecordsAsync();
    Task<List<CapacityBalanceItem>> GetCapacityBalanceAsync();
}

public class ScheduleService : IScheduleService
{
    private readonly List<DispatchQueueItem> _dispatchQueue = [];
    private readonly List<DispatchRuleItem> _dispatchRules = [];
    private readonly List<CapacityItem> _capacity = [];
    private readonly List<WorkOrderScheduleItem> _woSchedules = [];
    private readonly List<MrpItem> _mrpData = [];
    private readonly List<DeliveryRecord> _deliveries = [];
    private readonly List<CapacityBalanceItem> _capacityBalance = [];

    public ScheduleService() => SeedData();

    private void SeedData()
    {
        _dispatchQueue.AddRange([
            new DispatchQueueItem { EquipmentId = "DB-01", LotId = "LOT-2026001", Product = "PRD-001", Priority = 1, Step = "DIE_ATTACH", RecipeId = "RCP-001", Customer = "客户A", DueDate = DateTime.Now.AddDays(5) },
            new DispatchQueueItem { EquipmentId = "DB-01", LotId = "LOT-2026003", Product = "PRD-002", Priority = 3, Step = "DIE_ATTACH", RecipeId = "RCP-002", Customer = "客户B", DueDate = DateTime.Now.AddDays(10) },
            new DispatchQueueItem { EquipmentId = "WB-01", LotId = "LOT-2026002", Product = "PRD-001", Priority = 1, Step = "WIRE_BOND", RecipeId = "RCP-003", Customer = "客户A", DueDate = DateTime.Now.AddDays(7) },
            new DispatchQueueItem { EquipmentId = "WB-01", LotId = "LOT-2026005", Product = "PRD-003", Priority = 2, Step = "WIRE_BOND", RecipeId = "RCP-004", Customer = "客户C", DueDate = DateTime.Now.AddDays(3) },
            new DispatchQueueItem { EquipmentId = "MP-01", LotId = "LOT-2026004", Product = "PRD-001", Priority = 1, Step = "MOLDING", RecipeId = "RCP-005", Customer = "客户A", DueDate = DateTime.Now.AddDays(12) },
            new DispatchQueueItem { EquipmentId = "TH-01", LotId = "LOT-2026008", Product = "PRD-001", Priority = 4, Step = "TEST_CP", RecipeId = "RCP-008", Customer = "客户A", DueDate = DateTime.Now.AddDays(15) },
            new DispatchQueueItem { EquipmentId = "TF-01", LotId = "LOT-2026007", Product = "PRD-004", Priority = 1, Step = "TRIM_FORM", RecipeId = "RCP-007", Customer = "客户D", DueDate = DateTime.Now.AddDays(2) },
        ]);

        _dispatchRules.AddRange([
            new DispatchRuleItem { Id = "DR-001", RuleName = "交期优先", Description = "按交货日期升序排列", IsEnabled = true, Priority = 1 },
            new DispatchRuleItem { Id = "DR-002", RuleName = "客户优先级", Description = "按客户等级排列", IsEnabled = true, Priority = 2 },
            new DispatchRuleItem { Id = "DR-003", RuleName = "产品族优先", Description = "相同产品族批量加工", IsEnabled = false, Priority = 3 },
            new DispatchRuleItem { Id = "DR-004", RuleName = "最短处理时间", Description = "优先处理时间最短的批次", IsEnabled = false, Priority = 4 },
        ]);

        _capacity.AddRange([
            new CapacityItem { EquipmentId = "WB-01", EquipmentName = "Wire Bonder #1", Type = "Wire Bonder", PlannedUtilization = 85.0, ActualUtilization = 87.5, WipCount = 5 },
            new CapacityItem { EquipmentId = "WB-02", EquipmentName = "Wire Bonder #2", Type = "Wire Bonder", PlannedUtilization = 80.0, ActualUtilization = 82.3, WipCount = 3 },
            new CapacityItem { EquipmentId = "DB-01", EquipmentName = "Die Bonder #1", Type = "Die Bonder", PlannedUtilization = 90.0, ActualUtilization = 91.2, WipCount = 4 },
            new CapacityItem { EquipmentId = "MP-01", EquipmentName = "Mold Press #1", Type = "Mold Press", PlannedUtilization = 85.0, ActualUtilization = 75.8, WipCount = 2 },
            new CapacityItem { EquipmentId = "MP-02", EquipmentName = "Mold Press #2", Type = "Mold Press", PlannedUtilization = 85.0, ActualUtilization = 88.9, WipCount = 6 },
            new CapacityItem { EquipmentId = "TH-01", EquipmentName = "Trim & Form #1", Type = "Trim & Form", PlannedUtilization = 80.0, ActualUtilization = 85.1, WipCount = 8 },
            new CapacityItem { EquipmentId = "WS-01", EquipmentName = "Wafer Saw #1", Type = "Wafer Saw", PlannedUtilization = 75.0, ActualUtilization = 89.4, WipCount = 3 },
        ]);

        _woSchedules.AddRange([
            new WorkOrderScheduleItem { WorkOrderNo = "WO-2024-001", ProductName = "QFN48-001", PlanQty = 10000, CompletedQty = 7500, StartDate = DateTime.Now.AddDays(-5), PlanEndDate = DateTime.Now.AddDays(5), Status = "InProgress", RiskLevel = "Low" },
            new WorkOrderScheduleItem { WorkOrderNo = "WO-2024-002", ProductName = "SOP8-001", PlanQty = 20000, CompletedQty = 5000, StartDate = DateTime.Now.AddDays(-3), PlanEndDate = DateTime.Now.AddDays(8), Status = "InProgress", RiskLevel = "Medium" },
            new WorkOrderScheduleItem { WorkOrderNo = "WO-2024-003", ProductName = "BGA256-001", PlanQty = 5000, CompletedQty = 0, StartDate = DateTime.Now.AddDays(2), PlanEndDate = DateTime.Now.AddDays(12), Status = "NotStarted", RiskLevel = "Low" },
            new WorkOrderScheduleItem { WorkOrderNo = "WO-2024-004", ProductName = "LQFP128-001", PlanQty = 8000, CompletedQty = 6000, StartDate = DateTime.Now.AddDays(-10), PlanEndDate = DateTime.Now.AddDays(-1), Status = "Delayed", RiskLevel = "High" },
        ]);

        _mrpData.AddRange([
            new MrpItem { MaterialNo = "MAT-WF-001", MaterialName = "8寸硅片", RequiredQty = 500, AvailableQty = 500, ShortageQty = 0, Supplier = "信越化学", LeadTime = 14 },
            new MrpItem { MaterialNo = "MAT-AU-001", MaterialName = "金线0.8mil", RequiredQty = 200, AvailableQty = 200, ShortageQty = 0, Supplier = "田中贵金属", LeadTime = 21 },
            new MrpItem { MaterialNo = "MAT-EP-001", MaterialName = "环氧树脂", RequiredQty = 50, AvailableQty = 8, ShortageQty = 42, Supplier = "Namics", LeadTime = 30 },
            new MrpItem { MaterialNo = "MAT-LF-001", MaterialName = "引线框架-QFN48", RequiredQty = 15000, AvailableQty = 5000, ShortageQty = 10000, Supplier = "三井高科技", LeadTime = 7 },
        ]);

        _deliveries.AddRange([
            new DeliveryRecord { Id = "DEL-001", WorkOrderNo = "WO-2024-001", CustomerName = "客户A", ProductName = "QFN48", PlanQty = 10000, DeliverQty = 0, PlanDeliveryDate = DateTime.Now.AddDays(5), Status = "Pending" },
            new DeliveryRecord { Id = "DEL-002", WorkOrderNo = "WO-2024-002", CustomerName = "客户B", ProductName = "SOP8", PlanQty = 20000, DeliverQty = 0, PlanDeliveryDate = DateTime.Now.AddDays(8), Status = "Pending" },
            new DeliveryRecord { Id = "DEL-003", WorkOrderNo = "WO-2024-003", CustomerName = "客户C", ProductName = "BGA256", PlanQty = 5000, DeliverQty = 0, PlanDeliveryDate = DateTime.Now.AddDays(12), Status = "Pending" },
        ]);

        _capacityBalance.AddRange([
            new CapacityBalanceItem { ProcessName = "Die Attach", DemandHours = 120, AvailableHours = 160, BalanceHours = 40, Utilization = 75.0 },
            new CapacityBalanceItem { ProcessName = "Wire Bond", DemandHours = 200, AvailableHours = 180, BalanceHours = -20, Utilization = 111.1 },
            new CapacityBalanceItem { ProcessName = "Molding", DemandHours = 150, AvailableHours = 160, BalanceHours = 10, Utilization = 93.8 },
            new CapacityBalanceItem { ProcessName = "Trim/Form", DemandHours = 80, AvailableHours = 120, BalanceHours = 40, Utilization = 66.7 },
            new CapacityBalanceItem { ProcessName = "Test", DemandHours = 100, AvailableHours = 120, BalanceHours = 20, Utilization = 83.3 },
        ]);
    }

    public async Task<List<DispatchQueueItem>> GetDispatchQueueAsync() { await Task.Delay(50); return _dispatchQueue.OrderBy(x => x.Priority).ToList(); }
    public async Task<List<DispatchRuleItem>> GetDispatchRulesAsync() { await Task.Delay(50); return _dispatchRules.OrderBy(x => x.Priority).ToList(); }
    public async Task<List<CapacityItem>> GetCapacityAsync() { await Task.Delay(50); return _capacity.ToList(); }
    public async Task<List<WorkOrderScheduleItem>> GetWorkOrderSchedulesAsync() { await Task.Delay(50); return _woSchedules.ToList(); }
    public async Task<List<MrpItem>> GetMrpDataAsync() { await Task.Delay(50); return _mrpData.ToList(); }
    public async Task<List<DeliveryRecord>> GetDeliveryRecordsAsync() { await Task.Delay(50); return _deliveries.ToList(); }
    public async Task<List<CapacityBalanceItem>> GetCapacityBalanceAsync() { await Task.Delay(50); return _capacityBalance.ToList(); }
}
