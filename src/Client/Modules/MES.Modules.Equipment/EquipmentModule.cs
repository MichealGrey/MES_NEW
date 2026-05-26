using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Equipment.Views;
using MES.Modules.Equipment.ViewModels;

namespace MES.Modules.Equipment;

public class EquipmentModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<EquipmentOverviewView>();
        containerRegistry.RegisterForNavigation<EquipmentDetailView>();
        containerRegistry.RegisterForNavigation<EapControlView>();
        containerRegistry.RegisterForNavigation<PmScheduleView>();
        containerRegistry.RegisterForNavigation<EquipmentAlarmView>();

        // ViewModel 注册
        containerRegistry.Register<EquipmentOverviewViewModel>();
        containerRegistry.Register<EquipmentDetailViewModel>();
        containerRegistry.Register<EapControlViewModel>();
        containerRegistry.Register<PmScheduleViewModel>();
        containerRegistry.Register<EquipmentAlarmViewModel>();
    }
}
