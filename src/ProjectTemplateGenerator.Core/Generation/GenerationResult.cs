namespace ProjectTemplateGenerator.Core.Generation;

/// <param name="ProjectDirectory">Absolute path of the generated project.</param>
/// <param name="TemplateId">Template it was generated from.</param>
/// <param name="FilesWritten">Paths relative to <paramref name="ProjectDirectory"/>, sorted.</param>
public sealed record GenerationResult(
    string ProjectDirectory,
    string TemplateId,
    IReadOnlyList<string> FilesWritten);
