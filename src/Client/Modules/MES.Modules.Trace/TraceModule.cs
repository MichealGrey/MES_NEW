using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Trace.Views;
using MES.Modules.Trace.ViewModels;
using MES.Modules.Trace.Services;

namespace MES.Modules.Trace;

public class TraceModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Register views
        containerRegistry.RegisterForNavigation<LotTraceView>();
        containerRegistry.RegisterForNavigation<ForwardTraceView>();
        containerRegistry.RegisterForNavigation<BackwardTraceView>();
        containerRegistry.RegisterForNavigation<GenealogyView>();
        containerRegistry.RegisterForNavigation<ImpactAnalysisView>();
        containerRegistry.RegisterForNavigation<CustomerTraceReportView>();

        // Register TraceService (HttpClient injected from Shell level)
        containerRegistry.RegisterSingleton<ITraceService, TraceService>();

        // Register ViewModels
        containerRegistry.Register<LotTraceViewModel>();
        containerRegistry.Register<ForwardTraceViewModel>();
        containerRegistry.Register<BackwardTraceViewModel>();
        containerRegistry.Register<GenealogyViewModel>();
        containerRegistry.Register<ImpactAnalysisViewModel>();
        containerRegistry.Register<CustomerTraceReportViewModel>();
    }
}
