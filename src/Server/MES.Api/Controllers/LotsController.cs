using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Production;
using MES.Contracts.Common;
using MES.Services.Production;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LotsController : ControllerBase
{
    private readonly ILotService _lotService;
    private readonly ILogger<LotsController> _logger;

    public LotsController(ILotService lotService, ILogger<LotsController> logger)
    {
        _lotService = lotService;
        _logger = logger;
    }

    /// <summary>
    /// 获取批次列表（分页）
    /// GET /api/lots?pageIndex=1&pageSize=20&orderId=&status=&keyword=
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<LotDto>>> GetLots(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? orderId = null,
        [FromQuery] string? keyword = null)
    {
        var result = await _lotService.GetPagedAsync(pageIndex, pageSize, status, orderId, keyword);
        return ApiResponse<PagedResult<LotDto>>.Ok(result);
    }

    /// <summary>
    /// 获取批次详情（含工序和流转记录）
    /// GET /api/lots/{lotId}
    /// </summary>
    [HttpGet("{lotId}")]
    public async Task<ApiResponse<LotDetailResponse?>> GetLotDetail(string lotId)
    {
        var result = await _lotService.GetLotDetailAsync(lotId);
        return result != null
            ? ApiResponse<LotDetailResponse?>.Ok(result)
            : ApiResponse<LotDetailResponse?>.Fail("批次未找到");
    }

    /// <summary>
    /// 获取批次完整追踪链
    /// GET /api/lots/{lotId}/tracking
    /// </summary>
    [HttpGet("{lotId}/tracking")]
    public async Task<ApiResponse<List<LotTrackRecord>>> GetLotTracking(string lotId)
    {
        var result = await _lotService.GetLotTrackingAsync(lotId);
        return ApiResponse<List<LotTrackRecord>>.Ok(result);
    }

    /// <summary>
    /// 获取批次统计信息（各状态数量）
    /// GET /api/lots/stats
    /// </summary>
    [HttpGet("stats")]
    public async Task<ApiResponse<LotStatsResponse>> GetLotStats()
    {
        var result = await _lotService.GetLotStatsAsync();
        return ApiResponse<LotStatsResponse>.Ok(result);
    }

    /// <summary>
    /// 获取工单下的所有批次
    /// GET /api/lots/order/{orderId}
    /// </summary>
    [HttpGet("order/{orderId}")]
    public async Task<ApiResponse<List<LotDto>>> GetByOrder(string orderId)
    {
        var result = await _lotService.GetByOrderIdAsync(orderId);
        return ApiResponse<List<LotDto>>.Ok(result);
    }

    /// <summary>
    /// 获取批次工序列表
    /// GET /api/lots/{lotId}/steps
    /// </summary>
    [HttpGet("{lotId}/steps")]
    public async Task<ApiResponse<List<LotStepDto>>> GetLotSteps(string lotId)
    {
        var result = await _lotService.GetLotStepsAsync(lotId);
        return ApiResponse<List<LotStepDto>>.Ok(result);
    }

    /// <summary>
    /// 获取仪表板统计（生产中批次数量）
    /// GET /api/lots/dashboard/stats
    /// </summary>
    [HttpGet("dashboard/stats")]
    public async Task<ApiResponse<int>> GetDashboardStats()
    {
        var result = await _lotService.GetDashboardStatsAsync();
        return ApiResponse<int>.Ok(result);
    }
}
