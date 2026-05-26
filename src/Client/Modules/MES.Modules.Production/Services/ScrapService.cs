using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class ScrapService : IScrapService
{
    private readonly ILotRepository _lotRepo;
    private readonly IScrapRecordRepository _scrapRepo;
    private const int ScrapThresholdPercent = 5;

    public ScrapService(ILotRepository lotRepo, IScrapRecordRepository scrapRepo)
    {
        _lotRepo = lotRepo;
        _scrapRepo = scrapRepo;
    }

    public async Task<ScrapRecord> RecordScrapAsync(string lotId, int scrapQty, string reason,
        string reasonCode, string operatorId, string operatorName)
    {
        var lot = await _lotRepo.GetByIdAsync(lotId);
        if (lot is null)
            throw new InvalidOperationException($"批次 {lotId} 不存在");

        if (scrapQty > lot.UnitCount)
            throw new InvalidOperationException($"报废数量 {scrapQty} 超过可用数量 {lot.UnitCount}");

        var requiresApproval = RequiresApproval(scrapQty, lot.UnitCount);

        var record = new ProdScrapRecord
        {
            ScrapId = Guid.NewGuid().ToString("N"),
            LotId = lotId,
            StepCode = lot.CurrentStepCode ?? string.Empty,
            StepSeq = lot.CurrentStepSeq,
            ScrapQty = scrapQty,
            ScrapReason = reason,
            ScrapReasonCode = reasonCode,
            OperatorId = operatorId,
            ScrapTime = DateTime.UtcNow,
            RequiresApproval = requiresApproval,
        };

        await _scrapRepo.AddAsync(record);

        lot.UnitCount -= scrapQty;
        lot.TotalScrapQty += scrapQty;
        lot.UpdatedAt = DateTime.UtcNow;

        if (lot.UnitCount <= 0)
        {
            lot.Status = "Scrapped";
        }

        await _lotRepo.UpdateAsync(lot);

        return MapToModel(record);
    }

    public bool RequiresApproval(int scrapQty, int totalQty)
    {
        if (totalQty <= 0) return false;
        var scrapRate = (double)scrapQty / totalQty * 100;
        return scrapRate >= ScrapThresholdPercent;
    }

    public async Task<List<ScrapRecord>> GetScrapRecordsAsync(string lotId)
    {
        var records = await _scrapRepo.GetByLotIdAsync(lotId);
        return records.Select(MapToModel).ToList();
    }

    private static ScrapRecord MapToModel(ProdScrapRecord e) => new()
    {
        ScrapId = e.ScrapId,
        LotId = e.LotId,
        StepCode = e.StepCode,
        StepSeq = e.StepSeq,
        ScrapQty = e.ScrapQty,
        ScrapReason = e.ScrapReason,
        ScrapReasonCode = e.ScrapReasonCode,
        OperatorId = e.OperatorId,
        ScrapTime = e.ScrapTime,
        RequiresApproval = e.RequiresApproval,
    };
}
