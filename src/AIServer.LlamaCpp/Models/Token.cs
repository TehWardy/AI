using System.Text.Json.Serialization;

namespace AIServer.LlamaCpp.Models;

internal class Token
{
    [JsonPropertyName("choices")]
    public Choice[] Choices { get; set; }

    [JsonPropertyName("object")]
    public string Object { get; set; }
}
