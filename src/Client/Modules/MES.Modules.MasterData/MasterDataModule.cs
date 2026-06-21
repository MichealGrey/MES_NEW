using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using MES.Modules.MasterData.Views;
using MES.Modules.MasterData.ViewModels;
using MES.Modules.MasterData.Services;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;

namespace MES.Modules.MasterData;

public class MasterDataModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<ProductManagementView>();
        containerRegistry.RegisterForNavigation<RouteManagementView>();
        containerRegistry.RegisterForNavigation<RecipeManagementView>();
        containerRegistry.RegisterForNavigation<EquipmentManagementView>();
        containerRegistry.RegisterForNavigation<MaterialManagementView>();
        containerRegistry.RegisterForNavigation<CustomerManagementView>();
        containerRegistry.RegisterForNavigation<ReasonCodeManagementView>();
        containerRegistry.RegisterForNavigation<DefectCodeManagementView>();
        containerRegistry.RegisterForNavigation<CarrierManagementView>();
        containerRegistry.RegisterForNavigation<YieldRuleManagementView>();
        containerRegistry.RegisterForNavigation<ScrapRuleManagementView>();
        containerRegistry.RegisterForNavigation<AlarmRuleManagementView>();
        
        containerRegistry.Register<IMasterDataService, MasterDataService>();
        
        containerRegistry.Register<ProductManagementViewModel>();
        containerRegistry.Register<RouteManagementViewModel>();
        containerRegistry.Register<RecipeManagementViewModel>();
        containerRegistry.Register<EquipmentManagementViewModel>();
        containerRegistry.Register<MaterialManagementViewModel>();
        containerRegistry.Register<CustomerManagementViewModel>();
        containerRegistry.Register<ReasonCodeManagementViewModel>();
        containerRegistry.Register<DefectCodeManagementViewModel>();
        containerRegistry.Register<CarrierManagementViewModel>();
        containerRegistry.Register<YieldRuleManagementViewModel>();
        containerRegistry.Register<ScrapRuleManagementViewModel>();
        containerRegistry.Register<AlarmRuleManagementViewModel>();
    }
}
