using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Domain.Production;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MES.Modules.Production.ViewModels;

/// <summary>
/// 批次列表 ViewModel V2 版本
/// 支持：
/// - 5层层级筛选 (Wafer / Mother / Sub / Grade / MfgID)
/// - 树形展开/折叠
/// - 层级统计
/// - 更多操作
/// </summary>
public class LotListViewModelV2 : BindableBase
{
    private readonly IProductionDataService _dataService;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<LotInfo> _lots = [];
    private ObservableCollection<LotInfo> _filteredLots = [];
    private LotInfo? _selectedLot;
    private string _searchText = string.Empty;
    private string _filterStatus = "全部状态";
    private string _filterStage = "全部阶段";
    
    // 5层层级显示控制
    private bool _showAllLevels = true;
    private bool _showWaferLots = false;
    private bool _showMotherLots = false;
    private bool _showSubLots = false;
    private bool _showGradeLots = false;
    private bool _showMfgIds = false;
    
    private string? _errorMessage;

    public LotListViewModelV2(IProductionDataService dataService, IRegionManager regionManager)
    {
        _dataService = dataService;
        _regionManager = regionManager;

        // 初始化命令
        RefreshCommand = new DelegateCommand(async () => await ReloadDataAsync());
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        ViewDetailCommand = new DelegateCommand<LotInfo?>(OnViewDetail, lot => lot != null);
        HoldLotCommand = new DelegateCommand<LotInfo?>(OnHoldLot, lot => lot != null && !lot.IsHold);
        ReleaseLotCommand = new DelegateCommand<LotInfo?>(OnReleaseLot, lot => lot != null && lot.IsHold);
        SplitLotCommand = new DelegateCommand<LotInfo?>(OnSplitLot, lot => lot != null);
        MergeLotCommand = new DelegateCommand<LotInfo?>(OnMergeLot, lot => lot != null);
        TraceLotCommand = new DelegateCommand<LotInfo?>(OnTraceLot, lot => lot != null);
        ExpandAllCommand = new DelegateCommand(OnExpandAll);
        CollapseAllCommand = new DelegateCommand(OnCollapseAll);
        ToggleExpandCommand = new DelegateCommand<LotInfo?>(OnToggleExpand, lot => lot != null);

        _ = InitializeAsync();
    }

    #region 公开属性

    public ObservableCollection<LotInfo> Lots
    {
        get => _lots;
        set => SetProperty(ref _lots, value);
    }

    public ObservableCollection<LotInfo> FilteredLots
    {
        get => _filteredLots;
        set => SetProperty(ref _filteredLots, value);
    }

    public LotInfo? SelectedLot
    {
        get => _selectedLot;
        set => SetProperty(ref _selectedLot, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                ApplyFilter();
        }
    }

    public string FilterStatus
    {
        get => _filterStatus;
        set
        {
            if (SetProperty(ref _filterStatus, value))
                ApplyFilter();
        }
    }

    public string FilterStage
    {
        get => _filterStage;
        set
        {
            if (SetProperty(ref _filterStage, value))
                ApplyFilter();
        }
    }

    // 层级显示控制
    public bool ShowAllLevels
    {
        get => _showAllLevels;
        set
        {
            if (SetProperty(ref _showAllLevels, value))
            {
                if (value)
                {
                    ShowWaferLots = false;
                    ShowMotherLots = false;
                    ShowSubLots = false;
                    ShowGradeLots = false;
                    ShowMfgIds = false;
                }
                ApplyFilter();
            }
        }
    }

    public bool ShowWaferLots
    {
        get => _showWaferLots;
        set
        {
            if (SetProperty(ref _showWaferLots, value))
            {
                if (value)
                {
                    ShowAllLevels = false;
                    ShowMotherLots = false;
                    ShowSubLots = false;
                    ShowGradeLots = false;
                    ShowMfgIds = false;
                }
                ApplyFilter();
            }
        }
    }

    public bool ShowMotherLots
    {
        get => _showMotherLots;
        set
        {
            if (SetProperty(ref _showMotherLots, value))
            {
                if (value)
                {
                    ShowAllLevels = false;
                    ShowWaferLots = false;
                    ShowSubLots = false;
                    ShowGradeLots = false;
                    ShowMfgIds = false;
                }
                ApplyFilter();
            }
        }
    }

    public bool ShowSubLots
    {
        get => _showSubLots;
        set
        {
            if (SetProperty(ref _showSubLots, value))
            {
                if (value)
                {
                    ShowAllLevels = false;
                    ShowWaferLots = false;
                    ShowMotherLots = false;
                    ShowGradeLots = false;
                    ShowMfgIds = false;
                }
                ApplyFilter();
            }
        }
    }

    public bool ShowGradeLots
    {
        get => _showGradeLots;
        set
        {
            if (SetProperty(ref _showGradeLots, value))
            {
                if (value)
                {
                    ShowAllLevels = false;
                    ShowWaferLots = false;
                    ShowMotherLots = false;
                    ShowSubLots = false;
                    ShowMfgIds = false;
                }
                ApplyFilter();
            }
        }
    }

    public bool ShowMfgIds
    {
        get => _showMfgIds;
        set
        {
            if (SetProperty(ref _showMfgIds, value))
            {
                if (value)
                {
                    ShowAllLevels = false;
                    ShowWaferLots = false;
                    ShowMotherLots = false;
                    ShowSubLots = false;
                    ShowGradeLots = false;
                }
                ApplyFilter();
            }
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    #endregion

    #region 5层层级统计

    public int WaferLotCount => Lots.Count(l => l.LotLevel == LotLevel.WaferLot);
    public int MotherLotCount => Lots.Count(l => l.LotLevel == LotLevel.MotherLot);
    public int SubLotCount => Lots.Count(l => l.LotLevel == LotLevel.SubLot);
    public int GradeLotCount => Lots.Count(l => l.LotLevel == LotLevel.GradeLot);
    public int MfgIdCount => Lots.Count(l => l.LotLevel == LotLevel.MfgId);
    public int HoldCount => Lots.Count(l => l.Status == "Hold");

    #endregion

    #region 命令

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand<LotInfo?> ViewDetailCommand { get; }
    public DelegateCommand<LotInfo?> HoldLotCommand { get; }
    public DelegateCommand<LotInfo?> ReleaseLotCommand { get; }
    public DelegateCommand<LotInfo?> SplitLotCommand { get; }
    public DelegateCommand<LotInfo?> MergeLotCommand { get; }
    public DelegateCommand<LotInfo?> TraceLotCommand { get; }
    public DelegateCommand ExpandAllCommand { get; }
    public DelegateCommand CollapseAllCommand { get; }
    public DelegateCommand<LotInfo?> ToggleExpandCommand { get; }

    #endregion

    #region 初始化和数据加载

    private async Task InitializeAsync()
    {
        try
        {
            ErrorMessage = null;
            await _dataService.EnsureSeededAsync();
            await ReloadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"数据加载失败: {ex.Message}";
        }
    }

    private async Task ReloadDataAsync()
    {
        var lots = await _dataService.GetAllLotsAsync();
        Lots = new ObservableCollection<LotInfo>(lots);
        
        // 构建树形关系
        BuildLotHierarchy();
        
        ApplyFilter();
        UpdateStatistics();
    }

    /// <summary>
    /// 构建批次层级关系
    /// 将扁平数据转换为树形结构
    /// </summary>
    private void BuildLotHierarchy()
    {
        // 先清空所有子批次
        foreach (var lot in Lots)
        {
            lot.Children.Clear();
        }
        
        // 建立父子关系
        foreach (var childLot in Lots.Where(l => !string.IsNullOrEmpty(l.ParentLotId)))
        {
            var parentLot = Lots.FirstOrDefault(l => l.LotId == childLot.ParentLotId);
            if (parentLot != null)
            {
                parentLot.Children.Add(childLot);
            }
        }
    }

    #endregion

    #region 过滤逻辑

    private void ApplyFilter()
    {
        var query = Lots.AsEnumerable();

        // 1. 层级过滤（只显示顶级批次，子批次通过展开显示）
        if (!ShowAllLevels)
        {
            query = ShowWaferLots ? query.Where(l => l.LotLevel == LotLevel.WaferLot) :
                    ShowMotherLots ? query.Where(l => l.LotLevel == LotLevel.MotherLot) :
                    ShowSubLots ? query.Where(l => l.LotLevel == LotLevel.SubLot) :
                    ShowGradeLots ? query.Where(l => l.LotLevel == LotLevel.GradeLot) :
                    ShowMfgIds ? query.Where(l => l.LotLevel == LotLevel.MfgId) : query;
        }
        else
        {
            // 只显示顶级批次（没有父批次的）
            query = query.Where(l => string.IsNullOrEmpty(l.ParentLotId));
        }

        // 2. 状态过滤
        if (!string.IsNullOrWhiteSpace(FilterStatus) && FilterStatus != "全部状态")
            query = query.Where(l => l.Status == FilterStatus);

        // 3. 工艺阶段过滤
        if (!string.IsNullOrWhiteSpace(FilterStage) && FilterStage != "全部阶段")
        {
            if (FilterStage == "Assemble")
                query = query.Where(l => l.ProcessStage == Models.ProcessStage.Assemble);
            else if (FilterStage == "Test")
                query = query.Where(l => l.ProcessStage == Models.ProcessStage.Test);
            else if (FilterStage == "Finished")
                query = query.Where(l => l.ProcessStage == Models.ProcessStage.Finished);
        }

        // 4. 搜索
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var key = SearchText.Trim().ToLower();
            query = query.Where(l =>
                l.LotId.ToLower().Contains(key)
                || l.OrderId.ToLower().Contains(key)
                || l.ProductName.ToLower().Contains(key)
                || l.DieName.ToLower().Contains(key)
                || l.CurrentStep.ToLower().Contains(key));
        }

        // 5. 排序
        var sorted = query
            .OrderByDescending(l => l.Priority == "Urgent")
            .ThenByDescending(l => l.Status == "Hold")
            .ThenBy(l => l.CurrentStepSeq)
            .ThenBy(l => l.LotId)
            .ToList();

        FilteredLots = new ObservableCollection<LotInfo>(sorted);
    }

    private void OnClearFilter()
    {
        SearchText = string.Empty;
        FilterStatus = "全部状态";
        FilterStage = "全部阶段";
        ShowAllLevels = true;
    }

    #endregion

    #region 树形展开/折叠

    private void OnExpandAll()
    {
        foreach (var lot in FilteredLots)
        {
            SetExpandState(lot, true);
        }
        RefreshCollection();
    }

    private void OnCollapseAll()
    {
        foreach (var lot in FilteredLots)
        {
            SetExpandState(lot, false);
        }
        RefreshCollection();
    }

    private void OnToggleExpand(LotInfo? lot)
    {
        if (lot == null) return;
        
        // 切换展开状态
        lot.IsExpanded = !lot.IsExpanded;
        RefreshCollection();
    }

    private void SetExpandState(LotInfo lot, bool isExpanded)
    {
        lot.IsExpanded = isExpanded;
        
        // 递归设置子批次
        foreach (var child in lot.Children)
        {
            SetExpandState(child, isExpanded);
        }
    }

    private void RefreshCollection()
    {
        // 触发 UI 刷新
        var temp = FilteredLots;
        FilteredLots = null;
        FilteredLots = temp;
    }

    #endregion

    #region 操作命令

    private void OnViewDetail(LotInfo? lot)
    {
        if (lot == null) return;
        var parameters = new NavigationParameters { { "LotId", lot.LotId } };
        _regionManager.RequestNavigate("MainContentRegion", "LotDetailViewV2", parameters);
    }

    private void OnHoldLot(LotInfo? lot)
    {
        if (lot == null) return;
        System.Windows.MessageBox.Show(
            $"Hold 批次: {lot.LotId}",
            "Hold",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information);
    }

    private void OnReleaseLot(LotInfo? lot)
    {
        if (lot == null) return;
        System.Windows.MessageBox.Show(
            $"Release 批次: {lot.LotId}",
            "Release",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information);
    }

    private void OnSplitLot(LotInfo? lot)
    {
        if (lot == null) return;
        System.Windows.MessageBox.Show(
            $"拆分批次: {lot.LotId}",
            "拆分",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information);
    }

    private void OnMergeLot(LotInfo? lot)
    {
        if (lot == null) return;
        System.Windows.MessageBox.Show(
            $"合并批次: {lot.LotId}",
            "合并",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information);
    }

    private void OnTraceLot(LotInfo? lot)
    {
        if (lot == null) return;
        System.Windows.MessageBox.Show(
            $"追踪批次: {lot.LotId}",
            "追踪",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information);
    }

    #endregion

    #region 统计更新

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(WaferLotCount));
        RaisePropertyChanged(nameof(MotherLotCount));
        RaisePropertyChanged(nameof(SubLotCount));
        RaisePropertyChanged(nameof(GradeLotCount));
        RaisePropertyChanged(nameof(MfgIdCount));
        RaisePropertyChanged(nameof(HoldCount));
    }

    private void RaiseCanExecuteChanged()
    {
        ViewDetailCommand.RaiseCanExecuteChanged();
        HoldLotCommand.RaiseCanExecuteChanged();
        ReleaseLotCommand.RaiseCanExecuteChanged();
        SplitLotCommand.RaiseCanExecuteChanged();
        MergeLotCommand.RaiseCanExecuteChanged();
        TraceLotCommand.RaiseCanExecuteChanged();
    }

    #endregion
}
