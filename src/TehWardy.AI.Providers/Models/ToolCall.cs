namespace TehWardy.AI.Providers.Models;

public class ToolCall
{
    public string ToolName { get; set; }
    public string FunctionName { get; set; }
    public IDictionary<string, object> Arguments { get; set; }
}