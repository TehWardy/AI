using System.Text.Json;
using AIServer.AI.Models;

namespace AIServer.AI;

public class AIChatClient(string hostingServerUrl, string model)
{
    readonly List<MessageData> history = [];

    private readonly HttpClient client = new() 
    { 
        BaseAddress = new Uri(hostingServerUrl) 
    };

    public async Task<string> SendMessageAsync(string message)
    {
        history.Add(new MessageData { Role = "user", Content = message });

        var requestBody = new
        {
            model = model,
            messages = history,
            options = new 
            { 
                temperature = 1.0,
                max_tokens = 2048
            }
        };

        var response = await client
            .PostAsJsonAsync("/api/chat", requestBody);

        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var responseJson = $"[{responseString.Trim().Replace("\n", ",")}]";

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var parsedResponse = JsonSerializer
            .Deserialize<ChatResponse[]>(responseJson, options);

        history.AddRange(parsedResponse.Select(i => i.Message));

        return string.Join(" ", parsedResponse.Select(i => i.Message.Content));
    }
}