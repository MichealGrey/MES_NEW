using System.Collections.ObjectModel;

namespace MES.Modules.ReportCenter.Models;

/// <summary>
/// 报表仪表板汇总数据
/// </summary>
public class DashboardSummary
{
    public int TotalProductionOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int InProgressOrders { get; set; }
    public int PendingOrders { get; set; }
    public double OverallYield { get; set; }
    public double TargetYield { get; set; }
    public int TotalLots { get; set; }
    public int ActiveLots { get; set; }
    public int EquipmentRunning { get; set; }
    public int EquipmentTotal { get; set; }
    public int QualityAlerts { get; set; }
    public int OverdueOrders { get; set; }
    public DateTime ReportDate { get; set; } = DateTime.Now;
}

/// <summary>
/// 生产日报数据
/// </summary>
public class ProductionDailyReport
{
    public string ReportId { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public int InputQty { get; set; }
    public int OutputQty { get; set; }
    public int GoodQty { get; set; }
    public int ScrapQty { get; set; }
    public double YieldRate { get; set; }
    public string Shift { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string EquipmentNo { get; set; } = string.Empty;
    public string ProcessStep { get; set; } = string.Empty;
    public double Efficiency { get; set; }
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 良率报表数据
/// </summary>
public class YieldReportItem
{
    public string ReportId { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public int TotalInput { get; set; }
    public int TotalOutput { get; set; }
    public int GoodQty { get; set; }
    public int ScrapQty { get; set; }
    public double YieldRate { get; set; }
    public double TargetYield { get; set; }
    public bool IsBelowTarget => YieldRate < TargetYield;
    public string TopDefectType { get; set; } = string.Empty;
    public int TopDefectCount { get; set; }
    public string ProcessStep { get; set; } = string.Empty;
    public string Shift { get; set; } = string.Empty;
}

/// <summary>
/// 良率趋势数据点
/// </summary>
public class YieldTrendPoint
{
    public DateTime Date { get; set; }
    public double YieldRate { get; set; }
    public double TargetYield { get; set; }
    public string ProductName { get; set; } = string.Empty;
}

/// <summary>
/// 缺陷分析数据
/// </summary>
public class DefectAnalysisItem
{
    public string DefectType { get; set; } = string.Empty;
    public int DefectCount { get; set; }
    public double Percentage { get; set; }
    public string AffectedProduct { get; set; } = string.Empty;
    public DateTime FirstOccurrence { get; set; }
    public DateTime LastOccurrence { get; set; }
}

/// <summary>
/// 质量报表数据
/// </summary>
public class QualityReportItem
{
    public string ReportId { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public string InspectionNo { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProcessStep { get; set; } = string.Empty;
    public int InspectedQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public double PassRate { get; set; }
    public string DefectCode { get; set; } = string.Empty;
    public string DefectDescription { get; set; } = string.Empty;
    public string Severity { get; set; } = "Normal";
    public string Disposition { get; set; } = string.Empty;
    public string Inspector { get; set; } = string.Empty;
    public string Remark { get; set; } = string.Empty;
}

/// <summary>
/// 质量统计数据
/// </summary>
public class QualityStatistics
{
    public int TotalInspections { get; set; }
    public int TotalPassed { get; set; }
    public int TotalFailed { get; set; }
    public double OverallPassRate { get; set; }
    public int CriticalDefects { get; set; }
    public int MajorDefects { get; set; }
    public int MinorDefects { get; set; }
    public string TopDefectType { get; set; } = string.Empty;
    public int TopDefectCount { get; set; }
}

/// <summary>
/// 批次追溯数据
/// </summary>
public class LotGenealogyReportItem
{
    public string LotId { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ParentLotNo { get; set; } = string.Empty;
    public string ChildLotNo { get; set; } = string.Empty;
    public string ProcessStep { get; set; } = string.Empty;
    public string EquipmentNo { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int InputQty { get; set; }
    public int OutputQty { get; set; }
    public string Status { get; set; } = string.Empty;
    public string SplitMergeType { get; set; } = string.Empty;
    public string TraceLevel { get; set; } = string.Empty;
}

/// <summary>
/// 设备报表数据
/// </summary>
public class EquipmentReportItem
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentNo { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public double OEE { get; set; }
    public double Availability { get; set; }
    public double Performance { get; set; }
    public double QualityRate { get; set; }
    public int TotalRunTime { get; set; }
    public int TotalIdleTime { get; set; }
    public int TotalDownTime { get; set; }
    public int TotalProcessQty { get; set; }
    public int AlarmCount { get; set; }
    public string TopAlarmType { get; set; } = string.Empty;
    public int TopAlarmCount { get; set; }
    public DateTime LastMaintenanceDate { get; set; }
    public DateTime NextMaintenanceDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// 设备趋势数据点
/// </summary>
public class EquipmentTrendPoint
{
    public DateTime Date { get; set; }
    public double OEE { get; set; }
    public double Availability { get; set; }
    public string EquipmentNo { get; set; } = string.Empty;
}

/// <summary>
/// 报表筛选条件
/// </summary>
public class ReportFilter
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ProductId { get; set; }
    public string? LotNo { get; set; }
    public string? WorkOrderNo { get; set; }
    public string? ProcessStep { get; set; }
    public string? EquipmentNo { get; set; }
    public string? Shift { get; set; }
    public string? Status { get; set; }
}
