namespace ProjectTemplateGenerator.Cli;

/// <summary>Raised for malformed command lines; the CLI prints the message and usage.</summary>
public sealed class CommandLineException : Exception
{
    public CommandLineException(string message) : base(message)
    {
    }
}

/// <summary>Parsed form of <c>new &lt;ProjectName&gt; [options]</c>.</summary>
public sealed record NewCommandOptions
{
    public required string ProjectName { get; init; }

    public string? OutputDirectory { get; init; }

    public string? TemplateId { get; init; }

    public bool Force { get; init; }

    public bool GitInit { get; init; }
}
