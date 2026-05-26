using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Shell.Models;
using MES.Shared.Services;
using System.Collections.ObjectModel;

namespace MES.Shell.ViewModels;

public class NavigationViewModel : BindableBase, INavigationAware
{
    private readonly IRegionManager _regionManager;
    private readonly ISessionService _session;
    private string _currentModuleTitle = "生产管理";
    private ObservableCollection<NavigationItem> _navigationItems = [];

    public string CurrentModuleTitle { get => _currentModuleTitle; set => SetProperty(ref _currentModuleTitle, value); }
    public ObservableCollection<NavigationItem> NavigationItems { get => _navigationItems; set => SetProperty(ref _navigationItems, value); }

    public NavigationViewModel(IRegionManager regionManager, ISessionService session)
    {
        _regionManager = regionManager;
        _session = session;
    }

    public void NavigateTo(string viewName)
    {
        if (string.IsNullOrEmpty(viewName)) return;
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
            "Equipment" => BuildEquipmentTree(),
            "Recipe" => BuildRecipeTree(),
            "Quality" => BuildQualityTree(),
            "Warehouse" => BuildWarehouseTree(),
            "Schedule" => BuildScheduleTree(),
            "Alarm" => BuildAlarmTree(),
            "Trace" => BuildTraceTree(),
            "Yield" => BuildYieldTree(),
            "EHS" => BuildEhsTree(),
            _ => new List<NavigationItem>()
        };

        // 按权限过滤
        var filtered = fullTree
            .Where(item => item.IsLeaf && _session.HasPermission(moduleKey, item.ViewName))
            .ToList();

        CurrentModuleTitle = moduleKey switch
        {
            "Production" => "生产管理",
            "Equipment" => "设备管理",
            "Recipe" => "配方管理",
            "Quality" => "质量管理",
            "Warehouse" => "仓储管理",
            "Schedule" => "排程调度",
            "Alarm" => "告警中心",
            "Trace" => "追溯管理",
            "Yield" => "良率管理",
            "EHS" => "EHS管理",
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
    ];
    private static List<NavigationItem> BuildRecipeTree() => [
        new() { Title = "配方列表", ViewName = "RecipeListView", Icon = "📄" },
        new() { Title = "配方审批", ViewName = "RecipeApprovalView", Icon = "✅" },
        new() { Title = "配方参数", ViewName = "RecipeParameterView", Icon = "🔢" },
        new() { Title = "设备绑定", ViewName = "RecipeEquipmentView", Icon = "🔗" },
    ];
    private static List<NavigationItem> BuildQualityTree() => [
        new() { Title = "SPC控制图", ViewName = "SpcChartView", Icon = "📈" },
        new() { Title = "SPC规则配置", ViewName = "SpcRuleConfigView", Icon = "⚙" },
        new() { Title = "OOC事件", ViewName = "OocEventView", Icon = "🔴" },
        new() { Title = "FDC监控", ViewName = "FdcMonitorView", Icon = "📡" },
        new() { Title = "检验管理", ViewName = "InspectionView", Icon = "🔬" },
    ];
    private static List<NavigationItem> BuildWarehouseTree() => [
        new() { Title = "载具管理", ViewName = "FoupManagementView", Icon = "📦" },
        new() { Title = "物料管理", ViewName = "MaterialListView", Icon = "🧪" },
        new() { Title = "工装管理", ViewName = "ReticleManagementView", Icon = "🔲" },
        new() { Title = "Stocker", ViewName = "StockerView", Icon = "🏗" },
    ];
    private static List<NavigationItem> BuildScheduleTree() => [
        new() { Title = "派工看板", ViewName = "DispatchBoardView", Icon = "📋" },
        new() { Title = "派工管理", ViewName = "DispatchView", Icon = "📤" },
        new() { Title = "派工规则", ViewName = "DispatchRuleConfigView", Icon = "⚖" },
        new() { Title = "产能分析", ViewName = "CapacityAnalysisView", Icon = "📊" },
    ];
    private static List<NavigationItem> BuildAlarmTree() => [
        new() { Title = "报警看板", ViewName = "AlarmDashboardView", Icon = "🚨" },
        new() { Title = "实时告警", ViewName = "ActiveAlarmView", Icon = "�" },
        new() { Title = "告警历史", ViewName = "AlarmHistoryView", Icon = "📜" },
        new() { Title = "告警规则", ViewName = "AlarmRuleConfigView", Icon = "⚙" },
        new() { Title = "告警统计", ViewName = "AlarmStatisticsView", Icon = "📊" },
    ];
    private static List<NavigationItem> BuildTraceTree() => [
        new() { Title = "批次追溯", ViewName = "LotTraceView", Icon = "🔍" },
        new() { Title = "血缘图谱", ViewName = "GenealogyView", Icon = "🌳" },
        new() { Title = "影响分析", ViewName = "ImpactAnalysisView", Icon = "💥" },
    ];
    private static List<NavigationItem> BuildYieldTree() => [
        new() { Title = "良率看板", ViewName = "YieldDashboardView", Icon = "🎯" },
        new() { Title = "良率趋势", ViewName = "YieldTrendView", Icon = "📈" },
        new() { Title = "Bin Map", ViewName = "WaferMapView", Icon = "🔵" },
    ];
    private static List<NavigationItem> BuildEhsTree() => [
        new() { Title = "环境监控", ViewName = "EnvironmentMonitorView", Icon = "🌡" },
        new() { Title = "气体监控", ViewName = "GasMonitorView", Icon = "☁" },
        new() { Title = "化学品管理", ViewName = "ChemicalManagementView", Icon = "🧪" },
    ];
}