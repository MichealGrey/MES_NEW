using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Moq;
using Xunit;

namespace MES.Modules.Production.Tests;

public class AlarmServiceTests
{
    private AlarmService CreateService(
        Mock<IRepository<ProdAlarm>>? alarmRepo = null,
        Mock<IRepository<MasterAlarmRule>>? ruleRepo = null)
    {
        return new AlarmService(
            alarmRepo?.Object ?? Mock.Of<IRepository<ProdAlarm>>(),
            ruleRepo?.Object ?? Mock.Of<IRepository<MasterAlarmRule>>());
    }

    [Fact]
    public async Task RaiseAlarmAsync_CreatesAlarmRecord()
    {
        var alarmRepo = new Mock<IRepository<ProdAlarm>>();
        ProdAlarm? captured = null;
        alarmRepo.Setup(r => r.AddAsync(It.IsAny<ProdAlarm>()))
                 .Callback<ProdAlarm>(a => captured = a)
                 .ReturnsAsync((ProdAlarm a) => a);

        var ruleRepo = new Mock<IRepository<MasterAlarmRule>>();
        ruleRepo.Setup(r => r.GetWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<MasterAlarmRule, bool>>>()))
                .ReturnsAsync(new List<MasterAlarmRule>());

        var service = CreateService(alarmRepo: alarmRepo, ruleRepo: ruleRepo);
        var alarm = await service.RaiseAlarmAsync("LowYield", "良率过低", lotId: "LOT-001");

        Assert.NotNull(alarm);
        Assert.Equal("LowYield", alarm.AlarmType);
        Assert.Equal("LOT-001", alarm.LotId);
        Assert.False(alarm.IsAcknowledged);
        Assert.Null(alarm.ResolvedAt);
        Assert.NotNull(captured);
        Assert.Equal("Active", captured.Status);
    }

    [Fact]
    public async Task AcknowledgeAlarmAsync_SetsAcknowledged()
    {
        var alarmEntity = new ProdAlarm
        {
            AlarmId = "ALM-001",
            AlarmType = "LowYield",
            Severity = "Error",
            Message = "良率过低",
            Status = "Active",
            LotId = "LOT-001",
        };

        var alarmRepo = new Mock<IRepository<ProdAlarm>>();
        alarmRepo.Setup(r => r.GetByIdAsync("ALM-001")).ReturnsAsync(alarmEntity);
        alarmRepo.Setup(r => r.UpdateAsync(It.IsAny<ProdAlarm>())).Returns(Task.CompletedTask);

        var service = CreateService(alarmRepo: alarmRepo);
        await service.AcknowledgeAlarmAsync("ALM-001", "USER-001");

        Assert.Equal("Acknowledged", alarmEntity.Status);
        Assert.Equal("USER-001", alarmEntity.AcknowledgedBy);
    }

    [Fact]
    public async Task ResolveAlarmAsync_SetsResolved()
    {
        var alarmEntity = new ProdAlarm
        {
            AlarmId = "ALM-001",
            AlarmType = "LowYield",
            Severity = "Error",
            Message = "良率过低",
            Status = "Active",
            LotId = "LOT-001",
        };

        var alarmRepo = new Mock<IRepository<ProdAlarm>>();
        alarmRepo.Setup(r => r.GetByIdAsync("ALM-001")).ReturnsAsync(alarmEntity);
        alarmRepo.Setup(r => r.UpdateAsync(It.IsAny<ProdAlarm>())).Returns(Task.CompletedTask);

        var service = CreateService(alarmRepo: alarmRepo);
        await service.ResolveAlarmAsync("ALM-001", "USER-001");

        Assert.Equal("Resolved", alarmEntity.Status);
        Assert.Equal("USER-001", alarmEntity.ResolvedBy);
    }

    [Fact]
    public async Task GetActiveAlarmsAsync_ReturnsActiveAlarms()
    {
        var alarmRepo = new Mock<IRepository<ProdAlarm>>();
        alarmRepo.Setup(r => r.GetWhereAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ProdAlarm, bool>>>()))
                 .ReturnsAsync(new List<ProdAlarm>
                 {
                     new ProdAlarm { AlarmId = "ALM-001", AlarmType = "LowYield", Severity = "Error", Message = "良率过低 1", Status = "Active" },
                     new ProdAlarm { AlarmId = "ALM-002", AlarmType = "LowYield", Severity = "Error", Message = "良率过低 2", Status = "Active" },
                 });

        var service = CreateService(alarmRepo: alarmRepo);
        var alarms = await service.GetActiveAlarmsAsync();

        Assert.Equal(2, alarms.Count);
    }
}
