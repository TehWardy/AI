using TehWardy.AI.Tools.DotNet;
using TehWardy.AI.Tools.FileSystem;
using TehWardy.AI.Tools.Github;

namespace TehWardy.AI.API.Configuration;

internal static partial class Data
{
    static readonly string userRoot = "E:\\AI\\Data\\Cache";

    public static IDictionary<string, object> UserConfiguration => new Dictionary<string, object>
    {
        {
            "Agent", new AgentConfiguration
            {
                RoutingProviderName = "Ollama",
                RoutingModelName = "gpt-oss:20b",
                RoutingContextLength = 64000
            }
        },
        {
            "Github", new GithubConfiguration
            {
                GitExePath = "git",
                AuthToken = Environment.GetEnvironmentVariable("AIGithubToken"),
                LocalRepoRoot = $"{userRoot}\\Github"
            }
        },
        {
            "DotNet", new DotNetConfiguration
            {
                DotNetExePath = "dotnet",
                RootWorkingDirectory = userRoot
            }
        },
        {
            "FileSystem", new FileSystemConfiguration
            {
                RootWorkingDirectory = userRoot
            }
        }
    };
}