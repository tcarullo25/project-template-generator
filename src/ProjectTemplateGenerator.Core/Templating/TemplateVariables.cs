using System.Collections;

namespace ProjectTemplateGenerator.Core.Templating;

/// <summary>
/// The immutable set of <c>{{Name}}</c> values available to a template render.
/// </summary>
public sealed class TemplateVariables : IReadOnlyCollection<KeyValuePair<string, string>>
{
    private readonly IReadOnlyDictionary<string, string> _values;

    public TemplateVariables(IReadOnlyDictionary<string, string> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        _values = new Dictionary<string, string>(values, StringComparer.Ordinal);
    }

    public int Count => _values.Count;

    public bool TryGetValue(string name, out string value) => _values.TryGetValue(name, out value!);

    public string this[string name] => _values[name];

    /// <summary>Returns a copy with <paramref name="additional"/> merged in. Later values win.</summary>
    public TemplateVariables With(IReadOnlyDictionary<string, string> additional)
    {
        var merged = new Dictionary<string, string>(_values, StringComparer.Ordinal);
        foreach (var (key, value) in additional)
        {
            merged[key] = value;
        }

        return new TemplateVariables(merged);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
