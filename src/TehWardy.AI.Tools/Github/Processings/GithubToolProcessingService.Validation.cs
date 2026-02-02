using TehWardy.AI.Tools;
using TehWardy.AI.Tools.Github.Models;

namespace TehWardy.Github.Processings;

internal partial class GithubToolProcessingService
{
    static void ValidateGithubCreateBranchCommandRequest(GithubCreateBranchCommandRequest request)
    {
        ValidateRequest(request);
        ValidateRepoUrl(request.RepoUrl);
        ValidateRepoPath(request.RepoPath);

        if (string.IsNullOrWhiteSpace(request.BranchName))
            throw new ArgumentException("BranchName is required.", nameof(request.BranchName));
    }

    static void ValidateGithubCommitCommandRequest(GithubCommitCommandRequest request)
    {
        ValidateRequest(request);
        ValidateRepoUrl(request.RepoUrl);
        ValidateRepoPath(request.RepoPath);

        if (string.IsNullOrWhiteSpace(request.CommitMessage))
            throw new ArgumentException("CommitMessage is required.", nameof(request.CommitMessage));
    }

    static void ValididateGithubPushCommandRequest(GithubPushCommandRequest request)
    {
        ValidateRequest(request);
        ValidateRepoUrl(request.RepoUrl);
        ValidateRepoPath(request.RepoPath);

        if (string.IsNullOrWhiteSpace(request.BranchName))
            throw new ArgumentException("BranchName is required.", nameof(request.BranchName));
    }

    static void ValidatePullRequest(CreatePullRequestRequest request)
    {
        ValidateRequest(request);
        ValidateRepoUrl(request.RepoUrl);

        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.HeadBranch))
            throw new ArgumentException("HeadBranch is required.", nameof(request));

        if (string.IsNullOrWhiteSpace(request.BaseBranch))
            throw new ArgumentException("BaseBranch is required.", nameof(request));
    }

    static void ValidateRepoPath(string repoPath)
    {
        if (string.IsNullOrWhiteSpace(repoPath))
            throw new ArgumentException("RepoPath is required.", nameof(repoPath));
    }

    static void ValidateRequest(object request)
    {
        if (request is null)
            throw new ArgumentException("request is required.");
    }

    static void ValidateRepoUrl(string repoUrl)
    {
        // must not be null or empty
        if (string.IsNullOrWhiteSpace(repoUrl))
            throw new ArgumentException("RepoUrl is required.", nameof(repoUrl));

        // git@github.com:owner/repo(.git)
        if (repoUrl.StartsWith("git@github.com:", StringComparison.OrdinalIgnoreCase))
            return;

        // must be an absolute URI to github.com
        if (!Uri.TryCreate(repoUrl, UriKind.Absolute, out var uri))
            throw new ArgumentException("RepoUrl is not a valid absolute URI.", nameof(repoUrl));

        // host must be exactly "github.com"
        if (!string.Equals(uri.Host, "github.com", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("RepoUrl must point to github.com.", nameof(repoUrl));
    }
}