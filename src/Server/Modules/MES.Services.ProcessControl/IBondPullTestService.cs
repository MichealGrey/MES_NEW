using MES.Contracts.Common;
using MES.Contracts.Phase3;

namespace MES.Services.ProcessControl;

public interface IBondPullTestService
{
    Task<BondPullTestResponse> CreateTestAsync(CreateBondPullTestRequest request, string operatorId);
    Task<BondPullTestResponse> GetTestAsync(string testId);
    Task<PagedResult<BondPullTestResponse>> QueryTestsAsync(BondPullTestQuery query);
}
