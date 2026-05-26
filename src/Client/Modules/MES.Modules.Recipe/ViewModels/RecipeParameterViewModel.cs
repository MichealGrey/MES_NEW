using Prism.Mvvm;
using MES.Modules.Recipe.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Recipe.ViewModels;

public class RecipeParameterViewModel : BindableBase
{
    private ObservableCollection<RecipeParameterItem> _items = [];
    private RecipeParameterItem? _selectedItem;

    public ObservableCollection<RecipeParameterItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public RecipeParameterItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public RecipeParameterViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<RecipeParameterItem>
        {
            new() { ParameterName = "BondPower", Value = "180", Unit = "mW", Min = "150", Max = "210" },
            new() { ParameterName = "BondForce", Value = "35", Unit = "gf", Min = "25", Max = "45" },
            new() { ParameterName = "BondTime", Value = "25", Unit = "ms", Min = "18", Max = "32" },
            new() { ParameterName = "LoopHeight", Value = "230", Unit = "um", Min = "200", Max = "260" },
            new() { ParameterName = "MoldTemp", Value = "175", Unit = "C", Min = "165", Max = "185" },
            new() { ParameterName = "TransferPressure", Value = "850", Unit = "kgf", Min = "750", Max = "950" },
            new() { ParameterName = "CureTime", Value = "120", Unit = "s", Min = "100", Max = "140" },
            new() { ParameterName = "DieAttachTemp", Value = "150", Unit = "C", Min = "140", Max = "160" },
            new() { ParameterName = "PickForce", Value = "60", Unit = "gf", Min = "40", Max = "80" },
            new() { ParameterName = "TestVoltage", Value = "3.3", Unit = "V", Min = "3.0", Max = "3.6" },
        };
    }
}
