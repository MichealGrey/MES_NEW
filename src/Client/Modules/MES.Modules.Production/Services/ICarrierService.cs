using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface ICarrierService
{
    /// <summary>
    /// 绑定载具到批次
    /// </summary>
    Task<LotCarrierBinding> BindCarrierAsync(string lotId, string carrierId,
        string carrierType, string stepCode, int stepSeq, string operatorId);

    /// <summary>
    /// 解绑载具
    /// </summary>
    Task UnbindCarrierAsync(string lotId, string operatorId);

    /// <summary>
    /// 转移载具（从一批次到另一批次）
    /// </summary>
    Task<LotCarrierBinding> TransferCarrierAsync(string fromLotId, string toLotId,
        string stepCode, int stepSeq, string operatorId);

    /// <summary>
    /// 查询载具历史
    /// </summary>
    Task<List<LotCarrierBinding>> GetCarrierHistoryAsync(string lotId);

    /// <summary>
    /// 查询载具当前绑定
    /// </summary>
    Task<LotCarrierBinding?> GetCurrentBindingAsync(string carrierId);
}
