-- ============================================================
-- MES 优化改善计划 - 阶段二
-- 订单评审流程
-- ============================================================
-- 表名: sales_order, order_review, order_review_item, order_version
-- 依赖: 无
-- ============================================================

-- 1. 销售订单表
CREATE TABLE IF NOT EXISTS `sales_order` (
    `order_id` VARCHAR(50) PRIMARY KEY COMMENT '订单编号',
    `customer_id` VARCHAR(50) NOT NULL COMMENT '客户ID',
    `customer_name` VARCHAR(200) NOT NULL COMMENT '客户名称',
    `product_id` VARCHAR(50) NOT NULL COMMENT '产品ID',
    `product_name` VARCHAR(200) NOT NULL COMMENT '产品名称',
    `product_spec` VARCHAR(500) COMMENT '产品规格',
    `quantity` INT NOT NULL COMMENT '订单数量',
    `unit_price` DECIMAL(12,4) COMMENT '单价',
    `total_amount` DECIMAL(15,2) COMMENT '总金额',
    `currency` VARCHAR(10) DEFAULT 'CNY' COMMENT '币种',
    `order_date` DATE NOT NULL COMMENT '下单日期',
    `delivery_date` DATE NOT NULL COMMENT '交期',
    `priority` VARCHAR(20) DEFAULT 'Normal' COMMENT '优先级: Normal/Urgent/Rush',
    `status` VARCHAR(30) DEFAULT 'Draft' COMMENT '状态: Draft/PendingReview/Reviewing/Approved/Rejected/Cancelled/InProduction/Completed/Shipped',
    `review_status` VARCHAR(30) DEFAULT 'NotStarted' COMMENT '评审状态: NotStarted/InProgress/Passed/Rejected/ConditionalPassed',
    `review_result` TEXT COMMENT '评审结果说明',
    `package_type` VARCHAR(100) COMMENT '封装类型',
    `lead_frame_type` VARCHAR(100) COMMENT '引线框架类型',
    `wire_type` VARCHAR(50) COMMENT '线材类型: Gold/Copper/Aluminum',
    `special_requirements` TEXT COMMENT '特殊要求',
    `quality_level` VARCHAR(50) DEFAULT 'Commercial' COMMENT '质量等级: Commercial/Automotive/Military',
    `remark` TEXT COMMENT '备注',
    `created_by` VARCHAR(50) COMMENT '创建人',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_by` VARCHAR(50) COMMENT '更新人',
    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `deleted` TINYINT(1) DEFAULT 0 COMMENT '是否删除',
    INDEX idx_customer_id (`customer_id`),
    INDEX idx_status (`status`),
    INDEX idx_review_status (`review_status`),
    INDEX idx_delivery_date (`delivery_date`),
    INDEX idx_priority (`priority`),
    INDEX idx_created_at (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='销售订单表';

-- 2. 订单评审表
CREATE TABLE IF NOT EXISTS `order_review` (
    `review_id` VARCHAR(50) PRIMARY KEY COMMENT '评审编号',
    `order_id` VARCHAR(50) NOT NULL COMMENT '订单ID',
    `review_type` VARCHAR(50) DEFAULT 'Standard' COMMENT '评审类型: Standard/Express/Special',
    `status` VARCHAR(30) DEFAULT 'Pending' COMMENT '评审状态: Pending/InProgress/Passed/Rejected/ConditionalPassed',
    `start_time` DATETIME COMMENT '开始时间',
    `end_time` DATETIME COMMENT '结束时间',
    `deadline` DATETIME COMMENT '评审截止时间',
    `initiated_by` VARCHAR(50) COMMENT '发起人',
    `conclusion` TEXT COMMENT '评审结论',
    `conditions` TEXT COMMENT '有条件通过的条件说明',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    INDEX idx_order_id (`order_id`),
    INDEX idx_status (`status`),
    INDEX idx_deadline (`deadline`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='订单评审表';

-- 3. 评审明细表（各角色投票记录）
CREATE TABLE IF NOT EXISTS `order_review_item` (
    `item_id` BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '明细ID',
    `review_id` VARCHAR(50) NOT NULL COMMENT '评审ID',
    `reviewer_role` VARCHAR(50) NOT NULL COMMENT '评审角色: Sales/Engineering/Quality/Production/Purchasing/Finance',
    `reviewer_name` VARCHAR(100) COMMENT '评审人姓名',
    `vote` VARCHAR(20) COMMENT '投票: Approve/Reject/Conditional',
    `comments` TEXT COMMENT '评审意见',
    `conditions` TEXT COMMENT '附加条件',
    `review_time` DATETIME COMMENT '评审时间',
    `status` VARCHAR(20) DEFAULT 'Pending' COMMENT '状态: Pending/Reviewed/Timeout',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    INDEX idx_review_id (`review_id`),
    INDEX idx_reviewer_role (`reviewer_role`),
    INDEX idx_vote (`vote`),
    INDEX idx_status (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='评审明细表';

-- 4. 订单版本表
CREATE TABLE IF NOT EXISTS `order_version` (
    `version_id` BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '版本ID',
    `order_id` VARCHAR(50) NOT NULL COMMENT '订单ID',
    `version_no` INT NOT NULL COMMENT '版本号',
    `change_type` VARCHAR(50) COMMENT '变更类型: Quantity/Date/Spec/Package/All',
    `change_reason` TEXT COMMENT '变更原因',
    `change_description` TEXT COMMENT '变更说明',
    `old_data` JSON COMMENT '变更前数据',
    `new_data` JSON COMMENT '变更后数据',
    `changed_by` VARCHAR(50) COMMENT '变更人',
    `changed_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '变更时间',
    `approved_by` VARCHAR(50) COMMENT '审批人',
    INDEX idx_order_id (`order_id`),
    INDEX idx_version_no (`order_id`, `version_no`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='订单版本表';
