using AIServer.Llama.Models;

namespace AIServer.Llama.Foundations;

public interface ILlamaService
{
    IAsyncEnumerable<string> SendPromptAsync(ChatPrompt prompt);
    ValueTask InitializeChatSession(string modelName, string systemPrompt);
}
