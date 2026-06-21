using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase2;
using MES.Services.Planning;

namespace MES.Api.Controllers.Phase2;

/// <summary>
/// 订单管理 API
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// 创建销售订单
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CustomerId))
                return BadRequest(ApiResponse<OrderResponse>.Fail("客户ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.ProductId))
                return BadRequest(ApiResponse<OrderResponse>.Fail("产品ID不能为空"));
            if (request.Quantity <= 0)
                return BadRequest(ApiResponse<OrderResponse>.Fail("订单数量必须大于0"));
            if (request.DeliveryDate <= DateTime.UtcNow.Date)
                return BadRequest(ApiResponse<OrderResponse>.Fail("交期必须大于当前日期"));

            var result = await _orderService.CreateOrderAsync(request, GetOperatorId());
            return Ok(ApiResponse<OrderResponse>.Ok(result, "订单创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建订单时发生异常");
            return BadRequest(ApiResponse<OrderResponse>.Fail("创建订单失败"));
        }
    }

    /// <summary>
    /// 查询订单列表（分页）
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders([FromQuery] OrderQuery query)
    {
        try
        {
            var result = await _orderService.GetOrdersAsync(query);
            return Ok(ApiResponse<PagedResult<OrderResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询订单列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<OrderResponse>>.Fail("查询订单列表失败"));
        }
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    [HttpGet("{orderId}")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(string orderId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return BadRequest(ApiResponse<OrderResponse>.Fail("订单ID不能为空"));

            var result = await _orderService.GetOrderAsync(orderId);
            return Ok(ApiResponse<OrderResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<OrderResponse>.Fail("订单不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取订单详情时发生异常");
            return BadRequest(ApiResponse<OrderResponse>.Fail("获取订单详情失败"));
        }
    }

    /// <summary>
    /// 发起订单评审
    /// </summary>
    [HttpPost("{orderId}/review")]
    [ProducesResponseType(typeof(ApiResponse<OrderReviewResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> StartReview(string orderId, [FromBody] StartOrderReviewRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return BadRequest(ApiResponse<OrderReviewResponse>.Fail("订单ID不能为空"));

            var result = await _orderService.StartReviewAsync(orderId, request, GetOperatorId());
            return Ok(ApiResponse<OrderReviewResponse>.Ok(result, "订单评审已发起"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发起订单评审时发生异常");
            return BadRequest(ApiResponse<OrderReviewResponse>.Fail("发起订单评审失败"));
        }
    }

    /// <summary>
    /// 评审投票
    /// </summary>
    [HttpPost("review/{reviewId}/vote/{role}")]
    [ProducesResponseType(typeof(ApiResponse<OrderReviewResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VoteReview(string reviewId, string role, [FromBody] VoteReviewRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(reviewId))
                return BadRequest(ApiResponse<OrderReviewResponse>.Fail("评审ID不能为空"));
            if (string.IsNullOrWhiteSpace(role))
                return BadRequest(ApiResponse<OrderReviewResponse>.Fail("角色不能为空"));

            var result = await _orderService.VoteReviewAsync(reviewId, role, request, GetOperatorId());
            return Ok(ApiResponse<OrderReviewResponse>.Ok(result, "投票成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "评审投票时发生异常");
            return BadRequest(ApiResponse<OrderReviewResponse>.Fail("投票失败"));
        }
    }

    /// <summary>
    /// 获取订单评审状态
    /// </summary>
    [HttpGet("{orderId}/review-status")]
    [ProducesResponseType(typeof(ApiResponse<OrderReviewResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReviewStatus(string orderId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return BadRequest(ApiResponse<OrderReviewResponse>.Fail("订单ID不能为空"));

            var result = await _orderService.GetReviewStatusAsync(orderId);
            return Ok(ApiResponse<OrderReviewResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取评审状态时发生异常");
            return BadRequest(ApiResponse<OrderReviewResponse>.Fail("获取评审状态失败"));
        }
    }

    /// <summary>
    /// 完成订单评审
    /// </summary>
    [HttpPost("review/{reviewId}/complete")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CompleteReview(string reviewId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(reviewId))
                return BadRequest(ApiResponse<OrderResponse>.Fail("评审ID不能为空"));

            var result = await _orderService.CompleteReviewAsync(reviewId, GetOperatorId());
            return Ok(ApiResponse<OrderResponse>.Ok(result, "订单评审已完成"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "完成订单评审时发生异常");
            return BadRequest(ApiResponse<OrderResponse>.Fail("完成订单评审失败"));
        }
    }

    /// <summary>
    /// 获取订单版本历史
    /// </summary>
    [HttpGet("{orderId}/versions")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrderVersionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrderVersions(string orderId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return BadRequest(ApiResponse<PagedResult<OrderVersionResponse>>.Fail("订单ID不能为空"));

            var result = await _orderService.GetOrderVersionsAsync(orderId, pageIndex, pageSize);
            return Ok(ApiResponse<PagedResult<OrderVersionResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取订单版本历史时发生异常");
            return BadRequest(ApiResponse<PagedResult<OrderVersionResponse>>.Fail("获取订单版本历史失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
