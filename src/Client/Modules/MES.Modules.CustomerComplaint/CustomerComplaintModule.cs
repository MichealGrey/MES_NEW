using Prism.Ioc;
using Prism.Modularity;
using Prism.Navigation.Regions;
using MES.Modules.CustomerComplaint.Views;
using MES.Modules.CustomerComplaint.ViewModels;
using MES.Modules.CustomerComplaint.Services;

namespace MES.Modules.CustomerComplaint;

public class CustomerComplaintModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider)
    {
        var regionManager = containerProvider.Resolve<IRegionManager>();
    }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<ComplaintListView>();
        containerRegistry.RegisterForNavigation<ComplaintDetailView>();
        containerRegistry.RegisterForNavigation<ComplaintAnalysisView>();
        containerRegistry.RegisterForNavigation<ComplaintActionView>();
        containerRegistry.RegisterForNavigation<ComplaintReportView>();
        containerRegistry.RegisterForNavigation<ComplaintStatisticsView>();

        containerRegistry.Register<ComplaintListViewModel>();
        containerRegistry.Register<ComplaintDetailViewModel>();
        containerRegistry.Register<ComplaintAnalysisViewModel>();
        containerRegistry.Register<ComplaintActionViewModel>();
        containerRegistry.Register<ComplaintReportViewModel>();
        containerRegistry.Register<ComplaintStatisticsViewModel>();

        containerRegistry.RegisterSingleton<IComplaintService, ComplaintService>();
    }
}