using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using MES.Shared.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class TrackInViewModel : BindableBase
{
    private readonly ISessionService _session;
    private readonly IRepository<ProdLot> _lotRepo;
    private readonly IProductionDataService _dataService;
    private readonly IRouteService _routeService;
    private readonly ITrackService _trackService;
    private readonly IQuantityService _quantityService;
    private readonly IOperationHistoryService _opHistoryService;

    // --- 扫描输入 ---
    private string _lotId = string.Empty;
    private string _equipmentId = string.Empty;
    private string _carrierId = string.Empty;
    private TrackOperationMode _operationMode = TrackOperationMode.TrackIn;

    // --- 批次信息 ---
    private LotInfo? _currentLotInfo;
    private bool _isLotInfoVisible;

    // --- 出站数量输入 ---
    private int _inputQty;
    private int _passQty;
    private int _failQty;
    private int _scrapQty;
    private int _reworkQty;
    private int _holdQty;
    private int _pendingQty;
    private string _trackOutRemark = string.Empty;
    private bool _isTrackOutInputVisible;

    // --- UI 状态 ---
    private bool _isLoading;
    private bool _canConfirm;
    private bool _canForceConfirm;
    private bool _hasCriticalFailures;
    private string _failureSummaryText = string.Empty;
    private string _currentOperator = "当前用户";
    private string _statusMessage = string.Empty;

    // --- 工单扩展信息 ---
    private string _customerName = string.Empty;
    private string _recipeName = string.Empty;
    private string _targetYieldDisplay = string.Empty;
    private string _unitLabel = "颗";
    private string _waferSourceDisplay = string.Empty;
    private string _routeDisplay = string.Empty;

    // --- 设备状态 ---
    private string _equipmentStatusDisplay = "Idle";
    private Brush _equipmentStatusBrush = Brushes.Gray;

    // --- Hold 告警 ---
    private bool _isHoldBannerVisible;
    private string _holdBannerText = string.Empty;
    private string _holdDetailText = string.Empty;
    private string _holdDurationText = string.Empty;

    // --- 集合 ---
    private ObservableCollection<ValidationResult> _validationResults = [];
    private ObservableCollection<OperationRecord> _operationHistory = [];

    #region === 属性 ===

    public string LotId
    {
        get => _lotId;
        set
        {
            SetProperty(ref _lotId, value);
        }
    }

    public string EquipmentId
    {
        get => _equipmentId;
        set
        {
            SetProperty(ref _equipmentId, value);
        }
    }

    public string CarrierId
    {
        get => _carrierId;
        set => SetProperty(ref _carrierId, value);
    }

    public bool IsTrackInMode
    {
        get => _operationMode == TrackOperationMode.TrackIn;
        set
        {
            if (value && _operationMode != TrackOperationMode.TrackIn)
            {
                _operationMode = TrackOperationMode.TrackIn;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsTrackOutMode));
                RaisePropertyChanged(nameof(PageTitle));
                IsTrackOutInputVisible = false;
            }
        }
    }

    public bool IsTrackOutMode
    {
        get => _operationMode == TrackOperationMode.TrackOut;
        set
        {
            if (value && _operationMode != TrackOperationMode.TrackOut)
            {
                _operationMode = TrackOperationMode.TrackOut;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsTrackInMode));
                RaisePropertyChanged(nameof(PageTitle));
                IsTrackOutInputVisible = _currentLotInfo != null;
            }
        }
    }

    public TrackOperationMode OperationMode
    {
        get => _operationMode;
        set
        {
            SetProperty(ref _operationMode, value);
            RaisePropertyChanged(nameof(IsTrackInMode));
            RaisePropertyChanged(nameof(IsTrackOutMode));
            RaisePropertyChanged(nameof(PageTitle));
        }
    }

    public string PageTitle => OperationMode == TrackOperationMode.TrackIn ? "进站" : "出站";

    public LotInfo? CurrentLotInfo
    {
        get => _currentLotInfo;
        set
        {
            SetProperty(ref _currentLotInfo, value);
            RaisePropertyChanged(nameof(IsLotInfoVisible));
        }
    }

    public bool IsLotInfoVisible
    {
        get => _isLotInfoVisible;
        set => SetProperty(ref _isLotInfoVisible, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool CanConfirm
    {
        get => _canConfirm;
        set => SetProperty(ref _canConfirm, value);
    }

    public bool CanForceConfirm
    {
        get => _canForceConfirm;
        set => SetProperty(ref _canForceConfirm, value);
    }

    public bool HasCriticalFailures
    {
        get => _hasCriticalFailures;
        set => SetProperty(ref _hasCriticalFailures, value);
    }

    public string FailureSummaryText
    {
        get => _failureSummaryText;
        set => SetProperty(ref _failureSummaryText, value);
    }

    public string CurrentOperator
    {
        get => _currentOperator;
        set => SetProperty(ref _currentOperator, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string HistoryCountDisplay => $"共 {OperationHistory.Count} 条记录";

    public string CustomerName
    {
        get => _customerName;
        set => SetProperty(ref _customerName, value);
    }

    public string RecipeName
    {
        get => _recipeName;
        set => SetProperty(ref _recipeName, value);
    }

    public string TargetYieldDisplay
    {
        get => _targetYieldDisplay;
        set => SetProperty(ref _targetYieldDisplay, value);
    }

    public string UnitLabel
    {
        get => _unitLabel;
        set => SetProperty(ref _unitLabel, value);
    }

    public string WaferSourceDisplay
    {
        get => _waferSourceDisplay;
        set => SetProperty(ref _waferSourceDisplay, value);
    }

    public string RouteDisplay
    {
        get => _routeDisplay;
        set => SetProperty(ref _routeDisplay, value);
    }

    public string EquipmentStatusDisplay
    {
        get => _equipmentStatusDisplay;
        set => SetProperty(ref _equipmentStatusDisplay, value);
    }

    public Brush EquipmentStatusBrush
    {
        get => _equipmentStatusBrush;
        set => SetProperty(ref _equipmentStatusBrush, value);
    }

    public bool IsHoldBannerVisible
    {
        get => _isHoldBannerVisible;
        set => SetProperty(ref _isHoldBannerVisible, value);
    }

    public string HoldBannerText
    {
        get => _holdBannerText;
        set => SetProperty(ref _holdBannerText, value);
    }

    public string HoldDetailText
    {
        get => _holdDetailText;
        set => SetProperty(ref _holdDetailText, value);
    }

    public string HoldDurationText
    {
        get => _holdDurationText;
        set => SetProperty(ref _holdDurationText, value);
    }

    // --- 出站数量 ---
    public bool IsTrackOutInputVisible
    {
        get => _isTrackOutInputVisible;
        set => SetProperty(ref _isTrackOutInputVisible, value);
    }

    public int InputQty
    {
        get => _inputQty;
        set
        {
            SetProperty(ref _inputQty, value);
            UpdateBalanceStatus();
        }
    }

    public int PassQty
    {
        get => _passQty;
        set
        {
            SetProperty(ref _passQty, value);
            UpdateBalanceStatus();
        }
    }

    public int FailQty
    {
        get => _failQty;
        set
        {
            SetProperty(ref _failQty, value);
            UpdateBalanceStatus();
        }
    }

    public int ScrapQty
    {
        get => _scrapQty;
        set
        {
            SetProperty(ref _scrapQty, value);
            UpdateBalanceStatus();
        }
    }

    public int ReworkQty
    {
        get => _reworkQty;
        set
        {
            SetProperty(ref _reworkQty, value);
            UpdateBalanceStatus();
        }
    }

    public int HoldQty
    {
        get => _holdQty;
        set
        {
            SetProperty(ref _holdQty, value);
            UpdateBalanceStatus();
        }
    }

    public int PendingQty
    {
        get => _pendingQty;
        set
        {
            SetProperty(ref _pendingQty, value);
            UpdateBalanceStatus();
        }
    }

    public string TrackOutRemark
    {
        get => _trackOutRemark;
        set => SetProperty(ref _trackOutRemark, value);
    }

    public string BalanceStatusText { get; private set; } = string.Empty;
    public Brush BalanceStatusBrush { get; private set; } = Brushes.Gray;

    // --- 集合 ---
    public ObservableCollection<ValidationResult> ValidationResults
    {
        get => _validationResults;
        set => SetProperty(ref _validationResults, value);
    }

    public ObservableCollection<OperationRecord> OperationHistory
    {
        get => _operationHistory;
        set => SetProperty(ref _operationHistory, value);
    }

    #endregion

    public ICommand ScanLotCommand { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand ForceConfirmCommand { get; }

    public TrackInViewModel(
        ISessionService session,
        IRepository<ProdLot> lotRepo,
        IProductionDataService dataService,
        IRouteService routeService,
        ITrackService trackService,
        IQuantityService quantityService,
        IOperationHistoryService opHistoryService)
    {
        _session = session;
        _lotRepo = lotRepo;
        _dataService = dataService;
        _routeService = routeService;
        _trackService = trackService;
        _quantityService = quantityService;
        _opHistoryService = opHistoryService;

        _currentOperator = session.IsLoggedIn
            ? $"{session.DisplayName}({session.EmployeeId})"
            : "未登录";

        ValidationResults = new ObservableCollection<ValidationResult>();
        OperationHistory = new ObservableCollection<OperationRecord>();

        ScanLotCommand = new DelegateCommand(OnScanLot);
        ConfirmCommand = new DelegateCommand(OnConfirm, () => CanConfirm);
        CancelCommand = new DelegateCommand(OnCancel);
        ForceConfirmCommand = new DelegateCommand(OnForceConfirm, () => CanForceConfirm);
    }

    private static LotInfo MapToLotInfo(ProdLot entity)
    {
        return new LotInfo
        {
            LotId = entity.LotId,
            OrderId = entity.OrderId,
            ProductId = entity.ProductId,
            ProductName = entity.ProductName,
            DieName = entity.DieName ?? string.Empty,
            CurrentStep = entity.CurrentStepCode ?? string.Empty,
            Status = entity.Status,
            UnitCount = entity.UnitCount,
            StripCount = entity.StripCount,
            Priority = entity.Priority,
            RouteId = entity.RouteId,
            RouteVersion = entity.RouteVersion,
            CurrentStepSeq = entity.CurrentStepSeq,
            IsPartialLot = entity.IsPartialLot,
            MotherLotId = entity.MotherLotId,
            SplitReason = entity.SplitReason,
            SplitTime = entity.SplitTime,
            SplitQty = entity.SplitQty,
            IsReworkLot = entity.IsReworkLot,
            OriginalRouteId = entity.OriginalRouteId,
            ReworkRouteId = entity.ReworkRouteId,
            ReworkCount = entity.ReworkCount,
            ReworkReason = entity.ReworkReason,
            IsArchived = entity.IsArchived,
            IsUnderMRB = entity.IsUnderMrb,
            MRBReference = entity.MrbReference,
            MRBDisposition = entity.MrbDisposition,
            Grade = entity.Grade,
            OriginalLotId = entity.OriginalLotId,
            WaferLotId = entity.WaferLotId,
            OriginalQty = entity.OriginalQty,
            TotalPassQty = entity.TotalPassQty,
            TotalScrapQty = entity.TotalScrapQty,
            TotalReworkQty = entity.TotalReworkQty,
            TotalHoldQty = entity.TotalHoldQty,
            CarrierId = entity.CarrierId ?? string.Empty,
            BinResult = entity.BinResult,
            TestResult = entity.TestResult,
            QtyPass = entity.QtyPass,
            QtyFail = entity.QtyFail,
            HoldReason = entity.HoldReason,
            HoldTime = entity.HoldTime,
            HoldOperator = entity.HoldOperator,
            ReleaseCondition = entity.ReleaseCondition,
        };
    }

    private void SetEquipmentStatus(string status)
    {
        EquipmentStatusDisplay = status switch
        {
            "Idle" => "待机",
            "Running" => "运行中",
            "Down" => "停机",
            "PM" => "保养中",
            "Engineering" => "工程中",
            _ => status
        };

        EquipmentStatusBrush = status switch
        {
            "Idle" => new SolidColorBrush(Color.FromRgb(0x4C, 0xC5, 0x6C)),
            "Running" => new SolidColorBrush(Color.FromRgb(0x00, 0xD2, 0xFF)),
            "Down" => new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C)),
            "PM" => new SolidColorBrush(Color.FromRgb(0xF3, 0x9C, 0x12)),
            "Engineering" => new SolidColorBrush(Color.FromRgb(0x9B, 0x59, 0xB6)),
            _ => new SolidColorBrush(Color.FromRgb(0xB0, 0xB0, 0xC0))
        };
    }

    private void UpdateBalanceStatus()
    {
        var outputTotal = PassQty + FailQty + ScrapQty + ReworkQty + HoldQty + PendingQty;
        if (InputQty <= 0)
        {
            BalanceStatusText = "请输入投入数量";
            BalanceStatusBrush = Brushes.Gray;
            return;
        }

        if (outputTotal == InputQty)
        {
            BalanceStatusText = $"数量平衡 ✓ (投入 {InputQty} = 产出合计 {outputTotal})";
            BalanceStatusBrush = new SolidColorBrush(Color.FromRgb(0x4C, 0xC5, 0x6C));
        }
        else
        {
            BalanceStatusText = $"数量不平衡 ✗ (投入 {InputQty} ≠ 产出合计 {outputTotal}, 差异 {outputTotal - InputQty})";
            BalanceStatusBrush = new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C));
        }
        RaisePropertyChanged(nameof(BalanceStatusText));
        RaisePropertyChanged(nameof(BalanceStatusBrush));
    }

    private async void OnScanLot()
    {
        if (string.IsNullOrWhiteSpace(LotId))
            return;

        IsLoading = true;
        StatusMessage = string.Empty;

        try
        {
            // 从数据库查询批次
            var lotEntity = await _lotRepo.GetByIdAsync(LotId);
            LotInfo? lot = lotEntity is null ? null : MapToLotInfo(lotEntity);

            if (lot is null)
            {
                // 尝试从 Hold 批次中查询
                var holdLots = await _dataService.GetAllHoldLotsAsync();
                lot = holdLots.FirstOrDefault(l => l.LotId == LotId);
            }

            if (lot is null)
            {
                StatusMessage = $"批次 {LotId} 不存在";
                CurrentLotInfo = null;
                IsLotInfoVisible = false;
                ValidationResults.Clear();
                UpdateCommandStates();
                return;
            }

            CurrentLotInfo = lot;
            IsLotInfoVisible = true;

            // 填充扩展信息
            var wo = await _dataService.GetWorkOrderAsync(lot.OrderId);
            if (wo != null)
            {
                CustomerName = wo.CustomerName;
                RecipeName = wo.TestProgram;
                TargetYieldDisplay = $"{wo.TargetCPYield:F1}%";
                WaferSourceDisplay = wo.WaferSource;
                RouteDisplay = wo.RouteName;
            }

            SetEquipmentStatus("Idle");
            RefreshHoldBanner();

            // 加载操作历史
            await LoadOperationHistoryAsync(lot.LotId);

            // 如果切换到出站模式，自动填充投入数量
            if (IsTrackOutMode)
            {
                InputQty = lot.UnitCount;
                IsTrackOutInputVisible = true;
            }

            // 执行校验
            RunValidation(lot);

            if (string.IsNullOrWhiteSpace(CarrierId))
            {
                CarrierId = lot.CarrierId;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void RefreshHoldBanner()
    {
        if (CurrentLotInfo?.Status == "Hold" && !string.IsNullOrWhiteSpace(CurrentLotInfo.HoldReason))
        {
            IsHoldBannerVisible = true;
            var holdType = CurrentLotInfo.HoldTypeDisplay;
            HoldBannerText = $"[{holdType}] {CurrentLotInfo.HoldReason}";

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(CurrentLotInfo.HoldOperator))
                parts.Add($"提报人: {CurrentLotInfo.HoldOperator}");
            if (!string.IsNullOrWhiteSpace(CurrentLotInfo.ReleaseCondition))
                parts.Add($"释放条件: {CurrentLotInfo.ReleaseCondition}");

            HoldDetailText = string.Join(" | ", parts);
            HoldDurationText = CurrentLotInfo.HoldDuration;
        }
        else
        {
            IsHoldBannerVisible = false;
            HoldBannerText = string.Empty;
            HoldDetailText = string.Empty;
            HoldDurationText = string.Empty;
        }
    }

    private void RunValidation(LotInfo lot)
    {
        ValidationResults.Clear();

        // 1. 批次状态校验
        if (lot.Status == "Hold")
        {
            ValidationResults.Add(new ValidationResult
            {
                CheckItem = "批次状态",
                Status = ValidationStatus.Fail,
                Message = $"批次处于 Hold 状态，禁止{PageTitle}"
            });
        }
        else
        {
            ValidationResults.Add(new ValidationResult
            {
                CheckItem = "批次状态",
                Status = ValidationStatus.Pass,
                Message = $"批次状态: {lot.Status}"
            });
        }

        // 2. 工单校验
        if (!string.IsNullOrWhiteSpace(lot.OrderId))
        {
            ValidationResults.Add(new ValidationResult
            {
                CheckItem = "工单校验",
                Status = ValidationStatus.Pass,
                Message = $"已关联工单 {lot.OrderId}"
            });
        }
        else
        {
            ValidationResults.Add(new ValidationResult
            {
                CheckItem = "工单校验",
                Status = ValidationStatus.Fail,
                Message = "未找到关联工单"
            });
        }

        // 3. Route/Step 校验
        if (!string.IsNullOrWhiteSpace(lot.RouteId))
        {
            ValidationResults.Add(new ValidationResult
            {
                CheckItem = "工艺路线",
                Status = ValidationStatus.Pass,
                Message = $"路线 {lot.RouteId} V{lot.RouteVersion}, 当前工序: {lot.CurrentStep} (Step {lot.CurrentStepSeq})"
            });
        }
        else
        {
            ValidationResults.Add(new ValidationResult
            {
                CheckItem = "工艺路线",
                Status = ValidationStatus.Warning,
                Message = "未绑定工艺路线，将使用默认路线"
            });
        }

        // 4. 设备校验
        if (!string.IsNullOrWhiteSpace(EquipmentId))
        {
            ValidationResults.Add(new ValidationResult
            {
                CheckItem = "设备匹配",
                Status = ValidationStatus.Pass,
                Message = $"设备 {EquipmentId} 可用"
            });
        }
        else
        {
            ValidationResults.Add(new ValidationResult
            {
                CheckItem = "设备匹配",
                Status = ValidationStatus.Warning,
                Message = "未输入设备号"
            });
        }

        // 5. 载具校验
        if (!string.IsNullOrWhiteSpace(CarrierId))
        {
            ValidationResults.Add(new ValidationResult
            {
                CheckItem = "载具匹配",
                Status = ValidationStatus.Pass,
                Message = $"载具 {CarrierId}"
            });
        }

        UpdateCommandStates();
    }

    private async Task LoadOperationHistoryAsync(string lotId)
    {
        OperationHistory.Clear();
        var records = await _opHistoryService.GetByLotAsync(lotId);
        foreach (var r in records.OrderByDescending(x => x.Time).Take(20))
        {
            OperationHistory.Add(new OperationRecord
            {
                Time = r.Time,
                LotId = r.LotId,
                Operator = r.Operator,
                OperationType = r.OperationType,
                EquipmentId = r.EquipmentId,
                Remark = r.Remark
            });
        }
        RaisePropertyChanged(nameof(HistoryCountDisplay));
    }

    private void UpdateCommandStates()
    {
        var hasFail = ValidationResults.Any(v => v.Status == ValidationStatus.Fail);
        var hasWarning = ValidationResults.Any(v => v.Status == ValidationStatus.Warning);
        var failCount = ValidationResults.Count(v => v.Status == ValidationStatus.Fail);

        CanConfirm = !hasFail && CurrentLotInfo != null;
        CanForceConfirm = CurrentLotInfo != null;

        HasCriticalFailures = hasFail;
        FailureSummaryText = hasFail
            ? $"检测到 {failCount} 项校验失败，请处理后操作"
            : string.Empty;

        ((DelegateCommand)ConfirmCommand).RaiseCanExecuteChanged();
        ((DelegateCommand)ForceConfirmCommand).RaiseCanExecuteChanged();
    }

    private async void OnConfirm()
    {
        if (CurrentLotInfo == null)
            return;

        if (!_session.HasPermission("Production", "TrackInView"))
        {
            StatusMessage = "权限不足：您没有执行进出站操作的权限。";
            return;
        }

        IsLoading = true;
        StatusMessage = string.Empty;

        try
        {
            if (IsTrackInMode)
            {
                await ExecuteTrackInAsync();
            }
            else
            {
                await ExecuteTrackOutAsync();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async void OnForceConfirm()
    {
        if (CurrentLotInfo == null)
            return;

        if (!_session.HasPermission("Production", "TrackInView"))
        {
            StatusMessage = "权限不足：您没有执行强制进出站操作的权限。";
            return;
        }

        // 简单确认（V2 应改为电子签核弹窗）
        var reason = Microsoft.VisualBasic.Interaction.InputBox("请输入强制操作原因:", "强制操作确认", "异常放行", -1, -1);
        if (string.IsNullOrWhiteSpace(reason))
            return;

        IsLoading = true;
        StatusMessage = string.Empty;

        try
        {
            if (IsTrackInMode)
            {
                await ExecuteForceTrackInAsync(reason);
            }
            else
            {
                await ExecuteForceTrackOutAsync(reason);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExecuteTrackInAsync()
    {
        if (CurrentLotInfo is null)
        {
            StatusMessage = "请先扫描批次";
            return;
        }

        var lot = CurrentLotInfo;
        var routeId = string.IsNullOrEmpty(lot.RouteId) ? "RT-001" : lot.RouteId;
        var stepName = lot.CurrentStep;
        var stepSeq = lot.CurrentStepSeq > 0 ? lot.CurrentStepSeq : 1;

        // 获取当前 Step 信息
        var steps = await _routeService.GetStepsAsync(routeId);
        var currentStep = steps.FirstOrDefault(s => s.StepName == stepName) ??
            new RouteStep { StepSeq = stepSeq, StepName = stepName, StepCode = stepName };

        var request = new TrackInRequest
        {
            LotId = lot.LotId,
            RouteId = routeId,
            RouteVersion = lot.RouteVersion,
            StepSeq = currentStep.StepSeq,
            StepCode = currentStep.StepCode,
            StepName = currentStep.StepName,
            EquipmentId = EquipmentId,
            CarrierId = CarrierId,
            OperatorId = _session.EmployeeId,
            OperatorName = _session.DisplayName,
            Workstation = Environment.MachineName,
            InputQty = lot.UnitCount,
            Remark = "正常进站"
        };

        var validation = await _trackService.ValidateTrackInAsync(request);
        if (!validation.IsValid)
        {
            StatusMessage = $"进站校验失败: {string.Join("; ", validation.Errors)}";
            ValidationResults.Clear();
            foreach (var err in validation.Errors)
                ValidationResults.Add(new ValidationResult { CheckItem = "校验", Status = ValidationStatus.Fail, Message = err });
            foreach (var warn in validation.Warnings)
                ValidationResults.Add(new ValidationResult { CheckItem = "校验", Status = ValidationStatus.Warning, Message = warn });
            UpdateCommandStates();
            return;
        }

        var result = await _trackService.TrackInAsync(request);
        if (result.Success)
        {
            StatusMessage = result.Message;
            // 刷新批次信息
            var updatedLot = await _lotRepo.GetByIdAsync(lot.LotId);
            if (updatedLot is not null)
            {
                CurrentLotInfo = MapToLotInfo(updatedLot);
                RefreshHoldBanner();
                RunValidation(CurrentLotInfo);
            }
            await LoadOperationHistoryAsync(lot.LotId);
        }
        else
        {
            StatusMessage = $"进站失败: {result.Message}";
        }
    }

    private async Task ExecuteTrackOutAsync()
    {
        if (CurrentLotInfo is null)
        {
            StatusMessage = "请先扫描批次";
            return;
        }

        var lot = CurrentLotInfo;
        var routeId = string.IsNullOrEmpty(lot.RouteId) ? "RT-001" : lot.RouteId;

        var request = new TrackOutRequest
        {
            LotId = lot.LotId,
            RouteId = routeId,
            RouteVersion = lot.RouteVersion,
            StepSeq = lot.CurrentStepSeq,
            StepCode = lot.CurrentStep,
            StepName = lot.CurrentStep,
            EquipmentId = EquipmentId,
            OperatorId = _session.EmployeeId,
            OperatorName = _session.DisplayName,
            Workstation = Environment.MachineName,
            InputQty = InputQty,
            PassQty = PassQty,
            FailQty = FailQty,
            ScrapQty = ScrapQty,
            ReworkQty = ReworkQty,
            HoldQty = HoldQty,
            PendingQty = PendingQty,
            Remark = TrackOutRemark
        };

        var validation = await _trackService.ValidateTrackOutAsync(request);
        if (!validation.IsValid)
        {
            StatusMessage = $"出站校验失败: {string.Join("; ", validation.Errors)}";
            ValidationResults.Clear();
            foreach (var err in validation.Errors)
                ValidationResults.Add(new ValidationResult { CheckItem = "校验", Status = ValidationStatus.Fail, Message = err });
            foreach (var warn in validation.Warnings)
                ValidationResults.Add(new ValidationResult { CheckItem = "校验", Status = ValidationStatus.Warning, Message = warn });
            UpdateCommandStates();
            return;
        }

        var result = await _trackService.TrackOutAsync(request);
        if (result.Success)
        {
            StatusMessage = result.Message + (result.NextStepName != null ? $"\n下一站: {result.NextStepName}" : "\n工单已完成");
            var updatedLot = await _lotRepo.GetByIdAsync(lot.LotId);
            if (updatedLot is not null)
            {
                CurrentLotInfo = MapToLotInfo(updatedLot);
                RefreshHoldBanner();
                RunValidation(CurrentLotInfo);
            }
            await LoadOperationHistoryAsync(lot.LotId);
            ResetTrackOutInputs();
        }
        else
        {
            StatusMessage = $"出站失败: {result.Message}";
        }
    }

    private async Task ExecuteForceTrackInAsync(string reason)
    {
        if (CurrentLotInfo is null)
        {
            StatusMessage = "请先扫描批次";
            return;
        }

        var lot = CurrentLotInfo;
        var routeId = string.IsNullOrEmpty(lot.RouteId) ? "RT-001" : lot.RouteId;

        var request = new TrackInRequest
        {
            LotId = lot.LotId,
            RouteId = routeId,
            RouteVersion = lot.RouteVersion,
            StepSeq = lot.CurrentStepSeq > 0 ? lot.CurrentStepSeq : 1,
            StepCode = lot.CurrentStep,
            StepName = lot.CurrentStep,
            EquipmentId = EquipmentId,
            CarrierId = CarrierId,
            OperatorId = _session.EmployeeId,
            OperatorName = _session.DisplayName,
            Workstation = Environment.MachineName,
            InputQty = lot.UnitCount,
            Remark = $"强制进站: {reason}"
        };

        var result = await _trackService.ForceTrackInAsync(request, reason);
        if (result.Success)
        {
            StatusMessage = result.Message;
            var updatedLot = await _lotRepo.GetByIdAsync(lot.LotId);
            if (updatedLot is not null)
            {
                CurrentLotInfo = MapToLotInfo(updatedLot);
                RefreshHoldBanner();
                RunValidation(CurrentLotInfo);
            }
            await LoadOperationHistoryAsync(lot.LotId);
        }
        else
        {
            StatusMessage = $"强制进站失败: {result.Message}";
        }
    }

    private async Task ExecuteForceTrackOutAsync(string reason)
    {
        if (CurrentLotInfo is null)
        {
            StatusMessage = "请先扫描批次";
            return;
        }

        var lot = CurrentLotInfo;
        var routeId = string.IsNullOrEmpty(lot.RouteId) ? "RT-001" : lot.RouteId;

        var request = new TrackOutRequest
        {
            LotId = lot.LotId,
            RouteId = routeId,
            RouteVersion = lot.RouteVersion,
            StepSeq = lot.CurrentStepSeq,
            StepCode = lot.CurrentStep,
            StepName = lot.CurrentStep,
            EquipmentId = EquipmentId,
            OperatorId = _session.EmployeeId,
            OperatorName = _session.DisplayName,
            Workstation = Environment.MachineName,
            InputQty = InputQty,
            PassQty = PassQty,
            FailQty = FailQty,
            ScrapQty = ScrapQty,
            ReworkQty = ReworkQty,
            HoldQty = HoldQty,
            PendingQty = PendingQty,
            Remark = $"强制出站: {reason}"
        };

        var result = await _trackService.ForceTrackOutAsync(request, reason);
        if (result.Success)
        {
            StatusMessage = result.Message;
            var updatedLot = await _lotRepo.GetByIdAsync(lot.LotId);
            if (updatedLot is not null)
            {
                CurrentLotInfo = MapToLotInfo(updatedLot);
                RefreshHoldBanner();
                RunValidation(CurrentLotInfo);
            }
            await LoadOperationHistoryAsync(lot.LotId);
            ResetTrackOutInputs();
        }
        else
        {
            StatusMessage = $"强制出站失败: {result.Message}";
        }
    }

    private void ResetTrackOutInputs()
    {
        InputQty = 0;
        PassQty = 0;
        FailQty = 0;
        ScrapQty = 0;
        ReworkQty = 0;
        HoldQty = 0;
        PendingQty = 0;
        TrackOutRemark = string.Empty;
    }

    private void OnCancel()
    {
        ResetView();
    }

    private void ResetView()
    {
        LotId = string.Empty;
        EquipmentId = string.Empty;
        CarrierId = string.Empty;
        CurrentLotInfo = null;
        IsLotInfoVisible = false;
        IsLoading = false;
        IsHoldBannerVisible = false;
        HasCriticalFailures = false;
        FailureSummaryText = string.Empty;
        StatusMessage = string.Empty;
        CustomerName = string.Empty;
        RecipeName = string.Empty;
        TargetYieldDisplay = string.Empty;
        WaferSourceDisplay = string.Empty;
        RouteDisplay = string.Empty;
        EquipmentStatusDisplay = "Idle";
        EquipmentStatusBrush = Brushes.Gray;
        ValidationResults.Clear();
        OperationHistory.Clear();
        CanConfirm = false;
        CanForceConfirm = false;
        ResetTrackOutInputs();
        IsTrackOutInputVisible = false;
        ((DelegateCommand)ConfirmCommand).RaiseCanExecuteChanged();
        ((DelegateCommand)ForceConfirmCommand).RaiseCanExecuteChanged();
    }
}
