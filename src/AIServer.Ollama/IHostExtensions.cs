using AIServer.Llama;
using Microsoft.Extensions.Hosting;

namespace AIServer.Ollama;

public static class IHostExtensions
{
    public static async void StartOllama(this IHost host)
    {
        var serviceHost = (IOllamaServiceHost)host.Services
            .GetService(typeof(IOllamaServiceHost));

        IAsyncEnumerable<string> ollamaConsoleStream = 
            serviceHost.StartAsync();

        await foreach (string consoleLine in ollamaConsoleStream)
            Console.WriteLine(consoleLine);
    }
}