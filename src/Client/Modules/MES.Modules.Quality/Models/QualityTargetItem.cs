namespace MES.Modules.Quality.Models;

public class QualityTargetItem
{
    public string Id { get; set; } = string.Empty;
    public string TargetName { get; set; } = string.Empty;
    public double CurrentValue { get; set; }
    public double TargetValue { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Trend { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double AchievementRate => TargetValue > 0 ? (CurrentValue / TargetValue) * 100 : 0;
}
