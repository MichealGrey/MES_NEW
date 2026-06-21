using Microsoft.EntityFrameworkCore;
using MES.Contracts.Common;
using MES.Contracts.Equipment;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.Extensions.Logging;

namespace MES.Services.Equipment;

public interface IEquipmentService
{
    /// <summary>
    /// 查询设备列表（分页）
    /// </summary>
    Task<PagedResult<EquipmentListResponse>> GetEquipmentAsync(
        int page, int pageSize, string? type = null, string? status = null, string? location = null);

    /// <summary>
    /// 获取设备详情
    /// </summary>
    Task<EquipmentDetailResponse?> GetEquipmentDetailAsync(string equipmentId);

    /// <summary>
    /// 更新设备状态
    /// </summary>
    Task<bool> UpdateEquipmentStatusAsync(EquipmentStatusUpdateRequest request);

    /// <summary>
    /// 创建维护记录
    /// </summary>
    Task<MaintenanceResponse> CreateMaintenanceAsync(MaintenanceCreateRequest request);

    /// <summary>
    /// 查询维护记录
    /// </summary>
    Task<List<MaintenanceResponse>> GetMaintenanceAsync(string equipmentId, string? status = null);

    /// <summary>
    /// 完成维护
    /// </summary>
    Task<bool> CompleteMaintenanceAsync(long maintenanceId, double actualHours, string? notes, string? technicianId);

    /// <summary>
    /// 创建故障记录
    /// </summary>
    Task<FailureResponse> CreateFailureAsync(FailureCreateRequest request);

    /// <summary>
    /// 查询故障记录
    /// </summary>
    Task<List<FailureResponse>> GetFailuresAsync(string equipmentId, string? status = null, int? days = null);

    /// <summary>
    /// 解决故障
    /// </summary>
    Task<bool> ResolveFailureAsync(long failureId, string resolution, string? resolvedBy);

    /// <summary>
    /// 计算设备OEE
    /// </summary>
    Task<EquipmentOeeResponse> GetEquipmentOeeAsync(string equipmentId, int days = 30);

    /// <summary>
    /// 设备看板
    /// </summary>
    Task<EquipmentDashboardResponse> GetDashboardAsync();
}

public class EquipmentService : IEquipmentService
{
    private readonly MesDbContext _context;
    private readonly ILogger<EquipmentService> _logger;

    private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Available", "Running", "Maintenance", "Breakdown", "Offline"
    };

    private static readonly HashSet<string> ValidMaintenanceTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Preventive", "Corrective", "Calibration"
    };

    private static readonly HashSet<string> ValidSeverities = new(StringComparer.OrdinalIgnoreCase)
    {
        "Minor", "Major", "Critical"
    };

    public EquipmentService(MesDbContext context, ILogger<EquipmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<EquipmentListResponse>> GetEquipmentAsync(
        int page, int pageSize, string? type = null, string? status = null, string? location = null)
    {
        var query = _context.MasterEquipments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(e => e.EquipmentType == type || e.EquipmentGroup == type);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status == status);

        if (!string.IsNullOrWhiteSpace(location))
            query = query.Where(e => e.Location != null && e.Location.Contains(location));

        query = query.OrderByDescending(e => e.EquipmentName);

        var totalCount = await query.CountAsync();

        var equipments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Calculate OEE for each equipment (last 30 days)
        var cutoffDate = DateTime.UtcNow.AddDays(-30);
        var equipmentIds = equipments.Select(e => e.EquipmentId).ToList();

        var oeeData = await _context.ProdOperationHistories
            .Where(h => equipmentIds.Contains(h.EquipmentId!) && h.CreatedAt >= cutoffDate)
            .GroupBy(h => h.EquipmentId)
            .Select(g => new
            {
                EquipmentId = g.Key,
                TotalUnits = g.Sum(h => (int?)h.OutputQty) ?? 0,
                ScrapQty = g.Sum(h => (int?)h.ScrapQty) ?? 0
            })
            .ToDictionaryAsync(x => x.EquipmentId!);

        var failureData = await _context.EquipmentFailures
            .Where(f => equipmentIds.Contains(f.EquipmentId) && f.ReportedAt >= cutoffDate)
            .GroupBy(f => f.EquipmentId)
            .Select(g => new
            {
                EquipmentId = g.Key,
                TotalDowntime = g.Sum(f => f.DowntimeMinutes)
            })
            .ToDictionaryAsync(x => x.EquipmentId);

        var items = equipments.Select(e =>
        {
            double? oee = null;
            if (oeeData.TryGetValue(e.EquipmentId, out var oeeInfo))
            {
                var totalPlanned = e.RunningHours * 60.0; // minutes
                var downtime = failureData.TryGetValue(e.EquipmentId, out var failInfo)
                    ? failInfo.TotalDowntime ?? 0
                    : 0;
                var runTime = Math.Max(0, totalPlanned - downtime);
                var availability = totalPlanned > 0 ? runTime / totalPlanned : 1.0;
                var idealCycleRate = 1.0; // units per minute, should be configurable
                var performance = runTime > 0 ? (double)oeeInfo.TotalUnits / runTime / idealCycleRate : 0;
                var quality = oeeInfo.TotalUnits > 0
                    ? (double)(oeeInfo.TotalUnits - oeeInfo.ScrapQty) / oeeInfo.TotalUnits
                    : 1.0;
                oee = Math.Round(availability * Math.Min(performance, 1.0) * quality * 100, 2);
            }

            return new EquipmentListResponse
            {
                EquipmentId = e.EquipmentId,
                EquipmentName = e.EquipmentName,
                Type = e.EquipmentType,
                Status = e.Status,
                Location = e.Location,
                LastMaintenance = e.LastMaintenanceDate,
                Oee = oee
            };
        }).ToList();

        return new PagedResult<EquipmentListResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = page,
            PageSize = pageSize
        };
    }

    public async Task<EquipmentDetailResponse?> GetEquipmentDetailAsync(string equipmentId)
    {
        var equipment = await _context.MasterEquipments
            .FirstOrDefaultAsync(e => e.EquipmentId == equipmentId);

        if (equipment == null) return null;

        var recentMaintenances = await _context.EquipmentMaintenances
            .Where(m => m.EquipmentId == equipmentId)
            .OrderByDescending(m => m.ScheduledDate)
            .Take(5)
            .Select(m => new MaintenanceResponse
            {
                MaintenanceId = m.Id,
                EquipmentId = m.EquipmentId,
                Type = m.MaintenanceType,
                Status = m.Status,
                Technician = m.TechnicianName,
                ScheduledDate = m.ScheduledDate,
                CompletedDate = m.CompletedDate,
                Description = m.Description,
                ActualHours = m.ActualHours,
                EstimatedHours = m.EstimatedHours,
                PartsReplaced = m.PartsReplaced
            })
            .ToListAsync();

        var recentFailures = await _context.EquipmentFailures
            .Where(f => f.EquipmentId == equipmentId)
            .OrderByDescending(f => f.ReportedAt)
            .Take(5)
            .Select(f => new FailureResponse
            {
                FailureId = f.Id,
                EquipmentId = f.EquipmentId,
                Type = f.FailureType,
                Severity = f.Severity,
                Status = f.Status,
                ReportedBy = f.ReportedByName ?? f.ReportedBy,
                ReportedTime = f.ReportedAt,
                ResolvedTime = f.ResolvedAt,
                DowntimeMinutes = f.DowntimeMinutes,
                Resolution = f.Resolution,
                RootCause = f.RootCause
            })
            .ToListAsync();

        return new EquipmentDetailResponse
        {
            EquipmentId = equipment.EquipmentId,
            EquipmentName = equipment.EquipmentName,
            EquipmentGroup = equipment.EquipmentGroup,
            EquipmentType = equipment.EquipmentType,
            ProcessStage = equipment.ProcessStage,
            Vendor = equipment.Vendor,
            Model = equipment.Model,
            SerialNumber = equipment.SerialNumber,
            Status = equipment.Status,
            CurrentLotId = equipment.CurrentLotId,
            CurrentRecipe = equipment.CurrentRecipe,
            Location = equipment.Location,
            ResponsiblePerson = equipment.ResponsiblePerson,
            LastMaintenanceDate = equipment.LastMaintenanceDate,
            MaintenanceIntervalHours = equipment.MaintenanceIntervalHours,
            RunningHours = equipment.RunningHours,
            RecentMaintenances = recentMaintenances,
            RecentFailures = recentFailures
        };
    }

    public async Task<bool> UpdateEquipmentStatusAsync(EquipmentStatusUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EquipmentId))
            throw new ArgumentException("设备ID不能为空", nameof(request.EquipmentId));

        if (string.IsNullOrWhiteSpace(request.Status))
            throw new ArgumentException("状态不能为空", nameof(request.Status));

        if (!ValidStatuses.Contains(request.Status))
            throw new ArgumentException($"无效的设备状态: {request.Status}");

        var equipment = await _context.MasterEquipments
            .FirstOrDefaultAsync(e => e.EquipmentId == request.EquipmentId);

        if (equipment == null)
        {
            _logger.LogWarning("设备不存在: {EquipmentId}", request.EquipmentId);
            return false;
        }

        var oldStatus = equipment.Status;
        equipment.Status = request.Status;

        if (request.Status == "Available" && oldStatus == "Maintenance")
        {
            equipment.LastMaintenanceDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "设备状态已更新: {EquipmentId} {OldStatus} -> {NewStatus}, 原因: {Reason}, 操作员: {OperatorId}",
            request.EquipmentId, oldStatus, request.Status, request.Reason, request.OperatorId);

        return true;
    }

    public async Task<MaintenanceResponse> CreateMaintenanceAsync(MaintenanceCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EquipmentId))
            throw new ArgumentException("设备ID不能为空", nameof(request.EquipmentId));

        if (string.IsNullOrWhiteSpace(request.MaintenanceType))
            throw new ArgumentException("维护类型不能为空", nameof(request.MaintenanceType));

        if (!ValidMaintenanceTypes.Contains(request.MaintenanceType))
            throw new ArgumentException($"无效的维护类型: {request.MaintenanceType}");

        var equipment = await _context.MasterEquipments
            .FirstOrDefaultAsync(e => e.EquipmentId == request.EquipmentId);

        if (equipment == null)
            throw new InvalidOperationException($"设备不存在: {request.EquipmentId}");

        var technicianName = "";
        if (!string.IsNullOrWhiteSpace(request.TechnicianId))
        {
            var user = await _context.SysUsers
                .FirstOrDefaultAsync(u => u.UserId == request.TechnicianId);
            technicianName = user?.UserName ?? "";
        }

        var maintenance = new EquipmentMaintenance
        {
            EquipmentId = request.EquipmentId,
            MaintenanceType = request.MaintenanceType,
            Description = request.Description,
            Status = "Scheduled",
            TechnicianId = request.TechnicianId,
            TechnicianName = technicianName,
            ScheduledDate = request.ScheduledDate,
            EstimatedHours = request.EstimatedHours,
            CreatedBy = request.ReportedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.EquipmentMaintenances.AddAsync(maintenance);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "维护记录已创建: EquipmentId={EquipmentId}, Type={Type}, ScheduledDate={Date}",
            request.EquipmentId, request.MaintenanceType, request.ScheduledDate);

        return new MaintenanceResponse
        {
            MaintenanceId = maintenance.Id,
            EquipmentId = maintenance.EquipmentId,
            Type = maintenance.MaintenanceType,
            Status = maintenance.Status,
            Technician = maintenance.TechnicianName,
            ScheduledDate = maintenance.ScheduledDate,
            Description = maintenance.Description,
            ActualHours = maintenance.ActualHours,
            EstimatedHours = maintenance.EstimatedHours,
            PartsReplaced = maintenance.PartsReplaced
        };
    }

    public async Task<List<MaintenanceResponse>> GetMaintenanceAsync(string equipmentId, string? status = null)
    {
        var query = _context.EquipmentMaintenances
            .Where(m => m.EquipmentId == equipmentId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(m => m.Status == status);

        var maintenances = await query
            .OrderByDescending(m => m.ScheduledDate)
            .Select(m => new MaintenanceResponse
            {
                MaintenanceId = m.Id,
                EquipmentId = m.EquipmentId,
                Type = m.MaintenanceType,
                Status = m.Status,
                Technician = m.TechnicianName,
                ScheduledDate = m.ScheduledDate,
                CompletedDate = m.CompletedDate,
                Description = m.Description,
                ActualHours = m.ActualHours,
                EstimatedHours = m.EstimatedHours,
                PartsReplaced = m.PartsReplaced
            })
            .ToListAsync();

        return maintenances;
    }

    public async Task<bool> CompleteMaintenanceAsync(long maintenanceId, double actualHours, string? notes, string? technicianId)
    {
        var maintenance = await _context.EquipmentMaintenances
            .FirstOrDefaultAsync(m => m.Id == maintenanceId);

        if (maintenance == null)
        {
            _logger.LogWarning("维护记录不存在: {MaintenanceId}", maintenanceId);
            return false;
        }

        if (maintenance.Status is "Completed" or "Cancelled")
        {
            _logger.LogWarning("维护记录已完成或已取消: {MaintenanceId}", maintenanceId);
            return false;
        }

        maintenance.Status = "Completed";
        maintenance.ActualHours = actualHours;
        maintenance.Notes = notes;
        maintenance.CompletedDate = DateTime.UtcNow;
        maintenance.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(technicianId))
        {
            maintenance.TechnicianId = technicianId;
            var user = await _context.SysUsers
                .FirstOrDefaultAsync(u => u.UserId == technicianId);
            maintenance.TechnicianName = user?.UserName ?? maintenance.TechnicianName;
        }

        // Update equipment last maintenance date
        var equipment = await _context.MasterEquipments
            .FirstOrDefaultAsync(e => e.EquipmentId == maintenance.EquipmentId);

        if (equipment != null)
        {
            equipment.LastMaintenanceDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "维护记录已完成: {MaintenanceId}, EquipmentId={EquipmentId}, ActualHours={Hours}",
            maintenanceId, maintenance.EquipmentId, actualHours);

        return true;
    }

    public async Task<FailureResponse> CreateFailureAsync(FailureCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EquipmentId))
            throw new ArgumentException("设备ID不能为空", nameof(request.EquipmentId));

        if (string.IsNullOrWhiteSpace(request.FailureType))
            throw new ArgumentException("故障类型不能为空", nameof(request.FailureType));

        if (!ValidSeverities.Contains(request.Severity))
            throw new ArgumentException($"无效的故障严重度: {request.Severity}");

        var equipment = await _context.MasterEquipments
            .FirstOrDefaultAsync(e => e.EquipmentId == request.EquipmentId);

        if (equipment == null)
            throw new InvalidOperationException($"设备不存在: {request.EquipmentId}");

        var reportedByName = "";
        if (!string.IsNullOrWhiteSpace(request.ReportedBy))
        {
            var user = await _context.SysUsers
                .FirstOrDefaultAsync(u => u.UserId == request.ReportedBy);
            reportedByName = user?.UserName ?? "";
        }

        var failure = new EquipmentFailure
        {
            EquipmentId = request.EquipmentId,
            FailureType = request.FailureType,
            Description = request.Description,
            Severity = request.Severity,
            Status = "Open",
            ReportedBy = request.ReportedBy,
            ReportedByName = reportedByName,
            ReportedAt = DateTime.UtcNow,
            DowntimeMinutes = request.DowntimeMinutes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // If Critical severity, set equipment to Breakdown
        if (request.Severity.Equals("Critical", StringComparison.OrdinalIgnoreCase))
        {
            equipment.Status = "Breakdown";
        }

        await _context.EquipmentFailures.AddAsync(failure);
        await _context.SaveChangesAsync();

        _logger.LogWarning(
            "故障记录已创建: EquipmentId={EquipmentId}, Type={Type}, Severity={Severity}",
            request.EquipmentId, request.FailureType, request.Severity);

        return new FailureResponse
        {
            FailureId = failure.Id,
            EquipmentId = failure.EquipmentId,
            Type = failure.FailureType,
            Severity = failure.Severity,
            Status = failure.Status,
            ReportedBy = failure.ReportedByName ?? failure.ReportedBy,
            ReportedTime = failure.ReportedAt,
            DowntimeMinutes = failure.DowntimeMinutes
        };
    }

    public async Task<List<FailureResponse>> GetFailuresAsync(string equipmentId, string? status = null, int? days = null)
    {
        var query = _context.EquipmentFailures
            .Where(f => f.EquipmentId == equipmentId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(f => f.Status == status);

        if (days.HasValue && days.Value > 0)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-days.Value);
            query = query.Where(f => f.ReportedAt >= cutoffDate);
        }

        var failures = await query
            .OrderByDescending(f => f.ReportedAt)
            .Select(f => new FailureResponse
            {
                FailureId = f.Id,
                EquipmentId = f.EquipmentId,
                Type = f.FailureType,
                Severity = f.Severity,
                Status = f.Status,
                ReportedBy = f.ReportedByName ?? f.ReportedBy,
                ReportedTime = f.ReportedAt,
                ResolvedTime = f.ResolvedAt,
                DowntimeMinutes = f.DowntimeMinutes,
                Resolution = f.Resolution,
                RootCause = f.RootCause
            })
            .ToListAsync();

        return failures;
    }

    public async Task<bool> ResolveFailureAsync(long failureId, string resolution, string? resolvedBy)
    {
        var failure = await _context.EquipmentFailures
            .FirstOrDefaultAsync(f => f.Id == failureId);

        if (failure == null)
        {
            _logger.LogWarning("故障记录不存在: {FailureId}", failureId);
            return false;
        }

        if (failure.Status is "Resolved" or "Closed")
        {
            _logger.LogWarning("故障记录已解决或已关闭: {FailureId}", failureId);
            return false;
        }

        failure.Status = "Resolved";
        failure.Resolution = resolution;
        failure.ResolvedAt = DateTime.UtcNow;
        failure.ResolvedBy = resolvedBy;
        failure.UpdatedAt = DateTime.UtcNow;

        // Calculate downtime if not provided
        if (!failure.DowntimeMinutes.HasValue)
        {
            failure.DowntimeMinutes = (int)(failure.ResolvedAt.Value - failure.ReportedAt).TotalMinutes;
        }

        // If equipment was in Breakdown status, set to Available
        var equipment = await _context.MasterEquipments
            .FirstOrDefaultAsync(e => e.EquipmentId == failure.EquipmentId);

        if (equipment != null && equipment.Status == "Breakdown")
        {
            equipment.Status = "Available";
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "故障记录已解决: {FailureId}, EquipmentId={EquipmentId}",
            failureId, failure.EquipmentId);

        return true;
    }

    public async Task<EquipmentOeeResponse> GetEquipmentOeeAsync(string equipmentId, int days = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        var equipment = await _context.MasterEquipments
            .FirstOrDefaultAsync(e => e.EquipmentId == equipmentId);

        if (equipment == null)
            throw new InvalidOperationException($"设备不存在: {equipmentId}");

        // Get operation history for this equipment
        var operations = await _context.ProdOperationHistories
            .Where(h => h.EquipmentId == equipmentId && h.CreatedAt >= cutoffDate)
            .ToListAsync();

        // Get failures for downtime calculation
        var failures = await _context.EquipmentFailures
            .Where(f => f.EquipmentId == equipmentId && f.ReportedAt >= cutoffDate)
            .ToListAsync();

        var totalPlannedProductionTime = equipment.RunningHours * 60.0; // Convert hours to minutes
        var totalDowntime = failures.Sum(f => f.DowntimeMinutes ?? 0);
        var runTime = Math.Max(0, totalPlannedProductionTime - totalDowntime);

        var totalUnits = operations.Sum(h => (long)(h.OutputQty ?? 0));
        var goodUnits = operations.Sum(h => (long)((h.OutputQty ?? 0) - (h.ScrapQty ?? 0)));

        // OEE Calculation
        var availability = totalPlannedProductionTime > 0 ? runTime / totalPlannedProductionTime : 0;
        var idealCycleRate = 1.0; // units per minute - should be from equipment config
        var performance = runTime > 0 ? (double)totalUnits / runTime / idealCycleRate : 0;
        var quality = totalUnits > 0 ? (double)goodUnits / totalUnits : 0;

        // Cap performance at 1.0
        performance = Math.Min(performance, 1.0);

        var oee = availability * performance * quality;

        return new EquipmentOeeResponse
        {
            EquipmentId = equipmentId,
            Availability = Math.Round(availability * 100, 2),
            Performance = Math.Round(performance * 100, 2),
            Quality = Math.Round(quality * 100, 2),
            Oee = Math.Round(oee * 100, 2),
            RunTime = Math.Round(runTime, 2),
            Downtime = Math.Round((double)totalDowntime, 2),
            TotalUnits = totalUnits,
            GoodUnits = goodUnits
        };
    }

    public async Task<EquipmentDashboardResponse> GetDashboardAsync()
    {
        var allEquipments = await _context.MasterEquipments.ToListAsync();

        var statusDistribution = allEquipments
            .GroupBy(e => e.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        // Active alerts (critical failures and breakdown equipment)
        var criticalFailures = await _context.EquipmentFailures
            .Where(f => f.Severity == "Critical" && (f.Status == "Open" || f.Status == "InProgress"))
            .OrderByDescending(f => f.ReportedAt)
            .Take(10)
            .ToListAsync();

        var breakdownEquipments = allEquipments
            .Where(e => e.Status == "Breakdown")
            .ToList();

        var activeAlerts = new List<EquipmentAlert>();

        foreach (var failure in criticalFailures)
        {
            var equip = allEquipments.FirstOrDefault(e => e.EquipmentId == failure.EquipmentId);
            activeAlerts.Add(new EquipmentAlert
            {
                EquipmentId = failure.EquipmentId,
                EquipmentName = equip?.EquipmentName ?? failure.EquipmentId,
                AlertType = "Failure",
                Severity = failure.Severity,
                Message = failure.Description,
                CreatedAt = failure.ReportedAt
            });
        }

        foreach (var equip in breakdownEquipments)
        {
            if (!activeAlerts.Any(a => a.EquipmentId == equip.EquipmentId))
            {
                activeAlerts.Add(new EquipmentAlert
                {
                    EquipmentId = equip.EquipmentId,
                    EquipmentName = equip.EquipmentName,
                    AlertType = "Breakdown",
                    Severity = "Critical",
                    Message = $"设备 {equip.EquipmentName} 处于故障状态",
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        // Upcoming maintenances (scheduled within next 7 days)
        var sevenDaysLater = DateTime.UtcNow.AddDays(7);
        var upcomingMaintenances = await _context.EquipmentMaintenances
            .Where(m => m.Status == "Scheduled" && m.ScheduledDate <= sevenDaysLater && m.ScheduledDate >= DateTime.UtcNow)
            .OrderBy(m => m.ScheduledDate)
            .Take(10)
            .ToListAsync();

        var upcomingMaintList = upcomingMaintenances.Select(m =>
        {
            var equip = allEquipments.FirstOrDefault(e => e.EquipmentId == m.EquipmentId);
            return new UpcomingMaintenance
            {
                EquipmentId = m.EquipmentId,
                EquipmentName = equip?.EquipmentName ?? m.EquipmentId,
                MaintenanceType = m.MaintenanceType,
                ScheduledDate = m.ScheduledDate
            };
        }).ToList();

        return new EquipmentDashboardResponse
        {
            TotalEquipment = allEquipments.Count,
            StatusDistribution = statusDistribution,
            ActiveAlerts = activeAlerts,
            UpcomingMaintenances = upcomingMaintList
        };
    }
}
