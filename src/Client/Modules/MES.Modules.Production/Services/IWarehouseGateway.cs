using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IWarehouseGateway
{
    Task<bool> IsMaterialReadyAsync(string lotId, string stepCode);
    Task RecordMaterialConsumeAsync(MaterialConsume consume);
    Task<List<string>> GetRequiredMaterialsAsync(string stepCode);
}
