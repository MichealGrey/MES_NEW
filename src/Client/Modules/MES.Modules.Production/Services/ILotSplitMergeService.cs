using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface ILotSplitMergeService
{
    /// <summary>
    /// 拆批：从母批次拆出一个子批次
    /// </summary>
    Task<LotSplitRecord> SplitLotAsync(string motherLotId, int splitQty,
        string splitReason, string splitType, string operatorId, string operatorName);

    /// <summary>
    /// 等级拆分：按品质等级拆分为 A/B/C 级
    /// </summary>
    Task<List<LotSplitRecord>> GradeSplitAsync(string lotId,
        Dictionary<string, int> gradeQtyMap, string operatorId, string operatorName);

    /// <summary>
    /// 合批：将多个子批次合并到一个目标批次
    /// </summary>
    Task<LotMergeRecord> MergeLotsAsync(string targetLotId, List<string> sourceLotIds,
        string mergeReason, string operatorId, string operatorName);

    /// <summary>
    /// 查询子批次列表
    /// </summary>
    Task<List<LotInfo>> GetChildLotsAsync(string motherLotId);

    /// <summary>
    /// 查询拆批记录
    /// </summary>
    Task<List<LotSplitRecord>> GetSplitRecordsAsync(string lotId);

    /// <summary>
    /// 查询合批记录
    /// </summary>
    Task<List<LotMergeRecord>> GetMergeRecordsAsync(string lotId);
}
