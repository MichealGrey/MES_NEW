namespace MES.Modules.Production.Models;

public class YieldRule
{
    public string RuleId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public double YieldThreshold { get; set; }
    public string ActionType { get; set; } = "AutoHold";
    public string NotifyRole { get; set; } = "QA";
    public bool IsActive { get; set; } = true;
}
