using MES.Modules.Alarm.Models;

namespace MES.Modules.Alarm.Services;

/// <summary>
/// Alarm service interface for managing alarms, alarm rules, and alarm statistics.
/// </summary>
public interface IAlarmService
{
    Task<List<ActiveAlarmItem>> GetActiveAlarmsAsync();
    Task<List<AlarmHistoryItem>> GetAlarmHistoryAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<AlarmRuleItem>> GetAlarmRulesAsync();
    Task<AlarmStatisticsSummary> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);

    Task<bool> AcknowledgeAlarmAsync(string alarmId, string ackBy);
    Task<bool> CloseAlarmAsync(string alarmId, string closeBy);
    Task<bool> SaveAlarmRuleAsync(AlarmRuleItem rule);
    Task<bool> DeleteAlarmRuleAsync(string alarmCode);
    Task<bool> ToggleRuleStatusAsync(string alarmCode, bool enabled);
}

public class AlarmStatisticsSummary
{
    public int TotalAlarms { get; set; }
    public int CriticalCount { get; set; }
    public int MajorCount { get; set; }
    public int MinorCount { get; set; }
    public int ActiveCount { get; set; }
    public int ClosedCount { get; set; }
    public double AvgAckTimeMinutes { get; set; }
    public List<AlarmStatisticsItem> DailyTrend { get; set; } = new();
}

public class InMemoryAlarmService : IAlarmService
{
    private readonly List<ActiveAlarmItem> _activeAlarms = new();
    private readonly List<AlarmHistoryItem> _history = new();
    private readonly List<AlarmRuleItem> _rules = new();

    public InMemoryAlarmService()
    {
        SeedData();
    }

    private void SeedData()
    {
        _activeAlarms.AddRange(new[]
        {
            new ActiveAlarmItem { AlarmId = "ALM-001", AlarmCode = "E301", EquipmentId = "EQ-003", Severity = "Critical", Text = "腔体温度超上限 (>250C)", AlarmTime = DateTime.Now.AddMinutes(-5), Duration = "00:05:12", Status = "Active" },
            new ActiveAlarmItem { AlarmId = "ALM-002", AlarmCode = "E102", EquipmentId = "EQ-008", Severity = "Major", Text = "RF功率偏差超过10%", AlarmTime = DateTime.Now.AddMinutes(-15), Duration = "00:15:30", Status = "Active" },
            new ActiveAlarmItem { AlarmId = "ALM-003", AlarmCode = "E205", EquipmentId = "EQ-012", Severity = "Minor", Text = "冷却水流量偏低", AlarmTime = DateTime.Now.AddHours(-1), Duration = "01:02:45", Status = "Active" },
            new ActiveAlarmItem { AlarmId = "ALM-004", AlarmCode = "E401", EquipmentId = "EQ-001", Severity = "Critical", Text = "对准标记偏差超限", AlarmTime = DateTime.Now.AddMinutes(-2), Duration = "00:02:18", Status = "Active" },
            new ActiveAlarmItem { AlarmId = "ALM-005", AlarmCode = "E110", EquipmentId = "EQ-015", Severity = "Major", Text = "真空度超出规格", AlarmTime = DateTime.Now.AddMinutes(-30), Duration = "00:30:55", Status = "Active" },
        });

        _history.AddRange(new[]
        {
            new AlarmHistoryItem { AlarmId = "ALM-101", AlarmCode = "E301", EquipmentId = "EQ-003", Severity = "Critical", Text = "腔体温度超上限", AlarmTime = DateTime.Now.AddHours(-5), AckTime = DateTime.Now.AddHours(-5).AddMinutes(3), AckBy = "Zhang Wei", CloseTime = DateTime.Now.AddHours(-4), Status = "Closed" },
            new AlarmHistoryItem { AlarmId = "ALM-102", AlarmCode = "E102", EquipmentId = "EQ-008", Severity = "Major", Text = "RF功率偏差", AlarmTime = DateTime.Now.AddHours(-8), AckTime = DateTime.Now.AddHours(-8).AddMinutes(10), AckBy = "Li Ming", CloseTime = DateTime.Now.AddHours(-6), Status = "Closed" },
            new AlarmHistoryItem { AlarmId = "ALM-103", AlarmCode = "E205", EquipmentId = "EQ-012", Severity = "Minor", Text = "冷却水流量偏低", AlarmTime = DateTime.Now.AddHours(-12), AckTime = DateTime.Now.AddHours(-12).AddMinutes(15), AckBy = "Wang Fang", CloseTime = DateTime.Now.AddHours(-10), Status = "Closed" },
            new AlarmHistoryItem { AlarmId = "ALM-104", AlarmCode = "E401", EquipmentId = "EQ-001", Severity = "Critical", Text = "对准标记偏差超限", AlarmTime = DateTime.Now.AddDays(-1), AckTime = DateTime.Now.AddDays(-1).AddMinutes(5), AckBy = "Liu Jun", CloseTime = DateTime.Now.AddDays(-1).AddHours(2), Status = "Closed" },
        });

        _rules.AddRange(new[]
        {
            new AlarmRuleItem { AlarmCode = "E301", AlarmName = "温度超限", EquipmentType = "MoldingPress", Severity = "Critical", AutoHold = true, Description = "模压温度超过185C时触发", Enabled = true },
            new AlarmRuleItem { AlarmCode = "E102", AlarmName = "超声波功率异常", EquipmentType = "WireBonder", Severity = "Major", AutoHold = true, Description = "超声波功率偏差超过10%", Enabled = true },
            new AlarmRuleItem { AlarmCode = "E205", AlarmName = "冷却水流量低", EquipmentType = "DicingSaw", Severity = "Minor", AutoHold = false, Description = "冷却水流量低于5L/min", Enabled = true },
            new AlarmRuleItem { AlarmCode = "E401", AlarmName = "贴装偏差超限", EquipmentType = "DieBonder", Severity = "Critical", AutoHold = true, Description = "芯片贴装偏移超过50μm", Enabled = true },
            new AlarmRuleItem { AlarmCode = "E110", AlarmName = "真空度异常", EquipmentType = "MoldingPress", Severity = "Major", AutoHold = true, Description = "模压腔体真空度超出设定值", Enabled = false },
        });
    }

    public Task<List<ActiveAlarmItem>> GetActiveAlarmsAsync()
        => Task.FromResult(_activeAlarms.ToList());

    public Task<List<AlarmHistoryItem>> GetAlarmHistoryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _history.AsEnumerable();
        if (startDate.HasValue) query = query.Where(h => h.AlarmTime >= startDate.Value);
        if (endDate.HasValue) query = query.Where(h => h.AlarmTime <= endDate.Value);
        return Task.FromResult(query.ToList());
    }

    public Task<List<AlarmRuleItem>> GetAlarmRulesAsync()
        => Task.FromResult(_rules.ToList());

    public Task<AlarmStatisticsSummary> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var allItems = _history.Concat(_activeAlarms.Select(a => new AlarmHistoryItem
        {
            AlarmId = a.AlarmId,
            AlarmCode = a.AlarmCode,
            Severity = a.Severity,
            AlarmTime = a.AlarmTime,
            Status = "Active"
        }));

        return Task.FromResult(new AlarmStatisticsSummary
        {
            TotalAlarms = _history.Count + _activeAlarms.Count,
            CriticalCount = allItems.Count(a => a.Severity == "Critical"),
            MajorCount = allItems.Count(a => a.Severity == "Major"),
            MinorCount = allItems.Count(a => a.Severity == "Minor"),
            ActiveCount = _activeAlarms.Count,
            ClosedCount = _history.Count,
            AvgAckTimeMinutes = _history.Where(h => h.AckTime.HasValue && h.AckTime.Value > h.AlarmTime)
                .Average(h => (h.AckTime.Value - h.AlarmTime).TotalMinutes),
            DailyTrend = GenerateDailyTrend()
        });
    }

    public Task<bool> AcknowledgeAlarmAsync(string alarmId, string ackBy)
    {
        var alarm = _activeAlarms.FirstOrDefault(a => a.AlarmId == alarmId);
        if (alarm != null)
        {
            alarm.Status = "Acknowledged";
            var historyItem = new AlarmHistoryItem
            {
                AlarmId = alarm.AlarmId,
                AlarmCode = alarm.AlarmCode,
                EquipmentId = alarm.EquipmentId,
                Severity = alarm.Severity,
                Text = alarm.Text,
                AlarmTime = alarm.AlarmTime,
                AckTime = DateTime.Now,
                AckBy = ackBy,
                Status = "Acknowledged"
            };
            _history.Add(historyItem);
            _activeAlarms.Remove(alarm);
        }
        return Task.FromResult(true);
    }

    public Task<bool> CloseAlarmAsync(string alarmId, string closeBy)
    {
        var historyItem = _history.FirstOrDefault(h => h.AlarmId == alarmId && h.Status == "Acknowledged");
        if (historyItem != null)
        {
            historyItem.Status = "Closed";
            historyItem.CloseTime = DateTime.Now;
        }
        return Task.FromResult(true);
    }

    public Task<bool> SaveAlarmRuleAsync(AlarmRuleItem rule)
    {
        var existing = _rules.FirstOrDefault(r => r.AlarmCode == rule.AlarmCode);
        if (existing != null)
        {
            existing.AlarmName = rule.AlarmName;
            existing.EquipmentType = rule.EquipmentType;
            existing.Severity = rule.Severity;
            existing.AutoHold = rule.AutoHold;
            existing.Description = rule.Description;
            existing.Enabled = rule.Enabled;
        }
        else
        {
            _rules.Add(rule);
        }
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAlarmRuleAsync(string alarmCode)
    {
        var rule = _rules.FirstOrDefault(r => r.AlarmCode == alarmCode);
        if (rule != null)
        {
            _rules.Remove(rule);
        }
        return Task.FromResult(true);
    }

    public Task<bool> ToggleRuleStatusAsync(string alarmCode, bool enabled)
    {
        var rule = _rules.FirstOrDefault(r => r.AlarmCode == alarmCode);
        if (rule != null)
        {
            rule.Enabled = enabled;
        }
        return Task.FromResult(true);
    }

    private List<AlarmStatisticsItem> GenerateDailyTrend()
    {
        var trend = new List<AlarmStatisticsItem>();
        var baseDate = DateTime.Today;
        for (int i = 6; i >= 0; i--)
        {
            var date = baseDate.AddDays(-i);
            var random = new Random(i);
            var critical = random.Next(0, 4);
            var major = random.Next(2, 8);
            var minor = random.Next(5, 16);
            trend.Add(new AlarmStatisticsItem
            {
                Date = date,
                CriticalCount = critical,
                MajorCount = major,
                MinorCount = minor,
                TotalCount = critical + major + minor
            });
        }
        return trend;
    }
}
