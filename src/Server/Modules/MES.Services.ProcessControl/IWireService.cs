using MES.Contracts.Common;
using MES.Contracts.Phase3;

namespace MES.Services.ProcessControl;

public interface IWireService
{
    Task<WireMaterialSwitchResponse> RecordWireSwitchAsync(WireMaterialSwitchRequest request, string operatorId);
    Task<long> RecordWireConsumptionAsync(string lotId, string stepCode, int stepSeq, string equipmentId, string wireMaterialId, string wireMaterialName, decimal consumedLength, string lengthUnit, int? bondCount, string operatorId);
    Task<PagedResult<WireConsumptionResponse>> QueryWireConsumptionsAsync(WireConsumptionQuery query);
}
