namespace {{ProjectNamespace}}.Application.Status;

/// <summary>
/// Reports whether the application and its database are working. Exists to prove
/// the frontend -> API -> PostgreSQL path end to end; it is also a small, real
/// example of the layering and of a unit test with a substituted dependency.
/// </summary>
public sealed class StatusService
{
    private readonly IDatabaseProbe _databaseProbe;
    private readonly IEnvironmentInfo _environment;

    public StatusService(IDatabaseProbe databaseProbe, IEnvironmentInfo environment)
    {
        _databaseProbe = databaseProbe;
        _environment = environment;
    }

    public async Task<ApplicationStatus> GetStatusAsync(CancellationToken cancellationToken)
    {
        var databaseConnected = await _databaseProbe.CanConnectAsync(cancellationToken);

        return new ApplicationStatus(
            _environment.ApplicationName,
            _environment.EnvironmentName,
            databaseConnected);
    }
}
