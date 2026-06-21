using System.Collections.ObjectModel;
using System.Windows;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class CustomerManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<Models.CustomerInfo> _customers = [];
    private Models.CustomerInfo? _selectedCustomer;
    private string? _errorMessage;
    private string _searchKeyword = string.Empty;
    private bool _isLoading;

    public ObservableCollection<Models.CustomerInfo> Customers
    {
        get => _customers;
        set => SetProperty(ref _customers, value);
    }

    public Models.CustomerInfo? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            SetProperty(ref _selectedCustomer, value);
            RaiseCanExecuteChanged();
        }
    }

    public string SearchKeyword
    {
        get => _searchKeyword;
        set
        {
            SetProperty(ref _searchKeyword, value);
            FilterCustomers();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AddCommand { get; }
    public DelegateCommand EditCommand { get; }
    public DelegateCommand ToggleStatusCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public CustomerManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedCustomer != null);
        ToggleStatusCommand = new DelegateCommand(OnToggleStatus, () => SelectedCustomer != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        if (_isLoading) return;
        _isLoading = true;
        try
        {
            ErrorMessage = null;
            var customers = await _masterDataService.GetAllCustomersAsync();
            Customers = new ObservableCollection<Models.CustomerInfo>(customers);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void FilterCustomers()
    {
        if (string.IsNullOrWhiteSpace(SearchKeyword))
        {
            // Don't reload from DB while already loading
            if (!_isLoading) _ = LoadDataAsync();
            return;
        }
        var keyword = SearchKeyword.ToLower();
        var filtered = Customers.Where(c =>
            c.CustomerName.ToLower().Contains(keyword) ||
            c.CustomerCode.ToLower().Contains(keyword) ||
            (c.ContactPerson?.ToLower().Contains(keyword) ?? false)).ToList();
    }

    private void OnAdd()
    {
        var newCustomer = new Models.CustomerInfo { CustomerId = $"CUST-{DateTime.Now:yyyyMMddHHmmss}" };
        var dialog = new Views.CustomerDialogWindow(newCustomer);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
        {
            _ = SaveCustomerAsync(dialog.Customer);
        }
    }

    private void OnEdit()
    {
        if (SelectedCustomer == null) return;
        var clone = Clone(SelectedCustomer);
        var dialog = new Views.CustomerDialogWindow(clone);
        dialog.Owner = Application.Current.MainWindow;
        if (dialog.ShowDialog() == true)
        {
            _ = SaveCustomerAsync(dialog.Customer);
        }
    }

    private async Task SaveCustomerAsync(Models.CustomerInfo customer)
    {
        try
        {
            await _masterDataService.SaveCustomerAsync(customer);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"保存失败: {ex.Message}";
            MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void OnToggleStatus()
    {
        if (SelectedCustomer == null) return;
        var newStatus = SelectedCustomer.Status == "Active" ? "Inactive" : "Active";
        try
        {
            await _masterDataService.UpdateCustomerStatusAsync(SelectedCustomer.CustomerId, newStatus);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"状态更新失败: {ex.Message}";
            MessageBox.Show($"状态更新失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RaiseCanExecuteChanged()
    {
        EditCommand.RaiseCanExecuteChanged();
        ToggleStatusCommand.RaiseCanExecuteChanged();
    }

    private static Models.CustomerInfo Clone(Models.CustomerInfo c) => new()
    {
        CustomerId = c.CustomerId,
        CustomerName = c.CustomerName,
        CustomerCode = c.CustomerCode,
        ContactPerson = c.ContactPerson,
        ContactPhone = c.ContactPhone,
        Email = c.Email,
        Address = c.Address,
        CustomerPnPrefix = c.CustomerPnPrefix,
        QualityLevel = c.QualityLevel,
        SpecialRequirements = c.SpecialRequirements,
        DefaultPackingSpec = c.DefaultPackingSpec,
        DefaultOqcSpec = c.DefaultOqcSpec,
        Status = c.Status,
    };
}
