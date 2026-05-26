# MES 整体改写规划

> 整合 `Production 核心闭环 MES 方案.md` + 当前项目状态 + V2 架构设计
> 生成日期：2026-05-23

---

## 一、现状快照

| 维度 | 当前状态 | 方案要求 | 差距 |
|------|---------|---------|------|
| **数据存储** | MySQL KV 表 (mes_kv_store) | 18 张规范化表 | ⚠️ V3 需改造 |
| **架构** | WPF+Prism 客户端 + ASP.NET API 服务端 | 14 条能力线 | ⚠️ 仅覆盖 2 条线 |
| **Route** | 基础 RouteInfo/RouteStep | 含 Rework Route、Retest Route | ❌ 缺少 |
| **Track** | 基础 TrackIn/TrackOut | 含 12 项校验链 + 设备/Recipe/Quality 联动 | ⚠️ 无联动 |
| **Quantity** | 基础平衡校验 | 含 Split/Merge 数量管控 | ❌ 缺少 |
| **History** | 基础 OperationRecord | 含 14 种操作类型 | ⚠️ 不完整 |
| **Exception** | 基础 Hold | Hold/Release/Scrap/Rework/MRB/GradeSplit/AutoHold | ❌ 缺少 |
| **Audit/Signature** | 基础 AuditTrail | 4 级签核 (L0-L3) | ❌ 缺少 |
| **Genealogy** | 无 | Wafer→Lot→Test→Packing→Shipping 全链 | ❌ 无 |
| **Integration** | 无 | Equipment/Recipe/Quality/Warehouse/Yield/Alarm | ❌ 无 |
| **Customer** | 工单有 CustomerName | 完整客户交付线 | ❌ 缺少 |
| **Master Data** | 手写 | 18 类主数据受控 | ❌ 无 |
| **Dispatch** | 无 | 派工评分、瓶颈分析 | ❌ 无 |
| **Report** | 无 | 13 种报表 | ❌ 无 |
| **Data Governance** | 无 | 冷热分层、归档、权限隔离 | ❌ 无 |
| **客户端** | 有 UI 骨架 | 按 26 节菜单重构 | ⚠️ 需重构 |

---

## 二、改写阶段总览

```
Phase 1 ── V1 最小闭环（已完成 ✅）
  Route + Track + Quantity + History + 基础 Hold + 基础 Audit + MySQL

Phase 2 ── V2 异常闭环（本次改写重点）
  完整 Hold/Release + 电子签核 + Scrap + Rework + ForceTrack
  + 完整 Route(含 Rework Route) + Seed 全流程数据

Phase 3 ── V3 追溯变更
  Lot Genealogy + Split + Merge + Carrier 绑定 + 物料绑定

Phase 4 ── V4 联动客户交付
  Equipment/Recipe/Quality/Warehouse 联动
  + 客户批次查询 + 交期风险 + 派工

Phase 5 ── V5 正式上线
  主数据 + 报表 + 运维 + 数据治理 + 外部系统对接
```

---

## 三、Phase 2 详细改写计划（本次重点）

### 3.1 数据模型扩展（7 个现有模型修改 + 8 个新模型）

#### 现有模型修改

| 模型 | 文件 | 新增字段 |
|------|------|---------|
| **LotInfo** | LotInfo.cs | MotherLotId, IsPartialLot, SplitReason, ReworkRouteId, ReworkCount, IsUnderMRB, Grade, OriginalQty, TotalPassQty, TotalScrapQty, WaferLotId |
| **RouteStep** | RouteStep.cs | YieldThreshold, RequireSplit, AllowMerge, ReworkRouteId, RequiredCarrierType, EnableMRB, RequiredSignatureLevel |
| **HoldRecord** | HoldRecord.cs | ReleaseCondition, NotifyRoles, EscalationLevel |
| **AuditTrail** | AuditTrail.cs | (已满足方案要求) |
| **TrackRequest** | TrackRequest.cs | RouteId, RouteVersion, StepSeq, StepCode, StepName, InputQty, Remark |
| **TrackResult** | TrackResult.cs | NextStepName, Warnings (已有) |
| **OperationRecord** | OperationRecord.cs | (已满足方案要求) |

#### 新增模型（8 个）

| 模型 | 文件 | 用途 |
|------|------|------|
| **LotSplitRecord** | LotSplitRecord.cs | 拆批记录（母批→子批） |
| **LotMergeRecord** | LotMergeRecord.cs | 合批记录（多批→一批） |
| **LotCarrierBinding** | LotCarrierBinding.cs | 载具绑定/解绑/转移 |
| **SignatureRecord** | SignatureRecord.cs | 电子签核（L1/L2/L3） |
| **YieldRule** | YieldRule.cs | 良率阈值规则 |
| **ScrapRecord** | ScrapRecord.cs | 报废记录 |
| **ReworkRecord** | ReworkRecord.cs | 重工记录 |
| **LotGenealogy** | LotGenealogy.cs | 谱系关系节点 |

### 3.2 服务层新增（7 个服务接口 + 实现）

| 服务 | 接口 | 核心方法 |
|------|------|---------|
| **LotSplitMerge** | ILotSplitMergeService | SplitLot, MultiSplit, MergeLots, GradeSplit, GetChildLots |
| **Carrier** | ICarrierService | BindCarrier, UnbindCarrier, TransferCarrier, GetCarrierHistory |
| **Signature** | ISignatureService | RequestSignature, VerifySignature, GetSignatures |
| **Yield** | IYieldService | CalculateYield, CheckYieldRule, AutoHoldOnLowYield |
| **Genealogy** | IGenealogyService | RecordWaferToLot, RecordSplit, RecordMerge, GetFullGenealogy |
| **Rework** | IReworkService | CreateReworkLot, SwitchToReworkRoute, CompleteRework |
| **Scrap** | IScrapService | RecordScrap, CheckThreshold, RequireApproval |

### 3.3 TrackIn/TrackOut 增强

#### TrackIn 校验链（从当前 5 项 → 12 项）

```
1. ✅ Lot 存在
2. ✅ Lot 状态允许进站（不是 Completed/Scrapped）
3. ✅ Lot 不在 Hold
4. ❌ Lot 不在 MRB（新增）
5. ✅ 当前 Step 存在于 Route
6. ❌ 上一站已完成（新增）
7. ✅ 设备属于 Step 设备组
8. ❌ 载具类型匹配 Step 要求（新增）
9. ❌ Recipe 已审批且适配（新增）
10. ❌ 无未关闭品质异常（新增）
11. ❌ 物料齐套检查（新增，V4）
12. ❌ 上站 QueueTime 未超时（新增）
```

#### TrackOut 处理链（从当前 3 项 → 11 项）

```
1. ✅ 数量平衡校验
2. ❌ 良率计算 + YieldRule 检查 → Auto Hold（新增）
3. ❌ Scrap 超阈值 → L3 双人签核（新增）
4. ❌ Step 完成记录写入（已有，需完善）
5. ✅ QuantityTransaction 记录
6. ✅ AuditTrail 记录
7. ❌ 下一站计算（正常/Split/Rework/MRB）（新增）
8. ❌ 载具转移/解绑（新增）
9. ❌ Lot 当前 Step 更新（已有，需完善）
10. ❌ Genealogy 更新（新增）
11. ❌ 事件通知（Integration，V4）
```

### 3.4 电子签核策略

| 操作 | 签核等级 | 签核角色 | 说明 |
|------|---------|---------|------|
| 正常 TrackIn/Out | L0 | 操作员 | 仅记录，不需签核 |
| 普通 Hold | L1 | 操作员+主管 | 审计记录 |
| ForceTrackIn/Out | L2 | 主管/工程 | 密码+原因 |
| QA Hold Release | L2 | QA/Manager | 密码+原因 |
| Customer Hold Release | L3 | Manager+客服 | 双人复核 |
| Scrap < 阈值 | L1 | 操作员 | 审计记录 |
| Scrap ≥ 阈值 | L3 | 主管+QA | 双人复核 |
| Split | L2 | 工程/QA | 密码+原因 |
| Merge | L2 | 工程/QA | 密码+原因 |
| Rework | L2 | 工程/QA | 密码+原因 |
| MRB 判定 | L3 | 工程+QA+生产 | 三人复核 |
| Route 变更 | L3 | 工程+QA+主管 | 双人复核 |

### 3.5 Seed 数据设计

#### Route: QFN-STD-V2.0（12 步）

| Seq | Step | 载具类型 | YieldThreshold | ReworkRoute | SigLevel |
|-----|------|---------|----------------|-------------|----------|
| 1 | Saw | TapeFrame | 98% | RW-Saw | - |
| 2 | DieAttach | Magazine | 97% | RW-DieAttach | L1 |
| 3 | Cure | Magazine | - | - | - |
| 4 | WireBond | Magazine | 96% | RW-WireBond | L1 |
| 5 | Mold | MoldPlate | 99% | - | - |
| 6 | PMC | OvenCart | - | - | - |
| 7 | Mark | Magazine | - | - | - |
| 8 | Singulation | SingTray | 98% | - | L1 |
| 9 | FinalTest | TestTray | - | - | - |
| 10 | OQC | Tray | 99.5% | - | L2 |
| 11 | Packing | Reel/Tray | - | - | - |

#### 初始批次种子

| 批次 | 状态 | 说明 |
|------|------|------|
| LOT-001 | Waiting (Step 1: Saw) | 新批次，待投产 |
| LOT-COMPLETE-001 | Completed | 已完成全部 12 步（展示完整履历） |
| LOT-HOLD-001 | Hold (Step 4: WireBond) | WireBond 低良率 Hold |
| LOT-REWORK-001 | Processing (Rework Route) | WireBond 重工中 |
| LOT-MRB-001 | MRB (Step 8: Singulation) | 外观不良 MRB |
| LOT-GRADE-001 | Completed (已 Grade Split) | A 级 8500 + B 级 1030 |

#### 工单种子

| 工单 | 产品 | 客户 | 数量 | Route | 状态 |
|------|------|------|------|-------|------|
| WO-001 | QFN-24 | 车规客户 | 10,000 | QFN-STD-V2.0 | 生产中 |

---

## 四、Phase 2 实施步骤

### Step 1：扩展数据模型
- [ ] 修改 LotInfo（+12 字段）
- [ ] 修改 RouteStep（+8 字段）
- [ ] 新建 8 个模型文件

### Step 2：完善 TrackService
- [ ] TrackIn 校验链增强到 12 项
- [ ] TrackOut 处理链增强到 11 项
- [ ] 集成 YieldRule 自动 Hold
- [ ] 集成 Signature 校验
- [ ] 集成 Genealogy 记录

### Step 3：新增服务层
- [ ] ILotSplitMergeService + 实现
- [ ] ICarrierService + 实现
- [ ] ISignatureService + 实现
- [ ] IYieldService + 实现
- [ ] IGenealogyService + 实现
- [ ] IReworkService + 实现
- [ ] IScrapService + 实现

### Step 4：重写 ProductionDataService.SeedAsync
- [ ] QFN-STD-V2.0 Route（12 步）
- [ ] Rework Routes（RW-Saw, RW-DieAttach, RW-WireBond）
- [ ] 6 个种子批次 + 完整履历
- [ ] YieldRule 种子数据
- [ ] SignaturePolicy 种子数据

### Step 5：服务端 API 扩展
- [ ] RoutesController（完整 CRUD + 版本管理）
- [ ] TrackController（完整 TrackIn/TrackOut）
- [ ] SplitMergeController（拆分/合批 API）
- [ ] GenealogyController（谱系查询 API）
- [ ] OperationsController（操作历史 + 审计）

### Step 6：客户端 UI 更新
- [ ] TrackInView（完整校验链展示 + 签核弹窗）
- [ ] WipOverviewView（完整状态展示）
- [ ] BatchHistoryView（时间线 + Genealogy 树）
- [ ] 新增 LotSplitView（拆批页面）
- [ ] 新增 LotMergeView（合批页面）
- [ ] 新增 ReworkView（重工管理）

### Step 7：编译验证 + 功能测试
- [ ] 全量编译 0 错误 0 警告
- [ ] MySQL 连接验证
- [ ] Seed 数据完整性验证
- [ ] TrackIn/TrackOut 闭环测试
- [ ] Split/Merge 测试
- [ ] Hold/Release 测试

---

## 五、Phase 3 概要（下一阶段）

### 5.1 Genealogy 全链路
```
WaferLot → SawLot → AssemblyLot → TestLot → GradeLot → PackingLot → ShippingLot
```

### 5.2 Carrier 全追踪
```
FOUP → TapeFrame → Magazine → MoldPlate → OvenCart → SingTray → TestTray → Reel/Tray
```

### 5.3 物料绑定
```
LeadFrame/EMC/GoldWire/Substrate → Lot @ Step → 消耗记录 → 追溯查询
```

### 5.4 Retest 流程
```
TestLot → RetestLot → 新结果 → 合并判定
```

---

## 六、Phase 4 概要（下一阶段）

### 6.1 Equipment 联动
- TrackIn 前校验设备状态/占用/设备组
- TrackOut 后释放设备占用

### 6.2 Recipe 联动
- TrackIn 前校验 Recipe 审批/版本/适配
- EAP 接口（预留）

### 6.3 Quality 联动
- TrackIn 前校验无未关闭品质异常
- TrackOut 后 SPC/FDC 检查 + Auto Hold

### 6.4 客户交付
- 客户批次进度查询
- 交期风险计算
- 客户追溯报告导出

### 6.5 派工
- 工序派工清单
- 派工评分算法
- 设备负载看板

---

## 七、Phase 5 概要（正式上线）

### 7.1 主数据管理
18 类主数据 CRUD + 审批 + 版本管理

### 7.2 报表
13 种报表实现

### 7.3 运维
- 数据库备份恢复
- 日志监控
- 健康检查
- 发布回滚

### 7.4 数据治理
- 冷热分层
- 历史归档
- 审计保留
- 数据质量检查

---

## 八、数据库演进路线

| 阶段 | 存储方案 | 原因 |
|------|---------|------|
| Phase 1-2 | MySQL KV 表 (mes_kv_store) | 快速实现，不改基础设施 |
| Phase 3 | 保持 KV + 增加索引字段 | 复杂查询性能可接受 |
| Phase 4 | 评估拆分规范化 | 联动查询增多，可能需要 |
| Phase 5 | 规范化关系表 + EF Core 迁移 | 正式上线需规范化 |

---

## 九、关键技术决策

| 决策 | 选择 | 原因 |
|------|------|------|
| 存储 | KV 表 V2，规范化 V5 | 速度 vs 规范的权衡 |
| 批次标识 | LotId 后缀 (LOT-001-01) | 直观易追溯 |
| 电子签核 | 密码确认 V2，真实签核 V5 | Demo 阶段够用 |
| 签核策略 | 硬编码策略 V2，可配置 V5 | 快速实现 |
| Route 版本 | 字符串 "1.0"/"V2.0" | 简单够用 |
| 载具追踪 | 绑定记录表 | 每步独立记录 |
| 良率 Hold | 阈值规则表 + 实时计算 | 灵活可扩展 |

---

## 十、验收标准

### Phase 2 验收

```
✅ Lot 能按 12 步 Route 完整流转
✅ TrackIn 12 项校验全部生效
✅ TrackOut 11 项处理全部生效
✅ Hold Lot 禁止进站
✅ 低良率自动 Hold
✅ Scrap 超阈值需签核
✅ ForceTrackIn/Out 需签核
✅ Split 后子批可独立流转
✅ Merge 后数量正确
✅ Rework Lot 可切换重工路线
✅ 所有操作有 AuditTrail
✅ 所有操作有 OperationHistory
✅ 所有数量变化有 QuantityTransaction
✅ Seed 数据完整展示全流程
✅ 编译 0 错误 0 警告
```

---

## 十一、文件变更清单

### 新建文件（约 30 个）

| 分类 | 文件数 | 示例 |
|------|--------|------|
| 模型 | 8 | LotSplitRecord, LotMergeRecord, LotGenealogy, SignatureRecord... |
| 服务接口 | 7 | ILotSplitMergeService, ICarrierService, ISignatureService... |
| 服务实现 | 7 | LotSplitMergeService, CarrierService, SignatureService... |
| 控制器 | 3 | SplitMergeController, GenealogyController, ReworkController |
| ViewModel | 3 | LotSplitViewModel, ReworkViewModel, GenealogyViewModel |
| 视图 | 3 | LotSplitView, ReworkView, GenealogyView |

### 修改文件（约 15 个）

| 文件 | 修改内容 |
|------|---------|
| LotInfo.cs | +12 字段 |
| RouteStep.cs | +8 字段 |
| RouteInfo.cs | 新增 ReworkRoute 支持 |
| TrackService.cs | 校验链增强 + 处理链增强 |
| TrackInViewModel.cs | 完整 UI 适配 |
| ProductionDataService.cs | 完整 Seed 数据 |
| ProductionModule.cs | 新服务注册 |
| App.xaml.cs | （无变化） |
| 服务端 ProductionService.cs | 完整 TrackIn/TrackOut 逻辑 |
| RoutesController.cs | 完整 CRUD |
| TrackController.cs | 完整 TrackIn/TrackOut |
| 服务端 model 文件 | 与客户端同步 |

---

## 十二、风险与缓解

| 风险 | 影响 | 缓解措施 |
|------|------|---------|
| KV 表复杂查询慢 | Phase 3+ | 加索引，Phase 4 评估拆分 |
| 签核安全性不足 | 正式上线 | Phase 5 接真实电子签名 |
| Seed 数据量大 | 启动时间 | 异步初始化 |
| UI 改动量大 | 开发周期 | 先服务层，后 UI |
| MySQL 无事务 | 数据一致性 | 应用层补偿，Phase 5 规范化 |
