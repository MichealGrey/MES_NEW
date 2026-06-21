using MES.Contracts.Production;

namespace MES.Services.Production;

/// <summary>
/// 测试管理服务接口
/// </summary>
public interface ITestManagementService
{
    /// <summary>
    /// 执行CP测试（晶圆探针测试）
    /// </summary>
    Task<TestExecutionResponse> ExecuteCpTestAsync(CpTestRequest request);

    /// <summary>
    /// 执行FT测试（最终测试）
    /// </summary>
    Task<TestExecutionResponse> ExecuteFtTestAsync(FtTestRequest request);

    /// <summary>
    /// 获取批次测试结果
    /// </summary>
    Task<List<TestResultResponse>> GetTestResultsAsync(string lotId, string? testType = null);

    /// <summary>
    /// 获取良率统计
    /// </summary>
    Task<YieldStatisticsResponse> GetYieldStatisticsAsync(string lotId);

    /// <summary>
    /// 获取良率趋势
    /// </summary>
    Task<List<YieldTrendItem>> GetYieldTrendAsync(string productCode, int days = 30);
}
