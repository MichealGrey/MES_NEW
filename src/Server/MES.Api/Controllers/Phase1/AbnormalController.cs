using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Services.Production;

namespace MES.Api.Controllers.Phase1;

/// <summary>
/// 异常/停线 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AbnormalController : ControllerBase
{
    private readonly IAbnormalService _abnormalService;
    private readonly ILogger<AbnormalController> _logger;

    public AbnormalController(IAbnormalService abnormalService, ILogger<AbnormalController> logger)
    {
        _abnormalService = abnormalService;
        _logger = logger;
    }

    /// <summary>
    /// 报告异常
    /// </summary>
    /// <param name="request">异常报告请求</param>
    /// <returns>异常记录响应</returns>
    [HttpPost("report")]
    [ProducesResponseType(typeof(ApiResponse<AbnormalRecordResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AbnormalRecordResponse>), 400)]
    public async Task<ApiResponse<AbnormalRecordResponse>> Report([FromBody] ReportAbnormalRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.AbnormalType))
                return ApiResponse<AbnormalRecordResponse>.Fail("异常类型不能为空");

            if (string.IsNullOrWhiteSpace(request.Description))
                return ApiResponse<AbnormalRecordResponse>.Fail("异常描述不能为空");

            var result = await _abnormalService.ReportAsync(request);
            return ApiResponse<AbnormalRecordResponse>.Ok(result, "异常报告成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "报告异常时发生异常");
            return ApiResponse<AbnormalRecordResponse>.Fail("报告异常失败");
        }
    }

    /// <summary>
    /// 查询异常记录（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页异常记录列表</returns>
    [HttpGet("records")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AbnormalRecordResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<AbnormalRecordResponse>>> GetRecords([FromQuery] AbnormalQuery query)
    {
        try
        {
            var result = await _abnormalService.GetRecordsAsync(query);
            return ApiResponse<PagedResult<AbnormalRecordResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询异常记录时发生异常");
            return ApiResponse<PagedResult<AbnormalRecordResponse>>.Fail("查询异常记录失败");
        }
    }

    /// <summary>
    /// 获取异常详情
    /// </summary>
    /// <param name="abnormalId">异常ID</param>
    /// <returns>异常记录详情</returns>
    [HttpGet("records/{abnormalId}")]
    [ProducesResponseType(typeof(ApiResponse<AbnormalRecordResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<AbnormalRecordResponse>), 404)]
    public async Task<ApiResponse<AbnormalRecordResponse>> GetDetail(string abnormalId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(abnormalId))
                return ApiResponse<AbnormalRecordResponse>.Fail("异常ID不能为空");

            var result = await _abnormalService.GetDetailAsync(abnormalId);
            if (result == null)
                return ApiResponse<AbnormalRecordResponse>.Fail("异常记录不存在");

            return ApiResponse<AbnormalRecordResponse>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取异常详情时发生异常");
            return ApiResponse<AbnormalRecordResponse>.Fail("获取异常详情失败");
        }
    }

    /// <summary>
    /// 停线
    /// </summary>
    /// <param name="request">停线请求</param>
    /// <returns>停线响应</returns>
    [HttpPost("line-stop")]
    [ProducesResponseType(typeof(ApiResponse<LineStopResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<LineStopResponse>), 400)]
    public async Task<ApiResponse<LineStopResponse>> StopLine([FromBody] LineStopRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.AbnormalId))
                return ApiResponse<LineStopResponse>.Fail("异常ID不能为空");

            if (string.IsNullOrWhiteSpace(request.StopScope))
                return ApiResponse<LineStopResponse>.Fail("停线范围不能为空");

            var result = await _abnormalService.StopLineAsync(request);
            return ApiResponse<LineStopResponse>.Ok(result, "停线成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停线时发生异常");
            return ApiResponse<LineStopResponse>.Fail("停线失败");
        }
    }

    /// <summary>
    /// 恢复生产线
    /// </summary>
    /// <param name="stopId">停线ID</param>
    /// <param name="request">恢复生产线请求</param>
    /// <returns>停线响应</returns>
    [HttpPost("line-stop/{stopId}/resume")]
    [ProducesResponseType(typeof(ApiResponse<LineStopResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<LineStopResponse>), 400)]
    public async Task<ApiResponse<LineStopResponse>> ResumeLine(string stopId, [FromBody] ResumeLineRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(stopId))
                return ApiResponse<LineStopResponse>.Fail("停线ID不能为空");

            var result = await _abnormalService.ResumeLineAsync(stopId, request.ResumeBy, request.Comment ?? string.Empty);
            return ApiResponse<LineStopResponse>.Ok(result, "生产线恢复成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "恢复生产线时发生异常");
            return ApiResponse<LineStopResponse>.Fail("生产线恢复失败");
        }
    }

    /// <summary>
    /// 处理异常
    /// </summary>
    /// <param name="abnormalId">异常ID</param>
    /// <param name="request">异常处理请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("records/{abnormalId}/handle")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> Handle(string abnormalId, [FromBody] HandleAbnormalRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(abnormalId))
                return ApiResponse<bool>.Fail("异常ID不能为空");

            var result = await _abnormalService.HandleAsync(request);
            return ApiResponse<bool>.Ok(result, "异常处理成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理异常时发生异常");
            return ApiResponse<bool>.Fail("异常处理失败");
        }
    }

    /// <summary>
    /// 验证异常处理
    /// </summary>
    /// <param name="abnormalId">异常ID</param>
    /// <param name="request">异常验证请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("records/{abnormalId}/verify")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> Verify(string abnormalId, [FromBody] VerifyAbnormalRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(abnormalId))
                return ApiResponse<bool>.Fail("异常ID不能为空");

            if (string.IsNullOrWhiteSpace(request.VerifyResult))
                return ApiResponse<bool>.Fail("验证结果不能为空");

            var result = await _abnormalService.VerifyAsync(request);
            return ApiResponse<bool>.Ok(result, "异常验证成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证异常时发生异常");
            return ApiResponse<bool>.Fail("异常验证失败");
        }
    }

    /// <summary>
    /// 获取异常统计
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>异常统计结果</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<AbnormalStatisticsResponse>), 200)]
    public async Task<ApiResponse<AbnormalStatisticsResponse>> GetStatistics([FromQuery] AbnormalQuery query)
    {
        try
        {
            var result = await _abnormalService.GetStatisticsAsync(query);
            return ApiResponse<AbnormalStatisticsResponse>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取异常统计时发生异常");
            return ApiResponse<AbnormalStatisticsResponse>.Fail("获取异常统计失败");
        }
    }
}

/// <summary>
/// 恢复生产线请求
/// </summary>
public class ResumeLineRequest
{
    /// <summary>
    /// 恢复人工号
    /// </summary>
    public string ResumeBy { get; set; } = string.Empty;

    /// <summary>
    /// 恢复备注
    /// </summary>
    public string? Comment { get; set; }
}
