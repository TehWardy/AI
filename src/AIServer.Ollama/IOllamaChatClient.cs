using AIServer.Ollama.Models;

namespace AIServer.Ollama;

public interface IOllamaChatClient
{
    List<MessageData> history { get; set; }
    string ModelId { get; set; }

    IAsyncEnumerable<ResponseToken> SendAsync(string userMessage);
}