using MES.Contracts.Common;
using MES.Contracts.Quality;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Quality;

public interface ISpcMeasurementService
{
    Task<PagedResult<SpcMeasurementDto>> GetPagedAsync(int pageIndex, int pageSize, string? lotId = null, string? stepCode = null, string? parameterName = null);
    Task<List<SpcMeasurementDto>> GetByLotAsync(string lotId);
    Task<SpcMeasurementDto> CreateAsync(CreateSpcMeasurementRequest request, string operatorId);
    Task<SpcStatisticsDto> GetStatisticsAsync(string stepCode, string parameterName, DateTime? from = null, DateTime? to = null);
    Task<List<SpcMeasurementDto>> GetOutOfControlAsync(int count = 50);
}

public class SpcMeasurementService : ISpcMeasurementService
{
    private readonly MesDbContext _context;

    public SpcMeasurementService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<SpcMeasurementDto>> GetPagedAsync(int pageIndex, int pageSize, string? lotId = null, string? stepCode = null, string? parameterName = null)
    {
        var query = _context.SpcMeasurements.AsQueryable();

        if (!string.IsNullOrEmpty(lotId))
            query = query.Where(x => x.LotId == lotId);

        if (!string.IsNullOrEmpty(stepCode))
            query = query.Where(x => x.StepCode == stepCode);

        if (!string.IsNullOrEmpty(parameterName))
            query = query.Where(x => x.ParameterName == parameterName);

        query = query.OrderByDescending(x => x.MeasuredAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<SpcMeasurementDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<List<SpcMeasurementDto>> GetByLotAsync(string lotId)
    {
        var items = await _context.SpcMeasurements
            .Where(x => x.LotId == lotId)
            .OrderBy(x => x.MeasuredAt)
            .ToListAsync();

        return items.Select(MapToDto).ToList();
    }

    public async Task<SpcMeasurementDto> CreateAsync(CreateSpcMeasurementRequest request, string operatorId)
    {
        var measurement = new SpcMeasurement
        {
            LotId = request.LotId,
            StepCode = request.StepCode,
            ParameterName = request.ParameterName,
            MeasuredValue = request.MeasuredValue,
            Usl = request.Usl,
            Lsl = request.Lsl,
            TargetValue = request.TargetValue,
            EquipmentId = request.EquipmentId,
            OperatorId = operatorId,
            MeasuredAt = DateTime.UtcNow,
            IsOutOfControl = false
        };

        // Check out-of-control
        if (request.Usl.HasValue && request.MeasuredValue > request.Usl.Value)
            measurement.IsOutOfControl = true;
        if (request.Lsl.HasValue && request.MeasuredValue < request.Lsl.Value)
            measurement.IsOutOfControl = true;

        await _context.SpcMeasurements.AddAsync(measurement);
        await _context.SaveChangesAsync();

        return MapToDto(measurement);
    }

    public async Task<SpcStatisticsDto> GetStatisticsAsync(string stepCode, string parameterName, DateTime? from = null, DateTime? to = null)
    {
        var query = _context.SpcMeasurements
            .Where(x => x.StepCode == stepCode && x.ParameterName == parameterName && x.MeasuredValue.HasValue);

        if (from.HasValue)
            query = query.Where(x => x.MeasuredAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.MeasuredAt <= to.Value);

        var data = await query.ToListAsync();

        if (data.Count == 0)
        {
            return new SpcStatisticsDto { ParameterName = parameterName };
        }

        var values = data.Where(x => x.MeasuredValue.HasValue).Select(x => x.MeasuredValue!.Value).ToList();
        var avg = values.Average();
        var min = values.Min();
        var max = values.Max();
        var variance = values.Average(v => Math.Pow((double)(v - avg), 2));
        var stdDev = (decimal)Math.Sqrt((double)variance);

        // Cpk calculation
        var sample = data.FirstOrDefault(x => x.Usl.HasValue && x.Lsl.HasValue);
        var cpk = 0m;
        if (sample != null && stdDev > 0)
        {
            var cpu = (sample.Usl!.Value - avg) / (3 * stdDev);
            var cpl = (avg - sample.Lsl!.Value) / (3 * stdDev);
            cpk = (decimal)Math.Min((double)cpu, (double)cpl);
        }

        return new SpcStatisticsDto
        {
            ParameterName = parameterName,
            Count = data.Count,
            Average = avg,
            Min = min,
            Max = max,
            StdDev = stdDev,
            Cpk = (decimal)cpk,
            OutOfControlCount = data.Count(x => x.IsOutOfControl)
        };
    }

    public async Task<List<SpcMeasurementDto>> GetOutOfControlAsync(int count = 50)
    {
        var items = await _context.SpcMeasurements
            .Where(x => x.IsOutOfControl)
            .OrderByDescending(x => x.MeasuredAt)
            .Take(count)
            .ToListAsync();

        return items.Select(MapToDto).ToList();
    }

    private static SpcMeasurementDto MapToDto(SpcMeasurement m) => new()
    {
        Id = m.Id,
        LotId = m.LotId,
        StepCode = m.StepCode,
        ParameterName = m.ParameterName,
        MeasuredValue = m.MeasuredValue,
        Usl = m.Usl,
        Lsl = m.Lsl,
        TargetValue = m.TargetValue,
        EquipmentId = m.EquipmentId,
        OperatorId = m.OperatorId,
        MeasuredAt = m.MeasuredAt,
        IsOutOfControl = m.IsOutOfControl
    };
}
