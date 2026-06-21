using System;
using System.Text.Json.Serialization;
using MES.Domain.Production;

namespace MES.Modules.Production.Models;

/// <summary>
/// 工艺阶段枚举
/// </summary>
public enum ProcessStage
{
    Assemble,  // 前道
    Test,      // 后道
    Finished        // 所有项目
}

/// <summary>
/// Hold 类型枚举
/// </summary>
public enum HoldType
{
    Engineering,  // 工程 Hold
    Quality,      // 品质 Hold
    Customer,     // 客户 Hold
    Material,     // 物料 Hold
    Equipment,    // 设备 Hold
    YieldHold,    // 良率 Hold
    DataHold,     // 数据 Hold
    MRB           // MRB 评审 Hold
}

/// <summary>
/// Hold 维度枚举（9 种）
/// </summary>
public enum HoldScope
{
    WorkOrder,    // 工单级
    WaferLot,     // 来料级
    Product,      // 产品级
    Package,      // 封装级
    Equipment,    // 设备级
    Step,         // 工序级
    Bin,          // Bin 结果级
    Lot,          // 单批次级
    Route         // 工艺路线级
}

public class LotInfo
{
    // === 基础字段 ===
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string DieName { get; set; } = string.Empty;
    public PackageType PackageType { get; set; } = PackageType.QFP;
    
    // === 新增批次层级字段 (5层模型) ===
    public LotLevel LotLevel { get; set; } = LotLevel.MotherLot;
    public string LotHierarchyPath { get; set; } = string.Empty;  // "WF-20260526-001/LOT-20260001/LOT-20260001-T1"
    public string RootWaferLotId { get; set; } = string.Empty;
    public ProcessStage ProcessStage { get; set; } = ProcessStage.Assemble;
    
    // === 5层关系字段 ===
    public string? WaferLotId { get; set; }         // Wafer Lot ID (L1)
    public string? MotherLotId { get; set; }          // Mother Lot ID (L2)
    public string? SubLotId { get; set; }           // Sub Lot ID (L3)
    public string? GradeLotId { get; set; }         // Grade Lot ID (L4)
    public string? MfgLotId { get; set; }           // Mfg ID (L5)
    public string? ParentLotId { get; set; }           // 直接父批次
    
    // 子批次
    public List<LotInfo> Children { get; set; } = new();
    public bool HasChildren => Children.Count > 0;
    
    // 树形展开状态
    public bool IsExpanded { get; set; }
    
    // === Route/Step 跟踪 ===
    public string RouteId { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = "1.0";
    public int CurrentStepSeq { get; set; }
    private string _currentStepCode = string.Empty;
    public string CurrentStepCode { get => _currentStepCode; set => _currentStepCode = value; }
    public string CurrentStep { get => _currentStepCode; set => _currentStepCode = value; }
    public string CurrentEquipment { get; set; } = string.Empty;
    public bool IsFirstStep => CurrentStepSeq <= 1;
    
    // === 状态 ===
    public string Status { get; set; } = "Waiting";
    public int UnitCount { get; set; }
    public int StripCount { get; set; }
    public string Priority { get; set; } = "Normal";
    
    // === 载具信息 ===
    public CarrierType CarrierType { get; set; } = CarrierType.Strip;
    public string CarrierId { get; set; } = string.Empty;
    public string? FrameId { get; set; }
    
    // === 数量字段 ===
    public int OriginalQty { get; set; }
    public int TotalPassQty { get; set; }
    public int TotalScrapQty { get; set; }
    public int TotalReworkQty { get; set; }
    public int TotalHoldQty { get; set; }
    
    // === Grade/Bin/测试 ===
    public string? BinResult { get; set; }
    public string? Grade { get; set; }
    public string? TestResult { get; set; }
    public int QtyPass { get; set; }
    public int QtyFail { get; set; }
    
    // === Split/Merge 追踪 ===
    public bool IsPartialLot { get; set; }
    public string? SplitReason { get; set; }
    public DateTime? SplitTime { get; set; }
    public int? SplitQty { get; set; }
    
    // === Rework ===
    public bool IsReworkLot { get; set; }
    public string? OriginalRouteId { get; set; }
    public string? ReworkRouteId { get; set; }
    public int? ReworkCount { get; set; }
    public string? ReworkReason { get; set; }
    
    // === MRB ===
    public bool IsUnderMRB { get; set; }
    public string? MRBReference { get; set; }
    public string? MRBDisposition { get; set; }
    
    // === Grade Split ===
    public string? OriginalLotId { get; set; }
    
    // === Hold 相关 ===
    public HoldType HoldCategory { get; set; }
    public HoldScope HoldScope { get; set; } = HoldScope.Lot;
    public string? HoldReason { get; set; }
    public DateTime? HoldTime { get; set; }
    public string? HoldOperator { get; set; }
    public string? ReleaseCondition { get; set; }
    
    // === 时间字段 ===
    public DateTime? CompletedAt { get; set; }
    public DateTime? InWarehouseAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    
    // === 归档 ===
    public bool IsArchived { get; set; }
    
    // === 只读计算属性 ===
    [JsonIgnore]
    public double CurrentYield => OriginalQty > 0 
        ? (double)(OriginalQty - TotalScrapQty) / OriginalQty * 100 : 100;
    
    [JsonIgnore]
    public bool IsHold => Status == "Hold";
    
    [JsonIgnore]
    public bool IsCompleted => Status == "Completed";
    
    [JsonIgnore]
    public bool IsScrapped => Status == "Scrapped";
    
    [JsonIgnore]
    public bool CanTrackIn => Status is "Waiting" or "Released" && !IsHold && !IsUnderMRB;
    
    [JsonIgnore]
    public string HoldDuration => HoldTime.HasValue
        ? FormatDuration(DateTime.Now - HoldTime.Value)
        : "—";
    
    [JsonIgnore]
    public bool IsHoldOverdue => HoldTime.HasValue
        && (DateTime.Now - HoldTime.Value).TotalHours > 24;
    
    [JsonIgnore]
    public string HoldTypeDisplay => HoldCategory switch
    {
        HoldType.Engineering => "ENG",
        HoldType.Quality => "QA",
        HoldType.Customer => "CUST",
        HoldType.Material => "MAT",
        HoldType.Equipment => "EQ",
        HoldType.YieldHold => "YLD",
        HoldType.DataHold => "DATA",
        _ => HoldCategory.ToString()
    };
    
    [JsonIgnore]
    public string PackageTypeDisplay => PackageType.ToString();
    
    [JsonIgnore]
    public string CarrierTypeDisplay => CarrierType switch
    {
        CarrierType.WaferFrame => "Frame",
        CarrierType.LeadFrame => "LF",
        CarrierType.Strip => "Strip",
        CarrierType.Magazine => "Mag",
        CarrierType.Tray => "Tray",
        CarrierType.Reel => "Reel",
        CarrierType.WafflePack => "WP",
        _ => CarrierType.ToString()
    };
    
    [JsonIgnore]
    public string LotLevelDisplay => LotLevel switch
    {
        LotLevel.WaferLot => "Wafer Lot",
        LotLevel.MotherLot => "Mother Lot",
        LotLevel.SubLot => "Sub Lot",
        LotLevel.GradeLot => "Grade Lot",
        LotLevel.MfgId => "MFG ID",
        _ => LotLevel.ToString()
    };
    
    [JsonIgnore]
    public string LotLevelShort => LotLevel switch
    {
        LotLevel.WaferLot => "WF",
        LotLevel.MotherLot => "M",
        LotLevel.SubLot => "S",
        LotLevel.GradeLot => "G",
        LotLevel.MfgId => "MFG",
        _ => LotLevel.ToString()
    };
    
    [JsonIgnore]
    public string ProcessStageDisplay => ProcessStage switch
    {
        ProcessStage.Assemble => "Assemble",
        ProcessStage.Test => "Test",
        ProcessStage.Finished => "Finished",
        _ => ProcessStage.ToString()
    };
    
    private static string FormatDuration(TimeSpan ts)
    {
        if (ts.TotalHours < 1) return $"{(int)ts.TotalMinutes}min";
        if (ts.TotalDays < 1) return $"{(int)ts.TotalHours}h{(int)ts.Minutes}m";
        return $"{(int)ts.TotalDays}d{(int)ts.Hours}h";
    }
}
