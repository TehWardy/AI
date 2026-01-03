using AIServer.LlamaCpp.Foundations;
using Microsoft.Extensions.Hosting;

namespace AIServer.LlamaCpp;

public static class IHostExtensions
{
    static ILlamaCppHostService serviceHost;

    public static async IAsyncEnumerable<string> StartLlamaHostAsync(this IHost host, string modelName)
    {
        if (serviceHost is null)
        {
            serviceHost = (ILlamaCppHostService)host.Services
                .GetService(typeof(ILlamaCppHostService));

            IAsyncEnumerable<string> llamaCppConsoleStream =
                serviceHost.StartAsync(modelName);

            await foreach (string consoleLine in llamaCppConsoleStream)
                yield return consoleLine;
        }
    }

    public static async ValueTask StopLlamaHostAsync(this IHost host)
    {
        if (serviceHost is not null)
        {
            await serviceHost.StopAsync();
            serviceHost = null;
        }
    }
}