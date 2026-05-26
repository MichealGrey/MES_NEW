# 半导体封装测试厂（OSAT）MES 适配改造方案

## 一、背景与目标

当前 MES 系统按**晶圆制造厂（Wafer Fab / 前段）**业务建模，包含光刻、蚀刻、CVD、CMP、离子注入等前段工序，使用 FOUP/光罩等 Fab 专有概念。

目标：将系统改造为符合**半导体封装测试厂（OSAT / 后段）**实际业务的 MES，覆盖从晶圆接收 → 晶圆切割 → 芯片贴装 → 引线键合 → 塑封 → 切筋成型 → 测试 → 包装出货的完整封测流程。

---

## 二、Fab 与 OSAT 核心差异对照

| 维度 | 当前 Fab 模型 | OSAT 应改为 |
|------|-------------|------------|
| **核心工序** | PHOTO, ETCH, CVD, CMP, IMP | WaferSaw, DieAttach, WireBond, Mold, Mark, Singulation, TrimForm, Test |
| **设备类型** | Lithography, Etch, CVD, PVD, Implant, CMP, Metrology, Cleaning | DieBonder, WireBonder, MoldingPress, DicingSaw, TestHandler, TrimForm, AOI, XRay |
| **载具/容器** | FOUP（前开式晶圆传送盒，25片/盒） | WaferFrame（晶圆贴膜框）, LeadFrame（引线框架）, Strip（条带）, Magazine（料管）, Tray（吸塑盘）, Reel（编带盘） |
| **追踪单位** | Wafer（晶圆）→ Die（管芯） | WaferFrame → Strip → SingulatedDie → PackagedUnit |
| **检验类型** | ADI, OCD, SEM, Defect, Metrology | VisualInspection, XRay, AOI, CrossSection, SAM, ElectricalTest |
| **良率模型** | Wafer Map（晶圆管芯图）+ Die Yield | Strip Yield（条带良率）+ Test Yield（测试良率）+ Package Yield（封装良率）+ Bin分布 |
| **物料类型** | 光阻、显影液、蚀刻液、CVD前驱体、CMP研磨液 | 芯片粘合剂（DAF/Epoxy）、引线框架、金线/铜线、塑封料（EMC）、锡球、编带 |
| **环境要求** | Class 1/10 超净间，粒子/温湿度极严 | Class 1K~10K，ESD防护更重要 |
| **光罩管理** | Reticle（光罩/掩膜板）—— Fab 核心资产 | 无光罩，改为**模具/工具管理**（MoldCavity, BondTool, SawBlade） |
| **测试环节** | Wafer Probing（晶圆探针测试，CP） | Final Test（成品测试，FT）+ Burn-in（老化测试）+ OS（开封目检） |
| **批次规模** | 25 wafers / FOUP | 按Strip（通常50~250颗/Strip），或按Reel/Tray |
| **Hold类型** | Engineering, Quality, Customer, Material, Equipment | 基本相同，增加 **YieldHold**（良率异常Hold）和 **DataHold**（数据缺失Hold） |

---

## 三、各模块改造详细方案

### 3.1 领域枚举改造（MES.Domain）

#### 3.1.1 EquipmentType（设备类型）

```
现有: Lithography, Etch, CVD, PVD, Implant, CMP, Metrology, Cleaning
改为:
  DicingSaw       // 切割机（晶圆切割/划片）
  DieBonder       // 贴片机（芯片贴装）
  WireBonder      // 焊线机（引线键合）
  MoldingPress    // 塑封机（模压封装）
  LaserMark       // 激光打标机
  TrimForm        // 切筋成型机
  TestHandler     // 测试分选机
  BurnIn          // 老化测试箱
  AOI             // 自动光学检测
  XRay            // X射线检测
  PlasmaClean     // 等离子清洗
  TapeReel        // 编带包装机
```

#### 3.1.2 InspectionType（检验类型）

```
现有: ADI, OCD, SEM, Defect, Metrology
改为:
  VisualInspect   // 外观目检
  XRay            // X射线检测（空洞/线弧/位移）
  AOI             // 自动光学检测
  CrossSection    // 切片分析
  SAM             // 声学扫描（分层/空洞）
  ElectricalTest  // 电性测试
  OpenShort       // 开短路测试
```

#### 3.1.3 新增枚举

```csharp
/// 封装类型
public enum PackageType
{
    QFP,        // 四方扁平封装
    BGA,        // 球栅阵列
    QFN,        // 四方无引脚扁平
    SOP,        // 小外形封装
    DIP,        // 双列直插
    CSP,        // 芯片级封装
    WLCSP,      // 晶圆级芯片封装
    SiP,        // 系统级封装
    TO,         // 插入式封装
    SOT         // 小外形晶体管
}

/// 载具类型
public enum CarrierType
{
    WaferFrame,   // 晶圆贴膜框
    LeadFrame,    // 引线框架
    Strip,        // 条带（已切筋前）
    Magazine,     // 料管
    Tray,         // 吸塑盘/托盘
    Reel,         // 编带盘
    JellyBean,    // 散装
    WafflePack    // 华夫盘
}

/// 测试类型
public enum TestType
{
    FinalTest,    // 成品测试（FT）
    BurnIn,       // 老化测试
    OS,           // 开封目检
    PreTest,      // 预测试
    SampleTest    // 抽检
}

/// 测试Bin分类
public enum BinCategory
{
    Bin1,     // 良品
    Bin2,     // 良品（备选等级）
    BinFail,  // 电性不良
    BinVisual // 外观不良
}

/// 切割方式
public enum DicingMethod
{
    Blade,     // 刀片切割
    Laser,     // 激光切割
  LaserStealth  // 隐形激光切割
}
```

#### 3.1.4 ProcessStatus 保留，HoldType 扩展

```
HoldType 新增:
  YieldHold    // 良率异常Hold（良率低于阈值自动触发）
  DataHold     // 数据缺失Hold（测试数据/检验结果未上传）
```

---

### 3.2 Production 模块（生产管理）

#### 3.2.1 工单模型 WorkOrderInfo 改造

| 字段 | 现有 | 改造 | 说明 |
|------|------|------|------|
| Device | ✅ | → `DieName` | 管芯名称（OSAT称Die不称Device） |
| TechNode | ✅ | → `PackageType` | Fab关注工艺节点，OSAT关注封装类型 |
| WaferQty | ✅ | → 改为双字段 `WaferQty` + `UnitQty` | 晶圆数 + 成品颗数 |
| RouteName | ✅ | 保留 | 工艺路线，但路线内容从Photo-Etch改为Saw-DA-WB-Mold-Test |
| — | 缺 | + `SubconLotId` | 委外批号（OSAT常见代工/委外场景） |
| — | 缺 | + `CustomerPN` | 客户料号 |
| — | 缺 | + `InternalPN` | 内部料号 |
| — | 缺 | + `TestProgram` | 测试程序版本号 |
| — | 缺 | + `BinSpec` | Bin规格定义（哪些Bin算良品） |
| — | 缺 | + `GradeSpec` | 等级分选规格 |
| — | 缺 | + `WaferSource` | 晶圆来源（来料晶圆厂/内部） |
| — | 缺 | + `TargetCPYield` | 目标封装良率 |
| — | 缺 | + `TargetFTYield` | 目标测试良率 |

**工艺路线示例（OSAT）**：
```
Wafer Incoming → Wafer Mount → Dicing → Die Attach → Cure →
Wire Bond → Plasma Clean → Mold → Post Mold Cure →
Mark → Singulation → Trim & Form → Final Test →
Visual Inspection → Tape & Reel → Pack Out
```

#### 3.2.2 批次模型 LotInfo 改造

| 字段 | 现有 | 改造 | 说明 |
|------|------|------|------|
| WaferCount | 25 | → `UnitCount` | 计数单位从晶圆改为颗/条 |
| CurrentStep | "ETCH" | → "DieAttach" | 工序名称改为封测工序 |
| CurrentEquipment | "EQ-ETCH-02" | → "DB-01" | 设备编号前缀改为封测设备 |
| — | 缺 | + `CarrierType` | 载具类型（Strip/Magazine/Tray/Reel） |
| — | 缺 | + `CarrierId` | 载具编号 |
| — | 缺 | + `StripCount` | 条带数 |
| — | 缺 | + `FrameId` | 框架编号（当前绑定的LeadFrame） |
| — | 缺 | + `BinResult` | Bin分类结果（测试后） |
| — | 缺 | + `TestResult` | 测试结论（Pass/Fail） |
| — | 缺 | + `QtyPass` | 良品数 |
| — | 缺 | + `QtyFail` | 不良数 |

#### 3.2.3 视图改造

| 视图 | 现有标题 | 改造 |
|------|---------|------|
| WorkOrderListView | 工单管理 | 保留，字段按上表调整 |
| LotListView | 批次管理 | 保留，增加载具/条带/Bin信息列 |
| LotHoldView | 批次Hold管理 | 保留，HoldType增加YieldHold/DataHold |
| TrackInView | 进站/出站 | 保留，设备下拉改为封测设备 |
| WipOverviewView | WIP总览 | 工序列从Fab工序改为封测工序 |
| AddWorkOrderWin | 创建工单 | 表单字段替换（封装类型替代工艺节点等） |

---

### 3.3 Equipment 模块（设备管理）

#### 模拟设备数据改造

```
现有 30 台 Fab 设备:
  EQ-PHOTO-01, EQ-ETCH-02, EQ-CVD-03, EQ-CMP-01, EQ-IMP-02

改为 OSAT 设备:
  WS-01 ~ WS-05     (DicingSaw / 切割机)
  DB-01 ~ DB-05      (DieBonder / 贴片机)
  WB-01 ~ WB-08      (WireBonder / 焊线机，数量最多)
  MP-01 ~ MP-04      (MoldingPress / 塑封机)
  MK-01 ~ MK-02      (LaserMark / 打标机)
  TF-01 ~ TF-03      (TrimForm / 切筋成型机)
  TH-01 ~ TH-06      (TestHandler / 测试分选机)
  BI-01 ~ BI-02      (BurnIn / 老化箱)
  AOI-01 ~ AOI-03    (AOI / 自动光学检测)
```

#### EAP控制改造

Fab 的 EAP 控制关注 FOUP 端口（LoadPort/FOUP Open/Close），OSAT 关注：
- Strip 的进出（Strip Loader/Unload）
- Wire Bond 的金线供给（Wire Feeder）
- Test Handler 的分选仓（Bin Slot）

---

### 3.4 Quality 模块（质量管理）

#### 3.4.1 SPC 参数改造

```
现有 Fab SPC 参数: CD（关键尺寸）, Overlay（套刻精度）, Etch Rate

改为 OSAT SPC 参数:
  焊线: WirePullStrength（拉力）, BallShear（推力）, LoopHeight（线弧高度）
  贴片: DieShearStrength（推力）, DieAttachThickness（胶厚）, EpoxyVoid（胶空洞率）
  塑封: WireSweep（金线偏移）, PackageVoid（封装空洞）, Flash（溢料）
  切割: DieSize（管芯尺寸）, Chipping（崩边）, SawKerf（刀缝宽度）
```

#### 3.4.2 FDC 监控改造

```
现有: Chamber T2/SPE（反应腔体参数）
改为: WireBonder 超声功率/压力/时间, MoldingPress 温度/压力/时间, TestHandler 温度/测试时间
```

#### 3.4.3 检验管理改造

| 现有 | 改为 |
|------|------|
| ADI（显影后检验） | PostDieAttach（贴片后检验） |
| OCD（关键尺寸量测） | PostWireBond（焊线后检验） |
| SEM（电子显微镜） | PostMold（塑封后X-Ray/SAM） |
| Defect（缺陷检测） | PostSaw（切割后AOI） |
| Metrology（量测） | FinalVisual（成品外观目检） |

---

### 3.5 Recipe 模块（配方管理）

OSAT 的"配方"概念与 Fab 不同：

| Fab | OSAT |
|-----|------|
| Equipment Recipe（设备工艺配方） | **Recipe** 保留，但内容不同 |
| 光刻配方: 曝光能量/焦距/NA | 焊线配方: 金线规格/键合参数/图案 |
| 蚀刻配方: 气体流量/RF功率 | 塑封配方: EMC型号/温度曲线/保压时间 |
| CVD配方: 温度/压力/气体比 | 测试配方: 测试程序/Bin定义/条件 |

**设备类型下拉改造**：`EquipmentType` 枚举改完后自动同步。

---

### 3.6 Warehouse 模块（仓储管理）

#### 需要大幅改造

| 现有视图 | Fab概念 | OSAT改造 |
|---------|--------|---------|
| FoupManagement | FOUP管理 | → **载具管理**（CarrierManagement）：WaferFrame/LeadFrame/Strip/Magazine/Tray/Reel |
| ReticleManagement | 光罩管理 | → **工具/模具管理**（ToolingManagement）：MoldCavity（模腔）/BondCapillary（瓷嘴）/SawBlade（切割刀片）/TestSocket（测试座） |
| Stocker | Stocker管理 | → **仓库/货架管理**：原料仓/WIP仓/成品仓/待检仓 |
| MaterialList | 物料管理 | 保留，但物料类型改为封测物料 |

#### 新增物料类型

```
封装物料:
  EMC           // 环氧塑封料（Epoxy Mold Compound）
  DAF           // 贴片膜（Die Attach Film）
  Epoxy         // 贴片胶（Die Attach Epoxy）
  GoldWire      // 金线
  CopperWire    // 铜线
  SilverWire    // 银线
  LeadFrame     // 引线框架
  Substrate     // 基板（BGA用）
  SolderBall    // 锡球（BGA用）
  TapeReel      // 编带
  Tube          // 料管
  TrayPkg       // 托盘
  Label         // 标签
  DryPack       // 干燥剂/防潮包
```

---

### 3.7 Yield 模块（良率管理）

#### 需要大幅改造

| 现有 | 改为 |
|------|------|
| WaferMap（晶圆管芯图） | **StripMap**（条带分布图）：按Strip展示每个位置的Pass/Fail |
| DieYield（管芯良率） | 保留 + 新增 **PackageYield**（封装良率）、**FTYield**（测试良率） |
| LineYield（线良率） | 保留，含义变为工序间良率（通过率） |
| — | + **BinDistribution**（Bin分布图）：FT后各Bin数量分布 |
| — | + **YieldByPackage**（按封装类型良率统计） |
| — | + **YieldByCustomer**（按客户良率统计） |

**良率看板 KPI 改造**：

```
现有: LineYield, DieYield, TestYield, DailyOutput
改为:
  CPYield        // 封装良率（Chip Pass Yield）
  FTYield        // 测试良率（Final Test Yield）
  LineYield      // 线良率（工序直通率）
  Throughput      // 日产出（颗/天）
  SBLRate        // 焊线不良率
  MSLRate        // 塑封不良率
  DicingChippingRate  // 切割崩边率
  BurnInFailRate       // 老化失效率
```

---

### 3.8 Trace 模块（追溯管理）

| 现有 | 改为 |
|------|------|
| LotTrace（批次工序追溯） | 保留，工序名改为封测工序 |
| Genealogy（血缘图谱） | 改造：增加**晶圆→条带→成品**的拆分关系（Fab无拆分，OSAT有晶圆拆成多个Strip） |
| ImpactAnalysis（影响分析） | 保留，增加按**金线批号/EMC批号**的影响范围分析 |

**OSAT 特有的追溯场景**：
- 一片晶圆拆成 N 个 Strip → 每个 Strip 追溯到原始晶圆
- 一卷金线用于多个批次 → 金线批号关联所有批次
- EMC 批号关联所有塑封批次

---

### 3.9 Schedule 模块（排程管理）

| 现有 | 改为 |
|------|------|
| DispatchBoard（派工看板） | 保留，按封测工序/设备排列 |
| DispatchRuleConfig | 保留，规则权重改为封测业务相关（如焊线机优先级按金线型号匹配） |
| CapacityAnalysis | 保留，产能单位从"wafer/h"改为"unit/h"或"strip/h" |

**OSAT 排程特点**：
- 焊线机通常是瓶颈设备（最多台数），排程优先级最高
- 测试机+Handler组合，需要协同排程
- Burn-in 时间长（24h~168h），需要提前排入

---

### 3.10 Alarm 模块（告警管理）

基本框架可复用，仅调整：
- `AlarmCategory` 增加 `Yield`（良率告警）和 `Material`（物料异常）
- 告警规则模板改为封测场景（如焊线拉力低于阈值、塑封空洞超标、FT良率低于Target）

---

### 3.11 EHS 模块（环境健康安全）

| 现有 | 改为 |
|------|------|
| EnvironmentMonitor（温湿度/粒子） | 保留 + 增加 **ESD监控**（静电防护，OSAT核心关注） |
| GasMonitor（气体监控） | 保留，但气体类型从Fab工艺气改为封测相关（EMC蒸气、清洗溶剂蒸气等） |
| ChemicalManagement（化学品管理） | 保留，化学品改为封测用化学品（贴片胶、清洗剂、助焊剂等） |

---

## 四、改造优先级与阶段

### P0 - 核心改造（必须先做，影响全局）

| 序号 | 改造项 | 影响范围 | 工作量 |
|------|--------|---------|--------|
| 1 | `EquipmentType` 枚举改为封测设备 | Domain + Equipment + Recipe + Schedule | 小 |
| 2 | `InspectionType` 枚举改为封测检验 | Domain + Quality | 小 |
| 3 | 新增 `PackageType`/`CarrierType`/`TestType` 枚举 | Domain | 小 |
| 4 | `HoldType` 增加 YieldHold/DataHold | Production | 小 |
| 5 | `WorkOrderInfo` 字段改造 | Production + Contracts | 中 |
| 6 | `LotInfo` 字段改造 | Production + Contracts | 中 |
| 7 | Mock 数据全部改为封测场景 | 所有模块 | 中 |

### P1 - 业务适配（第二优先）

| 序号 | 改造项 | 影响范围 | 工作量 |
|------|--------|---------|--------|
| 8 | Warehouse: FOUP管理→载具管理 | Warehouse | 大 |
| 9 | Warehouse: 光罩管理→工具/模具管理 | Warehouse | 大 |
| 10 | Quality: SPC/FDC参数改为封测参数 | Quality | 中 |
| 11 | Yield: WaferMap→StripMap + BinDistribution | Yield | 大 |
| 12 | AddWorkOrderWin 表单改造 | Production | 中 |
| 13 | EHS: 增加ESD监控 | EHS | 中 |

### P2 - 增强功能（第三优先）

| 序号 | 改造项 | 影响范围 | 工作量 |
|------|--------|---------|--------|
| 14 | Trace: 晶圆→Strip拆分关系 | Trace | 中 |
| 15 | Trace: 物料批号关联追溯 | Trace | 中 |
| 16 | Schedule: Burn-in长时排程适配 | Schedule | 中 |
| 17 | Alarm: 增加良率告警/物料异常告警 | Alarm | 小 |
| 18 | Equipment: EAP端口模型改为Strip/Reel | Equipment | 中 |

---

## 五、OSAT 关键业务场景

以下是改造后 MES 应支持的核心业务场景，开发和测试时以此为验收依据：

### 场景 1: 工单全流程
```
客户下单 → 创建工单（指定封装类型BGA256、客户料号、目标良率）
  → 下达工单 → 晶圆入料（Wafer Incoming）
  → 切割（Dicing）→ 贴片（Die Attach）→ 固化（Cure）
  → 焊线（Wire Bond）→ 等离子清洗（Plasma Clean）
  → 塑封（Mold）→ 后固化（PMC）
  → 打标（Mark）→ 切筋成型（Trim & Form）
  → 成品测试（Final Test）→ 外观目检（Visual Inspect）
  → 编带包装（Tape & Reel）→ 出货（Pack Out）
```

### 场景 2: 晶圆拆分与Strip追踪
```
1片晶圆（25颗Die）→ 切割后贴装到5个Strip（5颗/Strip）
→ 每个Strip独立追踪，追溯回原始晶圆批号
→ 任意Strip Hold 不影响其他Strip继续流转
```

### 场景 3: 测试与Bin分选
```
FT测试 → 按电性结果分Bin:
  Bin1: 良品（98.5%）→ Tape & Reel → 出货
  Bin2: 备选等级（0.8%）→ 降级出货
  Bin3: 电性Fail（0.5%）→ 报废/分析
  Bin4: 外观Fail（0.2%）→ 返修/报废
```

### 场景 4: 良率异常自动Hold
```
某批次FT测试良率 95% < 目标良率 98%
  → 系统自动触发 YieldHold
  → LotHoldView 显示 Hold 原因为"良率低于目标值"
  → 工程师分析 → 确认OK后手动Release
```

### 场景 5: 物料批号追溯
```
发现某批次EMC（塑封料）有质量问题
  → 通过EMC批号反查所有使用该批EMC的塑封批次
  → ImpactAnalysis 显示所有受影响批次及风险等级
  → 批量Hold受影响批次
```

---

## 六、视图标题改造对照

| 模块 | 现有标题 | 改为 |
|------|---------|------|
| Production | 工单管理 | 工单管理（不变） |
| Production | 批次管理 | 批次管理（不变） |
| Production | 批次Hold管理 | 批次Hold管理（不变） |
| Production | 进站/出站 | 进站/出站（不变） |
| Production | WIP总览 | WIP总览（不变） |
| Equipment | 设备状态总览 | 设备状态总览（不变） |
| Equipment | EAP控制 | 设备控制（去EAP，更通用） |
| Equipment | 预防性维护 | 预防性维护（不变） |
| Quality | SPC监控 | SPC监控（不变） |
| Quality | FDC监控 | FDC监控（不变） |
| Quality | 检验管理 | 检验管理（不变） |
| Warehouse | FOUP管理 | **载具管理** |
| Warehouse | 光罩管理 | **工装模具管理** |
| Warehouse | Stocker管理 | **仓库管理** |
| Warehouse | 物料管理 | 物料管理（不变） |
| Yield | Wafer Map | **Strip Map / Bin分布** |
| Yield | 良率趋势 | 良率趋势（不变） |
| Yield | 良率看板 | 良率看板（不变） |
| EHS | 气体监控 | **环境安全监控** |
| 其余 | — | 不变 |

---

## 七、技术实施注意事项

1. **枚举改造是全局性的** — `EquipmentType`、`InspectionType` 等枚举在 Domain 层修改后，所有引用模块的 Mock 数据、下拉选项、筛选条件需同步更新
2. **数据库/Redis 兼容** — 枚举值以字符串形式存储在 Redis JSON 中（`JsonStringEnumConverter`），修改枚举名后旧数据需要清理（删除 `mes:seeded` key 重新种子）
3. **Contracts DTO 同步** — `WorkOrderDto`、`LotDto` 等需与模型同步改造
4. **渐进式改造** — 建议按 P0 → P1 → P2 阶段执行，每个阶段完成后做集成测试
5. **Mock 数据一致性** — 所有模块的 Mock 数据需使用统一的封测术语和设备编号
