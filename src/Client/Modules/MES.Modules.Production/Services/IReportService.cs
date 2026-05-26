using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IReportService
{
    Task<ProductionReport> GenerateDailyReportAsync(DateTime date);
    Task<YieldReport> GenerateYieldReportAsync(string routeId, DateTime startDate, DateTime endDate);
    Task<SystemMonitor> GetSystemSnapshotAsync();
    Task<List<EquipmentUtilization>> GetEquipmentUtilizationAsync(DateTime date);
}
