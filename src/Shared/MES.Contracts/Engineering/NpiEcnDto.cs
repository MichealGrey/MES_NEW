namespace MES.Contracts.Engineering;

public class NpiProjectDto
{
    public string ProjectId { get; set; } = string.Empty;
    public string? ProjectName { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? Status { get; set; }
    public string? Phase { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? TargetCompletion { get; set; }
    public DateTime? ActualCompletion { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateNpiProjectRequest
{
    public string ProjectName { get; set; } = string.Empty;
    public string? CustomerId { get; set; }
    public string? ProductId { get; set; }
    public string? Phase { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? TargetCompletion { get; set; }
}

public class UpdateNpiProjectRequest
{
    public string ProjectId { get; set; } = string.Empty;
    public string? Status { get; set; }
    public string? Phase { get; set; }
    public DateTime? ActualCompletion { get; set; }
}
