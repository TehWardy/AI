using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

public sealed class OllamaSchema
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("properties")]
    public Dictionary<string, OllamaSchemaProperty> Properties { get; set; } = new();

    [JsonPropertyName("required")]
    public string[] Required { get; set; } = Array.Empty<string>();
}
