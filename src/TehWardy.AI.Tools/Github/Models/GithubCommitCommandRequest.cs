namespace TehWardy.AI.Tools.Github.Models;

public class GithubCommitCommandRequest
{
    public string RepoPath { get; set; }
    public string RepoUrl { get; set; }
    public string CommitMessage { get; set; }
}