using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Analytics;

public class SystemConfigService : ISystemConfigService
{
    private readonly MesDbContext _context;
    private readonly ILogger<SystemConfigService> _logger;

    public SystemConfigService(MesDbContext context, ILogger<SystemConfigService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<SystemConfigResponse>> GetConfigsAsync(SystemConfigQuery query)
    {
        var iqQuery = _context.SystemConfigs.AsQueryable();

        if (!string.IsNullOrEmpty(query.Category))
            iqQuery = iqQuery.Where(x => x.Category == query.Category);
        if (!string.IsNullOrEmpty(query.Keyword))
            iqQuery = iqQuery.Where(x => x.ConfigKey.Contains(query.Keyword) || x.Description != null && x.Description.Contains(query.Keyword));

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderBy(x => x.ConfigKey)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => MapToResponse(x))
            .ToListAsync();

        return new PagedResult<SystemConfigResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<SystemConfigResponse?> GetConfigByKeyAsync(string configKey)
    {
        var config = await _context.SystemConfigs
            .FirstOrDefaultAsync(x => x.ConfigKey == configKey);

        if (config == null)
            return null;

        return MapToResponse(config);
    }

    public async Task<SystemConfigResponse> UpdateConfigAsync(UpdateSystemConfigRequest request, string operatorId)
    {
        var config = await _context.SystemConfigs
            .FirstOrDefaultAsync(x => x.ConfigKey == request.ConfigKey);

        if (config == null)
            throw new KeyNotFoundException($"System config '{request.ConfigKey}' not found");

        config.ConfigValue = request.ConfigValue;
        config.ConfigType = request.ConfigType;
        config.Category = request.Category;
        config.Description = request.Description;
        config.UpdatedBy = operatorId;
        config.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated system config {ConfigKey}", request.ConfigKey);

        return MapToResponse(config);
    }

    public async Task<AlertRuleResponse> CreateAlertRuleAsync(CreateAlertRuleRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var ruleId = $"ARULE-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var rule = new AlertRule
        {
            RuleId = ruleId,
            RuleName = request.RuleName,
            RuleType = request.RuleType,
            ConditionExpression = request.ConditionExpression,
            ThresholdValue = request.ThresholdValue,
            Severity = request.Severity,
            NotificationChannels = request.NotificationChannels,
            Enabled = request.Enabled,
            CreatedBy = operatorId,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.AlertRules.Add(rule);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created alert rule {RuleId}: {RuleName}", ruleId, request.RuleName);

        return MapToResponse(rule);
    }

    public async Task<PagedResult<AlertRuleResponse>> QueryAlertRulesAsync(AlertRuleQuery query)
    {
        var iqQuery = _context.AlertRules.AsQueryable();

        if (!string.IsNullOrEmpty(query.RuleType))
            iqQuery = iqQuery.Where(x => x.RuleType == query.RuleType);
        if (query.Enabled.HasValue)
            iqQuery = iqQuery.Where(x => x.Enabled == query.Enabled.Value);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => MapToResponse(x))
            .ToListAsync();

        return new PagedResult<AlertRuleResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<AlertRuleResponse> UpdateAlertRuleAsync(string ruleId, CreateAlertRuleRequest request, string operatorId)
    {
        var rule = await _context.AlertRules
            .FirstOrDefaultAsync(x => x.RuleId == ruleId);

        if (rule == null)
            throw new KeyNotFoundException($"Alert rule '{ruleId}' not found");

        rule.RuleName = request.RuleName;
        rule.RuleType = request.RuleType;
        rule.ConditionExpression = request.ConditionExpression;
        rule.ThresholdValue = request.ThresholdValue;
        rule.Severity = request.Severity;
        rule.NotificationChannels = request.NotificationChannels;
        rule.Enabled = request.Enabled;
        rule.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated alert rule {RuleId}", ruleId);

        return MapToResponse(rule);
    }

    private static SystemConfigResponse MapToResponse(SystemConfig entity) => new()
    {
        ConfigId = entity.ConfigId,
        ConfigKey = entity.ConfigKey,
        ConfigValue = entity.ConfigValue,
        ConfigType = entity.ConfigType,
        Category = entity.Category,
        Description = entity.Description,
        IsPublic = entity.IsPublic,
        UpdatedBy = entity.UpdatedBy,
        UpdatedAt = entity.UpdatedAt
    };

    private static AlertRuleResponse MapToResponse(AlertRule entity) => new()
    {
        RuleId = entity.RuleId,
        RuleName = entity.RuleName,
        RuleType = entity.RuleType,
        ConditionExpression = entity.ConditionExpression,
        ThresholdValue = entity.ThresholdValue,
        Severity = entity.Severity,
        NotificationChannels = entity.NotificationChannels,
        Enabled = entity.Enabled,
        CreatedBy = entity.CreatedBy,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
