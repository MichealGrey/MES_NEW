using Prism.Mvvm;
using MES.Modules.Recipe.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MES.Modules.Recipe.ViewModels;

public class RecipeListViewModel : BindableBase
{
    private ObservableCollection<RecipeListModel> _items = [];
    private RecipeListModel? _selectedItem;

    public ObservableCollection<RecipeListModel> Items { get => _items; set => SetProperty(ref _items, value); }
    public RecipeListModel? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public RecipeListViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        var statuses = new[] { "Draft", "Submitted", "Approved", "Active" };
        var eqTypes = new[] { "DicingSaw", "DieBonder", "WireBonder", "MoldingPress", "LaserMark", "TrimForm", "TestHandler", "BurnIn", "AOI", "PlasmaClean" };
        var names = new[] { "Dicing_Main", "DieAttach_Main", "WireBond_Gold", "Molding_Transfer",
            "LaserMark_Body", "TrimForm_Singulation", "Test_CP", "BurnIn_168H", "AOI_PostMold", "PlasmaClean_PreBond" };
        var creators = new[] { "Zhang Wei", "Li Ming", "Wang Fang", "Liu Jun", "Chen Lei" };

        Items = new ObservableCollection<RecipeListModel>(
            Enumerable.Range(1, 10).Select(i => new RecipeListModel
            {
                RecipeId = $"RCP-{i:D3}",
                Name = names[i - 1],
                Version = $"V{1 + i % 3}.{i % 5}",
                Status = statuses[i % 4],
                EquipmentType = eqTypes[i % 10],
                CreatedBy = creators[i % 5]
            }));
    }
}
