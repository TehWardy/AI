namespace TehWardy.AI.Tools.DotNet.Models;

internal class DotNetToken
{
    public DotNetTokenType TokenType { get; set; }
    public string Value { get; set; }
    public string Error { get; set; }
    public bool IsFinished { get; set; }
    public bool IsFinalToken { get; set; }
    public int ExitCode { get; set; }
}
