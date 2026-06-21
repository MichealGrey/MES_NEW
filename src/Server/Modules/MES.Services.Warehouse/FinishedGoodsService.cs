using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Warehouse;

public class FinishedGoodsService : IFinishedGoodsService
{
    private readonly MesDbContext _context;

    public FinishedGoodsService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<WarehouseReceiptResponse> ReceiveFinishedGoodsAsync(FinishedGoodsReceiptRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LotId))
            throw new ApplicationException("批次ID不能为空");
        if (request.Quantity <= 0)
            throw new ApplicationException("数量必须大于0");

        var receiptId = GenerateId("FG");
        var now = DateTime.UtcNow;

        var receipt = new FinishedGoodsReceipt
        {
            ReceiptId = receiptId,
            LotId = request.LotId,
            OrderId = request.WorkOrderId,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            Unit = "EA",
            WarehouseId = request.LocationId,
            LocationId = request.LocationId,
            FqcRecordId = request.FqcRecordId,
            Status = "Received",
            OperatorId = request.ReceivedBy,
            ReceiptTime = now,
            Remark = request.Remark,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<FinishedGoodsReceipt>().AddAsync(receipt);

        // Create finished goods inventory
        var inventory = new FinishedGoodsInventory
        {
            WarehouseId = request.LocationId,
            LocationId = request.LocationId,
            LotId = request.LotId,
            OrderId = request.WorkOrderId,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            AvailableQty = request.Quantity,
            ReservedQty = 0,
            Unit = "EA",
            ProducedDate = now,
            Status = "Available",
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<FinishedGoodsInventory>().AddAsync(inventory);

        await _context.SaveChangesAsync();

        return new WarehouseReceiptResponse
        {
            ReceiptId = receipt.ReceiptId,
            BatchId = request.LotId,
            MaterialId = receipt.ProductId,
            MaterialName = receipt.ProductName,
            Quantity = receipt.Quantity,
            LocationId = receipt.LocationId ?? string.Empty,
            Status = receipt.Status,
            ReceivedAt = receipt.ReceiptTime ?? now
        };
    }

    public async Task<PagedResult<FinishedGoodsInventoryResponse>> GetInventoryAsync(FinishedGoodsQuery query)
    {
        var iq = _context.Set<FinishedGoodsInventory>().AsQueryable();

        if (!string.IsNullOrEmpty(query.ProductId))
            iq = iq.Where(i => i.ProductId == query.ProductId);
        if (!string.IsNullOrEmpty(query.LotId))
            iq = iq.Where(i => i.LotId == query.LotId);
        if (!string.IsNullOrEmpty(query.Status))
            iq = iq.Where(i => i.Status == query.Status);
        if (query.DateFrom.HasValue)
            iq = iq.Where(i => i.ProducedDate >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(i => i.ProducedDate <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var items = await iq
            .OrderByDescending(i => i.ProducedDate)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(i => new FinishedGoodsInventoryResponse
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                LotId = i.LotId,
                BatchId = null,
                LocationId = i.LocationId ?? string.Empty,
                AvailableQty = i.AvailableQty,
                ShippedQty = i.Quantity - i.AvailableQty,
                ReceiptDate = i.ProducedDate ?? i.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<FinishedGoodsInventoryResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<ShipmentResponse> ShipFinishedGoodsAsync(ShipFinishedGoodsRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ShipmentId))
            throw new ApplicationException("出货单ID不能为空");
        if (request.Items.Count == 0)
            throw new ApplicationException("发货项目不能为空");

        var now = DateTime.UtcNow;

        var shipment = new FinishedGoodsShipment
        {
            ShipmentId = request.ShipmentId,
            Status = "Shipped",
            ShipmentTime = now,
            Carrier = request.Carrier,
            OperatorId = request.ShippedBy,
            Remark = request.Remark,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<FinishedGoodsShipment>().AddAsync(shipment);

        foreach (var item in request.Items)
        {
            // Deduct from finished goods inventory
            var inv = await _context.Set<FinishedGoodsInventory>()
                .Where(i => i.LotId == item.LotId && i.ProductId == item.ProductId && i.AvailableQty > 0)
                .OrderBy(i => i.ProducedDate)
                .FirstOrDefaultAsync();

            if (inv == null || inv.AvailableQty < item.Quantity)
                throw new ApplicationException($"批次 {item.LotId} 可用库存不足");

            inv.AvailableQty -= item.Quantity;
            inv.UpdatedAt = now;

            var shipmentItem = new FinishedGoodsShipmentItem
            {
                ShipmentId = request.ShipmentId,
                LotId = item.LotId,
                ProductId = item.ProductId,
                ProductName = inv.ProductName,
                Quantity = item.Quantity,
                Unit = inv.Unit,
                LocationId = inv.LocationId,
                WarehouseId = inv.WarehouseId,
                CreatedAt = now
            };
            await _context.Set<FinishedGoodsShipmentItem>().AddAsync(shipmentItem);
        }

        await _context.SaveChangesAsync();

        return new ShipmentResponse
        {
            ShipmentId = request.ShipmentId,
            Status = "Shipped",
            ShippedBy = request.ShippedBy,
            ShippedAt = now
        };
    }

    public async Task<PagedResult<ShipmentResponse>> GetShipmentsAsync(FinishedGoodsQuery query)
    {
        var iq = _context.Set<FinishedGoodsShipment>().AsQueryable();

        if (!string.IsNullOrEmpty(query.Status))
            iq = iq.Where(s => s.Status == query.Status);
        if (query.DateFrom.HasValue)
            iq = iq.Where(s => s.ShipmentTime >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(s => s.ShipmentTime <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var items = await iq
            .OrderByDescending(s => s.ShipmentTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(s => new ShipmentResponse
            {
                ShipmentId = s.ShipmentId,
                CustomerId = s.CustomerId,
                CustomerName = s.CustomerName,
                Status = s.Status,
                ShippedBy = s.OperatorName,
                ShippedAt = s.ShipmentTime,
                TrackingNo = s.TrackingNumber
            })
            .ToListAsync();

        return new PagedResult<ShipmentResponse>
        {
            Items = items,
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
