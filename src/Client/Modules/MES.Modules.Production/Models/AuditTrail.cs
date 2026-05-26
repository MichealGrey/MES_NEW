namespace MES.Modules.Production.Models;

/// <summary>
/// 审计追踪记录
/// </summary>
public class AuditTrail
{
    public string AuditId { get; set; } = Guid.NewGuid().ToString("N");
    public string EntityType { get; set; } = string.Empty; // "Lot", "WorkOrder", "Route", etc.
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "TrackIn", "TrackOut", "Hold", "Release", etc.
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? BeforeState { get; set; }
    public string? AfterState { get; set; }
    public string? Reason { get; set; }
    public string? Detail { get; set; }
}
