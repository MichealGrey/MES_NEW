namespace MES.Infrastructure.Persistence.Entities;

// ============================================================
// MFGID 追溯体系实体
// ============================================================

/// <summary>
/// MFG 最小追溯单位 (Reel/Tape 级别)
/// </summary>
public class MfgUnit
{
    public string MfgId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string RootWaferLotId { get; set; } = string.Empty;
    public string WaferId { get; set; } = string.Empty;
    public int DieX { get; set; } = -1;
    public int DieY { get; set; } = -1;
    public string SerialNumber { get; set; } = string.Empty;
    public string ReelId { get; set; } = string.Empty;
    public int ReelCapacity { get; set; }
    public int ActualQty { get; set; }
    public string Status { get; set; } = "Created";
    public string Grade { get; set; } = string.Empty;
    public string BinResult { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public int ReworkCount { get; set; }
    public DateTime? PackTime { get; set; }
    public string PackedBy { get; set; } = string.Empty;
    public string BoxId { get; set; } = string.Empty;
    public string PalletId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
}

/// <summary>
/// MFG 操作历史记录
/// </summary>
public class MfgOperationHistory
{
    public string HistoryId { get; set; } = string.Empty;
    public string MfgId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string? Operation { get; set; } // TrackIn/TrackOut/Hold/Release/Rework/Scrap
    public string EquipmentId { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string? TestData { get; set; } // JSON
    public string Remark { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// MFG 包装追溯记录
/// </summary>
public class MfgPackTrace
{
    public string PackTraceId { get; set; } = string.Empty;
    public string MfgId { get; set; } = string.Empty;
    public string? PackLevel { get; set; } // Unit/Reel/Box/Pallet
    public string? PackId { get; set; }
    public string ParentPackId { get; set; } = string.Empty;
    public int PackQty { get; set; }
    public DateTime PackedAt { get; set; } = DateTime.UtcNow;
    public string PackedBy { get; set; } = string.Empty;
}

/// <summary>
/// 批次追溯链
/// </summary>
public class LotTraceChain
{
    public string ChainId { get; set; } = string.Empty;
    public string ChildLotId { get; set; } = string.Empty;
    public string ParentLotId { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty; // Split/Merge/GradeSplit/Rework/AssembleToTest
    public int SplitQty { get; set; }
    public string MergeSourceLotIds { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
}

/// <summary>
/// 自动 Hold 规则
/// </summary>
public class AutoHoldRule
{
    public string RuleId { get; set; } = string.Empty;
    public string RuleName { get; set; } = string.Empty;
    public string RuleType { get; set; } = string.Empty; // Yield/Equipment/Quality/Step
    public string HoldScope { get; set; } = "Lot";
    public string? TriggerCondition { get; set; } // JSON
    public string HoldReasonCode { get; set; } = string.Empty;
    public string HoldReason { get; set; } = string.Empty;
    public bool AutoRelease { get; set; }
    public string? AutoReleaseCondition { get; set; } // JSON
    public bool IsActive { get; set; } = true;
    public int Priority { get; set; } = 5;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// ============================================================
// Assemble / Test 数据实体
// ============================================================

/// <summary>
/// 装配数据记录
/// </summary>
public class ProdAssembleData
{
    public string AssembleId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string? MfgId { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string EquipmentId { get; set; } = string.Empty;
    public string? RecipeId { get; set; }
    public string? CarrierId { get; set; }
    public int InputQty { get; set; }
    public int OutputQty { get; set; }
    public int ScrapQty { get; set; }
    public int ReworkQty { get; set; }
    // 打线工艺参数
    public int WireBondCount { get; set; }
    public decimal? BondForce { get; set; }
    public decimal? BondTemp { get; set; }
    public int? BondTime { get; set; }
    public decimal? UltrasonicPower { get; set; }
    // 塑封工艺参数
    public decimal? MoldPressure { get; set; }
    public decimal? MoldTemp { get; set; }
    public int? MoldTime { get; set; }
    public decimal? TransferPressure { get; set; }
    // 固化工艺参数
    public int? CureTime { get; set; }
    public decimal? CureTemp { get; set; }
    // 贴片工艺参数
    public decimal? DieAttachForce { get; set; }
    public decimal? DieAttachTemp { get; set; }
    public decimal? DieAttachOffsetX { get; set; }
    public decimal? DieAttachOffsetY { get; set; }
    // 操作员信息
    public string OperatorId { get; set; } = string.Empty;
    public string? OperatorName { get; set; }
    // 时间信息
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; } = "Completed";
    public string? Detail { get; set; } // JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 测试数据记录
/// </summary>
public class ProdTestData
{
    public string TestId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string? MfgId { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string TestProgram { get; set; } = string.Empty;
    public string? TestVersion { get; set; }
    public string EquipmentId { get; set; } = string.Empty;
    public string? HandlerId { get; set; }
    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public int ScrapQty { get; set; }
    public int RetestQty { get; set; }
    // 测试条件
    public decimal? TestTemp { get; set; }
    public decimal? TestVoltage { get; set; }
    public decimal? TestCurrent { get; set; }
    public decimal? TestFrequency { get; set; }
    // BIN 结果
    public string? BinSummary { get; set; }
    public int Bin1Qty { get; set; }
    public int Bin2Qty { get; set; }
    public int Bin3Qty { get; set; }
    public int Bin4Qty { get; set; }
    public int Bin5Qty { get; set; }
    public int Bin6Qty { get; set; }
    public int Bin7Qty { get; set; }
    public int Bin8Qty { get; set; }
    // 良率计算
    public decimal? YieldPercent { get; set; }
    public decimal? FirstPassYield { get; set; }
    public decimal? FinalYield { get; set; }
    public string? TestResult { get; set; }
    // 参数测试数据
    public string? ParametricData { get; set; } // JSON
    // 操作员信息
    public string OperatorId { get; set; } = string.Empty;
    public string? OperatorName { get; set; }
    // 时间信息
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; } = "Completed";
    public string? Detail { get; set; } // JSON
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
