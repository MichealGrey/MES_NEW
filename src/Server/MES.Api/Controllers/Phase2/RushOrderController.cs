using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase2;
using MES.Services.Planning;

namespace MES.Api.Controllers.Phase2;

/// <summary>
/// 紧急订单管理 API
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
[Authorize]
public class RushOrderController : ControllerBase
{
    private readonly IRushOrderService _rushOrderService;
    private readonly ILogger<RushOrderController> _logger;

    public RushOrderController(IRushOrderService rushOrderService, ILogger<RushOrderController> logger)
    {
        _rushOrderService = rushOrderService;
        _logger = logger;
    }

    /// <summary>
    /// 创建紧急订单
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RushOrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RushOrderResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRushOrder([FromBody] CreateRushOrderRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.OrderId))
                return BadRequest(ApiResponse<RushOrderResponse>.Fail("订单ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.RushReason))
                return BadRequest(ApiResponse<RushOrderResponse>.Fail("加急原因不能为空"));

            var result = await _rushOrderService.CreateRushOrderAsync(request, GetOperatorId());
            return Ok(ApiResponse<RushOrderResponse>.Ok(result, "紧急订单创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建紧急订单时发生异常");
            return BadRequest(ApiResponse<RushOrderResponse>.Fail("创建紧急订单失败"));
        }
    }

    /// <summary>
    /// 查询紧急订单列表（分页）
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RushOrderResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRushOrders([FromQuery] RushOrderQuery query)
    {
        try
        {
            var result = await _rushOrderService.GetRushOrdersAsync(query);
            return Ok(ApiResponse<PagedResult<RushOrderResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询紧急订单列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<RushOrderResponse>>.Fail("查询紧急订单列表失败"));
        }
    }

    /// <summary>
    /// 获取紧急订单详情
    /// </summary>
    [HttpGet("{requestId}")]
    [ProducesResponseType(typeof(ApiResponse<RushOrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRushOrder(string requestId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(requestId))
                return BadRequest(ApiResponse<RushOrderResponse>.Fail("紧急订单ID不能为空"));

            var result = await _rushOrderService.GetRushOrderAsync(requestId);
            return Ok(ApiResponse<RushOrderResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<RushOrderResponse>.Fail("紧急订单不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取紧急订单详情时发生异常");
            return BadRequest(ApiResponse<RushOrderResponse>.Fail("获取紧急订单详情失败"));
        }
    }

    /// <summary>
    /// 分析插单影响
    /// </summary>
    [HttpGet("{requestId}/impact")]
    [ProducesResponseType(typeof(ApiResponse<List<RushOrderImpactResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AnalyzeImpact(string requestId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(requestId))
                return BadRequest(ApiResponse<List<RushOrderImpactResponse>>.Fail("紧急订单ID不能为空"));

            var result = await _rushOrderService.AnalyzeImpactAsync(requestId, GetOperatorId());
            return Ok(ApiResponse<List<RushOrderImpactResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分析插单影响时发生异常");
            return BadRequest(ApiResponse<List<RushOrderImpactResponse>>.Fail("分析插单影响失败"));
        }
    }

    /// <summary>
    /// 审批紧急订单
    /// </summary>
    [HttpPost("{requestId}/approve")]
    [ProducesResponseType(typeof(ApiResponse<RushOrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveRushOrder(string requestId, [FromBody] ApproveRushOrderRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(requestId))
                return BadRequest(ApiResponse<RushOrderResponse>.Fail("紧急订单ID不能为空"));

            var result = await _rushOrderService.ApproveRushOrderAsync(requestId, request, GetOperatorId());
            return Ok(ApiResponse<RushOrderResponse>.Ok(result, request.ApprovalResult == "Approved" ? "紧急订单已审批" : "紧急订单已驳回"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "审批紧急订单时发生异常");
            return BadRequest(ApiResponse<RushOrderResponse>.Fail("审批紧急订单失败"));
        }
    }

    /// <summary>
    /// 执行紧急插单
    /// </summary>
    [HttpPost("{requestId}/execute")]
    [ProducesResponseType(typeof(ApiResponse<RushOrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExecuteRushOrder(string requestId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(requestId))
                return BadRequest(ApiResponse<RushOrderResponse>.Fail("紧急订单ID不能为空"));

            var result = await _rushOrderService.ExecuteRushOrderAsync(requestId, GetOperatorId());
            return Ok(ApiResponse<RushOrderResponse>.Ok(result, "紧急插单已执行"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行紧急插单时发生异常");
            return BadRequest(ApiResponse<RushOrderResponse>.Fail("执行紧急插单失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
