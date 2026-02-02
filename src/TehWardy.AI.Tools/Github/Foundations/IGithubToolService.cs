using TehWardy.AI.Providers.Models;
using TehWardy.AI.Tools.Github.Models;

namespace TehWardy.AI.Tools;

internal interface IGithubToolService
{
    IAsyncEnumerable<ProcessToken> CloneRepoAsync(GithubCloneCommandRequest request);
    IAsyncEnumerable<ProcessToken> CommitAsync(GithubCommitCommandRequest request);
    IAsyncEnumerable<ProcessToken> CreateBranchAsync(GithubCreateBranchCommandRequest request);
    IAsyncEnumerable<ProcessToken> CreatePullRequestAsync(CreatePullRequestRequest request);
    IAsyncEnumerable<ProcessToken> PushAsync(GithubPushCommandRequest request);
}