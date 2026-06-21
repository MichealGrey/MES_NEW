-- ============================================================
-- V4.0.7: 设备故障管理与保养
-- 描述: 创建设备故障记录、维修备件使用、PM 保养计划及 PM 执行记录表
-- 依赖: 无
-- 作者: MES Team
-- 日期: 2026-06-07
-- ============================================================

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
