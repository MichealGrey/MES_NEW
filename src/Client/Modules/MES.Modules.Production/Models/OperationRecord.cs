namespace MES.Modules.Production.Models;

/// <summary>
/// 操作记录模型
/// </summary>
public class OperationRecord
{
    public DateTime Time { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty; // "进站" / "出站"
    public string EquipmentId { get; set; } = string.Empty;
    public string? Remark { get; set; }                        // 备注（校验结果摘要）

    /// <summary>格式化时间显示</summary>
    public string TimeDisplay => Time.ToString("HH:mm:ss");
}
