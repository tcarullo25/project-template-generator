using {{ProjectNamespace}}.Application.Status;

namespace {{ProjectNamespace}}.Api.Hosting;

/// <summary>Adapts ASP.NET Core's <see cref="IHostEnvironment"/> to the application layer.</summary>
internal sealed class HostEnvironmentInfo : IEnvironmentInfo
{
    private readonly IHostEnvironment _hostEnvironment;

    public HostEnvironmentInfo(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public string ApplicationName => _hostEnvironment.ApplicationName;

    public string EnvironmentName => _hostEnvironment.EnvironmentName;
}
