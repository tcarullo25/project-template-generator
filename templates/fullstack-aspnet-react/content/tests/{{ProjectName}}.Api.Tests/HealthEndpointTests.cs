using System.Net;

namespace {{ProjectNamespace}}.Api.Tests;

/// <summary>
/// Verifies that the application starts, its dependency-injection graph is
/// valid, and the liveness endpoint responds. This is the test that catches a
/// broken Program.cs or a missing service registration.
/// </summary>
public sealed class HealthEndpointTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public HealthEndpointTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Liveness_endpoint_returns_healthy()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
}
