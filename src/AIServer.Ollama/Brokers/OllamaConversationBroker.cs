using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AIServer.Ollama.Configurations;
using AIServer.Ollama.Models;

namespace AIServer.Ollama.Brokers;

internal class OllamaConversationBroker : IOllamaConversationBroker
{
    private readonly HttpClient client;

    JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, 
        PropertyNameCaseInsensitive = true
    };

    public OllamaConversationBroker(OllamaConversationCofiguration config)
    {
        client = new()
        {
            BaseAddress = new Uri(config.OllamaHostUrl),
            Timeout = TimeSpan.FromMinutes(10)
        };
    }

    public async ValueTask<Stream> SendPromptAsync(ChatPrompt prompt)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
        {
            Content = JsonContent.Create(
                inputValue: prompt, 
                options: jsonSerializerOptions)
        };

        httpRequest.Headers.Accept.ParseAdd("application/x-ndjson");

        var response = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadAsStreamAsync();
    }
}