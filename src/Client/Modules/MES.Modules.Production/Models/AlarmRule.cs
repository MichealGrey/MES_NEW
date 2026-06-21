namespace MES.Modules.Production.Models;

public class AlarmRule
{
    public string RuleId { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string NotifyRoles { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
