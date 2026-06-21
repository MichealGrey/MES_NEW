using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase3;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.ProcessControl;

public class ProcessParameterService : IProcessParameterService
{
    private readonly MesDbContext _context;
    private readonly ILogger<ProcessParameterService> _logger;

    public ProcessParameterService(MesDbContext context, ILogger<ProcessParameterService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProcessParameterSetResponse> CreateParameterSetAsync(CreateProcessParameterSetRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var parameterSetId = $"PPS-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var parameterSet = new ProcessParameterSet
        {
            ParameterSetId = parameterSetId,
            ProcessCode = request.ProcessCode,
            ProcessName = request.ProcessName,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            EquipmentType = request.EquipmentType,
            Version = request.Version,
            Status = "Draft",
            Description = request.Description,
            ItemCount = request.Items.Count,
            CreatedBy = operatorId,
            CreatedAt = now,
            UpdatedAt = now,
            Deleted = false
        };

        _context.ProcessParameterSets.Add(parameterSet);

        foreach (var item in request.Items)
        {
            var parameterItem = new ProcessParameterItem
            {
                ParameterSetId = parameterSetId,
                ParameterCode = item.ParameterCode,
                ParameterName = item.ParameterName,
                ParameterType = item.ParameterType,
                Unit = item.Unit,
                StandardValue = item.StandardValue,
                LowerLimit = item.LowerLimit,
                UpperLimit = item.UpperLimit,
                TargetValue = item.TargetValue,
                WarningLowerLimit = item.WarningLowerLimit,
                WarningUpperLimit = item.WarningUpperLimit,
                IsRequired = item.IsRequired,
                IsAutoCollect = item.IsAutoCollect,
                DefaultValue = item.DefaultValue,
                ValidationRule = item.ValidationRule,
                SortOrder = item.SortOrder,
                Remark = item.Remark,
                CreatedAt = now
            };
            _context.ProcessParameterItems.Add(parameterItem);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Created process parameter set {ParameterSetId} for process {ProcessCode}", parameterSetId, request.ProcessCode);

        return MapToResponse(parameterSet);
    }

    public async Task<PagedResult<ProcessParameterSetResponse>> QueryParameterSetsAsync(ProcessParameterQuery query)
    {
        var iqQuery = _context.ProcessParameterSets.Where(p => !p.Deleted).AsQueryable();

        if (!string.IsNullOrEmpty(query.ProcessCode))
            iqQuery = iqQuery.Where(p => p.ProcessCode == query.ProcessCode);
        if (!string.IsNullOrEmpty(query.ProductId))
            iqQuery = iqQuery.Where(p => p.ProductId == query.ProductId);
        if (!string.IsNullOrEmpty(query.EquipmentType))
            iqQuery = iqQuery.Where(p => p.EquipmentType == query.EquipmentType);
        if (!string.IsNullOrEmpty(query.Status))
            iqQuery = iqQuery.Where(p => p.Status == query.Status);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => MapToResponse(p))
            .ToListAsync();

        return new PagedResult<ProcessParameterSetResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<ProcessParameterSetResponse> GetParameterSetAsync(string parameterSetId)
    {
        var parameterSet = await _context.ProcessParameterSets
            .FirstOrDefaultAsync(p => p.ParameterSetId == parameterSetId && !p.Deleted);

        if (parameterSet == null)
            throw new KeyNotFoundException($"Process parameter set {parameterSetId} not found");

        return MapToResponse(parameterSet);
    }

    public async Task<List<ParameterItemResponse>> GetParameterItemsAsync(string parameterSetId)
    {
        var parameterSet = await _context.ProcessParameterSets
            .FirstOrDefaultAsync(p => p.ParameterSetId == parameterSetId && !p.Deleted);

        if (parameterSet == null)
            throw new KeyNotFoundException($"Process parameter set {parameterSetId} not found");

        return await _context.ProcessParameterItems
            .Where(i => i.ParameterSetId == parameterSetId)
            .OrderBy(i => i.SortOrder)
            .Select(i => new ParameterItemResponse
            {
                ItemId = i.ItemId,
                ParameterSetId = i.ParameterSetId,
                ParameterCode = i.ParameterCode,
                ParameterName = i.ParameterName,
                ParameterType = i.ParameterType,
                Unit = i.Unit,
                StandardValue = i.StandardValue,
                LowerLimit = i.LowerLimit,
                UpperLimit = i.UpperLimit,
                TargetValue = i.TargetValue,
                WarningLowerLimit = i.WarningLowerLimit,
                WarningUpperLimit = i.WarningUpperLimit,
                IsRequired = i.IsRequired,
                IsAutoCollect = i.IsAutoCollect,
                DefaultValue = i.DefaultValue,
                ValidationRule = i.ValidationRule,
                SortOrder = i.SortOrder,
                Remark = i.Remark,
                CreatedAt = i.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ProcessParameterSetResponse> ActivateParameterSetAsync(string parameterSetId, ActivateParameterSetRequest request, string operatorId)
    {
        var parameterSet = await _context.ProcessParameterSets
            .FirstOrDefaultAsync(p => p.ParameterSetId == parameterSetId && !p.Deleted);

        if (parameterSet == null)
            throw new KeyNotFoundException($"Process parameter set {parameterSetId} not found");

        var now = DateTime.UtcNow;
        parameterSet.Status = "Active";
        parameterSet.EffectiveDate = request.EffectiveDate;
        parameterSet.ExpiryDate = request.ExpiryDate;
        parameterSet.UpdatedBy = operatorId;
        parameterSet.UpdatedAt = now;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Activated process parameter set {ParameterSetId}", parameterSetId);

        return MapToResponse(parameterSet);
    }

    public async Task<ParameterOverrideLogResponse> OverrideParameterAsync(string parameterSetId, OverrideParameterRequest request, string operatorId)
    {
        var parameterSet = await _context.ProcessParameterSets
            .FirstOrDefaultAsync(p => p.ParameterSetId == parameterSetId && !p.Deleted);

        if (parameterSet == null)
            throw new KeyNotFoundException($"Process parameter set {parameterSetId} not found");

        var item = await _context.ProcessParameterItems
            .FirstOrDefaultAsync(i => i.ParameterSetId == parameterSetId && i.ParameterCode == request.ParameterCode);

        if (item == null)
            throw new KeyNotFoundException($"Parameter {request.ParameterCode} not found in set {parameterSetId}");

        var now = DateTime.UtcNow;
        var log = new ProcessParameterOverrideLog
        {
            ParameterSetId = parameterSetId,
            ItemId = item.ItemId,
            ParameterCode = request.ParameterCode,
            ParameterName = item.ParameterName,
            LotId = request.LotId,
            EquipmentId = request.EquipmentId,
            OriginalValue = item.StandardValue,
            NewValue = request.NewValue,
            OriginalLowerLimit = item.LowerLimit?.ToString(),
            OriginalUpperLimit = item.UpperLimit?.ToString(),
            NewLowerLimit = request.NewLowerLimit,
            NewUpperLimit = request.NewUpperLimit,
            OverrideType = "Manual",
            Reason = request.Reason,
            OperatorId = operatorId,
            OperatorName = operatorId,
            OverrideTime = now
        };

        _context.ProcessParameterOverrideLogs.Add(log);
        await _context.SaveChangesAsync();

        _logger.LogWarning("Parameter {ParameterCode} overridden in set {ParameterSetId} by {OperatorId}", request.ParameterCode, parameterSetId, operatorId);

        return MapOverrideLogToResponse(log);
    }

    public async Task<PagedResult<ParameterOverrideLogResponse>> GetOverrideLogsAsync(string parameterSetId, int pageIndex, int pageSize)
    {
        var parameterSet = await _context.ProcessParameterSets
            .FirstOrDefaultAsync(p => p.ParameterSetId == parameterSetId && !p.Deleted);

        if (parameterSet == null)
            throw new KeyNotFoundException($"Process parameter set {parameterSetId} not found");

        var iqQuery = _context.ProcessParameterOverrideLogs
            .Where(l => l.ParameterSetId == parameterSetId);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(l => l.OverrideTime)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(l => MapOverrideLogToResponse(l))
            .ToListAsync();

        return new PagedResult<ParameterOverrideLogResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    // ==================== 固化温度曲线 ====================

    public async Task<CuringCurveResponse> CreateCuringCurveAsync(CreateCuringCurveRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var curveId = $"CURVE-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var curve = new CuringTemperatureCurve
        {
            CurveId = curveId,
            CurveName = request.CurveName,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            GlueType = request.GlueType,
            EquipmentType = request.EquipmentType,
            Version = request.Version,
            Status = "Draft",
            TotalZones = request.TotalZones,
            PreheatTemp = request.PreheatTemp,
            PreheatDuration = request.PreheatDuration,
            CuringTemp = request.CuringTemp,
            CuringDuration = request.CuringDuration,
            CoolingTemp = request.CoolingTemp,
            CoolingDuration = request.CoolingDuration,
            RampUpRate = request.RampUpRate,
            RampDownRate = request.RampDownRate,
            ZoneTemperatures = request.ZoneTemperatures,
            ProfileData = request.ProfileData,
            CreatedBy = operatorId,
            CreatedAt = now,
            UpdatedAt = now,
            Deleted = false
        };

        _context.CuringTemperatureCurves.Add(curve);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created curing curve {CurveId} for product {ProductId}", curveId, request.ProductId);

        return MapCurveToResponse(curve);
    }

    public async Task<PagedResult<CuringCurveResponse>> QueryCuringCurvesAsync(CuringCurveQuery query)
    {
        var iqQuery = _context.CuringTemperatureCurves.Where(c => !c.Deleted).AsQueryable();

        if (!string.IsNullOrEmpty(query.ProductId))
            iqQuery = iqQuery.Where(c => c.ProductId == query.ProductId);
        if (!string.IsNullOrEmpty(query.EquipmentType))
            iqQuery = iqQuery.Where(c => c.EquipmentType == query.EquipmentType);
        if (!string.IsNullOrEmpty(query.Status))
            iqQuery = iqQuery.Where(c => c.Status == query.Status);
        if (!string.IsNullOrEmpty(query.GlueType))
            iqQuery = iqQuery.Where(c => c.GlueType == query.GlueType);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(c => c.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => MapCurveToResponse(c))
            .ToListAsync();

        return new PagedResult<CuringCurveResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<CuringCurveResponse> GetCuringCurveAsync(string curveId)
    {
        var curve = await _context.CuringTemperatureCurves
            .FirstOrDefaultAsync(c => c.CurveId == curveId && !c.Deleted);

        if (curve == null)
            throw new KeyNotFoundException($"Curing curve {curveId} not found");

        return MapCurveToResponse(curve);
    }

    public async Task<CuringCurveResponse> ActivateCuringCurveAsync(string curveId, string operatorId)
    {
        var curve = await _context.CuringTemperatureCurves
            .FirstOrDefaultAsync(c => c.CurveId == curveId && !c.Deleted);

        if (curve == null)
            throw new KeyNotFoundException($"Curing curve {curveId} not found");

        var now = DateTime.UtcNow;
        curve.Status = "Active";
        curve.EffectiveDate = now;
        curve.UpdatedBy = operatorId;
        curve.UpdatedAt = now;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Activated curing curve {CurveId}", curveId);

        return MapCurveToResponse(curve);
    }

    // ==================== 映射方法 ====================

    private static ProcessParameterSetResponse MapToResponse(ProcessParameterSet entity) => new()
    {
        ParameterSetId = entity.ParameterSetId,
        ProcessCode = entity.ProcessCode,
        ProcessName = entity.ProcessName,
        ProductId = entity.ProductId,
        ProductName = entity.ProductName,
        EquipmentType = entity.EquipmentType,
        Version = entity.Version,
        Status = entity.Status,
        EffectiveDate = entity.EffectiveDate,
        ExpiryDate = entity.ExpiryDate,
        Description = entity.Description,
        ItemCount = entity.ItemCount,
        CreatedBy = entity.CreatedBy,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static ParameterOverrideLogResponse MapOverrideLogToResponse(ProcessParameterOverrideLog entity) => new()
    {
        LogId = entity.LogId,
        ParameterSetId = entity.ParameterSetId,
        ParameterCode = entity.ParameterCode,
        ParameterName = entity.ParameterName,
        LotId = entity.LotId,
        EquipmentId = entity.EquipmentId,
        OriginalValue = entity.OriginalValue,
        NewValue = entity.NewValue,
        OverrideType = entity.OverrideType,
        Reason = entity.Reason,
        OperatorName = entity.OperatorName,
        OverrideTime = entity.OverrideTime
    };

    private static CuringCurveResponse MapCurveToResponse(CuringTemperatureCurve entity) => new()
    {
        CurveId = entity.CurveId,
        CurveName = entity.CurveName,
        ProductId = entity.ProductId,
        ProductName = entity.ProductName,
        GlueType = entity.GlueType,
        EquipmentType = entity.EquipmentType,
        Version = entity.Version,
        Status = entity.Status,
        TotalZones = entity.TotalZones,
        PreheatTemp = entity.PreheatTemp,
        PreheatDuration = entity.PreheatDuration,
        CuringTemp = entity.CuringTemp,
        CuringDuration = entity.CuringDuration,
        ZoneTemperatures = entity.ZoneTemperatures,
        ProfileData = entity.ProfileData,
        EffectiveDate = entity.EffectiveDate,
        CreatedBy = entity.CreatedBy,
        CreatedAt = entity.CreatedAt
    };
}
