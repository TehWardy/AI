namespace TehWardy.AI.Providers.Models;

public class Tool
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<ToolFunction> ToolFunctions { get; set; }
}