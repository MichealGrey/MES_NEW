-- ============================================================
-- MES V3 Migration V1.0.1 - Assemble/Test 数据表
-- 创建日期: 2026-05-27
-- 说明: 新增装配数据和测试数据详细记录表
-- ============================================================

USE `mes_prod`;

-- ============================================================
-- 一、新增 Assemble Data 表
-- ============================================================

CREATE TABLE `prod_assemble_data` (
  `assemble_id` VARCHAR(50) NOT NULL,
  `lot_id` VARCHAR(50) NOT NULL COMMENT '所属批次 ID',
  `mfg_id` VARCHAR(50) DEFAULT NULL COMMENT '关联的 MFG ID',
  `step_code` VARCHAR(50) NOT NULL COMMENT '工序代码',
  `step_seq` INT NOT NULL COMMENT '工序序号',
  `equipment_id` VARCHAR(50) NOT NULL COMMENT '设备 ID',
  `recipe_id` VARCHAR(100) DEFAULT NULL COMMENT '配方 ID',
  `carrier_id` VARCHAR(50) DEFAULT NULL COMMENT '载具 ID',
  `input_qty` INT NOT NULL DEFAULT 0 COMMENT '投入数量',
  `output_qty` INT NOT NULL DEFAULT 0 COMMENT '产出数量',
  `scrap_qty` INT NOT NULL DEFAULT 0 COMMENT '报废数量',
  `rework_qty` INT NOT NULL DEFAULT 0 COMMENT '重工数量',
  -- 打线工艺参数
  `wire_bond_count` INT DEFAULT 0 COMMENT '打线数量',
  `bond_force` DECIMAL(5,2) DEFAULT NULL COMMENT '键合力(gf)',
  `bond_temp` DECIMAL(5,2) DEFAULT NULL COMMENT '键合温度(℃)',
  `bond_time` INT DEFAULT NULL COMMENT '键合时间(ms)',
  `ultrasonic_power` DECIMAL(5,2) DEFAULT NULL COMMENT '超声功率(mW)',
  -- 塑封工艺参数
  `mold_pressure` DECIMAL(5,2) DEFAULT NULL COMMENT '塑封压力(MPa)',
  `mold_temp` DECIMAL(5,2) DEFAULT NULL COMMENT '塑封温度(℃)',
  `mold_time` INT DEFAULT NULL COMMENT '塑封时间(s)',
  `transfer_pressure` DECIMAL(5,2) DEFAULT NULL COMMENT '转移压力(MPa)',
  -- 固化工艺参数
  `cure_time` INT DEFAULT NULL COMMENT '固化时间(min)',
  `cure_temp` DECIMAL(5,2) DEFAULT NULL COMMENT '固化温度(℃)',
  -- 贴片工艺参数
  `die_attach_force` DECIMAL(5,2) DEFAULT NULL COMMENT '贴片压力(N)',
  `die_attach_temp` DECIMAL(5,2) DEFAULT NULL COMMENT '贴片温度(℃)',
  `die_attach_offset_x` DECIMAL(5,2) DEFAULT NULL COMMENT '贴片偏移X(μm)',
  `die_attach_offset_y` DECIMAL(5,2) DEFAULT NULL COMMENT '贴片偏移Y(μm)',
  -- 操作员信息
  `operator_id` VARCHAR(50) NOT NULL COMMENT '操作员 ID',
  `operator_name` VARCHAR(100) DEFAULT NULL COMMENT '操作员姓名',
  -- 时间信息
  `start_time` DATETIME DEFAULT NULL COMMENT '开始时间',
  `end_time` DATETIME DEFAULT NULL COMMENT '结束时间',
  `status` VARCHAR(20) NOT NULL DEFAULT 'Completed' COMMENT '状态',
  `detail` JSON DEFAULT NULL COMMENT '扩展数据 JSON',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`assemble_id`),
  INDEX `idx_lot_id` (`lot_id`),
  INDEX `idx_mfg_id` (`mfg_id`),
  INDEX `idx_step_code` (`step_code`),
  INDEX `idx_equipment_id` (`equipment_id`),
  INDEX `idx_start_time` (`start_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 二、新增 Test Data 表
-- ============================================================

CREATE TABLE `prod_test_data` (
  `test_id` VARCHAR(50) NOT NULL,
  `lot_id` VARCHAR(50) NOT NULL COMMENT '所属批次 ID',
  `mfg_id` VARCHAR(50) DEFAULT NULL COMMENT '关联的 MFG ID',
  `step_code` VARCHAR(50) NOT NULL COMMENT '工序代码',
  `step_seq` INT NOT NULL COMMENT '工序序号',
  `test_program` VARCHAR(100) NOT NULL COMMENT '测试程序',
  `test_version` VARCHAR(20) DEFAULT NULL COMMENT '测试程序版本',
  `equipment_id` VARCHAR(50) NOT NULL COMMENT '测试设备 ID',
  `handler_id` VARCHAR(50) DEFAULT NULL COMMENT '分选机 ID',
  `input_qty` INT NOT NULL DEFAULT 0 COMMENT '投入数量',
  `pass_qty` INT NOT NULL DEFAULT 0 COMMENT '通过数量',
  `fail_qty` INT NOT NULL DEFAULT 0 COMMENT '失败数量',
  `scrap_qty` INT NOT NULL DEFAULT 0 COMMENT '报废数量',
  `retest_qty` INT NOT NULL DEFAULT 0 COMMENT '重测数量',
  -- 测试条件
  `test_temp` DECIMAL(5,2) DEFAULT NULL COMMENT '测试温度(℃)',
  `test_voltage` DECIMAL(5,2) DEFAULT NULL COMMENT '测试电压(V)',
  `test_current` DECIMAL(5,2) DEFAULT NULL COMMENT '测试电流(mA)',
  `test_frequency` DECIMAL(10,2) DEFAULT NULL COMMENT '测试频率(MHz)',
  -- BIN 结果
  `bin_summary` VARCHAR(255) DEFAULT NULL COMMENT 'BIN 结果汇总',
  `bin1_qty` INT DEFAULT 0 COMMENT 'BIN1 数量(Pass)',
  `bin2_qty` INT DEFAULT 0 COMMENT 'BIN2 数量',
  `bin3_qty` INT DEFAULT 0 COMMENT 'BIN3 数量',
  `bin4_qty` INT DEFAULT 0 COMMENT 'BIN4 数量',
  `bin5_qty` INT DEFAULT 0 COMMENT 'BIN5 数量',
  `bin6_qty` INT DEFAULT 0 COMMENT 'BIN6 数量',
  `bin7_qty` INT DEFAULT 0 COMMENT 'BIN7 数量',
  `bin8_qty` INT DEFAULT 0 COMMENT 'BIN8 数量',
  -- 良率计算
  `yield_percent` DECIMAL(5,2) DEFAULT NULL COMMENT '良率(%)',
  `first_pass_yield` DECIMAL(5,2) DEFAULT NULL COMMENT '一次通过率(%)',
  `final_yield` DECIMAL(5,2) DEFAULT NULL COMMENT '最终良率(%)',
  `test_result` VARCHAR(50) DEFAULT NULL COMMENT '总体结果:Pass/Fail/Partial',
  -- 参数测试数据
  `parametric_data` JSON DEFAULT NULL COMMENT '参数测试结果 JSON',
  -- 操作员信息
  `operator_id` VARCHAR(50) NOT NULL COMMENT '操作员 ID',
  `operator_name` VARCHAR(100) DEFAULT NULL COMMENT '操作员姓名',
  -- 时间信息
  `start_time` DATETIME DEFAULT NULL COMMENT '开始时间',
  `end_time` DATETIME DEFAULT NULL COMMENT '结束时间',
  `status` VARCHAR(20) NOT NULL DEFAULT 'Completed' COMMENT '状态',
  `detail` JSON DEFAULT NULL COMMENT '扩展数据 JSON',
  `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`test_id`),
  INDEX `idx_lot_id` (`lot_id`),
  INDEX `idx_mfg_id` (`mfg_id`),
  INDEX `idx_step_code` (`step_code`),
  INDEX `idx_equipment_id` (`equipment_id`),
  INDEX `idx_test_program` (`test_program`),
  INDEX `idx_start_time` (`start_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
