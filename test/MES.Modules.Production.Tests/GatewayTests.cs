using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Moq;
using Xunit;

namespace MES.Modules.Production.Tests;

public class GatewayTests
{
    private readonly Mock<IMasterDataService> _masterDataMock;

    public GatewayTests()
    {
        _masterDataMock = new Mock<IMasterDataService>();
    }

    [Fact]
    public async Task EquipmentGateway_CheckEquipment_NullEquipment_ReturnsNotAllowed()
    {
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync((EquipmentInfo?)null);

        var gateway = new EquipmentGateway(_masterDataMock.Object);
        var result = await gateway.CheckEquipmentAsync("EQ-001", "S1");

        Assert.False(result.IsAllowed);
        Assert.Contains("不存在", result.Reason ?? string.Empty);
    }

    [Fact]
    public async Task EquipmentGateway_CheckEquipment_OfflineEquipment_ReturnsNotAllowed()
    {
        var equipment = new EquipmentInfo
        {
            EquipmentId = "EQ-001",
            EquipmentName = "测试设备",
            EquipmentGroup = "Saw",
            Status = "Offline"
        };
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync(equipment);

        var gateway = new EquipmentGateway(_masterDataMock.Object);
        var result = await gateway.CheckEquipmentAsync("EQ-001", "S1");

        Assert.False(result.IsAllowed);
        Assert.Contains("Offline", result.Reason ?? string.Empty);
    }

    [Fact]
    public async Task EquipmentGateway_CheckEquipment_AvailableEquipment_ReturnsAllowed()
    {
        var equipment = new EquipmentInfo
        {
            EquipmentId = "EQ-001",
            EquipmentName = "测试设备",
            EquipmentGroup = "Saw",
            Status = "Available"
        };
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync(equipment);

        var gateway = new EquipmentGateway(_masterDataMock.Object);
        var result = await gateway.CheckEquipmentAsync("EQ-001", "S1");

        Assert.True(result.IsAllowed);
    }

    [Fact]
    public async Task EquipmentGateway_CheckEquipment_RunningEquipment_ReturnsAllowed()
    {
        var equipment = new EquipmentInfo
        {
            EquipmentId = "EQ-001",
            EquipmentName = "测试设备",
            EquipmentGroup = "Saw",
            Status = "Running"
        };
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync(equipment);

        var gateway = new EquipmentGateway(_masterDataMock.Object);
        var result = await gateway.CheckEquipmentAsync("EQ-001", "S1");

        Assert.True(result.IsAllowed);
    }

    [Fact]
    public async Task EquipmentGateway_IsEquipmentAvailableAsync_TrueForAvailable()
    {
        var equipment = new EquipmentInfo { EquipmentId = "EQ-001", Status = "Available" };
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync(equipment);

        var gateway = new EquipmentGateway(_masterDataMock.Object);
        var result = await gateway.IsEquipmentAvailableAsync("EQ-001");

        Assert.True(result);
    }

    [Fact]
    public async Task EquipmentGateway_IsEquipmentAvailableAsync_FalseForMaintenance()
    {
        var equipment = new EquipmentInfo { EquipmentId = "EQ-001", Status = "Maintenance" };
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync(equipment);

        var gateway = new EquipmentGateway(_masterDataMock.Object);
        var result = await gateway.IsEquipmentAvailableAsync("EQ-001");

        Assert.False(result);
    }

    [Fact]
    public async Task RecipeGateway_IsRecipeApproved_InactiveRecipe_ReturnsFalse()
    {
        var recipe = new RecipeInfo
        {
            RecipeId = "RC-001",
            RecipeName = "测试 Recipe",
            EquipmentGroup = "Saw",
            StepCode = "S1",
            IsActive = false
        };
        _masterDataMock.Setup(m => m.GetRecipeAsync("RC-001")).ReturnsAsync(recipe);

        var gateway = new RecipeGateway(_masterDataMock.Object);
        var result = await gateway.IsRecipeApprovedAsync("RC-001", "S1", "EQ-001");

        Assert.False(result);
    }

    [Fact]
    public async Task RecipeGateway_IsRecipeApproved_ActiveRecipe_ReturnsTrue()
    {
        var recipe = new RecipeInfo
        {
            RecipeId = "RC-001",
            RecipeName = "测试 Recipe",
            EquipmentGroup = "Saw",
            StepCode = "S1",
            IsActive = true
        };
        var equipment = new EquipmentInfo
        {
            EquipmentId = "EQ-001",
            EquipmentGroup = "Saw",
            Status = "Available"
        };
        _masterDataMock.Setup(m => m.GetRecipeAsync("RC-001")).ReturnsAsync(recipe);
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync(equipment);

        var gateway = new RecipeGateway(_masterDataMock.Object);
        var result = await gateway.IsRecipeApprovedAsync("RC-001", "S1", "EQ-001");

        Assert.True(result);
    }

    [Fact]
    public async Task RecipeGateway_IsRecipeApproved_NoRecipeId_ReturnsTrue()
    {
        var gateway = new RecipeGateway(_masterDataMock.Object);
        var result = await gateway.IsRecipeApprovedAsync("", "S1", "EQ-001");

        Assert.True(result);
    }

    [Fact]
    public async Task RecipeGateway_IsRecipeMatchEquipment_MatchingGroup_ReturnsTrue()
    {
        var recipe = new RecipeInfo { RecipeId = "RC-001", EquipmentGroup = "Saw" };
        var equipment = new EquipmentInfo { EquipmentId = "EQ-001", EquipmentGroup = "Saw" };
        _masterDataMock.Setup(m => m.GetRecipeAsync("RC-001")).ReturnsAsync(recipe);
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync(equipment);

        var gateway = new RecipeGateway(_masterDataMock.Object);
        var result = await gateway.IsRecipeMatchEquipmentAsync("RC-001", "EQ-001");

        Assert.True(result);
    }

    [Fact]
    public async Task RecipeGateway_IsRecipeMatchEquipment_MismatchedGroup_ReturnsFalse()
    {
        var recipe = new RecipeInfo { RecipeId = "RC-001", EquipmentGroup = "Saw" };
        var equipment = new EquipmentInfo { EquipmentId = "EQ-001", EquipmentGroup = "WB" };
        _masterDataMock.Setup(m => m.GetRecipeAsync("RC-001")).ReturnsAsync(recipe);
        _masterDataMock.Setup(m => m.GetEquipmentAsync("EQ-001")).ReturnsAsync(equipment);

        var gateway = new RecipeGateway(_masterDataMock.Object);
        var result = await gateway.IsRecipeMatchEquipmentAsync("RC-001", "EQ-001");

        Assert.False(result);
    }

    [Fact]
    public async Task QualityGateway_IsQualityGatePassed_NoGates_ReturnsTrue()
    {
        var gateRepo = new Mock<IRepository<QualityGateEntity>>();
        gateRepo.Setup(r => r.GetWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<QualityGateEntity, bool>>>()))
                .ReturnsAsync(new List<QualityGateEntity>());

        var gateway = new QualityGateway(gateRepo.Object);
        var result = await gateway.IsQualityGatePassedAsync("LOT-001", "S1");

        Assert.True(result);
    }

    [Fact]
    public async Task QualityGateway_RequiresQualityGate_ReturnsFalseWhenNoGates()
    {
        var gateRepo = new Mock<IRepository<QualityGateEntity>>();
        gateRepo.Setup(r => r.GetWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<QualityGateEntity, bool>>>()))
                .ReturnsAsync(new List<QualityGateEntity>());

        var gateway = new QualityGateway(gateRepo.Object);
        var result = await gateway.RequiresQualityGateAsync("LOT-001", "S1");

        Assert.False(result);
    }

    [Fact]
    public async Task WarehouseGateway_IsMaterialReady_NoRequirements_ReturnsTrue()
    {
        var materialRepo = new Mock<IRepository<MasterMaterial>>();
        var consumeRepo = new Mock<IRepository<MaterialConsumeEntity>>();
        var requirementRepo = new Mock<IRepository<MaterialRequirement>>();
        requirementRepo.Setup(r => r.GetWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<MaterialRequirement, bool>>>()))
                       .ReturnsAsync(new List<MaterialRequirement>());

        var gateway = new WarehouseGateway(materialRepo.Object, consumeRepo.Object, requirementRepo.Object);
        var result = await gateway.IsMaterialReadyAsync("LOT-001", "S1");

        Assert.True(result);
    }
}
