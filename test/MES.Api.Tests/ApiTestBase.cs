using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MES.Api.Tests;

public class ApiTestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;
    protected readonly WebApplicationFactory<Program> _factory;

    public ApiTestBase(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
}
