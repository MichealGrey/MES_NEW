using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Quality.Views;
using MES.Modules.Quality.ViewModels;

namespace MES.Modules.Quality;

public class QualityModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<SpcChartView>();
        containerRegistry.RegisterForNavigation<SpcRuleConfigView>();
        containerRegistry.RegisterForNavigation<OocEventView>();
        containerRegistry.RegisterForNavigation<FdcMonitorView>();
        containerRegistry.RegisterForNavigation<InspectionView>();

        // ViewModel 注册
        containerRegistry.Register<SpcChartViewModel>();
        containerRegistry.Register<SpcRuleConfigViewModel>();
        containerRegistry.Register<OocEventViewModel>();
        containerRegistry.Register<FdcMonitorViewModel>();
        containerRegistry.Register<InspectionViewModel>();
    }
}
