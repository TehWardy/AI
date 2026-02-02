using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

public class ToolFunctionDetails
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("arguments")]
    public IDictionary<string, object> Arguments { get; set; }
}