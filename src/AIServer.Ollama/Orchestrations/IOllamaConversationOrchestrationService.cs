using AIServer.Ollama.Models;

namespace AIServer.Ollama.Orchestrations;
public interface IOllamaConversationOrchestrationService
{
    IAsyncEnumerable<ResponseToken> SendPromptAsync(ChatPrompt prompt);
}