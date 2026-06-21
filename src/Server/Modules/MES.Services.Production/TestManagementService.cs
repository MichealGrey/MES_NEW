using MES.Contracts.Production;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace MES.Services.Production;

/// <summary>
/// 测试管理服务实现
/// </summary>
public class TestManagementService : ITestManagementService
{
    private readonly MesDbContext _context;
    private readonly ILogger<TestManagementService> _logger;

    public TestManagementService(MesDbContext context, ILogger<TestManagementService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// 执行CP测试（晶圆探针测试）
    /// </summary>
    public async Task<TestExecutionResponse> ExecuteCpTestAsync(CpTestRequest request)
    {
        var now = DateTime.UtcNow;
        var testId = $"CP-{now:yyyyMMddHHmmss}-{request.LotId[^4..]}";

        try
        {
            // 1. 验证批次是否存在
            var lot = await _context.ProdLots.FindAsync(request.LotId);
            if (lot == null)
            {
                return Fail(testId, $"批次 {request.LotId} 不存在");
            }

            if (lot.Status == "Completed" || lot.Status == "Scrapped" || lot.Status == "Archived")
            {
                return Fail(testId, $"批次 {request.LotId} 状态为 {lot.Status}，不允许执行测试");
            }

            // 2. 验证数量
            if (request.TotalDice <= 0)
            {
                return Fail(testId, "总晶粒数必须大于0");
            }

            if (request.PassDice + request.FailDice != request.TotalDice)
            {
                return Fail(testId,
                    $"数量不平衡: 总晶粒 {request.TotalDice}，通过 {request.PassDice} + 失败 {request.FailDice}");
            }

            // 3. 计算良率
            var yield = request.TotalDice > 0
                ? Math.Round((decimal)request.PassDice / request.TotalDice, 4)
                : 0m;

            // 4. 计算Bin分布百分比
            foreach (var bin in request.BinDistribution)
            {
                bin.Percentage = request.TotalDice > 0
                    ? Math.Round((decimal)bin.Count / request.TotalDice, 4)
                    : 0m;
            }

            // 5. 记录测试执行到 ProdOperationHistory
            var operationHistory = new ProdOperationHistory
            {
                OperationId = testId,
                LotId = request.LotId,
                OrderId = lot.OrderId,
                OperationType = "CPTest",
                StepCode = "CP",
                OperatorId = request.OperatorId,
                InputQty = request.TotalDice,
                OutputQty = request.PassDice,
                ScrapQty = request.FailDice,
                Detail = JsonSerializer.Serialize(new
                {
                    TestType = "CP",
                    WaferId = request.WaferId,
                    TestProgram = request.TestProgram,
                    TotalDice = request.TotalDice,
                    PassDice = request.PassDice,
                    FailDice = request.FailDice,
                    Yield = yield,
                    BinDistribution = request.BinDistribution
                }),
                CreatedAt = now
            };

            _context.ProdOperationHistories.Add(operationHistory);

            // 6. 更新批次累计数量
            lot.TotalPassQty += request.PassDice;
            lot.TotalScrapQty += request.FailDice;
            lot.QtyPass = request.PassDice;
            lot.QtyFail = request.FailDice;
            lot.UpdatedAt = now;

            // 7. 保存更改
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "批次 {LotId} 执行CP测试成功: 测试ID={TestId}, 总晶粒={TotalDice}, 通过={PassDice}, 良率={Yield:P2}",
                request.LotId, testId, request.TotalDice, request.PassDice, yield);

            return new TestExecutionResponse
            {
                Success = true,
                TestId = testId,
                Message = $"CP测试执行成功，良率: {yield:P2}",
                Yield = yield
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CP测试执行失败: {LotId}", request.LotId);
            return Fail(testId, $"CP测试执行失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 执行FT测试（最终测试）
    /// </summary>
    public async Task<TestExecutionResponse> ExecuteFtTestAsync(FtTestRequest request)
    {
        var now = DateTime.UtcNow;
        var testId = $"FT-{now:yyyyMMddHHmmss}-{request.LotId[^4..]}";

        try
        {
            // 1. 验证批次是否存在
            var lot = await _context.ProdLots.FindAsync(request.LotId);
            if (lot == null)
            {
                return Fail(testId, $"批次 {request.LotId} 不存在");
            }

            if (lot.Status == "Completed" || lot.Status == "Scrapped" || lot.Status == "Archived")
            {
                return Fail(testId, $"批次 {request.LotId} 状态为 {lot.Status}，不允许执行测试");
            }

            // 2. 验证数量
            if (request.TotalQty <= 0)
            {
                return Fail(testId, "总数量必须大于0");
            }

            if (request.PassQty + request.FailQty != request.TotalQty)
            {
                return Fail(testId,
                    $"数量不平衡: 总数量 {request.TotalQty}，通过 {request.PassQty} + 失败 {request.FailQty}");
            }

            // 3. 计算良率
            var yield = request.TotalQty > 0
                ? Math.Round((decimal)request.PassQty / request.TotalQty, 4)
                : 0m;

            // 4. 计算Bin分布百分比
            foreach (var bin in request.BinDistribution)
            {
                bin.Percentage = request.TotalQty > 0
                    ? Math.Round((decimal)bin.Count / request.TotalQty, 4)
                    : 0m;
            }

            // 5. 记录测试执行到 ProdOperationHistory
            var operationHistory = new ProdOperationHistory
            {
                OperationId = testId,
                LotId = request.LotId,
                OrderId = lot.OrderId,
                OperationType = "FTTest",
                StepCode = "FT",
                OperatorId = request.OperatorId,
                InputQty = request.TotalQty,
                OutputQty = request.PassQty,
                ScrapQty = request.FailQty,
                Detail = JsonSerializer.Serialize(new
                {
                    TestType = "FT",
                    TestProgram = request.TestProgram,
                    TotalQty = request.TotalQty,
                    PassQty = request.PassQty,
                    FailQty = request.FailQty,
                    Yield = yield,
                    BinDistribution = request.BinDistribution
                }),
                CreatedAt = now
            };

            _context.ProdOperationHistories.Add(operationHistory);

            // 6. 更新批次累计数量
            lot.TotalPassQty += request.PassQty;
            lot.TotalScrapQty += request.FailQty;
            lot.QtyPass = request.PassQty;
            lot.QtyFail = request.FailQty;
            lot.UpdatedAt = now;

            // 7. 保存更改
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "批次 {LotId} 执行FT测试成功: 测试ID={TestId}, 总数量={TotalQty}, 通过={PassQty}, 良率={Yield:P2}",
                request.LotId, testId, request.TotalQty, request.PassQty, yield);

            return new TestExecutionResponse
            {
                Success = true,
                TestId = testId,
                Message = $"FT测试执行成功，良率: {yield:P2}",
                Yield = yield
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FT测试执行失败: {LotId}", request.LotId);
            return Fail(testId, $"FT测试执行失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 获取批次测试结果
    /// </summary>
    public async Task<List<TestResultResponse>> GetTestResultsAsync(string lotId, string? testType = null)
    {
        var query = _context.ProdOperationHistories
            .Where(x => x.LotId == lotId && (x.OperationType == "CPTest" || x.OperationType == "FTTest"));

        if (!string.IsNullOrEmpty(testType))
        {
            var opType = testType.ToUpper() == "CP" ? "CPTest" : "FTTest";
            query = query.Where(x => x.OperationType == opType);
        }

        var histories = await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var results = new List<TestResultResponse>();

        foreach (var history in histories)
        {
            var testResult = new TestResultResponse
            {
                TestId = history.OperationId,
                LotId = history.LotId,
                TestType = history.OperationType == "CPTest" ? "CP" : "FT",
                TotalQty = history.InputQty ?? 0,
                PassQty = history.OutputQty ?? 0,
                FailQty = history.ScrapQty ?? 0,
                TestTime = history.CreatedAt,
                TestProgram = null,
                WaferId = null
            };

            // 计算良率
            testResult.Yield = testResult.TotalQty > 0
                ? Math.Round((decimal)testResult.PassQty / testResult.TotalQty, 4)
                : 0m;

            // 解析详情中的Bin分布和其他信息
            if (!string.IsNullOrEmpty(history.Detail))
            {
                try
                {
                    using var doc = JsonDocument.Parse(history.Detail);
                    var root = doc.RootElement;

                    // 提取测试程序
                    if (root.TryGetProperty("TestProgram", out var testProgram))
                    {
                        testResult.TestProgram = testProgram.GetString();
                    }

                    // 提取晶圆ID (仅CP测试)
                    if (root.TryGetProperty("WaferId", out var waferId))
                    {
                        testResult.WaferId = waferId.GetString();
                    }

                    // 提取Bin分布
                    if (root.TryGetProperty("BinDistribution", out var binArray) && binArray.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var binElement in binArray.EnumerateArray())
                        {
                            var binItem = new BinDistributionItem();
                            if (binElement.TryGetProperty("BinCode", out var binCode))
                                binItem.BinCode = binCode.GetString() ?? string.Empty;
                            if (binElement.TryGetProperty("BinName", out var binName))
                                binItem.BinName = binName.GetString() ?? string.Empty;
                            if (binElement.TryGetProperty("Count", out var count))
                                binItem.Count = count.GetInt32();
                            if (binElement.TryGetProperty("Percentage", out var percentage))
                                binItem.Percentage = percentage.GetDecimal();

                            testResult.BinDistribution.Add(binItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "解析测试详情JSON失败: {OperationId}", history.OperationId);
                }
            }

            results.Add(testResult);
        }

        return results;
    }

    /// <summary>
    /// 获取良率统计
    /// </summary>
    public async Task<YieldStatisticsResponse> GetYieldStatisticsAsync(string lotId)
    {
        // 获取所有CP测试记录
        var cpHistories = await _context.ProdOperationHistories
            .Where(x => x.LotId == lotId && x.OperationType == "CPTest")
            .ToListAsync();

        // 获取所有FT测试记录
        var ftHistories = await _context.ProdOperationHistories
            .Where(x => x.LotId == lotId && x.OperationType == "FTTest")
            .ToListAsync();

        // 计算CP良率
        var cpTotalQty = cpHistories.Sum(x => x.InputQty ?? 0);
        var cpPassQty = cpHistories.Sum(x => x.OutputQty ?? 0);
        var cpYield = cpTotalQty > 0 ? Math.Round((decimal)cpPassQty / cpTotalQty, 4) : 0m;

        // 计算FT良率
        var ftTotalQty = ftHistories.Sum(x => x.InputQty ?? 0);
        var ftPassQty = ftHistories.Sum(x => x.OutputQty ?? 0);
        var ftYield = ftTotalQty > 0 ? Math.Round((decimal)ftPassQty / ftTotalQty, 4) : 0m;

        // 综合良率 = CP良率 × FT良率
        var overallYield = Math.Round(cpYield * ftYield, 4);

        // 获取目标良率（从工单）
        var lot = await _context.ProdLots.FindAsync(lotId);
        var targetYield = 0m;

        if (lot != null)
        {
            var workOrder = await _context.ProdWorkOrders.FindAsync(lot.OrderId);
            if (workOrder != null)
            {
                // 如果有CP和FT目标良率，取综合目标
                if (workOrder.TargetCpYield.HasValue && workOrder.TargetFtYield.HasValue)
                {
                    targetYield = Math.Round((workOrder.TargetCpYield.Value / 100m) * (workOrder.TargetFtYield.Value / 100m), 4);
                }
                else if (workOrder.TargetCpYield.HasValue)
                {
                    targetYield = workOrder.TargetCpYield.Value / 100m;
                }
                else if (workOrder.TargetFtYield.HasValue)
                {
                    targetYield = workOrder.TargetFtYield.Value / 100m;
                }
            }
        }

        return new YieldStatisticsResponse
        {
            CpYield = cpYield,
            FtYield = ftYield,
            OverallYield = overallYield,
            TargetYield = targetYield,
            CpTotalQty = cpTotalQty,
            CpPassQty = cpPassQty,
            FtTotalQty = ftTotalQty,
            FtPassQty = ftPassQty,
            CpTestCount = cpHistories.Count,
            FtTestCount = ftHistories.Count
        };
    }

    /// <summary>
    /// 获取良率趋势
    /// </summary>
    public async Task<List<YieldTrendItem>> GetYieldTrendAsync(string productCode, int days = 30)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);

        // 获取指定产品在指定日期范围内的所有批次
        var lots = await _context.ProdLots
            .Where(x => x.ProductId == productCode && x.CreatedAt >= startDate)
            .ToListAsync();

        if (lots.Count == 0)
        {
            return new List<YieldTrendItem>();
        }

        var lotIds = lots.Select(x => x.LotId).ToList();

        // 获取这些批次的所有测试记录
        var cpHistories = await _context.ProdOperationHistories
            .Where(x => lotIds.Contains(x.LotId) && x.OperationType == "CPTest")
            .ToListAsync();

        var ftHistories = await _context.ProdOperationHistories
            .Where(x => lotIds.Contains(x.LotId) && x.OperationType == "FTTest")
            .ToListAsync();

        // 构建批次映射
        var lotMap = lots.ToDictionary(x => x.LotId, x => x);

        // 按日期分组计算良率趋势
        var dailyData = new Dictionary<DateTime, (int cpTotal, int cpPass, int ftTotal, int ftPass, int lotCount)>();

        foreach (var lot in lots)
        {
            var date = lot.CreatedAt.Date;
            if (!dailyData.ContainsKey(date))
            {
                dailyData[date] = (0, 0, 0, 0, 0);
            }

            var existing = dailyData[date];
            existing.lotCount++;

            // 累加CP测试数据
            var lotCpHistories = cpHistories.Where(x => x.LotId == lot.LotId).ToList();
            existing.cpTotal += lotCpHistories.Sum(x => x.InputQty ?? 0);
            existing.cpPass += lotCpHistories.Sum(x => x.OutputQty ?? 0);

            // 累加FT测试数据
            var lotFtHistories = ftHistories.Where(x => x.LotId == lot.LotId).ToList();
            existing.ftTotal += lotFtHistories.Sum(x => x.InputQty ?? 0);
            existing.ftPass += lotFtHistories.Sum(x => x.OutputQty ?? 0);

            dailyData[date] = existing;
        }

        var result = dailyData
            .OrderBy(x => x.Key)
            .Select(x =>
            {
                var data = x.Value;
                var cpYield = data.cpTotal > 0 ? Math.Round((decimal)data.cpPass / data.cpTotal, 4) : 0m;
                var ftYield = data.ftTotal > 0 ? Math.Round((decimal)data.ftPass / data.ftTotal, 4) : 0m;
                var overallYield = Math.Round(cpYield * ftYield, 4);

                return new YieldTrendItem
                {
                    Date = x.Key,
                    ProductCode = productCode,
                    CpYield = cpYield,
                    FtYield = ftYield,
                    OverallYield = overallYield,
                    LotCount = data.lotCount
                };
            })
            .ToList();

        return result;
    }

    private static TestExecutionResponse Fail(string testId, string message)
    {
        return new TestExecutionResponse
        {
            Success = false,
            TestId = testId,
            Message = message
        };
    }
}
