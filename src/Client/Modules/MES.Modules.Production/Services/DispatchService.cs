using MES.Infrastructure.Persistence.Entities;
using MES.Infrastructure.Persistence.Repositories;
using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public class DispatchService : IDispatchService
{
    private readonly IRepository<ProdDispatchTask> _taskRepo;
    private readonly IRepository<ProdLot> _lotRepo;

    public DispatchService(IRepository<ProdDispatchTask> taskRepo, IRepository<ProdLot> lotRepo)
    {
        _taskRepo = taskRepo;
        _lotRepo = lotRepo;
    }

    public async Task<List<DispatchTask>> GenerateDispatchListAsync()
    {
        var waitingLots = await _lotRepo.GetWhereAsync(l => l.Status == "Waiting");
        var tasks = new List<DispatchTask>();

        foreach (var lot in waitingLots)
        {
            var task = new DispatchTask
            {
                TaskId = Guid.NewGuid().ToString("N"),
                LotId = lot.LotId,
                OrderId = lot.OrderId,
                ProductId = lot.ProductId,
                StepCode = lot.CurrentStepCode ?? string.Empty,
                StepSeq = lot.CurrentStepSeq,
                Qty = lot.UnitCount,
                Priority = lot.Priority switch
                {
                    "High" => "High",
                    "Urgent" => "Urgent",
                    _ => "Normal"
                },
                Status = "Pending",
                IsOverdue = false,
                CreatedAt = DateTime.UtcNow,
            };
            tasks.Add(task);
        }

        tasks = tasks.OrderByDescending(t => t.Priority switch
        {
            "Urgent" => 4,
            "High" => 3,
            "Normal" => 2,
            _ => 1
        }).ToList();

        return tasks;
    }

    public async Task AssignTaskAsync(string taskId, string operatorId)
    {
        var task = await _taskRepo.GetByIdAsync(taskId);
        if (task is null) return;
        task.Status = "Assigned";
        task.AssignedOperator = operatorId;
        task.AssignedAt = DateTime.UtcNow;
        await _taskRepo.UpdateAsync(task);
    }

    public async Task StartTaskAsync(string taskId)
    {
        var task = await _taskRepo.GetByIdAsync(taskId);
        if (task is null) return;
        task.Status = "Running";
        task.StartedAt = DateTime.UtcNow;
        await _taskRepo.UpdateAsync(task);
    }

    public async Task CompleteTaskAsync(string taskId)
    {
        var task = await _taskRepo.GetByIdAsync(taskId);
        if (task is null) return;
        task.Status = "Completed";
        task.CompletedAt = DateTime.UtcNow;
        await _taskRepo.UpdateAsync(task);
    }

    public async Task<List<DispatchTask>> GetPendingTasksAsync()
    {
        var tasks = await _taskRepo.GetWhereAsync(t => t.Status == "Pending");
        return tasks.Select(MapToModel).ToList();
    }

    public async Task<List<DispatchTask>> GetTasksByEquipmentAsync(string equipmentId)
    {
        // Dispatch tasks don't have equipment directly, return all tasks for now
        var allTasks = await _taskRepo.GetAllAsync();
        return allTasks.Select(MapToModel).ToList();
    }

    public async Task<List<DispatchTask>> GetOverdueTasksAsync()
    {
        var tasks = await _taskRepo.GetWhereAsync(t => t.IsOverdue);
        return tasks.Select(MapToModel).ToList();
    }

    private static DispatchTask MapToModel(ProdDispatchTask entity) => new()
    {
        TaskId = entity.TaskId,
        LotId = entity.LotId,
        OrderId = entity.OrderId,
        ProductId = entity.ProductId,
        StepCode = entity.StepCode,
        StepSeq = entity.StepSeq,
        Qty = entity.Qty,
        Priority = entity.Priority,
        Status = entity.Status,
        AssignedOperator = entity.AssignedOperator,
        AssignedAt = entity.AssignedAt,
        StartedAt = entity.StartedAt,
        CompletedAt = entity.CompletedAt,
        IsOverdue = entity.IsOverdue,
        CreatedAt = entity.CreatedAt,
    };
}
