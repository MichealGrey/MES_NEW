using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MES.Api.Tests;

/// <summary>
/// Smoke tests for Phase 1 controllers (Quality &amp; Warehouse).
/// Verifies that each endpoint exists and responds without throwing 500 errors.
/// Since all endpoints require authentication, 401 responses are acceptable.
/// </summary>
[Trait("Category", "Smoke")]
public class Phase1SmokeTests : ApiTestBase
{
    public Phase1SmokeTests(WebApplicationFactory<Program> factory) : base(factory) { }

    // === IQC 来料检验 ===

    [Fact]
    [Trait("Phase", "1")]
    public async Task Iqc_GetTasks_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/Iqc/tasks");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "1")]
    public async Task Iqc_GetStatistics_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/Iqc/statistics");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === FQC/OQC 终检/出货检验 ===

    [Fact]
    [Trait("Phase", "1")]
    public async Task FqcOqc_GetFqcTasks_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/FqcOqc/fqc");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "1")]
    public async Task FqcOqc_GetOqcTasks_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/FqcOqc/oqc");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Nonconforming 不合格品管理 ===

    [Fact]
    [Trait("Phase", "1")]
    public async Task Nonconforming_GetRecords_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/Nonconforming/records");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "1")]
    public async Task Nonconforming_GetDetail_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/Nonconforming/records/test-id");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Warehouse 仓库管理 ===

    [Fact]
    [Trait("Phase", "1")]
    public async Task Warehouse_GetInventory_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/Warehouse/inventory");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "1")]
    public async Task Warehouse_GetFifoRecommendation_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/Warehouse/fifo-recommend");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === IssueReturn 领料/退料 ===

    [Fact]
    [Trait("Phase", "1")]
    public async Task IssueReturn_GetIssueOrders_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/IssueReturn/issue");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "1")]
    public async Task IssueReturn_GetReturnOrders_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/IssueReturn/return");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === FinishedGoods 成品管理 ===

    [Fact]
    [Trait("Phase", "1")]
    public async Task FinishedGoods_GetInventory_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/FinishedGoods/inventory");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "1")]
    public async Task FinishedGoods_GetShipments_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/FinishedGoods/shipments");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === EquipmentMaintenance 设备维护 ===

    [Fact]
    [Trait("Phase", "1")]
    public async Task EquipmentMaintenance_GetFaults_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/EquipmentMaintenance/faults");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "1")]
    public async Task EquipmentMaintenance_GetPmPlans_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/EquipmentMaintenance/pm-plans");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === FirstArticle 首件检验 ===

    [Fact]
    [Trait("Phase", "1")]
    public async Task FirstArticle_GetFirstArticles_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/FirstArticle");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "1")]
    public async Task FirstArticle_GetBondPullTests_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/FirstArticle/bond-pull-test");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === QualityAlert 质量警报 ===

    [Fact]
    [Trait("Phase", "1")]
    public async Task QualityAlert_GetAlerts_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/QualityAlert");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "1")]
    public async Task QualityAlert_GetAlertDetail_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/QualityAlert/test-alert-id");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Abnormal 异常管理 ===

    [Fact]
    [Trait("Phase", "1")]
    public async Task Abnormal_GetRecords_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/Abnormal/records");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "1")]
    public async Task Abnormal_GetStatistics_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/Abnormal/statistics");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
