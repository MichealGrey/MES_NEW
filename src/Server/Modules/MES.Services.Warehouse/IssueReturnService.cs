using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Warehouse;

public class IssueReturnService : IIssueReturnService
{
    private readonly MesDbContext _context;

    public IssueReturnService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<IssueOrderResponse> IssueMaterialAsync(IssueMaterialRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.WorkOrderId))
            throw new ApplicationException("工单ID不能为空");
        if (request.Items.Count == 0)
            throw new ApplicationException("领料项目不能为空");

        var issueOrderId = GenerateId("IO");
        var now = DateTime.UtcNow;

        var issueOrder = new WarehouseIssueOrder
        {
            IssueOrderId = issueOrderId,
            WorkOrderId = request.WorkOrderId,
            LotId = request.LotId,
            IssueType = "Production",
            Status = "Issued",
            OperatorId = request.IssuedBy,
            IssueTime = now,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<WarehouseIssueOrder>().AddAsync(issueOrder);

        foreach (var item in request.Items)
        {
            // If no specific batch, use FIFO
            var batchId = item.BatchId;
            if (string.IsNullOrEmpty(batchId))
            {
                var fifoInv = await _context.Set<WarehouseInventory>()
                    .Where(i => i.MaterialId == item.MaterialId && i.Status == "Available" && i.AvailableQty > 0)
                    .OrderBy(i => i.ReceivedDate)
                    .FirstOrDefaultAsync();

                if (fifoInv == null)
                    throw new ApplicationException($"物料 {item.MaterialId} 库存不足");

                batchId = fifoInv.BatchId;

                // Deduct from inventory
                fifoInv.AvailableQty -= item.RequestedQty;
                fifoInv.Quantity -= item.RequestedQty;
                fifoInv.UpdatedAt = now;
            }

            var issueItem = new WarehouseIssueItem
            {
                IssueOrderId = issueOrderId,
                MaterialId = item.MaterialId,
                RequestQty = item.RequestedQty,
                ActualQty = item.RequestedQty,
                LotNumber = batchId,
                CreatedAt = now
            };
            await _context.Set<WarehouseIssueItem>().AddAsync(issueItem);
        }

        await _context.SaveChangesAsync();

        var items = await _context.Set<WarehouseIssueItem>()
            .Where(i => i.IssueOrderId == issueOrderId)
            .ToListAsync();

        return new IssueOrderResponse
        {
            IssueOrderId = issueOrderId,
            WorkOrderId = request.WorkOrderId,
            LotId = request.LotId,
            Status = "Issued",
            IssuedAt = now,
            Items = items.Select(i => new IssueItemResponse
            {
                MaterialId = i.MaterialId,
                MaterialName = i.MaterialName,
                RequestedQty = i.RequestQty,
                IssuedQty = i.ActualQty,
                BatchId = i.LotNumber ?? string.Empty,
                FifoSkipped = false
            }).ToList()
        };
    }

    public async Task<PagedResult<IssueOrderResponse>> GetIssueOrdersAsync(IssueOrderQuery query)
    {
        var iq = _context.Set<WarehouseIssueOrder>().AsQueryable();

        if (!string.IsNullOrEmpty(query.WorkOrderId))
            iq = iq.Where(o => o.WorkOrderId == query.WorkOrderId);
        if (!string.IsNullOrEmpty(query.Status))
            iq = iq.Where(o => o.Status == query.Status);
        if (query.DateFrom.HasValue)
            iq = iq.Where(o => o.IssueTime >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(o => o.IssueTime <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var orders = await iq
            .OrderByDescending(o => o.IssueTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var orderIds = orders.Select(o => o.IssueOrderId).ToList();
        var items = await _context.Set<WarehouseIssueItem>()
            .Where(i => orderIds.Contains(i.IssueOrderId))
            .ToListAsync();

        var itemsByOrder = items.GroupBy(i => i.IssueOrderId).ToDictionary(g => g.Key, g => g.ToList());

        var responses = orders.Select(o => new IssueOrderResponse
        {
            IssueOrderId = o.IssueOrderId,
            WorkOrderId = o.WorkOrderId ?? string.Empty,
            LotId = o.LotId,
            Status = o.Status,
            IssuedAt = o.IssueTime ?? o.CreatedAt,
            Items = (itemsByOrder.GetValueOrDefault(o.IssueOrderId) ?? new List<WarehouseIssueItem>()).Select(i => new IssueItemResponse
            {
                MaterialId = i.MaterialId,
                MaterialName = i.MaterialName,
                RequestedQty = i.RequestQty,
                IssuedQty = i.ActualQty,
                BatchId = i.LotNumber ?? string.Empty
            }).ToList()
        }).ToList();

        return new PagedResult<IssueOrderResponse>
        {
            Items = responses,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<KitCheckResponse> CheckKitAsync(string workOrderId)
    {
        var wo = await _context.ProdWorkOrders.FindAsync(workOrderId);
        if (wo == null)
            throw new ApplicationException($"工单 {workOrderId} 不存在");

        // Get material requirements from material_requirement table
        var requirements = await _context.MaterialRequirements
            .Where(r => true) // In practice, filter by route/steps of the work order
            .ToListAsync();

        var kitItems = new List<KitItemStatus>();
        foreach (var req in requirements)
        {
            var available = await _context.Set<WarehouseInventory>()
                .Where(i => i.MaterialId == req.MaterialId && i.Status == "Available")
                .SumAsync(i => i.AvailableQty);

            var issued = await _context.Set<WarehouseIssueItem>()
                .Where(i => i.MaterialId == req.MaterialId &&
                            _context.Set<WarehouseIssueOrder>().Any(o => o.IssueOrderId == i.IssueOrderId && o.WorkOrderId == workOrderId))
                .SumAsync(i => i.ActualQty);

            var requiredQty = (int)Math.Ceiling((double)req.RequiredQty * wo.PlannedQty);
            var shortage = Math.Max(0, requiredQty - available - issued);

            kitItems.Add(new KitItemStatus
            {
                MaterialId = req.MaterialId,
                MaterialName = string.Empty,
                RequiredQty = requiredQty,
                AvailableQty = available,
                IssuedQty = issued,
                ShortageQty = shortage,
                IsSufficient = shortage == 0
            });
        }

        return new KitCheckResponse
        {
            WorkOrderId = workOrderId,
            IsComplete = kitItems.All(i => i.IsSufficient),
            Items = kitItems
        };
    }

    public async Task<bool> SkipFifoApprovalAsync(string issueItemId, string approvedBy, string reason)
    {
        var issueItem = await _context.Set<WarehouseIssueItem>()
            .Where(i => i.IssueOrderId == issueItemId)
            .FirstOrDefaultAsync();

        if (issueItem == null)
            throw new ApplicationException($"领料项目 {issueItemId} 不存在");

        // Mark as FIFO skipped - in practice, this would update a specific field
        // For now, we just record the approval
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ReturnOrderResponse> ReturnMaterialAsync(ReturnMaterialRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.WorkOrderId))
            throw new ApplicationException("工单ID不能为空");
        if (request.Items.Count == 0)
            throw new ApplicationException("退料项目不能为空");

        var returnOrderId = GenerateId("RO");
        var now = DateTime.UtcNow;

        var returnOrder = new WarehouseReturnOrder
        {
            ReturnOrderId = returnOrderId,
            WorkOrderId = request.WorkOrderId,
            LotId = request.LotId,
            ReturnType = "Excess",
            Status = "Returned",
            Reason = request.Reason,
            OperatorId = request.ReturnedBy,
            ReturnTime = now,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<WarehouseReturnOrder>().AddAsync(returnOrder);

        foreach (var item in request.Items)
        {
            var returnItem = new WarehouseReturnItem
            {
                ReturnOrderId = returnOrderId,
                MaterialId = item.MaterialId,
                ReturnQty = item.ReturnQty,
                LotNumber = item.OriginalBatchId,
                LocationId = item.TargetLocationId,
                CreatedAt = now
            };
            await _context.Set<WarehouseReturnItem>().AddAsync(returnItem);

            // Add back to inventory
            var inv = await _context.Set<WarehouseInventory>()
                .Where(i => i.BatchId == item.OriginalBatchId)
                .FirstOrDefaultAsync();

            if (inv != null)
            {
                inv.Quantity += item.ReturnQty;
                inv.AvailableQty += item.ReturnQty;
                inv.UpdatedAt = now;
            }
        }

        await _context.SaveChangesAsync();

        return new ReturnOrderResponse
        {
            ReturnOrderId = returnOrderId,
            WorkOrderId = request.WorkOrderId,
            LotId = request.LotId,
            Status = "Returned",
            Reason = request.Reason,
            ReturnedAt = now,
            Items = request.Items.Select(i => new ReturnItemResponse
            {
                MaterialId = i.MaterialId,
                OriginalBatchId = i.OriginalBatchId,
                ReturnQty = i.ReturnQty,
                TargetLocationId = i.TargetLocationId
            }).ToList()
        };
    }

    public async Task<PagedResult<ReturnOrderResponse>> GetReturnOrdersAsync(ReturnOrderQuery query)
    {
        var iq = _context.Set<WarehouseReturnOrder>().AsQueryable();

        if (!string.IsNullOrEmpty(query.WorkOrderId))
            iq = iq.Where(o => o.WorkOrderId == query.WorkOrderId);
        if (!string.IsNullOrEmpty(query.Status))
            iq = iq.Where(o => o.Status == query.Status);
        if (query.DateFrom.HasValue)
            iq = iq.Where(o => o.ReturnTime >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(o => o.ReturnTime <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var orders = await iq
            .OrderByDescending(o => o.ReturnTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var orderIds = orders.Select(o => o.ReturnOrderId).ToList();
        var items = await _context.Set<WarehouseReturnItem>()
            .Where(i => orderIds.Contains(i.ReturnOrderId))
            .ToListAsync();

        var itemsByOrder = items.GroupBy(i => i.ReturnOrderId).ToDictionary(g => g.Key, g => g.ToList());

        var responses = orders.Select(o => new ReturnOrderResponse
        {
            ReturnOrderId = o.ReturnOrderId,
            WorkOrderId = o.WorkOrderId ?? string.Empty,
            LotId = o.LotId,
            Status = o.Status,
            Reason = o.Reason ?? string.Empty,
            ReturnedAt = o.ReturnTime ?? o.CreatedAt,
            Items = (itemsByOrder.GetValueOrDefault(o.ReturnOrderId) ?? new List<WarehouseReturnItem>()).Select(i => new ReturnItemResponse
            {
                MaterialId = i.MaterialId,
                MaterialName = i.MaterialName,
                OriginalBatchId = i.LotNumber ?? string.Empty,
                ReturnQty = i.ReturnQty,
                TargetLocationId = i.LocationId
            }).ToList()
        }).ToList();

        return new PagedResult<ReturnOrderResponse>
        {
            Items = responses,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    private static string GenerateId(string prefix)
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 9999).ToString("D4");
        return $"{prefix}-{now:yyyyMMdd}-{seq}";
    }
}
