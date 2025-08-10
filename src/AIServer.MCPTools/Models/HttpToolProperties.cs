using System.Text.Json.Serialization;

namespace AIServer.MCPTools.Models;

public class ToolProperties
{

}

public class HttpToolProperties : ToolProperties
{
    [JsonPropertyName("endpoint")]
    public ToolProperty Endpoint { get; set; }

    [JsonPropertyName("verb")]
    public ToolProperty Verb { get; set; }

    [JsonPropertyName("body")]
    public ToolProperty Body { get; set; }

    [JsonPropertyName("headers")]
    public ToolProperty Headers { get; set; }

    [JsonPropertyName("query")]
    public ToolProperty Query { get; set; }
}

public class ToolProperty
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}
