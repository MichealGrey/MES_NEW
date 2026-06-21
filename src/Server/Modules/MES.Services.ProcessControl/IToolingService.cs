using MES.Contracts.Common;
using MES.Contracts.Phase3;

namespace MES.Services.ProcessControl;

public interface IToolingService
{
    Task<ToolingResponse> CreateToolingAsync(CreateToolingRequest request, string operatorId);
    Task<ToolingResponse> GetToolingAsync(string toolingId);
    Task<PagedResult<ToolingResponse>> QueryToolingsAsync(ToolingQuery query);
    Task<ToolingUsageLogResponse> LogUsageAsync(ToolingUsageLogRequest request, string operatorId);
    Task<ToolingReplacementResponse> RecordReplacementAsync(ToolingReplacementRequest request, string operatorId);
    Task<PagedResult<ToolingUsageLogResponse>> GetToolingUsageLogsAsync(string toolingId, int pageIndex, int pageSize);
}
