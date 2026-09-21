using ProjectTemplateGenerator.Core.Generation;

namespace ProjectTemplateGenerator.Cli;

/// <summary>Writes progress to stdout and warnings to stderr.</summary>
public sealed class ConsoleGenerationProgress : IGenerationProgress
{
    public void Step(string message) => Console.WriteLine($"  {message}...");

    public void Warning(string message) => Console.Error.WriteLine($"  warning: {message}");
}
