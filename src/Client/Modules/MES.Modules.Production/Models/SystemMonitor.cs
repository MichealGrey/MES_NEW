namespace MES.Modules.Production.Models;

public class SystemMonitor
{
    public string MonitorId { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int TotalLots { get; set; }
    public int ProcessingLots { get; set; }
    public int WaitingLots { get; set; }
    public int HoldLots { get; set; }
    public int CompletedLots { get; set; }
    public int AvailableEquipments { get; set; }
    public int RunningEquipments { get; set; }
    public int OfflineEquipments { get; set; }
    public int AvailableCarriers { get; set; }
    public int InUseCarriers { get; set; }
    public double SystemUptime { get; set; }
    public int PendingTasks { get; set; }
    public int OverdueTasks { get; set; }
    public List<AlertInfo> Alerts { get; set; } = [];
}

public class AlertInfo
{
    public string AlertId { get; set; } = Guid.NewGuid().ToString("N");
    public string AlertType { get; set; } = string.Empty; // YieldHold/EquipmentOffline/OverdueTask
    public string Severity { get; set; } = "Warning"; // Info/Warning/Error/Critical
    public string Message { get; set; } = string.Empty;
    public DateTime TriggeredAt { get; set; }
    public bool IsAcknowledged { get; set; }
}
