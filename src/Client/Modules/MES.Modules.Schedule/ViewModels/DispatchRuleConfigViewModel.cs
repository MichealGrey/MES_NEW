using Prism.Mvvm;
using MES.Modules.Schedule.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Schedule.ViewModels;

public class DispatchRuleConfigViewModel : BindableBase
{
    private ObservableCollection<DispatchRuleItem> _items = [];
    private DispatchRuleItem? _selectedItem;

    public ObservableCollection<DispatchRuleItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public DispatchRuleItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public DispatchRuleConfigViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<DispatchRuleItem>
        {
            new() { RuleName = "DueDate", Weight = 0.30, IsEnabled = true, Description = "交期优先 - 距离交期越近权重越高" },
            new() { RuleName = "WipBalance", Weight = 0.20, IsEnabled = true, Description = "WIP平衡 - 避免设备空闲或过载" },
            new() { RuleName = "SetupMinimize", Weight = 0.15, IsEnabled = true, Description = "换型最小化 - 减少设备setup次数" },
            new() { RuleName = "LotPriority", Weight = 0.15, IsEnabled = true, Description = "批次优先级 - Hot/Express批次优先" },
            new() { RuleName = "EquipmentHealth", Weight = 0.10, IsEnabled = true, Description = "设备健康度 - 优先分配健康设备" },
            new() { RuleName = "ProcessConstraint", Weight = 0.05, IsEnabled = true, Description = "工艺约束 - 满足工艺等待时间要求" },
            new() { RuleName = "Fifo", Weight = 0.05, IsEnabled = false, Description = "先进先出 - 同优先级按到达顺序" },
        };
    }
}
