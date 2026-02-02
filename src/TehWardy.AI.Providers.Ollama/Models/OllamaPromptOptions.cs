using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

public class OllamaPromptOptions
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("num_ctx")]
    public int ContextLength { get; set; }  // name in C# can stay, JSON must be num_ctx
}