using MES.Contracts.Production;

namespace MES.Services.Production;

/// <summary>
/// 工序执行服务接口
/// </summary>
public interface IProcessExecutionService
{
    /// <summary>
    /// 开始工序（记录进站时间、设备、操作员）
    /// </summary>
    Task<StepExecutionResponse> StartStepAsync(StepStartRequest request);

    /// <summary>
    /// 完成工序（记录出站时间、数量、参数）
    /// </summary>
    Task<StepExecutionResponse> CompleteStepAsync(StepCompleteRequest request);

    /// <summary>
    /// 记录工艺参数
    /// </summary>
    Task<StepExecutionResponse> RecordParametersAsync(string lotId, string stepCode, List<StepParameterRecord> parameters);

    /// <summary>
    /// 查询工序状态
    /// </summary>
    Task<StepStatusResponse> GetStepStatusAsync(string lotId, string stepCode);

    /// <summary>
    /// 获取当前工序
    /// </summary>
    Task<StepStatusResponse?> GetCurrentStepAsync(string lotId);
}
