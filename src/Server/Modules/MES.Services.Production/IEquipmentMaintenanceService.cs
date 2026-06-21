using MES.Contracts.Common;
using MES.Contracts.Phase1;

namespace MES.Services.Production;

public interface IEquipmentMaintenanceService
{
    Task<EquipmentFaultResponse> ReportFaultAsync(ReportEquipmentFaultRequest request);
    Task<PagedResult<EquipmentFaultResponse>> GetFaultsAsync(EquipmentFaultQuery query);
    Task<bool> DispatchFaultAsync(DispatchFaultRequest request);
    Task<bool> CompleteRepairAsync(CompleteRepairRequest request);
    Task<PmPlanResponse> CreatePmPlanAsync(CreatePmPlanRequest request);
    Task<PagedResult<PmPlanResponse>> GetPmPlansAsync(PmPlanQuery query);
    Task<bool> ExecutePmAsync(ExecutePmRequest request);
    Task<List<MtbfMttrResponse>> GetMtbfMttrAsync(string equipmentId, string period);
}
