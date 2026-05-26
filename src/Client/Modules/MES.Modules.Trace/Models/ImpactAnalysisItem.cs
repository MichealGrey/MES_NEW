namespace MES.Modules.Trace.Models;

public class ImpactAnalysisItem
{
    public string LotId { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public DateTime ProcessTime { get; set; }
    public string RiskLevel { get; set; } = "Low";
}
