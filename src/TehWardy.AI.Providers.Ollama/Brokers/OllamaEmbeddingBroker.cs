using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TehWardy.AI.Providers.Ollama.Models;

namespace TehWardy.AI.Providers.Ollama.Brokers;

internal class OllamaEmbeddingBroker : IOllamaEmbeddingBroker
{
    private readonly HttpClient client;
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    public OllamaEmbeddingBroker(OllamaHostConfiguration config)
    {
        client = new()
        {
            BaseAddress = new Uri(config.HostUrl)
        };
    }

    public async ValueTask<OllamaEmbedResponse> EmbedAsync(
        OllamaEmbedRequest ollamaEmbedRequest)
    {
        using var resp = await client
            .PostAsJsonAsync(
                "/api/embed",
                ollamaEmbedRequest,
                jsonSerializerOptions);

        resp.EnsureSuccessStatusCode();

        return await resp.Content
            .ReadFromJsonAsync<OllamaEmbedResponse>(
                jsonSerializerOptions);
    }
}
