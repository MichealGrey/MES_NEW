using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IScrapService
{
    /// <summary>
    /// 记录报废
    /// </summary>
    Task<ScrapRecord> RecordScrapAsync(string lotId, int scrapQty, string reason,
        string reasonCode, string operatorId, string operatorName);

    /// <summary>
    /// 检查是否需要审批
    /// </summary>
    bool RequiresApproval(int scrapQty, int totalQty);

    /// <summary>
    /// 查询报废记录
    /// </summary>
    Task<List<ScrapRecord>> GetScrapRecordsAsync(string lotId);
}
