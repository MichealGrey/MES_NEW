using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Equipment.Views;
using MES.Modules.Equipment.ViewModels;
using MES.Modules.Equipment.Services;

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
        containerRegistry.RegisterForNavigation<SparePartView>();
        containerRegistry.RegisterForNavigation<EquipmentPerformanceView>();
        containerRegistry.RegisterForNavigation<EquipmentHistoryView>();
        containerRegistry.RegisterForNavigation<FixtureView>();

        containerRegistry.Register<IEquipmentService, EquipmentService>();

        containerRegistry.Register<EquipmentOverviewViewModel>();
        containerRegistry.Register<EquipmentDetailViewModel>();
        containerRegistry.Register<EapControlViewModel>();
        containerRegistry.Register<PmScheduleViewModel>();
        containerRegistry.Register<EquipmentAlarmViewModel>();
        containerRegistry.Register<SparePartViewModel>();
        containerRegistry.Register<EquipmentPerformanceViewModel>();
        containerRegistry.Register<EquipmentHistoryViewModel>();
        containerRegistry.Register<FixtureViewModel>();
    }
}
