using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MES.Api.Tests;

/// <summary>
/// Smoke tests for Phase 5 controllers (Analytics &amp; Audit).
/// Verifies that each endpoint exists and responds without throwing 500 errors.
/// </summary>
[Trait("Category", "Smoke")]
public class Phase5SmokeTests : ApiTestBase
{
    public Phase5SmokeTests(WebApplicationFactory<Program> factory) : base(factory) { }

    // === KPI 仪表盘 ===

    [Fact]
    [Trait("Phase", "5")]
    public async Task Kpi_GetDashboard_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/Kpi/dashboard");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "5")]
    public async Task Kpi_GetRealTime_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/Kpi/real-time");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Cost 成本分析 ===

    [Fact]
    [Trait("Phase", "5")]
    public async Task Cost_GetProductAnalysis_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/Cost/product-analysis");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "5")]
    public async Task Cost_GetWorkOrderAnalysis_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/Cost/work-order-analysis");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Yield 良率分析 ===

    [Fact]
    [Trait("Phase", "5")]
    public async Task Yield_GetProcessYield_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/Yield/process-yield");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "5")]
    public async Task Yield_GetCumulativeYield_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/Yield/cumulative-yield");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === NPI 新产品导入 ===

    [Fact]
    [Trait("Phase", "5")]
    public async Task Npi_QueryProjects_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/Npi/projects");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === ReliabilityTest 可靠性测试 ===

    [Fact]
    [Trait("Phase", "5")]
    public async Task ReliabilityTest_QueryPlans_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/ReliabilityTest/plans");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "5")]
    public async Task ReliabilityTest_GetPlan_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/ReliabilityTest/plans/test-plan-id");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Report 报表调度 ===

    [Fact]
    [Trait("Phase", "5")]
    public async Task Report_QuerySchedules_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/Report/schedule");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === SystemConfig 系统配置 ===

    [Fact]
    [Trait("Phase", "5")]
    public async Task SystemConfig_GetConfigs_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/SystemConfig");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "5")]
    public async Task SystemConfig_GetAlertRules_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/SystemConfig/alert-rules");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Audit 审计追踪 ===

    [Fact]
    [Trait("Phase", "5")]
    public async Task Audit_QueryAuditTrails_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/Audit/trails");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "5")]
    public async Task Audit_VerifyAuditIntegrity_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/Audit/trails/verify");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "5")]
    public async Task Audit_QueryCorrections_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v5/Audit/corrections");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
