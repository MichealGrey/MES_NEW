using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class QualityGateway : IQualityGateway
{
    private readonly IRepository<QualityGateEntity> _gateRepo;

    public QualityGateway(IRepository<QualityGateEntity> gateRepo)
    {
        _gateRepo = gateRepo;
    }

    public async Task<bool> RequiresQualityGateAsync(string lotId, string stepCode)
    {
        var gates = await _gateRepo.GetWhereAsync(g => g.LotId == lotId && g.StepCode == stepCode);
        return gates.Count > 0;
    }

    public async Task<bool> IsQualityGatePassedAsync(string lotId, string stepCode)
    {
        var gates = await _gateRepo.GetWhereAsync(g => g.LotId == lotId && g.StepCode == stepCode);
        if (gates.Count == 0) return true;
        return gates.All(g => g.Status == "Passed");
    }

    public async Task<QualityGate> CreateQualityGateAsync(string lotId, string stepCode, int stepSeq, string gateType)
    {
        var entity = new QualityGateEntity
        {
            GateId = Guid.NewGuid().ToString("N"),
            LotId = lotId,
            StepCode = stepCode,
            StepSeq = stepSeq,
            GateType = gateType,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _gateRepo.AddAsync(entity);
        return MapToModel(entity);
    }

    public async Task<bool> PassQualityGateAsync(string gateId, string checkedBy, string checkedByName, string? comment = null)
    {
        var entity = await _gateRepo.GetByIdAsync(gateId);
        if (entity is null) return false;

        entity.Status = "Passed";
        entity.CheckedBy = checkedBy;
        entity.CheckedByName = checkedByName;
        entity.CheckedAt = DateTime.UtcNow;
        entity.Comment = comment;

        await _gateRepo.UpdateAsync(entity);
        return true;
    }

    private static QualityGate MapToModel(QualityGateEntity e) => new()
    {
        GateId = e.GateId,
        LotId = e.LotId,
        StepCode = e.StepCode,
        StepSeq = e.StepSeq,
        GateType = e.GateType,
        Status = e.Status,
        CheckedBy = e.CheckedBy,
        CheckedByName = e.CheckedByName,
        CheckedAt = e.CheckedAt,
        Comment = e.Comment,
        CreatedAt = e.CreatedAt,
        ExpireAt = e.ExpireAt,
    };
}
