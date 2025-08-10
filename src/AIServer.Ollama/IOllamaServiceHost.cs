
namespace AIServer.Llama;

public interface IOllamaServiceHost
{
    IAsyncEnumerable<string> StartAsync();
    ValueTask StopAsync();
}