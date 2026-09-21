using ProjectTemplateGenerator.Core.Naming;
using ProjectTemplateGenerator.Core.Templating;

namespace ProjectTemplateGenerator.Core.Configuration;

/// <summary>
/// Builds the placeholder values derived from the project itself. Version and
/// dependency placeholders come from the template manifest instead and are
/// merged on top of these.
/// </summary>
public static class ProjectVariables
{
    public const string ProjectNameKey = "ProjectName";
    public const string ProjectNamespaceKey = "ProjectNamespace";
    public const string ProjectSlugKey = "ProjectSlug";
    public const string DatabaseNameKey = "DatabaseName";
    public const string DatabaseUserKey = "DatabaseUser";

    public static TemplateVariables Build(ProjectSpecification specification)
    {
        ArgumentNullException.ThrowIfNull(specification);

        var name = specification.ProjectName;
        var snake = ProjectNaming.ToSnakeCase(name);

        return new TemplateVariables(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ProjectNameKey] = name,
            [ProjectNamespaceKey] = name,
            [ProjectSlugKey] = ProjectNaming.ToKebabCase(name),
            [DatabaseNameKey] = snake,
            [DatabaseUserKey] = snake,
        });
    }
}
