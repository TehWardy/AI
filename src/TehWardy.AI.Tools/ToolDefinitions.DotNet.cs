using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools;

public partial class ToolDefinitions
{
    public static Tool DotNet = new Tool
    {
        Name = "DotNetTool",
        ToolFunctions =
        [
            new ToolFunction
            {
                Name = "DotNetAddReferenceAsync",
                Description = "Adds a reference to the project.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "projectPath",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "workingDirectory",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "referencePath",
                        Type = "string",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "DotNetBuildAsync",
                Description = "Builds a project or solution.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "solutionOrProjectPath",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "workingDirectory",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "configuration",
                        Type = "string",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "DotNetFormatAsync",
                Description = "Formats a project or solution.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "solutionOrProjectPath",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "workingDirectory",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "verifyNoChanges",
                        Type = "boolean",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "DotnetNewAsync",
                Description = "Creates a new project.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "template",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "outputDirectory",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "name",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "additionalArgs",
                        Type = "object",
                        Required = false
                    }
                ]
            }
        ]
    };
}