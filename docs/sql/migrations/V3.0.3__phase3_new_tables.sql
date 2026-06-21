-- V3.0.3: Phase 3 New Tables
-- Creates tables for quality inspection items, equipment maintenance/failure, and NCR

SET @now = CURRENT_TIMESTAMP;

-- ============================================================================
-- 1. QUALITY_INSPECTION_ITEM - Individual inspection items
-- ============================================================================
CREATE TABLE IF NOT EXISTS quality_inspection_item (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    inspection_id VARCHAR(50) NOT NULL,
    item_code VARCHAR(50) NOT NULL COMMENT 'Inspection item code',
    item_name VARCHAR(100) NOT NULL COMMENT 'Inspection item name',
    specification VARCHAR(100) COMMENT 'Specification target',
    usl DECIMAL(10,4) COMMENT 'Upper specification limit',
    lsl DECIMAL(10,4) COMMENT 'Lower specification limit',
    target_value DECIMAL(10,4) COMMENT 'Target value',
    measured_value DECIMAL(10,4) COMMENT 'Actual measured value',
    unit VARCHAR(20) COMMENT 'Measurement unit',
    result VARCHAR(20) NOT NULL COMMENT 'PASS/FAIL/NA',
    remark VARCHAR(500),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_inspection (inspection_id),
    INDEX idx_item_code (item_code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================================
-- 2. NON_CONFORMANCE_REPORT - NCR records
-- ============================================================================
CREATE TABLE IF NOT EXISTS non_conformance_report (
    ncr_id VARCHAR(50) PRIMARY KEY,
    lot_id VARCHAR(50) NOT NULL,
    step_code VARCHAR(50) COMMENT 'Process step where defect found',
    defect_type VARCHAR(50) NOT NULL COMMENT 'Defect category',
    defect_description TEXT NOT NULL COMMENT 'Defect description',
    quantity INT NOT NULL COMMENT 'Affected quantity',
    severity VARCHAR(20) NOT NULL COMMENT 'Critical/Major/Minor',
    status VARCHAR(30) NOT NULL DEFAULT 'Open' COMMENT 'Open/UnderReview/Dispositioned/Closed',
    disposition VARCHAR(50) COMMENT 'Rework/Scrap/Return/UseAsIs',
    disposition_detail TEXT,
    discovered_by VARCHAR(50) NOT NULL,
    discovered_at DATETIME NOT NULL,
    reviewer_id VARCHAR(50),
    reviewed_at DATETIME,
    resolved_by VARCHAR(50),
    resolved_at DATETIME,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_lot (lot_id),
    INDEX idx_status (status),
    INDEX idx_severity (severity),
    INDEX idx_discovered_at (discovered_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================================
-- 3. EQUIPMENT_MAINTENANCE - Equipment maintenance records
-- ============================================================================
CREATE TABLE IF NOT EXISTS equipment_maintenance (
    maintenance_id VARCHAR(50) PRIMARY KEY,
    equipment_id VARCHAR(50) NOT NULL,
    maintenance_type VARCHAR(30) NOT NULL COMMENT 'Preventive/Corrective/Calibration',
    description TEXT,
    status VARCHAR(30) NOT NULL DEFAULT 'Scheduled' COMMENT 'Scheduled/InProgress/Completed/Cancelled',
    technician_id VARCHAR(50),
    scheduled_at DATETIME NOT NULL,
    started_at DATETIME,
    completed_at DATETIME,
    actual_hours DECIMAL(5,2),
    parts_replaced VARCHAR(500),
    notes TEXT,
    created_by VARCHAR(50),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_equipment (equipment_id),
    INDEX idx_status (status),
    INDEX idx_type (maintenance_type),
    INDEX idx_scheduled_at (scheduled_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================================================
-- 4. EQUIPMENT_FAILURE - Equipment failure records
-- ============================================================================
CREATE TABLE IF NOT EXISTS equipment_failure (
    failure_id VARCHAR(50) PRIMARY KEY,
    equipment_id VARCHAR(50) NOT NULL,
    failure_type VARCHAR(50) NOT NULL COMMENT 'Mechanical/Electrical/Optical/Hydraulic/Software',
    description TEXT NOT NULL,
    severity VARCHAR(20) NOT NULL COMMENT 'Critical/Major/Minor',
    status VARCHAR(30) NOT NULL DEFAULT 'Open' COMMENT 'Open/InProgress/Resolved',
    reported_by VARCHAR(50) NOT NULL,
    reported_at DATETIME NOT NULL,
    resolved_at DATETIME,
    resolved_by VARCHAR(50),
    downtime_minutes INT COMMENT 'Total downtime in minutes',
    root_cause TEXT,
    resolution TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_equipment (equipment_id),
    INDEX idx_status (status),
    INDEX idx_severity (severity),
    INDEX idx_reported_at (reported_at)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
