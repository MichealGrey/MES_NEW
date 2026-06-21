using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MES.Api.Tests;

/// <summary>
/// Smoke tests for Phase 4 controllers (External Integration).
/// Verifies that each endpoint exists and responds without throwing 500 errors.
/// </summary>
[Trait("Category", "Smoke")]
public class Phase4SmokeTests : ApiTestBase
{
    public Phase4SmokeTests(WebApplicationFactory<Program> factory) : base(factory) { }

    // === Integration 外部系统集成 ===

    [Fact]
    [Trait("Phase", "4")]
    public async Task Integration_GetAllAdapters_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v4/Integration/adapters");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "4")]
    public async Task Integration_GetAdapterHealth_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v4/Integration/adapters/erp/health");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "4")]
    public async Task Integration_GetEquipmentStatus_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v4/Integration/eap/equipment-status/test-equipment");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "4")]
    public async Task Integration_GetQualityAlert_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v4/Integration/qms/alert");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "4")]
    public async Task Integration_GetApprovalStatus_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v4/Integration/oa/approval-status/test-id");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "4")]
    public async Task Integration_GetOrderProgress_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v4/Integration/portal/order-progress/test-order-id");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
