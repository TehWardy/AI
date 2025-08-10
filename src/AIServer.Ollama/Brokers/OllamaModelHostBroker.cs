using System.Diagnostics;
using AIServer.Ollama.Configurations;

namespace AIServer.Ollama.Brokers;

public class OllamaModelHostBroker : IOllamaModelHostBroker
{
    private Process ollamaHostProcess = null;
    private readonly OllamaHostConfiguration config;

    public OllamaModelHostBroker(OllamaHostConfiguration config) =>
        this.config = config;

    public async ValueTask<Process> CreateOllamaModelDownloadProcessAsync(string model)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = config.OllamaExePath,
                Arguments = "pull " + model,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
    }

    public async ValueTask<Process> CreateOllamaHostProcessAsync()
    {
        ollamaHostProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = config.OllamaExePath,
                Arguments = "serve",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        return ollamaHostProcess;
    }

    public async ValueTask StopOllamaHostProcessAsync()
    {
        ollamaHostProcess.Kill();
        ollamaHostProcess.Dispose();
    }

    public async ValueTask<bool> IsHostProcessRunningAsync() => 
        ollamaHostProcess is not null;
}