using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Quality.Views;
using MES.Modules.Quality.ViewModels;
using MES.Modules.Quality.Services;

namespace MES.Modules.Quality;

public class QualityModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Views
        containerRegistry.RegisterForNavigation<SpcChartView>();
        containerRegistry.RegisterForNavigation<SpcRuleConfigView>();
        containerRegistry.RegisterForNavigation<OocEventView>();
        containerRegistry.RegisterForNavigation<FdcMonitorView>();
        containerRegistry.RegisterForNavigation<InspectionView>();
        containerRegistry.RegisterForNavigation<FirstArticleInspectionView>();
        containerRegistry.RegisterForNavigation<PatrolInspectionView>();
        containerRegistry.RegisterForNavigation<NonconformingView>();
        containerRegistry.RegisterForNavigation<FmeaView>();
        containerRegistry.RegisterForNavigation<QualityTargetView>();
        containerRegistry.RegisterForNavigation<MsaView>();

        // Service
        containerRegistry.Register<IQualityService, QualityService>();

        // ViewModels
        containerRegistry.Register<SpcChartViewModel>();
        containerRegistry.Register<SpcRuleConfigViewModel>();
        containerRegistry.Register<OocEventViewModel>();
        containerRegistry.Register<FdcMonitorViewModel>();
        containerRegistry.Register<InspectionViewModel>();
        containerRegistry.Register<FirstArticleInspectionViewModel>();
        containerRegistry.Register<PatrolInspectionViewModel>();
        containerRegistry.Register<NonconformingViewModel>();
        containerRegistry.Register<FmeaViewModel>();
        containerRegistry.Register<QualityTargetViewModel>();
        containerRegistry.Register<MsaViewModel>();
    }
}
