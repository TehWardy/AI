using AIServer.Ollama.Brokers;

namespace AIServer.Ollama.Foundations;

internal class MCPToolHttpCallRequestService : IMCPToolHttpCallRequestService
{
    private readonly IMCPToolHttpCallRequestBroker requestBroker;

    public MCPToolHttpCallRequestService(IMCPToolHttpCallRequestBroker requestBroker) =>
        this.requestBroker = requestBroker;

    public async ValueTask<HttpResponseMessage> SendAsync(HttpRequestMessage request) =>
        await requestBroker.SendAsync(request);
}
