using Microsoft.AspNetCore.Mvc;
using MES.Services.Production;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IProductionService _service;

    public RoutesController(IProductionService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllRoutesAsync());

    [HttpGet("{routeId}")]
    public async Task<IActionResult> Get(string routeId, [FromQuery] string? version = null)
    {
        var route = await _service.GetRouteAsync(routeId, version);
        return route is null ? NotFound() : Ok(route);
    }

    [HttpGet("{routeId}/steps")]
    public async Task<IActionResult> GetSteps(string routeId, [FromQuery] string? version = null)
        => Ok(await _service.GetRouteStepsAsync(routeId, version));
}
