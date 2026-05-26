using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class SignatureService : ISignatureService
{
    private readonly IRepository<ProdSignature> _signatureRepo;

    public SignatureService(IRepository<ProdSignature> signatureRepo)
    {
        _signatureRepo = signatureRepo;
    }

    public async Task<SignatureRecord> RequestSignatureAsync(string entityType, string entityId,
        string level, string signerId, string signerName, string signerRole,
        string reason, string? comment = null)
    {
        var entity = new ProdSignature
        {
            EntityType = entityType,
            EntityId = entityId,
            Level = level,
            SignerId = signerId,
            SignerName = signerName,
            SignerRole = signerRole,
            Reason = reason,
            Comment = comment ?? string.Empty,
            SignTime = DateTime.UtcNow,
        };

        await _signatureRepo.AddAsync(entity);
        return MapToModel(entity);
    }

    public async Task<bool> VerifySignatureAsync(string entityType, string entityId, string requiredLevel)
    {
        var signatures = await _signatureRepo.GetWhereAsync(s => s.EntityType == entityType && s.EntityId == entityId);
        if (signatures.Count == 0) return false;

        return signatures.Any(s => IsLevelSufficient(s.Level, requiredLevel));
    }

    public async Task<List<SignatureRecord>> GetSignaturesAsync(string entityType, string entityId)
    {
        var signatures = await _signatureRepo.GetWhereAsync(s => s.EntityType == entityType && s.EntityId == entityId);
        return signatures.Select(MapToModel).ToList();
    }

    private static bool IsLevelSufficient(string actual, string required)
    {
        var levelOrder = new Dictionary<string, int>
        {
            { "Level0", 0 }, { "Level1", 1 }, { "Level2", 2 }, { "Level3", 3 }
        };
        if (!levelOrder.TryGetValue(actual, out var a)) a = 0;
        if (!levelOrder.TryGetValue(required, out var r)) r = 0;
        return a >= r;
    }

    private static SignatureRecord MapToModel(ProdSignature entity) => new()
    {
        SignatureId = entity.SignatureId,
        EntityType = entity.EntityType,
        EntityId = entity.EntityId,
        Level = entity.Level,
        SignerId = entity.SignerId,
        SignerName = entity.SignerName,
        SignerRole = entity.SignerRole,
        Reason = entity.Reason,
        Comment = entity.Comment,
        SignTime = entity.SignTime,
    };
}
