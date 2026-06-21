-- ============================================================================
-- V3.0.0 - API Layer & Permission System Tables
-- Date: 2026-06-04
-- Description: Add permission system tables, complaint 8D, NPI, SPC tables
-- ============================================================================

-- 0. 签字级别表
CREATE TABLE IF NOT EXISTS sys_signature_level (
    level_code VARCHAR(10) PRIMARY KEY,
    level_name VARCHAR(50) NOT NULL,
    level_order INT NOT NULL,
    description VARCHAR(200),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 1. 菜单定义表
CREATE TABLE IF NOT EXISTS sys_menu (
    menu_id VARCHAR(50) PRIMARY KEY,
    menu_name VARCHAR(100) NOT NULL,
    parent_id VARCHAR(50),
    icon VARCHAR(50),
    view_name VARCHAR(100),
    module_key VARCHAR(50),
    permission_code VARCHAR(100),
    sort_order INT DEFAULT 0,
    is_visible TINYINT(1) DEFAULT 1,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_parent (parent_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 2. 用户-角色多对多关联
CREATE TABLE IF NOT EXISTS sys_user_role (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    user_id VARCHAR(50) NOT NULL,
    role_id VARCHAR(50) NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    UNIQUE KEY uk_user_role (user_id, role_id),
    INDEX idx_user (user_id),
    INDEX idx_role (role_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. 角色-菜单关联
CREATE TABLE IF NOT EXISTS sys_role_menu (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    role_id VARCHAR(50) NOT NULL,
    menu_id VARCHAR(50) NOT NULL,
    UNIQUE KEY uk_role_menu (role_id, menu_id),
    INDEX idx_role (role_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 4. 角色-权限码关联
CREATE TABLE IF NOT EXISTS sys_role_permission (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    role_id VARCHAR(50) NOT NULL,
    permission_code VARCHAR(100) NOT NULL,
    UNIQUE KEY uk_role_perm (role_id, permission_code),
    INDEX idx_role (role_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5. 登录日志
CREATE TABLE IF NOT EXISTS sys_login_log (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    user_id VARCHAR(50),
    employee_id VARCHAR(50),
    login_time DATETIME,
    ip_address VARCHAR(50),
    result VARCHAR(20),
    error_message TEXT,
    INDEX idx_user (user_id),
    INDEX idx_login_time (login_time)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 6. 客诉8D主表
CREATE TABLE IF NOT EXISTS complaint_8d (
    complaint_id VARCHAR(50) PRIMARY KEY,
    customer_id VARCHAR(50) NOT NULL,
    customer_name VARCHAR(100),
    lot_id VARCHAR(50),
    product_id VARCHAR(50),
    defect_type VARCHAR(50),
    severity VARCHAR(20),
    status VARCHAR(20) DEFAULT 'Open',
    d1_team_members JSON,
    d2_problem_description TEXT,
    d3_containment_action TEXT,
    d4_root_cause TEXT,
    d5_permanent_action TEXT,
    d6_implementation TEXT,
    d7_prevention TEXT,
    d8_closure_comment TEXT,
    created_by VARCHAR(50),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    closed_at DATETIME,
    closed_by VARCHAR(50),
    due_date DATETIME,
    INDEX idx_customer (customer_id),
    INDEX idx_status (status),
    INDEX idx_created (created_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 7. 工程变更请求
CREATE TABLE IF NOT EXISTS ecn_request (
    ecn_id VARCHAR(50) PRIMARY KEY,
    ecn_title VARCHAR(200),
    ecn_type VARCHAR(50),
    reason TEXT,
    status VARCHAR(20) DEFAULT 'Draft',
    affected_routes JSON,
    affected_products JSON,
    impact_assessment TEXT,
    requested_by VARCHAR(50),
    requested_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    approved_by VARCHAR(50),
    approved_at DATETIME,
    effective_date DATETIME,
    INDEX idx_status (status),
    INDEX idx_requested (requested_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 8. NPI项目表
CREATE TABLE IF NOT EXISTS npi_project (
    project_id VARCHAR(50) PRIMARY KEY,
    project_name VARCHAR(100),
    customer_id VARCHAR(50),
    product_id VARCHAR(50),
    status VARCHAR(20),
    phase VARCHAR(20),
    start_date DATE,
    target_completion DATE,
    actual_completion DATE,
    created_by VARCHAR(50),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_customer (customer_id),
    INDEX idx_status (status)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 9. SPC测量数据
CREATE TABLE IF NOT EXISTS spc_measurement (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    lot_id VARCHAR(50),
    step_code VARCHAR(50),
    parameter_name VARCHAR(50),
    measured_value DECIMAL(10,4),
    usl DECIMAL(10,4),
    lsl DECIMAL(10,4),
    target_value DECIMAL(10,4),
    equipment_id VARCHAR(50),
    operator_id VARCHAR(50),
    measured_at DATETIME,
    is_out_of_control TINYINT(1) DEFAULT 0,
    INDEX idx_step_param (step_code, parameter_name),
    INDEX idx_measured_at (measured_at),
    INDEX idx_lot (lot_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 10. 排班表
CREATE TABLE IF NOT EXISTS shift_schedule (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    employee_id VARCHAR(50) NOT NULL,
    shift_date DATE NOT NULL,
    shift_type VARCHAR(20),
    department_id VARCHAR(50),
    workshop VARCHAR(50),
    INDEX idx_employee_date (employee_id, shift_date),
    INDEX idx_date (shift_date)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================================
-- 性能优化索引 (忽略已存在的索引)
-- ============================================================================
-- 使用存储过程安全创建索引
DROP PROCEDURE IF EXISTS AddIndexIfNotExists;
DELIMITER $$
CREATE PROCEDURE AddIndexIfNotExists()
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='mes_prod' AND table_name='prod_lot_step' AND index_name='idx_lot_step_lot_status') THEN
        ALTER TABLE prod_lot_step ADD INDEX idx_lot_step_lot_status (lot_id, status);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='mes_prod' AND table_name='prod_operation_history' AND index_name='idx_operation_lot_type') THEN
        ALTER TABLE prod_operation_history ADD INDEX idx_operation_lot_type (lot_id, operation_type, created_at);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='mes_prod' AND table_name='lot_trace_chain' AND index_name='idx_trace_child') THEN
        ALTER TABLE lot_trace_chain ADD INDEX idx_trace_child (child_lot_id);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='mes_prod' AND table_name='lot_trace_chain' AND index_name='idx_trace_parent') THEN
        ALTER TABLE lot_trace_chain ADD INDEX idx_trace_parent (parent_lot_id);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='mes_prod' AND table_name='mfg_operation_history' AND index_name='idx_mfg_lot_step') THEN
        ALTER TABLE mfg_operation_history ADD INDEX idx_mfg_lot_step (lot_id, step_code);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='mes_prod' AND table_name='prod_work_order' AND index_name='idx_wo_customer_status') THEN
        ALTER TABLE prod_work_order ADD INDEX idx_wo_customer_status (customer_id, status);
    END IF;
    IF NOT EXISTS (SELECT 1 FROM information_schema.statistics WHERE table_schema='mes_prod' AND table_name='prod_lot' AND index_name='idx_lot_order_id') THEN
        ALTER TABLE prod_lot ADD INDEX idx_lot_order_id (order_id);
    END IF;
END$$
DELIMITER ;
CALL AddIndexIfNotExists();
DROP PROCEDURE IF EXISTS AddIndexIfNotExists;
