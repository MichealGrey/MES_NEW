using MES.Contracts.Common;
using MES.Contracts.Phase3;

namespace MES.Services.ProcessControl;

public interface IBinService
{
    Task<BinDefinitionResponse> CreateBinDefinitionAsync(CreateBinDefinitionRequest request, string operatorId);
    Task<BinDefinitionResponse> GetBinDefinitionAsync(string binId);
    Task<PagedResult<BinDefinitionResponse>> QueryBinDefinitionsAsync(BinQuery query);
    Task<List<BinStatisticsResponse>> GetBinSummaryAsync(BinSummaryQuery query);
}
