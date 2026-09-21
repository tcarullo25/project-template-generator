namespace ProjectTemplateGenerator.Core.Templates;

/// <summary>Reads a template from a directory containing <c>template.json</c> and a content folder.</summary>
public sealed class DirectoryTemplateSource : ITemplateSource
{
    public const string ManifestFileName = "template.json";

    private readonly string _contentRoot;

    public DirectoryTemplateSource(string templateDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(templateDirectory);

        var manifestPath = Path.Combine(templateDirectory, ManifestFileName);
        if (!File.Exists(manifestPath))
        {
            throw new TemplateException($"No {ManifestFileName} found in '{templateDirectory}'.");
        }

        Manifest = TemplateManifest.Parse(File.ReadAllText(manifestPath), manifestPath);

        _contentRoot = Path.Combine(templateDirectory, Manifest.ContentDirectory);
        if (!Directory.Exists(_contentRoot))
        {
            throw new TemplateException(
                $"Template '{Manifest.Id}' declares content directory '{Manifest.ContentDirectory}', " +
                $"but '{_contentRoot}' does not exist.");
        }
    }

    public TemplateManifest Manifest { get; }

    public IReadOnlyList<TemplateFile> EnumerateFiles() =>
        Directory
            .EnumerateFiles(_contentRoot, "*", SearchOption.AllDirectories)
            .Select(absolutePath =>
            {
                var relativePath = Path
                    .GetRelativePath(_contentRoot, absolutePath)
                    .Replace(Path.DirectorySeparatorChar, '/');

                return new TemplateFile(relativePath, () => File.ReadAllText(absolutePath));
            })
            .OrderBy(file => file.RelativePath, StringComparer.Ordinal)
            .ToList();
}
