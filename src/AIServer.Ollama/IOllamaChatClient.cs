using AIServer.Ollama.Models;

namespace AIServer.Llama;
public interface IOllamaChatClient
{
    List<MessageData> history { get; set; }
    string ModelId { get; set; }
    IEnumerable<IDictionary<string, object>> Tools { get; set; }

    IAsyncEnumerable<ResponseToken> SendAsync(string userMessage);
}