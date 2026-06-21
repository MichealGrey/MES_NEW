using MES.Contracts.Common;
using MES.Contracts.Phase1;

namespace MES.Services.Quality;

public interface IFirstArticleService
{
    Task<FirstArticleResponse> CreateAsync(CreateFirstArticleRequest request);
    Task<PagedResult<FirstArticleResponse>> GetFirstArticlesAsync(FirstArticleQuery query);
    Task<FirstArticleResponse> GetDetailAsync(string faId);
    Task<FirstArticleResponse> ExecuteAsync(ExecuteFirstArticleRequest request);
    Task<FirstArticleResponse> ConfirmAsync(ConfirmFirstArticleRequest request);
    Task<bool> RejectAsync(RejectFirstArticleRequest request);
    Task<BondPullTestResponse> RecordBondPullTestAsync(BondPullTestRequest request);
    Task<List<BondPullTestResponse>> GetBondPullTestsAsync(string lotId, string? workOrderId = null);
}
