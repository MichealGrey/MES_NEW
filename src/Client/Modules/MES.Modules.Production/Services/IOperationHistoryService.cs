using MES.Modules.Production.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Production.Services;

public interface IOperationHistoryService
{
    Task WriteAsync(string lotId, string orderId, string operationType, string stepName, int stepSeq,
        string? equipmentId, string? recipeId, string? carrierId,
        int? beforeQty, int? afterQty, string operatorId, string operatorName,
        string workstation, string? remark);
    Task<List<OperationRecord>> GetByLotAsync(string lotId);
}
