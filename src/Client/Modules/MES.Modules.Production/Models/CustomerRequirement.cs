namespace MES.Modules.Production.Models;

public class CustomerRequirement
{
    public string RequirementId { get; set; } = Guid.NewGuid().ToString("N");
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? OrderId { get; set; }
    public string? ProductId { get; set; }
    public string RequirementType { get; set; } = string.Empty; // Traceability/Yield/LeadTime/Packaging/Testing/Documentation
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal"; // High/Normal/Low
    public bool IsMandatory { get; set; }
    public string? VerificationMethod { get; set; }
    public string Status { get; set; } = "Active"; // Active/Completed/Cancelled
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
