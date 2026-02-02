using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

internal class OllamaModel
{
    [JsonPropertyName("license")]
    public string License { get; set; }

    [JsonPropertyName("model")]
    public string ModelName { get; set; }

    [JsonPropertyName("modelfile")]
    public string ModelFilePath { get; set; }

    [JsonPropertyName("model_info")]
    public IDictionary<string, object> Parameters { get; set; }

    [JsonPropertyName("details")]
    public OllamaModelDetails Details { get; set; }

    [JsonPropertyName("capabilities")]
    public string[] Capabilities { get; set; }
}