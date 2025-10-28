using System.Text.Json.Serialization;

namespace AIServer.LlamaCpp.Models;

internal class Choice
{
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("delta")]
    public Delta Delta { get; set; }
}
