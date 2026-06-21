using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MES.Contracts.Common;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Api.Controllers;

/// <summary>
/// 良率分析 API 控制器 - 提供良率 KPI、趋势、晶圆图、产品分析、BIN 分析和缺陷分析。
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class YieldController : ControllerBase
{
    private readonly MesDbContext _dbContext;
    private readonly ILogger<YieldController> _logger;

    public YieldController(MesDbContext dbContext, ILogger<YieldController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    // ============================================================================
    // DTO Records
    // ============================================================================

    public record YieldKpiDto(string KpiName, decimal CurrentValue, decimal TargetValue, string Trend);

    public record YieldTrendDto(string Id, DateTime Date, string Product, decimal Yield, decimal TargetYield, int TotalQty, int GoodQty);

    public record WaferDieDto(string WaferId, int DieX, int DieY, string BinCode, string Result, string? DefectType);

    public record YieldAnalysisDto(string Product, decimal AvgYield, decimal MinYield, decimal MaxYield, int TotalLots, decimal PassRate);

    public record BinAnalysisDto(string BinCode, string BinName, int TotalQty, decimal Percentage);

    public record DefectAnalysisDto(string DefectType, int Count, decimal Percentage, string Trend);

    // ============================================================================
    // GET /api/yield/dashboard-kpis - Get yield KPIs
    // ============================================================================

    /// <summary>
    /// 获取良率看板 KPI 指标（各工序良率 + 综合良率）。
    /// 数据来源：ProdTestData（测试良率）+ ProdAssembleData（装配良率）+ ProdLotStep（工序良率）。
    /// GET /api/yield/dashboard-kpis
    /// </summary>
    [HttpGet("dashboard-kpis")]
    public async Task<ApiResponse<List<YieldKpiDto>>> GetDashboardKpis()
    {
        var kpis = new List<YieldKpiDto>();

        // 1. 各装配工序良率 - 从 ProdAssembleData 按 StepCode 聚合
        var assemblySteps = new Dictionary<string, string>
        {
            { "DA", "Die Attach" },
            { "WB", "Wire Bond" },
            { "MLD", "Molding" },
            { "TF", "Trim/Form" }
        };

        var assemblyData = await _dbContext.ProdAssembleData.ToListAsync();

        foreach (var (stepCode, stepName) in assemblySteps)
        {
            var stepRecords = assemblyData.Where(x => x.StepCode == stepCode).ToList();
            if (stepRecords.Any())
            {
                var totalInput = stepRecords.Sum(x => x.InputQty);
                var totalOutput = stepRecords.Sum(x => x.OutputQty);
                var yieldRate = totalInput > 0 ? Math.Round((decimal)totalOutput / totalInput * 100, 2) : 0m;

                var threshold = await _dbContext.MasterYieldRules
                    .Where(r => r.StepCode == stepCode && r.IsActive)
                    .Select(r => r.YieldThreshold)
                    .FirstOrDefaultAsync();

                kpis.Add(new YieldKpiDto(
                    stepName,
                    yieldRate,
                    threshold > 0 ? threshold : 98.0m,
                    yieldRate >= (threshold > 0 ? threshold : 98.0m) ? "Up" : "Down"));
            }
            else
            {
                // 无数据时使用默认目标值
                var threshold = await _dbContext.MasterYieldRules
                    .Where(r => r.StepCode == stepCode && r.IsActive)
                    .Select(r => r.YieldThreshold)
                    .FirstOrDefaultAsync();

                kpis.Add(new YieldKpiDto(stepName, 0m, threshold > 0 ? threshold : 98.0m, "Stable"));
            }
        }

        // 2. 测试良率 - 从 ProdTestData 聚合
        var testData = await _dbContext.ProdTestData.ToListAsync();
        if (testData.Any())
        {
            var totalInput = testData.Sum(x => x.InputQty);
            var totalPass = testData.Sum(x => x.PassQty);
            var testYield = totalInput > 0 ? Math.Round((decimal)totalPass / totalInput * 100, 2) : 0m;

            kpis.Add(new YieldKpiDto(
                "Test",
                testYield,
                97.0m,
                testYield >= 97.0m ? "Up" : "Down"));
        }
        else
        {
            kpis.Add(new YieldKpiDto("Test", 0m, 97.0m, "Stable"));
        }

        // 3. 综合良率 - 从 YieldStatistics 聚合
        var yieldStats = await _dbContext.YieldStatistics.ToListAsync();
        if (yieldStats.Any())
        {
            var totalInput = yieldStats.Sum(x => x.InputQty);
            var totalOutput = yieldStats.Sum(x => x.OutputQty);
            var overallYield = totalInput > 0 ? Math.Round((decimal)totalOutput / totalInput * 100, 2) : 0m;

            kpis.Add(new YieldKpiDto(
                "Overall",
                overallYield,
                94.0m,
                overallYield >= 94.0m ? "Up" : "Down"));
        }
        else
        {
            // 从 ProdLotStep 计算综合良率
            var lotSteps = await _dbContext.ProdLotSteps
                .Where(s => s.Status == "Completed")
                .ToListAsync();

            if (lotSteps.Any())
            {
                var totalInput = lotSteps.Sum(x => x.InputQty);
                var totalPass = lotSteps.Sum(x => x.PassQty);
                var overallYield = totalInput > 0 ? Math.Round((decimal)totalPass / totalInput * 100, 2) : 0m;

                kpis.Add(new YieldKpiDto(
                    "Overall",
                    overallYield,
                    94.0m,
                    overallYield >= 94.0m ? "Up" : "Down"));
            }
            else
            {
                kpis.Add(new YieldKpiDto("Overall", 0m, 94.0m, "Stable"));
            }
        }

        return ApiResponse<List<YieldKpiDto>>.Ok(kpis);
    }

    // ============================================================================
    // GET /api/yield/trend - Get yield trend over time
    // ============================================================================

    /// <summary>
    /// 获取良率趋势（按日期和产品分组，从 ProdLotStep 聚合）。
    /// GET /api/yield/trend?days={days}
    /// </summary>
    [HttpGet("trend")]
    public async Task<ApiResponse<List<YieldTrendDto>>> GetYieldTrend([FromQuery] int days = 30)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);

        var lotSteps = await _dbContext.ProdLotSteps
            .Where(s => s.TrackOutTime.HasValue && s.TrackOutTime >= startDate)
            .ToListAsync();

        // 关联 ProdLots 获取产品信息
        var lotIds = lotSteps.Select(s => s.LotId).Distinct().ToList();
        var lots = await _dbContext.ProdLots
            .Where(l => lotIds.Contains(l.LotId))
            .ToListAsync();

        var lotProductMap = lots.ToDictionary(l => l.LotId, l => l.ProductName);

        // 按日期和产品分组
        var trendGroups = lotSteps
            .Where(s => s.TrackOutTime.HasValue && s.InputQty > 0)
            .GroupBy(s => new { Date = s.TrackOutTime!.Value.Date, LotId = s.LotId })
            .Select(g => new
            {
                Date = g.Key.Date,
                LotId = g.Key.LotId,
                TotalQty = g.Sum(x => x.InputQty),
                GoodQty = g.Sum(x => x.PassQty),
                Yield = g.Sum(x => x.InputQty) > 0
                    ? Math.Round((decimal)g.Sum(x => x.PassQty) / g.Sum(x => x.InputQty) * 100, 2)
                    : 0m
            })
            .ToList();

        // 获取目标良率（从 MasterYieldRule 或 ProdWorkOrder）
        var rules = await _dbContext.MasterYieldRules
            .Where(r => r.IsActive)
            .ToListAsync();

        var result = new List<YieldTrendDto>();
        int seq = 1;

        foreach (var group in trendGroups.OrderByDescending(x => x.Date))
        {
            var product = lotProductMap.GetValueOrDefault(group.LotId) ?? "Unknown";
            var targetYield = rules
                .Where(r => r.StepCode != null)
                .Select(r => r.YieldThreshold)
                .DefaultIfEmpty(95.0m)
                .Min();

            result.Add(new YieldTrendDto(
                $"YT-{seq:D3}",
                group.Date,
                product,
                group.Yield,
                targetYield,
                group.TotalQty,
                group.GoodQty));

            seq++;
        }

        return ApiResponse<List<YieldTrendDto>>.Ok(result);
    }

    // ============================================================================
    // GET /api/yield/wafer-map?waferId={waferId} - Get wafer die map data
    // ============================================================================

    /// <summary>
    /// 获取晶圆 Die 分布图数据（基于 ProdTestData BIN 数据生成模拟 Die 位置）。
    /// GET /api/yield/wafer-map?waferId={waferId}
    /// </summary>
    [HttpGet("wafer-map")]
    public async Task<ApiResponse<List<WaferDieDto>>> GetWaferMap([FromQuery] string waferId)
    {
        if (string.IsNullOrEmpty(waferId))
            return ApiResponse<List<WaferDieDto>>.Fail("晶圆ID不能为空");

        // 查找该晶圆的测试数据
        var testData = await _dbContext.ProdTestData
            .Where(t => t.StepCode.Contains("CP") || t.TestProgram.Contains("CP"))
            .ToListAsync();

        // 从 MfgUnits 获取该晶圆的实际 Die 数据
        var mfgUnits = await _dbContext.MfgUnits
            .Where(u => u.WaferId == waferId)
            .ToListAsync();

        var result = new List<WaferDieDto>();

        if (mfgUnits.Any())
        {
            // 使用实际 MFG Die 数据
            foreach (var unit in mfgUnits)
            {
                var binCode = unit.BinResult switch
                {
                    "Pass" => "1",
                    "Marginal" => "2",
                    "Fail" => "3",
                    _ => "1"
                };

                var defectType = unit.BinResult == "Fail"
                    ? GetDefectTypeFromBin(binCode)
                    : null;

                result.Add(new WaferDieDto(
                    waferId,
                    unit.DieX >= 0 ? unit.DieX : 0,
                    unit.DieY >= 0 ? unit.DieY : 0,
                    binCode,
                    unit.BinResult == "Pass" ? "Good" : (unit.BinResult == "Marginal" ? "Marginal" : "Fail"),
                    defectType));
            }
        }
        else if (testData.Any())
        {
            // 基于 BIN 数据生成模拟晶圆图
            var testRecord = testData.FirstOrDefault();
            if (testRecord != null)
            {
                result = GenerateMockWaferMap(waferId, testRecord);
            }
        }

        if (!result.Any())
        {
            // 无数据时返回空晶圆图模板
            result = GenerateEmptyWaferMap(waferId);
        }

        return ApiResponse<List<WaferDieDto>>.Ok(result);
    }

    // ============================================================================
    // GET /api/yield/analysis?product={product} - Get yield analysis by product
    // ============================================================================

    /// <summary>
    /// 获取产品良率分析（从 ProdLotStep 聚合按产品统计）。
    /// GET /api/yield/analysis?product={product}
    /// </summary>
    [HttpGet("analysis")]
    public async Task<ApiResponse<List<YieldAnalysisDto>>> GetYieldAnalysis([FromQuery] string? product = null)
    {
        var lotSteps = await _dbContext.ProdLotSteps
            .Where(s => s.Status == "Completed" && s.InputQty > 0)
            .ToListAsync();

        var lotIds = lotSteps.Select(s => s.LotId).Distinct().ToList();
        var lots = await _dbContext.ProdLots
            .Where(l => lotIds.Contains(l.LotId))
            .ToListAsync();

        var lotProductMap = lots.ToDictionary(l => l.LotId, l => l.ProductName);

        var analysisGroups = lotSteps
            .Select(s => new { s.LotId, Product = lotProductMap.GetValueOrDefault(s.LotId) ?? "Unknown", Yield = s.InputQty > 0 ? (decimal)s.PassQty / s.InputQty * 100 : 0m })
            .Where(x => !string.IsNullOrEmpty(product) ? x.Product.Contains(product) : true)
            .GroupBy(x => x.Product)
            .Select(g => new YieldAnalysisDto(
                g.Key,
                Math.Round(g.Average(x => x.Yield), 2),
                Math.Round(g.Min(x => x.Yield), 2),
                Math.Round(g.Max(x => x.Yield), 2),
                g.Select(x => x.LotId).Distinct().Count(),
                Math.Round((decimal)g.Count(x => x.Yield >= 95) / g.Count() * 100, 2)))
            .OrderByDescending(x => x.AvgYield)
            .ToList();

        return ApiResponse<List<YieldAnalysisDto>>.Ok(analysisGroups);
    }

    // ============================================================================
    // GET /api/yield/bin-analysis - Get BIN analysis
    // ============================================================================

    /// <summary>
    /// 获取 BIN 分析（从 ProdTestData 按 BIN 代码聚合统计）。
    /// GET /api/yield/bin-analysis
    /// </summary>
    [HttpGet("bin-analysis")]
    public async Task<ApiResponse<List<BinAnalysisDto>>> GetBinAnalysis()
    {
        var testData = await _dbContext.ProdTestData.ToListAsync();

        if (!testData.Any())
            return ApiResponse<List<BinAnalysisDto>>.Ok(new List<BinAnalysisDto>());

        var totalQty = testData.Sum(x => x.PassQty + x.FailQty + x.ScrapQty);

        // 聚合 BIN 数据 (Bin1-Bin8)
        var binMap = new Dictionary<string, (string Name, int Qty)>
        {
            { "1", ("Pass", testData.Sum(x => x.Bin1Qty)) },
            { "2", ("Marginal", testData.Sum(x => x.Bin2Qty)) },
            { "3", ("Open", testData.Sum(x => x.Bin3Qty)) },
            { "4", ("Short", testData.Sum(x => x.Bin4Qty)) },
            { "5", ("Leakage", testData.Sum(x => x.Bin5Qty)) },
            { "6", ("Parametric Fail", testData.Sum(x => x.Bin6Qty)) },
            { "7", ("Functional Fail", testData.Sum(x => x.Bin7Qty)) },
            { "8", ("Other", testData.Sum(x => x.Bin8Qty)) },
        };

        var result = binMap
            .Where(x => x.Value.Qty > 0)
            .Select(x => new BinAnalysisDto(
                x.Key,
                x.Value.Name,
                x.Value.Qty,
                totalQty > 0 ? Math.Round((decimal)x.Value.Qty / totalQty * 100, 2) : 0m))
            .OrderByDescending(x => x.TotalQty)
            .ToList();

        return ApiResponse<List<BinAnalysisDto>>.Ok(result);
    }

    // ============================================================================
    // GET /api/yield/defect-analysis - Get defect analysis
    // ============================================================================

    /// <summary>
    /// 获取缺陷分析（从 ProdTestData 失败数据 + MasterDefectCode 聚合）。
    /// GET /api/yield/defect-analysis
    /// </summary>
    [HttpGet("defect-analysis")]
    public async Task<ApiResponse<List<DefectAnalysisDto>>> GetDefectAnalysis()
    {
        var testData = await _dbContext.ProdTestData
            .Where(t => t.FailQty > 0)
            .ToListAsync();

        if (!testData.Any())
            return ApiResponse<List<DefectAnalysisDto>>.Ok(new List<DefectAnalysisDto>());

        // 根据 BIN 数据推断缺陷类型
        var defectCounts = new Dictionary<string, int>();

        foreach (var record in testData)
        {
            if (record.Bin3Qty > 0) AddDefect(defectCounts, "Open", record.Bin3Qty);
            if (record.Bin4Qty > 0) AddDefect(defectCounts, "Short", record.Bin4Qty);
            if (record.Bin5Qty > 0) AddDefect(defectCounts, "Leakage", record.Bin5Qty);
            if (record.Bin6Qty > 0) AddDefect(defectCounts, "Parametric Fail", record.Bin6Qty);
            if (record.Bin7Qty > 0) AddDefect(defectCounts, "Functional Fail", record.Bin7Qty);
            if (record.Bin8Qty > 0) AddDefect(defectCounts, "Other", record.Bin8Qty);
        }

        var totalDefects = defectCounts.Values.Sum();

        var result = defectCounts
            .Select(x => new DefectAnalysisDto(
                x.Key,
                x.Value,
                totalDefects > 0 ? Math.Round((decimal)x.Value / totalDefects * 100, 2) : 0m,
                GetTrendForDefect(x.Key, testData)))
            .OrderByDescending(x => x.Count)
            .ToList();

        return ApiResponse<List<DefectAnalysisDto>>.Ok(result);
    }

    // ============================================================================
    // Helper Methods
    // ============================================================================

    /// <summary>
    /// 根据 BIN 代码获取缺陷类型
    /// </summary>
    private static string? GetDefectTypeFromBin(string binCode)
    {
        return binCode switch
        {
            "3" => "Open",
            "4" => "Short",
            "5" => "Leakage",
            "6" => "Parametric Fail",
            "7" => "Functional Fail",
            "8" => "Other",
            _ => null
        };
    }

    /// <summary>
    /// 生成模拟晶圆图数据
    /// </summary>
    private static List<WaferDieDto> GenerateMockWaferMap(string waferId, ProdTestData testRecord)
    {
        var result = new List<WaferDieDto>();
        var rng = new Random(waferId.GetHashCode());

        // 模拟 10x10 晶圆网格
        int gridSize = 10;

        // 基于实际 BIN 比例生成
        var total = testRecord.Bin1Qty + testRecord.Bin2Qty + testRecord.Bin3Qty +
                    testRecord.Bin4Qty + testRecord.Bin5Qty + testRecord.Bin6Qty +
                    testRecord.Bin7Qty + testRecord.Bin8Qty;

        if (total == 0) total = 1;

        var dice = new List<(string BinCode, string Result, string? DefectType)>();

        // 按 BIN 比例生成 Die
        AddDice(dice, "1", "Good", null, testRecord.Bin1Qty, total, gridSize, rng);
        AddDice(dice, "2", "Marginal", null, testRecord.Bin2Qty, total, gridSize, rng);
        AddDice(dice, "3", "Fail", "Open", testRecord.Bin3Qty, total, gridSize, rng);
        AddDice(dice, "4", "Fail", "Short", testRecord.Bin4Qty, total, gridSize, rng);
        AddDice(dice, "5", "Fail", "Leakage", testRecord.Bin5Qty, total, gridSize, rng);
        AddDice(dice, "6", "Fail", "Parametric Fail", testRecord.Bin6Qty, total, gridSize, rng);
        AddDice(dice, "7", "Fail", "Functional Fail", testRecord.Bin7Qty, total, gridSize, rng);
        AddDice(dice, "8", "Fail", "Other", testRecord.Bin8Qty, total, gridSize, rng);

        // 打散排列
        dice = dice.OrderBy(_ => rng.Next()).ToList();

        int idx = 0;
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if (idx < dice.Count)
                {
                    var die = dice[idx];
                    result.Add(new WaferDieDto(waferId, x, y, die.BinCode, die.Result, die.DefectType));
                    idx++;
                }
            }
        }

        return result;
    }

    private static void AddDice(
        List<(string BinCode, string Result, string? DefectType)> dice,
        string binCode, string result, string? defectType,
        int binQty, int total, int gridSize, Random rng)
    {
        int targetCount = (int)Math.Round((double)binQty / total * gridSize * gridSize);
        for (int i = 0; i < targetCount; i++)
        {
            dice.Add((binCode, result, defectType));
        }
    }

    /// <summary>
    /// 生成空晶圆图模板
    /// </summary>
    private static List<WaferDieDto> GenerateEmptyWaferMap(string waferId)
    {
        var result = new List<WaferDieDto>();
        int gridSize = 10;

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                result.Add(new WaferDieDto(waferId, x, y, "1", "Good", null));
            }
        }

        return result;
    }

    private static void AddDefect(Dictionary<string, int> counts, string type, int qty)
    {
        if (counts.ContainsKey(type))
            counts[type] += qty;
        else
            counts[type] = qty;
    }

    private static string GetTrendForDefect(string defectType, List<ProdTestData> testData)
    {
        // 基于最近数据判断趋势
        var sorted = testData.OrderByDescending(x => x.CreatedAt).ToList();
        if (sorted.Count < 2) return "Stable";

        var recent = sorted.Take(sorted.Count / 2).ToList();
        var older = sorted.Skip(sorted.Count / 2).ToList();

        int GetDefectQty(List<ProdTestData> records, string type) => type switch
        {
            "Open" => records.Sum(r => r.Bin3Qty),
            "Short" => records.Sum(r => r.Bin4Qty),
            "Leakage" => records.Sum(r => r.Bin5Qty),
            "Parametric Fail" => records.Sum(r => r.Bin6Qty),
            "Functional Fail" => records.Sum(r => r.Bin7Qty),
            "Other" => records.Sum(r => r.Bin8Qty),
            _ => 0
        };

        var recentQty = GetDefectQty(recent, defectType);
        var olderQty = GetDefectQty(older, defectType);

        if (olderQty == 0) return "New";
        return recentQty > olderQty * 1.1m ? "Up" : (recentQty < olderQty * 0.9m ? "Down" : "Stable");
    }
}
