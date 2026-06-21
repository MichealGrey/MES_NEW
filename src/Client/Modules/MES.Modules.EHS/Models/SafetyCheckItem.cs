namespace MES.Modules.EHS.Models;

/// <summary>
/// Safety checklist item for daily/weekly/monthly safety inspections.
/// </summary>
public class SafetyCheckItem
{
    public string Id { get; set; } = string.Empty;
    public string CheckCategory { get; set; } = string.Empty;
    public string CheckItemName { get; set; } = string.Empty;
    public string CheckLocation { get; set; } = string.Empty;
    public string Frequency { get; set; } = "Daily";
    public string Result { get; set; } = "Pass";
    public string Findings { get; set; } = string.Empty;
    public string InspectorName { get; set; } = string.Empty;
    public DateTime InspectionDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsOverdue { get; set; }
    public string RemediationPlan { get; set; } = string.Empty;
    public string RemediationStatus { get; set; } = "NotStarted";
}
