namespace MES.Modules.Production.Models;

public class SignatureRecord
{
    public string SignatureId { get; set; } = Guid.NewGuid().ToString("N");
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Level { get; set; } = "Level1";
    public string SignerId { get; set; } = string.Empty;
    public string SignerName { get; set; } = string.Empty;
    public string SignerRole { get; set; } = string.Empty;
    public DateTime SignTime { get; set; } = DateTime.UtcNow;
    public string? SecondSignerId { get; set; }
    public string? SecondSignerName { get; set; }
    public DateTime? SecondSignTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public bool IsDualApproved => !string.IsNullOrEmpty(SecondSignerId);
}
