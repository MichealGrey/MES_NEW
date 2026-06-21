using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Quality;

public class FirstArticleService : IFirstArticleService
{
    private readonly MesDbContext _context;

    public FirstArticleService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<FirstArticleResponse> CreateAsync(CreateFirstArticleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.WorkOrderId))
            throw new ApplicationException("工单ID不能为空");
        if (string.IsNullOrWhiteSpace(request.LotId))
            throw new ApplicationException("批次ID不能为空");
        if (string.IsNullOrWhiteSpace(request.ProductId))
            throw new ApplicationException("产品ID不能为空");
        if (string.IsNullOrWhiteSpace(request.StepCode))
            throw new ApplicationException("工序代码不能为空");
        if (string.IsNullOrWhiteSpace(request.TriggerReason))
            throw new ApplicationException("触发原因不能为空");

        var faId = GenerateId("FA");
        var now = DateTime.UtcNow;

        var fa = new FirstArticleInspection
        {
            FaId = faId,
            LotId = request.LotId,
            OrderId = request.WorkOrderId,
            ProductId = request.ProductId,
            ProductName = request.ProductName,
            StepCode = request.StepCode,
            StepName = request.StepName,
            EquipmentId = request.EquipmentId,
            Status = "Pending",
            Judgment = string.Empty,
            Remark = request.Remark,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _context.Set<FirstArticleInspection>().AddAsync(fa);
        await _context.SaveChangesAsync();

        return MapToResponse(fa, triggerReason: request.TriggerReason);
    }

    public async Task<PagedResult<FirstArticleResponse>> GetFirstArticlesAsync(FirstArticleQuery query)
    {
        var iq = _context.Set<FirstArticleInspection>().AsQueryable();

        if (!string.IsNullOrEmpty(query.WorkOrderId))
            iq = iq.Where(f => f.OrderId == query.WorkOrderId);
        if (!string.IsNullOrEmpty(query.Status))
            iq = iq.Where(f => f.Status == query.Status);
        if (query.DateFrom.HasValue)
            iq = iq.Where(f => f.CreatedAt >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(f => f.CreatedAt <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var items = await iq
            .OrderByDescending(f => f.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var responses = items.Select(f => MapToResponse(f)).ToList();

        return new PagedResult<FirstArticleResponse>
        {
            Items = responses,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<FirstArticleResponse> GetDetailAsync(string faId)
    {
        var fa = await _context.Set<FirstArticleInspection>().FindAsync(faId);
        if (fa == null)
            throw new ApplicationException($"首件检验 {faId} 不存在");

        var items = await _context.Set<FirstArticleInspectionItem>()
            .Where(i => i.FaId == faId)
            .ToListAsync();

        var signatures = await _context.Set<FirstArticleSignature>()
            .Where(s => s.FaId == faId)
            .ToListAsync();

        var response = MapToResponse(fa);
        response.Items = items.Select(i => new FirstArticleItemInput
        {
            ItemCode = i.InspectionItem,
            ItemName = i.InspectionItem,
            StandardValue = i.StandardValue,
            LowerLimit = decimal.TryParse(i.LowerLimit, out var ll) ? ll : null,
            UpperLimit = decimal.TryParse(i.UpperLimit, out var ul) ? ul : null,
            ActualValue = i.ActualValue,
            Unit = i.Unit,
            Result = i.Judgment,
            Remark = i.DefectDescription
        }).ToList();
        response.Signatures = signatures.Select(s => new
        {
            s.SignerId,
            s.SignerName,
            s.SignerRole,
            s.SignTime,
            s.Comment
        }).ToList();

        return response;
    }

    public async Task<FirstArticleResponse> ExecuteAsync(ExecuteFirstArticleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FaId))
            throw new ApplicationException("首件ID不能为空");

        var fa = await _context.Set<FirstArticleInspection>().FindAsync(request.FaId);
        if (fa == null)
            throw new ApplicationException($"首件检验 {request.FaId} 不存在");
        if (fa.Status != "Pending" && fa.Status != "Inspecting")
            throw new ApplicationException($"首件状态为 {fa.Status}，无法执行检验");

        var now = DateTime.UtcNow;

        // Add inspection items
        foreach (var item in request.Items)
        {
            var faItem = new FirstArticleInspectionItem
            {
                FaId = request.FaId,
                InspectionItem = item.ItemName,
                StandardValue = item.StandardValue,
                UpperLimit = item.UpperLimit?.ToString(),
                LowerLimit = item.LowerLimit?.ToString(),
                ActualValue = item.ActualValue,
                Unit = item.Unit,
                Judgment = item.Result,
                DefectDescription = item.Result == "Fail" ? item.Remark : null,
                CreatedAt = now
            };
            await _context.Set<FirstArticleInspectionItem>().AddAsync(faItem);
        }

        // Add technician signature
        var signature = new FirstArticleSignature
        {
            SignatureId = GenerateId("FAS"),
            FaId = request.FaId,
            SignerId = request.TechnicianId,
            SignerName = request.TechnicianName,
            SignerRole = "Technician",
            SignatureType = "TechnicianConfirm",
            SignTime = now,
            Comment = request.Remark,
            CreatedAt = now
        };
        await _context.Set<FirstArticleSignature>().AddAsync(signature);

        // Update FA status
        fa.Status = "TechnicianConfirmed";
        fa.InspectorId = request.TechnicianId;
        fa.InspectorName = request.TechnicianName;
        fa.InspectionTime = now;
        fa.Judgment = DetermineJudgment(request.Items);
        fa.UpdatedAt = now;

        await _context.SaveChangesAsync();

        return MapToResponse(fa);
    }

    public async Task<FirstArticleResponse> ConfirmAsync(ConfirmFirstArticleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FaId))
            throw new ApplicationException("首件ID不能为空");
        if (string.IsNullOrWhiteSpace(request.ConfirmerId))
            throw new ApplicationException("确认人ID不能为空");
        if (string.IsNullOrWhiteSpace(request.ConfirmerRole))
            throw new ApplicationException("确认人角色不能为空");

        var validConfirmations = new[] { "Approve", "Reject" };
        if (!validConfirmations.Contains(request.Confirmation))
            throw new ApplicationException($"无效的确认结果: {request.Confirmation}");

        var fa = await _context.Set<FirstArticleInspection>().FindAsync(request.FaId);
        if (fa == null)
            throw new ApplicationException($"首件检验 {request.FaId} 不存在");

        var now = DateTime.UtcNow;

        // Add confirmation signature
        var signature = new FirstArticleSignature
        {
            SignatureId = GenerateId("FAS"),
            FaId = request.FaId,
            SignerId = request.ConfirmerId,
            SignerName = request.ConfirmerId,
            SignerRole = request.ConfirmerRole,
            SignatureType = $"{request.ConfirmerRole}Confirm",
            SignTime = now,
            Comment = request.Comment,
            CreatedAt = now
        };
        await _context.Set<FirstArticleSignature>().AddAsync(signature);

        if (request.ConfirmerRole == "Technician")
        {
            fa.Status = request.Confirmation == "Approve" ? "TechnicianConfirmed" : "Rejected";
        }
        else if (request.ConfirmerRole == "IPQC")
        {
            fa.Status = request.Confirmation == "Approve" ? "Approved" : "Rejected";
            fa.Judgment = request.Confirmation == "Approve" ? "Pass" : "Fail";
        }

        fa.UpdatedAt = now;
        await _context.SaveChangesAsync();

        return MapToResponse(fa);
    }

    public async Task<bool> RejectAsync(RejectFirstArticleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FaId))
            throw new ApplicationException("首件ID不能为空");
        if (string.IsNullOrWhiteSpace(request.RejectionReason))
            throw new ApplicationException("驳回原因不能为空");

        var fa = await _context.Set<FirstArticleInspection>().FindAsync(request.FaId);
        if (fa == null)
            throw new ApplicationException($"首件检验 {request.FaId} 不存在");

        var now = DateTime.UtcNow;

        // Add rejection signature
        var signature = new FirstArticleSignature
        {
            SignatureId = GenerateId("FAS"),
            FaId = request.FaId,
            SignerId = request.RejectedBy,
            SignerName = request.RejectedBy,
            SignerRole = "Rejector",
            SignatureType = "Reject",
            SignTime = now,
            Comment = request.RejectionReason,
            CreatedAt = now
        };
        await _context.Set<FirstArticleSignature>().AddAsync(signature);

        fa.Status = "Rejected";
        fa.Judgment = "Fail";
        fa.Remark = request.RejectionReason;
        fa.UpdatedAt = now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<BondPullTestResponse> RecordBondPullTestAsync(BondPullTestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LotId))
            throw new ApplicationException("批次ID不能为空");
        if (string.IsNullOrWhiteSpace(request.TestedBy))
            throw new ApplicationException("测试人不能为空");
        if (request.PullForceGrams <= 0)
            throw new ApplicationException("拉力值必须大于0");

        var now = DateTime.UtcNow;
        var testId = GenerateId("BPT");

        // Determine pass/fail
        var isPass = true;
        if (request.LowerLimitGrams.HasValue && request.PullForceGrams < request.LowerLimitGrams.Value)
            isPass = false;
        if (request.UpperLimitGrams.HasValue && request.PullForceGrams > request.UpperLimitGrams.Value)
            isPass = false;

        // Look up product info from lot
        var lot = await _context.Set<ProdLot>().FirstOrDefaultAsync(l => l.LotId == request.LotId);
        var orderId = request.WorkOrderId ?? lot?.OrderId;
        var productId = lot?.ProductId ?? string.Empty;
        var productName = lot?.ProductName;

        var test = new BondPullTestRecord
        {
            TestId = testId,
            LotId = request.LotId,
            OrderId = orderId,
            ProductId = productId,
            ProductName = productName,
            SampleSize = 1,
            LowerLimit = request.LowerLimitGrams?.ToString(),
            UpperLimit = request.UpperLimitGrams?.ToString(),
            Unit = "g",
            AvgValue = request.PullForceGrams,
            MinValue = request.PullForceGrams,
            MaxValue = request.PullForceGrams,
            Judgment = isPass ? "Pass" : "Fail",
            TesterId = request.TestedBy,
            TesterName = request.TestedBy,
            TestTime = now,
            Remark = request.Remark,
            CreatedAt = now
        };

        await _context.Set<BondPullTestRecord>().AddAsync(test);
        await _context.SaveChangesAsync();

        return new BondPullTestResponse
        {
            TestId = test.TestId,
            LotId = test.LotId,
            ProductId = test.ProductId,
            ProductName = test.ProductName,
            SampleNo = request.SampleNo,
            PullForceGrams = request.PullForceGrams,
            LowerLimitGrams = request.LowerLimitGrams,
            UpperLimitGrams = request.UpperLimitGrams,
            Result = test.Judgment,
            FailureMode = request.FailureMode,
            TestedAt = test.TestTime
        };
    }

    public async Task<List<BondPullTestResponse>> GetBondPullTestsAsync(string lotId, string? workOrderId = null)
    {
        var iq = _context.Set<BondPullTestRecord>()
            .Where(t => t.LotId == lotId);

        if (!string.IsNullOrEmpty(workOrderId))
            iq = iq.Where(t => t.OrderId == workOrderId);

        var tests = await iq
            .OrderByDescending(t => t.TestTime)
            .ToListAsync();

        return tests.Select(t => new BondPullTestResponse
        {
            TestId = t.TestId,
            LotId = t.LotId,
            ProductId = t.ProductId,
            ProductName = t.ProductName,
            SampleNo = t.SampleSize,
            PullForceGrams = t.AvgValue ?? 0,
            LowerLimitGrams = decimal.TryParse(t.LowerLimit, out var ll) ? ll : null,
            UpperLimitGrams = decimal.TryParse(t.UpperLimit, out var ul) ? ul : null,
            Result = t.Judgment,
            TestedAt = t.TestTime
        }).ToList();
    }

    private static FirstArticleResponse MapToResponse(FirstArticleInspection fa, string? triggerReason = null)
    {
        // Try to get trigger reason from remark if not provided
        var reason = triggerReason ?? fa.Remark ?? string.Empty;

        return new FirstArticleResponse
        {
            FaId = fa.FaId,
            WorkOrderId = fa.OrderId,
            LotId = fa.LotId,
            ProductId = fa.ProductId,
            ProductName = fa.ProductName,
            StepCode = fa.StepCode,
            StepName = fa.StepName ?? string.Empty,
            EquipmentId = fa.EquipmentId ?? string.Empty,
            TriggerReason = reason,
            Status = fa.Status,
            TechnicianId = fa.InspectorId,
            IpqcId = fa.Status == "Approved" ? fa.InspectorId : null,
            Result = fa.Judgment,
            CreatedAt = fa.CreatedAt
        };
    }

    private static string DetermineJudgment(List<FirstArticleItemInput> items)
    {
        if (items.Count == 0) return string.Empty;
        var failCount = items.Count(i => i.Result == "Fail");
        return failCount == 0 ? "Pass" : "Fail";
    }

    private static string GenerateId(string prefix)
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 9999).ToString("D4");
        return $"{prefix}-{now:yyyyMMdd}-{seq}";
    }
}
