using System.Diagnostics;
using AIServer.LlamaCpp.Brokers;
using AIServer.LlamaCpp.Configurations;

namespace AIServer.LlamaCpp.Foundations;

internal class LlamaCppHostService : ILlamaCppHostService
{
    private readonly ILlamaCppHostBroker llamaCppHostBroker;
    private readonly LlamaCppConfiguration config;

    public LlamaCppHostService(
        ILlamaCppHostBroker llamaCppHostBroker,
        LlamaCppConfiguration config)
    {
        this.llamaCppHostBroker = llamaCppHostBroker;
        this.config = config;
    }

    public async ValueTask<Process> StartAsync(string modelName)
    {
        Process llamaServerProcess = await llamaCppHostBroker
            .StartAsync(modelName);

        await WaitForServerReadyAsync();
        return llamaServerProcess;
    }

    async ValueTask WaitForServerReadyAsync(int retries = 60, int delayMs = 500)
    {
        var llamaClient = new HttpClient
        {
            BaseAddress = new Uri($"http://127.0.0.1:{config.ServerPort}"),
            Timeout = TimeSpan.FromMinutes(30)
        };

        await Task.Delay(delayMs);

        for (int i = 0; i < retries; i++)
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, "/version");

            using var rsp = await llamaClient
                .SendAsync(req)
                .ConfigureAwait(false);

            if (rsp.IsSuccessStatusCode)
                return;

            await Task.Delay(delayMs);
        }
    }
}