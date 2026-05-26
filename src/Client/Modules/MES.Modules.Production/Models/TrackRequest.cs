namespace MES.Modules.Production.Models;

/// <summary>
/// 进站请求
/// </summary>
public class TrackInRequest
{
    public string LotId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string? RouteVersion { get; set; } = "1.0";
    public int StepSeq { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string? EquipmentGroup { get; set; }
    public string? RecipeId { get; set; }
    public string? CarrierId { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string Workstation { get; set; } = string.Empty;
    public int InputQty { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 出站请求
/// </summary>
public class TrackOutRequest
{
    public string LotId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string? RouteVersion { get; set; } = "1.0";
    public int StepSeq { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string? RecipeId { get; set; }
    public string? CarrierId { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string Workstation { get; set; } = string.Empty;
    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public int ScrapQty { get; set; }
    public int ReworkQty { get; set; }
    public int HoldQty { get; set; }
    public int PendingQty { get; set; }
    public string? Remark { get; set; }
}
