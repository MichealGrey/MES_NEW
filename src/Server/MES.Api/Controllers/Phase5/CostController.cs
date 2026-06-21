using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Services.Analytics;

namespace MES.Api.Controllers.Phase5;

/// <summary>
/// 成本分析 API
/// </summary>
[ApiController]
[Route("api/v5/[controller]")]
[Authorize]
public class CostController : ControllerBase
{
    private readonly ICostService _costService;
    private readonly ILogger<CostController> _logger;

    public CostController(ICostService costService, ILogger<CostController> logger)
    {
        _costService = costService;
        _logger = logger;
    }

    /// <summary>
    /// 获取产品成本分析
    /// </summary>
    [HttpGet("product-analysis")]
    [ProducesResponseType(typeof(ApiResponse<CostAnalysisResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductAnalysis([FromQuery] string productId, [FromQuery] DateOnly? startDate = null, [FromQuery] DateOnly? endDate = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(productId))
                return BadRequest(ApiResponse<CostAnalysisResponse>.Fail("产品ID不能为空"));

            var result = await _costService.GetProductAnalysisAsync(productId, startDate, endDate);
            return Ok(ApiResponse<CostAnalysisResponse>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取产品成本分析时发生异常");
            return BadRequest(ApiResponse<CostAnalysisResponse>.Fail("获取产品成本分析失败"));
        }
    }

    /// <summary>
    /// 获取工单成本分析
    /// </summary>
    [HttpGet("work-order-analysis")]
    [ProducesResponseType(typeof(ApiResponse<CostAnalysisResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CostAnalysisResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkOrderAnalysis([FromQuery] string workOrderId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(workOrderId))
                return BadRequest(ApiResponse<CostAnalysisResponse>.Fail("工单ID不能为空"));

            var result = await _costService.GetWorkOrderAnalysisAsync(workOrderId);
            return Ok(ApiResponse<CostAnalysisResponse>.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<CostAnalysisResponse>.Fail("未找到该工单的成本记录"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工单成本分析时发生异常");
            return BadRequest(ApiResponse<CostAnalysisResponse>.Fail("获取工单成本分析失败"));
        }
    }

    /// <summary>
    /// 获取成本差异分析
    /// </summary>
    [HttpGet("variance-analysis")]
    [ProducesResponseType(typeof(ApiResponse<List<CostRecordResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVarianceAnalysis([FromQuery] string workOrderId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(workOrderId))
                return BadRequest(ApiResponse<List<CostRecordResponse>>.Fail("工单ID不能为空"));

            var result = await _costService.GetVarianceAnalysisAsync(workOrderId);
            return Ok(ApiResponse<List<CostRecordResponse>>.Ok(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取成本差异分析时发生异常");
            return BadRequest(ApiResponse<List<CostRecordResponse>>.Fail("获取成本差异分析失败"));
        }
    }

    /// <summary>
    /// 计算成本
    /// </summary>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(ApiResponse<List<CostRecordResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<CostRecordResponse>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CalculateCosts([FromBody] CostCalculationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.WorkOrderId))
                return BadRequest(ApiResponse<List<CostRecordResponse>>.Fail("工单ID不能为空"));
            if (string.IsNullOrWhiteSpace(request.ProductId))
                return BadRequest(ApiResponse<List<CostRecordResponse>>.Fail("产品ID不能为空"));

            var result = await _costService.CalculateCostsAsync(request, GetOperatorId());
            return Ok(ApiResponse<List<CostRecordResponse>>.Ok(result, "成本计算成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "计算成本时发生异常");
            return BadRequest(ApiResponse<List<CostRecordResponse>>.Fail("成本计算失败"));
        }
    }

    /// <summary>
    /// 记录成本
    /// </summary>
    [HttpPost("record")]
    [ProducesResponseType(typeof(ApiResponse<CostRecordResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CostRecordResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordCost([FromBody] CostCalculationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.WorkOrderId))
                return BadRequest(ApiResponse<CostRecordResponse>.Fail("工单ID不能为空"));

            var result = await _costService.RecordCostAsync(request, GetOperatorId());
            return Ok(ApiResponse<CostRecordResponse>.Ok(result, "成本记录成功"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "记录成本时发生异常");
            return BadRequest(ApiResponse<CostRecordResponse>.Fail("记录成本失败"));
        }
    }

    private string GetOperatorId() => User.Identity?.Name ?? "system";
}
