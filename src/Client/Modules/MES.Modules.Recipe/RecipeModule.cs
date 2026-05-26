using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Recipe.Views;
using MES.Modules.Recipe.ViewModels;

namespace MES.Modules.Recipe;

public class RecipeModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<RecipeListView>();
        containerRegistry.RegisterForNavigation<RecipeApprovalView>();
        containerRegistry.RegisterForNavigation<RecipeParameterView>();
        containerRegistry.RegisterForNavigation<RecipeEquipmentView>();

        // ViewModel 注册
        containerRegistry.Register<RecipeListViewModel>();
        containerRegistry.Register<RecipeApprovalViewModel>();
        containerRegistry.Register<RecipeParameterViewModel>();
        containerRegistry.Register<RecipeEquipmentViewModel>();
    }
}
