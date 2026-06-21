using System.Collections.ObjectModel;
using System.Windows;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class RecipeManagementViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;
    private ObservableCollection<RecipeInfo> _recipes = [];
    private RecipeInfo? _selectedRecipe;
    private string? _errorMessage;

    public ObservableCollection<RecipeInfo> Recipes
    {
        get => _recipes;
        set => SetProperty(ref _recipes, value);
    }

    public RecipeInfo? SelectedRecipe
    {
        get => _selectedRecipe;
        set
        {
            SetProperty(ref _selectedRecipe, value);
            RaiseCanExecuteChanged();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public DelegateCommand AddCommand { get; }
    public DelegateCommand EditCommand { get; }
    public DelegateCommand ToggleStatusCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public RecipeManagementViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        AddCommand = new DelegateCommand(OnAdd);
        EditCommand = new DelegateCommand(OnEdit, () => SelectedRecipe != null);
        ToggleStatusCommand = new DelegateCommand(OnToggleStatus, () => SelectedRecipe != null);
        RefreshCommand = new DelegateCommand(async () => await LoadDataAsync());
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            ErrorMessage = null;
            var recipes = await _masterDataService.GetAllRecipesAsync();
            Recipes = new ObservableCollection<RecipeInfo>(recipes);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private void OnAdd()
    {
        MessageBox.Show("配方新增对话框尚未实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OnEdit()
    {
        if (SelectedRecipe == null) return;
        var clone = Clone(SelectedRecipe);
        MessageBox.Show("配方编辑对话框尚未实现", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async void OnToggleStatus()
    {
        if (SelectedRecipe == null) return;

        var newStatus = !SelectedRecipe.IsActive;
        var result = MessageBox.Show(
            $"确认将配方 {SelectedRecipe.RecipeName} ({SelectedRecipe.RecipeId}) 的状态从 '{(SelectedRecipe.IsActive ? "激活" : "停用")}' 切换为 '{(newStatus ? "激活" : "停用")}'?",
            "确认切换", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            var updated = Clone(SelectedRecipe);
            updated.IsActive = newStatus;
            await _masterDataService.SaveRecipeAsync(updated);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"状态切换失败: {ex.Message}";
            MessageBox.Show($"状态切换失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RaiseCanExecuteChanged()
    {
        EditCommand.RaiseCanExecuteChanged();
        ToggleStatusCommand.RaiseCanExecuteChanged();
    }

    private static RecipeInfo Clone(RecipeInfo r) => new()
    {
        RecipeId = r.RecipeId,
        RecipeName = r.RecipeName,
        EquipmentGroup = r.EquipmentGroup,
        ProductId = r.ProductId,
        StepCode = r.StepCode,
        Version = r.Version,
        IsActive = r.IsActive,
        Parameters = r.Parameters,
        ApprovedBy = r.ApprovedBy,
        ApprovedAt = r.ApprovedAt,
        CreatedAt = r.CreatedAt,
    };
}
