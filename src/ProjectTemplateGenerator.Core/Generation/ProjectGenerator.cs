using ProjectTemplateGenerator.Core.Configuration;
using ProjectTemplateGenerator.Core.Templates;
using ProjectTemplateGenerator.Core.Templating;
using ProjectTemplateGenerator.Core.Validation;

namespace ProjectTemplateGenerator.Core.Generation;

/// <summary>
/// Turns a <see cref="ProjectSpecification"/> into a project on disk.
/// <para>
/// The generator knows nothing about ASP.NET Core, React or PostgreSQL: it
/// validates the name, resolves a template, renders every file in it and writes
/// the results. All stack-specific knowledge lives in the template directory.
/// </para>
/// </summary>
public sealed class ProjectGenerator
{
    private readonly TemplateCatalog _catalog;

    public ProjectGenerator(TemplateCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        _catalog = catalog;
    }

    public GenerationResult Generate(ProjectSpecification specification, IGenerationProgress progress)
    {
        ArgumentNullException.ThrowIfNull(specification);
        ArgumentNullException.ThrowIfNull(progress);

        var nameValidation = ProjectNameValidator.Validate(specification.ProjectName);
        if (!nameValidation.IsValid)
        {
            throw new GenerationException(nameValidation.Error!);
        }

        var template = _catalog.Load(specification.TemplateId);
        var variables = ProjectVariables.Build(specification).With(template.Manifest.Variables);

        var projectDirectory = specification.ProjectDirectory;
        EnsureTargetDirectoryIsUsable(projectDirectory, specification.Overwrite);

        progress.Step($"Creating project '{specification.ProjectName}' in {projectDirectory}");
        Directory.CreateDirectory(projectDirectory);

        var written = WriteTemplate(template, variables, projectDirectory, progress);

        if (specification.InitializeGitRepository)
        {
            progress.Step("Initializing git repository");
            GitRepositoryInitializer.TryInitialize(projectDirectory, progress);
        }

        return new GenerationResult(projectDirectory, template.Manifest.Id, written);
    }

    private static List<string> WriteTemplate(
        ITemplateSource template,
        TemplateVariables variables,
        string projectDirectory,
        IGenerationProgress progress)
    {
        var planned = template
            .EnumerateFiles()
            .Select(file => new
            {
                File = file,
                TargetPath = TemplatePathTransformer.Transform(file.RelativePath, variables),
            })
            .ToList();

        if (planned.Count == 0)
        {
            throw new GenerationException($"Template '{template.Manifest.Id}' contains no files.");
        }

        var byPhase = planned.ToLookup(item => GenerationPhase.Classify(item.TargetPath));
        var written = new List<string>();

        foreach (var phase in GenerationPhase.Order)
        {
            var items = byPhase[phase].ToList();
            if (items.Count == 0)
            {
                continue;
            }

            progress.Step($"Creating {phase}");

            foreach (var item in items.OrderBy(i => i.TargetPath, StringComparer.Ordinal))
            {
                var absolutePath = ResolveWithinProject(projectDirectory, item.TargetPath);
                var content = TemplateRenderer.Render(item.File.ReadText(), variables, item.File.RelativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
                File.WriteAllText(absolutePath, content);

                written.Add(item.TargetPath);
            }
        }

        written.Sort(StringComparer.Ordinal);
        return written;
    }

    private static void EnsureTargetDirectoryIsUsable(string projectDirectory, bool overwrite)
    {
        if (File.Exists(projectDirectory))
        {
            throw new GenerationException($"'{projectDirectory}' already exists and is a file.");
        }

        if (!Directory.Exists(projectDirectory))
        {
            return;
        }

        var isEmpty = !Directory.EnumerateFileSystemEntries(projectDirectory).Any();
        if (isEmpty || overwrite)
        {
            return;
        }

        throw new GenerationException(
            $"'{projectDirectory}' already exists and is not empty. " +
            "Choose another name or another --output directory, or pass --force to write into it. " +
            "Nothing was written.");
    }

    /// <summary>
    /// Guards against a template path escaping the project directory (a '..' segment
    /// or an absolute path). The generator must never write outside what it created.
    /// </summary>
    private static string ResolveWithinProject(string projectDirectory, string relativePath)
    {
        var root = Path.GetFullPath(projectDirectory);
        var candidate = Path.GetFullPath(Path.Combine(root, relativePath));

        var rootWithSeparator = root.EndsWith(Path.DirectorySeparatorChar)
            ? root
            : root + Path.DirectorySeparatorChar;

        if (!candidate.StartsWith(rootWithSeparator, StringComparison.Ordinal))
        {
            throw new GenerationException(
                $"Template path '{relativePath}' resolves outside the project directory. Generation aborted.");
        }

        return candidate;
    }
}
