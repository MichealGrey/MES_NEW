# MES 优化改善计划 — 阶段一详细实施方案

> **文档编号：** MES-PHASE1-2026-001  
> **版本：** V1.0  
> **创建日期：** 2026-06-06  
> **项目：** OSAT Semiconductor Assembly & Test MES System  
> **计划性质：** 阶段一（核心质量与仓储 P0 补齐）— 详细设计文档  
> **前置文档：** `MES_优化改善计划.md`（OUTLINE级别路线图）、`MES_需求满足度评估报告.md`

---

## 一、阶段一总览

### 1.1 阶段目标

补齐 **质量管理** 和 **仓储物料** 的核心后端 API，使系统具备 **来料检验 → 生产 → 出货** 的完整质量与物料管控闭环。实现从 IQC 来料检验到紧急质量召回的端到端流程。

### 1.2 范围清单

| 序号 | 模块 | 需求编号 | 优先级 | 预估周期 |
|------|------|---------|--------|---------|
| 1 | IQC 来料检验管理 | REQ-QM-001 | P0 | 1.5 周 |
| 2 | FQC/OQC 终检管理 | REQ-QM-005 | P0 | 1 周 |
| 3 | 不合格品管理与 MRB 评审 | REQ-QM-006 | P0 | 1.5 周 |
| 4 | 原材料入库 / FIFO / 有效期 | REQ-WM-001~003 | P0 | 2 周 |
| 5 | 产线发料/退料管理 | REQ-WM-004 | P0 | 1.5 周 |
| 6 | 成品入库/出库 | REQ-WM-006 | P0 | 1 周 |
| 7 | 生产异常上报与停线 | REQ-PO-011 | P0 | 1.5 周 |
| 8 | 设备故障管理与保养 | REQ-EQ-002, REQ-EQ-003 | P0 | 1.5 周 |
| 9 | 首件检验流程 | REQ-QM-002 | P0 | 1 周 |
| 10 | 紧急质量通知与召回 | REQ-QM-011 | P0 | 1.5 周 |

### 1.3 技术约束

| 约束项 | 说明 |
|--------|------|
| 数据库 | MySQL 8.0+，utf8mb4 字符集 |
| ORM | EF Core + Dapper 混合（复杂查询用 Dapper） |
| 架构 | Controller → Service → DbContext/Repository |
| API 规范 | RESTful，URL 版本化 `/api/v1/` |
| 认证 | JWT Bearer Token，沿用现有 AuthController |
| 审计 | 关键操作强制写入 `prod_audit_trail` |

### 1.4 代码结构规划

```
src/
├── MES.Api/
│   └── Controllers/
│       ├── IqcController.cs                 # IQC 来料检验
│       ├── FqcOqcController.cs              # FQC/OQC 终检
│       ├── NonconformingController.cs        # 不合格品/MRB
│       ├── WarehouseController.cs            # 仓储管理（入库/库存/发料/退料/有效期）
│       ├── FinishedGoodsController.cs        # 成品入库/出库
│       ├── AbnormalController.cs             # 异常上报/停线
│       ├── EquipmentMaintenanceController.cs # 设备故障/保养
│       ├── FirstArticleController.cs         # 首件检验
│       └── QualityAlertController.cs         # 紧急质量通知/召回
│
├── Server/
│   ├── Modules/
│   │   ├── MES.Services.Quality/
│   │   │   ├── IIqcService.cs
│   │   │   ├── IqcService.cs
│   │   │   ├── IFqcOqcService.cs
│   │   │   ├── FqcOqcService.cs
│   │   │   ├── INonconformingService.cs
│   │   │   ├── NonconformingService.cs
│   │   │   ├── IFirstArticleService.cs
│   │   │   ├── FirstArticleService.cs
│   │   │   ├── IQualityAlertService.cs
│   │   │   └── QualityAlertService.cs
│   │   │
│   │   ├── MES.Services.Warehouse/
│   │   │   ├── IWarehouseService.cs
│   │   │   ├── WarehouseService.cs
│   │   │   ├── IFinishedGoodsService.cs
│   │   │   └── FinishedGoodsService.cs
│   │   │
│   │   ├── MES.Services.Production/
│   │   │   ├── IAbnormalService.cs
│   │   │   ├── AbnormalService.cs
│   │   │   ├── IEquipmentMaintenanceService.cs
│   │   │   └── EquipmentMaintenanceService.cs
│   │   │
│   │   └── MES.Models/
│   │       └── Dtos/
│   │           ├── IqcDtos.cs
│   │           ├── FqcOqcDtos.cs
│   │           ├── NonconformingDtos.cs
│   │           ├── WarehouseDtos.cs
│   │           ├── FinishedGoodsDtos.cs
│   │           ├── AbnormalDtos.cs
│   │           ├── EquipmentMaintenanceDtos.cs
│   │           ├── FirstArticleDtos.cs
│   │           └── QualityAlertDtos.cs
│   │
│   └── Infrastructure/
│       └── MES.Infrastructure.Persistence/
│           ├── MesDbContext.cs               # 扩展 DbSets
│           └── Entities/
│               ├── QualityEntities.cs         # IQC/FQC/OQC/MRB 实体
│               ├── WarehouseEntities.cs       # 仓储实体
│               ├── ProductionAbnormalEntities.cs # 异常/停线实体
│               └── EquipmentMaintenanceEntities.cs # 设备故障/保养实体
│
└── docs/
    └── sql/
        └── migrations/
            ├── V4.0.0__iqc_incoming_material.sql
            ├── V4.0.1__fqc_oqc_inspection.sql
            ├── V4.0.2__nonconforming_mrb.sql
            ├── V4.0.3__warehouse_inbound.sql
            ├── V4.0.4__material_issue_return.sql
            ├── V4.0.5__finished_goods.sql
            ├── V4.0.6__abnormal_line_stop.sql
            ├── V4.0.7__equipment_maintenance.sql
            ├── V4.0.8__first_article_inspection.sql
            └── V4.0.9__quality_alert_recall.sql
```

---

## 二、模块一：IQC 来料检验管理

### 2.1 模块概述

IQC（Incoming Quality Control）负责所有来料（晶圆、辅料、包材）的入库前检验。检验任务由 ERP 到货通知或仓库收货单自动触发，IQC 检验员执行检验录入后，系统根据检验标准自动判定合格/不合格。合格批次放行入库，不合格批次自动隔离并触发 MRB 评审流程。

### 2.2 现状分析

| 现状 | 说明 |
|------|------|
| `quality_inspection` 表已存在 | 通用检验记录表，缺少 IQC 专用字段（供应商、批次号、MSL 等级等） |
| Quality 模块有 InspectionView UI | 前端界面已存在但缺少后端 API 支撑 |
| `master_material` 表已存在 | 物料主数据基础可用，需扩展供应商关联 |
| 无检验任务管理 | 缺少检验任务创建、分配、状态跟踪机制 |
| 无供应商质量管理 | 缺少供应商来料质量统计 |

### 2.3 功能清单

| 功能编号 | 功能名称 | 功能描述 | 优先级 |
|---------|---------|---------|--------|
| IQC-F01 | 检验任务创建 | ERP 到货通知/仓库收货后自动创建 IQC 检验任务 | P0 |
| IQC-F02 | 检验任务查询 | 按状态/供应商/物料/日期查询待检任务列表 | P0 |
| IQC-F03 | 检验结果录入 | IQC 检验员录入外观/尺寸/电性等检验项目结果 | P0 |
| IQC-F04 | 自动判定 | 根据 AQL 标准自动判定 Pass/Fail | P0 |
| IQC-F05 | 合格放行 | 检验合格的批次自动解除隔离，状态变为 Passed | P0 |
| IQC-F06 | 不合格隔离 | 检验不合格批次自动隔离，状态变为 Failed | P0 |
| IQC-F07 | MRB 触发 | 不合格自动创建 MRB 评审请求 | P0 |
| IQC-F08 | 批次追溯 | 关联供应商批次号、生产批次、检验记录 | P1 |
| IQC-F09 | 供应商质量统计 | 按供应商统计合格率/不合格率 | P2 |

### 2.4 接口设计

#### 2.4.1 API 接口清单

| 方法 | 路径 | 描述 | 请求体 | 返回类型 |
|------|------|------|--------|---------|
| POST | `/api/v1/iqc/tasks` | 创建检验任务 | `CreateIqcTaskRequest` | `IqcTaskResponse` |
| GET | `/api/v1/iqc/tasks` | 查询检验任务列表 | Query 参数 | `PagedResult<IqcTaskResponse>` |
| GET | `/api/v1/iqc/tasks/{taskId}` | 获取检验任务详情 | - | `IqcTaskDetailResponse` |
| POST | `/api/v1/iqc/tasks/{taskId}/execute` | 执行检验录入 | `ExecuteInspectionRequest` | `InspectionResultResponse` |
| POST | `/api/v1/iqc/tasks/{taskId}/judge` | 检验判定 | `JudgeRequest` | `IqcTaskResponse` |
| POST | `/api/v1/iqc/batches/{batchId}/isolate` | 一键隔离 | - | `ApiResponse` |
| POST | `/api/v1/iqc/batches/{batchId}/release` | 解除隔离 | - | `ApiResponse` |
| GET | `/api/v1/iqc/statistics` | IQC 统计 | Query 参数 | `IqcStatisticsResponse` |

#### 2.4.2 核心 DTO 定义

```csharp
// 创建检验任务
public class CreateIqcTaskRequest
{
    public string MaterialId { get; set; }
    public string MaterialName { get; set; }
    public string SupplierId { get; set; }
    public string SupplierName { get; set; }
    public string SupplierBatchNo { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; } = "PCS";
    public string PurchaseOrderNo { get; set; }
    public DateTime? ManufacturingDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? MslLevel { get; set; }
    public int? MslFloorLifeHours { get; set; }
    public string CoaReference { get; set; }
    public string ReceivedBy { get; set; }
    public string Remark { get; set; }
}

// 检验任务响应
public class IqcTaskResponse
{
    public string TaskId { get; set; }
    public string BatchId { get; set; }
    public string MaterialId { get; set; }
    public string MaterialName { get; set; }
    public string SupplierId { get; set; }
    public string SupplierName { get; set; }
    public string SupplierBatchNo { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; }
    public string InspectionStatus { get; set; }
    public string JudgmentResult { get; set; }
    public DateTime CreatedAt { get; set; }
}

// 检验结果录入
public class ExecuteInspectionRequest
{
    public List<InspectionItemInput> Items { get; set; }
    public string InspectorId { get; set; }
    public string InspectorName { get; set; }
    public string Remark { get; set; }
}

public class InspectionItemInput
{
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string StandardValue { get; set; }
    public string ActualValue { get; set; }
    public string LowerLimit { get; set; }
    public string UpperLimit { get; set; }
    public string Result { get; set; } // Pass/Fail
    public string Unit { get; set; }
}

// 判定请求
public class JudgeRequest
{
    public string Judgment { get; set; } // Pass/Fail/ConditionalPass
    public string Disposition { get; set; } // Accept/Reject/Return/Concession
    public string JudgeBy { get; set; }
    public string JudgeComment { get; set; }
}
```

#### 2.4.3 Service 接口

```csharp
public interface IIqcService
{
    Task<IqcTaskResponse> CreateTaskAsync(CreateIqcTaskRequest request);
    Task<PagedResult<IqcTaskResponse>> GetTasksAsync(IqcTaskQuery query);
    Task<IqcTaskDetailResponse> GetTaskDetailAsync(string taskId);
    Task<InspectionResultResponse> ExecuteInspectionAsync(string taskId, ExecuteInspectionRequest request);
    Task<IqcTaskResponse> JudgeAsync(string taskId, JudgeRequest request);
    Task<bool> IsolateBatchAsync(string batchId, string operatorId);
    Task<bool> ReleaseBatchAsync(string batchId, string operatorId);
    Task<IqcStatisticsResponse> GetStatisticsAsync(IqcStatisticsQuery query);
}
```

### 2.5 数据表变更

#### 2.5.1 新增表

```sql
-- 文件: docs/sql/migrations/V4.0.0__iqc_incoming_material.sql

-- IQC 来料批次表
CREATE TABLE IF NOT EXISTS `iqc_incoming_batch` (
    `batch_id` VARCHAR(50) NOT NULL COMMENT '批次ID (格式: IB-YYYYMMDD-NNN)',
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) DEFAULT NULL,
    `supplier_id` VARCHAR(50) NOT NULL,
    `supplier_name` VARCHAR(100) DEFAULT NULL,
    `supplier_batch_no` VARCHAR(100) NOT NULL COMMENT '供应商批次号',
    `quantity` INT NOT NULL DEFAULT 0,
    `unit` VARCHAR(20) DEFAULT 'PCS',
    `purchase_order_no` VARCHAR(50) DEFAULT NULL,
    `manufacturing_date` DATE DEFAULT NULL,
    `expiry_date` DATE DEFAULT NULL,
    `msl_level` INT DEFAULT NULL COMMENT '湿度敏感等级 1~6',
    `msl_floor_life_hours` INT DEFAULT NULL COMMENT '车间寿命(小时)',
    `msl_exposure_start` DATETIME DEFAULT NULL COMMENT '暴露开始时间',
    `msl_expiry` DATETIME DEFAULT NULL COMMENT 'MSL过期时间',
    `coa_reference` VARCHAR(100) DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'PendingInspection'
        COMMENT 'PendingInspection/Inspecting/Passed/Failed/Frozen',
    `inspection_status` VARCHAR(20) DEFAULT 'NotStarted'
        COMMENT 'NotStarted/InProgress/Completed',
    `judgment_result` VARCHAR(20) DEFAULT NULL COMMENT 'Pass/Fail/Conditional',
    `disposition` VARCHAR(20) DEFAULT NULL COMMENT 'Accept/Reject/Return/Concession',
    `iqc_task_id` VARCHAR(50) DEFAULT NULL,
    `ncr_id` VARCHAR(50) DEFAULT NULL,
    `mrb_id` VARCHAR(50) DEFAULT NULL,
    `warehouse_location` VARCHAR(50) DEFAULT NULL COMMENT '入库库位',
    `received_by` VARCHAR(50) DEFAULT NULL,
    `received_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`batch_id`),
    INDEX `idx_material_id` (`material_id`),
    INDEX `idx_supplier_id` (`supplier_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_supplier_batch_no` (`supplier_batch_no`),
    INDEX `idx_received_at` (`received_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- IQC 检验任务表
CREATE TABLE IF NOT EXISTS `iqc_inspection_task` (
    `task_id` VARCHAR(50) NOT NULL COMMENT '任务ID (格式: IQC-YYYYMMDD-NNN)',
    `batch_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `inspection_type` VARCHAR(50) NOT NULL DEFAULT 'Incoming'
        COMMENT 'Incoming/ReInspect/Special',
    `sampling_plan` VARCHAR(50) DEFAULT NULL COMMENT '抽样方案 (AQL标准)',
    `sample_size` INT DEFAULT NULL COMMENT '抽样数量',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/InProgress/Completed/Cancelled',
    `assigned_to` VARCHAR(50) DEFAULT NULL COMMENT '分配给谁',
    `assigned_at` DATETIME DEFAULT NULL,
    `started_at` DATETIME DEFAULT NULL,
    `completed_at` DATETIME DEFAULT NULL,
    `priority` VARCHAR(20) DEFAULT 'Normal' COMMENT 'Urgent/High/Normal/Low',
    `due_time` DATETIME DEFAULT NULL COMMENT '要求完成时间',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`task_id`),
    UNIQUE KEY `uk_batch_task` (`batch_id`),
    INDEX `idx_material_id` (`material_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_assigned_to` (`assigned_to`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- IQC 检验结果明细表
CREATE TABLE IF NOT EXISTS `iqc_inspection_result` (
    `result_id` VARCHAR(50) NOT NULL,
    `task_id` VARCHAR(50) NOT NULL,
    `batch_id` VARCHAR(50) NOT NULL,
    `item_code` VARCHAR(50) NOT NULL COMMENT '检验项目编号',
    `item_name` VARCHAR(100) NOT NULL,
    `item_type` VARCHAR(20) NOT NULL COMMENT 'Dimensional/Electrical/Visual/Functional',
    `standard_value` VARCHAR(50) DEFAULT NULL,
    `lower_limit` VARCHAR(50) DEFAULT NULL,
    `upper_limit` VARCHAR(50) DEFAULT NULL,
    `actual_value` VARCHAR(50) DEFAULT NULL,
    `unit` VARCHAR(20) DEFAULT NULL,
    `result` VARCHAR(20) NOT NULL COMMENT 'Pass/Fail/N/A',
    `measuring_equipment` VARCHAR(50) DEFAULT NULL COMMENT '测量设备',
    `inspector_id` VARCHAR(50) NOT NULL,
    `inspector_name` VARCHAR(100) DEFAULT NULL,
    `inspected_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `remark` TEXT DEFAULT NULL,
    PRIMARY KEY (`result_id`),
    INDEX `idx_task_id` (`task_id`),
    INDEX `idx_batch_id` (`batch_id`),
    INDEX `idx_item_code` (`item_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- IQC 检验标准表
CREATE TABLE IF NOT EXISTS `iqc_inspection_standard` (
    `standard_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `inspection_item_code` VARCHAR(50) NOT NULL,
    `inspection_item_name` VARCHAR(100) NOT NULL,
    `item_type` VARCHAR(20) NOT NULL,
    `standard_value` VARCHAR(50) DEFAULT NULL,
    `lower_limit` VARCHAR(50) DEFAULT NULL,
    `upper_limit` VARCHAR(50) DEFAULT NULL,
    `unit` VARCHAR(20) DEFAULT NULL,
    `sampling_plan` VARCHAR(50) DEFAULT 'AQL-0.65',
    `is_mandatory` TINYINT(1) NOT NULL DEFAULT 1,
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`standard_id`),
    UNIQUE KEY `uk_material_item` (`material_id`, `inspection_item_code`),
    INDEX `idx_material_id` (`material_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 供应商质量统计表
CREATE TABLE IF NOT EXISTS `iqc_supplier_quality_stat` (
    `stat_id` VARCHAR(50) NOT NULL,
    `supplier_id` VARCHAR(50) NOT NULL,
    `supplier_name` VARCHAR(100) DEFAULT NULL,
    `stat_month` VARCHAR(7) NOT NULL COMMENT '格式: YYYY-MM',
    `total_batches` INT NOT NULL DEFAULT 0,
    `passed_batches` INT NOT NULL DEFAULT 0,
    `failed_batches` INT NOT NULL DEFAULT 0,
    `pass_rate` DECIMAL(5,2) DEFAULT 0,
    `major_defect_count` INT NOT NULL DEFAULT 0,
    `minor_defect_count` INT NOT NULL DEFAULT 0,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`stat_id`),
    UNIQUE KEY `uk_supplier_month` (`supplier_id`, `stat_month`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### 2.5.2 扩展现有表

```sql
-- 扩展 master_material 增加 IQC 相关字段
ALTER TABLE `master_material`
    ADD COLUMN IF NOT EXISTS `inspection_required` TINYINT(1) DEFAULT 1
        COMMENT '是否需要IQC检验',
    ADD COLUMN IF NOT EXISTS `inspection_standard_id` VARCHAR(50) DEFAULT NULL
        COMMENT '默认检验标准ID',
    ADD COLUMN IF NOT EXISTS `msl_controlled` TINYINT(1) DEFAULT 0
        COMMENT '是否MSL管控';
```

### 2.6 业务规则

| 规则编号 | 规则描述 | 违反后果 |
|---------|---------|---------|
| IQC-R01 | 所有来料必须经过 IQC 检验才能入库（除非物料标记为 inspection_required=false） | 禁止入库 |
| IQC-R02 | 检验任务创建后 4 小时内必须开始检验（可根据物料 urgency 调整） | 超时自动告警升级 |
| IQC-R03 | 所有必填检验项目必须录入结果后才能提交判定 | 禁止提交 |
| IQC-R04 | AQL 判定：不合格数 > AQL 允许数 → 整批不合格 | 自动判定 Fail |
| IQC-R05 | 不合格批次自动触发隔离（status → Failed）并创建 NCR | 系统自动执行 |
| IQC-R06 | NCR 创建后自动触发 MRB 评审流程 | 系统自动执行 |
| IQC-R07 | MSL 管控物料必须记录 MSL 等级和暴露时间 | 禁止入库 |
| IQC-R08 | 已隔离批次禁止任何出入库操作 | 系统拦截 |

### 2.7 状态机

```
IQC 检验任务状态机：
  Pending → InProgress → Completed
     ↓           ↓
  Cancelled   (自动判定)
  
来料批次状态机：
  PendingInspection → Inspecting → Passed → (入库)
                             ↓
                          Failed → (隔离 → MRB → 处置)
                             ↓
                          Frozen (质量警报冻结)
```

### 2.8 依赖关系

| 依赖 | 类型 | 说明 |
|------|------|------|
| `master_material` | 已有 | 物料主数据 |
| `master_equipment` | 已有 | 测量设备关联 |
| MRB 评审模块 | 本阶段 | IQC 不合格触发 MRB |
| 仓储入库模块 | 本阶段 | IQC 合格后触发入库 |

---

## 三、模块二：FQC/OQC 终检管理

### 3.1 模块概述

FQC（Final Quality Control）在生产完工后自动触发终检，确保产品符合出货标准。OQC（Outgoing Quality Control）在出货前强制抽检，验证包装、标签、MSL 出货条件。FQC 合格后自动触发成品入库流程。

### 3.2 现状分析

| 现状 | 说明 |
|------|------|
| `quality_inspection` 表已存在 | 通用检验表可扩展用于 FQC/OQC |
| Quality 模块有相关 UI | InspectionView 等界面存在 |
| 无终检触发机制 | 缺少工序完工→FQC 自动触发逻辑 |
| 无 OQC 出货检验 | 出货前无强制检验拦截 |
| 无 MSL 出货检查 | 出货前未验证 MSL 有效期 |

### 3.3 功能清单

| 功能编号 | 功能名称 | 功能描述 | 优先级 |
|---------|---------|---------|--------|
| FQC-F01 | FQC 任务自动触发 | 工单/批次完成所有工序后自动创建 FQC 任务 | P0 |
| FQC-F02 | FQC 检验录入 | 录入终检项目（外观/功能/电性/包装） | P0 |
| FQC-F03 | FQC 自动判定 | 根据检验标准判定合格/不合格 | P0 |
| FQC-F04 | FQC 合格→入库 | FQC 合格后自动触发成品入库 | P0 |
| FQC-F05 | OQC 任务创建 | 出货前创建 OQC 检验任务 | P0 |
| OQC-F06 | OQC 抽检录入 | 录入 AQL 抽检结果 | P0 |
| OQC-F07 | MSL 出货检查 | 出货前验证 MSL 等级、烘烤记录、剩余寿命 | P0 |
| OQC-F08 | 出货拦截 | OQC 不合格或 MSL 过期则禁止出货 | P0 |

### 3.4 接口设计

#### 3.4.1 API 接口清单

| 方法 | 路径 | 描述 |
|------|------|------|
| GET | `/api/v1/fqc/tasks` | 查询 FQC 待检任务列表 |
| GET | `/api/v1/fqc/tasks/{taskId}` | 获取 FQC 任务详情 |
| POST | `/api/v1/fqc/tasks/{taskId}/execute` | 执行 FQC 检验 |
| GET | `/api/v1/oqc/tasks` | 查询 OQC 出货检验任务 |
| GET | `/api/v1/oqc/tasks/{taskId}` | 获取 OQC 任务详情 |
| POST | `/api/v1/oqc/tasks/{taskId}/execute` | 执行 OQC 检验 |
| POST | `/api/v1/oqc/msl-check` | MSL 出货检查 |

#### 3.4.2 核心 DTO

```csharp
public class FqcTaskResponse
{
    public string TaskId { get; set; }
    public string LotId { get; set; }
    public string WorkOrderId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } // Pending/InProgress/Passed/Failed
    public string JudgmentResult { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class ExecuteFqcRequest
{
    public List<InspectionItemInput> Items { get; set; }
    public string InspectorId { get; set; }
    public string InspectorName { get; set; }
    public string Remark { get; set; }
}

public class OqcMslCheckRequest
{
    public string LotId { get; set; }
    public string BatchId { get; set; }
    public DateTime PlannedShipDate { get; set; }
}

public class MslCheckResult
{
    public bool Passed { get; set; }
    public string LotId { get; set; }
    public int? MslLevel { get; set; }
    public DateTime? MslExposureStart { get; set; }
    public DateTime? MslExpiry { get; set; }
    public int RemainingFloorLifeHours { get; set; }
    public string Result { get; set; } // Pass/Fail/Expired
    public string FailureReason { get; set; }
}
```

#### 3.4.3 Service 接口

```csharp
public interface IFqcOqcService
{
    // FQC
    Task<FqcTaskResponse> CreateFqcTaskAsync(string lotId, string workOrderId);
    Task<PagedResult<FqcTaskResponse>> GetFqcTasksAsync(FqcTaskQuery query);
    Task<FqcTaskResponse> ExecuteFqcAsync(string taskId, ExecuteFqcRequest request);

    // OQC
    Task<OqcTaskResponse> CreateOqcTaskAsync(string lotId, string shipmentId);
    Task<PagedResult<OqcTaskResponse>> GetOqcTasksAsync(OqcTaskQuery query);
    Task<OqcTaskResponse> ExecuteOqcAsync(string taskId, ExecuteOqcRequest request);

    // MSL
    Task<MslCheckResult> CheckMslForShipmentAsync(OqcMslCheckRequest request);
}
```

### 3.5 数据表变更

```sql
-- 文件: docs/sql/migrations/V4.0.1__fqc_oqc_inspection.sql

-- FQC 终检记录表
CREATE TABLE IF NOT EXISTS `fqc_inspection_record` (
    `record_id` VARCHAR(50) NOT NULL,
    `task_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `work_order_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `inspection_type` VARCHAR(50) NOT NULL DEFAULT 'Final',
    `quantity` INT NOT NULL DEFAULT 0,
    `sample_size` INT DEFAULT NULL,
    `result` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/InProgress/Passed/Failed',
    `judgment` VARCHAR(20) DEFAULT NULL COMMENT 'Pass/Fail',
    `inspector_id` VARCHAR(50) NOT NULL,
    `inspector_name` VARCHAR(100) DEFAULT NULL,
    `inspection_time` DATETIME DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`record_id`),
    UNIQUE KEY `uk_lot_fqc` (`lot_id`),
    INDEX `idx_task_id` (`task_id`),
    INDEX `idx_work_order_id` (`work_order_id`),
    INDEX `idx_result` (`result`),
    INDEX `idx_inspection_time` (`inspection_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- OQC 出货检验记录表
CREATE TABLE IF NOT EXISTS `oqc_inspection_record` (
    `record_id` VARCHAR(50) NOT NULL,
    `task_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `shipment_id` VARCHAR(50) DEFAULT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `quantity` INT NOT NULL DEFAULT 0,
    `sample_size` INT DEFAULT NULL,
    `aql_standard` VARCHAR(50) DEFAULT 'AQL-1.0',
    `result` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/InProgress/Passed/Failed',
    `judgment` VARCHAR(20) DEFAULT NULL,
    `packaging_check` TINYINT(1) DEFAULT NULL COMMENT '包装检查是否通过',
    `label_check` TINYINT(1) DEFAULT NULL COMMENT '标签检查是否通过',
    `documentation_check` TINYINT(1) DEFAULT NULL COMMENT '文件检查是否通过',
    `inspector_id` VARCHAR(50) NOT NULL,
    `inspector_name` VARCHAR(100) DEFAULT NULL,
    `inspection_time` DATETIME DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`record_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_shipment_id` (`shipment_id`),
    INDEX `idx_result` (`result`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 出货 MSL 检查记录表
CREATE TABLE IF NOT EXISTS `shipment_msl_check` (
    `check_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `batch_id` VARCHAR(50) DEFAULT NULL,
    `msl_level` INT DEFAULT NULL,
    `exposure_start_time` DATETIME DEFAULT NULL,
    `msl_expiry_time` DATETIME DEFAULT NULL,
    `bake_record_id` VARCHAR(50) DEFAULT NULL COMMENT '最近烘烤记录',
    `remaining_floor_life_hours` INT DEFAULT NULL,
    `check_result` VARCHAR(20) NOT NULL COMMENT 'Pass/Fail/Expired',
    `failure_reason` VARCHAR(255) DEFAULT NULL,
    `checked_by` VARCHAR(50) NOT NULL,
    `checked_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`check_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_check_result` (`check_result`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 3.6 业务规则

| 规则编号 | 规则描述 | 违反后果 |
|---------|---------|---------|
| FQC-R01 | 工单所有工序完成后自动创建 FQC 任务 | 系统自动执行 |
| FQC-R02 | FQC 未通过禁止触发成品入库 | 系统拦截 |
| OQC-R01 | 出货前必须完成 OQC 检验 | 禁止创建出货单 |
| OQC-R02 | OQC 包装/标签/文件三项检查必须全部通过 | 禁止出货 |
| OQC-R03 | MSL 管控物料出货前必须验证剩余寿命 ≥ 72 小时 | 禁止出货 |
| OQC-R04 | MSL 过期物料必须先烘烤再出货 | 禁止出货 |

### 3.7 状态机

```
FQC 任务：Pending → InProgress → Passed → (自动入库)
                                  → Failed → (隔离 → MRB)

OQC 任务：Pending → InProgress → Passed → (允许出货)
                                → Failed → (禁止出货 → 重新检验)
```

### 3.8 依赖关系

| 依赖 | 类型 | 说明 |
|------|------|------|
| `prod_lot` / `prod_work_order` | 已有 | 工单/批次状态 |
| IQC 模块 | 本阶段 | 来料批次 MSL 信息 |
| 成品入库模块 | 本阶段 | FQC 合格→入库 |
| MRB 模块 | 本阶段 | 不合格触发 MRB |

---

## 四、模块三：不合格品管理与 MRB 评审

### 4.1 模块概述

不合格品管理涵盖从 IQC/FQC/OQC/制程中发现的不合格品的标识、隔离、记录、MRB（Material Review Board）评审到最终处置（返工/报废/让步接收/退货）的全流程闭环。MRB 评审需要质量、工艺、工程三方会签。

### 4.2 现状分析

| 现状 | 说明 |
|------|------|
| `prod_lot.is_under_mrb` 字段存在 | 已有标记字段但无完整流程 |
| `prod_hold_record` 表存在 | 可部分支撑不合格品隔离 |
| Complaint8DService 已实现 | 8D 审批流模式可复用 |
| 无 NCR 管理 | 缺少不合格品记录表 |
| 无 MRB 评审流程 | 缺少评审表和会签机制 |
| 无处置执行跟踪 | 返工/报废/让步缺少执行记录 |

### 4.3 功能清单

| 功能编号 | 功能名称 | 功能描述 | 优先级 |
|---------|---------|---------|--------|
| NCR-F01 | 不合格品记录创建 | 检验不合格/制程异常时自动/手动创建 NCR | P0 |
| NCR-F02 | 不合格品隔离 | 自动锁定相关批次，禁止出入库 | P0 |
| NCR-F03 | NCR 查询 | 按来源/状态/日期查询不合格品记录 | P0 |
| MRB-F01 | 发起 MRB 评审 | NCR 创建后自动/手动触发 MRB | P0 |
| MRB-F02 | MRB 会签 | 质量/工艺/工程三方在线会签 | P0 |
| MRB-F03 | MRB 处置决策 | 返工/报废/让步/退货决策 | P0 |
| MRB-F04 | 处置执行跟踪 | 跟踪处置执行状态和结果 | P0 |
| MRB-F05 | 返工重检验证 | 返工完成后触发重检验证 | P0 |

### 4.4 接口设计

#### 4.4.1 API 接口清单

| 方法 | 路径 | 描述 |
|------|------|------|
| POST | `/api/v1/nonconforming/records` | 创建不合格品记录 |
| GET | `/api/v1/nonconforming/records` | 查询不合格品列表 |
| GET | `/api/v1/nonconforming/records/{ncrId}` | 获取详情 |
| POST | `/api/v1/nonconforming/records/{ncrId}/isolate` | 隔离不合格品 |
| POST | `/api/v1/nonconforming/records/{ncrId}/mrb` | 发起 MRB 评审 |
| GET | `/api/v1/mrb/reviews/{mrbId}` | 获取 MRB 评审详情 |
| POST | `/api/v1/mrb/reviews/{mrbId}/vote` | MRB 会签投票 |
| POST | `/api/v1/nonconforming/records/{ncrId}/disposition` | 处置执行 |
| POST | `/api/v1/nonconforming/records/{ncrId}/rework-verify` | 返工重检验证 |

#### 4.4.2 核心 DTO

```csharp
public class CreateNonconformingRequest
{
    public string Source { get; set; } // IQC/FQC/OQC/Process/Audit
    public string SourceReference { get; set; } // 来源单号
    public string LotId { get; set; }
    public string BatchId { get; set; }
    public string ProductId { get; set; }
    public string DefectCode { get; set; }
    public string DefectDescription { get; set; }
    public string DefectCategory { get; set; } // Critical/Major/Minor
    public int AffectedQty { get; set; }
    public string ReportedBy { get; set; }
    public string ReportedByName { get; set; }
    public string Remark { get; set; }
}

public class NonconformingRecordResponse
{
    public string NcrId { get; set; }
    public string Source { get; set; }
    public string LotId { get; set; }
    public string ProductId { get; set; }
    public string DefectCode { get; set; }
    public string DefectCategory { get; set; }
    public int AffectedQty { get; set; }
    public string IsolationStatus { get; set; }
    public string MrbStatus { get; set; }
    public string Disposition { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MrbVoteRequest
{
    public string VoterId { get; set; }
    public string VoterRole { get; set; } // Quality/Process/Engineering
    public string Vote { get; set; } // Approve/Reject/ConditionalApprove
    public string DispositionRecommendation { get; set; }
    public string Comment { get; set; }
}

public class DispositionRequest
{
    public string Disposition { get; set; } // Rework/Scrap/Concession/Return
    public string DispositionDetail { get; set; }
    public string ApprovedBy { get; set; }
    public string ApprovalComment { get; set; }
}
```

### 4.5 数据表变更

```sql
-- 文件: docs/sql/migrations/V4.0.2__nonconforming_mrb.sql

-- 不合格品记录表 (NCR)
CREATE TABLE IF NOT EXISTS `nonconforming_record` (
    `ncr_id` VARCHAR(50) NOT NULL COMMENT '格式: NCR-YYYYMMDD-NNN',
    `source` VARCHAR(50) NOT NULL COMMENT 'IQC/FQC/OQC/Process/Audit/Customer',
    `source_reference` VARCHAR(50) DEFAULT NULL COMMENT '来源检验任务ID或工单号',
    `lot_id` VARCHAR(50) NOT NULL,
    `batch_id` VARCHAR(50) DEFAULT NULL,
    `work_order_id` VARCHAR(50) DEFAULT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `product_name` VARCHAR(100) DEFAULT NULL,
    `defect_code` VARCHAR(50) NOT NULL,
    `defect_description` TEXT NOT NULL,
    `defect_category` VARCHAR(20) NOT NULL COMMENT 'Critical/Major/Minor',
    `affected_qty` INT NOT NULL DEFAULT 0,
    `reported_by` VARCHAR(50) NOT NULL,
    `reported_by_name` VARCHAR(100) DEFAULT NULL,
    `reported_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `isolation_status` VARCHAR(20) NOT NULL DEFAULT 'Isolated'
        COMMENT 'Isolated/Released',
    `isolation_time` DATETIME DEFAULT NULL,
    `mrb_id` VARCHAR(50) DEFAULT NULL,
    `mrb_status` VARCHAR(20) DEFAULT NULL
        COMMENT 'NotRequired/Pending/InProgress/Completed/Cancelled',
    `disposition` VARCHAR(20) DEFAULT NULL
        COMMENT 'Rework/Scrap/Concession/Return/UseAsIs',
    `disposition_detail` TEXT DEFAULT NULL,
    `disposition_executed` TINYINT(1) DEFAULT 0 COMMENT '处置是否已执行',
    `disposition_executed_at` DATETIME DEFAULT NULL,
    `rework_verify_result` VARCHAR(20) DEFAULT NULL COMMENT 'Pass/Fail',
    `rework_verify_time` DATETIME DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Open'
        COMMENT 'Open/InReview/Disposed/Closed/Cancelled',
    `closed_at` DATETIME DEFAULT NULL,
    `closed_by` VARCHAR(50) DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`ncr_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_product_id` (`product_id`),
    INDEX `idx_source` (`source`),
    INDEX `idx_status` (`status`),
    INDEX `idx_defect_code` (`defect_code`),
    INDEX `idx_reported_at` (`reported_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- MRB 评审表
CREATE TABLE IF NOT EXISTS `mrb_review` (
    `mrb_id` VARCHAR(50) NOT NULL COMMENT '格式: MRB-YYYYMMDD-NNN',
    `ncr_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `affected_qty` INT NOT NULL DEFAULT 0,
    `review_type` VARCHAR(20) DEFAULT 'Standard'
        COMMENT 'Standard/Expedited/Escalated',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/InProgress/Completed/Cancelled',
    `quality_vote` VARCHAR(20) DEFAULT NULL COMMENT 'Approve/Reject/ConditionalApprove',
    `quality_voter` VARCHAR(50) DEFAULT NULL,
    `quality_vote_time` DATETIME DEFAULT NULL,
    `quality_comment` TEXT DEFAULT NULL,
    `process_vote` VARCHAR(20) DEFAULT NULL,
    `process_voter` VARCHAR(50) DEFAULT NULL,
    `process_vote_time` DATETIME DEFAULT NULL,
    `process_comment` TEXT DEFAULT NULL,
    `engineering_vote` VARCHAR(20) DEFAULT NULL,
    `engineering_voter` VARCHAR(50) DEFAULT NULL,
    `engineering_vote_time` DATETIME DEFAULT NULL,
    `engineering_comment` TEXT DEFAULT NULL,
    `final_disposition` VARCHAR(20) DEFAULT NULL
        COMMENT 'Rework/Scrap/Concession/Return/UseAsIs',
    `final_decision_by` VARCHAR(50) DEFAULT NULL,
    `final_decision_time` DATETIME DEFAULT NULL,
    `concession_limit` VARCHAR(100) DEFAULT NULL COMMENT '让步条件',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `completed_at` DATETIME DEFAULT NULL,
    PRIMARY KEY (`mrb_id`),
    UNIQUE KEY `uk_ncr_mrb` (`ncr_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- MRB 评审明细表
CREATE TABLE IF NOT EXISTS `mrb_review_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `mrb_id` VARCHAR(50) NOT NULL,
    `voter_role` VARCHAR(50) NOT NULL COMMENT 'Quality/Process/Engineering',
    `voter_id` VARCHAR(50) NOT NULL,
    `voter_name` VARCHAR(100) DEFAULT NULL,
    `vote` VARCHAR(20) NOT NULL COMMENT 'Approve/Reject/ConditionalApprove',
    `disposition_recommendation` VARCHAR(50) DEFAULT NULL,
    `comment` TEXT DEFAULT NULL,
    `vote_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`item_id`),
    UNIQUE KEY `uk_mrb_role` (`mrb_id`, `voter_role`),
    INDEX `idx_mrb_id` (`mrb_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 处置执行记录表
CREATE TABLE IF NOT EXISTS `disposition_record` (
    `disposition_id` VARCHAR(50) NOT NULL,
    `ncr_id` VARCHAR(50) NOT NULL,
    `mrb_id` VARCHAR(50) DEFAULT NULL,
    `disposition_type` VARCHAR(20) NOT NULL
        COMMENT 'Rework/Scrap/Concession/Return/UseAsIs',
    `detail` TEXT DEFAULT NULL,
    `executed_by` VARCHAR(50) NOT NULL,
    `executed_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `verified_by` VARCHAR(50) DEFAULT NULL,
    `verified_at` DATETIME DEFAULT NULL,
    `verify_result` VARCHAR(20) DEFAULT NULL,
    `related_work_order_id` VARCHAR(50) DEFAULT NULL COMMENT '返工工单号',
    `related_scrap_id` VARCHAR(50) DEFAULT NULL COMMENT '报废单号',
    PRIMARY KEY (`disposition_id`),
    INDEX `idx_ncr_id` (`ncr_id`),
    INDEX `idx_disposition_type` (`disposition_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 4.6 业务规则

| 规则编号 | 规则描述 | 违反后果 |
|---------|---------|---------|
| NCR-R01 | 检验不合格自动创建 NCR | 系统自动执行 |
| NCR-R02 | NCR 创建后立即隔离相关批次 | 系统自动执行 |
| NCR-R03 | Critical 缺陷必须走 MRB 评审 | Major/Minor 可快速处置 |
| MRB-R01 | MRB 必须质量+工艺+工程三方会签 | 缺一不可 |
| MRB-R02 | 三方全部 Approve 才形成决议 | 任一方 Reject 需重新评审 |
| MRB-R03 | MRB 决议必须在 24 小时内完成 | 超时自动升级 |
| DISP-R01 | 返工必须创建返工工单并跟踪 | 禁止无单返工 |
| DISP-R02 | 报废必须经过审批（电子签名） | 禁止无审批报废 |
| DISP-R03 | 让步必须记录条件和限制 | 条件过期自动失效 |
| DISP-R04 | 返工完成后必须重检验证 | 验证不合格重新 MRB |

### 4.7 依赖关系

| 依赖 | 类型 | 说明 |
|------|------|------|
| IQC/FQC/OQC 模块 | 本阶段 | 不合格品来源 |
| 仓储模块 | 本阶段 | 隔离/释放库存 |
| 返工记录 | 已有 | `prod_rework_record` |
| 报废记录 | 已有 | `prod_scrap_record` |
| 电子签名 | 已有 | `prod_signature` |

---

## 五、模块四：原材料入库 / FIFO / 有效期

### 5.1 模块概述

原材料入库管理涵盖 IQC 合格后物料的入库登记、库位分配、批次台账管理。FIFO（先进先出）强制在发料时优先使用最早入库的批次。有效期管理对有时效要求的物料进行自动预警和到期锁定。

### 5.2 现状分析

| 现状 | 说明 |
|------|------|
| `master_material` 表已存在 | 有 current_stock 字段但无批次管理 |
| `material_consume` 表已存在 | 有消耗记录但无发料单管理 |
| 无入库单管理 | 缺少原材料入库单和批次台账 |
| 无库位管理 | 缺少库位定义和库位-批次关联 |
| 无 FIFO 机制 | 缺少按入库时间排序的发料推荐 |
| 无有效期预警 | 缺少到期自动锁定机制 |

### 5.3 功能清单

| 功能编号 | 功能名称 | 功能描述 | 优先级 |
|---------|---------|---------|--------|
| WH-F01 | 原材料入库 | IQC 合格后创建入库单，分配库位 | P0 |
| WH-F02 | 批次台账 | 维护入库批次、数量、库位、有效期 | P0 |
| WH-F03 | 库存查询 | 按物料/批次/库位查询库存 | P0 |
| WH-F04 | FIFO 发料推荐 | 按入库时间排序推荐发料批次 | P0 |
| WH-F05 | 强制 FIFO | 跳过 FIFO 需审批 | P0 |
| WH-F06 | 有效期预警 | 提前 N 天预警即将过期物料 | P0 |
| WH-F07 | 到期自动锁定 | 过期物料自动锁定禁止使用 | P0 |
| WH-F08 | 批次锁定/解锁 | 手动锁定/解锁指定批次 | P0 |
| WH-F09 | 库存调整 | 盘点差异/损耗调整 | P1 |

### 5.4 接口设计

#### 5.4.1 API 接口清单

| 方法 | 路径 | 描述 |
|------|------|------|
| POST | `/api/v1/warehouse/receipt` | 原材料入库 |
| GET | `/api/v1/warehouse/receipts` | 入库单查询 |
| GET | `/api/v1/warehouse/receipts/{receiptId}` | 入库单详情 |
| GET | `/api/v1/warehouse/inventory` | 库存查询 |
| GET | `/api/v1/warehouse/inventory/{materialId}` | 物料库存查询 |
| GET | `/api/v1/warehouse/inventory/fifo-recommend` | FIFO 发料推荐 |
| GET | `/api/v1/warehouse/expiry-warnings` | 有效期预警 |
| POST | `/api/v1/warehouse/batches/{batchId}/lock` | 批次锁定 |
| POST | `/api/v1/warehouse/batches/{batchId}/unlock` | 批次解锁 |
| PUT | `/api/v1/warehouse/inventory/adjust` | 库存调整 |

#### 5.4.2 核心 DTO

```csharp
public class WarehouseReceiptRequest
{
    public string BatchId { get; set; }
    public string MaterialId { get; set; }
    public string MaterialName { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; }
    public string SupplierId { get; set; }
    public string SupplierBatchNo { get; set; }
    public string LocationId { get; set; }
    public string LocationName { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? ShelfLifeDays { get; set; }
    public int? MslLevel { get; set; }
    public int? MslFloorLifeHours { get; set; }
    public string IqcTaskId { get; set; }
    public string ReceivedBy { get; set; }
    public string Remark { get; set; }
}

public class InventoryResponse
{
    public string MaterialId { get; set; }
    public string MaterialName { get; set; }
    public string BatchId { get; set; }
    public string LocationId { get; set; }
    public string LocationName { get; set; }
    public int AvailableQty { get; set; }
    public int LockedQty { get; set; }
    public string Unit { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int RemainingDays { get; set; }
    public string Status { get; set; } // Available/Locked/Expiring/Expired
    public int? MslLevel { get; set; }
    public DateTime ReceivedAt { get; set; }
    public bool IsFifoEligible { get; set; }
}

public class FifoRecommendResponse
{
    public string MaterialId { get; set; }
    public int RequestedQty { get; set; }
    public List<FifoBatchRecommendation> Recommendations { get; set; }
}

public class FifoBatchRecommendation
{
    public string BatchId { get; set; }
    public string LocationId { get; set; }
    public int AvailableQty { get; set; }
    public int RecommendQty { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string SupplierBatchNo { get; set; }
}

public class ExpiryWarningResponse
{
    public string MaterialId { get; set; }
    public string MaterialName { get; set; }
    public string BatchId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int RemainingDays { get; set; }
    public int Quantity { get; set; }
    public string LocationId { get; set; }
    public string WarningLevel { get; set; } // Warning/Urgent/Expired
}
```

#### 5.4.3 Service 接口

```csharp
public interface IWarehouseService
{
    Task<WarehouseReceiptResponse> ReceiveMaterialAsync(WarehouseReceiptRequest request);
    Task<PagedResult<InventoryResponse>> GetInventoryAsync(InventoryQuery query);
    Task<FifoRecommendResponse> GetFifoRecommendationAsync(string materialId, int requestedQty);
    Task<List<ExpiryWarningResponse>> GetExpiryWarningsAsync(int warningDays);
    Task<bool> LockBatchAsync(string batchId, string reason, string operatorId);
    Task<bool> UnlockBatchAsync(string batchId, string operatorId);
    Task<bool> AdjustInventoryAsync(InventoryAdjustRequest request);
}
```

### 5.5 数据表变更

```sql
-- 文件: docs/sql/migrations/V4.0.3__warehouse_inbound.sql

-- 原材料入库单表
CREATE TABLE IF NOT EXISTS `warehouse_receipt` (
    `receipt_id` VARCHAR(50) NOT NULL COMMENT '格式: WR-YYYYMMDD-NNN',
    `batch_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) DEFAULT NULL,
    `quantity` INT NOT NULL DEFAULT 0,
    `unit` VARCHAR(20) DEFAULT 'PCS',
    `supplier_id` VARCHAR(50) DEFAULT NULL,
    `supplier_batch_no` VARCHAR(100) DEFAULT NULL,
    `location_id` VARCHAR(50) NOT NULL COMMENT '目标库位',
    `location_name` VARCHAR(100) DEFAULT NULL,
    `expiry_date` DATE DEFAULT NULL,
    `shelf_life_days` INT DEFAULT NULL,
    `msl_level` INT DEFAULT NULL,
    `msl_floor_life_hours` INT DEFAULT NULL,
    `iqc_task_id` VARCHAR(50) DEFAULT NULL,
    `purchase_order_no` VARCHAR(50) DEFAULT NULL,
    `received_by` VARCHAR(50) NOT NULL,
    `received_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Completed' COMMENT 'Pending/Completed/Cancelled',
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`receipt_id`),
    UNIQUE KEY `uk_batch_receipt` (`batch_id`),
    INDEX `idx_material_id` (`material_id`),
    INDEX `idx_location_id` (`location_id`),
    INDEX `idx_received_at` (`received_at`),
    INDEX `idx_expiry_date` (`expiry_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 库存台账表
CREATE TABLE IF NOT EXISTS `warehouse_inventory` (
    `inventory_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) DEFAULT NULL,
    `batch_id` VARCHAR(50) NOT NULL,
    `supplier_batch_no` VARCHAR(100) DEFAULT NULL,
    `location_id` VARCHAR(50) NOT NULL,
    `location_name` VARCHAR(100) DEFAULT NULL,
    `total_qty` INT NOT NULL DEFAULT 0,
    `available_qty` INT NOT NULL DEFAULT 0,
    `locked_qty` INT NOT NULL DEFAULT 0,
    `allocated_qty` INT NOT NULL DEFAULT 0,
    `unit` VARCHAR(20) DEFAULT 'PCS',
    `expiry_date` DATE DEFAULT NULL,
    `shelf_life_days` INT DEFAULT NULL,
    `msl_level` INT DEFAULT NULL,
    `msl_floor_life_hours` INT DEFAULT NULL,
    `msl_exposure_start` DATETIME DEFAULT NULL,
    `msl_expiry` DATETIME DEFAULT NULL,
    `received_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Available'
        COMMENT 'Available/PartiallyUsed/Locked/Expired',
    `lock_reason` VARCHAR(255) DEFAULT NULL,
    `last_issue_at` DATETIME DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`inventory_id`),
    UNIQUE KEY `uk_batch_location` (`batch_id`, `location_id`),
    INDEX `idx_material_id` (`material_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_expiry_date` (`expiry_date`),
    INDEX `idx_received_at` (`received_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 库位表
CREATE TABLE IF NOT EXISTS `warehouse_location` (
    `location_id` VARCHAR(50) NOT NULL COMMENT '格式: WH-A-01',
    `location_name` VARCHAR(100) NOT NULL,
    `warehouse_zone` VARCHAR(50) NOT NULL COMMENT 'Warehouse/LineSide/Quarantine',
    `location_type` VARCHAR(50) DEFAULT NULL COMMENT 'Shelf/Rack/Bin/Floor',
    `temperature_controlled` TINYINT(1) DEFAULT 0,
    `humidity_controlled` TINYINT(1) DEFAULT 0,
    `max_capacity` INT DEFAULT NULL,
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`location_id`),
    INDEX `idx_zone` (`warehouse_zone`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 物料有效期记录表
CREATE TABLE IF NOT EXISTS `material_shelf_life` (
    `shelf_life_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `batch_id` VARCHAR(50) NOT NULL,
    `shelf_life_days` INT NOT NULL,
    `manufacturing_date` DATE DEFAULT NULL,
    `expiry_date` DATE NOT NULL,
    `warning_days_before` INT DEFAULT 30 COMMENT '预警天数',
    `alert_level` VARCHAR(20) NOT NULL DEFAULT 'Normal'
        COMMENT 'Normal/Warning/Urgent/Expired',
    `is_expired` TINYINT(1) NOT NULL DEFAULT 0,
    `locked_at` DATETIME DEFAULT NULL,
    `lock_reason` VARCHAR(255) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`shelf_life_id`),
    UNIQUE KEY `uk_batch_shelf` (`batch_id`),
    INDEX `idx_material_id` (`material_id`),
    INDEX `idx_expiry_date` (`expiry_date`),
    INDEX `idx_alert_level` (`alert_level`),
    INDEX `idx_is_expired` (`is_expired`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 5.6 业务规则

| 规则编号 | 规则描述 | 违反后果 |
|---------|---------|---------|
| WH-R01 | IQC 合格后才允许入库 | 系统拦截 |
| WH-R02 | 入库必须指定库位 | 禁止入库 |
| WH-R03 | 有有效期的物料必须记录 expiry_date | 禁止入库 |
| WH-R04 | 发料时按 received_at 升序推荐（FIFO） | 系统自动排序 |
| WH-R05 | 跳过 FIFO 推荐需审批并记录原因 | 记录审批日志 |
| WH-R06 | expiry_date ≤ 当前日期 → 自动锁定 | 系统自动执行 |
| WH-R07 | expiry_date ≤ 当前日期 + warning_days → 预警 | 发送预警通知 |
| WH-R08 | MSL 过期（msl_expiry < now）→ 自动锁定 | 系统自动执行 |
| WH-R09 | locked_qty > 0 的批次禁止发料 | 系统拦截 |

### 5.7 依赖关系

| 依赖 | 类型 | 说明 |
|------|------|------|
| IQC 模块 | 本阶段 | 合格批次信息 |
| `master_material` | 已有 | 物料主数据 |
| 发料/退料模块 | 本阶段 | 出库操作 |

---

## 六、模块五：产线发料/退料管理

### 6.1 模块概述

产线发料管理工单所需的物料从仓库发放到产线的过程，强制 FIFO 和有效期校验。退料管理产线未使用物料退回仓库的流程，包含剩余数量登记和库位重新分配。

### 6.2 现状分析

| 现状 | 说明 |
|------|------|
| `material_consume` 表已存在 | 有消耗记录但无发料单管理 |
| `material_requirement` 表已存在 | 有工序物料需求定义 |
| 无发料单管理 | 缺少发料申请、审批、执行流程 |
| 无退料管理 | 缺少退料登记和库存回冲 |
| 无工单物料齐套检查 | 缺少发料前齐套性校验 |

### 6.3 功能清单

| 功能编号 | 功能名称 | 功能描述 | 优先级 |
|---------|---------|---------|--------|
| ISSUE-F01 | 发料申请 | 按工单/工序创建发料申请 | P0 |
| ISSUE-F02 | 齐套检查 | 检查工单所需物料是否齐套 | P0 |
| ISSUE-F03 | FIFO 发料执行 | 按 FIFO 推荐批次执行发料 | P0 |
| ISSUE-F04 | 发料记录 | 记录发料批次、数量、接收人 | P0 |
| ISSUE-F05 | 非 FIFO 发料审批 | 跳过 FIFO 需审批 | P0 |
| RETURN-F01 | 退料申请 | 产线未使用物料退料申请 | P0 |
| RETURN-F02 | 退料执行 | 退料入库、库存回冲 | P0 |
| RETURN-F03 | 退料有效期检查 | 退料时检查是否过期 | P0 |

### 6.4 接口设计

#### 6.4.1 API 接口清单

| 方法 | 路径 | 描述 |
|------|------|------|
| POST | `/api/v1/warehouse/issue` | 产线发料 |
| GET | `/api/v1/warehouse/issue-orders` | 发料单查询 |
| POST | `/api/v1/warehouse/issue/check-kit` | 齐套检查 |
| POST | `/api/v1/warehouse/issue/skip-fifo` | 非FIFO发料审批 |
| POST | `/api/v1/warehouse/return` | 产线退料 |
| GET | `/api/v1/warehouse/return-orders` | 退料单查询 |

#### 6.4.2 核心 DTO

```csharp
public class IssueMaterialRequest
{
    public string WorkOrderId { get; set; }
    public string LotId { get; set; }
    public string StepCode { get; set; }
    public List<IssueItemRequest> Items { get; set; }
    public string IssuedBy { get; set; }
    public string ReceiverId { get; set; }
    public string Remark { get; set; }
}

public class IssueItemRequest
{
    public string MaterialId { get; set; }
    public int RequestedQty { get; set; }
    public string BatchId { get; set; } // 空则系统FIFO推荐
    public bool SkipFifo { get; set; }
    public string SkipFifoReason { get; set; }
}

public class IssueOrderResponse
{
    public string IssueOrderId { get; set; }
    public string WorkOrderId { get; set; }
    public string LotId { get; set; }
    public List<IssueItemResponse> Items { get; set; }
    public string Status { get; set; }
    public DateTime IssuedAt { get; set; }
}

public class ReturnMaterialRequest
{
    public string WorkOrderId { get; set; }
    public string LotId { get; set; }
    public List<ReturnItemRequest> Items { get; set; }
    public string ReturnedBy { get; set; }
    public string Reason { get; set; }
}

public class ReturnItemRequest
{
    public string MaterialId { get; set; }
    public string OriginalBatchId { get; set; }
    public int ReturnQty { get; set; }
    public string TargetLocationId { get; set; }
}

public class KitCheckResponse
{
    public string WorkOrderId { get; set; }
    public bool IsComplete { get; set; }
    public List<KitItemStatus> Items { get; set; }
}

public class KitItemStatus
{
    public string MaterialId { get; set; }
    public string MaterialName { get; set; }
    public int RequiredQty { get; set; }
    public int AvailableQty { get; set; }
    public int IssuedQty { get; set; }
    public int ShortageQty { get; set; }
    public bool IsSufficient { get; set; }
}
```

### 6.5 数据表变更

```sql
-- 文件: docs/sql/migrations/V4.0.4__material_issue_return.sql

-- 发料单表
CREATE TABLE IF NOT EXISTS `warehouse_issue_order` (
    `issue_order_id` VARCHAR(50) NOT NULL COMMENT '格式: WI-YYYYMMDD-NNN',
    `work_order_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) DEFAULT NULL,
    `step_code` VARCHAR(50) DEFAULT NULL,
    `issue_type` VARCHAR(20) NOT NULL DEFAULT 'Normal'
        COMMENT 'Normal/Urgent/Rework',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/Approved/Issued/Completed/Cancelled',
    `issued_by` VARCHAR(50) NOT NULL,
    `receiver_id` VARCHAR(50) DEFAULT NULL,
    `received_at` DATETIME DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `remark` TEXT DEFAULT NULL,
    PRIMARY KEY (`issue_order_id`),
    INDEX `idx_work_order_id` (`work_order_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 发料单明细表
CREATE TABLE IF NOT EXISTS `warehouse_issue_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `issue_order_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) DEFAULT NULL,
    `requested_qty` INT NOT NULL DEFAULT 0,
    `issued_qty` INT NOT NULL DEFAULT 0,
    `unit` VARCHAR(20) DEFAULT 'PCS',
    `batch_id` VARCHAR(50) NOT NULL COMMENT '实际发料批次',
    `location_id` VARCHAR(50) DEFAULT NULL COMMENT '出库库位',
    `fifo_skipped` TINYINT(1) DEFAULT 0,
    `fifo_skip_reason` VARCHAR(255) DEFAULT NULL,
    `fifo_skip_approved_by` VARCHAR(50) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`item_id`),
    INDEX `idx_issue_order_id` (`issue_order_id`),
    INDEX `idx_batch_id` (`batch_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 退料单表
CREATE TABLE IF NOT EXISTS `warehouse_return_order` (
    `return_order_id` VARCHAR(50) NOT NULL COMMENT '格式: WRet-YYYYMMDD-NNN',
    `work_order_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) DEFAULT NULL,
    `return_type` VARCHAR(20) NOT NULL DEFAULT 'Normal'
        COMMENT 'Normal/Excess/Defective',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/Received/Completed/Rejected',
    `returned_by` VARCHAR(50) NOT NULL,
    `received_by` VARCHAR(50) DEFAULT NULL,
    `received_at` DATETIME DEFAULT NULL,
    `reason` VARCHAR(255) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`return_order_id`),
    INDEX `idx_work_order_id` (`work_order_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 退料单明细表
CREATE TABLE IF NOT EXISTS `warehouse_return_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `return_order_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) DEFAULT NULL,
    `return_qty` INT NOT NULL DEFAULT 0,
    `original_batch_id` VARCHAR(50) NOT NULL,
    `target_location_id` VARCHAR(50) DEFAULT NULL,
    `is_expired` TINYINT(1) DEFAULT 0,
    `expiry_check_result` VARCHAR(20) DEFAULT NULL,
    `unit` VARCHAR(20) DEFAULT 'PCS',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`item_id`),
    INDEX `idx_return_order_id` (`return_order_id`),
    INDEX `idx_original_batch_id` (`original_batch_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 6.6 业务规则

| 规则编号 | 规则描述 | 违反后果 |
|---------|---------|---------|
| ISSUE-R01 | 发料前必须通过齐套检查 | 缺少物料则预警 |
| ISSUE-R02 | 发料按 FIFO 推荐批次执行 | 系统自动排序 |
| ISSUE-R03 | 跳过 FIFO 需审批并记录原因 | 无审批禁止发料 |
| ISSUE-R04 | 发料批次必须 available_qty > 0 | 库存不足则拦截 |
| RETURN-R01 | 退料时检查有效期，过期禁止退库 | 系统自动校验 |
| RETURN-R02 | 退料数量 ≤ 发料数量 - 消耗数量 | 超额退料拦截 |
| RETURN-R03 | 退料必须指定目标库位 | 无库位禁止退料 |

### 6.7 依赖关系

| 依赖 | 类型 | 说明 |
|------|------|------|
| 入库/库存模块 | 本阶段 | 库存台账操作 |
| `material_consume` | 已有 | 消耗记录关联 |
| `material_requirement` | 已有 | 物料需求定义 |

---

## 七、模块六：成品入库/出库管理

### 7.1 模块概述

成品入库管理 FQC/OQC 合格后产品的入库流程，支持分批入库和库位分配。成品出库管理出货单创建、OQC 校验、出库执行和库存扣减。

### 7.2 现状分析

| 现状 | 说明 |
|------|------|
| 无成品入库表 | 缺少成品入库单和成品库存表 |
| 无成品出库流程 | 缺少出货单和出库记录 |
| 无成品库位管理 | 成品与原材料库位可共用 `warehouse_location` |
| FQC 合格后无自动入库触发 | 需建立 FQC→入库联动机制 |

### 7.3 功能清单

| 功能编号 | 功能名称 | 功能描述 | 优先级 |
|---------|---------|---------|--------|
| FG-F01 | 成品入库 | FQC 合格后自动/手动创建成品入库单 | P0 |
| FG-F02 | 分批入库 | 支持一个工单分批入库 | P0 |
| FG-F03 | 成品库存查询 | 按产品/批次/库位查询成品库存 | P0 |
| FG-F04 | 出货单创建 | 创建成品出货单 | P0 |
| FG-F05 | OQC 校验拦截 | 出货前校验 OQC 状态 | P0 |
| FG-F06 | 出库执行 | 扣减成品库存，记录出库信息 | P0 |

### 7.4 接口设计

#### 7.4.1 API 接口清单

| 方法 | 路径 | 描述 |
|------|------|------|
| POST | `/api/v1/warehouse/finished-goods/receipt` | 成品入库 |
| GET | `/api/v1/warehouse/finished-goods/receipts` | 成品入库单查询 |
| GET | `/api/v1/warehouse/finished-goods/inventory` | 成品库存查询 |
| POST | `/api/v1/warehouse/finished-goods/ship` | 成品出库 |
| GET | `/api/v1/warehouse/finished-goods/shipments` | 出货单查询 |

#### 7.4.2 核心 DTO

```csharp
public class FinishedGoodsReceiptRequest
{
    public string WorkOrderId { get; set; }
    public string LotId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public string FqcRecordId { get; set; }
    public string LocationId { get; set; }
    public string Grade { get; set; }
    public string ReceivedBy { get; set; }
    public string Remark { get; set; }
}

public class FinishedGoodsInventoryResponse
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string LotId { get; set; }
    public string BatchId { get; set; }
    public string LocationId { get; set; }
    public string Grade { get; set; }
    public int AvailableQty { get; set; }
    public int ShippedQty { get; set; }
    public DateTime ReceiptDate { get; set; }
}

public class ShipFinishedGoodsRequest
{
    public string ShipmentId { get; set; }
    public List<ShipmentItemRequest> Items { get; set; }
    public string ShippedBy { get; set; }
    public string Carrier { get; set; }
    public string Remark { get; set; }
}

public class ShipmentItemRequest
{
    public string ProductId { get; set; }
    public string LotId { get; set; }
    public int Quantity { get; set; }
}
```

### 7.5 数据表变更

```sql
-- 文件: docs/sql/migrations/V4.0.5__finished_goods.sql

-- 成品入库单表
CREATE TABLE IF NOT EXISTS `finished_goods_receipt` (
    `receipt_id` VARCHAR(50) NOT NULL COMMENT '格式: FGR-YYYYMMDD-NNN',
    `work_order_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `product_name` VARCHAR(100) DEFAULT NULL,
    `quantity` INT NOT NULL DEFAULT 0,
    `grade` VARCHAR(20) DEFAULT NULL,
    `fqc_record_id` VARCHAR(50) NOT NULL COMMENT '关联FQC检验记录',
    `location_id` VARCHAR(50) NOT NULL,
    `location_name` VARCHAR(100) DEFAULT NULL,
    `received_by` VARCHAR(50) NOT NULL,
    `received_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Completed'
        COMMENT 'Pending/Completed/Cancelled',
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`receipt_id`),
    INDEX `idx_work_order_id` (`work_order_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_product_id` (`product_id`),
    INDEX `idx_received_at` (`received_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 成品库存表
CREATE TABLE IF NOT EXISTS `finished_goods_inventory` (
    `inventory_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `product_name` VARCHAR(100) DEFAULT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `work_order_id` VARCHAR(50) DEFAULT NULL,
    `grade` VARCHAR(20) DEFAULT NULL,
    `location_id` VARCHAR(50) NOT NULL,
    `location_name` VARCHAR(100) DEFAULT NULL,
    `total_qty` INT NOT NULL DEFAULT 0,
    `available_qty` INT NOT NULL DEFAULT 0,
    `shipped_qty` INT NOT NULL DEFAULT 0,
    `receipt_id` VARCHAR(50) NOT NULL,
    `received_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Available'
        COMMENT 'Available/PartiallyShipped/Shipped/Frozen',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`inventory_id`),
    UNIQUE KEY `uk_lot_location` (`lot_id`, `location_id`),
    INDEX `idx_product_id` (`product_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_grade` (`grade`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 出货单表
CREATE TABLE IF NOT EXISTS `finished_goods_shipment` (
    `shipment_id` VARCHAR(50) NOT NULL COMMENT '格式: FGS-YYYYMMDD-NNN',
    `customer_id` VARCHAR(50) DEFAULT NULL,
    `customer_name` VARCHAR(100) DEFAULT NULL,
    `oqc_record_id` VARCHAR(50) DEFAULT NULL COMMENT '关联OQC检验记录',
    `carrier` VARCHAR(100) DEFAULT NULL,
    `tracking_no` VARCHAR(100) DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/Approved/Shipped/Delivered/Cancelled',
    `shipped_by` VARCHAR(50) DEFAULT NULL,
    `shipped_at` DATETIME DEFAULT NULL,
    `shipped_to` VARCHAR(255) DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`shipment_id`),
    INDEX `idx_customer_id` (`customer_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 出货单明细表
CREATE TABLE IF NOT EXISTS `finished_goods_shipment_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `shipment_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `grade` VARCHAR(20) DEFAULT NULL,
    `quantity` INT NOT NULL DEFAULT 0,
    `inventory_id` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`item_id`),
    INDEX `idx_shipment_id` (`shipment_id`),
    INDEX `idx_lot_id` (`lot_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 7.6 业务规则

| 规则编号 | 规则描述 | 违反后果 |
|---------|---------|---------|
| FG-R01 | FQC 合格后才允许成品入库 | 系统拦截 |
| FG-R02 | 入库数量 ≤ FQC 合格数量 | 超额入库拦截 |
| FG-R03 | 出货前必须完成 OQC 检验 | 禁止创建出货单 |
| FG-R04 | 出货数量 ≤ 成品可用库存 | 超额出货拦截 |
| FG-R05 | 出货单审批后才能执行出库 | 未审批禁止出库 |

### 7.7 依赖关系

| 依赖 | 类型 | 说明 |
|------|------|------|
| FQC/OQC 模块 | 本阶段 | 合格检验记录 |
| 库位表 | 本阶段 | `warehouse_location` |

---

## 八、模块七：生产异常上报与停线机制

### 8.1 模块概述

生产异常管理提供一键异常上报（设备/质量/物料/工艺/安全分类），IPQC/班组长停线权限，异常处理全流程闭环（上报→响应→处理→验证→关闭）。停线指令执行后自动暂停相关工单/批次的过站操作。

### 8.2 现状分析

| 现状 | 说明 |
|------|------|
| `alarm_record` 表已存在 | 有报警记录但非异常管理 |
| `alarm_rule` 表已存在 | 有规则定义但非异常上报 |
| 无异常记录表 | 缺少结构化的异常记录和管理 |
| 无停线机制 | 缺少停线指令和自动拦截逻辑 |
| 无异常处理闭环 | 缺少异常处理/验证/关闭流程 |

### 8.3 功能清单

| 功能编号 | 功能名称 | 功能描述 | 优先级 |
|---------|---------|---------|--------|
| AB-F01 | 异常上报 | 一键上报异常（设备/质量/物料/工艺/安全） | P0 |
| AB-F02 | 异常分类与定级 | 自动/手动分类和严重程度定级 | P0 |
| AB-F03 | 异常查询 | 按状态/类型/日期/产线查询 | P0 |
| AB-F04 | 停线指令 | IPQC/班组长下发停线指令 | P0 |
| AB-F05 | 自动拦截 | 停线后自动暂停过站操作 | P0 |
| AB-F06 | 异常处理 | 记录处理措施和责任人 | P0 |
| AB-F07 | 处理验证 | IPQC 验证处理效果 | P0 |
| AB-F08 | 恢复生产 | 停线恢复，解除过站拦截 | P0 |
| AB-F09 | 异常统计 | 按类型/原因/产线统计分析 | P1 |

### 8.4 接口设计

#### 8.4.1 API 接口清单

| 方法 | 路径 | 描述 |
|------|------|------|
| POST | `/api/v1/abnormal/report` | 异常上报 |
| GET | `/api/v1/abnormal/records` | 异常记录查询 |
| GET | `/api/v1/abnormal/records/{abnormalId}` | 异常详情 |
| POST | `/api/v1/abnormal/line-stop` | 停线指令 |
| POST | `/api/v1/abnormal/line-resume` | 恢复生产 |
| POST | `/api/v1/abnormal/records/{abnormalId}/handle` | 异常处理 |
| POST | `/api/v1/abnormal/records/{abnormalId}/verify` | 处理验证 |
| GET | `/api/v1/abnormal/statistics` | 异常统计 |

#### 8.4.2 核心 DTO

```csharp
public class ReportAbnormalRequest
{
    public string AbnormalType { get; set; } // Equipment/Quality/Material/Process/Safety
    public string Severity { get; set; } // Critical/Major/Minor
    public string WorkOrderId { get; set; }
    public string LotId { get; set; }
    public string StepCode { get; set; }
    public string EquipmentId { get; set; }
    public string Description { get; set; }
    public string ReportedBy { get; set; }
    public string ReportedByName { get; set; }
    public string[] PhotoUrls { get; set; }
    public string Remark { get; set; }
}

public class AbnormalRecordResponse
{
    public string AbnormalId { get; set; }
    public string AbnormalType { get; set; }
    public string Severity { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public bool IsLineStopped { get; set; }
    public DateTime ReportedAt { get; set; }
    public DateTime? HandledAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}

public class LineStopRequest
{
    public string AbnormalId { get; set; }
    public string StopScope { get; set; } // Line/Equipment/WorkOrder/Lot
    public string StopTargetId { get; set; }
    public string StopReason { get; set; }
    public string IssuedBy { get; set; }
}

public class HandleAbnormalRequest
{
    public string AbnormalId { get; set; }
    public string RootCause { get; set; }
    public string CorrectiveAction { get; set; }
    public string PreventiveAction { get; set; }
    public string HandledBy { get; set; }
    public string Remark { get; set; }
}
```

### 8.5 数据表变更

```sql
-- 文件: docs/sql/migrations/V4.0.6__abnormal_line_stop.sql

-- 异常记录表
CREATE TABLE IF NOT EXISTS `abnormal_record` (
    `abnormal_id` VARCHAR(50) NOT NULL COMMENT '格式: ABN-YYYYMMDD-NNN',
    `abnormal_type` VARCHAR(50) NOT NULL
        COMMENT 'Equipment/Quality/Material/Process/Safety',
    `severity` VARCHAR(20) NOT NULL COMMENT 'Critical/Major/Minor',
    `work_order_id` VARCHAR(50) DEFAULT NULL,
    `lot_id` VARCHAR(50) DEFAULT NULL,
    `step_code` VARCHAR(50) DEFAULT NULL,
    `equipment_id` VARCHAR(50) DEFAULT NULL,
    `description` TEXT NOT NULL,
    `reported_by` VARCHAR(50) NOT NULL,
    `reported_by_name` VARCHAR(100) DEFAULT NULL,
    `reported_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `assigned_to` VARCHAR(50) DEFAULT NULL,
    `assigned_at` DATETIME DEFAULT NULL,
    `root_cause` TEXT DEFAULT NULL,
    `corrective_action` TEXT DEFAULT NULL,
    `preventive_action` TEXT DEFAULT NULL,
    `handled_by` VARCHAR(50) DEFAULT NULL,
    `handled_at` DATETIME DEFAULT NULL,
    `verified_by` VARCHAR(50) DEFAULT NULL,
    `verified_at` DATETIME DEFAULT NULL,
    `verify_result` VARCHAR(20) DEFAULT NULL COMMENT 'Pass/Fail',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Open'
        COMMENT 'Open/Assigned/InHandling/Resolved/Verified/Closed/Cancelled',
    `closed_at` DATETIME DEFAULT NULL,
    `closed_by` VARCHAR(50) DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`abnormal_id`),
    INDEX `idx_abnormal_type` (`abnormal_type`),
    INDEX `idx_severity` (`severity`),
    INDEX `idx_work_order_id` (`work_order_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_equipment_id` (`equipment_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_reported_at` (`reported_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 停线记录表
CREATE TABLE IF NOT EXISTS `line_stop_record` (
    `stop_id` VARCHAR(50) NOT NULL COMMENT '格式: LS-YYYYMMDD-NNN',
    `abnormal_id` VARCHAR(50) NOT NULL,
    `stop_scope` VARCHAR(50) NOT NULL COMMENT 'Line/Equipment/WorkOrder/Lot',
    `stop_target_id` VARCHAR(50) NOT NULL,
    `stop_target_name` VARCHAR(100) DEFAULT NULL,
    `stop_reason` TEXT NOT NULL,
    `issued_by` VARCHAR(50) NOT NULL,
    `issued_by_name` VARCHAR(100) DEFAULT NULL,
    `issued_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `resume_by` VARCHAR(50) DEFAULT NULL,
    `resume_at` DATETIME DEFAULT NULL,
    `resume_comment` TEXT DEFAULT NULL,
    `duration_minutes` INT DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Stopped'
        COMMENT 'Stopped/Resumed',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`stop_id`),
    INDEX `idx_abnormal_id` (`abnormal_id`),
    INDEX `idx_stop_scope` (`stop_scope`),
    INDEX `idx_stop_target_id` (`stop_target_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_issued_at` (`issued_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 8.6 业务规则

| 规则编号 | 规则描述 | 违反后果 |
|---------|---------|---------|
| AB-R01 | Critical 级别异常必须立即停线 | 系统自动停线 |
| AB-R02 | 停线后相关工单/批次禁止过站 | 系统自动拦截 TrackIn |
| AB-R03 | 停线必须由 IPQC 或班组长级别以上人员发起 | 权限校验 |
| AB-R04 | 异常处理完成后必须 IPQC 验证 | 未验证不可关闭 |
| AB-R05 | 验证通过后才能恢复生产 | 系统校验 |
| AB-R06 | 异常响应时间：Critical ≤ 15 分钟，Major ≤ 30 分钟 | 超时自动升级 |

### 8.7 依赖关系

| 依赖 | 类型 | 说明 |
|------|------|------|
| `alarm_record` | 已有 | 可与报警关联 |
| TrackService | 已有 | 停线时拦截过站 |
| RBAC 权限 | 已有 | 停线权限控制 |

---

## 九、模块八：设备故障管理与保养

### 9.1 模块概述

设备故障管理涵盖故障上报、维修派工、维修执行、验证关闭全流程。PM（Preventive Maintenance）保养计划管理涵盖计划制定、执行、记录、合规率统计。计算 MTBF/MTTR 指标。

### 9.2 现状分析

| 现状 | 说明 |
|------|------|
| `master_equipment` 表已存在 | 有设备台账基础 |
| EquipmentController 部分实现 | 有列表/状态查询 |
| EquipmentService 存在 | 有基础服务层 |
| PmScheduleView 有界面 | 前端 PM 计划界面存在 |
| 无故障记录表 | 缺少设备故障/维修记录 |
| 无 PM 执行记录 | 缺少保养执行和合规统计 |
| 无 MTBF/MTTR 计算 | 缺少可靠性指标 |

### 9.3 功能清单

| 功能编号 | 功能名称 | 功能描述 | 优先级 |
|---------|---------|---------|--------|
| EQ-F01 | 故障上报 | 设备故障时上报故障信息 | P0 |
| EQ-F02 | 维修派工 | 将故障指派给维修人员 | P0 |
| EQ-F03 | 维修执行 | 记录维修过程和更换备件 | P0 |
| EQ-F04 | 维修验证关闭 | 维修完成后验证并关闭 | P0 |
| EQ-F05 | PM 计划制定 | 制定设备保养计划 | P0 |
| EQ-F06 | PM 执行记录 | 记录保养执行详情 | P0 |
| EQ-F07 | PM 合规率统计 | 统计保养计划执行合规率 | P1 |
| EQ-F08 | MTBF/MTTR 计算 | 自动计算设备可靠性指标 | P1 |

### 9.4 接口设计

#### 9.4.1 API 接口清单

| 方法 | 路径 | 描述 |
|------|------|------|
| POST | `/api/v1/equipment/faults` | 故障上报 |
| GET | `/api/v1/equipment/faults` | 故障记录查询 |
| POST | `/api/v1/equipment/faults/{faultId}/dispatch` | 维修派工 |
| POST | `/api/v1/equipment/faults/{faultId}/complete` | 维修完成 |
| GET | `/api/v1/equipment/pm-schedule` | PM 计划查询 |
| POST | `/api/v1/equipment/pm-schedule` | 创建 PM 计划 |
| POST | `/api/v1/equipment/pm-schedule/{planId}/execute` | 执行保养 |
| GET | `/api/v1/equipment/mtbf-mttr` | MTBF/MTTR 统计 |

#### 9.4.2 核心 DTO

```csharp
public class ReportEquipmentFaultRequest
{
    public string EquipmentId { get; set; }
    public string FaultType { get; set; } // Mechanical/Electrical/Software/Other
    public string Severity { get; set; } // Critical/Major/Minor
    public string Description { get; set; }
    public string ReportedBy { get; set; }
    public string ReportedByName { get; set; }
    public string[] PhotoUrls { get; set; }
    public string Remark { get; set; }
}

public class EquipmentFaultResponse
{
    public string FaultId { get; set; }
    public string EquipmentId { get; set; }
    public string EquipmentName { get; set; }
    public string FaultType { get; set; }
    public string Severity { get; set; }
    public string Status { get; set; } // Reported/Dispatched/InRepair/Completed/Verified
    public DateTime ReportedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int RepairDurationMinutes { get; set; }
}

public class DispatchFaultRequest
{
    public string FaultId { get; set; }
    public string AssigneeId { get; set; }
    public string AssigneeName { get; set; }
    public string Priority { get; set; }
    public string Remark { get; set; }
}

public class CompleteRepairRequest
{
    public string FaultId { get; set; }
    public string RootCause { get; set; }
    public string RepairAction { get; set; }
    public List<string> SparePartsUsed { get; set; }
    public string CompletedBy { get; set; }
    public string Remark { get; set; }
}

public class PmPlanResponse
{
    public string PlanId { get; set; }
    public string EquipmentId { get; set; }
    public string EquipmentName { get; set; }
    public string PmType { get; set; } // Daily/Weekly/Monthly/Quarterly/Annual
    public string Description { get; set; }
    public DateTime PlannedDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public string Status { get; set; } // Planned/InProgress/Completed/Overdue/Skipped
    public string ExecutedBy { get; set; }
    public string Result { get; set; } // Pass/Fail/Partial
}

public class MtbfMttrResponse
{
    public string EquipmentId { get; set; }
    public string EquipmentName { get; set; }
    public decimal MtbfHours { get; set; }
    public decimal MttrHours { get; set; }
    public int TotalFaults { get; set; }
    public decimal AvailabilityPercent { get; set; }
    public string Period { get; set; } // YYYY-MM
}
```

### 9.5 数据表变更

```sql
-- 文件: docs/sql/migrations/V4.0.7__equipment_maintenance.sql

-- 设备故障记录表
CREATE TABLE IF NOT EXISTS `equipment_fault_record` (
    `fault_id` VARCHAR(50) NOT NULL COMMENT '格式: EF-YYYYMMDD-NNN',
    `equipment_id` VARCHAR(50) NOT NULL,
    `equipment_name` VARCHAR(100) DEFAULT NULL,
    `fault_type` VARCHAR(50) NOT NULL COMMENT 'Mechanical/Electrical/Software/Other',
    `severity` VARCHAR(20) NOT NULL COMMENT 'Critical/Major/Minor',
    `description` TEXT NOT NULL,
    `reported_by` VARCHAR(50) NOT NULL,
    `reported_by_name` VARCHAR(100) DEFAULT NULL,
    `reported_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `assignee_id` VARCHAR(50) DEFAULT NULL,
    `assignee_name` VARCHAR(100) DEFAULT NULL,
    `dispatched_at` DATETIME DEFAULT NULL,
    `priority` VARCHAR(20) DEFAULT 'Normal',
    `root_cause` TEXT DEFAULT NULL,
    `repair_action` TEXT DEFAULT NULL,
    `completed_by` VARCHAR(50) DEFAULT NULL,
    `completed_at` DATETIME DEFAULT NULL,
    `verified_by` VARCHAR(50) DEFAULT NULL,
    `verified_at` DATETIME DEFAULT NULL,
    `repair_duration_minutes` INT DEFAULT NULL,
    `downtime_minutes` INT DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Reported'
        COMMENT 'Reported/Dispatched/InRepair/Completed/Verified',
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`fault_id`),
    INDEX `idx_equipment_id` (`equipment_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_severity` (`severity`),
    INDEX `idx_reported_at` (`reported_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 维修备件使用记录表
CREATE TABLE IF NOT EXISTS `equipment_repair_spare_part` (
    `id` BIGINT AUTO_INCREMENT,
    `fault_id` VARCHAR(50) NOT NULL,
    `spare_part_code` VARCHAR(50) NOT NULL,
    `spare_part_name` VARCHAR(100) NOT NULL,
    `quantity` INT NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`),
    INDEX `idx_fault_id` (`fault_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- PM 保养计划表
CREATE TABLE IF NOT EXISTS `equipment_pm_plan` (
    `plan_id` VARCHAR(50) NOT NULL COMMENT '格式: PM-YYYYMMDD-NNN',
    `equipment_id` VARCHAR(50) NOT NULL,
    `equipment_name` VARCHAR(100) DEFAULT NULL,
    `pm_type` VARCHAR(50) NOT NULL COMMENT 'Daily/Weekly/Monthly/Quarterly/Annual',
    `description` TEXT NOT NULL,
    `check_items` JSON DEFAULT NULL COMMENT '检查项目JSON',
    `planned_date` DATE NOT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Planned'
        COMMENT 'Planned/InProgress/Completed/Overdue/Skipped',
    `executed_by` VARCHAR(50) DEFAULT NULL,
    `executed_at` DATETIME DEFAULT NULL,
    `result` VARCHAR(20) DEFAULT NULL COMMENT 'Pass/Fail/Partial',
    `findings` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`plan_id`),
    INDEX `idx_equipment_id` (`equipment_id`),
    INDEX `idx_planned_date` (`planned_date`),
    INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- PM 执行记录表
CREATE TABLE IF NOT EXISTS `equipment_pm_execution` (
    `execution_id` VARCHAR(50) NOT NULL,
    `plan_id` VARCHAR(50) NOT NULL,
    `equipment_id` VARCHAR(50) NOT NULL,
    `item_code` VARCHAR(50) NOT NULL,
    `item_name` VARCHAR(100) NOT NULL,
    `standard` VARCHAR(255) DEFAULT NULL,
    `actual_value` VARCHAR(255) DEFAULT NULL,
    `result` VARCHAR(20) NOT NULL COMMENT 'Pass/Fail/N/A',
    `remark` TEXT DEFAULT NULL,
    `executed_by` VARCHAR(50) NOT NULL,
    `executed_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`execution_id`),
    INDEX `idx_plan_id` (`plan_id`),
    INDEX `idx_equipment_id` (`equipment_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 扩展 master_equipment
ALTER TABLE `master_equipment`
    ADD COLUMN IF NOT EXISTS `mtbf_hours` DECIMAL(10,2) DEFAULT NULL
        COMMENT '平均故障间隔时间(小时)',
    ADD COLUMN IF NOT EXISTS `mttr_hours` DECIMAL(10,2) DEFAULT NULL
        COMMENT '平均修复时间(小时)',
    ADD COLUMN IF NOT EXISTS `last_fault_date` DATETIME DEFAULT NULL
        COMMENT '最后一次故障时间',
    ADD COLUMN IF NOT EXISTS `total_downtime_minutes` INT DEFAULT 0
        COMMENT '累计停机时间(分钟)',
    ADD COLUMN IF NOT EXISTS `fault_count` INT DEFAULT 0
        COMMENT '累计故障次数';
```

### 9.6 业务规则

| 规则编号 | 规则描述 | 违反后果 |
|---------|---------|---------|
| EQ-R01 | 故障上报后设备状态自动变更为 Fault | 系统自动执行 |
| EQ-R02 | 维修完成验证通过后设备状态恢复 Available | 系统自动执行 |
| EQ-R03 | PM 计划到期前 1 天自动提醒 | 发送通知 |
| EQ-R04 | PM 计划超期未完成自动标为 Overdue | 系统自动更新 |
| EQ-R05 | MTBF = 总运行时间 / 故障次数 | 自动计算 |
| EQ-R06 | MTTR = 总维修时间 / 故障次数 | 自动计算 |
| EQ-R07 | 设备在维修中禁止 TrackIn | 系统拦截 |

### 9.7 依赖关系

| 依赖 | 类型 | 说明 |
|------|------|------|
| `master_equipment` | 已有 | 设备台账 |
| TrackService | 已有 | 维修中拦截过站 |
| 备件管理 | 已有 | SparePartView |

---

## 十、模块九：首件检验流程

### 10.1 模块概述

首件检验在换线/开班/工艺变更/设备维修后自动触发，需要技术员和 IPQC 双人确认。检验项目包括尺寸、外观、功能等，首件合格后方可批量生产，不合格则锁定工单。

### 10.2 现状分析

| 现状 | 说明 |
|------|------|
| FirstArticleInspectionView UI 存在 | 前端界面已存在 |
| `quality_inspection` 表已存在 | 可复用但缺少首件专用字段 |
| 无首件触发机制 | 缺少换线/开班自动触发逻辑 |
| 无双人签名机制 | 缺少技术员+IPQC 会签 |
| 无工单锁定联动 | 首件不合格无工单锁定 |

### 10.3 功能清单

| 功能编号 | 功能名称 | 功能描述 | 优先级 |
|---------|---------|---------|--------|
| FA-F01 | 待首件检验列表 | 查询需要首件检验的工单/批次 | P0 |
| FA-F02 | 首件触发 | 换线/开班/工艺变更自动触发 | P0 |
| FA-F03 | 检验录入 | 技术员录入首件检验数据 | P0 |
| FA-F04 | 双人确认 | 技术员+IPQC 双重签名确认 | P0 |
| FA-F05 | 首件合格→放行 | 合格后解除工单锁定 | P0 |
| FA-F06 | 首件不合格→锁定 | 不合格锁定工单并触发异常 | P0 |
| FA-F07 | 焊线拉力测试 | 键合工序首件拉力测试录入 | P1 |

### 10.4 接口设计

#### 10.4.1 API 接口清单

| 方法 | 路径 | 描述 |
|------|------|------|
| GET | `/api/v1/first-article/pending` | 待首件检验列表 |
| POST | `/api/v1/first-article/trigger` | 触发首件检验 |
| GET | `/api/v1/first-article/{faId}` | 获取首件检验详情 |
| POST | `/api/v1/first-article/{faId}/inspect` | 检验录入 |
| POST | `/api/v1/first-article/{faId}/confirm` | 双人确认 |
| POST | `/api/v1/first-article/{faId}/reject` | 首件不合格处理 |
| POST | `/api/v1/first-article/{faId}/bond-test` | 焊线拉力测试 |

#### 10.4.2 核心 DTO

```csharp
public class FirstArticleResponse
{
    public string FaId { get; set; }
    public string WorkOrderId { get; set; }
    public string LotId { get; set; }
    public string ProductId { get; set; }
    public string StepCode { get; set; }
    public string EquipmentId { get; set; }
    public string TriggerReason { get; set; } // LineChange/ShiftStart/ProcessChange/Maintenance
    public string Status { get; set; } // Pending/Inspecting/TechConfirmed/IPQCConfirmed/Approved/Rejected
    public DateTime CreatedAt { get; set; }
    public string TechnicianId { get; set; }
    public string IpqcId { get; set; }
}

public class ExecuteFirstArticleRequest
{
    public string FaId { get; set; }
    public List<InspectionItemInput> Items { get; set; }
    public string TechnicianId { get; set; }
    public string TechnicianName { get; set; }
    public string Remark { get; set; }
}

public class ConfirmFirstArticleRequest
{
    public string FaId { get; set; }
    public string ConfirmerId { get; set; }
    public string ConfirmerRole { get; set; } // Technician/IPQC
    public string Confirmation { get; set; } // Approve/Reject
    public string Comment { get; set; }
}
```

### 10.5 数据表变更

```sql
-- 文件: docs/sql/migrations/V4.0.8__first_article_inspection.sql

-- 首件检验表
CREATE TABLE IF NOT EXISTS `first_article_inspection` (
    `fa_id` VARCHAR(50) NOT NULL COMMENT '格式: FA-YYYYMMDD-NNN',
    `work_order_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `product_name` VARCHAR(100) DEFAULT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `step_name` VARCHAR(100) DEFAULT NULL,
    `equipment_id` VARCHAR(50) DEFAULT NULL,
    `trigger_reason` VARCHAR(50) NOT NULL
        COMMENT 'LineChange/ShiftStart/ProcessChange/Maintenance/RecipeChange',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/Inspecting/TechConfirmed/IPQCConfirmed/Approved/Rejected',
    `technician_id` VARCHAR(50) DEFAULT NULL,
    `technician_name` VARCHAR(100) DEFAULT NULL,
    `technician_confirmed_at` DATETIME DEFAULT NULL,
    `ipqc_id` VARCHAR(50) DEFAULT NULL,
    `ipqc_name` VARCHAR(100) DEFAULT NULL,
    `ipqc_confirmed_at` DATETIME DEFAULT NULL,
    `result` VARCHAR(20) DEFAULT NULL COMMENT 'Pass/Fail',
    `rejection_reason` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`fa_id`),
    INDEX `idx_work_order_id` (`work_order_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 首件检验项目明细表
CREATE TABLE IF NOT EXISTS `first_article_inspection_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `fa_id` VARCHAR(50) NOT NULL,
    `item_code` VARCHAR(50) NOT NULL,
    `item_name` VARCHAR(100) NOT NULL,
    `item_type` VARCHAR(50) DEFAULT NULL,
    `standard_value` VARCHAR(50) DEFAULT NULL,
    `lower_limit` VARCHAR(50) DEFAULT NULL,
    `upper_limit` VARCHAR(50) DEFAULT NULL,
    `actual_value` VARCHAR(50) DEFAULT NULL,
    `unit` VARCHAR(20) DEFAULT NULL,
    `result` VARCHAR(20) NOT NULL COMMENT 'Pass/Fail/N/A',
    `measured_by` VARCHAR(50) DEFAULT NULL,
    `measured_at` DATETIME DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    PRIMARY KEY (`item_id`),
    INDEX `idx_fa_id` (`fa_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 首件签名记录表
CREATE TABLE IF NOT EXISTS `first_article_signature` (
    `signature_id` VARCHAR(50) NOT NULL,
    `fa_id` VARCHAR(50) NOT NULL,
    `signer_role` VARCHAR(50) NOT NULL COMMENT 'Technician/IPQC',
    `signer_id` VARCHAR(50) NOT NULL,
    `signer_name` VARCHAR(100) NOT NULL,
    `confirmation` VARCHAR(20) NOT NULL COMMENT 'Approve/Reject',
    `comment` TEXT DEFAULT NULL,
    `signed_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`signature_id`),
    UNIQUE KEY `uk_fa_role` (`fa_id`, `signer_role`),
    INDEX `idx_fa_id` (`fa_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 焊线拉力测试记录表
CREATE TABLE IF NOT EXISTS `bond_pull_test_record` (
    `test_id` VARCHAR(50) NOT NULL,
    `fa_id` VARCHAR(50) DEFAULT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `work_order_id` VARCHAR(50) DEFAULT NULL,
    `sample_no` INT NOT NULL,
    `pull_force_grams` DECIMAL(8,2) NOT NULL,
    `lower_limit_grams` DECIMAL(8,2) DEFAULT NULL,
    `upper_limit_grams` DECIMAL(8,2) DEFAULT NULL,
    `result` VARCHAR(20) NOT NULL COMMENT 'Pass/Fail',
    `failure_mode` VARCHAR(50) DEFAULT NULL,
    `tested_by` VARCHAR(50) NOT NULL,
    `tested_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `remark` TEXT DEFAULT NULL,
    PRIMARY KEY (`test_id`),
    INDEX `idx_fa_id` (`fa_id`),
    INDEX `idx_lot_id` (`lot_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 10.6 业务规则

| 规则编号 | 规则描述 | 违反后果 |
|---------|---------|---------|
| FA-R01 | 换线/开班/工艺变更/设备维修后必须执行首件 | 禁止批量生产 |
| FA-R02 | 首件必须技术员+IPQC 双人确认 | 缺一不可 |
| FA-R03 | 首件检验所有项目必须录入 | 禁止提交 |
| FA-R04 | 首件合格前工单锁定禁止过站 | 系统自动拦截 |
| FA-R05 | 首件不合格触发异常上报和工单锁定 | 系统自动执行 |
| FA-R06 | 焊线拉力测试值必须在规格范围内 | 超限标记 Fail |

### 10.7 依赖关系

| 依赖 | 类型 | 说明 |
|------|------|------|
| `prod_work_order` / `prod_lot` | 已有 | 工单/批次信息 |
| TrackService | 已有 | 首件锁定过站拦截 |
| 异常上报模块 | 本阶段 | 首件不合格触发异常 |

---

## 十一、模块十：紧急质量通知与召回

### 11.1 模块概述

紧急质量通知（Quality Alert）在发现重大质量风险时发布，自动冻结在制品/库存品中的受影响批次，利用追溯链查询所有受影响的产品，生成召回清单并通知相关方。

### 11.2 现状分析

| 现状 | 说明 |
|------|------|
| 无 Quality Alert 机制 | 缺少质量警报发布和管理 |
| `prod_lot` 有 hold 机制 | 可复用实现批次冻结 |
| GenealogyService 已实现 | 追溯基础可复用 |
| 无召回管理 | 缺少召回清单生成和跟踪 |
| 无通知分发机制 | 缺少警报通知相关人员 |

### 11.3 功能清单

| 功能编号 | 功能名称 | 功能描述 | 优先级 |
|---------|---------|---------|--------|
| QA-F01 | 质量警报发布 | 发布质量警报（含原因、影响范围） | P0 |
| QA-F02 | 自动冻结 | 冻结相关在制品/库存批次 | P0 |
| QA-F03 | 受影响批次追溯 | 利用追溯链查找受影响批次 | P0 |
| QA-F04 | 召回清单生成 | 生成召回清单（含客户、数量、位置） | P0 |
| QA-F05 | 通知分发 | 通知相关部门和人员 | P0 |
| QA-F06 | 解除冻结 | 调查完成后解除批次冻结 | P0 |
| QA-F07 | 召回跟踪 | 跟踪召回执行进度 | P1 |

### 11.4 接口设计

#### 11.4.1 API 接口清单

| 方法 | 路径 | 描述 |
|------|------|------|
| POST | `/api/v1/quality-alert` | 发布质量警报 |
| GET | `/api/v1/quality-alerts` | 质量警报查询 |
| GET | `/api/v1/quality-alert/{alertId}` | 警报详情 |
| POST | `/api/v1/quality-alert/{alertId}/freeze` | 冻结相关批次 |
| GET | `/api/v1/quality-alert/{alertId}/affected-lots` | 查询受影响批次 |
| POST | `/api/v1/quality-alert/{alertId}/recall-notice` | 生成召回通知 |
| POST | `/api/v1/quality-alert/{alertId}/unfreeze` | 解除冻结 |
| POST | `/api/v1/quality-alert/{alertId}/close` | 关闭警报 |

#### 11.4.2 核心 DTO

```csharp
public class CreateQualityAlertRequest
{
    public string AlertType { get; set; } // MaterialDefect/ProcessDefect/EquipmentIssue/CustomerComplaint
    public string Severity { get; set; } // Critical/High/Medium
    public string Title { get; set; }
    public string Description { get; set; }
    public string RootCause { get; set; }
    public string SourceMaterialId { get; set; }
    public string SourceBatchId { get; set; }
    public string SourceWorkOrderId { get; set; }
    public string SourceLotId { get; set; }
    public DateTime? OccurrenceDate { get; set; }
    public string IssuedBy { get; set; }
    public string IssuedByName { get; set; }
    public string[] NotifyDepartments { get; set; }
    public string Remark { get; set; }
}

public class QualityAlertResponse
{
    public string AlertId { get; set; }
    public string AlertType { get; set; }
    public string Severity { get; set; }
    public string Title { get; set; }
    public string Status { get; set; } // Active/Investigating/Resolved/Closed
    public int FrozenLotsCount { get; set; }
    public int AffectedLotsCount { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}

public class AffectedLotInfo
{
    public string LotId { get; set; }
    public string WorkOrderId { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string CurrentStatus { get; set; }
    public string CurrentStep { get; set; }
    public int AffectedQty { get; set; }
    public string Location { get; set; }
    public string TraceRelation { get; set; }
}

public class RecallNoticeResponse
{
    public string RecallId { get; set; }
    public string AlertId { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<RecallItem> Items { get; set; }
    public int TotalAffectedQty { get; set; }
    public int TotalRecalledQty { get; set; }
}
```

### 11.5 数据表变更

```sql
-- 文件: docs/sql/migrations/V4.0.9__quality_alert_recall.sql

-- 质量警报表
CREATE TABLE IF NOT EXISTS `quality_alert` (
    `alert_id` VARCHAR(50) NOT NULL COMMENT '格式: QA-YYYYMMDD-NNN',
    `alert_type` VARCHAR(50) NOT NULL
        COMMENT 'MaterialDefect/ProcessDefect/EquipmentIssue/CustomerComplaint',
    `severity` VARCHAR(20) NOT NULL COMMENT 'Critical/High/Medium',
    `title` VARCHAR(200) NOT NULL,
    `description` TEXT NOT NULL,
    `root_cause` TEXT DEFAULT NULL,
    `source_material_id` VARCHAR(50) DEFAULT NULL,
    `source_batch_id` VARCHAR(50) DEFAULT NULL,
    `source_work_order_id` VARCHAR(50) DEFAULT NULL,
    `source_lot_id` VARCHAR(50) DEFAULT NULL,
    `occurrence_date` DATETIME DEFAULT NULL,
    `issued_by` VARCHAR(50) NOT NULL,
    `issued_by_name` VARCHAR(100) DEFAULT NULL,
    `issued_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Active'
        COMMENT 'Active/Investigating/Resolved/Closed',
    `investigation_result` TEXT DEFAULT NULL,
    `resolved_by` VARCHAR(50) DEFAULT NULL,
    `resolved_at` DATETIME DEFAULT NULL,
    `closed_by` VARCHAR(50) DEFAULT NULL,
    `closed_at` DATETIME DEFAULT NULL,
    `frozen_lots_count` INT DEFAULT 0,
    `affected_lots_count` INT DEFAULT 0,
    `notify_departments` JSON DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`alert_id`),
    INDEX `idx_alert_type` (`alert_type`),
    INDEX `idx_severity` (`severity`),
    INDEX `idx_status` (`status`),
    INDEX `idx_issued_at` (`issued_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 质量警报影响批次关联表
CREATE TABLE IF NOT EXISTS `quality_alert_affected_lot` (
    `id` BIGINT AUTO_INCREMENT,
    `alert_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `work_order_id` VARCHAR(50) DEFAULT NULL,
    `product_id` VARCHAR(50) DEFAULT NULL,
    `affected_qty` INT DEFAULT 0,
    `trace_relation` VARCHAR(100) DEFAULT NULL
        COMMENT 'SameBatch/SameMaterial/SameEquipment/SameProcess',
    `is_frozen` TINYINT(1) NOT NULL DEFAULT 0,
    `frozen_at` DATETIME DEFAULT NULL,
    `unfrozen_at` DATETIME DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_alert_lot` (`alert_id`, `lot_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_is_frozen` (`is_frozen`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 召回通知表
CREATE TABLE IF NOT EXISTS `recall_notice` (
    `recall_id` VARCHAR(50) NOT NULL COMMENT '格式: RC-YYYYMMDD-NNN',
    `alert_id` VARCHAR(50) NOT NULL,
    `recall_scope` VARCHAR(50) NOT NULL
        COMMENT 'FullLot/Partial/SpecificCustomer',
    `total_affected_qty` INT NOT NULL DEFAULT 0,
    `total_recalled_qty` INT NOT NULL DEFAULT 0,
    `recall_reason` TEXT NOT NULL,
    `generated_by` VARCHAR(50) NOT NULL,
    `generated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Generated'
        COMMENT 'Generated/Notified/InProgress/Completed',
    `completed_at` DATETIME DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    PRIMARY KEY (`recall_id`),
    INDEX `idx_alert_id` (`alert_id`),
    INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 召回明细表
CREATE TABLE IF NOT EXISTS `recall_notice_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `recall_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `customer_id` VARCHAR(50) DEFAULT NULL,
    `customer_name` VARCHAR(100) DEFAULT NULL,
    `affected_qty` INT NOT NULL DEFAULT 0,
    `recalled_qty` INT NOT NULL DEFAULT 0,
    `current_location` VARCHAR(100) DEFAULT NULL
        COMMENT 'WIP/Warehouse/InTransit/CustomerSite',
    `recall_status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/Notified/InTransit/Returned/Disposed',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`item_id`),
    INDEX `idx_recall_id` (`recall_id`),
    INDEX `idx_lot_id` (`lot_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 11.6 业务规则

| 规则编号 | 规则描述 | 违反后果 |
|---------|---------|---------|
| QA-R01 | 质量警报发布后 30 秒内自动冻结受影响批次 | 系统自动执行 |
| QA-R02 | 冻结批次禁止任何出入库和过站操作 | 系统拦截 |
| QA-R03 | 利用追溯链向前/向后追溯所有受影响批次 | 系统自动查询 |
| QA-R04 | 召回清单必须包含客户、数量、当前位置 | 完整性校验 |
| QA-R05 | 只有 QA Manager 及以上级别可发布警报 | 权限校验 |
| QA-R06 | 解除冻结必须有调查结论和审批 | 禁止随意解冻 |

### 11.7 依赖关系

| 依赖 | 类型 | 说明 |
|------|------|------|
| GenealogyService | 已有 | 追溯链查询 |
| `prod_lot` | 已有 | 批次状态管理 |
| 仓储模块 | 本阶段 | 库存批次冻结 |
| 异常上报模块 | 本阶段 | 警报→异常联动 |

---

## 十二、实施顺序与依赖关系

### 12.1 甘特图式实施顺序

```
Week 1-2:  [IQC 来料检验管理 ████████████]
Week 2-3:                    [不合格品/MRB ████████████]
Week 2-3:                    [首件检验 ████]
Week 3-4:                    [原材料入库/FIFO/有效期 ████████████]
Week 4-5:                                               [产线发料/退料 ████████████]
Week 3-4:                    [FQC/OQC 终检 ████████]
Week 4-5:                                               [成品入库/出库 ████████]
Week 5-6:                                                                          [异常上报/停线 ████████████]
Week 5-6:                                                                          [设备故障/保养 ████████████]
Week 6-7:                                                                                                     [质量警报/召回 ████████████]
Week 7-8:                                                                                                                  [集成测试 ████████]
```

### 12.2 详细依赖关系图

```
                    ┌──────────┐
                    │  主数据   │ (已有)
                    └────┬─────┘
                         │
          ┌──────────────┼──────────────┐
          ▼              ▼              ▼
    ┌──────────┐  ┌──────────┐  ┌──────────┐
    │ IQC检验  │  │ 首件检验  │  │ 设备台账  │
    └────┬─────┘  └──────────┘  └────┬─────┘
         │                           │
    ┌────┴─────┐              ┌─────┴──────┐
    ▼          ▼              ▼            ▼
┌──────┐  ┌───────┐    ┌──────────┐ ┌──────────┐
│ 入库  │  │ 不合格 │    │ 设备故障  │ │ 异常上报  │
│ FIFO │  │ 品MRB │    │ PM保养   │ │ 停线     │
└──┬───┘  └───┬───┘    └──────────┘ └────┬─────┘
   │          │                           │
   └────┬─────┘                           │
        ▼                                │
  ┌──────────┐                           │
  │ 发料/退料 │◄──────────────────────────┘
  └────┬─────┘
       │
       ▼
  ┌──────────┐    ┌──────────┐    ┌──────────┐
  │ FQC/OQC  │───►│ 成品入库  │───►│ 质量警报  │
  └──────────┘    └──────────┘    │   召回    │
                                 └──────────┘
```

### 12.3 实施阶段划分

| 阶段 | 周次 | 包含模块 | 里程碑 |
|------|------|---------|--------|
| Sprint 1 | W1-W2 | IQC + 首件检验 + 不合格品 | 来料检验全流程可用 |
| Sprint 2 | W3-W4 | 原材料入库/FIFO/有效期 + FQC/OQC | 仓储入库+终检可用 |
| Sprint 3 | W4-W5 | 发料/退料 + 成品入库/出库 | 仓储全流程可用 |
| Sprint 4 | W5-W6 | 异常上报/停线 + 设备故障/保养 | 异常管控可用 |
| Sprint 5 | W6-W7 | 质量警报/召回 | 召回机制可用 |
| Sprint 6 | W7-W8 | 集成测试 + Bug修复 + 文档 | 全链路集成测试通过 |

---

## 十三、验收标准

### 13.1 单元测试标准

| 测试维度 | 覆盖率目标 | 工具 |
|---------|-----------|------|
| Service 层业务逻辑 | ≥70% | xUnit + Moq |
| 核心校验规则 | 100% | xUnit |
| 状态机转换 | 100% | xUnit |

### 13.2 集成测试标准

| 测试场景 | 验收标准 |
|---------|---------|
| IQC→合格入库→领料→生产→FQC→成品入库→OQC→出货 | 全链路端到端测试通过，各状态转换正确 |
| IQC 不合格→隔离→MRB→处置执行 | MRB 三方会签完整，处置记录可追溯 |
| 异常上报→停线→处理→验证→恢复 | 停线后 TrackIn 被拦截，恢复后恢复正常 |
| 设备故障→维修→验证→恢复 | 维修中设备禁止过站，MTBF/MTTR 计算正确 |
| 首件检验→双人确认→放行 | 未确认前工单过站被拦截 |
| 质量警报→冻结→追溯→召回 | 30 秒内完成冻结，追溯链完整 |
| FIFO 发料 | 按 received_at 升序推荐，跳过需审批 |
| 有效期预警 | 提前 N 天预警，到期自动锁定 |
| 库存同步 | 发料/退料/入库后库存台账实时更新，账实一致 |

### 13.3 性能标准

| 指标 | 目标值 |
|------|--------|
| API 响应时间（查询类） | < 500ms |
| API 响应时间（写入类） | < 1s |
| 追溯查询响应时间 | < 3s |
| FIFO 推荐查询 | < 500ms |
| 并发用户数 | ≥ 50 |
| 数据库连接池 | ≤ 50 连接 |

---

## 十四、数据库迁移计划

### 14.1 迁移脚本清单

| 编号 | 文件名 | 内容 | 依赖 |
|------|--------|------|------|
| V4.0.0 | `V4.0.0__iqc_incoming_material.sql` | IQC 表 + 检验标准 | 无 |
| V4.0.1 | `V4.0.1__fqc_oqc_inspection.sql` | FQC/OQC 表 + MSL检查 | V4.0.0 |
| V4.0.2 | `V4.0.2__nonconforming_mrb.sql` | NCR + MRB + 处置表 | V4.0.0, V4.0.1 |
| V4.0.3 | `V4.0.3__warehouse_inbound.sql` | 入库单 + 库存台账 + 库位 + 有效期 | V4.0.0 |
| V4.0.4 | `V4.0.4__material_issue_return.sql` | 发料单 + 退料单 | V4.0.3 |
| V4.0.5 | `V4.0.5__finished_goods.sql` | 成品入库 + 成品库存 + 出货单 | V4.0.1 |
| V4.0.6 | `V4.0.6__abnormal_line_stop.sql` | 异常记录 + 停线记录 | 无 |
| V4.0.7 | `V4.0.7__equipment_maintenance.sql` | 故障记录 + PM计划 + PM执行 | 无 |
| V4.0.8 | `V4.0.8__first_article_inspection.sql` | 首件检验 + 拉力测试 | 无 |
| V4.0.9 | `V4.0.9__quality_alert_recall.sql` | 质量警报 + 召回 | 无 |
| V4.0.10 | `V4.0.10__seed_data.sql` | 基础数据种子（库位、检验标准等） | V4.0.0~V4.0.9 |

### 14.2 迁移执行步骤

```powershell
# 1. 备份当前数据库
.\Backup-Database.ps1

# 2. 按顺序执行迁移脚本
mysql -u root -p mes_prod < docs/sql/migrations/V4.0.0__iqc_incoming_material.sql
mysql -u root -p mes_prod < docs/sql/migrations/V4.0.1__fqc_oqc_inspection.sql
mysql -u root -p mes_prod < docs/sql/migrations/V4.0.2__nonconforming_mrb.sql
mysql -u root -p mes_prod < docs/sql/migrations/V4.0.3__warehouse_inbound.sql
mysql -u root -p mes_prod < docs/sql/migrations/V4.0.4__material_issue_return.sql
mysql -u root -p mes_prod < docs/sql/migrations/V4.0.5__finished_goods.sql
mysql -u root -p mes_prod < docs/sql/migrations/V4.0.6__abnormal_line_stop.sql
mysql -u root -p mes_prod < docs/sql/migrations/V4.0.7__equipment_maintenance.sql
mysql -u root -p mes_prod < docs/sql/migrations/V4.0.8__first_article_inspection.sql
mysql -u root -p mes_prod < docs/sql/migrations/V4.0.9__quality_alert_recall.sql
mysql -u root -p mes_prod < docs/sql/migrations/V4.0.10__seed_data.sql

# 3. 验证迁移结果
mysql -u root -p mes_prod -e "SHOW TABLES LIKE '%iqc%';"
mysql -u root -p mes_prod -e "SHOW TABLES LIKE '%warehouse%';"
mysql -u root -p mes_prod -e "SHOW TABLES LIKE '%fqc%';"
mysql -u root -p mes_prod -e "SHOW TABLES LIKE '%nonconforming%';"
mysql -u root -p mes_prod -e "SHOW TABLES LIKE '%abnormal%';"
mysql -u root -p mes_prod -e "SHOW TABLES LIKE '%first_article%';"
mysql -u root -p mes_prod -e "SHOW TABLES LIKE '%quality_alert%';"
```

### 14.3 回滚策略

每个迁移脚本附带对应的回滚脚本：

| 迁移脚本 | 回滚脚本 |
|---------|---------|
| V4.0.0 | rollback_V4.0.0.sql |
| V4.0.1 | rollback_V4.0.1.sql |
| ... | ... |
| V4.0.9 | rollback_V4.0.9.sql |

回滚脚本模板：
```sql
-- rollback_V4.0.X.sql
DROP TABLE IF EXISTS `table_name_created_in_V4.0.X`;
-- 如果修改了已有表结构，使用 ALTER TABLE DROP COLUMN 还原
```

### 14.4 数据种子要求

| 数据类型 | 内容 | 数量 |
|---------|------|------|
| 库位数据 | 原材料库、成品库、隔离区、线边仓 | 20+ |
| IQC 检验标准 | 常用物料的检验项目定义 | 50+ |
| 缺陷代码 | 质量缺陷分类和代码 | 30+ |
| 异常类型 | 异常分类和严重程度定义 | 10+ |
| PM 保养模板 | 各设备类型的标准保养项 | 20+ |
| 首件检验模板 | 各工序的首件检验项目 | 15+ |

---

## 十五、关键风险与缓解措施

### 15.1 技术风险

| 风险 | 影响 | 概率 | 缓解措施 |
|------|------|------|---------|
| EF Core 模型配置与现有 MesDbContext 冲突 | 高 | 中 | 每个模块独立配置类，使用 IEntityTypeConfiguration |
| 数据库迁移导致数据不一致 | 高 | 低 | 所有迁移脚本支持幂等执行，先备份后迁移 |
| 复杂查询性能问题 | 中 | 中 | 使用 Dapper 优化复杂查询，预先建立索引 |

### 15.2 业务风险

| 风险 | 影响 | 概率 | 缓解措施 |
|------|------|------|---------|
| FIFO 强制发料影响生产灵活性 | 中 | 高 | 提供审批跳过机制，记录原因 |
| 首件检验流程增加生产准备时间 | 中 | 中 | 优化检验项目，支持并行检验 |
| MRB 评审耗时过长影响生产进度 | 中 | 中 | 设置 24 小时超时自动升级 |

---

## 十六、与现有代码资产的关系

| 现有资产 | 状态 | 本计划利用方式 |
|---------|------|--------------|
| `prod_lot` / `prod_work_order` | 已有 | 批次/工单状态关联 |
| `quality_inspection` | 已有 | IQC/FQC/OQC 可复用基础字段 |
| `prod_hold_record` | 已有 | 不合格品隔离关联 |
| `prod_rework_record` | 已有 | MRB 返工处置关联 |
| `prod_scrap_record` | 已有 | MRB 报废处置关联 |
| `prod_signature` | 已有 | 电子签名复用 |
| `alarm_record` | 已有 | 异常/警报可关联 |
| GenealogyService | 已有 | 质量警报追溯链查询 |
| EquipmentController | 已有 | 设备故障管理扩展 |
| Complaint8DService | 已有 | MRB 审批流模式参考 |
| TrackService | 已有 | 停线/首件拦截过站 |

---

## 十七、预估工作量

### 17.1 总体预估

| 维度 | 预估 |
|------|------|
| 总周期 | **8 周** |
| 新增 API Controller | 9 个 |
| 新增 Service 接口+实现 | 9 对（18个类） |
| 新增数据表 | 28 张 |
| 修改数据表 | 3 张 |
| 数据库迁移脚本 | 11 个 |
| 新增 DTO 类 | ~60 个 |
| 预估代码行数 | ~15,000-20,000 行 |

### 17.2 按模块预估

| 模块 | 开发(天) | 测试(天) | 文档(天) | 合计(天) |
|------|---------|---------|---------|---------|
| IQC 来料检验 | 6 | 3 | 1 | 10 |
| FQC/OQC 终检 | 4 | 2 | 1 | 7 |
| 不合格品/MRB | 6 | 3 | 1 | 10 |
| 原材料入库/FIFO/有效期 | 7 | 3 | 1 | 11 |
| 产线发料/退料 | 5 | 2 | 1 | 8 |
| 成品入库/出库 | 4 | 2 | 1 | 7 |
| 异常上报/停线 | 5 | 2 | 1 | 8 |
| 设备故障/保养 | 5 | 2 | 1 | 8 |
| 首件检验 | 4 | 2 | 1 | 7 |
| 质量警报/召回 | 5 | 2 | 1 | 8 |
| 集成测试 | - | 5 | 1 | 6 |
| **合计** | **51** | **28** | **10** | **89 人天 ≈ 18 周(单人)** |

> 注：以上为单人开发预估，实际周期取决于团队规模和并行度。建议 2-3 名后端开发人员并行，可将周期压缩至 8 周。

---

> **文档版本：** V1.0  
> **创建日期：** 2026-06-06  
> **维护说明：** 本计划为详细设计文档，实施前需经过技术评审。每个模块实施完成后更新实际进度。  
> **下次更新：** Sprint 1 完成后重新评估后续模块预估

---
