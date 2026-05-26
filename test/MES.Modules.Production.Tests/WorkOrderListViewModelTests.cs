using MES.Modules.Production.ViewModels;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Xunit;

namespace MES.Modules.Production.Tests;

public class WorkOrderListViewModelTests
{
    [Fact]
    public void ProgressPercent_CalculatedCorrectly()
    {
        var wo = new WorkOrderInfo { PlannedQty = 100, CompletedQty = 25 };
        Assert.Equal(25.0, wo.ProgressPercent);
    }

    [Fact]
    public void ProgressPercent_ZeroPlannedQty_ReturnsZero()
    {
        var wo = new WorkOrderInfo { PlannedQty = 0, CompletedQty = 0 };
        Assert.Equal(0, wo.ProgressPercent);
    }

    [Fact]
    public void LotInfo_HoldTypeDisplay()
    {
        var lot = new LotInfo { HoldCategory = HoldType.Quality };
        Assert.Equal("QA", lot.HoldTypeDisplay);
    }

    [Fact]
    public void LotInfo_HoldTypeDisplay_Engineering()
    {
        var lot = new LotInfo { HoldCategory = HoldType.Engineering };
        Assert.Equal("ENG", lot.HoldTypeDisplay);
    }

    [Fact]
    public void LotInfo_IsHoldOverdue_Over24Hours()
    {
        var lot = new LotInfo { HoldTime = DateTime.Now.AddHours(-25) };
        Assert.True(lot.IsHoldOverdue);
    }

    [Fact]
    public void LotInfo_IsHoldOverdue_Under24Hours()
    {
        var lot = new LotInfo { HoldTime = DateTime.Now.AddHours(-1) };
        Assert.False(lot.IsHoldOverdue);
    }
}
