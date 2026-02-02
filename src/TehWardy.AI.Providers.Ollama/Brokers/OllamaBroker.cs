using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TehWardy.AI.Providers.Ollama.Models;

namespace TehWardy.AI.Providers.Ollama.Brokers;

internal class OllamaBroker : IOllamaBroker
{
    private readonly HttpClient client;

    JsonSerializerOptions jsonSerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    public OllamaBroker(OllamaHostConfiguration config)
    {
        client = new()
        {
            BaseAddress = new Uri(config.HostUrl),
            Timeout = TimeSpan.FromMinutes(10)
        };
    }

    public async ValueTask<Stream> SendPromptForStreamingAsync(OllamaPrompt prompt)
    {
        Debug.WriteLine($"[inferrence request]\n{JsonSerializer.Serialize(prompt, jsonSerializerOptions)}");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
        {
            Content = JsonContent.Create(
                inputValue: prompt,
                options: jsonSerializerOptions)
        };

        httpRequest.Headers.Accept.ParseAdd("application/x-ndjson");

        var response = await client.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadAsStreamAsync();
    }

    public async ValueTask<OllamaToken> SendPromptAsync(OllamaPrompt prompt)
    {
        Debug.WriteLine(
            $"[inference request]\n{JsonSerializer.Serialize(prompt, jsonSerializerOptions)}");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
        {
            Content = JsonContent.Create(
                inputValue: prompt,
                options: jsonSerializerOptions)
        };

        // Normal JSON response (NOT ndjson)
        httpRequest.Headers.Accept.ParseAdd("application/json");

        var response = await client.SendAsync(httpRequest);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        Debug.WriteLine($"[inference response]\n{json}");

        return JsonSerializer.Deserialize<OllamaToken>(
            json,
            jsonSerializerOptions
        )!;
    }
}