namespace MES.Modules.Production.Models;

public class TraceReport
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string CustomerPN { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; } = DateTime.UtcNow;
    public string GeneratedBy { get; set; } = string.Empty;
    public List<TraceReportStep> Steps { get; set; } = new();
    public List<TraceReportException> Exceptions { get; set; } = new();
    public TraceReportSummary Summary { get; set; } = new();
}

public class TraceReportStep
{
    public int StepSeq { get; set; }
    public string StepName { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? TrackInTime { get; set; }
    public DateTime? TrackOutTime { get; set; }
    public string? EquipmentId { get; set; }
    public string? Operator { get; set; }
    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public int ScrapQty { get; set; }
    public double YieldPercent { get; set; }
    public string? Remark { get; set; }
}

public class TraceReportException
{
    public string Type { get; set; } = string.Empty; // Hold/Rework/Scrap/MRB
    public string StepName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class TraceReportSummary
{
    public int OriginalQty { get; set; }
    public int FinalQty { get; set; }
    public double OverallYield { get; set; }
    public int TotalHoldCount { get; set; }
    public int TotalReworkCount { get; set; }
    public int TotalScrapQty { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int TotalSteps { get; set; }
    public int CompletedSteps { get; set; }
}
