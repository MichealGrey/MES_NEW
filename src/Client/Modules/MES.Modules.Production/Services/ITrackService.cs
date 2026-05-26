using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface ITrackService
{
    Task<TrackValidationResult> ValidateTrackInAsync(TrackInRequest request);
    Task<TrackResult> TrackInAsync(TrackInRequest request);

    Task<TrackValidationResult> ValidateTrackOutAsync(TrackOutRequest request);
    Task<TrackResult> TrackOutAsync(TrackOutRequest request);

    Task<TrackResult> ForceTrackInAsync(TrackInRequest request, string reason);
    Task<TrackResult> ForceTrackOutAsync(TrackOutRequest request, string reason);
}
