using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.EHS.Views;
using MES.Modules.EHS.ViewModels;

namespace MES.Modules.EHS;

public class EHSModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<EnvironmentMonitorView>();
        containerRegistry.RegisterForNavigation<GasMonitorView>();
        containerRegistry.RegisterForNavigation<ChemicalManagementView>();

        // ViewModel 注册
        containerRegistry.Register<EnvironmentMonitorViewModel>();
        containerRegistry.Register<GasMonitorViewModel>();
        containerRegistry.Register<ChemicalManagementViewModel>();
    }
}
