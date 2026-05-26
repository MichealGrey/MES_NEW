using Prism.Mvvm;
using MES.Modules.Quality.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Quality.ViewModels;

public class SpcRuleConfigViewModel : BindableBase
{
    private ObservableCollection<SpcRuleItem> _items = [];
    private SpcRuleItem? _selectedItem;

    public ObservableCollection<SpcRuleItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public SpcRuleItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public SpcRuleConfigViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<SpcRuleItem>
        {
            new() { RuleNumber = 1, RuleName = "Rule 1", Description = "1个点超出3 Sigma", IsEnabled = true },
            new() { RuleNumber = 2, RuleName = "Rule 2", Description = "连续9个点在中心线同侧", IsEnabled = true },
            new() { RuleNumber = 3, RuleName = "Rule 3", Description = "连续6个点递增或递减", IsEnabled = true },
            new() { RuleNumber = 4, RuleName = "Rule 4", Description = "连续14个点交替上下", IsEnabled = false },
            new() { RuleNumber = 5, RuleName = "Rule 5", Description = "3个点中有2个超出2 Sigma", IsEnabled = true },
            new() { RuleNumber = 6, RuleName = "Rule 6", Description = "5个点中有4个超出1 Sigma", IsEnabled = true },
            new() { RuleNumber = 7, RuleName = "Rule 7", Description = "连续15个点在1 Sigma内", IsEnabled = false },
            new() { RuleNumber = 8, RuleName = "Rule 8", Description = "连续8个点超出1 Sigma且在中心线两侧", IsEnabled = true },
        };
    }
}
