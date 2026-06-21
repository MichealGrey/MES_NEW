using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Quality;
using MES.Contracts.Common;
using MES.Services.Quality;

namespace MES.Api.Controllers;

/// <summary>
/// 质量检验 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QualityInspectionsController : ControllerBase
{
    private readonly IQualityInspectionService _qualityService;
    private readonly ILogger<QualityInspectionsController> _logger;

    public QualityInspectionsController(IQualityInspectionService qualityService, ILogger<QualityInspectionsController> logger)
    {
        _qualityService = qualityService;
        _logger = logger;
    }

    /// <summary>
    /// 创建检验记录
    /// </summary>
    /// <param name="request">检验创建请求</param>
    /// <returns>检验响应</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<InspectionResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<InspectionResponse>), 400)]
    public async Task<ApiResponse<InspectionResponse>> CreateInspection([FromBody] InspectionCreateRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.LotId))
                return ApiResponse<InspectionResponse>.Fail("批次号不能为空");

            if (string.IsNullOrWhiteSpace(request.StepCode))
                return ApiResponse<InspectionResponse>.Fail("工序代码不能为空");

            if (string.IsNullOrWhiteSpace(request.InspectionType))
                return ApiResponse<InspectionResponse>.Fail("检验类型不能为空");

            if (string.IsNullOrWhiteSpace(request.InspectorId))
                return ApiResponse<InspectionResponse>.Fail("检验员工号不能为空");

            var createdBy = User.Identity?.Name ?? "System";
            var result = await _qualityService.CreateInspectionAsync(request, createdBy);

            return ApiResponse<InspectionResponse>.Ok(result, "检验记录创建成功");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "创建检验记录失败: {Message}", ex.Message);
            return ApiResponse<InspectionResponse>.Fail(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建检验记录时发生异常");
            return ApiResponse<InspectionResponse>.Fail("创建检验记录失败");
        }
    }

    /// <summary>
    /// 查询检验记录（分页）
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="lotId">批次号筛选</param>
    /// <param name="type">检验类型筛选</param>
    /// <param name="result">检验结果筛选</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<InspectionListResponse>), 200)]
    public async Task<ApiResponse<InspectionListResponse>> GetInspections(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? lotId = null,
        [FromQuery] string? type = null,
        [FromQuery] string? result = null)
    {
        try
        {
            var queryResult = await _qualityService.GetInspectionsAsync(pageIndex, pageSize, lotId, type, result);
            return ApiResponse<InspectionListResponse>.Ok(queryResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询检验记录时发生异常");
            return ApiResponse<InspectionListResponse>.Fail("查询检验记录失败");
        }
    }

    /// <summary>
    /// 获取检验详情
    /// </summary>
    /// <param name="id">检验ID</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<InspectionResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<InspectionResponse>), 404)]
    public async Task<ApiResponse<InspectionResponse>> GetInspectionDetail(string id)
    {
        try
        {
            var result = await _qualityService.GetInspectionDetailAsync(id);
            if (result == null)
                return ApiResponse<InspectionResponse>.Fail("检验记录不存在");

            return ApiResponse<InspectionResponse>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取检验详情时发生异常");
            return ApiResponse<InspectionResponse>.Fail("获取检验详情失败");
        }
    }

    /// <summary>
    /// 创建不合格品报告（NCR）
    /// </summary>
    /// <param name="request">NCR创建请求</param>
    [HttpPost("ncrs")]
    [ProducesResponseType(typeof(ApiResponse<NcrResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<NcrResponse>), 400)]
    public async Task<ApiResponse<NcrResponse>> CreateNcr([FromBody] NcrCreateRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.LotId))
                return ApiResponse<NcrResponse>.Fail("批次号不能为空");

            if (string.IsNullOrWhiteSpace(request.StepCode))
                return ApiResponse<NcrResponse>.Fail("工序代码不能为空");

            if (string.IsNullOrWhiteSpace(request.DefectType))
                return ApiResponse<NcrResponse>.Fail("缺陷类型不能为空");

            if (request.Quantity <= 0)
                return ApiResponse<NcrResponse>.Fail("不合格数量必须大于0");

            if (string.IsNullOrWhiteSpace(request.DiscovererId))
                return ApiResponse<NcrResponse>.Fail("发现人工号不能为空");

            var discovererName = User.Identity?.Name;
            var result = await _qualityService.CreateNcrAsync(request, discovererName);

            return ApiResponse<NcrResponse>.Ok(result, "不合格品报告创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建NCR时发生异常");
            return ApiResponse<NcrResponse>.Fail("创建不合格品报告失败");
        }
    }

    /// <summary>
    /// 查询NCR（分页）
    /// </summary>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="lotId">批次号筛选</param>
    /// <param name="status">状态筛选</param>
    /// <param name="severity">严重程度筛选</param>
    [HttpGet("ncrs")]
    [ProducesResponseType(typeof(ApiResponse<NcrListResponse>), 200)]
    public async Task<ApiResponse<NcrListResponse>> GetNcrs(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? lotId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? severity = null)
    {
        try
        {
            var queryResult = await _qualityService.GetNcrsAsync(pageIndex, pageSize, lotId, status, severity);
            return ApiResponse<NcrListResponse>.Ok(queryResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询NCR时发生异常");
            return ApiResponse<NcrListResponse>.Fail("查询NCR失败");
        }
    }

    /// <summary>
    /// 更新NCR状态
    /// </summary>
    /// <param name="id">NCR ID</param>
    /// <param name="request">状态更新请求</param>
    [HttpPut("ncrs/{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
    public async Task<ApiResponse<bool>> UpdateNcrStatus(string id, [FromBody] NcrStatusUpdateRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Status))
                return ApiResponse<bool>.Fail("状态不能为空");

            var reviewerId = User.Identity?.Name ?? "System";
            var reviewerName = User.Identity?.Name;
            var result = await _qualityService.UpdateNcrStatusAsync(
                id,
                request.Status,
                request.Disposition,
                reviewerId,
                reviewerName,
                request.ClosureComment);

            if (!result)
                return ApiResponse<bool>.Fail("NCR不存在");

            return ApiResponse<bool>.Ok(true, "NCR状态更新成功");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "更新NCR状态失败: {Message}", ex.Message);
            return ApiResponse<bool>.Fail(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新NCR状态时发生异常");
            return ApiResponse<bool>.Fail("更新NCR状态失败");
        }
    }

    /// <summary>
    /// 获取质量统计（合格率、不良率等）
    /// </summary>
    /// <param name="days">统计天数，默认30天</param>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponse<QualityStatsResponse>), 200)]
    public async Task<ApiResponse<QualityStatsResponse>> GetQualityStats([FromQuery] int days = 30)
    {
        try
        {
            if (days <= 0 || days > 365)
                return ApiResponse<QualityStatsResponse>.Fail("统计天数必须在1-365之间");

            var stats = await _qualityService.GetQualityStatsAsync(days);
            return ApiResponse<QualityStatsResponse>.Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取质量统计时发生异常");
            return ApiResponse<QualityStatsResponse>.Fail("获取质量统计失败");
        }
    }
}
