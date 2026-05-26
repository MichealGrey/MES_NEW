## 完整数据库架构设计（MFGID 为最小追溯单位）

### 架构设计原则

1. **批次层级管理**：Wafer Lot → Mother Lot → Sub Lot → Grade Lot → MFGID
2. **MFGID 为最小追溯单位**：Reel/Tape Level，ERP 财务核算基础
3. **批次与 MFGID 分离**：Lot 是管理单位，MFGID 是追溯单位
4. **完整追溯链**：从 MFGID 可以追溯到 Wafer Lot、工单、产品、客户
5. **Hold 维度扩展**：支持 9 种 Hold 维度（PO/WaferLot/Product/Package/Equipment/Step/Bin/Lot/Route）

### 表分类

| 分类 | 表数量 | 说明 |
|------|--------|------|
| 系统基础表 | 7 | 部门、角色、用户、权限、签名等 |
| 主数据表 | 8 | 产品、路线、工序、设备、载具、配方等 |
| **来料管理表** | **3** | **Wafer Lot、Wafer Map、IQC 检验（新增）** |
| 批次管理表 | 1 | ProdLot（原有，需扩展字段） |
| 批次步骤表 | 1 | ProdLotStep（原有） |
| 操作历史表 | 2 | OperationHistory、AuditTrail（原有） |
| Hold 管理表 | 2 | HoldRecord（扩展）、AutoHoldRule（新增） |
| 追溯链表 | 2 | LotTraceChain（新增）、LotSplit/Merge（原有） |
| **MFGID 表** | **3** | **MfgUnit、MfgOperationHistory、MfgPackTrace（新增）** |
| 工单管理表 | 1 | WorkOrder（原有） |
| 质量门表 | 3 | QualityGate、Inspection、GateInstance（原有） |
| 物料管理表 | 3 | Material、Requirement、Consume（原有） |
| 报表归档表 | 4 | DailyReport、LotArchive、SystemEvent、Config（原有） |
| 外部系统表 | 2 | SystemEvent、Config（原有） |

总计：**42 张表**（新增 6 张，修改 2 张，删除 0 张）

---

### 一、系统基础表（7 张）

```sql
-- 1.1 部门表
CREATE TABLE `sys_department` (
  `dept_id` VARCHAR(50) PRIMARY KEY,
  `dept_name` VARCHAR(100) NOT NULL,
  `parent_id` VARCHAR(50) DEFAULT NULL,
  `manager_id` VARCHAR(50) DEFAULT NULL,
  `status` VARCHAR(20) DEFAULT 'Active',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 1.2 角色表
CREATE TABLE `sys_role` (
  `role_id` VARCHAR(50) PRIMARY KEY,
  `role_name` VARCHAR(100) NOT NULL,
  `description` VARCHAR(255) DEFAULT NULL,
  `level` INT DEFAULT 0,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 1.3 用户表
CREATE TABLE `sys_user` (
  `user_id` VARCHAR(50) PRIMARY KEY,
  `user_name` VARCHAR(100) NOT NULL,
  `password_hash` VARCHAR(255) NOT NULL,
  `role_id` VARCHAR(50) NOT NULL,
  `dept_id` VARCHAR(50) NOT NULL,
  `shift` VARCHAR(20) DEFAULT 'A',
  `is_active` TINYINT(1) DEFAULT 1,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`role_id`) REFERENCES `sys_role`(`role_id`),
  FOREIGN KEY (`dept_id`) REFERENCES `sys_department`(`dept_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 1.4 用户权限表
CREATE TABLE `sys_user_permission` (
  `id` BIGINT AUTO_INCREMENT PRIMARY KEY,
  `user_id` VARCHAR(50) NOT NULL,
  `permission_code` VARCHAR(100) NOT NULL,
  `granted_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY `uk_user_permission` (`user_id`, `permission_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 1.5 签名等级表
CREATE TABLE `sys_signature_level` (
  `level_id` VARCHAR(50) PRIMARY KEY,
  `level_name` VARCHAR(100) NOT NULL,
  `required_role` VARCHAR(50) NOT NULL,
  `min_level` INT NOT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 1.6 电子签名表
CREATE TABLE `sys_signature` (
  `signature_id` VARCHAR(50) PRIMARY KEY,
  `user_id` VARCHAR(50) NOT NULL,
  `password_hash` VARCHAR(255) NOT NULL,
  `level_id` VARCHAR(50) NOT NULL,
  `is_active` TINYINT(1) DEFAULT 1,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`user_id`) REFERENCES `sys_user`(`user_id`),
  FOREIGN KEY (`level_id`) REFERENCES `sys_signature_level`(`level_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 1.7 系统配置表
CREATE TABLE `sys_config` (
  `config_key` VARCHAR(100) PRIMARY KEY,
  `config_value` TEXT NOT NULL,
  `description` VARCHAR(255) DEFAULT NULL,
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` VARCHAR(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 二、主数据表（8 张）

```sql
-- 2.1 产品表
CREATE TABLE `master_product` (
  `product_id` VARCHAR(50) PRIMARY KEY,
  `product_name` VARCHAR(100) NOT NULL,
  `die_name` VARCHAR(100) DEFAULT NULL,
  `package_type` VARCHAR(50) NOT NULL,
  `customer_id` VARCHAR(50) DEFAULT NULL,
  `customer_name` VARCHAR(100) DEFAULT NULL,
  `customer_pn` VARCHAR(100) DEFAULT NULL,
  `internal_pn` VARCHAR(100) DEFAULT NULL,
  `status` VARCHAR(20) DEFAULT 'Active',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.2 工艺路线表
CREATE TABLE `master_route` (
  `route_id` VARCHAR(100) PRIMARY KEY,
  `route_name` VARCHAR(100) NOT NULL,
  `route_version` VARCHAR(20) DEFAULT '1.0',
  `product_id` VARCHAR(50) DEFAULT NULL,
  `package_type` VARCHAR(50) NOT NULL,
  `route_type` VARCHAR(20) DEFAULT 'Normal' COMMENT 'Normal/Rework',
  `is_active` TINYINT(1) DEFAULT 1,
  `is_approved` TINYINT(1) DEFAULT 0,
  `approved_by` VARCHAR(50) DEFAULT NULL,
  `approved_at` DATETIME DEFAULT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.3 工艺路线工序表
CREATE TABLE `master_route_step` (
  `id` BIGINT AUTO_INCREMENT PRIMARY KEY,
  `route_id` VARCHAR(100) NOT NULL,
  `step_seq` INT NOT NULL,
  `step_code` VARCHAR(50) NOT NULL,
  `step_name` VARCHAR(100) NOT NULL,
  `equipment_group` VARCHAR(50) NOT NULL,
  `is_rework` TINYINT(1) DEFAULT 0,
  `is_critical` TINYINT(1) DEFAULT 0,
  `is_key_process` TINYINT(1) DEFAULT 0,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY `uk_route_step` (`route_id`, `step_seq`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.4 设备表
CREATE TABLE `master_equipment` (
  `equipment_id` VARCHAR(50) PRIMARY KEY,
  `equipment_name` VARCHAR(100) NOT NULL,
  `equipment_group` VARCHAR(50) NOT NULL,
  `equipment_type` VARCHAR(50) NOT NULL,
  `status` VARCHAR(20) DEFAULT 'Available',
  `location` VARCHAR(100) DEFAULT NULL,
  `responsible_person` VARCHAR(50) DEFAULT NULL,
  `last_maintenance_date` DATETIME DEFAULT NULL,
  `maintenance_interval_hours` INT DEFAULT 500,
  `running_hours` INT DEFAULT 0,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.5 设备路线关联表
CREATE TABLE `master_equipment_route` (
  `id` BIGINT AUTO_INCREMENT PRIMARY KEY,
  `equipment_id` VARCHAR(50) NOT NULL,
  `route_id` VARCHAR(100) NOT NULL,
  `step_code` VARCHAR(50) NOT NULL,
  `is_qualified` TINYINT(1) DEFAULT 1,
  `qualified_at` DATETIME DEFAULT NULL,
  `qualified_by` VARCHAR(50) DEFAULT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY `uk_equipment_route_step` (`equipment_id`, `route_id`, `step_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.6 载具表
CREATE TABLE `master_carrier` (
  `carrier_id` VARCHAR(50) PRIMARY KEY,
  `carrier_type` VARCHAR(50) NOT NULL,
  `carrier_name` VARCHAR(100) NOT NULL,
  `max_capacity` INT NOT NULL,
  `status` VARCHAR(20) DEFAULT 'Available',
  `location` VARCHAR(100) DEFAULT NULL,
  `last_calibration_date` DATETIME DEFAULT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.7 配方表
CREATE TABLE `master_recipe` (
  `recipe_id` VARCHAR(100) PRIMARY KEY,
  `recipe_name` VARCHAR(100) NOT NULL,
  `product_id` VARCHAR(50) DEFAULT NULL,
  `route_id` VARCHAR(100) DEFAULT NULL,
  `step_code` VARCHAR(50) NOT NULL,
  `equipment_id` VARCHAR(50) NOT NULL,
  `version` VARCHAR(20) DEFAULT '1.0',
  `is_active` TINYINT(1) DEFAULT 1,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.8 良率规则表
CREATE TABLE `master_yield_rule` (
  `rule_id` VARCHAR(50) PRIMARY KEY,
  `rule_name` VARCHAR(100) NOT NULL,
  `product_id` VARCHAR(50) DEFAULT NULL,
  `step_code` VARCHAR(50) DEFAULT NULL,
  `yield_threshold` DECIMAL(5,2) NOT NULL,
  `action` VARCHAR(50) NOT NULL COMMENT 'Hold/Alarm/Notify',
  `is_active` TINYINT(1) DEFAULT 1,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### 三、来料管理表（新增 3 张）

```sql
-- 3.1 来料晶圆批次表（新增）
CREATE TABLE `wafer_lot` (
  `wafer_lot_id` VARCHAR(50) PRIMARY KEY COMMENT '来料批次号：WF-YYYYMMDD-NNN',
  `wafer_id` VARCHAR(50) NOT NULL COMMENT '晶圆 ID',
  `wafer_fab` VARCHAR(100) DEFAULT '' COMMENT '晶圆厂',
  `wafer_size` VARCHAR(20) DEFAULT '' COMMENT '晶圆尺寸：6inch/8inch/12inch',
  `total_die_count` INT DEFAULT 0 COMMENT '总 Die 数量',
  `good_die_count` INT DEFAULT 0 COMMENT '良品 Die 数量 (根据 Wafer Map)',
  `product_id` VARCHAR(50) DEFAULT '' COMMENT '产品 ID',
  `order_id` VARCHAR(50) DEFAULT '' COMMENT '关联工单号',
  `status` VARCHAR(20) DEFAULT 'Received' COMMENT '状态：Received/Inspected/Released/Consumed',
  `iqc_result` VARCHAR(20) DEFAULT '' COMMENT 'IQC 检验结果：Pass/Fail/Pending',
  `iqc_inspector` VARCHAR(50) DEFAULT '' COMMENT 'IQC 检验员',
  `iqc_inspected_at` DATETIME NULL COMMENT 'IQC 检验时间',
  `released_at` DATETIME NULL COMMENT '释放到生产的时间',
  `consumed_at` DATETIME NULL COMMENT '全部消耗完成时间',
  `remark` VARCHAR(500) DEFAULT '' COMMENT '备注',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `created_by` VARCHAR(50) DEFAULT '',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` VARCHAR(50) DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='来料晶圆批次表';

CREATE INDEX `idx_wafer_lot_status` ON `wafer_lot`(`status`);
CREATE INDEX `idx_wafer_lot_product` ON `wafer_lot`(`product_id`);
CREATE INDEX `idx_wafer_lot_order` ON `wafer_lot`(`order_id`);

-- 3.2 Wafer Map 表（新增）
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='Wafer Map 表';

CREATE INDEX `idx_wafer_map_lot` ON `wafer_map`(`wafer_lot_id`);

-- 3.3 来料检验记录表（新增）
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='来料检验记录表';

CREATE INDEX `idx_incoming_inspection_lot` ON `incoming_inspection`(`wafer_lot_id`);
```

### 四、批次管理表（修改 1 张）

```sql
-- 4.1 批次表（扩展字段）
CREATE TABLE `prod_lot` (
  -- === 基础字段（保留原有）===
  `lot_id` VARCHAR(50) NOT NULL,
  `order_id` VARCHAR(50) NOT NULL,
  `product_id` VARCHAR(50) NOT NULL,
  `product_name` VARCHAR(100) NOT NULL,
  `die_name` VARCHAR(100) DEFAULT NULL,
  `package_type` VARCHAR(50) NOT NULL,
  `route_id` VARCHAR(100) NOT NULL,
  `route_version` VARCHAR(20) NOT NULL DEFAULT '1.0',
  `current_step_seq` INT NOT NULL DEFAULT 0,
  `current_step_code` VARCHAR(50) DEFAULT NULL,
  `status` VARCHAR(20) NOT NULL DEFAULT 'Waiting',
  `unit_count` INT NOT NULL DEFAULT 0,
  `strip_count` INT NOT NULL DEFAULT 0,
  `priority` VARCHAR(20) NOT NULL DEFAULT 'Normal',
  `carrier_type` VARCHAR(50) DEFAULT NULL,
  `carrier_id` VARCHAR(50) DEFAULT NULL,
  `wafer_lot_id` VARCHAR(50) DEFAULT NULL,
  
  -- === 新增批次层级字段 ===
  `lot_level` VARCHAR(20) DEFAULT 'Mother' COMMENT '批次层级：Wafer/Mother/Sub/Grade',
  `lot_hierarchy_path` VARCHAR(500) DEFAULT '' COMMENT '层级路径：如 "WF-20260526-001/LOT-20260001/LOT-20260001-T1"',
  `root_wafer_lot_id` VARCHAR(50) DEFAULT '' COMMENT '根晶圆批次 ID',
  `process_stage` VARCHAR(20) DEFAULT 'Assemble' COMMENT '工艺阶段：Assemble/Test/Finished',
  `mother_lot_id` VARCHAR(50) DEFAULT NULL COMMENT '母批次 ID',
  
  -- === 新增数量字段 ===
  `original_qty` INT NOT NULL DEFAULT 0 COMMENT '原始数量',
  `total_pass_qty` INT NOT NULL DEFAULT 0 COMMENT '累计合格数量',
  `total_scrap_qty` INT NOT NULL DEFAULT 0 COMMENT '累计报废数量',
  `total_rework_qty` INT NOT NULL DEFAULT 0 COMMENT '累计重工数量',
  `total_hold_qty` INT NOT NULL DEFAULT 0 COMMENT '累计 Hold 数量',
  
  -- === 新增 Grade/Bin 字段 ===
  `bin_result` VARCHAR(50) DEFAULT NULL COMMENT 'Bin 结果',
  `grade` VARCHAR(20) DEFAULT NULL COMMENT '等级：A/B/C',
  `test_result` VARCHAR(50) DEFAULT NULL COMMENT '测试结果',
  `qty_pass` INT NOT NULL DEFAULT 0 COMMENT '良品数',
  `qty_fail` INT NOT NULL DEFAULT 0 COMMENT '不良数',
  
  -- === 新增重工字段 ===
  `is_rework_lot` TINYINT(1) NOT NULL DEFAULT 0,
  `original_route_id` VARCHAR(100) DEFAULT NULL,
  `rework_route_id` VARCHAR(100) DEFAULT NULL,
  `rework_count` INT DEFAULT NULL,
  `rework_reason` VARCHAR(255) DEFAULT NULL,
  
  -- === 新增 MRB 字段 ===
  `is_under_mrb` TINYINT(1) NOT NULL DEFAULT 0,
  `mrb_reference` VARCHAR(50) DEFAULT NULL,
  `mrb_disposition` VARCHAR(50) DEFAULT NULL,
  
  -- === 新增时间字段 ===
  `completed_at` DATETIME NULL COMMENT '完成时间',
  `in_warehouse_at` DATETIME NULL COMMENT '入库时间',
  `shipped_at` DATETIME NULL COMMENT '出货时间',
  
  -- === 原有字段（保留）===
  `is_partial_lot` TINYINT(1) NOT NULL DEFAULT 0,
  `split_reason` VARCHAR(255) DEFAULT NULL,
  `split_time` DATETIME DEFAULT NULL,
  `split_qty` INT DEFAULT NULL,
  `hold_category` VARCHAR(50) DEFAULT NULL,
  `hold_reason` VARCHAR(255) DEFAULT NULL,
  `hold_time` DATETIME DEFAULT NULL,
  `hold_operator` VARCHAR(50) DEFAULT NULL,
  `release_condition` VARCHAR(255) DEFAULT NULL,
  `is_archived` TINYINT(1) NOT NULL DEFAULT 0,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  
  PRIMARY KEY (`lot_id`),
  INDEX `idx_order_id` (`order_id`),
  INDEX `idx_product_id` (`product_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_current_step` (`current_step_code`),
  INDEX `idx_lot_level` (`lot_level`),
  INDEX `idx_root_wafer_lot` (`root_wafer_lot_id`),
  INDEX `idx_process_stage` (`process_stage`),
  INDEX `idx_mother_lot` (`mother_lot_id`),
  INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='生产批次表';
```

### 五、MFGID 表（新增 3 张）

```sql
-- 5.1 MFG 最小追溯单位表（新增）
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
  `completed_at` DATETIME NULL COMMENT '完成时间',
  `shipped_at` DATETIME NULL COMMENT '出货时间',
  INDEX `idx_lot_id` (`lot_id`),
  INDEX `idx_root_wafer_lot` (`root_wafer_lot_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_grade` (`grade`),
  INDEX `idx_reel_id` (`reel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='MFG 最小追溯单位表';

-- 5.2 MFG 操作历史表（新增）
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='MFG 操作历史表';

-- 5.3 MFG 包装追溯表（新增）
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='MFG 包装追溯表';
```

### 六、Hold 管理表（扩展 1 张，新增 1 张）

```sql
-- 6.1 Hold 记录表（扩展字段）
CREATE TABLE `prod_hold_record` (
  -- === 原有字段（保留）===
  `hold_id` VARCHAR(50) NOT NULL,
  `lot_id` VARCHAR(50) NOT NULL,
  `hold_type` VARCHAR(50) NOT NULL,
  `hold_reason_code` VARCHAR(50) DEFAULT NULL,
  `hold_reason` VARCHAR(255) NOT NULL,
  `hold_qty` INT NOT NULL,
  `responsible_dept` VARCHAR(50) DEFAULT NULL,
  `owner` VARCHAR(50) DEFAULT NULL,
  `status` VARCHAR(20) NOT NULL DEFAULT 'Open',
  `hold_by` VARCHAR(50) NOT NULL,
  `hold_time` DATETIME NOT NULL,
  `root_cause` TEXT DEFAULT NULL,
  `corrective_action` TEXT DEFAULT NULL,
  `disposition` VARCHAR(255) DEFAULT NULL,
  `release_by` VARCHAR(50) DEFAULT NULL,
  `release_time` DATETIME DEFAULT NULL,
  `release_comment` TEXT DEFAULT NULL,
  `approved_by` VARCHAR(50) DEFAULT NULL,
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  
  -- === 新增 Hold 维度字段 ===
  `hold_scope` VARCHAR(20) DEFAULT 'Lot' COMMENT 'Hold 范围：WorkOrder/WaferLot/Product/Package/Equipment/Step/Bin/Lot/Route',
  `scope_id` VARCHAR(50) DEFAULT '' COMMENT '范围 ID：根据 HoldScope 存储对应的 ID',
  `hold_priority` INT DEFAULT 9 COMMENT 'Hold 优先级：1(PO Hold) ~ 9(Lot Hold)',
  `is_cascade` TINYINT(1) DEFAULT 0 COMMENT '是否级联 Hold',
  
  PRIMARY KEY (`hold_id`),
  INDEX `idx_lot_id` (`lot_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_hold_scope` (`hold_scope`),
  INDEX `idx_scope_id` (`scope_id`),
  INDEX `idx_hold_priority` (`hold_priority`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 6.2 自动 Hold 规则表（新增）
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='自动 Hold 规则表';
```

### 七、追溯链表（新增 1 张，保留 2 张）

```sql
-- 7.1 批次追溯链表（新增）
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='批次追溯链表';

-- 7.2 批次拆分记录表（保留原有）
-- 7.3 批次合并记录表（保留原有）
```

### 八、其他表（保留原有）

保留以下原有表（不修改）：
- `prod_work_order` 工单表
- `prod_lot_step` 批次步骤表
- `prod_operation_history` 操作历史表
- `prod_audit_trail` 审计追踪表
- `prod_scrap_record` 报废记录表
- `prod_rework_record` 重工记录表
- `prod_lot_merge` 批次合并表
- `prod_genealogy` 批次族谱表
- `prod_carrier_binding` 载具绑定表
- `prod_dispatch_task` 调度任务表
- `quality_gate` 质量门表
- `quality_inspection` 质量检验表
- `quality_gate_instance` 质量门实例表
- `alarm_rule` 告警规则表
- `alarm_record` 告警记录表
- `customer_requirement` 客户需求表
- `report_production_daily` 生产日报表
- `prod_lot_archive` 批次归档表
- `ext_system_event` 外部系统事件表
- `ext_system_config` 外部系统配置表
- `master_material` 物料主数据表
- `material_requirement` 物料需求表
- `material_consume` 物料消耗表
- `prod_signature` 生产签名表
- `quantity_transaction` 数量交易表

---

### 总结

| 分类 | 原有表数 | 新增表数 | 修改表数 | 删除表数 | 最终表数 |
|------|---------|---------|---------|---------|---------|
| 系统基础表 | 7 | 0 | 0 | 0 | 7 |
| 主数据表 | 8 | 0 | 0 | 0 | 8 |
| 来料管理表 | 0 | 3 | 0 | 0 | 3 |
| 批次管理表 | 1 | 0 | 1 | 0 | 1 |
| MFGID 表 | 0 | 3 | 0 | 0 | 3 |
| Hold 管理表 | 1 | 1 | 1 | 0 | 2 |
| 追溯链表 | 2 | 1 | 0 | 0 | 3 |
| 其他表 | 24 | 0 | 0 | 0 | 24 |
| **总计** | **43** | **8** | **2** | **0** | **51** |

注：原 `prod_lot` 表扩展字段，`prod_hold_record` 表扩展字段，其他表保持不变。
