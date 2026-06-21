namespace MES.Modules.Trace.Models;

public class ForwardTraceItem
{
    public string StepNo { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Operator { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public string Result { get; set; } = "Pass";
}

public class BackwardTraceItem
{
    public string LotId { get; set; } = string.Empty;
    public string WaferId { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public string MaterialLot { get; set; } = string.Empty;
    public DateTime ReceiveDate { get; set; }
    public string InspectionResult { get; set; } = string.Empty;
}

public class CustomerTraceReport
{
    public string ReportId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQty { get; set; }
    public string TraceResult { get; set; } = string.Empty;
}
