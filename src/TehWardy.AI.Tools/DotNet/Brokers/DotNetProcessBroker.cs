using TehWardy.AI.Providers;
using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Tools.DotNet.Brokers;

internal class DotNetProcessBroker(IExternalProcessProvider externalProcessProvider, DotNetConfiguration config)
    : IDotNetProcessBroker
{
    public IAsyncEnumerable<ProcessToken> ExecuteDotNetAsync(string arguments, string workingDirectory)
    {
        return externalProcessProvider.ExecuteProcessAsync(
            executablePath: config.DotNetExePath,
            arguments: arguments,
            workingDirectory: workingDirectory);
    }

    public bool PathExists(string path) =>
        File.Exists(path) || Directory.Exists(path);

    public void CreateDirectory(string path) =>
        Directory.CreateDirectory(path);
}
