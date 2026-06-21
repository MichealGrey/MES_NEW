using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics.Tests;

[Trait("Category", "UnitTest")]
[Trait("Phase", "5")]
public class AuditServiceTests
{
    private readonly Mock<ILogger<AuditService>> _mockLogger = new();

    private MesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MesDbContext(options);
    }

    [Fact]
    public async Task QueryAuditTrailsAsync_ReturnsTrails()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new AuditService(context, _mockLogger.Object);

        var query = new DataCorrectionQuery { PageIndex = 1, PageSize = 20 };

        // Act
        var result = await service.QueryCorrectionsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task QueryAuditTrailsAsync_WithData_ReturnsPopulatedTrails()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new AuditService(context, _mockLogger.Object);

        await service.CreateCorrectionAsync(new CreateDataCorrectionRequest
        {
            TableName = "prod_lot",
            RecordId = "LOT-001",
            FieldName = "status",
            OldValue = "InProgress",
            NewValue = "Completed",
            Reason = "Manual correction"
        }, "admin");

        var query = new DataCorrectionQuery { PageIndex = 1, PageSize = 20 };

        // Act
        var result = await service.QueryCorrectionsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("prod_lot", result.Items[0].TableName);
        Assert.Equal("LOT-001", result.Items[0].RecordId);
    }

    [Fact]
    public async Task QueryAuditTrailsAsync_FilterByTableName_ReturnsFilteredTrails()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new AuditService(context, _mockLogger.Object);

        await service.CreateCorrectionAsync(new CreateDataCorrectionRequest
        {
            TableName = "prod_lot",
            RecordId = "LOT-001",
            FieldName = "status",
            OldValue = "InProgress",
            NewValue = "Completed",
            Reason = "Correction 1"
        }, "admin");

        await service.CreateCorrectionAsync(new CreateDataCorrectionRequest
        {
            TableName = "prod_work_order",
            RecordId = "WO-001",
            FieldName = "quantity",
            OldValue = "100",
            NewValue = "150",
            Reason = "Correction 2"
        }, "admin");

        var query = new DataCorrectionQuery { PageIndex = 1, PageSize = 20, TableName = "prod_lot" };

        // Act
        var result = await service.QueryCorrectionsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("prod_lot", result.Items[0].TableName);
    }

    [Fact]
    public async Task VerifyAuditIntegrityAsync_ReturnsIntegrityStatus()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new AuditService(context, _mockLogger.Object);

        // Act
        var result = await service.VerifyAuditIntegrityAsync();

        // Assert
        Assert.True(result); // Empty corrections = integrity passed
    }

    [Fact]
    public async Task VerifyAuditIntegrityAsync_WithValidCorrections_ReturnsTrue()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new AuditService(context, _mockLogger.Object);

        await service.CreateCorrectionAsync(new CreateDataCorrectionRequest
        {
            TableName = "prod_lot",
            RecordId = "LOT-001",
            FieldName = "status",
            OldValue = "A",
            NewValue = "B",
            Reason = "Valid correction"
        }, "admin");

        // Act
        var result = await service.VerifyAuditIntegrityAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CreateCorrectionAsync_WithValidData_CreatesCorrection()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new AuditService(context, _mockLogger.Object);
        var request = new CreateDataCorrectionRequest
        {
            TableName = "prod_lot",
            RecordId = "LOT-001",
            FieldName = "status",
            OldValue = "InProgress",
            NewValue = "Completed",
            Reason = "Manual correction",
            ApprovedBy = "manager1"
        };

        // Act
        var result = await service.CreateCorrectionAsync(request, "admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("prod_lot", result.TableName);
        Assert.Equal("LOT-001", result.RecordId);
        Assert.Equal("status", result.FieldName);
        Assert.Equal("InProgress", result.OldValue);
        Assert.Equal("Completed", result.NewValue);
        Assert.Equal("admin", result.CorrectedBy);
        Assert.NotNull(result.CorrectionId);
        Assert.StartsWith("CORR-", result.CorrectionId);
    }

    [Fact]
    public async Task HashCheckAsync_ReturnsHashes()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new AuditService(context, _mockLogger.Object);

        await service.CreateCorrectionAsync(new CreateDataCorrectionRequest
        {
            TableName = "test_table",
            RecordId = "REC-001",
            FieldName = "field1",
            OldValue = "old",
            NewValue = "new",
            Reason = "test"
        }, "admin");

        // Act
        var result = await service.HashCheckAsync("test_table");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task HashCheckAsync_WithNoData_ReturnsEmpty()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new AuditService(context, _mockLogger.Object);

        // Act
        var result = await service.HashCheckAsync("nonexistent_table");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
