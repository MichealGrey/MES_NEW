using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;

namespace MES.Modules.Execute.Services;

public interface IExecuteService
{
    Task<List<TrackInRecord>> GetAllTrackInRecordsAsync();
    Task<TrackInRecord?> GetTrackInRecordAsync(string recordId);
    Task SaveTrackInRecordAsync(TrackInRecord record);
    Task DeleteTrackInRecordAsync(string recordId);
    Task<List<DispatchInfo>> GetAllDispatchesAsync();
    Task<DispatchInfo?> GetDispatchAsync(string dispatchId);
    Task SaveDispatchAsync(DispatchInfo dispatch);
    Task UpdateDispatchStatusAsync(string dispatchId, string status);
    Task<List<WipInfo>> GetAllWipAsync();
    Task<List<AlarmInfo>> GetAllAlarmsAsync();
    Task<List<AlarmInfo>> GetActiveAlarmsAsync();
    Task AcknowledgeAlarmAsync(string alarmId);
    Task ClearAlarmAsync(string alarmId);
}

public class TrackInRecord
{
    public string RecordId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public DateTime TrackInTime { get; set; }
    public DateTime? TrackOutTime { get; set; }
    public string Status { get; set; } = "InProgress";
    public int QtyIn { get; set; }
    public int QtyOut { get; set; }
    public int QtyPass { get; set; }
    public int QtyFail { get; set; }
    public string? Remark { get; set; }
}

public class DispatchInfo
{
    public string DispatchId { get; set; } = string.Empty;
    public string LotId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string StepCode { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public int Qty { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Operator { get; set; }
    public int Priority { get; set; }
    public string? Remark { get; set; }
}

public class WipInfo
{
    public string LotId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string CurrentEquipment { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Qty { get; set; }
    public DateTime TrackInTime { get; set; }
    public double? ElapsedMinutes { get; set; }
    public string ProcessStage { get; set; } = string.Empty;
}

public class AlarmInfo
{
    public string AlarmId { get; set; } = string.Empty;
    public string EquipmentId { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Warning";
    public string Message { get; set; } = string.Empty;
    public DateTime RaisedAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? ClearedAt { get; set; }
    public string Status { get; set; } = "Active";
    public string? AcknowledgedBy { get; set; }
    public string? Remark { get; set; }
}

public class ExecuteService : IExecuteService
{
    private readonly IRepository<ProdLotStep> _lotStepRepo;
    private readonly IRepository<ProdLot> _lotRepo;

    public ExecuteService(
        IRepository<ProdLotStep> lotStepRepo,
        IRepository<ProdLot> lotRepo)
    {
        _lotStepRepo = lotStepRepo;
        _lotRepo = lotRepo;
    }

    public async Task<List<TrackInRecord>> GetAllTrackInRecordsAsync()
    {
        var entities = await _lotStepRepo.GetAllAsync();
        return entities.Select(MapToTrackInRecord).ToList();
    }

    public async Task<TrackInRecord?> GetTrackInRecordAsync(string recordId)
    {
        var entity = await _lotStepRepo.GetByIdAsync(recordId);
        return entity is null ? null : MapToTrackInRecord(entity);
    }

    public async Task SaveTrackInRecordAsync(TrackInRecord record)
    {
        var entity = MapToEntity(record);
        var existing = await _lotStepRepo.GetByIdAsync(record.RecordId);
        if (existing is null)
            await _lotStepRepo.AddAsync(entity);
        else
            await _lotStepRepo.UpdateAsync(entity);
    }

    public async Task DeleteTrackInRecordAsync(string recordId)
    {
        var entity = await _lotStepRepo.GetByIdAsync(recordId);
        if (entity is not null)
            await _lotStepRepo.DeleteAsync(entity);
    }

    public async Task<List<DispatchInfo>> GetAllDispatchesAsync()
    {
        var entities = await _lotStepRepo.GetAllAsync();
        return entities.Where(e => e.Status == "Pending" || e.Status == "InProgress")
                       .Select(MapToDispatchInfo).ToList();
    }

    public async Task<DispatchInfo?> GetDispatchAsync(string dispatchId)
    {
        var entity = await _lotStepRepo.GetByIdAsync(dispatchId);
        return entity is null ? null : MapToDispatchInfo(entity);
    }

    public async Task SaveDispatchAsync(DispatchInfo dispatch)
    {
        var entity = MapToDispatchEntity(dispatch);
        var existing = await _lotStepRepo.GetByIdAsync(dispatch.DispatchId);
        if (existing is null)
            await _lotStepRepo.AddAsync(entity);
        else
            await _lotStepRepo.UpdateAsync(entity);
    }

    public async Task UpdateDispatchStatusAsync(string dispatchId, string status)
    {
        var entity = await _lotStepRepo.GetByIdAsync(dispatchId);
        if (entity is null) return;

        entity.Status = status;
        if (status == "InProgress")
            entity.TrackInTime = DateTime.UtcNow;
        else if (status == "Completed")
            entity.TrackOutTime = DateTime.UtcNow;

        await _lotStepRepo.UpdateAsync(entity);
    }

    public async Task<List<WipInfo>> GetAllWipAsync()
    {
        var lots = await _lotRepo.GetWhereAsync(l => l.Status == "InProgress");
        var lotSteps = await _lotStepRepo.GetAllAsync();

        return lots.Select(l =>
        {
            var currentStep = lotSteps.FirstOrDefault(s => s.LotId == l.LotId && s.Status == "InProgress");
            return new WipInfo
            {
                LotId = l.LotId,
                ProductId = l.ProductId,
                ProductName = l.ProductName,
                CurrentStep = l.CurrentStepCode ?? string.Empty,
                CurrentEquipment = currentStep?.TrackInEquipment ?? string.Empty,
                Status = l.Status,
                Qty = l.UnitCount,
                TrackInTime = currentStep?.TrackInTime ?? l.CreatedAt,
                ElapsedMinutes = currentStep?.TrackInTime.HasValue == true
                    ? (DateTime.UtcNow - currentStep.TrackInTime.Value).TotalMinutes
                    : null,
                ProcessStage = l.ProcessStage ?? string.Empty,
            };
        }).ToList();
    }

    public async Task<List<AlarmInfo>> GetAllAlarmsAsync()
    {
        return new List<AlarmInfo>();
    }

    public async Task<List<AlarmInfo>> GetActiveAlarmsAsync()
    {
        return new List<AlarmInfo>();
    }

    public async Task AcknowledgeAlarmAsync(string alarmId)
    {
        await Task.CompletedTask;
    }

    public async Task ClearAlarmAsync(string alarmId)
    {
        await Task.CompletedTask;
    }

    private static TrackInRecord MapToTrackInRecord(ProdLotStep e) => new()
    {
        RecordId = e.RecordId,
        LotId = e.LotId,
        EquipmentId = e.TrackInEquipment ?? string.Empty,
        StepCode = e.StepCode,
        StepName = e.StepName ?? string.Empty,
        Operator = e.TrackInOperator ?? string.Empty,
        TrackInTime = e.TrackInTime ?? e.CreatedAt,
        TrackOutTime = e.TrackOutTime,
        Status = e.Status ?? "Waiting",
        QtyIn = e.InputQty,
        QtyOut = e.PassQty + e.FailQty,
        QtyPass = e.PassQty,
        QtyFail = e.FailQty,
        Remark = e.Remark,
    };

    private static ProdLotStep MapToEntity(TrackInRecord m) => new()
    {
        RecordId = m.RecordId,
        LotId = m.LotId,
        StepCode = m.StepCode,
        StepName = m.StepName,
        TrackInEquipment = m.EquipmentId,
        TrackInOperator = m.Operator,
        TrackInTime = m.TrackInTime,
        TrackOutTime = m.TrackOutTime,
        Status = m.Status,
        InputQty = m.QtyIn,
        PassQty = m.QtyPass,
        FailQty = m.QtyFail,
        Remark = m.Remark,
    };

    private static DispatchInfo MapToDispatchInfo(ProdLotStep e) => new()
    {
        DispatchId = e.RecordId,
        LotId = e.LotId,
        EquipmentId = e.TrackInEquipment ?? string.Empty,
        StepCode = e.StepCode,
        Status = e.Status ?? "Waiting",
        Qty = e.InputQty,
        CreatedAt = e.CreatedAt,
        StartedAt = e.TrackInTime,
        CompletedAt = e.TrackOutTime,
        Operator = e.TrackInOperator ?? string.Empty,
        Priority = 0,
        Remark = e.Remark,
    };

    private static ProdLotStep MapToDispatchEntity(DispatchInfo m) => new()
    {
        RecordId = m.DispatchId,
        LotId = m.LotId,
        StepCode = m.StepCode,
        TrackInEquipment = m.EquipmentId,
        TrackInOperator = m.Operator,
        TrackInTime = m.StartedAt,
        TrackOutTime = m.CompletedAt,
        Status = m.Status,
        InputQty = m.Qty,
        Remark = m.Remark,
    };
}
