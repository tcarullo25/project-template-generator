using ProjectTemplateGenerator.Cli;

namespace ProjectTemplateGenerator.Tests.Cli;

public sealed class CommandLineParserTests
{
    [Fact]
    public void Parses_a_bare_project_name()
    {
        var options = CommandLineParser.ParseNew(["MahjongTracker"]);

        Assert.Equal("MahjongTracker", options.ProjectName);
        Assert.Null(options.OutputDirectory);
        Assert.Null(options.TemplateId);
        Assert.False(options.Force);
        Assert.False(options.GitInit);
    }

    [Fact]
    public void Parses_all_options_in_any_order()
    {
        var options = CommandLineParser.ParseNew(
            ["--force", "-o", "/tmp/code", "MahjongTracker", "--git-init", "--template", "custom"]);

        Assert.Equal("MahjongTracker", options.ProjectName);
        Assert.Equal("/tmp/code", options.OutputDirectory);
        Assert.Equal("custom", options.TemplateId);
        Assert.True(options.Force);
        Assert.True(options.GitInit);
    }

    [Theory]
    [InlineData("")]                        // no project name
    [InlineData("A B")]                     // two positional arguments
    [InlineData("A --nope")]                // unknown option
    [InlineData("A --output")]              // option without a value
    [InlineData("A -t")]                    // option without a value
    public void Rejects_malformed_command_lines(string commandLine)
    {
        var args = commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        Assert.Throws<CommandLineException>(() => CommandLineParser.ParseNew(args));
    }
}
