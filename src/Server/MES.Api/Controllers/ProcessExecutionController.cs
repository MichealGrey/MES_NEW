using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Production;
using MES.Services.Production;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessExecutionController : ControllerBase
{
    private readonly IProcessExecutionService _service;
    private readonly ILogger<ProcessExecutionController> _logger;

    public ProcessExecutionController(
        IProcessExecutionService service,
        ILogger<ProcessExecutionController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// 开始工序
    /// </summary>
    [HttpPost("start-step")]
    public async Task<IActionResult> StartStep([FromBody] StepStartRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LotId))
            return BadRequest("批次ID不能为空");

        if (string.IsNullOrWhiteSpace(request.StepCode))
            return BadRequest("工序代码不能为空");

        if (string.IsNullOrWhiteSpace(request.EquipmentId))
            return BadRequest("设备ID不能为空");

        if (string.IsNullOrWhiteSpace(request.OperatorId))
            return BadRequest("操作员ID不能为空");

        var result = await _service.StartStepAsync(request);

        if (!result.Success)
            return BadRequest(new { result.OperationId, result.Message });

        _logger.LogInformation("工序启动成功: {LotId} - {StepCode}, 操作ID: {OperationId}",
            request.LotId, request.StepCode, result.OperationId);

        return Ok(result);
    }

    /// <summary>
    /// 完成工序
    /// </summary>
    [HttpPost("complete-step")]
    public async Task<IActionResult> CompleteStep([FromBody] StepCompleteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LotId))
            return BadRequest("批次ID不能为空");

        if (string.IsNullOrWhiteSpace(request.StepCode))
            return BadRequest("工序代码不能为空");

        if (string.IsNullOrWhiteSpace(request.OperatorId))
            return BadRequest("操作员ID不能为空");

        if (request.PassQty < 0 || request.FailQty < 0 || request.ScrapQty < 0)
            return BadRequest("数量不能为负数");

        var result = await _service.CompleteStepAsync(request);

        if (!result.Success)
            return BadRequest(new { result.OperationId, result.Message });

        _logger.LogInformation("工序完成成功: {LotId} - {StepCode}, 操作ID: {OperationId}",
            request.LotId, request.StepCode, result.OperationId);

        return Ok(result);
    }

    /// <summary>
    /// 记录工艺参数
    /// </summary>
    [HttpPost("parameters")]
    public async Task<IActionResult> RecordParameters([FromBody] RecordParametersRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LotId))
            return BadRequest("批次ID不能为空");

        if (string.IsNullOrWhiteSpace(request.StepCode))
            return BadRequest("工序代码不能为空");

        if (request.Parameters == null || request.Parameters.Count == 0)
            return BadRequest("参数列表不能为空");

        var result = await _service.RecordParametersAsync(
            request.LotId,
            request.StepCode,
            request.Parameters);

        if (!result.Success)
            return BadRequest(new { result.OperationId, result.Message });

        return Ok(result);
    }

    /// <summary>
    /// 查询工序状态
    /// </summary>
    [HttpGet("{lotId}/{stepCode}/status")]
    public async Task<IActionResult> GetStepStatus(string lotId, string stepCode)
    {
        try
        {
            var result = await _service.GetStepStatusAsync(lotId, stepCode);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 获取当前工序
    /// </summary>
    [HttpGet("{lotId}/current-step")]
    public async Task<IActionResult> GetCurrentStep(string lotId)
    {
        try
        {
            var result = await _service.GetCurrentStepAsync(lotId);

            if (result == null)
                return NotFound(new { message = $"批次 {lotId} 没有当前工序" });

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
