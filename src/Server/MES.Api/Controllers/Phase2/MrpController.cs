using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase2;
using MES.Services.Planning;

namespace MES.Api.Controllers.Phase2;

/// <summary>
/// BOM与MRP物料需求计划 API
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
[Authorize]
public class MrpController : ControllerBase
{
    private readonly IMrpService _mrpService;
    private readonly ILogger<MrpController> _logger;

    public MrpController(IMrpService mrpService, ILogger<MrpController> logger)
    {
        _mrpService = mrpService;
        _logger = logger;
    }

    /// <summary>
    /// 创建BOM
    /// </summary>
    [HttpPost("bom")]
    [ProducesResponseType(typeof(ApiResponse<BomResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<BomResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBom([FromBody] CreateBomRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.ProductId))
                return BadRequest(ApiResponse<BomResponse>.Fail("产品ID不能为空"));
            if (request.Items == null || request.Items.Count == 0)
                return BadRequest(ApiResponse<BomResponse>.Fail("BOM子件不能为空"));

            var result = await _mrpService.CreateBomAsync(request, GetOperatorId());
            return Ok(ApiResponse<BomResponse>.Ok(result, "BOM创建成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建BOM时发生异常");
            return BadRequest(ApiResponse<BomResponse>.Fail("创建BOM失败"));
        }
    }

    /// <summary>
    /// 获取BOM详情
    /// </summary>
    [HttpGet("bom/{bomId}")]
    [ProducesResponseType(typeof(ApiResponse<BomResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBom(string bomId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(bomId))
                return BadRequest(ApiResponse<BomResponse>.Fail("BOM ID不能为空"));

            var result = await _mrpService.GetBomAsync(bomId);
            return Ok(ApiResponse<BomResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<BomResponse>.Fail("BOM不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取BOM详情时发生异常");
            return BadRequest(ApiResponse<BomResponse>.Fail("获取BOM详情失败"));
        }
    }

    /// <summary>
    /// 查询产品的BOM列表
    /// </summary>
    [HttpGet("bom/product/{productId}")]
    [ProducesResponseType(typeof(ApiResponse<List<BomResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBomsByProduct(string productId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(productId))
                return BadRequest(ApiResponse<List<BomResponse>>.Fail("产品ID不能为空"));

            var result = await _mrpService.GetBomsByProductAsync(productId);
            return Ok(ApiResponse<List<BomResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询产品BOM列表时发生异常");
            return BadRequest(ApiResponse<List<BomResponse>>.Fail("查询BOM列表失败"));
        }
    }

    /// <summary>
    /// 执行MRP计算
    /// </summary>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(ApiResponse<MrpCalculationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MrpCalculationResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CalculateMrp([FromBody] MrpCalculationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.PlanId))
                return BadRequest(ApiResponse<MrpCalculationResponse>.Fail("计划ID不能为空"));

            var result = await _mrpService.CalculateMrpAsync(request, GetOperatorId());
            return Ok(ApiResponse<MrpCalculationResponse>.Ok(result, "MRP计算完成"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MRP计算时发生异常");
            return BadRequest(ApiResponse<MrpCalculationResponse>.Fail("MRP计算失败"));
        }
    }

    /// <summary>
    /// 获取MRP计算结果
    /// </summary>
    [HttpGet("calculation/{calculationId}")]
    [ProducesResponseType(typeof(ApiResponse<MrpCalculationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMrpCalculation(string calculationId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(calculationId))
                return BadRequest(ApiResponse<MrpCalculationResponse>.Fail("计算ID不能为空"));

            var result = await _mrpService.GetMrpCalculationAsync(calculationId);
            return Ok(ApiResponse<MrpCalculationResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<MrpCalculationResponse>.Fail("MRP计算记录不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取MRP计算结果时发生异常");
            return BadRequest(ApiResponse<MrpCalculationResponse>.Fail("获取MRP计算结果失败"));
        }
    }

    /// <summary>
    /// 查询物料短缺预警（分页）
    /// </summary>
    [HttpGet("shortage-warnings")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MrpShortageWarningResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShortageWarnings([FromQuery] MrpQuery query)
    {
        try
        {
            var result = await _mrpService.GetShortageWarningsAsync(query);
            return Ok(ApiResponse<PagedResult<MrpShortageWarningResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询物料短缺预警时发生异常");
            return BadRequest(ApiResponse<PagedResult<MrpShortageWarningResponse>>.Fail("查询短缺预警失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
