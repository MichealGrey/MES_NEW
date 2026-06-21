namespace MES.Modules.Alarm.Models;

public class ActiveAlarmItem
{
    public string AlarmId { get; set; } = string.Empty;
    public string AlarmCode { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string Severity { get; set; } = "Minor";
    public string Text { get; set; } = string.Empty;
    public DateTime AlarmTime { get; set; }
    public string Duration { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string AckBy { get; set; } = string.Empty;
}
