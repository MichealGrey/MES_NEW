using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IQuantityService
{
    Task<QuantityValidationResult> ValidateTrackOutQuantityAsync(TrackOutRequest request);
    Task RecordTransactionAsync(QuantityTransaction transaction);
    Task<LotQuantitySummary> GetLotQuantitySummaryAsync(string lotId);
}
