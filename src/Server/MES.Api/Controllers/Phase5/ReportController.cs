using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Services.Analytics;

namespace MES.Api.Controllers.Phase5;

/// <summary>
/// 报表管理 API
/// </summary>
[ApiController]
[Route("api/v5/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IReportService reportService, ILogger<ReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// 创建报表计划
    /// </summary>
    [HttpPost("schedule")]
    [ProducesResponseType(typeof(ApiResponse<ReportScheduleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ReportScheduleResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateReportScheduleRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ReportName))
                return BadRequest(ApiResponse<ReportScheduleResponse>.Fail("报表名称不能为空"));
            if (string.IsNullOrWhiteSpace(request.ReportType))
                return BadRequest(ApiResponse<ReportScheduleResponse>.Fail("报表类型不能为空"));

            var result = await _reportService.CreateScheduleAsync(request, GetOperatorId());
            return Ok(ApiResponse<ReportScheduleResponse>.Ok(result, "报表计划创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建报表计划时发生异常");
            return BadRequest(ApiResponse<ReportScheduleResponse>.Fail("创建报表计划失败"));
        }
    }

    /// <summary>
    /// 查询报表计划列表
    /// </summary>
    [HttpGet("schedule")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ReportScheduleResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QuerySchedules([FromQuery] ReportScheduleQuery query)
    {
        try
        {
            var result = await _reportService.QuerySchedulesAsync(query);
            return Ok(ApiResponse<PagedResult<ReportScheduleResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询报表计划列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<ReportScheduleResponse>>.Fail("查询报表计划列表失败"));
        }
    }

    /// <summary>
    /// 生成报表
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateReport([FromBody] GenerateReportRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ScheduleId))
                return BadRequest(ApiResponse<bool>.Fail("计划ID不能为空"));

            var result = await _reportService.GenerateReportAsync(request.ScheduleId);
            return Ok(ApiResponse<bool>.Ok(result, "报表生成成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<bool>.Fail("报表计划不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成报表时发生异常");
            return BadRequest(ApiResponse<bool>.Fail("生成报表失败"));
        }
    }

    /// <summary>
    /// 下载报表
    /// </summary>
    [HttpGet("{scheduleId}/download")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<byte[]>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadReport(string scheduleId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(scheduleId))
                return BadRequest(ApiResponse<byte[]>.Fail("计划ID不能为空"));

            var content = await _reportService.DownloadReportAsync(scheduleId);
            return File(content, "application/octet-stream", $"report_{scheduleId}.pdf");
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<byte[]>.Fail("报表计划不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "下载报表时发生异常");
            return BadRequest(ApiResponse<byte[]>.Fail("下载报表失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}

/// <summary>
/// 生成报表请求
/// </summary>
public class GenerateReportRequest
{
    public string ScheduleId { get; set; } = string.Empty;
}
