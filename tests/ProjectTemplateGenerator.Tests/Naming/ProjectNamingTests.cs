using ProjectTemplateGenerator.Core.Naming;

namespace ProjectTemplateGenerator.Tests.Naming;

public sealed class ProjectNamingTests
{
    [Theory]
    [InlineData("MahjongTracker", "mahjong-tracker")]
    [InlineData("App", "app")]
    [InlineData("APIServer", "api-server")]
    [InlineData("Budget2026", "budget-2026")]
    public void ToKebabCase_splits_on_word_boundaries(string input, string expected)
    {
        Assert.Equal(expected, ProjectNaming.ToKebabCase(input));
    }

    [Theory]
    [InlineData("MahjongTracker", "mahjong_tracker")]
    [InlineData("App", "app")]
    [InlineData("APIServer", "api_server")]
    public void ToSnakeCase_splits_on_word_boundaries(string input, string expected)
    {
        Assert.Equal(expected, ProjectNaming.ToSnakeCase(input));
    }
}
