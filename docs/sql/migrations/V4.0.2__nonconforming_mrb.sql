-- ============================================================
-- V4.0.2: 不合格品管理与 MRB 评审
-- 描述: 创建不合格品记录(NCR)、MRB 评审表、评审明细表及处置执行记录表
-- 依赖: V4.0.0, V4.0.1
-- 作者: MES Team
-- 日期: 2026-06-07
-- ============================================================

-- 不合格品记录表 (NCR)
CREATE TABLE IF NOT EXISTS `nonconforming_record` (
    `ncr_id` VARCHAR(50) NOT NULL COMMENT '格式: NCR-YYYYMMDD-NNN',
    `source` VARCHAR(50) NOT NULL COMMENT 'IQC/FQC/OQC/Process/Audit/Customer',
    `source_reference` VARCHAR(50) DEFAULT NULL COMMENT '来源检验任务ID或工单号',
    `lot_id` VARCHAR(50) NOT NULL,
    `batch_id` VARCHAR(50) DEFAULT NULL,
    `work_order_id` VARCHAR(50) DEFAULT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `product_name` VARCHAR(100) DEFAULT NULL,
    `defect_code` VARCHAR(50) NOT NULL,
    `defect_description` TEXT NOT NULL,
    `defect_category` VARCHAR(20) NOT NULL COMMENT 'Critical/Major/Minor',
    `affected_qty` INT NOT NULL DEFAULT 0,
    `reported_by` VARCHAR(50) NOT NULL,
    `reported_by_name` VARCHAR(100) DEFAULT NULL,
    `reported_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `isolation_status` VARCHAR(20) NOT NULL DEFAULT 'Isolated'
        COMMENT 'Isolated/Released',
    `isolation_time` DATETIME DEFAULT NULL,
    `mrb_id` VARCHAR(50) DEFAULT NULL,
    `mrb_status` VARCHAR(20) DEFAULT NULL
        COMMENT 'NotRequired/Pending/InProgress/Completed/Cancelled',
    `disposition` VARCHAR(20) DEFAULT NULL
        COMMENT 'Rework/Scrap/Concession/Return/UseAsIs',
    `disposition_detail` TEXT DEFAULT NULL,
    `disposition_executed` TINYINT(1) DEFAULT 0 COMMENT '处置是否已执行',
    `disposition_executed_at` DATETIME DEFAULT NULL,
    `rework_verify_result` VARCHAR(20) DEFAULT NULL COMMENT 'Pass/Fail',
    `rework_verify_time` DATETIME DEFAULT NULL,
    `status` VARCHAR(20) NOT NULL DEFAULT 'Open'
        COMMENT 'Open/InReview/Disposed/Closed/Cancelled',
    `closed_at` DATETIME DEFAULT NULL,
    `closed_by` VARCHAR(50) DEFAULT NULL,
    `remark` TEXT DEFAULT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`ncr_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_product_id` (`product_id`),
    INDEX `idx_source` (`source`),
    INDEX `idx_status` (`status`),
    INDEX `idx_defect_code` (`defect_code`),
    INDEX `idx_reported_at` (`reported_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- MRB 评审表
CREATE TABLE IF NOT EXISTS `mrb_review` (
    `mrb_id` VARCHAR(50) NOT NULL COMMENT '格式: MRB-YYYYMMDD-NNN',
    `ncr_id` VARCHAR(50) NOT NULL,
    `lot_id` VARCHAR(50) NOT NULL,
    `product_id` VARCHAR(50) NOT NULL,
    `affected_qty` INT NOT NULL DEFAULT 0,
    `review_type` VARCHAR(20) DEFAULT 'Standard'
        COMMENT 'Standard/Expedited/Escalated',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending'
        COMMENT 'Pending/InProgress/Completed/Cancelled',
    `quality_vote` VARCHAR(20) DEFAULT NULL COMMENT 'Approve/Reject/ConditionalApprove',
    `quality_voter` VARCHAR(50) DEFAULT NULL,
    `quality_vote_time` DATETIME DEFAULT NULL,
    `quality_comment` TEXT DEFAULT NULL,
    `process_vote` VARCHAR(20) DEFAULT NULL,
    `process_voter` VARCHAR(50) DEFAULT NULL,
    `process_vote_time` DATETIME DEFAULT NULL,
    `process_comment` TEXT DEFAULT NULL,
    `engineering_vote` VARCHAR(20) DEFAULT NULL,
    `engineering_voter` VARCHAR(50) DEFAULT NULL,
    `engineering_vote_time` DATETIME DEFAULT NULL,
    `engineering_comment` TEXT DEFAULT NULL,
    `final_disposition` VARCHAR(20) DEFAULT NULL
        COMMENT 'Rework/Scrap/Concession/Return/UseAsIs',
    `final_decision_by` VARCHAR(50) DEFAULT NULL,
    `final_decision_time` DATETIME DEFAULT NULL,
    `concession_limit` VARCHAR(100) DEFAULT NULL COMMENT '让步条件',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `completed_at` DATETIME DEFAULT NULL,
    PRIMARY KEY (`mrb_id`),
    UNIQUE KEY `uk_ncr_mrb` (`ncr_id`),
    INDEX `idx_lot_id` (`lot_id`),
    INDEX `idx_status` (`status`),
    INDEX `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- MRB 评审明细表
CREATE TABLE IF NOT EXISTS `mrb_review_item` (
    `item_id` VARCHAR(50) NOT NULL,
    `mrb_id` VARCHAR(50) NOT NULL,
    `voter_role` VARCHAR(50) NOT NULL COMMENT 'Quality/Process/Engineering',
    `voter_id` VARCHAR(50) NOT NULL,
    `voter_name` VARCHAR(100) DEFAULT NULL,
    `vote` VARCHAR(20) NOT NULL COMMENT 'Approve/Reject/ConditionalApprove',
    `disposition_recommendation` VARCHAR(50) DEFAULT NULL,
    `comment` TEXT DEFAULT NULL,
    `vote_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`item_id`),
    UNIQUE KEY `uk_mrb_role` (`mrb_id`, `voter_role`),
    INDEX `idx_mrb_id` (`mrb_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 处置执行记录表
CREATE TABLE IF NOT EXISTS `disposition_record` (
    `disposition_id` VARCHAR(50) NOT NULL,
    `ncr_id` VARCHAR(50) NOT NULL,
    `mrb_id` VARCHAR(50) DEFAULT NULL,
    `disposition_type` VARCHAR(20) NOT NULL
        COMMENT 'Rework/Scrap/Concession/Return/UseAsIs',
    `detail` TEXT DEFAULT NULL,
    `executed_by` VARCHAR(50) NOT NULL,
    `executed_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `verified_by` VARCHAR(50) DEFAULT NULL,
    `verified_at` DATETIME DEFAULT NULL,
    `verify_result` VARCHAR(20) DEFAULT NULL,
    `related_work_order_id` VARCHAR(50) DEFAULT NULL COMMENT '返工工单号',
    `related_scrap_id` VARCHAR(50) DEFAULT NULL COMMENT '报废单号',
    PRIMARY KEY (`disposition_id`),
    INDEX `idx_ncr_id` (`ncr_id`),
    INDEX `idx_disposition_type` (`disposition_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
