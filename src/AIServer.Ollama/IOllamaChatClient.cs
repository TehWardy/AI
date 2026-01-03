using AIServer.Ollama.Models;

namespace AIServer.Ollama;

public interface IOllamaChatClient
{
    List<MessageData> history { get; set; }
    string ModelId { get; set; }

    void AddSystemPrompt(string prompt);
    IAsyncEnumerable<ResponseToken> SendAsync(string userMessage);
}