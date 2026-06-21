using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Yield.Views;
using MES.Modules.Yield.ViewModels;
using MES.Modules.Yield.Services;

namespace MES.Modules.Yield;

public class YieldModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Register views
        containerRegistry.RegisterForNavigation<YieldDashboardView>();
        containerRegistry.RegisterForNavigation<YieldTrendView>();
        containerRegistry.RegisterForNavigation<WaferMapView>();
        containerRegistry.RegisterForNavigation<YieldAnalysisView>();
        containerRegistry.RegisterForNavigation<DefectAnalysisView>();
        containerRegistry.RegisterForNavigation<BinAnalysisView>();

        // Register YieldService (HttpClient injected from Shell level)
        containerRegistry.RegisterSingleton<IYieldService, YieldService>();

        // Register ViewModels
        containerRegistry.Register<YieldDashboardViewModel>();
        containerRegistry.Register<YieldTrendViewModel>();
        containerRegistry.Register<WaferMapViewModel>();
        containerRegistry.Register<YieldAnalysisViewModel>();
        containerRegistry.Register<DefectAnalysisViewModel>();
        containerRegistry.Register<BinAnalysisViewModel>();
    }
}
