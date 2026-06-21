using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Media;
using MES.Domain.Production;
using MES.Modules.Production.Models;
using MES.Shared.Converters;

namespace MES.Modules.Production.ViewModels;

/// <summary>
/// 批次详情 ViewModel V2 版本
/// 包含：
/// - 层级关系图数据
/// - 工序流程数据
/// - 操作记录数据
/// </summary>
public class LotDetailViewModelV2 : BindableBase
{
    private LotInfo? _lotInfo;
    private ObservableCollection<HierarchyNode> _hierarchyNodes = [];
    private ObservableCollection<ProcessStep> _processSteps = [];
    private ObservableCollection<LotOperationRecord> _operationRecords = [];

    public LotDetailViewModelV2()
    {
        // 构造示例数据
        InitializeDemoData();
    }

    #region 公开属性

    public LotInfo? LotInfo
    {
        get => _lotInfo;
        set => SetProperty(ref _lotInfo, value);
    }

    public ObservableCollection<HierarchyNode> HierarchyNodes
    {
        get => _hierarchyNodes;
        set => SetProperty(ref _hierarchyNodes, value);
    }

    public ObservableCollection<ProcessStep> ProcessSteps
    {
        get => _processSteps;
        set => SetProperty(ref _processSteps, value);
    }

    public ObservableCollection<LotOperationRecord> OperationRecords
    {
        get => _operationRecords;
        set => SetProperty(ref _operationRecords, value);
    }

    #endregion

    #region 示例数据初始化

    private void InitializeDemoData()
    {
        // 示例批次
        LotInfo = new LotInfo
        {
            LotId = "LOT-20260001-T1",
            OrderId = "WO-2026-001",
            LotLevel = LotLevel.SubLot,
            ProductName = "IC-1001",
            PackageType = PackageType.QFP,
            CurrentStep = "WireBond",
            CurrentStepSeq = 3,
            Status = "Processing",
            UnitCount = 1200,
            OriginalQty = 1200,
            TotalPassQty = 1150,
            TotalScrapQty = 50,
            CarrierId = "TRAY-001",
            Priority = "Normal",
            RouteId = "RT-ASSM-001"
        };

        // 层级关系图数据
        HierarchyNodes = new ObservableCollection<HierarchyNode>
        {
            new HierarchyNode
            {
                Level = LotLevel.WaferLot,
                LevelLabel = "L1 Wafer",
                LotId = "WF-20260501-001",
                Summary = "晶圆批次: 10000 片",
                IsCurrent = false,
                HasChild = true,
                LevelBrush = new SolidColorBrush(Color.FromRgb(0x8B, 0x5C, 0xF6)),
                LevelForeground = Brushes.White
            },
            new HierarchyNode
            {
                Level = LotLevel.MotherLot,
                LevelLabel = "L2 Mother",
                LotId = "LOT-20260001",
                Summary = "母批次: 12000 pcs",
                IsCurrent = false,
                HasChild = true,
                LevelBrush = new SolidColorBrush(Color.FromRgb(0x06, 0xB6, 0xD4)),
                LevelForeground = Brushes.White
            },
            new HierarchyNode
            {
                Level = LotLevel.SubLot,
                LevelLabel = "L3 Sub",
                LotId = "LOT-20260001-T1",
                Summary = "子批次: 4000 pcs",
                IsCurrent = true,
                HasChild = true,
                LevelBrush = new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)),
                LevelForeground = Brushes.White
            },
            new HierarchyNode
            {
                Level = LotLevel.GradeLot,
                LevelLabel = "L4 Grade",
                LotId = "LOT-20260001-T1-G1",
                Summary = "等级 A: 3000 pcs",
                IsCurrent = false,
                HasChild = true,
                LevelBrush = new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B)),
                LevelForeground = Brushes.Black
            },
            new HierarchyNode
            {
                Level = LotLevel.MfgId,
                LevelLabel = "L5 Mfg",
                LotId = "MFG-001-20260529",
                Summary = "Mfg ID: 1000 pcs",
                IsCurrent = false,
                HasChild = false,
                LevelBrush = new SolidColorBrush(Color.FromRgb(0xEC, 0x48, 0x99)),
                LevelForeground = Brushes.Black
            }
        };

        // 工序流程数据
        ProcessSteps = new ObservableCollection<ProcessStep>
        {
            new ProcessStep
            {
                StepName = "Die Attach",
                Status = "Completed",
                Icon = "✓",
                StatusBrush = new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81)),
                HasNext = true
            },
            new ProcessStep
            {
                StepName = "Wire Bond",
                Status = "Processing",
                Icon = "🔄",
                StatusBrush = new SolidColorBrush(Color.FromRgb(0x3B, 0x82, 0xF6)),
                HasNext = true
            },
            new ProcessStep
            {
                StepName = "Mold",
                Status = "Pending",
                Icon = "⚪",
                StatusBrush = new SolidColorBrush(Color.FromRgb(0x4A, 0x5A, 0x72)),
                HasNext = true
            },
            new ProcessStep
            {
                StepName = "Trim Form",
                Status = "Pending",
                Icon = "⚪",
                StatusBrush = new SolidColorBrush(Color.FromRgb(0x4A, 0x5A, 0x72)),
                HasNext = true
            },
            new ProcessStep
            {
                StepName = "Marking",
                Status = "Pending",
                Icon = "⚪",
                StatusBrush = new SolidColorBrush(Color.FromRgb(0x4A, 0x5A, 0x72)),
                HasNext = true
            },
            new ProcessStep
            {
                StepName = "Final Test",
                Status = "Pending",
                Icon = "⚪",
                StatusBrush = new SolidColorBrush(Color.FromRgb(0x4A, 0x5A, 0x72)),
                HasNext = false
            }
        };

        // 操作记录数据
        OperationRecords = new ObservableCollection<LotOperationRecord>
        {
            new LotOperationRecord
            {
                Time = DateTime.Now.AddHours(-4),
                Action = "进站",
                Operator = "张工",
                Details = "进入 WireBond 工序, 使用设备 WB-001"
            },
            new LotOperationRecord
            {
                Time = DateTime.Now.AddHours(-10),
                Action = "出站",
                Operator = "李工",
                Details = "从 DieAttach 工序出站, 合格 1150pcs, 报废 50pcs"
            },
            new LotOperationRecord
            {
                Time = DateTime.Now.AddHours(-12),
                Action = "进站",
                Operator = "王工",
                Details = "进入 DieAttach 工序, 使用设备 DA-001"
            },
            new LotOperationRecord
            {
                Time = DateTime.Now.AddDays(-1),
                Action = "拆分",
                Operator = "班长",
                Details = "从母批次 LOT-20260001 拆分, 分配 4000pcs"
            }
        };
    }

    #endregion
}

#region 数据模型

/// <summary>
/// 层级关系图节点
/// </summary>
public class HierarchyNode : BindableBase
{
    private LotLevel _level;
    private string _levelLabel = string.Empty;
    private string _lotId = string.Empty;
    private string _summary = string.Empty;
    private bool _isCurrent;
    private bool _hasChild;
    private Brush _levelBrush = Brushes.Gray;
    private Brush _levelForeground = Brushes.White;

    public LotLevel Level
    {
        get => _level;
        set => SetProperty(ref _level, value);
    }

    public string LevelLabel
    {
        get => _levelLabel;
        set => SetProperty(ref _levelLabel, value);
    }

    public string LotId
    {
        get => _lotId;
        set => SetProperty(ref _lotId, value);
    }

    public string Summary
    {
        get => _summary;
        set => SetProperty(ref _summary, value);
    }

    public bool IsCurrent
    {
        get => _isCurrent;
        set => SetProperty(ref _isCurrent, value);
    }

    public bool HasChild
    {
        get => _hasChild;
        set => SetProperty(ref _hasChild, value);
    }

    public Brush LevelBrush
    {
        get => _levelBrush;
        set => SetProperty(ref _levelBrush, value);
    }

    public Brush LevelForeground
    {
        get => _levelForeground;
        set => SetProperty(ref _levelForeground, value);
    }
}

/// <summary>
/// 工序步骤
/// </summary>
public class ProcessStep : BindableBase
{
    private string _stepName = string.Empty;
    private string _status = string.Empty;
    private string _icon = string.Empty;
    private Brush _statusBrush = Brushes.Gray;
    private bool _hasNext;

    public string StepName
    {
        get => _stepName;
        set => SetProperty(ref _stepName, value);
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public string Icon
    {
        get => _icon;
        set => SetProperty(ref _icon, value);
    }

    public Brush StatusBrush
    {
        get => _statusBrush;
        set => SetProperty(ref _statusBrush, value);
    }

    public bool HasNext
    {
        get => _hasNext;
        set => SetProperty(ref _hasNext, value);
    }
}

/// <summary>
/// 操作记录
/// </summary>
public class LotOperationRecord : BindableBase
{
    private DateTime _time;
    private string _action = string.Empty;
    private string _operator = string.Empty;
    private string _details = string.Empty;

    public DateTime Time
    {
        get => _time;
        set => SetProperty(ref _time, value);
    }

    public string Action
    {
        get => _action;
        set => SetProperty(ref _action, value);
    }

    public string Operator
    {
        get => _operator;
        set => SetProperty(ref _operator, value);
    }

    public string Details
    {
        get => _details;
        set => SetProperty(ref _details, value);
    }
}

#endregion
