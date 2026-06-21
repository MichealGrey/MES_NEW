-- ============================================================================
-- MES Database Migration V4.9.0
-- Audit Trail Enhancement and Data Correction
-- ============================================================================

-- 1. Enhance audit_trail with blockchain-style hash chain for data integrity
ALTER TABLE `audit_trail` ADD COLUMN IF NOT EXISTS `hash_value` VARCHAR(128) COMMENT '当前记录哈希值';
ALTER TABLE `audit_trail` ADD COLUMN IF NOT EXISTS `prev_hash` VARCHAR(128) COMMENT '前一条记录哈希值(链式)';

-- 2. Data Correction Record
CREATE TABLE IF NOT EXISTS `data_correction_record` (
    `correction_id` VARCHAR(50) PRIMARY KEY,
    `table_name` VARCHAR(100) NOT NULL COMMENT '表名',
    `record_id` VARCHAR(100) NOT NULL COMMENT '记录ID',
    `field_name` VARCHAR(100) NOT NULL COMMENT '字段名',
    `old_value` TEXT COMMENT '原值',
    `new_value` TEXT COMMENT '新值',
    `reason` TEXT NOT NULL COMMENT '修正原因',
    `approved_by` VARCHAR(50) COMMENT '审批人',
    `corrected_by` VARCHAR(50) NOT NULL COMMENT '修正人',
    `corrected_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_dcr_table_record` (`table_name`, `record_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='数据修正记录表';
