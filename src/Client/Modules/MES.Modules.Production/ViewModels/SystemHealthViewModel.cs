using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class SystemHealthViewModel : BindableBase
{
    private readonly ISystemHealthService _healthService;
    private SystemHealthReport _healthReport = new();
    private int _cleanedCount;

    public SystemHealthReport HealthReport
    {
        get => _healthReport;
        set => SetProperty(ref _healthReport, value);
    }

    public int CleanedCount { get => _cleanedCount; set => SetProperty(ref _cleanedCount, value); }

    public ICommand RefreshCommand { get; }
    public ICommand CleanupCommand { get; }

    public SystemHealthViewModel(ISystemHealthService healthService)
    {
        _healthService = healthService;
        RefreshCommand = new DelegateCommand(async () => await RefreshAsync());
        CleanupCommand = new DelegateCommand(async () => await CleanupAsync());
    }

    private async System.Threading.Tasks.Task RefreshAsync()
    {
        HealthReport = await _healthService.GetHealthReportAsync();
    }

    private async System.Threading.Tasks.Task CleanupAsync()
    {
        var result = System.Windows.MessageBox.Show("确定要清理 90 天前的过期数据吗？", "数据清理", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
        if (result == System.Windows.MessageBoxResult.Yes)
        {
            CleanedCount = await _healthService.CleanupExpiredDataAsync(90);
            System.Windows.MessageBox.Show($"已清理 {CleanedCount} 条过期数据", "清理完成", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            await RefreshAsync();
        }
    }
}
