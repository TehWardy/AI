using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

public class OllamaMessageData
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}