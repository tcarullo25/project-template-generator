using ProjectTemplateGenerator.Core.Templates;

namespace ProjectTemplateGenerator.Core.Configuration;

/// <summary>
/// Everything the generator needs to know about the project being created.
/// Parsed from the command line; deliberately separate from generation logic so
/// that a config file or interactive prompt could produce one later.
/// </summary>
public sealed record ProjectSpecification
{
    public required string ProjectName { get; init; }

    /// <summary>Directory the project directory is created inside. Defaults to the working directory.</summary>
    public required string OutputDirectory { get; init; }

    public string TemplateId { get; init; } = TemplateCatalog.DefaultTemplateId;

    /// <summary>Allow writing into a directory that already exists and is not empty.</summary>
    public bool Overwrite { get; init; }

    /// <summary>Run <c>git init</c> in the generated project. Off by default.</summary>
    public bool InitializeGitRepository { get; init; }

    /// <summary>Absolute path of the directory that will be created.</summary>
    public string ProjectDirectory => Path.GetFullPath(Path.Combine(OutputDirectory, ProjectName));
}
