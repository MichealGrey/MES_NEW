using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Services.Analytics;

namespace MES.Api.Controllers.Phase5;

/// <summary>
/// KPI 看板 API
/// </summary>
[ApiController]
[Route("api/v5/[controller]")]
[Authorize]
public class KpiController : ControllerBase
{
    private readonly IKpiService _kpiService;
    private readonly ILogger<KpiController> _logger;

    public KpiController(IKpiService kpiService, ILogger<KpiController> logger)
    {
        _kpiService = kpiService;
        _logger = logger;
    }

    /// <summary>
    /// 获取KPI看板数据
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<KpiDashboardResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard([FromQuery] string? periodType = null)
    {
        try
        {
            var result = await _kpiService.GetDashboardAsync(periodType);
            return Ok(ApiResponse<KpiDashboardResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取KPI看板数据时发生异常");
            return BadRequest(ApiResponse<KpiDashboardResponse>.Fail("获取KPI看板数据失败"));
        }
    }

    /// <summary>
    /// 获取KPI指标详情
    /// </summary>
    [HttpGet("dashboard/{metricCode}/detail")]
    [ProducesResponseType(typeof(ApiResponse<KpiMetricResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<KpiMetricResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMetricDetail(string metricCode, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(metricCode))
                return BadRequest(ApiResponse<KpiMetricResponse>.Fail("指标代码不能为空"));

            var result = await _kpiService.GetMetricDetailAsync(metricCode, startDate, endDate);
            return Ok(ApiResponse<KpiMetricResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<KpiMetricResponse>.Fail("KPI指标不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取KPI指标详情时发生异常");
            return BadRequest(ApiResponse<KpiMetricResponse>.Fail("获取指标详情失败"));
        }
    }

    /// <summary>
    /// 获取实时KPI数据
    /// </summary>
    [HttpGet("real-time")]
    [ProducesResponseType(typeof(ApiResponse<KpiDashboardResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRealTime()
    {
        try
        {
            var result = await _kpiService.GetDashboardAsync("Hourly");
            return Ok(ApiResponse<KpiDashboardResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取实时KPI数据时发生异常");
            return BadRequest(ApiResponse<KpiDashboardResponse>.Fail("获取实时KPI数据失败"));
        }
    }

    /// <summary>
    /// 采集KPI指标
    /// </summary>
    [HttpPost("capture")]
    [ProducesResponseType(typeof(ApiResponse<KpiMetricResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<KpiMetricResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CaptureMetric([FromBody] KpiCaptureRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.MetricCode))
                return BadRequest(ApiResponse<KpiMetricResponse>.Fail("指标代码不能为空"));
            if (string.IsNullOrWhiteSpace(request.MetricName))
                return BadRequest(ApiResponse<KpiMetricResponse>.Fail("指标名称不能为空"));

            var result = await _kpiService.CaptureMetricAsync(request, GetOperatorId());
            return Ok(ApiResponse<KpiMetricResponse>.Ok(result, "KPI指标采集成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "采集KPI指标时发生异常");
            return BadRequest(ApiResponse<KpiMetricResponse>.Fail("采集KPI指标失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
