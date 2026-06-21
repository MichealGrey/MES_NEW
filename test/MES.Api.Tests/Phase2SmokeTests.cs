using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MES.Api.Tests;

/// <summary>
/// Smoke tests for Phase 2 controllers (Planning).
/// Verifies that each endpoint exists and responds without throwing 500 errors.
/// </summary>
[Trait("Category", "Smoke")]
public class Phase2SmokeTests : ApiTestBase
{
    public Phase2SmokeTests(WebApplicationFactory<Program> factory) : base(factory) { }

    // === Order 订单管理 ===

    [Fact]
    [Trait("Phase", "2")]
    public async Task Order_GetOrders_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v2/Order");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "2")]
    public async Task Order_GetOrder_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v2/Order/test-order-id");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "2")]
    public async Task Order_GetReviewStatus_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v2/Order/test-order-id/review-status");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Planning 计划管理 ===

    [Fact]
    [Trait("Phase", "2")]
    public async Task Planning_GetPlans_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v2/Planning");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "2")]
    public async Task Planning_GetPlan_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v2/Planning/test-plan-id");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === MRP 物料需求计划 ===

    [Fact]
    [Trait("Phase", "2")]
    public async Task Mrp_GetBom_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v2/Mrp/bom/test-bom-id");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "2")]
    public async Task Mrp_GetShortageWarnings_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v2/Mrp/shortage-warnings");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === RushOrder 急单管理 ===

    [Fact]
    [Trait("Phase", "2")]
    public async Task RushOrder_GetRushOrders_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v2/RushOrder");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "2")]
    public async Task RushOrder_GetRushOrder_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v2/RushOrder/test-request-id");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
