namespace MES.Contracts.Schedule;

public class DispatchSuggestionDto
{
    public string EquipmentId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string Reason { get; set; } = string.Empty;
}
