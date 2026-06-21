using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MES.Contracts.Common;
using MES.Contracts.Engineering;
using MES.Modules.EngineeringChange.Models;

namespace MES.Modules.EngineeringChange.Services;

/// <summary>
/// Interface for Engineering Change Notice operations.
/// </summary>
public interface IECNService
{
    // Main CRUD
    Task<List<ECNInfo>> GetAllECNsAsync();
    Task<ECNInfo?> GetECNAsync(string ecnId);
    Task SaveECNAsync(ECNInfo ecn);
    Task DeleteECNAsync(string ecnId);
    Task UpdateECNStatusAsync(string ecnId, string status);
    Task<List<ECNHistory>> GetECNHistoryAsync(string ecnId);
    Task SubmitForApprovalAsync(string ecnId);
    Task ApproveECNAsync(string ecnId, string approver);
    Task RejectECNAsync(string ecnId, string reason);

    // Statistics
    Task<EcnStatistics> GetStatisticsAsync();

    // Sub-table: EcnItem
    Task<List<EcnItem>> GetItemsAsync(string ecnId);
    Task SaveItemAsync(EcnItem item);
    Task DeleteItemAsync(string itemId);

    // Sub-table: EcnImpact
    Task<List<EcnImpact>> GetImpactsAsync(string ecnId);
    Task SaveImpactAsync(EcnImpact impact);
    Task DeleteImpactAsync(string impactId);

    // Sub-table: EcnApprover
    Task<List<EcnApprover>> GetApproversAsync(string ecnId);
    Task SaveApproverAsync(EcnApprover approver);
    Task DeleteApproverAsync(string approverId);

    // Sub-table: EcnNotifyDept
    Task<List<EcnNotifyDept>> GetNotifyDeptsAsync(string ecnId);
    Task SaveNotifyDeptAsync(EcnNotifyDept dept);

    // Sub-table: EcnImplement
    Task<List<EcnImplement>> GetImplementsAsync(string ecnId);
    Task SaveImplementAsync(EcnImplement impl);
    Task UpdateImplementAsync(EcnImplement impl);
    Task DeleteImplementAsync(string implementId);
}

/// <summary>
/// REST API client service for Engineering Change Notice operations.
/// Communicates with the backend EngineeringChangeController.
/// </summary>
public class ECNService : IECNService
{
    private readonly HttpClient _httpClient;

    public ECNService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ============================================================================
    // Main CRUD
    // ============================================================================

    public async Task<List<ECNInfo>> GetAllECNsAsync()
    {
        var response = await _httpClient.GetAsync("EngineeringChange");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<EcnRequestDto>>>();
        return apiResponse?.Data?.Items.Select(MapToECNInfo).ToList() ?? new List<ECNInfo>();
    }

    public async Task<ECNInfo?> GetECNAsync(string ecnId)
    {
        var response = await _httpClient.GetAsync($"EngineeringChange/{ecnId}");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EcnRequestDto>>();
        return apiResponse?.Data != null ? MapToECNInfo(apiResponse.Data) : null;
    }

    public async Task SaveECNAsync(ECNInfo ecn)
    {
        if (string.IsNullOrEmpty(ecn.EcnId) || ecn.EcnId == "0")
        {
            // Create new ECN
            var request = new CreateEcnRequest
            {
                EcnTitle = ecn.EcnTitle,
                EcnType = ecn.EcnType,
                ChangeCategory = ecn.ChangeCategory,
                Reason = ecn.Reason,
                ChangeDescription = ecn.ChangeDescription,
                ChangeContent = ecn.ChangeContent,
                OldValue = ecn.OldValue,
                NewValue = ecn.NewValue,
                AffectedRoutes = ecn.AffectedRoutes,
                AffectedProducts = ecn.AffectedProducts,
                ImpactAssessment = ecn.ImpactAssessment,
                Urgency = ecn.Urgency,
                RiskLevel = ecn.RiskLevel,
                PlannedDate = ecn.PlannedDate,
                CostEstimate = ecn.CostEstimate,
                IsUrgent = ecn.IsUrgent,
                Remark = ecn.Remark,
            };
            var response = await _httpClient.PostAsJsonAsync("EngineeringChange", request);
            response.EnsureSuccessStatusCode();
        }
        else
        {
            // Update existing ECN
            var request = new UpdateEcnRequest
            {
                EcnId = ecn.EcnId,
                EcnTitle = ecn.EcnTitle,
                Reason = ecn.Reason,
                ChangeDescription = ecn.ChangeDescription,
                ImpactAssessment = ecn.ImpactAssessment,
                PlannedDate = ecn.PlannedDate,
                Remark = ecn.Remark,
            };
            var response = await _httpClient.PutAsJsonAsync($"EngineeringChange/{ecn.EcnId}", request);
            response.EnsureSuccessStatusCode();
        }
    }

    public async Task DeleteECNAsync(string ecnId)
    {
        var response = await _httpClient.DeleteAsync($"EngineeringChange/{ecnId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateECNStatusAsync(string ecnId, string status)
    {
        var request = new { TargetStatus = status, Comments = "" };
        var response = await _httpClient.PostAsJsonAsync($"EngineeringChange/{ecnId}/advance", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ECNHistory>> GetECNHistoryAsync(string ecnId)
    {
        // History is not a separate endpoint in backend. We return empty list.
        // If a history endpoint is added later, map it here.
        await Task.CompletedTask;
        return new List<ECNHistory>();
    }

    public async Task SubmitForApprovalAsync(string ecnId)
    {
        var response = await _httpClient.PostAsync($"EngineeringChange/{ecnId}/submit", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task ApproveECNAsync(string ecnId, string approver)
    {
        var request = new EcnApprovalRequest
        {
            EcnId = ecnId,
            Approved = true,
            Comments = $"Approved by {approver}",
        };
        var response = await _httpClient.PostAsJsonAsync($"EngineeringChange/{ecnId}/approve", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task RejectECNAsync(string ecnId, string reason)
    {
        var request = new EcnApprovalRequest
        {
            EcnId = ecnId,
            Approved = false,
            RejectReason = reason,
        };
        var response = await _httpClient.PostAsJsonAsync($"EngineeringChange/{ecnId}/approve", request);
        response.EnsureSuccessStatusCode();
    }

    // ============================================================================
    // Statistics
    // ============================================================================

    public async Task<EcnStatistics> GetStatisticsAsync()
    {
        var response = await _httpClient.GetAsync("EngineeringChange/statistics");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<EcnStatisticsDto>>();
        return apiResponse?.Data != null ? MapToEcnStatistics(apiResponse.Data) : new EcnStatistics();
    }

    // ============================================================================
    // Sub-table: EcnItem
    // ============================================================================

    public async Task<List<EcnItem>> GetItemsAsync(string ecnId)
    {
        var response = await _httpClient.GetAsync($"EngineeringChange/{ecnId}/items");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<EcnItemDto>>>();
        return apiResponse?.Data?.Select(MapToEcnItem).ToList() ?? new List<EcnItem>();
    }

    public async Task SaveItemAsync(EcnItem item)
    {
        if (string.IsNullOrEmpty(item.ItemId) || item.ItemId == "0")
        {
            var request = new CreateEcnItemRequest
            {
                EcnId = item.EcnId,
                ItemType = item.ItemType,
                ItemCode = item.ItemCode,
                ItemName = item.ItemName,
                OldValue = item.OldValue,
                NewValue = item.NewValue,
                ChangeReason = item.ChangeReason,
                Remark = item.Remark,
                SortOrder = item.SortOrder,
            };
            var response = await _httpClient.PostAsJsonAsync("EngineeringChange/items", request);
            response.EnsureSuccessStatusCode();
        }
        else
        {
            var request = new CreateEcnItemRequest
            {
                EcnId = item.EcnId,
                ItemType = item.ItemType,
                ItemCode = item.ItemCode,
                ItemName = item.ItemName,
                OldValue = item.OldValue,
                NewValue = item.NewValue,
                ChangeReason = item.ChangeReason,
                Remark = item.Remark,
                SortOrder = item.SortOrder,
            };
            var response = await _httpClient.PutAsJsonAsync($"EngineeringChange/items/{item.ItemId}", request);
            response.EnsureSuccessStatusCode();
        }
    }

    public async Task DeleteItemAsync(string itemId)
    {
        var response = await _httpClient.DeleteAsync($"EngineeringChange/items/{itemId}");
        response.EnsureSuccessStatusCode();
    }

    // ============================================================================
    // Sub-table: EcnImpact
    // ============================================================================

    public async Task<List<EcnImpact>> GetImpactsAsync(string ecnId)
    {
        var response = await _httpClient.GetAsync($"EngineeringChange/{ecnId}/impacts");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<EcnImpactItemDto>>>();
        return apiResponse?.Data?.Select(MapToEcnImpact).ToList() ?? new List<EcnImpact>();
    }

    public async Task SaveImpactAsync(EcnImpact impact)
    {
        var request = new CreateEcnImpactItemRequest
        {
            EcnId = impact.EcnId,
            ImpactType = impact.ImpactType,
            Severity = impact.Severity,
            Description = impact.Description,
            ImpactAnalysis = impact.ImpactAnalysis,
            Action = impact.Action,
            Responsible = impact.Responsible,
            DueDate = impact.DueDate,
        };
        var response = await _httpClient.PostAsJsonAsync("EngineeringChange/impacts", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteImpactAsync(string impactId)
    {
        var response = await _httpClient.DeleteAsync($"EngineeringChange/impacts/{impactId}");
        response.EnsureSuccessStatusCode();
    }

    // ============================================================================
    // Sub-table: EcnApprover
    // ============================================================================

    public async Task<List<EcnApprover>> GetApproversAsync(string ecnId)
    {
        var response = await _httpClient.GetAsync($"EngineeringChange/{ecnId}/approvers");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<EcnApproverDto>>>();
        return apiResponse?.Data?.Select(MapToEcnApprover).ToList() ?? new List<EcnApprover>();
    }

    public async Task SaveApproverAsync(EcnApprover approver)
    {
        if (string.IsNullOrEmpty(approver.ApproverId) || approver.ApproverId == "0")
        {
            var request = new CreateEcnApproverRequest
            {
                EcnId = approver.EcnId,
                ApproverName = approver.ApproverName,
                Role = approver.Role,
                ApprovalOrder = approver.ApprovalOrder,
            };
            var response = await _httpClient.PostAsJsonAsync("EngineeringChange/approvers", request);
            response.EnsureSuccessStatusCode();
        }
        else
        {
            var request = new UpdateEcnApproverRequest
            {
                ApproverId = approver.ApproverId,
                Status = approver.Status,
                Result = approver.Result,
                Comments = approver.Comments,
            };
            var response = await _httpClient.PutAsJsonAsync($"EngineeringChange/approvers/{approver.ApproverId}", request);
            response.EnsureSuccessStatusCode();
        }
    }

    public async Task DeleteApproverAsync(string approverId)
    {
        var response = await _httpClient.DeleteAsync($"EngineeringChange/approvers/{approverId}");
        response.EnsureSuccessStatusCode();
    }

    // ============================================================================
    // Sub-table: EcnNotifyDept
    // ============================================================================

    public async Task<List<EcnNotifyDept>> GetNotifyDeptsAsync(string ecnId)
    {
        var response = await _httpClient.GetAsync($"EngineeringChange/{ecnId}/notify-depts");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<EcnNotifyDeptDto>>>();
        return apiResponse?.Data?.Select(MapToEcnNotifyDept).ToList() ?? new List<EcnNotifyDept>();
    }

    public async Task SaveNotifyDeptAsync(EcnNotifyDept dept)
    {
        var request = new CreateEcnNotifyDeptRequest
        {
            EcnId = dept.EcnId,
            DeptId = dept.DeptId,
            DeptName = dept.DeptName,
        };
        var response = await _httpClient.PostAsJsonAsync("EngineeringChange/notify-depts", request);
        response.EnsureSuccessStatusCode();
    }

    // ============================================================================
    // Sub-table: EcnImplement
    // ============================================================================

    public async Task<List<EcnImplement>> GetImplementsAsync(string ecnId)
    {
        var response = await _httpClient.GetAsync($"EngineeringChange/{ecnId}/implements");
        response.EnsureSuccessStatusCode();
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<EcnImplementDto>>>();
        return apiResponse?.Data?.Select(MapToEcnImplement).ToList() ?? new List<EcnImplement>();
    }

    public async Task SaveImplementAsync(EcnImplement impl)
    {
        var request = new CreateEcnImplementRequest
        {
            EcnId = impl.EcnId,
            TaskName = impl.TaskName,
            Description = impl.Description,
            Responsible = impl.Responsible,
            PlanDate = impl.PlanDate,
            Remark = impl.Remark,
        };
        var response = await _httpClient.PostAsJsonAsync("EngineeringChange/implements", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateImplementAsync(EcnImplement impl)
    {
        var request = new UpdateEcnImplementRequest
        {
            ImplementId = impl.ImplementId,
            Status = impl.Status,
            ActualDate = impl.ActualDate,
            Result = impl.Result,
            Remark = impl.Remark,
        };
        var response = await _httpClient.PutAsJsonAsync($"EngineeringChange/implements/{impl.ImplementId}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteImplementAsync(string implementId)
    {
        var response = await _httpClient.DeleteAsync($"EngineeringChange/implements/{implementId}");
        response.EnsureSuccessStatusCode();
    }

    // ============================================================================
    // Mapping helpers: Backend DTO -> Client Model
    // ============================================================================

    private static ECNInfo MapToECNInfo(EcnRequestDto dto) => new()
    {
        EcnId = dto.EcnId ?? string.Empty,
        EcnNo = dto.EcnNo ?? string.Empty,
        EcnTitle = dto.EcnTitle ?? string.Empty,
        EcnType = dto.EcnType ?? string.Empty,
        ChangeCategory = dto.ChangeCategory ?? string.Empty,
        Reason = dto.Reason ?? string.Empty,
        ChangeDescription = dto.ChangeDescription ?? string.Empty,
        ChangeContent = dto.ChangeContent ?? string.Empty,
        OldValue = dto.OldValue ?? string.Empty,
        NewValue = dto.NewValue ?? string.Empty,
        Status = dto.Status ?? string.Empty,
        AffectedRoutes = dto.AffectedRoutes ?? string.Empty,
        AffectedProducts = dto.AffectedProducts ?? string.Empty,
        ImpactAssessment = dto.ImpactAssessment ?? string.Empty,
        Urgency = dto.Urgency ?? string.Empty,
        RiskLevel = dto.RiskLevel ?? string.Empty,
        ReviewComments = dto.ReviewComments ?? string.Empty,
        RejectReason = dto.RejectReason ?? string.Empty,
        VerifyResult = dto.VerifyResult ?? string.Empty,
        RequestedBy = dto.RequestedBy ?? string.Empty,
        RequestedAt = dto.RequestedAt,
        ApprovedBy = dto.ApprovedBy ?? string.Empty,
        ApprovedAt = dto.ApprovedAt,
        EffectiveDate = dto.EffectiveDate,
        PlannedDate = dto.PlannedDate,
        ActualDate = dto.ActualDate,
        IsComplete = dto.IsComplete,
        CloseDate = dto.CloseDate,
        DaysElapsed = dto.DaysElapsed,
        OAFlowId = dto.OAFlowId ?? string.Empty,
        OANo = dto.OANo ?? string.Empty,
        IsUrgent = dto.IsUrgent,
        CostEstimate = dto.CostEstimate,
        Remark = dto.Remark ?? string.Empty,
        CreatedAt = dto.CreatedAt,
        CreatedBy = dto.CreatedBy ?? string.Empty,
        UpdatedAt = dto.UpdatedAt,
        UpdatedBy = dto.UpdatedBy ?? string.Empty,
        Items = dto.Items?.Select(MapToEcnItem).ToList() ?? new List<EcnItem>(),
        Impacts = dto.Impacts?.Select(MapToEcnImpact).ToList() ?? new List<EcnImpact>(),
        Approvers = dto.Approvers?.Select(MapToEcnApprover).ToList() ?? new List<EcnApprover>(),
        NotifyDepts = dto.NotifyDepts?.Select(MapToEcnNotifyDept).ToList() ?? new List<EcnNotifyDept>(),
        Implements = dto.Implements?.Select(MapToEcnImplement).ToList() ?? new List<EcnImplement>(),
    };

    private static EcnItem MapToEcnItem(EcnItemDto dto) => new()
    {
        ItemId = dto.ItemId ?? string.Empty,
        EcnId = dto.EcnId ?? string.Empty,
        ItemType = dto.ItemType ?? string.Empty,
        ItemCode = dto.ItemCode ?? string.Empty,
        ItemName = dto.ItemName ?? string.Empty,
        OldValue = dto.OldValue ?? string.Empty,
        NewValue = dto.NewValue ?? string.Empty,
        ChangeReason = dto.ChangeReason ?? string.Empty,
        Remark = dto.Remark ?? string.Empty,
        SortOrder = dto.SortOrder,
        CreatedAt = dto.CreatedAt,
        CreatedBy = dto.CreatedBy ?? string.Empty,
        UpdatedAt = dto.UpdatedAt,
        UpdatedBy = dto.UpdatedBy ?? string.Empty,
    };

    private static EcnImpact MapToEcnImpact(EcnImpactItemDto dto) => new()
    {
        ImpactId = dto.ImpactId ?? string.Empty,
        EcnId = dto.EcnId ?? string.Empty,
        ImpactType = dto.ImpactType ?? string.Empty,
        Severity = dto.Severity ?? string.Empty,
        Description = dto.Description ?? string.Empty,
        ImpactAnalysis = dto.ImpactAnalysis ?? string.Empty,
        Action = dto.Action ?? string.Empty,
        Responsible = dto.Responsible ?? string.Empty,
        DueDate = dto.DueDate,
        CreatedAt = dto.CreatedAt,
        CreatedBy = dto.CreatedBy ?? string.Empty,
    };

    private static EcnApprover MapToEcnApprover(EcnApproverDto dto) => new()
    {
        ApproverId = dto.ApproverId ?? string.Empty,
        EcnId = dto.EcnId ?? string.Empty,
        ApproverName = dto.ApproverName ?? string.Empty,
        Role = dto.Role ?? string.Empty,
        ApprovalOrder = dto.ApprovalOrder,
        Status = dto.Status ?? string.Empty,
        Result = dto.Result ?? string.Empty,
        Comments = dto.Comments ?? string.Empty,
        ApprovedAt = dto.ApprovedAt,
        CreatedAt = dto.CreatedAt,
    };

    private static EcnNotifyDept MapToEcnNotifyDept(EcnNotifyDeptDto dto) => new()
    {
        NotifyId = dto.NotifyId ?? string.Empty,
        EcnId = dto.EcnId ?? string.Empty,
        DeptId = dto.DeptId ?? string.Empty,
        DeptName = dto.DeptName ?? string.Empty,
        Confirmed = dto.Confirmed,
        NotifiedAt = dto.NotifiedAt,
        ConfirmedBy = dto.ConfirmedBy ?? string.Empty,
        ConfirmedAt = dto.ConfirmedAt,
        CreatedAt = dto.CreatedAt,
    };

    private static EcnImplement MapToEcnImplement(EcnImplementDto dto) => new()
    {
        ImplementId = dto.ImplementId ?? string.Empty,
        EcnId = dto.EcnId ?? string.Empty,
        TaskName = dto.TaskName ?? string.Empty,
        Description = dto.Description ?? string.Empty,
        Responsible = dto.Responsible ?? string.Empty,
        PlanDate = dto.PlanDate,
        ActualDate = dto.ActualDate,
        Status = dto.Status ?? string.Empty,
        Result = dto.Result ?? string.Empty,
        Remark = dto.Remark ?? string.Empty,
        CreatedAt = dto.CreatedAt,
        CreatedBy = dto.CreatedBy ?? string.Empty,
        UpdatedAt = dto.UpdatedAt,
        UpdatedBy = dto.UpdatedBy ?? string.Empty,
    };

    private static EcnStatistics MapToEcnStatistics(EcnStatisticsDto dto) => new()
    {
        TotalCount = dto.TotalCount,
        DraftCount = dto.DraftCount,
        ReviewCount = dto.ReviewCount,
        ApprovalCount = dto.ApprovalCount,
        ImplementCount = dto.ImplementCount,
        VerifyCount = dto.VerifyCount,
        ClosedCount = dto.ClosedCount,
        RejectedCount = dto.RejectedCount,
        UrgentCount = dto.UrgentCount,
        CloseRate = dto.CloseRate,
        AvgDaysToClose = dto.AvgDaysToClose ?? 0,
        TypeDistribution = dto.TypeDistribution ?? new Dictionary<string, int>(),
        SeverityDistribution = dto.SeverityDistribution ?? new Dictionary<string, int>(),
        StatusDistribution = dto.StatusDistribution ?? new Dictionary<string, int>(),
    };
}
