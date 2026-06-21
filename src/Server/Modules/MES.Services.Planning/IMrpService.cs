using MES.Contracts.Common;
using MES.Contracts.Phase2;

namespace MES.Services.Planning;

public interface IMrpService
{
    Task<BomResponse> CreateBomAsync(CreateBomRequest request, string operatorId);
    Task<BomResponse> GetBomAsync(string bomId);
    Task<List<BomResponse>> GetBomsByProductAsync(string productId);
    Task<MrpCalculationResponse> CalculateMrpAsync(MrpCalculationRequest request, string operatorId);
    Task<PagedResult<MrpShortageWarningResponse>> GetShortageWarningsAsync(MrpQuery query);
    Task<MrpCalculationResponse> GetMrpCalculationAsync(string calculationId);
}
