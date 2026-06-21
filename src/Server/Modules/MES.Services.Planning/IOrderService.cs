using MES.Contracts.Common;
using MES.Contracts.Phase2;

namespace MES.Services.Planning;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, string operatorId);
    Task<PagedResult<OrderResponse>> GetOrdersAsync(OrderQuery query);
    Task<OrderResponse> GetOrderAsync(string orderId);
    Task<OrderReviewResponse> StartReviewAsync(string orderId, StartOrderReviewRequest request, string operatorId);
    Task<OrderReviewResponse> VoteReviewAsync(string reviewId, string role, VoteReviewRequest request, string operatorId);
    Task<OrderReviewResponse> GetReviewStatusAsync(string orderId);
    Task<OrderResponse> CompleteReviewAsync(string reviewId, string operatorId);
    Task<PagedResult<OrderVersionResponse>> GetOrderVersionsAsync(string orderId, int pageIndex, int pageSize);
}
