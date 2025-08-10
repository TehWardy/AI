using System.Text.Json.Serialization;

namespace AIServer.Ollama.Models;

public class PromptOptions
{
    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("num_ctc")]
    public int ContextLength { get; set; }
}
