using MES.Domain.Production;
using MES.Infrastructure.Persistence.Entities;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IProductionDataService
{
    Task EnsureSeededAsync();

    // WorkOrder CRUD
    Task<List<WorkOrderInfo>> GetAllWorkOrdersAsync();
    Task<WorkOrderInfo?> GetWorkOrderAsync(string orderId);
    Task SaveWorkOrderAsync(WorkOrderInfo workOrder);
    Task DeleteWorkOrderAsync(string orderId);
    Task UpdateWorkOrderStatusAsync(string orderId, ProcessStatus status);
    Task HoldWorkOrderAsync(string orderId, string reason, string? remark = null);
    Task ReleaseHoldWorkOrderAsync(string orderId);

    // Lot queries
    Task<List<LotInfo>> GetAllLotsAsync();
    Task<List<LotInfo>> GetAllHoldLotsAsync();
    Task<List<LotInfo>> GetLotsByStageAsync(string processStage);
    Task HoldLotAsync(LotInfo lot);
    Task ReleaseLotAsync(string lotId);
    Task BatchReleaseAsync();

    // Archive queries
    Task<List<ProdLotArchive>> GetArchivedLotsAsync();
    Task<List<ProdLotArchive>> GetArchivedLotsByStageAsync(string processStage);
}
