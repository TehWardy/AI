using TehWardy.AI.Models;

namespace TehWardy.AI.API.Configuration;

internal static partial class Data
{
    public static Agent[] Agents() =>
     [
        new()
        {
            Name = "Default",
            Description = "The Default Agent, use this for all user queries that don't fit other Agents.",
            RunbookName = "Default"
        },
        new()
        {
            Name = "Architect",
            Description = "Use this Agent to solve programming questions that involve building out software architectures. " +
            "This agent builds out Architecture Specifications, for later use with the Programmer Agent",
            RunbookName = "Architect", 
            UIToolName = "ArchitectureDesigner"
        },
        new()
        {
            Name = "Programmer",
            Description = "Use this Agent to solve programming questions that involve writing code such as bug fixes, creating new code and dealing with code repositories (like github).",
            RunbookName = "Default"
        }
     ];
}