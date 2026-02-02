namespace TehWardy.AI.Tools;

public sealed class GitCommandResult
{
    public int ExitCode { get; init; }
    public string StdOut { get; init; } = "";
    public string StdErr { get; init; } = "";
}

