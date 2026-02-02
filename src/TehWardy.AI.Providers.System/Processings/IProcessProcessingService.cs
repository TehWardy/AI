using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers;
internal interface IProcessProcessingService
{
    IAsyncEnumerable<ProcessToken> ExecuteProcessAsync(string executablePath, string workingDirectory, string arguments);
}