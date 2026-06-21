using MES.Modules.Equipment.Models;

namespace MES.Modules.Equipment.Services;

public interface IEquipmentService
{
    Task<List<EquipmentInfo>> GetAllEquipmentAsync();
    Task<EquipmentInfo?> GetEquipmentAsync(string equipmentId);
    Task SaveEquipmentAsync(EquipmentInfo equipment);
    Task DeleteEquipmentAsync(string equipmentId);
    Task<List<EquipmentHistoryRecord>> GetEquipmentHistoryAsync(string equipmentId);
    Task<List<PmScheduleItem>> GetPmSchedulesAsync();
    Task SavePmScheduleAsync(PmScheduleItem item);
    Task<List<SparePartItem>> GetSparePartsAsync();
    Task SaveSparePartAsync(SparePartItem item);
    Task DeleteSparePartAsync(string partId);
    Task<List<FixtureItem>> GetFixturesAsync();
    Task SaveFixtureAsync(FixtureItem item);
    Task<List<EquipmentPerformanceItem>> GetPerformanceAsync();
    Task<List<EquipmentAlarmRecord>> GetEquipmentAlarmsAsync(string equipmentId);
}
