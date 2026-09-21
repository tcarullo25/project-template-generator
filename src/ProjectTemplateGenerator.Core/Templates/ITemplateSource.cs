namespace ProjectTemplateGenerator.Core.Templates;

/// <summary>
/// Supplies a template's manifest and files. The generator only ever sees this
/// interface, so templates can come from disk today and from somewhere else
/// (embedded resources, a package) later without touching generation logic.
/// </summary>
public interface ITemplateSource
{
    TemplateManifest Manifest { get; }

    /// <summary>Every file in the template, in a stable, deterministic order.</summary>
    IReadOnlyList<TemplateFile> EnumerateFiles();
}
