using System.Diagnostics;
using ProjectTemplateGenerator.Core.Configuration;
using ProjectTemplateGenerator.Core.Generation;
using ProjectTemplateGenerator.Core.Templates;

namespace ProjectTemplateGenerator.Tests.Generation;

/// <summary>
/// Generates a project and actually builds and tests it.
/// <para>
/// Opt-in: these need a .NET SDK, Node.js and network access for package
/// restore, and they take minutes rather than milliseconds. Enable with
/// <c>PTG_SMOKE_TESTS=1</c>; the rest of the suite stays fast and hermetic.
/// </para>
/// </summary>
public sealed class GeneratedProjectSmokeTests
{
    private const string EnableVariable = "PTG_SMOKE_TESTS";

    private static bool Enabled =>
        Environment.GetEnvironmentVariable(EnableVariable) is "1" or "true";

    [SkippableFact]
    public void Generated_backend_builds_and_its_tests_pass()
    {
        Skip.IfNot(Enabled, $"Set {EnableVariable}=1 to run the generated-project smoke tests.");

        using var temp = new TemporaryDirectory();
        var project = GenerateProject(temp, "SmokeApp");

        RunOrFail(project, "dotnet", ["build", "SmokeApp.sln", "--nologo"]);
        RunOrFail(project, "dotnet", ["test", "SmokeApp.sln", "--nologo", "--no-build"]);
    }

    [SkippableFact]
    public void Generated_frontend_builds_and_its_tests_pass()
    {
        Skip.IfNot(Enabled, $"Set {EnableVariable}=1 to run the generated-project smoke tests.");

        using var temp = new TemporaryDirectory();
        var project = GenerateProject(temp, "SmokeApp");
        var web = Path.Combine(project, "src", "Frontend", "web");

        RunOrFail(web, "npm", ["install", "--no-audit", "--no-fund"]);
        RunOrFail(web, "npm", ["run", "build"]);
        RunOrFail(web, "npm", ["test"]);
    }

    private static string GenerateProject(TemporaryDirectory temp, string projectName)
    {
        var specification = new ProjectSpecification
        {
            ProjectName = projectName,
            OutputDirectory = temp.Path,
        };

        return new ProjectGenerator(TemplateCatalog.CreateDefault())
            .Generate(specification, NullGenerationProgress.Instance)
            .ProjectDirectory;
    }

    private static void RunOrFail(string workingDirectory, string fileName, string[] arguments)
    {
        var startInfo = new ProcessStartInfo(fileName)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo)
                            ?? throw new InvalidOperationException($"Could not start '{fileName}'.");

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        Assert.True(
            process.ExitCode == 0,
            $"'{fileName} {string.Join(' ', arguments)}' exited with {process.ExitCode}.\n{output}\n{error}");
    }
}
