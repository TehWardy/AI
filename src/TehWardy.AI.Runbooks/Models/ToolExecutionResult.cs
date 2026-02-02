namespace TehWardy.AI.Runbooks.Models;

internal class ToolExecutionToken
{
    public string ToolName { get; set; }
    public string FunctionName { get; set; }
    public IDictionary<string, object> Arguments { get; set; }
    public bool Succeeded { get; set; }
    public object Output { get; set; }
    public string Error { get; set; }
}