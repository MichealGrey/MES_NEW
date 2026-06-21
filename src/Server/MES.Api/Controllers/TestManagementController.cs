using Microsoft.AspNetCore.Mvc;
using MES.Contracts.Common;
using MES.Contracts.Production;
using MES.Services.Production;

namespace MES.Api.Controllers;

/// <summary>
/// 测试管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestManagementController : ControllerBase
{
    private readonly ITestManagementService _service;
    private readonly ILogger<TestManagementController> _logger;

    public TestManagementController(
        ITestManagementService service,
        ILogger<TestManagementController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// 执行CP测试（晶圆探针测试）
    /// </summary>
    [HttpPost("cp")]
    public async Task<ApiResponse<TestExecutionResponse>> ExecuteCpTest([FromBody] CpTestRequest request)
    {
        // 输入验证
        if (string.IsNullOrWhiteSpace(request.LotId))
            return ApiResponse<TestExecutionResponse>.Fail("批次ID不能为空");

        if (string.IsNullOrWhiteSpace(request.WaferId))
            return ApiResponse<TestExecutionResponse>.Fail("晶圆ID不能为空");

        if (string.IsNullOrWhiteSpace(request.TestProgram))
            return ApiResponse<TestExecutionResponse>.Fail("测试程序不能为空");

        if (string.IsNullOrWhiteSpace(request.OperatorId))
            return ApiResponse<TestExecutionResponse>.Fail("操作员ID不能为空");

        if (request.TotalDice <= 0)
            return ApiResponse<TestExecutionResponse>.Fail("总晶粒数必须大于0");

        if (request.PassDice < 0 || request.FailDice < 0)
            return ApiResponse<TestExecutionResponse>.Fail("数量不能为负数");

        var result = await _service.ExecuteCpTestAsync(request);

        if (!result.Success)
            return ApiResponse<TestExecutionResponse>.Fail(result.Message);

        _logger.LogInformation("CP测试执行成功: {LotId} - {TestId}, 良率: {Yield:P2}",
            request.LotId, result.TestId, result.Yield);

        return ApiResponse<TestExecutionResponse>.Ok(result, "CP测试执行成功");
    }

    /// <summary>
    /// 执行FT测试（最终测试）
    /// </summary>
    [HttpPost("ft")]
    public async Task<ApiResponse<TestExecutionResponse>> ExecuteFtTest([FromBody] FtTestRequest request)
    {
        // 输入验证
        if (string.IsNullOrWhiteSpace(request.LotId))
            return ApiResponse<TestExecutionResponse>.Fail("批次ID不能为空");

        if (string.IsNullOrWhiteSpace(request.TestProgram))
            return ApiResponse<TestExecutionResponse>.Fail("测试程序不能为空");

        if (string.IsNullOrWhiteSpace(request.OperatorId))
            return ApiResponse<TestExecutionResponse>.Fail("操作员ID不能为空");

        if (request.TotalQty <= 0)
            return ApiResponse<TestExecutionResponse>.Fail("总数量必须大于0");

        if (request.PassQty < 0 || request.FailQty < 0)
            return ApiResponse<TestExecutionResponse>.Fail("数量不能为负数");

        var result = await _service.ExecuteFtTestAsync(request);

        if (!result.Success)
            return ApiResponse<TestExecutionResponse>.Fail(result.Message);

        _logger.LogInformation("FT测试执行成功: {LotId} - {TestId}, 良率: {Yield:P2}",
            request.LotId, result.TestId, result.Yield);

        return ApiResponse<TestExecutionResponse>.Ok(result, "FT测试执行成功");
    }

    /// <summary>
    /// 获取批次测试结果
    /// </summary>
    [HttpGet("{lotId}/results")]
    public async Task<ApiResponse<List<TestResultResponse>>> GetTestResults(
        string lotId,
        [FromQuery] string? testType = null)
    {
        if (string.IsNullOrWhiteSpace(lotId))
            return ApiResponse<List<TestResultResponse>>.Fail("批次ID不能为空");

        var results = await _service.GetTestResultsAsync(lotId, testType);

        return ApiResponse<List<TestResultResponse>>.Ok(results);
    }

    /// <summary>
    /// 获取批次良率统计
    /// </summary>
    [HttpGet("{lotId}/yield")]
    public async Task<ApiResponse<YieldStatisticsResponse>> GetYieldStatistics(string lotId)
    {
        if (string.IsNullOrWhiteSpace(lotId))
            return ApiResponse<YieldStatisticsResponse>.Fail("批次ID不能为空");

        var result = await _service.GetYieldStatisticsAsync(lotId);

        return ApiResponse<YieldStatisticsResponse>.Ok(result);
    }

    /// <summary>
    /// 获取产品良率趋势
    /// </summary>
    [HttpGet("yield-trend")]
    public async Task<ApiResponse<List<YieldTrendItem>>> GetYieldTrend(
        [FromQuery] string productCode,
        [FromQuery] int days = 30)
    {
        if (string.IsNullOrWhiteSpace(productCode))
            return ApiResponse<List<YieldTrendItem>>.Fail("产品代码不能为空");

        if (days <= 0 || days > 365)
            return ApiResponse<List<YieldTrendItem>>.Fail("天数必须在1-365之间");

        var result = await _service.GetYieldTrendAsync(productCode, days);

        return ApiResponse<List<YieldTrendItem>>.Ok(result);
    }
}
