namespace MES.Modules.Quality.Models;

public class SpcRuleItem
{
    public string Id { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int ViolationCount { get; set; }
    public DateTime? LastTrigger { get; set; }
}
