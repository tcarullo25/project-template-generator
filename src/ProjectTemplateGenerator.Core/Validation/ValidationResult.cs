namespace ProjectTemplateGenerator.Core.Validation;

/// <summary>
/// Outcome of a validation check. Validation failures are expected user errors,
/// so they are returned as values rather than thrown as exceptions.
/// </summary>
public sealed class ValidationResult
{
    private ValidationResult(bool isValid, string? error)
    {
        IsValid = isValid;
        Error = error;
    }

    public bool IsValid { get; }

    /// <summary>Human-readable explanation of the failure. Null when <see cref="IsValid"/>.</summary>
    public string? Error { get; }

    public static ValidationResult Success() => new(true, null);

    public static ValidationResult Failure(string error) => new(false, error);
}
