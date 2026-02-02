using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools.DotNet.Brokers;

internal interface IDotNetProcessBroker
{
    IAsyncEnumerable<ProcessToken> ExecuteDotNetAsync(string arguments, string workingDirectory);
    bool PathExists(string path);
    void CreateDirectory(string path);
}