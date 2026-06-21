-- ============================================================================
-- V3.0.2 - Phase 3 Seed Data (Production Records & Quality & Equipment)
-- Date: 2026-06-05
-- Description: Realistic OSAT production records covering typical manufacturing
--              scenarios: lot step progress, operation history, quality
--              inspections (IQC/IPQC/OQC), equipment maintenance & failures.
-- All data is interconnected with existing V3.0.1 seed data.
-- ============================================================================

SET @now = UTC_TIMESTAMP();

-- ============================================================================
-- 1. PROD_LOT_STEP - Batch Operation Records (~120 records)
-- ============================================================================

-- LOT-BGA-20260515-001 (Completed BGA MCU) - Full route
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-BGA-MCU01-S10', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 10, 'GRIND', 'Wafer Grinding', 'Completed', 'EQP-GRD-001', DATE_SUB(@now, INTERVAL 20 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 20 DAY), 'EMP-020', 50000, 49900, 50, 50, 'RCP-GRD-STD-001'),
('SR-BGA-MCU01-S20', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 20, 'DICE', 'Wafer Dicing', 'Completed', 'EQP-DIC-001', DATE_SUB(@now, INTERVAL 19 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 19 DAY), 'EMP-021', 49900, 49750, 80, 70, 'RCP-DIC-STD-001'),
('SR-BGA-MCU01-S30', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 30, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-001', DATE_SUB(@now, INTERVAL 19 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 18 DAY), 'EMP-020', 49750, 49600, 80, 70, 'RCP-DAB-BGA-001'),
('SR-BGA-MCU01-S40', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 40, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-001', DATE_SUB(@now, INTERVAL 18 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 18 DAY), 'EMP-022', 49600, 49550, 50, 0, 'RCP-OVN-DA-001'),
('SR-BGA-MCU01-S50', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'Wire Bonding', 'Completed', 'EQP-WBD-001', DATE_SUB(@now, INTERVAL 17 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 16 DAY), 'EMP-021', 49550, 49400, 100, 50, 'RCP-WBD-BGA-001'),
('SR-BGA-MCU01-S60', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 60, 'MOLD', 'Molding', 'Completed', 'EQP-MLD-001', DATE_SUB(@now, INTERVAL 16 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 16 DAY), 'EMP-020', 49400, 49250, 100, 50, 'RCP-MLD-BGA-001'),
('SR-BGA-MCU01-S70', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 70, 'CURE', 'Post Mold Cure', 'Completed', 'EQP-OVN-002', DATE_SUB(@now, INTERVAL 15 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 15 DAY), 'EMP-022', 49250, 49200, 50, 0, 'RCP-OVN-CURE-001'),
('SR-BGA-MCU01-S80', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 80, 'LASER_MARK', 'Laser Marking', 'Completed', 'EQP-LMK-001', DATE_SUB(@now, INTERVAL 14 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 14 DAY), 'EMP-021', 49200, 49150, 30, 20, 'RCP-LMK-BGA-001'),
('SR-BGA-MCU01-S90', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 90, 'PLATE', 'Plating', 'Completed', 'EQP-PLT-001', DATE_SUB(@now, INTERVAL 13 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 13 DAY), 'EMP-020', 49150, 49100, 50, 0, 'RCP-PLT-BGA-001'),
('SR-BGA-MCU01-S100', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 100, 'SINGULATE', 'Singulation', 'Completed', 'EQP-TRF-001', DATE_SUB(@now, INTERVAL 12 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 12 DAY), 'EMP-021', 49100, 49000, 50, 50, 'RCP-TRF-BGA-001'),
('SR-BGA-MCU01-S110', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 110, 'TEST_FT', 'Final Test', 'Completed', 'EQP-TST-001', DATE_SUB(@now, INTERVAL 11 DAY), 'EMP-028', DATE_SUB(@now, INTERVAL 10 DAY), 'EMP-028', 49000, 48500, 400, 100, 'RCP-TST-BGA-001'),
('SR-BGA-MCU01-S120', 'LOT-BGA-20260515-001', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Final Packing', 'Completed', 'EQP-PCK-001', DATE_SUB(@now, INTERVAL 10 DAY), 'EMP-029', DATE_SUB(@now, INTERVAL 10 DAY), 'EMP-029', 48500, 48400, 50, 50, NULL);

-- LOT-BGA-20260515-002 (Completed) - Full route
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-BGA-MCU02-S10', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 10, 'GRIND', 'Wafer Grinding', 'Completed', 'EQP-GRD-002', DATE_SUB(@now, INTERVAL 19 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 19 DAY), 'EMP-020', 50000, 49850, 80, 70, 'RCP-GRD-STD-001'),
('SR-BGA-MCU02-S20', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 20, 'DICE', 'Wafer Dicing', 'Completed', 'EQP-DIC-002', DATE_SUB(@now, INTERVAL 18 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 18 DAY), 'EMP-021', 49850, 49700, 80, 70, 'RCP-DIC-STD-001'),
('SR-BGA-MCU02-S30', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 30, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-002', DATE_SUB(@now, INTERVAL 18 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 17 DAY), 'EMP-020', 49700, 49550, 80, 70, 'RCP-DAB-BGA-001'),
('SR-BGA-MCU02-S40', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 40, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-003', DATE_SUB(@now, INTERVAL 17 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 17 DAY), 'EMP-022', 49550, 49500, 50, 0, 'RCP-OVN-DA-001'),
('SR-BGA-MCU02-S50', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'Wire Bonding', 'Completed', 'EQP-WBD-002', DATE_SUB(@now, INTERVAL 16 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 15 DAY), 'EMP-021', 49500, 49350, 100, 50, 'RCP-WBD-BGA-001'),
('SR-BGA-MCU02-S60', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 60, 'MOLD', 'Molding', 'Completed', 'EQP-MLD-002', DATE_SUB(@now, INTERVAL 15 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 15 DAY), 'EMP-020', 49350, 49200, 100, 50, 'RCP-MLD-BGA-001'),
('SR-BGA-MCU02-S70', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 70, 'CURE', 'Post Mold Cure', 'Completed', 'EQP-OVN-004', DATE_SUB(@now, INTERVAL 14 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 14 DAY), 'EMP-022', 49200, 49150, 50, 0, 'RCP-OVN-CURE-001'),
('SR-BGA-MCU02-S80', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 80, 'LASER_MARK', 'Laser Marking', 'Completed', 'EQP-LMK-002', DATE_SUB(@now, INTERVAL 13 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 13 DAY), 'EMP-021', 49150, 49100, 30, 20, 'RCP-LMK-BGA-001'),
('SR-BGA-MCU02-S90', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 90, 'PLATE', 'Plating', 'Completed', 'EQP-PLT-002', DATE_SUB(@now, INTERVAL 12 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 12 DAY), 'EMP-020', 49100, 49050, 50, 0, 'RCP-PLT-BGA-001'),
('SR-BGA-MCU02-S100', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 100, 'SINGULATE', 'Singulation', 'Completed', 'EQP-TRF-002', DATE_SUB(@now, INTERVAL 11 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 11 DAY), 'EMP-021', 49050, 48950, 50, 50, 'RCP-TRF-BGA-001'),
('SR-BGA-MCU02-S110', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 110, 'TEST_FT', 'Final Test', 'Completed', 'EQP-TST-002', DATE_SUB(@now, INTERVAL 10 DAY), 'EMP-028', DATE_SUB(@now, INTERVAL 9 DAY), 'EMP-028', 48950, 48450, 400, 100, 'RCP-TST-BGA-001'),
('SR-BGA-MCU02-S120', 'LOT-BGA-20260515-002', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Final Packing', 'Completed', 'EQP-PCK-002', DATE_SUB(@now, INTERVAL 9 DAY), 'EMP-029', DATE_SUB(@now, INTERVAL 9 DAY), 'EMP-029', 48450, 48350, 50, 50, NULL);

-- LOT-QFN-20260601-002 (QFN Sensor at TEST_FT)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-QFN-SEN02-S10', 'LOT-QFN-20260601-002', 'QFN-STD', '1.0', 10, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-003', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-020', 50000, 49850, 80, 70, 'RCP-DAB-QFN-001'),
('SR-QFN-SEN02-S20', 'LOT-QFN-20260601-002', 'QFN-STD', '1.0', 20, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-004', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-022', 49850, 49750, 100, 0, 'RCP-OVN-DA-001'),
('SR-QFN-SEN02-S30', 'LOT-QFN-20260601-002', 'QFN-STD', '1.0', 30, 'WIRE_BOND', 'Wire Bonding', 'Completed', 'EQP-WBD-003', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-021', 49750, 49600, 100, 50, 'RCP-WBD-QFN-001'),
('SR-QFN-SEN02-S40', 'LOT-QFN-20260601-002', 'QFN-STD', '1.0', 40, 'MOLD', 'Molding', 'Completed', 'EQP-MLD-002', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', 49600, 49450, 100, 50, 'RCP-MLD-QFN-001'),
('SR-QFN-SEN02-S50', 'LOT-QFN-20260601-002', 'QFN-STD', '1.0', 50, 'CURE', 'Post Mold Cure', 'Completed', 'EQP-OVN-002', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', 49450, 49400, 50, 0, 'RCP-OVN-CURE-001'),
('SR-QFN-SEN02-S60', 'LOT-QFN-20260601-002', 'QFN-STD', '1.0', 60, 'PLATE', 'Plating', 'Completed', 'EQP-PLT-002', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-020', 49400, 49300, 100, 0, 'RCP-PLT-QFN-001'),
('SR-QFN-SEN02-S70', 'LOT-QFN-20260601-002', 'QFN-STD', '1.0', 70, 'SINGULATE', 'Singulation', 'Completed', 'EQP-TRF-002', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', 49300, 49200, 100, 0, 'RCP-TRF-BGA-001'),
('SR-QFN-SEN02-S80', 'LOT-QFN-20260601-002', 'QFN-STD', '1.0', 80, 'TEST_FT', 'Final Test', 'InProgress', 'EQP-TST-002', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-028', NULL, NULL, 49200, 0, 0, 0, 'RCP-TST-QFN-001');

-- LOT-SOP-20260601-002 (SOP MOSFET at PLATE)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-SOP-MOS02-S10', 'LOT-SOP-20260601-002', 'SOP-STD', '1.0', 10, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-004', DATE_SUB(@now, INTERVAL 9 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 9 DAY), 'EMP-020', 50000, 49800, 100, 100, 'RCP-DAB-BGA-001'),
('SR-SOP-MOS02-S20', 'LOT-SOP-20260601-002', 'SOP-STD', '1.0', 20, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-003', DATE_SUB(@now, INTERVAL 8 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 8 DAY), 'EMP-022', 49800, 49700, 100, 0, 'RCP-OVN-DA-001'),
('SR-SOP-MOS02-S30', 'LOT-SOP-20260601-002', 'SOP-STD', '1.0', 30, 'WIRE_BOND', 'Wire Bonding', 'Completed', 'EQP-WBD-004', DATE_SUB(@now, INTERVAL 8 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-021', 49700, 49550, 100, 50, 'RCP-WBD-SOP-001'),
('SR-SOP-MOS02-S40', 'LOT-SOP-20260601-002', 'SOP-STD', '1.0', 40, 'MOLD', 'Molding', 'Completed', 'EQP-MLD-003', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-020', 49550, 49400, 100, 50, 'RCP-MLD-QFN-001'),
('SR-SOP-MOS02-S50', 'LOT-SOP-20260601-002', 'SOP-STD', '1.0', 50, 'CURE', 'Post Mold Cure', 'Completed', 'EQP-OVN-004', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-022', 49400, 49350, 50, 0, 'RCP-OVN-CURE-001'),
('SR-SOP-MOS02-S60', 'LOT-SOP-20260601-002', 'SOP-STD', '1.0', 60, 'PLATE', 'Plating', 'InProgress', 'EQP-PLT-003', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', NULL, NULL, 49350, 0, 0, 0, 'RCP-PLT-SOP-001');

-- LOT-DFN-20260601-001 (DFN LDO at TEST_FT)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-DFN-LDO01-S10', 'LOT-DFN-20260601-001', 'DFN-STD', '1.0', 10, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-001', DATE_SUB(@now, INTERVAL 8 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 8 DAY), 'EMP-020', 50000, 49850, 80, 70, 'RCP-DAB-DFN-001'),
('SR-DFN-LDO01-S20', 'LOT-DFN-20260601-001', 'DFN-STD', '1.0', 20, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-001', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-022', 49850, 49800, 50, 0, 'RCP-OVN-DA-001'),
('SR-DFN-LDO01-S30', 'LOT-DFN-20260601-001', 'DFN-STD', '1.0', 30, 'WIRE_BOND', 'Wire Bonding', 'Completed', 'EQP-WBD-001', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-021', 49800, 49650, 100, 50, 'RCP-WBD-QFN-001'),
('SR-DFN-LDO01-S40', 'LOT-DFN-20260601-001', 'DFN-STD', '1.0', 40, 'MOLD', 'Molding', 'Completed', 'EQP-MLD-003', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', 49650, 49500, 100, 50, 'RCP-MLD-QFN-001'),
('SR-DFN-LDO01-S50', 'LOT-DFN-20260601-001', 'DFN-STD', '1.0', 50, 'CURE', 'Post Mold Cure', 'Completed', 'EQP-OVN-003', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-022', 49500, 49450, 50, 0, 'RCP-OVN-CURE-001'),
('SR-DFN-LDO01-S60', 'LOT-DFN-20260601-001', 'DFN-STD', '1.0', 60, 'PLATE', 'Plating', 'Completed', 'EQP-PLT-001', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', 49450, 49400, 50, 0, 'RCP-PLT-QFN-001'),
('SR-DFN-LDO01-S70', 'LOT-DFN-20260601-001', 'DFN-STD', '1.0', 70, 'SINGULATE', 'Singulation', 'Completed', 'EQP-TRF-003', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', 49400, 49350, 50, 0, 'RCP-TRF-BGA-001'),
('SR-DFN-LDO01-S80', 'LOT-DFN-20260601-001', 'DFN-STD', '1.0', 80, 'TEST_FT', 'Final Test', 'InProgress', 'EQP-TST-003', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-028', NULL, NULL, 49350, 0, 0, 0, 'RCP-TST-DFN-001');

-- LOT-DFN-20260601-002 (DFN LDO #2 at TEST_FT)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-DFN-LDO02-S10', 'LOT-DFN-20260601-002', 'DFN-STD', '1.0', 10, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-002', DATE_SUB(@now, INTERVAL 8 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 8 DAY), 'EMP-020', 50000, 49800, 100, 100, 'RCP-DAB-DFN-001'),
('SR-DFN-LDO02-S20', 'LOT-DFN-20260601-002', 'DFN-STD', '1.0', 20, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-002', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-022', 49800, 49750, 50, 0, 'RCP-OVN-DA-001'),
('SR-DFN-LDO02-S30', 'LOT-DFN-20260601-002', 'DFN-STD', '1.0', 30, 'WIRE_BOND', 'Wire Bonding', 'Completed', 'EQP-WBD-002', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-021', 49750, 49600, 100, 50, 'RCP-WBD-QFN-001'),
('SR-DFN-LDO02-S40', 'LOT-DFN-20260601-002', 'DFN-STD', '1.0', 40, 'MOLD', 'Molding', 'Completed', 'EQP-MLD-004', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', 49600, 49500, 100, 0, 'RCP-MLD-QFN-001'),
('SR-DFN-LDO02-S50', 'LOT-DFN-20260601-002', 'DFN-STD', '1.0', 50, 'CURE', 'Post Mold Cure', 'Completed', 'EQP-OVN-004', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-022', 49500, 49450, 50, 0, 'RCP-OVN-CURE-001'),
('SR-DFN-LDO02-S60', 'LOT-DFN-20260601-002', 'DFN-STD', '1.0', 60, 'PLATE', 'Plating', 'Completed', 'EQP-PLT-002', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', 49450, 49400, 50, 0, 'RCP-PLT-QFN-001'),
('SR-DFN-LDO02-S70', 'LOT-DFN-20260601-002', 'DFN-STD', '1.0', 70, 'SINGULATE', 'Singulation', 'Completed', 'EQP-TRF-001', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', 49400, 49300, 100, 0, 'RCP-TRF-BGA-001'),
('SR-DFN-LDO02-S80', 'LOT-DFN-20260601-002', 'DFN-STD', '1.0', 80, 'TEST_FT', 'Final Test', 'InProgress', 'EQP-TST-004', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-028', NULL, NULL, 49300, 0, 0, 0, 'RCP-TST-DFN-001');

-- LOT-BGA-20260601-003 (BGA WiFi at TEST_FT)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-BGA-WIFI03-S10', 'LOT-BGA-20260601-003', 'BGA-STD', '1.0', 10, 'GRIND', 'Wafer Grinding', 'Completed', 'EQP-GRD-001', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', 50000, 49850, 80, 70, 'RCP-GRD-STD-001'),
('SR-BGA-WIFI03-S20', 'LOT-BGA-20260601-003', 'BGA-STD', '1.0', 20, 'DICE', 'Wafer Dicing', 'Completed', 'EQP-DIC-001', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-021', 49850, 49700, 80, 70, 'RCP-DIC-STD-001'),
('SR-BGA-WIFI03-S30', 'LOT-BGA-20260601-003', 'BGA-STD', '1.0', 30, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-003', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', 49700, 49550, 80, 70, 'RCP-DAB-BGA-001'),
('SR-BGA-WIFI03-S40', 'LOT-BGA-20260601-003', 'BGA-STD', '1.0', 40, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-003', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', 49550, 49500, 50, 0, 'RCP-OVN-DA-001'),
('SR-BGA-WIFI03-S50', 'LOT-BGA-20260601-003', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'Wire Bonding', 'Completed', 'EQP-WBD-003', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', 49500, 49350, 100, 50, 'RCP-WBD-BGA-001'),
('SR-BGA-WIFI03-S60', 'LOT-BGA-20260601-003', 'BGA-STD', '1.0', 60, 'MOLD', 'Molding', 'Completed', 'EQP-MLD-001', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-020', 49350, 49200, 100, 50, 'RCP-MLD-BGA-001'),
('SR-BGA-WIFI03-S70', 'LOT-BGA-20260601-003', 'BGA-STD', '1.0', 70, 'CURE', 'Post Mold Cure', 'Completed', 'EQP-OVN-001', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-022', 49200, 49150, 50, 0, 'RCP-OVN-CURE-001'),
('SR-BGA-WIFI03-S80', 'LOT-BGA-20260601-003', 'BGA-STD', '1.0', 80, 'LASER_MARK', 'Laser Marking', 'Completed', 'EQP-LMK-001', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-021', 49150, 49100, 30, 20, 'RCP-LMK-BGA-001'),
('SR-BGA-WIFI03-S90', 'LOT-BGA-20260601-003', 'BGA-STD', '1.0', 90, 'PLATE', 'Plating', 'Completed', 'EQP-PLT-001', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-020', 49100, 49050, 50, 0, 'RCP-PLT-BGA-001'),
('SR-BGA-WIFI03-S100', 'LOT-BGA-20260601-003', 'BGA-STD', '1.0', 100, 'SINGULATE', 'Singulation', 'Completed', 'EQP-TRF-001', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-021', 49050, 48950, 50, 50, 'RCP-TRF-BGA-001'),
('SR-BGA-WIFI03-S110', 'LOT-BGA-20260601-003', 'BGA-STD', '1.0', 110, 'TEST_FT', 'Final Test', 'InProgress', 'EQP-TST-001', DATE_SUB(@now, INTERVAL 12 HOUR), 'EMP-028', NULL, NULL, 48950, 0, 0, 0, 'RCP-TST-BGA-001');

-- LOT-BGA-20260602-003 (BGA WiFi at PLATE)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-BGA-WIFI04-S10', 'LOT-BGA-20260602-003', 'BGA-STD', '1.0', 10, 'GRIND', 'Wafer Grinding', 'Completed', 'EQP-GRD-002', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', 50000, 49900, 50, 50, 'RCP-GRD-STD-001'),
('SR-BGA-WIFI04-S20', 'LOT-BGA-20260602-003', 'BGA-STD', '1.0', 20, 'DICE', 'Wafer Dicing', 'Completed', 'EQP-DIC-002', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', 49900, 49750, 80, 70, 'RCP-DIC-STD-001'),
('SR-BGA-WIFI04-S30', 'LOT-BGA-20260602-003', 'BGA-STD', '1.0', 30, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-004', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-020', 49750, 49600, 80, 70, 'RCP-DAB-BGA-001'),
('SR-BGA-WIFI04-S40', 'LOT-BGA-20260602-003', 'BGA-STD', '1.0', 40, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-004', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-022', 49600, 49550, 50, 0, 'RCP-OVN-DA-001'),
('SR-BGA-WIFI04-S50', 'LOT-BGA-20260602-003', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'Wire Bonding', 'Completed', 'EQP-WBD-004', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-021', 49550, 49400, 100, 50, 'RCP-WBD-BGA-001'),
('SR-BGA-WIFI04-S60', 'LOT-BGA-20260602-003', 'BGA-STD', '1.0', 60, 'MOLD', 'Molding', 'Completed', 'EQP-MLD-002', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-020', 49400, 49250, 100, 50, 'RCP-MLD-BGA-001'),
('SR-BGA-WIFI04-S70', 'LOT-BGA-20260602-003', 'BGA-STD', '1.0', 70, 'CURE', 'Post Mold Cure', 'Completed', 'EQP-OVN-002', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-022', 49250, 49200, 50, 0, 'RCP-OVN-CURE-001'),
('SR-BGA-WIFI04-S80', 'LOT-BGA-20260602-003', 'BGA-STD', '1.0', 80, 'LASER_MARK', 'Laser Marking', 'Completed', 'EQP-LMK-002', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-021', 49200, 49150, 30, 20, 'RCP-LMK-BGA-001'),
('SR-BGA-WIFI04-S90', 'LOT-BGA-20260602-003', 'BGA-STD', '1.0', 90, 'PLATE', 'Plating', 'InProgress', 'EQP-PLT-002', DATE_SUB(@now, INTERVAL 8 HOUR), 'EMP-020', NULL, NULL, 49150, 0, 0, 0, 'RCP-PLT-BGA-001');

-- LOT-QFN-20260601-003 (QFN Temp Sensor Auto at SINGULATE)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-QFN-TEMP03-S10', 'LOT-QFN-20260601-003', 'QFN-STD', '1.0', 10, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-001', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', 50000, 49850, 80, 70, 'RCP-DAB-QFN-001'),
('SR-QFN-TEMP03-S20', 'LOT-QFN-20260601-003', 'QFN-STD', '1.0', 20, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-001', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-022', 49850, 49800, 50, 0, 'RCP-OVN-DA-001'),
('SR-QFN-TEMP03-S30', 'LOT-QFN-20260601-003', 'QFN-STD', '1.0', 30, 'WIRE_BOND', 'Wire Bonding', 'Completed', 'EQP-WBD-001', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-021', 49800, 49650, 100, 50, 'RCP-WBD-QFN-001'),
('SR-QFN-TEMP03-S40', 'LOT-QFN-20260601-003', 'QFN-STD', '1.0', 40, 'MOLD', 'Molding', 'Completed', 'EQP-MLD-001', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-020', 49650, 49500, 100, 50, 'RCP-MLD-QFN-001'),
('SR-QFN-TEMP03-S50', 'LOT-QFN-20260601-003', 'QFN-STD', '1.0', 50, 'CURE', 'Post Mold Cure', 'Completed', 'EQP-OVN-002', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-022', 49500, 49450, 50, 0, 'RCP-OVN-CURE-001'),
('SR-QFN-TEMP03-S60', 'LOT-QFN-20260601-003', 'QFN-STD', '1.0', 60, 'PLATE', 'Plating', 'Completed', 'EQP-PLT-001', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 18 HOUR), 'EMP-020', 49450, 49400, 50, 0, 'RCP-PLT-QFN-001'),
('SR-QFN-TEMP03-S70', 'LOT-QFN-20260601-003', 'QFN-STD', '1.0', 70, 'SINGULATE', 'Singulation', 'InProgress', 'EQP-TRF-001', DATE_SUB(@now, INTERVAL 12 HOUR), 'EMP-021', NULL, NULL, 49400, 0, 0, 0, 'RCP-TRF-BGA-001');

-- LOT-QFP-20260601-001 (QFP MCU at TEST_FT)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-QFP-MCU01-S10', 'LOT-QFP-20260601-001', 'QFP-STD', '1.0', 10, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-003', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-020', 50000, 49800, 100, 100, 'RCP-DAB-QFP-001'),
('SR-QFP-MCU01-S20', 'LOT-QFP-20260601-001', 'QFP-STD', '1.0', 20, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-003', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-022', 49800, 49750, 50, 0, 'RCP-OVN-DA-001'),
('SR-QFP-MCU01-S30', 'LOT-QFP-20260601-001', 'QFP-STD', '1.0', 30, 'WIRE_BOND', 'Wire Bonding', 'Completed', 'EQP-WBD-003', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-021', 49750, 49600, 100, 50, 'RCP-WBD-BGA-001'),
('SR-QFP-MCU01-S40', 'LOT-QFP-20260601-001', 'QFP-STD', '1.0', 40, 'MOLD', 'Molding', 'Completed', 'EQP-MLD-003', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', 49600, 49450, 100, 50, 'RCP-MLD-BGA-001'),
('SR-QFP-MCU01-S50', 'LOT-QFP-20260601-001', 'QFP-STD', '1.0', 50, 'CURE', 'Post Mold Cure', 'Completed', 'EQP-OVN-001', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', 49450, 49400, 50, 0, 'RCP-OVN-CURE-001'),
('SR-QFP-MCU01-S60', 'LOT-QFP-20260601-001', 'QFP-STD', '1.0', 60, 'PLATE', 'Plating', 'Completed', 'EQP-PLT-002', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-020', 49400, 49350, 50, 0, 'RCP-PLT-BGA-001'),
('SR-QFP-MCU01-S70', 'LOT-QFP-20260601-001', 'QFP-STD', '1.0', 70, 'TRIM_FORM', 'Trim Form', 'Completed', 'EQP-TRF-002', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', 49350, 49250, 50, 50, 'RCP-TRF-SOP-001'),
('SR-QFP-MCU01-S80', 'LOT-QFP-20260601-001', 'QFP-STD', '1.0', 80, 'TEST_FT', 'Final Test', 'InProgress', 'EQP-TST-002', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-028', NULL, NULL, 49250, 0, 0, 0, 'RCP-TST-BGA-001');

-- LOT-BGA-20260601-004 (BGA SoC Auto at LASER_MARK)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-BGA-SOC04-S10', 'LOT-BGA-20260601-004', 'BGA-STD', '1.0', 10, 'GRIND', 'Wafer Grinding', 'Completed', 'EQP-GRD-001', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', 50000, 49900, 50, 50, 'RCP-GRD-STD-001'),
('SR-BGA-SOC04-S20', 'LOT-BGA-20260601-004', 'BGA-STD', '1.0', 20, 'DICE', 'Wafer Dicing', 'Completed', 'EQP-DIC-001', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', 49900, 49750, 80, 70, 'RCP-DIC-STD-001'),
('SR-BGA-SOC04-S30', 'LOT-BGA-20260601-004', 'BGA-STD', '1.0', 30, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-001', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-020', 49750, 49600, 80, 70, 'RCP-DAB-BGA-001'),
('SR-BGA-SOC04-S40', 'LOT-BGA-20260601-004', 'BGA-STD', '1.0', 40, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-001', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-022', 49600, 49550, 50, 0, 'RCP-OVN-DA-001'),
('SR-BGA-SOC04-S50', 'LOT-BGA-20260601-004', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'Wire Bonding', 'Completed', 'EQP-WBD-001', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-021', 49550, 49400, 100, 50, 'RCP-WBD-BGA-001'),
('SR-BGA-SOC04-S60', 'LOT-BGA-20260601-004', 'BGA-STD', '1.0', 60, 'MOLD', 'Molding', 'Completed', 'EQP-MLD-001', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-020', 49400, 49250, 100, 50, 'RCP-MLD-BGA-001'),
('SR-BGA-SOC04-S70', 'LOT-BGA-20260601-004', 'BGA-STD', '1.0', 70, 'CURE', 'Post Mold Cure', 'Completed', 'EQP-OVN-003', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-022', 49250, 49200, 50, 0, 'RCP-OVN-CURE-001'),
('SR-BGA-SOC04-S80', 'LOT-BGA-20260601-004', 'BGA-STD', '1.0', 80, 'LASER_MARK', 'Laser Marking', 'InProgress', 'EQP-LMK-001', DATE_SUB(@now, INTERVAL 8 HOUR), 'EMP-021', NULL, NULL, 49200, 0, 0, 0, 'RCP-LMK-BGA-001');

-- LOT-QFN-20260602-001 (QFN Sensor at WIRE_BOND)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-QFN-SEN03-S10', 'LOT-QFN-20260602-001', 'QFN-STD', '1.0', 10, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-004', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', 50000, 49800, 100, 100, 'RCP-DAB-QFN-001'),
('SR-QFN-SEN03-S20', 'LOT-QFN-20260602-001', 'QFN-STD', '1.0', 20, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-004', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-022', 49800, 49750, 50, 0, 'RCP-OVN-DA-001'),
('SR-QFN-SEN03-S30', 'LOT-QFN-20260602-001', 'QFN-STD', '1.0', 30, 'WIRE_BOND', 'Wire Bonding', 'InProgress', 'EQP-WBD-004', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-021', NULL, NULL, 49750, 0, 0, 0, 'RCP-WBD-QFN-001');

-- LOT-SOP-20260602-001 (SOP MOSFET at WIRE_BOND)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-SOP-MOS03-S10', 'LOT-SOP-20260602-001', 'SOP-STD', '1.0', 10, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-001', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', 50000, 49850, 80, 70, 'RCP-DAB-BGA-001'),
('SR-SOP-MOS03-S20', 'LOT-SOP-20260602-001', 'SOP-STD', '1.0', 20, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-001', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-022', 49850, 49800, 50, 0, 'RCP-OVN-DA-001'),
('SR-SOP-MOS03-S30', 'LOT-SOP-20260602-001', 'SOP-STD', '1.0', 30, 'WIRE_BOND', 'Wire Bonding', 'InProgress', 'EQP-WBD-005', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', NULL, NULL, 49800, 0, 0, 0, 'RCP-WBD-SOP-001');

-- LOT-BGA-20260602-004 (BGA SoC at WIRE_BOND)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-BGA-SOC05-S10', 'LOT-BGA-20260602-004', 'BGA-STD', '1.0', 10, 'GRIND', 'Wafer Grinding', 'Completed', 'EQP-GRD-002', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-020', 50000, 49900, 50, 50, 'RCP-GRD-STD-001'),
('SR-BGA-SOC05-S20', 'LOT-BGA-20260602-004', 'BGA-STD', '1.0', 20, 'DICE', 'Wafer Dicing', 'Completed', 'EQP-DIC-003', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-021', 49900, 49750, 80, 70, 'RCP-DIC-STD-001'),
('SR-BGA-SOC05-S30', 'LOT-BGA-20260602-004', 'BGA-STD', '1.0', 30, 'DIE_ATTACH', 'Die Attach', 'Completed', 'EQP-DAB-002', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-020', 49750, 49600, 80, 70, 'RCP-DAB-BGA-001'),
('SR-BGA-SOC05-S40', 'LOT-BGA-20260602-004', 'BGA-STD', '1.0', 40, 'CURE_DA', 'Die Attach Cure', 'Completed', 'EQP-OVN-002', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-022', 49600, 49550, 50, 0, 'RCP-OVN-DA-001'),
('SR-BGA-SOC05-S50', 'LOT-BGA-20260602-004', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'Wire Bonding', 'InProgress', 'EQP-WBD-002', DATE_SUB(@now, INTERVAL 18 HOUR), 'EMP-021', NULL, NULL, 49550, 0, 0, 0, 'RCP-WBD-BGA-001');

-- ============================================================================
-- 2. PROD_OPERATION_HISTORY - Operation Execution Records (~180 records)
-- ============================================================================

-- Operations for LOT-BGA-20260515-001 (Completed)
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-BGA-MCU01-001', 'LOT-BGA-20260515-001', 10, 'GRIND', 'Start', 'EQP-GRD-001', 'EMP-020', 'RCP-GRD-STD-001', DATE_SUB(@now, INTERVAL 20 DAY), 50000, NULL, NULL),
('OPH-BGA-MCU01-002', 'LOT-BGA-20260515-001', 10, 'GRIND', 'Complete', 'EQP-GRD-001', 'EMP-020', 'RCP-GRD-STD-001', DATE_SUB(@now, INTERVAL 20 DAY), 50000, 49900, '{"passQty":49850,"failQty":50,"scrapQty":50}'),
('OPH-BGA-MCU01-003', 'LOT-BGA-20260515-001', 20, 'DICE', 'Start', 'EQP-DIC-001', 'EMP-021', 'RCP-DIC-STD-001', DATE_SUB(@now, INTERVAL 19 DAY), 49900, NULL, NULL),
('OPH-BGA-MCU01-004', 'LOT-BGA-20260515-001', 20, 'DICE', 'Complete', 'EQP-DIC-001', 'EMP-021', 'RCP-DIC-STD-001', DATE_SUB(@now, INTERVAL 19 DAY), 49900, 49750, '{"passQty":49670,"failQty":80,"scrapQty":70}'),
('OPH-BGA-MCU01-005', 'LOT-BGA-20260515-001', 30, 'DIE_ATTACH', 'Start', 'EQP-DAB-001', 'EMP-020', 'RCP-DAB-BGA-001', DATE_SUB(@now, INTERVAL 19 DAY), 49750, NULL, NULL),
('OPH-BGA-MCU01-006', 'LOT-BGA-20260515-001', 30, 'DIE_ATTACH', 'Complete', 'EQP-DAB-001', 'EMP-020', 'RCP-DAB-BGA-001', DATE_SUB(@now, INTERVAL 18 DAY), 49750, 49600, '{"passQty":49520,"failQty":80,"scrapQty":70}'),
('OPH-BGA-MCU01-007', 'LOT-BGA-20260515-001', 40, 'CURE_DA', 'Start', 'EQP-OVN-001', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 18 DAY), 49600, NULL, NULL),
('OPH-BGA-MCU01-008', 'LOT-BGA-20260515-001', 40, 'CURE_DA', 'Complete', 'EQP-OVN-001', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 18 DAY), 49600, 49550, '{"passQty":49550,"failQty":50,"scrapQty":0}'),
('OPH-BGA-MCU01-009', 'LOT-BGA-20260515-001', 50, 'WIRE_BOND', 'Start', 'EQP-WBD-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 17 DAY), 49550, NULL, NULL),
('OPH-BGA-MCU01-010', 'LOT-BGA-20260515-001', 50, 'WIRE_BOND', 'Complete', 'EQP-WBD-001', 'EMP-021', 'RCP-WBD-BGA-001', DATE_SUB(@now, INTERVAL 17 DAY), 49550, 49400, '{"passQty":49300,"failQty":100,"scrapQty":50}'),
('OPH-BGA-MCU01-011', 'LOT-BGA-20260515-001', 60, 'MOLD', 'Start', 'EQP-MLD-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 16 DAY), 49400, NULL, NULL),
('OPH-BGA-MCU01-012', 'LOT-BGA-20260515-001', 60, 'MOLD', 'Complete', 'EQP-MLD-001', 'EMP-020', 'RCP-MLD-BGA-001', DATE_SUB(@now, INTERVAL 16 DAY), 49400, 49250, '{"passQty":49150,"failQty":100,"scrapQty":50}'),
('OPH-BGA-MCU01-013', 'LOT-BGA-20260515-001', 70, 'CURE', 'Start', 'EQP-OVN-002', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 15 DAY), 49250, NULL, NULL),
('OPH-BGA-MCU01-014', 'LOT-BGA-20260515-001', 70, 'CURE', 'Complete', 'EQP-OVN-002', 'EMP-022', 'RCP-OVN-CURE-001', DATE_SUB(@now, INTERVAL 15 DAY), 49250, 49200, '{"passQty":49200,"failQty":50,"scrapQty":0}'),
('OPH-BGA-MCU01-015', 'LOT-BGA-20260515-001', 80, 'LASER_MARK', 'Start', 'EQP-LMK-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 14 DAY), 49200, NULL, NULL),
('OPH-BGA-MCU01-016', 'LOT-BGA-20260515-001', 80, 'LASER_MARK', 'Complete', 'EQP-LMK-001', 'EMP-021', 'RCP-LMK-BGA-001', DATE_SUB(@now, INTERVAL 14 DAY), 49200, 49150, '{"passQty":49120,"failQty":30,"scrapQty":20}'),
('OPH-BGA-MCU01-017', 'LOT-BGA-20260515-001', 90, 'PLATE', 'Start', 'EQP-PLT-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 13 DAY), 49150, NULL, NULL),
('OPH-BGA-MCU01-018', 'LOT-BGA-20260515-001', 90, 'PLATE', 'Complete', 'EQP-PLT-001', 'EMP-020', 'RCP-PLT-BGA-001', DATE_SUB(@now, INTERVAL 13 DAY), 49150, 49100, '{"passQty":49100,"failQty":50,"scrapQty":0}'),
('OPH-BGA-MCU01-019', 'LOT-BGA-20260515-001', 100, 'SINGULATE', 'Start', 'EQP-TRF-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 12 DAY), 49100, NULL, NULL),
('OPH-BGA-MCU01-020', 'LOT-BGA-20260515-001', 100, 'SINGULATE', 'Complete', 'EQP-TRF-001', 'EMP-021', 'RCP-TRF-BGA-001', DATE_SUB(@now, INTERVAL 12 DAY), 49100, 49000, '{"passQty":48950,"failQty":50,"scrapQty":50}'),
('OPH-BGA-MCU01-021', 'LOT-BGA-20260515-001', 110, 'TEST_FT', 'Start', 'EQP-TST-001', 'EMP-028', NULL, DATE_SUB(@now, INTERVAL 11 DAY), 49000, NULL, NULL),
('OPH-BGA-MCU01-022', 'LOT-BGA-20260515-001', 110, 'TEST_FT', 'Complete', 'EQP-TST-001', 'EMP-028', 'RCP-TST-BGA-001', DATE_SUB(@now, INTERVAL 11 DAY), 49000, 48500, '{"passQty":48500,"failQty":400,"scrapQty":100}'),
('OPH-BGA-MCU01-023', 'LOT-BGA-20260515-001', 120, 'FINAL_PACK', 'Start', 'EQP-PCK-001', 'EMP-029', NULL, DATE_SUB(@now, INTERVAL 10 DAY), 48500, NULL, NULL),
('OPH-BGA-MCU01-024', 'LOT-BGA-20260515-001', 120, 'FINAL_PACK', 'Complete', 'EQP-PCK-001', 'EMP-029', NULL, DATE_SUB(@now, INTERVAL 10 DAY), 48500, 48400, '{"passQty":48400,"failQty":50,"scrapQty":50}');

-- Operations for LOT-BGA-20260515-002 (Completed)
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-BGA-MCU02-001', 'LOT-BGA-20260515-002', 10, 'GRIND', 'Start', 'EQP-GRD-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 19 DAY), 50000, NULL, NULL),
('OPH-BGA-MCU02-002', 'LOT-BGA-20260515-002', 10, 'GRIND', 'Complete', 'EQP-GRD-002', 'EMP-020', 'RCP-GRD-STD-001', DATE_SUB(@now, INTERVAL 19 DAY), 50000, 49850, '{"passQty":49850,"failQty":80,"scrapQty":70}'),
('OPH-BGA-MCU02-003', 'LOT-BGA-20260515-002', 20, 'DICE', 'Start', 'EQP-DIC-002', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 18 DAY), 49850, NULL, NULL),
('OPH-BGA-MCU02-004', 'LOT-BGA-20260515-002', 20, 'DICE', 'Complete', 'EQP-DIC-002', 'EMP-021', 'RCP-DIC-STD-001', DATE_SUB(@now, INTERVAL 18 DAY), 49850, 49700, '{"passQty":49620,"failQty":80,"scrapQty":70}'),
('OPH-BGA-MCU02-005', 'LOT-BGA-20260515-002', 30, 'DIE_ATTACH', 'Start', 'EQP-DAB-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 18 DAY), 49700, NULL, NULL),
('OPH-BGA-MCU02-006', 'LOT-BGA-20260515-002', 30, 'DIE_ATTACH', 'Complete', 'EQP-DAB-002', 'EMP-020', 'RCP-DAB-BGA-001', DATE_SUB(@now, INTERVAL 18 DAY), 49700, 49550, '{"passQty":49470,"failQty":80,"scrapQty":70}'),
('OPH-BGA-MCU02-007', 'LOT-BGA-20260515-002', 40, 'CURE_DA', 'Start', 'EQP-OVN-003', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 17 DAY), 49550, NULL, NULL),
('OPH-BGA-MCU02-008', 'LOT-BGA-20260515-002', 40, 'CURE_DA', 'Complete', 'EQP-OVN-003', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 17 DAY), 49550, 49500, '{"passQty":49500,"failQty":50,"scrapQty":0}'),
('OPH-BGA-MCU02-009', 'LOT-BGA-20260515-002', 50, 'WIRE_BOND', 'Start', 'EQP-WBD-002', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 16 DAY), 49500, NULL, NULL),
('OPH-BGA-MCU02-010', 'LOT-BGA-20260515-002', 50, 'WIRE_BOND', 'Complete', 'EQP-WBD-002', 'EMP-021', 'RCP-WBD-BGA-001', DATE_SUB(@now, INTERVAL 16 DAY), 49500, 49350, '{"passQty":49250,"failQty":100,"scrapQty":50}'),
('OPH-BGA-MCU02-011', 'LOT-BGA-20260515-002', 60, 'MOLD', 'Start', 'EQP-MLD-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 15 DAY), 49350, NULL, NULL),
('OPH-BGA-MCU02-012', 'LOT-BGA-20260515-002', 60, 'MOLD', 'Complete', 'EQP-MLD-002', 'EMP-020', 'RCP-MLD-BGA-001', DATE_SUB(@now, INTERVAL 15 DAY), 49350, 49200, '{"passQty":49100,"failQty":100,"scrapQty":50}'),
('OPH-BGA-MCU02-013', 'LOT-BGA-20260515-002', 70, 'CURE', 'Start', 'EQP-OVN-004', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 14 DAY), 49200, NULL, NULL),
('OPH-BGA-MCU02-014', 'LOT-BGA-20260515-002', 70, 'CURE', 'Complete', 'EQP-OVN-004', 'EMP-022', 'RCP-OVN-CURE-001', DATE_SUB(@now, INTERVAL 14 DAY), 49200, 49150, '{"passQty":49150,"failQty":50,"scrapQty":0}'),
('OPH-BGA-MCU02-015', 'LOT-BGA-20260515-002', 80, 'LASER_MARK', 'Start', 'EQP-LMK-002', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 13 DAY), 49150, NULL, NULL),
('OPH-BGA-MCU02-016', 'LOT-BGA-20260515-002', 80, 'LASER_MARK', 'Complete', 'EQP-LMK-002', 'EMP-021', 'RCP-LMK-BGA-001', DATE_SUB(@now, INTERVAL 13 DAY), 49150, 49100, '{"passQty":49070,"failQty":30,"scrapQty":20}'),
('OPH-BGA-MCU02-017', 'LOT-BGA-20260515-002', 90, 'PLATE', 'Start', 'EQP-PLT-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 12 DAY), 49100, NULL, NULL),
('OPH-BGA-MCU02-018', 'LOT-BGA-20260515-002', 90, 'PLATE', 'Complete', 'EQP-PLT-002', 'EMP-020', 'RCP-PLT-BGA-001', DATE_SUB(@now, INTERVAL 12 DAY), 49100, 49050, '{"passQty":49050,"failQty":50,"scrapQty":0}'),
('OPH-BGA-MCU02-019', 'LOT-BGA-20260515-002', 100, 'SINGULATE', 'Start', 'EQP-TRF-002', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 11 DAY), 49050, NULL, NULL),
('OPH-BGA-MCU02-020', 'LOT-BGA-20260515-002', 100, 'SINGULATE', 'Complete', 'EQP-TRF-002', 'EMP-021', 'RCP-TRF-BGA-001', DATE_SUB(@now, INTERVAL 11 DAY), 49050, 48950, '{"passQty":48900,"failQty":50,"scrapQty":50}'),
('OPH-BGA-MCU02-021', 'LOT-BGA-20260515-002', 110, 'TEST_FT', 'Start', 'EQP-TST-002', 'EMP-028', NULL, DATE_SUB(@now, INTERVAL 10 DAY), 48950, NULL, NULL),
('OPH-BGA-MCU02-022', 'LOT-BGA-20260515-002', 110, 'TEST_FT', 'Complete', 'EQP-TST-002', 'EMP-028', 'RCP-TST-BGA-001', DATE_SUB(@now, INTERVAL 10 DAY), 48950, 48450, '{"passQty":48450,"failQty":400,"scrapQty":100}'),
('OPH-BGA-MCU02-023', 'LOT-BGA-20260515-002', 120, 'FINAL_PACK', 'Start', 'EQP-PCK-002', 'EMP-029', NULL, DATE_SUB(@now, INTERVAL 9 DAY), 48450, NULL, NULL),
('OPH-BGA-MCU02-024', 'LOT-BGA-20260515-002', 120, 'FINAL_PACK', 'Complete', 'EQP-PCK-002', 'EMP-029', NULL, DATE_SUB(@now, INTERVAL 9 DAY), 48450, 48350, '{"passQty":48350,"failQty":50,"scrapQty":50}');

-- Operations for LOT-QFN-20260601-002
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-QFN-SEN02-001', 'LOT-QFN-20260601-002', 10, 'DIE_ATTACH', 'Start', 'EQP-DAB-003', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 7 DAY), 50000, NULL, NULL),
('OPH-QFN-SEN02-002', 'LOT-QFN-20260601-002', 10, 'DIE_ATTACH', 'Complete', 'EQP-DAB-003', 'EMP-020', 'RCP-DAB-QFN-001', DATE_SUB(@now, INTERVAL 7 DAY), 50000, 49850, '{"passQty":49770,"failQty":80,"scrapQty":70}'),
('OPH-QFN-SEN02-003', 'LOT-QFN-20260601-002', 20, 'CURE_DA', 'Start', 'EQP-OVN-004', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 6 DAY), 49850, NULL, NULL),
('OPH-QFN-SEN02-004', 'LOT-QFN-20260601-002', 20, 'CURE_DA', 'Complete', 'EQP-OVN-004', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 6 DAY), 49850, 49750, '{"passQty":49750,"failQty":100,"scrapQty":0}'),
('OPH-QFN-SEN02-005', 'LOT-QFN-20260601-002', 30, 'WIRE_BOND', 'Start', 'EQP-WBD-003', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 6 DAY), 49750, NULL, NULL),
('OPH-QFN-SEN02-006', 'LOT-QFN-20260601-002', 30, 'WIRE_BOND', 'Complete', 'EQP-WBD-003', 'EMP-021', 'RCP-WBD-QFN-001', DATE_SUB(@now, INTERVAL 6 DAY), 49750, 49600, '{"passQty":49500,"failQty":100,"scrapQty":50}'),
('OPH-QFN-SEN02-007', 'LOT-QFN-20260601-002', 40, 'MOLD', 'Start', 'EQP-MLD-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 49600, NULL, NULL),
('OPH-QFN-SEN02-008', 'LOT-QFN-20260601-002', 40, 'MOLD', 'Complete', 'EQP-MLD-002', 'EMP-020', 'RCP-MLD-QFN-001', DATE_SUB(@now, INTERVAL 5 DAY), 49600, 49450, '{"passQty":49350,"failQty":100,"scrapQty":50}'),
('OPH-QFN-SEN02-009', 'LOT-QFN-20260601-002', 50, 'CURE', 'Start', 'EQP-OVN-002', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49450, NULL, NULL),
('OPH-QFN-SEN02-010', 'LOT-QFN-20260601-002', 50, 'CURE', 'Complete', 'EQP-OVN-002', 'EMP-022', 'RCP-OVN-CURE-001', DATE_SUB(@now, INTERVAL 4 DAY), 49450, 49400, '{"passQty":49400,"failQty":50,"scrapQty":0}'),
('OPH-QFN-SEN02-011', 'LOT-QFN-20260601-002', 60, 'PLATE', 'Start', 'EQP-PLT-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49400, NULL, NULL),
('OPH-QFN-SEN02-012', 'LOT-QFN-20260601-002', 60, 'PLATE', 'Complete', 'EQP-PLT-002', 'EMP-020', 'RCP-PLT-QFN-001', DATE_SUB(@now, INTERVAL 4 DAY), 49400, 49300, '{"passQty":49300,"failQty":100,"scrapQty":0}'),
('OPH-QFN-SEN02-013', 'LOT-QFN-20260601-002', 70, 'SINGULATE', 'Start', 'EQP-TRF-002', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49300, NULL, NULL),
('OPH-QFN-SEN02-014', 'LOT-QFN-20260601-002', 70, 'SINGULATE', 'Complete', 'EQP-TRF-002', 'EMP-021', 'RCP-TRF-BGA-001', DATE_SUB(@now, INTERVAL 3 DAY), 49300, 49200, '{"passQty":49100,"failQty":100,"scrapQty":0}'),
('OPH-QFN-SEN02-015', 'LOT-QFN-20260601-002', 80, 'TEST_FT', 'Start', 'EQP-TST-002', 'EMP-028', NULL, DATE_SUB(@now, INTERVAL 2 DAY), 49200, NULL, NULL);

-- Operations for LOT-SOP-20260601-002
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-SOP-MOS02-001', 'LOT-SOP-20260601-002', 10, 'DIE_ATTACH', 'Start', 'EQP-DAB-004', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 9 DAY), 50000, NULL, NULL),
('OPH-SOP-MOS02-002', 'LOT-SOP-20260601-002', 10, 'DIE_ATTACH', 'Complete', 'EQP-DAB-004', 'EMP-020', 'RCP-DAB-BGA-001', DATE_SUB(@now, INTERVAL 9 DAY), 50000, 49800, '{"passQty":49700,"failQty":100,"scrapQty":100}'),
('OPH-SOP-MOS02-003', 'LOT-SOP-20260601-002', 20, 'CURE_DA', 'Start', 'EQP-OVN-003', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 8 DAY), 49800, NULL, NULL),
('OPH-SOP-MOS02-004', 'LOT-SOP-20260601-002', 20, 'CURE_DA', 'Complete', 'EQP-OVN-003', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 8 DAY), 49800, 49700, '{"passQty":49700,"failQty":100,"scrapQty":0}'),
('OPH-SOP-MOS02-005', 'LOT-SOP-20260601-002', 30, 'WIRE_BOND', 'Start', 'EQP-WBD-004', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 8 DAY), 49700, NULL, NULL),
('OPH-SOP-MOS02-006', 'LOT-SOP-20260601-002', 30, 'WIRE_BOND', 'Complete', 'EQP-WBD-004', 'EMP-021', 'RCP-WBD-SOP-001', DATE_SUB(@now, INTERVAL 8 DAY), 49700, 49550, '{"passQty":49450,"failQty":100,"scrapQty":50}'),
('OPH-SOP-MOS02-007', 'LOT-SOP-20260601-002', 40, 'MOLD', 'Start', 'EQP-MLD-003', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 7 DAY), 49550, NULL, NULL),
('OPH-SOP-MOS02-008', 'LOT-SOP-20260601-002', 40, 'MOLD', 'Complete', 'EQP-MLD-003', 'EMP-020', 'RCP-MLD-QFN-001', DATE_SUB(@now, INTERVAL 7 DAY), 49550, 49400, '{"passQty":49300,"failQty":100,"scrapQty":50}'),
('OPH-SOP-MOS02-009', 'LOT-SOP-20260601-002', 50, 'CURE', 'Start', 'EQP-OVN-004', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 6 DAY), 49400, NULL, NULL),
('OPH-SOP-MOS02-010', 'LOT-SOP-20260601-002', 50, 'CURE', 'Complete', 'EQP-OVN-004', 'EMP-022', 'RCP-OVN-CURE-001', DATE_SUB(@now, INTERVAL 6 DAY), 49400, 49350, '{"passQty":49350,"failQty":50,"scrapQty":0}'),
('OPH-SOP-MOS02-011', 'LOT-SOP-20260601-002', 60, 'PLATE', 'Start', 'EQP-PLT-003', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 49350, NULL, NULL);

-- Operations for LOT-DFN-20260601-001
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-DFN-LDO01-001', 'LOT-DFN-20260601-001', 10, 'DIE_ATTACH', 'Start', 'EQP-DAB-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 8 DAY), 50000, NULL, NULL),
('OPH-DFN-LDO01-002', 'LOT-DFN-20260601-001', 10, 'DIE_ATTACH', 'Complete', 'EQP-DAB-001', 'EMP-020', 'RCP-DAB-DFN-001', DATE_SUB(@now, INTERVAL 8 DAY), 50000, 49850, '{"passQty":49770,"failQty":80,"scrapQty":70}'),
('OPH-DFN-LDO01-003', 'LOT-DFN-20260601-001', 20, 'CURE_DA', 'Start', 'EQP-OVN-001', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 7 DAY), 49850, NULL, NULL),
('OPH-DFN-LDO01-004', 'LOT-DFN-20260601-001', 20, 'CURE_DA', 'Complete', 'EQP-OVN-001', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 7 DAY), 49850, 49800, '{"passQty":49800,"failQty":50,"scrapQty":0}'),
('OPH-DFN-LDO01-005', 'LOT-DFN-20260601-001', 30, 'WIRE_BOND', 'Start', 'EQP-WBD-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 7 DAY), 49800, NULL, NULL),
('OPH-DFN-LDO01-006', 'LOT-DFN-20260601-001', 30, 'WIRE_BOND', 'Complete', 'EQP-WBD-001', 'EMP-021', 'RCP-WBD-QFN-001', DATE_SUB(@now, INTERVAL 7 DAY), 49800, 49650, '{"passQty":49550,"failQty":100,"scrapQty":50}'),
('OPH-DFN-LDO01-007', 'LOT-DFN-20260601-001', 40, 'MOLD', 'Start', 'EQP-MLD-003', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 6 DAY), 49650, NULL, NULL),
('OPH-DFN-LDO01-008', 'LOT-DFN-20260601-001', 40, 'MOLD', 'Complete', 'EQP-MLD-003', 'EMP-020', 'RCP-MLD-QFN-001', DATE_SUB(@now, INTERVAL 6 DAY), 49650, 49500, '{"passQty":49400,"failQty":100,"scrapQty":50}'),
('OPH-DFN-LDO01-009', 'LOT-DFN-20260601-001', 50, 'CURE', 'Start', 'EQP-OVN-003', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 49500, NULL, NULL),
('OPH-DFN-LDO01-010', 'LOT-DFN-20260601-001', 50, 'CURE', 'Complete', 'EQP-OVN-003', 'EMP-022', 'RCP-OVN-CURE-001', DATE_SUB(@now, INTERVAL 5 DAY), 49500, 49450, '{"passQty":49450,"failQty":50,"scrapQty":0}'),
('OPH-DFN-LDO01-011', 'LOT-DFN-20260601-001', 60, 'PLATE', 'Start', 'EQP-PLT-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 49450, NULL, NULL),
('OPH-DFN-LDO01-012', 'LOT-DFN-20260601-001', 60, 'PLATE', 'Complete', 'EQP-PLT-001', 'EMP-020', 'RCP-PLT-QFN-001', DATE_SUB(@now, INTERVAL 5 DAY), 49450, 49400, '{"passQty":49400,"failQty":50,"scrapQty":0}'),
('OPH-DFN-LDO01-013', 'LOT-DFN-20260601-001', 70, 'SINGULATE', 'Start', 'EQP-TRF-003', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49400, NULL, NULL),
('OPH-DFN-LDO01-014', 'LOT-DFN-20260601-001', 70, 'SINGULATE', 'Complete', 'EQP-TRF-003', 'EMP-021', 'RCP-TRF-BGA-001', DATE_SUB(@now, INTERVAL 4 DAY), 49400, 49350, '{"passQty":49300,"failQty":50,"scrapQty":0}'),
('OPH-DFN-LDO01-015', 'LOT-DFN-20260601-001', 80, 'TEST_FT', 'Start', 'EQP-TST-003', 'EMP-028', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49350, NULL, NULL);

-- Operations for LOT-DFN-20260601-002
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-DFN-LDO02-001', 'LOT-DFN-20260601-002', 10, 'DIE_ATTACH', 'Start', 'EQP-DAB-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 8 DAY), 50000, NULL, NULL),
('OPH-DFN-LDO02-002', 'LOT-DFN-20260601-002', 10, 'DIE_ATTACH', 'Complete', 'EQP-DAB-002', 'EMP-020', 'RCP-DAB-DFN-001', DATE_SUB(@now, INTERVAL 8 DAY), 50000, 49800, '{"passQty":49700,"failQty":100,"scrapQty":100}'),
('OPH-DFN-LDO02-003', 'LOT-DFN-20260601-002', 20, 'CURE_DA', 'Start', 'EQP-OVN-002', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 7 DAY), 49800, NULL, NULL),
('OPH-DFN-LDO02-004', 'LOT-DFN-20260601-002', 20, 'CURE_DA', 'Complete', 'EQP-OVN-002', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 7 DAY), 49800, 49750, '{"passQty":49750,"failQty":50,"scrapQty":0}'),
('OPH-DFN-LDO02-005', 'LOT-DFN-20260601-002', 30, 'WIRE_BOND', 'Start', 'EQP-WBD-002', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 7 DAY), 49750, NULL, NULL),
('OPH-DFN-LDO02-006', 'LOT-DFN-20260601-002', 30, 'WIRE_BOND', 'Complete', 'EQP-WBD-002', 'EMP-021', 'RCP-WBD-QFN-001', DATE_SUB(@now, INTERVAL 7 DAY), 49750, 49600, '{"passQty":49500,"failQty":100,"scrapQty":50}'),
('OPH-DFN-LDO02-007', 'LOT-DFN-20260601-002', 40, 'MOLD', 'Start', 'EQP-MLD-004', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 6 DAY), 49600, NULL, NULL),
('OPH-DFN-LDO02-008', 'LOT-DFN-20260601-002', 40, 'MOLD', 'Complete', 'EQP-MLD-004', 'EMP-020', 'RCP-MLD-QFN-001', DATE_SUB(@now, INTERVAL 6 DAY), 49600, 49500, '{"passQty":49500,"failQty":100,"scrapQty":0}'),
('OPH-DFN-LDO02-009', 'LOT-DFN-20260601-002', 50, 'CURE', 'Start', 'EQP-OVN-004', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 49500, NULL, NULL),
('OPH-DFN-LDO02-010', 'LOT-DFN-20260601-002', 50, 'CURE', 'Complete', 'EQP-OVN-004', 'EMP-022', 'RCP-OVN-CURE-001', DATE_SUB(@now, INTERVAL 5 DAY), 49500, 49450, '{"passQty":49450,"failQty":50,"scrapQty":0}'),
('OPH-DFN-LDO02-011', 'LOT-DFN-20260601-002', 60, 'PLATE', 'Start', 'EQP-PLT-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 49450, NULL, NULL),
('OPH-DFN-LDO02-012', 'LOT-DFN-20260601-002', 60, 'PLATE', 'Complete', 'EQP-PLT-002', 'EMP-020', 'RCP-PLT-QFN-001', DATE_SUB(@now, INTERVAL 5 DAY), 49450, 49400, '{"passQty":49400,"failQty":50,"scrapQty":0}'),
('OPH-DFN-LDO02-013', 'LOT-DFN-20260601-002', 70, 'SINGULATE', 'Start', 'EQP-TRF-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49400, NULL, NULL),
('OPH-DFN-LDO02-014', 'LOT-DFN-20260601-002', 70, 'SINGULATE', 'Complete', 'EQP-TRF-001', 'EMP-021', 'RCP-TRF-BGA-001', DATE_SUB(@now, INTERVAL 4 DAY), 49400, 49300, '{"passQty":49200,"failQty":100,"scrapQty":0}'),
('OPH-DFN-LDO02-015', 'LOT-DFN-20260601-002', 80, 'TEST_FT', 'Start', 'EQP-TST-004', 'EMP-028', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49300, NULL, NULL);

-- Operations for LOT-BGA-20260601-003
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-BGA-WIFI03-001', 'LOT-BGA-20260601-003', 10, 'GRIND', 'Start', 'EQP-GRD-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 6 DAY), 50000, NULL, NULL),
('OPH-BGA-WIFI03-002', 'LOT-BGA-20260601-003', 10, 'GRIND', 'Complete', 'EQP-GRD-001', 'EMP-020', 'RCP-GRD-STD-001', DATE_SUB(@now, INTERVAL 6 DAY), 50000, 49850, '{"passQty":49850,"failQty":80,"scrapQty":70}'),
('OPH-BGA-WIFI03-003', 'LOT-BGA-20260601-003', 20, 'DICE', 'Start', 'EQP-DIC-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 49850, NULL, NULL),
('OPH-BGA-WIFI03-004', 'LOT-BGA-20260601-003', 20, 'DICE', 'Complete', 'EQP-DIC-001', 'EMP-021', 'RCP-DIC-STD-001', DATE_SUB(@now, INTERVAL 5 DAY), 49850, 49700, '{"passQty":49620,"failQty":80,"scrapQty":70}'),
('OPH-BGA-WIFI03-005', 'LOT-BGA-20260601-003', 30, 'DIE_ATTACH', 'Start', 'EQP-DAB-003', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 49700, NULL, NULL),
('OPH-BGA-WIFI03-006', 'LOT-BGA-20260601-003', 30, 'DIE_ATTACH', 'Complete', 'EQP-DAB-003', 'EMP-020', 'RCP-DAB-BGA-001', DATE_SUB(@now, INTERVAL 5 DAY), 49700, 49550, '{"passQty":49470,"failQty":80,"scrapQty":70}'),
('OPH-BGA-WIFI03-007', 'LOT-BGA-20260601-003', 40, 'CURE_DA', 'Start', 'EQP-OVN-003', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49550, NULL, NULL),
('OPH-BGA-WIFI03-008', 'LOT-BGA-20260601-003', 40, 'CURE_DA', 'Complete', 'EQP-OVN-003', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 4 DAY), 49550, 49500, '{"passQty":49500,"failQty":50,"scrapQty":0}'),
('OPH-BGA-WIFI03-009', 'LOT-BGA-20260601-003', 50, 'WIRE_BOND', 'Start', 'EQP-WBD-003', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49500, NULL, NULL),
('OPH-BGA-WIFI03-010', 'LOT-BGA-20260601-003', 50, 'WIRE_BOND', 'Complete', 'EQP-WBD-003', 'EMP-021', 'RCP-WBD-BGA-001', DATE_SUB(@now, INTERVAL 4 DAY), 49500, 49350, '{"passQty":49250,"failQty":100,"scrapQty":50}'),
('OPH-BGA-WIFI03-011', 'LOT-BGA-20260601-003', 60, 'MOLD', 'Start', 'EQP-MLD-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49350, NULL, NULL),
('OPH-BGA-WIFI03-012', 'LOT-BGA-20260601-003', 60, 'MOLD', 'Complete', 'EQP-MLD-001', 'EMP-020', 'RCP-MLD-BGA-001', DATE_SUB(@now, INTERVAL 3 DAY), 49350, 49200, '{"passQty":49100,"failQty":100,"scrapQty":50}'),
('OPH-BGA-WIFI03-013', 'LOT-BGA-20260601-003', 70, 'CURE', 'Start', 'EQP-OVN-001', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 2 DAY), 49200, NULL, NULL),
('OPH-BGA-WIFI03-014', 'LOT-BGA-20260601-003', 70, 'CURE', 'Complete', 'EQP-OVN-001', 'EMP-022', 'RCP-OVN-CURE-001', DATE_SUB(@now, INTERVAL 2 DAY), 49200, 49150, '{"passQty":49150,"failQty":50,"scrapQty":0}'),
('OPH-BGA-WIFI03-015', 'LOT-BGA-20260601-003', 80, 'LASER_MARK', 'Start', 'EQP-LMK-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 2 DAY), 49150, NULL, NULL),
('OPH-BGA-WIFI03-016', 'LOT-BGA-20260601-003', 80, 'LASER_MARK', 'Complete', 'EQP-LMK-001', 'EMP-021', 'RCP-LMK-BGA-001', DATE_SUB(@now, INTERVAL 2 DAY), 49150, 49100, '{"passQty":49070,"failQty":30,"scrapQty":20}'),
('OPH-BGA-WIFI03-017', 'LOT-BGA-20260601-003', 90, 'PLATE', 'Start', 'EQP-PLT-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 1 DAY), 49100, NULL, NULL),
('OPH-BGA-WIFI03-018', 'LOT-BGA-20260601-003', 90, 'PLATE', 'Complete', 'EQP-PLT-001', 'EMP-020', 'RCP-PLT-BGA-001', DATE_SUB(@now, INTERVAL 1 DAY), 49100, 49050, '{"passQty":49050,"failQty":50,"scrapQty":0}'),
('OPH-BGA-WIFI03-019', 'LOT-BGA-20260601-003', 100, 'SINGULATE', 'Start', 'EQP-TRF-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 1 DAY), 49050, NULL, NULL),
('OPH-BGA-WIFI03-020', 'LOT-BGA-20260601-003', 100, 'SINGULATE', 'Complete', 'EQP-TRF-001', 'EMP-021', 'RCP-TRF-BGA-001', DATE_SUB(@now, INTERVAL 1 DAY), 49050, 48950, '{"passQty":48900,"failQty":50,"scrapQty":50}'),
('OPH-BGA-WIFI03-021', 'LOT-BGA-20260601-003', 110, 'TEST_FT', 'Start', 'EQP-TST-001', 'EMP-028', NULL, DATE_SUB(@now, INTERVAL 12 HOUR), 48950, NULL, NULL);

-- Operations for LOT-QFN-20260601-003
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-QFN-TEMP03-001', 'LOT-QFN-20260601-003', 10, 'DIE_ATTACH', 'Start', 'EQP-DAB-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 50000, NULL, NULL),
('OPH-QFN-TEMP03-002', 'LOT-QFN-20260601-003', 10, 'DIE_ATTACH', 'Complete', 'EQP-DAB-001', 'EMP-020', 'RCP-DAB-QFN-001', DATE_SUB(@now, INTERVAL 4 DAY), 50000, 49850, '{"passQty":49770,"failQty":80,"scrapQty":70}'),
('OPH-QFN-TEMP03-003', 'LOT-QFN-20260601-003', 20, 'CURE_DA', 'Start', 'EQP-OVN-001', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49850, NULL, NULL),
('OPH-QFN-TEMP03-004', 'LOT-QFN-20260601-003', 20, 'CURE_DA', 'Complete', 'EQP-OVN-001', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 3 DAY), 49850, 49800, '{"passQty":49800,"failQty":50,"scrapQty":0}'),
('OPH-QFN-TEMP03-005', 'LOT-QFN-20260601-003', 30, 'WIRE_BOND', 'Start', 'EQP-WBD-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49800, NULL, NULL),
('OPH-QFN-TEMP03-006', 'LOT-QFN-20260601-003', 30, 'WIRE_BOND', 'Complete', 'EQP-WBD-001', 'EMP-021', 'RCP-WBD-QFN-001', DATE_SUB(@now, INTERVAL 3 DAY), 49800, 49650, '{"passQty":49550,"failQty":100,"scrapQty":50}'),
('OPH-QFN-TEMP03-007', 'LOT-QFN-20260601-003', 40, 'MOLD', 'Start', 'EQP-MLD-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 2 DAY), 49650, NULL, NULL),
('OPH-QFN-TEMP03-008', 'LOT-QFN-20260601-003', 40, 'MOLD', 'Complete', 'EQP-MLD-001', 'EMP-020', 'RCP-MLD-QFN-001', DATE_SUB(@now, INTERVAL 2 DAY), 49650, 49500, '{"passQty":49400,"failQty":100,"scrapQty":50}'),
('OPH-QFN-TEMP03-009', 'LOT-QFN-20260601-003', 50, 'CURE', 'Start', 'EQP-OVN-002', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 1 DAY), 49500, NULL, NULL),
('OPH-QFN-TEMP03-010', 'LOT-QFN-20260601-003', 50, 'CURE', 'Complete', 'EQP-OVN-002', 'EMP-022', 'RCP-OVN-CURE-001', DATE_SUB(@now, INTERVAL 1 DAY), 49500, 49450, '{"passQty":49450,"failQty":50,"scrapQty":0}'),
('OPH-QFN-TEMP03-011', 'LOT-QFN-20260601-003', 60, 'PLATE', 'Start', 'EQP-PLT-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 1 DAY), 49450, NULL, NULL),
('OPH-QFN-TEMP03-012', 'LOT-QFN-20260601-003', 60, 'PLATE', 'Complete', 'EQP-PLT-001', 'EMP-020', 'RCP-PLT-QFN-001', DATE_SUB(@now, INTERVAL 1 DAY), 49450, 49400, '{"passQty":49400,"failQty":50,"scrapQty":0}'),
('OPH-QFN-TEMP03-013', 'LOT-QFN-20260601-003', 70, 'SINGULATE', 'Start', 'EQP-TRF-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 12 HOUR), 49400, NULL, NULL);

-- Operations for LOT-BGA-20260602-003
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-BGA-WIFI04-001', 'LOT-BGA-20260602-003', 10, 'GRIND', 'Start', 'EQP-GRD-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 50000, NULL, NULL),
('OPH-BGA-WIFI04-002', 'LOT-BGA-20260602-003', 10, 'GRIND', 'Complete', 'EQP-GRD-002', 'EMP-020', 'RCP-GRD-STD-001', DATE_SUB(@now, INTERVAL 5 DAY), 50000, 49900, '{"passQty":49900,"failQty":50,"scrapQty":50}'),
('OPH-BGA-WIFI04-003', 'LOT-BGA-20260602-003', 20, 'DICE', 'Start', 'EQP-DIC-002', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49900, NULL, NULL),
('OPH-BGA-WIFI04-004', 'LOT-BGA-20260602-003', 20, 'DICE', 'Complete', 'EQP-DIC-002', 'EMP-021', 'RCP-DIC-STD-001', DATE_SUB(@now, INTERVAL 4 DAY), 49900, 49750, '{"passQty":49670,"failQty":80,"scrapQty":70}'),
('OPH-BGA-WIFI04-005', 'LOT-BGA-20260602-003', 30, 'DIE_ATTACH', 'Start', 'EQP-DAB-004', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49750, NULL, NULL),
('OPH-BGA-WIFI04-006', 'LOT-BGA-20260602-003', 30, 'DIE_ATTACH', 'Complete', 'EQP-DAB-004', 'EMP-020', 'RCP-DAB-BGA-001', DATE_SUB(@now, INTERVAL 4 DAY), 49750, 49600, '{"passQty":49520,"failQty":80,"scrapQty":70}'),
('OPH-BGA-WIFI04-007', 'LOT-BGA-20260602-003', 40, 'CURE_DA', 'Start', 'EQP-OVN-004', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49600, NULL, NULL),
('OPH-BGA-WIFI04-008', 'LOT-BGA-20260602-003', 40, 'CURE_DA', 'Complete', 'EQP-OVN-004', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 3 DAY), 49600, 49550, '{"passQty":49550,"failQty":50,"scrapQty":0}'),
('OPH-BGA-WIFI04-009', 'LOT-BGA-20260602-003', 50, 'WIRE_BOND', 'Start', 'EQP-WBD-004', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49550, NULL, NULL),
('OPH-BGA-WIFI04-010', 'LOT-BGA-20260602-003', 50, 'WIRE_BOND', 'Complete', 'EQP-WBD-004', 'EMP-021', 'RCP-WBD-BGA-001', DATE_SUB(@now, INTERVAL 3 DAY), 49550, 49400, '{"passQty":49300,"failQty":100,"scrapQty":50}'),
('OPH-BGA-WIFI04-011', 'LOT-BGA-20260602-003', 60, 'MOLD', 'Start', 'EQP-MLD-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 2 DAY), 49400, NULL, NULL),
('OPH-BGA-WIFI04-012', 'LOT-BGA-20260602-003', 60, 'MOLD', 'Complete', 'EQP-MLD-002', 'EMP-020', 'RCP-MLD-BGA-001', DATE_SUB(@now, INTERVAL 2 DAY), 49400, 49250, '{"passQty":49150,"failQty":100,"scrapQty":50}'),
('OPH-BGA-WIFI04-013', 'LOT-BGA-20260602-003', 70, 'CURE', 'Start', 'EQP-OVN-002', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 1 DAY), 49250, NULL, NULL),
('OPH-BGA-WIFI04-014', 'LOT-BGA-20260602-003', 70, 'CURE', 'Complete', 'EQP-OVN-002', 'EMP-022', 'RCP-OVN-CURE-001', DATE_SUB(@now, INTERVAL 1 DAY), 49250, 49200, '{"passQty":49200,"failQty":50,"scrapQty":0}'),
('OPH-BGA-WIFI04-015', 'LOT-BGA-20260602-003', 80, 'LASER_MARK', 'Start', 'EQP-LMK-002', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 1 DAY), 49200, NULL, NULL),
('OPH-BGA-WIFI04-016', 'LOT-BGA-20260602-003', 80, 'LASER_MARK', 'Complete', 'EQP-LMK-002', 'EMP-021', 'RCP-LMK-BGA-001', DATE_SUB(@now, INTERVAL 1 DAY), 49200, 49150, '{"passQty":49120,"failQty":30,"scrapQty":20}'),
('OPH-BGA-WIFI04-017', 'LOT-BGA-20260602-003', 90, 'PLATE', 'Start', 'EQP-PLT-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 8 HOUR), 49150, NULL, NULL);

-- Operations for LOT-QFP-20260601-001
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-QFP-MCU01-001', 'LOT-QFP-20260601-001', 10, 'DIE_ATTACH', 'Start', 'EQP-DAB-003', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 7 DAY), 50000, NULL, NULL),
('OPH-QFP-MCU01-002', 'LOT-QFP-20260601-001', 10, 'DIE_ATTACH', 'Complete', 'EQP-DAB-003', 'EMP-020', 'RCP-DAB-QFP-001', DATE_SUB(@now, INTERVAL 7 DAY), 50000, 49800, '{"passQty":49700,"failQty":100,"scrapQty":100}'),
('OPH-QFP-MCU01-003', 'LOT-QFP-20260601-001', 20, 'CURE_DA', 'Start', 'EQP-OVN-003', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 6 DAY), 49800, NULL, NULL),
('OPH-QFP-MCU01-004', 'LOT-QFP-20260601-001', 20, 'CURE_DA', 'Complete', 'EQP-OVN-003', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 6 DAY), 49800, 49750, '{"passQty":49750,"failQty":50,"scrapQty":0}'),
('OPH-QFP-MCU01-005', 'LOT-QFP-20260601-001', 30, 'WIRE_BOND', 'Start', 'EQP-WBD-003', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 6 DAY), 49750, NULL, NULL),
('OPH-QFP-MCU01-006', 'LOT-QFP-20260601-001', 30, 'WIRE_BOND', 'Complete', 'EQP-WBD-003', 'EMP-021', 'RCP-WBD-BGA-001', DATE_SUB(@now, INTERVAL 6 DAY), 49750, 49600, '{"passQty":49500,"failQty":100,"scrapQty":50}'),
('OPH-QFP-MCU01-007', 'LOT-QFP-20260601-001', 40, 'MOLD', 'Start', 'EQP-MLD-003', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 49600, NULL, NULL),
('OPH-QFP-MCU01-008', 'LOT-QFP-20260601-001', 40, 'MOLD', 'Complete', 'EQP-MLD-003', 'EMP-020', 'RCP-MLD-BGA-001', DATE_SUB(@now, INTERVAL 5 DAY), 49600, 49450, '{"passQty":49350,"failQty":100,"scrapQty":50}'),
('OPH-QFP-MCU01-009', 'LOT-QFP-20260601-001', 50, 'CURE', 'Start', 'EQP-OVN-001', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49450, NULL, NULL),
('OPH-QFP-MCU01-010', 'LOT-QFP-20260601-001', 50, 'CURE', 'Complete', 'EQP-OVN-001', 'EMP-022', 'RCP-OVN-CURE-001', DATE_SUB(@now, INTERVAL 4 DAY), 49450, 49400, '{"passQty":49400,"failQty":50,"scrapQty":0}'),
('OPH-QFP-MCU01-011', 'LOT-QFP-20260601-001', 60, 'PLATE', 'Start', 'EQP-PLT-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49400, NULL, NULL),
('OPH-QFP-MCU01-012', 'LOT-QFP-20260601-001', 60, 'PLATE', 'Complete', 'EQP-PLT-002', 'EMP-020', 'RCP-PLT-BGA-001', DATE_SUB(@now, INTERVAL 4 DAY), 49400, 49350, '{"passQty":49350,"failQty":50,"scrapQty":0}'),
('OPH-QFP-MCU01-013', 'LOT-QFP-20260601-001', 70, 'TRIM_FORM', 'Start', 'EQP-TRF-002', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49350, NULL, NULL),
('OPH-QFP-MCU01-014', 'LOT-QFP-20260601-001', 70, 'TRIM_FORM', 'Complete', 'EQP-TRF-002', 'EMP-021', 'RCP-TRF-SOP-001', DATE_SUB(@now, INTERVAL 3 DAY), 49350, 49250, '{"passQty":49200,"failQty":50,"scrapQty":50}'),
('OPH-QFP-MCU01-015', 'LOT-QFP-20260601-001', 80, 'TEST_FT', 'Start', 'EQP-TST-002', 'EMP-028', NULL, DATE_SUB(@now, INTERVAL 2 DAY), 49250, NULL, NULL);

-- Operations for LOT-BGA-20260601-004 (partial - at LASER_MARK)
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-BGA-SOC04-001', 'LOT-BGA-20260601-004', 10, 'GRIND', 'Start', 'EQP-GRD-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 50000, NULL, NULL),
('OPH-BGA-SOC04-002', 'LOT-BGA-20260601-004', 10, 'GRIND', 'Complete', 'EQP-GRD-001', 'EMP-020', 'RCP-GRD-STD-001', DATE_SUB(@now, INTERVAL 5 DAY), 50000, 49900, '{"passQty":49900,"failQty":50,"scrapQty":50}'),
('OPH-BGA-SOC04-003', 'LOT-BGA-20260601-004', 20, 'DICE', 'Start', 'EQP-DIC-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49900, NULL, NULL),
('OPH-BGA-SOC04-004', 'LOT-BGA-20260601-004', 20, 'DICE', 'Complete', 'EQP-DIC-001', 'EMP-021', 'RCP-DIC-STD-001', DATE_SUB(@now, INTERVAL 4 DAY), 49900, 49750, '{"passQty":49670,"failQty":80,"scrapQty":70}'),
('OPH-BGA-SOC04-005', 'LOT-BGA-20260601-004', 30, 'DIE_ATTACH', 'Start', 'EQP-DAB-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49750, NULL, NULL),
('OPH-BGA-SOC04-006', 'LOT-BGA-20260601-004', 30, 'DIE_ATTACH', 'Complete', 'EQP-DAB-001', 'EMP-020', 'RCP-DAB-BGA-001', DATE_SUB(@now, INTERVAL 4 DAY), 49750, 49600, '{"passQty":49520,"failQty":80,"scrapQty":70}'),
('OPH-BGA-SOC04-007', 'LOT-BGA-20260601-004', 40, 'CURE_DA', 'Start', 'EQP-OVN-001', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49600, NULL, NULL),
('OPH-BGA-SOC04-008', 'LOT-BGA-20260601-004', 40, 'CURE_DA', 'Complete', 'EQP-OVN-001', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 3 DAY), 49600, 49550, '{"passQty":49550,"failQty":50,"scrapQty":0}'),
('OPH-BGA-SOC04-009', 'LOT-BGA-20260601-004', 50, 'WIRE_BOND', 'Start', 'EQP-WBD-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49550, NULL, NULL),
('OPH-BGA-SOC04-010', 'LOT-BGA-20260601-004', 50, 'WIRE_BOND', 'Complete', 'EQP-WBD-001', 'EMP-021', 'RCP-WBD-BGA-001', DATE_SUB(@now, INTERVAL 3 DAY), 49550, 49400, '{"passQty":49300,"failQty":100,"scrapQty":50}'),
('OPH-BGA-SOC04-011', 'LOT-BGA-20260601-004', 60, 'MOLD', 'Start', 'EQP-MLD-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 2 DAY), 49400, NULL, NULL),
('OPH-BGA-SOC04-012', 'LOT-BGA-20260601-004', 60, 'MOLD', 'Complete', 'EQP-MLD-001', 'EMP-020', 'RCP-MLD-BGA-001', DATE_SUB(@now, INTERVAL 2 DAY), 49400, 49250, '{"passQty":49150,"failQty":100,"scrapQty":50}'),
('OPH-BGA-SOC04-013', 'LOT-BGA-20260601-004', 70, 'CURE', 'Start', 'EQP-OVN-003', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 1 DAY), 49250, NULL, NULL),
('OPH-BGA-SOC04-014', 'LOT-BGA-20260601-004', 70, 'CURE', 'Complete', 'EQP-OVN-003', 'EMP-022', 'RCP-OVN-CURE-001', DATE_SUB(@now, INTERVAL 1 DAY), 49250, 49200, '{"passQty":49200,"failQty":50,"scrapQty":0}'),
('OPH-BGA-SOC04-015', 'LOT-BGA-20260601-004', 80, 'LASER_MARK', 'Start', 'EQP-LMK-001', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 8 HOUR), 49200, NULL, NULL);

-- Operations for LOT-QFN-20260602-001 (partial)
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-QFN-SEN03-001', 'LOT-QFN-20260602-001', 10, 'DIE_ATTACH', 'Start', 'EQP-DAB-004', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 50000, NULL, NULL),
('OPH-QFN-SEN03-002', 'LOT-QFN-20260602-001', 10, 'DIE_ATTACH', 'Complete', 'EQP-DAB-004', 'EMP-020', 'RCP-DAB-QFN-001', DATE_SUB(@now, INTERVAL 4 DAY), 50000, 49800, '{"passQty":49700,"failQty":100,"scrapQty":100}'),
('OPH-QFN-SEN03-003', 'LOT-QFN-20260602-001', 20, 'CURE_DA', 'Start', 'EQP-OVN-004', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 49800, NULL, NULL),
('OPH-QFN-SEN03-004', 'LOT-QFN-20260602-001', 20, 'CURE_DA', 'Complete', 'EQP-OVN-004', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 3 DAY), 49800, 49750, '{"passQty":49750,"failQty":50,"scrapQty":0}'),
('OPH-QFN-SEN03-005', 'LOT-QFN-20260602-001', 30, 'WIRE_BOND', 'Start', 'EQP-WBD-004', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 2 DAY), 49750, NULL, NULL);

-- Operations for LOT-SOP-20260602-001 (partial)
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-SOP-MOS03-001', 'LOT-SOP-20260602-001', 10, 'DIE_ATTACH', 'Start', 'EQP-DAB-001', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 6 DAY), 50000, NULL, NULL),
('OPH-SOP-MOS03-002', 'LOT-SOP-20260602-001', 10, 'DIE_ATTACH', 'Complete', 'EQP-DAB-001', 'EMP-020', 'RCP-DAB-BGA-001', DATE_SUB(@now, INTERVAL 6 DAY), 50000, 49850, '{"passQty":49770,"failQty":80,"scrapQty":70}'),
('OPH-SOP-MOS03-003', 'LOT-SOP-20260602-001', 20, 'CURE_DA', 'Start', 'EQP-OVN-001', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 5 DAY), 49850, NULL, NULL),
('OPH-SOP-MOS03-004', 'LOT-SOP-20260602-001', 20, 'CURE_DA', 'Complete', 'EQP-OVN-001', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 5 DAY), 49850, 49800, '{"passQty":49800,"failQty":50,"scrapQty":0}'),
('OPH-SOP-MOS03-005', 'LOT-SOP-20260602-001', 30, 'WIRE_BOND', 'Start', 'EQP-WBD-005', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 4 DAY), 49800, NULL, NULL);

-- Operations for LOT-BGA-20260602-004 (partial)
INSERT IGNORE INTO prod_operation_history (operation_id, lot_id, step_seq, step_code, operation_type, equipment_id, operator_id, recipe_id, created_at, input_qty, output_qty, detail) VALUES
('OPH-BGA-SOC05-001', 'LOT-BGA-20260602-004', 10, 'GRIND', 'Start', 'EQP-GRD-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 3 DAY), 50000, NULL, NULL),
('OPH-BGA-SOC05-002', 'LOT-BGA-20260602-004', 10, 'GRIND', 'Complete', 'EQP-GRD-002', 'EMP-020', 'RCP-GRD-STD-001', DATE_SUB(@now, INTERVAL 3 DAY), 50000, 49900, '{"passQty":49900,"failQty":50,"scrapQty":50}'),
('OPH-BGA-SOC05-003', 'LOT-BGA-20260602-004', 20, 'DICE', 'Start', 'EQP-DIC-003', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 2 DAY), 49900, NULL, NULL),
('OPH-BGA-SOC05-004', 'LOT-BGA-20260602-004', 20, 'DICE', 'Complete', 'EQP-DIC-003', 'EMP-021', 'RCP-DIC-STD-001', DATE_SUB(@now, INTERVAL 2 DAY), 49900, 49750, '{"passQty":49670,"failQty":80,"scrapQty":70}'),
('OPH-BGA-SOC05-005', 'LOT-BGA-20260602-004', 30, 'DIE_ATTACH', 'Start', 'EQP-DAB-002', 'EMP-020', NULL, DATE_SUB(@now, INTERVAL 2 DAY), 49750, NULL, NULL),
('OPH-BGA-SOC05-006', 'LOT-BGA-20260602-004', 30, 'DIE_ATTACH', 'Complete', 'EQP-DAB-002', 'EMP-020', 'RCP-DAB-BGA-001', DATE_SUB(@now, INTERVAL 2 DAY), 49750, 49600, '{"passQty":49520,"failQty":80,"scrapQty":70}'),
('OPH-BGA-SOC05-007', 'LOT-BGA-20260602-004', 40, 'CURE_DA', 'Start', 'EQP-OVN-002', 'EMP-022', NULL, DATE_SUB(@now, INTERVAL 1 DAY), 49600, NULL, NULL),
('OPH-BGA-SOC05-008', 'LOT-BGA-20260602-004', 40, 'CURE_DA', 'Complete', 'EQP-OVN-002', 'EMP-022', 'RCP-OVN-DA-001', DATE_SUB(@now, INTERVAL 1 DAY), 49600, 49550, '{"passQty":49550,"failQty":50,"scrapQty":0}'),
('OPH-BGA-SOC05-009', 'LOT-BGA-20260602-004', 50, 'WIRE_BOND', 'Start', 'EQP-WBD-002', 'EMP-021', NULL, DATE_SUB(@now, INTERVAL 18 HOUR), 49550, NULL, NULL);

-- ============================================================================
-- 3. QUALITY_INSPECTION - Quality Inspection Records (~35 records)
-- ============================================================================

-- IQC (Incoming Quality Control) inspections
INSERT IGNORE INTO quality_inspection (inspection_id, lot_id, step_code, inspection_type, result, inspector_id, inspection_time, detail, remark) VALUES
('IQC-2026-001', 'LOT-BGA-20260515-001', NULL, 'IQC', 'Accepted', 'EMP-023', DATE_SUB(@now, INTERVAL 21 DAY), '{"defectCount":0,"sampleSize":125}', 'Leadframe incoming inspection - all dimensions within tolerance'),
('IQC-2026-002', 'LOT-BGA-20260515-002', NULL, 'IQC', 'Accepted', 'EMP-023', DATE_SUB(@now, INTERVAL 20 DAY), '{"defectCount":0,"sampleSize":125}', 'Leadframe incoming inspection - batch quality good'),
('IQC-2026-003', 'LOT-QFN-20260601-001', NULL, 'IQC', 'Accepted', 'EMP-023', DATE_SUB(@now, INTERVAL 10 DAY), '{"defectCount":0,"sampleSize":125}', 'Leadframe and wire incoming inspection'),
('IQC-2026-004', 'LOT-QFN-20260601-002', NULL, 'IQC', 'Accepted', 'EMP-023', DATE_SUB(@now, INTERVAL 8 DAY), '{"defectCount":0,"sampleSize":125}', 'Sensor die incoming inspection - electrical parameters normal'),
('IQC-2026-005', 'LOT-SOP-20260601-001', NULL, 'IQC', 'Accepted', 'EMP-024', DATE_SUB(@now, INTERVAL 10 DAY), '{"defectCount":0,"sampleSize":125}', 'MOSFET die incoming inspection'),
('IQC-2026-006', 'LOT-SOP-20260601-002', NULL, 'IQC', 'Accepted', 'EMP-024', DATE_SUB(@now, INTERVAL 10 DAY), '{"defectCount":0,"sampleSize":125}', 'Leadframe inspection for SOP package'),
('IQC-2026-007', 'LOT-DFN-20260601-001', NULL, 'IQC', 'Accepted', 'EMP-023', DATE_SUB(@now, INTERVAL 9 DAY), '{"defectCount":0,"sampleSize":125}', 'LDO regulator die incoming inspection'),
('IQC-2026-008', 'LOT-DFN-20260601-002', NULL, 'IQC', 'Accepted', 'EMP-024', DATE_SUB(@now, INTERVAL 9 DAY), '{"defectCount":0,"sampleSize":125}', 'Leadframe and mold compound inspection'),
('IQC-2026-009', 'LOT-BGA-20260601-003', NULL, 'IQC', 'Accepted', 'EMP-023', DATE_SUB(@now, INTERVAL 7 DAY), '{"defectCount":0,"sampleSize":125}', 'WiFi SoC die incoming inspection - all parameters OK'),
('IQC-2026-010', 'LOT-BGA-20260601-004', NULL, 'IQC', 'Accepted', 'EMP-024', DATE_SUB(@now, INTERVAL 6 DAY), '{"defectCount":0,"sampleSize":125}', 'SoC die inspection for BGA package'),
('IQC-2026-011', 'LOT-QFP-20260601-001', NULL, 'IQC', 'Accepted', 'EMP-023', DATE_SUB(@now, INTERVAL 8 DAY), '{"defectCount":2,"sampleSize":125}', 'MCU die incoming inspection - 2 minor cosmetic defects found but acceptable'),
('IQC-2026-012', 'LOT-QFN-20260602-001', NULL, 'IQC', 'Accepted', 'EMP-024', DATE_SUB(@now, INTERVAL 5 DAY), '{"defectCount":0,"sampleSize":125}', 'Sensor die batch inspection');

-- IPQC (In-Process Quality Control) inspections
INSERT IGNORE INTO quality_inspection (inspection_id, lot_id, step_code, inspection_type, result, inspector_id, inspection_time, detail, remark) VALUES
('IPQC-2026-001', 'LOT-BGA-20260515-001', 'WIRE_BOND', 'IPQC', 'Accepted', 'EMP-025', DATE_SUB(@now, INTERVAL 16 DAY), '{"defectCount":3,"sampleSize":200}', 'Wire bond pull test - 3 samples slightly below target but within spec'),
('IPQC-2026-002', 'LOT-BGA-20260515-001', 'MOLD', 'IPQC', 'Accepted', 'EMP-025', DATE_SUB(@now, INTERVAL 16 DAY), '{"defectCount":1,"sampleSize":150}', 'Mold compound cure check - minor void in 1 unit, re-inspected OK'),
('IPQC-2026-003', 'LOT-BGA-20260515-001', 'TEST_FT', 'IPQC', 'Accepted', 'EMP-028', DATE_SUB(@now, INTERVAL 10 DAY), '{"defectCount":400,"sampleSize":49000}', 'FT yield 99.2 percent, bin1 distribution normal'),
('IPQC-2026-004', 'LOT-BGA-20260515-002', 'WIRE_BOND', 'IPQC', 'Accepted', 'EMP-025', DATE_SUB(@now, INTERVAL 15 DAY), '{"defectCount":2,"sampleSize":200}', 'Wire bond shear test - all results within specification'),
('IPQC-2026-005', 'LOT-BGA-20260515-002', 'TEST_FT', 'IPQC', 'Accepted', 'EMP-028', DATE_SUB(@now, INTERVAL 9 DAY), '{"defectCount":500,"sampleSize":48950}', 'FT yield 99.0 percent'),
('IPQC-2026-006', 'LOT-QFN-20260601-002', 'WIRE_BOND', 'IPQC', 'Accepted', 'EMP-026', DATE_SUB(@now, INTERVAL 5 DAY), '{"defectCount":5,"sampleSize":200}', 'Wire bond inspection - 5 loops slightly high, adjusted process'),
('IPQC-2026-007', 'LOT-QFN-20260601-002', 'MOLD', 'IPQC', 'Accepted', 'EMP-026', DATE_SUB(@now, INTERVAL 5 DAY), '{"defectCount":2,"sampleSize":150}', 'Mold inspection - flash thickness within limits'),
('IPQC-2026-008', 'LOT-SOP-20260601-002', 'WIRE_BOND', 'IPQC', 'Accepted', 'EMP-025', DATE_SUB(@now, INTERVAL 7 DAY), '{"defectCount":4,"sampleSize":200}', 'Wire bond quality check for MOSFET'),
('IPQC-2026-009', 'LOT-SOP-20260601-002', 'MOLD', 'IPQC', 'Accepted', 'EMP-025', DATE_SUB(@now, INTERVAL 7 DAY), '{"defectCount":3,"sampleSize":150}', 'Mold visual inspection - minor marks acceptable'),
('IPQC-2026-010', 'LOT-DFN-20260601-001', 'WIRE_BOND', 'IPQC', 'Accepted', 'EMP-026', DATE_SUB(@now, INTERVAL 6 DAY), '{"defectCount":2,"sampleSize":200}', 'Wire bond pull test for LDO packages'),
('IPQC-2026-011', 'LOT-DFN-20260601-002', 'MOLD', 'IPQC', 'Accepted', 'EMP-025', DATE_SUB(@now, INTERVAL 6 DAY), '{"defectCount":1,"sampleSize":150}', 'Mold inspection - good quality'),
('IPQC-2026-012', 'LOT-BGA-20260601-003', 'WIRE_BOND', 'IPQC', 'Accepted', 'EMP-025', DATE_SUB(@now, INTERVAL 3 DAY), '{"defectCount":4,"sampleSize":200}', 'Wire bond inspection for WiFi packages'),
('IPQC-2026-013', 'LOT-BGA-20260601-003', 'MOLD', 'IPQC', 'Accepted', 'EMP-026', DATE_SUB(@now, INTERVAL 3 DAY), '{"defectCount":2,"sampleSize":150}', 'Mold compound cure inspection'),
('IPQC-2026-014', 'LOT-BGA-20260602-003', 'DIE_ATTACH', 'IPQC', 'Accepted', 'EMP-025', DATE_SUB(@now, INTERVAL 3 DAY), '{"defectCount":1,"sampleSize":100}', 'Die attach shear test - all within spec'),
('IPQC-2026-015', 'LOT-QFP-20260601-001', 'WIRE_BOND', 'IPQC', 'Accepted', 'EMP-026', DATE_SUB(@now, INTERVAL 5 DAY), '{"defectCount":6,"sampleSize":200}', 'Wire bond for QFP MCU - 6 leads need rework, re-inspected OK'),
('IPQC-2026-016', 'LOT-QFP-20260601-001', 'MOLD', 'IPQC', 'Accepted', 'EMP-025', DATE_SUB(@now, INTERVAL 5 DAY), '{"defectCount":3,"sampleSize":150}', 'Mold inspection for QFP packages'),
('IPQC-2026-017', 'LOT-QFN-20260602-001', 'WIRE_BOND', 'IPQC', 'In-Progress', 'EMP-026', DATE_SUB(@now, INTERVAL 1 DAY), '{"defectCount":0,"sampleSize":100}', 'In-process wire bond inspection'),
('IPQC-2026-018', 'LOT-BGA-20260601-004', 'WIRE_BOND', 'IPQC', 'Accepted', 'EMP-025', DATE_SUB(@now, INTERVAL 2 DAY), '{"defectCount":3,"sampleSize":200}', 'Wire bond inspection for SoC packages'),
('IPQC-2026-019', 'LOT-BGA-20260602-004', 'DIE_ATTACH', 'IPQC', 'Accepted', 'EMP-026', DATE_SUB(@now, INTERVAL 1 DAY), '{"defectCount":0,"sampleSize":100}', 'Die attach inspection for SoC - good quality');

-- OQC (Outgoing Quality Control) inspections
INSERT IGNORE INTO quality_inspection (inspection_id, lot_id, step_code, inspection_type, result, inspector_id, inspection_time, detail, remark) VALUES
('OQC-2026-001', 'LOT-BGA-20260515-001', 'FINAL_PACK', 'OQC', 'Released', 'EMP-027', DATE_SUB(@now, INTERVAL 10 DAY), '{"defectCount":0,"sampleSize":200}', 'Final pack inspection - packaging integrity verified'),
('OQC-2026-002', 'LOT-BGA-20260515-001', 'FINAL_PACK', 'OQC', 'Released', 'EMP-027', DATE_SUB(@now, INTERVAL 10 DAY), '{"defectCount":0,"sampleSize":50}', 'Shipping label and traceability verification'),
('OQC-2026-003', 'LOT-BGA-20260515-002', 'FINAL_PACK', 'OQC', 'Released', 'EMP-027', DATE_SUB(@now, INTERVAL 9 DAY), '{"defectCount":0,"sampleSize":200}', 'Final pack inspection - dry pack and MSL labeling verified'),
('OQC-2026-004', 'LOT-BGA-20260515-002', 'FINAL_PACK', 'OQC', 'Released', 'EMP-027', DATE_SUB(@now, INTERVAL 9 DAY), '{"defectCount":0,"sampleSize":50}', 'Customer-specific packaging requirements verified');

-- ============================================================================
-- 4. QUALITY_INSPECTION_ITEM - Inspection Item Details (~120 records)
-- ============================================================================

-- IQC inspection items
INSERT IGNORE INTO quality_inspection_item (inspection_id, item_code, item_name, usl, lsl, target_value, measured_value, unit, result, remark) VALUES
('IQC-2026-001', 'LF-WIDTH', 'Leadframe Width', 13.05, 12.95, 13.00, 13.01, 'mm', 'Pass', NULL),
('IQC-2026-001', 'LF-THICK', 'Leadframe Thickness', 0.210, 0.190, 0.200, 0.198, 'mm', 'Pass', NULL),
('IQC-2026-001', 'LF-FLAT', 'Leadframe Flatness', 0.10, NULL, NULL, 0.05, 'mm', 'Pass', NULL),
('IQC-2026-001', 'LF-PLATE', 'Plating Thickness', 4.00, 2.00, 3.00, 2.95, 'um', 'Pass', NULL),
('IQC-2026-001', 'LF-VISUAL', 'Visual Inspection', NULL, NULL, NULL, NULL, 'N/A', 'Pass', 'No visible defects'),
('IQC-2026-002', 'LF-WIDTH', 'Leadframe Width', 13.05, 12.95, 13.00, 12.99, 'mm', 'Pass', NULL),
('IQC-2026-002', 'LF-THICK', 'Leadframe Thickness', 0.210, 0.190, 0.200, 0.201, 'mm', 'Pass', NULL),
('IQC-2026-002', 'LF-PLATE', 'Plating Thickness', 4.00, 2.00, 3.00, 3.10, 'um', 'Pass', NULL),
('IQC-2026-004', 'DIE-THICK', 'Die Thickness', 0.310, 0.290, 0.300, 0.298, 'mm', 'Pass', 'Sensor die thickness'),
('IQC-2026-004', 'DIE-WIDTH', 'Die Width', 4.05, 3.95, 4.00, 4.01, 'mm', 'Pass', NULL),
('IQC-2026-004', 'DIE-ELEC', 'Electrical Test - VF', 0.80, 0.60, 0.70, 0.69, 'V', 'Pass', 'Forward voltage test'),
('IQC-2026-004', 'DIE-VISUAL', 'Die Visual Inspection', NULL, NULL, NULL, NULL, 'N/A', 'Pass', 'No cracks or chips'),
('IQC-2026-009', 'DIE-THICK', 'Die Thickness', 0.110, 0.090, 0.100, 0.098, 'mm', 'Pass', 'WiFi SoC die'),
('IQC-2026-009', 'DIE-ELEC', 'Electrical Test - IDD', 5.0, NULL, NULL, 3.2, 'mA', 'Pass', 'Quiescent current test'),
('IQC-2026-011', 'DIE-ELEC', 'Electrical Test - FMAX', NULL, 190, 200, 195, 'MHz', 'Pass', 'Max frequency test'),
('IQC-2026-011', 'DIE-VISUAL', 'Die Visual Inspection', NULL, NULL, NULL, NULL, 'N/A', 'Fail', '2 units with minor cosmetic marks');

-- IPQC inspection items - Wire Bond
INSERT IGNORE INTO quality_inspection_item (inspection_id, item_code, item_name, usl, lsl, target_value, measured_value, unit, result, remark) VALUES
('IPQC-2026-001', 'WB-PULL', 'Wire Pull Strength', NULL, 3.0, 4.5, 4.2, 'gf', 'Pass', NULL),
('IPQC-2026-001', 'WB-LOOP', 'Loop Height', 0.15, 0.08, 0.12, 0.11, 'mm', 'Pass', NULL),
('IPQC-2026-001', 'WB-NSPD', 'NSD Width', 0.07, 0.03, 0.05, 0.05, 'mm', 'Pass', 'Non-soldered pad deformation'),
('IPQC-2026-001', 'WB-VISUAL', 'Wire Sweep', 0.20, NULL, NULL, 0.12, 'mm', 'Pass', NULL),
('IPQC-2026-004', 'WB-PULL', 'Wire Pull Strength', NULL, 3.0, 4.5, 4.8, 'gf', 'Pass', NULL),
('IPQC-2026-004', 'WB-SHEAR', 'Wire Shear Strength', NULL, 2.0, 3.5, 3.6, 'gf', 'Pass', NULL),
('IPQC-2026-004', 'WB-LOOP', 'Loop Height', 0.15, 0.08, 0.12, 0.13, 'mm', 'Pass', NULL),
('IPQC-2026-006', 'WB-PULL', 'Wire Pull Strength', NULL, 3.0, 4.0, 3.8, 'gf', 'Pass', 'QFN sensor wire bond'),
('IPQC-2026-006', 'WB-LOOP', 'Loop Height', 0.13, 0.06, 0.10, 0.12, 'mm', 'Fail', '5 samples above 0.13mm'),
('IPQC-2026-006', 'WB-VISUAL', 'Wire Sweep', 0.15, NULL, NULL, 0.10, 'mm', 'Pass', NULL),
('IPQC-2026-008', 'WB-PULL', 'Wire Pull Strength', NULL, 3.0, 4.0, 4.1, 'gf', 'Pass', 'SOP MOSFET wire bond'),
('IPQC-2026-008', 'WB-SHEAR', 'Wire Shear Strength', NULL, 2.0, 3.0, 3.2, 'gf', 'Pass', NULL),
('IPQC-2026-010', 'WB-PULL', 'Wire Pull Strength', NULL, 3.0, 4.0, 4.0, 'gf', 'Pass', 'DFN LDO wire bond'),
('IPQC-2026-012', 'WB-PULL', 'Wire Pull Strength', NULL, 3.0, 4.5, 4.3, 'gf', 'Pass', 'BGA WiFi wire bond'),
('IPQC-2026-012', 'WB-LOOP', 'Loop Height', 0.15, 0.08, 0.12, 0.12, 'mm', 'Pass', NULL),
('IPQC-2026-015', 'WB-PULL', 'Wire Pull Strength', NULL, 3.0, 4.0, 3.7, 'gf', 'Pass', 'QFP MCU wire bond'),
('IPQC-2026-015', 'WB-NSPD', 'NSD Width', 0.07, 0.03, 0.05, 0.06, 'mm', 'Pass', NULL),
('IPQC-2026-015', 'WB-VISUAL', 'Wire Sweep', 0.15, NULL, NULL, 0.14, 'mm', 'Pass', '6 leads reworked');

-- IPQC inspection items - Mold
INSERT IGNORE INTO quality_inspection_item (inspection_id, item_code, item_name, usl, lsl, target_value, measured_value, unit, result, remark) VALUES
('IPQC-2026-002', 'MOLD-WIDTH', 'Package Width', 13.10, 12.90, 13.00, 13.02, 'mm', 'Pass', NULL),
('IPQC-2026-002', 'MOLD-THICK', 'Package Thickness', 1.45, 1.35, 1.40, 1.41, 'mm', 'Pass', NULL),
('IPQC-2026-002', 'MOLD-VOID', 'Void Size', 0.5, NULL, NULL, 0.3, 'mm', 'Pass', '1 unit with void, re-inspected OK'),
('IPQC-2026-002', 'MOLD-FLASH', 'Flash Thickness', 0.05, NULL, NULL, 0.03, 'mm', 'Pass', NULL),
('IPQC-2026-007', 'MOLD-WIDTH', 'Package Width', 6.10, 5.90, 6.00, 6.01, 'mm', 'Pass', 'QFN package width'),
('IPQC-2026-007', 'MOLD-THICK', 'Package Thickness', 0.90, 0.80, 0.85, 0.86, 'mm', 'Pass', NULL),
('IPQC-2026-007', 'MOLD-FLASH', 'Flash Thickness', 0.05, NULL, NULL, 0.04, 'mm', 'Pass', 'Flash within limits'),
('IPQC-2026-009', 'MOLD-WIDTH', 'Package Width', 4.00, 3.80, 3.90, 3.91, 'mm', 'Pass', 'SOP package width'),
('IPQC-2026-009', 'MOLD-THICK', 'Package Thickness', 1.30, 1.20, 1.25, 1.26, 'mm', 'Pass', NULL),
('IPQC-2026-013', 'MOLD-THICK', 'Package Thickness', 1.45, 1.35, 1.40, 1.39, 'mm', 'Pass', 'BGA package thickness'),
('IPQC-2026-013', 'MOLD-COP', 'Coplanarity', 0.10, NULL, NULL, 0.06, 'mm', 'Pass', NULL),
('IPQC-2026-016', 'MOLD-WIDTH', 'Package Width', 10.10, 9.90, 10.00, 10.02, 'mm', 'Pass', 'QFP package width'),
('IPQC-2026-016', 'MOLD-THICK', 'Package Thickness', 1.45, 1.35, 1.40, 1.41, 'mm', 'Pass', NULL);

-- IPQC inspection items - Die Attach
INSERT IGNORE INTO quality_inspection_item (inspection_id, item_code, item_name, usl, lsl, target_value, measured_value, unit, result, remark) VALUES
('IPQC-2026-014', 'DA-SHEAR', 'Die Shear Strength', NULL, 2.0, 3.0, 3.1, 'kgf', 'Pass', NULL),
('IPQC-2026-014', 'DA-BLT', 'Bond Line Thickness', 0.040, 0.020, 0.030, 0.028, 'mm', 'Pass', NULL),
('IPQC-2026-014', 'DA-OFFSET', 'Die Placement Offset', 0.05, NULL, NULL, 0.03, 'mm', 'Pass', NULL),
('IPQC-2026-019', 'DA-SHEAR', 'Die Shear Strength', NULL, 2.0, 3.0, 3.3, 'kgf', 'Pass', 'SoC die attach'),
('IPQC-2026-019', 'DA-BLT', 'Bond Line Thickness', 0.035, 0.015, 0.025, 0.024, 'mm', 'Pass', NULL);

-- FT Test inspection items
INSERT IGNORE INTO quality_inspection_item (inspection_id, item_code, item_name, usl, lsl, target_value, measured_value, unit, result, remark) VALUES
('IPQC-2026-003', 'FT-VDD', 'Supply Current', 10.0, NULL, NULL, 4.5, 'mA', 'Pass', NULL),
('IPQC-2026-003', 'FT-VIH', 'Input High Voltage', NULL, 2.0, NULL, 2.4, 'V', 'Pass', NULL),
('IPQC-2026-003', 'FT-VIL', 'Input Low Voltage', 0.8, NULL, NULL, 0.4, 'V', 'Pass', NULL),
('IPQC-2026-003', 'FT-FREQ', 'Max Frequency', NULL, 190, 200, 198, 'MHz', 'Pass', NULL),
('IPQC-2026-003', 'FT-LEAK', 'Leakage Current', 10.0, NULL, NULL, 1.2, 'uA', 'Pass', NULL),
('IPQC-2026-005', 'FT-VDD', 'Supply Current', 10.0, NULL, NULL, 5.1, 'mA', 'Pass', NULL),
('IPQC-2026-005', 'FT-FREQ', 'Max Frequency', NULL, 190, 200, 196, 'MHz', 'Pass', NULL),
('IPQC-2026-005', 'FT-TEMP', 'Thermal Test', 125, NULL, NULL, 95, 'C', 'Pass', NULL);

-- OQC inspection items
INSERT IGNORE INTO quality_inspection_item (inspection_id, item_code, item_name, usl, lsl, target_value, measured_value, unit, result, remark) VALUES
('OQC-2026-001', 'OQC-LABEL', 'Label Verification', NULL, NULL, NULL, NULL, 'N/A', 'Pass', 'Product label correct'),
('OQC-2026-001', 'OQC-PACK', 'Package Integrity', NULL, NULL, NULL, NULL, 'N/A', 'Pass', 'No damage to trays'),
('OQC-2026-001', 'OQC-QTY', 'Quantity Check', 500, 500, 500, 500, 'units', 'Pass', 'Per tray quantity'),
('OQC-2026-001', 'OQC-DRY', 'Dry Pack Check', NULL, NULL, NULL, NULL, 'N/A', 'Pass', 'Desiccant and humidity card OK'),
('OQC-2026-002', 'OQC-TRACE', 'Traceability Check', NULL, NULL, NULL, NULL, 'N/A', 'Pass', 'Lot ID and date code verified'),
('OQC-2026-003', 'OQC-LABEL', 'Label Verification', NULL, NULL, NULL, NULL, 'N/A', 'Pass', 'Customer-specific labels correct'),
('OQC-2026-003', 'OQC-MSL', 'MSL Label', NULL, NULL, NULL, NULL, 'N/A', 'Pass', 'Moisture sensitivity level correct'),
('OQC-2026-004', 'OQC-PACK', 'Custom Package', NULL, NULL, NULL, NULL, 'N/A', 'Pass', 'Reel packaging per customer spec');

-- ============================================================================
-- 5. EQUIPMENT_MAINTENANCE - Equipment Maintenance Records (~22 records)
-- ============================================================================

-- Preventive Maintenance (PM)
INSERT IGNORE INTO equipment_maintenance (maintenance_id, equipment_id, maintenance_type, description, status, technician_id, scheduled_at, started_at, completed_at, actual_hours, parts_replaced, notes, created_by) VALUES
('MT-2026-001', 'EQP-GRD-001', 'Preventive', 'Quarterly PM - grinding wheel replacement and spindle calibration', 'Completed', 'EMP-030', DATE_SUB(@now, INTERVAL 25 DAY), DATE_SUB(@now, INTERVAL 25 DAY), DATE_SUB(@now, INTERVAL 25 DAY), NULL, 'Grinding Wheel GW-200, Spindle Bearing SB-001', 'Completed on schedule', NULL),
('MT-2026-002', 'EQP-GRD-002', 'Preventive', 'Quarterly PM - grinding wheel and coolant system check', 'Completed', 'EMP-031', DATE_SUB(@now, INTERVAL 24 DAY), DATE_SUB(@now, INTERVAL 24 DAY), DATE_SUB(@now, INTERVAL 24 DAY), NULL, 'Grinding Wheel GW-200', 'Coolant concentration adjusted', NULL),
('MT-2026-003', 'EQP-DIC-001', 'Preventive', 'Monthly PM - blade inspection and spindle alignment', 'Completed', 'EMP-030', DATE_SUB(@now, INTERVAL 20 DAY), DATE_SUB(@now, INTERVAL 20 DAY), DATE_SUB(@now, INTERVAL 20 DAY), NULL, 'Dicing Blade DB-120', 'Blade life 85 percent consumed', NULL),
('MT-2026-004', 'EQP-DIC-002', 'Preventive', 'Monthly PM - blade replacement and cutting depth calibration', 'Completed', 'EMP-031', DATE_SUB(@now, INTERVAL 19 DAY), DATE_SUB(@now, INTERVAL 19 DAY), DATE_SUB(@now, INTERVAL 19 DAY), NULL, 'Dicing Blade DB-120', NULL, NULL),
('MT-2026-005', 'EQP-WBD-001', 'Preventive', 'Weekly PM - capillary inspection and ultrasonic power calibration', 'Completed', 'EMP-030', DATE_SUB(@now, INTERVAL 15 DAY), DATE_SUB(@now, INTERVAL 15 DAY), DATE_SUB(@now, INTERVAL 15 DAY), NULL, 'Capillary CAP-25um', 'Ultrasonic power adjusted to nominal', NULL),
('MT-2026-006', 'EQP-WBD-002', 'Preventive', 'Weekly PM - capillary replacement and wire pull test calibration', 'Completed', 'EMP-031', DATE_SUB(@now, INTERVAL 14 DAY), DATE_SUB(@now, INTERVAL 14 DAY), DATE_SUB(@now, INTERVAL 14 DAY), NULL, 'Capillary CAP-25um', NULL, NULL),
('MT-2026-007', 'EQP-MLD-001', 'Preventive', 'Monthly PM - mold chase cleaning and clamp force calibration', 'Completed', 'EMP-030', DATE_SUB(@now, INTERVAL 12 DAY), DATE_SUB(@now, INTERVAL 12 DAY), DATE_SUB(@now, INTERVAL 12 DAY), NULL, 'Mold Chase Cleaner MCC-001', 'Mold chase life 70 percent', NULL),
('MT-2026-008', 'EQP-MLD-002', 'Preventive', 'Monthly PM - mold chase cleaning and temperature sensor verification', 'Completed', 'EMP-031', DATE_SUB(@now, INTERVAL 11 DAY), DATE_SUB(@now, INTERVAL 11 DAY), DATE_SUB(@now, INTERVAL 11 DAY), NULL, NULL, 'All temperature sensors within tolerance', NULL),
('MT-2026-009', 'EQP-OVN-001', 'Preventive', 'Quarterly PM - temperature profile calibration and nitrogen flow check', 'Completed', 'EMP-030', DATE_SUB(@now, INTERVAL 10 DAY), DATE_SUB(@now, INTERVAL 10 DAY), DATE_SUB(@now, INTERVAL 10 DAY), NULL, NULL, 'Temperature uniformity +/- 2C', NULL),
('MT-2026-010', 'EQP-OVN-002', 'Preventive', 'Quarterly PM - temperature calibration and conveyor speed check', 'Completed', 'EMP-031', DATE_SUB(@now, INTERVAL 9 DAY), DATE_SUB(@now, INTERVAL 9 DAY), DATE_SUB(@now, INTERVAL 9 DAY), NULL, NULL, NULL, NULL),
('MT-2026-011', 'EQP-TST-001', 'Preventive', 'Monthly PM - test head cleaning and contactor inspection', 'Completed', 'EMP-030', DATE_SUB(@now, INTERVAL 8 DAY), DATE_SUB(@now, INTERVAL 8 DAY), DATE_SUB(@now, INTERVAL 8 DAY), NULL, 'Contactor CTR-400', 'Pin resistance within spec', NULL),
('MT-2026-012', 'EQP-TST-002', 'Preventive', 'Monthly PM - test head calibration and golden unit verification', 'Completed', 'EMP-031', DATE_SUB(@now, INTERVAL 7 DAY), DATE_SUB(@now, INTERVAL 7 DAY), DATE_SUB(@now, INTERVAL 7 DAY), NULL, NULL, 'Golden unit passed all tests', NULL),
('MT-2026-013', 'EQP-PLT-001', 'Preventive', 'Weekly PM - plating bath analysis and anode inspection', 'Completed', 'EMP-030', DATE_SUB(@now, INTERVAL 6 DAY), DATE_SUB(@now, INTERVAL 6 DAY), DATE_SUB(@now, INTERVAL 6 DAY), NULL, 'Plating Anode PA-Ni', 'Bath concentration adjusted', NULL),
('MT-2026-014', 'EQP-LMK-001', 'Preventive', 'Monthly PM - laser power calibration and lens cleaning', 'Completed', 'EMP-031', DATE_SUB(@now, INTERVAL 5 DAY), DATE_SUB(@now, INTERVAL 5 DAY), DATE_SUB(@now, INTERVAL 5 DAY), NULL, NULL, 'Laser power at 98 percent of nominal', NULL);

-- Corrective Maintenance (CM)
INSERT IGNORE INTO equipment_maintenance (maintenance_id, equipment_id, maintenance_type, description, status, technician_id, scheduled_at, started_at, completed_at, actual_hours, parts_replaced, notes, created_by) VALUES
('MT-2026-015', 'EQP-DIC-003', 'Corrective', 'Emergency repair - spindle vibration alarm, replaced worn bearing', 'Completed', 'EMP-030', DATE_SUB(@now, INTERVAL 18 DAY), DATE_SUB(@now, INTERVAL 18 DAY), DATE_SUB(@now, INTERVAL 17 DAY), NULL, 'Spindle Bearing SB-002, Belt BLT-003', 'Root cause: bearing wear due to extended use beyond replacement interval', NULL),
('MT-2026-016', 'EQP-WBD-003', 'Corrective', 'Emergency repair - wire feed jam, cleaned and replaced feed mechanism', 'Completed', 'EMP-031', DATE_SUB(@now, INTERVAL 16 DAY), DATE_SUB(@now, INTERVAL 16 DAY), DATE_SUB(@now, INTERVAL 15 DAY), NULL, 'Wire Feed Roller WFR-001', 'Cause: contaminated roller surface, cleaned and replaced', NULL),
('MT-2026-017', 'EQP-MLD-003', 'Corrective', 'Emergency repair - mold clamp force insufficient, hydraulic system repair', 'Completed', 'EMP-030', DATE_SUB(@now, INTERVAL 14 DAY), DATE_SUB(@now, INTERVAL 14 DAY), DATE_SUB(@now, INTERVAL 13 DAY), NULL, 'Hydraulic Seal HS-005, Filter FLT-010', 'Hydraulic fluid replaced, system re-pressurized', NULL),
('MT-2026-018', 'EQP-OVN-003', 'Corrective', 'Emergency repair - temperature fluctuation, replaced faulty thermocouple', 'Completed', 'EMP-031', DATE_SUB(@now, INTERVAL 11 DAY), DATE_SUB(@now, INTERVAL 11 DAY), DATE_SUB(@now, INTERVAL 10 DAY), NULL, 'Thermocouple TC-K-001', NULL, NULL),
('MT-2026-019', 'EQP-TRF-002', 'Corrective', 'Emergency repair - cutting blade misalignment, realigned and replaced blade', 'Completed', 'EMP-030', DATE_SUB(@now, INTERVAL 8 DAY), DATE_SUB(@now, INTERVAL 8 DAY), DATE_SUB(@now, INTERVAL 7 DAY), NULL, 'Trim Form Blade TFB-002', 'Alignment recalibrated to spec', NULL),
('MT-2026-020', 'EQP-PCK-001', 'Corrective', 'Emergency repair - tape feeder jam, cleaned and realigned', 'Completed', 'EMP-031', DATE_SUB(@now, INTERVAL 5 DAY), DATE_SUB(@now, INTERVAL 5 DAY), DATE_SUB(@now, INTERVAL 5 DAY), NULL, NULL, 'Feeder mechanism lubricated', NULL),
('MT-2026-021', 'EQP-TST-003', 'Preventive', 'Scheduled PM - test head cleaning and calibration', 'Scheduled', 'EMP-030', DATE_SUB(@now, INTERVAL -2 DAY), NULL, NULL, NULL, NULL, 'Planned for next maintenance window', NULL),
('MT-2026-022', 'EQP-PLT-003', 'Preventive', 'Scheduled PM - plating bath analysis and filter replacement', 'Scheduled', 'EMP-031', DATE_SUB(@now, INTERVAL -3 DAY), NULL, NULL, NULL, 'Filter FLT-PLT-001', 'Bath sample sent to lab for analysis', NULL);

-- ============================================================================
-- 6. EQUIPMENT_FAILURE - Equipment Failure Records (~12 records)
-- ============================================================================

INSERT IGNORE INTO equipment_failure (failure_id, equipment_id, failure_type, description, severity, status, reported_by, reported_at, resolved_at, resolved_by, downtime_minutes, root_cause, resolution) VALUES
('FAIL-2026-001', 'EQP-DIC-003', 'Mechanical', 'Spindle vibration alarm triggered during dicing operation', 'Major', 'Resolved', 'EMP-030', DATE_SUB(@now, INTERVAL 18 DAY), DATE_SUB(@now, INTERVAL 17 DAY), 'EMP-030', 480, 'Spindle bearing wear beyond replacement interval', 'Replaced spindle bearing and belt, recalibrated spindle alignment'),
('FAIL-2026-002', 'EQP-WBD-003', 'Mechanical', 'Wire feed mechanism jam causing wire breakage', 'Major', 'Resolved', 'EMP-031', DATE_SUB(@now, INTERVAL 16 DAY), DATE_SUB(@now, INTERVAL 15 DAY), 'EMP-031', 360, 'Contaminated wire feed roller surface', 'Replaced wire feed roller, cleaned feed path, adjusted wire tension'),
('FAIL-2026-003', 'EQP-MLD-003', 'Hydraulic', 'Mold clamp force dropped below minimum threshold during molding cycle', 'Major', 'Resolved', 'EMP-030', DATE_SUB(@now, INTERVAL 14 DAY), DATE_SUB(@now, INTERVAL 13 DAY), 'EMP-030', 540, 'Hydraulic seal failure causing fluid leak', 'Replaced hydraulic seals and filter, refilled hydraulic fluid, re-pressurized system'),
('FAIL-2026-004', 'EQP-OVN-003', 'Electrical', 'Temperature fluctuation detected during cure cycle', 'Major', 'Resolved', 'EMP-031', DATE_SUB(@now, INTERVAL 11 DAY), DATE_SUB(@now, INTERVAL 10 DAY), 'EMP-031', 300, 'Faulty thermocouple providing erratic readings', 'Replaced thermocouple, verified temperature profile with reference sensor'),
('FAIL-2026-005', 'EQP-TRF-002', 'Mechanical', 'Cutting blade misalignment causing lead deformation during trim/form', 'Major', 'Resolved', 'EMP-030', DATE_SUB(@now, INTERVAL 8 DAY), DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-030', 420, 'Blade mounting fixture wear', 'Replaced blade, realigned fixture, verified cut quality with sample strips'),
('FAIL-2026-006', 'EQP-PLT-002', 'Chemical', 'Plating bath concentration drifted outside specification', 'Minor', 'Resolved', 'EMP-031', DATE_SUB(@now, INTERVAL 6 DAY), DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-031', 240, 'Anode degradation rate higher than expected', 'Replaced plating anode, adjusted bath chemistry, increased monitoring frequency'),
('FAIL-2026-007', 'EQP-LMK-002', 'Optical', 'Laser marking quality degraded - faint characters on some units', 'Minor', 'Resolved', 'EMP-030', DATE_SUB(@now, INTERVAL 4 DAY), DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-030', 120, 'Lens contamination from mold compound particles', 'Cleaned laser lens, verified marking quality, added lens cover maintenance to PM schedule'),
('FAIL-2026-008', 'EQP-TST-001', 'Electrical', 'Test contactor pin resistance increased causing intermittent test failures', 'Major', 'Resolved', 'EMP-031', DATE_SUB(@now, INTERVAL 3 DAY), DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-031', 360, 'Contactor pin oxidation from extended use', 'Replaced contactor, performed golden unit verification, updated replacement schedule'),
('FAIL-2026-009', 'EQP-DAB-004', 'Mechanical', 'Die placement accuracy degraded - offset exceeding tolerance', 'Minor', 'Resolved', 'EMP-030', DATE_SUB(@now, INTERVAL 2 DAY), DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-030', 180, 'Pick-and-place nozzle wear', 'Replaced nozzle, recalibrated placement accuracy, verified with die shear test'),
('FAIL-2026-010', 'EQP-GRD-001', 'Mechanical', 'Grinding wheel vibration causing wafer backside scratches', 'Minor', 'Resolved', 'EMP-031', DATE_SUB(@now, INTERVAL 1 DAY), DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-031', 150, 'Wheel dressing needed, coolant nozzle misaligned', 'Dressed grinding wheel, realigned coolant nozzle, verified surface finish'),
('FAIL-2026-011', 'EQP-MLD-004', 'Mechanical', 'Mold chase sticking causing package flash issues', 'Major', 'Open', 'EMP-030', DATE_SUB(@now, INTERVAL 4 HOUR), NULL, NULL, 180, 'Pending investigation - suspected temperature sensor drift', 'In progress - checking all temperature zones'),
('FAIL-2026-012', 'EQP-WBD-004', 'Mechanical', 'Wire bond pull strength trending low - process capability concern', 'Major', 'Open', 'EMP-031', DATE_SUB(@now, INTERVAL 1 HOUR), NULL, NULL, 60, 'Under investigation - suspected capillary wear', 'Ordered replacement capillary, monitoring pull strength every 30 minutes');

-- ============================================================================
-- 7. UPDATE EQUIPMENT STATUSES
-- ============================================================================

-- Update equipment status to reflect current operational state
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-BGA-20260601-003' WHERE equipment_id IN ('EQP-TST-001');
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-QFN-20260601-002' WHERE equipment_id IN ('EQP-TST-002');
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-DFN-20260601-001' WHERE equipment_id IN ('EQP-TST-003');
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-DFN-20260601-002' WHERE equipment_id IN ('EQP-TST-004');

UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-QFN-20260601-003' WHERE equipment_id IN ('EQP-TRF-001');
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-QFP-20260601-001' WHERE equipment_id IN ('EQP-TRF-002');

UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-BGA-20260601-004' WHERE equipment_id IN ('EQP-LMK-001');
UPDATE master_equipment SET status = 'Available', current_lot_id = NULL WHERE equipment_id IN ('EQP-LMK-002');

UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-BGA-20260602-003' WHERE equipment_id IN ('EQP-PLT-002');
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-SOP-20260601-002' WHERE equipment_id IN ('EQP-PLT-003');
UPDATE master_equipment SET status = 'Available', current_lot_id = NULL WHERE equipment_id IN ('EQP-PLT-001');

UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-QFN-20260601-002' WHERE equipment_id IN ('EQP-MLD-002');
UPDATE master_equipment SET status = 'Available', current_lot_id = NULL WHERE equipment_id IN ('EQP-MLD-001', 'EQP-MLD-003');

UPDATE master_equipment SET status = 'Maintenance', current_lot_id = NULL WHERE equipment_id IN ('EQP-MLD-004');

UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-BGA-20260601-003' WHERE equipment_id IN ('EQP-WBD-003');
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-QFN-20260602-001' WHERE equipment_id IN ('EQP-WBD-004');
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-SOP-20260602-001' WHERE equipment_id IN ('EQP-WBD-005');
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-BGA-20260602-004' WHERE equipment_id IN ('EQP-WBD-002');
UPDATE master_equipment SET status = 'Available', current_lot_id = NULL WHERE equipment_id IN ('EQP-WBD-001');

UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-BGA-20260602-004' WHERE equipment_id IN ('EQP-DAB-002');
UPDATE master_equipment SET status = 'Available', current_lot_id = NULL WHERE equipment_id IN ('EQP-DAB-001', 'EQP-DAB-003', 'EQP-DAB-004');

UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-BGA-20260601-004' WHERE equipment_id IN ('EQP-OVN-001');
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-QFN-20260601-003' WHERE equipment_id IN ('EQP-OVN-002');
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-BGA-20260602-004' WHERE equipment_id IN ('EQP-OVN-003');
UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-QFN-20260602-001' WHERE equipment_id IN ('EQP-OVN-004');

UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-BGA-20260602-004' WHERE equipment_id IN ('EQP-DIC-003');
UPDATE master_equipment SET status = 'Available', current_lot_id = NULL WHERE equipment_id IN ('EQP-DIC-001', 'EQP-DIC-002');

UPDATE master_equipment SET status = 'Running', current_lot_id = 'LOT-BGA-20260602-004' WHERE equipment_id IN ('EQP-GRD-002');
UPDATE master_equipment SET status = 'Available', current_lot_id = NULL WHERE equipment_id IN ('EQP-GRD-001');

UPDATE master_equipment SET status = 'Available', current_lot_id = NULL WHERE equipment_id IN ('EQP-PCK-001', 'EQP-PCK-002');