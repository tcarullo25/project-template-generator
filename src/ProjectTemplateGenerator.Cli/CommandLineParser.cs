namespace ProjectTemplateGenerator.Cli;

/// <summary>
/// Parses the <c>new</c> command's arguments.
/// <para>
/// Hand-written on purpose: the surface is one command with four options, and a
/// parsing library would be the generator's only third-party dependency.
/// Revisit if the command set grows.
/// </para>
/// </summary>
public static class CommandLineParser
{
    public static NewCommandOptions ParseNew(IReadOnlyList<string> args)
    {
        ArgumentNullException.ThrowIfNull(args);

        string? projectName = null;
        string? output = null;
        string? template = null;
        var force = false;
        var gitInit = false;

        for (var i = 0; i < args.Count; i++)
        {
            var arg = args[i];

            switch (arg)
            {
                case "--output" or "-o":
                    output = ReadValue(args, ref i, arg);
                    break;

                case "--template" or "-t":
                    template = ReadValue(args, ref i, arg);
                    break;

                case "--force":
                    force = true;
                    break;

                case "--git-init":
                    gitInit = true;
                    break;

                default:
                    if (arg.StartsWith('-'))
                    {
                        throw new CommandLineException($"Unknown option '{arg}'.");
                    }

                    if (projectName is not null)
                    {
                        throw new CommandLineException(
                            $"Unexpected argument '{arg}'. 'new' takes a single project name.");
                    }

                    projectName = arg;
                    break;
            }
        }

        if (projectName is null)
        {
            throw new CommandLineException("Missing project name. Usage: project-template-generator new <ProjectName>");
        }

        return new NewCommandOptions
        {
            ProjectName = projectName,
            OutputDirectory = output,
            TemplateId = template,
            Force = force,
            GitInit = gitInit,
        };
    }

    private static string ReadValue(IReadOnlyList<string> args, ref int index, string optionName)
    {
        if (index + 1 >= args.Count)
        {
            throw new CommandLineException($"Option '{optionName}' requires a value.");
        }

        index++;
        return args[index];
    }
}
