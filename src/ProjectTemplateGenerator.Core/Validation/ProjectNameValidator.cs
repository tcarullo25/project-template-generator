using System.Text.RegularExpressions;

namespace ProjectTemplateGenerator.Core.Validation;

/// <summary>
/// Validates the project name supplied on the command line.
/// <para>
/// The name is used as a directory name, a .NET root namespace, an assembly
/// name and the stem of several derived identifiers, so the accepted set is
/// deliberately narrow: PascalCase ASCII letters and digits. Anything that
/// would need escaping somewhere downstream is rejected up front.
/// </para>
/// </summary>
public static class ProjectNameValidator
{
    public const int MaxLength = 64;

    private static readonly Regex Pattern = new(@"^[A-Za-z][A-Za-z0-9]*$", RegexOptions.Compiled);

    /// <summary>Names Windows reserves for devices; they cannot be used as directory names.</summary>
    private static readonly HashSet<string> ReservedDeviceNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9",
    };

    /// <summary>C# keywords that cannot appear as a namespace or assembly identifier.</summary>
    private static readonly HashSet<string> ReservedCSharpKeywords = new(StringComparer.Ordinal)
    {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
        "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
        "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
        "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
        "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
        "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
        "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
        "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual",
        "void", "volatile", "while",
    };

    public static ValidationResult Validate(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ValidationResult.Failure("Project name is required.");
        }

        if (name.Length > MaxLength)
        {
            return ValidationResult.Failure(
                $"Project name must be {MaxLength} characters or fewer (got {name.Length}).");
        }

        if (!Pattern.IsMatch(name))
        {
            return ValidationResult.Failure(
                $"'{name}' is not a valid project name. Use letters and digits only, starting with a letter " +
                "(for example: MahjongTracker).");
        }

        if (ReservedDeviceNames.Contains(name))
        {
            return ValidationResult.Failure(
                $"'{name}' is a reserved device name on Windows and cannot be used as a directory name.");
        }

        if (ReservedCSharpKeywords.Contains(name))
        {
            return ValidationResult.Failure(
                $"'{name}' is a C# keyword and cannot be used as a namespace.");
        }

        return ValidationResult.Success();
    }
}
