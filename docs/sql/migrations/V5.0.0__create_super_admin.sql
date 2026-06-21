-- ============================================================================
-- 创建超级管理员账号 (Super Admin)
-- 用户名: admin
-- 密码: SuperAdmin@2026
-- 角色: ROLE-ADMIN (系统最高权限)
-- ============================================================================

-- 1. 插入超级管理员用户
INSERT IGNORE INTO sys_user (user_id, user_name, password_hash, role_id, dept_id, shift, is_active, created_at, updated_at) 
VALUES 
('USR-ADMIN', 'admin', '$2a$11$VrPu3V.XsWtIlmlcZqd1pe4W0eG.oMav5qwrGVJ0zsn9FVU5kz31K', 'ROLE-ADMIN', 'DEPT-PROD', NULL, 1, NOW(), NOW());

-- 2. 赋予超级管理员所有权限
INSERT IGNORE INTO sys_user_permission (user_id, permission_code, created_at) VALUES
('USR-ADMIN', 'admin.all', NOW());
