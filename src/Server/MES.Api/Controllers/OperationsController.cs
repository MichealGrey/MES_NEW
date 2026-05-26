using Microsoft.AspNetCore.Mvc;
using MES.Services.Production;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperationsController : ControllerBase
{
    private readonly IProductionService _service;

    public OperationsController(IProductionService service) => _service = service;

    [HttpGet("history/{lotId}")]
    public async Task<IActionResult> GetHistory(string lotId)
        => Ok(await _service.GetOperationHistoryAsync(lotId));

    [HttpGet("audit/{entityType}/{entityId}")]
    public async Task<IActionResult> GetAudit(string entityType, string entityId)
        => Ok(await _service.GetAuditTrailAsync(entityType, entityId));

    [HttpGet("quantity/{lotId}")]
    public async Task<IActionResult> GetQuantitySummary(string lotId)
        => Ok(await _service.GetLotQuantitySummaryAsync(lotId));
}
