using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class EquipmentGateway : IEquipmentGateway
{
    private readonly IMasterDataService _masterData;

    public EquipmentGateway(IMasterDataService masterData)
    {
        _masterData = masterData;
    }

    public async Task<EquipmentCheckResult> CheckEquipmentAsync(string equipmentId, string stepCode)
    {
        var result = new EquipmentCheckResult { IsAllowed = true };

        var equipment = await _masterData.GetEquipmentAsync(equipmentId);
        if (equipment is null)
        {
            result.IsAllowed = false;
            result.Reason = $"设备 {equipmentId} 不存在";
            return result;
        }

        result.EquipmentStatus = equipment.Status;

        if (!await IsEquipmentAvailableAsync(equipmentId))
        {
            result.IsAllowed = false;
            result.Reason = $"设备 {equipmentId} 状态为 {equipment.Status}，不允许进站";
            return result;
        }

        return result;
    }

    public async Task<bool> IsEquipmentAvailableAsync(string equipmentId)
    {
        var equipment = await _masterData.GetEquipmentAsync(equipmentId);
        if (equipment is null) return false;
        return equipment.Status is "Available" or "Running";
    }

    public async Task<bool> IsEquipmentInGroupAsync(string equipmentId, string equipmentGroup)
    {
        var equipment = await _masterData.GetEquipmentAsync(equipmentId);
        if (equipment is null) return false;
        return equipment.EquipmentGroup == equipmentGroup;
    }
}
