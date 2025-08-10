using System.Text.Json.Serialization;

namespace AIServer.Ollama.Models;

public class ResponseToken
{
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("message")]
    public MessageData Message { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }

    [JsonPropertyName("tool_calls")]
    public ToolCallDetails[] ToolCalls { get; set; }
}
