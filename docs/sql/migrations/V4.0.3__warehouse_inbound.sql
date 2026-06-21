-- ============================================================
-- V4.0.3: 原材料入库 / FIFO / 有效期
-- 描述: 创建原材料入库单、库存台账、库位及物料有效期记录表
-- 依赖: V4.0.0 (iqc_incoming_batch)
-- 作者: MES Team
-- 日期: 2026-06-07
-- ============================================================

-- 原材料入库单表
CREATE TABLE IF NOT EXISTS `warehouse_receipt` (
    `receipt_id` VARCHAR(50) NOT NULL COMMENT '格式: WR-YYYYMMDD-NNN',
    `batch_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) DEFAULT NULL,
    `quantity` INT NOT NULL DEFAULT 0,
    `unit` VARCHAR(20) DEFAULT 'PCS',
    `supplier_id` VARCHAR(50) DEFAULT NULL,
    `supplier_batch_no` VARCHAR(100) DEFAULT NULL,
    `location_id` VARCHAR(50) NOT NULL COMMENT '目标库位',
    `location_name` VARCHAR(100) DEFAULT NULL,
    `expiry_date` DATE DEFAULT NULL,
    `shelf_life_days` INT DEFAULT NULL,
    `msl_level` INT DEFAULT NULL,
    `msl_floor_life_hours` INT DEFAULT NULL,
    `iqc_task_id` VARCHAR(50) DEFAULT NULL,
    `purchase_order_no` VARCHAR(50) DEFAULT NULL,
    `received_by` VARCHAR(50) NOT NULL,
    `received_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Completed' COMMENT 'Pending/Completed/Cancelled',
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`receipt_id`),
    UNIQUE KEY `uk_batch_receipt` (`batch_id`),
    INDEX `idx_material_id` (`material_id`),
    INDEX `idx_location_id` (`location_id`),
    INDEX `idx_received_at` (`received_at`),
    INDEX `idx_expiry_date` (`expiry_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 库存台账表
CREATE TABLE IF NOT EXISTS `warehouse_inventory` (
    `inventory_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) DEFAULT NULL,
    `batch_id` VARCHAR(50) NOT NULL,
    `supplier_batch_no` VARCHAR(100) DEFAULT NULL,
    `location_id` VARCHAR(50) NOT NULL,
    `location_name` VARCHAR(100) DEFAULT NULL,
    `total_qty` INT NOT NULL DEFAULT 0,
    `available_qty` INT NOT NULL DEFAULT 0,
    `locked_qty` INT NOT NULL DEFAULT 0,
    `allocated_qty` INT NOT NULL DEFAULT 0,
    `unit` VARCHAR(20) DEFAULT 'PCS',
    `expiry_date` DATE DEFAULT NULL,
    `shelf_life_days` INT DEFAULT NULL,
    `msl_level` INT DEFAULT NULL,
    `msl_floor_life_hours` INT DEFAULT NULL,
    `msl_exposure_start` DATETIME DEFAULT NULL,
    `msl_expiry` DATETIME DEFAULT NULL,
    `received_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Available'
        COMMENT 'Available/PartiallyUsed/Locked/Expired',
    `lock_reason` VARCHAR(255) DEFAULT NULL,
    `last_issue_at` DATETIME DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`inventory_id`),
    UNIQUE KEY `uk_batch_location` (`batch_id`, `location_id`),
    INDEX `idx_material_id` (`material_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_expiry_date` (`expiry_date`),
    INDEX `idx_received_at` (`received_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 库位表
CREATE TABLE IF NOT EXISTS `warehouse_location` (
    `location_id` VARCHAR(50) NOT NULL COMMENT '格式: WH-A-01',
    `location_name` VARCHAR(100) NOT NULL,
    `warehouse_zone` VARCHAR(50) NOT NULL COMMENT 'Warehouse/LineSide/Quarantine',
    `location_type` VARCHAR(50) DEFAULT NULL COMMENT 'Shelf/Rack/Bin/Floor',
    `temperature_controlled` TINYINT(1) DEFAULT 0,
    `humidity_controlled` TINYINT(1) DEFAULT 0,
    `max_capacity` INT DEFAULT NULL,
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`location_id`),
    INDEX `idx_zone` (`warehouse_zone`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 物料有效期记录表
CREATE TABLE IF NOT EXISTS `material_shelf_life` (
    `shelf_life_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `batch_id` VARCHAR(50) NOT NULL,
    `shelf_life_days` INT NOT NULL,
    `manufacturing_date` DATE DEFAULT NULL,
    `expiry_date` DATE NOT NULL,
    `warning_days_before` INT DEFAULT 30 COMMENT '预警天数',
    `alert_level` VARCHAR(20) NOT NULL DEFAULT 'Normal'
        COMMENT 'Normal/Warning/Urgent/Expired',
    `is_expired` TINYINT(1) NOT NULL DEFAULT 0,
    `locked_at` DATETIME DEFAULT NULL,
    `lock_reason` VARCHAR(255) DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`shelf_life_id`),
    UNIQUE KEY `uk_batch_shelf` (`batch_id`),
    INDEX `idx_material_id` (`material_id`),
    INDEX `idx_expiry_date` (`expiry_date`),
    INDEX `idx_alert_level` (`alert_level`),
    INDEX `idx_is_expired` (`is_expired`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
