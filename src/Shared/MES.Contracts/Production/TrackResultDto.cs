namespace MES.Contracts.Production;

/// <summary>
/// Public API DTO representing the result of a TrackIn or TrackOut operation.
/// </summary>
public class TrackResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string? StepCode { get; set; }
    public string? NextStepCode { get; set; }
    public DateTime? Timestamp { get; set; }
}
