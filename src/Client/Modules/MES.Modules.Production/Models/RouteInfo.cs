namespace MES.Modules.Production.Models;

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
