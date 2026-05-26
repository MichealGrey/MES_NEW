namespace MES.Modules.Production.Models;

public class AlarmRule
{
    public string RuleId { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string Severity { get; set; } = "Warning";
    public int? ThresholdMinutes { get; set; }
    public double? ThresholdYield { get; set; }
    public int? ThresholdQty { get; set; }
    public string NotifyRole { get; set; } = "Supervisor";
    public string? Condition { get; set; }
}
