SET @now = NOW();
-- ============================================================================
-- 15. LOTS - 40 lots with various statuses
-- ============================================================================

-- WO-001 lots (BGA Power IC, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-BGA-20260601-001', 'WO-2026-06-001', 'PROD-001', 'BGA-156 Power IC', 'PowerDie-7x7', 'BGA', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 98500, 800, 0, 49500, 300, 'WF-20260520-001'),
('LOT-BGA-20260601-002', 'WO-2026-06-001', 'PROD-001', 'BGA-156 Power IC', 'PowerDie-7x7', 'BGA', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 99200, 500, 0, 49700, 200, 'WF-20260520-002'),
('LOT-BGA-20260602-001', 'WO-2026-06-001', 'PROD-001', 'BGA-156 Power IC', 'PowerDie-7x7', 'BGA', 'BGA-STD', '1.0', 40, 'CURE_DA', 'Hold', 'Assemble', 50000, 0, 'High', 50000, 98000, 1200, 0, 49000, 500, 'WF-20260520-003'),
('LOT-BGA-20260602-002', 'WO-2026-06-001', 'PROD-001', 'BGA-156 Power IC', 'PowerDie-7x7', 'BGA', 'BGA-STD', '1.0', 80, 'LASER_MARK', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 49500, 300, 0, 49500, 300, 'WF-20260521-001'),
('LOT-BGA-20260603-001', 'WO-2026-06-001', 'PROD-001', 'BGA-156 Power IC', 'PowerDie-7x7', 'BGA', 'BGA-STD', '1.0', 110, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 148500, 600, 0, 49200, 400, 'WF-20260521-002'),

-- WO-002 lots (BGA MCU, Completed)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, is_archived, wafer_lot_id) VALUES
('LOT-BGA-20260515-001', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49800, 0, 'Normal', 50000, 49800, 100, 0, 49800, 100, 1, 'WF-20260510-001'),
('LOT-BGA-20260515-002', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49600, 0, 'Normal', 50000, 49600, 200, 0, 49600, 200, 1, 'WF-20260510-002'),
('LOT-BGA-20260516-001', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49700, 0, 'Normal', 50000, 49700, 150, 0, 49700, 150, 1, 'WF-20260510-003'),
('LOT-BGA-20260516-002', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49500, 0, 'Normal', 50000, 49500, 250, 0, 49500, 250, 1, 'WF-20260510-004'),
('LOT-BGA-20260517-001', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49900, 0, 'Normal', 50000, 49900, 50, 0, 49900, 50, 1, 'WF-20260511-001'),
('LOT-BGA-20260517-002', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49800, 0, 'Normal', 50000, 49800, 100, 0, 49800, 100, 1, 'WF-20260511-002'),

-- WO-003 lots (QFN Sensor, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-QFN-20260601-001', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 80, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 248000, 2500, 0, 49600, 300, 'WF-20260518-001'),
('LOT-QFN-20260601-002', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 80, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 249000, 2200, 0, 49700, 250, 'WF-20260518-002'),
('LOT-QFN-20260602-001', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 30, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 149000, 1800, 0, 49800, 150, 'WF-20260518-003'),
('LOT-QFN-20260602-002', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 30, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 148500, 2000, 0, 49500, 400, 'WF-20260519-001'),
('LOT-QFN-20260603-001', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 10, 'DIE_ATTACH', 'Waiting', 'Assemble', 50000, 0, 'Normal', 50000, 198000, 2100, 0, 49500, 300, 'WF-20260519-002'),
('LOT-QFN-20260603-002', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 10, 'DIE_ATTACH', 'Waiting', 'Assemble', 50000, 0, 'Normal', 50000, 198500, 2000, 0, 49600, 250, 'WF-20260519-003'),

-- WO-005 lots (SOP MOSFET, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-SOP-20260601-001', 'WO-2026-06-005', 'PROD-007', 'SOP-8 MOSFET', 'MOS-Die-3x2', 'SOP', 'SOP-STD', '1.0', 90, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 549000, 8000, 0, 49800, 200, 'WF-20260515-001'),
('LOT-SOP-20260601-002', 'WO-2026-06-005', 'PROD-007', 'SOP-8 MOSFET', 'MOS-Die-3x2', 'SOP', 'SOP-STD', '1.0', 60, 'PLATE', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 548500, 8200, 0, 49700, 250, 'WF-20260515-002'),
('LOT-SOP-20260602-001', 'WO-2026-06-005', 'PROD-007', 'SOP-8 MOSFET', 'MOS-Die-3x2', 'SOP', 'SOP-STD', '1.0', 30, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 548000, 8500, 0, 49600, 300, 'WF-20260515-003'),
('LOT-SOP-20260602-002', 'WO-2026-06-005', 'PROD-007', 'SOP-8 MOSFET', 'MOS-Die-3x2', 'SOP', 'SOP-STD', '1.0', 30, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 547500, 8800, 0, 49500, 350, 'WF-20260516-001'),

-- WO-006 lots (DFN LDO, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-DFN-20260601-001', 'WO-2026-06-006', 'PROD-009', 'DFN-6 LDO', 'LDO-Die-1x1', 'DFN', 'DFN-STD', '1.0', 80, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 1248000, 15000, 0, 49900, 100, 'WF-20260512-001'),
('LOT-DFN-20260601-002', 'WO-2026-06-006', 'PROD-009', 'DFN-6 LDO', 'LDO-Die-1x1', 'DFN', 'DFN-STD', '1.0', 80, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 1247500, 15200, 0, 49800, 150, 'WF-20260512-002'),
('LOT-DFN-20260602-001', 'WO-2026-06-006', 'PROD-009', 'DFN-6 LDO', 'LDO-Die-1x1', 'DFN', 'DFN-STD', '1.0', 40, 'MOLD', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 1247000, 15500, 0, 49700, 200, 'WF-20260512-003'),
('LOT-DFN-20260603-001', 'WO-2026-06-006', 'PROD-009', 'DFN-6 LDO', 'LDO-Die-1x1', 'DFN', 'DFN-STD', '1.0', 10, 'DIE_ATTACH', 'Waiting', 'Assemble', 50000, 0, 'Normal', 50000, 1246000, 16000, 0, 49500, 300, 'WF-20260513-001'),

-- WO-009 lots (BGA WiFi, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-BGA-20260601-003', 'WO-2026-06-009', 'PROD-015', 'BGA-64 WiFi Module', 'WiFi-Die-5x5', 'BGA', 'BGA-STD', '1.0', 110, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 198000, 4000, 0, 49500, 400, 'WF-20260522-001'),
('LOT-BGA-20260602-003', 'WO-2026-06-009', 'PROD-015', 'BGA-64 WiFi Module', 'WiFi-Die-5x5', 'BGA', 'BGA-STD', '1.0', 90, 'PLATE', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 197500, 4200, 0, 49400, 450, 'WF-20260522-002'),
('LOT-BGA-20260603-002', 'WO-2026-06-009', 'PROD-015', 'BGA-64 WiFi Module', 'WiFi-Die-5x5', 'BGA', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 197000, 4500, 0, 49200, 500, 'WF-20260523-001'),
('LOT-BGA-20260603-003', 'WO-2026-06-009', 'PROD-015', 'BGA-64 WiFi Module', 'WiFi-Die-5x5', 'BGA', 'BGA-STD', '1.0', 20, 'DIE_ATTACH', 'Waiting', 'Assemble', 50000, 0, 'Normal', 50000, 196500, 4800, 0, 49100, 550, 'WF-20260523-002'),

-- WO-011 lots (BGA SoC, InProgress, Auto grade)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, grade, wafer_lot_id) VALUES
('LOT-BGA-20260601-004', 'WO-2026-06-011', 'PROD-019', 'BGA-144 SoC', 'SoC-Die-8x8', 'BGA', 'BGA-STD', '1.0', 80, 'LASER_MARK', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 50000, 800, 0, 49600, 300, 'A', 'WF-20260525-001'),
('LOT-BGA-20260602-004', 'WO-2026-06-011', 'PROD-019', 'BGA-144 SoC', 'SoC-Die-8x8', 'BGA', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 49500, 900, 0, 49300, 350, 'A', 'WF-20260525-002'),

-- WO-012 lots (QFP MCU, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-QFP-20260601-001', 'WO-2026-06-012', 'PROD-021', 'QFP-44 MCU', 'MCU-Die-5x5', 'QFP', 'QFP-STD', '1.0', 80, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 148000, 5000, 0, 49000, 500, 'WF-20260520-004'),
('LOT-QFP-20260602-001', 'WO-2026-06-012', 'PROD-021', 'QFP-44 MCU', 'MCU-Die-5x5', 'QFP', 'QFP-STD', '1.0', 40, 'MOLD', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 147500, 5200, 0, 48800, 550, 'WF-20260520-005'),
('LOT-QFP-20260603-001', 'WO-2026-06-012', 'PROD-021', 'QFP-44 MCU', 'MCU-Die-5x5', 'QFP', 'QFP-STD', '1.0', 20, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 147000, 5500, 0, 48500, 600, 'WF-20260521-003'),
('LOT-QFP-20260603-002', 'WO-2026-06-012', 'PROD-021', 'QFP-44 MCU', 'MCU-Die-5x5', 'QFP', 'QFP-STD', '1.0', 10, 'DIE_ATTACH', 'Waiting', 'Assemble', 50000, 0, 'Normal', 50000, 146500, 5800, 0, 48200, 650, 'WF-20260521-004'),

-- WO-015 lots (QFN Temp Sensor Auto, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, grade, wafer_lot_id) VALUES
('LOT-QFN-20260601-003', 'WO-2026-06-015', 'PROD-045', 'QFN-20 Temp Sensor Auto', 'Temp-Die-2x2-Auto', 'QFN', 'QFN-STD', '1.0', 70, 'SINGULATE', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 49800, 500, 0, 49700, 200, 'A', 'WF-20260528-001'),
('LOT-QFN-20260602-003', 'WO-2026-06-015', 'PROD-045', 'QFN-20 Temp Sensor Auto', 'Temp-Die-2x2-Auto', 'QFN', 'QFN-STD', '1.0', 30, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 49500, 600, 0, 49400, 250, 'A', 'WF-20260528-002'),
('LOT-QFN-20260603-003', 'WO-2026-06-015', 'PROD-045', 'QFN-20 Temp Sensor Auto', 'Temp-Die-2x2-Auto', 'QFN', 'QFN-STD', '1.0', 10, 'DIE_ATTACH', 'Waiting', 'Assemble', 50000, 0, 'Normal', 50000, 49200, 700, 0, 49100, 300, 'A', 'WF-20260529-001');

-- ============================================================================
-- 16. LOT STEP RECORDS - Partial steps for key lots
-- ============================================================================

-- LOT-BGA-20260601-001: Completed up to WIRE_BOND (step 50)
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-LOT-BGA-001-S10', 'LOT-BGA-20260601-001', 'BGA-STD', '1.0', 10, 'GRIND', '晶圆减薄', 'Completed', 'EQP-GRD-001', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', 50000, 49800, 100, 100, 'RCP-GRD-STD-001'),
('SR-LOT-BGA-001-S20', 'LOT-BGA-20260601-001', 'BGA-STD', '1.0', 20, 'DICE', '晶圆切割', 'Completed', 'EQP-DIC-001', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-021', 49800, 49600, 100, 100, 'RCP-DIC-STD-001'),
('SR-LOT-BGA-001-S30', 'LOT-BGA-20260601-001', 'BGA-STD', '1.0', 30, 'DIE_ATTACH', '贴片', 'Completed', 'EQP-DAB-001', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', 49600, 49400, 100, 100, 'RCP-DAB-BGA-001'),
('SR-LOT-BGA-001-S40', 'LOT-BGA-20260601-001', 'BGA-STD', '1.0', 40, 'CURE_DA', '贴片固化', 'Completed', 'EQP-OVN-001', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', 49400, 49300, 100, 0, 'RCP-OVN-DA-001'),
('SR-LOT-BGA-001-S50', 'LOT-BGA-20260601-001', 'BGA-STD', '1.0', 50, 'WIRE_BOND', '引线键合', 'InProgress', 'EQP-WBD-001', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', NULL, NULL, 49300, 0, 0, 0, 'RCP-WBD-BGA-001');

-- LOT-BGA-20260601-002: Similar
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-LOT-BGA-002-S10', 'LOT-BGA-20260601-002', 'BGA-STD', '1.0', 10, 'GRIND', '晶圆减薄', 'Completed', 'EQP-GRD-002', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-020', 50000, 49850, 80, 70, 'RCP-GRD-STD-001'),
('SR-LOT-BGA-002-S20', 'LOT-BGA-20260601-002', 'BGA-STD', '1.0', 20, 'DICE', '晶圆切割', 'Completed', 'EQP-DIC-002', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-021', 49850, 49700, 80, 70, 'RCP-DIC-STD-001'),
('SR-LOT-BGA-002-S30', 'LOT-BGA-20260601-002', 'BGA-STD', '1.0', 30, 'DIE_ATTACH', '贴片', 'Completed', 'EQP-DAB-002', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', 49700, 49550, 80, 70, 'RCP-DAB-BGA-001'),
('SR-LOT-BGA-002-S40', 'LOT-BGA-20260601-002', 'BGA-STD', '1.0', 40, 'CURE_DA', '贴片固化', 'Completed', 'EQP-OVN-002', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', 49550, 49500, 50, 0, 'RCP-OVN-DA-001'),
('SR-LOT-BGA-002-S50', 'LOT-BGA-20260601-002', 'BGA-STD', '1.0', 50, 'WIRE_BOND', '引线键合', 'InProgress', 'EQP-WBD-002', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', NULL, NULL, 49500, 0, 0, 0, 'RCP-WBD-BGA-001');

-- LOT-QFN-20260601-001: Up to TEST_FT
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id, test_program) VALUES
('SR-LOT-QFN-001-S10', 'LOT-QFN-20260601-001', 'QFN-STD', '1.0', 10, 'DIE_ATTACH', '贴片', 'Completed', 'EQP-DAB-003', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-020', 50000, 49800, 100, 100, 'RCP-DAB-QFN-001', NULL),
('SR-LOT-QFN-001-S20', 'LOT-QFN-20260601-001', 'QFN-STD', '1.0', 20, 'CURE_DA', '贴片固化', 'Completed', 'EQP-OVN-003', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-022', 49800, 49700, 100, 0, 'RCP-OVN-DA-001', NULL),
('SR-LOT-QFN-001-S30', 'LOT-QFN-20260601-001', 'QFN-STD', '1.0', 30, 'WIRE_BOND', '引线键合', 'Completed', 'EQP-WBD-003', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-021', 49700, 49500, 100, 100, 'RCP-WBD-QFN-001', NULL),
('SR-LOT-QFN-001-S40', 'LOT-QFN-20260601-001', 'QFN-STD', '1.0', 40, 'MOLD', '塑封', 'Completed', 'EQP-MLD-001', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', 49500, 49300, 100, 100, 'RCP-MLD-QFN-001', NULL),
('SR-LOT-QFN-001-S50', 'LOT-QFN-20260601-001', 'QFN-STD', '1.0', 50, 'CURE', '后固化', 'Completed', 'EQP-OVN-001', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-022', 49300, 49200, 100, 0, 'RCP-OVN-CURE-001', NULL),
('SR-LOT-QFN-001-S60', 'LOT-QFN-20260601-001', 'QFN-STD', '1.0', 60, 'PLATE', '电镀', 'Completed', 'EQP-PLT-001', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-020', 49200, 49100, 100, 0, 'RCP-PLT-QFN-001', NULL),
('SR-LOT-QFN-001-S70', 'LOT-QFN-20260601-001', 'QFN-STD', '1.0', 70, 'SINGULATE', '切筋成型', 'Completed', 'EQP-TRF-001', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', 49100, 49000, 100, 0, 'RCP-TRF-BGA-001', NULL),
('SR-LOT-QFN-001-S80', 'LOT-QFN-20260601-001', 'QFN-STD', '1.0', 80, 'TEST_FT', '成品测试', 'InProgress', 'EQP-TST-001', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-028', NULL, NULL, 49000, 0, 0, 0, 'RCP-TST-QFN-001', 'TP-QFN32-SEN-V2.1');

-- LOT-SOP-20260601-001: Up to TEST_FT
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, track_out_time, track_out_operator, input_qty, pass_qty, fail_qty, scrap_qty, recipe_id) VALUES
('SR-LOT-SOP-001-S10', 'LOT-SOP-20260601-001', 'SOP-STD', '1.0', 10, 'DIE_ATTACH', '贴片', 'Completed', 'EQP-DAB-004', DATE_SUB(@now, INTERVAL 9 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 9 DAY), 'EMP-020', 50000, 49800, 100, 100, 'RCP-DAB-SOP-001'),
('SR-LOT-SOP-001-S20', 'LOT-SOP-20260601-001', 'SOP-STD', '1.0', 20, 'CURE_DA', '贴片固化', 'Completed', 'EQP-OVN-004', DATE_SUB(@now, INTERVAL 8 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 8 DAY), 'EMP-022', 49800, 49700, 100, 0, 'RCP-OVN-DA-001'),
('SR-LOT-SOP-001-S30', 'LOT-SOP-20260601-001', 'SOP-STD', '1.0', 30, 'WIRE_BOND', '引线键合', 'Completed', 'EQP-WBD-004', DATE_SUB(@now, INTERVAL 8 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-021', 49700, 49500, 100, 100, 'RCP-WBD-SOP-001'),
('SR-LOT-SOP-001-S40', 'LOT-SOP-20260601-001', 'SOP-STD', '1.0', 40, 'MOLD', '塑封', 'Completed', 'EQP-MLD-002', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 7 DAY), 'EMP-020', 49500, 49300, 100, 100, 'RCP-MLD-QFN-001'),
('SR-LOT-SOP-001-S50', 'LOT-SOP-20260601-001', 'SOP-STD', '1.0', 50, 'CURE', '后固化', 'Completed', 'EQP-OVN-002', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-022', DATE_SUB(@now, INTERVAL 6 DAY), 'EMP-022', 49300, 49200, 100, 0, 'RCP-OVN-CURE-001'),
('SR-LOT-SOP-001-S60', 'LOT-SOP-20260601-001', 'SOP-STD', '1.0', 60, 'PLATE', '电镀', 'Completed', 'EQP-PLT-002', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', DATE_SUB(@now, INTERVAL 5 DAY), 'EMP-020', 49200, 49100, 100, 0, 'RCP-PLT-SOP-001'),
('SR-LOT-SOP-001-S70', 'LOT-SOP-20260601-001', 'SOP-STD', '1.0', 70, 'TRIM_FORM', '切筋成型', 'Completed', 'EQP-TRF-002', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 4 DAY), 'EMP-021', 49100, 49000, 100, 0, 'RCP-TRF-SOP-001'),
('SR-LOT-SOP-001-S80', 'LOT-SOP-20260601-001', 'SOP-STD', '1.0', 80, 'LASER_MARK', '激光打标', 'Completed', 'EQP-LMK-001', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-021', 49000, 48900, 100, 0, 'RCP-LMK-BGA-001'),
('SR-LOT-SOP-001-S90', 'LOT-SOP-20260601-001', 'SOP-STD', '1.0', 90, 'TEST_FT', '成品测试', 'InProgress', 'EQP-TST-002', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-028', NULL, NULL, 48900, 0, 0, 0, 'RCP-TST-SOP-001');

-- LOT-BGA-20260603-001: At TEST_FT
INSERT IGNORE INTO prod_lot_step (record_id, lot_id, route_id, route_version, step_seq, step_code, step_name, status, track_in_equipment, track_in_time, track_in_operator, input_qty, recipe_id) VALUES
('SR-LOT-BGA-003-S10', 'LOT-BGA-20260603-001', 'BGA-STD', '1.0', 10, 'GRIND', '晶圆减薄', 'Completed', 'EQP-GRD-001', DATE_SUB(@now, INTERVAL 3 DAY), 'EMP-020', 50000, 'RCP-GRD-STD-001'),
('SR-LOT-BGA-003-S20', 'LOT-BGA-20260603-001', 'BGA-STD', '1.0', 20, 'DICE', '晶圆切割', 'Completed', 'EQP-DIC-003', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-021', 49800, 'RCP-DIC-STD-001'),
('SR-LOT-BGA-003-S30', 'LOT-BGA-20260603-001', 'BGA-STD', '1.0', 30, 'DIE_ATTACH', '贴片', 'Completed', 'EQP-DAB-001', DATE_SUB(@now, INTERVAL 2 DAY), 'EMP-020', 49600, 'RCP-DAB-BGA-001'),
('SR-LOT-BGA-003-S40', 'LOT-BGA-20260603-001', 'BGA-STD', '1.0', 40, 'CURE_DA', '贴片固化', 'Completed', 'EQP-OVN-001', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-022', 49400, 'RCP-OVN-DA-001'),
('SR-LOT-BGA-003-S50', 'LOT-BGA-20260603-001', 'BGA-STD', '1.0', 50, 'WIRE_BOND', '引线键合', 'Completed', 'EQP-WBD-001', DATE_SUB(@now, INTERVAL 1 DAY), 'EMP-021', 49300, 'RCP-WBD-BGA-001'),
('SR-LOT-BGA-003-S60', 'LOT-BGA-20260603-001', 'BGA-STD', '1.0', 60, 'MOLD', '塑封', 'Completed', 'EQP-MLD-001', DATE_SUB(@now, INTERVAL 12 HOUR), 'EMP-020', 49100, 'RCP-MLD-BGA-001'),
('SR-LOT-BGA-003-S70', 'LOT-BGA-20260603-001', 'BGA-STD', '1.0', 70, 'CURE', '后固化', 'Completed', 'EQP-OVN-003', DATE_SUB(@now, INTERVAL 8 HOUR), 'EMP-022', 49000, 'RCP-OVN-CURE-001'),
('SR-LOT-BGA-003-S80', 'LOT-BGA-20260603-001', 'BGA-STD', '1.0', 80, 'LASER_MARK', '激光打标', 'Completed', 'EQP-LMK-002', DATE_SUB(@now, INTERVAL 6 HOUR), 'EMP-021', 48900, 'RCP-LMK-BGA-001'),
('SR-LOT-BGA-003-S90', 'LOT-BGA-20260603-001', 'BGA-STD', '1.0', 90, 'PLATE', '电镀', 'Completed', 'EQP-PLT-001', DATE_SUB(@now, INTERVAL 4 HOUR), 'EMP-020', 48800, 'RCP-PLT-BGA-001'),
('SR-LOT-BGA-003-S100', 'LOT-BGA-20260603-00