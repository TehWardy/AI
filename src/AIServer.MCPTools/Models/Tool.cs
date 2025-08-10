using System.Text.Json.Serialization;

namespace AIServer.MCPTools.Models;
public class Tool
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("function")]
    public ToolFunction Function { get; set; }
}

public class HttpTool : Tool
{
    [JsonPropertyName("function")]
    public new HttpToolFunction Function { get; set; }
}
