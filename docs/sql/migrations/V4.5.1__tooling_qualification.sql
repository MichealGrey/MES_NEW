-- V4.5.1: Wire Material Switch & Tooling Lifecycle & Operator Qualification
-- 金线/铜线切换 + 模具/刀片/劈刀寿命管理 + 操作员资质校验 + 焊线拉力测试

-- ===========================
-- 1. 线材切换记录表
-- ===========================
CREATE TABLE IF NOT EXISTS `wire_material_switch_record` (
    `switch_id` VARCHAR(50) PRIMARY KEY,
    `work_order_id` VARCHAR(50) NOT NULL COMMENT '工单ID',
    `equipment_id` VARCHAR(50) NOT NULL COMMENT '设备ID',
    `from_material_type` VARCHAR(50) NOT NULL COMMENT '原线材类型: GoldWire/CopperWire/SilverWire',
    `to_material_type` VARCHAR(50) NOT NULL COMMENT '新线材类型',
    `from_wire_reel` VARCHAR(50) COMMENT '原线材卷号',
    `to_wire_reel` VARCHAR(50) COMMENT '新线材卷号',
    `new_parameter_set_id` VARCHAR(50) COMMENT '新参数集ID',
    `switch_reason` TEXT COMMENT '切换原因',
    `first_article_required` TINYINT(1) DEFAULT 1 COMMENT '是否需要首件检验',
    `first_article_completed` TINYINT(1) DEFAULT 0 COMMENT '首件是否已完成',
    `status` VARCHAR(30) DEFAULT 'Pending' COMMENT '状态: Pending/Confirmed/Rejected',
    `requested_by` VARCHAR(50) NOT NULL COMMENT '申请人',
    `requested_at` DATETIME NOT NULL COMMENT '申请时间',
    `confirmed_by` VARCHAR(50) COMMENT '确认人',
    `confirmed_at` DATETIME COMMENT '确认时间',
    INDEX `idx_wmsr_wo` (`work_order_id`),
    INDEX `idx_wmsr_equip` (`equipment_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='线材切换记录表';

-- ===========================
-- 2. 线材消耗记录表
-- ===========================
CREATE TABLE IF NOT EXISTS `wire_consumption` (
    `consumption_id` VARCHAR(50) PRIMARY KEY,
    `work_order_id` VARCHAR(50) NOT NULL COMMENT '工单ID',
    `wire_reel` VARCHAR(50) NOT NULL COMMENT '线材卷号',
    `wire_type` VARCHAR(50) NOT NULL COMMENT '线材类型',
    `wire_diameter` DECIMAL(8,4) COMMENT '线径(mm)',
    `quantity_used` DECIMAL(10,2) NOT NULL COMMENT '消耗数量(m)',
    `usage_date` DATETIME NOT NULL COMMENT '使用时间',
    `operator_id` VARCHAR(50) NOT NULL COMMENT '操作人',
    `lot_id` VARCHAR(50) COMMENT '批次ID',
    INDEX `idx_wc_wo` (`work_order_id`),
    INDEX `idx_wc_reel` (`wire_reel`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='线材消耗记录表';

-- ===========================
-- 3. 工装台账表（模具/刀片/劈刀）
-- ===========================
CREATE TABLE IF NOT EXISTS `tooling_registry` (
    `tooling_id` VARCHAR(50) PRIMARY KEY COMMENT '工装编号',
    `tooling_type` VARCHAR(50) NOT NULL COMMENT '类型: Mold/Blade/Capillary',
    `tooling_name` VARCHAR(200) NOT NULL COMMENT '工装名称',
    `specification` VARCHAR(200) COMMENT '规格型号',
    `manufacturer` VARCHAR(200) COMMENT '制造商',
    `max_lifespan` INT NOT NULL COMMENT '最大寿命(次数)',
    `current_usage` INT DEFAULT 0 COMMENT '当前使用次数',
    `warning_threshold` INT COMMENT '预警阈值(%)',
    `status` VARCHAR(30) DEFAULT 'Available' COMMENT '状态: Available/InUse/Warning/Expired/Maintenance',
    `installed_equipment_id` VARCHAR(50) COMMENT '当前安装设备ID',
    `install_date` DATETIME COMMENT '安装日期',
    `registered_by` VARCHAR(50) NOT NULL COMMENT '注册人',
    `registered_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '注册时间',
    `deleted` TINYINT(1) DEFAULT 0 COMMENT '逻辑删除',
    INDEX `idx_tr_type_status` (`tooling_type`, `status`),
    INDEX `idx_tr_equip` (`installed_equipment_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工装台账表';

-- ===========================
-- 4. 工装使用日志表
-- ===========================
CREATE TABLE IF NOT EXISTS `tooling_usage_log` (
    `usage_id` VARCHAR(50) PRIMARY KEY,
    `tooling_id` VARCHAR(50) NOT NULL COMMENT '工装ID',
    `work_order_id` VARCHAR(50) COMMENT '工单ID',
    `equipment_id` VARCHAR(50) NOT NULL COMMENT '设备ID',
    `usage_count` INT NOT NULL DEFAULT 1 COMMENT '本次使用次数',
    `usage_date` DATETIME NOT NULL COMMENT '使用时间',
    `operator_id` VARCHAR(50) NOT NULL COMMENT '操作人',
    `lot_id` VARCHAR(50) COMMENT '批次ID',
    `notes` TEXT COMMENT '备注',
    INDEX `idx_tul_tooling` (`tooling_id`),
    INDEX `idx_tul_date` (`usage_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工装使用日志表';

-- ===========================
-- 5. 工装更换记录表
-- ===========================
CREATE TABLE IF NOT EXISTS `tooling_replacement_record` (
    `replacement_id` VARCHAR(50) PRIMARY KEY,
    `tooling_id` VARCHAR(50) NOT NULL COMMENT '工装ID',
    `equipment_id` VARCHAR(50) NOT NULL COMMENT '设备ID',
    `replacement_type` VARCHAR(30) NOT NULL COMMENT '类型: Scheduled/LifespanExceeded/Breakdown',
    `old_tooling_usage` INT COMMENT '旧工装使用次数',
    `new_tooling_id` VARCHAR(50) COMMENT '新工装ID',
    `reason` TEXT COMMENT '更换原因',
    `replaced_by` VARCHAR(50) NOT NULL COMMENT '更换人',
    `replaced_at` DATETIME NOT NULL COMMENT '更换时间',
    `verified_by` VARCHAR(50) COMMENT '验证人',
    `verified_at` DATETIME COMMENT '验证时间',
    INDEX `idx_trr_tooling` (`tooling_id`),
    INDEX `idx_trr_equip` (`equipment_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='工装更换记录表';

-- ===========================
-- 6. 操作员资质表
-- ===========================
CREATE TABLE IF NOT EXISTS `operator_qualification` (
    `qualification_id` VARCHAR(50) PRIMARY KEY,
    `operator_id` VARCHAR(50) NOT NULL COMMENT '操作员ID',
    `qualification_type` VARCHAR(50) NOT NULL COMMENT '资质类型: Process/Equipment',
    `qualification_code` VARCHAR(50) NOT NULL COMMENT '资质编码(如 WireBond_OP_Level2)',
    `qualification_name` VARCHAR(200) NOT NULL COMMENT '资质名称',
    `level` VARCHAR(20) DEFAULT 'Basic' COMMENT '等级: Basic/Intermediate/Advanced',
    `issue_date` DATE NOT NULL COMMENT '发证日期',
    `expiry_date` DATE NOT NULL COMMENT '到期日期',
    `status` VARCHAR(30) DEFAULT 'Active' COMMENT '状态: Active/Expired/Revoked',
    `certified_by` VARCHAR(50) NOT NULL COMMENT '认证人',
    `certification_notes` TEXT COMMENT '认证说明',
    INDEX `idx_oq_operator` (`operator_id`),
    INDEX `idx_oq_status` (`status`),
    INDEX `idx_oq_expiry` (`expiry_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='操作员资质表';

-- ===========================
-- 7. 资质校验日志表
-- ===========================
CREATE TABLE IF NOT EXISTS `qualification_check_log` (
    `check_id` VARCHAR(50) PRIMARY KEY,
    `operator_id` VARCHAR(50) NOT NULL COMMENT '操作员ID',
    `process_step` VARCHAR(50) COMMENT '工序',
    `equipment_id` VARCHAR(50) COMMENT '设备ID',
    `work_order_id` VARCHAR(50) COMMENT '工单ID',
    `required_qualification` VARCHAR(50) NOT NULL COMMENT '需要的资质编码',
    `has_qualification` TINYINT(1) NOT NULL COMMENT '是否具备资质',
    `check_result` VARCHAR(30) NOT NULL COMMENT '结果: Pass/Fail/Expired',
    `check_time` DATETIME NOT NULL COMMENT '校验时间',
    `blocked` TINYINT(1) DEFAULT 0 COMMENT '是否被拦截',
    INDEX `idx_qcl_operator` (`operator_id`),
    INDEX `idx_qcl_time` (`check_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='资质校验日志表';

-- ===========================
-- 8. 焊线拉力测试记录表
-- ===========================
CREATE TABLE IF NOT EXISTS `bond_pull_test_record` (
    `test_id` VARCHAR(50) PRIMARY KEY,
    `first_article_id` VARCHAR(50) NOT NULL COMMENT '首件检验ID',
    `work_order_id` VARCHAR(50) NOT NULL COMMENT '工单ID',
    `sample_count` INT NOT NULL COMMENT '样本数量',
    `avg_pull_force` DECIMAL(8,2) COMMENT '平均拉力(gf)',
    `min_pull_force` DECIMAL(8,2) COMMENT '最小拉力(gf)',
    `max_pull_force` DECIMAL(8,2) COMMENT '最大拉力(gf)',
    `spec_min` DECIMAL(8,2) COMMENT '规格下限(gf)',
    `spec_max` DECIMAL(8,2) COMMENT '规格上限(gf)',
    `result` VARCHAR(30) DEFAULT 'Pass' COMMENT '结果: Pass/Fail',
    `tested_by` VARCHAR(50) NOT NULL COMMENT '测试人',
    `tested_at` DATETIME NOT NULL COMMENT '测试时间',
    `comments` TEXT COMMENT '备注',
    INDEX `idx_bptr_fa` (`first_article_id`),
    INDEX `idx_bptr_wo` (`work_order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='焊线拉力测试记录表';
