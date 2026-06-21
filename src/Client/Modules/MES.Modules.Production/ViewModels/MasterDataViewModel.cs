using System.Windows.Input;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class MasterDataViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private object? _currentView;
    private string _selectedNav = "Product";

    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public string SelectedNav
    {
        get => _selectedNav;
        set => SetProperty(ref _selectedNav, value);
    }

    public ICommand NavigateCommand { get; }

    public MasterDataViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        NavigateCommand = new DelegateCommand<string>(OnNavigate);
        OnNavigate("Product");
    }

    private void OnNavigate(string? viewName)
    {
        if (string.IsNullOrEmpty(viewName)) viewName = "Product";
        SelectedNav = viewName;

        CurrentView = viewName switch
        {
            "Product" => new Views.ProductManagementView(),
            "Route" => new Views.RouteManagementView(),
            "Equipment" => new Views.EquipmentManagementView(),
            "Carrier" => new Views.CarrierManagementView(),
            "Recipe" => new Views.RecipeManagementView(),
            "Material" => new Views.MaterialManagementView(),
            "Customer" => new Views.CustomerManagementView(),
            "ReasonCode" => new Views.ReasonCodeManagementView(),
            "DefectCode" => new Views.DefectCodeManagementView(),
            "YieldRule" => new Views.YieldRuleManagementView(),
            "AlarmRule" => new Views.AlarmRuleManagementView(),
            "ScrapRule" => new Views.ScrapRuleManagementView(),
            _ => new Views.ProductManagementView()
        };
    }
}
