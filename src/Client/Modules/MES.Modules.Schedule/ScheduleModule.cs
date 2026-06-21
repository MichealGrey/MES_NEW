using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Schedule.Views;
using MES.Modules.Schedule.ViewModels;
using MES.Modules.Schedule.Services;

namespace MES.Modules.Schedule;

public class ScheduleModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<DispatchBoardView>();
        containerRegistry.RegisterForNavigation<DispatchRuleConfigView>();
        containerRegistry.RegisterForNavigation<CapacityAnalysisView>();
        containerRegistry.RegisterForNavigation<WorkOrderScheduleView>();
        containerRegistry.RegisterForNavigation<MrpView>();
        containerRegistry.RegisterForNavigation<DeliveryManageView>();
        containerRegistry.RegisterForNavigation<CapacityBalanceView>();

        containerRegistry.Register<IScheduleService, ScheduleService>();

        containerRegistry.Register<DispatchBoardViewModel>();
        containerRegistry.Register<DispatchRuleConfigViewModel>();
        containerRegistry.Register<CapacityAnalysisViewModel>();
        containerRegistry.Register<WorkOrderScheduleViewModel>();
        containerRegistry.Register<MrpViewModel>();
        containerRegistry.Register<DeliveryManageViewModel>();
        containerRegistry.Register<CapacityBalanceViewModel>();
    }
}
