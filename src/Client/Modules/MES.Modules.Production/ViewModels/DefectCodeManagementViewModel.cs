using System.Collections.ObjectModel;
using System.Windows;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class DefectCodeManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<DefectCodeInfo> _defectCodes = [];
    private DefectCodeInfo? _selectedDefectCode;
    private string? _errorMessage;
    private string _selectedCategory = string.Empty;

    public ObservableCollection<DefectCodeInfo> DefectCodes
    {
        get => _defectCodes;
        set => SetProperty(ref _defectCodes, value);
    }

    public DefectCodeInfo? SelectedDefectCode
    {
        get => _selectedDefectCode;
        set
        {
            SetProperty(ref _selectedDefectCode, value);
            RaiseCanExecuteChanged();
        }
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            SetProperty(ref _selectedCategory, value);
            FilterDefectCodes();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AddCommand { get; }
    public DelegateCommand EditCommand { get; }
    public DelegateCommand ToggleEnabledCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public DefectCodeManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedDefectCode != null);
        ToggleEnabledCommand = new DelegateCommand(OnToggleEnabled, () => SelectedDefectCode != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            ErrorMessage = null;
            var codes = await _masterDataService.GetAllDefectCodesAsync();
            DefectCodes = new ObservableCollection<DefectCodeInfo>(codes);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private void FilterDefectCodes()
    {
        var filtered = DefectCodes.Where(c =>
            string.IsNullOrEmpty(SelectedCategory) || c.DefectCategory == SelectedCategory).ToList();
    }

    private void OnAdd()
    {
        var newCode = new DefectCodeInfo
        {
            DefectCodeId = $"DEF-{DateTime.Now:yyyyMMddHHmmss}",
            DefectCategory = "Cosmetic",
            Severity = "Major"
        };
        var dialog = new Views.DefectCodeDialogWindow(newCode);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
        {
            _ = SaveDefectCodeAsync(dialog.DefectCode);
        }
    }

    private void OnEdit()
    {
        if (SelectedDefectCode == null) return;
        var clone = Clone(SelectedDefectCode);
        var dialog = new Views.DefectCodeDialogWindow(clone);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
        {
            _ = SaveDefectCodeAsync(dialog.DefectCode);
        }
    }

    private async Task SaveDefectCodeAsync(DefectCodeInfo code)
    {
        try
        {
            await _masterDataService.SaveDefectCodeAsync(code);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
            MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void OnToggleEnabled()
    {
        if (SelectedDefectCode == null) return;
        SelectedDefectCode.IsEnabled = !SelectedDefectCode.IsEnabled;
        await SaveDefectCodeAsync(SelectedDefectCode);
    }

    private void RaiseCanExecuteChanged()
    {
        EditCommand.RaiseCanExecuteChanged();
        ToggleEnabledCommand.RaiseCanExecuteChanged();
    }

    private static DefectCodeInfo Clone(DefectCodeInfo c) => new()
    {
        DefectCodeId = c.DefectCodeId,
        DefectCategory = c.DefectCategory,
        DefectText = c.DefectText,
        Severity = c.Severity,
        IsEnabled = c.IsEnabled,
    };
}
