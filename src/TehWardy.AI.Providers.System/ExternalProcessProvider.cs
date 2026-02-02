using TehWardy.AI.Providers.Models;

namespace TehWardy.AI.Providers;

internal class ExternalProcessProvider(IProcessProcessingService processProcessingService)
    : IExternalProcessProvider
{
    public IAsyncEnumerable<ProcessToken> ExecuteProcessAsync(string executablePath, string workingDirectory, string arguments) =>
        processProcessingService.ExecuteProcessAsync(executablePath, workingDirectory, arguments);
}