using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class AlarmService : IAlarmService
{
    private readonly IRepository<ProdAlarm> _alarmRepo;
    private readonly IRepository<MasterAlarmRule> _ruleRepo;

    public AlarmService(IRepository<ProdAlarm> alarmRepo, IRepository<MasterAlarmRule> ruleRepo)
    {
        _alarmRepo = alarmRepo;
        _ruleRepo = ruleRepo;
    }

    public async Task<AlarmRecord> RaiseAlarmAsync(string alarmType, string message, string? lotId = null, string? equipmentId = null, string? stepCode = null, string? detail = null)
    {
        var rule = (await _ruleRepo.GetWhereAsync(r => r.AlarmType == alarmType)).FirstOrDefault();

        var msg = message;
        if (!string.IsNullOrEmpty(stepCode))
            msg = $"[{stepCode}] {message}";
        if (!string.IsNullOrEmpty(detail))
            msg = $"{msg} - {detail}";

        var alarm = new ProdAlarm
        {
            AlarmId = Guid.NewGuid().ToString("N"),
            RuleId = rule?.RuleId ?? string.Empty,
            AlarmType = alarmType,
            Severity = rule?.Severity ?? "Warning",
            Message = msg,
            LotId = lotId,
            EquipmentId = equipmentId,
            Status = "Active",
        };

        await _alarmRepo.AddAsync(alarm);
        return MapToModel(alarm);
    }

    public async Task AcknowledgeAlarmAsync(string alarmId, string acknowledgedBy)
    {
        var alarm = await _alarmRepo.GetByIdAsync(alarmId);
        if (alarm is null) return;

        alarm.Status = "Acknowledged";
        alarm.AcknowledgedBy = acknowledgedBy;
        alarm.AcknowledgedAt = DateTime.UtcNow;
        await _alarmRepo.UpdateAsync(alarm);
    }

    public async Task ResolveAlarmAsync(string alarmId, string resolvedBy)
    {
        var alarm = await _alarmRepo.GetByIdAsync(alarmId);
        if (alarm is null) return;

        alarm.ResolvedBy = resolvedBy;
        alarm.ResolvedAt = DateTime.UtcNow;
        await _alarmRepo.UpdateAsync(alarm);
    }

    public async Task<List<AlarmRecord>> GetActiveAlarmsAsync()
    {
        var alarms = await _alarmRepo.GetWhereAsync(a => !a.ResolvedAt.HasValue);
        return alarms.Select(MapToModel).ToList();
    }

    public async Task<List<AlarmRecord>> GetAlarmsByTypeAsync(string alarmType)
    {
        var alarms = await _alarmRepo.GetWhereAsync(a => a.AlarmType == alarmType);
        return alarms.Select(MapToModel).ToList();
    }

    public async Task CheckAndRaiseAsync(string alarmType, string lotId, string? equipmentId, string? stepCode, double? yieldValue = null, int? qtyValue = null, TimeSpan? duration = null)
    {
        var rules = await _ruleRepo.GetWhereAsync(r => r.AlarmType == alarmType && r.IsEnabled);
        var rule = rules.FirstOrDefault();
        if (rule is null) return;

        bool shouldRaise = false;
        string message = string.Empty;

        switch (alarmType)
        {
            case "LowYield":
                if (yieldValue.HasValue && rule.ThresholdYield.HasValue && yieldValue.Value < (double)rule.ThresholdYield.Value)
                {
                    shouldRaise = true;
                    message = $"良率 {yieldValue.Value:F1}% 低于阈值 {rule.ThresholdYield.Value:F1}%";
                }
                break;

            case "QueueTimeout":
            case "HoldTimeout":
                if (duration.HasValue && rule.ThresholdMinutes.HasValue && duration.Value.TotalMinutes > rule.ThresholdMinutes.Value)
                {
                    shouldRaise = true;
                    message = $"超时 {duration.Value.TotalMinutes:F0} 分钟，超过阈值 {rule.ThresholdMinutes.Value} 分钟";
                }
                break;

            case "MaterialShort":
                if (qtyValue.HasValue && rule.ThresholdQty.HasValue && qtyValue.Value < rule.ThresholdQty.Value)
                {
                    shouldRaise = true;
                    message = $"物料数量 {qtyValue.Value} 低于阈值 {rule.ThresholdQty.Value}";
                }
                break;
        }

        if (shouldRaise)
        {
            await RaiseAlarmAsync(alarmType, message, lotId, equipmentId, stepCode);
        }
    }

    private static AlarmRecord MapToModel(ProdAlarm entity) => new()
    {
        AlarmId = entity.AlarmId,
        AlarmType = entity.AlarmType,
        Severity = entity.Severity,
        Message = entity.Message,
        LotId = entity.LotId,
        EquipmentId = entity.EquipmentId,
        IsAcknowledged = entity.Status == "Acknowledged" || entity.Status == "Resolved",
        AcknowledgedBy = entity.AcknowledgedBy,
        AcknowledgedAt = entity.AcknowledgedAt,
        ResolvedBy = entity.ResolvedBy,
        ResolvedAt = entity.ResolvedAt,
        TriggeredAt = entity.CreatedAt,
    };
}
