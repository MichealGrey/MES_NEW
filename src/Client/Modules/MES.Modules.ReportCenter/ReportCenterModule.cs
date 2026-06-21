using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using MES.Modules.ReportCenter.Views;
using MES.Modules.ReportCenter.ViewModels;
using MES.Modules.ReportCenter.Services;

namespace MES.Modules.ReportCenter;

public class ReportCenterModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Register views
        containerRegistry.RegisterForNavigation<ProductionReportView>();
        containerRegistry.RegisterForNavigation<YieldReportView>();
        containerRegistry.RegisterForNavigation<QualityReportView>();
        containerRegistry.RegisterForNavigation<EquipmentReportView>();
        containerRegistry.RegisterForNavigation<LotGenealogyReportView>();
        containerRegistry.RegisterForNavigation<DashboardView>();

        // Register ViewModels
        containerRegistry.Register<ProductionReportViewModel>();
        containerRegistry.Register<YieldReportViewModel>();
        containerRegistry.Register<QualityReportViewModel>();
        containerRegistry.Register<EquipmentReportViewModel>();
        containerRegistry.Register<LotGenealogyReportViewModel>();
        containerRegistry.Register<DashboardViewModel>();

        // Register ReportService (HttpClient injected from Shell level)
        containerRegistry.RegisterSingleton<IReportService, ReportService>();
    }
}
