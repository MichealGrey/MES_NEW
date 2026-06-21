using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MES.Contracts.Common;
using MES.Contracts.Phase5;
using MES.Infrastructure.Persistence;
using MES.Infrastructure.Persistence.Entities;

namespace MES.Services.Analytics;

public class NpiService : INpiService
{
    private readonly MesDbContext _context;
    private readonly ILogger<NpiService> _logger;

    public NpiService(MesDbContext context, ILogger<NpiService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<NpiProjectResponse> CreateProjectAsync(CreateNpiProjectRequest request, string operatorId)
    {
        var now = DateTime.UtcNow;
        var projectId = $"NPI-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var project = new NpiProject
        {
            ProjectId = projectId,
            ProjectName = request.ProjectName,
            ProductId = request.ProductId,
            Status = "Active",
            Phase = "Initiation",
            StartDate = now,
            TargetCompletion = request.TargetDate?.ToDateTime(TimeOnly.MinValue),
            CreatedBy = operatorId,
            CreatedAt = now
        };

        _context.NpiProjects.Add(project);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created NPI project {ProjectId}: {ProjectName}", projectId, request.ProjectName);

        return MapToResponse(project);
    }

    public async Task<NpiProjectResponse> GetProjectAsync(string projectId)
    {
        var project = await _context.NpiProjects
            .FirstOrDefaultAsync(x => x.ProjectId == projectId);

        if (project == null)
            throw new KeyNotFoundException($"NPI project '{projectId}' not found");

        return MapToResponse(project);
    }

    public async Task<PagedResult<NpiProjectResponse>> QueryProjectsAsync(NpiProjectQuery query)
    {
        var iqQuery = _context.NpiProjects.AsQueryable();

        if (!string.IsNullOrEmpty(query.Keyword))
            iqQuery = iqQuery.Where(x => x.ProjectName != null && x.ProjectName.Contains(query.Keyword));
        if (!string.IsNullOrEmpty(query.Status))
            iqQuery = iqQuery.Where(x => x.Status == query.Status);
        if (!string.IsNullOrEmpty(query.CurrentStage))
            iqQuery = iqQuery.Where(x => x.Phase == query.CurrentStage);

        var totalCount = await iqQuery.CountAsync();

        var items = await iqQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => MapToResponse(x))
            .ToListAsync();

        return new PagedResult<NpiProjectResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<List<NpiStageResponse>> GetStagesAsync(string projectId)
    {
        var stages = await _context.NpiStages
            .Where(x => x.ProjectId == projectId)
            .OrderBy(x => x.StageOrder)
            .ToListAsync();

        return stages.Select(MapStageToResponse).ToList();
    }

    public async Task<NpiStageResponse> ExecuteTrialRunAsync(string projectId, TrialRunRequest request, string operatorId)
    {
        var project = await _context.NpiProjects
            .FirstOrDefaultAsync(x => x.ProjectId == projectId);

        if (project == null)
            throw new KeyNotFoundException($"NPI project '{projectId}' not found");

        project.Phase = "TrialRun";

        var now = DateTime.UtcNow;
        var stageId = $"STAGE-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var stage = new NpiStage
        {
            StageId = stageId,
            ProjectId = projectId,
            StageName = "Trial Run",
            StageOrder = 3,
            Status = "InProgress",
            StartDate = now,
            Result = request.Remark,
            CreatedAt = now
        };

        _context.NpiStages.Add(stage);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Executed trial run for NPI project {ProjectId}", projectId);

        return MapStageToResponse(stage);
    }

    public async Task<NpiStageResponse> ReviewAsync(string projectId, NpiReviewRequest request, string operatorId)
    {
        var stage = await _context.NpiStages
            .FirstOrDefaultAsync(x => x.StageId == request.StageId && x.ProjectId == projectId);

        if (stage == null)
            throw new KeyNotFoundException($"NPI stage '{request.StageId}' not found");

        stage.Status = request.Result == "Pass" ? "Completed" : "Failed";
        stage.EndDate = DateTime.UtcNow;
        stage.Result = request.Comment;

        var project = await _context.NpiProjects
            .FirstOrDefaultAsync(x => x.ProjectId == projectId);

        if (project != null && stage.Status == "Completed")
            project.Phase = "Review";

        await _context.SaveChangesAsync();

        _logger.LogInformation("Reviewed NPI stage {StageId} with result {Result}", request.StageId, request.Result);

        return MapStageToResponse(stage);
    }

    public async Task<NpiProjectResponse> TransferToMassProductionAsync(string projectId, string operatorId)
    {
        var project = await _context.NpiProjects
            .FirstOrDefaultAsync(x => x.ProjectId == projectId);

        if (project == null)
            throw new KeyNotFoundException($"NPI project '{projectId}' not found");

        project.Phase = "MassProduction";
        project.Status = "Completed";
        project.ActualCompletion = DateTime.UtcNow;

        var now = DateTime.UtcNow;
        var stageId = $"STAGE-{now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

        var stage = new NpiStage
        {
            StageId = stageId,
            ProjectId = projectId,
            StageName = "Mass Production Transfer",
            StageOrder = 5,
            Status = "Completed",
            StartDate = now,
            EndDate = now,
            Result = "Transferred to mass production",
            CreatedAt = now
        };

        _context.NpiStages.Add(stage);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Transferred NPI project {ProjectId} to mass production", projectId);

        return MapToResponse(project);
    }

    private static NpiProjectResponse MapToResponse(NpiProject entity) => new()
    {
        ProjectId = entity.ProjectId,
        ProjectCode = entity.ProjectId, // Phase 3 NpiProject doesn't have ProjectCode
        ProjectName = entity.ProjectName ?? string.Empty,
        ProductId = entity.ProductId,
        CurrentStage = entity.Phase ?? string.Empty,
        ProjectManager = null,
        TargetDate = entity.TargetCompletion.HasValue ? DateOnly.FromDateTime(entity.TargetCompletion.Value) : null,
        ActualDate = entity.ActualCompletion.HasValue ? DateOnly.FromDateTime(entity.ActualCompletion.Value) : null,
        Status = entity.Status ?? string.Empty,
        Description = null,
        CreatedBy = entity.CreatedBy,
        CreatedAt = entity.CreatedAt,
        UpdatedBy = null,
        UpdatedAt = DateTime.UtcNow
    };

    private static NpiStageResponse MapStageToResponse(NpiStage entity) => new()
    {
        StageId = entity.StageId,
        ProjectId = entity.ProjectId,
        StageName = entity.StageName,
        StageOrder = entity.StageOrder,
        Status = entity.Status,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        Result = entity.Result,
        CreatedAt = entity.CreatedAt
    };
}
