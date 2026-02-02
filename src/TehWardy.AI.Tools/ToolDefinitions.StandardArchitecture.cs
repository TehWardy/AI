using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools;

public partial class ToolDefinitions
{
    public static Tool StandardArchitecture = new()
    {
        Name = "StandardArchitectureTool",
        ToolFunctions =
        [
            new ToolFunction
            {
                Name = "ValidateAndNormalize",
                Description = "Validates & normalizes the given architecture specification.",
                Parameters =
                [
                    new ToolParameter { Type = "string", Name = "architectureSpecJson", Required = true }
                ]
            }
        ]
    };
}