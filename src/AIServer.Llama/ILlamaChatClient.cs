using AIServer.Llama.Models;

namespace AIServer.Llama;
public interface ILlamaChatClient
{
    IAsyncEnumerable<string> SendAsync(string userMessage);
    IAsyncEnumerable<string> InitializeChatSession(string modelName);
}