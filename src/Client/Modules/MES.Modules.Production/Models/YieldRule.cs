namespace MES.Modules.Production.Models;

public class YieldRule
{
    public string RuleId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    // Phase 4 properties
    public double WarningYield { get; set; }
    public double AlarmYield { get; set; }
    public double HoldYield { get; set; }
    // Backward-compatible properties for existing YieldService
    public double YieldThreshold { get; set; }
    public string ActionType { get; set; } = "AutoHold";
    public string NotifyRole { get; set; } = "QA";
    public bool IsActive { get; set; } = true;
}
