# MES系统优化改善计划——阶段五（Phase 5）实施方案

> **版本：** V5.0.0  
> **日期：** 2026-06-06  
> **定位：** 收尾优化阶段——完善追溯体系、管理决策支持、系统优化  
> **前置依赖：** 阶段一~四已完成（基础数据、生产执行、质量管理、设备管理、计划调度）

---

## 一、阶段五概述

### 1.1 总体目标

完善三大核心领域：

| 领域 | 目标 | 状态 |
|------|------|------|
| **追溯深化** | 从"有表有界面"到"全链路可追溯、可视化、可导出" | 数据库表完整，UI框架存在，缺服务层和API |
| **管理决策支持** | 从"mock数据看板"到"真实KPI驱动的管理决策平台" | UI存在但全部使用硬编码模拟数据 |
| **系统优化** | 从"功能可用"到"生产就绪" | 基础功能有，缺性能优化、归档、审计保护 |

### 1.2 实施范围

| 序号 | 模块 | 类型 | 工作量评估 |
|------|------|------|------------|
| 1 | 追溯深化模块 | **改造** | 大（服务层/API/报告导出/MFGID追溯） |
| 2 | 全厂KPI看板 | **改造** | 大（后端聚合+UI重构） |
| 3 | 生产成本分析 | **新增** | 中（新表+后端+UI） |
| 4 | 人员绩效管理 | **新增** | 中（新表+后端+UI） |
| 5 | NPI管理深化 | **完善** | 小（NpiProject已存在，补服务层） |
| 6 | 可靠性测试 | **新增** | 中（新表+后端+UI+AEC-Q100） |
| 7 | 报表自动化 | **新增** | 大（定时任务+模板+多格式导出） |
| 8 | 审计日志不可篡改 | **完善** | 小（哈希链+权限保护） |
| 9 | 系统配置优化 | **完善** | 小（API+热配置） |
| 10 | 数据归档与性能优化 | **新增** | 中（归档策略+缓存+索引） |
| 11 | 合规报表 | **新增** | 小（IATF16949/AEC-Q100/ISO9001） |

### 1.3 技术栈说明

- **后端：** ASP.NET Core 8 Web API + Entity Framework Core + MySQL
- **前端：** WPF (Prism) + .NET 8
- **缓存：** MemoryCache（本地）/ Redis（可选，生产推荐）
- **定时任务：** IHostedService / Hangfire（可选）
- **报告导出：** QuestPDF（PDF）/ ClosedXML（Excel）/ CsvHelper（CSV）

---

## 二、现有UI详细评估

### 2.1 Trace模块（追溯）

| View | 文件路径 | 当前功能 | 符合场景 | 缺失功能 | 优化建议 | 真实API |
|------|----------|----------|----------|----------|----------|---------|
| **LotTraceView** | `src/Client/Modules/MES.Modules.Trace/Views/LotTraceView.xaml` | DataGrid展示批次追溯列表（工序/设备/配方/进出站时间/结果） | ✅ 基本符合 | 无搜索/筛选、无导出、无批次选择器 | 增加LotId搜索框、日期筛选、导出按钮；右侧增加正向追溯Timeline面板 | ❌ 使用TraceService硬编码数据 |
| **ForwardTraceView** | `src/Client/Modules/MES.Modules.Trace/Views/ForwardTraceView.xaml` | 占位页面"开发中..." | ❌ 不可用 | 完整功能缺失 | 按工序顺序展示正向追溯链，用TreeView或自定义Timeline控件 | ❌ 无实现 |
| **BackwardTraceView** | `src/Client/Modules/MES.Modules.Trace/Views/BackwardTraceView.xaml` | 占位页面"开发中..." | ❌ 不可用 | 完整功能缺失 | 从原材料→Wafer→MFG→Lot的反向追溯链，用TreeView展示 | ❌ 无实现 |
| **GenealogyView** (Trace模块) | `src/Client/Modules/MES.Modules.Trace/Views/GenealogyView.xaml` | DataGrid展示血缘关系（批次/父批次/产品/工序/时间/Strip数） | ⚠️ 基础符合 | 无树形可视化、无交互展开、无MFGID关联 | 改用TreeView或自定义控件展示层级谱系图；支持点击展开子批次 | ❌ 使用硬编码数据 |
| **ImpactAnalysisView** | `src/Client/Modules/MES.Modules.Trace/Views/ImpactAnalysisView.xaml` | DataGrid展示影响分析列表 | ⚠️ 基础符合 | 无根因选择、无影响范围计算、无导出 | 增加根因Lot输入框 → 自动计算影响批次范围 → 风险等级颜色标记 → 导出报告 | ❌ 使用硬编码数据 |
| **CustomerTraceReportView** | `src/Client/Modules/MES.Modules.Trace/Views/CustomerTraceReportView.xaml` | 占位页面"开发中..." | ❌ 不可用 | 完整功能缺失 | 客户追溯报告生成界面：选择客户+批次 → 生成包含正向/反向追溯+测试数据+检验记录的综合报告 → 导出PDF | ❌ 无实现 |

### 2.2 Yield模块（良率）

| View | 文件路径 | 当前功能 | 符合场景 | 缺失功能 | 优化建议 | 真实API |
|------|----------|----------|----------|----------|----------|---------|
| **YieldDashboardView** | `src/Client/Modules/MES.Modules.Yield/Views/YieldDashboardView.xaml` | KPI卡片+趋势图 | ⚠️ 基础符合 | 无筛选条件、无对比分析、无预警标记 | 增加产品/日期筛选；目标线对比；低于目标的KPI红色预警 | ❌ 使用YieldService硬编码数据 |
| **YieldTrendView** | `src/Client/Modules/MES.Modules.Yield/Views/YieldTrendView.xaml` | 良率趋势折线图 | ✅ 基本符合 | 无多产品对比、无工序维度下钻 | 支持多产品曲线叠加对比；点击数据点下钻到工序级别 | ❌ 使用硬编码数据 |
| **YieldAnalysisView** | `src/Client/Modules/MES.Modules.Yield/Views/YieldAnalysisView.xaml` | 良率分析面板 | ⚠️ 基础符合 | 无统计过程控制(SPC)分析 | 增加Cp/Cpk计算；控制图；异常点标记 | ❌ 使用硬编码数据 |
| **WaferMapView** | `src/Client/Modules/MES.Modules.Yield/Views/WaferMapView.xaml` | 晶圆地图展示 | ✅ 基本符合 | 无多Wafer对比、无Bin统计 | 支持多Wafer切换；侧边Bin分布统计；点击Die查看详情 | ❌ 使用硬编码数据 |
| **BinAnalysisView** | `src/Client/Modules/MES.Modules.Yield/Views/BinAnalysisView.xaml` | Bin分析 | ⚠️ 基础符合 | 无柏拉图(Pareto)分析 | 增加Bin柏拉图；Top3失效模式突出显示 | ❌ 使用硬编码数据 |
| **DefectAnalysisView** | `src/Client/Modules/MES.Modules.Yield/Views/DefectAnalysisView.xaml` | 缺陷分析 | ⚠️ 基础符合 | 无趋势分析 | 缺陷趋势图；缺陷分布饼图；关联工序分析 | ❌ 使用硬编码数据 |

### 2.3 ReportCenter模块（报表中心）

| View | 文件路径 | 当前功能 | 符合场景 | 缺失功能 | 优化建议 | 真实API |
|------|----------|----------|----------|----------|----------|---------|
| **DashboardView** | `src/Client/Modules/MES.Modules.ReportCenter/Views/DashboardView.xaml` | 全厂数据看板 | ⚠️ 基础符合 | 无时间维度切换、无对比分析 | 增加日/周/月/年切换；环比/同比对比 | 需确认是否已有聚合API |
| **ProductionReportView** | `src/Client/Modules/MES.Modules.ReportCenter/Views/ProductionReportView.xaml` | 生产报表 | ✅ 基本符合 | 无导出、无自动刷新 | 增加PDF/Excel导出按钮；自动刷新开关 | 需确认 |
| **QualityReportView** | `src/Client/Modules/MES.Modules.ReportCenter/Views/QualityReportView.xaml` | 质量报表 | ✅ 基本符合 | 无导出 | 增加导出功能；缺陷分类统计图 | 需确认 |
| **YieldReportView** | `src/Client/Modules/MES.Modules.ReportCenter/Views/YieldReportView.xaml` | 良率报表 | ✅ 基本符合 | 无导出、无趋势图 | 增加导出+趋势叠加 | 需确认 |
| **EquipmentReportView** | `src/Client/Modules/MES.Modules.ReportCenter/Views/EquipmentReportView.xaml` | 设备报表 | ✅ 基本符合 | 无OEE趋势图 | 增加OEE趋势、MTBF/MTTR统计 | 需确认 |
| **LotGenealogyReportView** | `src/Client/Modules/MES.Modules.ReportCenter/Views/LotGenealogyReportView.xaml` | 批次谱系报表 | ✅ 基本符合 | 无导出、无MFGID | 增加导出PDF；MFGID追溯信息 | 需确认 |

### 2.4 SystemSettings模块（系统设置）

| View | 文件路径 | 当前功能 | 符合场景 | 缺失功能 | 优化建议 | 真实API |
|------|----------|----------|----------|----------|----------|---------|
| **SystemParamView** | `src/Client/Modules/MES.Modules.SystemSettings/Views/SystemParamView.xaml` | 系统参数列表（已有SystemParameter模型） | ⚠️ 基础符合 | 无API、无热配置、无修改历史 | 增加搜索/分类筛选；修改时记录变更历史；重要参数修改需审批 | ❌ 无Controller/Service |
| **OperationLogView** | `src/Client/Modules/MES.Modules.SystemSettings/Views/OperationLogView.xaml` | 操作日志查看 | ✅ 基本符合 | 无筛选/导出 | 增加多维筛选（时间/用户/类型/模块）；导出CSV | 需确认 |
| **UserPermissionView** | `src/Client/Modules/MES.Modules.SystemSettings/Views/UserPermissionView.xaml` | 用户权限管理 | ✅ 基本符合 | 无角色权限矩阵视图 | 增加权限矩阵（行列交叉式）；批量权限操作 | 需确认 |
| **SystemMonitorView** | `src/Client/Modules/MES.Modules.SystemSettings/Views/SystemMonitorView.xaml` | 系统监控 | ⚠️ 基础符合 | 无实时监控、无告警 | 增加WebSocket实时推送；告警阈值设置 | ❌ 硬编码数据 |
| **SystemHealthView** | `src/Client/Modules/MES.Modules.SystemSettings/Views/SystemHealthView.xaml` | 系统健康报告 | ⚠️ 基础符合 | 无历史趋势、无报告导出 | 增加健康评分趋势图；导出PDF报告 | ❌ 硬编码数据 |
| **ExternalSystemView** | `src/Client/Modules/MES.Modules.SystemSettings/Views/ExternalSystemView.xaml` | 外部系统管理 | ✅ 基本符合 | 无测试连接、无同步日志 | 增加"测试连接"按钮；同步日志查看 | 需确认 |

---

## 三、追溯深化模块（改造）

### 3.1 现状分析

**已有数据库表（MesDbContext已定义）：**

| 实体类 | 表名 | 用途 |
|--------|------|------|
| ProdGenealogy | prod_genealogy | 批次血缘关系（parent/child lot） |
| ProdLotSplit | prod_lot_split | 批次拆分记录 |
| ProdLotMerge | prod_lot_merge | 批次合并记录 |
| ProdCarrierBinding | prod_carrier_binding | 载具绑定关系 |
| ProdOperationHistory | prod_operation_history | 操作历史 |
| ProdAuditTrail | prod_audit_trail | 审计追踪 |
| MfgUnit | mfg_unit | MFGID级追溯 |
| MfgOperationHistory | mfg_operation_history | MFGID操作历史 |
| MfgPackTrace | mfg_pack_trace | MFGID包装追溯 |
| LotTraceChain | lot_trace_chain | 追溯链路 |
| ExtSystemEvent | ext_system_event | 外部系统事件 |

**已有UI界面：**
- LotTraceView.xaml - 批次追溯列表
- ForwardTraceView.xaml - 正向追溯（仅占位）
- BackwardTraceView.xaml - 反向追溯（仅占位）
- GenealogyView.xaml - 血缘图谱（DataGrid，非树形）
- ImpactAnalysisView.xaml - 影响分析（基础DataGrid）
- CustomerTraceReportView.xaml - 客户追溯报告（仅占位）
- LotGenealogyReportView.xaml - 谱系报表

**核心问题：TraceService.cs 全部使用硬编码模拟数据，未对接真实API。**

### 3.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 说明 |
|----------|----------|--------|------|
| P5-T-01 | 批次正向追溯查询 | P0 | 给定LotId，查询其后所有工序/子批次/MFGID链路 |
| P5-T-02 | 批次反向追溯查询 | P0 | 给定LotId，向上追溯原材料/Wafer/父批次 |
| P5-T-03 | 完整追溯链路查询 | P0 | 正向+反向合并为完整追溯树 |
| P5-T-04 | MFGID级追溯 | P0 | 单个MFGID的全流程追溯（从Wafer到成品） |
| P5-T-05 | 影响分析引擎 | P1 | 给定问题根因，自动计算影响批次范围 |
| P5-T-06 | 谱系树可视化数据 | P1 | 为前端提供层级JSON结构 |
| P5-T-07 | 客户追溯报告生成 | P1 | 综合报告（正向+反向+测试+检验） |
| P5-T-08 | 追溯报告导出（PDF/Excel） | P1 | 标准格式导出 |

### 3.3 接口设计：TraceController

**路径前缀：** `api/v1/trace`

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class TraceController : ControllerBase
{
    // ========== 批次追溯 ==========

    /// <summary>正向追溯：查询Lot经过的所有工序及子批次</summary>
    [HttpGet("{lotId}/forward")]
    public async Task<ActionResult<ForwardTraceTreeDto>> GetForwardTrace(string lotId)

    /// <summary>反向追溯：查询Lot的原材料来源和父批次链路</summary>
    [HttpGet("{lotId}/backward")]
    public async Task<ActionResult<BackwardTraceTreeDto>> GetBackwardTrace(string lotId)

    /// <summary>完整追溯树：正向+反向合并</summary>
    [HttpGet("{lotId}/full-chain")]
    public async Task<ActionResult<GenealogyTreeDto>> GetFullTraceChain(string lotId)

    /// <summary>追溯链路列表（带分页/筛选）</summary>
    [HttpGet("lot-traces")]
    public async Task<ActionResult<PagedResult<LotTraceDto>>> GetLotTraces(
        [FromQuery] string? lotId,
        [FromQuery] string? productId,
        [FromQuery] string? status,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)

    // ========== MFGID追溯 ==========

    /// <summary>MFGID全流程追溯</summary>
    [HttpGet("mfg/{mfgId}")]
    public async Task<ActionResult<MfgTraceDto>> GetMfgTrace(string mfgId)

    /// <summary>批量MFGID追溯（根据LotId）</summary>
    [HttpGet("mfg/by-lot/{lotId}")]
    public async Task<ActionResult<List<MfgTraceDto>>> GetMfgTracesByLot(string lotId)

    /// <summary>MFGID包装追溯（Reel→Box→Pallet）</summary>
    [HttpGet("mfg/{mfgId}/pack-chain")]
    public async Task<ActionResult<PackChainDto>> GetMfgPackChain(string mfgId)

    // ========== 影响分析 ==========

    /// <summary>影响分析：给定根因批次，计算影响范围</summary>
    [HttpPost("impact-analysis")]
    public async Task<ActionResult<ImpactAnalysisResultDto>> RunImpactAnalysis(
        [FromBody] ImpactAnalysisRequest request)

    /// <summary>影响分析：给定问题设备+时间范围</summary>
    [HttpGet("impact-analysis/equipment/{equipmentId}")]
    public async Task<ActionResult<ImpactAnalysisResultDto>> ImpactAnalysisByEquipment(
        string equipmentId,
        [FromQuery] DateTime startTime,
        [FromQuery] DateTime endTime)

    // ========== 谱系图 ==========

    /// <summary>谱系树数据（JSON层级结构）</summary>
    [HttpGet("genealogy-tree/{lotId}")]
    public async Task<ActionResult<GenealogyTreeDto>> GetGenealogyTree(string lotId)

    /// <summary>谱系树批量查询（用于报表）</summary>
    [HttpGet("genealogy-tree/batch")]
    public async Task<ActionResult<List<GenealogyTreeDto>>> GetGenealogyTreesBatch(
        [FromQuery] string[] lotIds)

    // ========== 追溯报告 ==========

    /// <summary>生成客户追溯报告</summary>
    [HttpPost("report/customer-trace")]
    public async Task<ActionResult<CustomerTraceReportDto>> GenerateCustomerTraceReport(
        [FromBody] CustomerTraceReportRequest request)

    /// <summary>导出追溯报告（PDF）</summary>
    [HttpGet("report/{reportId}/export/pdf")]
    public async Task<FileResult> ExportTraceReportPdf(string reportId)

    /// <summary>导出追溯报告（Excel）</summary>
    [HttpGet("report/{reportId}/export/excel")]
    public async Task<FileResult> ExportTraceReportExcel(string reportId)

    // ========== 谱系报表 ==========

    /// <summary>批次谱系报表数据</summary>
    [HttpGet("report/genealogy")]
    public async Task<ActionResult<List<LotGenealogyReportDto>>> GetGenealogyReport(
        [FromQuery] string? lotId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? productId)
}
```

### 3.4 服务层设计：TraceService

```csharp
public class TraceService : ITraceService
{
    private readonly MesDbContext _context;

    // ========== 正向追溯 ==========
    public async Task<ForwardTraceTreeDto> GetForwardTraceAsync(string lotId)
    {
        var lot = await _context.ProdLots.FirstOrDefaultAsync(x => x.LotId == lotId);
        if (lot == null) throw new NotFoundException($"批次 {lotId} 不存在");

        var operations = await _context.ProdOperationHistories
            .Where(x => x.LotId == lotId)
            .OrderBy(x => x.StepSeq)
            .ToListAsync();

        var children = await GetChildLotsRecursiveAsync(lotId);

        var mfgUnits = await _context.MfgUnits
            .Where(x => x.LotId == lotId)
            .ToListAsync();

        return new ForwardTraceTreeDto
        {
            RootLot = MapLotDto(lot),
            Operations = operations.Select(MapOperationDto).ToList(),
            ChildLots = children,
            MfgUnits = mfgUnits.Select(MapMfgDto).ToList(),
            TotalChildLots = children.Count,
            TotalMfgUnits = mfgUnits.Count
        };
    }

    // ========== 反向追溯 ==========
    public async Task<BackwardTraceTreeDto> GetBackwardTraceAsync(string lotId)
    {
        var chain = new List<TraceNodeDto>();
        await BuildBackwardChainAsync(lotId, chain, depth: 0);

        var materials = await _context.MaterialConsumes
            .Where(x => x.LotId == lotId)
            .ToListAsync();

        var carriers = await _context.ProdCarrierBindings
            .Where(x => x.LotId == lotId)
            .OrderByDescending(x => x.BindTime)
            .ToListAsync();

        return new BackwardTraceTreeDto
        {
            RootLotId = lotId,
            ParentChain = chain,
            Materials = materials.Select(MapMaterialDto).ToList(),
            CarrierHistory = carriers.Select(MapCarrierDto).ToList(),
            MaxDepth = chain.Max(x => x.Depth)
        };
    }

    private async Task BuildBackwardChainAsync(string lotId, List<TraceNodeDto> chain, int depth)
    {
        var parents = await _context.ProdGenealogies
            .Where(x => x.ChildLotId == lotId)
            .ToListAsync();

        foreach (var parent in parents)
        {
            var parentLot = await _context.ProdLots
                .FirstOrDefaultAsync(x => x.LotId == parent.ParentLotId);

            chain.Add(new TraceNodeDto
            {
                LotId = parent.ParentLotId,
                RelationType = parent.RelationType,
                Depth = depth + 1,
                StepCode = parent.StepCode,
                Lot = parentLot != null ? MapLotDto(parentLot) : null
            });

            await BuildBackwardChainAsync(parent.ParentLotId, chain, depth + 1);
        }
    }

    // ========== 影响分析引擎 ==========
    public async Task<ImpactAnalysisResultDto> RunImpactAnalysisAsync(ImpactAnalysisRequest request)
    {
        var impactedLots = new HashSet<string>();
        var riskDetails = new List<ImpactRiskDetailDto>();

        // 1. 向下追溯所有子批次
        await FindDownstreamLotsAsync(request.RootLotId, impactedLots);

        // 2. 向上追溯所有父批次
        await FindUpstreamLotsAsync(request.RootLotId, impactedLots);

        // 3. 同设备同时间段的其他批次
        if (!string.IsNullOrEmpty(request.EquipmentId) && request.TimeRange.HasValue)
        {
            var sameEquipmentLots = await _context.ProdOperationHistories
                .Where(x => x.EquipmentId == request.EquipmentId
                    && x.CreatedAt >= request.TimeRange.Value.Start
                    && x.CreatedAt <= request.TimeRange.Value.End)
                .Select(x => x.LotId)
                .DistinctAsync();

            foreach (var lot in sameEquipmentLots)
                impactedLots.Add(lot);
        }

        // 4. 同原材料批次的其他批次
        if (!string.IsNullOrEmpty(request.MaterialBatchNo))
        {
            var sameMaterialLots = await _context.MaterialConsumes
                .Where(x => x.BatchNo == request.MaterialBatchNo)
                .Select(x => x.LotId)
                .DistinctAsync();

            foreach (var lot in sameMaterialLots)
                impactedLots.Add(lot);
        }

        foreach (var lotId in impactedLots)
        {
            var risk = await AssessLotRiskAsync(lotId, request);
            riskDetails.Add(risk);
        }

        return new ImpactAnalysisResultDto
        {
            RootLotId = request.RootLotId,
            RootCause = request.RootCause,
            ImpactedLotCount = impactedLots.Count,
            ImpactedMfgCount = await CountImpactedMfgUnitsAsync(impactedLots),
            RiskDetails = riskDetails.OrderByDescending(x => x.RiskLevel).ToList(),
            HighRiskCount = riskDetails.Count(x => x.RiskLevel == "High"),
            MediumRiskCount = riskDetails.Count(x => x.RiskLevel == "Medium"),
            LowRiskCount = riskDetails.Count(x => x.RiskLevel == "Low"),
            RecommendedActions = GenerateRecommendedActions(riskDetails)
        };
    }

    private async Task FindDownstreamLotsAsync(string lotId, HashSet<string> impacted)
    {
        impacted.Add(lotId);
        var children = await _context.ProdGenealogies
            .Where(x => x.ParentLotId == lotId)
            .ToListAsync();

        foreach (var child in children)
        {
            if (!impacted.Contains(child.ChildLotId))
                await FindDownstreamLotsAsync(child.ChildLotId, impacted);
        }
    }

    // ========== MFGID追溯 ==========
    public async Task<MfgTraceDto> GetMfgTraceAsync(string mfgId)
    {
        var mfg = await _context.MfgUnits.FirstOrDefaultAsync(x => x.MfgId == mfgId);
        if (mfg == null) throw new NotFoundException($"MFGID {mfgId} 不存在");

        var operations = await _context.MfgOperationHistories
            .Where(x => x.MfgId == mfgId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        var packChain = await _context.MfgPackTraces
            .Where(x => x.MfgId == mfgId)
            .ToListAsync();

        return new MfgTraceDto
        {
            MfgId = mfgId,
            LotId = mfg.LotId,
            RootWaferLotId = mfg.RootWaferLotId,
            WaferId = mfg.WaferId,
            DieCoordinate = $"({mfg.DieX},{mfg.DieY})",
            SerialNumber = mfg.SerialNumber,
            Status = mfg.Status,
            Grade = mfg.Grade,
            Operations = operations.Select(MapMfgOperationDto).ToList(),
            PackChain = packChain.Select(MapPackDto).ToList(),
            Lot = await GetLotBriefAsync(mfg.LotId)
        };
    }
}
```

### 3.5 核心DTO定义

```csharp
public class ForwardTraceTreeDto
{
    public LotBriefDto RootLot { get; set; }
    public List<OperationTraceDto> Operations { get; set; } = [];
    public List<LotBriefDto> ChildLots { get; set; } = [];
    public List<MfgBriefDto> MfgUnits { get; set; } = [];
    public int TotalChildLots { get; set; }
    public int TotalMfgUnits { get; set; }
}

public class BackwardTraceTreeDto
{
    public string RootLotId { get; set; }
    public List<TraceNodeDto> ParentChain { get; set; } = [];
    public List<MaterialTraceDto> Materials { get; set; } = [];
    public List<CarrierTraceDto> CarrierHistory { get; set; } = [];
    public int MaxDepth { get; set; }
}

public class TraceNodeDto
{
    public string LotId { get; set; }
    public string RelationType { get; set; }
    public int Depth { get; set; }
    public string? StepCode { get; set; }
    public LotBriefDto? Lot { get; set; }
    public List<TraceNodeDto> Children { get; set; } = [];
}

public class GenealogyTreeDto
{
    public string RootLotId { get; set; }
    public LotBriefDto RootLot { get; set; }
    public List<GenealogyTreeNodeDto> Children { get; set; } = [];
    public int TotalNodes { get; set; }
    public int MaxDepth { get; set; }
}

public class GenealogyTreeNodeDto
{
    public string LotId { get; set; }
    public string? LotStatus { get; set; }
    public string? ProductName { get; set; }
    public string RelationType { get; set; }
    public string? StepCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<GenealogyTreeNodeDto> Children { get; set; } = [];
}

public class MfgTraceDto
{
    public string MfgId { get; set; }
    public string LotId { get; set; }
    public string RootWaferLotId { get; set; }
    public string WaferId { get; set; }
    public string DieCoordinate { get; set; }
    public string SerialNumber { get; set; }
    public string Status { get; set; }
    public string Grade { get; set; }
    public List<MfgOperationDto> Operations { get; set; } = [];
    public List<PackDto> PackChain { get; set; } = [];
    public LotBriefDto Lot { get; set; }
}

public class ImpactAnalysisRequest
{
    public string RootLotId { get; set; } = string.Empty;
    public string RootCause { get; set; } = string.Empty;
    public string? EquipmentId { get; set; }
    public string? MaterialBatchNo { get; set; }
    public (DateTime Start, DateTime End)? TimeRange { get; set; }
}

public class ImpactAnalysisResultDto
{
    public string RootLotId { get; set; }
    public string RootCause { get; set; }
    public int ImpactedLotCount { get; set; }
    public int ImpactedMfgCount { get; set; }
    public List<ImpactRiskDetailDto> RiskDetails { get; set; } = [];
    public int HighRiskCount { get; set; }
    public int MediumRiskCount { get; set; }
    public int LowRiskCount { get; set; }
    public List<string> RecommendedActions { get; set; } = [];
}

public class ImpactRiskDetailDto
{
    public string LotId { get; set; }
    public string ProductName { get; set; }
    public string CurrentStep { get; set; }
    public string Status { get; set; }
    public int Qty { get; set; }
    public string RiskLevel { get; set; }
    public string RiskReason { get; set; }
    public string RecommendedAction { get; set; }
}
```

### 3.6 UI改造建议

#### 3.6.1 ForwardTraceView.xaml 改造

```xml
<Grid Margin="24">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>  <!-- 搜索栏 -->
        <RowDefinition Height="Auto"/>  <!-- 结果概要 -->
        <RowDefinition Height="*"/>     <!-- 追溯链Timeline -->
    </Grid.RowDefinitions>

    <StackPanel Orientation="Horizontal" Margin="0,0,0,16">
        <TextBox Width="200" Text="{Binding SearchLotId}" />
        <Button Content="查询" Command="{Binding SearchCommand}" />
        <Button Content="导出PDF" Command="{Binding ExportPdfCommand}" />
        <Button Content="导出Excel" Command="{Binding ExportExcelCommand}" />
    </StackPanel>

    <ItemsControl ItemsSource="{Binding TraceSteps}" Grid.Row="2">
        <ItemsControl.ItemsPanel>
            <ItemsPanelTemplate><StackPanel /></ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
        <ItemsControl.ItemTemplate>
            <DataTemplate>
                <Border Style="{StaticResource StepCardStyle}">
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto"/>  <!-- 步骤序号+连接线 -->
                            <ColumnDefinition Width="*"/>     <!-- 工序详情 -->
                            <ColumnDefinition Width="Auto"/>  <!-- Pass/Fail徽章 -->
                        </Grid.ColumnDefinitions>
                    </Grid>
                </Border>
            </DataTemplate>
        </ItemsControl.ItemTemplate>
    </ItemsControl>
</Grid>
```

#### 3.6.2 BackwardTraceView.xaml 改造

```xml
<Grid Margin="24">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*"/>  <!-- 左侧：原材料追溯链 -->
        <ColumnDefinition Width="*"/>  <!-- 右侧：父批次谱系 -->
    </Grid.ColumnDefinitions>

    <TreeView ItemsSource="{Binding MaterialChain}" />
    <TreeView ItemsSource="{Binding ParentHierarchy}" Grid.Column="1" />
</Grid>
```

#### 3.6.3 GenealogyView.xaml 改造（树形可视化）

```xml
<Grid Margin="16">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>

    <StackPanel Orientation="Horizontal">
        <TextBox Width="200" Text="{Binding SearchLotId}" />
        <Button Content="查询谱系" Command="{Binding SearchCommand}" />
        <ComboBox ItemsSource="{Binding DisplayModes}" SelectedItem="{Binding DisplayMode}" />
    </StackPanel>

    <TreeView x:Name="GenealogyTree" Grid.Row="1" ItemsSource="{Binding TreeNodes}">
        <TreeView.ItemTemplate>
            <HierarchicalDataTemplate ItemsSource="{Binding Children}">
                <StackPanel Orientation="Horizontal">
                    <TextBlock Text="{Binding LotId}" FontWeight="Bold"/>
                    <TextBlock Text="{Binding ProductName}" Margin="8,0,0,0" Foreground="Gray"/>
                    <TextBlock Text="{Binding Status}" Margin="8,0,0,0"/>
                    <TextBlock Text="{Binding RelationType}" Margin="8,0,0,0" FontSize="10"/>
                </StackPanel>
            </HierarchicalDataTemplate>
        </TreeView.ItemTemplate>
    </TreeView>
</Grid>
```

#### 3.6.4 ImpactAnalysisView.xaml 改造

```xml
<Grid Margin="24">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>

    <GroupBox Header="影响分析条件">
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="Auto"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            <TextBlock Text="根因批次：" VerticalAlignment="Center"/>
            <TextBox Grid.Column="1" Text="{Binding RootLotId}"/>
            <TextBlock Grid.Column="2" Text="问题设备：" Margin="16,0,0,0"/>
            <TextBox Grid.Column="3" Text="{Binding EquipmentId}"/>
            <Button Grid.Column="4" Content="运行分析" Command="{Binding AnalyzeCommand}"/>
        </Grid>
    </GroupBox>

    <StackPanel Orientation="Horizontal" Grid.Row="1" Margin="0,16">
        <Border Background="Red" Foreground="White" Padding="12,6">
            <TextBlock Text="{Binding HighRiskCount, StringFormat='高风险: {0}'}"/>
        </Border>
        <Border Background="Orange" Foreground="White" Padding="12,6" Margin="8,0">
            <TextBlock Text="{Binding MediumRiskCount, StringFormat='中风险: {0}'}"/>
        </Border>
        <Border Background="Green" Foreground="White" Padding="12,6">
            <TextBlock Text="{Binding LowRiskCount, StringFormat='低风险: {0}'}"/>
        </Border>
    </StackPanel>

    <DataGrid Grid.Row="2" ItemsSource="{Binding ImpactedLots}"
              RowStyle="{StaticResource RiskLevelRowStyle}" />
</Grid>
```

### 3.7 追溯报告导出

#### PDF报告结构

```
┌─────────────────────────────────────┐
│          批次追溯报告                  │
│  报告编号: TR-2026-0606-001          │
│  生成时间: 2026-06-06 14:30:00       │
│  批次号: LOT-2026-0001               │
├─────────────────────────────────────┤
│ 一、批次基本信息                      │
│  产品: QFN48 | 客户: XX电子           │
│  工单: WO-2026-0001 | 工艺: ASSY+FT   │
├─────────────────────────────────────┤
│ 二、正向追溯（工序链路）               │
│  步骤 | 工序 | 设备 | 时间 | 结果     │
│  10  | Dicing | WS-01 | ... | Pass  │
│  20  | Die Attach | DB-01 | ... | Pass│
├─────────────────────────────────────┤
│ 三、反向追溯（原材料来源）             │
│  WaferLot | 供应商 | 来料日期 | 结果  │
├─────────────────────────────────────┤
│ 四、测试数据                          │
│  测试项目 | 结果 | Bin分布             │
├─────────────────────────────────────┤
│ 五、检验记录                          │
│  检验类型 | 结果 | 检验员 | 日期        │
├─────────────────────────────────────┤
│ 六、MFGID追溯（抽样10条）             │
│  MFGID | Wafer | Die | 状态 | 等级    │
├─────────────────────────────────────┤
│ 七、异常记录（如有）                   │
│  类型 | 描述 | 处理 | 状态             │
└─────────────────────────────────────┘
```

#### Excel导出实现

```csharp
public class TraceReportExcelExporter
{
    public byte[] ExportForwardTrace(ForwardTraceTreeDto trace)
    {
        using var workbook = new XLWorkbook();

        // Sheet 1: 批次信息
        var ws1 = workbook.Worksheets.Add("批次信息");
        ws1.Cell(1, 1).Value = "批次号";
        ws1.Cell(1, 2).Value = trace.RootLot.LotId;
        ws1.Cell(2, 1).Value = "产品";
        ws1.Cell(2, 2).Value = trace.RootLot.ProductName;

        // Sheet 2: 正向追溯
        var ws2 = workbook.Worksheets.Add("正向追溯");
        ws2.Cell(1, 1).Value = "步骤";
        ws2.Cell(1, 2).Value = "工序";
        ws2.Cell(1, 3).Value = "设备";
        ws2.Cell(1, 4).Value = "配方";
        ws2.Cell(1, 5).Value = "进站时间";
        ws2.Cell(1, 6).Value = "出站时间";
        ws2.Cell(1, 7).Value = "操作员";
        ws2.Cell(1, 8).Value = "结果";

        for (int i = 0; i < trace.Operations.Count; i++)
        {
            var row = i + 2;
            var op = trace.Operations[i];
            ws2.Cell(row, 1).Value = op.StepSeq;
            ws2.Cell(row, 2).Value = op.StepName;
            ws2.Cell(row, 3).Value = op.EquipmentId;
            ws2.Cell(row, 4).Value = op.RecipeId;
            ws2.Cell(row, 5).Value = op.StartTime;
            ws2.Cell(row, 6).Value = op.EndTime;
            ws2.Cell(row, 7).Value = op.OperatorName;
            ws2.Cell(row, 8).Value = op.Result;
            ws2.Cell(row, 8).Style.Font.FontColor = op.Result == "Pass" ? XLColor.Green : XLColor.Red;
        }

        // Sheet 3-5: 子批次、MFGID、原材料

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
```

---

## 四、全厂KPI看板模块（改造）

### 4.1 现状分析

- **DashboardView.xaml** 存在于 ReportCenter 模块
- **所有数据为硬编码模拟数据**，无真实API对接
- 缺少：OEE计算、交付达成率、设备利用率、人均产出等关键指标
- 缺少：时间维度切换（日/周/月/年）、环比/同比分析

### 4.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 说明 |
|----------|----------|--------|------|
| P5-K-01 | 全厂KPI聚合查询 | P0 | 从多表聚合计算核心KPI |
| P5-K-02 | OEE计算引擎 | P0 | 设备综合效率 = 可用率 × 性能率 × 良率 |
| P5-K-03 | 交付达成率 | P0 | 按期交付工单数 / 总工单数 |
| P5-K-04 | 人均产出 | P1 | 产出数量 / 投入人时 |
| P5-K-05 | 设备利用率 | P1 | 运行时间 / (运行+待机+故障) |
| P5-K-06 | 工单完成率 | P1 | 完成工单 / 计划工单 |
| P5-K-07 | KPI趋势分析 | P1 | 日/周/月趋势对比 |
| P5-K-08 | 异常KPI告警 | P1 | 低于目标的指标高亮 |

### 4.3 接口设计：KpiController

**路径前缀：** `api/v1/kpi`

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class KpiController : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<FactoryDashboardDto>> GetDashboard(
        [FromQuery] KpiPeriod period = KpiPeriod.Today,
        [FromQuery] DateTime? date = null)

    [HttpGet("oee")]
    public async Task<ActionResult<OeeDashboardDto>> GetOeeDashboard(
        [FromQuery] string? equipmentGroup,
        [FromQuery] KpiPeriod period = KpiPeriod.Today)

    [HttpGet("oee/{equipmentId}")]
    public async Task<ActionResult<EquipmentOeeDetailDto>> GetEquipmentOee(
        string equipmentId,
        [FromQuery] DateTime date)

    [HttpGet("delivery")]
    public async Task<ActionResult<DeliveryKpiDto>> GetDeliveryKpi(
        [FromQuery] KpiPeriod period = KpiPeriod.ThisMonth)

    [HttpGet("productivity")]
    public async Task<ActionResult<ProductivityKpiDto>> GetProductivityKpi(
        [FromQuery] string? department,
        [FromQuery] KpiPeriod period = KpiPeriod.ThisMonth)

    [HttpGet("trend")]
    public async Task<ActionResult<List<KpiTrendPointDto>>> GetKpiTrend(
        [FromQuery] KpiMetric metric,
        [FromQuery] KpiPeriod period,
        [FromQuery] int months = 6)

    [HttpGet("yield-summary")]
    public async Task<ActionResult<YieldSummaryDto>> GetYieldSummary(
        [FromQuery] string? productId,
        [FromQuery] KpiPeriod period = KpiPeriod.Today)

    [HttpGet("equipment-utilization")]
    public async Task<ActionResult<EquipmentUtilizationDto>> GetEquipmentUtilization(
        [FromQuery] string? equipmentGroup)
}
```

### 4.4 KPI指标定义与计算公式

#### 4.4.1 OEE（设备综合效率）

```
OEE = 可用率(Availability) × 性能率(Performance) × 良率(Quality)

可用率 = 实际运行时间 / 计划运行时间
       = (计划运行时间 - 停机时间) / 计划运行时间

性能率 = 实际产出 / 理论产能
       = (总加工数量 × 标准节拍) / 实际运行时间

良率 = 合格品数量 / 总加工数量

目标值：OEE ≥ 85% (世界级制造标准)
       可用率 ≥ 90%
       性能率 ≥ 95%
       良率 ≥ 99%
```

#### 4.4.2 交付达成率

```
交付达成率 = 按期交付工单数 / 应交付工单总数 × 100%

按期交付：实际完成日期 ≤ 计划完成日期的工单
应交付：计划完成日期在当前统计周期内的工单

目标值：≥ 95%
```

#### 4.4.3 人均产出

```
人均产出 = 总产出数量 / 投入人时

投入人时 = 出勤人数 × 工作时长（按班次计算）
总产出数量 = 统计周期内完成工序的总合格品数量

目标值：根据产品和工序定义基准值
```

#### 4.4.4 设备利用率

```
设备利用率 = 实际运行时间 / (实际运行时间 + 待机时间 + 故障停机时间 + 维护时间) × 100%

数据来源：
- 运行时间：prod_operation_history 中的进站到出站时间差
- 故障停机：equipment_failure 中的 downtime_minutes
- 维护时间：equipment_maintenance 中的 actual_hours × 60

目标值：≥ 90%
```

#### 4.4.5 工单完成率

```
工单完成率 = 完成工单数 / (完成工单数 + 进行中的工单数) × 100%

目标值：按周统计 ≥ 80%
```

#### 4.4.6 全厂KPI汇总DTO

```csharp
public class FactoryDashboardDto
{
    public int TotalWorkOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int InProgressOrders { get; set; }
    public double OrderCompletionRate { get; set; }
    public double DeliveryRate { get; set; }
    public double OverallYield { get; set; }
    public double TargetYield { get; set; }
    public bool YieldOnTarget => OverallYield >= TargetYield;
    public double CpYield { get; set; }
    public double FtYield { get; set; }
    public double AvgOee { get; set; }
    public double EquipmentUtilization { get; set; }
    public int EquipmentRunning { get; set; }
    public int EquipmentTotal { get; set; }
    public double EquipmentAvailability { get; set; }
    public double ProductivityPerCapita { get; set; }
    public double AvgCycleTime { get; set; }
    public int QualityAlerts { get; set; }
    public int OpenComplaints { get; set; }
    public int HoldLots { get; set; }
    public int ScrapQtyToday { get; set; }
    public double OrderCompletionRateChange { get; set; }
    public double YieldChange { get; set; }
    public double OeeChange { get; set; }
    public DateTime ReportDate { get; set; }
}
```

### 4.5 UI改造建议（DashboardView.xaml）

```xml
<Grid Margin="16">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>  <!-- 时间选择器+刷新 -->
        <RowDefinition Height="Auto"/>  <!-- KPI卡片行（8个指标） -->
        <RowDefinition Height="*"/>     <!-- 图表区域 -->
    </Grid.RowDefinitions>

    <StackPanel Orientation="Horizontal">
        <ComboBox ItemsSource="今天,本周,本月,本季度,本年" SelectedItem="{Binding Period}" />
        <DatePicker SelectedDate="{Binding SelectedDate}" />
        <Button Content="刷新" Command="{Binding RefreshCommand}" />
    </StackPanel>

    <WrapPanel Grid.Row="1" Margin="0,16">
        <local:KpiCardControl Title="工单完成率" Value="{Binding OrderCompletionRate}"
            Target="{Binding TargetCompletionRate}" Trend="{Direction}" />
        <local:KpiCardControl Title="交付达成率" ... />
        <local:KpiCardControl Title="综合良率" ... />
        <local:KpiCardControl Title="平均OEE" ... />
        <local:KpiCardControl Title="设备利用率" ... />
        <local:KpiCardControl Title="人均产出" ... />
        <local:KpiCardControl Title="质量告警" ... />
        <local:KpiCardControl Title="Hold批次" ... />
    </WrapPanel>

    <TabControl Grid.Row="2">
        <TabItem Header="OEE趋势"><lvc:CartesianChart Series="{Binding OeeSeries}" /></TabItem>
        <TabItem Header="良率趋势"><lvc:CartesianChart Series="{Binding YieldSeries}" /></TabItem>
        <TabItem Header="设备状态"><!-- 饼图 --></TabItem>
        <TabItem Header="工单进度"><!-- 甘特图 --></TabItem>
    </TabControl>
</Grid>
```

### 4.6 KPI后端服务聚合

```csharp
public class KpiService : IKpiService
{
    private readonly MesDbContext _context;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public async Task<FactoryDashboardDto> GetDashboardAsync(KpiPeriod period, DateTime? date)
    {
        var targetDate = date ?? DateTime.Today;
        var (start, end) = GetPeriodRange(period, targetDate);
        var cacheKey = $"kpi:dashboard:{period}:{targetDate:yyyyMMdd}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            var (startPrev, endPrev) = GetPeriodRange(period, targetDate, previous: true);

            return new FactoryDashboardDto
            {
                TotalWorkOrders = await _context.ProdWorkOrders
                    .CountAsync(x => x.CreatedAt >= start && x.CreatedAt < end),
                CompletedOrders = await _context.ProdWorkOrders
                    .CountAsync(x => x.Status == "Completed" && x.ActualEndDate >= start && x.ActualEndDate < end),
                InProgressOrders = await _context.ProdWorkOrders
                    .CountAsync(x => x.Status == "InProgress"),
                DeliveryRate = await CalcDeliveryRateAsync(start, end),
                OverallYield = await CalcOverallYieldAsync(start, end),
                TargetYield = 94.0,
                AvgOee = await CalcAvgOeeAsync(start, end),
                EquipmentRunning = await _context.MasterEquipments.CountAsync(x => x.Status == "Running"),
                EquipmentTotal = await _context.MasterEquipments.CountAsync(),
                HoldLots = await _context.ProdLots.CountAsync(x => x.Status == "Hold"),
                ReportDate = targetDate
            };
        });
    }

    private (DateTime start, DateTime end) GetPeriodRange(KpiPeriod period, DateTime date, bool previous = false)
    {
        var offset = previous ? -1 : 0;
        return period switch
        {
            KpiPeriod.Today => (date.AddDays(offset).Date, date.AddDays(offset + 1).Date),
            KpiPeriod.ThisWeek => (date.AddDays(offset * 7).StartOfWeek(), date.AddDays(offset * 7 + 7).StartOfWeek()),
            KpiPeriod.ThisMonth => (new DateTime(date.Year, date.Month, 1).AddMonths(offset),
                                    new DateTime(date.Year, date.Month, 1).AddMonths(offset + 1)),
            KpiPeriod.ThisQuarter => (date.StartOfQuarter().AddMonths(offset * 3),
                                      date.StartOfQuarter().AddMonths((offset + 1) * 3)),
            KpiPeriod.ThisYear => (new DateTime(date.Year, 1, 1).AddYears(offset),
                                   new DateTime(date.Year + offset + 1, 1, 1)),
            _ => (date.Date, date.AddDays(1).Date)
        };
    }
}
```

---

## 五、生产成本分析模块（新增）

### 5.1 现状分析

- **完全缺失**。无任何成本相关表、服务、UI
- 已有数据源：material_consume（材料消耗）、prod_operation_history（操作记录含工时）、equipment_failure（停机时间）

### 5.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 说明 |
|----------|----------|--------|------|
| P5-C-01 | 材料成本核算 | P0 | 按Lot/工序统计材料消耗成本 |
| P5-C-02 | 人工成本核算 | P0 | 按Lot/工序统计人工工时成本 |
| P5-C-03 | 设备成本核算 | P1 | 按Lot/工序统计设备运行成本 |
| P5-C-04 | 成本汇总 | P0 | 单Lot总成本 = 材料 + 人工 + 设备 |
| P5-C-05 | 单位成本计算 | P0 | 单位成本 = 总成本 / 合格品数量 |
| P5-C-06 | 成本趋势分析 | P1 | 产品/工序成本趋势 |
| P5-C-07 | 成本异常预警 | P1 | 超预算成本告警 |
| P5-C-08 | 成本报表导出 | P2 | Excel/PDF导出 |

### 5.3 数据表设计

#### 5.3.1 cost_material（材料成本表）

```sql
CREATE TABLE cost_material (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    lot_id          VARCHAR(50) NOT NULL,
    step_code       VARCHAR(50),
    material_id     VARCHAR(50) NOT NULL,
    material_name   VARCHAR(100),
    consumed_qty    DECIMAL(12,4) NOT NULL,
    unit            VARCHAR(20),
    unit_cost       DECIMAL(12,4) NOT NULL COMMENT '单价',
    total_cost      DECIMAL(14,4) NOT NULL COMMENT '消耗量 × 单价',
    batch_no        VARCHAR(50),
    consumed_at     DATETIME NOT NULL,
    operator_id     VARCHAR(50),
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_lot (lot_id),
    INDEX idx_material (material_id),
    INDEX idx_date (consumed_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='材料成本明细表';
```

#### 5.3.2 cost_labor（人工成本表）

```sql
CREATE TABLE cost_labor (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    lot_id          VARCHAR(50) NOT NULL,
    step_code       VARCHAR(50),
    employee_id     VARCHAR(50) NOT NULL,
    employee_name   VARCHAR(100),
    work_hours      DECIMAL(6,2) NOT NULL,
    hourly_rate     DECIMAL(10,2) NOT NULL,
    total_cost      DECIMAL(12,2) NOT NULL,
    shift           VARCHAR(20),
    work_date       DATE NOT NULL,
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_lot (lot_id),
    INDEX idx_employee (employee_id),
    INDEX idx_date (work_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='人工成本明细表';
```

#### 5.3.3 cost_equipment（设备成本表）

```sql
CREATE TABLE cost_equipment (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    lot_id          VARCHAR(50) NOT NULL,
    step_code       VARCHAR(50),
    equipment_id    VARCHAR(50) NOT NULL,
    equipment_name  VARCHAR(100),
    run_hours       DECIMAL(6,2) NOT NULL,
    hourly_cost     DECIMAL(10,2) NOT NULL COMMENT '设备时成本(折旧+能耗+维护)',
    total_cost      DECIMAL(12,2) NOT NULL,
    energy_kwh      DECIMAL(10,2),
    run_date        DATE NOT NULL,
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,

    INDEX idx_lot (lot_id),
    INDEX idx_equipment (equipment_id),
    INDEX idx_date (run_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='设备成本明细表';
```

#### 5.3.4 cost_summary（成本汇总表）

```sql
CREATE TABLE cost_summary (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    lot_id          VARCHAR(50) NOT NULL,
    product_id      VARCHAR(50),
    product_name    VARCHAR(100),
    order_id        VARCHAR(50),
    material_cost   DECIMAL(14,4) NOT NULL DEFAULT 0,
    labor_cost      DECIMAL(12,2) NOT NULL DEFAULT 0,
    equipment_cost  DECIMAL(12,2) NOT NULL DEFAULT 0,
    total_cost      DECIMAL(14,4) NOT NULL,
    total_qty       INT NOT NULL,
    good_qty        INT NOT NULL,
    unit_cost       DECIMAL(12,4) NOT NULL COMMENT '总成本 / 合格品数',
    cost_status     VARCHAR(20) DEFAULT 'Normal',
    budget_cost     DECIMAL(14,4),
    budget_variance DECIMAL(14,4),
    calculated_at   DATETIME NOT NULL,
    calculated_by   VARCHAR(50),
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at      DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    UNIQUE KEY uk_lot (lot_id),
    INDEX idx_product (product_id),
    INDEX idx_order (order_id),
    INDEX idx_date (calculated_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='批次成本汇总表';
```

### 5.4 接口设计：CostController

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class CostController : ControllerBase
{
    [HttpGet("lot/{lotId}")]
    public async Task<ActionResult<LotCostDetailDto>> GetLotCost(string lotId)

    [HttpGet("product-summary")]
    public async Task<ActionResult<List<ProductCostSummaryDto>>> GetProductCostSummary(
        [FromQuery] string? productId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)

    [HttpGet("material")]
    public async Task<ActionResult<List<MaterialCostDto>>> GetMaterialCosts(
        [FromQuery] string? lotId,
        [FromQuery] string? materialId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)

    [HttpGet("labor")]
    public async Task<ActionResult<List<LaborCostDto>>> GetLaborCosts(
        [FromQuery] string? lotId,
        [FromQuery] string? employeeId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)

    [HttpGet("equipment")]
    public async Task<ActionResult<List<EquipmentCostDto>>> GetEquipmentCosts(
        [FromQuery] string? lotId,
        [FromQuery] string? equipmentId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)

    [HttpGet("trend")]
    public async Task<ActionResult<List<CostTrendDto>>> GetCostTrend(
        [FromQuery] string productId,
        [FromQuery] int months = 6)

    [HttpGet("alerts")]
    public async Task<ActionResult<List<CostAlertDto>>> GetCostAlerts(
        [FromQuery] double thresholdPercent = 10.0)

    [HttpPost("calculate/{lotId}")]
    public async Task<ActionResult> CalculateLotCost(string lotId)
}
```

### 5.5 成本计算规则

```
单Lot总成本 = 材料成本 + 人工成本 + 设备成本

材料成本 = Σ(材料消耗量 × 单价)
人工成本 = Σ(工时 × 时薪)
设备成本 = Σ(运行时长 × 设备时成本)

设备时成本 = (设备折旧/月运行小时) + 能耗成本/小时 + 维护成本/小时

默认设备时成本参考：
- WS(晶圆锯切): ¥50/小时
- DB(固晶): ¥45/小时
- WB(焊线): ¥40/小时
- MP(molding): ¥55/小时
- TF(切筋): ¥30/小时
- TH(测试): ¥80/小时

单位成本 = 总成本 / 合格品数量
```

### 5.6 UI设计建议（新增 CostAnalysisView.xaml）

```xml
<Grid Margin="16">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>  <!-- 筛选栏 -->
        <RowDefinition Height="Auto"/>  <!-- 成本概览卡片 -->
        <RowDefinition Height="*"/>     <!-- 成本构成图表 -->
    </Grid.RowDefinitions>

    <StackPanel Orientation="Horizontal">
        <ComboBox ItemsSource="{Binding Products}" />
        <DatePicker SelectedDate="{Binding StartDate}" />
        <DatePicker SelectedDate="{Binding EndDate}" />
        <Button Content="查询" Command="{Binding SearchCommand}" />
    </StackPanel>

    <WrapPanel Grid.Row="1" Margin="0,16">
        <local:CostCard Title="材料成本" Value="{Binding TotalMaterialCost}" Percentage="{Binding MaterialPercent}" />
        <local:CostCard Title="人工成本" Value="{Binding TotalLaborCost}" Percentage="{Binding LaborPercent}" />
        <local:CostCard Title="设备成本" Value="{Binding TotalEquipmentCost}" Percentage="{Binding EquipmentPercent}" />
        <local:CostCard Title="总成本" Value="{Binding TotalCost}" />
        <local:CostCard Title="单位成本" Value="{Binding UnitCost}" Unit="元/件" />
    </WrapPanel>

    <TabControl Grid.Row="2">
        <TabItem Header="成本构成"><lvc:PieChart Series="{Binding CostCompositionSeries}" /></TabItem>
        <TabItem Header="成本趋势"><lvc:CartesianChart Series="{Binding CostTrendSeries}" /></TabItem>
        <TabItem Header="产品对比"><!-- 柱状图 --></TabItem>
        <TabItem Header="成本明细"><DataGrid ItemsSource="{Binding CostDetails}" /></TabItem>
    </TabControl>
</Grid>
```

---

## 六、人员绩效模块（新增）

### 6.1 现状分析

- **完全缺失**。已有基础数据：sys_user、sys_login_log、shift_schedule、prod_operation_history

### 6.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 说明 |
|----------|----------|--------|------|
| P5-P-01 | 操作员效率统计 | P0 | 单位时间产出、操作准确率 |
| P5-P-02 | 质量绩效 | P0 | 操作良率、报废率、返工率 |
| P5-P-03 | 出勤统计 | P1 | 出勤天数、迟到早退、加班时长 |
| P5-P-04 | 综合评分 | P1 | 效率×40% + 质量×40% + 出勤×20% |
| P5-P-05 | 班组对比 | P1 | 按班组/部门对比绩效 |
| P5-P-06 | 绩效排名 | P2 | 月度/季度排名 |
| P5-P-07 | 绩效报表 | P2 | 导出个人/班组绩效报表 |

### 6.3 数据表设计

#### 6.3.1 operator_performance（操作员绩效表）

```sql
CREATE TABLE operator_performance (
    id                  BIGINT AUTO_INCREMENT PRIMARY KEY,
    employee_id         VARCHAR(50) NOT NULL,
    employee_name       VARCHAR(100),
    department_id       VARCHAR(50),
    department_name     VARCHAR(100),
    shift               VARCHAR(20),
    performance_date    DATE NOT NULL,
    total_operations    INT DEFAULT 0,
    total_output_qty    INT DEFAULT 0,
    total_work_hours    DECIMAL(5,2) DEFAULT 0,
    output_per_hour     DECIMAL(8,2),
    efficiency_score    DECIMAL(5,2),
    pass_qty            INT DEFAULT 0,
    scrap_qty           INT DEFAULT 0,
    rework_qty          INT DEFAULT 0,
    quality_rate        DECIMAL(5,2),
    quality_score       DECIMAL(5,2),
    attendance_status   VARCHAR(20) DEFAULT 'Present',
    actual_hours        DECIMAL(5,2),
    overtime_hours      DECIMAL(5,2),
    attendance_score    DECIMAL(5,2),
    overall_score       DECIMAL(5,2) COMMENT '效率×40% + 质量×40% + 出勤×20%',
    rank                INT,
    remark              VARCHAR(255),
    created_at          DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at          DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    INDEX idx_employee (employee_id),
    INDEX idx_date (performance_date),
    INDEX idx_department (department_id),
    UNIQUE KEY uk_employee_date (employee_id, performance_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='操作员每日绩效表';
```

#### 6.3.2 performance_metrics（绩效指标配置表）

```sql
CREATE TABLE performance_metrics (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    metric_code     VARCHAR(50) NOT NULL,
    metric_name     VARCHAR(100) NOT NULL,
    category        VARCHAR(50) NOT NULL COMMENT 'Efficiency/Quality/Attendance',
    weight          DECIMAL(5,2) NOT NULL,
    target_value    DECIMAL(10,2),
    calculation_formula VARCHAR(500),
    is_active       BOOLEAN DEFAULT TRUE,
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at      DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    UNIQUE KEY uk_code (metric_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='绩效指标配置表';

-- 初始数据
INSERT INTO performance_metrics (metric_code, metric_name, category, weight, target_value) VALUES
('EFF_OUTPUT', '每小时产出', 'Efficiency', 25, 100),
('EFF_ACCURACY', '操作准确率', 'Efficiency', 15, 99.5),
('QUAL_YIELD', '操作良率', 'Quality', 25, 99.0),
('QUAL_SCRAP', '报废率', 'Quality', 10, 0.5),
('QUAL_REWORK', '返工率', 'Quality', 5, 1.0),
('ATTEND_RATE', '出勤率', 'Attendance', 15, 98),
('ATTEND_OT', '加班配合度', 'Attendance', 5, 100);
```

### 6.4 接口设计：PerformanceController

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class PerformanceController : ControllerBase
{
    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<EmployeePerformanceDto>> GetEmployeePerformance(
        string employeeId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)

    [HttpGet("department")]
    public async Task<ActionResult<List<DepartmentPerformanceDto>>> GetDepartmentPerformance(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)

    [HttpGet("shift-comparison")]
    public async Task<ActionResult<List<ShiftComparisonDto>>> GetShiftComparison(
        [FromQuery] DateTime date)

    [HttpGet("ranking")]
    public async Task<ActionResult<List<EmployeeRankingDto>>> GetPerformanceRanking(
        [FromQuery] string? department,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int topN = 10)

    [HttpGet("attendance")]
    public async Task<ActionResult<AttendanceSummaryDto>> GetAttendanceSummary(
        [FromQuery] string? employeeId,
        [FromQuery] DateTime month)

    [HttpPost("calculate")]
    public async Task<ActionResult> CalculatePerformance(
        [FromBody] PerformanceCalculationRequest request)
}
```

### 6.5 绩效计算规则

```
综合评分 = 效率得分 × 40% + 质量得分 × 40% + 出勤得分 × 20%

效率得分 = min(100, 每小时产出 / 目标产出 × 100)
质量得分 = 操作良率 - 报废扣分(每个报废扣0.1分，最高扣20分)
出勤得分 = 出勤状态评分（Present=100, Late=80, Absent=0）
```

### 6.6 UI设计建议（新增 PerformanceView.xaml）

```xml
<Grid Margin="16">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>

    <StackPanel Orientation="Horizontal">
        <ComboBox ItemsSource="{Binding Departments}" />
        <DatePicker SelectedDate="{Binding StartDate}" />
        <DatePicker SelectedDate="{Binding EndDate}" />
        <Button Content="查询" Command="{Binding SearchCommand}" />
        <Button Content="计算绩效" Command="{Binding CalculateCommand}" />
    </StackPanel>

    <WrapPanel Grid.Row="1" Margin="0,16">
        <local:PerfCard Title="平均效率" Value="{Binding AvgEfficiency}" />
        <local:PerfCard Title="平均良率" Value="{Binding AvgQualityRate}" Unit="%" />
        <local:PerfCard Title="出勤率" Value="{Binding AttendanceRate}" Unit="%" />
        <local:PerfCard Title="综合评分" Value="{Binding AvgOverallScore}" />
    </WrapPanel>

    <TabControl Grid.Row="2">
        <TabItem Header="员工排名">
            <DataGrid ItemsSource="{Binding Rankings}">
                <DataGrid.Columns>
                    <DataGridTextColumn Header="排名" Binding="{Binding Rank}" Width="60"/>
                    <DataGridTextColumn Header="姓名" Binding="{Binding EmployeeName}" Width="80"/>
                    <DataGridTextColumn Header="部门" Binding="{Binding Department}" Width="100"/>
                    <DataGridTextColumn Header="效率" Binding="{Binding EfficiencyScore}" Width="80"/>
                    <DataGridTextColumn Header="质量" Binding="{Binding QualityScore}" Width="80"/>
                    <DataGridTextColumn Header="出勤" Binding="{Binding AttendanceScore}" Width="80"/>
                    <DataGridTextColumn Header="综合" Binding="{Binding OverallScore}" Width="80"/>
                </DataGrid.Columns>
            </DataGrid>
        </TabItem>
        <TabItem Header="趋势分析"><lvc:CartesianChart Series="{Binding PerfTrendSeries}" /></TabItem>
        <TabItem Header="班组对比"><!-- 柱状图 --></TabItem>
        <TabItem Header="出勤统计"><DataGrid ItemsSource="{Binding AttendanceRecords}" /></TabItem>
    </TabControl>
</Grid>
```

---

## 七、NPI管理深化模块（完善）

### 7.1 现状分析

**已有：** NpiProject实体（npi_project表），字段：project_id, project_name, customer_id, product_id, status, phase, start_date, target_completion, actual_completion, created_by, created_at
**缺失：** 无Service/Controller/UI

### 7.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 说明 |
|----------|----------|--------|------|
| P5-N-01 | NPI项目管理 | P0 | 创建/编辑/查询NPI项目 |
| P5-N-02 | NPI阶段流转 | P0 | Planning → Design → Prototype → Pilot → MP |
| P5-N-03 | 里程碑跟踪 | P1 | 各阶段关键里程碑完成情况 |
| P5-N-04 | 试产数据记录 | P1 | 试产批次关联、试产良率 |
| P5-N-05 | 问题追踪 | P1 | NPI阶段发现的问题记录与闭环 |
| P5-N-06 | NPI看板 | P2 | 项目进度可视化 |

### 7.3 NPI阶段定义与流转规则

```
阶段代码          描述
Planning         项目立项，需求定义，可行性评估
Design           工艺设计，路线定义，BOM确定
Prototype        原型试产，工艺验证
Pilot            小批量试产，良率爬坡
Qualification    可靠性测试，客户认证
MP               量产导入（Mass Production）

阶段流转规则：
Planning → Design:    需完成需求评审 + 可行性报告
Design → Prototype:   需完成工艺路线审批 + BOM确认
Prototype → Pilot:    需完成原型评估 + 工艺参数固化
Pilot → Qualification: 试产良率达标(≥目标良率-5%) + 连续3批次
Qualification → MP:  可靠性测试通过 + 客户认证通过
```

### 7.4 接口设计：NpiController

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class NpiController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<NpiProjectDto>>> GetProjects(
        [FromQuery] string? status,
        [FromQuery] string? phase)

    [HttpGet("{projectId}")]
    public async Task<ActionResult<NpiProjectDetailDto>> GetProject(string projectId)

    [HttpPost]
    public async Task<ActionResult> CreateProject([FromBody] CreateNpiProjectRequest request)

    [HttpPost("{projectId}/advance-phase")]
    public async Task<ActionResult> AdvancePhase(
        string projectId,
        [FromBody] AdvancePhaseRequest request)

    [HttpGet("{projectId}/milestones")]
    public async Task<ActionResult<List<NpiMilestoneDto>>> GetMilestones(string projectId)

    [HttpGet("{projectId}/pilot-data")]
    public async Task<ActionResult<List<NpiPilotDataDto>>> GetPilotData(string projectId)

    [HttpGet("{projectId}/issues")]
    public async Task<ActionResult<List<NpiIssueDto>>> GetIssues(string projectId)
}
```

### 7.5 NPI补充数据表

```sql
CREATE TABLE npi_milestone (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    project_id      VARCHAR(50) NOT NULL,
    milestone_name  VARCHAR(100) NOT NULL,
    phase           VARCHAR(20) NOT NULL,
    planned_date    DATE,
    actual_date     DATE,
    status          VARCHAR(20) DEFAULT 'Pending',
    description     TEXT,
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_project (project_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='NPI里程碑表';

CREATE TABLE npi_pilot_lot (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    project_id      VARCHAR(50) NOT NULL,
    lot_id          VARCHAR(50) NOT NULL,
    pilot_round     INT NOT NULL,
    yield_rate      DECIMAL(5,2),
    pass_qty        INT,
    total_qty       INT,
    issues          TEXT,
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uk_project_lot (project_id, lot_id),
    INDEX idx_project (project_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='NPI试产批次表';

CREATE TABLE npi_issue (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    project_id      VARCHAR(50) NOT NULL,
    issue_title     VARCHAR(200) NOT NULL,
    issue_type      VARCHAR(50),
    severity        VARCHAR(20),
    description     TEXT,
    status          VARCHAR(20) DEFAULT 'Open',
    assigned_to     VARCHAR(50),
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    resolved_at     DATETIME,
    resolution      TEXT,
    INDEX idx_project (project_id),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='NPI问题追踪表';
```

---

## 八、可靠性测试模块（新增）

### 8.1 现状分析

- **完全缺失**。半导体封测行业必须做可靠性测试（AEC-Q100汽车级、JEDEC工业级）

### 8.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 说明 |
|----------|----------|--------|------|
| P5-R-01 | 测试计划管理 | P0 | 创建可靠性测试计划 |
| P5-R-02 | 测试项目管理 | P0 | HTOL/HAST/TC/ESD等测试项目 |
| P5-R-03 | 测试结果记录 | P0 | 各测试项目结果录入 |
| P5-R-04 | 测试报告生成 | P1 | 可靠性测试综合报告 |
| P5-R-05 | 标准对照 | P1 | AEC-Q100/JEDEC标准对照 |
| P5-R-06 | 不合格追踪 | P1 | 可靠性不合格品的MRB处理 |

### 8.3 数据表设计

```sql
CREATE TABLE reliability_test_plan (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    plan_id         VARCHAR(50) NOT NULL,
    plan_name       VARCHAR(100) NOT NULL,
    product_id      VARCHAR(50),
    product_name    VARCHAR(100),
    lot_id          VARCHAR(50),
    standard        VARCHAR(50) NOT NULL COMMENT 'AEC-Q100/JEDEC/Custom',
    grade           VARCHAR(20),
    test_cycle      VARCHAR(20),
    planned_date    DATE,
    actual_date     DATE,
    status          VARCHAR(20) DEFAULT 'Planned',
    sample_size     INT,
    created_by      VARCHAR(50),
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    completed_at    DATETIME,
    UNIQUE KEY uk_plan_id (plan_id),
    INDEX idx_product (product_id),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='可靠性测试计划表';

CREATE TABLE reliability_test_item (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    plan_id         VARCHAR(50) NOT NULL,
    item_code       VARCHAR(50) NOT NULL COMMENT 'HTOL/HAST/TC/ESD/LU/HTGB',
    item_name       VARCHAR(100) NOT NULL,
    standard        VARCHAR(50),
    clause          VARCHAR(20) COMMENT '条款号',
    condition_desc  VARCHAR(255),
    duration_hours  INT,
    temperature     VARCHAR(50),
    humidity        VARCHAR(50),
    sample_size     INT,
    pass_criteria   VARCHAR(255),
    result          VARCHAR(20),
    tested_by       VARCHAR(50),
    tested_at       DATETIME,
    remark          TEXT,
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_plan (plan_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='可靠性测试项目表';

CREATE TABLE reliability_test_result (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    item_id         BIGINT NOT NULL,
    sample_id       VARCHAR(50),
    test_parameter  VARCHAR(50),
    initial_value   DECIMAL(12,4),
    final_value     DECIMAL(12,4),
    limit_value     DECIMAL(12,4),
    is_pass         BOOLEAN,
    measurement_unit VARCHAR(20),
    tested_at       DATETIME,
    tested_by       VARCHAR(50),
    remark          VARCHAR(255),
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_item (item_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='可靠性测试结果表';
```

### 8.4 AEC-Q100测试标准

| 测试代码 | 测试名称 | 测试条件 | 判定标准 |
|----------|----------|----------|----------|
| HTOL | 高温工作寿命 | 125°C/1000h | 失效数=0 |
| HAST | 高加速应力测试 | 130°C/85%RH/96h | 无失效 |
| TC | 温度循环 | -55°C~125°C/1000次 | 无开裂/分层 |
| ESD | 静电放电 | HBM 2kV/MM 200V | 无损伤 |
| LU | 闩锁效应 | ±100mA/25°C | 无闩锁 |
| HTGB | 高温栅偏 | 150°C/Vg/168h | 参数漂移<10% |

### 8.5 接口设计：ReliabilityController

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class ReliabilityController : ControllerBase
{
    [HttpGet("plans")]
    public async Task<ActionResult<List<ReliabilityTestPlanDto>>> GetPlans(
        [FromQuery] string? status,
        [FromQuery] string? productId)

    [HttpPost("plans")]
    public async Task<ActionResult> CreatePlan([FromBody] CreateTestPlanRequest request)

    [HttpGet("plans/{planId}/items")]
    public async Task<ActionResult<List<TestItemDto>>> GetTestItems(string planId)

    [HttpPost("items/{itemId}/result")]
    public async Task<ActionResult> SubmitTestResult(
        string itemId,
        [FromBody] SubmitTestResultRequest request)

    [HttpGet("plans/{planId}/report")]
    public async Task<ActionResult<ReliabilityReportDto>> GenerateReport(string planId)

    [HttpGet("standards")]
    public async Task<ActionResult<List<ReliabilityStandardDto>>> GetStandards(
        [FromQuery] string standard = "AEC-Q100")
}
```

---

## 九、报表自动化模块（新增）

### 9.1 现状分析

- **完全缺失**。无定时任务、报表模板、自动导出功能

### 9.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 说明 |
|----------|----------|--------|------|
| P5-A-01 | 报表模板管理 | P0 | 定义报表格式、内容、数据源 |
| P5-A-02 | 定时任务配置 | P0 | 生成频率、生成时间、输出格式 |
| P5-A-03 | 自动分发 | P1 | 邮件/飞书/文件共享推送 |
| P5-A-04 | 报表实例管理 | P0 | 历史报表查看/下载 |
| P5-A-05 | 预定义报表 | P0 | 生产日报、良率周报、质量月报 |

### 9.3 数据表设计

```sql
CREATE TABLE report_template (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    template_id     VARCHAR(50) NOT NULL,
    template_name   VARCHAR(100) NOT NULL,
    template_type   VARCHAR(50) NOT NULL,
    description     TEXT,
    data_source     VARCHAR(255) NOT NULL,
    parameters      JSON,
    layout_config   JSON,
    output_formats  JSON COMMENT '["PDF","Excel","CSV"]',
    is_active       BOOLEAN DEFAULT TRUE,
    created_by      VARCHAR(50),
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at      DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    UNIQUE KEY uk_template_id (template_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='报表模板表';

CREATE TABLE report_schedule (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    schedule_id     VARCHAR(50) NOT NULL,
    template_id     VARCHAR(50) NOT NULL,
    schedule_name   VARCHAR(100) NOT NULL,
    cron_expression VARCHAR(50) NOT NULL,
    output_format   VARCHAR(20) NOT NULL DEFAULT 'PDF',
    recipients      JSON,
    delivery_method VARCHAR(20) DEFAULT 'Email',
    parameters      JSON,
    is_active       BOOLEAN DEFAULT TRUE,
    last_run_at     DATETIME,
    last_run_status VARCHAR(20),
    next_run_at     DATETIME,
    created_by      VARCHAR(50),
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uk_schedule_id (schedule_id),
    INDEX idx_template (template_id),
    INDEX idx_active (is_active)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='报表调度表';

CREATE TABLE report_instance (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    instance_id     VARCHAR(50) NOT NULL,
    schedule_id     VARCHAR(50),
    template_id     VARCHAR(50) NOT NULL,
    report_name     VARCHAR(200),
    file_path       VARCHAR(500),
    file_size       BIGINT,
    output_format   VARCHAR(20),
    generated_at    DATETIME NOT NULL,
    generated_by    VARCHAR(50),
    status          VARCHAR(20) DEFAULT 'Generated',
    delivery_status VARCHAR(20),
    error_message   TEXT,
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uk_instance_id (instance_id),
    INDEX idx_template (template_id),
    INDEX idx_date (generated_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='报表实例表';
```

### 9.4 预定义报表清单

| 报表名称 | 类型 | 频率 | Cron表达式 | 内容 |
|----------|------|------|------------|------|
| 生产日报 | Production | 每日08:00 | `0 8 * * *` | 昨日工单完成情况、产出、良率、报废 |
| 良率周报 | Yield | 每周一09:00 | `0 9 * * 1` | 本周各产品/工序良率趋势、异常分析 |
| 质量月报 | Quality | 每月1日09:00 | `0 9 1 * *` | 月度质量指标、客诉、不合格、CAPA |
| 设备月报 | Equipment | 每月1日09:00 | `0 9 1 * *` | 月度OEE、MTBF/MTTR、维护记录 |
| 成本月报 | Cost | 每月5日09:00 | `0 9 5 * *` | 月度成本分析、预算对比 |
| 合规季报 | Compliance | 每季度首月 | `0 9 1 1,4,7,10 *` | 季度合规状态、审计记录 |

### 9.5 定时任务实现

```csharp
public class ReportSchedulerService : IHostedService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ReportSchedulerService> _logger;
    private Timer? _timer;

    public Task StartAsync(CancellationToken ct)
    {
        _timer = new Timer(CheckSchedules, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    private async void CheckSchedules(object? state)
    {
        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MesDbContext>();
        var now = DateTime.Now;

        var dueSchedules = await context.Set<ReportSchedule>()
            .Where(s => s.IsActive && s.NextRunAt <= now)
            .ToListAsync();

        foreach (var schedule in dueSchedules)
        {
            try
            {
                await ExecuteScheduleAsync(scope.ServiceProvider, schedule);
                schedule.LastRunAt = now;
                schedule.LastRunStatus = "Success";
                schedule.NextRunAt = CalcNextRun(schedule.CronExpression, now);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "报表调度失败: {ScheduleId}", schedule.ScheduleId);
                schedule.LastRunStatus = "Failed";
                await context.SaveChangesAsync();
            }
        }
    }

    private async Task ExecuteScheduleAsync(IServiceProvider provider, ReportSchedule schedule)
    {
        var template = await provider.GetRequiredService<MesDbContext>()
            .Set<ReportTemplate>().FirstAsync(t => t.TemplateId == schedule.TemplateId);

        var reportService = provider.GetRequiredService<ReportGenerationService>();
        var data = await reportService.FetchReportDataAsync(template, schedule.Parameters);

        var fileBytes = schedule.OutputFormat switch
        {
            "PDF" => await reportService.GeneratePdfAsync(template, data),
            "Excel" => await reportService.GenerateExcelAsync(template, data),
            "CSV" => await reportService.GenerateCsvAsync(template, data),
            _ => throw new NotSupportedException()
        };

        var instanceId = $"RPT-{DateTime.Now:yyyyMMddHHmmss}-{schedule.ScheduleId}";
        var filePath = Path.Combine("Reports", $"{instanceId}.{schedule.OutputFormat.ToLower()}");
        await File.WriteAllBytesAsync(filePath, fileBytes);

        if (schedule.Recipients != null && schedule.Recipients.Any())
        {
            await reportService.DeliverAsync(schedule.DeliveryMethod, schedule.Recipients, filePath);
        }
    }

    private DateTime CalcNextRun(string cronExpression, DateTime from)
    {
        var cron = CronExpression.Parse(cronExpression);
        return cron.GetNextOccurrence(from) ?? from.AddHours(1);
    }
}
```

---

## 十、审计日志不可篡改模块（完善）

### 10.1 现状分析

**已有：** ProdAuditTrail表，含audit_id, entity_type, entity_id, action, operator_id, timestamp, before_state(JSON), after_state(JSON), reason, detail, signature_level, approved_by
**缺失：** 不可篡改性保护、无哈希链、无签名验证、无删除保护

### 10.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 说明 |
|----------|----------|--------|------|
| P5-A-01 | 哈希链保护 | P0 | 每条记录包含前一条记录的SHA-256哈希 |
| P5-A-02 | 数字签名 | P0 | 关键操作需数字签名 |
| P5-A-03 | 只读保护 | P0 | 审计记录不可UPDATE/DELETE |
| P5-A-04 | 完整性验证 | P1 | 定期校验哈希链完整性 |
| P5-A-05 | 归档策略 | P1 | 审计日志长期归档 |
| P5-A-06 | FDA合规 | P1 | 21 CFR Part 11电子签名/记录 |

### 10.3 数据表增强

```sql
ALTER TABLE prod_audit_trail
ADD COLUMN prev_hash VARCHAR(64) COMMENT '前一条记录的SHA-256哈希',
ADD COLUMN record_hash VARCHAR(64) NOT NULL COMMENT '本条记录的SHA-256哈希',
ADD COLUMN is_locked BOOLEAN DEFAULT FALSE,
ADD COLUMN digital_signature TEXT,
ADD COLUMN signature_algorithm VARCHAR(20) DEFAULT 'SHA256WithRSA',
ADD INDEX idx_hash (record_hash);
```

### 10.4 哈希链实现

```csharp
public class AuditTrailService
{
    public async Task CreateAuditEntryAsync(AuditEntry entry)
    {
        var lastEntry = await _context.ProdAuditTrails
            .OrderByDescending(x => x.Timestamp)
            .FirstOrDefaultAsync();

        var prevHash = lastEntry?.RecordHash
            ?? "0000000000000000000000000000000000000000000000000000000000000000";

        var content = JsonSerializer.Serialize(new
        {
            entry.EntityType, entry.EntityId, entry.Action,
            entry.OperatorId, entry.Timestamp,
            entry.BeforeState, entry.AfterState,
            PrevHash = prevHash
        });

        var recordHash = ComputeSha256Hash(content);

        var audit = new ProdAuditTrail
        {
            AuditId = $"AUD-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8]}",
            EntityType = entry.EntityType,
            EntityId = entry.EntityId,
            Action = entry.Action,
            OperatorId = entry.OperatorId,
            OperatorName = entry.OperatorName,
            Timestamp = entry.Timestamp,
            BeforeState = entry.BeforeState,
            AfterState = entry.AfterState,
            Reason = entry.Reason,
            Detail = entry.Detail,
            PrevHash = prevHash,
            RecordHash = recordHash,
            IsLocked = true
        };

        _context.ProdAuditTrails.Add(audit);
        await _context.SaveChangesAsync();
    }

    public async Task<AuditIntegrityResult> VerifyIntegrityAsync(DateTime? startDate = null)
    {
        var entries = await _context.ProdAuditTrails
            .Where(x => !startDate.HasValue || x.Timestamp >= startDate)
            .OrderBy(x => x.Timestamp)
            .ToListAsync();

        var expectedHash = "0000000000000000000000000000000000000000000000000000000000000000";
        var issues = new List<string>();

        foreach (var entry in entries)
        {
            if (entry.PrevHash != expectedHash)
                issues.Add($"审计记录 {entry.AuditId} 的PrevHash不匹配");

            var content = JsonSerializer.Serialize(new
            {
                entry.EntityType, entry.EntityId, entry.Action,
                entry.OperatorId, entry.Timestamp,
                entry.BeforeState, entry.AfterState,
                PrevHash = entry.PrevHash
            });

            var computedHash = ComputeSha256Hash(content);
            if (computedHash != entry.RecordHash)
                issues.Add($"审计记录 {entry.AuditId} 的RecordHash被篡改");

            expectedHash = entry.RecordHash;
        }

        return new AuditIntegrityResult
        {
            TotalEntries = entries.Count,
            HasIssues = issues.Any(),
            Issues = issues,
            VerifiedAt = DateTime.UtcNow
        };
    }

    private string ComputeSha256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
}
```

### 10.5 EF Core删除保护

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var auditEntries = ChangeTracker.Entries<ProdAuditTrail>()
        .Where(e => e.State == EntityState.Modified || e.State == EntityState.Deleted);

    foreach (var entry in auditEntries)
    {
        if (entry.State == EntityState.Deleted)
            throw new InvalidOperationException("审计记录禁止删除");

        if (entry.State == EntityState.Modified && entry.Entity.IsLocked)
            throw new InvalidOperationException($"已锁定的审计记录 {entry.Entity.AuditId} 禁止修改");
    }

    return await base.SaveChangesAsync(cancellationToken);
}
```

### 10.6 FDA 21 CFR Part 11 合规

| 要求 | 实现方式 |
|------|----------|
| 电子签名与手写签名等效 | ProdSignature表 + 多级签名审批 |
| 签名包含：签名人、日期时间、含义 | SignatureLevel + SignTime + Reason |
| 电子记录防篡改 | 哈希链 + 只读保护 |
| 操作审计追踪 | ProdAuditTrail完整记录before/after |
| 权限控制 | RBAC + PermissionConfirm |
| 记录保留 | 审计日志归档 + 最小保留期限配置 |

---

## 十一、系统配置优化模块（完善）

### 11.1 现状分析

**已有：** SystemParamView.xaml + SystemParameter模型
**缺失：** 无API/Service/热加载/修改历史

### 11.2 接口设计：SystemConfigController

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class SystemConfigController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Dictionary<string, List<ConfigDto>>>> GetAllConfigs()

    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<ConfigDto>>> GetByCategory(string category)

    [HttpGet("{configKey}")]
    public async Task<ActionResult<ConfigDto>> GetConfig(string configKey)

    [HttpPut("{configKey}")]
    public async Task<ActionResult> UpdateConfig(
        string configKey,
        [FromBody] UpdateConfigRequest request)

    [HttpPut("batch")]
    public async Task<ActionResult> BatchUpdate([FromBody] List<UpdateConfigRequest> requests)

    [HttpGet("export")]
    public async Task<FileResult> ExportConfigs([FromQuery] string? category)

    [HttpPost("import")]
    public async Task<ActionResult<ImportResult>> ImportConfigs(IFormFile file)

    [HttpGet("{configKey}/history")]
    public async Task<ActionResult<List<ConfigHistoryDto>>> GetHistory(string configKey)
}
```

### 11.3 数据表设计

```sql
CREATE TABLE IF NOT EXISTS sys_config (
    config_id       VARCHAR(50) PRIMARY KEY,
    config_key      VARCHAR(100) NOT NULL UNIQUE,
    config_name     VARCHAR(100) NOT NULL,
    config_value    TEXT,
    config_type     VARCHAR(20) DEFAULT 'String',
    category        VARCHAR(50) NOT NULL,
    description     VARCHAR(255),
    default_value   TEXT,
    is_system       BOOLEAN DEFAULT FALSE,
    is_visible      BOOLEAN DEFAULT TRUE,
    is_hot_reload   BOOLEAN DEFAULT TRUE,
    validation_rule VARCHAR(255),
    created_at      DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at      DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    updated_by      VARCHAR(50)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='系统配置表';

CREATE TABLE sys_config_history (
    id              BIGINT AUTO_INCREMENT PRIMARY KEY,
    config_key      VARCHAR(100) NOT NULL,
    old_value       TEXT,
    new_value       TEXT,
    changed_by      VARCHAR(50) NOT NULL,
    changed_at      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    reason          VARCHAR(255),
    INDEX idx_key (config_key),
    INDEX idx_date (changed_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='系统配置修改历史表';
```

### 11.4 热配置实现

```csharp
public class ConfigService : IConfigService
{
    private readonly MesDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, object> _hotConfig = new();

    public T GetConfig<T>(string key, T defaultValue = default!)
    {
        if (_hotConfig.TryGetValue(key, out var value))
            return (T)value;

        var cacheKey = $"config:{key}";
        return _cache.GetOrCreate(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            var config = _context.SysConfigs.FirstOrDefault(c => c.ConfigKey == key);
            if (config == null) return defaultValue;
            var typedValue = ConvertValue<T>(config.ConfigValue, config.ConfigType);
            _hotConfig[key] = typedValue;
            return typedValue;
        }) ?? defaultValue;
    }

    public async Task UpdateConfigAsync(string key, string newValue, string changedBy, string? reason = null)
    {
        var config = await _context.SysConfigs.FirstAsync(c => c.ConfigKey == key);
        var oldValue = config.ConfigValue;

        config.ConfigValue = newValue;
        config.UpdatedAt = DateTime.Now;
        config.UpdatedBy = changedBy;
        await _context.SaveChangesAsync();

        _context.SysConfigHistories.Add(new SysConfigHistory
        {
            ConfigKey = key, OldValue = oldValue, NewValue = newValue,
            ChangedBy = changedBy, Reason = reason
        });
        await _context.SaveChangesAsync();

        if (config.IsHotReload)
        {
            var typedValue = ConvertValue<object>(newValue, config.ConfigType);
            _hotConfig[key] = typedValue;
            _cache.Remove($"config:{key}");
        }
    }
}
```

### 11.5 预定义系统配置

```sql
INSERT INTO sys_config (config_id, config_key, config_name, config_value, config_type, category) VALUES
('CFG-001', 'system.lot.id.prefix', '批次号前缀', 'LOT', 'String', 'System'),
('CFG-002', 'system.lot.id.date.format', '批次号日期格式', 'yyyyMMdd', 'String', 'System'),
('CFG-003', 'system.lot.id.sequence.length', '批次号流水号长度', '4', 'Int', 'System'),
('CFG-004', 'production.yield.warning.threshold', '良率预警阈值', '94.0', 'String', 'Production'),
('CFG-005', 'production.yield.hold.threshold', '良率Hold阈值', '90.0', 'String', 'Production'),
('CFG-006', 'production.max.rework.count', '最大返工次数', '3', 'Int', 'Production'),
('CFG-007', 'equipment.oee.target', 'OEE目标值', '85.0', 'String', 'Equipment'),
('CFG-008', 'quality.spc.rule.enabled', 'SPC规则开关', 'true', 'Bool', 'Quality'),
('CFG-009', 'trace.export.format.default', '追溯报告默认格式', 'PDF', 'String', 'Trace'),
('CFG-010', 'archive.lot.days.threshold', '批次归档天数阈值', '90', 'Int', 'System'),
('CFG-011', 'audit.integrity.check.cron', '审计完整性检查频率', '0 2 * * 0', 'String', 'System');
```

---

## 十二、数据归档与性能优化

### 12.1 数据归档策略

#### 12.1.1 批次归档

**规则：** 批次状态 = Completed/Closed 且完成时间 > 90天

```csharp
public class LotArchiveService : IHostedService
{
    private readonly MesDbContext _context;
    private readonly ILogger<LotArchiveService> _logger;
    private Timer? _timer;

    public Task StartAsync(CancellationToken ct)
    {
        _timer = new Timer(ExecuteArchive, null, GetNextRunTime(), TimeSpan.FromDays(1));
        return Task.CompletedTask;
    }

    private async void ExecuteArchive(object? state)
    {
        var threshold = DateTime.Now.AddDays(-90);

        var lotsToArchive = await _context.ProdLots
            .Where(x => (x.Status == "Completed" || x.Status == "Closed")
                && x.UpdatedAt < threshold && !x.IsArchived)
            .Take(1000)
            .ToListAsync();

        foreach (var lot in lotsToArchive)
        {
            _context.ProdLotArchives.Add(new ProdLotArchive
            {
                LotId = lot.LotId,
                OrderId = lot.OrderId,
                ProductId = lot.ProductId,
                ProductName = lot.ProductName,
                RouteId = lot.RouteId,
                ProcessStage = lot.ProcessStage,
                Status = lot.Status,
                OriginalQty = lot.OriginalQty,
                TotalPassQty = lot.TotalPassQty,
                TotalScrapQty = lot.TotalScrapQty,
                TotalReworkQty = lot.TotalReworkQty,
                TotalHoldQty = lot.TotalHoldQty,
                FinalYield = lot.TotalPassQty > 0
                    ? 100.0m * lot.TotalPassQty / lot.OriginalQty : 0,
                Grade = lot.Grade,
                CompletedAt = lot.UpdatedAt,
                ArchivedAt = DateTime.Now,
                ArchivedBy = "System"
            });

            lot.IsArchived = true;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("归档 {Count} 个批次", lotsToArchive.Count);
    }
}
```

#### 12.1.2 归档数据范围

| 数据表 | 归档条件 | 归档操作 | 保留期 |
|--------|----------|----------|--------|
| prod_lot | Completed + 90天 | 快照到 prod_lot_archive | 主表90天，归档永久 |
| prod_operation_history | 关联批次已归档 | 快照到history分区 | 3年 |
| prod_audit_trail | 创建时间 > 1年 | 分区归档 | 7年 |
| prod_test_data | 关联批次已归档 | 快照到archive表 | 5年 |
| prod_alarm | 已处理 + 180天 | 清理成功记录 | 1年 |
| ext_system_event | 已发送 + 90天 | 清理成功记录 | 90天 |

### 12.2 缓存策略

#### 12.2.1 缓存层级

```
L1 - IMemoryCache（进程内，< 5分钟）
  ├── 系统配置、产品基本信息、工艺路线、设备状态、KPI看板数据

L2 - Redis（可选，分布式，< 30分钟）
  ├── 用户权限、菜单配置、频繁查询的报表数据、追溯链路缓存
```

#### 12.2.2 缓存失效策略

| 数据类型 | 缓存时长 | 失效触发条件 |
|----------|----------|--------------|
| 系统配置 | 5分钟 | 配置修改后立即失效 |
| 产品数据 | 30分钟 | 产品更新后失效 |
| 工艺路线 | 30分钟 | 路线审批/更新后失效 |
| 设备状态 | 1分钟 | 设备状态变更时实时更新（WebSocket） |
| KPI看板 | 5分钟 | 定时刷新 |
| 追溯链路 | 10分钟 | 批次操作后失效 |
| 用户权限 | 30分钟 | 权限变更后失效 |

### 12.3 查询优化

#### 12.3.1 建议新增索引

```sql
-- 追溯相关索引（高频查询）
CREATE INDEX idx_prod_operation_lot_step ON prod_operation_history(lot_id, step_seq);
CREATE INDEX idx_prod_operation_equip_time ON prod_operation_history(equipment_id, created_at);
CREATE INDEX idx_prod_genealogy_parent ON prod_genealogy(parent_lot_id);
CREATE INDEX idx_prod_genealogy_child ON prod_genealogy(child_lot_id);

-- 良率查询索引
CREATE INDEX idx_prod_test_lot_time ON prod_test_data(lot_id, start_time);
CREATE INDEX idx_prod_test_step_time ON prod_test_data(step_code, start_time);

-- 成本查询索引
CREATE INDEX idx_material_consume_lot_time ON material_consume(lot_id, consumed_at);
```

#### 12.3.2 分页优化（KeySet替代Offset）

```csharp
public async Task<PagedResult<LotDto>> GetLotsAsync(string? lastLotId, int pageSize)
{
    var query = _context.ProdLots.AsQueryable();

    if (!string.IsNullOrEmpty(lastLotId))
    {
        var lastCreatedAt = _context.ProdLots.First(l => l.LotId == lastLotId).CreatedAt;
        query = query.Where(x => x.CreatedAt < lastCreatedAt)
                     .OrderByDescending(x => x.CreatedAt);
    }

    var items = await query.OrderByDescending(x => x.CreatedAt)
        .Take(pageSize + 1).ToListAsync();

    var hasMore = items.Count > pageSize;
    if (hasMore) items.RemoveAt(items.Count - 1);

    return new PagedResult<LotDto>
    {
        Items = items.Select(MapToDto).ToList(),
        HasMore = hasMore,
        NextCursor = hasMore ? items.Last().LotId : null
    };
}
```

### 12.4 数据库维护计划

```sql
-- 每周执行
OPTIMIZE TABLE prod_operation_history;
OPTIMIZE TABLE prod_lot;
ANALYZE TABLE prod_operation_history;
ANALYZE TABLE prod_lot;

-- 慢查询日志配置（my.ini）
-- slow_query_log = 1
-- slow_query_log_file = /var/log/mysql/slow.log
-- long_query_time = 2
```

---

## 十三、合规报表

### 13.1 IATF16949 合规

| 要求 | MES实现 | 报表 |
|------|---------|------|
| 8.5.1 生产过程控制 | 工艺路线强制执行、参数验证 | 生产过程控制报告 |
| 8.5.2 标识与追溯 | 批次追溯、MFGID追溯 | 追溯能力报告 |
| 8.5.3 不合格品控制 | Hold/Scrap/MRB流程 | 不合格品报告 |
| 8.5.4 防护 | 载具管理、环境监控 | 防护合规报告 |
| 8.5.5 交付后活动 | 客诉8D、追溯报告 | 客诉处理报告 |
| 8.5.6 变更控制 | ECN流程、版本管理 | 变更控制报告 |
| 9.1 监视和测量 | SPC、良率监控、KPI | 质量趋势报告 |

### 13.2 AEC-Q100 合规

| 要求 | MES实现 | 报表 |
|------|---------|------|
| 应力测试记录 | 可靠性测试模块 | AEC-Q100测试报告 |
| 质量等级追溯 | MFGID级追溯 + Grade | 分级追溯报告 |
| 批次一致性 | 谱系追溯 | 批次一致性报告 |
| 参数记录 | ProdTestData、ProdAssembleData | 参数合规报告 |

### 13.3 ISO9001 合规

| 要求 | MES实现 | 报表 |
|------|---------|------|
| 7.5 文件化信息 | 审计日志、操作记录 | 文件化信息报告 |
| 8.1 运行规划与控制 | 工艺路线、工单管理 | 运行控制报告 |
| 8.2 产品要求 | 客户需求管理 | 客户需求满足报告 |
| 8.4 外部过程控制 | 供应商/来料检验 | 供应商质量报告 |
| 8.5 生产提供 | 全过程追溯 | 生产控制报告 |
| 8.6 放行 | 质量门控 | 放行记录报告 |
| 8.7 不合格输出 | 不合格品处理 | 不合格品报告 |
| 9.1 监视测量 | 质量指标/KPI | 质量绩效报告 |

### 13.4 合规报表清单

| 报表名称 | 标准 | 频率 | 内容 |
|----------|------|------|------|
| 追溯能力报告 | IATF16949 8.5.2 | 按需 | 批次追溯链路覆盖率、MFGID追溯完整性 |
| 不合格品报告 | IATF16949 8.5.3 | 月度 | 不合格品数量、原因分类、处置方式、CAPA |
| 变更控制报告 | IATF16949 8.5.6 | 按需 | ECN记录、影响评估、实施验证 |
| AEC-Q100测试报告 | AEC-Q100 | 按项目 | 各测试项目结果、条款对照、通过/失败 |
| 放行记录报告 | ISO9001 8.6 | 按需 | 质量门控通过记录、检验结果、放行人员 |
| 年度质量报告 | ISO9001/IATF16949 | 年度 | 全年质量趋势、客诉汇总、持续改进 |

---

## 十四、实施顺序与依赖

### 14.1 实施阶段

```
阶段A（核心基础，2-3周）
├── P5-S-01 系统配置API（被其他模块依赖）
├── P5-T-01~04 追溯核心API
├── P5-K-01 KPI聚合查询
└── P5-A-01~03 审计日志保护

阶段B（业务深化，3-4周）
├── P5-T-05~08 影响分析+报告导出
├── P5-K-02~08 OEE/交付/趋势等KPI
├── P5-C-01~05 生产成本分析
├── P5-P-01~04 人员绩效
└── P5-A-04 审计完整性验证

阶段C（新增模块，3-4周）
├── P5-R-01~06 可靠性测试
├── P5-N-01~06 NPI管理深化
├── P5-A-01~05 报表自动化（定时任务）
└── P5-S-02~05 热配置+导入导出

阶段D（收尾优化，1-2周）
├── P5-C-06~08 成本趋势+预警
├── P5-P-05~07 绩效排名+报表
├── 数据归档服务
├── 缓存策略部署
├── 索引优化
└── 合规报表
```

### 14.2 依赖关系图

```
系统配置API (P5-S) ──┐
                     ├──→ 所有模块（配置驱动）
审计保护 (P5-A) ────┘

追溯核心API (P5-T) ──┐
                     ├──→ KPI看板 (P5-K)  ← 追溯数据
                     ├──→ 合规报表 (P5-合规)
                     └──→ 报表自动化 (P5-Auto)

成本分析 (P5-C) ─────→ KPI看板 (单位成本指标)
人员绩效 (P5-P) ─────→ KPI看板 (人均产出指标)

可靠性测试 (P5-R) ──→ NPI阶段流转 (Qualification→MP)
NPI管理 (P5-N) ─────→ 报表自动化 (NPI进度报告)

数据归档 ───────────→ 不影响在线查询（独立服务）
缓存策略 ───────────→ 提升所有查询性能
索引优化 ───────────→ 提升所有查询性能
```

---

## 十五、验收标准

### 15.1 追溯模块验收

| 验收项 | 标准 | 测试方法 |
|--------|------|----------|
| 正向追溯 | 输入LotId，5秒内返回完整工序链路+子批次+MFGID | 自动化测试：随机选10个Lot验证 |
| 反向追溯 | 输入LotId，5秒内返回原材料+父批次链路 | 自动化测试：10个Lot验证 |
| MFGID追溯 | 输入MFGID，3秒内返回全流程追溯 | 自动化测试：20个MFGID验证 |
| 影响分析 | 输入根因批次，10秒内返回影响范围+风险评级 | 模拟问题批次验证 |
| 谱系树 | 返回层级JSON，前端Tree渲染无错误 | 前端UI测试 |
| 报告导出 | PDF/Excel格式正确，数据完整 | 手动抽查10份报告 |
| 追溯覆盖率 | 100%的Completed批次可追溯 | 数据库查询验证 |

### 15.2 KPI看板验收

| 验收项 | 标准 | 测试方法 |
|--------|------|----------|
| 数据准确性 | KPI计算结果与手工核算一致 | 随机抽取3个周期验证 |
| 响应时间 | Dashboard加载 < 3秒 | 性能测试 |
| 时间维度 | 日/周/月/季度/年切换正确 | 手动测试 |
| 环比计算 | 环比/同比数据准确 | 手动验证 |
| 缓存生效 | 5分钟内重复查询命中缓存 | 日志验证 |

### 15.3 成本分析验收

| 验收项 | 标准 | 测试方法 |
|--------|------|----------|
| 材料成本 | 与ERP/财务系统数据一致（误差<1%） | 对比验证 |
| 单位成本 | 总成本/合格品数计算正确 | 手工核算 |
| 成本预警 | 超预算10%自动告警 | 模拟超预算批次 |

### 15.4 人员绩效验收

| 验收项 | 标准 | 测试方法 |
|--------|------|----------|
| 效率统计 | 单位时间产出计算正确 | 手工验证 |
| 综合评分 | 公式：效率×40%+质量×40%+出勤×20% | 手工核算 |
| 排名 | 按综合评分正确排序 | 手动验证 |

### 15.5 系统优化验收

| 验收项 | 标准 | 测试方法 |
|--------|------|----------|
| 审计保护 | 尝试删除/修改审计记录被拒绝 | 自动化测试 |
| 哈希链 | 每周完整性检查通过 | 定期验证 |
| 缓存命中率 | > 80% | 监控指标 |
| 归档执行 | 每日自动归档，无数据丢失 | 日志+数据验证 |
| API响应 | P95 < 500ms | 压测（100并发） |

---

## 十六、数据库迁移计划

### 16.1 新增表清单（12张表）

| 序号 | 表名 | 模块 | 说明 |
|------|------|------|------|
| 1 | cost_material | 成本分析 | 材料成本明细 |
| 2 | cost_labor | 成本分析 | 人工成本明细 |
| 3 | cost_equipment | 成本分析 | 设备成本明细 |
| 4 | cost_summary | 成本分析 | 批次成本汇总 |
| 5 | operator_performance | 人员绩效 | 每日绩效记录 |
| 6 | performance_metrics | 人员绩效 | 指标配置 |
| 7 | npi_milestone | NPI管理 | 里程碑 |
| 8 | npi_pilot_lot | NPI管理 | 试产批次 |
| 9 | npi_issue | NPI管理 | 问题追踪 |
| 10 | reliability_test_plan | 可靠性测试 | 测试计划 |
| 11 | reliability_test_item | 可靠性测试 | 测试项目 |
| 12 | reliability_test_result | 可靠性测试 | 测试结果 |
| 13 | report_template | 报表自动化 | 报表模板 |
| 14 | report_schedule | 报表自动化 | 调度配置 |
| 15 | report_instance | 报表自动化 | 报表实例 |
| 16 | sys_config | 系统配置 | 配置参数 |
| 17 | sys_config_history | 系统配置 | 配置修改历史 |

### 16.2 表结构变更

| 表名 | 变更 | 说明 |
|------|------|------|
| prod_audit_trail | ADD: prev_hash, record_hash, is_locked, digital_signature, signature_algorithm | 审计保护 |
| sys_config | CREATE（如果不存在） | 系统配置 |

### 16.3 索引变更

| 表名 | 新增索引 |
|------|----------|
| prod_operation_history | idx_prod_operation_lot_step(lot_id, step_seq), idx_prod_operation_equip_time(equipment_id, created_at) |
| prod_genealogy | idx_prod_genealogy_parent(parent_lot_id), idx_prod_genealogy_child(child_lot_id) |
| prod_test_data | idx_prod_test_lot_time(lot_id, start_time), idx_prod_test_step_time(step_code, start_time) |
| material_consume | idx_material_consume_lot_time(lot_id, consumed_at) |
| prod_audit_trail | idx_hash(record_hash) |

### 16.4 迁移执行顺序

```
1. sys_config + sys_config_history (最先，配置驱动)
2. cost_* 四张表
3. operator_performance + performance_metrics
4. npi_milestone + npi_pilot_lot + npi_issue
5. reliability_test_plan + reliability_test_item + reliability_test_result
6. report_template + report_schedule + report_instance
7. prod_audit_trail ALTER TABLE
8. 新增索引（最后，避免阻塞写入）
```

### 16.5 初始数据

```sql
-- 绩效指标配置（7条）
-- 系统配置（11条）
-- 报表模板（6条：生产日报、良率周报、质量月报、设备月报、成本月报、合规季报）
-- 可靠性测试标准（AEC-Q100 6项标准条款）
```

### 16.6 回滚方案

- 所有DDL在事务中执行
- 新表创建前备份数据库
- 使用 EF Core Migration 或 Flyway 管理版本
- 每张新表创建后验证约束/索引
- ALTER TABLE 前验证列不存在

---

## 附录

### A. KPI枚举定义

```csharp
public enum KpiPeriod { Today, ThisWeek, ThisMonth, ThisQuarter, ThisYear }
public enum KpiMetric { Oee, Yield, Delivery, Productivity, Utilization, Completion }
```

### B. 追溯关系类型定义

```csharp
public static class RelationTypes
{
    public const string Split = "Split";          // 拆分
    public const string Merge = "Merge";          // 合并
    public const string Parent = "Parent";        // 父批次
    public const string Child = "Child";          // 子批次
    public const string Rework = "Rework";        // 返工
    public const string Assembly = "Assembly";    // 组装
}
```

### C. 风险等级定义

```csharp
public static class RiskLevels
{
    public const string High = "High";      // 已出货/客户影响
    public const string Medium = "Medium";  // 在制/可隔离
    public const string Low = "Low";        // 已完成/已检验
}
```

### D. 推荐NuGet包

| 包名 | 用途 | 版本 |
|------|------|------|
| ClosedXML | Excel导出 | ≥ 0.102 |
| QuestPDF | PDF生成 | ≥ 2024.3 |
| CsvHelper | CSV导出 | ≥ 33.0 |
| Cronos | Cron表达式解析 | ≥ 0.8 |
| MailKit | 邮件发送 | ≥ 4.3 |
| Microsoft.Extensions.Caching.Memory | 内存缓存 | 内置 |
| StackExchange.Redis | Redis缓存 | ≥ 2.7（可选） |

### E. 文件存储结构

```
Reports/
├── Daily/          # 日报
│   └── 2026/
│       └── 06/
├── Weekly/         # 周报
├── Monthly/        # 月报
├── Trace/          # 追溯报告
├── Reliability/    # 可靠性报告
└── Compliance/     # 合规报告
```

---

> **文档结束**
>
> 本文档为 MES 系统阶段五实施方案的完整规范，涵盖追溯深化、管理决策支持、系统优化三大领域。实施时应严格按照阶段A→B→C→D的顺序推进，确保依赖关系正确处理。
>
> **总计：**
> - 新增数据表：17张
> - 表结构变更：1张（prod_audit_trail）
> - 新增索引：7个
> - 新增API Controller：6个（Trace/Kpi/Cost/Performance/Npi/Reliability/SystemConfig/Report）
> - 新增/改造 UI View：20+个
> - 新增后台服务：3个（ReportScheduler/LotArchive/IntegrityCheck）
