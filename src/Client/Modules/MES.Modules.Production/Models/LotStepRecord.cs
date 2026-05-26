namespace MES.Modules.Production.Models;

public class LotStepRecord
{
    public string RecordId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = "1.0";
    public int StepSeq { get; set; }
    public string StepName { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;

    public string Status { get; set; } = "Waiting"; // Waiting, Processing, Completed, Skipped, Held

    public string? TrackInEquipment { get; set; }
    public string? TrackInCarrier { get; set; }
    public string? TrackInRecipe { get; set; }
    public DateTime? TrackInTime { get; set; }
    public string? TrackInOperator { get; set; }

    public DateTime? TrackOutTime { get; set; }
    public string? TrackOutOperator { get; set; }

    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public int ScrapQty { get; set; }
    public int ReworkQty { get; set; }
    public int HoldQty { get; set; }
    public int PendingQty { get; set; }

    public string? RecipeId { get; set; }
    public string? TestProgram { get; set; }
    public string? BinSummary { get; set; }
    public string? Remark { get; set; }
}
