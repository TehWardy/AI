using System.Diagnostics;
using AIServer.LlamaCpp.Foundations;
using Microsoft.Extensions.Hosting;

namespace AIServer.LlamaCpp;

public static class IHostExtensions
{
    static Process hostProcess;

    public static async ValueTask StartLlamaHostAsync(this IHost host, string modelName)
    {
        var serviceHost = (ILlamaCppHostService)host.Services
            .GetService(typeof(ILlamaCppHostService));

        hostProcess = await serviceHost.StartAsync(modelName);
    }
}