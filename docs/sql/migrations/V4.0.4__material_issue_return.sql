-- ============================================================
-- V4.0.4: 产线发料/退料管理
-- 描述: 创建发料单、发料单明细、退料单及退料单明细表
-- 依赖: V4.0.3 (warehouse_inventory)
-- 作者: MES Team
-- 日期: 2026-06-07
-- ============================================================

-- 发料单表
CREATE TABLE IF NOT EXISTS `warehouse_issue_order` (
    `issue_order_id` VARCHAR(50) NOT NULL COMMENT '格式: WI-YYYYMMDD-NNN',
    `work_order_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) DEFAULT NULL,
    `step_code` VARCHAR(50) DEFAULT NULL,
    `issue_type` VARCHAR(20) NOT NULL DEFAULT 'Normal'
        COMMENT 'Normal/Urgent/Rework',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/Approved/Issued/Completed/Cancelled',
    `issued_by` VARCHAR(50) NOT NULL,
    `receiver_id` VARCHAR(50) DEFAULT NULL,
    `received_at` DATETIME DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    `remark` TEXT DEFAULT NULL,
    PRIMARY KEY (`issue_order_id`),
    INDEX `idx_work_order_id` (`work_order_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 发料单明细表
CREATE TABLE IF NOT EXISTS `warehouse_issue_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `issue_order_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) DEFAULT NULL,
    `requested_qty` INT NOT NULL DEFAULT 0,
    `issued_qty` INT NOT NULL DEFAULT 0,
    `unit` VARCHAR(20) DEFAULT 'PCS',
    `batch_id` VARCHAR(50) NOT NULL COMMENT '实际发料批次',
    `location_id` VARCHAR(50) DEFAULT NULL COMMENT '出库库位',
    `fifo_skipped` TINYINT(1) DEFAULT 0,
    `fifo_skip_reason` VARCHAR(255) DEFAULT NULL,
    `fifo_skip_approved_by` VARCHAR(50) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`item_id`),
    INDEX `idx_issue_order_id` (`issue_order_id`),
    INDEX `idx_batch_id` (`batch_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 退料单表
CREATE TABLE IF NOT EXISTS `warehouse_return_order` (
    `return_order_id` VARCHAR(50) NOT NULL COMMENT '格式: WRet-YYYYMMDD-NNN',
    `work_order_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) DEFAULT NULL,
    `return_type` VARCHAR(20) NOT NULL DEFAULT 'Normal'
        COMMENT 'Normal/Excess/Defective',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/Received/Completed/Rejected',
    `returned_by` VARCHAR(50) NOT NULL,
    `received_by` VARCHAR(50) DEFAULT NULL,
    `received_at` DATETIME DEFAULT NULL,
    `reason` VARCHAR(255) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`return_order_id`),
    INDEX `idx_work_order_id` (`work_order_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 退料单明细表
CREATE TABLE IF NOT EXISTS `warehouse_return_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `return_order_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) DEFAULT NULL,
    `return_qty` INT NOT NULL DEFAULT 0,
    `original_batch_id` VARCHAR(50) NOT NULL,
    `target_location_id` VARCHAR(50) DEFAULT NULL,
    `is_expired` TINYINT(1) DEFAULT 0,
    `expiry_check_result` VARCHAR(20) DEFAULT NULL,
    `unit` VARCHAR(20) DEFAULT 'PCS',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`item_id`),
    INDEX `idx_return_order_id` (`return_order_id`),
    INDEX `idx_original_batch_id` (`original_batch_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
