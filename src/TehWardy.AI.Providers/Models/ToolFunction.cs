namespace TehWardy.AI.Providers.Models;

public class ToolFunction
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ToolParameter[] Parameters { get; set; }
}
