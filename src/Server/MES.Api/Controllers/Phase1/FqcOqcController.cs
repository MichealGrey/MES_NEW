using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Services.Quality;

namespace MES.Api.Controllers.Phase1;

/// <summary>
/// FQC/OQC检验 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FqcOqcController : ControllerBase
{
    private readonly IFqcOqcService _fqcOqcService;
    private readonly ILogger<FqcOqcController> _logger;

    public FqcOqcController(IFqcOqcService fqcOqcService, ILogger<FqcOqcController> logger)
    {
        _fqcOqcService = fqcOqcService;
        _logger = logger;
    }

    /// <summary>
    /// 创建FQC任务
    /// </summary>
    /// <param name="request">FQC任务创建请求</param>
    /// <returns>FQC任务响应</returns>
    [HttpPost("fqc")]
    [ProducesResponseType(typeof(ApiResponse<FqcTaskResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<FqcTaskResponse>), 400)]
    public async Task<ApiResponse<FqcTaskResponse>> CreateFqcTask([FromBody] CreateFqcTaskRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.LotId))
                return ApiResponse<FqcTaskResponse>.Fail("批次ID不能为空");

            if (string.IsNullOrWhiteSpace(request.WorkOrderId))
                return ApiResponse<FqcTaskResponse>.Fail("工单ID不能为空");

            var result = await _fqcOqcService.CreateFqcTaskAsync(request.LotId, request.WorkOrderId);
            return ApiResponse<FqcTaskResponse>.Ok(result, "FQC任务创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建FQC任务时发生异常");
            return ApiResponse<FqcTaskResponse>.Fail("创建FQC任务失败");
        }
    }

    /// <summary>
    /// 查询FQC任务列表（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页任务列表</returns>
    [HttpGet("fqc")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FqcTaskResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<FqcTaskResponse>>> GetFqcTasks([FromQuery] FqcTaskQuery query)
    {
        try
        {
            var result = await _fqcOqcService.GetFqcTasksAsync(query);
            return ApiResponse<PagedResult<FqcTaskResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询FQC任务时发生异常");
            return ApiResponse<PagedResult<FqcTaskResponse>>.Fail("查询FQC任务失败");
        }
    }

    /// <summary>
    /// 执行FQC检验
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="request">FQC检验执行请求</param>
    /// <returns>FQC任务响应</returns>
    [HttpPost("fqc/{taskId}")]
    [ProducesResponseType(typeof(ApiResponse<FqcTaskResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<FqcTaskResponse>), 400)]
    public async Task<ApiResponse<FqcTaskResponse>> ExecuteFqc(string taskId, [FromBody] ExecuteFqcRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(taskId))
                return ApiResponse<FqcTaskResponse>.Fail("任务ID不能为空");

            var result = await _fqcOqcService.ExecuteFqcAsync(taskId, request);
            return ApiResponse<FqcTaskResponse>.Ok(result, "FQC检验执行成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行FQC检验时发生异常");
            return ApiResponse<FqcTaskResponse>.Fail("执行FQC检验失败");
        }
    }

    /// <summary>
    /// 创建OQC任务
    /// </summary>
    /// <param name="request">OQC任务创建请求</param>
    /// <returns>OQC任务响应</returns>
    [HttpPost("oqc")]
    [ProducesResponseType(typeof(ApiResponse<OqcTaskResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<OqcTaskResponse>), 400)]
    public async Task<ApiResponse<OqcTaskResponse>> CreateOqcTask([FromBody] CreateOqcTaskRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.LotId))
                return ApiResponse<OqcTaskResponse>.Fail("批次ID不能为空");

            var result = await _fqcOqcService.CreateOqcTaskAsync(request.LotId, request.ShipmentId ?? string.Empty);
            return ApiResponse<OqcTaskResponse>.Ok(result, "OQC任务创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建OQC任务时发生异常");
            return ApiResponse<OqcTaskResponse>.Fail("创建OQC任务失败");
        }
    }

    /// <summary>
    /// 查询OQC任务列表（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页任务列表</returns>
    [HttpGet("oqc")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OqcTaskResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<OqcTaskResponse>>> GetOqcTasks([FromQuery] OqcTaskQuery query)
    {
        try
        {
            var result = await _fqcOqcService.GetOqcTasksAsync(query);
            return ApiResponse<PagedResult<OqcTaskResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询OQC任务时发生异常");
            return ApiResponse<PagedResult<OqcTaskResponse>>.Fail("查询OQC任务失败");
        }
    }

    /// <summary>
    /// 执行OQC检验
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="request">OQC检验执行请求</param>
    /// <returns>OQC任务响应</returns>
    [HttpPost("oqc/{taskId}")]
    [ProducesResponseType(typeof(ApiResponse<OqcTaskResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<OqcTaskResponse>), 400)]
    public async Task<ApiResponse<OqcTaskResponse>> ExecuteOqc(string taskId, [FromBody] ExecuteOqcRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(taskId))
                return ApiResponse<OqcTaskResponse>.Fail("任务ID不能为空");

            var result = await _fqcOqcService.ExecuteOqcAsync(taskId, request);
            return ApiResponse<OqcTaskResponse>.Ok(result, "OQC检验执行成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行OQC检验时发生异常");
            return ApiResponse<OqcTaskResponse>.Fail("执行OQC检验失败");
        }
    }

    /// <summary>
    /// MSL出货检查
    /// </summary>
    /// <param name="request">MSL检查请求</param>
    /// <returns>MSL检查结果</returns>
    [HttpPost("oqc/msl-check")]
    [ProducesResponseType(typeof(ApiResponse<MslCheckResult>), 200)]
    [ProducesResponseType(typeof(ApiResponse<MslCheckResult>), 400)]
    public async Task<ApiResponse<MslCheckResult>> CheckMslForShipment([FromBody] OqcMslCheckRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.LotId))
                return ApiResponse<MslCheckResult>.Fail("批次ID不能为空");

            var result = await _fqcOqcService.CheckMslForShipmentAsync(request);
            return ApiResponse<MslCheckResult>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MSL检查时发生异常");
            return ApiResponse<MslCheckResult>.Fail("MSL检查失败");
        }
    }
}

/// <summary>
/// 创建FQC任务请求
/// </summary>
public class CreateFqcTaskRequest
{
    /// <summary>
    /// 批次ID
    /// </summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>
    /// 工单ID
    /// </summary>
    public string WorkOrderId { get; set; } = string.Empty;
}

/// <summary>
/// 创建OQC任务请求
/// </summary>
public class CreateOqcTaskRequest
{
    /// <summary>
    /// 批次ID
    /// </summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>
    /// 出货单ID
    /// </summary>
    public string? ShipmentId { get; set; }
}
