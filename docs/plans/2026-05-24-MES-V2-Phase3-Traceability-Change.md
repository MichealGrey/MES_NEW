# MES V2 Phase 3 追溯变更 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现 Lot Genealogy 全链路追溯、Split/Merge 服务、Carrier 全追踪、物料绑定、Retest 流程，使 MES 具备完整的批次追溯和变更管理能力。

**Architecture:** 在 Phase 2 已创建的 8 个模型基础上，新增 4 个服务（LotSplitMergeService、CarrierService、ReworkService、ScrapService）和 4 个 ViewModel+View（拆批、合批、重工、谱系追溯），增强 TrackService 的载具绑定和 Genealogy 记录逻辑，重写 Seed 数据展示完整追溯链。

**Tech Stack:** .NET 8, WPF+Prism, MySQL (Pomelo EF Core), IRedisService 抽象层

---

## 文件结构总览

### 新建文件（12 个）

| # | 文件 | 职责 |
|---|------|------|
| 1 | `Services/ILotSplitMergeService.cs` | 拆批/合批服务接口 |
| 2 | `Services/LotSplitMergeService.cs` | 拆批/合批服务实现 |
| 3 | `Services/ICarrierService.cs` | 载具服务接口 |
| 4 | `Services/CarrierService.cs` | 载具服务实现 |
| 5 | `Services/IReworkService.cs` | 重工服务接口 |
| 6 | `Services/ReworkService.cs` | 重工服务实现 |
| 7 | `Services/IScrapService.cs` | 报废服务接口 |
| 8 | `Services/ScrapService.cs` | 报废服务实现 |
| 9 | `ViewModels/GenealogyViewModel.cs` | 谱系追溯 ViewModel |
| 10 | `Views/GenealogyView.xaml` | 谱系追溯 UI |
| 11 | `ViewModels/LotSplitMergeViewModel.cs` | 拆批/合批 ViewModel |
| 12 | `Views/LotSplitMergeView.xaml` | 拆批/合批 UI |

### 修改文件（6 个）

| # | 文件 | 修改内容 |
|---|------|---------|
| 1 | `Services/TrackService.cs` | TrackOut 增加载具绑定/解绑、Genealogy 记录增强 |
| 2 | `Services/ProductionDataService.cs` | 重写 Seed：增加 Genealogy 数据、Carrier 绑定记录 |
| 3 | `ProductionModule.cs` | 注册 4 个新服务 + 2 个 View |
| 4 | `Models/LotGenealogy.cs` | 确认字段完整 |
| 5 | `Models/LotCarrierBinding.cs` | 确认字段完整 |
| 6 | `Models/TrackRequest.cs` | 新增 CarrierType 字段 |

---

## Phase 3-A：拆批/合批服务

### Task 1：创建 ILotSplitMergeService 接口

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/ILotSplitMergeService.cs`

- [ ] **Step 1：创建接口文件**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface ILotSplitMergeService
{
    /// <summary>
    /// 拆批：从母批次拆出一个子批次
    /// </summary>
    Task<LotSplitRecord> SplitLotAsync(string motherLotId, int splitQty,
        string splitReason, string splitType, string operatorId, string operatorName);

    /// <summary>
    /// 等级拆分：按品质等级拆分为 A/B/C 级
    /// </summary>
    Task<List<LotSplitRecord>> GradeSplitAsync(string lotId,
        Dictionary<string, int> gradeQtyMap, string operatorId, string operatorName);

    /// <summary>
    /// 合批：将多个子批次合并到一个目标批次
    /// </summary>
    Task<LotMergeRecord> MergeLotsAsync(string targetLotId, List<string> sourceLotIds,
        string mergeReason, string operatorId, string operatorName);

    /// <summary>
    /// 查询子批次列表
    /// </summary>
    Task<List<LotInfo>> GetChildLotsAsync(string motherLotId);

    /// <summary>
    /// 查询拆批记录
    /// </summary>
    Task<List<LotSplitRecord>> GetSplitRecordsAsync(string lotId);

    /// <summary>
    /// 查询合批记录
    /// </summary>
    Task<List<LotMergeRecord>> GetMergeRecordsAsync(string lotId);
}
```

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 2：创建 LotSplitMergeService 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/LotSplitMergeService.cs`

- [ ] **Step 1：创建实现文件**

```csharp
using System.Text.Json;
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class LotSplitMergeService : ILotSplitMergeService
{
    private readonly IRedisService _redis;
    private readonly ISignatureService _signatureService;
    private readonly IGenealogyService _genealogyService;

    private const string SplitIndexKey = "mes:split:index:";
    private const string MergeIndexKey = "mes:merge:index:";

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = false
    };

    public LotSplitMergeService(IRedisService redis, ISignatureService signatureService,
        IGenealogyService genealogyService)
    {
        _redis = redis;
        _signatureService = signatureService;
        _genealogyService = genealogyService;
    }

    public async Task<LotSplitRecord> SplitLotAsync(string motherLotId, int splitQty,
        string splitReason, string splitType, string operatorId, string operatorName)
    {
        // 1. 获取母批次
        var motherKey = $"mes:lot:{motherLotId}";
        var motherLot = await _redis.GetObjectAsync<LotInfo>(motherKey);
        if (motherLot is null)
            throw new InvalidOperationException($"母批次 {motherLotId} 不存在");

        if (motherLot.UnitCount < splitQty)
            throw new InvalidOperationException($"拆分数量 {splitQty} 超过可用数量 {motherLot.UnitCount}");

        // 2. 生成子批次 ID
        var childLotId = $"{motherLotId}-S{Guid.NewGuid().ToString("N")[..4]}";

        // 3. 创建子批次
        var childLot = new LotInfo
        {
            LotId = childLotId,
            OrderId = motherLot.OrderId,
            ProductId = motherLot.ProductId,
            ProductName = motherLot.ProductName,
            DieName = motherLot.DieName,
            PackageType = motherLot.PackageType,
            RouteId = motherLot.RouteId,
            RouteVersion = motherLot.RouteVersion,
            CurrentStep = motherLot.CurrentStep,
            CurrentStepSeq = motherLot.CurrentStepSeq,
            Status = "Waiting",
            OriginalQty = splitQty,
            UnitCount = splitQty,
            StripCount = Math.Max(1, splitQty / (motherLot.UnitCount / Math.Max(1, motherLot.StripCount))),
            Priority = motherLot.Priority,
            CarrierType = motherLot.CarrierType,
            CarrierId = string.Empty,
            WaferLotId = motherLot.WaferLotId,
            IsPartialLot = true,
            MotherLotId = motherLotId,
            SplitReason = splitReason,
            SplitTime = DateTime.UtcNow,
            SplitQty = splitQty,
            CreatedAt = DateTime.UtcNow
        };

        // 4. 保存子批次
        await _redis.SetObjectAsync($"mes:lot:{childLotId}", childLot);

        // 5. 更新母批次数量
        motherLot.UnitCount -= splitQty;
        await _redis.SetObjectAsync(motherKey, motherLot);

        // 6. 创建拆批记录
        var record = new LotSplitRecord
        {
            MotherLotId = motherLotId,
            ChildLotId = childLotId,
            SplitQty = splitQty,
            SplitReason = splitReason,
            SplitType = splitType,
            StepCode = motherLot.CurrentStep,
            StepSeq = motherLot.CurrentStepSeq,
            OperatorId = operatorId,
            SplitTime = DateTime.UtcNow
        };

        // 7. 保存拆批记录到索引
        await _redis.SetObjectAsync($"mes:split:{record.SplitId}", record);
        await _redis.ListRightPushAsync($"{SplitIndexKey}{motherLotId}", record.SplitId);

        // 8. 记录谱系关系
        await _genealogyService.RecordRelationAsync(new LotGenealogy
        {
            ParentLotId = motherLotId,
            ChildLotId = childLotId,
            RelationType = "Split",
            StepCode = motherLot.CurrentStep,
            StepSeq = motherLot.CurrentStepSeq,
            Qty = splitQty,
            OperatorId = operatorId,
            ReasonCode = splitType,
            Remark = splitReason
        });

        return record;
    }

    public async Task<List<LotSplitRecord>> GradeSplitAsync(string lotId,
        Dictionary<string, int> gradeQtyMap, string operatorId, string operatorName)
    {
        var lotKey = $"mes:lot:{lotId}";
        var lot = await _redis.GetObjectAsync<LotInfo>(lotKey);
        if (lot is null)
            throw new InvalidOperationException($"批次 {lotId} 不存在");

        var totalSplitQty = gradeQtyMap.Values.Sum();
        if (totalSplitQty > lot.UnitCount)
            throw new InvalidOperationException($"拆分总数量 {totalSplitQty} 超过可用数量 {lot.UnitCount}");

        var records = new List<LotSplitRecord>();

        foreach (var (grade, qty) in gradeQtyMap)
        {
            var childLotId = $"{lotId}-G{grade}";

            var childLot = new LotInfo
            {
                LotId = childLotId,
                OrderId = lot.OrderId,
                ProductId = lot.ProductId,
                ProductName = lot.ProductName,
                DieName = lot.DieName,
                PackageType = lot.PackageType,
                RouteId = lot.RouteId,
                RouteVersion = lot.RouteVersion,
                CurrentStep = lot.CurrentStep,
                CurrentStepSeq = lot.CurrentStepSeq,
                Status = "Waiting",
                Grade = grade,
                OriginalLotId = lotId,
                OriginalQty = qty,
                UnitCount = qty,
                StripCount = Math.Max(1, qty / (lot.UnitCount / Math.Max(1, lot.StripCount))),
                Priority = lot.Priority,
                CarrierType = lot.CarrierType,
                CarrierId = string.Empty,
                WaferLotId = lot.WaferLotId,
                IsPartialLot = true,
                MotherLotId = lotId,
                SplitReason = $"Grade Split: {grade}",
                SplitTime = DateTime.UtcNow,
                SplitQty = qty,
                CreatedAt = DateTime.UtcNow
            };

            await _redis.SetObjectAsync($"mes:lot:{childLotId}", childLot);

            var record = new LotSplitRecord
            {
                MotherLotId = lotId,
                ChildLotId = childLotId,
                SplitQty = qty,
                SplitReason = $"Grade Split: {grade}",
                SplitType = "Grade",
                StepCode = lot.CurrentStep,
                StepSeq = lot.CurrentStepSeq,
                OperatorId = operatorId,
                SplitTime = DateTime.UtcNow
            };

            await _redis.SetObjectAsync($"mes:split:{record.SplitId}", record);
            await _redis.ListRightPushAsync($"{SplitIndexKey}{lotId}", record.SplitId);

            await _genealogyService.RecordRelationAsync(new LotGenealogy
            {
                ParentLotId = lotId,
                ChildLotId = childLotId,
                RelationType = "GradeSplit",
                StepCode = lot.CurrentStep,
                StepSeq = lot.CurrentStepSeq,
                Qty = qty,
                Grade = grade,
                OperatorId = operatorId,
                Remark = $"Grade Split: {grade}"
            });

            records.Add(record);
        }

        // 标记原批次为已拆分
        lot.Status = "Completed";
        await _redis.SetObjectAsync(lotKey, lot);

        return records;
    }

    public async Task<LotMergeRecord> MergeLotsAsync(string targetLotId, List<string> sourceLotIds,
        string mergeReason, string operatorId, string operatorName)
    {
        var targetKey = $"mes:lot:{targetLotId}";
        var targetLot = await _redis.GetObjectAsync<LotInfo>(targetKey);
        if (targetLot is null)
            throw new InvalidOperationException($"目标批次 {targetLotId} 不存在");

        int totalMergedQty = 0;

        foreach (var sourceId in sourceLotIds)
        {
            var sourceKey = $"mes:lot:{sourceId}";
            var sourceLot = await _redis.GetObjectAsync<LotInfo>(sourceKey);
            if (sourceLot is null)
                throw new InvalidOperationException($"源批次 {sourceId} 不存在");

            // 验证 Route 一致
            if (sourceLot.RouteId != targetLot.RouteId)
                throw new InvalidOperationException($"批次 {sourceId} 路线 {sourceLot.RouteId} 与目标批次 {targetLot.RouteId} 不一致");

            // 验证 Step 一致
            if (sourceLot.CurrentStepSeq != targetLot.CurrentStepSeq)
                throw new InvalidOperationException($"批次 {sourceId} 工序 {sourceLot.CurrentStepSeq} 与目标批次 {targetLot.CurrentStepSeq} 不一致");

            totalMergedQty += sourceLot.UnitCount;

            // 标记源批次为已合并
            sourceLot.Status = "Merged";
            sourceLot.UnitCount = 0;
            await _redis.SetObjectAsync(sourceKey, sourceLot);

            // 记录谱系
            await _genealogyService.RecordRelationAsync(new LotGenealogy
            {
                ParentLotId = sourceId,
                ChildLotId = targetLotId,
                RelationType = "Merge",
                StepCode = targetLot.CurrentStep,
                StepSeq = targetLot.CurrentStepSeq,
                Qty = sourceLot.UnitCount,
                OperatorId = operatorId,
                Remark = mergeReason
            });
        }

        // 更新目标批次数量
        targetLot.UnitCount += totalMergedQty;
        await _redis.SetObjectAsync(targetKey, targetLot);

        // 创建合批记录
        var record = new LotMergeRecord
        {
            TargetLotId = targetLotId,
            SourceLotIds = sourceLotIds,
            MergedQty = totalMergedQty,
            MergeReason = mergeReason,
            StepCode = targetLot.CurrentStep,
            StepSeq = targetLot.CurrentStepSeq,
            OperatorId = operatorId,
            MergeTime = DateTime.UtcNow
        };

        await _redis.SetObjectAsync($"mes:merge:{record.MergeId}", record);
        await _redis.ListRightPushAsync($"{MergeIndexKey}{targetLotId}", record.MergeId);

        return record;
    }

    public async Task<List<LotInfo>> GetChildLotsAsync(string motherLotId)
    {
        var splitIds = await _redis.ListRangeAsync($"{SplitIndexKey}{motherLotId}");
        var children = new List<LotInfo>();

        foreach (var splitId in splitIds)
        {
            var record = await _redis.GetObjectAsync<LotSplitRecord>($"mes:split:{splitId}");
            if (record is not null)
            {
                var child = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{record.ChildLotId}");
                if (child is not null) children.Add(child);
            }
        }

        return children;
    }

    public async Task<List<LotSplitRecord>> GetSplitRecordsAsync(string lotId)
    {
        var splitIds = await _redis.ListRangeAsync($"{SplitIndexKey}{lotId}");
        var records = new List<LotSplitRecord>();

        foreach (var splitId in splitIds)
        {
            var record = await _redis.GetObjectAsync<LotSplitRecord>($"mes:split:{splitId}");
            if (record is not null) records.Add(record);
        }

        return records;
    }

    public async Task<List<LotMergeRecord>> GetMergeRecordsAsync(string lotId)
    {
        var mergeIds = await _redis.ListRangeAsync($"{MergeIndexKey}{lotId}");
        var records = new List<LotMergeRecord>();

        foreach (var mergeId in mergeIds)
        {
            var record = await _redis.GetObjectAsync<LotMergeRecord>($"mes:merge:{mergeId}");
            if (record is not null) records.Add(record);
        }

        return records;
    }
}
```

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 3-B：载具服务

### Task 3：创建 ICarrierService 接口

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/ICarrierService.cs`

- [ ] **Step 1：创建接口文件**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface ICarrierService
{
    /// <summary>
    /// 绑定载具到批次
    /// </summary>
    Task<LotCarrierBinding> BindCarrierAsync(string lotId, string carrierId,
        string carrierType, string stepCode, int stepSeq, string operatorId);

    /// <summary>
    /// 解绑载具
    /// </summary>
    Task UnbindCarrierAsync(string lotId, string operatorId);

    /// <summary>
    /// 转移载具（从一批次到另一批次）
    /// </summary>
    Task<LotCarrierBinding> TransferCarrierAsync(string fromLotId, string toLotId,
        string stepCode, int stepSeq, string operatorId);

    /// <summary>
    /// 查询载具历史
    /// </summary>
    Task<List<LotCarrierBinding>> GetCarrierHistoryAsync(string lotId);

    /// <summary>
    /// 查询载具当前绑定
    /// </summary>
    Task<LotCarrierBinding?> GetCurrentBindingAsync(string carrierId);
}
```

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 4：创建 CarrierService 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/CarrierService.cs`

- [ ] **Step 1：创建实现文件**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class CarrierService : ICarrierService
{
    private readonly IRedisService _redis;
    private const string CarrierBindingKey = "mes:carrier:binding:";
    private const string CarrierHistoryKey = "mes:carrier:history:";
    private const string CarrierCurrentKey = "mes:carrier:current:";

    public CarrierService(IRedisService redis) => _redis = redis;

    public async Task<LotCarrierBinding> BindCarrierAsync(string lotId, string carrierId,
        string carrierType, string stepCode, int stepSeq, string operatorId)
    {
        // 先解绑当前载具（如果有）
        await UnbindCarrierAsync(lotId, operatorId);

        var binding = new LotCarrierBinding
        {
            LotId = lotId,
            StepCode = stepCode,
            StepSeq = stepSeq,
            CarrierId = carrierId,
            CarrierType = carrierType,
            BindTime = DateTime.UtcNow,
            OperatorId = operatorId
        };

        // 保存绑定记录
        await _redis.SetObjectAsync($"{CarrierBindingKey}{binding.BindingId}", binding);

        // 添加到批次历史
        await _redis.ListRightPushAsync($"{CarrierHistoryKey}{lotId}", binding.BindingId);

        // 更新载具当前绑定
        await _redis.SetObjectAsync($"{CarrierCurrentKey}{carrierId}", binding);

        // 更新 Lot 的载具信息
        var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{lotId}");
        if (lot is not null)
        {
            lot.CarrierId = carrierId;
            await _redis.SetObjectAsync($"mes:lot:{lotId}", lot);
        }

        return binding;
    }

    public async Task UnbindCarrierAsync(string lotId, string operatorId)
    {
        var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{lotId}");
        if (lot is null || string.IsNullOrEmpty(lot.CarrierId)) return;

        var currentCarrierId = lot.CarrierId;

        // 更新 Lot
        lot.CarrierId = string.Empty;
        await _redis.SetObjectAsync($"mes:lot:{lotId}", lot);

        // 清除载具当前绑定
        await _redis.KeyDeleteAsync($"{CarrierCurrentKey}{currentCarrierId}");
    }

    public async Task<LotCarrierBinding> TransferCarrierAsync(string fromLotId, string toLotId,
        string stepCode, int stepSeq, string operatorId)
    {
        var fromLot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{fromLotId}");
        if (fromLot is null || string.IsNullOrEmpty(fromLot.CarrierId))
            throw new InvalidOperationException($"批次 {fromLotId} 未绑定载具");

        var carrierId = fromLot.CarrierId;
        var carrierType = fromLot.CarrierType.ToString();

        // 解绑源批次
        await UnbindCarrierAsync(fromLotId, operatorId);

        // 绑定到目标批次
        var binding = await BindCarrierAsync(toLotId, carrierId, carrierType, stepCode, stepSeq, operatorId);

        // 记录转移
        binding.FromCarrierId = carrierId;
        await _redis.SetObjectAsync($"{CarrierBindingKey}{binding.BindingId}", binding);

        return binding;
    }

    public async Task<List<LotCarrierBinding>> GetCarrierHistoryAsync(string lotId)
    {
        var bindingIds = await _redis.ListRangeAsync($"{CarrierHistoryKey}{lotId}");
        var bindings = new List<LotCarrierBinding>();

        foreach (var id in bindingIds)
        {
            var binding = await _redis.GetObjectAsync<LotCarrierBinding>($"{CarrierBindingKey}{id}");
            if (binding is not null) bindings.Add(binding);
        }

        return bindings;
    }

    public async Task<LotCarrierBinding?> GetCurrentBindingAsync(string carrierId)
    {
        return await _redis.GetObjectAsync<LotCarrierBinding>($"{CarrierCurrentKey}{carrierId}");
    }
}
```

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 3-C：重工服务

### Task 5：创建 IReworkService 接口 + 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/IReworkService.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/ReworkService.cs`

- [ ] **Step 1：创建接口文件**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IReworkService
{
    /// <summary>
    /// 创建重工批次
    /// </summary>
    Task<ReworkRecord> CreateReworkLotAsync(string lotId, string reworkRouteId,
        string fromStepCode, string targetStepCode, int reworkQty, string reason,
        string operatorId, string operatorName);

    /// <summary>
    /// 完成重工，返回原路线
    /// </summary>
    Task CompleteReworkAsync(string reworkLotId, string operatorId, string operatorName);

    /// <summary>
    /// 查询重工记录
    /// </summary>
    Task<List<ReworkRecord>> GetReworkRecordsAsync(string lotId);
}
```

- [ ] **Step 2：创建实现文件**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class ReworkService : IReworkService
{
    private readonly IRedisService _redis;
    private readonly IGenealogyService _genealogyService;
    private const string ReworkIndexKey = "mes:rework:index:";

    public ReworkService(IRedisService redis, IGenealogyService genealogyService)
    {
        _redis = redis;
        _genealogyService = genealogyService;
    }

    public async Task<ReworkRecord> CreateReworkLotAsync(string lotId, string reworkRouteId,
        string fromStepCode, string targetStepCode, int reworkQty, string reason,
        string operatorId, string operatorName)
    {
        var lotKey = $"mes:lot:{lotId}";
        var lot = await _redis.GetObjectAsync<LotInfo>(lotKey);
        if (lot is null)
            throw new InvalidOperationException($"批次 {lotId} 不存在");

        // 获取重工路线
        var reworkRoute = await _redis.GetObjectAsync<RouteInfo>($"mes:route:{reworkRouteId}:1.0");
        if (reworkRoute is null)
            throw new InvalidOperationException($"重工路线 {reworkRouteId} 不存在");

        if (lot.UnitCount < reworkQty)
            throw new InvalidOperationException($"重工数量 {reworkQty} 超过可用数量 {lot.UnitCount}");

        // 创建重工子批次
        var reworkLotId = $"{lotId}-RW{Guid.NewGuid().ToString("N")[..4]}";

        var reworkLot = new LotInfo
        {
            LotId = reworkLotId,
            OrderId = lot.OrderId,
            ProductId = lot.ProductId,
            ProductName = lot.ProductName,
            DieName = lot.DieName,
            PackageType = lot.PackageType,
            RouteId = lot.RouteId,
            RouteVersion = lot.RouteVersion,
            ReworkRouteId = reworkRouteId,
            CurrentStep = reworkRoute.Steps.FirstOrDefault()?.StepName ?? string.Empty,
            CurrentStepSeq = 1,
            Status = "Waiting",
            IsReworkLot = true,
            OriginalRouteId = lot.RouteId,
            ReworkCount = (lot.ReworkCount ?? 0) + 1,
            ReworkReason = reason,
            OriginalQty = reworkQty,
            UnitCount = reworkQty,
            StripCount = Math.Max(1, reworkQty / (lot.UnitCount / Math.Max(1, lot.StripCount))),
            Priority = lot.Priority,
            CarrierType = lot.CarrierType,
            CarrierId = string.Empty,
            WaferLotId = lot.WaferLotId,
            IsPartialLot = true,
            MotherLotId = lotId,
            SplitReason = $"Rework: {reason}",
            SplitTime = DateTime.UtcNow,
            SplitQty = reworkQty,
            CreatedAt = DateTime.UtcNow
        };

        await _redis.SetObjectAsync($"mes:lot:{reworkLotId}", reworkLot);

        // 减少母批次数量
        lot.UnitCount -= reworkQty;
        lot.TotalReworkQty += reworkQty;
        await _redis.SetObjectAsync(lotKey, lot);

        // 创建重工记录
        var record = new ReworkRecord
        {
            LotId = lotId,
            OriginalRouteId = lot.RouteId,
            ReworkRouteId = reworkRouteId,
            FromStepCode = fromStepCode,
            TargetStepCode = targetStepCode,
            ReworkQty = reworkQty,
            ReworkReason = reason,
            OperatorId = operatorId,
            ReworkCount = reworkLot.ReworkCount.Value,
            CreatedAt = DateTime.UtcNow
        };

        await _redis.SetObjectAsync($"mes:rework:{record.ReworkId}", record);
        await _redis.ListRightPushAsync($"{ReworkIndexKey}{lotId}", record.ReworkId);

        // 记录谱系
        await _genealogyService.RecordRelationAsync(new LotGenealogy
        {
            ParentLotId = lotId,
            ChildLotId = reworkLotId,
            RelationType = "Rework",
            StepCode = fromStepCode,
            Qty = reworkQty,
            OperatorId = operatorId,
            Remark = reason
        });

        return record;
    }

    public async Task CompleteReworkAsync(string reworkLotId, string operatorId, string operatorName)
    {
        var reworkLot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{reworkLotId}");
        if (reworkLot is null)
            throw new InvalidOperationException($"重工批次 {reworkLotId} 不存在");

        if (!reworkLot.IsReworkLot)
            throw new InvalidOperationException($"批次 {reworkLotId} 不是重工批次");

        // 更新重工批次状态
        reworkLot.Status = "Completed";
        reworkLot.TotalPassQty += reworkLot.UnitCount;
        await _redis.SetObjectAsync($"mes:lot:{reworkLotId}", reworkLot);

        // 更新重工记录
        var reworkIds = await _redis.ListRangeAsync($"{ReworkIndexKey}{reworkLot.MotherLotId}");
        foreach (var reworkId in reworkIds)
        {
            var record = await _redis.GetObjectAsync<ReworkRecord>($"mes:rework:{reworkId}");
            if (record?.LotId == reworkLot.MotherLotId && !record.CompletedAt.HasValue)
            {
                record.CompletedAt = DateTime.UtcNow;
                await _redis.SetObjectAsync($"mes:rework:{reworkId}", record);
                break;
            }
        }
    }

    public async Task<List<ReworkRecord>> GetReworkRecordsAsync(string lotId)
    {
        var reworkIds = await _redis.ListRangeAsync($"{ReworkIndexKey}{lotId}");
        var records = new List<ReworkRecord>();

        foreach (var reworkId in reworkIds)
        {
            var record = await _redis.GetObjectAsync<ReworkRecord>($"mes:rework:{reworkId}");
            if (record is not null) records.Add(record);
        }

        return records;
    }
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 3-D：报废服务

### Task 6：创建 IScrapService 接口 + 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/IScrapService.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/ScrapService.cs`

- [ ] **Step 1：创建接口文件**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IScrapService
{
    /// <summary>
    /// 记录报废
    /// </summary>
    Task<ScrapRecord> RecordScrapAsync(string lotId, int scrapQty, string reason,
        string reasonCode, string operatorId, string operatorName);

    /// <summary>
    /// 检查是否需要审批
    /// </summary>
    bool RequiresApproval(int scrapQty, int totalQty);

    /// <summary>
    /// 查询报废记录
    /// </summary>
    Task<List<ScrapRecord>> GetScrapRecordsAsync(string lotId);
}
```

- [ ] **Step 2：创建实现文件**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class ScrapService : IScrapService
{
    private readonly IRedisService _redis;
    private const string ScrapIndexKey = "mes:scrap:index:";
    private const int ScrapThresholdPercent = 5; // 超过 5% 需要审批

    public ScrapService(IRedisService redis) => _redis = redis;

    public async Task<ScrapRecord> RecordScrapAsync(string lotId, int scrapQty, string reason,
        string reasonCode, string operatorId, string operatorName)
    {
        var lotKey = $"mes:lot:{lotId}";
        var lot = await _redis.GetObjectAsync<LotInfo>(lotKey);
        if (lot is null)
            throw new InvalidOperationException($"批次 {lotId} 不存在");

        if (scrapQty > lot.UnitCount)
            throw new InvalidOperationException($"报废数量 {scrapQty} 超过可用数量 {lot.UnitCount}");

        var requiresApproval = RequiresApproval(scrapQty, lot.UnitCount);

        var record = new ScrapRecord
        {
            LotId = lotId,
            StepCode = lot.CurrentStep,
            StepSeq = lot.CurrentStepSeq,
            ScrapQty = scrapQty,
            ScrapReason = reason,
            ScrapReasonCode = reasonCode,
            OperatorId = operatorId,
            ScrapTime = DateTime.UtcNow,
            RequiresApproval = requiresApproval
        };

        // 保存报废记录
        await _redis.SetObjectAsync($"mes:scrap:{record.ScrapId}", record);
        await _redis.ListRightPushAsync($"{ScrapIndexKey}{lotId}", record.ScrapId);

        // 更新批次数量
        lot.UnitCount -= scrapQty;
        lot.TotalScrapQty += scrapQty;

        // 如果全部报废，标记为报废状态
        if (lot.UnitCount <= 0)
        {
            lot.Status = "Scrapped";
        }

        await _redis.SetObjectAsync(lotKey, lot);

        return record;
    }

    public bool RequiresApproval(int scrapQty, int totalQty)
    {
        if (totalQty <= 0) return false;
        var scrapRate = (double)scrapQty / totalQty * 100;
        return scrapRate >= ScrapThresholdPercent;
    }

    public async Task<List<ScrapRecord>> GetScrapRecordsAsync(string lotId)
    {
        var scrapIds = await _redis.ListRangeAsync($"{ScrapIndexKey}{lotId}");
        var records = new List<ScrapRecord>();

        foreach (var scrapId in scrapIds)
        {
            var record = await _redis.GetObjectAsync<ScrapRecord>($"mes:scrap:{scrapId}");
            if (record is not null) records.Add(record);
        }

        return records;
    }
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 3-E：增强 TrackService

### Task 7：增强 TrackService 的载具绑定和 Genealogy 记录

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/Services/TrackService.cs`

- [ ] **Step 1：添加 ICarrierService 依赖**

在 TrackService 构造函数中添加 `_carrierService`：

```csharp
    private readonly ICarrierService _carrierService;

    public TrackService(
        IRedisService redis,
        IRouteService routeService,
        IQuantityService quantityService,
        IOperationHistoryService opHistoryService,
        IAuditService auditService,
        IYieldService yieldService,
        IGenealogyService genealogyService,
        ICarrierService carrierService)
    {
        _redis = redis;
        _routeService = routeService;
        _quantityService = quantityService;
        _opHistoryService = opHistoryService;
        _auditService = auditService;
        _yieldService = yieldService;
        _genealogyService = genealogyService;
        _carrierService = carrierService;
    }
```

- [ ] **Step 2：在 TrackInAsync 中添加载具绑定**

在 `await _redis.SetObjectAsync($"mes:lot:{request.LotId}", lot);` 之后添加：

```csharp
        // 载具绑定
        if (!string.IsNullOrEmpty(request.CarrierId))
        {
            var steps = await _routeService.GetStepsAsync(routeId);
            var currentStep = steps.FirstOrDefault(s => s.StepSeq == request.StepSeq);
            var carrierType = currentStep?.RequiredCarrierType ?? string.Empty;

            await _carrierService.BindCarrierAsync(
                request.LotId, request.CarrierId, carrierType,
                request.StepCode, request.StepSeq, request.OperatorId);
        }
```

- [ ] **Step 3：在 TrackOutAsync 中增强 Genealogy 记录**

在现有谱系记录逻辑中，增加 Split/Merge 后的 Genealogy 记录：

```csharp
        // 谱系记录 - 完成时记录
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
        else if (lot.IsPartialLot && !string.IsNullOrEmpty(lot.MotherLotId))
        {
            // 子批次完成时记录与母批的关系
            await _genealogyService.RecordRelationAsync(new LotGenealogy
            {
                ParentLotId = lot.MotherLotId,
                ChildLotId = request.LotId,
                RelationType = "PartialCompleted",
                StepCode = request.StepCode,
                StepSeq = request.StepSeq,
                Qty = request.PassQty,
                OperatorId = request.OperatorId
            });
        }
```

- [ ] **Step 4：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 3-F：谱系追溯 UI

### Task 8：创建 GenealogyViewModel

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/ViewModels/GenealogyViewModel.cs`

- [ ] **Step 1：创建 ViewModel 文件**

```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class GenealogyViewModel : BindableBase
{
    private readonly IRedisService _redis;
    private readonly IGenealogyService _genealogyService;
    private readonly ILotSplitMergeService _splitMergeService;

    private string _searchLotId = string.Empty;
    private LotInfo? _selectedLot;
    private bool _isLoading;
    private string _statusMessage = string.Empty;

    public ObservableCollection<GenealogyNode> GenealogyTree { get; } = [];
    public ObservableCollection<LotInfo> ChildLots { get; } = [];
    public ObservableCollection<LotSplitRecord> SplitRecords { get; } = [];
    public ObservableCollection<LotMergeRecord> MergeRecords { get; } = [];
    public ObservableCollection<LotCarrierBinding> CarrierHistory { get; } = [];
    public ObservableCollection<ReworkRecord> ReworkRecords { get; } = [];
    public ObservableCollection<ScrapRecord> ScrapRecords { get; } = [];

    public string SearchLotId
    {
        get => _searchLotId;
        set => SetProperty(ref _searchLotId, value);
    }

    public LotInfo? SelectedLot
    {
        get => _selectedLot;
        set => SetProperty(ref _selectedLot, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public ICommand SearchCommand { get; }
    public ICommand RefreshCommand { get; }

    public GenealogyViewModel(IRedisService redis, IGenealogyService genealogyService,
        ILotSplitMergeService splitMergeService)
    {
        _redis = redis;
        _genealogyService = genealogyService;
        _splitMergeService = splitMergeService;

        SearchCommand = new DelegateCommand(OnSearch);
        RefreshCommand = new DelegateCommand(OnRefresh);
    }

    private async void OnSearch()
    {
        if (string.IsNullOrWhiteSpace(SearchLotId)) return;

        IsLoading = true;
        StatusMessage = string.Empty;

        try
        {
            var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{SearchLotId}");
            if (lot is null)
            {
                StatusMessage = $"批次 {SearchLotId} 不存在";
                ClearResults();
                return;
            }

            SelectedLot = lot;
            GenealogyTree.Clear();
            ChildLots.Clear();
            SplitRecords.Clear();
            MergeRecords.Clear();
            CarrierHistory.Clear();
            ReworkRecords.Clear();
            ScrapRecords.Clear();

            var upstream = await _genealogyService.GetUpstreamAsync(SearchLotId);
            var downstream = await _genealogyService.GetDownstreamAsync(SearchLotId);

            foreach (var rel in upstream)
            {
                GenealogyTree.Add(new GenealogyNode
                {
                    LotId = rel.ParentLotId,
                    RelationType = rel.RelationType,
                    StepCode = rel.StepCode,
                    Qty = rel.Qty,
                    Grade = rel.Grade,
                    Direction = "Upstream"
                });
            }

            GenealogyTree.Add(new GenealogyNode
            {
                LotId = SearchLotId,
                RelationType = "Current",
                StepCode = lot.CurrentStep,
                Qty = lot.UnitCount,
                Grade = lot.Grade,
                Direction = "Current"
            });

            foreach (var rel in downstream)
            {
                GenealogyTree.Add(new GenealogyNode
                {
                    LotId = rel.ChildLotId,
                    RelationType = rel.RelationType,
                    StepCode = rel.StepCode,
                    Qty = rel.Qty,
                    Grade = rel.Grade,
                    Direction = "Downstream"
                });
            }

            var children = await _splitMergeService.GetChildLotsAsync(SearchLotId);
            foreach (var child in children) ChildLots.Add(child);

            var splits = await _splitMergeService.GetSplitRecordsAsync(SearchLotId);
            foreach (var split in splits) SplitRecords.Add(split);

            var merges = await _splitMergeService.GetMergeRecordsAsync(SearchLotId);
            foreach (var merge in merges) MergeRecords.Add(merge);

            var reworkService = new ReworkService(_redis, _genealogyService);
            var reworks = await reworkService.GetReworkRecordsAsync(SearchLotId);
            foreach (var rework in reworks) ReworkRecords.Add(rework);

            var scrapService = new ScrapService(_redis);
            var scraps = await scrapService.GetScrapRecordsAsync(SearchLotId);
            foreach (var scrap in scraps) ScrapRecords.Add(scrap);

            StatusMessage = $"找到 {GenealogyTree.Count} 条谱系记录";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async void OnRefresh()
    {
        if (!string.IsNullOrWhiteSpace(SearchLotId))
            OnSearch();
    }

    private void ClearResults()
    {
        SelectedLot = null;
        GenealogyTree.Clear();
        ChildLots.Clear();
        SplitRecords.Clear();
        MergeRecords.Clear();
        CarrierHistory.Clear();
        ReworkRecords.Clear();
        ScrapRecords.Clear();
    }
}

public class GenealogyNode : BindableBase
{
    private string _lotId = string.Empty;
    private string _relationType = string.Empty;
    private string _stepCode = string.Empty;
    private int _qty;
    private string? _grade;
    private string _direction = string.Empty;

    public string LotId { get => _lotId; set => SetProperty(ref _lotId, value); }
    public string RelationType { get => _relationType; set => SetProperty(ref _relationType, value); }
    public string StepCode { get => _stepCode; set => SetProperty(ref _stepCode, value); }
    public int Qty { get => _qty; set => SetProperty(ref _qty, value); }
    public string? Grade { get => _grade; set => SetProperty(ref _grade, value); }
    public string Direction { get => _direction; set => SetProperty(ref _direction, value); }

    public string RelationIcon => RelationType switch
    {
        "Split" => "⬇",
        "Merge" => "⬆",
        "GradeSplit" => "🔀",
        "Rework" => "🔄",
        "Completed" => "✅",
        "Current" => "📍",
        _ => "•"
    };
}
```

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 9：创建 GenealogyView

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Views/GenealogyView.xaml`
- Create: `src/Client/Modules/MES.Modules.Production/Views/GenealogyView.xaml.cs`

- [ ] **Step 1：创建 XAML 文件**

```xml
<UserControl x:Class="MES.Modules.Production.Views.GenealogyView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:prism="http://prismlibrary.com/"
             prism:ViewModelLocator.AutoWireViewModel="True"
             Background="{StaticResource MesBackgroundBrush}">

    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- 搜索区 -->
        <Border Grid.Row="0"
                Background="{StaticResource MesCardBrush}"
                BorderBrush="{StaticResource MesBorderBrush}"
                BorderThickness="1"
                CornerRadius="4"
                Padding="16"
                Margin="0,0,0,16">
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="批次号:"
                           VerticalAlignment="Center"
                           Margin="0,0,8,0"/>
                <TextBox Text="{Binding SearchLotId, UpdateSourceTrigger=PropertyChanged}"
                         Width="200"
                         Margin="0,0,8,0"/>
                <Button Content="查询"
                        Command="{Binding SearchCommand}"
                        Margin="0,0,8,0"/>
                <Button Content="刷新"
                        Command="{Binding RefreshCommand}"/>
                <TextBlock Text="{Binding StatusMessage}"
                           VerticalAlignment="Center"
                           Margin="16,0,0,0"
                           Foreground="{StaticResource MesTextSecondaryBrush}"/>
            </StackPanel>
        </Border>

        <!-- 谱系树 -->
        <Border Grid.Row="1"
                Background="{StaticResource MesCardBrush}"
                BorderBrush="{StaticResource MesBorderBrush}"
                BorderThickness="1"
                CornerRadius="4"
                Padding="16"
                Margin="0,0,0,16"
                MaxHeight="300">
            <StackPanel>
                <TextBlock Text="谱系追溯树"
                           FontSize="14"
                           FontWeight="SemiBold"
                           Margin="0,0,0,8"/>
                <DataGrid ItemsSource="{Binding GenealogyTree}"
                          AutoGenerateColumns="False"
                          IsReadOnly="True"
                          Background="{StaticResource MesSurfaceBrush}"
                          BorderBrush="{StaticResource MesBorderBrush}"
                          HeadersVisibility="Column"
                          FontSize="13"
                          GridLinesVisibility="Horizontal"
                          CanUserAddRows="False"
                          CanUserDeleteRows="False"
                          Height="200">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="方向" Binding="{Binding Direction}" Width="80"/>
                        <DataGridTextColumn Header="图标" Binding="{Binding RelationIcon}" Width="50"/>
                        <DataGridTextColumn Header="关系" Binding="{Binding RelationType}" Width="100"/>
                        <DataGridTextColumn Header="批次号" Binding="{Binding LotId}" Width="180"/>
                        <DataGridTextColumn Header="工序" Binding="{Binding StepCode}" Width="80"/>
                        <DataGridTextColumn Header="数量" Binding="{Binding Qty}" Width="80"/>
                        <DataGridTextColumn Header="等级" Binding="{Binding Grade}" Width="60"/>
                    </DataGrid.Columns>
                </DataGrid>
            </StackPanel>
        </Border>

        <!-- 详细信息 -->
        <TabControl Grid.Row="2"
                    Background="{StaticResource MesCardBrush}"
                    BorderBrush="{StaticResource MesBorderBrush}"
                    BorderThickness="1">
            <TabItem Header="子批次">
                <DataGrid ItemsSource="{Binding ChildLots}"
                          AutoGenerateColumns="False"
                          IsReadOnly="True"
                          Background="{StaticResource MesSurfaceBrush}"
                          BorderBrush="{StaticResource MesBorderBrush}"
                          HeadersVisibility="Column"
                          FontSize="13"
                          GridLinesVisibility="Horizontal"
                          CanUserAddRows="False"
                          CanUserDeleteRows="False">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="批次号" Binding="{Binding LotId}" Width="180"/>
                        <DataGridTextColumn Header="状态" Binding="{Binding Status}" Width="80"/>
                        <DataGridTextColumn Header="数量" Binding="{Binding UnitCount}" Width="80"/>
                        <DataGridTextColumn Header="等级" Binding="{Binding Grade}" Width="60"/>
                        <DataGridTextColumn Header="当前工序" Binding="{Binding CurrentStep}" Width="120"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
            <TabItem Header="拆批记录">
                <DataGrid ItemsSource="{Binding SplitRecords}"
                          AutoGenerateColumns="False"
                          IsReadOnly="True"
                          Background="{StaticResource MesSurfaceBrush}"
                          BorderBrush="{StaticResource MesBorderBrush}"
                          HeadersVisibility="Column"
                          FontSize="13"
                          GridLinesVisibility="Horizontal"
                          CanUserAddRows="False"
                          CanUserDeleteRows="False">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="拆批ID" Binding="{Binding SplitId}" Width="120"/>
                        <DataGridTextColumn Header="母批次" Binding="{Binding MotherLotId}" Width="140"/>
                        <DataGridTextColumn Header="子批次" Binding="{Binding ChildLotId}" Width="140"/>
                        <DataGridTextColumn Header="数量" Binding="{Binding SplitQty}" Width="60"/>
                        <DataGridTextColumn Header="类型" Binding="{Binding SplitType}" Width="80"/>
                        <DataGridTextColumn Header="原因" Binding="{Binding SplitReason}" Width="*"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
            <TabItem Header="重工记录">
                <DataGrid ItemsSource="{Binding ReworkRecords}"
                          AutoGenerateColumns="False"
                          IsReadOnly="True"
                          Background="{StaticResource MesSurfaceBrush}"
                          BorderBrush="{StaticResource MesBorderBrush}"
                          HeadersVisibility="Column"
                          FontSize="13"
                          GridLinesVisibility="Horizontal"
                          CanUserAddRows="False"
                          CanUserDeleteRows="False">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="重工ID" Binding="{Binding ReworkId}" Width="120"/>
                        <DataGridTextColumn Header="批次" Binding="{Binding LotId}" Width="140"/>
                        <DataGridTextColumn Header="重工路线" Binding="{Binding ReworkRouteId}" Width="100"/>
                        <DataGridTextColumn Header="数量" Binding="{Binding ReworkQty}" Width="60"/>
                        <DataGridTextColumn Header="原因" Binding="{Binding ReworkReason}" Width="*"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
            <TabItem Header="报废记录">
                <DataGrid ItemsSource="{Binding ScrapRecords}"
                          AutoGenerateColumns="False"
                          IsReadOnly="True"
                          Background="{StaticResource MesSurfaceBrush}"
                          BorderBrush="{StaticResource MesBorderBrush}"
                          HeadersVisibility="Column"
                          FontSize="13"
                          GridLinesVisibility="Horizontal"
                          CanUserAddRows="False"
                          CanUserDeleteRows="False">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="报废ID" Binding="{Binding ScrapId}" Width="120"/>
                        <DataGridTextColumn Header="批次" Binding="{Binding LotId}" Width="140"/>
                        <DataGridTextColumn Header="数量" Binding="{Binding ScrapQty}" Width="60"/>
                        <DataGridTextColumn Header="原因" Binding="{Binding ScrapReason}" Width="*"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
        </TabControl>
    </Grid>
</UserControl>
```

- [ ] **Step 2：创建 GenealogyView.xaml.cs**

```csharp
using System.Windows.Controls;

namespace MES.Modules.Production.Views;

public partial class GenealogyView : UserControl
{
    public GenealogyView()
    {
        InitializeComponent();
    }
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 3-G：拆批/合批 UI

### Task 10：创建 LotSplitMergeViewModel

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/ViewModels/LotSplitMergeViewModel.cs`

- [ ] **Step 1：创建 ViewModel 文件**

```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class LotSplitMergeViewModel : BindableBase
{
    private readonly IRedisService _redis;
    private readonly ILotSplitMergeService _splitMergeService;

    private string _lotId = string.Empty;
    private int _splitQty;
    private string _splitReason = string.Empty;
    private string _splitType = "Partial";
    private string _mergeTargetLotId = string.Empty;
    private string _mergeReason = string.Empty;
    private bool _isLoading;
    private string _statusMessage = string.Empty;
    private LotInfo? _currentLot;

    public ObservableCollection<LotSplitRecord> SplitRecords { get; } = [];
    public ObservableCollection<LotMergeRecord> MergeRecords { get; } = [];
    public ObservableCollection<LotInfo> ChildLots { get; } = [];

    public string LotId
    {
        get => _lotId;
        set => SetProperty(ref _lotId, value);
    }

    public int SplitQty
    {
        get => _splitQty;
        set => SetProperty(ref _splitQty, value);
    }

    public string SplitReason
    {
        get => _splitReason;
        set => SetProperty(ref _splitReason, value);
    }

    public string SplitType
    {
        get => _splitType;
        set => SetProperty(ref _splitType, value);
    }

    public string MergeTargetLotId
    {
        get => _mergeTargetLotId;
        set => SetProperty(ref _mergeTargetLotId, value);
    }

    public string MergeReason
    {
        get => _mergeReason;
        set => SetProperty(ref _mergeReason, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public LotInfo? CurrentLot
    {
        get => _currentLot;
        set => SetProperty(ref _currentLot, value);
    }

    public ICommand QueryLotCommand { get; }
    public ICommand SplitCommand { get; }
    public ICommand MergeCommand { get; }
    public ICommand RefreshCommand { get; }

    public LotSplitMergeViewModel(IRedisService redis, ILotSplitMergeService splitMergeService)
    {
        _redis = redis;
        _splitMergeService = splitMergeService;

        QueryLotCommand = new DelegateCommand(OnQueryLot);
        SplitCommand = new DelegateCommand(OnSplit);
        MergeCommand = new DelegateCommand(OnMerge);
        RefreshCommand = new DelegateCommand(OnRefresh);
    }

    private async void OnQueryLot()
    {
        if (string.IsNullOrWhiteSpace(LotId)) return;

        IsLoading = true;
        try
        {
            var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{LotId}");
            if (lot is null)
            {
                StatusMessage = $"批次 {LotId} 不存在";
                CurrentLot = null;
                return;
            }

            CurrentLot = lot;
            SplitQty = lot.UnitCount / 2;

            SplitRecords.Clear();
            MergeRecords.Clear();
            ChildLots.Clear();

            var splits = await _splitMergeService.GetSplitRecordsAsync(LotId);
            foreach (var s in splits) SplitRecords.Add(s);

            var merges = await _splitMergeService.GetMergeRecordsAsync(LotId);
            foreach (var m in merges) MergeRecords.Add(m);

            var children = await _splitMergeService.GetChildLotsAsync(LotId);
            foreach (var c in children) ChildLots.Add(c);

            StatusMessage = $"批次 {LotId}: 数量 {lot.UnitCount}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async void OnSplit()
    {
        if (CurrentLot is null || SplitQty <= 0 || string.IsNullOrWhiteSpace(SplitReason))
            return;

        IsLoading = true;
        try
        {
            var record = await _splitMergeService.SplitLotAsync(
                LotId, SplitQty, SplitReason, SplitType, "OP-001", "操作员");

            StatusMessage = $"拆批成功: 子批次 {record.ChildLotId}";
            OnRefresh();
        }
        catch (Exception ex)
        {
            StatusMessage = $"拆批失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async void OnMerge()
    {
        if (string.IsNullOrWhiteSpace(MergeTargetLotId) || string.IsNullOrWhiteSpace(MergeReason))
            return;

        IsLoading = true;
        try
        {
            var sourceIds = new List<string> { LotId };
            var record = await _splitMergeService.MergeLotsAsync(
                MergeTargetLotId, sourceIds, MergeReason, "OP-001", "操作员");

            StatusMessage = $"合批成功: 合并 {record.MergedQty} 到 {MergeTargetLotId}";
            OnRefresh();
        }
        catch (Exception ex)
        {
            StatusMessage = $"合批失败: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void OnRefresh()
    {
        if (!string.IsNullOrWhiteSpace(LotId))
            OnQueryLot();
    }
}
```

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 11：创建 LotSplitMergeView

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Views/LotSplitMergeView.xaml`
- Create: `src/Client/Modules/MES.Modules.Production/Views/LotSplitMergeView.xaml.cs`

- [ ] **Step 1：创建 XAML 文件**

```xml
<UserControl x:Class="MES.Modules.Production.Views.LotSplitMergeView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:prism="http://prismlibrary.com/"
             prism:ViewModelLocator.AutoWireViewModel="True"
             Background="{StaticResource MesBackgroundBrush}">

    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- 搜索区 -->
        <Border Grid.Row="0"
                Background="{StaticResource MesCardBrush}"
                BorderBrush="{StaticResource MesBorderBrush}"
                BorderThickness="1"
                CornerRadius="4"
                Padding="16"
                Margin="0,0,0,16">
            <StackPanel>
                <StackPanel Orientation="Horizontal" Margin="0,0,0,8">
                    <TextBlock Text="批次号:" VerticalAlignment="Center" Margin="0,0,8,0"/>
                    <TextBox Text="{Binding LotId, UpdateSourceTrigger=PropertyChanged}"
                             Width="200" Margin="0,0,8,0"/>
                    <Button Content="查询" Command="{Binding QueryLotCommand}" Margin="0,0,8,0"/>
                    <Button Content="刷新" Command="{Binding RefreshCommand}"/>
                </StackPanel>
                <TextBlock Text="{Binding StatusMessage}"
                           Foreground="{StaticResource MesTextSecondaryBrush}"/>
            </StackPanel>
        </Border>

        <!-- 操作区 -->
        <Border Grid.Row="1"
                Background="{StaticResource MesCardBrush}"
                BorderBrush="{StaticResource MesBorderBrush}"
                BorderThickness="1"
                CornerRadius="4"
                Padding="16"
                Margin="0,0,0,16">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="20"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>

                <!-- 拆批 -->
                <StackPanel Grid.Column="0">
                    <TextBlock Text="拆批操作" FontSize="14" FontWeight="SemiBold" Margin="0,0,0,8"/>
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto"/>
                            <ColumnDefinition Width="*"/>
                        </Grid.ColumnDefinitions>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                        </Grid.RowDefinitions>

                        <TextBlock Grid.Row="0" Grid.Column="0" Text="数量:" Margin="0,0,8,4"/>
                        <TextBox Grid.Row="0" Grid.Column="1"
                                 Text="{Binding SplitQty, UpdateSourceTrigger=PropertyChanged}"
                                 Margin="0,0,0,4"/>

                        <TextBlock Grid.Row="1" Grid.Column="0" Text="原因:" Margin="0,0,8,4"/>
                        <TextBox Grid.Row="1" Grid.Column="1"
                                 Text="{Binding SplitReason, UpdateSourceTrigger=PropertyChanged}"
                                 Margin="0,0,0,4"/>

                        <TextBlock Grid.Row="2" Grid.Column="0" Text="类型:" Margin="0,0,8,4"/>
                        <ComboBox Grid.Row="2" Grid.Column="1"
                                  SelectedValue="{Binding SplitType}"
                                  SelectedValuePath="Content">
                            <ComboBoxItem Content="Partial"/>
                            <ComboBoxItem Content="Grade"/>
                            <ComboBoxItem Content="Rework"/>
                        </ComboBox>
                    </Grid>
                    <Button Content="执行拆批"
                            Command="{Binding SplitCommand}"
                            Margin="0,8,0,0"/>
                </StackPanel>

                <!-- 合批 -->
                <StackPanel Grid.Column="2">
                    <TextBlock Text="合批操作" FontSize="14" FontWeight="SemiBold" Margin="0,0,0,8"/>
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto"/>
                            <ColumnDefinition Width="*"/>
                        </Grid.ColumnDefinitions>
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                            <RowDefinition Height="Auto"/>
                        </Grid.RowDefinitions>

                        <TextBlock Grid.Row="0" Grid.Column="0" Text="目标批次:" Margin="0,0,8,4"/>
                        <TextBox Grid.Row="0" Grid.Column="1"
                                 Text="{Binding MergeTargetLotId, UpdateSourceTrigger=PropertyChanged}"
                                 Margin="0,0,0,4"/>

                        <TextBlock Grid.Row="1" Grid.Column="0" Text="原因:" Margin="0,0,8,4"/>
                        <TextBox Grid.Row="1" Grid.Column="1"
                                 Text="{Binding MergeReason, UpdateSourceTrigger=PropertyChanged}"
                                 Margin="0,0,0,4"/>
                    </Grid>
                    <Button Content="执行合批"
                            Command="{Binding MergeCommand}"
                            Margin="0,8,0,0"/>
                </StackPanel>
            </Grid>
        </Border>

        <!-- 记录区 -->
        <TabControl Grid.Row="2"
                    Background="{StaticResource MesCardBrush}"
                    BorderBrush="{StaticResource MesBorderBrush}"
                    BorderThickness="1">
            <TabItem Header="子批次">
                <DataGrid ItemsSource="{Binding ChildLots}"
                          AutoGenerateColumns="False"
                          IsReadOnly="True"
                          Background="{StaticResource MesSurfaceBrush}"
                          BorderBrush="{StaticResource MesBorderBrush}"
                          HeadersVisibility="Column"
                          FontSize="13"
                          GridLinesVisibility="Horizontal"
                          CanUserAddRows="False"
                          CanUserDeleteRows="False">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="批次号" Binding="{Binding LotId}" Width="180"/>
                        <DataGridTextColumn Header="状态" Binding="{Binding Status}" Width="80"/>
                        <DataGridTextColumn Header="数量" Binding="{Binding UnitCount}" Width="80"/>
                        <DataGridTextColumn Header="等级" Binding="{Binding Grade}" Width="60"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
            <TabItem Header="拆批记录">
                <DataGrid ItemsSource="{Binding SplitRecords}"
                          AutoGenerateColumns="False"
                          IsReadOnly="True"
                          Background="{StaticResource MesSurfaceBrush}"
                          BorderBrush="{StaticResource MesBorderBrush}"
                          HeadersVisibility="Column"
                          FontSize="13"
                          GridLinesVisibility="Horizontal"
                          CanUserAddRows="False"
                          CanUserDeleteRows="False">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="拆批ID" Binding="{Binding SplitId}" Width="120"/>
                        <DataGridTextColumn Header="子批次" Binding="{Binding ChildLotId}" Width="140"/>
                        <DataGridTextColumn Header="数量" Binding="{Binding SplitQty}" Width="60"/>
                        <DataGridTextColumn Header="类型" Binding="{Binding SplitType}" Width="80"/>
                        <DataGridTextColumn Header="原因" Binding="{Binding SplitReason}" Width="*"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
            <TabItem Header="合批记录">
                <DataGrid ItemsSource="{Binding MergeRecords}"
                          AutoGenerateColumns="False"
                          IsReadOnly="True"
                          Background="{StaticResource MesSurfaceBrush}"
                          BorderBrush="{StaticResource MesBorderBrush}"
                          HeadersVisibility="Column"
                          FontSize="13"
                          GridLinesVisibility="Horizontal"
                          CanUserAddRows="False"
                          CanUserDeleteRows="False">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="合批ID" Binding="{Binding MergeId}" Width="120"/>
                        <DataGridTextColumn Header="目标批次" Binding="{Binding TargetLotId}" Width="140"/>
                        <DataGridTextColumn Header="合并数量" Binding="{Binding MergedQty}" Width="80"/>
                        <DataGridTextColumn Header="原因" Binding="{Binding MergeReason}" Width="*"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
        </TabControl>
    </Grid>
</UserControl>
```

- [ ] **Step 2：创建 LotSplitMergeView.xaml.cs**

```csharp
using System.Windows.Controls;

namespace MES.Modules.Production.Views;

public partial class LotSplitMergeView : UserControl
{
    public LotSplitMergeView()
    {
        InitializeComponent();
    }
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 3-H：DI 注册 + Seed 数据

### Task 12：更新 ProductionModule 注册新服务

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/ProductionModule.cs`

- [ ] **Step 1：添加新服务注册**

在现有注册后添加：

```csharp
        // V3 新增服务
        containerRegistry.Register<ILotSplitMergeService, LotSplitMergeService>();
        containerRegistry.Register<ICarrierService, CarrierService>();
        containerRegistry.Register<IReworkService, ReworkService>();
        containerRegistry.Register<IScrapService, ScrapService>();
```

- [ ] **Step 2：注册新 View**

```csharp
        // V3 新增 View 注册
        containerRegistry.RegisterForNavigation<GenealogyView>("GenealogyView");
        containerRegistry.RegisterForNavigation<LotSplitMergeView>("LotSplitMergeView");
```

- [ ] **Step 3：添加 using 语句**

在文件顶部添加：

```csharp
using MES.Modules.Production.Views;
```

- [ ] **Step 4：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 13：重写 ProductionDataService Seed 数据

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/Services/ProductionDataService.cs`

- [ ] **Step 1：在 Seed 方法末尾添加 Genealogy 和 Carrier 数据**

在现有 Seed 逻辑完成后添加：

```csharp
        // ===== V3: Genealogy 追溯数据 =====
        var genealogyService = container.Resolve<IGenealogyService>();
        var splitMergeService = container.Resolve<ILotSplitMergeService>();
        var carrierService = container.Resolve<ICarrierService>();

        // 模拟 LOT-001 的载具绑定历史
        await carrierService.BindCarrierAsync("LOT-001", "TF-001", "TapeFrame", "SAW", 1, "OP-001");
        await carrierService.BindCarrierAsync("LOT-001", "MAG-001", "Magazine", "DA", 2, "OP-001");

        // 模拟 LOT-COMPLETE-001 的完整载具链
        var carrierChain = new[]
        {
            ("TF-100", "TapeFrame", "SAW", 1),
            ("MAG-100", "Magazine", "DA", 2),
            ("MAG-101", "Magazine", "CURE", 3),
            ("MAG-102", "Magazine", "WB", 4),
            ("MP-100", "MoldPlate", "MOLD", 5),
            ("OC-100", "OvenCart", "PMC", 6),
            ("MAG-103", "Magazine", "MARK", 7),
            ("ST-100", "SingTray", "SING", 8),
            ("TT-100", "TestTray", "FT", 9),
            ("TR-100", "Tray", "OQC", 10),
            ("RL-100", "Reel", "PACK", 11)
        };

        foreach (var (carrierId, carrierType, stepCode, stepSeq) in carrierChain)
        {
            await carrierService.BindCarrierAsync("LOT-COMPLETE-001", carrierId, carrierType, stepCode, stepSeq, "OP-001");
        }

        // 模拟 LOT-GRADE-001 的等级拆分
        var gradeSplitRecords = await splitMergeService.GradeSplitAsync(
            "LOT-GRADE-001",
            new Dictionary<string, int> { { "A", 8500 }, { "B", 1030 } },
            "OP-001", "操作员");

        // 模拟谱系记录
        await genealogyService.RecordRelationAsync(new LotGenealogy
        {
            ParentLotId = "LOT-001",
            ChildLotId = "LOT-001",
            RelationType = "WaferIn",
            StepCode = "SAW",
            StepSeq = 1,
            Qty = 10000,
            OperatorId = "OP-001",
            Remark = "晶圆进厂"
        });

        await genealogyService.RecordRelationAsync(new LotGenealogy
        {
            ParentLotId = "LOT-COMPLETE-001",
            ChildLotId = "LOT-COMPLETE-001",
            RelationType = "Completed",
            StepCode = "PACK",
            StepSeq = 11,
            Qty = 9500,
            OperatorId = "OP-001",
            Remark = "成品入库"
        });

        Console.WriteLine("[V3] Genealogy and Carrier seed data created");
```

- [ ] **Step 2：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 3-I：验收标准

### 验收清单

```
✅ ILotSplitMergeService 实现完整（SplitLot, GradeSplit, MergeLots）
✅ ICarrierService 实现完整（Bind, Unbind, Transfer, History）
✅ IReworkService 实现完整（CreateReworkLot, CompleteRework）
✅ IScrapService 实现完整（RecordScrap, RequiresApproval）
✅ TrackService 增强载具绑定和 Genealogy 记录
✅ GenealogyView 可查询谱系树、子批次、拆批/合批/重工/报废记录
✅ LotSplitMergeView 可执行拆批/合批操作
✅ ProductionModule 注册所有新服务和新 View
✅ Seed 数据包含完整载具链和 Genealogy 记录
✅ 编译 0 错误 0 警告
```

---

## 执行顺序建议

```
Phase 3-A (Task 1-2)  → 拆批/合批服务层
Phase 3-B (Task 3-4)  → 载具服务层
Phase 3-C (Task 5)    → 重工服务层
Phase 3-D (Task 6)    → 报废服务层
Phase 3-E (Task 7)    → TrackService 增强
Phase 3-F (Task 8-9)  → 谱系追溯 UI
Phase 3-G (Task 10-11)→ 拆批/合批 UI
Phase 3-H (Task 12-13)→ DI 注册 + Seed 数据
Phase 3-I             → 验收
```

每个 Phase 完成后都应运行 `dotnet build` 验证编译通过。

---

## 风险与缓解

| 风险 | 影响 | 缓解措施 |
|------|------|---------|
| GenealogyService 缺少 GetUpstreamAsync/GetDownstreamAsync | 谱系查询失败 | 在 Task 8 前先检查并补充接口 |
| LotCarrierBinding 模型字段不完整 | 载具绑定失败 | Task 3 前先检查模型 |
| XAML 资源引用错误 | UI 无法加载 | 使用与现有 View 相同的资源引用方式 |
| Seed 数据冲突 | 启动失败 | 使用 TryGet/Exists 检查后再创建 |
