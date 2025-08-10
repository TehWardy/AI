using System.Text.Json.Serialization;

namespace AIServer.MCPTools.Models;

public class HttpToolFunction : ToolFunction
{
    [JsonPropertyName("parameters")]
    public new HttpToolParameters Parameters { get; set; }
}