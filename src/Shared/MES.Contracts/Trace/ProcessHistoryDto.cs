namespace MES.Contracts.Trace;

public class ProcessHistoryDto
{
    public string LotId { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public DateTime TrackInTime { get; set; }
    public DateTime? TrackOutTime { get; set; }
    public string Result { get; set; } = string.Empty;
}
