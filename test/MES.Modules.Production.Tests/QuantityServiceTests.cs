using System.Threading.Tasks;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Moq;
using Xunit;

namespace MES.Modules.Production.Tests;

public class QuantityServiceTests
{
    private QuantityService CreateService(Mock<IQuantityTransactionRepository>? qtyTxnRepo = null)
    {
        return new QuantityService(qtyTxnRepo?.Object ?? Mock.Of<IQuantityTransactionRepository>());
    }

    [Fact]
    public async Task ValidateTrackOutQuantityAsync_BalancedQuantities_ReturnsValid()
    {
        var service = CreateService();
        var request = new TrackOutRequest
        {
            LotId = "LOT-001",
            StepCode = "S1",
            InputQty = 100,
            PassQty = 90,
            FailQty = 5,
            ScrapQty = 3,
            ReworkQty = 2,
            HoldQty = 0,
            PendingQty = 0
        };

        var result = await service.ValidateTrackOutQuantityAsync(request);

        Assert.True(result.IsBalanced);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ValidateTrackOutQuantityAsync_UnbalancedQuantities_ReturnsError()
    {
        var service = CreateService();
        var request = new TrackOutRequest
        {
            LotId = "LOT-001",
            StepCode = "S1",
            InputQty = 100,
            PassQty = 50,
            FailQty = 0,
            ScrapQty = 0,
            ReworkQty = 0,
            HoldQty = 0,
            PendingQty = 0
        };

        var result = await service.ValidateTrackOutQuantityAsync(request);

        Assert.False(result.IsBalanced);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public async Task ValidateTrackOutQuantityAsync_ZeroInputQty_ReturnsError()
    {
        var service = CreateService();
        var request = new TrackOutRequest
        {
            LotId = "LOT-001",
            StepCode = "S1",
            InputQty = 0,
            PassQty = 0,
            FailQty = 0,
            ScrapQty = 0,
            ReworkQty = 0,
            HoldQty = 0,
            PendingQty = 0
        };

        var result = await service.ValidateTrackOutQuantityAsync(request);

        Assert.False(result.IsBalanced);
        Assert.Contains(result.Errors, e => e.Contains("投入数量"));
    }

    [Fact]
    public async Task ValidateTrackOutQuantityAsync_NegativeQuantity_ReturnsError()
    {
        var service = CreateService();
        var request = new TrackOutRequest
        {
            LotId = "LOT-001",
            StepCode = "S1",
            InputQty = 100,
            PassQty = -1,
            FailQty = 0,
            ScrapQty = 0,
            ReworkQty = 0,
            HoldQty = 0,
            PendingQty = 0
        };

        var result = await service.ValidateTrackOutQuantityAsync(request);

        Assert.False(result.IsBalanced);
        Assert.Contains(result.Errors, e => e.Contains("负数"));
    }

    [Fact]
    public async Task ValidateTrackOutQuantityAsync_LowYield_ReturnsWarning()
    {
        var service = CreateService();
        var request = new TrackOutRequest
        {
            LotId = "LOT-001",
            StepCode = "S1",
            InputQty = 100,
            PassQty = 80,
            FailQty = 10,
            ScrapQty = 5,
            ReworkQty = 5,
            HoldQty = 0,
            PendingQty = 0
        };

        var result = await service.ValidateTrackOutQuantityAsync(request);

        Assert.True(result.IsBalanced);
        Assert.NotEmpty(result.Warnings);
        Assert.Contains(result.Warnings, w => w.Contains("良率过低"));
    }

    [Fact]
    public async Task ValidateTrackOutQuantityAsync_ExactMatch_NoWarning()
    {
        var service = CreateService();
        var request = new TrackOutRequest
        {
            LotId = "LOT-001",
            StepCode = "S1",
            InputQty = 100,
            PassQty = 100,
            FailQty = 0,
            ScrapQty = 0,
            ReworkQty = 0,
            HoldQty = 0,
            PendingQty = 0
        };

        var result = await service.ValidateTrackOutQuantityAsync(request);

        Assert.True(result.IsBalanced);
        Assert.Empty(result.Warnings);
    }
}
