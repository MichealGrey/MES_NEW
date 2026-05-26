#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
MES V3 模拟数据生成脚本
生成完整的SQL插入语句，覆盖多种业务场景
"""

import os
from datetime import datetime

OUTPUT_FILE = os.path.join(os.path.dirname(__file__), 'mes_mock_data.sql')

lines = []

def L(line=''):
    lines.append(line)

def sql_val(v):
    """Convert Python value to SQL literal. DATE_SUB/DATE_ADD functions are not quoted."""
    if v is None or v == 'None':
        return 'NULL'
    if isinstance(v, str) and ('DATE_SUB' in v or 'DATE_ADD' in v):
        return v
    if isinstance(v, str):
        return f"'{v}'"
    return str(v)

def write_sql():
    L('-- ============================================================')
    L('-- MES V3 模拟数据脚本（完整版）')
    L('-- 数据库: mes_prod')
    L(f'-- 生成日期: {datetime.now().strftime("%Y-%m-%d %H:%M:%S")}')
    L('-- ============================================================')
    L('')
    L('USE `mes_prod`;')
    L('')
    
    # 一、系统基础数据
    L('-- ============================================================')
    L('-- 一、系统基础数据')
    L('-- ============================================================')
    L('')
    
    L('INSERT INTO `sys_department` (`dept_id`, `dept_name`, `parent_id`, `manager_id`, `status`) VALUES')
    depts = [
        ('DEPT-PROD', '生产部', None, 'USR-001', 'Active'),
        ('DEPT-QA', '品质部', None, 'USR-002', 'Active'),
        ('DEPT-ENG', '工程部', None, 'USR-003', 'Active'),
        ('DEPT-WH', '仓储部', None, 'USR-004', 'Active'),
        ('DEPT-PM', '计划部', None, 'USR-013', 'Active'),
        ('DEPT-SAW', '切割班组', 'DEPT-PROD', 'USR-005', 'Active'),
        ('DEPT-DA', '贴片班组', 'DEPT-PROD', 'USR-006', 'Active'),
        ('DEPT-WB', '焊线班组', 'DEPT-PROD', 'USR-007', 'Active'),
        ('DEPT-MOLD', '塑封班组', 'DEPT-PROD', 'USR-008', 'Active'),
        ('DEPT-FT', '测试班组', 'DEPT-PROD', 'USR-009', 'Active'),
        ('DEPT-OQC', '出货检验班组', 'DEPT-QA', 'USR-014', 'Active'),
        ('DEPT-MAINT', '设备维护组', 'DEPT-ENG', 'USR-015', 'Active'),
        ('DEPT-PMC', '后固化班组', 'DEPT-PROD', 'USR-021', 'Active'),
        ('DEPT-LASER', '打标班组', 'DEPT-PROD', 'USR-022', 'Active'),
        ('DEPT-SING', '分条班组', 'DEPT-PROD', 'USR-023', 'Active'),
    ]
    for i, d in enumerate(depts):
        comma = ',' if i < len(depts) - 1 else ';'
        parent = f"'{d[2]}'" if d[2] else 'NULL'
        L(f"('{d[0]}', '{d[1]}', {parent}, '{d[3]}', '{d[4]}'){comma}")
    L('')
    
    L('INSERT INTO `sys_role` (`role_id`, `role_name`, `description`, `level`) VALUES')
    roles = [
        ('ROLE-ADMIN', '系统管理员', '系统最高权限', 3),
        ('ROLE-ENG', '工程师', '工程/工艺工程师', 2),
        ('ROLE-QA', '品质工程师', '品质检验与放行', 2),
        ('ROLE-SUPERVISOR', '班组长', '产线班组长', 1),
        ('ROLE-OPERATOR', '操作员', '产线操作员', 0),
        ('ROLE-PLANNER', '计划员', '生产计划排程', 1),
        ('ROLE-MANAGER', '部门经理', '部门管理', 2),
    ]
    for i, r in enumerate(roles):
        comma = ',' if i < len(roles) - 1 else ';'
        L(f"('{r[0]}', '{r[1]}', '{r[2]}', {r[3]}){comma}")
    L('')
    
    L('INSERT INTO `sys_user` (`user_id`, `user_name`, `role_id`, `dept_id`, `shift`, `is_active`) VALUES')
    users = [
        ('USR-001', '张经理', 'ROLE-ADMIN', 'DEPT-PROD', 'A', 1),
        ('USR-002', '李品质', 'ROLE-QA', 'DEPT-QA', 'A', 1),
        ('USR-003', '王工程', 'ROLE-ENG', 'DEPT-ENG', 'A', 1),
        ('USR-004', '赵仓储', 'ROLE-SUPERVISOR', 'DEPT-WH', 'A', 1),
        ('USR-005', '孙切割', 'ROLE-SUPERVISOR', 'DEPT-SAW', 'A', 1),
        ('USR-006', '周贴片', 'ROLE-SUPERVISOR', 'DEPT-DA', 'B', 1),
        ('USR-007', '吴焊线', 'ROLE-SUPERVISOR', 'DEPT-WB', 'A', 1),
        ('USR-008', '郑塑封', 'ROLE-SUPERVISOR', 'DEPT-MOLD', 'B', 1),
        ('USR-009', '陈测试', 'ROLE-SUPERVISOR', 'DEPT-FT', 'C', 1),
        ('USR-010', '刘操作员', 'ROLE-OPERATOR', 'DEPT-SAW', 'A', 1),
        ('USR-011', '黄操作员', 'ROLE-OPERATOR', 'DEPT-DA', 'A', 1),
        ('USR-012', '林操作员', 'ROLE-OPERATOR', 'DEPT-WB', 'B', 1),
        ('USR-013', '何计划', 'ROLE-PLANNER', 'DEPT-PM', 'A', 1),
        ('USR-014', '钱检验', 'ROLE-QA', 'DEPT-OQC', 'A', 1),
        ('USR-015', '马维护', 'ROLE-ENG', 'DEPT-MAINT', 'B', 1),
        ('USR-016', '杨操作员', 'ROLE-OPERATOR', 'DEPT-MOLD', 'A', 1),
        ('USR-017', '许操作员', 'ROLE-OPERATOR', 'DEPT-FT', 'B', 1),
        ('USR-018', '曹操作员', 'ROLE-OPERATOR', 'DEPT-SAW', 'B', 1),
        ('USR-019', '田操作员', 'ROLE-OPERATOR', 'DEPT-FT', 'A', 1),
        ('USR-020', '潘操作员', 'ROLE-OPERATOR', 'DEPT-OQC', 'A', 1),
        ('USR-021', '蒋固化', 'ROLE-SUPERVISOR', 'DEPT-PMC', 'C', 1),
        ('USR-022', '蔡打标', 'ROLE-SUPERVISOR', 'DEPT-LASER', 'A', 1),
        ('USR-023', '贾分条', 'ROLE-SUPERVISOR', 'DEPT-SING', 'B', 1),
        ('USR-024', '丁操作员', 'ROLE-OPERATOR', 'DEPT-PMC', 'C', 1),
        ('USR-025', '魏操作员', 'ROLE-OPERATOR', 'DEPT-LASER', 'A', 1),
        ('USR-026', '薛操作员', 'ROLE-OPERATOR', 'DEPT-SING', 'B', 1),
        ('USR-027', '叶经理', 'ROLE-MANAGER', 'DEPT-QA', 'A', 1),
        ('USR-028', '阎工程', 'ROLE-ENG', 'DEPT-ENG', 'B', 1),
    ]
    for i, u in enumerate(users):
        comma = ',' if i < len(users) - 1 else ';'
        L(f"('{u[0]}', '{u[1]}', '{u[2]}', '{u[3]}', '{u[4]}', {u[5]}){comma}")
    L('')
    
    L('INSERT INTO `sys_user_permission` (`user_id`, `permission_code`) VALUES')
    perms = [
        ('USR-001', 'admin.all'), ('USR-001', 'prod.wo.create'), ('USR-001', 'prod.lot.hold'),
        ('USR-001', 'prod.lot.release'), ('USR-001', 'prod.lot.scrap'), ('USR-001', 'prod.lot.rework'),
        ('USR-002', 'qa.inspection'), ('USR-002', 'qa.gate.release'), ('USR-002', 'prod.lot.hold'), ('USR-002', 'prod.lot.release'),
        ('USR-003', 'eng.route.manage'), ('USR-003', 'eng.recipe.manage'), ('USR-003', 'prod.lot.rework'), ('USR-003', 'prod.lot.scrap'),
        ('USR-004', 'wh.receive'), ('USR-004', 'wh.ship'), ('USR-004', 'prod.lot.pack'),
        ('USR-005', 'prod.trackin'), ('USR-005', 'prod.trackout'),
        ('USR-006', 'prod.trackin'), ('USR-006', 'prod.trackout'),
        ('USR-007', 'prod.trackin'), ('USR-007', 'prod.trackout'),
        ('USR-008', 'prod.trackin'), ('USR-008', 'prod.trackout'),
        ('USR-009', 'prod.trackin'), ('USR-009', 'prod.trackout'),
        ('USR-010', 'prod.trackin'), ('USR-010', 'prod.trackout'),
        ('USR-011', 'prod.trackin'), ('USR-011', 'prod.trackout'),
        ('USR-012', 'prod.trackin'), ('USR-012', 'prod.trackout'),
        ('USR-013', 'prod.wo.create'), ('USR-013', 'prod.wo.schedule'),
        ('USR-014', 'qa.oqc'), ('USR-014', 'qa.inspection'),
        ('USR-015', 'equip.maintenance'), ('USR-015', 'equip.status'),
        ('USR-016', 'prod.trackin'), ('USR-016', 'prod.trackout'),
        ('USR-017', 'prod.trackin'), ('USR-017', 'prod.trackout'),
        ('USR-018', 'prod.trackin'), ('USR-018', 'prod.trackout'),
        ('USR-019', 'prod.trackin'), ('USR-019', 'prod.trackout'),
        ('USR-020', 'prod.trackin'), ('USR-020', 'prod.trackout'),
        ('USR-021', 'prod.trackin'), ('USR-021', 'prod.trackout'),
        ('USR-022', 'prod.trackin'), ('USR-022', 'prod.trackout'),
        ('USR-023', 'prod.trackin'), ('USR-023', 'prod.trackout'),
        ('USR-024', 'prod.trackin'), ('USR-024', 'prod.trackout'),
        ('USR-025', 'prod.trackin'), ('USR-025', 'prod.trackout'),
        ('USR-026', 'prod.trackin'), ('USR-026', 'prod.trackout'),
        ('USR-027', 'qa.inspection'), ('USR-027', 'qa.gate.release'), ('USR-027', 'qa.mrb'),
        ('USR-028', 'eng.route.manage'), ('USR-028', 'eng.recipe.manage'),
    ]
    for i, p in enumerate(perms):
        comma = ',' if i < len(perms) - 1 else ';'
        L(f"('{p[0]}', '{p[1]}'){comma}")
    L('')
    
    # 二、主数据
    L('-- ============================================================')
    L('-- 二、主数据')
    L('-- ============================================================')
    L('')
    
    L('INSERT INTO `master_product` (`product_id`, `product_name`, `die_name`, `package_type`, `customer_id`, `customer_name`, `customer_pn`, `internal_pn`, `status`) VALUES')
    products = [
        ('PROD-QFN88', 'QFN-88 控制器', 'CTRL-2024', 'QFN', 'CUST-AUTO', '某汽车电子', 'AE-QFN88-001', 'PN-QFN88-STD', 'Active'),
        ('PROD-SOP16', 'SOP-16 电源IC', 'PWR-500', 'SOP', 'CUST-IND', '某工业客户', 'IND-SOP16-002', 'PN-SOP16-STD', 'Active'),
        ('PROD-BGA256', 'BGA-256 处理器', 'CPU-3000', 'BGA', 'CUST-AUTO', '某汽车电子', 'AE-BGA256-003', 'PN-BGA256-HI', 'Active'),
        ('PROD-QFN48', 'QFN-48 传感器', 'SENS-100', 'QFN', 'CUST-CON', '某消费电子', 'CE-QFN48-004', 'PN-QFN48-STD', 'Active'),
        ('PROD-SOP8', 'SOP-8 MOSFET', 'MOS-200', 'SOP', 'CUST-IND', '某工业客户', 'IND-SOP8-005', 'PN-SOP8-STD', 'Active'),
        ('PROD-BGA64', 'BGA-64 存储器', 'MEM-500', 'BGA', 'CUST-AUTO', '某汽车电子', 'AE-BGA64-006', 'PN-BGA64-STD', 'Active'),
        ('PROD-QFN64', 'QFN-64 驱动器', 'DRV-300', 'QFN', 'CUST-AUTO', '某汽车电子', 'AE-QFN64-007', 'PN-QFN64-STD', 'Active'),
        ('PROD-SOP14', 'SOP-14 运放', 'OP-100', 'SOP', 'CUST-CON', '某消费电子', 'CE-SOP14-008', 'PN-SOP14-STD', 'Active'),
    ]
    for i, p in enumerate(products):
        comma = ',' if i < len(products) - 1 else ';'
        L(f"('{p[0]}', '{p[1]}', '{p[2]}', '{p[3]}', '{p[4]}', '{p[5]}', '{p[6]}', '{p[7]}', '{p[8]}'){comma}")
    L('')
    
    L('INSERT INTO `master_route` (`route_id`, `route_name`, `route_version`, `product_id`, `package_type`, `is_active`, `is_approved`, `approved_by`, `approved_at`) VALUES')
    routes = [
        ('QFN-STD:2.0', 'QFN 标准路线', '2.0', 'PROD-QFN88', 'QFN', 1, 1, 'USR-003', 'DATE_SUB(NOW(), INTERVAL 30 DAY)'),
        ('SOP-STD:1.0', 'SOP 标准路线', '1.0', 'PROD-SOP16', 'SOP', 1, 1, 'USR-003', 'DATE_SUB(NOW(), INTERVAL 60 DAY)'),
        ('BGA-STD:1.0', 'BGA 标准路线', '1.0', 'PROD-BGA256', 'BGA', 1, 1, 'USR-003', 'DATE_SUB(NOW(), INTERVAL 45 DAY)'),
        ('QFN48-STD:1.0', 'QFN-48 标准路线', '1.0', 'PROD-QFN48', 'QFN', 1, 1, 'USR-003', 'DATE_SUB(NOW(), INTERVAL 20 DAY)'),
        ('SOP8-STD:1.0', 'SOP-8 标准路线', '1.0', 'PROD-SOP8', 'SOP', 1, 1, 'USR-003', 'DATE_SUB(NOW(), INTERVAL 90 DAY)'),
        ('BGA64-STD:1.0', 'BGA-64 标准路线', '1.0', 'PROD-BGA64', 'BGA', 1, 1, 'USR-003', 'DATE_SUB(NOW(), INTERVAL 15 DAY)'),
        ('QFN64-STD:1.0', 'QFN-64 标准路线', '1.0', 'PROD-QFN64', 'QFN', 1, 1, 'USR-003', 'DATE_SUB(NOW(), INTERVAL 25 DAY)'),
        ('SOP14-STD:1.0', 'SOP-14 标准路线', '1.0', 'PROD-SOP14', 'SOP', 1, 1, 'USR-003', 'DATE_SUB(NOW(), INTERVAL 40 DAY)'),
        ('RW-DA:1.0', 'DieAttach 重工路线', '1.0', 'PROD-QFN88', 'QFN', 1, 1, 'USR-003', 'DATE_SUB(NOW(), INTERVAL 15 DAY)'),
        ('RW-WB:1.0', 'WireBond 重工路线', '1.0', 'PROD-QFN88', 'QFN', 1, 1, 'USR-003', 'DATE_SUB(NOW(), INTERVAL 15 DAY)'),
        ('RW-MOLD:1.0', 'Mold 重工路线', '1.0', 'PROD-QFN88', 'QFN', 1, 1, 'USR-003', 'DATE_SUB(NOW(), INTERVAL 10 DAY)'),
    ]
    for i, r in enumerate(routes):
        comma = ',' if i < len(routes) - 1 else ';'
        L(f"('{r[0]}', '{r[1]}', '{r[2]}', '{r[3]}', '{r[4]}', {r[5]}, {r[6]}, '{r[7]}', {r[8]}){comma}")
    L('')
    
    L('INSERT INTO `master_route_step` (`route_id`, `step_seq`, `step_code`, `step_name`, `equipment_group`, `is_rework`) VALUES')
    steps = [
        ('QFN-STD:2.0', 1, 'SAW', '晶圆切割', 'SAW', 0), ('QFN-STD:2.0', 2, 'DA', '芯片贴装', 'DA', 0),
        ('QFN-STD:2.0', 3, 'CURE', '固化', 'CURE', 0), ('QFN-STD:2.0', 4, 'WB', '焊线', 'WB', 0),
        ('QFN-STD:2.0', 5, 'MOLD', '塑封', 'MOLD', 0), ('QFN-STD:2.0', 6, 'PMC', '后固化', 'PMC', 0),
        ('QFN-STD:2.0', 7, 'MARK', '激光打标', 'LASER', 0), ('QFN-STD:2.0', 8, 'SING', '切割分条', 'SING', 0),
        ('QFN-STD:2.0', 9, 'FT', '最终测试', 'TEST', 0), ('QFN-STD:2.0', 10, 'OQC', '出货检验', 'OQC', 0),
        ('QFN-STD:2.0', 11, 'PACK', '包装入库', 'PACK', 0),
        ('SOP-STD:1.0', 1, 'SAW', '晶圆切割', 'SAW', 0), ('SOP-STD:1.0', 2, 'DA', '芯片贴装', 'DA', 0),
        ('SOP-STD:1.0', 3, 'WB', '焊线', 'WB', 0), ('SOP-STD:1.0', 4, 'MOLD', '塑封', 'MOLD', 0),
        ('SOP-STD:1.0', 5, 'MARK', '激光打标', 'LASER', 0), ('SOP-STD:1.0', 6, 'SING', '切割分条', 'SING', 0),
        ('SOP-STD:1.0', 7, 'FT', '最终测试', 'TEST', 0), ('SOP-STD:1.0', 8, 'OQC', '出货检验', 'OQC', 0),
        ('SOP-STD:1.0', 9, 'PACK', '包装入库', 'PACK', 0),
        ('BGA-STD:1.0', 1, 'SAW', '晶圆切割', 'SAW', 0), ('BGA-STD:1.0', 2, 'DA', '芯片贴装', 'DA', 0),
        ('BGA-STD:1.0', 3, 'CURE', '固化', 'CURE', 0), ('BGA-STD:1.0', 4, 'WB', '焊线', 'WB', 0),
        ('BGA-STD:1.0', 5, 'MOLD', '塑封', 'MOLD', 0), ('BGA-STD:1.0', 6, 'PMC', '后固化', 'PMC', 0),
        ('BGA-STD:1.0', 7, 'BALL', '植球', 'BALL', 0), ('BGA-STD:1.0', 8, 'REFLOW', '回流焊', 'REFLOW', 0),
        ('BGA-STD:1.0', 9, 'MARK', '激光打标', 'LASER', 0), ('BGA-STD:1.0', 10, 'FT', '最终测试', 'TEST', 0),
        ('BGA-STD:1.0', 11, 'OQC', '出货检验', 'OQC', 0), ('BGA-STD:1.0', 12, 'PACK', '包装入库', 'PACK', 0),
        ('QFN48-STD:1.0', 1, 'SAW', '晶圆切割', 'SAW', 0), ('QFN48-STD:1.0', 2, 'DA', '芯片贴装', 'DA', 0),
        ('QFN48-STD:1.0', 3, 'WB', '焊线', 'WB', 0), ('QFN48-STD:1.0', 4, 'MOLD', '塑封', 'MOLD', 0),
        ('QFN48-STD:1.0', 5, 'PMC', '后固化', 'PMC', 0), ('QFN48-STD:1.0', 6, 'MARK', '激光打标', 'LASER', 0),
        ('QFN48-STD:1.0', 7, 'SING', '切割分条', 'SING', 0), ('QFN48-STD:1.0', 8, 'FT', '最终测试', 'TEST', 0),
        ('QFN48-STD:1.0', 9, 'OQC', '出货检验', 'OQC', 0), ('QFN48-STD:1.0', 10, 'PACK', '包装入库', 'PACK', 0),
        ('SOP8-STD:1.0', 1, 'DA', '芯片贴装', 'DA', 0), ('SOP8-STD:1.0', 2, 'WB', '焊线', 'WB', 0),
        ('SOP8-STD:1.0', 3, 'MOLD', '塑封', 'MOLD', 0), ('SOP8-STD:1.0', 4, 'MARK', '激光打标', 'LASER', 0),
        ('SOP8-STD:1.0', 5, 'SING', '切割分条', 'SING', 0), ('SOP8-STD:1.0', 6, 'FT', '最终测试', 'TEST', 0),
        ('SOP8-STD:1.0', 7, 'OQC', '出货检验', 'OQC', 0), ('SOP8-STD:1.0', 8, 'PACK', '包装入库', 'PACK', 0),
        ('BGA64-STD:1.0', 1, 'SAW', '晶圆切割', 'SAW', 0), ('BGA64-STD:1.0', 2, 'DA', '芯片贴装', 'DA', 0),
        ('BGA64-STD:1.0', 3, 'CURE', '固化', 'CURE', 0), ('BGA64-STD:1.0', 4, 'WB', '焊线', 'WB', 0),
        ('BGA64-STD:1.0', 5, 'MOLD', '塑封', 'MOLD', 0), ('BGA64-STD:1.0', 6, 'PMC', '后固化', 'PMC', 0),
        ('BGA64-STD:1.0', 7, 'BALL', '植球', 'BALL', 0), ('BGA64-STD:1.0', 8, 'FT', '最终测试', 'TEST', 0),
        ('BGA64-STD:1.0', 9, 'OQC', '出货检验', 'OQC', 0), ('BGA64-STD:1.0', 10, 'PACK', '包装入库', 'PACK', 0),
        ('QFN64-STD:1.0', 1, 'SAW', '晶圆切割', 'SAW', 0), ('QFN64-STD:1.0', 2, 'DA', '芯片贴装', 'DA', 0),
        ('QFN64-STD:1.0', 3, 'CURE', '固化', 'CURE', 0), ('QFN64-STD:1.0', 4, 'WB', '焊线', 'WB', 0),
        ('QFN64-STD:1.0', 5, 'MOLD', '塑封', 'MOLD', 0), ('QFN64-STD:1.0', 6, 'PMC', '后固化', 'PMC', 0),
        ('QFN64-STD:1.0', 7, 'MARK', '激光打标', 'LASER', 0), ('QFN64-STD:1.0', 8, 'SING', '切割分条', 'SING', 0),
        ('QFN64-STD:1.0', 9, 'FT', '最终测试', 'TEST', 0), ('QFN64-STD:1.0', 10, 'OQC', '出货检验', 'OQC', 0),
        ('QFN64-STD:1.0', 11, 'PACK', '包装入库', 'PACK', 0),
        ('SOP14-STD:1.0', 1, 'DA', '芯片贴装', 'DA', 0), ('SOP14-STD:1.0', 2, 'WB', '焊线', 'WB', 0),
        ('SOP14-STD:1.0', 3, 'MOLD', '塑封', 'MOLD', 0), ('SOP14-STD:1.0', 4, 'MARK', '激光打标', 'LASER', 0),
        ('SOP14-STD:1.0', 5, 'SING', '切割分条', 'SING', 0), ('SOP14-STD:1.0', 6, 'FT', '最终测试', 'TEST', 0),
        ('SOP14-STD:1.0', 7, 'OQC', '出货检验', 'OQC', 0), ('SOP14-STD:1.0', 8, 'PACK', '包装入库', 'PACK', 0),
        ('RW-DA:1.0', 1, 'DEBOND-DA', '去胶', 'DEBOND', 1), ('RW-DA:1.0', 2, 'CLEAN-DA', '清洗', 'CLEAN', 1),
        ('RW-DA:1.0', 3, 'DA', '重新贴片', 'DA', 1),
        ('RW-WB:1.0', 1, 'DEBOND-WB', '去线', 'DEBOND', 1), ('RW-WB:1.0', 2, 'CLEAN-WB', '清洗', 'CLEAN', 1),
        ('RW-WB:1.0', 3, 'WB', '重新焊线', 'WB', 1),
        ('RW-MOLD:1.0', 1, 'DECAP', '去封装', 'DECAP', 1), ('RW-MOLD:1.0', 2, 'CLEAN-MOLD', '清洗', 'CLEAN', 1),
        ('RW-MOLD:1.0', 3, 'MOLD', '重新塑封', 'MOLD', 1),
    ]
    for i, s in enumerate(steps):
        comma = ',' if i < len(steps) - 1 else ';'
        L(f"('{s[0]}', {s[1]}, '{s[2]}', '{s[3]}', '{s[4]}', {s[5]}){comma}")
    L('')
    
    L('INSERT INTO `master_equipment` (`equipment_id`, `equipment_name`, `equipment_group`, `equipment_type`, `status`, `location`, `responsible_person`, `last_maintenance_date`, `maintenance_interval_hours`, `running_hours`) VALUES')
    equipments = [
        ('SAW-001', '切割机 #1', 'SAW', 'DicingSaw', 'Available', 'Line A-01', '张工', 'DATE_SUB(NOW(), INTERVAL 7 DAY)', 500, 120),
        ('SAW-002', '切割机 #2', 'SAW', 'DicingSaw', 'Running', 'Line A-02', '张工', 'DATE_SUB(NOW(), INTERVAL 3 DAY)', 500, 350),
        ('SAW-003', '切割机 #3', 'SAW', 'DicingSaw', 'Available', 'Line A-03', '张工', 'DATE_SUB(NOW(), INTERVAL 10 DAY)', 500, 80),
        ('SAW-004', '切割机 #4', 'SAW', 'DicingSaw', 'Maintenance', 'Line A-04', '张工', 'DATE_SUB(NOW(), INTERVAL 1 DAY)', 500, 490),
        ('DA-001', '贴片机 #1', 'DA', 'DieBonder', 'Available', 'Line A-05', '周工', 'DATE_SUB(NOW(), INTERVAL 5 DAY)', 400, 200),
        ('DA-002', '贴片机 #2', 'DA', 'DieBonder', 'Running', 'Line A-06', '周工', 'DATE_SUB(NOW(), INTERVAL 1 DAY)', 400, 380),
        ('DA-003', '贴片机 #3', 'DA', 'DieBonder', 'Available', 'Line A-07', '周工', 'DATE_SUB(NOW(), INTERVAL 8 DAY)', 400, 150),
        ('DA-004', '贴片机 #4', 'DA', 'DieBonder', 'Offline', 'Line A-08', '周工', 'DATE_SUB(NOW(), INTERVAL 20 DAY)', 400, 395),
        ('WB-001', '焊线机 #1', 'WB', 'WireBonder', 'Available', 'Line B-01', '吴工', 'DATE_SUB(NOW(), INTERVAL 10 DAY)', 600, 150),
        ('WB-002', '焊线机 #2', 'WB', 'WireBonder', 'Running', 'Line B-02', '吴工', 'DATE_SUB(NOW(), INTERVAL 2 DAY)', 600, 420),
        ('WB-003', '焊线机 #3', 'WB', 'WireBonder', 'Available', 'Line B-03', '吴工', 'DATE_SUB(NOW(), INTERVAL 15 DAY)', 600, 50),
        ('WB-004', '焊线机 #4', 'WB', 'WireBonder', 'Offline', 'Line B-04', '吴工', 'DATE_SUB(NOW(), INTERVAL 30 DAY)', 600, 580),
        ('WB-005', '焊线机 #5', 'WB', 'WireBonder', 'Available', 'Line B-05', '吴工', 'DATE_SUB(NOW(), INTERVAL 5 DAY)', 600, 280),
        ('MOLD-001', '塑封机 #1', 'MOLD', 'MoldingPress', 'Available', 'Line B-06', '郑工', 'DATE_SUB(NOW(), INTERVAL 14 DAY)', 800, 300),
        ('MOLD-002', '塑封机 #2', 'MOLD', 'MoldingPress', 'Running', 'Line B-07', '郑工', 'DATE_SUB(NOW(), INTERVAL 8 DAY)', 800, 500),
        ('MOLD-003', '塑封机 #3', 'MOLD', 'MoldingPress', 'Available', 'Line B-08', '郑工', 'DATE_SUB(NOW(), INTERVAL 2 DAY)', 800, 720),
        ('LASER-001', '激光打标机 #1', 'LASER', 'LaserMark', 'Available', 'Line C-01', '陈工', 'DATE_SUB(NOW(), INTERVAL 20 DAY)', 1000, 100),
        ('LASER-002', '激光打标机 #2', 'LASER', 'LaserMark', 'Running', 'Line C-02', '陈工', 'DATE_SUB(NOW(), INTERVAL 12 DAY)', 1000, 400),
        ('SING-001', '切割分条机 #1', 'SING', 'TrimForm', 'Available', 'Line C-03', '陈工', 'DATE_SUB(NOW(), INTERVAL 6 DAY)', 500, 250),
        ('SING-002', '切割分条机 #2', 'SING', 'TrimForm', 'Running', 'Line C-04', '陈工', 'DATE_SUB(NOW(), INTERVAL 1 DAY)', 500, 480),
        ('FT-001', '测试机 #1', 'TEST', 'TestHandler', 'Available', 'Line C-05', '陈工', 'DATE_SUB(NOW(), INTERVAL 4 DAY)', 300, 180),
        ('FT-002', '测试机 #2', 'TEST', 'TestHandler', 'Running', 'Line C-06', '陈工', 'DATE_SUB(NOW(), INTERVAL 1 DAY)', 300, 280),
        ('FT-003', '测试机 #3', 'TEST', 'TestHandler', 'Available', 'Line C-07', '陈工', 'DATE_SUB(NOW(), INTERVAL 6 DAY)', 300, 90),
        ('FT-004', '测试机 #4', 'TEST', 'TestHandler', 'Maintenance', 'Line C-08', '陈工', 'DATE_SUB(NOW(), INTERVAL 1 DAY)', 300, 295),
        ('OQC-001', '出货检验台 #1', 'OQC', 'AOI', 'Available', 'Line D-01', '李品质', 'DATE_SUB(NOW(), INTERVAL 30 DAY)', 2000, 50),
        ('OQC-002', '出货检验台 #2', 'OQC', 'AOI', 'Available', 'Line D-02', '钱检验', 'DATE_SUB(NOW(), INTERVAL 25 DAY)', 2000, 80),
        ('BALL-001', '植球机 #1', 'BALL', 'BallMount', 'Available', 'Line E-01', '马维护', 'DATE_SUB(NOW(), INTERVAL 18 DAY)', 700, 200),
        ('BALL-002', '植球机 #2', 'BALL', 'BallMount', 'Running', 'Line E-02', '马维护', 'DATE_SUB(NOW(), INTERVAL 5 DAY)', 700, 450),
        ('REFLOW-001', '回流焊 #1', 'REFLOW', 'ReflowOven', 'Available', 'Line E-03', '马维护', 'DATE_SUB(NOW(), INTERVAL 22 DAY)', 900, 150),
        ('REFLOW-002', '回流焊 #2', 'REFLOW', 'ReflowOven', 'Running', 'Line E-04', '马维护', 'DATE_SUB(NOW(), INTERVAL 10 DAY)', 900, 600),
        ('CURE-001', '固化炉 #1', 'CURE', 'CuringOven', 'Available', 'Line F-01', '蒋固化', 'DATE_SUB(NOW(), INTERVAL 15 DAY)', 1000, 200),
        ('CURE-002', '固化炉 #2', 'CURE', 'CuringOven', 'Running', 'Line F-02', '蒋固化', 'DATE_SUB(NOW(), INTERVAL 8 DAY)', 1000, 500),
        ('PMC-001', '后固化炉 #1', 'PMC', 'PostCureOven', 'Available', 'Line F-03', '蒋固化', 'DATE_SUB(NOW(), INTERVAL 12 DAY)', 1200, 300),
        ('PMC-002', '后固化炉 #2', 'PMC', 'PostCureOven', 'Running', 'Line F-04', '蒋固化', 'DATE_SUB(NOW(), INTERVAL 3 DAY)', 1200, 800),
    ]
    for i, e in enumerate(equipments):
        comma = ',' if i < len(equipments) - 1 else ';'
        L(f"('{e[0]}', '{e[1]}', '{e[2]}', '{e[3]}', '{e[4]}', '{e[5]}', '{e[6]}', {e[7]}, {e[8]}, {e[9]}){comma}")
    L('')
    
    L('INSERT INTO `master_equipment_route` (`equipment_id`, `route_id`) VALUES')
    equip_routes = [
        ('SAW-001', 'QFN-STD:2.0'), ('SAW-002', 'QFN-STD:2.0'), ('SAW-003', 'QFN-STD:2.0'),
        ('DA-001', 'QFN-STD:2.0'), ('DA-002', 'QFN-STD:2.0'), ('DA-003', 'QFN-STD:2.0'),
        ('WB-001', 'QFN-STD:2.0'), ('WB-002', 'QFN-STD:2.0'), ('WB-003', 'QFN-STD:2.0'),
        ('MOLD-001', 'QFN-STD:2.0'), ('MOLD-002', 'QFN-STD:2.0'), ('MOLD-003', 'QFN-STD:2.0'),
        ('LASER-001', 'QFN-STD:2.0'), ('LASER-002', 'QFN-STD:2.0'),
        ('SING-001', 'QFN-STD:2.0'), ('SING-002', 'QFN-STD:2.0'),
        ('FT-001', 'QFN-STD:2.0'), ('FT-002', 'QFN-STD:2.0'), ('FT-003', 'QFN-STD:2.0'),
        ('OQC-001', 'QFN-STD:2.0'), ('OQC-002', 'QFN-STD:2.0'),
        ('SAW-001', 'SOP-STD:1.0'), ('SAW-002', 'SOP-STD:1.0'),
        ('DA-001', 'SOP-STD:1.0'), ('DA-003', 'SOP-STD:1.0'),
        ('WB-001', 'SOP-STD:1.0'), ('WB-002', 'SOP-STD:1.0'),
        ('MOLD-001', 'SOP-STD:1.0'), ('MOLD-002', 'SOP-STD:1.0'),
        ('LASER-001', 'SOP-STD:1.0'), ('SING-001', 'SOP-STD:1.0'),
        ('FT-001', 'SOP-STD:1.0'), ('FT-003', 'SOP-STD:1.0'), ('OQC-001', 'SOP-STD:1.0'),
        ('SAW-001', 'BGA-STD:1.0'), ('SAW-003', 'BGA-STD:1.0'),
        ('DA-001', 'BGA-STD:1.0'), ('DA-002', 'BGA-STD:1.0'),
        ('WB-001', 'BGA-STD:1.0'), ('WB-003', 'BGA-STD:1.0'),
        ('MOLD-001', 'BGA-STD:1.0'), ('MOLD-003', 'BGA-STD:1.0'),
        ('BALL-001', 'BGA-STD:1.0'), ('REFLOW-001', 'BGA-STD:1.0'),
        ('LASER-001', 'BGA-STD:1.0'), ('LASER-002', 'BGA-STD:1.0'),
        ('FT-001', 'BGA-STD:1.0'), ('FT-002', 'BGA-STD:1.0'),
        ('OQC-001', 'BGA-STD:1.0'), ('OQC-002', 'BGA-STD:1.0'),
        ('DA-001', 'RW-DA:1.0'), ('DA-002', 'RW-DA:1.0'), ('DA-003', 'RW-DA:1.0'),
        ('WB-001', 'RW-WB:1.0'), ('WB-002', 'RW-WB:1.0'), ('WB-003', 'RW-WB:1.0'),
        ('MOLD-001', 'RW-MOLD:1.0'), ('MOLD-002', 'RW-MOLD:1.0'), ('MOLD-003', 'RW-MOLD:1.0'),
        ('SAW-001', 'QFN48-STD:1.0'), ('SAW-002', 'QFN48-STD:1.0'),
        ('DA-001', 'QFN48-STD:1.0'), ('DA-003', 'QFN48-STD:1.0'),
        ('WB-001', 'QFN48-STD:1.0'), ('WB-002', 'QFN48-STD:1.0'),
        ('MOLD-001', 'QFN48-STD:1.0'), ('MOLD-002', 'QFN48-STD:1.0'),
        ('LASER-001', 'QFN48-STD:1.0'), ('SING-001', 'QFN48-STD:1.0'),
        ('FT-001', 'QFN48-STD:1.0'), ('FT-003', 'QFN48-STD:1.0'),
        ('SAW-001', 'BGA64-STD:1.0'), ('SAW-003', 'BGA64-STD:1.0'),
        ('DA-001', 'BGA64-STD:1.0'), ('DA-002', 'BGA64-STD:1.0'),
        ('WB-001', 'BGA64-STD:1.0'), ('WB-003', 'BGA64-STD:1.0'),
        ('MOLD-001', 'BGA64-STD:1.0'), ('MOLD-003', 'BGA64-STD:1.0'),
        ('BALL-001', 'BGA64-STD:1.0'), ('FT-001', 'BGA64-STD:1.0'),
        ('OQC-001', 'BGA64-STD:1.0'),
        ('SAW-001', 'QFN64-STD:1.0'), ('SAW-002', 'QFN64-STD:1.0'),
        ('DA-001', 'QFN64-STD:1.0'), ('DA-003', 'QFN64-STD:1.0'),
        ('WB-001', 'QFN64-STD:1.0'), ('WB-002', 'QFN64-STD:1.0'),
        ('MOLD-001', 'QFN64-STD:1.0'), ('MOLD-002', 'QFN64-STD:1.0'),
        ('LASER-001', 'QFN64-STD:1.0'), ('SING-001', 'QFN64-STD:1.0'),
        ('FT-001', 'QFN64-STD:1.0'), ('FT-002', 'QFN64-STD:1.0'),
        ('OQC-001', 'QFN64-STD:1.0'),
        ('DA-001', 'SOP8-STD:1.0'), ('DA-003', 'SOP8-STD:1.0'),
        ('WB-001', 'SOP8-STD:1.0'), ('WB-002', 'SOP8-STD:1.0'),
        ('MOLD-001', 'SOP8-STD:1.0'), ('MOLD-002', 'SOP8-STD:1.0'),
        ('LASER-001', 'SOP8-STD:1.0'), ('SING-001', 'SOP8-STD:1.0'),
        ('FT-001', 'SOP8-STD:1.0'), ('FT-003', 'SOP8-STD:1.0'),
        ('OQC-001', 'SOP8-STD:1.0'),
        ('DA-001', 'SOP14-STD:1.0'), ('DA-003', 'SOP14-STD:1.0'),
        ('WB-001', 'SOP14-STD:1.0'), ('WB-002', 'SOP14-STD:1.0'),
        ('MOLD-001', 'SOP14-STD:1.0'), ('MOLD-002', 'SOP14-STD:1.0'),
        ('LASER-001', 'SOP14-STD:1.0'), ('SING-001', 'SOP14-STD:1.0'),
        ('FT-001', 'SOP14-STD:1.0'), ('FT-003', 'SOP14-STD:1.0'),
        ('OQC-001', 'SOP14-STD:1.0'),
    ]
    for i, er in enumerate(equip_routes):
        comma = ',' if i < len(equip_routes) - 1 else ';'
        L(f"('{er[0]}', '{er[1]}'){comma}")
    L('')
    
    L('INSERT INTO `master_carrier` (`carrier_id`, `carrier_type`, `status`, `capacity`, `use_count`, `max_use_count`, `location`) VALUES')
    carriers = [
        ('CARRIER-TRAY-001', 'Tray', 'Available', 500, 80, 1000, 'WH-A-01'),
        ('CARRIER-TRAY-002', 'Tray', 'Available', 500, 150, 1000, 'WH-A-02'),
        ('CARRIER-TRAY-003', 'Tray', 'InUse', 500, 200, 1000, 'Line A-05'),
        ('CARRIER-TRAY-004', 'Tray', 'Available', 500, 50, 1000, 'WH-A-03'),
        ('CARRIER-TRAY-005', 'Tray', 'Available', 500, 300, 1000, 'WH-A-04'),
        ('CARRIER-LEAD-001', 'LeadFrame', 'Available', 1000, 120, 2000, 'WH-B-01'),
        ('CARRIER-LEAD-002', 'LeadFrame', 'InUse', 1000, 250, 2000, 'Line B-01'),
        ('CARRIER-LEAD-003', 'LeadFrame', 'Available', 1000, 80, 2000, 'WH-B-02'),
        ('CARRIER-LEAD-004', 'LeadFrame', 'Available', 1000, 400, 2000, 'WH-B-03'),
        ('CARRIER-LEAD-005', 'LeadFrame', 'Available', 1000, 180, 2000, 'WH-B-04'),
        ('CARRIER-MAG-001', 'Magazine', 'Available', 200, 60, 500, 'WH-C-01'),
        ('CARRIER-MAG-002', 'Magazine', 'InUse', 200, 100, 500, 'Line C-05'),
        ('CARRIER-MAG-003', 'Magazine', 'Available', 200, 30, 500, 'WH-C-02'),
        ('CARRIER-MAG-004', 'Magazine', 'Available', 200, 150, 500, 'WH-C-03'),
        ('CARRIER-MAG-005', 'Magazine', 'Available', 200, 90, 500, 'WH-C-04'),
    ]
    for i, c in enumerate(carriers):
        comma = ',' if i < len(carriers) - 1 else ';'
        L(f"('{c[0]}', '{c[1]}', '{c[2]}', {c[3]}, {c[4]}, {c[5]}, '{c[6]}'){comma}")
    L('')
    
    L('-- master_recipe 数据已插入，跳过')
    L('')
    
    L('INSERT INTO `master_yield_rule` (`rule_id`, `route_id`, `step_code`, `yield_threshold`, `action_type`, `notify_role`, `is_active`) VALUES')
    yield_rules = [
        ('YR-QFN-SAW', 'QFN-STD:2.0', 'SAW', 99.50, 'AutoHold', 'QA', 1),
        ('YR-QFN-DA', 'QFN-STD:2.0', 'DA', 99.00, 'AutoHold', 'QA', 1),
        ('YR-QFN-WB', 'QFN-STD:2.0', 'WB', 98.50, 'AutoHold', 'QA', 1),
        ('YR-QFN-MOLD', 'QFN-STD:2.0', 'MOLD', 99.00, 'AutoHold', 'QA', 1),
        ('YR-QFN-FT', 'QFN-STD:2.0', 'FT', 95.00, 'AutoHold', 'QA', 1),
        ('YR-SOP-SAW', 'SOP-STD:1.0', 'SAW', 99.50, 'AutoHold', 'QA', 1),
        ('YR-SOP-DA', 'SOP-STD:1.0', 'DA', 99.00, 'AutoHold', 'QA', 1),
        ('YR-SOP-WB', 'SOP-STD:1.0', 'WB', 98.50, 'AutoHold', 'QA', 1),
        ('YR-SOP-FT', 'SOP-STD:1.0', 'FT', 96.00, 'AutoHold', 'QA', 1),
        ('YR-BGA-SAW', 'BGA-STD:1.0', 'SAW', 99.80, 'AutoHold', 'QA', 1),
        ('YR-BGA-DA', 'BGA-STD:1.0', 'DA', 99.50, 'AutoHold', 'QA', 1),
        ('YR-BGA-WB', 'BGA-STD:1.0', 'WB', 99.00, 'AutoHold', 'QA', 1),
        ('YR-BGA-FT', 'BGA-STD:1.0', 'FT', 94.00, 'AutoHold', 'QA', 1),
        ('YR-QFN48-FT', 'QFN48-STD:1.0', 'FT', 95.50, 'AutoHold', 'QA', 1),
        ('YR-BGA64-FT', 'BGA64-STD:1.0', 'FT', 95.00, 'AutoHold', 'QA', 1),
        ('YR-QFN64-FT', 'QFN64-STD:1.0', 'FT', 95.50, 'AutoHold', 'QA', 1),
    ]
    for i, y in enumerate(yield_rules):
        comma = ',' if i < len(yield_rules) - 1 else ';'
        L(f"('{y[0]}', '{y[1]}', '{y[2]}', {y[3]}, '{y[4]}', '{y[5]}', {y[6]}){comma}")
    L('')
    
    # 三、生产执行数据
    L('-- ============================================================')
    L('-- 三、生产执行数据')
    L('-- ============================================================')
    L('')
    
    L('INSERT INTO `prod_work_order` (`order_id`, `product_id`, `product_name`, `route_id`, `route_name`, `die_name`, `package_type`, `planned_qty`, `completed_qty`, `wafer_qty`, `unit_qty`, `customer_id`, `customer_name`, `customer_pn`, `internal_pn`, `priority`, `status`, `creator`, `planned_start_date`, `planned_end_date`, `actual_start_date`, `actual_end_date`, `target_cp_yield`, `target_ft_yield`, `remark`) VALUES')
    orders = [
        ("WO-2026001", "PROD-QFN88", "QFN-88 控制器", "QFN-STD:2.0", "QFN 标准路线", "CTRL-2024", "QFN", 20000, 8000, 4, 20000, "CUST-AUTO", "某汽车电子", "AE-QFN88-001", "PN-QFN88-STD", "High", "Processing", "USR-013", "DATE_SUB(NOW(), INTERVAL 5 DAY)", "DATE_ADD(NOW(), INTERVAL 10 DAY)", "DATE_SUB(NOW(), INTERVAL 4 DAY)", None, 99.00, 98.00, "汽车电子紧急订单"),
        ("WO-2026002", "PROD-SOP16", "SOP-16 电源IC", "SOP-STD:1.0", "SOP 标准路线", "PWR-500", "SOP", 15000, 15000, 3, 15000, "CUST-IND", "某工业客户", "IND-SOP16-002", "PN-SOP16-STD", "Normal", "Completed", "USR-013", "DATE_SUB(NOW(), INTERVAL 15 DAY)", "DATE_SUB(NOW(), INTERVAL 2 DAY)", "DATE_SUB(NOW(), INTERVAL 14 DAY)", "DATE_SUB(NOW(), INTERVAL 2 DAY)", 99.00, 96.00, "已完成工单"),
        ("WO-2026003", "PROD-BGA256", "BGA-256 处理器", "BGA-STD:1.0", "BGA 标准路线", "CPU-3000", "BGA", 5000, 2000, 2, 5000, "CUST-AUTO", "某汽车电子", "AE-BGA256-003", "PN-BGA256-HI", "High", "Processing", "USR-013", "DATE_SUB(NOW(), INTERVAL 3 DAY)", "DATE_ADD(NOW(), INTERVAL 12 DAY)", "DATE_SUB(NOW(), INTERVAL 2 DAY)", None, 99.50, 94.00, "高端处理器订单"),
        ("WO-2026004", "PROD-QFN48", "QFN-48 传感器", "QFN48-STD:1.0", "QFN-48 标准路线", "SENS-100", "QFN", 30000, 0, 6, 30000, "CUST-CON", "某消费电子", "CE-QFN48-004", "PN-QFN48-STD", "Normal", "Created", "USR-013", "DATE_ADD(NOW(), INTERVAL 2 DAY)", "DATE_ADD(NOW(), INTERVAL 15 DAY)", None, None, 99.00, 95.50, "待开始工单"),
        ("WO-2026005", "PROD-SOP8", "SOP-8 MOSFET", "SOP8-STD:1.0", "SOP-8 标准路线", "MOS-200", "SOP", 50000, 25000, 10, 50000, "CUST-IND", "某工业客户", "IND-SOP8-005", "PN-SOP8-STD", "Low", "Processing", "USR-013", "DATE_SUB(NOW(), INTERVAL 8 DAY)", "DATE_ADD(NOW(), INTERVAL 5 DAY)", "DATE_SUB(NOW(), INTERVAL 7 DAY)", None, 99.00, 96.00, "大批量MOSFET订单"),
        ("WO-2026006", "PROD-BGA64", "BGA-64 存储器", "BGA64-STD:1.0", "BGA-64 标准路线", "MEM-500", "BGA", 10000, 6000, 4, 10000, "CUST-AUTO", "某汽车电子", "AE-BGA64-006", "PN-BGA64-STD", "Normal", "Processing", "USR-013", "DATE_SUB(NOW(), INTERVAL 6 DAY)", "DATE_ADD(NOW(), INTERVAL 8 DAY)", "DATE_SUB(NOW(), INTERVAL 5 DAY)", None, 99.00, 95.00, "存储器订单"),
        ("WO-2026007", "PROD-QFN64", "QFN-64 驱动器", "QFN64-STD:1.0", "QFN-64 标准路线", "DRV-300", "QFN", 25000, 12000, 5, 25000, "CUST-AUTO", "某汽车电子", "AE-QFN64-007", "PN-QFN64-STD", "High", "Processing", "USR-013", "DATE_SUB(NOW(), INTERVAL 4 DAY)", "DATE_ADD(NOW(), INTERVAL 9 DAY)", "DATE_SUB(NOW(), INTERVAL 3 DAY)", None, 99.00, 95.50, "驱动器紧急订单"),
        ("WO-2026008", "PROD-SOP14", "SOP-14 运放", "SOP14-STD:1.0", "SOP-14 标准路线", "OP-100", "SOP", 40000, 40000, 8, 40000, "CUST-CON", "某消费电子", "CE-SOP14-008", "PN-SOP14-STD", "Normal", "Completed", "USR-013", "DATE_SUB(NOW(), INTERVAL 20 DAY)", "DATE_SUB(NOW(), INTERVAL 5 DAY)", "DATE_SUB(NOW(), INTERVAL 19 DAY)", "DATE_SUB(NOW(), INTERVAL 5 DAY)", 99.00, 96.00, "已完成运放订单"),
    ]
    for i, o in enumerate(orders):
        comma = ',' if i < len(orders) - 1 else ';'
        L(f"('{o[0]}', '{o[1]}', '{o[2]}', '{o[3]}', '{o[4]}', '{o[5]}', '{o[6]}', {o[7]}, {o[8]}, {o[9]}, {o[10]}, '{o[11]}', '{o[12]}', '{o[13]}', '{o[14]}', '{o[15]}', '{o[16]}', '{o[17]}', {sql_val(o[18])}, {sql_val(o[19])}, {sql_val(o[20])}, {sql_val(o[21])}, {o[22]}, {o[23]}, '{o[24]}'){comma}")
    L('')
    
    # 3.2 批次
    L('INSERT INTO `prod_lot` (`lot_id`, `order_id`, `product_id`, `product_name`, `die_name`, `package_type`, `route_id`, `route_version`, `current_step_seq`, `current_step_code`, `status`, `original_qty`, `total_pass_qty`, `total_scrap_qty`, `total_rework_qty`, `total_hold_qty`, `is_rework_lot`, `carrier_type`, `carrier_id`) VALUES')
    lots = [
        ("LOT-001", "WO-2026001", "PROD-QFN88", "QFN-88 控制器", "CTRL-2024", "QFN", "QFN-STD:2.0", "2.0", 4, "WB", "Processing", 5000, 3000, 200, 0, 0, 0, "LeadFrame", "CARRIER-LEAD-002"),
        ("LOT-002", "WO-2026001", "PROD-QFN88", "QFN-88 控制器", "CTRL-2024", "QFN", "QFN-STD:2.0", "2.0", 5, "MOLD", "Processing", 5000, 2800, 150, 0, 0, 0, "Tray", "CARRIER-TRAY-003"),
        ("LOT-003", "WO-2026001", "PROD-QFN88", "QFN-88 控制器", "CTRL-2024", "QFN", "QFN-STD:2.0", "2.0", 6, "PMC", "Waiting", 5000, 0, 0, 0, 0, 0, None, None),
        ("LOT-HOLD-001", "WO-2026001", "PROD-QFN88", "QFN-88 控制器", "CTRL-2024", "QFN", "QFN-STD:2.0", "2.0", 2, "DA", "Hold", 5000, 0, 300, 0, 500, 0, None, None),
        ("LOT-REWORK-001", "WO-2026001", "PROD-QFN88", "QFN-88 控制器", "CTRL-2024", "QFN", "RW-WB:1.0", "1.0", 1, "DEBOND-WB", "Processing", 5000, 0, 0, 500, 0, 1, None, None),
        ("LOT-004", "WO-2026002", "PROD-SOP16", "SOP-16 电源IC", "PWR-500", "SOP", "SOP-STD:1.0", "1.0", 9, "PACK", "Completed", 5000, 4800, 100, 100, 0, 0, "Magazine", "CARRIER-MAG-002"),
        ("LOT-005", "WO-2026002", "PROD-SOP16", "SOP-16 电源IC", "PWR-500", "SOP", "SOP-STD:1.0", "1.0", 9, "PACK", "Completed", 5000, 4750, 150, 100, 0, 0, "Magazine", "CARRIER-MAG-003"),
        ("LOT-006", "WO-2026002", "PROD-SOP16", "SOP-16 电源IC", "PWR-500", "SOP", "SOP-STD:1.0", "1.0", 9, "PACK", "Completed", 5000, 4850, 80, 70, 0, 0, "Magazine", "CARRIER-MAG-004"),
        ("LOT-007", "WO-2026003", "PROD-BGA256", "BGA-256 处理器", "CPU-3000", "BGA", "BGA-STD:1.0", "1.0", 4, "WB", "Processing", 2500, 1500, 50, 0, 0, 0, "Tray", "CARRIER-TRAY-001"),
        ("LOT-008", "WO-2026003", "PROD-BGA256", "BGA-256 处理器", "CPU-3000", "BGA", "BGA-STD:1.0", "1.0", 5, "MOLD", "Processing", 2500, 1200, 30, 0, 0, 0, "Tray", "CARRIER-TRAY-002"),
        ("LOT-009", "WO-2026005", "PROD-SOP8", "SOP-8 MOSFET", "MOS-200", "SOP", "SOP8-STD:1.0", "1.0", 3, "MOLD", "Processing", 10000, 6000, 200, 0, 0, 0, "LeadFrame", "CARRIER-LEAD-001"),
        ("LOT-010", "WO-2026005", "PROD-SOP8", "SOP-8 MOSFET", "MOS-200", "SOP", "SOP8-STD:1.0", "1.0", 4, "MARK", "Processing", 10000, 5800, 150, 50, 0, 0, "LeadFrame", "CARRIER-LEAD-003"),
        ("LOT-011", "WO-2026006", "PROD-BGA64", "BGA-64 存储器", "MEM-500", "BGA", "BGA64-STD:1.0", "1.0", 6, "PMC", "Processing", 5000, 3000, 100, 0, 0, 0, "Tray", "CARRIER-TRAY-004"),
        ("LOT-012", "WO-2026007", "PROD-QFN64", "QFN-64 驱动器", "DRV-300", "QFN", "QFN64-STD:1.0", "1.0", 5, "MOLD", "Processing", 10000, 6000, 200, 0, 0, 0, "Tray", "CARRIER-TRAY-005"),
        ("LOT-013", "WO-2026007", "PROD-QFN64", "QFN-64 驱动器", "DRV-300", "QFN", "QFN64-STD:1.0", "1.0", 7, "MARK", "Processing", 10000, 5500, 150, 350, 0, 0, "LeadFrame", "CARRIER-LEAD-004"),
        ("LOT-014", "WO-2026008", "PROD-SOP14", "SOP-14 运放", "OP-100", "SOP", "SOP14-STD:1.0", "1.0", 8, "PACK", "Completed", 20000, 19500, 300, 200, 0, 0, "Magazine", "CARRIER-MAG-001"),
        ("LOT-015", "WO-2026008", "PROD-SOP14", "SOP-14 运放", "OP-100", "SOP", "SOP14-STD:1.0", "1.0", 8, "PACK", "Completed", 20000, 19600, 250, 150, 0, 0, "Magazine", "CARRIER-MAG-005"),
    ]
    for i, l in enumerate(lots):
        comma = ',' if i < len(lots) - 1 else ';'
        ct = f"'{l[17]}'" if l[17] else 'NULL'
        ci = f"'{l[18]}'" if l[18] else 'NULL'
        L(f"('{l[0]}', '{l[1]}', '{l[2]}', '{l[3]}', '{l[4]}', '{l[5]}', '{l[6]}', '{l[7]}', {l[8]}, '{l[9]}', '{l[10]}', {l[11]}, {l[12]}, {l[13]}, {l[14]}, {l[15]}, {l[16]}, {ct}, {ci}){comma}")
    L('')
    
    # 3.3 批次步骤记录
    L('INSERT INTO `prod_lot_step` (`record_id`, `lot_id`, `route_id`, `route_version`, `step_seq`, `step_code`, `step_name`, `status`, `track_in_equipment`, `track_in_carrier`, `track_in_recipe`, `track_in_time`, `track_in_operator`, `track_out_time`, `track_out_operator`, `input_qty`, `pass_qty`, `fail_qty`, `scrap_qty`, `rework_qty`, `hold_qty`, `pending_qty`) VALUES')
    
    lot_steps = [
        # LOT-001: QFN88, 当前在WB(4)
        ("RS-001-1", "LOT-001", "QFN-STD:2.0", "2.0", 1, "SAW", "晶圆切割", "Completed", "SAW-001", None, "REC-SAW-QFN88-001", "DATE_SUB(NOW(), INTERVAL 4 DAY)", "USR-005", "DATE_SUB(NOW(), INTERVAL 3 DAY)", "USR-010", 5000, 4900, 50, 50, 0, 0, 0),
        ("RS-001-2", "LOT-001", "QFN-STD:2.0", "2.0", 2, "DA", "芯片贴装", "Completed", "DA-001", "CARRIER-LEAD-002", "REC-DA-QFN88-001", "DATE_SUB(NOW(), INTERVAL 3 DAY)", "USR-006", "DATE_SUB(NOW(), INTERVAL 2 DAY)", "USR-011", 4900, 4800, 50, 50, 0, 0, 0),
        ("RS-001-3", "LOT-001", "QFN-STD:2.0", "2.0", 3, "CURE", "固化", "Completed", "CURE-001", "CARRIER-LEAD-002", "REC-CURE-QFN88-001", "DATE_SUB(NOW(), INTERVAL 2 DAY)", "USR-021", "DATE_SUB(NOW(), INTERVAL 1 DAY)", "USR-024", 4800, 4750, 30, 20, 0, 0, 0),
        ("RS-001-4", "LOT-001", "QFN-STD:2.0", "2.0", 4, "WB", "焊线", "Processing", "WB-002", "CARRIER-LEAD-002", "REC-WB-QFN88-001", "DATE_SUB(NOW(), INTERVAL 1 DAY)", "USR-007", None, None, 4750, 3000, 100, 150, 0, 0, 1500),
        ("RS-001-5", "LOT-001", "QFN-STD:2.0", "2.0", 5, "MOLD", "塑封", "Waiting", None, None, None, None, None, None, None, 0, 0, 0, 0, 0, 0, 4750),
        # LOT-HOLD-001: 被Hold在DA
        ("RS-HOLD-1", "LOT-HOLD-001", "QFN-STD:2.0", "2.0", 1, "SAW", "晶圆切割", "Completed", "SAW-002", None, "REC-SAW-QFN88-002", "DATE_SUB(NOW(), INTERVAL 5 DAY)", "USR-005", "DATE_SUB(NOW(), INTERVAL 4 DAY)", "USR-018", 5000, 4850, 80, 70, 0, 0, 0),
        ("RS-HOLD-2", "LOT-HOLD-001", "QFN-STD:2.0", "2.0", 2, "DA", "芯片贴装", "Hold", "DA-002", None, "REC-DA-QFN88-002", "DATE_SUB(NOW(), INTERVAL 4 DAY)", "USR-006", None, None, 4850, 0, 200, 300, 0, 500, 3850),
        # LOT-REWORK-001: 重工批次
        ("RS-RW-1", "LOT-REWORK-001", "RW-WB:1.0", "1.0", 1, "DEBOND-WB", "去线", "Processing", "WB-001", None, None, "DATE_SUB(NOW(), INTERVAL 1 DAY)", "USR-007", None, None, 5000, 0, 0, 0, 0, 0, 5000),
        # LOT-004: SOP16 已完成
        ("RS-004-1", "LOT-004", "SOP-STD:1.0", "1.0", 1, "SAW", "晶圆切割", "Completed", "SAW-001", None, "REC-SAW-SOP16-001", "DATE_SUB(NOW(), INTERVAL 14 DAY)", "USR-005", "DATE_SUB(NOW(), INTERVAL 13 DAY)", "USR-010", 5000, 4920, 40, 40, 0, 0, 0),
        ("RS-004-2", "LOT-004", "SOP-STD:1.0", "1.0", 2, "DA", "芯片贴装", "Completed", "DA-001", "CARRIER-MAG-002", "REC-DA-SOP16-001", "DATE_SUB(NOW(), INTERVAL 13 DAY)", "USR-006", "DATE_SUB(NOW(), INTERVAL 12 DAY)", "USR-011", 4920, 4850, 30, 40, 0, 0, 0),
        ("RS-004-3", "LOT-004", "SOP-STD:1.0", "1.0", 3, "WB", "焊线", "Completed", "WB-001", "CARRIER-MAG-002", "REC-WB-SOP16-001", "DATE_SUB(NOW(), INTERVAL 12 DAY)", "USR-007", "DATE_SUB(NOW(), INTERVAL 11 DAY)", "USR-012", 4850, 4780, 40, 30, 0, 0, 0),
        ("RS-004-4", "LOT-004", "SOP-STD:1.0", "1.0", 4, "MOLD", "塑封", "Completed", "MOLD-001", "CARRIER-MAG-002", "REC-MOLD-SOP16-001", "DATE_SUB(NOW(), INTERVAL 11 DAY)", "USR-008", "DATE_SUB(NOW(), INTERVAL 10 DAY)", "USR-016", 4780, 4720, 30, 30, 0, 0, 0),
        ("RS-004-5", "LOT-004", "SOP-STD:1.0", "1.0", 5, "MARK", "激光打标", "Completed", "LASER-001", "CARRIER-MAG-002", "REC-MARK-SOP16-001", "DATE_SUB(NOW(), INTERVAL 10 DAY)", "USR-022", "DATE_SUB(NOW(), INTERVAL 9 DAY)", "USR-025", 4720, 4680, 20, 20, 0, 0, 0),
        ("RS-004-6", "LOT-004", "SOP-STD:1.0", "1.0", 6, "SING", "切割分条", "Completed", "SING-001", "CARRIER-MAG-002", "REC-SING-SOP16-001", "DATE_SUB(NOW(), INTERVAL 9 DAY)", "USR-023", "DATE_SUB(NOW(), INTERVAL 8 DAY)", "USR-026", 4680, 4640, 20, 20, 0, 0, 0),
        ("RS-004-7", "LOT-004", "SOP-STD:1.0", "1.0", 7, "FT", "最终测试", "Completed", "FT-001", "CARRIER-MAG-002", "REC-FT-SOP16-001", "DATE_SUB(NOW(), INTERVAL 8 DAY)", "USR-009", "DATE_SUB(NOW(), INTERVAL 7 DAY)", "USR-019", 4640, 4500, 100, 40, 100, 0, 0),
        ("RS-004-8", "LOT-004", "SOP-STD:1.0", "1.0", 8, "OQC", "出货检验", "Completed", "OQC-001", "CARRIER-MAG-002", None, "DATE_SUB(NOW(), INTERVAL 7 DAY)", "USR-014", "DATE_SUB(NOW(), INTERVAL 6 DAY)", "USR-020", 4500, 4450, 50, 0, 0, 0, 0),
        ("RS-004-9", "LOT-004", "SOP-STD:1.0", "1.0", 9, "PACK", "包装入库", "Completed", None, "CARRIER-MAG-002", None, "DATE_SUB(NOW(), INTERVAL 6 DAY)", "USR-004", "DATE_SUB(NOW(), INTERVAL 5 DAY)", "USR-004", 4450, 4400, 0, 50, 0, 0, 0),
    ]
    for i, s in enumerate(lot_steps):
        comma = ',' if i < len(lot_steps) - 1 else ';'
        L(f"('{s[0]}', '{s[1]}', '{s[2]}', '{s[3]}', {s[4]}, '{s[5]}', '{s[6]}', '{s[7]}', {sql_val(s[8])}, {sql_val(s[9])}, {sql_val(s[10])}, {sql_val(s[11])}, {sql_val(s[12])}, {sql_val(s[13])}, {sql_val(s[14])}, {s[15]}, {s[16]}, {s[17]}, {s[18]}, {s[19]}, {s[20]}, {s[21]}){comma}")
    L('')
    
    # 3.4 操作历史
    L('INSERT INTO `prod_operation_history` (`operation_id`, `lot_id`, `order_id`, `operation_type`, `step_code`, `step_seq`, `equipment_id`, `carrier_id`, `recipe_id`, `operator_id`, `operator_name`, `input_qty`, `output_qty`, `scrap_qty`) VALUES')
    ops = [
        ("OP-001", "LOT-001", "WO-2026001", "TrackIn", "SAW", 1, "SAW-001", None, "REC-SAW-QFN88-001", "USR-005", "孙切割", 5000, None, None),
        ("OP-002", "LOT-001", "WO-2026001", "TrackOut", "SAW", 1, "SAW-001", None, "REC-SAW-QFN88-001", "USR-010", "刘操作员", 5000, 4900, 50),
        ("OP-003", "LOT-001", "WO-2026001", "TrackIn", "DA", 2, "DA-001", "CARRIER-LEAD-002", "REC-DA-QFN88-001", "USR-006", "周贴片", 4900, None, None),
        ("OP-004", "LOT-001", "WO-2026001", "TrackOut", "DA", 2, "DA-001", "CARRIER-LEAD-002", "REC-DA-QFN88-001", "USR-011", "黄操作员", 4900, 4800, 50),
        ("OP-005", "LOT-HOLD-001", "WO-2026001", "TrackIn", "DA", 2, "DA-002", None, "REC-DA-QFN88-002", "USR-006", "周贴片", 4850, None, None),
        ("OP-006", "LOT-HOLD-001", "WO-2026001", "Hold", "DA", 2, "DA-002", None, None, "USR-002", "李品质", None, None, 300),
        ("OP-007", "LOT-004", "WO-2026002", "TrackIn", "SAW", 1, "SAW-001", None, "REC-SAW-SOP16-001", "USR-005", "孙切割", 5000, None, None),
        ("OP-008", "LOT-004", "WO-2026002", "TrackOut", "PACK", 9, None, "CARRIER-MAG-002", None, "USR-004", "赵仓储", 4450, 4400, 50),
        ("OP-009", "LOT-REWORK-001", "WO-2026001", "Rework", "DEBOND-WB", 1, "WB-001", None, None, "USR-007", "吴焊线", 5000, None, None),
        ("OP-010", "LOT-007", "WO-2026003", "TrackIn", "SAW", 1, "SAW-003", None, "REC-SAW-BGA256-001", "USR-005", "孙切割", 2500, None, None),
    ]
    for i, o in enumerate(ops):
        comma = ',' if i < len(ops) - 1 else ';'
        iq = f'{o[11]}' if o[11] is not None else 'NULL'
        oq = f'{o[12]}' if o[12] is not None else 'NULL'
        sq = f'{o[13]}' if o[13] is not None else 'NULL'
        L(f"('{o[0]}', '{o[1]}', '{o[2]}', '{o[3]}', '{o[4]}', {o[5]}, {sql_val(o[6])}, {sql_val(o[7])}, {sql_val(o[8])}, '{o[9]}', '{o[10]}', {iq}, {oq}, {sq}){comma}")
    L('')
    
    # 3.5 审计追踪
    L('INSERT INTO `prod_audit_trail` (`audit_id`, `entity_type`, `entity_id`, `action`, `operator_id`, `operator_name`, `timestamp`, `before_state`, `after_state`, `reason`, `signature_level`) VALUES')
    audits = [
        ("AUD-001", "Lot", "LOT-001", "TrackIn", "USR-005", "孙切割", "DATE_SUB(NOW(), INTERVAL 4 DAY)", '{"status": "Waiting"}', '{"status": "Processing", "step": "SAW"}', '正常投产', 0),
        ("AUD-002", "Lot", "LOT-001", "TrackOut", "USR-010", "刘操作员", "DATE_SUB(NOW(), INTERVAL 3 DAY)", '{"step": "SAW", "status": "Processing"}', '{"step": "SAW", "status": "Completed", "pass_qty": 4900}', 'SAW完成', 0),
        ("AUD-003", "Lot", "LOT-HOLD-001", "Hold", "USR-002", "李品质", "DATE_SUB(NOW(), INTERVAL 4 DAY)", '{"status": "Processing"}', '{"status": "Hold", "hold_reason": "DA良率异常"}', 'DA工序良率低于阈值', 1),
        ("AUD-004", "Lot", "LOT-REWORK-001", "Rework", "USR-003", "王工程", "DATE_SUB(NOW(), INTERVAL 1 DAY)", '{"status": "Processing", "route": "QFN-STD:2.0"}', '{"status": "Processing", "route": "RW-WB:1.0"}', 'WB工序重工', 1),
        ("AUD-005", "Lot", "LOT-004", "Completed", "USR-004", "赵仓储", "DATE_SUB(NOW(), INTERVAL 5 DAY)", '{"status": "Processing"}', '{"status": "Completed", "final_yield": 96.0}', '工单完成入库', 0),
        ("AUD-006", "WorkOrder", "WO-2026002", "Completed", "USR-013", "何计划", "DATE_SUB(NOW(), INTERVAL 2 DAY)", '{"status": "Processing"}', '{"status": "Completed", "completed_qty": 15000}', '全部批次完成', 0),
    ]
    for i, a in enumerate(audits):
        comma = ',' if i < len(audits) - 1 else ';'
        L(f"('{a[0]}', '{a[1]}', '{a[2]}', '{a[3]}', '{a[4]}', '{a[5]}', {a[6]}, '{a[7]}', '{a[8]}', '{a[9]}', {a[10]}){comma}")
    L('')
    
    # 四、异常管理数据
    L('-- ============================================================')
    L('-- 四、异常管理数据')
    L('-- ============================================================')
    L('')
    
    # 4.1 Hold记录
    L('INSERT INTO `prod_hold_record` (`hold_id`, `lot_id`, `hold_type`, `hold_reason_code`, `hold_reason`, `hold_qty`, `responsible_dept`, `owner`, `status`, `hold_by`, `hold_time`, `root_cause`, `corrective_action`, `disposition`, `release_by`, `release_time`) VALUES')
    holds = [
        ("HOLD-001", "LOT-HOLD-001", "Quality", "YIELD_LOW", "DA工序良率低于99%阈值", 500, "DEPT-QA", "USR-002", "Open", "USR-002", "DATE_SUB(NOW(), INTERVAL 4 DAY)", "待分析", None, None, None, None),
        ("HOLD-002", "LOT-001", "Quality", "YIELD_WARN", "WB工序良率预警", 150, "DEPT-QA", "USR-002", "Closed", "USR-002", "DATE_SUB(NOW(), INTERVAL 1 DAY)", "设备参数偏移", "调整焊线参数", "继续生产", "USR-003", "DATE_SUB(NOW(), INTERVAL 0 DAY)"),
        ("HOLD-003", "LOT-007", "Equipment", "EQUIP_FAULT", "SAW-003设备异常", 200, "DEPT-ENG", "USR-015", "Open", "USR-015", "DATE_SUB(NOW(), INTERVAL 2 DAY)", "待维修", None, None, None, None),
    ]
    for i, h in enumerate(holds):
        comma = ',' if i < len(holds) - 1 else ';'
        L(f"('{h[0]}', '{h[1]}', '{h[2]}', '{h[3]}', '{h[4]}', {h[5]}, '{h[6]}', '{h[7]}', '{h[8]}', '{h[9]}', {sql_val(h[10])}, '{h[11]}', {sql_val(h[12])}, {sql_val(h[13])}, {sql_val(h[14])}, {sql_val(h[15])}){comma}")
    L('')
    
    # 4.2 报废记录
    L('INSERT INTO `prod_scrap_record` (`scrap_id`, `lot_id`, `step_code`, `step_seq`, `scrap_qty`, `scrap_reason`, `scrap_reason_code`, `operator_id`, `scrap_time`, `approved_by`) VALUES')
    scraps = [
        ("SCRAP-001", "LOT-001", "SAW", 1, 50, "切割偏移", "SC-001", "USR-010", "DATE_SUB(NOW(), INTERVAL 3 DAY)", "USR-005"),
        ("SCRAP-002", "LOT-001", "DA", 2, 50, "贴片偏移", "SC-002", "USR-011", "DATE_SUB(NOW(), INTERVAL 2 DAY)", "USR-006"),
        ("SCRAP-003", "LOT-HOLD-001", "DA", 2, 300, "芯片破损", "SC-003", "USR-011", "DATE_SUB(NOW(), INTERVAL 4 DAY)", "USR-003"),
        ("SCRAP-004", "LOT-004", "FT", 7, 40, "测试不良", "SC-004", "USR-019", "DATE_SUB(NOW(), INTERVAL 7 DAY)", "USR-009"),
        ("SCRAP-005", "LOT-007", "SAW", 1, 70, "切割裂纹", "SC-001", "USR-010", "DATE_SUB(NOW(), INTERVAL 2 DAY)", "USR-005"),
    ]
    for i, s in enumerate(scraps):
        comma = ',' if i < len(scraps) - 1 else ';'
        L(f"('{s[0]}', '{s[1]}', '{s[2]}', {s[3]}, {s[4]}, '{s[5]}', '{s[6]}', '{s[7]}', {s[8]}, '{s[9]}'){comma}")
    L('')
    
    # 4.3 重工记录
    L('INSERT INTO `prod_rework_record` (`rework_id`, `lot_id`, `original_route_id`, `rework_route_id`, `from_step_code`, `target_step_code`, `rework_qty`, `rework_reason`, `operator_id`, `rework_count`, `approved_by`) VALUES')
    reworks = [
        ("RW-001", "LOT-REWORK-001", "QFN-STD:2.0", "RW-WB:1.0", "WB", "DEBOND-WB", 5000, "焊线拉力不足", "USR-007", 1, "USR-003"),
        ("RW-002", "LOT-013", "QFN64-STD:1.0", "RW-MOLD:1.0", "MOLD", "DECAP", 350, "塑封气泡", "USR-008", 1, "USR-003"),
    ]
    for i, r in enumerate(reworks):
        comma = ',' if i < len(reworks) - 1 else ';'
        L(f"('{r[0]}', '{r[1]}', '{r[2]}', '{r[3]}', '{r[4]}', '{r[5]}', {r[6]}, '{r[7]}', '{r[8]}', {r[9]}, '{r[10]}'){comma}")
    L('')
    
    # 五、追溯数据
    L('-- ============================================================')
    L('-- 五、追溯数据')
    L('-- ============================================================')
    L('')
    
    # 5.1 批次拆分
    L('INSERT INTO `prod_lot_split` (`split_id`, `mother_lot_id`, `child_lot_id`, `split_qty`, `split_reason`, `split_type`, `step_code`, `step_seq`, `operator_id`, `split_time`, `approved_by`) VALUES')
    splits = [
        ("SPLIT-001", "LOT-001", "LOT-REWORK-001", 5000, "WB重工拆分", "Rework", "WB", 4, "USR-007", "DATE_SUB(NOW(), INTERVAL 1 DAY)", "USR-003"),
        ("SPLIT-002", "LOT-007", "LOT-007-GRADE", 200, "等级拆分", "Grade", "SAW", 1, "USR-005", "DATE_SUB(NOW(), INTERVAL 2 DAY)", "USR-003"),
    ]
    for i, s in enumerate(splits):
        comma = ',' if i < len(splits) - 1 else ';'
        L(f"('{s[0]}', '{s[1]}', '{s[2]}', {s[3]}, '{s[4]}', '{s[5]}', '{s[6]}', {s[7]}, '{s[8]}', {s[9]}, '{s[10]}'){comma}")
    L('')
    
    # 5.2 载具绑定
    L('INSERT INTO `prod_carrier_binding` (`binding_id`, `lot_id`, `step_code`, `step_seq`, `carrier_id`, `carrier_type`, `bind_time`, `operator_id`) VALUES')
    bindings = [
        ("BIND-001", "LOT-001", "DA", 2, "CARRIER-LEAD-002", "LeadFrame", "DATE_SUB(NOW(), INTERVAL 3 DAY)", "USR-006"),
        ("BIND-002", "LOT-001", "WB", 4, "CARRIER-LEAD-002", "LeadFrame", "DATE_SUB(NOW(), INTERVAL 1 DAY)", "USR-007"),
        ("BIND-003", "LOT-002", "MOLD", 5, "CARRIER-TRAY-003", "Tray", "DATE_SUB(NOW(), INTERVAL 1 DAY)", "USR-008"),
        ("BIND-004", "LOT-004", "SAW", 1, "CARRIER-MAG-002", "Magazine", "DATE_SUB(NOW(), INTERVAL 14 DAY)", "USR-005"),
        ("BIND-005", "LOT-007", "SAW", 1, "CARRIER-TRAY-001", "Tray", "DATE_SUB(NOW(), INTERVAL 2 DAY)", "USR-005"),
        ("BIND-006", "LOT-009", "MOLD", 3, "CARRIER-LEAD-001", "LeadFrame", "DATE_SUB(NOW(), INTERVAL 6 DAY)", "USR-008"),
    ]
    for i, b in enumerate(bindings):
        comma = ',' if i < len(bindings) - 1 else ';'
        L(f"('{b[0]}', '{b[1]}', '{b[2]}', {b[3]}, '{b[4]}', '{b[5]}', {b[6]}, '{b[7]}'){comma}")
    L('')
    
    # 六、派工与调度
    L('-- ============================================================')
    L('-- 六、派工与调度')
    L('-- ============================================================')
    L('')
    
    L('INSERT INTO `prod_dispatch_task` (`task_id`, `lot_id`, `order_id`, `product_id`, `step_code`, `step_name`, `step_seq`, `equipment_id`, `qty`, `priority`, `status`, `assigned_operator`, `created_at`) VALUES')
    tasks = [
        ("TASK-001", "LOT-001", "WO-2026001", "PROD-QFN88", "WB", "焊线", 4, "WB-002", 4750, "High", "Processing", "USR-007", "DATE_SUB(NOW(), INTERVAL 1 DAY)"),
        ("TASK-002", "LOT-001", "WO-2026001", "PROD-QFN88", "MOLD", "塑封", 5, "MOLD-001", 4750, "High", "Pending", None, "DATE_SUB(NOW(), INTERVAL 1 DAY)"),
        ("TASK-003", "LOT-002", "WO-2026001", "PROD-QFN88", "MOLD", "塑封", 5, "MOLD-002", 2800, "High", "Processing", "USR-008", "DATE_SUB(NOW(), INTERVAL 1 DAY)"),
        ("TASK-004", "LOT-003", "WO-2026001", "PROD-QFN88", "PMC", "后固化", 6, "PMC-001", 5000, "Normal", "Pending", None, "DATE_SUB(NOW(), INTERVAL 0 DAY)"),
        ("TASK-005", "LOT-007", "WO-2026003", "PROD-BGA256", "WB", "焊线", 4, "WB-003", 2500, "High", "Pending", None, "DATE_SUB(NOW(), INTERVAL 1 DAY)"),
        ("TASK-006", "LOT-009", "WO-2026005", "PROD-SOP8", "MOLD", "塑封", 3, "MOLD-001", 10000, "Normal", "Processing", "USR-016", "DATE_SUB(NOW(), INTERVAL 6 DAY)"),
        ("TASK-007", "LOT-011", "WO-2026006", "PROD-BGA64", "PMC", "后固化", 6, "PMC-002", 5000, "Normal", "Processing", "USR-024", "DATE_SUB(NOW(), INTERVAL 3 DAY)"),
        ("TASK-008", "LOT-012", "WO-2026007", "PROD-QFN64", "MOLD", "塑封", 5, "MOLD-003", 10000, "High", "Processing", "USR-008", "DATE_SUB(NOW(), INTERVAL 2 DAY)"),
    ]
    for i, t in enumerate(tasks):
        comma = ',' if i < len(tasks) - 1 else ';'
        L(f"('{t[0]}', '{t[1]}', '{t[2]}', '{t[3]}', '{t[4]}', '{t[5]}', {t[6]}, '{t[7]}', {t[8]}, '{t[9]}', '{t[10]}', {sql_val(t[11])}, {sql_val(t[12])}){comma}")
    L('')
    
    # 七、质量管理
    L('-- ============================================================')
    L('-- 七、质量管理')
    L('-- ============================================================')
    L('')
    
    # 7.1 质量Gate
    L('INSERT INTO `quality_gate` (`gate_id`, `lot_id`, `step_code`, `gate_type`, `status`, `checker_id`, `check_result`, `checked_at`) VALUES')
    gates = [
        ("GATE-001", "LOT-004", "FT", "QA", "Passed", "USR-002", "Pass", "DATE_SUB(NOW(), INTERVAL 7 DAY)"),
        ("GATE-002", "LOT-004", "OQC", "QA", "Passed", "USR-014", "Pass", "DATE_SUB(NOW(), INTERVAL 6 DAY)"),
        ("GATE-003", "LOT-005", "FT", "QA", "Passed", "USR-002", "Pass", "DATE_SUB(NOW(), INTERVAL 6 DAY)"),
        ("GATE-004", "LOT-005", "OQC", "QA", "Passed", "USR-014", "Pass", "DATE_SUB(NOW(), INTERVAL 5 DAY)"),
        ("GATE-005", "LOT-014", "FT", "QA", "Passed", "USR-002", "Pass", "DATE_SUB(NOW(), INTERVAL 8 DAY)"),
        ("GATE-006", "LOT-014", "OQC", "QA", "Passed", "USR-014", "Pass", "DATE_SUB(NOW(), INTERVAL 7 DAY)"),
        ("GATE-007", "LOT-HOLD-001", "DA", "QA", "Failed", "USR-002", "Fail", "DATE_SUB(NOW(), INTERVAL 4 DAY)"),
    ]
    for i, g in enumerate(gates):
        comma = ',' if i < len(gates) - 1 else ';'
        L(f"('{g[0]}', '{g[1]}', '{g[2]}', '{g[3]}', '{g[4]}', '{g[5]}', '{g[6]}', {g[7]}){comma}")
    L('')
    
    # 7.2 检验记录
    L('INSERT INTO `quality_inspection` (`inspection_id`, `lot_id`, `step_code`, `inspection_type`, `result`, `inspector_id`, `inspection_time`) VALUES')
    inspections = [
        ("INSP-001", "LOT-004", "FT", "FinalTest", "Pass", "USR-019", "DATE_SUB(NOW(), INTERVAL 7 DAY)"),
        ("INSP-002", "LOT-004", "OQC", "OQC", "Pass", "USR-020", "DATE_SUB(NOW(), INTERVAL 6 DAY)"),
        ("INSP-003", "LOT-HOLD-001", "DA", "InProcess", "Fail", "USR-002", "DATE_SUB(NOW(), INTERVAL 4 DAY)"),
        ("INSP-004", "LOT-007", "SAW", "InProcess", "Pass", "USR-002", "DATE_SUB(NOW(), INTERVAL 2 DAY)"),
        ("INSP-005", "LOT-014", "FT", "FinalTest", "Pass", "USR-019", "DATE_SUB(NOW(), INTERVAL 8 DAY)"),
    ]
    for i, ins in enumerate(inspections):
        comma = ',' if i < len(inspections) - 1 else ';'
        L(f"('{ins[0]}', '{ins[1]}', '{ins[2]}', '{ins[3]}', '{ins[4]}', '{ins[5]}', {ins[6]}){comma}")
    L('')
    
    # 八、报警
    L('-- ============================================================')
    L('-- 八、报警')
    L('-- ============================================================')
    L('')
    
    # 8.1 报警规则
    L('INSERT INTO `alarm_rule` (`rule_id`, `rule_name`, `rule_type`, `condition_expr`, `severity`, `notify_roles`, `is_active`) VALUES')
    alarm_rules = [
        ("AR-001", "良率过低报警", "Yield", "yield < 95%", "Critical", "ROLE-QA,ROLE-ENG", 1),
        ("AR-002", "Hold超时报警", "HoldTimeout", "hold_duration > 24h", "Warning", "ROLE-QA,ROLE-MANAGER", 1),
        ("AR-003", "队列超时报警", "QueueTimeout", "queue_duration > 4h", "Warning", "ROLE-SUPERVISOR", 1),
        ("AR-004", "设备离线报警", "Equipment", "status = Offline", "Warning", "ROLE-ENG,ROLE-MAINT", 1),
        ("AR-005", "重工次数超限", "Yield", "rework_count > 2", "Critical", "ROLE-QA,ROLE-ENG", 1),
    ]
    for i, ar in enumerate(alarm_rules):
        comma = ',' if i < len(alarm_rules) - 1 else ';'
        L(f"('{ar[0]}', '{ar[1]}', '{ar[2]}', '{ar[3]}', '{ar[4]}', '{ar[5]}', {ar[6]}){comma}")
    L('')
    
    # 8.2 报警记录
    L('INSERT INTO `alarm_record` (`alarm_id`, `rule_id`, `lot_id`, `equipment_id`, `alarm_type`, `severity`, `message`, `status`, `acknowledged_by`, `acknowledged_at`) VALUES')
    alarms = [
        ("ALARM-001", "AR-001", "LOT-HOLD-001", "DA-002", "Yield", "Critical", "LOT-HOLD-001 DA工序良率低于95%", "Acknowledged", "USR-002", "DATE_SUB(NOW(), INTERVAL 4 DAY)"),
        ("ALARM-002", "AR-004", None, "DA-004", "Equipment", "Warning", "DA-004设备离线超过20天", "Active", None, None),
        ("ALARM-003", "AR-004", None, "WB-004", "Equipment", "Warning", "WB-004设备离线超过30天", "Active", None, None),
        ("ALARM-004", "AR-001", "LOT-007", "SAW-003", "Yield", "Warning", "LOT-007 SAW工序良率97.2%接近阈值", "Resolved", "USR-002", "DATE_SUB(NOW(), INTERVAL 1 DAY)"),
        ("ALARM-005", "AR-002", "LOT-HOLD-001", None, "HoldTimeout", "Warning", "LOT-HOLD-001 Hold超过24小时", "Active", None, None),
    ]
    for i, al in enumerate(alarms):
        comma = ',' if i < len(alarms) - 1 else ';'
        lid = f"'{al[2]}'" if al[2] else 'NULL'
        eid = f"'{al[3]}'" if al[3] else 'NULL'
        ab = f"'{al[8]}'" if al[8] else 'NULL'
        at = f"'{al[9]}'" if al[9] else 'NULL'
        L(f"('{al[0]}', '{al[1]}', {lid}, {eid}, '{al[4]}', '{al[5]}', '{al[6]}', '{al[7]}', {ab}, {at}){comma}")
    L('')
    
    # 九、客户要求
    L('-- ============================================================')
    L('-- 九、客户要求')
    L('-- ============================================================')
    L('')
    
    L('INSERT INTO `customer_requirement` (`requirement_id`, `customer_id`, `customer_name`, `requirement_type`, `description`, `is_active`) VALUES')
    reqs = [
        ("REQ-001", "CUST-AUTO", "某汽车电子", "Quality", "汽车电子良率要求>=98%", 1),
        ("REQ-002", "CUST-AUTO", "某汽车电子", "Traceability", "要求全工序追溯", 1),
        ("REQ-003", "CUST-IND", "某工业客户", "Quality", "工业级良率要求>=96%", 1),
        ("REQ-004", "CUST-CON", "某消费电子", "Quality", "消费级良率要求>=95%", 1),
        ("REQ-005", "CUST-AUTO", "某汽车电子", "Packaging", "要求防静电包装", 1),
    ]
    for i, r in enumerate(reqs):
        comma = ',' if i < len(reqs) - 1 else ';'
        L(f"('{r[0]}', '{r[1]}', '{r[2]}', '{r[3]}', '{r[4]}', {r[5]}){comma}")
    L('')
    
    # 十、报表与归档
    L('-- ============================================================')
    L('-- 十、报表与归档')
    L('-- ============================================================')
    L('')
    
    # 10.1 生产报表
    L('INSERT INTO `report_production_daily` (`report_id`, `report_date`, `total_lots`, `completed_lots`, `wip_lots`, `hold_lots`, `total_input_qty`, `total_output_qty`, `total_scrap_qty`, `overall_yield`, `ft_yield`) VALUES')
    reports = [
        ("RPT-001", "DATE_SUB(CURDATE(), INTERVAL 1 DAY)", 15, 3, 10, 2, 95000, 45000, 2500, 97.50, 96.20),
        ("RPT-002", "DATE_SUB(CURDATE(), INTERVAL 2 DAY)", 12, 2, 8, 2, 80000, 38000, 2000, 97.00, 95.80),
        ("RPT-003", "DATE_SUB(CURDATE(), INTERVAL 3 DAY)", 10, 4, 5, 1, 70000, 50000, 1500, 97.80, 96.50),
        ("RPT-004", "DATE_SUB(CURDATE(), INTERVAL 4 DAY)", 8, 3, 4, 1, 60000, 42000, 1200, 97.20, 96.00),
        ("RPT-005", "DATE_SUB(CURDATE(), INTERVAL 5 DAY)", 10, 2, 7, 1, 65000, 30000, 1800, 96.80, 95.50),
    ]
    for i, r in enumerate(reports):
        comma = ',' if i < len(reports) - 1 else ';'
        L(f"('{r[0]}', {r[1]}, {r[2]}, {r[3]}, {r[4]}, {r[5]}, {r[6]}, {r[7]}, {r[8]}, {r[9]}, {r[10]}){comma}")
    L('')
    
    # 10.2 批次归档
    L('INSERT INTO `prod_lot_archive` (`lot_id`, `order_id`, `product_id`, `status`, `original_qty`, `total_pass_qty`, `total_scrap_qty`, `final_yield`, `completed_at`) VALUES')
    archives = [
        ("LOT-004", "WO-2026002", "PROD-SOP16", "Completed", 5000, 4800, 100, 96.00, "DATE_SUB(NOW(), INTERVAL 5 DAY)"),
        ("LOT-005", "WO-2026002", "PROD-SOP16", "Completed", 5000, 4750, 150, 95.00, "DATE_SUB(NOW(), INTERVAL 4 DAY)"),
        ("LOT-006", "WO-2026002", "PROD-SOP16", "Completed", 5000, 4850, 80, 97.00, "DATE_SUB(NOW(), INTERVAL 3 DAY)"),
        ("LOT-014", "WO-2026008", "PROD-SOP14", "Completed", 20000, 19500, 300, 97.50, "DATE_SUB(NOW(), INTERVAL 6 DAY)"),
        ("LOT-015", "WO-2026008", "PROD-SOP14", "Completed", 20000, 19600, 250, 98.00, "DATE_SUB(NOW(), INTERVAL 5 DAY)"),
    ]
    for i, a in enumerate(archives):
        comma = ',' if i < len(archives) - 1 else ';'
        L(f"('{a[0]}', '{a[1]}', '{a[2]}', '{a[3]}', {a[4]}, {a[5]}, {a[6]}, {a[7]}, {a[8]}){comma}")
    L('')
    
    L('-- ============================================================')
    L('-- 数据生成完成')
    L('-- ============================================================')
    
    # 写入文件
    with open(OUTPUT_FILE, 'w', encoding='utf-8') as f:
        f.write('\n'.join(lines))
    
    print(f'SQL文件已生成: {OUTPUT_FILE}')
    print(f'总行数: {len(lines)}')

if __name__ == '__main__':
    write_sql()