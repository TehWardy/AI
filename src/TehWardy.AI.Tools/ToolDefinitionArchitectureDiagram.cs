using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools;

public partial class ToolDefinitions
{
    public static Tool ArchitectureDiagram = new()
    {
        Name = "ArchitectureDiagramTool",
        ToolFunctions =
        [
            new ToolFunction
            {
                Name = "ParseDiagram",
                Description = "Validates & normalizes the given architecture specification.",
                Parameters =
                [
                    new ToolParameter { Type = "string", Name = "diagramJson", Required = true }
                ]
            }
        ]
    };
}