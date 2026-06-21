namespace MES.Modules.Alarm.Models;

public class AlarmRuleItem
{
    public string AlarmCode { get; set; } = string.Empty;
    public string AlarmName { get; set; } = string.Empty;
    public string EquipmentType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Minor";
    public bool AutoHold { get; set; }
    public bool Enabled { get; set; } = true;
    public string Description { get; set; } = string.Empty;
}
