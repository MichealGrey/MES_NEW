using System.Collections.ObjectModel;
using MES.Modules.CustomerComplaint.Models;

namespace MES.Modules.CustomerComplaint.Services;

public interface IComplaintService
{
    Task<List<ComplaintInfo>> GetAllComplaintsAsync();
    Task<ComplaintInfo?> GetComplaintAsync(string complaintId);
    Task SaveComplaintAsync(ComplaintInfo complaint);
    Task DeleteComplaintAsync(string complaintId);
    Task UpdateComplaintStatusAsync(string complaintId, string status);
    Task UpdateEightDStepAsync(string complaintId, string step, string content);
    Task<List<EightDStep>> GetEightDStepsAsync(string complaintId);
    Task<CustomerQualityReport> GenerateQualityReportAsync(string complaintId);
    Task<List<ComplaintInfo>> GetComplaintsByCustomerAsync(string customerId);
}

public class ComplaintService : IComplaintService
{
    private readonly List<ComplaintInfo> _complaints = [];
    private readonly Dictionary<string, List<EightDStep>> _eightDSteps = [];

    public ComplaintService()
    {
        SeedData();
    }

    private void SeedData()
    {
        _complaints.AddRange([
            new ComplaintInfo
            {
                ComplaintId = "CMP-2024-001",
                ComplaintNo = "CMP-2024-001",
                CustomerId = "CUST-001",
                CustomerName = "客户A",
                ProductId = "PROD-001",
                ProductName = "芯片A",
                LotId = "LOT-2024-001",
                DefectType = "功能不良",
                DefectDescription = "测试发现部分芯片功能异常",
                AffectedQty = 500,
                ReportDate = DateTime.Now.AddDays(-15),
                RequiredDate = DateTime.Now.AddDays(-5),
                Severity = "High",
                Status = "InProgress",
                EightDStatus = "D4",
                AssignedTo = "张工程师",
                RootCause = "待分析",
                Priority = "High",
            },
            new ComplaintInfo
            {
                ComplaintId = "CMP-2024-002",
                ComplaintNo = "CMP-2024-002",
                CustomerId = "CUST-002",
                CustomerName = "客户B",
                ProductId = "PROD-002",
                ProductName = "芯片B",
                LotId = "LOT-2024-002",
                DefectType = "外观不良",
                DefectDescription = "封装表面有划痕",
                AffectedQty = 1000,
                ReportDate = DateTime.Now.AddDays(-7),
                RequiredDate = DateTime.Now.AddDays(3),
                Severity = "Medium",
                Status = "Open",
                EightDStatus = "D2",
                AssignedTo = "李工程师",
                Priority = "Normal",
            },
            new ComplaintInfo
            {
                ComplaintId = "CMP-2024-003",
                ComplaintNo = "CMP-2024-003",
                CustomerId = "CUST-001",
                CustomerName = "客户A",
                ProductId = "PROD-003",
                ProductName = "芯片C",
                LotId = "LOT-2024-003",
                DefectType = "参数超标",
                DefectDescription = "电性参数超出规格范围",
                AffectedQty = 200,
                ReportDate = DateTime.Now.AddDays(-30),
                RequiredDate = DateTime.Now.AddDays(-20),
                ActualCloseDate = DateTime.Now.AddDays(-5),
                Severity = "High",
                Status = "Closed",
                EightDStatus = "D8",
                AssignedTo = "王工程师",
                RootCause = "原材料批次异常",
                CorrectiveAction = "更换供应商",
                PreventiveAction = "增加来料检验频次",
                Priority = "High",
            },
        ]);

        _eightDSteps["CMP-2024-001"] = [
            new EightDStep { Step = "D0", StepName = "准备", Description = "评估是否需要8D", IsCompleted = true, CompletedBy = "张工程师", CompletedDate = DateTime.Now.AddDays(-15) },
            new EightDStep { Step = "D1", StepName = "成立团队", Description = "组建跨功能团队", IsCompleted = true, CompletedBy = "张工程师", CompletedDate = DateTime.Now.AddDays(-14) },
            new EightDStep { Step = "D2", StepName = "问题描述", Description = "明确问题定义", IsCompleted = true, CompletedBy = "张工程师", CompletedDate = DateTime.Now.AddDays(-13) },
            new EightDStep { Step = "D3", StepName = "临时对策", Description = "实施围堵措施", IsCompleted = true, CompletedBy = "张工程师", CompletedDate = DateTime.Now.AddDays(-12) },
            new EightDStep { Step = "D4", StepName = "根本原因", Description = "分析根本原因", IsCompleted = false },
            new EightDStep { Step = "D5", StepName = "纠正措施", Description = "制定并验证纠正措施", IsCompleted = false },
            new EightDStep { Step = "D6", StepName = "实施措施", Description = "实施纠正措施", IsCompleted = false },
            new EightDStep { Step = "D7", StepName = "预防措施", Description = "防止问题再发生", IsCompleted = false },
            new EightDStep { Step = "D8", StepName = "团队祝贺", Description = "认可团队贡献", IsCompleted = false },
        ];
    }

    public async Task<List<ComplaintInfo>> GetAllComplaintsAsync()
    {
        await Task.Delay(100);
        return _complaints.ToList();
    }

    public async Task<ComplaintInfo?> GetComplaintAsync(string complaintId)
    {
        await Task.Delay(50);
        return _complaints.FirstOrDefault(c => c.ComplaintId == complaintId);
    }

    public async Task SaveComplaintAsync(ComplaintInfo complaint)
    {
        await Task.Delay(100);
        var existing = _complaints.FirstOrDefault(c => c.ComplaintId == complaint.ComplaintId);
        if (existing is null)
            _complaints.Add(complaint);
        else
        {
            var idx = _complaints.IndexOf(existing);
            _complaints[idx] = complaint;
        }
    }

    public async Task DeleteComplaintAsync(string complaintId)
    {
        await Task.Delay(50);
        var complaint = _complaints.FirstOrDefault(c => c.ComplaintId == complaintId);
        if (complaint is not null)
            _complaints.Remove(complaint);
    }

    public async Task UpdateComplaintStatusAsync(string complaintId, string status)
    {
        await Task.Delay(50);
        var complaint = _complaints.FirstOrDefault(c => c.ComplaintId == complaintId);
        if (complaint is null) return;

        complaint.Status = status;
        if (status == "Closed")
            complaint.ActualCloseDate = DateTime.Now;
    }

    public async Task UpdateEightDStepAsync(string complaintId, string step, string content)
    {
        await Task.Delay(100);
        if (!_eightDSteps.ContainsKey(complaintId))
            _eightDSteps[complaintId] = GetDefaultEightDSteps();

        var steps = _eightDSteps[complaintId];
        var stepInfo = steps.FirstOrDefault(s => s.Step == step);
        if (stepInfo is not null)
        {
            stepInfo.Content = content;
            stepInfo.IsCompleted = true;
            stepInfo.CompletedBy = "当前用户";
            stepInfo.CompletedDate = DateTime.Now;
        }
    }

    public async Task<List<EightDStep>> GetEightDStepsAsync(string complaintId)
    {
        await Task.Delay(50);
        return _eightDSteps.GetValueOrDefault(complaintId, GetDefaultEightDSteps());
    }

    public async Task<CustomerQualityReport> GenerateQualityReportAsync(string complaintId)
    {
        await Task.Delay(200);
        var complaint = await GetComplaintAsync(complaintId);
        return new CustomerQualityReport
        {
            ReportId = "QR-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
            ComplaintNo = complaint?.ComplaintNo ?? string.Empty,
            CustomerName = complaint?.CustomerName ?? string.Empty,
            ReportDate = DateTime.Now,
            ReportType = "8D",
            Summary = $"关于 {complaint?.ProductName} 的{complaint?.DefectType}问题报告",
            Findings = complaint?.DefectDescription ?? string.Empty,
            CorrectiveActions = complaint?.CorrectiveAction ?? string.Empty,
            PreventiveActions = complaint?.PreventiveAction ?? string.Empty,
            Conclusion = complaint?.Status == "Closed" ? "问题已解决" : "问题处理中",
        };
    }

    public async Task<List<ComplaintInfo>> GetComplaintsByCustomerAsync(string customerId)
    {
        await Task.Delay(100);
        return _complaints.Where(c => c.CustomerId == customerId).ToList();
    }

    private static List<EightDStep> GetDefaultEightDSteps() => [
        new EightDStep { Step = "D0", StepName = "准备", Description = "评估是否需要8D" },
        new EightDStep { Step = "D1", StepName = "成立团队", Description = "组建跨功能团队" },
        new EightDStep { Step = "D2", StepName = "问题描述", Description = "明确问题定义" },
        new EightDStep { Step = "D3", StepName = "临时对策", Description = "实施围堵措施" },
        new EightDStep { Step = "D4", StepName = "根本原因", Description = "分析根本原因" },
        new EightDStep { Step = "D5", StepName = "纠正措施", Description = "制定并验证纠正措施" },
        new EightDStep { Step = "D6", StepName = "实施措施", Description = "实施纠正措施" },
        new EightDStep { Step = "D7", StepName = "预防措施", Description = "防止问题再发生" },
        new EightDStep { Step = "D8", StepName = "团队祝贺", Description = "认可团队贡献" },
    ];
}
