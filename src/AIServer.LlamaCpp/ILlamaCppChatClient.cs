using AIServer.LlamaCpp.Models;

namespace AIServer.LlamaCpp;
public interface ILlamaCppChatClient
{
    List<Message> History { get; set; }
    string ModelId { get; set; }

    void SetSystemPrompt(string prompt);
    IAsyncEnumerable<ResponseToken> SendAsync(string userMessage);
}