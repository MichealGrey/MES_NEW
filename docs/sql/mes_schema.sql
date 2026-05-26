-- ============================================================
-- MES V3 存储架构 - 完整建表脚本
-- 数据库: mes_prod
-- 字符集: utf8mb4
-- 创建日期: 2026-05-25
-- ============================================================

CREATE DATABASE IF NOT EXISTS `mes_prod` 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE `mes_prod`;

-- ============================================================
-- 一、系统基础表 (System)
-- ============================================================

-- 1.1 部门表
CREATE TABLE `sys_department` (
    `dept_id` VARCHAR(50) NOT NULL,
    `dept_name` VARCHAR(100) NOT NULL,
    `parent_id` VARCHAR(50) DEFAULT NULL,
    `manager_id` VARCHAR(50) DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Active',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`dept_id`),
    INDEX `idx_parent_id` (`parent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 1.2 角色表
CREATE TABLE `sys_role` (
    `role_id` VARCHAR(50) NOT NULL,
    `role_name` VARCHAR(100) NOT NULL,
    `description` VARCHAR(255) DEFAULT NULL,
    `level` INT NOT NULL DEFAULT 0 COMMENT '权限级别 0-3',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`role_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 1.3 用户表
CREATE TABLE `sys_user` (
    `user_id` VARCHAR(50) NOT NULL,
    `user_name` VARCHAR(100) NOT NULL,
    `password_hash` VARCHAR(255) DEFAULT NULL,
    `role_id` VARCHAR(50) NOT NULL,
    `dept_id` VARCHAR(50) NOT NULL,
    `shift` VARCHAR(20) DEFAULT NULL COMMENT '班次 A/B/C',
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`user_id`),
    INDEX `idx_role_id` (`role_id`),
    INDEX `idx_dept_id` (`dept_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 1.4 用户权限关联表
CREATE TABLE `sys_user_permission` (
    `id` BIGINT AUTO_INCREMENT,
    `user_id` VARCHAR(50) NOT NULL,
    `permission_code` VARCHAR(100) NOT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_user_permission` (`user_id`, `permission_code`),
    INDEX `idx_user_id` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 二、主数据表 (Master Data)
-- ============================================================

-- 2.1 产品表
CREATE TABLE `master_product` (
    `product_id` VARCHAR(50) NOT NULL,
    `product_name` VARCHAR(100) NOT NULL,
    `die_name` VARCHAR(100) DEFAULT NULL,
    `package_type` VARCHAR(50) NOT NULL,
    `customer_id` VARCHAR(50) DEFAULT NULL,
    `customer_name` VARCHAR(100) DEFAULT NULL,
    `customer_pn` VARCHAR(100) DEFAULT NULL,
    `internal_pn` VARCHAR(100) DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Active',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`product_id`),
    INDEX `idx_customer_id` (`customer_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.2 工艺路线表
CREATE TABLE `master_route` (
    `route_id` VARCHAR(100) NOT NULL,
    `route_name` VARCHAR(100) NOT NULL,
    `route_version` VARCHAR(20) NOT NULL DEFAULT '1.0',
    `product_id` VARCHAR(50) NOT NULL,
    `package_type` VARCHAR(50) DEFAULT NULL,
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `is_approved` TINYINT(1) NOT NULL DEFAULT 0,
    `approved_by` VARCHAR(50) DEFAULT NULL,
    `approved_at` DATETIME DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`route_id`),
    INDEX `idx_product_id` (`product_id`),
    INDEX `idx_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.3 工艺步骤表
CREATE TABLE `master_route_step` (
    `id` BIGINT AUTO_INCREMENT,
    `route_id` VARCHAR(100) NOT NULL,
    `step_seq` INT NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `step_name` VARCHAR(100) NOT NULL,
    `equipment_group` VARCHAR(50) DEFAULT NULL,
    `is_rework` TINYINT(1) NOT NULL DEFAULT 0,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_route_step` (`route_id`, `step_seq`),
    INDEX `idx_route_id` (`route_id`),
    INDEX `idx_step_code` (`step_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.4 设备表
CREATE TABLE `master_equipment` (
    `equipment_id` VARCHAR(50) NOT NULL,
    `equipment_name` VARCHAR(100) NOT NULL,
    `equipment_group` VARCHAR(50) NOT NULL,
    `equipment_type` VARCHAR(50) NOT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Available',
    `current_lot_id` VARCHAR(50) DEFAULT NULL,
    `current_recipe` VARCHAR(100) DEFAULT NULL,
    `location` VARCHAR(100) DEFAULT NULL,
    `responsible_person` VARCHAR(50) DEFAULT NULL,
    `last_maintenance_date` DATETIME DEFAULT NULL,
    `maintenance_interval_hours` INT NOT NULL DEFAULT 500,
    `running_hours` INT NOT NULL DEFAULT 0,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`equipment_id`),
    INDEX `idx_equipment_group` (`equipment_group`),
    INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.5 设备支持路线关联表
CREATE TABLE `master_equipment_route` (
    `id` BIGINT AUTO_INCREMENT,
    `equipment_id` VARCHAR(50) NOT NULL,
    `route_id` VARCHAR(100) NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_equip_route` (`equipment_id`, `route_id`),
    INDEX `idx_equipment_id` (`equipment_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.6 载具表
CREATE TABLE `master_carrier` (
    `carrier_id` VARCHAR(50) NOT NULL,
    `carrier_type` VARCHAR(50) NOT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Available',
    `current_lot_id` VARCHAR(50) DEFAULT NULL,
    `capacity` INT NOT NULL DEFAULT 0,
    `use_count` INT NOT NULL DEFAULT 0,
    `max_use_count` INT NOT NULL DEFAULT 0,
    `last_clean_date` DATETIME DEFAULT NULL,
    `clean_interval_uses` INT NOT NULL DEFAULT 100,
    `location` VARCHAR(100) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`carrier_id`),
    INDEX `idx_carrier_type` (`carrier_type`),
    INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.7 Recipe 表
CREATE TABLE `master_recipe` (
    `recipe_id` VARCHAR(100) NOT NULL,
    `recipe_name` VARCHAR(100) NOT NULL,
    `equipment_group` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `version` VARCHAR(20) NOT NULL DEFAULT '1.0',
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `parameters` JSON DEFAULT NULL COMMENT '参数JSON',
    `approved_by` VARCHAR(50) DEFAULT NULL,
    `approved_at` DATETIME DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`recipe_id`),
    INDEX `idx_equipment_group` (`equipment_group`),
    INDEX `idx_product_id` (`product_id`),
    INDEX `idx_step_code` (`step_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2.8 良率规则表
CREATE TABLE `master_yield_rule` (
    `rule_id` VARCHAR(50) NOT NULL,
    `route_id` VARCHAR(100) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `yield_threshold` DECIMAL(5,2) NOT NULL,
    `action_type` VARCHAR(50) NOT NULL DEFAULT 'AutoHold',
    `notify_role` VARCHAR(50) NOT NULL DEFAULT 'QA',
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`rule_id`),
    INDEX `idx_route_id` (`route_id`),
    INDEX `idx_step_code` (`step_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 三、生产执行表 (Production Execution)
-- ============================================================

-- 3.1 工单表
CREATE TABLE `prod_work_order` (
    `order_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `product_name` VARCHAR(100) NOT NULL,
    `route_id` VARCHAR(100) NOT NULL,
    `route_name` VARCHAR(100) NOT NULL,
    `die_name` VARCHAR(100) DEFAULT NULL,
    `package_type` VARCHAR(50) NOT NULL,
    `planned_qty` INT NOT NULL,
    `completed_qty` INT NOT NULL DEFAULT 0,
    `wafer_qty` INT NOT NULL DEFAULT 0,
    `unit_qty` INT NOT NULL DEFAULT 0,
    `customer_id` VARCHAR(50) DEFAULT NULL,
    `customer_name` VARCHAR(100) DEFAULT NULL,
    `customer_pn` VARCHAR(100) DEFAULT NULL,
    `internal_pn` VARCHAR(100) DEFAULT NULL,
    `priority` VARCHAR(20) NOT NULL DEFAULT 'Normal',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Created',
    `creator` VARCHAR(50) NOT NULL,
    `planned_start_date` DATETIME DEFAULT NULL,
    `planned_end_date` DATETIME DEFAULT NULL,
    `actual_start_date` DATETIME DEFAULT NULL,
    `actual_end_date` DATETIME DEFAULT NULL,
    `target_cp_yield` DECIMAL(5,2) DEFAULT 99.00,
    `target_ft_yield` DECIMAL(5,2) DEFAULT 98.00,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`order_id`),
    INDEX `idx_product_id` (`product_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_priority` (`priority`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3.2 批次表
CREATE TABLE `prod_lot` (
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
    `original_qty` INT NOT NULL DEFAULT 0,
    `total_pass_qty` INT NOT NULL DEFAULT 0,
    `total_scrap_qty` INT NOT NULL DEFAULT 0,
    `total_rework_qty` INT NOT NULL DEFAULT 0,
    `total_hold_qty` INT NOT NULL DEFAULT 0,
    `is_partial_lot` TINYINT(1) NOT NULL DEFAULT 0,
    `mother_lot_id` VARCHAR(50) DEFAULT NULL,
    `split_reason` VARCHAR(255) DEFAULT NULL,
    `split_time` DATETIME DEFAULT NULL,
    `split_qty` INT DEFAULT NULL,
    `is_rework_lot` TINYINT(1) NOT NULL DEFAULT 0,
    `original_route_id` VARCHAR(100) DEFAULT NULL,
    `rework_route_id` VARCHAR(100) DEFAULT NULL,
    `rework_count` INT DEFAULT NULL,
    `rework_reason` VARCHAR(255) DEFAULT NULL,
    `is_under_mrb` TINYINT(1) NOT NULL DEFAULT 0,
    `mrb_reference` VARCHAR(50) DEFAULT NULL,
    `mrb_disposition` VARCHAR(50) DEFAULT NULL,
    `grade` VARCHAR(20) DEFAULT NULL,
    `original_lot_id` VARCHAR(50) DEFAULT NULL,
    `bin_result` VARCHAR(50) DEFAULT NULL,
    `test_result` VARCHAR(50) DEFAULT NULL,
    `qty_pass` INT NOT NULL DEFAULT 0,
    `qty_fail` INT NOT NULL DEFAULT 0,
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
    INDEX `idx_priority` (`priority`),
    INDEX `idx_mother_lot` (`mother_lot_id`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3.3 批次步骤记录表
CREATE TABLE `prod_lot_step` (
    `record_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `route_id` VARCHAR(100) NOT NULL,
    `route_version` VARCHAR(20) NOT NULL DEFAULT '1.0',
    `step_seq` INT NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `step_name` VARCHAR(100) NOT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Waiting',
    `track_in_equipment` VARCHAR(50) DEFAULT NULL,
    `track_in_carrier` VARCHAR(50) DEFAULT NULL,
    `track_in_recipe` VARCHAR(100) DEFAULT NULL,
    `track_in_time` DATETIME DEFAULT NULL,
    `track_in_operator` VARCHAR(50) DEFAULT NULL,
    `track_out_time` DATETIME DEFAULT NULL,
    `track_out_operator` VARCHAR(50) DEFAULT NULL,
    `input_qty` INT NOT NULL DEFAULT 0,
    `pass_qty` INT NOT NULL DEFAULT 0,
    `fail_qty` INT NOT NULL DEFAULT 0,
    `scrap_qty` INT NOT NULL DEFAULT 0,
    `rework_qty` INT NOT NULL DEFAULT 0,
    `hold_qty` INT NOT NULL DEFAULT 0,
    `pending_qty` INT NOT NULL DEFAULT 0,
    `recipe_id` VARCHAR(100) DEFAULT NULL,
    `test_program` VARCHAR(100) DEFAULT NULL,
    `bin_summary` VARCHAR(255) DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`record_id`),
    UNIQUE KEY `uk_lot_step` (`lot_id`, `route_id`, `step_seq`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_step_code` (`step_code`),
    INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3.4 操作历史表
CREATE TABLE `prod_operation_history` (
    `id` BIGINT AUTO_INCREMENT,
    `operation_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `order_id` VARCHAR(50) DEFAULT NULL,
    `operation_type` VARCHAR(50) NOT NULL COMMENT 'TrackIn/TrackOut/Hold/Release/Split/Merge/etc',
    `step_code` VARCHAR(50) DEFAULT NULL,
    `step_seq` INT DEFAULT NULL,
    `equipment_id` VARCHAR(50) DEFAULT NULL,
    `carrier_id` VARCHAR(50) DEFAULT NULL,
    `recipe_id` VARCHAR(100) DEFAULT NULL,
    `operator_id` VARCHAR(50) NOT NULL,
    `operator_name` VARCHAR(100) DEFAULT NULL,
    `input_qty` INT DEFAULT NULL,
    `output_qty` INT DEFAULT NULL,
    `scrap_qty` INT DEFAULT NULL,
    `detail` JSON DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_operation_id` (`operation_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_order_id` (`order_id`),
    INDEX `idx_operation_type` (`operation_type`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3.5 审计追踪表
CREATE TABLE `prod_audit_trail` (
    `audit_id` VARCHAR(50) NOT NULL,
    `entity_type` VARCHAR(50) NOT NULL COMMENT 'Lot/WorkOrder/Route/etc',
    `entity_id` VARCHAR(50) NOT NULL,
    `action` VARCHAR(50) NOT NULL COMMENT 'TrackIn/TrackOut/Hold/Release/etc',
    `operator_id` VARCHAR(50) NOT NULL,
    `operator_name` VARCHAR(100) DEFAULT NULL,
    `timestamp` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `before_state` JSON DEFAULT NULL,
    `after_state` JSON DEFAULT NULL,
    `reason` VARCHAR(255) DEFAULT NULL,
    `detail` TEXT DEFAULT NULL,
    `signature_level` INT DEFAULT 0 COMMENT '0-3 签核级别',
    `approved_by` VARCHAR(50) DEFAULT NULL,
    PRIMARY KEY (`audit_id`),
    INDEX `idx_entity` (`entity_type`, `entity_id`),
    INDEX `idx_action` (`action`),
    INDEX `idx_timestamp` (`timestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 四、异常管理表 (Exception Management)
-- ============================================================

-- 4.1 Hold 记录表
CREATE TABLE `prod_hold_record` (
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
    PRIMARY KEY (`hold_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_hold_type` (`hold_type`),
    INDEX `idx_hold_time` (`hold_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 4.2 报废记录表
CREATE TABLE `prod_scrap_record` (
    `scrap_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `step_seq` INT NOT NULL,
    `scrap_qty` INT NOT NULL,
    `scrap_reason` VARCHAR(255) NOT NULL,
    `scrap_reason_code` VARCHAR(50) NOT NULL,
    `operator_id` VARCHAR(50) NOT NULL,
    `scrap_time` DATETIME NOT NULL,
    `approved_by` VARCHAR(50) DEFAULT NULL,
    `signature_id` VARCHAR(50) DEFAULT NULL,
    `requires_approval` TINYINT(1) NOT NULL DEFAULT 0,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`scrap_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_step_code` (`step_code`),
    INDEX `idx_scrap_time` (`scrap_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 4.3 重工记录表
CREATE TABLE `prod_rework_record` (
    `rework_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `original_route_id` VARCHAR(100) NOT NULL,
    `rework_route_id` VARCHAR(100) NOT NULL,
    `from_step_code` VARCHAR(50) NOT NULL,
    `target_step_code` VARCHAR(50) NOT NULL,
    `rework_qty` INT NOT NULL,
    `rework_reason` VARCHAR(255) NOT NULL,
    `operator_id` VARCHAR(50) NOT NULL,
    `rework_count` INT NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `completed_at` DATETIME DEFAULT NULL,
    `approved_by` VARCHAR(50) DEFAULT NULL,
    `signature_id` VARCHAR(50) DEFAULT NULL,
    PRIMARY KEY (`rework_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_rework_route` (`rework_route_id`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 五、追溯表 (Traceability)
-- ============================================================

-- 5.1 批次拆分记录表
CREATE TABLE `prod_lot_split` (
    `split_id` VARCHAR(50) NOT NULL,
    `mother_lot_id` VARCHAR(50) NOT NULL,
    `child_lot_id` VARCHAR(50) NOT NULL,
    `split_qty` INT NOT NULL,
    `split_reason` VARCHAR(255) NOT NULL,
    `split_type` VARCHAR(50) NOT NULL COMMENT 'Normal/Grade',
    `step_code` VARCHAR(50) NOT NULL,
    `step_seq` INT NOT NULL,
    `operator_id` VARCHAR(50) NOT NULL,
    `split_time` DATETIME NOT NULL,
    `approved_by` VARCHAR(50) DEFAULT NULL,
    `signature_id` VARCHAR(50) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`split_id`),
    INDEX `idx_mother_lot` (`mother_lot_id`),
    INDEX `idx_child_lot` (`child_lot_id`),
    INDEX `idx_split_time` (`split_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 5.2 批次合并记录表
CREATE TABLE `prod_lot_merge` (
    `merge_id` VARCHAR(50) NOT NULL,
    `target_lot_id` VARCHAR(50) NOT NULL,
    `source_lot_id` VARCHAR(50) NOT NULL,
    `merge_qty` INT NOT NULL,
    `merge_reason` VARCHAR(255) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `step_seq` INT NOT NULL,
    `operator_id` VARCHAR(50) NOT NULL,
    `merge_time` DATETIME NOT NULL,
    `approved_by` VARCHAR(50) DEFAULT NULL,
    `signature_id` VARCHAR(50) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`merge_id`),
    INDEX `idx_target_lot` (`target_lot_id`),
    INDEX `idx_source_lot` (`source_lot_id`),
    INDEX `idx_merge_time` (`merge_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 5.3 谱系关系表
CREATE TABLE `prod_genealogy` (
    `genealogy_id` VARCHAR(50) NOT NULL,
    `parent_lot_id` VARCHAR(50) NOT NULL,
    `child_lot_id` VARCHAR(50) NOT NULL,
    `relation_type` VARCHAR(50) NOT NULL COMMENT 'Create/Split/Merge/Rework/GradeSplit',
    `step_code` VARCHAR(50) NOT NULL,
    `step_seq` INT NOT NULL,
    `qty` INT NOT NULL,
    `grade` VARCHAR(20) DEFAULT NULL,
    `wafer_lot_id` VARCHAR(50) DEFAULT NULL,
    `operator_id` VARCHAR(50) NOT NULL,
    `reason_code` VARCHAR(50) DEFAULT NULL,
    `remark` VARCHAR(255) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`genealogy_id`),
    INDEX `idx_parent_lot` (`parent_lot_id`),
    INDEX `idx_child_lot` (`child_lot_id`),
    INDEX `idx_relation_type` (`relation_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 5.4 载具绑定记录表
CREATE TABLE `prod_carrier_binding` (
    `binding_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `step_seq` INT NOT NULL,
    `carrier_id` VARCHAR(50) NOT NULL,
    `carrier_type` VARCHAR(50) NOT NULL,
    `from_carrier_id` VARCHAR(50) DEFAULT NULL,
    `bind_time` DATETIME NOT NULL,
    `unbind_time` DATETIME DEFAULT NULL,
    `operator_id` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`binding_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_carrier_id` (`carrier_id`),
    INDEX `idx_bind_time` (`bind_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 六、派工与调度表 (Dispatch & Schedule)
-- ============================================================

-- 6.1 派工任务表
CREATE TABLE `prod_dispatch_task` (
    `task_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `order_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `step_name` VARCHAR(100) NOT NULL,
    `step_seq` INT NOT NULL,
    `equipment_id` VARCHAR(50) DEFAULT NULL,
    `recipe_id` VARCHAR(100) DEFAULT NULL,
    `qty` INT NOT NULL,
    `priority` VARCHAR(20) NOT NULL DEFAULT 'Normal',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending',
    `assigned_operator` VARCHAR(50) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `assigned_at` DATETIME DEFAULT NULL,
    `started_at` DATETIME DEFAULT NULL,
    `completed_at` DATETIME DEFAULT NULL,
    `due_hours` DECIMAL(6,2) DEFAULT NULL,
    `remaining_hours` DECIMAL(6,2) DEFAULT NULL,
    `is_overdue` TINYINT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (`task_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_order_id` (`order_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_priority` (`priority`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 七、质量管理表 (Quality)
-- ============================================================

-- 7.1 质量 Gate 表
CREATE TABLE `quality_gate` (
    `gate_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `gate_type` VARCHAR(50) NOT NULL COMMENT 'QA/Customer/MRB',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending',
    `checker_id` VARCHAR(50) DEFAULT NULL,
    `check_result` VARCHAR(50) DEFAULT NULL,
    `check_comment` TEXT DEFAULT NULL,
    `checked_at` DATETIME DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`gate_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_gate_type` (`gate_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 7.2 检验记录表
CREATE TABLE `quality_inspection` (
    `inspection_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `inspection_type` VARCHAR(50) NOT NULL,
    `result` VARCHAR(50) NOT NULL,
    `inspector_id` VARCHAR(50) NOT NULL,
    `inspection_time` DATETIME NOT NULL,
    `detail` JSON DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`inspection_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_inspection_time` (`inspection_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 八、报警表 (Alarm)
-- ============================================================

-- 8.1 报警规则表
CREATE TABLE `alarm_rule` (
    `rule_id` VARCHAR(50) NOT NULL,
    `rule_name` VARCHAR(100) NOT NULL,
    `rule_type` VARCHAR(50) NOT NULL COMMENT 'Yield/HoldTimeout/QueueTimeout/Equipment',
    `condition_expr` VARCHAR(255) NOT NULL,
    `severity` VARCHAR(20) NOT NULL DEFAULT 'Warning',
    `notify_roles` VARCHAR(255) DEFAULT NULL,
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`rule_id`),
    INDEX `idx_rule_type` (`rule_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 8.2 报警记录表
CREATE TABLE `alarm_record` (
    `alarm_id` VARCHAR(50) NOT NULL,
    `rule_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) DEFAULT NULL,
    `equipment_id` VARCHAR(50) DEFAULT NULL,
    `alarm_type` VARCHAR(50) NOT NULL,
    `severity` VARCHAR(20) NOT NULL,
    `message` TEXT NOT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Active',
    `acknowledged_by` VARCHAR(50) DEFAULT NULL,
    `acknowledged_at` DATETIME DEFAULT NULL,
    `resolved_by` VARCHAR(50) DEFAULT NULL,
    `resolved_at` DATETIME DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`alarm_id`),
    INDEX `idx_rule_id` (`rule_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 九、客户交付表 (Customer)
-- ============================================================

-- 9.1 客户要求表
CREATE TABLE `customer_requirement` (
    `requirement_id` VARCHAR(50) NOT NULL,
    `customer_id` VARCHAR(50) NOT NULL,
    `customer_name` VARCHAR(100) NOT NULL,
    `order_id` VARCHAR(50) DEFAULT NULL,
    `product_id` VARCHAR(50) DEFAULT NULL,
    `requirement_type` VARCHAR(50) NOT NULL COMMENT 'Traceability/Yield/LeadTime/Packaging/Testing/Documentation',
    `description` TEXT NOT NULL,
    `priority` VARCHAR(20) NOT NULL DEFAULT 'Normal' COMMENT 'High/Normal/Low',
    `is_mandatory` TINYINT(1) NOT NULL DEFAULT 0,
    `verification_method` VARCHAR(100) DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Active' COMMENT 'Active/Completed/Cancelled',
    `created_by` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `approved_by` VARCHAR(50) DEFAULT NULL,
    `approved_at` DATETIME DEFAULT NULL,
    PRIMARY KEY (`requirement_id`),
    INDEX `idx_customer_id` (`customer_id`),
    INDEX `idx_order_id` (`order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 十、报表与归档表 (Report & Archive)
-- ============================================================

-- 10.1 生产报表快照表
CREATE TABLE `report_production_daily` (
    `report_id` VARCHAR(50) NOT NULL,
    `report_date` DATE NOT NULL,
    `total_lots` INT NOT NULL DEFAULT 0,
    `completed_lots` INT NOT NULL DEFAULT 0,
    `wip_lots` INT NOT NULL DEFAULT 0,
    `hold_lots` INT NOT NULL DEFAULT 0,
    `total_input_qty` INT NOT NULL DEFAULT 0,
    `total_output_qty` INT NOT NULL DEFAULT 0,
    `total_scrap_qty` INT NOT NULL DEFAULT 0,
    `overall_yield` DECIMAL(5,2) DEFAULT 0,
    `ft_yield` DECIMAL(5,2) DEFAULT 0,
    `generated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`report_id`),
    UNIQUE KEY `uk_report_date` (`report_date`),
    INDEX `idx_report_date` (`report_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 10.2 批次归档表（已完成批次迁移）
CREATE TABLE `prod_lot_archive` (
    `lot_id` VARCHAR(50) NOT NULL,
    `order_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `status` VARCHAR(20) NOT NULL,
    `original_qty` INT NOT NULL,
    `total_pass_qty` INT NOT NULL,
    `total_scrap_qty` INT NOT NULL,
    `final_yield` DECIMAL(5,2) DEFAULT 0,
    `completed_at` DATETIME NOT NULL,
    `archived_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`lot_id`),
    INDEX `idx_order_id` (`order_id`),
    INDEX `idx_completed_at` (`completed_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 十一、外部系统集成表 (External System Integration)
-- ============================================================

-- 11.1 外部系统事件表
CREATE TABLE `ext_system_event` (
    `event_id` VARCHAR(50) NOT NULL,
    `event_type` VARCHAR(50) NOT NULL COMMENT 'LotTrackIn/LotTrackOut/AlarmRaised/OrderCompleted',
    `source_system` VARCHAR(50) NOT NULL DEFAULT 'MES',
    `target_system` VARCHAR(50) NOT NULL,
    `payload` JSON DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT 'Pending/Sent/Failed/Acknowledged',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `sent_at` DATETIME DEFAULT NULL,
    `error_message` TEXT DEFAULT NULL,
    `retry_count` INT NOT NULL DEFAULT 0,
    PRIMARY KEY (`event_id`),
    INDEX `idx_target_system` (`target_system`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 11.2 外部系统配置表
CREATE TABLE `ext_system_config` (
    `system_id` VARCHAR(50) NOT NULL,
    `system_name` VARCHAR(100) NOT NULL,
    `system_type` VARCHAR(50) NOT NULL COMMENT 'ERP/EAP/QMS/WMS',
    `endpoint` VARCHAR(255) NOT NULL,
    `auth_type` VARCHAR(20) NOT NULL DEFAULT 'None' COMMENT 'None/Basic/ApiKey/OAuth',
    `auth_credential` TEXT DEFAULT NULL,
    `is_enabled` TINYINT(1) NOT NULL DEFAULT 1,
    `timeout_seconds` INT NOT NULL DEFAULT 30,
    `max_retries` INT NOT NULL DEFAULT 3,
    `subscribed_events` JSON DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`system_id`),
    INDEX `idx_system_type` (`system_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 十二、物料管理表 (Material Management)
-- ============================================================

-- 12.1 物料主数据表
CREATE TABLE `master_material` (
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) NOT NULL,
    `material_type` VARCHAR(50) NOT NULL COMMENT 'RawMaterial/Consumable/Packaging',
    `specification` VARCHAR(255) DEFAULT NULL,
    `unit` VARCHAR(20) NOT NULL DEFAULT 'pcs',
    `supplier` VARCHAR(100) DEFAULT NULL,
    `min_stock` INT NOT NULL DEFAULT 0,
    `current_stock` INT NOT NULL DEFAULT 0,
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`material_id`),
    INDEX `idx_material_type` (`material_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 12.2 工序物料需求表
CREATE TABLE `material_requirement` (
    `requirement_id` VARCHAR(50) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `required_qty` DECIMAL(10,2) NOT NULL DEFAULT 0,
    `unit` VARCHAR(20) NOT NULL DEFAULT 'pcs',
    `is_mandatory` TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (`requirement_id`),
    INDEX `idx_step_code` (`step_code`),
    INDEX `idx_material_id` (`material_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 12.3 物料消耗记录表
CREATE TABLE `material_consume` (
    `consume_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) NOT NULL,
    `consumed_qty` DECIMAL(10,2) NOT NULL,
    `unit` VARCHAR(20) NOT NULL DEFAULT 'pcs',
    `batch_no` VARCHAR(50) DEFAULT NULL,
    `operator_id` VARCHAR(50) NOT NULL,
    `consumed_at` DATETIME NOT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`consume_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_step_code` (`step_code`),
    INDEX `idx_consumed_at` (`consumed_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 十三、质量 Gate 表 (Quality Gate)
-- ============================================================

-- 13.1 质量 Gate 实例表
CREATE TABLE `quality_gate_instance` (
    `gate_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `step_seq` INT NOT NULL,
    `gate_type` VARCHAR(50) NOT NULL DEFAULT 'QACheck',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT 'Pending/Passed/Failed',
    `checked_by` VARCHAR(50) DEFAULT NULL,
    `checked_by_name` VARCHAR(100) DEFAULT NULL,
    `checked_at` DATETIME DEFAULT NULL,
    `comment` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `expire_at` DATETIME DEFAULT NULL,
    PRIMARY KEY (`gate_id`),
    INDEX `idx_lot_step` (`lot_id`, `step_code`),
    INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 十四、签核与数量事务表 (Signature & Quantity Transaction)
-- ============================================================

-- 14.1 签核记录表
CREATE TABLE `prod_signature` (
    `signature_id` VARCHAR(50) NOT NULL,
    `entity_type` VARCHAR(50) NOT NULL,
    `entity_id` VARCHAR(50) NOT NULL,
    `level` VARCHAR(20) NOT NULL,
    `signer_id` VARCHAR(50) NOT NULL,
    `signer_name` VARCHAR(100) NOT NULL,
    `signer_role` VARCHAR(50) NOT NULL,
    `reason` VARCHAR(255) NOT NULL,
    `comment` VARCHAR(255) DEFAULT NULL,
    `sign_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`signature_id`),
    INDEX `idx_entity` (`entity_type`, `entity_id`),
    INDEX `idx_signer_id` (`signer_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 14.2 数量事务记录表
CREATE TABLE `quantity_transaction` (
    `id` BIGINT AUTO_INCREMENT,
    `lot_id` VARCHAR(50) NOT NULL,
    `route_id` VARCHAR(100) NOT NULL,
    `step_seq` INT NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `step_name` VARCHAR(100) NOT NULL,
    `equipment_id` VARCHAR(50) DEFAULT NULL,
    `input_qty` INT NOT NULL DEFAULT 0,
    `pass_qty` INT NOT NULL DEFAULT 0,
    `fail_qty` INT NOT NULL DEFAULT 0,
    `scrap_qty` INT NOT NULL DEFAULT 0,
    `rework_qty` INT NOT NULL DEFAULT 0,
    `hold_qty` INT NOT NULL DEFAULT 0,
    `pending_qty` INT NOT NULL DEFAULT 0,
    `operator_id` VARCHAR(50) NOT NULL,
    `operator_name` VARCHAR(100) NOT NULL,
    `timestamp` DATETIME NOT NULL,
    PRIMARY KEY (`id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_step_code` (`step_code`),
    INDEX `idx_timestamp` (`timestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 14.3 签核级别定义表
CREATE TABLE `sys_signature_level` (
    `level_code` VARCHAR(20) NOT NULL,
    `level_name` VARCHAR(50) NOT NULL,
    `level_order` INT NOT NULL,
    `description` VARCHAR(255) DEFAULT NULL,
    PRIMARY KEY (`level_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
