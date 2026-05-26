# MES V2 Phase 7 性能优化 + 缓存策略 Implementation Plan

## 背景

当前系统使用 MySQL 作为 KV 存储替代 Redis，但存在以下性能问题：

### 问题 1：每次操作创建/销毁 DbContext
`MySqlStorageService` 每次调用都执行 `CreateDbContextAsync()` → `EnsureCreatedAsync()`，导致：
- 每次 TrackIn/TrackOut 操作产生 10+ 次数据库连接
- `EnsureCreatedAsync()` 每次检查表是否存在，开销巨大

### 问题 2：N+1 查询问题
- `RouteService.GetAllRoutesAsync()` 对每个 route ID 单独查询
- `TrackService.ValidateTrackInAsync()` 先查 lot，再查 route steps，再查 prev step record
- `TrackService.TrackOutAsync()` 在 validation 阶段查 lot，执行阶段又查一次

### 问题 3：重复计算
- `TrackOutAsync` 中 `CalculateStepYieldAsync` 被调用 2 次（validation + execution）
- `ShouldAutoHoldAsync` 也被调用 2 次

### 问题 4：缺少内存缓存
- Route 信息（工艺路线）是静态数据，每次都从数据库读取
- Yield 规则是配置数据，不需要每次查询

## 优化方案

### Task 1：DbContext 生命周期管理
**Files:**
- Modify: `src/Client/MES.Infrastructure/Cache/MySqlStorageService.cs`

**Changes:**
- 使用共享 DbContext 实例（scoped lifetime），而非每次创建
- 移除每次调用的 `EnsureCreatedAsync()`，改为初始化时一次性检查
- 添加连接池配置

### Task 2：添加内存缓存层（IMemoryCache）
**Files:**
- Create: `src/Client/MES.Infrastructure/Cache/CachingRedisService.cs`
- Modify: `src/Client/Modules/MES.Modules.Production/ProductionModule.cs`

**Changes:**
- 创建 `CachingRedisService` 装饰器，包装 `MySqlStorageService`
- 对 `mes:route:*` 键添加内存缓存（TTL 5 分钟）
- 对 `mes:yield:rules:*` 键添加内存缓存（TTL 10 分钟）
- 写操作自动失效对应缓存

### Task 3：消除 TrackService 重复查询
**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/Services/TrackService.cs`

**Changes:**
- `TrackOutAsync` 复用 validation 阶段获取的 lot 对象
- 将 validation 和执行合并为单次 lot 读取
- 缓存 yield 计算结果，避免重复调用

### Task 4：RouteService 批量查询优化
**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/Services/RouteService.cs`

**Changes:**
- `GetAllRoutesAsync()` 使用批量查询替代 N+1
- 添加 `GetStepsAsync` 缓存

### Task 5：添加性能监控
**Files:**
- Create: `src/Client/MES.Infrastructure/Cache/PerformanceMonitor.cs`

**Changes:**
- 记录每次存储操作的耗时
- 统计缓存命中率
- 提供慢查询日志

### Task 6：编译验证 + 性能基准测试
**Files:**
- Create: `test/MES.Modules.Production.Tests/PerformanceBenchmarkTests.cs`

**Changes:**
- 编写基准测试对比优化前后的性能
- 验证功能正确性不受影响

## 验收标准

1. TrackIn/TrackOut 操作的数据库查询次数减少 50%+
2. Route 信息缓存命中率 > 90%
3. 全量测试通过（45+ 测试）
4. 无功能回归
