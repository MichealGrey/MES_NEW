namespace MES.Contracts.Production;

public class LotDto
{
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string DieName { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = "1.0";
    public int CurrentStepSeq { get; set; }
    public string CurrentStepCode { get; set; } = string.Empty;

    // Backward-compatibility alias for in-memory ProductionService
    public string CurrentStep
    {
        get => CurrentStepCode;
        set => CurrentStepCode = value;
    }

    public string? CurrentEquipment { get; set; }

    public string Status { get; set; } = "Waiting";
    public string ProcessStage { get; set; } = string.Empty;
    public int UnitCount { get; set; }
    public int StripCount { get; set; }
    public string Priority { get; set; } = "Normal";
    public int OriginalQty { get; set; }
    public int TotalPassQty { get; set; }
    public int TotalScrapQty { get; set; }
    public int TotalHoldQty { get; set; }
    public int QtyPass { get; set; }
    public int QtyFail { get; set; }
    public string? WaferLotId { get; set; }
    public string? CarrierType { get; set; }
    public string? CarrierId { get; set; }
    public string? Grade { get; set; }
    public string? BinResult { get; set; }
    public string? TestResult { get; set; }
    public bool IsReworkLot { get; set; }
    public bool IsUnderMrb { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateLotRequest
{
    public string OrderId { get; set; } = string.Empty;
    public int OriginalQty { get; set; }
    public string? WaferLotId { get; set; }
    public string Priority { get; set; } = "Normal";
}

public class LotStepDto
{
    public string RecordId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? TrackInEquipment { get; set; }
    public string? TrackInRecipe { get; set; }
    public DateTime? TrackInTime { get; set; }
    public string? TrackInOperator { get; set; }
    public DateTime? TrackOutTime { get; set; }
    public string? TrackOutOperator { get; set; }
    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public int ScrapQty { get; set; }
    public int ReworkQty { get; set; }
    public int HoldQty { get; set; }
    public int PendingQty { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 批次详情响应（包含工序流转历史）
/// </summary>
public class LotDetailResponse
{
    public LotDto Lot { get; set; } = new();
    public List<LotStepHistory> StepHistory { get; set; } = [];
    public List<LotTrackRecord> TrackRecords { get; set; } = [];
}

/// <summary>
/// 工序历史记录
/// </summary>
public class LotStepHistory
{
    public string RecordId { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? TrackInEquipment { get; set; }
    public string? TrackInCarrier { get; set; }
    public string? TrackInRecipe { get; set; }
    public DateTime? TrackInTime { get; set; }
    public string? TrackInOperator { get; set; }
    public DateTime? TrackOutTime { get; set; }
    public string? TrackOutOperator { get; set; }
    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public int ScrapQty { get; set; }
    public int ReworkQty { get; set; }
    public int HoldQty { get; set; }
    public int PendingQty { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 流转记录
/// </summary>
public class LotTrackRecord
{
    public string OperationId { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public string? StepCode { get; set; }
    public int? StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string? CarrierId { get; set; }
    public string? RecipeId { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public string? OperatorName { get; set; }
    public int? InputQty { get; set; }
    public int? OutputQty { get; set; }
    public int? ScrapQty { get; set; }
    public string? Detail { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 批次统计信息
/// </summary>
public class LotStatsResponse
{
    public int TotalLots { get; set; }
    public int WaitingLots { get; set; }
    public int InProductionLots { get; set; }
    public int HoldLots { get; set; }
    public int CompletedLots { get; set; }
    public int ScrapedLots { get; set; }
}
