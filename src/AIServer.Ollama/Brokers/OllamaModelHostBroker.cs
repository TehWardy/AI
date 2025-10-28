using System.Diagnostics;
using AIServer.Ollama.Configurations;

namespace AIServer.Ollama.Brokers;

public class OllamaModelHostBroker : IOllamaModelHostBroker
{
    private Process ollamaHostProcess = null;
    private readonly OllamaHostConfiguration config;

    public OllamaModelHostBroker(OllamaHostConfiguration config) =>
        this.config = config;

    public ValueTask<Process> CreateOllamaModelDownloadProcessAsync(string model)
    {
        var result = new Process
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

        return ValueTask.FromResult(result);
    }

    public ValueTask<Process> CreateOllamaHostProcessAsync()
    {
        var start = new ProcessStartInfo
        {
            FileName = config.OllamaExePath,
            Arguments = "serve",
            WorkingDirectory = Path.GetDirectoryName(config.OllamaExePath),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        start.EnvironmentVariables["OLLAMA_HOST"] = config.OllamaHostUrl;
        start.EnvironmentVariables["OLLAMA_MODELS"] = config.OllamaModelsPath;

        ollamaHostProcess = new Process { StartInfo = start };

        return ValueTask.FromResult(ollamaHostProcess);
    }

    public ValueTask StopOllamaHostProcessAsync()
    {
        ollamaHostProcess.Kill();
        ollamaHostProcess.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> IsHostProcessRunningAsync() => 
        ValueTask.FromResult(ollamaHostProcess is not null);
}