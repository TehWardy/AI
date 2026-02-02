using System.Diagnostics;
using TehWardy.AI.Providers.Foundations;
using TehWardy.AI.Providers.System.Brokers;

namespace TehWardy.AI.Providers.System.Foundations;

internal class ProcessService(IProcessBroker processBroker) : IProcessService
{
    public Process CreateProcess(string executablePath, string workingDirectory, string arguments) =>
        processBroker.CreateProcess(executablePath, workingDirectory, arguments);
}
