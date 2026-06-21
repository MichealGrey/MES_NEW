# MES 系统优化改善计划 — 阶段四：外部系统集成框架

> **采用适配器模式 + Mock 实现，可平滑切换真实系统**
> 文档版本：v1.0 | 创建日期：2026-06-06 | 状态：待评审

---

## 目录

- [一、阶段四概述](#一阶段四概述)
- [二、现有 UI 与基础设施评估](#二现有-ui-与基础设施评估)
- [三、集成层架构设计（核心）](#三集成层架构设计核心)
- [四、ERP 集成模块设计](#四erp-集成模块设计)
- [五、EAP 集成模块设计](#五eap-集成模块设计)
- [六、WMS 集成模块设计](#六wms-集成模块设计)
- [七、QMS 集成模块设计](#七qms-集成模块设计)
- [八、OA 集成模块设计](#八oa-集成模块设计)
- [九、客户门户集成模块设计](#九客户门户集成模块设计)
- [十、事件分发器设计](#十事件分发器设计)
- [十一、运行时切换机制](#十一运行时切换机制)
- [十二、各系统管理面板 UI 设计](#十二各系统管理面板-ui-设计)
- [十三、实施顺序与依赖](#十三实施顺序与依赖)
- [十四、验收标准](#十四验收标准)
- [十五、数据库迁移计划](#十五数据库迁移计划)
- [附录 A：完整文件清单](#附录a完整文件清单)
- [附录 B：API 端点汇总](#附录bapi-端点汇总)

---

## 一、阶段四概述

### 1.1 目标

建立 MES 与外部系统的集成框架，实现以下能力：

- **接口层与实现层分离**：通过适配器模式定义统一接口，Mock 实现先行，真实对接预留
- **6 大外部系统集成**：ERP、EAP、WMS、QMS、OA、客户门户
- **事件驱动架构**：基于现有 `ext_system_event` 表的事件队列机制，配合事件分发器实现可靠投递
- **运行时可配置**：通过 `appsettings.json` 控制 Mock/Real 切换，支持热切换
- **可观测性**：事件队列可视化、重试机制、失败告警

### 1.2 核心原则

| 原则 | 说明 |
|------|------|
| **接口优先** | 先定义 Abstractions 接口，再写实现 |
| **Mock 先行** | 所有集成先提供 Mock 实现（模拟延迟 + 随机失败） |
| **平滑切换** | DI 容器根据配置注册 Mock 或 Real 实现，无需改业务代码 |
| **事件驱动** | 业务模块只负责发布事件，由 EventDispatcher 统一分发 |
| **容错设计** | 指数退避重试 + 失败事件隔离 + 告警通知 |

### 1.3 范围与排除

| 包含 | 排除 |
|------|------|
| ERP 集成（工单同步、BOM、物料主数据、库存、发货确认） | 移动端 APP 开发 |
| EAP 集成（配方下发、参数采集、报警上传、设备状态） | 真实外部系统对接（预留 Real 项目） |
| WMS 集成（入库确认、出库指令、库存查询） | 数据中台/BI 对接 |
| QMS 集成（检验结果、SPC 数据、8D 报告、不合格品） | |
| OA 集成（ECN 审批、报废审批、返工审批） | |
| 客户门户（订单进度、质量报告、追溯查询） | |
| 事件分发器 + 重试机制 | |
| 6 个管理面板 UI | |

---

## 二、现有 UI 与基础设施评估

### 2.1 现有数据库表

| 表名 | Entity | 状态 | 用途 |
|------|--------|------|------|
| `ext_system_event` | `ExtSystemEvent` | ✅ 已配置 | 外部系统事件队列 |
| `ext_system_config` | `ExtSystemConfig` | ✅ 已配置 | 外部系统连接配置 |
| `customer_requirement` | `CustomerRequirement` | ✅ 已配置 | 客户需求管理 |

**`ExtSystemEvent` 字段：** `EventId`, `EventType`, `SourceSystem`, `TargetSystem`, `Payload`(JSON), `Status`(Pending/Sent/Failed/Acknowledged), `CreatedAt`, `SentAt`, `ErrorMessage`, `RetryCount`

**`ExtSystemConfig` 字段：** `SystemId`, `SystemName`, `SystemType`, `Endpoint`, `AuthType`, `AuthCredential`, `IsEnabled`, `TimeoutSeconds`, `MaxRetries`, `SubscribedEvents`(JSON), `CreatedAt`

### 2.2 现有 UI — ExternalSystemView.xaml

**位置：** `src/Client/MES.Modules.Production/Views/ExternalSystemView.xaml`

**现状分析：**

| 功能 | 现状 | 评估 |
|------|------|------|
| 事件队列 Tab | DataGrid 展示 EventType、TargetSystem、Status、RetryCount、CreatedAt、SentAt、ErrorMessage | ✅ 基础功能可用 |
| 系统配置 Tab | DataGrid 展示 SystemId、SystemName、SystemType、Endpoint、AuthType、TimeoutSeconds、MaxRetries、IsEnabled | ✅ 只读展示 |
| 状态徽章 | Pending=warning, Sent=success, Failed=danger | ✅ 视觉区分 |
| 操作按钮 | 刷新、重发失败事件 | ⚠️ 重发逻辑在 ViewModel 中本地模拟，未调用后端 API |
| ViewModel | `ExternalSystemViewModel` 依赖 `IExternalSystemService` | ⚠️ Service 为客户端本地实现，直接操作 Repository |

**缺失的集成管理面板：**

| 面板 | 功能 | 优先级 |
|------|------|--------|
| ERP 管理面板 | 工单同步状态、BOM 映射、库存对账、发货确认 | P0 |
| EAP 监控面板 | 设备连接状态、配方下发记录、参数采集实时数据、报警上传 | P0 |
| WMS 对接面板 | 入库/出库指令列表、库存同步状态、预警信息 | P1 |
| QMS 对接面板 | 检验结果同步、SPC 数据推送、8D 报告提交、不合格品跟踪 | P1 |
| OA 审批面板 | 审批流状态、待审批事项、审批历史 | P2 |
| 客户门户管理 | 客户列表、订单进度查询、质量报告下载、追溯报告 | P2 |

### 2.3 现有 API

**现状：** 无专用集成控制器。无 `ExtSystemEventController` 或 `ExtSystemConfigController`。

**需要新建：**

| 控制器 | 用途 |
|--------|------|
| `IntegrationController` | 统一集成管理（手动触发同步、测试连接） |
| `ExtSystemConfigController` | 系统配置 CRUD |
| `ExtSystemEventController` | 事件管理（查询、重试、清除） |

---

## 三、集成层架构设计（核心）

### 3.1 整体架构

```
┌─────────────────────────────────────────────────────────────────┐
│                    MES 业务模块（Controllers/Services）             │
│         WorkOrderService │ LotService │ QualityInspectionService  │
└──────────────────────┬────────────────────────────────────────────┘
                       │ 调用
┌──────────────────────▼────────────────────────────────────────────┐
│                     IEventDispatcher（事件分发器）                   │
│  接收业务事件 → 写入 ext_system_event → 分发到对应适配器 → 标记状态   │
└──────────────────────┬────────────────────────────────────────────┘
                       │ 路由
┌──────────────────────▼────────────────────────────────────────────┐
│                    适配器接口层（Abstractions）                      │
│  IMesErpAdapter │ IMesEapAdapter │ IMesWmsAdapter │ ...            │
└──────────────────────┬────────────────────────────────────────────┘
                       │ 实现
          ┌────────────┴────────────┐
          ▼                         ▼
┌──────────────────┐    ┌──────────────────────┐
│  Adapters.Mock   │    │  Adapters.Real（预留）  │
│  (当前 - 模拟)    │    │  (未来 - 真实对接)      │
└──────────────────┘    └──────────────────────┘
          │
          ▼
┌─────────────────────────────────────────────────────────────────┐
│                     appsettings.json（运行时切换）                  │
│   "Integration": { "Provider": "Mock" }                          │
└─────────────────────────────────────────────────────────────────┘
```

### 3.2 项目结构

```
e:\AiProj\MES_NEW\src\Server\
├── Adapters\
│   ├── MES.Adapters.Abstractions\
│   │   ├── MES.Adapters.Abstractions.csproj
│   │   ├── IMesErpAdapter.cs
│   │   ├── IMesEapAdapter.cs
│   │   ├── IMesWmsAdapter.cs
│   │   ├── IMesQmsAdapter.cs
│   │   ├── IMesOaAdapter.cs
│   │   ├── IMesCustomerPortalAdapter.cs
│   │   ├── IEventDispatcher.cs
│   │   ├── AdapterResult.cs
│   │   └── AdapterException.cs
│   │
│   ├── MES.Adapters.Mock\
│   │   ├── MES.Adapters.Mock.csproj
│   │   ├── MockErpAdapter.cs
│   │   ├── MockEapAdapter.cs
│   │   ├── MockWmsAdapter.cs
│   │   ├── MockQmsAdapter.cs
│   │   ├── MockOaAdapter.cs
│   │   ├── MockCustomerPortalAdapter.cs
│   │   ├── MockData\
│   │   │   ├── erp_work_orders.json
│   │   │   ├── erp_materials.json
│   │   │   ├── erp_inventory.json
│   │   │   ├── eap_recipes.json
│   │   │   ├── eap_parameters.json
│   │   │   ├── eap_alarms.json
│   │   │   ├── wms_inbound.json
│   │   │   ├── wms_inventory.json
│   │   │   ├── qms_inspections.json
│   │   │   ├── qms_spc_data.json
│   │   │   ├── qms_8d_reports.json
│   │   │   ├── oa_approvals.json
│   │   │   ├── portal_orders.json
│   │   │   └── portal_quality_reports.json
│   │   └── MockHelper.cs（通用模拟工具：延迟、随机失败、日志）
│   │
│   └── MES.Adapters.Real\（预留，空项目）
│       ├── MES.Adapters.Real.csproj
│       └── README.md（对接说明与开发指南）
│
└── MES.Api\
    └── Controllers\
        ├── IntegrationController.cs（统一管理入口）
        ├── ExtSystemConfigController.cs（配置 CRUD）
        └── ExtSystemEventController.cs（事件管理）
```

**项目依赖关系：**

```
MES.Adapters.Abstractions  ← 无任何项目依赖（纯接口层）
         ▲
    ┌────┴────┐
    │         │
MES.Adapters.Mock    MES.Adapters.Real（预留）
    │         │
    └────┬────┘
         ▼
MES.Api（注册 DI 时引用具体实现项目）
```

### 3.3 公共类型定义

#### 3.3.1 AdapterResult.cs — 统一返回类型

```csharp
// 文件：src/Server/Adapters/MES.Adapters.Abstractions/AdapterResult.cs
namespace MES.Adapters.Abstractions;

public class AdapterResult
{
    public bool Success { get; init; }
    public string? Data { get; init; }          // JSON 序列化后的响应数据
    public string? ExternalId { get; init; }    // 外部系统返回的 ID（如 ERP 订单号）
    public string? ErrorMessage { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static AdapterResult Ok(string? data = null, string? externalId = null) =>
        new() { Success = true, Data = data, ExternalId = externalId };

    public static AdapterResult Fail(string errorMessage) =>
        new() { Success = false, ErrorMessage = errorMessage };
}

public class AdapterResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? ExternalId { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static AdapterResult<T> Ok(T data, string? externalId = null) =>
        new() { Success = true, Data = data, ExternalId = externalId };

    public static AdapterResult<T> Fail(string errorMessage) =>
        new() { Success = false, ErrorMessage = errorMessage };
}
```

#### 3.3.2 AdapterException.cs — 适配器异常

```csharp
// 文件：src/Server/Adapters/MES.Adapters.Abstractions/AdapterException.cs
namespace MES.Adapters.Abstractions;

public class AdapterException : Exception
{
    public string AdapterType { get; }
    public string Operation { get; }
    public string? ExternalSystemResponse { get; }

    public AdapterException(string adapterType, string operation, string message, string? response = null)
        : base(message)
    {
        AdapterType = adapterType;
        Operation = operation;
        ExternalSystemResponse = response;
    }
}
```

### 3.4 项目文件（.csproj）

#### MES.Adapters.Abstractions.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

#### MES.Adapters.Mock.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\MES.Adapters.Abstractions\MES.Adapters.Abstractions.csproj" />
    <EmbeddedResource Include="MockData\**\*.json" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.*" />
  </ItemGroup>
</Project>
```

---

## 四、ERP 集成模块设计

### 4.1 对接场景

| 场景 | 触发条件 | 数据流向 | 调用方法 |
|------|----------|----------|----------|
| 工单创建同步 | MES 创建工单后 | MES → ERP | `SyncWorkOrderAsync` |
| 工单完工上报 | MES 工单完工 | MES → ERP | `ReportWorkOrderCompletionAsync` |
| BOM 同步 | 工艺路线变更后 | MES → ERP | `SyncBOMAsync` |
| 物料主数据查询 | 新物料引入 | ERP → MES | `GetMaterialMasterAsync` |
| 库存对账 | 定时任务/手动触发 | MES ↔ ERP | `ReconcileInventoryAsync` |
| 发货确认 | 成品入库完成 | MES → ERP | `ConfirmShippingAsync` |

### 4.2 接口定义 — IMesErpAdapter.cs

```csharp
// 文件：src/Server/Adapters/MES.Adapters.Abstractions/IMesErpAdapter.cs
namespace MES.Adapters.Abstractions;

public interface IMesErpAdapter
{
    /// <summary>工单创建同步：MES → ERP</summary>
    Task<AdapterResult> SyncWorkOrderAsync(string workOrderId, string workOrderPayload);

    /// <summary>工单完工上报：MES → ERP</summary>
    Task<AdapterResult> ReportWorkOrderCompletionAsync(string workOrderId, decimal completedQty, decimal scrapQty);

    /// <summary>BOM 同步：MES → ERP</summary>
    Task<AdapterResult> SyncBOMAsync(string productId, string bomPayload);

    /// <summary>物料主数据查询：ERP → MES</summary>
    Task<AdapterResult<string>> GetMaterialMasterAsync(string materialId);

    /// <summary>物料主数据批量查询</summary>
    Task<AdapterResult<string>> GetMaterialMasterBatchAsync(IEnumerable<string> materialIds);

    /// <summary>库存对账：MES ↔ ERP</summary>
    Task<AdapterResult<string>> ReconcileInventoryAsync(string materialId);

    /// <summary>发货确认：MES → ERP</summary>
    Task<AdapterResult> ConfirmShippingAsync(string shippingPayload);

    /// <summary>测试连接</summary>
    Task<bool> TestConnectionAsync();
}
```

### 4.3 Mock 实现策略

**MockErpAdapter.cs 核心逻辑：**

```csharp
// 文件：src/Server/Adapters/MES.Adapters.Mock/MockErpAdapter.cs
namespace MES.Adapters.Mock;

public class MockErpAdapter : IMesErpAdapter
{
    private readonly ILogger<MockErpAdapter> _logger;
    private readonly MockHelper _helper;

    public MockErpAdapter(ILogger<MockErpAdapter> logger)
    {
        _logger = logger;
        _helper = new MockHelper("ERP", failRate: 0.05, latencyMs: (100, 500));
    }

    public async Task<AdapterResult> SyncWorkOrderAsync(string workOrderId, string payload)
    {
        return await _helper.ExecuteAsync(async () =>
        {
            var erpOrderId = $"ERP-WO-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
            _logger.LogInformation("Mock ERP: 工单 {WorkOrderId} 同步成功 → ERP订单 {ErpOrderId}",
                workOrderId, erpOrderId);
            return AdapterResult.Ok($"{{\"erpOrderId\":\"{erpOrderId}\",\"status\":\"Created\"}}", erpOrderId);
        });
    }

    public async Task<AdapterResult> ReportWorkOrderCompletionAsync(string workOrderId, decimal completedQty, decimal scrapQty)
    {
        return await _helper.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Mock ERP: 工单 {WorkOrderId} 完工上报: 完成={CompletedQty}, 报废={ScrapQty}",
                workOrderId, completedQty, scrapQty);
            return AdapterResult.Ok($"{{\"status\":\"Received\",\"receivedAt\":\"{DateTime.UtcNow:O}\"}}");
        });
    }

    // ... 其余方法类似实现
}
```

### 4.4 数据表扩展（ERP 映射）

```sql
-- 工单 ERP 映射表
CREATE TABLE IF NOT EXISTS `erp_work_order_mapping` (
    `id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `mes_work_order_id` VARCHAR(50) NOT NULL,
    `erp_order_id` VARCHAR(100) NULL COMMENT 'ERP返回的订单号',
    `sync_status` VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT 'Pending/Success/Failed',
    `last_sync_at` DATETIME NULL,
    `sync_payload` JSON NULL COMMENT '最后发送的payload',
    `error_message` TEXT NULL,
    `created_at` DATETIME NOT NULL DEFAULT NOW(),
    `updated_at` DATETIME NOT NULL DEFAULT NOW(),
    INDEX idx_mes_wo (`mes_work_order_id`),
    INDEX idx_sync_status (`sync_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工单ERP同步映射';
```

---

## 五、EAP 集成模块设计

### 5.1 对接场景

| 场景 | 触发条件 | 数据流向 | 调用方法 |
|------|----------|----------|----------|
| 配方下发 | 设备 Track-In 前 | MES → EAP | `DownloadRecipeAsync` |
| 参数采集 | 生产进行中（实时） | EAP → MES | `CollectParametersAsync` |
| 报警上传 | 设备触发报警 | EAP → MES | `UploadEquipmentAlarmAsync` |
| 设备状态查询 | 定时轮询/手动 | EAP → MES | `GetEquipmentStatusAsync` |
| 设备状态变更通知 | 设备状态变化 | EAP → MES | `PushEquipmentStatusChangeAsync` |

### 5.2 接口定义 — IMesEapAdapter.cs

```csharp
// 文件：src/Server/Adapters/MES.Adapters.Abstractions/IMesEapAdapter.cs
namespace MES.Adapters.Abstractions;

public interface IMesEapAdapter
{
    /// <summary>配方下发：MES → EAP → 设备</summary>
    Task<AdapterResult> DownloadRecipeAsync(string equipmentId, string recipeId, string recipePayload);

    /// <summary>配方验证：EAP 确认配方已正确加载</summary>
    Task<AdapterResult<string>> VerifyRecipeAsync(string equipmentId, string recipeId);

    /// <summary>参数采集：EAP → MES（实时工艺参数）</summary>
    Task<AdapterResult<string>> CollectParametersAsync(string equipmentId, IEnumerable<string> parameterNames);

    /// <summary>批量参数采集（多设备）</summary>
    Task<AdapterResult<string>> CollectParametersBatchAsync(IEnumerable<string> equipmentIds);

    /// <summary>报警上传：EAP → MES</summary>
    Task<AdapterResult> UploadEquipmentAlarmAsync(string alarmPayload);

    /// <summary>设备状态查询</summary>
    Task<AdapterResult<string>> GetEquipmentStatusAsync(string equipmentId);

    /// <summary>设备状态变更推送（MES接收EAP推送）</summary>
    Task<AdapterResult> PushEquipmentStatusChangeAsync(string statusPayload);

    /// <summary>测试连接</summary>
    Task<bool> TestConnectionAsync();
}
```

### 5.3 Mock 实现策略

- **配方下发**：模拟 200~800ms 延迟，5% 失败率（模拟设备未就绪）
- **参数采集**：返回模拟 JSON，包含温度、压力、时间等参数，值在一定范围内随机波动
- **报警上传**：10% 概率生成模拟报警数据（设备离线、参数超限、安全门打开等）
- **设备状态**：随机返回 Idle/Running/Maintenance/Alarm，状态按概率分布

**MockData/eap_parameters.json 示例：**

```json
{
  "parameters": [
    { "name": "BondForce", "unit": "N", "min": 2.0, "max": 5.0, "target": 3.5 },
    { "name": "BondTemp", "unit": "°C", "min": 180, "max": 220, "target": 200 },
    { "name": "MoldPressure", "unit": "MPa", "min": 5.0, "max": 15.0, "target": 10.0 },
    { "name": "MoldTemp", "unit": "°C", "min": 150, "max": 180, "target": 165 }
  ]
}
```

---

## 六、WMS 集成模块设计

### 6.1 对接场景

| 场景 | 触发条件 | 数据流向 | 调用方法 |
|------|----------|----------|----------|
| 物料入库确认 | 收货完成 | MES → WMS | `ConfirmInboundAsync` |
| 物料出库指令 | 工单领料 | MES → WMS | `RequestOutboundAsync` |
| 库存查询 | 实时查询 | MES → WMS | `QueryInventoryAsync` |
| 库存同步 | 定时/手动 | WMS → MES | `SyncInventoryAsync` |
| 库存预警 | 低于安全库存 | WMS → MES | `GetInventoryAlertsAsync` |

### 6.2 接口定义 — IMesWmsAdapter.cs

```csharp
// 文件：src/Server/Adapters/MES.Adapters.Abstractions/IMesWmsAdapter.cs
namespace MES.Adapters.Abstractions;

public interface IMesWmsAdapter
{
    /// <summary>物料入库确认：MES → WMS</summary>
    Task<AdapterResult> ConfirmInboundAsync(string inboundPayload);

    /// <summary>物料出库指令：MES → WMS</summary>
    Task<AdapterResult> RequestOutboundAsync(string outboundPayload);

    /// <summary>库存查询：WMS → MES</summary>
    Task<AdapterResult<string>> QueryInventoryAsync(string materialId);

    /// <summary>库存同步：WMS → MES（全量/增量）</summary>
    Task<AdapterResult<string>> SyncInventoryAsync(string? materialId = null);

    /// <summary>库存预警查询</summary>
    Task<AdapterResult<string>> GetInventoryAlertsAsync();

    /// <summary>测试连接</summary>
    Task<bool> TestConnectionAsync();
}
```

### 6.3 Mock 实现策略

- **入库确认**：模拟 100~300ms 延迟，返回 WMS 入库单号
- **出库指令**：模拟 150~400ms 延迟，3% 失败率（库存不足）
- **库存查询**：返回 mock JSON，包含库位、批次、数量、有效期
- **库存预警**：定期生成低于 `master_material.min_stock` 的预警

---

## 七、QMS 集成模块设计

### 7.1 对接场景

| 场景 | 触发条件 | 数据流向 | 调用方法 |
|------|----------|----------|----------|
| 检验结果同步 | 检验完成 | MES → QMS | `SyncInspectionResultAsync` |
| SPC 数据推送 | SPC 测量完成 | MES → QMS | `PushSPCDataAsync` |
| 8D 报告提交 | 客诉处理完成 | MES → QMS | `Submit8DReportAsync` |
| 不合格品跟踪 | NCR 状态变更 | MES → QMS | `UpdateNonConformanceAsync` |
| 质量标准查询 | 新检验项创建 | QMS → MES | `GetQualityStandardAsync` |

### 7.2 接口定义 — IMesQmsAdapter.cs

```csharp
// 文件：src/Server/Adapters/MES.Adapters.Abstractions/IMesQmsAdapter.cs
namespace MES.Adapters.Abstractions;

public interface IMesQmsAdapter
{
    /// <summary>检验结果同步：MES → QMS</summary>
    Task<AdapterResult> SyncInspectionResultAsync(string inspectionPayload);

    /// <summary>SPC 数据推送：MES → QMS</summary>
    Task<AdapterResult> PushSPCDataAsync(string spcPayload);

    /// <summary>批量 SPC 推送</summary>
    Task<AdapterResult> PushSPCDataBatchAsync(IEnumerable<string> spcPayloads);

    /// <summary>8D 报告提交：MES → QMS</summary>
    Task<AdapterResult> Submit8DReportAsync(string complaintId, string reportPayload);

    /// <summary>不合格品状态更新：MES → QMS</summary>
    Task<AdapterResult> UpdateNonConformanceAsync(string ncrId, string statusPayload);

    /// <summary>质量标准查询：QMS → MES</summary>
    Task<AdapterResult<string>> GetQualityStandardAsync(string productId, string? stepCode = null);

    /// <summary>测试连接</summary>
    Task<bool> TestConnectionAsync();
}
```

### 7.3 Mock 实现策略

- **检验结果同步**：模拟 100~300ms 延迟，返回 QMS 检验报告编号
- **SPC 推送**：模拟接收并返回确认，2% 失败率
- **8D 报告**：返回 QMS 工单号，格式 `QMS-8D-YYYYMMDD-XXXX`
- **质量标准**：返回 mock JSON，包含检验项目、规格上下限、抽样方案

---

## 八、OA 集成模块设计

### 8.1 对接场景

| 场景 | 触发条件 | 数据流向 | 调用方法 |
|------|----------|----------|----------|
| ECN 审批 | ECN 提交 | MES → OA | `SubmitECNApprovalAsync` |
| 报废审批 | 报废超过阈值 | MES → OA | `SubmitScrapApprovalAsync` |
| 返工审批 | 返工操作 | MES → OA | `SubmitReworkApprovalAsync` |
| 审批结果回调 | OA 审批完成 | OA → MES | `ProcessApprovalCallbackAsync` |
| 人员资质审批 | 新员工上岗 | MES → OA | `SubmitQualificationApprovalAsync` |

### 8.2 接口定义 — IMesOaAdapter.cs

```csharp
// 文件：src/Server/Adapters/MES.Adapters.Abstractions/IMesOaAdapter.cs
namespace MES.Adapters.Abstractions;

/// <summary>
/// 审批类型枚举
/// </summary>
public enum ApprovalType
{
    ECN,        // 工程变更
    Scrap,      // 报废
    Rework,     // 返工
    Qualification // 人员资质
}

/// <summary>
/// 审批状态枚举
/// </summary>
public enum ApprovalStatus
{
    Pending,    // 待审批
    Approved,   // 已通过
    Rejected,   // 已驳回
    Cancelled   // 已取消
}

public interface IMesOaAdapter
{
    /// <summary>提交审批：MES → OA</summary>
    Task<AdapterResult> SubmitApprovalAsync(ApprovalType type, string entityId, string approvalPayload);

    /// <summary>审批结果回调处理：OA → MES</summary>
    Task<AdapterResult> ProcessApprovalCallbackAsync(string approvalId, ApprovalStatus status, string callbackPayload);

    /// <summary>查询审批状态</summary>
    Task<AdapterResult<string>> GetApprovalStatusAsync(string approvalId);

    /// <summary>撤回审批</summary>
    Task<AdapterResult> CancelApprovalAsync(string approvalId);

    /// <summary>测试连接</summary>
    Task<bool> TestConnectionAsync();
}
```

### 8.3 Mock 实现策略

- **提交审批**：返回 OA 流程号，格式 `OA-{Type}-YYYYMMDD-XXXX`
- **审批回调**：MockHelper 提供 `SimulateApprovalCallback` 方法，50% 通过/50% 驳回
- **状态查询**：根据模拟审批号返回 Pending/Approved/Rejected
- **延迟**：200~600ms（模拟 OA 系统响应较慢）

---

## 九、客户门户集成模块设计

### 9.1 对接场景

| 场景 | 触发条件 | 数据流向 | 调用方法 |
|------|----------|----------|----------|
| 订单进度查询 | 客户登录门户 | Portal → MES | `GetOrderProgressAsync` |
| 质量证书下载 | 客户要求 | Portal → MES | `GetQualityCertificateAsync` |
| 追溯报告生成 | 客户要求 | Portal → MES | `GenerateTraceabilityReportAsync` |
| 发货状态通知 | 发货完成 | MES → Portal | `NotifyShippingStatusAsync` |
| 客户订单推送 | 客户下单 | Portal → MES | `PushCustomerOrderAsync` |

### 9.2 接口定义 — IMesCustomerPortalAdapter.cs

```csharp
// 文件：src/Server/Adapters/MES.Adapters.Abstractions/IMesCustomerPortalAdapter.cs
namespace MES.Adapters.Abstractions;

public interface IMesCustomerPortalAdapter
{
    /// <summary>订单进度查询：Portal → MES</summary>
    Task<AdapterResult<string>> GetOrderProgressAsync(string orderId);

    /// <summary>质量证书获取</summary>
    Task<AdapterResult<string>> GetQualityCertificateAsync(string orderId, string? lotId = null);

    /// <summary>追溯报告生成</summary>
    Task<AdapterResult<string>> GenerateTraceabilityReportAsync(string lotId);

    /// <summary>发货状态通知：MES → Portal</summary>
    Task<AdapterResult> NotifyShippingStatusAsync(string shippingPayload);

    /// <summary>客户订单推送：Portal → MES</summary>
    Task<AdapterResult> PushCustomerOrderAsync(string orderPayload);

    /// <summary>测试连接</summary>
    Task<bool> TestConnectionAsync();
}
```

### 9.3 Mock 实现策略

- **订单进度**：返回包含工单创建、生产中、质检、包装、发货等阶段的 JSON
- **质量证书**：返回模拟 PDF 下载 URL
- **追溯报告**：返回包含物料批次、工序记录、检验结果的完整追溯链
- **延迟**：300~800ms（模拟外部访问较慢）

---

## 十、事件分发器设计

### 10.1 架构

```
业务模块
  │
  ├─> WorkOrderService.CreateAsync()
  │     └─> _eventDispatcher.PublishAsync("WorkOrder.Created", "ERP", payload)
  │
  ├─> QualityInspectionService.CompleteAsync()
  │     └─> _eventDispatcher.PublishAsync("Inspection.Completed", "QMS", payload)
  │
  └─> LotService.TrackInAsync()
        └─> _eventDispatcher.PublishAsync("Lot.TrackIn", "EAP", payload)
```

### 10.2 IEventDispatcher 接口

```csharp
// 文件：src/Server/Adapters/MES.Adapters.Abstractions/IEventDispatcher.cs
namespace MES.Adapters.Abstractions;

public interface IEventDispatcher
{
    /// <summary>发布事件到队列（异步非阻塞）</summary>
    Task<string> PublishAsync(string eventType, string targetSystem, object payload);

    /// <summary>处理待发送事件（由定时任务调用）</summary>
    Task<int> ProcessPendingEventsAsync(int maxBatch = 50);

    /// <summary>重试失败事件</summary>
    Task<int> RetryFailedEventsAsync(int maxRetries = 10);

    /// <summary>手动重发指定事件</summary>
    Task<bool> RetryEventAsync(string eventId);

    /// <summary>注册事件处理器</summary>
    void RegisterHandler(string eventType, Func<object, Task> handler);
}
```

### 10.3 EventDispatcherService 实现

```csharp
// 文件：src/Server/MES.Api/Services/EventDispatcherService.cs
namespace MES.Api.Services;

public class EventDispatcherService : IEventDispatcher
{
    private readonly IRepository<ExtSystemEvent> _eventRepo;
    private readonly IRepository<ExtSystemConfig> _configRepo;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventDispatcherService> _logger;
    private readonly Dictionary<string, List<Func<object, Task>>> _handlers = new();

    // 指数退避参数
    private const int BaseDelaySeconds = 10;
    private const int MaxBackoffSeconds = 3600; // 1小时上限

    public EventDispatcherService(
        IRepository<ExtSystemEvent> eventRepo,
        IRepository<ExtSystemConfig> configRepo,
        IServiceProvider serviceProvider,
        ILogger<EventDispatcherService> logger)
    {
        _eventRepo = eventRepo;
        _configRepo = configRepo;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<string> PublishAsync(string eventType, string targetSystem, object payload)
    {
        var eventId = $"EVT-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid():N}".Substring(0, 50);

        var entity = new ExtSystemEvent
        {
            EventId = eventId,
            EventType = eventType,
            SourceSystem = "MES",
            TargetSystem = targetSystem,
            Payload = System.Text.Json.JsonSerializer.Serialize(payload),
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            RetryCount = 0
        };

        await _eventRepo.AddAsync(entity);
        _logger.LogInformation("事件已发布: {EventType} → {TargetSystem} [{EventId}]",
            eventType, targetSystem, eventId);

        return eventId;
    }

    public async Task<int> ProcessPendingEventsAsync(int maxBatch = 50)
    {
        var pendingEvents = await _eventRepo.GetWhereAsync(
            e => e.Status == "Pending",
            maxBatch);

        int processed = 0;
        foreach (var evt in pendingEvents)
        {
            try
            {
                await DispatchEventAsync(evt);
                processed++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "事件分发失败: {EventId}", evt.EventId);
                await MarkEventFailedAsync(evt, ex.Message);
            }
        }

        return processed;
    }

    public async Task<int> RetryFailedEventsAsync(int maxRetries = 10)
    {
        var failedEvents = await _eventRepo.GetWhereAsync(
            e => e.Status == "Failed" && e.RetryCount < maxRetries);

        int retried = 0;
        foreach (var evt in failedEvents)
        {
            // 检查是否到达下次重试时间（指数退避）
            var nextRetryAt = CalculateNextRetryTime(evt.CreatedAt, evt.RetryCount);
            if (DateTime.UtcNow < nextRetryAt) continue;

            try
            {
                await DispatchEventAsync(evt);
                retried++;
            }
            catch (Exception ex)
            {
                await MarkEventFailedAsync(evt, ex.Message);
            }
        }

        return retried;
    }

    public async Task<bool> RetryEventAsync(string eventId)
    {
        var evt = await _eventRepo.GetByIdAsync(eventId);
        if (evt == null) return false;

        try
        {
            evt.Status = "Pending";
            evt.ErrorMessage = null;
            await _eventRepo.UpdateAsync(evt);
            await DispatchEventAsync(evt);
            return true;
        }
        catch (Exception ex)
        {
            await MarkEventFailedAsync(evt, ex.Message);
            return false;
        }
    }

    public void RegisterHandler(string eventType, Func<object, Task> handler)
    {
        if (!_handlers.ContainsKey(eventType))
            _handlers[eventType] = new List<Func<object, Task>>();
        _handlers[eventType].Add(handler);
    }

    // ========== 私有方法 ==========

    private async Task DispatchEventAsync(ExtSystemEvent evt)
    {
        using var scope = _serviceProvider.CreateScope();

        // 根据 TargetSystem 路由到对应适配器
        var result = evt.TargetSystem switch
        {
            "ERP" => await DispatchToErpAsync(evt, scope),
            "EAP" => await DispatchToEapAsync(evt, scope),
            "WMS" => await DispatchToWmsAsync(evt, scope),
            "QMS" => await DispatchToQmsAsync(evt, scope),
            "OA" => await DispatchToOaAsync(evt, scope),
            "CustomerPortal" => await DispatchToCustomerPortalAsync(evt, scope),
            _ => throw new AdapterException("Unknown", "Dispatch", $"未知的目标系统: {evt.TargetSystem}")
        };

        if (result.Success)
        {
            evt.Status = "Sent";
            evt.SentAt = DateTime.UtcNow;
            evt.ErrorMessage = null;
        }
        else
        {
            evt.RetryCount++;
            evt.Status = "Failed";
            evt.ErrorMessage = result.ErrorMessage;
        }

        await _eventRepo.UpdateAsync(evt);
    }

    private async Task<AdapterResult> DispatchToErpAsync(ExtSystemEvent evt, IServiceScope scope)
    {
        var adapter = scope.ServiceProvider.GetRequiredService<IMesErpAdapter>();
        return evt.EventType switch
        {
            "WorkOrder.Created" => await adapter.SyncWorkOrderAsync(
                ParseWorkOrderId(evt.Payload), evt.Payload),
            "WorkOrder.Completed" => await adapter.ReportWorkOrderCompletionAsync(
                ParseWorkOrderId(evt.Payload), 0, 0),
            "BOM.Updated" => await adapter.SyncBOMAsync(
                ParseProductId(evt.Payload), evt.Payload),
            "Shipping.Confirmed" => await adapter.ConfirmShippingAsync(evt.Payload),
            _ => AdapterResult.Fail($"未处理的ERP事件类型: {evt.EventType}")
        };
    }

    private async Task<AdapterResult> DispatchToEapAsync(ExtSystemEvent evt, IServiceScope scope)
    {
        var adapter = scope.ServiceProvider.GetRequiredService<IMesEapAdapter>();
        return evt.EventType switch
        {
            "Recipe.Download" => await adapter.DownloadRecipeAsync(
                ParseEquipmentId(evt.Payload), ParseRecipeId(evt.Payload), evt.Payload),
            "Alarm.Upload" => await adapter.UploadEquipmentAlarmAsync(evt.Payload),
            _ => AdapterResult.Fail($"未处理的EAP事件类型: {evt.EventType}")
        };
    }

    private async Task<AdapterResult> DispatchToWmsAsync(ExtSystemEvent evt, IServiceScope scope)
    {
        var adapter = scope.ServiceProvider.GetRequiredService<IMesWmsAdapter>();
        return evt.EventType switch
        {
            "Inbound.Confirmed" => await adapter.ConfirmInboundAsync(evt.Payload),
            "Outbound.Requested" => await adapter.RequestOutboundAsync(evt.Payload),
            _ => AdapterResult.Fail($"未处理的WMS事件类型: {evt.EventType}")
        };
    }

    private async Task<AdapterResult> DispatchToQmsAsync(ExtSystemEvent evt, IServiceScope scope)
    {
        var adapter = scope.ServiceProvider.GetRequiredService<IMesQmsAdapter>();
        return evt.EventType switch
        {
            "Inspection.Completed" => await adapter.SyncInspectionResultAsync(evt.Payload),
            "SPC.Measured" => await adapter.PushSPCDataAsync(evt.Payload),
            "Complaint.8D_Submitted" => await adapter.Submit8DReportAsync(
                ParseComplaintId(evt.Payload), evt.Payload),
            "NCR.Updated" => await adapter.UpdateNonConformanceAsync(
                ParseNcrId(evt.Payload), evt.Payload),
            _ => AdapterResult.Fail($"未处理的QMS事件类型: {evt.EventType}")
        };
    }

    private async Task<AdapterResult> DispatchToOaAsync(ExtSystemEvent evt, IServiceScope scope)
    {
        var adapter = scope.ServiceProvider.GetRequiredService<IMesOaAdapter>();
        return evt.EventType switch
        {
            "ECN.Submitted" => await adapter.SubmitApprovalAsync(
                ApprovalType.ECN, ParseEcnId(evt.Payload), evt.Payload),
            "Scrap.Requested" => await adapter.SubmitApprovalAsync(
                ApprovalType.Scrap, ParseScrapId(evt.Payload), evt.Payload),
            "Rework.Requested" => await adapter.SubmitApprovalAsync(
                ApprovalType.Rework, ParseReworkId(evt.Payload), evt.Payload),
            _ => AdapterResult.Fail($"未处理的OA事件类型: {evt.EventType}")
        };
    }

    private async Task<AdapterResult> DispatchToCustomerPortalAsync(ExtSystemEvent evt, IServiceScope scope)
    {
        var adapter = scope.ServiceProvider.GetRequiredService<IMesCustomerPortalAdapter>();
        return evt.EventType switch
        {
            "Shipping.Completed" => await adapter.NotifyShippingStatusAsync(evt.Payload),
            _ => AdapterResult.Fail($"未处理的客户门户事件类型: {evt.EventType}")
        };
    }

    private async Task MarkEventFailedAsync(ExtSystemEvent evt, string errorMessage)
    {
        evt.RetryCount++;
        evt.Status = "Failed";
        evt.ErrorMessage = errorMessage?.Length > 1000
            ? errorMessage.Substring(0, 1000)
            : errorMessage;
        await _eventRepo.UpdateAsync(evt);
    }

    // 指数退避计算
    private static DateTime CalculateNextRetryTime(DateTime createdAt, int retryCount)
    {
        var delaySeconds = Math.Min(
            BaseDelaySeconds * (int)Math.Pow(2, retryCount),
            MaxBackoffSeconds);
        return createdAt.AddSeconds(delaySeconds);
    }

    // 简单的 JSON 解析辅助方法
    private static string ParseWorkOrderId(string payload) { /* JSON解析逻辑 */ return string.Empty; }
    private static string ParseProductId(string payload) { return string.Empty; }
    private static string ParseEquipmentId(string payload) { return string.Empty; }
    private static string ParseRecipeId(string payload) { return string.Empty; }
    private static string ParseComplaintId(string payload) { return string.Empty; }
    private static string ParseNcrId(string payload) { return string.Empty; }
    private static string ParseEcnId(string payload) { return string.Empty; }
    private static string ParseScrapId(string payload) { return string.Empty; }
    private static string ParseReworkId(string payload) { return string.Empty; }
}
```

### 10.4 重试机制 — 指数退避

| 重试次数 | 延迟时间 | 累计时间 |
|----------|----------|----------|
| 0（首次） | 立即 | 0s |
| 1 | 10s | 10s |
| 2 | 20s | 30s |
| 3 | 40s | 1min 10s |
| 4 | 80s | 2min 30s |
| 5 | 160s | 5min 10s |
| 6 | 320s | 10min 30s |
| 7 | 640s | 21min 10s |
| 8 | 1280s | 42min 30s |
| 9+ | 3600s（上限） | 递增 |

### 10.5 定时任务集成

在 `MES.Api` 中使用 `BackgroundService` 实现定时处理：

```csharp
// 文件：src/Server/MES.Api/Services/EventDispatcherBackgroundService.cs
namespace MES.Api.Services;

public class EventDispatcherBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventDispatcherBackgroundService> _logger;

    public EventDispatcherBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<EventDispatcherBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IEventDispatcher>();

                // 处理 Pending 事件
                int processed = await dispatcher.ProcessPendingEventsAsync(maxBatch: 50);
                if (processed > 0)
                    _logger.LogInformation("已处理 {Count} 个待发送事件", processed);

                // 重试 Failed 事件
                int retried = await dispatcher.RetryFailedEventsAsync(maxRetries: 5);
                if (retried > 0)
                    _logger.LogInformation("已重试 {Count} 个失败事件", retried);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "事件分发后台任务异常");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // 每30秒执行一次
        }
    }
}
```

---

## 十一、运行时切换机制

### 11.1 appsettings.json 配置格式

```json
{
  "Integration": {
    "DefaultProvider": "Mock",
    "Adapters": {
      "ERP": {
        "Provider": "Mock",
        "Enabled": true,
        "MockConfig": {
          "FailRate": 0.05,
          "MinLatencyMs": 100,
          "MaxLatencyMs": 500
        }
      },
      "EAP": {
        "Provider": "Mock",
        "Enabled": true,
        "MockConfig": {
          "FailRate": 0.03,
          "MinLatencyMs": 200,
          "MaxLatencyMs": 800
        }
      },
      "WMS": {
        "Provider": "Mock",
        "Enabled": true,
        "MockConfig": {
          "FailRate": 0.02,
          "MinLatencyMs": 100,
          "MaxLatencyMs": 300
        }
      },
      "QMS": {
        "Provider": "Mock",
        "Enabled": true,
        "MockConfig": {
          "FailRate": 0.02,
          "MinLatencyMs": 100,
          "MaxLatencyMs": 300
        }
      },
      "OA": {
        "Provider": "Mock",
        "Enabled": true,
        "MockConfig": {
          "FailRate": 0.01,
          "MinLatencyMs": 200,
          "MaxLatencyMs": 600
        }
      },
      "CustomerPortal": {
        "Provider": "Mock",
        "Enabled": true,
        "MockConfig": {
          "FailRate": 0.01,
          "MinLatencyMs": 300,
          "MaxLatencyMs": 800
        }
      }
    }
  },
  "EventDispatcher": {
    "Enabled": true,
    "PollingIntervalSeconds": 30,
    "MaxBatchSize": 50,
    "MaxRetries": 5,
    "BaseDelaySeconds": 10
  }
}
```

### 11.2 DI 容器注册（Program.cs 扩展）

```csharp
// 文件：src/Server/MES.Api/Configuration/IntegrationServiceCollectionExtensions.cs
namespace MES.Api.Configuration;

public static class IntegrationServiceCollectionExtensions
{
    public static IServiceCollection AddIntegrationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var integrationConfig = configuration.GetSection("Integration");
        var defaultProvider = integrationConfig.GetValue<string>("DefaultProvider") ?? "Mock";

        // 注册每个适配器（根据配置选择 Mock 或 Real）
        services.RegisterAdapter<IMesErpAdapter, MockErpAdapter>(
            integrationConfig, "ERP", defaultProvider);
        services.RegisterAdapter<IMesEapAdapter, MockEapAdapter>(
            integrationConfig, "EAP", defaultProvider);
        services.RegisterAdapter<IMesWmsAdapter, MockWmsAdapter>(
            integrationConfig, "WMS", defaultProvider);
        services.RegisterAdapter<IMesQmsAdapter, MockQmsAdapter>(
            integrationConfig, "QMS", defaultProvider);
        services.RegisterAdapter<IMesOaAdapter, MockOaAdapter>(
            integrationConfig, "OA", defaultProvider);
        services.RegisterAdapter<IMesCustomerPortalAdapter, MockCustomerPortalAdapter>(
            integrationConfig, "CustomerPortal", defaultProvider);

        // 注册事件分发器
        services.AddScoped<IEventDispatcher, EventDispatcherService>();

        // 注册后台服务
        services.AddHostedService<EventDispatcherBackgroundService>();

        return services;
    }

    private static IServiceCollection RegisterAdapter<TInterface, TMock>(
        this IServiceCollection services,
        IConfigurationSection config,
        string systemKey,
        string defaultProvider)
        where TInterface : class
        where TMock : class, TInterface
    {
        var systemConfig = config.GetSection("Adapters").GetSection(systemKey);
        var provider = systemConfig.GetValue<string>("Provider") ?? defaultProvider;
        var enabled = systemConfig.GetValue<bool>("Enabled", true);

        if (!enabled)
        {
            // 禁用的适配器注册为 No-Op 实现
            return services;
        }

        switch (provider.ToUpperInvariant())
        {
            case "MOCK":
                services.AddScoped<TInterface, TMock>();
                break;
            case "REAL":
                // 预留：services.AddScoped<TInterface, RealErpAdapter>();
                services.AddScoped<TInterface, TMock>(); // 暂时 fallback 到 Mock
                break;
            default:
                services.AddScoped<TInterface, TMock>();
                break;
        }

        return services;
    }
}
```

**在 Program.cs 中调用：**

```csharp
// 在 Program.cs 的 builder.Services 配置块中添加：
builder.Services.AddIntegrationServices(builder.Configuration);
```

### 11.3 热切换支持

通过监听 `appsettings.json` 变化，支持运行时切换适配器（需要自定义实现配置监听器）。初期阶段使用**重启生效**方式即可，热切换作为后续优化项。

---

## 十二、各系统管理面板 UI 设计

### 12.1 总体布局方案

**方案：扩展 ExternalSystemView 为多 Tab 结构**

在现有 ExternalSystemView 基础上，将原有两个 Tab（事件队列、系统配置）保留，新增 6 个系统专用 Tab：

```
TabControl
├── Tab 1: 事件队列（现有）
├── Tab 2: 系统配置（现有）
├── Tab 3: ERP 管理面板（新增）
├── Tab 4: EAP 监控面板（新增）
├── Tab 5: WMS 对接面板（新增）
├── Tab 6: QMS 对接面板（新增）
├── Tab 7: OA 审批面板（新增）
└── Tab 8: 客户门户（新增）
```

### 12.2 ERP 管理面板

```xml
<!-- Tab: ERP 管理面板 -->
<StackPanel Margin="16">
    <!-- 状态概览 -->
    <StackPanel Orientation="Horizontal" Margin="0,0,0,16">
        <Border Style="{StaticResource StatCard}" Width="160">
            <StackPanel>
                <TextBlock Text="连接状态" FontSize="12" Foreground="{DynamicResource MesTextSecondaryBrush}"/>
                <TextBlock Text="{Binding ErpConnectionStatus}" FontSize="18" FontWeight="SemiBold">
                    <TextBlock.Foreground>
                        <MultiBinding Converter="{StaticResource StatusColorConverter}">
                            <Binding Path="ErpConnectionStatus"/>
                        </MultiBinding>
                    </TextBlock.Foreground>
                </TextBlock>
            </StackPanel>
        </Border>
        <Border Style="{StaticResource StatCard}" Width="160" Margin="12,0">
            <StackPanel>
                <TextBlock Text="今日同步" FontSize="12" Foreground="{DynamicResource MesTextSecondaryBrush}"/>
                <TextBlock Text="{Binding ErpTodaySyncCount}" FontSize="18" FontWeight="SemiBold"/>
            </StackPanel>
        </Border>
        <Border Style="{StaticResource StatCard}" Width="160" Margin="12,0">
            <StackPanel>
                <TextBlock Text="失败数" FontSize="12" Foreground="{DynamicResource MesTextSecondaryBrush}"/>
                <TextBlock Text="{Binding ErpFailCount}" FontSize="18" FontWeight="SemiBold" Foreground="{DynamicResource MesDangerBrush}"/>
            </StackPanel>
        </Border>
        <Button Content="测试连接" Command="{Binding TestErpConnectionCommand}" Margin="auto,0,0,0"
                Style="{DynamicResource MesPrimaryButton}" VerticalAlignment="Center"/>
    </StackPanel>

    <!-- 工单同步列表 -->
    <TextBlock Text="工单同步记录" FontSize="16" FontWeight="SemiBold" Margin="0,0,0,8"/>
    <DataGrid ItemsSource="{Binding ErpWorkOrderSyncList}" AutoGenerateColumns="False" IsReadOnly="True" Height="200">
        <DataGrid.Columns>
            <DataGridTextColumn Header="MES工单号" Binding="{Binding MesWorkOrderId}" Width="120"/>
            <DataGridTextColumn Header="ERP订单号" Binding="{Binding ErpOrderId}" Width="120"/>
            <DataGridTemplateColumn Header="同步状态" Width="100">
                <DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <Border Style="{StaticResource StatusBadge}">
                            <TextBlock Text="{Binding SyncStatus}"/>
                        </Border>
                    </DataTemplate>
                </DataGridTemplateColumn.CellTemplate>
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="最后同步" Binding="{Binding LastSyncAt, StringFormat=HH:mm:ss}" Width="100"/>
            <DataGridTextColumn Header="错误信息" Binding="{Binding ErrorMessage}" Width="*"/>
        </DataGrid.Columns>
    </DataGrid>

    <!-- 库存对账 -->
    <TextBlock Text="库存对账" FontSize="16" FontWeight="SemiBold" Margin="0,16,0,8"/>
    <StackPanel Orientation="Horizontal">
        <TextBox Text="{Binding InventoryReconcileMaterialId}" Width="200" Margin="0,0,8,0"
                 Text="输入物料编号..."/>
        <Button Content="执行对账" Command="{Binding ReconcileInventoryCommand}"
                Style="{DynamicResource MesPrimaryButton}"/>
    </StackPanel>
</StackPanel>
```

### 12.3 EAP 监控面板

```xml
<!-- Tab: EAP 监控面板 -->
<StackPanel Margin="16">
    <!-- 设备连接状态 -->
    <TextBlock Text="设备连接状态" FontSize="16" FontWeight="SemiBold" Margin="0,0,0,8"/>
    <DataGrid ItemsSource="{Binding EapEquipmentList}" AutoGenerateColumns="False" IsReadOnly="True" Height="150">
        <DataGrid.Columns>
            <DataGridTextColumn Header="设备编号" Binding="{Binding EquipmentId}" Width="100"/>
            <DataGridTextColumn Header="设备名称" Binding="{Binding EquipmentName}" Width="120"/>
            <DataGridTemplateColumn Header="连接状态" Width="80">
                <DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <Ellipse Width="10" Height="10" Fill="{Binding ConnectionState, Converter={StaticResource ConnectionColorConverter}}"
                                 HorizontalAlignment="Center" VerticalAlignment="Center"/>
                    </DataTemplate>
                </DataGridTemplateColumn.CellTemplate>
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="当前配方" Binding="{Binding CurrentRecipe}" Width="150"/>
            <DataGridTextColumn Header="设备状态" Binding="{Binding EquipmentStatus}" Width="80"/>
            <DataGridTextColumn Header="最后心跳" Binding="{Binding LastHeartbeat, StringFormat=HH:mm:ss}" Width="100"/>
        </DataGrid.Columns>
    </DataGrid>

    <!-- 配方下发记录 -->
    <TextBlock Text="配方下发记录" FontSize="16" FontWeight="SemiBold" Margin="0,16,0,8"/>
    <DataGrid ItemsSource="{Binding EapRecipeDownloads}" AutoGenerateColumns="False" IsReadOnly="True" Height="150">
        <DataGrid.Columns>
            <DataGridTextColumn Header="配方ID" Binding="{Binding RecipeId}" Width="120"/>
            <DataGridTextColumn Header="设备" Binding="{Binding EquipmentId}" Width="100"/>
            <DataGridTemplateColumn Header="状态" Width="80">
                <DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <Border Style="{StaticResource StatusBadge}">
                            <TextBlock Text="{Binding DownloadStatus}"/>
                        </Border>
                    </DataTemplate>
                </DataGridTemplateColumn.CellTemplate>
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="下发时间" Binding="{Binding DownloadAt, StringFormat=yyyy-MM-dd HH:mm}" Width="150"/>
            <DataGridTextColumn Header="验证结果" Binding="{Binding VerifyResult}" Width="*"/>
        </DataGrid.Columns>
    </DataGrid>

    <!-- 实时参数采集 -->
    <TextBlock Text="实时参数采集" FontSize="16" FontWeight="SemiBold" Margin="0,16,0,8"/>
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="200"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>
        <ListBox Grid.Column="0" ItemsSource="{Binding EapEquipmentList}"
                 SelectedItem="{Binding SelectedEapEquipment}"
                 DisplayMemberPath="EquipmentName" Margin="0,0,8,0"/>
        <DataGrid Grid.Column="1" ItemsSource="{Binding SelectedEapEquipmentParameters}"
                  AutoGenerateColumns="False" IsReadOnly="True">
            <DataGrid.Columns>
                <DataGridTextColumn Header="参数名" Binding="{Binding Name}" Width="120"/>
                <DataGridTextColumn Header="当前值" Binding="{Binding Value}" Width="80"/>
                <DataGridTextColumn Header="单位" Binding="{Binding Unit}" Width="60"/>
                <DataGridTextColumn Header="下限" Binding="{Binding Lsl}" Width="60"/>
                <DataGridTextColumn Header="上限" Binding="{Binding Usl}" Width="60"/>
                <DataGridTemplateColumn Header="状态" Width="60">
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <TextBlock Text="{Binding IsNormal, Converter={StaticResource BoolToStatusConverter}}"
                                       Foreground="{Binding IsNormal, Converter={StaticResource BoolToColorConverter}}"/>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
</StackPanel>
```

### 12.4 WMS 对接面板

```xml
<!-- Tab: WMS 对接面板 -->
<StackPanel Margin="16">
    <!-- 操作按钮 -->
    <StackPanel Orientation="Horizontal" Margin="0,0,0,16">
        <Button Content="测试连接" Command="{Binding TestWmsConnectionCommand}" Style="{DynamicResource MesPrimaryButton}" Margin="0,0,8,0"/>
        <Button Content="同步库存" Command="{Binding SyncWmsInventoryCommand}" Style="{DynamicResource MesPrimaryButton}" Margin="0,0,8,0"/>
        <Button Content="刷新" Command="{Binding RefreshWmsCommand}" Style="{DynamicResource MesSecondaryButton}"/>
    </StackPanel>

    <!-- 入出库指令 -->
    <TextBlock Text="入出库指令" FontSize="16" FontWeight="SemiBold" Margin="0,0,0,8"/>
    <DataGrid ItemsSource="{Binding WmsInOutOrders}" AutoGenerateColumns="False" IsReadOnly="True" Height="200">
        <DataGrid.Columns>
            <DataGridTextColumn Header="指令号" Binding="{Binding OrderNo}" Width="120"/>
            <DataGridTemplateColumn Header="类型" Width="60">
                <DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <TextBlock Text="{Binding Type}" Foreground="{Binding Type, Converter={StaticResource InOutColorConverter}}"/>
                    </DataTemplate>
                </DataGridTemplateColumn.CellTemplate>
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="物料" Binding="{Binding MaterialId}" Width="100"/>
            <DataGridTextColumn Header="数量" Binding="{Binding Quantity}" Width="80"/>
            <DataGridTemplateColumn Header="状态" Width="80">
                <DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <Border Style="{StaticResource StatusBadge}">
                            <TextBlock Text="{Binding Status}"/>
                        </Border>
                    </DataTemplate>
                </DataGridTemplateColumn.CellTemplate>
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="WMS确认号" Binding="{Binding WmsConfirmNo}" Width="120"/>
            <DataGridTextColumn Header="时间" Binding="{Binding CreatedAt, StringFormat=yyyy-MM-dd HH:mm}" Width="150"/>
        </DataGrid.Columns>
    </DataGrid>

    <!-- 库存预警 -->
    <TextBlock Text="库存预警" FontSize="16" FontWeight="SemiBold" Margin="0,16,0,8"/>
    <ItemsControl ItemsSource="{Binding WmsInventoryAlerts}">
        <ItemsControl.ItemTemplate>
            <DataTemplate>
                <Border Style="{StaticResource AlertCard}" Margin="0,4">
                    <StackPanel Orientation="Horizontal">
                        <TextBlock Text="⚠" Foreground="{DynamicResource MesWarningBrush}" Margin="0,0,8,0"/>
                        <TextBlock Text="{Binding AlertMessage}" FontWeight="SemiBold"/>
                        <TextBlock Text="{Binding CurrentStock, StringFormat='当前库存: {0}'}" Margin="16,0,0,0"/>
                    </StackPanel>
                </Border>
            </DataTemplate>
        </ItemsControl.ItemTemplate>
    </ItemsControl>
</StackPanel>
```

### 12.5 QMS 对接面板

```xml
<!-- Tab: QMS 对接面板 -->
<StackPanel Margin="16">
    <StackPanel Orientation="Horizontal" Margin="0,0,0,16">
        <Button Content="测试连接" Command="{Binding TestQmsConnectionCommand}" Style="{DynamicResource MesPrimaryButton}" Margin="0,0,8,0"/>
        <Button Content="同步检验结果" Command="{Binding SyncInspectionResultsCommand}" Style="{DynamicResource MesPrimaryButton}" Margin="0,0,8,0"/>
        <Button Content="推送SPC数据" Command="{Binding PushSpcDataCommand}" Style="{DynamicResource MesPrimaryButton}"/>
    </StackPanel>

    <!-- 检验结果同步 -->
    <TextBlock Text="检验结果同步" FontSize="16" FontWeight="SemiBold" Margin="0,0,0,8"/>
    <DataGrid ItemsSource="{Binding QmsInspectionSyncs}" AutoGenerateColumns="False" IsReadOnly="True" Height="180">
        <DataGrid.Columns>
            <DataGridTextColumn Header="检验编号" Binding="{Binding InspectionId}" Width="120"/>
            <DataGridTextColumn Header="批次" Binding="{Binding LotId}" Width="100"/>
            <DataGridTextColumn Header="类型" Binding="{Binding InspectionType}" Width="80"/>
            <DataGridTextColumn Header="结果" Binding="{Binding Result}" Width="60"/>
            <DataGridTemplateColumn Header="同步状态" Width="80">
                <DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <Border Style="{StaticResource StatusBadge}">
                            <TextBlock Text="{Binding SyncStatus}"/>
                        </Border>
                    </DataTemplate>
                </DataGridTemplateColumn.CellTemplate>
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="同步时间" Binding="{Binding SyncAt, StringFormat=yyyy-MM-dd HH:mm}" Width="150"/>
        </DataGrid.Columns>
    </DataGrid>

    <!-- 8D报告 -->
    <TextBlock Text="8D 报告提交" FontSize="16" FontWeight="SemiBold" Margin="0,16,0,8"/>
    <DataGrid ItemsSource="{Binding Qms8DReports}" AutoGenerateColumns="False" IsReadOnly="True" Height="150">
        <DataGrid.Columns>
            <DataGridTextColumn Header="客诉编号" Binding="{Binding ComplaintId}" Width="120"/>
            <DataGridTextColumn Header="QMS工单号" Binding="{Binding QmsTicketNo}" Width="120"/>
            <DataGridTemplateColumn Header="状态" Width="80">
                <DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <Border Style="{StaticResource StatusBadge}">
                            <TextBlock Text="{Binding Status}"/>
                        </Border>
                    </DataTemplate>
                </DataGridTemplateColumn.CellTemplate>
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="提交时间" Binding="{Binding SubmittedAt, StringFormat=yyyy-MM-dd HH:mm}" Width="150"/>
            <DataGridTextColumn Header="客诉描述" Binding="{Binding Description}" Width="*"/>
        </DataGrid.Columns>
    </DataGrid>
</StackPanel>
```

### 12.6 OA 审批面板

```xml
<!-- Tab: OA 审批面板 -->
<StackPanel Margin="16">
    <StackPanel Orientation="Horizontal" Margin="0,0,0,16">
        <Button Content="测试连接" Command="{Binding TestOaConnectionCommand}" Style="{DynamicResource MesPrimaryButton}" Margin="0,0,8,0"/>
        <Button Content="刷新" Command="{Binding RefreshOaCommand}" Style="{DynamicResource MesSecondaryButton}"/>
    </StackPanel>

    <!-- 审批事项列表 -->
    <TextBlock Text="审批事项" FontSize="16" FontWeight="SemiBold" Margin="0,0,0,8"/>
    <DataGrid ItemsSource="{Binding OaApprovalItems}" AutoGenerateColumns="False" IsReadOnly="True" Height="250">
        <DataGrid.Columns>
            <DataGridTextColumn Header="OA流程号" Binding="{Binding ApprovalId}" Width="140"/>
            <DataGridTemplateColumn Header="类型" Width="80">
                <DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <TextBlock Text="{Binding ApprovalType}"
                                   Foreground="{Binding ApprovalType, Converter={StaticResource ApprovalTypeColorConverter}}"/>
                    </DataTemplate>
                </DataGridTemplateColumn.CellTemplate>
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="关联实体" Binding="{Binding EntityId}" Width="100"/>
            <DataGridTemplateColumn Header="审批状态" Width="80">
                <DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <Border Style="{StaticResource StatusBadge}">
                            <TextBlock Text="{Binding ApprovalStatus}"/>
                        </Border>
                    </DataTemplate>
                </DataGridTemplateColumn.CellTemplate>
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="提交时间" Binding="{Binding SubmittedAt, StringFormat=yyyy-MM-dd HH:mm}" Width="150"/>
            <DataGridTextColumn Header="审批意见" Binding="{Binding ApprovalComment}" Width="*"/>
        </DataGrid.Columns>
    </DataGrid>
</StackPanel>
```

### 12.7 客户门户管理面板

```xml
<!-- Tab: 客户门户管理 -->
<StackPanel Margin="16">
    <StackPanel Orientation="Horizontal" Margin="0,0,0,16">
        <Button Content="测试连接" Command="{Binding TestPortalConnectionCommand}" Style="{DynamicResource MesPrimaryButton}" Margin="0,0,8,0"/>
    </StackPanel>

    <!-- 客户列表 -->
    <TextBlock Text="已开通客户" FontSize="16" FontWeight="SemiBold" Margin="0,0,0,8"/>
    <DataGrid ItemsSource="{Binding PortalCustomers}" AutoGenerateColumns="False" IsReadOnly="True" Height="120">
        <DataGrid.Columns>
            <DataGridTextColumn Header="客户编号" Binding="{Binding CustomerId}" Width="100"/>
            <DataGridTextColumn Header="客户名称" Binding="{Binding CustomerName}" Width="150"/>
            <DataGridCheckBoxColumn Header="门户已开通" Binding="{Binding PortalEnabled}" Width="100"/>
            <DataGridTextColumn Header="订单数" Binding="{Binding OrderCount}" Width="80"/>
            <DataGridTextColumn Header="开通时间" Binding="{Binding ActivatedAt, StringFormat=yyyy-MM-dd}" Width="120"/>
        </DataGrid.Columns>
    </DataGrid>

    <!-- 订单进度 -->
    <TextBlock Text="订单进度" FontSize="16" FontWeight="SemiBold" Margin="0,16,0,8"/>
    <DataGrid ItemsSource="{Binding PortalOrderProgress}" AutoGenerateColumns="False" IsReadOnly="True" Height="200">
        <DataGrid.Columns>
            <DataGridTextColumn Header="订单号" Binding="{Binding OrderId}" Width="120"/>
            <DataGridTextColumn Header="客户" Binding="{Binding CustomerName}" Width="120"/>
            <DataGridTextColumn Header="产品" Binding="{Binding ProductName}" Width="100"/>
            <DataGridTemplateColumn Header="进度" Width="150">
                <DataGridTemplateColumn.CellTemplate>
                    <DataTemplate>
                        <ProgressBar Value="{Binding ProgressPercent}" Maximum="100" Height="16"
                                     Foreground="{Binding ProgressPercent, Converter={StaticResource ProgressColorConverter}}"/>
                    </DataTemplate>
                </DataGridTemplateColumn.CellTemplate>
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="当前阶段" Binding="{Binding CurrentStage}" Width="100"/>
            <DataGridTextColumn Header="预计完成" Binding="{Binding EstimatedCompletion, StringFormat=yyyy-MM-dd}" Width="120"/>
        </DataGrid.Columns>
    </DataGrid>
</StackPanel>
```

### 12.8 ViewModel 改造

需要改造 `ExternalSystemViewModel`，新增以下属性和命令：

| 类别 | 属性/命令 | 用途 |
|------|-----------|------|
| ERP | `ErpConnectionStatus`, `ErpTodaySyncCount`, `ErpFailCount`, `ErpWorkOrderSyncList` | ERP 状态与数据 |
| ERP | `TestErpConnectionCommand`, `ReconcileInventoryCommand` | ERP 操作 |
| EAP | `EapEquipmentList`, `EapRecipeDownloads`, `SelectedEapEquipment`, `SelectedEapEquipmentParameters` | EAP 监控数据 |
| WMS | `WmsInOutOrders`, `WmsInventoryAlerts` | WMS 数据 |
| WMS | `TestWmsConnectionCommand`, `SyncWmsInventoryCommand` | WMS 操作 |
| QMS | `QmsInspectionSyncs`, `Qms8DReports` | QMS 数据 |
| QMS | `TestQmsConnectionCommand`, `SyncInspectionResultsCommand`, `PushSpcDataCommand` | QMS 操作 |
| OA | `OaApprovalItems` | OA 审批数据 |
| OA | `TestOaConnectionCommand`, `RefreshOaCommand` | OA 操作 |
| Portal | `PortalCustomers`, `PortalOrderProgress` | 客户门户数据 |
| Portal | `TestPortalConnectionCommand` | 门户操作 |

---

## 十三、实施顺序与依赖

### 13.1 实施阶段

```
Phase 4.1: 基础设施搭建（2天）
├── 创建 MES.Adapters.Abstractions 项目
├── 创建 MES.Adapters.Mock 项目
├── 创建 MES.Adapters.Real 预留项目
├── 定义公共类型 (AdapterResult, AdapterException)
├── 定义 6 个适配器接口
└── MockHelper 通用工具类

Phase 4.2: Mock 实现（4天）
├── MockErpAdapter + MockData（1天）
├── MockEapAdapter + MockData（1天）
├── MockWmsAdapter + MockData（0.5天）
├── MockQmsAdapter + MockData（0.5天）
├── MockOaAdapter + MockData（0.5天）
├── MockCustomerPortalAdapter + MockData（0.5天）
└── DI 注册 + appsettings.json 配置

Phase 4.3: 事件分发器（2天）
├── EventDispatcherService 实现
├── 事件路由分发逻辑
├── 指数退避重试机制
├── 后台定时任务
└── 集成到 Program.cs

Phase 4.4: API 控制器（1.5天）
├── IntegrationController（测试连接、手动触发同步）
├── ExtSystemConfigController（配置 CRUD）
├── ExtSystemEventController（事件查询、重试）
└── Swagger 文档完善

Phase 4.5: 数据库迁移（0.5天）
├── erp_work_order_mapping 表
├── ext_system_config 初始数据
└── EF Core Migration

Phase 4.6: UI 面板开发（5天）
├── 扩展 ExternalSystemView 为 8 Tab 结构
├── ERP 管理面板（1天）
├── EAP 监控面板（1.5天）
├── WMS 对接面板（0.5天）
├── QMS 对接面板（1天）
├── OA 审批面板（0.5天）
└── 客户门户管理（0.5天）

Phase 4.7: 联调与验收（1天）
├── 端到端测试
├── 重试机制验证
├── 配置热切换测试
└── 验收检查清单
```

### 13.2 依赖关系图

```
Phase 4.1（基础设施）
    │
    ├─→ Phase 4.2（Mock实现）
    │       │
    │       └─→ Phase 4.3（事件分发器）
    │               │
    │               └─→ Phase 4.4（API控制器）
    │                       │
    │                       └─→ Phase 4.7（联调验收）
    │
    ├─→ Phase 4.5（数据库迁移）
    │       │
    │       └─→ Phase 4.6（UI面板）
    │               │
    │               └─→ Phase 4.7（联调验收）
```

---

## 十四、验收标准

### 14.1 功能验收

| # | 验收项 | 标准 | 验证方式 |
|---|--------|------|----------|
| 1 | 6 个适配器接口定义完整 | 编译通过，方法签名符合文档 | 编译检查 |
| 2 | 6 个 Mock 实现可用 | 调用后返回模拟数据，延迟在设定范围内 | 单元测试 |
| 3 | 事件分发器正常工作 | 调用 PublishAsync 后事件写入 ext_system_event | 日志验证 |
| 4 | 事件路由正确 | ERP 事件调用 IMesErpAdapter，QMS 事件调用 IMesQmsAdapter | 单元测试 |
| 5 | 重试机制生效 | 模拟失败后按指数退避重试，最多 N 次 | 集成测试 |
| 6 | Mock/Real 切换 | 修改 appsettings.json 后重启生效 | 配置测试 |
| 7 | API 控制器可用 | Swagger 可见所有集成端点，调用正常 | Swagger 测试 |
| 8 | ExtSystemConfig CRUD | 增删改查系统配置正常 | API 测试 |
| 9 | UI 面板展示 | 8 个 Tab 均正常显示，数据绑定正确 | 手动验证 |
| 10 | 测试连接功能 | 点击"测试连接"返回连接状态 | 手动验证 |

### 14.2 性能验收

| # | 验收项 | 标准 |
|---|--------|------|
| 1 | 事件处理吞吐量 | ≥ 100 events/minute |
| 2 | Mock 接口响应时间 | 在配置的延迟范围内 |
| 3 | 事件队列积压告警 | Pending 事件 > 500 时记录 Warning |
| 4 | 数据库查询性能 | 事件列表查询 < 200ms |

### 14.3 代码质量验收

| # | 验收项 | 标准 |
|---|--------|------|
| 1 | 编译零警告 | 所有项目 `dotnet build` 无警告 |
| 2 | 单元测试覆盖 | Mock 适配器核心方法 ≥ 80% 覆盖 |
| 3 | 代码风格 | 符合项目现有命名与编码规范 |
| 4 | 无硬编码 | 所有配置走 appsettings.json |

---

## 十五、数据库迁移计划

### 15.1 新增表

#### 15.1.1 erp_work_order_mapping

```sql
CREATE TABLE IF NOT EXISTS `erp_work_order_mapping` (
    `id` BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '自增主键',
    `mes_work_order_id` VARCHAR(50) NOT NULL COMMENT 'MES工单号',
    `erp_order_id` VARCHAR(100) NULL COMMENT 'ERP返回的订单号',
    `sync_status` VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT '同步状态: Pending/Success/Failed',
    `last_sync_at` DATETIME NULL COMMENT '最后同步时间',
    `sync_payload` JSON NULL COMMENT '最后发送的payload',
    `error_message` TEXT NULL COMMENT '错误信息',
    `created_at` DATETIME NOT NULL DEFAULT NOW() COMMENT '创建时间',
    `updated_at` DATETIME NOT NULL DEFAULT NOW() ON UPDATE NOW() COMMENT '更新时间',
    INDEX `idx_mes_wo` (`mes_work_order_id`),
    INDEX `idx_erp_order` (`erp_order_id`),
    INDEX `idx_sync_status` (`sync_status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='工单ERP同步映射表';
```

#### 15.1.2 eap_recipe_download_log

```sql
CREATE TABLE IF NOT EXISTS `eap_recipe_download_log` (
    `id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `equipment_id` VARCHAR(50) NOT NULL COMMENT '设备编号',
    `recipe_id` VARCHAR(100) NOT NULL COMMENT '配方编号',
    `download_status` VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT '下发状态',
    `verify_result` VARCHAR(50) NULL COMMENT '验证结果',
    `downloaded_at` DATETIME NULL COMMENT '下发时间',
    `error_message` TEXT NULL,
    `created_at` DATETIME NOT NULL DEFAULT NOW(),
    INDEX `idx_equipment` (`equipment_id`),
    INDEX `idx_recipe` (`recipe_id`),
    INDEX `idx_status` (`download_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='EAP配方下发日志';
```

#### 15.1.3 wms_inout_order

```sql
CREATE TABLE IF NOT EXISTS `wms_inout_order` (
    `order_no` VARCHAR(50) PRIMARY KEY COMMENT '指令编号',
    `order_type` VARCHAR(20) NOT NULL COMMENT '类型: Inbound/Outbound',
    `material_id` VARCHAR(50) NOT NULL COMMENT '物料编号',
    `quantity` DECIMAL(10,2) NOT NULL COMMENT '数量',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT '状态',
    `wms_confirm_no` VARCHAR(100) NULL COMMENT 'WMS确认号',
    `error_message` TEXT NULL,
    `created_at` DATETIME NOT NULL DEFAULT NOW(),
    `updated_at` DATETIME NOT NULL DEFAULT NOW() ON UPDATE NOW(),
    INDEX `idx_type` (`order_type`),
    INDEX `idx_status` (`status`),
    INDEX `idx_material` (`material_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='WMS入出库指令';
```

#### 15.1.4 oa_approval_tracking

```sql
CREATE TABLE IF NOT EXISTS `oa_approval_tracking` (
    `approval_id` VARCHAR(50) PRIMARY KEY COMMENT 'OA流程号',
    `approval_type` VARCHAR(20) NOT NULL COMMENT '审批类型: ECN/Scrap/Rework/Qualification',
    `entity_id` VARCHAR(50) NOT NULL COMMENT '关联实体ID',
    `entity_type` VARCHAR(50) NOT NULL COMMENT '关联实体类型',
    `approval_status` VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT '审批状态',
    `submitted_at` DATETIME NOT NULL COMMENT '提交时间',
    `approved_at` DATETIME NULL COMMENT '审批完成时间',
    `approver` VARCHAR(50) NULL COMMENT '审批人',
    `approval_comment` TEXT NULL COMMENT '审批意见',
    `error_message` TEXT NULL,
    `created_at` DATETIME NOT NULL DEFAULT NOW(),
    INDEX `idx_type` (`approval_type`),
    INDEX `idx_status` (`approval_status`),
    INDEX `idx_entity` (`entity_type`, `entity_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='OA审批跟踪表';
```

### 15.2 ext_system_config 初始数据

```sql
-- ERP 系统配置
INSERT INTO `ext_system_config` (`system_id`, `system_name`, `system_type`, `endpoint`, `auth_type`, `is_enabled`, `timeout_seconds`, `max_retries`, `subscribed_events`, `created_at`)
VALUES ('ERP-001', 'SAP ERP', 'ERP', 'https://sap-dev.internal.local:8443/odata', 'Basic', TRUE, 30, 3, '["WorkOrder.Created","WorkOrder.Completed","BOM.Updated","Shipping.Confirmed"]', NOW())
ON DUPLICATE KEY UPDATE `system_name` = VALUES(`system_name`);

-- EAP 系统配置
INSERT INTO `ext_system_config` (`system_id`, `system_name`, `system_type`, `endpoint`, `auth_type`, `is_enabled`, `timeout_seconds`, `max_retries`, `subscribed_events`, `created_at`)
VALUES ('EAP-001', 'Equipment Automation', 'EAP', 'https://eap-dev.internal.local:9000/api', 'ApiKey', TRUE, 15, 5, '["Recipe.Download","Alarm.Upload"]', NOW())
ON DUPLICATE KEY UPDATE `system_name` = VALUES(`system_name`);

-- WMS 系统配置
INSERT INTO `ext_system_config` (`system_id`, `system_name`, `system_type`, `endpoint`, `auth_type`, `is_enabled`, `timeout_seconds`, `max_retries`, `subscribed_events`, `created_at`)
VALUES ('WMS-001', 'Warehouse Management', 'WMS', 'https://wms-dev.internal.local:8080/api', 'Bearer', TRUE, 30, 3, '["Inbound.Confirmed","Outbound.Requested"]', NOW())
ON DUPLICATE KEY UPDATE `system_name` = VALUES(`system_name`);

-- QMS 系统配置
INSERT INTO `ext_system_config` (`system_id`, `system_name`, `system_type`, `endpoint`, `auth_type`, `is_enabled`, `timeout_seconds`, `max_retries`, `subscribed_events`, `created_at`)
VALUES ('QMS-001', 'Quality Management', 'QMS', 'https://qms-dev.internal.local:8080/api', 'Bearer', TRUE, 30, 3, '["Inspection.Completed","SPC.Measured","Complaint.8D_Submitted","NCR.Updated"]', NOW())
ON DUPLICATE KEY UPDATE `system_name` = VALUES(`system_name`);

-- OA 系统配置
INSERT INTO `ext_system_config` (`system_id`, `system_name`, `system_type`, `endpoint`, `auth_type`, `is_enabled`, `timeout_seconds`, `max_retries`, `subscribed_events`, `created_at`)
VALUES ('OA-001', 'Office Automation', 'OA', 'https://oa-dev.internal.local:8080/api', 'OAuth2', TRUE, 60, 3, '["ECN.Submitted","Scrap.Requested","Rework.Requested"]', NOW())
ON DUPLICATE KEY UPDATE `system_name` = VALUES(`system_name`);

-- 客户门户配置
INSERT INTO `ext_system_config` (`system_id`, `system_name`, `system_type`, `endpoint`, `auth_type`, `is_enabled`, `timeout_seconds`, `max_retries`, `subscribed_events`, `created_at`)
VALUES ('PORTAL-001', 'Customer Portal', 'CustomerPortal', 'https://portal-dev.internal.local:443/api', 'OAuth2', TRUE, 30, 3, '["Shipping.Completed"]', NOW())
ON DUPLICATE KEY UPDATE `system_name` = VALUES(`system_name`);
```

### 15.3 Entity 定义（新增）

需要在 `SystemEntities.cs` 中新增以下 Entity 类：

```csharp
// 文件：src/Infrastructure/MES.Infrastructure.Persistence/Entities/IntegrationEntities.cs

public class ErpWorkOrderMapping
{
    public long Id { get; set; }
    public string MesWorkOrderId { get; set; } = string.Empty;
    public string? ErpOrderId { get; set; }
    public string SyncStatus { get; set; } = "Pending";
    public DateTime? LastSyncAt { get; set; }
    public string? SyncPayload { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class EapRecipeDownloadLog
{
    public long Id { get; set; }
    public string EquipmentId { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public string DownloadStatus { get; set; } = "Pending";
    public string? VerifyResult { get; set; }
    public DateTime? DownloadedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class WmsInOutOrder
{
    public string OrderNo { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty; // Inbound/Outbound
    public string MaterialId { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Status { get; set; } = "Pending";
    public string? WmsConfirmNo { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class OaApprovalTracking
{
    public string ApprovalId { get; set; } = string.Empty;
    public string ApprovalType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string ApprovalStatus { get; set; } = "Pending";
    public DateTime SubmittedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Approver { get; set; }
    public string? ApprovalComment { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### 15.4 MesDbContext 注册

在 `MesDbContext.cs` 中添加：

```csharp
// Integration tables (Phase 4)
public DbSet<ErpWorkOrderMapping> ErpWorkOrderMappings => Set<ErpWorkOrderMapping>();
public DbSet<EapRecipeDownloadLog> EapRecipeDownloadLogs => Set<EapRecipeDownloadLog>();
public DbSet<WmsInOutOrder> WmsInOutOrders => Set<WmsInOutOrder>();
public DbSet<OaApprovalTracking> OaApprovalTrackings => Set<OaApprovalTracking>();
```

---

## 附录 A：完整文件清单

### 新增文件

| 文件路径 | 类型 | 说明 |
|----------|------|------|
| `src/Server/Adapters/MES.Adapters.Abstractions/MES.Adapters.Abstractions.csproj` | 项目文件 | 适配器接口层 |
| `src/Server/Adapters/MES.Adapters.Abstractions/IMesErpAdapter.cs` | C# 接口 | ERP 适配器接口（8 方法） |
| `src/Server/Adapters/MES.Adapters.Abstractions/IMesEapAdapter.cs` | C# 接口 | EAP 适配器接口（8 方法） |
| `src/Server/Adapters/MES.Adapters.Abstractions/IMesWmsAdapter.cs` | C# 接口 | WMS 适配器接口（6 方法） |
| `src/Server/Adapters/MES.Adapters.Abstractions/IMesQmsAdapter.cs` | C# 接口 | QMS 适配器接口（7 方法） |
| `src/Server/Adapters/MES.Adapters.Abstractions/IMesOaAdapter.cs` | C# 接口 | OA 适配器接口（5 方法） |
| `src/Server/Adapters/MES.Adapters.Abstractions/IMesCustomerPortalAdapter.cs` | C# 接口 | 客户门户接口（6 方法） |
| `src/Server/Adapters/MES.Adapters.Abstractions/IEventDispatcher.cs` | C# 接口 | 事件分发器接口 |
| `src/Server/Adapters/MES.Adapters.Abstractions/AdapterResult.cs` | C# 类 | 统一返回类型 |
| `src/Server/Adapters/MES.Adapters.Abstractions/AdapterException.cs` | C# 类 | 适配器异常 |
| `src/Server/Adapters/MES.Adapters.Mock/MES.Adapters.Mock.csproj` | 项目文件 | Mock 实现层 |
| `src/Server/Adapters/MES.Adapters.Mock/MockErpAdapter.cs` | C# 类 | ERP Mock 实现 |
| `src/Server/Adapters/MES.Adapters.Mock/MockEapAdapter.cs` | C# 类 | EAP Mock 实现 |
| `src/Server/Adapters/MES.Adapters.Mock/MockWmsAdapter.cs` | C# 类 | WMS Mock 实现 |
| `src/Server/Adapters/MES.Adapters.Mock/MockQmsAdapter.cs` | C# 类 | QMS Mock 实现 |
| `src/Server/Adapters/MES.Adapters.Mock/MockOaAdapter.cs` | C# 类 | OA Mock 实现 |
| `src/Server/Adapters/MES.Adapters.Mock/MockCustomerPortalAdapter.cs` | C# 类 | 客户门户 Mock 实现 |
| `src/Server/Adapters/MES.Adapters.Mock/MockHelper.cs` | C# 类 | 通用模拟工具 |
| `src/Server/Adapters/MES.Adapters.Mock/MockData/*.json` (14 files) | JSON | Mock 测试数据 |
| `src/Server/Adapters/MES.Adapters.Real/MES.Adapters.Real.csproj` | 项目文件 | 真实对接预留 |
| `src/Server/Adapters/MES.Adapters.Real/README.md` | 文档 | 对接指南 |
| `src/Server/MES.Api/Controllers/IntegrationController.cs` | C# 控制器 | 集成管理 API |
| `src/Server/MES.Api/Controllers/ExtSystemConfigController.cs` | C# 控制器 | 系统配置 API |
| `src/Server/MES.Api/Controllers/ExtSystemEventController.cs` | C# 控制器 | 事件管理 API |
| `src/Server/MES.Api/Services/EventDispatcherService.cs` | C# 服务 | 事件分发器实现 |
| `src/Server/MES.Api/Services/EventDispatcherBackgroundService.cs` | C# 服务 | 后台定时任务 |
| `src/Server/MES.Api/Configuration/IntegrationServiceCollectionExtensions.cs` | C# 扩展 | DI 注册扩展 |
| `src/Infrastructure/MES.Infrastructure.Persistence/Entities/IntegrationEntities.cs` | C# 实体 | 集成表实体 |

### 修改文件

| 文件路径 | 修改内容 |
|----------|----------|
| `src/Server/MES.Api/MES.Api.csproj` | 添加 Adapters.Abstractions、Adapters.Mock 项目引用 |
| `src/Server/MES.Api/Program.cs` | 添加 `builder.Services.AddIntegrationServices()` 和后台服务注册 |
| `src/Server/MES.Api/appsettings.json` | 添加 `Integration` 和 `EventDispatcher` 配置节 |
| `src/Infrastructure/MES.Infrastructure.Persistence/MesDbContext.cs` | 添加 4 个新 DbSet 和对应 ModelBuilder 配置 |
| `src/Client/MES.Modules.Production/Views/ExternalSystemView.xaml` | 扩展为 8 Tab 结构，添加 6 个系统面板 |
| `src/Client/MES.Modules.Production/ViewModels/ExternalSystemViewModel.cs` | 新增 6 个系统的数据属性和命令 |

### 统计

- **新增项目：** 3 个（Abstractions, Mock, Real）
- **新增接口：** 7 个（6 适配器 + 1 事件分发器）
- **新增实现：** 7 个（6 Mock + 1 EventDispatcher）
- **新增控制器：** 3 个
- **新增服务：** 2 个
- **新增 Entity：** 4 个
- **新增数据库表：** 4 个
- **新增 Mock 数据文件：** 14 个
- **新增/修改 UI 文件：** 2 个（XAML + ViewModel）
- **修改现有文件：** 4 个
- **总计新增文件：** ~35 个

---

## 附录 B：API 端点汇总

### B.1 IntegrationController — `/api/integration`

| 方法 | 端点 | 说明 | 请求体 |
|------|------|------|--------|
| POST | `/api/integration/test-connection/{systemType}` | 测试指定系统连接 | 无 |
| POST | `/api/integration/test-connection/all` | 测试所有系统连接 | 无 |
| POST | `/api/integration/sync/work-order/{orderId}` | 手动触发工单同步 | 无 |
| POST | `/api/integration/sync/bom/{productId}` | 手动触发 BOM 同步 | 无 |
| POST | `/api/integration/reconcile-inventory` | 执行库存对账 | `{ "materialId": "..." }` |
| POST | `/api/integration/process-events` | 手动触发事件处理 | `{ "maxBatch": 50 }` |
| POST | `/api/integration/retry-failed` | 手动重试失败事件 | `{ "maxRetries": 5 }` |
| POST | `/api/integration/retry-event/{eventId}` | 重发指定事件 | 无 |
| GET | `/api/integration/status` | 获取所有集成系统状态 | 无 |
| GET | `/api/integration/config` | 获取集成配置信息 | 无 |

### B.2 ExtSystemConfigController — `/api/ext-system-config`

| 方法 | 端点 | 说明 | 请求体 |
|------|------|------|--------|
| GET | `/api/ext-system-config` | 获取所有系统配置 | 无 |
| GET | `/api/ext-system-config/{systemId}` | 获取指定系统配置 | 无 |
| POST | `/api/ext-system-config` | 创建系统配置 | `ExtSystemConfigDto` |
| PUT | `/api/ext-system-config/{systemId}` | 更新系统配置 | `ExtSystemConfigDto` |
| DELETE | `/api/ext-system-config/{systemId}` | 删除系统配置 | 无 |
| PUT | `/api/ext-system-config/{systemId}/toggle` | 启用/禁用系统 | 无 |

### B.3 ExtSystemEventController — `/api/ext-system-event`

| 方法 | 端点 | 说明 | 查询参数 |
|------|------|------|----------|
| GET | `/api/ext-system-event` | 查询事件列表 | `status`, `targetSystem`, `pageIndex`, `pageSize` |
| GET | `/api/ext-system-event/{eventId}` | 获取事件详情 | 无 |
| POST | `/api/ext-system-event/{eventId}/retry` | 重试单个事件 | 无 |
| POST | `/api/ext-system-event/retry-failed` | 批量重试失败事件 | `{ "maxRetries": 5 }` |
| DELETE | `/api/ext-system-event/clean` | 清理已完成事件 | `olderThanDays=30` |
| GET | `/api/ext-system-event/stats` | 获取事件统计 | 无 |

---

## 附录 C：事件类型注册表

| 事件类型 | 目标系统 | 触发场景 | Payload 关键字段 |
|----------|----------|----------|------------------|
| `WorkOrder.Created` | ERP | 工单创建 | `orderId`, `productId`, `plannedQty`, `customerId` |
| `WorkOrder.Completed` | ERP | 工单完工 | `orderId`, `completedQty`, `scrapQty`, `completedAt` |
| `BOM.Updated` | ERP | 工艺路线变更 | `productId`, `bomVersion`, `items[]` |
| `Shipping.Confirmed` | ERP | 成品发货 | `shippingId`, `orderId`, `items[]`, `shipDate` |
| `Recipe.Download` | EAP | 设备 Track-In | `equipmentId`, `recipeId`, `parameters{}` |
| `Alarm.Upload` | EAP | 设备报警 | `equipmentId`, `alarmCode`, `severity`, `message` |
| `Inbound.Confirmed` | WMS | 物料入库 | `materialId`, `batchNo`, `quantity`, `warehouse` |
| `Outbound.Requested` | WMS | 工单领料 | `workOrderId`, `materialId`, `quantity` |
| `Inspection.Completed` | QMS | 检验完成 | `inspectionId`, `lotId`, `stepCode`, `result`, `items[]` |
| `SPC.Measured` | QMS | SPC 测量 | `lotId`, `stepCode`, `parameterName`, `measuredValue` |
| `Complaint.8D_Submitted` | QMS | 8D 报告提交 | `complaintId`, `customerId`, `d1~d8{}` |
| `NCR.Updated` | QMS | 不合格品状态变更 | `ncrId`, `status`, `disposition` |
| `ECN.Submitted` | OA | ECN 提交审批 | `ecnId`, `title`, `type`, `affectedProducts[]` |
| `Scrap.Requested` | OA | 报废审批 | `scrapId`, `lotId`, `qty`, `reason` |
| `Rework.Requested` | OA | 返工审批 | `reworkId`, `lotId`, `reason` |
| `Shipping.Completed` | CustomerPortal | 发货完成通知 | `orderId`, `trackingNo`, `shipDate` |

---

> **文档结束**
>
> 本计划为 Phase 4 完整实施指南。所有接口定义、Mock 策略、数据表设计、UI 布局、API 端点均已明确。实施时按 4.1 → 4.7 顺序推进，每个阶段完成后进行代码审查和测试验证。
