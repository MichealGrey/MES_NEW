using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class CustomerProgressViewModel : BindableBase
{
    private readonly ICustomerProductionService _customerService;
    private ObservableCollection<CustomerLotProgress> _lotProgressList = new();
    private ObservableCollection<DueDateRisk> _dueDateRisks = new();
    private ObservableCollection<CustomerInfo> _customers = new();
    private CustomerInfo? _selectedCustomer;
    private string _searchOrderId = string.Empty;

    public ObservableCollection<CustomerLotProgress> LotProgressList
    {
        get => _lotProgressList;
        set => SetProperty(ref _lotProgressList, value);
    }

    public ObservableCollection<DueDateRisk> DueDateRisks
    {
        get => _dueDateRisks;
        set => SetProperty(ref _dueDateRisks, value);
    }

    public ObservableCollection<CustomerInfo> Customers
    {
        get => _customers;
        set => SetProperty(ref _customers, value);
    }

    public CustomerInfo? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            if (SetProperty(ref _selectedCustomer, value))
            {
                _ = LoadCustomerData(); // Fire-and-forget is acceptable here
            }
        }
    }

    public string SearchOrderId
    {
        get => _searchOrderId;
        set => SetProperty(ref _searchOrderId, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand GenerateReportCommand { get; }

    public CustomerProgressViewModel(ICustomerProductionService customerService)
    {
        _customerService = customerService;
        RefreshCommand = new DelegateCommand(async () => await RefreshAsync());
        SearchCommand = new DelegateCommand(async () => await SearchAsync());
        GenerateReportCommand = new DelegateCommand<string>(async (lotId) => await GenerateReportAsync(lotId));
    }

    private async System.Threading.Tasks.Task RefreshAsync()
    {
        var risks = await _customerService.GetAllDueDateRisksAsync();
        DueDateRisks = new ObservableCollection<DueDateRisk>(risks);

        var allProgress = new List<CustomerLotProgress>();
        var customerIds = new HashSet<string>();
        var customerNames = new Dictionary<string, string>();

        // Only get progress for orders that have risks (limited set)
        foreach (var risk in risks)
        {
            var progress = await _customerService.GetOrderProgressAsync(risk.OrderId);
            allProgress.AddRange(progress);
            if (!string.IsNullOrEmpty(risk.CustomerId))
            {
                customerIds.Add(risk.CustomerId);
                customerNames[risk.CustomerId] = risk.CustomerName;
            }
        }

        LotProgressList = new ObservableCollection<CustomerLotProgress>(allProgress);

        Customers = new ObservableCollection<CustomerInfo>(
            customerIds.Select(id => new CustomerInfo { CustomerId = id, CustomerName = customerNames.GetValueOrDefault(id, id) }));
    }

    private async System.Threading.Tasks.Task LoadCustomerData()
    {
        if (SelectedCustomer is null) return;

        var progress = await _customerService.GetCustomerProgressAsync(SelectedCustomer.CustomerId);
        LotProgressList = new ObservableCollection<CustomerLotProgress>(progress);
    }

    private async System.Threading.Tasks.Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchOrderId))
        {
            await RefreshAsync();
            return;
        }

        var progress = await _customerService.GetOrderProgressAsync(SearchOrderId.Trim());
        LotProgressList = new ObservableCollection<CustomerLotProgress>(progress);
    }

    private async System.Threading.Tasks.Task GenerateReportAsync(string? lotId)
    {
        if (string.IsNullOrEmpty(lotId)) return;
        var report = await _customerService.GenerateTraceReportAsync(lotId, "Operator");
        System.Windows.MessageBox.Show($"追溯报告已生成: {report.ReportId}\n批次: {report.LotId}\n总良率: {report.Summary.OverallYield:F1}%", "报告生成", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }
}

public class CustomerInfo
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
}
