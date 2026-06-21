using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using MES.Modules.Execute.Views;
using MES.Modules.Execute.ViewModels;
using MES.Modules.Execute.Services;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;

namespace MES.Modules.Execute;

public class ExecuteModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<TrackInView>();
        containerRegistry.RegisterForNavigation<DispatchView>();
        containerRegistry.RegisterForNavigation<WipOverviewView>();
        containerRegistry.RegisterForNavigation<AlarmDashboardView>();
        
        containerRegistry.Register<IExecuteService, ExecuteService>();
        
        containerRegistry.Register<TrackInViewModel>();
        containerRegistry.Register<DispatchViewModel>();
        containerRegistry.Register<WipOverviewViewModel>();
        containerRegistry.Register<AlarmDashboardViewModel>();
    }
}
