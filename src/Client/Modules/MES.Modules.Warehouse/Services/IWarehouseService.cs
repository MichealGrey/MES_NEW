using MES.Modules.Warehouse.Models;

namespace MES.Modules.Warehouse.Services;

public interface IWarehouseService
{
    Task<List<FoupItem>> GetFoupsAsync();
    Task SaveFoupAsync(FoupItem item);
    Task<List<MaterialItem>> GetMaterialsAsync();
    Task SaveMaterialAsync(MaterialItem item);
    Task DeleteMaterialAsync(string materialId);
    Task<List<ReticleItem>> GetReticlesAsync();
    Task SaveReticleAsync(ReticleItem item);
    Task<List<StockerItem>> GetStockersAsync();
    Task SaveStockerAsync(StockerItem item);
    Task<List<InboundRecord>> GetInboundRecordsAsync();
    Task SaveInboundRecordAsync(InboundRecord record);
    Task<List<OutboundRecord>> GetOutboundRecordsAsync();
    Task SaveOutboundRecordAsync(OutboundRecord record);
    Task<List<InventoryRecord>> GetInventoryRecordsAsync();
}
