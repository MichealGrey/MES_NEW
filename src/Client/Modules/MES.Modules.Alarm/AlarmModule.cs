using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Alarm.Views;
using MES.Modules.Alarm.ViewModels;
using MES.Modules.Alarm.Services;

namespace MES.Modules.Alarm;

public class AlarmModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Register Services
        containerRegistry.RegisterSingleton<IAlarmService, InMemoryAlarmService>();

        // Register Views for Navigation
        containerRegistry.RegisterForNavigation<ActiveAlarmView>();
        containerRegistry.RegisterForNavigation<AlarmHistoryView>();
        containerRegistry.RegisterForNavigation<AlarmRuleConfigView>();
        containerRegistry.RegisterForNavigation<AlarmStatisticsView>();
        containerRegistry.RegisterForNavigation<AlarmRuleView>();

        // Register ViewModels
        containerRegistry.Register<ActiveAlarmViewModel>();
        containerRegistry.Register<AlarmHistoryViewModel>();
        containerRegistry.Register<AlarmRuleConfigViewModel>();
        containerRegistry.Register<AlarmStatisticsViewModel>();
        containerRegistry.Register<AlarmRuleViewModel>();
    }
}
