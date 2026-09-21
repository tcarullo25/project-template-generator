namespace {{ProjectNamespace}}.Application.Status;

/// <summary>
/// Checks that the database is reachable. Implemented in the infrastructure
/// layer so that this project never references a database driver.
/// </summary>
public interface IDatabaseProbe
{
    Task<bool> CanConnectAsync(CancellationToken cancellationToken);
}
