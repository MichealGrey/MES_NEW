namespace MES.Modules.Production.Models;

public class ExternalSystemEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString("N");
    public string EventType { get; set; } = string.Empty; // LotTrackIn/LotTrackOut/AlarmRaised/OrderCompleted
    public string SourceSystem { get; set; } = string.Empty; // MES/ERP/EAP/QMS/WMS
    public string TargetSystem { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending/Sent/Failed/Acknowledged
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
}

public class ExternalSystemConfig
{
    public string SystemId { get; set; } = string.Empty;
    public string SystemName { get; set; } = string.Empty;
    public string SystemType { get; set; } = string.Empty; // ERP/EAP/QMS/WMS
    public string Endpoint { get; set; } = string.Empty;
    public string AuthType { get; set; } = "None"; // None/Basic/ApiKey/OAuth
    public string? AuthCredential { get; set; }
    public bool IsEnabled { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
    public List<string> SubscribedEvents { get; set; } = new();
}
