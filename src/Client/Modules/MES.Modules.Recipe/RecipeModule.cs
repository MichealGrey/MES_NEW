using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Recipe.Views;
using MES.Modules.Recipe.ViewModels;
using MES.Modules.Recipe.Services;

namespace MES.Modules.Recipe;

public class RecipeModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Register Services
        containerRegistry.RegisterSingleton<IRecipeService, InMemoryRecipeService>();

        // Register Views for Navigation
        containerRegistry.RegisterForNavigation<RecipeListView>();
        containerRegistry.RegisterForNavigation<RecipeParameterView>();
        containerRegistry.RegisterForNavigation<RecipeEquipmentView>();
        containerRegistry.RegisterForNavigation<RecipeApprovalView>();
        containerRegistry.RegisterForNavigation<RecipeDispatchView>();
        containerRegistry.RegisterForNavigation<RecipeVersionView>();

        // Register ViewModels
        containerRegistry.Register<RecipeListViewModel>();
        containerRegistry.Register<RecipeParameterViewModel>();
        containerRegistry.Register<RecipeEquipmentViewModel>();
        containerRegistry.Register<RecipeApprovalViewModel>();
        containerRegistry.Register<RecipeDispatchViewModel>();
        containerRegistry.Register<RecipeVersionViewModel>();
    }
}
