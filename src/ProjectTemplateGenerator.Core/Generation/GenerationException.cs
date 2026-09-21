namespace ProjectTemplateGenerator.Core.Generation;

/// <summary>
/// An expected, user-facing generation failure (target directory in the way,
/// unsafe template path, ...). The CLI prints the message without a stack trace.
/// </summary>
public sealed class GenerationException : Exception
{
    public GenerationException(string message) : base(message)
    {
    }
}
