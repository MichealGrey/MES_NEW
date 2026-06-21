namespace MES.Modules.Production.Models;

/// <summary>
/// Product information for the product management view.
/// </summary>
public class ProductInfo
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string PackageType { get; set; } = string.Empty;
    public string ProcessStage { get; set; } = "Assemble";
    public string? DefaultRouteId { get; set; }
    public int UnitQty { get; set; } = 1;
    public string Status { get; set; } = "Active";
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Customer information for the customer management view.
/// </summary>
public class CustomerInfo
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? CustomerPnPrefix { get; set; }
    public string QualityLevel { get; set; } = "Industrial";
    public string? SpecialRequirements { get; set; }
    public string? DefaultPackingSpec { get; set; }
    public string? DefaultOqcSpec { get; set; }
    public string Status { get; set; } = "Active";
}

/// <summary>
/// Reason code information for the reason code management view.
/// </summary>
public class ReasonCodeInfo
{
    public string ReasonCodeId { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string ReasonText { get; set; } = string.Empty;
    public string ApplicableTo { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// Defect code information for the defect code management view.
/// </summary>
public class DefectCodeInfo
{
    public string DefectCodeId { get; set; } = string.Empty;
    public string DefectCategory { get; set; } = string.Empty;
    public string DefectText { get; set; } = string.Empty;
    public string Severity { get; set; } = "Major";
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// Material information for the material management view.
/// </summary>
public class MaterialInfo
{
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public string MaterialCode { get; set; } = string.Empty;
    public string MaterialType { get; set; } = string.Empty;
    public string? Supplier { get; set; }
    public string Unit { get; set; } = "pcs";
    public double MinStock { get; set; }
    public string Status { get; set; } = "Active";
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Scrap rule information for the scrap rule management view.
/// </summary>
public class ScrapRule
{
    public string RuleId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int MaxScrapQty { get; set; }
    public double MaxScrapPercent { get; set; }
    public string ApprovalLevel { get; set; } = string.Empty;
}
