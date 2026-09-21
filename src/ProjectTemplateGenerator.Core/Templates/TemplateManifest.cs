using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectTemplateGenerator.Core.Templates;

/// <summary>
/// The <c>template.json</c> that sits at the root of every template directory.
/// <para>
/// <see cref="Variables"/> is where tool and dependency versions live (.NET target
/// framework, EF Core, React, Vite, the PostgreSQL image tag, ...). Upgrading a
/// generated stack is an edit to that one file, not a hunt through generator code.
/// </para>
/// </summary>
public sealed class TemplateManifest
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    /// <summary>Directory, relative to the manifest, holding the files to generate.</summary>
    [JsonPropertyName("contentDirectory")]
    public string ContentDirectory { get; init; } = "content";

    /// <summary>Template-supplied placeholder values, chiefly pinned dependency versions.</summary>
    [JsonPropertyName("variables")]
    public IReadOnlyDictionary<string, string> Variables { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static TemplateManifest Parse(string json, string origin)
    {
        TemplateManifest? manifest;
        try
        {
            manifest = JsonSerializer.Deserialize<TemplateManifest>(json, SerializerOptions);
        }
        catch (JsonException ex)
        {
            throw new TemplateException($"Template manifest '{origin}' is not valid JSON: {ex.Message}");
        }

        if (manifest is null)
        {
            throw new TemplateException($"Template manifest '{origin}' is empty.");
        }

        if (string.IsNullOrWhiteSpace(manifest.Id))
        {
            throw new TemplateException($"Template manifest '{origin}' does not declare an 'id'.");
        }

        return manifest;
    }
}
