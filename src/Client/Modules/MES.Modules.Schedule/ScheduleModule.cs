using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Schedule.Views;
using MES.Modules.Schedule.ViewModels;

namespace MES.Modules.Schedule;

public class ScheduleModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<DispatchBoardView>();
        containerRegistry.RegisterForNavigation<DispatchRuleConfigView>();
        containerRegistry.RegisterForNavigation<CapacityAnalysisView>();

        // ViewModel 注册
        containerRegistry.Register<DispatchBoardViewModel>();
        containerRegistry.Register<DispatchRuleConfigViewModel>();
        containerRegistry.Register<CapacityAnalysisViewModel>();
    }
}
