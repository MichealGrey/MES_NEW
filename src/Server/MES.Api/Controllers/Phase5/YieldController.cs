using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Services.Analytics;

namespace MES.Api.Controllers.Phase5;

/// <summary>
/// 良率分析 API
/// </summary>
[ApiController]
[Route("api/v5/[controller]")]
[Authorize]
public class YieldController : ControllerBase
{
    private readonly IYieldService _yieldService;
    private readonly ILogger<YieldController> _logger;

    public YieldController(IYieldService yieldService, ILogger<YieldController> logger)
    {
        _yieldService = yieldService;
        _logger = logger;
    }

    /// <summary>
    /// 获取工序良率（分页）
    /// </summary>
    [HttpGet("process-yield")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<YieldStatisticsResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProcessYield([FromQuery] YieldQuery query)
    {
        try
        {
            var result = await _yieldService.GetProcessYieldAsync(query);
            return Ok(ApiResponse<PagedResult<YieldStatisticsResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工序良率时发生异常");
            return BadRequest(ApiResponse<PagedResult<YieldStatisticsResponse>>.Fail("获取工序良率失败"));
        }
    }

    /// <summary>
    /// 获取累计良率
    /// </summary>
    [HttpGet("cumulative-yield")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCumulativeYield([FromQuery] string lotId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(lotId))
                return BadRequest(ApiResponse<decimal>.Fail("批次ID不能为空"));

            var result = await _yieldService.GetCumulativeYieldAsync(lotId);
            return Ok(ApiResponse<decimal>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取累计良率时发生异常");
            return BadRequest(ApiResponse<decimal>.Fail("获取累计良率失败"));
        }
    }

    /// <summary>
    /// 获取良率趋势
    /// </summary>
    [HttpGet("trend")]
    [ProducesResponseType(typeof(ApiResponse<YieldTrendResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetYieldTrend([FromQuery] string lotId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(lotId))
                return BadRequest(ApiResponse<YieldTrendResponse>.Fail("批次ID不能为空"));

            var result = await _yieldService.GetYieldTrendAsync(lotId);
            return Ok(ApiResponse<YieldTrendResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取良率趋势时发生异常");
            return BadRequest(ApiResponse<YieldTrendResponse>.Fail("获取良率趋势失败"));
        }
    }

    /// <summary>
    /// 获取帕累托分析
    /// </summary>
    [HttpGet("pareto")]
    [ProducesResponseType(typeof(ApiResponse<YieldParetoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPareto([FromQuery] string? lotId = null, [FromQuery] DateOnly? startDate = null, [FromQuery] DateOnly? endDate = null)
    {
        try
        {
            var result = await _yieldService.GetParetoAsync(lotId, startDate, endDate);
            return Ok(ApiResponse<YieldParetoResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取帕累托分析时发生异常");
            return BadRequest(ApiResponse<YieldParetoResponse>.Fail("获取帕累托分析失败"));
        }
    }

    /// <summary>
    /// 获取DPPM数据
    /// </summary>
    [HttpGet("dppm")]
    [ProducesResponseType(typeof(ApiResponse<DppmResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDppm([FromQuery] string productId, [FromQuery] DateOnly periodStart, [FromQuery] DateOnly periodEnd)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(productId))
                return BadRequest(ApiResponse<DppmResponse>.Fail("产品ID不能为空"));

            var result = await _yieldService.GetDppmAsync(productId, periodStart, periodEnd);
            return Ok(ApiResponse<DppmResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取DPPM数据时发生异常");
            return BadRequest(ApiResponse<DppmResponse>.Fail("获取DPPM数据失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
