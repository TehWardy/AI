namespace AIServer.Ollama.Models;

public class HttpRequestDetails
{
    public IDictionary<string, string> Headers { get; set; }
    public HttpMethod HttpVerb { get; set; }
    public string Url { get; set; }
    public string Body { get; set; }
}
