namespace MES.Modules.Production.Models;

public class SystemHealthReport
{
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public string OverallStatus { get; set; } = "Healthy"; // Healthy/Degraded/Critical
    public List<ComponentHealth> Components { get; set; } = new();
    public int TotalLots { get; set; }
    public int ActiveLots { get; set; }
    public int TotalWorkOrders { get; set; }
    public int ActiveWorkOrders { get; set; }
    public int CacheHitRate { get; set; }
    public int UnresolvedAlarms { get; set; }
    public int DataInconsistencies { get; set; }
    public List<string> Warnings { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

public class ComponentHealth
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "Healthy"; // Healthy/Degraded/Down
    public string? Message { get; set; }
    public double? ResponseTimeMs { get; set; }
    public DateTime LastChecked { get; set; } = DateTime.UtcNow;
}
