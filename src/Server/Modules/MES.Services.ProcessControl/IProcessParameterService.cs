using MES.Contracts.Common;
using MES.Contracts.Phase3;

namespace MES.Services.ProcessControl;

public interface IProcessParameterService
{
    // 工序参数集
    Task<ProcessParameterSetResponse> CreateParameterSetAsync(CreateProcessParameterSetRequest request, string operatorId);
    Task<PagedResult<ProcessParameterSetResponse>> QueryParameterSetsAsync(ProcessParameterQuery query);
    Task<ProcessParameterSetResponse> GetParameterSetAsync(string parameterSetId);
    Task<List<ParameterItemResponse>> GetParameterItemsAsync(string parameterSetId);
    Task<ProcessParameterSetResponse> ActivateParameterSetAsync(string parameterSetId, ActivateParameterSetRequest request, string operatorId);
    Task<ParameterOverrideLogResponse> OverrideParameterAsync(string parameterSetId, OverrideParameterRequest request, string operatorId);
    Task<PagedResult<ParameterOverrideLogResponse>> GetOverrideLogsAsync(string parameterSetId, int pageIndex, int pageSize);

    // 固化温度曲线
    Task<CuringCurveResponse> CreateCuringCurveAsync(CreateCuringCurveRequest request, string operatorId);
    Task<PagedResult<CuringCurveResponse>> QueryCuringCurvesAsync(CuringCurveQuery query);
    Task<CuringCurveResponse> GetCuringCurveAsync(string curveId);
    Task<CuringCurveResponse> ActivateCuringCurveAsync(string curveId, string operatorId);
}
