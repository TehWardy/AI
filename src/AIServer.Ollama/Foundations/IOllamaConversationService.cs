using AIServer.Ollama.Models;

namespace AIServer.Ollama.Foundations;
internal interface IOllamaConversationService
{
    IAsyncEnumerable<ResponseToken> SendPromptAsync(ChatPrompt prompt);
}