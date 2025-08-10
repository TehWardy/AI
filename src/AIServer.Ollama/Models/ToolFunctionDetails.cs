using System.Text.Json.Serialization;

namespace AIServer.Ollama.Models;

public class ToolFunctionDetails
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("arguments")]
    public IDictionary<string, object> Arguments { get; set; }
}