using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Services.Analytics;

namespace MES.Api.Controllers.Phase5;

/// <summary>
/// 审计管理 API
/// </summary>
[ApiController]
[Route("api/v5/[controller]")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditController> _logger;

    public AuditController(IAuditService auditService, ILogger<AuditController> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    /// <summary>
    /// 查询审计追踪记录
    /// </summary>
    [HttpGet("trails")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DataCorrectionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryAuditTrails([FromQuery] DataCorrectionQuery query)
    {
        try
        {
            var result = await _auditService.QueryCorrectionsAsync(query);
            return Ok(ApiResponse<PagedResult<DataCorrectionResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询审计追踪记录时发生异常");
            return BadRequest(ApiResponse<PagedResult<DataCorrectionResponse>>.Fail("查询审计追踪记录失败"));
        }
    }

    /// <summary>
    /// 验证审计完整性
    /// </summary>
    [HttpGet("trails/verify")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyAuditIntegrity()
    {
        try
        {
            var result = await _auditService.VerifyAuditIntegrityAsync();
            return Ok(ApiResponse<bool>.Ok(result, result ? "审计完整性验证通过" : "审计完整性验证失败"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证审计完整性时发生异常");
            return BadRequest(ApiResponse<bool>.Fail("验证审计完整性失败"));
        }
    }

    /// <summary>
    /// 哈希校验
    /// </summary>
    [HttpPost("trails/hash-check")]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<string, string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> HashCheck([FromBody] HashCheckRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.TableName))
                return BadRequest(ApiResponse<Dictionary<string, string>>.Fail("表名不能为空"));

            var result = await _auditService.HashCheckAsync(request.TableName, request.StartDate, request.EndDate);
            return Ok(ApiResponse<Dictionary<string, string>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行哈希校验时发生异常");
            return BadRequest(ApiResponse<Dictionary<string, string>>.Fail("哈希校验失败"));
        }
    }

    /// <summary>
    /// 查询数据修正记录
    /// </summary>
    [HttpGet("corrections")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DataCorrectionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryCorrections([FromQuery] DataCorrectionQuery query)
    {
        try
        {
            var result = await _auditService.QueryCorrectionsAsync(query);
            return Ok(ApiResponse<PagedResult<DataCorrectionResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询数据修正记录时发生异常");
            return BadRequest(ApiResponse<PagedResult<DataCorrectionResponse>>.Fail("查询数据修正记录失败"));
        }
    }

    /// <summary>
    /// 创建数据修正记录
    /// </summary>
    [HttpPost("corrections")]
    [ProducesResponseType(typeof(ApiResponse<DataCorrectionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<DataCorrectionResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCorrection([FromBody] CreateDataCorrectionRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.TableName))
                return BadRequest(ApiResponse<DataCorrectionResponse>.Fail("表名不能为空"));
            if (string.IsNullOrWhiteSpace(request.RecordId))
                return BadRequest(ApiResponse<DataCorrectionResponse>.Fail("记录ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.FieldName))
                return BadRequest(ApiResponse<DataCorrectionResponse>.Fail("字段名不能为空"));
            if (string.IsNullOrWhiteSpace(request.Reason))
                return BadRequest(ApiResponse<DataCorrectionResponse>.Fail("修正原因不能为空"));

            var result = await _auditService.CreateCorrectionAsync(request, GetOperatorId());
            return Ok(ApiResponse<DataCorrectionResponse>.Ok(result, "数据修正记录创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建数据修正记录时发生异常");
            return BadRequest(ApiResponse<DataCorrectionResponse>.Fail("创建数据修正记录失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}

/// <summary>
/// 哈希校验请求
/// </summary>
public class HashCheckRequest
{
    public string TableName { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
