using ProjectTemplateGenerator.Core.Configuration;
using ProjectTemplateGenerator.Core.Generation;
using ProjectTemplateGenerator.Core.Templates;

namespace ProjectTemplateGenerator.Cli.Commands;

/// <summary>Implements <c>project-template-generator new &lt;ProjectName&gt;</c>.</summary>
public static class NewCommand
{
    public static int Execute(IReadOnlyList<string> args, TemplateCatalog catalog)
    {
        var options = CommandLineParser.ParseNew(args);

        var specification = new ProjectSpecification
        {
            ProjectName = options.ProjectName,
            OutputDirectory = options.OutputDirectory ?? Directory.GetCurrentDirectory(),
            TemplateId = options.TemplateId ?? TemplateCatalog.DefaultTemplateId,
            Overwrite = options.Force,
            InitializeGitRepository = options.GitInit,
        };

        var generator = new ProjectGenerator(catalog);
        var result = generator.Generate(specification, new ConsoleGenerationProgress());

        Console.WriteLine();
        Console.WriteLine($"Project created successfully: {result.ProjectDirectory}");
        Console.WriteLine($"  template: {result.TemplateId}");
        Console.WriteLine($"  files:    {result.FilesWritten.Count}");
        Console.WriteLine();
        Console.WriteLine("Next steps:");
        Console.WriteLine($"  cd {options.ProjectName}");
        Console.WriteLine("  cp .env.example .env");
        Console.WriteLine("  docker compose up");
        Console.WriteLine();
        Console.WriteLine("See the generated README.md for the full local development workflow.");

        return ExitCodes.Success;
    }
}
