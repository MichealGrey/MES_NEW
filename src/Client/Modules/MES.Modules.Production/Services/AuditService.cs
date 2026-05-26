using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class AuditService : IAuditService
{
    private readonly IRepository<ProdAuditTrail> _auditRepo;

    public AuditService(IRepository<ProdAuditTrail> auditRepo)
    {
        _auditRepo = auditRepo;
    }

    public async Task WriteAsync(AuditTrail audit)
    {
        if (string.IsNullOrEmpty(audit.AuditId))
            audit.AuditId = Guid.NewGuid().ToString("N");

        if (audit.Timestamp == default)
            audit.Timestamp = DateTime.UtcNow;

        var entity = new ProdAuditTrail
        {
            AuditId = audit.AuditId,
            EntityType = audit.EntityType,
            EntityId = audit.EntityId,
            Action = audit.Action,
            OperatorId = audit.OperatorId,
            OperatorName = audit.OperatorName,
            Timestamp = audit.Timestamp,
            BeforeState = audit.BeforeState,
            AfterState = audit.AfterState,
            Reason = audit.Reason,
            Detail = audit.Detail,
        };

        await _auditRepo.AddAsync(entity);
    }

    public async Task<List<AuditTrail>> GetByEntityAsync(string entityType, string entityId)
    {
        var audits = await _auditRepo.GetWhereAsync(a => a.EntityType == entityType && a.EntityId == entityId);
        return audits
            .Select(MapToModel)
            .OrderByDescending(a => a.Timestamp)
            .ToList();
    }

    private static AuditTrail MapToModel(ProdAuditTrail entity) => new()
    {
        AuditId = entity.AuditId,
        EntityType = entity.EntityType,
        EntityId = entity.EntityId,
        Action = entity.Action,
        OperatorId = entity.OperatorId,
        OperatorName = entity.OperatorName,
        Timestamp = entity.Timestamp,
        BeforeState = entity.BeforeState,
        AfterState = entity.AfterState,
        Reason = entity.Reason,
        Detail = entity.Detail,
    };
}
