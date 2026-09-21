namespace ProjectTemplateGenerator.Core.Generation;

/// <summary>Progress sink. The CLI writes to the console; tests use a no-op or a recorder.</summary>
public interface IGenerationProgress
{
    void Step(string message);

    void Warning(string message);
}

/// <summary>Discards all progress. Useful in tests and when running non-interactively.</summary>
public sealed class NullGenerationProgress : IGenerationProgress
{
    public static readonly NullGenerationProgress Instance = new();

    private NullGenerationProgress()
    {
    }

    public void Step(string message)
    {
    }

    public void Warning(string message)
    {
    }
}
