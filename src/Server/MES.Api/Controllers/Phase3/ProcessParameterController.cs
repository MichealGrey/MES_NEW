using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Services.ProcessControl;

namespace MES.Api.Controllers.Phase3;

/// <summary>
/// 工序参数管控 API
/// </summary>
[ApiController]
[Route("api/v3/[controller]")]
[Authorize]
public class ProcessParameterController : ControllerBase
{
    private readonly IProcessParameterService _processParameterService;
    private readonly ILogger<ProcessParameterController> _logger;

    public ProcessParameterController(IProcessParameterService processParameterService, ILogger<ProcessParameterController> logger)
    {
        _processParameterService = processParameterService;
        _logger = logger;
    }

    /// <summary>
    /// 创建工序参数集
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProcessParameterSetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProcessParameterSetResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateParameterSet([FromBody] CreateProcessParameterSetRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ProcessCode))
                return BadRequest(ApiResponse<ProcessParameterSetResponse>.Fail("工序代码不能为空"));
            if (string.IsNullOrWhiteSpace(request.ProcessName))
                return BadRequest(ApiResponse<ProcessParameterSetResponse>.Fail("工序名称不能为空"));
            if (request.Items == null || request.Items.Count == 0)
                return BadRequest(ApiResponse<ProcessParameterSetResponse>.Fail("参数项列表不能为空"));

            var result = await _processParameterService.CreateParameterSetAsync(request, GetOperatorId());
            return Ok(ApiResponse<ProcessParameterSetResponse>.Ok(result, "工序参数集创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建工序参数集时发生异常");
            return BadRequest(ApiResponse<ProcessParameterSetResponse>.Fail("创建工序参数集失败"));
        }
    }

    /// <summary>
    /// 查询工序参数集列表（分页）
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProcessParameterSetResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryParameterSets([FromQuery] ProcessParameterQuery query)
    {
        try
        {
            var result = await _processParameterService.QueryParameterSetsAsync(query);
            return Ok(ApiResponse<PagedResult<ProcessParameterSetResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询工序参数集列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<ProcessParameterSetResponse>>.Fail("查询工序参数集列表失败"));
        }
    }

    /// <summary>
    /// 获取工序参数集详情
    /// </summary>
    [HttpGet("{parameterSetId}")]
    [ProducesResponseType(typeof(ApiResponse<ProcessParameterSetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProcessParameterSetResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetParameterSet(string parameterSetId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(parameterSetId))
                return BadRequest(ApiResponse<ProcessParameterSetResponse>.Fail("参数集ID不能为空"));

            var result = await _processParameterService.GetParameterSetAsync(parameterSetId);
            return Ok(ApiResponse<ProcessParameterSetResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<ProcessParameterSetResponse>.Fail("参数集不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工序参数集详情时发生异常");
            return BadRequest(ApiResponse<ProcessParameterSetResponse>.Fail("获取工序参数集详情失败"));
        }
    }

    /// <summary>
    /// 获取参数集的参数项列表
    /// </summary>
    [HttpGet("{parameterSetId}/items")]
    [ProducesResponseType(typeof(ApiResponse<List<ParameterItemResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<ParameterItemResponse>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetParameterItems(string parameterSetId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(parameterSetId))
                return BadRequest(ApiResponse<List<ParameterItemResponse>>.Fail("参数集ID不能为空"));

            var result = await _processParameterService.GetParameterItemsAsync(parameterSetId);
            return Ok(ApiResponse<List<ParameterItemResponse>>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<List<ParameterItemResponse>>.Fail("参数集不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取参数项列表时发生异常");
            return BadRequest(ApiResponse<List<ParameterItemResponse>>.Fail("获取参数项列表失败"));
        }
    }

    /// <summary>
    /// 激活工序参数集
    /// </summary>
    [HttpPost("{parameterSetId}/activate")]
    [ProducesResponseType(typeof(ApiResponse<ProcessParameterSetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ProcessParameterSetResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActivateParameterSet(string parameterSetId, [FromBody] ActivateParameterSetRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(parameterSetId))
                return BadRequest(ApiResponse<ProcessParameterSetResponse>.Fail("参数集ID不能为空"));
            if (request == null)
                return BadRequest(ApiResponse<ProcessParameterSetResponse>.Fail("请求体不能为空"));

            var result = await _processParameterService.ActivateParameterSetAsync(parameterSetId, request, GetOperatorId());
            return Ok(ApiResponse<ProcessParameterSetResponse>.Ok(result, "参数集激活成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<ProcessParameterSetResponse>.Fail("参数集不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "激活工序参数集时发生异常");
            return BadRequest(ApiResponse<ProcessParameterSetResponse>.Fail("激活参数集失败"));
        }
    }

    /// <summary>
    /// 覆盖参数值
    /// </summary>
    [HttpPost("{parameterSetId}/override")]
    [ProducesResponseType(typeof(ApiResponse<ParameterOverrideLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ParameterOverrideLogResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> OverrideParameter(string parameterSetId, [FromBody] OverrideParameterRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(parameterSetId))
                return BadRequest(ApiResponse<ParameterOverrideLogResponse>.Fail("参数集ID不能为空"));
            if (request == null || string.IsNullOrWhiteSpace(request.ParameterCode))
                return BadRequest(ApiResponse<ParameterOverrideLogResponse>.Fail("参数代码不能为空"));
            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(ApiResponse<ParameterOverrideLogResponse>.Fail("覆盖原因不能为空"));

            var result = await _processParameterService.OverrideParameterAsync(parameterSetId, request, GetOperatorId());
            return Ok(ApiResponse<ParameterOverrideLogResponse>.Ok(result, "参数覆盖成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<ParameterOverrideLogResponse>.Fail("参数集不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "覆盖参数时发生异常");
            return BadRequest(ApiResponse<ParameterOverrideLogResponse>.Fail("覆盖参数失败"));
        }
    }

    /// <summary>
    /// 获取参数覆盖日志（分页）
    /// </summary>
    [HttpGet("{parameterSetId}/override-logs")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ParameterOverrideLogResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverrideLogs(string parameterSetId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(parameterSetId))
                return BadRequest(ApiResponse<PagedResult<ParameterOverrideLogResponse>>.Fail("参数集ID不能为空"));

            var result = await _processParameterService.GetOverrideLogsAsync(parameterSetId, pageIndex, pageSize);
            return Ok(ApiResponse<PagedResult<ParameterOverrideLogResponse>>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<PagedResult<ParameterOverrideLogResponse>>.Fail("参数集不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取参数覆盖日志时发生异常");
            return BadRequest(ApiResponse<PagedResult<ParameterOverrideLogResponse>>.Fail("获取参数覆盖日志失败"));
        }
    }

    /// <summary>
    /// 创建固化温度曲线
    /// </summary>
    [HttpPost("curing-curves")]
    [ProducesResponseType(typeof(ApiResponse<CuringCurveResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CuringCurveResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCuringCurve([FromBody] CreateCuringCurveRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CurveName))
                return BadRequest(ApiResponse<CuringCurveResponse>.Fail("曲线名称不能为空"));

            var result = await _processParameterService.CreateCuringCurveAsync(request, GetOperatorId());
            return Ok(ApiResponse<CuringCurveResponse>.Ok(result, "固化温度曲线创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建固化温度曲线时发生异常");
            return BadRequest(ApiResponse<CuringCurveResponse>.Fail("创建固化温度曲线失败"));
        }
    }

    /// <summary>
    /// 查询固化温度曲线列表（分页）
    /// </summary>
    [HttpGet("curing-curves")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CuringCurveResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryCuringCurves([FromQuery] CuringCurveQuery query)
    {
        try
        {
            var result = await _processParameterService.QueryCuringCurvesAsync(query);
            return Ok(ApiResponse<PagedResult<CuringCurveResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询固化温度曲线列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<CuringCurveResponse>>.Fail("查询固化温度曲线列表失败"));
        }
    }

    /// <summary>
    /// 获取固化温度曲线详情
    /// </summary>
    [HttpGet("curing-curves/{curveId}")]
    [ProducesResponseType(typeof(ApiResponse<CuringCurveResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CuringCurveResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCuringCurve(string curveId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(curveId))
                return BadRequest(ApiResponse<CuringCurveResponse>.Fail("曲线ID不能为空"));

            var result = await _processParameterService.GetCuringCurveAsync(curveId);
            return Ok(ApiResponse<CuringCurveResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<CuringCurveResponse>.Fail("固化温度曲线不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取固化温度曲线详情时发生异常");
            return BadRequest(ApiResponse<CuringCurveResponse>.Fail("获取固化温度曲线详情失败"));
        }
    }

    /// <summary>
    /// 激活固化温度曲线
    /// </summary>
    [HttpPost("curing-curves/{curveId}/activate")]
    [ProducesResponseType(typeof(ApiResponse<CuringCurveResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CuringCurveResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateCuringCurve(string curveId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(curveId))
                return BadRequest(ApiResponse<CuringCurveResponse>.Fail("曲线ID不能为空"));

            var result = await _processParameterService.ActivateCuringCurveAsync(curveId, GetOperatorId());
            return Ok(ApiResponse<CuringCurveResponse>.Ok(result, "固化温度曲线激活成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<CuringCurveResponse>.Fail("固化温度曲线不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "激活固化温度曲线时发生异常");
            return BadRequest(ApiResponse<CuringCurveResponse>.Fail("激活固化温度曲线失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
