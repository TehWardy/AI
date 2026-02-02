namespace TehWardy.AI.Providers.Models;

public class ToolParameter
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Required { get; set; }
    public ToolParameter[] Properties { get; set; }
}