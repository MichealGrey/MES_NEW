using Prism.Mvvm;
using MES.Modules.Yield.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Yield.ViewModels;

public class YieldTrendViewModel : BindableBase
{
    private ObservableCollection<YieldTrendItem> _items = [];
    private YieldTrendItem? _selectedItem;

    public ObservableCollection<YieldTrendItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public YieldTrendItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public YieldTrendViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        var baseDate = DateTime.Today.AddDays(-6);
        Items = new ObservableCollection<YieldTrendItem>
        {
            new() { Date = baseDate, LineYield = 97.2, DieYield = 95.5, TestYield = 98.1, DailyOutput = 4500 },
            new() { Date = baseDate.AddDays(1), LineYield = 97.5, DieYield = 95.8, TestYield = 98.3, DailyOutput = 4650 },
            new() { Date = baseDate.AddDays(2), LineYield = 97.3, DieYield = 96.0, TestYield = 98.0, DailyOutput = 4520 },
            new() { Date = baseDate.AddDays(3), LineYield = 97.0, DieYield = 95.6, TestYield = 98.2, DailyOutput = 4680 },
            new() { Date = baseDate.AddDays(4), LineYield = 97.8, DieYield = 96.2, TestYield = 98.5, DailyOutput = 4750 },
            new() { Date = baseDate.AddDays(5), LineYield = 98.0, DieYield = 96.5, TestYield = 98.6, DailyOutput = 4800 },
            new() { Date = baseDate.AddDays(6), LineYield = 97.6, DieYield = 96.3, TestYield = 98.8, DailyOutput = 4850 },
        };
    }
}
