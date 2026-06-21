-- V4.5.0: Process Parameter Control & Bin Management
-- 工序参数管控 + Bin 管理

-- ===========================
-- 1. 工序参数集表
-- ===========================
CREATE TABLE IF NOT EXISTS `process_parameter_set` (
    `set_id` VARCHAR(50) PRIMARY KEY COMMENT '参数集ID',
    `set_name` VARCHAR(200) NOT NULL COMMENT '参数集名称',
    `product_id` VARCHAR(50) NOT NULL COMMENT '产品ID',
    `process_step` VARCHAR(50) NOT NULL COMMENT '工序: Thinning/DieAttach/WireBond/Molding/Curing',
    `recipe_id` VARCHAR(50) COMMENT '关联Recipe ID',
    `version` VARCHAR(20) NOT NULL DEFAULT 'V1.0' COMMENT '版本号',
    `status` VARCHAR(30) DEFAULT 'Draft' COMMENT '状态: Draft/Active/Archived',
    `description` TEXT COMMENT '描述',
    `created_by` VARCHAR(50) NOT NULL COMMENT '创建人',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `approved_by` VARCHAR(50) COMMENT '审批人',
    `approved_at` DATETIME COMMENT '审批时间',
    `deleted` TINYINT(1) DEFAULT 0 COMMENT '逻辑删除',
    INDEX `idx_pps_product_process` (`product_id`, `process_step`),
    INDEX `idx_pps_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工序参数集表';

-- ===========================
-- 2. 参数明细表
-- ===========================
CREATE TABLE IF NOT EXISTS `process_parameter_item` (
    `item_id` BIGINT AUTO_INCREMENT PRIMARY KEY,
    `set_id` VARCHAR(50) NOT NULL COMMENT '参数集ID',
    `param_name` VARCHAR(100) NOT NULL COMMENT '参数名称',
    `param_code` VARCHAR(50) NOT NULL COMMENT '参数编码',
    `data_type` VARCHAR(20) NOT NULL COMMENT '数据类型: Number/Text/Boolean/Enum',
    `unit` VARCHAR(20) COMMENT '单位',
    `target_value` VARCHAR(50) COMMENT '目标值',
    `upper_limit` VARCHAR(50) COMMENT '上限',
    `lower_limit` VARCHAR(50) COMMENT '下限',
    `is_required` TINYINT(1) DEFAULT 1 COMMENT '是否必填',
    `display_order` INT DEFAULT 0 COMMENT '显示顺序',
    INDEX `idx_ppi_set_id` (`set_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工序参数明细表';

-- ===========================
-- 3. 参数修改日志表
-- ===========================
CREATE TABLE IF NOT EXISTS `process_parameter_override_log` (
    `log_id` VARCHAR(50) PRIMARY KEY,
    `set_id` VARCHAR(50) NOT NULL COMMENT '参数集ID',
    `work_order_id` VARCHAR(50) COMMENT '工单ID',
    `param_item_id` BIGINT NOT NULL COMMENT '参数明细ID',
    `original_value` VARCHAR(100) COMMENT '原值',
    `override_value` VARCHAR(100) COMMENT '修改后值',
    `override_reason` TEXT COMMENT '修改原因',
    `requested_by` VARCHAR(50) NOT NULL COMMENT '申请人',
    `requested_at` DATETIME NOT NULL COMMENT '申请时间',
    `approved_by` VARCHAR(50) COMMENT '审批人',
    `approved_at` DATETIME COMMENT '审批时间',
    `status` VARCHAR(30) DEFAULT 'Pending' COMMENT '状态: Pending/Approved/Rejected',
    INDEX `idx_ppol_set` (`set_id`),
    INDEX `idx_ppol_wo` (`work_order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工序参数修改日志表';

-- ===========================
-- 4. 固化温度曲线表
-- ===========================
CREATE TABLE IF NOT EXISTS `curing_temperature_curve` (
    `curve_id` VARCHAR(50) PRIMARY KEY,
    `product_id` VARCHAR(50) NOT NULL COMMENT '产品ID',
    `curve_name` VARCHAR(200) NOT NULL COMMENT '曲线名称',
    `zone_count` INT NOT NULL COMMENT '温区数量',
    `conveyor_speed` DECIMAL(10,2) COMMENT '传送速度(cm/min)',
    `peak_temperature` DECIMAL(8,2) COMMENT '峰值温度(℃)',
    `tali_time` DECIMAL(8,2) COMMENT '保温时间(min)',
    `status` VARCHAR(30) DEFAULT 'Draft' COMMENT '状态',
    `created_by` VARCHAR(50) NOT NULL,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX `idx_ctc_product` (`product_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='固化温度曲线表';

-- ===========================
-- 5. Bin 定义表
-- ===========================
CREATE TABLE IF NOT EXISTS `bin_definition` (
    `bin_id` INT AUTO_INCREMENT PRIMARY KEY,
    `bin_code` VARCHAR(20) NOT NULL COMMENT 'Bin 编码(001-064)',
    `bin_name` VARCHAR(100) NOT NULL COMMENT 'Bin 名称',
    `bin_type` VARCHAR(30) NOT NULL COMMENT '类型: Pass/Fail/Meta',
    `bin_category` VARCHAR(50) COMMENT '分类: Electrical/Visual/Functional',
    `description` TEXT COMMENT '描述',
    `is_active` TINYINT(1) DEFAULT 1 COMMENT '是否启用',
    `sort_order` INT DEFAULT 0 COMMENT '排序',
    UNIQUE INDEX `uk_bin_code` (`bin_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Bin 定义表';

-- ===========================
-- 6. Bin 分选记录表
-- ===========================
CREATE TABLE IF NOT EXISTS `bin_sort_record` (
    `record_id` VARCHAR(50) PRIMARY KEY,
    `work_order_id` VARCHAR(50) NOT NULL COMMENT '工单ID',
    `lot_id` VARCHAR(50) COMMENT '批次ID',
    `bin_id` INT NOT NULL COMMENT 'Bin ID',
    `quantity` INT NOT NULL COMMENT '数量',
    `sort_time` DATETIME NOT NULL COMMENT '分选时间',
    `operator_id` VARCHAR(50) NOT NULL COMMENT '操作人',
    `equipment_id` VARCHAR(50) COMMENT '设备ID',
    `test_program` VARCHAR(100) COMMENT '测试程序',
    `comments` TEXT COMMENT '备注',
    INDEX `idx_bsr_wo` (`work_order_id`),
    INDEX `idx_bsr_lot` (`lot_id`),
    INDEX `idx_bsr_time` (`sort_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Bin 分选记录表';

-- ===========================
-- 7. Bin 统计表
-- ===========================
CREATE TABLE IF NOT EXISTS `bin_statistics` (
    `stat_id` VARCHAR(50) PRIMARY KEY,
    `work_order_id` VARCHAR(50) NOT NULL COMMENT '工单ID',
    `bin_id` INT NOT NULL COMMENT 'Bin ID',
    `total_quantity` INT DEFAULT 0 COMMENT '累计数量',
    `percentage` DECIMAL(5,2) COMMENT '占比(%)',
    `last_updated` DATETIME NOT NULL COMMENT '最后更新时间',
    UNIQUE INDEX `uk_bstat_wo_bin` (`work_order_id`, `bin_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Bin 统计表';
