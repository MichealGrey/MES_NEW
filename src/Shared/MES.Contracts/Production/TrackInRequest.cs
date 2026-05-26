namespace MES.Contracts.Production;

/// <summary>
/// Public API DTO for TrackIn requests.
/// </summary>
public class TrackInRequest
{
    public string LotId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public string? RecipeId { get; set; }
    public string? Remark { get; set; }
}
