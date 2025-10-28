using System.Text.Json.Serialization;

namespace AIServer.LlamaCpp.Models;

public class ResponseToken
{
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("message")]
    public ResponseMessageData Message { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }
}
