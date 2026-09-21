namespace {{ProjectNamespace}}.Api.Diagnostics;

internal static class HealthCheckTags
{
    /// <summary>Checks that must pass before the app can serve traffic.</summary>
    public const string Ready = "ready";
}
