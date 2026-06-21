using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Quality;
using MES.Contracts.Common;
using MES.Services.Quality;

namespace MES.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpcController : ControllerBase
{
    private readonly ISpcMeasurementService _spcService;

    public SpcController(ISpcMeasurementService spcService)
    {
        _spcService = spcService;
    }

    [HttpGet]
    public async Task<ApiResponse<PagedResult<SpcMeasurementDto>>> GetMeasurements(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? lotId = null,
        [FromQuery] string? stepCode = null,
        [FromQuery] string? parameterName = null)
    {
        var result = await _spcService.GetPagedAsync(pageIndex, pageSize, lotId, stepCode, parameterName);
        return ApiResponse<PagedResult<SpcMeasurementDto>>.Ok(result);
    }

    [HttpGet("lot/{lotId}")]
    public async Task<ApiResponse<List<SpcMeasurementDto>>> GetByLot(string lotId)
    {
        var result = await _spcService.GetByLotAsync(lotId);
        return ApiResponse<List<SpcMeasurementDto>>.Ok(result);
    }

    [HttpGet("statistics")]
    public async Task<ApiResponse<SpcStatisticsDto>> GetStatistics(
        [FromQuery] string stepCode,
        [FromQuery] string parameterName,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var result = await _spcService.GetStatisticsAsync(stepCode, parameterName, from, to);
        return ApiResponse<SpcStatisticsDto>.Ok(result);
    }

    [HttpGet("out-of-control")]
    public async Task<ApiResponse<List<SpcMeasurementDto>>> GetOutOfControl([FromQuery] int count = 50)
    {
        var result = await _spcService.GetOutOfControlAsync(count);
        return ApiResponse<List<SpcMeasurementDto>>.Ok(result);
    }

    [HttpPost]
    public async Task<ApiResponse<SpcMeasurementDto>> CreateMeasurement([FromBody] CreateSpcMeasurementRequest request)
    {
        var operatorId = User.Identity?.Name ?? "System";
        var result = await _spcService.CreateAsync(request, operatorId);
        return ApiResponse<SpcMeasurementDto>.Ok(result, "Measurement recorded");
    }
}
