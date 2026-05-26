# MES V2 完整架构设计方案

> 基于完整半导体封测产线案例：Wafer 进厂 → Saw → DieAttach → Cure → WireBond → Mold → PMC → Mark → Singulation → FinalTest → OQC → Packing → 成品仓

---

## 一、现状审计

### 已实现（V1）
| 功能 | 状态 |
|------|------|
| 基础 TrackIn/TrackOut | ✅ 有 |
| 基础 Route/Step | ✅ 有（简单版） |
| 基础 Lot 模型 | ✅ 有 |
| 基础数量平衡 | ✅ 有 |
| 基础 Hold/Release | ✅ 有 |
| 基础 OperationHistory | ✅ 有 |
| MySQL 存储 | ✅ 有 |

### 缺失（V2 需要）
| 功能 | 优先级 | 说明 |
|------|--------|------|
| Lot Split/Merge | P0 | 拆批/合批，Partial Lot 追踪 |
| Genealogy 谱系 | P0 | 全链路追溯 Wafer→Lot→SubLot→成品 |
| Rework Route | P0 | 重工路线切换 |
| 载具绑定 | P0 | 每步 Carrier 绑定/解绑/转移 |
| 良率自动 Hold | P0 | 阈值触发自动 Hold |
| MRB 管理 | P1 | 材料审查委员会流程 |
| Grade Split | P1 | 按测试等级拆分 |
| 电子签核等级 | P1 | Level 1/2/3 签核 |
| 完整 Seed 数据 | P0 | 12步Route + 全流程数据 |

---

## 二、数据模型变更

### 2.1 LotInfo 扩展

```csharp
public class LotInfo
{
    // === 已有 ===
    public string LotId { get; set; }
    public string OrderId { get; set; }
    public string ProductId { get; set; }
    public string RouteId { get; set; }
    public string RouteVersion { get; set; }
    public int CurrentStepSeq { get; set; }
    public string Status { get; set; }

    // === 新增：Split/Merge 追踪 ===
    public bool IsPartialLot { get; set; }                    // 是否为部分批次
    public string? MotherLotId { get; set; }                  // 母批次ID（Split后子批指向母批）
    public string? SplitReason { get; set; }                  // 拆批原因
    public DateTime? SplitTime { get; set; }
    public int? SplitQty { get; set; }                        // 拆分数量
    
    // === 新增：Rework ===
    public bool IsReworkLot { get; set; }                     // 是否为重工批次
    public string? OriginalRouteId { get; set; }              // 原始路线ID
    public string? ReworkRouteId { get; set; }                // 当前重工路线ID
    public int? ReworkCount { get; set; }                     // 重工次数
    public string? ReworkReason { get; set; }

    // === 新增：MRB ===
    public bool IsUnderMRB { get; set; }                      // 是否处于MRB审查
    public string? MRBReference { get; set; }                 // MRB单号
    public string? MRBDisposition { get; set; }               // MRB结论: UseAsIs/Rework/Scrap/ReturnToVendor

    // === 新增：Grade ===
    public string? Grade { get; set; }                        // 等级: A(车规)/B(工业)/C(消费)
    public string? OriginalLotId { get; set; }                // Grade Split前的原始批次ID

    // === 新增：Wafer 关联 ===
    public string? WaferLotId { get; set; }                   // 来料晶圆批次号
    public string? WaferId { get; set; }                      // 具体晶圆片号
    public int? WaferCount { get; set; }                      // 晶圆片数

    // === 新增：数量累计 ===
    public int OriginalQty { get; set; }                      // 初始投入数量
    public int TotalPassQty { get; set; }                     // 累计合格
    public int TotalScrapQty { get; set; }                    // 累计报废
    public int TotalReworkQty { get; set; }                   // 累计重工
    public int TotalHoldQty { get; set; }                     // 累计Hold

    // === 新增：良率 ===
    public double CurrentYield => OriginalQty > 0 
        ? (double)(OriginalQty - TotalScrapQty) / OriginalQty * 100 : 100;
}
```

### 2.2 RouteStep 扩展

```csharp
public class RouteStep
{
    // === 已有 ===
    public string RouteId { get; set; }
    public int StepSeq { get; set; }
    public string StepName { get; set; }
    public string StepCode { get; set; }

    // === 新增：控制规则 ===
    public int? YieldThreshold { get; set; }                  // 良率阈值(%)，低于则自动Hold
    public bool RequireSplit { get; set; }                    // 是否必须拆批（如QA取样）
    public string? SplitRule { get; set; }                    // 拆批规则JSON
    public bool AllowMerge { get; set; }                      // 是否允许合批
    public string? MergeCondition { get; set; }               // 合批条件
    
    // === 新增：重工 ===
    public string? ReworkRouteId { get; set; }                // 关联重工路线ID
    public string? ReworkTargetStep { get; set; }             // 重工后回到哪一步
    
    // === 新增：载具 ===
    public string RequiredCarrierType { get; set; }           // 必须载具类型
    public int? MaxCarrierQty { get; set; }                   // 最大载具数量
    
    // === 新增：MRB ===
    public bool EnableMRB { get; set; }                       // 是否允许MRB
    public int? MRBThreshold { get; set; }                    // 不良超过多少触发MRB

    // === 新增：签核 ===
    public string? RequiredSignatureLevel { get; set; }       // "Level1"/"Level2"/"Level3"
}
```

### 2.3 新增：LotSplitRecord（拆批记录）

```csharp
public class LotSplitRecord
{
    public string SplitId { get; set; } = Guid.NewGuid().ToString("N");
    public string MotherLotId { get; set; } = string.Empty;    // 母批次
    public string ChildLotId { get; set; } = string.Empty;     // 子批次
    public int SplitQty { get; set; }                          // 拆分数量
    public string SplitReason { get; set; } = string.Empty;    // 原因: LowYield/QA Sample/Engineering/MRB
    public string SplitType { get; set; } = string.Empty;      // "Partial"/"Grade"/"Rework"
    public string OperatorId { get; set; } = string.Empty;
    public DateTime SplitTime { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }                    // 审批人
    public string? SignatureId { get; set; }                   // 签核ID
}
```

### 2.4 新增：LotMergeRecord（合批记录）

```csharp
public class LotMergeRecord
{
    public string MergeId { get; set; } = Guid.NewGuid().ToString("N");
    public string TargetLotId { get; set; } = string.Empty;    // 合并后的批次ID
    public List<string> SourceLotIds { get; set; } = [];       // 被合并的批次列表
    public int MergedQty { get; set; }                         // 合并后总数量
    public string MergeReason { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public DateTime MergeTime { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
}
```

### 2.5 新增：LotCarrierBinding（载具绑定）

```csharp
public class LotCarrierBinding
{
    public string BindingId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string CarrierId { get; set; } = string.Empty;
    public string CarrierType { get; set; } = string.Empty;    // FOUP/TapeFrame/Magazine/Tray/Reel等
    public string? FromCarrierId { get; set; }                 // 从上站转移来的载具
    public string? ToCarrierId { get; set; }                   // 转移到下站的载具
    public DateTime BindTime { get; set; } = DateTime.UtcNow;
    public DateTime? UnbindTime { get; set; }
    public string OperatorId { get; set; } = string.Empty;
}
```

### 2.6 新增：SignatureRecord（电子签核）

```csharp
public class SignatureRecord
{
    public string SignatureId { get; set; } = Guid.NewGuid().ToString("N");
    public string EntityType { get; set; } = string.Empty;     // "Split"/"Merge"/"Rework"/"Scrap"/"Hold"/"Release"
    public string EntityId { get; set; } = string.Empty;
    public string Level { get; set; } = "Level1";              // Level1/Level2/Level3
    public string SignerId { get; set; } = string.Empty;
    public string SignerName { get; set; } = string.Empty;
    public string SignerRole { get; set; } = string.Empty;
    public DateTime SignTime { get; set; } = DateTime.UtcNow;
    public string? SecondSignerId { get; set; }                // Level3 需要双人
    public string? SecondSignerName { get; set; }
    public DateTime? SecondSignTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}
```

### 2.7 新增：YieldRule（良率规则）

```csharp
public class YieldRule
{
    public string RuleId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public double YieldThreshold { get; set; }                 // 良率阈值%
    public string ActionType { get; set; } = "AutoHold";       // AutoHold/Alert/Block
    public string NotifyRole { get; set; } = "QA";            // 通知角色
    public bool IsActive { get; set; } = true;
}
```

### 2.8 新增：GenealogyNode（谱系节点）

```csharp
public class GenealogyNode
{
    public string NodeId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string? ParentLotId { get; set; }                  // 父批次（Split来源）
    public string? WaferLotId { get; set; }                   // 来料晶圆批次
    public string? WorkOrderId { get; set; }
    public string? PackingLotId { get; set; }                 // 最终包装批次
    public string NodeType { get; set; } = "Lot";              // Wafer/Lot/SubLot/Grade/Packing
    public string RelationType { get; set; } = string.Empty;   // Split/Merge/GradeSplit/Rework
    public int Qty { get; set; }
    public string? Grade { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

---

## 三、服务层新增

### 3.1 ILotSplitMergeService

```csharp
public interface ILotSplitMergeService
{
    // 拆批
    Task<List<LotInfo>> SplitLotAsync(string motherLotId, int splitQty, 
        string reason, string operatorId, string? approvedBy = null);
    
    // 多路拆批（一次拆成多个子批）
    Task<List<LotInfo>> MultiSplitAsync(string motherLotId, 
        List<(string childLotId, int qty, string reason)> splits,
        string operatorId, string? approvedBy = null);
    
    // 合批
    Task<LotInfo> MergeLotsAsync(List<string> sourceLotIds, 
        string targetLotId, string reason, string operatorId);
    
    // Grade 拆分
    Task<List<LotInfo>> GradeSplitAsync(string lotId, 
        List<(string grade, int qty)> grades, string operatorId);
    
    // 查询子批
    Task<List<LotInfo>> GetChildLotsAsync(string motherLotId);
    
    // 查询母批
    Task<LotInfo?> GetMotherLotAsync(string lotId);
}
```

### 3.2 ICarrierService

```csharp
public interface ICarrierService
{
    Task BindCarrierAsync(string lotId, string stepCode, int stepSeq, 
        string carrierId, string carrierType, string operatorId);
    Task UnbindCarrierAsync(string lotId, string carrierId);
    Task TransferCarrierAsync(string fromLotId, string toLotId, string carrierId);
    Task<List<LotCarrierBinding>> GetLotCarriersAsync(string lotId);
    Task<List<LotCarrierBinding>> GetCarrierHistoryAsync(string carrierId);
}
```

### 3.3 ISignatureService

```csharp
public interface ISignatureService
{
    Task<SignatureRecord> RequestSignatureAsync(string entityType, string entityId,
        string level, string signerId, string reason);
    Task<bool> VerifySignatureAsync(string entityType, string entityId, string level);
    Task<List<SignatureRecord>> GetSignaturesAsync(string entityType, string entityId);
}
```

### 3.4 IYieldService

```csharp
public interface IYieldService
{
    Task<double> CalculateStepYieldAsync(string lotId, string stepCode);
    Task<double> CalculateCumulativeYieldAsync(string lotId);
    Task<YieldRule?> CheckYieldRuleAsync(string lotId, string stepCode, 
        int passQty, int totalQty);
    Task<bool> AutoHoldOnLowYieldAsync(string lotId, string stepCode, 
        double actualYield, double threshold);
}
```

### 3.5 IGenealogyService

```csharp
public interface IGenealogyService
{
    Task RecordWaferToLotAsync(string waferLotId, string lotId, int qty);
    Task RecordSplitAsync(string motherLotId, string childLotId, int qty, string reason);
    Task RecordMergeAsync(string targetLotId, List<string> sourceLotIds, int qty);
    Task RecordGradeSplitAsync(string lotId, string gradeLotId, string grade, int qty);
    Task<List<GenealogyNode>> GetFullGenealogyAsync(string lotId);
    Task<List<GenealogyNode>> GetUpstreamTraceAsync(string lotId);
    Task<List<GenealogyNode>> GetDownstreamTraceAsync(string lotId);
}
```

### 3.6 IReworkService

```csharp
public interface IReworkService
{
    Task<LotInfo> CreateReworkLotAsync(string originalLotId, string reworkRouteId,
        int qty, string reason, string operatorId);
    Task<LotInfo> SwitchToReworkRouteAsync(string lotId, string reworkRouteId,
        int targetStepSeq, string reason, string operatorId);
    Task<LotInfo> CompleteReworkAsync(string reworkLotId, string operatorId);
}
```

### 3.7 IMRBService

```csharp
public interface IMRBService
{
    Task<string> CreateMRBAsync(string lotId, int qty, string reason, string operatorId);
    Task SetMRBDispositionAsync(string mrbReference, string disposition,
        string comment, string operatorId);
    Task ExecuteMRBDispositionAsync(string mrbReference, string operatorId);
    Task<bool> IsUnderMRBAsync(string lotId);
}
```

---

## 四、状态机设计

### 4.1 Lot 状态转换

```
Created → Released → Waiting → Processing → Completed
                       ↓            ↓
                     Hold ←─── Processing
                       ↓            ↓
                   Released    Processing (Rework Route)
                                  ↓
                              Processing (Main Route)
                       ↓
                     MRB → UseAsIs/Rework/Scrap
                       ↓
                     Scrapped
```

### 4.2 合法状态转换表

| 当前状态 | 允许转换 | 触发条件 |
|---------|---------|---------|
| Created | → Released | 计划员Release |
| Released | → Waiting | 自动（到第一站） |
| Waiting | → Processing | TrackIn |
| Waiting | → Hold | 外部Hold |
| Processing | → Hold | 低良率/QA/工程 |
| Processing | → Processing | TrackOut（同站重工） |
| Processing | → Waiting | TrackOut（到下一站） |
| Processing | → Completed | TrackOut（最后一站） |
| Hold | → Released | Release审批通过 |
| Hold | → MRB | MRB触发 |
| MRB | → Processing | MRB结论UseAsIs/Rework |
| MRB | → Scrapped | MRB结论Scrap |
| Processing (Rework) | → Processing | 重工完成回到主线 |

---

## 五、TrackIn 校验链增强

```
TrackIn(lotId, equipmentId, carrierId)
  │
  ├─ 1. Lot 是否存在？
  ├─ 2. Lot 状态是否允许进站？（不是 Completed/Scrapped）
  ├─ 3. Lot 是否 Hold？→ 拒绝
  ├─ 4. Lot 是否 MRB？→ 拒绝
  ├─ 5. 当前 Step 是否存在于 Route？
  ├─ 6. 设备是否属于该 Step 的设备组？
  ├─ 7. 载具类型是否匹配 Step 要求？
  ├─ 8. 前置工序是否全部完成？
  ├─ 9. 是否有未关闭的品质异常？
  ├─ 10. Recipe 是否匹配？
  ├─ 11. 物料是否齐套？
  └─ 12. 上站 QueueTime 是否超时？
```

---

## 六、TrackOut 处理链增强

```
TrackOut(lotId, passQty, failQty, scrapQty, reworkQty, holdQty)
  │
  ├─ 1. 数量平衡校验：input = pass + fail + scrap + rework + hold + pending
  ├─ 2. 良率计算：yield = pass / input
  ├─ 3. 良率规则检查 → 低于阈值 → Auto Hold
  ├─ 4. Scrap 超限 → Level 3 双人签核
  ├─ 5. Step 完成记录写入 LotStepRecord
  ├─ 6. QuantityTransaction 记录
  ├─ 7. AuditTrail 记录
  ├─ 8. 计算下一站
  │     ├─ 正常 → 下一站
  │     ├─ 需要 Split → 生成子批
  │     ├─ 需要 Rework → 切换重工路线
  │     └─ 需要 MRB → 进入MRB
  ├─ 9. 载具转移/解绑
  ├─ 10. 更新 Lot 当前 Step
  └─ 11. Genealogy 更新
```

---

## 七、Seed 数据设计（QFN-Standard 完整流程）

### 7.1 Route: QFN-STD-V2.0

| Seq | Step Code | Step Name | 载具类型 | YieldThreshold | ReworkRoute | Signature |
|-----|-----------|-----------|---------|----------------|-------------|-----------|
| 1 | Saw | 晶圆切割 | TapeFrame | 98 | RW-Saw | - |
| 2 | DieAttach | 固晶 | Magazine | 97 | RW-DieAttach | Level1 |
| 3 | Cure | 烘烤固化 | Magazine | - | - | - |
| 4 | WireBond | 焊线 | Magazine | 96 | RW-WireBond | Level1 |
| 5 | Mold | 塑封 | MoldPlate | 99 | - | - |
| 6 | PMC | 后固化 | OvenCart | - | - | - |
| 7 | Mark | 印字 | Magazine | - | - | - |
| 8 | Singulation | 切筋成型 | SingTray | 98 | - | Level1 |
| 9 | FinalTest | 终测 | TestTray | - | - | - |
| 10 | OQC | 出货检验 | Tray | 99.5 | - | Level2 |
| 11 | Packing | 包装 | Reel/Tray | - | - | - |

### 7.2 初始种子数据

```
工单: WO-001
  产品: QFN-24
  客户: 车规客户
  计划数量: 10,000 (1片晶圆)
  Wafer Lot: WFR-20260520-001
  
  批次: LOT-001
    状态: Waiting (在第1站Saw)
    Route: QFN-STD-V2.0
    Carrier: Foup-007 → TapeFrame-033
    
  已完成的批次（展示完整流程）:
    LOT-COMPLETE-001 (已完成全部12步)
    LOT-HOLD-001 (在WireBond被Hold)
    LOT-REWORK-001 (在重工路线上)
    LOT-MRB-001 (在MRB审查中)
```

---

## 八、架构评估

### 8.1 当前架构 vs 实际生产

| 评估项 | 当前 | 实际生产需要 | 差距 |
|--------|------|-------------|------|
| 数据存储 | MySQL 单KV表 | 关系型表 + 审计表 | ⚠️ 性能问题 |
| 事务性 | 无事务 | ACID 事务 | ❌ 关键缺失 |
| 并发控制 | 无 | 乐观锁/悲观锁 | ❌ 关键缺失 |
| 审计追溯 | 基础 | 完整四级签核 | ⚠️ 不完整 |
| 设备联动 | 无 | EAP/SECS-GEM | ❌ V4+ |
| 实时监控 | 无 | WebSocket推送 | ❌ V4+ |

### 8.2 建议

**V2 阶段保持当前架构**，原因：
1. 单机 Demo 场景，MySQL 单表可支撑 10万+ 记录
2. 暂不需要分布式事务
3. 核心是**业务逻辑闭环**，不是基础设施

**V3+ 建议**：
1. 拆分为规范化关系表（Lot表、LotStep表、SplitRecord表等）
2. 引入 EF Core 迁移管理
3. 引入乐观锁（RowVersion字段）
4. 引入消息队列处理异步事件（良率告警、设备联动）

---

## 九、实施顺序

| 阶段 | 内容 | 预估 |
|------|------|------|
| Phase 1 | 扩展数据模型（LotInfo/RouteStep/新增7个模型） | 基础 |
| Phase 2 | Seed 完整 Route（12步 QFN-STD）+ 初始数据 | 可演示 |
| Phase 3 | Split/Merge 服务 + UI | 核心 |
| Phase 4 | Genealogy 服务 + 追溯页面 | 核心 |
| Phase 5 | Carrier 绑定服务 | 核心 |
| Phase 6 | Yield 自动 Hold + 签核服务 | 核心 |
| Phase 7 | Rework/MRB 服务 | 核心 |
| Phase 8 | TrackIn/TrackOut 增强校验链 | 闭环 |
| Phase 9 | UI 更新（TrackIn/WIP/BatchHistory） | 展示 |

---

## 十、关键技术决策

### Decision 1: 保持 KV 表还是规范化？
**选择：V2 保持 KV 表**。所有新模型同样用 JSON 序列化存储。
- 理由：快速实现，不改基础设施
- 风险：复杂查询性能差（但 Demo 场景可接受）

### Decision 2: 如何标识 Partial Lot？
**选择：LotId 加后缀**，如 `LOT-001-01`、`LOT-001-02`
- 理由：直观，易追溯
- MotherLotId 字段关联母批

### Decision 3: 电子签核实现方式？
**选择：简化为密码确认**，V2 实现 Level 记录，V3 接真实电子签名
- 理由：Demo 阶段无需真实签核
- 记录完整签核链（谁、什么时候、为什么）

### Decision 4: 载具如何追踪？
**选择：LotCarrierBinding 表记录每次绑定**
- TrackIn 时绑定载具
- TrackOut 时解绑/转移
- 支持载具历史查询
