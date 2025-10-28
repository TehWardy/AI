using Microsoft.Extensions.Hosting;

namespace AIServer.Ollama;

public static class IHostExtensions
{
    static IOllamaServiceHost serviceHost;

    public static async ValueTask StartOllamaAsync(this IHost host)
    {
        serviceHost = (IOllamaServiceHost)host.Services
            .GetService(typeof(IOllamaServiceHost));

        IAsyncEnumerable<string> ollamaConsoleStream = 
            serviceHost.StartAsync();

        await foreach (string consoleLine in ollamaConsoleStream)
            Console.WriteLine(consoleLine);
    }

    public static async ValueTask StopOllamaAsync(this IHost host) =>
        await serviceHost.StopAsync();
}