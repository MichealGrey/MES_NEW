using MES.Contracts.Common;
using MES.Contracts.Phase1;

namespace MES.Services.Warehouse;

public interface IFinishedGoodsService
{
    Task<WarehouseReceiptResponse> ReceiveFinishedGoodsAsync(FinishedGoodsReceiptRequest request);
    Task<PagedResult<FinishedGoodsInventoryResponse>> GetInventoryAsync(FinishedGoodsQuery query);
    Task<ShipmentResponse> ShipFinishedGoodsAsync(ShipFinishedGoodsRequest request);
    Task<PagedResult<ShipmentResponse>> GetShipmentsAsync(FinishedGoodsQuery query);
}
