using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence;
using MES.Contracts.Phase3;

namespace MES.Services.ProcessControl.Tests;

[Trait("Category", "UnitTest")]
[Trait("Phase", "3")]
public class QualificationServiceTests
{
    private readonly Mock<ILogger<QualificationService>> _mockLogger = new();

    private MesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MesDbContext(options);
    }

    [Fact]
    public async Task CreateQualificationAsync_WithValidData_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new QualificationService(context, _mockLogger.Object);
        var request = new CreateOperatorQualificationRequest
        {
            OperatorId = "OP-001",
            OperatorName = "Test Operator",
            Department = "Production",
            Position = "Wire Bond Operator",
            ProcessCode = "WIRE_BOND",
            ProcessName = "Wire Bonding",
            QualificationLevel = "Senior",
            CertificationType = "Internal",
            IssueDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddYears(1),
            IssuedBy = "QA Manager",
            CertificationNo = "CERT-2024-001"
        };

        // Act
        var result = await service.CreateQualificationAsync(request, "admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("OP-001", result.OperatorId);
        Assert.Equal("WIRE_BOND", result.ProcessCode);
        Assert.Equal("Active", result.Status);
        Assert.NotNull(result.QualificationId);
        Assert.StartsWith("QUAL-", result.QualificationId);
    }

    [Fact]
    public async Task CreateQualificationAsync_WithExpiredDate_ReturnsExpiredStatus()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new QualificationService(context, _mockLogger.Object);
        var request = new CreateOperatorQualificationRequest
        {
            OperatorId = "OP-002",
            OperatorName = "Expired Operator",
            ProcessCode = "CURE",
            ProcessName = "Curing",
            QualificationLevel = "Junior",
            CertificationType = "External",
            IssueDate = DateTime.UtcNow.AddYears(-2),
            ExpiryDate = DateTime.UtcNow.AddMonths(-1)
        };

        // Act
        var result = await service.CreateQualificationAsync(request, "admin");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Expired", result.Status);
    }

    [Fact]
    public async Task GetQualificationAsync_WhenFound_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new QualificationService(context, _mockLogger.Object);

        var createResult = await service.CreateQualificationAsync(new CreateOperatorQualificationRequest
        {
            OperatorId = "OP-003",
            OperatorName = "Found Operator",
            ProcessCode = "WIRE_BOND",
            ProcessName = "Wire Bonding",
            QualificationLevel = "Senior"
        }, "admin");

        // Act
        var result = await service.GetQualificationAsync(createResult.QualificationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createResult.QualificationId, result.QualificationId);
        Assert.Equal("OP-003", result.OperatorId);
    }

    [Fact]
    public async Task GetQualificationAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new QualificationService(context, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetQualificationAsync("NONEXISTENT"));
    }

    [Fact]
    public async Task QueryQualificationsAsync_ReturnsPagedResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new QualificationService(context, _mockLogger.Object);

        await service.CreateQualificationAsync(new CreateOperatorQualificationRequest
        {
            OperatorId = "OP-001",
            OperatorName = "Operator A",
            ProcessCode = "WIRE_BOND",
            ProcessName = "Wire Bonding",
            QualificationLevel = "Senior"
        }, "admin");

        await service.CreateQualificationAsync(new CreateOperatorQualificationRequest
        {
            OperatorId = "OP-002",
            OperatorName = "Operator B",
            ProcessCode = "CURE",
            ProcessName = "Curing",
            QualificationLevel = "Junior"
        }, "admin");

        var query = new OperatorQualificationQuery { PageIndex = 1, PageSize = 10 };

        // Act
        var result = await service.QueryQualificationsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task CheckQualificationAsync_WithValidQualification_ReturnsQualified()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new QualificationService(context, _mockLogger.Object);

        // Create an active, non-expired qualification
        await service.CreateQualificationAsync(new CreateOperatorQualificationRequest
        {
            OperatorId = "OP-100",
            OperatorName = "Qualified Operator",
            ProcessCode = "WIRE_BOND",
            ProcessName = "Wire Bonding",
            QualificationLevel = "Senior",
            IssueDate = DateTime.UtcNow.AddMonths(-6),
            ExpiryDate = DateTime.UtcNow.AddMonths(6)
        }, "admin");

        var checkRequest = new OperatorQualificationCheckRequest
        {
            OperatorId = "OP-100",
            ProcessCode = "WIRE_BOND",
            StepCode = "WIRE_BOND",
            StepSeq = 1,
            CheckType = "TrackIn"
        };

        // Act
        var result = await service.CheckQualificationAsync(checkRequest, "system");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsQualified);
        Assert.Equal("Pass", result.CheckResult);
        Assert.Equal("Allow", result.Action);
    }

    [Fact]
    public async Task CheckQualificationAsync_WithExpiredQualification_ReturnsNotQualified()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new QualificationService(context, _mockLogger.Object);

        // Create an expired qualification
        await service.CreateQualificationAsync(new CreateOperatorQualificationRequest
        {
            OperatorId = "OP-200",
            OperatorName = "Expired Operator",
            ProcessCode = "CURE",
            ProcessName = "Curing",
            QualificationLevel = "Junior",
            IssueDate = DateTime.UtcNow.AddYears(-2),
            ExpiryDate = DateTime.UtcNow.AddMonths(-1)
        }, "admin");

        var checkRequest = new OperatorQualificationCheckRequest
        {
            OperatorId = "OP-200",
            ProcessCode = "CURE",
            StepCode = "CURE",
            StepSeq = 1,
            CheckType = "TrackIn"
        };

        // Act
        var result = await service.CheckQualificationAsync(checkRequest, "system");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsQualified);
        Assert.Equal("Fail", result.CheckResult);
        Assert.Equal("Reject", result.Action);
        Assert.Contains("过期", result.FailReason);
    }

    [Fact]
    public async Task CheckQualificationAsync_WithNoQualification_ReturnsNotQualified()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new QualificationService(context, _mockLogger.Object);

        var checkRequest = new OperatorQualificationCheckRequest
        {
            OperatorId = "OP-999",
            ProcessCode = "WIRE_BOND",
            StepCode = "WIRE_BOND",
            StepSeq = 1,
            CheckType = "TrackIn"
        };

        // Act
        var result = await service.CheckQualificationAsync(checkRequest, "system");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsQualified);
        Assert.Equal("Fail", result.CheckResult);
        Assert.Equal("Reject", result.Action);
        Assert.Contains("未找到", result.FailReason);
    }
}
