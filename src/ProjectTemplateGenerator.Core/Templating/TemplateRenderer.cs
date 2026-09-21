using System.Text.RegularExpressions;

namespace ProjectTemplateGenerator.Core.Templating;

/// <summary>
/// The whole template language: <c>{{VariableName}}</c> is replaced by its value.
/// <para>
/// There are deliberately no conditionals, loops or expressions. Template files
/// stay valid C#/TypeScript/YAML that an editor can parse, and the substitution
/// rules fit in one paragraph. Unknown placeholders are an error rather than
/// being left in place, so a typo fails generation instead of shipping a broken
/// project.
/// </para>
/// </summary>
public static class TemplateRenderer
{
    private static readonly Regex Placeholder =
        new(@"\{\{\s*(?<name>[A-Za-z][A-Za-z0-9_]*)\s*\}\}", RegexOptions.Compiled);

    /// <summary>Renders template text. <paramref name="origin"/> only appears in error messages.</summary>
    public static string Render(string content, TemplateVariables variables, string origin)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(variables);

        return Placeholder.Replace(content, match =>
        {
            var name = match.Groups["name"].Value;
            if (variables.TryGetValue(name, out var value))
            {
                return value;
            }

            throw new TemplateRenderException(
                $"Template '{origin}' uses undefined placeholder '{{{{{name}}}}}'. " +
                "Define it in the template manifest (template.json) or in ProjectVariables.");
        });
    }

    /// <summary>Returns every placeholder name referenced by <paramref name="content"/>.</summary>
    public static IReadOnlySet<string> FindPlaceholders(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        return Placeholder.Matches(content)
            .Select(m => m.Groups["name"].Value)
            .ToHashSet(StringComparer.Ordinal);
    }
}
