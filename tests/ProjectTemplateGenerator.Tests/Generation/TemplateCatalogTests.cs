using ProjectTemplateGenerator.Core.Configuration;
using ProjectTemplateGenerator.Core.Templates;
using ProjectTemplateGenerator.Core.Templating;

namespace ProjectTemplateGenerator.Tests.Generation;

public sealed class TemplateCatalogTests
{
    [Fact]
    public void Ships_the_default_template()
    {
        var ids = TemplateCatalog.CreateDefault().ListTemplates().Select(m => m.Id);

        Assert.Contains(TemplateCatalog.DefaultTemplateId, ids);
    }

    [Fact]
    public void Default_template_declares_the_versions_it_pins()
    {
        var manifest = TemplateCatalog.CreateDefault().Load(TemplateCatalog.DefaultTemplateId).Manifest;

        Assert.False(string.IsNullOrWhiteSpace(manifest.DisplayName));
        Assert.False(string.IsNullOrWhiteSpace(manifest.Description));
        Assert.Contains("TargetFramework", manifest.Variables.Keys);
        Assert.Contains("EfCoreVersion", manifest.Variables.Keys);
        Assert.Contains("ReactVersion", manifest.Variables.Keys);
        Assert.Contains("PostgresImageTag", manifest.Variables.Keys);
    }

    /// <summary>
    /// Guards the failure mode the renderer cannot catch until generation time:
    /// a template file referring to a placeholder nobody defines.
    /// </summary>
    [Fact]
    public void Every_placeholder_used_by_the_template_is_defined()
    {
        var template = TemplateCatalog.CreateDefault().Load(TemplateCatalog.DefaultTemplateId);

        var specification = new ProjectSpecification
        {
            ProjectName = "PlaceholderProbe",
            OutputDirectory = Path.GetTempPath(),
        };

        var defined = ProjectVariables.Build(specification).With(template.Manifest.Variables)
            .Select(pair => pair.Key)
            .ToHashSet(StringComparer.Ordinal);

        var undefined = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var file in template.EnumerateFiles())
        {
            foreach (var name in TemplateRenderer.FindPlaceholders(file.RelativePath)
                         .Concat(TemplateRenderer.FindPlaceholders(file.ReadText()))
                         .Where(name => !defined.Contains(name)))
            {
                undefined.Add($"{name} (in {file.RelativePath})");
            }
        }

        Assert.Empty(undefined);
    }

    /// <summary>Unused variables are dead weight in the manifest; flag them early.</summary>
    [Fact]
    public void Every_manifest_variable_is_used_by_the_template()
    {
        var template = TemplateCatalog.CreateDefault().Load(TemplateCatalog.DefaultTemplateId);

        var used = template.EnumerateFiles()
            .SelectMany(file => TemplateRenderer.FindPlaceholders(file.RelativePath)
                .Concat(TemplateRenderer.FindPlaceholders(file.ReadText())))
            .ToHashSet(StringComparer.Ordinal);

        var unused = template.Manifest.Variables.Keys
            .Where(key => !used.Contains(key))
            .OrderBy(key => key, StringComparer.Ordinal)
            .ToList();

        Assert.Empty(unused);
    }
}
