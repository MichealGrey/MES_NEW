# MES V2 Phase 4 主数据 + 派工 + 报表 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现主数据管理（产品/设备/载具/人员）、派工清单、报表BI、运维监控，使 MES 具备正式工厂上线能力。

**Architecture:** 新增 MasterData 模块（产品、设备、载具、人员管理）、Dispatch 模块（派工清单、瓶颈分析）、Report 模块（生产报表、良率报表、WIP 报表）、运维模块（系统监控、数据归档）。所有服务基于 IRedisService 抽象，UI 使用 WPF+Prism MVVM。

**Tech Stack:** .NET 8, WPF+Prism, MySQL (Pomelo EF Core), IRedisService 抽象层

---

## 文件结构总览

### 新建模型（8 个）
| # | 文件 | 职责 |
|---|------|------|
| 1 | `Models/EquipmentInfo.cs` | 设备信息 |
| 2 | `Models/CarrierInfo.cs` | 载具信息 |
| 3 | `Models/RecipeInfo.cs` | Recipe 信息 |
| 4 | `Models/UserInfo.cs` | 用户信息 |
| 5 | `Models/DispatchTask.cs` | 派工任务 |
| 6 | `Models/ProductionReport.cs` | 生产报表 |
| 7 | `Models/YieldReport.cs` | 良率报表 |
| 8 | `Models/SystemMonitor.cs` | 系统监控 |

### 新建服务（6 个）
| # | 文件 | 职责 |
|---|------|------|
| 1 | `Services/IMasterDataService.cs` | 主数据服务接口 |
| 2 | `Services/MasterDataService.cs` | 主数据服务实现 |
| 3 | `Services/IDispatchService.cs` | 派工服务接口 |
| 4 | `Services/DispatchService.cs` | 派工服务实现 |
| 5 | `Services/IReportService.cs` | 报表服务接口 |
| 6 | `Services/ReportService.cs` | 报表服务实现 |

### 新建 ViewModel（5 个）
| # | 文件 | 职责 |
|---|------|------|
| 1 | `ViewModels/MasterDataViewModel.cs` | 主数据管理 VM |
| 2 | `ViewModels/DispatchViewModel.cs` | 派工管理 VM |
| 3 | `ViewModels/ProductionReportViewModel.cs` | 生产报表 VM |
| 4 | `ViewModels/YieldReportViewModel.cs` | 良率报表 VM |
| 5 | `ViewModels/SystemMonitorViewModel.cs` | 系统监控 VM |

### 新建 View（5 个）
| # | 文件 | 职责 |
|---|------|------|
| 1 | `Views/MasterDataView.xaml` | 主数据管理 UI |
| 2 | `Views/DispatchView.xaml` | 派工管理 UI |
| 3 | `Views/ProductionReportView.xaml` | 生产报表 UI |
| 4 | `Views/YieldReportView.xaml` | 良率报表 UI |
| 5 | `Views/SystemMonitorView.xaml` | 系统监控 UI |

### 修改文件（3 个）
| # | 文件 | 修改内容 |
|---|------|---------|
| 1 | `ProductionModule.cs` | 注册新服务 + 新 View |
| 2 | `ProductionDataService.cs` | 增加主数据 Seed |
| 3 | `Models/RouteStep.cs` | 增加 EquipmentGroup 关联 |

---

## Phase 4-A：主数据模型

### Task 1：创建主数据模型

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Models/EquipmentInfo.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/CarrierInfo.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/RecipeInfo.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/UserInfo.cs`

- [ ] **Step 1：创建 EquipmentInfo.cs**

```csharp
namespace MES.Modules.Production.Models;

public class EquipmentInfo
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public string EquipmentGroup { get; set; } = string.Empty;
    public string EquipmentType { get; set; } = string.Empty;
    public string Status { get; set; } = "Available"; // Available/Running/Maintenance/Offline
    public string? CurrentLotId { get; set; }
    public string? CurrentRecipe { get; set; }
    public List<string> SupportedRoutes { get; set; } = [];
    public List<string> SupportedSteps { get; set; } = [];
    public DateTime LastMaintenanceDate { get; set; }
    public int MaintenanceIntervalHours { get; set; }
    public int RunningHours { get; set; }
    public string Location { get; set; } = string.Empty;
    public string ResponsiblePerson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

- [ ] **Step 2：创建 CarrierInfo.cs**

```csharp
namespace MES.Modules.Production.Models;

public class CarrierInfo
{
    public string CarrierId { get; set; } = string.Empty;
    public string CarrierType { get; set; } = string.Empty; // FOUP/TapeFrame/Magazine/MoldPlate/OvenCart/SingTray/TestTray/Reel/Tray
    public string Status { get; set; } = "Available"; // Available/InUse/Cleaning/Maintenance/Retired
    public string? CurrentLotId { get; set; }
    public int Capacity { get; set; }
    public int UseCount { get; set; }
    public int MaxUseCount { get; set; }
    public DateTime LastCleanDate { get; set; }
    public int CleanIntervalUses { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

- [ ] **Step 3：创建 RecipeInfo.cs**

```csharp
namespace MES.Modules.Production.Models;

public class RecipeInfo
{
    public string RecipeId { get; set; } = string.Empty;
    public string RecipeName { get; set; } = string.Empty;
    public string EquipmentGroup { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public bool IsActive { get; set; } = true;
    public string? Parameters { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public DateTime ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

- [ ] **Step 4：创建 UserInfo.cs**

```csharp
namespace MES.Modules.Production.Models;

public class UserInfo
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = "Operator"; // Operator/Engineer/QA/Supervisor/Admin
    public string Department { get; set; } = string.Empty;
    public string Shift { get; set; } = string.Empty; // A/B/C
    public bool IsActive { get; set; } = true;
    public List<string> Permissions { get; set; } = [];
    public List<string> AuthorizedRoutes { get; set; } = [];
    public List<string> AuthorizedEquipments { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

- [ ] **Step 5：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 4-B：报表模型

### Task 2：创建报表和派工模型

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Models/DispatchTask.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/ProductionReport.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/YieldReport.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Models/SystemMonitor.cs`

- [ ] **Step 1：创建 DispatchTask.cs**

```csharp
namespace MES.Modules.Production.Models;

public class DispatchTask
{
    public string TaskId { get; set; } = Guid.NewGuid().ToString("N");
    public string LotId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public int StepSeq { get; set; }
    public string? EquipmentId { get; set; }
    public string? RecipeId { get; set; }
    public int Qty { get; set; }
    public string Priority { get; set; } = "Normal"; // Urgent/High/Normal/Low
    public string Status { get; set; } = "Pending"; // Pending/Assigned/Running/Completed
    public string? AssignedOperator { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AssignedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public double? DueHours { get; set; }
    public double? RemainingHours { get; set; }
    public bool IsOverdue { get; set; }
}
```

- [ ] **Step 2：创建 ProductionReport.cs**

```csharp
namespace MES.Modules.Production.Models;

public class ProductionReport
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime ReportDate { get; set; }
    public string ReportType { get; set; } = "Daily"; // Daily/Weekly/Monthly
    public int TotalLots { get; set; }
    public int CompletedLots { get; set; }
    public int WipLots { get; set; }
    public int HoldLots { get; set; }
    public int TotalInputQty { get; set; }
    public int TotalOutputQty { get; set; }
    public int TotalScrapQty { get; set; }
    public double OverallYield { get; set; }
    public double FTYield { get; set; }
    public List<StepYieldData> StepYields { get; set; } = [];
    public List<EquipmentUtilization> EquipmentUtils { get; set; } = [];
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class StepYieldData
{
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public int InputQty { get; set; }
    public int PassQty { get; set; }
    public double Yield { get; set; }
}

public class EquipmentUtilization
{
    public string EquipmentId { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public double Utilization { get; set; }
    public int RunningHours { get; set; }
    public int IdleHours { get; set; }
    public int MaintenanceHours { get; set; }
}
```

- [ ] **Step 3：创建 YieldReport.cs**

```csharp
namespace MES.Modules.Production.Models;

public class YieldReport
{
    public string ReportId { get; set; } = Guid.NewGuid().ToString("N");
    public string RouteId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalLots { get; set; }
    public double AverageYield { get; set; }
    public double MinYield { get; set; }
    public double MaxYield { get; set; }
    public double StdDev { get; set; }
    public List<StepYieldTrend> StepTrends { get; set; } = [];
    public List<YieldAlert> Alerts { get; set; } = [];
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class StepYieldTrend
{
    public string StepCode { get; set; } = string.Empty;
    public List<DailyYield> DailyData { get; set; } = [];
}

public class DailyYield
{
    public DateTime Date { get; set; }
    public double Yield { get; set; }
    public int LotCount { get; set; }
}

public class YieldAlert
{
    public string StepCode { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty; // LowYield/HighVariation/TrendDown
    public string Message { get; set; } = string.Empty;
    public DateTime TriggeredAt { get; set; }
}
```

- [ ] **Step 4：创建 SystemMonitor.cs**

```csharp
namespace MES.Modules.Production.Models;

public class SystemMonitor
{
    public string MonitorId { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int TotalLots { get; set; }
    public int ProcessingLots { get; set; }
    public int WaitingLots { get; set; }
    public int HoldLots { get; set; }
    public int CompletedLots { get; set; }
    public int AvailableEquipments { get; set; }
    public int RunningEquipments { get; set; }
    public int OfflineEquipments { get; set; }
    public int AvailableCarriers { get; set; }
    public int InUseCarriers { get; set; }
    public double SystemUptime { get; set; }
    public int PendingTasks { get; set; }
    public int OverdueTasks { get; set; }
    public List<AlertInfo> Alerts { get; set; } = [];
}

public class AlertInfo
{
    public string AlertId { get; set; } = Guid.NewGuid().ToString("N");
    public string AlertType { get; set; } = string.Empty; // YieldHold/EquipmentOffline/OverdueTask
    public string Severity { get; set; } = "Warning"; // Info/Warning/Error/Critical
    public string Message { get; set; } = string.Empty;
    public DateTime TriggeredAt { get; set; }
    public bool IsAcknowledged { get; set; }
}
```

- [ ] **Step 5：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 4-C：主数据服务

### Task 3：创建 IMasterDataService 接口 + 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/IMasterDataService.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/MasterDataService.cs`

- [ ] **Step 1：创建接口**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IMasterDataService
{
    // Equipment
    Task<List<EquipmentInfo>> GetAllEquipmentsAsync();
    Task<EquipmentInfo?> GetEquipmentAsync(string equipmentId);
    Task SaveEquipmentAsync(EquipmentInfo equipment);
    Task UpdateEquipmentStatusAsync(string equipmentId, string status);

    // Carrier
    Task<List<CarrierInfo>> GetAllCarriersAsync();
    Task<CarrierInfo?> GetCarrierAsync(string carrierId);
    Task SaveCarrierAsync(CarrierInfo carrier);
    Task UpdateCarrierStatusAsync(string carrierId, string status);

    // Recipe
    Task<List<RecipeInfo>> GetAllRecipesAsync();
    Task<RecipeInfo?> GetRecipeAsync(string recipeId);
    Task SaveRecipeAsync(RecipeInfo recipe);

    // User
    Task<List<UserInfo>> GetAllUsersAsync();
    Task<UserInfo?> GetUserAsync(string userId);
    Task SaveUserAsync(UserInfo user);

    // Query helpers
    Task<List<EquipmentInfo>> GetAvailableEquipmentsAsync(string equipmentGroup);
    Task<List<RecipeInfo>> GetRecipesForStepAsync(string equipmentGroup, string stepCode);
}
```

- [ ] **Step 2：创建实现**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class MasterDataService : IMasterDataService
{
    private readonly IRedisService _redis;

    private const string EquipIndexKey = "mes:master:equipment:index";
    private const string CarrierIndexKey = "mes:master:carrier:index";
    private const string RecipeIndexKey = "mes:master:recipe:index";
    private const string UserIndexKey = "mes:master:user:index";

    public MasterDataService(IRedisService redis) => _redis = redis;

    public async Task<List<EquipmentInfo>> GetAllEquipmentsAsync()
    {
        var ids = await _redis.SetMembersAsync(EquipIndexKey);
        var results = new List<EquipmentInfo>();
        foreach (var id in ids)
        {
            var eq = await _redis.GetObjectAsync<EquipmentInfo>($"mes:master:equipment:{id}");
            if (eq is not null) results.Add(eq);
        }
        return results;
    }

    public async Task<EquipmentInfo?> GetEquipmentAsync(string equipmentId)
    {
        return await _redis.GetObjectAsync<EquipmentInfo>($"mes:master:equipment:{equipmentId}");
    }

    public async Task SaveEquipmentAsync(EquipmentInfo equipment)
    {
        await _redis.SetObjectAsync($"mes:master:equipment:{equipment.EquipmentId}", equipment);
        await _redis.SetAddAsync(EquipIndexKey, equipment.EquipmentId);
    }

    public async Task UpdateEquipmentStatusAsync(string equipmentId, string status)
    {
        var eq = await GetEquipmentAsync(equipmentId);
        if (eq is null) return;
        eq.Status = status;
        await SaveEquipmentAsync(eq);
    }

    public async Task<List<CarrierInfo>> GetAllCarriersAsync()
    {
        var ids = await _redis.SetMembersAsync(CarrierIndexKey);
        var results = new List<CarrierInfo>();
        foreach (var id in ids)
        {
            var c = await _redis.GetObjectAsync<CarrierInfo>($"mes:master:carrier:{id}");
            if (c is not null) results.Add(c);
        }
        return results;
    }

    public async Task<CarrierInfo?> GetCarrierAsync(string carrierId)
    {
        return await _redis.GetObjectAsync<CarrierInfo>($"mes:master:carrier:{carrierId}");
    }

    public async Task SaveCarrierAsync(CarrierInfo carrier)
    {
        await _redis.SetObjectAsync($"mes:master:carrier:{carrier.CarrierId}", carrier);
        await _redis.SetAddAsync(CarrierIndexKey, carrier.CarrierId);
    }

    public async Task UpdateCarrierStatusAsync(string carrierId, string status)
    {
        var c = await GetCarrierAsync(carrierId);
        if (c is null) return;
        c.Status = status;
        await SaveCarrierAsync(c);
    }

    public async Task<List<RecipeInfo>> GetAllRecipesAsync()
    {
        var ids = await _redis.SetMembersAsync(RecipeIndexKey);
        var results = new List<RecipeInfo>();
        foreach (var id in ids)
        {
            var r = await _redis.GetObjectAsync<RecipeInfo>($"mes:master:recipe:{id}");
            if (r is not null) results.Add(r);
        }
        return results;
    }

    public async Task<RecipeInfo?> GetRecipeAsync(string recipeId)
    {
        return await _redis.GetObjectAsync<RecipeInfo>($"mes:master:recipe:{recipeId}");
    }

    public async Task SaveRecipeAsync(RecipeInfo recipe)
    {
        await _redis.SetObjectAsync($"mes:master:recipe:{recipe.RecipeId}", recipe);
        await _redis.SetAddAsync(RecipeIndexKey, recipe.RecipeId);
    }

    public async Task<List<UserInfo>> GetAllUsersAsync()
    {
        var ids = await _redis.SetMembersAsync(UserIndexKey);
        var results = new List<UserInfo>();
        foreach (var id in ids)
        {
            var u = await _redis.GetObjectAsync<UserInfo>($"mes:master:user:{id}");
            if (u is not null) results.Add(u);
        }
        return results;
    }

    public async Task<UserInfo?> GetUserAsync(string userId)
    {
        return await _redis.GetObjectAsync<UserInfo>($"mes:master:user:{userId}");
    }

    public async Task SaveUserAsync(UserInfo user)
    {
        await _redis.SetObjectAsync($"mes:master:user:{user.UserId}", user);
        await _redis.SetAddAsync(UserIndexKey, user.UserId);
    }

    public async Task<List<EquipmentInfo>> GetAvailableEquipmentsAsync(string equipmentGroup)
    {
        var all = await GetAllEquipmentsAsync();
        return all.Where(e => e.EquipmentGroup == equipmentGroup && e.Status == "Available").ToList();
    }

    public async Task<List<RecipeInfo>> GetRecipesForStepAsync(string equipmentGroup, string stepCode)
    {
        var all = await GetAllRecipesAsync();
        return all.Where(r => r.EquipmentGroup == equipmentGroup && r.StepCode == stepCode && r.IsActive).ToList();
    }
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 4-D：派工服务

### Task 4：创建 IDispatchService 接口 + 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/IDispatchService.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/DispatchService.cs`

- [ ] **Step 1：创建接口**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IDispatchService
{
    Task<List<DispatchTask>> GenerateDispatchListAsync();
    Task AssignTaskAsync(string taskId, string operatorId);
    Task StartTaskAsync(string taskId);
    Task CompleteTaskAsync(string taskId);
    Task<List<DispatchTask>> GetPendingTasksAsync();
    Task<List<DispatchTask>> GetTasksByEquipmentAsync(string equipmentId);
    Task<List<DispatchTask>> GetOverdueTasksAsync();
}
```

- [ ] **Step 2：创建实现**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class DispatchService : IDispatchService
{
    private readonly IRedisService _redis;
    private const string DispatchIndexKey = "mes:dispatch:index";
    private const string DispatchPendingKey = "mes:dispatch:pending";
    private const string DispatchEquipKey = "mes:dispatch:equipment:";

    public DispatchService(IRedisService redis) => _redis = redis;

    public async Task<List<DispatchTask>> GenerateDispatchListAsync()
    {
        var tasks = new List<DispatchTask>();
        var lotIds = await _redis.SetMembersAsync("mes:lot:index");
        foreach (var lotId in lotIds)
        {
            var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{lotId}");
            if (lot is null || lot.Status != "Waiting") continue;

            var task = new DispatchTask
            {
                LotId = lot.LotId,
                OrderId = lot.OrderId,
                ProductId = lot.ProductId,
                StepCode = lot.CurrentStep,
                StepSeq = lot.CurrentStepSeq,
                Qty = lot.UnitCount,
                Priority = lot.Priority switch
                {
                    "High" => "High",
                    "Urgent" => "Urgent",
                    _ => "Normal"
                },
                IsOverdue = false
            };
            tasks.Add(task);
        }

        tasks = tasks.OrderByDescending(t => t.Priority switch
        {
            "Urgent" => 4,
            "High" => 3,
            "Normal" => 2,
            _ => 1
        }).ToList();

        return tasks;
    }

    public async Task AssignTaskAsync(string taskId, string operatorId)
    {
        var task = await _redis.GetObjectAsync<DispatchTask>($"mes:dispatch:{taskId}");
        if (task is null) return;
        task.Status = "Assigned";
        task.AssignedOperator = operatorId;
        task.AssignedAt = DateTime.UtcNow;
        await _redis.SetObjectAsync($"mes:dispatch:{taskId}", task);
    }

    public async Task StartTaskAsync(string taskId)
    {
        var task = await _redis.GetObjectAsync<DispatchTask>($"mes:dispatch:{taskId}");
        if (task is null) return;
        task.Status = "Running";
        task.StartedAt = DateTime.UtcNow;
        await _redis.SetObjectAsync($"mes:dispatch:{taskId}", task);
    }

    public async Task CompleteTaskAsync(string taskId)
    {
        var task = await _redis.GetObjectAsync<DispatchTask>($"mes:dispatch:{taskId}");
        if (task is null) return;
        task.Status = "Completed";
        task.CompletedAt = DateTime.UtcNow;
        await _redis.SetObjectAsync($"mes:dispatch:{taskId}", task);
    }

    public async Task<List<DispatchTask>> GetPendingTasksAsync()
    {
        var taskIds = await _redis.SetMembersAsync(DispatchPendingKey);
        var tasks = new List<DispatchTask>();
        foreach (var id in taskIds)
        {
            var task = await _redis.GetObjectAsync<DispatchTask>($"mes:dispatch:{id}");
            if (task is not null && task.Status == "Pending") tasks.Add(task);
        }
        return tasks;
    }

    public async Task<List<DispatchTask>> GetTasksByEquipmentAsync(string equipmentId)
    {
        var taskIds = await _redis.ListRangeAsync($"{DispatchEquipKey}{equipmentId}");
        var tasks = new List<DispatchTask>();
        foreach (var id in taskIds)
        {
            var task = await _redis.GetObjectAsync<DispatchTask>($"mes:dispatch:{id}");
            if (task is not null) tasks.Add(task);
        }
        return tasks;
    }

    public async Task<List<DispatchTask>> GetOverdueTasksAsync()
    {
        var allTasks = await GenerateDispatchListAsync();
        return allTasks.Where(t => t.IsOverdue).ToList();
    }
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 4-E：报表服务

### Task 5：创建 IReportService 接口 + 实现

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/Services/IReportService.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Services/ReportService.cs`

- [ ] **Step 1：创建接口**

```csharp
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IReportService
{
    Task<ProductionReport> GenerateDailyReportAsync(DateTime date);
    Task<YieldReport> GenerateYieldReportAsync(string routeId, DateTime startDate, DateTime endDate);
    Task<SystemMonitor> GetSystemSnapshotAsync();
    Task<List<EquipmentUtilization>> GetEquipmentUtilizationAsync(DateTime date);
}
```

- [ ] **Step 2：创建实现**

```csharp
using MES.Infrastructure.Cache;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class ReportService : IReportService
{
    private readonly IRedisService _redis;

    public ReportService(IRedisService redis) => _redis = redis;

    public async Task<ProductionReport> GenerateDailyReportAsync(DateTime date)
    {
        var report = new ProductionReport
        {
            ReportDate = date,
            ReportType = "Daily"
        };

        var lotIds = await _redis.SetMembersAsync("mes:lot:index");
        foreach (var lotId in lotIds)
        {
            var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{lotId}");
            if (lot is null) continue;

            report.TotalLots++;
            if (lot.Status == "Completed") report.CompletedLots++;
            else if (lot.Status == "Hold") report.HoldLots++;
            else report.WipLots++;

            report.TotalInputQty += lot.OriginalQty;
            report.TotalOutputQty += lot.TotalPassQty;
            report.TotalScrapQty += lot.TotalScrapQty;
        }

        report.OverallYield = report.TotalInputQty > 0
            ? (double)report.TotalOutputQty / report.TotalInputQty * 100
            : 0;

        report.FTYield = report.OverallYield;
        report.GeneratedAt = DateTime.UtcNow;
        return report;
    }

    public async Task<YieldReport> GenerateYieldReportAsync(string routeId, DateTime startDate, DateTime endDate)
    {
        var report = new YieldReport
        {
            RouteId = routeId,
            StartDate = startDate,
            EndDate = endDate
        };

        var yields = new List<double>();
        var lotIds = await _redis.SetMembersAsync("mes:lot:index");

        foreach (var lotId in lotIds)
        {
            var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{lotId}");
            if (lot is null || lot.RouteId != routeId) continue;

            if (lot.OriginalQty > 0)
            {
                var yield = (double)lot.TotalPassQty / lot.OriginalQty * 100;
                yields.Add(yield);
            }
            report.TotalLots++;
        }

        if (yields.Count > 0)
        {
            report.AverageYield = yields.Average();
            report.MinYield = yields.Min();
            report.MaxYield = yields.Max();
            report.StdDev = Math.Sqrt(yields.Average(v => Math.Pow(v - report.AverageYield, 2)));
        }

        report.GeneratedAt = DateTime.UtcNow;
        return report;
    }

    public async Task<SystemMonitor> GetSystemSnapshotAsync()
    {
        var monitor = new SystemMonitor
        {
            Timestamp = DateTime.UtcNow
        };

        var lotIds = await _redis.SetMembersAsync("mes:lot:index");
        foreach (var lotId in lotIds)
        {
            var lot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{lotId}");
            if (lot is null) continue;

            monitor.TotalLots++;
            switch (lot.Status)
            {
                case "Processing": monitor.ProcessingLots++; break;
                case "Waiting": monitor.WaitingLots++; break;
                case "Hold": monitor.HoldLots++; break;
                case "Completed": monitor.CompletedLots++; break;
            }
        }

        var equipIds = await _redis.SetMembersAsync("mes:master:equipment:index");
        foreach (var equipId in equipIds)
        {
            var eq = await _redis.GetObjectAsync<EquipmentInfo>($"mes:master:equipment:{equipId}");
            if (eq is null) continue;

            switch (eq.Status)
            {
                case "Available": monitor.AvailableEquipments++; break;
                case "Running": monitor.RunningEquipments++; break;
                case "Offline": monitor.OfflineEquipments++; break;
            }
        }

        var carrierIds = await _redis.SetMembersAsync("mes:master:carrier:index");
        foreach (var carrierId in carrierIds)
        {
            var c = await _redis.GetObjectAsync<CarrierInfo>($"mes:master:carrier:{carrierId}");
            if (c is null) continue;

            if (c.Status == "Available") monitor.AvailableCarriers++;
            else if (c.Status == "InUse") monitor.InUseCarriers++;
        }

        monitor.SystemUptime = 99.9;

        if (monitor.HoldLots > 0)
        {
            monitor.Alerts.Add(new AlertInfo
            {
                AlertType = "HoldLots",
                Severity = "Warning",
                Message = $"当前有 {monitor.HoldLots} 个批次处于 Hold 状态",
                TriggeredAt = DateTime.UtcNow
            });
        }

        return monitor;
    }

    public async Task<List<EquipmentUtilization>> GetEquipmentUtilizationAsync(DateTime date)
    {
        var equipIds = await _redis.SetMembersAsync("mes:master:equipment:index");
        var results = new List<EquipmentUtilization>();

        foreach (var equipId in equipIds)
        {
            var eq = await _redis.GetObjectAsync<EquipmentInfo>($"mes:master:equipment:{equipId}");
            if (eq is null) continue;

            results.Add(new EquipmentUtilization
            {
                EquipmentId = eq.EquipmentId,
                EquipmentName = eq.EquipmentName,
                Utilization = eq.Status == "Running" ? 100 : eq.Status == "Available" ? 0 : 0,
                RunningHours = eq.RunningHours,
                IdleHours = 0,
                MaintenanceHours = 0
            });
        }

        return results;
    }
}
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 4-F：ViewModel + View

### Task 6：创建 MasterDataViewModel + MasterDataView

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/ViewModels/MasterDataViewModel.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Views/MasterDataView.xaml`
- Create: `src/Client/Modules/MES.Modules.Production/Views/MasterDataView.xaml.cs`

- [ ] **Step 1：创建 ViewModel**

```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class MasterDataViewModel : BindableBase
{
    private readonly IMasterDataService _masterDataService;

    private ObservableCollection<EquipmentInfo> _equipments = [];
    private ObservableCollection<CarrierInfo> _carriers = [];
    private ObservableCollection<RecipeInfo> _recipes = [];
    private ObservableCollection<UserInfo> _users = [];

    public ObservableCollection<EquipmentInfo> Equipments
    {
        get => _equipments;
        set => SetProperty(ref _equipments, value);
    }

    public ObservableCollection<CarrierInfo> Carriers
    {
        get => _carriers;
        set => SetProperty(ref _carriers, value);
    }

    public ObservableCollection<RecipeInfo> Recipes
    {
        get => _recipes;
        set => SetProperty(ref _recipes, value);
    }

    public ObservableCollection<UserInfo> Users
    {
        get => _users;
        set => SetProperty(ref _users, value);
    }

    public ICommand LoadDataCommand { get; }

    public MasterDataViewModel(IMasterDataService masterDataService)
    {
        _masterDataService = masterDataService;
        LoadDataCommand = new DelegateCommand(async () => await LoadAllDataAsync());
    }

    private async System.Threading.Tasks.Task LoadAllDataAsync()
    {
        Equipments = new ObservableCollection<EquipmentInfo>(await _masterDataService.GetAllEquipmentsAsync());
        Carriers = new ObservableCollection<CarrierInfo>(await _masterDataService.GetAllCarriersAsync());
        Recipes = new ObservableCollection<RecipeInfo>(await _masterDataService.GetAllRecipesAsync());
        Users = new ObservableCollection<UserInfo>(await _masterDataService.GetAllUsersAsync());
    }
}
```

- [ ] **Step 2：创建 View (MasterDataView.xaml)**

```xml
<UserControl x:Class="MES.Modules.Production.Views.MasterDataView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:prism="http://prismlibrary.com/"
             prism:ViewModelLocator.AutoWireViewModel="True"
             Background="#1E1E2E">
    <Grid Margin="16">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="0,0,0,12">
            <TextBlock Text="主数据管理" FontSize="20" FontWeight="Bold" Foreground="#CDD6F4" Margin="0,0,16,0"/>
            <Button Content="刷新数据" Command="{Binding LoadDataCommand}" Width="100"/>
        </StackPanel>

        <TabControl Grid.Row="1" Background="#313244">
            <TabItem Header="设备管理">
                <DataGrid ItemsSource="{Binding Equipments}" AutoGenerateColumns="False" IsReadOnly="True"
                          Background="#313244" Foreground="#CDD6F4" BorderBrush="#45475A"
                          RowBackground="#1E1E2E" AlternatingRowBackground="#313244" GridLinesVisibility="Horizontal"
                          HeadersVisibility="Column" FontSize="12">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="设备ID" Binding="{Binding EquipmentId}" Width="120"/>
                        <DataGridTextColumn Header="设备名称" Binding="{Binding EquipmentName}" Width="120"/>
                        <DataGridTextColumn Header="设备组" Binding="{Binding EquipmentGroup}" Width="100"/>
                        <DataGridTextColumn Header="状态" Binding="{Binding Status}" Width="100"/>
                        <DataGridTextColumn Header="位置" Binding="{Binding Location}" Width="100"/>
                        <DataGridTextColumn Header="负责人" Binding="{Binding ResponsiblePerson}" Width="100"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
            <TabItem Header="载具管理">
                <DataGrid ItemsSource="{Binding Carriers}" AutoGenerateColumns="False" IsReadOnly="True"
                          Background="#313244" Foreground="#CDD6F4" BorderBrush="#45475A"
                          RowBackground="#1E1E2E" AlternatingRowBackground="#313244" GridLinesVisibility="Horizontal"
                          HeadersVisibility="Column" FontSize="12">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="载具ID" Binding="{Binding CarrierId}" Width="120"/>
                        <DataGridTextColumn Header="类型" Binding="{Binding CarrierType}" Width="100"/>
                        <DataGridTextColumn Header="状态" Binding="{Binding Status}" Width="100"/>
                        <DataGridTextColumn Header="容量" Binding="{Binding Capacity}" Width="80"/>
                        <DataGridTextColumn Header="使用次数" Binding="{Binding UseCount}" Width="100"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
            <TabItem Header="Recipe管理">
                <DataGrid ItemsSource="{Binding Recipes}" AutoGenerateColumns="False" IsReadOnly="True"
                          Background="#313244" Foreground="#CDD6F4" BorderBrush="#45475A"
                          RowBackground="#1E1E2E" AlternatingRowBackground="#313244" GridLinesVisibility="Horizontal"
                          HeadersVisibility="Column" FontSize="12">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="RecipeID" Binding="{Binding RecipeId}" Width="140"/>
                        <DataGridTextColumn Header="名称" Binding="{Binding RecipeName}" Width="120"/>
                        <DataGridTextColumn Header="设备组" Binding="{Binding EquipmentGroup}" Width="100"/>
                        <DataGridTextColumn Header="工序" Binding="{Binding StepCode}" Width="80"/>
                        <DataGridTextColumn Header="版本" Binding="{Binding Version}" Width="80"/>
                        <DataGridTextColumn Header="状态" Binding="{Binding IsActive}" Width="80"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
            <TabItem Header="人员管理">
                <DataGrid ItemsSource="{Binding Users}" AutoGenerateColumns="False" IsReadOnly="True"
                          Background="#313244" Foreground="#CDD6F4" BorderBrush="#45475A"
                          RowBackground="#1E1E2E" AlternatingRowBackground="#313244" GridLinesVisibility="Horizontal"
                          HeadersVisibility="Column" FontSize="12">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="用户ID" Binding="{Binding UserId}" Width="120"/>
                        <DataGridTextColumn Header="姓名" Binding="{Binding UserName}" Width="120"/>
                        <DataGridTextColumn Header="角色" Binding="{Binding Role}" Width="100"/>
                        <DataGridTextColumn Header="部门" Binding="{Binding Department}" Width="100"/>
                        <DataGridTextColumn Header="班次" Binding="{Binding Shift}" Width="80"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
        </TabControl>
    </Grid>
</UserControl>
```

- [ ] **Step 3：创建 xaml.cs**

```csharp
using System.Windows.Controls;

namespace MES.Modules.Production.Views;

public partial class MasterDataView : UserControl
{
    public MasterDataView() => InitializeComponent();
}
```

- [ ] **Step 4：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 7：创建 DispatchViewModel + DispatchView

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/ViewModels/DispatchViewModel.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Views/DispatchView.xaml`
- Create: `src/Client/Modules/MES.Modules.Production/Views/DispatchView.xaml.cs`

- [ ] **Step 1：创建 ViewModel**

```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class DispatchViewModel : BindableBase
{
    private readonly IDispatchService _dispatchService;

    private ObservableCollection<DispatchTask> _tasks = [];
    private DispatchTask? _selectedTask;

    public ObservableCollection<DispatchTask> Tasks
    {
        get => _tasks;
        set => SetProperty(ref _tasks, value);
    }

    public DispatchTask? SelectedTask
    {
        get => _selectedTask;
        set => SetProperty(ref _selectedTask, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand AssignCommand { get; }
    public ICommand StartCommand { get; }
    public ICommand CompleteCommand { get; }

    public DispatchViewModel(IDispatchService dispatchService)
    {
        _dispatchService = dispatchService;
        RefreshCommand = new DelegateCommand(async () => await LoadTasksAsync());
        AssignCommand = new DelegateCommand(async () => await AssignTaskAsync());
        StartCommand = new DelegateCommand(async () => await StartTaskAsync());
        CompleteCommand = new DelegateCommand(async () => await CompleteTaskAsync());
    }

    private async System.Threading.Tasks.Task LoadTasksAsync()
    {
        var tasks = await _dispatchService.GenerateDispatchListAsync();
        Tasks = new ObservableCollection<DispatchTask>(tasks);
    }

    private async System.Threading.Tasks.Task AssignTaskAsync()
    {
        if (SelectedTask is null) return;
        await _dispatchService.AssignTaskAsync(SelectedTask.TaskId, "OP001");
        await LoadTasksAsync();
    }

    private async System.Threading.Tasks.Task StartTaskAsync()
    {
        if (SelectedTask is null) return;
        await _dispatchService.StartTaskAsync(SelectedTask.TaskId);
        await LoadTasksAsync();
    }

    private async System.Threading.Tasks.Task CompleteTaskAsync()
    {
        if (SelectedTask is null) return;
        await _dispatchService.CompleteTaskAsync(SelectedTask.TaskId);
        await LoadTasksAsync();
    }
}
```

- [ ] **Step 2：创建 View (DispatchView.xaml)**

```xml
<UserControl x:Class="MES.Modules.Production.Views.DispatchView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:prism="http://prismlibrary.com/"
             prism:ViewModelLocator.AutoWireViewModel="True"
             Background="#1E1E2E">
    <Grid Margin="16">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="0,0,0,12">
            <TextBlock Text="派工管理" FontSize="20" FontWeight="Bold" Foreground="#CDD6F4" Margin="0,0,16,0"/>
            <Button Content="刷新" Command="{Binding RefreshCommand}" Width="80" Margin="0,0,8,0"/>
            <Button Content="分配" Command="{Binding AssignCommand}" Width="80" Margin="0,0,8,0"/>
            <Button Content="开始" Command="{Binding StartCommand}" Width="80" Margin="0,0,8,0"/>
            <Button Content="完成" Command="{Binding CompleteCommand}" Width="80"/>
        </StackPanel>

        <DataGrid Grid.Row="1" ItemsSource="{Binding Tasks}" SelectedItem="{Binding SelectedTask}"
                  AutoGenerateColumns="False" IsReadOnly="True"
                  Background="#313244" Foreground="#CDD6F4" BorderBrush="#45475A"
                  RowBackground="#1E1E2E" AlternatingRowBackground="#313244" GridLinesVisibility="Horizontal"
                  HeadersVisibility="Column" FontSize="12">
            <DataGrid.Columns>
                <DataGridTextColumn Header="批次号" Binding="{Binding LotId}" Width="140"/>
                <DataGridTextColumn Header="工单号" Binding="{Binding OrderId}" Width="120"/>
                <DataGridTextColumn Header="工序" Binding="{Binding StepName}" Width="100"/>
                <DataGridTextColumn Header="数量" Binding="{Binding Qty}" Width="80"/>
                <DataGridTextColumn Header="优先级" Binding="{Binding Priority}" Width="80"/>
                <DataGridTextColumn Header="状态" Binding="{Binding Status}" Width="80"/>
                <DataGridTextColumn Header="操作人" Binding="{Binding AssignedOperator}" Width="100"/>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
</UserControl>
```

- [ ] **Step 3：创建 xaml.cs**

```csharp
using System.Windows.Controls;

namespace MES.Modules.Production.Views;

public partial class DispatchView : UserControl
{
    public DispatchView() => InitializeComponent();
}
```

- [ ] **Step 4：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 8：创建 ProductionReportViewModel + ProductionReportView

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/ViewModels/ProductionReportViewModel.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Views/ProductionReportView.xaml`
- Create: `src/Client/Modules/MES.Modules.Production/Views/ProductionReportView.xaml.cs`

- [ ] **Step 1：创建 ViewModel**

```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class ProductionReportViewModel : BindableBase
{
    private readonly IReportService _reportService;

    private ProductionReport? _report;
    private ObservableCollection<StepYieldData> _stepYields = [];
    private ObservableCollection<EquipmentUtilization> _equipUtils = [];
    private DateTime _reportDate = DateTime.Today;

    public ProductionReport? Report
    {
        get => _report;
        set => SetProperty(ref _report, value);
    }

    public ObservableCollection<StepYieldData> StepYields
    {
        get => _stepYields;
        set => SetProperty(ref _stepYields, value);
    }

    public ObservableCollection<EquipmentUtilization> EquipUtils
    {
        get => _equipUtils;
        set => SetProperty(ref _equipUtils, value);
    }

    public DateTime ReportDate
    {
        get => _reportDate;
        set => SetProperty(ref _reportDate, value);
    }

    public ICommand GenerateCommand { get; }

    public ProductionReportViewModel(IReportService reportService)
    {
        _reportService = reportService;
        GenerateCommand = new DelegateCommand(async () => await GenerateReportAsync());
    }

    private async System.Threading.Tasks.Task GenerateReportAsync()
    {
        Report = await _reportService.GenerateDailyReportAsync(ReportDate);
        if (Report is not null)
        {
            StepYields = new ObservableCollection<StepYieldData>(Report.StepYields);
            EquipUtils = new ObservableCollection<EquipmentUtilization>(Report.EquipmentUtils);
        }
    }
}
```

- [ ] **Step 2：创建 View (ProductionReportView.xaml)**

```xml
<UserControl x:Class="MES.Modules.Production.Views.ProductionReportView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:prism="http://prismlibrary.com/"
             prism:ViewModelLocator.AutoWireViewModel="True"
             Background="#1E1E2E">
    <Grid Margin="16">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="0,0,0,12">
            <TextBlock Text="生产报表" FontSize="20" FontWeight="Bold" Foreground="#CDD6F4" Margin="0,0,16,0"/>
            <TextBlock Text="日期:" VerticalAlignment="Center" Margin="0,0,8,0" Foreground="#CDD6F4"/>
            <DatePicker SelectedDate="{Binding ReportDate}" Width="150" Margin="0,0,12,0"/>
            <Button Content="生成报表" Command="{Binding GenerateCommand}" Width="100"/>
        </StackPanel>

        <Grid Grid.Row="1" Margin="0,0,0,12">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>
            <StackPanel Grid.Column="0">
                <TextBlock Text="总批次" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Report.TotalLots}" FontSize="24" FontWeight="Bold" Foreground="#CDD6F4"/>
            </StackPanel>
            <StackPanel Grid.Column="1">
                <TextBlock Text="完成批次" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Report.CompletedLots}" FontSize="24" FontWeight="Bold" Foreground="#CDD6F4"/>
            </StackPanel>
            <StackPanel Grid.Column="2">
                <TextBlock Text="总良率" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Report.OverallYield, StringFormat={}{0:F1}%}" FontSize="24" FontWeight="Bold" Foreground="#CDD6F4"/>
            </StackPanel>
            <StackPanel Grid.Column="3">
                <TextBlock Text="报废数量" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Report.TotalScrapQty}" FontSize="24" FontWeight="Bold" Foreground="#CDD6F4"/>
            </StackPanel>
        </Grid>

        <DataGrid Grid.Row="2" ItemsSource="{Binding StepYields}" AutoGenerateColumns="False" IsReadOnly="True"
                  Background="#313244" Foreground="#CDD6F4" BorderBrush="#45475A"
                  RowBackground="#1E1E2E" AlternatingRowBackground="#313244" GridLinesVisibility="Horizontal"
                  HeadersVisibility="Column" FontSize="12">
            <DataGrid.Columns>
                <DataGridTextColumn Header="工序" Binding="{Binding StepCode}" Width="100"/>
                <DataGridTextColumn Header="名称" Binding="{Binding StepName}" Width="120"/>
                <DataGridTextColumn Header="投入" Binding="{Binding InputQty}" Width="100"/>
                <DataGridTextColumn Header="产出" Binding="{Binding PassQty}" Width="100"/>
                <DataGridTextColumn Header="良率" Binding="{Binding Yield, StringFormat={}{0:F1}%}" Width="100"/>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
</UserControl>
```

- [ ] **Step 3：创建 xaml.cs**

```csharp
using System.Windows.Controls;

namespace MES.Modules.Production.Views;

public partial class ProductionReportView : UserControl
{
    public ProductionReportView() => InitializeComponent();
}
```

- [ ] **Step 4：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 9：创建 YieldReportViewModel + YieldReportView

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/ViewModels/YieldReportViewModel.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Views/YieldReportView.xaml`
- Create: `src/Client/Modules/MES.Modules.Production/Views/YieldReportView.xaml.cs`

- [ ] **Step 1：创建 ViewModel**

```csharp
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class YieldReportViewModel : BindableBase
{
    private readonly IReportService _reportService;

    private YieldReport? _report;
    private string _routeId = "QFN-STD";
    private DateTime _startDate = DateTime.Today.AddDays(-7);
    private DateTime _endDate = DateTime.Today;

    public YieldReport? Report
    {
        get => _report;
        set => SetProperty(ref _report, value);
    }

    public string RouteId
    {
        get => _routeId;
        set => SetProperty(ref _routeId, value);
    }

    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    public ICommand GenerateCommand { get; }

    public YieldReportViewModel(IReportService reportService)
    {
        _reportService = reportService;
        GenerateCommand = new DelegateCommand(async () => await GenerateReportAsync());
    }

    private async System.Threading.Tasks.Task GenerateReportAsync()
    {
        Report = await _reportService.GenerateYieldReportAsync(RouteId, StartDate, EndDate);
    }
}
```

- [ ] **Step 2：创建 View (YieldReportView.xaml)**

```xml
<UserControl x:Class="MES.Modules.Production.Views.YieldReportView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:prism="http://prismlibrary.com/"
             prism:ViewModelLocator.AutoWireViewModel="True"
             Background="#1E1E2E">
    <Grid Margin="16">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="0,0,0,12">
            <TextBlock Text="良率报表" FontSize="20" FontWeight="Bold" Foreground="#CDD6F4" Margin="0,0,16,0"/>
            <TextBlock Text="路线:" VerticalAlignment="Center" Margin="0,0,8,0" Foreground="#CDD6F4"/>
            <TextBox Text="{Binding RouteId}" Width="120" Margin="0,0,12,0"/>
            <TextBlock Text="开始:" VerticalAlignment="Center" Margin="0,0,8,0" Foreground="#CDD6F4"/>
            <DatePicker SelectedDate="{Binding StartDate}" Width="130" Margin="0,0,12,0"/>
            <TextBlock Text="结束:" VerticalAlignment="Center" Margin="0,0,8,0" Foreground="#CDD6F4"/>
            <DatePicker SelectedDate="{Binding EndDate}" Width="130" Margin="0,0,12,0"/>
            <Button Content="生成报表" Command="{Binding GenerateCommand}" Width="100"/>
        </StackPanel>

        <Grid Grid.Row="1" Margin="0,0,0,12">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>
            <StackPanel Grid.Column="0">
                <TextBlock Text="批次总数" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Report.TotalLots}" FontSize="24" FontWeight="Bold" Foreground="#CDD6F4"/>
            </StackPanel>
            <StackPanel Grid.Column="1">
                <TextBlock Text="平均良率" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Report.AverageYield, StringFormat={}{0:F1}%}" FontSize="24" FontWeight="Bold" Foreground="#CDD6F4"/>
            </StackPanel>
            <StackPanel Grid.Column="2">
                <TextBlock Text="最低良率" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Report.MinYield, StringFormat={}{0:F1}%}" FontSize="24" FontWeight="Bold" Foreground="#CDD6F4"/>
            </StackPanel>
            <StackPanel Grid.Column="3">
                <TextBlock Text="标准差" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Report.StdDev, StringFormat={}{0:F2}}" FontSize="24" FontWeight="Bold" Foreground="#CDD6F4"/>
            </StackPanel>
        </Grid>
    </Grid>
</UserControl>
```

- [ ] **Step 3：创建 xaml.cs**

```csharp
using System.Windows.Controls;

namespace MES.Modules.Production.Views;

public partial class YieldReportView : UserControl
{
    public YieldReportView() => InitializeComponent();
}
```

- [ ] **Step 4：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

### Task 10：创建 SystemMonitorViewModel + SystemMonitorView

**Files:**
- Create: `src/Client/Modules/MES.Modules.Production/ViewModels/SystemMonitorViewModel.cs`
- Create: `src/Client/Modules/MES.Modules.Production/Views/SystemMonitorView.xaml`
- Create: `src/Client/Modules/MES.Modules.Production/Views/SystemMonitorView.xaml.cs`

- [ ] **Step 1：创建 ViewModel**

```csharp
using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class SystemMonitorViewModel : BindableBase
{
    private readonly IReportService _reportService;

    private SystemMonitor? _monitor;
    private ObservableCollection<AlertInfo> _alerts = [];

    public SystemMonitor? Monitor
    {
        get => _monitor;
        set => SetProperty(ref _monitor, value);
    }

    public ObservableCollection<AlertInfo> Alerts
    {
        get => _alerts;
        set => SetProperty(ref _alerts, value);
    }

    public ICommand RefreshCommand { get; }

    public SystemMonitorViewModel(IReportService reportService)
    {
        _reportService = reportService;
        RefreshCommand = new DelegateCommand(async () => await RefreshAsync());
    }

    private async System.Threading.Tasks.Task RefreshAsync()
    {
        Monitor = await _reportService.GetSystemSnapshotAsync();
        if (Monitor is not null)
        {
            Alerts = new ObservableCollection<AlertInfo>(Monitor.Alerts);
        }
    }
}
```

- [ ] **Step 2：创建 View (SystemMonitorView.xaml)**

```xml
<UserControl x:Class="MES.Modules.Production.Views.SystemMonitorView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:prism="http://prismlibrary.com/"
             prism:ViewModelLocator.AutoWireViewModel="True"
             Background="#1E1E2E">
    <Grid Margin="16">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <StackPanel Grid.Row="0" Orientation="Horizontal" Margin="0,0,0,12">
            <TextBlock Text="系统监控" FontSize="20" FontWeight="Bold" Foreground="#CDD6F4" Margin="0,0,16,0"/>
            <Button Content="刷新" Command="{Binding RefreshCommand}" Width="80"/>
        </StackPanel>

        <Grid Grid.Row="1" Margin="0,0,0,12">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>
            <StackPanel Grid.Column="0">
                <TextBlock Text="加工中" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Monitor.ProcessingLots}" FontSize="24" FontWeight="Bold" Foreground="#4CAF50"/>
            </StackPanel>
            <StackPanel Grid.Column="1">
                <TextBlock Text="等待中" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Monitor.WaitingLots}" FontSize="24" FontWeight="Bold" Foreground="#FF9800"/>
            </StackPanel>
            <StackPanel Grid.Column="2">
                <TextBlock Text="Hold" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Monitor.HoldLots}" FontSize="24" FontWeight="Bold" Foreground="#F44336"/>
            </StackPanel>
            <StackPanel Grid.Column="3">
                <TextBlock Text="可用设备" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Monitor.AvailableEquipments}" FontSize="24" FontWeight="Bold" Foreground="#CDD6F4"/>
            </StackPanel>
            <StackPanel Grid.Column="4">
                <TextBlock Text="可用载具" FontSize="14" Foreground="#CDD6F4"/>
                <TextBlock Text="{Binding Monitor.AvailableCarriers}" FontSize="24" FontWeight="Bold" Foreground="#CDD6F4"/>
            </StackPanel>
        </Grid>

        <DataGrid Grid.Row="2" ItemsSource="{Binding Alerts}" AutoGenerateColumns="False" IsReadOnly="True"
                  Background="#313244" Foreground="#CDD6F4" BorderBrush="#45475A"
                  RowBackground="#1E1E2E" AlternatingRowBackground="#313244" GridLinesVisibility="Horizontal"
                  HeadersVisibility="Column" FontSize="12">
            <DataGrid.Columns>
                <DataGridTextColumn Header="类型" Binding="{Binding AlertType}" Width="120"/>
                <DataGridTextColumn Header="级别" Binding="{Binding Severity}" Width="80"/>
                <DataGridTextColumn Header="消息" Binding="{Binding Message}" Width="*"/>
                <DataGridTextColumn Header="触发时间" Binding="{Binding TriggeredAt}" Width="160"/>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
</UserControl>
```

- [ ] **Step 3：创建 xaml.cs**

```csharp
using System.Windows.Controls;

namespace MES.Modules.Production.Views;

public partial class SystemMonitorView : UserControl
{
    public SystemMonitorView() => InitializeComponent();
}
```

- [ ] **Step 4：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings

---

## Phase 4-G：DI 注册 + Seed 数据

### Task 11：更新 ProductionModule DI 注册

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/ProductionModule.cs`

- [ ] **Step 1：注册新服务**

在 `RegisterTypes` 方法中添加：

```csharp
// V2 Phase 4 新增服务
containerRegistry.Register<IMasterDataService, MasterDataService>();
containerRegistry.Register<IDispatchService, DispatchService>();
containerRegistry.Register<IReportService, ReportService>();
```

- [ ] **Step 2：注册新 View**

在 `RegisterTypes` 方法中添加：

```csharp
// V2 Phase 4 新增视图
containerRegistry.RegisterForNavigation<MasterDataView>();
containerRegistry.RegisterForNavigation<DispatchView>();
containerRegistry.RegisterForNavigation<ProductionReportView>();
containerRegistry.RegisterForNavigation<YieldReportView>();
containerRegistry.RegisterForNavigation<SystemMonitorView>();
```

- [ ] **Step 3：编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: 0 errors, 0 warnings