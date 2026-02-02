using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

public class ToolCallDetails
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("function")]
    public ToolFunctionDetails Function { get; set; }
}
