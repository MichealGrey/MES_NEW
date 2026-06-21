using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Quality;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Quality;

// ============================================================================
// IQualityInspectionService 接口
// ============================================================================

public interface IQualityInspectionService
{
    /// <summary>创建检验记录（IQC/IPQC/OQC）</summary>
    Task<InspectionResponse> CreateInspectionAsync(InspectionCreateRequest request, string createdBy);

    /// <summary>查询检验记录（分页）</summary>
    Task<InspectionListResponse> GetInspectionsAsync(int pageIndex, int pageSize, string? lotId = null, string? type = null, string? result = null);

    /// <summary>获取检验详情</summary>
    Task<InspectionResponse?> GetInspectionDetailAsync(string inspectionId);

    /// <summary>创建不合格品报告（NCR）</summary>
    Task<NcrResponse> CreateNcrAsync(NcrCreateRequest request, string? discovererName = null);

    /// <summary>查询NCR（分页）</summary>
    Task<NcrListResponse> GetNcrsAsync(int pageIndex, int pageSize, string? lotId = null, string? status = null, string? severity = null);

    /// <summary>更新NCR状态</summary>
    Task<bool> UpdateNcrStatusAsync(string ncrId, string status, string? disposition, string reviewerId, string? reviewerName = null, string? closureComment = null);

    /// <summary>获取质量统计</summary>
    Task<QualityStatsResponse> GetQualityStatsAsync(int days = 30);
}

// ============================================================================
// QualityInspectionService 实现
// ============================================================================

public class QualityInspectionService : IQualityInspectionService
{
    private readonly MesDbContext _context;
    private readonly ILogger<QualityInspectionService> _logger;

    public QualityInspectionService(MesDbContext context, ILogger<QualityInspectionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<InspectionResponse> CreateInspectionAsync(InspectionCreateRequest request, string createdBy)
    {
        _logger.LogInformation("Creating inspection for lot {LotId}, step {StepCode}, type {Type}",
            request.LotId, request.StepCode, request.InspectionType);

        // 1. 验证批次存在
        var lot = await _context.ProdLots.FindAsync(request.LotId);
        if (lot == null)
        {
            throw new InvalidOperationException($"批次 {request.LotId} 不存在");
        }

        // 2. 验证工序存在
        var lotStep = await _context.ProdLotSteps
            .FirstOrDefaultAsync(s => s.LotId == request.LotId && s.StepCode == request.StepCode);
        if (lotStep == null)
        {
            throw new InvalidOperationException($"批次 {request.LotId} 中不存在工序 {request.StepCode}");
        }

        // 3. 验证检验类型
        var validTypes = new[] { "IQC", "IPQC", "OQC" };
        if (!validTypes.Contains(request.InspectionType.ToUpperInvariant()))
        {
            throw new InvalidOperationException($"无效的检验类型: {request.InspectionType}，有效值: {string.Join(", ", validTypes)}");
        }

        // 4. 确定整体结果
        var overallResult = DetermineOverallResult(request.Items);

        // 5. 创建检验记录
        var inspectionId = GenerateInspectionId(request.InspectionType);
        var inspection = new QualityInspection
        {
            InspectionId = inspectionId,
            LotId = request.LotId,
            StepCode = request.StepCode,
            InspectionType = request.InspectionType.ToUpperInvariant(),
            InspectorId = request.InspectorId,
            Result = overallResult,
            Remark = request.Remarks,
            InspectionTime = DateTime.UtcNow,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.QualityInspections.AddAsync(inspection);

        // 6. 创建检验项目
        foreach (var item in request.Items)
        {
            var inspectionItem = new QualityInspectionItem
            {
                InspectionId = inspectionId,
                ItemCode = item.ItemCode,
                ItemName = item.ItemName,
                Specification = item.Specification,
                MeasuredValue = item.MeasuredValue,
                Result = item.Result,
                Unit = item.Unit,
                CreatedAt = DateTime.UtcNow
            };
            await _context.QualityInspectionItems.AddAsync(inspectionItem);
        }

        // 7. 如果不合格且设置了自动创建NCR，则创建NCR
        string? createdNcrId = null;
        if (overallResult == "Fail" && request.AutoCreateNcr)
        {
            var failItems = request.Items.Where(i => i.Result == "Fail").ToList();
            var ncrId = await CreateAutoNcrAsync(request.LotId, request.StepCode, failItems, request.InspectorId, createdBy);
            createdNcrId = ncrId;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Inspection {InspectionId} created with result {Result}", inspectionId, overallResult);

        return new InspectionResponse
        {
            InspectionId = inspection.InspectionId,
            LotId = inspection.LotId,
            StepCode = inspection.StepCode,
            Type = inspection.InspectionType,
            Inspector = inspection.InspectorId,
            InspectionTime = inspection.InspectionTime,
            Result = inspection.Result,
            Remarks = inspection.Remark,
            Items = request.Items,
            NcrId = createdNcrId,
            CreatedAt = inspection.CreatedAt
        };
    }

    public async Task<InspectionListResponse> GetInspectionsAsync(int pageIndex, int pageSize, string? lotId = null, string? type = null, string? result = null)
    {
        var query = _context.QualityInspections.AsQueryable();

        if (!string.IsNullOrEmpty(lotId))
            query = query.Where(x => x.LotId == lotId);

        if (!string.IsNullOrEmpty(type))
            query = query.Where(x => x.InspectionType == type);

        if (!string.IsNullOrEmpty(result))
            query = query.Where(x => x.Result == result);

        query = query.OrderByDescending(x => x.InspectionTime);

        var totalCount = await query.CountAsync();

        var inspections = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // 获取每个检验的项目统计
        var inspectionIds = inspections.Select(i => i.InspectionId).ToList();
        var itemStats = await _context.QualityInspectionItems
            .Where(i => inspectionIds.Contains(i.InspectionId))
            .GroupBy(i => i.InspectionId)
            .Select(g => new
            {
                InspectionId = g.Key,
                ItemCount = g.Count(),
                PassCount = g.Count(i => i.Result == "Pass"),
                FailCount = g.Count(i => i.Result == "Fail")
            })
            .ToDictionaryAsync(x => x.InspectionId);

        var items = inspections.Select(i =>
        {
            var stats = itemStats.GetValueOrDefault(i.InspectionId);
            return new InspectionSummaryDto
            {
                InspectionId = i.InspectionId,
                LotId = i.LotId,
                StepCode = i.StepCode,
                Type = i.InspectionType,
                Inspector = i.InspectorId,
                InspectionTime = i.InspectionTime,
                Result = i.Result,
                ItemCount = stats?.ItemCount ?? 0,
                PassCount = stats?.PassCount ?? 0,
                FailCount = stats?.FailCount ?? 0
            };
        }).ToList();

        return new InspectionListResponse
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<InspectionResponse?> GetInspectionDetailAsync(string inspectionId)
    {
        var inspection = await _context.QualityInspections.FindAsync(inspectionId);
        if (inspection == null) return null;

        var items = await _context.QualityInspectionItems
            .Where(i => i.InspectionId == inspectionId)
            .ToListAsync();

        // 查找关联的NCR
        var relatedNcr = await _context.NonConformanceReports
            .Where(n => n.LotId == inspection.LotId &&
                        n.StepCode == inspection.StepCode &&
                        n.DiscoveredTime >= inspection.InspectionTime.AddMinutes(-1) &&
                        n.DiscoveredTime <= inspection.InspectionTime.AddMinutes(1))
            .OrderBy(n => n.DiscoveredTime)
            .FirstOrDefaultAsync();

        return new InspectionResponse
        {
            InspectionId = inspection.InspectionId,
            LotId = inspection.LotId,
            StepCode = inspection.StepCode,
            Type = inspection.InspectionType,
            Inspector = inspection.InspectorId,
            InspectionTime = inspection.InspectionTime,
            Result = inspection.Result,
            Remarks = inspection.Remark,
            Items = items.Select(i => new InspectionItemDto
            {
                ItemCode = i.ItemCode,
                ItemName = i.ItemName,
                Specification = i.Specification,
                MeasuredValue = i.MeasuredValue,
                Result = i.Result,
                Unit = i.Unit
            }).ToList(),
            NcrId = relatedNcr?.NcrId,
            CreatedAt = inspection.CreatedAt
        };
    }

    public async Task<NcrResponse> CreateNcrAsync(NcrCreateRequest request, string? discovererName = null)
    {
        _logger.LogInformation("Creating NCR for lot {LotId}, defect {DefectType}", request.LotId, request.DefectType);

        var ncrId = GenerateNcrId();
        var ncr = new NonConformanceReport
        {
            NcrId = ncrId,
            LotId = request.LotId,
            StepCode = request.StepCode,
            DefectType = request.DefectType,
            DefectDescription = request.DefectDescription,
            Quantity = request.Quantity,
            Severity = request.Severity,
            Status = "Open",
            DiscovererId = request.DiscovererId,
            DiscovererName = discovererName,
            DiscoveredTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.NonConformanceReports.AddAsync(ncr);
        await _context.SaveChangesAsync();

        _logger.LogInformation("NCR {NcrId} created with severity {Severity}", ncrId, request.Severity);

        return MapToNcrResponse(ncr);
    }

    public async Task<NcrListResponse> GetNcrsAsync(int pageIndex, int pageSize, string? lotId = null, string? status = null, string? severity = null)
    {
        var query = _context.NonConformanceReports.AsQueryable();

        if (!string.IsNullOrEmpty(lotId))
            query = query.Where(x => x.LotId == lotId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);

        if (!string.IsNullOrEmpty(severity))
            query = query.Where(x => x.Severity == severity);

        query = query.OrderByDescending(x => x.DiscoveredTime);

        var totalCount = await query.CountAsync();

        var ncrs = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new NcrListResponse
        {
            Items = ncrs.Select(MapToNcrResponse).ToList(),
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    public async Task<bool> UpdateNcrStatusAsync(string ncrId, string status, string? disposition, string reviewerId, string? reviewerName = null, string? closureComment = null)
    {
        _logger.LogInformation("Updating NCR {NcrId} status to {Status}", ncrId, status);

        var ncr = await _context.NonConformanceReports.FindAsync(ncrId);
        if (ncr == null)
        {
            _logger.LogWarning("NCR {NcrId} not found", ncrId);
            return false;
        }

        // 验证状态流转
        if (!IsValidStatusTransition(ncr.Status, status))
        {
            throw new InvalidOperationException($"无效的状态转换: {ncr.Status} -> {status}");
        }

        ncr.Status = status;
        ncr.UpdatedAt = DateTime.UtcNow;

        if (disposition != null)
        {
            var validDispositions = new[] { "Rework", "Scrap", "Return", "UseAsIs" };
            if (!validDispositions.Contains(disposition))
            {
                throw new InvalidOperationException($"无效的处置方式: {disposition}，有效值: {string.Join(", ", validDispositions)}");
            }
            ncr.Disposition = disposition;
        }

        if (status == "UnderReview")
        {
            ncr.ReviewerId = reviewerId;
            ncr.ReviewerName = reviewerName;
            ncr.ReviewedAt = DateTime.UtcNow;
        }

        if (status == "Closed")
        {
            ncr.CloserId = reviewerId;
            ncr.ClosedAt = DateTime.UtcNow;
            ncr.ClosureComment = closureComment;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("NCR {NcrId} status updated to {Status}", ncrId, status);
        return true;
    }

    public async Task<QualityStatsResponse> GetQualityStatsAsync(int days = 30)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-days);

        // 检验统计
        var allInspections = await _context.QualityInspections
            .Where(x => x.InspectionTime >= cutoffDate)
            .ToListAsync();

        var totalInspections = allInspections.Count;
        var passedInspections = allInspections.Count(x => x.Result == "Pass");
        var failedInspections = allInspections.Count(x => x.Result == "Fail");
        var conditionalInspections = allInspections.Count(x => x.Result == "Conditional");

        var passRate = totalInspections > 0
            ? Math.Round((decimal)passedInspections / totalInspections * 100, 2)
            : 0m;

        // 按检验类型统计
        var statsByType = allInspections
            .GroupBy(x => x.InspectionType)
            .Select(g => new TypeStatItem
            {
                InspectionType = g.Key,
                Total = g.Count(),
                Passed = g.Count(x => x.Result == "Pass"),
                Failed = g.Count(x => x.Result == "Fail"),
                PassRate = g.Count() > 0
                    ? Math.Round((decimal)g.Count(x => x.Result == "Pass") / g.Count() * 100, 2)
                    : 0m
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        // 缺陷类型统计
        var allNcrs = await _context.NonConformanceReports
            .Where(x => x.DiscoveredTime >= cutoffDate)
            .ToListAsync();

        var statsByDefectType = allNcrs
            .GroupBy(x => x.DefectType)
            .Select(g => new DefectStatItem
            {
                DefectType = g.Key,
                Count = g.Count(),
                TotalQuantity = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        // NCR按状态统计
        var ncrStatsByStatus = allNcrs
            .GroupBy(x => x.Status)
            .Select(g => new StatusStatItem
            {
                Status = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        // 近期趋势（按日）
        var dailyTrend = allInspections
            .GroupBy(x => x.InspectionTime.Date)
            .Select(g => new
            {
                Date = g.Key,
                Inspections = g.Count(),
                Passed = g.Count(x => x.Result == "Pass"),
                Failed = g.Count(x => x.Result == "Fail"),
                NcrCount = allNcrs.Count(n => n.DiscoveredTime.Date == g.Key)
            })
            .OrderBy(x => x.Date)
            .Select(x => new TrendItem
            {
                Date = x.Date,
                Inspections = x.Inspections,
                Passed = x.Passed,
                Failed = x.Failed,
                PassRate = x.Inspections > 0
                    ? Math.Round((decimal)x.Passed / x.Inspections * 100, 2)
                    : 0m,
                NcrCount = x.NcrCount
            })
            .ToList();

        return new QualityStatsResponse
        {
            TotalInspections = totalInspections,
            PassedInspections = passedInspections,
            FailedInspections = failedInspections,
            ConditionalInspections = conditionalInspections,
            PassRate = passRate,
            StatsByType = statsByType,
            StatsByDefectType = statsByDefectType,
            NcrStatsByStatus = ncrStatsByStatus,
            DailyTrend = dailyTrend
        };
    }

    // ============================================================================
    // 私有辅助方法
    // ============================================================================

    private static string DetermineOverallResult(List<InspectionItemDto> items)
    {
        if (items.Count == 0) return "Pass";

        var failCount = items.Count(i => i.Result == "Fail");
        if (failCount == 0) return "Pass";
        if (failCount == items.Count) return "Fail";
        return "Conditional"; // 部分不合格
    }

    private static string GenerateInspectionId(string type)
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 999).ToString("D3");
        return $"QC-{type.Substring(0, 1)}-{now:yyyyMMdd}-{seq}";
    }

    private static string GenerateNcrId()
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 999).ToString("D3");
        return $"NCR-{now:yyyyMMdd}-{seq}";
    }

    private static NcrResponse MapToNcrResponse(NonConformanceReport ncr) => new()
    {
        NcrId = ncr.NcrId,
        LotId = ncr.LotId,
        StepCode = ncr.StepCode,
        DefectType = ncr.DefectType,
        DefectDescription = ncr.DefectDescription,
        Quantity = ncr.Quantity,
        Severity = ncr.Severity,
        Status = ncr.Status,
        Disposition = ncr.Disposition,
        Discoverer = ncr.DiscovererName ?? ncr.DiscovererId,
        DiscoveredTime = ncr.DiscoveredTime,
        Reviewer = ncr.ReviewerName ?? ncr.ReviewerId,
        ReviewedAt = ncr.ReviewedAt,
        CreatedAt = ncr.CreatedAt
    };

    private static bool IsValidStatusTransition(string currentStatus, string newStatus)
    {
        return (currentStatus, newStatus) switch
        {
            ("Open", "UnderReview") => true,
            ("Open", "Closed") => true,
            ("UnderReview", "Dispositioned") => true,
            ("UnderReview", "Closed") => true,
            ("Dispositioned", "Closed") => true,
            _ => false
        };
    }

    private async Task<string> CreateAutoNcrAsync(string lotId, string stepCode, List<InspectionItemDto> failItems, string inspectorId, string createdBy)
    {
        var ncrId = GenerateNcrId();
        var defectDescription = $"自动创建：检验不合格项目 - {string.Join(", ", failItems.Select(i => $"{i.ItemName}({i.ItemCode})"))}";

        var ncr = new NonConformanceReport
        {
            NcrId = ncrId,
            LotId = lotId,
            StepCode = stepCode,
            DefectType = "InspectionFailure",
            DefectDescription = defectDescription,
            Quantity = 0,
            Severity = "Minor",
            Status = "Open",
            DiscovererId = inspectorId,
            DiscoveredTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.NonConformanceReports.AddAsync(ncr);
        _logger.LogInformation("Auto-created NCR {NcrId} for failed inspection on lot {LotId}", ncrId, lotId);

        return ncrId;
    }
}
