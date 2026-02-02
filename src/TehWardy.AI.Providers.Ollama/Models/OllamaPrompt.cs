using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

public class OllamaPrompt
{
    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("messages")]
    public List<OllamaMessageData> Messages { get; set; }

    [JsonPropertyName("options")]
    public OllamaPromptOptions Options { get; set; }

    [JsonPropertyName("tools")]
    public OllamaTool[] Tools { get; set; }
}