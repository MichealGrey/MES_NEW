namespace MES.Contracts.EHS;

public class EnvironmentDataDto
{
    public string Area { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public int ParticleCount { get; set; }
    public DateTime Timestamp { get; set; }
}
