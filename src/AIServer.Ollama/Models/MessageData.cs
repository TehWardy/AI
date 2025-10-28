using System.Text.Json.Serialization;

namespace AIServer.Ollama.Models;

public class MessageData
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}
