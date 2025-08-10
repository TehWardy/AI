using AIServer.Llama;
using AIServer.Llama.Models;
using AIServer.MCPTools.Models;
using AIServer.Memory;
using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers.Api.AI;

[Route("api/chat/ollama/")]
public class OllamaChatController(
    OllamaChatClient chatClient,
    ConversationHistoryProvider<MessageData> conversationHistory) 
        : ChatController
{
    [HttpPost("{modelId}/{id}")]
    public async Task PostAsync(
        [FromRoute] string modelId,
        [FromRoute] string id,
        [FromBody] string message)
    {
        var api = new HttpClient()
        {
            BaseAddress = new Uri("https://localhost:7181/api/tools/")
        };

        chatClient.ModelId = modelId;

        chatClient.history = await conversationHistory
            .GetConversationAsync(id);

        chatClient.Tools = await api
            .GetFromJsonAsync<IEnumerable<IDictionary<string, object>>>(
                "from-swagger-spec?websiteRootUri=https://localhost:7181/&fromPath=/api/tools");

        await Respond(chatClient.SendAsync(message));

        await conversationHistory
            .UpdateConversationHistoryAsync(id, chatClient.history);
    }
}