namespace MES.Adapters.Mock;

public class MockConfig
{
    public int DelayMs { get; set; } = 200;
    public double FailureRate { get; set; } = 0.0;
    public string? DataFile { get; set; }
}
