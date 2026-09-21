using {{ProjectNamespace}}.Application.Status;

namespace {{ProjectNamespace}}.UnitTests.Status;

/// <summary>
/// Example unit test for the application layer: dependencies are substituted by
/// hand, so no mocking package is needed. Delete along with StatusService once
/// the template's example code is gone.
/// </summary>
public sealed class StatusServiceTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetStatusAsync_reports_database_connectivity(bool canConnect)
    {
        var service = new StatusService(
            new StubDatabaseProbe(canConnect),
            new StubEnvironmentInfo("{{ProjectName}}.Api", "Development"));

        var status = await service.GetStatusAsync(CancellationToken.None);

        Assert.Equal(canConnect, status.DatabaseConnected);
        Assert.Equal("{{ProjectName}}.Api", status.Application);
        Assert.Equal("Development", status.Environment);
    }

    private sealed class StubDatabaseProbe : IDatabaseProbe
    {
        private readonly bool _canConnect;

        public StubDatabaseProbe(bool canConnect) => _canConnect = canConnect;

        public Task<bool> CanConnectAsync(CancellationToken cancellationToken) =>
            Task.FromResult(_canConnect);
    }

    private sealed class StubEnvironmentInfo : IEnvironmentInfo
    {
        public StubEnvironmentInfo(string applicationName, string environmentName)
        {
            ApplicationName = applicationName;
            EnvironmentName = environmentName;
        }

        public string ApplicationName { get; }

        public string EnvironmentName { get; }
    }
}
