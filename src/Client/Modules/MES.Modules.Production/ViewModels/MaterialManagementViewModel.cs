using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class MaterialManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<MaterialInfo> _materials = [];
    private MaterialInfo? _selectedMaterial;
    private string? _errorMessage;

    public ObservableCollection<MaterialInfo> Materials
    {
        get => _materials;
        set => SetProperty(ref _materials, value);
    }

    public MaterialInfo? SelectedMaterial
    {
        get => _selectedMaterial;
        set
        {
            SetProperty(ref _selectedMaterial, value);
            RaiseCanExecuteChanged();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AddCommand { get; }
    public DelegateCommand EditCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public MaterialManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedMaterial != null);
        DeleteCommand = new DelegateCommand(OnDelete, () => SelectedMaterial != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            ErrorMessage = null;
            var materials = await _masterDataService.GetAllMaterialsAsync();
            Materials = new ObservableCollection<MaterialInfo>(materials);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private void OnAdd()
    {
        var dialog = new Views.MaterialDialogWindow(new MaterialInfo());
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
            _ = SaveMaterialAsync(dialog.Material);
    }

    private void OnEdit()
    {
        if (SelectedMaterial == null) return;
        var clone = Clone(SelectedMaterial);
        var dialog = new Views.MaterialDialogWindow(clone);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
            _ = SaveMaterialAsync(dialog.Material);
    }

    private async Task SaveMaterialAsync(MaterialInfo material)
    {
        try
        {
            await _masterDataService.SaveMaterialAsync(material);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
            MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void OnDelete()
    {
        if (SelectedMaterial == null) return;
        var result = MessageBox.Show(
            $"确认删除物料 {SelectedMaterial.MaterialName} ({SelectedMaterial.MaterialId})？",
            "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _masterDataService.DeleteMaterialAsync(SelectedMaterial.MaterialId);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
            MessageBox.Show($"删除失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RaiseCanExecuteChanged()
    {
        EditCommand.RaiseCanExecuteChanged();
        DeleteCommand.RaiseCanExecuteChanged();
    }

    private static MaterialInfo Clone(MaterialInfo m) => new()
    {
        MaterialId = m.MaterialId,
        MaterialName = m.MaterialName,
        MaterialCode = m.MaterialCode,
        MaterialType = m.MaterialType,
        Supplier = m.Supplier,
        Unit = m.Unit,
        MinStock = m.MinStock,
        Status = m.Status,
        Description = m.Description,
    };
}

public class YieldRuleManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<YieldRule> _yieldRules = [];
    private YieldRule? _selectedYieldRule;
    private string? _errorMessage;

    public ObservableCollection<YieldRule> YieldRules
    {
        get => _yieldRules;
        set => SetProperty(ref _yieldRules, value);
    }

    public YieldRule? SelectedYieldRule
    {
        get => _selectedYieldRule;
        set
        {
            SetProperty(ref _selectedYieldRule, value);
            RaiseCanExecuteChanged();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AddCommand { get; }
    public DelegateCommand EditCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public YieldRuleManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedYieldRule != null);
        DeleteCommand = new DelegateCommand(OnDelete, () => SelectedYieldRule != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            ErrorMessage = null;
            var rules = await _masterDataService.GetAllYieldRulesAsync();
            YieldRules = new ObservableCollection<YieldRule>(rules);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private void OnAdd()
    {
        var dialog = new Views.YieldRuleDialogWindow(new YieldRule());
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
            _ = SaveYieldRuleAsync(dialog.YieldRule);
    }

    private void OnEdit()
    {
        if (SelectedYieldRule == null) return;
        var clone = Clone(SelectedYieldRule);
        var dialog = new Views.YieldRuleDialogWindow(clone);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
            _ = SaveYieldRuleAsync(dialog.YieldRule);
    }

    private async Task SaveYieldRuleAsync(YieldRule rule)
    {
        try
        {
            await _masterDataService.SaveYieldRuleAsync(rule);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
            MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void OnDelete()
    {
        if (SelectedYieldRule == null) return;
        var result = MessageBox.Show(
            $"确认删除良率规则 (路线: {SelectedYieldRule.RouteId}, 工序: {SelectedYieldRule.StepCode})？",
            "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _masterDataService.DeleteYieldRuleAsync(SelectedYieldRule.RuleId);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
            MessageBox.Show($"删除失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RaiseCanExecuteChanged()
    {
        EditCommand.RaiseCanExecuteChanged();
        DeleteCommand.RaiseCanExecuteChanged();
    }

    private static YieldRule Clone(YieldRule r) => new()
    {
        RuleId = r.RuleId,
        RouteId = r.RouteId,
        StepCode = r.StepCode,
        WarningYield = r.WarningYield,
        AlarmYield = r.AlarmYield,
        HoldYield = r.HoldYield,
    };
}

public class AlarmRuleManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<AlarmRule> _alarmRules = [];
    private AlarmRule? _selectedAlarmRule;
    private string? _errorMessage;

    public ObservableCollection<AlarmRule> AlarmRules
    {
        get => _alarmRules;
        set => SetProperty(ref _alarmRules, value);
    }

    public AlarmRule? SelectedAlarmRule
    {
        get => _selectedAlarmRule;
        set
        {
            SetProperty(ref _selectedAlarmRule, value);
            RaiseCanExecuteChanged();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AddCommand { get; }
    public DelegateCommand EditCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public AlarmRuleManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedAlarmRule != null);
        DeleteCommand = new DelegateCommand(OnDelete, () => SelectedAlarmRule != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            ErrorMessage = null;
            var rules = await _masterDataService.GetAllAlarmRulesAsync();
            AlarmRules = new ObservableCollection<AlarmRule>(rules);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private void OnAdd()
    {
        var dialog = new Views.AlarmRuleDialogWindow(new AlarmRule());
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
            _ = SaveAlarmRuleAsync(dialog.AlarmRule);
    }

    private void OnEdit()
    {
        if (SelectedAlarmRule == null) return;
        var clone = Clone(SelectedAlarmRule);
        var dialog = new Views.AlarmRuleDialogWindow(clone);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
            _ = SaveAlarmRuleAsync(dialog.AlarmRule);
    }

    private async Task SaveAlarmRuleAsync(AlarmRule rule)
    {
        try
        {
            await _masterDataService.SaveAlarmRuleAsync(rule);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
            MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void OnDelete()
    {
        if (SelectedAlarmRule == null) return;
        var result = MessageBox.Show(
            $"确认删除报警规则 {SelectedAlarmRule.RuleName}？",
            "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _masterDataService.DeleteAlarmRuleAsync(SelectedAlarmRule.RuleId);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
            MessageBox.Show($"删除失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RaiseCanExecuteChanged()
    {
        EditCommand.RaiseCanExecuteChanged();
        DeleteCommand.RaiseCanExecuteChanged();
    }

    private static AlarmRule Clone(AlarmRule r) => new()
    {
        RuleId = r.RuleId,
        RuleName = r.RuleName,
        AlarmType = r.AlarmType,
        Condition = r.Condition,
        NotifyRoles = r.NotifyRoles,
        IsActive = r.IsActive,
    };
}

public class ScrapRuleManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<ScrapRule> _scrapRules = [];
    private ScrapRule? _selectedScrapRule;
    private string? _errorMessage;

    public ObservableCollection<ScrapRule> ScrapRules
    {
        get => _scrapRules;
        set => SetProperty(ref _scrapRules, value);
    }

    public ScrapRule? SelectedScrapRule
    {
        get => _selectedScrapRule;
        set
        {
            SetProperty(ref _selectedScrapRule, value);
            RaiseCanExecuteChanged();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AddCommand { get; }
    public DelegateCommand EditCommand { get; }
    public DelegateCommand DeleteCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public ScrapRuleManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedScrapRule != null);
        DeleteCommand = new DelegateCommand(OnDelete, () => SelectedScrapRule != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            ErrorMessage = null;
            var rules = await _masterDataService.GetAllScrapRulesAsync();
            ScrapRules = new ObservableCollection<ScrapRule>(rules);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private void OnAdd()
    {
        var dialog = new Views.ScrapRuleDialogWindow(new ScrapRule());
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
            _ = SaveScrapRuleAsync(dialog.ScrapRule);
    }

    private void OnEdit()
    {
        if (SelectedScrapRule == null) return;
        var clone = Clone(SelectedScrapRule);
        var dialog = new Views.ScrapRuleDialogWindow(clone);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
            _ = SaveScrapRuleAsync(dialog.ScrapRule);
    }

    private async Task SaveScrapRuleAsync(ScrapRule rule)
    {
        try
        {
            await _masterDataService.SaveScrapRuleAsync(rule);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
            MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void OnDelete()
    {
        if (SelectedScrapRule == null) return;
        var result = MessageBox.Show(
            $"确认删除报废规则 (路线: {SelectedScrapRule.RouteId}, 工序: {SelectedScrapRule.StepCode})？",
            "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _masterDataService.DeleteScrapRuleAsync(SelectedScrapRule.RuleId);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"删除失败: {ex.Message}";
            MessageBox.Show($"删除失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RaiseCanExecuteChanged()
    {
        EditCommand.RaiseCanExecuteChanged();
        DeleteCommand.RaiseCanExecuteChanged();
    }

    private static ScrapRule Clone(ScrapRule r) => new()
    {
        RuleId = r.RuleId,
        RouteId = r.RouteId,
        StepCode = r.StepCode,
        MaxScrapQty = r.MaxScrapQty,
        MaxScrapPercent = r.MaxScrapPercent,
        ApprovalLevel = r.ApprovalLevel,
    };
}
