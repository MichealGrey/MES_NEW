namespace MES.Modules.Trace.Models;

public class LotTraceItem
{
    public string Id { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string Status { get; set; } = "InProcess";
    public DateTime CreatedTime { get; set; }
    public DateTime? CompletedTime { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string WorkOrderNo { get; set; } = string.Empty;
}
