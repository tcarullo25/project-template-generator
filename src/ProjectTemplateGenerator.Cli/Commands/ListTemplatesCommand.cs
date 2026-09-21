using ProjectTemplateGenerator.Core.Templates;

namespace ProjectTemplateGenerator.Cli.Commands;

/// <summary>Implements <c>project-template-generator list-templates</c>.</summary>
public static class ListTemplatesCommand
{
    public static int Execute(TemplateCatalog catalog)
    {
        var templates = catalog.ListTemplates();

        if (templates.Count == 0)
        {
            Console.Error.WriteLine("No templates found. The 'templates' directory is missing from the installation.");
            return ExitCodes.UserError;
        }

        Console.WriteLine("Available templates:");
        foreach (var template in templates)
        {
            Console.WriteLine();
            Console.WriteLine($"  {template.Id}");
            Console.WriteLine($"    {template.DisplayName}");
            Console.WriteLine($"    {template.Description}");
        }

        return ExitCodes.Success;
    }
}
