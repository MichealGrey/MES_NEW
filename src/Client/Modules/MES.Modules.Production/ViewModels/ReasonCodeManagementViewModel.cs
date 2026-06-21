using System.Collections.ObjectModel;
using System.Windows;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class ReasonCodeManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<ReasonCodeInfo> _reasonCodes = [];
    private ReasonCodeInfo? _selectedReasonCode;
    private string? _errorMessage;
    private string _selectedCategory = string.Empty;
    private string _selectedApplicableTo = string.Empty;

    public ObservableCollection<ReasonCodeInfo> ReasonCodes
    {
        get => _reasonCodes;
        set => SetProperty(ref _reasonCodes, value);
    }

    public ReasonCodeInfo? SelectedReasonCode
    {
        get => _selectedReasonCode;
        set
        {
            SetProperty(ref _selectedReasonCode, value);
            RaiseCanExecuteChanged();
        }
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            SetProperty(ref _selectedCategory, value);
            FilterReasonCodes();
        }
    }

    public string SelectedApplicableTo
    {
        get => _selectedApplicableTo;
        set
        {
            SetProperty(ref _selectedApplicableTo, value);
            FilterReasonCodes();
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

    public ReasonCodeManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedReasonCode != null);
        ToggleEnabledCommand = new DelegateCommand(OnToggleEnabled, () => SelectedReasonCode != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            ErrorMessage = null;
            var codes = await _masterDataService.GetAllReasonCodesAsync();
            ReasonCodes = new ObservableCollection<ReasonCodeInfo>(codes);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private void FilterReasonCodes()
    {
        var filtered = ReasonCodes.Where(c =>
            (string.IsNullOrEmpty(SelectedCategory) || c.Category == SelectedCategory) &&
            (string.IsNullOrEmpty(SelectedApplicableTo) || c.ApplicableTo == SelectedApplicableTo)).ToList();
    }

    private void OnAdd()
    {
        var newCode = new ReasonCodeInfo
        {
            ReasonCodeId = $"RSN-{DateTime.Now:yyyyMMddHHmmss}",
            Category = "Material",
            ApplicableTo = "Hold"
        };
        var dialog = new Views.ReasonCodeDialogWindow(newCode);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
        {
            _ = SaveReasonCodeAsync(dialog.ReasonCode);
        }
    }

    private void OnEdit()
    {
        if (SelectedReasonCode == null) return;
        var clone = Clone(SelectedReasonCode);
        var dialog = new Views.ReasonCodeDialogWindow(clone);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
        {
            _ = SaveReasonCodeAsync(dialog.ReasonCode);
        }
    }

    private async Task SaveReasonCodeAsync(ReasonCodeInfo code)
    {
        try
        {
            await _masterDataService.SaveReasonCodeAsync(code);
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
        if (SelectedReasonCode == null) return;
        SelectedReasonCode.IsEnabled = !SelectedReasonCode.IsEnabled;
        await SaveReasonCodeAsync(SelectedReasonCode);
    }

    private void RaiseCanExecuteChanged()
    {
        EditCommand.RaiseCanExecuteChanged();
        ToggleEnabledCommand.RaiseCanExecuteChanged();
    }

    private static ReasonCodeInfo Clone(ReasonCodeInfo c) => new()
    {
        ReasonCodeId = c.ReasonCodeId,
        Category = c.Category,
        SubCategory = c.SubCategory,
        ReasonText = c.ReasonText,
        ApplicableTo = c.ApplicableTo,
        IsEnabled = c.IsEnabled,
    };
}
