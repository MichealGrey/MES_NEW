namespace MES.Modules.Trace.Models;

public class LotTraceItem
{
    public int Step { get; set; }
    public string ProcessName { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Result { get; set; } = "Pass";
}
