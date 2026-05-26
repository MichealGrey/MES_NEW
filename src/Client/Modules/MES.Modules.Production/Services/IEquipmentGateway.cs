using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IEquipmentGateway
{
    Task<EquipmentCheckResult> CheckEquipmentAsync(string equipmentId, string stepCode);
    Task<bool> IsEquipmentAvailableAsync(string equipmentId);
    Task<bool> IsEquipmentInGroupAsync(string equipmentId, string equipmentGroup);
}
