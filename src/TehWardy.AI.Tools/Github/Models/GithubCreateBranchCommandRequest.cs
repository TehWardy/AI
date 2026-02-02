namespace TehWardy.AI.Tools;

public class GithubCreateBranchCommandRequest
{
    public string RepoPath { get; set; }
    public string RepoUrl { get; set; }
    public string BranchName { get; set; }
}