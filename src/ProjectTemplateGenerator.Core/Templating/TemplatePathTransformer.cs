namespace ProjectTemplateGenerator.Core.Templating;

/// <summary>
/// Translates a path inside the template tree into the path it should take in
/// the generated project.
/// <para>Two rules, applied per path segment:</para>
/// <list type="number">
///   <item><description><c>{{Variable}}</c> is substituted, so directories can be named after the project.</description></item>
///   <item><description>A leading underscore becomes a dot: <c>_gitignore</c> becomes <c>.gitignore</c>.
///   Dot-files are stored underscored so that the template tree itself is not affected by tooling
///   (git, npm, EF) that treats dot-files specially.</description></item>
/// </list>
/// </summary>
public static class TemplatePathTransformer
{
    public static string Transform(string relativePath, TemplateVariables variables)
    {
        ArgumentException.ThrowIfNullOrEmpty(relativePath);
        ArgumentNullException.ThrowIfNull(variables);

        var segments = relativePath
            .Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(segment => TransformSegment(segment, relativePath, variables));

        return string.Join('/', segments);
    }

    private static string TransformSegment(string segment, string origin, TemplateVariables variables)
    {
        var rendered = TemplateRenderer.Render(segment, variables, origin);

        return rendered.StartsWith('_') ? string.Concat(".", rendered.AsSpan(1)) : rendered;
    }
}
