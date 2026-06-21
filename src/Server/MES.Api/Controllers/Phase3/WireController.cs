using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Services.ProcessControl;

namespace MES.Api.Controllers.Phase3;

/// <summary>
/// 线材管控 API
/// </summary>
[ApiController]
[Route("api/v3/[controller]")]
[Authorize]
public class WireController : ControllerBase
{
    private readonly IWireService _wireService;
    private readonly ILogger<WireController> _logger;

    public WireController(IWireService wireService, ILogger<WireController> logger)
    {
        _wireService = wireService;
        _logger = logger;
    }

    /// <summary>
    /// 记录线材切换
    /// </summary>
    [HttpPost("switch")]
    [ProducesResponseType(typeof(ApiResponse<WireMaterialSwitchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<WireMaterialSwitchResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordWireSwitch([FromBody] WireMaterialSwitchRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LotId))
                return BadRequest(ApiResponse<WireMaterialSwitchResponse>.Fail("批次ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.StepCode))
                return BadRequest(ApiResponse<WireMaterialSwitchResponse>.Fail("工序代码不能为空"));
            if (string.IsNullOrWhiteSpace(request.OldWireMaterialId))
                return BadRequest(ApiResponse<WireMaterialSwitchResponse>.Fail("旧线材物料ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.NewWireMaterialId))
                return BadRequest(ApiResponse<WireMaterialSwitchResponse>.Fail("新线材物料ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.SwitchReason))
                return BadRequest(ApiResponse<WireMaterialSwitchResponse>.Fail("切换原因不能为空"));

            var result = await _wireService.RecordWireSwitchAsync(request, GetOperatorId());
            return Ok(ApiResponse<WireMaterialSwitchResponse>.Ok(result, "线材切换记录成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<WireMaterialSwitchResponse>.Fail("相关记录不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "记录线材切换时发生异常");
            return BadRequest(ApiResponse<WireMaterialSwitchResponse>.Fail("记录线材切换失败"));
        }
    }

    /// <summary>
    /// 记录线材消耗
    /// </summary>
    [HttpPost("consumption")]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordWireConsumption([FromBody] RecordWireConsumptionRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LotId))
                return BadRequest(ApiResponse<long>.Fail("批次ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.StepCode))
                return BadRequest(ApiResponse<long>.Fail("工序代码不能为空"));
            if (string.IsNullOrWhiteSpace(request.WireMaterialId))
                return BadRequest(ApiResponse<long>.Fail("线材物料ID不能为空"));
            if (request.ConsumedLength <= 0)
                return BadRequest(ApiResponse<long>.Fail("消耗长度必须大于0"));

            var result = await _wireService.RecordWireConsumptionAsync(
                request.LotId,
                request.StepCode,
                request.StepSeq,
                request.EquipmentId ?? string.Empty,
                request.WireMaterialId,
                request.WireMaterialName ?? string.Empty,
                request.ConsumedLength,
                request.LengthUnit ?? "mm",
                request.BondCount,
                GetOperatorId());

            return Ok(ApiResponse<long>.Ok(result, "线材消耗记录成功"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<long>.Fail("相关记录不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "记录线材消耗时发生异常");
            return BadRequest(ApiResponse<long>.Fail("记录线材消耗失败"));
        }
    }

    /// <summary>
    /// 查询线材消耗记录（分页）
    /// </summary>
    [HttpGet("consumptions")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<WireConsumptionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryWireConsumptions([FromQuery] WireConsumptionQuery query)
    {
        try
        {
            var result = await _wireService.QueryWireConsumptionsAsync(query);
            return Ok(ApiResponse<PagedResult<WireConsumptionResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询线材消耗记录时发生异常");
            return BadRequest(ApiResponse<PagedResult<WireConsumptionResponse>>.Fail("查询线材消耗记录失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}

/// <summary>
/// 记录线材消耗请求体
/// </summary>
public class RecordWireConsumptionRequest
{
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string WireMaterialId { get; set; } = string.Empty;
    public string? WireMaterialName { get; set; }
    public decimal ConsumedLength { get; set; }
    public string? LengthUnit { get; set; }
    public int? BondCount { get; set; }
}
