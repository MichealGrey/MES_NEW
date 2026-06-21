using MES.Domain.Production;
using MES.Modules.Order.Models;

namespace MES.Modules.Order.Services;

public interface IProductionDataService
{
    Task EnsureSeededAsync();

    Task<List<WorkOrderInfo>> GetAllWorkOrdersAsync();
    Task<WorkOrderInfo?> GetWorkOrderAsync(string orderId);
    Task SaveWorkOrderAsync(WorkOrderInfo workOrder);
    Task DeleteWorkOrderAsync(string orderId);
    Task UpdateWorkOrderStatusAsync(string orderId, ProcessStatus status);
    Task HoldWorkOrderAsync(string orderId, string reason, string? remark = null);
    Task ReleaseHoldWorkOrderAsync(string orderId);
}
