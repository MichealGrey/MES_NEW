namespace MES.Modules.Schedule.Models;

public class DispatchRuleItem
{
    public string RuleName { get; set; } = string.Empty;
    public double Weight { get; set; }
    public bool IsEnabled { get; set; }
    public string Description { get; set; } = string.Empty;
}
