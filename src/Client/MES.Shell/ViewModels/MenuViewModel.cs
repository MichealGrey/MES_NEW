using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Shell.Models;
using MES.Shell.Services;
using System.Collections.ObjectModel;
using MES.Shared.Services;

using ISessionService = MES.Shell.Services.ISessionService;

namespace MES.Shell.ViewModels;

public class MenuViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;
    private readonly ISessionService _session;
    public ObservableCollection<MenuItemInfo> MenuItems { get; }

    public DelegateCommand<string> SelectModuleCommand { get; }

    public MenuViewModel(IRegionManager regionManager, ISessionService session)
    {
        _regionManager = regionManager;
        _session = session;

        SelectModuleCommand = new DelegateCommand<string>(OnSelectModule);
        MenuItems = [];

        BuildMenu();
    }

    /// <summary>登录后重建菜单（由 App.xaml.cs 调用）</summary>
    public void BuildMenu()
    {
        MenuItems.Clear();

        var allMenus = new (string key, string displayName)[]
        {
            ("Order",       "工单管理"),
            ("Execute",     "生产执行"),
            ("Lot",         "批次管理"),
            ("MasterData",  "主数据"),
            ("Equipment",   "设备管理"),
            ("Recipe",      "配方管理"),
            ("Quality",     "质量管理"),
            ("Warehouse",   "仓储管理"),
            ("Schedule",    "排程调度"),
            ("Alarm",       "告警中心"),
            ("Trace",       "追溯管理"),
            ("Yield",       "良率管理"),
            ("EHS",         "EHS管理"),
            ("CustomerComplaint", "客诉8D"),
            ("EngineeringChange", "工程变更"),
            ("ReportCenter", "报表中心"),
            ("SystemSettings", "系统设置"),
        };

        bool isFirst = true;
        foreach (var (key, name) in allMenus)
        {
            if (_session.HasModuleAccess(key))
            {
                MenuItems.Add(new MenuItemInfo
                {
                    ModuleKey = key,
                    DisplayName = name,
                    IsSelected = isFirst
                });
                isFirst = false;
            }
        }

        // 如果一个模块都看不到 → 提供一个退出的入口
        if (MenuItems.Count == 0)
        {
            MenuItems.Add(new MenuItemInfo
            {
                ModuleKey = string.Empty,
                DisplayName = "无可用模块",
                IsSelected = true
            });
        }
    }

    private void OnSelectModule(string moduleKey)
    {
        if (string.IsNullOrEmpty(moduleKey)) return;

        // 再次检查权限（安全防护）
        if (!_session.HasModuleAccess(moduleKey))
            return;

        var parameters = new NavigationParameters { { "ModuleKey", moduleKey } };
        _regionManager.RequestNavigate("NavigationRegion", "NavigationView", parameters);

        var defaultView = GetDefaultView(moduleKey);

        // 检查具体视图权限
        if (!string.IsNullOrEmpty(defaultView) && _session.HasPermission(moduleKey, defaultView))
            _regionManager.RequestNavigate("MainContentRegion", defaultView);
        else
        {
            // 尝试查找该模块第一个有权限的视图
            var fallback = GetFallbackView(moduleKey);
            if (!string.IsNullOrEmpty(fallback))
                _regionManager.RequestNavigate("MainContentRegion", fallback);
        }
    }

    private string? GetFallbackView(string moduleKey) => moduleKey switch
    {
        "Production" => FindFirstPermitted("Production", "WorkOrderListView", "LotListView", "TrackInView", "WipOverviewView", "LotHoldView", "GenealogyView", "LotSplitMergeView", "MasterDataView", "DispatchView", "ProductionReportView", "YieldReportView", "SystemMonitorView"),
        "Order" => FindFirstPermitted("Order", "WorkOrderListView", "AddWorkOrderWin", "WorkOrderScheduleView", "CustomerProgressView"),
        "Execute" => FindFirstPermitted("Execute", "TrackInView", "DispatchView", "ProductionReportView", "MaterialManagementView", "GradeSortView", "LotSplitMergeView"),
        "Lot" => FindFirstPermitted("Lot", "LotListView", "LotDetailView", "LotHoldView", "WipOverviewView"),
        "MasterData" => FindFirstPermitted("MasterData", "ProductManagementView", "RouteManagementView", "RecipeManagementView", "EquipmentManagementView"),
        "Equipment" => FindFirstPermitted("Equipment", "EquipmentOverviewView", "EquipmentDetailView", "EapControlView"),
        "Recipe" => FindFirstPermitted("Recipe", "RecipeListView", "RecipeApprovalView", "RecipeParameterView"),
        "Quality" => FindFirstPermitted("Quality", "SpcChartView", "SpcRuleConfigView", "OocEventView", "FdcMonitorView", "InspectionView"),
        "Warehouse" => FindFirstPermitted("Warehouse", "FoupManagementView", "MaterialListView", "StockerView"),
        "Schedule" => FindFirstPermitted("Schedule", "DispatchBoardView", "DispatchRuleConfigView", "CapacityAnalysisView"),
        "Alarm" => FindFirstPermitted("Alarm", "ActiveAlarmView", "AlarmHistoryView", "AlarmRuleConfigView"),
        "Trace" => FindFirstPermitted("Trace", "LotTraceView", "GenealogyView", "ImpactAnalysisView"),
        "Yield" => FindFirstPermitted("Yield", "YieldDashboardView", "YieldTrendView", "WaferMapView"),
        "EHS" => FindFirstPermitted("EHS", "EnvironmentMonitorView", "GasMonitorView", "ChemicalManagementView"),
        "CustomerComplaint" => FindFirstPermitted("CustomerComplaint", "ComplaintListView", "ComplaintDetailView", "ComplaintAnalysisView"),
        "EngineeringChange" => FindFirstPermitted("EngineeringChange", "ECNListView", "ECNDetailView", "ECNApprovalView"),
        "ReportCenter" => FindFirstPermitted("ReportCenter", "ProductionReportView", "YieldReportView", "QualityReportView"),
        "SystemSettings" => FindFirstPermitted("SystemSettings", "SystemMonitorView", "SystemHealthView", "ExternalSystemView"),
        _ => null
    };

    private string? FindFirstPermitted(string module, params string[] views)
    {
        foreach (var v in views)
            if (_session.HasPermission(module, v)) return v;
        return null;
    }

    private static string GetDefaultView(string moduleKey) => moduleKey switch
    {
        "Production" => "WorkOrderListView",
        "Order" => "WorkOrderListView",
        "Execute" => "TrackInView",
        "Lot" => "LotListView",
        "MasterData" => "ProductManagementView",
        "Equipment"  => "EquipmentOverviewView",
        "Recipe"     => "RecipeListView",
        "Quality"    => "SpcChartView",
        "Warehouse"  => "FoupManagementView",
        "Schedule"   => "DispatchBoardView",
        "Alarm"      => "ActiveAlarmView",
        "Trace"      => "LotTraceView",
        "Yield"      => "YieldDashboardView",
        "EHS"        => "EnvironmentMonitorView",
        "CustomerComplaint" => "ComplaintListView",
        "EngineeringChange" => "ECNListView",
        "ReportCenter" => "ProductionReportView",
        "SystemSettings" => "SystemMonitorView",
        _ => string.Empty
    };
}