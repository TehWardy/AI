using TehWardy.AI.Providers;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools.Github.Brokers;

internal class GithubBroker : IGithubBroker
{
    readonly IExternalProcessProvider processProvider;
    readonly HttpClient httpClient;
    readonly GithubConfiguration config;

    public GithubBroker(GithubConfiguration config, HttpClient httpClient, IExternalProcessProvider processProvider)
    {
        this.config = config;
        this.processProvider = processProvider;
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri("https://api.github.com/");
        this.httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("TehWardyAgenticAI/1.0");
        this.httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
    }

    public IAsyncEnumerable<ProcessToken> CloneRepoAsync(
        string repoUrl,
        string targetPath)
    {
        string parent = Directory.GetParent(targetPath).FullName;

        if (!Directory.Exists(parent))
            Directory.CreateDirectory(parent);

        string args = $"{BuildGitAuthConfigArgs()} clone {Escape(repoUrl)} {Escape(targetPath)}";
        return RunAsync(parent, args);
    }

    public IAsyncEnumerable<ProcessToken> CreateBranchAsync(
        string repoPath,
        string branchName)
    {
        string args = $"{BuildGitAuthConfigArgs()} checkout -b {Escape(branchName)}";
        return RunAsync(repoPath, args);
    }

    public async IAsyncEnumerable<ProcessToken> CommitAsync(
        string repoPath,
        string message)
    {
        await foreach (var token in RunAsync(repoPath, $"{BuildGitAuthConfigArgs()} add -A"))
            yield return token;

        await foreach (var token in RunAsync(repoPath, $"{BuildGitAuthConfigArgs()} commit -m {Escape(message)}"))
            yield return token;
    }

    public IAsyncEnumerable<ProcessToken> PushAsync(
        string repoPath,
        string branchName)
    {
        string args = $"{BuildGitAuthConfigArgs()} push -u origin {Escape(branchName)}";
        return RunAsync(repoPath, args);
    }

    public async ValueTask<HttpResponseMessage> CreatePullRequestAsync(
        HttpRequestMessage createPullRequestMessage)
    {
        createPullRequestMessage.Headers.Authorization = new System.Net.Http.Headers
            .AuthenticationHeaderValue("Bearer", config.AuthToken);

        return await httpClient.SendAsync(createPullRequestMessage);
    }

    IAsyncEnumerable<ProcessToken> RunAsync(string workingDirectory, string arguments) =>
        processProvider.ExecuteProcessAsync(config.GitExePath, workingDirectory, arguments);

    string BuildGitAuthConfigArgs()
    {
        if (string.IsNullOrWhiteSpace(config.AuthToken))
            return string.Empty;

        string basic = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"x-access-token:{config.AuthToken}"));

        // -c http.extraHeader="AUTHORIZATION: basic <base64>"
        return $"-c http.extraHeader=\"AUTHORIZATION: basic {basic}\"";
    }

    private static string Escape(string value) =>
        "\"" + value.Replace("\"", "\\\"") + "\"";
}