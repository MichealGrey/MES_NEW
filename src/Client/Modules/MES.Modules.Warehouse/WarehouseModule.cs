using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Warehouse.Views;
using MES.Modules.Warehouse.ViewModels;

namespace MES.Modules.Warehouse;

public class WarehouseModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<FoupManagementView>();
        containerRegistry.RegisterForNavigation<MaterialListView>();
        containerRegistry.RegisterForNavigation<ReticleManagementView>();
        containerRegistry.RegisterForNavigation<StockerView>();

        // ViewModel 注册
        containerRegistry.Register<FoupManagementViewModel>();
        containerRegistry.Register<MaterialListViewModel>();
        containerRegistry.Register<ReticleManagementViewModel>();
        containerRegistry.Register<StockerViewModel>();
    }
}
