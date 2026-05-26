# MES V2 Phase 5 完善 + 测试 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完善 MES V2 现有功能，修复潜在 bug，优化 UI/UX，补充缺失功能，编写单元测试。

**Architecture:** 基于现有 Phase 1-4 代码，进行质量提升和功能补全，不引入新架构。

**Tech Stack:** WPF/Prism, .NET 8, xUnit, Moq

---

### Task 1: 修复潜在 Bug — TrackInViewModel 空引用和异常处理

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/ViewModels/TrackInViewModel.cs`

- [ ] **Step 1: 修复 ExecuteTrackInAsync 中的 null 风险**

当前代码中 `CurrentLotInfo!` 使用了 null-forgiving 操作符，但方法执行时 CurrentLotInfo 可能为 null。添加 null 检查。

```csharp
private async Task ExecuteTrackInAsync()
{
    if (CurrentLotInfo is null)
    {
        StatusMessage = "请先扫描批次";
        return;
    }

    var lot = CurrentLotInfo;
    var routeId = string.IsNullOrEmpty(lot.RouteId) ? "RT-001" : lot.RouteId;

    var request = new TrackInRequest
    {
        LotId = lot.LotId,
        RouteId = routeId,
        RouteVersion = lot.RouteVersion,
        StepSeq = lot.CurrentStepSeq > 0 ? lot.CurrentStepSeq : 1,
        StepCode = lot.CurrentStep,
        StepName = lot.CurrentStep,
        EquipmentId = EquipmentId,
        CarrierId = CarrierId,
        OperatorId = _session.EmployeeId,
        OperatorName = _session.DisplayName,
        Workstation = Environment.MachineName,
        InputQty = lot.UnitCount,
        Remark = string.Empty
    };

    var result = await _trackService.TrackInAsync(request);
    if (result.Success)
    {
        StatusMessage = result.Message;
        var updatedLot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{lot.LotId}");
        if (updatedLot is not null)
        {
            CurrentLotInfo = updatedLot;
            RefreshHoldBanner();
            RunValidation(updatedLot);
        }
        await LoadOperationHistoryAsync(lot.LotId);
    }
    else
    {
        StatusMessage = $"进站失败: {result.Message}";
    }
}
```

- [ ] **Step 2: 修复 ExecuteTrackOutAsync 中的 null 风险**

同样移除 `CurrentLotInfo!`，添加 null 检查。

```csharp
private async Task ExecuteTrackOutAsync()
{
    if (CurrentLotInfo is null)
    {
        StatusMessage = "请先扫描批次";
        return;
    }

    var lot = CurrentLotInfo;
    var routeId = string.IsNullOrEmpty(lot.RouteId) ? "RT-001" : lot.RouteId;

    var request = new TrackOutRequest
    {
        LotId = lot.LotId,
        RouteId = routeId,
        RouteVersion = lot.RouteVersion,
        StepSeq = lot.CurrentStepSeq,
        StepCode = lot.CurrentStep,
        StepName = lot.CurrentStep,
        EquipmentId = EquipmentId,
        OperatorId = _session.EmployeeId,
        OperatorName = _session.DisplayName,
        Workstation = Environment.MachineName,
        InputQty = InputQty,
        PassQty = PassQty,
        FailQty = FailQty,
        ScrapQty = ScrapQty,
        ReworkQty = ReworkQty,
        HoldQty = HoldQty,
        PendingQty = PendingQty,
        Remark = TrackOutRemark
    };

    var validation = await _trackService.ValidateTrackOutAsync(request);
    if (!validation.IsValid)
    {
        StatusMessage = $"出站校验失败: {string.Join("; ", validation.Errors)}";
        ValidationResults.Clear();
        foreach (var err in validation.Errors)
            ValidationResults.Add(new ValidationResult { CheckItem = "校验", Status = ValidationStatus.Fail, Message = err });
        foreach (var warn in validation.Warnings)
            ValidationResults.Add(new ValidationResult { CheckItem = "校验", Status = ValidationStatus.Warning, Message = warn });
        UpdateCommandStates();
        return;
    }

    var result = await _trackService.TrackOutAsync(request);
    if (result.Success)
    {
        StatusMessage = result.Message + (result.NextStepName != null ? $"\n下一站: {result.NextStepName}" : "\n工单已完成");
        var updatedLot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{lot.LotId}");
        if (updatedLot is not null)
        {
            CurrentLotInfo = updatedLot;
            RefreshHoldBanner();
            RunValidation(updatedLot);
        }
        await LoadOperationHistoryAsync(lot.LotId);
        ResetTrackOutInputs();
    }
    else
    {
        StatusMessage = $"出站失败: {result.Message}";
    }
}
```

- [ ] **Step 3: 修复 ForceTrackInAsync 和 ForceTrackOutAsync 中的 null 风险**

同样移除 `CurrentLotInfo!`，添加 null 检查。

```csharp
private async Task ExecuteForceTrackInAsync(string reason)
{
    if (CurrentLotInfo is null)
    {
        StatusMessage = "请先扫描批次";
        return;
    }

    var lot = CurrentLotInfo;
    var routeId = string.IsNullOrEmpty(lot.RouteId) ? "RT-001" : lot.RouteId;

    var request = new TrackInRequest
    {
        LotId = lot.LotId,
        RouteId = routeId,
        RouteVersion = lot.RouteVersion,
        StepSeq = lot.CurrentStepSeq > 0 ? lot.CurrentStepSeq : 1,
        StepCode = lot.CurrentStep,
        StepName = lot.CurrentStep,
        EquipmentId = EquipmentId,
        CarrierId = CarrierId,
        OperatorId = _session.EmployeeId,
        OperatorName = _session.DisplayName,
        Workstation = Environment.MachineName,
        InputQty = lot.UnitCount,
        Remark = $"强制进站: {reason}"
    };

    var result = await _trackService.ForceTrackInAsync(request, reason);
    if (result.Success)
    {
        StatusMessage = result.Message;
        var updatedLot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{lot.LotId}");
        if (updatedLot is not null)
        {
            CurrentLotInfo = updatedLot;
            RefreshHoldBanner();
            RunValidation(updatedLot);
        }
        await LoadOperationHistoryAsync(lot.LotId);
    }
    else
    {
        StatusMessage = $"强制进站失败: {result.Message}";
    }
}

private async Task ExecuteForceTrackOutAsync(string reason)
{
    if (CurrentLotInfo is null)
    {
        StatusMessage = "请先扫描批次";
        return;
    }

    var lot = CurrentLotInfo;
    var routeId = string.IsNullOrEmpty(lot.RouteId) ? "RT-001" : lot.RouteId;

    var request = new TrackOutRequest
    {
        LotId = lot.LotId,
        RouteId = routeId,
        RouteVersion = lot.RouteVersion,
        StepSeq = lot.CurrentStepSeq,
        StepCode = lot.CurrentStep,
        StepName = lot.CurrentStep,
        EquipmentId = EquipmentId,
        OperatorId = _session.EmployeeId,
        OperatorName = _session.DisplayName,
        Workstation = Environment.MachineName,
        InputQty = InputQty,
        PassQty = PassQty,
        FailQty = FailQty,
        ScrapQty = ScrapQty,
        ReworkQty = ReworkQty,
        HoldQty = HoldQty,
        PendingQty = PendingQty,
        Remark = $"强制出站: {reason}"
    };

    var result = await _trackService.ForceTrackOutAsync(request, reason);
    if (result.Success)
    {
        StatusMessage = result.Message;
        var updatedLot = await _redis.GetObjectAsync<LotInfo>($"mes:lot:{lot.LotId}");
        if (updatedLot is not null)
        {
            CurrentLotInfo = updatedLot;
            RefreshHoldBanner();
            RunValidation(updatedLot);
        }
        await LoadOperationHistoryAsync(lot.LotId);
        ResetTrackOutInputs();
    }
    else
    {
        StatusMessage = $"强制出站失败: {result.Message}";
    }
}
```

- [ ] **Step 4: 编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: Build succeeds with 0 errors

---

### Task 2: 修复潜在 Bug — TrackService 空引用和边界条件

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/Services/TrackService.cs`

- [ ] **Step 1: 修复 TrackInAsync 中 lot.OrderId 可能为 null 的问题**

```csharp
// 修改前:
await _opHistoryService.WriteAsync(
    request.LotId, string.IsNullOrEmpty(lot.OrderId) ? "N/A" : lot.OrderId, "TrackIn", ...);

// 修改后:
await _opHistoryService.WriteAsync(
    request.LotId, lot.OrderId ?? "N/A", "TrackIn", ...);
```

- [ ] **Step 2: 修复 TrackOutAsync 中 lot.OrderId 可能为 null 的问题**

```csharp
// 修改前:
await _opHistoryService.WriteAsync(
    request.LotId, string.IsNullOrEmpty(lot.OrderId) ? "N/A" : lot.OrderId, "TrackOut", ...);

// 修改后:
await _opHistoryService.WriteAsync(
    request.LotId, lot.OrderId ?? "N/A", "TrackOut", ...);
```

- [ ] **Step 3: 修复 TrackOutAsync 中良率计算除零风险**

```csharp
// 在 TrackOutAsync 中，计算良率前检查 inputQty
var yield = request.InputQty > 0
    ? await _yieldService.CalculateStepYieldAsync(request.PassQty, request.InputQty)
    : 0.0;
```

- [ ] **Step 4: 编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: Build succeeds with 0 errors

---

### Task 3: 补充缺失功能 — MenuViewModel 添加 Phase 4 新视图导航

**Files:**
- Modify: `src/Client/MES.Shell/ViewModels/MenuViewModel.cs`

- [ ] **Step 1: 更新 Production 模块的 FallbackView 列表**

添加 Phase 3 和 Phase 4 新增的视图到 fallback 列表。

```csharp
private string? GetFallbackView(string moduleKey) => moduleKey switch
{
    "Production" => FindFirstPermitted("Production", "WorkOrderListView", "LotListView", "TrackInView", "WipOverviewView", "LotHoldView", "GenealogyView", "LotSplitMergeView", "MasterDataView", "DispatchView", "ProductionReportView", "YieldReportView", "SystemMonitorView"),
    // ... 其他模块保持不变
};
```

- [ ] **Step 2: 编译验证**

Run: `dotnet build src/Client/MES.Shell/MES.Shell.csproj`
Expected: Build succeeds with 0 errors

---

### Task 4: 优化 UI/UX — 统一 View 样式和主题

**Files:**
- Modify: `src/Client/Modules/MES.Modules.Production/Views/DispatchView.xaml`
- Modify: `src/Client/Modules/MES.Modules.Production/Views/MasterDataView.xaml`
- Modify: `src/Client/Modules/MES.Modules.Production/Views/ProductionReportView.xaml`
- Modify: `src/Client/Modules/MES.Modules.Production/Views/YieldReportView.xaml`
- Modify: `src/Client/Modules/MES.Modules.Production/Views/SystemMonitorView.xaml`

- [ ] **Step 1: 统一所有 View 的 DataGrid 样式**

为所有 DataGrid 添加统一的样式定义，包括行高、列头样式、选中行样式等。

```xml
<DataGrid.Resources>
    <Style TargetType="DataGridColumnHeader">
        <Setter Property="Background" Value="#313244"/>
        <Setter Property="Foreground" Value="#CDD6F4"/>
        <Setter Property="FontWeight" Value="Bold"/>
        <Setter Property="Padding" Value="8,4"/>
    </Style>
    <Style TargetType="DataGridRow">
        <Setter Property="Height" Value="32"/>
        <Style.Triggers>
            <Trigger Property="IsSelected" Value="True">
                <Setter Property="Background" Value="#45475A"/>
            </Trigger>
        </Style.Triggers>
    </Style>
</DataGrid.Resources>
```

- [ ] **Step 2: 为所有 View 添加加载状态指示器**

在每个 View 的顶部添加 Loading 指示器。

```xml
<Grid>
    <Grid.Style>
        <Style TargetType="Grid">
            <Setter Property="Visibility" Value="Collapsed"/>
            <Style.Triggers>
                <DataTrigger Binding="{Binding IsLoading}" Value="True">
                    <Setter Property="Visibility" Value="Visible"/>
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </Grid.Style>
    <TextBlock Text="加载中..." HorizontalAlignment="Center" VerticalAlignment="Center" FontSize="16" Foreground="#CDD6F4"/>
</Grid>
```

- [ ] **Step 3: 为所有 ViewModel 添加 IsLoading 属性**

在 DispatchViewModel、MasterDataViewModel、ProductionReportViewModel、YieldReportViewModel、SystemMonitorViewModel 中添加 IsLoading 属性。

```csharp
private bool _isLoading;
public bool IsLoading
{
    get => _isLoading;
    set => SetProperty(ref _isLoading, value);
}
```

- [ ] **Step 4: 编译验证**

Run: `dotnet build src/Client/Modules/MES.Modules.Production/MES.Modules.Production.csproj`
Expected: Build succeeds with 0 errors

---

### Task 5: 编写单元测试 — TrackService 核心逻辑测试

**Files:**
- Create: `tests/MES.Modules.Production.Tests/Services/TrackServiceTests.cs`
- Create: `tests/MES.Modules.Production.Tests/MES.Modules.Production.Tests.csproj`

- [ ] **Step 1: 创建测试项目**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0"/>
    <PackageReference Include="xunit" Version="2.7.0"/>
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.7"/>
    <PackageReference Include="Moq" Version="4.20.70"/>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\Client\Modules\MES.Modules.Production\MES.Modules.Production.csproj"/>
  </ItemGroup>
</Project>
```

- [ ] **Step 2: 编写 TrackIn 校验测试**

```csharp
public class TrackServiceTests
{
    private readonly Mock<IRedisService> _redisMock;
    private readonly Mock<IRouteService> _routeMock;
    private readonly Mock<IQuantityService> _qtyMock;
    private readonly Mock<IOperationHistoryService> _opHistoryMock;
    private readonly Mock<IAuditService> _auditMock;
    private readonly Mock<IYieldService> _yieldMock;
    private readonly Mock<IGenealogyService> _genealogyMock;
    private readonly Mock<ICarrierService> _carrierMock;
    private readonly TrackService _service;

    public TrackServiceTests()
    {
        _redisMock = new Mock<IRedisService>();
        _routeMock = new Mock<IRouteService>();
        _qtyMock = new Mock<IQuantityService>();
        _opHistoryMock = new Mock<IOperationHistoryService>();
        _auditMock = new Mock<IAuditService>();
        _yieldMock = new Mock<IYieldService>();
        _genealogyMock = new Mock<IGenealogyService>();
        _carrierMock = new Mock<ICarrierService>();

        _service = new TrackService(
            _redisMock.Object,
            _routeMock.Object,
            _qtyMock.Object,
            _opHistoryMock.Object,
            _auditMock.Object,
            _yieldMock.Object,
            _genealogyMock.Object,
            _carrierMock.Object);
    }

    [Fact]
    public async Task ValidateTrackInAsync_LotNotFound_ReturnsError()
    {
        // Arrange
        _redisMock.Setup(r => r.GetObjectAsync<LotInfo>("mes:lot:LOT-999"))
            .ReturnsAsync((LotInfo?)null);

        var request = new TrackInRequest { LotId = "LOT-999" };

        // Act
        var result = await _service.ValidateTrackInAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("不存在", result.Errors[0]);
    }

    [Fact]
    public async Task ValidateTrackInAsync_HoldLot_ReturnsError()
    {
        // Arrange
        var lot = new LotInfo
        {
            LotId = "LOT-001",
            Status = "Hold",
            HoldCategory = HoldType.ManualHold
        };
        _redisMock.Setup(r => r.GetObjectAsync<LotInfo>("mes:lot:LOT-001"))
            .ReturnsAsync(lot);

        var request = new TrackInRequest { LotId = "LOT-001" };

        // Act
        var result = await _service.ValidateTrackInAsync(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Hold", result.Errors[0]);
    }

    [Fact]
    public async Task ValidateTrackInAsync_WaitingLot_ReturnsSuccess()
    {
        // Arrange
        var lot = new LotInfo
        {
            LotId = "LOT-001",
            Status = "Waiting",
            RouteId = "RT-001",
            RouteVersion = "1.0"
        };
        _redisMock.Setup(r => r.GetObjectAsync<LotInfo>("mes:lot:LOT-001"))
            .ReturnsAsync(lot);

        var request = new TrackInRequest { LotId = "LOT-001" };

        // Act
        var result = await _service.ValidateTrackInAsync(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
```

- [ ] **Step 3: 运行测试**

Run: `dotnet test tests/MES.Modules.Production.Tests/MES.Modules.Production.Tests.csproj -v n`
Expected: All tests pass

---

### Task 6: 编写单元测试 — QuantityService 数量平衡测试

**Files:**
- Modify: `tests/MES.Modules.Production.Tests/Services/TrackServiceTests.cs` (or create new file)

- [ ] **Step 1: 编写数量平衡校验测试**

```csharp
public class QuantityServiceTests
{
    private readonly Mock<IRedisService> _redisMock;
    private readonly QuantityService _service;

    public QuantityServiceTests()
    {
        _redisMock = new Mock<IRedisService>();
        _service = new QuantityService(_redisMock.Object);
    }

    [Fact]
    public async Task ValidateTrackOutQuantityAsync_Balanced_ReturnsTrue()
    {
        // Arrange
        var request = new TrackOutRequest
        {
            InputQty = 100,
            PassQty = 90,
            FailQty = 5,
            ScrapQty = 3,
            ReworkQty = 2,
            HoldQty = 0,
            PendingQty = 0
        };

        // Act
        var result = await _service.ValidateTrackOutQuantityAsync(request);

        // Assert
        Assert.True(result.IsBalanced);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateTrackOutQuantityAsync_Unbalanced_ReturnsFalse()
    {
        // Arrange
        var request = new TrackOutRequest
        {
            InputQty = 100,
            PassQty = 90,
            FailQty = 5,
            ScrapQty = 3,
            ReworkQty = 0,
            HoldQty = 0,
            PendingQty = 0
        };

        // Act
        var result = await _service.ValidateTrackOutQuantityAsync(request);

        // Assert
        Assert.False(result.IsBalanced);
        Assert.NotEmpty(result.Errors);
    }
}
```

- [ ] **Step 2: 运行测试**

Run: `dotnet test tests/MES.Modules.Production.Tests/MES.Modules.Production.Tests.csproj -v n`
Expected: All tests pass

---

### Task 7: 最终编译验证 + 全量测试

**Files:**
- All project files

- [ ] **Step 1: 全量编译验证**

Run: `dotnet build`
Expected: Build succeeds with 0 errors, 0 warnings

- [ ] **Step 2: 全量测试运行**

Run: `dotnet test --verbosity normal`
Expected: All tests pass

- [ ] **Step 3: 清理 obj/bin 目录**

Run: `dotnet clean && dotnet build --no-incremental`
Expected: Clean build succeeds
