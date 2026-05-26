using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class ReportService : IReportService
{
    private readonly IRepository<ProdLot> _lotRepo;
    private readonly IRepository<MasterEquipment> _equipRepo;
    private readonly IRepository<MasterCarrier> _carrierRepo;

    public ReportService(
        IRepository<ProdLot> lotRepo,
        IRepository<MasterEquipment> equipRepo,
        IRepository<MasterCarrier> carrierRepo)
    {
        _lotRepo = lotRepo;
        _equipRepo = equipRepo;
        _carrierRepo = carrierRepo;
    }

    public async Task<ProductionReport> GenerateDailyReportAsync(DateTime date)
    {
        var report = new ProductionReport
        {
            ReportDate = date,
            ReportType = "Daily"
        };

        var lots = await _lotRepo.GetAllAsync();
        foreach (var lot in lots)
        {
            report.TotalLots++;
            if (lot.Status == "Completed") report.CompletedLots++;
            else if (lot.Status == "Hold") report.HoldLots++;
            else report.WipLots++;

            report.TotalInputQty += lot.OriginalQty;
            report.TotalOutputQty += lot.TotalPassQty;
            report.TotalScrapQty += lot.TotalScrapQty;
        }

        report.OverallYield = report.TotalInputQty > 0
            ? (double)report.TotalOutputQty / report.TotalInputQty * 100
            : 0;

        report.FTYield = report.OverallYield;
        report.GeneratedAt = DateTime.UtcNow;
        return report;
    }

    public async Task<YieldReport> GenerateYieldReportAsync(string routeId, DateTime startDate, DateTime endDate)
    {
        var report = new YieldReport
        {
            RouteId = routeId,
            StartDate = startDate,
            EndDate = endDate
        };

        var yields = new List<double>();
        var lots = await _lotRepo.GetWhereAsync(l => l.RouteId == routeId);

        foreach (var lot in lots)
        {
            if (lot.OriginalQty > 0)
            {
                var yield = (double)lot.TotalPassQty / lot.OriginalQty * 100;
                yields.Add(yield);
            }
            report.TotalLots++;
        }

        if (yields.Count > 0)
        {
            report.AverageYield = yields.Average();
            report.MinYield = yields.Min();
            report.MaxYield = yields.Max();
            report.StdDev = Math.Sqrt(yields.Average(v => Math.Pow(v - report.AverageYield, 2)));
        }

        report.GeneratedAt = DateTime.UtcNow;
        return report;
    }

    public async Task<SystemMonitor> GetSystemSnapshotAsync()
    {
        var monitor = new SystemMonitor
        {
            Timestamp = DateTime.UtcNow
        };

        var lots = await _lotRepo.GetAllAsync();
        foreach (var lot in lots)
        {
            monitor.TotalLots++;
            switch (lot.Status)
            {
                case "Processing": monitor.ProcessingLots++; break;
                case "Waiting": monitor.WaitingLots++; break;
                case "Hold": monitor.HoldLots++; break;
                case "Completed": monitor.CompletedLots++; break;
            }
        }

        var equipments = await _equipRepo.GetAllAsync();
        foreach (var eq in equipments)
        {
            switch (eq.Status)
            {
                case "Available": monitor.AvailableEquipments++; break;
                case "Running": monitor.RunningEquipments++; break;
                case "Offline": monitor.OfflineEquipments++; break;
            }
        }

        var carriers = await _carrierRepo.GetAllAsync();
        foreach (var c in carriers)
        {
            if (c.Status == "Available") monitor.AvailableCarriers++;
            else if (c.Status == "InUse") monitor.InUseCarriers++;
        }

        monitor.SystemUptime = 99.9;

        if (monitor.HoldLots > 0)
        {
            monitor.Alerts.Add(new AlertInfo
            {
                AlertType = "HoldLots",
                Severity = "Warning",
                Message = $"当前有 {monitor.HoldLots} 个批次处于 Hold 状态",
                TriggeredAt = DateTime.UtcNow
            });
        }

        return monitor;
    }

    public async Task<List<EquipmentUtilization>> GetEquipmentUtilizationAsync(DateTime date)
    {
        var equipments = await _equipRepo.GetAllAsync();
        return equipments.Select(eq => new EquipmentUtilization
        {
            EquipmentId = eq.EquipmentId,
            EquipmentName = eq.EquipmentName,
            Utilization = eq.Status == "Running" ? 100 : eq.Status == "Available" ? 0 : 0,
            RunningHours = eq.RunningHours,
            IdleHours = 0,
            MaintenanceHours = 0
        }).ToList();
    }
}
