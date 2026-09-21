using ProjectTemplateGenerator.Core.Configuration;
using ProjectTemplateGenerator.Core.Generation;
using ProjectTemplateGenerator.Core.Templates;

namespace ProjectTemplateGenerator.Tests.Generation;

/// <summary>
/// Exercises the real shipped template, writing only into a temporary directory.
/// </summary>
public sealed class ProjectGeneratorTests
{
    private const string ProjectName = "MahjongTracker";

    [Fact]
    public void Generates_the_documented_project_structure()
    {
        using var temp = new TemporaryDirectory();
        var result = Generate(temp, ProjectName);

        string[] expected =
        [
            ".editorconfig",
            ".env.example",
            ".gitignore",
            "Directory.Build.props",
            "MahjongTracker.sln",
            "README.md",
            "docker-compose.yml",
            "docker/api.Dockerfile",
            "docker/web.Dockerfile",
            "src/Backend/MahjongTracker.Api/MahjongTracker.Api.csproj",
            "src/Backend/MahjongTracker.Api/Program.cs",
            "src/Backend/MahjongTracker.Api/appsettings.json",
            "src/Backend/MahjongTracker.Application/MahjongTracker.Application.csproj",
            "src/Backend/MahjongTracker.Domain/MahjongTracker.Domain.csproj",
            "src/Backend/MahjongTracker.Infrastructure/Persistence/ApplicationDbContext.cs",
            "src/Frontend/web/package.json",
            "src/Frontend/web/src/App.tsx",
            "src/Frontend/web/vite.config.ts",
            "tests/MahjongTracker.Api.Tests/MahjongTracker.Api.Tests.csproj",
            "tests/MahjongTracker.UnitTests/MahjongTracker.UnitTests.csproj",
        ];

        foreach (var relativePath in expected)
        {
            Assert.True(
                File.Exists(Path.Combine(result.ProjectDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar))),
                $"Expected generated file '{relativePath}'.");
        }

        Assert.Equal(expected.Length, result.FilesWritten.Intersect(expected, StringComparer.Ordinal).Count());
    }

    [Fact]
    public void Substitutes_the_project_name_in_paths_and_contents()
    {
        using var temp = new TemporaryDirectory();
        var result = Generate(temp, ProjectName);

        var program = ReadGenerated(result, "src/Backend/MahjongTracker.Api/Program.cs");
        Assert.Contains("using MahjongTracker.Application;", program, StringComparison.Ordinal);

        var packageJson = ReadGenerated(result, "src/Frontend/web/package.json");
        Assert.Contains("\"name\": \"mahjong-tracker-web\"", packageJson, StringComparison.Ordinal);

        var compose = ReadGenerated(result, "docker-compose.yml");
        Assert.Contains("mahjong_tracker", compose, StringComparison.Ordinal);
        Assert.Contains("name: mahjong-tracker", compose, StringComparison.Ordinal);
    }

    [Fact]
    public void Leaves_no_unresolved_placeholders_anywhere()
    {
        using var temp = new TemporaryDirectory();
        var result = Generate(temp, ProjectName);

        var offenders = EnumerateGeneratedFiles(result)
            .Where(path => File.ReadAllText(path).Contains("{{", StringComparison.Ordinal))
            .ToList();

        Assert.Empty(offenders);
    }

    [Fact]
    public void Generated_project_does_not_reference_the_generator()
    {
        using var temp = new TemporaryDirectory();
        var result = Generate(temp, ProjectName);

        var offenders = EnumerateGeneratedFiles(result)
            .Where(path => File.ReadAllText(path)
                .Contains("ProjectTemplateGenerator", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.Empty(offenders);
    }

    [Fact]
    public void Refuses_to_write_into_a_non_empty_directory()
    {
        using var temp = new TemporaryDirectory();
        var projectDirectory = Path.Combine(temp.Path, ProjectName);
        Directory.CreateDirectory(projectDirectory);
        File.WriteAllText(Path.Combine(projectDirectory, "existing.txt"), "keep me");

        var exception = Assert.Throws<GenerationException>(() => Generate(temp, ProjectName));

        Assert.Contains("--force", exception.Message, StringComparison.Ordinal);
        Assert.Equal("keep me", File.ReadAllText(Path.Combine(projectDirectory, "existing.txt")));
        Assert.False(File.Exists(Path.Combine(projectDirectory, "README.md")));
    }

    [Fact]
    public void Writes_into_an_existing_empty_directory()
    {
        using var temp = new TemporaryDirectory();
        Directory.CreateDirectory(Path.Combine(temp.Path, ProjectName));

        var result = Generate(temp, ProjectName);

        Assert.True(File.Exists(Path.Combine(result.ProjectDirectory, "README.md")));
    }

    [Fact]
    public void Writes_into_a_non_empty_directory_when_forced()
    {
        using var temp = new TemporaryDirectory();
        var projectDirectory = Path.Combine(temp.Path, ProjectName);
        Directory.CreateDirectory(projectDirectory);
        File.WriteAllText(Path.Combine(projectDirectory, "existing.txt"), "keep me");

        var result = Generate(temp, ProjectName, overwrite: true);

        Assert.True(File.Exists(Path.Combine(result.ProjectDirectory, "README.md")));
        Assert.True(File.Exists(Path.Combine(projectDirectory, "existing.txt")));
    }

    [Fact]
    public void Rejects_an_invalid_project_name_before_creating_anything()
    {
        using var temp = new TemporaryDirectory();

        Assert.Throws<GenerationException>(() => Generate(temp, "Not A Name"));
        Assert.Empty(Directory.EnumerateFileSystemEntries(temp.Path));
    }

    [Fact]
    public void Rejects_an_unknown_template()
    {
        using var temp = new TemporaryDirectory();

        var specification = new ProjectSpecification
        {
            ProjectName = ProjectName,
            OutputDirectory = temp.Path,
            TemplateId = "does-not-exist",
        };

        Assert.Throws<TemplateException>(
            () => new ProjectGenerator(TemplateCatalog.CreateDefault())
                .Generate(specification, NullGenerationProgress.Instance));
    }

    [Fact]
    public void Generation_is_deterministic()
    {
        using var first = new TemporaryDirectory();
        using var second = new TemporaryDirectory();

        var a = Generate(first, ProjectName);
        var b = Generate(second, ProjectName);

        Assert.Equal(a.FilesWritten, b.FilesWritten);

        foreach (var relativePath in a.FilesWritten)
        {
            Assert.Equal(ReadGenerated(a, relativePath), ReadGenerated(b, relativePath));
        }
    }

    private static GenerationResult Generate(TemporaryDirectory temp, string projectName, bool overwrite = false)
    {
        var specification = new ProjectSpecification
        {
            ProjectName = projectName,
            OutputDirectory = temp.Path,
            Overwrite = overwrite,
        };

        return new ProjectGenerator(TemplateCatalog.CreateDefault())
            .Generate(specification, NullGenerationProgress.Instance);
    }

    private static string ReadGenerated(GenerationResult result, string relativePath) =>
        File.ReadAllText(Path.Combine(
            result.ProjectDirectory,
            relativePath.Replace('/', Path.DirectorySeparatorChar)));

    private static IEnumerable<string> EnumerateGeneratedFiles(GenerationResult result) =>
        Directory.EnumerateFiles(result.ProjectDirectory, "*", SearchOption.AllDirectories);
}
