using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class MasterDataViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;

    private ObservableCollection<EquipmentInfo> _equipments = [];
    private ObservableCollection<CarrierInfo> _carriers = [];
    private ObservableCollection<RecipeInfo> _recipes = [];
    private ObservableCollection<UserInfo> _users = [];
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ObservableCollection<EquipmentInfo> Equipments
    {
        get => _equipments;
        set => SetProperty(ref _equipments, value);
    }

    public ObservableCollection<CarrierInfo> Carriers
    {
        get => _carriers;
        set => SetProperty(ref _carriers, value);
    }

    public ObservableCollection<RecipeInfo> Recipes
    {
        get => _recipes;
        set => SetProperty(ref _recipes, value);
    }

    public ObservableCollection<UserInfo> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }

    public ICommand LoadDataCommand { get; }

    public MasterDataViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        LoadDataCommand = new DelegateCommand(async () => await LoadAllDataAsync());
        LoadDataCommand.Execute(masterDataService);
    }

    private async System.Threading.Tasks.Task LoadAllDataAsync()
    {
        Equipments = new ObservableCollection<EquipmentInfo>(await _masterDataService.GetAllEquipmentsAsync());
        Carriers = new ObservableCollection<CarrierInfo>(await _masterDataService.GetAllCarriersAsync());
        Recipes = new ObservableCollection<RecipeInfo>(await _masterDataService.GetAllRecipesAsync());
        Users = new ObservableCollection<UserInfo>(await _masterDataService.GetAllUsersAsync());
    }
}
