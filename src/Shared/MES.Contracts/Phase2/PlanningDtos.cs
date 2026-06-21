namespace MES.Contracts.Phase2;

// ==================== 主生产计划 DTOs ====================

public class CreateMppRequest
{
    public string PlanName { get; set; } = string.Empty;
    public string PlanType { get; set; } = "MPS";
    public DateTime PlanPeriodStart { get; set; }
    public DateTime PlanPeriodEnd { get; set; }
    public string? Planner { get; set; }
    public string? Remark { get; set; }
}

public class MppResponse
{
    public string PlanId { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string PlanType { get; set; } = "MPS";
    public DateTime PlanPeriodStart { get; set; }
    public DateTime PlanPeriodEnd { get; set; }
    public string Status { get; set; } = "Draft";
    public int? TotalDemandQty { get; set; }
    public int? TotalCapacity { get; set; }
    public decimal? CapacityUtilization { get; set; }
    public bool BottleneckIdentified { get; set; }
    public string? BottleneckDescription { get; set; }
    public string? Planner { get; set; }
    public string? Remark { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class MppQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CapacityLoadResponse
{
    public long LoadId { get; set; }
    public string PlanId { get; set; } = string.Empty;
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string? EquipmentGroup { get; set; }
    public decimal? Uph { get; set; }
    public decimal? AvailableHours { get; set; }
    public decimal? RequiredHours { get; set; }
    public decimal? LoadRate { get; set; }
    public bool IsBottleneck { get; set; }
    public int? AvailableQty { get; set; }
    public int? RequiredQty { get; set; }
    public int? ShortageQty { get; set; }
    public string? ShiftPlan { get; set; }
    public DateTime CalculatedAt { get; set; }
}

public class CapacitySimulationRequest
{
    public string SimulationName { get; set; } = string.Empty;
    public string? BasePlanId { get; set; }
    public string? ScenarioDescription { get; set; }
    public string? ScenarioParams { get; set; }
}

public class CapacitySimulationResponse
{
    public string SimulationId { get; set; } = string.Empty;
    public string SimulationName { get; set; } = string.Empty;
    public string? BasePlanId { get; set; }
    public string? ScenarioDescription { get; set; }
    public int? TotalDemandQty { get; set; }
    public int? TotalCapacity { get; set; }
    public decimal? CapacityUtilization { get; set; }
    public int? BottleneckCount { get; set; }
    public string? ResultSummary { get; set; }
    public string Status { get; set; } = "Completed";
    public DateTime CreatedAt { get; set; }
}
