namespace MES.Modules.Production.Models;

/// <summary>
/// 校验结果
/// </summary>
public class TrackValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public bool HasWarnings => Warnings.Count > 0;
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];

    public void AddError(string error) => Errors.Add(error);
    public void AddWarning(string warning) => Warnings.Add(warning);

    public static TrackValidationResult Pass() => new();

    public static TrackValidationResult Fail(params string[] errors)
    {
        var result = new TrackValidationResult();
        result.Errors.AddRange(errors);
        return result;
    }
}

/// <summary>
/// 过站结果
/// </summary>
public class TrackResult
{
    public bool Success { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? NextStepName { get; set; }
    public DateTime? TrackTime { get; set; }
    public string? RecordId { get; set; }
    public List<string> Warnings { get; set; } = [];
}
