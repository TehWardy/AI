namespace TehWardy.AI.Providers.Models;

public class Token
{
    public string Content { get; set; }
    public string Thought { get; set; }
    public ToolCall[] ToolCalls { get; set; }
}
