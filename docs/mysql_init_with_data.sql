-- MES MySQL Database Initialization Script with Seed Data
-- 运行此脚本前请确保 MySQL 服务已启动
-- 该脚本将创建数据库、表结构，并插入所有种子数据

-- 创建数据库
CREATE DATABASE IF NOT EXISTS `mes_prod` 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE `mes_prod`;

-- 创建统一 KV 存储表（替代 Redis）
CREATE TABLE IF NOT EXISTS `mes_kv_store` (
    `key` VARCHAR(255) NOT NULL,
    `value` LONGTEXT NOT NULL,
    `data_type` VARCHAR(20) NOT NULL DEFAULT 'string',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`key`),
    INDEX `idx_data_type` (`data_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ========================================
-- 1. Route 数据 - QFN-STD V2.0 (11步)
-- ========================================
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:route:QFN-STD:2.0', JSON_OBJECT(
    'RouteId', 'QFN-STD',
    'RouteName', 'QFN 标准封装流程',
    'RouteVersion', '2.0',
    'ProductId', 'QFN-24',
    'PackageType', 'QFN',
    'IsActive', true,
    'IsApproved', true,
    'ApprovedBy', 'Admin',
    'ApprovedAt', NOW(),
    'Steps', JSON_ARRAY(
        JSON_OBJECT('RouteId', 'QFN-STD', 'StepSeq', 1, 'StepName', 'Saw', 'StepCode', 'SAW', 'EquipmentGroup', 'SAW', 'RequiredCarrierType', 'TapeFrame', 'YieldThreshold', 98, 'QueueTimeLimitMinutes', 480),
        JSON_OBJECT('RouteId', 'QFN-STD', 'StepSeq', 2, 'StepName', 'DieAttach', 'StepCode', 'DA', 'EquipmentGroup', 'DA', 'RequiredCarrierType', 'Magazine', 'YieldThreshold', 97, 'RequiredSignatureLevel', 'Level1', 'ReworkRouteId', 'RW-DA', 'QueueTimeLimitMinutes', 240),
        JSON_OBJECT('RouteId', 'QFN-STD', 'StepSeq', 3, 'StepName', 'Cure', 'StepCode', 'CURE', 'EquipmentGroup', 'OVEN', 'RequiredCarrierType', 'Magazine', 'QueueTimeLimitMinutes', 120),
        JSON_OBJECT('RouteId', 'QFN-STD', 'StepSeq', 4, 'StepName', 'WireBond', 'StepCode', 'WB', 'EquipmentGroup', 'WB', 'RequiredCarrierType', 'Magazine', 'YieldThreshold', 96, 'RequiredSignatureLevel', 'Level1', 'ReworkRouteId', 'RW-WB', 'QueueTimeLimitMinutes', 480),
        JSON_OBJECT('RouteId', 'QFN-STD', 'StepSeq', 5, 'StepName', 'Mold', 'StepCode', 'MOLD', 'EquipmentGroup', 'MP', 'RequiredCarrierType', 'MoldPlate', 'YieldThreshold', 99, 'QueueTimeLimitMinutes', 240),
        JSON_OBJECT('RouteId', 'QFN-STD', 'StepSeq', 6, 'StepName', 'PMC', 'StepCode', 'PMC', 'EquipmentGroup', 'OVEN', 'RequiredCarrierType', 'OvenCart', 'QueueTimeLimitMinutes', 480),
        JSON_OBJECT('RouteId', 'QFN-STD', 'StepSeq', 7, 'StepName', 'Mark', 'StepCode', 'MARK', 'EquipmentGroup', 'MARK', 'RequiredCarrierType', 'Magazine', 'QueueTimeLimitMinutes', 240),
        JSON_OBJECT('RouteId', 'QFN-STD', 'StepSeq', 8, 'StepName', 'Singulation', 'StepCode', 'SING', 'EquipmentGroup', 'SING', 'RequiredCarrierType', 'SingTray', 'YieldThreshold', 98, 'RequiredSignatureLevel', 'Level1', 'EnableMRB', true, 'MRBThreshold', 10, 'QueueTimeLimitMinutes', 240),
        JSON_OBJECT('RouteId', 'QFN-STD', 'StepSeq', 9, 'StepName', 'FinalTest', 'StepCode', 'FT', 'EquipmentGroup', 'TEST', 'RequiredCarrierType', 'TestTray', 'QueueTimeLimitMinutes', 480),
        JSON_OBJECT('RouteId', 'QFN-STD', 'StepSeq', 10, 'StepName', 'OQC', 'StepCode', 'OQC', 'EquipmentGroup', 'OQC', 'RequiredCarrierType', 'Tray', 'YieldThreshold', 100, 'RequiredSignatureLevel', 'Level2', 'QueueTimeLimitMinutes', 240),
        JSON_OBJECT('RouteId', 'QFN-STD', 'StepSeq', 11, 'StepName', 'Packing', 'StepCode', 'PACK', 'EquipmentGroup', 'PACK', 'RequiredCarrierType', 'Reel', 'QueueTimeLimitMinutes', 480)
    )
), 'string'),

('mes:route:index', JSON_ARRAY('QFN-STD:2.0'), 'set');

-- ========================================
-- 2. Rework Routes
-- ========================================
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:route:RW-DA:1.0', JSON_OBJECT(
    'RouteId', 'RW-DA',
    'RouteName', 'DieAttach 重工路线',
    'RouteVersion', '1.0',
    'ProductId', 'QFN-24',
    'PackageType', 'QFN',
    'IsActive', true,
    'IsApproved', true,
    'ApprovedBy', 'Admin',
    'ApprovedAt', NOW(),
    'Steps', JSON_ARRAY(
        JSON_OBJECT('RouteId', 'RW-DA', 'StepSeq', 1, 'StepName', 'Debond', 'StepCode', 'DEBOND', 'EquipmentGroup', 'DA', 'RequiredCarrierType', 'Magazine', 'QueueTimeLimitMinutes', 120),
        JSON_OBJECT('RouteId', 'RW-DA', 'StepSeq', 2, 'StepName', 'Clean', 'StepCode', 'CLEAN', 'EquipmentGroup', 'CLEAN', 'RequiredCarrierType', 'Magazine', 'QueueTimeLimitMinutes', 60),
        JSON_OBJECT('RouteId', 'RW-DA', 'StepSeq', 3, 'StepName', 'Re-DieAttach', 'StepCode', 'DA', 'EquipmentGroup', 'DA', 'RequiredCarrierType', 'Magazine', 'YieldThreshold', 97, 'QueueTimeLimitMinutes', 240),
        JSON_OBJECT('RouteId', 'RW-DA', 'StepSeq', 4, 'StepName', 'Re-Cure', 'StepCode', 'CURE', 'EquipmentGroup', 'OVEN', 'RequiredCarrierType', 'Magazine', 'QueueTimeLimitMinutes', 120)
    )
), 'string'),

('mes:route:RW-WB:1.0', JSON_OBJECT(
    'RouteId', 'RW-WB',
    'RouteName', 'WireBond 重工路线',
    'RouteVersion', '1.0',
    'ProductId', 'QFN-24',
    'PackageType', 'QFN',
    'IsActive', true,
    'IsApproved', true,
    'ApprovedBy', 'Admin',
    'ApprovedAt', NOW(),
    'Steps', JSON_ARRAY(
        JSON_OBJECT('RouteId', 'RW-WB', 'StepSeq', 1, 'StepName', 'Debond-WB', 'StepCode', 'DEBOND-WB', 'EquipmentGroup', 'WB', 'RequiredCarrierType', 'Magazine', 'QueueTimeLimitMinutes', 120),
        JSON_OBJECT('RouteId', 'RW-WB', 'StepSeq', 2, 'StepName', 'Clean-WB', 'StepCode', 'CLEAN-WB', 'EquipmentGroup', 'CLEAN', 'RequiredCarrierType', 'Magazine', 'QueueTimeLimitMinutes', 60),
        JSON_OBJECT('RouteId', 'RW-WB', 'StepSeq', 3, 'StepName', 'Re-WireBond', 'StepCode', 'WB', 'EquipmentGroup', 'WB', 'RequiredCarrierType', 'Magazine', 'YieldThreshold', 96, 'QueueTimeLimitMinutes', 480)
    )
), 'string');

-- 更新 route index
UPDATE `mes_kv_store` SET `value` = JSON_ARRAY('QFN-STD:2.0', 'RW-DA:1.0', 'RW-WB:1.0') WHERE `key` = 'mes:route:index';

-- ========================================
-- 3. YieldRules
-- ========================================
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:yieldrule:QFN-STD:SAW', JSON_OBJECT('RuleId', 'YR-SAW', 'RouteId', 'QFN-STD', 'StepCode', 'SAW', 'YieldThreshold', 98, 'ActionType', 'AutoHold', 'NotifyRole', 'QA', 'IsActive', true), 'string'),
('mes:yieldrule:QFN-STD:DA', JSON_OBJECT('RuleId', 'YR-DA', 'RouteId', 'QFN-STD', 'StepCode', 'DA', 'YieldThreshold', 97, 'ActionType', 'AutoHold', 'NotifyRole', 'QA', 'IsActive', true), 'string'),
('mes:yieldrule:QFN-STD:WB', JSON_OBJECT('RuleId', 'YR-WB', 'RouteId', 'QFN-STD', 'StepCode', 'WB', 'YieldThreshold', 96, 'ActionType', 'AutoHold', 'NotifyRole', 'QA', 'IsActive', true), 'string'),
('mes:yieldrule:QFN-STD:MOLD', JSON_OBJECT('RuleId', 'YR-MOLD', 'RouteId', 'QFN-STD', 'StepCode', 'MOLD', 'YieldThreshold', 99, 'ActionType', 'AutoHold', 'NotifyRole', 'QA', 'IsActive', true), 'string'),
('mes:yieldrule:QFN-STD:SING', JSON_OBJECT('RuleId', 'YR-SING', 'RouteId', 'QFN-STD', 'StepCode', 'SING', 'YieldThreshold', 98, 'ActionType', 'AutoHold', 'NotifyRole', 'QA', 'IsActive', true), 'string'),
('mes:yieldrule:QFN-STD:OQC', JSON_OBJECT('RuleId', 'YR-OQC', 'RouteId', 'QFN-STD', 'StepCode', 'OQC', 'YieldThreshold', 99.5, 'ActionType', 'AutoHold', 'NotifyRole', 'QA', 'IsActive', true), 'string');

-- ========================================
-- 4. WorkOrder
-- ========================================
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:wo:WO-2026001', JSON_OBJECT(
    'OrderId', 'WO-2026001',
    'ProductId', 'QFN-24',
    'ProductName', 'QFN-24 封装',
    'DieName', 'IC-QFN-24',
    'PackageType', 'QFN',
    'PlannedQty', 10000,
    'CompletedQty', 0,
    'UnitQty', 10000,
    'Status', 'InProgress',
    'Priority', 'High',
    'RouteId', 'QFN-STD',
    'RouteName', 'QFN 标准封装流程 V2.0',
    'CustomerId', 'CUS-AUTO-001',
    'CustomerName', '车规客户',
    'CustomerPN', 'AUTO-QFN24-001',
    'InternalPN', 'INT-QFN24-001',
    'TestProgram', 'TP-QFN-FT-V1',
    'BinSpec', 'Bin1:Pass(车规) Bin2:Grade2(工业) Bin3:Fail',
    'WaferSource', 'TSMC',
    'Area', 'PKG1',
    'Line', 'LINE-1',
    'Creator', 'Admin',
    'PlannedStartDate', DATE_SUB(NOW(), INTERVAL 5 DAY),
    'PlannedEndDate', DATE_ADD(NOW(), INTERVAL 10 DAY),
    'TargetCPYield', 99.0,
    'TargetFTYield', 98.0,
    'YieldTarget', 96.0,
    'CreatedAt', DATE_SUB(NOW(), INTERVAL 5 DAY)
), 'string'),

('mes:wo:index', JSON_ARRAY('WO-2026001'), 'set');

-- ========================================
-- 5. 批次数据
-- ========================================

-- 5a. LOT-001 - 新批次，等待在 Saw
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:lot:hold:LOT-001', JSON_OBJECT(
    'LotId', 'LOT-001',
    'OrderId', 'WO-2026001',
    'ProductId', 'QFN-24',
    'ProductName', 'QFN-24 封装',
    'DieName', 'IC-QFN-24',
    'PackageType', 'QFN',
    'RouteId', 'QFN-STD',
    'RouteVersion', '2.0',
    'CurrentStep', 'Saw',
    'CurrentStepSeq', 1,
    'Status', 'Released',
    'OriginalQty', 10000,
    'UnitCount', 10000,
    'StripCount', 100,
    'Priority', 'High',
    'CarrierType', 'WaferFrame',
    'CarrierId', 'FOUP-007',
    'WaferLotId', 'WFR-20260520-001'
), 'string');

-- 5b. LOT-COMPLETE-001 - 已完成全部 11 步
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:lot:hold:LOT-COMPLETE-001', JSON_OBJECT(
    'LotId', 'LOT-COMPLETE-001',
    'OrderId', 'WO-2026001',
    'ProductId', 'QFN-24',
    'ProductName', 'QFN-24 封装',
    'DieName', 'IC-QFN-24',
    'PackageType', 'QFN',
    'RouteId', 'QFN-STD',
    'RouteVersion', '2.0',
    'CurrentStep', 'Packing (完成)',
    'CurrentStepSeq', 11,
    'Status', 'Completed',
    'OriginalQty', 10000,
    'UnitCount', 9550,
    'TotalPassQty', 9550,
    'TotalScrapQty', 450,
    'StripCount', 100,
    'Priority', 'Normal',
    'CarrierType', 'Reel',
    'CarrierId', 'REEL-001',
    'WaferLotId', 'WFR-20260520-001'
), 'string');

-- LOT-COMPLETE-001 的 Step 记录 (11条)
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:1', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-COMPLETE-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 1, 'StepName', 'Saw', 'StepCode', 'SAW', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackInOperator', 'OP-001', 'TrackOutOperator', 'OP-001', 'InputQty', 9970, 'PassQty', 9940, 'FailQty', 10, 'ScrapQty', 20, 'Remark', 'Step 1 Saw 完成'), 'string'),
('mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:2', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-COMPLETE-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 2, 'StepName', 'DieAttach', 'StepCode', 'DA', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackInOperator', 'OP-001', 'TrackOutOperator', 'OP-001', 'InputQty', 9940, 'PassQty', 9910, 'FailQty', 10, 'ScrapQty', 20, 'Remark', 'Step 2 DieAttach 完成'), 'string'),
('mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:3', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-COMPLETE-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 3, 'StepName', 'Cure', 'StepCode', 'CURE', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackInOperator', 'OP-001', 'TrackOutOperator', 'OP-001', 'InputQty', 9910, 'PassQty', 9880, 'FailQty', 10, 'ScrapQty', 20, 'Remark', 'Step 3 Cure 完成'), 'string'),
('mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:4', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-COMPLETE-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 4, 'StepName', 'WireBond', 'StepCode', 'WB', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackInOperator', 'OP-001', 'TrackOutOperator', 'OP-001', 'InputQty', 9880, 'PassQty', 9850, 'FailQty', 10, 'ScrapQty', 20, 'Remark', 'Step 4 WireBond 完成'), 'string'),
('mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:5', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-COMPLETE-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 5, 'StepName', 'Mold', 'StepCode', 'MOLD', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackInOperator', 'OP-001', 'TrackOutOperator', 'OP-001', 'InputQty', 9850, 'PassQty', 9820, 'FailQty', 10, 'ScrapQty', 20, 'Remark', 'Step 5 Mold 完成'), 'string'),
('mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:6', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-COMPLETE-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 6, 'StepName', 'PMC', 'StepCode', 'PMC', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackInOperator', 'OP-001', 'TrackOutOperator', 'OP-001', 'InputQty', 9820, 'PassQty', 9790, 'FailQty', 10, 'ScrapQty', 20, 'Remark', 'Step 6 PMC 完成'), 'string'),
('mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:7', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-COMPLETE-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 7, 'StepName', 'Mark', 'StepCode', 'MARK', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackInOperator', 'OP-001', 'TrackOutOperator', 'OP-001', 'InputQty', 9790, 'PassQty', 9760, 'FailQty', 10, 'ScrapQty', 20, 'Remark', 'Step 7 Mark 完成'), 'string'),
('mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:8', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-COMPLETE-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 8, 'StepName', 'Singulation', 'StepCode', 'SING', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackInOperator', 'OP-001', 'TrackOutOperator', 'OP-001', 'InputQty', 9760, 'PassQty', 9730, 'FailQty', 10, 'ScrapQty', 20, 'Remark', 'Step 8 Singulation 完成'), 'string'),
('mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:9', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-COMPLETE-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 9, 'StepName', 'FinalTest', 'StepCode', 'FT', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackInOperator', 'OP-001', 'TrackOutOperator', 'OP-001', 'InputQty', 9730, 'PassQty', 9700, 'FailQty', 10, 'ScrapQty', 20, 'Remark', 'Step 9 FinalTest 完成'), 'string'),
('mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:10', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-COMPLETE-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 10, 'StepName', 'OQC', 'StepCode', 'OQC', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackInOperator', 'OP-001', 'TrackOutOperator', 'OP-001', 'InputQty', 9700, 'PassQty', 9670, 'FailQty', 10, 'ScrapQty', 20, 'Remark', 'Step 10 OQC 完成'), 'string'),
('mes:lot:step:LOT-COMPLETE-001:QFN-STD:2.0:11', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-COMPLETE-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 11, 'StepName', 'Packing', 'StepCode', 'PACK', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 3 DAY), 'TrackInOperator', 'OP-001', 'TrackOutOperator', 'OP-001', 'InputQty', 9670, 'PassQty', 9550, 'FailQty', 10, 'ScrapQty', 110, 'Remark', 'Step 11 Packing 完成'), 'string');

-- 5c. LOT-HOLD-001 - WireBond 低良率 Hold
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:lot:hold:LOT-HOLD-001', JSON_OBJECT(
    'LotId', 'LOT-HOLD-001',
    'OrderId', 'WO-2026001',
    'ProductId', 'QFN-24',
    'ProductName', 'QFN-24 封装',
    'DieName', 'IC-QFN-24',
    'PackageType', 'QFN',
    'RouteId', 'QFN-STD',
    'RouteVersion', '2.0',
    'CurrentStep', 'WireBond',
    'CurrentStepSeq', 4,
    'Status', 'Hold',
    'OriginalQty', 9900,
    'UnitCount', 8200,
    'TotalPassQty', 8200,
    'TotalScrapQty', 1200,
    'TotalHoldQty', 500,
    'StripCount', 100,
    'Priority', 'High',
    'CarrierType', 'Magazine',
    'CarrierId', 'MAG-022',
    'WaferLotId', 'WFR-20260520-001',
    'HoldCategory', 'YieldHold',
    'HoldReason', 'WireBond 良率 82.8% < 阈值 96%，自动 Hold',
    'HoldTime', DATE_SUB(NOW(), INTERVAL 2 HOUR),
    'HoldOperator', 'SYSTEM',
    'ReleaseCondition', '工程分析原因并签核后释放'
), 'string'),

('mes:lot:hold:index', JSON_ARRAY('LOT-HOLD-001'), 'set');

-- LOT-HOLD-001 的 Step 记录
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:lot:step:LOT-HOLD-001:QFN-STD:2.0:1', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-HOLD-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 1, 'StepName', 'Saw', 'StepCode', 'SAW', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 2 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 2 DAY), 'TrackInOperator', 'OP-002', 'TrackOutOperator', 'OP-002', 'InputQty', 9900, 'PassQty', 9900, 'ScrapQty', 50, 'Remark', 'Step 1 完成'), 'string'),
('mes:lot:step:LOT-HOLD-001:QFN-STD:2.0:2', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-HOLD-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 2, 'StepName', 'DieAttach', 'StepCode', 'DA', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 2 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 2 DAY), 'TrackInOperator', 'OP-002', 'TrackOutOperator', 'OP-002', 'InputQty', 9800, 'PassQty', 9800, 'ScrapQty', 50, 'Remark', 'Step 2 完成'), 'string'),
('mes:lot:step:LOT-HOLD-001:QFN-STD:2.0:3', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-HOLD-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 3, 'StepName', 'Cure', 'StepCode', 'CURE', 'Status', 'Completed', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 2 DAY), 'TrackOutTime', DATE_SUB(NOW(), INTERVAL 2 DAY), 'TrackInOperator', 'OP-002', 'TrackOutOperator', 'OP-002', 'InputQty', 9700, 'PassQty', 9700, 'ScrapQty', 50, 'Remark', 'Step 3 完成'), 'string'),
('mes:lot:step:LOT-HOLD-001:QFN-STD:2.0:4', JSON_OBJECT('RecordId', REPLACE(UUID(), '-', ''), 'LotId', 'LOT-HOLD-001', 'RouteId', 'QFN-STD', 'RouteVersion', '2.0', 'StepSeq', 4, 'StepName', 'WireBond', 'StepCode', 'WB', 'Status', 'Processing', 'TrackInTime', DATE_SUB(NOW(), INTERVAL 2 DAY), 'TrackInOperator', 'OP-002', 'InputQty', 9600, 'PassQty', 8200, 'ScrapQty', 1200, 'Remark', 'WireBond 加工中触发低良率'), 'string');

-- 5d. LOT-REWORK-001 - WireBond 重工中
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:lot:hold:LOT-REWORK-001', JSON_OBJECT(
    'LotId', 'LOT-REWORK-001',
    'OrderId', 'WO-2026001',
    'ProductId', 'QFN-24',
    'ProductName', 'QFN-24 封装',
    'DieName', 'IC-QFN-24',
    'PackageType', 'QFN',
    'RouteId', 'QFN-STD',
    'RouteVersion', '2.0',
    'CurrentStep', 'Re-WireBond',
    'CurrentStepSeq', 3,
    'Status', 'Processing',
    'IsReworkLot', true,
    'OriginalRouteId', 'QFN-STD',
    'ReworkRouteId', 'RW-WB',
    'ReworkCount', 1,
    'ReworkReason', 'WireBond 断线重工',
    'OriginalQty', 6000,
    'UnitCount', 5700,
    'TotalPassQty', 5700,
    'TotalScrapQty', 300,
    'StripCount', 60,
    'Priority', 'High',
    'CarrierType', 'Magazine',
    'CarrierId', 'MAG-033',
    'WaferLotId', 'WFR-20260520-001'
), 'string');

-- 5e. LOT-MRB-001 - MRB 审查中
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:lot:hold:LOT-MRB-001', JSON_OBJECT(
    'LotId', 'LOT-MRB-001',
    'OrderId', 'WO-2026001',
    'ProductId', 'QFN-24',
    'ProductName', 'QFN-24 封装',
    'DieName', 'IC-QFN-24',
    'PackageType', 'QFN',
    'RouteId', 'QFN-STD',
    'RouteVersion', '2.0',
    'CurrentStep', 'Singulation',
    'CurrentStepSeq', 8,
    'Status', 'Hold',
    'IsUnderMRB', true,
    'MRBReference', 'MRB-20260522-001',
    'MRBDisposition', 'Pending',
    'OriginalQty', 9800,
    'UnitCount', 9600,
    'TotalPassQty', 9600,
    'TotalScrapQty', 200,
    'TotalHoldQty', 200,
    'StripCount', 98,
    'Priority', 'High',
    'CarrierType', 'SingTray',
    'CarrierId', 'SING-015',
    'WaferLotId', 'WFR-20260520-001',
    'HoldCategory', 'Quality',
    'HoldReason', 'Singulation 不良率 2.04% > MRB 阈值 1%，触发 MRB 审查',
    'HoldTime', DATE_SUB(NOW(), INTERVAL 4 HOUR),
    'HoldOperator', 'QA-001',
    'ReleaseCondition', 'MRB 结论: Repair 180 pcs, Scrap 20 pcs'
), 'string');

-- 更新 hold index
UPDATE `mes_kv_store` SET `value` = JSON_ARRAY('LOT-HOLD-001', 'LOT-MRB-001') WHERE `key` = 'mes:lot:hold:index';

-- 5f. LOT-GRADE-A/B-001 - 已完成 + Grade Split
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:lot:hold:LOT-GRADE-A-001', JSON_OBJECT(
    'LotId', 'LOT-GRADE-A-001',
    'OrderId', 'WO-2026001',
    'ProductId', 'QFN-24',
    'ProductName', 'QFN-24 封装',
    'DieName', 'IC-QFN-24',
    'PackageType', 'QFN',
    'RouteId', 'QFN-STD',
    'RouteVersion', '2.0',
    'CurrentStep', 'Packing (完成)',
    'CurrentStepSeq', 11,
    'Status', 'Completed',
    'Grade', 'A',
    'OriginalLotId', 'LOT-GRADE-001',
    'OriginalQty', 8500,
    'UnitCount', 8500,
    'TotalPassQty', 8500,
    'StripCount', 85,
    'Priority', 'High',
    'CarrierType', 'Reel',
    'CarrierId', 'REEL-002',
    'WaferLotId', 'WFR-20260520-001'
), 'string'),
('mes:lot:hold:LOT-GRADE-B-001', JSON_OBJECT(
    'LotId', 'LOT-GRADE-B-001',
    'OrderId', 'WO-2026001',
    'ProductId', 'QFN-24',
    'ProductName', 'QFN-24 封装',
    'DieName', 'IC-QFN-24',
    'PackageType', 'QFN',
    'RouteId', 'QFN-STD',
    'RouteVersion', '2.0',
    'CurrentStep', 'Packing (完成)',
    'CurrentStepSeq', 11,
    'Status', 'Completed',
    'Grade', 'B',
    'OriginalLotId', 'LOT-GRADE-001',
    'OriginalQty', 1030,
    'UnitCount', 1030,
    'TotalPassQty', 1030,
    'StripCount', 10,
    'Priority', 'Normal',
    'CarrierType', 'Tray',
    'CarrierId', 'TRAY-003',
    'WaferLotId', 'WFR-20260520-001'
), 'string');

-- ========================================
-- 6. Phase 3 Seed Data
-- ========================================

-- 6a. 拆分批次
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:split:SPL-001', JSON_OBJECT('SplitId', 'SPL-001', 'MotherLotId', 'LOT-001', 'ChildLotId', 'LOT-001-S001', 'SplitQty', 3000, 'SplitReason', '分线生产', 'SplitType', 'Normal', 'StepCode', 'SAW', 'StepSeq', 1, 'OperatorId', 'OP-001', 'SplitTime', DATE_SUB(NOW(), INTERVAL 1 HOUR)), 'string'),
('mes:split:index:LOT-001', JSON_ARRAY('SPL-001'), 'list'),
('mes:lot:hold:LOT-001-S001', JSON_OBJECT(
    'LotId', 'LOT-001-S001',
    'OrderId', 'WO-2026001',
    'ProductId', 'QFN-24',
    'ProductName', 'QFN-24 封装',
    'DieName', 'IC-QFN-24',
    'PackageType', 'QFN',
    'RouteId', 'QFN-STD',
    'RouteVersion', '2.0',
    'CurrentStep', 'Saw',
    'CurrentStepSeq', 1,
    'Status', 'Waiting',
    'OriginalQty', 3000,
    'UnitCount', 3000,
    'StripCount', 30,
    'Priority', 'High',
    'CarrierType', 'WaferFrame',
    'CarrierId', 'FOUP-008',
    'WaferLotId', 'WFR-20260520-001',
    'IsPartialLot', true,
    'MotherLotId', 'LOT-001',
    'SplitReason', '分线生产',
    'SplitTime', DATE_SUB(NOW(), INTERVAL 1 HOUR),
    'SplitQty', 3000
), 'string');

-- 更新 LOT-001 数量
UPDATE `mes_kv_store` SET `value` = JSON_SET(`value`, '$.UnitCount', 7000) WHERE `key` = 'mes:lot:hold:LOT-001';

-- 6b. 等级拆分
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:split:SPL-GRADE-A', JSON_OBJECT('SplitId', 'SPL-GRADE-A', 'MotherLotId', 'LOT-COMPLETE-001', 'ChildLotId', 'LOT-COMPLETE-001-GA', 'SplitQty', 8500, 'SplitReason', 'Grade Split: Bin1 车规级', 'SplitType', 'Grade', 'StepCode', 'FT', 'StepSeq', 9, 'OperatorId', 'OP-001', 'SplitTime', DATE_SUB(NOW(), INTERVAL 2 HOUR)), 'string'),
('mes:split:index:LOT-COMPLETE-001', JSON_ARRAY('SPL-GRADE-A'), 'list'),
('mes:lot:hold:LOT-COMPLETE-001-GA', JSON_OBJECT(
    'LotId', 'LOT-COMPLETE-001-GA',
    'OrderId', 'WO-2026001',
    'ProductId', 'QFN-24',
    'ProductName', 'QFN-24 封装',
    'DieName', 'IC-QFN-24',
    'PackageType', 'QFN',
    'RouteId', 'QFN-STD',
    'RouteVersion', '2.0',
    'CurrentStep', 'OQC',
    'CurrentStepSeq', 10,
    'Status', 'Waiting',
    'OriginalQty', 8500,
    'UnitCount', 8500,
    'StripCount', 85,
    'Priority', 'High',
    'CarrierType', 'Tray',
    'CarrierId', 'TRAY-GA-001',
    'WaferLotId', 'WFR-20260520-001',
    'IsPartialLot', true,
    'MotherLotId', 'LOT-COMPLETE-001',
    'SplitReason', 'Grade Split: Bin1 车规级',
    'SplitTime', DATE_SUB(NOW(), INTERVAL 2 HOUR),
    'SplitQty', 8500,
    'Grade', 'Grade1'
), 'string');

-- 6c. 载具绑定历史
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:carrier:binding:BIND-001', JSON_OBJECT('BindingId', 'BIND-001', 'LotId', 'LOT-001', 'StepCode', 'SAW', 'StepSeq', 1, 'CarrierId', 'FOUP-007', 'CarrierType', 'WaferFrame', 'BindTime', DATE_SUB(NOW(), INTERVAL 1 DAY), 'OperatorId', 'OP-001'), 'string'),
('mes:carrier:binding:BIND-002', JSON_OBJECT('BindingId', 'BIND-002', 'LotId', 'LOT-HOLD-001', 'StepCode', 'DA', 'StepSeq', 2, 'CarrierId', 'MAG-022', 'CarrierType', 'Magazine', 'BindTime', DATE_SUB(NOW(), INTERVAL 2 DAY), 'OperatorId', 'OP-002'), 'string'),
('mes:carrier:binding:BIND-003', JSON_OBJECT('BindingId', 'BIND-003', 'LotId', 'LOT-REWORK-001', 'StepCode', 'DEBOND-WB', 'StepSeq', 1, 'CarrierId', 'MAG-033', 'CarrierType', 'Magazine', 'BindTime', DATE_SUB(NOW(), INTERVAL 3 HOUR), 'OperatorId', 'OP-003'), 'string'),
('mes:carrier:history:LOT-001', JSON_ARRAY('BIND-001'), 'list'),
('mes:carrier:history:LOT-HOLD-001', JSON_ARRAY('BIND-002'), 'list'),
('mes:carrier:history:LOT-REWORK-001', JSON_ARRAY('BIND-003'), 'list'),
('mes:carrier:current:FOUP-007', JSON_OBJECT('BindingId', 'BIND-001', 'LotId', 'LOT-001', 'StepCode', 'SAW', 'StepSeq', 1, 'CarrierId', 'FOUP-007', 'CarrierType', 'WaferFrame', 'BindTime', DATE_SUB(NOW(), INTERVAL 1 DAY), 'OperatorId', 'OP-001'), 'string'),
('mes:carrier:current:MAG-022', JSON_OBJECT('BindingId', 'BIND-002', 'LotId', 'LOT-HOLD-001', 'StepCode', 'DA', 'StepSeq', 2, 'CarrierId', 'MAG-022', 'CarrierType', 'Magazine', 'BindTime', DATE_SUB(NOW(), INTERVAL 2 DAY), 'OperatorId', 'OP-002'), 'string'),
('mes:carrier:current:MAG-033', JSON_OBJECT('BindingId', 'BIND-003', 'LotId', 'LOT-REWORK-001', 'StepCode', 'DEBOND-WB', 'StepSeq', 1, 'CarrierId', 'MAG-033', 'CarrierType', 'Magazine', 'BindTime', DATE_SUB(NOW(), INTERVAL 3 HOUR), 'OperatorId', 'OP-003'), 'string');

-- 6d. 谱系记录
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:genealogy:GEN-001', JSON_OBJECT('GenealogyId', 'GEN-001', 'ParentLotId', 'WFR-20260520-001', 'ChildLotId', 'LOT-001', 'RelationType', 'Create', 'StepCode', 'SAW', 'Qty', 10000, 'OperatorId', 'OP-001', 'CreatedAt', DATE_SUB(NOW(), INTERVAL 1 DAY)), 'string'),
('mes:genealogy:GEN-002', JSON_OBJECT('GenealogyId', 'GEN-002', 'ParentLotId', 'LOT-001', 'ChildLotId', 'LOT-001-S001', 'RelationType', 'Split', 'StepCode', 'SAW', 'Qty', 3000, 'OperatorId', 'OP-001', 'CreatedAt', DATE_SUB(NOW(), INTERVAL 1 HOUR)), 'string'),
('mes:genealogy:GEN-003', JSON_OBJECT('GenealogyId', 'GEN-003', 'ParentLotId', 'LOT-COMPLETE-001', 'ChildLotId', 'LOT-COMPLETE-001-GA', 'RelationType', 'GradeSplit', 'StepCode', 'FT', 'Qty', 8500, 'OperatorId', 'OP-001', 'CreatedAt', DATE_SUB(NOW(), INTERVAL 2 HOUR)), 'string'),
('mes:genealogy:GEN-004', JSON_OBJECT('GenealogyId', 'GEN-004', 'ParentLotId', 'LOT-HOLD-001', 'ChildLotId', 'LOT-REWORK-001', 'RelationType', 'Rework', 'StepCode', 'WB', 'Qty', 6000, 'OperatorId', 'OP-002', 'CreatedAt', DATE_SUB(NOW(), INTERVAL 3 HOUR)), 'string'),
('mes:genealogy:tree:WFR-20260520-001', JSON_ARRAY('GEN-001'), 'list'),
('mes:genealogy:tree:LOT-001', JSON_ARRAY('GEN-001', 'GEN-002'), 'list'),
('mes:genealogy:tree:LOT-001-S001', JSON_ARRAY('GEN-002'), 'list'),
('mes:genealogy:tree:LOT-COMPLETE-001', JSON_ARRAY('GEN-003'), 'list'),
('mes:genealogy:tree:LOT-COMPLETE-001-GA', JSON_ARRAY('GEN-003'), 'list'),
('mes:genealogy:tree:LOT-HOLD-001', JSON_ARRAY('GEN-004'), 'list'),
('mes:genealogy:tree:LOT-REWORK-001', JSON_ARRAY('GEN-004'), 'list');

-- 6e. 报废记录
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:scrap:SCR-001', JSON_OBJECT('ScrapId', 'SCR-001', 'LotId', 'LOT-HOLD-001', 'StepCode', 'WB', 'StepSeq', 4, 'ScrapQty', 1200, 'ScrapReason', 'WireBond 断线', 'ScrapReasonCode', 'WB-BROKEN', 'OperatorId', 'OP-002', 'ScrapTime', DATE_SUB(NOW(), INTERVAL 2 HOUR), 'RequiresApproval', true), 'string'),
('mes:scrap:SCR-002', JSON_OBJECT('ScrapId', 'SCR-002', 'LotId', 'LOT-REWORK-001', 'StepCode', 'DEBOND-WB', 'StepSeq', 1, 'ScrapQty', 300, 'ScrapReason', '重工过程中损坏', 'ScrapReasonCode', 'RW-DAMAGE', 'OperatorId', 'OP-003', 'ScrapTime', DATE_SUB(NOW(), INTERVAL 1 HOUR), 'RequiresApproval', false), 'string'),
('mes:scrap:index:LOT-HOLD-001', JSON_ARRAY('SCR-001'), 'list'),
('mes:scrap:index:LOT-REWORK-001', JSON_ARRAY('SCR-002'), 'list');

-- 6f. 重工记录
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:rework:RW-001', JSON_OBJECT('ReworkId', 'RW-001', 'LotId', 'LOT-HOLD-001', 'OriginalRouteId', 'QFN-STD', 'ReworkRouteId', 'RW-WB', 'FromStepCode', 'WB', 'TargetStepCode', 'WB', 'ReworkQty', 6000, 'ReworkReason', 'WireBond 断线重工', 'OperatorId', 'OP-002', 'ReworkCount', 1, 'CreatedAt', DATE_SUB(NOW(), INTERVAL 3 HOUR)), 'string'),
('mes:rework:index:LOT-HOLD-001', JSON_ARRAY('RW-001'), 'list');

-- ========================================
-- 7. Master Data
-- ========================================

-- 7a. 设备数据
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:master:equipment:SAW-001', JSON_OBJECT('EquipmentId', 'SAW-001', 'EquipmentName', '切割机 #1', 'EquipmentGroup', 'SAW', 'EquipmentType', 'Dicing', 'Status', 'Available', 'Location', 'Line A-01', 'ResponsiblePerson', '张工', 'LastMaintenanceDate', DATE_SUB(NOW(), INTERVAL 7 DAY), 'MaintenanceIntervalHours', 500, 'RunningHours', 120, 'CreatedAt', NOW()), 'string'),
('mes:master:equipment:SAW-002', JSON_OBJECT('EquipmentId', 'SAW-002', 'EquipmentName', '切割机 #2', 'EquipmentGroup', 'SAW', 'EquipmentType', 'Dicing', 'Status', 'Running', 'CurrentLotId', 'LOT-001', 'Location', 'Line A-02', 'ResponsiblePerson', '张工', 'LastMaintenanceDate', DATE_SUB(NOW(), INTERVAL 3 DAY), 'MaintenanceIntervalHours', 500, 'RunningHours', 350, 'CreatedAt', NOW()), 'string'),
('mes:master:equipment:DA-001', JSON_OBJECT('EquipmentId', 'DA-001', 'EquipmentName', '固晶机 #1', 'EquipmentGroup', 'DA', 'EquipmentType', 'DieAttach', 'Status', 'Available', 'Location', 'Line B-01', 'ResponsiblePerson', '李工', 'LastMaintenanceDate', DATE_SUB(NOW(), INTERVAL 5 DAY), 'MaintenanceIntervalHours', 400, 'RunningHours', 200, 'CreatedAt', NOW()), 'string'),
('mes:master:equipment:WB-001', JSON_OBJECT('EquipmentId', 'WB-001', 'EquipmentName', '焊线机 #1', 'EquipmentGroup', 'WB', 'EquipmentType', 'WireBond', 'Status', 'Available', 'Location', 'Line C-01', 'ResponsiblePerson', '王工', 'LastMaintenanceDate', DATE_SUB(NOW(), INTERVAL 2 DAY), 'MaintenanceIntervalHours', 300, 'RunningHours', 180, 'CreatedAt', NOW()), 'string'),
('mes:master:equipment:WB-002', JSON_OBJECT('EquipmentId', 'WB-002', 'EquipmentName', '焊线机 #2', 'EquipmentGroup', 'WB', 'EquipmentType', 'WireBond', 'Status', 'Maintenance', 'Location', 'Line C-02', 'ResponsiblePerson', '王工', 'LastMaintenanceDate', NOW(), 'MaintenanceIntervalHours', 300, 'RunningHours', 300, 'CreatedAt', NOW()), 'string'),
('mes:master:equipment:MD-001', JSON_OBJECT('EquipmentId', 'MD-001', 'EquipmentName', '塑封机 #1', 'EquipmentGroup', 'MD', 'EquipmentType', 'Molding', 'Status', 'Available', 'Location', 'Line D-01', 'ResponsiblePerson', '赵工', 'LastMaintenanceDate', DATE_SUB(NOW(), INTERVAL 10 DAY), 'MaintenanceIntervalHours', 600, 'RunningHours', 450, 'CreatedAt', NOW()), 'string'),
('mes:master:equipment:PM-001', JSON_OBJECT('EquipmentId', 'PM-001', 'EquipmentName', '切筋成型机 #1', 'EquipmentGroup', 'PM', 'EquipmentType', 'TrimForm', 'Status', 'Available', 'Location', 'Line E-01', 'ResponsiblePerson', '刘工', 'LastMaintenanceDate', DATE_SUB(NOW(), INTERVAL 4 DAY), 'MaintenanceIntervalHours', 400, 'RunningHours', 280, 'CreatedAt', NOW()), 'string'),
('mes:master:equipment:PL-001', JSON_OBJECT('EquipmentId', 'PL-001', 'EquipmentName', '电镀线 #1', 'EquipmentGroup', 'PL', 'EquipmentType', 'Plating', 'Status', 'Available', 'Location', 'Line F-01', 'ResponsiblePerson', '陈工', 'LastMaintenanceDate', DATE_SUB(NOW(), INTERVAL 6 DAY), 'MaintenanceIntervalHours', 350, 'RunningHours', 150, 'CreatedAt', NOW()), 'string'),
('mes:master:equipment:FT-001', JSON_OBJECT('EquipmentId', 'FT-001', 'EquipmentName', '终测机 #1', 'EquipmentGroup', 'FT', 'EquipmentType', 'FinalTest', 'Status', 'Available', 'Location', 'Line G-01', 'ResponsiblePerson', '孙工', 'LastMaintenanceDate', DATE_SUB(NOW(), INTERVAL 1 DAY), 'MaintenanceIntervalHours', 200, 'RunningHours', 90, 'CreatedAt', NOW()), 'string'),
('mes:master:equipment:OQC-001', JSON_OBJECT('EquipmentId', 'OQC-001', 'EquipmentName', 'OQC 检验台 #1', 'EquipmentGroup', 'OQC', 'EquipmentType', 'Inspection', 'Status', 'Available', 'Location', 'Line H-01', 'ResponsiblePerson', '周工', 'LastMaintenanceDate', DATE_SUB(NOW(), INTERVAL 15 DAY), 'MaintenanceIntervalHours', 1000, 'RunningHours', 50, 'CreatedAt', NOW()), 'string'),
('mes:master:equipment:index', JSON_ARRAY('SAW-001', 'SAW-002', 'DA-001', 'WB-001', 'WB-002', 'MD-001', 'PM-001', 'PL-001', 'FT-001', 'OQC-001'), 'set');

-- 7b. 载具主数据
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:master:carrier:FOUP-001', JSON_OBJECT('CarrierId', 'FOUP-001', 'CarrierType', 'FOUP', 'Status', 'Available', 'Capacity', 25, 'UseCount', 120, 'MaxUseCount', 1000, 'LastCleanDate', DATE_SUB(NOW(), INTERVAL 1 DAY), 'CleanIntervalUses', 5, 'Location', 'Warehouse A', 'CreatedAt', NOW()), 'string'),
('mes:master:carrier:FOUP-002', JSON_OBJECT('CarrierId', 'FOUP-002', 'CarrierType', 'FOUP', 'Status', 'InUse', 'CurrentLotId', 'LOT-001', 'Capacity', 25, 'UseCount', 85, 'MaxUseCount', 1000, 'LastCleanDate', DATE_SUB(NOW(), INTERVAL 2 DAY), 'CleanIntervalUses', 5, 'Location', 'Line A', 'CreatedAt', NOW()), 'string'),
('mes:master:carrier:MAG-001', JSON_OBJECT('CarrierId', 'MAG-001', 'CarrierType', 'Magazine', 'Status', 'Available', 'Capacity', 10, 'UseCount', 200, 'MaxUseCount', 2000, 'LastCleanDate', DATE_SUB(NOW(), INTERVAL 3 DAY), 'CleanIntervalUses', 10, 'Location', 'Warehouse B', 'CreatedAt', NOW()), 'string'),
('mes:master:carrier:MAG-002', JSON_OBJECT('CarrierId', 'MAG-002', 'CarrierType', 'Magazine', 'Status', 'Available', 'Capacity', 10, 'UseCount', 150, 'MaxUseCount', 2000, 'LastCleanDate', DATE_SUB(NOW(), INTERVAL 1 DAY), 'CleanIntervalUses', 10, 'Location', 'Warehouse B', 'CreatedAt', NOW()), 'string'),
('mes:master:carrier:TRAY-001', JSON_OBJECT('CarrierId', 'TRAY-001', 'CarrierType', 'Tray', 'Status', 'Available', 'Capacity', 50, 'UseCount', 300, 'MaxUseCount', 5000, 'LastCleanDate', DATE_SUB(NOW(), INTERVAL 5 DAY), 'CleanIntervalUses', 20, 'Location', 'Warehouse C', 'CreatedAt', NOW()), 'string'),
('mes:master:carrier:REEL-001', JSON_OBJECT('CarrierId', 'REEL-001', 'CarrierType', 'Reel', 'Status', 'Available', 'Capacity', 5000, 'UseCount', 50, 'MaxUseCount', 10000, 'LastCleanDate', DATE_SUB(NOW(), INTERVAL 10 DAY), 'CleanIntervalUses', 50, 'Location', 'Warehouse D', 'CreatedAt', NOW()), 'string'),
('mes:master:carrier:index', JSON_ARRAY('FOUP-001', 'FOUP-002', 'MAG-001', 'MAG-002', 'TRAY-001', 'REEL-001'), 'set');

-- 7c. Recipe 数据
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:master:recipe:RCP-SAW-QFN24-001', JSON_OBJECT('RecipeId', 'RCP-SAW-QFN24-001', 'RecipeName', 'QFN-24 切割配方', 'EquipmentGroup', 'SAW', 'ProductId', 'QFN-24', 'StepCode', 'SAW', 'Version', '2.1', 'IsActive', true, 'ApprovedBy', 'ENG-001', 'ApprovedAt', DATE_SUB(NOW(), INTERVAL 30 DAY), 'CreatedAt', NOW()), 'string'),
('mes:master:recipe:RCP-DA-QFN24-001', JSON_OBJECT('RecipeId', 'RCP-DA-QFN24-001', 'RecipeName', 'QFN-24 固晶配方', 'EquipmentGroup', 'DA', 'ProductId', 'QFN-24', 'StepCode', 'DA', 'Version', '1.5', 'IsActive', true, 'ApprovedBy', 'ENG-001', 'ApprovedAt', DATE_SUB(NOW(), INTERVAL 25 DAY), 'CreatedAt', NOW()), 'string'),
('mes:master:recipe:RCP-WB-QFN24-001', JSON_OBJECT('RecipeId', 'RCP-WB-QFN24-001', 'RecipeName', 'QFN-24 焊线配方', 'EquipmentGroup', 'WB', 'ProductId', 'QFN-24', 'StepCode', 'WB', 'Version', '3.0', 'IsActive', true, 'ApprovedBy', 'ENG-002', 'ApprovedAt', DATE_SUB(NOW(), INTERVAL 15 DAY), 'CreatedAt', NOW()), 'string'),
('mes:master:recipe:RCP-MD-QFN24-001', JSON_OBJECT('RecipeId', 'RCP-MD-QFN24-001', 'RecipeName', 'QFN-24 塑封配方', 'EquipmentGroup', 'MD', 'ProductId', 'QFN-24', 'StepCode', 'MD', 'Version', '1.2', 'IsActive', true, 'ApprovedBy', 'ENG-001', 'ApprovedAt', DATE_SUB(NOW(), INTERVAL 20 DAY), 'CreatedAt', NOW()), 'string'),
('mes:master:recipe:RCP-FT-QFN24-001', JSON_OBJECT('RecipeId', 'RCP-FT-QFN24-001', 'RecipeName', 'QFN-24 终测配方', 'EquipmentGroup', 'FT', 'ProductId', 'QFN-24', 'StepCode', 'FT', 'Version', '2.0', 'IsActive', true, 'ApprovedBy', 'QA-001', 'ApprovedAt', DATE_SUB(NOW(), INTERVAL 10 DAY), 'CreatedAt', NOW()), 'string'),
('mes:master:recipe:index', JSON_ARRAY('RCP-SAW-QFN24-001', 'RCP-DA-QFN24-001', 'RCP-WB-QFN24-001', 'RCP-MD-QFN24-001', 'RCP-FT-QFN24-001'), 'set');

-- 7d. 用户数据
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:master:user:OP-001', JSON_OBJECT('UserId', 'OP-001', 'UserName', '张三', 'Role', 'Operator', 'Department', '生产部', 'Shift', 'A', 'IsActive', true, 'Permissions', JSON_ARRAY('TrackIn', 'TrackOut', 'Hold'), 'CreatedAt', NOW()), 'string'),
('mes:master:user:OP-002', JSON_OBJECT('UserId', 'OP-002', 'UserName', '李四', 'Role', 'Operator', 'Department', '生产部', 'Shift', 'B', 'IsActive', true, 'Permissions', JSON_ARRAY('TrackIn', 'TrackOut', 'Hold'), 'CreatedAt', NOW()), 'string'),
('mes:master:user:OP-003', JSON_OBJECT('UserId', 'OP-003', 'UserName', '王五', 'Role', 'Operator', 'Department', '生产部', 'Shift', 'C', 'IsActive', true, 'Permissions', JSON_ARRAY('TrackIn', 'TrackOut'), 'CreatedAt', NOW()), 'string'),
('mes:master:user:ENG-001', JSON_OBJECT('UserId', 'ENG-001', 'UserName', '赵工', 'Role', 'Engineer', 'Department', '工程部', 'Shift', 'A', 'IsActive', true, 'Permissions', JSON_ARRAY('TrackIn', 'TrackOut', 'Hold', 'Release', 'Rework', 'Scrap', 'ForceTrack'), 'CreatedAt', NOW()), 'string'),
('mes:master:user:ENG-002', JSON_OBJECT('UserId', 'ENG-002', 'UserName', '刘工', 'Role', 'Engineer', 'Department', '工程部', 'Shift', 'B', 'IsActive', true, 'Permissions', JSON_ARRAY('TrackIn', 'TrackOut', 'Hold', 'Release', 'Rework'), 'CreatedAt', NOW()), 'string'),
('mes:master:user:QA-001', JSON_OBJECT('UserId', 'QA-001', 'UserName', '陈工', 'Role', 'QA', 'Department', '质量部', 'Shift', 'A', 'IsActive', true, 'Permissions', JSON_ARRAY('Hold', 'Release', 'Scrap', 'Audit'), 'CreatedAt', NOW()), 'string'),
('mes:master:user:SUP-001', JSON_OBJECT('UserId', 'SUP-001', 'UserName', '孙主管', 'Role', 'Supervisor', 'Department', '生产部', 'Shift', 'A', 'IsActive', true, 'Permissions', JSON_ARRAY('TrackIn', 'TrackOut', 'Hold', 'Release', 'Rework', 'Scrap', 'ForceTrack', 'Split', 'Merge'), 'CreatedAt', NOW()), 'string'),
('mes:master:user:ADMIN', JSON_OBJECT('UserId', 'ADMIN', 'UserName', '系统管理员', 'Role', 'Admin', 'Department', 'IT', 'Shift', 'A', 'IsActive', true, 'Permissions', JSON_ARRAY('*'), 'CreatedAt', NOW()), 'string'),
('mes:master:user:index', JSON_ARRAY('OP-001', 'OP-002', 'OP-003', 'ENG-001', 'ENG-002', 'QA-001', 'SUP-001', 'ADMIN'), 'set');

-- ========================================
-- 8. AlarmRule 种子数据
-- ========================================
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:alarm:rule:LowYield', JSON_OBJECT('RuleId', 'AR-001', 'AlarmType', 'LowYield', 'IsEnabled', true, 'Severity', 'Error', 'ThresholdYield', 90.0, 'NotifyRole', 'QA'), 'string'),
('mes:alarm:rule:QueueTimeout', JSON_OBJECT('RuleId', 'AR-002', 'AlarmType', 'QueueTimeout', 'IsEnabled', true, 'Severity', 'Warning', 'ThresholdMinutes', 120, 'NotifyRole', 'Supervisor'), 'string'),
('mes:alarm:rule:HoldTimeout', JSON_OBJECT('RuleId', 'AR-003', 'AlarmType', 'HoldTimeout', 'IsEnabled', true, 'Severity', 'Warning', 'ThresholdMinutes', 480, 'NotifyRole', 'QA'), 'string'),
('mes:alarm:rule:EquipmentDown', JSON_OBJECT('RuleId', 'AR-004', 'AlarmType', 'EquipmentDown', 'IsEnabled', true, 'Severity', 'Critical', 'NotifyRole', 'Maintenance'), 'string'),
('mes:alarm:rule:RecipeError', JSON_OBJECT('RuleId', 'AR-005', 'AlarmType', 'RecipeError', 'IsEnabled', true, 'Severity', 'Error', 'NotifyRole', 'Engineer'), 'string'),
('mes:alarm:rule:MaterialShort', JSON_OBJECT('RuleId', 'AR-006', 'AlarmType', 'MaterialShort', 'IsEnabled', true, 'Severity', 'Warning', 'ThresholdQty', 10, 'NotifyRole', 'Warehouse'), 'string'),
('mes:alarm:rule:ForceOperation', JSON_OBJECT('RuleId', 'AR-007', 'AlarmType', 'ForceOperation', 'IsEnabled', true, 'Severity', 'Critical', 'NotifyRole', 'Manager'), 'string'),
('mes:alarm:rule:QtyImbalance', JSON_OBJECT('RuleId', 'AR-008', 'AlarmType', 'QtyImbalance', 'IsEnabled', true, 'Severity', 'Error', 'NotifyRole', 'QA'), 'string');

-- ========================================
-- 9. 标记已种子化
-- ========================================
INSERT INTO `mes_kv_store` (`key`, `value`, `data_type`) VALUES
('mes:seeded', '1', 'string');

-- ========================================
-- 验证数据
-- ========================================
SELECT 
    '数据库初始化完成' AS message,
    COUNT(*) AS total_records
FROM `mes_kv_store`;

SELECT 
    data_type,
    COUNT(*) AS record_count
FROM `mes_kv_store`
GROUP BY data_type;
