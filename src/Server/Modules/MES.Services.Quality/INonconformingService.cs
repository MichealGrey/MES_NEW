using MES.Contracts.Common;
using MES.Contracts.Phase1;

namespace MES.Services.Quality;

public interface INonconformingService
{
    Task<NonconformingRecordResponse> CreateRecordAsync(CreateNonconformingRequest request);
    Task<PagedResult<NonconformingRecordResponse>> GetRecordsAsync(NonconformingQuery query);
    Task<NonconformingRecordResponse> GetDetailAsync(string ncrId);
    Task<bool> IsolateAsync(string ncrId, string operatorId);
    Task<MrbReviewResponse> CreateMrbAsync(string ncrId, string createdBy);
    Task<MrbReviewResponse> VoteMrbAsync(string mrbId, MrbVoteRequest request);
    Task<bool> ExecuteDispositionAsync(string ncrId, DispositionRequest request);
    Task<bool> ReworkVerifyAsync(string ncrId, string result, string verifiedBy);
}
