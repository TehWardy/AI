using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

public class ResponseMessageData : OllamaMessageData
{
    [JsonPropertyName("thinking")]
    public string Thought { get; set; }

    [JsonPropertyName("tool_name")]
    public string ToolName { get; set; }

    [JsonPropertyName("tool_calls")]
    public ToolCallDetails[] ToolCalls { get; set; }
}