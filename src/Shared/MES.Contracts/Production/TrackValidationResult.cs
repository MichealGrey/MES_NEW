namespace MES.Contracts.Production;

/// <summary>
/// Public API DTO for TrackIn/TrackOut validation results.
/// </summary>
public class TrackValidationResult
{
    public bool IsValid { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string? CurrentStepCode { get; set; }
    public string? TargetStepCode { get; set; }
    public string RouteId { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public string? LotStatus { get; set; }
    public string? EquipmentId { get; set; }
}
