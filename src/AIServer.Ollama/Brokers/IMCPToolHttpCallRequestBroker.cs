
namespace AIServer.Ollama.Brokers;

internal interface IMCPToolHttpCallRequestBroker
{
    ValueTask<HttpResponseMessage> SendAsync(HttpRequestMessage request);
}