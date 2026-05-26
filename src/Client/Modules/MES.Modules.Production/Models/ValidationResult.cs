namespace MES.Modules.Production.Models;

public enum TrackOperationMode
{
    TrackIn,
    TrackOut
}

public enum ValidationStatus
{
    Pass,
    Warning,
    Fail
}

public class ValidationResult
{
    public string CheckItem { get; set; } = string.Empty;
    public ValidationStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;

    public string StatusText => Status switch
    {
        ValidationStatus.Pass => "通过",
        ValidationStatus.Warning => "警告",
        ValidationStatus.Fail => "失败",
        _ => "未知"
    };

    public string StatusColor => Status switch
    {
        ValidationStatus.Pass => "#4CC56C",
        ValidationStatus.Warning => "#F39C12",
        ValidationStatus.Fail => "#E74C3C",
        _ => "#B0B0C0"
    };
}
