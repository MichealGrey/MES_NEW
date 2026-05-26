using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface ISystemHealthService
{
    Task<SystemHealthReport> GetHealthReportAsync();
    Task<List<string>> CheckDataConsistencyAsync();
    Task<int> CleanupExpiredDataAsync(int retentionDays = 90);
}

public class SystemHealthService : ISystemHealthService
{
    private readonly IRepository<ProdLot> _lotRepo;
    private readonly IRepository<ProdWorkOrder> _woRepo;
    private readonly IRepository<ProdAlarm> _alarmRepo;
    private readonly IRepository<ProdLotStep> _lotStepRepo;

    public SystemHealthService(
        IRepository<ProdLot> lotRepo,
        IRepository<ProdWorkOrder> woRepo,
        IRepository<ProdAlarm> alarmRepo,
        IRepository<ProdLotStep> lotStepRepo)
    {
        _lotRepo = lotRepo;
        _woRepo = woRepo;
        _alarmRepo = alarmRepo;
        _lotStepRepo = lotStepRepo;
    }

    public async Task<SystemHealthReport> GetHealthReportAsync()
    {
        var report = new SystemHealthReport();
        var warnings = new List<string>();
        var errors = new List<string>();

        // 数据库连接检查
        var dbHealth = new ComponentHealth { Name = "Database", Status = "Healthy" };
        try
        {
            var lots = await _lotRepo.GetAllAsync();
            dbHealth.ResponseTimeMs = 1;
        }
        catch (Exception ex)
        {
            dbHealth.Status = "Down";
            dbHealth.Message = ex.Message;
            errors.Add($"数据库连接失败: {ex.Message}");
        }
        report.Components.Add(dbHealth);

        // 统计批次数据
        var allLots = await _lotRepo.GetAllAsync();
        report.TotalLots = allLots.Count;
        report.ActiveLots = allLots.Count(l => l.Status is "Processing" or "Hold" or "Waiting");

        // 统计工单数据
        var allWOs = await _woRepo.GetAllAsync();
        report.TotalWorkOrders = allWOs.Count;
        report.ActiveWorkOrders = allWOs.Count(w => w.Status is "Released" or "InProgress");

        // 检查未解决报警
        var alarms = await _alarmRepo.GetAllAsync();
        report.UnresolvedAlarms = alarms.Count(a => a.Status != "Acknowledged" && a.Status != "Resolved");

        // 数据一致性检查
        var inconsistencies = await CheckDataConsistencyAsync();
        report.DataInconsistencies = inconsistencies.Count;

        // 整体状态判断
        if (errors.Count > 0)
            report.OverallStatus = "Critical";
        else if (warnings.Count > 0 || report.UnresolvedAlarms > 5 || inconsistencies.Count > 0)
            report.OverallStatus = "Degraded";
        else
            report.OverallStatus = "Healthy";

        report.Warnings = warnings;
        report.Errors = errors;

        return report;
    }

    public async Task<List<string>> CheckDataConsistencyAsync()
    {
        var issues = new List<string>();

        var allLots = await _lotRepo.GetAllAsync();
        var allWOs = await _woRepo.GetAllAsync();
        var woDict = allWOs.ToDictionary(w => w.OrderId);

        // 检查批次与工单的一致性
        foreach (var lot in allLots)
        {
            // 检查工单是否存在
            if (!string.IsNullOrEmpty(lot.OrderId) && !woDict.ContainsKey(lot.OrderId))
                issues.Add($"批次 {lot.LotId} 关联的工单 {lot.OrderId} 不存在");

            // 检查数量一致性
            if (lot.UnitCount < 0)
                issues.Add($"批次 {lot.LotId} 的数量为负数: {lot.UnitCount}");

            if (lot.TotalPassQty + lot.TotalScrapQty > lot.OriginalQty + 100)
                issues.Add($"批次 {lot.LotId} 产出数量异常: 原始 {lot.OriginalQty}, 合格 {lot.TotalPassQty}, 报废 {lot.TotalScrapQty}");
        }

        // 检查孤立工单（没有关联批次）
        var lotOrderIds = allLots.Select(l => l.OrderId).ToHashSet();
        foreach (var wo in allWOs)
        {
            if (!lotOrderIds.Contains(wo.OrderId) && wo.Status is "Released" or "InProgress")
                issues.Add($"工单 {wo.OrderId} 状态为 {wo.Status} 但没有关联批次");
        }

        return issues;
    }

    public async Task<int> CleanupExpiredDataAsync(int retentionDays = 90)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
        int cleanedCount = 0;

        // 清理已完成的批次历史数据
        var allLots = await _lotRepo.GetAllAsync();
        foreach (var lot in allLots)
        {
            if (lot.Status == "Completed" && !lot.IsArchived)
            {
                lot.IsArchived = true;
                await _lotRepo.UpdateAsync(lot);
                cleanedCount++;
            }
        }

        // 清理已解决的旧报警
        var alarms = await _alarmRepo.GetAllAsync();
        foreach (var alarm in alarms)
        {
            if (alarm.Status == "Resolved" && alarm.ResolvedAt.HasValue && alarm.ResolvedAt.Value < cutoffDate)
            {
                await _alarmRepo.DeleteAsync(alarm);
                cleanedCount++;
            }
        }

        return cleanedCount;
    }
}
