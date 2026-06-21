using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Production;

public class EquipmentMaintenanceService : IEquipmentMaintenanceService
{
    private readonly MesDbContext _context;

    public EquipmentMaintenanceService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<EquipmentFaultResponse> ReportFaultAsync(ReportEquipmentFaultRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EquipmentId))
            throw new ApplicationException("设备ID不能为空");
        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ApplicationException("故障描述不能为空");

        var faultId = GenerateId("EF");
        var now = DateTime.UtcNow;

        var equipment = await _context.MasterEquipments.FindAsync(request.EquipmentId);

        var fault = new EquipmentFaultRecord
        {
            FaultId = faultId,
            EquipmentId = request.EquipmentId,
            EquipmentName = equipment?.EquipmentName,
            FaultType = request.FaultType,
            FaultDescription = request.Description,
            Severity = request.Severity,
            FaultTime = now,
            Status = "Reported",
            ReportedBy = request.ReportedBy,
            ReportedByName = request.ReportedByName,
            Remark = request.Remark,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<EquipmentFaultRecord>().AddAsync(fault);

        // Update equipment status
        if (equipment != null)
        {
            equipment.Status = "Fault";
            equipment.CurrentLotId = null;
        }

        await _context.SaveChangesAsync();

        return new EquipmentFaultResponse
        {
            FaultId = fault.FaultId,
            EquipmentId = fault.EquipmentId,
            EquipmentName = fault.EquipmentName ?? string.Empty,
            FaultType = fault.FaultType,
            Severity = fault.Severity,
            Status = fault.Status,
            ReportedAt = fault.FaultTime
        };
    }

    public async Task<PagedResult<EquipmentFaultResponse>> GetFaultsAsync(EquipmentFaultQuery query)
    {
        var iq = _context.Set<EquipmentFaultRecord>().AsQueryable();

        if (!string.IsNullOrEmpty(query.EquipmentId))
            iq = iq.Where(f => f.EquipmentId == query.EquipmentId);
        if (!string.IsNullOrEmpty(query.Severity))
            iq = iq.Where(f => f.Severity == query.Severity);
        if (!string.IsNullOrEmpty(query.Status))
            iq = iq.Where(f => f.Status == query.Status);
        if (query.DateFrom.HasValue)
            iq = iq.Where(f => f.FaultTime >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(f => f.FaultTime <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var items = await iq
            .OrderByDescending(f => f.FaultTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(f => new EquipmentFaultResponse
            {
                FaultId = f.FaultId,
                EquipmentId = f.EquipmentId,
                EquipmentName = f.EquipmentName ?? string.Empty,
                FaultType = f.FaultType,
                Severity = f.Severity,
                Status = f.Status,
                ReportedAt = f.FaultTime,
                CompletedAt = f.RepairEndTime,
                RepairDurationMinutes = f.RepairDurationMinutes
            })
            .ToListAsync();

        return new PagedResult<EquipmentFaultResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<bool> DispatchFaultAsync(DispatchFaultRequest request)
    {
        var fault = await _context.Set<EquipmentFaultRecord>().FindAsync(request.FaultId);
        if (fault == null)
            throw new ApplicationException($"故障记录 {request.FaultId} 不存在");

        fault.Status = "Dispatched";
        fault.RepairBy = request.AssigneeId;
        fault.RepairByName = request.AssigneeName;
        fault.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteRepairAsync(CompleteRepairRequest request)
    {
        var fault = await _context.Set<EquipmentFaultRecord>().FindAsync(request.FaultId);
        if (fault == null)
            throw new ApplicationException($"故障记录 {request.FaultId} 不存在");

        var now = DateTime.UtcNow;
        fault.Status = "Completed";
        fault.RepairEndTime = now;
        fault.RepairAction = request.RepairAction;
        fault.RootCause = request.RootCause;
        fault.RepairDurationMinutes = (int)(now - fault.FaultTime).TotalMinutes;
        fault.UpdatedAt = now;

        // Update equipment status back to Available
        var equipment = await _context.MasterEquipments.FindAsync(fault.EquipmentId);
        if (equipment != null)
        {
            equipment.Status = "Available";
        }

        // Record spare parts if any
        if (request.SparePartsUsed != null && request.SparePartsUsed.Length > 0)
        {
            foreach (var part in request.SparePartsUsed)
            {
                var sparePart = new EquipmentRepairSparePart
                {
                    FaultId = request.FaultId,
                    SparePartId = part,
                    SparePartName = part,
                    Quantity = 1,
                    CreatedAt = now
                };
                await _context.Set<EquipmentRepairSparePart>().AddAsync(sparePart);
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PmPlanResponse> CreatePmPlanAsync(CreatePmPlanRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EquipmentId))
            throw new ApplicationException("设备ID不能为空");

        var planId = GenerateId("PM");
        var now = DateTime.UtcNow;

        var equipment = await _context.MasterEquipments.FindAsync(request.EquipmentId);

        var plan = new EquipmentPmPlan
        {
            PmPlanId = planId,
            EquipmentId = request.EquipmentId,
            EquipmentName = equipment?.EquipmentName,
            PmType = request.PmType,
            PmName = request.Description,
            PmDescription = request.Description,
            NextDueDate = request.PlannedDate,
            Status = "Active",
            CreatedBy = "System",
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<EquipmentPmPlan>().AddAsync(plan);
        await _context.SaveChangesAsync();

        return new PmPlanResponse
        {
            PlanId = plan.PmPlanId,
            EquipmentId = plan.EquipmentId,
            EquipmentName = plan.EquipmentName ?? string.Empty,
            PmType = plan.PmType,
            Description = plan.PmName,
            PlannedDate = plan.NextDueDate ?? request.PlannedDate,
            Status = plan.Status
        };
    }

    public async Task<PagedResult<PmPlanResponse>> GetPmPlansAsync(PmPlanQuery query)
    {
        var iq = _context.Set<EquipmentPmPlan>().AsQueryable();

        if (!string.IsNullOrEmpty(query.EquipmentId))
            iq = iq.Where(p => p.EquipmentId == query.EquipmentId);
        if (!string.IsNullOrEmpty(query.PmType))
            iq = iq.Where(p => p.PmType == query.PmType);
        if (!string.IsNullOrEmpty(query.Status))
            iq = iq.Where(p => p.Status == query.Status);
        if (query.DateFrom.HasValue)
            iq = iq.Where(p => p.NextDueDate >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(p => p.NextDueDate <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var items = await iq
            .OrderBy(p => p.NextDueDate)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new PmPlanResponse
            {
                PlanId = p.PmPlanId,
                EquipmentId = p.EquipmentId,
                EquipmentName = p.EquipmentName ?? string.Empty,
                PmType = p.PmType,
                Description = p.PmName,
                PlannedDate = p.NextDueDate ?? DateTime.UtcNow,
                Status = p.Status
            })
            .ToListAsync();

        return new PagedResult<PmPlanResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<bool> ExecutePmAsync(ExecutePmRequest request)
    {
        var plan = await _context.Set<EquipmentPmPlan>().FindAsync(request.PlanId);
        if (plan == null)
            throw new ApplicationException($"保养计划 {request.PlanId} 不存在");

        var now = DateTime.UtcNow;
        var execId = GenerateId("PME");

        var execution = new EquipmentPmExecution
        {
            PmExecutionId = execId,
            PmPlanId = request.PlanId,
            EquipmentId = plan.EquipmentId,
            EquipmentName = plan.EquipmentName,
            PmType = plan.PmType,
            PmContent = plan.PmDescription,
            ScheduledDate = plan.NextDueDate ?? now,
            ActualStartTime = now,
            ActualEndTime = now,
            ActualDurationMinutes = 0,
            ExecutedBy = request.ExecutedBy,
            ExecutedByName = request.ExecutedBy,
            Result = request.Result,
            IssuesFound = request.Findings,
            Status = "Completed",
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<EquipmentPmExecution>().AddAsync(execution);

        plan.Status = "Completed";
        plan.UpdatedAt = now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<MtbfMttrResponse>> GetMtbfMttrAsync(string equipmentId, string period)
    {
        // Parse period (e.g., "2026-06" or "2026-Q2")
        DateTime startDate;
        if (period.Length == 7 && period.Contains("-"))
        {
            // Monthly: "YYYY-MM"
            var parts = period.Split('-');
            startDate = new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), 1);
        }
        else
        {
            // Default to last 30 days
            startDate = DateTime.UtcNow.AddDays(-30);
        }

        var faults = await _context.Set<EquipmentFaultRecord>()
            .Where(f => f.EquipmentId == equipmentId && f.FaultTime >= startDate && f.Status == "Completed")
            .ToListAsync();

        var totalFaults = faults.Count;
        var totalRepairHours = faults.Sum(f => (f.RepairDurationMinutes ?? 0) / 60.0);
        var totalOperatingHours = (DateTime.UtcNow - startDate).TotalHours;

        var mtbf = totalFaults > 0 ? Math.Round((decimal)(totalOperatingHours / totalFaults), 2) : 0m;
        var mttr = totalFaults > 0 ? Math.Round((decimal)(totalRepairHours / totalFaults), 2) : 0m;
        var availability = totalOperatingHours > 0
            ? Math.Round((decimal)((totalOperatingHours - totalRepairHours) / totalOperatingHours * 100), 2)
            : 100m;

        return new List<MtbfMttrResponse>
        {
            new MtbfMttrResponse
            {
                EquipmentId = equipmentId,
                EquipmentName = faults.FirstOrDefault()?.EquipmentName ?? string.Empty,
                MtbfHours = mtbf,
                MttrHours = mttr,
                TotalFaults = totalFaults,
                AvailabilityPercent = availability,
                Period = period
            }
        };
    }

    private static string GenerateId(string prefix)
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 9999).ToString("D4");
        return $"{prefix}-{now:yyyyMMdd}-{seq}";
    }
}
