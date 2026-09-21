using ProjectTemplateGenerator.Core.Templating;

namespace ProjectTemplateGenerator.Tests.Templating;

public sealed class TemplateRendererTests
{
    private static readonly TemplateVariables Variables = new(new Dictionary<string, string>
    {
        ["ProjectName"] = "MahjongTracker",
        ["DatabaseName"] = "mahjong_tracker",
    });

    [Fact]
    public void Substitutes_every_occurrence()
    {
        var rendered = TemplateRenderer.Render(
            "namespace {{ProjectName}}.Api; // {{ProjectName}}",
            Variables,
            "test");

        Assert.Equal("namespace MahjongTracker.Api; // MahjongTracker", rendered);
    }

    [Fact]
    public void Allows_whitespace_inside_the_braces()
    {
        Assert.Equal("MahjongTracker", TemplateRenderer.Render("{{  ProjectName  }}", Variables, "test"));
    }

    [Fact]
    public void Leaves_text_without_placeholders_untouched()
    {
        const string source = "const style = { color: 'red' }; // single braces are not placeholders";

        Assert.Equal(source, TemplateRenderer.Render(source, Variables, "test"));
    }

    [Fact]
    public void Throws_on_an_undefined_placeholder_naming_the_file()
    {
        var exception = Assert.Throws<TemplateRenderException>(
            () => TemplateRenderer.Render("{{Nope}}", Variables, "Program.cs"));

        Assert.Contains("Nope", exception.Message, StringComparison.Ordinal);
        Assert.Contains("Program.cs", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void FindPlaceholders_reports_each_name_once()
    {
        var found = TemplateRenderer.FindPlaceholders("{{A}} {{B}} {{A}}");

        Assert.Equal(new[] { "A", "B" }, found.OrderBy(x => x, StringComparer.Ordinal));
    }
}
