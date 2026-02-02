namespace TehWardy.AI.Providers.Ollama.Models;

internal class OllamaEmbedRequest
{
    public string Model { get; set; }
    public string[] Inputs { get; set; }
    public bool? Truncate { get; set; }
    public int? Dimensions { get; set; }
}