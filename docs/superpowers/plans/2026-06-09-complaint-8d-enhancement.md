# 客诉8D全功能完善实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完善客诉8D整个栏目的所有项目，包括后端数据模型扩展、6个子表、完整CRUD、8D状态流转、前端UI改造

**Architecture:** 后端扩展 Entity/DTO/Service/Controller → SQL迁移脚本 → 前端数据模型 → ViewModel对接 → 各View界面改造。保持现有代码风格，遵循 DRY/TDD。

**Tech Stack:** C# .NET 8, Entity Framework Core, WPF/Prism, XAML

---

## 文件映射

### 后端新增文件
- `src/Infrastructure/MES.Infrastructure.Persistence/Entities/Complaint8DSubEntities.cs` - 6个子表实体
- `docs/sql/migrations/V3.1.0__complaint_8d_enhancement.sql` - 数据库迁移脚本

### 后端修改文件
- `src/Infrastructure/MES.Infrastructure.Persistence/Entities/SystemEntities.cs:217-240` - Complaint8D Entity扩展
- `src/Infrastructure/MES.Infrastructure.Persistence/MesDbContext.cs:1323-1352` - 新增子表配置
- `src/Shared/MES.Contracts/Quality/Complaint8DDto.cs` - DTO扩展+子表DTO
- `src/Server/Modules/MES.Services.Quality/Complaint8DService.cs` - Service完整重写
- `src/Server/MES.Api/Controllers/ComplaintsController.cs` - API端点扩展

### 前端修改文件
- `src/Client/Modules/MES.Modules.CustomerComplaint/Models/ComplaintModels.cs` - 数据模型扩展
- `src/Client/Modules/MES.Modules.CustomerComplaint/Services/ComplaintService.cs` - 对接API或保留Mock
- `src/Client/Modules/MES.Modules.CustomerComplaint/ViewModels/AllComplaintViewModels.cs` - ViewModel扩展
- `src/Client/Modules/MES.Modules.CustomerComplaint/Views/ComplaintDetailView.xaml` - 详情页改造
- `src/Client/Modules/MES.Modules.CustomerComplaint/Views/ComplaintActionView.xaml` - 编辑页重写
- `src/Client/Modules/MES.Modules.CustomerComplaint/Views/ComplaintReportView.xaml` - 报告页重写
- `src/Client/Modules/MES.Modules.CustomerComplaint/Views/ComplaintListView.xaml` - 列表页增强
- `src/Client/Modules/MES.Modules.CustomerComplaint/Views/ComplaintAnalysisView.xaml` - 分析页增强

---
