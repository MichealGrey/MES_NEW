using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class ProductionReportViewModel : BindableBase
{
    private readonly IReportService _reportService;

    private ProductionReport? _report;
    private ObservableCollection<StepYieldData> _stepYields = [];
    private ObservableCollection<EquipmentUtilization> _equipUtils = [];
    private DateTime _reportDate = DateTime.Today;
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ProductionReport? Report
    {
        get => _report;
        set => SetProperty(ref _report, value);
    }

    public ObservableCollection<StepYieldData> StepYields
    {
        get => _stepYields;
        set => SetProperty(ref _stepYields, value);
    }

    public ObservableCollection<EquipmentUtilization> EquipUtils
    {
        get => _equipUtils;
        set => SetProperty(ref _equipUtils, value);
    }

    public DateTime ReportDate
    {
        get => _reportDate;
        set => SetProperty(ref _reportDate, value);
    }

    public ICommand GenerateCommand { get; }

    public ProductionReportViewModel(IReportService reportService)
    {
        _reportService = reportService;
        GenerateCommand = new DelegateCommand(async () => await GenerateReportAsync());
    }

    private async System.Threading.Tasks.Task GenerateReportAsync()
    {
        Report = await _reportService.GenerateDailyReportAsync(ReportDate);
        if (Report is not null)
        {
            StepYields = new ObservableCollection<StepYieldData>(Report.StepYields);
            EquipUtils = new ObservableCollection<EquipmentUtilization>(Report.EquipmentUtils);
        }
    }
}
