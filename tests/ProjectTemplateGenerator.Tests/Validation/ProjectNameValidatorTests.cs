using ProjectTemplateGenerator.Core.Validation;

namespace ProjectTemplateGenerator.Tests.Validation;

public sealed class ProjectNameValidatorTests
{
    [Theory]
    [InlineData("MahjongTracker")]
    [InlineData("A")]
    [InlineData("App2")]
    [InlineData("lowercaseIsAllowed")]
    public void Accepts_letters_and_digits_starting_with_a_letter(string name)
    {
        Assert.True(ProjectNameValidator.Validate(name).IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("2Cool")]            // starts with a digit
    [InlineData("My App")]           // whitespace
    [InlineData("My-App")]           // separator
    [InlineData("My.App")]           // dot
    [InlineData("My_App")]           // underscore
    [InlineData("../escape")]        // path traversal
    [InlineData("/absolute")]
    [InlineData("C:\\Windows")]
    [InlineData("Caf\u00e9")]        // non-ASCII
    public void Rejects_names_that_are_unsafe_or_malformed(string? name)
    {
        var result = ProjectNameValidator.Validate(name);

        Assert.False(result.IsValid);
        Assert.False(string.IsNullOrWhiteSpace(result.Error));
    }

    [Theory]
    [InlineData("CON")]
    [InlineData("nul")]
    [InlineData("COM1")]
    public void Rejects_names_Windows_reserves_for_devices(string name)
    {
        Assert.False(ProjectNameValidator.Validate(name).IsValid);
    }

    [Theory]
    [InlineData("class")]
    [InlineData("namespace")]
    public void Rejects_csharp_keywords(string name)
    {
        Assert.False(ProjectNameValidator.Validate(name).IsValid);
    }

    [Fact]
    public void Rejects_names_longer_than_the_limit()
    {
        var tooLong = new string('A', ProjectNameValidator.MaxLength + 1);

        Assert.False(ProjectNameValidator.Validate(tooLong).IsValid);
        Assert.True(ProjectNameValidator.Validate(tooLong[..ProjectNameValidator.MaxLength]).IsValid);
    }
}
