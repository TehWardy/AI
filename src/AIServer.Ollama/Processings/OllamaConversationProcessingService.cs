using AIServer.Ollama.Foundations;
using AIServer.Ollama.Models;

namespace AIServer.Ollama.Processings;

internal class OllamaConversationProcessingService : IOllamaConversationProcessingService
{
    private readonly IOllamaConversationService conversationService;

    public OllamaConversationProcessingService(IOllamaConversationService conversationService) =>
        this.conversationService = conversationService;

    public IAsyncEnumerable<ResponseToken> SendPromptAsync(ChatPrompt prompt) =>
        conversationService.SendPromptAsync(prompt);
}
