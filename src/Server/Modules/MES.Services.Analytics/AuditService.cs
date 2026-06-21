using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Analytics;

public class AuditService : IAuditService
{
    private readonly MesDbContext _context;
    private readonly ILogger<AuditService> _logger;

    public AuditService(MesDbContext context, ILogger<AuditService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<DataCorrectionResponse>> QueryCorrectionsAsync(DataCorrectionQuery query)
    {
        var iqQuery = _context.DataCorrectionRecords.AsQueryable();

        if (!string.IsNullOrEmpty(query.TableName))
            iqQuery = iqQuery.Where(x => x.TableName == query.TableName);
        if (!string.IsNullOrEmpty(query.RecordId))
            iqQuery = iqQuery.Where(x => x.RecordId == query.RecordId);
        if (query.StartDate.HasValue)
            iqQuery = iqQuery.Where(x => x.CorrectedAt >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            iqQuery = iqQuery.Where(x => x.CorrectedAt <= query.EndDate.Value);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(x => x.CorrectedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => MapToResponse(x))
            .ToListAsync();

        return new PagedResult<DataCorrectionResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<bool> VerifyAuditIntegrityAsync()
    {
        var corrections = await _context.DataCorrectionRecords
            .OrderBy(x => x.CorrectedAt)
            .ToListAsync();

        if (corrections.Count == 0)
            return true;

        // Verify that all corrections have required fields
        var hasMissingFields = corrections.Any(x =>
            string.IsNullOrEmpty(x.TableName) ||
            string.IsNullOrEmpty(x.RecordId) ||
            string.IsNullOrEmpty(x.FieldName) ||
            string.IsNullOrEmpty(x.Reason));

        if (hasMissingFields)
        {
            _logger.LogWarning("Audit integrity check failed: missing required fields in corrections");
            return false;
        }

        _logger.LogInformation("Audit integrity check passed with {Count} corrections", corrections.Count);
        return true;
    }

    public async Task<Dictionary<string, string>> HashCheckAsync(string tableName, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.DataCorrectionRecords
            .Where(x => x.TableName == tableName);

        if (startDate.HasValue)
            query = query.Where(x => x.CorrectedAt >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(x => x.CorrectedAt <= endDate.Value);

        var corrections = await query.ToListAsync();

        // Generate hash for each record
        var hashes = new Dictionary<string, string>();
        foreach (var correction in corrections)
        {
            var data = $"{correction.TableName}|{correction.RecordId}|{correction.FieldName}|{correction.OldValue}|{correction.NewValue}|{correction.CorrectedAt:O}";
            var hash = ComputeSha256Hash(data);
            hashes[correction.CorrectionId] = hash;
        }

        return hashes;
    }

    public async Task<DataCorrectionResponse> CreateCorrectionAsync(CreateDataCorrectionRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var correctionId = $"CORR-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var correction = new DataCorrectionRecord
        {
            CorrectionId = correctionId,
            TableName = request.TableName,
            RecordId = request.RecordId,
            FieldName = request.FieldName,
            OldValue = request.OldValue,
            NewValue = request.NewValue,
            Reason = request.Reason,
            ApprovedBy = request.ApprovedBy,
            CorrectedBy = operatorId,
            CorrectedAt = now
        };

        _context.DataCorrectionRecords.Add(correction);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created data correction {CorrectionId} for {TableName}.{FieldName}", correctionId, request.TableName, request.FieldName);

        return MapToResponse(correction);
    }

    private static DataCorrectionResponse MapToResponse(DataCorrectionRecord entity) => new()
    {
        CorrectionId = entity.CorrectionId,
        TableName = entity.TableName,
        RecordId = entity.RecordId,
        FieldName = entity.FieldName,
        OldValue = entity.OldValue,
        NewValue = entity.NewValue,
        Reason = entity.Reason,
        ApprovedBy = entity.ApprovedBy,
        CorrectedBy = entity.CorrectedBy,
        CorrectedAt = entity.CorrectedAt
    };

    private static string ComputeSha256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
