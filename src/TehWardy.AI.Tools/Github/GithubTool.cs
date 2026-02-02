using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools.Github;

internal class GithubTool(IGithubToolProcessingService githubToolProcessingService) : IGithubTool
{
    public string GetLocalPathForRepoUrl(string repoUrl) =>
        githubToolProcessingService.GetLocalPathForRepoUrl(repoUrl);

    public IAsyncEnumerable<ProcessToken> CloneRepoAsync(string repoUrl) =>
        githubToolProcessingService.CloneRepoAsync(repoUrl);

    public IAsyncEnumerable<ProcessToken> CommitAsync(string repoUrl, string commitMessage) =>
        githubToolProcessingService.CommitAsync(repoUrl, commitMessage);

    public IAsyncEnumerable<ProcessToken> CreateBranchAsync(string repoUrl, string branchName) =>
        githubToolProcessingService.CreateBranchAsync(repoUrl, branchName);

    public IAsyncEnumerable<ProcessToken> CreatePullRequestAsync(string repoUrl, string headBranchName, string title) =>
        githubToolProcessingService.CreatePullRequestAsync(repoUrl, headBranchName, title);

    public IAsyncEnumerable<ProcessToken> PushAsync(string repoUrl, string branchName) =>
        githubToolProcessingService.PushAsync(repoUrl, branchName);
}