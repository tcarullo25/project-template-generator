namespace ProjectTemplateGenerator.Core.Templates;

/// <summary>
/// One file inside a template, identified by its path relative to the template's
/// content directory (always using '/' separators).
/// </summary>
/// <param name="RelativePath">e.g. <c>src/Backend/{{ProjectName}}.Api/Program.cs</c>.</param>
/// <param name="ReadText">Reads the raw, unrendered contents.</param>
public sealed record TemplateFile(string RelativePath, Func<string> ReadText);
