using Prism.Mvvm;
using Prism.Commands;
using MES.Modules.EHS.Models;
using MES.Modules.EHS.Services;
using System.Collections.ObjectModel;

namespace MES.Modules.EHS.ViewModels;

public class EnvironmentMonitorViewModel : BindableBase
{
    private readonly IEHSService _ehsService;
    private ObservableCollection<EnvironmentMonitorItem> _items = [];
    private EnvironmentMonitorItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _totalCount;
    private int _normalCount;
    private int _warningCount;
    private int _alarmCount;

    public ObservableCollection<EnvironmentMonitorItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public EnvironmentMonitorItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int NormalCount { get => _normalCount; set => SetProperty(ref _normalCount, value); }
    public int WarningCount { get => _warningCount; set => SetProperty(ref _warningCount, value); }
    public int AlarmCount { get => _alarmCount; set => SetProperty(ref _alarmCount, value); }

    public DelegateCommand RefreshCommand { get; }

    public EnvironmentMonitorViewModel(IEHSService ehsService)
    {
        _ehsService = ehsService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _ehsService.GetEnvironmentMonitorDataAsync();
            Items = new ObservableCollection<EnvironmentMonitorItem>(data);
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        NormalCount = Items.Count(e => e.TempStatus == "Normal" && e.HumidityStatus == "Normal" && e.ParticleStatus == "Normal");
        WarningCount = Items.Count(e => e.TempStatus == "Warning" || e.HumidityStatus == "Warning" || e.ParticleStatus == "Warning");
        AlarmCount = Items.Count(e => e.TempStatus == "Alarm" || e.HumidityStatus == "Alarm" || e.ParticleStatus == "Alarm");
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<EnvironmentMonitorItem>
        {
            new() { Area = "Fab-1/Bay-1", Temperature = 22.3, Humidity = 43.5, Particles = 12, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
            new() { Area = "Fab-1/Bay-2", Temperature = 22.8, Humidity = 44.1, Particles = 15, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
            new() { Area = "Fab-1/Bay-3", Temperature = 23.5, Humidity = 46.2, Particles = 28, TempStatus = "Warning", HumidityStatus = "Warning", ParticleStatus = "Normal" },
            new() { Area = "Fab-1/Bay-4", Temperature = 24.1, Humidity = 48.0, Particles = 45, TempStatus = "Alarm", HumidityStatus = "Alarm", ParticleStatus = "Warning" },
            new() { Area = "Fab-2/Bay-1", Temperature = 22.1, Humidity = 42.8, Particles = 8, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
            new() { Area = "Fab-2/Bay-2", Temperature = 22.5, Humidity = 43.2, Particles = 10, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
            new() { Area = "Fab-2/Bay-3", Temperature = 21.8, Humidity = 41.5, Particles = 52, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Alarm" },
            new() { Area = "Utility/Chiller", Temperature = 25.0, Humidity = 50.0, Particles = 5, TempStatus = "Normal", HumidityStatus = "Normal", ParticleStatus = "Normal" },
        };
        UpdateStatistics();
    }
}

public class GasMonitorViewModel : BindableBase
{
    private readonly IEHSService _ehsService;
    private ObservableCollection<GasMonitorItem> _items = [];
    private GasMonitorItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _totalCount;
    private int _normalCount;
    private int _warningCount;

    public ObservableCollection<GasMonitorItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public GasMonitorItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int NormalCount { get => _normalCount; set => SetProperty(ref _normalCount, value); }
    public int WarningCount { get => _warningCount; set => SetProperty(ref _warningCount, value); }

    public DelegateCommand RefreshCommand { get; }

    public GasMonitorViewModel(IEHSService ehsService)
    {
        _ehsService = ehsService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _ehsService.GetGasMonitorDataAsync();
            Items = new ObservableCollection<GasMonitorItem>(data);
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        NormalCount = Items.Count(g => g.Status == "Normal");
        WarningCount = Items.Count(g => g.Status == "Warning");
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<GasMonitorItem>
        {
            new() { GasType = "SiH4", Location = "Gas-Cabinet-1", Concentration = 0.5, Unit = "ppm", Threshold = 5.0, Status = "Normal" },
            new() { GasType = "PH3", Location = "Gas-Cabinet-1", Concentration = 0.1, Unit = "ppm", Threshold = 0.5, Status = "Normal" },
            new() { GasType = "AsH3", Location = "Gas-Cabinet-2", Concentration = 0.05, Unit = "ppm", Threshold = 0.05, Status = "Warning" },
            new() { GasType = "NF3", Location = "Gas-Cabinet-2", Concentration = 1.2, Unit = "ppm", Threshold = 10.0, Status = "Normal" },
            new() { GasType = "Cl2", Location = "Gas-Cabinet-3", Concentration = 0.3, Unit = "ppm", Threshold = 1.0, Status = "Normal" },
            new() { GasType = "H2", Location = "Gas-Cabinet-3", Concentration = 0.8, Unit = "ppm", Threshold = 4.0, Status = "Normal" },
            new() { GasType = "HF", Location = "Exhaust-1", Concentration = 1.5, Unit = "ppm", Threshold = 2.0, Status = "Warning" },
            new() { GasType = "HCl", Location = "Exhaust-2", Concentration = 0.4, Unit = "ppm", Threshold = 5.0, Status = "Normal" },
            new() { GasType = "CO", Location = "Fab-1/General", Concentration = 2.0, Unit = "ppm", Threshold = 25.0, Status = "Normal" },
            new() { GasType = "O3", Location = "Fab-1/General", Concentration = 0.05, Unit = "ppm", Threshold = 0.1, Status = "Normal" },
        };
        UpdateStatistics();
    }
}

public class ChemicalManagementViewModel : BindableBase
{
    private readonly IEHSService _ehsService;
    private ObservableCollection<ChemicalItem> _items = [];
    private ChemicalItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _totalCount;
    private int _validMsdsCount;
    private int _expiredMsdsCount;
    private int _expiringCount;

    public ObservableCollection<ChemicalItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public ChemicalItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int ValidMsdsCount { get => _validMsdsCount; set => SetProperty(ref _validMsdsCount, value); }
    public int ExpiredMsdsCount { get => _expiredMsdsCount; set => SetProperty(ref _expiredMsdsCount, value); }
    public int ExpiringCount { get => _expiringCount; set => SetProperty(ref _expiringCount, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AddCommand { get; }
    public DelegateCommand DeleteCommand { get; }

    public ChemicalManagementViewModel(IEHSService ehsService)
    {
        _ehsService = ehsService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        AddCommand = new DelegateCommand(AddNewChemical);
        DeleteCommand = new DelegateCommand(async () => await DeleteSelectedAsync(), () => SelectedItem != null);
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _ehsService.GetChemicalDataAsync();
            Items = new ObservableCollection<ChemicalItem>(data);
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteSelectedAsync()
    {
        if (SelectedItem == null) return;
        IsLoading = true;
        try
        {
            await _ehsService.DeleteChemicalAsync(SelectedItem.ChemicalName);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            UpdateStatistics();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddNewChemical()
    {
        var newChemical = new ChemicalItem
        {
            ChemicalName = "新化学品",
            CAS = "N/A-NEW",
            Quantity = 0,
            Unit = "kg",
            Location = "Chem-Store-A",
            MsdsStatus = "Valid",
            ExpiryDate = DateTime.Now.AddMonths(12)
        };
        Items.Add(newChemical);
        SelectedItem = newChemical;
        TotalCount = Items.Count;
    }

    private void UpdateStatistics()
    {
        TotalCount = Items.Count;
        ValidMsdsCount = Items.Count(c => c.MsdsStatus == "Valid");
        ExpiredMsdsCount = Items.Count(c => c.MsdsStatus == "Expired");
        ExpiringCount = Items.Count(c => c.MsdsStatus == "Expiring");
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<ChemicalItem>
        {
            new() { ChemicalName = "EMC G700", CAS = "N/A-EMC-700", Quantity = 500.0, Unit = "kg", Location = "Chem-Store-C", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(6) },
            new() { ChemicalName = "Underfill UF-300", CAS = "N/A-UF-300", Quantity = 300.0, Unit = "kg", Location = "Chem-Store-C", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(3) },
            new() { ChemicalName = "Flux FM-280", CAS = "N/A-FX-280", Quantity = 100.0, Unit = "L", Location = "Chem-Store-D", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(4) },
            new() { ChemicalName = "Solder Paste SAC305", CAS = "N/A-SP-305", Quantity = 50.0, Unit = "kg", Location = "Chem-Store-A", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(8) },
            new() { ChemicalName = "Dicing Tape DT-200", CAS = "N/A-DT-200", Quantity = 200.0, Unit = "卷", Location = "Chem-Store-A", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(5) },
            new() { ChemicalName = "Mold Compound MC-15", CAS = "N/A-MC-015", Quantity = 100.0, Unit = "kg", Location = "Chem-Store-B", MsdsStatus = "Expired", ExpiryDate = DateTime.Now.AddMonths(-1) },
            new() { ChemicalName = "Isopropyl Alcohol (IPA)", CAS = "67-63-0", Quantity = 250.0, Unit = "L", Location = "Chem-Store-B", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(12) },
            new() { ChemicalName = "Conductive Adhesive CA-80", CAS = "N/A-CA-080", Quantity = 80.0, Unit = "kg", Location = "Chem-Store-D", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(2) },
            new() { ChemicalName = "Cleaning Solvent CS-40", CAS = "N/A-CS-040", Quantity = 150.0, Unit = "L", Location = "Chem-Store-D", MsdsStatus = "Expiring", ExpiryDate = DateTime.Now.AddDays(14) },
            new() { ChemicalName = "Die Attach Film DAF-10", CAS = "N/A-DAF-010", Quantity = 60.0, Unit = "卷", Location = "Chem-Store-D", MsdsStatus = "Valid", ExpiryDate = DateTime.Now.AddMonths(9) },
        };
        UpdateStatistics();
    }
}

public class EsdMonitorViewModel : BindableBase
{
    private readonly IEHSService _ehsService;
    private ObservableCollection<EsdMonitorItem> _items = [];
    private EsdMonitorItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private int _totalCount;
    private int _normalCount;
    private int _warningCount;
    private int _alarmCount;
    private int _overdueTests;
    private double _avgResistance;

    public ObservableCollection<EsdMonitorItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public EsdMonitorItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int NormalCount { get => _normalCount; set => SetProperty(ref _normalCount, value); }
    public int WarningCount { get => _warningCount; set => SetProperty(ref _warningCount, value); }
    public int AlarmCount { get => _alarmCount; set => SetProperty(ref _alarmCount, value); }
    public int OverdueTests { get => _overdueTests; set => SetProperty(ref _overdueTests, value); }
    public double AvgResistance { get => _avgResistance; set => SetProperty(ref _avgResistance, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand RecordTestCommand { get; }
    public DelegateCommand RetestCommand { get; }

    public EsdMonitorViewModel(IEHSService ehsService)
    {
        _ehsService = ehsService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        RecordTestCommand = new DelegateCommand(async () => await RecordNewTestAsync());
        RetestCommand = new DelegateCommand(async () => await RetestSelectedAsync(), () => SelectedItem != null);
        LoadMockData();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _ehsService.GetEsdMonitorDataAsync();
            Items = new ObservableCollection<EsdMonitorItem>(data);
            var stats = await _ehsService.GetEsdStatisticsAsync();
            UpdateStatistics(stats);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RecordNewTestAsync()
    {
        var random = new Random();
        var newTest = new EsdMonitorItem
        {
            Id = $"ESD-{_items.Count + 1:D3}",
            StationId = $"ST-{random.Next(100, 999)}",
            StationName = $"WS-{random.Next(1, 99)}",
            OperatorId = $"OP-{random.Next(100, 999)}",
            OperatorName = "操作员",
            WriststrapResistance = Math.Round(random.NextDouble() * 2 + 0.5, 1),
            FloorResistance = Math.Round(random.NextDouble() * 2 + 0.5, 1),
            IonizerBalance = random.Next(0, 15),
            LastTestTime = DateTime.Now,
            WriststrapTest = true,
            FloorTest = true,
            IonizerTest = true
        };

        IsLoading = true;
        try
        {
            await _ehsService.RecordEsdTestAsync(newTest);
            Items.Add(newTest);
            var stats = await _ehsService.GetEsdStatisticsAsync();
            UpdateStatistics(stats);
            ErrorMessage = $"新测试记录已添加到 {newTest.StationName}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"记录失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RetestSelectedAsync()
    {
        if (SelectedItem == null) return;
        IsLoading = true;
        try
        {
            SelectedItem.LastTestTime = DateTime.Now;
            SelectedItem.WriststrapTest = true;
            SelectedItem.FloorTest = true;
            SelectedItem.IonizerTest = true;
            SelectedItem.Status = "Normal";
            await _ehsService.RecordEsdTestAsync(SelectedItem);
            var stats = await _ehsService.GetEsdStatisticsAsync();
            UpdateStatistics(stats);
            ErrorMessage = $"工位 {SelectedItem.StationName} 重新测试完成";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"重新测试失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateStatistics(EsdStatistics? stats = null)
    {
        if (stats != null)
        {
            TotalCount = stats.TotalStations;
            NormalCount = stats.NormalCount;
            WarningCount = stats.WarningCount;
            AlarmCount = stats.AlarmCount;
            AvgResistance = stats.AvgWriststrapResistance;
            OverdueTests = stats.OverdueTests;
        }
        else
        {
            TotalCount = Items.Count;
            NormalCount = Items.Count(e => e.Status == "Normal");
            WarningCount = Items.Count(e => e.Status == "Warning");
            AlarmCount = Items.Count(e => e.Status == "Alarm");
            AvgResistance = Items.Any() ? Items.Average(e => e.WriststrapResistance) : 0;
            OverdueTests = Items.Count(e => e.LastTestTime < DateTime.Now.AddHours(-4));
        }
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<EsdMonitorItem>
        {
            new() { Id = "ESD-001", StationId = "ST-001", StationName = "WB-01", OperatorId = "OP-001", OperatorName = "张伟", WriststrapResistance = 0.8, FloorResistance = 1.2, IonizerBalance = 5, Status = "Normal", LastTestTime = DateTime.Now.AddHours(-1), WriststrapTest = true, FloorTest = true, IonizerTest = true },
            new() { Id = "ESD-002", StationId = "ST-002", StationName = "WB-02", OperatorId = "OP-002", OperatorName = "李明", WriststrapResistance = 1.2, FloorResistance = 0.9, IonizerBalance = 8, Status = "Normal", LastTestTime = DateTime.Now.AddHours(-2), WriststrapTest = true, FloorTest = true, IonizerTest = true },
            new() { Id = "ESD-003", StationId = "ST-003", StationName = "DB-01", OperatorId = "OP-003", OperatorName = "王芳", WriststrapResistance = 3.5, FloorResistance = 2.1, IonizerBalance = 25, Status = "Warning", LastTestTime = DateTime.Now.AddHours(-4), WriststrapTest = true, FloorTest = false, IonizerTest = false },
            new() { Id = "ESD-004", StationId = "ST-004", StationName = "MP-01", OperatorId = "OP-004", OperatorName = "赵强", WriststrapResistance = 0.5, FloorResistance = 1.0, IonizerBalance = 3, Status = "Normal", LastTestTime = DateTime.Now.AddMinutes(-30), WriststrapTest = true, FloorTest = true, IonizerTest = true },
            new() { Id = "ESD-005", StationId = "ST-005", StationName = "TH-01", OperatorId = "OP-005", OperatorName = "陈磊", WriststrapResistance = 5.0, FloorResistance = 4.5, IonizerBalance = 45, Status = "Alarm", LastTestTime = DateTime.Now.AddHours(-8), WriststrapTest = false, FloorTest = false, IonizerTest = false },
        };
        UpdateStatistics();
    }
}

public class SafetyCheckViewModel : BindableBase
{
    private readonly IEHSService _ehsService;
    private ObservableCollection<SafetyCheckItem> _items = [];
    private SafetyCheckItem? _selectedItem;
    private string _errorMessage = string.Empty;
    private bool _isLoading;
    private string _filterCategory = string.Empty;
    private int _totalCount;
    private int _passCount;
    private int _failCount;
    private int _pendingCount;
    private int _overdueCount;
    private double _passRate;
    private string _findings = string.Empty;
    private string _testResult = "Pass";

    public ObservableCollection<SafetyCheckItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public SafetyCheckItem? SelectedItem { get => _selectedItem; set { SetProperty(ref _selectedItem, value); UpdateCommandStates(); } }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string FilterCategory { get => _filterCategory; set { SetProperty(ref _filterCategory, value); ApplyFilter(); } }
    public int TotalCount { get => _totalCount; set => SetProperty(ref _totalCount, value); }
    public int PassCount { get => _passCount; set => SetProperty(ref _passCount, value); }
    public int FailCount { get => _failCount; set => SetProperty(ref _failCount, value); }
    public int PendingCount { get => _pendingCount; set => SetProperty(ref _pendingCount, value); }
    public int OverdueCount { get => _overdueCount; set => SetProperty(ref _overdueCount, value); }
    public double PassRate { get => _passRate; set => SetProperty(ref _passRate, value); }
    public string Findings { get => _findings; set => SetProperty(ref _findings, value); }
    public string TestResult { get => _testResult; set => SetProperty(ref _testResult, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand CompleteCheckCommand { get; }
    public DelegateCommand AddCheckCommand { get; }
    public DelegateCommand OverdueChecksCommand { get; }

    public SafetyCheckViewModel(IEHSService ehsService)
    {
        _ehsService = ehsService;
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        CompleteCheckCommand = new DelegateCommand(async () => await CompleteSelectedCheckAsync(), () => SelectedItem?.Result == "Pending");
        AddCheckCommand = new DelegateCommand(AddNewCheck);
        OverdueChecksCommand = new DelegateCommand(FilterOverdueChecks);
        LoadMockData();
    }

    private void UpdateCommandStates()
    {
        CompleteCheckCommand.RaiseCanExecuteChanged();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _ehsService.GetSafetyCheckItemsAsync();
            Items = new ObservableCollection<SafetyCheckItem>(data);
            var stats = await _ehsService.GetSafetyCheckStatisticsAsync();
            UpdateStatistics(stats);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"加载失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CompleteSelectedCheckAsync()
    {
        if (SelectedItem == null || SelectedItem.Result == "Pending") return;
        if (string.IsNullOrWhiteSpace(Findings) && TestResult == "Fail")
        {
            ErrorMessage = "检查失败时必须填写发现内容";
            return;
        }

        IsLoading = true;
        try
        {
            await _ehsService.CompleteSafetyCheckAsync(SelectedItem.Id, TestResult, Findings);
            SelectedItem.Result = TestResult;
            SelectedItem.Findings = Findings;
            SelectedItem.InspectorName = "CurrentUser";
            SelectedItem.InspectionDate = DateTime.Now;
            SelectedItem.IsOverdue = false;
            Findings = string.Empty;

            var stats = await _ehsService.GetSafetyCheckStatisticsAsync();
            UpdateStatistics(stats);
            UpdateCommandStates();
            ErrorMessage = $"安全检查 {SelectedItem.CheckItemName} 已完成";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"完成检查失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddNewCheck()
    {
        var newCheck = new SafetyCheckItem
        {
            Id = $"SC-{_items.Count + 1:D3}",
            CheckCategory = "消防安全",
            CheckItemName = "新检查项目",
            CheckLocation = "Fab-1",
            Frequency = "Daily",
            Result = "Pending",
            DueDate = DateTime.Now.AddDays(1)
        };
        Items.Add(newCheck);
        SelectedItem = newCheck;
        TotalCount = Items.Count;
    }

    private void FilterOverdueChecks()
    {
        var overdue = Items.Where(s => s.IsOverdue).ToList();
        Items = new ObservableCollection<SafetyCheckItem>(overdue);
        ErrorMessage = $"找到 {overdue.Count} 条逾期检查";
    }

    private void ApplyFilter()
    {
        if (string.IsNullOrWhiteSpace(FilterCategory))
        {
            LoadMockData();
            return;
        }

        var filtered = Items.Where(s =>
            s.CheckCategory.Contains(FilterCategory, StringComparison.OrdinalIgnoreCase) ||
            s.CheckItemName.Contains(FilterCategory, StringComparison.OrdinalIgnoreCase) ||
            s.CheckLocation.Contains(FilterCategory, StringComparison.OrdinalIgnoreCase)).ToList();

        Items = new ObservableCollection<SafetyCheckItem>(filtered);
    }

    private void UpdateStatistics(SafetyCheckStatistics? stats = null)
    {
        if (stats != null)
        {
            TotalCount = stats.TotalChecks;
            PassCount = stats.PassedCount;
            FailCount = stats.FailedCount;
            PendingCount = stats.PendingCount;
            OverdueCount = stats.OverdueCount;
            PassRate = stats.PassRate;
        }
        else
        {
            TotalCount = Items.Count;
            PassCount = Items.Count(s => s.Result == "Pass");
            FailCount = Items.Count(s => s.Result == "Fail");
            PendingCount = Items.Count(s => s.Result == "Pending");
            OverdueCount = Items.Count(s => s.IsOverdue);
            PassRate = (PassCount + FailCount) > 0 ? (double)PassCount / (PassCount + FailCount) * 100 : 0;
        }
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<SafetyCheckItem>
        {
            new() { Id = "SC-001", CheckCategory = "消防安全", CheckItemName = "灭火器压力检查", CheckLocation = "Fab-1", Frequency = "Weekly", Result = "Pass", InspectorName = "张安全", InspectionDate = DateTime.Now.AddDays(-1), DueDate = DateTime.Now.AddDays(6) },
            new() { Id = "SC-002", CheckCategory = "电气安全", CheckItemName = "接地电阻测试", CheckLocation = "Fab-2", Frequency = "Monthly", Result = "Pass", InspectorName = "李电工", InspectionDate = DateTime.Now.AddDays(-5), DueDate = DateTime.Now.AddDays(25) },
            new() { Id = "SC-003", CheckCategory = "化学品安全", CheckItemName = "化学品存储检查", CheckLocation = "Chem-Store-A", Frequency = "Daily", Result = "Fail", Findings = "部分化学品标签脱落", InspectorName = "王化验", InspectionDate = DateTime.Now.AddHours(-4), DueDate = DateTime.Now.AddHours(-4), IsOverdue = false, RemediationPlan = "重新贴标", RemediationStatus = "InProgress" },
            new() { Id = "SC-004", CheckCategory = "通风系统", CheckItemName = "排风量测试", CheckLocation = "Utility", Frequency = "Weekly", Result = "Pending", InspectorName = "", InspectionDate = DateTime.MinValue, DueDate = DateTime.Now.AddDays(-1), IsOverdue = true },
            new() { Id = "SC-005", CheckCategory = "ESD防护", CheckItemName = "防静电手环检测", CheckLocation = "Fab-1", Frequency = "Daily", Result = "Pass", InspectorName = "赵质量", InspectionDate = DateTime.Now.AddHours(-2), DueDate = DateTime.Now.AddHours(22) },
        };
        UpdateStatistics();
    }
}
