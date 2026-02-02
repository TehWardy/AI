namespace TehWardy.AI.Providers.Models;

public class EmbeddingRequest
{
    public string ProviderName { get; set; }
    public string ModelName { get; set; }
    public string Input { get; set; }
}