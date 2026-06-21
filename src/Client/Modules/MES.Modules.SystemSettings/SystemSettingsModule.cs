using DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using MES.Modules.SystemSettings.Views;
using MES.Modules.SystemSettings.ViewModels;
using MES.Modules.SystemSettings.Services;

namespace MES.Modules.SystemSettings;

public class SystemSettingsModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<UserPermissionView>();
        containerRegistry.RegisterForNavigation<OperationLogView>();
        containerRegistry.RegisterForNavigation<SystemParamView>();
        containerRegistry.RegisterForNavigation<SystemMonitorView>();
        containerRegistry.RegisterForNavigation<SystemHealthView>();
        containerRegistry.RegisterForNavigation<ExternalSystemView>();

        // Register SettingsService (HttpClient injected from Shell level)
        containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();

        // Register ViewModels
        containerRegistry.Register<UserPermissionViewModel>();
        containerRegistry.Register<OperationLogViewModel>();
        containerRegistry.Register<SystemParamViewModel>();
        containerRegistry.Register<SystemMonitorViewModel>();
        containerRegistry.Register<SystemHealthViewModel>();
        containerRegistry.Register<ExternalSystemViewModel>();
    }
}
