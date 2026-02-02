using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

public sealed class OllamaToolFunction
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("parameters")]
    public OllamaSchema Parameters { get; set; }
}
