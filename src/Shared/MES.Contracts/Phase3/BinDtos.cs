namespace MES.Contracts.Phase3;

// ==================== Bin 分选管控 DTOs ====================

public class CreateBinDefinitionRequest
{
    public string BinCode { get; set; } = string.Empty;
    public string BinName { get; set; } = string.Empty;
    public string BinCategory { get; set; } = "Good"; // Good, Fail, Skip
    public int BinNo { get; set; }
    public string? Description { get; set; }
    public string Color { get; set; } = "#000000";
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ProductId { get; set; }
    public string? ProcessCode { get; set; }
    public string? TestType { get; set; } // CP, FT
    public int SortOrder { get; set; }
}

public class BinDefinitionResponse
{
    public string BinId { get; set; } = string.Empty;
    public string BinCode { get; set; } = string.Empty;
    public string BinName { get; set; } = string.Empty;
    public string BinCategory { get; set; } = string.Empty;
    public int BinNo { get; set; }
    public string? Description { get; set; }
    public string Color { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public string? ProductId { get; set; }
    public string? ProcessCode { get; set; }
    public string? TestType { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BinQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? BinCategory { get; set; }
    public string? ProcessCode { get; set; }
    public string? TestType { get; set; }
    public bool? IsActive { get; set; }
}

public class BinSortRecordResponse
{
    public string RecordId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string? TestProgram { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public string BinName { get; set; } = string.Empty;
    public string BinCategory { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal? YieldContribution { get; set; }
    public string? TestResult { get; set; }
    public DateTime SortTime { get; set; }
}

public class BinStatisticsResponse
{
    public long StatId { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public string BinName { get; set; } = string.Empty;
    public string BinCategory { get; set; } = string.Empty;
    public int TotalQty { get; set; }
    public decimal? Percentage { get; set; }
    public int InputQty { get; set; }
    public decimal? CumulativeYield { get; set; }
    public DateTime StatPeriod { get; set; }
}

public class BinSummaryQuery
{
    public string LotId { get; set; } = string.Empty;
    public string? StepCode { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
