-- ============================================================
-- MES 优化改善计划 - 阶段二
-- 插单处理
-- ============================================================
-- 表名: rush_order_request, rush_order_impact
-- 依赖: sales_order, master_production_plan
-- ============================================================

-- 1. 插单申请表
CREATE TABLE IF NOT EXISTS `rush_order_request` (
    `request_id` VARCHAR(50) PRIMARY KEY COMMENT '申请编号',
    `order_id` VARCHAR(50) NOT NULL COMMENT '关联订单ID',
    `customer_id` VARCHAR(50) COMMENT '客户ID',
    `customer_name` VARCHAR(200) COMMENT '客户名称',
    `product_id` VARCHAR(50) NOT NULL COMMENT '产品ID',
    `product_name` VARCHAR(200) NOT NULL COMMENT '产品名称',
    `rush_quantity` INT NOT NULL COMMENT '插单数量',
    `required_date` DATE NOT NULL COMMENT '要求交期',
    `rush_reason` TEXT COMMENT '插单原因',
    `priority_level` VARCHAR(20) DEFAULT 'Urgent' COMMENT '优先级: Urgent/Critical',
    `status` VARCHAR(30) DEFAULT 'Pending' COMMENT '状态: Pending/Analyzing/Approved/Rejected/Executing/Completed/Cancelled',
    `impact_analysis_done` TINYINT(1) DEFAULT 0 COMMENT '是否已完成影响分析',
    `analysis_summary` TEXT COMMENT '影响分析摘要',
    `approval_result` VARCHAR(20) COMMENT '审批结果: Approved/Rejected',
    `approval_by` VARCHAR(50) COMMENT '审批人',
    `approval_at` DATETIME COMMENT '审批时间',
    `approval_comments` TEXT COMMENT '审批意见',
    `executed_by` VARCHAR(50) COMMENT '执行人',
    `executed_at` DATETIME COMMENT '执行时间',
    `created_by` VARCHAR(50) COMMENT '创建人',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    INDEX idx_order_id (`order_id`),
    INDEX idx_status (`status`),
    INDEX idx_required_date (`required_date`),
    INDEX idx_created_at (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='插单申请表';

-- 2. 插单影响分析表
CREATE TABLE IF NOT EXISTS `rush_order_impact` (
    `impact_id` BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '影响分析ID',
    `request_id` VARCHAR(50) NOT NULL COMMENT '插单申请ID',
    `affected_order_id` VARCHAR(50) NOT NULL COMMENT '受影响订单ID',
    `affected_work_order_id` VARCHAR(50) COMMENT '受影响工单ID',
    `impact_type` VARCHAR(50) COMMENT '影响类型: Delay/MaterialShortage/Capacity/Resource',
    `impact_description` TEXT COMMENT '影响说明',
    `original_delivery_date` DATE COMMENT '原交期',
    `new_estimated_delivery` DATE COMMENT '新预计交期',
    `delay_days` INT COMMENT '延迟天数',
    `material_shortage_items` JSON COMMENT '缺料项列表',
    `capacity_conflict_details` JSON COMMENT '产能冲突详情',
    `severity` VARCHAR(20) DEFAULT 'Medium' COMMENT '严重程度: Low/Medium/High/Critical',
    `mitigation_plan` TEXT COMMENT '缓解方案',
    `analyzed_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '分析时间',
    INDEX idx_request_id (`request_id`),
    INDEX idx_affected_order_id (`affected_order_id`),
    INDEX idx_severity (`severity`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='插单影响分析表';
