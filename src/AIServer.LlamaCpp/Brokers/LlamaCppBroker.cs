using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AIServer.LlamaCpp.Models;

namespace AIServer.LlamaCpp.Brokers;

internal class LlamaCppBroker : ILlamaCppBroker
{
    private HttpClient llamaClient;

    public async ValueTask<StreamReader> SendCompletionRequestAsync(LlamaServerCompletionRequest request)
    {
        using var req = new HttpRequestMessage(HttpMethod.Post, "/completion")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json")
        };

        req.Headers.Accept.Clear();
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));

        using var response = await llamaClient
            .SendAsync(req, HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content
            .ReadAsStreamAsync();

        return new StreamReader(
            stream: stream,
            encoding: Encoding.UTF8);
    }
}