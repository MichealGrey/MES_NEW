namespace MES.Modules.Quality.Models;

public class SpcRuleItem
{
    public int RuleNumber { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
