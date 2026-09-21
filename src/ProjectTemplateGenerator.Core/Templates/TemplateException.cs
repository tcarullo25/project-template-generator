namespace ProjectTemplateGenerator.Core.Templates;

/// <summary>Raised when a template is missing or malformed.</summary>
public sealed class TemplateException : Exception
{
    public TemplateException(string message) : base(message)
    {
    }
}
