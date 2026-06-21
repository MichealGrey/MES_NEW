using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Services.ProcessControl;

namespace MES.Api.Controllers.Phase3;

/// <summary>
/// 焊线拉力测试 API
/// </summary>
[ApiController]
[Route("api/v3/[controller]")]
[Authorize]
public class BondPullTestController : ControllerBase
{
    private readonly IBondPullTestService _bondPullTestService;
    private readonly ILogger<BondPullTestController> _logger;

    public BondPullTestController(IBondPullTestService bondPullTestService, ILogger<BondPullTestController> logger)
    {
        _bondPullTestService = bondPullTestService;
        _logger = logger;
    }

    /// <summary>
    /// 创建焊线拉力测试
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BondPullTestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BondPullTestResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTest([FromBody] CreateBondPullTestRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LotId))
                return BadRequest(ApiResponse<BondPullTestResponse>.Fail("批次ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.StepCode))
                return BadRequest(ApiResponse<BondPullTestResponse>.Fail("工序代码不能为空"));
            if (request.SampleSize <= 0)
                return BadRequest(ApiResponse<BondPullTestResponse>.Fail("样本数量必须大于0"));

            var result = await _bondPullTestService.CreateTestAsync(request, GetOperatorId());
            return Ok(ApiResponse<BondPullTestResponse>.Ok(result, "焊线拉力测试创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建焊线拉力测试时发生异常");
            return BadRequest(ApiResponse<BondPullTestResponse>.Fail("创建焊线拉力测试失败"));
        }
    }

    /// <summary>
    /// 查询焊线拉力测试列表（分页）
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<BondPullTestResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryTests([FromQuery] BondPullTestQuery query)
    {
        try
        {
            var result = await _bondPullTestService.QueryTestsAsync(query);
            return Ok(ApiResponse<PagedResult<BondPullTestResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询焊线拉力测试列表时发生异常");
            return BadRequest(ApiResponse<PagedResult<BondPullTestResponse>>.Fail("查询焊线拉力测试列表失败"));
        }
    }

    /// <summary>
    /// 获取焊线拉力测试详情
    /// </summary>
    [HttpGet("{testId}")]
    [ProducesResponseType(typeof(ApiResponse<BondPullTestResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BondPullTestResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTest(string testId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(testId))
                return BadRequest(ApiResponse<BondPullTestResponse>.Fail("测试ID不能为空"));

            var result = await _bondPullTestService.GetTestAsync(testId);
            return Ok(ApiResponse<BondPullTestResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<BondPullTestResponse>.Fail("焊线拉力测试不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取焊线拉力测试详情时发生异常");
            return BadRequest(ApiResponse<BondPullTestResponse>.Fail("获取焊线拉力测试详情失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
