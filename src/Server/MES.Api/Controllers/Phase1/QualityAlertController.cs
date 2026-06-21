using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Services.Quality;

namespace MES.Api.Controllers.Phase1;

/// <summary>
/// 质量预警/召回 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QualityAlertController : ControllerBase
{
    private readonly IQualityAlertService _qualityAlertService;
    private readonly ILogger<QualityAlertController> _logger;

    public QualityAlertController(IQualityAlertService qualityAlertService, ILogger<QualityAlertController> logger)
    {
        _qualityAlertService = qualityAlertService;
        _logger = logger;
    }

    /// <summary>
    /// 创建质量预警
    /// </summary>
    /// <param name="request">预警创建请求</param>
    /// <returns>质量预警响应</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<QualityAlertResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<QualityAlertResponse>), 400)]
    public async Task<ApiResponse<QualityAlertResponse>> CreateAlert([FromBody] CreateQualityAlertRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.AlertType))
                return ApiResponse<QualityAlertResponse>.Fail("预警类型不能为空");

            if (string.IsNullOrWhiteSpace(request.Severity))
                return ApiResponse<QualityAlertResponse>.Fail("严重程度不能为空");

            if (string.IsNullOrWhiteSpace(request.Title))
                return ApiResponse<QualityAlertResponse>.Fail("标题不能为空");

            var result = await _qualityAlertService.CreateAlertAsync(request);
            return ApiResponse<QualityAlertResponse>.Ok(result, "质量预警创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建质量预警时发生异常");
            return ApiResponse<QualityAlertResponse>.Fail("创建质量预警失败");
        }
    }

    /// <summary>
    /// 查询质量预警（分页）
    /// </summary>
    /// <param name="query">查询条件</param>
    /// <returns>分页预警列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<QualityAlertResponse>>), 200)]
    public async Task<ApiResponse<PagedResult<QualityAlertResponse>>> GetAlerts([FromQuery] QualityAlertQuery query)
    {
        try
        {
            var result = await _qualityAlertService.GetAlertsAsync(query);
            return ApiResponse<PagedResult<QualityAlertResponse>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询质量预警时发生异常");
            return ApiResponse<PagedResult<QualityAlertResponse>>.Fail("查询质量预警失败");
        }
    }

    /// <summary>
    /// 获取质量预警详情
    /// </summary>
    /// <param name="alertId">预警ID</param>
    /// <returns>预警详情</returns>
    [HttpGet("{alertId}")]
    [ProducesResponseType(typeof(ApiResponse<QualityAlertResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<QualityAlertResponse>), 404)]
    public async Task<ApiResponse<QualityAlertResponse>> GetDetail(string alertId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(alertId))
                return ApiResponse<QualityAlertResponse>.Fail("预警ID不能为空");

            var result = await _qualityAlertService.GetDetailAsync(alertId);
            if (result == null)
                return ApiResponse<QualityAlertResponse>.Fail("预警不存在");

            return ApiResponse<QualityAlertResponse>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取质量预警详情时发生异常");
            return ApiResponse<QualityAlertResponse>.Fail("获取质量预警详情失败");
        }
    }

    /// <summary>
    /// 分析受影响批次
    /// </summary>
    /// <param name="alertId">预警ID</param>
    /// <returns>受影响批次信息列表</returns>
    [HttpGet("{alertId}/affected-lots")]
    [ProducesResponseType(typeof(ApiResponse<List<AffectedLotInfo>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<List<AffectedLotInfo>>), 400)]
    public async Task<ApiResponse<List<AffectedLotInfo>>> AnalyzeAffectedLots(string alertId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(alertId))
                return ApiResponse<List<AffectedLotInfo>>.Fail("预警ID不能为空");

            var result = await _qualityAlertService.AnalyzeAffectedLotsAsync(alertId);
            return ApiResponse<List<AffectedLotInfo>>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分析受影响批次时发生异常");
            return ApiResponse<List<AffectedLotInfo>>.Fail("分析受影响批次失败");
        }
    }

    /// <summary>
    /// 冻结批次
    /// </summary>
    /// <param name="alertId">预警ID</param>
    /// <param name="request">冻结批次请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("{alertId}/freeze-lots")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> FreezeLots(string alertId, [FromBody] FreezeLotsRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(alertId))
                return ApiResponse<bool>.Fail("预警ID不能为空");

            if (request.LotIds == null || request.LotIds.Count == 0)
                return ApiResponse<bool>.Fail("批次列表不能为空");

            var result = await _qualityAlertService.FreezeLotsAsync(alertId, request.LotIds, request.OperatorId);
            return ApiResponse<bool>.Ok(result, "批次冻结成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "冻结批次时发生异常");
            return ApiResponse<bool>.Fail("批次冻结失败");
        }
    }

    /// <summary>
    /// 生成召回通知
    /// </summary>
    /// <param name="alertId">预警ID</param>
    /// <param name="request">生成召回请求</param>
    /// <returns>召回通知响应</returns>
    [HttpPost("{alertId}/generate-recall")]
    [ProducesResponseType(typeof(ApiResponse<RecallNoticeResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<RecallNoticeResponse>), 400)]
    public async Task<ApiResponse<RecallNoticeResponse>> GenerateRecall(string alertId, [FromBody] GenerateRecallRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(alertId))
                return ApiResponse<RecallNoticeResponse>.Fail("预警ID不能为空");

            var result = await _qualityAlertService.GenerateRecallAsync(alertId, request.IssuedBy, request.IssuedByName);
            return ApiResponse<RecallNoticeResponse>.Ok(result, "召回通知生成成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成召回通知时发生异常");
            return ApiResponse<RecallNoticeResponse>.Fail("生成召回通知失败");
        }
    }

    /// <summary>
    /// 关闭质量预警
    /// </summary>
    /// <param name="request">关闭预警请求</param>
    /// <returns>是否成功</returns>
    [HttpPost("close")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> CloseAlert([FromBody] CloseQualityAlertRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.AlertId))
                return ApiResponse<bool>.Fail("预警ID不能为空");

            var result = await _qualityAlertService.CloseAlertAsync(request);
            return ApiResponse<bool>.Ok(result, "质量预警关闭成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "关闭质量预警时发生异常");
            return ApiResponse<bool>.Fail("关闭质量预警失败");
        }
    }
}

/// <summary>
/// 冻结批次请求
/// </summary>
public class FreezeLotsRequest
{
    /// <summary>
    /// 批次ID列表
    /// </summary>
    public List<string> LotIds { get; set; } = [];

    /// <summary>
    /// 操作人工号
    /// </summary>
    public string OperatorId { get; set; } = string.Empty;
}

/// <summary>
/// 生成召回请求
/// </summary>
public class GenerateRecallRequest
{
    /// <summary>
    /// 发布人工号
    /// </summary>
    public string IssuedBy { get; set; } = string.Empty;

    /// <summary>
    /// 发布人姓名
    /// </summary>
    public string IssuedByName { get; set; } = string.Empty;
}
