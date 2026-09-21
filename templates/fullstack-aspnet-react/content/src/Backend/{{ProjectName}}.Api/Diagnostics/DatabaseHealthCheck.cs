using Microsoft.Extensions.Diagnostics.HealthChecks;
using {{ProjectNamespace}}.Application.Status;

namespace {{ProjectNamespace}}.Api.Diagnostics;

/// <summary>
/// Readiness check for PostgreSQL. Uses the application layer's probe rather
/// than a third-party health-check package, so the API keeps one fewer dependency.
/// </summary>
internal sealed class DatabaseHealthCheck : IHealthCheck
{
    public const string Name = "postgresql";

    private readonly IDatabaseProbe _databaseProbe;

    public DatabaseHealthCheck(IDatabaseProbe databaseProbe)
    {
        _databaseProbe = databaseProbe;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _databaseProbe.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy("Database is reachable.")
                : HealthCheckResult.Unhealthy("Database is not reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connectivity check failed.", ex);
        }
    }
}
