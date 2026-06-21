-- V3.1.0: Complaint 8D Enhancement
-- Adds new columns to complaint_8d table and creates 6 sub-tables
-- for team members, containments, root causes, actions, doc updates, and attachments

-- ============================================================================
-- 1. ALTER complaint_8d - Add new columns
-- ============================================================================

-- D0 准备阶段
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d0_assessment TINYINT(1) COMMENT 'D0评估结果';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d0_assessment_comment TEXT COMMENT 'D0评估备注';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d0_date DATETIME COMMENT 'D0日期';

-- D1 团队日期
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d1_date DATETIME COMMENT 'D1完成日期';

-- D2 5W2H 详细字段
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_what TEXT COMMENT 'D2-What(什么问题)';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_who VARCHAR(200) COMMENT 'D2-Who(谁发现)';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_where VARCHAR(200) COMMENT 'D2-Where(在哪里)';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_when VARCHAR(200) COMMENT 'D2-When(何时)';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_why TEXT COMMENT 'D2-Why(为什么)';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_how TEXT COMMENT 'D2-How(如何发现)';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_how_many VARCHAR(200) COMMENT 'D2-How Many(多少)';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_defect_location VARCHAR(200) COMMENT 'D2缺陷位置';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_occurrence_date DATETIME COMMENT 'D2发生日期';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_discovery_date DATETIME COMMENT 'D2发现日期';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_discovery_method VARCHAR(100) COMMENT 'D2发现方式';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d2_date DATETIME COMMENT 'D2完成日期';

-- D3 围堵日期
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d3_date DATETIME COMMENT 'D3完成日期';

-- D4 原因分析详细
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d4_occurrence_cause TEXT COMMENT 'D4发生原因';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d4_escape_cause TEXT COMMENT 'D4流出原因';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d4_date DATETIME COMMENT 'D4完成日期';

-- D5 纠正措施详细
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d5_validation_date DATETIME COMMENT 'D5验证日期';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d5_date DATETIME COMMENT 'D5完成日期';

-- D6 实施验证详细
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d6_implement_date DATETIME COMMENT 'D6实施日期';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d6_date DATETIME COMMENT 'D6完成日期';

-- D7 预防措施详细
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d7_standardization TEXT COMMENT 'D7标准化';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d7_horizontal_expand TEXT COMMENT 'D7水平展开';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d7_date DATETIME COMMENT 'D7完成日期';

-- D8 结案详细
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d8_team_recognition TEXT COMMENT 'D8团队认可';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d8_effectiveness_confirm TEXT COMMENT 'D8有效性确认';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS d8_date DATETIME COMMENT 'D8完成日期';

-- 8D状态与审批
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS eight_d_status VARCHAR(10) DEFAULT 'D0' COMMENT '8D当前步骤: D0-D8';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS approval_status VARCHAR(20) DEFAULT 'Pending' COMMENT '审批状态: Pending/Approved/Rejected';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS approver VARCHAR(50) COMMENT '审批人';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS approve_date DATETIME COMMENT '审批日期';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS approval_comment TEXT COMMENT '审批备注';

-- 新增字段
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS order_no VARCHAR(50) COMMENT '订单号';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS customer_po_no VARCHAR(50) COMMENT '客户PO号';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS product_name VARCHAR(100) COMMENT '产品名称';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS priority VARCHAR(20) DEFAULT 'Normal' COMMENT '优先级: Low/Normal/High/Urgent';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS affected_qty INT DEFAULT 0 COMMENT '受影响数量';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS return_qty INT DEFAULT 0 COMMENT '退货数量';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS sample_qty INT DEFAULT 0 COMMENT '样品数量';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS attachments JSON COMMENT '附件JSON';
ALTER TABLE complaint_8d ADD COLUMN IF NOT EXISTS remark TEXT COMMENT '备注';

-- 添加索引
ALTER TABLE complaint_8d ADD INDEX IF NOT EXISTS idx_eight_d_status (eight_d_status);
ALTER TABLE complaint_8d ADD INDEX IF NOT EXISTS idx_approval_status (approval_status);

-- ============================================================================
-- 2. complaint_8d_team_member - D1 团队成员明细
-- ============================================================================
CREATE TABLE IF NOT EXISTS complaint_8d_team_member (
    member_id VARCHAR(50) PRIMARY KEY COMMENT '成员ID',
    complaint_id VARCHAR(50) NOT NULL COMMENT '8D投诉ID',
    member_name VARCHAR(50) NOT NULL COMMENT '成员姓名',
    department VARCHAR(50) NOT NULL COMMENT '所属部门',
    role VARCHAR(50) NOT NULL COMMENT '角色: 组长/质量/工程/生产/采购等',
    contact_info VARCHAR(100) COMMENT '联系方式',
    join_date DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '加入日期',
    remark TEXT COMMENT '备注',
    INDEX idx_complaint (complaint_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='8D投诉-团队成员明细';

-- ============================================================================
-- 3. complaint_8d_containment - D3 围堵措施明细
-- ============================================================================
CREATE TABLE IF NOT EXISTS complaint_8d_containment (
    containment_id VARCHAR(50) PRIMARY KEY COMMENT '围堵措施ID',
    complaint_id VARCHAR(50) NOT NULL COMMENT '8D投诉ID',
    action_description TEXT NOT NULL COMMENT '措施描述',
    affected_lot VARCHAR(50) COMMENT '受影响批次',
    affected_qty INT DEFAULT 0 COMMENT '受影响数量',
    contained_qty INT DEFAULT 0 COMMENT '已围堵数量',
    disposition VARCHAR(50) COMMENT '处置方式: 退货/返工/报废/挑选/让步接收',
    result TEXT COMMENT '围堵结果',
    responsible_person VARCHAR(50) COMMENT '负责人',
    plan_date DATETIME COMMENT '计划日期',
    actual_date DATETIME COMMENT '实际日期',
    status VARCHAR(20) DEFAULT 'Pending' COMMENT '状态: Pending/InProgress/Completed',
    remark TEXT COMMENT '备注',
    INDEX idx_complaint (complaint_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='8D投诉-围堵措施明细';

-- ============================================================================
-- 4. complaint_8d_root_cause - D4 原因分析明细
-- ============================================================================
CREATE TABLE IF NOT EXISTS complaint_8d_root_cause (
    cause_id VARCHAR(50) PRIMARY KEY COMMENT '原因ID',
    complaint_id VARCHAR(50) NOT NULL COMMENT '8D投诉ID',
    cause_type VARCHAR(20) NOT NULL COMMENT '原因类型: Occurrence(发生)/Escape(流出)',
    analysis_method VARCHAR(50) COMMENT '分析方法: 鱼骨图/5Why/FMEA/FTA等',
    cause_description TEXT NOT NULL COMMENT '原因描述',
    why_1 TEXT COMMENT 'Why 1',
    why_2 TEXT COMMENT 'Why 2',
    why_3 TEXT COMMENT 'Why 3',
    why_4 TEXT COMMENT 'Why 4',
    why_5 TEXT COMMENT 'Why 5',
    root_cause_conclusion TEXT COMMENT '根本原因结论',
    responsible_person VARCHAR(50) COMMENT '分析负责人',
    analysis_date DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '分析日期',
    remark TEXT COMMENT '备注',
    INDEX idx_complaint (complaint_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='8D投诉-原因分析明细';

-- ============================================================================
-- 5. complaint_8d_action - D5/D6 纠正措施明细
-- ============================================================================
CREATE TABLE IF NOT EXISTS complaint_8d_action (
    action_id VARCHAR(50) PRIMARY KEY COMMENT '措施ID',
    complaint_id VARCHAR(50) NOT NULL COMMENT '8D投诉ID',
    action_type VARCHAR(20) NOT NULL COMMENT '措施类型: Corrective(纠正)/Preventive(预防)',
    action_description TEXT NOT NULL COMMENT '措施描述',
    responsible_person VARCHAR(50) NOT NULL COMMENT '负责人',
    plan_date DATETIME COMMENT '计划日期',
    actual_date DATETIME COMMENT '实际日期',
    status VARCHAR(20) DEFAULT 'Pending' COMMENT '状态: Pending/InProgress/Completed/Verified',
    verification_method VARCHAR(200) COMMENT '验证方法',
    verification_result TEXT COMMENT '验证结果',
    verification_date DATETIME COMMENT '验证日期',
    remark TEXT COMMENT '备注',
    INDEX idx_complaint (complaint_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='8D投诉-纠正措施明细';

-- ============================================================================
-- 6. complaint_8d_doc_update - D7 文件更新记录
-- ============================================================================
CREATE TABLE IF NOT EXISTS complaint_8d_doc_update (
    doc_id VARCHAR(50) PRIMARY KEY COMMENT '文档ID',
    complaint_id VARCHAR(50) NOT NULL COMMENT '8D投诉ID',
    doc_type VARCHAR(50) NOT NULL COMMENT '文档类型: SOP/SIP/FMEA/ControlPlan/WorkInstruction等',
    doc_name VARCHAR(200) NOT NULL COMMENT '文档名称',
    doc_no VARCHAR(50) COMMENT '文档编号',
    update_description TEXT NOT NULL COMMENT '更新说明',
    responsible_person VARCHAR(50) COMMENT '负责人',
    plan_date DATETIME COMMENT '计划日期',
    actual_date DATETIME COMMENT '实际日期',
    status VARCHAR(20) DEFAULT 'Pending' COMMENT '状态: Pending/Completed',
    remark TEXT COMMENT '备注',
    INDEX idx_complaint (complaint_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='8D投诉-文件更新记录';

-- ============================================================================
-- 7. complaint_8d_attachment - 附件管理
-- ============================================================================
CREATE TABLE IF NOT EXISTS complaint_8d_attachment (
    attachment_id VARCHAR(50) PRIMARY KEY COMMENT '附件ID',
    complaint_id VARCHAR(50) NOT NULL COMMENT '8D投诉ID',
    file_name VARCHAR(200) NOT NULL COMMENT '文件名',
    file_path VARCHAR(500) NOT NULL COMMENT '文件路径',
    file_type VARCHAR(20) COMMENT '文件类型: image/pdf/excel/word/other',
    file_size BIGINT DEFAULT 0 COMMENT '文件大小(字节)',
    upload_stage VARCHAR(10) COMMENT '上传阶段: D0/D1/D2/D3/D4/D5/D6/D7/D8',
    uploaded_by VARCHAR(50) NOT NULL COMMENT '上传人',
    uploaded_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT '上传时间',
    remark TEXT COMMENT '备注',
    INDEX idx_complaint (complaint_id),
    INDEX idx_upload_stage (upload_stage)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='8D投诉-附件管理';
