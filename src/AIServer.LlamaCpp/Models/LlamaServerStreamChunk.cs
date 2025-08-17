using System.Text.Json.Serialization;

namespace AIServer.LlamaCpp.Models;

internal class LlamaServerStreamChunk
{
    [JsonPropertyName("content")] 
    public string Content { get; set; }

    [JsonPropertyName("stop")] 
    public bool? Stop { get; set; }
}