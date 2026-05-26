USE mes_prod;

-- Child WO for WO-2026001 (Assemble - front end)
INSERT IGNORE INTO prod_work_order (order_id, parent_order_id, wo_type, product_id, product_name, route_id, route_name, planned_qty, completed_qty, customer_id, customer_name, priority, status, planned_start_date, planned_end_date, created_at) VALUES
('WO-2026001-ASM', 'WO-2026001', 'Child-Assemble', 'PROD-QFN88', 'QFN-88 Power IC', 'QFN-STD:2.0', 'QFN Standard Route v2.0', 20000, 15000, 'CUST-AUTO', 'Tesla Auto', 'High', 'Processing', DATE_SUB(NOW(), INTERVAL 10 DAY), DATE_ADD(NOW(), INTERVAL 5 DAY), DATE_SUB(NOW(), INTERVAL 10 DAY));

-- Child WO for WO-2026001 (Test - back end)
INSERT IGNORE INTO prod_work_order (order_id, parent_order_id, wo_type, product_id, product_name, route_id, route_name, planned_qty, completed_qty, customer_id, customer_name, priority, status, planned_start_date, planned_end_date, created_at) VALUES
('WO-2026001-FT', 'WO-2026001', 'Child-Test', 'PROD-QFN88', 'QFN-88 Power IC FT', 'QFN-STD:2.0', 'QFN Standard Route v2.0', 20000, 12000, 'CUST-AUTO', 'Tesla Auto', 'High', 'Processing', DATE_SUB(NOW(), INTERVAL 5 DAY), DATE_ADD(NOW(), INTERVAL 10 DAY), DATE_SUB(NOW(), INTERVAL 5 DAY));

-- Child WO for WO-2026003 (BGA Auto)
INSERT IGNORE INTO prod_work_order (order_id, parent_order_id, wo_type, product_id, product_name, route_id, route_name, planned_qty, completed_qty, customer_id, customer_name, priority, status, planned_start_date, planned_end_date, created_at) VALUES
('WO-2026003-ASM', 'WO-2026003', 'Child-Assemble', 'PROD-BGA256', 'BGA-256 MCU', 'BGA64-STD:1.0', 'BGA Standard Route', 5000, 3000, 'CUST-AUTO', 'Tesla Auto', 'High', 'Processing', DATE_SUB(NOW(), INTERVAL 8 DAY), DATE_ADD(NOW(), INTERVAL 7 DAY), DATE_SUB(NOW(), INTERVAL 8 DAY));

INSERT IGNORE INTO prod_work_order (order_id, parent_order_id, wo_type, product_id, product_name, route_id, route_name, planned_qty, completed_qty, customer_id, customer_name, priority, status, planned_start_date, planned_end_date, created_at) VALUES
('WO-2026003-FT', 'WO-2026003', 'Child-Test', 'PROD-BGA256', 'BGA-256 MCU FT', 'BGA64-STD:1.0', 'BGA Standard Route', 5000, 2800, 'CUST-AUTO', 'Tesla Auto', 'High', 'Processing', DATE_SUB(NOW(), INTERVAL 3 DAY), DATE_ADD(NOW(), INTERVAL 12 DAY), DATE_SUB(NOW(), INTERVAL 3 DAY));

-- Child WO for WO-2026005 (SOP Industrial)
INSERT IGNORE INTO prod_work_order (order_id, parent_order_id, wo_type, product_id, product_name, route_id, route_name, planned_qty, completed_qty, customer_id, customer_name, priority, status, planned_start_date, planned_end_date, created_at) VALUES
('WO-2026005-ASM', 'WO-2026005', 'Child-Assemble', 'PROD-SOP8', 'SOP-8 MOSFET', 'SOP8-STD:1.0', 'SOP8 Standard Route', 50000, 40000, 'CUST-IND', 'Siemens Industrial', 'Normal', 'Processing', DATE_SUB(NOW(), INTERVAL 12 DAY), DATE_ADD(NOW(), INTERVAL 3 DAY), DATE_SUB(NOW(), INTERVAL 12 DAY));

INSERT IGNORE INTO prod_work_order (order_id, parent_order_id, wo_type, product_id, product_name, route_id, route_name, planned_qty, completed_qty, customer_id, customer_name, priority, status, planned_start_date, planned_end_date, created_at) VALUES
('WO-2026005-FT', 'WO-2026005', 'Child-Test', 'PROD-SOP8', 'SOP-8 MOSFET FT', 'SOP8-STD:1.0', 'SOP8 Standard Route', 50000, 45000, 'CUST-IND', 'Siemens Industrial', 'Normal', 'Processing', DATE_SUB(NOW(), INTERVAL 7 DAY), DATE_ADD(NOW(), INTERVAL 8 DAY), DATE_SUB(NOW(), INTERVAL 7 DAY));

-- Child WO for WO-2026006 (BGA Auto)
INSERT IGNORE INTO prod_work_order (order_id, parent_order_id, wo_type, product_id, product_name, route_id, route_name, planned_qty, completed_qty, customer_id, customer_name, priority, status, planned_start_date, planned_end_date, created_at) VALUES
('WO-2026006-ASM', 'WO-2026006', 'Child-Assemble', 'PROD-BGA64', 'BGA-64 Sensor', 'BGA-STD:1.0', 'BGA Standard Route', 10000, 8000, 'CUST-AUTO', 'Tesla Auto', 'Normal', 'Processing', DATE_SUB(NOW(), INTERVAL 6 DAY), DATE_ADD(NOW(), INTERVAL 9 DAY), DATE_SUB(NOW(), INTERVAL 6 DAY));

INSERT IGNORE INTO prod_work_order (order_id, parent_order_id, wo_type, product_id, product_name, route_id, route_name, planned_qty, completed_qty, customer_id, customer_name, priority, status, planned_start_date, planned_end_date, created_at) VALUES
('WO-2026006-FT', 'WO-2026006', 'Child-Test', 'PROD-BGA64', 'BGA-64 Sensor FT', 'BGA-STD:1.0', 'BGA Standard Route', 10000, 7500, 'CUST-AUTO', 'Tesla Auto', 'Normal', 'Processing', DATE_SUB(NOW(), INTERVAL 2 DAY), DATE_ADD(NOW(), INTERVAL 13 DAY), DATE_SUB(NOW(), INTERVAL 2 DAY));

-- Child WO for WO-2026007 (QFN Auto)
INSERT IGNORE INTO prod_work_order (order_id, parent_order_id, wo_type, product_id, product_name, route_id, route_name, planned_qty, completed_qty, customer_id, customer_name, priority, status, planned_start_date, planned_end_date, created_at) VALUES
('WO-2026007-ASM', 'WO-2026007', 'Child-Assemble', 'PROD-QFN64', 'QFN-64 Driver IC', 'QFN64-STD:1.0', 'QFN64 Standard Route', 25000, 20000, 'CUST-AUTO', 'Tesla Auto', 'High', 'Processing', DATE_SUB(NOW(), INTERVAL 9 DAY), DATE_ADD(NOW(), INTERVAL 6 DAY), DATE_SUB(NOW(), INTERVAL 9 DAY));

INSERT IGNORE INTO prod_work_order (order_id, parent_order_id, wo_type, product_id, product_name, route_id, route_name, planned_qty, completed_qty, customer_id, customer_name, priority, status, planned_start_date, planned_end_date, created_at) VALUES
('WO-2026007-FT', 'WO-2026007', 'Child-Test', 'PROD-QFN64', 'QFN-64 Driver IC FT', 'QFN64-STD:1.0', 'QFN64 Standard Route', 25000, 18000, 'CUST-AUTO', 'Tesla Auto', 'High', 'Processing', DATE_SUB(NOW(), INTERVAL 4 DAY), DATE_ADD(NOW(), INTERVAL 11 DAY), DATE_SUB(NOW(), INTERVAL 4 DAY));
