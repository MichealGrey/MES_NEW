# MES V3 Phase 1 模块联动补全 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现 Equipment/Recipe/Quality/Material/Alarm 五大联动，使 TrackIn 前能自动校验设备状态、Recipe 审批、质量 Gate、物料齐套，异常时自动报警。

**Architecture:** 创建 4 个 Gateway 接口 + AlarmService，增强 TrackService 校验链，在 ValidateTrackInAsync 中增加联动校验步骤。所有 Gateway 提供默认实现（返回 true），预留外部系统对接点。

**Tech Stack:** .NET 8, WPF+Prism, MySQL (Pomelo EF Core), IRedisService 抽象层

---

## 文件结构总览

### 新建模型（5 个）
| # | 文件 | 职责 |
|---|------|------|
| 1 | `Models/AlarmRecord.cs` | 报警记录 |
| 2 | `Models/AlarmRule.cs` | 报警规则配置 |
| 3 | `Models/QualityGate.cs` | 质量放行 Gate |
| 4 | `Models/MaterialConsume.cs` | 物料消耗记录 |
| 5 | `Models/EquipmentCheckResult.cs` | 设备校验结果 DTO |

### 新建服务（6 个）
| # | 文件 | 职责 |
|---|------|------|
| 1 | `Services/IEquipmentGateway.cs` | 设备联动接口 |
| 2 | `Services/IRecipeGateway.cs` | Recipe 联动接口 |
| 3 | `Services/IQualityGateway.cs` | 质量 Gate 接口 |
| 4 | `Services/IWarehouseGateway.cs` | 仓储/物料联动接口 |
| 5 | `Services/IAlarmService.cs` | 报警服务接口 |
| 6 | `Services/AlarmService.cs` | 报警服务实现 |

### 新建 Gateway 实现（4 个）
| # | 文件 | 职责 |
|---|------|------|
| 1 | `Services/EquipmentGateway.cs` | 设备联动实现（基于 MasterDataService） |
| 2 | `Services/RecipeGateway.cs` | Recipe 联动实现（基于 MasterDataService） |
| 3 | `Services/QualityGateway.cs` | 质量 Gate 实现（基于 Redis 存储） |
| 4 | `Services/WarehouseGateway.cs` | 物料联动实现（基于 Redis 存储） |

### 修改文件（3 个）
| # | 文件 | 修改内容 |
|---|------|---------|
| 1 | `Services/TrackService.cs` | ValidateTrackInAsync 增加 4 个联动校验 |
| 2 | `ProductionModule.cs` | 注册 6 个新服务 |
| 3 | `ProductionDataService.cs` | 增加 AlarmRule + QualityGate Seed 数据 |

---

### Task 1：创建报警模型

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Models/AlarmRecord.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/AlarmRule.cs`

- [ ] **Step 1：创建 AlarmRecord.cs**

```csharp
namespace MES.Modules.Production.Models;

public class AlarmRecord
{
    public string AlarmId { get; set; } = Guid.NewGuid().ToString("N");
    public string AlarmType { get; set; } = string.Empty; // LowYield/QueueTimeout/HoldTimeout/EquipmentDown/RecipeError/MaterialShort/ForceOperation/QtyImbalance
    public string Severity { get; set; } = "Warning"; // Info/Warning/Error/Critical
    public string Message { get; set; } = string.Empty;
    public string? LotId { get; set; }
    public string? EquipmentId { get; set; }
    public string? StepCode { get; set; }
    public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
    public bool IsAcknowledged { get; set; }
    public string? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? Detail { get; set; }
}
```

- [ ] **Step 2：创建 AlarmRule.cs**

```csharp
namespace MES.Modules.Production.Models;

public class AlarmRule
{
    public string RuleId { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string Severity { get; set; } = "Warning";
    public int? ThresholdMinutes { get; set; } // 超时阈值（分钟）
    public double? ThresholdYield { get; set; } // 良率阈值（%）
    public int? ThresholdQty { get; set; } // 数量阈值
    public string NotifyRole { get; set; } = "Supervisor"; // 通知角色
    public string? Condition { get; set; } // 附加条件 JSON
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors

---

### Task 2：创建质量 Gate 和物料模型

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Models/QualityGate.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/MaterialConsume.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/EquipmentCheckResult.cs`

- [ ] **Step 1：创建 QualityGate.cs**

```csharp
namespace MES.Modules.Production.Models;

public class QualityGate
{
    public string GateId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string GateType { get; set; } = "QACheck"; // QACheck/EngineeringRelease/CustomerRelease
    public string Status { get; set; } = "Pending"; // Pending/Passed/Failed/Expired
    public string? CheckedBy { get; set; }
    public string? CheckedByName { get; set; }
    public DateTime? CheckedAt { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpireAt { get; set; }
}
```

- [ ] **Step 2：创建 MaterialConsume.cs**

```csharp
namespace MES.Modules.Production.Models;

public class MaterialConsume
{
    public string ConsumeId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialName { get; set; } = string.Empty;
    public double ConsumedQty { get; set; }
    public string Unit { get; set; } = "pcs";
    public string? BatchNo { get; set; }
    public string OperatorId { get; set; } = string.Empty;
    public DateTime ConsumedAt { get; set; } = DateTime.UtcNow;
}
```

- [ ] **Step 3：创建 EquipmentCheckResult.cs**

```csharp
namespace MES.Modules.Production.Models;

public class EquipmentCheckResult
{
    public bool IsAllowed { get; set; }
    public string? Reason { get; set; }
    public string EquipmentStatus { get; set; } = string.Empty; // Available/Running/Maintenance/Offline/PM
    public bool IsRecipeMatch { get; set; }
    public bool IsInServiceWindow { get; set; }
}
```

- [ ] **Step 4：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors

---

### Task 3：创建 Gateway 接口

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/IEquipmentGateway.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/IRecipeGateway.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/IQualityGateway.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/IWarehouseGateway.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/IAlarmService.cs`

- [ ] **Step 1：创建 IEquipmentGateway.cs**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IEquipmentGateway
{
    /// <summary>
    /// 校验设备是否允许进站
    /// </summary>
    Task<EquipmentCheckResult> CheckEquipmentAsync(string equipmentId, string stepCode);

    /// <summary>
    /// 校验设备状态（Available/Running/Maintenance/Offline/PM）
    /// </summary>
    Task<bool> IsEquipmentAvailableAsync(string equipmentId);

    /// <summary>
    /// 校验设备是否属于指定设备组
    /// </summary>
    Task<bool> IsEquipmentInGroupAsync(string equipmentId, string equipmentGroup);
}
```

- [ ] **Step 2：创建 IRecipeGateway.cs**

```csharp
namespace MES.Modules.Production.Services;

public interface IRecipeGateway
{
    /// <summary>
    /// 校验 Recipe 是否已审批且适配当前 Step
    /// </summary>
    Task<bool> IsRecipeApprovedAsync(string recipeId, string stepCode, string equipmentId);

    /// <summary>
    /// 校验 Recipe 与设备是否匹配
    /// </summary>
    Task<bool> IsRecipeMatchEquipmentAsync(string recipeId, string equipmentId);

    /// <summary>
    /// 获取 Step 可用的 Recipe 列表
    /// </summary>
    Task<List<string>> GetAvailableRecipesAsync(string stepCode, string equipmentGroup);
}
```

- [ ] **Step 3：创建 IQualityGateway.cs**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IQualityGateway
{
    /// <summary>
    /// 检查批次是否需要质量 Gate 放行
    /// </summary>
    Task<bool> RequiresQualityGateAsync(string lotId, string stepCode);

    /// <summary>
    /// 检查质量 Gate 是否已通过
    /// </summary>
    Task<bool> IsQualityGatePassedAsync(string lotId, string stepCode);

    /// <summary>
    /// 创建质量 Gate
    /// </summary>
    Task<QualityGate> CreateQualityGateAsync(string lotId, string stepCode, int stepSeq, string gateType);

    /// <summary>
    /// 放行质量 Gate
    /// </summary>
    Task<bool> PassQualityGateAsync(string gateId, string checkedBy, string checkedByName, string? comment = null);
}
```

- [ ] **Step 4：创建 IWarehouseGateway.cs**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IWarehouseGateway
{
    /// <summary>
    /// 检查物料是否齐套
    /// </summary>
    Task<bool> IsMaterialReadyAsync(string lotId, string stepCode);

    /// <summary>
    /// 记录物料消耗
    /// </summary>
    Task RecordMaterialConsumeAsync(MaterialConsume consume);

    /// <summary>
    /// 获取 Step 所需物料清单
    /// </summary>
    Task<List<string>> GetRequiredMaterialsAsync(string stepCode);
}
```

- [ ] **Step 5：创建 IAlarmService.cs**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IAlarmService
{
    /// <summary>
    /// 触发报警
    /// </summary>
    Task<AlarmRecord> RaiseAlarmAsync(string alarmType, string message, string? lotId = null, string? equipmentId = null, string? stepCode = null, string? detail = null);

    /// <summary>
    /// 确认报警
    /// </summary>
    Task AcknowledgeAlarmAsync(string alarmId, string acknowledgedBy);

    /// <summary>
    /// 解决报警
    /// </summary>
    Task ResolveAlarmAsync(string alarmId, string resolvedBy);

    /// <summary>
    /// 获取未解决报警
    /// </summary>
    Task<List<AlarmRecord>> GetActiveAlarmsAsync();

    /// <summary>
    /// 按类型查询报警
    /// </summary>
    Task<List<AlarmRecord>> GetAlarmsByTypeAsync(string alarmType);

    /// <summary>
    /// 检查报警规则并自动触发
    /// </summary>
    Task CheckAndRaiseAsync(string alarmType, string lotId, string? equipmentId, string? stepCode, double? yieldValue = null, int? qtyValue = null, TimeSpan? duration = null);
}
```

- [ ] **Step 6：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors

---

### Task 4：创建 Gateway 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/EquipmentGateway.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/RecipeGateway.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/QualityGateway.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/WarehouseGateway.cs`

- [ ] **Step 1：创建 EquipmentGateway.cs**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class EquipmentGateway : IEquipmentGateway
{
    private readonly IRedisService _redis;
    private readonly IMasterDataService _masterData;

    public EquipmentGateway(IRedisService redis, IMasterDataService masterData)
    {
        _redis = redis;
        _masterData = masterData;
    }

    public async Task<EquipmentCheckResult> CheckEquipmentAsync(string equipmentId, string stepCode)
    {
        var result = new EquipmentCheckResult { IsAllowed = true };

        var equipment = await _masterData.GetEquipmentAsync(equipmentId);
        if (equipment is null)
        {
            result.IsAllowed = false;
            result.Reason = $"设备 {equipmentId} 不存在";
            return result;
        }

        result.EquipmentStatus = equipment.Status;

        // 状态检查
        if (!await IsEquipmentAvailableAsync(equipmentId))
        {
            result.IsAllowed = false;
            result.Reason = $"设备 {equipmentId} 状态为 {equipment.Status}，不允许进站";
            return result;
        }

        // 设备组检查
        var routeStep = await GetStepFromRouteAsync(stepCode);
        if (routeStep is not null && !string.IsNullOrEmpty(routeStep.EquipmentGroup))
        {
            if (!await IsEquipmentInGroupAsync(equipmentId, routeStep.EquipmentGroup))
            {
                result.IsAllowed = false;
                result.Reason = $"设备 {equipmentId} 不属于工序 {stepCode} 允许的设备组 {routeStep.EquipmentGroup}";
                return result;
            }
        }

        return result;
    }

    public async Task<bool> IsEquipmentAvailableAsync(string equipmentId)
    {
        var equipment = await _masterData.GetEquipmentAsync(equipmentId);
        if (equipment is null) return false;
        return equipment.Status is "Available" or "Running";
    }

    public async Task<bool> IsEquipmentInGroupAsync(string equipmentId, string equipmentGroup)
    {
        var equipment = await _masterData.GetEquipmentAsync(equipmentId);
        if (equipment is null) return false;
        return equipment.EquipmentGroup == equipmentGroup;
    }

    private async Task<RouteStep?> GetStepFromRouteAsync(string stepCode)
    {
        // 从所有 Route 中查找匹配的 Step
        var routes = await _masterData.GetAllEquipmentsAsync(); // 这里简化处理，实际应从 RouteService 获取
        return null; // 暂时返回 null，由 TrackService 传入 Step 信息
    }
}
```

- [ ] **Step 2：创建 RecipeGateway.cs**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class RecipeGateway : IRecipeGateway
{
    private readonly IMasterDataService _masterData;

    public RecipeGateway(IMasterDataService masterData)
    {
        _masterData = masterData;
    }

    public async Task<bool> IsRecipeApprovedAsync(string recipeId, string stepCode, string equipmentId)
    {
        if (string.IsNullOrEmpty(recipeId)) return true; // 无 Recipe 要求

        var recipe = await _masterData.GetRecipeAsync(recipeId);
        if (recipe is null) return false;

        // 检查审批状态
        if (!recipe.IsActive) return false;

        // 检查 Step 匹配
        if (!string.IsNullOrEmpty(recipe.StepCode) && recipe.StepCode != stepCode) return false;

        // 检查设备匹配
        return await IsRecipeMatchEquipmentAsync(recipeId, equipmentId);
    }

    public async Task<bool> IsRecipeMatchEquipmentAsync(string recipeId, string equipmentId)
    {
        var recipe = await _masterData.GetRecipeAsync(recipeId);
        var equipment = await _masterData.GetEquipmentAsync(equipmentId);

        if (recipe is null || equipment is null) return false;

        // Recipe 的设备组必须与设备一致
        return recipe.EquipmentGroup == equipment.EquipmentGroup;
    }

    public async Task<List<string>> GetAvailableRecipesAsync(string stepCode, string equipmentGroup)
    {
        var allRecipes = await _masterData.GetAllRecipesAsync();
        return allRecipes
            .Where(r => r.IsActive &&
                       (string.IsNullOrEmpty(r.StepCode) || r.StepCode == stepCode) &&
                       (string.IsNullOrEmpty(r.EquipmentGroup) || r.EquipmentGroup == equipmentGroup))
            .Select(r => r.RecipeId)
            .ToList();
    }
}
```

- [ ] **Step 3：创建 QualityGateway.cs**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class QualityGateway : IQualityGateway
{
    private readonly IRedisService _redis;
    private const string QualityGateKeyPrefix = "mes:quality:gate:";
    private const string QualityGateIndexKey = "mes:quality:gate:index:";

    private static string GateKey(string gateId) => $"{QualityGateKeyPrefix}{gateId}";
    private static string LotGateIndexKey(string lotId, string stepCode) => $"{QualityGateIndexKey}{lotId}:{stepCode}";

    public QualityGateway(IRedisService redis) => _redis = redis;

    public async Task<bool> RequiresQualityGateAsync(string lotId, string stepCode)
    {
        var indexKey = LotGateIndexKey(lotId, stepCode);
        var gateIds = await _redis.ListRangeAsync(indexKey);
        return gateIds.Count > 0;
    }

    public async Task<bool> IsQualityGatePassedAsync(string lotId, string stepCode)
    {
        var indexKey = LotGateIndexKey(lotId, stepCode);
        var gateIds = await _redis.ListRangeAsync(indexKey);

        if (gateIds.Count == 0) return true; // 无 Gate 要求，默认通过

        foreach (var gateId in gateIds)
        {
            var gate = await _redis.GetObjectAsync<QualityGate>(GateKey(gateId));
            if (gate is null || gate.Status != "Passed") return false;
        }

        return true;
    }

    public async Task<QualityGate> CreateQualityGateAsync(string lotId, string stepCode, int stepSeq, string gateType)
    {
        var gate = new QualityGate
        {
            GateId = Guid.NewGuid().ToString("N"),
            LotId = lotId,
            StepCode = stepCode,
            StepSeq = stepSeq,
            GateType = gateType,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _redis.SetObjectAsync(GateKey(gate.GateId), gate);
        await _redis.ListRightPushAsync(LotGateIndexKey(lotId, stepCode), gate.GateId);

        return gate;
    }

    public async Task<bool> PassQualityGateAsync(string gateId, string checkedBy, string checkedByName, string? comment = null)
    {
        var gate = await _redis.GetObjectAsync<QualityGate>(GateKey(gateId));
        if (gate is null) return false;

        gate.Status = "Passed";
        gate.CheckedBy = checkedBy;
        gate.CheckedByName = checkedByName;
        gate.CheckedAt = DateTime.UtcNow;
        gate.Comment = comment;

        await _redis.SetObjectAsync(GateKey(gateId), gate);
        return true;
    }
}
```

- [ ] **Step 4：创建 WarehouseGateway.cs**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class WarehouseGateway : IWarehouseGateway
{
    private readonly IRedisService _redis;
    private const string MaterialRequirementKey = "mes:material:requirement:";
    private const string MaterialConsumeKey = "mes:material:consume:";

    public WarehouseGateway(IRedisService redis) => _redis = redis;

    public async Task<bool> IsMaterialReadyAsync(string lotId, string stepCode)
    {
        // 获取 Step 所需物料
        var requiredMaterials = await GetRequiredMaterialsAsync(stepCode);
        if (requiredMaterials.Count == 0) return true; // 无物料要求

        // 检查物料是否齐套（简化实现：检查物料是否存在）
        foreach (var materialId in requiredMaterials)
        {
            var material = await _redis.GetObjectAsync<object>($"mes:master:material:{materialId}");
            if (material is null) return false;
        }

        return true;
    }

    public async Task RecordMaterialConsumeAsync(MaterialConsume consume)
    {
        await _redis.SetObjectAsync($"{MaterialConsumeKey}{consume.ConsumeId}", consume);
        await _redis.ListRightPushAsync($"mes:material:consume:{consume.LotId}", consume.ConsumeId);
    }

    public async Task<List<string>> GetRequiredMaterialsAsync(string stepCode)
    {
        var materials = await _redis.ListRangeAsync($"{MaterialRequirementKey}{stepCode}");
        return materials;
    }
}
```

- [ ] **Step 5：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors

---

### Task 5：创建 AlarmService 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/AlarmService.cs`

- [ ] **Step 1：创建 AlarmService.cs**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class AlarmService : IAlarmService
{
    private readonly IRedisService _redis;
    private const string AlarmKeyPrefix = "mes:alarm:";
    private const string AlarmIndexKey = "mes:alarm:index";
    private const string AlarmActiveKey = "mes:alarm:active";
    private const string AlarmRuleKeyPrefix = "mes:alarm:rule:";

    public AlarmService(IRedisService redis) => _redis = redis;

    public async Task<AlarmRecord> RaiseAlarmAsync(string alarmType, string message, string? lotId = null, string? equipmentId = null, string? stepCode = null, string? detail = null)
    {
        var alarm = new AlarmRecord
        {
            AlarmType = alarmType,
            Message = message,
            LotId = lotId,
            EquipmentId = equipmentId,
            StepCode = stepCode,
            Detail = detail,
            TriggeredAt = DateTime.UtcNow
        };

        // 根据规则设置严重级别
        var rule = await _redis.GetObjectAsync<AlarmRule>($"{AlarmRuleKeyPrefix}{alarmType}");
        if (rule is not null)
        {
            alarm.Severity = rule.Severity;
        }

        await _redis.SetObjectAsync($"{AlarmKeyPrefix}{alarm.AlarmId}", alarm);
        await _redis.ListRightPushAsync(AlarmIndexKey, alarm.AlarmId);
        await _redis.ListRightPushAsync(AlarmActiveKey, alarm.AlarmId);

        return alarm;
    }

    public async Task AcknowledgeAlarmAsync(string alarmId, string acknowledgedBy)
    {
        var alarm = await _redis.GetObjectAsync<AlarmRecord>($"{AlarmKeyPrefix}{alarmId}");
        if (alarm is null) return;

        alarm.IsAcknowledged = true;
        alarm.AcknowledgedBy = acknowledgedBy;
        alarm.AcknowledgedAt = DateTime.UtcNow;

        await _redis.SetObjectAsync($"{AlarmKeyPrefix}{alarmId}", alarm);
    }

    public async Task ResolveAlarmAsync(string alarmId, string resolvedBy)
    {
        var alarm = await _redis.GetObjectAsync<AlarmRecord>($"{AlarmKeyPrefix}{alarmId}");
        if (alarm is null) return;

        alarm.ResolvedBy = resolvedBy;
        alarm.ResolvedAt = DateTime.UtcNow;

        await _redis.SetObjectAsync($"{AlarmKeyPrefix}{alarmId}", alarm);

        // 从活跃列表移除
        await _redis.ListRightPushAsync($"mes:alarm:resolved:{alarmId}", alarmId);
    }

    public async Task<List<AlarmRecord>> GetActiveAlarmsAsync()
    {
        var activeIds = await _redis.ListRangeAsync(AlarmActiveKey);
        var alarms = new List<AlarmRecord>();

        foreach (var id in activeIds)
        {
            var alarm = await _redis.GetObjectAsync<AlarmRecord>($"{AlarmKeyPrefix}{id}");
            if (alarm is not null && !alarm.ResolvedAt.HasValue)
            {
                alarms.Add(alarm);
            }
        }

        return alarms;
    }

    public async Task<List<AlarmRecord>> GetAlarmsByTypeAsync(string alarmType)
    {
        var allIds = await _redis.ListRangeAsync(AlarmIndexKey);
        var alarms = new List<AlarmRecord>();

        foreach (var id in allIds)
        {
            var alarm = await _redis.GetObjectAsync<AlarmRecord>($"{AlarmKeyPrefix}{id}");
            if (alarm is not null && alarm.AlarmType == alarmType)
            {
                alarms.Add(alarm);
            }
        }

        return alarms;
    }

    public async Task CheckAndRaiseAsync(string alarmType, string lotId, string? equipmentId, string? stepCode, double? yieldValue = null, int? qtyValue = null, TimeSpan? duration = null)
    {
        var rule = await _redis.GetObjectAsync<AlarmRule>($"{AlarmRuleKeyPrefix}{alarmType}");
        if (rule is null || !rule.IsEnabled) return;

        bool shouldRaise = false;
        string message = string.Empty;

        switch (alarmType)
        {
            case "LowYield":
                if (yieldValue.HasValue && rule.ThresholdYield.HasValue && yieldValue.Value < rule.ThresholdYield.Value)
                {
                    shouldRaise = true;
                    message = $"良率 {yieldValue.Value:F1}% 低于阈值 {rule.ThresholdYield.Value:F1}%";
                }
                break;

            case "QueueTimeout":
            case "HoldTimeout":
                if (duration.HasValue && rule.ThresholdMinutes.HasValue && duration.Value.TotalMinutes > rule.ThresholdMinutes.Value)
                {
                    shouldRaise = true;
                    message = $"超时 {duration.Value.TotalMinutes:F0} 分钟，超过阈值 {rule.ThresholdMinutes.Value} 分钟";
                }
                break;

            case "MaterialShort":
                if (qtyValue.HasValue && rule.ThresholdQty.HasValue && qtyValue.Value < rule.ThresholdQty.Value)
                {
                    shouldRaise = true;
                    message = $"物料数量 {qtyValue.Value} 低于阈值 {rule.ThresholdQty.Value}";
                }
                break;
        }

        if (shouldRaise)
        {
            await RaiseAlarmAsync(alarmType, message, lotId, equipmentId, stepCode);
        }
    }
}
```

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors

---

### Task 6：增强 TrackService 校验链

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/Services/TrackService.cs`

- [ ] **Step 1：在 TrackService 构造函数中注入 Gateway 服务**

修改 TrackService.cs 的构造函数，添加 4 个 Gateway 依赖：

```csharp
private readonly IEquipmentGateway _equipmentGateway;
private readonly IRecipeGateway _recipeGateway;
private readonly IQualityGateway _qualityGateway;
private readonly IWarehouseGateway _warehouseGateway;
private readonly IAlarmService _alarmService;

public TrackService(
    IRedisService redis,
    IRouteService routeService,
    IQuantityService quantityService,
    IOperationHistoryService opHistoryService,
    IAuditService auditService,
    IYieldService yieldService,
    IGenealogyService genealogyService,
    ICarrierService carrierService,
    IEquipmentGateway equipmentGateway,
    IRecipeGateway recipeGateway,
    IQualityGateway qualityGateway,
    IWarehouseGateway warehouseGateway,
    IAlarmService alarmService)
{
    _redis = redis;
    _routeService = routeService;
    _quantityService = quantityService;
    _opHistoryService = opHistoryService;
    _auditService = auditService;
    _yieldService = yieldService;
    _genealogyService = genealogyService;
    _carrierService = carrierService;
    _equipmentGateway = equipmentGateway;
    _recipeGateway = recipeGateway;
    _qualityGateway = qualityGateway;
    _warehouseGateway = warehouseGateway;
    _alarmService = alarmService;
}
```

- [ ] **Step 2：在 ValidateTrackInAsync 中增加联动校验**

在 ValidateTrackInAsync 方法中，现有校验之后添加联动校验：

```csharp
// 联动校验 1：设备状态检查
var equipCheck = await _equipmentGateway.CheckEquipmentAsync(request.EquipmentId, request.StepCode);
if (!equipCheck.IsAllowed)
{
    result.AddError(equipCheck.Reason ?? "设备校验失败");
    await _alarmService.RaiseAlarmAsync("EquipmentDown", equipCheck.Reason ?? "设备不允许进站", equipmentId: request.EquipmentId, stepCode: request.StepCode);
}

// 联动校验 2：Recipe 审批检查
if (!string.IsNullOrEmpty(request.RecipeId))
{
    var recipeApproved = await _recipeGateway.IsRecipeApprovedAsync(request.RecipeId, request.StepCode, request.EquipmentId);
    if (!recipeApproved)
    {
        result.AddError($"Recipe {request.RecipeId} 未审批或不匹配");
        await _alarmService.RaiseAlarmAsync("RecipeError", $"Recipe {request.RecipeId} 未审批", lotId: request.LotId, equipmentId: request.EquipmentId, stepCode: request.StepCode);
    }
}

// 联动校验 3：质量 Gate 检查
var requiresGate = await _qualityGateway.RequiresQualityGateAsync(request.LotId, request.StepCode);
if (requiresGate)
{
    var gatePassed = await _qualityGateway.IsQualityGatePassedAsync(request.LotId, request.StepCode);
    if (!gatePassed)
    {
        result.AddError($"工序 {request.StepCode} 需要质量 Gate 放行");
    }
}

// 联动校验 4：物料齐套检查
var materialReady = await _warehouseGateway.IsMaterialReadyAsync(request.LotId, request.StepCode);
if (!materialReady)
{
    result.AddError($"工序 {request.StepCode} 物料不齐套");
    await _alarmService.RaiseAlarmAsync("MaterialShort", $"物料不齐套", lotId: request.LotId, stepCode: request.StepCode);
}
```

- [ ] **Step 3：在 TrackOutAsync 中增加良率报警**

在 TrackOutAsync 方法中，良率计算后添加报警检查：

```csharp
// 良率报警检查
await _alarmService.CheckAndRaiseAsync("LowYield", request.LotId, request.EquipmentId, request.StepCode, yieldValue: yield);
```

- [ ] **Step 4：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors

---

### Task 7：注册服务 + Seed 数据

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/ProductionModule.cs`
- Modify: `src/Client/Modules/MES.Modules.Production/Services/ProductionDataService.cs`

- [ ] **Step 1：在 ProductionModule.cs 中注册新服务**

在 RegisterTypes 方法中添加：

```csharp
// V3 Phase 1 新增服务
containerRegistry.Register<IEquipmentGateway, EquipmentGateway>();
containerRegistry.Register<IRecipeGateway, RecipeGateway>();
containerRegistry.Register<IQualityGateway, QualityGateway>();
containerRegistry.Register<IWarehouseGateway, WarehouseGateway>();
containerRegistry.Register<IAlarmService, AlarmService>();
```

- [ ] **Step 2：在 ProductionDataService.cs 中添加 AlarmRule Seed 数据**

在 BuildSeedAsync 方法中添加：

```csharp
// AlarmRule Seed 数据
var alarmRules = new List<AlarmRule>
{
    new AlarmRule { RuleId = "AR-001", AlarmType = "LowYield", IsEnabled = true, Severity = "Error", ThresholdYield = 90.0, NotifyRole = "QA" },
    new AlarmRule { RuleId = "AR-002", AlarmType = "QueueTimeout", IsEnabled = true, Severity = "Warning", ThresholdMinutes = 120, NotifyRole = "Supervisor" },
    new AlarmRule { RuleId = "AR-003", AlarmType = "HoldTimeout", IsEnabled = true, Severity = "Warning", ThresholdMinutes = 480, NotifyRole = "QA" },
    new AlarmRule { RuleId = "AR-004", AlarmType = "EquipmentDown", IsEnabled = true, Severity = "Critical", NotifyRole = "Maintenance" },
    new AlarmRule { RuleId = "AR-005", AlarmType = "RecipeError", IsEnabled = true, Severity = "Error", NotifyRole = "Engineer" },
    new AlarmRule { RuleId = "AR-006", AlarmType = "MaterialShort", IsEnabled = true, Severity = "Warning", ThresholdQty = 10, NotifyRole = "Warehouse" },
    new AlarmRule { RuleId = "AR-007", AlarmType = "ForceOperation", IsEnabled = true, Severity = "Critical", NotifyRole = "Manager" },
    new AlarmRule { RuleId = "AR-008", AlarmType = "QtyImbalance", IsEnabled = true, Severity = "Error", NotifyRole = "QA" }
};

foreach (var rule in alarmRules)
{
    await _redis.SetObjectAsync($"mes:alarm:rule:{rule.AlarmType}", rule);
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors

---

### Task 8：编写单元测试

**Files:**
- Create: `test/MES.Modules.Production.Tests/AlarmServiceTests.cs`
- Create: `test/MES.Modules.Production.Tests/GatewayTests.cs`

- [ ] **Step 1：创建 AlarmServiceTests.cs**

```csharp
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using MES.Infrastructure.Cache;
using Xunit;

namespace MES.Modules.Production.Tests;

public class AlarmServiceTests
{
    private readonly InMemoryRedisService _redis;
    private readonly AlarmService _service;

    public AlarmServiceTests()
    {
        _redis = new InMemoryRedisService();
        _service = new AlarmService(_redis);
    }

    [Fact]
    public async Task RaiseAlarmAsync_CreatesAlarmRecord()
    {
        var alarm = await _service.RaiseAlarmAsync("LowYield", "良率过低", lotId: "LOT-001");

        Assert.NotNull(alarm);
        Assert.Equal("LowYield", alarm.AlarmType);
        Assert.Equal("LOT-001", alarm.LotId);
        Assert.False(alarm.IsAcknowledged);
        Assert.Null(alarm.ResolvedAt);
    }

    [Fact]
    public async Task AcknowledgeAlarmAsync_SetsAcknowledged()
    {
        var alarm = await _service.RaiseAlarmAsync("LowYield", "良率过低");
        await _service.AcknowledgeAlarmAsync(alarm.AlarmId, "USER-001");

        var alarms = await _service.GetActiveAlarmsAsync();
        var acknowledged = alarms.FirstOrDefault(a => a.AlarmId == alarm.AlarmId);
        Assert.NotNull(acknowledged);
        Assert.True(acknowledged.IsAcknowledged);
        Assert.Equal("USER-001", acknowledged.AcknowledgedBy);
    }

    [Fact]
    public async Task ResolveAlarmAsync_SetsResolved()
    {
        var alarm = await _service.RaiseAlarmAsync("LowYield", "良率过低");
        await _service.ResolveAlarmAsync(alarm.AlarmId, "USER-001");

        var alarms = await _service.GetActiveAlarmsAsync();
        var resolved = alarms.FirstOrDefault(a => a.AlarmId == alarm.AlarmId);
        Assert.Null(resolved); // 已解决的报警不应出现在活跃列表
    }

    [Fact]
    public async Task GetAlarmsByTypeAsync_ReturnsMatchingAlarms()
    {
        await _service.RaiseAlarmAsync("LowYield", "良率过低 1");
        await _service.RaiseAlarmAsync("LowYield", "良率过低 2");
        await _service.RaiseAlarmAsync("EquipmentDown", "设备宕机");

        var lowYieldAlarms = await _service.GetAlarmsByTypeAsync("LowYield");
        Assert.Equal(2, lowYieldAlarms.Count);
        Assert.All(lowYieldAlarms, a => Assert.Equal("LowYield", a.AlarmType));
    }
}
```

- [ ] **Step 2：创建 GatewayTests.cs**

```csharp
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using MES.Infrastructure.Cache;
using Moq;
using Xunit;

namespace MES.Modules.Production.Tests;

public class GatewayTests
{
    private readonly InMemoryRedisService _redis;
    private readonly Mock<IMasterDataService> _masterDataMock;

    public GatewayTests()
    {
        _redis = new InMemoryRedisService();
        _masterDataMock = new Mock<IMasterDataService>();
    }

    [Fact]
    public async Task EquipmentGateway_CheckEquipment_NullEquipment_ReturnsNotAllowed()
    {
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync((EquipmentInfo?)null);

        var gateway = new EquipmentGateway(_redis, _masterDataMock.Object);
        var result = await gateway.CheckEquipmentAsync("EQ-001", "S1");

        Assert.False(result.IsAllowed);
        Assert.Contains("不存在", result.Reason ?? string.Empty);
    }

    [Fact]
    public async Task EquipmentGateway_CheckEquipment_OfflineEquipment_ReturnsNotAllowed()
    {
        var equipment = new EquipmentInfo
        {
            EquipmentId = "EQ-001",
            EquipmentName = "测试设备",
            EquipmentGroup = "Saw",
            Status = "Offline"
        };
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync(equipment);

        var gateway = new EquipmentGateway(_redis, _masterDataMock.Object);
        var result = await gateway.CheckEquipmentAsync("EQ-001", "S1");

        Assert.False(result.IsAllowed);
        Assert.Contains("Offline", result.Reason ?? string.Empty);
    }

    [Fact]
    public async Task EquipmentGateway_CheckEquipment_AvailableEquipment_ReturnsAllowed()
    {
        var equipment = new EquipmentInfo
        {
            EquipmentId = "EQ-001",
            EquipmentName = "测试设备",
            EquipmentGroup = "Saw",
            Status = "Available"
        };
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync(equipment);

        var gateway = new EquipmentGateway(_redis, _masterDataMock.Object);
        var result = await gateway.CheckEquipmentAsync("EQ-001", "S1");

        Assert.True(result.IsAllowed);
    }

    [Fact]
    public async Task RecipeGateway_IsRecipeApproved_InactiveRecipe_ReturnsFalse()
    {
        var recipe = new RecipeInfo
        {
            RecipeId = "RC-001",
            RecipeName = "测试 Recipe",
            EquipmentGroup = "Saw",
            StepCode = "S1",
            IsActive = false
        };
        _masterDataMock.Setup(m => m.GetRecipeAsync("RC-001")).ReturnsAsync(recipe);

        var gateway = new RecipeGateway(_masterDataMock.Object);
        var result = await gateway.IsRecipeApprovedAsync("RC-001", "S1", "EQ-001");

        Assert.False(result);
    }

    [Fact]
    public async Task RecipeGateway_IsRecipeApproved_ActiveRecipe_ReturnsTrue()
    {
        var recipe = new RecipeInfo
        {
            RecipeId = "RC-001",
            RecipeName = "测试 Recipe",
            EquipmentGroup = "Saw",
            StepCode = "S1",
            IsActive = true
        };
        var equipment = new EquipmentInfo
        {
            EquipmentId = "EQ-001",
            EquipmentGroup = "Saw",
            Status = "Available"
        };
        _masterDataMock.Setup(m => m.GetRecipeAsync("RC-001")).ReturnsAsync(recipe);
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync(equipment);

        var gateway = new RecipeGateway(_masterDataMock.Object);
        var result = await gateway.IsRecipeApprovedAsync("RC-001", "S1", "EQ-001");

        Assert.True(result);
    }
}
```

- [ ] **Step 3：运行测试**

Run: `dotnet test test/MES.Modules.Production.Tests/MES.Modules.Production.Tests.csproj -v n`
Expected: All tests pass

---

## 验收标准

1. 设备 Down/PM 时不能进站 ✅
2. Recipe 未审批不能进站 ✅
3. QA Hold 未释放不能进站 ✅
4. 物料不齐套不能进站 ✅
5. 低良率自动报警 ✅
6. Queue Time 超时报警 ✅
7. 所有 Gateway 提供默认实现（返回 true），预留外部系统对接点 ✅
8. 单元测试通过 ✅
