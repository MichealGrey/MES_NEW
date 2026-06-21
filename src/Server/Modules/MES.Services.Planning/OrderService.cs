using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase2;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Planning;

public class OrderService : IOrderService
{
    private readonly MesDbContext _context;
    private readonly ILogger<OrderService> _logger;

    public OrderService(MesDbContext context, ILogger<OrderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var orderId = $"SO-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var order = new SalesOrder
        {
            OrderId = orderId,
            CustomerId = request.CustomerId,
            CustomerName = request.CustomerName,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            ProductSpec = request.ProductSpec,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            TotalAmount = request.UnitPrice.HasValue ? request.UnitPrice.Value * request.Quantity : null,
            DeliveryDate = request.DeliveryDate,
            Priority = request.Priority,
            PackageType = request.PackageType,
            LeadFrameType = request.LeadFrameType,
            WireType = request.WireType,
            SpecialRequirements = request.SpecialRequirements,
            QualityLevel = request.QualityLevel,
            Remark = request.Remark,
            CreatedBy = operatorId,
            OrderDate = now,
            Status = "Draft",
            ReviewStatus = "NotStarted"
        };

        _context.SalesOrders.Add(order);
        await _context.SaveChangesAsync();

        return MapToResponse(order);
    }

    public async Task<PagedResult<OrderResponse>> GetOrdersAsync(OrderQuery query)
    {
        var iqQuery = _context.SalesOrders.Where(o => !o.Deleted).AsQueryable();

        if (!string.IsNullOrEmpty(query.CustomerId))
            iqQuery = iqQuery.Where(o => o.CustomerId == query.CustomerId);
        if (!string.IsNullOrEmpty(query.Status))
            iqQuery = iqQuery.Where(o => o.Status == query.Status);
        if (!string.IsNullOrEmpty(query.ReviewStatus))
            iqQuery = iqQuery.Where(o => o.ReviewStatus == query.ReviewStatus);
        if (!string.IsNullOrEmpty(query.Priority))
            iqQuery = iqQuery.Where(o => o.Priority == query.Priority);
        if (query.StartDate.HasValue)
            iqQuery = iqQuery.Where(o => o.OrderDate >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            iqQuery = iqQuery.Where(o => o.OrderDate <= query.EndDate.Value);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(o => o.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(o => MapToResponse(o))
            .ToListAsync();

        return new PagedResult<OrderResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<OrderResponse> GetOrderAsync(string orderId)
    {
        var order = await _context.SalesOrders
            .FirstOrDefaultAsync(o => o.OrderId == orderId && !o.Deleted);

        if (order == null)
            throw new KeyNotFoundException($"Order {orderId} not found");

        return MapToResponse(order);
    }

    public async Task<OrderReviewResponse> StartReviewAsync(string orderId, StartOrderReviewRequest request, string operatorId)
    {
        var order = await _context.SalesOrders.FirstOrDefaultAsync(o => o.OrderId == orderId && !o.Deleted);
        if (order == null)
            throw new KeyNotFoundException($"Order {orderId} not found");

        var now = DateTime.UtcNow;
        var reviewId = $"REV-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var review = new OrderReview
        {
            ReviewId = reviewId,
            OrderId = orderId,
            ReviewType = request.ReviewType,
            Status = "InProgress",
            StartTime = now,
            Deadline = request.Deadline,
            InitiatedBy = operatorId
        };

        _context.OrderReviews.Add(review);

        // Create review items for each role
        var roles = new[] { "Sales", "Engineering", "Quality", "Production", "Purchasing" };
        foreach (var role in roles)
        {
            var item = new OrderReviewItem
            {
                ReviewId = reviewId,
                ReviewerRole = role,
                Status = "Pending"
            };
            _context.OrderReviewItems.Add(item);
        }

        order.ReviewStatus = "Reviewing";
        order.Status = "PendingReview";
        order.UpdatedBy = operatorId;

        await _context.SaveChangesAsync();

        return await GetReviewWithItemsAsync(reviewId);
    }

    public async Task<OrderReviewResponse> VoteReviewAsync(string reviewId, string role, VoteReviewRequest request, string operatorId)
    {
        var review = await _context.OrderReviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        if (review == null)
            throw new KeyNotFoundException($"Review {reviewId} not found");

        var reviewItem = await _context.OrderReviewItems
            .FirstOrDefaultAsync(i => i.ReviewId == reviewId && i.ReviewerRole == role);

        if (reviewItem == null)
            throw new KeyNotFoundException($"Review item for role {role} not found");

        var now = DateTime.UtcNow;
        reviewItem.Vote = request.Vote;
        reviewItem.Comments = request.Comments;
        reviewItem.Conditions = request.Conditions;
        reviewItem.ReviewTime = now;
        reviewItem.Status = "Reviewed";

        await _context.SaveChangesAsync();

        return await GetReviewWithItemsAsync(reviewId);
    }

    public async Task<OrderReviewResponse> GetReviewStatusAsync(string orderId)
    {
        var review = await _context.OrderReviews
            .FirstOrDefaultAsync(r => r.OrderId == orderId);

        if (review == null)
            throw new KeyNotFoundException($"No review found for order {orderId}");

        return await GetReviewWithItemsAsync(review.ReviewId);
    }

    public async Task<OrderResponse> CompleteReviewAsync(string reviewId, string operatorId)
    {
        var review = await _context.OrderReviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        if (review == null)
            throw new KeyNotFoundException($"Review {reviewId} not found");

        var items = await _context.OrderReviewItems.Where(i => i.ReviewId == reviewId).ToListAsync();

        var allReviewed = items.All(i => i.Status == "Reviewed");
        if (!allReviewed)
            throw new InvalidOperationException("Not all reviewers have voted");

        var hasReject = items.Any(i => i.Vote == "Reject");
        var allApprove = items.All(i => i.Vote == "Approve");
        var hasConditional = items.Any(i => i.Vote == "Conditional");

        var now = DateTime.UtcNow;
        review.EndTime = now;

        var order = await _context.SalesOrders.FirstOrDefaultAsync(o => o.OrderId == review.OrderId && !o.Deleted);
        if (order == null)
            throw new KeyNotFoundException($"Order {review.OrderId} not found");

        if (hasReject)
        {
            review.Status = "Rejected";
            review.Conclusion = "订单评审未通过";
            order.Status = "Rejected";
            order.ReviewStatus = "Rejected";
        }
        else if (allApprove)
        {
            review.Status = "Passed";
            review.Conclusion = "订单评审全部通过";
            order.Status = "Approved";
            order.ReviewStatus = "Passed";
        }
        else
        {
            review.Status = "ConditionalPassed";
            review.Conclusion = "订单评审有条件通过";
            review.Conditions = string.Join("; ", items.Where(i => i.Vote == "Conditional").Select(i => i.Conditions));
            order.Status = "Approved";
            order.ReviewStatus = "ConditionalPassed";
        }

        order.UpdatedBy = operatorId;
        await _context.SaveChangesAsync();

        return MapToResponse(order);
    }

    public async Task<PagedResult<OrderVersionResponse>> GetOrderVersionsAsync(string orderId, int pageIndex, int pageSize)
    {
        var totalCount = await _context.OrderVersions.CountAsync(v => v.OrderId == orderId);

        var items = await _context.OrderVersions
            .Where(v => v.OrderId == orderId)
            .OrderByDescending(v => v.VersionNo)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new OrderVersionResponse
            {
                VersionId = v.VersionId,
                OrderId = v.OrderId,
                VersionNo = v.VersionNo,
                ChangeType = v.ChangeType,
                ChangeReason = v.ChangeReason,
                ChangeDescription = v.ChangeDescription,
                ChangedBy = v.ChangedBy,
                ChangedAt = v.ChangedAt,
                ApprovedBy = v.ApprovedBy
            })
            .ToListAsync();

        return new PagedResult<OrderVersionResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    private async Task<OrderReviewResponse> GetReviewWithItemsAsync(string reviewId)
    {
        var review = await _context.OrderReviews
            .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

        if (review == null)
            throw new KeyNotFoundException($"Review {reviewId} not found");

        var items = await _context.OrderReviewItems
            .Where(i => i.ReviewId == reviewId)
            .OrderBy(i => i.ItemId)
            .Select(i => new OrderReviewItemResponse
            {
                ItemId = i.ItemId,
                ReviewId = i.ReviewId,
                ReviewerRole = i.ReviewerRole,
                ReviewerName = i.ReviewerName,
                Vote = i.Vote,
                Comments = i.Comments,
                Conditions = i.Conditions,
                ReviewTime = i.ReviewTime,
                Status = i.Status
            })
            .ToListAsync();

        return new OrderReviewResponse
        {
            ReviewId = review.ReviewId,
            OrderId = review.OrderId,
            ReviewType = review.ReviewType,
            Status = review.Status,
            StartTime = review.StartTime,
            EndTime = review.EndTime,
            Deadline = review.Deadline,
            InitiatedBy = review.InitiatedBy,
            Conclusion = review.Conclusion,
            Conditions = review.Conditions,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt,
            Items = items
        };
    }

    private static OrderResponse MapToResponse(SalesOrder order) => new()
    {
        OrderId = order.OrderId,
        CustomerId = order.CustomerId,
        CustomerName = order.CustomerName,
        ProductId = order.ProductId,
        ProductName = order.ProductName,
        ProductSpec = order.ProductSpec,
        Quantity = order.Quantity,
        UnitPrice = order.UnitPrice,
        TotalAmount = order.TotalAmount,
        Currency = order.Currency,
        OrderDate = order.OrderDate,
        DeliveryDate = order.DeliveryDate,
        Priority = order.Priority,
        Status = order.Status,
        ReviewStatus = order.ReviewStatus,
        ReviewResult = order.ReviewResult,
        PackageType = order.PackageType,
        LeadFrameType = order.LeadFrameType,
        WireType = order.WireType,
        SpecialRequirements = order.SpecialRequirements,
        QualityLevel = order.QualityLevel,
        Remark = order.Remark,
        CreatedBy = order.CreatedBy,
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt
    };
}
