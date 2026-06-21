namespace MES.Contracts.Quality;

public class SpcMeasurementDto
{
    public long Id { get; set; }
    public string? LotId { get; set; }
    public string? StepCode { get; set; }
    public string? ParameterName { get; set; }
    public decimal? MeasuredValue { get; set; }
    public decimal? Usl { get; set; }
    public decimal? Lsl { get; set; }
    public decimal? TargetValue { get; set; }
    public string? EquipmentId { get; set; }
    public string? OperatorId { get; set; }
    public DateTime? MeasuredAt { get; set; }
    public bool IsOutOfControl { get; set; }
}

public class CreateSpcMeasurementRequest
{
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public decimal MeasuredValue { get; set; }
    public decimal? Usl { get; set; }
    public decimal? Lsl { get; set; }
    public decimal? TargetValue { get; set; }
    public string? EquipmentId { get; set; }
}

public class SpcStatisticsDto
{
    public string ParameterName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Average { get; set; }
    public decimal Min { get; set; }
    public decimal Max { get; set; }
    public decimal StdDev { get; set; }
    public decimal Cpk { get; set; }
    public int OutOfControlCount { get; set; }
}
