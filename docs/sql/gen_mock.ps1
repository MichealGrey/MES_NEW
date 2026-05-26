# Generate mes_mock_data.sql
$lines = @()
$lines += '-- ============================================================'
$lines += '-- MES V3 Mock Data - Part 2 (Production Data)'
$lines += '-- ============================================================'
$lines += ''
$lines += '-- Yield Rules'
$lines += "INSERT INTO \`master_yield_rule\` (\`rule_id\`, \`route_id\`, \`step_code\`, \`yield_threshold\`, \`action_type\`, \`notify_role\`, \`is_active\`) VALUES"
$lines += "('YLD-QFN-DA', 'QFN-STD:2.0', 'DA', 99.00, 'AutoHold', 'QA', 1),"
$lines += "('YLD-QFN-WB', 'QFN-STD:2.0', 'WB', 99.50, 'AutoHold', 'QA', 1),"
$lines += "('YLD-QFN-MOLD', 'QFN-STD:2.0', 'MOLD', 99.00, 'AutoHold', 'QA', 1),"
$lines += "('YLD-QFN-FT', 'QFN-STD:2.0', 'FT', 95.00, 'AutoHold', 'QA', 1),"
$lines += "('YLD-SOP-DA', 'SOP-STD:1.0', 'DA', 99.00, 'AutoHold', 'QA', 1),"
$lines += "('YLD-SOP-WB', 'SOP-STD:1.0', 'WB', 99.50, 'AutoHold', 'QA', 1),"
$lines += "('YLD-SOP-FT', 'SOP-STD:1.0', 'FT', 96.00, 'AutoHold', 'QA', 1),"
$lines += "('YLD-BGA-DA', 'BGA-STD:1.0', 'DA', 98.50, 'AutoHold', 'QA', 1),"
$lines += "('YLD-BGA-WB', 'BGA-STD:1.0', 'WB', 99.00, 'AutoHold', 'QA', 1),"
$lines += "('YLD-BGA-FT', 'BGA-STD:1.0', 'FT', 94.00, 'AutoHold', 'QA', 1);"
$lines += ''

# Work Orders
$lines += '-- ============================================================'
$lines += '-- 三、生产执行数据 - 工单'
$lines += '-- ============================================================'
$lines += ''
$lines += "INSERT INTO \`prod_work_order\` (\`order_id\`, \`product_id\`, \`product_name\`, \`route_id\`, \`route_name\`, \`die_name\`, \`package_type\`, \`planned_qty\`, \`completed_qty\`, \`wafer_qty\`, \`unit_qty\`, \`customer_id\`, \`customer_name\`, \`customer_pn\`, \`internal_pn\`, \`priority\`, \`status\`, \`creator\`, \`planned_start_date\`, \`planned_end_date\`, \`actual_start_date\`, \`actual_end_date\`, \`target_cp_yield\`, \`target_ft_yield\`, \`remark\`) VALUES"
$lines += "('WO-2026001', 'PROD-QFN88', 'QFN-88 控制器', 'QFN-STD:2.0', 'QFN 标准路线', 'CTRL-2024', 'QFN', 50000, 12000, 20, 50000, 'CUST-AUTO', '某汽车电子', 'AE-QFN88-001', 'PN-QFN88-STD', 'High', 'Processing', 'USR-013', DATE_SUB(NOW(), INTERVAL 10 DAY), DATE_ADD(NOW(), INTERVAL 5 DAY), DATE_SUB(NOW(), INTERVAL 8 DAY), NULL, 99.00, 98.00, '汽车电子紧急订单'),"
$lines += "('WO-2026002', 'PROD-SOP16', 'SOP-16 电源IC', 'SOP-STD:1.0', 'SOP 标准路线', 'PWR-500', 'SOP', 30000, 30000, 12, 30000, 'CUST-IND', '某工业客户', 'IND-SOP16-002', 'PN-SOP16-STD', 'Normal', 'Completed', 'USR-013', DATE_SUB(NOW(), INTERVAL 20 DAY), DATE_SUB(NOW(), INTERVAL 5 DAY), DATE_SUB(NOW(), INTERVAL 18 DAY), DATE_SUB(NOW(), INTERVAL 5 DAY), 99.00, 97.00, NULL),"
$lines += "('WO-2026003', 'PROD-BGA256', 'BGA-256 处理器', 'BGA-STD:1.0', 'BGA 标准路线', 'CPU-3000', 'BGA', 10000, 3000, 5, 10000, 'CUST-AUTO', '某汽车电子', 'AE-BGA256-003', 'PN-BGA256-HI', 'High', 'Processing', 'USR-013', DATE_SUB(NOW(), INTERVAL 5 DAY), DATE_ADD(NOW(), INTERVAL 10 DAY), DATE_SUB(NOW(), INTERVAL 3 DAY), NULL, 98.50, 94.00, '高端处理器'),"
$lines += "('WO-2026004', 'PROD-QFN48', 'QFN-48 传感器', 'QFN48-STD:1.0', 'QFN-48 标准路线', 'SENS-100', 'QFN', 40000, 28000, 16, 40000, 'CUST-CON', '某消费电子', 'CE-QFN48-004', 'PN-QFN48-STD', 'Normal', 'Processing', 'USR-013', DATE_SUB(NOW(), INTERVAL 12 DAY), DATE_ADD(NOW(), INTERVAL 3 DAY), DATE_SUB(NOW(), INTERVAL 10 DAY), NULL, 99.00, 97.50, NULL),"
$lines += "('WO-2026005', 'PROD-SOP8', 'SOP-8 MOSFET', 'SOP8-STD:1.0', 'SOP-8 标准路线', 'MOS-200', 'SOP', 60000, 60000, 24, 60000, 'CUST-IND', '某工业客户', 'IND-SOP8-005', 'PN-SOP8-STD', 'Low', 'Completed', 'USR-013', DATE_SUB(NOW(), INTERVAL 30 DAY), DATE_SUB(NOW(), INTERVAL 10 DAY), DATE_SUB(NOW(), INTERVAL 28 DAY), DATE_SUB(NOW(), INTERVAL 10 DAY), 99.00, 98.00, NULL),"
$lines += "('WO-2026006', 'PROD-BGA64', 'BGA-64 存储器', 'BGA64-STD:1.0', 'BGA-64 标准路线', 'MEM-500', 'BGA', 15000, 0, 6, 15000, 'CUST-AUTO', '某汽车电子', 'AE-BGA64-006', 'PN-BGA64-STD', 'Normal', 'Created', 'USR-013', DATE_ADD(NOW(), INTERVAL 2 DAY), DATE_ADD(NOW(), INTERVAL 15 DAY), NULL, NULL, 98.50, 95.00, NULL),"
$lines += "('WO-2026007', 'PROD-QFN64', 'QFN-64 驱动器', 'QFN64-STD:1.0', 'QFN-64 标准路线', 'DRV-300', 'QFN', 25000, 15000, 10, 25000, 'CUST-AUTO', '某汽车电子', 'AE-QFN64-007', 'PN-QFN64-STD', 'High', 'Processing', 'USR-013', DATE_SUB(NOW(), INTERVAL 7 DAY), DATE_ADD(NOW(), INTERVAL 8 DAY), DATE_SUB(NOW(), INTERVAL 5 DAY), NULL, 99.00, 97.00, NULL),"
$lines += "('WO-2026008', 'PROD-SOP14', 'SOP-14 运放', 'SOP14-STD:1.0', 'SOP-14 标准路线', 'OP-100', 'SOP', 35000, 20000, 14, 35000, 'CUST-CON', '某消费电子', 'CE-SOP14-008', 'PN-SOP14-STD', 'Normal', 'Processing', 'USR-013', DATE_SUB(NOW(), INTERVAL 8 DAY), DATE_ADD(NOW(), INTERVAL 7 DAY), DATE_SUB(NOW(), INTERVAL 6 DAY), NULL, 99.00, 97.50, NULL);"
$lines += ''

# Lots - Batch 1: QFN88 Normal Production
$lines += '-- ============================================================'
$lines += '-- 批次数据'
$lines += '-- ============================================================'
$lines += ''
$lines += '-- LOT-001: QFN88 正常生产中'
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`) VALUES"
$lines += "('LOT-001', 'WO-2026001', 'PROD-QFN88', 'QFN-88 控制器', 'CTRL-2024', 'QFN', 'QFN-STD:2.0', '2.0', 5, 'MOLD', 'Processing', 10000, 400, 'High', 'Magazine', 'MAG-002', 'WFR-20260517-001', 10000, 0, 150, 0, 0, 0, 0, 0, 'A', NULL, NULL, NULL, 0, 0);"
$lines += ''

$lines += "INSERT INTO \`prod_lot_step\` (\`record_id\`, \`lot_id\`, \`route_id\`, \`route_version\`, \`step_seq\`, \`step_code\`, \`step_name\`, \`status\`, \`track_in_equipment\`, \`track_in_carrier\`, \`track_in_recipe\`, \`track_in_time\`, \`track_in_operator\`, \`track_out_time\`, \`track_out_operator\`, \`input_qty\`, \`pass_qty\`, \`fail_qty\`, \`scrap_qty\`, \`rework_qty\`, \`hold_qty\`, \`pending_qty\`, \`recipe_id\`, \`remark\`) VALUES"
$lines += "('STEP-001-1', 'LOT-001', 'QFN-STD:2.0', '2.0', 1, 'SAW', '晶圆切割', 'Completed', 'SAW-001', 'FOUP-002', 'REC-SAW-QFN-001', DATE_SUB(NOW(), INTERVAL 5 DAY), 'USR-010', DATE_SUB(NOW(), INTERVAL 5 DAY), 'USR-010', 10000, 9950, 20, 30, 0, 0, 0, 'REC-SAW-QFN-001', NULL),"
$lines += "('STEP-001-2', 'LOT-001', 'QFN-STD:2.0', '2.0', 2, 'DA', '芯片贴装', 'Completed', 'DA-001', 'MAG-002', 'REC-DA-QFN-001', DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-011', 9950, 9900, 20, 30, 0, 0, 0, 'REC-DA-QFN-001', NULL),"
$lines += "('STEP-001-3', 'LOT-001', 'QFN-STD:2.0', '2.0', 3, 'CURE', '固化', 'Completed', NULL, NULL, NULL, DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-011', 9900, 9870, 10, 20, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-001-4', 'LOT-001', 'QFN-STD:2.0', '2.0', 4, 'WB', '焊线', 'Completed', 'WB-002', 'MAG-006', 'REC-WB-QFN-001', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-012', DATE_SUB(NOW(), INTERVAL 2 DAY), 'USR-012', 9870, 9850, 10, 10, 0, 0, 0, 'REC-WB-QFN-001', NULL),"
$lines += "('STEP-001-5', 'LOT-001', 'QFN-STD:2.0', '2.0', 5, 'MOLD', '塑封', 'Processing', 'MOLD-002', NULL, 'REC-MOLD-QFN-001', NOW(), 'USR-016', NULL, NULL, 9850, 0, 0, 0, 0, 0, 9850, 'REC-MOLD-QFN-001', NULL);"
$lines += ''

# LOT-002: QFN88 第二批
$lines += "-- LOT-002: QFN88 第二批"
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`) VALUES"
$lines += "('LOT-002', 'WO-2026001', 'PROD-QFN88', 'QFN-88 控制器', 'CTRL-2024', 'QFN', 'QFN-STD:2.0', '2.0', 3, 'CURE', 'Processing', 10000, 400, 'High', 'Magazine', 'MAG-003', 'WFR-20260517-002', 10000, 0, 80, 0, 0, 0, 0, 0, 'A', NULL, NULL, NULL, 0, 0);"
$lines += ''
$lines += "INSERT INTO \`prod_lot_step\` (\`record_id\`, \`lot_id\`, \`route_id\`, \`route_version\`, \`step_seq\`, \`step_code\`, \`step_name\`, \`status\`, \`track_in_equipment\`, \`track_in_carrier\`, \`track_in_recipe\`, \`track_in_time\`, \`track_in_operator\`, \`track_out_time\`, \`track_out_operator\`, \`input_qty\`, \`pass_qty\`, \`fail_qty\`, \`scrap_qty\`, \`rework_qty\`, \`hold_qty\`, \`pending_qty\`, \`recipe_id\`, \`remark\`) VALUES"
$lines += "('STEP-002-1', 'LOT-002', 'QFN-STD:2.0', '2.0', 1, 'SAW', '晶圆切割', 'Completed', 'SAW-002', 'FOUP-006', 'REC-SAW-QFN-001', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-018', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-018', 10000, 9960, 15, 25, 0, 0, 0, 'REC-SAW-QFN-001', NULL),"
$lines += "('STEP-002-2', 'LOT-002', 'QFN-STD:2.0', '2.0', 2, 'DA', '芯片贴装', 'Completed', 'DA-003', 'MAG-007', 'REC-DA-QFN-001', DATE_SUB(NOW(), INTERVAL 2 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 2 DAY), 'USR-011', 9960, 9930, 15, 15, 0, 0, 0, 'REC-DA-QFN-001', NULL),"
$lines += "('STEP-002-3', 'LOT-002', 'QFN-STD:2.0', '2.0', 3, 'CURE', 'Processing', NULL, NULL, NULL, NOW(), 'USR-011', NULL, NULL, 9930, 0, 0, 0, 0, 0, 9930, NULL, NULL);"
$lines += ''

# LOT-003: SOP16 已完成
$lines += "-- LOT-003: SOP16 已完成"
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`) VALUES"
$lines += "('LOT-003', 'WO-2026002', 'PROD-SOP16', 'SOP-16 电源IC', 'PWR-500', 'SOP', 'SOP-STD:1.0', '1.0', 9, 'PACK', 'Completed', 10000, 500, 'Normal', 'Tray', 'TRAY-004', 'WFR-20260505-001', 10000, 9850, 80, 0, 0, 0, 0, 0, 'A', NULL, 'Bin1', 'Pass', 9850, 150);"
$lines += ''
$lines += "INSERT INTO \`prod_lot_step\` (\`record_id\`, \`lot_id\`, \`route_id\`, \`route_version\`, \`step_seq\`, \`step_code\`, \`step_name\`, \`status\`, \`track_in_equipment\`, \`track_in_carrier\`, \`track_in_recipe\`, \`track_in_time\`, \`track_in_operator\`, \`track_out_time\`, \`track_out_operator\`, \`input_qty\`, \`pass_qty\`, \`fail_qty\`, \`scrap_qty\`, \`rework_qty\`, \`hold_qty\`, \`pending_qty\`, \`recipe_id\`, \`remark\`) VALUES"
$lines += "('STEP-003-1', 'LOT-003', 'SOP-STD:1.0', '1.0', 1, 'SAW', '晶圆切割', 'Completed', 'SAW-001', 'FOUP-001', 'REC-SAW-SOP-001', DATE_SUB(NOW(), INTERVAL 15 DAY), 'USR-010', DATE_SUB(NOW(), INTERVAL 15 DAY), 'USR-010', 10000, 9970, 10, 20, 0, 0, 0, 'REC-SAW-SOP-001', NULL),"
$lines += "('STEP-003-2', 'LOT-003', 'SOP-STD:1.0', '1.0', 2, 'DA', '芯片贴装', 'Completed', 'DA-001', 'MAG-001', 'REC-DA-SOP-001', DATE_SUB(NOW(), INTERVAL 14 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 14 DAY), 'USR-011', 9970, 9940, 10, 20, 0, 0, 0, 'REC-DA-SOP-001', NULL),"
$lines += "('STEP-003-3', 'LOT-003', 'SOP-STD:1.0', '1.0', 3, 'WB', '焊线', 'Completed', 'WB-001', 'MAG-002', 'REC-WB-SOP-001', DATE_SUB(NOW(), INTERVAL 13 DAY), 'USR-012', DATE_SUB(NOW(), INTERVAL 13 DAY), 'USR-012', 9940, 9920, 10, 10, 0, 0, 0, 'REC-WB-SOP-001', NULL),"
$lines += "('STEP-003-4', 'LOT-003', 'SOP-STD:1.0', '1.0', 4, 'MOLD', '塑封', 'Completed', 'MOLD-001', NULL, 'REC-MOLD-SOP-001', DATE_SUB(NOW(), INTERVAL 12 DAY), 'USR-016', DATE_SUB(NOW(), INTERVAL 12 DAY), 'USR-016', 9920, 9900, 10, 10, 0, 0, 0, 'REC-MOLD-SOP-001', NULL),"
$lines += "('STEP-003-5', 'LOT-003', 'SOP-STD:1.0', '1.0', 5, 'MARK', '激光打标', 'Completed', 'LASER-001', NULL, NULL, DATE_SUB(NOW(), INTERVAL 11 DAY), 'USR-025', DATE_SUB(NOW(), INTERVAL 11 DAY), 'USR-025', 9900, 9880, 10, 10, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-003-6', 'LOT-003', 'SOP-STD:1.0', '1.0', 6, 'SING', '切割分条', 'Completed', 'SING-001', NULL, NULL, DATE_SUB(NOW(), INTERVAL 10 DAY), 'USR-026', DATE_SUB(NOW(), INTERVAL 10 DAY), 'USR-026', 9880, 9860, 10, 10, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-003-7', 'LOT-003', 'SOP-STD:1.0', '1.0', 7, 'FT', '最终测试', 'Completed', 'FT-001', 'TRAY-002', 'REC-FT-SOP-001', DATE_SUB(NOW(), INTERVAL 9 DAY), 'USR-019', DATE_SUB(NOW(), INTERVAL 8 DAY), 'USR-019', 9860, 9850, 5, 5, 0, 0, 0, 'REC-FT-SOP-001', NULL),"
$lines += "('STEP-003-8', 'LOT-003', 'SOP-STD:1.0', '1.0', 8, 'OQC', '出货检验', 'Completed', 'OQC-001', NULL, NULL, DATE_SUB(NOW(), INTERVAL 7 DAY), 'USR-020', DATE_SUB(NOW(), INTERVAL 6 DAY), 'USR-020', 9850, 9850, 0, 0, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-003-9', 'LOT-003', 'SOP-STD:1.0', '1.0', 9, 'PACK', '包装入库', 'Completed', NULL, NULL, NULL, DATE_SUB(NOW(), INTERVAL 5 DAY), 'USR-004', DATE_SUB(NOW(), INTERVAL 5 DAY), 'USR-004', 9850, 9850, 0, 0, 0, 0, 0, NULL, NULL);"
$lines += ''

# LOT-004: BGA256 Processing
$lines += "-- LOT-004: BGA256 Processing"
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`) VALUES"
$lines += "('LOT-004', 'WO-2026003', 'PROD-BGA256', 'BGA-256 处理器', 'CPU-3000', 'BGA', 'BGA-STD:1.0', '1.0', 6, 'PMC', 'Processing', 3000, 120, 'High', 'Magazine', 'MAG-004', 'WFR-20260520-001', 3000, 0, 60, 0, 0, 0, 0, 0, 'A', NULL, NULL, NULL, 0, 0);"
$lines += ''
$lines += "INSERT INTO \`prod_lot_step\` (\`record_id\`, \`lot_id\`, \`route_id\`, \`route_version\`, \`step_seq\`, \`step_code\`, \`step_name\`, \`status\`, \`track_in_equipment\`, \`track_in_carrier\`, \`track_in_recipe\`, \`track_in_time\`, \`track_in_operator\`, \`track_out_time\`, \`track_out_operator\`, \`input_qty\`, \`pass_qty\`, \`fail_qty\`, \`scrap_qty\`, \`rework_qty\`, \`hold_qty\`, \`pending_qty\`, \`recipe_id\`, \`remark\`) VALUES"
$lines += "('STEP-004-1', 'LOT-004', 'BGA-STD:1.0', '1.0', 1, 'SAW', '晶圆切割', 'Completed', 'SAW-003', 'FOUP-003', 'REC-SAW-BGA-001', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-010', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-010', 3000, 2980, 5, 15, 0, 0, 0, 'REC-SAW-BGA-001', NULL),"
$lines += "('STEP-004-2', 'LOT-004', 'BGA-STD:1.0', '1.0', 2, 'DA', '芯片贴装', 'Completed', 'DA-002', 'MAG-004', 'REC-DA-BGA-001', DATE_SUB(NOW(), INTERVAL 2 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 2 DAY), 'USR-011', 2980, 2960, 5, 15, 0, 0, 0, 'REC-DA-BGA-001', NULL),"
$lines += "('STEP-004-3', 'LOT-004', 'BGA-STD:1.0', '1.0', 3, 'CURE', '固化', 'Completed', NULL, NULL, NULL, DATE_SUB(NOW(), INTERVAL 2 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 1 DAY), 'USR-011', 2960, 2950, 5, 5, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-004-4', 'LOT-004', 'BGA-STD:1.0', '1.0', 4, 'WB', '焊线', 'Completed', 'WB-003', 'MAG-005', 'REC-WB-BGA-001', DATE_SUB(NOW(), INTERVAL 1 DAY), 'USR-012', DATE_SUB(NOW(), INTERVAL 1 DAY), 'USR-012', 2950, 2940, 5, 5, 0, 0, 0, 'REC-WB-BGA-001', NULL),"
$lines += "('STEP-004-5', 'LOT-004', 'BGA-STD:1.0', '1.0', 5, 'MOLD', '塑封', 'Completed', 'MOLD-003', NULL, 'REC-MOLD-BGA-001', DATE_SUB(NOW(), INTERVAL 1 DAY), 'USR-016', NOW(), 'USR-016', 2940, 2920, 5, 15, 0, 0, 0, 'REC-MOLD-BGA-001', NULL),"
$lines += "('STEP-004-6', 'LOT-004', 'BGA-STD:1.0', '1.0', 6, 'PMC', '后固化', 'Processing', NULL, NULL, NULL, NOW(), 'USR-024', NULL, NULL, 2920, 0, 0, 0, 0, 0, 2920, NULL, NULL);"
$lines += ''

# LOT-005: QFN48 Processing
$lines += "-- LOT-005: QFN48 Processing"
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`) VALUES"
$lines += "('LOT-005', 'WO-2026004', 'PROD-QFN48', 'QFN-48 传感器', 'SENS-100', 'QFN', 'QFN48-STD:1.0', '1.0', 4, 'MOLD', 'Processing', 10000, 400, 'Normal', 'Magazine', 'MAG-009', 'WFR-20260515-001', 10000, 0, 100, 0, 0, 0, 0, 0, 'A', NULL, NULL, NULL, 0, 0);"
$lines += ''
$lines += "INSERT INTO \`prod_lot_step\` (\`record_id\`, \`lot_id\`, \`route_id\`, \`route_version\`, \`step_seq\`, \`step_code\`, \`step_name\`, \`status\`, \`track_in_equipment\`, \`track_in_carrier\`, \`track_in_recipe\`, \`track_in_time\`, \`track_in_operator\`, \`track_out_time\`, \`track_out_operator\`, \`input_qty\`, \`pass_qty\`, \`fail_qty\`, \`scrap_qty\`, \`rework_qty\`, \`hold_qty\`, \`pending_qty\`, \`recipe_id\`, \`remark\`) VALUES"
$lines += "('STEP-005-1', 'LOT-005', 'QFN48-STD:1.0', '1.0', 1, 'SAW', '晶圆切割', 'Completed', 'SAW-001', 'FOUP-005', 'REC-SAW-QFN48-001', DATE_SUB(NOW(), INTERVAL 6 DAY), 'USR-010', DATE_SUB(NOW(), INTERVAL 6 DAY), 'USR-010', 10000, 9960, 15, 25, 0, 0, 0, 'REC-SAW-QFN48-001', NULL),"
$lines += "('STEP-005-2', 'LOT-005', 'QFN48-STD:1.0', '1.0', 2, 'DA', '芯片贴装', 'Completed', 'DA-001', 'MAG-009', 'REC-DA-QFN48-001', DATE_SUB(NOW(), INTERVAL 5 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 5 DAY), 'USR-011', 9960, 9930, 10, 20, 0, 0, 0, 'REC-DA-QFN48-001', NULL),"
$lines += "('STEP-005-3', 'LOT-005', 'QFN48-STD:1.0', '1.0', 3, 'WB', '焊线', 'Completed', 'WB-001', 'MAG-010', 'REC-WB-QFN48-001', DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-012', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-012', 9930, 9910, 10, 10, 0, 0, 0, 'REC-WB-QFN48-001', NULL),"
$lines += "('STEP-005-4', 'LOT-005', 'QFN48-STD:1.0', '1.0', 4, 'MOLD', '塑封', 'Processing', 'MOLD-001', NULL, 'REC-MOLD-QFN48-001', NOW(), 'USR-016', NULL, NULL, 9910, 0, 0, 0, 0, 0, 9910, 'REC-MOLD-QFN48-001', NULL);"
$lines += ''

# LOT-006: SOP8 Completed
$lines += "-- LOT-006: SOP8 Completed"
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`) VALUES"
$lines += "('LOT-006', 'WO-2026005', 'PROD-SOP8', 'SOP-8 MOSFET', 'MOS-200', 'SOP', 'SOP8-STD:1.0', '1.0', 8, 'PACK', 'Completed', 15000, 750, 'Low', 'Tray', 'TRAY-006', 'WFR-20260425-001', 15000, 14800, 100, 0, 0, 0, 0, 0, 'A', NULL, 'Bin1', 'Pass', 14800, 200);"
$lines += ''
$lines += "INSERT INTO \`prod_lot_step\` (\`record_id\`, \`lot_id\`, \`route_id\`, \`route_version\`, \`step_seq\`, \`step_code\`, \`step_name\`, \`status\`, \`track_in_equipment\`, \`track_in_carrier\`, \`track_in_recipe\`, \`track_in_time\`, \`track_in_operator\`, \`track_out_time\`, \`track_out_operator\`, \`input_qty\`, \`pass_qty\`, \`fail_qty\`, \`scrap_qty\`, \`rework_qty\`, \`hold_qty\`, \`pending_qty\`, \`recipe_id\`, \`remark\`) VALUES"
$lines += "('STEP-006-1', 'LOT-006', 'SOP8-STD:1.0', '1.0', 1, 'DA', '芯片贴装', 'Completed', 'DA-001', 'MAG-011', 'REC-DA-SOP8-001', DATE_SUB(NOW(), INTERVAL 20 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 20 DAY), 'USR-011', 15000, 14950, 20, 30, 0, 0, 0, 'REC-DA-SOP8-001', NULL),"
$lines += "('STEP-006-2', 'LOT-006', 'SOP8-STD:1.0', '1.0', 2, 'WB', '焊线', 'Completed', 'WB-001', 'MAG-012', 'REC-WB-SOP8-001', DATE_SUB(NOW(), INTERVAL 19 DAY), 'USR-012', DATE_SUB(NOW(), INTERVAL 19 DAY), 'USR-012', 14950, 14920, 10, 20, 0, 0, 0, 'REC-WB-SOP8-001', NULL),"
$lines += "('STEP-006-3', 'LOT-006', 'SOP8-STD:1.0', '1.0', 3, 'MOLD', '塑封', 'Completed', 'MOLD-001', NULL, 'REC-MOLD-SOP8-001', DATE_SUB(NOW(), INTERVAL 18 DAY), 'USR-016', DATE_SUB(NOW(), INTERVAL 18 DAY), 'USR-016', 14920, 14900, 10, 10, 0, 0, 0, 'REC-MOLD-SOP8-001', NULL),"
$lines += "('STEP-006-4', 'LOT-006', 'SOP8-STD:1.0', '1.0', 4, 'MARK', '激光打标', 'Completed', 'LASER-001', NULL, NULL, DATE_SUB(NOW(), INTERVAL 17 DAY), 'USR-025', DATE_SUB(NOW(), INTERVAL 17 DAY), 'USR-025', 14900, 14880, 10, 10, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-006-5', 'LOT-006', 'SOP8-STD:1.0', '1.0', 5, 'SING', '切割分条', 'Completed', 'SING-001', NULL, NULL, DATE_SUB(NOW(), INTERVAL 16 DAY), 'USR-026', DATE_SUB(NOW(), INTERVAL 16 DAY), 'USR-026', 14880, 14860, 10, 10, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-006-6', 'LOT-006', 'SOP8-STD:1.0', '1.0', 6, 'FT', '最终测试', 'Completed', 'FT-001', 'TRAY-006', 'REC-FT-SOP8-001', DATE_SUB(NOW(), INTERVAL 15 DAY), 'USR-019', DATE_SUB(NOW(), INTERVAL 14 DAY), 'USR-019', 14860, 14800, 30, 30, 0, 0, 0, 'REC-FT-SOP8-001', NULL),"
$lines += "('STEP-006-7', 'LOT-006', 'SOP8-STD:1.0', '1.0', 7, 'OQC', '出货检验', 'Completed', 'OQC-001', NULL, NULL, DATE_SUB(NOW(), INTERVAL 13 DAY), 'USR-020', DATE_SUB(NOW(), INTERVAL 12 DAY), 'USR-020', 14800, 14800, 0, 0, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-006-8', 'LOT-006', 'SOP8-STD:1.0', '1.0', 8, 'PACK', '包装入库', 'Completed', NULL, NULL, NULL, DATE_SUB(NOW(), INTERVAL 10 DAY), 'USR-004', DATE_SUB(NOW(), INTERVAL 10 DAY), 'USR-004', 14800, 14800, 0, 0, 0, 0, 0, NULL, NULL);"
$lines += ''

# LOT-007: BGA64 Waiting
$lines += "-- LOT-007: BGA64 Waiting"
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`) VALUES"
$lines += "('LOT-007', 'WO-2026006', 'PROD-BGA64', 'BGA-64 存储器', 'MEM-500', 'BGA', 'BGA64-STD:1.0', '1.0', 0, NULL, 'Waiting', 5000, 200, 'Normal', NULL, NULL, 'WFR-20260525-001', 5000, 0, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, NULL, 0, 0);"
$lines += ''

# LOT-008: QFN64 Processing
$lines += "-- LOT-008: QFN64 Processing"
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`) VALUES"
$lines += "('LOT-008', 'WO-2026007', 'PROD-QFN64', 'QFN-64 驱动器', 'DRV-300', 'QFN', 'QFN64-STD:1.0', '1.0', 7, 'MARK', 'Processing', 8000, 320, 'High', 'Magazine', 'MAG-015', 'WFR-20260518-001', 8000, 0, 120, 0, 0, 0, 0, 0, 'A', NULL, NULL, NULL, 0, 0);"
$lines += ''
$lines += "INSERT INTO \`prod_lot_step\` (\`record_id\`, \`lot_id\`, \`route_id\`, \`route_version\`, \`step_seq\`, \`step_code\`, \`step_name\`, \`status\`, \`track_in_equipment\`, \`track_in_carrier\`, \`track_in_recipe\`, \`track_in_time\`, \`track_in_operator\`, \`track_out_time\`, \`track_out_operator\`, \`input_qty\`, \`pass_qty\`, \`fail_qty\`, \`scrap_qty\`, \`rework_qty\`, \`hold_qty\`, \`pending_qty\`, \`recipe_id\`, \`remark\`) VALUES"
$lines += "('STEP-008-1', 'LOT-008', 'QFN64-STD:1.0', '1.0', 1, 'SAW', '晶圆切割', 'Completed', 'SAW-002', 'FOUP-009', 'REC-SAW-QFN64-001', DATE_SUB(NOW(), INTERVAL 5 DAY), 'USR-018', DATE_SUB(NOW(), INTERVAL 5 DAY), 'USR-018', 8000, 7970, 10, 20, 0, 0, 0, 'REC-SAW-QFN64-001', NULL),"
$lines += "('STEP-008-2', 'LOT-008', 'QFN64-STD:1.0', '1.0', 2, 'DA', '芯片贴装', 'Completed', 'DA-002', 'MAG-015', 'REC-DA-QFN64-001', DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-011', 7970, 7940, 10, 20, 0, 0, 0, 'REC-DA-QFN64-001', NULL),"
$lines += "('STEP-008-3', 'LOT-008', 'QFN64-STD:1.0', '1.0', 3, 'CURE', '固化', 'Completed', NULL, NULL, NULL, DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-011', 7940, 7920, 10, 10, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-008-4', 'LOT-008', 'QFN64-STD:1.0', '1.0', 4, 'WB', '焊线', 'Completed', 'WB-002', 'MAG-016', 'REC-WB-QFN64-001', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-012', DATE_SUB(NOW(), INTERVAL 2 DAY), 'USR-012', 7920, 7900, 10, 10, 0, 0, 0, 'REC-WB-QFN64-001', NULL),"
$lines += "('STEP-008-5', 'LOT-008', 'QFN64-STD:1.0', '1.0', 5, 'MOLD', '塑封', 'Completed', 'MOLD-002', NULL, 'REC-MOLD-QFN64-001', DATE_SUB(NOW(), INTERVAL 2 DAY), 'USR-016', DATE_SUB(NOW(), INTERVAL 1 DAY), 'USR-016', 7900, 7880, 10, 10, 0, 0, 0, 'REC-MOLD-QFN64-001', NULL),"
$lines += "('STEP-008-6', 'LOT-008', 'QFN64-STD:1.0', '1.0', 6, 'PMC', '后固化', 'Completed', NULL, NULL, NULL, DATE_SUB(NOW(), INTERVAL 1 DAY), 'USR-024', NOW(), 'USR-024', 7880, 7860, 10, 10, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-008-7', 'LOT-008', 'QFN64-STD:1.0', '1.0', 7, 'MARK', '激光打标', 'Processing', 'LASER-002', NULL, NULL, NOW(), 'USR-025', NULL, NULL, 7860, 0, 0, 0, 0, 0, 7860, NULL, NULL);"
$lines += ''

# LOT-009: SOP14 Processing
$lines += "-- LOT-009: SOP14 Processing"
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`) VALUES"
$lines += "('LOT-009', 'WO-2026008', 'PROD-SOP14', 'SOP-14 运放', 'OP-100', 'SOP', 'SOP14-STD:1.0', '1.0', 3, 'MOLD', 'Processing', 10000, 500, 'Normal', 'Magazine', 'MAG-018', 'WFR-20260517-003', 10000, 0, 90, 0, 0, 0, 0, 0, 'A', NULL, NULL, NULL, 0, 0);"
$lines += ''
$lines += "INSERT INTO \`prod_lot_step\` (\`record_id\`, \`lot_id\`, \`route_id\`, \`route_version\`, \`step_seq\`, \`step_code\`, \`step_name\`, \`status\`, \`track_in_equipment\`, \`track_in_carrier\`, \`track_in_recipe\`, \`track_in_time\`, \`track_in_operator\`, \`track_out_time\`, \`track_out_operator\`, \`input_qty\`, \`pass_qty\`, \`fail_qty\`, \`scrap_qty\`, \`rework_qty\`, \`hold_qty\`, \`pending_qty\`, \`recipe_id\`, \`remark\`) VALUES"
$lines += "('STEP-009-1', 'LOT-009', 'SOP14-STD:1.0', '1.0', 1, 'DA', '芯片贴装', 'Completed', 'DA-003', 'MAG-018', 'REC-DA-SOP14-001', DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-011', 10000, 9960, 15, 25, 0, 0, 0, 'REC-DA-SOP14-001', NULL),"
$lines += "('STEP-009-2', 'LOT-009', 'SOP14-STD:1.0', '1.0', 2, 'WB', '焊线', 'Completed', 'WB-002', 'MAG-019', 'REC-WB-SOP14-001', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-012', DATE_SUB(NOW(), INTERVAL 2 DAY), 'USR-012', 9960, 9930, 10, 20, 0, 0, 0, 'REC-WB-SOP14-001', NULL),"
$lines += "('STEP-009-3', 'LOT-009', 'SOP14-STD:1.0', '1.0', 3, 'MOLD', '塑封', 'Processing', 'MOLD-002', NULL, 'REC-MOLD-SOP14-001', NOW(), 'USR-016', NULL, NULL, 9930, 0, 0, 0, 0, 0, 9930, 'REC-MOLD-SOP14-001', NULL);"
$lines += ''

# LOT-010: QFN88 Hold (品质Hold)
$lines += "-- LOT-010: QFN88 品质Hold"
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`, \`hold_category\`, \`hold_reason\`, \`hold_time\`, \`hold_operator\`) VALUES"
$lines += "('LOT-010', 'WO-2026001', 'PROD-QFN88', 'QFN-88 控制器', 'CTRL-2024', 'QFN', 'QFN-STD:2.0', '2.0', 4, 'WB', 'Hold', 10000, 400, 'High', 'Magazine', 'MAG-021', 'WFR-20260517-003', 10000, 0, 50, 0, 10000, 0, 0, 0, 'A', NULL, NULL, NULL, 0, 0, 'Quality', 'WireBond 拉力测试不合格', DATE_SUB(NOW(), INTERVAL 1 DAY), 'USR-002');"
$lines += ''
$lines += "INSERT INTO \`prod_lot_step\` (\`record_id\`, \`lot_id\`, \`route_id\`, \`route_version\`, \`step_seq\`, \`step_code\`, \`step_name\`, \`status\`, \`track_in_equipment\`, \`track_in_carrier\`, \`track_in_recipe\`, \`track_in_time\`, \`track_in_operator\`, \`track_out_time\`, \`track_out_operator\`, \`input_qty\`, \`pass_qty\`, \`fail_qty\`, \`scrap_qty\`, \`rework_qty\`, \`hold_qty\`, \`pending_qty\`, \`recipe_id\`, \`remark\`) VALUES"
$lines += "('STEP-010-1', 'LOT-010', 'QFN-STD:2.0', '2.0', 1, 'SAW', '晶圆切割', 'Completed', 'SAW-003', 'FOUP-011', 'REC-SAW-QFN-001', DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-018', DATE_SUB(NOW(), INTERVAL 4 DAY), 'USR-018', 10000, 9940, 20, 40, 0, 0, 0, 'REC-SAW-QFN-001', NULL),"
$lines += "('STEP-010-2', 'LOT-010', 'QFN-STD:2.0', '2.0', 2, 'DA', '芯片贴装', 'Completed', 'DA-002', 'MAG-021', 'REC-DA-QFN-001', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-011', 9940, 9910, 10, 20, 0, 0, 0, 'REC-DA-QFN-001', NULL),"
$lines += "('STEP-010-3', 'LOT-010', 'QFN-STD:2.0', '2.0', 3, 'CURE', '固化', 'Completed', NULL, NULL, NULL, DATE_SUB(NOW(), INTERVAL 3 DAY), 'USR-011', DATE_SUB(NOW(), INTERVAL 2 DAY), 'USR-011', 9910, 9890, 10, 10, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-010-4', 'LOT-010', 'QFN-STD:2.0', '2.0', 4, 'WB', '焊线', 'Hold', 'WB-005', 'MAG-021', 'REC-WB-QFN-001', DATE_SUB(NOW(), INTERVAL 2 DAY), 'USR-012', NULL, NULL, 9890, 0, 0, 0, 0, 9890, 0, 'REC-WB-QFN-001', '拉力测试异常，待品质确认');"
$lines += ''

# LOT-011: QFN88 Rework (重工)
$lines += "-- LOT-011: QFN88 重工"
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`original_route_id\`, \`rework_route_id\`, \`rework_count\`, \`rework_reason\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`) VALUES"
$lines += "('LOT-011', 'WO-2026001', 'PROD-QFN88', 'QFN-88 控制器', 'CTRL-2024', 'QFN', 'RW-WB:1.0', '1.0', 1, 'DEBOND-WB', 'Processing', 500, 20, 'High', 'Magazine', 'MAG-013', 'WFR-20260517-001', 500, 0, 0, 0, 0, 0, 1, 'QFN-STD:2.0', 'RW-WB:1.0', 1, 'WireBond 断线重工', 0, NULL, 'LOT-001', NULL, NULL, 0, 0);"
$lines += ''
$lines += "INSERT INTO \`prod_lot_step\` (\`record_id\`, \`lot_id\`, \`route_id\`, \`route_version\`, \`step_seq\`, \`step_code\`, \`step_name\`, \`status\`, \`track_in_equipment\`, \`track_in_carrier\`, \`track_in_recipe\`, \`track_in_time\`, \`track_in_operator\`, \`track_out_time\`, \`track_out_operator\`, \`input_qty\`, \`pass_qty\`, \`fail_qty\`, \`scrap_qty\`, \`rework_qty\`, \`hold_qty\`, \`pending_qty\`, \`recipe_id\`, \`remark\`) VALUES"
$lines += "('STEP-011-1', 'LOT-011', 'RW-WB:1.0', '1.0', 1, 'DEBOND-WB', '去线', 'Completed', 'WB-001', 'MAG-013', NULL, DATE_SUB(NOW(), INTERVAL 1 DAY), 'USR-012', DATE_SUB(NOW(), INTERVAL 1 DAY), 'USR-012', 500, 500, 0, 0, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-011-2', 'LOT-011', 'RW-WB:1.0', '1.0', 2, 'CLEAN-WB', '清洗', 'Completed', NULL, NULL, NULL, DATE_SUB(NOW(), INTERVAL 1 DAY), 'USR-012', NOW(), 'USR-012', 500, 500, 0, 0, 0, 0, 0, NULL, NULL),"
$lines += "('STEP-011-3', 'LOT-011', 'RW-WB:1.0', '1.0', 3, 'WB', '重新焊线', 'Processing', 'WB-001', 'MAG-013', 'REC-WB-QFN-001', NOW(), 'USR-012', NULL, NULL, 500, 0, 0, 0, 0, 0, 500, 'REC-WB-QFN-001', NULL);"
$lines += ''

# LOT-012: SOP16 第二批已完成
$lines += "-- LOT-012: SOP16 第二批已完成"
$lines += "INSERT INTO \`prod_lot\` (\`lot_id\`, \`order_id\`, \`product_id\`, \`product_name\`, \`die_name\`, \`package_type\`, \`route_id\`, \`route_version\`, \`current_step_seq\`, \`current_step_code\`, \`status\`, \`unit_count\`, \`strip_count\`, \`priority\`, \`carrier_type\`, \`carrier_id\`, \`wafer_lot_id\`, \`original_qty\`, \`total_pass_qty\`, \`total_scrap_qty\`, \`total_rework_qty\`, \`total_hold_qty\`, \`is_partial_lot\`, \`is_rework_lot\`, \`is_under_mrb\`, \`grade\`, \`original_lot_id\`, \`bin_result\`, \`test_result\`, \`qty_pass\`, \`qty_fail\`) VALUES"
$lines += "('LOT-012', 'WO-2026002', 'PROD-SOP16', 'SOP-16 电源IC', 'PWR-500', 'SOP', 'SOP-STD:1.0', '1.0', 9, 'PACK', 'Completed', 10000, 500, 'Normal', 'Tray', 'TRAY-009', 'WFR-20260505-002', 10000, 9900, 50, 0, 0, 0, 0, 0, 'A', NULL, 'Bin1', 'Pass', 9900, 100);"
$lines += ''
$lines += "INSERT INTO \`prod_lot_step\` (\`record_id\`, \`lot_id\`, \`route_id\`, \`route_version\`, \`step_seq\`, \`step_code\`, \`step_name\`, \`status\`, \`track_in_equipment\`, \`track_in_carrier\`, \`track_in_recipe\`, \`track_in_time\`, \`track_in_operator\`, \`track_out_time\`, \`track_out_operator\`, \`input_qty\`, \`pass_qty\`, \`fail_qty\`, \`scrap_qty\`, \`rework_qty\`, \`hold_qty\`, \`pending_qty\`, \`recipe_id\`, \`remark\`) VALUES"
$lines += "('STEP-012-1', 'LOT-012', 'SOP-STD:1.0', '1.0', 1, 'SAW', '晶圆切割', 'Completed', 'SAW-002', 'FOUP-007', 'REC-SAW-SOP-001', DATE_SUB(NOW(), INTERVAL 12 DAY), 'USR-018', DATE_SUB(NOW(), INTERVAL 12 DAY), 'USR-0