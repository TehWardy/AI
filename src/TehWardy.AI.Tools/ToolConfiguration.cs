using Microsoft.Extensions.DependencyInjection;
using TehWardy.AI.Tools.DotNet;
using TehWardy.AI.Tools.DotNet.Brokers;
using TehWardy.AI.Tools.DotNet.Foundations;
using TehWardy.AI.Tools.DotNet.Processings;
using TehWardy.AI.Tools.FileSystem;
using TehWardy.AI.Tools.FileSystem.Brokers;
using TehWardy.AI.Tools.FileSystem.Foundations;
using TehWardy.AI.Tools.FileSystem.Orchestration;
using TehWardy.AI.Tools.Github;
using TehWardy.AI.Tools.Github.Brokers;
using TehWardy.AI.Tools.Github.Foundations;
using TehWardy.AI.Tools.Standard;
using TehWardy.AI.Tools.Standard.Orchestrators;
using TehWardy.AI.Tools.Standard.Processings;
using TehWardy.Github.Processings;

namespace TehWardy.AI.Tools;

public class ToolConfiguration
{
    private readonly IServiceCollection services;

    internal ToolConfiguration(IServiceCollection services)
    {
        this.services = services;

        services.AddHttpClient();

    }

    public void AddFileSystemTool(Func<IServiceProvider, FileSystemConfiguration> getConfigurationFunc)
    {
        services.AddTransient(getConfigurationFunc);
        services.AddKeyedTransient<object, FileSystemTool>("FileSystem");
        services.AddTransient<IFileSystemOrchestrationService, FileSystemOrchestrationService>();
        services.AddTransient<IDirectoryService, DirectoryService>();
        services.AddTransient<IFileService, FileService>();
        services.AddTransient<IDirectoryBroker, DirectoryBroker>();
        services.AddTransient<IFileBroker, FileBroker>();
    }

    public void AddGithubTool(Func<IServiceProvider, GithubConfiguration> getConfigurationFunc)
    {
        services.AddTransient(getConfigurationFunc);
        services.AddKeyedTransient<object, GithubTool>("Github");
        services.AddTransient<IGithubToolProcessingService, GithubToolProcessingService>();
        services.AddTransient<IGithubToolService, GithubToolService>();
        services.AddTransient<IGithubBroker, GithubBroker>();
    }

    public void AddDotNetTool(Func<IServiceProvider, DotNetConfiguration> getConfigurationFunc)
    {
        services.AddTransient(getConfigurationFunc);
        services.AddKeyedTransient<object, DotNetTool>("DotNet");
        services.AddTransient<IDotNetProcessingService, DotNetProcessingService>();
        services.AddTransient<IDotNetService, DotNetService>();
        services.AddTransient<IDotNetProcessBroker, DotNetProcessBroker>();
    }

    public void AddStandardArchitectureTool()
    {
        services.AddKeyedTransient<object, StandardArchitectureTool>("StandardArchitecture");

        services.AddTransient<
            IArchitectureSpecificationOrchestrationService,
            ArchitectureSpecificationOrchestrationService>();

        services.AddTransient<
            IArchitectureNormalizerProcessingService,
            ArchitectureNormalizerProcessingService>();

        services.AddTransient<
            IArchitectureValidatorProcessingService,
            ArchitectureValidatorProcessingService>();

        services.AddTransient<
            IManifestGenerationProcessingService,
            ManifestGenerationProcessingService>();
    }
}