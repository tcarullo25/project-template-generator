using System.Diagnostics;

namespace ProjectTemplateGenerator.Core.Generation;

/// <summary>
/// Runs <c>git init</c> in the generated project.
/// <para>
/// Opt-in (<c>--git-init</c>) rather than automatic: generating files and
/// creating a repository are separate decisions, and the generator should not
/// require git to be installed. The argument list is fixed — nothing the user
/// types is ever passed to a shell. Failure is a warning, not an error: the
/// generated project is already complete without it.
/// </para>
/// </summary>
internal static class GitRepositoryInitializer
{
    public static void TryInitialize(string projectDirectory, IGenerationProgress progress)
    {
        var startInfo = new ProcessStartInfo("git")
        {
            WorkingDirectory = projectDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        startInfo.ArgumentList.Add("init");
        startInfo.ArgumentList.Add("--initial-branch=main");

        try
        {
            using var process = Process.Start(startInfo);
            if (process is null)
            {
                progress.Warning("Could not start git; skipped repository initialization.");
                return;
            }

            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                progress.Warning($"'git init' failed ({process.ExitCode}): {error.Trim()}");
            }
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            progress.Warning($"git is not available; skipped repository initialization ({ex.Message}).");
        }
    }
}
