using System.Text.Json.Serialization;

namespace AIServer.LlamaCpp.Models;

internal class LlamaServerCompletionRequest
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }

    [JsonPropertyName("n_predict")]
    public int NPredict { get; set; } = 512;

    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 0.7f;

    [JsonPropertyName("top_p")]
    public float TopP { get; set; } = 0.9f;

    [JsonPropertyName("min_p")]
    public float MinP { get; set; } = 0.05f;

    [JsonPropertyName("top_k")]
    public int TopK { get; set; } = 40;

    [JsonPropertyName("repeat_penalty")]
    public float RepeatPenalty { get; set; } = 1.1f;

    [JsonPropertyName("stop")]
    public string[] Stop { get; set; }

    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = true;

    [JsonPropertyName("cache_prompt")]
    public bool CachePrompt { get; set; } = true;
}