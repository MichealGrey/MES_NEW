using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Services.ProcessControl;

namespace MES.Api.Controllers.Phase3;

/// <summary>
/// Bin 分选管控 API
/// </summary>
[ApiController]
[Route("api/v3/[controller]")]
[Authorize]
public class BinController : ControllerBase
{
    private readonly IBinService _binService;
    private readonly ILogger<BinController> _logger;

    public BinController(IBinService binService, ILogger<BinController> logger)
    {
        _binService = binService;
        _logger = logger;
    }

    /// <summary>
    /// 创建 Bin 定义
    /// </summary>
    [HttpPost("definitions")]
    [ProducesResponseType(typeof(ApiResponse<BinDefinitionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BinDefinitionResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBinDefinition([FromBody] CreateBinDefinitionRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.BinCode))
                return BadRequest(ApiResponse<BinDefinitionResponse>.Fail("Bin代码不能为空"));
            if (string.IsNullOrWhiteSpace(request.BinName))
                return BadRequest(ApiResponse<BinDefinitionResponse>.Fail("Bin名称不能为空"));
            if (string.IsNullOrWhiteSpace(request.BinCategory))
                return BadRequest(ApiResponse<BinDefinitionResponse>.Fail("Bin类别不能为空"));

            var result = await _binService.CreateBinDefinitionAsync(request, GetOperatorId());
            return Ok(ApiResponse<BinDefinitionResponse>.Ok(result, "Bin定义创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建Bin定义时发生异常");
            return BadRequest(ApiResponse<BinDefinitionResponse>.Fail("创建Bin定义失败"));
        }
    }

    /// <summary>
    /// 查询 Bin 定义列表（分页）
    /// </summary>
    [HttpGet("definitions")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BinDefinitionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryBinDefinitions([FromQuery] BinQuery query)
    {
        try
        {
            var result = await _binService.QueryBinDefinitionsAsync(query);
            return Ok(ApiResponse<PagedResult<BinDefinitionResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询Bin定义列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<BinDefinitionResponse>>.Fail("查询Bin定义列表失败"));
        }
    }

    /// <summary>
    /// 获取 Bin 定义详情
    /// </summary>
    [HttpGet("definitions/{binId}")]
    [ProducesResponseType(typeof(ApiResponse<BinDefinitionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BinDefinitionResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBinDefinition(string binId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(binId))
                return BadRequest(ApiResponse<BinDefinitionResponse>.Fail("BinID不能为空"));

            var result = await _binService.GetBinDefinitionAsync(binId);
            return Ok(ApiResponse<BinDefinitionResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<BinDefinitionResponse>.Fail("Bin定义不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取Bin定义详情时发生异常");
            return BadRequest(ApiResponse<BinDefinitionResponse>.Fail("获取Bin定义详情失败"));
        }
    }

    /// <summary>
    /// 获取 Bin 统计汇总
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<List<BinStatisticsResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBinSummary([FromQuery] BinSummaryQuery query)
    {
        try
        {
            var result = await _binService.GetBinSummaryAsync(query);
            return Ok(ApiResponse<List<BinStatisticsResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取Bin统计汇总时发生异常");
            return BadRequest(ApiResponse<List<BinStatisticsResponse>>.Fail("获取Bin统计汇总失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
