using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Services.Analytics;

namespace MES.Api.Controllers.Phase5;

/// <summary>
/// 可靠性测试 API
/// </summary>
[ApiController]
[Route("api/v5/[controller]")]
[Authorize]
public class ReliabilityTestController : ControllerBase
{
    private readonly IReliabilityTestService _reliabilityTestService;
    private readonly ILogger<ReliabilityTestController> _logger;

    public ReliabilityTestController(IReliabilityTestService reliabilityTestService, ILogger<ReliabilityTestController> logger)
    {
        _reliabilityTestService = reliabilityTestService;
        _logger = logger;
    }

    /// <summary>
    /// 创建可靠性测试计划
    /// </summary>
    [HttpPost("plans")]
    [ProducesResponseType(typeof(ApiResponse<ReliabilityTestPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ReliabilityTestPlanResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePlan([FromBody] CreateReliabilityTestPlanRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.PlanName))
                return BadRequest(ApiResponse<ReliabilityTestPlanResponse>.Fail("计划名称不能为空"));
            if (string.IsNullOrWhiteSpace(request.TestType))
                return BadRequest(ApiResponse<ReliabilityTestPlanResponse>.Fail("测试类型不能为空"));
            if (string.IsNullOrWhiteSpace(request.ProductId))
                return BadRequest(ApiResponse<ReliabilityTestPlanResponse>.Fail("产品ID不能为空"));

            var result = await _reliabilityTestService.CreatePlanAsync(request, GetOperatorId());
            return Ok(ApiResponse<ReliabilityTestPlanResponse>.Ok(result, "可靠性测试计划创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建可靠性测试计划时发生异常");
            return BadRequest(ApiResponse<ReliabilityTestPlanResponse>.Fail("创建测试计划失败"));
        }
    }

    /// <summary>
    /// 查询可靠性测试计划列表
    /// </summary>
    [HttpGet("plans")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ReliabilityTestPlanResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryPlans([FromQuery] ReliabilityTestQuery query)
    {
        try
        {
            var result = await _reliabilityTestService.QueryPlansAsync(query);
            return Ok(ApiResponse<PagedResult<ReliabilityTestPlanResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询可靠性测试计划列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<ReliabilityTestPlanResponse>>.Fail("查询测试计划列表失败"));
        }
    }

    /// <summary>
    /// 获取可靠性测试计划详情
    /// </summary>
    [HttpGet("plans/{planId}")]
    [ProducesResponseType(typeof(ApiResponse<ReliabilityTestPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ReliabilityTestPlanResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlan(string planId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                return BadRequest(ApiResponse<ReliabilityTestPlanResponse>.Fail("计划ID不能为空"));

            var result = await _reliabilityTestService.GetPlanAsync(planId);
            return Ok(ApiResponse<ReliabilityTestPlanResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<ReliabilityTestPlanResponse>.Fail("可靠性测试计划不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取可靠性测试计划详情时发生异常");
            return BadRequest(ApiResponse<ReliabilityTestPlanResponse>.Fail("获取测试计划详情失败"));
        }
    }

    /// <summary>
    /// 执行可靠性测试
    /// </summary>
    [HttpPost("plans/{planId}/execute")]
    [ProducesResponseType(typeof(ApiResponse<ReliabilityTestPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ReliabilityTestPlanResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExecuteTest(string planId, [FromBody] ExecuteTestRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                return BadRequest(ApiResponse<ReliabilityTestPlanResponse>.Fail("计划ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.ResultSummary))
                return BadRequest(ApiResponse<ReliabilityTestPlanResponse>.Fail("测试结果不能为空"));

            var result = await _reliabilityTestService.ExecuteTestAsync(planId, request.ResultSummary, GetOperatorId());
            return Ok(ApiResponse<ReliabilityTestPlanResponse>.Ok(result, "可靠性测试执行成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<ReliabilityTestPlanResponse>.Fail("可靠性测试计划不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行可靠性测试时发生异常");
            return BadRequest(ApiResponse<ReliabilityTestPlanResponse>.Fail("执行测试失败"));
        }
    }

    /// <summary>
    /// 触发失效分析
    /// </summary>
    [HttpPost("plans/{planId}/fail-fa")]
    [ProducesResponseType(typeof(ApiResponse<ReliabilityTestPlanResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ReliabilityTestPlanResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TriggerFailFa(string planId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                return BadRequest(ApiResponse<ReliabilityTestPlanResponse>.Fail("计划ID不能为空"));

            var result = await _reliabilityTestService.TriggerFailFaAsync(planId, GetOperatorId());
            return Ok(ApiResponse<ReliabilityTestPlanResponse>.Ok(result, "失效分析已触发"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<ReliabilityTestPlanResponse>.Fail("可靠性测试计划不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "触发失效分析时发生异常");
            return BadRequest(ApiResponse<ReliabilityTestPlanResponse>.Fail("触发失效分析失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}

/// <summary>
/// 执行测试请求
/// </summary>
public class ExecuteTestRequest
{
    public string ResultSummary { get; set; } = string.Empty;
}
