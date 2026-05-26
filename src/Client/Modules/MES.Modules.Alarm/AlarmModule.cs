using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Alarm.Views;
using MES.Modules.Alarm.ViewModels;

namespace MES.Modules.Alarm;

public class AlarmModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<ActiveAlarmView>();
        containerRegistry.RegisterForNavigation<AlarmHistoryView>();
        containerRegistry.RegisterForNavigation<AlarmRuleConfigView>();
        containerRegistry.RegisterForNavigation<AlarmStatisticsView>();

        // ViewModel 注册
        containerRegistry.Register<ActiveAlarmViewModel>();
        containerRegistry.Register<AlarmHistoryViewModel>();
        containerRegistry.Register<AlarmRuleConfigViewModel>();
        containerRegistry.Register<AlarmStatisticsViewModel>();
    }
}
