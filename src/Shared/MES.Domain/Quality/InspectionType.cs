namespace MES.Domain.Quality;

/// <summary>
/// 封装测试厂检验类型
/// </summary>
public enum InspectionType
{
    VisualInspect,   // 外观目检
    XRay,            // X射线检测（空洞/线弧/位移）
    AOI,             // 自动光学检测
    CrossSection,    // 切片分析
    SAM,             // 声学扫描（分层/空洞）
    ElectricalTest,  // 电性测试
    OpenShort        // 开短路测试
}
