using AIServer.Llama.Models;

namespace AIServer.Llama.Processings;
public interface ILlamaProcessingService
{
    ValueTask InitializeChatSession(string modelName, string systemPrompt);
    IAsyncEnumerable<string> SendPromptAsync(ChatPrompt prompt);
}