-- ============================================================
-- MES 优化改善计划 - 阶段二
-- 订单进度查询与交付达成率
-- ============================================================
-- 表名: order_progress_snapshot, otd_statistics, delay_reason_record
-- 依赖: sales_order, work_order
-- ============================================================

-- 1. 订单进度快照表
CREATE TABLE IF NOT EXISTS `order_progress_snapshot` (
    `snapshot_id` BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '快照ID',
    `order_id` VARCHAR(50) NOT NULL COMMENT '订单ID',
    `snapshot_time` DATETIME NOT NULL COMMENT '快照时间',
    `total_quantity` INT NOT NULL COMMENT '订单总数量',
    `completed_quantity` INT DEFAULT 0 COMMENT '已完成数量',
    `in_progress_quantity` INT DEFAULT 0 COMMENT '在制数量',
    `pending_quantity` INT DEFAULT 0 COMMENT '待产数量',
    `defective_quantity` INT DEFAULT 0 COMMENT '不良品数量',
    `yield_rate` DECIMAL(5,2) COMMENT '良率(%)',
    `progress_percentage` DECIMAL(5,2) COMMENT '进度百分比',
    `current_stage` VARCHAR(100) COMMENT '当前工序',
    `estimated_completion` DATE COMMENT '预计完成日期',
    `is_delayed` TINYINT(1) DEFAULT 0 COMMENT '是否延期',
    `delay_days` INT DEFAULT 0 COMMENT '延期天数',
    `work_order_count` INT DEFAULT 0 COMMENT '关联工单数',
    `completed_work_order_count` INT DEFAULT 0 COMMENT '已完成工单数',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    INDEX idx_order_id (`order_id`),
    INDEX idx_snapshot_time (`snapshot_time`),
    INDEX idx_is_delayed (`is_delayed`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='订单进度快照表';

-- 2. OTD统计表
CREATE TABLE IF NOT EXISTS `otd_statistics` (
    `stat_id` BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '统计ID',
    `stat_period` VARCHAR(20) NOT NULL COMMENT '统计周期: YYYY-MM',
    `total_orders` INT DEFAULT 0 COMMENT '总订单数',
    `on_time_orders` INT DEFAULT 0 COMMENT '按时交付订单数',
    `late_orders` INT DEFAULT 0 COMMENT '延迟订单数',
    `otd_rate` DECIMAL(5,2) COMMENT 'OTD达成率(%)',
    `avg_delay_days` DECIMAL(5,2) COMMENT '平均延迟天数',
    `max_delay_days` INT COMMENT '最大延迟天数',
    `total_quantity` INT DEFAULT 0 COMMENT '总交付数量',
    `on_time_quantity` INT DEFAULT 0 COMMENT '按时交付数量',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    UNIQUE KEY uk_stat_period (`stat_period`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='OTD统计表';

-- 3. 延迟原因记录表
CREATE TABLE IF NOT EXISTS `delay_reason_record` (
    `record_id` BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '记录ID',
    `order_id` VARCHAR(50) NOT NULL COMMENT '订单ID',
    `work_order_id` VARCHAR(50) COMMENT '工单ID',
    `delay_reason_category` VARCHAR(50) NOT NULL COMMENT '延迟原因类别: Material/Equipment/Quality/Capacity/Planning/Other',
    `delay_reason_detail` TEXT COMMENT '延迟原因详细说明',
    `delay_days` INT NOT NULL COMMENT '延迟天数',
    `impact_quantity` INT COMMENT '影响数量',
    `responsible_dept` VARCHAR(100) COMMENT '责任部门',
    `corrective_action` TEXT COMMENT '纠正措施',
    `preventive_action` TEXT COMMENT '预防措施',
    `reported_by` VARCHAR(50) COMMENT '上报人',
    `reported_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '上报时间',
    INDEX idx_order_id (`order_id`),
    INDEX idx_delay_reason_category (`delay_reason_category`),
    INDEX idx_reported_at (`reported_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='延迟原因记录表';
