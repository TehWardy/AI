using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

public sealed class OllamaSchemaProperty
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("required")]
    public string[] Required { get; set; }

    // Only include when Type == "object"
    [JsonPropertyName("properties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, OllamaSchemaProperty> Properties { get; set; }

    // Only include when Type == "array"
    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OllamaSchemaProperty Items { get; set; }
}