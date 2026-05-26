# Production 核心闭环 MES 方案

## 1. 文档目标

本文档用于定义 `MES.Modules.Production` 从当前生产模块原型升级为半导体封测厂实际可用 MES Production 核心系统的方案。

目标不是只做展示型页面，而是形成可支撑真实生产执行的闭环：

```text
工单创建
→ 批次创建
→ 绑定 Route/Step
→ 进站校验
→ TrackIn
→ TrackOut
→ 数量平衡
→ 自动流转下一站
→ Hold/Release/异常处理
→ 履历/追溯/客户查询
```

最终目标是让 Production 模块具备以下能力：

- 生产流程可控
- 批次流转可追溯
- 数量变化可平衡
- 异常处理可闭环
- 操作行为可审计
- 关键动作可签核
- 客户交付可解释
- 系统运行可维护

---

## 2. 总体判断

当前 Production 模块已经具备封测 MES 的基础概念，包括：

- 工单
- 批次
- WIP
- 进站/出站
- Hold/Release
- 设备号
- 载具号
- Recipe/Test Program
- 客户料号
- Bin Spec
- Wafer 来源

但当前系统更接近：

```text
封测 MES 生产模块原型 / Demo
```

还不能直接作为：

```text
封测厂正式生产上线 MES
```

主要不足包括：

- TrackIn/TrackOut 还没有形成真实生产闭环
- Route/Step 控制不足
- 操作履历和审计不完整
- Hold/Release 缺少审批和历史
- 缺少 Lot Genealogy
- 缺少数量平衡
- 缺少电子签核
- 缺少设备、Recipe、质量、仓储联动
- 缺少客户批次查询和交期风险
- 缺少正式生产所需的主数据、派工、报表、运维、数据治理能力

---

## 3. 完整 Production MES 能力线

要成为封测厂实际可用的 MES Production 系统，建议建设 14 条能力线：

```text
1. Route 控制线
2. Track 执行线
3. Quantity 数量线
4. History 履历线
5. Exception 异常线
6. Audit/Signature 审计签核线
7. Genealogy 谱系线
8. Integration 模块联动线
9. Customer 客户交付线
10. Master Data 主数据线
11. Dispatch/Planning 派工计划线
12. Reliability/Operation 运维可靠性线
13. Report/BI 报表线
14. Data Governance 数据治理线
```

---

## 4. 上线成熟度判断

### 4.1 只完成 1-5 条线

```text
适合 Demo、原型评审、试验线模拟
不建议正式生产上线
```

原因：缺少审计、签核、谱系、模块联动和客户追溯。

### 4.2 完成 1-9 条线

```text
可以作为封测厂小范围试点 MES
适合一条线、一个车间、部分产品试运行
```

具备生产执行闭环，但主数据、派工、报表、运维和数据治理仍需补齐。

### 4.3 完成 1-14 条线

```text
可以作为封测厂正式生产上线 MES 的基础方案
```

后续还需要根据工厂实际情况对接：

- ERP
- EAP/SECS/GEM
- WMS
- STDF/ATDF 测试数据
- OQC
- Shipping
- 标签系统
- 客户系统

---

# 第一部分：生产执行核心闭环

---

## 5. Route 控制线

### 5.1 目标

Route 控制线用于保证 Lot 必须按照工艺路线流转，防止：

- 跳站
- 错站
- 错设备
- 错 Recipe
- 未完成上一站就进入下一站
- 未满足质量 Gate 就继续生产

### 5.2 典型封测 Route

```text
Saw
→ DieAttach
→ Cure
→ WireBond
→ Mold
→ PMC
→ Mark
→ Singulation
→ FinalTest
→ OQC
→ Packing
```

不同产品可以有不同 Route：

- BGA Route
- QFN Route
- SOP Route
- Engineering Route
- Rework Route
- Retest Route

### 5.3 数据模型

```csharp
public class RouteInfo
{
    public string RouteId { get; set; } = string.Empty;
    public string RouteName { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = "1.0";
    public string ProductId { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsApproved { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public DateTime? ApprovedAt { get; set; }
}

public class RouteStep
{
    public string RouteId { get; set; } = string.Empty;
    public string RouteVersion { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string StepName { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;

    public string EquipmentGroup { get; set; } = string.Empty;
    public string RequiredRecipeType { get; set; } = string.Empty;

    public bool RequireTrackIn { get; set; } = true;
    public bool RequireTrackOut { get; set; } = true;
    public bool RequireRecipeCheck { get; set; } = true;
    public bool RequireEquipmentCheck { get; set; } = true;
    public bool RequireMaterialCheck { get; set; }
    public bool RequireQualityGate { get; set; }
    public bool RequireQuantityBalance { get; set; } = true;

    public bool AllowSkip { get; set; }
    public bool AllowReworkToThisStep { get; set; }

    public int QueueTimeLimitMinutes { get; set; }
}
```

### 5.4 TrackIn 前规则

```text
1. Lot 存在。
2. Lot 状态必须是 Waiting 或 Released。
3. Lot 不在 Hold。
4. Lot 当前 Step 必须存在于 Route。
5. 上一站必须已完成。
6. 当前设备必须属于 Step 允许设备组。
7. 当前 Recipe 必须适配 Lot 产品、Step、设备。
8. 当前 Step 不允许跳过。
9. 如果 Step 需要 Quality Gate，必须 QA 放行。
```

### 5.5 TrackOut 前规则

```text
1. Lot 必须已经 TrackIn。
2. TrackIn 设备必须和 TrackOut 设备一致，除非允许换机。
3. 必填工艺参数必须录入。
4. 数量必须平衡。
5. 如果触发低良率，自动 Hold。
6. 出站成功后推进下一 Step。
```

### 5.6 服务接口

```csharp
public interface IRouteService
{
    Task<RouteInfo?> GetRouteAsync(string routeId, string version);
    Task<RouteStep?> GetCurrentStepAsync(string lotId);
    Task<RouteStep?> GetNextStepAsync(string lotId);
    Task<bool> IsEquipmentAllowedAsync(string stepCode, string equipmentId);
    Task<bool> IsRecipeAllowedAsync(string lotId, string stepCode, string recipeId);
}
```

---

## 6. Track 执行线

### 6.1 目标

Track 执行线用于实现真实进站/出站闭环。

### 6.2 TrackIn 流程

```text
扫码 Lot
→ 扫码 Equipment
→ 扫码 Carrier
→ 系统校验
→ 校验通过
→ 写入 LotStepRecord
→ Lot.Status = Processing
→ Lot.CurrentEquipment = EquipmentId
→ 写 OperationHistory
→ 写 AuditTrail
```

### 6.3 TrackOut 流程

```text
输入产出数量
→ 输入不良/报废/重工数量
→ 输入 Recipe/TestProgram/Bin 结果
→ 数量平衡校验
→ 质量规则校验
→ 写 LotStepRecord.TrackOut
→ 写 QuantityTransaction
→ Lot 推进下一站
→ 写 OperationHistory
→ 写 AuditTrail
```

### 6.4 请求模型

```csharp
public class TrackInRequest
{
    public string LotId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string CarrierId { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public string Workstation { get; set; } = string.Empty;
    public string? RecipeId { get; set; }
}

public class TrackOutRequest
{
    public string LotId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public string Workstation { get; set; } = string.Empty;

    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public int ScrapQty { get; set; }
    public int ReworkQty { get; set; }
    public int HoldQty { get; set; }

    public string? RecipeId { get; set; }
    public string? TestProgram { get; set; }
    public string? BinSummary { get; set; }
    public string? Remark { get; set; }
}
```

### 6.5 服务接口

```csharp
public interface ITrackService
{
    Task<TrackValidationResult> ValidateTrackInAsync(TrackInRequest request);
    Task<TrackResult> TrackInAsync(TrackInRequest request);

    Task<TrackValidationResult> ValidateTrackOutAsync(TrackOutRequest request);
    Task<TrackResult> TrackOutAsync(TrackOutRequest request);

    Task<TrackResult> ForceTrackInAsync(TrackInRequest request, ElectronicSignature signature);
    Task<TrackResult> ForceTrackOutAsync(TrackOutRequest request, ElectronicSignature signature);
}
```

---

## 7. Quantity 数量线

### 7.1 目标

数量线用于保证每次生产动作的数量变化都可解释、可追溯、可平衡。

### 7.2 出站数量公式

建议统一公式：

```text
InputQty = PassQty + FailQty + ScrapQty + ReworkQty + HoldQty + PendingQty
```

或根据业务定义：

```text
InputQty = OutputQty + ScrapQty + LossQty
OutputQty = PassQty + ReworkQty + HoldQty + PendingQty
```

关键是全系统必须使用统一规则，不能每个页面各算各的。

### 7.3 数据模型

```csharp
public class QuantityTransaction
{
    public string TransactionId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string OperationId { get; set; } = string.Empty;

    public string TransactionType { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;

    public int BeforeQty { get; set; }
    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public int FailQty { get; set; }
    public int ScrapQty { get; set; }
    public int ReworkQty { get; set; }
    public int HoldQty { get; set; }
    public int PendingQty { get; set; }
    public int AfterQty { get; set; }

    public string UnitOfMeasure { get; set; } = "Unit";
    public string ReasonCode { get; set; } = string.Empty;

    public string OperatorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

### 7.4 服务接口

```csharp
public interface IQuantityService
{
    Task<QuantityValidationResult> ValidateTrackOutQuantityAsync(TrackOutRequest request);
    Task RecordQuantityTransactionAsync(QuantityTransaction transaction);
    Task<LotQuantitySummary> GetLotQuantitySummaryAsync(string lotId);
}
```

### 7.5 验收标准

```text
1. 数量不平衡不能 TrackOut。
2. ScrapQty 不能超过 CurrentQty。
3. Split 子批数量合计不能超过母批数量。
4. Merge 后目标批数量等于来源批数量合计。
5. Rework、Retest、Scrap 都必须生成 QuantityTransaction。
```

---

## 8. History 履历线

### 8.1 目标

履历线用于记录所有生产动作。

### 8.2 必须记录的动作

```text
CreateLot
ReleaseLot
TrackIn
TrackOut
Hold
ReleaseHold
Split
Merge
Rework
Scrap
ForceTrackIn
ForceTrackOut
ChangeQty
ChangeRoute
CloseWorkOrder
```

### 8.3 数据模型

```csharp
public class OperationRecord
{
    public string OperationId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string WorkOrderId { get; set; } = string.Empty;

    public string OperationType { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public int StepSeq { get; set; }

    public string? EquipmentId { get; set; }
    public string? RecipeId { get; set; }
    public string? CarrierId { get; set; }

    public int? BeforeQty { get; set; }
    public int? AfterQty { get; set; }

    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string Workstation { get; set; } = string.Empty;

    public DateTime OperationTime { get; set; }
    public string? Remark { get; set; }
}
```

### 8.4 页面建议

新增：

```text
批次履历页面 LotHistoryView
```

Timeline 示例：

```text
2026-05-22 08:01 创建批次
2026-05-22 08:30 进站 DieAttach DB-01 张三
2026-05-22 10:10 出站 DieAttach Pass 12000 Scrap 5
2026-05-22 10:20 进入 WireBond 等待
2026-05-22 11:05 Hold QA 异常
2026-05-22 13:30 Release QA 李四
```

---

## 9. Exception 异常线

### 9.1 目标

异常线用于处理真实生产异常，包括：

- Hold
- Release
- Scrap
- Rework
- Retest
- Split
- Merge
- MRB
- Low Yield Auto Hold

---

## 10. Hold/Release 履历和审批

### 10.1 Hold 流程

```text
提交 Hold
→ Lot.Status = Hold
→ 生成 HoldRecord
→ 写 OperationHistory
→ 写 AuditTrail
→ 通知责任部门
```

### 10.2 Release 流程

```text
Release 申请
→ 检查 Release 条件
→ 电子签核 / 审批
→ HoldRecord.Status = Released
→ Lot.Status = Waiting 或 Processing
→ 写 OperationHistory
→ 写 AuditTrail
```

### 10.3 数据模型

```csharp
public class HoldRecord
{
    public string HoldId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;

    public string HoldType { get; set; } = string.Empty;
    public string HoldReasonCode { get; set; } = string.Empty;
    public string HoldReason { get; set; } = string.Empty;
    public int HoldQty { get; set; }

    public string ResponsibleDept { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;

    public string Status { get; set; } = "Open";

    public string HoldBy { get; set; } = string.Empty;
    public DateTime HoldTime { get; set; }

    public string? RootCause { get; set; }
    public string? CorrectiveAction { get; set; }
    public string? Disposition { get; set; }

    public string? ReleaseBy { get; set; }
    public DateTime? ReleaseTime { get; set; }
    public string? ReleaseComment { get; set; }
    public string? ApprovedBy { get; set; }
}
```

### 10.4 Hold 类型权限

| Hold 类型 | 可 Release 角色 |
|---|---|
| Quality | QA / Manager |
| Engineering | Engineer / Manager |
| Customer | Manager / Customer Service / QA |
| Material | Warehouse / Planner / Manager |
| Equipment | Equipment / Maintenance |
| YieldHold | Engineer / QA |
| DataHold | Engineer / Test |

---

# 第二部分：审计、谱系、联动、客户交付

---

## 11. Audit/Signature 审计签核线

### 11.1 目标

解决以下问题：

```text
谁做了什么？
什么时候做的？
为什么做？
改前是什么？
改后是什么？
谁批准的？
是否越权？
客户审计时能不能证明？
```

封测厂正式上线 MES 时，审计和电子签核是必须能力。

### 11.2 操作分级

#### Level 0：普通记录

只写 OperationHistory，不要求二次签核。

```text
正常 TrackIn
正常 TrackOut
普通查询
刷新看板
```

#### Level 1：审计记录

写 AuditTrail，但不要求电子签核。

```text
普通 Hold
普通 Release
修改备注
修改优先级
批次状态变更
```

#### Level 2：电子签核

必须输入授权账号、密码/工号卡、原因。

```text
ForceTrackIn
ForceTrackOut
Release QA Hold
Release Customer Hold
Scrap
Rework
Retest
Split
Merge
ChangeQuantity
SkipStep
ChangeRoute
CloseWorkOrder
CancelWorkOrder
```

#### Level 3：双人复核

需要操作人 + 审批人两个人。

```text
大数量 Scrap
客户 Hold Release
MRB 判定
跳站
路线变更
出货前异常放行
车规客户特殊放行
```

### 11.3 AuditTrail 模型

```csharp
public class AuditTrail
{
    public string AuditId { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;
    public string ActionLevel { get; set; } = "Audit";

    public string OperatorId { get; set; } = string.Empty;
    public string OperatorName { get; set; } = string.Empty;
    public string OperatorRole { get; set; } = string.Empty;

    public string Workstation { get; set; } = string.Empty;
    public string ClientIp { get; set; } = string.Empty;

    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }

    public string? ReasonCode { get; set; }
    public string? ReasonText { get; set; }

    public string? SignatureId { get; set; }

    public DateTime CreatedAt { get; set; }
}
```

### 11.4 ElectronicSignatureRecord 模型

```csharp
public class ElectronicSignatureRecord
{
    public string SignatureId { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;

    public string RequestedBy { get; set; } = string.Empty;
    public string SignedBy { get; set; } = string.Empty;
    public string SignedByRole { get; set; } = string.Empty;

    public string RequiredRole { get; set; } = string.Empty;
    public string ReasonCode { get; set; } = string.Empty;
    public string ReasonText { get; set; } = string.Empty;

    public bool IsApproved { get; set; }
    public string? RejectReason { get; set; }

    public string Workstation { get; set; } = string.Empty;
    public DateTime SignedAt { get; set; }
}
```

### 11.5 SignaturePolicy 模型

```csharp
public class SignaturePolicy
{
    public string PolicyId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;

    public bool RequireSignature { get; set; }
    public bool RequireDualApproval { get; set; }

    public string RequiredRole { get; set; } = string.Empty;
    public string? RequiredDept { get; set; }

    public int? QtyThreshold { get; set; }
    public string? CustomerLevel { get; set; }

    public bool IsActive { get; set; }
}
```

### 11.6 服务接口

```csharp
public interface IAuditService
{
    Task WriteAsync(AuditTrail audit);
    Task<List<AuditTrail>> GetByEntityAsync(string entityType, string entityId);
    Task<List<AuditTrail>> SearchAsync(AuditQuery query);
}

public interface ISignatureService
{
    Task<SignatureRequirement> GetRequirementAsync(string action, string entityId, string operatorId);
    Task<SignatureResult> SignAsync(SignatureRequest request);
    Task<bool> ValidatePermissionAsync(string signerId, string requiredRole);
}
```

### 11.7 页面建议

```text
电子签核弹窗
签核策略配置
审计日志查询
强制作业审批
异常放行审批
```

### 11.8 验收标准

```text
1. ForceTrackIn 必须签核。
2. ForceTrackOut 必须签核。
3. Scrap 超阈值必须双人复核。
4. QA Hold 只能 QA 或 Manager Release。
5. Customer Hold 不能由普通操作员 Release。
6. 所有签核都能查到谁签的、为什么签。
7. 所有关键对象能查改前/改后。
8. 审计记录不能被普通用户删除。
```

---

## 12. Genealogy 谱系线

### 12.1 目标

解决以下问题：

```text
这批货从哪里来？
分到哪里去了？
用了哪些 Wafer？
经过哪些 Assembly Lot？
哪些 Test Lot 合成出货批？
哪些批次被拆分、合并、重工、报废？
客户追溯时能否完整还原？
```

### 12.2 封测典型谱系

```text
Customer Wafer Lot
→ Wafer ID
→ Saw Lot
→ Assembly Lot
→ Strip / LeadFrame / Substrate
→ Test Lot
→ Bin / Grade Lot
→ Packing Lot
→ Shipping Lot
```

示例：

```text
WAFER-LOT-A
  ├─ Wafer 01
  ├─ Wafer 02
  └─ Wafer 03
       ↓
ASM-LOT-001
       ↓
TST-LOT-001
  ├─ Grade-A-Lot
  ├─ Grade-B-Lot
  └─ Scrap-Lot
       ↓
PACK-LOT-001
       ↓
SHIP-LOT-001
```

### 12.3 Genealogy 类型

```text
WaferToAssembly
AssemblyToTest
Split
Merge
Rework
Retest
Scrap
GradeSplit
Packing
Shipping
MaterialAttach
CarrierBind
```

### 12.4 LotGenealogy 模型

```csharp
public class LotGenealogy
{
    public string GenealogyId { get; set; } = string.Empty;

    public string ParentLotId { get; set; } = string.Empty;
    public string ChildLotId { get; set; } = string.Empty;

    public string RelationType { get; set; } = string.Empty;

    public string OperationId { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;

    public int Qty { get; set; }
    public string UnitOfMeasure { get; set; } = "Unit";

    public string OperatorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public string? ReasonCode { get; set; }
    public string? Remark { get; set; }
}
```

### 12.5 WaferRelation 模型

```csharp
public class WaferRelation
{
    public string Id { get; set; } = string.Empty;

    public string CustomerWaferLotId { get; set; } = string.Empty;
    public string WaferId { get; set; } = string.Empty;

    public string AssemblyLotId { get; set; } = string.Empty;
    public string? TestLotId { get; set; }

    public int DieQty { get; set; }
    public int GoodDieQty { get; set; }
    public int RejectDieQty { get; set; }
}
```

### 12.6 MaterialGenealogy 模型

```csharp
public class MaterialGenealogy
{
    public string Id { get; set; } = string.Empty;

    public string LotId { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;

    public string MaterialType { get; set; } = string.Empty;
    public string MaterialLotId { get; set; } = string.Empty;
    public string SupplierLotId { get; set; } = string.Empty;

    public decimal UsedQty { get; set; }
    public string Unit { get; set; } = string.Empty;

    public string EquipmentId { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public DateTime BoundAt { get; set; }
}
```

### 12.7 CarrierGenealogy 模型

```csharp
public class CarrierGenealogy
{
    public string Id { get; set; } = string.Empty;

    public string LotId { get; set; } = string.Empty;
    public string CarrierType { get; set; } = string.Empty;
    public string CarrierId { get; set; } = string.Empty;

    public string BindAction { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;

    public string OperatorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

### 12.8 服务接口

```csharp
public interface IGenealogyService
{
    Task AddLotRelationAsync(LotGenealogy relation);
    Task AddMaterialRelationAsync(MaterialGenealogy relation);
    Task AddCarrierRelationAsync(CarrierGenealogy relation);

    Task<LotGenealogyTree> GetUpstreamTreeAsync(string lotId);
    Task<LotGenealogyTree> GetDownstreamTreeAsync(string lotId);
    Task<LotTraceSummary> GetTraceSummaryAsync(string lotId);

    Task<List<MaterialGenealogy>> GetMaterialsByLotAsync(string lotId);
    Task<List<CarrierGenealogy>> GetCarriersByLotAsync(string lotId);
}
```

### 12.9 必须接入 Genealogy 的动作

| 动作 | 生成 Genealogy |
|---|---|
| 创建 Assembly Lot | WaferToAssembly |
| 创建 Test Lot | AssemblyToTest |
| Split | Parent → Child |
| Merge | Source Lots → Target Lot |
| Rework | Original Lot → Rework Lot |
| Retest | Test Lot → Retest Lot |
| Scrap | Lot → Scrap Record |
| Packing | Finished Lot → Packing Lot |
| Shipping | Packing Lot → Shipping Lot |
| 绑定物料 | Lot → MaterialLot |
| 绑定载具 | Lot → Carrier |

### 12.10 页面建议

```text
Lot Genealogy 查询
Lot Trace Summary
物料追溯
载具追溯
客户追溯报告
```

### 12.11 验收标准

```text
1. 任意出货批能反查 Wafer Lot。
2. 任意 Lot 能查父批和子批。
3. Split/Merge 后关系不能丢。
4. Rework/Retest 后关系能追。
5. 能查使用的关键物料批次。
6. 能查使用的载具。
7. 客户审计时能导出完整追溯报告。
```

---

## 13. Integration 模块联动线

### 13.1 目标

Production 不能自己说了算。进站/出站必须联动：

- Equipment
- Recipe
- Quality
- Warehouse
- Schedule
- Yield
- Alarm
- Trace
- EAP/SECS/GEM
- ERP/WMS 等外部系统

### 13.2 集成服务

```csharp
public interface IProductionIntegrationService
{
    Task<List<ValidationItem>> ValidateTrackInAsync(ProductionContext context);
    Task<List<ValidationItem>> ValidateTrackOutAsync(ProductionContext context);
    Task NotifyAfterTrackInAsync(ProductionEvent evt);
    Task NotifyAfterTrackOutAsync(ProductionEvent evt);
    Task NotifyAfterHoldAsync(ProductionEvent evt);
}
```

### 13.3 ProductionContext

```csharp
public class ProductionContext
{
    public LotInfo Lot { get; set; } = new();
    public RouteStep Step { get; set; } = new();

    public string EquipmentId { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public string CarrierId { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;

    public int? InputQty { get; set; }
    public int? PassQty { get; set; }
    public int? FailQty { get; set; }
    public int? ScrapQty { get; set; }
}
```

### 13.4 统一校验结果

```csharp
public class ValidationItem
{
    public string Source { get; set; } = string.Empty;
    public string CheckCode { get; set; } = string.Empty;
    public string CheckName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty; // Pass, Warning, Fail
    public string Message { get; set; } = string.Empty;
    public string Suggestion { get; set; } = string.Empty;

    public bool CanOverride { get; set; }
    public string? RequiredRoleToOverride { get; set; }
}
```

### 13.5 Equipment 联动

TrackIn 必须校验：

```text
设备存在
设备状态为 Idle / Available
设备不在 Down
设备不在 PM
设备属于当前工序设备组
设备没有被其他 Lot 占用
设备 EAP 通讯正常
设备 Recipe 下载状态正常
```

接口：

```csharp
public interface IEquipmentGateway
{
    Task<EquipmentDto?> GetEquipmentAsync(string equipmentId);
    Task<bool> IsEquipmentAvailableAsync(string equipmentId);
    Task<bool> IsEquipmentInGroupAsync(string equipmentId, string equipmentGroup);
    Task<bool> IsEquipmentOccupiedAsync(string equipmentId);

    Task BindLotAsync(string equipmentId, string lotId);
    Task ReleaseLotAsync(string equipmentId, string lotId);
}
```

### 13.6 Recipe 联动

TrackIn 必须校验：

```text
Recipe 存在
Recipe 已审批
Recipe Version 正确
Recipe 适配当前产品
Recipe 适配当前工序
Recipe 适配当前设备
Recipe 未过期
Recipe 参数未被锁定或异常
```

如果设备支持 EAP，应做到：

```text
进站前校验设备当前 Recipe
如果不一致，触发 Recipe 下载
下载成功后才允许进站
Recipe 下载失败禁止进站
```

接口：

```csharp
public interface IRecipeGateway
{
    Task<RecipeValidationResult> ValidateRecipeAsync(
        string productId,
        string stepCode,
        string equipmentId,
        string recipeId);

    Task<RecipeDownloadResult> DownloadRecipeAsync(
        string equipmentId,
        string recipeId);
}
```

### 13.7 Quality 联动

TrackIn 必须校验：

```text
Lot 没有未关闭 Quality Hold
没有未关闭 OOC/OOS
当前 Step 的 QA Gate 已放行
首件检验如果要求，必须完成
```

TrackOut 必须校验：

```text
SPC/FDC 是否触发异常
良率是否低于目标
不良率是否超阈值
是否需要自动 Hold
是否需要抽检
```

接口：

```csharp
public interface IQualityGateway
{
    Task<bool> HasOpenQualityIssueAsync(string lotId);
    Task<bool> IsQualityGatePassedAsync(string lotId, string stepCode);
    Task<QualityCheckResult> CheckTrackOutQualityAsync(TrackOutRequest request);
    Task CreateAutoHoldIfNeededAsync(string lotId, QualityCheckResult result);
}
```

### 13.8 Warehouse 联动

TrackIn 必须校验：

```text
载具有效
载具未被其他 Lot 占用
载具和 Lot 绑定正确
当前 Step 所需物料齐套
物料批号未过期
物料批号未 Hold
物料剩余数量足够
```

封测关键物料包括：

```text
LeadFrame
Substrate
GoldWire
CopperWire
EMC
DAF
Epoxy
Tray
Reel
Tube
Label
PackingBox
```

接口：

```csharp
public interface IWarehouseGateway
{
    Task<bool> IsCarrierValidAsync(string carrierId);
    Task<bool> IsCarrierAvailableAsync(string carrierId);
    Task<bool> IsCarrierBoundToLotAsync(string carrierId, string lotId);

    Task<MaterialCheckResult> CheckMaterialReadyAsync(string lotId, string stepCode);
    Task ConsumeMaterialsAsync(MaterialConsumeRequest request);
}
```

### 13.9 Yield 联动

TrackOut 后：

```text
计算当前站良率
计算累计良率
按产品/客户/工序阈值判断
低于阈值自动 Yield Hold
推送 Yield 模块
```

接口：

```csharp
public interface IYieldGateway
{
    Task<YieldCheckResult> CheckYieldAsync(
        string lotId,
        string stepCode,
        int inputQty,
        int passQty,
        int failQty);

    Task RecordYieldAsync(YieldRecord record);
}
```

### 13.10 Alarm 联动

异常时自动发报警：

```text
Low Yield
Queue Time 超时
Hold 超时
设备 Down
Recipe 下载失败
物料不齐套
客户急件超期
强制作业
数量不平衡尝试
```

接口：

```csharp
public interface IAlarmGateway
{
    Task RaiseAlarmAsync(AlarmRequest request);
    Task ClearAlarmAsync(string alarmId);
}
```

### 13.11 验收标准

```text
1. 错设备不能进站。
2. 设备 Down/PM 不能进站。
3. Recipe 未审批不能进站。
4. Recipe 与设备不匹配不能进站。
5. QA Hold 未释放不能进站。
6. 物料不齐套不能进站。
7. 低良率能自动 Hold。
8. Queue Time 超时能报警。
9. 所有联动失败都有明确提示和处理建议。
```

---

## 14. Customer 客户交付线

### 14.1 目标

解决以下问题：

```text
客户这批货在哪里？
什么时候能完成？
为什么 Delay？
有没有 Hold？
良率多少？
出货数量多少？
是否满足客户特殊要求？
```

### 14.2 客户相关字段

WorkOrder 需要：

```text
CustomerId
CustomerName
CustomerPO
CustomerOrderNo
CustomerPN
InternalPN
CustomerLotId
WaferLotId
DueDate
CommitDate
Priority
CustomerSpecialInstruction
PackingSpec
MarkingSpec
OqcSpec
TestSpec
BinSpec
GradeSpec
```

Lot 需要：

```text
CustomerLotId
CustomerSubLotId
CustomerDevice
CustomerPackage
CustomerGrade
CustomerDueDate
CustomerHoldFlag
ShipmentLotId
PackingLotId
```

### 14.3 客户批次进度 DTO

```csharp
public class CustomerLotProgressDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPoNo { get; set; } = string.Empty;
    public string CustomerLotId { get; set; } = string.Empty;
    public string CustomerPN { get; set; } = string.Empty;
    public string InternalPN { get; set; } = string.Empty;

    public string LotId { get; set; } = string.Empty;
    public string WorkOrderId { get; set; } = string.Empty;

    public string CurrentStepName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public int OriginalQty { get; set; }
    public int CurrentQty { get; set; }
    public int CompletedQty { get; set; }
    public int ScrapQty { get; set; }
    public int HoldQty { get; set; }

    public double ProgressPercent { get; set; }

    public DateTime? DueDate { get; set; }
    public DateTime? EstimatedCompleteTime { get; set; }

    public string RiskLevel { get; set; } = "Normal";
    public string? RiskReason { get; set; }

    public bool IsHold { get; set; }
    public string? HoldReason { get; set; }
    public string? HoldOwner { get; set; }
}
```

### 14.4 交期风险算法

基础算法：

```text
预计完成时间 = 当前时间
           + 当前工序剩余等待时间
           + 当前工序标准加工时间
           + 后续工序标准 Queue Time
           + 后续工序标准加工时间
           + OQC/Packing 标准时间
```

风险等级：

```text
Normal:
预计完成时间 <= DueDate - 安全缓冲

Warning:
预计完成时间接近 DueDate

High:
预计完成时间 > DueDate

Critical:
已超 DueDate 或客户急件仍 Hold
```

风险原因：

```text
Hold 超时
Queue Time 超时
设备瓶颈
物料不齐
Recipe 未审批
QA 未放行
低良率异常
急件未投产
工单未释放
```

### 14.5 客户特殊要求管控

特殊要求示例：

```text
指定 Test Program
指定温度条件
指定 Packing Spec
指定 Marking 内容
指定 Label 格式
指定 OQC 抽样标准
指定 Bin/Grade 规则
不允许混批
不允许 Split/Merge
必须客户 Release 才能继续
```

模型：

```csharp
public class CustomerRequirement
{
    public string RequirementId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;

    public string RequirementType { get; set; } = string.Empty;
    public string RequirementValue { get; set; } = string.Empty;

    public string ControlPoint { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }

    public string? EffectiveFrom { get; set; }
    public string? EffectiveTo { get; set; }

    public bool IsActive { get; set; }
}
```

### 14.6 服务接口

```csharp
public interface ICustomerProductionService
{
    Task<List<CustomerLotProgressDto>> SearchCustomerLotsAsync(CustomerLotQuery query);
    Task<CustomerLotDetailDto> GetCustomerLotDetailAsync(string lotId);
    Task<List<DueDateRiskDto>> GetDueDateRiskLotsAsync();
    Task<CustomerTraceReport> GenerateTraceReportAsync(string customerLotId);
    Task<List<CustomerHoldDto>> GetCustomerHoldsAsync(string customerId);
}
```

### 14.7 页面建议

```text
客户批次进度
客户 WIP 汇总
客户 Hold 明细
客户交期风险
客户出货追溯报告
```

### 14.8 验收标准

```text
1. 能按客户 Lot 查询内部 Lot。
2. 能按客户 PO 查询工单进度。
3. 能看到当前工序、状态、数量、预计完成时间。
4. 能识别交期风险。
5. 能解释 Delay 原因。
6. 能查询客户 Hold。
7. 能导出客户追溯报告。
8. 能管控客户特殊要求。
```

---

# 第三部分：正式生产上线补充能力

---

## 15. Master Data 主数据线

### 15.1 为什么必须有

没有主数据，系统会变成到处手输：

```text
设备组手输
工序手输
Recipe 手输
客户料号手输
原因代码手输
物料类型手输
```

正式 MES 必须主数据化。

### 15.2 必须维护的主数据

```text
Customer
Product
CustomerPN / InternalPN
PackageType
Route
RouteStep
Equipment
EquipmentGroup
Recipe
Material
MaterialSpec
Carrier
ReasonCode
HoldReason
ScrapReason
ReworkReason
User
Role
Permission
Shift
Calendar
```

### 15.3 验收标准

```text
1. 所有生产关键字段不能随便手输。
2. Route/Recipe/Equipment/ReasonCode 必须来自主数据。
3. 主数据变更有审批和版本。
4. 旧批次使用旧版本，新批次使用新版本。
```

---

## 16. Dispatch/Planning 派工计划线

### 16.1 为什么必须有

生产现场不只问“能不能做”，还问：

```text
下一批做哪一个？
急件先跑哪个？
哪台设备最适合？
哪个客户快 delay？
哪个批次 Queue Time 快超？
```

### 16.2 派工评分

```text
Score =
客户优先级权重
+ 交期风险权重
+ Queue Time 权重
+ Hold 后释放权重
+ 设备可用性权重
+ Recipe Ready 权重
+ 物料齐套权重
```

### 16.3 页面建议

```text
Dispatch Board
工序派工清单
设备负载看板
交期风险看板
```

### 16.4 验收标准

```text
1. 领班能看到每个工序推荐生产批次。
2. 急件和交期风险批次排前。
3. 不可生产批次不会被推荐。
4. 设备不可用时不会派工。
```

---

## 17. Reliability/Operation 运维可靠性线

### 17.1 为什么必须有

MES 是 7x24 系统，不能只看业务功能。

### 17.2 必须具备

```text
正式数据库
备份恢复
权限隔离
日志监控
异常报警
接口重试
客户端崩溃日志
服务健康检查
数据库连接监控
Redis 监控
发布回滚
测试环境/生产环境隔离
```

### 17.3 验收标准

```text
1. 数据库每日备份。
2. 关键服务异常有报警。
3. 操作失败可查日志。
4. 网络中断不造成重复出站。
5. 发布失败可回滚。
6. 权限配置不影响生产数据安全。
```

---

## 18. Report/BI 报表线

### 18.1 为什么必须有

管理层、客户、质量、计划都需要报表。

### 18.2 必须有的报表

```text
WIP Summary
Output Report
Daily Production Report
Hold Aging Report
Scrap Report
Rework Report
Yield Report
Cycle Time Report
Queue Time Report
Customer Lot Progress
Due Date Risk Report
Equipment Utilization
Operator Activity
Audit Report
```

### 18.3 验收标准

```text
1. 主管能看日达成率。
2. QA 能看 Hold Aging。
3. 计划能看交期风险。
4. 客户服务能导出客户批次进度。
5. IT/QA 能导出审计日志。
```

---

## 19. Data Governance 数据治理线

### 19.1 为什么必须有

MES 长期运行后会面临：

```text
历史数据越来越多
查询越来越慢
客户审计要查旧数据
测试数据文件很大
STDF/ATDF 数据归档
Audit 数据不能删
```

### 19.2 建议方案

```text
冷热数据分层
历史数据归档
审计数据保留策略
客户数据权限隔离
数据字典
数据质量检查
批次唯一性检查
重复操作检查
```

### 19.3 验收标准

```text
1. 3 年前批次还能查追溯。
2. 当前 WIP 查询不被历史数据拖慢。
3. Audit 不能被普通用户改。
4. 客户数据权限隔离。
5. 批次号、客户 Lot、工单号唯一性受控。
```

---

# 第四部分：关键业务场景

---

## 20. 正常生产流程

```text
1. Planner 创建工单 WO-001
2. 系统创建 Lot LOT-001
3. Lot 绑定 Route: BGA-STD-V1
4. Lot 当前 Step = Saw
5. 操作员扫码 LOT-001 + SAW-01 + Frame-001
6. 系统校验：
   - Lot 存在
   - Lot 未 Hold
   - 当前 Step = Saw
   - SAW-01 属于 Saw 设备组
   - Recipe 已审批
   - Carrier 有效
7. TrackIn 成功
8. Lot.Status = Processing
9. 操作员出站，输入数量
10. 系统校验数量平衡
11. TrackOut 成功
12. Lot 当前 Step 自动变成 DieAttach
13. 写 OperationHistory / AuditTrail / QuantityTransaction
```

---

## 21. Hold 流程

```text
1. QA 发现 WireBond 异常
2. QA 对 LOT-001 发起 Quality Hold
3. Lot.Status = Hold
4. HoldRecord.Status = Open
5. 操作员尝试进站 Mold
6. 系统禁止进站，提示 QA Hold
7. QA 处理完成，填写 RootCause / Action
8. QA 主管电子签核 Release
9. Lot.Status = Waiting
10. HoldRecord.Status = Released
11. Lot 可继续流转
```

---

## 22. Scrap 流程

```text
1. Mold 出站发现 50 pcs 报废
2. 操作员输入 ScrapQty = 50
3. 系统检查数量平衡
4. Scrap 超阈值，需要主管签核
5. 主管签核
6. 写 ScrapRecord
7. 写 QuantityTransaction
8. Lot.CurrentQty 扣减
9. 写 AuditTrail
```

---

## 23. Split 拆批流程

```text
1. 选择母批 LOT-001
2. 输入子批 LOT-001-A / LOT-001-B 和数量
3. 系统校验子批数量合计 <= 母批 CurrentQty
4. 需要电子签核
5. 创建子批
6. 更新母批数量
7. 写 LotGenealogy
8. 写 QuantityTransaction
9. 写 OperationHistory / AuditTrail
```

---

## 24. Merge 合批流程

```text
1. 选择多个来源批次
2. 系统校验客户、产品、Route、Step 是否一致
3. Hold 批禁止合批
4. 创建目标批或选择主批
5. 写 LotGenealogy
6. 写 QuantityTransaction
7. 写 OperationHistory / AuditTrail
```

---

## 25. Rework 重工流程

```text
1. 选择需要重工的 Lot
2. 选择 Rework Route
3. 填写重工原因
4. 工程/QA 电子签核
5. Lot 当前 Step 改到 Rework Route 首站
6. 写 ReworkRecord
7. 写 LotGenealogy
8. 写 OperationHistory / AuditTrail
```

---

# 第五部分：页面结构建议

---

## 26. Production 菜单建议

```text
生产管理
├─ 生产作业
│  ├─ 进站/出站
│  ├─ 批次查询
│  ├─ 批次履历
│  └─ 强制作业审批
│
├─ 工单与批次
│  ├─ 工单管理
│  ├─ 批次管理
│  ├─ 批次拆分
│  ├─ 批次合批
│  └─ 工单结案
│
├─ WIP 管理
│  ├─ WIP 总览
│  ├─ 派工清单
│  ├─ 瓶颈分析
│  └─ 交期风险
│
├─ Hold 管理
│  ├─ 批次 Hold
│  ├─ Hold Aging
│  └─ Release 审批
│
├─ 异常处理
│  ├─ 重工管理
│  ├─ 重测管理
│  ├─ 报废管理
│  └─ MRB 判定
│
├─ 追溯管理
│  ├─ Lot Genealogy
│  ├─ 物料追溯
│  ├─ 载具追溯
│  └─ 客户追溯报告
│
└─ 客户查询
   ├─ 客户批次进度
   ├─ 客户 WIP
   └─ 客户 Hold 明细
```

---

# 第六部分：推荐实施路线

---

## 27. V1：生产执行最小闭环

目标：Lot 可以真实进站/出站。

必须做：

```text
1. Route/Step 表
2. Lot 当前 Step
3. TrackIn/TrackOut 服务
4. OperationHistory
5. 数量平衡
6. 基础 Hold 禁止进站
7. 基础 AuditTrail
```

页面：

```text
进站/出站
批次查询
批次履历
WIP 总览
```

验收标准：

```text
Lot 不能跳站
Lot 不能重复进站
Hold Lot 不能进站
TrackIn 后 Lot 状态正确
TrackOut 后 Lot 自动进入下一站
数量不平衡不能出站
每次操作有履历
每次关键操作有 AuditTrail
```

---

## 28. V2：异常闭环

目标：生产异常能管住。

增加：

```text
1. HoldRecord
2. Release 审批
3. 电子签核
4. Scrap
5. Rework
6. ForceTrackIn/ForceTrackOut
7. 动作级权限
```

页面：

```text
批次 Hold
Release 审批
报废管理
重工管理
强制作业审批
```

验收标准：

```text
QA Hold 只能 QA Release
Customer Hold 需要主管/客户服务签核
Scrap 需要原因代码
超过阈值 Scrap 需要审批
强制进站必须电子签核
所有异常操作可追溯
```

---

## 29. V3：追溯和批次变更

目标：客户审计能回答清楚。

增加：

```text
1. Lot Genealogy
2. Split
3. Merge
4. Retest
5. Material Binding
6. Carrier Binding
```

页面：

```text
批次拆分
批次合批
Lot Genealogy
物料/载具追溯
```

验收标准：

```text
拆批后可查父子关系
合批后可查来源批次
重工后可查重工路线
报废后数量可平衡
客户审计可查完整 genealogy
```

---

## 30. V4：联动和客户交付

目标：更接近正式封测 MES。

增加：

```text
1. Equipment 联动
2. Recipe 联动
3. Quality 联动
4. Warehouse 联动
5. 客户批次查询
6. 交期风险
7. 派工清单
```

页面：

```text
客户批次进度
交期风险
派工清单
瓶颈分析
```

验收标准：

```text
错设备不能进站
错 Recipe 不能进站
物料未齐套不能进站
质量 Gate 未放行不能进站
客户可查批次进度
系统可识别交期风险
```

---

## 31. V5：正式上线支撑能力

目标：支撑封测厂长期生产使用。

增加：

```text
1. Master Data 主数据
2. Dispatch/Planning 派工计划
3. Reliability/Operation 运维可靠性
4. Report/BI 报表
5. Data Governance 数据治理
6. ERP/EAP/WMS/STDF/OQC/Shipping 接口
```

验收标准：

```text
主数据受控
派工可用
报表可用
系统可监控
数据可备份恢复
历史数据可归档查询
客户审计可应对
```

---

# 第七部分：推荐数据库表清单

---

## 32. Production 核心表

```text
Production_WorkOrder
Production_Lot
Production_Route
Production_RouteStep
Production_LotStepRecord
Production_OperationRecord
Production_QuantityTransaction
Production_HoldRecord
Production_ElectronicSignature
Production_AuditTrail
Production_LotGenealogy
Production_ScrapRecord
Production_ReworkRecord
Production_SplitMergeRecord
Production_CustomerRequirement
Production_CustomerLotProgressView
```

## 33. 集成和主数据相关表

```text
Master_Customer
Master_Product
Master_CustomerPart
Master_InternalPart
Master_Equipment
Master_EquipmentGroup
Master_Recipe
Master_Material
Master_Carrier
Master_ReasonCode
Master_User
Master_Role
Master_Permission
Master_Shift
Master_Calendar
```

---

# 第八部分：最终结论

---

## 34. 是否能成为封测厂实际能用 MES？

### 34.1 只做当前 Production 原型

```text
不能作为封测厂正式生产上线 MES。
```

适合：

```text
Demo
原型评审
内部流程讨论
培训模拟
```

### 34.2 完成 V1-V2

```text
可以支撑生产执行核心试运行。
```

但仍缺客户审计、谱系追溯、模块联动。

### 34.3 完成 V1-V4

```text
可以作为封测厂小范围试点 MES。
```

适合：

```text
一条线
一个车间
部分产品
内部受控试运行
```

### 34.4 完成 V1-V5

```text
可以作为封测厂正式生产上线 MES 的基础方案。
```

但仍需结合实际工厂补齐：

```text
ERP 接口
EAP/SECS/GEM 接口
WMS 接口
STDF/ATDF 测试数据接口
OQC/Shipping 接口
标签系统
客户系统接口
```

---

## 35. 最终上线判断标准

一个封测厂实际可用 MES 至少要满足：

### 生产能跑

```text
Lot 按 Route 走
能进站/出站
能防错站/错设备/错 Recipe
能 Hold/Release
能出站数量平衡
```

### 异常能控

```text
能 Scrap
能 Rework
能 Retest
能 Split/Merge
能低良率 Hold
能电子签核
```

### 数据能追

```text
能查 Lot 履历
能查 Genealogy
能查设备
能查 Recipe
能查物料
能查操作人
能查 Audit
```

### 客户能交代

```text
能查客户批次进度
能查交期风险
能查 Hold 原因
能导出追溯报告
能管控客户特殊要求
```

### 系统能运维

```text
正式数据库
备份恢复
权限控制
日志监控
发布回滚
性能稳定
数据归档
```

---

## 36. 最终建议

推荐推进路线：

```text
第一阶段：1-5 条线，打通生产执行闭环
第二阶段：6-9 条线，补审计、谱系、联动、客户交付
第三阶段：10-14 条线，补主数据、派工、运维、报表、数据治理
第四阶段：接 ERP/EAP/WMS/STDF/OQC/Shipping，进入真实工厂集成
```

做到第二阶段，可以试点；做到第三阶段，可以正式上线；做到第四阶段，才算更完整的封测厂 MES。