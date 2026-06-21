using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MES.Contracts.Common;
using MES.Modules.Yield.Models;

namespace MES.Modules.Yield.Services;

/// <summary>
/// Interface for Yield operations.
/// </summary>
public interface IYieldService
{
    Task<List<YieldKpiItem>> GetDashboardKpisAsync();
    Task<List<YieldTrendItem>> GetYieldTrendAsync();
    Task<List<WaferDieItem>> GetWaferMapAsync(string waferId);
    Task<List<YieldKpiItem>> GetYieldAnalysisAsync(string product);
    Task<List<WaferDieItem>> GetBinAnalysisAsync();
    Task<List<WaferDieItem>> GetDefectAnalysisAsync();
}

/// <summary>
/// REST API client service for Yield operations.
/// Communicates with the backend YieldController.
/// </summary>
public class YieldService : IYieldService
{
    private readonly HttpClient _httpClient;

    public YieldService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ============================================================================
    // GET /api/yield/dashboard-kpis
    // ============================================================================

    public async Task<List<YieldKpiItem>> GetDashboardKpisAsync()
    {
        var response = await _httpClient.GetAsync("Yield/dashboard-kpis");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<YieldKpiDto>>>();
        return apiResponse?.Data?.Select(MapToYieldKpiItem).ToList() ?? new List<YieldKpiItem>();
    }

    // ============================================================================
    // GET /api/yield/trend
    // ============================================================================

    public async Task<List<YieldTrendItem>> GetYieldTrendAsync()
    {
        var response = await _httpClient.GetAsync("Yield/trend");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<YieldTrendDto>>>();
        return apiResponse?.Data?.Select(MapToYieldTrendItem).ToList() ?? new List<YieldTrendItem>();
    }

    // ============================================================================
    // GET /api/yield/wafer-map?waferId={waferId}
    // ============================================================================

    public async Task<List<WaferDieItem>> GetWaferMapAsync(string waferId)
    {
        var response = await _httpClient.GetAsync($"Yield/wafer-map?waferId={waferId}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<WaferDieDto>>>();
        return apiResponse?.Data?.Select(MapToWaferDieItem).ToList() ?? new List<WaferDieItem>();
    }

    // ============================================================================
    // GET /api/yield/analysis?product={product}
    // ============================================================================

    public async Task<List<YieldKpiItem>> GetYieldAnalysisAsync(string product)
    {
        var response = await _httpClient.GetAsync($"Yield/analysis?product={product}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<YieldAnalysisDto>>>();
        return apiResponse?.Data?.Select(MapToYieldAnalysisItem).ToList() ?? new List<YieldKpiItem>();
    }

    // ============================================================================
    // GET /api/yield/bin-analysis
    // ============================================================================

    public async Task<List<WaferDieItem>> GetBinAnalysisAsync()
    {
        var response = await _httpClient.GetAsync("Yield/bin-analysis");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<BinAnalysisDto>>>();
        return apiResponse?.Data?.Select(MapToBinAnalysisItem).ToList() ?? new List<WaferDieItem>();
    }

    // ============================================================================
    // GET /api/yield/defect-analysis
    // ============================================================================

    public async Task<List<WaferDieItem>> GetDefectAnalysisAsync()
    {
        var response = await _httpClient.GetAsync("Yield/defect-analysis");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<DefectAnalysisDto>>>();
        return apiResponse?.Data?.Select(MapToDefectAnalysisItem).ToList() ?? new List<WaferDieItem>();
    }

    // ============================================================================
    // Mapping helpers: Backend DTO -> Client Model
    // ============================================================================

    private static YieldKpiItem MapToYieldKpiItem(YieldKpiDto dto) => new()
    {
        KpiName = dto.KpiName,
        CurrentValue = (double)dto.CurrentValue,
        TargetValue = (double)dto.TargetValue,
        Trend = dto.Trend,
    };

    private static YieldTrendItem MapToYieldTrendItem(YieldTrendDto dto) => new()
    {
        Id = dto.Id,
        Date = dto.Date,
        Product = dto.Product,
        Yield = (double)dto.Yield,
        TargetYield = (double)dto.TargetYield,
        TotalQty = dto.TotalQty,
        GoodQty = dto.GoodQty,
    };

    private static WaferDieItem MapToWaferDieItem(WaferDieDto dto) => new()
    {
        WaferId = dto.WaferId,
        DieX = dto.DieX,
        DieY = dto.DieY,
        BinCode = dto.BinCode,
        Result = dto.Result,
        DefectType = dto.DefectType ?? string.Empty,
    };

    private static YieldKpiItem MapToYieldAnalysisItem(YieldAnalysisDto dto) => new()
    {
        KpiName = dto.Product,
        CurrentValue = (double)dto.AvgYield,
        TargetValue = (double)dto.PassRate,
        Trend = dto.AvgYield >= dto.PassRate ? "Up" : "Down",
    };

    private static WaferDieItem MapToBinAnalysisItem(BinAnalysisDto dto) => new()
    {
        WaferId = "Analysis",
        DieX = 0,
        DieY = 0,
        BinCode = dto.BinCode,
        Result = dto.BinName == "Pass" ? "Good" : (dto.BinName == "Marginal" ? "Marginal" : "Fail"),
        DefectType = dto.BinName,
    };

    private static WaferDieItem MapToDefectAnalysisItem(DefectAnalysisDto dto) => new()
    {
        WaferId = "Defect",
        DieX = 0,
        DieY = 0,
        BinCode = string.Empty,
        Result = "Fail",
        DefectType = dto.DefectType,
    };

    // ============================================================================
    // Internal DTO classes matching YieldController response format
    // ============================================================================

    private record YieldKpiDto(string KpiName, decimal CurrentValue, decimal TargetValue, string Trend);

    private record YieldTrendDto(string Id, DateTime Date, string Product, decimal Yield, decimal TargetYield, int TotalQty, int GoodQty);

    private record WaferDieDto(string WaferId, int DieX, int DieY, string BinCode, string Result, string? DefectType);

    private record YieldAnalysisDto(string Product, decimal AvgYield, decimal MinYield, decimal MaxYield, int TotalLots, decimal PassRate);

    private record BinAnalysisDto(string BinCode, string BinName, int TotalQty, decimal Percentage);

    private record DefectAnalysisDto(string DefectType, int Count, decimal Percentage, string Trend);
}
