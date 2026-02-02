using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools;

public partial class ToolDefinitions
{
    public static Tool FileSystem = new Tool
    {
        Name = "FileSystemTool",
        ToolFunctions =
        [
            new ToolFunction
            {
                Name = "DirectoryExistsAsync",
                Description = "Check if a directory exists.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "path",
                        Type = "string",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "FileExistsAsync",
                Description = "Check if a file exists.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "path",
                        Type = "string",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "ListAllWithinPathAsync",
                Description = "Lists all child files and folders within a given path, optionally recursive.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "path",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "recusive",
                        Type = "boolean",
                        Required = false
                    }
                ]
            },
            new ToolFunction
            {
                Name = "ListDirectoriesAsync",
                Description = "Lists all child directories within a given path.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "path",
                        Type = "string",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "ListFilesAsync",
                Description = "Lists all child files within a given path.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "path",
                        Type = "string",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "ReadTextFromFileAsync",
                Description = "Reads all the text from a file at the given path as a string.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "path",
                        Type = "string",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "StreamFileAsync",
                Description = "Reads from a file as a stream.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "path",
                        Type = "string",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "WriteTextToFileAsync",
                Description = "Writes text to a file, if the file does not exist will also create it, returns a confirmation that the file exists by calling FileExistsAsync(path) when done.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "path",
                        Type = "string",
                        Required = true
                    },
                    new ToolParameter
                    {
                        Name = "text",
                        Type = "string",
                        Required = true
                    }
                ]
            },
            new ToolFunction
            {
                Name = "CreateDirectoryAsync",
                Description = "Creates a new directory at the given path (if it does not exist), does nothing if the directory already exists returns a boolean to confirm that it completed the operation.",
                Parameters =
                [
                    new ToolParameter
                    {
                        Name = "path",
                        Type = "string",
                        Required = true
                    }
                ]
            }
        ]
    };
}