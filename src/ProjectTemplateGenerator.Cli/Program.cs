using System.Reflection;
using ProjectTemplateGenerator.Cli;
using ProjectTemplateGenerator.Cli.Commands;
using ProjectTemplateGenerator.Core.Generation;
using ProjectTemplateGenerator.Core.Templates;
using ProjectTemplateGenerator.Core.Templating;

return CliEntryPoint.Run(args);

internal static class CliEntryPoint
{
    public static int Run(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return ExitCodes.UserError;
        }

        try
        {
            var catalog = TemplateCatalog.CreateDefault();

            return args[0] switch
            {
                "new" => NewCommand.Execute(args[1..], catalog),
                "list-templates" => ListTemplatesCommand.Execute(catalog),
                "--help" or "-h" or "help" => PrintUsage(),
                "--version" or "-v" => PrintVersion(),
                _ => UnknownCommand(args[0]),
            };
        }
        catch (Exception ex) when (ex is CommandLineException or GenerationException
                                      or TemplateException or TemplateRenderException)
        {
            // Expected user-facing failures: message only, no stack trace.
            Console.Error.WriteLine($"error: {ex.Message}");
            return ExitCodes.UserError;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("error: the generator failed unexpectedly. This is a bug.");
            Console.Error.WriteLine(ex);
            return ExitCodes.UnexpectedError;
        }
    }

    private static int UnknownCommand(string command)
    {
        Console.Error.WriteLine($"error: unknown command '{command}'.");
        PrintUsage();
        return ExitCodes.UserError;
    }

    private static int PrintVersion()
    {
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";

        // Strip the source-revision suffix MSBuild appends (e.g. "1.0.0+abcdef").
        Console.WriteLine(version.Split('+')[0]);
        return ExitCodes.Success;
    }

    private static int PrintUsage()
    {
        Console.WriteLine("""
            project-template-generator - create a full-stack project from a template.

            Usage:
              project-template-generator new <ProjectName> [options]
              project-template-generator list-templates
              project-template-generator --help | --version

            Options for 'new':
              -o, --output <dir>     Directory to create the project in (default: current directory).
              -t, --template <id>    Template to use (default: fullstack-aspnet-react).
                  --force            Write into the target directory even if it already has files.
                  --git-init         Run 'git init' in the generated project.

            Examples:
              project-template-generator new MahjongTracker
              project-template-generator new MahjongTracker --output ~/code --git-init
            """);

        return ExitCodes.Success;
    }
}
