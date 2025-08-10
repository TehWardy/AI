using System.Text.Json.Serialization;

namespace AIServer.MCPTools.Models;

public class ToolParameters
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";
}

public class HttpToolParameters : ToolParameters
{
    [JsonPropertyName("properties")]
    public HttpToolProperties Properties { get; set; }

    [JsonPropertyName("required")]
    public string[] Required { get; set; }
}
