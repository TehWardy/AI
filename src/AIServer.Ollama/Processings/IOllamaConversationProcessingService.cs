using AIServer.Ollama.Models;

namespace AIServer.Ollama.Processings;
internal interface IOllamaConversationProcessingService
{
    IAsyncEnumerable<ResponseToken> SendPromptAsync(ChatPrompt prompt);
}