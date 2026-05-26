namespace MES.Modules.Production.Models;

/// <summary>
/// 数量校验结果
/// </summary>
public class QuantityValidationResult
{
    public bool IsBalanced => Errors.Count == 0;
    public int ExpectedTotal { get; set; }
    public int ActualTotal { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];

    public void AddError(string error) => Errors.Add(error);
    public void AddWarning(string warning) => Warnings.Add(warning);
}

/// <summary>
/// 数量事务记录
/// </summary>
public class QuantityTransaction
{
    public string TransactionId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public int ScrapQty { get; set; }
    public int ReworkQty { get; set; }
    public int HoldQty { get; set; }
    public int PendingQty { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? Remark { get; set; }
}

/// <summary>
/// 批次数量汇总
/// </summary>
public class LotQuantitySummary
{
    public string LotId { get; set; } = string.Empty;
    public int TotalInput { get; set; }
    public int TotalPass { get; set; }
    public int TotalFail { get; set; }
    public int TotalScrap { get; set; }
    public int TotalRework { get; set; }
    public int TotalHold { get; set; }
    public int TotalPending { get; set; }
    public double YieldRate => TotalInput > 0 ? Math.Round((double)TotalPass / TotalInput * 100, 2) : 0;
    public List<QuantityTransaction> Transactions { get; set; } = [];
}
