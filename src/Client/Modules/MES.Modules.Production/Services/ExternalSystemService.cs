using System.Text.Json;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IExternalSystemService
{
    Task<ExternalSystemEvent> PublishEventAsync(string eventType, string targetSystem, object payload);
    Task<List<ExternalSystemEvent>> GetPendingEventsAsync(string? targetSystem = null);
    Task MarkEventSentAsync(string eventId);
    Task MarkEventFailedAsync(string eventId, string errorMessage);
    Task<ExternalSystemConfig> GetSystemConfigAsync(string systemId);
    Task<List<ExternalSystemConfig>> GetAllSystemConfigsAsync();
    Task SaveSystemConfigAsync(ExternalSystemConfig config);
}

public class ExternalSystemService : IExternalSystemService
{
    private readonly IRepository<ExtSystemEvent> _eventRepo;
    private readonly IRepository<ExtSystemConfig> _configRepo;

    public ExternalSystemService(
        IRepository<ExtSystemEvent> eventRepo,
        IRepository<ExtSystemConfig> configRepo)
    {
        _eventRepo = eventRepo;
        _configRepo = configRepo;
    }

    public async Task<ExternalSystemEvent> PublishEventAsync(string eventType, string targetSystem, object payload)
    {
        var evt = new ExternalSystemEvent
        {
            EventType = eventType,
            TargetSystem = targetSystem,
            SourceSystem = "MES",
            Payload = JsonSerializer.Serialize(payload)
        };

        var entity = new ExtSystemEvent
        {
            EventId = evt.EventId,
            EventType = evt.EventType,
            TargetSystem = evt.TargetSystem,
            SourceSystem = evt.SourceSystem,
            Payload = evt.Payload,
            Status = evt.Status,
            CreatedAt = evt.CreatedAt,
        };

        await _eventRepo.AddAsync(entity);
        return evt;
    }

    public async Task<List<ExternalSystemEvent>> GetPendingEventsAsync(string? targetSystem = null)
    {
        List<ExtSystemEvent> entities;
        if (!string.IsNullOrEmpty(targetSystem))
        {
            entities = await _eventRepo.GetWhereAsync(e => e.TargetSystem == targetSystem && e.Status == "Pending");
        }
        else
        {
            entities = await _eventRepo.GetWhereAsync(e => e.Status == "Pending");
        }

        return entities.Select(MapToEventModel).ToList();
    }

    public async Task MarkEventSentAsync(string eventId)
    {
        var entity = await _eventRepo.GetByIdAsync(eventId);
        if (entity is null) return;

        entity.Status = "Sent";
        entity.SentAt = DateTime.UtcNow;
        await _eventRepo.UpdateAsync(entity);
    }

    public async Task MarkEventFailedAsync(string eventId, string errorMessage)
    {
        var entity = await _eventRepo.GetByIdAsync(eventId);
        if (entity is null) return;

        entity.Status = "Failed";
        entity.ErrorMessage = errorMessage;
        entity.RetryCount++;
        await _eventRepo.UpdateAsync(entity);
    }

    public async Task<ExternalSystemConfig> GetSystemConfigAsync(string systemId)
    {
        var entity = await _configRepo.GetByIdAsync(systemId);
        if (entity is null) return new ExternalSystemConfig { SystemId = systemId };
        return MapToConfigModel(entity);
    }

    public async Task<List<ExternalSystemConfig>> GetAllSystemConfigsAsync()
    {
        var entities = await _configRepo.GetAllAsync();
        return entities.Select(MapToConfigModel).ToList();
    }

    public async Task SaveSystemConfigAsync(ExternalSystemConfig config)
    {
        var entity = MapToConfigEntity(config);
        var exists = await _configRepo.ExistsAsync(c => c.SystemId == config.SystemId);
        if (exists)
            await _configRepo.UpdateAsync(entity);
        else
            await _configRepo.AddAsync(entity);
    }

    private static ExternalSystemEvent MapToEventModel(ExtSystemEvent e) => new()
    {
        EventId = e.EventId,
        EventType = e.EventType,
        SourceSystem = e.SourceSystem,
        TargetSystem = e.TargetSystem,
        Payload = e.Payload ?? string.Empty,
        Status = e.Status,
        CreatedAt = e.CreatedAt,
        SentAt = e.SentAt,
        ErrorMessage = e.ErrorMessage,
        RetryCount = e.RetryCount,
    };

    private static ExternalSystemConfig MapToConfigModel(ExtSystemConfig e) => new()
    {
        SystemId = e.SystemId,
        SystemName = e.SystemName,
        SystemType = e.SystemType,
        Endpoint = e.Endpoint,
        AuthType = e.AuthType,
        AuthCredential = e.AuthCredential,
        IsEnabled = e.IsEnabled,
        TimeoutSeconds = e.TimeoutSeconds,
        MaxRetries = e.MaxRetries,
        SubscribedEvents = string.IsNullOrEmpty(e.SubscribedEvents)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(e.SubscribedEvents) ?? new List<string>(),
    };

    private static ExtSystemConfig MapToConfigEntity(ExternalSystemConfig m) => new()
    {
        SystemId = m.SystemId,
        SystemName = m.SystemName,
        SystemType = m.SystemType,
        Endpoint = m.Endpoint,
        AuthType = m.AuthType,
        AuthCredential = m.AuthCredential,
        IsEnabled = m.IsEnabled,
        TimeoutSeconds = m.TimeoutSeconds,
        MaxRetries = m.MaxRetries,
        SubscribedEvents = m.SubscribedEvents.Count > 0
            ? JsonSerializer.Serialize(m.SubscribedEvents)
            : null,
        CreatedAt = DateTime.UtcNow,
    };
}
