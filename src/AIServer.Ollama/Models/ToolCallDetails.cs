using System.Text.Json.Serialization;

namespace AIServer.Ollama.Models;

public class ToolCallDetails
{
    [JsonPropertyName("function")]
    public ToolFunctionDetails Function { get; set; }
}
