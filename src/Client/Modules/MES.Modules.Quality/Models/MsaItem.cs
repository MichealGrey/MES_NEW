namespace MES.Modules.Quality.Models;

public class MsaItem
{
    public string Id { get; set; } = string.Empty;
    public string GaugeName { get; set; } = string.Empty;
    public string Parameter { get; set; } = string.Empty;
    public int OperatorCount { get; set; }
    public int PartCount { get; set; }
    public int TrialCount { get; set; }
    public double GRR { get; set; }
    public string Status { get; set; } = string.Empty;
    public int NDC { get; set; }
    public DateTime LastCalibration { get; set; }
}
