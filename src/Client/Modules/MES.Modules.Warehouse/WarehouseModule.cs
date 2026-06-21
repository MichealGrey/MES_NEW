using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Warehouse.Views;
using MES.Modules.Warehouse.ViewModels;
using MES.Modules.Warehouse.Services;

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
        containerRegistry.RegisterForNavigation<InboundManageView>();
        containerRegistry.RegisterForNavigation<OutboundManageView>();
        containerRegistry.RegisterForNavigation<InventoryManageView>();
        containerRegistry.RegisterForNavigation<ExpiryAlertView>();

        containerRegistry.Register<IWarehouseService, WarehouseService>();

        containerRegistry.Register<FoupManagementViewModel>();
        containerRegistry.Register<MaterialListViewModel>();
        containerRegistry.Register<ReticleManagementViewModel>();
        containerRegistry.Register<StockerViewModel>();
        containerRegistry.Register<InboundManageViewModel>();
        containerRegistry.Register<OutboundManageViewModel>();
        containerRegistry.Register<InventoryManageViewModel>();
        containerRegistry.Register<ExpiryAlertViewModel>();
    }
}
