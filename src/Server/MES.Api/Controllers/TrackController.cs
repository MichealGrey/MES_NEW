using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Production;
using MES.Services.Production;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackController : ControllerBase
{
    private readonly IProductionService _service;

    public TrackController(IProductionService service) => _service = service;

    [HttpPost("trackin/validate")]
    public async Task<IActionResult> ValidateTrackIn([FromBody] TrackInRequest request)
        => Ok(await _service.ValidateTrackInAsync(request));

    [HttpPost("trackin")]
    public async Task<IActionResult> TrackIn([FromBody] TrackInRequest request)
        => Ok(await _service.TrackInAsync(request));

    [HttpPost("trackout/validate")]
    public async Task<IActionResult> ValidateTrackOut([FromBody] TrackOutRequest request)
        => Ok(await _service.ValidateTrackOutAsync(request));

    [HttpPost("trackout")]
    public async Task<IActionResult> TrackOut([FromBody] TrackOutRequest request)
        => Ok(await _service.TrackOutAsync(request));
}
