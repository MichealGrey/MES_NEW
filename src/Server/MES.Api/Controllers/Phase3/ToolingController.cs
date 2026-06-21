using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Services.ProcessControl;

namespace MES.Api.Controllers.Phase3;

/// <summary>
/// 工装管控 API
/// </summary>
[ApiController]
[Route("api/v3/[controller]")]
[Authorize]
public class ToolingController : ControllerBase
{
    private readonly IToolingService _toolingService;
    private readonly ILogger<ToolingController> _logger;

    public ToolingController(IToolingService toolingService, ILogger<ToolingController> logger)
    {
        _toolingService = toolingService;
        _logger = logger;
    }

    /// <summary>
    /// 创建工装
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ToolingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ToolingResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTooling([FromBody] CreateToolingRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.ToolingCode))
                return BadRequest(ApiResponse<ToolingResponse>.Fail("工装代码不能为空"));
            if (string.IsNullOrWhiteSpace(request.ToolingName))
                return BadRequest(ApiResponse<ToolingResponse>.Fail("工装名称不能为空"));
            if (string.IsNullOrWhiteSpace(request.ToolingType))
                return BadRequest(ApiResponse<ToolingResponse>.Fail("工装类型不能为空"));

            var result = await _toolingService.CreateToolingAsync(request, GetOperatorId());
            return Ok(ApiResponse<ToolingResponse>.Ok(result, "工装创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建工装时发生异常");
            return BadRequest(ApiResponse<ToolingResponse>.Fail("创建工装失败"));
        }
    }

    /// <summary>
    /// 查询工装列表（分页）
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ToolingResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryToolings([FromQuery] ToolingQuery query)
    {
        try
        {
            var result = await _toolingService.QueryToolingsAsync(query);
            return Ok(ApiResponse<PagedResult<ToolingResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询工装列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<ToolingResponse>>.Fail("查询工装列表失败"));
        }
    }

    /// <summary>
    /// 获取工装详情
    /// </summary>
    [HttpGet("{toolingId}")]
    [ProducesResponseType(typeof(ApiResponse<ToolingResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ToolingResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTooling(string toolingId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(toolingId))
                return BadRequest(ApiResponse<ToolingResponse>.Fail("工装ID不能为空"));

            var result = await _toolingService.GetToolingAsync(toolingId);
            return Ok(ApiResponse<ToolingResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<ToolingResponse>.Fail("工装不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工装详情时发生异常");
            return BadRequest(ApiResponse<ToolingResponse>.Fail("获取工装详情失败"));
        }
    }

    /// <summary>
    /// 记录工装使用
    /// </summary>
    [HttpPost("{toolingId}/usage")]
    [ProducesResponseType(typeof(ApiResponse<ToolingUsageLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ToolingUsageLogResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LogUsage(string toolingId, [FromBody] ToolingUsageLogRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(toolingId))
                return BadRequest(ApiResponse<ToolingUsageLogResponse>.Fail("工装ID不能为空"));
            if (request == null || string.IsNullOrWhiteSpace(request.LotId))
                return BadRequest(ApiResponse<ToolingUsageLogResponse>.Fail("批次ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.StepCode))
                return BadRequest(ApiResponse<ToolingUsageLogResponse>.Fail("工序代码不能为空"));

            var result = await _toolingService.LogUsageAsync(request, GetOperatorId());
            return Ok(ApiResponse<ToolingUsageLogResponse>.Ok(result, "工装使用记录成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<ToolingUsageLogResponse>.Fail("工装不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "记录工装使用时发生异常");
            return BadRequest(ApiResponse<ToolingUsageLogResponse>.Fail("记录工装使用失败"));
        }
    }

    /// <summary>
    /// 获取工装使用日志（分页）
    /// </summary>
    [HttpGet("{toolingId}/usage-logs")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ToolingUsageLogResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsageLogs(string toolingId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(toolingId))
                return BadRequest(ApiResponse<PagedResult<ToolingUsageLogResponse>>.Fail("工装ID不能为空"));

            var result = await _toolingService.GetToolingUsageLogsAsync(toolingId, pageIndex, pageSize);
            return Ok(ApiResponse<PagedResult<ToolingUsageLogResponse>>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<PagedResult<ToolingUsageLogResponse>>.Fail("工装不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工装使用日志时发生异常");
            return BadRequest(ApiResponse<PagedResult<ToolingUsageLogResponse>>.Fail("获取工装使用日志失败"));
        }
    }

    /// <summary>
    /// 记录工装更换
    /// </summary>
    [HttpPost("replacement")]
    [ProducesResponseType(typeof(ApiResponse<ToolingReplacementResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ToolingReplacementResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordReplacement([FromBody] ToolingReplacementRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.OldToolingId))
                return BadRequest(ApiResponse<ToolingReplacementResponse>.Fail("旧工装ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.NewToolingId))
                return BadRequest(ApiResponse<ToolingReplacementResponse>.Fail("新工装ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.ReplacementReason))
                return BadRequest(ApiResponse<ToolingReplacementResponse>.Fail("更换原因不能为空"));

            var result = await _toolingService.RecordReplacementAsync(request, GetOperatorId());
            return Ok(ApiResponse<ToolingReplacementResponse>.Ok(result, "工装更换记录成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<ToolingReplacementResponse>.Fail("工装不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "记录工装更换时发生异常");
            return BadRequest(ApiResponse<ToolingReplacementResponse>.Fail("记录工装更换失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
