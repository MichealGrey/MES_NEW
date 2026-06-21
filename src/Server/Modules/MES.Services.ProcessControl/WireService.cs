using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.ProcessControl;

public class WireService : IWireService
{
    private readonly MesDbContext _context;
    private readonly ILogger<WireService> _logger;

    public WireService(MesDbContext context, ILogger<WireService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WireMaterialSwitchResponse> RecordWireSwitchAsync(WireMaterialSwitchRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var switchId = $"WIRE-SW-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var switchRecord = new WireMaterialSwitchRecord
        {
            SwitchId = switchId,
            LotId = request.LotId,
            StepCode = request.StepCode,
            StepSeq = request.StepSeq,
            EquipmentId = request.EquipmentId,
            OldWireMaterialId = request.OldWireMaterialId,
            OldWireMaterialName = request.OldWireMaterialName,
            OldWireLotNo = request.OldWireLotNo,
            OldWireDiameter = request.OldWireDiameter,
            NewWireMaterialId = request.NewWireMaterialId,
            NewWireMaterialName = request.NewWireMaterialName,
            NewWireLotNo = request.NewWireLotNo,
            NewWireDiameter = request.NewWireDiameter,
            WireSupplier = request.WireSupplier,
            SwitchReason = request.SwitchReason,
            OperatorId = operatorId,
            OperatorName = operatorId,
            SwitchTime = now,
            CreatedAt = now
        };

        _context.WireMaterialSwitchRecords.Add(switchRecord);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Recorded wire switch {SwitchId} for lot {LotId} by operator {OperatorId}",
            switchId, request.LotId, operatorId);

        return MapSwitchToResponse(switchRecord);
    }

    public async Task<long> RecordWireConsumptionAsync(string lotId, string stepCode, int stepSeq, string equipmentId, string wireMaterialId, string wireMaterialName, decimal consumedLength, string lengthUnit, int? bondCount, string operatorId)
    {
        var now = DateTime.UtcNow;

        var consumption = new WireConsumption
        {
            LotId = lotId,
            StepCode = stepCode,
            StepSeq = stepSeq,
            EquipmentId = equipmentId,
            WireMaterialId = wireMaterialId,
            WireMaterialName = wireMaterialName,
            ConsumedLength = consumedLength,
            LengthUnit = lengthUnit,
            BondCount = bondCount,
            AvgLengthPerBond = bondCount.HasValue && bondCount.Value > 0
                ? Math.Round(consumedLength / bondCount.Value, 4)
                : null,
            ConsumptionTime = now,
            CreatedAt = now
        };

        _context.WireConsumptions.Add(consumption);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Recorded wire consumption for lot {LotId}, wire {WireMaterialId}, length {ConsumedLength}",
            lotId, wireMaterialId, consumedLength);

        return consumption.ConsumptionId;
    }

    public async Task<PagedResult<WireConsumptionResponse>> QueryWireConsumptionsAsync(WireConsumptionQuery query)
    {
        var iqQuery = _context.WireConsumptions.AsQueryable();

        if (!string.IsNullOrEmpty(query.LotId))
            iqQuery = iqQuery.Where(w => w.LotId == query.LotId);
        if (!string.IsNullOrEmpty(query.StepCode))
            iqQuery = iqQuery.Where(w => w.StepCode == query.StepCode);
        if (!string.IsNullOrEmpty(query.WireMaterialId))
            iqQuery = iqQuery.Where(w => w.WireMaterialId == query.WireMaterialId);
        if (query.StartDate.HasValue)
            iqQuery = iqQuery.Where(w => w.ConsumptionTime >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            iqQuery = iqQuery.Where(w => w.ConsumptionTime <= query.EndDate.Value);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(w => w.ConsumptionTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(w => new WireConsumptionResponse
            {
                ConsumptionId = w.ConsumptionId,
                LotId = w.LotId,
                StepCode = w.StepCode,
                EquipmentId = w.EquipmentId,
                WireMaterialId = w.WireMaterialId,
                WireMaterialName = w.WireMaterialName,
                ConsumedLength = w.ConsumedLength,
                LengthUnit = w.LengthUnit,
                BondCount = w.BondCount,
                AvgLengthPerBond = w.AvgLengthPerBond,
                LossRate = w.LossRate,
                ConsumptionTime = w.ConsumptionTime
            })
            .ToListAsync();

        return new PagedResult<WireConsumptionResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    private static WireMaterialSwitchResponse MapSwitchToResponse(WireMaterialSwitchRecord entity) => new()
    {
        SwitchId = entity.SwitchId,
        LotId = entity.LotId,
        StepCode = entity.StepCode,
        EquipmentId = entity.EquipmentId,
        OldWireMaterialId = entity.OldWireMaterialId,
        OldWireMaterialName = entity.OldWireMaterialName,
        NewWireMaterialId = entity.NewWireMaterialId,
        NewWireMaterialName = entity.NewWireMaterialName,
        SwitchReason = entity.SwitchReason,
        OperatorName = entity.OperatorName,
        SwitchTime = entity.SwitchTime
    };
}
