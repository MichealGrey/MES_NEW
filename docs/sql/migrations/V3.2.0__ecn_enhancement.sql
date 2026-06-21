-- V3.2.0: 工程变更(ECN)增强 - 扩展主表 + 新增5个子表

-- 扩展 ecn_request 主表
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS ecn_no VARCHAR(50) DEFAULT '';
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS change_category VARCHAR(50);
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS change_description TEXT;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS change_content TEXT;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS old_value VARCHAR(500);
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS new_value VARCHAR(500);
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS urgency VARCHAR(20);
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS risk_level VARCHAR(20);
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS review_comments TEXT;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS reject_reason TEXT;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS verify_result TEXT;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS planned_date DATETIME;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS actual_date DATETIME;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS is_complete TINYINT(1) DEFAULT 0;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS close_date DATETIME;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS days_elapsed INT;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS oa_flow_id VARCHAR(50);
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS oa_no VARCHAR(50);
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS is_urgent TINYINT(1) DEFAULT 0;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS cost_estimate DECIMAL(15,2);
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS remark TEXT;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS created_at DATETIME DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS created_by VARCHAR(50);
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS updated_at DATETIME;
ALTER TABLE ecn_request ADD COLUMN IF NOT EXISTS updated_by VARCHAR(50);

-- 新增索引
ALTER TABLE ecn_request ADD INDEX IF NOT EXISTS idx_ecn_no (ecn_no);
ALTER TABLE ecn_request ADD INDEX IF NOT EXISTS idx_ecn_type (ecn_type);

-- 子表1: ecn_item (变更项)
CREATE TABLE IF NOT EXISTS ecn_item (
    item_id VARCHAR(50) PRIMARY KEY,
    ecn_id VARCHAR(50) NOT NULL,
    item_type VARCHAR(50) NOT NULL,
    item_code VARCHAR(100) NOT NULL,
    item_name VARCHAR(200) NOT NULL,
    old_value TEXT,
    new_value TEXT,
    change_reason TEXT,
    remark VARCHAR(500),
    sort_order INT DEFAULT 0,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(50),
    updated_at DATETIME,
    updated_by VARCHAR(50),
    INDEX idx_ecn_id (ecn_id),
    INDEX idx_item_code (item_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 子表2: ecn_impact_item (影响评估项)
CREATE TABLE IF NOT EXISTS ecn_impact_item (
    impact_id VARCHAR(50) PRIMARY KEY,
    ecn_id VARCHAR(50) NOT NULL,
    impact_type VARCHAR(50) NOT NULL,
    severity VARCHAR(20) DEFAULT 'Medium',
    description TEXT,
    impact_analysis TEXT,
    action TEXT,
    responsible VARCHAR(50),
    due_date DATETIME,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(50),
    INDEX idx_ecn_id (ecn_id),
    INDEX idx_impact_type (impact_type),
    INDEX idx_severity (severity)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 子表3: ecn_approver (审批人)
CREATE TABLE IF NOT EXISTS ecn_approver (
    approver_id VARCHAR(50) PRIMARY KEY,
    ecn_id VARCHAR(50) NOT NULL,
    approver_name VARCHAR(50) NOT NULL,
    role VARCHAR(50),
    approval_order INT DEFAULT 1,
    status VARCHAR(20) DEFAULT 'Pending',
    result VARCHAR(50),
    comments TEXT,
    approved_at DATETIME,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_ecn_id (ecn_id),
    INDEX idx_status (status),
    INDEX idx_approval_order (approval_order)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 子表4: ecn_notify_dept (通知部门)
CREATE TABLE IF NOT EXISTS ecn_notify_dept (
    notify_id VARCHAR(50) PRIMARY KEY,
    ecn_id VARCHAR(50) NOT NULL,
    dept_id VARCHAR(50) NOT NULL,
    dept_name VARCHAR(100) NOT NULL,
    confirmed TINYINT(1) DEFAULT 0,
    notified_at DATETIME,
    confirmed_by VARCHAR(50),
    confirmed_at DATETIME,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_ecn_id (ecn_id),
    INDEX idx_dept_id (dept_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 子表5: ecn_implement (实施记录)
CREATE TABLE IF NOT EXISTS ecn_implement (
    implement_id VARCHAR(50) PRIMARY KEY,
    ecn_id VARCHAR(50) NOT NULL,
    task_name VARCHAR(200) NOT NULL,
    description TEXT,
    responsible VARCHAR(50),
    plan_date DATETIME,
    actual_date DATETIME,
    status VARCHAR(20) DEFAULT 'Pending',
    result VARCHAR(500),
    remark VARCHAR(500),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    created_by VARCHAR(50),
    updated_at DATETIME,
    updated_by VARCHAR(50),
    INDEX idx_ecn_id (ecn_id),
    INDEX idx_status (status),
    INDEX idx_responsible (responsible)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
