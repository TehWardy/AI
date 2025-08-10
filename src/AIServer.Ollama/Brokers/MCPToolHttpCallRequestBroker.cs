namespace AIServer.Ollama.Brokers;

internal class MCPToolHttpCallRequestBroker : IMCPToolHttpCallRequestBroker
{
    HttpClient client;

    public MCPToolHttpCallRequestBroker() =>
        client = new HttpClient();

    public async ValueTask<HttpResponseMessage> SendAsync(HttpRequestMessage request) =>
        await client.SendAsync(request);
}