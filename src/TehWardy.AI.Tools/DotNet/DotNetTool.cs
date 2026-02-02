using TehWardy.AI.Providers.Models;
using TehWardy.AI.Tools.DotNet.Processings;

namespace TehWardy.AI.Tools.DotNet;

internal class DotNetTool(IDotNetProcessingService dotNetProcessingService) : IDotNetTool
{
    public IAsyncEnumerable<ProcessToken> DotNetTestAsync(string solutionOrProjectPath, string workingDirectory, string configuration, bool noBuild, string filter) =>
        dotNetProcessingService.DotNetTestAsync(solutionOrProjectPath, workingDirectory, configuration, noBuild, filter);


    public IAsyncEnumerable<ProcessToken> DotNetBuildAsync(string solutionOrProjectPath, string workingDirectory, string configuration) =>
        dotNetProcessingService.DotNetBuildAsync(solutionOrProjectPath, workingDirectory, configuration);


    public IAsyncEnumerable<ProcessToken> DotNetFormatAsync(string solutionOrProjectPath, string workingDirectory, bool verifyNoChanges) =>
        dotNetProcessingService.DotNetFormatAsync(solutionOrProjectPath, workingDirectory, verifyNoChanges);


    public IAsyncEnumerable<ProcessToken> DotnetNewAsync(string template, string outputDirectory, string name, IDictionary<string, string> additionalArgs = null) =>
        dotNetProcessingService.DotnetNewAsync(template, outputDirectory, name, additionalArgs ?? new Dictionary<string, string>());


    public IAsyncEnumerable<ProcessToken> DotNetAddReferenceAsync(string projectPath, string workingDirectory, string referencePath) =>
        dotNetProcessingService.DotNetAddReference(projectPath, workingDirectory, referencePath);
}
