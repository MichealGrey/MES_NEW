-- ============================================================
-- V4.0.6: 生产异常上报与停线机制
-- 描述: 创建异常记录表及停线记录表
-- 依赖: 无
-- 作者: MES Team
-- 日期: 2026-06-07
-- ============================================================

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
