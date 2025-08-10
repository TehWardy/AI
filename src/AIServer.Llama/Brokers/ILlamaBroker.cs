
namespace AIServer.Llama.Brokers;

internal interface ILlamaBroker
{
    IAsyncEnumerable<string> SendAsync(string userMessage);
}