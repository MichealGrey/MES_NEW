using MES.Contracts.Common;
using MES.Contracts.Phase1;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace MES.Services.Quality;

public class NonconformingService : INonconformingService
{
    private readonly MesDbContext _context;

    public NonconformingService(MesDbContext context)
    {
        _context = context;
    }

    public async Task<NonconformingRecordResponse> CreateRecordAsync(CreateNonconformingRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.LotId))
            throw new ApplicationException("批次ID不能为空");
        if (string.IsNullOrWhiteSpace(request.DefectCode))
            throw new ApplicationException("缺陷代码不能为空");
        if (request.AffectedQty <= 0)
            throw new ApplicationException("受影响数量必须大于0");

        var ncrId = GenerateId("NCR");
        var now = DateTime.UtcNow;

        var record = new NonconformingRecord
        {
            NcrId = ncrId,
            LotId = request.LotId,
            OrderId = null,
            ProductId = request.ProductId,
            SourceType = request.Source,
            DefectQty = request.AffectedQty,
            DefectCode = request.DefectCode,
            DefectDescription = request.DefectDescription,
            Severity = request.DefectCategory,
            Status = "Open",
            Disposition = string.Empty,
            ReporterId = request.ReportedBy,
            ReporterName = request.ReportedByName,
            ReportTime = now,
            Remark = request.Remark,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<NonconformingRecord>().AddAsync(record);
        await _context.SaveChangesAsync();

        return MapToResponse(record);
    }

    public async Task<PagedResult<NonconformingRecordResponse>> GetRecordsAsync(NonconformingQuery query)
    {
        var iq = _context.Set<NonconformingRecord>().AsQueryable();

        if (!string.IsNullOrEmpty(query.Source))
            iq = iq.Where(r => r.SourceType == query.Source);
        if (!string.IsNullOrEmpty(query.Status))
            iq = iq.Where(r => r.Status == query.Status);
        if (!string.IsNullOrEmpty(query.LotId))
            iq = iq.Where(r => r.LotId == query.LotId);
        if (!string.IsNullOrEmpty(query.DefectCategory))
            iq = iq.Where(r => r.Severity == query.DefectCategory);
        if (query.DateFrom.HasValue)
            iq = iq.Where(r => r.ReportTime >= query.DateFrom.Value);
        if (query.DateTo.HasValue)
            iq = iq.Where(r => r.ReportTime <= query.DateTo.Value);

        var totalCount = await iq.CountAsync();

        var items = await iq
            .OrderByDescending(r => r.ReportTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(r => new NonconformingRecordResponse
            {
                NcrId = r.NcrId,
                Source = r.SourceType,
                LotId = r.LotId,
                ProductId = r.ProductId,
                DefectCode = r.DefectCode ?? string.Empty,
                DefectCategory = r.Severity,
                AffectedQty = r.DefectQty,
                IsolationStatus = "NotIsolated",
                MrbStatus = string.IsNullOrEmpty(r.MrbReference) ? "NoMrb" : "UnderReview",
                Disposition = r.Disposition,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<NonconformingRecordResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<NonconformingRecordResponse> GetDetailAsync(string ncrId)
    {
        var record = await _context.Set<NonconformingRecord>().FindAsync(ncrId);
        if (record == null)
            throw new ApplicationException($"不合格品记录 {ncrId} 不存在");

        return MapToResponse(record);
    }

    public async Task<bool> IsolateAsync(string ncrId, string operatorId)
    {
        var record = await _context.Set<NonconformingRecord>().FindAsync(ncrId);
        if (record == null)
            throw new ApplicationException($"不合格品记录 {ncrId} 不存在");

        record.Status = "Isolated";
        record.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<MrbReviewResponse> CreateMrbAsync(string ncrId, string createdBy)
    {
        var record = await _context.Set<NonconformingRecord>().FindAsync(ncrId);
        if (record == null)
            throw new ApplicationException($"不合格品记录 {ncrId} 不存在");

        var mrbId = GenerateId("MRB");
        var now = DateTime.UtcNow;

        var mrb = new MrbReview
        {
            MrbId = mrbId,
            NcrId = ncrId,
            LotId = record.LotId,
            ProductId = record.ProductId,
            ProductName = record.ProductName,
            AffectedQty = record.DefectQty,
            Disposition = string.Empty,
            Status = "Pending",
            ReviewType = "Standard",
            ReviewerIds = "Quality,Process,Engineering",
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<MrbReview>().AddAsync(mrb);

        record.MrbReference = mrbId;
        record.Status = "UnderMrbReview";
        record.UpdatedAt = now;

        await _context.SaveChangesAsync();

        return MapToMrbResponse(mrb);
    }

    public async Task<MrbReviewResponse> VoteMrbAsync(string mrbId, MrbVoteRequest request)
    {
        var mrb = await _context.Set<MrbReview>().FindAsync(mrbId);
        if (mrb == null)
            throw new ApplicationException($"MRB评审 {mrbId} 不存在");

        var now = DateTime.UtcNow;
        var votes = new[] { request.Vote };

        // Simple vote tracking - in production, this would be more sophisticated
        switch (request.VoterRole)
        {
            case "Quality":
                mrb.ReviewConclusion = $"{mrb.ReviewConclusion}[Quality:{request.Vote}] ";
                break;
            case "Process":
                mrb.ReviewConclusion = $"{mrb.ReviewConclusion}[Process:{request.Vote}] ";
                break;
            case "Engineering":
                mrb.ReviewConclusion = $"{mrb.ReviewConclusion}[Engineering:{request.Vote}] ";
                break;
        }

        // Check if all votes are in - simple check
        var hasAllVotes = mrb.ReviewConclusion?.Contains("Quality") == true &&
                          mrb.ReviewConclusion?.Contains("Process") == true &&
                          mrb.ReviewConclusion?.Contains("Engineering") == true;

        if (hasAllVotes && request.DispositionRecommendation != null)
        {
            mrb.Disposition = request.DispositionRecommendation;
            mrb.Status = "Completed";
            mrb.ReviewTime = now;
        }
        else
        {
            mrb.Status = "InReview";
        }

        mrb.UpdatedAt = now;
        await _context.SaveChangesAsync();

        return MapToMrbResponse(mrb);
    }

    public async Task<bool> ExecuteDispositionAsync(string ncrId, DispositionRequest request)
    {
        var record = await _context.Set<NonconformingRecord>().FindAsync(ncrId);
        if (record == null)
            throw new ApplicationException($"不合格品记录 {ncrId} 不存在");

        var validDispositions = new[] { "Rework", "Scrap", "Concession", "Return", "UseAsIs" };
        if (!validDispositions.Contains(request.Disposition))
            throw new ApplicationException($"无效的处置方式: {request.Disposition}");

        var now = DateTime.UtcNow;
        record.Disposition = request.Disposition;
        record.Status = "Dispositioned";
        record.UpdatedAt = now;

        // Create disposition record
        var dispId = GenerateId("DISP");
        var dispRecord = new DispositionRecord
        {
            DispositionId = dispId,
            NcrId = ncrId,
            LotId = record.LotId,
            DispositionType = request.Disposition,
            DispositionQty = record.DefectQty,
            ExecutionDetail = request.DispositionDetail,
            Status = "Completed",
            ExecutorId = request.ApprovedBy,
            ExecutionTime = now,
            Remark = request.ApprovalComment,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _context.Set<DispositionRecord>().AddAsync(dispRecord);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReworkVerifyAsync(string ncrId, string result, string verifiedBy)
    {
        var record = await _context.Set<NonconformingRecord>().FindAsync(ncrId);
        if (record == null)
            throw new ApplicationException($"不合格品记录 {ncrId} 不存在");

        var now = DateTime.UtcNow;

        // Find related disposition record
        var dispRecord = await _context.Set<DispositionRecord>()
            .Where(d => d.NcrId == ncrId)
            .OrderByDescending(d => d.CreatedAt)
            .FirstOrDefaultAsync();

        if (dispRecord != null)
        {
            dispRecord.VerifierId = verifiedBy;
            dispRecord.VerificationTime = now;
            dispRecord.UpdatedAt = now;
        }

        if (result == "Pass")
        {
            record.Status = "Closed";
            record.ClosedBy = verifiedBy;
            record.ClosedTime = now;
        }
        else
        {
            record.Status = "ReworkFailed";
        }

        record.UpdatedAt = now;
        await _context.SaveChangesAsync();
        return true;
    }

    private static NonconformingRecordResponse MapToResponse(NonconformingRecord record)
    {
        return new NonconformingRecordResponse
        {
            NcrId = record.NcrId,
            Source = record.SourceType,
            LotId = record.LotId,
            ProductId = record.ProductId,
            DefectCode = record.DefectCode ?? string.Empty,
            DefectCategory = record.Severity,
            AffectedQty = record.DefectQty,
            IsolationStatus = record.Status == "Isolated" ? "Isolated" : "NotIsolated",
            MrbStatus = string.IsNullOrEmpty(record.MrbReference) ? "NoMrb" : "UnderReview",
            Disposition = record.Disposition,
            CreatedAt = record.CreatedAt
        };
    }

    private static MrbReviewResponse MapToMrbResponse(MrbReview mrb)
    {
        return new MrbReviewResponse
        {
            MrbId = mrb.MrbId,
            NcrId = mrb.NcrId ?? string.Empty,
            LotId = mrb.LotId,
            ProductId = mrb.ProductId,
            AffectedQty = mrb.AffectedQty,
            Status = mrb.Status,
            FinalDisposition = mrb.Disposition,
            CreatedAt = mrb.CreatedAt
        };
    }

    private static string GenerateId(string prefix)
    {
        var now = DateTime.UtcNow;
        var seq = new Random().Next(1, 9999).ToString("D4");
        return $"{prefix}-{now:yyyyMMdd}-{seq}";
    }
}
