using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class QuantityService : IQuantityService
{
    private readonly IQuantityTransactionRepository _qtyTxnRepo;

    public QuantityService(IQuantityTransactionRepository qtyTxnRepo)
    {
        _qtyTxnRepo = qtyTxnRepo;
    }

    public Task<QuantityValidationResult> ValidateTrackOutQuantityAsync(TrackOutRequest request)
    {
        var result = new QuantityValidationResult();
        var outputTotal = request.PassQty + request.FailQty + request.ScrapQty + request.ReworkQty + request.HoldQty + request.PendingQty;
        result.ExpectedTotal = request.InputQty;
        result.ActualTotal = outputTotal;

        if (request.InputQty <= 0)
        {
            result.AddError("投入数量必须大于 0");
        }

        if (outputTotal != request.InputQty)
        {
            result.AddError($"数量不平衡: 投入 {request.InputQty} ≠ 产出合计 {outputTotal} (良品 {request.PassQty} + 不良 {request.FailQty} + 报废 {request.ScrapQty} + 返工 {request.ReworkQty} + Hold {request.HoldQty} + 待处理 {request.PendingQty})");
        }

        if (request.PassQty < 0 || request.FailQty < 0 || request.ScrapQty < 0 ||
            request.ReworkQty < 0 || request.HoldQty < 0 || request.PendingQty < 0)
        {
            result.AddError("各分项数量不能为负数");
        }

        if (request.InputQty > 0)
        {
            var yield = (double)request.PassQty / request.InputQty * 100;
            if (yield < 90)
            {
                result.AddWarning($"良率过低: {yield:F1}% (良品 {request.PassQty} / 投入 {request.InputQty})");
            }
        }

        return Task.FromResult(result);
    }

    public async Task RecordTransactionAsync(QuantityTransaction transaction)
    {
        // This method is kept for backward compatibility with the interface.
        // The actual transaction recording is now done directly in TrackService.
    }

    public async Task<LotQuantitySummary> GetLotQuantitySummaryAsync(string lotId)
    {
        var summary = new LotQuantitySummary { LotId = lotId };
        var txns = await _qtyTxnRepo.GetByLotIdAsync(lotId);

        foreach (var txn in txns)
        {
            summary.TotalInput += txn.InputQty;
            summary.TotalPass += txn.PassQty;
            summary.TotalFail += txn.FailQty;
            summary.TotalScrap += txn.ScrapQty;
            summary.TotalRework += txn.ReworkQty;
            summary.TotalHold += txn.HoldQty;
            summary.TotalPending += txn.PendingQty;
            summary.Transactions.Add(new QuantityTransaction
            {
                LotId = txn.LotId,
                RouteId = txn.RouteId,
                StepSeq = txn.StepSeq,
                StepCode = txn.StepCode,
                StepName = txn.StepName,
                EquipmentId = txn.EquipmentId,
                InputQty = txn.InputQty,
                PassQty = txn.PassQty,
                FailQty = txn.FailQty,
                ScrapQty = txn.ScrapQty,
                ReworkQty = txn.ReworkQty,
                HoldQty = txn.HoldQty,
                PendingQty = txn.PendingQty,
                OperatorId = txn.OperatorId,
                OperatorName = txn.OperatorName,
                Timestamp = txn.Timestamp,
            });
        }

        summary.Transactions = summary.Transactions.OrderByDescending(t => t.Timestamp).ToList();
        return summary;
    }
}
