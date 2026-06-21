using MES.Contracts.Common;
using MES.Contracts.Production;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Production;

public interface IWorkOrderService
{
    Task<PagedResult<WorkOrderDto>> GetPagedAsync(int pageIndex, int pageSize, string? status = null, string? customerId = null, string? keyword = null);
    Task<WorkOrderDto?> GetByIdAsync(string orderId);
    Task<WorkOrderDto> CreateAsync(CreateWorkOrderRequest request);
    Task<bool> UpdateAsync(UpdateWorkOrderRequest request);
    Task<bool> DeleteAsync(string orderId);
    Task<List<WorkOrderDto>> GetByCustomerIdAsync(string customerId);
}

public class WorkOrderService : IWorkOrderService
{
    private readonly MesDbContext _context;

    public WorkOrderService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<WorkOrderDto>> GetPagedAsync(int pageIndex, int pageSize, string? status = null, string? customerId = null, string? keyword = null)
    {
        var query = _context.ProdWorkOrders.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);

        if (!string.IsNullOrEmpty(customerId))
            query = query.Where(x => x.CustomerId == customerId);

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(x => x.ProductName.Contains(keyword) || x.OrderId.Contains(keyword) || x.CustomerName.Contains(keyword));

        query = query.OrderByDescending(x => x.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new WorkOrderDto
            {
                OrderId = x.OrderId,
                WoType = x.WoType,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                RouteId = x.RouteId,
                RouteName = x.RouteName,
                PlannedQty = x.PlannedQty,
                CompletedQty = x.CompletedQty,
                WaferQty = x.WaferQty,
                UnitQty = x.UnitQty,
                Status = x.Status,
                Priority = x.Priority,
                DieName = x.DieName,
                PackageType = x.PackageType,
                CustomerId = x.CustomerId,
                CustomerName = x.CustomerName,
                CustomerPn = x.CustomerPn,
                InternalPn = x.InternalPn,
                TargetCpYield = x.TargetCpYield.HasValue ? (double)x.TargetCpYield.Value : 0,
                TargetFtYield = x.TargetFtYield.HasValue ? (double)x.TargetFtYield.Value : 0,
                PlannedStartDate = x.PlannedStartDate,
                PlannedEndDate = x.PlannedEndDate,
                ActualStartDate = x.ActualStartDate,
                ActualEndDate = x.ActualEndDate,
                Remark = x.Remark,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync();

        return new PagedResult<WorkOrderDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<WorkOrderDto?> GetByIdAsync(string orderId)
    {
        var wo = await _context.ProdWorkOrders.FindAsync(orderId);
        if (wo == null) return null;

        return MapToDto(wo);
    }

    public async Task<WorkOrderDto> CreateAsync(CreateWorkOrderRequest request)
    {
        var product = await _context.MasterProducts.FindAsync(request.ProductId);
        var route = await _context.MasterRoutes.FindAsync(request.RouteId);

        var wo = new ProdWorkOrder
        {
            OrderId = GenerateWorkOrderId(),
            WoType = request.WoType,
            ParentOrderId = request.ParentOrderId,
            ProductId = request.ProductId,
            ProductName = product?.ProductName ?? "",
            RouteId = request.RouteId,
            RouteName = route?.RouteName ?? "",
            DieName = product?.DieName ?? "",
            PackageType = product?.PackageType ?? "",
            PlannedQty = request.PlannedQty,
            CompletedQty = 0,
            WaferQty = request.WaferQty,
            UnitQty = request.UnitQty,
            CustomerId = request.CustomerId,
            CustomerName = "",
            CustomerPn = "",
            InternalPn = "",
            Priority = request.Priority,
            Status = "Created",
            PlannedStartDate = request.PlannedStartDate,
            PlannedEndDate = request.PlannedEndDate,
            Remark = request.Remark,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (!string.IsNullOrEmpty(request.CustomerId))
        {
            var customer = await _context.MasterCustomers.FindAsync(request.CustomerId);
            wo.CustomerName = customer?.CustomerName ?? "";
            wo.CustomerPn = customer?.CustomerPnPrefix ?? "";
        }

        await _context.ProdWorkOrders.AddAsync(wo);
        await _context.SaveChangesAsync();

        return MapToDto(wo);
    }

    public async Task<bool> UpdateAsync(UpdateWorkOrderRequest request)
    {
        var wo = await _context.ProdWorkOrders.FindAsync(request.OrderId);
        if (wo == null) return false;

        if (!string.IsNullOrEmpty(request.Status)) wo.Status = request.Status;
        if (!string.IsNullOrEmpty(request.Priority)) wo.Priority = request.Priority;
        if (!string.IsNullOrEmpty(request.Remark)) wo.Remark = request.Remark;
        wo.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(string orderId)
    {
        var wo = await _context.ProdWorkOrders.FindAsync(orderId);
        if (wo == null) return false;

        _context.ProdWorkOrders.Remove(wo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<WorkOrderDto>> GetByCustomerIdAsync(string customerId)
    {
        var items = await _context.ProdWorkOrders
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return items.Select(MapToDto).ToList();
    }

    private static WorkOrderDto MapToDto(ProdWorkOrder wo) => new()
    {
        OrderId = wo.OrderId,
        WoType = wo.WoType,
        ProductId = wo.ProductId,
        ProductName = wo.ProductName,
        RouteId = wo.RouteId,
        RouteName = wo.RouteName,
        PlannedQty = wo.PlannedQty,
        CompletedQty = wo.CompletedQty,
        WaferQty = wo.WaferQty,
        UnitQty = wo.UnitQty,
        Status = wo.Status,
        Priority = wo.Priority,
        DieName = wo.DieName,
        PackageType = wo.PackageType,
        CustomerId = wo.CustomerId,
        CustomerName = wo.CustomerName,
        CustomerPn = wo.CustomerPn,
        InternalPn = wo.InternalPn,
        TargetCpYield = (double)wo.TargetCpYield.GetValueOrDefault(0),
        TargetFtYield = (double)wo.TargetFtYield.GetValueOrDefault(0),
        PlannedStartDate = wo.PlannedStartDate,
        PlannedEndDate = wo.PlannedEndDate,
        ActualStartDate = wo.ActualStartDate,
        ActualEndDate = wo.ActualEndDate,
        Remark = wo.Remark,
        CreatedAt = wo.CreatedAt,
        UpdatedAt = wo.UpdatedAt
    };

    private static string GenerateWorkOrderId()
    {
        var now = DateTime.UtcNow;
        var dateStr = now.ToString("yyyy-MM-dd");
        var seq = new Random().Next(1, 999).ToString("D3");
        return $"WO-{dateStr}-{seq}";
    }
}
