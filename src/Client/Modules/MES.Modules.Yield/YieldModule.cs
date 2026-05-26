using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Yield.Views;
using MES.Modules.Yield.ViewModels;

namespace MES.Modules.Yield;

public class YieldModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<YieldDashboardView>();
        containerRegistry.RegisterForNavigation<YieldTrendView>();
        containerRegistry.RegisterForNavigation<WaferMapView>();

        // ViewModel 注册
        containerRegistry.Register<YieldDashboardViewModel>();
        containerRegistry.Register<YieldTrendViewModel>();
        containerRegistry.Register<WaferMapViewModel>();
    }
}
