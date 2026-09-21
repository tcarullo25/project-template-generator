using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace {{ProjectNamespace}}.Api.Tests;

/// <summary>
/// Hosts the real API in-process for tests.
/// <para>
/// A connection string is supplied so dependency injection can be built, but no
/// database is contacted: tests here should exercise endpoints that do not touch
/// PostgreSQL. Tests that do need a database should stand one up explicitly (for
/// example with Testcontainers) rather than rely on a developer's local instance.
/// </para>
/// <para>
/// The settings are applied as environment variables rather than through
/// <c>ConfigureAppConfiguration</c>: with minimal hosting, Program.cs reads
/// configuration while registering services, which happens before the test
/// host's configuration callbacks run.
/// </para>
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private const string TestConnectionString =
        "Host=localhost;Port=5432;Database={{DatabaseName}}_test;Username={{DatabaseUser}};Password=unused";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", TestConnectionString);

        builder.UseEnvironment("Testing");
    }
}
