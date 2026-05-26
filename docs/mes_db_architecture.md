### 5.8 完整数据库架构（基于 MFGID 为最小追溯单位）

**架构设计原则：**

1. **批次层级管理**：Wafer Lot → Mother Lot → Sub Lot → Grade Lot → MFGID
2. **MFGID 为最小追溯单位**：Reel/Tape Level，ERP 财务核算基础
3. **批次与 MFGID 分离**：Lot 是管理单位，MFGID 是追溯单位
4. **完整追溯链**：从 MFGID 可以追溯到 Wafer Lot、工单、产品、客户
5. **Hold 维度扩展**：支持 9 种 Hold 维度（PO/WaferLot/Product/Package/Equipment/Step/Bin/Lot/Route）

**表分类总览：**

| 分类 | 表数量 | 说明 |
|------|--------|------|
| 系统基础表 | 7 | 部门、角色、用户、权限、签名等 |
| 主数据表 | 8 | 产品、路线、工序、设备、载具、配方等 |
| **来料管理表** | **3** | **Wafer Lot、Wafer Map、IQC 检验（新增）** |
| 批次管理表 | 1 | ProdLot（原有，需扩展字段） |
| MFGID 表 | 3 | MfgUnit、MfgOperationHistory、MfgPackTrace（新增） |
| Hold 管理表 | 2 | HoldRecord（扩展）、AutoHoldRule（新增） |
| 追溯链表 | 3 | LotTraceChain（新增）、LotSplit/Merge（原有） |
| 其他表 | 25 | 工单、步骤、操作历史、质量门、物料、报表等（原有） |
| **总计** | **51** | **新增 8 张，修改 2 张，删除 0 张** |

#### 批次层级字段定义（ProdLot 表扩展）

```sql
-- 批次层级：Wafer/Mother/Sub/Grade
ALTER TABLE `prod_lot` ADD COLUMN `lot_level` VARCHAR(20) DEFAULT 'Mother'
  COMMENT '批次层级：Wafer(来料), Mother(前道母批), Sub(后道子批), Grade(等级批次)';

-- 层级路径：完整追溯链
ALTER TABLE `prod_lot` ADD COLUMN `lot_hierarchy_path` VARCHAR(500) DEFAULT ''
  COMMENT '层级路径：如 "WF-20260526-001/LOT-20260001/LOT-20260001-T1/LOT-20260001-GA"';

-- 根晶圆批次 ID
ALTER TABLE `prod_lot` ADD COLUMN `root_wafer_lot_id` VARCHAR(50) DEFAULT ''
  COMMENT '根晶圆批次 ID: 始终指向最顶层的 Wafer Lot';

-- 工艺阶段：Assemble/Test/Finished
ALTER TABLE `prod_lot` ADD COLUMN `process_stage` VARCHAR(20) DEFAULT 'Assemble'
  COMMENT '工艺阶段：Assemble(前道), Test(后道), Finished(成品)';

-- 母批次 ID
ALTER TABLE `prod_lot` ADD COLUMN `mother_lot_id` VARCHAR(50) DEFAULT NULL
  COMMENT '母批次 ID: Split 后子批指向母批';

-- 完成时间
ALTER TABLE `prod_lot` ADD COLUMN `completed_at` DATETIME NULL
  COMMENT '完成时间：所有工序完成的时间';

-- 入库时间
ALTER TABLE `prod_lot` ADD COLUMN `in_warehouse_at` DATETIME NULL
  COMMENT '入库时间：成品仓入库完成的时间';

-- 出货时间
ALTER TABLE `prod_lot` ADD COLUMN `shipped_at` DATETIME NULL
  COMMENT '出货时间：MES 生命周期终点';

CREATE INDEX `idx_lot_level` ON `prod_lot`(`lot_level`);
CREATE INDEX `idx_root_wafer_lot` ON `prod_lot`(`root_wafer_lot_id`);
CREATE INDEX `idx_process_stage` ON `prod_lot`(`process_stage`);
```

#### Hold 维度字段（HoldRecord 表扩展）

```sql
-- Hold 范围：9 种维度
ALTER TABLE `prod_hold_record` ADD COLUMN `hold_scope` VARCHAR(20) DEFAULT 'Lot'
  COMMENT 'Hold 范围：WorkOrder/WaferLot/Product/Package/Equipment/Step/Bin/Lot/Route';

-- 范围 ID
ALTER TABLE `prod_hold_record` ADD COLUMN `scope_id` VARCHAR(50) DEFAULT ''
  COMMENT '范围 ID: 根据 HoldScope 存储对应的 ID (OrderId/WaferLotId/ProductId 等)';

-- Hold 优先级
ALTER TABLE `prod_hold_record` ADD COLUMN `hold_priority` INT DEFAULT 9
  COMMENT 'Hold 优先级：1(PO Hold) ~ 9(Lot Hold), 数字越小优先级越高';

-- 是否级联
ALTER TABLE `prod_hold_record` ADD COLUMN `is_cascade` TINYINT(1) DEFAULT 0
  COMMENT '是否级联 Hold: 1=自动 Hold 关联批次，0=仅 Hold 当前批次';

CREATE INDEX `idx_hold_scope` ON `prod_hold_record`(`hold_scope`);
CREATE INDEX `idx_scope_id` ON `prod_hold_record`(`scope_id`);
CREATE INDEX `idx_hold_priority` ON `prod_hold_record`(`hold_priority`);
```

#### 新增来料管理表（3 张）

```sql
-- 3.1 来料晶圆批次表
CREATE TABLE `wafer_lot` (
  `wafer_lot_id` VARCHAR(50) PRIMARY KEY COMMENT '来料批次号：WF-YYYYMMDD-NNN',
  `wafer_id` VARCHAR(50) NOT NULL COMMENT '晶圆 ID',
  `wafer_fab` VARCHAR(100) DEFAULT '' COMMENT '晶圆厂',
  `wafer_size` VARCHAR(20) DEFAULT '' COMMENT '晶圆尺寸：6inch/8inch/12inch',
  `total_die_count` INT DEFAULT 0 COMMENT '总 Die 数量',
  `good_die_count` INT DEFAULT 0 COMMENT '良品 Die 数量',
  `product_id` VARCHAR(50) DEFAULT '' COMMENT '产品 ID',
  `order_id` VARCHAR(50) DEFAULT '' COMMENT '关联工单号',
  `status` VARCHAR(20) DEFAULT 'Received' COMMENT '状态：Received/Inspected/Released/Consumed',
  `iqc_result` VARCHAR(20) DEFAULT '' COMMENT 'IQC 检验结果',
  `iqc_inspector` VARCHAR(50) DEFAULT '' COMMENT 'IQC 检验员',
  `iqc_inspected_at` DATETIME NULL COMMENT 'IQC 检验时间',
  `released_at` DATETIME NULL COMMENT '释放到生产的时间',
  `consumed_at` DATETIME NULL COMMENT '全部消耗完成时间',
  `remark` VARCHAR(500) DEFAULT '' COMMENT '备注',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `created_by` VARCHAR(50) DEFAULT '',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` VARCHAR(50) DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE INDEX `idx_wafer_lot_status` ON `wafer_lot`(`status`);
CREATE INDEX `idx_wafer_lot_product` ON `wafer_lot`(`product_id`);
CREATE INDEX `idx_wafer_lot_order` ON `wafer_lot`(`order_id`);

-- 3.2 Wafer Map 表
CREATE TABLE `wafer_map` (
  `map_id` VARCHAR(50) PRIMARY KEY COMMENT 'Map ID',
  `wafer_lot_id` VARCHAR(50) NOT NULL COMMENT '来料批次号',
  `wafer_id` VARCHAR(50) NOT NULL COMMENT '晶圆 ID',
  `map_data` JSON COMMENT 'Map 数据：JSON 格式存储每个 Die 的坐标和状态',
  `total_die` INT DEFAULT 0 COMMENT '总 Die 数',
  `good_die` INT DEFAULT 0 COMMENT '良品 Die 数',
  `bad_die` INT DEFAULT 0 COMMENT '不良 Die 数',
  `map_format` VARCHAR(20) DEFAULT '' COMMENT 'Map 格式：CSV/XML/JSON',
  `imported_by` VARCHAR(50) DEFAULT '' COMMENT '导入人',
  `imported_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '导入时间',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE INDEX `idx_wafer_map_lot` ON `wafer_map`(`wafer_lot_id`);

-- 3.3 来料检验记录表
CREATE TABLE `incoming_inspection` (
  `inspection_id` VARCHAR(50) PRIMARY KEY COMMENT '检验记录 ID',
  `wafer_lot_id` VARCHAR(50) NOT NULL COMMENT '来料批次号',
  `inspection_type` VARCHAR(20) DEFAULT 'IQC' COMMENT '检验类型：IQC/IPQC/OQC',
  `inspection_result` VARCHAR(20) DEFAULT '' COMMENT '检验结果：Pass/Fail/Conditional',
  `defect_code` VARCHAR(50) DEFAULT '' COMMENT '缺陷代码',
  `defect_description` VARCHAR(500) DEFAULT '' COMMENT '缺陷描述',
  `inspector_id` VARCHAR(50) DEFAULT '' COMMENT '检验员 ID',
  `inspector_name` VARCHAR(50) DEFAULT '' COMMENT '检验员姓名',
  `inspected_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '检验时间',
  `remark` VARCHAR(500) DEFAULT '' COMMENT '备注',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE INDEX `idx_incoming_inspection_lot` ON `incoming_inspection`(`wafer_lot_id`);
```

#### 新增 MFGID 表（3 张）

```sql
-- 5.1 MFG 最小追溯单位表
CREATE TABLE `mfg_unit` (
  `mfg_id` VARCHAR(50) PRIMARY KEY COMMENT 'MFGID: 最小追溯单位唯一标识',
  `lot_id` VARCHAR(50) NOT NULL COMMENT '当前所属批次 ID',
  `root_wafer_lot_id` VARCHAR(50) NOT NULL COMMENT '根晶圆批次 ID',
  `wafer_id` VARCHAR(50) DEFAULT '' COMMENT '晶圆 ID',
  `die_x` INT DEFAULT -1 COMMENT 'Die X 坐标',
  `die_y` INT DEFAULT -1 COMMENT 'Die Y 坐标',
  `serial_number` VARCHAR(50) DEFAULT '' COMMENT '序列号',
  `reel_id` VARCHAR(50) DEFAULT '' COMMENT '卷带编号',
  `reel_capacity` INT DEFAULT 0 COMMENT '卷带容量',
  `actual_qty` INT DEFAULT 0 COMMENT '实际数量',
  `status` VARCHAR(20) DEFAULT 'Created' COMMENT '状态：Created/Processing/Completed/Scrapped/Shipped',
  `grade` VARCHAR(10) DEFAULT '' COMMENT '等级：A/B/C',
  `bin_result` VARCHAR(20) DEFAULT '' COMMENT 'Bin 结果',
  `current_step` VARCHAR(50) DEFAULT '' COMMENT '当前工序',
  `rework_count` INT DEFAULT 0 COMMENT '重工次数',
  `pack_time` DATETIME NULL COMMENT '包装时间',
  `packed_by` VARCHAR(50) DEFAULT '' COMMENT '包装操作员',
  `box_id` VARCHAR(50) DEFAULT '' COMMENT '绑定到的 Box ID',
  `pallet_id` VARCHAR(50) DEFAULT '' COMMENT '绑定到的 Pallet ID',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `completed_at` DATETIME NULL,
  `shipped_at` DATETIME NULL,
  INDEX `idx_lot_id` (`lot_id`),
  INDEX `idx_root_wafer_lot` (`root_wafer_lot_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_grade` (`grade`),
  INDEX `idx_reel_id` (`reel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 5.2 MFG 操作历史表
CREATE TABLE `mfg_operation_history` (
  `history_id` VARCHAR(50) PRIMARY KEY,
  `mfg_id` VARCHAR(50) NOT NULL COMMENT 'MFGID',
  `lot_id` VARCHAR(50) NOT NULL COMMENT '批次 ID',
  `step_code` VARCHAR(50) NOT NULL COMMENT '工序代码',
  `operation` VARCHAR(20) COMMENT '操作：TrackIn/TrackOut/Hold/Release/Rework/Scrap',
  `equipment_id` VARCHAR(50) DEFAULT '' COMMENT '设备 ID',
  `operator_id` VARCHAR(50) DEFAULT '' COMMENT '操作员',
  `result` VARCHAR(20) DEFAULT '' COMMENT '结果：Pass/Fail',
  `test_data` JSON COMMENT '测试数据',
  `remark` VARCHAR(500) DEFAULT '',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  INDEX `idx_mfg_id` (`mfg_id`),
  INDEX `idx_lot_id` (`lot_id`),
  INDEX `idx_step_code` (`step_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 5.3 MFG 包装追溯表
CREATE TABLE `mfg_pack_trace` (
  `pack_trace_id` VARCHAR(50) PRIMARY KEY,
  `mfg_id` VARCHAR(50) NOT NULL COMMENT 'MFGID',
  `pack_level` VARCHAR(20) COMMENT '包装层级：Unit/Reel/Box/Pallet',
  `pack_id` VARCHAR(50) COMMENT '包装 ID',
  `parent_pack_id` VARCHAR(50) DEFAULT '' COMMENT '父包装 ID',
  `pack_qty` INT DEFAULT 0 COMMENT '包装数量',
  `packed_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `packed_by` VARCHAR(50) DEFAULT '' COMMENT '包装操作员',
  INDEX `idx_mfg_id` (`mfg_id`),
  INDEX `idx_pack_id` (`pack_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### 新增追溯链表（LotTraceChain）

```sql
-- 7.1 批次追溯链表
CREATE TABLE `lot_trace_chain` (
  `chain_id` VARCHAR(50) PRIMARY KEY COMMENT '追溯链 ID',
  `child_lot_id` VARCHAR(50) NOT NULL COMMENT '子批次 ID',
  `parent_lot_id` VARCHAR(50) NOT NULL COMMENT '父批次 ID',
  `relation_type` VARCHAR(20) DEFAULT '' COMMENT '关系类型：Split/Merge/GradeSplit/Rework/AssembleToTest',
  `split_qty` INT DEFAULT 0 COMMENT '拆分数量',
  `merge_source_lot_ids` VARCHAR(500) DEFAULT '' COMMENT '合并源批次 ID 列表',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `created_by` VARCHAR(50) DEFAULT '',
  INDEX `idx_child_lot` (`child_lot_id`),
  INDEX `idx_parent_lot` (`parent_lot_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### 新增自动 Hold 规则表（AutoHoldRule）

```sql
-- 6.2 自动 Hold 规则表
CREATE TABLE `auto_hold_rule` (
  `rule_id` VARCHAR(50) PRIMARY KEY COMMENT '规则 ID',
  `rule_name` VARCHAR(100) NOT NULL COMMENT '规则名称',
  `rule_type` VARCHAR(20) DEFAULT '' COMMENT '规则类型：Yield/Equipment/Quality/Step',
  `hold_scope` VARCHAR(20) DEFAULT 'Lot' COMMENT 'Hold 范围',
  `trigger_condition` JSON COMMENT '触发条件：JSON 格式',
  `hold_reason_code` VARCHAR(50) DEFAULT '' COMMENT 'Hold 原因代码',
  `hold_reason` VARCHAR(500) DEFAULT '' COMMENT 'Hold 原因',
  `auto_release` TINYINT(1) DEFAULT 0 COMMENT '是否自动释放',
  `auto_release_condition` JSON COMMENT '自动释放条件',
  `is_active` TINYINT(1) DEFAULT 1 COMMENT '是否启用',
  `priority` INT DEFAULT 5 COMMENT '优先级：1-10',
  `created_by` VARCHAR(50) DEFAULT '',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_by` VARCHAR(50) DEFAULT '',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  INDEX `idx_rule_type` (`rule_type`),
  INDEX `idx_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```
