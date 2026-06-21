using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using MES.Modules.CustomerComplaint.Models;
using MES.Modules.CustomerComplaint.Services;

namespace MES.Modules.CustomerComplaint.ViewModels;

public class ComplaintListViewModel : BindableBase
{
    private readonly IComplaintService _service;
    private readonly IRegionManager _regionManager;
    private ObservableCollection<ComplaintInfo> _complaints = [];
    private ComplaintInfo? _selectedComplaint;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private string? _filterStatus;
    private string? _filterSeverity;
    private string? _filterEightDStatus;
    private string? _filterPriority;
    private bool _showOverdueOnly;

    public ObservableCollection<ComplaintInfo> Complaints { get => _complaints; set => SetProperty(ref _complaints, value); }
    public ComplaintInfo? SelectedComplaint { get => _selectedComplaint; set { if (SetProperty(ref _selectedComplaint, value)) RaiseCanExecuteChanged(); } }
    public string SearchText { get => _searchText; set { if (SetProperty(ref _searchText, value)) ApplyFilter(); } }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public string? FilterStatus { get => _filterStatus; set { if (SetProperty(ref _filterStatus, value)) ApplyFilter(); } }
    public string? FilterSeverity { get => _filterSeverity; set { if (SetProperty(ref _filterSeverity, value)) ApplyFilter(); } }
    public string? FilterEightDStatus { get => _filterEightDStatus; set { if (SetProperty(ref _filterEightDStatus, value)) ApplyFilter(); } }
    public string? FilterPriority { get => _filterPriority; set { if (SetProperty(ref _filterPriority, value)) ApplyFilter(); } }
    public bool ShowOverdueOnly { get => _showOverdueOnly; set { if (SetProperty(ref _showOverdueOnly, value)) ApplyFilter(); } }

    public int TotalCount => Complaints.Count;
    public int OpenCount => Complaints.Count(c => c.Status == "Open");
    public int InProgressCount => Complaints.Count(c => c.Status == "InProgress");
    public int ClosedCount => Complaints.Count(c => c.Status == "Closed");
    public int OverdueCount => Complaints.Count(c => c.RequiredDate.HasValue && c.RequiredDate.Value < DateTime.Now && c.Status != "Closed");
    public double AvgOpenDays => Complaints.Any(c => !c.IsClosed) ? Complaints.Where(c => !c.IsClosed).Average(c => c.OpenDays) : 0;
    public double CloseRate => TotalCount > 0 ? (double)ClosedCount / TotalCount * 100 : 0;

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand<ComplaintInfo?> ViewDetailCommand { get; }
    public DelegateCommand<ComplaintInfo?> EditCommand { get; }
    public DelegateCommand<ComplaintInfo?> AdvanceStepCommand { get; }
    public DelegateCommand<ComplaintInfo?> ApproveCommand { get; }
    public DelegateCommand<ComplaintInfo?> CloseCommand { get; }
    public DelegateCommand ClearFilterCommand { get; }
    public DelegateCommand AddCommand { get; }

    public ComplaintListViewModel(IComplaintService service, IRegionManager regionManager)
    {
        _service = service;
        _regionManager = regionManager;
        RefreshCommand = new DelegateCommand(OnRefresh);
        ViewDetailCommand = new DelegateCommand<ComplaintInfo?>(OnViewDetail, c => c != null);
        EditCommand = new DelegateCommand<ComplaintInfo?>(OnEdit, c => c != null && c.Status != "Closed");
        AdvanceStepCommand = new DelegateCommand<ComplaintInfo?>(OnAdvanceStep, c => c != null && c.Status != "Closed");
        ApproveCommand = new DelegateCommand<ComplaintInfo?>(OnApprove, c => c != null && c.Status != "Closed");
        CloseCommand = new DelegateCommand<ComplaintInfo?>(OnClose, c => c?.Status != "Closed");
        ClearFilterCommand = new DelegateCommand(OnClearFilter);
        AddCommand = new DelegateCommand(OnAdd);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"数据加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllComplaintsAsync(); Complaints = new ObservableCollection<ComplaintInfo>(items.OrderByDescending(c => c.ReportDate)); UpdateStatistics(); }

    private void ApplyFilter()
    {
        var query = Complaints.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(SearchText)) { var k = SearchText.Trim().ToLower(); query = query.Where(c => c.ComplaintNo.ToLower().Contains(k) || c.CustomerName.ToLower().Contains(k) || c.ProductName.ToLower().Contains(k) || c.DefectType.ToLower().Contains(k)); }
        if (!string.IsNullOrWhiteSpace(FilterStatus)) query = query.Where(c => c.Status == FilterStatus);
        if (!string.IsNullOrWhiteSpace(FilterSeverity)) query = query.Where(c => c.Severity == FilterSeverity);
        if (!string.IsNullOrWhiteSpace(FilterEightDStatus)) query = query.Where(c => c.EightDStatus == FilterEightDStatus);
        if (!string.IsNullOrWhiteSpace(FilterPriority)) query = query.Where(c => c.Priority == FilterPriority);
        if (ShowOverdueOnly) query = query.Where(c => c.RequiredDate.HasValue && c.RequiredDate.Value < DateTime.Now && c.Status != "Closed");
        Complaints = new ObservableCollection<ComplaintInfo>(query.OrderByDescending(c => c.ReportDate));
        UpdateStatistics();
    }

    private async void OnRefresh() { try { ErrorMessage = null; SearchText = string.Empty; FilterStatus = null; FilterSeverity = null; FilterEightDStatus = null; FilterPriority = null; ShowOverdueOnly = false; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }
    private void OnClearFilter() { SearchText = string.Empty; FilterStatus = null; FilterSeverity = null; FilterEightDStatus = null; FilterPriority = null; ShowOverdueOnly = false; _ = ReloadDataAsync(); }

    private void OnViewDetail(ComplaintInfo? c)
    {
        if (c == null) return;
        var parameters = new NavigationParameters { { "ComplaintId", c.ComplaintId } };
        _regionManager.RequestNavigate("MainContentRegion", "ComplaintDetailView", parameters);
    }

    private void OnEdit(ComplaintInfo? c)
    {
        if (c == null) return;
        var parameters = new NavigationParameters { { "ComplaintId", c.ComplaintId } };
        _regionManager.RequestNavigate("MainContentRegion", "ComplaintActionView", parameters);
    }

    private async void OnAdvanceStep(ComplaintInfo? c)
    {
        if (c == null) return;
        var nextStep = GetNextEightDStep(c.EightDStatus);
        if (string.IsNullOrEmpty(nextStep)) return;
        try
        {
            await _service.UpdateEightDStepAsync(c.ComplaintId, c.EightDStatus, "步骤已完成");
            c.EightDStatus = nextStep;
            await _service.SaveComplaintAsync(c);
            await ReloadDataAsync();
        }
        catch (Exception ex) { ErrorMessage = $"推进步骤失败: {ex.Message}"; }
    }

    private async void OnApprove(ComplaintInfo? c)
    {
        if (c == null) return;
        try
        {
            c.ApprovalStatus = "Submitted";
            await _service.SaveComplaintAsync(c);
            await ReloadDataAsync();
        }
        catch (Exception ex) { ErrorMessage = $"提交审批失败: {ex.Message}"; }
    }

    private async void OnClose(ComplaintInfo? c)
    {
        if (c == null) return;
        try { await _service.UpdateComplaintStatusAsync(c.ComplaintId, "Closed"); await ReloadDataAsync(); }
        catch (Exception ex) { ErrorMessage = $"关闭失败: {ex.Message}"; }
    }

    private void OnAdd() { _regionManager.RequestNavigate("MainContentRegion", "ComplaintActionView"); }

    private void RaiseCanExecuteChanged() { ViewDetailCommand.RaiseCanExecuteChanged(); CloseCommand.RaiseCanExecuteChanged(); EditCommand.RaiseCanExecuteChanged(); AdvanceStepCommand.RaiseCanExecuteChanged(); ApproveCommand.RaiseCanExecuteChanged(); }
    private void UpdateStatistics() { RaisePropertyChanged(nameof(TotalCount)); RaisePropertyChanged(nameof(OpenCount)); RaisePropertyChanged(nameof(InProgressCount)); RaisePropertyChanged(nameof(ClosedCount)); RaisePropertyChanged(nameof(OverdueCount)); RaisePropertyChanged(nameof(AvgOpenDays)); RaisePropertyChanged(nameof(CloseRate)); }

    private static string? GetNextEightDStep(string currentStep)
    {
        var steps = new[] { "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8" };
        var idx = Array.IndexOf(steps, currentStep);
        return idx >= 0 && idx < steps.Length - 1 ? steps[idx + 1] : null;
    }
}

public class ComplaintDetailViewModel : BindableBase
{
    private readonly IComplaintService _service;
    private ComplaintInfo? _complaint;
    private ObservableCollection<EightDStep> _eightDSteps = [];
    private ObservableCollection<Complaint8DTeamMember> _teamMembers = [];
    private ObservableCollection<Complaint8DContainment> _containments = [];
    private ObservableCollection<Complaint8DRootCause> _rootCauses = [];
    private ObservableCollection<Complaint8DAction> _actions = [];
    private ObservableCollection<Complaint8DDocUpdate> _docUpdates = [];
    private ObservableCollection<string> _eightDStatusProgression = [];
    private string? _errorMessage;

    public ComplaintInfo? Complaint { get => _complaint; set => SetProperty(ref _complaint, value); }
    public ObservableCollection<EightDStep> EightDSteps { get => _eightDSteps; set => SetProperty(ref _eightDSteps, value); }
    public ObservableCollection<Complaint8DTeamMember> TeamMembers { get => _teamMembers; set => SetProperty(ref _teamMembers, value); }
    public ObservableCollection<Complaint8DContainment> Containments { get => _containments; set => SetProperty(ref _containments, value); }
    public ObservableCollection<Complaint8DRootCause> RootCauses { get => _rootCauses; set => SetProperty(ref _rootCauses, value); }
    public ObservableCollection<Complaint8DAction> Actions { get => _actions; set => SetProperty(ref _actions, value); }
    public ObservableCollection<Complaint8DDocUpdate> DocUpdates { get => _docUpdates; set => SetProperty(ref _docUpdates, value); }
    public ObservableCollection<string> EightDStatusProgression { get => _eightDStatusProgression; set => SetProperty(ref _eightDStatusProgression, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand<EightDStep?> UpdateStepCommand { get; }
    public DelegateCommand SubmitForApprovalCommand { get; }
    public DelegateCommand ExportReportCommand { get; }

    // 子表编辑命令
    public DelegateCommand AddTeamMemberCommand { get; }
    public DelegateCommand<Complaint8DTeamMember?> DeleteTeamMemberCommand { get; }
    public DelegateCommand AddContainmentCommand { get; }
    public DelegateCommand<Complaint8DContainment?> DeleteContainmentCommand { get; }
    public DelegateCommand AddRootCauseCommand { get; }
    public DelegateCommand<Complaint8DRootCause?> DeleteRootCauseCommand { get; }
    public DelegateCommand AddActionCommand { get; }
    public DelegateCommand<Complaint8DAction?> DeleteActionCommand { get; }
    public DelegateCommand AddDocUpdateCommand { get; }
    public DelegateCommand<Complaint8DDocUpdate?> DeleteDocUpdateCommand { get; }

    public ComplaintDetailViewModel(IComplaintService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        UpdateStepCommand = new DelegateCommand<EightDStep?>(OnUpdateStep);
        SubmitForApprovalCommand = new DelegateCommand(OnSubmitForApproval);
        ExportReportCommand = new DelegateCommand(OnExportReport);
        AddTeamMemberCommand = new DelegateCommand(OnAddTeamMember);
        DeleteTeamMemberCommand = new DelegateCommand<Complaint8DTeamMember?>(OnDeleteTeamMember);
        AddContainmentCommand = new DelegateCommand(OnAddContainment);
        DeleteContainmentCommand = new DelegateCommand<Complaint8DContainment?>(OnDeleteContainment);
        AddRootCauseCommand = new DelegateCommand(OnAddRootCause);
        DeleteRootCauseCommand = new DelegateCommand<Complaint8DRootCause?>(OnDeleteRootCause);
        AddActionCommand = new DelegateCommand(OnAddAction);
        DeleteActionCommand = new DelegateCommand<Complaint8DAction?>(OnDeleteAction);
        AddDocUpdateCommand = new DelegateCommand(OnAddDocUpdate);
        DeleteDocUpdateCommand = new DelegateCommand<Complaint8DDocUpdate?>(OnDeleteDocUpdate);
    }

    public async Task LoadComplaintAsync(string complaintId)
    {
        try
        {
            ErrorMessage = null;
            Complaint = await _service.GetComplaintAsync(complaintId);
            if (Complaint == null) return;

            var steps = await _service.GetEightDStepsAsync(complaintId);
            EightDSteps = new ObservableCollection<EightDStep>(steps);

            // 加载子表数据 (模拟)
            TeamMembers = new ObservableCollection<Complaint8DTeamMember>(GetSampleTeamMembers(complaintId));
            Containments = new ObservableCollection<Complaint8DContainment>(GetSampleContainments(complaintId));
            RootCauses = new ObservableCollection<Complaint8DRootCause>(GetSampleRootCauses(complaintId));
            Actions = new ObservableCollection<Complaint8DAction>(GetSampleActions(complaintId));
            DocUpdates = new ObservableCollection<Complaint8DDocUpdate>(GetSampleDocUpdates(complaintId));

            // 8D状态进展
            LoadEightDStatusProgression();
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
    }

    private void LoadEightDStatusProgression()
    {
        var steps = new[] { "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8" };
        var currentIdx = Array.IndexOf(steps, Complaint?.EightDStatus ?? "D0");
        EightDStatusProgression = new ObservableCollection<string>(
            steps.Select((s, i) => i <= currentIdx ? s : ""));
    }

    private async void OnRefresh() { if (Complaint != null) await LoadComplaintAsync(Complaint.ComplaintId); }

    private async void OnUpdateStep(EightDStep? step)
    {
        if (step == null || Complaint == null) return;
        try { await _service.UpdateEightDStepAsync(Complaint.ComplaintId, step.Step, step.Content); await LoadComplaintAsync(Complaint.ComplaintId); }
        catch (Exception ex) { ErrorMessage = $"更新失败: {ex.Message}"; }
    }

    private async void OnSubmitForApproval()
    {
        if (Complaint == null) return;
        try
        {
            Complaint.ApprovalStatus = "Submitted";
            await _service.SaveComplaintAsync(Complaint);
            ErrorMessage = "已提交审批";
        }
        catch (Exception ex) { ErrorMessage = $"提交审批失败: {ex.Message}"; }
    }

    private void OnExportReport()
    {
        System.Windows.MessageBox.Show("报告导出功能开发中...", "导出", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
    }

    // 子表操作方法
    private void OnAddTeamMember()
    {
        if (Complaint == null) return;
        var newMember = new Complaint8DTeamMember
        {
            MemberId = Guid.NewGuid().ToString(),
            ComplaintId = Complaint.ComplaintId,
            JoinDate = DateTime.Now
        };
        TeamMembers.Add(newMember);
    }

    private void OnDeleteTeamMember(Complaint8DTeamMember? member)
    {
        if (member != null && TeamMembers.Contains(member))
            TeamMembers.Remove(member);
    }

    private void OnAddContainment()
    {
        if (Complaint == null) return;
        var newContainment = new Complaint8DContainment
        {
            ContainmentId = Guid.NewGuid().ToString(),
            ComplaintId = Complaint.ComplaintId
        };
        Containments.Add(newContainment);
    }

    private void OnDeleteContainment(Complaint8DContainment? item)
    {
        if (item != null && Containments.Contains(item))
            Containments.Remove(item);
    }

    private void OnAddRootCause()
    {
        if (Complaint == null) return;
        var newCause = new Complaint8DRootCause
        {
            CauseId = Guid.NewGuid().ToString(),
            ComplaintId = Complaint.ComplaintId
        };
        RootCauses.Add(newCause);
    }

    private void OnDeleteRootCause(Complaint8DRootCause? item)
    {
        if (item != null && RootCauses.Contains(item))
            RootCauses.Remove(item);
    }

    private void OnAddAction()
    {
        if (Complaint == null) return;
        var newAction = new Complaint8DAction
        {
            ActionId = Guid.NewGuid().ToString(),
            ComplaintId = Complaint.ComplaintId
        };
        Actions.Add(newAction);
    }

    private void OnDeleteAction(Complaint8DAction? item)
    {
        if (item != null && Actions.Contains(item))
            Actions.Remove(item);
    }

    private void OnAddDocUpdate()
    {
        if (Complaint == null) return;
        var newDoc = new Complaint8DDocUpdate
        {
            DocId = Guid.NewGuid().ToString(),
            ComplaintId = Complaint.ComplaintId
        };
        DocUpdates.Add(newDoc);
    }

    private void OnDeleteDocUpdate(Complaint8DDocUpdate? item)
    {
        if (item != null && DocUpdates.Contains(item))
            DocUpdates.Remove(item);
    }

    // 模拟子表数据
    public static List<Complaint8DTeamMember> GetSampleTeamMembers(string complaintId) =>
    [
        new Complaint8DTeamMember { MemberId = "M1", ComplaintId = complaintId, MemberName = "张工程师", Department = "质量部", Role = "组长", JoinDate = DateTime.Now.AddDays(-15) },
        new Complaint8DTeamMember { MemberId = "M2", ComplaintId = complaintId, MemberName = "李工程师", Department = "生产部", Role = "成员", JoinDate = DateTime.Now.AddDays(-14) },
        new Complaint8DTeamMember { MemberId = "M3", ComplaintId = complaintId, MemberName = "王工程师", Department = "技术部", Role = "成员", JoinDate = DateTime.Now.AddDays(-14) },
    ];

    public static List<Complaint8DContainment> GetSampleContainments(string complaintId) =>
    [
        new Complaint8DContainment { ContainmentId = "C1", ComplaintId = complaintId, ActionDescription = "隔离不良品批次", AffectedLot = "LOT-2024-001", AffectedQty = 500, ContainedQty = 500, Disposition = "隔离待处理", ResponsiblePerson = "张工程师", PlanDate = DateTime.Now.AddDays(-14), Status = "Completed" },
    ];

    public static List<Complaint8DRootCause> GetSampleRootCauses(string complaintId) =>
    [
        new Complaint8DRootCause { CauseId = "R1", ComplaintId = complaintId, CauseType = "产生原因", AnalysisMethod = "5Why", Why1 = "设备参数偏移", Why2 = "设备未定期校准", Why3 = "校准计划缺失", ResponsiblePerson = "王工程师", AnalysisDate = DateTime.Now.AddDays(-10) },
    ];

    public static List<Complaint8DAction> GetSampleActions(string complaintId) =>
    [
        new Complaint8DAction { ActionId = "A1", ComplaintId = complaintId, ActionType = "纠正", ActionDescription = "重新校准设备", ResponsiblePerson = "王工程师", PlanDate = DateTime.Now.AddDays(-8), Status = "Completed" },
        new Complaint8DAction { ActionId = "A2", ComplaintId = complaintId, ActionType = "预防", ActionDescription = "制定设备定期校准计划", ResponsiblePerson = "张工程师", PlanDate = DateTime.Now.AddDays(-5), Status = "InProgress" },
    ];

    public static List<Complaint8DDocUpdate> GetSampleDocUpdates(string complaintId) =>
    [
        new Complaint8DDocUpdate { DocId = "D1", ComplaintId = complaintId, DocType = "SOP", DocName = "设备操作规范", DocNo = "SOP-QA-001", UpdateDescription = "增加校准步骤", ResponsiblePerson = "张工程师", Status = "Pending" },
    ];

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        if (navigationContext.Parameters.TryGetValue("ComplaintId", out string? complaintId) && !string.IsNullOrEmpty(complaintId))
        {
            _ = LoadComplaintAsync(complaintId);
        }
    }
}

public class ComplaintAnalysisViewModel : BindableBase
{
    private readonly IComplaintService _service;
    private ObservableCollection<ComplaintInfo> _complaints = [];
    private ObservableCollection<DistributionItem> _defectTypeDistribution = [];
    private ObservableCollection<DistributionItem> _eightDStepDistribution = [];
    private string? _errorMessage;
    private DateTime? _filterDateFrom;
    private DateTime? _filterDateTo;

    public ObservableCollection<ComplaintInfo> Complaints { get => _complaints; set => SetProperty(ref _complaints, value); }
    public ObservableCollection<DistributionItem> DefectTypeDistribution { get => _defectTypeDistribution; set => SetProperty(ref _defectTypeDistribution, value); }
    public ObservableCollection<DistributionItem> EightDStepDistribution { get => _eightDStepDistribution; set => SetProperty(ref _eightDStepDistribution, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public DateTime? FilterDateFrom { get => _filterDateFrom; set { if (SetProperty(ref _filterDateFrom, value)) ApplyFilter(); } }
    public DateTime? FilterDateTo { get => _filterDateTo; set { if (SetProperty(ref _filterDateTo, value)) ApplyFilter(); } }

    public int TotalDefectTypes => Complaints.GroupBy(c => c.DefectType).Count();
    public string TopDefectType => Complaints.GroupBy(c => c.DefectType).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key ?? "无";
    public double AvgOpenDays => Complaints.Any() ? Complaints.Average(c => c.OpenDays) : 0;
    public int TotalCount => Complaints.Count;
    public int OpenCount => Complaints.Count(c => c.Status == "Open");
    public int InProgressCount => Complaints.Count(c => c.Status == "InProgress");
    public int ClosedCount => Complaints.Count(c => c.Status == "Closed");
    public double CloseRate => TotalCount > 0 ? (double)ClosedCount / TotalCount * 100 : 0;

    public DelegateCommand RefreshCommand { get; }

    public ComplaintAnalysisViewModel(IComplaintService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllComplaintsAsync(); Complaints = new ObservableCollection<ComplaintInfo>(items); UpdateStatistics(); }

    private void ApplyFilter()
    {
        var query = Complaints.AsEnumerable();
        if (FilterDateFrom.HasValue) query = query.Where(c => c.ReportDate >= FilterDateFrom.Value);
        if (FilterDateTo.HasValue) query = query.Where(c => c.ReportDate <= FilterDateTo.Value.AddDays(1));
        UpdateDistributions(query.ToList());
        UpdateStatistics();
    }

    private async void OnRefresh() { try { ErrorMessage = null; FilterDateFrom = null; FilterDateTo = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalDefectTypes));
        RaisePropertyChanged(nameof(TopDefectType));
        RaisePropertyChanged(nameof(AvgOpenDays));
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(OpenCount));
        RaisePropertyChanged(nameof(InProgressCount));
        RaisePropertyChanged(nameof(ClosedCount));
        RaisePropertyChanged(nameof(CloseRate));
    }

    private void UpdateDistributions(List<ComplaintInfo> items)
    {
        // 缺陷类型分布
        var defectGroups = items.GroupBy(c => string.IsNullOrEmpty(c.DefectType) ? "未分类" : c.DefectType)
            .OrderByDescending(g => g.Count())
            .Select(g => new DistributionItem { Label = g.Key, Count = g.Count(), Percentage = items.Count > 0 ? (double)g.Count() / items.Count * 100 : 0 })
            .ToList();
        DefectTypeDistribution = new ObservableCollection<DistributionItem>(defectGroups);

        // 8D步骤分布
        var stepGroups = items.GroupBy(c => string.IsNullOrEmpty(c.EightDStatus) ? "D0" : c.EightDStatus)
            .OrderBy(g => g.Key)
            .Select(g => new DistributionItem { Label = g.Key, Count = g.Count(), Percentage = items.Count > 0 ? (double)g.Count() / items.Count * 100 : 0 })
            .ToList();
        EightDStepDistribution = new ObservableCollection<DistributionItem>(stepGroups);
    }
}

public class ComplaintActionViewModel : BindableBase
{
    private readonly IComplaintService _service;
    private readonly IRegionManager? _regionManager;
    private ComplaintInfo? _selectedComplaint;
    private string _selectedEightDStep = "D0";
    private ObservableCollection<ComplaintInfo> _pendingActions = [];
    private ObservableCollection<Complaint8DTeamMember> _teamMembers = [];
    private ObservableCollection<Complaint8DContainment> _containments = [];
    private ObservableCollection<Complaint8DRootCause> _rootCauses = [];
    private ObservableCollection<Complaint8DAction> _actions = [];
    private ObservableCollection<Complaint8DDocUpdate> _docUpdates = [];
    private string? _errorMessage;

    // D0 fields
    private bool _d0AssessmentNeeded;
    private string _d0AssessmentComment = string.Empty;

    // D2 5W2H fields
    private string _d2WhatDescription = string.Empty;
    private string _d2WhoDescription = string.Empty;
    private string _d2WhereDescription = string.Empty;
    private string _d2WhenDescription = string.Empty;
    private string _d2WhyDescription = string.Empty;
    private string _d2HowDescription = string.Empty;
    private string _d2HowManyDescription = string.Empty;
    private string _d2DefectLocation = string.Empty;

    // D3 fields
    private string _d3ContainmentResult = string.Empty;

    // D4 fields
    private string _d4RootCause = string.Empty;

    // D5/D6 fields
    private string _d5CorrectiveAction = string.Empty;
    private string _d6ImplementationResult = string.Empty;

    // D7 fields
    private string _d7PreventionDescription = string.Empty;
    private string _d7HorizontalExpansion = string.Empty;

    // D8 fields
    private string _d8ClosureComment = string.Empty;
    private string _d8TeamRecognition = string.Empty;
    private bool _d8EffectivenessConfirmed;

    // Selected item for sub-table editing
    private Complaint8DTeamMember? _selectedTeamMember;
    private Complaint8DContainment? _selectedContainment;
    private Complaint8DRootCause? _selectedRootCause;
    private Complaint8DAction? _selectedAction;
    private Complaint8DDocUpdate? _selectedDocUpdate;

    public ComplaintInfo? SelectedComplaint { get => _selectedComplaint; set { if (SetProperty(ref _selectedComplaint, value)) OnSelectedComplaintChanged(); } }
    public string SelectedEightDStep { get => _selectedEightDStep; set => SetProperty(ref _selectedEightDStep, value); }
    public ObservableCollection<ComplaintInfo> PendingActions { get => _pendingActions; set => SetProperty(ref _pendingActions, value); }
    public ObservableCollection<Complaint8DTeamMember> TeamMembers { get => _teamMembers; set => SetProperty(ref _teamMembers, value); }
    public ObservableCollection<Complaint8DContainment> Containments { get => _containments; set => SetProperty(ref _containments, value); }
    public ObservableCollection<Complaint8DRootCause> RootCauses { get => _rootCauses; set => SetProperty(ref _rootCauses, value); }
    public ObservableCollection<Complaint8DAction> Actions { get => _actions; set => SetProperty(ref _actions, value); }
    public ObservableCollection<Complaint8DDocUpdate> DocUpdates { get => _docUpdates; set => SetProperty(ref _docUpdates, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    // D0
    public bool D0AssessmentNeeded { get => _d0AssessmentNeeded; set => SetProperty(ref _d0AssessmentNeeded, value); }
    public string D0AssessmentComment { get => _d0AssessmentComment; set => SetProperty(ref _d0AssessmentComment, value); }

    // D2
    public string D2WhatDescription { get => _d2WhatDescription; set => SetProperty(ref _d2WhatDescription, value); }
    public string D2WhoDescription { get => _d2WhoDescription; set => SetProperty(ref _d2WhoDescription, value); }
    public string D2WhereDescription { get => _d2WhereDescription; set => SetProperty(ref _d2WhereDescription, value); }
    public string D2WhenDescription { get => _d2WhenDescription; set => SetProperty(ref _d2WhenDescription, value); }
    public string D2WhyDescription { get => _d2WhyDescription; set => SetProperty(ref _d2WhyDescription, value); }
    public string D2HowDescription { get => _d2HowDescription; set => SetProperty(ref _d2HowDescription, value); }
    public string D2HowManyDescription { get => _d2HowManyDescription; set => SetProperty(ref _d2HowManyDescription, value); }
    public string D2DefectLocation { get => _d2DefectLocation; set => SetProperty(ref _d2DefectLocation, value); }

    // D3
    public string D3ContainmentResult { get => _d3ContainmentResult; set => SetProperty(ref _d3ContainmentResult, value); }

    // D4
    public string D4RootCause { get => _d4RootCause; set => SetProperty(ref _d4RootCause, value); }

    // D5/D6
    public string D5CorrectiveAction { get => _d5CorrectiveAction; set => SetProperty(ref _d5CorrectiveAction, value); }
    public string D6ImplementationResult { get => _d6ImplementationResult; set => SetProperty(ref _d6ImplementationResult, value); }

    // D7
    public string D7PreventionDescription { get => _d7PreventionDescription; set => SetProperty(ref _d7PreventionDescription, value); }
    public string D7HorizontalExpansion { get => _d7HorizontalExpansion; set => SetProperty(ref _d7HorizontalExpansion, value); }

    // D8
    public string D8ClosureComment { get => _d8ClosureComment; set => SetProperty(ref _d8ClosureComment, value); }
    public string D8TeamRecognition { get => _d8TeamRecognition; set => SetProperty(ref _d8TeamRecognition, value); }
    public bool D8EffectivenessConfirmed { get => _d8EffectivenessConfirmed; set => SetProperty(ref _d8EffectivenessConfirmed, value); }

    // Selection
    public Complaint8DTeamMember? SelectedTeamMember { get => _selectedTeamMember; set => SetProperty(ref _selectedTeamMember, value); }
    public Complaint8DContainment? SelectedContainment { get => _selectedContainment; set => SetProperty(ref _selectedContainment, value); }
    public Complaint8DRootCause? SelectedRootCause { get => _selectedRootCause; set => SetProperty(ref _selectedRootCause, value); }
    public Complaint8DAction? SelectedAction { get => _selectedAction; set => SetProperty(ref _selectedAction, value); }
    public Complaint8DDocUpdate? SelectedDocUpdate { get => _selectedDocUpdate; set => SetProperty(ref _selectedDocUpdate, value); }

    // Commands
    public DelegateCommand RefreshCommand { get; }
    public DelegateCommand AdvanceStepCommand { get; }
    public DelegateCommand SubmitForApprovalCommand { get; }
    public DelegateCommand SaveCommand { get; }
    public DelegateCommand GoToPreviousStepCommand { get; }
    public DelegateCommand GoToNextStepCommand { get; }

    // Sub-table commands
    public DelegateCommand AddTeamMemberCommand { get; }
    public DelegateCommand DeleteTeamMemberCommand { get; }
    public DelegateCommand AddContainmentCommand { get; }
    public DelegateCommand DeleteContainmentCommand { get; }
    public DelegateCommand AddRootCauseCommand { get; }
    public DelegateCommand DeleteRootCauseCommand { get; }
    public DelegateCommand AddActionCommand { get; }
    public DelegateCommand DeleteActionCommand { get; }
    public DelegateCommand AddDocUpdateCommand { get; }
    public DelegateCommand DeleteDocUpdateCommand { get; }

    public ComplaintActionViewModel(IComplaintService service, IRegionManager? regionManager = null)
    {
        _service = service;
        _regionManager = regionManager;
        RefreshCommand = new DelegateCommand(OnRefresh);
        AdvanceStepCommand = new DelegateCommand(OnAdvanceStep, CanAdvanceStep);
        SubmitForApprovalCommand = new DelegateCommand(OnSubmitForApproval, () => SelectedComplaint != null);
        SaveCommand = new DelegateCommand(OnSave, () => SelectedComplaint != null);
        GoToPreviousStepCommand = new DelegateCommand(OnGoToPreviousStep, CanGoPrevious);
        GoToNextStepCommand = new DelegateCommand(OnGoToNextStep, CanGoNext);
        AddTeamMemberCommand = new DelegateCommand(OnAddTeamMember);
        DeleteTeamMemberCommand = new DelegateCommand(OnDeleteTeamMember);
        AddContainmentCommand = new DelegateCommand(OnAddContainment);
        DeleteContainmentCommand = new DelegateCommand(OnDeleteContainment);
        AddRootCauseCommand = new DelegateCommand(OnAddRootCause);
        DeleteRootCauseCommand = new DelegateCommand(OnDeleteRootCause);
        AddActionCommand = new DelegateCommand(OnAddAction);
        DeleteActionCommand = new DelegateCommand(OnDeleteAction);
        AddDocUpdateCommand = new DelegateCommand(OnAddDocUpdate);
        DeleteDocUpdateCommand = new DelegateCommand(OnDeleteDocUpdate);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync() { var items = await _service.GetAllComplaintsAsync(); PendingActions = new ObservableCollection<ComplaintInfo>(items.Where(c => c.Status != "Closed").OrderByDescending(c => c.ReportDate)); }

    private void OnSelectedComplaintChanged()
    {
        if (SelectedComplaint == null) return;
        LoadComplaintData();
        AdvanceStepCommand.RaiseCanExecuteChanged();
    }

    private void LoadComplaintData()
    {
        if (SelectedComplaint == null) return;
        ErrorMessage = null;

        SelectedEightDStep = SelectedComplaint.EightDStatus ?? "D0";

        D0AssessmentNeeded = SelectedComplaint.D0AssessmentNeeded;
        D0AssessmentComment = SelectedComplaint.D0AssessmentComment;
        D2WhatDescription = SelectedComplaint.D2WhatDescription;
        D2WhoDescription = SelectedComplaint.D2WhoDescription;
        D2WhereDescription = SelectedComplaint.D2WhereDescription;
        D2WhenDescription = SelectedComplaint.D2WhenDescription;
        D2WhyDescription = SelectedComplaint.D2WhyDescription;
        D2HowDescription = SelectedComplaint.D2HowDescription;
        D2HowManyDescription = SelectedComplaint.D2HowManyDescription;
        D2DefectLocation = SelectedComplaint.D2DefectLocation;
        D3ContainmentResult = SelectedComplaint.D3ContainmentResult;
        D4RootCause = SelectedComplaint.D4RootCause;
        D5CorrectiveAction = SelectedComplaint.D5CorrectiveAction;
        D6ImplementationResult = SelectedComplaint.D6ImplementationResult;
        D7PreventionDescription = SelectedComplaint.D7PreventionDescription;
        D7HorizontalExpansion = SelectedComplaint.D7HorizontalExpansion;
        D8ClosureComment = SelectedComplaint.D8ClosureComment;
        D8TeamRecognition = SelectedComplaint.D8TeamRecognition;
        D8EffectivenessConfirmed = SelectedComplaint.D8EffectivenessConfirmed;

        // Load sub-tables
        var cid = SelectedComplaint.ComplaintId;
        TeamMembers = new ObservableCollection<Complaint8DTeamMember>(ComplaintDetailViewModel.GetSampleTeamMembers(cid));
        Containments = new ObservableCollection<Complaint8DContainment>(ComplaintDetailViewModel.GetSampleContainments(cid));
        RootCauses = new ObservableCollection<Complaint8DRootCause>(ComplaintDetailViewModel.GetSampleRootCauses(cid));
        Actions = new ObservableCollection<Complaint8DAction>(ComplaintDetailViewModel.GetSampleActions(cid));
        DocUpdates = new ObservableCollection<Complaint8DDocUpdate>(ComplaintDetailViewModel.GetSampleDocUpdates(cid));
    }

    private async void OnRefresh() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }

    private async void OnAdvanceStep()
    {
        if (SelectedComplaint == null) return;
        var steps = new[] { "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8" };
        var idx = Array.IndexOf(steps, SelectedEightDStep);
        if (idx < 0 || idx >= steps.Length - 1) return;

        var nextStep = steps[idx + 1];
        try
        {
            await _service.UpdateEightDStepAsync(SelectedComplaint.ComplaintId, SelectedEightDStep, "步骤已完成");
            SelectedComplaint.EightDStatus = nextStep;
            SelectedEightDStep = nextStep;
            await _service.SaveComplaintAsync(SelectedComplaint);
            await ReloadDataAsync();
            AdvanceStepCommand.RaiseCanExecuteChanged();
            GoToNextStepCommand.RaiseCanExecuteChanged();
            GoToPreviousStepCommand.RaiseCanExecuteChanged();
        }
        catch (Exception ex) { ErrorMessage = $"推进步骤失败: {ex.Message}"; }
    }

    private bool CanAdvanceStep() => SelectedComplaint != null && SelectedComplaint.Status != "Closed" && SelectedEightDStep != "D8";

    private async void OnSubmitForApproval()
    {
        if (SelectedComplaint == null) return;
        try
        {
            SaveCurrentStepData();
            SelectedComplaint.ApprovalStatus = "Submitted";
            await _service.SaveComplaintAsync(SelectedComplaint);
            ErrorMessage = "已提交审批";
        }
        catch (Exception ex) { ErrorMessage = $"提交审批失败: {ex.Message}"; }
    }

    private async void OnSave()
    {
        if (SelectedComplaint == null) return;
        try
        {
            SaveCurrentStepData();
            await _service.SaveComplaintAsync(SelectedComplaint);
            ErrorMessage = "保存成功";
        }
        catch (Exception ex) { ErrorMessage = $"保存失败: {ex.Message}"; }
    }

    private void SaveCurrentStepData()
    {
        if (SelectedComplaint == null) return;
        SelectedComplaint.D0AssessmentNeeded = D0AssessmentNeeded;
        SelectedComplaint.D0AssessmentComment = D0AssessmentComment;
        SelectedComplaint.D2WhatDescription = D2WhatDescription;
        SelectedComplaint.D2WhoDescription = D2WhoDescription;
        SelectedComplaint.D2WhereDescription = D2WhereDescription;
        SelectedComplaint.D2WhenDescription = D2WhenDescription;
        SelectedComplaint.D2WhyDescription = D2WhyDescription;
        SelectedComplaint.D2HowDescription = D2HowDescription;
        SelectedComplaint.D2HowManyDescription = D2HowManyDescription;
        SelectedComplaint.D2DefectLocation = D2DefectLocation;
        SelectedComplaint.D3ContainmentResult = D3ContainmentResult;
        SelectedComplaint.D4RootCause = D4RootCause;
        SelectedComplaint.D5CorrectiveAction = D5CorrectiveAction;
        SelectedComplaint.D6ImplementationResult = D6ImplementationResult;
        SelectedComplaint.D7PreventionDescription = D7PreventionDescription;
        SelectedComplaint.D7HorizontalExpansion = D7HorizontalExpansion;
        SelectedComplaint.D8ClosureComment = D8ClosureComment;
        SelectedComplaint.D8TeamRecognition = D8TeamRecognition;
        SelectedComplaint.D8EffectivenessConfirmed = D8EffectivenessConfirmed;
    }

    private void OnGoToPreviousStep()
    {
        var steps = new[] { "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8" };
        var idx = Array.IndexOf(steps, SelectedEightDStep);
        if (idx > 0)
        {
            SelectedEightDStep = steps[idx - 1];
            GoToPreviousStepCommand.RaiseCanExecuteChanged();
            GoToNextStepCommand.RaiseCanExecuteChanged();
        }
    }

    private void OnGoToNextStep()
    {
        var steps = new[] { "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8" };
        var idx = Array.IndexOf(steps, SelectedEightDStep);
        if (idx < steps.Length - 1)
        {
            SelectedEightDStep = steps[idx + 1];
            GoToPreviousStepCommand.RaiseCanExecuteChanged();
            GoToNextStepCommand.RaiseCanExecuteChanged();
        }
    }

    private bool CanGoPrevious()
    {
        var steps = new[] { "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8" };
        return Array.IndexOf(steps, SelectedEightDStep) > 0;
    }

    private bool CanGoNext()
    {
        var steps = new[] { "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8" };
        return Array.IndexOf(steps, SelectedEightDStep) < steps.Length - 1;
    }

    // Sub-table operations
    private void OnAddTeamMember()
    {
        if (SelectedComplaint == null) return;
        TeamMembers.Add(new Complaint8DTeamMember { MemberId = Guid.NewGuid().ToString(), ComplaintId = SelectedComplaint.ComplaintId, JoinDate = DateTime.Now });
    }

    private void OnDeleteTeamMember()
    {
        if (SelectedTeamMember != null && TeamMembers.Contains(SelectedTeamMember))
            TeamMembers.Remove(SelectedTeamMember);
    }

    private void OnAddContainment()
    {
        if (SelectedComplaint == null) return;
        Containments.Add(new Complaint8DContainment { ContainmentId = Guid.NewGuid().ToString(), ComplaintId = SelectedComplaint.ComplaintId });
    }

    private void OnDeleteContainment()
    {
        if (SelectedContainment != null && Containments.Contains(SelectedContainment))
            Containments.Remove(SelectedContainment);
    }

    private void OnAddRootCause()
    {
        if (SelectedComplaint == null) return;
        RootCauses.Add(new Complaint8DRootCause { CauseId = Guid.NewGuid().ToString(), ComplaintId = SelectedComplaint.ComplaintId });
    }

    private void OnDeleteRootCause()
    {
        if (SelectedRootCause != null && RootCauses.Contains(SelectedRootCause))
            RootCauses.Remove(SelectedRootCause);
    }

    private void OnAddAction()
    {
        if (SelectedComplaint == null) return;
        Actions.Add(new Complaint8DAction { ActionId = Guid.NewGuid().ToString(), ComplaintId = SelectedComplaint.ComplaintId });
    }

    private void OnDeleteAction()
    {
        if (SelectedAction != null && Actions.Contains(SelectedAction))
            Actions.Remove(SelectedAction);
    }

    private void OnAddDocUpdate()
    {
        if (SelectedComplaint == null) return;
        DocUpdates.Add(new Complaint8DDocUpdate { DocId = Guid.NewGuid().ToString(), ComplaintId = SelectedComplaint.ComplaintId });
    }

    private void OnDeleteDocUpdate()
    {
        if (SelectedDocUpdate != null && DocUpdates.Contains(SelectedDocUpdate))
            DocUpdates.Remove(SelectedDocUpdate);
    }
}

public class ComplaintReportViewModel : BindableBase
{
    private readonly IComplaintService _service;
    private CustomerQualityReport? _currentReport;
    private ComplaintInfo? _selectedComplaint;
    private ObservableCollection<ComplaintInfo> _complaints = [];
    private ObservableCollection<EightDStep> _reportEightDSteps = [];
    private string? _errorMessage;

    public CustomerQualityReport? CurrentReport { get => _currentReport; set => SetProperty(ref _currentReport, value); }
    public ComplaintInfo? SelectedComplaint { get => _selectedComplaint; set { if (SetProperty(ref _selectedComplaint, value)) RaisePropertyChanged(nameof(CanGenerateReport)); } }
    public ObservableCollection<ComplaintInfo> Complaints { get => _complaints; set => SetProperty(ref _complaints, value); }
    public ObservableCollection<EightDStep> ReportEightDSteps { get => _reportEightDSteps; set => SetProperty(ref _reportEightDSteps, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool CanGenerateReport => SelectedComplaint != null;

    public DelegateCommand GenerateReportCommand { get; }
    public DelegateCommand RefreshCommand { get; }

    public ComplaintReportViewModel(IComplaintService service)
    {
        _service = service;
        GenerateReportCommand = new DelegateCommand(OnGenerateReport, () => CanGenerateReport);
        RefreshCommand = new DelegateCommand(OnRefresh);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            ErrorMessage = null;
            var items = await _service.GetAllComplaintsAsync();
            Complaints = new ObservableCollection<ComplaintInfo>(items.OrderByDescending(c => c.ReportDate));
        }
        catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; }
    }

    private async void OnGenerateReport()
    {
        if (SelectedComplaint == null) return;
        try
        {
            ErrorMessage = null;
            CurrentReport = await _service.GenerateQualityReportAsync(SelectedComplaint.ComplaintId);
            var steps = await _service.GetEightDStepsAsync(SelectedComplaint.ComplaintId);
            ReportEightDSteps = new ObservableCollection<EightDStep>(steps);
        }
        catch (Exception ex) { ErrorMessage = $"生成失败: {ex.Message}"; }
    }

    private async void OnRefresh()
    {
        try { ErrorMessage = null; await InitializeAsync(); }
        catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; }
    }
}

public class ComplaintStatisticsViewModel : BindableBase
{
    private readonly IComplaintService _service;
    private ObservableCollection<ComplaintInfo> _complaints = [];
    private ObservableCollection<DefectTypeStatItem> _defectTypeStats = [];
    private string? _errorMessage;
    private double _maxBarWidth = 300;

    public ObservableCollection<ComplaintInfo> Complaints { get => _complaints; set => SetProperty(ref _complaints, value); }
    public ObservableCollection<DefectTypeStatItem> DefectTypeStats { get => _defectTypeStats; set => SetProperty(ref _defectTypeStats, value); }
    public string? ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public double MaxBarWidth { get => _maxBarWidth; set => SetProperty(ref _maxBarWidth, value); }

    // 基本统计
    public int TotalCount => Complaints.Count;
    public int OpenCount => Complaints.Count(c => c.Status == "Open");
    public int InProgressCount => Complaints.Count(c => c.Status == "InProgress");
    public int ClosedCount => Complaints.Count(c => c.Status == "Closed");
    public int OverdueCount => Complaints.Count(c => c.RequiredDate.HasValue && c.RequiredDate.Value < DateTime.Now && c.Status != "Closed");
    public double CloseRate => TotalCount > 0 ? (double)ClosedCount / TotalCount * 100 : 0;
    public double AvgOpenDays => Complaints.Any() ? Complaints.Average(c => c.OpenDays) : 0;
    public int HighSeverityCount => Complaints.Count(c => c.Severity == "High" || c.Severity == "Critical");

    // 百分比
    public double OpenPercent => TotalCount > 0 ? (double)OpenCount / TotalCount * 100 : 0;
    public double InProgressPercent => TotalCount > 0 ? (double)InProgressCount / TotalCount * 100 : 0;
    public double ClosedPercent => TotalCount > 0 ? (double)ClosedCount / TotalCount * 100 : 0;

    // 进度条宽度
    public double OpenBarWidth => TotalCount > 0 ? (double)OpenCount / TotalCount * MaxBarWidth : 0;
    public double InProgressBarWidth => TotalCount > 0 ? (double)InProgressCount / TotalCount * MaxBarWidth : 0;
    public double ClosedBarWidth => TotalCount > 0 ? (double)ClosedCount / TotalCount * MaxBarWidth : 0;

    // 严重度统计
    public int CriticalCount => Complaints.Count(c => c.Severity == "Critical");
    public int HighCount => Complaints.Count(c => c.Severity == "High");
    public int MediumCount => Complaints.Count(c => c.Severity == "Medium");
    public int LowCount => Complaints.Count(c => c.Severity == "Low");
    public double CriticalPercent => TotalCount > 0 ? (double)CriticalCount / TotalCount * 100 : 0;
    public double HighPercent => TotalCount > 0 ? (double)HighCount / TotalCount * 100 : 0;
    public double MediumPercent => TotalCount > 0 ? (double)MediumCount / TotalCount * 100 : 0;
    public double LowPercent => TotalCount > 0 ? (double)LowCount / TotalCount * 100 : 0;

    public DelegateCommand RefreshCommand { get; }

    public ComplaintStatisticsViewModel(IComplaintService service)
    {
        _service = service;
        RefreshCommand = new DelegateCommand(OnRefresh);
        _ = InitializeAsync();
    }

    private async Task InitializeAsync() { try { await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"加载失败: {ex.Message}"; } }
    private async Task ReloadDataAsync()
    {
        var items = await _service.GetAllComplaintsAsync();
        Complaints = new ObservableCollection<ComplaintInfo>(items);
        CalculateDefectTypeStats();
        UpdateStatistics();
    }
    private async void OnRefresh() { try { ErrorMessage = null; await ReloadDataAsync(); } catch (Exception ex) { ErrorMessage = $"刷新失败: {ex.Message}"; } }

    private void CalculateDefectTypeStats()
    {
        var stats = Complaints
            .GroupBy(c => c.DefectType)
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .Select(g => new DefectTypeStatItem
            {
                DefectType = g.Key,
                Count = g.Count(),
                Percentage = TotalCount > 0 ? (double)g.Count() / TotalCount * 100 : 0,
                AffectedCustomers = string.Join(", ", g.Select(c => c.CustomerName).Distinct().Take(3)),
                AffectedProducts = string.Join(", ", g.Select(c => c.ProductName).Distinct().Take(3))
            })
            .OrderByDescending(s => s.Count)
            .Take(10)
            .ToList();

        DefectTypeStats = new ObservableCollection<DefectTypeStatItem>(stats);
    }

    private void UpdateStatistics()
    {
        RaisePropertyChanged(nameof(TotalCount));
        RaisePropertyChanged(nameof(OpenCount));
        RaisePropertyChanged(nameof(InProgressCount));
        RaisePropertyChanged(nameof(ClosedCount));
        RaisePropertyChanged(nameof(OverdueCount));
        RaisePropertyChanged(nameof(CloseRate));
        RaisePropertyChanged(nameof(AvgOpenDays));
        RaisePropertyChanged(nameof(HighSeverityCount));
        RaisePropertyChanged(nameof(OpenPercent));
        RaisePropertyChanged(nameof(InProgressPercent));
        RaisePropertyChanged(nameof(ClosedPercent));
        RaisePropertyChanged(nameof(OpenBarWidth));
        RaisePropertyChanged(nameof(InProgressBarWidth));
        RaisePropertyChanged(nameof(ClosedBarWidth));
        RaisePropertyChanged(nameof(CriticalCount));
        RaisePropertyChanged(nameof(HighCount));
        RaisePropertyChanged(nameof(MediumCount));
        RaisePropertyChanged(nameof(LowCount));
        RaisePropertyChanged(nameof(CriticalPercent));
        RaisePropertyChanged(nameof(HighPercent));
        RaisePropertyChanged(nameof(MediumPercent));
        RaisePropertyChanged(nameof(LowPercent));
    }
}
