-- ============================================================
-- MES系统甲方测试数据 - SQL插入脚本
-- 创建日期: 2026-06-20
-- 用途: 模拟芯联半导体制造公司的复杂业务场景
-- ============================================================

USE `mes_prod`;

-- ============================================================
-- 一、基础主数据
-- ============================================================

-- 1.1 产品数据
INSERT INTO `master_product` VALUES
('PROD-X7', 'X7系列控制芯片', 'X7-DIE', 'QFN-48', 'CUST-A', '客户A', 'A-X7-001', 'MES-X7', 'Active', '2026-06-20 08:00:00'),
('PROD-Y3', 'Y3功率MOSFET', 'Y3-DIE', 'SOP-8', 'CUST-B', '客户B', 'B-Y3-002', 'MES-Y3', 'Active', '2026-06-20 08:00:00'),
('PROD-Z9', 'Z9传感器芯片', 'Z9-DIE', 'BGA-64', 'CUST-C', '客户C', 'C-Z9-003', 'MES-Z9', 'Active', '2026-06-20 08:00:00');

-- 1.2 工艺路线 - X7系列
INSERT INTO `master_route` VALUES
('RT-X7-1.0', 'X7系列工艺路线', '1.0', 'PROD-X7', 'QFN-48', 1, 1, 'QA-USER', '2026-06-19 16:00:00', '2026-06-19 10:00:00');

INSERT INTO `master_route_step` (`route_id`, `step_seq`, `step_code`, `step_name`, `equipment_group`, `is_rework`) VALUES
('RT-X7-1.0', 10, 'DIE-BOND', '芯片贴装', 'DIE_BONDER', 1),
('RT-X7-1.0', 20, 'WIRE-BOND', '引线键合', 'WIRE_BONDER', 1),
('RT-X7-1.0', 30, 'ENCAPS', '塑封成型', 'MOULDING', 0),
('RT-X7-1.0', 40, 'PLATING', '电镀处理', 'PLATING', 1),
('RT-X7-1.0', 50, 'MARK', '激光打标', 'LASER_MARK', 0),
('RT-X7-1.0', 60, 'TEST-CP', 'CP测试', 'TESTER', 0),
('RT-X7-1.0', 70, 'PKG-INS', '包装检验', 'PACKAGING', 0);

-- 1.3 工艺路线 - Y3系列
INSERT INTO `master_route` VALUES
('RT-Y3-1.0', 'Y3系列工艺路线', '1.0', 'PROD-Y3', 'SOP-8', 1, 1, 'QA-USER', '2026-06-19 16:00:00', '2026-06-19 10:00:00');

INSERT INTO `master_route_step` (`route_id`, `step_seq`, `step_code`, `step_name`, `equipment_group`, `is_rework`) VALUES
('RT-Y3-1.0', 10, 'DIE-BOND', '芯片贴装', 'DIE_BONDER', 1),
('RT-Y3-1.0', 20, 'WIRE-BOND', '引线键合', 'WIRE_BONDER', 1),
('RT-Y3-1.0', 30, 'ENCAPS', '塑封成型', 'MOULDING', 0),
('RT-Y3-1.0', 40, 'TRIM', '切筋成型', 'TRIM_FORM', 0),
('RT-Y3-1.0', 50, 'PLATING', '电镀处理', 'PLATING', 1),
('RT-Y3-1.0', 60, 'TEST-FT', 'FT测试', 'TESTER', 0),
('RT-Y3-1.0', 70, 'PKG-INS', '包装检验', 'PACKAGING', 0);

-- 1.4 工艺路线 - Z9系列
INSERT INTO `master_route` VALUES
('RT-Z9-1.0', 'Z9系列工艺路线', '1.0', 'PROD-Z9', 'BGA-64', 1, 1, 'QA-USER', '2026-06-19 16:00:00', '2026-06-19 10:00:00');

INSERT INTO `master_route_step` (`route_id`, `step_seq`, `step_code`, `step_name`, `equipment_group`, `is_rework`) VALUES
('RT-Z9-1.0', 10, 'DIE-BOND', '芯片贴装', 'DIE_BONDER', 1),
('RT-Z9-1.0', 20, 'WIRE-BOND', '引线键合', 'WIRE_BONDER', 1),
('RT-Z9-1.0', 30, 'UNDERFILL', '底部填充', 'DISPENSER', 1),
('RT-Z9-1.0', 40, 'ENCAPS', '塑封成型', 'MOULDING', 0),
('RT-Z9-1.0', 50, 'BALL-ATTACH', '植球', 'BALL_MOUNT', 1),
('RT-Z9-1.0', 60, 'REFLOW', '回流焊', 'REFLOW_OVEN', 0),
('RT-Z9-1.0', 70, 'TEST-CP', 'CP测试', 'TESTER', 0),
('RT-Z9-1.0', 80, 'PKG-INS', '包装检验', 'PACKAGING', 0);

-- 1.5 重工路线 - X7返回WIRE-BOND
INSERT INTO `master_route` VALUES
('RT-X7-RW-10', 'X7重工路线-返回键合', '1.0', 'PROD-X7', 'QFN-48', 1, 1, 'ENG-USER', '2026-06-20 09:00:00', '2026-06-20 08:00:00');

INSERT INTO `master_route_step` (`route_id`, `step_seq`, `step_code`, `step_name`, `equipment_group`, `is_rework`) VALUES
('RT-X7-RW-10', 20, 'WIRE-BOND', '引线键合(重工)', 'WIRE_BONDER', 1),
('RT-X7-RW-10', 30, 'ENCAPS', '塑封成型', 'MOULDING', 0),
('RT-X7-RW-10', 40, 'PLATING', '电镀处理', 'PLATING', 1),
('RT-X7-RW-10', 50, 'MARK', '激光打标', 'LASER_MARK', 0),
('RT-X7-RW-10', 60, 'TEST-CP', 'CP测试', 'TESTER', 0),
('RT-X7-RW-10', 70, 'PKG-INS', '包装检验', 'PACKAGING', 0);

-- 1.6 设备数据
INSERT INTO `master_equipment` VALUES
('EQ-DB-01', '贴片机#1', 'DIE_BONDER', 'ASM-800', 'Available', NULL, NULL, 'Line-A', '张三', '2026-06-15 08:00:00', 500, 200, '2026-06-20 08:00:00'),
('EQ-DB-02', '贴片机#2', 'DIE_BONDER', 'ASM-800', 'Available', NULL, NULL, 'Line-B', '李四', '2026-06-15 08:00:00', 500, 180, '2026-06-20 08:00:00'),
('EQ-WB-01', '键合机#1', 'WIRE_BONDER', 'K&S-4024', 'Available', NULL, NULL, 'Line-A', '张三', '2026-06-15 08:00:00', 500, 320, '2026-06-20 08:00:00'),
('EQ-WB-02', '键合机#2', 'WIRE_BONDER', 'K&S-4024', 'Available', NULL, NULL, 'Line-B', '李四', '2026-06-15 08:00:00', 500, 280, '2026-06-20 08:00:00'),
('EQ-WB-03', '键合机#3', 'WIRE_BONDER', 'K&S-4024', 'Available', NULL, NULL, 'Line-C', '王五', '2026-06-15 08:00:00', 500, 150, '2026-06-20 08:00:00'),
('EQ-MD-01', '塑封机#1', 'MOULDING', 'TOWA-LMC', 'Available', NULL, NULL, 'Line-A', '张三', '2026-06-15 08:00:00', 500, 450, '2026-06-20 08:00:00'),
('EQ-PL-01', '电镀线#1', 'PLATING', 'PLT-500', 'Available', NULL, NULL, 'Area-C', '赵六', '2026-06-15 08:00:00', 500, 400, '2026-06-20 08:00:00'),
('EQ-TS-01', '测试机#1', 'TESTER', 'Advantest-93K', 'Available', NULL, NULL, 'Test-Room', '周七', '2026-06-15 08:00:00', 500, 380, '2026-06-20 08:00:00'),
('EQ-TS-02', '测试机#2', 'TESTER', 'Advantest-93K', 'Available', NULL, NULL, 'Test-Room', '周七', '2026-06-15 08:00:00', 500, 350, '2026-06-20 08:00:00'),
('EQ-TF-01', '切筋机#1', 'TRIM_FORM', 'TF-100', 'Available', NULL, NULL, 'Line-B', '李四', '2026-06-15 08:00:00', 500, 220, '2026-06-20 08:00:00'),
('EQ-DP-01', '点胶机#1', 'DISPENSER', 'NORDSON', 'Available', NULL, NULL, 'Line-C', '王五', '2026-06-15 08:00:00', 500, 160, '2026-06-20 08:00:00'),
('EQ-BM-01', '植球机#1', 'BALL_MOUNT', 'BM-200', 'Available', NULL, NULL, 'Line-C', '王五', '2026-06-15 08:00:00', 500, 140, '2026-06-20 08:00:00'),
('EQ-RO-01', '回流炉#1', 'REFLOW_OVEN', 'BTU-Pyramax', 'Available', NULL, NULL, 'Line-C', '王五', '2026-06-15 08:00:00', 500, 130, '2026-06-20 08:00:00'),
('EQ-LM-01', '打标机#1', 'LASER_MARK', 'Keyence-MD', 'Available', NULL, NULL, 'Line-A', '张三', '2026-06-15 08:00:00', 500, 480, '2026-06-20 08:00:00'),
('EQ-PK-01', '包装机#1', 'PACKAGING', 'PKG-300', 'Available', NULL, NULL, 'Final-Area', '孙八', '2026-06-15 08:00:00', 500, 300, '2026-06-20 08:00:00');

-- 1.7 载具数据
INSERT INTO `master_carrier` VALUES
('CR-TRAY-01', 'TRAY', 'Available', NULL, 25, 50, 200, '2026-06-18 10:00:00', 100, 'Line-A', '2026-06-20 08:00:00'),
('CR-TRAY-02', 'TRAY', 'Available', NULL, 25, 48, 200, '2026-06-18 10:00:00', 100, 'Line-B', '2026-06-20 08:00:00'),
('CR-MAG-01', 'MAGAZINE', 'Available', NULL, 100, 120, 500, '2026-06-17 14:00:00', 200, 'Line-A', '2026-06-20 08:00:00'),
('CR-MAG-02', 'MAGAZINE', 'Available', NULL, 100, 95, 500, '2026-06-17 14:00:00', 200, 'Line-C', '2026-06-20 08:00:00'),
('CR-FOUP-01', 'FOUP', 'Available', NULL, 25, 30, 300, '2026-06-19 08:00:00', 150, 'CleanRoom', '2026-06-20 08:00:00'),
('CR-FOUP-02', 'FOUP', 'Available', NULL, 25, 28, 300, '2026-06-19 08:00:00', 150, 'CleanRoom', '2026-06-20 08:00:00');

-- 1.8 Recipe数据
INSERT INTO `master_recipe` VALUES
('RCP-X7-DB-10', 'X7贴装配方', 'DIE_BONDER', 'PROD-X7', 'DIE-BOND', '1.0', 1, '{"温度":250,"压力":5.2,"速度":120}', 'ENG-USER', '2026-06-19 16:00:00', '2026-06-19 10:00:00'),
('RCP-X7-WB-20', 'X7键合配方', 'WIRE_BONDER', 'PROD-X7', 'WIRE-BOND', '1.0', 1, '{"线径":25,"超声功率":65,"压力":40}', 'ENG-USER', '2026-06-19 16:00:00', '2026-06-19 10:00:00'),
('RCP-X7-CP-60', 'X7测试配方', 'TESTER', 'PROD-X7', 'TEST-CP', '1.0', 1, '{"电压":3.3,"频率":100,"温度":25}', 'ENG-USER', '2026-06-19 16:00:00', '2026-06-19 10:00:00'),
('RCP-Y3-DB-10', 'Y3贴装配方', 'DIE_BONDER', 'PROD-Y3', 'DIE-BOND', '1.0', 1, '{"温度":260,"压力":5.5,"速度":130}', 'ENG-USER', '2026-06-19 16:00:00', '2026-06-19 10:00:00'),
('RCP-Y3-WB-20', 'Y3键合配方', 'WIRE_BONDER', 'PROD-Y3', 'WIRE-BOND', '1.0', 1, '{"线径":50,"超声功率":70,"压力":45}', 'ENG-USER', '2026-06-19 16:00:00', '2026-06-19 10:00:00'),
('RCP-Z9-BM-50', 'Z9植球配方', 'BALL_MOUNT', 'PROD-Z9', 'BALL-ATTACH', '1.0', 1, '{"球径":0.5,"温度":230,"速度":80}', 'ENG-USER', '2026-06-19 16:00:00', '2026-06-19 10:00:00');

-- ============================================================
-- 二、工单数据
-- ============================================================

INSERT INTO `prod_work_order` VALUES
('WO-2026-001', 'PROD-X7', 'X7系列控制芯片', 'RT-X7-1.0', 'X7系列工艺路线', 'X7-DIE', 'QFN-48', 5000, 5000, 20, 0, 'CUST-A', '客户A', 'A-X7-001', 'MES-X7', 'High', 'InProgress', 'ADMIN', '2026-06-20 08:00:00', '2026-06-22 18:00:00', '2026-06-20 08:00:00', NULL, 99.00, 98.00, 'X7系列高优先级工单', '2026-06-20 08:00:00', '2026-06-20 08:30:00'),
('WO-2026-002', 'PROD-Y3', 'Y3功率MOSFET', 'RT-Y3-1.0', 'Y3系列工艺路线', 'Y3-DIE', 'SOP-8', 8000, 4000, 30, 0, 'CUST-B', '客户B', 'B-Y3-002', 'MES-Y3', 'Normal', 'InProgress', 'ADMIN', '2026-06-20 08:00:00', '2026-06-23 18:00:00', '2026-06-20 08:00:00', NULL, 99.00, 98.00, 'Y3系列常规工单', '2026-06-20 08:00:00', '2026-06-20 08:30:00'),
('WO-2026-003', 'PROD-Z9', 'Z9传感器芯片', 'RT-Z9-1.0', 'Z9系列工艺路线', 'Z9-DIE', 'BGA-64', 3000, 1500, 12, 0, 'CUST-C', '客户C', 'C-Z9-003', 'MES-Z9', 'High', 'InProgress', 'ADMIN', '2026-06-20 08:00:00', '2026-06-24 18:00:00', '2026-06-20 08:00:00', NULL, 99.00, 98.00, 'Z9系列高优先级工单', '2026-06-20 08:00:00', '2026-06-20 08:30:00');

-- ============================================================
-- 三、批次数据
-- ============================================================

-- 3.1 X7系列批次
-- LOT-X7-001: 正常完成
INSERT INTO `prod_lot` VALUES
('LOT-X7-001', 'WO-2026-001', 'PROD-X7', 'X7系列控制芯片', 'X7-DIE', 'QFN-48', 'RT-X7-1.0', '1.0', 70, 'PKG-INS', 'Completed', 1000, 0, 'Normal', 'CR-TRAY-01', 'CR-TRAY-01', NULL, 1000, 985, 15, 0, 0, 0, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'A', NULL, NULL, 985, 15, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 08:30:00', '2026-06-20 18:00:00');

-- LOT-X7-002: Hold后释放完成
INSERT INTO `prod_lot` VALUES
('LOT-X7-002', 'WO-2026-001', 'PROD-X7', 'X7系列控制芯片', 'X7-DIE', 'QFN-48', 'RT-X7-1.0', '1.0', 70, 'PKG-INS', 'Completed', 1000, 0, 'Normal', 'CR-TRAY-02', 'CR-TRAY-02', NULL, 1000, 990, 10, 0, 0, 0, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'A', NULL, NULL, 990, 10, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 08:30:00', '2026-06-20 20:00:00');

-- LOT-X7-003: 拆分批次（母批次）
INSERT INTO `prod_lot` VALUES
('LOT-X7-003', 'WO-2026-001', 'PROD-X7', 'X7系列控制芯片', 'X7-DIE', 'QFN-48', 'RT-X7-1.0', '1.0', 60, 'TEST-CP', 'Split', 1000, 0, 'Normal', 'CR-MAG-01', 'CR-MAG-01', NULL, 1000, 0, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 08:30:00', '2026-06-20 16:00:00');

-- LOT-X7-003A: BIN1合格品
INSERT INTO `prod_lot` VALUES
('LOT-X7-003A', 'WO-2026-001', 'PROD-X7', 'X7系列控制芯片', 'X7-DIE', 'QFN-48', 'RT-X7-1.0', '1.0', 70, 'PKG-INS', 'Completed', 850, 0, 'Normal', 'CR-MAG-01', 'CR-MAG-01', NULL, 850, 848, 2, 0, 0, 0, 0, 0, 'LOT-X7-003', 'Grade Split', '2026-06-20 16:00:00', 850, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'A', 'LOT-X7-003', 'BIN1', 'Pass', 848, 2, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 16:00:00', '2026-06-20 19:00:00');

-- LOT-X7-003B: BIN2次品
INSERT INTO `prod_lot` VALUES
('LOT-X7-003B', 'WO-2026-001', 'PROD-X7', 'X7系列控制芯片', 'X7-DIE', 'QFN-48', 'RT-X7-1.0', '1.0', 60, 'TEST-CP', 'InProgress', 120, 0, 'Normal', 'CR-MAG-01', 'CR-MAG-01', NULL, 120, 118, 2, 0, 0, 0, 0, 0, 'LOT-X7-003', 'Grade Split', '2026-06-20 16:00:00', 120, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'B', 'LOT-X7-003', 'BIN2', 'Pass', 118, 2, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 16:00:00', '2026-06-20 17:00:00');

-- LOT-X7-003C: 报废品
INSERT INTO `prod_lot` VALUES
('LOT-X7-003C', 'WO-2026-001', 'PROD-X7', 'X7系列控制芯片', 'X7-DIE', 'QFN-48', 'RT-X7-1.0', '1.0', 60, 'TEST-CP', 'Scrap', 30, 0, 'Normal', NULL, NULL, NULL, 30, 0, 30, 0, 0, 0, 0, 0, 'LOT-X7-003', 'Scrap Split', '2026-06-20 16:00:00', 30, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'C', 'LOT-X7-003', 'BIN3', 'Fail', 0, 30, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 16:00:00', '2026-06-20 16:30:00');

-- LOT-X7-004: 重工批次
INSERT INTO `prod_lot` VALUES
('LOT-X7-004', 'WO-2026-001', 'PROD-X7', 'X7系列控制芯片', 'X7-DIE', 'QFN-48', 'RT-X7-RW-10', '1.0', 20, 'WIRE-BOND', 'InProgress', 1000, 0, 'Normal', 'CR-FOUP-01', 'CR-FOUP-01', NULL, 1000, 0, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, 1, 'RT-X7-1.0', 'RT-X7-RW-10', 1, '键合拉力不足', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 08:30:00', '2026-06-20 14:00:00');

-- LOT-X7-005: 正常流转中
INSERT INTO `prod_lot` VALUES
('LOT-X7-005', 'WO-2026-001', 'PROD-X7', 'X7系列控制芯片', 'X7-DIE', 'QFN-48', 'RT-X7-1.0', '1.0', 20, 'WIRE-BOND', 'InProgress', 1000, 0, 'Normal', 'CR-FOUP-02', 'CR-FOUP-02', NULL, 1000, 0, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 08:30:00', '2026-06-20 12:00:00');

-- 3.2 Y3系列批次
-- LOT-Y3-001: 正常完成
INSERT INTO `prod_lot` VALUES
('LOT-Y3-001', 'WO-2026-002', 'PROD-Y3', 'Y3功率MOSFET', 'Y3-DIE', 'SOP-8', 'RT-Y3-1.0', '1.0', 70, 'PKG-INS', 'Completed', 2000, 0, 'Normal', 'CR-TRAY-01', 'CR-TRAY-01', NULL, 2000, 1960, 40, 0, 0, 0, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'A', NULL, NULL, 1960, 40, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 08:30:00', '2026-06-20 22:00:00');

-- LOT-Y3-002: Hold状态
INSERT INTO `prod_lot` VALUES
('LOT-Y3-002', 'WO-2026-002', 'PROD-Y3', 'Y3功率MOSFET', 'Y3-DIE', 'SOP-8', 'RT-Y3-1.0', '1.0', 30, 'ENCAPS', 'Hold', 2000, 0, 'Normal', 'CR-TRAY-02', 'CR-TRAY-02', NULL, 2000, 1950, 50, 0, 0, 0, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, 1, 'MRB-REF-001', 'Pending Disposition', 'A', NULL, NULL, 1950, 50, 'Quality', '塑封表面有气泡', '2026-06-20 15:00:00', 'OPER-B', NULL, 0, '2026-06-20 08:30:00', '2026-06-20 15:00:00');

-- LOT-Y3-003: 测试中
INSERT INTO `prod_lot` VALUES
('LOT-Y3-003', 'WO-2026-002', 'PROD-Y3', 'Y3功率MOSFET', 'Y3-DIE', 'SOP-8', 'RT-Y3-1.0', '1.0', 60, 'TEST-FT', 'InProgress', 2000, 0, 'Normal', 'CR-MAG-02', 'CR-MAG-02', NULL, 2000, 0, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 08:30:00', '2026-06-20 18:00:00');

-- LOT-Y3-004: 切筋中
INSERT INTO `prod_lot` VALUES
('LOT-Y3-004', 'WO-2026-002', 'PROD-Y3', 'Y3功率MOSFET', 'Y3-DIE', 'SOP-8', 'RT-Y3-1.0', '1.0', 40, 'TRIM', 'InProgress', 2000, 0, 'Normal', 'CR-MAG-01', 'CR-MAG-01', NULL, 2000, 0, 0, 0, 0, 0, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 08:30:00', '2026-06-20 16:00:00');

-- 3.3 Z9系列批次
-- LOT-Z9-001: 正常完成
INSERT INTO `prod_lot` VALUES
('LOT-Z9-001', 'WO-2026-003', 'PROD-Z9', 'Z9传感器芯片', 'Z9-DIE', 'BGA-64', 'RT-Z9-1.0', '1.0', 80, 'PKG-INS', 'Completed', 1500, 0, 'Normal', 'CR-FOUP-01', 'CR-FOUP-01', NULL, 1500, 1485, 15, 0, 0, 0, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 'A', NULL, NULL, 1485, 15, NULL, NULL, NULL, NULL, NULL, 0, '2026-06-20 08:30:00', '2026-06-21 10:00:00');

-- LOT-Z9-002: Hold状态（底部填充后）
INSERT INTO `prod_lot` VALUES
('LOT-Z9-002', 'WO-2026-003', 'PROD-Z9', 'Z9传感器芯片', 'Z9-DIE', 'BGA-64', 'RT-Z9-1.0', '1.0', 30, 'UNDERFILL', 'Hold', 1500, 0, 'Normal', 'CR-FOUP-02', 'CR-FOUP-02', NULL, 1500, 1470, 30, 0, 0, 0, 0, 0, NULL, NULL, NULL, 0, NULL, NULL, NULL, 1, 'MRB-REF-002', 'Pending Process Adjustment', 'A', NULL, NULL, 1470, 30, 'MRB', '底部填充气泡率超标', '2026-06-20 12:00:00', 'OPER-C', NULL, 0, '2026-06-20 08:30:00', '2026-06-20 12:00:00');

-- ============================================================
-- 四、批次步骤记录 - LOT-X7-001 (正常完成)
-- ============================================================

INSERT INTO `prod_lot_step` VALUES
('STEP-X7-001-10', 'LOT-X7-001', 'RT-X7-1.0', '1.0', 10, 'DIE-BOND', '芯片贴装', 'Completed', 'EQ-DB-01', 'CR-TRAY-01', 'RCP-X7-DB-10', '2026-06-20 08:45:00', 'OPER-A', '2026-06-20 09:15:00', 'OPER-A', 1000, 998, 2, 0, 0, 0, 1000, 'RCP-X7-DB-10', NULL, NULL, NULL, '2026-06-20 08:30:00'),
('STEP-X7-001-20', 'LOT-X7-001', 'RT-X7-1.0', '1.0', 20, 'WIRE-BOND', '引线键合', 'Completed', 'EQ-WB-01', 'CR-TRAY-01', 'RCP-X7-WB-20', '2026-06-20 09:30:00', 'OPER-A', '2026-06-20 10:30:00', 'OPER-A', 998, 995, 3, 0, 0, 0, 995, 'RCP-X7-WB-20', NULL, NULL, NULL, '2026-06-20 09:15:00'),
('STEP-X7-001-30', 'LOT-X7-001', 'RT-X7-1.0', '1.0', 30, 'ENCAPS', '塑封成型', 'Completed', 'EQ-MD-01', 'CR-TRAY-01', NULL, '2026-06-20 10:45:00', 'OPER-A', '2026-06-20 11:30:00', 'OPER-A', 995, 992, 3, 0, 0, 0, 992, NULL, NULL, NULL, NULL, '2026-06-20 10:30:00'),
('STEP-X7-001-40', 'LOT-X7-001', 'RT-X7-1.0', '1.0', 40, 'PLATING', '电镀处理', 'Completed', 'EQ-PL-01', 'CR-TRAY-01', NULL, '2026-06-20 11:45:00', 'OPER-A', '2026-06-20 13:00:00', 'OPER-A', 992, 990, 2, 0, 0, 0, 990, NULL, NULL, NULL, NULL, '2026-06-20 11:30:00'),
('STEP-X7-001-50', 'LOT-X7-001', 'RT-X7-1.0', '1.0', 50, 'MARK', '激光打标', 'Completed', 'EQ-LM-01', 'CR-TRAY-01', NULL, '2026-06-20 13:15:00', 'OPER-A', '2026-06-20 14:00:00', 'OPER-A', 990, 990, 0, 0, 0, 0, 990, NULL, NULL, NULL, NULL, '2026-06-20 13:00:00'),
('STEP-X7-001-60', 'LOT-X7-001', 'RT-X7-1.0', '1.0', 60, 'TEST-CP', 'CP测试', 'Completed', 'EQ-TS-01', 'CR-TRAY-01', 'RCP-X7-CP-60', '2026-06-20 14:15:00', 'OPER-A', '2026-06-20 16:00:00', 'OPER-A', 990, 985, 5, 0, 0, 0, 985, 'RCP-X7-CP-60', 'TP-X7-1.0', '{"BIN1":985,"BIN2":5}', NULL, '2026-06-20 14:00:00'),
('STEP-X7-001-70', 'LOT-X7-001', 'RT-X7-1.0', '1.0', 70, 'PKG-INS', '包装检验', 'Completed', 'EQ-PK-01', 'CR-TRAY-01', NULL, '2026-06-20 16:15:00', 'OPER-A', '2026-06-20 17:00:00', 'OPER-A', 985, 985, 0, 0, 0, 0, 985, NULL, NULL, NULL, NULL, '2026-06-20 16:00:00');

-- ============================================================
-- 四、批次步骤记录 - LOT-X7-002 (Hold后释放)
-- ============================================================

INSERT INTO `prod_lot_step` VALUES
('STEP-X7-002-10', 'LOT-X7-002', 'RT-X7-1.0', '1.0', 10, 'DIE-BOND', '芯片贴装', 'Completed', 'EQ-DB-02', 'CR-TRAY-02', 'RCP-X7-DB-10', '2026-06-20 08:45:00', 'OPER-B', '2026-06-20 09:15:00', 'OPER-B', 1000, 1000, 0, 0, 0, 0, 1000, 'RCP-X7-DB-10', NULL, NULL, NULL, '2026-06-20 08:30:00'),
('STEP-X7-002-20', 'LOT-X7-002', 'RT-X7-1.0', '1.0', 20, 'WIRE-BOND', '引线键合', 'Completed', 'EQ-WB-02', 'CR-TRAY-02', 'RCP-X7-WB-20', '2026-06-20 09:30:00', 'OPER-B', '2026-06-20 10:30:00', 'OPER-B', 1000, 1000, 0, 0, 0, 0, 1000, 'RCP-X7-WB-20', NULL, NULL, NULL, '2026-06-20 09:15:00'),
('STEP-X7-002-30', 'LOT-X7-002', 'RT-X7-1.0', '1.0', 30, 'ENCAPS', '塑封成型', 'Completed', 'EQ-MD-01', 'CR-TRAY-02', NULL, '2026-06-20 10:45:00', 'OPER-B', '2026-06-20 11:30:00', 'OPER-B', 1000, 998, 2, 0, 0, 0, 998, NULL, NULL, NULL, NULL, '2026-06-20 10:30:00'),
('STEP-X7-002-40', 'LOT-X7-002', 'RT-X7-1.0', '1.0', 40, 'PLATING', '电镀处理', 'Completed', 'EQ-PL-01', 'CR-TRAY-02', NULL, '2026-06-20 11:45:00', 'OPER-B', '2026-06-20 13:00:00', 'OPER-B', 998, 995, 3, 0, 0, 0, 995, NULL, NULL, NULL, NULL, '2026-06-20 11:30:00'),
('STEP-X7-002-50', 'LOT-X7-002', 'RT-X7-1.0', '1.0', 50, 'MARK', '激光打标', 'Completed', 'EQ-LM-01', 'CR-TRAY-02', NULL, '2026-06-20 13:15:00', 'OPER-B', '2026-06-20 14:00:00', 'OPER-B', 995, 995, 0, 0, 0, 0, 995, NULL, NULL, NULL, NULL, '2026-06-20 13:00:00'),
('STEP-X7-002-60', 'LOT-X7-002', 'RT-X7-1.0', '1.0', 60, 'TEST-CP', 'CP测试', 'Completed', 'EQ-TS-01', 'CR-TRAY-02', 'RCP-X7-CP-60', '2026-06-20 14:15:00', 'OPER-B', '2026-06-20 16:00:00', 'OPER-B', 995, 990, 5, 0, 0, 0, 990, 'RCP-X7-CP-60', 'TP-X7-1.0', '{"BIN1":990,"BIN2":5}', NULL, '2026-06-20 14:00:00'),
('STEP-X7-002-70', 'LOT-X7-002', 'RT-X7-1.0', '1.0', 70, 'PKG-INS', '包装检验', 'Completed', 'EQ-PK-01', 'CR-TRAY-02', NULL, '2026-06-20 17:00:00', 'OPER-B', '2026-06-20 18:00:00', 'OPER-B', 990, 990, 0, 0, 0, 0, 990, NULL, NULL, NULL, NULL, '2026-06-20 16:30:00');

-- ============================================================
-- 四、批次步骤记录 - LOT-X7-003 (拆分)
-- ============================================================

INSERT INTO `prod_lot_step` VALUES
('STEP-X7-003-10', 'LOT-X7-003', 'RT-X7-1.0', '1.0', 10, 'DIE-BOND', '芯片贴装', 'Completed', 'EQ-DB-01', 'CR-MAG-01', 'RCP-X7-DB-10', '2026-06-20 08:45:00', 'OPER-A', '2026-06-20 09:15:00', 'OPER-A', 1000, 1000, 0, 0, 0, 0, 1000, 'RCP-X7-DB-10', NULL, NULL, NULL, '2026-06-20 08:30:00'),
('STEP-X7-003-20', 'LOT-X7-003', 'RT-X7-1.0', '1.0', 20, 'WIRE-BOND', '引线键合', 'Completed', 'EQ-WB-01', 'CR-MAG-01', 'RCP-X7-WB-20', '2026-06-20 09:30:00', 'OPER-A', '2026-06-20 10:30:00', 'OPER-A', 1000, 1000, 0, 0, 0, 0, 1000, 'RCP-X7-WB-20', NULL, NULL, NULL, '2026-06-20 09:15:00'),
('STEP-X7-003-30', 'LOT-X7-003', 'RT-X7-1.0', '1.0', 30, 'ENCAPS', '塑封成型', 'Completed', 'EQ-MD-01', 'CR-MAG-01', NULL, '2026-06-20 10:45:00', 'OPER-A', '2026-06-20 11:30:00', 'OPER-A', 1000, 998, 2, 0, 0, 0, 998, NULL, NULL, NULL, NULL, '2026-06-20 10:30:00'),
('STEP-X7-003-40', 'LOT-X7-003', 'RT-X7-1.0', '1.0', 40, 'PLATING', '电镀处理', 'Completed', 'EQ-PL-01', 'CR-MAG-01', NULL, '2026-06-20 11:45:00', 'OPER-A', '2026-06-20 13:00:00', 'OPER-A', 998, 995, 3, 0, 0, 0, 995, NULL, NULL, NULL, NULL, '2026-06-20 11:30:00'),
('STEP-X7-003-50', 'LOT-X7-003', 'RT-X7-1.0', '1.0', 50, 'MARK', '激光打标', 'Completed', 'EQ-LM-01', 'CR-MAG-01', NULL, '2026-06-20 13:15:00', 'OPER-A', '2026-06-20 14:00:00', 'OPER-A', 995, 995, 0, 0, 0, 0, 995, NULL, NULL, NULL, NULL, '2026-06-20 13:00:00'),
('STEP-X7-003-60', 'LOT-X7-003', 'RT-X7-1.0', '1.0', 60, 'TEST-CP', 'CP测试', 'Completed', 'EQ-TS-01', 'CR-MAG-01', 'RCP-X7-CP-60', '2026-06-20 14:15:00', 'OPER-A', '2026-06-20 16:00:00', 'OPER-A', 995, 970, 25, 30, 0, 0, 970, 'RCP-X7-CP-60', 'TP-X7-1.0', '{"BIN1":850,"BIN2":120,"BIN3":30}', '测试后拆分', '2026-06-20 14:00:00');

-- ============================================================
-- 四、批次步骤记录 - LOT-X7-003A/003B/003C (子批次)
-- ============================================================

INSERT INTO `prod_lot_step` VALUES
('STEP-X7-003A-70', 'LOT-X7-003A', 'RT-X7-1.0', '1.0', 70, 'PKG-INS', '包装检验', 'Completed', 'EQ-PK-01', 'CR-MAG-01', NULL, '2026-06-20 16:30:00', 'OPER-A', '2026-06-20 17:30:00', 'OPER-A', 850, 848, 2, 0, 0, 0, 848, NULL, NULL, NULL, 'BIN1合格品', '2026-06-20 16:00:00');

INSERT INTO `prod_lot_step` VALUES
('STEP-X7-003B-60', 'LOT-X7-003B', 'RT-X7-1.0', '1.0', 60, 'TEST-CP', 'CP测试', 'InProgress', 'EQ-TS-02', 'CR-MAG-01', 'RCP-X7-CP-60', '2026-06-20 16:30:00', 'OPER-A', NULL, NULL, 120, 118, 2, 0, 0, 0, 118, 'RCP-X7-CP-60', 'TP-X7-1.0', '{"BIN1":118,"BIN2":2}', 'BIN2次品复检', '2026-06-20 16:00:00');

INSERT INTO `prod_lot_step` VALUES
('STEP-X7-003C-60', 'LOT-X7-003C', 'RT-X7-1.0', '1.0', 60, 'TEST-CP', 'CP测试', 'Completed', 'EQ-TS-01', NULL, 'RCP-X7-CP-60', '2026-06-20 16:00:00', 'OPER-A', '2026-06-20 16:15:00', 'OPER-A', 30, 0, 30, 30, 0, 0, 0, 'RCP-X7-CP-60', 'TP-X7-1.0', '{"BIN3":30}', '报废品', '2026-06-20 16:00:00');

-- ============================================================
-- 四、批次步骤记录 - LOT-X7-004 (重工)
-- ============================================================

INSERT INTO `prod_lot_step` VALUES
('STEP-X7-004-10', 'LOT-X7-004', 'RT-X7-1.0', '1.0', 10, 'DIE-BOND', '芯片贴装', 'Completed', 'EQ-DB-01', 'CR-FOUP-01', 'RCP-X7-DB-10', '2026-06-20 08:45:00', 'OPER-A', '2026-06-20 09:15:00', 'OPER-A', 1000, 1000, 0, 0, 0, 0, 1000, 'RCP-X7-DB-10', NULL, NULL, NULL, '2026-06-20 08:30:00'),
('STEP-X7-004-20', 'LOT-X7-004', 'RT-X7-1.0', '1.0', 20, 'WIRE-BOND', '引线键合', 'Completed', 'EQ-WB-01', 'CR-FOUP-01', 'RCP-X7-WB-20', '2026-06-20 09:30:00', 'OPER-A', '2026-06-20 10:30:00', 'OPER-A', 1000, 950, 50, 0, 0, 0, 950, 'RCP-X7-WB-20', NULL, '拉力检测不合格', '2026-06-20 09:15:00'),
('STEP-X7-004-20-RW', 'LOT-X7-004', 'RT-X7-RW-10', '1.0', 20, 'WIRE-BOND', '引线键合(重工)', 'InProgress', 'EQ-WB-02', 'CR-FOUP-01', 'RCP-X7-WB-20', '2026-06-20 14:00:00', 'OPER-A', NULL, NULL, 50, 0, 0, 0, 0, 0, 50, 'RCP-X7-WB-20', NULL, '重工键合', '2026-06-20 13:00:00');

-- ============================================================
-- 四、批次步骤记录 - LOT-X7-005 (正常流转中)
-- ============================================================

INSERT INTO `prod_lot_step` VALUES
('STEP-X7-005-10', 'LOT-X7-005', 'RT-X7-1.0', '1.0', 10, 'DIE-BOND', '芯片贴装', 'Completed', 'EQ-DB-02', 'CR-FOUP-02', 'RCP-X7-DB-10', '2026-06-20 08:45:00', 'OPER-B', '2026-06-20 09:15:00', 'OPER-B', 1000, 1000, 0, 0, 0, 0, 1000, 'RCP-X7-DB-10', NULL, NULL, NULL, '2026-06-20 08:30:00'),
('STEP-X7-005-20', 'LOT-X7-005', 'RT-X7-1.0', '1.0', 20, 'WIRE-BOND', '引线键合', 'InProgress', 'EQ-WB-03', 'CR-FOUP-02', 'RCP-X7-WB-20', '2026-06-20 09:30:00', 'OPER-B', NULL, NULL, 1000, 0, 0, 0, 0, 0, 1000, 'RCP-X7-WB-20', NULL, NULL, '2026-06-20 09:15:00');

-- ============================================================
-- 四、批次步骤记录 - Y3系列批次
-- ============================================================

-- LOT-Y3-001 (正常完成)
INSERT INTO `prod_lot_step` VALUES
('STEP-Y3-001-10', 'LOT-Y3-001', 'RT-Y3-1.0', '1.0', 10, 'DIE-BOND', '芯片贴装', 'Completed', 'EQ-DB-01', 'CR-TRAY-01', 'RCP-Y3-DB-10', '2026-06-20 08:45:00', 'OPER-B', '2026-06-20 09:30:00', 'OPER-B', 2000, 1998, 2, 0, 0, 0, 1998, 'RCP-Y3-DB-10', NULL, NULL, NULL, '2026-06-20 08:30:00'),
('STEP-Y3-001-20', 'LOT-Y3-001', 'RT-Y3-1.0', '1.0', 20, 'WIRE-BOND', '引线键合', 'Completed', 'EQ-WB-01', 'CR-TRAY-01', 'RCP-Y3-WB-20', '2026-06-20 09:45:00', 'OPER-B', '2026-06-20 11:00:00', 'OPER-B', 1998, 1995, 3, 0, 0, 0, 1995, 'RCP-Y3-WB-20', NULL, NULL, NULL, '2026-06-20 09:30:00'),
('STEP-Y3-001-30', 'LOT-Y3-001', 'RT-Y3-1.0', '1.0', 30, 'ENCAPS', '塑封成型', 'Completed', 'EQ-MD-01', 'CR-TRAY-01', NULL, '2026-06-20 11:15:00', 'OPER-B', '2026-06-20 12:00:00', 'OPER-B', 1995, 1990, 5, 0, 0, 0, 1990, NULL, NULL, NULL, NULL, '2026-06-20 11:00:00'),
('STEP-Y3-001-40', 'LOT-Y3-001', 'RT-Y3-1.0', '1.0', 40, 'TRIM', '切筋成型', 'Completed', 'EQ-TF-01', 'CR-TRAY-01', NULL, '2026-06-20 12:15:00', 'OPER-B', '2026-06-20 13:00:00', 'OPER-B', 1990, 1988, 2, 0, 0, 0, 1988, NULL, NULL, NULL, NULL, '2026-06-20 12:00:00'),
('STEP-Y3-001-50', 'LOT-Y3-001', 'RT-Y3-1.0', '1.0', 50, 'PLATING', '电镀处理', 'Completed', 'EQ-PL-01', 'CR-TRAY-01', NULL, '2026-06-20 13:15:00', 'OPER-B', '2026-06-20 15:00:00', 'OPER-B', 1988, 1980, 8, 0, 0, 0, 1980, NULL, NULL, NULL, NULL, '2026-06-20 13:00:00'),
('STEP-Y3-001-60', 'LOT-Y3-001', 'RT-Y3-1.0', '1.0', 60, 'TEST-FT', 'FT测试', 'Completed', 'EQ-TS-01', 'CR-TRAY-01', NULL, '2026-06-20 15:15:00', 'OPER-B', '2026-06-20 18:00:00', 'OPER-B', 1980, 1960, 20, 0, 0, 0, 1960, NULL, 'TP-Y3-1.0', '{"BIN1":1960,"BIN2":20}', NULL, '2026-06-20 15:00:00'),
('STEP-Y3-001-70', 'LOT-Y3-001', 'RT-Y3-1.0', '1.0', 70, 'PKG-INS', '包装检验', 'Completed', 'EQ-PK-01', 'CR-TRAY-01', NULL, '2026-06-20 18:15:00', 'OPER-B', '2026-06-20 19:30:00', 'OPER-B', 1960, 1960, 0, 0, 0, 0, 1960, NULL, NULL, NULL, NULL, '2026-06-20 18:00:00');

-- LOT-Y3-002 (Hold)
INSERT INTO `prod_lot_step` VALUES
('STEP-Y3-002-10', 'LOT-Y3-002', 'RT-Y3-1.0', '1.0', 10, 'DIE-BOND', '芯片贴装', 'Completed', 'EQ-DB-02', 'CR-TRAY-02', 'RCP-Y3-DB-10', '2026-06-20 08:45:00', 'OPER-B', '2026-06-20 09:30:00', 'OPER-B', 2000, 2000, 0, 0, 0, 0, 2000, 'RCP-Y3-DB-10', NULL, NULL, NULL, '2026-06-20 08:30:00'),
('STEP-Y3-002-20', 'LOT-Y3-002', 'RT-Y3-1.0', '1.0', 20, 'WIRE-BOND', '引线键合', 'Completed', 'EQ-WB-02', 'CR-TRAY-02', 'RCP-Y3-WB-20', '2026-06-20 09:45:00', 'OPER-B', '2026-06-20 11:00:00', 'OPER-B', 2000, 1998, 2, 0, 0, 0, 1998, 'RCP-Y3-WB-20', NULL, NULL, NULL, '2026-06-20 09:30:00'),
('STEP-Y3-002-30', 'LOT-Y3-002', 'RT-Y3-1.0', '1.0', 30, 'ENCAPS', '塑封成型', 'Hold', 'EQ-MD-01', 'CR-TRAY-02', NULL, '2026-06-20 11:15:00', 'OPER-B', NULL, NULL, 1998, 1950, 0, 0, 48, 0, 1950, NULL, NULL, '表面有气泡，等待MRB评审', '2026-06-20 11:00:00');

-- LOT-Y3-003 (测试中)
INSERT INTO `prod_lot_step` VALUES
('STEP-Y3-003-10', 'LOT-Y3-003', 'RT-Y3-1.0', '1.0', 10, 'DIE-BOND', '芯片贴装', 'Completed', 'EQ-DB-01', 'CR-MAG-02', 'RCP-Y3-DB-10', '2026-06-20 08:45:00', 'OPER-B', '2026-06-20 09:30:00', 'OPER-B', 2000, 2000, 0, 0, 0, 0, 2000, 'RCP-Y3-DB-10', NULL, NULL, NULL, '2026-06-20 08:30:00'),
('STEP-Y3-003-20', 'LOT-Y3-003', 'RT-Y3-1.0', '1.0', 20, 'WIRE-BOND', '引线键合', 'Completed', 'EQ-WB-03', 'CR-MAG-02', 'RCP-Y3-WB-20', '2026-06-20 09:45:00', 'OPER-B', '2026-06-20 11:00:00', 'OPER-B', 2000, 1995, 5, 0, 0, 0, 1995, 'RCP-Y3-WB-20', NULL, NULL, NULL, '2026-06-20 09:30:00'),
('STEP-Y3-003-30', 'LOT-Y3-003', 'RT-Y3-1.0', '1.0', 30, 'ENCAPS', '塑封成型', 'Completed', 'EQ-MD-01', 'CR-MAG-02', NULL, '2026-06-20 11:15:00', 'OPER-B', '2026-06-20 12:00:00', 'OPER-B', 1995, 1992, 3, 0, 0, 0, 1992, NULL, NULL, NULL, NULL, '2026-06-20 11:00:00'),
('STEP-Y3-003-40', 'LOT-Y3-003', 'RT-Y3-1.0', '1.0', 40, 'TRIM', '切筋成型', 'Completed', 'EQ-TF-01', 'CR-MAG-02', NULL, '2026-06-20 12:15:00', 'OPER-B', '2026-06-20 13:30:00', 'OPER-B', 1992, 1990, 2, 0, 0, 0, 1990, NULL, NULL, NULL, NULL, '2026-06-20 12:00:00'),
('STEP-Y3-003-50', 'LOT-Y3-003', 'RT-Y3-1.0', '1.0', 50, 'PLATING', '电镀处理', 'Completed', 'EQ-PL-01', 'CR-MAG-02', NULL, '2026-06-20 13:45:00', 'OPER-B', '2026-06-20 15:30:00', 'OPER-B', 1990, 1985, 5, 0, 0, 0, 1985, NULL, NULL, NULL, NULL, '2026-06-20 13:30:00'),
('STEP-Y3-003-60', 'LOT-Y3-003', 'RT-Y3-1.0', '1.0', 60, 'TEST-FT', 'FT测试', 'InProgress', 'EQ-TS-02', 'CR-MAG-02', NULL, '2026-06-20 15:45:00', 'OPER-B', NULL, NULL, 1985, 1500, 10, 0, 0, 0, 475, NULL, 'TP-Y3-1.0', NULL, NULL, '2026-06-20 15:30:00');

-- LOT-Y3-004 (切筋中)
INSERT INTO `prod_lot_step` VALUES
('STEP-Y3-004-10', 'LOT-Y3-004', 'RT-Y3-1.0', '1.0', 10, 'DIE-BOND', '芯片贴装', 'Completed', 'EQ-DB-02', 'CR-MAG-01', 'RCP-Y3-DB-10', '2026-06-20 08:45:00', 'OPER-B', '2026-06-20 09:30:00', 'OPER-B', 2000, 1998, 2, 0, 0, 0, 1998, 'RCP-Y3-DB-10', NULL, NULL, NULL, '2026-06-20 08:30:00'),
('STEP-Y3-004-20', 'LOT-Y3-004', 'RT-Y3-1.0', '1.0', 20, 'WIRE-BOND', '引线键合', 'Completed', 'EQ-WB-01', 'CR-MAG-01', 'RCP-Y3-WB-20', '2026-06-20 09:45:00', 'OPER-B', '2026-06-20 11:00:00', 'OPER-B', 1998, 1995, 3, 0, 0, 0, 1995, 'RCP-Y3-WB-20', NULL, NULL, NULL, '2026-06-20 09:30:00'),
('STEP-Y3-004-30', 'LOT-Y3-004', 'RT-Y3-1.0', '1.0', 30, 'ENCAPS', '塑封成型', 'Completed', 'EQ-MD-01', 'CR-MAG-01', NULL, '2026-06-20 11:15:00', 'OPER-B', '2026-06-20 12:00:00', 'OPER-B', 1995, 1993, 2, 0, 0, 0, 1993, NULL, NULL, NULL, NULL, '2026-06-20 11:00:00'),
('STEP-Y3-004-40', 'LOT-Y3-004', 'RT-Y3-1.0', '1.0', 40, 'TRIM', '切筋成型', 'InProgress', 'EQ-TF-01', 'CR-MAG-01', NULL, '2026-06-20 12:15:00', 'OPER-B', NULL, NULL, 1993, 1990, 0, 0, 0, 0, 1990, NULL, NULL, NULL, '2026-06-20 12:00:00');

-- ============================================================
-- 四、批次步骤记录 - Z9系列批次
-- ============================================================

-- LOT-Z9-001 (正常完成)
INSERT INTO `prod_lot_step` VALUES
('STEP-Z9-001-10', 'LOT-Z9-001', 'RT-Z9-1.0', '1.0', 10, 'DIE-BOND', '芯片贴装', 'Completed', 'EQ-DB-01', 'CR-FOUP-01', NULL, '2026-06-20 08:45:00', 'OPER-C', '2026-06-20 09:30:00', 'OPER-C', 1500, 1500, 0, 0, 0, 0, 1500, NULL, NULL, NULL, NULL, '2026-06-20 08:30:00'),
('STEP-Z9-001-20', 'LOT-Z9-001', 'RT-Z9-1.0', '1.0', 20, 'WIRE-BOND', '引线键合', 'Completed', 'EQ-WB-01', 'CR-FOUP-01', NULL, '2026-06-20 09:45:00', 'OPER-C', '2026-06-20 11:00:00', 'OPER-C', 1500, 1498, 2, 0, 0, 0, 1498, NULL, NULL, NULL, NULL, '2026-06-20 09:30:00'),
('STEP-Z9-001-30', 'LOT-Z9-001', 'RT-Z9-1.0', '1.0', 30, 'UNDERFILL', '底部填充', 'Completed', 'EQ-DP-01', 'CR-FOUP-01', NULL, '2026-06-20 11:15:00', 'OPER-C', '2026-06-20 12:30:00', 'OPER-C', 1498, 1495, 3, 0, 0, 0, 1495, NULL, NULL, NULL, NULL, '2026-06-20 11:00:00'),
('STEP-Z9-001-40', 'LOT-Z9-001', 'RT-Z9-1.0', '1.0', 40, 'ENCAPS', '塑封成型', 'Completed', 'EQ-MD-01', 'CR-FOUP-01', NULL, '2026-06-20 12:45:00', 'OPER-C', '2026-06-20 13:30:00', 'OPER-C', 1495, 1492, 3, 0, 0, 0, 1492, NULL, NULL, NULL, NULL, '2026-06-20 12:30:00'),
('STEP-Z9-001-50', 'LOT-Z9-001', 'RT-Z9-1.0', '1.0', 50, 'BALL-ATTACH', '植球', 'Completed', 'EQ-BM-01', 'CR-FOUP-01', 'RCP-Z9-BM-50', '2026-06-20 13:45:00', 'OPER-C', '2026-06-20 14:45:00', 'OPER-C', 1492, 1490, 2, 0, 0, 0, 1490, 'RCP-Z9-BM-50', NULL, NULL, NULL, '2026-06-20 13:30:00'),
('STEP-Z9-001-60', 'LOT-Z9-001', 'RT-Z9-1.0', '1.0', 60, 'REFLOW', '回流焊', 'Completed', 'EQ-RO-01', 'CR-FOUP-01', NULL, '2026-06-20 15:00:00', 'OPER-C', '2026-06-20 16:00:00', 'OPER-C', 1490, 1488, 2, 0, 0, 0, 1488, NULL, NULL, NULL, NULL, '2026-06-20 14:45:00'),
('STEP-Z9-001-70', 'LOT-Z9-001', 'RT-Z9-1.0', '1.0', 70, 'TEST-CP', 'CP测试', 'Completed', 'EQ-TS-01', 'CR-FOUP-01', NULL, '2026-06-20 16:15:00', 'OPER-C', '2026-06-20 18:00:00', 'OPER-C', 1488, 1485, 3, 0, 0, 0, 1485, NULL, 'TP-Z9-1.0', '{"BIN1":1485,"BIN2":3}', NULL, '2026-06-20 16:00:00'),
('STEP-Z9-001-80', 'LOT-Z9-001', 'RT-Z9-1.0', '1.0', 80, 'PKG-INS', '包装检验', 'Completed', 'EQ-PK-01', 'CR-FOUP-01', NULL, '2026-06-21 08:00:00', 'OPER-C', '2026-06-21 09:00:00', 'OPER-C', 1485, 1485, 0, 0, 0, 0, 1485, NULL, NULL, NULL, NULL, '2026-06-20 18:00:00');

-- LOT-Z9-002 (Hold)
INSERT INTO `prod_lot_step` VALUES
('STEP-Z9-002-10', 'LOT-Z9-002', 'RT-Z9-1.0', '1.0', 10, 'DIE-BOND', '芯片贴装', 'Completed', 'EQ-DB-02', 'CR-FOUP-02', NULL, '2026-06-20 08:45:00', 'OPER-C', '2026-06-20 09:30:00', 'OPER-C', 1500, 1500, 0, 0, 0, 0, 1500, NULL, NULL, NULL, NULL, '2026-06-20 08:30:00'),
('STEP-Z9-002-20', 'LOT-Z9-002', 'RT-Z9-1.0', '1.0', 20, 'WIRE-BOND', '引线键合', 'Completed', 'EQ-WB-02', 'CR-FOUP-02', NULL, '2026-06-20 09:45:00', 'OPER-C', '2026-06-20 11:00:00', 'OPER-C', 1500, 1495, 5, 0, 0, 0, 1495, NULL, NULL, NULL, NULL, '2026-06-20 09:30:00'),
('STEP-Z9-002-30', 'LOT-Z9-002', 'RT-Z9-1.0', '1.0', 30, 'UNDERFILL', '底部填充', 'Hold', 'EQ-DP-01', 'CR-FOUP-02', NULL, '2026-06-20 11:15:00', 'OPER-C', NULL, NULL, 1495, 1470, 0, 0, 25, 0, 1470, NULL, NULL, '气泡率超标，需要工艺调整', '2026-06-20 11:00:00');

-- ============================================================
-- 五、Hold/Release记录
-- ============================================================

-- HOLD-X7-001: 已释放
INSERT INTO `prod_hold_record` VALUES
('HOLD-X7-001', 'LOT-X7-002', 'Quality', 'QC-001', 'Die Bond偏移量超标，需要QA确认', 1000, 'QA', 'QA-USER', 'Released', 'OPER-A', '2026-06-20 09:20:00', '偏移量超过规格上限0.05mm', '调整贴片参数后复检合格', '放行继续生产', 'QA-USER', '2026-06-20 11:00:00', '偏移量复检合格，允许放行', 'QA-MANAGER', '2026-06-20 09:20:00');

-- HOLD-Y3-001: Open
INSERT INTO `prod_hold_record` VALUES
('HOLD-Y3-001', 'LOT-Y3-002', 'Quality', 'QC-002', '塑封表面有气泡，等待MRB评审', 1950, 'QA', 'QA-USER', 'Open', 'OPER-B', '2026-06-20 11:30:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-06-20 11:30:00');

-- HOLD-Z9-001: Open
INSERT INTO `prod_hold_record` VALUES
('HOLD-Z9-001', 'LOT-Z9-002', 'MRB', 'QC-003', '底部填充气泡率超标，需要工艺调整', 1470, 'ENG', 'ENG-USER', 'Open', 'OPER-C', '2026-06-20 12:00:00', NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-06-20 12:00:00');

-- ============================================================
-- 六、批次拆分记录
-- ============================================================

INSERT INTO `prod_lot_split` VALUES
('SPL-X7-003A', 'LOT-X7-003', 'LOT-X7-003A', 850, 'Grade Split', 'Grade', 'TEST-CP', 60, 'OPER-A', '2026-06-20 16:00:00', 'QA-USER', NULL, '2026-06-20 16:00:00'),
('SPL-X7-003B', 'LOT-X7-003', 'LOT-X7-003B', 120, 'Grade Split', 'Grade', 'TEST-CP', 60, 'OPER-A', '2026-06-20 16:00:00', 'QA-USER', NULL, '2026-06-20 16:00:00'),
('SPL-X7-003C', 'LOT-X7-003', 'LOT-X7-003C', 30, 'Scrap Split', 'Grade', 'TEST-CP', 60, 'OPER-A', '2026-06-20 16:00:00', 'QA-USER', NULL, '2026-06-20 16:00:00');

-- ============================================================
-- 七、谱系关系记录
-- ============================================================

INSERT INTO `prod_genealogy` VALUES
('GEN-X7-003A', 'LOT-X7-003', 'LOT-X7-003A', 'Split', 'TEST-CP', 60, 850, 'A', NULL, 'OPER-A', 'GRADE', 'BIN1合格品', '2026-06-20 16:00:00'),
('GEN-X7-003B', 'LOT-X7-003', 'LOT-X7-003B', 'Split', 'TEST-CP', 60, 120, 'B', NULL, 'OPER-A', 'GRADE', 'BIN2次品', '2026-06-20 16:00:00'),
('GEN-X7-003C', 'LOT-X7-003', 'LOT-X7-003C', 'Split', 'TEST-CP', 60, 30, 'C', NULL, 'OPER-A', 'SCRAP', '报废品', '2026-06-20 16:00:00');

-- ============================================================
-- 八、重工记录
-- ============================================================

INSERT INTO `prod_rework_record` VALUES
('RW-X7-001', 'LOT-X7-004', 'RT-X7-1.0', 'RT-X7-RW-10', 'WIRE-BOND', 'WIRE-BOND', 50, '键合拉力不足', 'OPER-A', 1, '2026-06-20 13:00:00', NULL, 'ENG-MANAGER', NULL);

-- ============================================================
-- 九、报警记录
-- ============================================================

INSERT INTO `alarm_record` VALUES
('ALM-Y3-001', 'RULE-YIELD-001', 'LOT-Y3-002', NULL, 'Yield', 'Critical', 'LOT-Y3-002在ENCAPS步骤良率低于阈值(97.5% < 98%)', 'Acknowledged', 'QA-USER', '2026-06-20 12:00:00', NULL, NULL, '2026-06-20 11:30:00'),
('ALM-HOLD-001', 'RULE-HOLD-001', 'LOT-Z9-002', NULL, 'HoldTimeout', 'Warning', 'LOT-Z9-002 Hold时间超过4小时未处理', 'Active', NULL, NULL, NULL, NULL, '2026-06-20 16:00:00');

-- ============================================================
-- 十、报警规则
-- ============================================================

INSERT INTO `alarm_rule` VALUES
('RULE-YIELD-001', '良率低报警', 'Yield', 'yield < 98.00', 'Critical', 'QA,ENG', 1, '2026-06-19 10:00:00'),
('RULE-HOLD-001', 'Hold超时报警', 'HoldTimeout', 'hold_hours > 4', 'Warning', 'QA,PROD', 1, '2026-06-19 10:00:00');

-- ============================================================
-- 十一、Quality Gate记录
-- ============================================================

INSERT INTO `quality_gate_instance` VALUES
('QG-X7-001', 'LOT-X7-001', 'TEST-CP', 60, 'QACheck', 'Passed', 'QA-USER', 'QA User', '2026-06-20 16:30:00', '良率98.5%，符合要求', '2026-06-20 16:00:00', '2026-06-20 20:00:00'),
('QG-X7-002', 'LOT-X7-002', 'DIE-BOND', 10, 'QACheck', 'Passed', 'QA-USER', 'QA User', '2026-06-20 11:00:00', 'Hold释放后复检合格', '2026-06-20 11:00:00', '2026-06-20 15:00:00');

-- ============================================================
-- 十二、载具绑定记录
-- ============================================================

INSERT INTO `prod_carrier_binding` VALUES
('BIND-X7-001-10', 'LOT-X7-001', 'DIE-BOND', 10, 'CR-TRAY-01', 'TRAY', NULL, '2026-06-20 08:45:00', '2026-06-20 09:15:00', 'OPER-A', '2026-06-20 08:45:00'),
('BIND-X7-001-20', 'LOT-X7-001', 'WIRE-BOND', 20, 'CR-TRAY-01', 'TRAY', NULL, '2026-06-20 09:30:00', '2026-06-20 10:30:00', 'OPER-A', '2026-06-20 09:30:00'),
('BIND-X7-001-70', 'LOT-X7-001', 'PKG-INS', 70, 'CR-TRAY-01', 'TRAY', NULL, '2026-06-20 16:15:00', '2026-06-20 17:00:00', 'OPER-A', '2026-06-20 16:15:00');

-- ============================================================
-- 十三、操作历史记录
-- ============================================================

INSERT INTO `prod_operation_history` (`operation_id`, `lot_id`, `order_id`, `operation_type`, `step_code`, `step_seq`, `equipment_id`, `carrier_id`, `recipe_id`, `operator_id`, `operator_name`, `input_qty`, `output_qty`, `scrap_qty`, `detail`, `remark`, `created_at`) VALUES
('OP-X7-001-TI', 'LOT-X7-001', 'WO-2026-001', 'TrackIn', 'DIE-BOND', 10, 'EQ-DB-01', 'CR-TRAY-01', 'RCP-X7-DB-10', 'OPER-A', 'Operator A', 1000, 1000, 0, '{"route":"RT-X7-1.0"}', '正常投产', '2026-06-20 08:45:00'),
('OP-X7-001-TO', 'LOT-X7-001', 'WO-2026-001', 'TrackOut', 'DIE-BOND', 10, 'EQ-DB-01', 'CR-TRAY-01', 'RCP-X7-DB-10', 'OPER-A', 'Operator A', 1000, 998, 2, '{"pass":998,"fail":2}', '贴装完成', '2026-06-20 09:15:00'),
('OP-X7-002-HD', 'LOT-X7-002', 'WO-2026-001', 'Hold', 'DIE-BOND', 10, NULL, NULL, NULL, 'OPER-A', 'Operator A', NULL, NULL, NULL, '{"reason":"偏移量超标"}', '质量Hold', '2026-06-20 09:20:00'),
('OP-X7-002-RL', 'LOT-X7-002', 'WO-2026-001', 'Release', 'DIE-BOND', 10, NULL, NULL, NULL, 'QA-USER', 'QA User', NULL, NULL, NULL, '{"comment":"复检合格"}', 'Hold释放', '2026-06-20 11:00:00'),
('OP-X7-003-SP', 'LOT-X7-003', 'WO-2026-001', 'Split', 'TEST-CP', 60, NULL, NULL, NULL, 'OPER-A', 'Operator A', 1000, 1000, 30, '{"children":["LOT-X7-003A","LOT-X7-003B","LOT-X7-003C"]}', '测试后按BIN拆分', '2026-06-20 16:00:00'),
('OP-X7-004-RW', 'LOT-X7-004', 'WO-2026-001', 'Rework', 'WIRE-BOND', 20, NULL, NULL, NULL, 'OPER-A', 'Operator A', 50, 50, 0, '{"from_route":"RT-X7-1.0","to_route":"RT-X7-RW-10"}', '键合重工', '2026-06-20 13:00:00');

-- ============================================================
-- 十四、审计追踪记录
-- ============================================================

INSERT INTO `prod_audit_trail` VALUES
('AUD-X7-001-01', 'Lot', 'LOT-X7-001', 'TrackIn', 'OPER-A', 'Operator A', '2026-06-20 08:45:00', NULL, '{"status":"InProgress"}', '正常投产', 'LOT-X7-001开始生产', 0, NULL),
('AUD-X7-001-02', 'Lot', 'LOT-X7-001', 'TrackOut', 'OPER-A', 'Operator A', '2026-06-20 17:00:00', '{"status":"InProgress"}', '{"status":"Completed"}', 'LOT-X7-001完成全部工序', '生产完成', 0, NULL),
('AUD-X7-002-01', 'Lot', 'LOT-X7-002', 'Hold', 'OPER-A', 'Operator A', '2026-06-20 09:20:00', '{"status":"InProgress"}', '{"status":"Hold"}', '质量Hold', '偏移量超标', 1, 'QA-USER'),
('AUD-X7-002-02', 'Lot', 'LOT-X7-002', 'Release', 'QA-USER', 'QA User', '2026-06-20 11:00:00', '{"status":"Hold"}', '{"status":"InProgress"}', 'Hold释放', '复检合格', 1, 'QA-MANAGER'),
('AUD-X7-003-01', 'Lot', 'LOT-X7-003', 'Split', 'OPER-A', 'Operator A', '2026-06-20 16:00:00', '{"status":"InProgress"}', '{"status":"Split"}', '批次拆分', '按BIN结果拆分', 1, 'QA-USER'),
('AUD-X7-004-01', 'Lot', 'LOT-X7-004', 'Rework', 'OPER-A', 'Operator A', '2026-06-20 13:00:00', '{"route_id":"RT-X7-1.0"}', '{"route_id":"RT-X7-RW-10"}', '重工', '键合拉力不足', 1, 'ENG-MANAGER');

-- ============================================================
-- 十五、数量事务记录
-- ============================================================

INSERT INTO `quantity_transaction` (`lot_id`, `route_id`, `step_seq`, `step_code`, `step_name`, `equipment_id`, `input_qty`, `pass_qty`, `fail_qty`, `scrap_qty`, `rework_qty`, `hold_qty`, `pending_qty`, `operator_id`, `operator_name`, `timestamp`) VALUES
('LOT-X7-001', 'RT-X7-1.0', 10, 'DIE-BOND', '芯片贴装', 'EQ-DB-01', 1000, 998, 2, 0, 0, 0, 1000, 'OPER-A', 'Operator A', '2026-06-20 09:15:00'),
('LOT-X7-001', 'RT-X7-1.0', 20, 'WIRE-BOND', '引线键合', 'EQ-WB-01', 998, 995, 3, 0, 0, 0, 995, 'OPER-A', 'Operator A', '2026-06-20 10:30:00'),
('LOT-X7-001', 'RT-X7-1.0', 60, 'TEST-CP', 'CP测试', 'EQ-TS-01', 990, 985, 5, 0, 0, 0, 985, 'OPER-A', 'Operator A', '2026-06-20 16:00:00'),
('LOT-X7-002', 'RT-X7-1.0', 10, 'DIE-BOND', '芯片贴装', 'EQ-DB-02', 1000, 1000, 0, 0, 0, 0, 1000, 'OPER-B', 'Operator B', '2026-06-20 09:15:00'),
('LOT-X7-003', 'RT-X7-1.0', 60, 'TEST-CP', 'CP测试', 'EQ-TS-01', 995, 970, 25, 30, 0, 0, 970, 'OPER-A', 'Operator A', '2026-06-20 16:00:00'),
('LOT-Y3-001', 'RT-Y3-1.0', 10, 'DIE-BOND', '芯片贴装', 'EQ-DB-01', 2000, 1998, 2, 0, 0, 0, 1998, 'OPER-B', 'Operator B', '2026-06-20 09:30:00'),
('LOT-Y3-001', 'RT-Y3-1.0', 60, 'TEST-FT', 'FT测试', 'EQ-TS-01', 1980, 1960, 20, 0, 0, 0, 1960, 'OPER-B', 'Operator B', '2026-06-20 18:00:00'),
('LOT-Y3-002', 'RT-Y3-1.0', 30, 'ENCAPS', '塑封成型', 'EQ-MD-01', 1998, 1950, 0, 0, 0, 48, 1950, 'OPER-B', 'Operator B', '2026-06-20 11:30:00'),
('LOT-Z9-001', 'RT-Z9-1.0', 10, 'DIE-BOND', '芯片贴装', 'EQ-DB-01', 1500, 1500, 0, 0, 0, 0, 1500, 'OPER-C', 'Operator C', '2026-06-20 09:30:00'),
('LOT-Z9-001', 'RT-Z9-1.0', 70, 'TEST-CP', 'CP测试', 'EQ-TS-01', 1488, 1485, 3, 0, 0, 0, 1485, 'OPER-C', 'Operator C', '2026-06-20 18:00:00'),
('LOT-Z9-002', 'RT-Z9-1.0', 30, 'UNDERFILL', '底部填充', 'EQ-DP-01', 1495, 1470, 0, 0, 0, 25, 1470, 'OPER-C', 'Operator C', '2026-06-20 12:00:00');

-- ============================================================
-- 十六、签核记录
-- ============================================================

INSERT INTO `prod_signature` VALUES
('SIG-X7-002-01', 'Lot', 'LOT-X7-002', 'Level1', 'QA-USER', 'QA User', 'QA', 'Hold释放签核', '偏移量复检合格，允许放行', '2026-06-20 11:00:00'),
('SIG-X7-003-01', 'Lot', 'LOT-X7-003', 'Level1', 'QA-USER', 'QA User', 'QA', '批次拆分审批', '同意按BIN结果拆分', '2026-06-20 16:00:00'),
('SIG-X7-004-01', 'Lot', 'LOT-X7-004', 'Level1', 'ENG-MANAGER', 'Engineering Manager', 'ENG', '重工审批', '同意WIRE-BOND重工', '2026-06-20 13:00:00');

-- ============================================================
-- 十七、签核级别定义
-- ============================================================

INSERT INTO `sys_signature_level` VALUES
('Level0', '无需签核', 0, '普通操作，无需签核'),
('Level1', '一级签核', 1, '需要班组长或QA签核'),
('Level2', '二级签核', 2, '需要主管签核'),
('Level3', '三级签核', 3, '需要厂长签核');

-- ============================================================
-- 完成
-- ============================================================
