USE `mes_prod`;

-- 1. Add process_stage to prod_lot
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'prod_lot' AND table_schema = DATABASE() AND column_name = 'process_stage') > 0,
  'SELECT 1',
  'ALTER TABLE prod_lot ADD COLUMN process_stage VARCHAR(20) NULL AFTER status'
));
PREPARE stmt FROM @preparedStatement;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 2. Add columns to prod_lot_archive
SET @col = 'product_name';
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'prod_lot_archive' AND table_schema = DATABASE() AND column_name = @col) > 0,
  'SELECT 1',
  'ALTER TABLE prod_lot_archive ADD COLUMN product_name VARCHAR(200) NULL AFTER product_id'
));
PREPARE stmt FROM @preparedStatement;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col = 'route_id';
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'prod_lot_archive' AND table_schema = DATABASE() AND column_name = @col) > 0,
  'SELECT 1',
  'ALTER TABLE prod_lot_archive ADD COLUMN route_id VARCHAR(100) NULL AFTER product_name'
));
PREPARE stmt FROM @preparedStatement;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col = 'process_stage';
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'prod_lot_archive' AND table_schema = DATABASE() AND column_name = @col) > 0,
  'SELECT 1',
  'ALTER TABLE prod_lot_archive ADD COLUMN process_stage VARCHAR(20) NULL AFTER route_id'
));
PREPARE stmt FROM @preparedStatement;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col = 'total_rework_qty';
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'prod_lot_archive' AND table_schema = DATABASE() AND column_name = @col) > 0,
  'SELECT 1',
  'ALTER TABLE prod_lot_archive ADD COLUMN total_rework_qty INT NULL AFTER total_scrap_qty'
));
PREPARE stmt FROM @preparedStatement;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col = 'total_hold_qty';
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'prod_lot_archive' AND table_schema = DATABASE() AND column_name = @col) > 0,
  'SELECT 1',
  'ALTER TABLE prod_lot_archive ADD COLUMN total_hold_qty INT NULL AFTER total_rework_qty'
));
PREPARE stmt FROM @preparedStatement;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col = 'grade';
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'prod_lot_archive' AND table_schema = DATABASE() AND column_name = @col) > 0,
  'SELECT 1',
  'ALTER TABLE prod_lot_archive ADD COLUMN grade VARCHAR(10) NULL AFTER final_yield'
));
PREPARE stmt FROM @preparedStatement;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SET @col = 'archived_by';
SET @preparedStatement = (SELECT IF(
  (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = 'prod_lot_archive' AND table_schema = DATABASE() AND column_name = @col) > 0,
  'SELECT 1',
  'ALTER TABLE prod_lot_archive ADD COLUMN archived_by VARCHAR(50) NULL AFTER archived_at'
));
PREPARE stmt FROM @preparedStatement;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Drop existing PK if it's on lot_id, then add id as auto-increment PK
ALTER TABLE prod_lot_archive DROP PRIMARY KEY, ADD COLUMN id BIGINT AUTO_INCREMENT PRIMARY KEY FIRST;

-- Add indexes (ignore duplicate key errors)
ALTER TABLE `prod_lot_archive` ADD INDEX `idx_archive_lot_id` (`lot_id`);
ALTER TABLE `prod_lot_archive` ADD INDEX `idx_archive_order_id` (`order_id`);
ALTER TABLE `prod_lot_archive` ADD INDEX `idx_archive_completed_at` (`completed_at`);
ALTER TABLE `prod_lot_archive` ADD INDEX `idx_archive_stage` (`process_stage`);

-- 3. Update existing data
UPDATE `prod_lot` SET `process_stage` = 'Backend' WHERE `process_stage` IS NULL;
UPDATE `prod_lot` SET `grade` = 'G1' WHERE `grade` IS NULL AND `order_id` IN ('WO-2026001', 'WO-2026003', 'WO-2026006', 'WO-2026007');
UPDATE `prod_lot` SET `grade` = 'G2' WHERE `grade` IS NULL AND `order_id` = 'WO-2026005';
UPDATE `prod_lot` SET `grade` = 'G3' WHERE `grade` IS NULL AND `order_id` IN ('WO-2026002', 'WO-2026004', 'WO-2026008');

-- 4. Seed data - grade split
INSERT IGNORE INTO `prod_lot_split` (`split_id`, `mother_lot_id`, `child_lot_id`, `split_qty`, `split_reason`, `split_type`, `step_code`, `step_seq`, `operator_id`, `split_time`, `approved_by`) VALUES
('SPLIT-003', 'LOT-009', 'LOT-009-G1', 8000, 'FT grade split-G1', 'Grade', 'FT', 6, 'USR-009', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-003'),
('SPLIT-004', 'LOT-009', 'LOT-009-G2', 1500, 'FT grade split-G2', 'Grade', 'FT', 6, 'USR-009', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-003'),
('SPLIT-005', 'LOT-009', 'LOT-009-SCRAP', 500, 'FT grade split-Scrap', 'Scrap', 'FT', 6, 'USR-009', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-003');

INSERT IGNORE INTO `prod_lot` (`lot_id`, `order_id`, `product_id`, `product_name`, `die_name`, `package_type`, `route_id`, `route_version`, `current_step_seq`, `current_step_code`, `status`, `process_stage`, `original_qty`, `total_pass_qty`, `total_scrap_qty`, `total_rework_qty`, `total_hold_qty`, `is_rework_lot`, `carrier_type`, `carrier_id`, `grade`) VALUES
('LOT-009-G1', 'WO-2026005', 'PROD-SOP8', 'SOP-8 MOSFET', 'MOS-200', 'SOP', 'SOP8-STD:1.0', '1.0', 7, 'OQC', 'Processing', 'Backend', 8000, 7800, 50, 0, 0, 0, 'LeadFrame', 'CARRIER-LEAD-005', 'G1'),
('LOT-009-G2', 'WO-2026005', 'PROD-SOP8', 'SOP-8 MOSFET', 'MOS-200', 'SOP', 'SOP8-STD:1.0', '1.0', 7, 'OQC', 'Processing', 'Backend', 1500, 1400, 30, 0, 0, 0, 'LeadFrame', 'CARRIER-LEAD-004', 'G2');

-- 5. Engineering Hold
INSERT IGNORE INTO `prod_hold_record` (`hold_id`, `lot_id`, `hold_type`, `hold_reason_code`, `hold_reason`, `hold_qty`, `responsible_dept`, `owner`, `status`, `hold_by`, `hold_time`, `root_cause`, `corrective_action`, `disposition`, `release_by`, `release_time`) VALUES
('HOLD-004', 'LOT-012', 'Engineering', 'ENG_HOLD', 'ECN verification in progress', 2000, 'DEPT-ENG', 'USR-003', 'Open', 'USR-003', DATE_SUB(NOW(), INTERVAL 1 DAY), 'ECN-2026-001', 'Verify then release', 'Pending', NULL, NULL);

-- 6. Customer Hold
INSERT IGNORE INTO `prod_hold_record` (`hold_id`, `lot_id`, `hold_type`, `hold_reason_code`, `hold_reason`, `hold_qty`, `responsible_dept`, `owner`, `status`, `hold_by`, `hold_time`, `root_cause`, `corrective_action`, `disposition`, `release_by`, `release_time`) VALUES
('HOLD-005', 'LOT-007', 'Customer', 'CUST_HOLD', 'Customer requires additional reliability test', 500, 'DEPT-QA', 'USR-002', 'Open', 'USR-002', DATE_SUB(NOW(), INTERVAL 3 DAY), 'Customer special request', 'Complete test then release', 'Pending test', NULL, NULL);

-- 7. MRB review
INSERT IGNORE INTO `prod_hold_record` (`hold_id`, `lot_id`, `hold_type`, `hold_reason_code`, `hold_reason`, `hold_qty`, `responsible_dept`, `owner`, `status`, `hold_by`, `hold_time`, `root_cause`, `corrective_action`, `disposition`, `release_by`, `release_time`) VALUES
('HOLD-006', 'LOT-008', 'MRB', 'MRB_REVIEW', 'Mold anomaly pending MRB review', 300, 'DEPT-QA', 'USR-027', 'Open', 'USR-027', DATE_SUB(NOW(), INTERVAL 2 DAY), 'Mold bubble rate exceeds limit', 'Pending MRB decision', 'Pending review', NULL, NULL);

UPDATE `prod_lot` SET `is_under_mrb` = 1, `mrb_reference` = 'MRB-2026-001' WHERE `lot_id` = 'LOT-008';

-- 8. Equipment status updates
UPDATE `master_equipment` SET `status` = 'Offline' WHERE `equipment_id` IN ('DA-004', 'WB-004');
UPDATE `master_equipment` SET `status` = 'Maintenance' WHERE `equipment_id` IN ('SAW-004', 'FT-004');

-- 9. Rework equipment
INSERT IGNORE INTO `master_equipment` (`equipment_id`, `equipment_name`, `equipment_group`, `equipment_type`, `status`, `location`, `responsible_person`, `last_maintenance_date`, `maintenance_interval_hours`, `running_hours`) VALUES
('DEBOND-001', 'Debonder #1', 'DEBOND', 'Debonder', 'Available', 'Line G-01', 'USR-015', DATE_SUB(NOW(), INTERVAL 10 DAY), 600, 200),
('CLEAN-001', 'Cleaner #1', 'CLEAN', 'Cleaner', 'Available', 'Line G-02', 'USR-015', DATE_SUB(NOW(), INTERVAL 5 DAY), 400, 150),
('DECAP-001', 'Decapsulator #1', 'DECAP', 'Decapsulator', 'Available', 'Line G-03', 'USR-015', DATE_SUB(NOW(), INTERVAL 8 DAY), 500, 100);

-- 10. New carriers
INSERT IGNORE INTO `master_carrier` (`carrier_id`, `carrier_type`, `status`, `capacity`, `use_count`, `max_use_count`, `location`) VALUES
('CARRIER-TRAY-006', 'Tray', 'Available', 500, 100, 1000, 'WH-A-05'),
('CARRIER-LEAD-006', 'LeadFrame', 'InUse', 1000, 300, 2000, 'Line C-01'),
('CARRIER-MAG-006', 'Magazine', 'Available', 200, 75, 500, 'WH-C-05');

-- 11. Quality gates and inspections
INSERT IGNORE INTO `quality_gate` (`gate_id`, `lot_id`, `step_code`, `gate_type`, `status`, `checker_id`, `check_result`, `checked_at`) VALUES
('GATE-008', 'LOT-007', 'WB', 'QA', 'Passed', 'USR-002', 'Pass', DATE_SUB(NOW(), INTERVAL 1 DAY)),
('GATE-009', 'LOT-008', 'MOLD', 'QA', 'Failed', 'USR-002', 'Fail', DATE_SUB(NOW(), INTERVAL 2 DAY)),
('GATE-010', 'LOT-011', 'BALL', 'QA', 'Passed', 'USR-002', 'Pass', DATE_SUB(NOW(), INTERVAL 1 DAY));

INSERT IGNORE INTO `quality_inspection` (`inspection_id`, `lot_id`, `step_code`, `inspection_type`, `result`, `inspector_id`, `inspection_time`) VALUES
('INSP-006', 'LOT-007', 'WB', 'InProcess', 'Pass', 'USR-002', DATE_SUB(NOW(), INTERVAL 1 DAY)),
('INSP-007', 'LOT-008', 'MOLD', 'InProcess', 'Fail', 'USR-002', DATE_SUB(NOW(), INTERVAL 2 DAY)),
('INSP-008', 'LOT-011', 'BALL', 'InProcess', 'Pass', 'USR-002', DATE_SUB(NOW(), INTERVAL 1 DAY));

-- 12. Lot archives
INSERT IGNORE INTO `prod_lot_archive` (`lot_id`, `order_id`, `product_id`, `product_name`, `route_id`, `process_stage`, `status`, `original_qty`, `total_pass_qty`, `total_scrap_qty`, `total_rework_qty`, `total_hold_qty`, `final_yield`, `grade`, `completed_at`, `archived_by`) VALUES
('LOT-004', 'WO-2026002', 'PROD-SOP16', 'SOP-16 电源IC', 'SOP-STD:1.0', 'Backend', 'Completed', 5000, 4800, 100, 100, 0, 96.0, 'G3', DATE_SUB(NOW(), INTERVAL 5 DAY), 'USR-013'),
('LOT-005', 'WO-2026002', 'PROD-SOP16', 'SOP-16 电源IC', 'SOP-STD:1.0', 'Backend', 'Completed', 5000, 4750, 150, 100, 0, 95.0, 'G3', DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-013'),
('LOT-006', 'WO-2026002', 'PROD-SOP16', 'SOP-16 电源IC', 'SOP-STD:1.0', 'Backend', 'Completed', 5000, 4850, 80, 70, 0, 97.0, 'G3', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-013'),
('LOT-014', 'WO-2026008', 'PROD-SOP14', 'SOP-14 运放', 'SOP14-STD:1.0', 'Backend', 'Completed', 20000, 19500, 300, 200, 0, 97.5, 'G3', DATE_SUB(NOW(), INTERVAL 6 DAY), 'USR-013'),
('LOT-015', 'WO-2026008', 'PROD-SOP14', 'SOP-14 运放', 'SOP14-STD:1.0', 'Backend', 'Completed', 20000, 19600, 250, 150, 0, 98.0, 'G3', DATE_SUB(NOW(), INTERVAL 5 DAY), 'USR-013');
