using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AIServer.LlamaCpp.Configurations;
using AIServer.LlamaCpp.Models;

namespace AIServer.LlamaCpp.Brokers;

internal class LlamaCppBroker : ILlamaCppBroker
{
    private HttpClient llamaClient;

    public LlamaCppBroker(LlamaCppConfiguration config)
    {
        llamaClient = new HttpClient
        {
            BaseAddress = new Uri($"http://localhost:{config.ServerPort}")
        };
    }

    public async ValueTask<HttpResponseMessage> SendCompletionRequestAsync(LlamaServerCompletionRequest request)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "v1/chat/completions")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(request), 
                Encoding.UTF8, 
                "application/json")
        };

        req.Headers.Accept.Clear();
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        return await llamaClient
            .SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
    }
}