using System.Text.Json.Serialization;
using MES.Domain.Production;

namespace MES.Modules.Production.Models;

public class WorkOrderInfo
{
    // --- 基础字段 ---
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int PlannedQty { get; set; }
    public int CompletedQty { get; set; }
    public ProcessStatus Status { get; set; } = ProcessStatus.Created;
    public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Normal;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // --- 封装测试核心字段 ---
    public string RouteId { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public string DieName { get; set; } = string.Empty;                // 管芯名称（原Device）
    public PackageType PackageType { get; set; } = PackageType.QFP;    // 封装类型（原TechNode）
    public int WaferQty { get; set; } = 25;                            // 晶圆数
    public int UnitQty { get; set; }                                    // 成品颗数
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPN { get; set; } = string.Empty;             // 客户料号
    public string InternalPN { get; set; } = string.Empty;             // 内部料号
    public string Area { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public string SpecId { get; set; } = string.Empty;
    public string SpecVersion { get; set; } = string.Empty;
    public string TestProgram { get; set; } = string.Empty;            // 测试程序版本
    public string BinSpec { get; set; } = string.Empty;                // Bin规格定义
    public string GradeSpec { get; set; } = string.Empty;              // 等级分选规格
    public string WaferSource { get; set; } = string.Empty;            // 晶圆来源
    public string? SubconLotId { get; set; }                           // 委外批号
    public string Creator { get; set; } = string.Empty;
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string? HoldReason { get; set; }
    public double TargetCPYield { get; set; } = 99.0;                  // 目标封装良率
    public double TargetFTYield { get; set; } = 98.0;                  // 目标测试良率
    public double YieldTarget { get; set; }                             // 保留兼容
    public string? Remark { get; set; }

    // --- 计算属性 ---
    [JsonIgnore]
    public double ProgressPercent => PlannedQty > 0
        ? Math.Round((double)CompletedQty / PlannedQty * 100, 1)
        : 0;

    /// <summary>封装类型显示文本</summary>
    [JsonIgnore]
    public string PackageTypeDisplay => PackageType.ToString();
}
