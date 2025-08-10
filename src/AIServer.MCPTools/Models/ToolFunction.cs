using System.Text.Json.Serialization;

namespace AIServer.MCPTools.Models;

public class ToolFunction
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("parameters")]
    public object Parameters { get; set; }
}
