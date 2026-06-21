-- ============================================================
-- V4.0.1: FQC/OQC 终检管理
-- 描述: 创建 FQC 终检记录、OQC 出货检验记录及出货 MSL 检查表
-- 依赖: V4.0.0 (iqc_inspection_standard)
-- 作者: MES Team
-- 日期: 2026-06-07
-- ============================================================

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
