using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers;

public interface IExternalProcessProvider
{
    IAsyncEnumerable<ProcessToken> ExecuteProcessAsync(string executablePath, string workingDirectory, string arguments);
}