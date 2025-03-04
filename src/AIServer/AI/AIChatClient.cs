using System.Text.Json;
using System.Xml.Linq;
using AIServer.AI.Models;

namespace AIServer.AI;

public class AIChatClient(string hostingServerUrl, string model)
{
    readonly List<MessageData> history = [];

    private readonly HttpClient client = new() 
    { 
        BaseAddress = new Uri(hostingServerUrl) 
    };

    public async Task<dynamic> SendMessageAsync(string message)
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

        var responseParts = parsedResponse
            .Select(i => i.Message.Content);

        var thoughtAndReplyXML = $"<message>{string.Join("", responseParts)}</message>";
        XElement root = XElement.Parse(thoughtAndReplyXML);
        var thought = root.Element("think").Value;
        root.Element("think").Remove();
        var reply = root.Value;

        return new 
        {
            thought = root.Element("think").Value,
            reply = reply
        };
    }
}