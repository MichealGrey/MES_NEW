namespace MES.Modules.Production.Models;

public class CarrierInfo
{
    public string CarrierId { get; set; } = string.Empty;
    public string CarrierType { get; set; } = string.Empty; // FOUP/TapeFrame/Magazine/MoldPlate/OvenCart/SingTray/TestTray/Reel/Tray
    public string Status { get; set; } = "Available"; // Available/InUse/Cleaning/Maintenance/Retired
    public string? CurrentLotId { get; set; }
    public int Capacity { get; set; }
    public int UseCount { get; set; }
    public int MaxUseCount { get; set; }
    public DateTime LastCleanDate { get; set; }
    public int CleanIntervalUses { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? ApplicableProcess { get; set; }
    public string? ApplicablePackage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
