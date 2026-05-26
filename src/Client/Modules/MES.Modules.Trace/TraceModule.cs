using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Trace.Views;
using MES.Modules.Trace.ViewModels;

namespace MES.Modules.Trace;

public class TraceModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<LotTraceView>();
        containerRegistry.RegisterForNavigation<GenealogyView>();
        containerRegistry.RegisterForNavigation<ImpactAnalysisView>();

        // ViewModel 注册
        containerRegistry.Register<LotTraceViewModel>();
        containerRegistry.Register<GenealogyViewModel>();
        containerRegistry.Register<ImpactAnalysisViewModel>();
    }
}
