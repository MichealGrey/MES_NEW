# MES系统优化改善计划 - 阶段三：工序管控深化

> **版本**: v1.0  
> **日期**: 2026-06-06  
> **状态**: 待评审  
> **前提**: 基于现有ProcessExecution/SPC/Test基础进行深化改造

---

## 一、阶段三概述

### 1.1 目标

在现有工序执行(ProcessExecution)、SPC监控、测试管理(TestManagement)基础之上，实现**工序参数管控、Bin管理、金线铜线切换、模具寿命管理、操作员资质管理、首件检验深化**的全面闭环管控，确保每个工序从进站到出站都有完整的参数校验、物料追踪、资质验证和质量管控。

### 1.2 范围

| 模块 | 核心内容 | 优先级 |
|------|---------|--------|
| 工序参数管控 | 参数模板、上下限校验、超限自动拦截 | P0 |
| Bin管理 | Bin码主数据、Bin结果判定、Bin-Yield联动 | P0 |
| 金线/铜线切换 | 线材类型管理、切换记录、兼容性校验 | P1 |
| 模具/刀片/劈刀寿命 | 工具定义、使用计数、寿命预警、到期锁定 | P1 |
| 操作员资质管理 | 资质定义、工序绑定、无资质拦截、到期预警 | P1 |
| 首件检验深化 | 后端API补齐、模板管理、首件放行联动 | P2 |

### 1.3 现有基础评估

#### 1.3.1 已有后端服务

| 服务 | 文件路径 | 状态 |
|------|---------|------|
| ProcessExecutionService | `src/Server/MES.Services.Production/ProcessExecutionService.cs` | ✅ 有基础CRUD，缺参数管控 |
| TestManagementService | `src/Server/MES.Services.Production/TestManagementService.cs` | ✅ CP/FT测试有基础 |
| SpcMeasurementService | `src/Server/MES.Services.Quality/SpcMeasurementService.cs` | ✅ SPC数据采集有基础 |
| IProcessExecutionService | `src/Server/MES.Services.Production/IProcessExecutionService.cs` | ✅ 接口定义完整 |

#### 1.3.2 已有API端点

```
POST /api/ProcessExecution/start-step
POST /api/ProcessExecution/complete-step
POST /api/ProcessExecution/parameters
GET  /api/ProcessExecution/{lotId}/{stepCode}/status
GET  /api/ProcessExecution/{lotId}/current-step
GET  /api/TestManagement/tests/{lotId}
GET  /api/TestManagement/test-results/{testId}
GET  /api/TestManagement/bin-summary/{lotId}
GET  /api/TestManagement/lot-yield/{lotId}
GET  /api/Spc/data
GET  /api/Spc/rule-config
```

#### 1.3.3 已有数据库实体（MesDbContext）

| 实体 | 表名 | 状态 |
|------|------|------|
| ProdAssembleData | prod_assemble_data | ✅ 实体存在，无独立Service |
| ProdTestData | prod_test_data | ✅ 实体存在，仅TestManagement查询 |
| SpcMeasurement | spc_measurement | ✅ 有完整Service |
| MasterRecipe | master_recipe | ✅ 有Service |
| MasterRoute | master_route | ✅ 有Service |
| MasterRouteStep | master_route_step | ✅ 有Service |
| QualityInspection | quality_inspection | ✅ 实体存在，Phase 3.4基础 |
| QualityInspectionItem | quality_inspection_item | ✅ 实体存在 |

### 1.4 缺失项清单

- ❌ 工序参数模板管理（无parameter_template表、无Service）
- ❌ 参数上下限自动校验（DTO有Usl/Lsl但Service未校验）
- ❌ Bin码主数据管理（无master_bin_code表）
- ❌ 金线/铜线类型管理（无wire_material表）
- ❌ 模具/工具寿命管理（无tool_definition表）
- ❌ 操作员资质管理（无operator_qualification表）
- ❌ 首件检验后端API（FirstArticleInspectionView仅有空UI）
- ❌ 工艺参数自动采集（无EAP集成）
- ❌ TrackIn与参数记录联动（TrackIn不自动创建参数记录）

---

## 二、现有UI详细评估

### 2.1 执行模块 (MES.Modules.Execute)

#### TrackInView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Execute/Views/TrackInView.xaml` |
| **ViewModel** | `src/Client/MES.Modules.Execute/ViewModels/TrackInViewModel.cs` |
| **当前功能** | 仅有占位文本"生产执行 - 批次投入 (开发中...)"，ViewModel有基础框架但未对接真实API |
| **符合场景** | ❌ 不满足 |
| **缺失功能** | 批次扫描录入、设备选择、Recipe选择、参数模板展示、资质校验显示、实时参数录入、超限告警提示 |
| **UI优化建议** | 1. 拆分为左右两栏：左侧批次列表，右侧参数面板<br>2. 添加ComboBox绑定设备列表(`GET /api/Equipment`)<br>3. 添加DataGrid展示当前工序参数模板，含Usl/Lsl实时校验<br>4. 参数超限行高亮红色背景<br>5. 底部添加"确认进站"和"确认出站"按钮，出站前校验资质<br>6. 添加资质状态Badge（✅已认证 / ⚠️即将到期 / ❌无资质） |
| **ViewModel是否完整** | ⚠️ 有基础Command和属性，但`IExecuteService`非真实后端Service |
| **是否对接真实API** | ❌ 否，使用本地`IExecuteService`模拟 |

#### DispatchView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Execute/Views/DispatchView.xaml` |
| **ViewModel** | `src/Client/MES.Modules.Execute/ViewModels/DispatchViewModel.cs` |
| **当前功能** | 仅有占位文本"生产执行 - 派工管理 (开发中...)" |
| **符合场景** | ❌ 不满足 |
| **缺失功能** | 派工任务列表、设备分配、优先级调整、操作员指派、资质匹配校验 |
| **UI优化建议** | 1. 上部搜索栏(工单号/批次号/设备)<br>2. 中部DataGrid展示待派工任务<br>3. 右侧弹出式设备选择面板<br>4. 添加资质匹配标识列 |

#### WipOverviewView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Execute/Views/WipOverviewView.xaml` |
| **ViewModel** | `src/Client/MES.Modules.Execute/ViewModels/WipOverviewViewModel.cs` |
| **当前功能** | 仅有占位文本"生产执行 - 在制品概览 (开发中...)" |
| **符合场景** | ❌ 不满足 |
| **缺失功能** | 在制品统计看板、工序分布图、Hold批次标识、滞留时间预警 |
| **UI优化建议** | 1. 顶部统计卡片(总在制数/待处理/生产中/Hold)<br>2. 中部工序流转图(横向时间轴)<br>3. 底部滞留超时列表(>8h高亮橙色，>24h红色) |

#### AlarmDashboardView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Execute/Views/AlarmDashboardView.xaml` |
| **ViewModel** | `src/Client/MES.Modules.Execute/ViewModels/AlarmDashboardViewModel.cs` |
| **当前功能** | 仅有占位文本"生产执行 - 告警看板 (开发中...)" |
| **符合场景** | ❌ 不满足 |
| **缺失功能** | 实时告警列表、告警分类筛选、参数超限告警、寿命预警告警 |

### 2.2 质量模块 (MES.Modules.Quality)

#### FirstArticleInspectionView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Quality/Views/FirstArticleInspectionView.xaml` |
| **ViewModel** | ❌ 不存在 |
| **当前功能** | 仅有占位文本"质量管理 - 首件检验 (开发中...)" |
| **符合场景** | ❌ 不满足 |
| **缺失功能** | 首件模板选择、检验项目列表、检验结果录入、判定结论、照片/附件上传、首件放行审批 |
| **UI优化建议** | 1. 顶部批次信息卡片<br>2. 左侧首件模板树形选择器<br>3. 右侧检验项目DataGrid(项目名、规格、实测值、判定结果)<br>4. 底部审批流进度条<br>5. 判定结果用颜色标识：通过(绿)/不通过(红)/待定(黄) |
| **ViewModel是否完整** | ❌ 不存在 |
| **是否对接真实API** | ❌ 无后端API |

#### SpcChartView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Quality/Views/SpcChartView.xaml` |
| **ViewModel** | `AllQualityViewModels.cs` |
| **当前功能** | DataGrid展示SPC数据项(设备ID、参数、UCL/CL/LCL、最新值) |
| **符合场景** | ⚠️ 部分满足 |
| **缺失功能** | SPC控制图可视化(X-bar/R图)、趋势分析、超差点标注、Cpk展示 |
| **UI优化建议** | 1. 添加图表控件展示X-bar控制图<br>2. UCL/CL/LCL用虚线标注<br>3. 超差点用红色圆点突出<br>4. 右侧统计面板展示Cpk/Ppk<br>5. 顶部添加参数选择器下拉框 |
| **ViewModel是否完整** | ⚠️ 基础数据绑定存在，缺图表逻辑 |
| **是否对接真实API** | ⚠️ 部分对接(SpcMeasurementService) |

#### SpcRuleConfigView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Quality/Views/SpcRuleConfigView.xaml` |
| **当前功能** | DataGrid展示SPC规则配置 |
| **符合场景** | ⚠️ 部分满足 |
| **UI优化建议** | 1. 添加Western Electric Rules配置面板<br>2. 规则启用/停用Toggle<br>3. 灵敏度调整滑块 |

#### FdcMonitorView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Quality/Views/FdcMonitorView.xaml` |
| **当前功能** | DataGrid展示FDC数据(设备ID、腔体、T2、SPE、状态) |
| **符合场景** | ⚠️ 部分满足 |
| **UI优化建议** | 1. 添加实时波形图<br>2. 状态用颜色Badge区分 |

#### OocEventView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Quality/Views/OocEventView.xaml` |
| **当前功能** | 失控事件列表 |
| **符合场景** | ⚠️ 部分满足 |
| **UI优化建议** | 1. 关联批次跳转<br>2. 处置状态筛选 |

#### InspectionView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Quality/Views/InspectionView.xaml` |
| **当前功能** | 检验界面（具体待查） |
| **符合场景** | ⚠️ 部分满足 |

### 2.3 配方模块 (MES.Modules.Recipe)

#### RecipeListView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Recipe/Views/RecipeListView.xaml` |
| **当前功能** | DataGrid展示配方列表(配方ID、配方名、版本、状态、设备类型、创建人) |
| **符合场景** | ⚠️ 部分满足 |
| **缺失功能** | 参数模板关联展示、工序绑定关系、线材类型标识(金线/铜线) |
| **UI优化建议** | 1. 添加线材类型列<br>2. 添加关联参数模板数<br>3. 双击查看参数详情 |

#### RecipeParameterView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Recipe/Views/RecipeParameterView.xaml` |
| **当前功能** | 配方参数展示 |
| **符合场景** | ⚠️ 部分满足 |
| **UI优化建议** | 1. 添加Usl/Lsl/Target编辑列<br>2. 参数超限变色<br>3. 与工序参数模板联动 |

#### RecipeApprovalView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Recipe/Views/RecipeApprovalView.xaml` |
| **当前功能** | 配方审批界面 |
| **符合场景** | ⚠️ 部分满足 |
| **UI优化建议** | 1. 添加参数变更对比视图(变更前后)<br>2. 审批意见输入 |

#### RecipeDispatchView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Recipe/Views/RecipeDispatchView.xaml` |
| **当前功能** | 配方下发界面 |
| **符合场景** | ⚠️ 部分满足 |

#### RecipeEquipmentView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Recipe/Views/RecipeEquipmentView.xaml` |
| **当前功能** | 配方与设备绑定 |
| **符合场景** | ⚠️ 部分满足 |

#### RecipeVersionView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Recipe/Views/RecipeVersionView.xaml` |
| **当前功能** | 配方版本管理 |
| **符合场景** | ✅ 基本满足 |

### 2.4 批次模块 (MES.Modules.Lot)

#### LotDetailView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Lot/Views/LotDetailView.xaml` |
| **ViewModel** | `src/Client/MES.Modules.Lot/ViewModels/LotDetailViewModel.cs` |
| **当前功能** | 仅有占位文本"批次管理 - 批次详情 (开发中...)" |
| **符合场景** | ❌ 不满足 |
| **缺失功能** | 批次全景信息(工序进度、参数记录、Bin结果、测试数据、使用物料、操作员记录) |
| **UI优化建议** | 1. Tab式布局：基本信息/工序进度/参数记录/测试结果/物料使用/追溯链<br>2. 工序进度用流程图展示<br>3. 参数记录用DataGrid+条件格式(超限高亮)<br>4. Bin结果用饼图展示<br>5. 物料使用记录线材类型、模具编号 |

#### LotListView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Lot/Views/LotListView.xaml` |
| **ViewModel** | `src/Client/MES.Modules.Lot/ViewModels/LotListViewModel.cs` |
| **当前功能** | 批次列表 |
| **符合场景** | ⚠️ 部分满足 |
| **UI优化建议** | 1. 添加状态筛选<br>2. 添加Hold标识列<br>3. 双击跳转详情 |

#### LotHoldView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Lot/Views/LotHoldView.xaml` |
| **ViewModel** | `src/Client/MES.Modules.Lot/ViewModels/LotHoldViewModel.cs` |
| **当前功能** | 批次Hold管理 |
| **符合场景** | ⚠️ 部分满足 |
| **UI优化建议** | 1. 添加Hold原因分类<br>2. Hold关联参数超限自动Hold标记 |

#### LotSplitMergeView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Lot/Views/LotSplitMergeView.xaml` |
| **当前功能** | 批次拆分合并 |
| **符合场景** | ⚠️ 部分满足 |

#### LotArchiveView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Lot/Views/LotArchiveView.xaml` |
| **当前功能** | 批次归档 |
| **符合场景** | ✅ 基本满足 |

### 2.5 良率模块 (MES.Modules.Yield)

#### BinAnalysisView

| 项目 | 详情 |
|------|------|
| **文件路径** | `src/Client/MES.Modules.Yield/Views/BinAnalysisView.xaml` |
| **当前功能** | Bin数据分析 |
| **符合场景** | ⚠️ 部分满足 |
| **UI优化建议** | 1. 添加Bin码维度筛选<br>2. Bin趋势图<br>3. Bin与SPC参数关联分析 |

---

## 三、工序参数管控模块（核心改造）

### 3.1 现状分析

**ProcessExecutionService** 已有 `RecordParametersAsync` 方法接收 `List<StepParameterRecord>`，每个 `StepParameterRecord` 已包含 `Usl`/`Lsl` 字段，但：
- ❌ **无任何校验逻辑**：参数值超出Usl/Lsl时不拦截
- ❌ **无参数模板**：每次手动传入参数名、Usl、Lsl，无复用
- ❌ **无强制录入校验**：关键参数不填也能完成工序
- ❌ **参数记录与SPC无联动**：不自动写入SpcMeasurement
- ❌ **无参数变更记录**：修改参数值无审计追踪

### 3.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 描述 |
|---------|---------|--------|------|
| PE-001 | 工序参数模板管理 | P0 | 按工序+配方定义参数模板，含名称、单位、Usl/Lsl/Target、是否必填、数据类型 |
| PE-002 | 参数模板版本管理 | P1 | 模板支持版本控制，变更需审批 |
| PE-003 | 参数上下限校验 | P0 | 录入参数时自动校验Usl/Lsl，超限拦截或告警 |
| PE-004 | 强制参数校验 | P0 | 标记为必填的参数，未录入不允许完成工序 |
| PE-005 | 参数超限自动Hold | P1 | 关键参数超限自动Hold批次 |
| PE-006 | 参数自动采集 | P2 | 通过EAP接口自动获取设备参数 |
| PE-007 | 参数变更记录 | P1 | 参数修改留痕，支持变更对比 |
| PE-008 | 参数-SPC联动 | P1 | 参数记录自动写入SpcMeasurement |

### 3.3 数据表设计

#### 3.3.1 master_parameter_template（工序参数模板主表）

```sql
CREATE TABLE master_parameter_template (
    template_id         VARCHAR(50)     NOT NULL COMMENT '模板ID，如PT-WB-001',
    template_name       VARCHAR(100)    NOT NULL COMMENT '模板名称',
    step_code           VARCHAR(50)     NOT NULL COMMENT '关联工序代码',
    recipe_id           VARCHAR(100)    NULL     COMMENT '关联配方ID，NULL表示通用模板',
    product_id          VARCHAR(50)     NULL     COMMENT '关联产品ID，NULL表示通用',
    version             VARCHAR(20)     NOT NULL DEFAULT '1.0' COMMENT '版本号',
    is_active           TINYINT(1)      NOT NULL DEFAULT 1 COMMENT '是否启用',
    is_approved         TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否已审批',
    approved_by         VARCHAR(50)     NULL     COMMENT '审批人',
    approved_at         DATETIME        NULL     COMMENT '审批时间',
    created_by          VARCHAR(50)     NOT NULL COMMENT '创建人',
    created_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (template_id),
    UNIQUE KEY uk_template_step_recipe (step_code, recipe_id, version),
    KEY idx_step_code (step_code),
    KEY idx_recipe_id (recipe_id),
    KEY idx_product_id (product_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工序参数模板主表';
```

#### 3.3.2 master_parameter_template_item（参数模板明细）

```sql
CREATE TABLE master_parameter_template_item (
    item_id             BIGINT          NOT NULL AUTO_INCREMENT COMMENT '明细ID',
    template_id         VARCHAR(50)     NOT NULL COMMENT '模板ID',
    parameter_code      VARCHAR(50)     NOT NULL COMMENT '参数代码，如BOND_FORCE',
    parameter_name      VARCHAR(100)    NOT NULL COMMENT '参数名称，如键合力',
    data_type           VARCHAR(20)     NOT NULL DEFAULT 'decimal' COMMENT '数据类型: decimal/int/string/boolean',
    unit                VARCHAR(20)     NULL     COMMENT '单位，如gf/℃/s',
    target_value        DECIMAL(10,4)   NULL     COMMENT '目标值',
    usl                 DECIMAL(10,4)   NULL     COMMENT '规格上限',
    lsl                 DECIMAL(10,4)   NULL     COMMENT '规格下限',
    ucl                 DECIMAL(10,4)   NULL     COMMENT '控制上限',
    lcl                 DECIMAL(10,4)   NULL     COMMENT '控制下限',
    is_mandatory        TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否必填',
    is_spc_monitored    TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否SPC监控',
    is_critical         TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否关键参数(超限自动Hold)',
    auto_collect        TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否自动采集',
    eap_signal_name     VARCHAR(100)    NULL     COMMENT 'EAP信号名称(自动采集用)',
    display_order       INT             NOT NULL DEFAULT 0 COMMENT '显示顺序',
    remark              VARCHAR(255)    NULL     COMMENT '备注',
    PRIMARY KEY (item_id),
    UNIQUE KEY uk_template_item (template_id, parameter_code),
    KEY idx_parameter_code (parameter_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工序参数模板明细';
```

#### 3.3.3 prod_step_parameter_record（工序参数实际记录）

```sql
CREATE TABLE prod_step_parameter_record (
    record_id           BIGINT          NOT NULL AUTO_INCREMENT COMMENT '记录ID',
    lot_id              VARCHAR(50)     NOT NULL COMMENT '批次ID',
    step_code           VARCHAR(50)     NOT NULL COMMENT '工序代码',
    operation_id        VARCHAR(50)     NOT NULL COMMENT '操作ID',
    parameter_code      VARCHAR(50)     NOT NULL COMMENT '参数代码',
    parameter_name      VARCHAR(100)    NOT NULL COMMENT '参数名称',
    parameter_value     DECIMAL(10,4)   NULL     COMMENT '参数值',
    string_value        VARCHAR(255)    NULL     COMMENT '字符串值(当数据类型为string时)',
    unit                VARCHAR(20)     NULL     COMMENT '单位',
    usl                 DECIMAL(10,4)   NULL     COMMENT '录入时规格上限',
    lsl                 DECIMAL(10,4)   NULL     COMMENT '录入时规格下限',
    target_value        DECIMAL(10,4)   NULL     COMMENT '目标值',
    is_out_of_spec      TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否超规格',
    is_auto_collected   TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否自动采集',
    collected_from      VARCHAR(50)     NULL     COMMENT '采集来源: EAP/MANUAL',
    recorded_by         VARCHAR(50)     NULL     COMMENT '记录人',
    recorded_at         DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '记录时间',
    PRIMARY KEY (record_id),
    KEY idx_lot_step (lot_id, step_code),
    KEY idx_operation (operation_id),
    KEY idx_parameter (parameter_code),
    KEY idx_recorded_at (recorded_at),
    KEY idx_out_of_spec (is_out_of_spec)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工序参数实际记录表';
```

### 3.4 接口设计

#### 3.4.1 ParameterTemplateController

**文件**: `src/Server/MES.Api/Controllers/ParameterTemplateController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
public class ParameterTemplateController : ControllerBase
{
    private readonly IParameterTemplateService _service;

    // POST /api/ParameterTemplate
    // 创建参数模板
    [HttpPost]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateParameterTemplateRequest request);

    // PUT /api/ParameterTemplate/{templateId}
    // 更新参数模板
    [HttpPut("{templateId}")]
    public async Task<IActionResult> UpdateTemplate(string templateId, [FromBody] UpdateParameterTemplateRequest request);

    // GET /api/ParameterTemplate/{templateId}
    // 获取模板详情（含明细）
    [HttpGet("{templateId}")]
    public async Task<IActionResult> GetTemplate(string templateId);

    // GET /api/ParameterTemplate/by-step/{stepCode}
    // 按工序查询模板列表
    [HttpGet("by-step/{stepCode}")]
    public async Task<IActionResult> GetTemplatesByStep(string stepCode, [FromQuery] string? recipeId, [FromQuery] string? productId);

    // GET /api/ParameterTemplate/active/{stepCode}/{recipeId?}
    // 获取当前生效模板（用于TrackIn时自动加载）
    [HttpGet("active/{stepCode}")]
    public async Task<IActionResult> GetActiveTemplate(string stepCode, [FromQuery] string? recipeId, [FromQuery] string? productId);

    // POST /api/ParameterTemplate/{templateId}/approve
    // 审批模板
    [HttpPost("{templateId}/approve")]
    public async Task<IActionResult> ApproveTemplate(string templateId, [FromBody] TemplateApprovalRequest request);

    // GET /api/ParameterTemplate
    // 分页查询模板
    [HttpGet]
    public async Task<IActionResult> QueryTemplates([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? stepCode, [FromQuery] string? keyword);
}
```

#### 3.4.2 ProcessExecutionService 扩展

**文件**: `src/Server/MES.Services.Production/ProcessExecutionService.cs`（修改）

新增方法签名（`IProcessExecutionService` 接口同步扩展）：

```csharp
// 验证参数是否在规格范围内
Task<ParameterValidationResult> ValidateParametersAsync(string lotId, string stepCode, List<StepParameterRecord> parameters);

// 记录参数（增强版：自动校验+SPC联动+超限告警）
Task<ParameterRecordResult> RecordParametersWithValidationAsync(string lotId, string stepCode, string operatorId, List<StepParameterRecord> parameters);

// 获取工序参数模板
Task<ParameterTemplateDto> GetActiveParameterTemplateAsync(string stepCode, string? recipeId, string? productId);

// 获取批次工序参数记录
Task<List<StepParameterRecordDto>> GetLotStepParametersAsync(string lotId, string stepCode);

// 检查工序参数完整性（是否所有必填参数已录入）
Task<ParameterCompletenessResult> CheckParameterCompletenessAsync(string lotId, string stepCode);
```

#### 3.4.3 扩展的DTO定义

**文件**: `src/Shared/MES.Contracts/Production/ParameterTemplateDto.cs`（新建）

```csharp
namespace MES.Contracts.Production;

/// <summary>
/// 参数模板DTO
/// </summary>
public class ParameterTemplateDto
{
    public string TemplateId { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string? RecipeId { get; set; }
    public string? ProductId { get; set; }
    public string Version { get; set; } = "1.0";
    public bool IsActive { get; set; }
    public bool IsApproved { get; set; }
    public List<ParameterTemplateItemDto> Items { get; set; } = [];
}

/// <summary>
/// 参数模板明细
/// </summary>
public class ParameterTemplateItemDto
{
    public long ItemId { get; set; }
    public string ParameterCode { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public string DataType { get; set; } = "decimal";
    public string? Unit { get; set; }
    public decimal? TargetValue { get; set; }
    public decimal? Usl { get; set; }
    public decimal? Lsl { get; set; }
    public decimal? Ucl { get; set; }
    public decimal? Lcl { get; set; }
    public bool IsMandatory { get; set; }
    public bool IsSpcMonitored { get; set; }
    public bool IsCritical { get; set; }
    public bool AutoCollect { get; set; }
    public string? EapSignalName { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>
/// 参数验证结果
/// </summary>
public class ParameterValidationResult
{
    public bool IsAllInSpec { get; set; }
    public bool HasCriticalViolation { get; set; }  // 有关键参数超限
    public List<ParameterViolationDetail> Violations { get; set; } = [];
}

/// <summary>
/// 参数违规明细
/// </summary>
public class ParameterViolationDetail
{
    public string ParameterCode { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public decimal ActualValue { get; set; }
    public decimal? Usl { get; set; }
    public decimal? Lsl { get; set; }
    public bool IsCritical { get; set; }
    public string ViolationType { get; set; } = string.Empty; // "AboveUSL" / "BelowLSL" / "Missing"
}

/// <summary>
/// 参数记录结果
/// </summary>
public class ParameterRecordResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int RecordedCount { get; set; }
    public int OutOfSpecCount { get; set; }
    public bool HasCriticalViolation { get; set; }
    public List<ParameterViolationDetail> Violations { get; set; } = [];
    public List<long> SpcMeasurementIds { get; set; } = [];  // 关联的SPC测量ID
}

/// <summary>
/// 参数完整性检查结果
/// </summary>
public class ParameterCompletenessResult
{
    public bool IsComplete { get; set; }
    public int TotalRequired { get; set; }
    public int RecordedCount { get; set; }
    public List<string> MissingParameters { get; set; } = [];
}

/// <summary>
/// 参数记录DTO（查询返回用）
/// </summary>
public class StepParameterRecordDto
{
    public long RecordId { get; set; }
    public string LotId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string ParameterCode { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public decimal? ParameterValue { get; set; }
    public string? StringValue { get; set; }
    public string? Unit { get; set; }
    public decimal? Usl { get; set; }
    public decimal? Lsl { get; set; }
    public decimal? TargetValue { get; set; }
    public bool IsOutOfSpec { get; set; }
    public bool IsAutoCollected { get; set; }
    public string RecordedBy { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; }
}
```

### 3.5 业务规则

| 规则编号 | 规则名称 | 规则描述 | 触发动作 |
|---------|---------|---------|---------|
| PR-01 | 必填参数校验 | 完成工序前，所有标记为IsMandatory的参数必须已录入 | 阻止工序完成，提示缺失参数列表 |
| PR-02 | 上下限校验 | 参数值超出Usl/Lsl范围时标记IsOutOfSpec=true | 记录违规，触发告警 |
| PR-03 | 关键参数超限拦截 | 标记为IsCritical的参数超限时 | 自动Hold批次，记录Hold原因 |
| PR-04 | SPC联动写入 | 标记为IsSpcMonitored的参数记录后 | 自动创建SpcMeasurement记录 |
| PR-05 | 模板匹配优先级 | recipe专属模板 > 产品专属模板 > 通用模板 | TrackIn时自动加载最高优先级模板 |
| PR-06 | 模板审批生效 | 未审批(is_approved=false)的模板不生效 | 使用上一已审批版本 |
| PR-07 | 自动采集优先 | AutoCollect=true的参数优先从EAP采集 | EAP采集失败时允许手动录入并标记 |

### 3.6 UI改造建议（TrackInView增强）

**文件**: `src/Client/MES.Modules.Execute/Views/TrackInView.xaml`

```xml
<!-- 整体布局 -->
<Grid Margin="24">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>  <!-- 顶部：批次输入区 -->
        <RowDefinition Height="Auto"/>  <!-- 统计信息卡片 -->
        <RowDefinition Height="*"/>     <!-- 主体：参数录入区 -->
        <RowDefinition Height="Auto"/>  <!-- 底部：操作按钮 -->
    </Grid.RowDefinitions>

    <!-- Row 0: 批次输入区 -->
    <StackPanel Orientation="Horizontal" Gap="12" Margin="0,0,0,16">
        <TextBox Width="200" Text="{Binding LotId}" 
                 materialDesign:HintAssist.Hint="扫描/输入批次ID" />
        <ComboBox Width="180" ItemsSource="{Binding EquipmentList}"
                  SelectedValue="{Binding SelectedEquipmentId}"
                  materialDesign:HintAssist.Hint="选择设备" />
        <ComboBox Width="150" ItemsSource="{Binding OperatorList}"
                  SelectedValue="{Binding SelectedOperatorId}"
                  materialDesign:HintAssist.Hint="操作员" />
        <!-- 资质Badge -->
        <Border Padding="8,4" CornerRadius="4"
                Background="{Binding OperatorQualificationColor}"
                Visibility="{Binding ShowQualificationBadge, Converter={StaticResource BooleanToVisibilityConverter}}">
            <TextBlock Text="{Binding OperatorQualificationText}" Foreground="White" FontWeight="SemiBold"/>
        </Border>
        <Button Content="进站" Command="{Binding TrackInCommand}" Style="{StaticResource PrimaryButton}"/>
    </StackPanel>

    <!-- Row 1: 统计卡片 -->
    <UniformGrid Columns="4" Margin="0,0,0,16">
        <!-- 待处理数 / 生产中 / 今日完成 / 超限告警 -->
        <Border Style="{StaticResource StatCard}">
            <StackPanel>
                <TextBlock Text="{Binding PendingCount}" FontSize="28" FontWeight="Bold"/>
                <TextBlock Text="待处理" Foreground="Gray"/>
            </StackPanel>
        </Border>
        <!-- ... 其他3个卡片 ... -->
    </UniformGrid>

    <!-- Row 2: 主体区（左右分栏） -->
    <Grid Grid.Row="2">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="300"/>  <!-- 左侧：批次列表 -->
            <ColumnDefinition Width="*"/>     <!-- 右侧：参数面板 -->
        </Grid.ColumnDefinitions>

        <!-- 左侧：批次列表 -->
        <DataGrid ItemsSource="{Binding TrackInRecords}" SelectedItem="{Binding SelectedRecord}"
                  AutoGenerateColumns="False" IsReadOnly="True">
            <DataGrid.Columns>
                <DataGridTextColumn Header="批次ID" Binding="{Binding LotId}" Width="*"/>
                <DataGridTextColumn Header="工序" Binding="{Binding StepCode}" Width="80"/>
                <DataGridTextColumn Header="状态" Binding="{Binding Status}" Width="70"/>
                <DataGridTextColumn Header="进站时间" Binding="{Binding TrackInTime}" Width="120"/>
                <!-- 超限标识 -->
                <DataGridTemplateColumn Header="超限" Width="50">
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <Ellipse Width="16" Height="16" Fill="{Binding HasViolation, Converter={StaticResource BoolToColorConverter}}"
                                     ToolTip="{Binding ViolationCount, StringFormat='{}{0}项超限'}"/>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>
            </DataGrid.Columns>
        </DataGrid>

        <!-- 右侧：参数面板 -->
        <TabControl Grid.Column="1">
            <TabItem Header="参数录入">
                <DataGrid ItemsSource="{Binding ParameterItems}" AutoGenerateColumns="False">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="参数名称" Binding="{Binding ParameterName}" Width="120"/>
                        <DataGridTextColumn Header="规格下限" Binding="{Binding Lsl}" Width="80"/>
                        <DataGridTextColumn Header="目标值" Binding="{Binding TargetValue}" Width="80"/>
                        <DataGridTextColumn Header="规格上限" Binding="{Binding Usl}" Width="80"/>
                        <DataGridTextColumn Header="单位" Binding="{Binding Unit}" Width="60"/>
                        <DataGridTemplateColumn Header="实测值" Width="120">
                            <DataGridTemplateColumn.CellTemplate>
                                <DataTemplate>
                                    <TextBox Text="{Binding ActualValue, UpdateSourceTrigger=PropertyChanged}"
                                             Background="{Binding IsOutOfSpec, Converter={StaticResource BoolToBackgroundConverter}}"
                                             LostFocus="OnParameterValueChanged"/>
                                </DataTemplate>
                            </DataGridTemplateColumn.CellTemplate>
                        </DataGridTemplateColumn>
                        <DataGridTextColumn Header="判定" Binding="{Binding ValidationResult}" Width="80">
                            <DataGridTextColumn.ElementStyle>
                                <Style TargetType="TextBlock">
                                    <Style.Triggers>
                                        <DataTrigger Binding="{Binding ValidationResult}" Value="OK">
                                            <Setter Property="Foreground" Value="Green"/>
                                        </DataTrigger>
                                        <DataTrigger Binding="{Binding ValidationResult}" Value="NG">
                                            <Setter Property="Foreground" Value="Red" FontWeight="Bold"/>
                                        </DataTrigger>
                                    </Style.Triggers>
                                </Style>
                            </DataGridTextColumn.ElementStyle>
                        </DataGridTextColumn>
                        <!-- 必填标记 -->
                        <DataGridTemplateColumn Header="必填" Width="50">
                            <DataGridTemplateColumn.CellTemplate>
                                <DataTemplate>
                                    <TextBlock Text="*" Foreground="Red" Visibility="{Binding IsMandatory, Converter={StaticResource BooleanToVisibilityConverter}}"/>
                                </DataTemplate>
                            </DataGridTemplateColumn.CellTemplate>
                        </DataGridTemplateColumn>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
            <TabItem Header="历史参数">
                <!-- 历史参数对比 -->
            </TabItem>
        </TabControl>
    </Grid>

    <!-- Row 3: 操作按钮 -->
    <StackPanel Grid.Row="3" Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,16,0,0" Gap="12">
        <Button Content="保存参数" Command="{Binding SaveParametersCommand}" Style="{StaticResource SecondaryButton}"/>
        <Button Content="出站" Command="{Binding TrackOutCommand}" Style="{StaticResource PrimaryButton}"/>
    </StackPanel>
</Grid>
```

---

## 四、Bin管理模块（新增）

### 4.1 现状分析

`ProdTestData` 实体已有 `Bin1Qty`~`Bin8Qty`、`BinSummary`、`YieldPercent` 等字段，`TestManagementService` 中 `CpTestRequest`/`FtTestRequest` 已支持 `BinDistribution`。但：
- ❌ 无Bin码主数据表（BinCode、BinName、BinType/Pass/Fail、等级定义）
- ❌ 无Bin与良率的联动判定规则
- ❌ 无Bin分布趋势分析
- ❌ Bin结果未与SPC参数关联

### 4.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 描述 |
|---------|---------|--------|------|
| BIN-001 | Bin码主数据管理 | P0 | 定义Bin代码、名称、类型(Pass/Fail/Inconclusive)、等级 |
| BIN-002 | Bin结果判定 | P0 | 根据Bin分布自动判定批次最终结果 |
| BIN-003 | Bin-Yield联动 | P0 | Bin分布变化实时计算良率 |
| BIN-004 | Bin趋势分析 | P1 | 按产品/时间维度统计Bin分布趋势 |
| BIN-005 | Bin-SPC关联 | P1 | 分析特定Bin与工艺参数的关联性 |
| BIN-006 | Bin码快速录入 | P1 | 测试界面支持Bin码批量录入 |

### 4.3 数据表设计

#### 4.3.1 master_bin_code（Bin码主数据）

```sql
CREATE TABLE master_bin_code (
    bin_code            VARCHAR(20)     NOT NULL COMMENT 'Bin代码，如BIN1/BIN2',
    bin_name            VARCHAR(100)    NOT NULL COMMENT 'Bin名称',
    bin_type            VARCHAR(20)     NOT NULL COMMENT '类型: Pass/Fail/Inconclusive',
    bin_category        VARCHAR(50)     NULL     COMMENT '分类: OPEN/CLOSE/WAFER/STRIP等',
    bin_grade           VARCHAR(20)     NULL     COMMENT '等级: GradeA/GradeB/Scrap',
    description         VARCHAR(255)    NULL     COMMENT '描述',
    is_active           TINYINT(1)      NOT NULL DEFAULT 1,
    created_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (bin_code),
    KEY idx_bin_type (bin_type),
    KEY idx_bin_category (bin_category)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Bin码主数据';

-- 预置数据
INSERT INTO master_bin_code (bin_code, bin_name, bin_type, bin_category, bin_grade, description) VALUES
('BIN1', 'Good Die', 'Pass', 'Final', 'GradeA', '合格品'),
('BIN2', 'Open/Short', 'Fail', 'Electrical', 'Scrap', '开路/短路'),
('BIN3', 'Leakage Fail', 'Fail', 'Electrical', 'Scrap', '漏电不良'),
('BIN4', 'Speed Fail', 'Fail', 'Performance', 'GradeB', '速度不良'),
('BIN5', 'Function Fail', 'Fail', 'Functional', 'Scrap', '功能不良'),
('BIN6', 'Retest', 'Inconclusive', 'Retest', NULL, '需要复测'),
('BIN7', 'Cosmetic Fail', 'Fail', 'Cosmetic', 'GradeB', '外观不良'),
('BIN8', 'Package Fail', 'Fail', 'Package', 'Scrap', '封装不良');
```

#### 4.3.2 bin_result_summary（Bin结果汇总表）

```sql
CREATE TABLE bin_result_summary (
    summary_id          BIGINT          NOT NULL AUTO_INCREMENT,
    lot_id              VARCHAR(50)     NOT NULL COMMENT '批次ID',
    mfg_id              VARCHAR(50)     NULL     COMMENT '单元ID',
    step_code           VARCHAR(50)     NOT NULL COMMENT '测试工序代码',
    test_id             VARCHAR(50)     NULL     COMMENT '测试记录ID',
    total_qty           INT             NOT NULL COMMENT '总数量',
    pass_qty            INT             NOT NULL DEFAULT 0 COMMENT '合格数量',
    fail_qty            INT             NOT NULL DEFAULT 0 COMMENT '不合格数量',
    yield_percent       DECIMAL(5,2)    NULL     COMMENT '良率百分比',
    lot_bin_result      VARCHAR(50)     NULL     COMMENT '批次Bin结果: AllPass/PartialFail/AllFail',
    test_result         VARCHAR(50)     NULL     COMMENT '测试结论: Pass/Fail/Hold',
    bin_details         JSON            NULL     COMMENT 'Bin明细: [{"binCode":"BIN1","count":100},...]',
    recorded_at         DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    recorded_by         VARCHAR(50)     NULL,
    PRIMARY KEY (summary_id),
    UNIQUE KEY uk_lot_step_test (lot_id, step_code, test_id),
    KEY idx_lot_id (lot_id),
    KEY idx_recorded_at (recorded_at),
    KEY idx_bin_result (lot_bin_result)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Bin结果汇总表';
```

#### 4.3.3 bin_yield_rule（Bin-良率判定规则）

```sql
CREATE TABLE bin_yield_rule (
    rule_id             VARCHAR(50)     NOT NULL,
    product_id          VARCHAR(50)     NULL     COMMENT '产品ID，NULL表示通用',
    route_id            VARCHAR(100)    NULL     COMMENT '工艺路线ID',
    step_code           VARCHAR(50)     NULL     COMMENT '工序代码',
    min_yield_percent   DECIMAL(5,2)    NOT NULL COMMENT '最低良率阈值(%)',
    max_fail_bin_count  INT             NULL     COMMENT '最大Fail Bin数量',
    critical_fail_bins  VARCHAR(200)    NULL     COMMENT '关键Fail Bin代码，逗号分隔',
    action_type         VARCHAR(50)     NOT NULL DEFAULT 'Warning' COMMENT '动作: Warning/HoldLot/ScrapLot',
    notify_role         VARCHAR(50)     NULL     COMMENT '通知角色',
    is_active           TINYINT(1)      NOT NULL DEFAULT 1,
    created_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (rule_id),
    KEY idx_product (product_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Bin-良率判定规则';
```

### 4.4 接口设计

#### BinManagementController

**文件**: `src/Server/MES.Api/Controllers/BinManagementController.cs`（新建）

```csharp
[ApiController]
[Route("api/[controller]")]
public class BinManagementController : ControllerBase
{
    private readonly IBinManagementService _service;

    // GET /api/BinManagement/codes
    // 获取所有Bin码
    [HttpGet("codes")]
    public async Task<IActionResult> GetBinCodes([FromQuery] string? type, [FromQuery] string? category);

    // POST /api/BinManagement/codes
    // 创建Bin码
    [HttpPost("codes")]
    public async Task<IActionResult> CreateBinCode([FromBody] CreateBinCodeRequest request);

    // PUT /api/BinManagement/codes/{binCode}
    // 更新Bin码
    [HttpPut("codes/{binCode}")]
    public async Task<IActionResult> UpdateBinCode(string binCode, [FromBody] UpdateBinCodeRequest request);

    // POST /api/BinManagement/record
    // 记录Bin结果
    [HttpPost("record")]
    public async Task<IActionResult> RecordBinResult([FromBody] RecordBinResultRequest request);

    // GET /api/BinManagement/summary/{lotId}
    // 获取批次Bin汇总
    [HttpGet("summary/{lotId}")]
    public async Task<IActionResult> GetBinSummary(string lotId);

    // GET /api/BinManagement/trend
    // Bin趋势分析
    [HttpGet("trend")]
    public async Task<IActionResult> GetBinTrend([FromQuery] string productCode, [FromQuery] int days);

    // GET /api/BinManagement/yield-rules
    // 获取良率判定规则
    [HttpGet("yield-rules")]
    public async Task<IActionResult> GetYieldRules([FromQuery] string? productId);
}
```

### 4.5 业务规则

| 规则编号 | 规则名称 | 规则描述 | 触发动作 |
|---------|---------|---------|---------|
| BR-01 | Bin良率计算 | yield = pass_qty / total_qty × 100 | 实时更新 |
| BR-02 | 批次结果判定 | 良率≥min_yield_percent → Pass，否则 → Fail/Hold | 按bin_yield_rule执行 |
| BR-03 | 关键Fail Bin拦截 | 出现critical_fail_bins中的Bin码 | 自动Hold批次 |
| BR-04 | Bin码校验 | 录入的BinCode必须存在于master_bin_code | 拒绝无效Bin码 |

### 4.6 UI设计建议（BinAnalysisView增强）

**文件**: `src/Client/MES.Modules.Yield/Views/BinAnalysisView.xaml`

```xml
<!-- Bin分析面板 -->
<Grid Margin="24">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>  <!-- 筛选栏 -->
        <RowDefinition Height="Auto"/>  <!-- 统计卡片 -->
        <RowDefinition Height="*"/>     <!-- 主体 -->
    </Grid.RowDefinitions>

    <!-- 筛选栏 -->
    <StackPanel Orientation="Horizontal" Gap="12">
        <ComboBox ItemsSource="{Binding ProductList}" SelectedItem="{Binding SelectedProduct}"
                  Width="200" materialDesign:HintAssist.Hint="产品"/>
        <DatePicker SelectedDate="{Binding FromDate}" Width="150"
                    materialDesign:HintAssist.Hint="起始日期"/>
        <DatePicker SelectedDate="{Binding ToDate}" Width="150"
                    materialDesign:HintAssist.Hint="截止日期"/>
        <Button Content="查询" Command="{Binding QueryCommand}"/>
    </StackPanel>

    <!-- 统计卡片 -->
    <UniformGrid Columns="5" Margin="0,16">
        <!-- 总批次/平均良率/PASS Bin数/FAIL Bin数/超限次数 -->
    </UniformGrid>

    <!-- 主体：左侧Bin分布图 + 右侧Bin趋势图 -->
    <Grid Grid.Row="2">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="400"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>

        <!-- Bin分布饼图/柱状图 -->
        <lvc:PieSeries ItemsSource="{Binding BinDistribution}" Title="Bin分布"/>

        <!-- Bin趋势折线图 -->
        <lvc:LineSeries Grid.Column="1" ItemsSource="{Binding BinTrend}" Title="Bin趋势"/>
    </Grid>
</Grid>
```

---

## 五、金线/铜线切换模块（新增）

### 5.1 现状分析

`MasterMaterial` 表有 `material_type` 字段可用于区分线材类型，`ProdAssembleData` 有 `WireBondCount` 字段但无线材类型标识。现有系统：
- ❌ 无法记录批次使用的线材类型
- ❌ 无线材切换记录（换线需要重新验证）
- ❌ 无线材兼容性校验（某些产品只能用金线）
- ❌ 无线材批次追溯

### 5.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 描述 |
|---------|---------|--------|------|
| WM-001 | 线材类型定义 | P0 | 金线/铜线/合金线，含规格、供应商、兼容性 |
| WM-002 | 线材使用记录 | P0 | 每个批次记录使用的线材类型、批次号、用量 |
| WM-003 | 线材切换管理 | P0 | 换线时记录切换时间、操作员、首件验证 |
| WM-004 | 线材兼容性校验 | P1 | 产品工艺路线定义可用线材类型 |
| WM-005 | 线材寿命管理 | P1 | 线材卷轴使用计数，用完预警 |

### 5.3 数据表设计

#### 5.3.1 wire_material_definition（线材定义）

```sql
CREATE TABLE wire_material_definition (
    wire_type_id        VARCHAR(50)     NOT NULL COMMENT '线材类型ID',
    wire_type_name      VARCHAR(100)    NOT NULL COMMENT '名称: 金线/铜线/银合金线',
    wire_material       VARCHAR(50)     NOT NULL COMMENT '材质: Au/Cu/Ag',
    wire_diameter_mm    DECIMAL(5,3)    NOT NULL COMMENT '线径(mm)',
    manufacturer        VARCHAR(100)    NULL     COMMENT '制造商',
    is_active           TINYINT(1)      NOT NULL DEFAULT 1,
    created_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (wire_type_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='线材类型定义';

INSERT INTO wire_material_definition (wire_type_id, wire_type_name, wire_material, wire_diameter_mm, manufacturer) VALUES
('WT-AU-25', '金线-25μm', 'Au', 0.025, 'Tanaka'),
('WT-AU-20', '金线-20μm', 'Au', 0.020, 'Tanaka'),
('WT-CU-25', '铜线-25μm', 'Cu', 0.025, 'Mitsui'),
('WT-CU-20', '铜线-20μm', 'Cu', 0.020, 'Mitsui'),
('WT-CCA-25', '铜合金线-25μm', 'Ag-Cu', 0.025, 'TDS');
```

#### 5.3.2 wire_material_usage（线材使用记录）

```sql
CREATE TABLE wire_material_usage (
    usage_id            BIGINT          NOT NULL AUTO_INCREMENT,
    lot_id              VARCHAR(50)     NOT NULL COMMENT '批次ID',
    step_code           VARCHAR(50)     NOT NULL COMMENT '工序代码(如WireBond)',
    wire_type_id        VARCHAR(50)     NOT NULL COMMENT '线材类型',
    wire_lot_no         VARCHAR(50)     NOT NULL COMMENT '线材批次号',
    wire_reel_id        VARCHAR(50)     NULL     COMMENT '线材卷轴ID',
    used_length_m       DECIMAL(10,2)   NULL     COMMENT '使用长度(米)',
    used_weight_g       DECIMAL(10,2)   NULL     COMMENT '使用重量(克)',
    wire_bond_count     INT             NULL     COMMENT '打线数量',
    operator_id         VARCHAR(50)     NOT NULL COMMENT '操作员',
    used_at             DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (usage_id),
    KEY idx_lot (lot_id),
    KEY idx_wire_lot (wire_lot_no),
    KEY idx_wire_reel (wire_reel_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='线材使用记录';
```

#### 5.3.3 wire_change_record（线材切换记录）

```sql
CREATE TABLE wire_change_record (
    change_id           BIGINT          NOT NULL AUTO_INCREMENT,
    equipment_id        VARCHAR(50)     NOT NULL COMMENT '设备ID',
    step_code           VARCHAR(50)     NOT NULL COMMENT '工序代码',
    old_wire_type       VARCHAR(50)     NULL     COMMENT '原线材类型',
    old_wire_lot_no     VARCHAR(50)     NULL     COMMENT '原线材批次号',
    new_wire_type       VARCHAR(50)     NOT NULL COMMENT '新线材类型',
    new_wire_lot_no     VARCHAR(50)     NOT NULL COMMENT '新线材批次号',
    change_reason       VARCHAR(255)    NULL     COMMENT '切换原因',
    first_article_verified TINYINT(1)   NOT NULL DEFAULT 0 COMMENT '首件是否已验证',
    first_article_result VARCHAR(20)    NULL     COMMENT '首件结果',
    change_by           VARCHAR(50)     NOT NULL COMMENT '操作人',
    changed_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    approved_by         VARCHAR(50)     NULL     COMMENT '审批人',
    PRIMARY KEY (change_id),
    KEY idx_equipment (equipment_id),
    KEY idx_changed_at (changed_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='线材切换记录';
```

#### 5.3.4 product_wire_compatibility（产品-线材兼容性）

```sql
CREATE TABLE product_wire_compatibility (
    id                  BIGINT          NOT NULL AUTO_INCREMENT,
    product_id          VARCHAR(50)     NOT NULL,
    wire_type_id        VARCHAR(50)     NOT NULL,
    is_default          TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否默认',
    is_mandatory        TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否强制(仅允许此类型)',
    PRIMARY KEY (id),
    UNIQUE KEY uk_product_wire (product_id, wire_type_id),
    KEY idx_product (product_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='产品-线材兼容性';
```

### 5.4 接口设计

#### WireManagementController

**文件**: `src/Server/MES.Api/Controllers/WireManagementController.cs`（新建）

```csharp
[ApiController]
[Route("api/[controller]")]
public class WireManagementController : ControllerBase
{
    private readonly IWireManagementService _service;

    // GET /api/WireManagement/types
    [HttpGet("types")]
    public async Task<IActionResult> GetWireTypes([FromQuery] string? material);

    // POST /api/WireManagement/usage
    [HttpPost("usage")]
    public async Task<IActionResult> RecordWireUsage([FromBody] RecordWireUsageRequest request);

    // POST /api/WireManagement/change
    [HttpPost("change")]
    public async Task<IActionResult> RecordWireChange([FromBody] WireChangeRequest request);

    // GET /api/WireManagement/changes
    [HttpGet("changes")]
    public async Task<IActionResult> GetWireChanges([FromQuery] string? equipmentId, [FromQuery] int days);

    // GET /api/WireManagement/compatibility/{productId}
    [HttpGet("compatibility/{productId}")]
    public async Task<IActionResult> GetWireCompatibility(string productId);

    // POST /api/WireManagement/validate
    // 验证线材与产品兼容性
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateWireCompatibility([FromBody] WireCompatibilityCheckRequest request);
}
```

### 5.5 业务规则

| 规则编号 | 规则名称 | 规则描述 | 触发动作 |
|---------|---------|---------|---------|
| WM-01 | 兼容性校验 | 批次进站时校验线材是否在product_wire_compatibility中 | 不兼容则拦截进站 |
| WM-02 | 强制线材校验 | is_mandatory=true的产品只能用指定线材 | 强制拦截 |
| WM-03 | 换线首件验证 | 线材切换后首件必须验证通过 | 未验证前不允许批量生产 |
| WM-04 | 线材追溯 | 每个批次的线材使用记录不可删除 | 永久追溯 |

### 5.6 UI设计建议

**文件**: `src/Client/MES.Modules.Execute/Views/TrackInView.xaml`（在参数面板中增加线材选择区）

```xml
<!-- 在TrackInView参数Tab前增加"线材"Tab -->
<TabItem Header="线材管理">
    <StackPanel Margin="12">
        <!-- 当前线材信息 -->
        <GroupBox Header="当前线材" Margin="0,0,0,12">
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
                <TextBlock Grid.Row="0" Text="线材类型:" Margin="0,4"/>
                <ComboBox Grid.Row="0" Grid.Column="1" Width="200"
                          ItemsSource="{Binding CompatibleWireTypes}"
                          SelectedItem="{Binding SelectedWireType}"
                          materialDesign:HintAssist.Hint="选择线材类型"/>
                <TextBlock Grid.Row="1" Text="线材批次号:" Margin="0,4"/>
                <TextBox Grid.Row="1" Grid.Column="1" Width="200"
                         Text="{Binding WireLotNo}"
                         materialDesign:HintAssist.Hint="扫描/输入批次号"/>
                <TextBlock Grid.Row="2" Text="卷轴ID:" Margin="0,4"/>
                <TextBox Grid.Row="2" Grid.Column="1" Width="200"
                         Text="{Binding WireReelId}"
                         materialDesign:HintAssist.Hint="扫描卷轴条码"/>
            </Grid>
        </GroupBox>

        <!-- 换线按钮 -->
        <Button Content="切换线材" Command="{Binding WireChangeCommand}"
                Style="{StaticResource WarningButton}" Margin="0,0,0,12"/>

        <!-- 换线历史记录 -->
        <TextBlock Text="近期换线记录" FontWeight="SemiBold" Margin="0,8"/>
        <DataGrid ItemsSource="{Binding RecentWireChanges}" Height="150"
                  AutoGenerateColumns="False" IsReadOnly="True">
            <DataGrid.Columns>
                <DataGridTextColumn Header="切换时间" Binding="{Binding ChangedAt}" Width="140"/>
                <DataGridTextColumn Header="原线材" Binding="{Binding OldWireType}" Width="100"/>
                <DataGridTextColumn Header="新线材" Binding="{Binding NewWireType}" Width="100"/>
                <DataGridTextColumn Header="操作人" Binding="{Binding ChangeBy}" Width="80"/>
                <DataGridTextColumn Header="首件验证" Binding="{Binding FirstArticleResult}" Width="80"/>
            </DataGrid.Columns>
        </DataGrid>
    </StackPanel>
</TabItem>
```

---

## 六、模具/刀片/劈刀寿命管理模块（新增）

### 6.1 现状分析

`MasterEquipment` 表有 `RunningHours`、`MaintenanceIntervalHours` 字段，但无工具级别的寿命管理。对于打线机（Wire Bonder）的劈刀（Capillary）、模具（Mold）、刀片（Blade）等消耗品：
- ❌ 无工具定义表
- ❌ 无工具使用计数
- ❌ 无寿命预警机制
- ❌ 无到期锁定功能

### 6.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 描述 |
|---------|---------|--------|------|
| TL-001 | 工具定义管理 | P0 | 定义工具类型、寿命上限、预警阈值 |
| TL-002 | 工具使用记录 | P0 | 每次使用记录打线次数/模压次数/切割次数 |
| TL-003 | 寿命预警 | P0 | 达到预警阈值时提示更换 |
| TL-004 | 到期锁定 | P0 | 超过寿命上限时锁定设备/工序 |
| TL-005 | 工具更换记录 | P1 | 记录更换时间、旧工具状态、新工具信息 |
| TL-006 | 工具与批次关联 | P1 | 每批次记录使用的工具编号 |

### 6.3 数据表设计

#### 6.3.1 tool_definition（工具定义）

```sql
CREATE TABLE tool_definition (
    tool_type_id        VARCHAR(50)     NOT NULL COMMENT '工具类型ID',
    tool_type_name      VARCHAR(100)    NOT NULL COMMENT '名称: 劈刀/模具/刀片',
    tool_category       VARCHAR(50)     NOT NULL COMMENT '分类: Capillary/Mold/Blade',
    applicable_equipment VARCHAR(200)   NULL     COMMENT '适用设备组，逗号分隔',
    applicable_step     VARCHAR(50)     NULL     COMMENT '适用工序',
    life_unit           VARCHAR(20)     NOT NULL COMMENT '寿命单位: Times/Hours/Cycles',
    max_life            INT             NOT NULL COMMENT '寿命上限',
    warning_threshold   DECIMAL(5,2)    NOT NULL DEFAULT 80.00 COMMENT '预警阈值(%)',
    is_active           TINYINT(1)      NOT NULL DEFAULT 1,
    created_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (tool_type_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工具类型定义';

INSERT INTO tool_definition (tool_type_id, tool_type_name, tool_category, applicable_equipment, applicable_step, life_unit, max_life, warning_threshold) VALUES
('TT-CAP-STD', '标准劈刀', 'Capillary', 'WB-01,WB-02', 'WireBond', 'Times', 500000, 80.00),
('TT-CAP-FINE', '精细劈刀', 'Capillary', 'WB-03', 'FineWireBond', 'Times', 300000, 80.00),
('TT-MOLD-STD', '标准模具', 'Mold', 'MD-01,MD-02', 'Molding', 'Cycles', 10000, 85.00),
('TT-BLADE-STD', '标准刀片', 'Blade', 'SA-01', 'Sawing', 'Times', 5000, 90.00),
('TT-BLADE-DIC', 'DIC刀片', 'Blade', 'SA-02', 'Dicing', 'Times', 3000, 85.00);
```

#### 6.3.2 tool_instance（工具实例）

```sql
CREATE TABLE tool_instance (
    tool_id             VARCHAR(50)     NOT NULL COMMENT '工具实例ID，如CAP-2024-001',
    tool_type_id        VARCHAR(50)     NOT NULL COMMENT '工具类型',
    tool_serial_no      VARCHAR(50)     NOT NULL COMMENT '序列号',
    manufacturer        VARCHAR(100)    NULL     COMMENT '制造商',
    current_equipment   VARCHAR(50)     NULL     COMMENT '当前安装设备',
    installed_at        DATETIME        NULL     COMMENT '安装时间',
    used_count          INT             NOT NULL DEFAULT 0 COMMENT '已使用次数',
    life_percent        DECIMAL(5,2)    NOT NULL DEFAULT 100.00 COMMENT '剩余寿命百分比',
    status              VARCHAR(20)     NOT NULL DEFAULT 'Available' COMMENT '状态: Available/InUse/Warning/Expired/Retired',
    last_used_at        DATETIME        NULL     COMMENT '最后使用时间',
    created_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (tool_id),
    UNIQUE KEY uk_tool_serial (tool_type_id, tool_serial_no),
    KEY idx_status (status),
    KEY idx_equipment (current_equipment),
    KEY idx_life_percent (life_percent)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工具实例';
```

#### 6.3.3 tool_usage_log（工具使用日志）

```sql
CREATE TABLE tool_usage_log (
    log_id              BIGINT          NOT NULL AUTO_INCREMENT,
    tool_id             VARCHAR(50)     NOT NULL COMMENT '工具ID',
    lot_id              VARCHAR(50)     NULL     COMMENT '批次ID',
    step_code           VARCHAR(50)     NULL     COMMENT '工序代码',
    equipment_id        VARCHAR(50)     NOT NULL COMMENT '设备ID',
    usage_count         INT             NOT NULL COMMENT '本次使用次数',
    cumulative_count    INT             NOT NULL COMMENT '累计使用次数',
    operator_id         VARCHAR(50)     NULL     COMMENT '操作员',
    used_at             DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (log_id),
    KEY idx_tool (tool_id),
    KEY idx_lot (lot_id),
    KEY idx_used_at (used_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工具使用日志';
```

#### 6.3.4 tool_change_record（工具更换记录）

```sql
CREATE TABLE tool_change_record (
    change_id           BIGINT          NOT NULL AUTO_INCREMENT,
    equipment_id        VARCHAR(50)     NOT NULL,
    old_tool_id         VARCHAR(50)     NULL     COMMENT '旧工具',
    old_tool_used_count INT             NULL     COMMENT '旧工具最终使用次数',
    old_tool_status     VARCHAR(20)     NULL     COMMENT '旧工具退役原因',
    new_tool_id         VARCHAR(50)     NOT NULL COMMENT '新工具',
    change_reason       VARCHAR(255)    NULL,
    change_by           VARCHAR(50)     NOT NULL,
    changed_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    first_article_verified TINYINT(1)   NOT NULL DEFAULT 0,
    PRIMARY KEY (change_id),
    KEY idx_equipment (equipment_id),
    KEY idx_changed_at (changed_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工具更换记录';
```

### 6.4 接口设计

#### ToolManagementController

**文件**: `src/Server/MES.Api/Controllers/ToolManagementController.cs`（新建）

```csharp
[ApiController]
[Route("api/[controller]")]
public class ToolManagementController : ControllerBase
{
    private readonly IToolManagementService _service;

    // GET /api/ToolManagement/types
    [HttpGet("types")]
    public async Task<IActionResult> GetToolTypes([FromQuery] string? category);

    // POST /api/ToolManagement/instances
    [HttpPost("instances")]
    public async Task<IActionResult> CreateToolInstance([FromBody] CreateToolInstanceRequest request);

    // GET /api/ToolManagement/instances
    [HttpGet("instances")]
    public async Task<IActionResult> GetToolInstances([FromQuery] string? toolTypeId, [FromQuery] string? status, [FromQuery] string? equipmentId);

    // POST /api/ToolManagement/usage
    [HttpPost("usage")]
    public async Task<IActionResult> RecordToolUsage([FromBody] ToolUsageRequest request);

    // POST /api/ToolManagement/change
    [HttpPost("change")]
    public async Task<IActionResult> ChangeTool([FromBody] ToolChangeRequest request);

    // GET /api/ToolManagement/warnings
    // 获取寿命预警列表
    [HttpGet("warnings")]
    public async Task<IActionResult> GetToolWarnings();

    // POST /api/ToolManagement/validate
    // 验证工具是否可用（未过期、未锁定）
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateTool([FromBody] ToolValidationRequest request);
}
```

### 6.5 业务规则

| 规则编号 | 规则名称 | 规则描述 | 触发动作 |
|---------|---------|---------|---------|
| TL-01 | 寿命计算 | life_percent = (max_life - used_count) / max_life × 100 | 每次使用后更新 |
| TL-02 | 预警触发 | life_percent ≤ warning_threshold → 状态变为Warning | 弹出预警通知 |
| TL-03 | 到期锁定 | used_count ≥ max_life → 状态变为Expired | 锁定设备，不允许继续生产 |
| TL-04 | 换线首件验证 | 更换工具后必须做首件验证 | 验证通过前不允许量产 |
| TL-05 | 批次工具关联 | 每批次记录使用工具ID | 永久追溯 |

### 6.6 UI设计建议

**新建文件**: `src/Client/MES.Modules.Equipment/Views/ToolManagementView.xaml`

```xml
<!-- 工具寿命管理界面 -->
<UserControl x:Class="MES.Modules.Equipment.Views.ToolManagementView">
    <Grid Margin="24">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>  <!-- Tab选择器 -->
            <RowDefinition Height="*"/>     <!-- 主体 -->
        </Grid.RowDefinitions>

        <TabControl>
            <!-- Tab 1: 工具概览 -->
            <TabItem Header="工具概览">
                <Grid>
                    <!-- 顶部统计: 在用/预警/过期/待更换 -->
                    <UniformGrid Rows="1" Columns="4" Margin="0,0,0,16">
                        <Border Style="{StaticResource StatCard}" Background="LightGreen">
                            <StackPanel>
                                <TextBlock Text="{Binding InUseCount}" FontSize="28" FontWeight="Bold"/>
                                <TextBlock Text="在用工具"/>
                            </StackPanel>
                        </Border>
                        <Border Style="{StaticResource StatCard}" Background="Orange">
                            <StackPanel>
                                <TextBlock Text="{Binding WarningCount}" FontSize="28" FontWeight="Bold" Foreground="DarkOrange"/>
                                <TextBlock Text="寿命预警"/>
                            </StackPanel>
                        </Border>
                        <Border Style="{StaticResource StatCard}" Background="LightCoral">
                            <StackPanel>
                                <TextBlock Text="{Binding ExpiredCount}" FontSize="28" FontWeight="Bold" Foreground="Red"/>
                                <TextBlock Text="已过期"/>
                            </StackPanel>
                        </Border>
                    </UniformGrid>

                    <!-- 工具列表（含寿命进度条） -->
                    <DataGrid ItemsSource="{Binding ToolInstances}" AutoGenerateColumns="False" Margin="0,80,0,0">
                        <DataGrid.Columns>
                            <DataGridTextColumn Header="工具ID" Binding="{Binding ToolId}" Width="120"/>
                            <DataGridTextColumn Header="类型" Binding="{Binding ToolTypeName}" Width="100"/>
                            <DataGridTextColumn Header="序列号" Binding="{Binding SerialNo}" Width="100"/>
                            <DataGridTextColumn Header="当前设备" Binding="{Binding EquipmentId}" Width="100"/>
                            <!-- 寿命进度条 -->
                            <DataGridTemplateColumn Header="剩余寿命" Width="200">
                                <DataGridTemplateColumn.CellTemplate>
                                    <DataTemplate>
                                        <Grid>
                                            <ProgressBar Value="{Binding LifePercent}" Maximum="100"
                                                         Foreground="{Binding LifePercent, Converter={StaticResource LifePercentToColorConverter}}"/>
                                            <TextBlock Text="{Binding LifePercent, StringFormat={}{0:F1}%}"
                                                       HorizontalAlignment="Center" VerticalAlignment="Center"
                                                       FontWeight="Bold"/>
                                        </Grid>
                                    </DataTemplate>
                                </DataGridTemplateColumn.CellTemplate>
                            </DataGridTemplateColumn>
                            <DataGridTextColumn Header="已用次数" Binding="{Binding UsedCount}" Width="80"/>
                            <DataGridTextColumn Header="状态" Binding="{Binding Status}" Width="80"/>
                            <DataGridTemplateColumn Header="操作" Width="80">
                                <DataGridTemplateColumn.CellTemplate>
                                    <DataTemplate>
                                        <Button Content="更换" Command="{Binding DataContext.ChangeToolCommand, RelativeSource={RelativeSource AncestorType=DataGrid}}"
                                                CommandParameter="{Binding}" Visibility="{Binding IsNearExpiry, Converter={StaticResource BooleanToVisibilityConverter}}"/>
                                    </DataTemplate>
                                </DataGridTemplateColumn.CellTemplate>
                            </DataGridTemplateColumn>
                        </DataGrid.Columns>
                    </DataGrid>
                </Grid>
            </TabItem>

            <!-- Tab 2: 更换记录 -->
            <TabItem Header="更换记录">
                <DataGrid ItemsSource="{Binding ChangeRecords}" AutoGenerateColumns="False"/>
            </TabItem>

            <!-- Tab 3: 使用日志 -->
            <TabItem Header="使用日志">
                <DataGrid ItemsSource="{Binding UsageLogs}" AutoGenerateColumns="False"/>
            </TabItem>
        </TabControl>
    </Grid>
</UserControl>
```

---

## 七、操作员资质管理模块（新增）

### 7.1 现状分析

`SysUser` 表有 `RoleId`、`DeptId`、`Shift` 等字段，但无工序资质绑定。当前 `ProcessExecutionService.StartStepAsync` 仅验证用户存在和IsActive：
- ❌ 无工序资质定义
- ❌ 无资质有效期管理
- ❌ 无资质校验逻辑
- ❌ 无到期预警

### 7.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 描述 |
|---------|---------|--------|------|
| OQ-001 | 资质类型定义 | P0 | 定义资质类型（如焊线资质、模具操作资质） |
| OQ-002 | 资质-工序绑定 | P0 | 哪些工序需要哪些资质 |
| OQ-003 | 人员资质授予 | P0 | 给操作员授予资质，含有效期 |
| OQ-004 | 资质校验拦截 | P0 | TrackIn/StartStep时校验操作员资质 |
| OQ-005 | 到期预警 | P1 | 资质即将到期时预警 |
| OQ-006 | 到期自动失效 | P1 | 过期资质自动失效 |
| OQ-007 | 资质培训记录 | P2 | 记录培训、考核、发证信息 |

### 7.3 数据表设计

#### 7.3.1 operator_qualification_type（资质类型）

```sql
CREATE TABLE operator_qualification_type (
    qual_type_id        VARCHAR(50)     NOT NULL,
    qual_type_name      VARCHAR(100)    NOT NULL COMMENT '资质名称',
    qual_category       VARCHAR(50)     NULL     COMMENT '分类: Equipment/Process/Safety',
    valid_days          INT             NOT NULL DEFAULT 365 COMMENT '有效期(天)',
    requires_training   TINYINT(1)      NOT NULL DEFAULT 1 COMMENT '是否需要培训',
    requires_exam       TINYINT(1)      NOT NULL DEFAULT 1 COMMENT '是否需要考试',
    exam_pass_score     DECIMAL(5,2)    NULL     COMMENT '考试合格分数',
    is_active           TINYINT(1)      NOT NULL DEFAULT 1,
    created_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (qual_type_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='资质类型';

INSERT INTO operator_qualification_type (qual_type_id, qual_type_name, qual_category, valid_days, requires_training, requires_exam, exam_pass_score) VALUES
('QT-WB', '焊线操作资质', 'Equipment', 365, 1, 1, 80.00),
('QT-MD', '模具操作资质', 'Equipment', 365, 1, 1, 80.00),
('QT-SA', '切割操作资质', 'Equipment', 365, 1, 1, 80.00),
('QT-QC', '质量检验资质', 'Process', 730, 1, 1, 85.00),
('QT-SAFE', '安全操作资质', 'Safety', 365, 1, 0, NULL);
```

#### 7.3.2 operator_qualification（人员资质）

```sql
CREATE TABLE operator_qualification (
    qual_id             BIGINT          NOT NULL AUTO_INCREMENT,
    employee_id         VARCHAR(50)     NOT NULL COMMENT '员工ID(=user_id)',
    qual_type_id        VARCHAR(50)     NOT NULL COMMENT '资质类型',
    qual_level          VARCHAR(20)     NULL     COMMENT '等级: Junior/Senior/Expert',
    certificate_no      VARCHAR(50)     NULL     COMMENT '证书编号',
    issue_date          DATE            NOT NULL COMMENT '发证日期',
    expiry_date         DATE            NOT NULL COMMENT '到期日期',
    issued_by           VARCHAR(50)     NULL     COMMENT '发证机构/人',
    training_hours      DECIMAL(5,1)    NULL     COMMENT '培训学时',
    exam_score          DECIMAL(5,2)    NULL     COMMENT '考试成绩',
    status              VARCHAR(20)     NOT NULL DEFAULT 'Active' COMMENT '状态: Active/Expired/Revoked',
    revoked_at          DATETIME        NULL     COMMENT '撤销时间',
    revoked_reason      VARCHAR(255)    NULL     COMMENT '撤销原因',
    created_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (qual_id),
    UNIQUE KEY uk_employee_qual (employee_id, qual_type_id),
    KEY idx_employee (employee_id),
    KEY idx_expiry (expiry_date),
    KEY idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='人员资质';
```

#### 7.3.3 qualification_step_mapping（资质-工序映射）

```sql
CREATE TABLE qualification_step_mapping (
    mapping_id          BIGINT          NOT NULL AUTO_INCREMENT,
    step_code           VARCHAR(50)     NOT NULL COMMENT '工序代码',
    qual_type_id        VARCHAR(50)     NOT NULL COMMENT '资质类型',
    min_level           VARCHAR(20)     NULL     COMMENT '最低等级要求',
    is_mandatory        TINYINT(1)      NOT NULL DEFAULT 1 COMMENT '是否强制',
    created_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (mapping_id),
    UNIQUE KEY uk_step_qual (step_code, qual_type_id),
    KEY idx_step (step_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='资质-工序映射';

-- 示例数据
INSERT INTO qualification_step_mapping (step_code, qual_type_id, min_level, is_mandatory) VALUES
('WireBond', 'QT-WB', NULL, 1),
('Molding', 'QT-MD', NULL, 1),
('Sawing', 'QT-SA', NULL, 1),
('Inspection', 'QT-QC', 'Junior', 1);
```

### 7.4 接口设计

#### OperatorQualificationController

**文件**: `src/Server/MES.Api/Controllers/OperatorQualificationController.cs`（新建）

```csharp
[ApiController]
[Route("api/[controller]")]
public class OperatorQualificationController : ControllerBase
{
    private readonly IOperatorQualificationService _service;

    // GET /api/OperatorQualification/types
    [HttpGet("types")]
    public async Task<IActionResult> GetQualificationTypes();

    // GET /api/OperatorQualification/employee/{employeeId}
    // 获取员工所有资质
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetEmployeeQualifications(string employeeId);

    // POST /api/OperatorQualification/grant
    // 授予资质
    [HttpPost("grant")]
    public async Task<IActionResult> GrantQualification([FromBody] GrantQualificationRequest request);

    // PUT /api/OperatorQualification/revoke/{qualId}
    // 撤销资质
    [HttpPut("revoke/{qualId}")]
    public async Task<IActionResult> RevokeQualification(long qualId, [FromBody] RevokeQualificationRequest request);

    // POST /api/OperatorQualification/validate
    // 校验资质（TrackIn时调用）
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateQualification([FromBody] QualificationValidateRequest request);

    // GET /api/OperatorQualification/expiring
    // 获取即将到期的资质
    [HttpGet("expiring")]
    public async Task<IActionResult> GetExpiringQualifications([FromQuery] int days = 30);

    // GET /api/OperatorQualification/step-mapping
    // 获取工序资质要求
    [HttpGet("step-mapping")]
    public async Task<IActionResult> GetStepMappings([FromQuery] string? stepCode);
}
```

#### ProcessExecutionService 扩展

在 `StartStepAsync` 中增加资质校验：

```csharp
// 在 ProcessExecutionService.StartStepAsync 中，步骤5之后增加：

// 5.5 验证操作员资质
var qualificationCheck = await _qualificationService.ValidateQualificationAsync(
    request.OperatorId, request.StepCode);

if (!qualificationCheck.IsQualified)
{
    return Fail(operationId,
        $"操作员 {request.OperatorId} 不具备工序 {request.StepCode} 的操作资质。" +
        $"缺失资质: {string.Join(", ", qualificationCheck.MissingQualifications)}");
}
```

### 7.5 业务规则

| 规则编号 | 规则名称 | 规则描述 | 触发动作 |
|---------|---------|---------|---------|
| OQ-01 | 资质校验 | TrackIn时校验操作员是否具备该工序所需资质 | 无资质则拦截 |
| OQ-02 | 最低等级校验 | 某些工序要求最低资质等级 | 等级不足则拦截 |
| OQ-03 | 到期预警 | 资质距到期≤30天时预警 | 通知员工和主管 |
| OQ-04 | 自动失效 | expiry_date < today → status = Expired | 定时任务每日执行 |
| OQ-05 | 强制资质 | is_mandatory=true的资质缺失不允许进站 | 拦截 |
| OQ-06 | 资质追溯 | 每批次记录操作员资质状态 | 永久追溯 |

### 7.6 UI设计建议

**新建文件**: `src/Client/MES.Modules.SystemSettings/Views/QualificationManagementView.xaml`

```xml
<!-- 资质管理界面 -->
<UserControl x:Class="MES.Modules.SystemSettings.Views.QualificationManagementView">
    <Grid Margin="24">
        <TabControl>
            <!-- Tab 1: 资质类型管理 -->
            <TabItem Header="资质类型">
                <DataGrid ItemsSource="{Binding QualificationTypes}" AutoGenerateColumns="False">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="资质代码" Binding="{Binding QualTypeId}" Width="100"/>
                        <DataGridTextColumn Header="名称" Binding="{Binding QualTypeName}" Width="120"/>
                        <DataGridTextColumn Header="分类" Binding="{Binding QualCategory}" Width="80"/>
                        <DataGridTextColumn Header="有效期(天)" Binding="{Binding ValidDays}" Width="80"/>
                        <DataGridCheckBoxColumn Header="需培训" Binding="{Binding RequiresTraining}" Width="60"/>
                        <DataGridCheckBoxColumn Header="需考试" Binding="{Binding RequiresExam}" Width="60"/>
                        <DataGridTextColumn Header="合格分数" Binding="{Binding ExamPassScore}" Width="80"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>

            <!-- Tab 2: 人员资质 -->
            <TabItem Header="人员资质">
                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="250"/>  <!-- 左侧: 员工列表 -->
                        <ColumnDefinition Width="*"/>     <!-- 右侧: 资质详情 -->
                    </Grid.ColumnDefinitions>

                    <ListView ItemsSource="{Binding Employees}" SelectedItem="{Binding SelectedEmployee}">
                        <ListView.ItemTemplate>
                            <DataTemplate>
                                <StackPanel Orientation="Horizontal">
                                    <TextBlock Text="{Binding EmployeeName}" FontWeight="SemiBold"/>
                                    <TextBlock Text=" (" Foreground="Gray"/>
                                    <TextBlock Text="{Binding EmployeeId}" Foreground="Gray"/>
                                    <TextBlock Text=")" Foreground="Gray"/>
                                </StackPanel>
                            </DataTemplate>
                        </ListView.ItemTemplate>
                    </ListView>

                    <!-- 右侧: 选中员工的资质列表 -->
                    <DataGrid Grid.Column="1" ItemsSource="{Binding EmployeeQualifications}"
                              AutoGenerateColumns="False">
                        <DataGrid.Columns>
                            <DataGridTextColumn Header="资质类型" Binding="{Binding QualTypeName}" Width="120"/>
                            <DataGridTextColumn Header="等级" Binding="{Binding QualLevel}" Width="80"/>
                            <DataGridTextColumn Header="证书编号" Binding="{Binding CertificateNo}" Width="100"/>
                            <DataGridTextColumn Header="发证日期" Binding="{Binding IssueDate, StringFormat=yyyy-MM-dd}" Width="100"/>
                            <DataGridTextColumn Header="到期日期" Binding="{Binding ExpiryDate, StringFormat=yyyy-MM-dd}" Width="100"/>
                            <!-- 剩余天数 -->
                            <DataGridTemplateColumn Header="剩余天数" Width="80">
                                <DataGridTemplateColumn.CellTemplate>
                                    <DataTemplate>
                                        <TextBlock Text="{Binding RemainingDays}"
                                                   Foreground="{Binding RemainingDays, Converter={StaticResource DaysToColorConverter}}"
                                                   FontWeight="Bold"/>
                                    </DataTemplate>
                                </DataGridTemplateColumn.CellTemplate>
                            </DataGridTemplateColumn>
                            <DataGridTextColumn Header="状态" Binding="{Binding Status}" Width="80"/>
                        </DataGrid.Columns>
                    </DataGrid>
                </Grid>
            </TabItem>

            <!-- Tab 3: 工序资质要求 -->
            <TabItem Header="工序资质要求">
                <DataGrid ItemsSource="{Binding StepMappings}" AutoGenerateColumns="False">
                    <DataGrid.Columns>
                        <DataGridTextColumn Header="工序代码" Binding="{Binding StepCode}" Width="120"/>
                        <DataGridTextColumn Header="工序名称" Binding="{Binding StepName}" Width="120"/>
                        <DataGridTextColumn Header="资质类型" Binding="{Binding QualTypeName}" Width="120"/>
                        <DataGridTextColumn Header="最低等级" Binding="{Binding MinLevel}" Width="80"/>
                        <DataGridCheckBoxColumn Header="强制" Binding="{Binding IsMandatory}" Width="60"/>
                    </DataGrid.Columns>
                </DataGrid>
            </TabItem>
        </TabControl>
    </Grid>
</UserControl>
```

---

## 八、首件检验深化模块（后端补齐）

### 8.1 现状分析

`FirstArticleInspectionView.xaml` 仅有占位文本，无任何后端支持。`QualityInspection` 和 `QualityInspectionItem` 实体已存在于 MesDbContext，可以作为首件检验的基础。但：
- ❌ 无首件检验专用模板
- ❌ 无首件检验专用Service/Controller
- ❌ 无首件与工序放行的联动
- ❌ 首件判定结果不影响后续生产

### 8.2 功能清单

| 功能编号 | 功能名称 | 优先级 | 描述 |
|---------|---------|--------|------|
| FA-001 | 首件模板管理 | P0 | 按工序定义首件检验项目模板 |
| FA-002 | 首件检验录入 | P0 | 按模板录入检验结果 |
| FA-003 | 首件判定 | P0 | 自动判定首件结果(Pass/Fail/Pending) |
| FA-004 | 首件审批流 | P1 | 首件通过需审批 |
| FA-005 | 首件放行联动 | P0 | 首件未通过不允许批量生产 |
| FA-006 | 首件历史记录 | P1 | 历史首件数据查询对比 |

### 8.3 数据表设计

#### 8.3.1 first_article_template（首件检验模板）

```sql
CREATE TABLE first_article_template (
    template_id         VARCHAR(50)     NOT NULL,
    template_name       VARCHAR(100)    NOT NULL,
    step_code           VARCHAR(50)     NOT NULL COMMENT '关联工序',
    product_id          VARCHAR(50)     NULL     COMMENT '关联产品，NULL表示通用',
    is_active           TINYINT(1)      NOT NULL DEFAULT 1,
    requires_approval   TINYINT(1)      NOT NULL DEFAULT 1 COMMENT '是否需要审批',
    approval_level      VARCHAR(20)     NULL     COMMENT '审批级别',
    created_by          VARCHAR(50)     NOT NULL,
    created_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (template_id),
    UNIQUE KEY uk_template_step (step_code, product_id),
    KEY idx_step (step_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='首件检验模板';
```

#### 8.3.2 first_article_template_item（首件检验模板明细）

```sql
CREATE TABLE first_article_template_item (
    item_id             BIGINT          NOT NULL AUTO_INCREMENT,
    template_id         VARCHAR(50)     NOT NULL,
    item_code           VARCHAR(50)     NOT NULL COMMENT '检验项目代码',
    item_name           VARCHAR(100)    NOT NULL COMMENT '检验项目名称',
    inspection_type     VARCHAR(20)     NOT NULL COMMENT '类型: Visual/Dimensional/Functional/Electrical',
    specification       VARCHAR(255)    NULL     COMMENT '规格描述',
    usl                 DECIMAL(10,4)   NULL,
    lsl                 DECIMAL(10,4)   NULL,
    target_value        DECIMAL(10,4)   NULL,
    unit                VARCHAR(20)     NULL,
    sampling_qty        INT             NOT NULL DEFAULT 1 COMMENT '抽样数量',
    is_critical         TINYINT(1)      NOT NULL DEFAULT 0 COMMENT '是否关键项',
    display_order       INT             NOT NULL DEFAULT 0,
    PRIMARY KEY (item_id),
    UNIQUE KEY uk_template_item (template_id, item_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='首件检验模板明细';
```

#### 8.3.3 first_article_inspection（首件检验记录）

```sql
CREATE TABLE first_article_inspection (
    inspection_id       VARCHAR(50)     NOT NULL,
    template_id         VARCHAR(50)     NOT NULL,
    lot_id              VARCHAR(50)     NOT NULL,
    step_code           VARCHAR(50)     NOT NULL,
    equipment_id        VARCHAR(50)     NOT NULL,
    inspector_id        VARCHAR(50)     NOT NULL,
    inspection_time     DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    overall_result      VARCHAR(20)     NOT NULL DEFAULT 'Pending' COMMENT '综合结果: Pass/Fail/Pending',
    fail_items          VARCHAR(500)    NULL     COMMENT '不合格项目代码，逗号分隔',
    approval_status     VARCHAR(20)     NULL     COMMENT '审批状态: Pending/Approved/Rejected',
    approved_by         VARCHAR(50)     NULL,
    approved_at         DATETIME        NULL,
    approval_comment    VARCHAR(255)    NULL,
    remark              VARCHAR(255)    NULL,
    created_at          DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (inspection_id),
    KEY idx_lot (lot_id),
    KEY idx_step (step_code),
    KEY idx_result (overall_result),
    KEY idx_inspection_time (inspection_time)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='首件检验记录';
```

#### 8.3.4 first_article_result（首件检验结果明细）

```sql
CREATE TABLE first_article_result (
    result_id           BIGINT          NOT NULL AUTO_INCREMENT,
    inspection_id       VARCHAR(50)     NOT NULL,
    item_code           VARCHAR(50)     NOT NULL,
    item_name           VARCHAR(100)    NOT NULL,
    specification       VARCHAR(255)    NULL,
    usl                 DECIMAL(10,4)   NULL,
    lsl                 DECIMAL(10,4)   NULL,
    measured_value      DECIMAL(10,4)   NULL,
    unit                VARCHAR(20)     NULL,
    result              VARCHAR(20)     NOT NULL COMMENT 'OK/NG',
    inspector_comment   VARCHAR(255)    NULL,
    PRIMARY KEY (result_id),
    UNIQUE KEY uk_inspection_item (inspection_id, item_code),
    KEY idx_result (result)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='首件检验结果明细';
```

### 8.4 接口设计

#### FirstArticleInspectionController

**文件**: `src/Server/MES.Api/Controllers/FirstArticleInspectionController.cs`（新建）

```csharp
[ApiController]
[Route("api/[controller]")]
public class FirstArticleInspectionController : ControllerBase
{
    private readonly IFirstArticleInspectionService _service;

    // GET /api/FirstArticleInspection/templates
    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplates([FromQuery] string? stepCode, [FromQuery] string? productId);

    // GET /api/FirstArticleInspection/template/{templateId}
    [HttpGet("template/{templateId}")]
    public async Task<IActionResult> GetTemplateDetail(string templateId);

    // POST /api/FirstArticleInspection
    // 创建首件检验记录
    [HttpPost]
    public async Task<IActionResult> CreateInspection([FromBody] CreateFirstArticleInspectionRequest request);

    // PUT /api/FirstArticleInspection/{inspectionId}/result
    // 录入检验结果
    [HttpPut("{inspectionId}/result")]
    public async Task<IActionResult> SubmitResults(string inspectionId, [FromBody] SubmitFirstArticleResultsRequest request);

    // POST /api/FirstArticleInspection/{inspectionId}/approve
    // 审批首件
    [HttpPost("{inspectionId}/approve")]
    public async Task<IActionResult> ApproveInspection(string inspectionId, [FromBody] ApproveFirstArticleRequest request);

    // GET /api/FirstArticleInspection/lot/{lotId}
    // 获取批次首件检验记录
    [HttpGet("lot/{lotId}")]
    public async Task<IActionResult> GetLotInspections(string lotId);

    // GET /api/FirstArticleInspection/{inspectionId}
    // 获取首件详情
    [HttpGet("{inspectionId}")]
    public async Task<IActionResult> GetInspectionDetail(string inspectionId);

    // POST /api/FirstArticleInspection/check-release
    // 检查首件是否已放行（批量生产前调用）
    [HttpPost("check-release")]
    public async Task<IActionResult> CheckRelease([FromBody] FirstArticleReleaseCheckRequest request);
}
```

#### FirstArticleInspectionService 新增方法

**文件**: `src/Server/MES.Services.Quality/FirstArticleInspectionService.cs`（新建）

```csharp
public interface IFirstArticleInspectionService
{
    Task<FirstArticleTemplateDto> GetTemplateAsync(string stepCode, string? productId);
    Task<string> CreateInspectionAsync(CreateFirstArticleInspectionRequest request);
    Task<FirstArticleInspectionResult> SubmitResultsAsync(string inspectionId, SubmitFirstArticleResultsRequest request);
    Task<bool> ApproveInspectionAsync(string inspectionId, string approverId, string comment);
    Task<List<FirstArticleInspectionDto>> GetLotInspectionsAsync(string lotId);
    Task<FirstArticleReleaseCheckResult> CheckReleaseAsync(string lotId, string stepCode);
}
```

### 8.5 业务规则

| 规则编号 | 规则名称 | 规则描述 | 触发动作 |
|---------|---------|---------|---------|
| FA-01 | 首件强制检验 | requires_approval=true的工序，首件未通过前不允许批量生产 | 拦截后续批次出站 |
| FA-02 | 自动判定 | 所有检验项result=OK → overall_result=Pass | 任何一项NG → Fail |
| FA-03 | 关键项一票否决 | is_critical=true的项目NG → 整单Fail | 不允许审批通过 |
| FA-04 | 审批流 | requires_approval=true → 需审批后才能放行 | 审批通过后解锁批量生产 |
| FA-05 | 首件追溯 | 首件记录永久关联批次 | 永久追溯 |

### 8.6 UI改造建议

**文件**: `src/Client/MES.Modules.Quality/Views/FirstArticleInspectionView.xaml`

```xml
<!-- 首件检验完整界面 -->
<UserControl x:Class="MES.Modules.Quality.Views.FirstArticleInspectionView">
    <Grid Margin="24">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>  <!-- 顶部信息 -->
            <RowDefinition Height="*"/>     <!-- 主体检验区 -->
            <RowDefinition Height="Auto"/>  <!-- 底部操作 -->
        </Grid.RowDefinitions>

        <!-- 顶部：批次+模板信息 -->
        <StackPanel Margin="0,0,0,16">
            <TextBlock Text="首件检验" FontSize="24" FontWeight="SemiBold" Margin="0,0,0,8"/>
            <StackPanel Orientation="Horizontal" Gap="16">
                <TextBox Width="200" Text="{Binding LotId}"
                         materialDesign:HintAssist.Hint="批次ID"/>
                <ComboBox Width="200" ItemsSource="{Binding TemplateList}"
                          SelectedItem="{Binding SelectedTemplate}"
                          materialDesign:HintAssist.Hint="检验模板"/>
                <ComboBox Width="150" ItemsSource="{Binding EquipmentList}"
                          SelectedItem="{Binding SelectedEquipment}"
                          materialDesign:HintAssist.Hint="设备"/>
                <Button Content="加载模板" Command="{Binding LoadTemplateCommand}"/>
            </StackPanel>
        </StackPanel>

        <!-- 主体：检验项目DataGrid -->
        <DataGrid Grid.Row="1" ItemsSource="{Binding InspectionItems}"
                  AutoGenerateColumns="False" CanUserAddRows="False">
            <DataGrid.Columns>
                <DataGridTextColumn Header="序号" Binding="{Binding DisplayOrder}" Width="50"/>
                <DataGridTextColumn Header="检验项目" Binding="{Binding ItemName}" Width="150"/>
                <DataGridTextColumn Header="检验类型" Binding="{Binding InspectionType}" Width="80"/>
                <DataGridTextColumn Header="规格" Binding="{Binding Specification}" Width="150"/>
                <DataGridTextColumn Header="下限" Binding="{Binding Lsl}" Width="70"/>
                <DataGridTextColumn Header="目标值" Binding="{Binding TargetValue}" Width="70"/>
                <DataGridTextColumn Header="上限" Binding="{Binding Usl}" Width="70"/>
                <DataGridTextColumn Header="单位" Binding="{Binding Unit}" Width="60"/>
                <DataGridTemplateColumn Header="实测值" Width="120">
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <TextBox Text="{Binding MeasuredValue, UpdateSourceTrigger=PropertyChanged}"
                                     LostFocus="OnMeasuredValueChanged"/>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>
                <!-- 判定结果自动计算 -->
                <DataGridTextColumn Header="判定" Binding="{Binding Result}" Width="60">
                    <DataGridTextColumn.ElementStyle>
                        <Style TargetType="TextBlock">
                            <Style.Triggers>
                                <DataTrigger Binding="{Binding Result}" Value="OK">
                                    <Setter Property="Foreground" Value="Green"/>
                                    <Setter Property="FontWeight" Value="Bold"/>
                                </DataTrigger>
                                <DataTrigger Binding="{Binding Result}" Value="NG">
                                    <Setter Property="Foreground" Value="Red"/>
                                    <Setter Property="FontWeight" Value="Bold"/>
                                </DataTrigger>
                            </Style.Triggers>
                        </Style>
                    </DataGridTextColumn.ElementStyle>
                </DataGridTextColumn>
                <DataGridTextColumn Header="备注" Binding="{Binding Comment}" Width="120"/>
                <!-- 关键项标记 -->
                <DataGridTemplateColumn Header="关键" Width="50">
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <TextBlock Text="★" Foreground="Red"
                                       Visibility="{Binding IsCritical, Converter={StaticResource BooleanToVisibilityConverter}}"/>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>
            </DataGrid.Columns>
        </DataGrid>

        <!-- 底部：操作按钮 + 综合结果 -->
        <StackPanel Grid.Row="2" Orientation="Horizontal" HorizontalAlignment="Right"
                    Margin="0,16,0,0" Gap="12">
            <!-- 综合结果Badge -->
            <Border Padding="12,6" CornerRadius="6"
                    Background="{Binding OverallResultColor}">
                <TextBlock Text="{Binding OverallResultText}" FontSize="16" FontWeight="Bold" Foreground="White"/>
            </Border>
            <Button Content="提交检验" Command="{Binding SubmitCommand}"/>
            <Button Content="提交审批" Command="{Binding ApproveCommand}"
                    Visibility="{Binding RequiresApproval, Converter={StaticResource BooleanToVisibilityConverter}}"/>
        </StackPanel>
    </Grid>
</UserControl>
```

---

## 九、现有UI全面优化方案

### 9.1 Execute模块优化汇总

| 视图 | 当前状态 | 优化方向 | 关键改动 |
|------|---------|---------|---------|
| TrackInView | ❌ 占位文本 | 完整工序执行界面 | 批次列表+参数模板+线材选择+资质Badge+超限告警 |
| DispatchView | ❌ 占位文本 | 派工任务看板 | 任务列表+设备分配+资质匹配+优先级调整 |
| WipOverviewView | ❌ 占位文本 | 在制品看板 | 统计卡片+工序流转图+滞留预警 |
| AlarmDashboardView | ❌ 占位文本 | 告警中心 | 实时告警+参数超限+寿命预警+分类筛选 |

### 9.2 Quality模块优化汇总

| 视图 | 当前状态 | 优化方向 | 关键改动 |
|------|---------|---------|---------|
| FirstArticleInspectionView | ❌ 占位文本 | 首件检验全流程 | 模板加载+检验录入+自动判定+审批流 |
| SpcChartView | ⚠️ 基础DataGrid | SPC控制图可视化 | X-bar控制图+趋势分析+Cpk面板+超差点标注 |
| SpcRuleConfigView | ⚠️ 基础DataGrid | 规则配置面板 | Western Electric Rules+灵敏度调整 |
| FdcMonitorView | ⚠️ 基础DataGrid | FDC实时监控 | 实时波形+状态Badge+异常告警 |
| OocEventView | ⚠️ 基础列表 | 失控事件管理 | 关联批次跳转+处置流程 |

### 9.3 Recipe模块优化汇总

| 视图 | 当前状态 | 优化方向 | 关键改动 |
|------|---------|---------|---------|
| RecipeListView | ⚠️ 基础列表 | 配方+参数模板关联 | 线材类型标识+参数模板关联数 |
| RecipeParameterView | ⚠️ 参数展示 | 参数模板编辑器 | Usl/Lsl编辑+超限变色+版本对比 |
| RecipeApprovalView | ⚠️ 审批界面 | 变更对比审批 | 变更前后对比视图+审批意见 |

### 9.4 Lot模块优化汇总

| 视图 | 当前状态 | 优化方向 | 关键改动 |
|------|---------|---------|---------|
| LotDetailView | ❌ 占位文本 | 批次全景视图 | Tab布局(工序/参数/Bin/物料/追溯) |
| LotListView | ⚠️ 基础列表 | 批次列表增强 | 状态筛选+Hold标识+超限标记 |
| LotHoldView | ⚠️ 基础Hold管理 | Hold原因关联 | 参数超限自动Hold标记+关联告警 |

### 9.5 公共UI组件规范

**新增共享组件**：

| 组件名称 | 用途 | 文件路径 |
|---------|------|---------|
| ParameterValidationIndicator | 参数超限指示器(红/黄/绿) | `src/Client/MES.Shell/Controls/ParameterValidationIndicator.cs` |
| QualificationBadge | 资质状态Badge | `src/Client/MES.Shell/Controls/QualificationBadge.cs` |
| LifeProgressBar | 工具寿命进度条 | `src/Client/MES.Shell/Controls/LifeProgressBar.cs` |
| StatusPill | 通用状态标签 | `src/Client/MES.Shell/Controls/StatusPill.cs` |
| StatCard | 统计卡片 | `src/Client/MES.Shell/Controls/StatCard.cs` |

---

## 十、实施顺序与依赖

### 10.1 实施阶段划分

```
阶段3.1 ──► 阶段3.2 ──► 阶段3.3 ──► 阶段3.4 ──► 阶段3.5
(2周)       (2周)       (2周)       (2周)       (2周)
```

| 阶段 | 周期 | 内容 | 依赖 |
|------|------|------|------|
| **3.1 参数管控** | 第1-2周 | 参数模板表+Service+API+TrackInView改造+参数校验 | 无 |
| **3.2 Bin管理** | 第3-4周 | Bin码主数据+Bin结果+Bin趋势+BinAnalysisView | 依赖3.1(TestManagement已有基础) |
| **3.3 金线/工具** | 第5-6周 | 线材管理+工具寿命+ToolManagementView | 依赖3.1(工序执行基础) |
| **3.4 资质+首件** | 第7-8周 | 操作员资质+首件检验+QualificationManagementView | 依赖3.1(工序执行拦截点) |
| **3.5 UI整合** | 第9-10周 | 全部UI整合+公共组件+联调测试 | 依赖3.1-3.4全部完成 |

### 10.2 模块依赖图

```
                     ┌─────────────────┐
                     │   MesDbContext   │
                     │  (已有实体基础)  │
                     └────────┬────────┘
                              │
            ┌─────────────────┼─────────────────┐
            │                 │                 │
     ┌──────▼──────┐   ┌─────▼──────┐   ┌─────▼──────┐
     │ ProcessExec │   │ SpcMeasure │   │ TestManage │
     │ Service     │   │ Service    │   │ Service    │
     └──────┬──────┘   └─────┬──────┘   └─────┬──────┘
            │                │                │
     ┌──────▼────────────────▼────────────────▼──────┐
     │            阶段3.1: 工序参数管控               │
     │  parameter_template + parameter_validation    │
     └──────┬───────────────────────────────────────┘
            │
     ┌──────▼──────┐  ┌──────▼──────┐  ┌──────▼──────┐
     │  3.2 Bin管理 │  │  3.3线材工具 │  │  3.4资质首件 │
     └─────────────┘  └─────────────┘  └─────────────┘
            │               │               │
     ┌──────▼───────────────▼───────────────▼──────┐
     │          阶段3.5: UI整合 + 联调              │
     └─────────────────────────────────────────────┘
```

### 10.3 新增文件清单

| 文件路径 | 类型 | 阶段 |
|---------|------|------|
| `src/Shared/MES.Contracts/Production/ParameterTemplateDto.cs` | DTO | 3.1 |
| `src/Server/MES.Services.Production/ParameterTemplateService.cs` | Service | 3.1 |
| `src/Server/MES.Services.Production/IParameterTemplateService.cs` | Interface | 3.1 |
| `src/Server/MES.Api/Controllers/ParameterTemplateController.cs` | Controller | 3.1 |
| `src/Shared/MES.Contracts/Production/BinManagementDto.cs` | DTO | 3.2 |
| `src/Server/MES.Services.Production/BinManagementService.cs` | Service | 3.2 |
| `src/Server/MES.Services.Production/IBinManagementService.cs` | Interface | 3.2 |
| `src/Server/MES.Api/Controllers/BinManagementController.cs` | Controller | 3.2 |
| `src/Shared/MES.Contracts/Production/WireManagementDto.cs` | DTO | 3.3 |
| `src/Server/MES.Services.Production/WireManagementService.cs` | Service | 3.3 |
| `src/Server/MES.Services.Production/IWireManagementService.cs` | Interface | 3.3 |
| `src/Server/MES.Api/Controllers/WireManagementController.cs` | Controller | 3.3 |
| `src/Shared/MES.Contracts/Equipment/ToolManagementDto.cs` | DTO | 3.3 |
| `src/Server/MES.Services.Equipment/ToolManagementService.cs` | Service | 3.3 |
| `src/Server/MES.Services.Equipment/IToolManagementService.cs` | Interface | 3.3 |
| `src/Server/MES.Api/Controllers/ToolManagementController.cs` | Controller | 3.3 |
| `src/Client/MES.Modules.Equipment/Views/ToolManagementView.xaml` | UI | 3.3 |
| `src/Shared/MES.Contracts/System/OperatorQualificationDto.cs` | DTO | 3.4 |
| `src/Server/MES.Services.System/OperatorQualificationService.cs` | Service | 3.4 |
| `src/Server/MES.Services.System/IOperatorQualificationService.cs` | Interface | 3.4 |
| `src/Server/MES.Api/Controllers/OperatorQualificationController.cs` | Controller | 3.4 |
| `src/Shared/MES.Contracts/Quality/FirstArticleInspectionDto.cs` | DTO | 3.4 |
| `src/Server/MES.Services.Quality/FirstArticleInspectionService.cs` | Service | 3.4 |
| `src/Server/MES.Services.Quality/IFirstArticleInspectionService.cs` | Interface | 3.4 |
| `src/Server/MES.Api/Controllers/FirstArticleInspectionController.cs` | Controller | 3.4 |
| `src/Client/MES.Modules.SystemSettings/Views/QualificationManagementView.xaml` | UI | 3.4 |

### 10.4 修改文件清单

| 文件路径 | 修改内容 | 阶段 |
|---------|---------|------|
| `src/Infrastructure/MES.Infrastructure.Persistence/MesDbContext.cs` | 新增10个DbSet+OnModelCreating配置 | 3.1-3.4 |
| `src/Server/MES.Services.Production/ProcessExecutionService.cs` | 扩展参数校验、SPC联动 | 3.1 |
| `src/Server/MES.Services.Production/IProcessExecutionService.cs` | 扩展接口 | 3.1 |
| `src/Shared/MES.Contracts/Production/ProcessExecutionDto.cs` | 扩展DTO | 3.1 |
| `src/Client/MES.Modules.Execute/Views/TrackInView.xaml` | 完整重写 | 3.1 |
| `src/Client/MES.Modules.Execute/ViewModels/TrackInViewModel.cs` | 完整重写 | 3.1 |
| `src/Client/MES.Modules.Quality/Views/FirstArticleInspectionView.xaml` | 完整重写 | 3.4 |
| `src/Client/MES.Modules.Quality/Views/SpcChartView.xaml` | 添加控制图 | 3.1 |

---

## 十一、验收标准

### 11.1 工序参数管控验收

- [ ] 参数模板CRUD完整，支持按工序/配方/产品查询
- [ ] TrackIn时自动加载对应参数模板
- [ ] 参数录入时实时校验Usl/Lsl，超限行红色高亮
- [ ] 必填参数未录入时阻止工序完成
- [ ] 关键参数超限自动Hold批次
- [ ] 参数记录自动写入SpcMeasurement（标记IsSpcMonitored的参数）
- [ ] 参数查询API返回完整记录含超规格标记

### 11.2 Bin管理验收

- [ ] Bin码主数据CRUD，预置标准Bin码
- [ ] 测试完成后自动计算良率和Bin分布
- [ ] Bin分布趋势图按产品/时间维度展示
- [ ] Bin超限(良率<阈值)自动触发预警
- [ ] Bin结果与批次状态联动

### 11.3 金线/铜线切换验收

- [ ] 线材类型CRUD，预置金线/铜线/合金线
- [ ] 批次进站时校验线材兼容性
- [ ] 线材切换记录完整(旧线→新线+时间+操作人)
- [ ] 换线后首件验证完成前不允许批量生产
- [ ] 线材使用记录永久追溯

### 11.4 模具/工具寿命验收

- [ ] 工具类型定义含寿命上限和预警阈值
- [ ] 工具实例创建+使用计数自动更新
- [ ] 寿命≤预警阈值时弹出预警通知
- [ ] 寿命=0时锁定设备不允许继续生产
- [ ] 工具更换记录+首件验证
- [ ] 工具寿命进度条可视化

### 11.5 操作员资质验收

- [ ] 资质类型定义含有效期
- [ ] 人员资质授予+到期日期
- [ ] TrackIn时校验操作员资质，无资质拦截
- [ ] 资质距到期≤30天时预警
- [ ] 过期资质自动失效(定时任务)
- [ ] 资质追溯记录

### 11.6 首件检验验收

- [ ] 首件模板按工序定义检验项目
- [ ] 首件检验录入界面支持模板加载
- [ ] 实测值自动判定OK/NG
- [ ] 关键项NG一票否决
- [ ] 首件未通过前阻止批量生产
- [ ] 审批流完整(提交→审批→放行)

---

## 十二、数据库迁移计划

### 12.1 迁移脚本清单

| 脚本文件 | 内容 | 阶段 |
|---------|------|------|
| `docs/sql/migration/031_001_parameter_template.sql` | 参数模板3表 | 3.1 |
| `docs/sql/migration/032_001_bin_management.sql` | Bin管理3表 | 3.2 |
| `docs/sql/migration/033_001_wire_material.sql` | 线材管理4表 | 3.3 |
| `docs/sql/migration/033_002_tool_management.sql` | 工具管理4表 | 3.3 |
| `docs/sql/migration/034_001_operator_qualification.sql` | 资质管理3表 | 3.4 |
| `docs/sql/migration/034_002_first_article_inspection.sql` | 首件检验4表 | 3.4 |

### 12.2 迁移执行顺序

```bash
# 阶段3.1
mysql -u root -p mes_db < docs/sql/migration/031_001_parameter_template.sql

# 阶段3.2
mysql -u root -p mes_db < docs/sql/migration/032_001_bin_management.sql

# 阶段3.3
mysql -u root -p mes_db < docs/sql/migration/033_001_wire_material.sql
mysql -u root -p mes_db < docs/sql/migration/033_002_tool_management.sql

# 阶段3.4
mysql -u root -p mes_db < docs/sql/migration/034_001_operator_qualification.sql
mysql -u root -p mes_db < docs/sql/migration/034_002_first_article_inspection.sql
```

### 12.3 预置数据脚本

```sql
-- 预置Bin码（阶段3.2）
INSERT INTO master_bin_code ... (已在4.3.1中定义)

-- 预置线材类型（阶段3.3）
INSERT INTO wire_material_definition ... (已在5.3.1中定义)

-- 预置工具类型（阶段3.3）
INSERT INTO tool_definition ... (已在6.3.1中定义)

-- 预置资质类型（阶段3.4）
INSERT INTO operator_qualification_type ... (已在7.3.1中定义)

-- 预置资质-工序映射（阶段3.4）
INSERT INTO qualification_step_mapping ... (已在7.3.3中定义)
```

### 12.4 回滚方案

每个迁移脚本包含对应的DROP语句在脚本末尾注释中：

```sql
-- 回滚脚本（注释状态，需要时取消注释执行）
-- DROP TABLE IF EXISTS prod_step_parameter_record;
-- DROP TABLE IF EXISTS master_parameter_template_item;
-- DROP TABLE IF EXISTS master_parameter_template;
```

### 12.5 EF Core Migration（可选）

如果使用EF Core Code First方式，在 `MesDbContext.cs` 中新增DbSet后执行：

```bash
# 阶段3.1
dotnet ef migrations add AddParameterTemplateTables

# 阶段3.2
dotnet ef migrations add AddBinManagementTables

# 阶段3.3
dotnet ef migrations add AddWireAndToolManagementTables

# 阶段3.4
dotnet ef migrations add AddQualificationAndFirstArticleTables

# 应用所有迁移
dotnet ef database update
```

---

## 附录

### A. 现有代码文件索引

| 模块 | 文件 | 路径 |
|------|------|------|
| Controller | ProcessExecutionController | `src/Server/MES.Api/Controllers/ProcessExecutionController.cs` |
| Controller | TestManagementController | `src/Server/MES.Api/Controllers/TestManagementController.cs` |
| Controller | SpcController | `src/Server/MES.Api/Controllers/SpcController.cs` |
| Service | ProcessExecutionService | `src/Server/MES.Services.Production/ProcessExecutionService.cs` |
| Service | TestManagementService | `src/Server/MES.Services.Production/TestManagementService.cs` |
| Service | SpcMeasurementService | `src/Server/MES.Services.Quality/SpcMeasurementService.cs` |
| DTO | ProcessExecutionDto | `src/Shared/MES.Contracts/Production/ProcessExecutionDto.cs` |
| DTO | TestManagementDto | `src/Shared/MES.Contracts/Production/TestManagementDto.cs` |
| DTO | SpcMeasurementDto | `src/Shared/MES.Contracts/Quality/SpcMeasurementDto.cs` |
| Entity | ProductionEntities | `src/Infrastructure/MES.Infrastructure.Persistence/Entities/ProductionEntities.cs` |
| DbContext | MesDbContext | `src/Infrastructure/MES.Infrastructure.Persistence/MesDbContext.cs` |
| UI | TrackInView (Execute) | `src/Client/MES.Modules.Execute/Views/TrackInView.xaml` |
| UI | TrackInViewModel (Execute) | `src/Client/MES.Modules.Execute/ViewModels/TrackInViewModel.cs` |
| UI | FirstArticleInspectionView | `src/Client/MES.Modules.Quality/Views/FirstArticleInspectionView.xaml` |
| UI | SpcChartView | `src/Client/MES.Modules.Quality/Views/SpcChartView.xaml` |
| UI | DispatchView | `src/Client/MES.Modules.Execute/Views/DispatchView.xaml` |
| UI | WipOverviewView | `src/Client/MES.Modules.Execute/Views/WipOverviewView.xaml` |

### B. 关键业务规则速查

| 规则 | 拦截点 | 动作 |
|------|--------|------|
| 参数超限(关键) | 参数录入时 | 自动Hold批次 |
| 参数超限(非关键) | 参数录入时 | 记录告警，允许继续 |
| 必填参数缺失 | 工序完成时 | 阻止完成 |
| Bin良率<阈值 | 测试完成时 | 按规则预警/Hold |
| 线材不兼容 | 批次进站时 | 拦截进站 |
| 换线未首件验证 | 换线后出站时 | 拦截出站 |
| 工具寿命到期 | 工序执行时 | 锁定设备 |
| 工具寿命预警 | 工序执行时 | 弹出通知 |
| 操作员无资质 | 批次进站时 | 拦截进站 |
| 资质即将到期 | 每日定时任务 | 通知预警 |
| 首件未通过 | 批量出站时 | 拦截出站 |

### C. 术语表

| 术语 | 英文 | 说明 |
|------|------|------|
| 工序 | Step/Process | 工艺路线中的一个步骤 |
| 批次 | Lot | 一组具有相同特征的产品集合 |
| 进站 | Track-In | 批次进入某工序 |
| 出站 | Track-Out | 批次完成某工序 |
| 参数模板 | Parameter Template | 定义工序需要记录的参数及规格 |
| Bin码 | Bin Code | 测试结果分类代码 |
| 金线/铜线 | Wire Bond | 芯片封装中的键合线材料 |
| 劈刀 | Capillary | 打线机上的消耗工具 |
| 模具 | Mold | 封装成型用的模具 |
| 刀片 | Blade | 切割用的刀片 |
| 首件检验 | First Article Inspection | 批次开始时的首个产品全面检验 |
| 资质 | Qualification | 操作员执行某工序的资格认证 |
| SPC | Statistical Process Control | 统计过程控制 |
| FDC | Fault Detection and Classification | 故障检测与分类 |
| OOC | Out of Control | 失控 |
| EAP | Equipment Automation Program | 设备自动化程序 |

---

> **文档结束**  
> 本文档为MES系统阶段三（工序管控深化）的完整实施计划，涵盖6大模块的详细设计、数据表定义、接口定义、UI方案、业务规则和实施路径。  
> 评审通过后即可按阶段3.1→3.5的顺序开始实施。
