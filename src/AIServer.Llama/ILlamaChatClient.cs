namespace AIServer.Llama;

public interface ILlamaChatClient
{
    IAsyncEnumerable<string> SendAsync(string userMessage);
    ValueTask InitializeChatSession(string modelName, string systemPrompt);
}
