using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IReworkService
{
    /// <summary>
    /// 创建重工批次
    /// </summary>
    Task<ReworkRecord> CreateReworkLotAsync(string lotId, string reworkRouteId,
        string fromStepCode, string targetStepCode, int reworkQty, string reason,
        string operatorId, string operatorName);

    /// <summary>
    /// 完成重工，返回原路线
    /// </summary>
    Task CompleteReworkAsync(string reworkLotId, string operatorId, string operatorName);

    /// <summary>
    /// 查询重工记录
    /// </summary>
    Task<List<ReworkRecord>> GetReworkRecordsAsync(string lotId);
}
