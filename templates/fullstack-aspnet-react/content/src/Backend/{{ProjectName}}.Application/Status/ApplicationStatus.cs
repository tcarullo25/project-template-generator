namespace {{ProjectNamespace}}.Application.Status;

/// <param name="Application">Name of this application.</param>
/// <param name="Environment">Hosting environment the API is running under.</param>
/// <param name="DatabaseConnected">Whether the database answered a connectivity check.</param>
public sealed record ApplicationStatus(string Application, string Environment, bool DatabaseConnected);
