-- ============================================================
-- MES V3 Migration V1.0.0 - MFGID 追溯体系
-- 创建日期: 2026-05-27
-- 说明: 扩展 prod_lot 表,新增 MFG Unit/操作历史/包装追溯/追溯链/自动Hold规则 表
-- ============================================================

USE `mes_prod`;

-- ============================================================
-- 一、扩展 prod_lot 表
-- ============================================================

-- 批次层级:Wafer/Mother/Sub/Grade
ALTER TABLE `prod_lot` ADD COLUMN `lot_level` VARCHAR(20) DEFAULT 'Mother'
  COMMENT '批次层级:Wafer(来料), Mother(前道母批), Sub(后道子批), Grade(等级批次)';

-- 层级路径:完整追溯链
ALTER TABLE `prod_lot` ADD COLUMN `lot_hierarchy_path` VARCHAR(500) DEFAULT ''
  COMMENT '层级路径:如 "WF-20260526-001/LOT-20260001/LOT-20260001-T1/LOT-20260001-GA"';

-- 根晶圆批次 ID
ALTER TABLE `prod_lot` ADD COLUMN `root_wafer_lot_id` VARCHAR(50) DEFAULT ''
  COMMENT '根晶圆批次 ID: 始终指向最顶层的 Wafer Lot';

-- 工艺阶段:Assemble/Test/Finished
ALTER TABLE `prod_lot` ADD COLUMN `process_stage` VARCHAR(20) DEFAULT 'Assemble'
  COMMENT '工艺阶段:Assemble(前道), Test(后道), Finished(成品)';

-- 完成时间
ALTER TABLE `prod_lot` ADD COLUMN `completed_at` DATETIME NULL
  COMMENT '完成时间:所有工序完成的时间';

-- 入库时间
ALTER TABLE `prod_lot` ADD COLUMN `in_warehouse_at` DATETIME NULL
  COMMENT '入库时间:成品仓入库完成的时间';

-- 出货时间
ALTER TABLE `prod_lot` ADD COLUMN `shipped_at` DATETIME NULL
  COMMENT '出货时间:MES 生命周期终点';

CREATE INDEX `idx_lot_level` ON `prod_lot`(`lot_level`);
CREATE INDEX `idx_root_wafer_lot` ON `prod_lot`(`root_wafer_lot_id`);
CREATE INDEX `idx_process_stage` ON `prod_lot`(`process_stage`);

-- ============================================================
-- 二、扩展 prod_hold_record 表
-- ============================================================

-- Hold 范围:9 种维度
ALTER TABLE `prod_hold_record` ADD COLUMN `hold_scope` VARCHAR(20) DEFAULT 'Lot'
  COMMENT 'Hold 范围:WorkOrder/WaferLot/Product/Package/Equipment/Step/Bin/Lot/Route';

-- 范围 ID
ALTER TABLE `prod_hold_record` ADD COLUMN `scope_id` VARCHAR(50) DEFAULT ''
  COMMENT '范围 ID: 根据 HoldScope 存储对应的 ID (OrderId/WaferLotId/ProductId 等)';

-- Hold 优先级
ALTER TABLE `prod_hold_record` ADD COLUMN `hold_priority` INT DEFAULT 9
  COMMENT 'Hold 优先级:1(PO Hold) ~ 9(Lot Hold), 数字越小优先级越高';

-- 是否级联
ALTER TABLE `prod_hold_record` ADD COLUMN `is_cascade` TINYINT(1) DEFAULT 0
  COMMENT '是否级联 Hold: 1=自动 Hold 关联批次,0=仅 Hold 当前批次';

CREATE INDEX `idx_hold_scope` ON `prod_hold_record`(`hold_scope`);
CREATE INDEX `idx_scope_id` ON `prod_hold_record`(`scope_id`);
CREATE INDEX `idx_hold_priority` ON `prod_hold_record`(`hold_priority`);

-- ============================================================
-- 三、新增 MFG Unit 表
-- ============================================================

CREATE TABLE `mfg_unit` (
  `mfg_id` VARCHAR(50) NOT NULL COMMENT 'MFGID: 最小追溯单位唯一标识',
  `lot_id` VARCHAR(50) NOT NULL COMMENT '当前所属批次 ID',
  `root_wafer_lot_id` VARCHAR(50) NOT NULL COMMENT '根晶圆批次 ID',
  `wafer_id` VARCHAR(50) DEFAULT '' COMMENT '晶圆 ID',
  `die_x` INT DEFAULT -1 COMMENT 'Die X 坐标',
  `die_y` INT DEFAULT -1 COMMENT 'Die Y 坐标',
  `serial_number` VARCHAR(50) DEFAULT '' COMMENT '序列号',
  `reel_id` VARCHAR(50) DEFAULT '' COMMENT '卷带编号',
  `reel_capacity` INT DEFAULT 0 COMMENT '卷带容量',
  `actual_qty` INT DEFAULT 0 COMMENT '实际数量',
  `status` VARCHAR(20) DEFAULT 'Created' COMMENT '状态:Created/Processing/Completed/Scrapped/Shipped',
  `grade` VARCHAR(10) DEFAULT '' COMMENT '等级:A/B/C',
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
  PRIMARY KEY (`mfg_id`),
  INDEX `idx_lot_id` (`lot_id`),
  INDEX `idx_root_wafer_lot` (`root_wafer_lot_id`),
  INDEX `idx_status` (`status`),
  INDEX `idx_grade` (`grade`),
  INDEX `idx_reel_id` (`reel_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 四、新增 MFG 操作历史表
-- ============================================================

CREATE TABLE `mfg_operation_history` (
  `history_id` VARCHAR(50) NOT NULL,
  `mfg_id` VARCHAR(50) NOT NULL COMMENT 'MFGID',
  `lot_id` VARCHAR(50) NOT NULL COMMENT '批次 ID',
  `step_code` VARCHAR(50) NOT NULL COMMENT '工序代码',
  `operation` VARCHAR(20) COMMENT '操作:TrackIn/TrackOut/Hold/Release/Rework/Scrap',
  `equipment_id` VARCHAR(50) DEFAULT '' COMMENT '设备 ID',
  `operator_id` VARCHAR(50) DEFAULT '' COMMENT '操作员',
  `result` VARCHAR(20) DEFAULT '' COMMENT '结果:Pass/Fail',
  `test_data` JSON COMMENT '测试数据',
  `remark` VARCHAR(500) DEFAULT '',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`history_id`),
  INDEX `idx_mfg_id` (`mfg_id`),
  INDEX `idx_lot_id` (`lot_id`),
  INDEX `idx_step_code` (`step_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 五、新增 MFG 包装追溯表
-- ============================================================

CREATE TABLE `mfg_pack_trace` (
  `pack_trace_id` VARCHAR(50) NOT NULL,
  `mfg_id` VARCHAR(50) NOT NULL COMMENT 'MFGID',
  `pack_level` VARCHAR(20) COMMENT '包装层级:Unit/Reel/Box/Pallet',
  `pack_id` VARCHAR(50) COMMENT '包装 ID',
  `parent_pack_id` VARCHAR(50) DEFAULT '' COMMENT '父包装 ID',
  `pack_qty` INT DEFAULT 0 COMMENT '包装数量',
  `packed_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `packed_by` VARCHAR(50) DEFAULT '' COMMENT '包装操作员',
  PRIMARY KEY (`pack_trace_id`),
  INDEX `idx_mfg_id` (`mfg_id`),
  INDEX `idx_pack_id` (`pack_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 六、新增 Lot Trace Chain 表
-- ============================================================

CREATE TABLE `lot_trace_chain` (
  `chain_id` VARCHAR(50) NOT NULL COMMENT '追溯链 ID',
  `child_lot_id` VARCHAR(50) NOT NULL COMMENT '子批次 ID',
  `parent_lot_id` VARCHAR(50) NOT NULL COMMENT '父批次 ID',
  `relation_type` VARCHAR(20) DEFAULT '' COMMENT '关系类型:Split/Merge/GradeSplit/Rework/AssembleToTest',
  `split_qty` INT DEFAULT 0 COMMENT '拆分数量',
  `merge_source_lot_ids` VARCHAR(500) DEFAULT '' COMMENT '合并源批次 ID 列表',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `created_by` VARCHAR(50) DEFAULT '',
  PRIMARY KEY (`chain_id`),
  INDEX `idx_child_lot` (`child_lot_id`),
  INDEX `idx_parent_lot` (`parent_lot_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 七、新增自动 Hold 规则表
-- ============================================================

CREATE TABLE `auto_hold_rule` (
  `rule_id` VARCHAR(50) NOT NULL COMMENT '规则 ID',
  `rule_name` VARCHAR(100) NOT NULL COMMENT '规则名称',
  `rule_type` VARCHAR(20) DEFAULT '' COMMENT '规则类型:Yield/Equipment/Quality/Step',
  `hold_scope` VARCHAR(20) DEFAULT 'Lot' COMMENT 'Hold 范围',
  `trigger_condition` JSON COMMENT '触发条件:JSON 格式',
  `hold_reason_code` VARCHAR(50) DEFAULT '' COMMENT 'Hold 原因代码',
  `hold_reason` VARCHAR(500) DEFAULT '' COMMENT 'Hold 原因',
  `auto_release` TINYINT(1) DEFAULT 0 COMMENT '是否自动释放',
  `auto_release_condition` JSON COMMENT '自动释放条件',
  `is_active` TINYINT(1) DEFAULT 1 COMMENT '是否启用',
  `priority` INT DEFAULT 5 COMMENT '优先级:1-10',
  `created_by` VARCHAR(50) DEFAULT '',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  `updated_by` VARCHAR(50) DEFAULT '',
  `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`rule_id`),
  INDEX `idx_rule_type` (`rule_type`),
  INDEX `idx_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
