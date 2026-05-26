namespace MES.Domain.Equipment;

/// <summary>
/// 封装测试厂设备类型
/// </summary>
public enum EquipmentType
{
    DicingSaw,      // 切割机/划片机
    DieBonder,      // 贴片机
    WireBonder,     // 焊线机
    MoldingPress,   // 塑封机
    LaserMark,      // 激光打标机
    TrimForm,       // 切筋成型机
    TestHandler,    // 测试分选机
    BurnIn,         // 老化测试箱
    AOI,            // 自动光学检测
    XRay,           // X射线检测
    PlasmaClean,    // 等离子清洗
    TapeReel        // 编带包装机
}
