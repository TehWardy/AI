using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools;

public partial class ToolDefinitions
{
    public static Tool Variable = new()
    {
        Name = "Variable",
        ToolFunctions =
        [
            new ToolFunction
            {
                Name = "SetBool",
                Description = "Sets a boolean state variable to the given value.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "key",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "value",
                        Type = "bool",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "SetString",
                Description = "Sets a string state variable to the given value.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "key",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "value",
                        Type = "string",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "SetInt",
                Description = "Sets a integer state variable to the given value.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "key",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "value",
                        Type = "integer",
                        Required = true
                    }
                ]
            }
        ]
    };
}