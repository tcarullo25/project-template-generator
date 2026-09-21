namespace ProjectTemplateGenerator.Cli;

public static class ExitCodes
{
    public const int Success = 0;

    /// <summary>Bad input or a refused operation. Reported as a plain message.</summary>
    public const int UserError = 1;

    /// <summary>A bug in the generator. Reported with a stack trace.</summary>
    public const int UnexpectedError = 2;
}
