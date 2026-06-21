using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Warehouse;

public class WarehouseService : IWarehouseService
{
    private readonly MesDbContext _context;

    public WarehouseService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<WarehouseReceiptResponse> ReceiveMaterialAsync(WarehouseReceiptRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.BatchId))
            throw new ApplicationException("批次ID不能为空");
        if (request.Quantity <= 0)
            throw new ApplicationException("数量必须大于0");

        var receiptId = GenerateId("WR");
        var now = DateTime.UtcNow;

        var receipt = new WarehouseReceipt
        {
            ReceiptId = receiptId,
            SupplierId = request.SupplierId ?? string.Empty,
            SupplierName = string.Empty,
            MaterialId = request.MaterialId,
            MaterialName = request.MaterialName,
            Quantity = request.Quantity,
            Unit = request.Unit,
            LotNumber = request.BatchId,
            ReceivedDate = now,
            WarehouseId = request.LocationId,
            LocationId = request.LocationId,
            ReceiptType = "Material",
            IqcBatchId = request.IqcTaskId,
            Status = "Received",
            OperatorId = request.ReceivedBy,
            Remark = request.Remark,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<WarehouseReceipt>().AddAsync(receipt);

        // Create inventory record
        var inventory = new WarehouseInventory
        {
            WarehouseId = request.LocationId,
            LocationId = request.LocationId,
            MaterialId = request.MaterialId,
            MaterialName = request.MaterialName,
            Quantity = request.Quantity,
            AvailableQty = request.Quantity,
            ReservedQty = 0,
            Unit = request.Unit,
            BatchId = request.BatchId,
            SupplierId = request.SupplierId,
            ReceivedDate = now,
            ExpiryDate = request.ExpiryDate,
            Status = "Available",
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<WarehouseInventory>().AddAsync(inventory);

        await _context.SaveChangesAsync();

        return new WarehouseReceiptResponse
        {
            ReceiptId = receipt.ReceiptId,
            BatchId = request.BatchId,
            MaterialId = receipt.MaterialId,
            MaterialName = receipt.MaterialName,
            Quantity = receipt.Quantity,
            LocationId = receipt.LocationId ?? string.Empty,
            Status = receipt.Status,
            ReceivedAt = receipt.ReceivedDate ?? now
        };
    }

    public async Task<PagedResult<InventoryResponse>> GetInventoryAsync(InventoryQuery query)
    {
        var iq = _context.Set<WarehouseInventory>().AsQueryable();

        if (!string.IsNullOrEmpty(query.MaterialId))
            iq = iq.Where(i => i.MaterialId == query.MaterialId);
        if (!string.IsNullOrEmpty(query.LocationId))
            iq = iq.Where(i => i.LocationId == query.LocationId);
        if (!string.IsNullOrEmpty(query.Status))
            iq = iq.Where(i => i.Status == query.Status);

        var totalCount = await iq.CountAsync();

        var items = await iq
            .OrderByDescending(i => i.ReceivedDate)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(i => new InventoryResponse
            {
                MaterialId = i.MaterialId,
                MaterialName = i.MaterialName,
                BatchId = i.BatchId ?? string.Empty,
                LocationId = i.LocationId ?? string.Empty,
                LocationName = string.Empty,
                AvailableQty = i.AvailableQty,
                LockedQty = i.Status == "Locked" ? i.Quantity : 0,
                AllocatedQty = i.ReservedQty ?? 0,
                Unit = i.Unit ?? string.Empty,
                ExpiryDate = i.ExpiryDate,
                RemainingDays = i.ExpiryDate.HasValue ? (int)(i.ExpiryDate.Value - DateTime.UtcNow).TotalDays : null,
                Status = i.Status,
                ReceivedAt = i.ReceivedDate ?? i.CreatedAt,
                IsFifoEligible = i.Status == "Available"
            })
            .ToListAsync();

        return new PagedResult<InventoryResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<FifoRecommendResponse> GetFifoRecommendationAsync(string materialId, int requestedQty)
    {
        var inventories = await _context.Set<WarehouseInventory>()
            .Where(i => i.MaterialId == materialId && i.Status == "Available" && i.AvailableQty > 0)
            .OrderBy(i => i.ReceivedDate)
            .ToListAsync();

        var recommendations = new List<FifoBatchRecommendation>();
        var remaining = requestedQty;

        foreach (var inv in inventories)
        {
            if (remaining <= 0) break;
            var takeQty = Math.Min(inv.AvailableQty, remaining);
            recommendations.Add(new FifoBatchRecommendation
            {
                BatchId = inv.BatchId ?? string.Empty,
                LocationId = inv.LocationId ?? string.Empty,
                AvailableQty = inv.AvailableQty,
                RecommendQty = takeQty,
                ReceivedAt = inv.ReceivedDate ?? inv.CreatedAt,
                ExpiryDate = inv.ExpiryDate,
                SupplierBatchNo = inv.LotNumber ?? string.Empty
            });
            remaining -= takeQty;
        }

        return new FifoRecommendResponse
        {
            MaterialId = materialId,
            RequestedQty = requestedQty,
            Recommendations = recommendations
        };
    }

    public async Task<List<ExpiryWarningResponse>> GetExpiryWarningsAsync(int warningDays)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(warningDays);

        var expiring = await _context.Set<WarehouseInventory>()
            .Where(i => i.ExpiryDate != null && i.ExpiryDate <= cutoffDate && i.Status != "Expired")
            .OrderBy(i => i.ExpiryDate)
            .ToListAsync();

        var now = DateTime.UtcNow;
        return expiring.Select(i =>
        {
            var days = (int)((i.ExpiryDate ?? now) - now).TotalDays;
            var level = days <= 0 ? "Expired" : days <= 7 ? "Urgent" : "Warning";

            return new ExpiryWarningResponse
            {
                MaterialId = i.MaterialId,
                MaterialName = i.MaterialName,
                BatchId = i.BatchId ?? string.Empty,
                ExpiryDate = i.ExpiryDate ?? now,
                RemainingDays = days,
                Quantity = i.Quantity,
                LocationId = i.LocationId ?? string.Empty,
                WarningLevel = level
            };
        }).ToList();
    }

    public async Task<bool> LockBatchAsync(string batchId, string reason, string operatorId)
    {
        var inventory = await _context.Set<WarehouseInventory>()
            .Where(i => i.BatchId == batchId)
            .ToListAsync();

        if (inventory.Count == 0)
            throw new ApplicationException($"批次 {batchId} 不存在");

        foreach (var inv in inventory)
        {
            inv.Status = "Locked";
            inv.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnlockBatchAsync(string batchId, string operatorId)
    {
        var inventory = await _context.Set<WarehouseInventory>()
            .Where(i => i.BatchId == batchId && i.Status == "Locked")
            .ToListAsync();

        if (inventory.Count == 0)
            throw new ApplicationException($"批次 {batchId} 未锁定或不存在");

        foreach (var inv in inventory)
        {
            inv.Status = "Available";
            inv.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AdjustInventoryAsync(InventoryAdjustRequest request)
    {
        var inventory = await _context.Set<WarehouseInventory>()
            .Where(i => i.BatchId == request.BatchId && i.MaterialId == request.MaterialId)
            .FirstOrDefaultAsync();

        if (inventory == null)
            throw new ApplicationException($"库存记录不存在: 物料={request.MaterialId}, 批次={request.BatchId}");

        var newQty = inventory.Quantity + request.AdjustQty;
        if (newQty < 0)
            throw new ApplicationException($"调整后数量不能为负数 (当前: {inventory.Quantity}, 调整: {request.AdjustQty})");

        inventory.Quantity = newQty;
        inventory.AvailableQty = Math.Max(0, inventory.AvailableQty + request.AdjustQty);
        inventory.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static string GenerateId(string prefix)
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 9999).ToString("D4");
        return $"{prefix}-{now:yyyyMMdd}-{seq}";
    }
}
