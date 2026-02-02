using System.Text.Json.Serialization;

namespace TehWardy.AI.Providers.Ollama.Models;

internal class OllamaModelDetails
{
    [JsonPropertyName("format")]
    public string Format { get; set; }

    [JsonPropertyName("parameter_size")]
    public string ParameterCount { get; set; }

    [JsonPropertyName("quantization_level")]
    public string Quantization { get; set; }

    [JsonPropertyName("family")]
    public string Family { get; set; }
}