namespace MES.Domain.Production;

/// <summary>
/// 批次层级枚举
/// 对应 OSAT 封测厂的五层批次模型
/// </summary>
public enum LotLevel
{
    /// <summary>
    /// 晶圆批次 (Wafer Lot)
    /// </summary>
    WaferLot = 1,

    /// <summary>
    /// 母批次 (Mother Lot)
    /// </summary>
    MotherLot = 2,

    /// <summary>
    /// 子批次 (Sub Lot)
    /// </summary>
    SubLot = 3,

    /// <summary>
    /// 等级批次 (Grade Lot)
    /// </summary>
    GradeLot = 4,

    /// <summary>
    /// 制造ID (MFG ID / 最小包装单位)
    /// </summary>
    MfgId = 5
}
