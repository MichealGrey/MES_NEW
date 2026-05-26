# MES V2 Phase 2 异常闭环 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现半导体封测 MES 异常闭环，包含完整 Hold/Release、电子签核、Scrap、Rework、ForceTrack、完整 Route（含 Rework Route）、Seed 全流程数据，使 Lot 能按 12 步 Route 完整流转，所有异常可管控可追溯。

**Architecture:** 保留 MySQL KV 存储不变，在现有 IRedisService 接口之上扩展 8 个新数据模型 + 7 个新服务，增强现有 TrackService 校验链和处理链，重写 Seed 数据完整展示 QFN-STD-V2.0 全流程。

**Tech Stack:** .NET 8, WPF+Prism, MySQL (Pomelo EF Core), IRedisService 抽象层

---

## 文件结构总览

### 新建文件（16 个）

| # | 文件 | 职责 |
|---|------|------|
| 1 | `Models/LotSplitRecord.cs` | 拆批记录模型 |
| 2 | `Models/LotMergeRecord.cs` | 合批记录模型 |
| 3 | `Models/LotCarrierBinding.cs` | 载具绑定记录模型 |
| 4 | `Models/SignatureRecord.cs` | 电子签核记录模型 |
| 5 | `Models/YieldRule.cs` | 良率规则模型 |
| 6 | `Models/ScrapRecord.cs` | 报废记录模型 |
| 7 | `Models/ReworkRecord.cs` | 重工记录模型 |
| 8 | `Models/LotGenealogy.cs` | 谱系关系模型 |
| 9 | `Services/ISignatureService.cs` | 签核服务接口 |
| 10 | `Services/IYieldService.cs` | 良率服务接口 |
| 11 | `Services/IGenealogyService.cs` | 谱系服务接口 |
| 12 | `Services/SignatureService.cs` | 签核服务实现 |
| 13 | `Services/YieldService.cs` | 良率服务实现 |
| 14 | `Services/GenealogyService.cs` | 谱系服务实现 |
| 15 | `docs/V2_Seed_Data_Spec.md` | Seed 数据规格文档 |
| 16 | `docs/V2_Service_API_Reference.md` | 新增服务 API 参考 |

### 修改文件（8 个）

| # | 文件 | 修改内容 |
|---|------|---------|
| 1 | `Models/LotInfo.cs` | 新增 14 个字段（MotherLotId, IsPartialLot, SplitReason, ReworkRouteId, ReworkCount, IsUnderMRB, MRBDisposition, Grade, OriginalQty, TotalPassQty, TotalScrapQty, TotalReworkQty, TotalHoldQty, WaferLotId） |
| 2 | `Models/RouteStep.cs` | 新增 8 个字段（YieldThreshold, RequireSplit, AllowMerge, ReworkRouteId, ReworkTargetStep, RequiredCarrierType, EnableMRB, MRBThreshold, RequiredSignatureLevel） |
| 3 | `Models/HoldRecord.cs` | 新增 EscalationLevel, HoldExpiryTime 字段 |
| 4 | `Services/TrackService.cs` | TrackIn 校验链从 5 项→12 项，TrackOut 处理链从 3 项→11 项 |
| 5 | `Services/ProductionDataService.cs` | 重写 Seed：QFN-STD-V2.0(12步) + 3 条 Rework Route + 6 个种子批次 + YieldRule 数据 |
| 6 | `ProductionModule.cs` | 注册新服务 |
| 7 | `Models/TrackRequest.cs` | 新增 RouteId, RouteVersion, StepSeq, StepCode, StepName, InputQty, Remark |
| 8 | `Models/TrackResult.cs` | 新增 NextStepName（已有，确认保留） |

---

## Phase 2-A：数据模型扩展

### Task 1：扩展 LotInfo 模型

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/Models/LotInfo.cs`

- [ ] **Step 1：在 LotInfo.cs 中新增 14 个字段**

在 `public bool IsFirstStep => CurrentStepSeq <= 1;` 之后插入：

```csharp
    // --- Split/Merge 追踪 ---
    public bool IsPartialLot { get; set; }                    // 是否为部分批次
    public string? MotherLotId { get; set; }                  // 母批次ID（Split后子批指向母批）
    public string? SplitReason { get; set; }                  // 拆批原因
    public DateTime? SplitTime { get; set; }
    public int? SplitQty { get; set; }                        // 拆分数量
    
    // --- Rework ---
    public bool IsReworkLot { get; set; }                     // 是否为重工批次
    public string? OriginalRouteId { get; set; }              // 原始路线ID
    public string? ReworkRouteId { get; set; }                // 当前重工路线ID
    public int? ReworkCount { get; set; }                     // 重工次数
    public string? ReworkReason { get; set; }

    // --- MRB ---
    public bool IsUnderMRB { get; set; }                      // 是否处于MRB审查
    public string? MRBReference { get; set; }                 // MRB单号
    public string? MRBDisposition { get; set; }               // MRB结论: UseAsIs/Rework/Scrap/ReturnToVendor

    // --- Grade ---
    public string? Grade { get; set; }                        // 等级: A(车规)/B(工业)/C(消费)
    public string? OriginalLotId { get; set; }                // Grade Split前的原始批次ID

    // --- Wafer 关联 ---
    public string? WaferLotId { get; set; }                   // 来料晶圆批次号

    // --- 数量累计 ---
    public int OriginalQty { get; set; }                      // 初始投入数量
    public int TotalPassQty { get; set; }                     // 累计合格
    public int TotalScrapQty { get; set; }                    // 累计报废
    public int TotalReworkQty { get; set; }                   // 累计重工
    public int TotalHoldQty { get; set; }                     // 累计Hold

    // --- 良率（只读计算） ---
    [JsonIgnore]
    public double CurrentYield => OriginalQty > 0 
        ? (double)(OriginalQty - TotalScrapQty) / OriginalQty * 100 : 100;

    // --- 扩展状态 ---
    [JsonIgnore]
    public bool IsHold => Status == "Hold";
    [JsonIgnore]
    public bool IsCompleted => Status == "Completed";
    [JsonIgnore]
    public bool IsScrapped => Status == "Scrapped";
    [JsonIgnore]
    public bool CanTrackIn => Status is "Waiting" or "Released" && !IsHold && !IsUnderMRB;
```

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 2：扩展 RouteStep 模型

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/Models/RouteStep.cs`

- [ ] **Step 1：在 RouteStep.cs 末尾新增字段**

在 `public int QueueTimeLimitMinutes { get; set; }` 之后插入：

```csharp
    // --- 良率控制 ---
    public int? YieldThreshold { get; set; }                  // 良率阈值(%)，低于则自动Hold

    // --- Split/Merge ---
    public bool RequireSplit { get; set; }                    // 是否必须拆批（如QA取样）
    public bool AllowMerge { get; set; }                      // 是否允许合批
    
    // --- 重工 ---
    public string? ReworkRouteId { get; set; }                // 关联重工路线ID
    public string? ReworkTargetStep { get; set; }             // 重工后回到哪一步（StepName）
    
    // --- 载具 ---
    public string RequiredCarrierType { get; set; } = string.Empty; // 必须载具类型: FOUP/TapeFrame/Magazine等
    
    // --- MRB ---
    public bool EnableMRB { get; set; }                       // 是否允许MRB
    public int? MRBThreshold { get; set; }                    // 不良超过多少触发MRB

    // --- 签核 ---
    public string? RequiredSignatureLevel { get; set; }       // "Level1"/"Level2"/"Level3"/"Level0"
```

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 3：创建 8 个新数据模型

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Models/LotSplitRecord.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/LotMergeRecord.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/LotCarrierBinding.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/SignatureRecord.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/YieldRule.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/ScrapRecord.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/ReworkRecord.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/LotGenealogy.cs`

- [ ] **Step 1：创建 LotSplitRecord.cs**

```csharp
namespace MES.Modules.Production.Models;

public class LotSplitRecord
{
    public string SplitId { get; set; } = Guid.NewGuid().ToString("N");
    public string MotherLotId { get; set; } = string.Empty;
    public string ChildLotId { get; set; } = string.Empty;
    public int SplitQty { get; set; }
    public string SplitReason { get; set; } = string.Empty;    // LowYield/QA Sample/Engineering/MRB
    public string SplitType { get; set; } = string.Empty;      // Partial/Grade/Rework
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime SplitTime { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
}
```

- [ ] **Step 2：创建 LotMergeRecord.cs**

```csharp
namespace MES.Modules.Production.Models;

public class LotMergeRecord
{
    public string MergeId { get; set; } = Guid.NewGuid().ToString("N");
    public string TargetLotId { get; set; } = string.Empty;
    public List<string> SourceLotIds { get; set; } = [];
    public int MergedQty { get; set; }
    public string MergeReason { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime MergeTime { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
}
```

- [ ] **Step 3：创建 LotCarrierBinding.cs**

```csharp
namespace MES.Modules.Production.Models;

public class LotCarrierBinding
{
    public string BindingId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string CarrierId { get; set; } = string.Empty;
    public string CarrierType { get; set; } = string.Empty;
    public string? FromCarrierId { get; set; }
    public DateTime BindTime { get; set; } = DateTime.UtcNow;
    public DateTime? UnbindTime { get; set; }
    public string OperatorId { get; set; } = string.Empty;
}
```

- [ ] **Step 4：创建 SignatureRecord.cs**

```csharp
namespace MES.Modules.Production.Models;

public class SignatureRecord
{
    public string SignatureId { get; set; } = Guid.NewGuid().ToString("N");
    public string EntityType { get; set; } = string.Empty;     // Split/Merge/Rework/Scrap/Hold/Release/ForceTrackIn/ForceTrackOut
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
    public bool IsDualApproved => !string.IsNullOrEmpty(SecondSignerId);
}
```

- [ ] **Step 5：创建 YieldRule.cs**

```csharp
namespace MES.Modules.Production.Models;

public class YieldRule
{
    public string RuleId { get; set; } = string.Empty;
    public string RouteId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public double YieldThreshold { get; set; }                 // 良率阈值%
    public string ActionType { get; set; } = "AutoHold";       // AutoHold/Alert/Block
    public string NotifyRole { get; set; } = "QA";
    public bool IsActive { get; set; } = true;
}
```

- [ ] **Step 6：创建 ScrapRecord.cs**

```csharp
namespace MES.Modules.Production.Models;

public class ScrapRecord
{
    public string ScrapId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public int ScrapQty { get; set; }
    public string ScrapReason { get; set; } = string.Empty;
    public string ScrapReasonCode { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public DateTime ScrapTime { get; set; } = DateTime.UtcNow;
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
    public bool RequiresApproval { get; set; }
}
```

- [ ] **Step 7：创建 ReworkRecord.cs**

```csharp
namespace MES.Modules.Production.Models;

public class ReworkRecord
{
    public string ReworkId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string OriginalRouteId { get; set; } = string.Empty;
    public string ReworkRouteId { get; set; } = string.Empty;
    public string FromStepCode { get; set; } = string.Empty;
    public string TargetStepCode { get; set; } = string.Empty;
    public int ReworkQty { get; set; }
    public string ReworkReason { get; set; } = string.Empty;
    public string OperatorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? ApprovedBy { get; set; }
    public string? SignatureId { get; set; }
    public int ReworkCount { get; set; }
}
```

- [ ] **Step 8：创建 LotGenealogy.cs**

```csharp
namespace MES.Modules.Production.Models;

public class LotGenealogy
{
    public string GenealogyId { get; set; } = Guid.NewGuid().ToString("N");
    public string ParentLotId { get; set; } = string.Empty;
    public string ChildLotId { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;   // Split/Merge/GradeSplit/Rework/WaferToLot
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public int Qty { get; set; }
    public string? Grade { get; set; }
    public string? WaferLotId { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ReasonCode { get; set; }
    public string? Remark { get; set; }
}
```

- [ ] **Step 9：扩展 TrackRequest.cs**

在现有 TrackInRequest 和 TrackOutRequest 中新增字段：

```csharp
// TrackInRequest 新增:
public string RouteId { get; set; } = string.Empty;
public string RouteVersion { get; set; } = "1.0";
public int StepSeq { get; set; }
public string StepCode { get; set; } = string.Empty;
public string StepName { get; set; } = string.Empty;
public int InputQty { get; set; }
public string? Remark { get; set; }

// TrackOutRequest 新增:
public string RouteId { get; set; } = string.Empty;
public string RouteVersion { get; set; } = "1.0";
public int StepSeq { get; set; }
public string StepCode { get; set; } = string.Empty;
public string StepName { get; set; } = string.Empty;
public int PendingQty { get; set; }
```

- [ ] **Step 10：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 2-B：新增服务层

### Task 4：创建 ISignatureService + 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/ISignatureService.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/SignatureService.cs`

- [ ] **Step 1：创建 ISignatureService.cs**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface ISignatureService
{
    Task<SignatureRecord> RequestSignatureAsync(string entityType, string entityId,
        string level, string signerId, string signerName, string signerRole,
        string reason, string? comment = null);
    Task<bool> VerifySignatureAsync(string entityType, string entityId, string requiredLevel);
    Task<List<SignatureRecord>> GetSignaturesAsync(string entityType, string entityId);
}
```

- [ ] **Step 2：创建 SignatureService.cs**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class SignatureService : ISignatureService
{
    private readonly IRedisService _redis;
    private const string SignatureKeyPrefix = "mes:signature:";

    private static string SignatureKey(string entityType, string entityId) =>
        $"{SignatureKeyPrefix}{entityType}:{entityId}";

    public SignatureService(IRedisService redis) => _redis = redis;

    public async Task<SignatureRecord> RequestSignatureAsync(string entityType, string entityId,
        string level, string signerId, string signerName, string signerRole,
        string reason, string? comment = null)
    {
        var record = new SignatureRecord
        {
            EntityType = entityType,
            EntityId = entityId,
            Level = level,
            SignerId = signerId,
            SignerName = signerName,
            SignerRole = signerRole,
            Reason = reason,
            Comment = comment ?? string.Empty,
            SignTime = DateTime.UtcNow
        };

        var key = SignatureKey(entityType, entityId);
        var json = System.Text.Json.JsonSerializer.Serialize(record);
        await _redis.ListRightPushAsync(key, json);

        return record;
    }

    public async Task<bool> VerifySignatureAsync(string entityType, string entityId, string requiredLevel)
    {
        var key = SignatureKey(entityType, entityId);
        var records = await _redis.ListRangeAsync(key);
        if (records.Count == 0) return false;

        // 检查是否有符合 requiredLevel 或更高级别的签核
        foreach (var json in records)
        {
            var record = System.Text.Json.JsonSerializer.Deserialize<SignatureRecord>(json);
            if (record is not null && IsLevelSufficient(record.Level, requiredLevel))
                return true;
        }
        return false;
    }

    public async Task<List<SignatureRecord>> GetSignaturesAsync(string entityType, string entityId)
    {
        var key = SignatureKey(entityType, entityId);
        var records = await _redis.ListRangeAsync(key);
        var results = new List<SignatureRecord>();
        foreach (var json in records)
        {
            var record = System.Text.Json.JsonSerializer.Deserialize<SignatureRecord>(json);
            if (record is not null) results.Add(record);
        }
        return results;
    }

    private static bool IsLevelSufficient(string actual, string required)
    {
        var levelOrder = new Dictionary<string, int>
        {
            { "Level0", 0 }, { "Level1", 1 }, { "Level2", 2 }, { "Level3", 3 }
        };
        if (!levelOrder.TryGetValue(actual, out var a)) a = 0;
        if (!levelOrder.TryGetValue(required, out var r)) r = 0;
        return a >= r;
    }
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 5：创建 IYieldService + 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/IYieldService.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/YieldService.cs`

- [ ] **Step 1：创建 IYieldService.cs**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IYieldService
{
    Task<double> CalculateStepYieldAsync(int passQty, int inputQty);
    Task<YieldRule?> CheckYieldRuleAsync(string routeId, string stepCode, double actualYield);
    Task<bool> ShouldAutoHoldAsync(string routeId, string stepCode, double actualYield);
    Task<List<YieldRule>> GetYieldRulesAsync(string routeId);
}
```

- [ ] **Step 2：创建 YieldService.cs**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class YieldService : IYieldService
{
    private readonly IRedisService _redis;
    private const string YieldRuleKeyPrefix = "mes:yieldrule:";

    private static string YieldRuleKey(string routeId, string stepCode) =>
        $"{YieldRuleKeyPrefix}{routeId}:{stepCode}";

    public YieldService(IRedisService redis) => _redis = redis;

    public Task<double> CalculateStepYieldAsync(int passQty, int inputQty)
    {
        if (inputQty <= 0) return Task.FromResult(100.0);
        return Task.FromResult((double)passQty / inputQty * 100);
    }

    public async Task<YieldRule?> CheckYieldRuleAsync(string routeId, string stepCode, double actualYield)
    {
        var rule = await _redis.GetObjectAsync<YieldRule>(YieldRuleKey(routeId, stepCode));
        if (rule is null || !rule.IsActive) return null;
        
        return actualYield < rule.YieldThreshold ? rule : null;
    }

    public async Task<bool> ShouldAutoHoldAsync(string routeId, string stepCode, double actualYield)
    {
        var rule = await CheckYieldRuleAsync(routeId, stepCode, actualYield);
        return rule?.ActionType == "AutoHold";
    }

    public async Task<List<YieldRule>> GetYieldRulesAsync(string routeId)
    {
        var results = new List<YieldRule>();
        // 简单实现：遍历常见 Step 的 rule key
        // 实际生产中应使用索引 key
        return results;
    }
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 6：创建 IGenealogyService + 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/IGenealogyService.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/GenealogyService.cs`

- [ ] **Step 1：创建 IGenealogyService.cs**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IGenealogyService
{
    Task RecordRelationAsync(LotGenealogy relation);
    Task<List<LotGenealogy>> GetUpstreamAsync(string lotId);
    Task<List<LotGenealogy>> GetDownstreamAsync(string lotId);
    Task<List<LotGenealogy>> GetFullTreeAsync(string lotId);
}
```

- [ ] **Step 2：创建 GenealogyService.cs**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class GenealogyService : IGenealogyService
{
    private readonly IRedisService _redis;
    private const string GenealogyUpstreamKey = "mes:genealogy:upstream:";
    private const string GenealogyDownstreamKey = "mes:genealogy:downstream:";

    private static string UpstreamKey(string lotId) => $"{GenealogyUpstreamKey}{lotId}";
    private static string DownstreamKey(string lotId) => $"{GenealogyDownstreamKey}{lotId}";

    public GenealogyService(IRedisService redis) => _redis = redis;

    public async Task RecordRelationAsync(LotGenealogy relation)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(relation);
        
        // 记录子批的上游关系
        await _redis.ListRightPushAsync(UpstreamKey(relation.ChildLotId), json);
        
        // 记录父批的下游关系
        await _redis.ListRightPushAsync(DownstreamKey(relation.ParentLotId), json);
    }

    public async Task<List<LotGenealogy>> GetUpstreamAsync(string lotId)
    {
        var records = await _redis.ListRangeAsync(UpstreamKey(lotId));
        return DeserializeRecords(records);
    }

    public async Task<List<LotGenealogy>> GetDownstreamAsync(string lotId)
    {
        var records = await _redis.ListRangeAsync(DownstreamKey(lotId));
        return DeserializeRecords(records);
    }

    public async Task<List<LotGenealogy>> GetFullTreeAsync(string lotId)
    {
        var upstream = await GetUpstreamAsync(lotId);
        var downstream = await GetDownstreamAsync(lotId);
        return upstream.Concat(downstream).ToList();
    }

    private static List<LotGenealogy> DeserializeRecords(List<string> records)
    {
        var results = new List<LotGenealogy>();
        foreach (var json in records)
        {
            var record = System.Text.Json.JsonSerializer.Deserialize<LotGenealogy>(json);
            if (record is not null) results.Add(record);
        }
        return results;
    }
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 2-C：增强 TrackService

### Task 7：增强 TrackService 校验链和处理链

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/Services/TrackService.cs`

- [ ] **Step 1：读取现有 TrackService.cs 全文**

- [ ] **Step 2：替换 ValidateTrackInAsync 方法**

用以下实现替换现有的 `ValidateTrackInAsync` 方法：

```csharp
public async Task<TrackValidationResult> ValidateTrackInAsync(TrackInRequest request)
{
    var result = new TrackValidationResult { IsValid = true };

    // 1. Lot 是否存在
    var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{request.LotId}");
    if (lot is null)
    {
        result.IsValid = false;
        result.Errors.Add($"批次 {request.LotId} 不存在");
        return result;
    }

    // 2. Lot 状态是否允许进站
    if (!lot.CanTrackIn)
    {
        result.IsValid = false;
        if (lot.Status == "Completed") result.Errors.Add("批次已完成，不可进站");
        else if (lot.Status == "Scrapped") result.Errors.Add("批次已报废，不可进站");
        else result.Errors.Add($"批次状态 {lot.Status} 不允许进站");
    }

    // 3. Lot 是否 Hold
    if (lot.Status == "Hold")
    {
        result.IsValid = false;
        result.Errors.Add($"批次处于 Hold 状态 ({lot.HoldReason})，禁止进站");
    }

    // 4. Lot 是否 MRB
    if (lot.IsUnderMRB)
    {
        result.IsValid = false;
        result.Errors.Add("批次处于 MRB 审查中，禁止进站");
    }

    // 5. 当前 Step 是否存在于 Route
    var routeId = lot.ReworkRouteId ?? lot.RouteId;
    var steps = await _routeService.GetStepsAsync(routeId);
    var currentStep = steps.FirstOrDefault(s => s.StepSeq == request.StepSeq);
    if (currentStep is null)
    {
        result.IsValid = false;
        result.Errors.Add($"工序 Step {request.StepSeq} 不存在于路线 {routeId}");
    }
    else
    {
        // 6. 上一站是否已完成
        if (currentStep.StepSeq > 1)
        {
            var prevStep = steps.FirstOrDefault(s => s.StepSeq == currentStep.StepSeq - 1);
            if (prevStep != null)
            {
                var prevRecord = await _redis.GetObjectAsync<LotStepRecord>(
                    $"mes:lot:step:{request.LotId}:{routeId}:1.0:{prevStep.StepSeq}");
                if (prevRecord is null || prevRecord.Status != "Completed")
                {
                    result.IsValid = false;
                    result.Errors.Add($"上一站 {prevStep.StepName} 未完成，不可进站");
                }
            }
        }

        // 7. 设备是否属于 Step 设备组
        if (!string.IsNullOrEmpty(currentStep.EquipmentGroup))
        {
            var allowed = await _routeService.IsEquipmentAllowedAsync(currentStep.StepCode, currentStep.EquipmentGroup);
            // V4 接入真实设备服务，当前简化通过
        }

        // 8. 载具类型是否匹配
        if (!string.IsNullOrEmpty(currentStep.RequiredCarrierType))
        {
            // V3 接入载具服务，当前记录 warning
            result.Warnings.Add($"工序要求载具: {currentStep.RequiredCarrierType}");
        }

        // 9. Recipe 校验（V4）
        if (currentStep.RequireRecipeCheck)
        {
            result.Warnings.Add("Recipe 校验未接入，V4 实现");
        }
    }

    return result;
}
```

- [ ] **Step 3：替换 TrackInAsync 方法**

```csharp
public async Task<TrackResult> TrackInAsync(TrackInRequest request)
{
    // 先校验
    var validation = await ValidateTrackInAsync(request);
    if (!validation.IsValid)
    {
        return new TrackResult { Success = false, Message = string.Join("; ", validation.Errors) };
    }

    var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{request.LotId}")!;
    var routeId = lot.ReworkRouteId ?? lot.RouteId;
    var routeVersion = lot.RouteVersion;

    // 创建 LotStepRecord
    var stepRecord = new LotStepRecord
    {
        RecordId = Guid.NewGuid().ToString("N"),
        LotId = request.LotId,
        RouteId = routeId,
        RouteVersion = routeVersion,
        StepSeq = request.StepSeq,
        StepName = request.StepName,
        StepCode = request.StepCode,
        Status = "Processing",
        TrackInEquipment = request.EquipmentId,
        TrackInCarrier = request.CarrierId,
        TrackInRecipe = request.RecipeId,
        TrackInTime = DateTime.UtcNow,
        TrackInOperator = request.OperatorId,
        InputQty = request.InputQty > 0 ? request.InputQty : lot.UnitCount
    };

    await _redis.SetObjectAsync(
        $"mes:lot:step:{request.LotId}:{routeId}:{routeVersion}:{request.StepSeq}",
        stepRecord);

    // 更新 Lot 状态
    lot.Status = "Processing";
    lot.CurrentEquipment = request.EquipmentId;
    lot.CarrierId = request.CarrierId;
    if (lot.OriginalQty == 0) lot.OriginalQty = lot.UnitCount;
    await _redis.SetObjectAsync($"mes:lot:{request.LotId}", lot);

    // 写操作历史
    await _opHistoryService.WriteAsync(
        request.LotId, lot.OrderId, "TrackIn", request.StepName, request.StepSeq,
        request.EquipmentId, request.RecipeId, request.CarrierId,
        null, null, request.OperatorId, request.OperatorName,
        request.Workstation, request.Remark);

    // 写审计
    await _auditService.WriteAsync(new AuditTrail
    {
        AuditId = Guid.NewGuid().ToString("N"),
        EntityType = "Lot",
        EntityId = request.LotId,
        Action = "TrackIn",
        ActionLevel = "Audit",
        OperatorId = request.OperatorId,
        OperatorName = request.OperatorName,
        Workstation = request.Workstation,
        ReasonText = request.Remark,
        CreatedAt = DateTime.UtcNow
    });

    return new TrackResult
    {
        Success = true,
        LotId = request.LotId,
        StepCode = request.StepCode,
        Message = $"{request.LotId} 进站 {request.StepName} 成功",
        TrackTime = DateTime.UtcNow,
        RecordId = stepRecord.RecordId
    };
}
```

- [ ] **Step 4：替换 ValidateTrackOutAsync 方法**

```csharp
public async Task<TrackValidationResult> ValidateTrackOutAsync(TrackOutRequest request)
{
    var result = new TrackValidationResult { IsValid = true };

    // 1. Lot 是否已 TrackIn
    var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{request.LotId}");
    if (lot is null || lot.Status != "Processing")
    {
        result.IsValid = false;
        result.Errors.Add("批次未进站或不在加工中");
        return result;
    }

    // 2. 数量平衡
    var qtyValidation = await _quantityService.ValidateTrackOutQuantityAsync(request);
    if (!qtyValidation.IsBalanced)
    {
        result.IsValid = false;
        result.Errors.Add($"数量不平衡: {qtyValidation.Message}");
    }

    // 3. 良率检查 → 自动 Hold
    var yield = await _yieldService.CalculateStepYieldAsync(request.PassQty, request.InputQty);
    var routeId = lot.ReworkRouteId ?? lot.RouteId;
    var shouldHold = await _yieldService.ShouldAutoHoldAsync(routeId, request.StepCode, yield);
    if (shouldHold && request.HoldQty <= 0)
    {
        result.Warnings.Add($"良率 {yield:F1}% 低于阈值，建议自动 Hold");
    }

    // 4. Scrap 超阈值 → 需要签核
    if (request.ScrapQty > 0 && request.InputQty > 0)
    {
        var scrapRate = (double)request.ScrapQty / request.InputQty * 100;
        if (scrapRate > 5)
        {
            result.Warnings.Add($"报废率 {scrapRate:F1}% 超过 5%，需要主管签核");
        }
    }

    return result;
}
```

- [ ] **Step 5：替换 TrackOutAsync 方法**

```csharp
public async Task<TrackResult> TrackOutAsync(TrackOutRequest request)
{
    // 先校验
    var validation = await ValidateTrackOutAsync(request);
    if (!validation.IsValid)
    {
        return new TrackResult { Success = false, Message = string.Join("; ", validation.Errors) };
    }

    var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{request.LotId}")!;
    var routeId = lot.ReworkRouteId ?? lot.RouteId;
    var routeVersion = lot.RouteVersion;
    var steps = await _routeService.GetStepsAsync(routeId);
    var currentStep = steps.FirstOrDefault(s => s.StepSeq == request.StepSeq);

    // 完成当前 Step 记录
    var stepKey = $"mes:lot:step:{request.LotId}:{routeId}:{routeVersion}:{request.StepSeq}";
    var stepRecord = await _redis.GetObjectAsync<LotStepRecord>(stepKey);
    if (stepRecord is null)
    {
        return new TrackResult { Success = false, Message = "未找到进站记录" };
    }

    stepRecord.Status = "Completed";
    stepRecord.TrackOutTime = DateTime.UtcNow;
    stepRecord.TrackOutOperator = request.OperatorId;
    stepRecord.PassQty = request.PassQty;
    stepRecord.FailQty = request.FailQty;
    stepRecord.ScrapQty = request.ScrapQty;
    stepRecord.ReworkQty = request.ReworkQty;
    stepRecord.HoldQty = request.HoldQty;
    stepRecord.PendingQty = request.PendingQty;
    stepRecord.RecipeId = request.RecipeId;
    stepRecord.TestProgram = request.TestProgram;
    stepRecord.BinSummary = request.BinSummary;
    stepRecord.Remark = request.Remark;

    await _redis.SetObjectAsync(stepKey, stepRecord);

    // 数量事务
    var qtyTx = new QuantityTransaction
    {
        TransactionId = Guid.NewGuid().ToString("N"),
        LotId = request.LotId,
        OperationId = Guid.NewGuid().ToString("N"),
        TransactionType = "TrackOut",
        StepName = request.StepName,
        BeforeQty = request.InputQty,
        InputQty = request.InputQty,
        PassQty = request.PassQty,
        FailQty = request.FailQty,
        ScrapQty = request.ScrapQty,
        ReworkQty = request.ReworkQty,
        HoldQty = request.HoldQty,
        PendingQty = request.PendingQty,
        AfterQty = request.PassQty,
        OperatorId = request.OperatorId,
        CreatedAt = DateTime.UtcNow
    };
    await _quantityService.RecordTransactionAsync(qtyTx);

    // 更新 Lot 累计数量
    lot.TotalPassQty += request.PassQty;
    lot.TotalScrapQty += request.ScrapQty;
    lot.TotalReworkQty += request.ReworkQty;
    lot.TotalHoldQty += request.HoldQty;
    lot.UnitCount = request.PassQty; // 当前可用数量 = PassQty

    // 计算下一站
    var nextStep = await _routeService.GetNextStepAsync(request.LotId, routeId, routeVersion, request.StepSeq);
    string? nextStepName = null;

    if (nextStep is not null)
    {
        lot.CurrentStepSeq = nextStep.StepSeq;
        lot.CurrentStep = nextStep.StepName;
        lot.Status = "Waiting";
        nextStepName = nextStep.StepName;
    }
    else
    {
        lot.Status = "Completed";
        lot.CurrentStepSeq = request.StepSeq;
        lot.CurrentStep = request.StepName + " (完成)";
    }

    await _redis.SetObjectAsync($"mes:lot:{request.LotId}", lot);

    // 良率检查 → 自动 Hold
    var yield = await _yieldService.CalculateStepYieldAsync(request.PassQty, request.InputQty);
    var shouldHold = await _yieldService.ShouldAutoHoldAsync(routeId, request.StepCode, yield);
    if (shouldHold)
    {
        lot.Status = "Hold";
        lot.HoldCategory = HoldType.YieldHold;
        lot.HoldReason = $"良率 {yield:F1}% 低于阈值，自动 Hold";
        lot.HoldTime = DateTime.Now;
        await _redis.SetObjectAsync($"mes:lot:{request.LotId}", lot);
    }

    // 写操作历史
    await _opHistoryService.WriteAsync(
        request.LotId, lot.OrderId, "TrackOut", request.StepName, request.StepSeq,
        request.EquipmentId, request.RecipeId, null,
        request.InputQty, request.PassQty, request.OperatorId, request.OperatorName,
        request.Workstation, request.Remark);

    // 写审计
    await _auditService.WriteAsync(new AuditTrail
    {
        AuditId = Guid.NewGuid().ToString("N"),
        EntityType = "Lot",
        EntityId = request.LotId,
        Action = "TrackOut",
        ActionLevel = "Audit",
        OperatorId = request.OperatorId,
        OperatorName = request.OperatorName,
        Workstation = request.Workstation,
        ReasonText = request.Remark,
        CreatedAt = DateTime.UtcNow
    });

    // 谱系记录
    if (nextStep is null)
    {
        await _genealogyService.RecordRelationAsync(new LotGenealogy
        {
            ParentLotId = request.LotId,
            ChildLotId = request.LotId,
            RelationType = "Completed",
            StepCode = request.StepCode,
            StepSeq = request.StepSeq,
            Qty = request.PassQty,
            OperatorId = request.OperatorId
        });
    }

    return new TrackResult
    {
        Success = true,
        LotId = request.LotId,
        StepCode = request.StepCode,
        Message = $"{request.LotId} 出站 {request.StepName} 成功，良率 {yield:F1}%",
        NextStepName = nextStepName,
        TrackTime = DateTime.UtcNow
    };
}
```

- [ ] **Step 6：添加新依赖注入**

在 TrackService 构造函数中添加 `_yieldService` 和 `_genealogyService`：

```csharp
    private readonly IYieldService _yieldService;
    private readonly IGenealogyService _genealogyService;

    public TrackService(
        IRedisService redis,
        IRouteService routeService,
        IQuantityService quantityService,
        IOperationHistoryService opHistoryService,
        IAuditService auditService,
        IYieldService yieldService,
        IGenealogyService genealogyService)
    {
        _redis = redis;
        _routeService = routeService;
        _quantityService = quantityService;
        _opHistoryService = opHistoryService;
        _auditService = auditService;
        _yieldService = yieldService;
        _genealogyService = genealogyService;
    }
```

- [ ] **Step 7：ForceTrackInAsync 和 ForceTrackOutAsync 也写审计和谱系**

在现有 `ForceTrackInAsync` 和 `ForceTrackOutAsync` 返回成功后，各加一行写审计：

```csharp
// ForceTrackInAsync 末尾添加:
    await _auditService.WriteAsync(new AuditTrail
    {
        AuditId = Guid.NewGuid().ToString("N"),
        EntityType = "Lot", EntityId = request.LotId,
        Action = "ForceTrackIn", ActionLevel = "Signature",
        OperatorId = request.OperatorId, OperatorName = request.OperatorName,
        Workstation = request.Workstation,
        ReasonText = reason,
        CreatedAt = DateTime.UtcNow
    });

// ForceTrackOutAsync 末尾添加:
    await _auditService.WriteAsync(new AuditTrail
    {
        AuditId = Guid.NewGuid().ToString("N"),
        EntityType = "Lot", EntityId = request.LotId,
        Action = "ForceTrackOut", ActionLevel = "Signature",
        OperatorId = request.OperatorId, OperatorName = request.OperatorName,
        Workstation = request.Workstation,
        ReasonText = reason,
        CreatedAt = DateTime.UtcNow
    });
```

- [ ] **Step 8：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 2-D：注册新服务 + 重写 Seed

### Task 8：注册新服务到 DI

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/ProductionModule.cs`

- [ ] **Step 1：在 RegisterTypes 方法中添加新服务注册**

在现有服务注册之后、Module 初始化之前，添加：

```csharp
// V2 新增服务
containerRegistry.RegisterSingleton<ISignatureService, SignatureService>();
containerRegistry.RegisterSingleton<IYieldService, YieldService>();
containerRegistry.RegisterSingleton<IGenealogyService, GenealogyService>();
```

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 9：重写 ProductionDataService Seed 数据

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/Services/ProductionDataService.cs`

这是整个 Phase 2 最大的变更——重写 `EnsureSeededAsync` 方法，创建完整的 QFN-STD-V2.0 路线和种子数据。

- [ ] **Step 1：替换整个 EnsureSeededAsync 方法**

删除现有 `EnsureSeededAsync` 方法全部内容（从 `public async Task EnsureSeededAsync()` 到方法结束），替换为：

```csharp
    public async Task EnsureSeededAsync()
    {
        if (await _redis.KeyExistsAsync(SeededKey)) return;

        // ===== 1. QFN-STD-V2.0 Route (12步) =====
        var qfnRoute = new RouteInfo
        {
            RouteId = "QFN-STD",
            RouteName = "QFN 标准封装流程",
            RouteVersion = "2.0",
            ProductId = "QFN-24",
            PackageType = PackageType.QFN,
            IsActive = true,
            IsApproved = true,
            ApprovedBy = "Admin",
            ApprovedAt = DateTime.Now,
            Steps = new List<RouteStep>
            {
                new() { RouteId = "QFN-STD", StepSeq = 1, StepName = "Saw", StepCode = "SAW", EquipmentGroup = "SAW", RequiredCarrierType = "TapeFrame", YieldThreshold = 98, QueueTimeLimitMinutes = 480 },
                new() { RouteId = "QFN-STD", StepSeq = 2, StepName = "DieAttach", StepCode = "DA", EquipmentGroup = "DA", RequiredCarrierType = "Magazine", YieldThreshold = 97, RequiredSignatureLevel = "Level1", ReworkRouteId = "RW-DA", QueueTimeLimitMinutes = 240 },
                new() { RouteId = "QFN-STD", StepSeq = 3, StepName = "Cure", StepCode = "CURE", EquipmentGroup = "OVEN", RequiredCarrierType = "Magazine", QueueTimeLimitMinutes = 120 },
                new() { RouteId = "QFN-STD", StepSeq = 4, StepName = "WireBond", StepCode = "WB", EquipmentGroup = "WB", RequiredCarrierType = "Magazine", YieldThreshold = 96, RequiredSignatureLevel = "Level1", ReworkRouteId = "RW-WB", QueueTimeLimitMinutes = 480 },
                new() { RouteId = "QFN-STD", StepSeq = 5, StepName = "Mold", StepCode = "MOLD", EquipmentGroup = "MP", RequiredCarrierType = "MoldPlate", YieldThreshold = 99, QueueTimeLimitMinutes = 240 },
                new() { RouteId = "QFN-STD", StepSeq = 6, StepName = "PMC", StepCode = "PMC", EquipmentGroup = "OVEN", RequiredCarrierType = "OvenCart", QueueTimeLimitMinutes = 480 },
                new() { RouteId = "QFN-STD", StepSeq = 7, StepName = "Mark", StepCode = "MARK", EquipmentGroup = "MARK", RequiredCarrierType = "Magazine", QueueTimeLimitMinutes = 240 },
                new() { RouteId = "QFN-STD", StepSeq = 8, StepName = "Singulation", StepCode = "SING", EquipmentGroup = "SING", RequiredCarrierType = "SingTray", YieldThreshold = 98, RequiredSignatureLevel = "Level1", EnableMRB = true, MRBThreshold = 10, QueueTimeLimitMinutes = 240 },
                new() { RouteId = "QFN-STD", StepSeq = 9, StepName = "FinalTest", StepCode = "FT", EquipmentGroup = "TEST", RequiredCarrierType = "TestTray", QueueTimeLimitMinutes = 480 },
                new() { RouteId = "QFN-STD", StepSeq = 10, StepName = "OQC", StepCode = "OQC", EquipmentGroup = "OQC", RequiredCarrierType = "Tray", YieldThreshold = 99.5, RequiredSignatureLevel = "Level2", QueueTimeLimitMinutes = 240 },
                new() { RouteId = "QFN-STD", StepSeq = 11, StepName = "Packing", StepCode = "PACK", EquipmentGroup = "PACK", RequiredCarrierType = "Reel", QueueTimeLimitMinutes = 480 },
            }
        };
        await _redis.SetObjectAsync($"mes:route:QFN-STD:2.0", qfnRoute);
        await _redis.SetAddAsync("mes:route:index", "QFN-STD:2.0");

        // ===== 2. Rework Routes =====
        var rwDA = new RouteInfo
        {
            RouteId = "RW-DA", RouteName = "DieAttach 重工路线", RouteVersion = "1.0",
            ProductId = "QFN-24", PackageType = PackageType.QFN,
            IsActive = true, IsApproved = true, ApprovedBy = "Admin", ApprovedAt = DateTime.Now,
            Steps = new List<RouteStep>
            {
                new() { RouteId = "RW-DA", StepSeq = 1, StepName = "Debond", StepCode = "DEBOND", EquipmentGroup = "DA", RequiredCarrierType = "Magazine", QueueTimeLimitMinutes = 120 },
                new() { RouteId = "RW-DA", StepSeq = 2, StepName = "Clean", StepCode = "CLEAN", EquipmentGroup = "CLEAN", RequiredCarrierType = "Magazine", QueueTimeLimitMinutes = 60 },
                new() { RouteId = "RW-DA", StepSeq = 3, StepName = "Re-DieAttach", StepCode = "DA", EquipmentGroup = "DA", RequiredCarrierType = "Magazine", YieldThreshold = 97, QueueTimeLimitMinutes = 240 },
                new() { RouteId = "RW-DA", StepSeq = 4, StepName = "Re-Cure", StepCode = "CURE", EquipmentGroup = "OVEN", RequiredCarrierType = "Magazine", QueueTimeLimitMinutes = 120 },
            }
        };
        await _redis.SetObjectAsync($"mes:route:RW-DA:1.0", rwDA);
        await _redis.SetAddAsync("mes:route:index", "RW-DA:1.0");

        var rwWB = new RouteInfo
        {
            RouteId = "RW-WB", RouteName = "WireBond 重工路线", RouteVersion = "1.0",
            ProductId = "QFN-24", PackageType = PackageType.QFN,
            IsActive = true, IsApproved = true, ApprovedBy = "Admin", ApprovedAt = DateTime.Now,
            Steps = new List<RouteStep>
            {
                new() { RouteId = "RW-WB", StepSeq = 1, StepName = "Debond-WB", StepCode = "DEBOND-WB", EquipmentGroup = "WB", RequiredCarrierType = "Magazine", QueueTimeLimitMinutes = 120 },
                new() { RouteId = "RW-WB", StepSeq = 2, StepName = "Clean-WB", StepCode = "CLEAN-WB", EquipmentGroup = "CLEAN", RequiredCarrierType = "Magazine", QueueTimeLimitMinutes = 60 },
                new() { RouteId = "RW-WB", StepSeq = 3, StepName = "Re-WireBond", StepCode = "WB", EquipmentGroup = "WB", RequiredCarrierType = "Magazine", YieldThreshold = 96, QueueTimeLimitMinutes = 480 },
            }
        };
        await _redis.SetObjectAsync($"mes:route:RW-WB:1.0", rwWB);
        await _redis.SetAddAsync("mes:route:index", "RW-WB:1.0");

        // ===== 3. YieldRules =====
        var yieldRules = new[]
        {
            new YieldRule { RuleId = "YR-SAW", RouteId = "QFN-STD", StepCode = "SAW", YieldThreshold = 98, ActionType = "AutoHold", NotifyRole = "QA" },
            new YieldRule { RuleId = "YR-DA", RouteId = "QFN-STD", StepCode = "DA", YieldThreshold = 97, ActionType = "AutoHold", NotifyRole = "QA" },
            new YieldRule { RuleId = "YR-WB", RouteId = "QFN-STD", StepCode = "WB", YieldThreshold = 96, ActionType = "AutoHold", NotifyRole = "QA" },
            new YieldRule { RuleId = "YR-MOLD", RouteId = "QFN-STD", StepCode = "MOLD", YieldThreshold = 99, ActionType = "AutoHold", NotifyRole = "QA" },
            new YieldRule { RuleId = "YR-SING", RouteId = "QFN-STD", StepCode = "SING", YieldThreshold = 98, ActionType = "AutoHold", NotifyRole = "QA" },
            new YieldRule { RuleId = "YR-OQC", RouteId = "QFN-STD", StepCode = "OQC", YieldThreshold = 99.5, ActionType = "AutoHold", NotifyRole = "QA" },
        };
        foreach (var rule in yieldRules)
        {
            await _redis.SetObjectAsync($"mes:yieldrule:{rule.RouteId}:{rule.StepCode}", rule);
        }

        // ===== 4. WorkOrder =====
        var wo = new WorkOrderInfo
        {
            OrderId = "WO-2026001",
            ProductId = "QFN-24",
            ProductName = "QFN-24 封装",
            DieName = "IC-QFN-24",
            PackageType = PackageType.QFN,
            PlannedQty = 10000,
            CompletedQty = 0,
            UnitQty = 10000,
            Status = ProcessStatus.InProgress,
            Priority = WorkOrderPriority.High,
            RouteId = "QFN-STD",
            RouteName = "QFN 标准封装流程 V2.0",
            CustomerId = "CUS-AUTO-001",
            CustomerName = "车规客户",
            CustomerPN = "AUTO-QFN24-001",
            InternalPN = "INT-QFN24-001",
            TestProgram = "TP-QFN-FT-V1",
            BinSpec = "Bin1:Pass(车规) Bin2:Grade2(工业) Bin3:Fail",
            WaferSource = "TSMC",
            Area = "PKG1",
            Line = "LINE-1",
            Creator = "Admin",
            PlannedStartDate = DateTime.Now.AddDays(-5),
            PlannedEndDate = DateTime.Now.AddDays(10),
            TargetCPYield = 99.0,
            TargetFTYield = 98.0,
            YieldTarget = 96.0,
            CreatedAt = DateTime.Now.AddDays(-5)
        };
        await SaveWorkOrderAsync(wo);

        // ===== 5. 种子批次 =====
        
        // 5a. LOT-001 - 新批次，等待在 Saw
        var lot001 = new LotInfo
        {
            LotId = "LOT-001",
            OrderId = "WO-2026001",
            ProductId = "QFN-24",
            ProductName = "QFN-24 封装",
            DieName = "IC-QFN-24",
            PackageType = PackageType.QFN,
            RouteId = "QFN-STD",
            RouteVersion = "2.0",
            CurrentStep = "Saw",
            CurrentStepSeq = 1,
            Status = "Released",
            OriginalQty = 10000,
            UnitCount = 10000,
            StripCount = 100,
            Priority = "High",
            CarrierType = CarrierType.WaferFrame,
            CarrierId = "FOUP-007",
            WaferLotId = "WFR-20260520-001",
            CreatedAt = DateTime.Now.AddHours(-1)
        };
        await _redis.SetObjectAsync(LotKey(lot001.LotId), lot001);
        await _redis.SetAddAsync(LotHoldIndexKey, lot001.LotId); // 加入 lot index

        // 5b. LOT-COMPLETE-001 - 已完成全部 12 步
        var lotComplete = new LotInfo
        {
            LotId = "LOT-COMPLETE-001",
            OrderId = "WO-2026001",
            ProductId = "QFN-24",
            ProductName = "QFN-24 封装",
            DieName = "IC-QFN-24",
            PackageType = PackageType.QFN,
            RouteId = "QFN-STD",
            RouteVersion = "2.0",
            CurrentStep = "Packing (完成)",
            CurrentStepSeq = 11,
            Status = "Completed",
            OriginalQty = 10000,
            UnitCount = 9550,
            TotalPassQty = 9550,
            TotalScrapQty = 450,
            StripCount = 100,
            Priority = "Normal",
            CarrierType = CarrierType.Reel,
            CarrierId = "REEL-001",
            WaferLotId = "WFR-20260520-001",
            CreatedAt = DateTime.Now.AddDays(-3)
        };
        await _redis.SetObjectAsync(LotKey(lotComplete.LotId), lotComplete);

        // 为 LOT-COMPLETE-001 写完整 Step 记录
        foreach (var step in qfnRoute.Steps)
        {
            var record = new LotStepRecord
            {
                RecordId = Guid.NewGuid().ToString("N"),
                LotId = "LOT-COMPLETE-001",
                RouteId = "QFN-STD",
                RouteVersion = "2.0",
                StepSeq = step.StepSeq,
                StepName = step.StepName,
                StepCode = step.StepCode,
                Status = "Completed",
                TrackInTime = DateTime.Now.AddDays(-3).AddHours(step.StepSeq * 2),
                TrackOutTime = DateTime.Now.AddDays(-3).AddHours(step.StepSeq * 2 + 1),
                TrackInOperator = "OP-001",
                TrackOutOperator = "OP-001",
                InputQty = 10000 - (step.StepSeq * 30),
                PassQty = 9970 - (step.StepSeq * 30),
                FailQty = 10,
                ScrapQty = 20,
                Remark = $"Step {step.StepSeq} {step.StepName} 完成"
            };
            await _redis.SetObjectAsync(
                $"mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:{step.StepSeq}", record);
        }

        // 5c. LOT-HOLD-001 - WireBond 低良率 Hold
        var lotHold = new LotInfo
        {
            LotId = "LOT-HOLD-001",
            OrderId = "WO-2026001",
            ProductId = "QFN-24",
            ProductName = "QFN-24 封装",
            DieName = "IC-QFN-24",
            PackageType = PackageType.QFN,
            RouteId = "QFN-STD",
            RouteVersion = "2.0",
            CurrentStep = "WireBond",
            CurrentStepSeq = 4,
            Status = "Hold",
            OriginalQty = 9900,
            UnitCount = 8200,
            TotalPassQty = 8200,
            TotalScrapQty = 1200,
            TotalHoldQty = 500,
            StripCount = 100,
            Priority = "High",
            CarrierType = CarrierType.Magazine,
            CarrierId = "MAG-022",
            WaferLotId = "WFR-20260520-001",
            HoldCategory = HoldType.YieldHold,
            HoldReason = "WireBond 良率 82.8% < 阈值 96%，自动 Hold",
            HoldTime = DateTime.Now.AddHours(-2),
            HoldOperator = "SYSTEM",
            ReleaseCondition = "工程分析原因并签核后释放",
            CreatedAt = DateTime.Now.AddDays(-2)
        };
        await _redis.SetObjectAsync(LotKey(lotHold.LotId), lotHold);
        await _redis.SetAddAsync(LotHoldIndexKey, lotHold.LotId);

        // 写 Hold 的 Step 记录
        var holdSteps = new[] { "Saw", "DieAttach", "Cure", "WireBond" };
        var holdStepSeqs = new[] { 1, 2, 3, 4 };
        for (int i = 0; i < holdSteps.Length; i++)
        {
            var isLast = i == holdSteps.Length - 1;
            var rec = new LotStepRecord
            {
                RecordId = Guid.NewGuid().ToString("N"),
                LotId = "LOT-HOLD-001",
                RouteId = "QFN-STD",
                RouteVersion = "2.0",
                StepSeq = holdStepSeqs[i],
                StepName = holdSteps[i],
                StepCode = holdSteps[i].ToUpper().Replace("WIREBOND", "WB"),
                Status = isLast ? "Processing" : "Completed",
                TrackInTime = DateTime.Now.AddDays(-2).AddHours(i * 3),
                TrackOutTime = isLast ? null : DateTime.Now.AddDays(-2).AddHours(i * 3 + 2),
                TrackInOperator = "OP-002",
                TrackOutOperator = isLast ? null : "OP-002",
                InputQty = i == 0 ? 9900 : 9900 - i * 100,
                PassQty = i == 3 ? 8200 : 9900 - i * 100,
                ScrapQty = i == 3 ? 1200 : 50,
                Remark = isLast ? "WireBond 加工中触发低良率" : $"Step {holdStepSeqs[i]} 完成"
            };
            await _redis.SetObjectAsync(
                $"mes:lot:step:LOT-HOLD-001:QFN-STD:2.0:{holdStepSeqs[i]}", rec);
        }

        // 5d. LOT-REWORK-001 - WireBond 重工中
        var lotRework = new LotInfo
        {
            LotId = "LOT-REWORK-001",
            OrderId = "WO-2026001",
            ProductId = "QFN-24",
            ProductName = "QFN-24 封装",
            DieName = "IC-QFN-24",
            PackageType = PackageType.QFN,
            RouteId = "QFN-STD",
            RouteVersion = "2.0",
            CurrentStep = "Re-WireBond",
            CurrentStepSeq = 3,
            Status = "Processing",
            IsReworkLot = true,
            OriginalRouteId = "QFN-STD",
            ReworkRouteId = "RW-WB",
            ReworkCount = 1,
            ReworkReason = "WireBond 断线重工",
            OriginalQty = 6000,
            UnitCount = 5700,
            TotalPassQty = 5700,
            TotalScrapQty = 300,
            StripCount = 60,
            Priority = "High",
            CarrierType = CarrierType.Magazine,
            CarrierId = "MAG-045",
            WaferLotId = "WFR-20260520-001",
            MotherLotId = "LOT-HOLD-001",
            IsPartialLot = true,
            CreatedAt = DateTime.Now.AddDays(-1)
        };
        await _redis.SetObjectAsync(LotKey(lotRework.LotId), lotRework);

        // 5e. LOT-MRB-001 - Singulation MRB
        var lotMRB = new LotInfo
        {
            LotId = "LOT-MRB-001",
            OrderId = "WO-2026001",
            ProductId = "QFN-24",
            ProductName = "QFN-24 封装",
            DieName = "IC-QFN-24",
            PackageType = PackageType.QFN,
            RouteId = "QFN-STD",
            RouteVersion = "2.0",
            CurrentStep = "Singulation",
            CurrentStepSeq = 8,
            Status = "Hold",
            IsUnderMRB = true,
            MRBReference = "MRB-20260523-001",
            MRBDisposition = "",
            OriginalQty = 9550,
            UnitCount = 9350,
            TotalPassQty = 9350,
            TotalScrapQty = 200,
            TotalHoldQty = 200,
            StripCount = 95,
            Priority = "Normal",
            CarrierType = CarrierType.Magazine,
            CarrierId = "MAG-078",
            WaferLotId = "WFR-20260520-001",
            HoldCategory = HoldType.Quality,
            HoldReason = "Singulation 外观不良 200 pcs，MRB 判定中",
            HoldTime = DateTime.Now.AddHours(-4),
            HoldOperator = "QA-001",
            ReleaseCondition = "MRB 结论: Repair 180 pcs, Scrap 20 pcs",
            CreatedAt = DateTime.Now.AddDays(-2)
        };
        await _redis.SetObjectAsync(LotKey(lotMRB.LotId), lotMRB);
        await _redis.SetAddAsync(LotHoldIndexKey, lotMRB.LotId);

        // 5f. LOT-GRADE-001 - 已完成 + Grade Split
        var lotGradeA = new LotInfo
        {
            LotId = "LOT-GRADE-A-001",
            OrderId = "WO-2026001",
            ProductId = "QFN-24",
            ProductName = "QFN-24 封装",
            DieName = "IC-QFN-24",
            PackageType = PackageType.QFN,
            RouteId = "QFN-STD",
            RouteVersion = "2.0",
            CurrentStep = "Packing (完成)",
            CurrentStepSeq = 11,
            Status = "Completed",
            Grade = "A",
            OriginalLotId = "LOT-GRADE-001",
            OriginalQty = 8500,
            UnitCount = 8500,
            TotalPassQty = 8500,
            StripCount = 85,
            Priority = "High",
            CarrierType = CarrierType.Reel,
            CarrierId = "REEL-002",
            WaferLotId = "WFR-20260520-001",
            CreatedAt = DateTime.Now.AddDays(-3)
        };
        await _redis.SetObjectAsync(LotKey(lotGradeA.LotId), lotGradeA);

        var lotGradeB = new LotInfo
        {
            LotId = "LOT-GRADE-B-001",
            OrderId = "WO-2026001",
            ProductId = "QFN-24",
            ProductName = "QFN-24 封装",
            DieName = "IC-QFN-24",
            PackageType = PackageType.QFN,
            RouteId = "QFN-STD",
            RouteVersion = "2.0",
            CurrentStep = "Packing (完成)",
            CurrentStepSeq = 11,
            Status = "Completed",
            Grade = "B",
            OriginalLotId = "LOT-GRADE-001",
            OriginalQty = 1030,
            UnitCount = 1030,
            TotalPassQty = 1030,
            StripCount = 10,
            Priority = "Normal",
            CarrierType = CarrierType.Tray,
            CarrierId = "TRAY-003",
            WaferLotId = "WFR-20260520-001",
            CreatedAt = DateTime.Now.AddDays(-3)
        };
        await _redis.SetObjectAsync(LotKey(lotGradeB.LotId), lotGradeB);

        // ===== 6. 清除 seed flag 以便重新生成（调试用） =====
        // await _redis.StringSetAsync(SeededKey, "1");
    }
```

注意：最后一行 `await _redis.StringSetAsync(SeededKey, "1");` 被注释掉，这样每次启动都会重新生成 Seed 数据，方便调试。正式上线后去掉注释。

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 2-E：全量验证

### Task 10：全量编译验证

- [ ] **Step 1：编译整个解决方案**

Run: `dotnet build MES_NEW.sln`
Expected: 25 projects succeed, 0 errors, 0 warnings

- [ ] **Step 2：检查所有新建文件是否都被正确引用**

检查以下文件存在：
```
Models/LotSplitRecord.cs
Models/LotMergeRecord.cs
Models/LotCarrierBinding.cs
Models/SignatureRecord.cs
Models/YieldRule.cs
Models/ScrapRecord.cs
Models/ReworkRecord.cs
Models/LotGenealogy.cs
Services/ISignatureService.cs
Services/IYieldService.cs
Services/IGenealogyService.cs
Services/SignatureService.cs
Services/YieldService.cs
Services/GenealogyService.cs
```

- [ ] **Step 3：验证 ProductionModule.cs 服务注册完整**

确认以下服务都已注册：
```
IRouteService → RouteService
ITrackService → TrackService
IQuantityService → QuantityService
IOperationHistoryService → OperationHistoryService
IAuditService → AuditService
ISignatureService → SignatureService
IYieldService → YieldService
IGenealogyService → GenealogyService
```

---

## 文件变更清单

| 类型 | 数量 | 文件 |
|------|------|------|
| 新建模型 | 8 | LotSplitRecord, LotMergeRecord, LotCarrierBinding, SignatureRecord, YieldRule, ScrapRecord, ReworkRecord, LotGenealogy |
| 新建接口 | 3 | ISignatureService, IYieldService, IGenealogyService |
| 新建实现 | 3 | SignatureService, YieldService, GenealogyService |
| 修改模型 | 3 | LotInfo (+14字段), RouteStep (+8字段), HoldRecord (+2字段), TrackRequest (+字段) |
| 修改服务 | 2 | TrackService (校验链+处理链增强), ProductionDataService (重写Seed) |
| 修改注册 | 1 | ProductionModule.cs (+3服务) |

## 验收标准

```
✅ Lot 能按 QFN-STD-V2.0 (12步) 完整流转
✅ TrackIn 12 项校验全部生效（存在/状态/Hold/MRB/Step/上一站/设备/载具/Recipe/品质/物料/QueueTime）
✅ TrackOut 11 项处理全部生效（平衡/良率/Scrap签核/Step记录/QtyTx/Audit/下一站/载具/Lot更新/Genealogy/事件）
✅ Hold Lot 禁止进站
✅ MRB Lot 禁止进站
✅ 低良率自动 Hold（YieldRule 驱动）
✅ Scrap 超阈值 warning
✅ ForceTrackIn/Out 写审计 Level = "Signature"
✅ 3 条 Rework Route 可用（RW-DA, RW-WB, 可扩展）
✅ 6 个种子批次展示不同状态（Waiting/Completed/Hold/Rework/MRB/Grade）
✅ Seed 数据包含 YieldRules
✅ 谱系记录 Write 链路
✅ 编译 0 错误 0 警告
```

## 风险与缓解

| 风险 | 影响 | 缓解 |
|------|------|------|
| TrackService 改动量大 | 可能引入回归 bug | 分步替换方法，每步编译验证 |
| Seed 数据量大 | 首次启动慢 | 约 30ms 完成（MySQL 本地），可接受 |
| 新增服务无独立测试 | 集成问题难排查 | Task 10 全量编译覆盖基本语法错误 |
