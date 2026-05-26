using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IAuditService
{
    Task WriteAsync(AuditTrail audit);
    Task<List<AuditTrail>> GetByEntityAsync(string entityType, string entityId);
}
