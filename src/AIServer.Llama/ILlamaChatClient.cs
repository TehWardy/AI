using AIServer.Llama.Models;

namespace AIServer.Llama;
public interface ILlamaChatClient
{
    List<MessageData> history { get; set; }
    string ModelId { get; set; }

    void AddSystemMessage();
    IAsyncEnumerable<string> SendAsync(string userMessage);
}