using System.Diagnostics;

namespace AIServer.Ollama.Brokers;
public interface IOllamaModelHostBroker
{
    ValueTask<Process> CreateOllamaHostProcessAsync();
    ValueTask<Process> CreateOllamaModelDownloadProcessAsync(string model);
    ValueTask StopOllamaHostProcessAsync();
    ValueTask<bool> IsHostProcessRunningAsync();
}