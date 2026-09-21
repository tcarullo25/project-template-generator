using ProjectTemplateGenerator.Core.Templating;

namespace ProjectTemplateGenerator.Tests.Templating;

public sealed class TemplatePathTransformerTests
{
    private static readonly TemplateVariables Variables =
        new(new Dictionary<string, string> { ["ProjectName"] = "MahjongTracker" });

    [Theory]
    [InlineData("src/Backend/{{ProjectName}}.Api/Program.cs", "src/Backend/MahjongTracker.Api/Program.cs")]
    [InlineData("_gitignore", ".gitignore")]
    [InlineData("_env.example", ".env.example")]
    [InlineData("src/Frontend/web/_gitignore", "src/Frontend/web/.gitignore")]
    [InlineData("{{ProjectName}}.sln", "MahjongTracker.sln")]
    public void Substitutes_variables_and_expands_leading_underscores(string input, string expected)
    {
        Assert.Equal(expected, TemplatePathTransformer.Transform(input, Variables));
    }

    [Fact]
    public void Normalises_backslashes_to_forward_slashes()
    {
        Assert.Equal(
            "docker/api.Dockerfile",
            TemplatePathTransformer.Transform("docker\\api.Dockerfile", Variables));
    }
}
