using System.Net.Http.Json;
using System.Text.Json;
using TehWardy.AI.Providers.Models;
using TehWardy.AI.Tools.Github.Models;

namespace TehWardy.AI.Tools.Github.Foundations;

internal class GithubToolService(IGithubBroker githubBroker)
    : IGithubToolService
{
    public IAsyncEnumerable<ProcessToken> CloneRepoAsync(GithubCloneCommandRequest request)
    {
        return githubBroker.CloneRepoAsync(
            request.RepoUrl,
            request.LocalRepoPath);
    }

    public IAsyncEnumerable<ProcessToken> CreateBranchAsync(
        GithubCreateBranchCommandRequest request)
    {
        ValidateRepoPath(request.RepoPath);

        return githubBroker.CreateBranchAsync(
            request.RepoPath,
            request.BranchName);
    }

    public IAsyncEnumerable<ProcessToken> CommitAsync(
        GithubCommitCommandRequest request)
    {
        ValidateRepoPath(request.RepoPath);

        return githubBroker.CommitAsync(
            request.RepoPath,
            request.CommitMessage);
    }

    public IAsyncEnumerable<ProcessToken> PushAsync(GithubPushCommandRequest request)
    {
        ValidateRepoPath(request.RepoPath);

        return githubBroker.PushAsync(
            request.RepoPath,
            request.BranchName);
    }

    public async IAsyncEnumerable<ProcessToken> CreatePullRequestAsync(
        CreatePullRequestRequest request)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"repos/{request.Owner}/{request.RepoName}/pulls");

        var payload = new
        {
            title = request.Title,
            body = request.Body,
            head = request.HeadBranch,
            @base = request.BaseBranch
        };

        httpRequest.Content = JsonContent.Create(payload);

        using var response = await githubBroker.CreatePullRequestAsync(httpRequest);

        string raw = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"GitHub PR create failed: {(int)response.StatusCode} {response.ReasonPhrase}\n{raw}");

        using var jsonDocument = JsonDocument.Parse(raw);

        var pr = new CreatePullRequestResponse
        {
            Url = jsonDocument.RootElement.GetProperty("html_url").GetString() ?? "",
            Number = jsonDocument.RootElement.GetProperty("number").GetInt32()
        };

        yield return new ProcessToken
        {
            ExitCode = 0,
            IsFinalToken = true,
            StreamSource = ProcessStreamSource.StdOut,
            Value = JsonSerializer.Serialize(pr)
        };
    }

    static void ValidateRepoPath(string repoPath)
    {
        if (!Directory.Exists(repoPath))
            throw new DirectoryNotFoundException(repoPath);

        if (!Directory.Exists(Path.Combine(repoPath, ".git")))
            throw new InvalidOperationException($"'{repoPath}' is not a git repository (missing .git).");
    }
}