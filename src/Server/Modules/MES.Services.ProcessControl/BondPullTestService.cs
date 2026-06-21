using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.ProcessControl;

public class BondPullTestService : IBondPullTestService
{
    private readonly MesDbContext _context;
    private readonly ILogger<BondPullTestService> _logger;

    public BondPullTestService(MesDbContext context, ILogger<BondPullTestService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<BondPullTestResponse> CreateTestAsync(CreateBondPullTestRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var testId = $"BPT-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var testRecord = new BondPullTestRecord
        {
            TestId = testId,
            LotId = request.LotId,
            StepCode = request.StepCode,
            EquipmentId = request.EquipmentId,
            BondWireType = request.WireMaterialName,
            SampleSize = request.SampleSize,
            StandardValue = request.AvgPullForce?.ToString(),
            UpperLimit = request.SpecUpperLimit?.ToString(),
            LowerLimit = request.SpecLowerLimit?.ToString(),
            Unit = request.Unit,
            AvgValue = request.AvgPullForce,
            MinValue = request.MinPullForce,
            MaxValue = request.MaxPullForce,
            Judgment = request.TestResult,
            TesterId = operatorId,
            TesterName = operatorId,
            TestTime = now
        };

        _context.BondPullTestRecords.Add(testRecord);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created bond pull test {TestId} for lot {LotId} with result {Result}",
            testId, request.LotId, request.TestResult);

        return MapToResponse(testRecord);
    }

    public async Task<BondPullTestResponse> GetTestAsync(string testId)
    {
        var testRecord = await _context.BondPullTestRecords
            .FirstOrDefaultAsync(t => t.TestId == testId);

        if (testRecord == null)
            throw new KeyNotFoundException($"Bond pull test {testId} not found");

        return MapToResponse(testRecord);
    }

    public async Task<PagedResult<BondPullTestResponse>> QueryTestsAsync(BondPullTestQuery query)
    {
        var iqQuery = _context.BondPullTestRecords.AsQueryable();

        if (!string.IsNullOrEmpty(query.LotId))
            iqQuery = iqQuery.Where(t => t.LotId == query.LotId);
        if (!string.IsNullOrEmpty(query.StepCode))
            iqQuery = iqQuery.Where(t => t.StepCode == query.StepCode);
        if (!string.IsNullOrEmpty(query.TestResult))
            iqQuery = iqQuery.Where(t => t.Judgment == query.TestResult);
        if (query.StartDate.HasValue)
            iqQuery = iqQuery.Where(t => t.TestTime >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            iqQuery = iqQuery.Where(t => t.TestTime <= query.EndDate.Value);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(t => t.TestTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(t => MapToResponse(t))
            .ToListAsync();

        return new PagedResult<BondPullTestResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    private static BondPullTestResponse MapToResponse(BondPullTestRecord entity) => new()
    {
        TestId = entity.TestId,
        LotId = entity.LotId,
        StepCode = entity.StepCode ?? string.Empty,
        EquipmentId = entity.EquipmentId,
        SampleSize = entity.SampleSize,
        MinPullForce = entity.MinValue,
        MaxPullForce = entity.MaxValue,
        AvgPullForce = entity.AvgValue,
        SpecLowerLimit = decimal.TryParse(entity.LowerLimit, out var low) ? low : null,
        SpecUpperLimit = decimal.TryParse(entity.UpperLimit, out var up) ? up : null,
        Unit = entity.Unit,
        TestResult = entity.Judgment,
        TesterName = entity.TesterName,
        TestTime = entity.TestTime
    };
}
