using Prism.Ioc;
using Prism.Modularity;
using MES.Modules.Production.Services;
using MES.Modules.Production.Views;
using MES.Modules.Production.ViewModels;

namespace MES.Modules.Production;

public class ProductionModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<IProductionDataService, ProductionDataService>();

        // V1 Services
        containerRegistry.Register<IRouteService, RouteService>();
        containerRegistry.Register<IQuantityService, QuantityService>();
        containerRegistry.Register<IAuditService, AuditService>();
        containerRegistry.Register<IOperationHistoryService, OperationHistoryService>();
        containerRegistry.Register<ITrackService, TrackService>();

        // V2 新增服务
        containerRegistry.Register<ISignatureService, SignatureService>();
        containerRegistry.Register<IYieldService, YieldService>();
        containerRegistry.Register<IGenealogyService, GenealogyService>();

        // V2 Phase 3 新增服务
        containerRegistry.Register<ILotSplitMergeService, LotSplitMergeService>();
        containerRegistry.Register<ICarrierService, CarrierService>();
        containerRegistry.Register<IReworkService, ReworkService>();
        containerRegistry.Register<IScrapService, ScrapService>();

        // V2 Phase 4 新增服务
        containerRegistry.Register<IMasterDataService, MasterDataService>();
        containerRegistry.Register<IDispatchService, DispatchService>();
        containerRegistry.Register<IReportService, ReportService>();

        // V3 Phase 1 新增服务
        containerRegistry.Register<IEquipmentGateway, EquipmentGateway>();
        containerRegistry.Register<IRecipeGateway, RecipeGateway>();
        containerRegistry.Register<IQualityGateway, QualityGateway>();
        containerRegistry.Register<IWarehouseGateway, WarehouseGateway>();
        containerRegistry.Register<IAlarmService, AlarmService>();

        // V3 Phase 2 新增服务
        containerRegistry.Register<ICustomerProductionService, CustomerProductionService>();

        // V3 Phase 3 新增服务
        containerRegistry.Register<ISystemHealthService, SystemHealthService>();

        // V3 Phase 4 新增服务
        containerRegistry.Register<IExternalSystemService, ExternalSystemService>();

        containerRegistry.RegisterForNavigation<WorkOrderListView>();
        containerRegistry.RegisterForNavigation<LotListView>();
        containerRegistry.RegisterForNavigation<WipOverviewView>();
        containerRegistry.RegisterForNavigation<TrackInView>();
        containerRegistry.RegisterForNavigation<LotHoldView>();
        containerRegistry.RegisterForNavigation<GenealogyView>();
        containerRegistry.RegisterForNavigation<LotSplitMergeView>();

        // V2 Phase 4 新增视图
        containerRegistry.RegisterForNavigation<MasterDataView>();
        containerRegistry.RegisterForNavigation<DispatchView>();
        containerRegistry.RegisterForNavigation<ProductionReportView>();
        containerRegistry.RegisterForNavigation<YieldReportView>();
        containerRegistry.RegisterForNavigation<SystemMonitorView>();

        // V3 Phase 5 新增视图
        containerRegistry.RegisterForNavigation<AlarmDashboardView>();
        containerRegistry.RegisterForNavigation<CustomerProgressView>();
        containerRegistry.RegisterForNavigation<SystemHealthView>();
        containerRegistry.RegisterForNavigation<ExternalSystemView>();

        // 注册 ViewModel（供 AutoWireViewModel 使用）
        containerRegistry.Register<TrackInViewModel>();
        containerRegistry.Register<LotHoldViewModel>();
        containerRegistry.Register<GenealogyViewModel>();
        containerRegistry.Register<LotSplitMergeViewModel>();
        containerRegistry.Register<WorkOrderListViewModel>();
        containerRegistry.Register<LotListViewModel>();
        containerRegistry.Register<WipOverviewViewModel>();
        containerRegistry.Register<MasterDataViewModel>();
        containerRegistry.Register<DispatchViewModel>();
        containerRegistry.Register<ProductionReportViewModel>();
        containerRegistry.Register<YieldReportViewModel>();
        containerRegistry.Register<SystemMonitorViewModel>();
        containerRegistry.Register<AlarmDashboardViewModel>();
        containerRegistry.Register<CustomerProgressViewModel>();
        containerRegistry.Register<SystemHealthViewModel>();
        containerRegistry.Register<ExternalSystemViewModel>();
    }
}
