namespace TehWardy.AI.Tools.Github.Models;

public sealed class CreatePullRequestRequest
{
    public string RepoUrl { get; set; }
    public string RepoPath { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string HeadBranch { get; set; }
    public string BaseBranch { get; set; }
    public string Owner { get; set; }
    public string RepoName { get; set; }
}