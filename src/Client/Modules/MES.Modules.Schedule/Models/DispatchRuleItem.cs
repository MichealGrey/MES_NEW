namespace MES.Modules.Schedule.Models;

public class DispatchRuleItem
{
    public string Id { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int Priority { get; set; }
}
