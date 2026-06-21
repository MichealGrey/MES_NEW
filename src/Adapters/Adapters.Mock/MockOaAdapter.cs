using MES.Adapters.Abstractions;
using MES.Adapters.Abstractions.Oa;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MES.Adapters.Mock;

/// <summary>
/// Mock OA 适配器 - 模拟审批流系统对接
/// </summary>
public class MockOaAdapter : IMesOaAdapter
{
    private readonly MockConfig _config;
    private readonly ILogger<MockOaAdapter> _logger;
    private readonly Random _random = new();

    public MockOaAdapter(IOptions<MockConfig> config, ILogger<MockOaAdapter> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    private async Task SimulateDelayAsync() => await Task.Delay(_config.DelayMs);

    public async Task<AdapterResult<ApprovalPushResult>> PushApprovalRequestAsync(ApprovalRequest request)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock OA: PushApprovalRequestAsync failed due to configured failure rate");
            return AdapterResult<ApprovalPushResult>.Fail("MOCK_OA_FAILURE", "Simulated OA approval push failure", "MockOA");
        }

        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var oaApprovalNo = $"OA-{timestamp}";

        _logger.LogInformation("Mock OA: Pushing approval request {ApprovalId} ({Type}), OA approval no: {OaApprovalNo}",
            request.ApprovalId, request.ApprovalType, oaApprovalNo);

        var result = new ApprovalPushResult
        {
            ApprovalId = request.ApprovalId,
            OaApprovalNo = oaApprovalNo,
            Pushed = true
        };

        return AdapterResult<ApprovalPushResult>.Ok(result, "MockOA");
    }

    public async Task<AdapterResult<ApprovalStatus>> GetApprovalStatusAsync(string approvalId)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock OA: GetApprovalStatusAsync failed due to configured failure rate");
            return AdapterResult<ApprovalStatus>.Fail("MOCK_OA_FAILURE", "Simulated OA approval status failure", "MockOA");
        }

        // Simulate status: 60% Approved, 30% Pending, 10% Rejected
        var rand = _random.NextDouble();
        var status = rand < 0.6 ? "Approved" : rand < 0.9 ? "Pending" : "Rejected";

        var statuses = new[] { "Approved", "Rejected", "Pending" };
        var approvers = new[] { "张经理", "李总监", "王副总", "赵总工" };
        var steps = new[] { "部门审批", "质量审批", "技术审批", "总经理审批" };

        var stepHistory = new List<ApprovalStepStatus>();
        var stepCount = _random.Next(1, 5);

        for (var i = 0; i < stepCount; i++)
        {
            stepHistory.Add(new ApprovalStepStatus
            {
                StepName = steps[i],
                Approver = approvers[i],
                Result = i < stepCount - 1 ? "Approved" : status == "Pending" ? "Pending" : status,
                Comment = i < stepCount - 1 ? "审批通过" : status == "Approved" ? "最终审批通过" : status == "Pending" ? "等待审批" : "审批驳回",
                ActionTime = i < stepCount - 1
                    ? DateTime.UtcNow.AddMinutes(-_random.Next(60, 1440))
                    : (status != "Pending" ? DateTime.UtcNow.AddMinutes(-_random.Next(0, 60)) : null)
            });
        }

        _logger.LogDebug("Mock OA: Approval {ApprovalId} status: {Status}", approvalId, status);

        var result = new ApprovalStatus
        {
            ApprovalId = approvalId,
            Status = status,
            CurrentApprover = status == "Pending" ? approvers[stepCount - 1] : null,
            CurrentStep = status == "Pending" ? steps[stepCount - 1] : null,
            SubmittedAt = DateTime.UtcNow.AddMinutes(-_random.Next(120, 2880)),
            CompletedAt = status != "Pending" ? DateTime.UtcNow.AddMinutes(-_random.Next(0, 60)) : null,
            StepHistory = stepHistory
        };

        return AdapterResult<ApprovalStatus>.Ok(result, "MockOA");
    }

    public async Task<AdapterResult<ApprovalCallback>> ReceiveApprovalCallbackAsync(ApprovalResult callback)
    {
        await SimulateDelayAsync();

        if (_random.NextDouble() < _config.FailureRate)
        {
            _logger.LogWarning("Mock OA: ReceiveApprovalCallbackAsync failed due to configured failure rate");
            return AdapterResult<ApprovalCallback>.Fail("MOCK_OA_FAILURE", "Simulated OA callback receive failure", "MockOA");
        }

        _logger.LogInformation("Mock OA: Received approval callback for {ApprovalId}, result: {Result}",
            callback.ApprovalId, callback.Result);

        var result = new ApprovalCallback
        {
            Processed = true,
            ApprovalId = callback.ApprovalId
        };

        return AdapterResult<ApprovalCallback>.Ok(result, "MockOA");
    }

    public async Task<AdapterResult<HealthStatus>> HealthCheckAsync()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await SimulateDelayAsync();
        sw.Stop();

        _logger.LogDebug("Mock OA: Health check completed in {ElapsedMs}ms", sw.ElapsedMilliseconds);

        var result = new HealthStatus
        {
            IsHealthy = true,
            Message = "Mock OA adapter is healthy",
            ResponseTimeMs = _config.DelayMs
        };

        return AdapterResult<HealthStatus>.Ok(result, "MockOA");
    }

    public AdapterInfo GetAdapterInfo()
    {
        return new AdapterInfo { Name = "MockOA", Version = "1.0.0", Provider = "Mock" };
    }
}
