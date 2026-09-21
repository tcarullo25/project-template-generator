namespace ProjectTemplateGenerator.Tests;

/// <summary>
/// A unique directory under the system temp path, deleted on dispose.
/// Generation tests write here and nowhere else, so running the suite never
/// touches the repository.
/// </summary>
public sealed class TemporaryDirectory : IDisposable
{
    public TemporaryDirectory()
    {
        Path = System.IO.Path.Combine(
            System.IO.Path.GetTempPath(),
            "ptg-tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
        catch (IOException)
        {
            // A locked file in a temp directory is not worth failing a test over.
        }
    }
}
