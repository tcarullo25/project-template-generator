namespace {{ProjectNamespace}}.Application.Status;

/// <summary>Describes the environment the application is hosted in.</summary>
public interface IEnvironmentInfo
{
    string ApplicationName { get; }

    string EnvironmentName { get; }
}
