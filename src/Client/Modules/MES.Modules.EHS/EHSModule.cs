using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.EHS.Views;
using MES.Modules.EHS.ViewModels;
using MES.Modules.EHS.Services;

namespace MES.Modules.EHS;

public class EHSModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Register Services
        containerRegistry.RegisterSingleton<IEHSService, InMemoryEHSService>();

        // Register Views for Navigation
        containerRegistry.RegisterForNavigation<EnvironmentMonitorView>();
        containerRegistry.RegisterForNavigation<GasMonitorView>();
        containerRegistry.RegisterForNavigation<ChemicalManagementView>();
        containerRegistry.RegisterForNavigation<EsdMonitorView>();
        containerRegistry.RegisterForNavigation<SafetyCheckView>();

        // Register ViewModels
        containerRegistry.Register<EnvironmentMonitorViewModel>();
        containerRegistry.Register<GasMonitorViewModel>();
        containerRegistry.Register<ChemicalManagementViewModel>();
        containerRegistry.Register<EsdMonitorViewModel>();
        containerRegistry.Register<SafetyCheckViewModel>();
    }
}
