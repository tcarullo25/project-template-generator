namespace ProjectTemplateGenerator.Core.Templating;

/// <summary>Raised when a template refers to a placeholder that was not supplied.</summary>
public sealed class TemplateRenderException : Exception
{
    public TemplateRenderException(string message) : base(message)
    {
    }
}
