namespace TehWardy.AI.Providers.Ollama.Models;

internal class OllamaEmbedResponse
{
    public string Model { get; set; }
    public List<List<double>> Embeddings { get; set; }
}