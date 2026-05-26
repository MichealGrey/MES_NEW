# MES - 半导体封测厂制造执行系统

> 面向半导体封装测试工厂的完整 MES 解决方案，覆盖从来料接收到成品出货的全流程管控。

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![WPF](https://img.shields.io/badge/UI-WPF-purple.svg)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![Prism](https://img.shields.io/badge/Framework-Prism-green.svg)](https://prismlibrary.com/)
[![MySQL](https://img.shields.io/badge/Database-MySQL-orange.svg)](https://www.mysql.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

---

## 项目简介

本项目是一个面向半导体封测厂（OSAT）的制造执行系统（MES），采用 **WPF + Prism + ASP.NET Core** 架构，实现从晶圆来料、封装测试到成品出货的全流程数字化管控。

### 核心能力

- **工单管理** - 母/子工单拆分，前后道工艺分离（晶圆/CP 前道 vs 封装/FT 后道）
- **批次追踪** - Wafer Lot → Mother Lot → Sub Lot 完整层级追溯
- **进站/出站** - 12 项校验链，设备/配方/质量联动卡控
- **异常管控** - Hold/Release/Scrap/Rework/MRB 全流程异常处理
- **良率管理** - 实时良率计算、Wafer Map 可视化、趋势分析
- **客户交付** - 客户批次查询、交期风险预警、生产进度跟踪
- **质量追溯** - 完整 Genealogy 追溯链，支持正向/反向追溯
- **权限控制** - 基于角色的访问控制（RBAC），电子签核（L0-L3）

---

## 技术栈

| 层级 | 技术 |
|------|------|
| **客户端** | WPF + Prism (DryIoc) + MVVM |
| **服务端** | ASP.NET Core 8.0 Web API |
| **数据库** | MySQL 8.0 + Entity Framework Core |
| **架构模式** | 模块化 + 领域驱动设计 (DDD) |
| **通信** | RESTful API + 事件总线 |

---

## 项目结构

```
MES_NEW/
├── src/
│   ├── Client/
│   │   ├── MES.Shell/              # WPF 主程序壳（登录、菜单、导航）
│   │   ├── MES.Client.Core/        # 客户端核心服务（API 通信）
│   │   ├── MES.Shared/             # 共享组件（转换器、会话服务）
│   │   ├── MES.Infrastructure/     # 客户端基础设施（缓存、性能监控）
│   │   └── Modules/                # 功能模块
│   │       ├── MES.Modules.Production/   # 生产管理（工单、批次、进站/出站）
│   │       ├── MES.Modules.Equipment/    # 设备管理
│   │       ├── MES.Modules.Quality/      # 质量管理
│   │       ├── MES.Modules.Yield/        # 良率管理
│   │       ├── MES.Modules.Trace/        # 追溯管理
│   │       ├── MES.Modules.Warehouse/    # 仓储管理
│   │       ├── MES.Modules.Alarm/        # 报警管理
│   │       ├── MES.Modules.Recipe/       # 配方管理
│   │       ├── MES.Modules.Schedule/     # 排程管理
│   │       └── MES.Modules.EHS/          # 环境健康安全
│   ├── Infrastructure/
│   │   └── MES.Infrastructure.Persistence/  # 数据持久化（EF Core、实体定义）
│   ├── Domain/                     # 领域层（实体、值对象、领域事件）
│   └── Contracts/                  # 接口契约（DTO、服务接口）
├── docs/                           # 项目文档
│   ├── 半导体封测厂 MES 业务场景 20 例.md
│   ├── 生产管理菜单改善方案.md
│   ├── Overall_Rewrite_Plan.md
│   ├── V2_Architecture_Design.md
│   └── sql/                        # 数据库迁移脚本
└── MES_NEW.sln                     # 解决方案文件
```

---

## 业务场景

系统覆盖半导体封测厂 **20 个核心业务场景**，涵盖 4 个难度级别：

| 难度 | 数量 | 示例 |
|------|------|------|
| 🟢 简单 | 3 | 标准工单流转、来料晶圆登记 |
| 🟡 中等 | 7 | 重工流程、拆批/合批、客户加急 |
| 🔴 复杂 | 10 | 多维度 Hold、良率异常自动拦截、委外加工 |
| 🔥 极其复杂 | 10 | 紧急插单、混合工艺路线、客户审计追溯 |

详细场景定义见：[半导体封测厂 MES 业务场景 20 例](docs/半导体封测厂%20MES%20业务场景%2020%20例.md)

---

## 快速开始

### 环境要求

- .NET 8.0 SDK
- MySQL 8.0+
- Windows 10/11（WPF 客户端）

### 数据库配置

1. 创建 MySQL 数据库：
```sql
CREATE DATABASE mes_prod CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

2. 运行迁移脚本：
```bash
mysql -u root -p mes_prod < docs/sql/migration_improvement_plan.sql
```

3. 配置连接字符串（根据实际环境修改）：
```
Server=localhost;Database=mes_prod;Uid=root;Pwd=YourPassword;
```

### 构建与运行

```bash
# 还原依赖
dotnet restore

# 构建
dotnet build MES_NEW.sln

# 运行客户端
dotnet run --project src/Client/MES.Shell/MES.Shell.csproj
```

### 测试账号

| 工号 | 密码 | 角色 | 权限 |
|------|------|------|------|
| admin | Admin@123 | 系统管理员 | 全部权限 |
| eng001 | Eng@123 | 工程师 | 生产操作、工艺管理 |
| op001 | Op@123 | 操作员 | 进站/出站操作 |
| qa001 | Qa@123 | 质量工程师 | 质量检验、Hold/Release |

---

## 开发路线图

```
Phase 1 ✅ V1 最小闭环
  Route + Track + Quantity + History + 基础 Hold + 基础 Audit + MySQL

Phase 2 ✅ V2 异常闭环
  完整 Hold/Release + 电子签核 + Scrap + Rework + ForceTrack

Phase 3 🔄 V3 追溯变更（进行中）
  Lot Genealogy + Split + Merge + Carrier 绑定 + 物料绑定

Phase 4 📋 V4 联动客户交付
  Equipment/Recipe/Quality/Warehouse 联动 + 客户批次查询 + 交期风险

Phase 5 📋 V5 正式上线
  主数据 + 报表 + 运维 + 数据治理 + 外部系统对接
```

详细规划见：[Overall Rewrite Plan](docs/Overall_Rewrite_Plan.md)

---

## 核心文档

| 文档 | 说明 |
|------|------|
| [业务场景 20 例](docs/半导体封测厂%20MES%20业务场景%2020%20例.md) | 完整业务场景定义与验收标准 |
| [生产管理改善方案](docs/生产管理菜单改善方案.md) | 前后道工艺分离、权限体系、菜单重构 |
| [架构设计 V2](docs/V2_Architecture_Design.md) | 系统架构与模块设计 |
| [整体改写规划](docs/Overall_Rewrite_Plan.md) | 分阶段实施计划 |
| [MySQL 迁移指南](docs/MYSQL_MIGRATION.md) | 数据库迁移步骤 |
| [核心闭环方案](docs/Production%20核心闭环%20MES%20方案.md) | 生产核心业务闭环设计 |

---

## 贡献指南

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 提交 Pull Request

---

## License

本项目采用 MIT License - 详见 [LICENSE](LICENSE) 文件
