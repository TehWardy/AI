using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TehWardy.AI.Providers.Ollama.Models;

namespace TehWardy.AI.Providers.Ollama.Brokers;

internal class OllamaModelBroker : IOllamaModelBroker
{
    private readonly HttpClient client;

    private readonly JsonSerializerOptions jsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    public OllamaModelBroker(OllamaHostConfiguration config)
    {
        client = new HttpClient
        {
            BaseAddress = new Uri(config.HostUrl),
            Timeout = TimeSpan.FromMinutes(30)
        };
    }

    public async ValueTask<OllamaTagsResponse> ListTagsAsync() =>
        await client.GetFromJsonAsync<OllamaTagsResponse>("/api/tags", jsonOptions)
           ?? new OllamaTagsResponse();

    public async ValueTask<OllamaPullResponse> PullAsync(string model)
    {
        using var resp = await client.PostAsJsonAsync(
            "/api/pull",
            new OllamaPullRequest { Model = model, Stream = false, Insecure = false },
            jsonOptions);

        resp.EnsureSuccessStatusCode();

        return await resp.Content.ReadFromJsonAsync<OllamaPullResponse>(jsonOptions)
            ?? new OllamaPullResponse { Status = "unknown" };
    }

    public async ValueTask<OllamaModel> GetModelDetailsAsync(string modelName)
    {
        var response = await client
            .PostAsync("/api/show", JsonContent.Create(new { name = modelName }));

        return await response.Content.ReadFromJsonAsync<OllamaModel>();
    }
}