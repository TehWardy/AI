using Microsoft.Extensions.Hosting;

namespace AIServer.Ollama;

public static class IHostExtensions
{
    public static async ValueTask StartOllamaAsync(this IHost host)
    {
        var serviceHost = (IOllamaServiceHost)host.Services
            .GetService(typeof(IOllamaServiceHost));

        IAsyncEnumerable<string> ollamaConsoleStream = 
            serviceHost.StartAsync();

        await foreach (string consoleLine in ollamaConsoleStream)
            Console.WriteLine(consoleLine);
    }
}