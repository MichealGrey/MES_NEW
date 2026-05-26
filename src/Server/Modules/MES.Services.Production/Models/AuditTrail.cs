namespace MES.Services.Production.Models;

public class AuditTrail
{
    public string AuditId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty; // Lot, Route, Operation, etc.
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create, Update, TrackIn, TrackOut, etc.
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? IpAddress { get; set; }
    public string? Remark { get; set; }
}
