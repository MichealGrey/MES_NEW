# MES 优化改善计划 - 阶段二：订单管理与生产计划

> **文档版本**: v1.0  
> **创建日期**: 2026-06-06  
> **状态**: 生产就绪  
> **目标模块**: `MES.Modules.Order` + `MES.Modules.Schedule`

---

## 目录

1. [阶段二概述](#1-阶段二概述)
2. [现有 UI 评估](#2-现有-ui-评估)
3. [订单评审模块](#3-订单评审模块)
4. [主生产计划与产能评估](#4-主生产计划与产能评估)
5. [工单分解与拆分](#5-工单分解与拆分)
6. [物料需求计划(MRP)](#6-物料需求计划mrp)
7. [订单进度跟踪](#7-订单进度跟踪)
8. [准时交付率(OTD)](#8-准时交付率otd)
9. [急单处理](#9-急单处理)
10. [UI 优化方案汇总](#10-ui-优化方案汇总)
11. [实施顺序与依赖关系](#11-实施顺序与依赖关系)
12. [验收标准](#12-验收标准)
13. [数据库迁移计划](#13-数据库迁移计划)

---

## 1. 阶段二概述

### 1.1 目标
构建完整的订单生命周期管理体系，覆盖从订单评审、创建、分解、排产、进度跟踪到交付的全流程。核心解决以下问题：
- 订单未经评审直接下达导致的生产异常
- 产能不足时盲目接单导致延期交付
- 工单无法按工艺阶段拆分管理
- 物料需求无法自动计算
- 订单进度不透明
- 缺乏交付率度量
- 急单插入缺乏影响评估

### 1.2 范围
- **包含**: 订单评审、主生产计划、工单分解、MRP、订单进度跟踪、OTD、急单处理
- **排除**: 库存管理细节（引用现有库存模块）、设备排程算法（阶段三）、财务结算（后续阶段）

### 1.3 技术栈
- **前端**: WPF + Prism MVVM + XAML
- **后端**: ASP.NET Core RESTful API
- **数据库**: MySQL (InnoDB, utf8mb4)
- **通信**: HTTP REST + 可选 WebSocket 实时推送

---

## 2. 现有 UI 评估

### 2.1 WorkOrderListView（工单列表）

**文件**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\Views\WorkOrderListView.xaml`  
**ViewModel**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\ViewModels\WorkOrderListViewModel.cs`

| 维度 | 当前状态 | 缺失功能 |
|------|----------|----------|
| **标题** | ✅ "工单管理" | — |
| **KPI 卡片** | ✅ 5 张(总数/待下达/进行中/已完成/Hold) | 缺少：待评审数、紧急工单数 |
| **筛选栏** | ✅ 状态/优先级/日期范围/搜索 | 缺少：客户/区域筛选 |
| **DataGrid** | ✅ 15 列（工单号、产品、管芯、封装、工艺路线、计划量、完成量、进度条、状态徽章、优先级徽章、客户、客户料号、区域、Hold原因、计划起止） | 缺少：父子工单层级、齐套状态、评审状态 |
| **操作按钮** | ✅ 创建/下达/Hold/解挂/关闭/取消/刷新 | 缺少：评审/拆分/查看子工单 |
| **分页** | ✅ 完整分页控件 | — |
| **架构** | ✅ Prism ViewModelLocator.AutoWireViewModel | ViewModel 使用 IProductionDataService（本地模拟），需对接后端 API |

**优化建议**:
1. KPI 卡片从 5 张扩展到 7 张，新增"待评审"和"紧急工单"
2. DataGrid 新增列：`ParentOrderId`（父工单号，缩进显示层级）、`KitStatus`（齐套状态）、`ReviewStatus`（评审状态）
3. 工具栏新增按钮："订单评审"、"工单拆分"
4. 筛选栏新增"客户"ComboBox 和"区域"ComboBox
5. ViewModel 新增 `IsParentOrder`、`KitStatus`、`ReviewStatus` 属性

### 2.2 CustomerProgressView（客户订单进度）

**文件**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\Views\CustomerProgressView.xaml`（推测在 Order 模块 Views 目录下）

**现状评估**: 需检查是否存在，若存在则评估完整度；若为占位符则需完全开发。

**功能需求**:
- 按客户维度展示所有关联工单的交付进度
- 订单时间轴（创建→评审→排产→生产→完工→交付）
- 延期预警（红色标记距交期<3天的订单）
- 客户料号与我方工单号的映射关系

### 2.3 MrpView（物料需求计划）

**文件**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Schedule\Views\MrpView.xaml`

**现状**: ❌ 完全占位符
```xml
<TextBlock Text="排程调度 - 物料需求计划 (开发中...)"/>
```

**ViewModel**: `AllScheduleViewModels.cs` 中的 `MrpViewModel`
- 当前依赖 `IScheduleService.GetMrpDataAsync()`，返回 `MrpItem` 列表
- 已有属性: `Items`、`ShortageCount`、`TotalMaterials`、`TotalShortageQty`、`ShowShortageOnly`

**需改造为**: 完整功能界面，包含 KPI 卡片、物料需求表格、运算按钮、短缺预警

### 2.4 DeliveryManageView（交付管理）

**文件**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Schedule\Views\DeliveryManageView.xaml`

**现状**: ❌ 完全占位符
```xml
<TextBlock Text="排程调度 - 交付管理 (开发中...)"/>
```

**ViewModel**: `AllScheduleViewModels.cs` 中的 `DeliveryManageViewModel`
- 当前依赖 `IScheduleService.GetDeliveryRecordsAsync()`，返回 `DeliveryRecord` 列表
- 已有属性: `Items`、`TotalDeliveries`、`PendingCount`、`CompletedCount`、`TotalPlanQty`、`TotalDeliverQty`

**需改造为**: 完整功能界面，包含交付看板、OTD 指标、交付明细表、导出功能

### 2.5 WorkOrderScheduleView（工单排程）

**文件**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Schedule\Views\WorkOrderScheduleView.xaml`（推测存在）

**现状**: 需检查。若为占位符则需改造。

**ViewModel**: `AllScheduleViewModels.cs` 中的 `WorkOrderScheduleViewModel`
- 已有属性: `Items`、`DelayedCount`、`TotalOrders`、`CompletedOrders`、`InProgressOrders`、`AvgProgress`

---

## 3. 订单评审模块

### 3.1 现状分析

当前系统工单创建后可以直接下达（`Created → Released`），缺少评审环节。这导致：
- 技术可行性未评估
- 产能冲突未检查
- 物料齐套性未确认
- 特殊要求未传递到生产

### 3.2 功能清单

| 编号 | 功能 | 优先级 |
|------|------|--------|
| OR-01 | 创建评审任务（自动/手动） | P0 |
| OR-02 | 多部门会签（工艺/质量/生产） | P0 |
| OR-03 | 评审结论（通过/驳回/条件通过） | P0 |
| OR-04 | 评审超时自动跳过 | P1 |
| OR-05 | 评审历史追溯 | P1 |
| OR-06 | 急单绿色通道（跳过评审） | P1 |

### 3.3 接口设计

**文件**: `e:\AiProj\MES_NEW\src\Server\Modules\MES.Services.OrderMgmt\IOrderReviewService.cs`

```csharp
public interface IOrderReviewService
{
    /// <summary>创建评审任务</summary>
    Task<ReviewTaskResponse> CreateTaskAsync(CreateReviewTaskRequest request);

    /// <summary>查询评审任务列表</summary>
    Task<PagedResult<ReviewTaskResponse>> GetTasksAsync(ReviewTaskQuery query);

    /// <summary>获取评审任务详情</summary>
    Task<ReviewTaskDetailResponse> GetTaskDetailAsync(string taskId);

    /// <summary>部门投票</summary>
    Task<ReviewVoteResponse> VoteAsync(string taskId, ReviewVoteRequest request);

    /// <summary>提交评审结论</summary>
    Task<ReviewTaskResponse> SubmitAsync(string taskId, string operatorId);

    /// <summary>获取订单评审历史</summary>
    Task<List<ReviewHistoryResponse>> GetReviewHistoryAsync(string orderId);
}
```

**Controller**: `e:\AiProj\MES_NEW\src\Server\Modules\MES.Services.OrderMgmt\Controllers\OrderReviewController.cs`

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/order-review/tasks` | 创建评审任务 |
| GET | `/api/order-review/tasks` | 查询评审任务列表 |
| GET | `/api/order-review/tasks/{taskId}` | 获取评审详情 |
| POST | `/api/order-review/tasks/{taskId}/vote` | 部门投票 |
| POST | `/api/order-review/tasks/{taskId}/submit` | 提交评审结论 |
| GET | `/api/order-review/orders/{orderId}/history` | 评审历史 |

**Request/Response 类型**:

```csharp
// MES.Contracts/OrderReview/ReviewDto.cs

public class CreateReviewTaskRequest
{
    public string OrderId { get; set; } = null!;
    public string ReviewType { get; set; } = "Standard"; // Standard/Expedited/Rush
    public string InitiatorId { get; set; } = null!;
    public string? Remark { get; set; }
    public List<string> ReviewDepartments { get; set; } = ["Process", "Quality", "Production"];
}

public class ReviewVoteRequest
{
    public string Department { get; set; } = null!; // Process/Quality/Production
    public string VoteResult { get; set; } = null!; // Approve/Reject/Conditional
    public string? Comments { get; set; }
    public string? Conditions { get; set; } // 条件通过时的附加条件
}

public class ReviewTaskResponse
{
    public string TaskId { get; set; } = null!;
    public string OrderId { get; set; } = null!;
    public string ReviewType { get; set; } = null!;
    public string Status { get; set; } = null!; // Pending/InProgress/Approved/Rejected/Expired/Bypassed
    public string? ProcessResult { get; set; }
    public string? QualityResult { get; set; }
    public string? ProductionResult { get; set; }
    public string? FinalResult { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class ReviewTaskDetailResponse : ReviewTaskResponse
{
    public List<ReviewVoteRecord> Votes { get; set; } = [];
    public string? BypassReason { get; set; }
    public string? Remark { get; set; }
}

public class ReviewVoteRecord
{
    public string Department { get; set; } = null!;
    public string VoteResult { get; set; } = null!;
    public string? Comments { get; set; }
    public string? Conditions { get; set; }
    public string? VoterId { get; set; }
    public DateTime? VotedAt { get; set; }
}

public class ReviewTaskQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
    public string? ReviewType { get; set; }
    public string? OrderId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}
```

### 3.4 数据表设计

**文件**: `e:\AiProj\MES_NEW\docs\sql\migrations\V5.0.0__order_review.sql`

```sql
-- 评审任务主表
CREATE TABLE IF NOT EXISTS `order_review_task` (
    `task_id` VARCHAR(50) NOT NULL COMMENT '格式: ORT-YYYYMMDD-NNN',
    `order_id` VARCHAR(50) NOT NULL,
    `review_type` VARCHAR(20) NOT NULL DEFAULT 'Standard' COMMENT 'Standard/Expedited/Rush',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT 'Pending/InProgress/Approved/Rejected/Expired/Bypassed',
    `process_status` VARCHAR(20) DEFAULT 'NotStarted' COMMENT 'NotStarted/Approved/Rejected/ConditionalApproved',
    `quality_status` VARCHAR(20) DEFAULT 'NotStarted',
    `production_status` VARCHAR(20) DEFAULT 'NotStarted',
    `final_result` VARCHAR(20) DEFAULT NULL COMMENT 'Approved/Rejected',
    `initiator_id` VARCHAR(50) NOT NULL,
    `remark` TEXT,
    `bypass_reason` VARCHAR(500),
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `completed_at` DATETIME DEFAULT NULL,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`task_id`),
    UNIQUE KEY `uk_order_review` (`order_id`),
    KEY `idx_status` (`status`),
    KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 评审投票记录表
CREATE TABLE IF NOT EXISTS `order_review_vote` (
    `vote_id` BIGINT AUTO_INCREMENT,
    `task_id` VARCHAR(50) NOT NULL,
    `department` VARCHAR(20) NOT NULL COMMENT 'Process/Quality/Production',
    `vote_result` VARCHAR(20) NOT NULL COMMENT 'Approve/Reject/Conditional',
    `comments` TEXT,
    `conditions` TEXT COMMENT '条件通过时的附加条件',
    `voter_id` VARCHAR(50),
    `voted_at` DATETIME,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`vote_id`),
    UNIQUE KEY `uk_task_dept` (`task_id`, `department`),
    KEY `idx_task` (`task_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 评审跳过规则表
CREATE TABLE IF NOT EXISTS `order_review_bypass_rule` (
    `rule_id` INT AUTO_INCREMENT,
    `condition_type` VARCHAR(20) NOT NULL COMMENT 'ProductFamiliarity/CustomerLevel/OrderQty',
    `condition_value` VARCHAR(100) NOT NULL,
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`rule_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 初始化默认跳过规则
INSERT INTO `order_review_bypass_rule` (`condition_type`, `condition_value`) VALUES
('ProductFamiliarity', 'Mature'),      -- 成熟产品跳过评审
('CustomerLevel', 'VIP'),              -- VIP客户急单跳过评审
('OrderQty', '<=100');                 -- 小批量订单跳过评审
```

### 3.5 UI 设计建议

**新增视图**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\Views\OrderReviewView.xaml`

```
┌─────────────────────────────────────────────────────────────┐
│  订单评审管理                              [新建评审] [刷新] │
├─────────────────────────────────────────────────────────────┤
│  KPI: 待评审(12) | 评审中(5) | 已通过(89) | 已驳回(3) | 已跳过(7) │
├─────────────────────────────────────────────────────────────┤
│  筛选: [状态▼] [评审类型▼] [日期范围] [订单号搜索] [清除]      │
├─────────────────────────────────────────────────────────────┤
│  任务ID      │ 订单号   │ 类型    │ 工艺  │ 质量  │ 生产  │ 结果   │ 时间    │
│  ORT-20260601│ WO-001   │ Standard│ ✓    │ ✓    │ ✓    │ 通过   │ 06-01   │
│  ORT-20260603│ WO-003   │ Standard│ ✓    │ ✗    │ —    │ 驳回   │ 06-03   │
│  ORT-20260605│ WO-005   │ Expedited│ ◐   │ —    │ —    │ 评审中 │ 06-05   │
├─────────────────────────────────────────────────────────────┤
│  [<] 第 1/5 页 (共 107 条) [>]                              │
└─────────────────────────────────────────────────────────────┘
```

**ViewModel**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\ViewModels\OrderReviewViewModel.cs`

```csharp
public class OrderReviewViewModel : BindableBase
{
    // KPI 属性
    public int PendingCount { get; }       // 待评审
    public int InProgressCount { get; }    // 评审中
    public int ApprovedCount { get; }      // 已通过
    public int RejectedCount { get; }      // 已驳回
    public int BypassedCount { get; }      // 已跳过

    // 列表属性
    public ObservableCollection<ReviewTaskResponse> Tasks { get; set; }
    public ReviewTaskResponse? SelectedTask { get; set; }

    // 筛选属性
    public string? FilterStatus { get; set; }
    public string? FilterReviewType { get; set; }
    public string SearchOrderId { get; set; }
    public DateTime? FilterDateFrom { get; set; }
    public DateTime? FilterDateTo { get; set; }

    // 分页属性
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    // 命令
    public DelegateCommand CreateTaskCommand { get; }
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand<ReviewTaskResponse> ViewDetailCommand { get; }
    public DelegateCommand<ReviewTaskResponse> VoteCommand { get; }
    public DelegateCommand<ReviewTaskResponse> SubmitCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
}
```

### 3.6 业务规则

1. **自动触发评审**: 工单创建后，若不符合跳过规则，自动创建评审任务，工单状态变为 `PendingReview`
2. **评审通过**: 三个部门全部投票 Approve → 自动通过，工单状态变为 `Approved`
3. **评审驳回**: 任一部门投票 Reject → 自动驳回，工单状态变为 `Rejected`，需重新修改后提交
4. **条件通过**: 所有部门均为 Approve 或 Conditional → 标记为 `ConditionalApproved`，需附加条件确认
5. **超时跳过**: 评审类型为标准件，24 小时内未完成所有投票 → 自动通过（可配置）
6. **急单通道**: 评审类型为 Expedited/Rush 时，超时时间缩短为 4 小时/1 小时
7. **状态流转**: `PendingReview → Approved → Created → Released` 或 `PendingReview → Rejected`

---

## 4. 主生产计划与产能评估

### 4.1 现状分析

当前系统缺少主生产计划（MPS）模块，无法从宏观层面统筹生产资源。产能评估仅在 `CapacityAnalysisViewModel` 中展示简单的利用率数据，缺少与订单的联动。

### 4.2 功能清单

| 编号 | 功能 | 优先级 |
|------|------|--------|
| MPS-01 | 创建主生产计划（按周/月） | P0 |
| MPS-02 | 产能负荷分析 | P0 |
| MPS-03 | 产能冲突预警 | P0 |
| MPS-04 | 计划调整与版本管理 | P1 |
| MPS-05 | 产能-订单联动 | P1 |

### 4.3 接口设计

**文件**: `e:\AiProj\MES_NEW\src\Server\Modules\MES.Services.Production\IMasterPlanService.cs`

```csharp
public interface IMasterPlanService
{
    Task<MasterPlanResponse> CreatePlanAsync(CreateMasterPlanRequest request);
    Task<PagedResult<MasterPlanResponse>> GetPlansAsync(MasterPlanQuery query);
    Task<MasterPlanDetailResponse> GetPlanDetailAsync(string planId);
    Task<CapacityLoadResponse> AnalyzeCapacityAsync(CapacityLoadRequest request);
    Task<List<CapacityConflict>> DetectConflictsAsync(string planId);
    Task<MasterPlanResponse> AdjustPlanAsync(string planId, AdjustPlanRequest request);
    Task<List<MasterPlanVersion>> GetPlanVersionsAsync(string planId);
}
```

**Controller**: `/api/master-plan/*`

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/master-plan` | 创建主生产计划 |
| GET | `/api/master-plan` | 查询计划列表 |
| GET | `/api/master-plan/{planId}` | 获取计划详情 |
| POST | `/api/master-plan/analyze-capacity` | 产能负荷分析 |
| POST | `/api/master-plan/{planId}/detect-conflicts` | 冲突检测 |
| POST | `/api/master-plan/{planId}/adjust` | 调整计划 |
| GET | `/api/master-plan/{planId}/versions` | 版本历史 |

### 4.4 数据表设计

**文件**: `e:\AiProj\MES_NEW\docs\sql\migrations\V5.1.0__master_production_plan.sql`

```sql
-- 主生产计划主表
CREATE TABLE IF NOT EXISTS `master_production_plan` (
    `plan_id` VARCHAR(50) NOT NULL COMMENT '格式: MPS-YYYYMMDD-NNN',
    `plan_name` VARCHAR(200) NOT NULL,
    `plan_period` VARCHAR(20) NOT NULL COMMENT 'Weekly/Monthly/Quarterly',
    `period_start` DATE NOT NULL,
    `period_end` DATE NOT NULL,
    `version` INT NOT NULL DEFAULT 1,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Draft' COMMENT 'Draft/Submitted/Approved/Active/Archived',
    `total_capacity_hours` DECIMAL(12,2) NOT NULL DEFAULT 0,
    `planned_capacity_hours` DECIMAL(12,2) NOT NULL DEFAULT 0,
    `utilization_rate` DECIMAL(5,2) DEFAULT 0 COMMENT '百分比',
    `has_conflict` TINYINT(1) NOT NULL DEFAULT 0,
    `created_by` VARCHAR(50) NOT NULL,
    `approved_by` VARCHAR(50),
    `approved_at` DATETIME,
    `remark` TEXT,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`plan_id`),
    KEY `idx_period` (`period_start`, `period_end`),
    KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 主生产计划明细行（每个产品/产线的计划量）
CREATE TABLE IF NOT EXISTS `master_production_plan_line` (
    `line_id` BIGINT AUTO_INCREMENT,
    `plan_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `product_name` VARCHAR(200),
    `production_line` VARCHAR(50),
    `planned_qty` INT NOT NULL,
    `capacity_hours` DECIMAL(10,2) NOT NULL,
    `priority` INT NOT NULL DEFAULT 0,
    `order_ids` JSON COMMENT '关联的订单ID列表',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`line_id`),
    KEY `idx_plan` (`plan_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 产能负荷表（按工序/产线维度）
CREATE TABLE IF NOT EXISTS `capacity_load` (
    `load_id` BIGINT AUTO_INCREMENT,
    `plan_id` VARCHAR(50) NOT NULL,
    `resource_type` VARCHAR(20) NOT NULL COMMENT 'Line/Equipment/Process',
    `resource_id` VARCHAR(50) NOT NULL,
    `resource_name` VARCHAR(200),
    `available_hours` DECIMAL(10,2) NOT NULL,
    `required_hours` DECIMAL(10,2) NOT NULL,
    `load_rate` DECIMAL(5,2) NOT NULL COMMENT '百分比',
    `is_overloaded` TINYINT(1) NOT NULL DEFAULT 0,
    `overload_hours` DECIMAL(10,2) DEFAULT 0,
    `calculated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`load_id`),
    KEY `idx_plan` (`plan_id`),
    KEY `idx_resource` (`resource_type`, `resource_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 产能冲突记录
CREATE TABLE IF NOT EXISTS `capacity_conflict` (
    `conflict_id` BIGINT AUTO_INCREMENT,
    `plan_id` VARCHAR(50) NOT NULL,
    `conflict_type` VARCHAR(20) NOT NULL COMMENT 'CapacityOverload/MaterialShortage/RouteConflict',
    `severity` VARCHAR(20) NOT NULL COMMENT 'High/Medium/Low',
    `resource_id` VARCHAR(50),
    `description` TEXT NOT NULL,
    `suggested_action` TEXT,
    `is_resolved` TINYINT(1) NOT NULL DEFAULT 0,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`conflict_id`),
    KEY `idx_plan` (`plan_id`),
    KEY `idx_severity` (`severity`),
    KEY `idx_resolved` (`is_resolved`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 4.5 UI 设计建议

**新增视图**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Schedule\Views\MasterPlanView.xaml`

```
┌─────────────────────────────────────────────────────────────┐
│  主生产计划                    [新建计划] [产能分析] [刷新]  │
├─────────────────────────────────────────────────────────────┤
│  KPI: 计划总数(15) | 进行中(3) | 产能利用率(82%) | 冲突数(2) │
├─────────────────────────────────────────────────────────────┤
│  筛选: [状态▼] [计划周期▼] [日期范围] [搜索]                  │
├─────────────────────────────────────────────────────────────┤
│  计划ID    │ 名称       │ 周期    │ 起止日期    │ 利用率│ 状态  │
│  MPS-202606│ 6月W1计划  │ Weekly  │ 06-01~06-07 │ 78%  │ Active│
│  MPS-202606│ 6月月度计划│ Monthly │ 06-01~06-30 │ 95%⚠ │ Draft │
├─────────────────────────────────────────────────────────────┤
│  [甘特图/列表] 切换                                        │
│  ▓▓▓▓▓▓▓░░░ 产线A: 78%                                     │
│  ▓▓▓▓▓▓▓▓▓░ 产线B: 95% ⚠️                                  │
│  ▓▓▓▓▓░░░░░ 产线C: 52%                                     │
└─────────────────────────────────────────────────────────────┘
```

### 4.6 业务规则

1. **产能计算**: 计划产能 = 各工序标准工时 × 计划量，可用产能 = 产线工作小时 × 产线数量
2. **负荷率**: 负荷率 = 计划产能 / 可用产能 × 100%
3. **冲突判定**: 负荷率 > 90% 标记预警，> 100% 标记冲突
4. **版本管理**: 每次调整生成新版本，旧版本归档
5. **审批流程**: Draft → Submitted → Approved → Active

---

## 5. 工单分解与拆分

### 5.1 现状分析

当前 `ProdWorkOrder` 实体已有 `ParentOrderId` 和 `WoType` 字段，但缺少工单分解的业务逻辑和 UI。工单拆分后需要自动创建子工单并关联工艺路线阶段。

### 5.2 功能清单

| 编号 | 功能 | 优先级 |
|------|------|--------|
| WO-01 | 按工艺阶段分解工单 | P0 |
| WO-02 | 手动拆分工单（按数量） | P0 |
| WO-03 | 父子工单层级展示 | P0 |
| WO-04 | 子工单进度汇总 | P1 |
| WO-05 | 拆分规则模板 | P1 |

### 5.3 接口设计

**文件**: `e:\AiProj\MES_NEW\src\Server\Modules\MES.Services.Production\IWorkOrderDecomposeService.cs`

```csharp
public interface IWorkOrderDecomposeService
{
    /// <summary>按工艺阶段自动分解工单</summary>
    Task<DecomposeResult> DecomposeByProcessAsync(DecomposeByProcessRequest request);

    /// <summary>按数量手动拆分工单</summary>
    Task<SplitResult> SplitByQuantityAsync(SplitByQuantityRequest request);

    /// <summary>获取工单树形结构</summary>
    Task<WorkOrderTreeResponse> GetWorkOrderTreeAsync(string orderId);

    /// <summary>汇总子工单进度</summary>
    Task<ChildProgressSummary> GetChildProgressSummaryAsync(string parentOrderId);
}
```

**Controller**: `/api/work-orders/decompose/*`

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/work-orders/{orderId}/decompose` | 按工艺阶段分解 |
| POST | `/api/work-orders/{orderId}/split` | 按数量拆分 |
| GET | `/api/work-orders/{orderId}/tree` | 获取工单树 |
| GET | `/api/work-orders/{parentOrderId}/children` | 获取子工单列表 |
| GET | `/api/work-orders/{parentOrderId}/progress` | 子工单进度汇总 |

### 5.4 数据表设计

**扩展现有表**: `prod_work_order`

```sql
-- 在 prod_work_order 表中新增字段（若不存在）
ALTER TABLE `prod_work_order`
    ADD COLUMN IF NOT EXISTS `parent_order_id` VARCHAR(50) DEFAULT NULL COMMENT '父工单号',
    ADD COLUMN IF NOT EXISTS `wo_type` VARCHAR(20) DEFAULT 'Parent' COMMENT 'Parent/Child-Assemble/Child-Test/Child-Bump',
    ADD COLUMN IF NOT EXISTS `process_stage` VARCHAR(50) DEFAULT NULL COMMENT '关联的工艺阶段',
    ADD COLUMN IF NOT EXISTS `kit_status` VARCHAR(20) DEFAULT 'NotChecked' COMMENT 'NotChecked/Complete/Partial/Shortage',
    ADD COLUMN IF NOT EXISTS `review_status` VARCHAR(20) DEFAULT 'NotRequired' COMMENT 'NotRequired/Pending/Approved/Rejected/Bypassed',
    ADD COLUMN IF NOT EXISTS `decompose_rule_id` INT DEFAULT NULL COMMENT '使用的分解规则ID';

-- 工单分解规则表
CREATE TABLE IF NOT EXISTS `work_order_decompose_rule` (
    `rule_id` INT AUTO_INCREMENT,
    `rule_name` VARCHAR(200) NOT NULL,
    `product_type` VARCHAR(50) COMMENT '适用产品类型',
    `decompose_strategy` VARCHAR(20) NOT NULL COMMENT 'ByProcess/ByQuantity/ByLot',
    `stages` JSON NOT NULL COMMENT '分解后的阶段列表',
    `is_default` TINYINT(1) NOT NULL DEFAULT 0,
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`rule_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 工单分解记录
CREATE TABLE IF NOT EXISTS `work_order_decompose_record` (
    `record_id` BIGINT AUTO_INCREMENT,
    `parent_order_id` VARCHAR(50) NOT NULL,
    `child_order_ids` JSON NOT NULL COMMENT '子工单号列表',
    `decompose_strategy` VARCHAR(20) NOT NULL,
    `rule_id` INT,
    `operator_id` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`record_id`),
    KEY `idx_parent` (`parent_order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 5.5 UI 设计建议

**改造现有视图**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\Views\WorkOrderListView.xaml`

1. **DataGrid 新增列**:
   - 层级缩进列：根据 `ParentOrderId` 缩进显示，父工单显示 `▶` 展开图标
   - `WoType` 列：显示工单类型（父/子-装配/子-测试/子-凸块）
   - `ProcessStage` 列：显示工艺阶段
   - `KitStatus` 列：齐套状态徽章（齐套/部分/缺料）
   - `ReviewStatus` 列：评审状态徽章

2. **新增操作按钮**: "工单分解"、"工单拆分"

3. **新增弹窗**: `WorkOrderDecomposeDialog.xaml`
   - 选择分解策略（按工艺阶段/按数量）
   - 预览分解结果
   - 确认执行

### 5.6 业务规则

1. **自动分解**: 工单评审通过并下达时，根据产品类型的分解规则自动创建子工单
2. **子工单继承**: 子工单继承父工单的产品、客户、优先级等信息
3. **进度汇总**: 父工单进度 = 所有子工单完成量之和 / 父工单计划量
4. **齐套判定**: 子工单所需物料全部到位 → 齐套；部分到位 → 部分齐套；未开始检查 → 未检查
5. **拆分约束**: 拆分后子工单数量之和必须等于父工单数量

---

## 6. 物料需求计划(MRP)

### 6.1 现状分析

当前 `MrpView.xaml` 为占位符，`MrpViewModel` 使用模拟数据。需要对接实际物料库存和工单需求，实现自动物料需求计算。

### 6.2 功能清单

| 编号 | 功能 | 优先级 |
|------|------|--------|
| MRP-01 | MRP 运算（根据工单计划） | P0 |
| MRP-02 | 物料短缺预警 | P0 |
| MRP-03 | 齐套率统计 | P0 |
| MRP-04 | 物料需求明细 | P1 |
| MRP-05 | 采购建议生成 | P1 |

### 6.3 接口设计

**文件**: `e:\AiProj\MES_NEW\src\Server\Modules\MES.Services.Inventory\IMrpService.cs`

```csharp
public interface IMrpService
{
    /// <summary>执行 MRP 运算</summary>
    Task<MrpResult> RunMrpAsync(MrpRunRequest request);

    /// <summary>查询物料需求明细</summary>
    Task<PagedResult<MrpMaterialDetail>> GetMaterialDetailsAsync(MrpDetailQuery query);

    /// <summary>获取齐套率统计</summary>
    Task<KitRateSummary> GetKitRateSummaryAsync(string orderId);

    /// <summary>获取短缺物料列表</summary>
    Task<List<ShortageMaterial>> GetShortageMaterialsAsync(ShortageQuery query);

    /// <summary>生成采购建议</summary>
    Task<List<PurchaseSuggestion>> GeneratePurchaseSuggestionsAsync(string mrpRunId);
}
```

**Controller**: `/api/mrp/*`

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/mrp/run` | 执行 MRP 运算 |
| GET | `/api/mrp/runs` | 历史运算记录 |
| GET | `/api/mrp/materials` | 物料需求明细 |
| GET | `/api/mrp/kit-rate/{orderId}` | 齐套率 |
| GET | `/api/mrp/shortage` | 短缺物料 |
| POST | `/api/mrp/purchase-suggestions` | 采购建议 |

### 6.4 数据表设计

**文件**: `e:\AiProj\MES_NEW\docs\sql\migrations\V5.2.0__mrp.sql`

```sql
-- MRP 运算记录
CREATE TABLE IF NOT EXISTS `mrp_run_record` (
    `run_id` VARCHAR(50) NOT NULL COMMENT '格式: MRP-YYYYMMDD-NNN',
    `run_type` VARCHAR(20) NOT NULL DEFAULT 'Full' COMMENT 'Full/Incremental/OrderSpecific',
    `scope` JSON COMMENT '运算范围（工单列表/产品列表）',
    `total_materials` INT NOT NULL DEFAULT 0,
    `shortage_materials` INT NOT NULL DEFAULT 0,
    `kit_rate` DECIMAL(5,2) DEFAULT 0 COMMENT '齐套率百分比',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Completed',
    `operator_id` VARCHAR(50),
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`run_id`),
    KEY `idx_created` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 物料需求明细
CREATE TABLE IF NOT EXISTS `mrp_requirement_detail` (
    `detail_id` BIGINT AUTO_INCREMENT,
    `run_id` VARCHAR(50) NOT NULL,
    `material_code` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(200),
    `specification` VARCHAR(200),
    `unit` VARCHAR(20) NOT NULL,
    `required_qty` DECIMAL(14,4) NOT NULL,
    `available_stock` DECIMAL(14,4) NOT NULL,
    `in_transit_qty` DECIMAL(14,4) DEFAULT 0,
    `allocated_qty` DECIMAL(14,4) DEFAULT 0,
    `shortage_qty` DECIMAL(14,4) NOT NULL,
    `safety_stock` DECIMAL(14,4) DEFAULT 0,
    `shortage_status` VARCHAR(20) NOT NULL COMMENT 'Sufficient/Warning/Shortage/Critical',
    `expected_arrival` DATE,
    `related_orders` JSON COMMENT '关联工单',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`detail_id`),
    KEY `idx_run` (`run_id`),
    KEY `idx_material` (`material_code`),
    KEY `idx_shortage` (`shortage_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 工单齐套状态
CREATE TABLE IF NOT EXISTS `work_order_kit_status` (
    `kit_id` BIGINT AUTO_INCREMENT,
    `order_id` VARCHAR(50) NOT NULL,
    `total_materials` INT NOT NULL,
    `kit_materials` INT NOT NULL,
    `shortage_materials` INT NOT NULL,
    `kit_rate` DECIMAL(5,2) NOT NULL,
    `kit_status` VARCHAR(20) NOT NULL COMMENT 'Complete/Partial/Shortage',
    `checked_at` DATETIME NOT NULL,
    `checked_by` VARCHAR(50),
    PRIMARY KEY (`kit_id`),
    UNIQUE KEY `uk_order` (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 6.5 UI 设计建议

**改造视图**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Schedule\Views\MrpView.xaml`

```
┌─────────────────────────────────────────────────────────────┐
│  物料需求计划(MRP)                [运算MRP] [采购建议] [刷新]│
├─────────────────────────────────────────────────────────────┤
│  KPI: 物料总数(256) | 缺料(12) | 齐套率(87%) | 预警(8)        │
├─────────────────────────────────────────────────────────────┤
│  筛选: [缺料仅看▼] [物料类型▼] [搜索物料号] [清除]             │
├─────────────────────────────────────────────────────────────┤
│  物料编码   │ 名称    │ 需求  │ 库存  │ 在途  │ 短缺  │ 状态  │
│  MAT-001    │ 金线    │ 100K  │ 120K  │ 0     │ 0     │ ✅齐套│
│  MAT-002    │ 引线框  │ 50K   │ 35K   │ 10K   │ 5K    │ ⚠缺料│
│  MAT-003    │ 塑封料  │ 200kg │ 50kg  │ 0     │ 150kg │ 🔴短缺│
├─────────────────────────────────────────────────────────────┤
│  运算记录: [2026-06-05 14:30] [2026-06-04 09:00] ...        │
└─────────────────────────────────────────────────────────────┘
```

**ViewModel 新增/修改属性**:
```csharp
public class MrpViewModel : BindableBase
{
    // 保留原有属性，新增:
    public ObservableCollection<MrpRunRecord> RunRecords { get; set; }
    public MrpRunRecord? SelectedRunRecord { get; set; }
    public decimal KitRate { get; set; }           // 齐套率
    public int WarningCount { get; set; }          // 预警数
    public string FilterMaterialType { get; set; } // 物料类型筛选

    public DelegateCommand RunMrpCommand { get; }          // 运算MRP
    public DelegateCommand GeneratePurchaseCommand { get; } // 采购建议
}
```

### 6.6 业务规则

1. **MRP 运算逻辑**: 需求 = Σ(工单计划量 × BOM 用量) - 可用库存 - 在途量 + 安全库存
2. **短缺状态**:
   - `Sufficient`: 短缺量 = 0
   - `Warning`: 短缺量 > 0 且 < 安全库存
   - `Shortage`: 短缺量 >= 安全库存 且 < 需求量的 50%
   - `Critical`: 短缺量 >= 需求量的 50%
3. **齐套率**: 齐套率 = 齐套工单数 / 总工单数 × 100%
4. **先进先出**: 物料发放遵循 FIFO 原则
5. **自动触发**: 工单状态变更为 Released 时自动触发 MRP 重算

---

## 7. 订单进度跟踪

### 7.1 现状分析

当前 `WorkOrderScheduleViewModel` 提供了基本的工单进度数据，但缺少按订单维度的全流程跟踪视图。需要从订单→工单→批次→工序的多层级进度追踪。

### 7.2 功能清单

| 编号 | 功能 | 优先级 |
|------|------|--------|
| TRK-01 | 订单全流程时间轴 | P0 |
| TRK-02 | 多层级进度钻取 | P0 |
| TRK-03 | 进度偏差预警 | P0 |
| TRK-04 | 关键里程碑标记 | P1 |
| TRK-05 | 进度报表导出 | P1 |

### 7.3 接口设计

**文件**: `e:\AiProj\MES_NEW\src\Server\Modules\MES.Services.OrderMgmt\IOrderProgressService.cs`

```csharp
public interface IOrderProgressService
{
    /// <summary>获取订单全流程时间轴</summary>
    Task<OrderTimelineResponse> GetTimelineAsync(string orderId);

    /// <summary>获取多层级进度（订单→工单→批次→工序）</summary>
    Task<MultiLevelProgressResponse> GetMultiLevelProgressAsync(string orderId);

    /// <summary>获取进度偏差分析</summary>
    Task<ProgressDeviationResponse> GetDeviationAsync(string orderId);

    /// <summary>标记里程碑</summary>
    Task MarkMilestoneAsync(MilestoneRequest request);
}
```

**Controller**: `/api/order-progress/*`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/order-progress/{orderId}/timeline` | 时间轴 |
| GET | `/api/order-progress/{orderId}/multi-level` | 多层级进度 |
| GET | `/api/order-progress/{orderId}/deviation` | 偏差分析 |
| POST | `/api/order-progress/milestone` | 标记里程碑 |

### 7.4 数据表设计

**扩展现有表**: `prod_work_order`

```sql
-- 在 prod_work_order 中新增进度跟踪字段
ALTER TABLE `prod_work_order`
    ADD COLUMN IF NOT EXISTS `schedule_start_date` DATETIME DEFAULT NULL COMMENT '排程计划开始',
    ADD COLUMN IF NOT EXISTS `schedule_end_date` DATETIME DEFAULT NULL COMMENT '排程计划结束',
    ADD COLUMN IF NOT EXISTS `progress_percent` DECIMAL(5,2) DEFAULT 0 COMMENT '进度百分比',
    ADD COLUMN IF NOT EXISTS `is_delayed` TINYINT(1) DEFAULT 0,
    ADD COLUMN IF NOT EXISTS `delay_days` INT DEFAULT 0,
    ADD COLUMN IF NOT EXISTS `delay_reason` VARCHAR(500);

-- 订单里程碑
CREATE TABLE IF NOT EXISTS `order_milestone` (
    `milestone_id` BIGINT AUTO_INCREMENT,
    `order_id` VARCHAR(50) NOT NULL,
    `milestone_type` VARCHAR(30) NOT NULL COMMENT 'OrderCreated/ReviewPassed/Scheduled/MaterialReady/ProductionStarted/CPCompleted/FTCompleted/Shipped/Delivered',
    `milestone_name` VARCHAR(100) NOT NULL,
    `planned_date` DATE,
    `actual_date` DATE,
    `is_achieved` TINYINT(1) NOT NULL DEFAULT 0,
    `remark` TEXT,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`milestone_id`),
    KEY `idx_order` (`order_id`),
    KEY `idx_type` (`milestone_type`),
    KEY `idx_achieved` (`is_achieved`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 7.5 UI 设计建议

**改造视图**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\Views\CustomerProgressView.xaml`

```
┌─────────────────────────────────────────────────────────────┐
│  订单进度跟踪 - SO-2026-001                   [导出] [刷新]  │
├─────────────────────────────────────────────────────────────┤
│  📋 订单信息: 客户=XX科技 | 产品=ABC123 | 数量=100K | 交期=06-30│
├─────────────────────────────────────────────────────────────┤
│  ── 时间轴 ──────────────────────────────────────────────   │
│  ●创建(06-01) → ●评审(06-02) → ●排产(06-03) → ●备料(06-04) │
│  → ◐生产中(06-05~) → ○完工(预计06-25) → ○交付(预计06-28)    │
├─────────────────────────────────────────────────────────────┤
│  ── 工单进度 ────────────────────────────────────────────   │
│  WO-001(父) ████████░░ 80%  [装配阶段]                       │
│    ├─ WO-001-A(子) ██████████ 100%  CP测试 ✅                │
│    ├─ WO-001-B(子) ██████░░░░ 60%   FT测试 ⏳                │
│    └─ WO-001-C(子) ░░░░░░░░░░ 0%    待开始                  │
├─────────────────────────────────────────────────────────────┤
│  ⚠️ 偏差预警: FT测试进度落后计划 2 天                        │
└─────────────────────────────────────────────────────────────┘
```

### 7.6 业务规则

1. **进度计算**: 
   - 工单进度 = Σ(子工单完成量) / 工单计划量 × 100%
   - 订单进度 = Σ(工单完成量) / 订单总量 × 100%
2. **偏差判定**: 实际进度 < 计划进度 × 时间比例 → 标记延期
3. **里程碑自动标记**: 系统状态变更时自动标记对应里程碑
4. **实时推送**: 关键状态变更通过 WebSocket 推送给关注人

---

## 8. 准时交付率(OTD)

### 8.1 现状分析

当前 `DeliveryManageViewModel` 使用模拟数据。需要基于实际订单交付数据计算 OTD 指标，支持按客户/产品/时间段多维度分析。

### 8.2 功能清单

| 编号 | 功能 | 优先级 |
|------|------|--------|
| OTD-01 | OTD 指标计算 | P0 |
| OTD-02 | 按客户维度分析 | P0 |
| OTD-03 | 按产品维度分析 | P1 |
| OTD-04 | 趋势图表展示 | P1 |
| OTD-05 | 导出报表 | P1 |

### 8.3 接口设计

**文件**: `e:\AiProj\MES_NEW\src\Server\Modules\MES.Services.OrderMgmt\IOtdService.cs`

```csharp
public interface IOtdService
{
    /// <summary>计算 OTD 指标</summary>
    Task<OtdSummary> CalculateOtdAsync(OtdQuery query);

    /// <summary>按客户维度分析 OTD</summary>
    Task<List<CustomerOtdAnalysis>> GetCustomerOtdAsync(OtdQuery query);

    /// <summary>按产品维度分析 OTD</summary>
    Task<List<ProductOtdAnalysis>> GetProductOtdAsync(OtdQuery query);

    /// <summary>获取 OTD 趋势数据</summary>
    Task<List<OtdTrendPoint>> GetOtdTrendAsync(OtdTrendQuery query);

    /// <summary>导出 OTD 报表</summary>
    Task<byte[]> ExportOtdReportAsync(OtdQuery query, string format);
}
```

**Controller**: `/api/otd/*`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/otd/summary` | OTD 汇总指标 |
| GET | `/api/otd/by-customer` | 按客户分析 |
| GET | `/api/otd/by-product` | 按产品分析 |
| GET | `/api/otd/trend` | 趋势数据 |
| GET | `/api/otd/export` | 导出报表 |

### 8.4 数据表设计

**文件**: `e:\AiProj\MES_NEW\docs\sql\migrations\V5.3.0__otd.sql`

```sql
-- 订单交付记录
CREATE TABLE IF NOT EXISTS `order_delivery_record` (
    `delivery_id` VARCHAR(50) NOT NULL COMMENT '格式: DEL-YYYYMMDD-NNN',
    `order_id` VARCHAR(50) NOT NULL,
    `customer_id` VARCHAR(50) NOT NULL,
    `customer_name` VARCHAR(200),
    `product_id` VARCHAR(50),
    `product_name` VARCHAR(200),
    `plan_qty` INT NOT NULL,
    `deliver_qty` INT NOT NULL,
    `shortage_qty` INT NOT NULL,
    `promised_date` DATE NOT NULL,
    `actual_delivery_date` DATE,
    `is_on_time` TINYINT(1) DEFAULT NULL COMMENT '1=准时, 0=延期, NULL=未交付',
    `delay_days` INT DEFAULT 0,
    `delay_reason` VARCHAR(500),
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT 'Pending/PartialDelivered/Delivered',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`delivery_id`),
    KEY `idx_order` (`order_id`),
    KEY `idx_customer` (`customer_id`),
    KEY `idx_promised` (`promised_date`),
    KEY `idx_on_time` (`is_on_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- OTD 统计快照（定期计算存储）
CREATE TABLE IF NOT EXISTS `otd_statistics` (
    `stat_id` BIGINT AUTO_INCREMENT,
    `stat_period` VARCHAR(20) NOT NULL COMMENT '格式: 2026-06 或 2026-W23',
    `period_type` VARCHAR(10) NOT NULL COMMENT 'Monthly/Weekly',
    `total_orders` INT NOT NULL,
    `delivered_orders` INT NOT NULL,
    `on_time_orders` INT NOT NULL,
    `otd_rate` DECIMAL(5,2) NOT NULL COMMENT '百分比',
    `avg_delay_days` DECIMAL(5,2) DEFAULT 0,
    `by_customer` JSON COMMENT '按客户维度的统计',
    `by_product` JSON COMMENT '按产品维度的统计',
    `calculated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`stat_id`),
    UNIQUE KEY `uk_period_type` (`stat_period`, `period_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 8.5 UI 设计建议

**改造视图**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Schedule\Views\DeliveryManageView.xaml`

```
┌─────────────────────────────────────────────────────────────┐
│  交付管理与 OTD                            [导出报表] [刷新] │
├─────────────────────────────────────────────────────────────┤
│  KPI: OTD(92.3%) | 本月交付(45) | 延期(3) | 平均延误(1.2天) │
├─────────────────────────────────────────────────────────────┤
│  趋势图: ███████████░ 6月:92.3% → ████████████░ 5月:95.1%   │
├─────────────────────────────────────────────────────────────┤
│  按客户分析:                                                 │
│  客户     │ 订单数 │ 准时 │ 延期 │ OTD率  │ 趋势             │
│ XX科技   │ 12     │ 11   │ 1    │ 91.7%  │ ▲                │
│ YY电子   │ 8      │ 8    │ 0    │ 100%   │ —                │
│ ZZ半导体 │ 15     │ 13   │ 2    │ 86.7%  │ ▼                │
├─────────────────────────────────────────────────────────────┤
│  交付明细:                                                   │
│  交付单号 │ 订单   │ 客户   │ 计划数│ 实发数│ 交期    │ 状态  │
│ DEL-001  │ SO-001 │ XX科技 │ 10K   │ 10K   │ 06-15   │ ✅准时│
│ DEL-002  │ SO-003 │ ZZ半导 │ 5K    │ 4.5K  │ 06-12   │ ⚠延期 │
└─────────────────────────────────────────────────────────────┘
```

**ViewModel 新增/修改属性**:
```csharp
public class DeliveryManageViewModel : BindableBase
{
    // 保留原有属性，新增:
    public decimal OtdRate { get; set; }          // OTD 比率
    public int DelayedCount { get; set; }         // 延期数
    public decimal AvgDelayDays { get; set; }     // 平均延误天数
    public ObservableCollection<CustomerOtdAnalysis> CustomerAnalysis { get; set; }
    public ObservableCollection<OtdTrendPoint> TrendData { get; set; }

    public DelegateCommand ExportReportCommand { get; }
}
```

### 8.6 业务规则

1. **准时定义**: 实际交付日期 ≤ 承诺交付日期 → 准时
2. **部分交付**: 若分批交付，以最后一批交付日期判断是否准时
3. **OTD 计算**: OTD = 准时交付订单数 / 应交付订单总数 × 100%
4. **统计周期**: 支持按月/按周/按季度统计
5. **自动计算**: 每日凌晨自动计算前一日 OTD 并存储快照

---

## 9. 急单处理

### 9.1 现状分析

当前系统缺少急单管理机制。急单插入可能影响现有排产计划，需要进行影响评估和优先级调整。

### 9.2 功能清单

| 编号 | 功能 | 优先级 |
|------|------|--------|
| RUSH-01 | 急单申请与审批 | P0 |
| RUSH-02 | 影响评估分析 | P0 |
| RUSH-03 | 自动优先级调整 | P0 |
| RUSH-04 | 通知相关方 | P1 |
| RUSH-05 | 急单跟踪看板 | P1 |

### 9.3 接口设计

**文件**: `e:\AiProj\MES_NEW\src\Server\Modules\MES.Services.OrderMgmt\IRushOrderService.cs`

```csharp
public interface IRushOrderService
{
    /// <summary>提交急单申请</summary>
    Task<RushOrderResponse> SubmitRushRequestAsync(SubmitRushRequest request);

    /// <summary>执行影响评估</summary>
    Task<ImpactAssessmentResponse> AssessImpactAsync(string rushOrderId);

    /// <summary>审批急单</summary>
    Task<RushOrderResponse> ApproveRushAsync(string rushOrderId, ApproveRushRequest request);

    /// <summary>自动调整优先级</summary>
    Task<List<PriorityAdjustment>> AutoAdjustPrioritiesAsync(string rushOrderId);

    /// <summary>获取急单看板数据</summary>
    Task<RushOrderDashboard> GetDashboardAsync();
}
```

**Controller**: `/api/rush-orders/*`

| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/rush-orders` | 提交急单申请 |
| GET | `/api/rush-orders` | 急单列表 |
| GET | `/api/rush-orders/{orderId}/assess` | 影响评估 |
| POST | `/api/rush-orders/{orderId}/approve` | 审批 |
| GET | `/api/rush-orders/dashboard` | 急单看板 |

### 9.4 数据表设计

**文件**: `e:\AiProj\MES_NEW\docs\sql\migrations\V5.4.0__rush_order.sql`

```sql
-- 急单记录
CREATE TABLE IF NOT EXISTS `rush_order` (
    `rush_id` VARCHAR(50) NOT NULL COMMENT '格式: RUSH-YYYYMMDD-NNN',
    `order_id` VARCHAR(50) NOT NULL,
    `original_due_date` DATE NOT NULL,
    `requested_due_date` DATE NOT NULL,
    `rush_reason` VARCHAR(500) NOT NULL,
    `customer_name` VARCHAR(200),
    `product_name` VARCHAR(200),
    `quantity` INT NOT NULL,
    `priority_level` VARCHAR(20) NOT NULL DEFAULT 'High' COMMENT 'High/Critical/Emergency',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT 'Pending/Assessing/Approved/Rejected/Executed/Completed',
    `impact_summary` TEXT COMMENT '影响评估摘要',
    `approved_by` VARCHAR(50),
    `approved_at` DATETIME,
    `rejection_reason` VARCHAR(500),
    `created_by` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `completed_at` DATETIME,
    PRIMARY KEY (`rush_id`),
    UNIQUE KEY `uk_order` (`order_id`),
    KEY `idx_status` (`status`),
    KEY `idx_priority` (`priority_level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 急单影响评估详情
CREATE TABLE IF NOT EXISTS `rush_order_impact` (
    `impact_id` BIGINT AUTO_INCREMENT,
    `rush_id` VARCHAR(50) NOT NULL,
    `affected_order_id` VARCHAR(50) NOT NULL,
    `affected_product` VARCHAR(200),
    `impact_type` VARCHAR(20) NOT NULL COMMENT 'ScheduleDelay/MaterialShortage/CapacityConflict',
    `impact_description` TEXT,
    `original_plan` VARCHAR(500),
    `new_plan` VARCHAR(500),
    `delay_days` INT DEFAULT 0,
    `is_acceptable` TINYINT(1) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`impact_id`),
    KEY `idx_rush` (`rush_id`),
    KEY `idx_affected` (`affected_order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 急单优先级调整记录
CREATE TABLE IF NOT EXISTS `rush_order_priority_adjustment` (
    `adjust_id` BIGINT AUTO_INCREMENT,
    `rush_id` VARCHAR(50) NOT NULL,
    `adjusted_order_id` VARCHAR(50) NOT NULL,
    `original_priority` VARCHAR(20),
    `new_priority` VARCHAR(20),
    `adjust_reason` VARCHAR(500),
    `adjusted_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `adjusted_by` VARCHAR(50),
    PRIMARY KEY (`adjust_id`),
    KEY `idx_rush` (`rush_id`),
    KEY `idx_order` (`adjusted_order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 9.5 UI 设计建议

**新增视图**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\Views\RushOrderView.xaml`

```
┌─────────────────────────────────────────────────────────────┐
│  急单处理看板                          [提交急单] [刷新]     │
├─────────────────────────────────────────────────────────────┤
│  KPI: 待处理(3) | 评估中(2) | 已批准(5) | 执行中(4) | 已完成(12)│
├─────────────────────────────────────────────────────────────┤
│  急单ID   │ 订单   │ 客户   │ 原因   │ 优先级  │ 状态   │ 交期  │
│  RUSH-001 │ SO-015 │ XX科技 │ 市场急需│🔴紧急 │ 已批准 │ 06-10 │
│  RUSH-002 │ SO-018 │ YY电子 │ 样品急需│🟠高   │ 评估中 │ 06-12 │
├─────────────────────────────────────────────────────────────┤
│  影响评估: RUSH-001 将导致以下订单延期:                       │
│    - SO-010: 延期 2 天 (产线 A 产能冲突)                     │
│    - SO-012: 延期 1 天 (物料金线短缺)                        │
│  [接受影响并批准] [驳回]                                     │
└─────────────────────────────────────────────────────────────┘
```

### 9.6 业务规则

1. **急单优先级**: Emergency > Critical > High > 原有优先级
2. **影响评估**: 系统自动分析产能冲突、物料短缺、排产延期
3. **审批流程**: 提交 → 影响评估 → 管理层审批 → 执行
4. **自动调整**: 急单批准后，系统自动降低受影响订单的优先级
5. **通知**: 急单状态变更时通知受影响订单的负责人
6. **完成标记**: 急单交付后自动标记完成，恢复原优先级顺序

---

## 10. UI 优化方案汇总

### 10.1 Order 模块视图

#### 10.1.1 WorkOrderListView（工单列表）

| 优化项 | 当前 | 目标 |
|--------|------|------|
| KPI 卡片 | 5 张 | 7 张（+待评审、紧急工单） |
| 筛选栏 | 状态/优先级/日期/搜索 | +客户、区域 |
| DataGrid 列 | 15 列 | +层级、工单类型、工艺阶段、齐套状态、评审状态 |
| 操作按钮 | 7 个 | +评审、拆分 |
| 架构 | 本地 DataService | REST API 对接 |

**修改文件**:
- `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\Views\WorkOrderListView.xaml`
- `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\ViewModels\WorkOrderListViewModel.cs`
- 新增: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\Views\OrderReviewView.xaml`
- 新增: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\Views\RushOrderView.xaml`

#### 10.1.2 CustomerProgressView（客户订单进度）

| 优化项 | 当前 | 目标 |
|--------|------|------|
| 视图状态 | 需检查 | 完整订单时间轴 + 多层级进度 |
| 数据源 | 需检查 | REST API (IOrderProgressService) |

**修改文件**:
- `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\Views\CustomerProgressView.xaml`
- `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Order\ViewModels\CustomerProgressViewModel.cs`

### 10.2 Schedule 模块视图

#### 10.2.1 MrpView（物料需求计划）

**从占位符改造为**:
```xml
<Grid Margin="24">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>  <!-- 标题 + 操作按钮 -->
        <RowDefinition Height="Auto"/>  <!-- KPI 卡片 -->
        <RowDefinition Height="Auto"/>  <!-- 筛选栏 -->
        <RowDefinition Height="*"/>     <!-- DataGrid 物料需求明细 -->
        <RowDefinition Height="Auto"/>  <!-- 运算历史 + 分页 -->
    </Grid.RowDefinitions>

    <!-- Grid.Row 0: 标题行 -->
    <DockPanel>
        <TextBlock Text="物料需求计划(MRP)" FontSize="24" FontWeight="SemiBold" Foreground="{DynamicResource MesTextPrimaryBrush}"/>
        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
            <Button Content="🔢 运算MRP" Command="{Binding RunMrpCommand}" Style="{DynamicResource MesPrimaryButton}"/>
            <Button Content="📋 采购建议" Command="{Binding GeneratePurchaseCommand}" Style="{DynamicResource MesSecondaryButton}"/>
            <Button Content="↻ 刷新" Command="{Binding RefreshCommand}" Style="{DynamicResource MesSecondaryButton}"/>
        </StackPanel>
    </DockPanel>

    <!-- Grid.Row 1: KPI 卡片 -->
    <UniformGrid Rows="1">
        <!-- 物料总数 | 缺料数 | 齐套率 | 预警数 -->
    </UniformGrid>

    <!-- Grid.Row 2: 筛选栏 -->
    <Border Style="{DynamicResource MesCard}">
        <WrapPanel>
            <ComboBox SelectedItem="{Binding ShowShortageOnly}" .../>
            <ComboBox SelectedItem="{Binding FilterMaterialType}" .../>
            <TextBox Text="{Binding SearchText}" .../>
            <Button Content="清除筛选" Command="{Binding ClearFilterCommand}"/>
        </WrapPanel>
    </Border>

    <!-- Grid.Row 3: DataGrid -->
    <DataGrid AutoGenerateColumns="False" ItemsSource="{Binding Items}">
        <DataGrid.Columns>
            <DataGridTextColumn Header="物料编码" Binding="{Binding MaterialCode}"/>
            <DataGridTextColumn Header="物料名称" Binding="{Binding MaterialName}"/>
            <DataGridTextColumn Header="规格" Binding="{Binding Specification}"/>
            <DataGridTextColumn Header="单位" Binding="{Binding Unit}"/>
            <DataGridTextColumn Header="需求量" Binding="{Binding RequiredQty}"/>
            <DataGridTextColumn Header="可用库存" Binding="{Binding AvailableStock}"/>
            <DataGridTextColumn Header="在途" Binding="{Binding InTransitQty}"/>
            <DataGridTextColumn Header="短缺" Binding="{Binding ShortageQty}"/>
            <DataGridTemplateColumn Header="状态">
                <!-- 齐套/预警/缺料/严重短缺 徽章 -->
            </DataGridTemplateColumn>
            <DataGridTextColumn Header="预计到货" Binding="{Binding ExpectedArrival}"/>
        </DataGrid.Columns>
    </DataGrid>

    <!-- Grid.Row 4: 运算历史 + 分页 -->
</Grid>
```

#### 10.2.2 DeliveryManageView（交付管理）

**从占位符改造为**:
```xml
<Grid Margin="24">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>  <!-- 标题 + 操作 -->
        <RowDefinition Height="Auto"/>  <!-- KPI 卡片 + OTD 趋势 -->
        <RowDefinition Height="Auto"/>  <!-- 按客户分析表 -->
        <RowDefinition Height="*"/>     <!-- 交付明细 DataGrid -->
        <RowDefinition Height="Auto"/>  <!-- 分页 -->
    </Grid.RowDefinitions>

    <!-- Grid.Row 0: 标题 -->
    <DockPanel>
        <TextBlock Text="交付管理与 OTD" FontSize="24" FontWeight="SemiBold"/>
        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
            <Button Content="📊 导出报表" Command="{Binding ExportReportCommand}"/>
            <Button Content="↻ 刷新" Command="{Binding RefreshCommand}"/>
        </StackPanel>
    </DockPanel>

    <!-- Grid.Row 1: KPI + 趋势图 -->
    <UniformGrid Rows="1">
        <!-- OTD率 | 本月交付数 | 延期数 | 平均延误天数 -->
    </UniformGrid>

    <!-- Grid.Row 2: 客户分析 -->
    <DataGrid ItemsSource="{Binding CustomerAnalysis}" ...>
        <!-- 客户 | 订单数 | 准时 | 延期 | OTD率 | 趋势 -->
    </DataGrid>

    <!-- Grid.Row 3: 交付明细 -->
    <DataGrid ItemsSource="{Binding Items}" ...>
        <!-- 交付单号 | 订单 | 客户 | 计划数 | 实发数 | 交期 | 状态 -->
    </DataGrid>

    <!-- Grid.Row 4: 分页 -->
</Grid>
```

#### 10.2.3 MasterPlanView（主生产计划）

**新增视图**: `e:\AiProj\MES_NEW\src\Client\Modules\MES.Modules.Schedule\Views\MasterPlanView.xaml`

包含: KPI 卡片、计划列表、产能负荷柱状图、冲突预警表、新建计划弹窗

### 10.3 UI 设计原则

1. **一致性**: 所有视图遵循统一的 MesCard、MesKpiCard、MesBadge、MesDataGrid 样式
2. **KPI 先行**: 每个视图顶部展示核心指标卡片
3. **筛选有效**: 常用筛选条件放在筛选栏，支持快速清除
4. **状态可视**: 使用颜色徽章区分状态（✅成功/⚠警告/🔴危险/ℹ信息）
5. **操作便捷**: 主操作按钮放在右上角，批量操作支持多选

---

## 11. 实施顺序与依赖关系

### 11.1 Sprint 规划

| Sprint | 周期 | 内容 | 交付物 |
|--------|------|------|--------|
| **Sprint 1** | 2 周 | 订单评审模块 | 评审 UI + 后端 API + 数据表 + 工单状态流转 |
| **Sprint 2** | 2 周 | 工单分解与拆分 | 分解 API + UI 改造 + 父子工单层级 + 齐套状态 |
| **Sprint 3** | 2 周 | 主生产计划 | MPS UI + API + 产能分析 + 冲突检测 |
| **Sprint 4** | 2 周 | MRP + 齐套管理 | MRP 运算 API + UI 改造 + 齐套率 |
| **Sprint 5** | 2 周 | 订单进度跟踪 + OTD | 时间轴 UI + 多层级进度 + OTD 指标 |
| **Sprint 6** | 2 周 | 急单处理 + 集成测试 | 急单 UI + API + 影响评估 + 端到端测试 |

### 11.2 依赖关系

```
订单评审(Sprint 1)
    ↓
工单分解(Sprint 2) ← 依赖评审通过后的工单状态
    ↓
主生产计划(Sprint 3) ← 依赖分解后的子工单
    ↓
MRP(Sprint 4) ← 依赖工单计划 + 库存数据
    ↓
订单进度 + OTD(Sprint 5) ← 依赖以上所有模块的运行数据
    ↓
急单处理(Sprint 6) ← 依赖完整的排产和进度体系
```

### 11.3 并行开发

- **前端与后端**: 每个 Sprint 前后端可并行开发，基于接口契约
- **数据表设计**: 可提前完成所有 DDL，分批执行迁移
- **UI Mock**: 前端可使用 Mock 数据先行开发 UI

### 11.4 里程碑

| 里程碑 | 时间节点 | 验收条件 |
|--------|----------|----------|
| M1: 评审流程上线 | Sprint 1 结束 | 工单 100% 经过评审后才能下达 |
| M2: 工单层级可见 | Sprint 2 结束 | WorkOrderListView 正确展示父子层级 |
| M3: 产能可视化 | Sprint 3 结束 | 可查看产能负荷和冲突预警 |
| M4: MRP 自动化 | Sprint 4 结束 | MRP 运算自动执行，齐套率可视化 |
| M5: OTD 可度量 | Sprint 5 结束 | OTD 指标按月自动生成报表 |
| M6: 全流程贯通 | Sprint 6 结束 | 从订单创建到交付的完整流程可运行 |

---

## 12. 验收标准

### 12.1 功能验收

| 模块 | 验收用例 | 预期结果 |
|------|----------|----------|
| 订单评审 | 创建工单 → 自动触发评审 → 三部门投票 → 通过 | 工单状态变更为 Approved |
| 订单评审 | 创建工单 → 任一部门驳回 | 工单状态变更为 Rejected |
| 订单评审 | 符合跳过规则的工单 | 自动跳过评审，直接进入 Created |
| 主生产计划 | 创建月度计划 → 产能分析 | 显示负荷率，>90% 预警 |
| 工单分解 | 父工单下达 → 自动分解 | 创建对应子工单，正确关联 |
| 工单分解 | 查看工单树 | 正确展示父子层级关系 |
| MRP | 执行 MRP 运算 | 正确计算物料需求和短缺 |
| MRP | 查看齐套率 | 按工单显示齐套状态 |
| 订单进度 | 查看订单时间轴 | 正确展示全流程里程碑 |
| 订单进度 | 多层级进度钻取 | 订单→工单→批次逐级展示 |
| OTD | 查看月度 OTD | 正确计算准时交付率 |
| OTD | 按客户分析 | 显示各客户 OTD 排名 |
| 急单处理 | 提交急单 → 影响评估 | 显示受影响订单清单 |
| 急单处理 | 批准急单 | 自动调整相关工单优先级 |

### 12.2 UI 验收

| 验收项 | 标准 |
|--------|------|
| 响应速度 | 页面加载 < 2 秒，数据刷新 < 1 秒 |
| 数据一致性 | 刷新后数据不丢失、不重复 |
| 状态颜色 | 成功=绿色、警告=橙色、危险=红色、信息=蓝色 |
| 分页性能 | 10000 条数据分页流畅 |
| 筛选联动 | 筛选条件改变后数据正确过滤 |
| 错误提示 | API 异常时显示友好错误信息 |

### 12.3 性能验收

| 指标 | 要求 |
|------|------|
| API 响应时间 | P95 < 500ms |
| MRP 运算时间 | 100 工单 < 10 秒 |
| 并发用户 | 支持 50 并发操作 |
| 数据库查询 | 复杂查询 < 1 秒（有索引） |

---

## 13. 数据库迁移计划

### 13.1 迁移脚本清单

| 文件 | 描述 | 依赖 |
|------|------|------|
| `V5.0.0__order_review.sql` | 订单评审表 | 无 |
| `V5.1.0__master_production_plan.sql` | 主生产计划表 | 无 |
| `V5.1.1__alter_prod_work_order.sql` | 扩展工单表 | 无 |
| `V5.2.0__mrp.sql` | MRP 相关表 | 无 |
| `V5.3.0__otd.sql` | OTD 相关表 | 无 |
| `V5.4.0__rush_order.sql` | 急单相关表 | 无 |

### 13.2 执行顺序

```bash
# 1. 扩展现有表（向后兼容，不影响现有数据）
mysql -u root -p mes_db < docs/sql/migrations/V5.1.1__alter_prod_work_order.sql

# 2. 新建评审表
mysql -u root -p mes_db < docs/sql/migrations/V5.0.0__order_review.sql

# 3. 新建主生产计划表
mysql -u root -p mes_db < docs/sql/migrations/V5.1.0__master_production_plan.sql

# 4. 新建 MRP 表
mysql -u root -p mes_db < docs/sql/migrations/V5.2.0__mrp.sql

# 5. 新建 OTD 表
mysql -u root -p mes_db < docs/sql/migrations/V5.3.0__otd.sql

# 6. 新建急单表
mysql -u root -p mes_db < docs/sql/migrations/V5.4.0__rush_order.sql
```

### 13.3 数据初始化

```sql
-- 初始化评审跳过规则
INSERT INTO `order_review_bypass_rule` (`condition_type`, `condition_value`) VALUES
('ProductFamiliarity', 'Mature'),
('CustomerLevel', 'VIP'),
('OrderQty', '<=100');

-- 初始化默认分解规则
INSERT INTO `work_order_decompose_rule` (`rule_name`, `product_type`, `decompose_strategy`, `stages`, `is_default`) VALUES
('标准封装分解', 'Standard', 'ByProcess', '["Bump", "Assemble", "CP_Test", "FT_Test"]', 1),
('先进封装分解', 'Advanced', 'ByProcess', '["Bump", "RDL", "Assemble", "CP_Test", "FT_Test"]', 0);

-- 初始化 OTD 统计（当前月）
INSERT INTO `otd_statistics` (`stat_period`, `period_type`, `total_orders`, `delivered_orders`, `on_time_orders`, `otd_rate`) VALUES
('2026-06', 'Monthly', 0, 0, 0, 0);
```

### 13.4 回滚方案

```sql
-- 回滚脚本（按反向顺序执行）
DROP TABLE IF EXISTS `rush_order_priority_adjustment`;
DROP TABLE IF EXISTS `rush_order_impact`;
DROP TABLE IF EXISTS `rush_order`;
DROP TABLE IF EXISTS `otd_statistics`;
DROP TABLE IF EXISTS `order_delivery_record`;
DROP TABLE IF EXISTS `work_order_kit_status`;
DROP TABLE IF EXISTS `mrp_requirement_detail`;
DROP TABLE IF EXISTS `mrp_run_record`;
DROP TABLE IF EXISTS `capacity_conflict`;
DROP TABLE IF EXISTS `capacity_load`;
DROP TABLE IF EXISTS `master_production_plan_line`;
DROP TABLE IF EXISTS `master_production_plan`;
DROP TABLE IF EXISTS `work_order_decompose_record`;
DROP TABLE IF EXISTS `work_order_decompose_rule`;
DROP TABLE IF EXISTS `order_milestone`;
DROP TABLE IF EXISTS `order_review_vote`;
DROP TABLE IF EXISTS `order_review_bypass_rule`;
DROP TABLE IF EXISTS `order_review_task`;

-- 恢复 prod_work_order 表（删除新增列）
ALTER TABLE `prod_work_order`
    DROP COLUMN IF EXISTS `review_status`,
    DROP COLUMN IF EXISTS `kit_status`,
    DROP COLUMN IF EXISTS `process_stage`,
    DROP COLUMN IF EXISTS `wo_type`,
    DROP COLUMN IF EXISTS `parent_order_id`,
    DROP COLUMN IF EXISTS `decompose_rule_id`,
    DROP COLUMN IF EXISTS `schedule_start_date`,
    DROP COLUMN IF EXISTS `schedule_end_date`,
    DROP COLUMN IF EXISTS `progress_percent`,
    DROP COLUMN IF EXISTS `is_delayed`,
    DROP COLUMN IF EXISTS `delay_days`,
    DROP COLUMN IF EXISTS `delay_reason`;
```

---

## 附录

### A. 文件路径汇总

**新增文件**:

| 文件路径 | 描述 |
|----------|------|
| `src/Server/Modules/MES.Services.OrderMgmt/IOrderReviewService.cs` | 订单评审接口 |
| `src/Server/Modules/MES.Services.OrderMgmt/OrderReviewService.cs` | 订单评审实现 |
| `src/Server/Modules/MES.Services.OrderMgmt/Controllers/OrderReviewController.cs` | 订单评审控制器 |
| `src/Server/Modules/MES.Services.Production/IMasterPlanService.cs` | 主生产计划接口 |
| `src/Server/Modules/MES.Services.Production/MasterPlanService.cs` | 主生产计划实现 |
| `src/Server/Modules/MES.Services.Production/Controllers/MasterPlanController.cs` | 主生产计划控制器 |
| `src/Server/Modules/MES.Services.Production/IWorkOrderDecomposeService.cs` | 工单分解接口 |
| `src/Server/Modules/MES.Services.Production/WorkOrderDecomposeService.cs` | 工单分解实现 |
| `src/Server/Modules/MES.Services.Production/Controllers/WorkOrderDecomposeController.cs` | 工单分解控制器 |
| `src/Server/Modules/MES.Services.Inventory/IMrpService.cs` | MRP 接口 |
| `src/Server/Modules/MES.Services.Inventory/MrpService.cs` | MRP 实现 |
| `src/Server/Modules/MES.Services.Inventory/Controllers/MrpController.cs` | MRP 控制器 |
| `src/Server/Modules/MES.Services.OrderMgmt/IOrderProgressService.cs` | 订单进度接口 |
| `src/Server/Modules/MES.Services.OrderMgmt/OrderProgressService.cs` | 订单进度实现 |
| `src/Server/Modules/MES.Services.OrderMgmt/Controllers/OrderProgressController.cs` | 订单进度控制器 |
| `src/Server/Modules/MES.Services.OrderMgmt/IOtdService.cs` | OTD 接口 |
| `src/Server/Modules/MES.Services.OrderMgmt/OtdService.cs` | OTD 实现 |
| `src/Server/Modules/MES.Services.OrderMgmt/Controllers/OtdController.cs` | OTD 控制器 |
| `src/Server/Modules/MES.Services.OrderMgmt/IRushOrderService.cs` | 急单接口 |
| `src/Server/Modules/MES.Services.OrderMgmt/RushOrderService.cs` | 急单实现 |
| `src/Server/Modules/MES.Services.OrderMgmt/Controllers/RushOrderController.cs` | 急单控制器 |
| `src/Shared/MES.Contracts/OrderReview/ReviewDto.cs` | 评审 DTO |
| `src/Shared/MES.Contracts/MasterPlan/MasterPlanDto.cs` | 主生产计划 DTO |
| `src/Shared/MES.Contracts/WorkOrder/DecomposeDto.cs` | 工单分解 DTO |
| `src/Shared/MES.Contracts/Mrp/MrpDto.cs` | MRP DTO |
| `src/Shared/MES.Contracts/OrderProgress/ProgressDto.cs` | 进度 DTO |
| `src/Shared/MES.Contracts/Otd/OtdDto.cs` | OTD DTO |
| `src/Shared/MES.Contracts/RushOrder/RushOrderDto.cs` | 急单 DTO |
| `src/Client/Modules/MES.Modules.Order/Views/OrderReviewView.xaml` | 评审视图 |
| `src/Client/Modules/MES.Modules.Order/Views/RushOrderView.xaml` | 急单视图 |
| `src/Client/Modules/MES.Modules.Order/ViewModels/OrderReviewViewModel.cs` | 评审 ViewModel |
| `src/Client/Modules/MES.Modules.Order/ViewModels/RushOrderViewModel.cs` | 急单 ViewModel |
| `src/Client/Modules/MES.Modules.Schedule/Views/MasterPlanView.xaml` | 主生产计划视图 |
| `src/Client/Modules/MES.Modules.Schedule/ViewModels/MasterPlanViewModel.cs` | 主生产计划 ViewModel |
| `docs/sql/migrations/V5.0.0__order_review.sql` | 评审 DDL |
| `docs/sql/migrations/V5.1.0__master_production_plan.sql` | 主生产计划 DDL |
| `docs/sql/migrations/V5.1.1__alter_prod_work_order.sql` | 工单表扩展 DDL |
| `docs/sql/migrations/V5.2.0__mrp.sql` | MRP DDL |
| `docs/sql/migrations/V5.3.0__otd.sql` | OTD DDL |
| `docs/sql/migrations/V5.4.0__rush_order.sql` | 急单 DDL |

**修改文件**:

| 文件路径 | 修改内容 |
|----------|----------|
| `src/Client/Modules/MES.Modules.Order/Views/WorkOrderListView.xaml` | 新增列、KPI、按钮 |
| `src/Client/Modules/MES.Modules.Order/ViewModels/WorkOrderListViewModel.cs` | 新增属性、命令 |
| `src/Client/Modules/MES.Modules.Order/Views/CustomerProgressView.xaml` | 时间轴、多层级进度 |
| `src/Client/Modules/MES.Modules.Schedule/Views/MrpView.xaml` | 从占位符改为完整 UI |
| `src/Client/Modules/MES.Modules.Schedule/ViewModels/AllScheduleViewModels.cs` | 扩展 MrpViewModel、DeliveryManageViewModel |
| `src/Client/Modules/MES.Modules.Schedule/Views/DeliveryManageView.xaml` | 从占位符改为完整 UI |
| `src/Infrastructure/MES.Infrastructure.Persistence/Entities/ProductionEntities.cs` | ProdWorkOrder 新增字段 |
| `src/Shared/MES.Contracts/Production/WorkOrderDto.cs` | 新增 DTO 属性 |

### B. 状态流转图

```
订单状态流转:
OrderCreated → PendingReview → [Review] → Approved → Created → Released → InProgress → Completed → Delivered
                                        ↓
                                     Rejected → Modified → PendingReview

工单状态流转:
Created → PendingReview → Approved → Released → InProgress → Hold → InProgress → Completed
                                                        ↓
                                                     Cancelled

评审状态流转:
Pending → InProgress → [All Approve] → Approved
                     → [Any Reject]   → Rejected
                     → [Timeout]      → Expired → AutoApproved

急单状态流转:
Pending → Assessing → [ImpactAcceptable + Approved] → Approved → Executed → Completed
                        ↓
                     [ImpactUnacceptable / Rejected] → Rejected
```

### C. 关键业务规则速查

| 规则 | 描述 |
|------|------|
| 评审超时 | Standard=24h, Expedited=4h, Rush=1h |
| 齐套判定 | 所有物料到位=齐套，部分=部分，未检=未检查 |
| 产能预警 | 负荷率>90% 预警，>100% 冲突 |
| OTD 计算 | 准时数/应交付总数×100% |
| 急单优先级 | Emergency > Critical > High > 原优先级 |
| MRP 运算 | 需求=Σ(工单×BOM)-库存-在途+安全库存 |
| 进度计算 | 父工单进度=Σ(子工单完成量)/父工单计划量 |
| 短缺分级 | Sufficient=0, Warning<安全库存, Shortage<50%, Critical≥50% |

---

> **文档维护**: 此文档为生产就绪级别的实施计划，所有技术细节（API 路径、SQL DDL、XAML 布局、ViewModel 属性）均基于当前代码库分析编写。实施过程中如有变更，请同步更新本文档。
