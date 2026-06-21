using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using MES.Modules.Lot.Views;
using MES.Modules.Lot.ViewModels;
using MES.Modules.Lot.Services;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;

namespace MES.Modules.Lot;

public class LotModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<LotListView>();
        containerRegistry.RegisterForNavigation<LotDetailView>();
        containerRegistry.RegisterForNavigation<LotSplitMergeView>();
        containerRegistry.RegisterForNavigation<LotHoldView>();
        containerRegistry.RegisterForNavigation<LotArchiveView>();
        containerRegistry.RegisterForNavigation<LotOverviewView>();
        
        containerRegistry.Register<ILotService, LotService>();
        
        containerRegistry.Register<LotListViewModel>();
        containerRegistry.Register<LotDetailViewModel>();
        containerRegistry.Register<LotSplitMergeViewModel>();
        containerRegistry.Register<LotHoldViewModel>();
        containerRegistry.Register<LotArchiveViewModel>();
        containerRegistry.Register<LotOverviewViewModel>();
    }
}
