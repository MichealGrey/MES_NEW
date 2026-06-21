-- ============================================
-- Migration: V2.0.0 - Master Data Expansion
-- Description: Add customer management, reason/defect codes,
--              and extend existing master tables
-- Date: 2026-05-28
-- ============================================

-- -------------------------------------------
-- 1. Customer Master Table
-- -------------------------------------------
CREATE TABLE master_customer (
    customer_id       VARCHAR(50)  NOT NULL,
    customer_name     VARCHAR(100) NOT NULL,
    customer_code     VARCHAR(50)  NOT NULL UNIQUE,
    contact_person    VARCHAR(100),
    contact_phone     VARCHAR(50),
    email             VARCHAR(100),
    address           VARCHAR(255),
    customer_pn_prefix VARCHAR(20),
    quality_level     VARCHAR(20)  NOT NULL DEFAULT 'Industrial',
    special_requirements JSON,
    default_packing_spec VARCHAR(100),
    default_oqc_spec  VARCHAR(100),
    status            VARCHAR(20)  DEFAULT 'Active',
    created_at        DATETIME     DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (customer_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- -------------------------------------------
-- 2. Reason Code Master Table
-- -------------------------------------------
CREATE TABLE master_reason_code (
    reason_code_id    VARCHAR(50)  NOT NULL,
    category          VARCHAR(50)  NOT NULL,
    sub_category      VARCHAR(50),
    reason_text       VARCHAR(255) NOT NULL,
    applicable_to     VARCHAR(50)  NOT NULL,
    is_enabled        TINYINT(1)   DEFAULT 1,
    created_at        DATETIME     DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (reason_code_id),
    INDEX idx_category (category)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- -------------------------------------------
-- 3. Defect Code Master Table
-- -------------------------------------------
CREATE TABLE master_defect_code (
    defect_code_id    VARCHAR(50)  NOT NULL,
    defect_category   VARCHAR(50)  NOT NULL,
    defect_text       VARCHAR(255) NOT NULL,
    severity          VARCHAR(20)  DEFAULT 'Major',
    is_enabled        TINYINT(1)   DEFAULT 1,
    created_at        DATETIME     DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (defect_code_id),
    INDEX idx_category (defect_category)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- -------------------------------------------
-- 4. Extend master_product
-- -------------------------------------------
ALTER TABLE master_product
    ADD COLUMN process_stage    VARCHAR(50)  DEFAULT 'Assemble',
    ADD COLUMN default_route_id VARCHAR(100),
    ADD COLUMN unit_qty         INT          DEFAULT 1;

-- -------------------------------------------
-- 5. Extend master_equipment
-- -------------------------------------------
ALTER TABLE master_equipment
    ADD COLUMN process_stage    VARCHAR(50)  DEFAULT 'Assemble',
    ADD COLUMN vendor           VARCHAR(100),
    ADD COLUMN model            VARCHAR(100),
    ADD COLUMN serial_number    VARCHAR(100),
    ADD COLUMN capability       JSON;

-- -------------------------------------------
-- 6. Extend master_carrier
-- -------------------------------------------
ALTER TABLE master_carrier
    ADD COLUMN applicable_process   JSON,
    ADD COLUMN applicable_package   JSON;

-- -------------------------------------------
-- 7. Seed Data: Customers
-- -------------------------------------------
INSERT INTO master_customer (customer_id, customer_name, customer_code, quality_level) VALUES
    ('CUST-AUTO', '某汽车电子', 'CUST-AUTO', 'Automotive'),
    ('CUST-IND',  '某工业客户', 'CUST-IND',  'Industrial'),
    ('CUST-CON',  '某消费电子', 'CUST-CON',  'Consumer');

-- -------------------------------------------
-- 8. Seed Data: Reason Codes
-- -------------------------------------------
-- Material category
INSERT INTO master_reason_code (reason_code_id, category, sub_category, reason_text, applicable_to) VALUES
    ('RC-MAT-HOLD',    'Material',  'Incoming',   '来料异常待确认', 'Hold'),
    ('RC-MAT-SCRAP',   'Material',  'Defective',  '来料不良报废',   'Scrap'),
    ('RC-MAT-REWORK',  'Material',  'WrongPart',  '物料用错返工',   'Rework');

-- Equipment category
INSERT INTO master_reason_code (reason_code_id, category, sub_category, reason_text, applicable_to) VALUES
    ('RC-EQP-HOLD',    'Equipment', 'Breakdown',  '设备故障停机待修',  'Hold'),
    ('RC-EQP-SCRAP',   'Equipment', 'OutOfSpec',  '设备精度超差报废',  'Scrap'),
    ('RC-EQP-REWORK',  'Equipment', 'Calibration','设备校准后返工',    'Rework');

-- Process category
INSERT INTO master_reason_code (reason_code_id, category, sub_category, reason_text, applicable_to) VALUES
    ('RC-PRC-HOLD',    'Process',   'ParamDrift',  '工艺参数偏移暂停',  'Hold'),
    ('RC-PRC-SCRAP',   'Process',   'WrongRoute',  '走错工艺路线报废',  'Scrap'),
    ('RC-PRC-REWORK',  'Process',   'SkipStep',    '漏工序返工',        'Rework');

-- Quality category
INSERT INTO master_reason_code (reason_code_id, category, sub_category, reason_text, applicable_to) VALUES
    ('RC-QLT-HOLD',    'Quality',   'OQCFail',    'OQC检验不合格扣留',  'Hold'),
    ('RC-QLT-SCRAP',   'Quality',   'Cosmetic',   '外观不良报废',       'Scrap'),
    ('RC-QLT-REWORK',  'Quality',   'TestFail',   '测试不合格返工',     'Rework');

-- -------------------------------------------
-- 9. Seed Data: Defect Codes
-- -------------------------------------------
-- Cosmetic defects
INSERT INTO master_defect_code (defect_code_id, defect_category, defect_text, severity) VALUES
    ('DFC-COS-001', 'Cosmetic',   '划痕',        'Minor'),
    ('DFC-COS-002', 'Cosmetic',   '污渍',        'Minor'),
    ('DFC-COS-003', 'Cosmetic',   '色差',        'Minor'),
    ('DFC-COS-004', 'Cosmetic',   '毛刺',        'Major');

-- Dimensional defects
INSERT INTO master_defect_code (defect_code_id, defect_category, defect_text, severity) VALUES
    ('DFC-DIM-001', 'Dimensional','长度超差',     'Major'),
    ('DFC-DIM-002', 'Dimensional','宽度超差',     'Major'),
    ('DFC-DIM-003', 'Dimensional','厚度超差',     'Major'),
    ('DFC-DIM-004', 'Dimensional','孔径超差',     'Critical');

-- Electrical defects
INSERT INTO master_defect_code (defect_code_id, defect_category, defect_text, severity) VALUES
    ('DFC-ELC-001', 'Electrical', '短路',        'Critical'),
    ('DFC-ELC-002', 'Electrical', '开路',        'Critical'),
    ('DFC-ELC-003', 'Electrical', '漏电',        'Major'),
    ('DFC-ELC-004', 'Electrical', '阻值超差',     'Major');

-- Functional defects
INSERT INTO master_defect_code (defect_code_id, defect_category, defect_text, severity) VALUES
    ('DFC-FUN-001', 'Functional', '功能失效',     'Critical'),
    ('DFC-FUN-002', 'Functional', '性能不达标',   'Major'),
    ('DFC-FUN-003', 'Functional', '间歇性故障',   'Major'),
    ('DFC-FUN-004', 'Functional', '通讯异常',     'Major');
