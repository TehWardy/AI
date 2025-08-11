
namespace AIServer.Ollama;

public interface IOllamaServiceHost
{
    IAsyncEnumerable<string> StartAsync();
    ValueTask StopAsync();
}