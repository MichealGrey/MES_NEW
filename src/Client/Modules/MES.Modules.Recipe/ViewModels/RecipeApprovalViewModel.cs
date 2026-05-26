using Prism.Mvvm;
using MES.Modules.Recipe.Models;
using System.Collections.ObjectModel;

namespace MES.Modules.Recipe.ViewModels;

public class RecipeApprovalViewModel : BindableBase
{
    private ObservableCollection<RecipeApprovalItem> _items = [];
    private RecipeApprovalItem? _selectedItem;

    public ObservableCollection<RecipeApprovalItem> Items { get => _items; set => SetProperty(ref _items, value); }
    public RecipeApprovalItem? SelectedItem { get => _selectedItem; set => SetProperty(ref _selectedItem, value); }

    public RecipeApprovalViewModel()
    {
        LoadMockData();
    }

    private void LoadMockData()
    {
        Items = new ObservableCollection<RecipeApprovalItem>
        {
            new() { RecipeId = "RCP-001", RecipeName = "DieAttach_Main", Version = "V2.0", Submitter = "Zhang Wei", Approver = "Li Ming", Result = "Pending", Comment = "待审核" },
            new() { RecipeId = "RCP-003", RecipeName = "WireBond_Au", Version = "V1.2", Submitter = "Wang Fang", Approver = "Liu Jun", Result = "Pending", Comment = "线径调整后重新提交" },
            new() { RecipeId = "RCP-005", RecipeName = "Molding_EMC", Version = "V3.1", Submitter = "Chen Lei", Approver = "Zhang Wei", Result = "Approved", Comment = "参数验证通过" },
            new() { RecipeId = "RCP-007", RecipeName = "TrimForm_Lead", Version = "V2.1", Submitter = "Li Ming", Approver = "Wang Fang", Result = "Rejected", Comment = "切筋尺寸超出规格" },
            new() { RecipeId = "RCP-002", RecipeName = "Test_FT_Bin", Version = "V1.3", Submitter = "Liu Jun", Approver = "Chen Lei", Result = "Pending", Comment = "" },
            new() { RecipeId = "RCP-009", RecipeName = "Mark_Laser", Version = "V2.0", Submitter = "Zhang Wei", Approver = "Li Ming", Result = "Approved", Comment = "激光打标参数符合要求" },
        };
    }
}
