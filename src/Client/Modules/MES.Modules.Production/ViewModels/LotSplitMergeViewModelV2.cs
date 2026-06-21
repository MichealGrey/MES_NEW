using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Media;
using MES.Domain.Production;
using MES.Modules.Production.Models;
using MES.Shared.Converters;

namespace MES.Modules.Production.ViewModels;

/// <summary>
/// 批次拆分/合并 ViewModel V2 版本
/// 支持五层模型的层级转换
/// </summary>
public class LotSplitMergeViewModelV2 : BindableBase
{
    #region 字段

    private bool _isSplitMode = true;
    private bool _isMergeMode;
    private string _sourceLotId = string.Empty;
    private LotLevel _sourceLotLevel = LotLevel.MotherLot;
    private int _sourceLotQty = 12000;
    
    private bool _targetLevelWafer;
    private bool _targetLevelMother = true;
    private bool _targetLevelSub;
    private bool _targetLevelGrade;
    private bool _targetLevelMfg;
    
    private string _splitReason = string.Empty;
    private string _validationMessage = "请选择源批次和目标层级，配置拆分数量后点击执行";
    
    private string _targetLotIdPreview = "LOT-20260001-T1";
    private int _targetLotQtyPreview = 4000;
    
    private ObservableCollection<SplitTarget> _splitTargets = [];
    private ObservableCollection<HistoryRecord> _historyRecords = [];

    #endregion

    #region 命令

    public DelegateCommand LoadSourceLotCommand { get; }
    public DelegateCommand SplitEvenlyCommand { get; }
    public DelegateCommand SplitByRatioCommand { get; }
    public DelegateCommand ExecuteSplitCommand { get; }
    public DelegateCommand ResetCommand { get; }

    #endregion

    #region 构造函数

    public LotSplitMergeViewModelV2()
    {
        // 初始化命令
        LoadSourceLotCommand = new DelegateCommand(LoadSourceLot);
        SplitEvenlyCommand = new DelegateCommand(SplitEvenly);
        SplitByRatioCommand = new DelegateCommand(SplitByRatio);
        ExecuteSplitCommand = new DelegateCommand(ExecuteSplit);
        ResetCommand = new DelegateCommand(Reset);
        
        // 初始化示例数据
        InitializeDemoData();
    }

    #endregion

    #region 公开属性

    public bool IsSplitMode
    {
        get => _isSplitMode;
        set
        {
            if (SetProperty(ref _isSplitMode, value))
            {
                if (value) IsMergeMode = false;
            }
        }
    }

    public bool IsMergeMode
    {
        get => _isMergeMode;
        set
        {
            if (SetProperty(ref _isMergeMode, value))
            {
                if (value) IsSplitMode = false;
            }
        }
    }

    public string SourceLotId
    {
        get => _sourceLotId;
        set => SetProperty(ref _sourceLotId, value);
    }

    public LotLevel SourceLotLevel
    {
        get => _sourceLotLevel;
        set => SetProperty(ref _sourceLotLevel, value);
    }

    public int SourceLotQty
    {
        get => _sourceLotQty;
        set => SetProperty(ref _sourceLotQty, value);
    }

    public bool TargetLevelWafer
    {
        get => _targetLevelWafer;
        set
        {
            if (SetProperty(ref _targetLevelWafer, value))
            {
                if (value)
                {
                    TargetLevelMother = false;
                    TargetLevelSub = false;
                    TargetLevelGrade = false;
                    TargetLevelMfg = false;
                }
                UpdateTargetPreview();
            }
        }
    }

    public bool TargetLevelMother
    {
        get => _targetLevelMother;
        set
        {
            if (SetProperty(ref _targetLevelMother, value))
            {
                if (value)
                {
                    TargetLevelWafer = false;
                    TargetLevelSub = false;
                    TargetLevelGrade = false;
                    TargetLevelMfg = false;
                }
                UpdateTargetPreview();
            }
        }
    }

    public bool TargetLevelSub
    {
        get => _targetLevelSub;
        set
        {
            if (SetProperty(ref _targetLevelSub, value))
            {
                if (value)
                {
                    TargetLevelWafer = false;
                    TargetLevelMother = false;
                    TargetLevelGrade = false;
                    TargetLevelMfg = false;
                }
                UpdateTargetPreview();
            }
        }
    }

    public bool TargetLevelGrade
    {
        get => _targetLevelGrade;
        set
        {
            if (SetProperty(ref _targetLevelGrade, value))
            {
                if (value)
                {
                    TargetLevelWafer = false;
                    TargetLevelMother = false;
                    TargetLevelSub = false;
                    TargetLevelMfg = false;
                }
                UpdateTargetPreview();
            }
        }
    }

    public bool TargetLevelMfg
    {
        get => _targetLevelMfg;
        set
        {
            if (SetProperty(ref _targetLevelMfg, value))
            {
                if (value)
                {
                    TargetLevelWafer = false;
                    TargetLevelMother = false;
                    TargetLevelSub = false;
                    TargetLevelGrade = false;
                }
                UpdateTargetPreview();
            }
        }
    }

    public string SplitReason
    {
        get => _splitReason;
        set => SetProperty(ref _splitReason, value);
    }

    public string ValidationMessage
    {
        get => _validationMessage;
        set => SetProperty(ref _validationMessage, value);
    }

    public string TargetLotIdPreview
    {
        get => _targetLotIdPreview;
        set => SetProperty(ref _targetLotIdPreview, value);
    }

    public int TargetLotQtyPreview
    {
        get => _targetLotQtyPreview;
        set => SetProperty(ref _targetLotQtyPreview, value);
    }

    public ObservableCollection<SplitTarget> SplitTargets
    {
        get => _splitTargets;
        set => SetProperty(ref _splitTargets, value);
    }

    public ObservableCollection<HistoryRecord> HistoryRecords
    {
        get => _historyRecords;
        set => SetProperty(ref _historyRecords, value);
    }

    // 目标层级相关的计算属性
    public Brush TargetLevelBrush
    {
        get
        {
            if (TargetLevelWafer) return new SolidColorBrush(Color.FromRgb(0x8B, 0x5C, 0xF6));
            if (TargetLevelMother) return new SolidColorBrush(Color.FromRgb(0x06, 0xB6, 0xD4));
            if (TargetLevelSub) return new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81));
            if (TargetLevelGrade) return new SolidColorBrush(Color.FromRgb(0xF5, 0x9E, 0x0B));
            if (TargetLevelMfg) return new SolidColorBrush(Color.FromRgb(0xEC, 0x48, 0x99));
            return Brushes.Gray;
        }
    }

    public Brush TargetLevelForeground
    {
        get
        {
            if (TargetLevelGrade || TargetLevelMfg)
                return Brushes.Black;
            return Brushes.White;
        }
    }

    public string TargetLevelLabel
    {
        get
        {
            if (TargetLevelWafer) return "L1 Wafer Lot";
            if (TargetLevelMother) return "L2 Mother Lot";
            if (TargetLevelSub) return "L3 Sub Lot";
            if (TargetLevelGrade) return "L4 Grade Lot";
            if (TargetLevelMfg) return "L5 Mfg ID";
            return "选择目标层级";
        }
    }

    #endregion

    #region 私有方法

    private void InitializeDemoData()
    {
        SourceLotId = "LOT-20260001";
        SourceLotLevel = LotLevel.MotherLot;
        SourceLotQty = 12000;
        SplitReason = "工序拆分";
        
        // 初始化拆分目标
        SplitTargets = new ObservableCollection<SplitTarget>
        {
            new SplitTarget { TargetLotId = "LOT-20260001-T1", Qty = 4000, Remarks = "Tray 1" },
            new SplitTarget { TargetLotId = "LOT-20260001-T2", Qty = 4000, Remarks = "Tray 2" },
            new SplitTarget { TargetLotId = "LOT-20260001-T3", Qty = 4000, Remarks = "Tray 3" }
        };
        
        // 初始化历史记录
        HistoryRecords = new ObservableCollection<HistoryRecord>
        {
            new HistoryRecord
            {
                Time = DateTime.Now.AddMinutes(-30),
                Action = "拆分",
                SourceLot = "LOT-20250001",
                TargetLot = "LOT-20250001-T1",
                Qty = 3000,
                Status = "成功"
            },
            new HistoryRecord
            {
                Time = DateTime.Now.AddHours(-1),
                Action = "拆分",
                SourceLot = "LOT-20250001",
                TargetLot = "LOT-20250001-T2",
                Qty = 3000,
                Status = "成功"
            }
        };
        
        UpdateTargetPreview();
    }

    private void UpdateTargetPreview()
    {
        // 更新目标预览
        if (TargetLevelSub)
        {
            TargetLotIdPreview = SourceLotId + "-T1";
            TargetLotQtyPreview = SourceLotQty / 3;
        }
        else if (TargetLevelGrade)
        {
            TargetLotIdPreview = SourceLotId + "-G1";
            TargetLotQtyPreview = SourceLotQty / 2;
        }
        else if (TargetLevelMfg)
        {
            TargetLotIdPreview = "MFG-" + DateTime.Now.ToString("yyyyMMdd");
            TargetLotQtyPreview = 1000;
        }
        else
        {
            TargetLotIdPreview = SourceLotId + "-NEW";
            TargetLotQtyPreview = SourceLotQty;
        }
        
        RaisePropertyChanged(nameof(TargetLevelBrush));
        RaisePropertyChanged(nameof(TargetLevelForeground));
        RaisePropertyChanged(nameof(TargetLevelLabel));
    }

    private void LoadSourceLot()
    {
        ValidationMessage = "源批次已加载: " + SourceLotId + ", 数量: " + SourceLotQty;
    }

    private void SplitEvenly()
    {
        int numLots = 3;
        int qtyPerLot = SourceLotQty / numLots;
        
        SplitTargets.Clear();
        for (int i = 1; i <= numLots; i++)
        {
            SplitTargets.Add(new SplitTarget
            {
                TargetLotId = SourceLotId + "-T" + i,
                Qty = qtyPerLot,
                Remarks = "Tray " + i
            });
        }
        
        ValidationMessage = "已平均拆分: " + numLots + " 个子批次, 每批 " + qtyPerLot + " pcs";
    }

    private void SplitByRatio()
    {
        // 按比例拆分示例：70% 和 30%
        int qty1 = (int)(SourceLotQty * 0.7);
        int qty2 = SourceLotQty - qty1;
        
        SplitTargets.Clear();
        SplitTargets.Add(new SplitTarget
        {
            TargetLotId = SourceLotId + "-A",
            Qty = qty1,
            Remarks = "Grade A (70%)"
        });
        SplitTargets.Add(new SplitTarget
        {
            TargetLotId = SourceLotId + "-B",
            Qty = qty2,
            Remarks = "Grade B (30%)"
        });
        
        ValidationMessage = "已按比例拆分: 70% - 30%";
    }

    private void ExecuteSplit()
    {
        // 验证
        int totalSplitQty = SplitTargets.Sum(t => t.Qty);
        if (totalSplitQty != SourceLotQty)
        {
            ValidationMessage = "验证失败: 拆分数量总和(" + totalSplitQty + ") 不等于源批次数量(" + SourceLotQty + ")";
            return;
        }
        
        // 模拟执行
        var record = new HistoryRecord
        {
            Time = DateTime.Now,
            Action = IsSplitMode ? "拆分" : "合并",
            SourceLot = SourceLotId,
            TargetLot = string.Join(", ", SplitTargets.Select(t => t.TargetLotId)),
            Qty = totalSplitQty,
            Status = "成功"
        };
        
        HistoryRecords.Insert(0, record);
        
        ValidationMessage = "执行成功！已" + record.Action + "批次，数量: " + totalSplitQty;
    }

    private void Reset()
    {
        SplitTargets.Clear();
        SplitReason = string.Empty;
        ValidationMessage = "已重置，请重新配置拆分参数";
    }

    #endregion
}

#region 数据模型

/// <summary>
/// 拆分目标
/// </summary>
public class SplitTarget : BindableBase
{
    private string _targetLotId = string.Empty;
    private int _qty;
    private string _remarks = string.Empty;

    public string TargetLotId
    {
        get => _targetLotId;
        set => SetProperty(ref _targetLotId, value);
    }

    public int Qty
    {
        get => _qty;
        set => SetProperty(ref _qty, value);
    }

    public string Remarks
    {
        get => _remarks;
        set => SetProperty(ref _remarks, value);
    }
}

/// <summary>
/// 历史记录
/// </summary>
public class HistoryRecord : BindableBase
{
    private DateTime _time;
    private string _action = string.Empty;
    private string _sourceLot = string.Empty;
    private string _targetLot = string.Empty;
    private int _qty;
    private string _status = string.Empty;

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

    public string SourceLot
    {
        get => _sourceLot;
        set => SetProperty(ref _sourceLot, value);
    }

    public string TargetLot
    {
        get => _targetLot;
        set => SetProperty(ref _targetLot, value);
    }

    public int Qty
    {
        get => _qty;
        set => SetProperty(ref _qty, value);
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }
}

#endregion
