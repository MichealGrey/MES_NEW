namespace MES.Modules.Production.Models;

public class AlarmRecord
{
    public string AlarmId { get; set; } = Guid.NewGuid().ToString("N");
    public string AlarmType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Warning";
    public string Message { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string? EquipmentId { get; set; }
    public string? StepCode { get; set; }
    public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
    public bool IsAcknowledged { get; set; }
    public string? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? Detail { get; set; }
}
