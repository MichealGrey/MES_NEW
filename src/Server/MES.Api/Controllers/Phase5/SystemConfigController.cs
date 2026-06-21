using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Services.Analytics;

namespace MES.Api.Controllers.Phase5;

/// <summary>
/// 系统配置 API
/// </summary>
[ApiController]
[Route("api/v5/[controller]")]
[Authorize]
public class SystemConfigController : ControllerBase
{
    private readonly ISystemConfigService _systemConfigService;
    private readonly ILogger<SystemConfigController> _logger;

    public SystemConfigController(ISystemConfigService systemConfigService, ILogger<SystemConfigController> logger)
    {
        _systemConfigService = systemConfigService;
        _logger = logger;
    }

    /// <summary>
    /// 获取系统配置列表
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<SystemConfigResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConfigs([FromQuery] SystemConfigQuery query)
    {
        try
        {
            var result = await _systemConfigService.GetConfigsAsync(query);
            return Ok(ApiResponse<PagedResult<SystemConfigResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取系统配置列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<SystemConfigResponse>>.Fail("获取系统配置列表失败"));
        }
    }

    /// <summary>
    /// 更新系统配置
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<SystemConfigResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SystemConfigResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateConfig([FromBody] UpdateSystemConfigRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ConfigKey))
                return BadRequest(ApiResponse<SystemConfigResponse>.Fail("配置键不能为空"));

            var result = await _systemConfigService.UpdateConfigAsync(request, GetOperatorId());
            return Ok(ApiResponse<SystemConfigResponse>.Ok(result, "系统配置更新成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<SystemConfigResponse>.Fail("系统配置不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新系统配置时发生异常");
            return BadRequest(ApiResponse<SystemConfigResponse>.Fail("更新系统配置失败"));
        }
    }

    /// <summary>
    /// 获取预警规则列表
    /// </summary>
    [HttpGet("alert-rules")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AlertRuleResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlertRules([FromQuery] AlertRuleQuery query)
    {
        try
        {
            var result = await _systemConfigService.QueryAlertRulesAsync(query);
            return Ok(ApiResponse<PagedResult<AlertRuleResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取预警规则列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<AlertRuleResponse>>.Fail("获取预警规则列表失败"));
        }
    }

    /// <summary>
    /// 创建预警规则
    /// </summary>
    [HttpPost("alert-rules")]
    [ProducesResponseType(typeof(ApiResponse<AlertRuleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AlertRuleResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAlertRule([FromBody] CreateAlertRuleRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RuleName))
                return BadRequest(ApiResponse<AlertRuleResponse>.Fail("规则名称不能为空"));
            if (string.IsNullOrWhiteSpace(request.RuleType))
                return BadRequest(ApiResponse<AlertRuleResponse>.Fail("规则类型不能为空"));

            var result = await _systemConfigService.CreateAlertRuleAsync(request, GetOperatorId());
            return Ok(ApiResponse<AlertRuleResponse>.Ok(result, "预警规则创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建预警规则时发生异常");
            return BadRequest(ApiResponse<AlertRuleResponse>.Fail("创建预警规则失败"));
        }
    }

    /// <summary>
    /// 更新预警规则
    /// </summary>
    [HttpPut("alert-rules/{ruleId}")]
    [ProducesResponseType(typeof(ApiResponse<AlertRuleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AlertRuleResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAlertRule(string ruleId, [FromBody] CreateAlertRuleRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ruleId))
                return BadRequest(ApiResponse<AlertRuleResponse>.Fail("规则ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.RuleName))
                return BadRequest(ApiResponse<AlertRuleResponse>.Fail("规则名称不能为空"));
            if (string.IsNullOrWhiteSpace(request.RuleType))
                return BadRequest(ApiResponse<AlertRuleResponse>.Fail("规则类型不能为空"));

            var result = await _systemConfigService.UpdateAlertRuleAsync(ruleId, request, GetOperatorId());
            return Ok(ApiResponse<AlertRuleResponse>.Ok(result, "预警规则更新成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<AlertRuleResponse>.Fail("预警规则不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新预警规则时发生异常");
            return BadRequest(ApiResponse<AlertRuleResponse>.Fail("更新预警规则失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
