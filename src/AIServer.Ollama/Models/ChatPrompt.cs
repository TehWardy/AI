using System.Text.Json.Serialization;

namespace AIServer.Ollama.Models;

public class ChatPrompt
{
    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }

    [JsonPropertyName("messages")]
    public List<MessageData> Messages { get; set; }

    [JsonPropertyName("options")]
    public PromptOptions Options { get; set; }

    [JsonPropertyName("tools")]
    public object Tools { get; set; }
}