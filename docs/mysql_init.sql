-- MES MySQL Database Initialization Script
-- 运行此脚本前请确保 MySQL 服务已启动

-- 创建数据库
CREATE DATABASE IF NOT EXISTS `mes_prod` 
CHARACTER SET utf8mb4 
COLLATE utf8mb4_unicode_ci;

USE `mes_prod`;

-- 创建统一 KV 存储表（替代 Redis）
-- 所有 Redis 数据类型都存储在一张表中：
--   string -> value 直接存 JSON 字符串
--   hash   -> value 存 {"field1":"val1","field2":"val2"}
--   list   -> value 存 ["val1","val2","val3"]
--   set    -> value 存 ["member1","member2"]
CREATE TABLE IF NOT EXISTS `mes_kv_store` (
    `key` VARCHAR(255) NOT NULL,
    `value` LONGTEXT NOT NULL,
    `data_type` VARCHAR(20) NOT NULL DEFAULT 'string',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`key`),
    INDEX `idx_data_type` (`data_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 验证表已创建
SELECT 
    TABLE_NAME, 
    TABLE_ROWS, 
    CREATE_TIME 
FROM information_schema.TABLES 
WHERE TABLE_SCHEMA = 'mes_prod' AND TABLE_NAME = 'mes_kv_store';
