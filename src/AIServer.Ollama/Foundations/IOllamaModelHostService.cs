
namespace AIServer.Ollama.Foundations;

public interface IOllamaModelHostService
{
    IAsyncEnumerable<string> DownloadedModelAsync(string model);
    IAsyncEnumerable<string> StartOllamaHostProcess();
    ValueTask StopOllamaHostProcessAsync();
}