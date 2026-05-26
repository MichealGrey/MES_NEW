using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IQualityGateway
{
    Task<bool> RequiresQualityGateAsync(string lotId, string stepCode);
    Task<bool> IsQualityGatePassedAsync(string lotId, string stepCode);
    Task<QualityGate> CreateQualityGateAsync(string lotId, string stepCode, int stepSeq, string gateType);
    Task<bool> PassQualityGateAsync(string gateId, string checkedBy, string checkedByName, string? comment = null);
}
