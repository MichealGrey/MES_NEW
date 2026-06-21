# MES 优化改善计划

> **文档编号：** MES-PLAN-2026-001  
> **版本：** V1.0  
> **创建日期：** 2026-06-06  
> **项目：** OSAT Semiconductor Assembly & Test MES System  
> **当前状态：** ~35% 需求覆盖率，核心框架已搭建，大量 UI 模块缺少后端 API  
> **计划性质：** 分阶段实施路线图（OUTLINE 级别）

---

## 一、总体策略

### 1.1 明确排除范围

| 排除项 | 原因 | 备注 |
|--------|------|------|
| 移动端/PDA 功能（REQ-MP-001~005） | 技术栈选型需单独论证，当前聚焦 Web/API 能力建设 | 后续独立项目推进 |
| REQ-99-EO-001 移动端全覆盖 | 同上 | 从规格说明书中移除 |

### 1.2 集成层架构策略：适配器模式

所有外部系统集成（ERP/EAP/WMS/QMS/OA/客户门户）统一采用 **Adapter Pattern** 架构：

- **接口层（Abstractions）：** 每个集成定义独立接口（`IMesErpAdapter`、`IMesEapAdapter` 等），放在 `Adapters.Abstractions` 项目中
- **Mock 实现：** 放在独立的 `Adapters.Mock` 项目/命名空间中，模拟真实系统响应，支撑 MES 内部功能完整联调
- **真实实现（预留）：** `Adapters.Real` 项目/命名空间，后续根据实际对接系统开发替换
- **切换机制：** 通过配置驱动（`appsettings.json`），支持运行时热切换

```
MES Core → Adapter Interface (Abstractions) → Mock Implementation (开发/测试期)
                                                ↘ Real Implementation (生产期)
```

### 1.3 实施阶段总览

| 阶段 | 名称 | 周期 | 核心目标 |
|------|------|------|---------|
| **阶段一** | 核心质量与仓储（P0 补齐） | 6-8 周 | IQC/FQC/OQC/MRB + 仓储全流程后端 API |
| **阶段二** | 订单与计划管理（P0/P1 补齐） | 5-7 周 | 订单评审 + 主生产计划 + MRP + 工单分解 |
| **阶段三** | 工序管控深化（P0/P1 补齐） | 6-8 周 | 各工序参数管控 + Bin 管理 + 金线铜线切换 + 模具寿命 + 资质校验 |
| **阶段四** | 外部系统集成（适配器+Mock） | 5-7 周 | ERP/EAP/WMS/QMS/OA/客户门户 六大集成 |
| **阶段五** | 追溯与管理决策（P1/P2 补齐） | 4-6 周 | KPI 看板 + 成本分析 + 良率分析 + NPI + 可靠性测试 + 审计追踪 |

### 1.4 预估总工作量

| 维度 | 预估 |
|------|------|
| 总周期 | **26-36 周**（约 6-9 个月） |
| 新增 API Controller | ~25-30 个 |
| 新增 Service 层 | ~30-35 个 |
| 新增/修改数据表 | ~40-50 张 |
| 新增 Adapter 接口 | 6 个（ERP/EAP/WMS/QMS/OA/CustomerPortal） |
| Mock 实现类 | ~15-20 个 |
| 数据库迁移脚本 | ~12-15 个版本 |
| 预估需求覆盖率提升 | **35% → 85%+** |

---

## 二、阶段划分

### 阶段一：核心质量与仓储（P0 补齐）

> **目标：** 补齐质量管理和仓储物料的核心后端 API，使系统具备来料检验→生产→出货的完整质量与物料管控闭环。

---

#### 1. IQC 来料检验管理

**对应需求：** REQ-QM-001 (P0), REQ-02-US-001 (P1), REQ-02-CD-001 (P0), REQ-02-EF-001 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 实现来料检验全流程：检验任务自动生成→IQC 录入→自动判定→合格放行/不合格触发 MRB |
| **核心接口** | `POST /api/iqc/tasks` 创建检验任务<br>`GET /api/iqc/tasks` 查询检验任务列表<br>`POST /api/iqc/tasks/{id}/execute` 执行检验录入<br>`POST /api/iqc/tasks/{id}/judge` 检验判定<br>`POST /api/iqc/batches/{id}/isolate` 一键隔离<br>`POST /api/iqc/batches/{id}/release` 解除隔离 |
| **数据表变更** | 新增 `iqc_inspection_task`（检验任务表）<br>新增 `iqc_inspection_result`（检验结果明细表）<br>新增 `material_batch`（物料批次表）<br>修改 `lot` 表增加 `msl_level`, `shelf_life_expiry`, `isolation_status` 字段 |
| **优先级** | P0 |

---

#### 2. FQC/OQC 终检管理

**对应需求：** REQ-QM-005 (P0), REQ-11-US-001 (P1), REQ-11-CD-001 (P0)

| 项目 | 内容 |
|------|------|
| **目标** | 完工自动触发 FQC 终检，出货前强制 OQC 抽检，MSL 出货检查 |
| **核心接口** | `GET /api/fqc/tasks` 查询 FQC 待检任务<br>`POST /api/fqc/tasks/{id}/execute` 执行终检<br>`GET /api/oqc/tasks` 查询 OQC 出货检验任务<br>`POST /api/oqc/tasks/{id}/execute` 执行出货检验<br>`POST /api/oqc/msl-check` MSL 出货检查 |
| **数据表变更** | 新增 `fqc_inspection_record`（FQC 检验记录）<br>新增 `oqc_inspection_record`（OQC 检验记录）<br>新增 `shipment_msl_check`（出货 MSL 检查记录） |
| **优先级** | P0 |

---

#### 3. 不合格品管理与 MRB 评审

**对应需求：** REQ-QM-006 (P0), REQ-02-PA-001 (P2)

| 项目 | 内容 |
|------|------|
| **目标** | 不合格品标识→隔离→MRB 评审（质量/工艺/工程三方会签）→处置执行（返工/报废/让步） |
| **核心接口** | `POST /api/nonconforming/records` 创建不合格品记录<br>`GET /api/nonconforming/records` 查询不合格品列表<br>`POST /api/nonconforming/records/{id}/mrb` 发起 MRB 评审<br>`POST /api/nonconforming/records/{id}/disposition` 处置执行<br>`POST /api/nonconforming/records/{id}/rework-verify` 返工重检验证 |
| **数据表变更** | 新增 `nonconforming_record`（不合格品记录表）<br>新增 `mrb_review`（MRB 评审表）<br>新增 `mrb_review_item`（MRB 评审明细表）<br>新增 `disposition_record`（处置记录表） |
| **优先级** | P0 |

---

#### 4. 原材料入库 / FIFO / 有效期 / 发退料

**对应需求：** REQ-WM-001~004 (P0 × 4)

| 项目 | 内容 |
|------|------|
| **目标** | 原材料入库批次管理、FIFO 强制发料、有效期自动预警与锁定、产线发料/退料全流程 |
| **核心接口** | `POST /api/warehouse/receipt` 原材料入库<br>`GET /api/warehouse/inventory` 库存查询<br>`POST /api/warehouse/issue` 产线发料（自动推荐 FIFO 批次）<br>`POST /api/warehouse/return` 产线退料<br>`GET /api/warehouse/expiry-warnings` 有效期预警列表<br>`POST /api/warehouse/batches/{id}/lock` 批次锁定（过期/异常） |
| **数据表变更** | 新增 `warehouse_receipt`（入库单表）<br>新增 `warehouse_inventory`（库存台账表）<br>新增 `warehouse_issue_order`（发料单表）<br>新增 `warehouse_return_order`（退料单表）<br>新增 `material_shelf_life`（物料有效期记录表）<br>新增 `warehouse_location`（库位表） |
| **优先级** | P0 |

---

#### 5. 成品入库

**对应需求：** REQ-WM-006 (P0)

| 项目 | 内容 |
|------|------|
| **目标** | 测试合格后自动触发成品入库，支持分批入库、库位分配 |
| **核心接口** | `POST /api/warehouse/finished-goods/receipt` 成品入库<br>`GET /api/warehouse/finished-goods/inventory` 成品库存查询<br>`POST /api/warehouse/finished-goods/ship` 成品出库 |
| **数据表变更** | 新增 `finished_goods_receipt`（成品入库单）<br>新增 `finished_goods_inventory`（成品库存表） |
| **优先级** | P0 |

---

#### 6. 生产异常上报与停线机制

**对应需求：** REQ-PO-011 (P0), REQ-99-PD-001 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 一键异常上报（设备/质量/物料/工艺/安全），IPQC/班组长停线权限，异常处理全流程闭环 |
| **核心接口** | `POST /api/abnormal/report` 异常上报<br>`POST /api/abnormal/line-stop` 停线指令<br>`POST /api/abnormal/line-resume` 恢复生产<br>`GET /api/abnormal/records` 异常记录查询<br>`POST /api/abnormal/records/{id}/handle` 异常处理<br>`POST /api/abnormal/records/{id}/verify` 处理验证 |
| **数据表变更** | 新增 `abnormal_record`（异常记录表）<br>新增 `line_stop_record`（停线记录表）<br>修改 `alarm_record` 增加与异常的关联字段 |
| **优先级** | P0 |

---

#### 7. 设备故障管理与保养

**对应需求：** REQ-EQ-002 (P0), REQ-EQ-003 (P0)

| 项目 | 内容 |
|------|------|
| **目标** | 设备故障上报→维修派工→维修执行→验证关闭；PM 保养计划→执行→记录→合规率统计 |
| **核心接口** | `POST /api/equipment/faults` 故障上报<br>`POST /api/equipment/faults/{id}/dispatch` 维修派工<br>`POST /api/equipment/faults/{id}/complete` 维修完成<br>`GET /api/equipment/pm-schedule` PM 计划查询<br>`POST /api/equipment/pm-schedule/{id}/execute` 执行保养<br>`GET /api/equipment/mtbf-mttr` MTBF/MTTR 统计 |
| **数据表变更** | 新增 `equipment_fault_record`（设备故障记录）<br>新增 `equipment_maintenance_record`（维修记录）<br>新增 `equipment_pm_plan`（PM 保养计划）<br>新增 `equipment_pm_execution`（PM 执行记录）<br>修改 `equipment_info` 增加 `mtbf`, `mttr`, `last_maintenance_date` 字段 |
| **优先级** | P0 |

---

#### 8. 首件检验流程

**对应需求：** REQ-QM-002 (P0), REQ-03-US-002 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 换线/开班/工艺变更自动触发首件检验，双人确认（技术员+IPQC），不合格锁定工单 |
| **核心接口** | `GET /api/first-article/pending` 待首件检验列表<br>`POST /api/first-article/trigger` 触发首件检验<br>`POST /api/first-article/{id}/inspect` 检验录入<br>`POST /api/first-article/{id}/confirm` 双人确认<br>`POST /api/first-article/{id}/reject` 首件不合格处理 |
| **数据表变更** | 新增 `first_article_inspection`（首件检验表）<br>新增 `first_article_inspection_item`（首件检验项目明细）<br>新增 `first_article_signature`（首件签名记录） |
| **优先级** | P0 |

---

#### 9. 紧急质量召回

**对应需求：** REQ-QM-011 (P0)

| 项目 | 内容 |
|------|------|
| **目标** | Quality Alert 发布→自动冻结在制品/库存品→追溯受影响批次→生成召回清单 |
| **核心接口** | `POST /api/quality-alert` 发布质量警报<br>`POST /api/quality-alert/{id}/freeze` 冻结相关批次<br>`GET /api/quality-alert/{id}/affected-lots` 查询受影响批次<br>`POST /api/quality-alert/{id}/recall-notice` 生成召回通知<br>`POST /api/quality-alert/{id}/unfreeze` 解除冻结 |
| **数据表变更** | 新增 `quality_alert`（质量警报表）<br>新增 `quality_alert_affected_lot`（影响批次关联表）<br>新增 `recall_notice`（召回通知表） |
| **优先级** | P0 |

---

### 阶段二：订单与计划管理（P0/P1 补齐）

> **目标：** 建立从客户订单到工单分解的完整计划链路，实现订单评审、产能评估、MRP 计算、插单处理。

---

#### 1. 订单评审流程

**对应需求：** REQ-01-PD-001 (P0), REQ-SL-001 (P0)

| 项目 | 内容 |
|------|------|
| **目标** | 线上订单评审工作流：自动触发→多角色评审→通过/驳回/有条件通过→通过后自动创建工单 |
| **核心接口** | `POST /api/orders` 创建订单<br>`POST /api/orders/{id}/review/start` 启动评审流程<br>`POST /api/orders/{id}/review/vote` 评审投票（通过/驳回/条件通过）<br>`GET /api/orders/{id}/review/status` 评审状态查询<br>`POST /api/orders/{id}/review/complete` 评审完成并创建工单 |
| **数据表变更** | 新增 `sales_order`（销售订单表）<br>新增 `order_review`（订单评审表）<br>新增 `order_review_item`（评审明细表）<br>新增 `order_version`（订单版本表） |
| **优先级** | P0 |

---

#### 2. 主生产计划与产能评估

**对应需求：** REQ-PP-001 (P0), REQ-SL-002 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 基于订单需求、工序 UPH、设备可用率、人员班次自动计算产能负荷，识别瓶颈 |
| **核心接口** | `POST /api/mpp/generate` 生成主生产计划<br>`GET /api/mpp/capacity-load` 查询产能负荷<br>`GET /api/mpp/bottlenecks` 瓶颈工序查询<br>`POST /api/mpp/simulate` What-if 产能模拟<br>`GET /api/mpp/rolling-forecast` 滚动预测查询 |
| **数据表变更** | 新增 `master_production_plan`（主生产计划表）<br>新增 `capacity_load`（产能负荷表）<br>新增 `capacity_simulation`（产能模拟记录表） |
| **优先级** | P0 |

---

#### 3. 工单分解与物料需求计划

**对应需求：** REQ-PP-002 (P0), REQ-PP-004 (P0)

| 项目 | 内容 |
|------|------|
| **目标** | 订单自动分解为工单并指定原材料批次；MRP 计算物料需求，对比库存生成缺料预警 |
| **核心接口** | `POST /api/work-orders/batch-create` 批量创建工单<br>`POST /api/work-orders/{id}/decompose` 工单分解<br>`POST /api/mrp/calculate` 执行 MRP 计算<br>`GET /api/mrp/shortage-warnings` 缺料预警查询<br>`POST /api/mrp/export-purchase-req` 导出采购申请 |
| **数据表变更** | 修改 `work_order` 增加 `material_batch_assignment`, `bom_version` 字段<br>新增 `mrp_calculation`（MRP 计算记录表）<br>新增 `mrp_shortage_warning`（缺料预警表）<br>新增 `bom`（物料清单表）<br>新增 `bom_item`（BOM 明细表） |
| **优先级** | P0 |

---

#### 4. 订单进度查询与交付达成率

**对应需求：** REQ-SL-003 (P1), REQ-PP-006 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 按订单/工单/批次实时查询生产进度；OTD 统计分析与延迟原因分类 |
| **核心接口** | `GET /api/orders/{id}/progress` 订单进度查询<br>`GET /api/orders/progress/batch` 批量进度查询<br>`GET /api/analytics/otd` OTD 达成率统计<br>`GET /api/analytics/otd/delay-reasons` 延迟原因分析 |
| **数据表变更** | 新增 `order_progress_snapshot`（订单进度快照表）<br>新增 `otd_statistics`（OTD 统计表）<br>新增 `delay_reason_record`（延迟原因记录表） |
| **优先级** | P1 |

---

#### 5. 插单处理

**对应需求：** REQ-SL-004 (P1), REQ-01-SS-001 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 插单影响分析（受影响的工单清单/延迟预估/物料齐套率），审批后自动调整排产计划 |
| **核心接口** | `POST /api/orders/rush-insert` 提交插单申请<br>`GET /api/orders/rush-insert/{id}/impact-analysis` 插单影响分析<br>`POST /api/orders/rush-insert/{id}/approve` 插单审批<br>`POST /api/orders/rush-insert/{id}/execute` 执行插单并更新排产 |
| **数据表变更** | 新增 `rush_order_request`（插单申请表）<br>新增 `rush_order_impact`（插单影响分析表） |
| **优先级** | P1 |

---

### 阶段三：工序管控深化（P0/P1 补齐）

> **目标：** 深化各工序参数管控、耗材寿命管理、操作员资质校验、首件确认流程，实现生产全过程精细化管控。

---

#### 1. 各工序参数管控

**对应需求：** REQ-03-US-001 (P0), REQ-04-PD-001 (P1), REQ-05-US-001 (P0), REQ-06-CD-001 (P0)

| 项目 | 内容 |
|------|------|
| **目标** | 减薄/贴片/键合/塑封各工序工艺参数自动加载、版本控制、越权修改拦截 |
| **核心接口** | `GET /api/process-params/work-order/{id}` 按工单加载工艺参数<br>`POST /api/process-params/override` 参数修改申请<br>`GET /api/process-params/history` 参数变更历史<br>`POST /api/process-params/validate` 参数合规校验 |
| **数据表变更** | 新增 `process_parameter_set`（工序参数集表）<br>新增 `process_parameter_item`（参数明细表）<br>新增 `process_parameter_override_log`（参数修改日志表）<br>新增 `curing_temperature_curve`（固化温度曲线表） |
| **优先级** | P0 |

---

#### 2. Bin 管理

**对应需求：** REQ-PO-009 (P0), REQ-09-PA-001 (P2)

| 项目 | 内容 |
|------|------|
| **目标** | 测试分选 Bin 定义（最多 64 个 Bin），各 Bin 数量统计，不良品自动隔离 |
| **核心接口** | `POST /api/bin/definitions` Bin 定义管理<br>`GET /api/bin/statistics/{workOrderId}` Bin 统计查询<br>`POST /api/bin/sort` Bin 分选操作<br>`GET /api/bin/pareto` 缺陷柏拉图数据 |
| **数据表变更** | 新增 `bin_definition`（Bin 定义表）<br>新增 `bin_sort_record`（Bin 分选记录表）<br>新增 `bin_statistics`（Bin 统计表） |
| **优先级** | P0 |

---

#### 3. 金线/铜线切换管理

**对应需求：** REQ-05-PD-001 (P1), REQ-PO-004 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 线材类型与参数集绑定，切换时强制参数重新确认和首件检验 |
| **核心接口** | `POST /api/wire-bonding/material-switch` 线材切换申请<br>`GET /api/wire-bonding/material-switch/{id}/validation` 切换校验<br>`POST /api/wire-bonding/material-switch/{id}/confirm` 切换确认（含首件） |
| **数据表变更** | 新增 `wire_material_switch_record`（线材切换记录表）<br>新增 `wire_consumption`（线材消耗记录表） |
| **优先级** | P1 |

---

#### 4. 模具/刀片/劈刀寿命管理

**对应需求：** REQ-03-PD-001 (P1), REQ-05-SS-001 (P1), REQ-06-US-001 (P1), REQ-08-US-002 (P2)

| 项目 | 内容 |
|------|------|
| **目标** | 模具/刀片/劈刀唯一编号、寿命设定、使用计数、预警、到期锁定、更换记录 |
| **核心接口** | `POST /api/tooling/register` 工装注册<br>`GET /api/tooling/lifecycle/{id}` 工装生命周期查询<br>`POST /api/tooling/{id}/use-record` 使用记录<br>`POST /api/tooling/{id}/replace` 更换记录<br>`GET /api/tooling/expiring` 即将到期工装列表 |
| **数据表变更** | 新增 `tooling_registry`（工装台账表）<br>新增 `tooling_usage_log`（工装使用日志表）<br>新增 `tooling_replacement_record`（更换记录表）<br>修改 `fixture` 表增加寿命相关字段 |
| **优先级** | P1 |

---

#### 5. 操作员资质校验

**对应需求：** REQ-PO-012 (P1), REQ-99-US-002 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 工序/设备资质认证管理，开工前自动校验，过期限制操作，到期预警 |
| **核心接口** | `POST /api/qualification/certify` 资质认证<br>`GET /api/qualification/check` 开工前资质校验<br>`GET /api/qualification/expiring` 即将过期资质<br>`POST /api/qualification/{id}/renew` 资质续期 |
| **数据表变更** | 新增 `operator_qualification`（操作员资质表）<br>新增 `qualification_certification`（认证记录表）<br>新增 `qualification_check_log`（资质校验日志表） |
| **优先级** | P1 |

---

#### 6. 首件确认流程深化

**对应需求：** REQ-03-US-002 (P1), REQ-05-CD-001 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 首件确认完整流程：换线触发→检验→双人签名→合格放行/不合格锁定 |
| **核心接口** | `POST /api/first-article/work-order/{id}/trigger` 按工单触发<br>`POST /api/first-article/{id}/bond-test` 焊线拉力测试录入<br>`POST /api/first-article/{id}/dual-sign` 双人签名确认 |
| **数据表变更** | 扩展 `first_article_inspection` 增加工序特定字段<br>新增 `bond_pull_test_record`（焊线拉力测试记录） |
| **优先级** | P1 |

---

### 阶段四：外部系统集成（适配器+Mock）

> **目标：** 建立六大外部系统集成的适配器架构，Mock 实现支撑联调测试，真实实现预留替换。

---

#### 1. ERP 系统对接

**对应需求：** REQ-IS-001 (P0), REQ-11-PD-001 (P1)

| 项目 | 内容 |
|------|------|
| **对接场景** | ERP→MES：销售订单、BOM、物料主数据<br>MES→ERP：工单完工、物料消耗、成品入库、报废、工时 |
| **接口定义** | `IMesErpAdapter`<br>`- Task<OrderSyncResult> SyncSalesOrdersAsync(OrderSyncRequest)`<br>`- Task<BomSyncResult> SyncBomAsync(BomSyncRequest)`<br>`- Task<MaterialSyncResult> SyncMaterialsAsync(MaterialSyncRequest)`<br>`- Task<CompletionSyncResult> ReportWorkOrderCompletionAsync(CompletionData)`<br>`- Task<MaterialConsumeResult> ReportMaterialConsumeAsync(ConsumeData)`<br>`- Task<FinishedGoodsResult> ReportFinishedGoodsReceiptAsync(ReceiptData)` |
| **Mock 数据** | 预置 5-10 个模拟订单、BOM、物料数据；返回固定格式的完工/消耗确认 |
| **切换方式** | `appsettings.json` 中 `ErpAdapter:Provider` 配置为 `Mock` 或 `SapErp`/`Kingdee` |
| **优先级** | P0 |

---

#### 2. EAP 设备自动化集成

**对应需求：** REQ-IS-002 (P0), REQ-EQ-007 (P1)

| 项目 | 内容 |
|------|------|
| **对接场景** | 工艺参数下发、设备状态采集、生产数据上报、设备报警推送 |
| **接口定义** | `IMesEapAdapter`<br>`- Task<ParameterDownloadResult> DownloadParametersAsync(ParameterSet)`<br>`- Task<EquipmentStatus> GetEquipmentStatusAsync(string equipmentId)`<br>`- Task<DataCollectionResult> CollectProcessDataAsync(string equipmentId)`<br>`- Task<AlarmPushResult> PushAlarmAsync(AlarmData)` |
| **Mock 数据** | 模拟 80 台焊线机、20 台贴片机等设备的状态切换、参数响应、报警事件 |
| **切换方式** | `EapAdapter:Provider` 配置为 `Mock` 或 `SecsGem`/`OpcUa` |
| **优先级** | P0 |

---

#### 3. WMS 仓储管理系统对接

**对应需求：** REQ-IS-003 (P1)

| 项目 | 内容 |
|------|------|
| **对接场景** | MES→WMS：物料需求、发料指令、退料指令<br>WMS→MES：库存信息、出入库确认 |
| **接口定义** | `IMesWmsAdapter`<br>`- Task<MaterialRequestResult> SendMaterialRequestAsync(MaterialRequest)`<br>`- Task<IssueResult> SendIssueInstructionAsync(IssueInstruction)`<br>`- Task<ReturnResult> SendReturnInstructionAsync(ReturnInstruction)`<br>`- Task<InventoryData> GetInventoryAsync(InventoryQuery)`<br>`- Task<ReceiptConfirm> ReceiveReceiptConfirmAsync(ReceiptData)` |
| **Mock 数据** | 预置仓库库位、库存台账、出入库确认响应 |
| **切换方式** | `WmsAdapter:Provider` 配置为 `Mock` 或 `Real` |
| **优先级** | P1 |

---

#### 4. QMS 质量管理系统对接

**对应需求：** REQ-IS-004 (P1)

| 项目 | 内容 |
|------|------|
| **对接场景** | MES→QMS：检验数据、不合格品信息、SPC 数据<br>QMS→MES：检验标准、FMEA 信息、质量 Alert |
| **接口定义** | `IMesQmsAdapter`<br>`- Task<InspectionPushResult> PushInspectionDataAsync(InspectionData)`<br>`- Task<NonconformingResult> PushNonconformingInfoAsync(NonconformingData)`<br>`- Task<SpcPushResult> PushSpcDataAsync(SpcData)`<br>`- Task<InspectionStandard> PullInspectionStandardAsync(StandardQuery)`<br>`- Task<FmeaData> PullFmeaDataAsync(FmeaQuery)`<br>`- Task<QualityAlert> ReceiveQualityAlertAsync()` |
| **Mock 数据** | 预置检验标准、FMEA 数据、模拟质量 Alert |
| **切换方式** | `QmsAdapter:Provider` 配置为 `Mock` 或 `Real` |
| **优先级** | P1 |

---

#### 5. OA 审批流集成

**对应需求：** REQ-IS-006 (P2)

| 项目 | 内容 |
|------|------|
| **对接场景** | MES 审批流程→OA：工艺变更、工程变更、不合格品处置、插单审批、报废审批 |
| **接口定义** | `IMesOaAdapter`<br>`- Task<ApprovalPushResult> PushApprovalRequestAsync(ApprovalRequest)`<br>`- Task<ApprovalStatus> GetApprovalStatusAsync(string approvalId)`<br>`- Task<ApprovalCallback> ReceiveApprovalCallbackAsync(ApprovalResult)` |
| **Mock 数据** | 模拟 OA 审批通过/驳回响应，支持超时模拟 |
| **切换方式** | `OaAdapter:Provider` 配置为 `Mock` 或 `Feishu`/`DingTalk`/`WeCom` |
| **优先级** | P2 |

---

#### 6. 客户门户对接

**对应需求：** REQ-IS-005 (P1)

| 项目 | 内容 |
|------|------|
| **对接场景** | 客户在线下单、订单进度查询、质量报告下载、客诉提交、出货文件获取 |
| **接口定义** | `IMesCustomerPortalAdapter`<br>`- Task<PortalOrderResult> ReceivePortalOrderAsync(PortalOrder)`<br>`- Task<OrderProgressData> GetOrderProgressAsync(string orderId)`<br>`- Task<QualityReport> GetQualityReportAsync(ReportQuery)`<br>`- Task<ComplaintResult> ReceiveComplaintAsync(ComplaintData)`<br>`- Task<ShippingDocument> GetShippingDocumentAsync(DocumentQuery)` |
| **Mock 数据** | 预置客户订单、进度模拟、质量报告模板 |
| **切换方式** | `CustomerPortalAdapter:Provider` 配置为 `Mock` 或 `RealApi` |
| **优先级** | P1 |

---

### 阶段五：追溯与管理决策（P1/P2 补齐）

> **目标：** 完善全厂 KPI 看板、成本分析、良率分析、NPI 管理、可靠性测试、审计追踪不可篡改。

---

#### 1. 全厂 KPI 看板

**对应需求：** REQ-MG-001 (P0), REQ-99-SS-001 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 实时展示产出达成率、综合良率、OEE、DPPM、客诉率、OTD、在制品数量 |
| **核心接口** | `GET /api/kpi/dashboard` KPI 看板数据<br>`GET /api/kpi/dashboard/{metric}/detail` 指标下钻<br>`GET /api/kpi/real-time` 实时数据刷新 |
| **数据表变更** | 新增 `kpi_dashboard_snapshot`（KPI 快照表）<br>新增 `kpi_metric_definition`（指标定义表） |
| **优先级** | P0 |

---

#### 2. 生产成本分析

**对应需求：** REQ-MG-002 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 按产品/工单/月份统计直接材料、直接人工、制造费用，单位成本计算与差异分析 |
| **核心接口** | `GET /api/cost/product-analysis` 产品成本分析<br>`GET /api/cost/work-order-analysis` 工单成本分析<br>`GET /api/cost/variance-analysis` 成本差异分析<br>`POST /api/cost/calculate` 触发成本计算 |
| **数据表变更** | 新增 `cost_record`（成本记录表）<br>新增 `cost_variance`（成本差异表）<br>新增 `unit_cost`（单位成本表） |
| **优先级** | P1 |

---

#### 3. 良率分析

**对应需求：** REQ-EN-005 (P1), REQ-QM-009 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 各工序良率实时统计、综合良率、良率趋势、缺陷柏拉图、DPPM 统计 |
| **核心接口** | `GET /api/yield/process-yield` 工序良率<br>`GET /api/yield/cumulative-yield` 综合良率<br>`GET /api/yield/trend` 良率趋势<br>`GET /api/yield/pareto` 缺陷柏拉图<br>`GET /api/yield/dppm` DPPM 统计 |
| **数据表变更** | 新增 `yield_statistics`（良率统计表）<br>新增 `defect_pareto`（缺陷柏拉图数据表）<br>新增 `dppm_statistics`（DPPM 统计表） |
| **优先级** | P1 |

---

#### 4. 人员绩效分析

**对应需求：** REQ-MG-003 (P2)

| 项目 | 内容 |
|------|------|
| **目标** | 操作员产出/合格率/工时利用率，技术员换线时间/首件通过率，班组排名 |
| **核心接口** | `GET /api/performance/operator` 操作员绩效<br>`GET /api/performance/technician` 技术员绩效<br>`GET /api/performance/team-ranking` 班组排名 |
| **数据表变更** | 新增 `operator_performance`（操作员绩效表）<br>新增 `team_performance`（班组绩效表） |
| **优先级** | P2 |

---

#### 5. NPI 新产品导入管理

**对应需求：** REQ-EN-003 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | NPI 项目全生命周期：立项→工艺设计→试产→评审→量产转移 |
| **核心接口** | `POST /api/npi/projects` 创建 NPI 项目<br>`GET /api/npi/projects/{id}/stages` 项目阶段查询<br>`POST /api/npi/projects/{id}/trial-run` 试产执行<br>`POST /api/npi/projects/{id}/review` 试产评审<br>`POST /api/npi/projects/{id}/transfer-mass-production` 量产转移 |
| **数据表变更** | 新增 `npi_project`（NPI 项目表）<br>新增 `npi_stage`（NPI 阶段表）<br>新增 `npi_trial_run`（试产记录表）<br>新增 `npi_review`（试产评审表） |
| **优先级** | P1 |

---

#### 6. 可靠性测试管理

**对应需求：** REQ-QM-008 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | HTOL/THB/TC/uHAST/ESD/Latch-up 测试计划、执行记录、失败触发 FA 流程 |
| **核心接口** | `POST /api/reliability/plans` 创建测试计划<br>`GET /api/reliability/plans` 测试计划查询<br>`POST /api/reliability/plans/{id}/execute` 执行测试<br>`POST /api/reliability/plans/{id}/fail-fa` 测试失败触发 FA |
| **数据表变更** | 新增 `reliability_test_plan`（可靠性测试计划表）<br>新增 `reliability_test_record`（测试记录表）<br>新增 `reliability_test_result`（测试结果表） |
| **优先级** | P1 |

---

#### 7. 报表自动生成

**对应需求：** REQ-MG-004 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 生产日报/周报/月报自动生成，定时推送，PDF/Excel 导出 |
| **核心接口** | `POST /api/reports/generate` 触发报表生成<br>`GET /api/reports/schedule` 报表计划查询<br>`POST /api/reports/schedule` 创建报表计划<br>`GET /api/reports/{id}/download` 报表下载 |
| **数据表变更** | 新增 `report_schedule`（报表计划表）<br>新增 `report_generation_log`（报表生成日志表）<br>新增 `report_file`（报表文件表） |
| **优先级** | P1 |

---

#### 8. 系统参数配置

**对应需求：** REQ-SC-002 (P1), REQ-SC-003 (P1)

| 项目 | 内容 |
|------|------|
| **目标** | 集中参数配置界面，预警规则配置，审批流程配置 |
| **核心接口** | `GET /api/system-config` 系统参数查询<br>`PUT /api/system-config` 系统参数更新<br>`GET /api/alert-rules` 预警规则查询<br>`POST /api/alert-rules` 创建预警规则<br>`PUT /api/alert-rules/{id}` 更新预警规则 |
| **数据表变更** | 新增 `system_config`（系统参数表）<br>新增 `alert_rule`（预警规则表）<br>新增 `alert_notification_config`（通知配置表） |
| **优先级** | P1 |

---

#### 9. 审计追踪不可篡改

**对应需求：** REQ-SC-005 (P0), REQ-99-CD-001 (P0), REQ-09-CD-001 (P0)

| 项目 | 内容 |
|------|------|
| **目标** | 关键操作强制电子签名，审计日志写入后不可修改，保留 ≥3 年，支持哈希校验防篡改 |
| **核心接口** | `GET /api/audit-trails` 审计日志查询<br>`GET /api/audit-trails/verify` 日志完整性校验<br>`POST /api/audit-trails/hash-check` 哈希校验<br>`GET /api/signature/records` 电子签名记录查询 |
| **数据表变更** | 修改 `audit_trail` 表增加 `hash_value`, `prev_hash` 字段（链式哈希）<br>新增 `data_correction_record`（数据修正记录表）<br>新增 `signature_record` 扩展表 |
| **优先级** | P0 |

---

## 三、集成层架构设计

### 3.1 适配器模式整体架构

```
┌─────────────────────────────────────────────────────────────────┐
│                        MES Core Layer                           │
│  (Order / Planning / Quality / Warehouse / Process / Trace)      │
│                                                                 │
│  Service 层通过接口调用适配器，不依赖任何具体实现                  │
└───────────────────────┬─────────────────────────────────────────┘
                        │ 依赖接口（Abstractions）
                        ▼
┌─────────────────────────────────────────────────────────────────┐
│                   Adapters.Abstractions                         │
│                                                                 │
│  IMesErpAdapter    IMesEapAdapter    IMesWmsAdapter             │
│  IMesQmsAdapter    IMesOaAdapter     IMesCustomerPortalAdapter   │
│                                                                 │
│  纯接口定义，无实现，无外部依赖                                     │
└───────────────┬─────────────────────┬───────────────────────────┘
                │                     │
        ┌───────▼──────┐    ┌────────▼──────────┐
        │ Adapters.Mock │    │  Adapters.Real    │
        │  (开发/测试期) │    │  (生产期, 预留)    │
        │              │    │                   │
        │ MockErpImpl  │    │ SapErpImpl       │
        │ MockEapImpl  │    │ SecsGemEapImpl   │
        │ MockWmsImpl  │    │ OpcUaEapImpl     │
        │ MockQmsImpl  │    │ KingdeeErpImpl   │
        │ MockOaImpl   │    │ FeishuOaImpl     │
        │ MockPortal   │    │ DingTalkOaImpl   │
        └──────────────┘    │ RealPortalImpl   │
                            └───────────────────┘
```

### 3.2 项目结构建议

```
src/
├── MES.Core/                              # 现有核心项目
│   ├── Controllers/
│   ├── Services/
│   └── Models/
│
├── Adapters/
│   ├── Adapters.Abstractions/             # 接口定义层
│   │   ├── Adapters.Abstractions.csproj
│   │   ├── Erp/
│   │   │   ├── IMesErpAdapter.cs
│   │   │   └── Dto/ (OrderSyncRequest, BomSyncRequest, etc.)
│   │   ├── Eap/
│   │   │   ├── IMesEapAdapter.cs
│   │   │   └── Dto/
│   │   ├── Wms/
│   │   │   ├── IMesWmsAdapter.cs
│   │   │   └── Dto/
│   │   ├── Qms/
│   │   │   ├── IMesQmsAdapter.cs
│   │   │   └── Dto/
│   │   ├── Oa/
│   │   │   ├── IMesOaAdapter.cs
│   │   │   └── Dto/
│   │   ├── CustomerPortal/
│   │   │   ├── IMesCustomerPortalAdapter.cs
│   │   │   └── Dto/
│   │   └── AdapterResult.cs              # 统一返回结构
│   │   └── AdapterException.cs           # 统一异常定义
│   │
│   ├── Adapters.Mock/                     # Mock 实现层
│   │   ├── Adapters.Mock.csproj
│   │   ├── MockErpAdapter.cs
│   │   ├── MockEapAdapter.cs
│   │   ├── MockWmsAdapter.cs
│   │   ├── MockQmsAdapter.cs
│   │   ├── MockOaAdapter.cs
│   │   ├── MockCustomerPortalAdapter.cs
│   │   ├── MockData/                     # Mock 数据文件
│   │   │   ├── erp_orders.json
│   │   │   ├── erp_boms.json
│   │   │   ├── eap_equipment_status.json
│   │   │   ├── wms_inventory.json
│   │   │   ├── qms_standards.json
│   │   │   └── portal_data.json
│   │   └── ServiceCollectionExtensions.cs # 注册扩展
│   │
│   └── Adapters.Real/                     # 真实实现层（预留）
│       ├── Adapters.Real.csproj
│       ├── Erp/
│       │   ├── SapErpAdapter.cs           # 示例：SAP ERP 实现
│       │   └── KingdeeErpAdapter.cs       # 示例：金蝶 ERP 实现
│       ├── Eap/
│       │   ├── SecsGemEapAdapter.cs       # SECS/GEM 协议实现
│       │   └── OpcUaEapAdapter.cs         # OPC UA 协议实现
│       ├── Wms/
│       ├── Qms/
│       ├── Oa/
│       │   ├── FeishuOaAdapter.cs         # 飞书审批实现
│       │   ├── DingTalkOaAdapter.cs       # 钉钉审批实现
│       │   └── WeComOaAdapter.cs          # 企业微信实现
│       └── CustomerPortal/
│
└── MES.Api/                               # 现有 API 项目
    ├── Controllers/
    │   └── IntegrationController.cs       # 集成层管理接口
    └── appsettings.json                   # 适配器配置
```

### 3.3 每个适配器接口的标准方法定义

```csharp
// 统一返回结构
public class AdapterResult<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; }
    public string SourceSystem { get; set; }
}

// 统一接口基类约定（各适配器遵循此模式）
public interface IMesAdapter
{
    /// <summary>
    /// 健康检查：验证外部系统连通性
    /// </summary>
    Task<AdapterResult<HealthStatus>> HealthCheckAsync();

    /// <summary>
    /// 获取适配器名称和版本
    /// </summary>
    AdapterInfo GetAdapterInfo();
}

// 示例：ERP 适配器完整接口
public interface IMesErpAdapter : IMesAdapter
{
    Task<AdapterResult<OrderSyncResult>> SyncSalesOrdersAsync(OrderSyncRequest request);
    Task<AdapterResult<BomSyncResult>> SyncBomAsync(BomSyncRequest request);
    Task<AdapterResult<MaterialSyncResult>> SyncMaterialsAsync(MaterialSyncRequest request);
    Task<AdapterResult<CompletionSyncResult>> ReportWorkOrderCompletionAsync(CompletionData data);
    Task<AdapterResult<MaterialConsumeResult>> ReportMaterialConsumeAsync(ConsumeData data);
    Task<AdapterResult<FinishedGoodsResult>> ReportFinishedGoodsReceiptAsync(ReceiptData data);
}
```

### 3.4 Mock 数据管理策略

| 策略 | 说明 |
|------|------|
| **JSON 文件存储** | Mock 数据以 JSON 文件存放在 `Adapters.Mock/MockData/` 目录 |
| **版本化** | Mock 数据文件带版本号（`erp_orders_v1.json`），随接口变更更新 |
| **场景化** | 每个接口支持正常响应、超时、异常等多种场景（通过配置切换） |
| **延迟模拟** | Mock 实现可配置响应延迟（模拟真实系统网络耗时） |
| **数据可配置** | Mock 数据支持通过 API 动态修改，无需重新编译 |
| **确定性** | Mock 数据保证同一请求返回一致结果，便于测试 |

```json
// appsettings.json 配置示例
{
  "Adapters": {
    "Erp": {
      "Provider": "Mock",
      "MockConfig": {
        "DelayMs": 200,
        "FailureRate": 0.0,
        "DataFile": "MockData/erp_orders_v1.json"
      }
    },
    "Eap": {
      "Provider": "Mock",
      "MockConfig": {
        "DelayMs": 100,
        "EquipmentCount": 80
      }
    },
    "Wms": { "Provider": "Mock" },
    "Qms": { "Provider": "Mock" },
    "Oa": { "Provider": "Mock" },
    "CustomerPortal": { "Provider": "Mock" }
  }
}
```

### 3.5 真实切换方式

| 切换方式 | 说明 |
|----------|------|
| **配置切换** | 修改 `appsettings.json` 中 `Adapters:{Name}:Provider` 值，重启生效 |
| **运行时切换** | 通过 `POST /api/integration/switch-adapter` 接口热切换（需重新初始化连接池） |
| **A/B 并行** | 支持同时运行 Mock 和 Real，对比输出差异（用于切换前验证） |
| **灰度切换** | 按业务场景灰度切换（如先切换 ERP 订单同步，再切换完工回传） |
| **回滚机制** | 切换后 10 分钟内可一键回滚到 Mock |

---

## 四、各阶段优先级排序矩阵

| 阶段 | 模块 | 优先级 | 依赖关系 | 预估复杂度 | 对应需求 |
|------|------|--------|---------|-----------|---------|
| 一 | IQC 来料检验 | P0 | 无 | 中 | REQ-QM-001, REQ-02-US-001 |
| 一 | FQC/OQC 终检 | P0 | IQC | 中 | REQ-QM-005, REQ-11-US-001 |
| 一 | 不合格品/MRB | P0 | IQC | 高 | REQ-QM-006 |
| 一 | 原材料入库/批次 | P0 | IQC | 中 | REQ-WM-001 |
| 一 | FIFO 先进先出 | P0 | 原材料入库 | 低 | REQ-WM-002 |
| 一 | 物料有效期管理 | P0 | 原材料入库 | 中 | REQ-WM-003 |
| 一 | 产线发料/退料 | P0 | FIFO, 有效期 | 中 | REQ-WM-004 |
| 一 | 成品入库 | P0 | FQC/OQC | 中 | REQ-WM-006 |
| 一 | 生产异常/停线 | P0 | 无 | 中 | REQ-PO-011, REQ-99-PD-001 |
| 一 | 设备故障/保养 | P0 | 设备台账(已有) | 中 | REQ-EQ-002, REQ-EQ-003 |
| 一 | 首件检验流程 | P0 | 无 | 中 | REQ-QM-002, REQ-03-US-002 |
| 一 | 紧急质量召回 | P0 | 追溯(已有) | 高 | REQ-QM-011 |
| 二 | 订单评审流程 | P0 | 无 | 高 | REQ-01-PD-001, REQ-SL-001 |
| 二 | 主生产计划/产能 | P0 | 订单评审 | 高 | REQ-PP-001, REQ-SL-002 |
| 二 | 工单分解/MRP | P0 | 主生产计划 | 高 | REQ-PP-002, REQ-PP-004 |
| 二 | 订单进度查询 | P1 | 工单分解 | 低 | REQ-SL-003 |
| 二 | 交付达成率 OTD | P1 | 订单进度 | 低 | REQ-PP-006 |
| 二 | 插单处理 | P1 | 主生产计划 | 高 | REQ-SL-004, REQ-01-SS-001 |
| 三 | 工序参数管控 | P0 | Recipe(已有) | 高 | REQ-03-US-001, REQ-05-US-001 |
| 三 | Bin 管理 | P0 | 无 | 中 | REQ-PO-009 |
| 三 | 金线/铜线切换 | P1 | 工序参数 | 中 | REQ-05-PD-001 |
| 三 | 模具/刀片寿命 | P1 | 设备台账(已有) | 中 | REQ-03-PD-001, REQ-06-US-001 |
| 三 | 操作员资质校验 | P1 | RBAC(已有) | 中 | REQ-PO-012, REQ-99-US-002 |
| 三 | 首件确认深化 | P1 | 首件检验(阶段一) | 低 | REQ-05-CD-001 |
| 四 | ERP 集成 | P0 | 订单评审, MRP | 高 | REQ-IS-001 |
| 四 | EAP 集成 | P0 | 工序参数管控 | 高 | REQ-IS-002, REQ-EQ-007 |
| 四 | WMS 集成 | P1 | 仓储管理(阶段一) | 中 | REQ-IS-003 |
| 四 | QMS 集成 | P1 | 质量管理(阶段一) | 中 | REQ-IS-004 |
| 四 | OA 审批集成 | P2 | 订单评审, MRB | 中 | REQ-IS-006 |
| 四 | 客户门户集成 | P1 | 订单进度查询 | 高 | REQ-IS-005 |
| 五 | KPI 看板 | P0 | 各模块数据 | 中 | REQ-MG-001 |
| 五 | 生产成本分析 | P1 | MRP, 仓储 | 高 | REQ-MG-002 |
| 五 | 良率分析/DPPM | P1 | Bin 管理, 测试 | 中 | REQ-EN-005, REQ-QM-009 |
| 五 | 人员绩效 | P2 | 资质校验, 报工 | 低 | REQ-MG-003 |
| 五 | NPI 管理 | P1 | 工艺路线(已有) | 高 | REQ-EN-003 |
| 五 | 可靠性测试 | P1 | 质量管理(阶段一) | 中 | REQ-QM-008 |
| 五 | 报表自动生成 | P1 | 各模块数据 | 中 | REQ-MG-004 |
| 五 | 系统参数配置 | P1 | 无 | 低 | REQ-SC-002, REQ-SC-003 |
| 五 | 审计不可篡改 | P0 | AuditTrail(已有) | 高 | REQ-SC-005, REQ-99-CD-001 |

---

## 五、实施注意事项

### 5.1 数据库迁移策略

| 原则 | 说明 |
|------|------|
| **增量迁移** | 每个功能模块对应独立的迁移脚本（`V4.x.x__{module}.sql`），按阶段顺序执行 |
| **向后兼容** | 新增字段使用 `ALTER TABLE ADD COLUMN`，不修改已有字段类型；新增表不删除旧表 |
| **数据回填** | 迁移脚本包含必要的数据回填逻辑（如默认值、历史数据迁移） |
| **幂等性** | 所有迁移脚本支持重复执行（使用 `IF NOT EXISTS` / `IF NOT EXISTS COLUMN`） |
| **回滚脚本** | 每个迁移脚本附带对应的回滚脚本（`rollback_V4.x.x.sql`） |
| **版本追踪** | 使用 `schema_version` 表记录已执行的迁移版本 |
| **外键策略** | 初期暂不添加数据库级外键约束，由应用层保证数据一致性 |
| **索引优化** | 对高频查询字段（`lot_id`, `work_order_id`, `batch_no`）预先建立索引 |

**阶段迁移脚本规划：**

| 阶段 | 迁移脚本编号范围 | 预估脚本数 |
|------|-----------------|-----------|
| 阶段一 | V4.0.0 ~ V4.2.x | 5-6 个 |
| 阶段二 | V4.3.0 ~ V4.4.x | 3-4 个 |
| 阶段三 | V4.5.0 ~ V4.6.x | 3-4 个 |
| 阶段四 | V4.7.0 ~ V4.7.x | 1-2 个（仅配置表） |
| 阶段五 | V4.8.0 ~ V4.10.x | 5-6 个 |

### 5.2 测试策略

| 测试类型 | 范围 | 工具建议 | 覆盖率目标 |
|----------|------|---------|-----------|
| **单元测试** | Service 层业务逻辑 | xUnit + Moq | ≥70% |
| **集成测试** | Controller + Service + DB 端到端 | xUnit + TestContainers (MySQL) | 核心流程 100% |
| **Mock 适配器测试** | 各 Mock 实现正确性 | xUnit | 100% |
| **接口契约测试** | Adapter 接口一致性 | Pact 或自定义契约测试 | 100% |
| **压力测试** | 高频查询接口（追溯、KPI） | k6 或 JMeter | 响应时间 <3s |
| **回归测试** | 每阶段完成后全量回归 | 自动化 API 测试集 | P0 用例 100% |

**关键测试场景：**

1. IQC→合格入库→领料→生产→FQC→成品入库→OQC→出货 全链路
2. 异常上报→停线→处理→恢复 全流程
3. 订单评审→工单创建→工单分解→MRP 计算 全流程
4. 追溯查询响应时间 <3 秒（百万级数据量）
5. Mock 适配器切换为 Real 后接口兼容性验证

### 5.3 代码规范建议

| 规范 | 说明 |
|------|------|
| **命名约定** | Controller 以 `Controller` 结尾，Service 以 `Service` 结尾，接口以 `I` 开头 |
| **适配器命名** | 接口 `IMes{System}Adapter`，实现 `{Provider}{System}Adapter` |
| **分层架构** | Controller → Service → Gateway/Repository，禁止跨层调用 |
| **DTO 管理** | 每个模块独立的 `Dtos/` 目录，禁止直接暴露领域模型给 Controller |
| **异常处理** | 统一使用 `BusinessException` + 错误码枚举，全局异常中间件处理 |
| **审计日志** | 使用 `[AuditLog]` 特性标记需要审计的 Controller Action |
| **API 版本** | URL 中包含版本号 `/api/v1/...`，重大变更升级版本号 |
| **Swagger** | 每个 API 必须有 `[Summary]` 和 `[Response]` 注解 |
| **数据库操作** | 使用 Dapper + 参数化查询，禁止字符串拼接 SQL |
| **事务管理** | 跨表操作使用 `IDbTransaction`，长事务控制在 10 秒内 |

### 5.4 阶段性验收标准

#### 阶段一验收标准

- [ ] IQC 检验任务可创建、执行、判定，合格率/不合格率统计正确
- [ ] 不合格品自动隔离，MRB 评审流程完整可追溯
- [ ] FIFO 发料强制执行，跳过需审批记录
- [ ] 物料过期自动锁定，预警列表正确
- [ ] 发料/退料库存账实时同步
- [ ] 成品入库自动触发（FQC 合格后）
- [ ] 异常上报响应 <1 秒，停线指令执行 <3 秒
- [ ] 设备故障/保养流程闭环，MTBF/MTTR 计算正确
- [ ] 首件检验双人签名，不合格工单锁定
- [ ] Quality Alert 发布后 30 秒内通知，受影响批次自动冻结

#### 阶段二验收标准

- [ ] 订单评审工作流可配置，超时自动升级
- [ ] 主生产计划生成时间 <5 分钟，瓶颈工序自动标红
- [ ] 工单分解正确继承订单信息，MRP 计算准确率 >99%
- [ ] 订单进度查询响应 <2 秒
- [ ] OTD 自动计算准确，延迟原因分类正确
- [ ] 插单影响分析生成 <30 秒，审批后自动更新排产

#### 阶段三验收标准

- [ ] 各工序参数自动加载，越权修改被拦截
- [ ] Bin 定义可配置（支持 64 个），分选记录完整
- [ ] 金线/铜线切换强制参数确认和首件检验
- [ ] 模具/刀片/劈刀寿命计数准确，到期自动锁定
- [ ] 操作员资质校验在开工前自动执行，过期禁止操作
- [ ] 首件确认流程完整，焊线拉力测试数据记录

#### 阶段四验收标准

- [ ] 6 个 Adapter 接口定义完整，Mock 实现可正常响应
- [ ] Mock 数据覆盖正常/异常/超时多种场景
- [ ] 配置切换 Mock↔Real 生效，无需重新编译
- [ ] 适配器健康检查接口可用
- [ ] 接口异常自动告警并重试
- [ ] 数据对账机制可用，差异可追踪

#### 阶段五验收标准

- [ ] KPI 看板数据刷新 <3 秒，支持下钻
- [ ] 成本分析按产品/工单/月份维度正确
- [ ] 良率趋势、缺陷柏拉图、DPPM 统计准确
- [ ] NPI 项目全流程可追踪
- [ ] 可靠性测试计划自动触发，失败触发 FA
- [ ] 审计日志哈希校验通过，不可篡改机制有效
- [ ] 报表自动生成并定时推送

---

## 附录 A：需求来源对照表

| 本文档模块 | 规格说明书需求 | 扩展分析需求 |
|-----------|-------------|-------------|
| IQC 来料检验 | REQ-02-US-001, REQ-02-CD-001, REQ-02-EF-001 | REQ-QM-001 |
| FQC/OQC 终检 | REQ-11-US-001, REQ-11-CD-001 | REQ-QM-005 |
| 不合格品/MRB | - | REQ-QM-006 |
| 原材料入库/仓储 | REQ-02-US-001 | REQ-WM-001~004, REQ-WM-006 |
| 生产异常/停线 | REQ-99-PD-001 | REQ-PO-011 |
| 设备故障/保养 | REQ-03-PD-001 | REQ-EQ-002, REQ-EQ-003 |
| 首件检验 | REQ-03-US-002 | REQ-QM-002 |
| 紧急质量召回 | - | REQ-QM-011 |
| 订单评审 | REQ-01-PD-001 | REQ-SL-001 |
| 主生产计划/产能 | - | REQ-PP-001, REQ-SL-002 |
| 工单分解/MRP | REQ-01-PA-001 | REQ-PP-002, REQ-PP-004 |
| 订单进度/OTD | REQ-01-PA-001 | REQ-SL-003, REQ-PP-006 |
| 插单处理 | REQ-01-SS-001 | REQ-SL-004 |
| 工序参数管控 | REQ-03-US-001, REQ-05-US-001, REQ-06-CD-001 | REQ-EN-002 |
| Bin 管理 | REQ-09-PA-001 | REQ-PO-009 |
| 金线/铜线切换 | REQ-05-PD-001 | REQ-PO-004 |
| 模具/寿命管理 | REQ-03-PD-001, REQ-06-US-001 | REQ-PO-005 |
| 操作员资质 | REQ-99-US-002 | REQ-PO-012 |
| 六大集成 | REQ-11-PD-001 | REQ-IS-001~006 |
| KPI 看板 | REQ-99-SS-001 | REQ-MG-001 |
| 成本分析 | - | REQ-MG-002 |
| 良率分析/DPPM | REQ-03-SS-001 | REQ-EN-005, REQ-QM-009 |
| NPI 管理 | - | REQ-EN-003 |
| 可靠性测试 | - | REQ-QM-008 |
| 报表生成 | - | REQ-MG-004 |
| 系统参数配置 | - | REQ-SC-002, REQ-SC-003 |
| 审计不可篡改 | REQ-99-CD-001, REQ-09-CD-001 | REQ-SC-005, REQ-TR-007 |

---

## 附录 B：与现有代码资产的关系

| 现有资产 | 状态 | 本计划利用方式 |
|---------|------|--------------|
| TrackController + TrackService | 已实现 | 工序过站基础可复用，阶段三深化参数管控 |
| GenealogyService + GenealogyView | 已实现 | 追溯基础可复用，阶段五扩展实时追溯性能 |
| SpcController + SpcService | 已实现 | SPC 功能已可用，阶段一 IQC 可复用检验录入模式 |
| Complaint8DService | 已实现 | 8D 流程已完整，阶段一 MRB 可复用审批流模式 |
| Recipe 模块 | 已实现 | 版本管理/审批/参数管理可复用，阶段三深化工序参数 |
| EngineeringChange (ECN) | 已实现 | 变更管理已完整，阶段五审计不可篡改在此基础上扩展 |
| RolesController + PermissionService | 已实现 | RBAC 已可用，阶段三资质校验在此基础上扩展 |
| EquipmentController | 部分实现 | 设备台账/列表可用，阶段一补充故障/保养 API |
| Quality 模块 UI（14 个页面） | 前端完整 | 阶段一后端 API 完成后直接对接 |
| Warehouse 模块 UI | 前端存在 | 阶段一后端 API 完成后直接对接 |
| EHS 模块 UI | 前端存在 | 阶段一可复用环境监测部分 |
| ReportCenter 模块 | 部分实现 | 阶段五报表生成在此基础上扩展 |

---

> **文档结束**  
> **文档版本：** V1.0  
> **创建日期：** 2026-06-06  
> **维护说明：** 本计划为 OUTLINE 级别文档，每个阶段实施前需输出详细设计文档（包含完整 API 定义、数据表 DDL、时序图、状态机图等）  
> **下次更新：** 阶段一实施完成后，重新评估需求覆盖率并更新后续阶段预估
