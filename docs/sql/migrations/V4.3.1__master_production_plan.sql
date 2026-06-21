-- ============================================================
-- MES 优化改善计划 - 阶段二
-- 主生产计划与产能评估
-- ============================================================
-- 表名: master_production_plan, capacity_load, capacity_simulation
-- 依赖: sales_order
-- ============================================================

-- 1. 主生产计划表
CREATE TABLE IF NOT EXISTS `master_production_plan` (
    `plan_id` VARCHAR(50) PRIMARY KEY COMMENT '计划编号',
    `plan_name` VARCHAR(200) NOT NULL COMMENT '计划名称',
    `plan_type` VARCHAR(50) DEFAULT 'MPS' COMMENT '计划类型: MPS/MRP',
    `plan_period_start` DATE NOT NULL COMMENT '计划起始日期',
    `plan_period_end` DATE NOT NULL COMMENT '计划结束日期',
    `status` VARCHAR(30) DEFAULT 'Draft' COMMENT '状态: Draft/Published/Executing/Completed/Cancelled',
    `total_demand_qty` INT COMMENT '总需求数量',
    `total_capacity` INT COMMENT '总产能',
    `capacity_utilization` DECIMAL(5,2) COMMENT '产能利用率(%)',
    `bottleneck_identified` TINYINT(1) DEFAULT 0 COMMENT '是否识别到瓶颈',
    `bottleneck_description` TEXT COMMENT '瓶颈描述',
    `planner` VARCHAR(100) COMMENT '计划员',
    `plan_data` JSON COMMENT '计划详细数据',
    `remark` TEXT COMMENT '备注',
    `created_by` VARCHAR(50) COMMENT '创建人',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_by` VARCHAR(50) COMMENT '更新人',
    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `published_by` VARCHAR(50) COMMENT '发布人',
    `published_at` DATETIME COMMENT '发布时间',
    INDEX idx_plan_period (`plan_period_start`, `plan_period_end`),
    INDEX idx_status (`status`),
    INDEX idx_created_at (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='主生产计划表';

-- 2. 产能负荷表
CREATE TABLE IF NOT EXISTS `capacity_load` (
    `load_id` BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '负荷ID',
    `plan_id` VARCHAR(50) NOT NULL COMMENT '计划ID',
    `process_code` VARCHAR(50) NOT NULL COMMENT '工序代码',
    `process_name` VARCHAR(100) NOT NULL COMMENT '工序名称',
    `equipment_group` VARCHAR(100) COMMENT '设备组',
    `uph` DECIMAL(10,2) COMMENT '每小时产出(UPH)',
    `available_hours` DECIMAL(10,2) COMMENT '可用工时',
    `required_hours` DECIMAL(10,2) COMMENT '需求工时',
    `load_rate` DECIMAL(5,2) COMMENT '负荷率(%)',
    `is_bottleneck` TINYINT(1) DEFAULT 0 COMMENT '是否瓶颈工序',
    `available_qty` INT COMMENT '可用产能数量',
    `required_qty` INT COMMENT '需求数量',
    `shortage_qty` INT COMMENT '缺口数量',
    `shift_plan` VARCHAR(50) COMMENT '班次计划',
    `calculated_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '计算时间',
    INDEX idx_plan_id (`plan_id`),
    INDEX idx_process_code (`process_code`),
    INDEX idx_is_bottleneck (`is_bottleneck`),
    INDEX idx_load_rate (`load_rate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='产能负荷表';

-- 3. 产能模拟记录表
CREATE TABLE IF NOT EXISTS `capacity_simulation` (
    `simulation_id` VARCHAR(50) PRIMARY KEY COMMENT '模拟编号',
    `simulation_name` VARCHAR(200) NOT NULL COMMENT '模拟名称',
    `base_plan_id` VARCHAR(50) COMMENT '基准计划ID',
    `scenario_description` TEXT COMMENT '场景描述',
    `scenario_params` JSON COMMENT '模拟参数',
    `total_demand_qty` INT COMMENT '总需求',
    `total_capacity` INT COMMENT '总产能',
    `capacity_utilization` DECIMAL(5,2) COMMENT '产能利用率',
    `bottleneck_count` INT COMMENT '瓶颈工序数',
    `result_summary` TEXT COMMENT '结果摘要',
    `result_data` JSON COMMENT '模拟结果详细数据',
    `status` VARCHAR(30) DEFAULT 'Completed' COMMENT '状态: Running/Completed/Error',
    `created_by` VARCHAR(50) COMMENT '创建人',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    INDEX idx_base_plan_id (`base_plan_id`),
    INDEX idx_status (`status`),
    INDEX idx_created_at (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='产能模拟记录表';
