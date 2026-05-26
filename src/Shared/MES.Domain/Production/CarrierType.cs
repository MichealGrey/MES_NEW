namespace MES.Domain.Production;

/// <summary>
/// 载具类型
/// </summary>
public enum CarrierType
{
    WaferFrame,   // 晶圆贴膜框
    LeadFrame,    // 引线框架
    Strip,        // 条带（已切筋前）
    Magazine,     // 料管
    Tray,         // 吸塑盘/托盘
    Reel,         // 编带盘
    WafflePack    // 华夫盘
}
