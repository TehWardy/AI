using AIServer.Ollama.Foundations;

namespace AIServer.Llama;

public class OllamaServiceHost : IOllamaServiceHost
{
    private readonly IOllamaModelHostService hostService;

    public OllamaServiceHost(IOllamaModelHostService hostService) =>
        this.hostService = hostService;

    public IAsyncEnumerable<string> StartAsync() =>
        hostService.StartOllamaHostProcess();

    public async ValueTask StopAsync() =>
        await hostService.StopOllamaHostProcessAsync();
}