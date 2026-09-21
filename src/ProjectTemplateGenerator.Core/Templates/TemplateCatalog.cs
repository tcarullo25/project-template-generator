using System.Reflection;

namespace ProjectTemplateGenerator.Core.Templates;

/// <summary>
/// Finds the templates shipped with the generator.
/// <para>
/// V1 has exactly one template. The catalog exists so that adding a second one
/// is a matter of dropping a directory under <c>templates/</c> — no generator
/// code changes, no registration list to keep in sync.
/// </para>
/// </summary>
public sealed class TemplateCatalog
{
    public const string DefaultTemplateId = "fullstack-aspnet-react";
    private const string TemplatesFolderName = "templates";

    private readonly string _templatesRoot;

    public TemplateCatalog(string templatesRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templatesRoot);
        _templatesRoot = templatesRoot;
    }

    /// <summary>The <c>templates</c> folder copied next to the running assembly.</summary>
    public static TemplateCatalog CreateDefault()
    {
        var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                            ?? AppContext.BaseDirectory;

        return new TemplateCatalog(Path.Combine(baseDirectory, TemplatesFolderName));
    }

    public IReadOnlyList<TemplateManifest> ListTemplates()
    {
        if (!Directory.Exists(_templatesRoot))
        {
            return [];
        }

        return Directory
            .EnumerateDirectories(_templatesRoot)
            .Where(directory => File.Exists(Path.Combine(directory, DirectoryTemplateSource.ManifestFileName)))
            .Select(directory => new DirectoryTemplateSource(directory).Manifest)
            .OrderBy(manifest => manifest.Id, StringComparer.Ordinal)
            .ToList();
    }

    public ITemplateSource Load(string templateId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateId);

        var directory = Path.Combine(_templatesRoot, templateId);
        if (!Directory.Exists(directory))
        {
            var known = ListTemplates().Select(m => m.Id).ToList();
            var suffix = known.Count > 0 ? $" Known templates: {string.Join(", ", known)}." : string.Empty;

            throw new TemplateException($"Unknown template '{templateId}'.{suffix}");
        }

        return new DirectoryTemplateSource(directory);
    }
}
