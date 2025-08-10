
namespace AIServer.Ollama.Foundations;

internal interface IMCPToolHttpCallRequestService
{
    ValueTask<HttpResponseMessage> SendAsync(HttpRequestMessage request);
}