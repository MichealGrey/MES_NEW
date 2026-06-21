using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics.Tests;

[Trait("Category", "UnitTest")]
[Trait("Phase", "5")]
public class SystemConfigServiceTests
{
    private readonly Mock<ILogger<SystemConfigService>> _mockLogger = new();

    private MesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MesDbContext(options);
    }

    [Fact]
    public async Task GetConfigsAsync_ReturnsAllConfigs()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new SystemConfigService(context, _mockLogger.Object);

        var query = new SystemConfigQuery { PageIndex = 1, PageSize = 20 };

        // Act
        var result = await service.GetConfigsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        // Empty database returns empty result
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetConfigByKeyAsync_WhenFound_ReturnsConfig()
    {
        // Arrange
        var context = CreateDbContext();

        // Seed a config directly
        context.SystemConfigs.Add(new MES.Infrastructure.Persistence.Entities.SystemConfig
        {
            ConfigKey = "test.config.key",
            ConfigValue = "test-value",
            ConfigType = "string",
            Category = "Test",
            Description = "Test config"
        });
        await context.SaveChangesAsync();

        var service = new SystemConfigService(context, _mockLogger.Object);

        // Act
        var result = await service.GetConfigByKeyAsync("test.config.key");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test.config.key", result.ConfigKey);
        Assert.Equal("test-value", result.ConfigValue);
    }

    [Fact]
    public async Task GetConfigByKeyAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new SystemConfigService(context, _mockLogger.Object);

        // Act
        var result = await service.GetConfigByKeyAsync("nonexistent.key");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateConfigAsync_WithValidData_UpdatesConfig()
    {
        // Arrange
        var context = CreateDbContext();

        context.SystemConfigs.Add(new MES.Infrastructure.Persistence.Entities.SystemConfig
        {
            ConfigKey = "update.test.key",
            ConfigValue = "old-value",
            ConfigType = "string",
            Category = "Test"
        });
        await context.SaveChangesAsync();

        var service = new SystemConfigService(context, _mockLogger.Object);
        var request = new UpdateSystemConfigRequest
        {
            ConfigKey = "update.test.key",
            ConfigValue = "new-value",
            ConfigType = "string",
            Category = "Test",
            Description = "Updated description"
        };

        // Act
        var result = await service.UpdateConfigAsync(request, "admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("update.test.key", result.ConfigKey);
        Assert.Equal("new-value", result.ConfigValue);
        Assert.Equal("Updated description", result.Description);
    }

    [Fact]
    public async Task UpdateConfigAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new SystemConfigService(context, _mockLogger.Object);
        var request = new UpdateSystemConfigRequest
        {
            ConfigKey = "nonexistent.key",
            ConfigValue = "some-value"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateConfigAsync(request, "admin"));
    }

    [Fact]
    public async Task CreateAlertRuleAsync_WithValidData_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new SystemConfigService(context, _mockLogger.Object);
        var request = new CreateAlertRuleRequest
        {
            RuleName = "Yield Alert",
            RuleType = "YieldThreshold",
            ConditionExpression = "yield < 90",
            ThresholdValue = 90m,
            Severity = "Critical",
            NotificationChannels = "email",
            Enabled = true
        };

        // Act
        var result = await service.CreateAlertRuleAsync(request, "admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Yield Alert", result.RuleName);
        Assert.Equal("YieldThreshold", result.RuleType);
        Assert.True(result.Enabled);
        Assert.NotNull(result.RuleId);
        Assert.StartsWith("ARULE-", result.RuleId);
    }

    [Fact]
    public async Task QueryAlertRulesAsync_ReturnsPagedResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new SystemConfigService(context, _mockLogger.Object);

        await service.CreateAlertRuleAsync(new CreateAlertRuleRequest
        {
            RuleName = "Alert 1",
            RuleType = "TypeA",
            Enabled = true
        }, "admin");

        await service.CreateAlertRuleAsync(new CreateAlertRuleRequest
        {
            RuleName = "Alert 2",
            RuleType = "TypeB",
            Enabled = false
        }, "admin");

        var query = new AlertRuleQuery { PageIndex = 1, PageSize = 10 };

        // Act
        var result = await service.QueryAlertRulesAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task UpdateAlertRuleAsync_WithValidData_UpdatesRule()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new SystemConfigService(context, _mockLogger.Object);

        var created = await service.CreateAlertRuleAsync(new CreateAlertRuleRequest
        {
            RuleName = "Original Rule",
            RuleType = "TypeA",
            Enabled = true
        }, "admin");

        var updateRequest = new CreateAlertRuleRequest
        {
            RuleName = "Updated Rule",
            RuleType = "TypeB",
            Enabled = false
        };

        // Act
        var result = await service.UpdateAlertRuleAsync(created.RuleId, updateRequest, "admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Rule", result.RuleName);
        Assert.Equal("TypeB", result.RuleType);
        Assert.False(result.Enabled);
    }

    [Fact]
    public async Task UpdateAlertRuleAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new SystemConfigService(context, _mockLogger.Object);
        var updateRequest = new CreateAlertRuleRequest
        {
            RuleName = "Test",
            RuleType = "Test"
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.UpdateAlertRuleAsync("NONEXISTENT", updateRequest, "admin"));
    }
}
