using Prism.Mvvm;
using MES.Modules.Alarm.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Alarm.ViewModels;

public class AlarmStatisticsViewModel : BindableBase
{
    private ObservableCollection<AlarmStatisticsItem> _items = [];
    private AlarmStatisticsItem? _selectedItem;

    public ObservableCollection<AlarmStatisticsItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public AlarmStatisticsItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public AlarmStatisticsViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        var baseDate = DateTime.Today;
        Items = new ObservableCollection<AlarmStatisticsItem>
        {
            new() { Date = baseDate.AddDays(-6), CriticalCount = 2, MajorCount = 5, MinorCount = 12, TotalCount = 19 },
            new() { Date = baseDate.AddDays(-5), CriticalCount = 1, MajorCount = 3, MinorCount = 8, TotalCount = 12 },
            new() { Date = baseDate.AddDays(-4), CriticalCount = 3, MajorCount = 7, MinorCount = 15, TotalCount = 25 },
            new() { Date = baseDate.AddDays(-3), CriticalCount = 0, MajorCount = 2, MinorCount = 6, TotalCount = 8 },
            new() { Date = baseDate.AddDays(-2), CriticalCount = 1, MajorCount = 4, MinorCount = 10, TotalCount = 15 },
            new() { Date = baseDate.AddDays(-1), CriticalCount = 2, MajorCount = 6, MinorCount = 11, TotalCount = 19 },
            new() { Date = baseDate, CriticalCount = 1, MajorCount = 2, MinorCount = 5, TotalCount = 8 },
        };
    }
}
