using AIServer.Llama;
using AIServer.Memory;
using LLama.Common;
using Microsoft.AspNetCore.Mvc;

namespace AIServer.Controllers.Api.AI;

[Route("api/chat/llama/")]
public class LlamaChatController(
    IConfiguration config,
    ILlamaChatClient chatClient,
    ConversationHistoryProvider<ChatHistory.Message> conversationProvider)
        : ChatController
{
    [HttpPost("llama/{modelName}/{id}")]
    public async Task PostAsync(
        [FromRoute] string modelName,
        [FromRoute] string id,
        [FromBody] string message)
    {
        string modelFolder = config
            .GetValue<string>("LLMRoot");

        string modelPath =
            $"{modelFolder}{modelName}.gguf";

        var history = await conversationProvider
            .GetConversationAsync(id);

        await Respond(chatClient.SendAsync(message));
    }
}