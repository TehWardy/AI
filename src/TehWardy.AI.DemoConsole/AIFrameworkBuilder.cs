using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TehWardy.AI.Agents;
using TehWardy.AI.Models;
using TehWardy.AI.Providers.Ollama;
using TehWardy.AI.Providers.ProviderFactories;
using TehWardy.AI.Providers.System;
using TehWardy.AI.Runbooks.Models;
using TehWardy.AI.Tools;
using TehWardy.AI.Tools.DotNet;
using TehWardy.AI.Tools.FileSystem;
using TehWardy.AI.Tools.Github;

namespace TehWardy.AI.DemoConsole;

internal static class AIFrameworkBuilder
{
    static ServiceProvider serviceProvider;

    public static void Build(string[] args, IDictionary<string, object> configurations)
    {
        var builder = Host.CreateApplicationBuilder(args);

        AgentConfiguration agentConfiguration =
            (AgentConfiguration)Data.UserConfiguration["Agent"];

        builder.Services.AddTehWardyAgenticAI(agentConfiguration, aiProviderConfiguration =>
        {
            // Configure Data Sources
            aiProviderConfiguration.WithGenericDataCacheProvider<Agent>();
            aiProviderConfiguration.WithGenericDataCacheProvider<Conversation>();
            aiProviderConfiguration.WithGenericDataCacheProvider<Runbook>();

            // Configure Rag Store Source
            aiProviderConfiguration.WithInMemoryMemoryProvider("MemCache");

            // Configure Model Provision
            aiProviderConfiguration.WithOllamaHostConfiguration(ollamaHostConfig =>
            {
                ollamaHostConfig.HostUrl = "http://localhost:11434";
            });

            aiProviderConfiguration.WithOllamaLargeLanguageModelProvider("Ollama");
            aiProviderConfiguration.WithOllamaEmbeddingProvider("Ollama");

            // Configure Runbook Step Handlers
            aiProviderConfiguration.WithRunbookStepHandlers(stepHandlingConfiguration =>
            {
                stepHandlingConfiguration.AddReasoningStepHandler("reason");
                stepHandlingConfiguration.AddToolCallingStepHandler("toolcall");
                stepHandlingConfiguration.AddRespondingStepHandler("respond");
                stepHandlingConfiguration.AddLoopingStepHandler("loop");
            });

            // Configure Tools For AI Use
            aiProviderConfiguration.WithExternalProcessExecutionProvider();
            aiProviderConfiguration.WithTools(toolConfiguration =>
            {
                toolConfiguration.AddFileSystemTool(serviceProvider =>
                    configurations["FileSystem"] as FileSystemConfiguration);

                toolConfiguration.AddGithubTool(serviceProvider =>
                    configurations["Github"] as GithubConfiguration);

                toolConfiguration.AddDotNetTool(serviceProvider =>
                    configurations["DotNet"] as DotNetConfiguration);

                toolConfiguration.AddStandardArchitectureTool();
            });
        });

        serviceProvider = builder.Services.BuildServiceProvider();
    }

    public static async ValueTask CacheAgents(Agent[] agents)
    {
        var agentCacheProvider = await serviceProvider
            .GetService<IDataCacheProviderFactory>()
            .CreateDataCacheProviderAsync<Agent>("Default");

        foreach (var agent in agents)
            await agentCacheProvider.AddOrUpdateAsync(agent.Name, agent);
    }

    public static async ValueTask CacheRunbooks(Runbook[] runbooks)
    {
        var runbookCacheProvider = await serviceProvider
            .GetService<IDataCacheProviderFactory>()
            .CreateDataCacheProviderAsync<Runbook>("Default");

        foreach (var runbook in runbooks)
            await runbookCacheProvider.AddOrUpdateAsync(runbook.Name, runbook);
    }

    public static IAgenticConversation GetAgenticConversation() =>
        serviceProvider.GetService<IAgenticConversation>();
}
