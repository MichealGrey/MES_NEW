using Prism.Mvvm;
using MES.Modules.Recipe.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Recipe.ViewModels;

public class RecipeEquipmentViewModel : BindableBase
{
    private ObservableCollection<RecipeEquipmentItem> _items = [];
    private RecipeEquipmentItem? _selectedItem;

    public ObservableCollection<RecipeEquipmentItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public RecipeEquipmentItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public RecipeEquipmentViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<RecipeEquipmentItem>
        {
            new() { EquipmentType = "DicingSaw", RecipeCount = 6, BindingStatus = "Bound" },
            new() { EquipmentType = "DieBonder", RecipeCount = 10, BindingStatus = "Bound" },
            new() { EquipmentType = "WireBonder", RecipeCount = 18, BindingStatus = "Bound" },
            new() { EquipmentType = "MoldingPress", RecipeCount = 8, BindingStatus = "Bound" },
            new() { EquipmentType = "LaserMark", RecipeCount = 4, BindingStatus = "Partial" },
            new() { EquipmentType = "TrimForm", RecipeCount = 5, BindingStatus = "Bound" },
            new() { EquipmentType = "TestHandler", RecipeCount = 12, BindingStatus = "Partial" },
            new() { EquipmentType = "BurnIn", RecipeCount = 3, BindingStatus = "Unbound" },
        };
    }
}
