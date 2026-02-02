using System.Diagnostics;

namespace TehWardy.AI.Providers.System.Brokers;

internal interface IProcessBroker
{
    Process CreateProcess(string executablePath, string workingDirectory, string arguments);
}