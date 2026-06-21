namespace MES.Modules.Trace.Models;

public class ImpactAnalysisItem
{
    public string AffectedLotId { get; set; } = string.Empty;
    public string RootCause { get; set; } = string.Empty;
    public string ImpactType { get; set; } = string.Empty;
    public int AffectedQty { get; set; }
    public string RecommendedAction { get; set; } = string.Empty;
    public DateTime AnalysisDate { get; set; }
}
