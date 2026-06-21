using MES.Modules.EHS.Models;

namespace MES.Modules.EHS.Services;

/// <summary>
/// EHS service interface for managing environment monitoring, gas monitoring, 
/// chemical management, ESD monitoring, and safety checks.
/// </summary>
public interface IEHSService
{
    // Environment Monitor
    Task<List<EnvironmentMonitorItem>> GetEnvironmentMonitorDataAsync();
    Task<bool> UpdateEnvironmentMonitorAsync(EnvironmentMonitorItem item);

    // Gas Monitor
    Task<List<GasMonitorItem>> GetGasMonitorDataAsync();
    Task<bool> UpdateGasMonitorAsync(GasMonitorItem item);

    // Chemical Management
    Task<List<ChemicalItem>> GetChemicalDataAsync();
    Task<bool> AddChemicalAsync(ChemicalItem item);
    Task<bool> UpdateChemicalAsync(ChemicalItem item);
    Task<bool> DeleteChemicalAsync(string chemicalId);

    // ESD Monitor
    Task<List<EsdMonitorItem>> GetEsdMonitorDataAsync();
    Task<bool> RecordEsdTestAsync(EsdMonitorItem item);
    Task<EsdStatistics> GetEsdStatisticsAsync();

    // Safety Check
    Task<List<SafetyCheckItem>> GetSafetyCheckItemsAsync();
    Task<bool> AddSafetyCheckAsync(SafetyCheckItem item);
    Task<bool> UpdateSafetyCheckAsync(SafetyCheckItem item);
    Task<bool> CompleteSafetyCheckAsync(string checkId, string result, string findings);
    Task<SafetyCheckStatistics> GetSafetyCheckStatisticsAsync();
}

public class EsdStatistics
{
    public int TotalStations { get; set; }
    public int NormalCount { get; set; }
    public int WarningCount { get; set; }
    public int AlarmCount { get; set; }
    public double AvgWriststrapResistance { get; set; }
    public int OverdueTests { get; set; }
}

public class SafetyCheckStatistics
{
    public int TotalChecks { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public int PendingCount { get; set; }
    public int OverdueCount { get; set; }
    public double PassRate { get; set; }
}

public class InMemoryEHSService : IEHSService
{
    private readonly List<EnvironmentMonitorItem> _environmentData = new();
    private readonly List<GasMonitorItem> _gasData = new();
    private readonly List<ChemicalItem> _chemicalData = new();
    private readonly List<EsdMonitorItem> _esdData = new();
    private readonly List<SafetyCheckItem> _safetyChecks = new();

    public InMemoryEHSService()
    {
        SeedData();
    }

    private void SeedData()
    {
        _environmentData.AddRange(new[]
        {
            new EnvironmentMonitorItem { Area = "Fab-1/Bay-1", Temperature = 22.3, Humidity = 43.5, Particles = 12, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
            new EnvironmentMonitorItem { Area = "Fab-1/Bay-2", Temperature = 22.8, Humidity = 44.1, Particles = 15, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
            new EnvironmentMonitorItem { Area = "Fab-1/Bay-3", Temperature = 23.5, Humidity = 46.2, Particles = 28, TempStatus = "Warning", HumidityStatus = "Warning", ParticleStatus = "Normal" },
            new EnvironmentMonitorItem { Area = "Fab-1/Bay-4", Temperature = 24.1, Humidity = 48.0, Particles = 45, TempStatus = "Alarm", HumidityStatus = "Alarm", ParticleStatus = "Warning" },
            new EnvironmentMonitorItem { Area = "Fab-2/Bay-1", Temperature = 22.1, Humidity = 42.8, Particles = 8, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
        });

        _gasData.AddRange(new[]
        {
            new GasMonitorItem { GasType = "SiH4", Location = "Gas-Cabinet-1", Concentration = 0.5, Unit = "ppm", Threshold = 5.0, Status = "Normal" },
            new GasMonitorItem { GasType = "PH3", Location = "Gas-Cabinet-1", Concentration = 0.1, Unit = "ppm", Threshold = 0.5, Status = "Normal" },
            new GasMonitorItem { GasType = "AsH3", Location = "Gas-Cabinet-2", Concentration = 0.05, Unit = "ppm", Threshold = 0.05, Status = "Warning" },
            new GasMonitorItem { GasType = "NF3", Location = "Gas-Cabinet-2", Concentration = 1.2, Unit = "ppm", Threshold = 10.0, Status = "Normal" },
            new GasMonitorItem { GasType = "Cl2", Location = "Gas-Cabinet-3", Concentration = 0.3, Unit = "ppm", Threshold = 1.0, Status = "Normal" },
        });

        _chemicalData.AddRange(new[]
        {
            new ChemicalItem { ChemicalName = "EMC G700", CAS = "N/A-EMC-700", Quantity = 500.0, Unit = "kg", Location = "Chem-Store-C", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(6) },
            new ChemicalItem { ChemicalName = "Underfill UF-300", CAS = "N/A-UF-300", Quantity = 300.0, Unit = "kg", Location = "Chem-Store-C", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(3) },
            new ChemicalItem { ChemicalName = "Isopropyl Alcohol (IPA)", CAS = "67-63-0", Quantity = 250.0, Unit = "L", Location = "Chem-Store-B", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(12) },
        });

        _esdData.AddRange(new[]
        {
            new EsdMonitorItem { Id = "ESD-001", StationId = "ST-001", StationName = "WB-01", OperatorId = "OP-001", OperatorName = "张伟", WriststrapResistance = 0.8, FloorResistance = 1.2, IonizerBalance = 5, Status = "Normal", LastTestTime = DateTime.Now.AddHours(-1), WriststrapTest = true, FloorTest = true, IonizerTest = true },
            new EsdMonitorItem { Id = "ESD-002", StationId = "ST-002", StationName = "WB-02", OperatorId = "OP-002", OperatorName = "李明", WriststrapResistance = 1.2, FloorResistance = 0.9, IonizerBalance = 8, Status = "Normal", LastTestTime = DateTime.Now.AddHours(-2), WriststrapTest = true, FloorTest = true, IonizerTest = true },
            new EsdMonitorItem { Id = "ESD-003", StationId = "ST-003", StationName = "DB-01", OperatorId = "OP-003", OperatorName = "王芳", WriststrapResistance = 3.5, FloorResistance = 2.1, IonizerBalance = 25, Status = "Warning", LastTestTime = DateTime.Now.AddHours(-4), WriststrapTest = true, FloorTest = false, IonizerTest = false },
            new EsdMonitorItem { Id = "ESD-004", StationId = "ST-004", StationName = "MP-01", OperatorId = "OP-004", OperatorName = "赵强", WriststrapResistance = 0.5, FloorResistance = 1.0, IonizerBalance = 3, Status = "Normal", LastTestTime = DateTime.Now.AddMinutes(-30), WriststrapTest = true, FloorTest = true, IonizerTest = true },
            new EsdMonitorItem { Id = "ESD-005", StationId = "ST-005", StationName = "TH-01", OperatorId = "OP-005", OperatorName = "陈磊", WriststrapResistance = 5.0, FloorResistance = 4.5, IonizerBalance = 45, Status = "Alarm", LastTestTime = DateTime.Now.AddHours(-8), WriststrapTest = false, FloorTest = false, IonizerTest = false },
        });

        _safetyChecks.AddRange(new[]
        {
            new SafetyCheckItem { Id = "SC-001", CheckCategory = "消防安全", CheckItemName = "灭火器压力检查", CheckLocation = "Fab-1", Frequency = "Weekly", Result = "Pass", InspectorName = "张安全", InspectionDate = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(6) },
            new SafetyCheckItem { Id = "SC-002", CheckCategory = "电气安全", CheckItemName = "接地电阻测试", CheckLocation = "Fab-2", Frequency = "Monthly", Result = "Pass", InspectorName = "李电工", InspectionDate = DateTime.Now.AddDays(-5), DueDate = DateTime.Now.AddDays(25) },
            new SafetyCheckItem { Id = "SC-003", CheckCategory = "化学品安全", CheckItemName = "化学品存储检查", CheckLocation = "Chem-Store-A", Frequency = "Daily", Result = "Fail", Findings = "部分化学品标签脱落", InspectorName = "王化验", InspectionDate = DateTime.Now.AddHours(-4), DueDate = DateTime.Now.AddHours(-4), IsOverdue = false, RemediationPlan = "重新贴标", RemediationStatus = "InProgress" },
            new SafetyCheckItem { Id = "SC-004", CheckCategory = "通风系统", CheckItemName = "排风量测试", CheckLocation = "Utility", Frequency = "Weekly", Result = "Pending", InspectorName = "", InspectionDate = DateTime.MinValue, DueDate = DateTime.Now.AddDays(-1), IsOverdue = true },
            new SafetyCheckItem { Id = "SC-005", CheckCategory = "ESD防护", CheckItemName = "防静电手环检测", CheckLocation = "Fab-1", Frequency = "Daily", Result = "Pass", InspectorName = "赵质量", InspectionDate = DateTime.Now.AddHours(-2), DueDate = DateTime.Now.AddHours(22) },
        });
    }

    public Task<List<EnvironmentMonitorItem>> GetEnvironmentMonitorDataAsync()
        => Task.FromResult(_environmentData.ToList());

    public Task<bool> UpdateEnvironmentMonitorAsync(EnvironmentMonitorItem item)
    {
        var existing = _environmentData.FirstOrDefault(e => e.Area == item.Area);
        if (existing != null)
        {
            existing.Temperature = item.Temperature;
            existing.Humidity = item.Humidity;
            existing.Particles = item.Particles;
            existing.TempStatus = item.TempStatus;
            existing.HumidityStatus = item.HumidityStatus;
            existing.ParticleStatus = item.ParticleStatus;
        }
        return Task.FromResult(true);
    }

    public Task<List<GasMonitorItem>> GetGasMonitorDataAsync()
        => Task.FromResult(_gasData.ToList());

    public Task<bool> UpdateGasMonitorAsync(GasMonitorItem item)
    {
        var existing = _gasData.FirstOrDefault(g => g.GasType == item.GasType && g.Location == item.Location);
        if (existing != null)
        {
            existing.Concentration = item.Concentration;
            existing.Status = item.Status;
        }
        return Task.FromResult(true);
    }

    public Task<List<ChemicalItem>> GetChemicalDataAsync()
        => Task.FromResult(_chemicalData.ToList());

    public Task<bool> AddChemicalAsync(ChemicalItem item)
    {
        _chemicalData.Add(item);
        return Task.FromResult(true);
    }

    public Task<bool> UpdateChemicalAsync(ChemicalItem item)
    {
        var existing = _chemicalData.FirstOrDefault(c => c.ChemicalName == item.ChemicalName);
        if (existing != null)
        {
            existing.Quantity = item.Quantity;
            existing.Location = item.Location;
            existing.MsdsStatus = item.MsdsStatus;
            existing.ExpiryDate = item.ExpiryDate;
        }
        return Task.FromResult(true);
    }

    public Task<bool> DeleteChemicalAsync(string chemicalId)
    {
        var item = _chemicalData.FirstOrDefault(c => c.ChemicalName == chemicalId);
        if (item != null)
            _chemicalData.Remove(item);
        return Task.FromResult(true);
    }

    public Task<List<EsdMonitorItem>> GetEsdMonitorDataAsync()
        => Task.FromResult(_esdData.ToList());

    public Task<bool> RecordEsdTestAsync(EsdMonitorItem item)
    {
        var existing = _esdData.FirstOrDefault(e => e.Id == item.Id);
        if (existing != null)
        {
            existing.WriststrapResistance = item.WriststrapResistance;
            existing.FloorResistance = item.FloorResistance;
            existing.IonizerBalance = item.IonizerBalance;
            existing.LastTestTime = DateTime.Now;
            existing.WriststrapTest = item.WriststrapTest;
            existing.FloorTest = item.FloorTest;
            existing.IonizerTest = item.IonizerTest;
            existing.Status = DetermineEsdStatus(item);
        }
        return Task.FromResult(true);
    }

    public Task<EsdStatistics> GetEsdStatisticsAsync()
    {
        return Task.FromResult(new EsdStatistics
        {
            TotalStations = _esdData.Count,
            NormalCount = _esdData.Count(e => e.Status == "Normal"),
            WarningCount = _esdData.Count(e => e.Status == "Warning"),
            AlarmCount = _esdData.Count(e => e.Status == "Alarm"),
            AvgWriststrapResistance = _esdData.Average(e => e.WriststrapResistance),
            OverdueTests = _esdData.Count(e => e.LastTestTime < DateTime.Now.AddHours(-4))
        });
    }

    public Task<List<SafetyCheckItem>> GetSafetyCheckItemsAsync()
        => Task.FromResult(_safetyChecks.ToList());

    public Task<bool> AddSafetyCheckAsync(SafetyCheckItem item)
    {
        _safetyChecks.Add(item);
        return Task.FromResult(true);
    }

    public Task<bool> UpdateSafetyCheckAsync(SafetyCheckItem item)
    {
        var existing = _safetyChecks.FirstOrDefault(s => s.Id == item.Id);
        if (existing != null)
        {
            existing.CheckCategory = item.CheckCategory;
            existing.CheckItemName = item.CheckItemName;
            existing.CheckLocation = item.CheckLocation;
            existing.Frequency = item.Frequency;
            existing.DueDate = item.DueDate;
            existing.RemediationPlan = item.RemediationPlan;
            existing.RemediationStatus = item.RemediationStatus;
        }
        return Task.FromResult(true);
    }

    public Task<bool> CompleteSafetyCheckAsync(string checkId, string result, string findings)
    {
        var item = _safetyChecks.FirstOrDefault(s => s.Id == checkId);
        if (item != null)
        {
            item.Result = result;
            item.Findings = findings;
            item.InspectorName = "CurrentUser";
            item.InspectionDate = DateTime.Now;
            item.IsOverdue = false;
            if (result == "Fail")
            {
                item.RemediationStatus = "NotStarted";
            }
        }
        return Task.FromResult(true);
    }

    public Task<SafetyCheckStatistics> GetSafetyCheckStatisticsAsync()
    {
        var total = _safetyChecks.Count;
        var passed = _safetyChecks.Count(s => s.Result == "Pass");
        var failed = _safetyChecks.Count(s => s.Result == "Fail");
        var pending = _safetyChecks.Count(s => s.Result == "Pending");
        var overdue = _safetyChecks.Count(s => s.IsOverdue);

        return Task.FromResult(new SafetyCheckStatistics
        {
            TotalChecks = total,
            PassedCount = passed,
            FailedCount = failed,
            PendingCount = pending,
            OverdueCount = overdue,
            PassRate = total > 0 ? (double)(passed + failed) / (passed + failed) * 100 : 0
        });
    }

    private string DetermineEsdStatus(EsdMonitorItem item)
    {
        bool allPassed = item.WriststrapTest && item.FloorTest && item.IonizerTest;
        if (allPassed) return "Normal";

        bool hasCritical = !item.WriststrapTest || item.WriststrapResistance > 3.5;
        if (hasCritical) return "Alarm";

        return "Warning";
    }
}
