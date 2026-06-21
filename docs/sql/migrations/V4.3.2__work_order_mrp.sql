-- ============================================================
-- MES 优化改善计划 - 阶段二
-- 工单分解与物料需求计划(MRP)
-- ============================================================
-- 表名: work_order (modify), bom, bom_item, mrp_calculation, mrp_shortage_warning
-- 依赖: master_production_plan
-- ============================================================

-- 1. 修改 work_order 表增加新字段
ALTER TABLE `work_order` ADD COLUMN IF NOT EXISTS `material_batch_assignment` JSON COMMENT '分配的原材料批次';
ALTER TABLE `work_order` ADD COLUMN IF NOT EXISTS `bom_version` VARCHAR(50) COMMENT 'BOM版本';
ALTER TABLE `work_order` ADD COLUMN IF NOT EXISTS `parent_order_id` VARCHAR(50) COMMENT '父工单ID';
ALTER TABLE `work_order` ADD COLUMN IF NOT EXISTS `decompose_level` INT DEFAULT 0 COMMENT '分解层级';

-- 2. 物料清单(BOM)表
CREATE TABLE IF NOT EXISTS `bom` (
    `bom_id` VARCHAR(50) PRIMARY KEY COMMENT 'BOM编号',
    `product_id` VARCHAR(50) NOT NULL COMMENT '产品ID',
    `product_name` VARCHAR(200) NOT NULL COMMENT '产品名称',
    `bom_version` VARCHAR(20) NOT NULL DEFAULT '1.0' COMMENT 'BOM版本',
    `status` VARCHAR(30) DEFAULT 'Active' COMMENT '状态: Draft/Active/Obsolete',
    `effective_date` DATE COMMENT '生效日期',
    `expiry_date` DATE COMMENT '失效日期',
    `total_items` INT DEFAULT 0 COMMENT '物料项数',
    `created_by` VARCHAR(50) COMMENT '创建人',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_by` VARCHAR(50) COMMENT '更新人',
    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    INDEX idx_product_id (`product_id`),
    INDEX idx_status (`status`),
    UNIQUE KEY uk_product_version (`product_id`, `bom_version`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='物料清单表';

-- 3. BOM明细表
CREATE TABLE IF NOT EXISTS `bom_item` (
    `item_id` BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '明细ID',
    `bom_id` VARCHAR(50) NOT NULL COMMENT 'BOM编号',
    `material_id` VARCHAR(50) NOT NULL COMMENT '物料ID',
    `material_name` VARCHAR(200) NOT NULL COMMENT '物料名称',
    `material_spec` VARCHAR(500) COMMENT '物料规格',
    `quantity_per_unit` DECIMAL(12,4) NOT NULL COMMENT '单位用量',
    `unit` VARCHAR(20) COMMENT '单位',
    `loss_rate` DECIMAL(5,2) DEFAULT 0 COMMENT '损耗率(%)',
    `substitute_materials` JSON COMMENT '替代物料列表',
    `sort_order` INT DEFAULT 0 COMMENT '排序',
    `remark` TEXT COMMENT '备注',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    INDEX idx_bom_id (`bom_id`),
    INDEX idx_material_id (`material_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='BOM明细表';

-- 4. MRP计算记录表
CREATE TABLE IF NOT EXISTS `mrp_calculation` (
    `calculation_id` VARCHAR(50) PRIMARY KEY COMMENT '计算编号',
    `plan_id` VARCHAR(50) COMMENT '关联计划ID',
    `work_order_id` VARCHAR(50) COMMENT '关联工单ID',
    `calculation_type` VARCHAR(50) DEFAULT 'Full' COMMENT '计算类型: Full/Incremental/Single',
    `status` VARCHAR(30) DEFAULT 'Completed' COMMENT '状态: Running/Completed/Error',
    `total_demand_items` INT COMMENT '总需求物料项数',
    `shortage_items` INT COMMENT '缺料项数',
    `sufficient_items` INT COMMENT '充足项数',
    `calculation_params` JSON COMMENT '计算参数',
    `result_summary` TEXT COMMENT '结果摘要',
    `result_data` JSON COMMENT '计算结果详细数据',
    `created_by` VARCHAR(50) COMMENT '创建人',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    INDEX idx_plan_id (`plan_id`),
    INDEX idx_work_order_id (`work_order_id`),
    INDEX idx_status (`status`),
    INDEX idx_created_at (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='MRP计算记录表';

-- 5. 缺料预警表
CREATE TABLE IF NOT EXISTS `mrp_shortage_warning` (
    `warning_id` BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '预警ID',
    `calculation_id` VARCHAR(50) NOT NULL COMMENT '计算编号',
    `material_id` VARCHAR(50) NOT NULL COMMENT '物料ID',
    `material_name` VARCHAR(200) NOT NULL COMMENT '物料名称',
    `required_qty` DECIMAL(15,4) NOT NULL COMMENT '需求数量',
    `available_qty` DECIMAL(15,4) DEFAULT 0 COMMENT '可用数量',
    `shortage_qty` DECIMAL(15,4) DEFAULT 0 COMMENT '缺口数量',
    `expected_arrival` DATE COMMENT '预计到货日期',
    `purchase_order_no` VARCHAR(50) COMMENT '采购单号',
    `severity` VARCHAR(20) DEFAULT 'Medium' COMMENT '严重程度: Low/Medium/High/Critical',
    `status` VARCHAR(30) DEFAULT 'Open' COMMENT '状态: Open/Resolved/Ignore',
    `resolution_note` TEXT COMMENT '处理说明',
    `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    INDEX idx_calculation_id (`calculation_id`),
    INDEX idx_material_id (`material_id`),
    INDEX idx_severity (`severity`),
    INDEX idx_status (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='缺料预警表';
