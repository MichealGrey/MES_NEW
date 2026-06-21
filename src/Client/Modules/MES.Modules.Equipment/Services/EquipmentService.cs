using MES.Modules.Equipment.Models;

namespace MES.Modules.Equipment.Services;

public class EquipmentService : IEquipmentService
{
    private readonly List<EquipmentInfo> _equipment = [];
    private readonly List<PmScheduleItem> _pmSchedules = [];
    private readonly List<SparePartItem> _spareParts = [];
    private readonly List<FixtureItem> _fixtures = [];
    private readonly List<EquipmentHistoryRecord> _history = [];
    private readonly List<EquipmentPerformanceItem> _performance = [];
    private readonly List<EquipmentAlarmRecord> _alarms = [];

    public EquipmentService() => SeedData();

    private void SeedData()
    {
        _equipment.AddRange([
            new EquipmentInfo { Id = "EQ-001", EquipmentId = "WB-01", Name = "Wire Bonder #1", Type = "Wire Bonder", Area = "Wire Bonding", Status = "Running", CurrentLot = "LOT-2024-001", RecipeId = "RCP-WB-001", OEE = 87.5, Availability = 95.0, Performance = 92.0, Quality = 99.5, LastMaintenance = DateTime.Now.AddDays(-15), TotalRunHours = 12500, Manufacturer = "Kulicke & Soffa", InstallDate = new DateTime(2020, 6, 1) },
            new EquipmentInfo { Id = "EQ-002", EquipmentId = "WB-02", Name = "Wire Bonder #2", Type = "Wire Bonder", Area = "Wire Bonding", Status = "Idle", RecipeId = "RCP-WB-002", OEE = 82.3, Availability = 91.0, Performance = 89.5, Quality = 99.8, LastMaintenance = DateTime.Now.AddDays(-20), TotalRunHours = 10800, Manufacturer = "Kulicke & Soffa", InstallDate = new DateTime(2021, 3, 15) },
            new EquipmentInfo { Id = "EQ-003", EquipmentId = "DB-01", Name = "Die Bonder #1", Type = "Die Bonder", Area = "Die Attach", Status = "Running", CurrentLot = "LOT-2024-002", RecipeId = "RCP-DB-001", OEE = 91.2, Availability = 96.5, Performance = 94.0, Quality = 99.2, LastMaintenance = DateTime.Now.AddDays(-7), TotalRunHours = 8500, Manufacturer = "ASM", InstallDate = new DateTime(2019, 11, 20) },
            new EquipmentInfo { Id = "EQ-004", EquipmentId = "MP-01", Name = "Mold Press #1", Type = "Mold Press", Area = "Molding", Status = "Maintenance", RecipeId = "RCP-MP-001", OEE = 75.8, Availability = 88.0, Performance = 85.5, Quality = 99.0, LastMaintenance = DateTime.Now, TotalRunHours = 15200, Manufacturer = "Towa", InstallDate = new DateTime(2018, 4, 10) },
            new EquipmentInfo { Id = "EQ-005", EquipmentId = "MP-02", Name = "Mold Press #2", Type = "Mold Press", Area = "Molding", Status = "Running", CurrentLot = "LOT-2024-003", RecipeId = "RCP-MP-002", OEE = 88.9, Availability = 94.0, Performance = 93.5, Quality = 99.7, LastMaintenance = DateTime.Now.AddDays(-30), TotalRunHours = 14800, Manufacturer = "Towa", InstallDate = new DateTime(2019, 1, 5) },
            new EquipmentInfo { Id = "EQ-006", EquipmentId = "TH-01", Name = "Trim & Form #1", Type = "Trim & Form", Area = "Trim/Form", Status = "Idle", RecipeId = "RCP-TH-001", OEE = 85.1, Availability = 92.5, Performance = 91.0, Quality = 99.9, LastMaintenance = DateTime.Now.AddDays(-10), TotalRunHours = 9600, Manufacturer = "Shinkawa", InstallDate = new DateTime(2020, 9, 15) },
            new EquipmentInfo { Id = "EQ-007", EquipmentId = "WS-01", Name = "Wafer Saw #1", Type = "Wafer Saw", Area = "Dicing", Status = "Running", CurrentLot = "LOT-2024-004", RecipeId = "RCP-WS-001", OEE = 89.4, Availability = 95.5, Performance = 93.0, Quality = 99.4, LastMaintenance = DateTime.Now.AddDays(-12), TotalRunHours = 7200, Manufacturer = "Disco", InstallDate = new DateTime(2021, 7, 20) },
            new EquipmentInfo { Id = "EQ-008", EquipmentId = "PL-01", Name = "Plating Line #1", Type = "Plating", Area = "Plating", Status = "Running", CurrentLot = "LOT-2024-005", RecipeId = "RCP-PL-001", OEE = 83.6, Availability = 90.0, Performance = 91.5, Quality = 99.1, LastMaintenance = DateTime.Now.AddDays(-25), TotalRunHours = 11000, Manufacturer = "Rocky", InstallDate = new DateTime(2019, 5, 8) },
        ]);

        _pmSchedules.AddRange([
            new PmScheduleItem { Id = "PM-001", EquipmentId = "WB-01", EquipmentName = "Wire Bonder #1", PmType = "Monthly", ScheduledDate = DateTime.Now.AddDays(5), Status = "Pending", AssignedTo = "张工程师", Description = "月度保养 - 清洁焊接头、检查送丝机构" },
            new PmScheduleItem { Id = "PM-002", EquipmentId = "DB-01", EquipmentName = "Die Bonder #1", PmType = "Quarterly", ScheduledDate = DateTime.Now.AddDays(15), Status = "Pending", AssignedTo = "李工程师", Description = "季度保养 - 校准贴片精度、更换吸嘴" },
            new PmScheduleItem { Id = "PM-003", EquipmentId = "MP-01", EquipmentName = "Mold Press #1", PmType = "Weekly", ScheduledDate = DateTime.Now.AddDays(-2), CompletedDate = DateTime.Now.AddDays(-2), Status = "Completed", AssignedTo = "王工程师", Description = "周保养 - 清洁模具、检查液压系统" },
            new PmScheduleItem { Id = "PM-004", EquipmentId = "TH-01", EquipmentName = "Trim & Form #1", PmType = "Monthly", ScheduledDate = DateTime.Now.AddDays(10), Status = "Pending", AssignedTo = "赵工程师", Description = "月度保养 - 检查模具磨损、润滑导轨" },
            new PmScheduleItem { Id = "PM-005", EquipmentId = "WS-01", EquipmentName = "Wafer Saw #1", PmType = "Daily", ScheduledDate = DateTime.Now, Status = "InProgress", AssignedTo = "张工程师", Description = "日保养 - 清洁切割台面、检查刀片" },
        ]);

        _spareParts.AddRange([
            new SparePartItem { Id = "SP-001", PartName = "焊接Capillary", PartNo = "CAP-WB-001", EquipmentType = "Wire Bonder", StockQty = 50, MinQty = 10, MaxQty = 100, Supplier = "Gaiser", Location = "A-01-01", UnitPrice = 180.0 },
            new SparePartItem { Id = "SP-002", PartName = "Die Bonder吸嘴", PartNo = "NZZ-DB-001", EquipmentType = "Die Bonder", StockQty = 20, MinQty = 5, MaxQty = 50, Supplier = "ASM", Location = "A-02-01", UnitPrice = 350.0 },
            new SparePartItem { Id = "SP-003", PartName = "切割刀片", PartNo = "BLD-WS-001", EquipmentType = "Wafer Saw", StockQty = 8, MinQty = 10, MaxQty = 30, Supplier = "Disco", Location = "B-01-01", UnitPrice = 520.0 },
            new SparePartItem { Id = "SP-004", PartName = "成型模具", PartNo = "MLD-MP-001", EquipmentType = "Mold Press", StockQty = 3, MinQty = 2, MaxQty = 5, Supplier = "Towa", Location = "B-02-01", UnitPrice = 8500.0 },
            new SparePartItem { Id = "SP-005", PartName = "金线0.8mil", PartNo = "AUW-008", EquipmentType = "Wire Bonder", StockQty = 200, MinQty = 50, MaxQty = 500, Supplier = "Tanaka", Location = "C-01-01", UnitPrice = 45.0 },
            new SparePartItem { Id = "SP-006", PartName = "Trim模具", PartNo = "TRM-TH-001", EquipmentType = "Trim & Form", StockQty = 5, MinQty = 2, MaxQty = 10, Supplier = "Shinkawa", Location = "C-02-01", UnitPrice = 2800.0 },
        ]);

        _fixtures.AddRange([
            new FixtureItem { Id = "FX-001", FixtureNo = "MAG-QFN48-01", FixtureType = "Magazine", EquipmentType = "Wire Bonder", UseCount = 1250, MaxUseCount = 5000, Status = "Available", LastUsed = DateTime.Now.AddHours(-2), LastCalibration = DateTime.Now.AddDays(-30), Location = "FX-AREA-A" },
            new FixtureItem { Id = "FX-002", FixtureNo = "MAG-SOP8-01", FixtureType = "Magazine", EquipmentType = "Wire Bonder", UseCount = 4800, MaxUseCount = 5000, Status = "NearExpiry", LastUsed = DateTime.Now.AddHours(-5), LastCalibration = DateTime.Now.AddDays(-45), Location = "FX-AREA-A" },
            new FixtureItem { Id = "FX-003", FixtureNo = "LEAD-BGA256-01", FixtureType = "Leadframe", EquipmentType = "Die Bonder", UseCount = 800, MaxUseCount = 3000, Status = "Available", LastUsed = DateTime.Now.AddHours(-1), LastCalibration = DateTime.Now.AddDays(-15), Location = "FX-AREA-B" },
            new FixtureItem { Id = "FX-004", FixtureNo = "MOLD-QFN48-01", FixtureType = "Mold", EquipmentType = "Mold Press", UseCount = 8500, MaxUseCount = 10000, Status = "NearExpiry", LastUsed = DateTime.Now.AddHours(-3), LastCalibration = DateTime.Now.AddDays(-60), Location = "FX-AREA-C" },
        ]);

        _performance.AddRange([
            new EquipmentPerformanceItem { EquipmentId = "WB-01", EquipmentName = "Wire Bonder #1", OEE = 87.5, Availability = 95.0, Performance = 92.0, Quality = 99.5, TotalLots = 1250, CompletedLots = 1238, AvgCycleTime = 45.2, AlarmCount = 3, Uptime = 96.5 },
            new EquipmentPerformanceItem { EquipmentId = "WB-02", EquipmentName = "Wire Bonder #2", OEE = 82.3, Availability = 91.0, Performance = 89.5, Quality = 99.8, TotalLots = 980, CompletedLots = 972, AvgCycleTime = 48.5, AlarmCount = 8, Uptime = 92.0 },
            new EquipmentPerformanceItem { EquipmentId = "DB-01", EquipmentName = "Die Bonder #1", OEE = 91.2, Availability = 96.5, Performance = 94.0, Quality = 99.2, TotalLots = 1100, CompletedLots = 1085, AvgCycleTime = 32.1, AlarmCount = 2, Uptime = 97.0 },
            new EquipmentPerformanceItem { EquipmentId = "MP-01", EquipmentName = "Mold Press #1", OEE = 75.8, Availability = 88.0, Performance = 85.5, Quality = 99.0, TotalLots = 850, CompletedLots = 830, AvgCycleTime = 120.5, AlarmCount = 12, Uptime = 85.5 },
            new EquipmentPerformanceItem { EquipmentId = "MP-02", EquipmentName = "Mold Press #2", OEE = 88.9, Availability = 94.0, Performance = 93.5, Quality = 99.7, TotalLots = 920, CompletedLots = 910, AvgCycleTime = 118.2, AlarmCount = 4, Uptime = 94.5 },
            new EquipmentPerformanceItem { EquipmentId = "TH-01", EquipmentName = "Trim & Form #1", OEE = 85.1, Availability = 92.5, Performance = 91.0, Quality = 99.9, TotalLots = 1500, CompletedLots = 1488, AvgCycleTime = 15.8, AlarmCount = 1, Uptime = 95.0 },
            new EquipmentPerformanceItem { EquipmentId = "WS-01", EquipmentName = "Wafer Saw #1", OEE = 89.4, Availability = 95.5, Performance = 93.0, Quality = 99.4, TotalLots = 680, CompletedLots = 675, AvgCycleTime = 28.5, AlarmCount = 5, Uptime = 94.0 },
            new EquipmentPerformanceItem { EquipmentId = "PL-01", EquipmentName = "Plating Line #1", OEE = 83.6, Availability = 90.0, Performance = 91.5, Quality = 99.1, TotalLots = 750, CompletedLots = 740, AvgCycleTime = 180.0, AlarmCount = 6, Uptime = 91.0 },
        ]);

        _alarms.AddRange([
            new EquipmentAlarmRecord { Id = "EA-001", EquipmentId = "WB-01", AlarmCode = "WB-ERR-001", AlarmMessage = "Wire break detected", AlarmTime = DateTime.Now.AddHours(-2), Severity = "Warning", Status = "Cleared" },
            new EquipmentAlarmRecord { Id = "EA-002", EquipmentId = "MP-01", AlarmCode = "MP-ERR-003", AlarmMessage = "Temperature over limit", AlarmTime = DateTime.Now.AddHours(-1), Severity = "Error", Status = "Active" },
            new EquipmentAlarmRecord { Id = "EA-003", EquipmentId = "WS-01", AlarmCode = "WS-ERR-002", AlarmMessage = "Blade wear warning", AlarmTime = DateTime.Now.AddDays(-1), Severity = "Warning", Status = "Cleared" },
            new EquipmentAlarmRecord { Id = "EA-004", EquipmentId = "DB-01", AlarmCode = "DB-ERR-001", AlarmMessage = "Pick error", AlarmTime = DateTime.Now.AddHours(-8), Severity = "Error", Status = "Cleared" },
        ]);

        _history.AddRange([
            new EquipmentHistoryRecord { Id = "EH-001", EquipmentId = "WB-01", EventTime = DateTime.Now.AddHours(-1), EventType = "Start", Description = "开始加工LOT-2024-001", Operator = "张工" },
            new EquipmentHistoryRecord { Id = "EH-002", EquipmentId = "WB-01", EventTime = DateTime.Now.AddHours(-5), EventType = "Complete", Description = "完成加工LOT-2024-000", Operator = "张工" },
            new EquipmentHistoryRecord { Id = "EH-003", EquipmentId = "MP-01", EventTime = DateTime.Now, EventType = "Maintenance", Description = "执行月度保养", Operator = "王工" },
            new EquipmentHistoryRecord { Id = "EH-004", EquipmentId = "DB-01", EventTime = DateTime.Now.AddHours(-2), EventType = "Start", Description = "开始加工LOT-2024-002", Operator = "李工" },
        ]);
    }

    public async Task<List<EquipmentInfo>> GetAllEquipmentAsync() { await Task.Delay(50); return _equipment.ToList(); }
    public async Task<EquipmentInfo?> GetEquipmentAsync(string equipmentId) { await Task.Delay(50); return _equipment.FirstOrDefault(e => e.EquipmentId == equipmentId); }
    public async Task SaveEquipmentAsync(EquipmentInfo equipment) { await Task.Delay(50); var existing = _equipment.FirstOrDefault(e => e.Id == equipment.Id); if (existing is null) _equipment.Add(equipment); else _equipment[_equipment.IndexOf(existing)] = equipment; }
    public async Task DeleteEquipmentAsync(string equipmentId) { await Task.Delay(50); var item = _equipment.FirstOrDefault(e => e.Id == equipmentId); if (item is not null) _equipment.Remove(item); }
    public async Task<List<EquipmentHistoryRecord>> GetEquipmentHistoryAsync(string equipmentId) { await Task.Delay(50); return _history.Where(h => h.EquipmentId == equipmentId).ToList(); }
    public async Task<List<PmScheduleItem>> GetPmSchedulesAsync() { await Task.Delay(50); return _pmSchedules.ToList(); }
    public async Task SavePmScheduleAsync(PmScheduleItem item) { await Task.Delay(50); var existing = _pmSchedules.FirstOrDefault(p => p.Id == item.Id); if (existing is null) _pmSchedules.Add(item); else _pmSchedules[_pmSchedules.IndexOf(existing)] = item; }
    public async Task<List<SparePartItem>> GetSparePartsAsync() { await Task.Delay(50); return _spareParts.ToList(); }
    public async Task SaveSparePartAsync(SparePartItem item) { await Task.Delay(50); var existing = _spareParts.FirstOrDefault(s => s.Id == item.Id); if (existing is null) _spareParts.Add(item); else _spareParts[_spareParts.IndexOf(existing)] = item; }
    public async Task DeleteSparePartAsync(string partId) { await Task.Delay(50); var item = _spareParts.FirstOrDefault(s => s.Id == partId); if (item is not null) _spareParts.Remove(item); }
    public async Task<List<FixtureItem>> GetFixturesAsync() { await Task.Delay(50); return _fixtures.ToList(); }
    public async Task SaveFixtureAsync(FixtureItem item) { await Task.Delay(50); var existing = _fixtures.FirstOrDefault(f => f.Id == item.Id); if (existing is null) _fixtures.Add(item); else _fixtures[_fixtures.IndexOf(existing)] = item; }
    public async Task<List<EquipmentPerformanceItem>> GetPerformanceAsync() { await Task.Delay(50); return _performance.ToList(); }
    public async Task<List<EquipmentAlarmRecord>> GetEquipmentAlarmsAsync(string equipmentId) { await Task.Delay(50); return _alarms.Where(a => a.EquipmentId == equipmentId).ToList(); }
}
