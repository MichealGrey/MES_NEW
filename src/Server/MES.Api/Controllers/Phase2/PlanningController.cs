using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase2;
using MES.Services.Planning;

namespace MES.Api.Controllers.Phase2;

/// <summary>
/// 生产计划管理 API
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
[Authorize]
public class PlanningController : ControllerBase
{
    private readonly IPlanningService _planningService;
    private readonly ILogger<PlanningController> _logger;

    public PlanningController(IPlanningService planningService, ILogger<PlanningController> logger)
    {
        _planningService = planningService;
        _logger = logger;
    }

    /// <summary>
    /// 生成主生产计划
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MppResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MppResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GeneratePlan([FromBody] CreateMppRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.PlanName))
                return BadRequest(ApiResponse<MppResponse>.Fail("计划名称不能为空"));
            if (request.PlanPeriodEnd <= request.PlanPeriodStart)
                return BadRequest(ApiResponse<MppResponse>.Fail("计划结束时间必须大于开始时间"));

            var result = await _planningService.GeneratePlanAsync(request, GetOperatorId());
            return Ok(ApiResponse<MppResponse>.Ok(result, "生产计划生成成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成生产计划时发生异常");
            return BadRequest(ApiResponse<MppResponse>.Fail("生成生产计划失败"));
        }
    }

    /// <summary>
    /// 查询生产计划列表（分页）
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MppResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans([FromQuery] MppQuery query)
    {
        try
        {
            var result = await _planningService.GetPlansAsync(query);
            return Ok(ApiResponse<PagedResult<MppResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询生产计划列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<MppResponse>>.Fail("查询生产计划列表失败"));
        }
    }

    /// <summary>
    /// 获取生产计划详情
    /// </summary>
    [HttpGet("{planId}")]
    [ProducesResponseType(typeof(ApiResponse<MppResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlan(string planId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                return BadRequest(ApiResponse<MppResponse>.Fail("计划ID不能为空"));

            var result = await _planningService.GetPlanAsync(planId);
            return Ok(ApiResponse<MppResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<MppResponse>.Fail("生产计划不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取生产计划详情时发生异常");
            return BadRequest(ApiResponse<MppResponse>.Fail("获取生产计划详情失败"));
        }
    }

    /// <summary>
    /// 发布生产计划
    /// </summary>
    [HttpPost("{planId}/publish")]
    [ProducesResponseType(typeof(ApiResponse<MppResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> PublishPlan(string planId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                return BadRequest(ApiResponse<MppResponse>.Fail("计划ID不能为空"));

            var result = await _planningService.PublishPlanAsync(planId, GetOperatorId());
            return Ok(ApiResponse<MppResponse>.Ok(result, "生产计划已发布"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布生产计划时发生异常");
            return BadRequest(ApiResponse<MppResponse>.Fail("发布生产计划失败"));
        }
    }

    /// <summary>
    /// 获取产能负载
    /// </summary>
    [HttpGet("{planId}/capacity-loads")]
    [ProducesResponseType(typeof(ApiResponse<List<CapacityLoadResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCapacityLoads(string planId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                return BadRequest(ApiResponse<List<CapacityLoadResponse>>.Fail("计划ID不能为空"));

            var result = await _planningService.GetCapacityLoadsAsync(planId);
            return Ok(ApiResponse<List<CapacityLoadResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取产能负载时发生异常");
            return BadRequest(ApiResponse<List<CapacityLoadResponse>>.Fail("获取产能负载失败"));
        }
    }

    /// <summary>
    /// 获取瓶颈工序
    /// </summary>
    [HttpGet("{planId}/bottlenecks")]
    [ProducesResponseType(typeof(ApiResponse<List<CapacityLoadResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBottlenecks(string planId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(planId))
                return BadRequest(ApiResponse<List<CapacityLoadResponse>>.Fail("计划ID不能为空"));

            var result = await _planningService.GetBottlenecksAsync(planId);
            return Ok(ApiResponse<List<CapacityLoadResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取瓶颈工序时发生异常");
            return BadRequest(ApiResponse<List<CapacityLoadResponse>>.Fail("获取瓶颈工序失败"));
        }
    }

    /// <summary>
    /// 产能模拟
    /// </summary>
    [HttpPost("simulate-capacity")]
    [ProducesResponseType(typeof(ApiResponse<CapacitySimulationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SimulateCapacity([FromBody] CapacitySimulationRequest request)
    {
        try
        {
            var result = await _planningService.SimulateCapacityAsync(request, GetOperatorId());
            return Ok(ApiResponse<CapacitySimulationResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "产能模拟时发生异常");
            return BadRequest(ApiResponse<CapacitySimulationResponse>.Fail("产能模拟失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
