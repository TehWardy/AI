using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools;

public interface IGithubTool
{
    string GetLocalPathForRepoUrl(string repoUrl);
    IAsyncEnumerable<ProcessToken> CloneRepoAsync(string repoUrl);
    IAsyncEnumerable<ProcessToken> CommitAsync(string repoUrl, string commitMessage);
    IAsyncEnumerable<ProcessToken> CreateBranchAsync(string repoUrl, string branchName);
    IAsyncEnumerable<ProcessToken> CreatePullRequestAsync(string repoUrl, string headBranchName, string title);
    IAsyncEnumerable<ProcessToken> PushAsync(string repoUrl, string branchName);
}