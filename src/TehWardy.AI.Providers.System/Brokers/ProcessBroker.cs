using System.Diagnostics;

namespace TehWardy.AI.Providers.System.Brokers;

internal class ProcessBroker : IProcessBroker
{
    public Process CreateProcess(string executablePath, string workingDirectory, string arguments)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        return new Process
        {
            StartInfo = processStartInfo,
            EnableRaisingEvents = true
        };
    }
}
