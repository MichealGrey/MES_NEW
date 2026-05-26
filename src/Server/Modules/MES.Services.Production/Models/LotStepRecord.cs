namespace MES.Services.Production.Models;

public class LotStepRecord
{
    public string RecordId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = "1.0";
    public int StepSeq { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Skipped
    public string? EquipmentId { get; set; }
    public string? OperatorId { get; set; }
    public DateTime? TrackInTime { get; set; }
    public DateTime? TrackOutTime { get; set; }
    public int? QtyIn { get; set; }
    public int? QtyOut { get; set; }
    public int? QtyPass { get; set; }
    public int? QtyFail { get; set; }
    public string? TrackOutResult { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
