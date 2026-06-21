-- ============================================================
-- V4.0.5: 成品入库/出库管理
-- 描述: 创建成品入库单、成品库存、出货单及出货单明细表
-- 依赖: V4.0.1 (fqc_inspection_record, oqc_inspection_record)
-- 作者: MES Team
-- 日期: 2026-06-07
-- ============================================================

-- 成品入库单表
CREATE TABLE IF NOT EXISTS `finished_goods_receipt` (
    `receipt_id` VARCHAR(50) NOT NULL COMMENT '格式: FGR-YYYYMMDD-NNN',
    `work_order_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `product_name` VARCHAR(100) DEFAULT NULL,
    `quantity` INT NOT NULL DEFAULT 0,
    `grade` VARCHAR(20) DEFAULT NULL,
    `fqc_record_id` VARCHAR(50) NOT NULL COMMENT '关联FQC检验记录',
    `location_id` VARCHAR(50) NOT NULL,
    `location_name` VARCHAR(100) DEFAULT NULL,
    `received_by` VARCHAR(50) NOT NULL,
    `received_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Completed'
        COMMENT 'Pending/Completed/Cancelled',
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`receipt_id`),
    INDEX `idx_work_order_id` (`work_order_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_product_id` (`product_id`),
    INDEX `idx_received_at` (`received_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 成品库存表
CREATE TABLE IF NOT EXISTS `finished_goods_inventory` (
    `inventory_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `product_name` VARCHAR(100) DEFAULT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `work_order_id` VARCHAR(50) DEFAULT NULL,
    `grade` VARCHAR(20) DEFAULT NULL,
    `location_id` VARCHAR(50) NOT NULL,
    `location_name` VARCHAR(100) DEFAULT NULL,
    `total_qty` INT NOT NULL DEFAULT 0,
    `available_qty` INT NOT NULL DEFAULT 0,
    `shipped_qty` INT NOT NULL DEFAULT 0,
    `receipt_id` VARCHAR(50) NOT NULL,
    `received_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Available'
        COMMENT 'Available/PartiallyShipped/Shipped/Frozen',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`inventory_id`),
    UNIQUE KEY `uk_lot_location` (`lot_id`, `location_id`),
    INDEX `idx_product_id` (`product_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_grade` (`grade`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 出货单表
CREATE TABLE IF NOT EXISTS `finished_goods_shipment` (
    `shipment_id` VARCHAR(50) NOT NULL COMMENT '格式: FGS-YYYYMMDD-NNN',
    `customer_id` VARCHAR(50) DEFAULT NULL,
    `customer_name` VARCHAR(100) DEFAULT NULL,
    `oqc_record_id` VARCHAR(50) DEFAULT NULL COMMENT '关联OQC检验记录',
    `carrier` VARCHAR(100) DEFAULT NULL,
    `tracking_no` VARCHAR(100) DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/Approved/Shipped/Delivered/Cancelled',
    `shipped_by` VARCHAR(50) DEFAULT NULL,
    `shipped_at` DATETIME DEFAULT NULL,
    `shipped_to` VARCHAR(255) DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`shipment_id`),
    INDEX `idx_customer_id` (`customer_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 出货单明细表
CREATE TABLE IF NOT EXISTS `finished_goods_shipment_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `shipment_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `grade` VARCHAR(20) DEFAULT NULL,
    `quantity` INT NOT NULL DEFAULT 0,
    `inventory_id` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`item_id`),
    INDEX `idx_shipment_id` (`shipment_id`),
    INDEX `idx_lot_id` (`lot_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
