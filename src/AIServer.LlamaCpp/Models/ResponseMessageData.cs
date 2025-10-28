using System.Text.Json.Serialization;

namespace AIServer.LlamaCpp.Models;

public class ResponseMessageData
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("thinking")]
    public string Thought { get; set; }

    [JsonPropertyName("tool_name")]
    public string ToolName { get; set; }

    //[JsonPropertyName("tool_calls")]
    //public ToolCallDetails[] ToolCalls { get; set; }
}
