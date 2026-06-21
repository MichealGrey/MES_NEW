-- ============================================================
-- V4.0.10: 基础数据种子 (Seed Data)
-- 描述: 插入库位、检验标准、缺陷代码、异常类型等基础数据
-- 依赖: V4.0.0 ~ V4.0.9
-- 作者: MES Team
-- 日期: 2026-06-07
-- ============================================================

-- ============================================================
-- 1. 库位数据 (warehouse_location)
-- ============================================================
INSERT INTO `warehouse_location` (`location_id`, `location_name`, `warehouse_zone`, `location_type`, `temperature_controlled`, `humidity_controlled`, `max_capacity`, `is_active`)
VALUES
('WH-A-01', '原材料仓库 A01', 'Warehouse', 'Rack', 0, 0, 10000, 1),
('WH-A-02', '原材料仓库 A02', 'Warehouse', 'Rack', 0, 0, 10000, 1),
('WH-A-03', '原材料仓库 A03', 'Warehouse', 'Shelf', 0, 0, 5000, 1),
('WH-B-01', '成品仓库 B01', 'Warehouse', 'Rack', 0, 0, 20000, 1),
('WH-B-02', '成品仓库 B02', 'Warehouse', 'Rack', 0, 0, 20000, 1),
('WH-C-01', '隔离区 C01', 'Quarantine', 'Floor', 0, 0, 5000, 1),
('WH-C-02', '隔离区 C02', 'Quarantine', 'Floor', 0, 0, 5000, 1),
('WH-D-01', '线边仓 D01', 'LineSide', 'Bin', 0, 0, 2000, 1),
('WH-D-02', '线边仓 D02', 'LineSide', 'Bin', 0, 0, 2000, 1),
('WH-E-01', 'MSL 管控仓 E01', 'Warehouse', 'Rack', 1, 1, 5000, 1),
('WH-E-02', 'MSL 管控仓 E02', 'Warehouse', 'Rack', 1, 1, 5000, 1),
('WH-F-01', '冷藏仓 F01', 'Warehouse', 'Shelf', 1, 0, 1000, 1),
('WH-G-01', '退货暂存区 G01', 'Quarantine', 'Floor', 0, 0, 3000, 1),
('WH-H-01', '成品待发区 H01', 'Warehouse', 'Floor', 0, 0, 15000, 1),
('WH-H-02', '成品待发区 H02', 'Warehouse', 'Floor', 0, 0, 15000, 1),
('WH-I-01', '包材仓 I01', 'Warehouse', 'Rack', 0, 0, 8000, 1),
('WH-I-02', '包材仓 I02', 'Warehouse', 'Rack', 0, 0, 8000, 1),
('WH-J-01', '辅料仓 J01', 'Warehouse', 'Shelf', 0, 0, 5000, 1),
('WH-K-01', '化学品仓 K01', 'Warehouse', 'Rack', 1, 1, 2000, 1),
('WH-K-02', '化学品仓 K02', 'Warehouse', 'Rack', 1, 1, 2000, 1)
ON DUPLICATE KEY UPDATE `location_name` = VALUES(`location_name`);

-- ============================================================
-- 2. 检验标准数据 (iqc_inspection_standard)
-- ============================================================
-- 晶圆类检验标准
INSERT INTO `iqc_inspection_standard` (`standard_id`, `material_id`, `inspection_item_code`, `inspection_item_name`, `item_type`, `standard_value`, `lower_limit`, `upper_limit`, `unit`, `sampling_plan`, `is_mandatory`, `is_active`)
VALUES
('STD-WAFER-001', 'MAT-WAFER-8IN', 'WAFER-DIA', '晶圆直径', 'Dimensional', '200.00', '199.90', '200.10', 'mm', 'AQL-0.65', 1, 1),
('STD-WAFER-002', 'MAT-WAFER-8IN', 'WAFER-THK', '晶圆厚度', 'Dimensional', '0.725', '0.715', '0.735', 'mm', 'AQL-0.65', 1, 1),
('STD-WAFER-003', 'MAT-WAFER-8IN', 'WAFER-TTV', '厚度偏差', 'Dimensional', '5.00', NULL, '10.00', 'μm', 'AQL-0.65', 1, 1),
('STD-WAFER-004', 'MAT-WAFER-8IN', 'WAFER-VIS', '表面外观', 'Visual', '无划痕/无污染', NULL, NULL, NULL, 'AQL-0.40', 1, 1),
('STD-WAFER-005', 'MAT-WAFER-8IN', 'WAFER-FLT', '翘曲度', 'Dimensional', NULL, NULL, '50.00', 'μm', 'AQL-0.65', 1, 1),

('STD-WAFER-006', 'MAT-WAFER-12IN', 'WAFER-DIA', '晶圆直径', 'Dimensional', '300.00', '299.90', '300.10', 'mm', 'AQL-0.65', 1, 1),
('STD-WAFER-007', 'MAT-WAFER-12IN', 'WAFER-THK', '晶圆厚度', 'Dimensional', '0.775', '0.765', '0.785', 'mm', 'AQL-0.65', 1, 1),
('STD-WAFER-008', 'MAT-WAFER-12IN', 'WAFER-VIS', '表面外观', 'Visual', '无划痕/无污染', NULL, NULL, NULL, 'AQL-0.40', 1, 1),

-- 引线框架检验标准
('STD-LF-001', 'MAT-LEADFRAME-Cu', 'LF-DIM', '框架尺寸', 'Dimensional', NULL, '-0.05', '+0.05', 'mm', 'AQL-0.65', 1, 1),
('STD-LF-002', 'MAT-LEADFRAME-Cu', 'LF-THK', '框架厚度', 'Dimensional', '0.254', '0.249', '0.259', 'mm', 'AQL-0.65', 1, 1),
('STD-LF-003', 'MAT-LEADFRAME-Cu', 'LF-VIS', '外观检查', 'Visual', '无氧化/无变形', NULL, NULL, NULL, 'AQL-0.65', 1, 1),
('STD-LF-004', 'MAT-LEADFRAME-Cu', 'LF-COP', '共面度', 'Dimensional', NULL, NULL, '0.05', 'mm', 'AQL-0.40', 1, 1),

-- 塑封料检验标准
('STD-EMC-001', 'MAT-EMC-SUMICON', 'EMC-GEL', '凝胶时间', 'Functional', '120', '90', '150', 'sec', 'AQL-0.65', 1, 1),
('STD-EMC-002', 'MAT-EMC-SUMICON', 'EMC-VIS', '粘度', 'Functional', '150', '120', '180', 'Pa·s', 'AQL-0.65', 1, 1),
('STD-EMC-003', 'MAT-EMC-SUMICON', 'EMC-SHELF', '有效期检查', 'Visual', '>=30天', NULL, NULL, NULL, 'AQL-1.0', 1, 1),

-- 金丝检验标准
('STD-AU-001', 'MAT-WIRE-AU-0.8MIL', 'AW-DIA', '线径', 'Dimensional', '0.020', '0.019', '0.021', 'mm', 'AQL-0.40', 1, 1),
('STD-AU-002', 'MAT-WIRE-AU-0.8MIL', 'AW-TEN', '拉力强度', 'Functional', '150', '120', NULL, 'gf', 'AQL-0.40', 1, 1),
('STD-AU-003', 'MAT-WIRE-AU-0.8MIL', 'AW-ELN', '延伸率', 'Functional', '4.0', '3.0', '6.0', '%', 'AQL-0.65', 1, 1),

-- 载带检验标准
('STD-TAPE-001', 'MAT-TAPE-8MM', 'TAPE-WID', '宽度', 'Dimensional', '8.00', '7.95', '8.05', 'mm', 'AQL-0.65', 1, 1),
('STD-TAPE-002', 'MAT-TAPE-8MM', 'TAPE-PIT', '节距', 'Dimensional', '4.00', '3.98', '4.02', 'mm', 'AQL-0.65', 1, 1),
('STD-TAPE-003', 'MAT-TAPE-8MM', 'TAPE-POC', '口袋深度', 'Dimensional', '1.50', '1.40', '1.60', 'mm', 'AQL-0.65', 1, 1)
ON DUPLICATE KEY UPDATE `inspection_item_name` = VALUES(`inspection_item_name`);

-- ============================================================
-- 3. 缺陷代码数据 (INSERT INTO 需要创建的缺陷代码表或临时使用)
-- ============================================================
-- 注: 以下为缺陷代码参考数据，实际可存入 sys_defect_code 表
-- 创建缺陷代码配置表
CREATE TABLE IF NOT EXISTS `sys_defect_code` (
    `defect_code` VARCHAR(50) NOT NULL COMMENT '缺陷代码',
    `defect_name` VARCHAR(100) NOT NULL COMMENT '缺陷名称',
    `defect_category` VARCHAR(50) NOT NULL COMMENT '缺陷分类: Dimensional/Visual/Electrical/Functional/Contamination/Material',
    `severity` VARCHAR(20) NOT NULL DEFAULT 'Major' COMMENT 'Critical/Major/Minor',
    `description` TEXT DEFAULT NULL COMMENT '缺陷描述',
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`defect_code`),
    INDEX `idx_category` (`defect_category`),
    INDEX `idx_severity` (`severity`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `sys_defect_code` (`defect_code`, `defect_name`, `defect_category`, `severity`, `description`, `is_active`)
VALUES
-- 外观类缺陷
('DFT-VIS-001', '表面划痕', 'Visual', 'Minor', '产品表面存在可见划痕', 1),
('DFT-VIS-002', '表面污染', 'Visual', 'Major', '产品表面有异物/油污/指纹等污染', 1),
('DFT-VIS-003', '标签错误', 'Visual', 'Major', '标签内容/位置/条码不正确', 1),
('DFT-VIS-004', '包装破损', 'Visual', 'Major', '产品包装存在破损/变形', 1),
('DFT-VIS-005', '氧化变色', 'Visual', 'Critical', '引脚/焊盘出现氧化变色', 1),
('DFT-VIS-006', '毛刺/飞边', 'Visual', 'Minor', '塑封体边缘存在毛刺或飞边', 1),
('DFT-VIS-007', '裂纹/碎裂', 'Visual', 'Critical', '产品表面或内部存在裂纹', 1),

-- 尺寸类缺陷
('DFT-DIM-001', '尺寸超差', 'Dimensional', 'Major', '产品关键尺寸超出规格范围', 1),
('DFT-DIM-002', '共面度不良', 'Dimensional', 'Major', '引脚共面度超出规格', 1),
('DFT-DIM-003', '厚度超差', 'Dimensional', 'Major', '产品厚度超出规格范围', 1),
('DFT-DIM-004', '宽度超差', 'Dimensional', 'Major', '产品宽度超出规格范围', 1),

-- 电性类缺陷
('DFT-ELEC-001', '开路', 'Electrical', 'Critical', '电路开路/断路', 1),
('DFT-ELEC-002', '短路', 'Electrical', 'Critical', '电路短路', 1),
('DFT-ELEC-003', '漏电流异常', 'Electrical', 'Major', '器件漏电流超出规格', 1),
('DFT-ELEC-004', '功能不良', 'Electrical', 'Critical', '器件功能测试不通过', 1),
('DFT-ELEC-005', '参数漂移', 'Electrical', 'Major', '关键参数偏离规格中心', 1),

-- 功能类缺陷
('DFT-FUNC-001', '键合拉力不足', 'Functional', 'Critical', '焊线拉力低于最低标准', 1),
('DFT-FUNC-002', '键合位置偏移', 'Functional', 'Major', '焊点位置偏离目标区域', 1),
('DFT-FUNC-003', '塑封不完整', 'Functional', 'Major', '塑封料未完全填充型腔', 1),
('DFT-FUNC-004', '切割偏移', 'Functional', 'Major', '切割位置偏离标记', 1),
('DFT-FUNC-005', '打印不良', 'Functional', 'Minor', '标识打印不清晰/不完整', 1),

-- 污染类缺陷
('DFT-CONT-001', '颗粒污染', 'Contamination', 'Major', '产品内部存在颗粒污染物', 1),
('DFT-CONT-002', '有机物污染', 'Contamination', 'Major', '产品表面存在有机残留物', 1),
('DFT-CONT-003', '金属离子污染', 'Contamination', 'Critical', '检测到金属离子超标', 1),

-- 材料类缺陷
('DFT-MAT-001', '材料过期', 'Material', 'Critical', '使用的材料超过有效期', 1),
('DFT-MAT-002', '材料错用', 'Material', 'Critical', '使用了错误的材料型号', 1),
('DFT-MAT-003', '材料批次异常', 'Material', 'Major', '材料来料批次信息不匹配', 1)
ON DUPLICATE KEY UPDATE `defect_name` = VALUES(`defect_name`);

-- ============================================================
-- 4. 异常类型数据 (sys_abnormal_type)
-- ============================================================
CREATE TABLE IF NOT EXISTS `sys_abnormal_type` (
    `type_code` VARCHAR(50) NOT NULL COMMENT '异常类型代码',
    `type_name` VARCHAR(100) NOT NULL COMMENT '异常类型名称',
    `default_severity` VARCHAR(20) NOT NULL DEFAULT 'Major' COMMENT '默认严重程度',
    `description` TEXT DEFAULT NULL,
    `response_time_minutes` INT DEFAULT 30 COMMENT '要求响应时间(分钟)',
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`type_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `sys_abnormal_type` (`type_code`, `type_name`, `default_severity`, `description`, `response_time_minutes`, `is_active`)
VALUES
('ABN-EQP', '设备异常', 'Major', '设备故障、设备参数异常、设备报警等', 15, 1),
('ABN-QTY', '质量异常', 'Critical', '产品质量异常、检验不合格、客户投诉等', 15, 1),
('ABN-MAT', '物料异常', 'Major', '物料短缺、物料不合格、物料错用等', 30, 1),
('ABN-PRC', '工艺异常', 'Major', '工艺参数偏差、工艺文件错误、工艺变更未审批等', 30, 1),
('ABN-SAF', '安全异常', 'Critical', '安全隐患、环境异常、化学品泄漏等', 5, 1),
('ABN-ENV', '环境异常', 'Major', '温湿度超标、洁净度超标、静电异常等', 30, 1),
('ABN-SYS', '系统异常', 'Minor', 'MES系统故障、数据异常、接口异常等', 60, 1),
('ABN-PRD', '生产异常', 'Major', '工单异常、计划异常、产能异常等', 30, 1),
('ABN-DOC', '文档异常', 'Minor', 'SOP/图纸/BOM等文件错误或过期', 60, 1),
('ABN-OTH', '其他异常', 'Minor', '其他未分类的异常情况', 30, 1)
ON DUPLICATE KEY UPDATE `type_name` = VALUES(`type_name`);

-- ============================================================
-- 5. PM 保养模板数据 (equipment_pm_template)
-- ============================================================
CREATE TABLE IF NOT EXISTS `equipment_pm_template` (
    `template_id` VARCHAR(50) NOT NULL,
    `equipment_type` VARCHAR(50) NOT NULL COMMENT '设备类型',
    `pm_type` VARCHAR(50) NOT NULL COMMENT 'Daily/Weekly/Monthly/Quarterly/Annual',
    `template_name` VARCHAR(100) NOT NULL,
    `check_items` JSON DEFAULT NULL COMMENT '检查项目JSON',
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`template_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `equipment_pm_template` (`template_id`, `equipment_type`, `pm_type`, `template_name`, `check_items`, `is_active`)
VALUES
('PM-TEMP-001', 'Wire Bonder', 'Daily', '键合机日常保养', '[
    {"code": "DB-CLEAN", "name": "劈刀清洁", "standard": "无残留"},
    {"code": "DB-ALIGN", "name": "劈刀对准检查", "standard": "偏差<0.01mm"},
    {"code": "DB-PARM", "name": "参数检查", "standard": "参数正常"},
    {"code": "DB-VIS", "name": "设备外观检查", "standard": "无异常"}
]', 1),
('PM-TEMP-002', 'Wire Bonder', 'Monthly', '键合机月度保养', '[
    {"code": "MB-CAL", "name": "校准", "standard": "精度达标"},
    {"code": "MB-REPL", "name": "易损件更换", "standard": "按周期更换"},
    {"code": "MB-DEEP", "name": "深度清洁", "standard": "无污染物"}
]', 1),
('PM-TEMP-003', 'Mold Press', 'Daily', '塑封机日常保养', '[
    {"code": "MP-CLEAN", "name": "模具清洁", "standard": "无塑封料残留"},
    {"code": "MP-TEMP", "name": "温度检查", "standard": "±5°C"},
    {"code": "MP-PRES", "name": "压力检查", "standard": "±2%"},
    {"code": "MP-VIS", "name": "设备外观", "standard": "无异常"}
]', 1),
('PM-TEMP-004', 'Die Bonder', 'Daily', '固晶机日常保养', '[
    {"code": "DB-CLEAN", "name": "吸嘴清洁", "standard": "无残留"},
    {"code": "DB-ALIGN", "name": "对位检查", "standard": "偏差<0.005mm"},
    {"code": "DB-ADH", "name": "胶量检查", "standard": "在规格内"}
]', 1),
('PM-TEMP-005', 'Test Handler', 'Daily', '测试分选机日常保养', '[
    {"code": "TH-CLEAN", "name": '测试座清洁', 'standard': '无残留'},
    {"code": "TH-PICK", "name": "取放检查", "standard": "正常"},
    {"code": "TH-SORT", "name": "分选检查", "standard": "Bin分类正确"}
]', 1),
('PM-TEMP-006', 'Saw Machine', 'Weekly', '切割机周保养', '[
    {"code": "SW-BLADE", "name": "刀片检查", "standard": "磨损<10%"},
    {"code": "SW-ALIGN", "name": "对位校准", "standard": "偏差<0.005mm"},
    {"code": "SW-WATER", "name": '冷却水检查', 'standard': '流量正常'},
    {"code": "SW-CLEAN", "name": "清洁", "standard": "无碎屑"}
]', 1),
('PM-TEMP-007', 'Plating Line', 'Monthly', '电镀线月度保养', '[
    {"code": "PL-CHEM", "name": "药液浓度检查", "standard": "在规格内"},
    {"code": "PL-PH", "name": "pH值检查", "standard": "在规格内"},
    {"code": "PL-TEMP", "name": "温度检查", "standard": "±2°C"},
    {"code": "PL-CLEAN", "name": "槽体清洁", "standard": "无沉淀"}
]', 1),
('PM-TEMP-008', 'Laser Mark', 'Daily', '激光打标机日常保养', '[
    {"code": "LM-POWER", "name": "功率检查", "standard": "在规格内"},
    {"code": "LM-FOCUS", "name": "焦距校准", "standard": "准确"},
    {"code": "LM-CLEAN", "name": "镜片清洁", "standard": "无污渍"}
]', 1),
('PM-TEMP-009', 'AOI', 'Daily', 'AOI日常保养', '[
    {"code": "AOI-CAL", "name": "校准", "standard": "精度达标"},
    {"code": "AOI-LENS", "name": "镜头清洁", "standard": "无污渍"},
    {"code": "AOI-LIGHT", "name": "光源检查", "standard": "亮度正常"}
]', 1),
('PM-TEMP-010', 'X-Ray', 'Weekly', 'X-Ray检查机周保养', '[
    {"code": "XR-CAL", "name": "校准", "standard": "精度达标"},
    {"code": "XR-SAFE", "name": "安全检查", "standard": "辐射正常"},
    {"code": "XR-IMAGE", "name": "图像检查", "standard": "清晰"}
]', 1),
('PM-TEMP-011', 'Bake Oven', 'Monthly', '烘烤炉月度保养', '[
    {"code": "BK-TEMP", "name": "温度校准", "standard": "±5°C"},
    {"code": "BK-NITRO", "name": "氮气检查", "standard": "流量正常"},
    {"code": "BK-CLEAN", "name": "腔体清洁", "standard": "无残留"}
]', 1),
('PM-TEMP-012', 'Pack Machine', 'Daily', '包装机日常保养', '[
    {"code": "PK-CLEAN", "name": "清洁", "standard": "无碎屑"},
    {"code": "PK-SEAL", "name": "封口检查", "standard": "密封良好"},
    {"code": "PK-LABEL", "name": "标签检查", "standard": "打印正确"}
]', 1)
ON DUPLICATE KEY UPDATE `template_name` = VALUES(`template_name`);

-- ============================================================
-- 6. 首件检验模板数据 (first_article_template)
-- ============================================================
CREATE TABLE IF NOT EXISTS `first_article_template` (
    `template_id` VARCHAR(50) NOT NULL,
    `step_code` VARCHAR(50) NOT NULL,
    `template_name` VARCHAR(100) NOT NULL,
    `check_items` JSON DEFAULT NULL,
    `is_active` TINYINT(1) NOT NULL DEFAULT 1,
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`template_id`),
    INDEX `idx_step_code` (`step_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO `first_article_template` (`template_id`, `step_code`, `template_name`, `check_items`, `is_active`)
VALUES
('FA-TEMP-001', 'WIRE_BOND', '键合首件检验模板', '[
    {"code": "FA-WB-001", "name": "焊线拉力", "type": "Functional", "standard": ">=5gf", "unit": "gf"},
    {"code": "FA-WB-002", "name": "焊点位置", "type": "Dimensional", "standard": "偏差<0.02mm", "unit": "mm"},
    {"code": "FA-WB-003", "name": "线弧高度", "type": "Dimensional", "standard": "在规格内", "unit": "mm"},
    {"code": "FA-WB-004", "name": "外观检查", "type": "Visual", "standard": "无异常", "unit": null}
]', 1),
('FA-TEMP-002', 'DIE_ATTACH', '固晶首件检验模板', '[
    {"code": "FA-DA-001", "name": "芯片位置偏差", "type": "Dimensional", "standard": "<0.05mm", "unit": "mm"},
    {"code": "FA-DA-002", "name": "胶量检查", "type": "Visual", "standard": "均匀覆盖", "unit": null},
    {"code": "FA-DA-003", "name": "溢胶检查", "type": "Visual", "standard": "无溢胶", "unit": null}
]', 1),
('FA-TEMP-003', 'MOLD', '塑封首件检验模板', '[
    {"code": "FA-MD-001", "name": "外观检查", "type": "Visual", "standard": "无缺陷", "unit": null},
    {"code": "FA-MD-002", "name": "尺寸检查", "type": "Dimensional", "standard": "在规格内", "unit": "mm"},
    {"code": "FA-MD-003", "name": '毛刺检查', 'type': 'Visual', 'standard': '毛刺<0.05mm', 'unit': 'mm'}
]', 1),
('FA-TEMP-004', 'DIE_SAW', '切割首件检验模板', '[
    {"code": "FA-DS-001", "name": "切割位置", "type": "Dimensional", "standard": "偏差<0.01mm", "unit": "mm"},
    {"code": "FA-DS-002", "name": "切割深度", "type": "Dimensional", "standard": "在规格内", "unit": "mm"},
    {"code": "FA-DS-003", "name": "背面崩边", "type": "Visual", "standard": "崩边<1/3厚度", "unit": null}
]', 1),
('FA-TEMP-005', 'PLATING', '电镀首件检验模板', '[
    {"code": "FA-PL-001", "name": "镀层厚度", "type": "Dimensional", "standard": "在规格内", "unit": "μm"},
    {"code": "FA-PL-002", "name": "外观检查", "type": "Visual", "standard": "无氧化变色", "unit": null},
    {"code": "FA-PL-003", "name": "附着力", "type": "Functional", "standard": "胶带测试通过", "unit": null}
]', 1)
ON DUPLICATE KEY UPDATE `template_name` = VALUES(`template_name`);
