using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Services.Quality;

namespace MES.Api.Controllers.Phase1;

/// <summary>
/// IQC来料检验 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IqcController : ControllerBase
{
    private readonly IIqcService _iqcService;
    private readonly ILogger<IqcController> _logger;

    public IqcController(IIqcService iqcService, ILogger<IqcController> logger)
    {
        _iqcService = iqcService;
        _logger = logger;
    }

    /// <summary>
    /// 创建IQC检验任务
    /// </summary>
    /// <param name="request">检验任务创建请求</param>
    /// <returns>检验任务响应</returns>
    [HttpPost("tasks")]
    [ProducesResponseType(typeof(ApiResponse<IqcTaskResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<IqcTaskResponse>), 400)]
    public async Task<ApiResponse<IqcTaskResponse>> CreateTask([FromBody] CreateIqcTaskRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.MaterialId))
                return ApiResponse<IqcTaskResponse>.Fail("物料ID不能为空");

            if (string.IsNullOrWhiteSpace(request.SupplierId))
                return ApiResponse<IqcTaskResponse>.Fail("供应商ID不能为空");

            if (request.Quantity <= 0)
                return ApiResponse<IqcTaskResponse>.Fail("数量必须大于0");

            var result = await _iqcService.CreateTaskAsync(request);
            return ApiResponse<IqcTaskResponse>.Ok(result, "IQC检验任务创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建IQC任务时发生异常");
            return ApiResponse<IqcTaskResponse>.Fail("创建IQC任务失败");
        }
    }

    /// <summary>
    /// 查询IQC任务列表（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页任务列表</returns>
    [HttpGet("tasks")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<IqcTaskResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<IqcTaskResponse>>> GetTasks([FromQuery] IqcTaskQuery query)
    {
        try
        {
            var result = await _iqcService.GetTasksAsync(query);
            return ApiResponse<PagedResult<IqcTaskResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询IQC任务时发生异常");
            return ApiResponse<PagedResult<IqcTaskResponse>>.Fail("查询IQC任务失败");
        }
    }

    /// <summary>
    /// 获取IQC任务详情
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <returns>任务详情</returns>
    [HttpGet("tasks/{taskId}")]
    [ProducesResponseType(typeof(ApiResponse<IqcTaskDetailResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<IqcTaskDetailResponse>), 404)]
    public async Task<ApiResponse<IqcTaskDetailResponse>> GetTaskDetail(string taskId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(taskId))
                return ApiResponse<IqcTaskDetailResponse>.Fail("任务ID不能为空");

            var result = await _iqcService.GetTaskDetailAsync(taskId);
            if (result == null)
                return ApiResponse<IqcTaskDetailResponse>.Fail("任务不存在");

            return ApiResponse<IqcTaskDetailResponse>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取IQC任务详情时发生异常");
            return ApiResponse<IqcTaskDetailResponse>.Fail("获取任务详情失败");
        }
    }

    /// <summary>
    /// 执行检验
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="request">检验执行请求</param>
    /// <returns>检验结果</returns>
    [HttpPost("tasks/{taskId}/inspect")]
    [ProducesResponseType(typeof(ApiResponse<InspectionResultResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<InspectionResultResponse>), 400)]
    public async Task<ApiResponse<InspectionResultResponse>> ExecuteInspection(string taskId, [FromBody] ExecuteInspectionRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(taskId))
                return ApiResponse<InspectionResultResponse>.Fail("任务ID不能为空");

            var result = await _iqcService.ExecuteInspectionAsync(taskId, request);
            return ApiResponse<InspectionResultResponse>.Ok(result, "检验记录成功");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "执行检验失败: {Message}", ex.Message);
            return ApiResponse<InspectionResultResponse>.Fail(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行检验时发生异常");
            return ApiResponse<InspectionResultResponse>.Fail("执行检验失败");
        }
    }

    /// <summary>
    /// 判定
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="request">判定请求</param>
    /// <returns>判定结果</returns>
    [HttpPost("tasks/{taskId}/judge")]
    [ProducesResponseType(typeof(ApiResponse<IqcTaskResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<IqcTaskResponse>), 400)]
    public async Task<ApiResponse<IqcTaskResponse>> Judge(string taskId, [FromBody] JudgeRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(taskId))
                return ApiResponse<IqcTaskResponse>.Fail("任务ID不能为空");

            if (string.IsNullOrWhiteSpace(request.Judgment))
                return ApiResponse<IqcTaskResponse>.Fail("判定结果不能为空");

            var result = await _iqcService.JudgeAsync(taskId, request);
            return ApiResponse<IqcTaskResponse>.Ok(result, "判定成功");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "判定失败: {Message}", ex.Message);
            return ApiResponse<IqcTaskResponse>.Fail(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "判定时发生异常");
            return ApiResponse<IqcTaskResponse>.Fail("判定失败");
        }
    }

    /// <summary>
    /// 隔离批次
    /// </summary>
    /// <param name="batchId">批次ID</param>
    /// <param name="request">隔离请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("batches/{batchId}/isolate")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> IsolateBatch(string batchId, [FromBody] IsolateBatchRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(batchId))
                return ApiResponse<bool>.Fail("批次ID不能为空");

            var result = await _iqcService.IsolateBatchAsync(batchId, request.OperatorId);
            return ApiResponse<bool>.Ok(result, "批次隔离成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "隔离批次时发生异常");
            return ApiResponse<bool>.Fail("批次隔离失败");
        }
    }

    /// <summary>
    /// 释放批次
    /// </summary>
    /// <param name="batchId">批次ID</param>
    /// <param name="request">释放请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("batches/{batchId}/release")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> ReleaseBatch(string batchId, [FromBody] ReleaseBatchRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(batchId))
                return ApiResponse<bool>.Fail("批次ID不能为空");

            var result = await _iqcService.ReleaseBatchAsync(batchId, request.OperatorId);
            return ApiResponse<bool>.Ok(result, "批次释放成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放批次时发生异常");
            return ApiResponse<bool>.Fail("批次释放失败");
        }
    }

    /// <summary>
    /// 获取IQC统计
    /// </summary>
    /// <param name="query">统计查询条件</param>
    /// <returns>IQC统计结果</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<IqcStatisticsResponse>), 200)]
    public async Task<ApiResponse<IqcStatisticsResponse>> GetStatistics([FromQuery] IqcStatisticsQuery query)
    {
        try
        {
            var result = await _iqcService.GetStatisticsAsync(query);
            return ApiResponse<IqcStatisticsResponse>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取IQC统计时发生异常");
            return ApiResponse<IqcStatisticsResponse>.Fail("获取IQC统计失败");
        }
    }
}

/// <summary>
/// 批次隔离请求
/// </summary>
public class IsolateBatchRequest
{
    /// <summary>
    /// 操作人工号
    /// </summary>
    public string OperatorId { get; set; } = string.Empty;
}

/// <summary>
/// 批次释放请求
/// </summary>
public class ReleaseBatchRequest
{
    /// <summary>
    /// 操作人工号
    /// </summary>
    public string OperatorId { get; set; } = string.Empty;
}
