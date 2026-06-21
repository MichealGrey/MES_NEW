using MES.Modules.Quality.Models;

namespace MES.Modules.Quality.Services;

public interface IQualityService
{
    // SPC Chart
    Task<List<SpcChartItem>> GetSpcChartsAsync();
    Task SaveSpcChartAsync(SpcChartItem item);

    // SPC Rules
    Task<List<SpcRuleItem>> GetSpcRulesAsync();
    Task SaveSpcRuleAsync(SpcRuleItem rule);
    Task DeleteSpcRuleAsync(string ruleId);

    // OOC Events
    Task<List<OocEventItem>> GetOocEventsAsync();
    Task UpdateOocEventStatusAsync(string eventId, string status);

    // FDC Monitor
    Task<List<FdcMonitorItem>> GetFdcMonitorsAsync();

    // Inspection
    Task<List<InspectionItem>> GetInspectionsAsync();
    Task SaveInspectionAsync(InspectionItem item);
    Task DeleteInspectionAsync(string inspectionId);

    // FMEA
    Task<List<FmeaItem>> GetFmeaItemsAsync();
    Task SaveFmeaAsync(FmeaItem item);
    Task DeleteFmeaAsync(string fmeaId);

    // Quality Target
    Task<List<QualityTargetItem>> GetQualityTargetsAsync();
    Task SaveQualityTargetAsync(QualityTargetItem item);
    Task DeleteQualityTargetAsync(string targetId);

    // MSA
    Task<List<MsaItem>> GetMsaItemsAsync();
    Task SaveMsaAsync(MsaItem item);
    Task DeleteMsaAsync(string msaId);
}
