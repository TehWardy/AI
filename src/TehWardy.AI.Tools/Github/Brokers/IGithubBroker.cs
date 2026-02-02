using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools;

internal interface IGithubBroker
{
    IAsyncEnumerable<ProcessToken> CloneRepoAsync(string repoUrl, string targetPath);
    IAsyncEnumerable<ProcessToken> CommitAsync(string repoPath, string message);
    IAsyncEnumerable<ProcessToken> CreateBranchAsync(string repoPath, string branchName);
    IAsyncEnumerable<ProcessToken> PushAsync(string repoPath, string branchName);
    ValueTask<HttpResponseMessage> CreatePullRequestAsync(HttpRequestMessage createPullRequestMessage);
}