namespace MES.Domain.Production;

/// <summary>
/// 测试类型
/// </summary>
public enum TestType
{
    FinalTest,    // 成品测试（FT）
    BurnIn,       // 老化测试
    OS,           // 开封目检
    PreTest,      // 预测试
    SampleTest    // 抽检
}
