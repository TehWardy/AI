using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

public sealed class OllamaTool
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "function";

    [JsonPropertyName("function")]
    public OllamaToolFunction Function { get; set; }
}
