# MES V3 存储分层架构方案

> 创建日期: 2026-05-25
> 状态: 待实施

---

## 一、架构总览

### 废弃的架构
```
IRedisService (统一接口)
  └── MySqlStorageService ──→ mes_kv_store (单表 KV 存储)
       └── 所有数据以 JSON 字符串存储在一张表中
       └── 模拟 Redis 的 String/Hash/List/Set 操作
```

### 新的架构
```
┌─────────────────────────────────────────────────────────┐
│                    L1: IMemoryCache                      │
│  内存缓存层 - 静态配置、频繁读取的只读数据                  │
│  Route信息、Yield规则、权限列表、设备支持路线               │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    L2: MySQL 关系表                       │
│  持久化层 - 业务实体、事务数据、审计数据                    │
│  30+ 张关系表，按业务域分类                                │
│  外键约束 + 索引优化                                      │
└─────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│                    L3: MySQL 归档表                       │
│  归档层 - 历史数据、已完成工单                             │
│  prod_lot_archive (已完成批次迁移)                        │
└─────────────────────────────────────────────────────────┘
```

---

## 二、数据分层策略

| 层级 | 存储方式 | 数据类型 | 示例 | 更新频率 | 保留策略 |
|------|---------|---------|------|---------|---------|
| **L1 内存缓存** | `IMemoryCache` | 静态配置、只读数据 | Route/Steps、Yield规则、用户权限 | 低（配置变更时） | 应用重启失效 |
| **L2 持久化** | MySQL 关系表 | 业务实体、事务数据 | 工单、批次、操作记录、Hold记录 | 高（实时） | 永久保留 |
| **L3 归档** | MySQL 归档表 | 历史数据 | 已完成工单、关闭的批次 | 低（定期迁移） | 按策略归档 |

### L1 内存缓存数据清单
| 数据 | 缓存 Key 模式 | TTL | 失效触发 |
|------|-------------|-----|---------|
| Route 信息 | `route:{route_id}` | 5 分钟 | Route 变更 |
| Route Steps | `route:steps:{route_id}` | 5 分钟 | Route 变更 |
| Yield 规则 | `yield:rules:{route_id}` | 10 分钟 | Yield 规则变更 |
| 用户权限 | `user:perms:{user_id}` | 30 分钟 | 权限变更 |
| 设备支持路线 | `equip:routes:{equip_id}` | 5 分钟 | 设备配置变更 |

### L2 持久化数据清单（按业务域）
| 业务域 | 表名 | 说明 |
|--------|------|------|
| **系统** | `sys_department` | 部门 |
| | `sys_role` | 角色 |
| | `sys_user` | 用户 |
| | `sys_user_permission` | 用户权限关联 |
| **主数据** | `master_product` | 产品 |
| | `master_route` | 工艺路线 |
| | `master_route_step` | 工艺步骤 |
| | `master_equipment` | 设备 |
| | `master_equipment_route` | 设备支持路线 |
| | `master_carrier` | 载具 |
| | `master_recipe` | Recipe |
| | `master_yield_rule` | 良率规则 |
| **生产执行** | `prod_work_order` | 工单 |
| | `prod_lot` | 批次 |
| | `prod_lot_step` | 批次步骤记录 |
| | `prod_operation_history` | 操作历史 |
| | `prod_audit_trail` | 审计追踪 |
| **异常管理** | `prod_hold_record` | Hold 记录 |
| | `prod_scrap_record` | 报废记录 |
| | `prod_rework_record` | 重工记录 |
| **追溯** | `prod_lot_split` | 批次拆分 |
| | `prod_lot_merge` | 批次合并 |
| | `prod_genealogy` | 谱系关系 |
| | `prod_carrier_binding` | 载具绑定 |
| **派工调度** | `prod_dispatch_task` | 派工任务 |
| **质量** | `quality_gate` | 质量 Gate |
| | `quality_inspection` | 检验记录 |
| **报警** | `alarm_rule` | 报警规则 |
| | `alarm_record` | 报警记录 |
| **客户** | `customer_requirement` | 客户要求 |
| **报表** | `report_production_daily` | 生产日报 |
| **归档** | `prod_lot_archive` | 批次归档 |

---

## 三、核心表关系图

```
sys_department ──┐
                 ├── sys_user ──┐
sys_role ────────┘              │
                                │
master_product ──┐              │
                 ├── prod_work_order ──┐
master_route ────┘                     │
                                       │
master_route_step ──┐                  │
                    ├── prod_lot ──────┤
master_equipment ───┤                  │
                    ├── prod_lot_step ─┤
master_carrier ─────┤                  │
                    ├── prod_operation_history
master_recipe ──────┘                  │
                                       │
master_yield_rule ──┐                  │
                    │                  │
prod_hold_record ───┤                  │
prod_scrap_record ──┤                  │
prod_rework_record ─┤                  │
prod_lot_split ─────┤                  │
prod_lot_merge ─────┤                  │
prod_genealogy ─────┤                  │
prod_carrier_binding┤                  │
prod_dispatch_task ─┤                  │
quality_gate ───────┤                  │
quality_inspection ─┤                  │
alarm_record ───────┤                  │
prod_audit_trail ───┘                  │
                                       │
prod_lot_archive ◄── (归档迁移) ────────┘
```

---

## 四、SQL 文件说明

| 文件 | 说明 |
|------|------|
| `docs/sql/mes_schema.sql` | 完整建表脚本（30+ 张表） |
| `docs/sql/mes_mock_data.sql` | 模拟数据脚本（包含完整业务场景） |

### 初始化步骤

```bash
# 1. 确保 MySQL 服务运行
net start MySQL

# 2. 执行建表脚本
mysql -u root < docs/sql/mes_schema.sql

# 3. 执行模拟数据脚本
mysql -u root < docs/sql/mes_mock_data.sql

# 4. 验证
mysql -u root -e "USE mes_prod; SHOW TABLES;"
```

---

## 五、模拟数据场景

| 场景 | 批次 | 状态 | 说明 |
|------|------|------|------|
| **正常生产** | LOT-001 | Processing | SAW 完成，DA 加工中 |
| **已完成** | LOT-COMPLETE-001 | Completed | 11 步全部完成，已入库 |
| **品质 Hold** | LOT-HOLD-001 | Hold | WireBond 断线，待品质确认 |
| **重工批次** | LOT-REWORK-001 | Processing | LOT-HOLD-001 的重工批次 |
| **拆分子批** | LOT-001-S001 | - | LOT-001 拆分出的子批 |
| **Grade 分选** | LOT-COMPLETE-001-GA | - | 车规级 Grade A 分选 |

### 数据量统计
| 实体 | 数量 |
|------|------|
| 部门 | 9 |
| 角色 | 5 |
| 用户 | 12 |
| 产品 | 3 |
| 工艺路线 | 4 |
| 工艺步骤 | 17 |
| 设备 | 13 |
| 载具 | 8 |
| Recipe | 5 |
| 良率规则 | 7 |
| 工单 | 3 |
| 批次 | 5 |
| 批次步骤 | 17 |
| Hold 记录 | 2 |
| 报废记录 | 3 |
| 重工记录 | 1 |
| 拆分记录 | 2 |
| 谱系关系 | 6 |
| 载具绑定 | 7 |
| 派工任务 | 4 |
| 质量 Gate | 2 |
| 检验记录 | 3 |
| 报警规则 | 4 |
| 报警记录 | 3 |
| 客户要求 | 3 |
| 生产日报 | 3 |
| 操作历史 | 8 |
| 审计追踪 | 5 |

---

## 六、后续实施步骤

1. **创建 EF Core Entity 类** - 对应每张表创建实体类
2. **创建 DbContext** - 配置所有 DbSet 和关系
3. **重写 Service 层** - 废弃 `IRedisService` 抽象，直接使用 EF Core
4. **添加 L1 缓存装饰器** - 对静态数据添加 `IMemoryCache` 缓存
5. **迁移现有数据** - 从 `mes_kv_store` 解析 JSON 迁移到新表
6. **更新依赖注入** - 修改 `App.xaml.cs` 和 `Program.cs` 注册
7. **更新测试** - 修改测试用例使用新的数据访问方式
