using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Quality;

public class QualityAlertService : IQualityAlertService
{
    private readonly MesDbContext _context;

    public QualityAlertService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<QualityAlertResponse> CreateAlertAsync(CreateQualityAlertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AlertType))
            throw new ApplicationException("预警类型不能为空");
        if (string.IsNullOrWhiteSpace(request.Severity))
            throw new ApplicationException("严重程度不能为空");
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ApplicationException("标题不能为空");
        if (string.IsNullOrWhiteSpace(request.Description))
            throw new ApplicationException("描述不能为空");

        var alertId = GenerateId("QA");
        var now = DateTime.UtcNow;

        var alert = new QualityAlert
        {
            AlertId = alertId,
            AlertType = request.AlertType,
            Severity = request.Severity,
            Title = request.Title,
            Description = request.Description,
            ProductId = request.SourceMaterialId,
            LotId = request.SourceLotId,
            SupplierId = request.SourceBatchId,
            Status = "Active",
            IssuedBy = request.IssuedBy,
            IssuedByName = request.IssuedByName,
            IssuedTime = now,
            Remark = request.Remark,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _context.Set<QualityAlert>().AddAsync(alert);

        // Auto-link source lot if provided
        if (!string.IsNullOrEmpty(request.SourceLotId))
        {
            var lot = await _context.Set<ProdLot>().FirstOrDefaultAsync(l => l.LotId == request.SourceLotId);
            if (lot != null)
            {
                var affectedLot = new QualityAlertAffectedLot
                {
                    AlertId = alertId,
                    LotId = request.SourceLotId,
                    OrderId = lot.OrderId,
                    ProductId = lot.ProductId,
                    AffectedQty = lot.UnitCount,
                    Status = "Pending",
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await _context.Set<QualityAlertAffectedLot>().AddAsync(affectedLot);

                // Freeze the lot
                lot.Status = "QualityHold";
                lot.UpdatedAt = now;
            }
        }

        await _context.SaveChangesAsync();

        return MapToResponse(alert);
    }

    public async Task<PagedResult<QualityAlertResponse>> GetAlertsAsync(QualityAlertQuery query)
    {
        var iq = _context.Set<QualityAlert>().AsQueryable();

        if (!string.IsNullOrEmpty(query.AlertType))
            iq = iq.Where(a => a.AlertType == query.AlertType);
        if (!string.IsNullOrEmpty(query.Severity))
            iq = iq.Where(a => a.Severity == query.Severity);
        if (!string.IsNullOrEmpty(query.Status))
            iq = iq.Where(a => a.Status == query.Status);
        if (query.DateFrom.HasValue)
            iq = iq.Where(a => a.CreatedAt >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(a => a.CreatedAt <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var items = await iq
            .OrderByDescending(a => a.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var responses = items.Select(MapToResponse).ToList();

        return new PagedResult<QualityAlertResponse>
        {
            Items = responses,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<QualityAlertResponse> GetDetailAsync(string alertId)
    {
        var alert = await _context.Set<QualityAlert>().FindAsync(alertId);
        if (alert == null)
            throw new ApplicationException($"质量预警 {alertId} 不存在");

        var affectedLots = await _context.Set<QualityAlertAffectedLot>()
            .Where(l => l.AlertId == alertId)
            .ToListAsync();

        var response = MapToResponse(alert);
        response.AffectedLotsCount = affectedLots.Count;
        response.FrozenLotsCount = affectedLots.Count(l => l.Status == "Frozen");

        return response;
    }

    public async Task<List<AffectedLotInfo>> AnalyzeAffectedLotsAsync(string alertId)
    {
        var alert = await _context.Set<QualityAlert>().FindAsync(alertId);
        if (alert == null)
            throw new ApplicationException($"质量预警 {alertId} 不存在");

        var result = new List<AffectedLotInfo>();

        // Find lots by product trace
        if (!string.IsNullOrEmpty(alert.ProductId))
        {
            var lots = await _context.Set<ProdLot>()
                .Where(l => l.ProductId == alert.ProductId && l.Status != "Completed" && l.Status != "Archived")
                .ToListAsync();

            foreach (var lot in lots)
            {
                result.Add(new AffectedLotInfo
                {
                    LotId = lot.LotId,
                    WorkOrderId = lot.OrderId ?? string.Empty,
                    ProductId = lot.ProductId,
                    ProductName = lot.ProductName ?? string.Empty,
                    CurrentStatus = lot.Status ?? "Unknown",
                    CurrentStep = string.Empty,
                    AffectedQty = lot.UnitCount,
                    Location = DetermineLocation(lot.Status),
                    TraceRelation = "SameProduct"
                });
            }
        }

        // Find lots by source lot (parent-child trace)
        if (!string.IsNullOrEmpty(alert.LotId))
        {
            var relatedLots = await _context.Set<ProdLot>()
                .Where(l => l.MotherLotId == alert.LotId)
                .ToListAsync();

            foreach (var lot in relatedLots)
            {
                result.Add(new AffectedLotInfo
                {
                    LotId = lot.LotId,
                    WorkOrderId = lot.OrderId ?? string.Empty,
                    ProductId = lot.ProductId,
                    ProductName = lot.ProductName ?? string.Empty,
                    CurrentStatus = lot.Status ?? "Unknown",
                    CurrentStep = string.Empty,
                    AffectedQty = lot.UnitCount,
                    Location = DetermineLocation(lot.Status),
                    TraceRelation = "ChildOfSourceLot"
                });
            }
        }

        return result;
    }

    public async Task<bool> FreezeLotsAsync(string alertId, List<string> lotIds, string operatorId)
    {
        if (lotIds == null || lotIds.Count == 0)
            throw new ApplicationException("批次列表不能为空");

        var alert = await _context.Set<QualityAlert>().FindAsync(alertId);
        if (alert == null)
            throw new ApplicationException($"质量预警 {alertId} 不存在");

        var now = DateTime.UtcNow;

        foreach (var lotId in lotIds)
        {
            var lot = await _context.Set<ProdLot>().FindAsync(lotId);
            if (lot != null)
            {
                lot.Status = "QualityHold";
                lot.UpdatedAt = now;
            }

            // Update or create affected lot record
            var existing = await _context.Set<QualityAlertAffectedLot>()
                .FirstOrDefaultAsync(l => l.AlertId == alertId && l.LotId == lotId);

            if (existing == null)
            {
                var affectedLot = new QualityAlertAffectedLot
                {
                    AlertId = alertId,
                    LotId = lotId,
                    OrderId = lot?.OrderId,
                    ProductId = lot?.ProductId,
                    AffectedQty = lot?.UnitCount ?? 0,
                    Disposition = "Frozen",
                    Status = "Frozen",
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await _context.Set<QualityAlertAffectedLot>().AddAsync(affectedLot);
            }
            else
            {
                existing.Disposition = "Frozen";
                existing.Status = "Frozen";
                existing.UpdatedAt = now;
            }
        }

        // Update alert status if investigating
        if (alert.Status == "Active")
        {
            alert.Status = "Investigating";
            alert.UpdatedAt = now;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<RecallNoticeResponse> GenerateRecallAsync(string alertId, string issuedBy, string issuedByName)
    {
        var alert = await _context.Set<QualityAlert>().FindAsync(alertId);
        if (alert == null)
            throw new ApplicationException($"质量预警 {alertId} 不存在");

        var now = DateTime.UtcNow;
        var recallId = GenerateId("RC");
        var noticeNumber = $"RN-{now:yyyyMMdd}-{new Random().Next(1, 9999):D4}";

        // Get all affected lots
        var affectedLots = await _context.Set<QualityAlertAffectedLot>()
            .Where(l => l.AlertId == alertId)
            .ToListAsync();

        var recall = new RecallNotice
        {
            RecallId = recallId,
            NoticeNumber = noticeNumber,
            RecallType = "QualityAlert",
            Severity = alert.Severity,
            Reason = alert.Title,
            ProductId = alert.ProductId,
            ProductName = alert.ProductName,
            IssuedBy = issuedBy,
            IssuedByName = issuedByName,
            IssuedTime = now,
            TotalAffectedQty = affectedLots.Sum(l => l.AffectedQty ?? 0),
            Status = "Open",
            CreatedAt = now,
            UpdatedAt = now
        };

        await _context.Set<RecallNotice>().AddAsync(recall);

        var items = new List<RecallItem>();
        foreach (var lot in affectedLots)
        {
            var prodLot = await _context.Set<ProdLot>().FindAsync(lot.LotId);
            var location = DetermineLocation(prodLot?.Status);

            var item = new RecallNoticeItem
            {
                RecallId = recallId,
                LotId = lot.LotId,
                ProductId = lot.ProductId,
                AffectedQty = lot.AffectedQty ?? 0,
                Disposition = lot.Disposition ?? "Recall",
                Status = "Pending",
                CreatedAt = now,
                UpdatedAt = now
            };
            await _context.Set<RecallNoticeItem>().AddAsync(item);

            items.Add(new RecallItem
            {
                LotId = lot.LotId,
                ProductId = lot.ProductId ?? string.Empty,
                AffectedQty = lot.AffectedQty ?? 0,
                CurrentLocation = location,
                RecallStatus = "Pending"
            });
        }

        await _context.SaveChangesAsync();

        return new RecallNoticeResponse
        {
            RecallId = recall.RecallId,
            AlertId = alertId,
            GeneratedAt = now,
            Items = items,
            TotalAffectedQty = recall.TotalAffectedQty
        };
    }

    public async Task<bool> CloseAlertAsync(CloseQualityAlertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AlertId))
            throw new ApplicationException("预警ID不能为空");

        var alert = await _context.Set<QualityAlert>().FindAsync(request.AlertId);
        if (alert == null)
            throw new ApplicationException($"质量预警 {request.AlertId} 不存在");

        var now = DateTime.UtcNow;

        alert.Status = "Closed";
        alert.ClosedBy = request.ClosedBy;
        alert.ClosedTime = now;
        alert.Remark = request.CloseReason;
        alert.UpdatedAt = now;

        await _context.SaveChangesAsync();
        return true;
    }

    private static QualityAlertResponse MapToResponse(QualityAlert alert)
    {
        return new QualityAlertResponse
        {
            AlertId = alert.AlertId,
            AlertType = alert.AlertType,
            Severity = alert.Severity,
            Title = alert.Title,
            Status = alert.Status,
            IssuedAt = alert.IssuedTime ?? alert.CreatedAt,
            ClosedAt = alert.ClosedTime
        };
    }

    private static string DetermineLocation(string? status)
    {
        return status switch
        {
            "InWarehouse" or "InStock" => "Warehouse",
            "Shipped" or "Delivered" => "InTransit",
            "Completed" or "Archived" => "CustomerSite",
            _ => "WIP"
        };
    }

    private static string GenerateId(string prefix)
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 9999).ToString("D4");
        return $"{prefix}-{now:yyyyMMdd}-{seq}";
    }
}
