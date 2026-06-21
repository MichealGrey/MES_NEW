namespace MES.Modules.Quality.Models;

public class FmeaItem
{
    public string Id { get; set; } = string.Empty;
    public string ProcessStep { get; set; } = string.Empty;
    public string FailureMode { get; set; } = string.Empty;
    public string FailureEffect { get; set; } = string.Empty;
    public int Severity { get; set; }
    public int Occurrence { get; set; }
    public int Detection { get; set; }
    public int RPN => Severity * Occurrence * Detection;
    public string ControlMeasure { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public string ImprovementAction { get; set; } = string.Empty;
}
