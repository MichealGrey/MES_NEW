-- ============================================================================
-- MES Database Migration V4.8.0
-- Analytics and NPI Module Tables
-- ============================================================================

-- 1. KPI Dashboard Snapshot
CREATE TABLE IF NOT EXISTS `kpi_dashboard_snapshot` (
    `snapshot_id` VARCHAR(50) PRIMARY KEY,
    `snapshot_date` DATETIME NOT NULL COMMENT '快照时间',
    `metric_code` VARCHAR(50) NOT NULL COMMENT '指标代码: YieldRate/OTD/OEE/DPPM/OutputRate/CustomerComplaint/WIPCount',
    `metric_name` VARCHAR(200) NOT NULL COMMENT '指标名称',
    `metric_value` DECIMAL(12,4) COMMENT '指标值',
    `target_value` DECIMAL(12,4) COMMENT '目标值',
    `unit` VARCHAR(20) COMMENT '单位: %/ppm/pcs',
    `status` VARCHAR(20) DEFAULT 'Normal' COMMENT '状态: Normal/Warning/Critical',
    `trend` VARCHAR(20) COMMENT '趋势: Up/Down/Stable',
    `period_type` VARCHAR(20) DEFAULT 'Daily' COMMENT '周期: Hourly/Daily/Weekly/Monthly',
    `detail_data` JSON COMMENT '明细数据',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_kds_date_metric` (`snapshot_date`, `metric_code`),
    INDEX `idx_kds_period` (`period_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='KPI看板快照表';

-- 2. Cost Record
CREATE TABLE IF NOT EXISTS `cost_record` (
    `cost_id` VARCHAR(50) PRIMARY KEY,
    `work_order_id` VARCHAR(50) NOT NULL COMMENT '工单ID',
    `product_id` VARCHAR(50) NOT NULL COMMENT '产品ID',
    `cost_date` DATE NOT NULL COMMENT '成本日期',
    `cost_type` VARCHAR(30) NOT NULL COMMENT '类型: DirectMaterial/DirectLabor/ManufacturingOverhead',
    `amount` DECIMAL(12,2) NOT NULL COMMENT '金额',
    `currency` VARCHAR(10) DEFAULT 'CNY' COMMENT '币种',
    `detail_json` JSON COMMENT '成本明细',
    `created_by` VARCHAR(50),
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_cr_work_order` (`work_order_id`),
    INDEX `idx_cr_date` (`cost_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='成本记录表';

-- 3. Yield Statistics
CREATE TABLE IF NOT EXISTS `yield_statistics` (
    `stat_id` VARCHAR(50) PRIMARY KEY,
    `lot_id` VARCHAR(50) NOT NULL COMMENT '批次ID',
    `step_code` VARCHAR(50) COMMENT '工序代码',
    `step_name` VARCHAR(100) COMMENT '工序名称',
    `input_qty` INT NOT NULL COMMENT '投入数量',
    `output_qty` INT NOT NULL COMMENT '产出数量',
    `yield_rate` DECIMAL(5,2) COMMENT '良率(%)',
    `scrap_qty` INT DEFAULT 0 COMMENT '报废数量',
    `rework_qty` INT DEFAULT 0 COMMENT '返工数量',
    `defect_json` JSON COMMENT '缺陷明细',
    `stat_date` DATE NOT NULL COMMENT '统计日期',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_ys_lot` (`lot_id`),
    INDEX `idx_ys_date` (`stat_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='良率统计表';

-- 4. NPI Project
CREATE TABLE IF NOT EXISTS `npi_project` (
    `project_id` VARCHAR(50) PRIMARY KEY,
    `project_code` VARCHAR(50) NOT NULL COMMENT '项目代码',
    `project_name` VARCHAR(200) NOT NULL COMMENT '项目名称',
    `product_id` VARCHAR(50) COMMENT '关联产品ID',
    `current_stage` VARCHAR(30) DEFAULT 'Initiation' COMMENT '当前阶段: Initiation/Design/TrialRun/Review/MassProduction',
    `project_manager` VARCHAR(100) COMMENT '项目经理',
    `target_date` DATE COMMENT '目标日期',
    `actual_date` DATE COMMENT '实际完成日期',
    `status` VARCHAR(30) DEFAULT 'Active' COMMENT '状态: Active/OnHold/Completed/Cancelled',
    `description` TEXT,
    `created_by` VARCHAR(50),
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_by` VARCHAR(50),
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_npi_status` (`status`),
    INDEX `idx_npi_stage` (`current_stage`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='NPI项目表';

-- 5. NPI Stage
CREATE TABLE IF NOT EXISTS `npi_stage` (
    `stage_id` VARCHAR(50) PRIMARY KEY,
    `project_id` VARCHAR(50) NOT NULL COMMENT '项目ID',
    `stage_name` VARCHAR(100) NOT NULL COMMENT '阶段名称',
    `stage_order` INT COMMENT '阶段顺序',
    `status` VARCHAR(30) DEFAULT 'Pending' COMMENT '状态: Pending/InProgress/Completed/Skipped',
    `start_date` DATETIME,
    `end_date` DATETIME,
    `result` TEXT COMMENT '阶段结果',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_ns_project` (`project_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='NPI阶段表';

-- 6. Reliability Test Plan
CREATE TABLE IF NOT EXISTS `reliability_test_plan` (
    `plan_id` VARCHAR(50) PRIMARY KEY,
    `plan_name` VARCHAR(200) NOT NULL COMMENT '计划名称',
    `test_type` VARCHAR(30) NOT NULL COMMENT '类型: HTOL/THB/TC/uHAST/ESD/LatchUp',
    `product_id` VARCHAR(50) NOT NULL COMMENT '产品ID',
    `lot_id` VARCHAR(50) COMMENT '批次ID',
    `sample_size` INT COMMENT '样本数量',
    `test_duration` INT COMMENT '测试时长(小时)',
    `test_conditions` TEXT COMMENT '测试条件',
    `start_date` DATETIME,
    `end_date` DATETIME,
    `status` VARCHAR(30) DEFAULT 'Planned' COMMENT '状态: Planned/InProgress/Completed/Failed',
    `result_summary` TEXT,
    `fa_triggered` TINYINT(1) DEFAULT 0 COMMENT '是否触发FA',
    `created_by` VARCHAR(50),
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_rtp_type` (`test_type`),
    INDEX `idx_rtp_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='可靠性测试计划表';

-- 7. Report Schedule
CREATE TABLE IF NOT EXISTS `report_schedule` (
    `schedule_id` VARCHAR(50) PRIMARY KEY,
    `report_name` VARCHAR(200) NOT NULL COMMENT '报表名称',
    `report_type` VARCHAR(30) NOT NULL COMMENT '类型: Daily/Weekly/Monthly/Custom',
    `schedule_cron` VARCHAR(100) COMMENT 'Cron表达式',
    `recipients` JSON COMMENT '接收人列表',
    `format` VARCHAR(20) DEFAULT 'PDF' COMMENT '格式: PDF/Excel/HTML',
    `enabled` TINYINT(1) DEFAULT 1 COMMENT '是否启用',
    `last_run_date` DATETIME,
    `next_run_date` DATETIME,
    `created_by` VARCHAR(50),
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_rs_type` (`report_type`),
    INDEX `idx_rs_enabled` (`enabled`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='报表计划表';

-- 8. System Config
CREATE TABLE IF NOT EXISTS `system_config` (
    `config_id` VARCHAR(50) PRIMARY KEY,
    `config_key` VARCHAR(100) NOT NULL UNIQUE COMMENT '配置键',
    `config_value` TEXT COMMENT '配置值',
    `config_type` VARCHAR(30) COMMENT '类型: String/Number/Boolean/JSON',
    `category` VARCHAR(50) COMMENT '分类: General/Quality/Production/Alert/Workflow',
    `description` VARCHAR(500),
    `is_public` TINYINT(1) DEFAULT 1 COMMENT '是否公开',
    `updated_by` VARCHAR(50),
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_sc_category` (`category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='系统参数配置表';

-- 9. Alert Rule
CREATE TABLE IF NOT EXISTS `alert_rule` (
    `rule_id` VARCHAR(50) PRIMARY KEY,
    `rule_name` VARCHAR(200) NOT NULL COMMENT '规则名称',
    `rule_type` VARCHAR(30) NOT NULL COMMENT '类型: Yield/Equipment/Material/Quality/Schedule',
    `condition_expression` TEXT COMMENT '条件表达式',
    `threshold_value` DECIMAL(10,2) COMMENT '阈值',
    `severity` VARCHAR(20) DEFAULT 'Warning' COMMENT '级别: Info/Warning/Critical',
    `notification_channels` JSON COMMENT '通知渠道: Email/SMS/System',
    `enabled` TINYINT(1) DEFAULT 1,
    `created_by` VARCHAR(50),
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_ar_type` (`rule_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='预警规则表';
