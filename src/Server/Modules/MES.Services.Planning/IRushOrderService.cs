using MES.Contracts.Common;
using MES.Contracts.Phase2;

namespace MES.Services.Planning;

public interface IRushOrderService
{
    Task<RushOrderResponse> CreateRushOrderAsync(CreateRushOrderRequest request, string operatorId);
    Task<PagedResult<RushOrderResponse>> GetRushOrdersAsync(RushOrderQuery query);
    Task<RushOrderResponse> GetRushOrderAsync(string requestId);
    Task<List<RushOrderImpactResponse>> AnalyzeImpactAsync(string requestId, string operatorId);
    Task<RushOrderResponse> ApproveRushOrderAsync(string requestId, ApproveRushOrderRequest request, string operatorId);
    Task<RushOrderResponse> ExecuteRushOrderAsync(string requestId, string operatorId);
}
