using Prism.Mvvm;
using Prism.Commands;
using Prism.Navigation.Regions;
using MES.Shell.Models;
using MES.Shared.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace MES.Shell.ViewModels;

public class NavigationViewModel : BindableBase, INavigationAware
{
    private readonly IRegionManager _regionManager;
    private readonly ISessionService _session;
    private string _currentModuleTitle = "生产管理";
    private ObservableCollection<NavigationItem> _navigationItems = [];

    // Window 类型的视图名称（不能用 RequestNavigate 导航）
    private static readonly HashSet<string> WindowViews = new(StringComparer.OrdinalIgnoreCase)
    {
        "AddWorkOrderWin"
    };

    public string CurrentModuleTitle { get => _currentModuleTitle; set => SetProperty(ref _currentModuleTitle, value); }
    public ObservableCollection<NavigationItem> NavigationItems { get => _navigationItems; set => SetProperty(ref _navigationItems, value); }

    public DelegateCommand<string> NavigateCommand { get; private set; }

    public NavigationViewModel(IRegionManager regionManager, ISessionService session)
    {
        _regionManager = regionManager;
        _session = session;
        NavigateCommand = new DelegateCommand<string>(NavigateTo);
    }

    public void NavigateTo(string? viewName)
    {
        if (string.IsNullOrEmpty(viewName)) return;

        // Window 类型需要特殊处理（ShowDialog 方式弹出）
        if (WindowViews.Contains(viewName))
        {
            try
            {
                Window? window = viewName switch
                {
                    "AddWorkOrderWin" => new MES.Modules.Order.Views.AddWorkOrderWin(),
                    _ => null
                };
                if (window != null)
                {
                    window.Owner = Application.Current.MainWindow;
                    window.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"打开窗口失败: {ex.Message}", "错误");
            }
            return;
        }

        // 普通 UserControl 使用 Prism 导航
        _regionManager.RequestNavigate("MainContentRegion", viewName);
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        var moduleKey = navigationContext.Parameters.GetValue<string>("ModuleKey");
        if (!string.IsNullOrEmpty(moduleKey)) UpdateNavigationTree(moduleKey);
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }

    private void UpdateNavigationTree(string moduleKey)
    {
        // 构建完整的导航树
        var fullTree = moduleKey switch
        {
            "Production" => BuildProductionTree(),
            "Order" => BuildOrderTree(),
            "Execute" => BuildExecuteTree(),
            "Lot" => BuildLotTree(),
            "MasterData" => BuildMasterDataTree(),
            "Equipment" => BuildEquipmentTree(),
            "Recipe" => BuildRecipeTree(),
            "Quality" => BuildQualityTree(),
            "Warehouse" => BuildWarehouseTree(),
            "Schedule" => BuildScheduleTree(),
            "Alarm" => BuildAlarmTree(),
            "Trace" => BuildTraceTree(),
            "Yield" => BuildYieldTree(),
            "EHS" => BuildEhsTree(),
            "CustomerComplaint" => BuildCustomerComplaintTree(),
            "EngineeringChange" => BuildEngineeringChangeTree(),
            "ReportCenter" => BuildReportCenterTree(),
            "SystemSettings" => BuildSystemSettingsTree(),
            _ => new List<NavigationItem>()
        };

        // 按权限过滤
        var filtered = fullTree
            .Where(item => item.IsLeaf && _session.HasPermission(moduleKey, item.ViewName))
            .ToList();

        CurrentModuleTitle = moduleKey switch
        {
            "Production" => "生产管理",
            "Order" => "工单管理",
            "Execute" => "生产执行",
            "Lot" => "批次管理",
            "MasterData" => "主数据",
            "Equipment" => "设备管理",
            "Recipe" => "配方管理",
            "Quality" => "质量管理",
            "Warehouse" => "仓储管理",
            "Schedule" => "排程调度",
            "Alarm" => "告警中心",
            "Trace" => "追溯管理",
            "Yield" => "良率管理",
            "EHS" => "EHS管理",
            "CustomerComplaint" => "客诉8D",
            "EngineeringChange" => "工程变更",
            "ReportCenter" => "报表中心",
            "SystemSettings" => "系统设置",
            _ => moduleKey
        };

        NavigationItems = new ObservableCollection<NavigationItem>(filtered);

        // 如果过滤后为空，添加一条提示
        if (filtered.Count == 0)
        {
            NavigationItems.Add(new NavigationItem
            {
                Title = "暂无可用功能",
                ViewName = string.Empty,
                Icon = "—"
            });
        }
    }

    private static List<NavigationItem> BuildProductionTree() => [
        // === 生产执行 - 前道 (晶圆/CP) ===
        new() { Title = "工单管理", ViewName = "WorkOrderListView", Icon = "📋" },
        new() { Title = "WIP总览(晶圆/CP)", ViewName = "WipOverviewView", Icon = "📊" },
        new() { Title = "进站/出站(前道)", ViewName = "TrackInView", Icon = "⬅➡" },
        new() { Title = "拆批/合批", ViewName = "LotSplitMergeView", Icon = "✂" },

        // === 生产执行 - 后道 (封装/FT) ===
        new() { Title = "WIP总览(封装/FT)", ViewName = "WipOverviewView", Icon = "📊" },
        new() { Title = "批次管理", ViewName = "LotListView", Icon = "📦" },
        new() { Title = "进站/出站(后道)", ViewName = "TrackInView", Icon = "⬅➡" },
        new() { Title = "批次Hold/Release", ViewName = "LotHoldView", Icon = "🔒" },
        new() { Title = "等级分选(FT)", ViewName = "LotListView", Icon = "🎯" },

        // === 生产监控 ===
        new() { Title = "批次全览", ViewName = "LotListView", Icon = "📦" },
        new() { Title = "主数据管理", ViewName = "MasterDataView", Icon = "🗃" },
        new() { Title = "客户交付跟踪", ViewName = "CustomerProgressView", Icon = "🤝" },
        new() { Title = "生产报表", ViewName = "ProductionReportView", Icon = "📑" },
        new() { Title = "良率报表", ViewName = "YieldReportView", Icon = "📈" },
        new() { Title = "Lot 归档", ViewName = "LotListView", Icon = "📁" },

        // === 系统管理 (仅 Admin) ===
        new() { Title = "系统监控", ViewName = "SystemMonitorView", Icon = "🖥" },
        new() { Title = "系统健康", ViewName = "SystemHealthView", Icon = "💚" },
        new() { Title = "外部系统接口", ViewName = "ExternalSystemView", Icon = "🔌" },
    ];
    private static List<NavigationItem> BuildEquipmentTree() => [
        new() { Title = "设备状态总览", ViewName = "EquipmentOverviewView", Icon = "🏭" },
        new() { Title = "设备详情", ViewName = "EquipmentDetailView", Icon = "🔍" },
        new() { Title = "EAP控制", ViewName = "EapControlView", Icon = "🎮" },
        new() { Title = "预防性维护", ViewName = "PmScheduleView", Icon = "🔧" },
        new() { Title = "设备告警", ViewName = "EquipmentAlarmView", Icon = "⚠" },
        new() { Title = "备件管理", ViewName = "SparePartView", Icon = "🔩" },
        new() { Title = "设备绩效", ViewName = "EquipmentPerformanceView", Icon = "📊" },
        new() { Title = "设备履历", ViewName = "EquipmentHistoryView", Icon = "📜" },
        new() { Title = "工装夹具", ViewName = "FixtureView", Icon = "🔧" },
    ];
    private static List<NavigationItem> BuildRecipeTree() => [
        new() { Title = "配方列表", ViewName = "RecipeListView", Icon = "📄" },
        new() { Title = "配方审批", ViewName = "RecipeApprovalView", Icon = "✅" },
        new() { Title = "配方参数", ViewName = "RecipeParameterView", Icon = "🔢" },
        new() { Title = "设备绑定", ViewName = "RecipeEquipmentView", Icon = "🔗" },
        new() { Title = "配方版本", ViewName = "RecipeVersionView", Icon = "📑" },
        new() { Title = "配方下发", ViewName = "RecipeDispatchView", Icon = "📤" },
    ];
    private static List<NavigationItem> BuildQualityTree() => [
        new() { Title = "SPC控制图", ViewName = "SpcChartView", Icon = "📈" },
        new() { Title = "SPC规则配置", ViewName = "SpcRuleConfigView", Icon = "⚙" },
        new() { Title = "OOC事件", ViewName = "OocEventView", Icon = "🔴" },
        new() { Title = "FDC监控", ViewName = "FdcMonitorView", Icon = "📡" },
        new() { Title = "检验管理", ViewName = "InspectionView", Icon = "🔬" },
        new() { Title = "首件检验", ViewName = "FirstArticleInspectionView", Icon = "🔍" },
        new() { Title = "巡检管理", ViewName = "PatrolInspectionView", Icon = "🔄" },
        new() { Title = "不合格品", ViewName = "NonconformingView", Icon = "❌" },
        new() { Title = "FMEA", ViewName = "FmeaView", Icon = "📋" },
        new() { Title = "质量目标", ViewName = "QualityTargetView", Icon = "🎯" },
        new() { Title = "MSA", ViewName = "MsaView", Icon = "📊" },
    ];
    private static List<NavigationItem> BuildWarehouseTree() => [
        new() { Title = "载具管理", ViewName = "FoupManagementView", Icon = "📦" },
        new() { Title = "物料管理", ViewName = "MaterialListView", Icon = "🧪" },
        new() { Title = "工装管理", ViewName = "ReticleManagementView", Icon = "🔲" },
        new() { Title = "Stocker", ViewName = "StockerView", Icon = "🏗" },
        new() { Title = "入库管理", ViewName = "InboundManageView", Icon = "📥" },
        new() { Title = "出库管理", ViewName = "OutboundManageView", Icon = "📤" },
        new() { Title = "库存管理", ViewName = "InventoryManageView", Icon = "📊" },
        new() { Title = "过期预警", ViewName = "ExpiryAlertView", Icon = "⏰" },
    ];
    private static List<NavigationItem> BuildScheduleTree() => [
        new() { Title = "派工看板", ViewName = "DispatchBoardView", Icon = "📋" },
        new() { Title = "派工管理", ViewName = "DispatchView", Icon = "📤" },
        new() { Title = "派工规则", ViewName = "DispatchRuleConfigView", Icon = "⚖" },
        new() { Title = "产能分析", ViewName = "CapacityAnalysisView", Icon = "📊" },
        new() { Title = "工单排程", ViewName = "WorkOrderScheduleView", Icon = "📅" },
        new() { Title = "物料需求计划", ViewName = "MrpView", Icon = "📦" },
        new() { Title = "交付管理", ViewName = "DeliveryManageView", Icon = "🚚" },
        new() { Title = "产能平衡", ViewName = "CapacityBalanceView", Icon = "⚖" },
    ];
    private static List<NavigationItem> BuildAlarmTree() => [
        new() { Title = "报警看板", ViewName = "AlarmDashboardView", Icon = "🚨" },
        new() { Title = "实时告警", ViewName = "ActiveAlarmView", Icon = "🔴" },
        new() { Title = "告警历史", ViewName = "AlarmHistoryView", Icon = "📜" },
        new() { Title = "告警规则", ViewName = "AlarmRuleConfigView", Icon = "⚙" },
        new() { Title = "告警统计", ViewName = "AlarmStatisticsView", Icon = "📊" },
        new() { Title = "报警规则", ViewName = "AlarmRuleView", Icon = "🔔" },
    ];
    private static List<NavigationItem> BuildTraceTree() => [
        new() { Title = "批次追溯", ViewName = "LotTraceView", Icon = "🔍" },
        new() { Title = "血缘图谱", ViewName = "GenealogyView", Icon = "🌳" },
        new() { Title = "影响分析", ViewName = "ImpactAnalysisView", Icon = "💥" },
        new() { Title = "正向追溯", ViewName = "ForwardTraceView", Icon = "➡" },
        new() { Title = "反向追溯", ViewName = "BackwardTraceView", Icon = "⬅" },
        new() { Title = "客户追溯报告", ViewName = "CustomerTraceReportView", Icon = "📄" },
    ];
    private static List<NavigationItem> BuildYieldTree() => [
        new() { Title = "良率看板", ViewName = "YieldDashboardView", Icon = "🎯" },
        new() { Title = "良率趋势", ViewName = "YieldTrendView", Icon = "📈" },
        new() { Title = "Bin Map", ViewName = "WaferMapView", Icon = "🔵" },
        new() { Title = "良率分析", ViewName = "YieldAnalysisView", Icon = "📊" },
        new() { Title = "缺陷分析", ViewName = "DefectAnalysisView", Icon = "🔴" },
        new() { Title = "Bin分析", ViewName = "BinAnalysisView", Icon = "📋" },
    ];
    private static List<NavigationItem> BuildEhsTree() => [
        new() { Title = "环境监控", ViewName = "EnvironmentMonitorView", Icon = "🌡" },
        new() { Title = "气体监控", ViewName = "GasMonitorView", Icon = "☁" },
        new() { Title = "化学品管理", ViewName = "ChemicalManagementView", Icon = "🧪" },
        new() { Title = "ESD监控", ViewName = "EsdMonitorView", Icon = "⚡" },
        new() { Title = "安全检查", ViewName = "SafetyCheckView", Icon = "🛡" },
    ];

    private static List<NavigationItem> BuildOrderTree() => [
        new() { Title = "工单列表", ViewName = "WorkOrderListView", Icon = "📋" },
        new() { Title = "创建工单", ViewName = "AddWorkOrderWin", Icon = "➕" },
        new() { Title = "工单排程", ViewName = "WorkOrderScheduleView", Icon = "📅" },
        new() { Title = "客户交付跟踪", ViewName = "CustomerProgressView", Icon = "🤝" },
        new() { Title = "工单关闭", ViewName = "WorkOrderCloseView", Icon = "✅" },
    ];

    private static List<NavigationItem> BuildExecuteTree() => [
        new() { Title = "快速进站", ViewName = "TrackInView", Icon = "⚡" },
        new() { Title = "进站/出站", ViewName = "TrackInView", Icon = "⬅➡" },
        new() { Title = "派工管理", ViewName = "DispatchView", Icon = "📤" },
        new() { Title = "生产报工", ViewName = "ProductionReportView", Icon = "📝" },
        new() { Title = "物料确认", ViewName = "MaterialManagementView", Icon = "✅" },
        new() { Title = "等级分选", ViewName = "GradeSortView", Icon = "🎯" },
        new() { Title = "拆批/合批", ViewName = "LotSplitMergeView", Icon = "✂" },
    ];

    private static List<NavigationItem> BuildLotTree() => [
        new() { Title = "批次列表", ViewName = "LotListView", Icon = "📦" },
        new() { Title = "批次详情", ViewName = "LotDetailView", Icon = "🔍" },
        new() { Title = "Hold/Release", ViewName = "LotHoldView", Icon = "🔒" },
        new() { Title = "WIP总览", ViewName = "WipOverviewView", Icon = "📊" },
        new() { Title = "批次归档", ViewName = "LotArchiveView", Icon = "📁" },
        new() { Title = "批次异常", ViewName = "LotExceptionView", Icon = "⚠" },
    ];

    private static List<NavigationItem> BuildMasterDataTree() => [
        new() { Title = "产品管理", ViewName = "ProductManagementView", Icon = "📦" },
        new() { Title = "工艺路线", ViewName = "RouteManagementView", Icon = "🛤" },
        new() { Title = "配方管理", ViewName = "RecipeManagementView", Icon = "📋" },
        new() { Title = "设备管理", ViewName = "EquipmentManagementView", Icon = "⚙" },
        new() { Title = "物料管理", ViewName = "MaterialManagementView", Icon = "🧪" },
        new() { Title = "客户管理", ViewName = "CustomerManagementView", Icon = "👥" },
        new() { Title = "原因代码", ViewName = "ReasonCodeManagementView", Icon = "❓" },
        new() { Title = "缺陷代码", ViewName = "DefectCodeManagementView", Icon = "🔴" },
        new() { Title = "载具管理", ViewName = "CarrierManagementView", Icon = "📦" },
        new() { Title = "良率规则", ViewName = "YieldRuleManagementView", Icon = "📈" },
        new() { Title = "报废规则", ViewName = "ScrapRuleManagementView", Icon = "🗑" },
        new() { Title = "报警规则", ViewName = "AlarmRuleManagementView", Icon = "🚨" },
    ];

    private static List<NavigationItem> BuildCustomerComplaintTree() => [
        new() { Title = "客诉列表", ViewName = "ComplaintListView", Icon = "📋" },
        new() { Title = "客诉详情", ViewName = "ComplaintDetailView", Icon = "🔍" },
        new() { Title = "原因分析", ViewName = "ComplaintAnalysisView", Icon = "🔬" },
        new() { Title = "纠正措施", ViewName = "ComplaintActionView", Icon = "🔧" },
        new() { Title = "8D报告", ViewName = "ComplaintReportView", Icon = "📄" },
        new() { Title = "统计分析", ViewName = "ComplaintStatisticsView", Icon = "📊" },
    ];

    private static List<NavigationItem> BuildEngineeringChangeTree() => [
        new() { Title = "变更列表", ViewName = "ECNListView", Icon = "📋" },
        new() { Title = "变更详情", ViewName = "ECNDetailView", Icon = "🔍" },
        new() { Title = "变更审批", ViewName = "ECNApprovalView", Icon = "✅" },
        new() { Title = "影响分析", ViewName = "ECNImpactView", Icon = "🔬" },
        new() { Title = "变更历史", ViewName = "ECNHistoryView", Icon = "📜" },
    ];

    private static List<NavigationItem> BuildReportCenterTree() => [
        new() { Title = "生产报表", ViewName = "ProductionReportView", Icon = "📊" },
        new() { Title = "良率报表", ViewName = "YieldReportView", Icon = "📈" },
        new() { Title = "质量报表", ViewName = "QualityReportView", Icon = "🔬" },
        new() { Title = "设备报表", ViewName = "EquipmentReportView", Icon = "⚙" },
        new() { Title = "批次追溯报表", ViewName = "LotGenealogyReportView", Icon = "🔍" },
        new() { Title = "综合看板", ViewName = "DashboardView", Icon = "📺" },
    ];

    private static List<NavigationItem> BuildSystemSettingsTree() => [
        new() { Title = "系统监控", ViewName = "SystemMonitorView", Icon = "🖥" },
        new() { Title = "系统健康", ViewName = "SystemHealthView", Icon = "💚" },
        new() { Title = "外部系统接口", ViewName = "ExternalSystemView", Icon = "🔌" },
        new() { Title = "用户权限", ViewName = "UserPermissionView", Icon = "👤" },
        new() { Title = "操作日志", ViewName = "OperationLogView", Icon = "📜" },
        new() { Title = "系统参数", ViewName = "SystemParamView", Icon = "⚙" },
    ];
}