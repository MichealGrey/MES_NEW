namespace MES.Contracts.Alarm;

public class AlarmDto
{
    public string AlarmId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string AlarmCode { get; set; } = string.Empty;
    public string Severity { get; set; } = "Minor";
    public string Text { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string Status { get; set; } = "Active";
}
