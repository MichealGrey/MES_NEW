using System.Collections.ObjectModel;
using System.Windows.Input;
using MES.Modules.Production.Models;
using MES.Modules.Production.Services;
using Prism.Commands;
using Prism.Mvvm;

namespace MES.Modules.Production.ViewModels;

public class DispatchViewModel : BindableBase
{
    private readonly IDispatchService _dispatchService;

    private ObservableCollection<DispatchTask> _tasks = [];
    private DispatchTask? _selectedTask;
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ObservableCollection<DispatchTask> Tasks
    {
        get => _tasks;
        set => SetProperty(ref _tasks, value);
    }

    public DispatchTask? SelectedTask
    {
        get => _selectedTask;
        set => SetProperty(ref _selectedTask, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand AssignCommand { get; }
    public ICommand StartCommand { get; }
    public ICommand CompleteCommand { get; }

    public DispatchViewModel(IDispatchService dispatchService)
    {
        _dispatchService = dispatchService;
        RefreshCommand = new DelegateCommand(async () => await LoadTasksAsync());
        AssignCommand = new DelegateCommand(async () => await AssignTaskAsync());
        StartCommand = new DelegateCommand(async () => await StartTaskAsync());
        CompleteCommand = new DelegateCommand(async () => await CompleteTaskAsync());
    }

    private async System.Threading.Tasks.Task LoadTasksAsync()
    {
        var tasks = await _dispatchService.GenerateDispatchListAsync();
        Tasks = new ObservableCollection<DispatchTask>(tasks);
    }

    private async System.Threading.Tasks.Task AssignTaskAsync()
    {
        if (SelectedTask is null) return;
        await _dispatchService.AssignTaskAsync(SelectedTask.TaskId, "OP001");
        await LoadTasksAsync();
    }

    private async System.Threading.Tasks.Task StartTaskAsync()
    {
        if (SelectedTask is null) return;
        await _dispatchService.StartTaskAsync(SelectedTask.TaskId);
        await LoadTasksAsync();
    }

    private async System.Threading.Tasks.Task CompleteTaskAsync()
    {
        if (SelectedTask is null) return;
        await _dispatchService.CompleteTaskAsync(SelectedTask.TaskId);
        await LoadTasksAsync();
    }
}
