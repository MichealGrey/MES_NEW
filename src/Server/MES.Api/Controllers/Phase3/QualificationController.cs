using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Services.ProcessControl;

namespace MES.Api.Controllers.Phase3;

/// <summary>
/// 操作员资质管理 API
/// </summary>
[ApiController]
[Route("api/v3/[controller]")]
[Authorize]
public class QualificationController : ControllerBase
{
    private readonly IQualificationService _qualificationService;
    private readonly ILogger<QualificationController> _logger;

    public QualificationController(IQualificationService qualificationService, ILogger<QualificationController> logger)
    {
        _qualificationService = qualificationService;
        _logger = logger;
    }

    /// <summary>
    /// 创建操作员资质
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OperatorQualificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OperatorQualificationResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateQualification([FromBody] CreateOperatorQualificationRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.OperatorId))
                return BadRequest(ApiResponse<OperatorQualificationResponse>.Fail("操作员ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.OperatorName))
                return BadRequest(ApiResponse<OperatorQualificationResponse>.Fail("操作员姓名不能为空"));
            if (string.IsNullOrWhiteSpace(request.ProcessCode))
                return BadRequest(ApiResponse<OperatorQualificationResponse>.Fail("工序代码不能为空"));
            if (string.IsNullOrWhiteSpace(request.ProcessName))
                return BadRequest(ApiResponse<OperatorQualificationResponse>.Fail("工序名称不能为空"));

            var result = await _qualificationService.CreateQualificationAsync(request, GetOperatorId());
            return Ok(ApiResponse<OperatorQualificationResponse>.Ok(result, "操作员资质创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建操作员资质时发生异常");
            return BadRequest(ApiResponse<OperatorQualificationResponse>.Fail("创建操作员资质失败"));
        }
    }

    /// <summary>
    /// 查询操作员资质列表（分页）
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OperatorQualificationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryQualifications([FromQuery] OperatorQualificationQuery query)
    {
        try
        {
            var result = await _qualificationService.QueryQualificationsAsync(query);
            return Ok(ApiResponse<PagedResult<OperatorQualificationResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询操作员资质列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<OperatorQualificationResponse>>.Fail("查询操作员资质列表失败"));
        }
    }

    /// <summary>
    /// 获取操作员资质详情
    /// </summary>
    [HttpGet("{qualificationId}")]
    [ProducesResponseType(typeof(ApiResponse<OperatorQualificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<OperatorQualificationResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQualification(string qualificationId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(qualificationId))
                return BadRequest(ApiResponse<OperatorQualificationResponse>.Fail("资质ID不能为空"));

            var result = await _qualificationService.GetQualificationAsync(qualificationId);
            return Ok(ApiResponse<OperatorQualificationResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<OperatorQualificationResponse>.Fail("操作员资质不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取操作员资质详情时发生异常");
            return BadRequest(ApiResponse<OperatorQualificationResponse>.Fail("获取操作员资质详情失败"));
        }
    }

    /// <summary>
    /// 检查操作员资质
    /// </summary>
    [HttpPost("check")]
    [ProducesResponseType(typeof(ApiResponse<QualificationCheckLogResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<QualificationCheckLogResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckQualification([FromBody] OperatorQualificationCheckRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.OperatorId))
                return BadRequest(ApiResponse<QualificationCheckLogResponse>.Fail("操作员ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.ProcessCode))
                return BadRequest(ApiResponse<QualificationCheckLogResponse>.Fail("工序代码不能为空"));
            if (string.IsNullOrWhiteSpace(request.StepCode))
                return BadRequest(ApiResponse<QualificationCheckLogResponse>.Fail("工序步骤代码不能为空"));

            var result = await _qualificationService.CheckQualificationAsync(request, GetOperatorId());
            return Ok(ApiResponse<QualificationCheckLogResponse>.Ok(result, "资质检查完成"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<QualificationCheckLogResponse>.Fail("相关资质记录不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查操作员资质时发生异常");
            return BadRequest(ApiResponse<QualificationCheckLogResponse>.Fail("资质检查失败"));
        }
    }

    /// <summary>
    /// 获取资质检查日志（分页）
    /// </summary>
    [HttpGet("check-logs")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<QualificationCheckLogResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCheckLogs([FromQuery] string? operatorIdFilter = null, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var result = await _qualificationService.GetCheckLogsAsync(operatorIdFilter ?? string.Empty, pageIndex, pageSize);
            return Ok(ApiResponse<PagedResult<QualificationCheckLogResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取资质检查日志时发生异常");
            return BadRequest(ApiResponse<PagedResult<QualificationCheckLogResponse>>.Fail("获取资质检查日志失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
