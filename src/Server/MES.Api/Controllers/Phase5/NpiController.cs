using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Services.Analytics;

namespace MES.Api.Controllers.Phase5;

/// <summary>
/// NPI 项目管理 API
/// </summary>
[ApiController]
[Route("api/v5/[controller]")]
[Authorize]
public class NpiController : ControllerBase
{
    private readonly INpiService _npiService;
    private readonly ILogger<NpiController> _logger;

    public NpiController(INpiService npiService, ILogger<NpiController> logger)
    {
        _npiService = npiService;
        _logger = logger;
    }

    /// <summary>
    /// 创建NPI项目
    /// </summary>
    [HttpPost("projects")]
    [ProducesResponseType(typeof(ApiResponse<NpiProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NpiProjectResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject([FromBody] CreateNpiProjectRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ProjectCode))
                return BadRequest(ApiResponse<NpiProjectResponse>.Fail("项目代码不能为空"));
            if (string.IsNullOrWhiteSpace(request.ProjectName))
                return BadRequest(ApiResponse<NpiProjectResponse>.Fail("项目名称不能为空"));

            var result = await _npiService.CreateProjectAsync(request, GetOperatorId());
            return Ok(ApiResponse<NpiProjectResponse>.Ok(result, "NPI项目创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建NPI项目时发生异常");
            return BadRequest(ApiResponse<NpiProjectResponse>.Fail("创建NPI项目失败"));
        }
    }

    /// <summary>
    /// 查询NPI项目列表
    /// </summary>
    [HttpGet("projects")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<NpiProjectResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryProjects([FromQuery] NpiProjectQuery query)
    {
        try
        {
            var result = await _npiService.QueryProjectsAsync(query);
            return Ok(ApiResponse<PagedResult<NpiProjectResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询NPI项目列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<NpiProjectResponse>>.Fail("查询NPI项目列表失败"));
        }
    }

    /// <summary>
    /// 获取NPI项目详情
    /// </summary>
    [HttpGet("projects/{projectId}")]
    [ProducesResponseType(typeof(ApiResponse<NpiProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NpiProjectResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject(string projectId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return BadRequest(ApiResponse<NpiProjectResponse>.Fail("项目ID不能为空"));

            var result = await _npiService.GetProjectAsync(projectId);
            return Ok(ApiResponse<NpiProjectResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<NpiProjectResponse>.Fail("NPI项目不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取NPI项目详情时发生异常");
            return BadRequest(ApiResponse<NpiProjectResponse>.Fail("获取NPI项目详情失败"));
        }
    }

    /// <summary>
    /// 获取NPI项目阶段列表
    /// </summary>
    [HttpGet("projects/{projectId}/stages")]
    [ProducesResponseType(typeof(ApiResponse<List<NpiStageResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStages(string projectId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return BadRequest(ApiResponse<List<NpiStageResponse>>.Fail("项目ID不能为空"));

            var result = await _npiService.GetStagesAsync(projectId);
            return Ok(ApiResponse<List<NpiStageResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取NPI项目阶段列表时发生异常");
            return BadRequest(ApiResponse<List<NpiStageResponse>>.Fail("获取阶段列表失败"));
        }
    }

    /// <summary>
    /// 执行试产
    /// </summary>
    [HttpPost("projects/{projectId}/trial-run")]
    [ProducesResponseType(typeof(ApiResponse<NpiStageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NpiStageResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExecuteTrialRun(string projectId, [FromBody] TrialRunRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return BadRequest(ApiResponse<NpiStageResponse>.Fail("项目ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.LotId))
                return BadRequest(ApiResponse<NpiStageResponse>.Fail("批次ID不能为空"));

            var result = await _npiService.ExecuteTrialRunAsync(projectId, request, GetOperatorId());
            return Ok(ApiResponse<NpiStageResponse>.Ok(result, "试产执行成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<NpiStageResponse>.Fail("NPI项目不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行试产时发生异常");
            return BadRequest(ApiResponse<NpiStageResponse>.Fail("执行试产失败"));
        }
    }

    /// <summary>
    /// 评审NPI阶段
    /// </summary>
    [HttpPost("projects/{projectId}/review")]
    [ProducesResponseType(typeof(ApiResponse<NpiStageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NpiStageResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Review(string projectId, [FromBody] NpiReviewRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return BadRequest(ApiResponse<NpiStageResponse>.Fail("项目ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.StageId))
                return BadRequest(ApiResponse<NpiStageResponse>.Fail("阶段ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.Result))
                return BadRequest(ApiResponse<NpiStageResponse>.Fail("评审结果不能为空"));

            var result = await _npiService.ReviewAsync(projectId, request, GetOperatorId());
            return Ok(ApiResponse<NpiStageResponse>.Ok(result, "评审完成"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<NpiStageResponse>.Fail("NPI阶段不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "评审NPI阶段时发生异常");
            return BadRequest(ApiResponse<NpiStageResponse>.Fail("评审失败"));
        }
    }

    /// <summary>
    /// 转量产
    /// </summary>
    [HttpPost("projects/{projectId}/transfer-mass-production")]
    [ProducesResponseType(typeof(ApiResponse<NpiProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<NpiProjectResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TransferToMassProduction(string projectId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(projectId))
                return BadRequest(ApiResponse<NpiProjectResponse>.Fail("项目ID不能为空"));

            var result = await _npiService.TransferToMassProductionAsync(projectId, GetOperatorId());
            return Ok(ApiResponse<NpiProjectResponse>.Ok(result, "转量产成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<NpiProjectResponse>.Fail("NPI项目不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "转量产时发生异常");
            return BadRequest(ApiResponse<NpiProjectResponse>.Fail("转量产失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
