namespace MES.Services.Production.Models;

public class RouteInfo
{
    public string RouteId { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = "1.0";
    public string ProductId { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsApproved { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public DateTime? ApprovedAt { get; set; }
    public List<RouteStep> Steps { get; set; } = [];
}

public class RouteStep
{
    public string RouteId { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = "1.0";
    public int StepSeq { get; set; }
    public string StepName { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;

    public string EquipmentGroup { get; set; } = string.Empty;
    public string RequiredRecipeType { get; set; } = string.Empty;

    public bool RequireTrackIn { get; set; } = true;
    public bool RequireTrackOut { get; set; } = true;
    public bool RequireRecipeCheck { get; set; } = true;
    public bool RequireEquipmentCheck { get; set; } = true;
    public bool RequireMaterialCheck { get; set; }
    public bool RequireQualityGate { get; set; }
    public bool RequireQuantityBalance { get; set; } = true;

    public bool AllowSkip { get; set; }
    public bool AllowReworkToThisStep { get; set; }

    public int QueueTimeLimitMinutes { get; set; }
}
