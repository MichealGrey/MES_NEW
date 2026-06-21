using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MES.Contracts.Common;
using MES.Modules.Trace.Models;

namespace MES.Modules.Trace.Services;

/// <summary>
/// Interface for Trace operations.
/// </summary>
public interface ITraceService
{
    Task<List<LotTraceItem>> GetLotTracesAsync();
    Task<List<ForwardTraceItem>> GetForwardTraceAsync(string lotId);
    Task<List<BackwardTraceItem>> GetBackwardTraceAsync(string lotId);
    Task<List<GenealogyItem>> GetGenealogyAsync(string lotId);
    Task<List<ImpactAnalysisItem>> GetImpactAnalysisAsync();
    Task<List<CustomerTraceReport>> GetCustomerReportsAsync();
}

/// <summary>
/// REST API client service for Trace operations.
/// Communicates with the backend TraceController.
/// </summary>
public class TraceService : ITraceService
{
    private readonly HttpClient _httpClient;

    public TraceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ============================================================================
    // GET /api/trace/lots
    // ============================================================================

    public async Task<List<LotTraceItem>> GetLotTracesAsync()
    {
        var response = await _httpClient.GetAsync("Trace/lots");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<LotTraceDto>>>();
        return apiResponse?.Data?.Select(MapToLotTraceItem).ToList() ?? new List<LotTraceItem>();
    }

    // ============================================================================
    // GET /api/trace/forward/{lotId}
    // ============================================================================

    public async Task<List<ForwardTraceItem>> GetForwardTraceAsync(string lotId)
    {
        var response = await _httpClient.GetAsync($"Trace/forward/{lotId}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<ForwardTraceDto>>>();
        return apiResponse?.Data?.Select(MapToForwardTraceItem).ToList() ?? new List<ForwardTraceItem>();
    }

    // ============================================================================
    // GET /api/trace/backward/{lotId}
    // ============================================================================

    public async Task<List<BackwardTraceItem>> GetBackwardTraceAsync(string lotId)
    {
        var response = await _httpClient.GetAsync($"Trace/backward/{lotId}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<BackwardTraceDto>>>();
        return apiResponse?.Data?.Select(MapToBackwardTraceItem).ToList() ?? new List<BackwardTraceItem>();
    }

    // ============================================================================
    // GET /api/trace/genealogy/{lotId}
    // ============================================================================

    public async Task<List<GenealogyItem>> GetGenealogyAsync(string lotId)
    {
        var response = await _httpClient.GetAsync($"Trace/genealogy/{lotId}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<GenealogyDto>>>();
        return apiResponse?.Data?.Select(MapToGenealogyItem).ToList() ?? new List<GenealogyItem>();
    }

    // ============================================================================
    // GET /api/trace/impact-analysis?lotId={lotId}
    // ============================================================================

    public async Task<List<ImpactAnalysisItem>> GetImpactAnalysisAsync()
    {
        var response = await _httpClient.GetAsync("Trace/impact-analysis");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<ImpactAnalysisDto>>>();
        return apiResponse?.Data?.Select(MapToImpactAnalysisItem).ToList() ?? new List<ImpactAnalysisItem>();
    }

    // ============================================================================
    // GET /api/trace/customer-report/{lotId}
    // ============================================================================

    public async Task<List<CustomerTraceReport>> GetCustomerReportsAsync()
    {
        // For listing all customer reports, we fetch all lots and generate a report per lot
        var lotResponse = await _httpClient.GetAsync("Trace/lots");
        lotResponse.EnsureSuccessStatusCode();
        var lotApiResponse = await lotResponse.Content.ReadFromJsonAsync<ApiResponse<List<LotTraceDto>>>();
        var lots = lotApiResponse?.Data ?? new List<LotTraceDto>();

        var result = new List<CustomerTraceReport>();
        foreach (var lot in lots)
        {
            var reportResponse = await _httpClient.GetAsync($"Trace/customer-report/{lot.LotId}");
            if (reportResponse.IsSuccessStatusCode)
            {
                var reportApiResponse = await reportResponse.Content.ReadFromJsonAsync<ApiResponse<List<CustomerTraceReportDto>>>();
                if (reportApiResponse?.Data != null)
                {
                    result.AddRange(reportApiResponse.Data.Select(MapToCustomerTraceReport));
                }
            }
        }

        return result;
    }

    // ============================================================================
    // Mapping helpers: Backend DTO -> Client Model
    // ============================================================================

    private static LotTraceItem MapToLotTraceItem(LotTraceDto dto) => new()
    {
        Id = dto.Id,
        LotId = dto.LotId,
        Product = dto.Product,
        CurrentStep = dto.CurrentStep,
        Status = dto.Status,
        CreatedTime = dto.CreatedTime,
        CompletedTime = dto.CompletedTime,
        Customer = dto.Customer,
        WorkOrderNo = dto.WorkOrderNo,
    };

    private static ForwardTraceItem MapToForwardTraceItem(ForwardTraceDto dto) => new()
    {
        StepNo = dto.StepNo,
        Operation = dto.Operation,
        EquipmentId = dto.EquipmentId,
        StartTime = dto.StartTime,
        EndTime = dto.EndTime,
        Operator = dto.Operator,
        RecipeId = dto.RecipeId,
        Result = dto.Result,
    };

    private static BackwardTraceItem MapToBackwardTraceItem(BackwardTraceDto dto) => new()
    {
        LotId = dto.LotId,
        WaferId = dto.WaferId,
        Supplier = dto.Supplier,
        MaterialLot = dto.MaterialLot,
        ReceiveDate = dto.ReceiveDate,
        InspectionResult = dto.InspectionResult,
    };

    private static GenealogyItem MapToGenealogyItem(GenealogyDto dto) => new()
    {
        Id = dto.Id,
        LotId = dto.LotId,
        ParentLotId = dto.ParentLotId,
        ChildLotId = dto.ChildLotId,
        Product = dto.Product,
        Operation = dto.Operation,
        Time = dto.Time,
        EquipmentId = dto.EquipmentId,
        Operator = dto.Operator,
    };

    private static ImpactAnalysisItem MapToImpactAnalysisItem(ImpactAnalysisDto dto) => new()
    {
        AffectedLotId = dto.AffectedLotId,
        RootCause = dto.RootCause,
        ImpactType = dto.ImpactType,
        AffectedQty = dto.AffectedQty,
        RecommendedAction = dto.RecommendedAction,
        AnalysisDate = dto.AnalysisDate,
    };

    private static CustomerTraceReport MapToCustomerTraceReport(CustomerTraceReportDto dto) => new()
    {
        ReportId = dto.ReportId,
        LotId = dto.LotId,
        CustomerName = dto.CustomerName,
        ReportDate = dto.ReportDate,
        ProductName = dto.ProductName,
        TotalQty = dto.TotalQty,
        TraceResult = dto.TraceResult,
    };

    // ============================================================================
    // Internal DTO classes matching TraceController response format
    // ============================================================================

    private record LotTraceDto(
        string Id,
        string LotId,
        string Product,
        string CurrentStep,
        string Status,
        DateTime CreatedTime,
        DateTime? CompletedTime,
        string Customer,
        string WorkOrderNo);

    private record ForwardTraceDto(
        string StepNo,
        string Operation,
        string EquipmentId,
        DateTime StartTime,
        DateTime EndTime,
        string Operator,
        string RecipeId,
        string Result);

    private record BackwardTraceDto(
        string LotId,
        string WaferId,
        string Supplier,
        string MaterialLot,
        DateTime ReceiveDate,
        string InspectionResult);

    private record GenealogyDto(
        string Id,
        string LotId,
        string ParentLotId,
        string ChildLotId,
        string Product,
        string Operation,
        DateTime Time,
        string EquipmentId,
        string Operator);

    private record ImpactAnalysisDto(
        string AffectedLotId,
        string RootCause,
        string ImpactType,
        int AffectedQty,
        string RecommendedAction,
        DateTime AnalysisDate);

    private record CustomerTraceReportDto(
        string ReportId,
        string LotId,
        string CustomerName,
        DateTime ReportDate,
        string ProductName,
        int TotalQty,
        string TraceResult);
}
