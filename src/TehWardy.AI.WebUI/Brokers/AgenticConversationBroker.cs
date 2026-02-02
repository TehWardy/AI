using System.Net.Http.Json;
using TehWardy.AI.WebUI.Models;

namespace TehWardy.AI.WebUI.Brokers;

internal class AgenticConversationBroker(HttpClient apiClient) : IAgenticConversationBroker
{
    private const string endpoint = "conversation";

    public async ValueTask<Conversation> CreateConversationAsync(Prompt prompt)
    {
        var response = await apiClient
            .PostAsJsonAsync($"{endpoint}/create", prompt);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Conversation>();
    }

    public async ValueTask<Conversation> RetrieveConversationAsync(Guid conversationId)
    {
        var response = await apiClient
            .GetAsync($"{endpoint}/{conversationId}");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Conversation>();
    }

    public async ValueTask<Stream> PostPromptAsync(Prompt prompt)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = JsonContent.Create(inputValue: prompt)
        };

        httpRequest.Headers.Accept.ParseAdd("application/x-ndjson");

        var response = await apiClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead);

        response.EnsureSuccessStatusCode();

        return await response.Content
            .ReadAsStreamAsync();
    }
}
