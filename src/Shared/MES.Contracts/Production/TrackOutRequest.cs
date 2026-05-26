namespace MES.Contracts.Production;

/// <summary>
/// Public API DTO for TrackOut requests.
/// </summary>
public class TrackOutRequest
{
    public string LotId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public int QtyPass { get; set; }
    public int QtyFail { get; set; }
    public string? FailReason { get; set; }
    public string? Remark { get; set; }
}
