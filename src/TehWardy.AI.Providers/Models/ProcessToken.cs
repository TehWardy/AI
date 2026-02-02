namespace TehWardy.AI.Providers.Models;

public class ProcessToken
{
    public ProcessStreamSource StreamSource { get; set; }
    public string Value { get; set; }
    public bool IsFinalToken { get; set; }
    public int ExitCode { get; set; }
}
