using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

public class OllamaToken
{
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("message")]
    public ResponseMessageData Message { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }

    [JsonPropertyName("done_reason")]
    public string Reason { get; set; }
}