using AIServer.Llama.Models;

namespace AIServer.Llama;
public interface ILlamaChatClient
{
    List<MessageData> history { get; set; }
    string ModelName { get; set; }

    void AddSystemMessage();
    IAsyncEnumerable<string> SendAsync(string userMessage);
    void LoadModel(string modelName);
}