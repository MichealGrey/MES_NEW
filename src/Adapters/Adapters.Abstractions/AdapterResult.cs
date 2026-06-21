namespace MES.Adapters.Abstractions;

/// <summary>
/// 统一适配器返回结构
/// </summary>
public class AdapterResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? SourceSystem { get; set; }

    public static AdapterResult<T> Ok(T data, string? sourceSystem = null) => new()
    {
        Success = true,
        Data = data,
        SourceSystem = sourceSystem,
        Timestamp = DateTime.UtcNow
    };

    public static AdapterResult<T> Fail(string errorCode, string errorMessage, string? sourceSystem = null) => new()
    {
        Success = false,
        ErrorCode = errorCode,
        ErrorMessage = errorMessage,
        SourceSystem = sourceSystem,
        Timestamp = DateTime.UtcNow
    };
}

/// <summary>
/// 适配器统一异常
/// </summary>
public class AdapterException : Exception
{
    public string ErrorCode { get; }
    public string SourceSystem { get; }

    public AdapterException(string errorCode, string message, string sourceSystem = "Unknown") : base(message)
    {
        ErrorCode = errorCode;
        SourceSystem = sourceSystem;
    }
}

/// <summary>
/// 适配器健康状态
/// </summary>
public class HealthStatus
{
    public bool IsHealthy { get; set; }
    public string? Message { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public double? ResponseTimeMs { get; set; }
}

/// <summary>
/// 适配器信息
/// </summary>
public class AdapterInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public string Provider { get; set; } = string.Empty;
}

/// <summary>
/// 基础适配器接口
/// </summary>
public interface IMesAdapter
{
    /// <summary>
    /// 健康检查：验证外部系统连通性
    /// </summary>
    Task<AdapterResult<HealthStatus>> HealthCheckAsync();

    /// <summary>
    /// 获取适配器名称和版本
    /// </summary>
    AdapterInfo GetAdapterInfo();
}
