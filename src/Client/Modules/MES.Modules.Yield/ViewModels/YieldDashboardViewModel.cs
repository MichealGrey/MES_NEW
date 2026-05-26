using Prism.Mvvm;
using MES.Modules.Yield.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Yield.ViewModels;

public class YieldDashboardViewModel : BindableBase
{
    private ObservableCollection<YieldKpiItem> _items = [];

    public ObservableCollection<YieldKpiItem> Items { get => _items; set => SetProperty(ref _items, value); }

    public YieldDashboardViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<YieldKpiItem>
        {
            new() { KpiName = "CP Yield", Value = 98.5, Unit = "%", Trend = "Up" },
            new() { KpiName = "FT Yield", Value = 96.3, Unit = "%", Trend = "Stable" },
            new() { KpiName = "Strip Yield", Value = 97.6, Unit = "%", Trend = "Up" },
            new() { KpiName = "Daily Output", Value = 4850, Unit = "unit", Trend = "Up" },
            new() { KpiName = "Bin1 Rate", Value = 92.1, Unit = "%", Trend = "Stable" },
            new() { KpiName = "Pkg Defect Rate", Value = 0.35, Unit = "%", Trend = "Down" },
        };
    }
}
