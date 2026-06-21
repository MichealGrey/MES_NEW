using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.ProcessControl;

public class ToolingService : IToolingService
{
    private readonly MesDbContext _context;
    private readonly ILogger<ToolingService> _logger;

    public ToolingService(MesDbContext context, ILogger<ToolingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ToolingResponse> CreateToolingAsync(CreateToolingRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var toolingId = $"TOOL-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var tooling = new ToolingRegistry
        {
            ToolingId = toolingId,
            ToolingCode = request.ToolingCode,
            ToolingName = request.ToolingName,
            ToolingType = request.ToolingType,
            Specification = request.Specification,
            Manufacturer = request.Manufacturer,
            Model = request.Model,
            Supplier = request.Supplier,
            PurchaseDate = request.PurchaseDate,
            ExpectedLifespan = request.ExpectedLifespan,
            LifespanUnit = request.LifespanUnit,
            Location = request.Location,
            AssociatedProcess = request.AssociatedProcess,
            CurrentUsage = 0,
            Status = "Available",
            Remark = request.Remark,
            CreatedBy = operatorId,
            CreatedAt = now,
            UpdatedAt = now,
            Deleted = false
        };

        _context.ToolingRegistries.Add(tooling);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created tooling {ToolingId} with code {ToolingCode}", toolingId, request.ToolingCode);

        return MapToResponse(tooling);
    }

    public async Task<ToolingResponse> GetToolingAsync(string toolingId)
    {
        var tooling = await _context.ToolingRegistries
            .FirstOrDefaultAsync(t => t.ToolingId == toolingId && !t.Deleted);

        if (tooling == null)
            throw new KeyNotFoundException($"Tooling {toolingId} not found");

        return MapToResponse(tooling);
    }

    public async Task<PagedResult<ToolingResponse>> QueryToolingsAsync(ToolingQuery query)
    {
        var iqQuery = _context.ToolingRegistries.Where(t => !t.Deleted).AsQueryable();

        if (!string.IsNullOrEmpty(query.ToolingType))
            iqQuery = iqQuery.Where(t => t.ToolingType == query.ToolingType);
        if (!string.IsNullOrEmpty(query.Status))
            iqQuery = iqQuery.Where(t => t.Status == query.Status);
        if (!string.IsNullOrEmpty(query.AssociatedEquipment))
            iqQuery = iqQuery.Where(t => t.AssociatedEquipment == query.AssociatedEquipment);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(t => t.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(t => MapToResponse(t))
            .ToListAsync();

        return new PagedResult<ToolingResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<ToolingUsageLogResponse> LogUsageAsync(ToolingUsageLogRequest request, string operatorId)
    {
        var tooling = await _context.ToolingRegistries
            .FirstOrDefaultAsync(t => t.ToolingId == request.ToolingId && !t.Deleted);

        if (tooling == null)
            throw new KeyNotFoundException($"Tooling {request.ToolingId} not found");

        var now = DateTime.UtcNow;
        var log = new ToolingUsageLog
        {
            ToolingId = request.ToolingId,
            ToolingCode = tooling.ToolingCode,
            ToolingName = tooling.ToolingName,
            LotId = request.LotId,
            StepCode = request.StepCode,
            StepSeq = request.StepSeq,
            EquipmentId = request.EquipmentId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            UsageDuration = request.UsageDuration,
            UsageDurationUnit = request.UsageDurationUnit,
            UsageCount = request.UsageCount,
            UsageStatus = request.UsageStatus,
            Remark = request.Remark,
            OperatorId = operatorId,
            OperatorName = operatorId,
            CreatedAt = now
        };

        _context.ToolingUsageLogs.Add(log);

        // Update CurrentUsage on ToolingRegistry
        var usageIncrement = request.UsageDuration != null ? (int)Math.Ceiling(request.UsageDuration.Value) : (request.UsageCount ?? 0);
        tooling.CurrentUsage += usageIncrement;
        tooling.UpdatedAt = now;

        // Check if usage exceeds threshold and update status
        if (tooling.ExpectedLifespan.HasValue && tooling.CurrentUsage >= tooling.ExpectedLifespan.Value)
        {
            tooling.Status = "Expired";
            _logger.LogWarning("Tooling {ToolingId} has exceeded its expected lifespan ({CurrentUsage}/{ExpectedLifespan})",
                request.ToolingId, tooling.CurrentUsage, tooling.ExpectedLifespan.Value);
        }
        else if (tooling.ExpectedLifespan.HasValue && tooling.CurrentUsage >= tooling.ExpectedLifespan.Value * 0.8)
        {
            tooling.Status = "Warning";
            _logger.LogInformation("Tooling {ToolingId} is approaching its expected lifespan ({CurrentUsage}/{ExpectedLifespan})",
                request.ToolingId, tooling.CurrentUsage, tooling.ExpectedLifespan.Value);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Logged usage for tooling {ToolingId} by operator {OperatorId}", request.ToolingId, operatorId);

        return MapUsageLogToResponse(log);
    }

    public async Task<ToolingReplacementResponse> RecordReplacementAsync(ToolingReplacementRequest request, string operatorId)
    {
        var oldTooling = await _context.ToolingRegistries
            .FirstOrDefaultAsync(t => t.ToolingId == request.OldToolingId && !t.Deleted);

        if (oldTooling == null)
            throw new KeyNotFoundException($"Old tooling {request.OldToolingId} not found");

        var newTooling = await _context.ToolingRegistries
            .FirstOrDefaultAsync(t => t.ToolingId == request.NewToolingId && !t.Deleted);

        if (newTooling == null)
            throw new KeyNotFoundException($"New tooling {request.NewToolingId} not found");

        var now = DateTime.UtcNow;
        var replacementId = $"REPLACE-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var usagePercentage = oldTooling.ExpectedLifespan.HasValue && oldTooling.ExpectedLifespan.Value > 0
            ? Math.Round((decimal)oldTooling.CurrentUsage / oldTooling.ExpectedLifespan.Value * 100, 2)
            : (decimal?)null;

        var replacement = new ToolingReplacementRecord
        {
            ReplacementId = replacementId,
            OldToolingId = request.OldToolingId,
            OldToolingCode = oldTooling.ToolingCode,
            OldToolingName = oldTooling.ToolingName,
            NewToolingId = request.NewToolingId,
            NewToolingCode = newTooling.ToolingCode,
            NewToolingName = newTooling.ToolingName,
            EquipmentId = request.EquipmentId,
            LotId = request.LotId,
            StepCode = request.StepCode,
            StepSeq = request.StepSeq,
            ReplacementReason = request.ReplacementReason,
            ReasonDetail = request.ReasonDetail,
            OldToolingUsage = oldTooling.CurrentUsage,
            ExpectedLifespan = oldTooling.ExpectedLifespan,
            UsagePercentage = usagePercentage,
            VerificationResult = request.VerificationResult,
            OperatorId = operatorId,
            OperatorName = operatorId,
            ReplacementTime = now,
            CreatedAt = now
        };

        _context.ToolingReplacementRecords.Add(replacement);

        // Update old tooling status
        oldTooling.Status = "Replaced";
        oldTooling.UpdatedAt = now;

        // Update new tooling status
        newTooling.Status = "InUse";
        newTooling.AssociatedEquipment = request.EquipmentId;
        newTooling.UpdatedAt = now;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Replaced tooling {OldToolingId} with {NewToolingId} by operator {OperatorId}",
            request.OldToolingId, request.NewToolingId, operatorId);

        return MapReplacementToResponse(replacement);
    }

    public async Task<PagedResult<ToolingUsageLogResponse>> GetToolingUsageLogsAsync(string toolingId, int pageIndex, int pageSize)
    {
        var tooling = await _context.ToolingRegistries
            .FirstOrDefaultAsync(t => t.ToolingId == toolingId && !t.Deleted);

        if (tooling == null)
            throw new KeyNotFoundException($"Tooling {toolingId} not found");

        var iqQuery = _context.ToolingUsageLogs.Where(l => l.ToolingId == toolingId);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(l => l.StartTime)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(l => MapUsageLogToResponse(l))
            .ToListAsync();

        return new PagedResult<ToolingUsageLogResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    // ==================== 映射方法 ====================

    private static ToolingResponse MapToResponse(ToolingRegistry entity) => new()
    {
        ToolingId = entity.ToolingId,
        ToolingCode = entity.ToolingCode,
        ToolingName = entity.ToolingName,
        ToolingType = entity.ToolingType,
        Specification = entity.Specification,
        Manufacturer = entity.Manufacturer,
        Model = entity.Model,
        ExpectedLifespan = entity.ExpectedLifespan,
        LifespanUnit = entity.LifespanUnit,
        CurrentUsage = entity.CurrentUsage,
        Status = entity.Status,
        AssociatedEquipment = entity.AssociatedEquipment,
        AssociatedProcess = entity.AssociatedProcess,
        NextMaintenanceDate = entity.NextMaintenanceDate,
        CreatedAt = entity.CreatedAt
    };

    private static ToolingUsageLogResponse MapUsageLogToResponse(ToolingUsageLog entity) => new()
    {
        LogId = entity.LogId,
        ToolingId = entity.ToolingId,
        ToolingCode = entity.ToolingCode,
        ToolingName = entity.ToolingName,
        LotId = entity.LotId,
        StepCode = entity.StepCode,
        EquipmentId = entity.EquipmentId,
        StartTime = entity.StartTime,
        EndTime = entity.EndTime,
        UsageDuration = entity.UsageDuration,
        UsageCount = entity.UsageCount,
        UsageStatus = entity.UsageStatus,
        CreatedAt = entity.CreatedAt
    };

    private static ToolingReplacementResponse MapReplacementToResponse(ToolingReplacementRecord entity) => new()
    {
        ReplacementId = entity.ReplacementId,
        OldToolingId = entity.OldToolingId,
        OldToolingCode = entity.OldToolingCode,
        OldToolingName = entity.OldToolingName,
        NewToolingId = entity.NewToolingId,
        NewToolingCode = entity.NewToolingCode,
        NewToolingName = entity.NewToolingName,
        EquipmentId = entity.EquipmentId,
        ReplacementReason = entity.ReplacementReason,
        OldToolingUsage = entity.OldToolingUsage,
        UsagePercentage = entity.UsagePercentage,
        ReplacementTime = entity.ReplacementTime
    };
}
