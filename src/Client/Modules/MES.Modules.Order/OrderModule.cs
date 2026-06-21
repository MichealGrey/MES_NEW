using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using MES.Modules.Order.Views;
using MES.Modules.Order.ViewModels;
using MES.Modules.Order.Services;

namespace MES.Modules.Order;

public class OrderModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<WorkOrderListView>();
        containerRegistry.RegisterForNavigation<AddWorkOrderWin>();
        containerRegistry.RegisterForNavigation<WorkOrderScheduleView>();
        containerRegistry.RegisterForNavigation<CustomerProgressView>();
        containerRegistry.RegisterForNavigation<WorkOrderCloseView>();
        
        containerRegistry.Register<WorkOrderListViewModel>();
        containerRegistry.Register<WorkOrderScheduleViewModel>();
        containerRegistry.Register<CustomerProgressViewModel>();
        containerRegistry.Register<WorkOrderCloseViewModel>();
        
        containerRegistry.RegisterSingleton<IProductionDataService, ProductionDataService>();
    }
}
