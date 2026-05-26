using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface ISignatureService
{
    Task<SignatureRecord> RequestSignatureAsync(string entityType, string entityId,
        string level, string signerId, string signerName, string signerRole,
        string reason, string? comment = null);
    Task<bool> VerifySignatureAsync(string entityType, string entityId, string requiredLevel);
    Task<List<SignatureRecord>> GetSignaturesAsync(string entityType, string entityId);
}
