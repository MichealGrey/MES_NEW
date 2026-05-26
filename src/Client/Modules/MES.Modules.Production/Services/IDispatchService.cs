using MES.Modules.Production.Models;

namespace MES.Modules.Production.Services;

public interface IDispatchService
{
    Task<List<DispatchTask>> GenerateDispatchListAsync();
    Task AssignTaskAsync(string taskId, string operatorId);
    Task StartTaskAsync(string taskId);
    Task CompleteTaskAsync(string taskId);
    Task<List<DispatchTask>> GetPendingTasksAsync();
    Task<List<DispatchTask>> GetTasksByEquipmentAsync(string equipmentId);
    Task<List<DispatchTask>> GetOverdueTasksAsync();
}
