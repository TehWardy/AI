using AIServer.LlamaCpp.Foundations;
using Microsoft.Extensions.Hosting;

namespace AIServer.LlamaCpp;

public static class IHostExtensions
{
    static ILlamaCppHostService serviceHost;

    public static async ValueTask StartLlamaHostAsync(this IHost host, string modelName)
    {
        if (serviceHost is null)
        {
            serviceHost = (ILlamaCppHostService)host.Services
                .GetService(typeof(ILlamaCppHostService));

            IAsyncEnumerable<string> llamaCppConsoleStream =
                serviceHost.StartAsync(modelName);

            await foreach (string consoleLine in llamaCppConsoleStream)
                Console.WriteLine(consoleLine);
        }
    }

    public static ValueTask StopLlamaHostAsync(this IHost host) =>
        serviceHost.StopAsync();
}