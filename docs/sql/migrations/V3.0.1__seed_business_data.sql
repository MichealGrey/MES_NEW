-- ============================================================================
-- V3.0.1 - Seed Business Data (OSAT Realistic Data)
-- Date: 2026-06-04
-- Description: Complete business data for demo - customers, products, routes,
--              equipment, work orders, lots, alarms, complaints, etc.
-- All data is interconnected and reflects real OSAT production scenarios.
-- ============================================================================

SET @now = UTC_TIMESTAMP();

-- ============================================================================
-- 1. DEPARTMENTS - Complete org tree
-- ============================================================================
INSERT IGNORE INTO sys_department (dept_id, dept_name, parent_id, manager_id, status) VALUES
('DEPT-001', '总经办', NULL, 'EMP-001', 'Active'),
('DEPT-010', '运营中心', 'DEPT-001', 'EMP-002', 'Active'),
('DEPT-011', '生产部', 'DEPT-010', 'EMP-003', 'Active'),
('DEPT-012', '设备部', 'DEPT-010', 'EMP-004', 'Active'),
('DEPT-013', '生产计划部', 'DEPT-010', 'EMP-005', 'Active'),
('DEPT-020', '质量中心', 'DEPT-001', 'EMP-006', 'Active'),
('DEPT-021', '质量保证部', 'DEPT-020', 'EMP-007', 'Active'),
('DEPT-022', '质量控制部', 'DEPT-020', 'EMP-008', 'Active'),
('DEPT-030', '工程中心', 'DEPT-001', 'EMP-009', 'Active'),
('DEPT-031', '工艺工程部', 'DEPT-030', 'EMP-010', 'Active'),
('DEPT-032', '产品工程部', 'DEPT-030', 'EMP-011', 'Active'),
('DEPT-033', '设备工程部', 'DEPT-030', 'EMP-012', 'Active'),
('DEPT-040', '商务中心', 'DEPT-001', 'EMP-013', 'Active'),
('DEPT-041', '销售部', 'DEPT-040', 'EMP-014', 'Active'),
('DEPT-042', '客户服务部', 'DEPT-040', 'EMP-015', 'Active');

-- ============================================================================
-- 2. ROLES - 15 roles matching OSAT org
-- ============================================================================
INSERT IGNORE INTO sys_role (role_id, role_name, description, level) VALUES
('ROLE-OPERATOR', '操作员', '产线操作员，执行工单上下料、报工', 1),
('ROLE-TECHNICIAN', '技术员', '设备调试、首件确认、工艺参数设定', 2),
('ROLE-TEAMLEADER', '班组长', '班组管理、工单调度、异常审批', 3),
('ROLE-SUPERVISOR', '车间主管', '车间全面管理、产能规划', 4),
('ROLE-PROCESS-ENG', '工艺工程师', '工艺路线、参数配置、DOE', 4),
('ROLE-PRODUCT-ENG', '产品工程师', '产品管理、NPI、良率提升', 4),
('ROLE-EQUIP-ENG', '设备工程师', '设备维护、保养计划、备件', 4),
('ROLE-QA-ENG', '质量保证工程师', '体系管理、持续改进、可靠性', 4),
('ROLE-QC-INSPECTOR', '质量巡检员', '制程巡检、首件检验、SPC监控', 2),
('ROLE-CQE', '客户质量工程师', '客诉处理、8D报告、客户对接', 4),
('ROLE-PLANNER', '计划员', '工单排产、进度跟踪', 3),
('ROLE-WAREHOUSE', '仓库管理员', '物料收发、库存管理', 1),
('ROLE-PRODUCTION-MGR', '生产经理', '生产部管理、绩效分析', 5),
('ROLE-QUALITY-MGR', '质量经理', '质量部管理、质量目标', 5),
('ROLE-ADMIN', '系统管理员', '系统配置、用户管理', 6);

-- ============================================================================
-- 3. USERS - 30 employees across all departments/roles
-- ============================================================================
INSERT IGNORE INTO sys_user (user_id, user_name, password_hash, role_id, dept_id, shift, is_active) VALUES
('EMP-001', '张明远', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-ADMIN', 'DEPT-001', NULL, 1),
('EMP-002', '李志强', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-PRODUCTION-MGR', 'DEPT-010', NULL, 1),
('EMP-003', '王建国', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-PRODUCTION-MGR', 'DEPT-011', NULL, 1),
('EMP-004', '赵文博', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-EQUIP-ENG', 'DEPT-012', NULL, 1),
('EMP-005', '刘慧芳', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-PLANNER', 'DEPT-013', NULL, 1),
('EMP-006', '陈伟明', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-QUALITY-MGR', 'DEPT-020', NULL, 1),
('EMP-007', '周秀兰', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-QA-ENG', 'DEPT-021', NULL, 1),
('EMP-008', '吴德强', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-QC-INSPECTOR', 'DEPT-022', NULL, 1),
('EMP-009', '郑海涛', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-PROCESS-ENG', 'DEPT-030', NULL, 1),
('EMP-010', '孙丽娟', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-PROCESS-ENG', 'DEPT-031', NULL, 1),
('EMP-011', '马志鹏', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-PRODUCT-ENG', 'DEPT-032', NULL, 1),
('EMP-012', '何志刚', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-EQUIP-ENG', 'DEPT-033', NULL, 1),
('EMP-013', '黄伟', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-PLANNER', 'DEPT-040', NULL, 1),
('EMP-014', '林晓峰', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-CQE', 'DEPT-042', NULL, 1),
('EMP-015', '杨婷婷', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-CQE', 'DEPT-042', NULL, 1),
('EMP-016', '胡建军', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-SUPERVISOR', 'DEPT-011', 'Day', 1),
('EMP-017', '郭秀英', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-SUPERVISOR', 'DEPT-011', 'Night', 1),
('EMP-018', '罗志明', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-TEAMLEADER', 'DEPT-011', 'Day', 1),
('EMP-019', '梁小红', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-TEAMLEADER', 'DEPT-011', 'Night', 1),
('EMP-020', '宋德伟', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-TECHNICIAN', 'DEPT-011', 'Day', 1),
('EMP-021', '谢明华', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-TECHNICIAN', 'DEPT-011', 'Night', 1),
('EMP-022', '韩秀梅', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-OPERATOR', 'DEPT-011', 'Day', 1),
('EMP-023', '唐志强', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-OPERATOR', 'DEPT-011', 'Day', 1),
('EMP-024', '冯丽娟', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-OPERATOR', 'DEPT-011', 'Night', 1),
('EMP-025', '曹文博', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-OPERATOR', 'DEPT-011', 'Night', 1),
('EMP-026', '邓海涛', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-QC-INSPECTOR', 'DEPT-022', 'Day', 1),
('EMP-027', '萧秀兰', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-QC-INSPECTOR', 'DEPT-022', 'Night', 1),
('EMP-028', '田志刚', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-TECHNICIAN', 'DEPT-031', 'Day', 1),
('EMP-029', '潘丽', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-WAREHOUSE', 'DEPT-011', 'Day', 1),
('EMP-030', '蒋伟', '$2a$11$hT1jc9DXpSysIGU0ZQG.i.S5xR821l1bFtXBsL4.z8T/6MkUudS0.', 'ROLE-WAREHOUSE', 'DEPT-011', 'Night', 1);

-- ============================================================================
-- 4. SIGNATURE LEVELS
-- ============================================================================
INSERT IGNORE INTO sys_signature_level (level_code, level_name, level_order, description) VALUES
('L1', '操作员签字', 1, '常规操作确认'),
('L2', '技术员签字', 2, '换线/首件确认'),
('L3', '班组长审批', 3, '异常处理/报废审批'),
('L4', '工程师审批', 4, '工艺变更/参数调整'),
('L5', '主管审批', 5, '重大异常/批量报废');

-- ============================================================================
-- 5. CUSTOMERS - 20 customers (Tier 1-4)
-- ============================================================================
INSERT IGNORE INTO master_customer (customer_id, customer_code, customer_name, contact_person, contact_phone, email, quality_level, customer_pn_prefix, status) VALUES
('CUST-001', 'INNOPEAK', 'InnoPeak Technology', 'David Chen', '0755-88881001', 'd.chen@innopeak.com', 'Automotive', 'IPN', 'Active'),
('CUST-002', 'CORECHIP', 'CoreChip Semiconductor', 'Sarah Wang', '021-66662002', 's.wang@corechip.com', 'Automotive', 'CCS', 'Active'),
('CUST-003', 'MICROTECH', 'MicroTech Industries', 'James Liu', '0512-88883003', 'j.liu@microtech.com', 'Industrial', 'MTI', 'Active'),
('CUST-004', 'NEXUS-IC', 'Nexus IC Design', 'Emily Zhang', '010-66664004', 'e.zhang@nexus-ic.com', 'Consumer', 'NXI', 'Active'),
('CUST-005', 'GLOBALPOWER', 'GlobalPower Semi', 'Robert Li', '0755-88885005', 'r.li@globalpower.com', 'Automotive', 'GPS', 'Active'),
('CUST-006', 'SINOWAVE', 'SinoWave Electronics', 'Angela Wu', '0592-66666006', 'a.wu@sinowave.com', 'Consumer', 'SWE', 'Active'),
('CUST-007', 'APEX-SEMI', 'Apex Semiconductor', 'Kevin Zhao', '021-88887007', 'k.zhao@apex-semi.com', 'Industrial', 'APS', 'Active'),
('CUST-008', 'BRIDGELINK', 'BridgeLink Technology', 'Lisa Huang', '0755-66668008', 'l.huang@bridgelink.com', 'Consumer', 'BLT', 'Active'),
('CUST-009', 'VOLTAGE-TECH', 'VoltageTech Corp', 'Mark Sun', '010-88889009', 'm.sun@voltagetech.com', 'Industrial', 'VTC', 'Active'),
('CUST-010', 'CRYSTAL-IC', 'Crystal IC Solutions', 'Amy Lin', '0512-66660010', 'a.lin@crystal-ic.com', 'Consumer', 'CRY', 'Active'),
('CUST-011', 'THUNDER-SEMI', 'Thunder Semiconductor', 'Paul Xu', '0755-88881011', 'p.xu@thunder-semi.com', 'Automotive', 'THS', 'Active'),
('CUST-012', 'QUANTUM-IC', 'Quantum IC Technology', 'Grace Ma', '021-66662012', 'g.ma@quantum-ic.com', 'Industrial', 'QIC', 'Active'),
('CUST-013', 'SMARTPOWER', 'SmartPower Electronics', 'Daniel Ye', '0592-88883013', 'd.ye@smartpower.com', 'Consumer', 'SPE', 'Active'),
('CUST-014', 'FUSION-SEMI', 'Fusion Semiconductor', 'Rachel Qian', '010-66664014', 'r.qian@fusion-semi.com', 'Automotive', 'FSM', 'Active'),
('CUST-015', 'NOVA-CHIP', 'NovaChip Design', 'Steven Fang', '0755-88885015', 's.fang@novachip.com', 'Consumer', 'NVC', 'Active'),
('CUST-016', 'STARLINK-IC', 'StarLink IC', 'Megan Cao', '0512-66666016', 'm.cao@starlink-ic.com', 'Industrial', 'SLI', 'Active'),
('CUST-017', 'PINNACLE-SEMI', 'Pinnacle Semiconductor', 'Eric Duan', '021-88887017', 'e.duan@pinnacle-semi.com', 'Consumer', 'PNS', 'Active'),
('CUST-018', 'OCEANWAVE-IC', 'OceanWave IC Design', 'Helen Kong', '010-66668018', 'h.kong@oceanwave-ic.com', 'Consumer', 'OWI', 'Active'),
('CUST-019', 'ZENITH-SEMI', 'Zenith Semiconductor', 'Tony Meng', '0755-88889019', 't.meng@zenith-semi.com', 'Industrial', 'ZNS', 'Active'),
('CUST-020', 'SUMMIT-CHIP', 'SummitChip Technology', 'Cindy Bai', '0592-66660020', 'c.bai@summit-chip.com', 'Consumer', 'SMC', 'Active');

-- ============================================================================
-- 6. PRODUCTS - 50 products across customers
-- ============================================================================
INSERT IGNORE INTO master_product (product_id, product_name, die_name, package_type, process_stage, default_route_id, unit_qty, customer_id, customer_name, customer_pn, internal_pn, status) VALUES
('PROD-001', 'BGA-156 Power IC', 'PowerDie-7x7', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-001', 'InnoPeak Technology', 'IPN-BGA156-PWR', 'INT-BGA156-001', 'Active'),
('PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-001', 'InnoPeak Technology', 'IPN-BGA256-MCU', 'INT-BGA256-002', 'Active'),
('PROD-003', 'BGA-100 RF Transceiver', 'RF-Die-5x5', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-002', 'CoreChip Semiconductor', 'CCS-BGA100-RF', 'INT-BGA100-003', 'Active'),
('PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-002', 'CoreChip Semiconductor', 'CCS-QFN32-SEN', 'INT-QFN32-004', 'Active'),
('PROD-005', 'QFN-48 LED Driver', 'LED-Die-4x4', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-003', 'MicroTech Industries', 'MTI-QFN48-LED', 'INT-QFN48-005', 'Active'),
('PROD-006', 'QFN-20 Temperature Sensor', 'Temp-Die-2x2', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-003', 'MicroTech Industries', 'MTI-QFN20-TEMP', 'INT-QFN20-006', 'Active'),
('PROD-007', 'SOP-8 MOSFET', 'MOS-Die-3x2', 'SOP', 'Assemble', 'SOP-STD', 1, 'CUST-004', 'Nexus IC Design', 'NXI-SOP8-MOS', 'INT-SOP8-007', 'Active'),
('PROD-008', 'SOP-16 Audio Amp', 'Audio-Die-4x2', 'SOP', 'Assemble', 'SOP-STD', 1, 'CUST-004', 'Nexus IC Design', 'NXI-SOP16-AUD', 'INT-SOP16-008', 'Active'),
('PROD-009', 'DFN-6 LDO', 'LDO-Die-1x1', 'DFN', 'Assemble', 'DFN-STD', 1, 'CUST-005', 'GlobalPower Semi', 'GPS-DFN6-LDO', 'INT-DFN6-009', 'Active'),
('PROD-010', 'DFN-8 DC-DC', 'DCDC-Die-2x2', 'DFN', 'Assemble', 'DFN-STD', 1, 'CUST-005', 'GlobalPower Semi', 'GPS-DFN8-DCDC', 'INT-DFN8-010', 'Active'),
('PROD-011', 'TO-220 IGBT', 'IGBT-Die-6x6', 'TO', 'Assemble', 'TO-STD', 1, 'CUST-006', 'SinoWave Electronics', 'SWE-TO220-IGBT', 'INT-TO220-011', 'Active'),
('PROD-012', 'TO-263 Schottky', 'SBD-Die-4x4', 'TO', 'Assemble', 'TO-STD', 1, 'CUST-006', 'SinoWave Electronics', 'SWE-TO263-SBD', 'INT-TO263-012', 'Active'),
('PROD-013', 'BGA-208 FPGA', 'FPGA-Die-15x15', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-007', 'Apex Semiconductor', 'APS-BGA208-FPGA', 'INT-BGA208-013', 'Active'),
('PROD-014', 'QFN-68 USB Hub', 'USB-Die-6x6', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-007', 'Apex Semiconductor', 'APS-QFN68-USB', 'INT-QFN68-014', 'Active'),
('PROD-015', 'BGA-64 WiFi Module', 'WiFi-Die-5x5', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-008', 'BridgeLink Technology', 'BLT-BGA64-WIFI', 'INT-BGA64-015', 'Active'),
('PROD-016', 'SOP-14 Comparator', 'CMP-Die-2x2', 'SOP', 'Assemble', 'SOP-STD', 1, 'CUST-008', 'BridgeLink Technology', 'BLT-SOP14-CMP', 'INT-SOP14-016', 'Active'),
('PROD-017', 'QFN-40 Power Mgmt', 'PMU-Die-5x5', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-009', 'VoltageTech Corp', 'VTC-QFN40-PMU', 'INT-QFN40-017', 'Active'),
('PROD-018', 'DFN-10 ESD Protect', 'ESD-Die-1x1', 'DFN', 'Assemble', 'DFN-STD', 1, 'CUST-009', 'VoltageTech Corp', 'VTC-DFN10-ESD', 'INT-DFN10-018', 'Active'),
('PROD-019', 'BGA-144 SoC', 'SoC-Die-8x8', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-010', 'Crystal IC Solutions', 'CRY-BGA144-SOC', 'INT-BGA144-019', 'Active'),
('PROD-020', 'SOP-20 Op-Amp', 'OPAMP-Die-3x2', 'SOP', 'Assemble', 'SOP-STD', 1, 'CUST-010', 'Crystal IC Solutions', 'CRY-SOP20-OPA', 'INT-SOP20-020', 'Active'),
('PROD-021', 'QFP-44 MCU', 'MCU-Die-5x5', 'QFP', 'Assemble', 'QFP-STD', 1, 'CUST-011', 'Thunder Semiconductor', 'THS-QFP44-MCU', 'INT-QFP44-021', 'Active'),
('PROD-022', 'QFP-100 Ethernet PHY', 'PHY-Die-7x7', 'QFP', 'Assemble', 'QFP-STD', 1, 'CUST-011', 'Thunder Semiconductor', 'THS-QFP100-PHY', 'INT-QFP100-022', 'Active'),
('PROD-023', 'BGA-324 DDR Controller', 'DDR-Die-12x12', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-012', 'Quantum IC Technology', 'QIC-BGA324-DDR', 'INT-BGA324-023', 'Active'),
('PROD-024', 'QFN-56 ADC', 'ADC-Die-5x5', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-012', 'Quantum IC Technology', 'QIC-QFN56-ADC', 'INT-QFN56-024', 'Active'),
('PROD-025', 'SOP-8 Voltage Ref', 'VREF-Die-1x1', 'SOP', 'Assemble', 'SOP-STD', 1, 'CUST-013', 'SmartPower Electronics', 'SPE-SOP8-VREF', 'INT-SOP8-025', 'Active'),
('PROD-026', 'DFN-12 Battery Mgmt', 'BMS-Die-3x3', 'DFN', 'Assemble', 'DFN-STD', 1, 'CUST-013', 'SmartPower Electronics', 'SPE-DFN12-BMS', 'INT-DFN12-026', 'Active'),
('PROD-027', 'BGA-96 Display Driver', 'DDI-Die-6x6', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-014', 'Fusion Semiconductor', 'FSM-BGA96-DDI', 'INT-BGA96-027', 'Active'),
('PROD-028', 'QFN-24 Motion Sensor', 'Motion-Die-3x3', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-014', 'Fusion Semiconductor', 'FSM-QFN24-MOT', 'INT-QFN24-028', 'Active'),
('PROD-029', 'SOP-16 Logic Gate', 'Logic-Die-2x2', 'SOP', 'Assemble', 'SOP-STD', 1, 'CUST-015', 'NovaChip Design', 'NVC-SOP16-LOG', 'INT-SOP16-029', 'Active'),
('PROD-030', 'QFN-88 PCIe Switch', 'PCIe-Die-8x8', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-015', 'NovaChip Design', 'NVC-QFN88-PCIe', 'INT-QFN88-030', 'Active'),
('PROD-031', 'BGA-48 GPS Receiver', 'GPS-Die-4x4', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-016', 'StarLink IC', 'SLI-BGA48-GPS', 'INT-BGA48-031', 'Active'),
('PROD-032', 'TO-247 MOSFET', 'PowerMos-8x8', 'TO', 'Assemble', 'TO-STD', 1, 'CUST-016', 'StarLink IC', 'SLI-TO247-MOS', 'INT-TO247-032', 'Active'),
('PROD-033', 'QFP-64 CAN Controller', 'CAN-Die-5x5', 'QFP', 'Assemble', 'QFP-STD', 1, 'CUST-017', 'Pinnacle Semiconductor', 'PNS-QFP64-CAN', 'INT-QFP64-033', 'Active'),
('PROD-034', 'DFN-6 LED Driver', 'LEDDrv-Die-1x1', 'DFN', 'Assemble', 'DFN-STD', 1, 'CUST-017', 'Pinnacle Semiconductor', 'PNS-DFN6-LED', 'INT-DFN6-034', 'Active'),
('PROD-035', 'BGA-180 GPU', 'GPU-Die-12x12', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-018', 'OceanWave IC Design', 'OWI-BGA180-GPU', 'INT-BGA180-035', 'Active'),
('PROD-036', 'QFN-32 Clock Gen', 'CLK-Die-3x3', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-018', 'OceanWave IC Design', 'OWI-QFN32-CLK', 'INT-QFN32-036', 'Active'),
('PROD-037', 'SOP-28 EEPROM', 'EEPROM-Die-2x3', 'SOP', 'Assemble', 'SOP-STD', 1, 'CUST-019', 'Zenith Semiconductor', 'ZNS-SOP28-EEP', 'INT-SOP28-037', 'Active'),
('PROD-038', 'QFN-16 Crystal Osc', 'OSC-Die-2x2', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-019', 'Zenith Semiconductor', 'ZNS-QFN16-OSC', 'INT-QFN16-038', 'Active'),
('PROD-039', 'BGA-72 Audio Codec', 'AudioC-Die-5x5', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-020', 'SummitChip Technology', 'SMC-BGA72-AUDC', 'INT-BGA72-039', 'Active'),
('PROD-040', 'SOP-8 Zener Diode', 'Zener-Die-1x1', 'SOP', 'Assemble', 'SOP-STD', 1, 'CUST-020', 'SummitChip Technology', 'SMC-SOP8-ZNR', 'INT-SOP8-040', 'Active'),
('PROD-041', 'BGA-156 Power IC Rev2', 'PowerDie-7x7-V2', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-001', 'InnoPeak Technology', 'IPN-BGA156-PWR2', 'INT-BGA156-041', 'Active'),
('PROD-042', 'QFN-32 Sensor Rev2', 'Sensor-Die-3x3-V2', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-002', 'CoreChip Semiconductor', 'CCS-QFN32-SEN2', 'INT-QFN32-042', 'Active'),
('PROD-043', 'QFN-48 LED Driver Rev2', 'LED-Die-4x4-V2', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-003', 'MicroTech Industries', 'MTI-QFN48-LED2', 'INT-QFN48-043', 'Active'),
('PROD-044', 'BGA-256 MCU Rev2', 'MCU-Die-10x10-V2', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-001', 'InnoPeak Technology', 'IPN-BGA256-MCU2', 'INT-BGA256-044', 'Active'),
('PROD-045', 'QFN-20 Temp Sensor Auto', 'Temp-Die-2x2-Auto', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-005', 'GlobalPower Semi', 'GPS-QFN20-TEMP-A', 'INT-QFN20-045', 'Active'),
('PROD-046', 'SOP-8 MOSFET Auto', 'MOS-Die-3x2-Auto', 'SOP', 'Assemble', 'SOP-STD', 1, 'CUST-007', 'Apex Semiconductor', 'APS-SOP8-MOS-A', 'INT-SOP8-046', 'Active'),
('PROD-047', 'BGA-100 RF Rev2', 'RF-Die-5x5-V2', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-002', 'CoreChip Semiconductor', 'CCS-BGA100-RF2', 'INT-BGA100-047', 'Active'),
('PROD-048', 'QFN-40 PMU Auto', 'PMU-Die-5x5-Auto', 'QFN', 'Assemble', 'QFN-STD', 1, 'CUST-011', 'Thunder Semiconductor', 'THS-QFN40-PMU-A', 'INT-QFN40-048', 'Active'),
('PROD-049', 'BGA-144 SoC Rev2', 'SoC-Die-8x8-V2', 'BGA', 'Assemble', 'BGA-STD', 1, 'CUST-010', 'Crystal IC Solutions', 'CRY-BGA144-SOC2', 'INT-BGA144-049', 'Active'),
('PROD-050', 'DFN-8 DC-DC Auto', 'DCDC-Die-2x2-Auto', 'DFN', 'Assemble', 'DFN-STD', 1, 'CUST-009', 'VoltageTech Corp', 'VTC-DFN8-DCDC-A', 'INT-DFN8-050', 'Active');

-- ============================================================================
-- 7. ROUTES - 8 routes (BGA/QFN/SOP/DFN/TO/QFP)
-- ============================================================================
-- BGA Route
INSERT IGNORE INTO master_route (route_id, route_name, route_version, product_id, package_type, is_active, is_approved, approved_by, approved_at) VALUES
('BGA-STD', 'BGA Standard Assembly Route', '1.0', 'BGA', 'BGA', 1, 1, 'Admin', DATE_SUB(@now, INTERVAL 30 DAY)),
('QFN-STD', 'QFN Standard Assembly Route', '1.0', 'QFN', 'QFN', 1, 1, 'Admin', DATE_SUB(@now, INTERVAL 30 DAY)),
('SOP-STD', 'SOP Standard Assembly Route', '1.0', 'SOP', 'SOP', 1, 1, 'Admin', DATE_SUB(@now, INTERVAL 30 DAY)),
('DFN-STD', 'DFN Standard Assembly Route', '1.0', 'DFN', 'DFN', 1, 1, 'Admin', DATE_SUB(@now, INTERVAL 30 DAY)),
('TO-STD', 'TO Standard Assembly Route', '1.0', 'TO', 'TO', 1, 1, 'Admin', DATE_SUB(@now, INTERVAL 30 DAY)),
('QFP-STD', 'QFP Standard Assembly Route', '1.0', 'QFP', 'QFP', 1, 1, 'Admin', DATE_SUB(@now, INTERVAL 30 DAY));

-- BGA Steps
INSERT IGNORE INTO master_route_step (route_id, step_seq, step_code, step_name, equipment_group) VALUES
('BGA-STD', 10, 'GRIND', '晶圆减薄', 'Grinder'),
('BGA-STD', 20, 'DICE', '晶圆切割', 'Dicing'),
('BGA-STD', 30, 'DIE_ATTACH', '贴片', 'Die Bonder'),
('BGA-STD', 40, 'CURE_DA', '贴片固化', 'Oven'),
('BGA-STD', 50, 'WIRE_BOND', '引线键合', 'Wire Bonder'),
('BGA-STD', 60, 'MOLD', '塑封', 'Mold Press'),
('BGA-STD', 70, 'CURE', '后固化', 'Oven'),
('BGA-STD', 80, 'LASER_MARK', '激光打标', 'Laser Marker'),
('BGA-STD', 90, 'PLATE', '电镀', 'Plating Line'),
('BGA-STD', 100, 'SINGULATE', '切筋成型', 'Trim Form'),
('BGA-STD', 110, 'TEST_FT', '成品测试', 'Tester'),
('BGA-STD', 120, 'FINAL_PACK', '终包', 'Packer');

-- QFN Steps
INSERT IGNORE INTO master_route_step (route_id, step_seq, step_code, step_name, equipment_group) VALUES
('QFN-STD', 10, 'DIE_ATTACH', '贴片', 'Die Bonder'),
('QFN-STD', 20, 'CURE_DA', '贴片固化', 'Oven'),
('QFN-STD', 30, 'WIRE_BOND', '引线键合', 'Wire Bonder'),
('QFN-STD', 40, 'MOLD', '塑封', 'Mold Press'),
('QFN-STD', 50, 'CURE', '后固化', 'Oven'),
('QFN-STD', 60, 'PLATE', '电镀', 'Plating Line'),
('QFN-STD', 70, 'SINGULATE', '切筋成型', 'Trim Form'),
('QFN-STD', 80, 'TEST_FT', '成品测试', 'Tester'),
('QFN-STD', 90, 'FINAL_PACK', '终包', 'Packer');

-- SOP Steps
INSERT IGNORE INTO master_route_step (route_id, step_seq, step_code, step_name, equipment_group) VALUES
('SOP-STD', 10, 'DIE_ATTACH', '贴片', 'Die Bonder'),
('SOP-STD', 20, 'CURE_DA', '贴片固化', 'Oven'),
('SOP-STD', 30, 'WIRE_BOND', '引线键合', 'Wire Bonder'),
('SOP-STD', 40, 'MOLD', '塑封', 'Mold Press'),
('SOP-STD', 50, 'CURE', '后固化', 'Oven'),
('SOP-STD', 60, 'PLATE', '电镀', 'Plating Line'),
('SOP-STD', 70, 'TRIM_FORM', '切筋成型', 'Trim Form'),
('SOP-STD', 80, 'LASER_MARK', '激光打标', 'Laser Marker'),
('SOP-STD', 90, 'TEST_FT', '成品测试', 'Tester'),
('SOP-STD', 100, 'FINAL_PACK', '终包', 'Packer');

-- DFN Steps (similar to QFN but shorter)
INSERT IGNORE INTO master_route_step (route_id, step_seq, step_code, step_name, equipment_group) VALUES
('DFN-STD', 10, 'DIE_ATTACH', '贴片', 'Die Bonder'),
('DFN-STD', 20, 'CURE_DA', '贴片固化', 'Oven'),
('DFN-STD', 30, 'WIRE_BOND', '引线键合', 'Wire Bonder'),
('DFN-STD', 40, 'MOLD', '塑封', 'Mold Press'),
('DFN-STD', 50, 'CURE', '后固化', 'Oven'),
('DFN-STD', 60, 'PLATE', '电镀', 'Plating Line'),
('DFN-STD', 70, 'SINGULATE', '切筋成型', 'Trim Form'),
('DFN-STD', 80, 'TEST_FT', '成品测试', 'Tester'),
('DFN-STD', 90, 'FINAL_PACK', '终包', 'Packer');

-- TO Steps
INSERT IGNORE INTO master_route_step (route_id, step_seq, step_code, step_name, equipment_group) VALUES
('TO-STD', 10, 'DIE_ATTACH', '贴片', 'Die Bonder'),
('TO-STD', 20, 'WIRE_BOND', '引线键合', 'Wire Bonder'),
('TO-STD', 30, 'MOLD', '塑封', 'Mold Press'),
('TO-STD', 40, 'PLATE', '电镀', 'Plating Line'),
('TO-STD', 50, 'TRIM_FORM', '切筋成型', 'Trim Form'),
('TO-STD', 60, 'TEST_FT', '成品测试', 'Tester'),
('TO-STD', 70, 'FINAL_PACK', '终包', 'Packer');

-- QFP Steps
INSERT IGNORE INTO master_route_step (route_id, step_seq, step_code, step_name, equipment_group) VALUES
('QFP-STD', 10, 'DIE_ATTACH', '贴片', 'Die Bonder'),
('QFP-STD', 20, 'CURE_DA', '贴片固化', 'Oven'),
('QFP-STD', 30, 'WIRE_BOND', '引线键合', 'Wire Bonder'),
('QFP-STD', 40, 'MOLD', '塑封', 'Mold Press'),
('QFP-STD', 50, 'CURE', '后固化', 'Oven'),
('QFP-STD', 60, 'PLATE', '电镀', 'Plating Line'),
('QFP-STD', 70, 'TRIM_FORM', '切筋成型', 'Trim Form'),
('QFP-STD', 80, 'TEST_FT', '成品测试', 'Tester'),
('QFP-STD', 90, 'FINAL_PACK', '终包', 'Packer');

-- ============================================================================
-- 8. EQUIPMENT - 40 machines across all process stages
-- ============================================================================
INSERT IGNORE INTO master_equipment (equipment_id, equipment_name, equipment_group, equipment_type, process_stage, vendor, model, serial_number, status, location, responsible_person) VALUES
-- Grinders (2)
('EQP-GRD-001', '晶圆减磨机#1', 'Grinder', 'Grinding Machine', 'Front-End', 'Disco', 'DFG-8540', 'SN-GRD-001', 'Running', '车间A-01', 'EMP-004'),
('EQP-GRD-002', '晶圆减磨机#2', 'Grinder', 'Grinding Machine', 'Front-End', 'Disco', 'DFG-8540', 'SN-GRD-002', 'Available', '车间A-01', 'EMP-004'),
-- Dicing (3)
('EQP-DIC-001', '激光切割机#1', 'Dicing', 'Laser Dicing', 'Front-End', 'Disco', 'DFL-7340', 'SN-DIC-001', 'Running', '车间A-02', 'EMP-004'),
('EQP-DIC-002', '刀片切割机#1', 'Dicing', 'Blade Dicing', 'Front-End', 'Disco', 'DFD-6360', 'SN-DIC-002', 'Available', '车间A-02', 'EMP-004'),
('EQP-DIC-003', '刀片切割机#2', 'Dicing', 'Blade Dicing', 'Front-End', 'Disco', 'DFD-6360', 'SN-DIC-003', 'Available', '车间A-02', 'EMP-004'),
-- Die Bonder (5)
('EQP-DAB-001', '全自动贴片机#1', 'Die Bonder', 'Die Bonder', 'Assemble', 'ASM', 'AD-830', 'SN-DAB-001', 'Running', '车间B-01', 'EMP-020'),
('EQP-DAB-002', '全自动贴片机#2', 'Die Bonder', 'Die Bonder', 'Assemble', 'ASM', 'AD-830', 'SN-DAB-002', 'Running', '车间B-01', 'EMP-020'),
('EQP-DAB-003', '全自动贴片机#3', 'Die Bonder', 'Die Bonder', 'Assemble', 'Kulicke', 'AD-838H', 'SN-DAB-003', 'Available', '车间B-01', 'EMP-020'),
('EQP-DAB-004', '全自动贴片机#4', 'Die Bonder', 'Die Bonder', 'Assemble', 'ASM', 'AD-830', 'SN-DAB-004', 'Available', '车间B-01', 'EMP-020'),
('EQP-DAB-005', '全自动贴片机#5', 'Die Bonder', 'Die Bonder', 'Assemble', 'Kulicke', 'AD-838H', 'SN-DAB-005', 'Maintenance', '车间B-01', 'EMP-020'),
-- Oven (4)
('EQP-OVN-001', '固化炉#1', 'Oven', 'Curing Oven', 'Assemble', 'BTU', 'Pyramax', 'SN-OVN-001', 'Running', '车间B-02', 'EMP-020'),
('EQP-OVN-002', '固化炉#2', 'Oven', 'Curing Oven', 'Assemble', 'BTU', 'Pyramax', 'SN-OVN-002', 'Available', '车间B-02', 'EMP-020'),
('EQP-OVN-003', '固化炉#3', 'Oven', 'Curing Oven', 'Assemble', 'Heller', '1800EXL', 'SN-OVN-003', 'Available', '车间B-02', 'EMP-020'),
('EQP-OVN-004', '固化炉#4', 'Oven', 'Curing Oven', 'Assemble', 'BTU', 'Pyramax', 'SN-OVN-004', 'Available', '车间B-02', 'EMP-020'),
-- Wire Bonder (6)
('EQP-WBD-001', '全自动焊线机#1', 'Wire Bonder', 'Wire Bonder', 'Assemble', 'Kulicke', 'IConn', 'SN-WBD-001', 'Running', '车间C-01', 'EMP-021'),
('EQP-WBD-002', '全自动焊线机#2', 'Wire Bonder', 'Wire Bonder', 'Assemble', 'Kulicke', 'IConn', 'SN-WBD-002', 'Running', '车间C-01', 'EMP-021'),
('EQP-WBD-003', '全自动焊线机#3', 'Wire Bonder', 'Wire Bonder', 'Assemble', 'ASM', 'Eagle', 'SN-WBD-003', 'Running', '车间C-01', 'EMP-021'),
('EQP-WBD-004', '全自动焊线机#4', 'Wire Bonder', 'Wire Bonder', 'Assemble', 'ASM', 'Eagle', 'SN-WBD-004', 'Available', '车间C-01', 'EMP-021'),
('EQP-WBD-005', '全自动焊线机#5', 'Wire Bonder', 'Wire Bonder', 'Assemble', 'Kulicke', 'IConn', 'SN-WBD-005', 'Available', '车间C-01', 'EMP-021'),
('EQP-WBD-006', '全自动焊线机#6', 'Wire Bonder', 'Wire Bonder', 'Assemble', 'ASM', 'Eagle', 'SN-WBD-006', 'Available', '车间C-01', 'EMP-021'),
-- Mold Press (4)
('EQP-MLD-001', '塑封机#1', 'Mold Press', 'Molding Press', 'Assemble', 'TOWA', 'YME-220S', 'SN-MLD-001', 'Running', '车间D-01', 'EMP-020'),
('EQP-MLD-002', '塑封机#2', 'Mold Press', 'Molding Press', 'Assemble', 'TOWA', 'YME-220S', 'SN-MLD-002', 'Running', '车间D-01', 'EMP-020'),
('EQP-MLD-003', '塑封机#3', 'Mold Press', 'Molding Press', 'Assemble', 'TOWA', 'YME-220S', 'SN-MLD-003', 'Available', '车间D-01', 'EMP-020'),
('EQP-MLD-004', '塑封机#4', 'Mold Press', 'Molding Press', 'Assemble', 'TOWA', 'YME-220S', 'SN-MLD-004', 'Available', '车间D-01', 'EMP-020'),
-- Laser Marker (3)
('EQP-LMK-001', '激光打标机#1', 'Laser Marker', 'Laser Marker', 'Back-End', 'ESI', '532nm', 'SN-LMK-001', 'Running', '车间D-02', 'EMP-021'),
('EQP-LMK-002', '激光打标机#2', 'Laser Marker', 'Laser Marker', 'Back-End', 'ESI', '532nm', 'SN-LMK-002', 'Available', '车间D-02', 'EMP-021'),
('EQP-LMK-003', '激光打标机#3', 'Laser Marker', 'Laser Marker', 'Back-End', 'ESI', '532nm', 'SN-LMK-003', 'Available', '车间D-02', 'EMP-021'),
-- Plating (3)
('EQP-PLT-001', '电镀线#1', 'Plating Line', 'Plating Line', 'Back-End', 'Custom', 'PL-2024', 'SN-PLT-001', 'Running', '车间E-01', 'EMP-020'),
('EQP-PLT-002', '电镀线#2', 'Plating Line', 'Plating Line', 'Back-End', 'Custom', 'PL-2024', 'SN-PLT-002', 'Available', '车间E-01', 'EMP-020'),
('EQP-PLT-003', '电镀线#3', 'Plating Line', 'Plating Line', 'Back-End', 'Custom', 'PL-2024', 'SN-PLT-003', 'Available', '车间E-01', 'EMP-020'),
-- Trim/Form (3)
('EQP-TRF-001', '切筋成型机#1', 'Trim Form', 'Trim Form', 'Back-End', 'ASM', 'TF-600', 'SN-TRF-001', 'Running', '车间E-02', 'EMP-021'),
('EQP-TRF-002', '切筋成型机#2', 'Trim Form', 'Trim Form', 'Back-End', 'ASM', 'TF-600', 'SN-TRF-002', 'Available', '车间E-02', 'EMP-021'),
('EQP-TRF-003', '切筋成型机#3', 'Trim Form', 'Trim Form', 'Back-End', 'ASM', 'TF-600', 'SN-TRF-003', 'Available', '车间E-02', 'EMP-021'),
-- Tester (4)
('EQP-TST-001', '自动测试机#1', 'Tester', 'ATE Tester', 'Test', 'Advantest', 'V93K', 'SN-TST-001', 'Running', '车间F-01', 'EMP-028'),
('EQP-TST-002', '自动测试机#2', 'Tester', 'ATE Tester', 'Test', 'Teradyne', 'UltraFlex', 'SN-TST-002', 'Running', '车间F-01', 'EMP-028'),
('EQP-TST-003', '自动测试机#3', 'Tester', 'ATE Tester', 'Test', 'Advantest', 'T5830', 'SN-TST-003', 'Available', '车间F-01', 'EMP-028'),
('EQP-TST-004', '自动测试机#4', 'Tester', 'ATE Tester', 'Test', 'Teradyne', 'J750', 'SN-TST-004', 'Available', '车间F-01', 'EMP-028'),
-- Packer (3)
('EQP-PCK-001', '自动编带机#1', 'Packer', 'Auto Taping', 'Back-End', 'Asymtek', 'AJ-400', 'SN-PCK-001', 'Running', '车间F-02', 'EMP-029'),
('EQP-PCK-002', '自动编带机#2', 'Packer', 'Auto Taping', 'Back-End', 'Asymtek', 'AJ-400', 'SN-PCK-002', 'Available', '车间F-02', 'EMP-029'),
('EQP-PCK-003', '真空包装机#1', 'Packer', 'Vacuum Packing', 'Back-End', 'Custom', 'VP-300', 'SN-PCK-003', 'Available', '车间F-02', 'EMP-029');

-- ============================================================================
-- 9. RECIPES - 25 recipes for key steps
-- ============================================================================
INSERT IGNORE INTO master_recipe (recipe_id, recipe_name, equipment_group, product_id, step_code, version, is_active, parameters) VALUES
('RCP-DAB-BGA-001', 'BGA贴片标准配方', 'Die Bonder', 'PROD-001', 'DIE_ATTACH', '1.0', 1, '{"bondForce": 2.5, "bondTemp": 280, "bondSpeed": 15, "placementAccuracy": 5}'),
('RCP-DAB-QFN-001', 'QFN贴片标准配方', 'Die Bonder', 'PROD-004', 'DIE_ATTACH', '1.0', 1, '{"bondForce": 2.0, "bondTemp": 260, "bondSpeed": 18, "placementAccuracy": 5}'),
('RCP-WBD-BGA-001', 'BGA焊线金线配方', 'Wire Bonder', 'PROD-001', 'WIRE_BOND', '1.0', 1, '{"wireDiameter": 1.0, "bondForce": 18, "ultrasonicPower": 45, "bondTime": 20}'),
('RCP-WBD-QFN-001', 'QFN焊线铜线配方', 'Wire Bonder', 'PROD-004', 'WIRE_BOND', '1.0', 1, '{"wireDiameter": 0.8, "bondForce": 22, "ultrasonicPower": 50, "bondTime": 18}'),
('RCP-WBD-SOP-001', 'SOP焊线标准配方', 'Wire Bonder', 'PROD-007', 'WIRE_BOND', '1.0', 1, '{"wireDiameter": 1.2, "bondForce": 20, "ultrasonicPower": 48, "bondTime": 22}'),
('RCP-MLD-BGA-001', 'BGA塑封标准配方', 'Mold Press', 'PROD-001', 'MOLD', '1.0', 1, '{"moldTemp": 175, "moldPressure": 80, "cureTime": 90, "transferPressure": 60}'),
('RCP-MLD-QFN-001', 'QFN塑封标准配方', 'Mold Press', 'PROD-004', 'MOLD', '1.0', 1, '{"moldTemp": 175, "moldPressure": 70, "cureTime": 80, "transferPressure": 55}'),
('RCP-OVN-DA-001', '贴片固化标准曲线', 'Oven', 'PROD-001', 'CURE_DA', '1.0', 1, '{"tempZone1": 120, "tempZone2": 150, "tempZone3": 175, "conveyorSpeed": 30}'),
('RCP-OVN-CURE-001', '后固化标准曲线', 'Oven', 'PROD-001', 'CURE', '1.0', 1, '{"tempZone1": 150, "tempZone2": 175, "tempZone3": 175, "conveyorSpeed": 25}'),
('RCP-GRD-STD-001', '晶圆减薄标准配方', 'Grinder', 'PROD-001', 'GRIND', '1.0', 1, '{"targetThickness": 150, "grindSpeed": 10, "coolantFlow": 5}'),
('RCP-DIC-STD-001', '激光切割标准配方', 'Dicing', 'PROD-001', 'DICE', '1.0', 1, '{"cutSpeed": 200, "laserPower": 30, "kerfWidth": 30}'),
('RCP-LMK-BGA-001', 'BGA打标标准配方', 'Laser Marker', 'PROD-001', 'LASER_MARK', '1.0', 1, '{"laserPower": 80, "markSpeed": 500, "frequency": 20}'),
('RCP-PLT-BGA-001', 'BGA电镀锡配方', 'Plating Line', 'PROD-001', 'PLATE', '1.0', 1, '{"currentDensity": 2.0, "platingTime": 120, "tempSn": 25, "phControl": 1.5}'),
('RCP-TRF-BGA-001', 'BGA切筋标准配方', 'Trim Form', 'PROD-001', 'SINGULATE', '1.0', 1, '{"pressForce": 500, "pressSpeed": 30}'),
('RCP-TST-BGA-001', 'BGA FT测试程序', 'Tester', 'PROD-001', 'TEST_FT', '1.0', 1, '{"testVoltage": 3.3, "testCurrent": 10, "testFrequency": 100, "temperature": 25}'),
('RCP-TST-QFN-001', 'QFN FT测试程序', 'Tester', 'PROD-004', 'TEST_FT', '1.0', 1, '{"testVoltage": 5.0, "testCurrent": 20, "testFrequency": 50, "temperature": 25}'),
('RCP-TST-SOP-001', 'SOP FT测试程序', 'Tester', 'PROD-007', 'TEST_FT', '1.0', 1, '{"testVoltage": 12.0, "testCurrent": 50, "testFrequency": 1, "temperature": 25}'),
('RCP-DAB-DFN-001', 'DFN贴片标准配方', 'Die Bonder', 'PROD-009', 'DIE_ATTACH', '1.0', 1, '{"bondForce": 1.8, "bondTemp": 250, "bondSpeed": 20, "placementAccuracy": 5}'),
('RCP-DAB-TO-001', 'TO贴片标准配方', 'Die Bonder', 'PROD-011', 'DIE_ATTACH', '1.0', 1, '{"bondForce": 3.0, "bondTemp": 300, "bondSpeed": 12, "placementAccuracy": 10}'),
('RCP-DAB-QFP-001', 'QFP贴片标准配方', 'Die Bonder', 'PROD-021', 'DIE_ATTACH', '1.0', 1, '{"bondForce": 2.2, "bondTemp": 270, "bondSpeed": 16, "placementAccuracy": 5}'),
('RCP-PLT-QFN-001', 'QFN电镀标准配方', 'Plating Line', 'PROD-004', 'PLATE', '1.0', 1, '{"currentDensity": 2.5, "platingTime": 100, "tempSn": 25, "phControl": 1.5}'),
('RCP-PLT-SOP-001', 'SOP电镀标准配方', 'Plating Line', 'PROD-007', 'PLATE', '1.0', 1, '{"currentDensity": 3.0, "platingTime": 90, "tempSn": 25, "phControl": 1.8}'),
('RCP-TRF-SOP-001', 'SOP切筋标准配方', 'Trim Form', 'PROD-007', 'TRIM_FORM', '1.0', 1, '{"pressForce": 400, "pressSpeed": 25}'),
('RCP-TST-DFN-001', 'DFN FT测试程序', 'Tester', 'PROD-009', 'TEST_FT', '1.0', 1, '{"testVoltage": 1.8, "testCurrent": 5, "testFrequency": 200, "temperature": 25}'),
('RCP-TST-TO-001', 'TO FT测试程序', 'Tester', 'PROD-011', 'TEST_FT', '1.0', 1, '{"testVoltage": 24.0, "testCurrent": 100, "testFrequency": 0.1, "temperature": 25}');

-- ============================================================================
-- 10. CARRIERS - 30 carriers
-- ============================================================================
INSERT IGNORE INTO master_carrier (carrier_id, carrier_type, status, capacity, use_count, max_use_count, location) VALUES
('CAR-WF-001', 'WaferFrame', 'Available', 25, 45, 100, '仓库A'),
('CAR-WF-002', 'WaferFrame', 'Available', 25, 32, 100, '仓库A'),
('CAR-WF-003', 'WaferFrame', 'Available', 25, 67, 100, '仓库A'),
('CAR-LF-001', 'LeadFrame', 'InUse', 480, 120, 500, '车间B-01'),
('CAR-LF-002', 'LeadFrame', 'InUse', 480, 95, 500, '车间B-01'),
('CAR-LF-003', 'LeadFrame', 'Available', 480, 200, 500, '仓库B'),
('CAR-LF-004', 'LeadFrame', 'Available', 480, 180, 500, '仓库B'),
('CAR-LF-005', 'LeadFrame', 'Available', 480, 50, 500, '仓库B'),
('CAR-LF-006', 'LeadFrame', 'InUse', 240, 300, 500, '车间C-01'),
('CAR-LF-007', 'LeadFrame', 'Available', 240, 150, 500, '仓库B'),
('CAR-TAPE-001', 'TapeReel', 'Available', 5000, 10, 50, '仓库C'),
('CAR-TAPE-002', 'TapeReel', 'Available', 5000, 25, 50, '仓库C'),
('CAR-TAPE-003', 'TapeReel', 'Available', 3000, 8, 50, '仓库C'),
('CAR-TAPE-004', 'TapeReel', 'InUse', 3000, 30, 50, '车间F-02'),
('CAR-TUBE-001', 'Tube', 'Available', 50, 60, 200, '仓库C'),
('CAR-TUBE-002', 'Tube', 'Available', 50, 45, 200, '仓库C'),
('CAR-TUBE-003', 'Tube', 'InUse', 50, 120, 200, '车间E-02'),
('CAR-TRAY-001', 'Tray', 'Available', 100, 30, 300, '仓库C'),
('CAR-TRAY-002', 'Tray', 'Available', 100, 15, 300, '仓库C'),
('CAR-TRAY-003', 'Tray', 'InUse', 100, 80, 300, '车间D-01'),
('CAR-TRAY-004', 'Tray', 'Available', 100, 25, 300, '仓库C'),
('CAR-CASS-001', 'Cassette', 'Available', 25, 50, 100, '仓库A'),
('CAR-CASS-002', 'Cassette', 'Available', 25, 38, 100, '仓库A'),
('CAR-CASS-003', 'Cassette', 'InUse', 25, 72, 100, '车间A-01'),
('CAR-MAG-001', 'Magazine', 'Available', 10, 20, 50, '车间A-02'),
('CAR-MAG-002', 'Magazine', 'Available', 10, 15, 50, '车间A-02'),
('CAR-MAG-003', 'Magazine', 'InUse', 10, 35, 50, '车间C-01'),
('CAR-MAG-004', 'Magazine', 'Available', 10, 5, 50, '仓库A'),
('CAR-MAG-005', 'Magazine', 'Available', 10, 10, 50, '仓库A'),
('CAR-MAG-006', 'Magazine', 'InUse', 10, 40, 50, '车间F-01');

-- ============================================================================
-- 11. REASON CODES & DEFECT CODES
-- ============================================================================
INSERT IGNORE INTO master_reason_code (reason_code_id, category, sub_category, reason_text, applicable_to) VALUES
('RC-ENG-001', 'Engineering', 'Process', '工艺参数异常', 'Hold'),
('RC-ENG-002', 'Engineering', 'Material', '来料不良', 'Hold'),
('RC-ENG-003', 'Engineering', 'Equipment', '设备故障', 'Hold'),
('RC-QA-001', 'Quality', 'SPC', 'SPC失控', 'Hold'),
('RC-QA-002', 'Quality', 'Appearance', '外观不良', 'Scrap'),
('RC-QA-003', 'Quality', 'Electrical', '电性不良', 'Scrap'),
('RC-PROD-001', 'Production', 'Overproduction', '超产', 'TrackOut'),
('RC-PROD-002', 'Production', 'Rework', '重工', 'Rework'),
('RC-CUST-001', 'Customer', 'Spec', '客户要求变更', 'Hold');

INSERT IGNORE INTO master_defect_code (defect_code_id, defect_category, defect_text, severity) VALUES
('DF-001', 'Appearance', '划痕', 'Minor'),
('DF-002', 'Appearance', '裂纹', 'Critical'),
('DF-003', 'Appearance', '脏污', 'Minor'),
('DF-004', 'Bond', '虚焊', 'Major'),
('DF-005', 'Bond', '焊偏', 'Major'),
('DF-006', 'Bond', '焊球过大', 'Minor'),
('DF-007', 'Mold', '填充不满', 'Major'),
('DF-008', 'Mold', '气泡', 'Minor'),
('DF-009', 'Mold', '翘曲', 'Major'),
('DF-010', 'Electrical', '开路', 'Critical'),
('DF-011', 'Electrical', '短路', 'Critical'),
('DF-012', 'Electrical', '漏电', 'Major'),
('DF-013', 'Plating', '镀层不均', 'Major'),
('DF-014', 'Plating', '氧化', 'Major'),
('DF-015', 'Cut', '崩边', 'Minor');

-- ============================================================================
-- 12. ALARM RULES
-- ============================================================================
INSERT IGNORE INTO master_alarm_rule (rule_id, alarm_type, severity, threshold_yield, threshold_qty, threshold_minutes, is_enabled, notify_role) VALUES
('ARL-001', 'YieldLow', 'Critical', 95.00, NULL, NULL, 1, 'QA'),
('ARL-002', 'YieldLow', 'Warning', 98.00, NULL, NULL, 1, 'PE'),
('ARL-003', 'EquipmentDown', 'Critical', NULL, NULL, 30, 1, 'EQP'),
('ARL-004', 'LotHold', 'Warning', NULL, NULL, NULL, 1, 'Production'),
('ARL-005', 'ScrapOver', 'Critical', NULL, 500, NULL, 1, 'QA'),
('ARL-006', 'CycleTime', 'Warning', NULL, NULL, 120, 1, 'Production'),
('ARL-007', 'SPCViolation', 'Warning', NULL, NULL, NULL, 1, 'QA'),
('ARL-008', 'RecipeMismatch', 'Critical', NULL, NULL, NULL, 1, 'PE');

-- ============================================================================
-- 13. YIELD RULES
-- ============================================================================
INSERT IGNORE INTO master_yield_rule (rule_id, route_id, step_code, yield_threshold, action_type, notify_role) VALUES
('YR-001', 'BGA-STD', 'WIRE_BOND', 99.50, 'AutoHold', 'PE'),
('YR-002', 'BGA-STD', 'MOLD', 99.80, 'AutoHold', 'PE'),
('YR-003', 'BGA-STD', 'TEST_FT', 98.00, 'AutoHold', 'CQE'),
('YR-004', 'QFN-STD', 'WIRE_BOND', 99.50, 'AutoHold', 'PE'),
('YR-005', 'QFN-STD', 'TEST_FT', 97.50, 'AutoHold', 'CQE'),
('YR-006', 'SOP-STD', 'WIRE_BOND', 99.60, 'AutoHold', 'PE'),
('YR-007', 'SOP-STD', 'TEST_FT', 98.50, 'AutoHold', 'CQE'),
('YR-008', 'DFN-STD', 'TEST_FT', 97.00, 'AutoHold', 'CQE');

-- ============================================================================
-- 14. WORK ORDERS - 15 orders across multiple customers
-- ============================================================================
INSERT IGNORE INTO prod_work_order (order_id, wo_type, product_id, product_name, route_id, route_name, die_name, package_type, planned_qty, completed_qty, wafer_qty, unit_qty, customer_id, customer_name, customer_pn, internal_pn, priority, status, creator, planned_start_date, planned_end_date, actual_start_date, actual_end_date, target_cp_yield, target_ft_yield, remark) VALUES
('WO-2026-06-001', 'Parent', 'PROD-001', 'BGA-156 Power IC', 'BGA-STD', 'BGA Standard Assembly Route', 'PowerDie-7x7', 'BGA', 500000, 180000, 10, 50000, 'CUST-001', 'InnoPeak Technology', 'IPN-BGA156-PWR', 'INT-BGA156-001', 'High', 'InProduction', 'EMP-005', DATE_SUB(@now, INTERVAL 8 DAY), DATE_ADD(@now, INTERVAL 12 DAY), DATE_SUB(@now, INTERVAL 7 DAY), NULL, 99.50, 98.00, 'Q2 bulk order'),
('WO-2026-06-002', 'Parent', 'PROD-002', 'BGA-256 MCU', 'BGA-STD', 'BGA Standard Assembly Route', 'MCU-Die-10x10', 'BGA', 300000, 300000, 6, 50000, 'CUST-001', 'InnoPeak Technology', 'IPN-BGA256-MCU', 'INT-BGA256-002', 'Normal', 'Completed', 'EMP-005', DATE_SUB(@now, INTERVAL 20 DAY), DATE_SUB(@now, INTERVAL 5 DAY), DATE_SUB(@now, INTERVAL 19 DAY), DATE_SUB(@now, INTERVAL 6 DAY), 99.50, 98.50, NULL),
('WO-2026-06-003', 'Parent', 'PROD-004', 'QFN-32 Sensor', 'QFN-STD', 'QFN Standard Assembly Route', 'Sensor-Die-3x3', 'QFN', 1000000, 650000, 20, 50000, 'CUST-002', 'CoreChip Semiconductor', 'CCS-QFN32-SEN', 'INT-QFN32-004', 'High', 'InProduction', 'EMP-005', DATE_SUB(@now, INTERVAL 10 DAY), DATE_ADD(@now, INTERVAL 10 DAY), DATE_SUB(@now, INTERVAL 9 DAY), NULL, 99.00, 97.50, 'Automotive grade quality requirement'),
('WO-2026-06-004', 'Parent', 'PROD-005', 'QFN-48 LED Driver', 'QFN-STD', 'QFN Standard Assembly Route', 'LED-Die-4x4', 'QFN', 800000, 800000, 16, 50000, 'CUST-003', 'MicroTech Industries', 'MTI-QFN48-LED', 'INT-QFN48-005', 'Normal', 'Completed', 'EMP-005', DATE_SUB(@now, INTERVAL 18 DAY), DATE_SUB(@now, INTERVAL 3 DAY), DATE_SUB(@now, INTERVAL 17 DAY), DATE_SUB(@now, INTERVAL 4 DAY), 99.00, 98.00, NULL),
('WO-2026-06-005', 'Parent', 'PROD-007', 'SOP-8 MOSFET', 'SOP-STD', 'SOP Standard Assembly Route', 'MOS-Die-3x2', 'SOP', 2000000, 1200000, 40, 50000, 'CUST-004', 'Nexus IC Design', 'NXI-SOP8-MOS', 'INT-SOP8-007', 'Normal', 'InProduction', 'EMP-005', DATE_SUB(@now, INTERVAL 12 DAY), DATE_ADD(@now, INTERVAL 8 DAY), DATE_SUB(@now, INTERVAL 11 DAY), NULL, 99.20, 98.50, NULL),
('WO-2026-06-006', 'Parent', 'PROD-009', 'DFN-6 LDO', 'DFN-STD', 'DFN Standard Assembly Route', 'LDO-Die-1x1', 'DFN', 5000000, 2500000, 100, 50000, 'CUST-005', 'GlobalPower Semi', 'GPS-DFN6-LDO', 'INT-DFN6-009', 'High', 'InProduction', 'EMP-005', DATE_SUB(@now, INTERVAL 14 DAY), DATE_ADD(@now, INTERVAL 6 DAY), DATE_SUB(@now, INTERVAL 13 DAY), NULL, 99.00, 97.00, NULL),
('WO-2026-06-007', 'Parent', 'PROD-011', 'TO-220 IGBT', 'TO-STD', 'TO Standard Assembly Route', 'IGBT-Die-6x6', 'TO', 200000, 200000, 4, 50000, 'CUST-006', 'SinoWave Electronics', 'SWE-TO220-IGBT', 'INT-TO220-011', 'Normal', 'Completed', 'EMP-005', DATE_SUB(@now, INTERVAL 25 DAY), DATE_SUB(@now, INTERVAL 8 DAY), DATE_SUB(@now, INTERVAL 24 DAY), DATE_SUB(@now, INTERVAL 9 DAY), 99.00, 98.00, 'Industrial grade'),
('WO-2026-06-008', 'Parent', 'PROD-013', 'BGA-208 FPGA', 'BGA-STD', 'BGA Standard Assembly Route', 'FPGA-Die-15x15', 'BGA', 100000, 0, 2, 50000, 'CUST-007', 'Apex Semiconductor', 'APS-BGA208-FPGA', 'INT-BGA208-013', 'High', 'Created', 'EMP-005', DATE_ADD(@now, INTERVAL 2 DAY), DATE_ADD(@now, INTERVAL 20 DAY), NULL, NULL, 99.50, 98.00, 'New product introduction'),
('WO-2026-06-009', 'Parent', 'PROD-015', 'BGA-64 WiFi Module', 'BGA-STD', 'BGA Standard Assembly Route', 'WiFi-Die-5x5', 'BGA', 600000, 450000, 12, 50000, 'CUST-008', 'BridgeLink Technology', 'BLT-BGA64-WIFI', 'INT-BGA64-015', 'Normal', 'InProduction', 'EMP-005', DATE_SUB(@now, INTERVAL 8 DAY), DATE_ADD(@now, INTERVAL 7 DAY), DATE_SUB(@now, INTERVAL 7 DAY), NULL, 99.30, 98.20, NULL),
('WO-2026-06-010', 'Parent', 'PROD-017', 'QFN-40 Power Mgmt', 'QFN-STD', 'QFN Standard Assembly Route', 'PMU-Die-5x5', 'QFN', 400000, 400000, 8, 50000, 'CUST-009', 'VoltageTech Corp', 'VTC-QFN40-PMU', 'INT-QFN40-017', 'Normal', 'Completed', 'EMP-005', DATE_SUB(@now, INTERVAL 22 DAY), DATE_SUB(@now, INTERVAL 7 DAY), DATE_SUB(@now, INTERVAL 21 DAY), DATE_SUB(@now, INTERVAL 8 DAY), 99.00, 97.50, NULL),
('WO-2026-06-011', 'Parent', 'PROD-019', 'BGA-144 SoC', 'BGA-STD', 'BGA Standard Assembly Route', 'SoC-Die-8x8', 'BGA', 150000, 100000, 3, 50000, 'CUST-010', 'Crystal IC Solutions', 'CRY-BGA144-SOC', 'INT-BGA144-019', 'High', 'InProduction', 'EMP-005', DATE_SUB(@now, INTERVAL 6 DAY), DATE_ADD(@now, INTERVAL 14 DAY), DATE_SUB(@now, INTERVAL 5 DAY), NULL, 99.50, 98.00, 'Auto grade, 100% FT test'),
('WO-2026-06-012', 'Parent', 'PROD-021', 'QFP-44 MCU', 'QFP-STD', 'QFP Standard Assembly Route', 'MCU-Die-5x5', 'QFP', 700000, 350000, 14, 50000, 'CUST-011', 'Thunder Semiconductor', 'THS-QFP44-MCU', 'INT-QFP44-021', 'Normal', 'InProduction', 'EMP-005', DATE_SUB(@now, INTERVAL 9 DAY), DATE_ADD(@now, INTERVAL 11 DAY), DATE_SUB(@now, INTERVAL 8 DAY), NULL, 99.00, 98.00, NULL),
('WO-2026-06-013', 'Parent', 'PROD-025', 'SOP-8 Voltage Ref', 'SOP-STD', 'SOP Standard Assembly Route', 'VREF-Die-1x1', 'SOP', 3000000, 3000000, 60, 50000, 'CUST-013', 'SmartPower Electronics', 'SPE-SOP8-VREF', 'INT-SOP8-025', 'Low', 'Completed', 'EMP-005', DATE_SUB(@now, INTERVAL 15 DAY), DATE_SUB(@now, INTERVAL 1 DAY), DATE_SUB(@now, INTERVAL 14 DAY), DATE_SUB(@now, INTERVAL 2 DAY), 99.50, 99.00, NULL),
('WO-2026-06-014', 'Parent', 'PROD-027', 'BGA-96 Display Driver', 'BGA-STD', 'BGA Standard Assembly Route', 'DDI-Die-6x6', 'BGA', 250000, 0, 5, 50000, 'CUST-014', 'Fusion Semiconductor', 'FSM-BGA96-DDI', 'INT-BGA96-027', 'Normal', 'Created', 'EMP-005', DATE_ADD(@now, INTERVAL 5 DAY), DATE_ADD(@now, INTERVAL 22 DAY), NULL, NULL, 99.30, 98.20, NULL),
('WO-2026-06-015', 'Parent', 'PROD-045', 'QFN-20 Temp Sensor Auto', 'QFN-STD', 'QFN Standard Assembly Route', 'Temp-Die-2x2-Auto', 'QFN', 500000, 200000, 10, 50000, 'CUST-005', 'GlobalPower Semi', 'GPS-QFN20-TEMP-A', 'INT-QFN20-045', 'High', 'InProduction', 'EMP-005', DATE_SUB(@now, INTERVAL 5 DAY), DATE_ADD(@now, INTERVAL 15 DAY), DATE_SUB(@now, INTERVAL 4 DAY), NULL, 99.00, 97.50, 'Auto grade, 0 PPM target');

-- ============================================================================
-- 15. LOTS - 40 lots with various statuses
-- ============================================================================

-- WO-001 lots (BGA Power IC, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-BGA-20260601-001', 'WO-2026-06-001', 'PROD-001', 'BGA-156 Power IC', 'PowerDie-7x7', 'BGA', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 98500, 800, 0, 49500, 300, 'WF-20260520-001'),
('LOT-BGA-20260601-002', 'WO-2026-06-001', 'PROD-001', 'BGA-156 Power IC', 'PowerDie-7x7', 'BGA', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 99200, 500, 0, 49700, 200, 'WF-20260520-002'),
('LOT-BGA-20260602-001', 'WO-2026-06-001', 'PROD-001', 'BGA-156 Power IC', 'PowerDie-7x7', 'BGA', 'BGA-STD', '1.0', 40, 'CURE_DA', 'Hold', 'Assemble', 50000, 0, 'High', 50000, 98000, 1200, 0, 49000, 500, 'WF-20260520-003'),
('LOT-BGA-20260602-002', 'WO-2026-06-001', 'PROD-001', 'BGA-156 Power IC', 'PowerDie-7x7', 'BGA', 'BGA-STD', '1.0', 80, 'LASER_MARK', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 49500, 300, 0, 49500, 300, 'WF-20260521-001'),
('LOT-BGA-20260603-001', 'WO-2026-06-001', 'PROD-001', 'BGA-156 Power IC', 'PowerDie-7x7', 'BGA', 'BGA-STD', '1.0', 110, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 148500, 600, 0, 49200, 400, 'WF-20260521-002');

-- WO-002 lots (BGA MCU, Completed)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, is_archived, wafer_lot_id) VALUES
('LOT-BGA-20260515-001', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49800, 0, 'Normal', 50000, 49800, 100, 0, 49800, 100, 1, 'WF-20260510-001'),
('LOT-BGA-20260515-002', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49600, 0, 'Normal', 50000, 49600, 200, 0, 49600, 200, 1, 'WF-20260510-002'),
('LOT-BGA-20260516-001', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49700, 0, 'Normal', 50000, 49700, 150, 0, 49700, 150, 1, 'WF-20260510-003'),
('LOT-BGA-20260516-002', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49500, 0, 'Normal', 50000, 49500, 250, 0, 49500, 250, 1, 'WF-20260510-004'),
('LOT-BGA-20260517-001', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49900, 0, 'Normal', 50000, 49900, 50, 0, 49900, 50, 1, 'WF-20260511-001'),
('LOT-BGA-20260517-002', 'WO-2026-06-002', 'PROD-002', 'BGA-256 MCU', 'MCU-Die-10x10', 'BGA', 'BGA-STD', '1.0', 120, 'FINAL_PACK', 'Completed', 'Assemble', 49800, 0, 'Normal', 50000, 49800, 100, 0, 49800, 100, 1, 'WF-20260511-002');

-- WO-003 lots (QFN Sensor, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-QFN-20260601-001', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 80, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 248000, 2500, 0, 49600, 300, 'WF-20260518-001'),
('LOT-QFN-20260601-002', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 80, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 249000, 2200, 0, 49700, 250, 'WF-20260518-002'),
('LOT-QFN-20260602-001', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 30, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 149000, 1800, 0, 49800, 150, 'WF-20260518-003'),
('LOT-QFN-20260602-002', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 30, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 148500, 2000, 0, 49500, 400, 'WF-20260519-001'),
('LOT-QFN-20260603-001', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 10, 'DIE_ATTACH', 'Waiting', 'Assemble', 50000, 0, 'Normal', 50000, 198000, 2100, 0, 49500, 300, 'WF-20260519-002'),
('LOT-QFN-20260603-002', 'WO-2026-06-003', 'PROD-004', 'QFN-32 Sensor', 'Sensor-Die-3x3', 'QFN', 'QFN-STD', '1.0', 10, 'DIE_ATTACH', 'Waiting', 'Assemble', 50000, 0, 'Normal', 50000, 198500, 2000, 0, 49600, 250, 'WF-20260519-003');

-- WO-005 lots (SOP MOSFET, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-SOP-20260601-001', 'WO-2026-06-005', 'PROD-007', 'SOP-8 MOSFET', 'MOS-Die-3x2', 'SOP', 'SOP-STD', '1.0', 90, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 549000, 8000, 0, 49800, 200, 'WF-20260515-001'),
('LOT-SOP-20260601-002', 'WO-2026-06-005', 'PROD-007', 'SOP-8 MOSFET', 'MOS-Die-3x2', 'SOP', 'SOP-STD', '1.0', 60, 'PLATE', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 548500, 8200, 0, 49700, 250, 'WF-20260515-002'),
('LOT-SOP-20260602-001', 'WO-2026-06-005', 'PROD-007', 'SOP-8 MOSFET', 'MOS-Die-3x2', 'SOP', 'SOP-STD', '1.0', 30, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 548000, 8500, 0, 49600, 300, 'WF-20260515-003'),
('LOT-SOP-20260602-002', 'WO-2026-06-005', 'PROD-007', 'SOP-8 MOSFET', 'MOS-Die-3x2', 'SOP', 'SOP-STD', '1.0', 30, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 547500, 8800, 0, 49500, 350, 'WF-20260516-001');

-- WO-006 lots (DFN LDO, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-DFN-20260601-001', 'WO-2026-06-006', 'PROD-009', 'DFN-6 LDO', 'LDO-Die-1x1', 'DFN', 'DFN-STD', '1.0', 80, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 1248000, 15000, 0, 49900, 100, 'WF-20260512-001'),
('LOT-DFN-20260601-002', 'WO-2026-06-006', 'PROD-009', 'DFN-6 LDO', 'LDO-Die-1x1', 'DFN', 'DFN-STD', '1.0', 80, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 1247500, 15200, 0, 49800, 150, 'WF-20260512-002'),
('LOT-DFN-20260602-001', 'WO-2026-06-006', 'PROD-009', 'DFN-6 LDO', 'LDO-Die-1x1', 'DFN', 'DFN-STD', '1.0', 40, 'MOLD', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 1247000, 15500, 0, 49700, 200, 'WF-20260512-003'),
('LOT-DFN-20260603-001', 'WO-2026-06-006', 'PROD-009', 'DFN-6 LDO', 'LDO-Die-1x1', 'DFN', 'DFN-STD', '1.0', 10, 'DIE_ATTACH', 'Waiting', 'Assemble', 50000, 0, 'Normal', 50000, 1246000, 16000, 0, 49500, 300, 'WF-20260513-001');

-- WO-009 lots (BGA WiFi, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-BGA-20260601-003', 'WO-2026-06-009', 'PROD-015', 'BGA-64 WiFi Module', 'WiFi-Die-5x5', 'BGA', 'BGA-STD', '1.0', 110, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 198000, 4000, 0, 49500, 400, 'WF-20260522-001'),
('LOT-BGA-20260602-003', 'WO-2026-06-009', 'PROD-015', 'BGA-64 WiFi Module', 'WiFi-Die-5x5', 'BGA', 'BGA-STD', '1.0', 90, 'PLATE', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 197500, 4200, 0, 49400, 450, 'WF-20260522-002'),
('LOT-BGA-20260603-002', 'WO-2026-06-009', 'PROD-015', 'BGA-64 WiFi Module', 'WiFi-Die-5x5', 'BGA', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 197000, 4500, 0, 49200, 500, 'WF-20260523-001'),
('LOT-BGA-20260603-003', 'WO-2026-06-009', 'PROD-015', 'BGA-64 WiFi Module', 'WiFi-Die-5x5', 'BGA', 'BGA-STD', '1.0', 20, 'DIE_ATTACH', 'Waiting', 'Assemble', 50000, 0, 'Normal', 50000, 196500, 4800, 0, 49100, 550, 'WF-20260523-002');

-- WO-011 lots (BGA SoC, InProgress, Auto grade)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, grade, wafer_lot_id) VALUES
('LOT-BGA-20260601-004', 'WO-2026-06-011', 'PROD-019', 'BGA-144 SoC', 'SoC-Die-8x8', 'BGA', 'BGA-STD', '1.0', 80, 'LASER_MARK', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 50000, 800, 0, 49600, 300, 'A', 'WF-20260525-001'),
('LOT-BGA-20260602-004', 'WO-2026-06-011', 'PROD-019', 'BGA-144 SoC', 'SoC-Die-8x8', 'BGA', 'BGA-STD', '1.0', 50, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'High', 50000, 49500, 900, 0, 49300, 350, 'A', 'WF-20260525-002');

-- WO-012 lots (QFP MCU, InProgress)
INSERT IGNORE INTO prod_lot (lot_id, order_id, product_id, product_name, die_name, package_type, route_id, route_version, current_step_seq, current_step_code, status, process_stage, unit_count, strip_count, priority, original_qty, total_pass_qty, total_scrap_qty, total_hold_qty, qty_pass, qty_fail, wafer_lot_id) VALUES
('LOT-QFP-20260601-001', 'WO-2026-06-012', 'PROD-021', 'QFP-44 MCU', 'MCU-Die-5x5', 'QFP', 'QFP-STD', '1.0', 80, 'TEST_FT', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 148000, 5000, 0, 49000, 500, 'WF-20260520-004'),
('LOT-QFP-20260602-001', 'WO-2026-06-012', 'PROD-021', 'QFP-44 MCU', 'MCU-Die-5x5', 'QFP', 'QFP-STD', '1.0', 40, 'MOLD', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 147500, 5200, 0, 48800, 550, 'WF-20260520-005'),
('LOT-QFP-20260603-001', 'WO-2026-06-012', 'PROD-021', 'QFP-44 MCU', 'MCU-Die-5x5', 'QFP', 'QFP-STD', '1.0', 20, 'WIRE_BOND', 'InProduction', 'Assemble', 50000, 0, 'Normal', 50000, 147000, 5500, 0, 48500, 600, 'WF-20260521-003'),
('LOT-QFP-20260603-002', 'WO-2026-06-012', 'PROD-021', 'QFP-44 MCU', 'MCU-Die-5x5', 'QFP', 'QFP-STD', '1.0', 10, 'DIE_ATTACH', 'Waiting', 'Assemble', 50000, 0, 'Normal', 50000, 146500, 5800, 0, 48200, 650, 'WF-20260521-004');

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
('SR-LOT-BGA-003-S100', 'LOT-BGA-20260603-001', 'BGA-STD', '1.0', 100, 'TEST_FT', '成品测试', 'InProgress', 'EQP-TST-001', DATE_SUB(@now, INTERVAL 2 HOUR), 'EMP-028', 48700, 'RCP-TST-BGA-001');

-- ============================================================================
-- 17. COMPLAINT 8D RECORDS - Sample complaints
-- ============================================================================

INSERT IGNORE INTO complaint_8d (complaint_id, customer_id, customer_name, lot_id, product_id, defect_type, severity, status, d1_team_members, d2_problem_description, d3_containment_action, d4_root_cause, d5_permanent_action, d6_implementation, d7_prevention, created_by, due_date) VALUES
('CMT-2026-06-001', 'CUST-001', 'InnoPeak Technology', 'LOT-BGA-20260601-001', 'PROD-001', 'WireBondPullForce', 'Critical', 'Open', '["EMP-001", "EMP-010", "EMP-021"]', 'Customer reported wire bond pull force measured at 3.2g, below minimum spec of 4.0g. Affected lot: LOT-BGA-20260601-001, approximately 5000 units.', 'All material from affected lot quarantined at customer site. Replacement lot shipped with 100% wire bond pull force test data.', 'Investigation revealed wire bond parameter drift on EQP-WBD-001. Gold wire tension was set 15% below optimal due to incorrect recipe version.', 'Updated wire bond recipe on all 4 wire bond machines to v1.3. Added SPC monitoring for wire bond pull force with control limits at 4.0g-6.5g.', '100% wire bond pull force test added for 3 consecutive lots. Recipe change approval process implemented.', 'Quarterly recipe audit established. SPC alarm rules updated for wire bond parameters.', 'EMP-001', DATE_ADD(@now, INTERVAL 15 DAY));

-- ============================================================================
-- 18. NPI PROJECTS - Sample NPI projects
-- ============================================================================

INSERT IGNORE INTO npi_project (project_id, project_name, customer_id, product_id, status, phase, start_date, target_completion, actual_completion, created_by) VALUES
('NPI-2026-06-001', 'BGA-144 Automotive SoC NPI', 'CUST-001', 'PROD-019', 'InProgress', 'EngineeringSample', DATE_SUB(@now, INTERVAL 60 DAY), DATE_ADD(@now, INTERVAL 120 DAY), NULL, 'EMP-001'),
('NPI-2026-06-002', 'QFN-20 Temp Sensor Auto NPI', 'CUST-005', 'PROD-045', 'InProgress', 'Qualification', DATE_SUB(@now, INTERVAL 90 DAY), DATE_ADD(@now, INTERVAL 45 DAY), NULL, 'EMP-002');

-- ============================================================================
-- 19. SPC MEASUREMENTS - Sample SPC data for wire bond
-- ============================================================================

INSERT IGNORE INTO spc_measurement (lot_id, step_code, parameter_name, measured_value, usl, lsl, target_value, equipment_id, operator_id, measured_at, is_out_of_control) VALUES
('LOT-BGA-20260601-001', 'WIRE_BOND', 'WireBondPullForce', 5.2000, 6.5000, 4.0000, 5.0000, 'EQP-WBD-001', 'EMP-021', DATE_SUB(@now, INTERVAL 3 DAY), 0),
('LOT-BGA-20260601-001', 'WIRE_BOND', 'WireBondPullForce', 4.8000, 6.5000, 4.0000, 5.0000, 'EQP-WBD-001', 'EMP-021', DATE_SUB(@now, INTERVAL 3 DAY), 0),
('LOT-BGA-20260601-001', 'WIRE_BOND', 'WireBondPullForce', 5.5000, 6.5000, 4.0000, 5.0000, 'EQP-WBD-001', 'EMP-021', DATE_SUB(@now, INTERVAL 3 DAY), 0),
('LOT-BGA-20260601-001', 'WIRE_BOND', 'WireBondPullForce', 5.1000, 6.5000, 4.0000, 5.0000, 'EQP-WBD-001', 'EMP-021', DATE_SUB(@now, INTERVAL 3 DAY), 0),
('LOT-BGA-20260601-001', 'WIRE_BOND', 'WireBondPullForce', 4.9000, 6.5000, 4.0000, 5.0000, 'EQP-WBD-001', 'EMP-021', DATE_SUB(@now, INTERVAL 3 DAY), 0);