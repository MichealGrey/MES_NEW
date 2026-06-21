using MES.Contracts.Common;
using MES.Contracts.Phase5;

namespace MES.Services.Analytics;

public interface ISystemConfigService
{
    Task<PagedResult<SystemConfigResponse>> GetConfigsAsync(SystemConfigQuery query);
    Task<SystemConfigResponse?> GetConfigByKeyAsync(string configKey);
    Task<SystemConfigResponse> UpdateConfigAsync(UpdateSystemConfigRequest request, string operatorId);
    Task<AlertRuleResponse> CreateAlertRuleAsync(CreateAlertRuleRequest request, string operatorId);
    Task<PagedResult<AlertRuleResponse>> QueryAlertRulesAsync(AlertRuleQuery query);
    Task<AlertRuleResponse> UpdateAlertRuleAsync(string ruleId, CreateAlertRuleRequest request, string operatorId);
}
