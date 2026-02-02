using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools;

public partial class ToolDefinitions
{
    public static Tool Github = new Tool
    {
        Name = "GithubTool",
        ToolFunctions =
        [
            new ToolFunction
            {
                Name = "GetLocalPathForRepoUrl",
                Description = "Gets the computed local path for a repo when it is cloned from the repo url and the configuration root path.",
                Parameters =
                [
                    new ToolParameter { Type = "string", Name = "repoUrl", Required = true }
                ]
            },
            new ToolFunction
            {
                Name = "CloneRepoAsync",
                Description = "Clones a github repository.",
                Parameters =
                [
                    new ToolParameter { Type = "string", Name = "repoUrl", Required = true }
                ]
            },
            new ToolFunction
            {
                Name = "CreateBranchAsync",
                Description = "Creates a new Branch in a github repository.",
                Parameters =
                [
                    new ToolParameter { Type = "string", Name = "repoUrl", Required = true },
                    new ToolParameter { Type = "string", Name = "branchName", Required = true }
                ]
            },
            new ToolFunction
            {
                Name = "CommitAsync",
                Description = "Commits changes to a github repository.",
                Parameters =
                [
                    new ToolParameter { Type = "string", Name = "repoUrl", Required = true },
                    new ToolParameter { Type = "string", Name = "commitMessage", Required = true }
                ]
            },
            new ToolFunction
            {
                Name = "CreatePullRequestAsync",
                Description = "Creates a new pull request on a github repository.",
                Parameters =
                [
                    new ToolParameter { Type = "string", Name = "repoUrl", Required = true },
                    new ToolParameter { Type = "string", Name = "headBranchName", Required = true },
                    new ToolParameter { Type = "string", Name = "title", Required = true }
                ]
            },
            new ToolFunction
            {
                Name = "PushAsync",
                Description = "Pushes a github repository state up to the remote source.",
                Parameters =
                [
                    new ToolParameter { Type = "string", Name = "repoUrl", Required = true },
                    new ToolParameter { Type = "string", Name = "branchName", Required = true }
                ]
            }
        ]
    };
}