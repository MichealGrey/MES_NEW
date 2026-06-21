using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MES.Infrastructure.Persistence;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics.Tests;

[Trait("Category", "UnitTest")]
[Trait("Phase", "5")]
public class NpiServiceTests
{
    private readonly Mock<ILogger<NpiService>> _mockLogger = new();

    private MesDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new MesDbContext(options);
    }

    [Fact]
    public async Task CreateProjectAsync_WithValidData_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new NpiService(context, _mockLogger.Object);
        var request = new CreateNpiProjectRequest
        {
            ProjectName = "New Product Alpha",
            ProductId = "PROD-ALPHA-001",
            TargetDate = new DateOnly(2026, 12, 31),
            Description = "New high-efficiency product development"
        };

        // Act
        var result = await service.CreateProjectAsync(request, "pm001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product Alpha", result.ProjectName);
        Assert.Equal("PROD-ALPHA-001", result.ProductId);
        Assert.Equal("Active", result.Status);
        Assert.Equal("Initiation", result.CurrentStage);
        Assert.NotNull(result.ProjectId);
        Assert.StartsWith("NPI-", result.ProjectId);
    }

    [Fact]
    public async Task CreateProjectAsync_WithMinimalData_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new NpiService(context, _mockLogger.Object);
        var request = new CreateNpiProjectRequest
        {
            ProjectName = "Quick Project"
        };

        // Act
        var result = await service.CreateProjectAsync(request, "pm001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Quick Project", result.ProjectName);
        Assert.Equal("Active", result.Status);
    }

    [Fact]
    public async Task GetProjectAsync_WhenFound_ReturnsResponse()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new NpiService(context, _mockLogger.Object);

        var createResult = await service.CreateProjectAsync(new CreateNpiProjectRequest
        {
            ProjectName = "Test Project",
            ProductId = "PROD-001"
        }, "pm001");

        // Act
        var result = await service.GetProjectAsync(createResult.ProjectId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createResult.ProjectId, result.ProjectId);
        Assert.Equal("Test Project", result.ProjectName);
    }

    [Fact]
    public async Task GetProjectAsync_WhenNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new NpiService(context, _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetProjectAsync("NONEXISTENT"));
    }

    [Fact]
    public async Task QueryProjectsAsync_ReturnsPagedResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new NpiService(context, _mockLogger.Object);

        await service.CreateProjectAsync(new CreateNpiProjectRequest
        {
            ProjectName = "Project Alpha",
            ProductId = "PROD-A"
        }, "pm001");

        await service.CreateProjectAsync(new CreateNpiProjectRequest
        {
            ProjectName = "Project Beta",
            ProductId = "PROD-B"
        }, "pm002");

        var query = new NpiProjectQuery { PageIndex = 1, PageSize = 10 };

        // Act
        var result = await service.QueryProjectsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task QueryProjectsAsync_WithKeyword_ReturnsFilteredResult()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new NpiService(context, _mockLogger.Object);

        await service.CreateProjectAsync(new CreateNpiProjectRequest
        {
            ProjectName = "Alpha Project",
            ProductId = "PROD-A"
        }, "pm001");

        await service.CreateProjectAsync(new CreateNpiProjectRequest
        {
            ProjectName = "Beta Project",
            ProductId = "PROD-B"
        }, "pm002");

        var query = new NpiProjectQuery { PageIndex = 1, PageSize = 10, Keyword = "Alpha" };

        // Act
        var result = await service.QueryProjectsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Contains("Alpha", result.Items[0].ProjectName);
    }

    [Fact]
    public async Task ExecuteTrialRunAsync_WithValidData_CreatesStage()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new NpiService(context, _mockLogger.Object);

        var project = await service.CreateProjectAsync(new CreateNpiProjectRequest
        {
            ProjectName = "Trial Project",
            ProductId = "PROD-001"
        }, "pm001");

        var request = new TrialRunRequest
        {
            LotId = "LOT-TRIAL-001",
            SampleSize = 100,
            Remark = "First trial run"
        };

        // Act
        var result = await service.ExecuteTrialRunAsync(project.ProjectId, request, "operator1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Trial Run", result.StageName);
        Assert.Equal("InProgress", result.Status);
    }

    [Fact]
    public async Task TransferToMassProductionAsync_WithValidData_CompletesProject()
    {
        // Arrange
        var context = CreateDbContext();
        var service = new NpiService(context, _mockLogger.Object);

        var project = await service.CreateProjectAsync(new CreateNpiProjectRequest
        {
            ProjectName = "Transfer Project",
            ProductId = "PROD-001"
        }, "pm001");

        // Act
        var result = await service.TransferToMassProductionAsync(project.ProjectId, "pm001");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Completed", result.Status);
        Assert.Equal("MassProduction", result.CurrentStage);
    }
}
