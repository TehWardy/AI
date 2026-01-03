using AIServer.Llama.Models;

namespace AIServer.Llama.Foundations;

internal interface ILlamaService
{
    IAsyncEnumerable<string> SendPromptAsync(ChatPrompt prompt);
    ValueTask InitializeChatSession(string modelName, string systemPrompt);
}
