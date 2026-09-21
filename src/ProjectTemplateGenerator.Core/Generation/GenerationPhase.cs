namespace ProjectTemplateGenerator.Core.Generation;

/// <summary>
/// Maps a generated file's path to the progress message it belongs under, so the
/// CLI can report "Creating backend..." without the generator hard-coding a list
/// of steps. The mapping is presentation only — nothing downstream depends on it.
/// </summary>
internal static class GenerationPhase
{
    private const string Fallback = "project files";

    /// <summary>Checked in order; the first matching prefix wins.</summary>
    private static readonly (string PathPrefix, string Label)[] Phases =
    [
        ("src/Backend/", "backend"),
        ("src/Frontend/", "frontend"),
        ("tests/", "tests"),
        ("docker/", "Docker configuration"),
        ("docker-compose", "Docker configuration"),
        (".env", "environment configuration"),
    ];

    public static string Classify(string relativePath)
    {
        foreach (var (prefix, label) in Phases)
        {
            if (relativePath.StartsWith(prefix, StringComparison.Ordinal))
            {
                return label;
            }
        }

        return Fallback;
    }

    /// <summary>Phase labels in the order they should be reported.</summary>
    public static IEnumerable<string> Order => Phases.Select(p => p.Label).Distinct().Append(Fallback);
}
