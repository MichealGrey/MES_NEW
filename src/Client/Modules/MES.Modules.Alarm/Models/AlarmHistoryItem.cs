namespace MES.Modules.Alarm.Models;

public class AlarmHistoryItem
{
    public string AlarmId { get; set; } = string.Empty;
    public string AlarmCode { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string Severity { get; set; } = "Minor";
    public string Text { get; set; } = string.Empty;
    public DateTime AlarmTime { get; set; }
    public DateTime? AckTime { get; set; }
    public string AckBy { get; set; } = string.Empty;
    public DateTime? CloseTime { get; set; }
    public string Status { get; set; } = "Closed";
}
