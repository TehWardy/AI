using TehWardy.AI.Providers.Models;
using TehWardy.AI.Tools;
using TehWardy.AI.Tools.Github;
using TehWardy.AI.Tools.Github.Models;

namespace TehWardy.Github.Processings;

internal partial class GithubToolProcessingService(GithubConfiguration githubConfig, IGithubToolService githubToolService)
    : IGithubToolProcessingService
{
    public string GetLocalPathForRepoUrl(string repoUrl) =>
        ComputeRepoPath(repoUrl).Replace(githubConfig.LocalRepoRoot, "");

    public IAsyncEnumerable<ProcessToken> CloneRepoAsync(string repoUrl)
    {
        ValidateRepoUrl(repoUrl);
        string repoPath = ComputeRepoPath(repoUrl);

        GithubCloneCommandRequest request = new()
        {
            LocalRepoPath = repoPath,
            RepoUrl = repoUrl
        };

        return githubToolService.CloneRepoAsync(request);
    }

    public IAsyncEnumerable<ProcessToken> CreateBranchAsync(string repoUrl, string branchName)
    {
        GithubCreateBranchCommandRequest request = new()
        {
            RepoUrl = repoUrl,
            BranchName = branchName,
            RepoPath = ComputeRepoPath(repoUrl)
        };

        ValidateGithubCreateBranchCommandRequest(request);
        return githubToolService.CreateBranchAsync(request);
    }

    public IAsyncEnumerable<ProcessToken> CommitAsync(string repoUrl, string commitMessage)
    {
        GithubCommitCommandRequest request = new()
        {
            RepoUrl = repoUrl,
            CommitMessage = commitMessage,
            RepoPath = ComputeRepoPath(repoUrl)
        };

        ValidateGithubCommitCommandRequest(request);
        return githubToolService.CommitAsync(request);
    }

    public IAsyncEnumerable<ProcessToken> PushAsync(string repoUrl, string branchName)
    {
        GithubPushCommandRequest request = new()
        {
            RepoUrl = repoUrl,
            BranchName = branchName,
            RepoPath = ComputeRepoPath(repoUrl)
        };

        ValididateGithubPushCommandRequest(request);
        return githubToolService.PushAsync(request);
    }

    public IAsyncEnumerable<ProcessToken> CreatePullRequestAsync(string repoUrl, string headBranchName, string title)
    {
        var (owner, repo) = ParseOwnerRepo(repoUrl);

        CreatePullRequestRequest request = new()
        {
            RepoUrl = repoUrl,
            RepoPath = ComputeRepoPath(repoUrl),
            HeadBranch = headBranchName,
            BaseBranch = "main",
            Body = null,
            Title = title,
            Owner = owner,
            RepoName = repo
        };

        ValidatePullRequest(request);

        return githubToolService.CreatePullRequestAsync(request);
    }

    string ComputeRepoPath(string repoUrl)
    {
        var (owner, repo) = ParseOwnerRepo(repoUrl);
        return Path.Combine(githubConfig.LocalRepoRoot, owner, repo);
    }

    static (string owner, string repo) ParseOwnerRepo(string repoUrl)
    {
        if (repoUrl.StartsWith("git@github.com:", StringComparison.OrdinalIgnoreCase))
        {
            string part = repoUrl["git@github.com:".Length..];
            part = part.EndsWith(".git", StringComparison.OrdinalIgnoreCase) ? part[..^4] : part;
            var bits = part.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return (bits[0], bits[1]);
        }

        var uri = new Uri(repoUrl);
        var segments = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length < 2)
            throw new ArgumentException($"Invalid repoUrl: {repoUrl}", nameof(repoUrl));

        string owner = segments[0];
        string repo = segments[1].EndsWith(".git", StringComparison.OrdinalIgnoreCase)
            ? segments[1][..^4]
            : segments[1];

        return (owner, repo);
    }
}