using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class YieldReportViewModel : BindableBase
{
    private readonly IReportService _reportService;

    private YieldReport? _report;
    private string _routeId = "QFN-STD";
    private DateTime _startDate = DateTime.Today.AddDays(-7);
    private DateTime _endDate = DateTime.Today;
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public YieldReport? Report
    {
        get => _report;
        set => SetProperty(ref _report, value);
    }

    public string RouteId
    {
        get => _routeId;
        set => SetProperty(ref _routeId, value);
    }

    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    public ICommand GenerateCommand { get; }

    public YieldReportViewModel(IReportService reportService)
    {
        _reportService = reportService;
        GenerateCommand = new DelegateCommand(async () => await GenerateReportAsync());
    }

    private async System.Threading.Tasks.Task GenerateReportAsync()
    {
        Report = await _reportService.GenerateYieldReportAsync(RouteId, StartDate, EndDate);
    }
}
