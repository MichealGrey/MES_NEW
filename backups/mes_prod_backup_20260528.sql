-- MySQL dump 10.13  Distrib 9.7.0, for Win64 (x86_64)
--
-- Host: localhost    Database: mes_prod
-- ------------------------------------------------------
-- Server version	9.7.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
SET @MYSQLDUMP_TEMP_LOG_BIN = @@SESSION.SQL_LOG_BIN;
SET @@SESSION.SQL_LOG_BIN= 0;

--
-- GTID state at the beginning of the backup 
--

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ '43f2bf6a-5681-11f1-b9a9-00e04e5db3bf:1-1419';

--
-- Table structure for table `alarm_record`
--

DROP TABLE IF EXISTS `alarm_record`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `alarm_record` (
  `alarm_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `rule_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `equipment_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `alarm_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `severity` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `message` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Active',
  `acknowledged_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `acknowledged_at` datetime DEFAULT NULL,
  `resolved_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `resolved_at` datetime DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`alarm_id`),
  KEY `idx_rule_id` (`rule_id`),
  KEY `idx_lot_id` (`lot_id`),
  KEY `idx_status` (`status`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `alarm_record`
--

LOCK TABLES `alarm_record` WRITE;
/*!40000 ALTER TABLE `alarm_record` DISABLE KEYS */;
INSERT INTO `alarm_record` VALUES ('ALARM-001','AR-001','LOT-HOLD-001','DA-002','Yield','Critical','LOT-HOLD-001 DA工序良率低于95%','Acknowledged','USR-002','2026-05-21 21:38:15',NULL,NULL,'2026-05-25 21:38:15'),('ALARM-002','AR-004',NULL,'DA-004','Equipment','Warning','DA-004设备离线超过20天','Active',NULL,NULL,NULL,NULL,'2026-05-25 21:38:15'),('ALARM-003','AR-004',NULL,'WB-004','Equipment','Warning','WB-004设备离线超过30天','Active',NULL,NULL,NULL,NULL,'2026-05-25 21:38:15'),('ALARM-004','AR-001','LOT-007','SAW-003','Yield','Warning','LOT-007 SAW工序良率97.2%接近阈值','Resolved','USR-002','2026-05-24 21:38:15',NULL,NULL,'2026-05-25 21:38:15'),('ALARM-005','AR-002','LOT-HOLD-001',NULL,'HoldTimeout','Warning','LOT-HOLD-001 Hold超过24小时','Active',NULL,NULL,NULL,NULL,'2026-05-25 21:38:15');
/*!40000 ALTER TABLE `alarm_record` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `alarm_rule`
--

DROP TABLE IF EXISTS `alarm_rule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `alarm_rule` (
  `rule_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `rule_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `rule_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Yield/HoldTimeout/QueueTimeout/Equipment',
  `condition_expr` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `severity` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Warning',
  `notify_roles` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`rule_id`),
  KEY `idx_rule_type` (`rule_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `alarm_rule`
--

LOCK TABLES `alarm_rule` WRITE;
/*!40000 ALTER TABLE `alarm_rule` DISABLE KEYS */;
INSERT INTO `alarm_rule` VALUES ('AR-001','良率过低报警','Yield','yield < 95%','Critical','ROLE-QA,ROLE-ENG',1,'2026-05-25 21:37:43'),('AR-002','Hold超时报警','HoldTimeout','hold_duration > 24h','Warning','ROLE-QA,ROLE-MANAGER',1,'2026-05-25 21:37:43'),('AR-003','队列超时报警','QueueTimeout','queue_duration > 4h','Warning','ROLE-SUPERVISOR',1,'2026-05-25 21:37:43'),('AR-004','设备离线报警','Equipment','status = Offline','Warning','ROLE-ENG,ROLE-MAINT',1,'2026-05-25 21:37:43'),('AR-005','重工次数超限','Yield','rework_count > 2','Critical','ROLE-QA,ROLE-ENG',1,'2026-05-25 21:37:43');
/*!40000 ALTER TABLE `alarm_rule` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_requirement`
--

DROP TABLE IF EXISTS `customer_requirement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_requirement` (
  `requirement_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `customer_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `customer_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `requirement_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` text COLLATE utf8mb4_unicode_ci NOT NULL,
  `applicable_products` json DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`requirement_id`),
  KEY `idx_customer_id` (`customer_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_requirement`
--

LOCK TABLES `customer_requirement` WRITE;
/*!40000 ALTER TABLE `customer_requirement` DISABLE KEYS */;
INSERT INTO `customer_requirement` VALUES ('REQ-001','CUST-AUTO','某汽车电子','Quality','汽车电子良率要求>=98%',NULL,1,'2026-05-25 21:38:20'),('REQ-002','CUST-AUTO','某汽车电子','Traceability','要求全工序追溯',NULL,1,'2026-05-25 21:38:20'),('REQ-003','CUST-IND','某工业客户','Quality','工业级良率要求>=96%',NULL,1,'2026-05-25 21:38:20'),('REQ-004','CUST-CON','某消费电子','Quality','消费级良率要求>=95%',NULL,1,'2026-05-25 21:38:20'),('REQ-005','CUST-AUTO','某汽车电子','Packaging','要求防静电包装',NULL,1,'2026-05-25 21:38:20');
/*!40000 ALTER TABLE `customer_requirement` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_alarm_rule`
--

DROP TABLE IF EXISTS `master_alarm_rule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_alarm_rule` (
  `rule_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `alarm_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `severity` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Warning',
  `threshold_yield` decimal(5,2) DEFAULT NULL,
  `threshold_qty` int DEFAULT NULL,
  `threshold_minutes` int DEFAULT NULL,
  `is_enabled` tinyint(1) DEFAULT '1',
  `notify_role` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`rule_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_alarm_rule`
--

LOCK TABLES `master_alarm_rule` WRITE;
/*!40000 ALTER TABLE `master_alarm_rule` DISABLE KEYS */;
INSERT INTO `master_alarm_rule` VALUES ('ALARM-DELAY-001','Delay','Warning',NULL,NULL,30,1,'ShiftLead,Eng','2026-05-28 15:11:46'),('ALARM-EQ-001','Equipment','Critical',NULL,NULL,NULL,1,'Maint,Mgr','2026-05-28 15:11:46'),('ALARM-INV-001','Inventory','Warning',NULL,NULL,NULL,1,'Planner,Mgr','2026-05-28 15:11:46'),('ALARM-QUALITY-001','Quality','Error',NULL,5,NULL,1,'QA,Mgr','2026-05-28 15:11:46'),('ALARM-YIELD-001','Yield','Error',90.00,NULL,NULL,1,'QA,Eng,Mgr','2026-05-28 15:11:46');
/*!40000 ALTER TABLE `master_alarm_rule` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_carrier`
--

DROP TABLE IF EXISTS `master_carrier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_carrier` (
  `carrier_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `carrier_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Available',
  `current_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `capacity` int NOT NULL DEFAULT '0',
  `use_count` int NOT NULL DEFAULT '0',
  `max_use_count` int NOT NULL DEFAULT '0',
  `last_clean_date` datetime DEFAULT NULL,
  `clean_interval_uses` int NOT NULL DEFAULT '100',
  `location` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `applicable_process` json DEFAULT NULL,
  `applicable_package` json DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`carrier_id`),
  KEY `idx_carrier_type` (`carrier_type`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_carrier`
--

LOCK TABLES `master_carrier` WRITE;
/*!40000 ALTER TABLE `master_carrier` DISABLE KEYS */;
INSERT INTO `master_carrier` VALUES ('CARRIER-LEAD-001','LeadFrame','Available',NULL,1000,120,2000,NULL,100,'WH-B-01',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-LEAD-002','LeadFrame','InUse',NULL,1000,250,2000,NULL,100,'Line B-01',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-LEAD-003','LeadFrame','Available',NULL,1000,80,2000,NULL,100,'WH-B-02',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-LEAD-004','LeadFrame','Available',NULL,1000,400,2000,NULL,100,'WH-B-03',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-LEAD-005','LeadFrame','Available',NULL,1000,180,2000,NULL,100,'WH-B-04',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-LEAD-006','LeadFrame','InUse',NULL,1000,300,2000,NULL,100,'Line C-01',NULL,NULL,'2026-05-26 20:52:22'),('CARRIER-MAG-001','Magazine','Available',NULL,200,60,500,NULL,100,'WH-C-01',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-MAG-002','Magazine','InUse',NULL,200,100,500,NULL,100,'Line C-05',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-MAG-003','Magazine','Available',NULL,200,30,500,NULL,100,'WH-C-02',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-MAG-004','Magazine','Available',NULL,200,150,500,NULL,100,'WH-C-03',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-MAG-005','Magazine','Available',NULL,200,90,500,NULL,100,'WH-C-04',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-MAG-006','Magazine','Available',NULL,200,75,500,NULL,100,'WH-C-05',NULL,NULL,'2026-05-26 20:52:22'),('CARRIER-TRAY-001','Tray','Available',NULL,500,80,1000,NULL,100,'WH-A-01',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-TRAY-002','Tray','Available',NULL,500,150,1000,NULL,100,'WH-A-02',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-TRAY-003','Tray','InUse',NULL,500,200,1000,NULL,100,'Line A-05',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-TRAY-004','Tray','Available',NULL,500,50,1000,NULL,100,'WH-A-03',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-TRAY-005','Tray','Available',NULL,500,300,1000,NULL,100,'WH-A-04',NULL,NULL,'2026-05-25 21:13:05'),('CARRIER-TRAY-006','Tray','Available',NULL,500,100,1000,NULL,100,'WH-A-05',NULL,NULL,'2026-05-26 20:52:22');
/*!40000 ALTER TABLE `master_carrier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_customer`
--

DROP TABLE IF EXISTS `master_customer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_customer` (
  `customer_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `customer_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `customer_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `contact_person` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `contact_phone` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `email` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `address` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `customer_pn_prefix` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `quality_level` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Industrial',
  `special_requirements` json DEFAULT NULL,
  `default_packing_spec` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `default_oqc_spec` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT 'Active',
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`customer_id`),
  UNIQUE KEY `uk_customer_code` (`customer_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_customer`
--

LOCK TABLES `master_customer` WRITE;
/*!40000 ALTER TABLE `master_customer` DISABLE KEYS */;
INSERT INTO `master_customer` VALUES ('CUST-AUTO','某汽车电子','CUST-AUTO','张总','13800001111','auto@example.com',NULL,'AE-','Automotive',NULL,'PKG-AUTO-STD','OQC-AUTO-STD','Active','2026-05-28 15:11:46'),('CUST-CON','某消费电子','CUST-CON','王采购','13800003333','consumer@example.com',NULL,'CE-','Consumer',NULL,'PKG-CON-STD','OQC-CON-STD','Active','2026-05-28 15:11:46'),('CUST-IND','某工业客户','CUST-IND','李经理','13800002222','industrial@example.com',NULL,'IND-','Industrial',NULL,'PKG-IND-STD','OQC-IND-STD','Active','2026-05-28 15:11:46');
/*!40000 ALTER TABLE `master_customer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_defect_code`
--

DROP TABLE IF EXISTS `master_defect_code`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_defect_code` (
  `defect_code_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `defect_category` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `defect_text` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `severity` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT 'Major',
  `is_enabled` tinyint(1) DEFAULT '1',
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`defect_code_id`),
  KEY `idx_category` (`defect_category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_defect_code`
--

LOCK TABLES `master_defect_code` WRITE;
/*!40000 ALTER TABLE `master_defect_code` DISABLE KEYS */;
INSERT INTO `master_defect_code` VALUES ('DEF-COS-001','Cosmetic','表面划伤','Major',1,'2026-05-28 15:11:46'),('DEF-COS-002','Cosmetic','脏污/异物','Minor',1,'2026-05-28 15:11:46'),('DEF-COS-003','Cosmetic','颜色不均','Minor',1,'2026-05-28 15:11:46'),('DEF-COS-004','Cosmetic','标记模糊','Major',1,'2026-05-28 15:11:46'),('DEF-COS-005','Cosmetic','引脚变形','Major',1,'2026-05-28 15:11:46'),('DEF-COS-006','Cosmetic','裂纹/破损','Critical',1,'2026-05-28 15:11:46'),('DEF-COS-007','Cosmetic','气泡','Minor',1,'2026-05-28 15:11:46'),('DEF-COS-008','Cosmetic','缺胶/填充不足','Major',1,'2026-05-28 15:11:46'),('DEF-DIM-001','Dimensional','外形尺寸超差','Major',1,'2026-05-28 15:11:46'),('DEF-DIM-002','Dimensional','引脚间距超差','Critical',1,'2026-05-28 15:11:46'),('DEF-DIM-003','Dimensional','厚度超差','Major',1,'2026-05-28 15:11:46'),('DEF-DIM-004','Dimensional','共面度超差','Critical',1,'2026-05-28 15:11:46'),('DEF-DIM-005','Dimensional','切割偏移','Major',1,'2026-05-28 15:11:47'),('DEF-ELE-001','Electrical','开路','Critical',1,'2026-05-28 15:11:47'),('DEF-ELE-002','Electrical','短路','Critical',1,'2026-05-28 15:11:47'),('DEF-ELE-003','Electrical','漏电超标','Critical',1,'2026-05-28 15:11:47'),('DEF-ELE-004','Electrical','参数漂移','Major',1,'2026-05-28 15:11:47'),('DEF-ELE-005','Electrical','耐压不良','Critical',1,'2026-05-28 15:11:47'),('DEF-ELE-006','Electrical','接触电阻超标','Major',1,'2026-05-28 15:11:47'),('DEF-ELE-007','Electrical','功能失效','Critical',1,'2026-05-28 15:11:47'),('DEF-FUN-001','Functional','信号传输异常','Critical',1,'2026-05-28 15:11:47'),('DEF-FUN-002','Functional','频率响应异常','Major',1,'2026-05-28 15:11:47'),('DEF-FUN-003','Functional','逻辑功能失效','Critical',1,'2026-05-28 15:11:47'),('DEF-FUN-004','Functional','温度特性不良','Major',1,'2026-05-28 15:11:47'),('DEF-FUN-005','Functional','ESD损坏','Critical',1,'2026-05-28 15:11:47');
/*!40000 ALTER TABLE `master_defect_code` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_equipment`
--

DROP TABLE IF EXISTS `master_equipment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_equipment` (
  `equipment_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `equipment_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `equipment_group` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `equipment_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `process_stage` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT 'Assemble',
  `vendor` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `model` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `serial_number` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `capability` json DEFAULT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Available',
  `current_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `current_recipe` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `location` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `responsible_person` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `last_maintenance_date` datetime DEFAULT NULL,
  `maintenance_interval_hours` int NOT NULL DEFAULT '500',
  `running_hours` int NOT NULL DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`equipment_id`),
  KEY `idx_equipment_group` (`equipment_group`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_equipment`
--

LOCK TABLES `master_equipment` WRITE;
/*!40000 ALTER TABLE `master_equipment` DISABLE KEYS */;
INSERT INTO `master_equipment` VALUES ('BALL-001','植球机 #1','BALL','BallMount','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line E-01','马维护','2026-05-07 21:13:05',700,200,'2026-05-25 21:13:05'),('BALL-002','植球机 #2','BALL','BallMount','Assemble',NULL,NULL,NULL,NULL,'Running',NULL,NULL,'Line E-02','马维护','2026-05-20 21:13:05',700,450,'2026-05-25 21:13:05'),('CLEAN-001','Cleaner #1','CLEAN','Cleaner','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line G-02','USR-015','2026-05-21 20:52:22',400,150,'2026-05-26 20:52:22'),('CURE-001','固化炉 #1','CURE','CuringOven','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line F-01','蒋固化','2026-05-10 21:13:05',1000,200,'2026-05-25 21:13:05'),('CURE-002','固化炉 #2','CURE','CuringOven','Assemble',NULL,NULL,NULL,NULL,'Running',NULL,NULL,'Line F-02','蒋固化','2026-05-17 21:13:05',1000,500,'2026-05-25 21:13:05'),('DA-001','贴片机 #1','DA','DieBonder','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line A-05','周工','2026-05-20 21:13:05',400,200,'2026-05-25 21:13:05'),('DA-002','贴片机 #2','DA','DieBonder','Assemble',NULL,NULL,NULL,NULL,'Running',NULL,NULL,'Line A-06','周工','2026-05-24 21:13:05',400,380,'2026-05-25 21:13:05'),('DA-003','贴片机 #3','DA','DieBonder','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line A-07','周工','2026-05-17 21:13:05',400,150,'2026-05-25 21:13:05'),('DA-004','贴片机 #4','DA','DieBonder','Assemble',NULL,NULL,NULL,NULL,'Offline',NULL,NULL,'Line A-08','周工','2026-05-05 21:13:05',400,395,'2026-05-25 21:13:05'),('DEBOND-001','Debonder #1','DEBOND','Debonder','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line G-01','USR-015','2026-05-16 20:52:22',600,200,'2026-05-26 20:52:22'),('DECAP-001','Decapsulator #1','DECAP','Decapsulator','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line G-03','USR-015','2026-05-18 20:52:22',500,100,'2026-05-26 20:52:22'),('FT-001','测试机 #1','TEST','TestHandler','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line C-05','陈工','2026-05-21 21:13:05',300,180,'2026-05-25 21:13:05'),('FT-002','测试机 #2','TEST','TestHandler','Assemble',NULL,NULL,NULL,NULL,'Running',NULL,NULL,'Line C-06','陈工','2026-05-24 21:13:05',300,280,'2026-05-25 21:13:05'),('FT-003','测试机 #3','TEST','TestHandler','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line C-07','陈工','2026-05-19 21:13:05',300,90,'2026-05-25 21:13:05'),('FT-004','测试机 #4','TEST','TestHandler','Assemble',NULL,NULL,NULL,NULL,'Maintenance',NULL,NULL,'Line C-08','陈工','2026-05-24 21:13:05',300,295,'2026-05-25 21:13:05'),('LASER-001','激光打标机 #1','LASER','LaserMark','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line C-01','陈工','2026-05-05 21:13:05',1000,100,'2026-05-25 21:13:05'),('LASER-002','激光打标机 #2','LASER','LaserMark','Assemble',NULL,NULL,NULL,NULL,'Running',NULL,NULL,'Line C-02','陈工','2026-05-13 21:13:05',1000,400,'2026-05-25 21:13:05'),('MOLD-001','塑封机 #1','MOLD','MoldingPress','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line B-06','郑工','2026-05-11 21:13:05',800,300,'2026-05-25 21:13:05'),('MOLD-002','塑封机 #2','MOLD','MoldingPress','Assemble',NULL,NULL,NULL,NULL,'Running',NULL,NULL,'Line B-07','郑工','2026-05-17 21:13:05',800,500,'2026-05-25 21:13:05'),('MOLD-003','塑封机 #3','MOLD','MoldingPress','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line B-08','郑工','2026-05-23 21:13:05',800,720,'2026-05-25 21:13:05'),('OQC-001','出货检验台 #1','OQC','AOI','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line D-01','李品质','2026-04-25 21:13:05',2000,50,'2026-05-25 21:13:05'),('OQC-002','出货检验台 #2','OQC','AOI','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line D-02','钱检验','2026-04-30 21:13:05',2000,80,'2026-05-25 21:13:05'),('PMC-001','后固化炉 #1','PMC','PostCureOven','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line F-03','蒋固化','2026-05-13 21:13:05',1200,300,'2026-05-25 21:13:05'),('PMC-002','后固化炉 #2','PMC','PostCureOven','Assemble',NULL,NULL,NULL,NULL,'Running',NULL,NULL,'Line F-04','蒋固化','2026-05-22 21:13:05',1200,800,'2026-05-25 21:13:05'),('REFLOW-001','回流焊 #1','REFLOW','ReflowOven','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line E-03','马维护','2026-05-03 21:13:05',900,150,'2026-05-25 21:13:05'),('REFLOW-002','回流焊 #2','REFLOW','ReflowOven','Assemble',NULL,NULL,NULL,NULL,'Running',NULL,NULL,'Line E-04','马维护','2026-05-15 21:13:05',900,600,'2026-05-25 21:13:05'),('SAW-001','切割机 #1','SAW','DicingSaw','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line A-01','张工','2026-05-18 21:13:05',500,120,'2026-05-25 21:13:05'),('SAW-002','切割机 #2','SAW','DicingSaw','Assemble',NULL,NULL,NULL,NULL,'Running',NULL,NULL,'Line A-02','张工','2026-05-22 21:13:05',500,350,'2026-05-25 21:13:05'),('SAW-003','切割机 #3','SAW','DicingSaw','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line A-03','张工','2026-05-15 21:13:05',500,80,'2026-05-25 21:13:05'),('SAW-004','切割机 #4','SAW','DicingSaw','Assemble',NULL,NULL,NULL,NULL,'Maintenance',NULL,NULL,'Line A-04','张工','2026-05-24 21:13:05',500,490,'2026-05-25 21:13:05'),('SING-001','切割分条机 #1','SING','TrimForm','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line C-03','陈工','2026-05-19 21:13:05',500,250,'2026-05-25 21:13:05'),('SING-002','切割分条机 #2','SING','TrimForm','Assemble',NULL,NULL,NULL,NULL,'Running',NULL,NULL,'Line C-04','陈工','2026-05-24 21:13:05',500,480,'2026-05-25 21:13:05'),('WB-001','焊线机 #1','WB','WireBonder','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line B-01','吴工','2026-05-15 21:13:05',600,150,'2026-05-25 21:13:05'),('WB-002','焊线机 #2','WB','WireBonder','Assemble',NULL,NULL,NULL,NULL,'Running',NULL,NULL,'Line B-02','吴工','2026-05-23 21:13:05',600,420,'2026-05-25 21:13:05'),('WB-003','焊线机 #3','WB','WireBonder','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line B-03','吴工','2026-05-10 21:13:05',600,50,'2026-05-25 21:13:05'),('WB-004','焊线机 #4','WB','WireBonder','Assemble',NULL,NULL,NULL,NULL,'Offline',NULL,NULL,'Line B-04','吴工','2026-04-25 21:13:05',600,580,'2026-05-25 21:13:05'),('WB-005','焊线机 #5','WB','WireBonder','Assemble',NULL,NULL,NULL,NULL,'Available',NULL,NULL,'Line B-05','吴工','2026-05-20 21:13:05',600,280,'2026-05-25 21:13:05');
/*!40000 ALTER TABLE `master_equipment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_equipment_route`
--

DROP TABLE IF EXISTS `master_equipment_route`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_equipment_route` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `equipment_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `route_id` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_equip_route` (`equipment_id`,`route_id`),
  KEY `idx_equipment_id` (`equipment_id`)
) ENGINE=InnoDB AUTO_INCREMENT=118 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_equipment_route`
--

LOCK TABLES `master_equipment_route` WRITE;
/*!40000 ALTER TABLE `master_equipment_route` DISABLE KEYS */;
INSERT INTO `master_equipment_route` VALUES (43,'BALL-001','BGA-STD:1.0'),(80,'BALL-001','BGA64-STD:1.0'),(37,'DA-001','BGA-STD:1.0'),(74,'DA-001','BGA64-STD:1.0'),(4,'DA-001','QFN-STD:2.0'),(62,'DA-001','QFN48-STD:1.0'),(85,'DA-001','QFN64-STD:1.0'),(51,'DA-001','RW-DA:1.0'),(24,'DA-001','SOP-STD:1.0'),(107,'DA-001','SOP14-STD:1.0'),(96,'DA-001','SOP8-STD:1.0'),(38,'DA-002','BGA-STD:1.0'),(75,'DA-002','BGA64-STD:1.0'),(5,'DA-002','QFN-STD:2.0'),(52,'DA-002','RW-DA:1.0'),(6,'DA-003','QFN-STD:2.0'),(63,'DA-003','QFN48-STD:1.0'),(86,'DA-003','QFN64-STD:1.0'),(53,'DA-003','RW-DA:1.0'),(25,'DA-003','SOP-STD:1.0'),(108,'DA-003','SOP14-STD:1.0'),(97,'DA-003','SOP8-STD:1.0'),(47,'FT-001','BGA-STD:1.0'),(81,'FT-001','BGA64-STD:1.0'),(17,'FT-001','QFN-STD:2.0'),(70,'FT-001','QFN48-STD:1.0'),(93,'FT-001','QFN64-STD:1.0'),(32,'FT-001','SOP-STD:1.0'),(115,'FT-001','SOP14-STD:1.0'),(104,'FT-001','SOP8-STD:1.0'),(48,'FT-002','BGA-STD:1.0'),(18,'FT-002','QFN-STD:2.0'),(94,'FT-002','QFN64-STD:1.0'),(19,'FT-003','QFN-STD:2.0'),(71,'FT-003','QFN48-STD:1.0'),(33,'FT-003','SOP-STD:1.0'),(116,'FT-003','SOP14-STD:1.0'),(105,'FT-003','SOP8-STD:1.0'),(45,'LASER-001','BGA-STD:1.0'),(13,'LASER-001','QFN-STD:2.0'),(68,'LASER-001','QFN48-STD:1.0'),(91,'LASER-001','QFN64-STD:1.0'),(30,'LASER-001','SOP-STD:1.0'),(113,'LASER-001','SOP14-STD:1.0'),(102,'LASER-001','SOP8-STD:1.0'),(46,'LASER-002','BGA-STD:1.0'),(14,'LASER-002','QFN-STD:2.0'),(41,'MOLD-001','BGA-STD:1.0'),(78,'MOLD-001','BGA64-STD:1.0'),(10,'MOLD-001','QFN-STD:2.0'),(66,'MOLD-001','QFN48-STD:1.0'),(89,'MOLD-001','QFN64-STD:1.0'),(57,'MOLD-001','RW-MOLD:1.0'),(28,'MOLD-001','SOP-STD:1.0'),(111,'MOLD-001','SOP14-STD:1.0'),(100,'MOLD-001','SOP8-STD:1.0'),(11,'MOLD-002','QFN-STD:2.0'),(67,'MOLD-002','QFN48-STD:1.0'),(90,'MOLD-002','QFN64-STD:1.0'),(58,'MOLD-002','RW-MOLD:1.0'),(29,'MOLD-002','SOP-STD:1.0'),(112,'MOLD-002','SOP14-STD:1.0'),(101,'MOLD-002','SOP8-STD:1.0'),(42,'MOLD-003','BGA-STD:1.0'),(79,'MOLD-003','BGA64-STD:1.0'),(12,'MOLD-003','QFN-STD:2.0'),(59,'MOLD-003','RW-MOLD:1.0'),(49,'OQC-001','BGA-STD:1.0'),(82,'OQC-001','BGA64-STD:1.0'),(20,'OQC-001','QFN-STD:2.0'),(95,'OQC-001','QFN64-STD:1.0'),(34,'OQC-001','SOP-STD:1.0'),(117,'OQC-001','SOP14-STD:1.0'),(106,'OQC-001','SOP8-STD:1.0'),(50,'OQC-002','BGA-STD:1.0'),(21,'OQC-002','QFN-STD:2.0'),(44,'REFLOW-001','BGA-STD:1.0'),(35,'SAW-001','BGA-STD:1.0'),(72,'SAW-001','BGA64-STD:1.0'),(1,'SAW-001','QFN-STD:2.0'),(60,'SAW-001','QFN48-STD:1.0'),(83,'SAW-001','QFN64-STD:1.0'),(22,'SAW-001','SOP-STD:1.0'),(2,'SAW-002','QFN-STD:2.0'),(61,'SAW-002','QFN48-STD:1.0'),(84,'SAW-002','QFN64-STD:1.0'),(23,'SAW-002','SOP-STD:1.0'),(36,'SAW-003','BGA-STD:1.0'),(73,'SAW-003','BGA64-STD:1.0'),(3,'SAW-003','QFN-STD:2.0'),(15,'SING-001','QFN-STD:2.0'),(69,'SING-001','QFN48-STD:1.0'),(92,'SING-001','QFN64-STD:1.0'),(31,'SING-001','SOP-STD:1.0'),(114,'SING-001','SOP14-STD:1.0'),(103,'SING-001','SOP8-STD:1.0'),(16,'SING-002','QFN-STD:2.0'),(39,'WB-001','BGA-STD:1.0'),(76,'WB-001','BGA64-STD:1.0'),(7,'WB-001','QFN-STD:2.0'),(64,'WB-001','QFN48-STD:1.0'),(87,'WB-001','QFN64-STD:1.0'),(54,'WB-001','RW-WB:1.0'),(26,'WB-001','SOP-STD:1.0'),(109,'WB-001','SOP14-STD:1.0'),(98,'WB-001','SOP8-STD:1.0'),(8,'WB-002','QFN-STD:2.0'),(65,'WB-002','QFN48-STD:1.0'),(88,'WB-002','QFN64-STD:1.0'),(55,'WB-002','RW-WB:1.0'),(27,'WB-002','SOP-STD:1.0'),(110,'WB-002','SOP14-STD:1.0'),(99,'WB-002','SOP8-STD:1.0'),(40,'WB-003','BGA-STD:1.0'),(77,'WB-003','BGA64-STD:1.0'),(9,'WB-003','QFN-STD:2.0'),(56,'WB-003','RW-WB:1.0');
/*!40000 ALTER TABLE `master_equipment_route` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_material`
--

DROP TABLE IF EXISTS `master_material`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_material` (
  `material_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `material_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `material_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `specification` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `unit` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'pcs',
  `supplier` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `min_stock` int DEFAULT '0',
  `current_stock` int DEFAULT '0',
  `is_active` tinyint(1) DEFAULT '1',
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`material_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_material`
--

LOCK TABLES `master_material` WRITE;
/*!40000 ALTER TABLE `master_material` DISABLE KEYS */;
INSERT INTO `master_material` VALUES ('MAT-BLADE-1','晶圆切割刀片','Blade','Dicing Blade 0.3mm Ni Bond','pcs','Disco',10,50,1,'2026-05-28 15:11:46'),('MAT-BLADE-2','塑封切割刀片','Blade','Dicing Blade 0.5mm Resin','pcs','Disco',10,40,1,'2026-05-28 15:11:46'),('MAT-DA-AG','导电银浆','DieAttach','Ag Epoxy DA-3000','roll','汉高',5,30,1,'2026-05-28 15:11:46'),('MAT-DA-DAF','Die Attach Film','DieAttach','DAF-500 25μm','roll','日东电工',5,25,1,'2026-05-28 15:11:46'),('MAT-DESICCANT','干燥剂','Packaging','50g Silica Gel Pack','pcs','通用',500,3000,1,'2026-05-28 15:11:46'),('MAT-EMC-LOW','低应力塑封料','EMC','EMC-LS300 Green','kg','住友电木',30,100,1,'2026-05-28 15:11:46'),('MAT-EMC-STD','标准型环氧塑封料','EMC','EMC-GP500 Black','kg','住友电木',50,200,1,'2026-05-28 15:11:46'),('MAT-HUMIDITY-CARD','湿度指示卡','Packaging','3-Spot 10%/60%/90%','pcs','通用',200,1000,1,'2026-05-28 15:11:46'),('MAT-LF-QFN88','QFN88 引线框架','LeadFrame','48x48x0.25mm Cu Alloy C194','strip','三井高科技',500,2500,1,'2026-05-28 15:11:46'),('MAT-LF-QFP64','QFP64 引线框架','LeadFrame','32x32x0.25mm Cu Alloy C194','strip','日东电工',300,1500,1,'2026-05-28 15:11:46'),('MAT-LF-SOP8','SOP8 引线框架','LeadFrame','20x10x0.25mm Cu Alloy C194','strip','三井高科技',1000,5000,1,'2026-05-28 15:11:46'),('MAT-REEL-13','13英寸卷盘','Packaging','13 inch Reel 8mm PS','pcs','通用',50,300,1,'2026-05-28 15:11:46'),('MAT-REEL-7','7英寸卷盘','Packaging','7 inch Reel 4mm Paper','pcs','通用',100,500,1,'2026-05-28 15:11:46'),('MAT-SUB-BGA256','BGA256 基板','Substrate','40x40mm 4-Layer FR4','panel','新光电气',200,800,1,'2026-05-28 15:11:46'),('MAT-SUB-BGA512','BGA512 基板','Substrate','50x50mm 6-Layer BT','panel','新光电气',100,400,1,'2026-05-28 15:11:46'),('MAT-TAPE-12MM','12mm 载带','Packaging','12mm Embossed Tape PS','reel','三井高科技',30,200,1,'2026-05-28 15:11:46'),('MAT-TAPE-16MM','16mm 载带','Packaging','16mm Embossed Tape PS','reel','三井高科技',20,150,1,'2026-05-28 15:11:46'),('MAT-TAPE-8MM','8mm 载带','Packaging','8mm Embossed Tape PS','reel','三井高科技',50,300,1,'2026-05-28 15:11:46'),('MAT-WIRE-AU-08','金线 0.8mil','Wire','99.99% Au 0.8mil','reel','贺利氏',20,100,1,'2026-05-28 15:11:46'),('MAT-WIRE-AU-10','金线 1.0mil','Wire','99.99% Au 1.0mil','reel','贺利氏',20,80,1,'2026-05-28 15:11:46'),('MAT-WIRE-CU-08','铜线 0.8mil','Wire','Cu 0.8mil Pd Coated','reel','田中贵金属',15,60,1,'2026-05-28 15:11:46');
/*!40000 ALTER TABLE `master_material` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_product`
--

DROP TABLE IF EXISTS `master_product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_product` (
  `product_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `product_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `die_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `package_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `process_stage` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT 'Assemble',
  `default_route_id` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `unit_qty` int DEFAULT '1',
  `customer_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `customer_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `customer_pn` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `internal_pn` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Active',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`product_id`),
  KEY `idx_customer_id` (`customer_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_product`
--

LOCK TABLES `master_product` WRITE;
/*!40000 ALTER TABLE `master_product` DISABLE KEYS */;
INSERT INTO `master_product` VALUES ('PROD-BGA256','BGA-256 处理器','CPU-3000','BGA','Assemble',NULL,1,'CUST-AUTO','某汽车电子','AE-BGA256-003','PN-BGA256-HI','Active','2026-05-25 21:13:05'),('PROD-BGA64','BGA-64 存储器','MEM-500','BGA','Assemble',NULL,1,'CUST-AUTO','某汽车电子','AE-BGA64-006','PN-BGA64-STD','Active','2026-05-25 21:13:05'),('PROD-QFN48','QFN-48 传感器','SENS-100','QFN','Assemble',NULL,1,'CUST-CON','某消费电子','CE-QFN48-004','PN-QFN48-STD','Active','2026-05-25 21:13:05'),('PROD-QFN64','QFN-64 驱动器','DRV-300','QFN','Assemble',NULL,1,'CUST-AUTO','某汽车电子','AE-QFN64-007','PN-QFN64-STD','Active','2026-05-25 21:13:05'),('PROD-QFN88','QFN-88 控制器','CTRL-2024','QFN','Assemble',NULL,1,'CUST-AUTO','某汽车电子','AE-QFN88-001','PN-QFN88-STD','Active','2026-05-25 21:13:05'),('PROD-SOP14','SOP-14 运放','OP-100','SOP','Assemble',NULL,1,'CUST-CON','某消费电子','CE-SOP14-008','PN-SOP14-STD','Active','2026-05-25 21:13:05'),('PROD-SOP16','SOP-16 电源IC','PWR-500','SOP','Assemble',NULL,1,'CUST-IND','某工业客户','IND-SOP16-002','PN-SOP16-STD','Active','2026-05-25 21:13:05'),('PROD-SOP8','SOP-8 MOSFET','MOS-200','SOP','Assemble',NULL,1,'CUST-IND','某工业客户','IND-SOP8-005','PN-SOP8-STD','Active','2026-05-25 21:13:05');
/*!40000 ALTER TABLE `master_product` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_reason_code`
--

DROP TABLE IF EXISTS `master_reason_code`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_reason_code` (
  `reason_code_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `category` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `sub_category` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `reason_text` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `applicable_to` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `is_enabled` tinyint(1) DEFAULT '1',
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`reason_code_id`),
  KEY `idx_category` (`category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_reason_code`
--

LOCK TABLES `master_reason_code` WRITE;
/*!40000 ALTER TABLE `master_reason_code` DISABLE KEYS */;
INSERT INTO `master_reason_code` VALUES ('RSN-EQ-001','Equipment','设备故障','设备突发故障，需维修','Hold',1,'2026-05-28 15:11:46'),('RSN-EQ-002','Equipment','保养中','设备定期保养','Hold',1,'2026-05-28 15:11:46'),('RSN-EQ-003','Equipment','参数异常','设备运行参数超出标准范围','Hold',1,'2026-05-28 15:11:46'),('RSN-EQ-004','Equipment','耗材耗尽','设备耗材耗尽需要更换','Hold',1,'2026-05-28 15:11:46'),('RSN-EQ-005','Equipment','设备损坏产品','设备异常导致产品损坏','Scrap',1,'2026-05-28 15:11:46'),('RSN-MAT-001','Material','来料不良','原材料来料不良，供应商责任','Hold',1,'2026-05-28 15:11:46'),('RSN-MAT-002','Material','材料混批','不同批次材料混用','Hold',1,'2026-05-28 15:11:46'),('RSN-MAT-003','Material','材料过期','材料超过有效期','Scrap',1,'2026-05-28 15:11:46'),('RSN-MAT-004','Material','材料短缺','生产中发现材料不足','Hold',1,'2026-05-28 15:11:46'),('RSN-MAT-005','Material','材料用错','使用了错误的材料型号','Rework',1,'2026-05-28 15:11:46'),('RSN-OTH-001','Other','工程实验','工程实验需要','Hold',1,'2026-05-28 15:11:46'),('RSN-OTH-002','Other','客户特殊要求','客户特殊要求暂停','Hold',1,'2026-05-28 15:11:46'),('RSN-OTH-003','Other','其他','其他原因','Hold',1,'2026-05-28 15:11:46'),('RSN-PER-001','Personnel','操作失误','人员操作失误','Rework',1,'2026-05-28 15:11:46'),('RSN-PER-002','Personnel','未培训上岗','操作人员未完成培训','Hold',1,'2026-05-28 15:11:46'),('RSN-PRC-001','Process','工艺变更','客户/工程工艺变更通知','Hold',1,'2026-05-28 15:11:46'),('RSN-PRC-002','Process','参数错误','使用了错误的工艺参数','Rework',1,'2026-05-28 15:11:46'),('RSN-PRC-003','Process','超时滞留','批次在某工序超时滞留','Hold',1,'2026-05-28 15:11:46'),('RSN-PRC-004','Process','漏工序','批次漏做了某个工序','Rework',1,'2026-05-28 15:11:46'),('RSN-PRC-005','Process','工艺违规','违反工艺操作规范','Scrap',1,'2026-05-28 15:11:46'),('RSN-QA-001','Quality','良率超标','工序良率低于控制下限','Hold',1,'2026-05-28 15:11:46'),('RSN-QA-002','Quality','检验不合格','品质检验判定不合格','Hold',1,'2026-05-28 15:11:46'),('RSN-QA-003','Quality','客户投诉','客户品质投诉，需追溯调查','Hold',1,'2026-05-28 15:11:46'),('RSN-QA-004','Quality','SPC异常','SPC监控发现异常趋势','Hold',1,'2026-05-28 15:11:46'),('RSN-QA-005','Quality','外观缺陷','产品外观存在不可接受缺陷','Scrap',1,'2026-05-28 15:11:46'),('RSN-QA-006','Quality','电性不良','产品电性参数不达标','Scrap',1,'2026-05-28 15:11:46');
/*!40000 ALTER TABLE `master_reason_code` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_recipe`
--

DROP TABLE IF EXISTS `master_recipe`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_recipe` (
  `recipe_id` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `recipe_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `equipment_group` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `product_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `version` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '1.0',
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `parameters` json DEFAULT NULL COMMENT '参数JSON',
  `approved_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `approved_at` datetime DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`recipe_id`),
  KEY `idx_equipment_group` (`equipment_group`),
  KEY `idx_product_id` (`product_id`),
  KEY `idx_step_code` (`step_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_recipe`
--

LOCK TABLES `master_recipe` WRITE;
/*!40000 ALTER TABLE `master_recipe` DISABLE KEYS */;
INSERT INTO `master_recipe` VALUES ('REC-DA-QFN-V1','QFN贴装参数','DA','','DA','V1',1,'{\"bondTemp\": 250, \"bondForce\": 5, \"pickDelay\": 50}',NULL,NULL,'2026-05-28 15:11:46'),('REC-FT-QFN-V1','QFN成品测试参数','TEST','','FT','V1',1,'{\"testCurrent\": 0.01, \"testVoltage\": 3.3, \"testFrequency\": 1000}',NULL,NULL,'2026-05-28 15:11:46'),('REC-LASER-QFN-V1','QFN打标参数','LASER','','LASER','V1',1,'{\"frequency\": 20, \"markSpeed\": 500, \"laserPower\": 80}',NULL,NULL,'2026-05-28 15:11:46'),('REC-MOLD-QFN-V1','QFN塑封参数','MOLD','','MOLD','V1',1,'{\"cureTime\": 120, \"moldTemp\": 175, \"moldPressure\": 8}',NULL,NULL,'2026-05-28 15:11:46'),('REC-OQC-QFN-V1','QFN出货检验标准','OQC','','OQC','V1',1,'{\"dimAQL\": 1.0, \"visualAQL\": 0.65, \"sampleSize\": \"AQL-II\"}',NULL,NULL,'2026-05-28 15:11:46'),('REC-PACK-QFN-V1','QFN包装参数','PACK','','PACK','V1',1,'{\"reelSize\": 13, \"sealTemp\": 150, \"tapeType\": \"Embossed\"}',NULL,NULL,'2026-05-28 15:11:46'),('REC-PMC-QFN-V1','QFN固化参数','CURE','','PMC','V1',1,'{\"cureTemp\": 175, \"cureTime\": 7200}',NULL,NULL,'2026-05-28 15:11:46'),('REC-SAW-QFN-V1','QFN切割参数','SAW','','SAW','V1',1,'{\"cutDepth\": 0.3, \"cutSpeed\": 15, \"spindleSpeed\": 30000}',NULL,NULL,'2026-05-28 15:11:46'),('REC-SING-QFN-V1','QFN切筋参数','SING','','SING','V1',1,'{\"cutSpeed\": 20, \"cutPressure\": 5}',NULL,NULL,'2026-05-28 15:11:46'),('REC-WB-QFN-V1','QFN引线键合参数','WB','','WB','V1',1,'{\"bondTime\": 50, \"bondForce\": 150, \"wireDiameter\": 0.8, \"wireMaterial\": \"Au\", \"ultrasonicPower\": 80}',NULL,NULL,'2026-05-28 15:11:46');
/*!40000 ALTER TABLE `master_recipe` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_route`
--

DROP TABLE IF EXISTS `master_route`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_route` (
  `route_id` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `route_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `route_version` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '1.0',
  `product_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `package_type` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `is_approved` tinyint(1) NOT NULL DEFAULT '0',
  `approved_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `approved_at` datetime DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`route_id`),
  KEY `idx_product_id` (`product_id`),
  KEY `idx_is_active` (`is_active`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_route`
--

LOCK TABLES `master_route` WRITE;
/*!40000 ALTER TABLE `master_route` DISABLE KEYS */;
INSERT INTO `master_route` VALUES ('BGA-STD:1.0','BGA 标准路线','1.0','PROD-BGA256','BGA',1,1,'USR-003','2026-04-10 21:13:05','2026-05-25 21:13:05'),('BGA64-STD:1.0','BGA-64 标准路线','1.0','PROD-BGA64','BGA',1,1,'USR-003','2026-05-10 21:13:05','2026-05-25 21:13:05'),('QFN-STD:2.0','QFN 标准路线','2.0','PROD-QFN88','QFN',1,1,'USR-003','2026-04-25 21:13:05','2026-05-25 21:13:05'),('QFN48-STD:1.0','QFN-48 标准路线','1.0','PROD-QFN48','QFN',1,1,'USR-003','2026-05-05 21:13:05','2026-05-25 21:13:05'),('QFN64-STD:1.0','QFN-64 标准路线','1.0','PROD-QFN64','QFN',1,1,'USR-003','2026-04-30 21:13:05','2026-05-25 21:13:05'),('RW-DA:1.0','DieAttach 重工路线','1.0','PROD-QFN88','QFN',1,1,'USR-003','2026-05-10 21:13:05','2026-05-25 21:13:05'),('RW-MOLD:1.0','Mold 重工路线','1.0','PROD-QFN88','QFN',1,1,'USR-003','2026-05-15 21:13:05','2026-05-25 21:13:05'),('RW-WB:1.0','WireBond 重工路线','1.0','PROD-QFN88','QFN',1,1,'USR-003','2026-05-10 21:13:05','2026-05-25 21:13:05'),('SOP-STD:1.0','SOP 标准路线','1.0','PROD-SOP16','SOP',1,1,'USR-003','2026-03-26 21:13:05','2026-05-25 21:13:05'),('SOP14-STD:1.0','SOP-14 标准路线','1.0','PROD-SOP14','SOP',1,1,'USR-003','2026-04-15 21:13:05','2026-05-25 21:13:05'),('SOP8-STD:1.0','SOP-8 标准路线','1.0','PROD-SOP8','SOP',1,1,'USR-003','2026-02-24 21:13:05','2026-05-25 21:13:05');
/*!40000 ALTER TABLE `master_route` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_route_step`
--

DROP TABLE IF EXISTS `master_route_step`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_route_step` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `route_id` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_seq` int NOT NULL,
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `equipment_group` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_rework` tinyint(1) NOT NULL DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_route_step` (`route_id`,`step_seq`),
  KEY `idx_route_id` (`route_id`),
  KEY `idx_step_code` (`step_code`)
) ENGINE=InnoDB AUTO_INCREMENT=89 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_route_step`
--

LOCK TABLES `master_route_step` WRITE;
/*!40000 ALTER TABLE `master_route_step` DISABLE KEYS */;
INSERT INTO `master_route_step` VALUES (1,'QFN-STD:2.0',1,'SAW','晶圆切割','SAW',0,'2026-05-25 21:13:05'),(2,'QFN-STD:2.0',2,'DA','芯片贴装','DA',0,'2026-05-25 21:13:05'),(3,'QFN-STD:2.0',3,'CURE','固化','CURE',0,'2026-05-25 21:13:05'),(4,'QFN-STD:2.0',4,'WB','焊线','WB',0,'2026-05-25 21:13:05'),(5,'QFN-STD:2.0',5,'MOLD','塑封','MOLD',0,'2026-05-25 21:13:05'),(6,'QFN-STD:2.0',6,'PMC','后固化','PMC',0,'2026-05-25 21:13:05'),(7,'QFN-STD:2.0',7,'MARK','激光打标','LASER',0,'2026-05-25 21:13:05'),(8,'QFN-STD:2.0',8,'SING','切割分条','SING',0,'2026-05-25 21:13:05'),(9,'QFN-STD:2.0',9,'FT','最终测试','TEST',0,'2026-05-25 21:13:05'),(10,'QFN-STD:2.0',10,'OQC','出货检验','OQC',0,'2026-05-25 21:13:05'),(11,'QFN-STD:2.0',11,'PACK','包装入库','PACK',0,'2026-05-25 21:13:05'),(12,'SOP-STD:1.0',1,'SAW','晶圆切割','SAW',0,'2026-05-25 21:13:05'),(13,'SOP-STD:1.0',2,'DA','芯片贴装','DA',0,'2026-05-25 21:13:05'),(14,'SOP-STD:1.0',3,'WB','焊线','WB',0,'2026-05-25 21:13:05'),(15,'SOP-STD:1.0',4,'MOLD','塑封','MOLD',0,'2026-05-25 21:13:05'),(16,'SOP-STD:1.0',5,'MARK','激光打标','LASER',0,'2026-05-25 21:13:05'),(17,'SOP-STD:1.0',6,'SING','切割分条','SING',0,'2026-05-25 21:13:05'),(18,'SOP-STD:1.0',7,'FT','最终测试','TEST',0,'2026-05-25 21:13:05'),(19,'SOP-STD:1.0',8,'OQC','出货检验','OQC',0,'2026-05-25 21:13:05'),(20,'SOP-STD:1.0',9,'PACK','包装入库','PACK',0,'2026-05-25 21:13:05'),(21,'BGA-STD:1.0',1,'SAW','晶圆切割','SAW',0,'2026-05-25 21:13:05'),(22,'BGA-STD:1.0',2,'DA','芯片贴装','DA',0,'2026-05-25 21:13:05'),(23,'BGA-STD:1.0',3,'CURE','固化','CURE',0,'2026-05-25 21:13:05'),(24,'BGA-STD:1.0',4,'WB','焊线','WB',0,'2026-05-25 21:13:05'),(25,'BGA-STD:1.0',5,'MOLD','塑封','MOLD',0,'2026-05-25 21:13:05'),(26,'BGA-STD:1.0',6,'PMC','后固化','PMC',0,'2026-05-25 21:13:05'),(27,'BGA-STD:1.0',7,'BALL','植球','BALL',0,'2026-05-25 21:13:05'),(28,'BGA-STD:1.0',8,'REFLOW','回流焊','REFLOW',0,'2026-05-25 21:13:05'),(29,'BGA-STD:1.0',9,'MARK','激光打标','LASER',0,'2026-05-25 21:13:05'),(30,'BGA-STD:1.0',10,'FT','最终测试','TEST',0,'2026-05-25 21:13:05'),(31,'BGA-STD:1.0',11,'OQC','出货检验','OQC',0,'2026-05-25 21:13:05'),(32,'BGA-STD:1.0',12,'PACK','包装入库','PACK',0,'2026-05-25 21:13:05'),(33,'QFN48-STD:1.0',1,'SAW','晶圆切割','SAW',0,'2026-05-25 21:13:05'),(34,'QFN48-STD:1.0',2,'DA','芯片贴装','DA',0,'2026-05-25 21:13:05'),(35,'QFN48-STD:1.0',3,'WB','焊线','WB',0,'2026-05-25 21:13:05'),(36,'QFN48-STD:1.0',4,'MOLD','塑封','MOLD',0,'2026-05-25 21:13:05'),(37,'QFN48-STD:1.0',5,'PMC','后固化','PMC',0,'2026-05-25 21:13:05'),(38,'QFN48-STD:1.0',6,'MARK','激光打标','LASER',0,'2026-05-25 21:13:05'),(39,'QFN48-STD:1.0',7,'SING','切割分条','SING',0,'2026-05-25 21:13:05'),(40,'QFN48-STD:1.0',8,'FT','最终测试','TEST',0,'2026-05-25 21:13:05'),(41,'QFN48-STD:1.0',9,'OQC','出货检验','OQC',0,'2026-05-25 21:13:05'),(42,'QFN48-STD:1.0',10,'PACK','包装入库','PACK',0,'2026-05-25 21:13:05'),(43,'SOP8-STD:1.0',1,'DA','芯片贴装','DA',0,'2026-05-25 21:13:05'),(44,'SOP8-STD:1.0',2,'WB','焊线','WB',0,'2026-05-25 21:13:05'),(45,'SOP8-STD:1.0',3,'MOLD','塑封','MOLD',0,'2026-05-25 21:13:05'),(46,'SOP8-STD:1.0',4,'MARK','激光打标','LASER',0,'2026-05-25 21:13:05'),(47,'SOP8-STD:1.0',5,'SING','切割分条','SING',0,'2026-05-25 21:13:05'),(48,'SOP8-STD:1.0',6,'FT','最终测试','TEST',0,'2026-05-25 21:13:05'),(49,'SOP8-STD:1.0',7,'OQC','出货检验','OQC',0,'2026-05-25 21:13:05'),(50,'SOP8-STD:1.0',8,'PACK','包装入库','PACK',0,'2026-05-25 21:13:05'),(51,'BGA64-STD:1.0',1,'SAW','晶圆切割','SAW',0,'2026-05-25 21:13:05'),(52,'BGA64-STD:1.0',2,'DA','芯片贴装','DA',0,'2026-05-25 21:13:05'),(53,'BGA64-STD:1.0',3,'CURE','固化','CURE',0,'2026-05-25 21:13:05'),(54,'BGA64-STD:1.0',4,'WB','焊线','WB',0,'2026-05-25 21:13:05'),(55,'BGA64-STD:1.0',5,'MOLD','塑封','MOLD',0,'2026-05-25 21:13:05'),(56,'BGA64-STD:1.0',6,'PMC','后固化','PMC',0,'2026-05-25 21:13:05'),(57,'BGA64-STD:1.0',7,'BALL','植球','BALL',0,'2026-05-25 21:13:05'),(58,'BGA64-STD:1.0',8,'FT','最终测试','TEST',0,'2026-05-25 21:13:05'),(59,'BGA64-STD:1.0',9,'OQC','出货检验','OQC',0,'2026-05-25 21:13:05'),(60,'BGA64-STD:1.0',10,'PACK','包装入库','PACK',0,'2026-05-25 21:13:05'),(61,'QFN64-STD:1.0',1,'SAW','晶圆切割','SAW',0,'2026-05-25 21:13:05'),(62,'QFN64-STD:1.0',2,'DA','芯片贴装','DA',0,'2026-05-25 21:13:05'),(63,'QFN64-STD:1.0',3,'CURE','固化','CURE',0,'2026-05-25 21:13:05'),(64,'QFN64-STD:1.0',4,'WB','焊线','WB',0,'2026-05-25 21:13:05'),(65,'QFN64-STD:1.0',5,'MOLD','塑封','MOLD',0,'2026-05-25 21:13:05'),(66,'QFN64-STD:1.0',6,'PMC','后固化','PMC',0,'2026-05-25 21:13:05'),(67,'QFN64-STD:1.0',7,'MARK','激光打标','LASER',0,'2026-05-25 21:13:05'),(68,'QFN64-STD:1.0',8,'SING','切割分条','SING',0,'2026-05-25 21:13:05'),(69,'QFN64-STD:1.0',9,'FT','最终测试','TEST',0,'2026-05-25 21:13:05'),(70,'QFN64-STD:1.0',10,'OQC','出货检验','OQC',0,'2026-05-25 21:13:05'),(71,'QFN64-STD:1.0',11,'PACK','包装入库','PACK',0,'2026-05-25 21:13:05'),(72,'SOP14-STD:1.0',1,'DA','芯片贴装','DA',0,'2026-05-25 21:13:05'),(73,'SOP14-STD:1.0',2,'WB','焊线','WB',0,'2026-05-25 21:13:05'),(74,'SOP14-STD:1.0',3,'MOLD','塑封','MOLD',0,'2026-05-25 21:13:05'),(75,'SOP14-STD:1.0',4,'MARK','激光打标','LASER',0,'2026-05-25 21:13:05'),(76,'SOP14-STD:1.0',5,'SING','切割分条','SING',0,'2026-05-25 21:13:05'),(77,'SOP14-STD:1.0',6,'FT','最终测试','TEST',0,'2026-05-25 21:13:05'),(78,'SOP14-STD:1.0',7,'OQC','出货检验','OQC',0,'2026-05-25 21:13:05'),(79,'SOP14-STD:1.0',8,'PACK','包装入库','PACK',0,'2026-05-25 21:13:05'),(80,'RW-DA:1.0',1,'DEBOND-DA','去胶','DEBOND',1,'2026-05-25 21:13:05'),(81,'RW-DA:1.0',2,'CLEAN-DA','清洗','CLEAN',1,'2026-05-25 21:13:05'),(82,'RW-DA:1.0',3,'DA','重新贴片','DA',1,'2026-05-25 21:13:05'),(83,'RW-WB:1.0',1,'DEBOND-WB','去线','DEBOND',1,'2026-05-25 21:13:05'),(84,'RW-WB:1.0',2,'CLEAN-WB','清洗','CLEAN',1,'2026-05-25 21:13:05'),(85,'RW-WB:1.0',3,'WB','重新焊线','WB',1,'2026-05-25 21:13:05'),(86,'RW-MOLD:1.0',1,'DECAP','去封装','DECAP',1,'2026-05-25 21:13:05'),(87,'RW-MOLD:1.0',2,'CLEAN-MOLD','清洗','CLEAN',1,'2026-05-25 21:13:05'),(88,'RW-MOLD:1.0',3,'MOLD','重新塑封','MOLD',1,'2026-05-25 21:13:05');
/*!40000 ALTER TABLE `master_route_step` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_scrap_rule`
--

DROP TABLE IF EXISTS `master_scrap_rule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_scrap_rule` (
  `rule_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `route_id` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `threshold_percent` decimal(5,2) NOT NULL,
  `requires_approval` tinyint(1) DEFAULT '1',
  `approval_level` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_active` tinyint(1) DEFAULT '1',
  `created_at` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`rule_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_scrap_rule`
--

LOCK TABLES `master_scrap_rule` WRITE;
/*!40000 ALTER TABLE `master_scrap_rule` DISABLE KEYS */;
INSERT INTO `master_scrap_rule` VALUES ('SCRAP-BGA-001','BGA-STD:1.0','WB',2.00,1,'Engineer',1,'2026-05-28 15:11:46'),('SCRAP-BGA-002','BGA-STD:1.0','FT',4.00,1,'Manager',1,'2026-05-28 15:11:46'),('SCRAP-QFN-001','QFN-STD:1.0','WB',3.00,1,'Engineer',1,'2026-05-28 15:11:46'),('SCRAP-QFN-002','QFN-STD:1.0','FT',5.00,1,'Manager',1,'2026-05-28 15:11:46'),('SCRAP-SOP-001','SOP-STD:1.0','WB',3.00,1,'Engineer',1,'2026-05-28 15:11:46'),('SCRAP-SOP-002','SOP-STD:1.0','FT',5.00,1,'Manager',1,'2026-05-28 15:11:46');
/*!40000 ALTER TABLE `master_scrap_rule` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `master_yield_rule`
--

DROP TABLE IF EXISTS `master_yield_rule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `master_yield_rule` (
  `rule_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `route_id` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `yield_threshold` decimal(5,2) NOT NULL,
  `action_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'AutoHold',
  `notify_role` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'QA',
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`rule_id`),
  KEY `idx_route_id` (`route_id`),
  KEY `idx_step_code` (`step_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `master_yield_rule`
--

LOCK TABLES `master_yield_rule` WRITE;
/*!40000 ALTER TABLE `master_yield_rule` DISABLE KEYS */;
INSERT INTO `master_yield_rule` VALUES ('YR-BGA-DA','BGA-STD:1.0','DA',99.50,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-BGA-FT','BGA-STD:1.0','FT',94.00,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-BGA-SAW','BGA-STD:1.0','SAW',99.80,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-BGA-WB','BGA-STD:1.0','WB',99.00,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-BGA64-FT','BGA64-STD:1.0','FT',95.00,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-QFN-DA','QFN-STD:2.0','DA',99.00,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-QFN-FT','QFN-STD:2.0','FT',95.00,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-QFN-MOLD','QFN-STD:2.0','MOLD',99.00,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-QFN-SAW','QFN-STD:2.0','SAW',99.50,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-QFN-WB','QFN-STD:2.0','WB',98.50,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-QFN48-FT','QFN48-STD:1.0','FT',95.50,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-QFN64-FT','QFN64-STD:1.0','FT',95.50,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-SOP-DA','SOP-STD:1.0','DA',99.00,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-SOP-FT','SOP-STD:1.0','FT',96.00,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-SOP-SAW','SOP-STD:1.0','SAW',99.50,'AutoHold','QA',1,'2026-05-25 21:13:05'),('YR-SOP-WB','SOP-STD:1.0','WB',98.50,'AutoHold','QA',1,'2026-05-25 21:13:05');
/*!40000 ALTER TABLE `master_yield_rule` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_audit_trail`
--

DROP TABLE IF EXISTS `prod_audit_trail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_audit_trail` (
  `audit_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `entity_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Lot/WorkOrder/Route/etc',
  `entity_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `action` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'TrackIn/TrackOut/Hold/Release/etc',
  `operator_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `operator_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `timestamp` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `before_state` json DEFAULT NULL,
  `after_state` json DEFAULT NULL,
  `reason` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `detail` text COLLATE utf8mb4_unicode_ci,
  `signature_level` int DEFAULT '0' COMMENT '0-3 签核级别',
  `approved_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`audit_id`),
  KEY `idx_entity` (`entity_type`,`entity_id`),
  KEY `idx_action` (`action`),
  KEY `idx_timestamp` (`timestamp`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_audit_trail`
--

LOCK TABLES `prod_audit_trail` WRITE;
/*!40000 ALTER TABLE `prod_audit_trail` DISABLE KEYS */;
INSERT INTO `prod_audit_trail` VALUES ('AUD-001','Lot','LOT-001','TrackIn','USR-005','孙切割','2026-05-21 21:36:54','{\"status\": \"Waiting\"}','{\"step\": \"SAW\", \"status\": \"Processing\"}','正常投产',NULL,0,NULL),('AUD-002','Lot','LOT-001','TrackOut','USR-010','刘操作员','2026-05-22 21:36:54','{\"step\": \"SAW\", \"status\": \"Processing\"}','{\"step\": \"SAW\", \"status\": \"Completed\", \"pass_qty\": 4900}','SAW完成',NULL,0,NULL),('AUD-003','Lot','LOT-HOLD-001','Hold','USR-002','李品质','2026-05-21 21:36:54','{\"status\": \"Processing\"}','{\"status\": \"Hold\", \"hold_reason\": \"DA良率异常\"}','DA工序良率低于阈值',NULL,1,NULL),('AUD-004','Lot','LOT-REWORK-001','Rework','USR-003','王工程','2026-05-24 21:36:54','{\"route\": \"QFN-STD:2.0\", \"status\": \"Processing\"}','{\"route\": \"RW-WB:1.0\", \"status\": \"Processing\"}','WB工序重工',NULL,1,NULL),('AUD-005','Lot','LOT-004','Completed','USR-004','赵仓储','2026-05-20 21:36:54','{\"status\": \"Processing\"}','{\"status\": \"Completed\", \"final_yield\": 96.0}','工单完成入库',NULL,0,NULL),('AUD-006','WorkOrder','WO-2026002','Completed','USR-013','何计划','2026-05-23 21:36:54','{\"status\": \"Processing\"}','{\"status\": \"Completed\", \"completed_qty\": 15000}','全部批次完成',NULL,0,NULL);
/*!40000 ALTER TABLE `prod_audit_trail` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_carrier_binding`
--

DROP TABLE IF EXISTS `prod_carrier_binding`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_carrier_binding` (
  `binding_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_seq` int NOT NULL,
  `carrier_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `carrier_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `from_carrier_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `bind_time` datetime NOT NULL,
  `unbind_time` datetime DEFAULT NULL,
  `operator_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`binding_id`),
  KEY `idx_lot_id` (`lot_id`),
  KEY `idx_carrier_id` (`carrier_id`),
  KEY `idx_bind_time` (`bind_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_carrier_binding`
--

LOCK TABLES `prod_carrier_binding` WRITE;
/*!40000 ALTER TABLE `prod_carrier_binding` DISABLE KEYS */;
INSERT INTO `prod_carrier_binding` VALUES ('BIND-001','LOT-001','DA',2,'CARRIER-LEAD-002','LeadFrame',NULL,'2026-05-22 21:37:27',NULL,'USR-006','2026-05-25 21:37:27'),('BIND-002','LOT-001','WB',4,'CARRIER-LEAD-002','LeadFrame',NULL,'2026-05-24 21:37:27',NULL,'USR-007','2026-05-25 21:37:27'),('BIND-003','LOT-002','MOLD',5,'CARRIER-TRAY-003','Tray',NULL,'2026-05-24 21:37:27',NULL,'USR-008','2026-05-25 21:37:27'),('BIND-004','LOT-004','SAW',1,'CARRIER-MAG-002','Magazine',NULL,'2026-05-11 21:37:27',NULL,'USR-005','2026-05-25 21:37:27'),('BIND-005','LOT-007','SAW',1,'CARRIER-TRAY-001','Tray',NULL,'2026-05-23 21:37:27',NULL,'USR-005','2026-05-25 21:37:27'),('BIND-006','LOT-009','MOLD',3,'CARRIER-LEAD-001','LeadFrame',NULL,'2026-05-19 21:37:27',NULL,'USR-008','2026-05-25 21:37:27');
/*!40000 ALTER TABLE `prod_carrier_binding` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_dispatch_task`
--

DROP TABLE IF EXISTS `prod_dispatch_task`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_dispatch_task` (
  `task_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `order_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `product_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_seq` int NOT NULL,
  `equipment_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `recipe_id` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `qty` int NOT NULL,
  `priority` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Normal',
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Pending',
  `assigned_operator` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `assigned_at` datetime DEFAULT NULL,
  `started_at` datetime DEFAULT NULL,
  `completed_at` datetime DEFAULT NULL,
  `due_hours` decimal(6,2) DEFAULT NULL,
  `remaining_hours` decimal(6,2) DEFAULT NULL,
  `is_overdue` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`task_id`),
  KEY `idx_lot_id` (`lot_id`),
  KEY `idx_order_id` (`order_id`),
  KEY `idx_status` (`status`),
  KEY `idx_priority` (`priority`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_dispatch_task`
--

LOCK TABLES `prod_dispatch_task` WRITE;
/*!40000 ALTER TABLE `prod_dispatch_task` DISABLE KEYS */;
INSERT INTO `prod_dispatch_task` VALUES ('TASK-001','LOT-001','WO-2026001','PROD-QFN88','WB','焊线',4,'WB-002',NULL,4750,'High','Processing','USR-007','2026-05-24 21:37:32',NULL,NULL,NULL,NULL,NULL,0),('TASK-002','LOT-001','WO-2026001','PROD-QFN88','MOLD','塑封',5,'MOLD-001',NULL,4750,'High','Pending',NULL,'2026-05-24 21:37:32',NULL,NULL,NULL,NULL,NULL,0),('TASK-003','LOT-002','WO-2026001','PROD-QFN88','MOLD','塑封',5,'MOLD-002',NULL,2800,'High','Processing','USR-008','2026-05-24 21:37:32',NULL,NULL,NULL,NULL,NULL,0),('TASK-004','LOT-003','WO-2026001','PROD-QFN88','PMC','后固化',6,'PMC-001',NULL,5000,'Normal','Pending',NULL,'2026-05-25 21:37:32',NULL,NULL,NULL,NULL,NULL,0),('TASK-005','LOT-007','WO-2026003','PROD-BGA256','WB','焊线',4,'WB-003',NULL,2500,'High','Pending',NULL,'2026-05-24 21:37:32',NULL,NULL,NULL,NULL,NULL,0),('TASK-006','LOT-009','WO-2026005','PROD-SOP8','MOLD','塑封',3,'MOLD-001',NULL,10000,'Normal','Processing','USR-016','2026-05-19 21:37:32',NULL,NULL,NULL,NULL,NULL,0),('TASK-007','LOT-011','WO-2026006','PROD-BGA64','PMC','后固化',6,'PMC-002',NULL,5000,'Normal','Processing','USR-024','2026-05-22 21:37:32',NULL,NULL,NULL,NULL,NULL,0),('TASK-008','LOT-012','WO-2026007','PROD-QFN64','MOLD','塑封',5,'MOLD-003',NULL,10000,'High','Processing','USR-008','2026-05-23 21:37:32',NULL,NULL,NULL,NULL,NULL,0);
/*!40000 ALTER TABLE `prod_dispatch_task` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_genealogy`
--

DROP TABLE IF EXISTS `prod_genealogy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_genealogy` (
  `genealogy_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `parent_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `child_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `relation_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Create/Split/Merge/Rework/GradeSplit',
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_seq` int NOT NULL,
  `qty` int NOT NULL,
  `grade` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `wafer_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `operator_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `reason_code` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `remark` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`genealogy_id`),
  KEY `idx_parent_lot` (`parent_lot_id`),
  KEY `idx_child_lot` (`child_lot_id`),
  KEY `idx_relation_type` (`relation_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_genealogy`
--

LOCK TABLES `prod_genealogy` WRITE;
/*!40000 ALTER TABLE `prod_genealogy` DISABLE KEYS */;
/*!40000 ALTER TABLE `prod_genealogy` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_hold_record`
--

DROP TABLE IF EXISTS `prod_hold_record`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_hold_record` (
  `hold_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `hold_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `hold_reason_code` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `hold_reason` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `hold_qty` int NOT NULL,
  `responsible_dept` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `owner` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Open',
  `hold_by` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `hold_time` datetime NOT NULL,
  `root_cause` text COLLATE utf8mb4_unicode_ci,
  `corrective_action` text COLLATE utf8mb4_unicode_ci,
  `disposition` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `release_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `release_time` datetime DEFAULT NULL,
  `release_comment` text COLLATE utf8mb4_unicode_ci,
  `approved_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`hold_id`),
  KEY `idx_lot_id` (`lot_id`),
  KEY `idx_status` (`status`),
  KEY `idx_hold_type` (`hold_type`),
  KEY `idx_hold_time` (`hold_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_hold_record`
--

LOCK TABLES `prod_hold_record` WRITE;
/*!40000 ALTER TABLE `prod_hold_record` DISABLE KEYS */;
INSERT INTO `prod_hold_record` VALUES ('HOLD-001','LOT-HOLD-001','Quality','YIELD_LOW','DA工序良率低于99%阈值',500,'DEPT-QA','USR-002','Open','USR-002','2026-05-21 21:36:59','待分析',NULL,NULL,NULL,NULL,NULL,NULL,'2026-05-25 21:36:59'),('HOLD-002','LOT-001','Quality','YIELD_WARN','WB工序良率预警',150,'DEPT-QA','USR-002','Closed','USR-002','2026-05-24 21:36:59','设备参数偏移','调整焊线参数','继续生产','USR-003','2026-05-25 21:36:59',NULL,NULL,'2026-05-25 21:36:59'),('HOLD-003','LOT-007','Equipment','EQUIP_FAULT','SAW-003设备异常',200,'DEPT-ENG','USR-015','Open','USR-015','2026-05-23 21:36:59','待维修',NULL,NULL,NULL,NULL,NULL,NULL,'2026-05-25 21:36:59'),('HOLD-004','LOT-012','Engineering','ENG_HOLD','ECN verification in progress',2000,'DEPT-ENG','USR-003','Open','USR-003','2026-05-25 20:52:21','ECN-2026-001','Verify then release','Pending',NULL,NULL,NULL,NULL,'2026-05-26 20:52:21'),('HOLD-005','LOT-007','Customer','CUST_HOLD','Customer requires additional reliability test',500,'DEPT-QA','USR-002','Open','USR-002','2026-05-23 20:52:21','Customer special request','Complete test then release','Pending test',NULL,NULL,NULL,NULL,'2026-05-26 20:52:21'),('HOLD-006','LOT-008','MRB','MRB_REVIEW','Mold anomaly pending MRB review',300,'DEPT-QA','USR-027','Open','USR-027','2026-05-24 20:52:22','Mold bubble rate exceeds limit','Pending MRB decision','Pending review',NULL,NULL,NULL,NULL,'2026-05-26 20:52:22');
/*!40000 ALTER TABLE `prod_hold_record` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_lot`
--

DROP TABLE IF EXISTS `prod_lot`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_lot` (
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `order_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `product_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `product_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `die_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `package_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `route_id` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `route_version` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '1.0',
  `current_step_seq` int NOT NULL DEFAULT '0',
  `current_step_code` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Waiting',
  `process_stage` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '??????: Frontend-???(???/CP), Backend-???(???/FT)',
  `unit_count` int NOT NULL DEFAULT '0',
  `strip_count` int NOT NULL DEFAULT '0',
  `priority` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Normal',
  `carrier_type` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `carrier_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `wafer_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `original_qty` int NOT NULL DEFAULT '0',
  `total_pass_qty` int NOT NULL DEFAULT '0',
  `total_scrap_qty` int NOT NULL DEFAULT '0',
  `total_rework_qty` int NOT NULL DEFAULT '0',
  `total_hold_qty` int NOT NULL DEFAULT '0',
  `is_partial_lot` tinyint(1) NOT NULL DEFAULT '0',
  `mother_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `split_reason` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `split_time` datetime DEFAULT NULL,
  `split_qty` int DEFAULT NULL,
  `is_rework_lot` tinyint(1) NOT NULL DEFAULT '0',
  `original_route_id` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `rework_route_id` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `rework_count` int DEFAULT NULL,
  `rework_reason` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_under_mrb` tinyint(1) NOT NULL DEFAULT '0',
  `mrb_reference` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `mrb_disposition` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `grade` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `original_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `bin_result` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `test_result` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `qty_pass` int NOT NULL DEFAULT '0',
  `qty_fail` int NOT NULL DEFAULT '0',
  `hold_category` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `hold_reason` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `hold_time` datetime DEFAULT NULL,
  `hold_operator` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `release_condition` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_archived` tinyint(1) NOT NULL DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`lot_id`),
  KEY `idx_order_id` (`order_id`),
  KEY `idx_product_id` (`product_id`),
  KEY `idx_status` (`status`),
  KEY `idx_current_step` (`current_step_code`),
  KEY `idx_priority` (`priority`),
  KEY `idx_mother_lot` (`mother_lot_id`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_lot`
--

LOCK TABLES `prod_lot` WRITE;
/*!40000 ALTER TABLE `prod_lot` DISABLE KEYS */;
INSERT INTO `prod_lot` VALUES ('LOT-001','WO-2026001','PROD-QFN88','QFN-88 控制器','CTRL-2024','QFN','QFN-STD:2.0','2.0',4,'WB','Processing','Backend',0,0,'Normal','LeadFrame','CARRIER-LEAD-002',NULL,5000,3000,200,0,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G1',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-002','WO-2026001','PROD-QFN88','QFN-88 控制器','CTRL-2024','QFN','QFN-STD:2.0','2.0',5,'MOLD','Processing','Backend',0,0,'Normal','Tray','CARRIER-TRAY-003',NULL,5000,2800,150,0,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G1',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-003','WO-2026001','PROD-QFN88','QFN-88 控制器','CTRL-2024','QFN','QFN-STD:2.0','2.0',6,'PMC','Waiting','Backend',0,0,'Normal',NULL,NULL,NULL,5000,0,0,0,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G1',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-004','WO-2026002','PROD-SOP16','SOP-16 电源IC','PWR-500','SOP','SOP-STD:1.0','1.0',9,'PACK','Completed','Backend',0,0,'Normal','Magazine','CARRIER-MAG-002',NULL,5000,4800,100,100,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G3',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-005','WO-2026002','PROD-SOP16','SOP-16 电源IC','PWR-500','SOP','SOP-STD:1.0','1.0',9,'PACK','Completed','Backend',0,0,'Normal','Magazine','CARRIER-MAG-003',NULL,5000,4750,150,100,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G3',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-006','WO-2026002','PROD-SOP16','SOP-16 电源IC','PWR-500','SOP','SOP-STD:1.0','1.0',9,'PACK','Completed','Backend',0,0,'Normal','Magazine','CARRIER-MAG-004',NULL,5000,4850,80,70,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G3',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-007','WO-2026003','PROD-BGA256','BGA-256 处理器','CPU-3000','BGA','BGA-STD:1.0','1.0',4,'WB','Processing','Backend',0,0,'Normal','Tray','CARRIER-TRAY-001',NULL,2500,1500,50,0,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G1',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-008','WO-2026003','PROD-BGA256','BGA-256 处理器','CPU-3000','BGA','BGA-STD:1.0','1.0',5,'MOLD','Processing','Backend',0,0,'Normal','Tray','CARRIER-TRAY-002',NULL,2500,1200,30,0,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,1,'MRB-2026-001',NULL,'G1',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:22'),('LOT-009','WO-2026005','PROD-SOP8','SOP-8 MOSFET','MOS-200','SOP','SOP8-STD:1.0','1.0',3,'MOLD','Processing','Backend',0,0,'Normal','LeadFrame','CARRIER-LEAD-001',NULL,10000,6000,200,0,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G2',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-009-G1','WO-2026005','PROD-SOP8','SOP-8 MOSFET','MOS-200','SOP','SOP8-STD:1.0','1.0',7,'OQC','Processing','Backend',0,0,'Normal','LeadFrame','CARRIER-LEAD-005',NULL,8000,7800,50,0,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G1',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-26 20:52:21','2026-05-26 20:52:21'),('LOT-009-G2','WO-2026005','PROD-SOP8','SOP-8 MOSFET','MOS-200','SOP','SOP8-STD:1.0','1.0',7,'OQC','Processing','Backend',0,0,'Normal','LeadFrame','CARRIER-LEAD-004',NULL,1500,1400,30,0,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G2',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-26 20:52:21','2026-05-26 20:52:21'),('LOT-010','WO-2026005','PROD-SOP8','SOP-8 MOSFET','MOS-200','SOP','SOP8-STD:1.0','1.0',4,'MARK','Processing','Backend',0,0,'Normal','LeadFrame','CARRIER-LEAD-003',NULL,10000,5800,150,50,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G2',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-011','WO-2026006','PROD-BGA64','BGA-64 存储器','MEM-500','BGA','BGA64-STD:1.0','1.0',6,'PMC','Processing','Backend',0,0,'Normal','Tray','CARRIER-TRAY-004',NULL,5000,3000,100,0,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G1',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-012','WO-2026007','PROD-QFN64','QFN-64 驱动器','DRV-300','QFN','QFN64-STD:1.0','1.0',5,'MOLD','Processing','Backend',0,0,'Normal','Tray','CARRIER-TRAY-005',NULL,10000,6000,200,0,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G1',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-013','WO-2026007','PROD-QFN64','QFN-64 驱动器','DRV-300','QFN','QFN64-STD:1.0','1.0',7,'MARK','Processing','Backend',0,0,'Normal','LeadFrame','CARRIER-LEAD-004',NULL,10000,5500,150,350,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G1',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-014','WO-2026008','PROD-SOP14','SOP-14 运放','OP-100','SOP','SOP14-STD:1.0','1.0',8,'PACK','Completed','Backend',0,0,'Normal','Magazine','CARRIER-MAG-001',NULL,20000,19500,300,200,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G3',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-015','WO-2026008','PROD-SOP14','SOP-14 运放','OP-100','SOP','SOP14-STD:1.0','1.0',8,'PACK','Completed','Backend',0,0,'Normal','Magazine','CARRIER-MAG-005',NULL,20000,19600,250,150,0,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G3',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-HOLD-001','WO-2026001','PROD-QFN88','QFN-88 控制器','CTRL-2024','QFN','QFN-STD:2.0','2.0',2,'DA','Hold','Backend',0,0,'Normal',NULL,NULL,NULL,5000,0,300,0,500,0,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,0,NULL,NULL,'G1',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01'),('LOT-REWORK-001','WO-2026001','PROD-QFN88','QFN-88 控制器','CTRL-2024','QFN','RW-WB:1.0','1.0',1,'DEBOND-WB','Processing','Backend',0,0,'Normal',NULL,NULL,NULL,5000,0,0,500,0,0,NULL,NULL,NULL,NULL,1,NULL,NULL,NULL,NULL,0,NULL,NULL,'G1',NULL,NULL,NULL,0,0,NULL,NULL,NULL,NULL,NULL,0,'2026-05-25 21:28:00','2026-05-26 20:52:01');
/*!40000 ALTER TABLE `prod_lot` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_lot_archive`
--

DROP TABLE IF EXISTS `prod_lot_archive`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_lot_archive` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `order_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `product_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `product_name` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '??????',
  `route_id` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '??????ID',
  `process_stage` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '??????',
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL,
  `original_qty` int NOT NULL,
  `total_pass_qty` int NOT NULL,
  `total_scrap_qty` int NOT NULL,
  `total_rework_qty` int DEFAULT NULL COMMENT '?????????',
  `total_hold_qty` int DEFAULT NULL COMMENT '?????????',
  `final_yield` decimal(5,2) DEFAULT '0.00',
  `grade` varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '??????',
  `completed_at` datetime NOT NULL,
  `archived_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `archived_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_order_id` (`order_id`),
  KEY `idx_completed_at` (`completed_at`),
  KEY `idx_archive_lot_id` (`lot_id`),
  KEY `idx_archive_order_id` (`order_id`),
  KEY `idx_archive_completed_at` (`completed_at`),
  KEY `idx_archive_stage` (`process_stage`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_lot_archive`
--

LOCK TABLES `prod_lot_archive` WRITE;
/*!40000 ALTER TABLE `prod_lot_archive` DISABLE KEYS */;
INSERT INTO `prod_lot_archive` VALUES (1,'LOT-004','WO-2026002','PROD-SOP16',NULL,NULL,NULL,'Completed',5000,4800,100,NULL,NULL,96.00,NULL,'2026-05-20 21:38:27','2026-05-25 21:38:27',NULL),(2,'LOT-005','WO-2026002','PROD-SOP16',NULL,NULL,NULL,'Completed',5000,4750,150,NULL,NULL,95.00,NULL,'2026-05-21 21:38:27','2026-05-25 21:38:27',NULL),(3,'LOT-006','WO-2026002','PROD-SOP16',NULL,NULL,NULL,'Completed',5000,4850,80,NULL,NULL,97.00,NULL,'2026-05-22 21:38:27','2026-05-25 21:38:27',NULL),(4,'LOT-014','WO-2026008','PROD-SOP14',NULL,NULL,NULL,'Completed',20000,19500,300,NULL,NULL,97.50,NULL,'2026-05-19 21:38:27','2026-05-25 21:38:27',NULL),(5,'LOT-015','WO-2026008','PROD-SOP14',NULL,NULL,NULL,'Completed',20000,19600,250,NULL,NULL,98.00,NULL,'2026-05-20 21:38:27','2026-05-25 21:38:27',NULL),(6,'LOT-004','WO-2026002','PROD-SOP16','SOP-16 ??IC','SOP-STD:1.0','Backend','Completed',5000,4800,100,100,0,96.00,'G3','2026-05-21 20:52:22','2026-05-26 20:52:22','USR-013'),(7,'LOT-005','WO-2026002','PROD-SOP16','SOP-16 ??IC','SOP-STD:1.0','Backend','Completed',5000,4750,150,100,0,95.00,'G3','2026-05-22 20:52:22','2026-05-26 20:52:22','USR-013'),(8,'LOT-006','WO-2026002','PROD-SOP16','SOP-16 ??IC','SOP-STD:1.0','Backend','Completed',5000,4850,80,70,0,97.00,'G3','2026-05-23 20:52:22','2026-05-26 20:52:22','USR-013'),(9,'LOT-014','WO-2026008','PROD-SOP14','SOP-14 ??','SOP14-STD:1.0','Backend','Completed',20000,19500,300,200,0,97.50,'G3','2026-05-20 20:52:22','2026-05-26 20:52:22','USR-013'),(10,'LOT-015','WO-2026008','PROD-SOP14','SOP-14 ??','SOP14-STD:1.0','Backend','Completed',20000,19600,250,150,0,98.00,'G3','2026-05-21 20:52:22','2026-05-26 20:52:22','USR-013');
/*!40000 ALTER TABLE `prod_lot_archive` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_lot_merge`
--

DROP TABLE IF EXISTS `prod_lot_merge`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_lot_merge` (
  `merge_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `target_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `source_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `merge_qty` int NOT NULL,
  `merge_reason` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_seq` int NOT NULL,
  `operator_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `merge_time` datetime NOT NULL,
  `approved_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `signature_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`merge_id`),
  KEY `idx_target_lot` (`target_lot_id`),
  KEY `idx_source_lot` (`source_lot_id`),
  KEY `idx_merge_time` (`merge_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_lot_merge`
--

LOCK TABLES `prod_lot_merge` WRITE;
/*!40000 ALTER TABLE `prod_lot_merge` DISABLE KEYS */;
/*!40000 ALTER TABLE `prod_lot_merge` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_lot_split`
--

DROP TABLE IF EXISTS `prod_lot_split`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_lot_split` (
  `split_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `mother_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `child_lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `split_qty` int NOT NULL,
  `split_reason` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `split_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Normal/Grade',
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_seq` int NOT NULL,
  `operator_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `split_time` datetime NOT NULL,
  `approved_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `signature_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`split_id`),
  KEY `idx_mother_lot` (`mother_lot_id`),
  KEY `idx_child_lot` (`child_lot_id`),
  KEY `idx_split_time` (`split_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_lot_split`
--

LOCK TABLES `prod_lot_split` WRITE;
/*!40000 ALTER TABLE `prod_lot_split` DISABLE KEYS */;
INSERT INTO `prod_lot_split` VALUES ('SPLIT-001','LOT-001','LOT-REWORK-001',5000,'WB重工拆分','Rework','WB',4,'USR-007','2026-05-24 21:37:24','USR-003',NULL,'2026-05-25 21:37:24'),('SPLIT-002','LOT-007','LOT-007-GRADE',200,'等级拆分','Grade','SAW',1,'USR-005','2026-05-23 21:37:24','USR-003',NULL,'2026-05-25 21:37:24'),('SPLIT-003','LOT-009','LOT-009-G1',8000,'FT grade split-G1','Grade','FT',6,'USR-009','2026-05-23 20:52:21','USR-003',NULL,'2026-05-26 20:52:21'),('SPLIT-004','LOT-009','LOT-009-G2',1500,'FT grade split-G2','Grade','FT',6,'USR-009','2026-05-23 20:52:21','USR-003',NULL,'2026-05-26 20:52:21'),('SPLIT-005','LOT-009','LOT-009-SCRAP',500,'FT grade split-Scrap','Scrap','FT',6,'USR-009','2026-05-23 20:52:21','USR-003',NULL,'2026-05-26 20:52:21');
/*!40000 ALTER TABLE `prod_lot_split` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_lot_step`
--

DROP TABLE IF EXISTS `prod_lot_step`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_lot_step` (
  `record_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `route_id` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `route_version` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '1.0',
  `step_seq` int NOT NULL,
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Waiting',
  `track_in_equipment` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `track_in_carrier` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `track_in_recipe` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `track_in_time` datetime DEFAULT NULL,
  `track_in_operator` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `track_out_time` datetime DEFAULT NULL,
  `track_out_operator` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `input_qty` int NOT NULL DEFAULT '0',
  `pass_qty` int NOT NULL DEFAULT '0',
  `fail_qty` int NOT NULL DEFAULT '0',
  `scrap_qty` int NOT NULL DEFAULT '0',
  `rework_qty` int NOT NULL DEFAULT '0',
  `hold_qty` int NOT NULL DEFAULT '0',
  `pending_qty` int NOT NULL DEFAULT '0',
  `recipe_id` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `test_program` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `bin_summary` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `remark` text COLLATE utf8mb4_unicode_ci,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`record_id`),
  UNIQUE KEY `uk_lot_step` (`lot_id`,`route_id`,`step_seq`),
  KEY `idx_lot_id` (`lot_id`),
  KEY `idx_step_code` (`step_code`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_lot_step`
--

LOCK TABLES `prod_lot_step` WRITE;
/*!40000 ALTER TABLE `prod_lot_step` DISABLE KEYS */;
INSERT INTO `prod_lot_step` VALUES ('RS-001-1','LOT-001','QFN-STD:2.0','2.0',1,'SAW','晶圆切割','Completed','SAW-001',NULL,'REC-SAW-QFN88-001','2026-05-21 21:36:40','USR-005','2026-05-22 21:36:40','USR-010',5000,4900,50,50,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-001-2','LOT-001','QFN-STD:2.0','2.0',2,'DA','芯片贴装','Completed','DA-001','CARRIER-LEAD-002','REC-DA-QFN88-001','2026-05-22 21:36:40','USR-006','2026-05-23 21:36:40','USR-011',4900,4800,50,50,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-001-3','LOT-001','QFN-STD:2.0','2.0',3,'CURE','固化','Completed','CURE-001','CARRIER-LEAD-002','REC-CURE-QFN88-001','2026-05-23 21:36:40','USR-021','2026-05-24 21:36:40','USR-024',4800,4750,30,20,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-001-4','LOT-001','QFN-STD:2.0','2.0',4,'WB','焊线','Processing','WB-002','CARRIER-LEAD-002','REC-WB-QFN88-001','2026-05-24 21:36:40','USR-007',NULL,NULL,4750,3000,100,150,0,0,1500,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-001-5','LOT-001','QFN-STD:2.0','2.0',5,'MOLD','塑封','Waiting',NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,0,0,0,0,0,4750,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-004-1','LOT-004','SOP-STD:1.0','1.0',1,'SAW','晶圆切割','Completed','SAW-001',NULL,'REC-SAW-SOP16-001','2026-05-11 21:36:40','USR-005','2026-05-12 21:36:40','USR-010',5000,4920,40,40,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-004-2','LOT-004','SOP-STD:1.0','1.0',2,'DA','芯片贴装','Completed','DA-001','CARRIER-MAG-002','REC-DA-SOP16-001','2026-05-12 21:36:40','USR-006','2026-05-13 21:36:40','USR-011',4920,4850,30,40,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-004-3','LOT-004','SOP-STD:1.0','1.0',3,'WB','焊线','Completed','WB-001','CARRIER-MAG-002','REC-WB-SOP16-001','2026-05-13 21:36:40','USR-007','2026-05-14 21:36:40','USR-012',4850,4780,40,30,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-004-4','LOT-004','SOP-STD:1.0','1.0',4,'MOLD','塑封','Completed','MOLD-001','CARRIER-MAG-002','REC-MOLD-SOP16-001','2026-05-14 21:36:40','USR-008','2026-05-15 21:36:40','USR-016',4780,4720,30,30,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-004-5','LOT-004','SOP-STD:1.0','1.0',5,'MARK','激光打标','Completed','LASER-001','CARRIER-MAG-002','REC-MARK-SOP16-001','2026-05-15 21:36:40','USR-022','2026-05-16 21:36:40','USR-025',4720,4680,20,20,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-004-6','LOT-004','SOP-STD:1.0','1.0',6,'SING','切割分条','Completed','SING-001','CARRIER-MAG-002','REC-SING-SOP16-001','2026-05-16 21:36:40','USR-023','2026-05-17 21:36:40','USR-026',4680,4640,20,20,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-004-7','LOT-004','SOP-STD:1.0','1.0',7,'FT','最终测试','Completed','FT-001','CARRIER-MAG-002','REC-FT-SOP16-001','2026-05-17 21:36:40','USR-009','2026-05-18 21:36:40','USR-019',4640,4500,100,40,100,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-004-8','LOT-004','SOP-STD:1.0','1.0',8,'OQC','出货检验','Completed','OQC-001','CARRIER-MAG-002',NULL,'2026-05-18 21:36:40','USR-014','2026-05-19 21:36:40','USR-020',4500,4450,50,0,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-004-9','LOT-004','SOP-STD:1.0','1.0',9,'PACK','包装入库','Completed',NULL,'CARRIER-MAG-002',NULL,'2026-05-19 21:36:40','USR-004','2026-05-20 21:36:40','USR-004',4450,4400,0,50,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-HOLD-1','LOT-HOLD-001','QFN-STD:2.0','2.0',1,'SAW','晶圆切割','Completed','SAW-002',NULL,'REC-SAW-QFN88-002','2026-05-20 21:36:40','USR-005','2026-05-21 21:36:40','USR-018',5000,4850,80,70,0,0,0,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-HOLD-2','LOT-HOLD-001','QFN-STD:2.0','2.0',2,'DA','芯片贴装','Hold','DA-002',NULL,'REC-DA-QFN88-002','2026-05-21 21:36:40','USR-006',NULL,NULL,4850,0,200,300,0,500,3850,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40'),('RS-RW-1','LOT-REWORK-001','RW-WB:1.0','1.0',1,'DEBOND-WB','去线','Processing','WB-001',NULL,NULL,'2026-05-24 21:36:40','USR-007',NULL,NULL,5000,0,0,0,0,0,5000,NULL,NULL,NULL,NULL,'2026-05-25 21:36:40');
/*!40000 ALTER TABLE `prod_lot_step` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_operation_history`
--

DROP TABLE IF EXISTS `prod_operation_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_operation_history` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `operation_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `order_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `operation_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'TrackIn/TrackOut/Hold/Release/Split/Merge/etc',
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `step_seq` int DEFAULT NULL,
  `equipment_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `carrier_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `recipe_id` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `operator_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `operator_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `input_qty` int DEFAULT NULL,
  `output_qty` int DEFAULT NULL,
  `scrap_qty` int DEFAULT NULL,
  `detail` json DEFAULT NULL,
  `remark` text COLLATE utf8mb4_unicode_ci,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_operation_id` (`operation_id`),
  KEY `idx_lot_id` (`lot_id`),
  KEY `idx_order_id` (`order_id`),
  KEY `idx_operation_type` (`operation_type`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_operation_history`
--

LOCK TABLES `prod_operation_history` WRITE;
/*!40000 ALTER TABLE `prod_operation_history` DISABLE KEYS */;
INSERT INTO `prod_operation_history` VALUES (1,'OP-001','LOT-001','WO-2026001','TrackIn','SAW',1,'SAW-001',NULL,'REC-SAW-QFN88-001','USR-005','孙切割',5000,NULL,NULL,NULL,NULL,'2026-05-25 21:36:47'),(2,'OP-002','LOT-001','WO-2026001','TrackOut','SAW',1,'SAW-001',NULL,'REC-SAW-QFN88-001','USR-010','刘操作员',5000,4900,50,NULL,NULL,'2026-05-25 21:36:47'),(3,'OP-003','LOT-001','WO-2026001','TrackIn','DA',2,'DA-001','CARRIER-LEAD-002','REC-DA-QFN88-001','USR-006','周贴片',4900,NULL,NULL,NULL,NULL,'2026-05-25 21:36:47'),(4,'OP-004','LOT-001','WO-2026001','TrackOut','DA',2,'DA-001','CARRIER-LEAD-002','REC-DA-QFN88-001','USR-011','黄操作员',4900,4800,50,NULL,NULL,'2026-05-25 21:36:47'),(5,'OP-005','LOT-HOLD-001','WO-2026001','TrackIn','DA',2,'DA-002',NULL,'REC-DA-QFN88-002','USR-006','周贴片',4850,NULL,NULL,NULL,NULL,'2026-05-25 21:36:47'),(6,'OP-006','LOT-HOLD-001','WO-2026001','Hold','DA',2,'DA-002',NULL,NULL,'USR-002','李品质',NULL,NULL,300,NULL,NULL,'2026-05-25 21:36:47'),(7,'OP-007','LOT-004','WO-2026002','TrackIn','SAW',1,'SAW-001',NULL,'REC-SAW-SOP16-001','USR-005','孙切割',5000,NULL,NULL,NULL,NULL,'2026-05-25 21:36:47'),(8,'OP-008','LOT-004','WO-2026002','TrackOut','PACK',9,NULL,'CARRIER-MAG-002',NULL,'USR-004','赵仓储',4450,4400,50,NULL,NULL,'2026-05-25 21:36:47'),(9,'OP-009','LOT-REWORK-001','WO-2026001','Rework','DEBOND-WB',1,'WB-001',NULL,NULL,'USR-007','吴焊线',5000,NULL,NULL,NULL,NULL,'2026-05-25 21:36:47'),(10,'OP-010','LOT-007','WO-2026003','TrackIn','SAW',1,'SAW-003',NULL,'REC-SAW-BGA256-001','USR-005','孙切割',2500,NULL,NULL,NULL,NULL,'2026-05-25 21:36:47');
/*!40000 ALTER TABLE `prod_operation_history` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_rework_record`
--

DROP TABLE IF EXISTS `prod_rework_record`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_rework_record` (
  `rework_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `original_route_id` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `rework_route_id` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `from_step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `target_step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `rework_qty` int NOT NULL,
  `rework_reason` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `operator_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `rework_count` int NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `completed_at` datetime DEFAULT NULL,
  `approved_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `signature_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`rework_id`),
  KEY `idx_lot_id` (`lot_id`),
  KEY `idx_rework_route` (`rework_route_id`),
  KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_rework_record`
--

LOCK TABLES `prod_rework_record` WRITE;
/*!40000 ALTER TABLE `prod_rework_record` DISABLE KEYS */;
INSERT INTO `prod_rework_record` VALUES ('RW-001','LOT-REWORK-001','QFN-STD:2.0','RW-WB:1.0','WB','DEBOND-WB',5000,'焊线拉力不足','USR-007',1,'2026-05-25 21:37:17',NULL,'USR-003',NULL),('RW-002','LOT-013','QFN64-STD:1.0','RW-MOLD:1.0','MOLD','DECAP',350,'塑封气泡','USR-008',1,'2026-05-25 21:37:17',NULL,'USR-003',NULL);
/*!40000 ALTER TABLE `prod_rework_record` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_scrap_record`
--

DROP TABLE IF EXISTS `prod_scrap_record`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_scrap_record` (
  `scrap_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_seq` int NOT NULL,
  `scrap_qty` int NOT NULL,
  `scrap_reason` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `scrap_reason_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `operator_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `scrap_time` datetime NOT NULL,
  `approved_by` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `signature_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `requires_approval` tinyint(1) NOT NULL DEFAULT '0',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`scrap_id`),
  KEY `idx_lot_id` (`lot_id`),
  KEY `idx_step_code` (`step_code`),
  KEY `idx_scrap_time` (`scrap_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_scrap_record`
--

LOCK TABLES `prod_scrap_record` WRITE;
/*!40000 ALTER TABLE `prod_scrap_record` DISABLE KEYS */;
INSERT INTO `prod_scrap_record` VALUES ('SCRAP-001','LOT-001','SAW',1,50,'切割偏移','SC-001','USR-010','2026-05-22 21:36:59','USR-005',NULL,0,'2026-05-25 21:36:59'),('SCRAP-002','LOT-001','DA',2,50,'贴片偏移','SC-002','USR-011','2026-05-23 21:36:59','USR-006',NULL,0,'2026-05-25 21:36:59'),('SCRAP-003','LOT-HOLD-001','DA',2,300,'芯片破损','SC-003','USR-011','2026-05-21 21:36:59','USR-003',NULL,0,'2026-05-25 21:36:59'),('SCRAP-004','LOT-004','FT',7,40,'测试不良','SC-004','USR-019','2026-05-18 21:36:59','USR-009',NULL,0,'2026-05-25 21:36:59'),('SCRAP-005','LOT-007','SAW',1,70,'切割裂纹','SC-001','USR-010','2026-05-23 21:36:59','USR-005',NULL,0,'2026-05-25 21:36:59');
/*!40000 ALTER TABLE `prod_scrap_record` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `prod_work_order`
--

DROP TABLE IF EXISTS `prod_work_order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prod_work_order` (
  `order_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `parent_order_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Parent work order ID',
  `wo_type` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'WO type: Parent/Child-Assemble/Child-Test',
  `product_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `product_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `route_id` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `route_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `die_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `package_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `planned_qty` int NOT NULL,
  `completed_qty` int NOT NULL DEFAULT '0',
  `wafer_qty` int NOT NULL DEFAULT '0',
  `unit_qty` int NOT NULL DEFAULT '0',
  `customer_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `customer_name` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `customer_pn` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `internal_pn` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `priority` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Normal',
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Created',
  `creator` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `planned_start_date` datetime DEFAULT NULL,
  `planned_end_date` datetime DEFAULT NULL,
  `actual_start_date` datetime DEFAULT NULL,
  `actual_end_date` datetime DEFAULT NULL,
  `target_cp_yield` decimal(5,2) DEFAULT '99.00',
  `target_ft_yield` decimal(5,2) DEFAULT '98.00',
  `remark` text COLLATE utf8mb4_unicode_ci,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`order_id`),
  KEY `idx_product_id` (`product_id`),
  KEY `idx_status` (`status`),
  KEY `idx_priority` (`priority`),
  KEY `idx_created_at` (`created_at`),
  KEY `idx_parent_order_id` (`parent_order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `prod_work_order`
--

LOCK TABLES `prod_work_order` WRITE;
/*!40000 ALTER TABLE `prod_work_order` DISABLE KEYS */;
INSERT INTO `prod_work_order` VALUES ('WO-2026001',NULL,'Parent','PROD-QFN88','QFN-88 控制器','QFN-STD:2.0','QFN 标准路线','CTRL-2024','QFN',20000,8000,4,20000,'CUST-AUTO','某汽车电子','AE-QFN88-001','PN-QFN88-STD','High','Processing','USR-013','2026-05-20 21:27:55','2026-06-04 21:27:55','2026-05-21 21:27:55',NULL,99.00,98.00,'汽车电子紧急订单','2026-05-25 21:27:55','2026-05-26 21:24:04'),('WO-2026001-ASM','WO-2026001','Child-Assemble','PROD-QFN88','QFN-88 Power IC','QFN-STD:2.0','QFN Standard Route v2.0',NULL,'',20000,15000,0,0,'CUST-AUTO','Tesla Auto',NULL,NULL,'High','Processing','','2026-05-16 21:25:00','2026-05-31 21:25:00',NULL,NULL,99.00,98.00,NULL,'2026-05-16 21:25:00','2026-05-26 21:25:00'),('WO-2026001-FT','WO-2026001','Child-Test','PROD-QFN88','QFN-88 Power IC FT','QFN-STD:2.0','QFN Standard Route v2.0',NULL,'',20000,12000,0,0,'CUST-AUTO','Tesla Auto',NULL,NULL,'High','Processing','','2026-05-21 21:25:00','2026-06-05 21:25:00',NULL,NULL,99.00,98.00,NULL,'2026-05-21 21:25:00','2026-05-26 21:25:00'),('WO-2026002',NULL,'Parent','PROD-SOP16','SOP-16 电源IC','SOP-STD:1.0','SOP 标准路线','PWR-500','SOP',15000,15000,3,15000,'CUST-IND','某工业客户','IND-SOP16-002','PN-SOP16-STD','Normal','Completed','USR-013','2026-05-10 21:27:55','2026-05-23 21:27:55','2026-05-11 21:27:55','2026-05-23 21:27:55',99.00,96.00,'已完成工单','2026-05-25 21:27:55','2026-05-26 21:24:04'),('WO-2026003',NULL,'Parent','PROD-BGA256','BGA-256 处理器','BGA-STD:1.0','BGA 标准路线','CPU-3000','BGA',5000,2000,2,5000,'CUST-AUTO','某汽车电子','AE-BGA256-003','PN-BGA256-HI','High','Processing','USR-013','2026-05-22 21:27:55','2026-06-06 21:27:55','2026-05-23 21:27:55',NULL,99.50,94.00,'高端处理器订单','2026-05-25 21:27:55','2026-05-26 21:24:04'),('WO-2026003-ASM','WO-2026003','Child-Assemble','PROD-BGA256','BGA-256 MCU','BGA64-STD:1.0','BGA Standard Route',NULL,'',5000,3000,0,0,'CUST-AUTO','Tesla Auto',NULL,NULL,'High','Processing','','2026-05-18 21:25:00','2026-06-02 21:25:00',NULL,NULL,99.00,98.00,NULL,'2026-05-18 21:25:00','2026-05-26 21:25:00'),('WO-2026003-FT','WO-2026003','Child-Test','PROD-BGA256','BGA-256 MCU FT','BGA64-STD:1.0','BGA Standard Route',NULL,'',5000,2800,0,0,'CUST-AUTO','Tesla Auto',NULL,NULL,'High','Processing','','2026-05-23 21:25:00','2026-06-07 21:25:00',NULL,NULL,99.00,98.00,NULL,'2026-05-23 21:25:00','2026-05-26 21:25:00'),('WO-2026004',NULL,'Parent','PROD-QFN48','QFN-48 传感器','QFN48-STD:1.0','QFN-48 标准路线','SENS-100','QFN',30000,0,6,30000,'CUST-CON','某消费电子','CE-QFN48-004','PN-QFN48-STD','Normal','Created','USR-013','2026-05-27 21:27:55','2026-06-09 21:27:55',NULL,NULL,99.00,95.50,'待开始工单','2026-05-25 21:27:55','2026-05-26 21:24:04'),('WO-2026005',NULL,'Parent','PROD-SOP8','SOP-8 MOSFET','SOP8-STD:1.0','SOP-8 标准路线','MOS-200','SOP',50000,25000,10,50000,'CUST-IND','某工业客户','IND-SOP8-005','PN-SOP8-STD','Low','Processing','USR-013','2026-05-17 21:27:55','2026-05-30 21:27:55','2026-05-18 21:27:55',NULL,99.00,96.00,'大批量MOSFET订单','2026-05-25 21:27:55','2026-05-26 21:24:04'),('WO-2026005-ASM','WO-2026005','Child-Assemble','PROD-SOP8','SOP-8 MOSFET','SOP8-STD:1.0','SOP8 Standard Route',NULL,'',50000,40000,0,0,'CUST-IND','Siemens Industrial',NULL,NULL,'Normal','Processing','','2026-05-14 21:25:00','2026-05-29 21:25:00',NULL,NULL,99.00,98.00,NULL,'2026-05-14 21:25:00','2026-05-26 21:25:00'),('WO-2026005-FT','WO-2026005','Child-Test','PROD-SOP8','SOP-8 MOSFET FT','SOP8-STD:1.0','SOP8 Standard Route',NULL,'',50000,45000,0,0,'CUST-IND','Siemens Industrial',NULL,NULL,'Normal','Processing','','2026-05-19 21:25:00','2026-06-03 21:25:00',NULL,NULL,99.00,98.00,NULL,'2026-05-19 21:25:00','2026-05-26 21:25:00'),('WO-2026006',NULL,'Parent','PROD-BGA64','BGA-64 存储器','BGA64-STD:1.0','BGA-64 标准路线','MEM-500','BGA',10000,6000,4,10000,'CUST-AUTO','某汽车电子','AE-BGA64-006','PN-BGA64-STD','Normal','Processing','USR-013','2026-05-19 21:27:55','2026-06-02 21:27:55','2026-05-20 21:27:55',NULL,99.00,95.00,'存储器订单','2026-05-25 21:27:55','2026-05-26 21:24:04'),('WO-2026006-ASM','WO-2026006','Child-Assemble','PROD-BGA64','BGA-64 Sensor','BGA-STD:1.0','BGA Standard Route',NULL,'',10000,8000,0,0,'CUST-AUTO','Tesla Auto',NULL,NULL,'Normal','Processing','','2026-05-20 21:25:00','2026-06-04 21:25:00',NULL,NULL,99.00,98.00,NULL,'2026-05-20 21:25:00','2026-05-26 21:25:00'),('WO-2026006-FT','WO-2026006','Child-Test','PROD-BGA64','BGA-64 Sensor FT','BGA-STD:1.0','BGA Standard Route',NULL,'',10000,7500,0,0,'CUST-AUTO','Tesla Auto',NULL,NULL,'Normal','Processing','','2026-05-24 21:25:00','2026-06-08 21:25:00',NULL,NULL,99.00,98.00,NULL,'2026-05-24 21:25:00','2026-05-26 21:25:00'),('WO-2026007',NULL,'Parent','PROD-QFN64','QFN-64 驱动器','QFN64-STD:1.0','QFN-64 标准路线','DRV-300','QFN',25000,12000,5,25000,'CUST-AUTO','某汽车电子','AE-QFN64-007','PN-QFN64-STD','High','Processing','USR-013','2026-05-21 21:27:55','2026-06-03 21:27:55','2026-05-22 21:27:55',NULL,99.00,95.50,'驱动器紧急订单','2026-05-25 21:27:55','2026-05-26 21:24:04'),('WO-2026007-ASM','WO-2026007','Child-Assemble','PROD-QFN64','QFN-64 Driver IC','QFN64-STD:1.0','QFN64 Standard Route',NULL,'',25000,20000,0,0,'CUST-AUTO','Tesla Auto',NULL,NULL,'High','Processing','','2026-05-17 21:25:00','2026-06-01 21:25:00',NULL,NULL,99.00,98.00,NULL,'2026-05-17 21:25:00','2026-05-26 21:25:00'),('WO-2026007-FT','WO-2026007','Child-Test','PROD-QFN64','QFN-64 Driver IC FT','QFN64-STD:1.0','QFN64 Standard Route',NULL,'',25000,18000,0,0,'CUST-AUTO','Tesla Auto',NULL,NULL,'High','Processing','','2026-05-22 21:25:01','2026-06-06 21:25:01',NULL,NULL,99.00,98.00,NULL,'2026-05-22 21:25:01','2026-05-26 21:25:01'),('WO-2026008',NULL,'Parent','PROD-SOP14','SOP-14 运放','SOP14-STD:1.0','SOP-14 标准路线','OP-100','SOP',40000,40000,8,40000,'CUST-CON','某消费电子','CE-SOP14-008','PN-SOP14-STD','Normal','Completed','USR-013','2026-05-05 21:27:55','2026-05-20 21:27:55','2026-05-06 21:27:55','2026-05-20 21:27:55',99.00,96.00,'已完成运放订单','2026-05-25 21:27:55','2026-05-26 21:24:04');
/*!40000 ALTER TABLE `prod_work_order` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quality_gate`
--

DROP TABLE IF EXISTS `quality_gate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `quality_gate` (
  `gate_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `gate_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'QA/Customer/MRB',
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Pending',
  `checker_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `check_result` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `check_comment` text COLLATE utf8mb4_unicode_ci,
  `checked_at` datetime DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`gate_id`),
  KEY `idx_lot_id` (`lot_id`),
  KEY `idx_status` (`status`),
  KEY `idx_gate_type` (`gate_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `quality_gate`
--

LOCK TABLES `quality_gate` WRITE;
/*!40000 ALTER TABLE `quality_gate` DISABLE KEYS */;
INSERT INTO `quality_gate` VALUES ('GATE-001','LOT-004','FT','QA','Passed','USR-002','Pass',NULL,'2026-05-18 21:37:36','2026-05-25 21:37:36'),('GATE-002','LOT-004','OQC','QA','Passed','USR-014','Pass',NULL,'2026-05-19 21:37:36','2026-05-25 21:37:36'),('GATE-003','LOT-005','FT','QA','Passed','USR-002','Pass',NULL,'2026-05-19 21:37:36','2026-05-25 21:37:36'),('GATE-004','LOT-005','OQC','QA','Passed','USR-014','Pass',NULL,'2026-05-20 21:37:36','2026-05-25 21:37:36'),('GATE-005','LOT-014','FT','QA','Passed','USR-002','Pass',NULL,'2026-05-17 21:37:36','2026-05-25 21:37:36'),('GATE-006','LOT-014','OQC','QA','Passed','USR-014','Pass',NULL,'2026-05-18 21:37:36','2026-05-25 21:37:36'),('GATE-007','LOT-HOLD-001','DA','QA','Failed','USR-002','Fail',NULL,'2026-05-21 21:37:36','2026-05-25 21:37:36'),('GATE-008','LOT-007','WB','QA','Passed','USR-002','Pass',NULL,'2026-05-25 20:52:22','2026-05-26 20:52:22'),('GATE-009','LOT-008','MOLD','QA','Failed','USR-002','Fail',NULL,'2026-05-24 20:52:22','2026-05-26 20:52:22'),('GATE-010','LOT-011','BALL','QA','Passed','USR-002','Pass',NULL,'2026-05-25 20:52:22','2026-05-26 20:52:22');
/*!40000 ALTER TABLE `quality_gate` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quality_inspection`
--

DROP TABLE IF EXISTS `quality_inspection`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `quality_inspection` (
  `inspection_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `lot_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `step_code` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `inspection_type` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `result` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `inspector_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `inspection_time` datetime NOT NULL,
  `detail` json DEFAULT NULL,
  `remark` text COLLATE utf8mb4_unicode_ci,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`inspection_id`),
  KEY `idx_lot_id` (`lot_id`),
  KEY `idx_inspection_time` (`inspection_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `quality_inspection`
--

LOCK TABLES `quality_inspection` WRITE;
/*!40000 ALTER TABLE `quality_inspection` DISABLE KEYS */;
INSERT INTO `quality_inspection` VALUES ('INSP-001','LOT-004','FT','FinalTest','Pass','USR-019','2026-05-18 21:37:40',NULL,NULL,'2026-05-25 21:37:40'),('INSP-002','LOT-004','OQC','OQC','Pass','USR-020','2026-05-19 21:37:40',NULL,NULL,'2026-05-25 21:37:40'),('INSP-003','LOT-HOLD-001','DA','InProcess','Fail','USR-002','2026-05-21 21:37:40',NULL,NULL,'2026-05-25 21:37:40'),('INSP-004','LOT-007','SAW','InProcess','Pass','USR-002','2026-05-23 21:37:40',NULL,NULL,'2026-05-25 21:37:40'),('INSP-005','LOT-014','FT','FinalTest','Pass','USR-019','2026-05-17 21:37:40',NULL,NULL,'2026-05-25 21:37:40'),('INSP-006','LOT-007','WB','InProcess','Pass','USR-002','2026-05-25 20:52:22',NULL,NULL,'2026-05-26 20:52:22'),('INSP-007','LOT-008','MOLD','InProcess','Fail','USR-002','2026-05-24 20:52:22',NULL,NULL,'2026-05-26 20:52:22'),('INSP-008','LOT-011','BALL','InProcess','Pass','USR-002','2026-05-25 20:52:22',NULL,NULL,'2026-05-26 20:52:22');
/*!40000 ALTER TABLE `quality_inspection` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `report_production_daily`
--

DROP TABLE IF EXISTS `report_production_daily`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `report_production_daily` (
  `report_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `report_date` date NOT NULL,
  `total_lots` int NOT NULL DEFAULT '0',
  `completed_lots` int NOT NULL DEFAULT '0',
  `wip_lots` int NOT NULL DEFAULT '0',
  `hold_lots` int NOT NULL DEFAULT '0',
  `total_input_qty` int NOT NULL DEFAULT '0',
  `total_output_qty` int NOT NULL DEFAULT '0',
  `total_scrap_qty` int NOT NULL DEFAULT '0',
  `overall_yield` decimal(5,2) DEFAULT '0.00',
  `ft_yield` decimal(5,2) DEFAULT '0.00',
  `generated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`report_id`),
  UNIQUE KEY `uk_report_date` (`report_date`),
  KEY `idx_report_date` (`report_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `report_production_daily`
--

LOCK TABLES `report_production_daily` WRITE;
/*!40000 ALTER TABLE `report_production_daily` DISABLE KEYS */;
INSERT INTO `report_production_daily` VALUES ('RPT-001','2026-05-24',15,3,10,2,95000,45000,2500,97.50,96.20,'2026-05-25 21:38:24'),('RPT-002','2026-05-23',12,2,8,2,80000,38000,2000,97.00,95.80,'2026-05-25 21:38:24'),('RPT-003','2026-05-22',10,4,5,1,70000,50000,1500,97.80,96.50,'2026-05-25 21:38:24'),('RPT-004','2026-05-21',8,3,4,1,60000,42000,1200,97.20,96.00,'2026-05-25 21:38:24'),('RPT-005','2026-05-20',10,2,7,1,65000,30000,1800,96.80,95.50,'2026-05-25 21:38:24');
/*!40000 ALTER TABLE `report_production_daily` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_department`
--

DROP TABLE IF EXISTS `sys_department`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_department` (
  `dept_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `dept_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `parent_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `manager_id` varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `status` varchar(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'Active',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`dept_id`),
  KEY `idx_parent_id` (`parent_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_department`
--

LOCK TABLES `sys_department` WRITE;
/*!40000 ALTER TABLE `sys_department` DISABLE KEYS */;
INSERT INTO `sys_department` VALUES ('DEPT-DA','贴片班组','DEPT-PROD','USR-006','Active','2026-05-25 21:12:52'),('DEPT-ENG','工程部',NULL,'USR-003','Active','2026-05-25 21:12:52'),('DEPT-FT','测试班组','DEPT-PROD','USR-009','Active','2026-05-25 21:12:52'),('DEPT-LASER','打标班组','DEPT-PROD','USR-022','Active','2026-05-25 21:12:52'),('DEPT-MAINT','设备维护组','DEPT-ENG','USR-015','Active','2026-05-25 21:12:52'),('DEPT-MOLD','塑封班组','DEPT-PROD','USR-008','Active','2026-05-25 21:12:52'),('DEPT-OQC','出货检验班组','DEPT-QA','USR-014','Active','2026-05-25 21:12:52'),('DEPT-PM','计划部',NULL,'USR-013','Active','2026-05-25 21:12:52'),('DEPT-PMC','后固化班组','DEPT-PROD','USR-021','Active','2026-05-25 21:12:52'),('DEPT-PROD','生产部',NULL,'USR-001','Active','2026-05-25 21:12:52'),('DEPT-QA','品质部',NULL,'USR-002','Active','2026-05-25 21:12:52'),('DEPT-SAW','切割班组','DEPT-PROD','USR-005','Active','2026-05-25 21:12:52'),('DEPT-SING','分条班组','DEPT-PROD','USR-023','Active','2026-05-25 21:12:52'),('DEPT-WB','焊线班组','DEPT-PROD','USR-007','Active','2026-05-25 21:12:52'),('DEPT-WH','仓储部',NULL,'USR-004','Active','2026-05-25 21:12:52');
/*!40000 ALTER TABLE `sys_department` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_role`
--

DROP TABLE IF EXISTS `sys_role`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_role` (
  `role_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `role_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `description` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `level` int NOT NULL DEFAULT '0' COMMENT '权限级别 0-3',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`role_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_role`
--

LOCK TABLES `sys_role` WRITE;
/*!40000 ALTER TABLE `sys_role` DISABLE KEYS */;
INSERT INTO `sys_role` VALUES ('ROLE-ADMIN','系统管理员','系统最高权限',3,'2026-05-25 21:13:05'),('ROLE-ENG','工程师','工程/工艺工程师',2,'2026-05-25 21:13:05'),('ROLE-MANAGER','部门经理','部门管理',2,'2026-05-25 21:13:05'),('ROLE-OPERATOR','操作员','产线操作员',0,'2026-05-25 21:13:05'),('ROLE-PLANNER','计划员','生产计划排程',1,'2026-05-25 21:13:05'),('ROLE-QA','品质工程师','品质检验与放行',2,'2026-05-25 21:13:05'),('ROLE-SUPERVISOR','班组长','产线班组长',1,'2026-05-25 21:13:05');
/*!40000 ALTER TABLE `sys_role` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_user`
--

DROP TABLE IF EXISTS `sys_user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_user` (
  `user_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `user_name` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `password_hash` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `role_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `dept_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `shift` varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT '班次 A/B/C',
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`),
  KEY `idx_role_id` (`role_id`),
  KEY `idx_dept_id` (`dept_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_user`
--

LOCK TABLES `sys_user` WRITE;
/*!40000 ALTER TABLE `sys_user` DISABLE KEYS */;
INSERT INTO `sys_user` VALUES ('USR-001','张经理','JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=','ROLE-ADMIN','DEPT-PROD','A',1,'2026-05-25 21:13:05','2026-05-26 21:17:16'),('USR-002','李品质',NULL,'ROLE-QA','DEPT-QA','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-003','王工程',NULL,'ROLE-ENG','DEPT-ENG','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-004','赵仓储',NULL,'ROLE-SUPERVISOR','DEPT-WH','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-005','孙切割',NULL,'ROLE-SUPERVISOR','DEPT-SAW','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-006','周贴片',NULL,'ROLE-SUPERVISOR','DEPT-DA','B',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-007','吴焊线',NULL,'ROLE-SUPERVISOR','DEPT-WB','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-008','郑塑封',NULL,'ROLE-SUPERVISOR','DEPT-MOLD','B',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-009','陈测试',NULL,'ROLE-SUPERVISOR','DEPT-FT','C',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-010','刘操作员',NULL,'ROLE-OPERATOR','DEPT-SAW','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-011','黄操作员',NULL,'ROLE-OPERATOR','DEPT-DA','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-012','林操作员',NULL,'ROLE-OPERATOR','DEPT-WB','B',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-013','何计划',NULL,'ROLE-PLANNER','DEPT-PM','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-014','钱检验',NULL,'ROLE-QA','DEPT-OQC','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-015','马维护',NULL,'ROLE-ENG','DEPT-MAINT','B',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-016','杨操作员',NULL,'ROLE-OPERATOR','DEPT-MOLD','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-017','许操作员',NULL,'ROLE-OPERATOR','DEPT-FT','B',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-018','曹操作员',NULL,'ROLE-OPERATOR','DEPT-SAW','B',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-019','田操作员',NULL,'ROLE-OPERATOR','DEPT-FT','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-020','潘操作员',NULL,'ROLE-OPERATOR','DEPT-OQC','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-021','蒋固化',NULL,'ROLE-SUPERVISOR','DEPT-PMC','C',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-022','蔡打标',NULL,'ROLE-SUPERVISOR','DEPT-LASER','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-023','贾分条',NULL,'ROLE-SUPERVISOR','DEPT-SING','B',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-024','丁操作员',NULL,'ROLE-OPERATOR','DEPT-PMC','C',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-025','魏操作员',NULL,'ROLE-OPERATOR','DEPT-LASER','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-026','薛操作员',NULL,'ROLE-OPERATOR','DEPT-SING','B',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-027','叶经理',NULL,'ROLE-MANAGER','DEPT-QA','A',1,'2026-05-25 21:13:05','2026-05-25 21:13:05'),('USR-028','阎工程',NULL,'ROLE-ENG','DEPT-ENG','B',1,'2026-05-25 21:13:05','2026-05-25 21:13:05');
/*!40000 ALTER TABLE `sys_user` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sys_user_permission`
--

DROP TABLE IF EXISTS `sys_user_permission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sys_user_permission` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `user_id` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `permission_code` varchar(100) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_permission` (`user_id`,`permission_code`),
  KEY `idx_user_id` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=67 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sys_user_permission`
--

LOCK TABLES `sys_user_permission` WRITE;
/*!40000 ALTER TABLE `sys_user_permission` DISABLE KEYS */;
INSERT INTO `sys_user_permission` VALUES (1,'USR-001','admin.all','2026-05-25 21:13:05'),(2,'USR-001','prod.wo.create','2026-05-25 21:13:05'),(3,'USR-001','prod.lot.hold','2026-05-25 21:13:05'),(4,'USR-001','prod.lot.release','2026-05-25 21:13:05'),(5,'USR-001','prod.lot.scrap','2026-05-25 21:13:05'),(6,'USR-001','prod.lot.rework','2026-05-25 21:13:05'),(7,'USR-002','qa.inspection','2026-05-25 21:13:05'),(8,'USR-002','qa.gate.release','2026-05-25 21:13:05'),(9,'USR-002','prod.lot.hold','2026-05-25 21:13:05'),(10,'USR-002','prod.lot.release','2026-05-25 21:13:05'),(11,'USR-003','eng.route.manage','2026-05-25 21:13:05'),(12,'USR-003','eng.recipe.manage','2026-05-25 21:13:05'),(13,'USR-003','prod.lot.rework','2026-05-25 21:13:05'),(14,'USR-003','prod.lot.scrap','2026-05-25 21:13:05'),(15,'USR-004','wh.receive','2026-05-25 21:13:05'),(16,'USR-004','wh.ship','2026-05-25 21:13:05'),(17,'USR-004','prod.lot.pack','2026-05-25 21:13:05'),(18,'USR-005','prod.trackin','2026-05-25 21:13:05'),(19,'USR-005','prod.trackout','2026-05-25 21:13:05'),(20,'USR-006','prod.trackin','2026-05-25 21:13:05'),(21,'USR-006','prod.trackout','2026-05-25 21:13:05'),(22,'USR-007','prod.trackin','2026-05-25 21:13:05'),(23,'USR-007','prod.trackout','2026-05-25 21:13:05'),(24,'USR-008','prod.trackin','2026-05-25 21:13:05'),(25,'USR-008','prod.trackout','2026-05-25 21:13:05'),(26,'USR-009','prod.trackin','2026-05-25 21:13:05'),(27,'USR-009','prod.trackout','2026-05-25 21:13:05'),(28,'USR-010','prod.trackin','2026-05-25 21:13:05'),(29,'USR-010','prod.trackout','2026-05-25 21:13:05'),(30,'USR-011','prod.trackin','2026-05-25 21:13:05'),(31,'USR-011','prod.trackout','2026-05-25 21:13:05'),(32,'USR-012','prod.trackin','2026-05-25 21:13:05'),(33,'USR-012','prod.trackout','2026-05-25 21:13:05'),(34,'USR-013','prod.wo.create','2026-05-25 21:13:05'),(35,'USR-013','prod.wo.schedule','2026-05-25 21:13:05'),(36,'USR-014','qa.oqc','2026-05-25 21:13:05'),(37,'USR-014','qa.inspection','2026-05-25 21:13:05'),(38,'USR-015','equip.maintenance','2026-05-25 21:13:05'),(39,'USR-015','equip.status','2026-05-25 21:13:05'),(40,'USR-016','prod.trackin','2026-05-25 21:13:05'),(41,'USR-016','prod.trackout','2026-05-25 21:13:05'),(42,'USR-017','prod.trackin','2026-05-25 21:13:05'),(43,'USR-017','prod.trackout','2026-05-25 21:13:05'),(44,'USR-018','prod.trackin','2026-05-25 21:13:05'),(45,'USR-018','prod.trackout','2026-05-25 21:13:05'),(46,'USR-019','prod.trackin','2026-05-25 21:13:05'),(47,'USR-019','prod.trackout','2026-05-25 21:13:05'),(48,'USR-020','prod.trackin','2026-05-25 21:13:05'),(49,'USR-020','prod.trackout','2026-05-25 21:13:05'),(50,'USR-021','prod.trackin','2026-05-25 21:13:05'),(51,'USR-021','prod.trackout','2026-05-25 21:13:05'),(52,'USR-022','prod.trackin','2026-05-25 21:13:05'),(53,'USR-022','prod.trackout','2026-05-25 21:13:05'),(54,'USR-023','prod.trackin','2026-05-25 21:13:05'),(55,'USR-023','prod.trackout','2026-05-25 21:13:05'),(56,'USR-024','prod.trackin','2026-05-25 21:13:05'),(57,'USR-024','prod.trackout','2026-05-25 21:13:05'),(58,'USR-025','prod.trackin','2026-05-25 21:13:05'),(59,'USR-025','prod.trackout','2026-05-25 21:13:05'),(60,'USR-026','prod.trackin','2026-05-25 21:13:05'),(61,'USR-026','prod.trackout','2026-05-25 21:13:05'),(62,'USR-027','qa.inspection','2026-05-25 21:13:05'),(63,'USR-027','qa.gate.release','2026-05-25 21:13:05'),(64,'USR-027','qa.mrb','2026-05-25 21:13:05'),(65,'USR-028','eng.route.manage','2026-05-25 21:13:05'),(66,'USR-028','eng.recipe.manage','2026-05-25 21:13:05');
/*!40000 ALTER TABLE `sys_user_permission` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'mes_prod'
--
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-05-28 23:14:44
