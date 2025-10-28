using System.Text.Json.Serialization;

namespace AIServer.LlamaCpp.Models;

internal class Delta
{
    [JsonPropertyName("content")] 
    public string Content { get; set; }
}