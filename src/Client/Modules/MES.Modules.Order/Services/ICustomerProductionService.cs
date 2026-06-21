using MES.Modules.Order.Models;
using MES.Domain.Production;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MES.Modules.Order.Services;

public interface ICustomerProductionService
{
    Task<List<DueDateRisk>> GetAllDueDateRisksAsync();
    Task<List<CustomerLotProgress>> GetOrderProgressAsync(string orderId);
    Task<List<CustomerLotProgress>> GetCustomerProgressAsync(string customerId);
    Task<TraceReport> GenerateTraceReportAsync(string lotId, string operatorId);
}

public class DueDateRisk
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPN { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? EstimatedEndDate { get; set; }
    public double ProgressPercent { get; set; }
}

public class CustomerLotProgress
{
    public string LotId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPN { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int OriginalQty { get; set; }
    public int TotalPassQty { get; set; }
    public double YieldPercent { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public bool IsAtRisk { get; set; }
}

public class TraceReport
{
    public string ReportId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public TraceSummary Summary { get; set; } = new();
}

public class TraceSummary
{
    public double OverallYield { get; set; }
}
