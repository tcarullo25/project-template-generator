using System.Text;

namespace ProjectTemplateGenerator.Core.Naming;

/// <summary>
/// Derives the naming variants a generated project needs from a single
/// PascalCase project name. Centralised here so that every template variable
/// that is "the project name, spelled differently" has exactly one source.
/// </summary>
public static class ProjectNaming
{
    /// <summary>"MahjongTracker" -&gt; ["Mahjong", "Tracker"]; "APIServer" -&gt; ["API", "Server"].</summary>
    public static IReadOnlyList<string> SplitWords(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var words = new List<string>();
        var current = new StringBuilder();

        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            var startsNewWord =
                current.Length > 0 &&
                (IsUpperFollowedByLower(name, i) || IsBoundaryBetweenClasses(name, i));

            if (startsNewWord)
            {
                words.Add(current.ToString());
                current.Clear();
            }

            current.Append(c);
        }

        if (current.Length > 0)
        {
            words.Add(current.ToString());
        }

        return words;
    }

    /// <summary>"MahjongTracker" -&gt; "mahjong-tracker". Used for npm and Docker names.</summary>
    public static string ToKebabCase(string name) =>
        string.Join('-', SplitWords(name).Select(w => w.ToLowerInvariant()));

    /// <summary>"MahjongTracker" -&gt; "mahjong_tracker". Used for PostgreSQL identifiers.</summary>
    public static string ToSnakeCase(string name) =>
        string.Join('_', SplitWords(name).Select(w => w.ToLowerInvariant()));

    // "abC" at index 2: an upper-case letter followed by a lower-case one ends the previous word.
    private static bool IsUpperFollowedByLower(string name, int index) =>
        char.IsUpper(name[index]) &&
        index + 1 < name.Length &&
        char.IsLower(name[index + 1]);

    // A transition between letter-case classes or between letters and digits.
    private static bool IsBoundaryBetweenClasses(string name, int index)
    {
        var previous = name[index - 1];
        var current = name[index];

        if (char.IsLower(previous) && char.IsUpper(current))
        {
            return true;
        }

        return char.IsDigit(previous) != char.IsDigit(current);
    }
}
