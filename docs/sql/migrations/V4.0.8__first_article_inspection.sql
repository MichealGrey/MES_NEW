-- ============================================================
-- V4.0.8: 首件检验流程
-- 描述: 创建首件检验表、检验项目明细表、签名记录表及焊线拉力测试记录表
-- 依赖: 无
-- 作者: MES Team
-- 日期: 2026-06-07
-- ============================================================

-- 首件检验表
CREATE TABLE IF NOT EXISTS `first_article_inspection` (
    `fa_id` VARCHAR(50) NOT NULL COMMENT '格式: FA-YYYYMMDD-NNN',
    `work_order_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `product_name` VARCHAR(100) DEFAULT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `step_name` VARCHAR(100) DEFAULT NULL,
    `equipment_id` VARCHAR(50) DEFAULT NULL,
    `trigger_reason` VARCHAR(50) NOT NULL
        COMMENT 'LineChange/ShiftStart/ProcessChange/Maintenance/RecipeChange',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/Inspecting/TechConfirmed/IPQCConfirmed/Approved/Rejected',
    `technician_id` VARCHAR(50) DEFAULT NULL,
    `technician_name` VARCHAR(100) DEFAULT NULL,
    `technician_confirmed_at` DATETIME DEFAULT NULL,
    `ipqc_id` VARCHAR(50) DEFAULT NULL,
    `ipqc_name` VARCHAR(100) DEFAULT NULL,
    `ipqc_confirmed_at` DATETIME DEFAULT NULL,
    `result` VARCHAR(20) DEFAULT NULL COMMENT 'Pass/Fail',
    `rejection_reason` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`fa_id`),
    INDEX `idx_work_order_id` (`work_order_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 首件检验项目明细表
CREATE TABLE IF NOT EXISTS `first_article_inspection_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `fa_id` VARCHAR(50) NOT NULL,
    `item_code` VARCHAR(50) NOT NULL,
    `item_name` VARCHAR(100) NOT NULL,
    `item_type` VARCHAR(50) DEFAULT NULL,
    `standard_value` VARCHAR(50) DEFAULT NULL,
    `lower_limit` VARCHAR(50) DEFAULT NULL,
    `upper_limit` VARCHAR(50) DEFAULT NULL,
    `actual_value` VARCHAR(50) DEFAULT NULL,
    `unit` VARCHAR(20) DEFAULT NULL,
    `result` VARCHAR(20) NOT NULL COMMENT 'Pass/Fail/N/A',
    `measured_by` VARCHAR(50) DEFAULT NULL,
    `measured_at` DATETIME DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    PRIMARY KEY (`item_id`),
    INDEX `idx_fa_id` (`fa_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 首件签名记录表
CREATE TABLE IF NOT EXISTS `first_article_signature` (
    `signature_id` VARCHAR(50) NOT NULL,
    `fa_id` VARCHAR(50) NOT NULL,
    `signer_role` VARCHAR(50) NOT NULL COMMENT 'Technician/IPQC',
    `signer_id` VARCHAR(50) NOT NULL,
    `signer_name` VARCHAR(100) NOT NULL,
    `confirmation` VARCHAR(20) NOT NULL COMMENT 'Approve/Reject',
    `comment` TEXT DEFAULT NULL,
    `signed_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`signature_id`),
    UNIQUE KEY `uk_fa_role` (`fa_id`, `signer_role`),
    INDEX `idx_fa_id` (`fa_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 焊线拉力测试记录表
CREATE TABLE IF NOT EXISTS `bond_pull_test_record` (
    `test_id` VARCHAR(50) NOT NULL,
    `fa_id` VARCHAR(50) DEFAULT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `work_order_id` VARCHAR(50) DEFAULT NULL,
    `sample_no` INT NOT NULL,
    `pull_force_grams` DECIMAL(8,2) NOT NULL,
    `lower_limit_grams` DECIMAL(8,2) DEFAULT NULL,
    `upper_limit_grams` DECIMAL(8,2) DEFAULT NULL,
    `result` VARCHAR(20) NOT NULL COMMENT 'Pass/Fail',
    `failure_mode` VARCHAR(50) DEFAULT NULL,
    `tested_by` VARCHAR(50) NOT NULL,
    `tested_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `remark` TEXT DEFAULT NULL,
    PRIMARY KEY (`test_id`),
    INDEX `idx_fa_id` (`fa_id`),
    INDEX `idx_lot_id` (`lot_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
