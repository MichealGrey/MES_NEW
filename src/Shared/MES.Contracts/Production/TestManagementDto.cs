namespace MES.Contracts.Production;

/// <summary>
/// CP测试（晶圆探针测试）请求
/// </summary>
public class CpTestRequest
{
    /// <summary>
    /// 批次ID
    /// </summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>
    /// 晶圆ID
    /// </summary>
    public string WaferId { get; set; } = string.Empty;

    /// <summary>
    /// 测试程序
    /// </summary>
    public string TestProgram { get; set; } = string.Empty;

    /// <summary>
    /// 总晶粒数
    /// </summary>
    public int TotalDice { get; set; }

    /// <summary>
    /// 通过晶粒数
    /// </summary>
    public int PassDice { get; set; }

    /// <summary>
    /// 失败晶粒数
    /// </summary>
    public int FailDice { get; set; }

    /// <summary>
    /// Bin分布
    /// </summary>
    public List<BinDistributionItem> BinDistribution { get; set; } = [];

    /// <summary>
    /// 操作员ID
    /// </summary>
    public string OperatorId { get; set; } = string.Empty;
}

/// <summary>
/// FT测试（最终测试）请求
/// </summary>
public class FtTestRequest
{
    /// <summary>
    /// 批次ID
    /// </summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>
    /// 测试程序
    /// </summary>
    public string TestProgram { get; set; } = string.Empty;

    /// <summary>
    /// 总数量
    /// </summary>
    public int TotalQty { get; set; }

    /// <summary>
    /// 通过数量
    /// </summary>
    public int PassQty { get; set; }

    /// <summary>
    /// 失败数量
    /// </summary>
    public int FailQty { get; set; }

    /// <summary>
    /// Bin分布
    /// </summary>
    public List<BinDistributionItem> BinDistribution { get; set; } = [];

    /// <summary>
    /// 操作员ID
    /// </summary>
    public string OperatorId { get; set; } = string.Empty;
}

/// <summary>
/// Bin分布项
/// </summary>
public class BinDistributionItem
{
    /// <summary>
    /// Bin代码
    /// </summary>
    public string BinCode { get; set; } = string.Empty;

    /// <summary>
    /// Bin名称
    /// </summary>
    public string BinName { get; set; } = string.Empty;

    /// <summary>
    /// 数量
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 百分比
    /// </summary>
    public decimal Percentage { get; set; }
}

/// <summary>
/// 测试结果响应
/// </summary>
public class TestResultResponse
{
    /// <summary>
    /// 测试ID
    /// </summary>
    public string TestId { get; set; } = string.Empty;

    /// <summary>
    /// 批次ID
    /// </summary>
    public string LotId { get; set; } = string.Empty;

    /// <summary>
    /// 测试类型 (CP/FT)
    /// </summary>
    public string TestType { get; set; } = string.Empty;

    /// <summary>
    /// 良率
    /// </summary>
    public decimal Yield { get; set; }

    /// <summary>
    /// 总数量
    /// </summary>
    public int TotalQty { get; set; }

    /// <summary>
    /// 通过数量
    /// </summary>
    public int PassQty { get; set; }

    /// <summary>
    /// 失败数量
    /// </summary>
    public int FailQty { get; set; }

    /// <summary>
    /// 测试时间
    /// </summary>
    public DateTime TestTime { get; set; }

    /// <summary>
    /// Bin分布
    /// </summary>
    public List<BinDistributionItem> BinDistribution { get; set; } = [];

    /// <summary>
    /// 测试程序
    /// </summary>
    public string? TestProgram { get; set; }

    /// <summary>
    /// 晶圆ID (仅CP测试)
    /// </summary>
    public string? WaferId { get; set; }
}

/// <summary>
/// 良率统计响应
/// </summary>
public class YieldStatisticsResponse
{
    /// <summary>
    /// CP良率
    /// </summary>
    public decimal CpYield { get; set; }

    /// <summary>
    /// FT良率
    /// </summary>
    public decimal FtYield { get; set; }

    /// <summary>
    /// 综合良率
    /// </summary>
    public decimal OverallYield { get; set; }

    /// <summary>
    /// 目标良率
    /// </summary>
    public decimal TargetYield { get; set; }

    /// <summary>
    /// CP测试总数量
    /// </summary>
    public int CpTotalQty { get; set; }

    /// <summary>
    /// CP通过数量
    /// </summary>
    public int CpPassQty { get; set; }

    /// <summary>
    /// FT测试总数量
    /// </summary>
    public int FtTotalQty { get; set; }

    /// <summary>
    /// FT通过数量
    /// </summary>
    public int FtPassQty { get; set; }

    /// <summary>
    /// CP测试次数
    /// </summary>
    public int CpTestCount { get; set; }

    /// <summary>
    /// FT测试次数
    /// </summary>
    public int FtTestCount { get; set; }
}

/// <summary>
/// 良率趋势数据项
/// </summary>
public class YieldTrendItem
{
    /// <summary>
    /// 日期
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 产品代码
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// CP良率
    /// </summary>
    public decimal CpYield { get; set; }

    /// <summary>
    /// FT良率
    /// </summary>
    public decimal FtYield { get; set; }

    /// <summary>
    /// 综合良率
    /// </summary>
    public decimal OverallYield { get; set; }

    /// <summary>
    /// 批次数量
    /// </summary>
    public int LotCount { get; set; }
}

/// <summary>
/// 测试执行响应
/// </summary>
public class TestExecutionResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 测试ID
    /// </summary>
    public string TestId { get; set; } = string.Empty;

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 良率
    /// </summary>
    public decimal Yield { get; set; }
}
