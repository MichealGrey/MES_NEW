using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Order.Models;
using MES.Domain.Production;

namespace MES.Modules.Order.Services;

public class ProductionDataService : IProductionDataService
{
    private readonly IRepository<ProdWorkOrder> _woRepo;

    public ProductionDataService(IRepository<ProdWorkOrder> woRepo)
    {
        _woRepo = woRepo;
    }

    public async Task EnsureSeededAsync()
    {
    }

    public async Task<List<WorkOrderInfo>> GetAllWorkOrdersAsync()
    {
        var entities = await _woRepo.GetAllAsync();
        return entities.Select(MapToWorkOrderInfo).ToList();
    }

    public async Task<WorkOrderInfo?> GetWorkOrderAsync(string orderId)
    {
        var entity = await _woRepo.GetByIdAsync(orderId);
        return entity is null ? null : MapToWorkOrderInfo(entity);
    }

    public async Task SaveWorkOrderAsync(WorkOrderInfo workOrder)
    {
        var entity = MapToEntity(workOrder);
        var existing = await _woRepo.GetByIdAsync(workOrder.OrderId);
        if (existing is null)
            await _woRepo.AddAsync(entity);
        else
            await _woRepo.UpdateAsync(entity);
    }

    public async Task DeleteWorkOrderAsync(string orderId)
    {
        var entity = await _woRepo.GetByIdAsync(orderId);
        if (entity is not null)
            await _woRepo.DeleteAsync(entity);
    }

    public async Task UpdateWorkOrderStatusAsync(string orderId, ProcessStatus status)
    {
        var entity = await _woRepo.GetByIdAsync(orderId);
        if (entity is null) return;

        entity.Status = status.ToString();
        entity.UpdatedAt = DateTime.UtcNow;

        if (status == ProcessStatus.Completed)
            entity.ActualEndDate = DateTime.UtcNow;
        else if (status == ProcessStatus.InProgress && !entity.ActualStartDate.HasValue)
            entity.ActualStartDate = DateTime.UtcNow;

        await _woRepo.UpdateAsync(entity);
    }

    public async Task HoldWorkOrderAsync(string orderId, string reason, string? remark = null)
    {
        var entity = await _woRepo.GetByIdAsync(orderId);
        if (entity is null) return;

        entity.Status = "Hold";
        entity.Remark = remark;
        entity.UpdatedAt = DateTime.UtcNow;
        await _woRepo.UpdateAsync(entity);
    }

    public async Task ReleaseHoldWorkOrderAsync(string orderId)
    {
        var entity = await _woRepo.GetByIdAsync(orderId);
        if (entity is null) return;

        entity.Status = "Released";
        entity.UpdatedAt = DateTime.UtcNow;
        await _woRepo.UpdateAsync(entity);
    }

    private static WorkOrderInfo MapToWorkOrderInfo(ProdWorkOrder e) => new()
    {
        OrderId = e.OrderId,
        ProductId = e.ProductId,
        ProductName = e.ProductName,
        PlannedQty = e.PlannedQty,
        CompletedQty = e.CompletedQty,
        Status = Enum.TryParse<ProcessStatus>(e.Status, true, out var s) ? s : ProcessStatus.Created,
        Priority = Enum.TryParse<WorkOrderPriority>(e.Priority, true, out var p) ? p : WorkOrderPriority.Normal,
        CreatedAt = e.CreatedAt,
        RouteId = e.RouteId,
        RouteName = e.RouteName,
        DieName = e.DieName ?? string.Empty,
        PackageType = Enum.TryParse<PackageType>(e.PackageType, true, out var pt) ? pt : PackageType.QFP,
        WaferQty = e.WaferQty,
        UnitQty = e.UnitQty,
        CustomerId = e.CustomerId ?? string.Empty,
        CustomerName = e.CustomerName ?? string.Empty,
        CustomerPN = e.CustomerPn ?? string.Empty,
        InternalPN = e.InternalPn ?? string.Empty,
        Creator = e.Creator,
        PlannedStartDate = e.PlannedStartDate,
        PlannedEndDate = e.PlannedEndDate,
        ActualStartDate = e.ActualStartDate,
        ActualEndDate = e.ActualEndDate,
        TargetCPYield = (double)(e.TargetCpYield ?? 99.0m),
        TargetFTYield = (double)(e.TargetFtYield ?? 98.0m),
        YieldTarget = (double)(e.TargetFtYield ?? 98.0m),
        Remark = e.Remark,
    };

    private static ProdWorkOrder MapToEntity(WorkOrderInfo m) => new()
    {
        OrderId = m.OrderId,
        ProductId = m.ProductId,
        ProductName = m.ProductName,
        RouteId = m.RouteId,
        RouteName = m.RouteName,
        DieName = m.DieName,
        PackageType = m.PackageType.ToString(),
        PlannedQty = m.PlannedQty,
        CompletedQty = m.CompletedQty,
        WaferQty = m.WaferQty,
        UnitQty = m.UnitQty,
        CustomerId = m.CustomerId,
        CustomerName = m.CustomerName,
        CustomerPn = m.CustomerPN,
        InternalPn = m.InternalPN,
        Priority = m.Priority.ToString(),
        Status = m.Status.ToString(),
        Creator = m.Creator,
        PlannedStartDate = m.PlannedStartDate,
        PlannedEndDate = m.PlannedEndDate,
        ActualStartDate = m.ActualStartDate,
        ActualEndDate = m.ActualEndDate,
        TargetCpYield = (decimal)m.TargetCPYield,
        TargetFtYield = (decimal)m.TargetFTYield,
        Remark = m.Remark,
        UpdatedAt = DateTime.UtcNow,
    };
}
