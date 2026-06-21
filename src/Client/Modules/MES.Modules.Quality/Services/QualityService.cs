using MES.Modules.Quality.Models;

namespace MES.Modules.Quality.Services;

public class QualityService : IQualityService
{
    private readonly List<SpcChartItem> _spcCharts = [];
    private readonly List<SpcRuleItem> _spcRules = [];
    private readonly List<OocEventItem> _oocEvents = [];
    private readonly List<FdcMonitorItem> _fdcMonitors = [];
    private readonly List<InspectionItem> _inspections = [];
    private readonly List<FmeaItem> _fmeaItems = [];
    private readonly List<QualityTargetItem> _qualityTargets = [];
    private readonly List<MsaItem> _msaItems = [];

    public QualityService() => SeedData();

    private void SeedData()
    {
        // SPC Charts
        _spcCharts.AddRange([
            new SpcChartItem { Id = "SPC-001", EquipmentId = "WB-01", Parameter = "WirePullStrength", UCL = 12.0, CL = 8.0, LCL = 4.0, LatestValue = 8.5, SampleCount = 120, Status = "InControl", LastUpdate = DateTime.Now.AddMinutes(-5) },
            new SpcChartItem { Id = "SPC-002", EquipmentId = "WB-01", Parameter = "BallShear", UCL = 35.0, CL = 25.0, LCL = 15.0, LatestValue = 26.3, SampleCount = 85, Status = "InControl", LastUpdate = DateTime.Now.AddMinutes(-10) },
            new SpcChartItem { Id = "SPC-003", EquipmentId = "WB-02", Parameter = "LoopHeight", UCL = 260, CL = 230, LCL = 200, LatestValue = 235, SampleCount = 95, Status = "InControl", LastUpdate = DateTime.Now.AddMinutes(-15) },
            new SpcChartItem { Id = "SPC-004", EquipmentId = "DB-01", Parameter = "DieShear", UCL = 45.0, CL = 30.0, LCL = 15.0, LatestValue = 32.1, SampleCount = 110, Status = "Warning", LastUpdate = DateTime.Now.AddMinutes(-8) },
            new SpcChartItem { Id = "SPC-005", EquipmentId = "MP-01", Parameter = "EpoxyVoid", UCL = 15.0, CL = 5.0, LCL = 0, LatestValue = 6.2, SampleCount = 75, Status = "InControl", LastUpdate = DateTime.Now.AddMinutes(-20) },
            new SpcChartItem { Id = "SPC-006", EquipmentId = "MP-02", Parameter = "WireSweep", UCL = 8.0, CL = 4.0, LCL = 0, LatestValue = 4.8, SampleCount = 60, Status = "OutControl", LastUpdate = DateTime.Now.AddMinutes(-3) },
            new SpcChartItem { Id = "SPC-007", EquipmentId = "WS-01", Parameter = "DieShear", UCL = 45.0, CL = 30.0, LCL = 15.0, LatestValue = 28.7, SampleCount = 90, Status = "InControl", LastUpdate = DateTime.Now.AddMinutes(-12) },
            new SpcChartItem { Id = "SPC-008", EquipmentId = "TH-01", Parameter = "LeadPull", UCL = 20.0, CL = 12.0, LCL = 5.0, LatestValue = 13.2, SampleCount = 100, Status = "InControl", LastUpdate = DateTime.Now.AddMinutes(-18) },
        ]);

        // SPC Rules
        _spcRules.AddRange([
            new SpcRuleItem { Id = "RULE-001", RuleName = "1点超出控制限", Description = "任何1点超出UCL或LCL", IsEnabled = true, ViolationCount = 12, LastTrigger = DateTime.Now.AddHours(-2) },
            new SpcRuleItem { Id = "RULE-002", RuleName = "连续9点在中心线同侧", Description = "连续9个点在CL同一侧", IsEnabled = true, ViolationCount = 8, LastTrigger = DateTime.Now.AddHours(-5) },
            new SpcRuleItem { Id = "RULE-003", RuleName = "连续6点递增或递减", Description = "连续6个点单调递增或递减", IsEnabled = true, ViolationCount = 15, LastTrigger = DateTime.Now.AddHours(-1) },
            new SpcRuleItem { Id = "RULE-004", RuleName = "连续14点交替上下", Description = "连续14个点交替上升下降", IsEnabled = false, ViolationCount = 3, LastTrigger = DateTime.Now.AddDays(-3) },
            new SpcRuleItem { Id = "RULE-005", RuleName = "2/3点超出2sigma", Description = "连续3点中有2点超出2sigma", IsEnabled = true, ViolationCount = 6, LastTrigger = DateTime.Now.AddHours(-8) },
            new SpcRuleItem { Id = "RULE-006", RuleName = "4/5点超出1sigma", Description = "连续5点中有4点超出1sigma", IsEnabled = true, ViolationCount = 20, LastTrigger = DateTime.Now.AddMinutes(-30) },
        ]);

        // OOC Events
        _oocEvents.AddRange([
            new OocEventItem { Id = "OOC-001", EquipmentId = "WB-01", Parameter = "WirePullStrength", RuleName = "1点超出控制限", EventTime = DateTime.Now.AddHours(-2), Status = "Pending", Severity = "High", LotId = "LOT-2024-001", ResponseTime = null },
            new OocEventItem { Id = "OOC-002", EquipmentId = "DB-01", Parameter = "DieShear", RuleName = "连续9点在中心线同侧", EventTime = DateTime.Now.AddHours(-5), Status = "Investigating", Severity = "Medium", LotId = "LOT-2024-002", ResponseTime = DateTime.Now.AddHours(-4) },
            new OocEventItem { Id = "OOC-003", EquipmentId = "MP-02", Parameter = "WireSweep", RuleName = "2/3点超出2sigma", EventTime = DateTime.Now.AddMinutes(-30), Status = "Pending", Severity = "High", LotId = "LOT-2024-003", ResponseTime = null },
            new OocEventItem { Id = "OOC-004", EquipmentId = "WB-02", Parameter = "LoopHeight", RuleName = "连续6点递增", EventTime = DateTime.Now.AddDays(-1), Status = "Closed", Severity = "Low", LotId = "LOT-2024-004", ResponseTime = DateTime.Now.AddDays(-1).AddHours(1) },
            new OocEventItem { Id = "OOC-005", EquipmentId = "TH-01", Parameter = "LeadPull", RuleName = "4/5点超出1sigma", EventTime = DateTime.Now.AddHours(-8), Status = "Closed", Severity = "Medium", LotId = "LOT-2024-005", ResponseTime = DateTime.Now.AddHours(-7) },
        ]);

        // FDC Monitors
        _fdcMonitors.AddRange([
            new FdcMonitorItem { Id = "FDC-001", EquipmentId = "WB-01", Chamber = "Head-1", T2 = 12.5, SPE = 3.2, Status = "Normal", RunId = "RUN-001", Timestamp = DateTime.Now.AddMinutes(-2) },
            new FdcMonitorItem { Id = "FDC-002", EquipmentId = "WB-01", Chamber = "Head-2", T2 = 28.7, SPE = 8.9, Status = "Warning", RunId = "RUN-002", Timestamp = DateTime.Now.AddMinutes(-5) },
            new FdcMonitorItem { Id = "FDC-003", EquipmentId = "WB-02", Chamber = "Head-1", T2 = 45.1, SPE = 15.3, Status = "Alarm", RunId = "RUN-003", Timestamp = DateTime.Now.AddMinutes(-1) },
            new FdcMonitorItem { Id = "FDC-004", EquipmentId = "MP-01", Chamber = "Cavity-1", T2 = 8.3, SPE = 2.1, Status = "Normal", RunId = "RUN-004", Timestamp = DateTime.Now.AddMinutes(-8) },
            new FdcMonitorItem { Id = "FDC-005", EquipmentId = "MP-01", Chamber = "Cavity-2", T2 = 22.4, SPE = 6.5, Status = "Warning", RunId = "RUN-005", Timestamp = DateTime.Now.AddMinutes(-3) },
            new FdcMonitorItem { Id = "FDC-006", EquipmentId = "DB-01", Chamber = "Head-1", T2 = 5.8, SPE = 1.4, Status = "Normal", RunId = "RUN-006", Timestamp = DateTime.Now.AddMinutes(-10) },
            new FdcMonitorItem { Id = "FDC-007", EquipmentId = "TH-01", Chamber = "Site-1", T2 = 9.1, SPE = 2.8, Status = "Normal", RunId = "RUN-007", Timestamp = DateTime.Now.AddMinutes(-15) },
            new FdcMonitorItem { Id = "FDC-008", EquipmentId = "MP-02", Chamber = "Cavity-1", T2 = 35.6, SPE = 11.2, Status = "Alarm", RunId = "RUN-008", Timestamp = DateTime.Now.AddMinutes(-1) },
            new FdcMonitorItem { Id = "FDC-009", EquipmentId = "WS-01", Chamber = "Spindle-1", T2 = 15.2, SPE = 4.1, Status = "Normal", RunId = "RUN-009", Timestamp = DateTime.Now.AddMinutes(-6) },
        ]);

        // Inspections
        _inspections.AddRange([
            new InspectionItem { Id = "INS-001", LotId = "LOT-2024-001", Product = "QFN48", InspectionType = "首检", Result = "Pass", Inspector = "张工", InspectTime = DateTime.Now.AddHours(-2), DefectCount = 0, Comments = "各项指标正常" },
            new InspectionItem { Id = "INS-002", LotId = "LOT-2024-002", Product = "SOP8", InspectionType = "巡检", Result = "Pass", Inspector = "李工", InspectTime = DateTime.Now.AddHours(-4), DefectCount = 1, Comments = "发现1个外观不良" },
            new InspectionItem { Id = "INS-003", LotId = "LOT-2024-003", Product = "BGA256", InspectionType = "末检", Result = "Fail", Inspector = "王工", InspectTime = DateTime.Now.AddHours(-1), DefectCount = 5, Comments = "电性测试不合格" },
            new InspectionItem { Id = "INS-004", LotId = "LOT-2024-004", Product = "QFN48", InspectionType = "首检", Result = "Pass", Inspector = "赵工", InspectTime = DateTime.Now.AddDays(-1), DefectCount = 0, Comments = "" },
            new InspectionItem { Id = "INS-005", LotId = "LOT-2024-005", Product = "LQFP128", InspectionType = "巡检", Result = "Pass", Inspector = "张工", InspectTime = DateTime.Now.AddHours(-6), DefectCount = 2, Comments = "焊接缺陷2个" },
        ]);

        // FMEA
        _fmeaItems.AddRange([
            new FmeaItem { Id = "FMEA-001", ProcessStep = "Wire Bonding", FailureMode = "WirePull强度不足", FailureEffect = "功能不良", Severity = 8, Occurrence = 3, Detection = 4, ControlMeasure = "WirePull测试", Responsible = "工艺部" },
            new FmeaItem { Id = "FMEA-002", ProcessStep = "Die Attach", FailureMode = "DieShear强度不足", FailureEffect = "芯片脱落", Severity = 9, Occurrence = 2, Detection = 5, ControlMeasure = "DieShear测试", Responsible = "质量部" },
            new FmeaItem { Id = "FMEA-003", ProcessStep = "Mold", FailureMode = "EpoxyVoid超标", FailureEffect = "封装缺陷", Severity = 7, Occurrence = 4, Detection = 3, ControlMeasure = "X-Ray检测", Responsible = "工艺部" },
            new FmeaItem { Id = "FMEA-004", ProcessStep = "Plating", FailureMode = "镀层厚度不均", FailureEffect = "焊接不良", Severity = 6, Occurrence = 5, Detection = 4, ControlMeasure = "厚度测量", Responsible = "工艺部" },
            new FmeaItem { Id = "FMEA-005", ProcessStep = "Marking", FailureMode = "标记不清", FailureEffect = "识别错误", Severity = 4, Occurrence = 6, Detection = 2, ControlMeasure = "AOI检测", Responsible = "质量部" },
        ]);

        // Quality Targets
        _qualityTargets.AddRange([
            new QualityTargetItem { Id = "QT-001", TargetName = "一次通过率", CurrentValue = 96.5, TargetValue = 98.0, Unit = "%", Period = "月", Trend = "Up", Status = "AtRisk" },
            new QualityTargetItem { Id = "QT-002", TargetName = "客户投诉率", CurrentValue = 120, TargetValue = 100, Unit = "PPM", Period = "月", Trend = "Down", Status = "Missed" },
            new QualityTargetItem { Id = "QT-003", TargetName = "SPC违规率", CurrentValue = 2.1, TargetValue = 1.5, Unit = "%", Period = "周", Trend = "Flat", Status = "Missed" },
            new QualityTargetItem { Id = "QT-004", TargetName = "OOC响应时间", CurrentValue = 25, TargetValue = 30, Unit = "分钟", Period = "月", Trend = "Up", Status = "OnTarget" },
            new QualityTargetItem { Id = "QT-005", TargetName = "MSA合格率", CurrentValue = 98.2, TargetValue = 95.0, Unit = "%", Period = "季度", Trend = "Up", Status = "OnTarget" },
        ]);

        // MSA
        _msaItems.AddRange([
            new MsaItem { Id = "MSA-001", GaugeName = "WirePull测试仪", Parameter = "WirePullStrength", OperatorCount = 3, PartCount = 10, TrialCount = 3, GRR = 8.5, Status = "Acceptable", NDC = 12, LastCalibration = DateTime.Now.AddDays(-30) },
            new MsaItem { Id = "MSA-002", GaugeName = "DieShear测试仪", Parameter = "DieShearForce", OperatorCount = 3, PartCount = 10, TrialCount = 3, GRR = 15.2, Status = "Conditional", NDC = 8, LastCalibration = DateTime.Now.AddDays(-15) },
            new MsaItem { Id = "MSA-003", GaugeName = "厚度测量仪", Parameter = "PackageThickness", OperatorCount = 2, PartCount = 10, TrialCount = 2, GRR = 5.1, Status = "Acceptable", NDC = 15, LastCalibration = DateTime.Now.AddDays(-45) },
            new MsaItem { Id = "MSA-004", GaugeName = "视觉检测系统", Parameter = "DefectCount", OperatorCount = 4, PartCount = 15, TrialCount = 3, GRR = 22.3, Status = "Unacceptable", NDC = 5, LastCalibration = DateTime.Now.AddDays(-60) },
        ]);
    }

    public async Task<List<SpcChartItem>> GetSpcChartsAsync()
    {
        await Task.Delay(50);
        return _spcCharts.ToList();
    }

    public async Task SaveSpcChartAsync(SpcChartItem item)
    {
        await Task.Delay(50);
        var existing = _spcCharts.FirstOrDefault(x => x.Id == item.Id);
        if (existing is null) _spcCharts.Add(item);
        else _spcCharts[_spcCharts.IndexOf(existing)] = item;
    }

    public async Task<List<SpcRuleItem>> GetSpcRulesAsync()
    {
        await Task.Delay(50);
        return _spcRules.ToList();
    }

    public async Task SaveSpcRuleAsync(SpcRuleItem rule)
    {
        await Task.Delay(50);
        var existing = _spcRules.FirstOrDefault(x => x.Id == rule.Id);
        if (existing is null) _spcRules.Add(rule);
        else _spcRules[_spcRules.IndexOf(existing)] = rule;
    }

    public async Task DeleteSpcRuleAsync(string ruleId)
    {
        await Task.Delay(50);
        var rule = _spcRules.FirstOrDefault(x => x.Id == ruleId);
        if (rule is not null) _spcRules.Remove(rule);
    }

    public async Task<List<OocEventItem>> GetOocEventsAsync()
    {
        await Task.Delay(50);
        return _oocEvents.ToList();
    }

    public async Task UpdateOocEventStatusAsync(string eventId, string status)
    {
        await Task.Delay(50);
        var evt = _oocEvents.FirstOrDefault(x => x.Id == eventId);
        if (evt is not null)
        {
            evt.Status = status;
            if (status == "Investigating") evt.ResponseTime = DateTime.Now;
        }
    }

    public async Task<List<FdcMonitorItem>> GetFdcMonitorsAsync()
    {
        await Task.Delay(50);
        return _fdcMonitors.ToList();
    }

    public async Task<List<InspectionItem>> GetInspectionsAsync()
    {
        await Task.Delay(50);
        return _inspections.ToList();
    }

    public async Task SaveInspectionAsync(InspectionItem item)
    {
        await Task.Delay(50);
        var existing = _inspections.FirstOrDefault(x => x.Id == item.Id);
        if (existing is null) _inspections.Add(item);
        else _inspections[_inspections.IndexOf(existing)] = item;
    }

    public async Task DeleteInspectionAsync(string inspectionId)
    {
        await Task.Delay(50);
        var item = _inspections.FirstOrDefault(x => x.Id == inspectionId);
        if (item is not null) _inspections.Remove(item);
    }

    public async Task<List<FmeaItem>> GetFmeaItemsAsync()
    {
        await Task.Delay(50);
        return _fmeaItems.ToList();
    }

    public async Task SaveFmeaAsync(FmeaItem item)
    {
        await Task.Delay(50);
        var existing = _fmeaItems.FirstOrDefault(x => x.Id == item.Id);
        if (existing is null) _fmeaItems.Add(item);
        else _fmeaItems[_fmeaItems.IndexOf(existing)] = item;
    }

    public async Task DeleteFmeaAsync(string fmeaId)
    {
        await Task.Delay(50);
        var item = _fmeaItems.FirstOrDefault(x => x.Id == fmeaId);
        if (item is not null) _fmeaItems.Remove(item);
    }

    public async Task<List<QualityTargetItem>> GetQualityTargetsAsync()
    {
        await Task.Delay(50);
        return _qualityTargets.ToList();
    }

    public async Task SaveQualityTargetAsync(QualityTargetItem item)
    {
        await Task.Delay(50);
        var existing = _qualityTargets.FirstOrDefault(x => x.Id == item.Id);
        if (existing is null) _qualityTargets.Add(item);
        else _qualityTargets[_qualityTargets.IndexOf(existing)] = item;
    }

    public async Task DeleteQualityTargetAsync(string targetId)
    {
        await Task.Delay(50);
        var item = _qualityTargets.FirstOrDefault(x => x.Id == targetId);
        if (item is not null) _qualityTargets.Remove(item);
    }

    public async Task<List<MsaItem>> GetMsaItemsAsync()
    {
        await Task.Delay(50);
        return _msaItems.ToList();
    }

    public async Task SaveMsaAsync(MsaItem item)
    {
        await Task.Delay(50);
        var existing = _msaItems.FirstOrDefault(x => x.Id == item.Id);
        if (existing is null) _msaItems.Add(item);
        else _msaItems[_msaItems.IndexOf(existing)] = item;
    }

    public async Task DeleteMsaAsync(string msaId)
    {
        await Task.Delay(50);
        var item = _msaItems.FirstOrDefault(x => x.Id == msaId);
        if (item is not null) _msaItems.Remove(item);
    }
}
