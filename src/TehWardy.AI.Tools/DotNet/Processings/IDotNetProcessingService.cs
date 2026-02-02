using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools.DotNet.Processings;
internal interface IDotNetProcessingService
{
    IAsyncEnumerable<ProcessToken> DotNetAddReference(string projectPath, string workingDirectory, string referencePath);
    IAsyncEnumerable<ProcessToken> DotNetBuildAsync(string solutionOrProjectPath, string workingDirectory, string configuration);
    IAsyncEnumerable<ProcessToken> DotNetFormatAsync(string solutionOrProjectPath, string workingDirectory, bool verifyNoChanges);
    IAsyncEnumerable<ProcessToken> DotnetNewAsync(string template, string outputDirectory, string name, IDictionary<string, string> additionalArgs);
    IAsyncEnumerable<ProcessToken> DotNetTestAsync(string solutionOrProjectPath, string workingDirectory, string configuration, bool noBuild, string filter);
}