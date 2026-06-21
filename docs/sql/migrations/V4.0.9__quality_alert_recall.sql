-- ============================================================
-- V4.0.9: 紧急质量通知与召回
-- 描述: 创建质量警报表、影响批次关联表、召回通知及召回明细表
-- 依赖: 无
-- 作者: MES Team
-- 日期: 2026-06-07
-- ============================================================

-- 质量警报表
CREATE TABLE IF NOT EXISTS `quality_alert` (
    `alert_id` VARCHAR(50) NOT NULL COMMENT '格式: QA-YYYYMMDD-NNN',
    `alert_type` VARCHAR(50) NOT NULL
        COMMENT 'MaterialDefect/ProcessDefect/EquipmentIssue/CustomerComplaint',
    `severity` VARCHAR(20) NOT NULL COMMENT 'Critical/High/Medium',
    `title` VARCHAR(200) NOT NULL,
    `description` TEXT NOT NULL,
    `root_cause` TEXT DEFAULT NULL,
    `source_material_id` VARCHAR(50) DEFAULT NULL,
    `source_batch_id` VARCHAR(50) DEFAULT NULL,
    `source_work_order_id` VARCHAR(50) DEFAULT NULL,
    `source_lot_id` VARCHAR(50) DEFAULT NULL,
    `occurrence_date` DATETIME DEFAULT NULL,
    `issued_by` VARCHAR(50) NOT NULL,
    `issued_by_name` VARCHAR(100) DEFAULT NULL,
    `issued_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Active'
        COMMENT 'Active/Investigating/Resolved/Closed',
    `investigation_result` TEXT DEFAULT NULL,
    `resolved_by` VARCHAR(50) DEFAULT NULL,
    `resolved_at` DATETIME DEFAULT NULL,
    `closed_by` VARCHAR(50) DEFAULT NULL,
    `closed_at` DATETIME DEFAULT NULL,
    `frozen_lots_count` INT DEFAULT 0,
    `affected_lots_count` INT DEFAULT 0,
    `notify_departments` JSON DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`alert_id`),
    INDEX `idx_alert_type` (`alert_type`),
    INDEX `idx_severity` (`severity`),
    INDEX `idx_status` (`status`),
    INDEX `idx_issued_at` (`issued_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 质量警报影响批次关联表
CREATE TABLE IF NOT EXISTS `quality_alert_affected_lot` (
    `id` BIGINT AUTO_INCREMENT,
    `alert_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `work_order_id` VARCHAR(50) DEFAULT NULL,
    `product_id` VARCHAR(50) DEFAULT NULL,
    `affected_qty` INT DEFAULT 0,
    `trace_relation` VARCHAR(100) DEFAULT NULL
        COMMENT 'SameBatch/SameMaterial/SameEquipment/SameProcess',
    `is_frozen` TINYINT(1) NOT NULL DEFAULT 0,
    `frozen_at` DATETIME DEFAULT NULL,
    `unfrozen_at` DATETIME DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_alert_lot` (`alert_id`, `lot_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_is_frozen` (`is_frozen`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 召回通知表
CREATE TABLE IF NOT EXISTS `recall_notice` (
    `recall_id` VARCHAR(50) NOT NULL COMMENT '格式: RC-YYYYMMDD-NNN',
    `alert_id` VARCHAR(50) NOT NULL,
    `recall_scope` VARCHAR(50) NOT NULL
        COMMENT 'FullLot/Partial/SpecificCustomer',
    `total_affected_qty` INT NOT NULL DEFAULT 0,
    `total_recalled_qty` INT NOT NULL DEFAULT 0,
    `recall_reason` TEXT NOT NULL,
    `generated_by` VARCHAR(50) NOT NULL,
    `generated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Generated'
        COMMENT 'Generated/Notified/InProgress/Completed',
    `completed_at` DATETIME DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    PRIMARY KEY (`recall_id`),
    INDEX `idx_alert_id` (`alert_id`),
    INDEX `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 召回明细表
CREATE TABLE IF NOT EXISTS `recall_notice_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `recall_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `customer_id` VARCHAR(50) DEFAULT NULL,
    `customer_name` VARCHAR(100) DEFAULT NULL,
    `affected_qty` INT NOT NULL DEFAULT 0,
    `recalled_qty` INT NOT NULL DEFAULT 0,
    `current_location` VARCHAR(100) DEFAULT NULL
        COMMENT 'WIP/Warehouse/InTransit/CustomerSite',
    `recall_status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/Notified/InTransit/Returned/Disposed',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`item_id`),
    INDEX `idx_recall_id` (`recall_id`),
    INDEX `idx_lot_id` (`lot_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
