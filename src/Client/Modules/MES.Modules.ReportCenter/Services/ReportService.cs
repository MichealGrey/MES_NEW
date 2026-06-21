using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MES.Contracts.Common;
using MES.Modules.ReportCenter.Models;

namespace MES.Modules.ReportCenter.Services;

public interface IReportService
{
    Task<DashboardSummary> GetDashboardSummaryAsync();
    Task<List<ProductionDailyReport>> GetProductionReportsAsync(ReportFilter? filter = null);
    Task<List<YieldReportItem>> GetYieldReportsAsync(ReportFilter? filter = null);
    Task<List<YieldTrendPoint>> GetYieldTrendAsync(string? productId = null, int days = 30);
    Task<List<DefectAnalysisItem>> GetDefectAnalysisAsync(ReportFilter? filter = null);
    Task<List<QualityReportItem>> GetQualityReportsAsync(ReportFilter? filter = null);
    Task<QualityStatistics> GetQualityStatisticsAsync(ReportFilter? filter = null);
    Task<List<LotGenealogyReportItem>> GetLotGenealogyAsync(string? lotNo = null);
    Task<List<EquipmentReportItem>> GetEquipmentReportsAsync(ReportFilter? filter = null);
    Task<List<EquipmentTrendPoint>> GetEquipmentTrendAsync(string? equipmentNo = null, int days = 30);
    Task ExportReportAsync(string reportType, List<object> data, string filePath);
}

/// <summary>
/// REST API client service for Report Center operations.
/// Communicates with the backend ReportController.
/// </summary>
public class ReportService : IReportService
{
    private readonly HttpClient _httpClient;

    public ReportService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ============================================================================
    // GET /api/report/dashboard
    // ============================================================================

    public async Task<DashboardSummary> GetDashboardSummaryAsync()
    {
        var response = await _httpClient.GetAsync("Report/dashboard");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<DashboardSummaryDto>>();
        return apiResponse?.Data != null ? MapToDashboardSummary(apiResponse.Data) : new DashboardSummary();
    }

    // ============================================================================
    // GET /api/report/production
    // ============================================================================

    public async Task<List<ProductionDailyReport>> GetProductionReportsAsync(ReportFilter? filter = null)
    {
        var query = BuildQueryString("Report/production", filter);
        var response = await _httpClient.GetAsync(query);
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<ProductionDailyReportDto>>>();
        return apiResponse?.Data?.Select(MapToProductionDailyReport).ToList() ?? new List<ProductionDailyReport>();
    }

    // ============================================================================
    // GET /api/report/yield
    // ============================================================================

    public async Task<List<YieldReportItem>> GetYieldReportsAsync(ReportFilter? filter = null)
    {
        var query = BuildQueryString("Report/yield", filter);
        var response = await _httpClient.GetAsync(query);
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<YieldReportItemDto>>>();
        return apiResponse?.Data?.Select(MapToYieldReportItem).ToList() ?? new List<YieldReportItem>();
    }

    // ============================================================================
    // GET /api/report/yield-trend
    // ============================================================================

    public async Task<List<YieldTrendPoint>> GetYieldTrendAsync(string? productId = null, int days = 30)
    {
        var url = $"Report/yield-trend?days={days}";
        if (!string.IsNullOrWhiteSpace(productId))
            url += $"&productId={Uri.EscapeDataString(productId)}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<YieldTrendPointDto>>>();
        return apiResponse?.Data?.Select(MapToYieldTrendPoint).ToList() ?? new List<YieldTrendPoint>();
    }

    // ============================================================================
    // GET /api/report/defect-analysis
    // ============================================================================

    public async Task<List<DefectAnalysisItem>> GetDefectAnalysisAsync(ReportFilter? filter = null)
    {
        var url = "Report/defect-analysis";
        if (filter != null && !string.IsNullOrWhiteSpace(filter.ProductId))
            url += $"?product={Uri.EscapeDataString(filter.ProductId)}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<DefectAnalysisDto>>>();
        return apiResponse?.Data?.Select(MapToDefectAnalysisItem).ToList() ?? new List<DefectAnalysisItem>();
    }

    // ============================================================================
    // GET /api/report/quality
    // ============================================================================

    public async Task<List<QualityReportItem>> GetQualityReportsAsync(ReportFilter? filter = null)
    {
        var query = BuildQueryString("Report/quality", filter);
        var response = await _httpClient.GetAsync(query);
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<QualityReportItemDto>>>();
        return apiResponse?.Data?.Select(MapToQualityReportItem).ToList() ?? new List<QualityReportItem>();
    }

    // ============================================================================
    // GET /api/report/quality-stats
    // ============================================================================

    public async Task<QualityStatistics> GetQualityStatisticsAsync(ReportFilter? filter = null)
    {
        var query = BuildQueryString("Report/quality-stats", filter);
        var response = await _httpClient.GetAsync(query);
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<QualityStatisticsDto>>();
        return apiResponse?.Data != null ? MapToQualityStatistics(apiResponse.Data) : new QualityStatistics();
    }

    // ============================================================================
    // GET /api/report/lot-genealogy
    // ============================================================================

    public async Task<List<LotGenealogyReportItem>> GetLotGenealogyAsync(string? lotNo = null)
    {
        var url = "Report/lot-genealogy";
        if (!string.IsNullOrWhiteSpace(lotNo))
            url += $"?lotNo={Uri.EscapeDataString(lotNo)}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<LotGenealogyReportItemDto>>>();
        return apiResponse?.Data?.Select(MapToLotGenealogyReportItem).ToList() ?? new List<LotGenealogyReportItem>();
    }

    // ============================================================================
    // GET /api/report/equipment
    // ============================================================================

    public async Task<List<EquipmentReportItem>> GetEquipmentReportsAsync(ReportFilter? filter = null)
    {
        var query = BuildQueryString("Report/equipment", filter);
        var response = await _httpClient.GetAsync(query);
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<EquipmentReportItemDto>>>();
        return apiResponse?.Data?.Select(MapToEquipmentReportItem).ToList() ?? new List<EquipmentReportItem>();
    }

    // ============================================================================
    // GET /api/report/equipment-trend
    // ============================================================================

    public async Task<List<EquipmentTrendPoint>> GetEquipmentTrendAsync(string? equipmentNo = null, int days = 30)
    {
        var url = $"Report/equipment-trend?days={days}";
        if (!string.IsNullOrWhiteSpace(equipmentNo))
            url += $"&equipmentNo={Uri.EscapeDataString(equipmentNo)}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<EquipmentTrendPointDto>>>();
        return apiResponse?.Data?.Select(MapToEquipmentTrendPoint).ToList() ?? new List<EquipmentTrendPoint>();
    }

    // ============================================================================
    // Export
    // ============================================================================

    public async Task ExportReportAsync(string reportType, List<object> data, string filePath)
    {
        await Task.Delay(200);
        // Actual export can be implemented via a dedicated API endpoint if needed
        System.Diagnostics.Debug.WriteLine($"Export report: {reportType}, data count: {data.Count}, path: {filePath}");
    }

    // ============================================================================
    // Helpers: Query string builder
    // ============================================================================

    private static string BuildQueryString(string baseUrl, ReportFilter? filter)
    {
        if (filter == null)
            return baseUrl;

        var parts = new List<string> { baseUrl };
        var queryParams = new List<string>();

        if (filter.StartDate.HasValue)
            queryParams.Add($"startDate={filter.StartDate.Value:yyyy-MM-dd}");
        if (filter.EndDate.HasValue)
            queryParams.Add($"endDate={filter.EndDate.Value:yyyy-MM-dd}");
        if (!string.IsNullOrWhiteSpace(filter.ProductId))
            queryParams.Add($"product={Uri.EscapeDataString(filter.ProductId)}");
        if (!string.IsNullOrWhiteSpace(filter.LotNo))
            queryParams.Add($"lotNo={Uri.EscapeDataString(filter.LotNo)}");
        if (!string.IsNullOrWhiteSpace(filter.WorkOrderNo))
            queryParams.Add($"workOrder={Uri.EscapeDataString(filter.WorkOrderNo)}");
        if (!string.IsNullOrWhiteSpace(filter.EquipmentNo))
            queryParams.Add($"equipment={Uri.EscapeDataString(filter.EquipmentNo)}");
        if (!string.IsNullOrWhiteSpace(filter.ProcessStep))
            queryParams.Add($"step={Uri.EscapeDataString(filter.ProcessStep)}");
        if (!string.IsNullOrWhiteSpace(filter.Shift))
            queryParams.Add($"shift={Uri.EscapeDataString(filter.Shift)}");
        if (!string.IsNullOrWhiteSpace(filter.Status))
            queryParams.Add($"severity={Uri.EscapeDataString(filter.Status)}");

        if (queryParams.Any())
            parts.Add("?" + string.Join("&", queryParams));

        return string.Concat(parts);
    }

    // ============================================================================
    // Mapping helpers: Backend DTO -> Client Model
    // ============================================================================

    private static DashboardSummary MapToDashboardSummary(DashboardSummaryDto dto) => new()
    {
        TotalProductionOrders = dto.TotalProductionOrders,
        CompletedOrders = dto.CompletedOrders,
        InProgressOrders = dto.InProgressOrders,
        PendingOrders = dto.PendingOrders,
        OverallYield = dto.OverallYield,
        TargetYield = dto.TargetYield,
        TotalLots = dto.TotalLots,
        ActiveLots = dto.ActiveLots,
        EquipmentRunning = dto.EquipmentRunning,
        EquipmentTotal = dto.EquipmentTotal,
        QualityAlerts = dto.QualityAlerts,
        OverdueOrders = dto.OverdueOrders,
        ReportDate = DateTime.Now,
    };

    private static ProductionDailyReport MapToProductionDailyReport(ProductionDailyReportDto dto) => new()
    {
        ReportId = dto.ReportId,
        ReportDate = dto.ReportDate,
        WorkOrderNo = dto.WorkOrderNo,
        ProductName = dto.ProductName,
        LotNo = dto.LotNo,
        InputQty = dto.InputQty,
        OutputQty = dto.OutputQty,
        GoodQty = dto.GoodQty,
        ScrapQty = dto.ScrapQty,
        YieldRate = dto.YieldRate,
        Shift = dto.Shift,
        Operator = dto.Operator,
        EquipmentNo = dto.EquipmentNo,
        ProcessStep = dto.ProcessStep,
        Efficiency = dto.Efficiency,
        Remark = dto.Remark,
    };

    private static YieldReportItem MapToYieldReportItem(YieldReportItemDto dto) => new()
    {
        ReportId = dto.ReportId,
        ReportDate = dto.ReportDate,
        ProductId = dto.ProductId,
        ProductName = dto.ProductName,
        LotNo = dto.LotNo,
        TotalInput = dto.TotalInput,
        TotalOutput = dto.TotalOutput,
        GoodQty = dto.GoodQty,
        ScrapQty = dto.ScrapQty,
        YieldRate = dto.YieldRate,
        TargetYield = dto.TargetYield,
        TopDefectType = dto.TopDefectType,
        TopDefectCount = dto.TopDefectCount,
        ProcessStep = dto.ProcessStep,
        Shift = dto.Shift,
    };

    private static YieldTrendPoint MapToYieldTrendPoint(YieldTrendPointDto dto) => new()
    {
        Date = dto.Date,
        YieldRate = dto.YieldRate,
        TargetYield = dto.TargetYield,
        ProductName = dto.ProductName,
    };

    private static DefectAnalysisItem MapToDefectAnalysisItem(DefectAnalysisDto dto) => new()
    {
        DefectType = dto.DefectType,
        DefectCount = dto.DefectCount,
        Percentage = dto.Percentage,
        AffectedProduct = dto.AffectedProduct,
        FirstOccurrence = dto.FirstOccurrence,
        LastOccurrence = dto.LastOccurrence,
    };

    private static QualityReportItem MapToQualityReportItem(QualityReportItemDto dto) => new()
    {
        ReportId = dto.ReportId,
        ReportDate = dto.ReportDate,
        InspectionNo = dto.InspectionNo,
        LotNo = dto.LotNo,
        ProductName = dto.ProductName,
        ProcessStep = dto.ProcessStep,
        InspectedQty = dto.InspectedQty,
        PassQty = dto.PassQty,
        FailQty = dto.FailQty,
        PassRate = dto.PassRate,
        DefectCode = dto.DefectCode,
        DefectDescription = dto.DefectDescription,
        Severity = dto.Severity,
        Disposition = dto.Disposition,
        Inspector = dto.Inspector,
        Remark = dto.Remark,
    };

    private static QualityStatistics MapToQualityStatistics(QualityStatisticsDto dto) => new()
    {
        TotalInspections = dto.TotalInspections,
        TotalPassed = dto.TotalPassed,
        TotalFailed = dto.TotalFailed,
        OverallPassRate = dto.OverallPassRate,
        CriticalDefects = dto.CriticalDefects,
        MajorDefects = dto.MajorDefects,
        MinorDefects = dto.MinorDefects,
        TopDefectType = dto.TopDefectType,
        TopDefectCount = dto.TopDefectCount,
    };

    private static LotGenealogyReportItem MapToLotGenealogyReportItem(LotGenealogyReportItemDto dto) => new()
    {
        LotId = dto.LotId,
        LotNo = dto.LotNo,
        ProductName = dto.ProductName,
        ParentLotNo = dto.ParentLotNo,
        ChildLotNo = dto.ChildLotNo,
        ProcessStep = dto.ProcessStep,
        EquipmentNo = dto.EquipmentNo,
        Operator = dto.Operator,
        StartTime = dto.StartTime,
        EndTime = dto.EndTime,
        InputQty = dto.InputQty,
        OutputQty = dto.OutputQty,
        Status = dto.Status,
        SplitMergeType = dto.SplitMergeType,
        TraceLevel = dto.TraceLevel,
    };

    private static EquipmentReportItem MapToEquipmentReportItem(EquipmentReportItemDto dto) => new()
    {
        EquipmentId = dto.EquipmentId,
        EquipmentNo = dto.EquipmentNo,
        EquipmentName = dto.EquipmentName,
        ReportDate = dto.ReportDate,
        OEE = dto.OEE,
        Availability = dto.Availability,
        Performance = dto.Performance,
        QualityRate = dto.QualityRate,
        TotalRunTime = dto.TotalRunTime,
        TotalIdleTime = dto.TotalIdleTime,
        TotalDownTime = dto.TotalDownTime,
        TotalProcessQty = dto.TotalProcessQty,
        AlarmCount = dto.AlarmCount,
        TopAlarmType = dto.TopAlarmType,
        TopAlarmCount = dto.TopAlarmCount,
        LastMaintenanceDate = dto.LastMaintenanceDate,
        NextMaintenanceDate = dto.NextMaintenanceDate,
        Status = dto.Status,
    };

    private static EquipmentTrendPoint MapToEquipmentTrendPoint(EquipmentTrendPointDto dto) => new()
    {
        Date = dto.Date,
        OEE = dto.OEE,
        Availability = dto.Availability,
        EquipmentNo = dto.EquipmentNo,
    };

    // ============================================================================
    // Internal DTO classes matching ReportController response format
    // ============================================================================

    private record DashboardSummaryDto(
        int TotalProductionOrders, int CompletedOrders, int InProgressOrders, int PendingOrders,
        double OverallYield, double TargetYield, int TotalLots, int ActiveLots,
        int EquipmentRunning, int EquipmentTotal, int QualityAlerts, int OverdueOrders);

    private record ProductionDailyReportDto(
        string ReportId, DateTime ReportDate, string WorkOrderNo, string ProductName,
        string LotNo, int InputQty, int OutputQty, int GoodQty, int ScrapQty,
        double YieldRate, string Shift, string Operator, string EquipmentNo,
        string ProcessStep, double Efficiency, string Remark);

    private record YieldReportItemDto(
        string ReportId, DateTime ReportDate, string ProductId, string ProductName,
        string LotNo, int TotalInput, int TotalOutput, int GoodQty, int ScrapQty,
        double YieldRate, double TargetYield, string TopDefectType, int TopDefectCount,
        string ProcessStep, string Shift);

    private record YieldTrendPointDto(DateTime Date, double YieldRate, double TargetYield, string ProductName);

    private record DefectAnalysisDto(string DefectType, int DefectCount, double Percentage, string AffectedProduct, DateTime FirstOccurrence, DateTime LastOccurrence);

    private record QualityReportItemDto(
        string ReportId, DateTime ReportDate, string InspectionNo, string LotNo,
        string ProductName, string ProcessStep, int InspectedQty, int PassQty,
        int FailQty, double PassRate, string DefectCode, string DefectDescription,
        string Severity, string Disposition, string Inspector, string Remark);

    private record QualityStatisticsDto(
        int TotalInspections, int TotalPassed, int TotalFailed, double OverallPassRate,
        int CriticalDefects, int MajorDefects, int MinorDefects, string TopDefectType, int TopDefectCount);

    private record LotGenealogyReportItemDto(
        string LotId, string LotNo, string ProductName, string ParentLotNo, string ChildLotNo,
        string ProcessStep, string EquipmentNo, string Operator, DateTime StartTime,
        DateTime? EndTime, int InputQty, int OutputQty, string Status, string SplitMergeType, string TraceLevel);

    private record EquipmentReportItemDto(
        string EquipmentId, string EquipmentNo, string EquipmentName, DateTime ReportDate,
        double OEE, double Availability, double Performance, double QualityRate,
        int TotalRunTime, int TotalIdleTime, int TotalDownTime, int TotalProcessQty,
        int AlarmCount, string TopAlarmType, int TopAlarmCount,
        DateTime LastMaintenanceDate, DateTime NextMaintenanceDate, string Status);

    private record EquipmentTrendPointDto(DateTime Date, double OEE, double Availability, string EquipmentNo);
}
