using MES.Contracts.Common;
using MES.Contracts.Phase1;

namespace MES.Services.Quality;

public interface IFqcOqcService
{
    Task<FqcTaskResponse> CreateFqcTaskAsync(string lotId, string workOrderId);
    Task<PagedResult<FqcTaskResponse>> GetFqcTasksAsync(FqcTaskQuery query);
    Task<FqcTaskResponse> ExecuteFqcAsync(string taskId, ExecuteFqcRequest request);
    Task<OqcTaskResponse> CreateOqcTaskAsync(string lotId, string shipmentId);
    Task<PagedResult<OqcTaskResponse>> GetOqcTasksAsync(OqcTaskQuery query);
    Task<OqcTaskResponse> ExecuteOqcAsync(string taskId, ExecuteOqcRequest request);
    Task<MslCheckResult> CheckMslForShipmentAsync(OqcMslCheckRequest request);
}
