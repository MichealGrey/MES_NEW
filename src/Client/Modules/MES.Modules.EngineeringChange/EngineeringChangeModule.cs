using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using MES.Modules.EngineeringChange.Views;
using MES.Modules.EngineeringChange.ViewModels;
using MES.Modules.EngineeringChange.Services;

namespace MES.Modules.EngineeringChange;

public class EngineeringChangeModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<ECNListView>();
        containerRegistry.RegisterForNavigation<ECNDetailView>();
        containerRegistry.RegisterForNavigation<ECNApprovalView>();
        containerRegistry.RegisterForNavigation<ECNImpactView>();
        containerRegistry.RegisterForNavigation<ECNHistoryView>();

        containerRegistry.Register<ECNListViewModel>();
        containerRegistry.Register<ECNDetailViewModel>();
        containerRegistry.Register<ECNApprovalViewModel>();
        containerRegistry.Register<ECNImpactViewModel>();
        containerRegistry.Register<ECNHistoryViewModel>();

        // ECNService (HttpClient injected from Shell level)
        containerRegistry.RegisterSingleton<IECNService, ECNService>();
    }
}