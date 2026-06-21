-- ============================================================
-- V4.0.0: IQC 来料检验管理
-- 描述: 创建 IQC 来料批次、检验任务、检验结果、检验标准及供应商质量统计表
-- 依赖: 无
-- 作者: MES Team
-- 日期: 2026-06-07
-- ============================================================

-- IQC 来料批次表
CREATE TABLE IF NOT EXISTS `iqc_incoming_batch` (
    `batch_id` VARCHAR(50) NOT NULL COMMENT '批次ID (格式: IB-YYYYMMDD-NNN)',
    `material_id` VARCHAR(50) NOT NULL,
    `material_name` VARCHAR(100) DEFAULT NULL,
    `supplier_id` VARCHAR(50) NOT NULL,
    `supplier_name` VARCHAR(100) DEFAULT NULL,
    `supplier_batch_no` VARCHAR(100) NOT NULL COMMENT '供应商批次号',
    `quantity` INT NOT NULL DEFAULT 0,
    `unit` VARCHAR(20) DEFAULT 'PCS',
    `purchase_order_no` VARCHAR(50) DEFAULT NULL,
    `manufacturing_date` DATE DEFAULT NULL,
    `expiry_date` DATE DEFAULT NULL,
    `msl_level` INT DEFAULT NULL COMMENT '湿度敏感等级 1~6',
    `msl_floor_life_hours` INT DEFAULT NULL COMMENT '车间寿命(小时)',
    `msl_exposure_start` DATETIME DEFAULT NULL COMMENT '暴露开始时间',
    `msl_expiry` DATETIME DEFAULT NULL COMMENT 'MSL过期时间',
    `coa_reference` VARCHAR(100) DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'PendingInspection'
        COMMENT 'PendingInspection/Inspecting/Passed/Failed/Frozen',
    `inspection_status` VARCHAR(20) DEFAULT 'NotStarted'
        COMMENT 'NotStarted/InProgress/Completed',
    `judgment_result` VARCHAR(20) DEFAULT NULL COMMENT 'Pass/Fail/Conditional',
    `disposition` VARCHAR(20) DEFAULT NULL COMMENT 'Accept/Reject/Return/Concession',
    `iqc_task_id` VARCHAR(50) DEFAULT NULL,
    `ncr_id` VARCHAR(50) DEFAULT NULL,
    `mrb_id` VARCHAR(50) DEFAULT NULL,
    `warehouse_location` VARCHAR(50) DEFAULT NULL COMMENT '入库库位',
    `received_by` VARCHAR(50) DEFAULT NULL,
    `received_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`batch_id`),
    INDEX `idx_material_id` (`material_id`),
    INDEX `idx_supplier_id` (`supplier_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_supplier_batch_no` (`supplier_batch_no`),
    INDEX `idx_received_at` (`received_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- IQC 检验任务表
CREATE TABLE IF NOT EXISTS `iqc_inspection_task` (
    `task_id` VARCHAR(50) NOT NULL COMMENT '任务ID (格式: IQC-YYYYMMDD-NNN)',
    `batch_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `inspection_type` VARCHAR(50) NOT NULL DEFAULT 'Incoming'
        COMMENT 'Incoming/ReInspect/Special',
    `sampling_plan` VARCHAR(50) DEFAULT NULL COMMENT '抽样方案 (AQL标准)',
    `sample_size` INT DEFAULT NULL COMMENT '抽样数量',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/InProgress/Completed/Cancelled',
    `assigned_to` VARCHAR(50) DEFAULT NULL COMMENT '分配给谁',
    `assigned_at` DATETIME DEFAULT NULL,
    `started_at` DATETIME DEFAULT NULL,
    `completed_at` DATETIME DEFAULT NULL,
    `priority` VARCHAR(20) DEFAULT 'Normal' COMMENT 'Urgent/High/Normal/Low',
    `due_time` DATETIME DEFAULT NULL COMMENT '要求完成时间',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`task_id`),
    UNIQUE KEY `uk_batch_task` (`batch_id`),
    INDEX `idx_material_id` (`material_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_assigned_to` (`assigned_to`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- IQC 检验结果明细表
CREATE TABLE IF NOT EXISTS `iqc_inspection_result` (
    `result_id` VARCHAR(50) NOT NULL,
    `task_id` VARCHAR(50) NOT NULL,
    `batch_id` VARCHAR(50) NOT NULL,
    `item_code` VARCHAR(50) NOT NULL COMMENT '检验项目编号',
    `item_name` VARCHAR(100) NOT NULL,
    `item_type` VARCHAR(20) NOT NULL COMMENT 'Dimensional/Electrical/Visual/Functional',
    `standard_value` VARCHAR(50) DEFAULT NULL,
    `lower_limit` VARCHAR(50) DEFAULT NULL,
    `upper_limit` VARCHAR(50) DEFAULT NULL,
    `actual_value` VARCHAR(50) DEFAULT NULL,
    `unit` VARCHAR(20) DEFAULT NULL,
    `result` VARCHAR(20) NOT NULL COMMENT 'Pass/Fail/N/A',
    `measuring_equipment` VARCHAR(50) DEFAULT NULL COMMENT '测量设备',
    `inspector_id` VARCHAR(50) NOT NULL,
    `inspector_name` VARCHAR(100) DEFAULT NULL,
    `inspected_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `remark` TEXT DEFAULT NULL,
    PRIMARY KEY (`result_id`),
    INDEX `idx_task_id` (`task_id`),
    INDEX `idx_batch_id` (`batch_id`),
    INDEX `idx_item_code` (`item_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- IQC 检验标准表
CREATE TABLE IF NOT EXISTS `iqc_inspection_standard` (
    `standard_id` VARCHAR(50) NOT NULL,
    `material_id` VARCHAR(50) NOT NULL,
    `inspection_item_code` VARCHAR(50) NOT NULL,
    `inspection_item_name` VARCHAR(100) NOT NULL,
    `item_type` VARCHAR(20) NOT NULL,
    `standard_value` VARCHAR(50) DEFAULT NULL,
    `lower_limit` VARCHAR(50) DEFAULT NULL,
    `upper_limit` VARCHAR(50) DEFAULT NULL,
    `unit` VARCHAR(20) DEFAULT NULL,
    `sampling_plan` VARCHAR(50) DEFAULT 'AQL-0.65',
    `is_mandatory` TINYINT(1) NOT NULL DEFAULT 1,
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`standard_id`),
    UNIQUE KEY `uk_material_item` (`material_id`, `inspection_item_code`),
    INDEX `idx_material_id` (`material_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 供应商质量统计表
CREATE TABLE IF NOT EXISTS `iqc_supplier_quality_stat` (
    `stat_id` VARCHAR(50) NOT NULL,
    `supplier_id` VARCHAR(50) NOT NULL,
    `supplier_name` VARCHAR(100) DEFAULT NULL,
    `stat_month` VARCHAR(7) NOT NULL COMMENT '格式: YYYY-MM',
    `total_batches` INT NOT NULL DEFAULT 0,
    `passed_batches` INT NOT NULL DEFAULT 0,
    `failed_batches` INT NOT NULL DEFAULT 0,
    `pass_rate` DECIMAL(5,2) DEFAULT 0,
    `major_defect_count` INT NOT NULL DEFAULT 0,
    `minor_defect_count` INT NOT NULL DEFAULT 0,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`stat_id`),
    UNIQUE KEY `uk_supplier_month` (`supplier_id`, `stat_month`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 扩展 master_material 增加 IQC 相关字段
ALTER TABLE `master_material`
    ADD COLUMN IF NOT EXISTS `inspection_required` TINYINT(1) DEFAULT 1
        COMMENT '是否需要IQC检验',
    ADD COLUMN IF NOT EXISTS `inspection_standard_id` VARCHAR(50) DEFAULT NULL
        COMMENT '默认检验标准ID',
    ADD COLUMN IF NOT EXISTS `msl_controlled` TINYINT(1) DEFAULT 0
        COMMENT '是否MSL管控';
