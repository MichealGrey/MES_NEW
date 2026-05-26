namespace MES.Domain.Production;

/// <summary>
/// 封装类型
/// </summary>
public enum PackageType
{
    QFP,     // 四方扁平封装
    BGA,     // 球栅阵列
    QFN,     // 四方无引脚扁平
    SOP,     // 小外形封装
    DIP,     // 双列直插
    CSP,     // 芯片级封装
    WLCSP,   // 晶圆级芯片封装
    SiP,     // 系统级封装
    TO,      // 插入式封装
    SOT      // 小外形晶体管
}
