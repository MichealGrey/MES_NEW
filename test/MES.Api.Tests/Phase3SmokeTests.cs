using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MES.Api.Tests;

/// <summary>
/// Smoke tests for Phase 3 controllers (Process Control).
/// Verifies that each endpoint exists and responds without throwing 500 errors.
/// </summary>
[Trait("Category", "Smoke")]
public class Phase3SmokeTests : ApiTestBase
{
    public Phase3SmokeTests(WebApplicationFactory<Program> factory) : base(factory) { }

    // === ProcessParameter 工艺参数 ===

    [Fact]
    [Trait("Phase", "3")]
    public async Task ProcessParameter_QueryParameterSets_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v3/ProcessParameter");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "3")]
    public async Task ProcessParameter_QueryCuringCurves_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v3/ProcessParameter/curing-curves");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Bin Bin定义 ===

    [Fact]
    [Trait("Phase", "3")]
    public async Task Bin_QueryBinDefinitions_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v3/Bin/definitions");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "3")]
    public async Task Bin_GetBinSummary_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v3/Bin/summary");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Tooling 治具管理 ===

    [Fact]
    [Trait("Phase", "3")]
    public async Task Tooling_QueryToolings_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v3/Tooling");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "3")]
    public async Task Tooling_GetUsageLogs_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v3/Tooling/test-tooling-id/usage-logs");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Wire 焊丝消耗 ===

    [Fact]
    [Trait("Phase", "3")]
    public async Task Wire_QueryWireConsumptions_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v3/Wire/consumptions");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === Qualification 资质认证 ===

    [Fact]
    [Trait("Phase", "3")]
    public async Task Qualification_QueryQualifications_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v3/Qualification");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "3")]
    public async Task Qualification_GetCheckLogs_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v3/Qualification/check-logs");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    // === BondPullTest 推拉力测试 ===

    [Fact]
    [Trait("Phase", "3")]
    public async Task BondPullTest_QueryTests_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v3/BondPullTest");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    [Trait("Phase", "3")]
    public async Task BondPullTest_GetTest_ShouldRespond_WithValidResponse()
    {
        var response = await _client.GetAsync("/api/v3/BondPullTest/test-id");
        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
