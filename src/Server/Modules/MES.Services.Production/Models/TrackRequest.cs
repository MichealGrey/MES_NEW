namespace MES.Services.Production.Models;

/// <summary>
/// Internal server-side model for TrackIn request (used by the service layer).
/// See MES.Contracts.Production.TrackInRequest for the public API DTO.
/// </summary>
public class TrackRequest
{
    public string LotId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public string? RecipeId { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// Internal server-side result for TrackIn/TrackOut operations.
/// </summary>
public class TrackResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string? StepCode { get; set; }
    public string? NextStepCode { get; set; }
    public DateTime? Timestamp { get; set; }
}
