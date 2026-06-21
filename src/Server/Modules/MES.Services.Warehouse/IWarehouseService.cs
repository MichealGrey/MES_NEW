using MES.Contracts.Common;
using MES.Contracts.Phase1;

namespace MES.Services.Warehouse;

public interface IWarehouseService
{
    Task<WarehouseReceiptResponse> ReceiveMaterialAsync(WarehouseReceiptRequest request);
    Task<PagedResult<InventoryResponse>> GetInventoryAsync(InventoryQuery query);
    Task<FifoRecommendResponse> GetFifoRecommendationAsync(string materialId, int requestedQty);
    Task<List<ExpiryWarningResponse>> GetExpiryWarningsAsync(int warningDays);
    Task<bool> LockBatchAsync(string batchId, string reason, string operatorId);
    Task<bool> UnlockBatchAsync(string batchId, string operatorId);
    Task<bool> AdjustInventoryAsync(InventoryAdjustRequest request);
}
