namespace MES.Contracts.Quality;

public class SpcDataDto
{
    public string EquipmentId { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
    public string? LotId { get; set; }
}
